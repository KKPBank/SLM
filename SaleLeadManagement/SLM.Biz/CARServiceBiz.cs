using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class CARServiceBiz
    {
        string _error = "";
        public string ErrorMessage {  get { return _error; } }
        public bool SaveCarResend(CASResendData rs, int maxResendCount)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var rsu = new kkslm_tr_cas_resend();

                    rsu.slm_CasReferenceNo = rs.slm_CasReferenceNo;
                    rsu.slm_CasSystemCode = rs.slm_CasSystemCode;
                    rsu.slm_CasServiceName = rs.slm_CasServiceName;
                    rsu.slm_CasLogType = rs.slm_CasLogType;
                    rsu.slm_CasResponse = rs.slm_CasResponse;
                    rsu.slm_CasErrorMessage = rs.slm_CasErrorMessage;
                    rsu.slm_CasDetail = rs.slm_CasDetail;
                    rsu.slm_LogStatus = "R";
                    rsu.slm_LogDate = DateTime.Now;
                    rsu.slm_ResendCount = 0;
                    rsu.slm_CreatedBy = rs.slm_CreatedBy;
                    rsu.slm_CreatedDate = DateTime.Now;
                    rsu.slm_UpdatedBy = rs.slm_UpdatedBy;
                    rsu.slm_UpdatedDate = DateTime.Now;

                    sdc.kkslm_tr_cas_resend.AddObject(rsu);
                    sdc.SaveChanges();

                    #region Backup
                    //var rsu = sdc.kkslm_tr_cas_resend.Where(r => r.slm_CasReferenceNo == rs.slm_CasReferenceNo).FirstOrDefault();
                    //if (rsu == null)
                    //{
                    //    // new just add

                    //    rsu = new kkslm_tr_cas_resend();

                    //    rsu.slm_CasReferenceNo = rs.slm_CasReferenceNo;
                    //    rsu.slm_CasSystemCode = rs.slm_CasSystemCode;
                    //    rsu.slm_CasServiceName = rs.slm_CasServiceName;
                    //    rsu.slm_CasLogType = rs.slm_CasLogType;
                    //    rsu.slm_CasResponse = rs.slm_CasResponse;
                    //    rsu.slm_CasErrorMessage = rs.slm_CasErrorMessage;
                    //    rsu.slm_CasDetail = rs.slm_CasDetail;
                    //    rsu.slm_LogStatus = "R";
                    //    rsu.slm_LogDate = DateTime.Now;
                    //    rsu.slm_ResendCount = 0;
                    //    rsu.slm_CreatedBy = rs.slm_CreatedBy;
                    //    rsu.slm_CreatedDate = DateTime.Now;
                    //    rsu.slm_UpdatedBy = rs.slm_UpdatedBy;
                    //    rsu.slm_UpdatedDate = DateTime.Now;

                    //    sdc.kkslm_tr_cas_resend.AddObject(rsu);
                    //}
                    //else
                    //{
                    //    // update, from batch
                    //    rsu.slm_CasSystemCode = rs.slm_CasSystemCode;
                    //    rsu.slm_CasServiceName = rs.slm_CasServiceName;
                    //    rsu.slm_UpdatedBy = rs.slm_CasSystemCode + "CARBatch";
                    //    rsu.slm_UpdatedDate = DateTime.Now;
                    //    rsu.slm_ResendCount++;

                    //    if (rsu.slm_ResendCount >= maxResendCount) rsu.slm_LogStatus = "E";

                    //}
                    //sdc.SaveChanges();
                    #endregion
                }
            }
            catch  (Exception ex)
            {
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ret = false;
            }
            return ret;
        }



        // resend batch 

        public List<kkslm_tr_cas_resend> GetResendList()
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_tr_cas_resend.Where(r => r.slm_LogStatus == "R").ToList();
            }
        }

        public bool UpdateCarResend(CASResendData rs, string updatedBy)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var log = sdc.kkslm_tr_cas_resend.Where(c => c.slm_CasLogId == rs.slm_CasLogId).FirstOrDefault();
                    if (log == null) throw new Exception("Invalid Log Id");

                    log.slm_LogStatus = rs.slm_LogStatus;
                    log.slm_ResendDate = rs.slm_ResendDate;
                    log.slm_ResendCount = rs.slm_ResendCount;
                    log.slm_LastResendDate = rs.slm_LastResendDate;
                    log.slm_LastResendRespone = rs.slm_LastResendRespone;
                    log.slm_LastResendMessage = rs.slm_LastResendMessage;
                    log.slm_BatchVersion = rs.slm_BatchVersion;                    
                    log.slm_UpdatedBy = updatedBy;
                    log.slm_UpdatedDate = DateTime.Now;

                    sdc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ret = false;
            }
            return ret;

        }

        // internal data

        public partial class CASResendData : kkslm_tr_cas_resend { }


        // batch monitor
        string batchCode = "OBT_PRO_25";
        public kkslm_ms_batch StartBatch(string pgID)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                if (batch != null)
                {
                    batch.slm_Status = "0";
                    batch.slm_StartTime = DateTime.Now;
                    batch.slm_EndTime = null;
                    batch.slm_ActionBy = pgID;
                    batch.slm_BatchDate = DateTime.Now;
                    batch.slm_ProcessTime = null;
                    sdc.SaveChanges();
                }
                return batch;
            }
        }


        public bool FinishBatch(int total, int succ, int fail, string status, List<kkslm_tr_batchlog> batchErrorList, string pgID)
        {
            bool ret = true;
            try {
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                    if (batch != null)
                    {
                        batch.slm_EndTime = DateTime.Now;
                        batch.slm_Status = status;
                        batch.slm_ProcessTime = Convert.ToInt32(Math.Ceiling((batch.slm_EndTime.Value - batch.slm_StartTime.Value).TotalSeconds / 60.0));

                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = batch.slm_StartTime;
                        bm.slm_DateEnd = batch.slm_EndTime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = pgID;
                        bm.slm_BatchDate = DateTime.Now.Date;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();

                        // insert batch log
                        if (batchErrorList != null && batchErrorList.Count > 0)
                        {
                            foreach(var itm in batchErrorList)
                            {
                                itm.slm_BatchMonitor_Id = bm.slm_BatchMonitor_Id;
                                sdc.kkslm_tr_batchlog.AddObject(itm);
                            }
                            sdc.SaveChanges();

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ret = false;
            }
            return ret;
        }



        // Get Data for CAR Service
        public static LeadDataForCARLogService GetDataForCARLogService(string ticketId, string preleadId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    LeadDataForCARLogService data = null;

                    if (!string.IsNullOrEmpty(ticketId))
                    {
                        string sql = @"SELECT lead.slm_TicketId AS TicketId, prelead.slm_Prelead_Id AS PreleadId, lead.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName, lead.slm_ChannelId AS ChannelId, lead.slm_Product_Group_Id AS ProductGroupId, lead.slm_Product_Id AS ProductId
                                    , lead.slm_Status AS StatusCode, opt.slm_OptionDesc AS StatusName, lead.slm_SubStatus AS SubStatusCode, lead.slm_ExternalSubStatusDesc AS SubStatusName
                                    , cus.slm_CitizenId AS CitizenId, cus.slm_CardType AS CardTypeId, cardtype.slm_CardTypeName AS CardTypeName, pg.product_name AS ProductGroupName, mp.sub_product_name AS ProductName
                                    , renew.slm_LicenseNo AS LicenseNo, renew.slm_ContractNo AS ContractNo
                                    , LEAD.slm_Owner AS Owner, STAFF.slm_StaffEmail AS OwnerEmail, LEAD.slm_Delegate AS Delegate, STAFF2.slm_StaffEmail AS DelegateEmail 
                                    , insurcom.slm_InsNameTh AS InsuranceCompany, renewcompare.slm_OD AS OD, renewcompare.slm_FT AS FT, renewcompare.slm_DiscountBath AS DiscountBath, renewcompare.slm_Vat1PercentBath AS Vat1PercentBath
                                    , renewcompare.slm_PolicyGrossPremiumPay AS PolicyGrossPremiumPay, ISNULL(title.slm_TitleName, '') + lead.slm_Name + ' ' + ISNULL(lead.slm_LastName, '') AS CustomerName, cardtype.slm_CIFSubscriptTypeId AS CBSSubScriptionTypeId
                                    , renew.slm_IncentiveFlag AS IncentiveFlag, renew.slm_ActIncentiveFlag AS IncentiveFlagAct, renew.slm_ReceiveNo as ReceiveNo, renew.slm_ActSendDate as ActSendDate 
                                    FROM kkslm_tr_lead LEAD
                                    LEFT JOIN kkslm_tr_cusinfo cus ON cus.slm_TicketId = lead.slm_ticketId
                                    LEFT JOIN kkslm_tr_renewinsurance renew ON renew.slm_TicketId = lead.slm_ticketId
                                    LEFT JOIN kkslm_ms_option opt ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' AND opt.is_Deleted = '0'
                                    LEFT JOIN kkslm_ms_campaign cam ON cam.slm_CampaignId = lead.slm_CampaignId
                                    LEFT JOIN kkslm_ms_cardtype cardtype ON cardtype.slm_CardTypeId = cus.slm_CardType
                                    LEFT JOIN kkslm_ms_staff STAFF ON LEAD.slm_Owner = STAFF.slm_UserName AND STAFF.is_Deleted = 0
                                    LEFT JOIN kkslm_ms_staff STAFF2 ON LEAD.slm_Delegate = STAFF2.slm_UserName AND STAFF2.is_Deleted = 0
                                    LEFT JOIN CMT_MS_PRODUCT_GROUP pg ON lead.slm_Product_Group_Id = pg.product_id
                                    LEFT JOIN CMT_MAPPING_PRODUCT mp ON mp.sub_product_id = lead.slm_Product_Id 
                                    LEFT JOIN " + SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com insurcom ON insurcom.slm_Ins_Com_Id = renew.slm_InsuranceComId
                                    LEFT JOIN kkslm_tr_renewinsurance_compare renewcompare ON renewcompare.slm_RenewInsureId = renew.slm_RenewInsureId AND renewcompare.slm_Selected = 1 
                                    LEFT JOIN kkslm_tr_prelead prelead ON prelead.slm_TicketId = lead.slm_TicketId
                                    LEFT JOIN kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId AND title.is_Deleted = 0
                                    WHERE LEAD.slm_ticketId = '" + ticketId + "' ";

                        data = slmdb.ExecuteStoreQuery<LeadDataForCARLogService>(sql).FirstOrDefault();
                    }
                    else if (!string.IsNullOrEmpty(preleadId))
                    {
                        string sql = @"SELECT '' AS TicketId, prelead.slm_Prelead_Id AS PreleadId, prelead.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName, '" + SLMConstant.CARLogService.CARPreleadChannelId + @"' AS ChannelId, mp.product_id AS ProductGroupId, prelead.slm_Product_Id AS ProductId
                                    , prelead.slm_Status AS StatusCode, opt.slm_OptionDesc AS StatusName, prelead.slm_SubStatus AS SubStatusCode, '' AS SubStatusName
                                    , prelead.slm_CitizenId AS CitizenId, prelead.slm_CardTypeId AS CardTypeId, cardtype.slm_CardTypeName AS CardTypeName, pg.product_name AS ProductGroupName, mp.sub_product_name AS ProductName
                                    , prelead.slm_Car_License_No AS LicenseNo, prelead.slm_Contract_Number AS ContractNo
                                    , prelead.slm_Owner AS Owner, STAFF.slm_StaffEmail AS OwnerEmail, '' AS Delegate, '' AS DelegateEmail 
                                    , insurcom.slm_InsNameTh AS InsuranceCompany, compare.slm_OD AS OD, compare.slm_FT AS FT, compare.slm_DiscountBath AS DiscountBath, compare.slm_Vat1PercentBath AS Vat1PercentBath
                                    , compare.slm_PolicyGrossPremiumPay AS PolicyGrossPremiumPay, ISNULL(title.slm_TitleName, '') + prelead.slm_Name + ' ' + ISNULL(prelead.slm_LastName, '') AS CustomerName, cardtype.slm_CIFSubscriptTypeId AS CBSSubScriptionTypeId
									, null AS IncentiveFlag, null AS IncentiveFlagAct, null as ActSendDate, '' as ReceiveNo
                                    FROM kkslm_tr_prelead prelead
                                    LEFT JOIN kkslm_ms_option opt ON prelead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' AND opt.is_Deleted = '0'
                                    LEFT JOIN kkslm_ms_cardtype cardtype ON cardtype.slm_CardTypeId = prelead.slm_CardTypeId
                                    LEFT JOIN kkslm_ms_campaign cam ON cam.slm_CampaignId = prelead.slm_CampaignId
                                    LEFT JOIN kkslm_ms_staff STAFF ON prelead.slm_Owner = STAFF.slm_EmpCode AND STAFF.is_Deleted = 0
                                    LEFT JOIN CMT_MAPPING_PRODUCT mp ON mp.sub_product_id = prelead.slm_Product_Id
                                    LEFT JOIN CMT_MS_PRODUCT_GROUP pg ON mp.product_id = pg.product_id 
                                    LEFT JOIN kkslm_tr_prelead_compare compare ON compare.slm_Prelead_Id = prelead.slm_Prelead_Id AND compare.slm_Selected = 1
                                    LEFT JOIN " + SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com insurcom ON insurcom.slm_Ins_Com_Id = compare.slm_Ins_Com_Id
                                    LEFT JOIN kkslm_ms_title title ON title.slm_TitleId = prelead.slm_TitleId AND title.is_Deleted = 0
                                    WHERE prelead.slm_Prelead_Id = '" + preleadId + "' ";

                        data = slmdb.ExecuteStoreQuery<LeadDataForCARLogService>(sql).FirstOrDefault();

                        if (data != null)
                        {
                            var list = new KKSlmMsConfigProductSubstatusModel().GetSubStatusList(data.ProductId, data.CampaignId, data.StatusCode);
                            string subStatusName = list.Where(p => p.ValueField == data.SubStatusCode).Select(p => p.TextField).FirstOrDefault();
                            data.SubStatusName = subStatusName;
                        }
                    }

                    return data;
                }
            }
            catch
            {
                throw;
            }
        }


    }

}
