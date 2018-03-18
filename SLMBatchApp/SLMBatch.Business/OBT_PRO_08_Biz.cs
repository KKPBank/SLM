using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SLMBatch.Business
{
    public class OBT_PRO_08_Biz : ServiceBase
    {
        #region Backup 05-06-2016 Classic Process
        //        public void UpdateSaleTransaction(string batchCode)
        //        {
        //            Int64 batchMonitorId = 0;
        //            int totalRecord = 0;
        //            int totalSuccess = 0;
        //            int totalFail = 0;

        //            SQLServerDB hpdb = null;
        //            SQLServerDB slmdb = null;
        //            string sql = "";
        //            string year = "";
        //            string month = "";
        //            string processState = "";
        //            string policyStartDate = "";
        //            string policyEndDate = "";
        //            string actStartDate = "";
        //            string actEndDate = "";
        //            string receiveDate = "";
        //            List<TempDataPro08> tempList = new List<TempDataPro08>();

        //            try
        //            {
        //                batchMonitorId = BizUtil.SetStartTime(batchCode);
        //                BizUtil.CheckPrerequisite(batchCode);

        //                List<SaleRenewInsuranceActData> list = GetSaleRenewInsuranceData();
        //                totalRecord = list.Count;

        //                if (list.Count > 0)
        //                {
        //                    hpdb = new SQLServerDB(AppConstant.HPConnectionString);
        //                    slmdb = new SQLServerDB(AppConstant.SLMDBConnectionString);

        //                    year = (DateTime.Now.Year + 543).ToString();
        //                    month = DateTime.Now.Month.ToString("00");

        //                    foreach (SaleRenewInsuranceActData data in list)
        //                    {
        //                        try
        //                        {
        //                            //Insert to HP
        //                            processState = "HP";

        //                            if (data.HVInfDate != null)
        //                                receiveDate = data.HVInfDate.Value.ToString("dd/MM/") + data.HVInfDate.Value.Year.ToString();
        //                            if (data.PolicyStartDate != null)
        //                                policyStartDate = data.PolicyStartDate.Value.ToString("dd/MM/") + data.PolicyStartDate.Value.Year.ToString();
        //                            if (data.PolicyEndDate != null)
        //                                policyEndDate = data.PolicyEndDate.Value.ToString("dd/MM/") + data.PolicyEndDate.Value.Year.ToString();
        //                            if (data.ActStartDate != null)
        //                                actStartDate = data.ActStartDate.Value.ToString("dd/MM/") + data.ActStartDate.Value.Year.ToString();
        //                            if (data.ActEndDate != null)
        //                                actEndDate = data.ActEndDate.Value.ToString("dd/MM/") + data.ActEndDate.Value.Year.ToString();

        //                            string HV_DISC_TOTAL = (data.PolicyDiscountAmt != null ? data.PolicyDiscountAmt.Value.ToString("0.00") : "0");
        //                            string HV_GROSS_PREM = (data.PolicyGrossPremium != null ? data.PolicyGrossPremium.Value.ToString("0.00") : "0");
        //                            string HV_VOL_TYPE = (data.CoverageTypeId != null ? data.CoverageTypeId.Value.ToString() : "");
        //                            string HV_COV_AMT = (data.PolicyCost != null ? data.PolicyCost.Value.ToString("0.00") : "0");
        //                            string HC_GROSS_PREM = (data.ActGrossPremium != null ? data.ActGrossPremium.Value.ToString("0.00") : "0");
        //                            string HC_NET_PREM = (data.ActNetPremium != null ? data.ActNetPremium.Value.ToString("0.00") : "0");
        //                            string HC_VAT_AMT = (data.ActVat != null ? data.ActVat.Value.ToString("0.00") : "0");
        //                            string HC_STAMP = (data.ActStamp != null ? data.ActStamp.Value.ToString("0.00") : "0");
        //                            string HC_FILLER10 = (data.HCFiller10 != null ? data.HCFiller10.Value.ToString("0.00") : "0");

        //                            //เก็บไว้ในกรณีมี row fail ให้นำค่าใน list ไปลบออกจากเบส HP
        //                            tempList.Add(new TempDataPro08()
        //                            {
        //                                YEAR = year,
        //                                MONTH = month,
        //                                CNT_NO = data.ContractNo,
        //                                HV_INF_NO = data.ReceiveNo,
        //                                HV_INF_DATE = receiveDate,
        //                                HV_FIN_CODE = data.EmpCode,
        //                                HV_INS_CODE = data.InsuranceComCode,
        //                                HV_EFF_DATE = policyStartDate,
        //                                HV_EXP_DATE = policyEndDate,
        //                                HV_DISC_TOTAL = HV_DISC_TOTAL,
        //                                HV_GROSS_PREM = HV_GROSS_PREM,
        //                                HV_VOL_TYPE = HV_VOL_TYPE,
        //                                HV_COV_AMT = HV_COV_AMT,
        //                                HV_FIXED_CLASS = data.RepairType,
        //                                HC_GROSS_PREM = HC_GROSS_PREM,
        //                                HC_INS_CODE = data.ActComCode,
        //                                HC_EFF_DATE = actStartDate,
        //                                HC_EXP_DATE = actEndDate,
        //                                HC_NET_PREM = HC_NET_PREM,
        //                                HC_VAT_AMT = HC_VAT_AMT,
        //                                HC_STAMP = HC_STAMP,
        //                                HC_FILLER10 = HC_FILLER10,
        //                                TicketId = data.TicketId
        //                            });

        //                            sql = @"INSERT INTO HP_MKT_Upload_Detail_OBT(
        //                                [YEAR]
        //                                , [MONTH]
        //                                , CNT_NO
        //                                , HV_INF_NO
        //                                , HV_INF_DATE
        //                                , HV_FIN_CODE
        //                                , HV_INS_CODE
        //                                , HV_EFF_DATE
        //                                , HV_EXP_DATE
        //                                , HV_DISC_TOTAL
        //                                , HV_GROSS_PREM
        //                                , HV_VOL_TYPE
        //                                , HV_COV_AMT
        //                                , HV_FIXED_CLASS
        //                                , HC_GROSS_PREM
        //                                , HC_INS_CODE
        //                                , HC_EFF_DATE
        //                                , HC_EXP_DATE
        //                                , HC_NET_PREM
        //                                , HC_VAT_AMT
        //                                , HC_STAMP
        //                                , HC_FILLER10)
        //                                VALUES('" + year + @"'
        //                                , '" + month + @"'
        //                                , '" + data.ContractNo + @"'
        //                                , '" + data.ReceiveNo + @"'
        //                                , '" + receiveDate + @"'
        //                                , '" + data.EmpCode + @"'
        //                                , '" + data.InsuranceComCode + @"'
        //                                , '" + policyStartDate + @"'
        //                                , '" + policyEndDate + @"'
        //                                , '" + HV_DISC_TOTAL + @"'
        //                                , '" + HV_GROSS_PREM + @"'
        //                                , '" + HV_VOL_TYPE + @"'
        //                                , '" + HV_COV_AMT + @"'
        //                                , '" + data.RepairType + @"'
        //                                , '" + HC_GROSS_PREM + @"'
        //                                , '" + data.ActComCode + @"'
        //                                , '" + actStartDate + @"'
        //                                , '" + actEndDate + @"'
        //                                , '" + HC_NET_PREM + @"'
        //                                , '" + HC_VAT_AMT + @"'
        //                                , '" + HC_STAMP + @"'
        //                                , '" + HC_FILLER10 + "')";

        //                            hpdb.ExceuteNonQuery(sql);

        //                            //Update Flag at SLM
        //                            processState = "SLM";

        //                            sql = @"UPDATE " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance
        //                                SET slm_ObtPro08Flag = 1
        //                                WHERE slm_TicketId = '" + data.TicketId + "'";

        //                            slmdb.ExceuteNonQuery(sql);

        //                            totalSuccess += 1;
        //                            Console.WriteLine("TicketId " + data.TicketId + ": SUCCESS");
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            totalFail += 1;
        //                            string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                            string errorDetail = "";

        //                            if (processState == "HP")
        //                                errorDetail = "Cannot Update Sale Insurance and Act to HP, TicketId=" + data.TicketId + ", Error=" + message;
        //                            else if (processState == "SLM")
        //                                errorDetail = "Cannot Update slm_ObtPro08Flag=true (kkslm_tr_renewinsurance) to SLM, TicketId=" + data.TicketId + ", Error=" + message;
        //                            else
        //                                errorDetail = "TicketId=" + data.TicketId + ", Error=" + message;

        //                            BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
        //                            Util.WriteLogFile(logfilename, batchCode, errorDetail);

        //                            Console.WriteLine("TicketId " + data.TicketId + ": FAIL");
        //                        }
        //                    }
        //                }

        //                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine("All FAIL");
        //                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

        //                Util.WriteLogFile(logfilename, batchCode, message);
        //                BizUtil.InsertLog(batchMonitorId, "", "", message);
        //                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
        //                throw ex;
        //            }
        //            finally
        //            {
        //                if (hpdb != null)
        //                    hpdb.CloseConnection();

        //                if (slmdb != null)
        //                    slmdb.CloseConnection();
        //            }
        //        }
        #endregion

        #region Backup 17-03-2017 Success All, Fail All

        //public void UpdateSaleTransaction(string batchCode)
        //{
        //    Int64 batchMonitorId = 0;
        //    int totalRecord = 0;
        //    int totalSuccess = 0;
        //    int totalFail = 0;

        //    SQLServerDB hpdb = null;
        //    SQLServerDB slmdb = null;
        //    string sql = "";
        //    string year = "";
        //    string month = "";
        //    string processState = "";
        //    //string policyStartDate = "";
        //    //string policyEndDate = "";
        //    //string actStartDate = "";
        //    //string actEndDate = "";
        //    string receiveDate = "";
        //    bool successFlag = true;
        //    List<TempDataPro08> tempList = new List<TempDataPro08>();

        //    try
        //    {
        //        batchMonitorId = BizUtil.SetStartTime(batchCode);
        //        BizUtil.CheckPrerequisite(batchCode);

        //        List<SaleRenewInsuranceActData> list = GetSaleRenewInsuranceData();
        //        totalRecord = list.Count;

        //        if (list.Count > 0)
        //        {
        //            hpdb = new SQLServerDB(AppConstant.HPConnectionString);
        //            slmdb = new SQLServerDB(SLMDBConnectionString);

        //            year = (DateTime.Now.Year + 543).ToString();
        //            month = DateTime.Now.Month.ToString("00");

        //            foreach (SaleRenewInsuranceActData data in list)
        //            {
        //                try
        //                {
        //                    //Insert to HP
        //                    processState = "HP";

        //                    if (data.HVInfDate != null)
        //                        receiveDate = data.HVInfDate.Value.ToString("dd/MM/") + data.HVInfDate.Value.Year.ToString();
        //                    //if (data.PolicyStartDate != null)
        //                    //    policyStartDate = data.PolicyStartDate.Value.ToString("dd/MM/") + data.PolicyStartDate.Value.Year.ToString();
        //                    //if (data.PolicyEndDate != null)
        //                    //    policyEndDate = data.PolicyEndDate.Value.ToString("dd/MM/") + data.PolicyEndDate.Value.Year.ToString();
        //                    //if (data.ActStartDate != null)
        //                    //    actStartDate = data.ActStartDate.Value.ToString("dd/MM/") + data.ActStartDate.Value.Year.ToString();
        //                    //if (data.ActEndDate != null)
        //                    //    actEndDate = data.ActEndDate.Value.ToString("dd/MM/") + data.ActEndDate.Value.Year.ToString();

        //                    string HV_DISC_TOTAL = (data.PolicyDiscountAmt != null ? data.PolicyDiscountAmt.Value.ToString("0.00") : "0");
        //                    string HV_GROSS_PREM = (data.PolicyGrossPremium != null ? data.PolicyGrossPremium.Value.ToString("0.00") : "0");
        //                    string HV_VOL_TYPE = (data.CoverageTypeId != null ? data.CoverageTypeId.Value.ToString() : "");
        //                    string HV_COV_AMT = (data.PolicyCost != null ? data.PolicyCost.Value.ToString("0.00") : "0");
        //                    string HC_GROSS_PREM = (data.ActGrossPremium != null ? data.ActGrossPremium.Value.ToString("0.00") : "0");
        //                    string HC_NET_PREM = (data.ActNetPremium != null ? data.ActNetPremium.Value.ToString("0.00") : "0");
        //                    string HC_VAT_AMT = (data.ActVat != null ? data.ActVat.Value.ToString("0.00") : "0");
        //                    string HC_STAMP = (data.ActStamp != null ? data.ActStamp.Value.ToString("0.00") : "0");
        //                    string HC_FILLER10 = (data.HCFiller10 != null ? data.HCFiller10.Value.ToString("0.00") : "0");

        //                    //เก็บไว้ในกรณีมี row fail ให้นำค่าใน list ไปลบออกจากเบส HP
        //                    tempList.Add(new TempDataPro08()
        //                    {
        //                        YEAR = year,
        //                        MONTH = month,
        //                        CNT_NO = data.ContractNo,
        //                        HV_INF_NO = data.ReceiveNo,
        //                        HV_INF_DATE = receiveDate,
        //                        HV_FIN_CODE = data.EmpCode,
        //                        HV_INS_CODE = data.InsuranceComCode,
        //                        HV_EFF_DATE = data.PolicyStartDate,
        //                        HV_EXP_DATE = data.PolicyEndDate,
        //                        HV_DISC_TOTAL = HV_DISC_TOTAL,
        //                        HV_GROSS_PREM = HV_GROSS_PREM,
        //                        HV_VOL_TYPE = HV_VOL_TYPE,
        //                        HV_COV_AMT = HV_COV_AMT,
        //                        HV_FIXED_CLASS = data.RepairType,
        //                        HC_GROSS_PREM = HC_GROSS_PREM,
        //                        HC_INS_CODE = data.ActComCode,
        //                        HC_EFF_DATE = data.ActStartDate,
        //                        HC_EXP_DATE = data.ActEndDate,
        //                        HC_NET_PREM = HC_NET_PREM,
        //                        HC_VAT_AMT = HC_VAT_AMT,
        //                        HC_STAMP = HC_STAMP,
        //                        HC_FILLER10 = HC_FILLER10,
        //                        TicketId = data.TicketId
        //                    });

        //                    sql = @"INSERT INTO HP_MKT_Upload_Detail_OBT(
        //                        [YEAR]
        //                        , [MONTH]
        //                        , CNT_NO
        //                        , HV_INF_NO
        //                        , HV_INF_DATE
        //                        , HV_FIN_CODE
        //                        , HV_INS_CODE
        //                        , HV_EFF_DATE
        //                        , HV_EXP_DATE
        //                        , HV_DISC_TOTAL
        //                        , HV_GROSS_PREM
        //                        , HV_VOL_TYPE
        //                        , HV_COV_AMT
        //                        , HV_FIXED_CLASS
        //                        , HC_GROSS_PREM
        //                        , HC_INS_CODE
        //                        , HC_EFF_DATE
        //                        , HC_EXP_DATE
        //                        , HC_NET_PREM
        //                        , HC_VAT_AMT
        //                        , HC_STAMP
        //                        , HC_FILLER10)
        //                        VALUES('" + year + @"'
        //                        , '" + month + @"'
        //                        , '" + data.ContractNo + @"'
        //                        , '" + data.ReceiveNo + @"'
        //                        , '" + receiveDate + @"'
        //                        , '" + data.EmpCode + @"'
        //                        , '" + data.InsuranceComCode + @"'
        //                        , '" + data.PolicyStartDate + @"'
        //                        , '" + data.PolicyEndDate + @"'
        //                        , '" + HV_DISC_TOTAL + @"'
        //                        , '" + HV_GROSS_PREM + @"'
        //                        , '" + HV_VOL_TYPE + @"'
        //                        , '" + HV_COV_AMT + @"'
        //                        , '" + data.RepairType + @"'
        //                        , '" + HC_GROSS_PREM + @"'
        //                        , '" + data.ActComCode + @"'
        //                        , '" + data.ActStartDate + @"'
        //                        , '" + data.ActEndDate + @"'
        //                        , '" + HC_NET_PREM + @"'
        //                        , '" + HC_VAT_AMT + @"'
        //                        , '" + HC_STAMP + @"'
        //                        , '" + HC_FILLER10 + "')";

        //                    hpdb.ExceuteNonQuery(sql);

        //                    //Update Flag at SLM
        //                    processState = "SLM";

        //                    sql = @"UPDATE " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance
        //                            SET slm_ObtPro08Flag = 1, slm_ObtPro08FlagDate = GETDATE()
        //                            WHERE slm_TicketId = '" + data.TicketId + "'";

        //                    slmdb.ExceuteNonQuery(sql);

        //                    totalSuccess += 1;
        //                    Console.WriteLine("TicketId " + data.TicketId + ": SUCCESS");
        //                }
        //                catch (Exception ex)
        //                {
        //                    successFlag = false;
        //                    RollbackHP(hpdb, batchMonitorId, batchCode, tempList);
        //                    RollbackRenewInsurance(slmdb, batchMonitorId, batchCode, tempList);

        //                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                    string errorDetail = "";

        //                    if (processState == "HP")
        //                        errorDetail = "Cannot Update Sale Insurance and Act to HP, TicketId=" + data.TicketId + ", Error=" + message;
        //                    else if (processState == "SLM")
        //                        errorDetail = "Cannot Update slm_ObtPro08Flag=true (kkslm_tr_renewinsurance) to SLM, TicketId=" + data.TicketId + ", Error=" + message;
        //                    else
        //                        errorDetail = "TicketId=" + data.TicketId + ", Error=" + message;

        //                    BizUtil.InsertLog(batchMonitorId, data.TicketId, "", errorDetail);
        //                    Util.WriteLogFile(logfilename, batchCode, errorDetail);

        //                    successFlag = false;
        //                    totalSuccess = 0;
        //                    totalFail = totalRecord;
        //                    break;
        //                    //Console.WriteLine("TicketId " + data.TicketId + ": FAIL");
        //                }
        //            }
        //        }

        //        BizUtil.SetEndTime(batchCode, batchMonitorId, (successFlag ? AppConstant.Success : AppConstant.Fail), totalRecord, totalSuccess, totalFail);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("All FAIL");
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

        //        Util.WriteLogFile(logfilename, batchCode, message);
        //        BizUtil.InsertLog(batchMonitorId, "", "", message);
        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
        //    }
        //    finally
        //    {
        //        if (hpdb != null)
        //            hpdb.CloseConnection();

        //        if (slmdb != null)
        //            slmdb.CloseConnection();
        //    }
        //}

        #endregion

        /// <summary>
        /// Update ข้อมูลการขายประกันภัยและพรบ.โดยกรองสถานะที่ขายได้ในวันนั้นๆ(เฉพาะข้อมูลที่ออกเลขรับแจ้งแล้ว ในวันนั้นๆ) จาก OBT ไปที่ HP
        /// </summary>
        /// <param name="batchCode"></param>
        public void UpdateSaleTransaction(string batchCode)
        {
            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;

            SQLServerDB hpdb = null;
            SQLServerDB slmdb = null;
            string sql = "";
            string year = "";
            string month = "";
            string processState = "";
            string receiveDate = "";
            List<TempDataPro08> tempList = new List<TempDataPro08>();
            List<kkslm_tr_renewinsurance> contractList = new List<kkslm_tr_renewinsurance>();

            try
            {
                batchMonitorId = BizUtil.SetStartTime(batchCode);
                BizUtil.CheckPrerequisite(batchCode);

                List<SaleRenewInsuranceActData> list = GetSaleRenewInsuranceData();
                totalRecord = list.Count;

                if (list.Count > 0)
                {
                    hpdb = new SQLServerDB(AppConstant.HPConnectionString);
                    slmdb = new SQLServerDB(SLMDBConnectionString);

                    year = (DateTime.Now.Year + 543).ToString();
                    month = DateTime.Now.Month.ToString("00");

                    string[] excludeStatus = new string[] { "08", "09", "10" };
                    foreach (SaleRenewInsuranceActData data in list)
                    {
                        try
                        {
                            processState = "Prepare Data";
                            if (data.HVInfDate != null)
                            {
                                receiveDate = data.HVInfDate.Value.ToString("dd/MM/") + data.HVInfDate.Value.Year.ToString();
                            }

                            string HV_DISC_TOTAL = (data.PolicyDiscountAmt != null ? data.PolicyDiscountAmt.Value.ToString("0.00") : "0");
                            string HV_GROSS_PREM = (data.PolicyGrossPremium != null ? data.PolicyGrossPremium.Value.ToString("0.00") : "0");
                            string HV_VOL_TYPE = (data.CoverageTypeId != null ? data.CoverageTypeId.Value.ToString() : "");
                            string HV_COV_AMT = (data.PolicyCost != null ? data.PolicyCost.Value.ToString("0.00") : "0");
                            string HC_GROSS_PREM = (data.ActGrossPremium != null ? data.ActGrossPremium.Value.ToString("0.00") : "0");
                            string HC_NET_PREM = (data.ActNetPremium != null ? data.ActNetPremium.Value.ToString("0.00") : "0");
                            string HC_VAT_AMT = (data.ActVat != null ? data.ActVat.Value.ToString("0.00") : "0");
                            string HC_STAMP = (data.ActStamp != null ? data.ActStamp.Value.ToString("0.00") : "0");
                            string HC_FILLER10 = (data.HCFiller10 != null ? data.HCFiller10.Value.ToString("0.00") : "0");

                            processState = "Check Duplicate ContractNo";
                            using (var db = AppUtil.GetSlmDbEntities())
                            {
                                contractList = (from re in db.kkslm_tr_renewinsurance
                                                join lead in db.kkslm_tr_lead on re.slm_TicketId equals lead.slm_ticketId
                                                where re.slm_ContractNo == data.ContractNo && lead.is_Deleted == 0 && excludeStatus.Contains(lead.slm_Status) == false
                                                select re).ToList();
                            }

                            if (contractList.Count == 1)
                            {
                                //Insert to HP
                                processState = "HP";

                                sql = @"INSERT INTO HP_MKT_Upload_Detail_OBT(
                                [YEAR]
                                , [MONTH]
                                , CNT_NO
                                , HV_INF_NO
                                , HV_INF_DATE
                                , HV_FIN_CODE
                                , HV_INS_CODE
                                , HV_EFF_DATE
                                , HV_EXP_DATE
                                , HV_DISC_TOTAL
                                , HV_GROSS_PREM
                                , HV_VOL_TYPE
                                , HV_COV_AMT
                                , HV_FIXED_CLASS
                                , HC_GROSS_PREM
                                , HC_INS_CODE
                                , HC_EFF_DATE
                                , HC_EXP_DATE
                                , HC_NET_PREM
                                , HC_VAT_AMT
                                , HC_STAMP
                                , HC_FILLER10)
                                VALUES('" + year + @"'
                                , '" + month + @"'
                                , '" + data.ContractNo + @"'
                                , '" + data.ReceiveNo + @"'
                                , '" + receiveDate + @"'
                                , '" + data.EmpCode + @"'
                                , '" + data.InsuranceComCode + @"'
                                , '" + data.PolicyStartDate + @"'
                                , '" + data.PolicyEndDate + @"'
                                , '" + HV_DISC_TOTAL + @"'
                                , '" + HV_GROSS_PREM + @"'
                                , '" + HV_VOL_TYPE + @"'
                                , '" + HV_COV_AMT + @"'
                                , '" + data.RepairType + @"'
                                , '" + HC_GROSS_PREM + @"'
                                , '" + data.ActComCode + @"'
                                , '" + data.ActStartDate + @"'
                                , '" + data.ActEndDate + @"'
                                , '" + HC_NET_PREM + @"'
                                , '" + HC_VAT_AMT + @"'
                                , '" + HC_STAMP + @"'
                                , '" + HC_FILLER10 + "')";

                                hpdb.ExecuteNonQuery(sql);

                                //Update Flag at SLM
                                processState = "SLM";

                                sql = @"UPDATE " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance
                                    SET slm_ObtPro08Flag = 1, slm_ObtPro08FlagDate = GETDATE()
                                    WHERE slm_TicketId = '" + data.TicketId + "'";

                                slmdb.ExecuteNonQuery(sql);
                            }
                            else if (contractList.Count > 1)
                            {
                                processState = "EXCEL";
                                tempList.Add(new TempDataPro08()
                                {
                                    YEAR = year,
                                    MONTH = month,
                                    CNT_NO = data.ContractNo,
                                    HV_INF_NO = data.ReceiveNo,
                                    HV_INF_DATE = receiveDate,
                                    HV_FIN_CODE = data.EmpCode,
                                    HV_INS_CODE = data.InsuranceComCode,
                                    HV_EFF_DATE = data.PolicyStartDate,
                                    HV_EXP_DATE = data.PolicyEndDate,
                                    HV_DISC_TOTAL = HV_DISC_TOTAL,
                                    HV_GROSS_PREM = HV_GROSS_PREM,
                                    HV_VOL_TYPE = HV_VOL_TYPE,
                                    HV_COV_AMT = HV_COV_AMT,
                                    HV_FIXED_CLASS = data.RepairType,
                                    HC_GROSS_PREM = HC_GROSS_PREM,
                                    HC_INS_CODE = data.ActComCode,
                                    HC_EFF_DATE = data.ActStartDate,
                                    HC_EXP_DATE = data.ActEndDate,
                                    HC_NET_PREM = HC_NET_PREM,
                                    HC_VAT_AMT = HC_VAT_AMT,
                                    HC_STAMP = HC_STAMP,
                                    HC_FILLER10 = HC_FILLER10,
                                    TicketId = data.TicketId
                                });

                                string ticketIds = "";
                                contractList.ForEach(p => { ticketIds += (ticketIds != "" ? ", " : "") + p.slm_TicketId; });

                                string infoMessage = string.Format("ContractNo : {0}, GoTo Excel, มี TicketId มากกว่า 1 รายการ ({1})", data.ContractNo, ticketIds);
                                Util.WriteLogFile(logfilename, batchCode, infoMessage);
                                BizUtil.InsertLog(batchMonitorId, "", "", infoMessage);
                            }
                            else
                            {
                                string infoMessage = string.Format("ContractNo : {0}, ไม่มีในระบบ SLM", data.ContractNo);
                                Util.WriteLogFile(logfilename, batchCode, infoMessage);
                                BizUtil.InsertLog(batchMonitorId, "", "", infoMessage);
                            }

                            totalSuccess += 1;
                            Console.WriteLine("TicketId " + data.TicketId + ": SUCCESS");
                        }
                        catch (Exception ex)
                        {
                            totalFail += 1;

                            string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            string errorDetail = "";

                            if (processState == "HP")
                                errorDetail = "Cannot Update Sale Insurance and Act to HP, ContractNo=" + data.ContractNo + ", TicketId=" + data.TicketId + ", Error=" + message;
                            else if (processState == "SLM")
                                errorDetail = "Cannot Update slm_ObtPro08Flag=true (kkslm_tr_renewinsurance) to SLM, ContractNo=" + data.ContractNo + ", TicketId=" + data.TicketId + ", Error=" + message;
                            else
                                errorDetail = "ContractNo=" + data.ContractNo + ", TicketId=" + data.TicketId + ", Error=" + message;

                            BizUtil.InsertLog(batchMonitorId, data.TicketId, "", errorDetail);
                            Util.WriteLogFile(logfilename, batchCode, errorDetail);
                            Console.WriteLine("TicketId " + data.TicketId + ": FAIL");
                        }
                    }
                    //end foreach

                    if (tempList.Count > 0)
                    {
                        CreateExcel(batchMonitorId, batchCode, tempList);
                    }
                }

                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Util.WriteLogFile(logfilename, batchCode, message);
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
            finally
            {
                if (hpdb != null)
                    hpdb.CloseConnection();

                if (slmdb != null)
                    slmdb.CloseConnection();
            }
        }

        private List<SaleRenewInsuranceActData> GetSaleRenewInsuranceData()
        {
            try
            {
                //Backup 27/06/2017
                //string sql = @"SELECT lead.slm_ticketId AS TicketId, ins.slm_ContractNo AS ContractNo, ins.slm_ReceiveNo AS ReceiveNo, ins.slm_ReceiveDate AS HVInfDate
                //                    , staff.slm_EmpCode AS EmpCode, com.slm_InsCode AS InsuranceComCode
                //                     ,CONVERT(varchar,ins.slm_PolicyStartCoverDate,103) AS PolicyStartDate
                //                        ,CONVERT(varchar,ins.slm_PolicyEndCoverDate,103) AS PolicyEndDate
                //                        , ins.slm_PolicyDiscountAmt AS PolicyDiscountAmt, ins.slm_PolicyGrossPremium AS PolicyGrossPremium
                //                        , ins.slm_CoverageTypeId AS CoverageTypeId, ins.slm_PolicyCost AS PolicyCost
                //                        , CASE WHEN repair.slm_RepairTypeName LIKE '%ห้าง%' THEN 'Y'
                //                               WHEN repair.slm_RepairTypeName LIKE '%อู่%' THEN 'N'
                //                               ELSE '' END AS RepairType
                //                        , ins.slm_ActGrossPremium AS ActGrossPremium, comact.slm_InsCode AS ActComCode
                //                        , CONVERT(varchar,ins.slm_ActStartCoverDate,103) AS ActStartDate
                //                        , CONVERT(varchar,ins.slm_ActEndCoverDate,103) AS ActEndDate, ins.slm_ActNetPremium AS ActNetPremium
                //                        , ins.slm_ActVat AS ActVat, ins.slm_ActStamp AS ActStamp
                //                        , ins.slm_ActGrossPremium AS HCFiller10
                //            FROM {0}.dbo.kkslm_tr_lead lead
                //            INNER JOIN {0}.dbo.kkslm_tr_renewinsurance ins ON ins.slm_TicketId = lead.slm_ticketId
                //            LEFT JOIN {1}.dbo.kkslm_ms_ins_com com ON com.slm_Ins_Com_Id = ins.slm_InsuranceComId
                //            LEFT JOIN {1}.dbo.kkslm_ms_ins_com comact ON comact.slm_Ins_Com_Id = ins.slm_ActComId
                //            LEFT JOIN {0}.dbo.kkslm_ms_repairtype repair ON repair.slm_RepairTypeId = ins.slm_RepairTypeId
                //            LEFT JOIN {0}.dbo.kkslm_ms_staff staff ON staff.slm_UserName = lead.slm_Owner
                //            WHERE lead.is_Deleted = 0 AND lead.slm_Status NOT IN ('08','09','10') 
                //            AND (ins.slm_PolicyGrossPremium > 0 OR ins.slm_ActGrossPremium > 0)
                //            AND (ins.slm_ObtPro08Flag IS NULL OR ins.slm_ObtPro08Flag = 0) ";

                string sql = @"SELECT lead.slm_ticketId AS TicketId, ins.slm_ContractNo AS ContractNo, ins.slm_ReceiveNo AS ReceiveNo, ins.slm_ReceiveDate AS HVInfDate
                                    , staff.slm_EmpCode AS EmpCode, com.slm_InsCode AS InsuranceComCode
                                     ,CONVERT(varchar,ins.slm_PolicyStartCoverDate,103) AS PolicyStartDate
                                        ,CONVERT(varchar,ins.slm_PolicyEndCoverDate,103) AS PolicyEndDate
                                        , ins.slm_PolicyDiscountAmt AS PolicyDiscountAmt, ins.slm_PolicyGrossPremium AS PolicyGrossPremium
                                        , ins.slm_CoverageTypeId AS CoverageTypeId, ins.slm_PolicyCost AS PolicyCost
                                        , CASE WHEN repair.slm_RepairTypeName LIKE '%ห้าง%' THEN 'Y'
                                               WHEN repair.slm_RepairTypeName LIKE '%อู่%' THEN 'N'
                                               ELSE '' END AS RepairType
                                        , ins.slm_ActGrossPremium AS ActGrossPremium, comact.slm_InsCode AS ActComCode
                                        , CONVERT(varchar,ins.slm_ActStartCoverDate,103) AS ActStartDate
                                        , CONVERT(varchar,ins.slm_ActEndCoverDate,103) AS ActEndDate, ins.slm_ActNetPremium AS ActNetPremium
                                        , ins.slm_ActVat AS ActVat, ins.slm_ActStamp AS ActStamp
                                        , ins.slm_ActGrossPremium AS HCFiller10
                            FROM {0}.dbo.kkslm_tr_lead lead
                            INNER JOIN {0}.dbo.kkslm_tr_renewinsurance ins ON ins.slm_TicketId = lead.slm_ticketId
                            LEFT JOIN {1}.dbo.kkslm_ms_ins_com com ON com.slm_Ins_Com_Id = ins.slm_InsuranceComId
                            LEFT JOIN {1}.dbo.kkslm_ms_ins_com comact ON comact.slm_Ins_Com_Id = ins.slm_ActComId
                            LEFT JOIN {0}.dbo.kkslm_ms_repairtype repair ON repair.slm_RepairTypeId = ins.slm_RepairTypeId
                            LEFT JOIN {0}.dbo.kkslm_ms_staff staff ON staff.slm_UserName = lead.slm_Owner
                            WHERE lead.is_Deleted = 0 AND lead.slm_Status NOT IN ('08','09','10') 
                            AND ins.slm_ReceiveNo IS NOT NULL 
                            AND (ins.slm_ObtPro08Flag IS NULL OR ins.slm_ObtPro08Flag = 0) ";

                sql = string.Format(sql, AppConstant.SLMDBName, AppConstant.OPERDBName);
                using (var slmdb = AppUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<SaleRenewInsuranceActData>(sql).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RollbackHP(SQLServerDB hpdb, Int64 batchMonitorId, string batchCode, List<TempDataPro08> tempList)
        {
            try
            {
                string sql = "";
                foreach (TempDataPro08 data in tempList)
                {
                    sql = @"DELETE FROM HP_MKT_Upload_Detail_OBT
                            WHERE [YEAR] = '" + data.YEAR + @"'
                                AND [MONTH] = '" + data.MONTH + @"'
                                AND CNT_NO = '" + data.CNT_NO + @"'
                                AND HV_INF_NO = '" + data.HV_INF_NO + @"'
                                AND HV_INF_DATE = '" + data.HV_INF_DATE + @"'
                                AND HV_FIN_CODE = '" + data.HV_FIN_CODE + @"'
                                AND HV_INS_CODE = '" + data.HV_INS_CODE + @"'
                                AND HV_EFF_DATE = '" + data.HV_EFF_DATE + @"'
                                AND HV_EXP_DATE = '" + data.HV_EXP_DATE + @"'
                                AND HV_DISC_TOTAL = '" + data.HV_DISC_TOTAL + @"'
                                AND HV_GROSS_PREM = '" + data.HV_GROSS_PREM + @"'
                                AND HV_VOL_TYPE = '" + data.HV_VOL_TYPE + @"'
                                AND HV_COV_AMT = '" + data.HV_COV_AMT + @"'
                                AND HV_FIXED_CLASS = '" + data.HV_FIXED_CLASS + @"'
                                AND HC_GROSS_PREM = '" + data.HC_GROSS_PREM + @"'
                                AND HC_INS_CODE = '" + data.HC_INS_CODE + @"'
                                AND HC_EFF_DATE = '" + data.HC_EFF_DATE + @"'
                                AND HC_EXP_DATE = '" + data.HC_EXP_DATE + @"'
                                AND HC_NET_PREM = '" + data.HC_NET_PREM + @"'
                                AND HC_VAT_AMT = '" + data.HC_VAT_AMT + @"'
                                AND HC_STAMP = '" + data.HC_STAMP + @"'
                                AND HC_FILLER10 = '" + data.HC_FILLER10 + @"' ";

                    hpdb.ExecuteNonQuery(sql);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                Util.WriteLogFile(logfilename, batchCode, message);
            }
        }

        private void RollbackRenewInsurance(SQLServerDB slmdb, Int64 batchMonitorId, string batchCode, List<TempDataPro08> tempList)
        {
            try
            {
                string sql = "";
                foreach (TempDataPro08 data in tempList)
                {
                    sql = @"UPDATE " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance
                                SET slm_ObtPro08Flag = 0, slm_ObtPro08FlagDate = NULL
                                WHERE slm_TicketId = '" + data.TicketId + "'";

                    slmdb.ExecuteNonQuery(sql);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                Util.WriteLogFile(logfilename, batchCode, message);
            }
        }

        private void CreateExcel(Int64 batchMonitorId, string batchCode, List<TempDataPro08> list)
        {
            try
            {
                string fileName = "";

                using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                {
                    if (unc.NetUseWithCredentials(AppConstant.ExcelSharePath, AppConstant.ExcelSharePathUsername, AppConstant.ExcelSharePathDomainName, AppConstant.ExcelSharePathPassword))
                    {
                        fileName = System.IO.Path.Combine(AppConstant.ExcelSharePath, "ExportHP_" + DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd_HHmmss") + ".xlsx");
                    }
                    else
                    {
                        throw new Exception(string.Format("Create Excel Failed: Unable to access {0}", AppConstant.ExcelSharePath));
                    }
                }

                using (SpreadsheetDocument document = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "HP" };

                    sheets.Append(sheet);
                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Constructing header
                    Row row = new Row();

                    row.Append(
                        ConstructCell("ปี", CellValues.String),
                        ConstructCell("เดือน", CellValues.String),
                        ConstructCell("เลขที่สัญญา", CellValues.String),
                        ConstructCell("เลขที่รับแจ้ง", CellValues.String),
                        ConstructCell("วันที่รับแจ้ง", CellValues.String),
                        ConstructCell("รหัสพนักงาน", CellValues.String),
                        ConstructCell("บริษัทประกัน", CellValues.String),
                        ConstructCell("วันเริ่มคุ้มครอง", CellValues.String),
                        ConstructCell("วันหมดอายุประกัน", CellValues.String),
                        ConstructCell("จำนวนส่วนลด", CellValues.String),
                        ConstructCell("เบี้ยประกันที่ชำระ", CellValues.String),
                        ConstructCell("รหัสประเภทประกันภัย", CellValues.String),
                        ConstructCell("ทุนประกัน", CellValues.String),
                        ConstructCell("ประเภทการซ่อม", CellValues.String),
                        ConstructCell("เบี้ยพรบ.ที่ต้องชำระ", CellValues.String),
                        ConstructCell("บริษัทพรบ.", CellValues.String),
                        ConstructCell("วันเริ่มต้นพรบ.", CellValues.String),
                        ConstructCell("วันสิ้นสุดพรบ.", CellValues.String),
                        ConstructCell("เบี้ยสุทธิพรบ.", CellValues.String),
                        ConstructCell("ภาษีพรบ.", CellValues.String),
                        ConstructCell("อากรพรบ.", CellValues.String),
                        ConstructCell("เบี้ยพรบ.ที่ต้องชำระ", CellValues.String)
                    );

                    // Insert the header row to the Sheet Data
                    sheetData.AppendChild(row);

                    // Inserting each employee
                    foreach (var data in list)
                    {
                        row = new Row();
                        row.Append(
                            ConstructCell(data.YEAR, CellValues.String),
                            ConstructCell(data.MONTH, CellValues.String),
                            ConstructCell(data.CNT_NO, CellValues.String),
                            ConstructCell(data.HV_INF_NO, CellValues.String),
                            ConstructCell(data.HV_INF_DATE, CellValues.String),
                            ConstructCell(data.HV_FIN_CODE, CellValues.String),
                            ConstructCell(data.HV_INS_CODE, CellValues.String),
                            ConstructCell(data.HV_EFF_DATE, CellValues.String),
                            ConstructCell(data.HV_EXP_DATE, CellValues.String),
                            ConstructCell(data.HV_DISC_TOTAL, CellValues.String),
                            ConstructCell(data.HV_GROSS_PREM, CellValues.String),
                            ConstructCell(data.HV_VOL_TYPE, CellValues.String),
                            ConstructCell(data.HV_COV_AMT, CellValues.String),
                            ConstructCell(data.HV_FIXED_CLASS, CellValues.String),
                            ConstructCell(data.HC_GROSS_PREM, CellValues.String),
                            ConstructCell(data.HC_INS_CODE, CellValues.String),
                            ConstructCell(data.HC_EFF_DATE, CellValues.String),
                            ConstructCell(data.HC_EXP_DATE, CellValues.String),
                            ConstructCell(data.HC_NET_PREM, CellValues.String),
                            ConstructCell(data.HC_VAT_AMT, CellValues.String),
                            ConstructCell(data.HC_STAMP, CellValues.String),
                            ConstructCell(data.HC_FILLER10, CellValues.String)
                        );

                        sheetData.AppendChild(row);
                    }

                    worksheetPart.Worksheet.Save();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                Util.WriteLogFile(logfilename, batchCode, message);
            }
        }

        private Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
    }

    public class TempDataPro08
    {
        public string YEAR { get; set; }
        public string MONTH { get; set; }
        public string CNT_NO { get; set; }
        public string HV_INF_NO { get; set; }
        public string HV_INF_DATE { get; set; }
        public string HV_FIN_CODE { get; set; }
        public string HV_INS_CODE { get; set; }
        public string HV_EFF_DATE { get; set; }
        public string HV_EXP_DATE { get; set; }
        public string HV_DISC_TOTAL { get; set; }
        public string HV_GROSS_PREM { get; set; }
        public string HV_VOL_TYPE { get; set; }
        public string HV_COV_AMT { get; set; }
        public string HV_FIXED_CLASS { get; set; }
        public string HC_GROSS_PREM { get; set; }
        public string HC_INS_CODE { get; set; }
        public string HC_EFF_DATE { get; set; }
        public string HC_EXP_DATE { get; set; }
        public string HC_NET_PREM { get; set; }
        public string HC_VAT_AMT { get; set; }
        public string HC_STAMP { get; set; }
        public string HC_FILLER10 { get; set; }
        public string TicketId { get; set; }
    }
}
