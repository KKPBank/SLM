using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Transactions;
using System.IO;
using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;

namespace SLMBatch.Business
{
    public class OBT_PRO_07_Biz : ServiceBase, IDisposable
    {       
        /// <summary>
        /// Update ข้อมูล Payment จาก DataWarehouse ไปที่ OBT
        /// </summary>
        /// <param name="batchCode"></param>
        
        enum eBDW
        {
            CONTRACT_NUMBER = 0,
            TRAN_CODE,
            INS_NO_DESC,
            INST_NO,
            REC_BY,
            REC_NO,
            REC_AMT,
            TRAN_DATE,
            UPDATEDATE,
            DATA_SOURCE
        }

        public OBT_PRO_07_Biz()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
        }

        //Start Batch Here
        public bool UpdatePayment(string batchCode)
        {
            OracleDB db = null;
            SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
            string[] statusExclude = new string[] { "08", "09", "10" };
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;
            bool ret = false;

            try
            {
                BatchCode = batchCode;
                BatchMonitorId = BizUtil.SetStartTime(batchCode);

                BizUtil.CheckPrerequisite(batchCode);

                db = new OracleDB(AppConstant.DWHConnectionString);

                //Check ว่าต้นทางมีข้อมูลพร้อมให้ load หรือยัง
                string zData_Date = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
                string sqlcheck = @"SELECT ZDATA_DATE, ZPROCESS_ID, ZSTART_DATE, MAX (ZSTART_TIME) AS ZSTART_TIME, ZFINISH_DATE, MAX (ZFINISH_TIME) AS ZFINISH_TIME, ZFINISH_FLG 
                                    FROM " + AppConstant.DWHSchema + @".ZBW_PROC_STAT 
                                    WHERE zprocess_id = 'OBT_IN' AND zfinish_flg = 'Y' AND ZDATA_DATE = '" + zData_Date + @"' 
                                    GROUP BY ZDATA_DATE, ZPROCESS_ID, ZSTART_DATE, ZFINISH_DATE, ZFINISH_FLG ";

                DataTable dtCheck = db.ExcecuteTable(sqlcheck);
                if (dtCheck.Rows.Count == 0)
                {
                    //Console.WriteLine("Datawarehouse : Pending Datadate " + zData_Date);
                    throw new Exception("ข้อมูลบนระบบ Datawarehouse ยังไม่พร้อมใช้งาน");
                }

                int numOfDay = AppConstant.DWHNumOfDay;
                string datefrom = DateTime.Today.AddDays(-numOfDay).Year.ToString() + DateTime.Today.AddDays(-numOfDay).ToString("MMdd");
                string dateto = DateTime.Today.Year.ToString() + DateTime.Today.ToString("MMdd");

                //Get Data From DW  OBT_INSURANCE_RECEIPT
                string sql_bdw = string.Format("SELECT REC.*, 'BDW' AS DATA_SOURCE FROM {0}.OBT_INSURANCE_RECEIPT REC WHERE REC.TRAN_CODE IN ('204','205') AND REC.UPDATEDATE BETWEEN '{1}' AND '{2}' ", AppConstant.DWHSchema, datefrom, dateto);
                DataTable dtBDW = db.ExcecuteTable(sql_bdw);
                DataTable dtCopy = dtBDW.Copy();

                var pendingList = slmdb.kkslm_tr_renewinsurance_hp_payment_pending.Where(p => p.is_Deleted == false).ToList();
                foreach (kkslm_tr_renewinsurance_hp_payment_pending item in pendingList)
                {
                    decimal recAmount = item.slm_Rec_Amount != null ? item.slm_Rec_Amount.Value : 0;

                    int count = dtCopy.AsEnumerable().Where(p => p[eBDW.CONTRACT_NUMBER.ToString()].ToString().Trim() == item.slm_Contract_Number
                                                            && p[eBDW.TRAN_CODE.ToString()].ToString().Trim() == item.slm_Tran_Code
                                                            && p[eBDW.REC_NO.ToString()].ToString().Trim() == item.slm_Rec_No
                                                            && (p[eBDW.REC_AMT.ToString()].ToString().Trim() != "" ? Convert.ToDecimal(p[eBDW.REC_AMT.ToString()]) : 0) == recAmount).Count();

                    if (count == 0)
                    {
                        DataRow dr = dtBDW.NewRow();
                        dr[eBDW.CONTRACT_NUMBER.ToString()] = item.slm_Contract_Number;
                        dr[eBDW.INS_NO_DESC.ToString()] = item.slm_Ins_No_Desc;
                        dr[eBDW.INST_NO.ToString()] = item.slm_Inst_No;
                        dr[eBDW.REC_AMT.ToString()] = item.slm_Rec_Amount;
                        dr[eBDW.REC_BY.ToString()] = item.slm_Rec_by;
                        dr[eBDW.REC_NO.ToString()] = item.slm_Rec_No;
                        dr[eBDW.TRAN_CODE.ToString()] = item.slm_Tran_Code;
                        dr[eBDW.TRAN_DATE.ToString()] = item.slm_Tran_Date;
                        dr[eBDW.UPDATEDATE.ToString()] = item.slm_Update_Date;
                        dr[eBDW.DATA_SOURCE.ToString()] = "PENDING";
                        dtBDW.Rows.Add(dr);
                    }
                }

                var paymentList = ConvretToList(dtBDW);

                int countTotalBDW = paymentList.Count(p => p.DataSource == "BDW");
                int countTotalPending = paymentList.Count(p => p.DataSource == "PENDING");

                //Snap data from datasource
                //Console.Write("Snap DataSource: ");
                SnapDataSource(slmdb, paymentList);
                //Console.WriteLine("Done");

                //Remove transactions which already exist in SLMDB
                //Console.Write("Remove Existing Payments: ");
                RemoveExistingPayment(slmdb, paymentList);
                //Console.WriteLine("Done");

                //Log amount of actual data to process
                int countBDW = paymentList.Count(p => p.DataSource == "BDW");
                int countPending = paymentList.Count(p => p.DataSource == "PENDING");
                string msg = string.Format("Data from BDW {0}/{1} (Total/To be Processed) records, Data from Pending {2}/{3} (Total/To be Processed) records", countTotalBDW.ToString(), countBDW.ToString(), countTotalPending.ToString(), countPending.ToString());
                Util.WriteLogFile(logfilename, BatchCode, msg);
                BizUtil.InsertLog(BatchMonitorId, "", "", msg);

                totalRecord = paymentList.Count;
                var contractNoList = paymentList.Select(p => p.ContractNumber).Distinct().ToList();
                var configProductPaymentList = slmdb.kkslm_ms_config_product_payment.Where(p => p.is_Deleted == false && (p.slm_PaymentCode == AppConstant.PaymentCode.Policy || p.slm_PaymentCode == AppConstant.PaymentCode.Act)).ToList();

                ClearPaymentTemp();

                List<InsuranceCompanyData> companyList = GetInsuranceCompany(slmdb);

                string ticketId = "";
                foreach (string contractNo in contractNoList)
                {
                    ticketId = "";
                    try
                    {
                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                        {
                            using (SLMDBEntities slm_db = AppUtil.GetSlmDbEntities())
                            {
                                //Case1
                                List<kkslm_tr_renewinsurance> renewList = (from r in slm_db.kkslm_tr_renewinsurance
                                                                              join l in slm_db.kkslm_tr_lead on r.slm_TicketId equals l.slm_ticketId
                                                                              where r.slm_ContractNo == contractNo && l.is_Deleted == 0
                                                                              && statusExclude.Contains(l.slm_Status) == false
                                                                              select r).ToList();
                                if (renewList.Count() > 0)
                                {
                                    if (renewList.Count() > 1)
                                    {
                                        //pending
                                        InsertPaymentPending(paymentList, contractNo, slm_db, AppConstant.PendingType.DuplicateTicketId);

                                        string ticketIds = "";
                                        renewList.ForEach(p => { ticketIds += (ticketIds != "" ? ", " : "") + p.slm_TicketId; });

                                        string errMessage = string.Format("ContractNo : {0}, GoToPending, มี TicketId มากกว่า 1 รายการ ({1})", contractNo, ticketIds);
                                        Util.WriteLogFile(logfilename, BatchCode, errMessage);
                                        BizUtil.InsertLog(BatchMonitorId, "", "", errMessage);
                                        //Console.WriteLine("ContractNo " + contractNo + ": PENDING");
                                    }
                                    else if (renewList.Count() == 1)
                                    {
                                        //normal flow
                                        var renew_ins = renewList.FirstOrDefault();
                                        ticketId = renew_ins.slm_TicketId;

                                        var lead = slm_db.kkslm_tr_lead.Where(p => p.slm_ticketId == renew_ins.slm_TicketId).FirstOrDefault();
                                        var payment_list = paymentList.Where(p => p.ContractNumber == contractNo).ToList();

                                        CheckProductPaymentConfig(lead.slm_ticketId, configProductPaymentList, lead.slm_Product_Id, contractNo, payment_list, slm_db);
                                        if (payment_list.Count > 0)
                                        {
                                            CaseNormal(lead, renew_ins, payment_list, contractNo, slm_db, companyList);
                                        }
                                        //Console.WriteLine("ContractNo " + contractNo + ": SUCCESS");
                                    }
                                }
                                else
                                {
                                    int count = 0;
                                    string errMsg = "";
                                    AppConstant.PendingType pendingType;
                                    var leads = (from r in slm_db.kkslm_tr_renewinsurance
                                                    join l in slm_db.kkslm_tr_lead on r.slm_TicketId equals l.slm_ticketId
                                                    where r.slm_ContractNo == contractNo && l.is_Deleted == 0
                                                    && statusExclude.Contains(l.slm_Status) == true
                                                    select l).ToList();

                                    if (leads.Count > 0)    
                                    {
                                        pendingType = AppConstant.PendingType.InappropriateLeadStatus;
                                        errMsg = string.Format("ContractNo : {0}, GoToPending : สถานะของ Lead in Cancel, Reject or Success", contractNo);
                                    }
                                    else
                                    {
                                        count = slm_db.kkslm_tr_prelead.Where(p => p.slm_Contract_Number == contractNo).Count();
                                        if (count > 0)
                                        {
                                            pendingType = AppConstant.PendingType.ContractNoInPreleadOnly;
                                            errMsg = string.Format("ContractNo : {0}, GoToPending : มีแต่ใน Prelead แต่ไม่มีใน Lead", contractNo);
                                        }
                                        else
                                        {
                                            pendingType = AppConstant.PendingType.ContractNoNotFound;
                                            errMsg = string.Format("ContractNo : {0}, GoToPending : ไม่พบ ContractNo ในระบบ", contractNo);
                                        }
                                    }

                                    InsertPaymentPending(paymentList, contractNo, slm_db, pendingType);
                                    Util.WriteLogFile(logfilename, BatchCode, errMsg);
                                    BizUtil.InsertLog(BatchMonitorId, "", "", errMsg);
                                    //Console.WriteLine("ContractNo " + contractNo + ": PENDING");
                                }
                            }

                            ts.Complete();
                            totalSuccess += paymentList.Count(p => p.ContractNumber == contractNo);
                        }
                    }
                    catch (Exception ex)
                    {
                        totalFail += paymentList.Count(p => p.ContractNumber == contractNo);
                        string message = "ContractNo : " + contractNo + ", Error : " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);

                        Util.WriteLogFile(logfilename, BatchCode, message);
                        BizUtil.InsertLog(BatchMonitorId, ticketId, "", message);
                        //Console.WriteLine("ContractNo " + contractNo + ": FAIL");
                    }
                }

                ret = true;
                BizUtil.SetEndTime(BatchCode, BatchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                ret = false;
                //Console.WriteLine("All FAIL");

                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Util.WriteLogFile(logfilename, BatchCode, message);
                BizUtil.InsertLog(BatchMonitorId, "", "", message);

                totalSuccess = 0;
                totalFail = totalRecord;

                BizUtil.SetEndTime(BatchCode, BatchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
            finally
            {
                if (db != null)
                {
                    db.CloseConnection();
                }
                if (slmdb != null)
                {
                    slmdb.Dispose();
                }
            }

            return ret;
        }

        private void CheckProductPaymentConfig(string ticketId, List<kkslm_ms_config_product_payment> configProductPaymentList, string productId, string contractNumber, List<TransactionData> paymentList, SLMDBEntities slm_db)
        {
            try
            {
                List<TransactionData> pendingList = new List<TransactionData>();
                foreach (TransactionData data in paymentList)
                {
                    int count = configProductPaymentList.Where(p => p.slm_Product_Id == productId && p.slm_PaymentCode == data.TranCode).Count();
                    if (count == 0)
                    {
                        pendingList.Add(data);

                        Util.WriteLogFile(logfilename, BatchCode, string.Format("ContractNo : {0}, GoToPending : ไม่พบ config product payment, ProductId {1}, PaymentCode {2}", contractNumber, productId, data.TranCode));
                        BizUtil.InsertLog(BatchMonitorId, ticketId, "", string.Format("ContractNo : {0}, GoToPending : ไม่พบ config product payment, ProductId {1}, PaymentCode {2}", contractNumber, productId, data.TranCode));
                    }
                }

                if (pendingList.Count > 0)
                {
                    pendingList.ForEach(p => paymentList.Remove(p));
                    InsertPaymentPending(pendingList, contractNumber, slm_db, AppConstant.PendingType.ConfigPaymentNotFound);

                    //Console.WriteLine("ContractNo " + contractNumber + ": PENDING");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<TransactionData> ConvretToList(DataTable dtBDW)
        {
            try
            {
                List<TransactionData> list = new List<TransactionData>();
                dtBDW.AsEnumerable().ToList().ForEach(p => {
                    TransactionData data = new TransactionData()
                    {
                        ContractNumber = p[eBDW.CONTRACT_NUMBER.ToString()].ToString().Trim(),
                        TranCode = p[eBDW.TRAN_CODE.ToString()].ToString().Trim(),
                        InsNoDesc = p[eBDW.INS_NO_DESC.ToString()].ToString().Trim(),
                        InstNo = p[eBDW.INST_NO.ToString()].ToString().Trim(),
                        RecBy = p[eBDW.REC_BY.ToString()].ToString().Trim(),
                        RecNo = p[eBDW.REC_NO.ToString()].ToString().Trim(),
                        TranDate = p[eBDW.TRAN_DATE.ToString()].ToString().Trim(),
                        UpdateDate = p[eBDW.UPDATEDATE.ToString()].ToString().Trim(),
                        DataSource = p[eBDW.DATA_SOURCE.ToString()].ToString().Trim()
                    };
                    if (p[eBDW.REC_AMT.ToString()].ToString().Trim() != "")
                    {
                        data.RecAmount = Convert.ToDecimal(p[eBDW.REC_AMT.ToString()]);
                    }
                    list.Add(data);
                });

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RemoveExistingPayment(SLMDBEntities slmdb, List<TransactionData> list)
        {
            //Remove transactions which already exist in SLMDB
            try
            {
                List<TransactionData> removeList = new List<TransactionData>();
                foreach (TransactionData data in list)
                {
                    int count = slmdb.kkslm_tr_renewinsurance_receipt_detail.Count(p => p.slm_PaymentCode == data.TranCode
                                    && p.slm_RecNo == data.RecNo && p.slm_RecAmount == data.RecAmount);

                    if (count > 0)
                    {
                        removeList.Add(data);
                    }
                }

                removeList.ForEach(p => list.Remove(p));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SnapDataSource(SLMDBEntities slmdb, List<TransactionData> paymentList)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    DateTime createdDate = DateTime.Now;

                    paymentList.ForEach(p => {
                        kkslm_tr_renewinsurance_hp_payment_snap snap = new kkslm_tr_renewinsurance_hp_payment_snap()
                        {
                            slm_CreatedDate = createdDate,
                            slm_CreatedBy = "SYSTEM",
                            slm_Contract_Number = p.ContractNumber,
                            slm_Ins_No_Desc = p.InsNoDesc,
                            slm_Inst_No = p.InstNo,
                            slm_Rec_Amount = p.RecAmount,
                            slm_Rec_by = p.RecBy,
                            slm_Rec_No = p.RecNo,
                            slm_Tran_Code = p.TranCode,
                            slm_Tran_Date = p.TranDate,
                            slm_Update_Date = p.UpdateDate,
                            slm_Source = p.DataSource
                        };
                        slmdb.kkslm_tr_renewinsurance_hp_payment_snap.AddObject(snap);
                    });

                    slmdb.SaveChanges();
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                string message = "Snap datasource failed because " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                Util.WriteLogFile(logfilename, BatchCode, message);
                BizUtil.InsertLog(BatchMonitorId, "", "", message);
            }
        }

        private void InsertPaymentPending(List<TransactionData> paymentList, string ContractNumber, SLMDBEntities slmdb, AppConstant.PendingType pendingType)
        {
            try
            {
                var list = paymentList.Where(p => p.ContractNumber == ContractNumber).ToList();
                DateTime createdDate = DateTime.Now;

                //Delete old record
                slmdb.kkslm_tr_renewinsurance_hp_payment_pending.Where(p => p.slm_Contract_Number == ContractNumber).ToList().ForEach(slmdb.kkslm_tr_renewinsurance_hp_payment_pending.DeleteObject);
                //================================
                //Insert new record
                foreach (TransactionData data in list)
                {
                    kkslm_tr_renewinsurance_hp_payment_pending pending = new kkslm_tr_renewinsurance_hp_payment_pending()
                    {
                        slm_Contract_Number = data.ContractNumber,
                        slm_Tran_Code = data.TranCode,
                        slm_Ins_No_Desc = data.InsNoDesc,
                        slm_Inst_No = data.InstNo,
                        slm_Rec_by = data.RecBy,
                        slm_Rec_No = data.RecNo,
                        slm_Rec_Amount = data.RecAmount,
                        slm_Tran_Date = data.TranDate,
                        slm_Update_Date = data.UpdateDate,
                        slm_Type = Convert.ToInt32(pendingType),
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createdDate,
                        is_Deleted = false
                    };
                    slmdb.kkslm_tr_renewinsurance_hp_payment_pending.AddObject(pending);

                    kkslm_tr_renewinsurance_hp_payment_temp temp = new kkslm_tr_renewinsurance_hp_payment_temp()
                    {
                        slm_Contract_Number = data.ContractNumber,
                        slm_Tran_Code = data.TranCode,
                        slm_Ins_No_Desc = data.InsNoDesc,
                        slm_Inst_No = data.InstNo,
                        slm_Rec_by = data.RecBy,
                        slm_Rec_No = data.RecNo,
                        slm_Rec_Amount = data.RecAmount,
                        slm_Tran_Date = data.TranDate,
                        slm_Update_Date = data.UpdateDate,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createdDate,
                        is_Deleted = false
                    };
                    slmdb.kkslm_tr_renewinsurance_hp_payment_temp.AddObject(temp);
                }
                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CaseNormal(kkslm_tr_lead lead, kkslm_tr_renewinsurance renew_ins, List<TransactionData> paymentList, string contractNumber, SLMDBEntities slmdb, List<InsuranceCompanyData> companyList)
        {
            try
            {
                DateTime createdDate = DateTime.Now;

                int countPolicy = slmdb.kkslm_tr_renewinsurance_compare.Count(p => p.slm_RenewInsureId == renew_ins.slm_RenewInsureId && p.slm_Selected == true);
                int countAct = slmdb.kkslm_tr_renewinsurance_compare_act.Count(p => p.slm_RenewInsureId == renew_ins.slm_RenewInsureId && p.slm_ActPurchaseFlag == true);

                List<TransactionData> rowsPayment = null;
                decimal? current_paid_policy = null;
                decimal? current_paid_act = null;
                decimal? total_paid_policy = null;
                decimal? total_paid_act = null;

                if (countPolicy > 0)    
                {
                    rowsPayment = paymentList.Where(p => p.TranCode == AppConstant.PaymentCode.Policy).ToList();
                    if (rowsPayment.Count > 0)
                    {
                        DoProcessPolicy(renew_ins, lead, rowsPayment, slmdb, createdDate, out current_paid_policy, companyList);
                    }

                    rowsPayment = paymentList.Where(p => p.TranCode == AppConstant.PaymentCode.Act).ToList();
                    if (countAct > 0)
                    {
                        if (rowsPayment.Count > 0)
                        {
                            DoProcessAct(renew_ins, lead, rowsPayment, slmdb, createdDate, true, out current_paid_act);
                        }
                    }
                    else
                    {
                        if (rowsPayment.Count > 0)
                        {
                            InsertReceipt(slmdb, lead.slm_ticketId, rowsPayment, createdDate);
                            current_paid_act = rowsPayment.Sum(p => p.RecAmount == null ? 0 : p.RecAmount.Value);
                        }
                    }
                }
                else if (countPolicy == 0 && countAct > 0) 
                {
                    rowsPayment = paymentList.Where(p => p.TranCode == AppConstant.PaymentCode.Act).ToList();
                    if (rowsPayment.Count > 0)
                    {
                        DoProcessAct(renew_ins, lead, rowsPayment, slmdb, createdDate, false, out current_paid_act);
                    }

                    rowsPayment = paymentList.Where(p => p.TranCode == AppConstant.PaymentCode.Policy).ToList();
                    if (rowsPayment.Count > 0)
                    {
                        InsertReceipt(slmdb, lead.slm_ticketId, rowsPayment, createdDate);
                        current_paid_policy = rowsPayment.Sum(p => p.RecAmount == null ? 0 : p.RecAmount.Value);
                    }
                }
                else
                {
                    //ไม่มี flag การซื้อประกันและพรบ. ให้นำใบเสร็จเข้าระบบ
                    rowsPayment = paymentList.Where(p => p.TranCode == AppConstant.PaymentCode.Policy).ToList();
                    if (rowsPayment.Count > 0)
                    {
                        InsertReceipt(slmdb, lead.slm_ticketId, rowsPayment, createdDate);
                        current_paid_policy = rowsPayment.Sum(p => p.RecAmount == null ? 0 : p.RecAmount.Value);
                    }

                    rowsPayment = paymentList.Where(p => p.TranCode == AppConstant.PaymentCode.Act).ToList();
                    if (rowsPayment.Count > 0)
                    {
                        InsertReceipt(slmdb, lead.slm_ticketId, rowsPayment, createdDate);
                        current_paid_act = rowsPayment.Sum(p => p.RecAmount == null ? 0 : p.RecAmount.Value);
                    }
                }

                var leadData = GetLeadData(slmdb, lead.slm_ticketId);
                //Get เงินรวม
                total_paid_policy = CalculateTotalPaidAmount(slmdb, renew_ins.slm_TicketId, AppConstant.PaymentCode.Policy);
                total_paid_act = CalculateTotalPaidAmount(slmdb, renew_ins.slm_TicketId, AppConstant.PaymentCode.Act);

                InsertPrepareEmail(slmdb, lead, paymentList, createdDate, countPolicy > 0, countAct > 0, total_paid_policy, total_paid_act, leadData);

                int phonecallId = InsertPhonecall(slmdb, lead, paymentList, createdDate, countPolicy > 0, countAct > 0, total_paid_policy, total_paid_act, leadData);
                CreateCASActivityLog(renew_ins, slmdb, phonecallId, countPolicy > 0, countAct > 0, current_paid_policy, current_paid_act, total_paid_policy, total_paid_act);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private LeadData GetLeadData(SLMDBEntities slmdb, string ticketId)
        {
            try
            {
                string sql = @"SELECT lead.slm_ticketId AS TicketId, renew.slm_ContractNo AS ContractNo, ISNULL(title.slm_TitleName, '') + lead.slm_Name + ' ' + ISNULL(lead.slm_LastName, '') AS CustomerName
                                , staff.slm_BranchCode AS BranchCode, ownerbranch.slm_BranchName AS BranchName
                                , LEAD.slm_Owner AS Owner, STAFF.slm_StaffEmail AS OwnerEmail, LEAD.slm_Delegate AS Delegate, STAFF2.slm_StaffEmail AS DelegateEmail
                                , renew.slm_ReceiveNo AS ReceiveNo, campaign.slm_CampaignName AS CampaignName
                                , renew.slm_IncentiveFlag AS IncentiveFlag, renew.slm_ActIncentiveFlag AS IncentiveFlagAct, renew.slm_ActSendDate AS ActSendDate
                                , opt.slm_OptionDesc AS StatusDesc, lead.slm_ExternalSubStatusDesc AS SubStatusDesc
                                FROM dbo.kkslm_tr_lead LEAD
                                LEFT JOIN dbo.kkslm_tr_renewinsurance renew ON renew.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN dbo.kkslm_ms_campaign campaign ON campaign.slm_CampaignId = lead.slm_CampaignId
                                LEFT JOIN dbo.kkslm_ms_staff STAFF ON LEAD.slm_Owner = STAFF.slm_UserName AND STAFF.is_Deleted = 0
                                LEFT JOIN dbo.kkslm_ms_staff STAFF2 ON LEAD.slm_Delegate = STAFF2.slm_UserName AND STAFF2.is_Deleted = 0
                                LEFT JOIN dbo.kkslm_ms_branch ownerbranch ON ownerbranch.slm_BranchCode = staff.slm_BranchCode 
                                LEFT JOIN dbo.kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId
                                LEFT JOIN dbo.kkslm_ms_option opt ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' AND opt.is_Deleted = 0 
                                WHERE LEAD.slm_ticketId = '" + ticketId + "' ";

                var data = slmdb.ExecuteStoreQuery<LeadData>(sql).FirstOrDefault();
                if (data == null)
                {
                    throw new Exception("ไม่พบ TicketId " + ticketId + " ในระบบ");
                }
                else
                {
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int InsertPhonecall(SLMDBEntities slmdb, kkslm_tr_lead lead, List<TransactionData> paymentList, DateTime createdDate, bool policyPurchased, bool actPurchased, decimal? total_paid_policy, decimal? total_paid_act, LeadData data)
        {
            try
            {
                //insert phonecall
                string content = "";

                content += "เลขที่รับแจ้ง:" + (string.IsNullOrEmpty(data.ReceiveNo) ? "" : (" " + data.ReceiveNo));
                if (data.ActSendDate != null)
                {
                    content += ", วันที่ส่งแจ้ง พรบ.: " + data.ActSendDate.Value.ToString("dd/MM/") + data.ActSendDate.Value.Year.ToString();
                }
                else
                {
                    content += ", วันที่ส่งแจ้ง พรบ.:";
                }

                //Incentive
                if (policyPurchased)
                    content += ", Incentive ประกัน: " + (data.IncentiveFlag == true ? "Y" : "N");
                else
                    content += ", Incentive ประกัน:";

                if (actPurchased)
                    content += ", Incentive พรบ.: " + (data.IncentiveFlagAct == true ? "Y" : "N");
                else
                    content += ", Incentive พรบ.:";

                //ค่าเบี้ยประกันที่ลูกค้าชำระ
                if (paymentList.Where(a => a.TranCode == AppConstant.PaymentCode.Policy).Count() > 0)
                {
                    decimal policyPaidAmount = paymentList.Where(a => a.TranCode == AppConstant.PaymentCode.Policy)
                                                            .Sum(b => b.RecAmount == null ? 0 : b.RecAmount.Value);

                    content += ", ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้: " + policyPaidAmount.ToString("#,##0.00");
                }
                else
                    content += ", ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้: ";

                //ค่าเบี้ยพรบ.ที่ลูกค้าชำระ
                if (paymentList.Where(a => a.TranCode == AppConstant.PaymentCode.Act).Count() > 0)
                {
                    decimal actPaidAmount = paymentList.Where(a => a.TranCode == AppConstant.PaymentCode.Act)
                                                        .Sum(b => b.RecAmount == null ? 0 : b.RecAmount.Value);

                    content += ", ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้: " + actPaidAmount.ToString("#,##0.00");
                }
                else
                    content += ", ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้: ";

                //ค่าเบี้ยชำระรวม
                content += ", ค่าเบี้ยประกันที่ลูกค้าชำระรวม: " + (total_paid_policy != null ? total_paid_policy.Value.ToString("#,##0.00") : "");
                content += ", ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม: " + (total_paid_act != null ? total_paid_act.Value.ToString("#,##0.00") : "");

                //if (policyPurchased)
                //    content += ", ค่าเบี้ยประกันที่ลูกค้าชำระรวม: " + (total_paid_policy != null ? total_paid_policy.Value.ToString("#,##0.00") : "");
                //else
                //    content += ", ค่าเบี้ยประกันที่ลูกค้าชำระรวม:";

                //if (actPurchased)
                //    content += ", ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม: " + (total_paid_act != null ? total_paid_act.Value.ToString("#,##0.00") : "");
                //else
                //    content += ", ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม:";

                kkslm_phone_call phone = new kkslm_phone_call()
                {
                    slm_TicketId = lead.slm_ticketId,
                    slm_Status = lead.slm_Status,
                    slm_SubStatus = lead.slm_SubStatus,
                    slm_SubStatusName = lead.slm_ExternalSubStatusDesc,
                    slm_Owner = lead.slm_Owner,
                    slm_Owner_Position = lead.slm_Owner_Position,
                    slm_ContactDetail = content,
                    slm_CreateBy = "SYSTEM",
                    slm_CreatedBy_Position = null,
                    slm_CreateDate = createdDate,
                    is_Deleted = 0
                };
                slmdb.kkslm_phone_call.AddObject(phone);
                slmdb.SaveChanges();

                return phone.slm_PhoneCallId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal? CalculateTotalPaidAmount(SLMDBEntities slmdb, string ticketId, string paymentCode)
        {
            try
            {
                string sql = @"SELECT COUNT(detail.slm_RecAmount) AS Amount
                                FROM dbo.kkslm_tr_renewinsurance_receipt receipt
                                INNER JOIN dbo.kkslm_tr_renewinsurance_receipt_detail detail ON receipt.slm_RenewInsuranceReceiptId = detail.slm_RenewInsuranceReceiptId
                                WHERE receipt.slm_ticketId = '" + ticketId + @"' AND detail.slm_PaymentCode = '" + paymentCode + @"' AND detail.is_Deleted = 0 ";

                var count = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
                if (count > 0)
                {
                    sql = @"SELECT SUM(detail.slm_RecAmount) AS TotalAmount
                                FROM dbo.kkslm_tr_renewinsurance_receipt receipt
                                INNER JOIN dbo.kkslm_tr_renewinsurance_receipt_detail detail ON receipt.slm_RenewInsuranceReceiptId = detail.slm_RenewInsuranceReceiptId
                                WHERE receipt.slm_ticketId = '" + ticketId + @"' AND detail.slm_PaymentCode = '" + paymentCode + @"' AND detail.is_Deleted = 0 ";

                    var amount = slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
                    return amount != null ? amount.Value : 0;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoProcessPolicy(kkslm_tr_renewinsurance renew_ins, kkslm_tr_lead lead, List<TransactionData> paymentList, SLMDBEntities slmdb, DateTime createdDate, out decimal? current_paid_policy, List<InsuranceCompanyData> companyList)
        {
            try
            {
                bool insentiveFlag = false;
                string paymentCode = AppConstant.PaymentCode.Policy;

                decimal paidAmount = paymentList.Sum(p => p.RecAmount == null ? 0 : p.RecAmount.Value);
                current_paid_policy = paidAmount;   

                var paidAmountDB = GetPolicyAmountPaid(slmdb, renew_ins.slm_TicketId, paymentCode);
                paidAmount += (paidAmountDB != null ? paidAmountDB.Value : 0);      

                decimal dueAmount = (renew_ins.slm_PolicyGrossPremium != null ? renew_ins.slm_PolicyGrossPremium.Value : 0);
                decimal? discount = slmdb.kkslm_ms_discount.Where(p => p.slm_InsuranceTypeCode == paymentCode && p.is_Deleted == false && p.slm_StaffTypeId == -1).Select(p => p.slm_DiscountBath).FirstOrDefault();
                decimal lowRange = discount != null ? (dueAmount - discount.Value) : dueAmount;
                decimal highRange = discount != null ? (dueAmount + discount.Value) : dueAmount;

                if (string.IsNullOrEmpty(renew_ins.slm_ReceiveNo))
                {
                    if (renew_ins.slm_PolicyStartCoverDate == null)
                    {
                        throw new Exception(string.Format("TicketId={0}, ไม่พบข้อมูลวันที่เริ่มต้นคุ้มครองประกัน", renew_ins.slm_TicketId));
                    }

                    if ((paidAmount >= lowRange && paidAmount <= highRange) && renew_ins.slm_PolicyStartCoverDate.Value.Date >= DateTime.Now.Date)
                    {
                        InsertReceipt(slmdb, lead.slm_ticketId, paymentList, createdDate);

                        bool ret = DoProcessReceiveNo(slmdb, lead, renew_ins, lead.slm_ticketId, createdDate, companyList);
                        if (ret)
                        {
                            if (renew_ins.slm_IncentiveFlag == null || renew_ins.slm_IncentiveFlag == false)
                            {
                                insentiveFlag = DoProcessPolicyIncentive(slmdb, lead, renew_ins, lead.slm_ticketId, createdDate);
                            }
                            else
                            {
                                UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "05", "26", AppConstant.LoggingType.EODUpdateCurrent);
                            }

                            InsertTableReport(slmdb, lead, renew_ins, paymentList, createdDate);
                        }
                        else
                        {
                            DoProcessReceiveNoFail(slmdb, lead, createdDate);
                        }
                    }
                    else
                    {
                        DoProcessOther(slmdb, lead, renew_ins, paymentList, createdDate);
                    }
                }
                else
                {
                    if (paidAmount >= lowRange && paidAmount <= highRange)
                    {
                        InsertReceipt(slmdb, lead.slm_ticketId, paymentList, createdDate);

                        if ((renew_ins.slm_PolicyGrossPremium != null && renew_ins.slm_PolicyGrossPremium.Value > 0)
                            && (renew_ins.slm_IncentiveFlag == null || renew_ins.slm_IncentiveFlag == false))
                        {
                            insentiveFlag = DoProcessPolicyIncentive(slmdb, lead, renew_ins, lead.slm_ticketId, createdDate);

                            //if (renew_ins.slm_PayOptionId == 2 && renew_ins.slm_ClaimFlag == "1")
                            if (renew_ins.slm_ClaimFlag == "1")
                            {
                                InsertCancelSettleClaimReport(slmdb, lead.slm_ticketId, createdDate, lead, renew_ins);
                            }
                        }
                        else
                        {
                            UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "05", "26", AppConstant.LoggingType.EODUpdateCurrent);
                        }
                    }
                    else
                    {
                        DoProcessOther(slmdb, lead, renew_ins, paymentList, createdDate);
                    }
                }

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoProcessAct(kkslm_tr_renewinsurance renew_ins, kkslm_tr_lead lead, List<TransactionData> paymentList, SLMDBEntities slmdb, DateTime createdDate, bool policyPurchased, out decimal? current_paid_act)
        {
            try
            {
                bool insentiveFlag = false;
                string paymentCode = AppConstant.PaymentCode.Act;

                decimal paidAmount = paymentList.Sum(p => p.RecAmount == null ? 0 : p.RecAmount.Value);
                current_paid_act = paidAmount;  

                var paidAmountDB = GetPolicyAmountPaid(slmdb, renew_ins.slm_TicketId, paymentCode);
                paidAmount += (paidAmountDB != null ? paidAmountDB.Value : 0); 

                decimal dueAmount = renew_ins.slm_ActGrossPremium != null ? renew_ins.slm_ActGrossPremium.Value : 0;
                decimal? discount = slmdb.kkslm_ms_discount.Where(p => p.slm_InsuranceTypeCode == paymentCode && p.is_Deleted == false && p.slm_StaffTypeId == -1).Select(p => p.slm_DiscountBath).FirstOrDefault();
                decimal lowRange = discount != null ? (dueAmount - discount.Value) : dueAmount;
                decimal highRange = discount != null ? (dueAmount + discount.Value) : dueAmount;

                if (renew_ins.slm_ActStartCoverDate == null)
                {
                    throw new Exception(string.Format("TicketId={0}, ไม่พบข้อมูลวันที่เริ่มต้นคุ้มครองพรบ.", renew_ins.slm_TicketId));
                }

                if ((paidAmount >= lowRange && paidAmount <= highRange) && renew_ins.slm_ActStartCoverDate.Value.Date >= DateTime.Now.Date)
                {
                    InsertReceipt(slmdb, lead.slm_ticketId, paymentList, createdDate);

                    if (renew_ins.slm_ActSendDate == null)
                    {     
                        DoProcessActSendDate(slmdb, lead, renew_ins, createdDate, policyPurchased);     
                        InsertActIssueReport(slmdb, lead.slm_ticketId, createdDate);
                    }

                    if (renew_ins.slm_ActIncentiveFlag == null || renew_ins.slm_ActIncentiveFlag == false)
                    {
                        insentiveFlag = DoProcessActIncentive(slmdb, lead, renew_ins, lead.slm_ticketId, createdDate, policyPurchased);
                    }
                    else
                    {
                        if (!policyPurchased)
                        {
                            UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "05", "27", AppConstant.LoggingType.EODUpdateCurrent);
                        }
                    }
                }
                else
                {
                    DoProcessOtherAct(slmdb, lead, renew_ins, paymentList, createdDate, policyPurchased);
                }

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoProcessActSendDate(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew_ins, DateTime createdDate, bool policyPurchased)
        {
            try
            {
                renew_ins.slm_ActSendDate = createdDate;

                if (!policyPurchased)
                {
                    UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "14", "35", AppConstant.LoggingType.EODUpdateCurrent);
                }

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal GetAmountPaidDB(SLMDBEntities slmdb, string ticketId)
        {
            try
            {
                string sql = @"SELECT SUM(detail.slm_RecAmount) AS TotalAmount
                                FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt receipt
                                INNER JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail detail ON receipt.slm_RenewInsuranceReceiptId = detail.slm_RenewInsuranceReceiptId
                                WHERE receipt.slm_ticketId = '" + ticketId + "' ";

                var amount = slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
                return amount != null ? amount.Value : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal? GetPolicyAmountPaid(SLMDBEntities slmdb, string ticketId, string paymentCode)
        {
            try
            {
                string sql = @"SELECT SUM(detail.slm_RecAmount) AS TotalAmount
                                FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt receipt
                                INNER JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail detail ON receipt.slm_RenewInsuranceReceiptId = detail.slm_RenewInsuranceReceiptId
                                WHERE receipt.slm_ticketId = '" + ticketId + @"' AND detail.slm_PaymentCode = '" + paymentCode + @"' AND detail.is_Deleted = 0 ";

                var amount = slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
                return amount != null ? amount : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool DoProcessReceiveNo(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew_ins, string ticketId, DateTime createdDate, List<InsuranceCompanyData> companyList)
        {
            try
            {
                string receiveNo = GenerateReceiveNo(slmdb, ticketId, createdDate);
                if (string.IsNullOrEmpty(receiveNo))
                    return false;

                renew_ins.slm_ReceiveNo = receiveNo;
                renew_ins.slm_ReceiveDate = createdDate;
                renew_ins.slm_GenSMSReceiveNo = null;
                renew_ins.slm_GenSMSReceiveNoDate = null;
                renew_ins.slm_UpdatedBy = "SYSTEM";
                renew_ins.slm_UpdatedDate = createdDate;

                UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "14", "22", AppConstant.LoggingType.EODUpdateCurrent);

                slmdb.SaveChanges();    
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool DoProcessPolicyIncentive(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew_ins, string ticketId, DateTime createdDate)
        {
            try
            {
                bool insentiveFlag = false;

                //ประกัน
                insentiveFlag = CalculatePolicyIncentive(slmdb, ticketId, renew_ins, createdDate);
                if (insentiveFlag)
                {
                    UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "05", "26", AppConstant.LoggingType.EODUpdateCurrent);

                    kkslm_tr_activity act_incentive = new kkslm_tr_activity()
                    {
                        slm_TicketId = ticketId,
                        slm_OldStatus = null,
                        slm_OldSubStatus = null,
                        slm_NewStatus = null,
                        slm_NewSubStatus = null,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedBy_Position = null,
                        slm_CreatedDate = createdDate,
                        slm_Type = AppConstant.LoggingType.GetIncentiveIns,     
                        slm_SystemAction = "SLM",                              
                        slm_SystemActionBy = "SLM",                             
                        slm_ExternalSystem_Old = null,
                        slm_ExternalStatus_Old = null,
                        slm_ExternalSubStatus_Old = null,
                        slm_ExternalSubStatusDesc_Old = null,
                        slm_ExternalStatus_New = null,
                        slm_ExternalSubStatus_New = null,
                        slm_ExternalSystem_New = null,
                        slm_ExternalSubStatusDesc_New = null
                    };
                    slmdb.kkslm_tr_activity.AddObject(act_incentive);
                }

                slmdb.SaveChanges();

                return insentiveFlag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //ซื้อ พรบ อย่างเดียว
        private bool DoProcessActIncentive(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew_ins, string ticketId, DateTime createdDate, bool policyPurchased)
        {
            try
            {
                bool insentiveFlag = false;

                insentiveFlag = CalculateActIncentive(slmdb, ticketId, renew_ins, createdDate);
                if (insentiveFlag)
                {
                    if (!policyPurchased)
                    {
                        UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "05", "27", AppConstant.LoggingType.EODUpdateCurrent);
                    }

                    kkslm_tr_activity act_incentive = new kkslm_tr_activity()
                    {
                        slm_TicketId = ticketId,
                        slm_OldStatus = null,
                        slm_OldSubStatus = null,
                        slm_NewStatus = null,
                        slm_NewSubStatus = null,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedBy_Position = null,
                        slm_CreatedDate = createdDate,
                        slm_Type = AppConstant.LoggingType.GetIncentiveAct,     
                        slm_SystemAction = "SLM",                               
                        slm_SystemActionBy = "SLM",                             
                        slm_ExternalSystem_Old = null,
                        slm_ExternalStatus_Old = null,
                        slm_ExternalSubStatus_Old = null,
                        slm_ExternalSubStatusDesc_Old = null,
                        slm_ExternalStatus_New = null,
                        slm_ExternalSubStatus_New = null,
                        slm_ExternalSystem_New = null,
                        slm_ExternalSubStatusDesc_New = null
                    };
                    slmdb.kkslm_tr_activity.AddObject(act_incentive);
                }

                slmdb.SaveChanges();

                return insentiveFlag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoProcessOther(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew_ins, List<TransactionData> paymentList, DateTime createdDate)
        {
            try
            {
                InsertReceipt(slmdb, lead.slm_ticketId, paymentList, createdDate);

                UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "14", "19", AppConstant.LoggingType.EODUpdateCurrent);

                if (renew_ins.slm_IncentiveFlag != null && renew_ins.slm_IncentiveFlag.Value == true && renew_ins.slm_IncentiveDate != null)
                {
                    string tranDate = paymentList.Select(p => p.TranDate).FirstOrDefault();
                    if (tranDate.Length == 8)
                    {
                        string month = tranDate.Substring(4, 2);
                        string year = tranDate.Substring(0, 4);

                        string inc_month = renew_ins.slm_IncentiveDate.Value.Month.ToString("00");
                        string inc_year = renew_ins.slm_IncentiveDate.Value.Year.ToString("0000");

                        if (month == inc_month && year == inc_year)
                        {
                            renew_ins.slm_PolicyComBath = null;
                            renew_ins.slm_PolicyComBathVat = null;
                            renew_ins.slm_PolicyComBathIncentive = null;
                            renew_ins.slm_PolicyOV1Bath = null;
                            renew_ins.slm_PolicyOV1BathVat = null;
                            renew_ins.slm_PolicyOV1BathIncentive = null;
                            renew_ins.slm_PolicyOV2Bath = null;
                            renew_ins.slm_PolicyOV2BathVat = null;
                            renew_ins.slm_PolicyOV2BathIncentive = null;
                            renew_ins.slm_PolicyIncentiveAmount = null;
                            renew_ins.slm_IncentiveFlag = null;
                            renew_ins.slm_IncentiveDate = null;
                            renew_ins.slm_PolicyReferenceNote = null;
                            renew_ins.slm_IncentiveCancelDate = createdDate;

                            kkslm_tr_activity activity = new kkslm_tr_activity()
                            {
                                slm_TicketId = lead.slm_ticketId,
                                slm_OldStatus = null,
                                slm_OldSubStatus = null,
                                slm_NewStatus = null,
                                slm_NewSubStatus = null,
                                slm_CreatedBy = "SYSTEM",
                                slm_CreatedBy_Position = null,
                                slm_CreatedDate = createdDate,
                                slm_Type = AppConstant.LoggingType.RevertIncentiveIns,
                                slm_SystemAction = "SLM",
                                slm_SystemActionBy = "SLM",
                                slm_ExternalSystem_Old = null,
                                slm_ExternalStatus_Old = null,
                                slm_ExternalSubStatus_Old = null,
                                slm_ExternalSubStatusDesc_Old = null,
                                slm_ExternalStatus_New = null,
                                slm_ExternalSubStatus_New = null,
                                slm_ExternalSystem_New = null,
                                slm_ExternalSubStatusDesc_New = null
                            };
                            slmdb.kkslm_tr_activity.AddObject(activity);
                        }
                    }
                    else
                    {
                        string message = "TicketId " + lead.slm_ticketId + ", Cannot revert incentive(policy) because trandate(BDW) is not in correct format(YYYYMMDD)";
                        Util.WriteLogFile(logfilename, BatchCode, message);
                        BizUtil.InsertLog(BatchMonitorId, "", "", message);
                    }
                }

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoProcessOtherAct(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew_ins, List<TransactionData> paymentList, DateTime createdDate, bool policyPurchased)
        {
            try
            {
                InsertReceipt(slmdb, lead.slm_ticketId, paymentList, createdDate);

                if (!policyPurchased)
                {
                    UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "14", "19", AppConstant.LoggingType.EODUpdateCurrent);
                }

                if (renew_ins.slm_ActIncentiveFlag != null && renew_ins.slm_ActIncentiveFlag.Value == true && renew_ins.slm_ActIncentiveDate != null)
                {
                    string tranDate = paymentList.Select(p => p.TranDate).FirstOrDefault();
                    if (tranDate.Length == 8)
                    {
                        string month = tranDate.Substring(4, 2);
                        string year = tranDate.Substring(0, 4);

                        string inc_month = renew_ins.slm_ActIncentiveDate.Value.Month.ToString("00");
                        string inc_year = renew_ins.slm_ActIncentiveDate.Value.Year.ToString("0000");

                        if (month == inc_month && year == inc_year)
                        {
                            renew_ins.slm_ActComBath = null;
                            renew_ins.slm_ActComBathVat = null;
                            renew_ins.slm_ActComBathIncentive = null;
                            renew_ins.slm_ActOV1Bath = null;
                            renew_ins.slm_ActOV1BathVat = null;
                            renew_ins.slm_ActOV1BathIncentive = null;
                            renew_ins.slm_ActOV2Bath = null;
                            renew_ins.slm_ActOV2BathVat = null;
                            renew_ins.slm_ActOV2BathIncentive = null;
                            renew_ins.slm_ActIncentiveAmount = null;
                            renew_ins.slm_ActIncentiveFlag = null;
                            renew_ins.slm_ActIncentiveDate = null;
                            renew_ins.slm_ActReferenceNote = null;
                            renew_ins.slm_ActIncentiveCancelDate = createdDate;

                            kkslm_tr_activity activity = new kkslm_tr_activity()
                            {
                                slm_TicketId = lead.slm_ticketId,
                                slm_OldStatus = null,
                                slm_OldSubStatus = null,
                                slm_NewStatus = null,
                                slm_NewSubStatus = null,
                                slm_CreatedBy = "SYSTEM",
                                slm_CreatedBy_Position = null,
                                slm_CreatedDate = createdDate,
                                slm_Type = AppConstant.LoggingType.RevertIncentiveAct,      //RevertIncentiveAct = 17
                                slm_SystemAction = "SLM",                                   //System ที่เข้ามาทำ action
                                slm_SystemActionBy = "SLM",                                 //action เกิดขึ้นที่ระบบอะไร
                                slm_ExternalSystem_Old = null,
                                slm_ExternalStatus_Old = null,
                                slm_ExternalSubStatus_Old = null,
                                slm_ExternalSubStatusDesc_Old = null,
                                slm_ExternalStatus_New = null,
                                slm_ExternalSubStatus_New = null,
                                slm_ExternalSystem_New = null,
                                slm_ExternalSubStatusDesc_New = null
                            };
                            slmdb.kkslm_tr_activity.AddObject(activity);
                        }
                    }
                    else
                    {
                        string message = "TicketId " + lead.slm_ticketId + ", Cannot revert incentive(act) because trandate(BDW) is not in correct format(YYYYMMDD)";
                        Util.WriteLogFile(logfilename, BatchCode, message);
                        BizUtil.InsertLog(BatchMonitorId, "", "", message);
                    }
                }

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoProcessReceiveNoFail(SLMDBEntities slmdb, kkslm_tr_lead lead, DateTime createdDate)
        {
            try
            {
                UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "14", "19", AppConstant.LoggingType.EODUpdateCurrent);
                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private void ClearPaymentTemp()
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                string sql = "delete from " + AppConstant.SLMDBName + ".dbo.kkslm_tr_renewinsurance_hp_payment_temp ";
                slmdb.ExecuteStoreCommand(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertReceipt(SLMDBEntities slmdb, string ticketId, List<TransactionData> paymentList, DateTime createdDate)
        {
            try
            {
                decimal receiptId = 0;
                foreach (TransactionData data in paymentList)
                {
                    var obj = slmdb.kkslm_tr_renewinsurance_receipt.Where(p => p.slm_ticketId == ticketId && p.slm_RecNo == data.RecNo && p.is_Deleted == false).FirstOrDefault();
                    if (obj == null)
                    {
                        kkslm_tr_renewinsurance_receipt receipt = new kkslm_tr_renewinsurance_receipt()
                        {
                            slm_ticketId = ticketId,
                            slm_RecNo = data.RecNo,
                            slm_Status = null,
                            slm_CreatedBy = "SYSTEM",
                            slm_CreatedDate = createdDate,
                            slm_UpdatedBy = "SYSTEM",
                            slm_UpdatedDate = createdDate,
                            is_Deleted = false
                        };
                        slmdb.kkslm_tr_renewinsurance_receipt.AddObject(receipt);
                        slmdb.SaveChanges();
                        receiptId = receipt.slm_RenewInsuranceReceiptId;
                    }
                    else
                        receiptId = obj.slm_RenewInsuranceReceiptId;

                    kkslm_tr_renewinsurance_receipt_detail detail = new kkslm_tr_renewinsurance_receipt_detail()
                    {
                        slm_RenewInsuranceReceiptId = receiptId,
                        slm_PaymentCode = data.TranCode,
                        slm_InsNoDesc = data.InsNoDesc,
                        slm_InstNo = data.InstNo,
                        slm_RecBy = data.RecBy,
                        slm_RecNo = data.RecNo,
                        slm_RecAmount = data.RecAmount,
                        slm_Status = null,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createdDate,
                        is_Deleted = false
                    };

                    if (!string.IsNullOrEmpty(data.TranDate))
                    {
                        detail.slm_TransDate = AppUtil.ConvertToDateTime("TRAN_DATE", data.TranDate, AppConstant.DateTimeFormat.Format1);
                    }

                    slmdb.kkslm_tr_renewinsurance_receipt_detail.AddObject(detail);

                    //mark flag is delete of pending table
                    slmdb.kkslm_tr_renewinsurance_hp_payment_pending.Where(x => x.slm_Contract_Number == data.ContractNumber).ToList().ForEach(p => { p.is_Deleted = true; p.slm_UpdatedDate = createdDate; });
                    //====================================

                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerateReceiveNo(SLMDBEntities slmdb, string ticketId, DateTime createdDate)
        {
            try
            {
                string str_createdDate = createdDate.Year.ToString() + createdDate.ToString("-MM-dd");

                string sql = @"SELECT LEAD.slm_Product_Id AS ProductId, REN.slm_InsuranceComId AS InsuranceComId 
                                FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_lead LEAD 
                                INNER JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                                WHERE LEAD.is_Deleted = 0 AND LEAD.slm_ticketId = '" + ticketId + "' ";

                var data = slmdb.ExecuteStoreQuery<ReceiveNoData>(sql).FirstOrDefault();
                if (data == null) { throw new Exception("ไม่พบข้อมูล kkslm_tr_renewinsurance ที่ TicketId=" + ticketId); }

                sql = @"SELECT TOP 1 slm_ReceiveNoListId AS ReceiveNoListId, slm_ReceiveNo AS ReceiveNo 
                        FROM " + AppConstant.OPERDBName + @".dbo.kkslm_ms_receivenolist
                        WHERE is_Deleted = '0' AND slm_Status = '0' AND slm_Product_Id = '" + data.ProductId + "' AND slm_Ins_Com_Id = '" + (data.InsuranceComId != null ? data.InsuranceComId.Value : 0).ToString() + @"' 
                        ORDER BY slm_ReceiveNoListId ";

                string errorMessage = "ไม่พบข้อมูลเลขที่รับแจ้งที่ใช้งานได้ (TicketId=" + ticketId + ", ProductId=" + data.ProductId + ", InsComId=" + (data.InsuranceComId != null ? data.InsuranceComId.Value.ToString() : "Null") + ")";
                var receiveNo = slmdb.ExecuteStoreQuery<ReceiveNoInfo>(sql).FirstOrDefault();
                if (receiveNo == null)
                {
                    Util.WriteLogFile(logfilename, BatchCode, errorMessage);
                    BizUtil.InsertLog(BatchMonitorId, ticketId, "", errorMessage);
                    return "";
                }
                else
                {
                    if (receiveNo.ReceiveNoListId == null || string.IsNullOrEmpty(receiveNo.ReceiveNo))
                    {
                        Util.WriteLogFile(logfilename, BatchCode, errorMessage);
                        BizUtil.InsertLog(BatchMonitorId, ticketId, "", errorMessage);
                        return "";
                    } 
                }

                sql = @"UPDATE " + AppConstant.OPERDBName + @".dbo.kkslm_ms_receivenolist
                        SET slm_TicketId = '" + ticketId + @"', slm_UseDate = '" + str_createdDate + @"', slm_UseBy = 'SYSTEM', slm_UpdatedBy = 'SYSTEM', slm_UpdatedDate = '" + str_createdDate + @"'
                        , slm_Status = '1'
                        WHERE slm_ReceiveNoListId = '" + receiveNo.ReceiveNoListId.Value.ToString() + "' ";

                slmdb.ExecuteStoreCommand(sql);

                return receiveNo.ReceiveNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CalculatePolicyIncentive(SLMDBEntities slmdb, string ticketId, kkslm_tr_renewinsurance renew_ins, DateTime createdDate)
        {
            try
            {
                //คำนวณ Incentive ประกัน
                string sql = "";
                string errMessage = "";

                sql = @"SELECT LEAD.SLM_PRODUCT_ID AS slm_Product_Id, renc.slm_Ins_Com_Id AS slm_Ins_Com_Id , PRO.slm_CampaignInsuranceId AS slm_CampaignInsuranceId
                        , ren.slm_CoverageTypeId AS slm_CoverageTypeId
                        FROM " + AppConstant.SLMDBName + @".DBO.KKSLM_TR_LEAD LEAD 
                        INNER JOIN " + AppConstant.SLMDBName + @".DBO.kkslm_tr_renewinsurance REN ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
                        INNER JOIN " + AppConstant.SLMDBName + @".DBO.kkslm_tr_renewinsurance_compare renc on ren.slm_RenewInsureId = renc.slm_RenewInsureId 
                        LEFT JOIN " + AppConstant.OPERDBName + @".DBO.kkslm_ms_promotioninsurance PRO ON PRO.slm_PromotionId = RENC.slm_PromotionId 
                        WHERE LEAD.IS_DELETED = 0 AND RENC.slm_Selected = 1 AND LEAD.slm_ticketId = '" + ticketId + "' ";

                var prod_com = slmdb.ExecuteStoreQuery<BenefitData>(sql).FirstOrDefault();
                if (prod_com == null)
                {
                    errMessage = "ไม่พบข้อมูล Product กับบริษัทประกัน ที่ใช้ในการคำนวณ Incentive ประกัน";
                    Util.WriteLogFile(logfilename, BatchCode, errMessage);
                    BizUtil.InsertLog(BatchMonitorId, ticketId, "", errMessage);
                    return false;
                }

                string productId = string.IsNullOrEmpty(prod_com.slm_Product_Id) ? "" : prod_com.slm_Product_Id;
                string insComId = prod_com.slm_Ins_Com_Id != null ? prod_com.slm_Ins_Com_Id.Value.ToString() : "0";
                string campaignInsuranceId = prod_com.slm_CampaignInsuranceId != null ? prod_com.slm_CampaignInsuranceId.Value.ToString() : "0";
                string coverageTypeId = prod_com.slm_CoverageTypeId != null ? prod_com.slm_CoverageTypeId.Value.ToString() : "0";

                var benefit = slmdb.ExecuteStoreQuery<BenefitData>(string.Format("exec dbo.SP_INQ_POLICY_BENEFIT '{0}', {1}, {2}, {3}", productId, insComId, campaignInsuranceId, coverageTypeId)).FirstOrDefault();
                if (benefit == null)
                {
                    errMessage = "ไม่พบข้อมูลผลประโยชน์ ที่ใช้ในการคำนวณ Incentive ประกัน";
                    Util.WriteLogFile(logfilename, BatchCode, errMessage);
                    BizUtil.InsertLog(BatchMonitorId, ticketId, "", errMessage);
                    return false;
                }

                //Validate VatFlag
                if (string.IsNullOrEmpty(benefit.slm_VatFlag))
                {
                    errMessage = "ไม่พบข้อมูล Include, Exclude Vat ใน Benefit ที่ใช้ในการคำนวณ Incentive ประกัน";
                    Util.WriteLogFile(logfilename, BatchCode, errMessage);
                    BizUtil.InsertLog(BatchMonitorId, ticketId, "", errMessage);
                    return false;
                }
                else
                {
                    if (benefit.slm_VatFlag != "I" && benefit.slm_VatFlag != "E")
                    {
                        errMessage = "ข้อมูล VatFlag ที่ใช้ในการคำนวณ Incentive ประกันไม่ถูกต้อง, VatFlag=" + benefit.slm_VatFlag;
                        Util.WriteLogFile(logfilename, BatchCode, errMessage);
                        BizUtil.InsertLog(BatchMonitorId, ticketId, "", errMessage);
                        return false;
                    }
                }

                decimal policyComBath = 0;
                decimal policyComBathVat = 0;
                decimal policyComBathIncentive = 0;
                decimal policyOv1Bath = 0;
                decimal policyOv1BathVat = 0;
                decimal policyOv1BathIncentive = 0;
                decimal policyOv2Bath = 0;
                decimal policyOv2BathVat = 0;
                decimal policyOv2BathIncentive = 0;
                decimal policyIncentiveAmount = 0;

                //1.หาค่าคอม
                if (benefit.slm_ComissionFlag == "1")
                {
                    decimal policyPremiumTotal = renew_ins.slm_PolicyNetGrossPremium != null ? renew_ins.slm_PolicyNetGrossPremium.Value : 0;
                    decimal comPercent = benefit.slm_ComissionPercentValue != null ? benefit.slm_ComissionPercentValue.Value : 0;
                    policyComBath = Math.Round((policyPremiumTotal * comPercent) / 100, 2);
                }
                else
                {
                    policyComBath = benefit.slm_ComissionBathValue != null ? benefit.slm_ComissionBathValue.Value : 0;
                }

                //2.หา VAT ของค่าคอม
                policyComBathVat = Math.Round((policyComBath * AppConstant.Vat) / 100, 2);

                //3.หา Incentive สุทธิของค่าคอม
                policyComBathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((policyComBath - policyComBathVat), 2) : Math.Round((policyComBath + policyComBathVat), 2);

                //คำนวณ OV1
                if (benefit.slm_OV1Flag == "1" || benefit.slm_OV1Flag == "2")
                {
                    //4.หา OV1
                    if (benefit.slm_OV1Flag == "1")
                    {
                        decimal policyPremiumTotal = renew_ins.slm_PolicyNetGrossPremium != null ? renew_ins.slm_PolicyNetGrossPremium.Value : 0;
                        decimal ov1Percent = benefit.slm_OV1PercentValue != null ? benefit.slm_OV1PercentValue.Value : 0;
                        policyOv1Bath = Math.Round((policyPremiumTotal * ov1Percent) / 100, 2);
                    }
                    else
                    {
                        policyOv1Bath = benefit.slm_OV1BathValue != null ? benefit.slm_OV1BathValue.Value : 0;
                    }

                    //5.หา Vat ของ OV1
                    policyOv1BathVat = Math.Round((policyOv1Bath * AppConstant.Vat) / 100, 2);

                    //6.หา Incentive ของค่า OV1
                    policyOv1BathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((policyOv1Bath - policyOv1BathVat), 2) : Math.Round((policyOv1Bath + policyOv1BathVat), 2);
                }

                //คำนวณ OV2
                if (benefit.slm_OV2Flag == "1" || benefit.slm_OV2Flag == "2")
                {
                    //7.หา OV2
                    if (benefit.slm_OV2Flag == "1")
                    {
                        decimal policyPremiumTotal = renew_ins.slm_PolicyNetGrossPremium != null ? renew_ins.slm_PolicyNetGrossPremium.Value : 0;
                        decimal ov2Percent = benefit.slm_OV2PercentValue != null ? benefit.slm_OV2PercentValue.Value : 0;
                        policyOv2Bath = Math.Round((policyPremiumTotal * ov2Percent) / 100, 2);
                    }
                    else
                    {
                        policyOv2Bath = benefit.slm_OV2BathValue != null ? benefit.slm_OV2BathValue.Value : 0;
                    }

                    //8.หา Vat ของ OV2
                    policyOv2BathVat = Math.Round((policyOv2Bath * AppConstant.Vat) / 100, 2);

                    //9.หา Incentive ของค่า OV2
                    policyOv2BathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((policyOv2Bath - policyOv2BathVat), 2) : Math.Round((policyOv2Bath + policyOv2BathVat), 2);
                }

                //10.คำนวณ policyIncentiveAmount
                policyIncentiveAmount = Math.Round(policyComBathIncentive + policyOv1BathIncentive + policyOv2BathIncentive, 2);

                renew_ins.slm_PolicyComBath = policyComBath;
                renew_ins.slm_PolicyComBathVat = policyComBathVat;
                renew_ins.slm_PolicyComBathIncentive = policyComBathIncentive;
                renew_ins.slm_PolicyOV1Bath = policyOv1Bath;
                renew_ins.slm_PolicyOV1BathVat = policyOv1BathVat;
                renew_ins.slm_PolicyOV1BathIncentive = policyOv1BathIncentive;
                renew_ins.slm_PolicyOV2Bath = policyOv2Bath;
                renew_ins.slm_PolicyOV2BathVat = policyOv2BathVat;
                renew_ins.slm_PolicyOV2BathIncentive = policyOv2BathIncentive;
                renew_ins.slm_PolicyIncentiveAmount = policyIncentiveAmount;
                renew_ins.slm_IncentiveFlag = true;
                renew_ins.slm_IncentiveDate = createdDate;
                renew_ins.slm_PolicyReferenceNote = "Scenario = " + benefit.Scenario + "|" +  benefit.slm_PolicyReferenceNote;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CalculateActIncentive(SLMDBEntities slmdb, string ticketId, kkslm_tr_renewinsurance renew_ins, DateTime createdDate)
        {
            try
            {
                //คำนวณ Incentive พรบ
                string sql = "";
                string errMessage = "";

                //sql = @"SELECT LEAD.SLM_PRODUCT_ID AS slm_Product_Id, rencc.slm_Ins_Com_Id AS slm_Ins_Com_Id , PRO.slm_CampaignInsuranceId AS slm_CampaignInsuranceId
                //        , convert(int, prod.slm_CarType) AS slm_InsurancecarTypeId
                //        FROM " + AppConstant.SLMDBName + @".DBO.KKSLM_TR_LEAD LEAD 
                //        INNER JOIN " + AppConstant.SLMDBName + @".DBO.kkslm_tr_renewinsurance REN ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
                //        LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_productinfo prod ON prod.slm_TicketId = lead.slm_ticketId
                //        INNER JOIN " + AppConstant.SLMDBName + @".DBO.kkslm_tr_renewinsurance_compare_act rencc on ren.slm_RenewInsureId = rencc.slm_RenewInsureId 
                //        LEFT JOIN " + AppConstant.OPERDBName + @".DBO.kkslm_ms_promotioninsurance PRO ON PRO.slm_PromotionId = rencc.slm_PromotionId 
                //        WHERE LEAD.IS_DELETED = 0 AND rencc.slm_ActPurchaseFlag = 1 AND LEAD.slm_ticketId = '" + ticketId + "' ";

                string slmdbName = AppConstant.SLMDBName;
                sql = @"SELECT LEAD.SLM_PRODUCT_ID AS slm_Product_Id, rencc.slm_Ins_Com_Id AS slm_Ins_Com_Id , PRO.slm_CampaignInsuranceId AS slm_CampaignInsuranceId
                        , REN.slm_InsurancecarTypeId AS slm_InsurancecarTypeId
                        FROM " + slmdbName + @".DBO.KKSLM_TR_LEAD LEAD 
                        INNER JOIN " + slmdbName + @".DBO.kkslm_tr_renewinsurance REN ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
                        INNER JOIN " + slmdbName + @".DBO.kkslm_tr_renewinsurance_compare_act rencc on ren.slm_RenewInsureId = rencc.slm_RenewInsureId 
                        LEFT JOIN " + AppConstant.OPERDBName + @".DBO.kkslm_ms_promotioninsurance PRO ON PRO.slm_PromotionId = rencc.slm_PromotionId 
                        WHERE LEAD.IS_DELETED = 0 AND rencc.slm_ActPurchaseFlag = 1 AND LEAD.slm_ticketId = '" + ticketId + "' ";

                var prod_com = slmdb.ExecuteStoreQuery<BenefitData>(sql).FirstOrDefault();
                if (prod_com == null)
                {
                    errMessage = "ไม่พบข้อมูล Product กับบริษัทประกัน ที่ใช้ในการคำนวณ Incentive พรบ.";
                    Util.WriteLogFile(logfilename, BatchCode, errMessage);
                    BizUtil.InsertLog(BatchMonitorId, ticketId, "", errMessage);
                    return false;
                }

                string productId = string.IsNullOrEmpty(prod_com.slm_Product_Id) ? "" : prod_com.slm_Product_Id;
                string insComId = prod_com.slm_Ins_Com_Id != null ? prod_com.slm_Ins_Com_Id.Value.ToString() : "0";
                string campaignInsuranceId = prod_com.slm_CampaignInsuranceId != null ? prod_com.slm_CampaignInsuranceId.Value.ToString() : "0";
                string insurancecarTypeId = prod_com.slm_InsurancecarTypeId != null ? prod_com.slm_InsurancecarTypeId.Value.ToString() : "0";

                var benefit = slmdb.ExecuteStoreQuery<BenefitData>(string.Format("exec dbo.SP_INQ_ACT_BENEFIT '{0}', {1}, {2}, {3}", productId, insComId, campaignInsuranceId, insurancecarTypeId)).FirstOrDefault();
                if (benefit == null)
                {
                    errMessage = "ไม่พบข้อมูลผลประโยชน์ ที่ใช้ในการคำนวณ Incentive พรบ.";
                    Util.WriteLogFile(logfilename, BatchCode, errMessage);
                    BizUtil.InsertLog(BatchMonitorId, ticketId, "", errMessage);
                    return false;
                }

                //Validate VatFlag
                if (string.IsNullOrEmpty(benefit.slm_VatFlag))
                {
                    errMessage = "ไม่พบข้อมูล Include, Exclude Vat ใน Benefit ที่ใช้ในการคำนวณ Incentive พรบ.";
                    Util.WriteLogFile(logfilename, BatchCode, errMessage);
                    BizUtil.InsertLog(BatchMonitorId, ticketId, "", errMessage);
                    return false;
                }
                else
                {
                    if (benefit.slm_VatFlag != "I" && benefit.slm_VatFlag != "E")
                    {
                        errMessage = "ข้อมูล VatFlag ที่ใช้ในการคำนวณ Incentive พรบ.ไม่ถูกต้อง, VatFlag=" + benefit.slm_VatFlag;
                        Util.WriteLogFile(logfilename, BatchCode, errMessage);
                        BizUtil.InsertLog(BatchMonitorId, ticketId, "", errMessage);
                        return false;
                    }
                }

                decimal actComBath = 0;
                decimal actComBathVat = 0;
                decimal actComBathIncentive = 0;
                decimal actOv1Bath = 0;
                decimal actOv1BathVat = 0;
                decimal actOv1BathIncentive = 0;
                decimal actOv2Bath = 0;
                decimal actOv2BathVat = 0;
                decimal actOv2BathIncentive = 0;
                decimal actIncentiveAmount = 0;

                //1.หาค่าคอม
                if (benefit.slm_ComissionFlag == "1")
                {
                    decimal actNetPremium = renew_ins.slm_ActNetPremium != null ? renew_ins.slm_ActNetPremium.Value : 0;
                    decimal comPercent = benefit.slm_ComissionPercentValue != null ? benefit.slm_ComissionPercentValue.Value : 0;
                    actComBath = Math.Round((actNetPremium * comPercent) / 100, 2);
                }
                else
                {
                    actComBath = benefit.slm_ComissionBathValue != null ? benefit.slm_ComissionBathValue.Value : 0;
                }

                //2.หา VAT ของค่าคอม
                actComBathVat = Math.Round((actComBath * AppConstant.Vat) / 100, 2);

                //3.หา Incentive สุทธิของค่าคอม
                actComBathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((actComBath - actComBathVat), 2) : Math.Round((actComBath + actComBathVat), 2);

                //คำนวณ OV1
                if (benefit.slm_OV1Flag == "1" || benefit.slm_OV1Flag == "2")
                {
                    //4.หา OV1
                    if (benefit.slm_OV1Flag == "1")
                    {
                        decimal actNetPremium = renew_ins.slm_ActNetPremium != null ? renew_ins.slm_ActNetPremium.Value : 0;
                        decimal ov1Percent = benefit.slm_OV1PercentValue != null ? benefit.slm_OV1PercentValue.Value : 0;
                        actOv1Bath = Math.Round((actNetPremium * ov1Percent) / 100, 2);
                    }
                    else
                    {
                        actOv1Bath = benefit.slm_OV1BathValue != null ? benefit.slm_OV1BathValue.Value : 0;
                    }

                    //5.หา Vat ของ OV1
                    actOv1BathVat = Math.Round((actOv1Bath * AppConstant.Vat) / 100, 2);

                    //6.หา Incentive ของค่า OV1
                    actOv1BathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((actOv1Bath - actOv1BathVat), 2) : Math.Round((actOv1Bath + actOv1BathVat), 2);
                }

                //คำนวณ OV2
                if (benefit.slm_OV2Flag == "1" || benefit.slm_OV2Flag == "2")
                {
                    //7.หา OV2
                    if (benefit.slm_OV2Flag == "1")
                    {
                        decimal actNetPremium = renew_ins.slm_ActNetPremium != null ? renew_ins.slm_ActNetPremium.Value : 0;
                        decimal ov2Percent = benefit.slm_OV2PercentValue != null ? benefit.slm_OV2PercentValue.Value : 0;
                        actOv2Bath = Math.Round((actNetPremium * ov2Percent) / 100, 2);
                    }
                    else
                    {
                        actOv2Bath = benefit.slm_OV2BathValue != null ? benefit.slm_OV2BathValue.Value : 0;
                    }

                    //8.หา Vat ของ OV2
                    actOv2BathVat = Math.Round((actOv2Bath * AppConstant.Vat) / 100, 2);

                    //9.หา Incentive ของค่า OV2
                    actOv2BathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((actOv2Bath - actOv2BathVat), 2) : Math.Round((actOv2Bath + actOv2BathVat), 2);
                }

                //10.คำนวณ policyIncentiveAmount
                actIncentiveAmount = Math.Round(actComBathIncentive + actOv1BathIncentive + actOv2BathIncentive, 2);

                renew_ins.slm_ActComBath = actComBath;
                renew_ins.slm_ActComBathVat = actComBathVat;
                renew_ins.slm_ActComBathIncentive = actComBathIncentive;
                renew_ins.slm_ActOV1Bath = actOv1Bath;
                renew_ins.slm_ActOV1BathVat = actOv1BathVat;
                renew_ins.slm_ActOV1BathIncentive = actOv1BathIncentive;
                renew_ins.slm_ActOV2Bath = actOv2Bath;
                renew_ins.slm_ActOV2BathVat = actOv2BathVat;
                renew_ins.slm_ActOV2BathIncentive = actOv2BathIncentive;
                renew_ins.slm_ActIncentiveAmount = actIncentiveAmount;
                renew_ins.slm_ActIncentiveFlag = true;
                renew_ins.slm_ActIncentiveDate = createdDate;
                renew_ins.slm_ActReferenceNote = "Scenario = " + benefit.Scenario + "|" + benefit.slm_ActReferenceNote;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertTableReport(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew_ins, List<TransactionData> paymentList, DateTime createdDate)
        {
            try
            {
                //if (renew_ins.slm_PayOptionId == 2 && renew_ins.slm_ClaimFlag == "1")
                if (renew_ins.slm_ClaimFlag == "1")
                {
                    kkslm_tr_setle_claim_cancel_report cancel_report = new kkslm_tr_setle_claim_cancel_report()
                    {
                        slm_CarLicenseNo = renew_ins.slm_LicenseNo,
                        slm_Contract_Number = renew_ins.slm_ContractNo,
                        slm_CoverageStartDate = renew_ins.slm_PolicyStartCoverDate,
                        slm_CoverageEndDate = renew_ins.slm_PolicyEndCoverDate,
                        slm_Export_Date = null,
                        slm_Export_Flag = false,
                        slm_Ins_Com_Id = renew_ins.slm_InsuranceComId,
                        slm_InsReceiveNo = renew_ins.slm_ReceiveNo,
                        slm_InsReceiveDate = renew_ins.slm_ReceiveDate,
                        slm_Title_Id = lead.slm_TitleId,
                        slm_Name = lead.slm_Name,
                        slm_LastName = lead.slm_LastName,
                        slm_Owner = lead.slm_Owner,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createdDate
                    };
                    slmdb.kkslm_tr_setle_claim_cancel_report.AddObject(cancel_report);
                    renew_ins.slm_ClaimFlag = "0";
                }

                var data = (from a in slmdb.kkslm_tr_lead
                            join b in slmdb.kkslm_tr_cusinfo on a.slm_ticketId equals b.slm_TicketId
                            join c in slmdb.kkslm_tr_productinfo on a.slm_ticketId equals c.slm_TicketId
                            where a.slm_ticketId == lead.slm_ticketId
                            select new 
                            { 
                                CitizenId = b.slm_CitizenId, 
                                CardType = b.slm_CardType,
                                BirthDate = b.slm_Birthdate, 
                                Occupation = b.slm_Occupation, 
                                MaritalStatus = b.slm_MaritalStatus, 
                                CarType = c.slm_CarType,
                                ProvinceRegis = c.slm_ProvinceRegis
                            }).FirstOrDefault();
                
                var renew_addr = slmdb.kkslm_tr_renewinsurance_address.Where(p => p.slm_RenewInsureId == renew_ins.slm_RenewInsureId && p.slm_AddressType == "D").FirstOrDefault();
                var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_TicketId == lead.slm_ticketId).FirstOrDefault();

                string sql = @"SELECT cp.slm_OldPolicyNo AS OldPolicyNo,  ci.slm_CampaignName AS CampaignName, cpOld.slm_FT as SumInsured_Year1
                                ,cpOld.slm_NetGrossPremium as NetPremium_Year1, cp.slm_OD as SumInsured_Year2
                                ,cp.slm_NetGrossPremium as NetPremium_Year2, cp.slm_Driver_Birthdate1 as Birthdate_Driver1
                                ,cp.slm_Driver_Birthdate2 as Birthdate_Driver2,cpNotice.slm_Discount_Amount as DiscountGoodHistory
                                ,cp.FullName_Driver1, cp.FullName_Driver2, renew.slm_CoverageTypeId AS CoverageTypeId
	                            ,cp.slm_FT AS SumInsured_Year2_FT, cpNotice.slm_Discount_Percent AS DiscountFeed
                                FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance renew
                                LEFT JOIN 
	                                (SELECT slm_OldPolicyNo,slm_RenewInsureId,slm_PromotionId,slm_FT, slm_OD,slm_NetGrossPremium,slm_Driver_Birthdate1,
	                                 slm_Driver_Birthdate2, T1.slm_TitleName + RC.slm_Driver_First_Name1 +' '+ RC.slm_Driver_Last_Name1  AS FullName_Driver1,
	                                 T2.slm_TitleName + RC.slm_Driver_First_Name2 +' '+ RC.slm_Driver_Last_Name2  AS FullName_Driver2
	                                FROM  " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare RC 
                                    LEFT JOIN " + AppConstant.SLMDBName + @".DBO.KKSLM_MS_TITLE T1 ON T1.slm_TitleId = RC.slm_Driver_TitleId1
	                                LEFT JOIN " + AppConstant.SLMDBName + @".DBO.KKSLM_MS_TITLE T2 ON T2.slm_TitleId = RC.slm_Driver_TitleId2
	                                WHERE slm_Selected = '1') as cp ON cp.slm_RenewInsureId = renew.slm_RenewInsureId
                                LEFT JOIN 
	                                (SELECT slm_OldPolicyNo,slm_RenewInsureId,slm_PromotionId,slm_FT,slm_NetGrossPremium
	                                FROM  " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare 
	                                Where slm_Seq = 1 and slm_NotifyPremiumId is null and slm_PromotionId is null ) AS cpOld ON cpOld.slm_RenewInsureId = renew.slm_RenewInsureId
                                LEFT JOIN 
	                                (SELECT rc.slm_RenewInsureId,np.slm_Discount_Amount, np.slm_Discount_Percent
	                                FROM  " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare rc inner join kkslm_tr_notify_premium np on np.slm_Id = rc.slm_NotifyPremiumId
	                                WHERE slm_Seq = 2 and slm_NotifyPremiumId is not null and slm_PromotionId is null) as cpNotice ON cpNotice.slm_RenewInsureId = renew.slm_RenewInsureId 
                                LEFT JOIN " + AppConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance pro ON pro.slm_PromotionId = cp.slm_PromotionId
                                LEFT JOIN " + AppConstant.OPERDBName + @".dbo.kkslm_ms_campaigninsurance ci ON ci.slm_CampaignInsuranceId = pro.slm_CampaignInsuranceId
                                WHERE renew.slm_RenewInsureId = '" + renew_ins.slm_RenewInsureId.ToString() + @"' ";

                var data2 = slmdb.ExecuteStoreQuery<ReportData>(sql).FirstOrDefault();

                kkslm_tr_notify_renewinsurance_report report = new kkslm_tr_notify_renewinsurance_report()
                {
                    slm_TicketId = renew_ins.slm_TicketId,
                    slm_InsReceiveNo = renew_ins.slm_ReceiveNo,
                    slm_InsReceiveDate = renew_ins.slm_ReceiveDate,
                    slm_Ins_Com_Id = renew_ins.slm_InsuranceComId,
                    slm_Contract_Number = renew_ins.slm_ContractNo,
                    //slm_PolicyNoOld = data2 != null ? data2.OldPolicyNo : null,
                    slm_PolicyNoOld = prelead != null ? prelead.slm_Voluntary_Policy_Number : null,
                    slm_BranchCode = prelead != null ? prelead.slm_BranchCode : null,
                    slm_CardTypeId = data != null ? data.CardType : null,
                    slm_CampaignName = data2 != null ? data2.CampaignName : null,
                    slm_SubCampaignName = null,
                    slm_Title_Id = lead.slm_TitleId,
                    slm_Name = lead.slm_Name,
                    slm_LastName = lead.slm_LastName,
                    slm_House_No = renew_addr != null ? renew_addr.slm_House_No : null,
                    slm_Moo = renew_addr != null ? renew_addr.slm_Moo : null,
                    slm_Village = renew_addr != null ? renew_addr.slm_Village : null,
                    slm_Building = renew_addr != null ? renew_addr.slm_BuildingName : null,
                    slm_Soi = renew_addr != null ? renew_addr.slm_Soi : null,
                    slm_Street = renew_addr != null ? renew_addr.slm_Street : null,
                    slm_TambolId = renew_addr != null ? renew_addr.slm_Tambon : null,
                    slm_AmphurId = renew_addr != null ? renew_addr.slm_Amphur : null,
                    slm_ProvinceId = renew_addr != null ? renew_addr.slm_Province : null,
                    slm_Zipcode = renew_addr != null ? renew_addr.slm_PostalCode : null,
                    slm_CoverageTypeId = renew_ins.slm_CoverageTypeId,
                    slm_CoverageStartDate = renew_ins.slm_PolicyStartCoverDate,
                    slm_CoverageEndDate = renew_ins.slm_PolicyEndCoverDate,
                    slm_RepairTypeId = renew_ins.slm_RepairTypeId,
                    slm_VehicleNo = "",
                    //slm_InsuranceCarTypeId = prelead != null ? prelead.slm_Car_By_Gov_Id : null,
                    //slm_InsuranceCarTypeId = data2 != null ? data2.CoverageTypeId : null,
                    slm_InsuranceCarTypeId = renew_ins.slm_InsurancecarTypeId,
                    slm_BrandCode = renew_ins.slm_RedbookBrandCode,
                    slm_ModelCode = renew_ins.slm_RedbookModelCode,
                    slm_KKKey = renew_ins.slm_RedbookKKKey,
                    slm_CarLicenseNo = renew_ins.slm_LicenseNo + (data != null ? (" " + GetProvinceName(slmdb, data.ProvinceRegis)) : ""),
                    slm_VIN = renew_ins.slm_ChassisNo,
                    slm_EngineNo = renew_ins.slm_EngineNo,
                    slm_ModelYearId = renew_ins.slm_RedbookYearGroup,
                    slm_Cc = renew_ins.slm_CC,
                    slm_WeightPerTon = null,
                    slm_Owner = lead.slm_Owner,
                    slm_Remark = renew_ins != null ? renew_ins.slm_RemarkPolicy : null,
                    slm_Discouont_Good_History = data2 != null ? data2.DiscountGoodHistory : null,
                    slm_Discount_Fleet = data2 != null ? data2.DiscountFeed : null,
                    slm_TelNo_1 = lead.slm_TelNo_1,
                    slm_CitizenId = data != null ? data.CitizenId : null,
                    slm_Birthdate = data != null ? data.BirthDate : null,
                    slm_Occupation = data != null ? data.Occupation : null,
                    slm_MaritalStatus = data != null ? data.MaritalStatus : null,
                    slm_TaxNo = prelead != null ? prelead.slm_Tax_Id : null,
                    slm_Title_Id_Committee1 = prelead != null ? prelead.slm_Guarantor_TitleId1 : null,
                    slm_Name_Committee1 = prelead != null ? prelead.slm_Guarantor_First_Name1 : null,
                    slm_LastName_Committee1 = prelead != null ? prelead.slm_Guarantor_Last_Name1 : null,
                    slm_CitizenId_Committe1 = prelead != null ? prelead.slm_Guarantor_Card_Id1: null,
                    slm_Title_Id_Committee2 = prelead != null ? prelead.slm_Guarantor_TitleId2 : null,
                    slm_Name_Committee2 = prelead != null ? prelead.slm_Guarantor_First_Name2 : null,
                    slm_LastName_Committee2 = prelead != null ? prelead.slm_Guarantor_Last_Name2 : null,
                    slm_CitizenId_Committe2 = prelead != null ? prelead.slm_Guarantor_Card_Id2 : null,
                    slm_Title_Id_Committee3 = prelead != null ? prelead.slm_Guarantor_TitleId3 : null,
                    slm_Name_Committee3 = prelead != null ? prelead.slm_Guarantor_First_Name3 : null,
                    slm_LastName_Committee3 = prelead != null ? prelead.slm_Guarantor_Last_Name3 : null,
                    slm_CitizenId_Committe3 = prelead != null ? prelead.slm_Guarantor_Card_Id3 : null,
                    slm_Export_Flag = false,
                    slm_Export_Date = null,
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedDate = createdDate,
                    slm_UpdatedBy = "SYSTEM",
                    slm_UpdatedDate = createdDate,
                    is_Deleted = false
                };

                if (prelead != null && prelead.slm_Voluntary_Cov_Amt != null)
                    report.slm_SumInsured_Year1 = prelead.slm_Voluntary_Cov_Amt;
                if (prelead != null && prelead.slm_Voluntary_Gross_Premium != null)
                    report.slm_NetPremium_Year1 = prelead.slm_Voluntary_Gross_Premium;

                var payment = paymentList.Where(p => p.TranCode == AppConstant.PaymentCode.Policy).OrderByDescending(p => p.TranDate).FirstOrDefault();
                if (payment != null)
                {
                    report.slm_InsPayDate = AppUtil.ConvertToDateTime("TRAN_DATE", payment.TranDate, AppConstant.DateTimeFormat.Format1);
                }
                if (data != null)
                {
                    if (data.CarType == "0")
                        report.slm_Cartype = "New";
                    else if (data.CarType == "1")
                        report.slm_Cartype = "Used";

                    if (data.CardType == 2)
                    {
                        report.slm_Title_Id_Receipt = lead.slm_TitleId;
                        report.slm_Name_Receipt = lead.slm_Name;
                        if (renew_addr != null)
                        {
                            report.slm_House_No_Receipt = renew_addr.slm_House_No;
                            report.slm_Village_Receipt = renew_addr.slm_Village;
                            report.slm_Building_Receipt = renew_addr.slm_BuildingName;
                            report.slm_Soi_Receipt = renew_addr.slm_Soi;
                            report.slm_Street_Receipt = renew_addr.slm_Street;
                            report.slm_TambolId_Receipt = renew_addr.slm_Tambon;
                            report.slm_AmphurId_Receipt = renew_addr.slm_Amphur;
                            report.slm_ProvinceId_Receipt = renew_addr.slm_Province;
                            report.slm_Zipcode_Receipt = renew_addr.slm_PostalCode;
                        }
                    }
                }
                if (data2 != null)
                { 
                    if (data2.SumInsured_Year2 != null) report.slm_SumInsured_Year2 = data2.SumInsured_Year2;
                    if (data2.SumInsured_Year2_FT != null) report.slm_SumInsured_Year2_FT = data2.SumInsured_Year2_FT;
                    if (data2.NetPremium_Year2 != null) report.slm_NetPremium_Year2 = data2.NetPremium_Year2;

                    report.slm_Birthdate_Driver1 = (string.IsNullOrEmpty(data2.FullName_Driver1) ? "" : data2.FullName_Driver1 + " ") + (data2.Birthdate_Driver1 != null ? (data2.Birthdate_Driver1.Value.ToString("dd/MM/") + (data2.Birthdate_Driver1.Value.Year + 543).ToString()) : "");
                    report.slm_Birthdate_Driver2 = (string.IsNullOrEmpty(data2.FullName_Driver2) ? "" : data2.FullName_Driver2 + " ") + (data2.Birthdate_Driver2 != null ? (data2.Birthdate_Driver2.Value.ToString("dd/MM/") + (data2.Birthdate_Driver2.Value.Year + 543).ToString()) : "");
                }

                slmdb.kkslm_tr_notify_renewinsurance_report.AddObject(report);
                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetProvinceName(SLMDBEntities slmdb, int? provinceId)
        {
            var name = slmdb.kkslm_ms_province.Where(p => p.slm_ProvinceId == provinceId).Select(p => p.slm_ProvinceNameTH).FirstOrDefault();
            return name != null ? name : "";
        }

        //รายงานระงับ claim
        private void InsertSettleClaimReport(SLMDBEntities slmdb, string ticketId, DateTime createdDate, kkslm_tr_renewinsurance renew_ins)
        {
            try
            {
                renew_ins.slm_ClaimFlag = "1";

                string sql = @"SELECT REN.slm_ReceiveNo  AS InsReceiveNo , REN.slm_InsuranceComId AS InsComId,
                                    REN.slm_ReceiveDate AS InsReceiveDate, REN.slm_ContractNo AS ContractNo,
                                    LEAD.slm_TitleId AS TitleId,LEAD.slm_Name AS Firstname, LEAD.slm_LastName AS Lastname,
                                    REN.slm_PolicyStartCoverDate AS CoverageStartDate,REN.slm_PolicyEndCoverDate AS CoverageEndDate,
                                    REN.slm_LicenseNo AS CarLicenseNo, ST.slm_EmpCode AS OwnerEmpCode
                                FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_lead LEAD 
                                INNER JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance REN ON REN.slm_TicketId = LEAD.slm_ticketId
                                LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner
                                WHERE ren.slm_TicketId = '" + ticketId + "' ";

                var data = slmdb.ExecuteStoreQuery<SettleClaimReport>(sql).FirstOrDefault();
                if (data != null)
                {
                    kkslm_tr_setle_claim report = new kkslm_tr_setle_claim()
                    {
                        slm_CarLicenseNo = data.CarLicenseNo,
                        slm_Contract_Number = data.ContractNo,
                        slm_CoverageStartDate = data.CoverageStartDate,
                        slm_CoverageEndDate = data.CoverageEndDate,
                        slm_Ins_Com_Id = data.InsComId,
                        slm_InsReceiveNo = data.InsReceiveNo,
                        slm_InsReceiveDate = data.InsReceiveDate,
                        slm_Title_Id = data.TitleId,
                        slm_Name = data.Firstname,
                        slm_LastName = data.Lastname,
                        slm_Owner = data.OwnerEmpCode,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createdDate,
                    };
                    slmdb.kkslm_tr_setle_claim.AddObject(report);
                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //รายงานยกเลิกระงับ Claim
        private void InsertCancelSettleClaimReport(SLMDBEntities slmdb, string ticketId, DateTime createdDate, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew_ins)
        {
            try
            {
                string provinceName = (from p in slmdb.kkslm_tr_productinfo
                                       join pv in slmdb.kkslm_ms_province on p.slm_ProvinceRegis equals pv.slm_ProvinceId
                                       where p.slm_TicketId == ticketId
                                       select pv.slm_ProvinceNameTH).FirstOrDefault();

                provinceName = provinceName == null ? "" : provinceName;

                kkslm_tr_setle_claim_cancel_report cancel_report = new kkslm_tr_setle_claim_cancel_report()
                {
                    slm_CarLicenseNo = renew_ins.slm_LicenseNo + (provinceName != "" ? " " + provinceName : ""),
                    slm_Contract_Number = renew_ins.slm_ContractNo,
                    slm_CoverageStartDate = renew_ins.slm_PolicyStartCoverDate,
                    slm_CoverageEndDate = renew_ins.slm_PolicyEndCoverDate,
                    slm_Export_Date = null,
                    slm_Export_Flag = false,
                    slm_Ins_Com_Id = renew_ins.slm_InsuranceComId,
                    slm_InsReceiveNo = renew_ins.slm_ReceiveNo,
                    slm_InsReceiveDate = renew_ins.slm_ReceiveDate,
                    slm_Title_Id = lead.slm_TitleId,
                    slm_Name = lead.slm_Name,
                    slm_LastName = lead.slm_LastName,
                    slm_Owner = lead.slm_Owner,
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedDate = createdDate,
                    slm_UpdatedBy = "SYSTEM",
                    slm_UpdatedDate = createdDate
                };
                slmdb.kkslm_tr_setle_claim_cancel_report.AddObject(cancel_report);
                renew_ins.slm_ClaimFlag = "0";

                kkslm_tr_activity act = new kkslm_tr_activity()
                {
                    slm_TicketId = ticketId,
                    slm_OldStatus = null,
                    slm_OldSubStatus = null,
                    slm_NewStatus = null,
                    slm_NewSubStatus = null,
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedBy_Position = null,
                    slm_CreatedDate = createdDate,
                    slm_Type = AppConstant.LoggingType.CancelSuspendedClaim,    //ยกเลิกระงับเคลม = 20
                    slm_SystemAction = "SLM",
                    slm_SystemActionBy = "SLM",
                    slm_ExternalSystem_Old = null,
                    slm_ExternalStatus_Old = null,
                    slm_ExternalSubStatus_Old = null,
                    slm_ExternalSubStatusDesc_Old = null,
                    slm_ExternalStatus_New = null,
                    slm_ExternalSubStatus_New = null,
                    slm_ExternalSystem_New = null,
                    slm_ExternalSubStatusDesc_New = null
                };
                slmdb.kkslm_tr_activity.AddObject(act);

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //รายงานแจ้งต่อ พรบ
        private void InsertActIssueReport(SLMDBEntities slmdb, string ticketId, DateTime createdDate)
        {
            try
            {
                string slmdbName = AppConstant.SLMDBName;
                string sql = @"SELECT REN.slm_ActComId  AS InsComId, comact.slm_ActSignNo AS ActNo,
                                pre.slm_BranchCode as BranchCode,ren.slm_ContractNo as ContractNo,
                                LEAD.slm_TitleId AS TitleId, LEAD.slm_Name AS Firstname, LEAD.slm_LastName AS Lastname, ISNULL(REN.slm_LicenseNo, '') + ' ' + ISNULL(proregis.slm_ProvinceNameTH, '') AS CarLicenseNo,
                                REN.slm_RedbookBrandCode AS RedbookBrandCode, REN.slm_ReceiveNo AS ReceiveNo, ST.slm_EmpCode AS OwnerEmpCode,
                                REN.slm_ChassisNo AS EngineNo, REN.slm_ActNetPremium as NetPremium,
                                ISNULL(REN.slm_ActNetPremium, 0) + ISNULL(REN.slm_ActVat, 0) + ISNULL(REN.slm_ActStamp, 0) AS NetPremiumIncludeVat ,
                                REN.slm_ActStartCoverDate  AS StartDateAct, REN.slm_ActEndCoverDate AS EndDateAct,
                                RA.slm_House_No AS HouseNo, RA.slm_Moo AS Moo, RA.slm_Village AS Village, RA.slm_BuildingName AS BuildingName, RA.slm_Soi AS Soi,
                                RA.slm_Street AS Street,RA.slm_Tambon AS TambolId, RA.slm_Amphur AS AmphurId, RA.slm_Province AS ProvinceId, 
                                RA.slm_PostalCode AS Zipcode, REN.slm_RemarkAct AS Remark, LEAD.slm_TelNo_1 AS TelNoContact,
                                CUS.slm_CitizenId AS CitizenId , pre.slm_Guarantor_TitleId1 as TitleId_Committee1,
                                PRE.slm_Guarantor_First_Name1 AS Name_Committee1, PRE.slm_Guarantor_Last_Name1 AS LastName_Committee1,
                                PRE.slm_Guarantor_Card_Id1 AS CitizenId_Committe1,PRE.slm_Guarantor_TitleId2 AS TitleId_Committee2,
                                PRE.slm_Guarantor_First_Name2 AS Name_Committee2, PRE.slm_Guarantor_Last_Name2 AS LastName_Committee2,
                                PRE.slm_Guarantor_Card_Id2 AS CitizenId_Committe2, PRE.slm_Guarantor_TitleId3 AS TitleId_Committee3,
                                PRE.slm_Guarantor_First_Name3 AS Name_Committee3, PRE.slm_Guarantor_Last_Name3 AS LastName_Committee3,
                                PRE.slm_Guarantor_Card_Id3 AS CitizenId_Committe3
                                FROM " + slmdbName + @".dbo.kkslm_tr_lead LEAD 
                                INNER JOIN " + slmdbName + @".dbo.kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                                INNER JOIN " + slmdbName + @".dbo.kkslm_tr_cusinfo CUS ON CUS.slm_TicketId = LEAD.slm_ticketId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_renewinsurance_compare_act comact ON comact.slm_RenewInsureId = REN.slm_RenewInsureId AND comact.slm_ActPurchaseFlag = 1
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_renewinsurance_address RA ON RA.slm_RenewInsureId = REN.slm_RenewInsureId AND slm_AddressType = 'D' 
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner 
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_prelead pre on pre.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_productinfo prod ON prod.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_province proregis ON prod.slm_ProvinceRegis = proregis.slm_ProvinceId
                                WHERE lead.slm_ticketId = '" + ticketId + "' ";

                var data = slmdb.ExecuteStoreQuery<ActIssueReport>(sql).FirstOrDefault();
                if (data != null)
                {
                    kkslm_tr_notify_act_report report = new kkslm_tr_notify_act_report()
                    {
                        slm_TicketId = ticketId,
                        slm_ActNo = data.ActNo,
                        slm_BranchCode = data.BranchCode,
                        slm_BrandCode = data.RedbookBrandCode,
                        slm_Building = data.BuildingName,
                        slm_CarLicenseNo = data.CarLicenseNo,
                        slm_CitizenId = data.CitizenId,
                        slm_CitizenId_Committe1 = data.CitizenId_Committe1,
                        slm_CitizenId_Committe2 = data.CitizenId_Committe2,
                        slm_CitizenId_Committe3 = data.CitizenId_Committe3,
                        slm_TitleId_Committee1 = data.TitleId_Committee1,
                        slm_TitleId_Committee2 = data.TitleId_Committee2,
                        slm_TitleId_Committee3 = data.TitleId_Committee3,
                        slm_Name_Committee1 = data.Name_Committee1,
                        slm_Name_Committee2 = data.Name_Committee2,
                        slm_Name_Committee3 = data.Name_Committee3,
                        slm_LastName_Committee1 = data.LastName_Committee1,
                        slm_LastName_Committee2 = data.LastName_Committee2,
                        slm_LastName_Committee3 = data.LastName_Committee3,
                        slm_Contract_Number = data.ContractNo,
                        slm_StartDateAct = data.StartDateAct,
                        slm_EndDateAct = data.EndDateAct,            
                        slm_Ins_Com_Id = data.InsComId,
                        slm_InsReceiveNo = data.ReceiveNo,
                        slm_TitleId = data.TitleId,
                        slm_Name = data.Firstname,
                        slm_LastName = data.Lastname,
                        slm_House_No = data.HouseNo,
                        slm_Soi = data.Soi,
                        slm_Village = data.Village,
                        slm_Moo = data.Moo,
                        slm_Street = data.Street,
                        slm_TambolId = data.TambolId,
                        slm_AmphurId = data.AmphurId,
                        slm_ProvinceId = data.ProvinceId,
                        slm_Zipcode = data.Zipcode,
                        slm_EngineNo = data.EngineNo,   //ใส่ค่า ChassisNo
                        slm_NetPremium = data.NetPremium,
                        slm_NetPremiumIncludeVat = data.NetPremiumIncludeVat,
                        slm_Owner = data.OwnerEmpCode,
                        slm_Remark = data.Remark,
                        slm_TelNoContact = data.TelNoContact,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createdDate,
                        is_Deleted = false
                    };
                    slmdb.kkslm_tr_notify_act_report.AddObject(report);
                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertPrepareEmail(SLMDBEntities slmdb, kkslm_tr_lead lead, List<TransactionData> paymentList, DateTime createdDate, bool policyPurchased, bool actPurchased, decimal? total_paid_policy, decimal? total_paid_act, LeadData data)
        {
            try
            {
                string filePath = AppConstant.EmailTemplatePaymentPath;

                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("ไม่พบ Config EmailTemplatePaymentPath ใน Configuration File");

                string ticketId = lead.slm_ticketId;
                bool sendEmail = false;
                string emailSubject = "";
                string template = "";
                DateTime tranDate = new DateTime();

                //ประกัน
                string strPolicyRecNoList = "";
                string strPolicyAmountList = "";

                var policyRecNoList = paymentList.Where(p => p.TranCode == AppConstant.PaymentCode.Policy).Select(p => p.RecNo).Distinct().ToList();
                if (policyRecNoList.Count > 0)
                {
                    policyRecNoList.ForEach(p => {
                        strPolicyRecNoList += (strPolicyRecNoList != "" ? ", " : "") + p.ToString().Trim();

                        decimal amountByRecNo = paymentList.Where(a => a.TranCode == AppConstant.PaymentCode.Policy && a.RecNo == p.ToString().Trim())
                                                .Sum(b => b.RecAmount == null ? 0 : b.RecAmount.Value);

                        strPolicyAmountList += (strPolicyAmountList != "" ? ", " : "") + amountByRecNo.ToString("#,##0.00");
                    });
                    if (!string.IsNullOrEmpty(strPolicyAmountList))
                        strPolicyAmountList += " บาท";
                }

                //พรบ.
                string strActRecNoList = "";
                string strActAmountList = "";

                var actRecNoList = paymentList.Where(p => p.TranCode == AppConstant.PaymentCode.Act).Select(p => p.RecNo).Distinct().ToList();
                if (actRecNoList.Count > 0)
                {
                    actRecNoList.ForEach(p => {
                        strActRecNoList += (strActRecNoList != "" ? ", " : "") + p.ToString().Trim();

                        decimal amountByRecNo = paymentList.Where(a => a.TranCode == AppConstant.PaymentCode.Act && a.RecNo == p.ToString().Trim())
                                                .Sum(b => b.RecAmount == null ? 0 : b.RecAmount.Value);

                        strActAmountList += (strActAmountList != "" ? ", " : "") + amountByRecNo.ToString("#,##0.00");
                    });
                    if (!string.IsNullOrEmpty(strActAmountList))
                        strActAmountList += " บาท";
                }

                if (paymentList.Count > 0)
                {
                    tranDate = AppUtil.ConvertToDateTime(eBDW.TRAN_DATE.ToString(), paymentList[0].TranDate, AppConstant.DateTimeFormat.Format1);
                }

                string type = GetFieldType(slmdb, lead.slm_CampaignId, lead.slm_Product_Id, "V");
                string actSendDate = data.ActSendDate != null ? (data.ActSendDate.Value.ToString("dd/MM/") + data.ActSendDate.Value.Year.ToString()) : "";

                template = File.ReadAllText(filePath, Encoding.UTF8);
                template = template.Replace("%ReceiveNo%", data.ReceiveNo)
                                    .Replace("%ActSendDate%", actSendDate)
                                    .Replace("%TicketId%", ticketId)
                                    .Replace("%ContractNo%", data.ContractNo)
                                    .Replace("%CustomerName%", data.CustomerName)
                                    .Replace("%BranchCode%", data.BranchCode)
                                    .Replace("%BranchName%", data.BranchName)
                                    .Replace("%RecNo%", strPolicyRecNoList)
                                    .Replace("%TranCode%", AppConstant.PaymentCode.Policy)
                                    .Replace("%PolicyPaidAmount%", strPolicyAmountList)
                                    .Replace("%RecNoAct%", strActRecNoList)
                                    .Replace("%TranCodeAct%", AppConstant.PaymentCode.Act)
                                    .Replace("%ActPaidAmount%", strActAmountList)
                                    .Replace("%PaidDate%", tranDate.ToString("dd/MM/") + tranDate.Year.ToString())
                                    .Replace("%CreatedDate%", createdDate.ToString("dd/MM/") + createdDate.Year.ToString())
                                    .Replace("%Type%", type);

                if (policyPurchased)
                    template = template.Replace("%IncentiveFlag%", data.IncentiveFlag == true ? "Yes" : "No");
                else
                    template = template.Replace("%IncentiveFlag%", "");

                if (actPurchased)
                    template = template.Replace("%IncentiveFlagAct%", data.IncentiveFlagAct == true ? "Yes" : "No");
                else
                    template = template.Replace("%IncentiveFlagAct%", "");

                emailSubject = "[" + data.StatusDesc + "]";
                emailSubject += string.IsNullOrEmpty(data.SubStatusDesc) ? "" : ("-[" + data.SubStatusDesc + "]");
                emailSubject += " SLM: Ticket: " + ticketId + " ได้รับรายการรับชำระ/แก้ไขใบเสร็จ : " + data.CampaignName
                                    + " ลูกค้า (" + data.ContractNo + " " + data.CustomerName + " " + data.BranchName + ") " + createdDate.ToString("dd/MM/") + createdDate.Year.ToString();

                //send email
                if (data != null && data.OwnerEmail != null && data.OwnerEmail.Trim() != "")
                {
                    sendEmail = true;
                    kkslm_prepare_email email = new kkslm_prepare_email()
                    {
                        slm_ticketId = ticketId,
                        slm_EmailAddress = data.OwnerEmail,
                        slm_EmailSubject = emailSubject,
                        slm_EmailContent = template,
                        slm_EmailSender = "SYSTEM",
                        slm_ExportStatus = "0",
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createdDate,
                        is_Deleted = 0
                    };
                    slmdb.kkslm_prepare_email.AddObject(email);
                }

                //insert note
                kkslm_note note = new kkslm_note()
                {
                    slm_TicketId = ticketId,
                    slm_EmailSubject = emailSubject,
                    slm_SendEmailFlag = sendEmail,
                    slm_NoteDetail = template.Replace("Email Message:", ""),
                    slm_CreateBy = "SYSTEM",
                    slm_CreatedBy_Position = null,
                    slm_CreateDate = createdDate
                };
                slmdb.kkslm_note.AddObject(note);

                lead.slm_NoteFlag = "1";
                lead.slm_UpdatedBy = "SYSTEM";
                lead.slm_UpdatedDate = createdDate;

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetFieldType(SLMDBEntities slmdb, string campaignId, string productId, string actionType)
        {
            try
            {
                string type = string.Empty;

                type = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_CampaignId == campaignId && p.slm_ActionType == actionType && p.is_Deleted == false)
                            .Select(p => p.slm_Type).FirstOrDefault();

                if (string.IsNullOrEmpty(type))
                {
                    type = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_Product_Id == productId && p.slm_ActionType == actionType && p.is_Deleted == false)
                            .Select(p => p.slm_Type).FirstOrDefault();
                }

                return type != null ? type : "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<InsuranceCompanyData> GetInsuranceCompany(SLMDBEntities slmdb)
        {
            try
            {
                string sql = @"SELECT slm_Ins_Com_Id AS InsComId, slm_InsCode AS InsComCode, slm_InsNameTh AS InsNameTH, slm_TelContact AS TelContact
                                FROM " + AppConstant.OPERDBName + ".dbo.kkslm_ms_ins_com WHERE is_Deleted = 0";

                return slmdb.ExecuteStoreQuery<InsuranceCompanyData>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private void InsertPrepareSMS(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew, List<InsuranceCompanyData> companyList, DateTime createdDate)
        //{
        //    try
        //    {
        //        string filePath = AppConstant.SMSTemplatePath;

        //        if (string.IsNullOrEmpty(filePath))
        //            throw new Exception("ไม่พบ Config SMSTemplatePath ใน Configuration File");

        //        InsuranceCompanyData insur_com = companyList.Where(p => p.InsComId == renew.slm_InsuranceComId).FirstOrDefault();

        //        string template = File.ReadAllText(filePath, Encoding.UTF8);
        //        template = template.Replace("%CarLicenseNo%", renew.slm_LicenseNo)
        //                            .Replace("%InsuranceCompany%", insur_com != null ? insur_com.InsNameTH : "")
        //                            .Replace("%ReceiveNo%", renew.slm_ReceiveNo)
        //                            .Replace("%InsuranceTelNo%", insur_com != null ? insur_com.TelContact : "");

        //        kkslm_tr_prelead_prepare_sms sms = new kkslm_tr_prelead_prepare_sms()
        //        {
        //            slm_ticketId = renew.slm_TicketId,
        //            slm_Message = template,
        //            slm_Message_CreatedBy = "SYSTEM",
        //            slm_MessageStatusId = "1",
        //            slm_PhoneNumber = lead != null ? lead.slm_TelNo_1 : null,
        //            slm_QueueId = "6",
        //            slm_RequestDate = createdDate,
        //            slm_RuleActivityId = 0,
        //            slm_ExportStatus = "0",
        //            slm_RefId = null,
        //            slm_SendingStatusCode = null,
        //            slm_SendingStatus = null,
        //            slm_ErrorCode = null,
        //            slm_ErrorReason = null,
        //            slm_CAS_Flag = null,
        //            slm_CAS_Date = null,
        //            slm_FlagType = "2",
        //            slm_CreatedBy = "SYSTEM",
        //            slm_CreatedDate = createdDate,
        //            slm_UpdatedBy = "SYSTEM",
        //            slm_UpdatedDate = createdDate,
        //            is_Deleted = 0
        //        };
        //        slmdb.kkslm_tr_prelead_prepare_sms.AddObject(sms);
        //        slmdb.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private void CreateCASActivityLog(kkslm_tr_renewinsurance reins, SLMDBEntities slmdb, int phonecall_id, bool policyPurchased, bool actPurchased, decimal? current_paid_policy, decimal? current_paid_act, decimal? total_paid_policy, decimal? total_paid_act)
        {
            try
            {
                var data = AppUtil.GetDataForCARLogService(reins.slm_TicketId, slmdb);
                string preleadId = "";
                string actSendDate = "";

                if (data != null)
                {
                    preleadId = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
                    actSendDate = data.ActSendDate != null ? (data.ActSendDate.Value.ToString("dd-MM-") + data.ActSendDate.Value.Year.ToString()) : "";
                }

                string incentiveValue = "";
                string incentiveActValue = "";

                if (policyPurchased)
                {
                    incentiveValue = data != null ? (data.IncentiveFlag == true ? "Y" : "N") : "N";
                }
                if (actPurchased)
                {
                    incentiveActValue = data != null ? (data.IncentiveFlagAct == true ? "Y" : "N") : "N";
                }

                //Activity Info
                List<CARService.DataItem> actInfoList = new List<CARService.DataItem>();
                actInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่รับแจ้ง", DataValue = data != null ? data.ReceiveNo : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "วันที่ส่งแจ้ง พรบ.", DataValue = actSendDate });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "Incentive ประกัน", DataValue = incentiveValue });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 4, DataLabel = "Incentive พรบ.", DataValue = incentiveActValue });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 5, DataLabel = "ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้", DataValue = current_paid_policy != null ? current_paid_policy.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 6, DataLabel = "ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้", DataValue = current_paid_act != null ? current_paid_act.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 7, DataLabel = "ค่าเบี้ยประกันที่ลูกค้าชำระรวม", DataValue = total_paid_policy != null ? total_paid_policy.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 8, DataLabel = "ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม", DataValue = total_paid_act != null ? total_paid_act.Value.ToString("#,##0.00") : "" });

                //Customer Info
                List<CARService.DataItem> cusInfoList = new List<CARService.DataItem>();
                cusInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = data != null ? data.CitizenId : "" });
                cusInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = data != null ? data.CardTypeName : "" });
                cusInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = data != null ? data.CustomerName : "" });

                //Product Info
                List<CARService.DataItem> prodInfoList = new List<CARService.DataItem>();
                prodInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = data != null ? data.ProductGroupName : "" });
                prodInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = data != null ? data.ProductName : "" });
                prodInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = data != null ? data.CampaignName : "" });

                //Contract Info
                List<CARService.DataItem> contInfoList = new List<CARService.DataItem>();
                contInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
                contInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = preleadId != "" ? "HP" : "" });
                contInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.LicenseNo : "" });

                //Officer Info
                List<CARService.DataItem> offInfoList = new List<CARService.DataItem>();
                offInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = "SYSTEM" });

                CARService.CARServiceData logdata = new CARService.CARServiceData()
                {
                    ReferenceNo = reins.slm_RenewInsureId.ToString(),
                    SecurityKey = preleadId != "" ? AppConstant.CARLogService.OBTSecurityKey : AppConstant.CARLogService.SLMSecurityKey,
                    ServiceName = "CreateActivityLog",
                    SystemCode = preleadId != "" ? AppConstant.CARLogService.CARLoginOBT : AppConstant.CARLogService.CARLoginSLM,           //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
                    TransactionDateTime = DateTime.Now,
                    ActivityInfoList = actInfoList,
                    CustomerInfoList = cusInfoList,
                    ProductInfoList = prodInfoList,
                    ContractInfoList = contInfoList,
                    OfficerInfoList = offInfoList,
                    ActivityDateTime = DateTime.Now,
                    CampaignId = data != null ? data.CampaignId : "",
                    ChannelId = data != null ? data.ChannelId : "",
                    PreleadId = preleadId,
                    ProductGroupId = data != null ? data.ProductGroupId : "",
                    ProductId = data != null ? data.ProductId : "",
                    Status = data != null ? data.StatusName : "",
                    SubStatus = data != null ? data.SubStatusName : "",
                    TicketId = data != null ? data.TicketId : "",
                    SubscriptionId = data != null ? data.CitizenId : "",
                    TypeId = AppConstant.CARLogService.Data.TypeId,
                    AreaId = AppConstant.CARLogService.Data.AreaId,
                    SubAreaId = AppConstant.CARLogService.Data.SubAreaId,
                    ActivityTypeId = AppConstant.CARLogService.Data.ActivityType.TodoId,
                    ContractNo = data != null ? data.ContractNo : ""
                };

                if (data != null && !string.IsNullOrEmpty(data.SubScriptionTypeId))
                    logdata.SubscriptionTypeId = data.SubScriptionTypeId;

                bool ret = CARService.CreateActivityLog(logdata, BatchMonitorId, BatchCode, reins.slm_TicketId, logfilename);
                AppUtil.UpdatePhonecallCASFlag(slmdb, phonecall_id, ret ? "1" : "2");
            }
            catch (Exception ex)
            {
                AppUtil.UpdatePhonecallCASFlag(slmdb, phonecall_id, "2");

                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BizUtil.InsertLog(BatchMonitorId, reins.slm_TicketId, "", "Error in Method CreateCASActivityLog: " + message);
                Util.WriteLogFile(logfilename, BatchCode, "Error in Method CreateCASActivityLog: " + message);
            }
        }
        
        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //set null
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public class ContractNoAmount
    {
        public string ContractNo { get; set; }
        public int Amount { get; set; }
        //Add By Kitipong.cho
        public string SourceSystem { get; set; }
    }

    public class TransactionData
    {
        public string ContractNumber { get; set; }
        public string TranCode { get; set; }
        public string InsNoDesc { get; set; }
        public string InstNo { get; set; }
        public string RecBy { get; set; }
        public string RecNo { get; set; }
        public decimal? RecAmount { get; set; }
        public string TranDate { get; set; }
        public string UpdateDate { get; set; }
        public string DataSource { get; set; }
    }

    public class BenefitData
    {
        public string Scenario { get; set; }
        public decimal? slm_BenefitId { get; set; }
        public string slm_Product_Id { get; set; }
        public decimal? slm_Ins_Com_Id { get; set; }
        public decimal? slm_CampaignInsuranceId { get; set; }
        public string slm_ComissionFlag { get; set; }
        public decimal? slm_ComissionPercentValue { get; set; }
        public decimal? slm_ComissionBathValue { get; set; }
        public string slm_OV1Flag { get; set; }
        public decimal? slm_OV1PercentValue { get; set; }
        public decimal? slm_OV1BathValue { get; set; }
        public string slm_OV2Flag { get; set; }
        public decimal? slm_OV2PercentValue { get; set; }
        public decimal? slm_OV2BathValue { get; set; }
        public string slm_VatFlag { get; set; }
        public string slm_BenefitTypeCode { get; set; }
        public int? slm_InsurancecarTypeId { get; set; }
        public int? slm_CoverageTypeId { get; set; }
        public string slm_PolicyReferenceNote { get; set; }
        public string slm_ActReferenceNote { get; set; }
    }

    public class ReceiveNoData
    {
        public string ProductId { get; set; }
        public decimal? InsuranceComId { get; set; }
    }

    public class ReportData
    {
        public string OldPolicyNo { get; set; }
        public string CampaignName { get; set; }
        public decimal? SumInsured_Year1 { get; set; }
        public decimal? NetPremium_Year1 { get; set; }
        public decimal? SumInsured_Year2 { get; set; }
        public decimal? SumInsured_Year2_FT { get; set; }
        public decimal? NetPremium_Year2 { get; set; }
        public string FullName_Driver1 { get; set; }
        public string FullName_Driver2 { get; set; }
        public DateTime? Birthdate_Driver1 { get; set; }
        public DateTime? Birthdate_Driver2 { get; set; }
        public decimal? DiscountGoodHistory { get; set; }
        public int? DiscountFeed { get; set; }
        public int? CoverageTypeId { get; set; }        //Added by Pom 18/08/2016
    }

    public class ReceiveNoInfo
    {
        public decimal? ReceiveNoListId { get; set; }
        public string ReceiveNo { get; set; }
    }

    public class LeadData
    {
        public string TicketId { get; set; }
        public string ContractNo { get; set; }
        public string CampaignName { get; set; }
        public string CustomerName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Owner { get; set; }
        public string OwnerEmail { get; set; }
        public string Delegate { get; set; }
        public string DelegateEmail { get; set; }
        public string ReceiveNo { get; set; }
        public bool? IncentiveFlag { get; set; }
        public bool? IncentiveFlagAct { get; set; }
        public DateTime? ActSendDate { get; set; }
        public string StatusDesc { get; set; }
        public string SubStatusDesc { get; set; }
    }

    public class SettleClaimReport
    {
        public string InsReceiveNo { get; set; }
        public decimal? InsComId { get; set; }
        public DateTime? InsReceiveDate { get; set; }
        public string ContractNo { get; set; }
        public int? TitleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? CoverageStartDate { get; set; }
        public DateTime? CoverageEndDate { get; set; }
        public string CarLicenseNo { get; set; }
        public string OwnerEmpCode { get; set; }
    }

    public class ActIssueReport
    {
        public decimal? InsComId { get; set; }
        public string ActNo { get; set; }
        public string BranchCode { get; set; }
        public string ContractNo { get; set; }
        public int? TitleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string CarLicenseNo { get; set; }
        public string RedbookBrandCode { get; set; }
        public string ReceiveNo { get; set; }
        public string OwnerEmpCode { get; set; }
        public string EngineNo { get; set; }
        public decimal? NetPremium { get; set; }
        public decimal? NetPremiumIncludeVat { get; set; }
        public DateTime? StartDateAct { get; set; }
        public DateTime? EndDateAct { get; set; }
        public string HouseNo { get; set; }
        public string Moo { get; set; }
        public string Village { get; set; }
        public string BuildingName { get; set; }
        public string Soi { get; set; }
        public string Street { get; set; }
        public int? TambolId { get; set; }
        public int? AmphurId { get; set; }
        public int? ProvinceId { get; set; }
        public string Zipcode { get; set; }
        public string Remark { get; set; }
        public string TelNoContact { get; set; }
        public string CitizenId { get; set; }
        public int? TitleId_Committee1 { get; set; }
        public string Name_Committee1 { get; set; }
        public string LastName_Committee1 { get; set; }
        public string CitizenId_Committe1 { get; set; }
        public int? TitleId_Committee2 { get; set; }
        public string Name_Committee2 { get; set; }
        public string LastName_Committee2 { get; set; }
        public string CitizenId_Committe2 { get; set; }
        public int? TitleId_Committee3 { get; set; }
        public string Name_Committee3 { get; set; }
        public string LastName_Committee3 { get; set; }
        public string CitizenId_Committe3 { get; set; }
    }
}
