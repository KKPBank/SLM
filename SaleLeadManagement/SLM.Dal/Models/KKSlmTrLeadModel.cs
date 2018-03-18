using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;
using System.Transactions;

namespace SLM.Dal.Models
{
    public class KKSlmTrLeadModel
    {
        private string SLMDBName = ConfigurationManager.AppSettings["SLMDBName"] != null ? ConfigurationManager.AppSettings["SLMDBName"] : "SLMDB";

        public void InsertData(LeadData leadData, string createByUsername, DateTime createDate)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    kkslm_tr_lead lead = new kkslm_tr_lead();
                    lead.slm_ticketId = leadData.TicketId;
                    lead.slm_TitleId = leadData.TitleId;
                    lead.slm_Name = leadData.Name;
                    lead.slm_LastName = leadData.LastName;
                    lead.slm_TelNo_1 = leadData.TelNo_1;
                    //lead.slm_Ext_1 = leadData.Ext_1; 
                    lead.slm_CampaignId = leadData.CampaignId;

                    if (!string.IsNullOrEmpty(leadData.Owner_Branch))
                        lead.slm_Owner_Branch = leadData.Owner_Branch;

                    if (!string.IsNullOrEmpty(leadData.Owner))
                    {
                        lead.slm_Owner = leadData.Owner;
                        lead.slm_Owner_Position = GetPositionId(leadData.Owner, slmdb);
                    }

                    lead.slm_Delegate = leadData.Delegate;
                    lead.slm_Status = "00";
                    lead.slm_StatusBy = createByUsername;
                    lead.slm_StatusDate = createDate;
                    lead.slm_StatusDateSource = createDate;
                    lead.slm_AvailableTime = leadData.AvailableTime;
                    if (leadData.StaffId != null)
                        lead.slm_StaffId = Convert.ToInt32("0" + leadData.StaffId);
                    if (leadData.counting != null)
                        lead.slm_Counting = Convert.ToDecimal(leadData.counting);
                    lead.slm_EmailFlag = leadData.EmailFlag;
                    lead.slm_SmsFlag = leadData.SmsFlag;
                    lead.slm_ChannelId = leadData.ChannelId;
                    lead.slm_CreatedBy = createByUsername;
                    lead.slm_CreatedBy_Position = GetPositionId(createByUsername, slmdb);
                    lead.slm_CreatedDate = createDate;
                    lead.slm_CreatedBy_Branch = leadData.CreatedBy_Branch;
                    lead.slm_UpdatedBy = createByUsername;
                    lead.slm_UpdatedDate = createDate;
                    lead.slm_Delegate_Flag = 0;
                    lead.slm_AssignedFlag = "0";
                    lead.slm_Product_Group_Id = leadData.ProductGroupId;
                    lead.slm_Product_Id = leadData.ProductId;
                    lead.slm_Product_Name = leadData.ProductName;
                    lead.slm_ContractNoRefer = leadData.ContractNoRefer;
                    lead.slm_ThisWork = 0;
                    lead.slm_TotalAlert = 0;
                    lead.slm_TotalWork = 0;

                    slmdb.kkslm_tr_lead.AddObject(lead);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        private int? GetPositionId(string username, SLM_DBEntities slmdb)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetPositionId(username);
        }

