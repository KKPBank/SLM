using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Transactions;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class RenewInsureBiz : IDisposable
    {
        private SLM_DBEntities slmdb = null;
        private string _calculateTotalSlaError = "";

        public RenewInsureBiz()
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

        public string CalculateTotalSlaError
        {
            get { return _calculateTotalSlaError; }
        }

        public void CloseConnection()
        {
            if (slmdb != null)
            {
                slmdb.Connection.Close();
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
							, (select top 1 slm_cp_blacklist_id from dbo.kkslm_ms_config_product_blacklist pdb where lead.slm_ticketId = pdb.slm_ticketId
									AND pdb.slm_CitizenId = cus.slm_CitizenId AND slm_IsActive = 1 AND 
							CONVERT(DATE,GETDATE()) BETWEEN CONVERT(DATE,slm_StartDate) AND CONVERT(DATE,slm_EndDate)) blacklist
                            , prelead.slm_Prelead_Id AS PreleadId, lead.slm_ChannelId AS ChannelId, lead.slm_Product_Group_Id AS ProductGroupId
                            , pg.product_name AS ProductGroupName, mp.sub_product_name AS ProductName, cus.slm_TelNoSms AS TelNoSms, cus.slm_country_id CountryId
                            FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) 
                            LEFT JOIN dbo.kkslm_tr_renewinsurance tre WITH (NOLOCK) ON tre.slm_ticketId = lead.slm_ticketId 
                            LEFT JOIN dbo.kkslm_tr_cusinfo cus WITH (NOLOCK) ON lead.slm_ticketId = cus.slm_TicketId 
                            LEFT JOIN dbo.kkslm_ms_campaign cam WITH (NOLOCK) ON lead.slm_CampaignId = cam.slm_CampaignId
                            LEFT JOIN dbo.CMT_MS_PRODUCT_GROUP pg WITH (NOLOCK) ON lead.slm_Product_Group_Id = pg.product_id
                            LEFT JOIN dbo.CMT_MAPPING_PRODUCT mp WITH (NOLOCK) ON mp.sub_product_id = lead.slm_Product_Id
                            LEFT JOIN dbo.kkslm_tr_prelead prelead WITH (NOLOCK) ON prelead.slm_TicketId = lead.slm_ticketId 
                            LEFT JOIN dbo.kkslm_ms_title title WITH (NOLOCK) ON title.slm_TitleId = lead.slm_TitleId AND title.is_Deleted = 0
                            WHERE lead.slm_ticketId = '" + ticketId + "'";

            return slmdb.ExecuteStoreQuery<LeadDataPhoneCallHistory>(sql).FirstOrDefault();
        }

        public bool checkBlackList(string Username)
        {
            string sql = @"SELECT count(1) FROM dbo.kkslm_ms_staff WITH (NOLOCK) WHERE slm_StaffTypeId in ('7','15') and is_Deleted = 0 and slm_UserName = '" + Username + "'";
            return slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault() > 0 ? true : false;
        }

        public List<ControlListData> GetCardTypeList()
        {
            return slmdb.kkslm_ms_cardtype.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_CardTypeId).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_CardTypeName, ValueField = p.slm_CardTypeId.ToString() }).ToList();
        }

        public List<ControlListData> GetBranchAccessRightList(int flag, string campaignId)
        {
            string sql = @"SELECT branch.slm_BranchName AS TextField, branch.slm_BranchCode AS ValueField 
                            FROM kkslm_ms_branch branch WITH (NOLOCK) INNER JOIN (
                                            SELECT DISTINCT Z.slm_BranchCode  
                                            FROM (
                                                    SELECT AR.slm_BranchCode
                                                    FROM kkslm_ms_access_right AR WITH (NOLOCK) 
                                                    INNER JOIN kkslm_ms_campaign CAM WITH (NOLOCK) ON CAM.slm_CampaignId = AR.slm_CampaignId 
                                                    WHERE CAM.slm_CampaignId = '{1}' 
                                                    UNION ALL
                                                    SELECT AR.slm_BranchCode
                                                    FROM kkslm_ms_access_right AR WITH (NOLOCK) 
                                                    INNER JOIN CMT_CAMPAIGN_PRODUCT CP WITH (NOLOCK) ON CP.PR_ProductId = AR.slm_Product_Id
                                                    WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WITH (NOLOCK) 
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
            string sql = $@"SELECT ca.slm_AvailableStatus AS ValueField, opt2.slm_OptionDesc AS TextField
                            FROM dbo.kkslm_ms_config_activity ca WITH (NOLOCK)
                            LEFT JOIN dbo.kkslm_ms_option opt2 WITH (NOLOCK) ON ca.slm_AvailableStatus = opt2.slm_OptionCode AND opt2.slm_OptionType = 'lead status'  
                            WHERE ca.is_Deleted = 0 AND ca.slm_Product_Id = '{productId}' AND ca.slm_LeadStatus = '{leadStatus}' ";

            return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
        }

        public List<ConfigProductSubStatusData> GetSubStatusListByStatusActivityConfig(string productId, string campaignId, List<string> statusValueList)
        {
            try
            {
                string sql = "";
                string strStatusValueList = "";
                List<ConfigProductSubStatusData> list = new List<ConfigProductSubStatusData>();

                if (statusValueList.Count > 0)
                {
                    if (string.IsNullOrEmpty(productId))
                    {
                        sql = "SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WITH (NOLOCK) WHERE PR_CampaignId = '" + campaignId + "'";
                        productId = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
                    }

                    foreach (string data in statusValueList)
                    {
                        strStatusValueList += (strStatusValueList != "" ? "," : "") + "'" + data + "'";
                    }

                    sql = @"SELECT substatus.slm_SubStatusName AS SubStatusName, substatus.slm_SubStatusCode AS SubStatusCode, substatus.slm_OptionCode AS StatusCode
                                FROM dbo.kkslm_ms_config_product_substatus substatus WITH (NOLOCK) 
                                WHERE substatus.is_Deleted = 0 AND substatus.slm_CampaignId = '" + campaignId + @"'
                                AND substatus.slm_OptionCode IN (" + strStatusValueList + @")
                                ORDER BY substatus.slm_SubStatusName ";

                    list = slmdb.ExecuteStoreQuery<ConfigProductSubStatusData>(sql).ToList();
                    if (list.Count == 0)
                    {
                        sql = @"SELECT substatus.slm_SubStatusName AS SubStatusName, substatus.slm_SubStatusCode AS SubStatusCode, substatus.slm_OptionCode AS StatusCode
                                FROM dbo.kkslm_ms_config_product_substatus substatus WITH (NOLOCK)
                                WHERE substatus.is_Deleted = 0 AND substatus.slm_Product_Id = '" + productId + @"'
                                AND substatus.slm_OptionCode IN (" + strStatusValueList + @")
                                ORDER BY substatus.slm_SubStatusName ";

                        list = slmdb.ExecuteStoreQuery<ConfigProductSubStatusData>(sql).ToList();
                    }
                }

                return list;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckBranchAccessRightExist(int flag, string campaignId, string branchcode)
        {
            string sql = "";

            sql = @"SELECT branch.slm_BranchName AS TextField, branch.slm_BranchCode AS ValueField 
                    FROM kkslm_ms_branch branch WITH (NOLOCK) INNER JOIN (
                                    SELECT DISTINCT Z.slm_BranchCode,Z.slm_StaffTypeId  
                                    FROM (
                                            SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                            FROM kkslm_ms_access_right AR WITH (NOLOCK) 
                                            INNER JOIN kkslm_ms_campaign CAM WITH (NOLOCK) ON CAM.slm_CampaignId = AR.slm_CampaignId 
                                            WHERE CAM.slm_CampaignId = '{1}' 
                                            UNION ALL
                                            SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                            FROM kkslm_ms_access_right AR WITH (NOLOCK) 
                                            INNER JOIN CMT_CAMPAIGN_PRODUCT CP WITH (NOLOCK) ON CP.PR_ProductId = AR.slm_Product_Id
                                            WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WITH (NOLOCK) 
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

        public List<ControlListData> GetStaffAllDataByAccessRight(string campaignId, string branch)
        {
            string sql = @"SELECT CASE WHEN PO.slm_PositionNameAbb IS NULL THEN  staff.slm_StaffNameTH  
                                  ELSE PO.slm_PositionNameAbb + ' - ' + STAFF.slm_StaffNameTH END TextField  ,staff.slm_UserName  AS ValueField 
                            FROM dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                            LEFT JOIN dbo.kkslm_ms_position po WITH (NOLOCK) on staff.slm_Position_id = po.slm_Position_id 
                            INNER JOIN (
                                            SELECT DISTINCT Z.slm_BranchCode,Z.slm_StaffTypeId  
                                            FROM (
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM dbo.kkslm_ms_access_right AR WITH (NOLOCK) 
                                                    INNER JOIN kkslm_ms_campaign CAM WITH (NOLOCK) ON CAM.slm_CampaignId = AR.slm_CampaignId 
                                                    WHERE CAM.slm_CampaignId = '{1}' 
                                                    UNION ALL
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM dbo.kkslm_ms_access_right AR WITH (NOLOCK) 
                                                    INNER JOIN dbo.CMT_CAMPAIGN_PRODUCT CP WITH (NOLOCK) ON CP.PR_ProductId = AR.slm_Product_Id
                                                    WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM dbo.CMT_CAMPAIGN_PRODUCT WITH (NOLOCK) 
													                            WHERE PR_CAMPAIGNID = '{1}')
                                                 ) AS Z
                                          ) AS A ON A.slm_BranchCode = STAFF.slm_BranchCode AND A.slm_StaffTypeId = STAFF.slm_StaffTypeId              
                            where staff.is_Deleted = 0 and staff.slm_BranchCode ='{0}' AND STAFF.slm_StaffTypeId NOT IN ({2})    
                            ORDER BY staff.slm_StaffNameTH";

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
		                            FROM dbo.kkslm_tr_lead lead WITH (NOLOCK)
		                            WHERE lead.is_Deleted = 0 AND lead.slm_AssignedFlag = '1' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Owner = staff.slm_UserName AND lead.slm_Delegate IS NULL) AS AmountOwner,
	                            (
		                            SELECT COUNT(*) AS NUM
		                            FROM dbo.kkslm_tr_lead lead WITH (NOLOCK)
		                            WHERE lead.is_Deleted = 0 AND lead.slm_Delegate_Flag = '0' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Delegate = staff.slm_UserName) AS AmountDelegate
                            FROM dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                            WHERE staff.is_Deleted = 0 AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator.ToString() + "' AND staff.slm_BranchCode = '" + branchCode + "'";

            return slmdb.ExecuteStoreQuery<StaffAmountJobOnHand>(sql).ToList();
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
            //List<StaffData> staffList = slmdb.kkslm_ms_staff.Where(p => p.is_Deleted == 0).Select(p => new StaffData { UserName = p.slm_UserName, StaffId = p.slm_StaffId, HeadStaffId = p.slm_HeadStaffId }).ToList();
            //int? staffId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).Select(p => p.slm_StaffId).FirstOrDefault();

            string sql = $"SELECT slm_UserName AS UserName, slm_StaffId AS StaffId, slm_HeadStaffId AS HeadStaffId FROM kkslm_ms_staff WITH (NOLOCK) WHERE is_Deleted = 0 ";
            var staffList = slmdb.ExecuteStoreQuery<StaffData>(sql).ToList();

            sql = $"SELECT slm_StaffId FROM kkslm_ms_staff WITH (NOLOCK) WHERE slm_UserName = '{username}' ";
            int? staffId = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

            List<string> list = new List<string>();
            list.Add(username.ToLower());    //เก็บ login staff

            FindStaffRecusive(staffId, list, staffList);

            return list;
        }

        private void FindStaffRecusive(int? headId, List<string> list, List<StaffData> staffList)
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

        //public int InsertRenewPhoneCallHistory(string ticketId, string cardType, string cardId, string leadStatusCode, string oldstatus, string ownerBranch, string owner, string oldOwner, string delegateLeadBranch, string delegateLead, string oldDelegateLead, string contactPhone, string contactDetail, string createBy, string subStatusCode, string subStatusDesc, bool Need_CompulsoryFlag, bool Need_PolicyFlag, bool Need_CompulsoryFlagOld, bool Need_PolicyFlagOld
        //            , string slm_CreditFileName,
        //            string slm_CreditFilePath,
        //            int? slm_CreditFileSize,
        //            string slm_50TawiFileName,
        //            string slm_50TawiFilePath,
        //            int? slm_50TawiFileSize,
        //            string slm_DriverLicenseiFileName,
        //            string slm_DriverLicenseFilePath,
        //            int? slm_DriverLicenseFileSize,
        //            DateTime? slm_NextContactDate,
        //            decimal? slm_cp_blacklist_id,
        //              bool Blacklist, out DateTime createdDate, out string externalSubStatusDesc)

        public int InsertRenewPhoneCallHistory(RenewPhoneCallHistoryData model)
        {
            bool insertActReport = false;
            try
            {
                kkslm_tr_lead lead = null;
                model.createdDate = DateTime.Now;
                int? createdByPositionId = null;
                int? ownerPositionId = null;
                string oldSubStatus = string.Empty;
                string oldExternalSystem = string.Empty;
                string oldExternalStatus = string.Empty;
                string oldExternalSubStatus = string.Empty;
                string oldExternalSubStatusDesc = string.Empty;

                lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == model.ticketId).FirstOrDefault();
                if (lead == null) { throw new Exception("ไม่พบข้อมูลผู้มุ่งหวัง Ticket Id=" + model.ticketId + " ในระบบ"); }

                var cusinfo = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId == model.ticketId).FirstOrDefault();
                if (cusinfo != null)
                {
                    if (model.cardType != "")
                        cusinfo.slm_CardType = int.Parse(model.cardType);
                    else
                        cusinfo.slm_CardType = null;

                    if (model.countryId != "")
                        cusinfo.slm_country_id = Convert.ToInt32(model.countryId);
                    else
                        cusinfo.slm_country_id = null;

                    string oldCitizenId = string.IsNullOrEmpty(cusinfo.slm_CitizenId) ? null : cusinfo.slm_CitizenId;
                    cusinfo.slm_CitizenId = model.cardId != "" ? model.cardId : null;

                    if (cusinfo.slm_TelNoSms != model.TelNoSms)
                    {
                        KKSlmTrHistoryModel.InsertHistory(slmdb, model.ticketId, SLMConstant.HistoryTypeCode.UpdateTelNoSms, cusinfo.slm_TelNoSms, model.TelNoSms, model.createBy, model.createdDate);
                        cusinfo.slm_TelNoSms = model.TelNoSms;
                    }

                    if (oldCitizenId != cusinfo.slm_CitizenId)
                        KKSlmTrHistoryModel.InsertHistory(slmdb, model.ticketId, SLMConstant.HistoryTypeCode.UpdateCardId, oldCitizenId, cusinfo.slm_CitizenId, model.createBy, model.createdDate);

                    slmdb.SaveChanges();
                }

                kkslm_phone_call phone = new kkslm_phone_call();
                phone.slm_TicketId = model.ticketId;
                phone.slm_ContactPhone = model.contactPhone;
                phone.slm_ContactDetail = model.contactDetail;
                phone.slm_Status = model.leadStatusCode;
                phone.slm_SubStatus = model.subStatusCode;
                phone.slm_SubStatusName = model.subStatusDesc;            //Added By Pom 23/05/2016
                phone.slm_Owner = model.owner;
                ownerPositionId = GetPositionId(model.owner);
                phone.slm_Owner_Position = ownerPositionId;
                phone.slm_CreateDate = model.createdDate;
                phone.slm_CreateBy = model.createBy;
                createdByPositionId = GetPositionId(model.createBy);
                phone.slm_CreatedBy_Position = createdByPositionId;
                phone.is_Deleted = 0;
                phone.slm_CAS_Flag = null;

                var reins = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == model.ticketId).FirstOrDefault();

                phone.slm_Need_CompulsoryFlag = model.Need_CompulsoryFlag ? "Y" : "N";
                if (reins != null)
                {
                    reins.slm_Need_CompulsoryFlag = phone.slm_Need_CompulsoryFlag;
                }
                if (model.Need_CompulsoryFlag != model.Need_CompulsoryFlagOld)
                {
                    phone.slm_Need_CompulsoryFlagDate = DateTime.Now;

                    //report
                    //ActivityLeadModel leadModel = new ActivityLeadModel();
                    if (model.Need_CompulsoryFlag)
                    {
                        insertActReport = true;// insertkkslm_tr_actnew_report(reins.slm_TicketId, model.createBy);
                        //leadModel.insertkkslm_tr_actnew_report(reins.slm_TicketId, createBy);
                    }
                    else
                    {
                        deletekkslm_tr_actnew_report(reins.slm_TicketId, model.createBy);
                        //leadModel.deletekkslm_tr_actnew_report(reins.slm_TicketId, createBy);
                    }

                    //report

                    if (reins != null)
                    {
                        reins.slm_Need_CompulsoryFlagDate = DateTime.Now;
                    }
                }

                phone.slm_Need_PolicyFlag = model.Need_PolicyFlag ? "Y" : "N";
                if (reins != null)
                {
                    reins.slm_Need_PolicyFlag = phone.slm_Need_PolicyFlag;
                }
                if (model.Need_PolicyFlag != model.Need_PolicyFlagOld)
                {
                    //ActivityLeadModel leadModel = new ActivityLeadModel();
                    //report
                    if (model.Need_PolicyFlag)
                    {
                        insertkkslm_tr_policynew_report(reins.slm_TicketId, model.createBy);

                        //leadModel.insertkkslm_tr_policynew_report(reins.slm_TicketId, createBy);
                    }
                    else
                    {
                        deletekkslm_tr_policynew_report(reins.slm_TicketId, model.createBy);

                        //leadModel.deletekkslm_tr_policynew_report(reins.slm_TicketId, createBy);
                    }

                    //report
                    phone.slm_Need_PolicyFlagDate = DateTime.Now;
                    if (reins != null)
                    {
                        reins.slm_Need_PolicyFlagDate = DateTime.Now;
                    }
                }

                if (!string.IsNullOrEmpty(model.slm_CreditFileName))
                {
                    phone.slm_CreditFileName = model.slm_CreditFileName;
                    phone.slm_CreditFilePath = model.slm_CreditFilePath;
                    if (model.slm_CreditFileSize != null)
                    {
                        phone.slm_CreditFileSize = model.slm_CreditFileSize;
                    }
                    if (reins != null)
                    {
                        reins.slm_CreditFileName = model.slm_CreditFileName;
                        reins.slm_CreditFilePath = model.slm_CreditFilePath;
                        if (model.slm_CreditFileSize != null)
                        {
                            reins.slm_CreditFileSize = model.slm_CreditFileSize;
                        }
                        reins.slm_Need_CreditFlag = "N";
                    }
                }

                if (!string.IsNullOrEmpty(model.slm_50TawiFileName))
                {
                    phone.slm_50TawiFileName = model.slm_50TawiFileName;
                    phone.slm_50TawiFilePath = model.slm_50TawiFilePath;
                    if (model.slm_50TawiFileSize != null)
                    {
                        phone.slm_50TawiFileSize = model.slm_50TawiFileSize;
                    }
                    if (reins != null)
                    {
                        reins.slm_50TawiFileName = model.slm_50TawiFileName;
                        reins.slm_50TawiFilePath = model.slm_50TawiFilePath;
                        if (model.slm_50TawiFileSize != null)
                        {
                            reins.slm_50TawiFileSize = model.slm_50TawiFileSize;
                        }
                        reins.slm_Need_50TawiFlag = "N";
                    }
                }

                if (!string.IsNullOrEmpty(model.slm_DriverLicenseiFileName))
                {
                    phone.slm_DriverLicenseiFileName = model.slm_DriverLicenseiFileName;
                    phone.slm_DriverLicenseFilePath = model.slm_DriverLicenseFilePath;
                    if (model.slm_DriverLicenseFileSize != null)
                    {
                        phone.slm_DriverLicenseFileSize = model.slm_DriverLicenseFileSize;
                    }
                    if (reins != null)
                    {
                        reins.slm_DriverLicenseiFileName = model.slm_DriverLicenseiFileName;
                        reins.slm_DriverLicenseFilePath = model.slm_DriverLicenseFilePath;
                        if (model.slm_DriverLicenseFileSize != null)
                        {
                            reins.slm_DriverLicenseFileSize = model.slm_DriverLicenseFileSize;
                        }
                        reins.slm_Need_DriverLicenseFlag = "N";

                    }
                }

                if (reins != null)
                {
                    reins.slm_UpdatedBy = model.createBy;
                    reins.slm_UpdatedDate = DateTime.Now;
                }

                slmdb.kkslm_phone_call.AddObject(phone);
                //slmdb.SaveChanges();  //Comment 07/09/2016

                //black List
                if (model.slm_cp_blacklist_id != null)
                {
                    kkslm_ms_config_product_blacklist dataBlacklist = slmdb.kkslm_ms_config_product_blacklist.Where(p => p.slm_cp_blacklist_id == model.slm_cp_blacklist_id).FirstOrDefault();

                    dataBlacklist.slm_IsActive = false;
                    dataBlacklist.slm_UpdatedBy = model.createBy;
                    dataBlacklist.slm_UpdatedDate = DateTime.Now;
                    //slmdb.SaveChanges();      //Comment 07/09/2016
                }

                if (model.Blacklist)
                {
                    kkslm_ms_config_product_blacklist dataBlacklist = new kkslm_ms_config_product_blacklist();

                    kkslm_tr_lead dataPrelead = slmdb.kkslm_tr_lead.Where(r => r.slm_ticketId == model.ticketId).FirstOrDefault();

                    kkslm_tr_cusinfo cus = slmdb.kkslm_tr_cusinfo.Where(r => r.slm_TicketId == model.ticketId).FirstOrDefault();
                    if (dataPrelead != null)
                    {
                        kkslm_ms_config_product_day day = slmdb.kkslm_ms_config_product_day.Where(cd => cd.slm_Product_Id == dataPrelead.slm_Product_Id && cd.slm_Type == "BLACKLIST").FirstOrDefault();
                        dataBlacklist.slm_Product_Id = lead.slm_Product_Id;
                        dataBlacklist.slm_Prelead_Id = null;
                        dataBlacklist.slm_ticketId = model.ticketId;
                        dataBlacklist.slm_Name = dataPrelead.slm_Name;
                        dataBlacklist.slm_LastName = dataPrelead.slm_LastName;
                        dataBlacklist.slm_CardType = cus.slm_CardType;
                        dataBlacklist.slm_CitizenId = cus.slm_CitizenId;
                        dataBlacklist.slm_StartDate = DateTime.Now;
                        dataBlacklist.slm_EndDate = (DateTime?)DateTime.Now.AddDays(day.slm_Days.Value);
                        dataBlacklist.slm_IsActive = true;
                        dataBlacklist.slm_CreatedDate = DateTime.Now;
                        dataBlacklist.slm_CreatedBy = model.createBy;
                        dataBlacklist.slm_UpdatedDate = DateTime.Now;
                        dataBlacklist.slm_UpdatedBy = model.createBy;

                        slmdb.kkslm_ms_config_product_blacklist.AddObject(dataBlacklist);
                    }
                }

                //================= Update Lead Data Section ================================================================

                lead.slm_NextContactDate = model.slm_NextContactDate;

                //Add By Pom 23/05/2016 - เก็บค่าเก่า
                model.oldstatus = lead.slm_Status;
                oldSubStatus = lead.slm_SubStatus;
                oldExternalSystem = lead.slm_ExternalSystem;
                oldExternalStatus = lead.slm_ExternalStatus;
                oldExternalSubStatus = string.IsNullOrEmpty(lead.slm_ExternalSubStatus) ? null : lead.slm_ExternalSubStatus;
                oldExternalSubStatusDesc = lead.slm_ExternalSubStatusDesc;

                if (model.leadStatusCode != model.oldstatus || model.subStatusCode != oldSubStatus)
                {
                    kkslm_tr_activity activity = new kkslm_tr_activity()
                    {
                        slm_TicketId = model.ticketId,
                        slm_OldStatus = model.oldstatus,
                        slm_NewStatus = model.leadStatusCode,
                        slm_OldSubStatus = oldSubStatus,
                        slm_NewSubStatus = model.subStatusCode,
                        slm_CreatedBy = model.createBy,
                        slm_CreatedBy_Position = createdByPositionId,
                        slm_CreatedDate = model.createdDate,
                        slm_Type = SLMConstant.ActionType.ChangeStatus,                //02
                        slm_SystemAction = SLMConstant.SystemName,                     //System ที่เข้ามาทำ action (23/05/2015)
                        slm_SystemActionBy = SLMConstant.SystemName,                   //action เกิดขึ้นที่ระบบอะไร (23/05/2015)
                        slm_ExternalSystem_Old = oldExternalSystem,                //add 23/05/2015
                        slm_ExternalStatus_Old = oldExternalStatus,                //add 23/05/2015
                        slm_ExternalSubStatus_Old = oldExternalSubStatus,          //add 23/05/2015
                        slm_ExternalSubStatusDesc_Old = oldExternalSubStatusDesc,  //add 23/05/2015
                        slm_ExternalSystem_New = null,
                        slm_ExternalStatus_New = null,
                        slm_ExternalSubStatus_New = null,
                        slm_ExternalSubStatusDesc_New = model.subStatusDesc
                    };
                    slmdb.kkslm_tr_activity.AddObject(activity);

                    CalculateTotalSLA(slmdb, lead, activity, model.createdDate);        //Method นี้ต้องอยู่ก่อนโค๊ดที่มีการเปลี่ยนแปลงค่าข้อมูล lead
                    lead.slm_Status = model.leadStatusCode;
                    lead.slm_SubStatus = model.subStatusCode;
                    lead.slm_StatusBy = model.createBy;
                    lead.slm_StatusDate = model.createdDate;
                    lead.slm_StatusDateSource = model.createdDate;
                    lead.slm_Counting = 0;
                    lead.slm_ExternalSystem = null;                     //add 23/05/2015
                    lead.slm_ExternalStatus = null;                     //add 23/05/2015
                    lead.slm_ExternalSubStatus = null;                  //add 23/05/2015
                    lead.slm_ExternalSubStatusDesc = model.subStatusDesc;     //add 23/05/2015
                    lead.slm_UpdatedBy = model.createBy;
                    lead.slm_UpdatedDate = model.createdDate;
                    //slmdb.SaveChanges();      //Comment 07/09/2016

                    KKSlmTrHistoryModel.InsertHistory(slmdb, model.ticketId, SLMConstant.HistoryTypeCode.UpdateStatus, model.oldstatus, model.leadStatusCode, model.createBy, model.createdDate);
                }

                if (model.owner != model.oldOwner)
                {
                    kkslm_tr_activity activity = new kkslm_tr_activity();
                    activity.slm_TicketId = model.ticketId;
                    if (!string.IsNullOrEmpty(model.oldOwner))
                    {
                        activity.slm_OldOwner = model.oldOwner;
                        activity.slm_OldOwner_Position = GetPositionId(model.oldOwner);
                    }
                    activity.slm_NewOwner = model.owner;
                    activity.slm_NewOwner_Position = ownerPositionId;
                    activity.slm_CreatedBy = model.createBy;
                    activity.slm_CreatedBy_Position = createdByPositionId;
                    activity.slm_CreatedDate = model.createdDate;
                    activity.slm_Type = SLMConstant.ActionType.ChangeOwner;
                    activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                    activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                    slmdb.kkslm_tr_activity.AddObject(activity);

                    lead.slm_StaffId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == model.owner).Select(p => p.slm_StaffId).FirstOrDefault();
                    lead.slm_Owner = model.owner;
                    lead.slm_Owner_Branch = model.ownerBranch;
                    lead.slm_Owner_Position = ownerPositionId;
                    lead.slm_AssignedFlag = "0";
                    lead.slm_AssignedDate = null;
                    lead.slm_AssignedBy = null;
                    lead.slm_OldOwner = model.oldOwner;

                    KKSlmTrHistoryModel.InsertHistory(slmdb, model.ticketId, SLMConstant.HistoryTypeCode.UpdateOwner, model.oldOwner, model.owner, model.createBy, model.createdDate);
                }
                else
                {
                    lead.slm_Owner = model.owner;
                    lead.slm_Owner_Branch = model.ownerBranch;
                    lead.slm_Owner_Position = ownerPositionId;
                }

                //slmdb.SaveChanges();      //Comment 07/09/2016

                if (model.delegateLead != model.oldDelegateLead)
                {
                    kkslm_tr_activity activity = new kkslm_tr_activity();
                    activity.slm_TicketId = model.ticketId;
                    if (!string.IsNullOrEmpty(model.oldDelegateLead))
                    {
                        activity.slm_OldDelegate = model.oldDelegateLead;
                        activity.slm_OldDelegate_Position = GetPositionId(model.oldDelegateLead);
                    }
                    activity.slm_NewDelegate = model.delegateLead;
                    var delegatePosId = GetPositionId(model.delegateLead);
                    activity.slm_NewDelegate_Position = delegatePosId;
                    activity.slm_CreatedBy = model.createBy;
                    activity.slm_CreatedBy_Position = createdByPositionId;
                    activity.slm_CreatedDate = model.createdDate;
                    activity.slm_Type = SLMConstant.ActionType.Delegate;
                    activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                    activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                    slmdb.kkslm_tr_activity.AddObject(activity);

                    lead.slm_Delegate_Flag = string.IsNullOrEmpty(model.delegateLead) ? 0 : 1;
                    if (!string.IsNullOrEmpty(model.delegateLead))
                        lead.slm_DelegateDate = model.createdDate;
                    else
                        lead.slm_DelegateDate = null;

                    lead.slm_Delegate = string.IsNullOrEmpty(model.delegateLead) ? null : model.delegateLead;
                    lead.slm_Delegate_Branch = string.IsNullOrEmpty(model.delegateLead) ? null : model.delegateLeadBranch;
                    lead.slm_Delegate_Position = delegatePosId;

                    KKSlmTrHistoryModel.InsertHistory(slmdb, model.ticketId, SLMConstant.HistoryTypeCode.UpdateDelegate, model.oldDelegateLead, model.delegateLead, model.createBy, model.createdDate);
                }
                else
                {
                    lead.slm_Delegate = string.IsNullOrEmpty(model.delegateLead) ? null : model.delegateLead;
                    lead.slm_Delegate_Branch = string.IsNullOrEmpty(model.delegateLead) ? null : model.delegateLeadBranch;
                    lead.slm_Delegate_Position = GetPositionId(model.delegateLead);
                }

                if (model.Need_PolicyFlag != model.Need_PolicyFlagOld)
                {
                    #region Update kkslm_tr_policynew_report
                    kkslm_tr_policynew_report report = slmdb.kkslm_tr_policynew_report.Where(e => e.slm_TicketId == model.ticketId).FirstOrDefault();
                    if (report != null)
                    {
                        if (report.slm_Export_Flag != true && model.Need_PolicyFlag == true)
                        {
                           // report.slm_RepairTypeId = model.slm_RepairTypeId;
                            report.slm_ActStartCoverDate = model.slm_ActStartCoverDate;
                            report.slm_ActEndCoverDate = model.slm_ActEndCoverDate;
                            report.slm_Remark = model.Remark;
                            if (model.Address != null)
                            {
                                report.slm_House_No = model.Address.slm_House_No;
                                report.slm_Moo = model.Address.slm_Moo;
                                report.slm_Village = model.Address.slm_Village;
                                report.slm_Building = model.Address.slm_Building;
                                report.slm_Soi = model.Address.slm_Soi;
                                report.slm_Street = model.Address.slm_Street;
                                report.slm_TambolId = model.Address.slm_TambolId;
                                report.slm_AmphurId = model.Address.AmphurId;
                                report.slm_ProvinceId = model.Address.ProvinceId;
                                report.slm_Zipcode = model.Address.slm_Zipcode;
                            }
                        }
                        else if (report.slm_Export_Flag != true && model.Need_PolicyFlag == false)
                        {
                            report.is_Deleted = true;
                        }
                    }

                    //kkslm_tr_notify_renewinsurance_report NotifyRenewReport = slmdb.kkslm_tr_notify_renewinsurance_report.Where(e => e.slm_TicketId == model.ticketId).FirstOrDefault();
                    //if(NotifyRenewReport == null || NotifyRenewReport.)
                    //{
                    //    new kkslm_tr_notify_renewinsurance_report
                    //    {
                    //        slm_RepairTypeId = model.slm_RepairTypeId,
                    //       // slm_convert
                    //    };
                    //}
                    #endregion

                }

                slmdb.SaveChanges();
                model.externalSubStatusDesc = lead.slm_ExternalSubStatusDesc;

                // รอให้บันทึกข้อมูลก่อนค่อยใส่ข้อมูลลงใน รายงาน
                if (insertActReport) 
                    insertkkslm_tr_actnew_report(reins.slm_TicketId, model.createBy);

                return phone != null ? phone.slm_PhoneCallId : 0;
            }
            catch
            {
                throw;
            }
        }

        private int? GetPositionId(string username)
        {
            return slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).Select(p => p.slm_Position_id).FirstOrDefault();
        }

        public void CalculateTotalSLA(SLM_DBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_activity act, DateTime createdDate , DateTime? oldStatusDate = null)
        {
            try
            {
                //Initial variables
                DateTime currentSla = new DateTime();
                DateTime currentDate = DeleteSeconds(createdDate);
                //if (oldStatusDate.HasValue) lead.slm_StatusDate = oldStatusDate;   
                int thisWork = lead.slm_ThisWork != null ? Convert.ToInt32(lead.slm_ThisWork) : 0;
                int slaCounting = lead.slm_Counting != null ? Convert.ToInt32(lead.slm_Counting.Value) : 0;

                int workingMinPerDay = 0;
                int startTimeHour = 0;
                int startTimeMin = 0;
                int endTimeHour = 0;
                int endTimeMin = 0;

                var calendarTab = slmdb.kkslm_ms_calendar_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch && p.is_Deleted == false).ToList();
                var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch).FirstOrDefault();

                if (branch == null) { throw new Exception("ไม่พบข้อมูลสาขา BranchCode:" + lead.slm_Owner_Branch);  }

                if (string.IsNullOrEmpty(branch.slm_StartTime_Hour) || string.IsNullOrEmpty(branch.slm_StartTime_Minute)
                    || string.IsNullOrEmpty(branch.slm_EndTime_Hour) || string.IsNullOrEmpty(branch.slm_EndTime_Minute))
                {
                    // กรณีไม่มีเวลาเริ่ม/สิ้นสุดใน branch ดึงจาก option
                    string start = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "startTime").Select(p => p.slm_OptionDesc).FirstOrDefault();
                    string end = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "endTime").Select(p => p.slm_OptionDesc).FirstOrDefault();


                    // แยกเวลาเริ่มออกมาเป็น H / M
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


                    // แยกเวลาสิ้นสุดออกมาเป็น H  / M
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
                    // ใช้วันที่จากสาขา
                    startTimeHour = int.Parse(branch.slm_StartTime_Hour);
                    startTimeMin = int.Parse(branch.slm_StartTime_Minute);
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
                    currentSla = oldStatusDate.HasValue ? DeleteSeconds(oldStatusDate.Value) : currentDate;
                  //  currentSla = lead.slm_StatusDate != null ? DeleteSeconds(lead.slm_StatusDate.Value) : currentDate;
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

        public List<string> GetAssignedFlagAndDelegateFlag(string ticketId)
        {
            try
            {
                List<string> list = new List<string>();

                string sql = $"SELECT slm_AssignedFlag AS AssignedFlag, slm_Delegate_Flag AS DelegateFlag FROM kkslm_tr_lead WITH (NOLOCK) WHERE slm_TicketId = '{ticketId}' ";

                var lead = slmdb.ExecuteStoreQuery<LeadDataPhoneCallHistory>(sql).FirstOrDefault();

                //var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                if (lead != null)
                {
                    list.Add(lead.AssignedFlag != null ? lead.AssignedFlag.Trim() : "");
                    list.Add(lead.DelegateFlag != null ? lead.DelegateFlag.ToString() : "");
                    return list;
                }
                else
                {
                    throw new Exception("ไม่พบ Ticket Id " + ticketId + " ในระบบ");
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
                    list.Add(renew.slm_ReceiveNo != null ? renew.slm_ReceiveNo : "");
                    list.Add(renew.slm_ActSendDate != null ? "ActSent" : "");
                }

                return list;
            }
            catch
            {
                throw;
            }
        }

        public string GetSubScriptionTypeId(int cardTypeId)
        {
            try
            {
                var subId = slmdb.kkslm_ms_cardtype.Where(p => p.slm_CardTypeId == cardTypeId && p.is_Deleted == false).Select(p => p.slm_CIFSubscriptTypeId).FirstOrDefault();
                return subId != null ? subId : "0";
            }
            catch
            {
                throw;
            }
        }

        public void deletekkslm_tr_actnew_report(string ticketId, string username)
        {
            try
            {
                //remove when found

                var sql = @"delete from [dbo].[kkslm_tr_actnew_report]
                    where  convert(date,slm_createddate) = convert(date,getdate()) and slm_Export_Flag is null 
                    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                slmdb.ExecuteStoreCommand(sql);

                //insertkkslm_tr_actnew_report(ticketId, username);
            }
            catch
            {
                throw;
            }

        }

        public void insertkkslm_tr_actnew_report(string ticketId, string username)
        {
            try
            {
                var sql = @"INSERT INTO [dbo].[kkslm_tr_actnew_report]
                           ([slm_Ins_Com_Id]           ,[slm_ActNo]           ,[slm_BrachCode]
                           ,[slm_Contract_Number]           ,[slm_Title_Id]           ,[slm_Name]
                           ,[slm_LastName]           ,[slm_InsReceiveNo]           ,[slm_CarLicenseNo]
                           ,[slm_BrandCode]           ,[slm_ChassisNo]           ,[slm_Owner]
                           ,[slm_StaffRequest]           ,[slm_ActGrossPremium]           ,[slm_PolicyGrossPremium]
                           ,[slm_ActStartCoverDate]           ,[slm_ActEndCoverDate]           ,[slm_ReceiveDate]
                           ,[slm_ReceiverName]           ,[slm_House_No]           ,[slm_Moo]
                           ,[slm_Village]           ,[slm_Building]           ,[slm_Soi]
                           ,[slm_Street]           ,[slm_TambolId]           ,[slm_AmphurId]
                           ,[slm_ProvinceId]           ,[slm_Zipcode]           ,[slm_PhoneContact]
                           ,[slm_Remark]           ,[slm_IsAddressBranch]           ,[slm_Export_Flag]
                           ,[slm_Export_Date]           ,[slm_CreatedBy]           ,[slm_CreatedDate]
                           ,[slm_UpdatedBy]           ,[slm_UpdatedDate], [slm_ticketId], [is_Deleted])
                        SELECT top 1 REN.slm_ActComId AS slm_Ins_Com_Id,REN.slm_ActNo , PRE.slm_BranchCode ,
                            REN.slm_ContractNo AS slm_Contract_Number, lead.slm_TitleId  as slm_Title_Id, lead.slm_Name,
                            LEAD.slm_LastName, REN.slm_ReceiveNo AS slm_InsReceiveNo ,
                            ISNULL(REN.slm_LicenseNo, '') + ' ' + ISNULL(proregis.slm_ProvinceNameTH, '') AS slm_CarLicenseNo, REN.slm_RedbookBrandCode AS slm_BrandCode,
                            REN.slm_ChassisNo , ST.slm_EmpCode AS slm_Owner, RQ.slm_EmpCode AS slm_StaffRequest,
                            --REN.slm_CoverageTypeId ,REN.slm_RepairTypeId,
                            REN.slm_ActNetPremium AS slm_ActGrossPremium, ISNULL(REN.slm_ActNetPremium, 0) + ISNULL(REN.slm_ActVat, 0) + ISNULL(REN.slm_ActStamp, 0) AS slm_PolicyGrossPremium, 
                            REN.slm_ActStartCoverDate,REN.slm_ActEndCoverDate AS slm_ActEndCoverDate,REN.slm_ActSendDate AS slm_ReceiveDate,
                            REN.slm_Receiver AS slm_ReceiverName,
                            RENA.slm_House_No, RENA.slm_Moo, RENA.slm_Village, RENA.slm_BuildingName, RENA.slm_Soi,
                            RENA.slm_Street, RENA.slm_Tambon AS slm_TambolId, RENA.slm_Amphur AS slm_AmphurId , 
                            RENA.slm_Province AS slm_ProvinceId,RENA.slm_PostalCode AS slm_Zipcode, LEAD.slm_TelNo_1 AS slm_PhoneContact,
                            CASE WHEN REN.slm_SendDocFlag = '3' THEN 'รับเอกสารสาขา '+ BH.slm_BranchName
                            ELSE 'ที่อยู่ตามระบบ' END slm_Remark,CASE WHEN REN.slm_SendDocFlag = '3' THEN '1' ELSE '2' END slm_IsAddressBranch, 
                            
                            null [slm_Export_Flag],null [slm_Export_Date],'" + username + @"' [slm_CreatedBy],
                            getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy],getdate() [slm_UpdatedDate],
                            lead.slm_ticketId, 0 
                        FROM kkslm_tr_lead LEAD WITH (NOLOCK) 
                            INNER JOIN kkslm_tr_renewinsurance REN WITH (NOLOCK) ON LEAD.slm_ticketId = REN.slm_TicketId
                            LEFT JOIN kkslm_tr_renewinsurance_address RENA WITH (NOLOCK) ON RENA.slm_RenewInsureId = REN.slm_RenewInsureId AND slm_AddressType = 'D'
                            LEFT JOIN kkslm_ms_staff ST WITH (NOLOCK) ON ST.slm_UserName = LEAD.slm_Owner
                            LEFT JOIN kkslm_ms_staff RQ WITH (NOLOCK) ON RQ.slm_UserName = '" + username + @"' 
                            LEFT JOIN kkslm_ms_branch BH WITH (NOLOCK) ON BH.slm_BranchCode = REN.slm_SendDocBrandCode
                            LEFT JOIN kkslm_tr_productinfo prod WITH (NOLOCK) ON prod.slm_TicketId = LEAD.slm_TicketId
                            LEFT JOIN kkslm_ms_province proregis WITH (NOLOCK) ON proregis.slm_ProvinceId = prod.slm_ProvinceRegis
                            LEFT JOIN KKSLM_TR_PRELEAD PRE WITH (NOLOCK) ON PRE.slm_TicketId = LEAD.slm_ticketId 
                        WHERE LEAD.is_Deleted = 0 AND REN.slm_ActComId is not null AND LEAD.slm_Status NOT IN ('08','09') 
                        and lead.slm_ticketId= '" + ticketId + "'";
                //AND REN.slm_ReceiveDate is not null
                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }

        }

        public void insertkkslm_tr_policynew_report(string ticketId, string username)
        {
            try
            {
                //string loginEmpCode = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).Select(p => p.slm_EmpCode).FirstOrDefault();

                string sql = $"SELECT slm_EmpCode FROM kkslm_ms_staff WITH (NOLOCK) WHERE slm_UserName = '{username}' ";
                string loginEmpCode = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
                loginEmpCode = loginEmpCode == null ? "" : loginEmpCode;

                sql = @"INSERT INTO [dbo].[kkslm_tr_policynew_report]
                               ([slm_Ins_Com_Id]           ,[slm_PolicyNo]           ,[slm_BrachCode]
                               ,[slm_Contract_Number]           ,[slm_Title_Id]           ,[slm_Name]
                               ,[slm_LastName]           ,[slm_InsReceiveNo]           ,[slm_ReceiveDate]
                               ,[slm_CarLicenseNo]           ,[slm_BrandCode]           ,[slm_ChassisNo]
                               ,[slm_Owner]           ,[slm_StaffRequest]           ,[slm_CoverageTypeId]
                               ,[slm_RepairTypeId]           ,[slm_PolicyCost]           ,[slm_PolicyGrossPremium]
                               ,[slm_ActStartCoverDate]           ,[slm_ActEndCoverDate]           ,[slm_ReceiverName]
                               ,[slm_IsAddressBranch]           ,[slm_House_No]           ,[slm_Moo]
                               ,[slm_Village]           ,[slm_Building]           ,[slm_Soi]
                               ,[slm_Street]           ,[slm_TambolId]           ,[slm_AmphurId]
                               ,[slm_ProvinceId]           ,[slm_Zipcode]           ,[slm_Remark]
                               ,[slm_PhoneContact]           ,[slm_Export_Flag]           ,[slm_Export_Date]
                               ,[slm_CreatedBy]           ,[slm_CreatedDate]           ,[slm_UpdatedBy]
                               ,[slm_UpdatedDate] ,[slm_TicketId] ,[is_Deleted])
                        SELECT top 1 REN.slm_InsuranceComId AS slm_Ins_Com_Id,REN.slm_PolicyNo , PRE.slm_BranchCode ,
                         REN.slm_ContractNo AS slm_Contract_Number, lead.slm_TitleId  as slm_Title_Id, lead.slm_Name,
                         LEAD.slm_LastName, REN.slm_ReceiveNo AS slm_InsReceiveNo ,REN.slm_ReceiveDate ,
                         ISNULL(REN.slm_LicenseNo, '') + ' ' + ISNULL(proregis.slm_ProvinceNameTH, '') AS slm_CarLicenseNo, REN.slm_RedbookBrandCode AS slm_BrandCode,
                         REN.slm_ChassisNo , ST.slm_EmpCode AS slm_Owner, '" + loginEmpCode + @"' AS slm_StaffRequest,
                         REN.slm_CoverageTypeId ,REN.slm_RepairTypeId ,RENC.slm_FT AS slm_PolicyCost, 
                         REN.slm_PolicyGrossPremiumTotal AS slm_PolicyGrossPremium, REN.slm_ActStartCoverDate,
                         REN.slm_ActEndCoverDate AS slm_ActEndCoverDate, REN.slm_Receiver AS slm_ReceiverName,
                         CASE WHEN REN.slm_SendDocFlag = '3' THEN '1' ELSE '2' END slm_IsAddressBranch,
                         RENA.slm_House_No, RENA.slm_Moo, RENA.slm_Village, RENA.slm_BuildingName, RENA.slm_Soi,
                         RENA.slm_Street, RENA.slm_Tambon AS slm_TambolId, RENA.slm_Amphur AS slm_AmphurId , 
                         RENA.slm_Province AS slm_ProvinceId,RENA.slm_PostalCode AS slm_Zipcode, 
                         CASE WHEN REN.slm_SendDocFlag = '3' THEN 'รับเอกสารสาขา '+ BH.slm_BranchName
                          ELSE 'ที่อยู่ตามระบบ' END slm_Remark,LEAD.slm_TelNo_1 AS slm_PhoneContact,
                         null [slm_Export_Flag],null [slm_Export_Date],'" + username + @"' [slm_CreatedBy],
                         getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy],getdate() [slm_UpdatedDate],
                         REN.slm_TicketId, 0 
                        FROM kkslm_tr_lead LEAD WITH (NOLOCK) 
                         INNER JOIN kkslm_tr_renewinsurance REN WITH (NOLOCK) ON LEAD.slm_ticketId = REN.slm_TicketId
                         LEFT JOIN kkslm_tr_renewinsurance_compare RENC WITH (NOLOCK) ON RENC.slm_RenewInsureId = REN.slm_RenewInsureId AND RENC.slm_Selected = 1 
                         LEFT JOIN kkslm_tr_renewinsurance_address RENA WITH (NOLOCK) ON RENA.slm_RenewInsureId = REN.slm_RenewInsureId AND slm_AddressType = 'D'
                         LEFT JOIN kkslm_ms_staff ST WITH (NOLOCK) ON ST.slm_UserName = LEAD.slm_Owner
                         LEFT JOIN kkslm_ms_staff RQ WITH (NOLOCK) ON RQ.slm_UserName = lead.slm_UpdatedBy 
                         LEFT JOIN kkslm_ms_branch BH WITH (NOLOCK) ON BH.slm_BranchCode = REN.slm_SendDocBrandCode
                         LEFT JOIN kkslm_tr_productinfo prod WITH (NOLOCK) ON prod.slm_TicketId = LEAD.slm_TicketId
                         LEFT JOIN kkslm_ms_province proregis WITH (NOLOCK) ON proregis.slm_ProvinceId = prod.slm_ProvinceRegis
                         LEFT JOIN KKSLM_TR_PRELEAD PRE WITH (NOLOCK) ON PRE.slm_TicketId = LEAD.slm_ticketId     
                        WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status NOT IN ('08','09') and REN.slm_ReceiveDate is not null and lead.slm_ticketId= '" + ticketId + "'";

                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }

        }

        public void deletekkslm_tr_policynew_report(string ticketId, string username)
        {
            try
            {
                //remove when found
                var sql = @"delete from [dbo].[kkslm_tr_policynew_report]
                    where  convert(date,slm_createddate) = convert(date,getdate()) and slm_Export_Flag is null 
                    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                var updatesql = @"update [dbo].[kkslm_tr_policynew_report] set is_Deleted = '1' 
                        where slm_Export_Flag = '0' and slm_TicketId  = '" + ticketId + "'"; //change exportflag from 1 to 0 * update เฉพาะที่ยังไม่ export OuMz 14/2/60

                slmdb.ExecuteStoreCommand(sql);
                slmdb.ExecuteStoreCommand(updatesql);
                //insertkkslm_tr_policynew_report(ticketId, username);
            }
            catch
            {
                throw;
            }
        }

        public List<PhoneCallHistoryData> SearchNewPhoneCallHistory(string citizenId, string ticketid, bool thisLead, string preleadId)
        {
            try
            {
                if (string.IsNullOrEmpty(preleadId))
                    preleadId = "0";

                string slmDBName = SLMConstant.SLMDBName;
                //Prelead
                string prelead_sql = @"SELECT phone.slm_CreatedDate AS CreatedDate, '' AS TicketId, prelead.slm_Name AS Firstname, opt.slm_OptionDesc AS StatusDesc, 
	                                cardtype.slm_CardTypeName AS CardTypeDesc,
	                                prelead.slm_CitizenId AS CitizenId, prelead.slm_LastName AS LastName, phone.slm_ContactPhone AS ContactPhone, phone.slm_ContactDetail AS ContactDetail
	                                ,CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
			                                ELSE posowner.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS OwnerName
	                                ,cam.slm_CampaignName AS CampaignName
	                                ,CASE WHEN ISNULL(staff2.slm_StaffNameTH, phone.slm_CreatedBy) = phone.slm_CreatedBy and poscreateby.slm_PositionNameAbb IS NOT NULL 
											  THEN poscreateby.slm_PositionNameAbb + ' - ' +  phone.slm_CreatedBy
                                          WHEN ISNULL(staff2.slm_StaffNameTH, phone.slm_CreatedBy) = phone.slm_CreatedBy THEN phone.slm_CreatedBy
			                              WHEN poscreateby.slm_PositionNameAbb IS NULL THEN staff2.slm_StaffNameTH
			                              ELSE poscreateby.slm_PositionNameAbb + ' - ' + staff2.slm_StaffNameTH END AS CreatedName
                                    , '' AS CreditFilePath, '' AS CreditFileName, '' AS Tawi50FilePath, '' AS Tawi50FileName, '' AS DriverLicenseFilePath, '' AS DriverLicenseFileName
                                    , phone.slm_InvoiceFilePath AS QuotationFilePath
                                    , phone.slm_InvoiceFileName AS QuotationFileName
	                                FROM " + slmDBName + @".dbo.kkslm_tr_prelead_phone_call phone WITH (NOLOCK)
	                                LEFT JOIN " + slmDBName + @".dbo.kkslm_tr_prelead prelead WITH (NOLOCK) on phone.slm_Prelead_Id = prelead.slm_Prelead_Id AND prelead.is_Deleted = 0
	                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position posowner WITH (NOLOCK) ON phone.slm_Owner_Position = posowner.slm_Position_id
	                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff staff WITH (NOLOCK) ON phone.slm_Owner = staff.slm_EmpCode  
	                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_option opt WITH (NOLOCK) ON phone.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
	                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_campaign cam WITH (NOLOCK) ON prelead.slm_CampaignId = cam.slm_CampaignId
	                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position poscreateby WITH (NOLOCK) ON phone.slm_CreatedBy_Position = poscreateby.slm_Position_id
	                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff staff2 WITH (NOLOCK) ON phone.slm_CreatedBy = staff2.slm_UserName 
	                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) ON prelead.slm_CardTypeId = cardtype.slm_CardTypeId ";

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
                                ,CASE WHEN ISNULL(staff2.slm_StaffNameTH, phone.slm_CreateBy) = phone.slm_CreateBy and poscreateby.slm_PositionNameAbb IS NOT NULL 
											THEN poscreateby.slm_PositionNameAbb + ' - ' +  phone.slm_CreateBy
                                      WHEN ISNULL(staff2.slm_StaffNameTH, phone.slm_CreateBy) = phone.slm_CreateBy THEN phone.slm_CreateBy
	                                  WHEN poscreateby.slm_PositionNameAbb IS NULL THEN staff2.slm_StaffNameTH
	                                  ELSE poscreateby.slm_PositionNameAbb + ' - ' + staff2.slm_StaffNameTH END AS CreatedName
                                , phone.slm_CreditFilePath AS CreditFilePath, phone.slm_CreditFileName AS CreditFileName
                                , phone.slm_50TawiFilePath AS Tawi50FilePath, phone.slm_50TawiFileName AS Tawi50FileName
                                , phone.slm_DriverLicenseFilePath AS DriverLicenseFilePath, phone.slm_DriverLicenseiFileName AS DriverLicenseiFileName
                                , phone.slm_InvoiceFilePath AS QuotationFilePath
                                , phone.slm_InvoiceFileName AS QuotationFileName
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

                //Main SQL
                string mainSql = "SELECT A.* FROM ( {0} UNION ALL {1} ) A ORDER BY A.CreatedDate DESC ";
                mainSql = string.Format(mainSql, prelead_sql, lead_sql);

                return slmdb.ExecuteStoreQuery<PhoneCallHistoryData>(mainSql).ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<ActivityConfigData> GetActivityConfig(string productId, string leadStatusCode)
        {
            return slmdb.kkslm_ms_config_activity.Where(p => p.slm_Product_Id == productId && p.slm_LeadStatus == leadStatusCode && p.is_Deleted == false)
                        .Select(p => new ActivityConfigData { ProductId = p.slm_Product_Id, LeadStatusCode = p.slm_LeadStatus, HaveRightAdd = p.slm_HaveRightAdd, LeadAvailableStatusCode = p.slm_AvailableStatus }).ToList();
        }

        public bool CheckRenewOrActPurchased(string ticketId) { int p, a; return CheckRenewOrActPurchased(ticketId, out p, out a); }
        public bool CheckRenewOrActPurchased(string ticketId, out int countPolicy , out int countAct )
        {
            try
            {
                string sql = "";
                countPolicy = 0;
                countAct = 0;

                sql = @"SELECT COUNT(compare.slm_RenewInsureCompareId)
                        FROM dbo.kkslm_tr_renewinsurance renew WITH (NOLOCK) 
                        INNER JOIN dbo.kkslm_tr_renewinsurance_compare compare WITH (NOLOCK) ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
                        WHERE renew.slm_TicketId = '" + ticketId + "' AND compare.slm_Selected = 1 ";

                countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                sql = @"SELECT COUNT(compare.slm_RenewInsureCompareActId)
                        FROM dbo.kkslm_tr_renewinsurance renew WITH (NOLOCK) 
                        INNER JOIN dbo.kkslm_tr_renewinsurance_compare_act compare WITH (NOLOCK) ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
                        WHERE renew.slm_TicketId = '" + ticketId + "' AND compare.slm_ActPurchaseFlag = 1 ";

                countAct = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                return (countPolicy == 0 && countAct == 0) ? false : true;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckRequireCardId(string statusCode)
        {
            var require = slmdb.kkslm_ms_config_close_job.Where(p => p.slm_Status == statusCode && p.is_Deleted == false).Select(p => p.slm_RequireCardId).FirstOrDefault();
            return require != null ? require.Value : false;
        }

        public bool CheckBranchActive(string branchCode)
        {
            var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == branchCode).FirstOrDefault();
            return branch != null ? (branch.is_Deleted ? false : true) : false;
        }

        public bool CheckSubLeadStatus(string ticketId)
        {
            var reNewInsCompr = slmdb.kkslm_tr_renewinsurance_compare;
            var reNewInx = slmdb.kkslm_tr_renewinsurance.Where(e => e.slm_TicketId == ticketId);
            var JoinTable = reNewInx.Join(reNewInsCompr,
                reNew => reNew.slm_RenewInsureId,
                Compr => Compr.slm_RenewInsureId,
                (reNew, Compr) => new { reNew, Compr }).Where(e => e.Compr.slm_Selected == true);

            return JoinTable.Count() > 0 ? true : false;
        }

        public int? GetSubStatusType(string subStatusCode)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_config_product_substatus.Where(s => s.slm_SubStatusCode == subStatusCode).Select(s=>s.slm_SubStatusType).FirstOrDefault();
            }
        }
    }
}
