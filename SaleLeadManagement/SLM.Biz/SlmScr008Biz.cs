using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Dal.Models;
using SLM.Dal;

namespace SLM.Biz
{
    public class SlmScr008Biz : IDisposable
    {
        private DateTime _createdDate = new DateTime();
        private SLM_DBEntities slmdb = null;
        private string _calculateTotalSlaError = "";

        public SlmScr008Biz()
        {
            slmdb = DBUtil.GetSlmDbEntities();
            slmdb.Connection.Open();
        }

        public void Dispose()
        {
            if (slmdb != null)
            {
                slmdb.Connection.Close();
                slmdb.Connection.Dispose();
                slmdb.Dispose();
            }
        }

        public void CloseConnection()
        {
            if (slmdb != null)
            {
                slmdb.Connection.Close();
            }
        }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
        }
        public string CalculateTotalSlaError
        {
            get { return _calculateTotalSlaError; }
        }

        public LeadDataPhoneCallHistory GetLeadDataPhoneCallHistoryDefault(string ticketId)
        {
            try
            {
                string sql = @"SELECT lead.slm_ticketId AS TicketId, ISNULL(title.slm_TitleName, '') + lead.slm_Name AS Name, lead.slm_LastName AS LastName, lead.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName
                            , lead.slm_Owner_Branch AS OwnerBranch, lead.slm_Owner AS [Owner], lead.slm_Delegate_Branch AS DelegateBranch, lead.slm_Delegate AS Delegate
                            , lead.slm_TelNo_1 AS TelNo1, lead.slm_Status AS LeadStatus, lead.slm_AssignedFlag AS AssignedFlag, lead.slm_Delegate_Flag AS DelegateFlag, lead.slm_Product_Id AS ProductId
                            , cus.slm_CardType AS CardType, cus.slm_CitizenId AS CitizenId, lead.slm_ChannelId AS ChannelId, lead.slm_Product_Group_Id AS ProductGroupId
                            , pg.product_name AS ProductGroupName, mp.sub_product_name AS ProductName
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_cusinfo cus ON lead.slm_ticketId = cus.slm_TicketId
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam ON lead.slm_CampaignId = cam.slm_CampaignId
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP pg ON lead.slm_Product_Group_Id = pg.product_id
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT mp ON mp.sub_product_id = lead.slm_Product_Id
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId AND title.is_Deleted = 0
                            WHERE lead.slm_ticketId = '" + ticketId + "'";

                return slmdb.ExecuteStoreQuery<LeadDataPhoneCallHistory>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public List<ControlListData> GetCardTypeList()
        {
            try
            {
                return slmdb.kkslm_ms_cardtype.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_CardTypeId).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_CardTypeName, ValueField = p.slm_CardTypeId.ToString() }).ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<ControlListData> GetBranchAccessRightList(int flag, string campaignId)
        {
            string sql = @"SELECT branch.slm_BranchName AS TextField, branch.slm_BranchCode AS ValueField 
                            FROM kkslm_ms_branch branch WITH (NOLOCK) INNER JOIN (
                                            SELECT DISTINCT Z.slm_BranchCode  
                                            FROM (
                                                    SELECT AR.slm_BranchCode
                                                    FROM kkslm_ms_access_right AR INNER JOIN kkslm_ms_campaign CAM ON CAM.slm_CampaignId = AR.slm_CampaignId 
                                                    WHERE CAM.slm_CampaignId = '{1}' 
                                                    UNION ALL
                                                    SELECT AR.slm_BranchCode
                                                    FROM kkslm_ms_access_right AR INNER JOIN CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
                                                    WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT 
													                            WHERE PR_CAMPAIGNID = '{1}')
                                                 ) AS Z
                                          ) AS A ON A.slm_BranchCode = branch.slm_BranchCode
                            {0}  
                            ORDER BY slm_BranchName  ";
            string wh = "";

            if (flag == SLMConstant.Branch.Active)
            {
                wh = " WHERE branch.is_Deleted = 0 ";
            }
            else if (flag == SLMConstant.Branch.InActive)
            {
                wh = " WHERE branch.is_Deleted = 1 ";
            }
            else if (flag == SLMConstant.Branch.All)
            {
                wh = "";
            }
            else
                wh = "";

            sql = string.Format(sql, wh, campaignId);

            return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
        }

        public List<ControlListData> GetStatusListByActivityConfig(string productId, string leadStatus)
        {
            string sql = @"SELECT ca.slm_AvailableStatus AS ValueField, opt2.slm_OptionDesc AS TextField
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_activity ca
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option opt2 ON ca.slm_AvailableStatus = opt2.slm_OptionCode AND opt2.slm_OptionType = 'lead status'  
                            WHERE ca.is_Deleted = 0 AND ca.slm_Product_Id = '" + productId + "' AND ca.slm_LeadStatus = '" + leadStatus + "'";

            return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
        }

        public List<ControlListData> GetStaffAllDataByAccessRight(string campaignId, string branch)
        {
            string sql = @"SELECT CASE WHEN PO.slm_PositionNameAbb IS NULL THEN  staff.slm_StaffNameTH  
                                  ELSE PO.slm_PositionNameAbb + ' - ' + STAFF.slm_StaffNameTH END TextField  ,staff.slm_UserName  AS ValueField 
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff 
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position po on staff.slm_Position_id = po.slm_Position_id 
                            INNER JOIN (
                                            SELECT DISTINCT Z.slm_BranchCode,Z.slm_StaffTypeId  
                                            FROM (
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_access_right AR INNER JOIN kkslm_ms_campaign CAM ON CAM.slm_CampaignId = AR.slm_CampaignId 
                                                    WHERE CAM.slm_CampaignId = '{1}' 
                                                    UNION ALL
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_access_right AR INNER JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
                                                    WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM " + SLMConstant.SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT 
													                            WHERE PR_CAMPAIGNID = '{1}')
                                                 ) AS Z
                                          ) AS A ON A.slm_BranchCode = STAFF.slm_BranchCode AND A.slm_StaffTypeId = STAFF.slm_StaffTypeId              
                            where staff.is_Deleted = 0 and staff.slm_BranchCode ='{0}' AND STAFF.slm_StaffTypeId NOT IN ({2})    
                            ORDER BY staff.slm_StaffNameTH";

