using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;
using SLM.Dal;
using System.Transactions;
using SLM.Resource;

namespace SLM.Biz
{
    public class LeadInfoBiz
    {
        private string _Error = "";
        public string ErrorMessage
        {
            get { return _Error; }
        }

        private Dictionary<string, string> _errList = new Dictionary<string, string>();
        public Dictionary<string, string> ErrorList
        {
            get { return _errList; }
        }

        public static string InsertLeadData(LeadData leadData, CampaignWSData camData, string createbyUsername)
        {
            string ticketId = "";
            try
            {
                StoreProcedure store = new StoreProcedure();
                ticketId = store.GenerateTicketId();
                leadData.TicketId = ticketId;
                camData.TicketId = ticketId;

                DateTime createDate = DateTime.Now;

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
                    lead.InsertData(leadData, createbyUsername, createDate);

                    KKSlmTrCusInfoModel customerInfo = new KKSlmTrCusInfoModel();
                    customerInfo.InsertData(leadData, createbyUsername, createDate);

                    KKSlmTrProductInfoModel productInfo = new KKSlmTrProductInfoModel();
                    productInfo.InsertData(leadData, createbyUsername);

                    KKSlmTrChannelInfoModel channelInfo = new KKSlmTrChannelInfoModel();
                    channelInfo.InsertData(leadData, createbyUsername, createDate);

                    KKSLMTrCampaignFinalModel camFinal = new KKSLMTrCampaignFinalModel();
                    camFinal.InsertData(camData, createbyUsername, createDate);

                    KKSlmTrHistoryModel history = new KKSlmTrHistoryModel();
                    history.InsertData(ticketId, SLMConstant.HistoryTypeCode.CreateLead, "", "", createbyUsername, createDate);

                    ts.Complete();
                }
                return ticketId;
            }
            catch
            {
                throw;
            }
        }

