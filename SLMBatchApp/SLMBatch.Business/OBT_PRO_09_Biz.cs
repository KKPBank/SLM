using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;

namespace SLMBatch.Business
{
    public class OBT_PRO_09_Biz : ServiceBase
    {
        enum eBDW
        {
            CONTRACT_NUMBER = 0,
            POLICY_TYPE,
            POLICY_NO,
            POLICY_YEAR,
            MAIN_BY,
            MAIN_DATE,
            MAIN_TIME,
            CREATE_BY,
            CREATE_DATE,
            CREATE_TIME,
            DATA_SOURCE
        }

        /// <summary>
        /// Update ข้อมูลเลขเล่มกรมธรรม์, เลขพรบ. จาก DataWarehouse ไปที่ OBT
        /// </summary>
        /// <param name="batchCode"></param>
        public bool UpdatePolicyNo(string batchCode)
        {
            // รอเทส SIT2

            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;
            string ticketId = "";
            string[] statusExclude = new string[] { "08", "09", "10" };
            bool ret = false;

            try
            {
                BatchCode = batchCode;
                BatchMonitorId = BizUtil.SetStartTime(batchCode);
                BizUtil.CheckPrerequisite(batchCode);

                DataTable dtPolicy = GetPolicyNoFromBDW(BatchMonitorId, batchCode);

                string contractListBDW = string.Join(",", dtPolicy.AsEnumerable().Select(y => y.Field<string>("CONTRACT_NUMBER")).Distinct().ToArray());

                var pendingList = new List<kkslm_tr_renewinsurance_hp_policyno_actno_pending>();
                using (SLMDBEntities db = AppUtil.GetSlmDbEntities())
                {
                    pendingList = db.kkslm_tr_renewinsurance_hp_policyno_actno_pending.Where(p => p.is_Deleted == false && !contractListBDW.Contains(p.slm_Contract_Number)).ToList();
                }

                //Move data from pending to precess again
                foreach (var item in pendingList)
                {
                    DataRow dr = dtPolicy.NewRow();
                    dr[eBDW.CONTRACT_NUMBER.ToString()] = item.slm_Contract_Number;
                    dr[eBDW.POLICY_YEAR.ToString()] = item.slm_Policy_Year;
                    dr[eBDW.POLICY_NO.ToString()] = item.slm_Policy_No;
                    dr[eBDW.POLICY_TYPE.ToString()] = item.slm_Policy_Type;
                    dr[eBDW.MAIN_BY.ToString()] = item.slm_Main_By;
                    dr[eBDW.MAIN_DATE.ToString()] = item.slm_Main_Date;
                    dr[eBDW.MAIN_TIME.ToString()] = item.slm_Main_Time;
                    dr[eBDW.CREATE_BY.ToString()] = item.slm_Create_By;
                    dr[eBDW.CREATE_DATE.ToString()] = item.slm_Create_Date;
                    dr[eBDW.CREATE_TIME.ToString()] = item.slm_Create_Time;
                    dr[eBDW.DATA_SOURCE.ToString()] = "PENDING";
                    dtPolicy.Rows.Add(dr);
                }

                var contractInsureList = ConvretToList(dtPolicy);

                int countTotalBDW = contractInsureList.Count(p => p.DataSource == "BDW");
                int countTotalPending = contractInsureList.Count(p => p.DataSource == "PENDING");

                //Snap DataSource
                SnapDataSource(contractInsureList);

                //Validate List
                ValidateList(contractInsureList);

                //Log amount of actual data to process
                int countBDW = contractInsureList.Count(p => p.DataSource == "BDW");
                int countPending = contractInsureList.Count(p => p.DataSource == "PENDING");
                string msg = string.Format("Data from BDW {0}/{1} (Total/To be Processed) records, Data to process from Pending {2}/{3} (Total/To be Processed) records", countTotalBDW.ToString(), countBDW.ToString(), countTotalPending.ToString(), countPending.ToString());
                Util.WriteLogFile(logfilename, BatchCode, msg);
                BizUtil.InsertLog(BatchMonitorId, "", "", msg);

                totalRecord = contractInsureList.Count;
                var contractNoList = contractInsureList.Select(p => p.ContractNo).Distinct().ToList();

                var insurComList = GetInsuranceCompany();
                var coverageList = GetCoverageTypeList();

                foreach (string contractNo in contractNoList)
                {
                    ticketId = "";
                    try
                    {
                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                        {
                            using (SLMDBEntities slmdb = AppUtil.GetSlmDbEntities())
                            {
                                var renewList = (from r in slmdb.kkslm_tr_renewinsurance
                                                    join l in slmdb.kkslm_tr_lead on r.slm_TicketId equals l.slm_ticketId
                                                    where r.slm_ContractNo == contractNo && l.is_Deleted == 0
                                                    && statusExclude.Contains(l.slm_Status) == false
                                                    select r).ToList();

                                if (renewList.Count > 0)
                                {
                                    if (renewList.Count == 1)
                                    {
                                        var list = contractInsureList.Where(p => p.ContractNo == contractNo).ToList();
                                        var renew = renewList.FirstOrDefault();
                                        ticketId = renew.slm_TicketId;

                                        DoProcess(slmdb, renew, list, insurComList, coverageList);
                                    }
                                    else
                                    {
                                        InsertPolicyActPending(contractInsureList, contractNo, slmdb, AppConstant.PendingType.DuplicateTicketId, "");

                                        string ticketIds = "";
                                        renewList.ForEach(p => { ticketIds += (ticketIds != "" ? ", " : "") + p.slm_TicketId; });

                                        string errMessage = string.Format("ContractNo : {0}, GoToPending : มี TicketId มากกว่า 1 รายการ ({1})", contractNo, ticketIds);
                                        Util.WriteLogFile(logfilename, BatchCode, errMessage);
                                        BizUtil.InsertLog(BatchMonitorId, "", "", errMessage);
                                        //Console.WriteLine("ContractNo " + contractNo + ": PENDING");
                                    }
                                }
                                else
                                {
                                    string errMsg = "";
                                    AppConstant.PendingType pendingType;
                                    var leads = (from r in slmdb.kkslm_tr_renewinsurance
                                                 join l in slmdb.kkslm_tr_lead on r.slm_TicketId equals l.slm_ticketId
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
                                        int count = count = slmdb.kkslm_tr_prelead.Where(p => p.slm_Contract_Number == contractNo).Count();
                                        if (count > 0)
                                        {
                                            pendingType = AppConstant.PendingType.ContractNoInPreleadOnly;
                                            errMsg = string.Format("ContractNo : {0}, GoToPending : มี ContractNo ใน Prelead แต่ไม่มีใน Lead", contractNo);
                                        }
                                        else
                                        {
                                            pendingType = AppConstant.PendingType.ContractNoNotFound;
                                            errMsg = string.Format("ContractNo : {0}, GoToPending : ไม่พบ ContractNo ในระบบ", contractNo);
                                        }
                                    }

                                    InsertPolicyActPending(contractInsureList, contractNo, slmdb, pendingType, "");
                                    Util.WriteLogFile(logfilename, BatchCode, errMsg);
                                    BizUtil.InsertLog(BatchMonitorId, "", "", errMsg);
                                    //Console.WriteLine("ContractNo " + contractNo + ": PENDING");
                                }
                            }

                            ts.Complete();
                            totalSuccess += contractInsureList.Count(p => p.ContractNo == contractNo);
                        }
                    }
                    catch (Exception ex)
                    {
                        totalFail += contractInsureList.Count(p => p.ContractNo == contractNo);
                        string message = "ContractNo " + contractNo + ", Error=" + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);

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

            return ret;
        }

        private void InsertPolicyActPending(List<ContractInsuranceData> contractInsureList, string ContractNumber, SLMDBEntities slmdb, AppConstant.PendingType pendingType, string policyType)
        {
            try
            {
                List<ContractInsuranceData> list = new List<ContractInsuranceData>();
                DateTime createdDate = DateTime.Now;

                if (!string.IsNullOrWhiteSpace(policyType))
                {
                    list = contractInsureList.Where(p => p.ContractNo == ContractNumber && p.PolicyType == policyType).ToList();
                    slmdb.kkslm_tr_renewinsurance_hp_policyno_actno_pending.Where(p => p.slm_Contract_Number == ContractNumber && p.slm_Policy_Type == policyType).ToList().ForEach(slmdb.kkslm_tr_renewinsurance_hp_policyno_actno_pending.DeleteObject);
                }
                else
                {
                    list = contractInsureList.Where(p => p.ContractNo == ContractNumber).ToList();
                    slmdb.kkslm_tr_renewinsurance_hp_policyno_actno_pending.Where(p => p.slm_Contract_Number == ContractNumber).ToList().ForEach(slmdb.kkslm_tr_renewinsurance_hp_policyno_actno_pending.DeleteObject);
                }
                
                //Insert new record
                foreach (ContractInsuranceData data in list)
                {
                    kkslm_tr_renewinsurance_hp_policyno_actno_pending pending = new kkslm_tr_renewinsurance_hp_policyno_actno_pending()
                    {
                        slm_Contract_Number = ContractNumber,
                        slm_Policy_Year = data.PolicyYear,
                        slm_Policy_No = data.PolicyNo,
                        slm_Policy_Type = data.PolicyType,
                        slm_Main_By = data.MainBy,
                        slm_Main_Date = data.MainDate,
                        slm_Main_Time = data.MainTime,
                        slm_Create_By = data.CreateBy,
                        slm_Create_Date = data.CreateDate,
                        slm_Create_Time = data.CreateTime,
                        slm_Type = Convert.ToInt32(pendingType),
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createdDate,
                        is_Deleted = false
                    };
                    slmdb.kkslm_tr_renewinsurance_hp_policyno_actno_pending.AddObject(pending);
                }

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoProcess(SLMDBEntities slmdb, kkslm_tr_renewinsurance reins, List<ContractInsuranceData> contractInsureList, List<InsuranceCompanyData> insurComList, List<CoverageTypeData> coverageList)
        {
            try
            {
                string errMessage = "";
                bool doUpdatePolicy = false;
                bool doUpdateAct = false;
                DateTime createdDate = DateTime.Now;
                var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == reins.slm_TicketId).FirstOrDefault();
                if (lead == null) { throw new Exception("ไม่พบข้อมูล Lead TicketId " + reins.slm_TicketId + " ในระบบ"); }

                int countPolicy = slmdb.kkslm_tr_renewinsurance_compare.Count(p => p.slm_RenewInsureId == reins.slm_RenewInsureId && p.slm_Selected == true);
                int countAct = slmdb.kkslm_tr_renewinsurance_compare_act.Count(p => p.slm_RenewInsureId == reins.slm_RenewInsureId && p.slm_ActPurchaseFlag == true);
                ContractInsuranceData data = null;

                if (countPolicy > 0)
                {
                    if (string.IsNullOrWhiteSpace(reins.slm_ReceiveNo))
                    {
                        InsertPolicyActPending(contractInsureList, reins.slm_ContractNo, slmdb, AppConstant.PendingType.ReceiveNoNotFound, "");

                        errMessage = string.Format("ContractNo : {0}, TicketId : {1}, GoToPending, Error : ไม่พบข้อมูลเลขที่รับแจ้ง", reins.slm_ContractNo, reins.slm_TicketId);
                        Util.WriteLogFile(logfilename, BatchCode, errMessage);
                        BizUtil.InsertLog(BatchMonitorId, lead.slm_ticketId, "", errMessage);
                        //Console.WriteLine("ContractNo " + reins.slm_ContractNo + ": PENDING");
                        return;
                    }

                    data = contractInsureList.Where(p => p.PolicyType == "V").FirstOrDefault();
                    if (data != null)
                    {
                        if (string.IsNullOrWhiteSpace(reins.slm_PolicyNo))
                            doUpdatePolicy = true;
                        else
                        {
                            if (reins.slm_PolicyNo.Trim().ToLower() != data.PolicyNo.ToLower())
                                doUpdatePolicy = true;
                        }

                        if (doUpdatePolicy)
                        {
                            reins.slm_PolicyNo = data.PolicyNo;
                            UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "10", "30", AppConstant.LoggingType.EODUpdateCurrent);
                            UpdatePending(slmdb, reins.slm_ContractNo, "V", createdDate);
                        }
                    }

                    data = contractInsureList.Where(p => p.PolicyType == "C").FirstOrDefault();
                    if (data != null)
                    {
                        if (countAct > 0)
                        {
                            if (string.IsNullOrWhiteSpace(reins.slm_ActNo))
                                doUpdateAct = true;
                            else
                            {
                                if (reins.slm_ActNo.Trim().ToLower() != data.PolicyNo.ToLower())
                                    doUpdateAct = true;
                            }

                            if (doUpdateAct)
                            {
                                reins.slm_ActNo = data.PolicyNo;
                                UpdatePending(slmdb, reins.slm_ContractNo, "C", createdDate);
                            } 
                        }
                        else
                        {
                            InsertPolicyActPending(contractInsureList, reins.slm_ContractNo, slmdb, AppConstant.PendingType.PurchaseDetailNotFound, "C");

                            errMessage = string.Format("ContractNo : {0}, TicketId : {1}, GoToPending, Error : ไม่พบข้อมูลการซื้อพรบ. แต่มีเลขพรบ.เข้าระบบ", reins.slm_ContractNo, reins.slm_TicketId);
                            Util.WriteLogFile(logfilename, BatchCode, errMessage);
                            BizUtil.InsertLog(BatchMonitorId, lead.slm_ticketId, "", errMessage);
                            //Console.WriteLine("ContractNo " + reins.slm_ContractNo + ": PENDING");
                        }
                    }
                }
                else if (countPolicy == 0 && countAct > 0)
                {
                    data = contractInsureList.Where(p => p.PolicyType == "C").FirstOrDefault();
                    if (data != null)
                    {
                        if (string.IsNullOrWhiteSpace(reins.slm_ActNo))
                            doUpdateAct = true;
                        else
                        {
                            if (reins.slm_ActNo.Trim().ToLower() != data.PolicyNo.ToLower())
                                doUpdateAct = true;
                        }

                        if (doUpdateAct)
                        {
                            reins.slm_ActNo = data.PolicyNo;
                            UpdateStatusAndOwnerLogging(slmdb, lead, createdDate, "10", "31", AppConstant.LoggingType.EODUpdateCurrent);
                            UpdatePending(slmdb, reins.slm_ContractNo, "C", createdDate);
                        }
                    }

                    data = contractInsureList.Where(p => p.PolicyType == "V").FirstOrDefault();
                    if (data != null)
                    {
                        InsertPolicyActPending(contractInsureList, reins.slm_ContractNo, slmdb, AppConstant.PendingType.PurchaseDetailNotFound, "V");

                        errMessage = string.Format("ContractNo : {0}, TicketId : {1}, GoToPending, Error : ไม่พบข้อมูลการซื้อประกัน แต่มีเลขกรมธรรม์เข้าระบบ", reins.slm_ContractNo, reins.slm_TicketId);
                        Util.WriteLogFile(logfilename, BatchCode, errMessage);
                        BizUtil.InsertLog(BatchMonitorId, lead.slm_ticketId, "", errMessage);
                        //Console.WriteLine("ContractNo " + reins.slm_ContractNo + ": PENDING");
                    }
                }
                else
                {
                    InsertPolicyActPending(contractInsureList, reins.slm_ContractNo, slmdb, AppConstant.PendingType.PurchaseDetailNotFound, "");
                    Util.WriteLogFile(logfilename, BatchCode, string.Format("ContractNo : {0}, Error : ไม่พบข้อมูลการซื้อขายประกันและพรบ.", reins.slm_ContractNo));
                    BizUtil.InsertLog(BatchMonitorId, lead.slm_ticketId, "", string.Format("ContractNo : {0}, Error : ไม่พบข้อมูลการซื้อขายประกันและพรบ.", reins.slm_ContractNo));
                    //Console.WriteLine("ContractNo " + reins.slm_ContractNo + ": PENDING");
                    return;     //ถ้าตก Pending ไม่มีการซื้อประกันและพรบ. ไม่ต้องลงบันทึกผลการติดต่อและไม่ส่ง CAR
                }

                if (doUpdatePolicy || doUpdateAct)
                {
                    reins.slm_UpdatedBy = "SYSTEM";
                    reins.slm_UpdatedDate = createdDate;
                    slmdb.SaveChanges();

                    var carData = AppUtil.GetDataForCARLogService(reins.slm_TicketId, slmdb);
                    var phonecall = InsertPhonecallHistory(slmdb, reins.slm_TicketId, createdDate, carData);

                    if (doUpdatePolicy && AppConstant.SendSMSPolicyNo)
                    {
                        InsertPrepareSMS(slmdb, lead, reins, insurComList, coverageList, createdDate);
                    }
                    
                    CreateCASActivityLog(reins, slmdb, BatchCode, BatchMonitorId, phonecall.slm_PhoneCallId, carData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdatePending(SLMDBEntities slmdb, string contractNo, string policyType, DateTime updatedDate)
        {
            try
            {
                var pending = slmdb.kkslm_tr_renewinsurance_hp_policyno_actno_pending.Where(p => p.slm_Contract_Number == contractNo && p.slm_Policy_Type == policyType).FirstOrDefault();
                if (pending != null)
                {
                    pending.is_Deleted = true;
                    pending.slm_UpdatedDate = updatedDate;
                    pending.slm_UpdatedBy = "SYSTEM";
                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<ContractInsuranceData> ConvretToList(DataTable dtBDW)
        {
            try
            {
                var list = new List<ContractInsuranceData>();
                dtBDW.AsEnumerable().ToList().ForEach(p => {
                    ContractInsuranceData data = new ContractInsuranceData()
                    {
                        ContractNo = p[eBDW.CONTRACT_NUMBER.ToString()].ToString().Trim(),
                        PolicyYear = p[eBDW.POLICY_YEAR.ToString()].ToString().Trim(),
                        PolicyNo = p[eBDW.POLICY_NO.ToString()].ToString().Trim(),
                        PolicyType = p[eBDW.POLICY_TYPE.ToString()].ToString().Trim().ToUpper(),
                        MainBy = p[eBDW.MAIN_BY.ToString()].ToString().Trim(),
                        MainDate = p[eBDW.MAIN_DATE.ToString()].ToString().Trim(),
                        MainTime = p[eBDW.MAIN_TIME.ToString()].ToString().Trim(),
                        CreateBy = p[eBDW.CREATE_BY.ToString()].ToString().Trim(),
                        CreateDate = p[eBDW.CREATE_DATE.ToString()].ToString().Trim(),
                        CreateTime = p[eBDW.CREATE_TIME.ToString()].ToString().Trim(),
                        DataSource = p[eBDW.DATA_SOURCE.ToString()].ToString().Trim()
                    };
                    list.Add(data);
                });

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SnapDataSource(List<ContractInsuranceData> contractInsurList)
        {
            try
            {
                DateTime createdDate = DateTime.Now;

                using (var slmdb = AppUtil.GetSlmDbEntities())
                {
                    foreach (var data in contractInsurList)
                    {
                        var snap = new kkslm_tr_renewinsurance_hp_policyno_actno_snap
                        {
                            slm_Contract_Number = data.ContractNo,
                            slm_Policy_Year = data.PolicyYear,
                            slm_Policy_Type = data.PolicyType,
                            slm_Policy_No = data.PolicyNo,
                            slm_Main_By = data.MainBy,
                            slm_Main_Date = data.MainDate,
                            slm_Main_Time = data.MainTime,
                            slm_Create_By = data.CreateBy,
                            slm_Create_Date = data.CreateDate,
                            slm_Create_Time = data.CreateTime,
                            slm_Source = data.DataSource,
                            slm_CreatedDate = createdDate,
                            slm_CreatedBy = "SYSTEM"
                        };
                        slmdb.kkslm_tr_renewinsurance_hp_policyno_actno_snap.AddObject(snap);
                    }
                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string message = "Snap datasource failed because " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                Util.WriteLogFile(logfilename, BatchCode, message);
                BizUtil.InsertLog(BatchMonitorId, "", "", message);
            }
        }

        private void ValidateList(List<ContractInsuranceData> contractInsureList)
        {
            try
            {
                var removeList = new List<ContractInsuranceData>();
                contractInsureList.ForEach(p => {
                    bool isRemoved = false;
                    if (string.IsNullOrWhiteSpace(p.ContractNo))
                    {
                        isRemoved = true;
                    }

                    if (string.IsNullOrWhiteSpace(p.PolicyType))
                    {
                        isRemoved = true;
                    }
                    else
                    {
                        if (p.PolicyType.ToUpper() != "V" && p.PolicyType.ToUpper() != "C")
                            isRemoved = true;
                    }

                    if (string.IsNullOrWhiteSpace(p.PolicyNo))
                    {
                        isRemoved = true;
                    }

                    if (isRemoved)
                        removeList.Add(p);
                });

                foreach (ContractInsuranceData data in removeList)
                {
                    contractInsureList.Remove(data);

                    Util.WriteLogFile(logfilename, BatchCode, string.Format("ContractNo : {0}, PolicyNo : {1}, PolicyType : {2}, Error : ข้อมูลนำเข้า ไม่สมบูรณ์", data.ContractNo, data.PolicyNo, data.PolicyType));
                    BizUtil.InsertLog(BatchMonitorId, "", "", string.Format("ContractNo : {0}, PolicyNo : {1}, PolicyType : {2}, Error : ข้อมูลนำเข้า ไม่สมบูรณ์", data.ContractNo, data.PolicyNo, data.PolicyType));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable GetPolicyNoFromBDW(Int64 batchMonitorId, string batchCode)
        {
            OracleDB db = null;
            try
            {
                db = new OracleDB(AppConstant.DWHConnectionString);

                //Check ว่าต้นทางมีข้อมูลพร้อมให้ load หรือยัง
                DateTime zDataDate = DateTime.Today.AddDays(-1);
                string zData_Date = zDataDate.Year.ToString() + zDataDate.ToString("MMdd");

                string sqlcheck = @"SELECT ZDATA_DATE, ZPROCESS_ID, ZSTART_DATE, MAX (ZSTART_TIME) AS ZSTART_TIME, ZFINISH_DATE, MAX (ZFINISH_TIME) AS ZFINISH_TIME, ZFINISH_FLG 
                                    FROM " + AppConstant.DWHSchema + @".ZBW_PROC_STAT 
                                    WHERE zprocess_id = 'OBT_IN' AND zfinish_flg = 'Y' AND ZDATA_DATE = '" + zData_Date + @"' 
                                    GROUP BY ZDATA_DATE, ZPROCESS_ID, ZSTART_DATE, ZFINISH_DATE, ZFINISH_FLG ";

                DataTable dtCheck = db.ExcecuteTable(sqlcheck);
                if (dtCheck.Rows.Count == 0)
                {
                    throw new Exception("ข้อมูลบนระบบ Datawarehouse ยังไม่พร้อมใช้งาน");
                }

                int numOfDay = AppConstant.DWHNumOfDay;
                string datefrom = DateTime.Today.AddDays(-numOfDay).Year.ToString() + DateTime.Today.AddDays(-numOfDay).ToString("MMdd");
                string dateto = DateTime.Today.Year.ToString() + DateTime.Today.ToString("MMdd");

                string sql = string.Format("SELECT C.*, 'BDW' AS DATA_SOURCE FROM {0}.OBT_CONTRACT_INSURANCE C WHERE C.POLICY_NO IS NOT NULL AND C.POLICY_TYPE IN ('V','C') AND C.CONTRACT_NUMBER IS NOT NULL AND C.CREATE_DATE BETWEEN '{1}' AND '{2}' ORDER BY C.CONTRACT_NUMBER ASC, C.POLICY_TYPE DESC ", AppConstant.DWHSchema, datefrom, dateto);
                return db.ExcecuteTable(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db != null)
                {
                    db.CloseConnection();
                }
            }
        }

        private List<InsuranceCompanyData> GetInsuranceCompany()
        {
            try
            {
                string sql = @"SELECT slm_Ins_Com_Id AS InsComId, slm_InsCode AS InsComCode, slm_InsNameTh AS InsNameTH, slm_TelContact AS TelContact, slm_InsABB AS InsNameAbb 
                                FROM " + AppConstant.OPERDBName + ".dbo.kkslm_ms_ins_com ";

                using (var slmdb = AppUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<InsuranceCompanyData>(sql).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        private List<CoverageTypeData> GetCoverageTypeList()
        {
            try
            {
                using (var slmdb = AppUtil.GetSlmDbEntities())
                {
                    return slmdb.kkslm_ms_coveragetype.Where(p => p.is_Deleted == false)
                                .Select(p => new CoverageTypeData { CoverageTypeId = p.slm_CoverageTypeId, CoverageTypeName = p.slm_ConverageTypeName }).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public decimal CalculateTotalPaidAmount(SLMDBEntities slmdb, string ticketId, string paymentCode)
        {
            try
            {
                string sql = @"SELECT SUM(detail.slm_RecAmount) AS TotalAmount
                                FROM dbo.kkslm_tr_renewinsurance_receipt receipt
                                INNER JOIN dbo.kkslm_tr_renewinsurance_receipt_detail detail ON receipt.slm_RenewInsuranceReceiptId = detail.slm_RenewInsuranceReceiptId
                                WHERE receipt.slm_ticketId = '" + ticketId + @"' AND detail.slm_PaymentCode = '" + paymentCode + @"' AND detail.is_Deleted = 0 ";

                var amount = slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
                return amount != null ? amount.Value : 0;
            }
            catch
            {
                throw;
            }
        }

        private string GetTelNoSMS(kkslm_tr_lead lead)
        {
            try
            {
                using (var slmdb = AppUtil.GetSlmDbEntities())
                {
                    var telNoSms = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId == lead.slm_ticketId).Select(p => p.slm_TelNoSms).FirstOrDefault();
                    return string.IsNullOrWhiteSpace(telNoSms) ? lead.slm_TelNo_1 : telNoSms;
                }
            }
            catch
            {
                throw;
            }
        }

        private void InsertPrepareSMS(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_renewinsurance renew, List<InsuranceCompanyData> insurComList, List<CoverageTypeData> coverageList, DateTime createdDate)
        {
            try
            {
                string filePath = AppConstant.SMSTemplatePathPolicyNo;

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    throw new Exception("ไม่พบ Config SMSTemplatePathPolicyNo ใน Configuration File");
                }

                var paidAmount = CalculateTotalPaidAmount(slmdb, renew.slm_TicketId, AppConstant.PaymentCode.Policy);
                var coverageTypeName = coverageList.Where(p => p.CoverageTypeId == renew.slm_CoverageTypeId).Select(p => p.CoverageTypeName).FirstOrDefault();
                var insurTelNo = insurComList.Where(p => p.InsComId == renew.slm_InsuranceComId).Select(p => p.TelContact).FirstOrDefault();
                var insNameAbb = insurComList.Where(p => p.InsComId == renew.slm_InsuranceComId).Select(p => p.InsNameAbb).FirstOrDefault();
                string policyStartCoverDate = "";
                if (renew.slm_PolicyStartCoverDate != null)
                {
                    policyStartCoverDate = renew.slm_PolicyStartCoverDate.Value.ToString("dd/MM/") + (renew.slm_PolicyStartCoverDate.Value.Year + 543).ToString().Substring(2, 2);
                }

                string template = File.ReadAllText(filePath, Encoding.UTF8);
                template = template.Replace("%InsNameAbb%", string.IsNullOrWhiteSpace(insNameAbb) ? "" : insNameAbb)
                                    .Replace("%CoverageType%", string.IsNullOrWhiteSpace(coverageTypeName) ? "" : coverageTypeName)
                                    .Replace("%PolicyPremiumPaid%", paidAmount.ToString("#,##0.00"))
                                    .Replace("%PolicyNo%", renew.slm_PolicyNo)
                                    .Replace("%PolicyStartCoverDate%", policyStartCoverDate)
                                    //.Replace("%CarLicenseNo%", renew.slm_LicenseNo)
                                    .Replace("%InsuranceTelNo%", string.IsNullOrWhiteSpace(insurTelNo) ? "" : insurTelNo);

                kkslm_tr_prelead_prepare_sms sms = new kkslm_tr_prelead_prepare_sms()
                {
                    slm_ticketId = renew.slm_TicketId,
                    slm_Message = template,
                    slm_Message_CreatedBy = "SYSTEM",
                    slm_MessageStatusId = "1",
                    slm_PhoneNumber = GetTelNoSMS(lead),
                    slm_QueueId = "6",
                    slm_RequestDate = createdDate,
                    slm_RuleActivityId = 0,
                    slm_ExportStatus = "0",
                    slm_RefId = null,
                    slm_SendingStatusCode = null,
                    slm_SendingStatus = null,
                    slm_ErrorCode = null,
                    slm_ErrorReason = null,
                    slm_CAS_Flag = null,
                    slm_CAS_Date = null,
                    slm_FlagType = "2",
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedDate = createdDate,
                    slm_UpdatedBy = "SYSTEM",
                    slm_UpdatedDate = createdDate,
                    is_Deleted = 0
                };
                slmdb.kkslm_tr_prelead_prepare_sms.AddObject(sms);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        private kkslm_phone_call InsertPhonecallHistory(SLMDBEntities slmdb, string ticketId, DateTime createdDate, LeadDataForCARLogService data)
        {
            try
            {
                string content = "";
                string policyStartCoverDate = "";
                string policyEndCoverDate = "";
                string actStartCoverDate = "";
                string actEndCoverDate = "";

                if (data.PolicyStartCoverDate != null)
                    policyStartCoverDate = data.PolicyStartCoverDate.Value.ToString("dd-MM-") + data.PolicyStartCoverDate.Value.Year.ToString();
                if (data.PolicyEndCoverDate != null)
                    policyEndCoverDate = data.PolicyEndCoverDate.Value.ToString("dd-MM-") + data.PolicyEndCoverDate.Value.Year.ToString();
                if (data.ActStartCoverDate != null)
                    actStartCoverDate = data.ActStartCoverDate.Value.ToString("dd-MM-") + data.ActStartCoverDate.Value.Year.ToString();
                if (data.ActEndCoverDate != null)
                    actEndCoverDate = data.ActEndCoverDate.Value.ToString("dd-MM-") + data.ActEndCoverDate.Value.Year.ToString();

                content = "เลขที่สัญญา:" + (data != null ? data.ContractNo : "");
                content += ", บริษัทประกันภัยรถยนต์:" + (data != null ? data.InsuranceCompany : "");
                content += ", เลขเล่มกรมธรรม์:" + (data != null ? data.PolicyNo : "");
                content += ", เลขลงทะเบียน:";
                content += ", วันที่คุ้มครอง:" + policyStartCoverDate;
                content += ", วันที่สิ้นสุดกรมธรรม์:" + policyEndCoverDate;
                content += ", เลขที่พรบ.:" + (data != null ? data.ActNo : "");
                content += ", วันที่เริ่มต้นพรบ.:" + actStartCoverDate;
                content += ", วันที่สิ้นสุดพรบ.:" + actEndCoverDate;

                var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                if (lead != null)
                {
                    kkslm_phone_call phone = new kkslm_phone_call()
                    {
                        slm_TicketId = ticketId,
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

                    return phone;
                }
                else
                {
                    throw new Exception("ไม่พบ TicketId " + ticketId + " ในระบบ");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private void CreateCASActivityLog(kkslm_tr_renewinsurance reins, SLMDBEntities slmdb, string batchCode, Int64 batchMonitorId, int phonecall_id, LeadDataForCARLogService data)
        {
            try
            {
                string policyStartCoverDate = "";
                string policyEndCoverDate = "";
                string actStartCoverDate = "";
                string actEndCoverDate = "";
                string preleadId = "";

                if (data != null)
                {
                    preleadId = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
                    if (data.PolicyStartCoverDate != null)
                        policyStartCoverDate = data.PolicyStartCoverDate.Value.ToString("dd-MM-") + data.PolicyStartCoverDate.Value.Year.ToString();
                    if (data.PolicyEndCoverDate != null)
                        policyEndCoverDate = data.PolicyEndCoverDate.Value.ToString("dd-MM-") + data.PolicyEndCoverDate.Value.Year.ToString();
                    if (data.ActStartCoverDate != null)
                        actStartCoverDate = data.ActStartCoverDate.Value.ToString("dd-MM-") + data.ActStartCoverDate.Value.Year.ToString();
                    if (data.ActEndCoverDate != null)
                        actEndCoverDate = data.ActEndCoverDate.Value.ToString("dd-MM-") + data.ActEndCoverDate.Value.Year.ToString();
                }

                //Activity Info
                List<CARService.DataItem> actInfoList = new List<CARService.DataItem>();
                actInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "บริษัทประกันภัยรถยนต์", DataValue = data != null ? data.InsuranceCompany : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "เลขเล่มกรมธรรม์", DataValue = data != null ? data.PolicyNo : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 4, DataLabel = "เลขลงทะเบียน", DataValue = "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 5, DataLabel = "วันที่คุ้มครอง", DataValue = policyStartCoverDate });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 6, DataLabel = "วันที่สิ้นสุดกรมธรรม์", DataValue = policyEndCoverDate });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 7, DataLabel = "เลขที่พรบ.", DataValue = data != null ? data.ActNo : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 8, DataLabel = "วันที่เริ่มต้นพรบ.", DataValue = actStartCoverDate });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 9, DataLabel = "วันที่สิ้นสุดพรบ.", DataValue = actEndCoverDate });

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

                bool ret = CARService.CreateActivityLog(logdata, batchMonitorId, batchCode, reins.slm_TicketId, logfilename);
                AppUtil.UpdatePhonecallCASFlag(slmdb, phonecall_id, ret ? "1" : "2");
            }
            catch (Exception ex)
            {
                AppUtil.UpdatePhonecallCASFlag(slmdb, phonecall_id, "2");

                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BizUtil.InsertLog(batchMonitorId, reins.slm_TicketId, "", message);
                Util.WriteLogFile(logfilename, batchCode, message);
            }
        }
    }

    public class ContractInsuranceData
    {
        public string ContractNo { get; set; }
        public string PolicyYear { get; set; }
        public string PolicyNo { get; set; }
        public string PolicyType { get; set; }
        public string MainBy { get; set; }
        public string MainDate { get; set; }
        public string MainTime { get; set; }
        public string CreateBy { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string DataSource { get; set; }
    }
}
