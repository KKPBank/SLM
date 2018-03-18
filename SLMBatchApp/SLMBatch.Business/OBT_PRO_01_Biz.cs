using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;

namespace SLMBatch.Business
{
    public class OBT_PRO_01_Biz : ServiceBase
    {
        #region Backup 04-06-2016 Classic
        //public void InsertReinsuranceView(string batchCode)
        //{
        //    Int64 batchMonitorId = 0;
        //    int totalRecord = 0;
        //    int totalSuccess = 0;
        //    int totalFail = 0;

        //    try
        //    {
        //        batchMonitorId = BizUtil.SetStartTime(batchCode);
        //        BizUtil.CheckPrerequisite(batchCode);

        //        List<RenewInsuranceViewData> list = RenewInsuranceDataList();
        //        totalRecord = list.Count;

        //        DeleteExistingData();

        //        foreach (RenewInsuranceViewData data in list)
        //        {
        //            try
        //            {
        //                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

        //                DateTime createDate = DateTime.Now;
        //                kkslm_tr_renewinsurance_view view = new kkslm_tr_renewinsurance_view()
        //                {
        //                    slm_TicketId = data.TicketId,
        //                    slm_CampaignId = data.CampaignId,
        //                    slm_Product_Id = data.ProductId,
        //                    slm_TitleName = data.TitleName,
        //                    slm_Name = data.FirstName,
        //                    slm_LastName = data.LastName,
        //                    slm_TelNo_1 = data.TelNo1,
        //                    slm_CardTypeName = data.CardTypeName,
        //                    slm_CitizenId = data.CitizenId,
        //                    slm_Birthdate = data.BirthDate,
        //                    slm_MaritalStatus = data.MaritalStatus,
        //                    slm_OccupationNameTH = data.OccupationName,
        //                    slm_ContractNo = data.ContractNo,
        //                    slm_PolicyNo = data.PolicyNo,
        //                    slm_PolicyStartCoverDate = data.PolicyStartDate,
        //                    slm_PolicyEndCoverDate = data.PolicyEndDate,
        //                    slm_PolicyCost = data.PolicyCost,
        //                    slm_PolicyGrossPremium = data.PolicyGrossPremium,
        //                    slm_PolicyDiscountAmt = data.PolicyDiscount,
        //                    slm_ActNo = data.ActNo,
        //                    slm_ActStartCoverDate = data.ActStartDate,
        //                    slm_ActEndCoverDate = data.ActEndDate,
        //                    slm_ActGrossPremium = data.ActGrossPremium,
        //                    slm_ActVat = data.ActVat,
        //                    slm_ActStamp = data.ActStamp,
        //                    slm_ActNetPremium = data.ActNetPremium,
        //                    slm_CreatedBy = "SYSTEM",
        //                    slm_CreatedDate = createDate,
        //                    slm_UpdatedBy = "SYSTEM",
        //                    slm_UpdatedDate = createDate,
        //                };
                        
        //                slmdb.kkslm_tr_renewinsurance_view.AddObject(view);
        //                slmdb.SaveChanges();

        //                totalSuccess += 1;
        //                Console.WriteLine("TicketId " + data.TicketId + ": SUCCESS");
        //            }
        //            catch (Exception ex)
        //            {
        //                totalFail += 1;
        //                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                string errorDetail = "TicketId=" + data.TicketId + ", Error=" + message;

        //                BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
        //                Util.WriteLogFile(logfilename, batchCode, errorDetail);

        //                Console.WriteLine("TicketId " + data.TicketId + ": FAIL");
        //            }
        //        }

        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("All FAIL");
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

        //        Util.WriteLogFile(logfilename, batchCode, message);
        //        BizUtil.InsertLog(batchMonitorId, "", "", message);
        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
        //        throw ex;
        //    }
        //}
        #endregion
        