        //public static string UpdateLeadData(LeadData leadData, string username, bool actStatus, bool actDelegate, bool actOwner)
        public static string UpdateLeadData(LeadData leadData, string username, bool actStatus, bool actDelegate, bool actOwner, string oldOwner)
        {
            try
            {
                DateTime updateDate = DateTime.Now;
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
                    lead.UpdateData(leadData, username, actDelegate, actOwner, updateDate, oldOwner);

                    KKSlmTrCusInfoModel customerInfo = new KKSlmTrCusInfoModel();
                    customerInfo.UpdateData(leadData, username, updateDate);

                    KKSlmTrProductInfoModel productInfo = new KKSlmTrProductInfoModel();
                    productInfo.UpdateData(leadData, username);

                    KKSlmTrChannelInfoModel channelInfo = new KKSlmTrChannelInfoModel();
                    channelInfo.UpdateData(leadData, username);

                    if (actStatus == true || actDelegate == true)
                    {
                        KKSlmTrActivityModel Activity = new KKSlmTrActivityModel();
                        Activity.InsertData(leadData, username, updateDate);

                        KKSlmTrHistoryModel historydel = new KKSlmTrHistoryModel();
                        historydel.InsertData(leadData.TicketId, SLMConstant.HistoryTypeCode.UpdateDelegate, leadData.OldDelegate, leadData.NewDelegate, username, updateDate);
                    }

                    if (actOwner == true)
                    {
                        KKSlmTrActivityModel Activity = new KKSlmTrActivityModel();
                        Activity.InsertDataChangeOwner(leadData, username, updateDate);

                        KKSlmTrHistoryModel historydel = new KKSlmTrHistoryModel();
                        historydel.InsertData(leadData.TicketId, SLMConstant.HistoryTypeCode.UpdateOwner, leadData.OldOwner2, leadData.NewOwner2, username, updateDate);
                    }

                    KKSlmTrHistoryModel history = new KKSlmTrHistoryModel();
                    history.InsertData(leadData.TicketId, SLMConstant.HistoryTypeCode.UpdateLead, "", "", username, updateDate);

                    ts.Complete();
                }
                return leadData.TicketId;
            }
            catch
            {
                throw;
            }
        }

        //public bool InsertLeadData(LeadData leadData, List<CampaignWSData> camDataList, string createbyUsername)
        //{
        //    string ticketId = "";
        //    try
        //    {
        //        StoreProcedure store = new StoreProcedure();
        //        ticketId = store.GenerateTicketId();
        //        leadData.TicketId = ticketId;

        //        foreach (CampaignWSData cpdata in camDataList)
        //        {
        //            cpdata.TicketId = ticketId;
        //        }

        //        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        //        {
        //            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
        //            lead.InsertData(leadData, createbyUsername);

        //            KKSlmTrCusInfoModel customerInfo = new KKSlmTrCusInfoModel();
        //            customerInfo.InsertData(leadData, createbyUsername);

        //            KKSlmTrProductInfoModel productInfo = new KKSlmTrProductInfoModel();
        //            productInfo.InsertData(leadData, createbyUsername);

        //            KKSlmTrChannelInfoModel channelInfo = new KKSlmTrChannelInfoModel();
        //            channelInfo.InsertData(leadData, createbyUsername);

        //            KKSLMTrCampaignFinalModel camFinal = new KKSLMTrCampaignFinalModel();
        //            camFinal.InsertCampaignList(camDataList, createbyUsername);

        //            ts.Complete();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _Error = ex.Message.ToString();
        //        return false;
        //    }
        //}

        public string InsertLeadSuggestCampaign(LeadData leadData, CampaignWSData cpdata, string createByUsername)
        {
            string ticketId = "";
            try
            {
                DateTime createDate = DateTime.Now;
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    StoreProcedure store = new StoreProcedure();
                    ticketId = store.GenerateTicketId();
                    leadData.TicketId = ticketId;
                    cpdata.TicketId = ticketId;

                    List<ProductData> prodList = SlmScr016Biz.GetProductCampaignDataForSuggestCampaign(cpdata.CampaignId);
                    if (prodList.Count > 0)
                    {
                        leadData.ProductGroupId = prodList[0].ProductGroupId;
                        leadData.ProductId = prodList[0].ProductId;
                        leadData.ProductName = prodList[0].ProductName;
                    }

                    KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
                    lead.InsertData(leadData, createByUsername, createDate);

                    KKSlmTrCusInfoModel customerInfo = new KKSlmTrCusInfoModel();
                    customerInfo.InsertData(leadData, createByUsername, createDate);

                    KKSlmTrProductInfoModel productInfo = new KKSlmTrProductInfoModel();
                    productInfo.InsertData(leadData, createByUsername);

                    KKSlmTrChannelInfoModel channelInfo = new KKSlmTrChannelInfoModel();
                    channelInfo.InsertData(leadData, createByUsername, createDate);

                    KKSLMTrCampaignFinalModel camFinal = new KKSLMTrCampaignFinalModel();
                    camFinal.InsertData(cpdata, createByUsername, createDate);

                    //Add RenewInsurance By Pom 24/03/2016
                    KKSlmMsConfigProductScreenModel screen = new KKSlmMsConfigProductScreenModel();
                    string tableName = screen.GetTableName(leadData.CampaignId, leadData.ProductId, "I");
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        switch (tableName)
                        {
                            case "kkslm_tr_renewinsurance":
                                KKSlmTrRenewinsuranceModel reins = new KKSlmTrRenewinsuranceModel();
                                reins.InsertData(ticketId, createByUsername, createDate);
                                break;
                            default:
                                break;
                        }
                    }

                    KKSlmTrHistoryModel history = new KKSlmTrHistoryModel();
                    history.InsertData(ticketId, SLMConstant.HistoryTypeCode.CreateLead, "", "", createByUsername, createDate);

                    ts.Complete();
                }
                return ticketId;
            }
            catch
            {
                throw;
            }
        }

        //public bool InsertLeadData2(LeadData leadData, List<CampaignWSData> camDataList, string UserId, List<string> ticketIdList)
        //{
        //    string ticketId = "";
        //    try
        //    {
        //        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        //        {
        //            foreach (CampaignWSData cpdata in camDataList)
        //            {
        //                StoreProcedure store = new StoreProcedure();
        //                ticketId = store.GenerateTicketId();
        //                leadData.TicketId = ticketId;
        //                ticketIdList.Add(ticketId);

        //                List<ProductData> prodList = SlmScr016Biz.GetProductCampaignDataForSuggestCampaign(cpdata.CampaignId);
        //                if (prodList.Count > 0)
        //                {
        //                    leadData.ProductGroupId = prodList[0].ProductGroupId;
        //                    leadData.ProductId = prodList[0].ProductId;
        //                    leadData.ProductName = prodList[0].ProductName;
        //                }

        //                leadData.CampaignId = cpdata.CampaignId;
        //                cpdata.TicketId = ticketId;

        //                KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
        //                lead.InsertData(leadData, UserId);

        //                KKSlmTrCusInfoModel customerInfo = new KKSlmTrCusInfoModel();
        //                customerInfo.InsertData(leadData, UserId);

        //                KKSlmTrProductInfoModel productInfo = new KKSlmTrProductInfoModel();
        //                productInfo.InsertData(leadData, UserId);

        //                KKSlmTrChannelInfoModel channelInfo = new KKSlmTrChannelInfoModel();
        //                channelInfo.InsertData(leadData, UserId);

        //                KKSLMTrCampaignFinalModel camFinal = new KKSLMTrCampaignFinalModel();
        //                camFinal.InsertData(cpdata, UserId);
        //            }

        //            ts.Complete();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _Error = ex.Message.ToString();
        //        return false;
        //    }
        //}

        public static int? GetProvinceId(string provinceCode)
        {
            KKSlmMsProvinceModel province = new KKSlmMsProvinceModel();
            return province.GetProvinceId(provinceCode);
        }

        public static int? GetAmphurId(string provinceCode, string amphurCode)
        {
            KKSlmMsAmphurModel amphur = new KKSlmMsAmphurModel();
            return amphur.GetAmphurId(provinceCode, amphurCode);
        }

        public static int? GetBrandId(string BrandCode)
        {
            KKSlmMsBrandModel brand = new KKSlmMsBrandModel();
            return brand.GetBrandId(BrandCode);
        }
        public static int? GetModelId(string BrandCode, string Family)
        {
            KKSlmMsModelModel model = new KKSlmMsModelModel();
            return model.GetModelId(BrandCode, Family);
        }
        public static string GetChannelId(string ChannelDesc)
        {
            KKSlmMsChannelModel channel = new KKSlmMsChannelModel();
            return channel.GetChannelId(ChannelDesc);
        }

        public List<SaveResultData> InsertNewLeads(string ticketId, List<ProductData> productList, string username, string staffNameTH, string channelId, string channelDesc)
        {
            List<SaveResultData> resultList = new List<SaveResultData>();
            DateTime createdDate = DateTime.Now;

            string jobOnHand = "";
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            StaffAmountJobOnHand tmp = staff.GetAmountJobOnHand(username);
            if (tmp != null)
            {
                jobOnHand = " (" + (tmp.AmountOwner + tmp.AmountDelegate).ToString() + " งาน)";
            }

            foreach (ProductData product in productList)
            {
                try
                {
                    string new_ticketId = "";
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                        {
                            StoreProcedure store = new StoreProcedure();
                            new_ticketId = store.GenerateTicketId();

                            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
                            lead.InsertData(slmdb, ticketId, new_ticketId, product, username, createdDate, channelId);

                            KKSlmTrCusInfoModel customerInfo = new KKSlmTrCusInfoModel();
                            customerInfo.InsertData(slmdb, ticketId, new_ticketId, username, createdDate);

                            KKSlmTrProductInfoModel productInfo = new KKSlmTrProductInfoModel();
                            productInfo.InsertData(slmdb, ticketId, new_ticketId, username, createdDate);

                            KKSlmTrChannelInfoModel channelInfo = new KKSlmTrChannelInfoModel();
                            channelInfo.InsertData(slmdb, ticketId, new_ticketId, username, createdDate, channelId);

                            KKSLMTrCampaignFinalModel camFinal = new KKSLMTrCampaignFinalModel();
                            camFinal.InsertData(slmdb, new_ticketId, product, username, createdDate);

                            //Add RenewInsurance By Pom 24/03/2016
                            KKSlmMsConfigProductScreenModel screen = new KKSlmMsConfigProductScreenModel();
                            string tableName = screen.GetTableName(product.CampaignId, product.ProductId, "I");
                            if (!string.IsNullOrEmpty(tableName))
                            {
                                switch (tableName)
                                {
                                    case "kkslm_tr_renewinsurance":
                                        KKSlmTrRenewinsuranceModel reins = new KKSlmTrRenewinsuranceModel();
                                        reins.InsertData(new_ticketId, username, createdDate);
                                        break;
                                    default:
                                        break;
                                }
                            }

                            KKSlmTrHistoryModel.InsertHistory(slmdb, new_ticketId, SLMConstant.HistoryTypeCode.CreateLead, "", "", username, createdDate);

                            slmdb.SaveChanges();
                            ts.Complete();
                        }
                    }
                    
                    SaveResultData data = new SaveResultData()
                    {
                        TicketId = new_ticketId,
                        CampaignName = product.CampaignName,
                        ChannelDesc = channelDesc,
                        Ownername = staffNameTH + jobOnHand
                    };
                    resultList.Add(data);
                }
                catch (Exception ex)
                {
                    _errList.Add(product.CampaignId, ex.Message);
                }
            }

            return resultList;
        }

        //public static LeadDataPhoneCallHistory GetLeadDataPhoneCallHistoryDefault(string ticketId)
        //{
        //    KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
        //    return lead.GetLeadDataPhoneCallHistoryDefault(ticketId);
        //}

        public static LeadDataPhoneCallHistory GetLeadDataPhoneCallHistory(string ticketId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetLeadDataPhoneCallHistory(ticketId);
        }

        public static string GetAssignedFlag(string ticketId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetAssignFlag(ticketId);
        }

        public static List<string> GetAssignedFlagAndDelegateFlag(string ticketId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetAssignedFlagAndDelegateFlag(ticketId);
        }

        public static bool CheckRequireCardId(string statusCode)
        {
            KKSlmMsConfigCloseJobModel config = new KKSlmMsConfigCloseJobModel();
            return config.CheckRequireCardId(statusCode);
        }

        public string[] GetProductCampaign(string ticketId)
        {
            return new KKSlmTrLeadModel().GetProductCampaign(ticketId);
        }

        //Added by Pom 22/04/2016
        public CustomerData GetCustomerData(string ticketId)
        {
            return new KKSlmTrLeadModel().GetCustomerData(ticketId);
        }

        public List<LeadData> GetLeadOnHandList(string username)
        {
            return new KKSlmTrLeadModel().GetLeadOnHandList(username);
        }

        public static List<ControlListData> getStatusLead(string CampaignId, string ProductId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.getStatusLead(CampaignId, ProductId);
        }

        public static List<ControlListData> getSubStatusLead(string CampaignId, string ProductId, string LeadStatus)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.getSubStatusLead(CampaignId, ProductId, LeadStatus);
        }


        public static bool checkBlackList(string username)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.checkBlackList(username);
        }

        // renew insurance

        public partial class LeadDetailInsData : kkslm_tr_renewinsurance
        {
            public bool IsCustomer { get; set; }
            public string CitizenId { get; set; }
            public string CardType { get; set; }
            public int? CountryId { get; set; }
            public string MaritalStatus { get; set; }
            public string Occupation { get; set; }
            public DateTime Birthdate { get; set; }
            public string ProvinceRegis { get; set; }
        }
        public partial class LeadDetailAddress : kkslm_tr_renewinsurance_address
        {

        }

        public bool SaveInsData(LeadData ldata, LeadDetailInsData ins, List<LeadDetailAddress> addrLst, string userID, out string TicketID) { return SaveInsData(ldata, ins, addrLst, userID, out TicketID, 0);  }
        public bool SaveInsData(LeadData ldata, LeadDetailInsData ins, List<LeadDetailAddress> addrLst, string userID, out string TicketID, int ctType)
        {
            TicketID = "";
            bool ret = true;
            bool isnew = false;
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                    {
                        //bool isnew = false;   //Comment By Pom 17/06/2016 - warning, variable is never used
                        var cmp = sdc.kkslm_ms_campaign.Where(c => c.slm_CampaignId == ldata.CampaignId && c.is_Deleted == 0).FirstOrDefault();
                        var curdate = DateTime.Now;

                        var createbyStaff = sdc.kkslm_ms_staff.Where(p => p.slm_UserName == userID && p.is_Deleted == 0).FirstOrDefault();

                        // lead data
                        #region lead and etc
                        var lead = sdc.kkslm_tr_lead.Where(l => l.slm_ticketId == ldata.TicketId).FirstOrDefault();
                        if (lead == null)
                        {
                            isnew = true;         //Comment By Pom 17/06/2016 - warning, variable is never used
                            lead = new kkslm_tr_lead();
                            StoreProcedure store = new StoreProcedure();
                            lead.slm_ticketId = store.GenerateTicketId();
                            lead.slm_SubStatus = ldata.slm_SubStatus;
                            lead.slm_CreatedBy = userID;
                            lead.slm_CreatedDate = DateTime.Now;
                            lead.slm_Status = SLMConstant.StatusCode.Interest;
                            lead.slm_StatusDate = DateTime.Now;
                            lead.slm_StatusDateSource = DateTime.Now;
                            lead.slm_StatusBy = userID;
                            lead.slm_Delegate_Flag = 0;
                            lead.slm_AssignedFlag = "0";
                            lead.slm_ThisWork = 0;
                            lead.slm_TotalAlert = 0;
                            lead.slm_TotalWork = 0;
                            lead.slm_Counting = 0;
                            lead.slm_CreatedBy_Branch = ldata.CreatedBy_Branch;

                            // find product for lead data
                            List<ProductData> prodList = new CmtCampaignProductModel().GetProductCampaignData(ldata.CampaignId);
                            if (prodList.Count > 0)
                            {
                                lead.slm_Product_Group_Id = prodList[0].ProductGroupId;
                                lead.slm_Product_Id = prodList[0].ProductId;
                                lead.slm_Product_Name = prodList[0].ProductName;
                                ldata.HasAdamsUrl = prodList[0].HasAdamsUrl;        //Added by Pom 12/07/2016
                                                                                    //lead.HasAdamsUrl = prodList[0].HasAdamsUrl;
                            }

                            //Added by Pom 12/07/2016
                            var subStatusList = new KKSlmMsConfigProductSubstatusModel().GetSubStatusList(lead.slm_Product_Id, ldata.CampaignId, lead.slm_Status);
                            lead.slm_ExternalSubStatusDesc = subStatusList.Where(p => p.ValueField == ldata.slm_SubStatus).Select(p => p.TextField).FirstOrDefault();

                            sdc.kkslm_tr_lead.AddObject(lead);

                            // insert campaign final
                            kkslm_tr_campaignfinal cmf = new kkslm_tr_campaignfinal();
                            cmf.slm_TicketId = lead.slm_ticketId;
                            cmf.slm_CampaignId = ldata.CampaignId;
                            cmf.slm_CampaignName = cmp == null ? ldata.CampaignName : cmp.slm_CampaignName;
                            cmf.slm_Description = cmp == null ? "" : cmp.slm_Desc_For_Customer;
                            cmf.slm_CreatedBy = userID;
                            cmf.slm_CreatedBy_Position = new KKSlmMsStaffModel().GetPositionId(userID);
                            cmf.slm_CreatedDate = curdate;
                            cmf.slm_UpdatedBy = userID;
                            cmf.slm_UpdatedDate = curdate;
                            cmf.is_Deleted = 0;

                            sdc.kkslm_tr_campaignfinal.AddObject(cmf);


                            // insert tr history
                            kkslm_tr_history his = new kkslm_tr_history();
                            his.slm_ticketId = lead.slm_ticketId;
                            his.slm_History_Type_Code = SLMConstant.HistoryTypeCode.CreateLead;
                            his.slm_CreatedBy = userID;
                            his.slm_CreatedBy_Position = createbyStaff != null ? createbyStaff.slm_Position_id : null;
                            his.slm_CreateBy_Branch = createbyStaff != null ? createbyStaff.slm_BranchCode : null;
                            his.slm_CreatedDate = curdate;
                            his.slm_UpdatedBy = userID;
                            his.slm_UpdatedDate = curdate;
                            his.is_Deleted = false;

                            sdc.kkslm_tr_history.AddObject(his);
                        }

                        lead.slm_TitleId = ldata.TitleId != 0 ? ldata.TitleId : null;
                        lead.slm_Name = ldata.Name;
                        lead.slm_LastName = ldata.LastName;
                        lead.slm_ChannelId = ldata.ChannelId;
                        lead.slm_TelNo_1 = ldata.TelNo_1;
                        lead.slm_CampaignId = ldata.CampaignId;
                        lead.slm_AvailableTime = ldata.AvailableTime;

                        string ticketIdRefer = string.IsNullOrEmpty(lead.slm_TicketIdRefer) ? "" : lead.slm_TicketIdRefer;
                        string newTicketIdRefer = string.IsNullOrEmpty(ldata.TicketIdRefer) ? "" : ldata.TicketIdRefer;
                        if (ticketIdRefer != newTicketIdRefer)
                        {
                            kkslm_tr_activity act = new kkslm_tr_activity();
                            act.slm_CreatedBy = userID;
                            act.slm_CreatedDate = DateTime.Now;
                            act.slm_UpdatedBy = userID;
                            act.slm_UpdatedDate = DateTime.Now;
                            act.is_Deleted = 0;
                            act.slm_Type = SLMConstant.ActionType.ReferTicket;
                            act.slm_TicketId = lead.slm_ticketId;
                            act.slm_SystemAction = SLMConstant.SystemName;
                            act.slm_SystemActionBy = SLMConstant.SystemName;
                            sdc.kkslm_tr_activity.AddObject(act);

                            lead.slm_TicketIdRefer = ldata.TicketIdRefer;
                        }

                        KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
                        if (!String.IsNullOrEmpty(ldata.Owner_Branch) && !String.IsNullOrEmpty(ldata.Owner))
                        {
                            lead.slm_Owner_Branch = ldata.Owner_Branch;
                            lead.slm_Owner = ldata.Owner;
                            string staffId = staff.GetStaffIdData(ldata.Owner);
                            lead.slm_StaffId = SLMUtil.SafeInt(staffId);
                        }
                        else
                        {
                            lead.slm_StaffId = null;
                            lead.slm_Owner = null;
                            lead.slm_Owner_Branch = null;
                        }

                        lead.slm_UpdatedBy = userID;
                        lead.slm_UpdatedDate = DateTime.Now;
                        lead.slm_CreatedBy_Position = staff.GetPositionId(userID);

                        if (ldata.HasNewDelegate)
                        {
                            lead.slm_Delegate_Flag = ldata.Delegate_Flag.Value;
                            if (string.IsNullOrEmpty(ldata.Delegate))
                                lead.slm_DelegateDate = null;
                            else
                                lead.slm_DelegateDate = DateTime.Now;

                            lead.slm_Delegate = string.IsNullOrEmpty(ldata.Delegate) ? null : ldata.Delegate;
                            lead.slm_Delegate_Position = string.IsNullOrEmpty(ldata.Delegate) ? null : GetPositionId(ldata.Delegate, sdc);

                            if (!string.IsNullOrEmpty(ldata.Delegate_Branch))
                                lead.slm_Delegate_Branch = ldata.Delegate_Branch;
                            else
                                lead.slm_Delegate_Branch = null;

                            KKSlmTrActivityModel Activity = new KKSlmTrActivityModel();
                            Activity.InsertData(ldata, userID, DateTime.Now);
                        }
                        else
                        {
                            if (lead.slm_Delegate == ldata.Delegate)
                            {
                                lead.slm_Delegate = string.IsNullOrEmpty(ldata.Delegate) ? null : ldata.Delegate;
                                lead.slm_Delegate_Position = string.IsNullOrEmpty(ldata.Delegate) ? null : GetPositionId(ldata.Delegate, sdc);

                                if (!string.IsNullOrEmpty(ldata.Delegate_Branch))
                                    lead.slm_Delegate_Branch = ldata.Delegate_Branch;
                                else
                                    lead.slm_Delegate_Branch = null;
                            }
                        }

                        if (ldata.HasNewOwner)
                        {
                            lead.slm_AssignedFlag = "0";
                            lead.slm_AssignedDate = null;
                            lead.slm_AssignedBy = null;

                            if (!string.IsNullOrEmpty(ldata.Owner_Branch))
                                lead.slm_Owner_Branch = ldata.Owner_Branch;
                            else
                                lead.slm_Owner_Branch = null;

                            lead.slm_Owner = ldata.Owner;
                            lead.slm_Owner_Position = GetPositionId(ldata.Owner, sdc);

                            if (isnew == false)
                            {
                                KKSlmTrActivityModel Activity = new KKSlmTrActivityModel();
                                Activity.InsertDataChangeOwner(ldata, userID, DateTime.Now);
                            }
                        }
                        else
                        {
                            if (lead.slm_Owner == ldata.Owner)
                            {
                                if (!string.IsNullOrEmpty(ldata.Owner_Branch))
                                    lead.slm_Owner_Branch = ldata.Owner_Branch;
                                else
                                    lead.slm_Owner_Branch = null;

                                lead.slm_Owner = ldata.Owner;
                                lead.slm_Owner_Position = GetPositionId(ldata.Owner, sdc);
                            }
                        }

                        lead.slm_OldOwner = ldata.slmOldOwner;

                        //if (ldata.StaffId != null)
                        //    lead.slm_StaffId = Convert.ToInt32("0" + ldata.StaffId);
                        //if (ldata.counting != null)
                        //    lead.slm_Counting = Convert.ToDecimal(ldata.counting);

                        TicketID = lead.slm_ticketId;

                        #endregion

                        // customer data
                        #region cus info
                        var cus = sdc.kkslm_tr_cusinfo.Where(c => c.slm_TicketId == lead.slm_ticketId).FirstOrDefault();
                        if (cus == null)
                        {
                            cus = new kkslm_tr_cusinfo();
                            cus.slm_TicketId = lead.slm_ticketId;
                            cus.slm_CreatedBy = userID;
                            cus.slm_CreatedDate = DateTime.Now;
                            cus.slm_TelNoSms = ldata.TelNo_1;
                            sdc.kkslm_tr_cusinfo.AddObject(cus);
                        }

                        cus.slm_LastName = ldata.LastName;
                        cus.slm_TelNo_2 = ldata.TelNo_2;
                        cus.slm_IsCustomer = ldata.IsCustomer;
                        cus.slm_CardType = ldata.CardType;
                        cus.slm_CitizenId = ldata.CitizenId;
                        cus.slm_country_id = ldata.CountryId;
                        cus.slm_Birthdate = ldata.Birthdate;
                        if (!String.IsNullOrEmpty(ins.Occupation)) cus.slm_Occupation = SLMUtil.SafeInt(ins.Occupation);
                        cus.slm_MaritalStatus = ins.MaritalStatus;
                        cus.slm_Detail = ldata.Detail;
                        cus.slm_UpdatedBy = userID;
                        cus.slm_UpdatedDate = DateTime.Now;

                        #endregion

                        // channel data
                        #region channel data
                        var channel = sdc.kkslm_tr_channelinfo.Where(c => c.slm_TicketId == lead.slm_ticketId).FirstOrDefault();
                        if (channel == null)
                        {
                            channel = new kkslm_tr_channelinfo();
                            sdc.kkslm_tr_channelinfo.AddObject(channel);
                            channel.slm_TicketId = lead.slm_ticketId;
                            channel.slm_ChannelId = ldata.ChannelId;
                            channel.slm_IPAddress = ldata.IPAddress;
                            channel.slm_BranchNo = ldata.BranchNo;
                            channel.slm_MachineNo = ldata.MachineNo;
                            channel.slm_ClientServiceType = ldata.ClientServiceType;
                            channel.slm_DocumentNo = ldata.DocumentNo;
                            channel.slm_CommPaidCode = ldata.CommPaidCode;
                            channel.slm_Zone = ldata.Zone;
                            channel.slm_RequestDate = DateTime.Now;
                        }

                        channel.slm_Company = ldata.Company;
                        channel.slm_Branch = ldata.Branch;
                        channel.slm_RequestBy = userID;

                        #endregion

                        // productinfo data
                        #region product info
                        var prodInfo = sdc.kkslm_tr_productinfo.Where(p => p.slm_TicketId == lead.slm_ticketId).FirstOrDefault();
                        if (prodInfo == null)
                        {
                            prodInfo = new kkslm_tr_productinfo();
                            sdc.kkslm_tr_productinfo.AddObject(prodInfo);
                            prodInfo.slm_TicketId = lead.slm_ticketId;
                            prodInfo.slm_OtherDetail_1 = ldata.OtherDetail_1;
                            prodInfo.slm_OtherDetail_2 = ldata.OtherDetail_2;
                            prodInfo.slm_OtherDetail_3 = ldata.OtherDetail_3;
                            prodInfo.slm_OtherDetail_4 = ldata.OtherDetail_4;
                            prodInfo.is_Deleted = 0;
                        }

                        prodInfo.slm_InterestedProd = ldata.InterestedProd;
                        prodInfo.slm_LicenseNo = ins.slm_LicenseNo;
                        prodInfo.slm_YearOfCar = ldata.YearOfCar;
                        prodInfo.slm_YearOfCarRegis = ldata.YearOfCarRegis;
                        prodInfo.slm_ProvinceRegis = ldata.ProvinceRegis;

                        prodInfo.slm_Brand = ldata.Brand;
                        prodInfo.slm_Model = ldata.Model;
                        prodInfo.slm_Submodel = ldata.Submodel;

                        prodInfo.slm_RedbookBrandCode = ins.slm_RedbookBrandCode;
                        prodInfo.slm_RedbookModelCode = ins.slm_RedbookModelCode;
                        prodInfo.slm_RedbookKKKey = ins.slm_RedbookKKKey;
                        prodInfo.slm_RedbookYearGroup = ins.slm_RedbookYearGroup;

                        prodInfo.slm_DownPayment = ldata.DownPayment;
                        prodInfo.slm_DownPercent = ldata.DownPercent;
                        prodInfo.slm_CarPrice = ldata.CarPrice;
                        prodInfo.slm_FinanceAmt = ldata.FinanceAmt;
                        prodInfo.slm_PaymentTerm = ldata.PaymentTerm;
                        prodInfo.slm_PaymentType = ldata.PaymentType;
                        prodInfo.slm_BalloonAmt = ldata.BalloonAmt;
                        prodInfo.slm_BalloonPercent = ldata.BalloonPercent;
                        prodInfo.slm_PlanType = ldata.PlanType;
                        prodInfo.slm_CoverageDate = ldata.CoverageDate;
                        prodInfo.slm_AccType = ldata.AccType;
                        prodInfo.slm_AccPromotion = ldata.AccPromotion;
                        prodInfo.slm_AccTerm = ldata.AccTerm;
                        prodInfo.slm_Interest = ldata.Interest;
                        prodInfo.slm_Invest = ldata.Invest;
                        prodInfo.slm_LoanOd = ldata.LoanOd;
                        prodInfo.slm_LoanOdTerm = ldata.LoanOdTerm;
                        prodInfo.slm_Ebank = ldata.Ebank;
                        prodInfo.slm_Atm = ldata.Atm;
                        if (ldata.CarType != null) prodInfo.slm_CarType = ldata.CarType; // 2017-03-21
                                                                                         //prodInfo.slm_CarType = ins.slm_CoverageTypeId.ToString(); zz 2017-03-21

                        #endregion

                        // renewins data
                        #region Renewinsurance
                        var insu = sdc.kkslm_tr_renewinsurance.Where(r => r.slm_RenewInsureId == ins.slm_RenewInsureId).FirstOrDefault();
                        if (insu == null)
                        {
                            insu = new kkslm_tr_renewinsurance();
                            insu.slm_TicketId = lead.slm_ticketId;
                            //insu.slm_ObtPro08Flag = false;
                            insu.slm_PolicyNo = ins.slm_PolicyNo;
                            insu.slm_PolicySaleType = ins.slm_PolicySaleType;
                            insu.slm_PolicyStartCoverDate = ins.slm_PolicyStartCoverDate;
                            insu.slm_PolicyEndCoverDate = ins.slm_PolicyEndCoverDate;
                            insu.slm_PolicyIssuePlace = ins.slm_PolicyIssuePlace;
                            insu.slm_PolicyIssueBranch = ins.slm_PolicyIssueBranch;

                            insu.slm_ActPurchaseFlag = ins.slm_ActPurchaseFlag;
                            insu.slm_ActNo = ins.slm_ActNo;
                            insu.slm_ActStartCoverDate = ins.slm_ActStartCoverDate;
                            insu.slm_ActEndCoverDate = ins.slm_ActEndCoverDate;
                            insu.slm_ActIssuePlace = ins.slm_ActIssuePlace;
                            insu.slm_ActIssueBranch = ins.slm_ActIssueBranch;

                            insu.slm_ContractNo = ins.slm_ContractNo;
                            insu.slm_CC = ins.slm_CC;
                            insu.slm_RedbookBrandCode = ins.slm_RedbookBrandCode;
                            insu.slm_RedbookModelCode = ins.slm_RedbookModelCode;
                            insu.slm_RedbookYearGroup = ins.slm_RedbookYearGroup;
                            insu.slm_RedbookKKKey = ins.slm_RedbookKKKey;
                            insu.slm_InsurancecarTypeId = ins.slm_InsurancecarTypeId;
                            //insu.slm_CoverageTypeId = ins.slm_CoverageTypeId;
                            insu.slm_LicenseNo = ins.slm_LicenseNo;
                            insu.slm_ChassisNo = ins.slm_ChassisNo;
                            insu.slm_EngineNo = ins.slm_EngineNo;
                            insu.slm_SendDocFlag = ins.slm_SendDocFlag;
                            insu.slm_SendDocBrandCode = ins.slm_SendDocBrandCode;

                            // default
                            insu.slm_PolicyDiscountAmt = ins.slm_PolicyDiscountAmt;
                            insu.slm_PolicyGrossVat = ins.slm_PolicyGrossVat;
                            insu.slm_PolicyGrossStamp = ins.slm_PolicyGrossStamp;
                            insu.slm_PolicyGrossPremium = ins.slm_PolicyGrossPremium;
                            insu.slm_PolicyGrossPremiumTotal = ins.slm_PolicyGrossPremiumTotal;
                            insu.slm_ActGrossPremium = ins.slm_ActGrossPremium;
                            insu.slm_ActComId = ins.slm_ActComId;
                            insu.slm_ActNetPremium = ins.slm_ActNetPremium;
                            insu.slm_ActVat = ins.slm_ActVat;
                            insu.slm_ActStamp = ins.slm_ActStamp;

                            if (ctType != 2)    //ถ้าไม่ใช่ พรบ.เดี่ยว
                            {
                                insu.slm_PeriodMonth = DateTime.Now.Month.ToString("00");
                                insu.slm_PeriodYear = DateTime.Now.Year.ToString();
                                insu.slm_BeneficiaryId = 1;
                            }

                            insu.slm_UpdatedBy = userID;
                            insu.slm_UpdatedDate = DateTime.Now;
                            insu.slm_CreatedBy = userID;
                            insu.slm_CreatedDate = DateTime.Now;

                            sdc.kkslm_tr_renewinsurance.AddObject(insu);

                            if (!string.IsNullOrWhiteSpace(ins.slm_ContractNo))
                            {
                                // insert tr history
                                kkslm_tr_history his = new kkslm_tr_history();
                                his.slm_ticketId = lead.slm_ticketId;
                                his.slm_History_Type_Code = SLMConstant.HistoryTypeCode.UpdateContractNumber;
                                his.slm_NewValue = ins.slm_ContractNo;
                                his.slm_CreatedBy = userID;
                                his.slm_CreatedBy_Position = createbyStaff != null ? createbyStaff.slm_Position_id : null;
                                his.slm_CreateBy_Branch = createbyStaff != null ? createbyStaff.slm_BranchCode : null;
                                his.slm_CreatedDate = curdate;
                                his.slm_UpdatedBy = userID;
                                his.slm_UpdatedDate = curdate;
                                his.is_Deleted = false;

                                sdc.kkslm_tr_history.AddObject(his);
                            }
                        }
                        else
                        {
                            if (ctType == 3)        //KK MotorBox
                            {
                                //Change PolicyNo, Insert to history
                                if (insu.slm_PolicyNo != ins.slm_PolicyNo)
                                {
                                    // insert tr history
                                    kkslm_tr_history hisPolicyNo = new kkslm_tr_history
                                    {
                                        slm_ticketId = lead.slm_ticketId,
                                        slm_History_Type_Code = SLMConstant.HistoryTypeCode.UpdatePolicyNo,
                                        slm_OldValue = insu.slm_PolicyNo,
                                        slm_NewValue = ins.slm_PolicyNo,
                                        slm_CreatedBy = userID,
                                        slm_CreatedBy_Position = createbyStaff != null ? createbyStaff.slm_Position_id : null,
                                        slm_CreateBy_Branch = createbyStaff != null ? createbyStaff.slm_BranchCode : null,
                                        slm_CreatedDate = curdate,
                                        slm_UpdatedBy = userID,
                                        slm_UpdatedDate = curdate,
                                        is_Deleted = false
                                    };
                                    sdc.kkslm_tr_history.AddObject(hisPolicyNo);
                                }

                                insu.slm_PolicyNo = ins.slm_PolicyNo;
                                insu.slm_PolicySaleType = ins.slm_PolicySaleType;
                                insu.slm_PolicyStartCoverDate = ins.slm_PolicyStartCoverDate;
                                insu.slm_PolicyEndCoverDate = ins.slm_PolicyEndCoverDate;
                                insu.slm_PolicyIssuePlace = ins.slm_PolicyIssuePlace;
                                insu.slm_PolicyIssueBranch = ins.slm_PolicyIssueBranch;
                            }
                            if (ctType == 2 || ctType == 3)     //Act, KK MotorBox
                            {
                                //Changed ActNo, Insert to history
                                if (insu.slm_ActNo != ins.slm_ActNo)
                                {
                                    // insert tr history
                                    kkslm_tr_history hisAct = new kkslm_tr_history
                                    {
                                        slm_ticketId = lead.slm_ticketId,
                                        slm_History_Type_Code = SLMConstant.HistoryTypeCode.UpdateActNo,
                                        slm_OldValue = insu.slm_ActNo,
                                        slm_NewValue = ins.slm_ActNo,
                                        slm_CreatedBy = userID,
                                        slm_CreatedBy_Position = createbyStaff != null ? createbyStaff.slm_Position_id : null,
                                        slm_CreateBy_Branch = createbyStaff != null ? createbyStaff.slm_BranchCode : null,
                                        slm_CreatedDate = curdate,
                                        slm_UpdatedBy = userID,
                                        slm_UpdatedDate = curdate,
                                        is_Deleted = false
                                    };
                                    sdc.kkslm_tr_history.AddObject(hisAct);
                                }

                                insu.slm_ActPurchaseFlag = ins.slm_ActPurchaseFlag;
                                insu.slm_ActNo = ins.slm_ActNo;
                                insu.slm_ActStartCoverDate = ins.slm_ActStartCoverDate;
                                insu.slm_ActEndCoverDate = ins.slm_ActEndCoverDate;
                                insu.slm_ActIssuePlace = ins.slm_ActIssuePlace;
                                insu.slm_ActIssueBranch = ins.slm_ActIssueBranch;
                            }

                            //Changed ContractNo, Insert to history
                            if (insu.slm_ContractNo != ins.slm_ContractNo)
                            {
                                // insert tr history
                                kkslm_tr_history his = new kkslm_tr_history
                                {
                                    slm_ticketId = lead.slm_ticketId,
                                    slm_History_Type_Code = SLMConstant.HistoryTypeCode.UpdateContractNumber,
                                    slm_OldValue = insu.slm_ContractNo,
                                    slm_NewValue = ins.slm_ContractNo,
                                    slm_CreatedBy = userID,
                                    slm_CreatedBy_Position = createbyStaff != null ? createbyStaff.slm_Position_id : null,
                                    slm_CreateBy_Branch = createbyStaff != null ? createbyStaff.slm_BranchCode : null,
                                    slm_CreatedDate = curdate,
                                    slm_UpdatedBy = userID,
                                    slm_UpdatedDate = curdate,
                                    is_Deleted = false
                                };
                                sdc.kkslm_tr_history.AddObject(his);
                            }
                            //Change InsuranceCarType, Insert to history
                            if (insu.slm_InsurancecarTypeId != ins.slm_InsurancecarTypeId)
                            {
                                // insert tr history
                                kkslm_tr_history hisInsCarType = new kkslm_tr_history
                                {
                                    slm_ticketId = lead.slm_ticketId,
                                    slm_History_Type_Code = SLMConstant.HistoryTypeCode.UpdateInsuranceCarType,
                                    slm_OldValue = insu.slm_InsurancecarTypeId != null ? insu.slm_InsurancecarTypeId.Value.ToString() : null,
                                    slm_NewValue = ins.slm_InsurancecarTypeId != null ? ins.slm_InsurancecarTypeId.Value.ToString() : null,
                                    slm_CreatedBy = userID,
                                    slm_CreatedBy_Position = createbyStaff != null ? createbyStaff.slm_Position_id : null,
                                    slm_CreateBy_Branch = createbyStaff != null ? createbyStaff.slm_BranchCode : null,
                                    slm_CreatedDate = curdate,
                                    slm_UpdatedBy = userID,
                                    slm_UpdatedDate = curdate,
                                    is_Deleted = false
                                };
                                sdc.kkslm_tr_history.AddObject(hisInsCarType);
                            }

                            insu.slm_ContractNo = ins.slm_ContractNo;
                            insu.slm_CC = ins.slm_CC;
                            insu.slm_RedbookBrandCode = ins.slm_RedbookBrandCode;
                            insu.slm_RedbookModelCode = ins.slm_RedbookModelCode;
                            insu.slm_RedbookYearGroup = ins.slm_RedbookYearGroup;
                            insu.slm_RedbookKKKey = ins.slm_RedbookKKKey;
                            insu.slm_InsurancecarTypeId = ins.slm_InsurancecarTypeId;
                            //insu.slm_CoverageTypeId = ins.slm_CoverageTypeId;
                            insu.slm_LicenseNo = ins.slm_LicenseNo;
                            insu.slm_ChassisNo = ins.slm_ChassisNo;
                            insu.slm_EngineNo = ins.slm_EngineNo;
                            insu.slm_SendDocFlag = ins.slm_SendDocFlag;
                            insu.slm_SendDocBrandCode = ins.slm_SendDocBrandCode;

                            // default
                            //insu.slm_PolicyDiscountAmt = ins.slm_PolicyDiscountAmt;
                            //insu.slm_PolicyGrossVat = ins.slm_PolicyGrossVat;
                            //insu.slm_PolicyGrossStamp = ins.slm_PolicyGrossStamp;
                            //insu.slm_PolicyGrossPremium = ins.slm_PolicyGrossPremium;
                            //insu.slm_PolicyGrossPremiumTotal = ins.slm_PolicyGrossPremiumTotal;
                            //insu.slm_ActGrossPremium = ins.slm_ActGrossPremium;
                            //insu.slm_ActComId = ins.slm_ActComId;
                            //insu.slm_ActNetPremium = ins.slm_ActNetPremium;
                            //insu.slm_ActVat = ins.slm_ActVat;
                            //insu.slm_ActStamp = ins.slm_ActStamp;

                            insu.slm_UpdatedBy = userID;
                            insu.slm_UpdatedDate = DateTime.Now;
                        }

                        sdc.SaveChanges();
                        #endregion

                        // renewins address data
                        #region renewinsurance_Address
                        var existaddr = addrLst.Select(a => a.slm_RenewInsureAddressId).ToList();
                        foreach (var adr in addrLst)
                        {
                            var adru = sdc.kkslm_tr_renewinsurance_address.Where(a => a.slm_RenewInsureAddressId == adr.slm_RenewInsureAddressId).FirstOrDefault();
                            if (adru == null)
                            {
                                adru = new kkslm_tr_renewinsurance_address();
                                adru.slm_RenewInsureId = insu.slm_RenewInsureId;
                                sdc.kkslm_tr_renewinsurance_address.AddObject(adru);
                            }

                            if (adr.slm_AddressType == "C")
                            {

                            }

                            adru.slm_AddressType = adr.slm_AddressType;
                            adru.slm_House_No = adr.slm_AddressNo;
                            adru.slm_Village = adr.slm_BuildingName;
                            //adru.slm_AddressNo = adr.slm_AddressNo;  // change 2016-06-18
                            //adru.slm_BuildingName = adr.slm_BuildingName;
                            adru.slm_Moo = adr.slm_Moo; // add 2017-02-24 zz
                            adru.slm_Street = adr.slm_Street;
                            adru.slm_Floor = adr.slm_Floor;
                            adru.slm_Soi = adr.slm_Soi;
                            adru.slm_Province = adr.slm_Province;
                            adru.slm_Amphur = adr.slm_Amphur;
                            adru.slm_Tambon = adr.slm_Tambon;
                            adru.slm_PostalCode = adr.slm_PostalCode;
                        }

                        var adrDel = sdc.kkslm_tr_renewinsurance_address.Where(a => a.slm_RenewInsureId == insu.slm_RenewInsureId && !existaddr.Contains(a.slm_RenewInsureAddressId)).ToList();
                        foreach (var itm in adrDel)
                            sdc.kkslm_tr_renewinsurance_address.DeleteObject(itm);

                        sdc.SaveChanges();
                        #endregion

                        // post work
                        // ctType 1 = renew , 2 = act, 3 = kk motorbox

                        #region Compare Act
                        if (ctType == 2 || (ctType == 3 && ins.slm_ActPurchaseFlag == true))
                        {
                            // seq 1
                            var ca1 = sdc.kkslm_tr_renewinsurance_compare_act.Where(c => c.slm_RenewInsureId == insu.slm_RenewInsureId && c.slm_Seq == 1).FirstOrDefault();
                            if (ca1 == null)
                            {
                                ca1 = new kkslm_tr_renewinsurance_compare_act()
                                {
                                    slm_RenewInsureId = insu.slm_RenewInsureId,
                                    slm_PromotionId = null,
                                    slm_Seq = 1,
                                    slm_Year = "",
                                    slm_Ins_Com_Id = 0,
                                    slm_ActIssuePlace = ins.slm_ActIssuePlace,
                                    slm_SendDocType = null,
                                    slm_ActNo = null,
                                    slm_ActStartCoverDate = ins.slm_ActStartCoverDate,
                                    slm_ActEndCoverDate = ins.slm_ActEndCoverDate,
                                    slm_ActIssueBranch = ins.slm_ActIssueBranch,
                                    slm_CarTaxExpiredDate = null,
                                    slm_ActGrossStamp = 0,
                                    slm_ActGrossVat = 0,
                                    slm_ActGrossPremium = 0,
                                    slm_ActNetGrossPremium = 0,
                                    slm_ActGrossPremiumPay = 0,
                                    slm_CreatedDate = DateTime.Now,
                                    slm_CreatedBy = userID,
                                    slm_UpdatedDate = DateTime.Now,
                                    slm_UpdatedBy = userID,
                                    slm_ActPurchaseFlag = false,
                                    slm_DiscountPercent = 0,
                                    slm_DiscountBath = 0,
                                    slm_ActSignNo = null,
                                    slm_ActNetGrossPremiumFull = 0,
                                    slm_Vat1Percent = null,
                                    slm_Vat1PercentBath = 0
                                };
                                sdc.kkslm_tr_renewinsurance_compare_act.AddObject(ca1);
                            }

                            // seq 2
                            var ca2 = sdc.kkslm_tr_renewinsurance_compare_act.Where(c => c.slm_RenewInsureId == insu.slm_RenewInsureId && c.slm_Seq == 2).FirstOrDefault();
                            if (ca2 == null)
                            {
                                ca2 = new kkslm_tr_renewinsurance_compare_act()
                                {
                                    slm_RenewInsureId = insu.slm_RenewInsureId,
                                    slm_PromotionId = 0,
                                    slm_Seq = 2,
                                    slm_Year = "",
                                    slm_Ins_Com_Id = 0,
                                    slm_SendDocType = null,
                                    slm_CarTaxExpiredDate = null,
                                    slm_ActGrossStamp = 0,
                                    slm_ActGrossVat = 0,
                                    slm_ActGrossPremium = 0,
                                    slm_ActNetGrossPremium = 0,
                                    slm_ActGrossPremiumPay = 0,
                                    slm_CreatedDate = DateTime.Now,
                                    slm_CreatedBy = userID,
                                    slm_UpdatedDate = DateTime.Now,
                                    slm_UpdatedBy = userID,
                                    slm_ActPurchaseFlag = true,
                                    slm_DiscountPercent = 0,
                                    slm_DiscountBath = 0,
                                    slm_ActSignNo = null,
                                    slm_ActNetGrossPremiumFull = 0,
                                    slm_Vat1Percent = null,
                                    slm_Vat1PercentBath = 0
                                };
                                sdc.kkslm_tr_renewinsurance_compare_act.AddObject(ca2);
                            }

                            ca2.slm_ActIssuePlace = ins.slm_ActIssuePlace;
                            ca2.slm_ActNo = ins.slm_ActIssueBranch;
                            ca2.slm_ActStartCoverDate = ins.slm_ActStartCoverDate;
                            ca2.slm_ActEndCoverDate = ins.slm_ActEndCoverDate;
                            ca2.slm_ActIssueBranch = ins.slm_ActIssueBranch;
                            ca2.slm_UpdatedDate = DateTime.Now;
                            ca2.slm_UpdatedBy = userID;

                            sdc.SaveChanges();

                        }
                        #endregion

                        #region Type3 KKMotor Box
                        if (ctType == 3)
                        {
                            // rewnew compare seq 1
                            var rc1 = sdc.kkslm_tr_renewinsurance_compare.Where(c => c.slm_RenewInsureId == insu.slm_RenewInsureId && c.slm_Seq == 1).FirstOrDefault();
                            if (rc1 == null)
                            {
                                // new 
                                rc1 = new kkslm_tr_renewinsurance_compare();
                                rc1.slm_RenewInsureId = insu.slm_RenewInsureId;
                                rc1.slm_Seq = 1;
                                rc1.slm_CreatedDate = DateTime.Now;
                                rc1.slm_CreatedBy = userID;

                                sdc.kkslm_tr_renewinsurance_compare.AddObject(rc1);
                            }

                            var pr = sdc.kkslm_tr_prelead.Where(p => p.slm_Contract_Number == ins.slm_ContractNo && p.slm_AssignFlag == "1" && p.slm_Assign_Status == "1" && p.is_Deleted == false).OrderByDescending(p => p.slm_CreatedDate).FirstOrDefault();
                            bool nopr = (pr == null);
                            rc1.slm_Ins_Com_Id = nopr ? 0 : SLMUtil.SafeDecimal(pr.slm_Compulsory_Company_Code);
                            rc1.slm_CoverageTypeId = nopr ? 0 : SLMUtil.SafeInt(pr.slm_Voluntary_Type_Key);
                            rc1.slm_PolicyGrossPremium = nopr ? 0 : pr.slm_Voluntary_Gross_Premium;
                            rc1.slm_NetGrossPremium = nopr ? 0 : pr.slm_Voluntary_Gross_Premium;
                            rc1.slm_PolicyGrossPremiumPay = nopr ? 0 : pr.slm_Voluntary_Gross_Premium;
                            rc1.slm_CostSave = 0;
                            rc1.slm_OldPolicyNo = nopr ? null : pr.slm_Voluntary_Policy_Number;
                            rc1.slm_DriverFlag = nopr ? "0" : (String.IsNullOrEmpty(pr.slm_Driver_First_Name1) && String.IsNullOrEmpty(pr.slm_Driver_First_Name2) ? "0" : "1");
                            rc1.slm_Driver_TitleId1 = nopr ? 0 : pr.slm_Driver_TitleId1;
                            rc1.slm_Driver_First_Name1 = nopr ? null : pr.slm_Driver_First_Name1;
                            rc1.slm_Driver_Last_Name1 = nopr ? null : pr.slm_Driver_Last_Name1;
                            rc1.slm_Driver_Birthdate1 = nopr ? null : pr.slm_Driver_Birthdate1;
                            rc1.slm_Driver_TitleId2 = nopr ? 0 : pr.slm_Driver_TitleId2;
                            rc1.slm_Driver_First_Name2 = nopr ? null : pr.slm_Driver_First_Name2;
                            rc1.slm_Driver_Last_Name2 = nopr ? null : pr.slm_Driver_Last_Name2;
                            rc1.slm_Driver_Birthdate2 = nopr ? null : pr.slm_Driver_Birthdate2;
                            rc1.slm_PolicyStartCoverDate = nopr ? null : pr.slm_Voluntary_Policy_Eff_Date;
                            rc1.slm_PolicyEndCoverDate = nopr ? null : pr.slm_Voluntary_Policy_Exp_Date;
                            rc1.slm_Year = nopr ? null : pr.slm_Voluntary_Policy_Eff_Date == null ? null : (pr.slm_Voluntary_Policy_Eff_Date.Value.Year + 543).ToString();
                            rc1.slm_PolicyGrossStamp = 0;
                            rc1.slm_PolicyGrossVat = 0;
                            rc1.slm_Selected = false;
                            rc1.slm_UpdatedDate = DateTime.Now;
                            rc1.slm_UpdatedBy = userID;


                            // renew compare seq 2
                            var rc2 = sdc.kkslm_tr_renewinsurance_compare.Where(c => c.slm_RenewInsureId == insu.slm_RenewInsureId && c.slm_Seq == 2).FirstOrDefault();
                            if (rc2 == null)
                            {
                                // new
                                rc2 = new kkslm_tr_renewinsurance_compare();
                                rc2.slm_RenewInsureId = insu.slm_RenewInsureId;
                                rc2.slm_Seq = 2;
                                rc2.slm_CreatedDate = DateTime.Now;
                                rc2.slm_CreatedBy = userID;

                                sdc.kkslm_tr_renewinsurance_compare.AddObject(rc2);
                            }

                            var np = sdc.kkslm_tr_notify_premium.Where(p => p.slm_VIN == ins.slm_ChassisNo).OrderByDescending(p => p.slm_CreatedDate).FirstOrDefault();
                            var nonp = (np == null);
                            rc2.slm_NotifyPremiumId = nonp ? 0 : np.slm_Id;         //Added by Pom 14/07/2016
                            rc2.slm_Ins_Com_Id = nonp ? 0 : np.slm_InsuranceComId == null ? 0 : np.slm_InsuranceComId.Value;
                            rc2.slm_CoverageTypeId = nonp ? 0 : np.slm_InsuranceCarTypeId == null ? 0 : np.slm_InsuranceCarTypeId.Value;
                            rc2.slm_RepairTypeId = nonp ? null : np.slm_RepairTypeId;
                            rc2.slm_OD = nonp ? null : np.slm_Sum_Insure;
                            rc2.slm_PolicyGrossStamp = nonp ? null : np.slm_Stamp;
                            rc2.slm_PolicyGrossVat = nonp ? null : np.slm_Vat_Amount;
                            rc2.slm_PolicyGrossPremium = nonp ? null : np.slm_NetPremium;
                            rc2.slm_NetGrossPremium = nonp ? null : np.slm_GrossPremium;
                            rc2.slm_PolicyGrossPremiumPay = nonp ? null : np.slm_GrossPremium;
                            rc2.slm_Selected = false;
                            rc2.slm_OldPolicyNo = nonp ? null : np.slm_PolicyNo;
                            rc2.slm_Year = nonp ? null : np.slm_InsExpireDate == null ? null : (np.slm_InsExpireDate.Value.Year + 543).ToString();
                            rc2.slm_UpdatedDate = DateTime.Now;
                            rc2.slm_UpdatedBy = userID;


                            // renew compare seq 3
                            var rc3 = sdc.kkslm_tr_renewinsurance_compare.Where(c => c.slm_RenewInsureId == insu.slm_RenewInsureId && c.slm_Seq == 3).FirstOrDefault();
                            if (rc3 == null)
                            {
                                // new
                                rc3 = new kkslm_tr_renewinsurance_compare();
                                rc3.slm_RenewInsureId = insu.slm_RenewInsureId;
                                rc3.slm_Seq = 3;
                                rc3.slm_CreatedDate = DateTime.Now;
                                rc3.slm_CreatedBy = userID;

                                sdc.kkslm_tr_renewinsurance_compare.AddObject(rc3);
                            }

                            rc3.slm_PromotionId = 0;        //Added by Pom 14/07/2016
                            rc3.slm_Selected = true;
                            rc3.slm_PolicyStartCoverDate = ins.slm_PolicyStartCoverDate;
                            rc3.slm_PolicyEndCoverDate = ins.slm_PolicyEndCoverDate;
                            rc3.slm_Year = (DateTime.Now.Year + 543).ToString();
                            rc3.slm_UpdatedDate = DateTime.Now;
                            rc3.slm_UpdatedBy = userID;


                            // save change
                            sdc.SaveChanges();

                        }
                        #endregion
                    }

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                _Error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ret = false;
            }
            return ret;
        }

        private int? GetPositionId(string username, SLM_DBEntities slmdb)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetPositionId(username);
        }

        public bool GetInsDetail(string ticketID, out LeadDetailInsData ins, out List<LeadDetailAddress> addrLst, string userID)
        {
            ins = null;
            addrLst = new List<LeadDetailAddress>();

            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    var cus = sdc.kkslm_tr_cusinfo.Where(c => c.slm_TicketId == ticketID).FirstOrDefault();
                    if (cus == null) throw new Exception("Cusinfo not found");

                    var prod = sdc.kkslm_tr_productinfo.Where(p => p.slm_TicketId == ticketID).FirstOrDefault();
                    if (prod == null) throw new Exception("ProductInfo not found");

                    var insu = sdc.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == ticketID).FirstOrDefault();
                    if (insu == null)
                    {
                        // create ins from productinfo

                        //var pd = sdc.kkslm_tr_productinfo.Where(p => p.slm_TicketId == ticketID).FirstOrDefault();
                        //if (pd != null)
                        //{
                        //    // copy data from productinfo
                        //    insu = new kkslm_tr_renewinsurance();
                        //    insu.slm_TicketId = pd.slm_TicketId;
                        //    insu.slm_CreatedBy = userID;
                        //    insu.slm_CreatedDate = DateTime.Now;
                        //    insu.slm_UpdatedBy = userID;
                        //    insu.slm_UpdatedDate = DateTime.Now;
                        //    insu.slm_LicenseNo = pd.slm_LicenseNo;
                        //    insu.slm_RedbookYearGroup = SLMUtil.SafeInt(pd.slm_YearOfCar);
                            

                        //    sdc.kkslm_tr_renewinsurance.AddObject(insu);
                        //    sdc.SaveChanges();
                        //}
                        //else
                            throw new Exception("Renewinsurance not found");
                    }

                    ins = new LeadDetailInsData();
                    ins.IsCustomer = cus.slm_IsCustomer == "Y";
                    ins.CitizenId = cus.slm_CitizenId;
                    ins.CardType =  cus.slm_CardType == null? "" : cus.slm_CardType.ToString();
                    ins.CountryId = cus.slm_country_id;
                    ins.MaritalStatus = cus.slm_MaritalStatus;
                    ins.Occupation = cus.slm_Occupation == null ? "" : cus.slm_Occupation.ToString();
                    ins.Birthdate = cus.slm_Birthdate == null ? new DateTime() : cus.slm_Birthdate.Value;
                    ins.ProvinceRegis = prod.slm_ProvinceRegis == null ? null : prod.slm_ProvinceRegis.Value.ToString();

                    ins.slm_RenewInsureId = insu.slm_RenewInsureId;
                    ins.slm_TicketId = insu.slm_TicketId;
                    ins.slm_PolicyNo = insu.slm_PolicyNo;
                    ins.slm_PolicySaleType = insu.slm_PolicySaleType;
                    ins.slm_PolicyStartCoverDate = insu.slm_PolicyStartCoverDate;
                    ins.slm_PolicyEndCoverDate = insu.slm_PolicyEndCoverDate;
                    ins.slm_PolicyIssuePlace = insu.slm_PolicyIssuePlace;
                    ins.slm_PolicyIssueBranch = insu.slm_PolicyIssueBranch;
                    ins.slm_ActPurchaseFlag = insu.slm_ActPurchaseFlag;
                    ins.slm_ActNo = insu.slm_ActNo;
                    ins.slm_ActStartCoverDate = insu.slm_ActStartCoverDate;
                    ins.slm_ActEndCoverDate = insu.slm_ActEndCoverDate;
                    ins.slm_ActIssuePlace = insu.slm_ActIssuePlace;
                    ins.slm_ActIssueBranch = insu.slm_ActIssueBranch;
                    ins.slm_ContractNo = insu.slm_ContractNo;
                    ins.slm_CC = insu.slm_CC;
                    ins.slm_RedbookBrandCode = insu.slm_RedbookBrandCode;
                    ins.slm_RedbookModelCode = insu.slm_RedbookModelCode;
                    ins.slm_RedbookYearGroup = insu.slm_RedbookYearGroup;
                    ins.slm_RedbookKKKey = insu.slm_RedbookKKKey;
                    ins.slm_RedbookBrandCodeExt = insu.slm_RedbookBrandCodeExt;
                    ins.slm_RedbookModelCodeExt = insu.slm_RedbookModelCodeExt;
                    ins.slm_RedbookYearGroupExt = insu.slm_RedbookYearGroupExt;
                    ins.slm_RedbookKKKeyExt = insu.slm_RedbookKKKeyExt;
                    //ins.slm_CoverageTypeId = insu.slm_CoverageTypeId;
                    //if (!string.IsNullOrEmpty(prod.slm_CarType))
                    //    ins.slm_CoverageTypeId = Convert.ToInt32(prod.slm_CarType);
                    ins.slm_InsurancecarTypeId = insu.slm_InsurancecarTypeId; // zz 2017-03-21
                    ins.slm_CoverageTypeId = insu.slm_CoverageTypeId; // zz 2017-03-21
                    
                    ins.slm_LicenseNo = insu.slm_LicenseNo;
                    ins.slm_ChassisNo = insu.slm_ChassisNo;
                    ins.slm_EngineNo = insu.slm_EngineNo;
                    ins.slm_CreatedBy = insu.slm_CreatedBy;
                    ins.slm_CreatedDate = insu.slm_CreatedDate;
                    ins.slm_UpdatedBy = insu.slm_UpdatedBy;
                    ins.slm_UpdatedDate = insu.slm_UpdatedDate;
                    ins.slm_ReceiveNo = insu.slm_ReceiveNo;
                    ins.slm_ReceiveDate = insu.slm_ReceiveDate;
                    ins.slm_InsuranceComId = insu.slm_InsuranceComId;
                    ins.slm_PolicyDiscountAmt = insu.slm_PolicyDiscountAmt;
                    ins.slm_PolicyGrossVat = insu.slm_PolicyGrossVat;
                    ins.slm_PolicyGrossStamp = insu.slm_PolicyGrossStamp;
                    ins.slm_PolicyGrossPremium = insu.slm_PolicyGrossPremium;
                    ins.slm_PolicyGrossPremiumTotal = insu.slm_PolicyGrossPremiumTotal;
                    ins.slm_PolicyNetGrossPremium = insu.slm_PolicyNetGrossPremium;
                    ins.slm_PolicyCost = insu.slm_PolicyCost;
                    ins.slm_RepairTypeId = insu.slm_RepairTypeId;
                    ins.slm_PolicyPayMethodId = insu.slm_PolicyPayMethodId;
                    ins.slm_PolicyAmountPeriod = insu.slm_PolicyAmountPeriod;
                    ins.slm_ActGrossPremium = insu.slm_ActGrossPremium;
                    ins.slm_ActComId = insu.slm_ActComId;
                    ins.slm_ActNetPremium = insu.slm_ActNetPremium;
                    ins.slm_ActVat = insu.slm_ActVat;
                    ins.slm_ActStamp = insu.slm_ActStamp;
                    ins.slm_ActPayMethodId = insu.slm_ActPayMethodId;
                    ins.slm_ActamountPeriod = insu.slm_ActamountPeriod;
                    ins.slm_ObtPro08Flag = insu.slm_ObtPro08Flag;
                    ins.slm_IncentiveFlag = insu.slm_IncentiveFlag;
                    ins.slm_IncentiveDate = insu.slm_IncentiveDate;
                    ins.slm_Need_CreditFlag = insu.slm_Need_CreditFlag;
                    ins.slm_Need_50TawiFlag = insu.slm_Need_50TawiFlag;
                    ins.slm_Need_DriverLicenseFlag = insu.slm_Need_DriverLicenseFlag;
                    ins.slm_CreditFileName = insu.slm_CreditFileName;
                    ins.slm_CreditFilePath = insu.slm_CreditFilePath;
                    ins.slm_CreditFileSize = insu.slm_CreditFileSize;
                    ins.slm_50TawiFileName = insu.slm_50TawiFileName;
                    ins.slm_50TawiFilePath = insu.slm_50TawiFilePath;
                    ins.slm_50TawiFileSize = insu.slm_50TawiFileSize;
                    ins.slm_DriverLicenseiFileName = insu.slm_DriverLicenseiFileName;
                    ins.slm_DriverLicenseFilePath = insu.slm_DriverLicenseFilePath;
                    ins.slm_DriverLicenseFileSize = insu.slm_DriverLicenseFileSize;
                    ins.slm_Need_CompulsoryFlag = insu.slm_Need_CompulsoryFlag;
                    ins.slm_Need_CompulsoryFlagDate = insu.slm_Need_CompulsoryFlagDate;
                    ins.slm_Need_PolicyFlag = insu.slm_Need_PolicyFlag;
                    ins.slm_Need_PolicyFlagDate = insu.slm_Need_PolicyFlagDate;
                    ins.slm_RemarkPayment = insu.slm_RemarkPayment;
                    ins.slm_Voluntary_First_Create_Date = insu.slm_Voluntary_First_Create_Date;
                    ins.slm_PolicyCostSave = insu.slm_PolicyCostSave;
                    ins.slm_Vat1Percent = insu.slm_Vat1Percent;
                    ins.slm_DiscountPercent = insu.slm_DiscountPercent;
                    ins.slm_Vat1PercentBath = insu.slm_Vat1PercentBath;
                    ins.slm_Grade = insu.slm_Grade;
                    ins.slm_SendDocFlag = insu.slm_SendDocFlag;
                    ins.slm_SendDocBrandCode = insu.slm_SendDocBrandCode;
                    ins.slm_Receiver = insu.slm_Receiver;
                    ins.slm_PayOptionId = insu.slm_PayOptionId;
                    ins.slm_PayBranchCode = insu.slm_PayBranchCode;
                    ins.slm_RemarkPolicy = insu.slm_RemarkPolicy;
                    ins.slm_RemarkAct = insu.slm_RemarkAct;
                    ins.slm_ActPayOptionId = insu.slm_ActPayOptionId;
                    ins.slm_ActPayBranchCode = insu.slm_ActPayBranchCode;
                    ins.slm_ActIncentiveFlag = insu.slm_ActIncentiveFlag;
                    ins.slm_ActIncentiveDate = insu.slm_ActIncentiveDate;
                    ins.slm_PolicyComBath = insu.slm_PolicyComBath;
                    ins.slm_PolicyComBathVat = insu.slm_PolicyComBathVat;
                    ins.slm_PolicyComBathIncentive = insu.slm_PolicyComBathIncentive;
                    ins.slm_PolicyOV1Bath = insu.slm_PolicyOV1Bath;
                    ins.slm_PolicyOV1BathVat = insu.slm_PolicyOV1BathVat;
                    ins.slm_PolicyOV1BathIncentive = insu.slm_PolicyOV1BathIncentive;
                    ins.slm_PolicyOV2Bath = insu.slm_PolicyOV2Bath;
                    ins.slm_PolicyOV2BathVat = insu.slm_PolicyOV2BathVat;
                    ins.slm_PolicyOV2BathIncentive = insu.slm_PolicyOV2BathIncentive;
                    ins.slm_PolicyIncentiveAmount = insu.slm_PolicyIncentiveAmount;
                    ins.slm_ActComBath = insu.slm_ActComBath;
                    ins.slm_ActComBathVat = insu.slm_ActComBathVat;
                    ins.slm_ActComBathIncentive = insu.slm_ActComBathIncentive;
                    ins.slm_ActOV1Bath = insu.slm_ActOV1Bath;
                    ins.slm_ActOV1BathVat = insu.slm_ActOV1BathVat;
                    ins.slm_ActOV1BathIncentive = insu.slm_ActOV1BathIncentive;
                    ins.slm_ActOV2Bath = insu.slm_ActOV2Bath;
                    ins.slm_ActOV2BathVat = insu.slm_ActOV2BathVat;
                    ins.slm_ActOV2BathIncentive = insu.slm_ActOV2BathIncentive;
                    ins.slm_ActIncentiveAmount = insu.slm_ActIncentiveAmount;
                    ins.slm_ClaimFlag = insu.slm_ClaimFlag;
                    ins.slm_ActSendDate = insu.slm_ActSendDate;


                    foreach (var adr in sdc.kkslm_tr_renewinsurance_address.Where(a => a.slm_RenewInsureId == insu.slm_RenewInsureId))
                    {
                        addrLst.Add(new LeadDetailAddress()
                        {
                            slm_RenewInsureAddressId = adr.slm_RenewInsureAddressId,
                            slm_RenewInsureId = adr.slm_RenewInsureId,
                            slm_AddressType = adr.slm_AddressType,
                            slm_AddressNo = adr.slm_House_No,
                            slm_BuildingName = adr.slm_Village,
                            slm_Floor = adr.slm_Floor,
                            slm_Soi = adr.slm_Soi,
                            slm_Street = adr.slm_Street,
                            slm_Tambon = adr.slm_Tambon,
                            slm_Amphur = adr.slm_Amphur,
                            slm_Province = adr.slm_Province,
                            slm_PostalCode = adr.slm_PostalCode,
                            slm_House_No = adr.slm_House_No,
                            slm_Moo = adr.slm_Moo,
                            slm_Village = adr.slm_Village
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                _Error = ex.Message;
                ret = false;
            }
            return ret;
        }

        //Added By Pom 25/05/2016
        public bool HasRenewInsurance(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //int count = slmdb.kkslm_tr_renewinsurance.Where(p => p.slm_TicketId == ticketId).Count();
                    //return count > 0 ? true : false;

                    return slmdb.kkslm_tr_renewinsurance.Any(p => p.slm_TicketId == ticketId);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetRenewInsuranceData(string ticketId)
        {
            try
            {
                List<string> list = new List<string>();
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var renew = slmdb.kkslm_tr_renewinsurance.Where(p => p.slm_TicketId == ticketId).FirstOrDefault();
                    if (renew != null)
                    {
                        //string url = SLMConstant.Ecm.SiteUrl.Replace("/dept/public", "");
                        string ecmUrl = SLMConstant.Ecm.SiteUrl + "/" + SLMConstant.Ecm.ListName + "/" + ticketId + "/";

                        list.Add(renew.slm_Need_PolicyFlag != null ? renew.slm_Need_PolicyFlag : "");
                        list.Add(renew.slm_Need_CompulsoryFlag != null ? renew.slm_Need_CompulsoryFlag : "");
                        list.Add(string.IsNullOrEmpty(renew.slm_CreditFileName) ? "" : (renew.slm_CreditFileName + " / " + ecmUrl + renew.slm_CreditFileName));
                        list.Add(string.IsNullOrEmpty(renew.slm_50TawiFileName) ? "" : (renew.slm_50TawiFileName + " / " + ecmUrl + renew.slm_50TawiFileName));
                        list.Add(string.IsNullOrEmpty(renew.slm_DriverLicenseiFileName) ? "" : (renew.slm_DriverLicenseiFileName + " / " + ecmUrl + renew.slm_DriverLicenseiFileName));
                        list.Add(renew.slm_LicenseNo != null ? renew.slm_LicenseNo : "");
                        list.Add(renew.slm_ContractNo != null ? renew.slm_ContractNo : "");
                    }

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        public decimal? GetPolicyAmountPaid(string ticketId, string paymentCode)
        {
            try
            {
                string sql = @"SELECT SUM(detail.slm_RecAmount) AS TotalAmount
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt receipt
                                INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail detail ON receipt.slm_RenewInsuranceReceiptId = detail.slm_RenewInsuranceReceiptId
                                WHERE receipt.slm_ticketId = '" + ticketId + @"' AND detail.slm_PaymentCode = '" + paymentCode + @"' AND detail.is_Deleted = 0 ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var amount = slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
                    return amount != null ? amount : 0;
                }
            }
            catch
            {
                throw;
            }
        }

        public static string GetBlacklistId(string productId, string citizenId)
        {
            try
            {
                string sql = @"SELECT pdb.slm_cp_blacklist_id 
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_blacklist pdb 
                                WHERE pdb.slm_Product_Id = '" + productId + "' AND pdb.slm_CitizenId = '" + citizenId + @"' AND pdb.slm_IsActive = 1
                                AND CONVERT(DATE,GETDATE()) BETWEEN CONVERT(DATE, pdb.slm_StartDate) AND CONVERT(DATE, pdb.slm_EndDate) ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var id = slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
                    return id != null ? id.Value.ToString() : "";
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