            sql = string.Format(sql, branch, campaignId, SLMConstant.StaffType.ITAdministrator);

            return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
        }

        public List<ControlListData> GetStaffNotDummyAllDataByAccessRight(string campaignId, string branch, string userName = null)
        {
            string whr = "";
            if (userName != null) whr = " OR ( staff.slm_UserName = '" + userName.Replace("'", "''") + "' ) ";
            string sql = @"SELECT CASE WHEN PO.slm_PositionNameAbb IS NULL THEN  staff.slm_StaffNameTH  
                                  ELSE PO.slm_PositionNameAbb + ' - ' + STAFF.slm_StaffNameTH END TextField  ,staff.slm_UserName  AS ValueField 
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff 
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position po on staff.slm_Position_id = po.slm_Position_id 
                            INNER JOIN (
                                            SELECT DISTINCT Z.slm_BranchCode,Z.slm_StaffTypeId  
                                            FROM (
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_access_right AR INNER JOIN kkslm_ms_campaign CAM ON CAM.slm_CampaignId = AR.slm_CampaignId 
                                                    WHERE CAM.slm_CampaignId = '{1}' 
                                                    UNION ALL
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_access_right AR INNER JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
                                                    WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM " + SLMConstant.SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT 
													                            WHERE PR_CAMPAIGNID = '{1}')
                                                 ) AS Z
                                          ) AS A ON A.slm_BranchCode = STAFF.slm_BranchCode AND A.slm_StaffTypeId = STAFF.slm_StaffTypeId              
                            where (staff.is_Deleted = 0 " + whr +@" ) and staff.slm_BranchCode ='{0}' AND STAFF.slm_StaffTypeId NOT IN ({2}) AND (staff.slm_UserType = 'I' OR staff.slm_UserType is null)    
                            ORDER BY staff.is_Deleted desc, staff.slm_StaffNameTH";

            sql = string.Format(sql, branch, campaignId, SLMConstant.StaffType.ITAdministrator);

            return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
        }

        public void CalculateAmountJobOnHandForDropdownlist(string branchCode, List<ControlListData> source)
        {
            var amountJobList = GetAmountJobOnHandList(branchCode);
            foreach (ControlListData owner in source)
            {
                var staff = amountJobList.Where(p => p.Username == owner.ValueField).FirstOrDefault();
                if (staff != null)
                    owner.TextField += " (" + (staff.AmountOwner + staff.AmountDelegate).ToString() + " งาน)";
            }
        }

        private List<StaffAmountJobOnHand> GetAmountJobOnHandList(string branchCode)
        {
            string sql = @"SELECT staff.slm_UserName AS Username, 
	                            (
		                            SELECT COUNT(*) AS NUM
		                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead WITH (NOLOCK)
		                            WHERE lead.is_Deleted = 0 AND lead.slm_AssignedFlag = '1' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Owner = staff.slm_UserName AND staff.is_Deleted = 0 AND lead.slm_Delegate IS NULL) AS AmountOwner,
	                            (
		                            SELECT COUNT(*) AS NUM
		                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead WITH (NOLOCK)
		                            WHERE lead.is_Deleted = 0 AND lead.slm_Delegate_Flag = '0' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Delegate = staff.slm_UserName AND staff.is_Deleted = 0) AS AmountDelegate
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff
                            WHERE staff.is_Deleted = 0 AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator.ToString() + "' AND staff.slm_BranchCode = '" + branchCode + "'";

            return slmdb.ExecuteStoreQuery<StaffAmountJobOnHand>(sql).ToList();
        }

        public bool CheckBranchAccessRightExist(int flag, string campaignId, string branchcode)
        {
            string sql = "";

            sql = @"SELECT branch.slm_BranchName AS TextField, branch.slm_BranchCode AS ValueField 
                    FROM kkslm_ms_branch branch  INNER JOIN (
                                    SELECT DISTINCT Z.slm_BranchCode,Z.slm_StaffTypeId  
                                    FROM (
                                            SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                            FROM kkslm_ms_access_right AR INNER JOIN kkslm_ms_campaign CAM ON CAM.slm_CampaignId = AR.slm_CampaignId 
                                            WHERE CAM.slm_CampaignId = '{1}' 
                                            UNION ALL
                                            SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                            FROM kkslm_ms_access_right AR INNER JOIN CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
                                            WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT 
						                                                WHERE PR_CAMPAIGNID = '{1}')
                                         ) AS Z
                                  ) AS A ON A.slm_BranchCode = branch.slm_BranchCode
                     WHERE  branch.slm_BranchCode = '{2}' {0}
                    ORDER BY slm_BranchName                 
                    ";

            string wh = "";

            if (flag == SLMConstant.Branch.Active)
            {
                wh = " AND branch.is_Deleted = 0 ";
            }
            else if (flag == SLMConstant.Branch.InActive)
            {
                wh = " AND branch.is_Deleted = 1 ";
            }
            else if (flag == SLMConstant.Branch.All)
            {
                wh = "";
            }
            else
                wh = "";

            sql = string.Format(sql, wh, campaignId, branchcode);

            var list = slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            if (list.Count > 0)
                return true;
            else
                return false;
        }

        public string GetBranchName(string branchCode)
        {
            var name = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == branchCode).Select(p => p.slm_BranchName).FirstOrDefault();
            return name != null ? name : "";
        }

        public void CheckOwnerPrivilege(string owner, string delegateLead, DropDownList cmbOwnerBranch, DropDownList cmbOwner, DropDownList cmbDelegateBranch, DropDownList cmbDelegateLead, string username)
        {
            try
            {
                bool privilegeOwner = false;
                cmbDelegateBranch.Enabled = false;
                cmbDelegateLead.Enabled = false;
                cmbOwnerBranch.Enabled = false;
                cmbOwner.Enabled = false;

                List<string> recusiveList = GetRecursiveStaffList(username);
                if (cmbOwner.Items.Count > 0 && !string.IsNullOrEmpty(owner))
                {
                    //ถ้าเป็น Owner หรือหัวหน้า Owner จะเปิดให้สามารถแก้ไข Owner และ Delegate ได้
                    if (recusiveList.Contains(owner.ToLower()))
                    {
                        privilegeOwner = true;
                        cmbOwner.Enabled = true;
                        cmbOwnerBranch.Enabled = true;
                        cmbDelegateLead.Enabled = true;
                        cmbDelegateBranch.Enabled = true;
                    }
                    else
                    {
                        cmbOwner.Enabled = false;
                        cmbOwnerBranch.Enabled = false;
                    }
                }

                if (!privilegeOwner)
                {
                    if (cmbDelegateLead.Items.Count > 0 && !string.IsNullOrEmpty(delegateLead))
                    {
                        //ถ้าเป็น Delegate หรือหัวหน้า Delegate จะเปิดให้สามารถแก้ไข Delegate ได้
                        if (recusiveList.Contains(delegateLead.ToLower()))
                        {
                            cmbDelegateLead.Enabled = true;
                            cmbDelegateBranch.Enabled = true;
                        }
                        else
                        {
                            cmbDelegateLead.Enabled = false;
                            cmbDelegateBranch.Enabled = false;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetRecursiveStaffList(string username)
        {
            try
            {
                //List<StaffData> staffList = slmdb.kkslm_ms_staff.Where(p => p.is_Deleted == 0).Select(p => new StaffData { UserName = p.slm_UserName, StaffId = p.slm_StaffId, HeadStaffId = p.slm_HeadStaffId }).ToList();
                List<StaffData> staffList = slmdb.kkslm_ms_staff.Select(p => new StaffData { UserName = p.slm_UserName, StaffId = p.slm_StaffId, HeadStaffId = p.slm_HeadStaffId }).ToList();
                int? staffId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).Select(p => p.slm_StaffId).FirstOrDefault();

                List<string> list = new List<string>();
                list.Add(username.ToLower());    //เก็บ login staff

                FindStaffRecusive(staffId, list, staffList);

                return list;
            }
            catch
            {
                throw;
            }
        }

        private void FindStaffRecusive(int? headId, List<string> list, List<StaffData> staffList)
        {
            try
            {
                foreach (StaffData staff in staffList)
                {
                    if (staff.HeadStaffId == headId)
                    {
                        if (!string.IsNullOrEmpty(staff.UserName)) list.Add(staff.UserName.ToLower());
                        FindStaffRecusive(staff.StaffId, list, staffList);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetAssignedFlagAndDelegateFlag(string ticketId)
        {
            try
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
            catch
            {
                throw;
            }
        }

        public bool CheckRequireCardId(string statusCode)
        {
            try
            {
                var require = slmdb.kkslm_ms_config_close_job.Where(p => p.slm_Status == statusCode && p.is_Deleted == false).Select(p => p.slm_RequireCardId).FirstOrDefault();
                return require != null ? require.Value : false;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckBranchActive(string branchCode)
        {
            try
            {
                var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == branchCode).FirstOrDefault();
                return branch != null ? (branch.is_Deleted ? false : true) : false;
            }
            catch
            {
                throw;
            }
        }

        public int InsertPhoneCallHistory(string ticketId, string cardType, string cardId, string leadStatusCode, string oldstatus, string ownerBranch, string owner, string oldOwner
            , string delegateLeadBranch, string delegateLead, string oldDelegateLead, string contactPhone, string contactDetail, string createBy, out string externalSubStatusDesc, string telNoSms, string countryId)
        {
            try
            {
                kkslm_tr_lead lead = null;
                _createdDate = DateTime.Now;
                 
                int? createdByPositionId = null;
                int? ownerPositionId = null;

                var cusinfo = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId == ticketId).FirstOrDefault();
                if (cusinfo != null)
                {
                    if (cardType != "")
                        cusinfo.slm_CardType = int.Parse(cardType);
                    else
                        cusinfo.slm_CardType = null;

                    string oldCitizenId = string.IsNullOrEmpty(cusinfo.slm_CitizenId) ? null : cusinfo.slm_CitizenId;
                    cusinfo.slm_CitizenId = cardId != "" ? cardId : null;

                    if (countryId != "")
                        cusinfo.slm_country_id = Convert.ToInt32(countryId);
                    else
                        cusinfo.slm_country_id = null;

                    if (cusinfo.slm_TelNoSms != telNoSms)
                    {
                        KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateTelNoSms, cusinfo.slm_TelNoSms, telNoSms, createBy, _createdDate);
                        cusinfo.slm_TelNoSms = telNoSms;
                    }

                    if (oldCitizenId != cusinfo.slm_CitizenId)
                    {
                        KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateCardId, oldCitizenId, cusinfo.slm_CitizenId, createBy, _createdDate);
                    }    
                }

                kkslm_phone_call phone = new kkslm_phone_call();
                phone.slm_TicketId = ticketId;
                phone.slm_ContactPhone = contactPhone;
                phone.slm_ContactDetail = contactDetail;
                phone.slm_Status = leadStatusCode;
                phone.slm_Owner = owner;
                ownerPositionId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == owner).Select(p => p.slm_Position_id).FirstOrDefault(); //GetPositionId(owner);
                phone.slm_Owner_Position = ownerPositionId;
                phone.slm_CreateDate = _createdDate;
                phone.slm_CreateBy = createBy;
                createdByPositionId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == createBy).Select(p => p.slm_Position_id).FirstOrDefault(); //GetPositionId(createBy);
                phone.slm_CreatedBy_Position = createdByPositionId;
                phone.is_Deleted = 0;
                slmdb.kkslm_phone_call.AddObject(phone);

                if (lead == null)
                    lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();

                if (leadStatusCode != oldstatus)
                {
                    kkslm_tr_activity activity = new kkslm_tr_activity();
                    activity.slm_TicketId = ticketId;
                    activity.slm_OldStatus = oldstatus;
                    activity.slm_NewStatus = leadStatusCode;
                    activity.slm_CreatedBy = createBy;
                    activity.slm_CreatedBy_Position = createdByPositionId;
                    activity.slm_CreatedDate = _createdDate;
                    activity.slm_Type = SLMConstant.ActionType.ChangeStatus;                //02
                    activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                    activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)

                    activity.slm_ExternalSystem_Old = lead.slm_ExternalSystem;                //add 14/10/2015
                    activity.slm_ExternalStatus_Old = lead.slm_ExternalStatus;                //add 14/10/2015
                    activity.slm_ExternalSubStatus_Old = lead.slm_ExternalSubStatus;          //add 14/10/2015
                    activity.slm_ExternalSubStatusDesc_Old = lead.slm_ExternalSubStatusDesc;  //add 14/10/2015

                    lead.slm_ExternalSystem = null;             //add 14/10/2015
                    lead.slm_ExternalStatus = null;             //add 14/10/2015
                    lead.slm_ExternalSubStatus = null;          //add 14/10/2015
                    lead.slm_ExternalSubStatusDesc = null;      //add 14/10/2015

                    slmdb.kkslm_tr_activity.AddObject(activity);

                    if (lead == null)
                        lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();

                    if (lead != null)
                    {
                        CalculateTotalSLA(slmdb, lead, activity, _createdDate);        //Method นี้ต้องอยู่ก่อนโค๊ดที่มีการเปลี่ยนแปลงค่าข้อมูล lead
                        lead.slm_Status = leadStatusCode;
                        lead.slm_StatusBy = createBy;
                        lead.slm_StatusDate = _createdDate;
                        lead.slm_StatusDateSource = _createdDate;     //add 16/03/2016
                        lead.slm_Counting = 0;
                        lead.slm_UpdatedBy = createBy;
                        lead.slm_UpdatedDate = _createdDate;
                    }

                    KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateStatus, oldstatus, leadStatusCode, createBy, _createdDate);
                }

                if (owner != oldOwner)
                {
                    kkslm_tr_activity activity = new kkslm_tr_activity();
                    activity.slm_TicketId = ticketId;
                    if (!string.IsNullOrEmpty(oldOwner))
                    {
                        activity.slm_OldOwner = oldOwner;
                        activity.slm_OldOwner_Position = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == oldOwner).Select(p => p.slm_Position_id).FirstOrDefault();   //GetPositionId(oldOwner);
                    }
                    activity.slm_NewOwner = owner;
                    activity.slm_NewOwner_Position = ownerPositionId;
                    activity.slm_CreatedBy = createBy;
                    activity.slm_CreatedBy_Position = createdByPositionId;
                    activity.slm_CreatedDate = _createdDate;
                    activity.slm_Type = SLMConstant.ActionType.ChangeOwner;
                    activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                    activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                    slmdb.kkslm_tr_activity.AddObject(activity);

                    if (lead == null)
                        lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();

                    if (lead != null)
                    {
                        lead.slm_StaffId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == owner).Select(p => p.slm_StaffId).FirstOrDefault();
                        lead.slm_Owner = owner;
                        lead.slm_Owner_Branch = ownerBranch;
                        lead.slm_Owner_Position = ownerPositionId;
                        lead.slm_AssignedFlag = "0";
                        lead.slm_AssignedDate = null;
                        lead.slm_AssignedBy = null;
                        lead.slm_OldOwner = oldOwner;
                    }

                    KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateOwner, oldOwner, owner, createBy, _createdDate);
                }
                else
                {
                    //NOTE: เมื่อมีการแก้ไข owner Lead กรณีที่เป็นเจ้าหน้าที่คนเดิม 20170530
                    lead.slm_Owner = owner;
                    lead.slm_Owner_Branch = ownerBranch;
                    lead.slm_Owner_Position = ownerPositionId;
                    //lead.slm_OldOwner = oldOwner;
                }

                if (delegateLead != oldDelegateLead)
                {
                    kkslm_tr_activity activity = new kkslm_tr_activity();
                    activity.slm_TicketId = ticketId;
                    if (!string.IsNullOrEmpty(oldDelegateLead))
                    {
                        activity.slm_OldDelegate = oldDelegateLead;
                        activity.slm_OldDelegate_Position = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == oldDelegateLead).Select(p => p.slm_Position_id).FirstOrDefault();    //GetPositionId(oldDelegateLead);
                    }
                    activity.slm_NewDelegate = delegateLead;

                    var delegatePosId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == delegateLead).Select(p => p.slm_Position_id).FirstOrDefault();
                    activity.slm_NewDelegate_Position = delegatePosId;
                    activity.slm_CreatedBy = createBy;
                    activity.slm_CreatedBy_Position = createdByPositionId;
                    activity.slm_CreatedDate = _createdDate;
                    activity.slm_Type = SLMConstant.ActionType.Delegate;
                    activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                    activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                    slmdb.kkslm_tr_activity.AddObject(activity);

                    if (lead == null)
                        lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();

                    if (lead != null)
                    {
                        lead.slm_Delegate_Flag = string.IsNullOrEmpty(delegateLead) ? 0 : 1;
                        if (!string.IsNullOrEmpty(delegateLead))
                            lead.slm_DelegateDate = _createdDate;
                        else
                            lead.slm_DelegateDate = null;

                        lead.slm_Delegate = string.IsNullOrEmpty(delegateLead) ? null : delegateLead;
                        lead.slm_Delegate_Branch = string.IsNullOrEmpty(delegateLead) ? null : delegateLeadBranch;

                        var id = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == delegateLead).Select(p => p.slm_Position_id).FirstOrDefault();
                        lead.slm_Delegate_Position = delegatePosId;
                    }

                    KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateDelegate, oldDelegateLead, delegateLead, createBy, _createdDate);
                }
                else
                {
                    lead.slm_Delegate = string.IsNullOrEmpty(delegateLead) ? null : delegateLead;
                    lead.slm_Delegate_Branch = string.IsNullOrEmpty(delegateLead) ? null : delegateLeadBranch;
                    lead.slm_Delegate_Position = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == delegateLead).Select(p => p.slm_Position_id).FirstOrDefault();
                }

                slmdb.SaveChanges();
                externalSubStatusDesc = lead.slm_ExternalSubStatusDesc;
                return phone.slm_PhoneCallId;
            }
            catch
            {
                throw;
            }
        }

        private void CalculateTotalSLA(SLM_DBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_activity act, DateTime createdDate)
        {
            try
            {
                //Initial variables
                DateTime currentSla = new DateTime();
                DateTime currentDate = DeleteSeconds(createdDate);
                int thisWork = lead.slm_ThisWork != null ? Convert.ToInt32(lead.slm_ThisWork) : 0;
                int slaCounting = lead.slm_Counting != null ? Convert.ToInt32(lead.slm_Counting.Value) : 0;

                int workingMinPerDay = 0;
                int startTimeHour = 0;
                int startTimeMin = 0;
                int endTimeHour = 0;
                int endTimeMin = 0;

                var calendarTab = slmdb.kkslm_ms_calendar_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch && p.is_Deleted == false).ToList();
                var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch).FirstOrDefault();

                if (branch == null) { throw new Exception("ไม่พบข้อมูลสาขา BranchCode:" + lead.slm_Owner_Branch); }

                if (string.IsNullOrEmpty(branch.slm_StartTime_Hour) || string.IsNullOrEmpty(branch.slm_StartTime_Minute)
                    || string.IsNullOrEmpty(branch.slm_EndTime_Hour) || string.IsNullOrEmpty(branch.slm_EndTime_Minute))
                {
                    string start = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "startTime").Select(p => p.slm_OptionDesc).FirstOrDefault();
                    string end = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "endTime").Select(p => p.slm_OptionDesc).FirstOrDefault();

                    if (start != null)
                    {
                        string[] str = start.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            startTimeHour = Convert.ToInt32(str[0]);
                            startTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                            throw new Exception("ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);
                    }
                    else
                        throw new Exception("ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);

                    if (end != null)
                    {
                        string[] str = end.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            endTimeHour = Convert.ToInt32(str[0]);
                            endTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                            throw new Exception("ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                    }
                    else
                        throw new Exception("ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                }
                else
                {
                    startTimeHour = int.Parse(branch.slm_StartTime_Hour);
                    endTimeMin = int.Parse(branch.slm_StartTime_Minute);
                    endTimeHour = int.Parse(branch.slm_EndTime_Hour);
                    endTimeMin = int.Parse(branch.slm_EndTime_Minute);

                    DateTime tmpStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startTimeHour, startTimeMin, 0);
                    DateTime tmpEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endTimeHour, endTimeMin, 0);

                    TimeSpan tmpTs = tmpEnd.Subtract(tmpStart);
                    workingMinPerDay = Convert.ToInt32(tmpTs.TotalMinutes);     //ได้เวลาที่ต้องทำงานในแต่ละวันของสาขา หน่วยเป็นนาที
                }

                if (slaCounting == 0)    //กรณีทำงานเสร็จก่อน sla เตือน
                {
                    //เผื่อในกรณี slm_StatusDate เป็น null, ซึ่งน่าจะไม่มีโอกาสเกิดเคสนี้ ยกเว้น คีย์ข้อมูลลงเบสตรง
                    currentSla = lead.slm_StatusDate != null ? DeleteSeconds(lead.slm_StatusDate.Value) : currentDate;
                }
                else
                {
                    currentSla = lead.slm_CurrentSLA != null ? DeleteSeconds(lead.slm_CurrentSLA.Value) : currentDate;
                }

                //ปรับเวลาในกรณี currentSla อยู่นอกเวลาทำงาน, หรือถ้าปรับแล้วยังไปตกวันหยุดก็ต้องให้เลื่อนไปจนถึงวันทำงาน
                int checkStartTime = Convert.ToInt32(startTimeHour.ToString("00") + startTimeMin.ToString("00"));
                int checkEndTime = Convert.ToInt32(endTimeHour.ToString("00") + endTimeMin.ToString("00"));
                int timeToCheck = Convert.ToInt32(currentSla.Hour.ToString("00") + currentSla.Minute.ToString("00"));

                if (timeToCheck < checkStartTime || timeToCheck > checkEndTime)     //ถ้าเวลาของ currentSla ไม่ได้อยู่ในช่วงเวลาทำงาน
                {
                    if (timeToCheck >= checkEndTime && timeToCheck <= 2359)
                    {
                        currentSla = currentSla.AddDays(1);
                        currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, startTimeHour, startTimeMin, 0);
                    }
                    else
                        currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, startTimeHour, startTimeMin, 0);
                }

                while (calendarTab.Where(p => p.slm_HolidayDate.Date == currentSla.Date).Count() > 0)
                {
                    currentSla = currentSla.AddDays(1);
                }

                //ปรับเวลาในกรณี currentDate อยู่นอกเวลาทำงาน, หรือถ้าปรับแล้วยังไปตกวันหยุดก็ต้องให้เลื่อนไปจนถึงวันทำงาน
                timeToCheck = Convert.ToInt32(currentDate.Hour.ToString("00") + currentDate.Minute.ToString("00"));

                if (timeToCheck < checkStartTime || timeToCheck > checkEndTime)     //ถ้าเวลาของ currentSla ไม่ได้อยู่ในช่วงเวลาทำงาน
                {
                    if (timeToCheck >= checkEndTime && timeToCheck <= 2359)
                    {
                        currentDate = currentDate.AddDays(1);
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);
                    }
                    else
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);
                }

                while (calendarTab.Where(p => p.slm_HolidayDate.Date == currentDate.Date).Count() > 0)
                {
                    currentDate = currentDate.AddDays(1);
                }

                //คำนวณ TotalAlert, TotalWork
                if (currentSla.Date == currentDate.Date)
                {
                    TimeSpan ts = currentDate.Subtract(currentSla);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);
                }
                else
                {
                    //หาวันที่อยู่ตรงกลางระหว่าง currentSla กับ currentDate
                    //เช่น currentSla = 12/04/2016, currentDate = 16/04/2016 ให้หาวันที่ 13, 14, 15 ออกมาเพื่อเช็กว่าเป็นวันทำงานหรือวันหยุด ถ้าเป็นวันทำงานให้เก็บจำนวนนาทีที่ต้องทำงานต่อวัน
                    DateTime startDate = currentSla.Date;
                    DateTime endDate = currentDate.Date;

                    DateTime tmpDate = startDate.AddDays(1);
                    while (tmpDate < endDate)
                    {
                        if (calendarTab.Where(p => p.slm_HolidayDate == tmpDate).Count() == 0)
                        {
                            thisWork += workingMinPerDay;
                        }
                        tmpDate = tmpDate.AddDays(1);
                    }

                    //ให้คำนวณเวลาที่เหลือในส่วนของวัน startDate และ endDate
                    DateTime endWorkTime_for_currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, endTimeHour, endTimeMin, 0);
                    DateTime startWorkTime_for_currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);

                    TimeSpan ts = endWorkTime_for_currentSla.Subtract(currentSla);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);

                    ts = currentDate.Subtract(startWorkTime_for_currentDate);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);
                }

                lead.slm_TotalAlert = (lead.slm_TotalAlert != null ? Convert.ToInt32(lead.slm_TotalAlert) : 0) + slaCounting;
                lead.slm_TotalWork = (lead.slm_TotalWork != null ? Convert.ToInt32(lead.slm_TotalWork) : 0) + thisWork;
                lead.slm_Counting = 0;
                lead.slm_ThisWork = 0;
                lead.slm_CurrentSLA = null;

                //OwnerLoggin
                act.slm_ThisAlert = slaCounting;
                act.slm_ThisWork = thisWork;
            }
            catch (Exception ex)
            {
                _calculateTotalSlaError = "CalculateTotalSLA Error=" + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        private DateTime DeleteSeconds(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
        }

        public string GetNewSubScriptionTypeId(int cardTypeId)
        {
            try
            {
                var subId = slmdb.kkslm_ms_cardtype.Where(p => p.slm_CardTypeId == cardTypeId && p.is_Deleted == false).Select(p => p.slm_CIFSubscriptTypeId).FirstOrDefault();
                return subId != null ? subId : "";
            }
            catch
            {
                throw;
            }
        }

        //public void GetDataForUpdate(string cardTypeId, string ticketId, out int subscriptionTypeId, out string externalSubstatusDesc)
        //{
        //    try
        //    {
        //        subscriptionTypeId = 0;
        //        if (cardTypeId != "")
        //        {
        //            var cardtype = Convert.ToInt32(cardTypeId);
        //            var subId = slmdb.kkslm_ms_cardtype.Where(p => p.slm_CardTypeId == cardtype && p.is_Deleted == false).Select(p => p.slm_CARSubscriptionTypeId).FirstOrDefault();
        //            subscriptionTypeId = subId != null ? subId.Value : 0;
        //        }
                    
        //        externalSubstatusDesc = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).Select(p => p.slm_ExternalSubStatusDesc).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public List<PhoneCallHistoryData> GetDefaultPhoneCallHistory(string citizenId, string ticketid, bool thisLead)
        {
            try
            {
                string slmDBName = SLMConstant.SLMDBName;
                string lead_sql = @"SELECT phone.slm_CreateDate AS CreatedDate, lead.slm_ticketId AS TicketId, lead.slm_Name AS Firstname, opt.slm_OptionDesc AS StatusDesc, 
                                cardtype.slm_CardTypeName AS CardTypeDesc,
                                info.slm_CitizenId AS CitizenId, info.slm_LastName AS LastName, phone.slm_ContactPhone AS ContactPhone, phone.slm_ContactDetail AS ContactDetail
                                ,CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                                  ELSE posowner.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS OwnerName
                                ,cam.slm_CampaignName AS CampaignName
                                ,CASE WHEN ISNULL(staff2.slm_StaffNameTH, phone.slm_CreateBy) = phone.slm_CreateBy THEN phone.slm_CreateBy
	                                  WHEN poscreateby.slm_PositionNameAbb IS NULL THEN staff2.slm_StaffNameTH
	                                  ELSE poscreateby.slm_PositionNameAbb + ' - ' + staff2.slm_StaffNameTH END AS CreatedName
                                , phone.slm_CreditFilePath AS CreditFilePath, phone.slm_CreditFileName AS CreditFileName
                                , phone.slm_50TawiFilePath AS Tawi50FilePath, phone.slm_50TawiFileName AS Tawi50FileName
                                , phone.slm_DriverLicenseFilePath AS DriverLicenseFilePath, phone.slm_DriverLicenseiFileName AS DriverLicenseiFileName
                                FROM " + slmDBName + @".dbo.kkslm_phone_call phone WITH (NOLOCK)
                                INNER JOIN " + slmDBName + @".dbo.kkslm_tr_lead lead WITH (NOLOCK) on phone.slm_TicketId = lead.slm_ticketId AND lead.is_Deleted = 0 
                                INNER JOIN " + slmDBName + @".dbo.kkslm_tr_cusinfo info WITH (NOLOCK) ON lead.slm_ticketId = info.slm_TicketId
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position posowner WITH (NOLOCK) ON phone.slm_Owner_Position = posowner.slm_Position_id
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff staff WITH (NOLOCK) ON phone.slm_Owner = staff.slm_UserName --AND staff.is_Deleted = 0
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_option opt WITH (NOLOCK) ON phone.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_campaign cam WITH (NOLOCK) ON lead.slm_CampaignId = cam.slm_CampaignId
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position poscreateby WITH (NOLOCK) ON phone.slm_CreatedBy_Position = poscreateby.slm_Position_id
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff staff2 WITH (NOLOCK) ON phone.slm_CreateBy = staff2.slm_UserName 
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) ON info.slm_CardType = cardtype.slm_CardTypeId ";

                if (thisLead)
                {
                    lead_sql += " WHERE phone.slm_TicketId = '" + ticketid + "' AND phone.is_Deleted = '0' ";
                }
                else
                {
                    if (!string.IsNullOrEmpty(citizenId))
                        lead_sql += " WHERE (info.slm_CitizenId = '" + citizenId + "' OR phone.slm_TicketId = '" + ticketid + @"') AND phone.is_Deleted = '0' ";
                    else
                        lead_sql += " WHERE phone.slm_TicketId = '" + ticketid + @"' AND phone.is_Deleted = '0' ";
                }

                lead_sql = lead_sql + " ORDER BY phone.slm_CreateDate desc";

                return slmdb.ExecuteStoreQuery<PhoneCallHistoryData>(lead_sql).ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<PhoneCallHistoryData> SearchNewPhoneCallHistory(string citizenId, string ticketid, bool thisLead, string preleadId)
        {
            if (string.IsNullOrEmpty(preleadId))
                preleadId = "0";

            //Prelead
            string prelead_sql = @"SELECT phone.slm_CreatedDate AS CreatedDate, '' AS TicketId, prelead.slm_Name AS Firstname, opt.slm_OptionDesc AS StatusDesc, 
	                                cardtype.slm_CardTypeName AS CardTypeDesc,
	                                prelead.slm_CitizenId AS CitizenId, prelead.slm_LastName AS LastName, phone.slm_ContactPhone AS ContactPhone, phone.slm_ContactDetail AS ContactDetail
	                                ,CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
			                                ELSE posowner.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS OwnerName
	                                ,cam.slm_CampaignName AS CampaignName
	                                ,CASE WHEN ISNULL(staff2.slm_StaffNameTH, phone.slm_CreatedBy) = phone.slm_CreatedBy THEN phone.slm_CreatedBy
			                                WHEN poscreateby.slm_PositionNameAbb IS NULL THEN staff2.slm_StaffNameTH
			                                ELSE poscreateby.slm_PositionNameAbb + ' - ' + staff2.slm_StaffNameTH END AS CreatedName
                                    , '' AS CreditFilePath, '' AS CreditFileName, '' AS Tawi50FilePath, '' AS Tawi50FileName, '' AS DriverLicenseFilePath, '' AS DriverLicenseFileName
	                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_phone_call phone WITH (NOLOCK)
	                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead prelead WITH (NOLOCK) on phone.slm_Prelead_Id = prelead.slm_Prelead_Id AND prelead.is_Deleted = 0
	                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position posowner ON phone.slm_Owner_Position = posowner.slm_Position_id
	                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff ON phone.slm_Owner = staff.slm_EmpCode  
	                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option opt ON phone.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
	                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam ON prelead.slm_CampaignId = cam.slm_CampaignId
	                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position poscreateby ON phone.slm_CreatedBy_Position = poscreateby.slm_Position_id
	                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff2 ON phone.slm_CreatedBy = staff2.slm_UserName 
	                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_cardtype cardtype ON prelead.slm_CardTypeId = cardtype.slm_CardTypeId ";

            if (thisLead)
            {
                prelead_sql += " WHERE phone.slm_Prelead_Id = '" + preleadId + "' ";
            }
            else
            {
                if (!string.IsNullOrEmpty(citizenId))
                    prelead_sql += " WHERE (prelead.slm_CitizenId = '" + citizenId + "' OR phone.slm_Prelead_Id = '" + preleadId + "') ";
                else
                    prelead_sql += " WHERE phone.slm_Prelead_Id = '" + preleadId + "' ";
            }

            //Lead
            string lead_sql = @"SELECT phone.slm_CreateDate AS CreatedDate, lead.slm_ticketId AS TicketId, lead.slm_Name AS Firstname, opt.slm_OptionDesc AS StatusDesc, 
                                cardtype.slm_CardTypeName AS CardTypeDesc,
                                info.slm_CitizenId AS CitizenId, info.slm_LastName AS LastName, phone.slm_ContactPhone AS ContactPhone, phone.slm_ContactDetail AS ContactDetail
                                ,CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                                  ELSE posowner.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS OwnerName
                                ,cam.slm_CampaignName AS CampaignName
                                ,CASE WHEN ISNULL(staff2.slm_StaffNameTH, phone.slm_CreateBy) = phone.slm_CreateBy THEN phone.slm_CreateBy
	                                  WHEN poscreateby.slm_PositionNameAbb IS NULL THEN staff2.slm_StaffNameTH
	                                  ELSE poscreateby.slm_PositionNameAbb + ' - ' + staff2.slm_StaffNameTH END AS CreatedName
                                , phone.slm_CreditFilePath AS CreditFilePath, phone.slm_CreditFileName AS CreditFileName
                                , phone.slm_50TawiFilePath AS Tawi50FilePath, phone.slm_50TawiFileName AS Tawi50FileName
                                , phone.slm_DriverLicenseFilePath AS DriverLicenseFilePath, phone.slm_DriverLicenseiFileName AS DriverLicenseiFileName
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_phone_call phone WITH (NOLOCK)
                                INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead WITH (NOLOCK) on phone.slm_TicketId = lead.slm_ticketId AND lead.is_Deleted = 0
                                INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_cusinfo info WITH (NOLOCK) ON lead.slm_ticketId = info.slm_TicketId
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position posowner ON phone.slm_Owner_Position = posowner.slm_Position_id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff ON phone.slm_Owner = staff.slm_UserName and staff.is_Deleted = 0 
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option opt ON phone.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam ON lead.slm_CampaignId = cam.slm_CampaignId
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position poscreateby ON phone.slm_CreatedBy_Position = poscreateby.slm_Position_id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff2 ON phone.slm_CreateBy = staff2.slm_UserName 
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_cardtype cardtype ON info.slm_CardType = cardtype.slm_CardTypeId ";

            if (thisLead)
            {
                lead_sql += " WHERE phone.slm_TicketId = '" + ticketid + "' AND phone.is_Deleted = '0' ";
            }
            else
            {
                if (!string.IsNullOrEmpty(citizenId))
                    lead_sql += " WHERE (info.slm_CitizenId = '" + citizenId + "' OR phone.slm_TicketId = '" + ticketid + @"') AND phone.is_Deleted = '0' ";
                else
                    lead_sql += " WHERE phone.slm_TicketId = '" + ticketid + @"' AND phone.is_Deleted = '0' ";
            }

            //Main SQL
            string mainSql = "SELECT A.* FROM ( {0} UNION ALL {1} ) A ORDER BY A.CreatedDate DESC ";
            mainSql = string.Format(mainSql, prelead_sql, lead_sql);

            return slmdb.ExecuteStoreQuery<PhoneCallHistoryData>(mainSql).ToList();
        }

        public List<ActivityConfigData> GetActivityConfig(string productId, string leadStatusCode)
        {
            return slmdb.kkslm_ms_config_activity.Where(p => p.slm_Product_Id == productId && p.slm_LeadStatus == leadStatusCode && p.is_Deleted == false)
                        .Select(p => new ActivityConfigData { ProductId = p.slm_Product_Id, LeadStatusCode = p.slm_LeadStatus, HaveRightAdd = p.slm_HaveRightAdd, LeadAvailableStatusCode = p.slm_AvailableStatus }).ToList();
        }

        public LeadDataForCARScript GetNewLeadDataForCAR(string ticketId)
        {
            string sql = @"SELECT lead.slm_ticketId AS TicketId, prelead.slm_Prelead_Id AS PreleadId, cus.slm_CardType AS CardType, cus.slm_CitizenId AS CitizenId
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_cusinfo cus ON cus.slm_TicketId = lead.slm_ticketId
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead prelead ON prelead.slm_TicketId = lead.slm_ticketId
                            WHERE lead.slm_ticketId = '" + ticketId + "' ";

            return slmdb.ExecuteStoreQuery<LeadDataForCARScript>(sql).FirstOrDefault();
        }

        public string GetTelNoSms(string ticketId)
        {
            var sms = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId == ticketId).Select(p => p.slm_TelNoSms).FirstOrDefault();
            return sms != null ? sms : "";
        }


        //============== Static Method ====================================================================================

        public static List<ControlListData> GetLeadStatus(string optionType)
        {
            KKSlmMsOptionModel option = new KKSlmMsOptionModel();
            return option.GetOptionList(optionType);
        }

        //public static List<ControlListData> GetLeadStatusForActivity(string optionType)
        //{
        //    KKSlmMsOptionModel option = new KKSlmMsOptionModel();
        //    return option.GetOptionListForActivity(optionType);
        //}

        public static List<PhoneCallHistoryData> SearchPhoneCallHistory(string citizenId, string ticketid, bool thisLead, string preleadId)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchPhoneCallHistory(citizenId, ticketid, thisLead, preleadId);
        }

        //public static int InsertPhoneCallHistory(string ticketId, string cardType, string cardId, string leadStatusCode, string oldstatus, string ownerBranch, string owner, string oldOwner, string delegateLeadBranch, string delegateLead, string oldDelegateLead, string contactPhone, string contactDetail, string createBy)
        //{
        //    KKSlmPhoneCallModel phone = new KKSlmPhoneCallModel();
        //    return phone.InsertPhoneCallHistory(ticketId, cardType, cardId, leadStatusCode, oldstatus, ownerBranch, owner, oldOwner, delegateLeadBranch, delegateLead, oldDelegateLead, contactPhone, contactDetail, createBy);
        //}

        

        public static string GetLeadStatusByTicketId(string ticketId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetLeadStatus(ticketId);
        }

        public static LeadData GetStatusAndAssignFlag(string ticketId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetStatusAndAssignFlag(ticketId); 
        }

        //Added by Pom 21/05/2016
        public static void UpdateCasFlag(int phonecallId, string flag)
        {
            new KKSlmPhoneCallModel().UpdateCasFlag(phonecallId, flag);
        }

        public static int InsertPhonecallEcm(string ticketId, string invoiceFilePath, string invoiceFileName, int invoiceFileSize, string createByUsername)
        {
            return new KKSlmPhoneCallModel().InsertPhonecallEcm(ticketId, invoiceFilePath, invoiceFileName, invoiceFileSize, createByUsername);
        }

        public static List<PhoneCallHistoryData> InsertPhonecallEcm(string[] ticketIdList, string invoiceFilePath, string invoiceFileName, int invoiceFileSize, string createByUsername, string mainId)
        {
            return new KKSlmPhoneCallModel().InsertPhonecallEcm(ticketIdList, invoiceFilePath, invoiceFileName, invoiceFileSize, createByUsername, mainId);
        }

        public static LeadDataForCARScript GetLeadDataForCAR(string ticketId)
        {
            return new KKSlmTrLeadModel().GetLeadDataForCAR(ticketId);
        }

        public static string GetSubScriptionTypeId(int cardTypeId)
        {
            return new KKSlmMsCardTypeModel().GetSubScriptionTypeId(cardTypeId);
        }

        public static int InsertRenewPhoneCallHistory(string ticketId, string cardType, string cardId, string leadStatusCode, string oldstatus, string ownerBranch, string owner, string oldOwner, string delegateLeadBranch, string delegateLead, string oldDelegateLead, string contactPhone, string contactDetail, string createBy, string subStatusCode, string subStatusDesc, bool Need_CompulsoryFlag, bool Need_PolicyFlag, bool Need_CompulsoryFlagOld, bool Need_PolicyFlagOld
                  , string slm_CreditFileName,
                  string slm_CreditFilePath,
                  int? slm_CreditFileSize,
                  string slm_50TawiFileName,
                  string slm_50TawiFilePath,
                  int? slm_50TawiFileSize,
                  string slm_DriverLicenseiFileName,
                  string slm_DriverLicenseFilePath,
                  int? slm_DriverLicenseFileSize,
                  DateTime? slm_NextContactDate,
                  decimal? slm_cp_blacklist_id,
                    bool Blacklist, out DateTime createdDate)
        {
            KKSlmPhoneCallModel phone = new KKSlmPhoneCallModel();
            int phonecallId = phone.InsertRenewPhoneCallHistory(ticketId, cardType, cardId, leadStatusCode, oldstatus, ownerBranch, owner, oldOwner, delegateLeadBranch, delegateLead, oldDelegateLead, contactPhone, contactDetail, createBy, subStatusCode, subStatusDesc, Need_CompulsoryFlag, Need_PolicyFlag, Need_CompulsoryFlagOld, Need_PolicyFlagOld,
                                slm_CreditFileName,
                                 slm_CreditFilePath,
                                 slm_CreditFileSize,
                                 slm_50TawiFileName,
                                 slm_50TawiFilePath,
                                 slm_50TawiFileSize,
                                 slm_DriverLicenseiFileName,
                                 slm_DriverLicenseFilePath,
                                 slm_DriverLicenseFileSize, slm_NextContactDate,
                                 slm_cp_blacklist_id,
                                 Blacklist);

            createdDate = phone.CreatedDate;
            return phonecallId;
        }
    }
}