        public void InsertReinsuranceView(string batchCode)
        {
            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;
            bool successFlag = true;

            try
            {
                batchMonitorId = BizUtil.SetStartTime(batchCode);
                BizUtil.CheckPrerequisite(batchCode);

                List<RenewInsuranceViewData> list = RenewInsuranceDataList();
                totalRecord = list.Count;

                DeleteExistingData();

                foreach (RenewInsuranceViewData data in list)
                {
                    try
                    {
                        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

                        DateTime createDate = DateTime.Now;
                        kkslm_tr_renewinsurance_view view = new kkslm_tr_renewinsurance_view()
                        {
                            slm_TicketId = data.TicketId,
                            slm_CampaignId = data.CampaignId,
                            slm_Product_Id = data.ProductId,
                            slm_TitleName = data.TitleName,
                            slm_Name = data.FirstName,
                            slm_LastName = data.LastName,
                            slm_TelNo_1 = data.TelNo1,
                            slm_CardTypeName = data.CardTypeName,
                            slm_CitizenId = data.CitizenId,
                            slm_Birthdate = data.BirthDate,
                            slm_MaritalStatus = data.MaritalStatus,
                            slm_OccupationNameTH = data.OccupationName,
                            slm_ContractNo = data.ContractNo,
                            slm_PolicyNo = data.PolicyNo,
                            slm_PolicyStartCoverDate = data.PolicyStartDate,
                            slm_PolicyEndCoverDate = data.PolicyEndDate,
                            slm_PolicyCost = data.PolicyCost,
                            slm_PolicyGrossPremium = data.PolicyGrossPremium,
                            slm_PolicyDiscountAmt = data.PolicyDiscount,
                            slm_ActNo = data.ActNo,
                            slm_ActStartCoverDate = data.ActStartDate,
                            slm_ActEndCoverDate = data.ActEndDate,
                            slm_ActGrossPremium = data.ActGrossPremium,
                            slm_ActVat = data.ActVat,
                            slm_ActStamp = data.ActStamp,
                            slm_ActNetPremium = data.ActNetPremium,
                            slm_CreatedBy = "SYSTEM",
                            slm_CreatedDate = createDate,
                            slm_UpdatedBy = "SYSTEM",
                            slm_UpdatedDate = createDate,
                        };

                        slmdb.kkslm_tr_renewinsurance_view.AddObject(view);
                        slmdb.SaveChanges();

                        totalSuccess += 1;
                        Console.WriteLine("TicketId " + data.TicketId + ": SUCCESS");
                    }
                    catch (Exception ex)
                    {
                        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        string errorDetail = "TicketId=" + data.TicketId + ", Error=" + message;

                        BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
                        Util.WriteLogFile(logfilename, batchCode, errorDetail);

                        successFlag = false;
                        totalSuccess = 0;
                        totalFail = totalRecord;
                        DeleteExistingData();
                        break;
                    }
                }

                BizUtil.SetEndTime(batchCode, batchMonitorId, (successFlag ? AppConstant.Success : AppConstant.Fail), totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Util.WriteLogFile(logfilename, batchCode, message);
                BizUtil.InsertLog(batchMonitorId, "", "", message);

                totalSuccess = 0;
                totalFail = totalRecord;

                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
        }

        private List<RenewInsuranceViewData> RenewInsuranceDataList()
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

                string sql = @"SELECT lead.slm_ticketId AS TicketId, lead.slm_CampaignId AS CampaignId, lead.slm_Product_Id AS ProductId
                                , title.slm_TitleName AS TitleName, lead.slm_Name AS FirstName, lead.slm_LastName AS LastName, lead.slm_TelNo_1 AS TelNo1
                                , ct.slm_CardTypeName AS CardTypeName, cus.slm_CitizenId AS CitizenId, cus.slm_Birthdate AS BirthDate, cus.slm_MaritalStatus AS MaritalStatus
                                , occ.slm_OccupationNameTH AS OccupationName, reins.slm_ContractNo AS ContractNo, reins.slm_PolicyNo AS PolicyNo, reins.slm_PolicyStartCoverDate AS PolicyStartDate
                                , reins.slm_PolicyEndCoverDate AS PolicyEndDate, reins.slm_PolicyCost AS PolicyCost, reins.slm_PolicyGrossPremium AS PolicyGrossPremium, reins.slm_PolicyDiscountAmt AS PolicyDiscount
                                , reins.slm_ActNo AS ActNo, reins.slm_ActStartCoverDate AS ActStartDate, reins.slm_ActEndCoverDate AS ActEndDate
                                , reins.slm_ActGrossPremium AS ActGrossPremium, reins.slm_ActVat AS ActVat, reins.slm_ActStamp AS ActStamp, reins.slm_ActNetPremium AS ActNetPremium
                                FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
                                LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_cusinfo cus ON cus.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId
                                LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_cardtype ct ON ct.slm_CardTypeId = cus.slm_CardType
                                LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_occupation occ ON cus.slm_Occupation = occ.slm_OccupationId
                                INNER JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance reins ON reins.slm_TicketId = lead.slm_ticketId	--inner เอาเฉพาะ renew insurance
                                WHERE lead.is_Deleted = 0 ";

                return slmdb.ExecuteStoreQuery<RenewInsuranceViewData>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteExistingData()
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                string sql = "TRUNCATE TABLE " + AppConstant.SLMDBName + ".dbo.kkslm_tr_renewinsurance_view";
                slmdb.ExecuteStoreCommand(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