        //public void UpdateData(LeadData leadData, string UserId, bool actDelegate, bool actOwner, DateTime updateDate)
        public void UpdateData(LeadData leadData, string UserId, bool actDelegate, bool actOwner, DateTime updateDate, string oldOwner)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == leadData.TicketId).FirstOrDefault();
                    if (lead != null)
                    {
                        lead.slm_ticketId = leadData.TicketId;
                        lead.slm_TitleId = leadData.TitleId;
                        lead.slm_Name = leadData.Name;
                        lead.slm_LastName = leadData.LastName;
                        lead.slm_TelNo_1 = leadData.TelNo_1;
                        //lead.slm_Ext_1 = leadData.Ext_1; 
                        lead.slm_CampaignId = leadData.CampaignId;
                        //2016-12-27 --> SR:5905-123

                        string ticketIdRefer = string.IsNullOrEmpty(lead.slm_TicketIdRefer) ? "" : lead.slm_TicketIdRefer;
                        string newTicketIdRefer = string.IsNullOrEmpty(leadData.TicketIdRefer) ? "" : leadData.TicketIdRefer;
                        if (ticketIdRefer != newTicketIdRefer)
                        {
                            kkslm_tr_activity act = new kkslm_tr_activity();
                            act.slm_CreatedBy = UserId;
                            act.slm_CreatedDate = DateTime.Now;
                            act.slm_UpdatedBy = UserId;
                            act.slm_UpdatedDate = DateTime.Now;
                            act.is_Deleted = 0;
                            act.slm_Type = SLMConstant.ActionType.ReferTicket;
                            act.slm_TicketId = leadData.TicketId;
                            act.slm_SystemAction = SLM.Resource.SLMConstant.SystemName;
                            act.slm_SystemActionBy = SLM.Resource.SLMConstant.SystemName;
                            slmdb.kkslm_tr_activity.AddObject(act);
                        }
                        lead.slm_TicketIdRefer = leadData.TicketIdRefer;
                        //---------------------------------------------------
                        lead.slm_AvailableTime = leadData.AvailableTime;
                        if (leadData.StaffId != null)
                            lead.slm_StaffId = Convert.ToInt32("0" + leadData.StaffId);
                        else
                            lead.slm_StaffId = null;

                        lead.slm_ChannelId = leadData.ChannelId;

                        if (actDelegate)
                        {
                            lead.slm_Delegate_Flag = leadData.Delegate_Flag.Value;
                            if (string.IsNullOrEmpty(leadData.Delegate))
                                lead.slm_DelegateDate = null;
                            else
                                lead.slm_DelegateDate = updateDate;

                            lead.slm_Delegate = string.IsNullOrEmpty(leadData.Delegate) ? null : leadData.Delegate;
                            lead.slm_Delegate_Position = string.IsNullOrEmpty(leadData.Delegate) ? null : GetPositionId(leadData.Delegate, slmdb);

                            if (!string.IsNullOrEmpty(leadData.Delegate_Branch))
                                lead.slm_Delegate_Branch = leadData.Delegate_Branch;
                            else
                                lead.slm_Delegate_Branch = null;
                        }
                        else
                        {
                            if (lead.slm_Delegate == leadData.Delegate)
                            {
                                lead.slm_Delegate = string.IsNullOrEmpty(leadData.Delegate) ? null : leadData.Delegate;
                                lead.slm_Delegate_Position = string.IsNullOrEmpty(leadData.Delegate) ? null : GetPositionId(leadData.Delegate, slmdb);

                                if (!string.IsNullOrEmpty(leadData.Delegate_Branch))
                                    lead.slm_Delegate_Branch = leadData.Delegate_Branch;
                                else
                                    lead.slm_Delegate_Branch = null;
                            }
                        }

                        if (actOwner)
                        {
                            lead.slm_AssignedFlag = "0";
                            lead.slm_AssignedDate = null;
                            lead.slm_AssignedBy = null;

                            if (!string.IsNullOrEmpty(leadData.Owner_Branch))
                                lead.slm_Owner_Branch = leadData.Owner_Branch;
                            else
                                lead.slm_Owner_Branch = null;

                            lead.slm_Owner = leadData.Owner;
                            lead.slm_Owner_Position = GetPositionId(leadData.Owner, slmdb);
                        }
                        else
                        {
                            var owner = GetOwner(leadData.TicketId);
                            if (oldOwner == owner)
                            {
                                //NOTE: เมื่อมีการแก้ไข owner Lead กรณีที่เป็นเจ้าหน้าที่คนเดิม 20170530
                                if (!string.IsNullOrEmpty(leadData.Owner_Branch))
                                    lead.slm_Owner_Branch = leadData.Owner_Branch;
                                else
                                    lead.slm_Owner_Branch = null;

                                lead.slm_Owner = leadData.Owner;
                                lead.slm_Owner_Position = GetPositionId(leadData.Owner, slmdb);
                            }
                        }

                        lead.slm_OldOwner = leadData.slmOldOwner;
                        lead.slm_UpdatedBy = UserId;
                        lead.slm_UpdatedDate = updateDate;
                        lead.slm_ContractNoRefer = leadData.ContractNoRefer;
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void ChangeNoteFlag(string ticketId, bool noteFlag, string updateBy)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                    if (lead != null)
                    {
                        lead.slm_NoteFlag = noteFlag ? "1" : "0";
                        lead.slm_UpdatedBy = updateBy;
                        lead.slm_UpdatedDate = DateTime.Now;
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public LeadData GetStatusAndAssignFlag(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).Select(p => new LeadData { Status = p.slm_Status, AssignedFlag = p.slm_AssignedFlag }).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public string GetLeadStatus(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                    if (lead != null)
                    {
                        return lead.slm_Status;
                    }
                    else
                        return string.Empty;
                }
            }
            catch
            {
                throw;
            }
        }
        public string GetOwner(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId && p.is_Deleted == 0).FirstOrDefault();
                    if (lead != null)
                    {
                        return lead.slm_Owner != null ? lead.slm_Owner : string.Empty;
                    }
                    else
                        return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool HasOwnerOrDelegate(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId && p.is_Deleted == 0).FirstOrDefault();
                    if (lead != null)
                    {
                        if (string.IsNullOrEmpty(lead.slm_Owner) && string.IsNullOrEmpty(lead.slm_Delegate))
                            return false;
                        else
                            return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                throw;
            }
        }

        public LeadOwnerDelegateData GetOwnerAndDelegateName(string ticketId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"SELECT LEAD.slm_ticketId AS TicketId, ISNULL(staff1.slm_StaffNameTH, LEAD.slm_Owner) AS [OwnerName], ISNULL(staff2.slm_StaffNameTH, LEAD.slm_Delegate) AS [DelegateName]
                            FROM dbo.kkslm_tr_lead LEAD WITH (NOLOCK) 
                            LEFT JOIN dbo.kkslm_ms_staff staff1 WITH (NOLOCK) ON LEAD.slm_Owner = staff1.slm_UserName
                            LEFT JOIN dbo.kkslm_ms_staff staff2 WITH (NOLOCK) ON LEAD.slm_Delegate = staff2.slm_UserName
                            WHERE LEAD.slm_ticketId = '" + ticketId + "'";

                return slmdb.ExecuteStoreQuery<LeadOwnerDelegateData>(sql).FirstOrDefault();
            }
        }

        public List<SearchLeadResult> GetLeadOwnerDataTab18_1(string owner)
        {
            string sql = @"SELECT lead.slm_ticketId AS TicketId,cus.slm_CitizenId AS CitizenId,lead.slm_Name AS Firstname,lead.slm_LastName AS Lastname,
	                            ct.slm_CardTypeName AS CardTypeDesc,
                                OP.slm_OptionDesc AS StatusDesc, LEAD.slm_CampaignId AS CampaignId, CAM.slm_CampaignName AS CampaignName,CHANNEL.slm_ChannelDesc AS ChannelDesc,
                                CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN SOWNER.slm_StaffNameTH
		                             ELSE posowner.slm_PositionNameAbb + ' - ' + SOWNER.slm_StaffNameTH END AS OwnerName
	                             ,OBRANCH.slm_BranchName AS OwnerBranchName,LEAD.slm_CreatedDate AS CreatedDate
	                             ,LEAD.slm_AssignedDate AS AssignedDate
                                 ,CASE WHEN posdelegate.slm_PositionNameAbb IS NULL THEN DeStaff.slm_StaffNameTH
		                               ELSE posdelegate.slm_PositionNameAbb + ' - ' + DeStaff.slm_StaffNameTH END AS DelegateName
                                 ,DBRANCH.slm_BranchName AS DelegateBranchName
                            FROM " + SLMDBName + @".DBO.kkslm_tr_lead LEAD 
                                INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_staff sOwner on sOwner.slm_UserName = LEAD.slm_Owner 
	                            INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_branch OBRANCH ON OBRANCH.slm_BranchCode = LEAD.slm_Owner_Branch 
	                            INNER JOIN " + SLMDBName + @".DBo.kkslm_ms_option op on op.slm_OptionCode = lead.slm_Status AND OP.slm_OptionType = 'lead status'
	                            INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_campaign CAM ON CAM.slm_CampaignId = LEAD.slm_CampaignId 
	                            INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_channel CHANNEL ON CHANNEL.slm_ChannelId = LEAD.slm_ChannelId 
	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_staff AS DeStaff on DeStaff.slm_UserName = LEAD.slm_Delegate 
	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_tr_cusinfo cus on cus.slm_TicketId = lead.slm_ticketId
	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_branch DBRANCH ON DBRANCH.slm_BranchCode = LEAD.slm_Delegate_Branch  
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_cardtype ct ON cus.slm_CardType = ct.slm_CardTypeId
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position posowner ON lead.slm_Owner_Position = posowner.slm_Position_id
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position posdelegate ON lead.slm_Delegate_Position = posdelegate.slm_Position_id
                            WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status NOT IN ('" + SLMConstant.StatusCode.Reject + @"','" + SLMConstant.StatusCode.Cancel + @"','" + SLMConstant.StatusCode.Close + @"') AND 
                                  LEAD.slm_Owner = '" + owner + "' ORDER BY  LEAD.slm_CreatedDate DESC ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SearchLeadResult>(sql).ToList();
            }
        }
        public List<SearchLeadResult> GetLeadDelegateDataTab18_2(string delegateuser)
        {
            string sql = @"SELECT lead.slm_ticketId AS TicketId,cus.slm_CitizenId AS CitizenId,lead.slm_Name AS Firstname,lead.slm_LastName AS Lastname,
	                            ct.slm_CardTypeName AS CardTypeDesc, 
                                OP.slm_OptionDesc AS StatusDesc, LEAD.slm_CampaignId AS CampaignId, CAM.slm_CampaignName AS CampaignName,CHANNEL.slm_ChannelDesc AS ChannelDesc
                                ,CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN SOWNER.slm_StaffNameTH
		                              ELSE posowner.slm_PositionNameAbb + ' - ' + SOWNER.slm_StaffNameTH END AS OwnerName
	                             ,OBRANCH.slm_BranchName AS OwnerBranchName,LEAD.slm_CreatedDate AS CreatedDate,
	                             LEAD.slm_AssignedDate AS AssignedDate
                                ,CASE WHEN posdelegate.slm_PositionNameAbb IS NULL THEN DeStaff.slm_StaffNameTH
		                              ELSE posdelegate.slm_PositionNameAbb + ' - ' + DeStaff.slm_StaffNameTH END AS DelegateName
                                ,DBRANCH.slm_BranchName AS DelegateBranchName
                            FROM " + SLMDBName + @".DBO.kkslm_tr_lead LEAD 
                                INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_staff sOwner on sOwner.slm_UserName = LEAD.slm_Owner 
	                            INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_branch OBRANCH ON OBRANCH.slm_BranchCode = LEAD.slm_Owner_Branch 
	                            INNER JOIN " + SLMDBName + @".DBo.kkslm_ms_option op on op.slm_OptionCode = lead.slm_Status AND OP.slm_OptionType = 'lead status'
	                            INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_campaign CAM ON CAM.slm_CampaignId = LEAD.slm_CampaignId 
	                            INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_channel CHANNEL ON CHANNEL.slm_ChannelId = LEAD.slm_ChannelId 
	                            INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_staff AS DeStaff on DeStaff.slm_UserName = LEAD.slm_Delegate 
                                INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_branch DBRANCH ON DBRANCH.slm_BranchCode = LEAD.slm_Delegate_Branch 
	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_tr_cusinfo cus on cus.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_cardtype ct ON cus.slm_CardType = ct.slm_CardTypeId
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position posowner ON lead.slm_Owner_Position = posowner.slm_Position_id
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position posdelegate ON lead.slm_Delegate_Position = posdelegate.slm_Position_id
                            WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status NOT IN ('" + SLMConstant.StatusCode.Reject + @"','" + SLMConstant.StatusCode.Cancel + @"','" + SLMConstant.StatusCode.Close + @"') AND
                                  LEAD.slm_Delegate = '" + delegateuser + "'  ORDER BY  LEAD.slm_CreatedDate DESC ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SearchLeadResult>(sql).ToList();
            }
        }

        public void UpdateTransferLeadOwner(List<string> TicketList, string newowner, int staffid, string Username, string branchcode)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var leadlist = slmdb.kkslm_tr_lead.Where(p => TicketList.Contains(p.slm_ticketId) == true).ToList();
                    if (leadlist.Count > 0)
                    {
                        foreach (kkslm_tr_lead lead in leadlist)
                        {
                            DateTime transferDate = DateTime.Now;
                            lead.slm_OldOwner = lead.slm_Owner;
                            lead.slm_Owner = newowner;
                            lead.slm_Owner_Position = GetPositionId(newowner, slmdb);
                            lead.slm_StaffId = staffid;
                            lead.slm_Owner_Branch = branchcode;
                            lead.slm_AssignedFlag = "0";
                            lead.slm_AssignedDate = null;
                            lead.slm_AssignedBy = null;
                            lead.slm_UpdatedBy = Username;
                            lead.slm_UpdatedDate = transferDate;
                            lead.slm_TransferDate = transferDate;
                        }
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateTransferLeadDelegate(List<string> TicketList, string newDelegate, int staffid, string Username, string branchcode)
        {
            
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var leadlist = slmdb.kkslm_tr_lead.Where(p => TicketList.Contains(p.slm_ticketId) == true).ToList();
                    if (leadlist.Count > 0)
                    {
                        foreach (kkslm_tr_lead lead in leadlist)
                        {
                            DateTime transferDate = DateTime.Now;
                            lead.slm_Delegate = newDelegate;
                            lead.slm_Delegate_Position = GetPositionId(newDelegate, slmdb);
                            lead.slm_Delegate_Branch = branchcode;
                            lead.slm_Delegate_Flag = 1;
                            lead.slm_DelegateDate = transferDate;
                            lead.slm_UpdatedBy = Username;
                            lead.slm_UpdatedDate = transferDate;
                            lead.slm_TransferDate = transferDate;
                        }
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckExistLeadOnHand(string username)
        {
            string sql = @" SELECT  COUNT(LEAD.SLM_TICKETID) AS CNT
                            FROM    " + SLMDBName + @".DBO.kkslm_tr_lead LEAD 
                            WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status NOT IN ('" + SLMConstant.StatusCode.Reject + @"','" + SLMConstant.StatusCode.Cancel + @"','" + SLMConstant.StatusCode.Close + @"') 
                                    AND (LEAD.slm_Owner = '" + username + @"' OR LEAD.slm_Delegate = '" + username + @"')";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var result = slmdb.ExecuteStoreQuery<int>(sql).Select(p => p.ToString()).FirstOrDefault();
                if (result != null)
                {
                    int cnt = int.Parse(result);
                    if (cnt > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        public bool CheckExistPreLeadOnHand(string username)
        {
            string sql = @" SELECT  COUNT(PLEAD.slm_Prelead_Id) AS CNT
                            FROM    " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead PLEAD 
                            WHERE PLEAD.is_Deleted = 0 AND PLEAD.slm_SubStatus NOT IN ('" + SLMConstant.SubStatusCode.NotRenew + @"','" + SLMConstant.SubStatusCode.CarSeized + @"','" + SLMConstant.SubStatusCode.ActPurchased + @"','" + SLMConstant.SubStatusCode.AcceptRenew + @"') 
                                     and slm_TicketId is null and PLEAD.slm_AssignFlag = '1' AND PLEAD.slm_Assign_Status = '1' 
                                    AND PLEAD.slm_Owner = (SELECT SLM_EMPCODE FROM KKSLM_MS_STAFF WHERE slm_UserName =  '" + username + @"')";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var result = slmdb.ExecuteStoreQuery<int>(sql).Select(p => p.ToString()).FirstOrDefault();
                if (result != null)
                {
                    int cnt = int.Parse(result);
                    if (cnt > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }
        public void InsertData(SLM_DBEntities slmdb, string ticketId, string new_ticketId, ProductData productData, string username, DateTime createdDate, string channelId)
        {
            try
            {
                var leadData = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                if (leadData != null)
                {
                    kkslm_tr_lead new_lead = new kkslm_tr_lead();
                    new_lead.slm_ticketId = new_ticketId;
                    new_lead.slm_Name = leadData.slm_Name;
                    new_lead.slm_LastName = leadData.slm_LastName;
                    new_lead.slm_TelNo_1 = leadData.slm_TelNo_1;
                    new_lead.slm_Status = "00";
                    new_lead.slm_StatusDate = createdDate;
                    new_lead.slm_StatusDateSource = createdDate;
                    new_lead.slm_StatusBy = username;
                    new_lead.slm_AvailableTime = leadData.slm_AvailableTime;
                    new_lead.slm_ChannelId = channelId;
                    new_lead.slm_CreatedBy = username;
                    new_lead.slm_CreatedBy_Position = GetPositionId(username, slmdb);
                    new_lead.slm_CreatedDate = createdDate;
                    new_lead.slm_UpdatedBy = username;
                    new_lead.slm_UpdatedDate = createdDate;
                    new_lead.slm_Counting = 0;
                    new_lead.slm_Delegate_Flag = 0;
                    new_lead.slm_AssignedFlag = "0";
                    new_lead.slm_ContractNoRefer = leadData.slm_ContractNoRefer;
                    new_lead.slm_ThisWork = 0;
                    new_lead.slm_TotalAlert = 0;
                    new_lead.slm_TotalWork = 0;

                    //ข้อมูลใหม่
                    new_lead.slm_Owner = username;
                    new_lead.slm_Owner_Position = GetPositionId(username, slmdb);
                    var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == new_lead.slm_Owner).FirstOrDefault();
                    if (staff != null)
                    {
                        new_lead.slm_StaffId = staff.slm_StaffId;
                        new_lead.slm_Owner_Branch = staff.slm_BranchCode;
                        new_lead.slm_CreatedBy_Branch = staff.slm_BranchCode;
                    }
                    new_lead.slm_CampaignId = productData.CampaignId;
                    new_lead.slm_Product_Group_Id = productData.ProductGroupId;
                    new_lead.slm_Product_Id = productData.ProductId;
                    new_lead.slm_Product_Name = productData.ProductName;

                    slmdb.kkslm_tr_lead.AddObject(new_lead);
                }
                else
                    throw new Exception("ไม่พบ Ticket Id " + ticketId + " ใน Table kkslm_tr_lead");
            }
            catch
            {
                throw;
            }
        }

        public string GetAssignFlag(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                    if (lead != null)
                    {
                        return lead.slm_AssignedFlag != null ? lead.slm_AssignedFlag : string.Empty;
                    }
                    else
                        return "";
                }
            }
            catch
            {
                throw;
            }
        }

        public LeadDataPhoneCallHistory GetLeadDataPhoneCallHistoryDefault(string ticketId)
        {
            string sql = @"SELECT lead.slm_ticketId AS TicketId, ISNULL(title.slm_TitleName, '') + lead.slm_Name AS Name, lead.slm_LastName AS LastName, lead.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName
                            , lead.slm_Owner_Branch AS OwnerBranch, lead.slm_Owner AS [Owner], lead.slm_Delegate_Branch AS DelegateBranch, lead.slm_Delegate AS Delegate
                            , lead.slm_TelNo_1 AS TelNo1, lead.slm_Status AS LeadStatus, lead.slm_AssignedFlag AS AssignedFlag, lead.slm_Delegate_Flag AS DelegateFlag, lead.slm_Product_Id AS ProductId
                            , cus.slm_CardType AS CardType, cus.slm_CitizenId AS CitizenId, lead.slm_ChannelId AS ChannelId, lead.slm_Product_Group_Id AS ProductGroupId
                            , pg.product_name AS ProductGroupName, mp.sub_product_name AS ProductName
                            FROM " + SLMDBName + @".dbo.kkslm_tr_lead lead
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_cusinfo cus ON lead.slm_ticketId = cus.slm_TicketId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign cam ON lead.slm_CampaignId = cam.slm_CampaignId
                            LEFT JOIN " + SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP pg ON lead.slm_Product_Group_Id = pg.product_id
                            LEFT JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT mp ON mp.sub_product_id = lead.slm_Product_Id
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId AND title.is_Deleted = 0
                            WHERE lead.slm_ticketId = '" + ticketId + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<LeadDataPhoneCallHistory>(sql).FirstOrDefault();
            }
        }

        public LeadDataPhoneCallHistory GetLeadDataPhoneCallHistory(string ticketId)
        {
            string sql = @"SELECT lead.slm_ticketId AS TicketId,tre.slm_contractno AS ContractNo, ISNULL(title.slm_TitleName, '') + lead.slm_Name AS Name, lead.slm_LastName AS LastName, lead.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName
                            , lead.slm_Owner_Branch AS OwnerBranch, lead.slm_Owner AS [Owner], lead.slm_Delegate_Branch AS DelegateBranch, lead.slm_Delegate AS Delegate
                            , lead.slm_TelNo_1 AS TelNo1, lead.slm_Status AS LeadStatus, lead.slm_SubStatus AS LeadSubStatus, lead.slm_AssignedFlag AS AssignedFlag, lead.slm_Delegate_Flag AS DelegateFlag, lead.slm_Product_Id AS ProductId
                            , cus.slm_CardType AS CardType, cus.slm_CitizenId AS CitizenId
                            , lead.slm_NextContactDate NextContactDate
                            , tre.slm_Need_CreditFlag NeedCreditFlag,tre.slm_CreditFileName CreditFileName
							, tre.slm_Need_50TawiFlag Need_50TawiFlag,tre. slm_50TawiFileName Tawi50FileName
							, tre. slm_Need_DriverLicenseFlag NeedDriverLicense, tre.slm_DriverLicenseiFileName DriverLicenseiFileName
							, tre.slm_Need_PolicyFlag NeedPolicyFlag
							, tre.slm_Need_CompulsoryFlag NeedCompulsoryFlag
							, (select top 1 slm_cp_blacklist_id from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_blacklist pdb where lead.slm_ticketId = pdb.slm_ticketId
									AND pdb.slm_CitizenId = cus.slm_CitizenId AND slm_IsActive = 1 AND 
							CONVERT(DATE,GETDATE()) BETWEEN CONVERT(DATE,slm_StartDate) AND CONVERT(DATE,slm_EndDate)) blacklist
                            , prelead.slm_Prelead_Id AS PreleadId, lead.slm_ChannelId AS ChannelId, lead.slm_Product_Group_Id AS ProductGroupId
                            , pg.product_name AS ProductGroupName, mp.sub_product_name AS ProductName
                            FROM " + SLMDBName + @".dbo.kkslm_tr_lead lead
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_renewinsurance tre ON tre.slm_ticketId = lead.slm_ticketId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_cusinfo cus ON lead.slm_ticketId = cus.slm_TicketId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign cam ON lead.slm_CampaignId = cam.slm_CampaignId
                            LEFT JOIN " + SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP pg ON lead.slm_Product_Group_Id = pg.product_id
                            LEFT JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT mp ON mp.sub_product_id = lead.slm_Product_Id
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_prelead prelead ON prelead.slm_TicketId = lead.slm_ticketId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId AND title.is_Deleted = 0
                            WHERE lead.slm_ticketId = '" + ticketId + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<LeadDataPhoneCallHistory>(sql).FirstOrDefault();
            }
        }

        public List<string> GetAssignedFlagAndDelegateFlag(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<string> list = new List<string>();

                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                    if (lead != null)
                    {
                        list.Add(lead.slm_AssignedFlag != null ? lead.slm_AssignedFlag.Trim() : "");
                        list.Add(lead.slm_Delegate_Flag.ToString());
                        return list;
                    }
                    else
                        throw new Exception("ไม่พบ Ticket Id " + ticketId + " ในระบบ");
                }
            }
            catch
            {
                throw;
            }
        }

        public string[] GetProductCampaign(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string[] arr = new string[2];
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                    if (lead != null)
                    {
                        arr[0] = lead.slm_CampaignId;
                        arr[1] = lead.slm_Product_Id;
                    }
                    return arr;
                }
            }
            catch
            {
                throw;
            }
        }

        //Added by Pom 22/04/2016
        public CustomerData GetCustomerData(string ticketId)
        {
            try
            {
                string sql = @"SELECT renew.slm_ContractNo AS ContractNo, ISNULL(title.slm_TitleName, '') + lead.slm_Name + ' ' + lead.slm_LastName AS CustomerName
                                , lead.slm_ticketId AS TicketId, cam.slm_CampaignName AS CampaignName, brand.slm_BrandName AS CarBrandName, model.slm_ModelName AS CarModelName
                                , renew.slm_LicenseNo AS CarLicenseNo
                                FROM dbo.kkslm_tr_lead lead with (nolock) 
                                LEFT JOIN dbo.kkslm_tr_renewinsurance renew with (nolock) ON lead.slm_TicketId = renew.slm_TicketId
                                LEFT JOIN dbo.kkslm_ms_title title with (nolock) ON title.slm_TitleId = lead.slm_TitleId
                                LEFT JOIN dbo.kkslm_ms_campaign cam with (nolock) ON cam.slm_CampaignId = lead.slm_CampaignId
                                LEFT JOIN dbo.kkslm_ms_redbook_brand brand with (nolock) ON brand.slm_BrandCode = renew.slm_RedbookBrandCode
                                LEFT JOIN dbo.kkslm_ms_redbook_model model with (nolock) ON model.slm_BrandCode = renew.slm_RedbookBrandCode AND model.slm_ModelCode = renew.slm_RedbookModelCode
                                WHERE lead.slm_ticketId = '" + ticketId + "' ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<CustomerData>(sql).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<LeadData> GetLeadOnHandList(string username)
        {
            try
            {
                string sql = @"SELECT slm_ticketId AS TicketId, slm_NextContactDate AS NextContactDate
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
                                WHERE lead.is_Deleted = 0 AND lead.slm_AssignedFlag = '1' AND lead.slm_Status NOT IN ('08','09','10') 
                                AND lead.slm_Owner = '" + username + @"' AND lead.slm_Delegate IS NULL
                                UNION ALL
                                SELECT slm_ticketId AS TicketId, slm_NextContactDate AS NextContactDate
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
                                WHERE lead.is_Deleted = 0 AND lead.slm_Delegate_Flag = '0' AND lead.slm_Status NOT IN ('08','09','10') 
                                AND lead.slm_Delegate = '" + username + @"' ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<LeadData>(sql).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void SaveLeadData(LeadData lead)
        {
            try
            {
                KKSlmTrProductInfoModel productInfo = new KKSlmTrProductInfoModel();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //saveData

                    //Prev
                    //Prelead Data

                    saveLeadDataToRenewInsurance_compare(lead);
                    saveLeadDataToRenewInsurance_compare_snap(lead);
                    //saveLeadDataToRenewInsurance();
                    saveLeadDataToRenewInsurance_paymentmain(lead);
                    productInfo.updateLeadDataToProductInfo();

                    //savePreleadData
                    scope.Complete();
                }
            }
            catch
            {
                throw;
            }
        }


        public void saveLeadDataToRenewInsurance_compare(LeadData lead)
        {

            kkslm_tr_renewinsurance_compare renewInsCom = new kkslm_tr_renewinsurance_compare();

            renewInsCom.slm_RenewInsureId = lead.slm_RenewInsureId;
            renewInsCom.slm_NotifyPremiumId = lead.slm_NotifyPremiumId;
            renewInsCom.slm_PromotionId = lead.slm_PromotionId;
            renewInsCom.slm_Seq = lead.slm_Seq;
            renewInsCom.slm_Year = lead.slm_Year;
            renewInsCom.slm_Ins_Com_Id = lead.slm_Ins_Com_Id;
            renewInsCom.slm_InjuryDeath = lead.slm_InjuryDeath;
            renewInsCom.slm_TPPD = lead.slm_TPPD;
            renewInsCom.slm_RepairTypeId = lead.slm_RepairTypeId;
            renewInsCom.slm_OD = lead.slm_OD;
            renewInsCom.slm_FT = lead.slm_FT;
            renewInsCom.slm_DeDuctible = lead.slm_DeDuctible;
            renewInsCom.slm_PersonalAccident = lead.slm_PersonalAccident;
            //renewInsCom.slm_PersonalAccidentPassenger = lead.slm_Prelead_Id;
            //renewInsCom.slm_PersonalAccidentDriver = lead.slm_Prelead_Id;
            renewInsCom.slm_MedicalFee = lead.slm_MedicalFee;
            //renewInsCom.slm_MedicalFeeDriver = lead.slm_Prelead_Id;
            //renewInsCom.slm_MedicalFeePassenger = lead.slm_Prelead_Id;
            renewInsCom.slm_InsuranceDriver = lead.slm_InsuranceDriver;
            renewInsCom.slm_PolicyGrossStamp = lead.slm_PolicyGrossStamp;
            renewInsCom.slm_PolicyGrossVat = lead.slm_PolicyGrossVat;
            renewInsCom.slm_PolicyGrossPremium = lead.slm_PolicyGrossPremium;
            renewInsCom.slm_NetGrossPremium = lead.slm_NetGrossPremium;
            renewInsCom.slm_CreatedDate = DateTime.Now;
            renewInsCom.slm_CreatedBy = lead.slm_CreatedBy;
            renewInsCom.slm_UpdatedDate = DateTime.Now;
            renewInsCom.slm_UpdatedBy = lead.slm_UpdatedBy;
            renewInsCom.slm_Selected = lead.slm_Selected;
            renewInsCom.slm_OldPolicyNo = lead.slm_OldPolicyNo;
            renewInsCom.slm_DriverFlag = lead.slm_DriverFlag;
            renewInsCom.slm_Driver_TitleId1 = lead.slm_Driver_TitleId1;
            renewInsCom.slm_Driver_First_Name1 = lead.slm_Driver_First_Name1;
            renewInsCom.slm_Driver_Last_Name1 = lead.slm_Driver_Last_Name1;
            renewInsCom.slm_Driver_Birthdate1 = lead.slm_Driver_Birthdate1;
            renewInsCom.slm_Driver_TitleId2 = lead.slm_Driver_TitleId2;
            renewInsCom.slm_Driver_First_Name2 = lead.slm_Driver_First_Name2;
            renewInsCom.slm_Driver_Last_Name2 = lead.slm_Driver_Last_Name2;
            renewInsCom.slm_Driver_Birthdate2 = lead.slm_Driver_Birthdate2;
            renewInsCom.slm_OldReceiveNo = lead.slm_OldReceiveNo;
            renewInsCom.slm_PolicyStartCoverDate = lead.slm_PolicyStartCoverDate;
            renewInsCom.slm_PolicyEndCoverDate = lead.slm_PolicyEndCoverDate;
            renewInsCom.slm_Vat1Percent = lead.slm_Vat1Percent;
            renewInsCom.slm_DiscountPercent = lead.slm_DiscountPercent;
            renewInsCom.slm_DiscountBath = lead.slm_DiscountBath;
            renewInsCom.slm_Vat1PercentBath = lead.slm_Vat1PercentBath;

            //renewInsCom.slm_RenewInsureId = pre.slm_Prelead_Id;
            //renewInsCom.slm_NotifyPremiumId = pre.slm_Prelead_Id;
            //renewInsCom.slm_PromotionId = pre.slm_Prelead_Id;
            //renewInsCom.slm_Seq = pre.slm_Prelead_Id;
            //renewInsCom.slm_Year = pre.slm_Prelead_Id;
            //renewInsCom.slm_Ins_Com_Id = pre.slm_Prelead_Id;
            //renewInsCom.slm_InjuryDeath = pre.slm_Prelead_Id;
            //renewInsCom.slm_TPPD = pre.slm_Prelead_Id;
            //renewInsCom.slm_RepairTypeId = pre.slm_Prelead_Id;
            //renewInsCom.slm_OD = pre.slm_Prelead_Id;
            //renewInsCom.slm_FT = pre.slm_Prelead_Id;
            //renewInsCom.slm_DeDuctible = pre.slm_Prelead_Id;
            //renewInsCom.slm_PersonalAccident = pre.slm_Prelead_Id;
            ////renewInsCom.slm_PersonalAccidentPassenger = pre.slm_Prelead_Id;
            ////renewInsCom.slm_PersonalAccidentDriver = pre.slm_Prelead_Id;
            //renewInsCom.slm_MedicalFee = pre.slm_Prelead_Id;
            ////renewInsCom.slm_MedicalFeeDriver = pre.slm_Prelead_Id;
            ////renewInsCom.slm_MedicalFeePassenger = pre.slm_Prelead_Id;
            //renewInsCom.slm_InsuranceDriver = pre.slm_Prelead_Id;
            //renewInsCom.slm_PolicyGrossStamp = pre.slm_Prelead_Id;
            //renewInsCom.slm_PolicyGrossVat = pre.slm_Prelead_Id;
            //renewInsCom.slm_PolicyGrossPremium = pre.slm_Prelead_Id;
            //renewInsCom.slm_NetGrossPremium = pre.slm_Prelead_Id;
            //renewInsCom.slm_CreatedDate = pre.slm_Prelead_Id;
            //renewInsCom.slm_CreatedBy = pre.slm_Prelead_Id;
            //renewInsCom.slm_UpdatedDate = pre.slm_Prelead_Id;
            //renewInsCom.slm_UpdatedBy = pre.slm_Prelead_Id;
            //renewInsCom.slm_Selected = pre.slm_Prelead_Id;
            //renewInsCom.slm_OldPolicyNo = pre.slm_Prelead_Id;
            //renewInsCom.slm_DriverFlag = pre.slm_Prelead_Id;
            //renewInsCom.slm_Driver_TitleId1 = pre.slm_Prelead_Id;
            //renewInsCom.slm_Driver_First_Name1 = pre.slm_Prelead_Id;
            //renewInsCom.slm_Driver_Last_Name1 = pre.slm_Prelead_Id;
            //renewInsCom.slm_Driver_Birthdate1 = pre.slm_Prelead_Id;
            //renewInsCom.slm_Driver_TitleId2 = pre.slm_Prelead_Id;
            //renewInsCom.slm_Driver_First_Name2 = pre.slm_Prelead_Id;
            //renewInsCom.slm_Driver_Last_Name2 = pre.slm_Prelead_Id;
            //renewInsCom.slm_Driver_Birthdate2 = pre.slm_Prelead_Id;
            //renewInsCom.slm_OldReceiveNo = pre.slm_Prelead_Id;
            //renewInsCom.slm_PolicyStartCoverDate = pre.slm_Prelead_Id;
            //renewInsCom.slm_PolicyEndCoverDate = pre.slm_Prelead_Id;
            //renewInsCom.slm_Vat1Percent = pre.slm_Prelead_Id;
            //renewInsCom.slm_DiscountPercent = pre.slm_Prelead_Id;
            //renewInsCom.slm_DiscountBath = pre.slm_Prelead_Id;
            //renewInsCom.slm_Vat1PercentBath = pre.slm_Prelead_Id;

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.kkslm_tr_renewinsurance_compare.AddObject(renewInsCom);
                slmdb.SaveChanges();
            }
        }

        public void saveLeadDataToRenewInsurance_compare_snap(LeadData lead)
        {

            kkslm_tr_renewinsurance_compare_snap renewInsCom = new kkslm_tr_renewinsurance_compare_snap();

            renewInsCom.slm_RenewInsureId = lead.slm_RenewInsureId;
            renewInsCom.slm_NotifyPremiumId = lead.slm_NotifyPremiumId;
            renewInsCom.slm_PromotionId = lead.slm_PromotionId;
            renewInsCom.slm_Seq = lead.slm_Seq;
            renewInsCom.slm_Year = lead.slm_Year;
            renewInsCom.slm_Ins_Com_Id = lead.slm_Ins_Com_Id;
            renewInsCom.slm_InjuryDeath = lead.slm_InjuryDeath;
            renewInsCom.slm_TPPD = lead.slm_TPPD;
            renewInsCom.slm_RepairTypeId = lead.slm_RepairTypeId;
            renewInsCom.slm_OD = lead.slm_OD;
            renewInsCom.slm_FT = lead.slm_FT;
            renewInsCom.slm_DeDuctible = lead.slm_DeDuctible;
            renewInsCom.slm_PersonalAccident = lead.slm_PersonalAccident;
            //renewInsCom.slm_PersonalAccidentPassenger = lead.slm_Prelead_Id;
            //renewInsCom.slm_PersonalAccidentDriver = lead.slm_Prelead_Id;
            renewInsCom.slm_MedicalFee = lead.slm_MedicalFee;
            //renewInsCom.slm_MedicalFeeDriver = lead.slm_Prelead_Id;
            //renewInsCom.slm_MedicalFeePassenger = lead.slm_Prelead_Id;
            renewInsCom.slm_InsuranceDriver = lead.slm_InsuranceDriver;
            renewInsCom.slm_PolicyGrossStamp = lead.slm_PolicyGrossStamp;
            renewInsCom.slm_PolicyGrossVat = lead.slm_PolicyGrossVat;
            renewInsCom.slm_PolicyGrossPremium = lead.slm_PolicyGrossPremium;
            renewInsCom.slm_NetGrossPremium = lead.slm_NetGrossPremium;
            renewInsCom.slm_CreatedDate = DateTime.Now;
            renewInsCom.slm_CreatedBy = lead.slm_CreatedBy;
            renewInsCom.slm_UpdatedDate = DateTime.Now;
            renewInsCom.slm_UpdatedBy = lead.slm_UpdatedBy;
            renewInsCom.slm_Selected = lead.slm_Selected;
            renewInsCom.slm_OldPolicyNo = lead.slm_OldPolicyNo;
            renewInsCom.slm_DriverFlag = lead.slm_DriverFlag;
            renewInsCom.slm_Driver_TitleId1 = lead.slm_Driver_TitleId1;
            renewInsCom.slm_Driver_First_Name1 = lead.slm_Driver_First_Name1;
            renewInsCom.slm_Driver_Last_Name1 = lead.slm_Driver_Last_Name1;
            renewInsCom.slm_Driver_Birthdate1 = lead.slm_Driver_Birthdate1;
            renewInsCom.slm_Driver_TitleId2 = lead.slm_Driver_TitleId2;
            renewInsCom.slm_Driver_First_Name2 = lead.slm_Driver_First_Name2;
            renewInsCom.slm_Driver_Last_Name2 = lead.slm_Driver_Last_Name2;
            renewInsCom.slm_Driver_Birthdate2 = lead.slm_Driver_Birthdate2;
            renewInsCom.slm_OldReceiveNo = lead.slm_OldReceiveNo;
            renewInsCom.slm_PolicyStartCoverDate = lead.slm_PolicyStartCoverDate;
            renewInsCom.slm_PolicyEndCoverDate = lead.slm_PolicyEndCoverDate;
            renewInsCom.slm_Vat1Percent = lead.slm_Vat1Percent;
            renewInsCom.slm_DiscountPercent = lead.slm_DiscountPercent;
            renewInsCom.slm_DiscountBath = lead.slm_DiscountBath;
            renewInsCom.slm_Vat1PercentBath = lead.slm_Vat1PercentBath;
            renewInsCom.slm_Version = lead.slm_Version;

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.kkslm_tr_renewinsurance_compare_snap.AddObject(renewInsCom);
                slmdb.SaveChanges();
            }
        }

        public void saveLeadDataToRenewInsurance(LeadData lead)
        {

            kkslm_tr_renewinsurance renewIns = new kkslm_tr_renewinsurance();

            renewIns.slm_RedbookBrandCode = lead.slm_RedbookBrandCode;
            renewIns.slm_RedbookModelCode = lead.slm_RedbookModelCode;
            renewIns.slm_CC = lead.slm_CC;
            renewIns.slm_CoverageTypeId = lead.slm_CoverageTypeId;
            renewIns.slm_InsuranceComId = lead.slm_InsuranceComId;
            renewIns.slm_PolicyDiscountAmt = lead.slm_PolicyDiscountAmt;
            renewIns.slm_PolicyGrossVat = lead.slm_PolicyGrossVat;
            renewIns.slm_PolicyGrossStamp = lead.slm_PolicyGrossStamp;
            renewIns.slm_PolicyGrossPremium = lead.slm_PolicyGrossPremium;
            renewIns.slm_PolicyGrossPremiumTotal = lead.slm_PolicyGrossPremiumTotal;
            renewIns.slm_PolicyCost = lead.slm_PolicyCost;
            renewIns.slm_RepairTypeId = lead.slm_RepairTypeId;
            renewIns.slm_PolicyPayMethodId = lead.slm_PolicyPayMethodId;
            //renewInsCom.slm_PersonalAccidentPassenger = lead.slm_Prelead_Id;
            //renewInsCom.slm_PersonalAccidentDriver = lead.slm_Prelead_Id;
            renewIns.slm_PolicyAmountPeriod = lead.slm_PolicyAmountPeriod;
            //renewInsCom.slm_MedicalFeeDriver = lead.slm_Prelead_Id;
            //renewInsCom.slm_MedicalFeePassenger = lead.slm_Prelead_Id;
            renewIns.slm_Need_CreditFlag = lead.slm_Need_CreditFlag;
            renewIns.slm_Need_50TawiFlag = lead.slm_Need_50TawiFlag;
            renewIns.slm_Need_DriverLicenseFlag = lead.slm_Need_DriverLicenseFlag;
            renewIns.slm_RemarkPayment = lead.slm_RemarkPayment;
            renewIns.slm_PolicyCostSave = lead.slm_PolicyCostSave;
            renewIns.slm_Vat1Percent = lead.slm_Vat1Percent;
            renewIns.slm_DiscountPercent = lead.slm_DiscountPercent;
            renewIns.slm_Vat1PercentBath = lead.slm_Vat1PercentBath;
            renewIns.slm_PayOptionId = lead.slm_PayOptionId;
            renewIns.slm_PayBranchCode = lead.slm_PayBranchCode;
            renewIns.slm_RemarkPolicy = lead.slm_RemarkPolicy;



            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.kkslm_tr_renewinsurance.AddObject(renewIns);
                slmdb.SaveChanges();
            }
        }

        public void saveLeadDataToRenewInsurance_paymentmain(LeadData lead)
        {

            kkslm_tr_renewinsurance_paymentmain renewPay = new kkslm_tr_renewinsurance_paymentmain();

            renewPay.slm_RenewInsureId = lead.slm_RenewInsureId;
            renewPay.slm_Seq = lead.slm_Seq;
            renewPay.slm_Type = lead.slm_Type;
            renewPay.slm_PaymentDate = lead.slm_PaymentDate;
            renewPay.slm_Period = lead.slm_Period;
            renewPay.slm_PaymentAmount = lead.slm_PaymentAmount;
            renewPay.slm_CreatedBy = lead.slm_CreatedBy;
            renewPay.slm_CreatedDate = DateTime.Now;
            renewPay.slm_UpdatedBy = lead.slm_UpdatedBy;
            renewPay.slm_UpdatedDate = DateTime.Now;

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.kkslm_tr_renewinsurance_paymentmain.AddObject(renewPay);
                slmdb.SaveChanges();
            }
        }

        public void updateLeadDataToLead(LeadData lead)
        {

            kkslm_tr_lead leadData = new kkslm_tr_lead();

            leadData.slm_Status = lead.slm_Status;
            leadData.slm_SubStatus = lead.slm_SubStatus;
            leadData.slm_ExternalSubStatusDesc = lead.slm_ExternalSubStatusDesc;
            leadData.slm_StatusDateSource = lead.slm_StatusDateSource;
            leadData.slm_StatusDate = lead.slm_StatusDate;
            leadData.slm_CreatedDate = DateTime.Now;
            leadData.slm_CreatedBy = lead.slm_CreatedBy;
            leadData.slm_UpdatedDate = DateTime.Now;
            leadData.slm_UpdatedBy = lead.slm_UpdatedBy;

            //slmdb.SaveChanges();
        }

        //public void saveLeadDataToRenewInsurance(LeadData lead)
        //{

        //    kkslm_tr_renewinsurance renewIns = new kkslm_tr_renewinsurance();

        //    renewIns.slm_RedbookBrandCode = lead.slm_RedbookBrandCode;
        //    renewIns.slm_RedbookModelCode = lead.slm_RedbookModelCode;
        //    renewIns.slm_CC = lead.slm_CC;
        //    renewIns.slm_CoverageTypeId = lead.slm_CoverageTypeId;
        //    renewIns.slm_InsuranceComId = lead.slm_InsuranceComId;
        //    renewIns.slm_PolicyDiscountAmt = lead.slm_PolicyDiscountAmt;
        //    renewIns.slm_PolicyGrossVat = lead.slm_PolicyGrossVat;
        //    renewIns.slm_PolicyGrossStamp = lead.slm_PolicyGrossStamp;
        //    renewIns.slm_PolicyGrossPremium = lead.slm_PolicyGrossPremium;
        //    renewIns.slm_PolicyGrossPremiumTotal = lead.slm_PolicyGrossPremiumTotal;
        //    renewIns.slm_PolicyCost = lead.slm_PolicyCost;
        //    renewIns.slm_RepairTypeId = lead.slm_RepairTypeId;
        //    renewIns.slm_PolicyPayMethodId = lead.slm_PolicyPayMethodId;
        //    //renewInsCom.slm_PersonalAccidentPassenger = lead.slm_Prelead_Id;
        //    //renewInsCom.slm_PersonalAccidentDriver = lead.slm_Prelead_Id;
        //    renewIns.slm_PolicyAmountPeriod = lead.slm_PolicyAmountPeriod;
        //    //renewInsCom.slm_MedicalFeeDriver = lead.slm_Prelead_Id;
        //    //renewInsCom.slm_MedicalFeePassenger = lead.slm_Prelead_Id;
        //    renewIns.slm_Need_CreditFlag = lead.slm_Need_CreditFlag;
        //    renewIns.slm_Need_50TawiFlag = lead.slm_Need_50TawiFlag;
        //    renewIns.slm_Need_DriverLicenseFlag = lead.slm_Need_DriverLicenseFlag;
        //    renewIns.slm_RemarkPayment = lead.slm_RemarkPayment;
        //    renewIns.slm_PolicyCostSave = lead.slm_PolicyCostSave;
        //    renewIns.slm_Vat1Percent = lead.slm_Vat1Percent;
        //    renewIns.slm_DiscountPercent = lead.slm_DiscountPercent;
        //    renewIns.slm_Vat1PercentBath = lead.slm_Vat1PercentBath;
        //    renewIns.slm_PayOptionId = lead.slm_PayOptionId;
        //    renewIns.slm_PayBranchCode = lead.slm_PayBranchCode;
        //    renewIns.slm_RemarkPolicy = lead.slm_RemarkPolicy;


        //    slmdb.kkslm_tr_renewinsurance.AddObject(renewIns);
        //    slmdb.SaveChanges();
        //}

        public List<ControlListData> getStatusLead(string CampaignId, string ProductId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql1 = @"SELECT DISTINCT mo.slm_OptionCode as ValueField, mo.slm_OptionDesc as TextField
                    FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus CS INNER JOIN kkslm_ms_option MO ON MO.slm_OptionCode = CS.slm_OptionCode AND MO.slm_OptionType = 'lead status'
                    WHERE cs.slm_Product_Id is null and cs.slm_CampaignId = '" + CampaignId + "' AND cs.is_Deleted = 0";

                List<ControlListData> data1 = slmdb.ExecuteStoreQuery<ControlListData>(sql1).ToList();

                if (data1 != null && data1.Count > 0)
                {
                    return data1;
                }
                else
                {
                    string sql2 = @"SELECT DISTINCT mo.slm_OptionCode as ValueField, mo.slm_OptionDesc as TextField
                                FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus CS INNER JOIN kkslm_ms_option MO ON MO.slm_OptionCode = CS.slm_OptionCode AND MO.slm_OptionType = 'lead status'
                                WHERE cs.slm_Product_Id = '" + ProductId + "' and cs.slm_CampaignId is null AND cs.is_Deleted = 0";

                    return slmdb.ExecuteStoreQuery<ControlListData>(sql2).ToList();
                }
            }
        }

        public List<ControlListData> getSubStatusLead(string CampaignId, string ProductId,string LeadStatus)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql1 = @"SELECT DISTINCT slm_SubStatusCode as ValueField, slm_SubStatusName as TextField
                    FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus CS
                    WHERE cs.slm_Product_Id is null and cs.slm_CampaignId = '" + CampaignId + "' AND slm_optionCode = '" + LeadStatus + "' AND  cs.is_Deleted = 0";

                List<ControlListData> data1 = slmdb.ExecuteStoreQuery<ControlListData>(sql1).ToList();

                if (data1 != null && data1.Count > 0)
                {
                    return data1;
                }
                else
                {
                    string sql2 = @"SELECT DISTINCT slm_SubStatusCode as ValueField, slm_SubStatusName as TextField
                                FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus CS
                                WHERE cs.slm_Product_Id = '" + ProductId + "' AND slm_optionCode = '" + LeadStatus + "' and cs.slm_CampaignId is null AND cs.is_Deleted = 0";

                    return slmdb.ExecuteStoreQuery<ControlListData>(sql2).ToList();
                }
            }
        }


        public bool checkBlackList(string Username)
        {
            string sql = @"SELECT count(1) FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff WHERE slm_StaffTypeId in ('7','15') and is_Deleted = 0 and slm_UserName = '" + Username + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault() > 0 ? true : false;
            }
        }

        public LeadDefaultData GetLeadDataForInitialPhoneCall(string ticketId)
        {
            string sql = @"SELECT lead.slm_TicketId AS TicketId, prelead.slm_Prelead_Id AS PreleadId, cus.slm_CardType AS CardType, cus.slm_CitizenId AS CitizenId, lead.slm_TelNo_1 AS TelNo1
                            , lead.slm_Product_Id AS ProductId, lead.slm_Status AS StatusCode, lead.coc_IsCOC AS ISCOC, lead.coc_CurrentTeam AS COCCurrentTeam
                            , cardtype.slm_CARSubscriptionTypeId AS SubScriptionTypeId
                            FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) 
                            LEFT JOIN dbo.kkslm_tr_cusInfo cus WITH (NOLOCK) ON lead.slm_TicketId = cus.slm_TicketId
                            LEFT JOIN dbo.kkslm_tr_prelead prelead WITH (NOLOCK) ON lead.slm_TicketId = prelead.slm_TicketId
                            LEFT JOIN dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) ON cardtype.slm_CardTypeId = cus.slm_CardType
                            WHERE lead.slm_TicketId = '" + ticketId + "' ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<LeadDefaultData>(sql).FirstOrDefault();
            }
        }

        public string GetStatus(string ticketId)
        {
            string sql = $"SELECT slm_Status FROM kkslm_tr_lead WITH (NOLOCK) WHERE slm_ticketId = '{ticketId}' ";
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
            }
        }

        public LeadDataForCARScript GetLeadDataForCAR(string ticketId)
        {
            string sql = @"SELECT lead.slm_ticketId AS TicketId, prelead.slm_Prelead_Id AS PreleadId, cus.slm_CardType AS CardType, cus.slm_CitizenId AS CitizenId
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_cusinfo cus ON cus.slm_TicketId = lead.slm_ticketId
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead prelead ON prelead.slm_TicketId = lead.slm_ticketId
                            WHERE lead.slm_ticketId = '" + ticketId + "' ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<LeadDataForCARScript>(sql).FirstOrDefault();
            }
        }
    }
}
