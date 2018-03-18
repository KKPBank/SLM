using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using SLM.Resource.Data;
using System.Configuration;
using SLM.Resource;
using System.Collections;

namespace SLM.Dal.Models
{
    public class SearchObtModel
    {
        private string SLMDBName = SLMConstant.SLMDBName;
        private string OPERDBName = SLMConstant.OPERDBName;

        /// <summary>
        /// Get จำนวนงานที่เป็นงาน Inbound, Outbound ของคน Login
        /// </summary>
        /// <param name="condd"></param>
        /// <returns></returns>
        public List<ControlListData> GetAmountInboundOutBound(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                //var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).FirstOrDefault();
                //if (staff == null) return new List<ControlListData>();

                string sql = $"SELECT * FROM kkslm_ms_staff WITH (NOLOCK) WHERE slm_UserName = '{username}' ";
                var staff = slmdb.ExecuteStoreQuery<kkslm_ms_staff>(sql).FirstOrDefault();
                if (staff == null)
                {
                    return new List<ControlListData>();
                }

                sql = @"SELECT CONVERT(VARCHAR, A.Amount) AS ValueField, A.Flag AS TextField
                        FROM ( SELECT COUNT(*) AS Amount, 'outbound' AS Flag
	                            FROM dbo.kkslm_tr_prelead prelead WITH (NOLOCK) 
	                            WHERE prelead.is_Deleted = 0 AND prelead.slm_TicketId IS NULL 
		                        AND prelead.slm_Owner = '" + staff.slm_EmpCode + @"'
                                AND prelead.slm_Status = '16' AND prelead.slm_SubStatus NOT IN ('06', '07', '08', '09') 
                                AND prelead.slm_AssignFlag = '1' AND prelead.slm_Assign_Status = '1' 
		                        UNION
		                        SELECT COUNT(*) AS Amount, 'inbound' AS Flag
		                        FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) 
		                        INNER JOIN 
		                        (
                                    SELECT DISTINCT B.slm_CampaignId
								    FROM (
			                                SELECT RTRIM(LTRIM(CP.PR_CampaignId)) AS slm_CampaignId 
                                            FROM kkslm_ms_access_right AR WITH (NOLOCK) 
			                                INNER  JOIN CMT_CAMPAIGN_PRODUCT CP WITH (NOLOCK) ON CP.PR_ProductId = AR.slm_Product_Id
			                                WHERE AR.slm_Product_Id IS NOT NULL AND
			                                AR.slm_BranchCode = '" + staff.slm_BranchCode + @"' AND AR.slm_StaffTypeId = '" + staff.slm_StaffTypeId + @"'
			                                UNION ALL
			                                SELECT RTRIM(LTRIM(AR.slm_CampaignId))  AS slm_CampaignId  
                                            FROM kkslm_ms_access_right AR WITH (NOLOCK) 
			                                WHERE AR.slm_CampaignId IS NOT NULL AND
			                                AR.slm_BranchCode = '" + staff.slm_BranchCode + @"' AND AR.slm_StaffTypeId = '" + staff.slm_StaffTypeId + @"'
                                        ) B
		                        ) AS Z ON Z.slm_CampaignId = lead.slm_CampaignId 
		                        LEFT JOIN dbo.kkslm_tr_renewinsurance renew WITH (NOLOCK) ON renew.slm_TicketId = lead.slm_TicketId
		                        WHERE lead.is_Deleted = 0 AND lead.slm_Status NOT IN ('08','09','10')  
		                        AND ( (lead.slm_Owner = '" + staff.slm_UserName + @"') OR (lead.slm_Delegate = '" + staff.slm_UserName + @"') ) 
                        ) A  ";

                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        private SqlParameter[] GetParameters(SearchObtCondition cond)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@firstname", "%" + cond.Firstname + "%")
                , new SqlParameter("@lastname", "%" + cond.Lastname + "%")
                , new SqlParameter("@citizenId", "%" + cond.CitizenId + "%")
                , new SqlParameter("@carlicenseno", "%" + cond.CarLicenseNo + "%")
                , new SqlParameter("@contractnorefer", "%" + cond.ContractNoRefer + "%")
            };

            return param;
        }

        #region Tab FollowUp

        /// <summary>
        /// Get จำนวนงานที่เป็นงาน follow up ของคน login ที่ Tab FollowUp
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public int GetAmountJobTabFollowUp(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                //var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).FirstOrDefault();
                //if (staff == null) return 0;

                string sql = $"SELECT * FROM kkslm_ms_staff WITH (NOLOCK) WHERE slm_UserName = '{username}' ";
                var staff = slmdb.ExecuteStoreQuery<kkslm_ms_staff>(sql).FirstOrDefault();
                if (staff == null)
                {
                    return 0;
                }

                sql = @"SELECT SUM(A.Amount) AS FollowupAmount
                        FROM ( SELECT COUNT(*) AS Amount
	                            FROM dbo.kkslm_tr_prelead prelead WITH (NOLOCK) 
	                            WHERE prelead.is_Deleted = 0 AND prelead.slm_TicketId IS NULL 
                                AND prelead.slm_Owner = '" + staff.slm_EmpCode + @"' 
                                AND prelead.slm_Status = '16' AND prelead.slm_SubStatus NOT IN ('06', '07', '08', '09') 
                                AND prelead.slm_AssignFlag = '1' AND prelead.slm_Assign_Status = '1' 
	                            AND (prelead.slm_NextContactDate IS NOT NULL AND CONVERT(DATE, prelead.slm_NextContactDate) <= CONVERT(DATE, GETDATE())) 
		                        UNION ALL 
		                        SELECT COUNT(*) AS Amount
		                        FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) 
		                        LEFT JOIN dbo.kkslm_tr_renewinsurance renew WITH (NOLOCK) ON renew.slm_TicketId = lead.slm_TicketId
		                        WHERE lead.is_Deleted = 0 AND lead.slm_Status NOT IN ('08','09','10') 
		                        AND (lead.slm_NextContactDate IS NOT NULL AND CONVERT(DATE, lead.slm_NextContactDate) <= CONVERT(DATE, GETDATE())) 
		                        AND ( (lead.slm_Owner = '" + staff.slm_UserName + @"') OR (lead.slm_Delegate = '" + staff.slm_UserName + @"') ) 
                        ) A ";

                return slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
            }
        }

        private void SaveActivityLog(SearchObtCondition cond, string searchType, out string logError)
        {
            try
            {
                logError = "";
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {

                    string nextContactDateFrom = "";
                    string nextContactDateTo = "";
                    string createdDate = "";
                    string assignedDate = "";
                    string workFlag = "";
                    string detail = "";

                    if (cond.NextContactDateFrom != null && cond.NextContactDateFrom.Year != 1)

                        nextContactDateFrom = cond.NextContactDateFrom.ToString("dd/MM/") + cond.NextContactDateFrom.Year.ToString();
                    if (cond.NextContactDateTo != null && cond.NextContactDateTo.Year != 1)
                    {
                        nextContactDateTo = cond.NextContactDateTo.ToString("dd/MM/") + cond.NextContactDateTo.Year.ToString();
                    }
                    if (cond.CreatedDate != null && cond.CreatedDate.Year != 1)
                    {
                        createdDate = cond.CreatedDate.ToString("dd/MM/") + cond.CreatedDate.Year.ToString();
                    }
                    if (cond.AssignedDate != null && cond.AssignedDate.Year != 1)
                    {
                        assignedDate = cond.AssignedDate.ToString("dd/MM/") + cond.AssignedDate.Year.ToString();
                    }

                    if (cond.CheckWaitCreditForm)
                    {
                        workFlag += (workFlag == "" ? "" : ",") + cond.CheckWaitCreditFormDesc;
                    }
                    if (cond.CheckWait50Tawi)
                    {
                        workFlag += (workFlag == "" ? "" : ",") + cond.CheckWait50TawiDesc;
                    }
                    if (cond.CheckWaitDriverLicense)
                    {
                        workFlag += (workFlag == "" ? "" : ",") + cond.CheckWaitDriverLicenseDesc;
                    }
                    if (cond.CheckPolicyNo)
                    {
                        workFlag += (workFlag == "" ? "" : ",") + cond.CheckPolicyNoDesc;
                    }
                    if (cond.CheckAct)
                    {
                        workFlag += (workFlag == "" ? "" : ",") + cond.CheckActDesc;
                    }
                    if (cond.CheckStopClaim)
                    {
                        workFlag += (workFlag == "" ? "" : ",") + cond.CheckStopClaimDesc;
                    }
                    if (cond.CheckStopClaim_Cancel)
                    {
                        workFlag += (workFlag == "" ? "" : ",") + cond.CheckStopClaim_CancelDesc;
                    }

                    if (searchType == "followup")
                    {
                        detail = string.Join(""
                                            , "|Tab:FollowUp"
                                            , string.IsNullOrEmpty(cond.ContractNo) ? "" : string.Format("|เลขที่สัญญา:{0}", cond.ContractNo)
                                            , string.IsNullOrEmpty(cond.TicketId) ? "" : string.Format("|Ticket ID:{0}", cond.TicketId)
                                            , string.IsNullOrEmpty(cond.Firstname) ? "" : string.Format("|ชื่อลูกค้า:{0}", cond.Firstname)
                                            , string.IsNullOrEmpty(cond.Lastname) ? "" : string.Format("|นามสกุลลูกค้า:{0}", cond.Lastname)
                                            , string.IsNullOrEmpty(cond.CardTypeDesc) ? "" : string.Format("|ประเภทบุคคล:{0}", cond.CardTypeDesc)
                                            , string.IsNullOrEmpty(cond.CitizenId) ? "" : string.Format("|เลขที่บัตร:{0}", cond.CitizenId)
                                            , string.IsNullOrEmpty(cond.ChannelId) ? "" : string.Format("|ช่องทาง:{0}", cond.ChannelId)
                                            , string.IsNullOrEmpty(cond.CampaignName) ? "" : string.Format("|แคมเปญ:{0}", cond.CampaignName)
                                            , string.IsNullOrEmpty(cond.CarLicenseNo) ? "" : string.Format("|ทะเบียนรถยนต์:{0}", cond.CarLicenseNo)
                                            , cond.Grade == null ? "" : string.Format("|เกรด:{0}", cond.Grade.Length == 0 ? "ไม่มี Grade" : string.Join("+", cond.Grade))
                                            , string.IsNullOrEmpty(nextContactDateFrom) ? "" : string.Format("|วันที่นัดโทรหาลูกค้าเริ่มต้น:{0}", nextContactDateFrom)
                                            , string.IsNullOrEmpty(nextContactDateTo) ? "" : string.Format("|วันที่นัดโทรหาลูกค้าสิ้นสุด:{0}", nextContactDateTo)
                                            , string.IsNullOrEmpty(cond.HasNotifyPremium) ? "" : string.Format("|มีค่าเบี้ยปีต่อ:{0}", cond.HasNotifyPremium)
                                            , string.IsNullOrEmpty(cond.NotifyGrossPremiumMin) ? "" : string.Format("|ค่าเบี้ยปีต่อเริ่มต้น:{0}", cond.NotifyGrossPremiumMin)
                                            , string.IsNullOrEmpty(cond.NotifyGrossPremiumMax) ? "" : string.Format("|ค่าเบี้ยปีต่อสิ้นสุด:{0}", cond.NotifyGrossPremiumMax)
                                            , string.IsNullOrEmpty(cond.PolicyExpirationYear) ? "" : string.Format("|ปีที่คุ้มครอง:{0}", cond.PolicyExpirationYear)
                                            , string.IsNullOrEmpty(cond.PolicyExpirationMonthName) ? "" : string.Format("|เดือนที่คุ้มครอง:{0}", cond.PolicyExpirationMonthName)
                                            , string.IsNullOrEmpty(cond.PeriodYear) ? "" : string.Format("|ปีที่แจก Leads:{0}", cond.PeriodYear)
                                            , string.IsNullOrEmpty(cond.PeriodMonthName) ? "" : string.Format("|เดือนที่แจก Leads:{0}", cond.PeriodMonthName)
                                            , string.IsNullOrEmpty(cond.ContractNoRefer) ? "" : string.Format("|เลขที่สัญญาที่เคยมีกับธนาคาร:{0}", cond.ContractNoRefer)
                                            , string.IsNullOrEmpty(createdDate) ? "" : string.Format("|วันที่สร้าง Lead:{0}", createdDate)
                                            , string.IsNullOrEmpty(assignedDate) ? "" : string.Format("|วันที่ได้รับมอบหมายล่าสุด:{0}", assignedDate)
                                            , string.IsNullOrEmpty(cond.CreatedByBranchName) ? "" : string.Format("|สาขาผู้สร้าง Lead:{0}", cond.CreatedByBranchName)
                                            , string.IsNullOrEmpty(cond.CreatedByName) ? "" : string.Format("|ผู้สร้าง Lead:{0}", cond.CreatedByName)
                                            , string.IsNullOrEmpty(workFlag) ? "" : string.Format("|Flag การทำงาน:{0}", workFlag)
                                            , string.IsNullOrEmpty(cond.StatusNameList) ? "" : string.Format("|สถานะของ Lead:{0}", cond.StatusNameList)
                                            , string.IsNullOrEmpty(cond.SubStatusNameList) ? "" : string.Format("|สถานะย่อยของ Lead:{0}", cond.SubStatusNameList)
                                        );
                    }
                    else
                    {
                        //alltask
                        string searchFlag = "";
                        if (cond.CheckFollowUp)
                        {
                            searchFlag += (searchFlag == "" ? "" : ",") + "Follow Up";
                        }
                        if (cond.CheckInbound)
                        {
                            searchFlag += (searchFlag == "" ? "" : ",") + "Inbound";
                        }
                        if (cond.CheckOutbound)
                        {
                            searchFlag += (searchFlag == "" ? "" : ",") + "Outbound";
                        }

                        detail = string.Join(""
                                            , "|Tab:AllTask"
                                            , string.IsNullOrEmpty(cond.ContractNo) ? "" : string.Format("|เลขที่สัญญา:{0}", cond.ContractNo)
                                            , string.IsNullOrEmpty(cond.TicketId) ? "" : string.Format("|Ticket ID:{0}", cond.TicketId)
                                            , string.IsNullOrEmpty(cond.Firstname) ? "" : string.Format("|ชื่อลูกค้า:{0}", cond.Firstname)
                                            , string.IsNullOrEmpty(cond.Lastname) ? "" : string.Format("|นามสกุลลูกค้า:{0}", cond.Lastname)
                                            , string.IsNullOrEmpty(cond.CardTypeDesc) ? "" : string.Format("|ประเภทบุคคล:{0}", cond.CardTypeDesc)
                                            , string.IsNullOrEmpty(cond.CitizenId) ? "" : string.Format("|เลขที่บัตร:{0}", cond.CitizenId)
                                            , string.IsNullOrEmpty(cond.ChannelId) ? "" : string.Format("|ช่องทาง:{0}", cond.ChannelId)
                                            , string.IsNullOrEmpty(cond.CampaignName) ? "" : string.Format("|แคมเปญ:{0}", cond.CampaignName)
                                            , string.IsNullOrEmpty(cond.CarLicenseNo) ? "" : string.Format("|ทะเบียนรถยนต์:{0}", cond.CarLicenseNo)
                                            , cond.Grade == null ? "" : string.Format("|เกรด:{0}", cond.Grade.Length == 0 ? "ไม่มี Grade" : string.Join("+", cond.Grade))
                                            , string.IsNullOrEmpty(nextContactDateFrom) ? "" : string.Format("|วันที่นัดโทรหาลูกค้าเริ่มต้น:{0}", nextContactDateFrom)
                                            , string.IsNullOrEmpty(nextContactDateTo) ? "" : string.Format("|วันที่นัดโทรหาลูกค้าสิ้นสุด:{0}", nextContactDateTo)
                                            , string.IsNullOrEmpty(cond.HasNotifyPremium) ? "" : string.Format("|มีค่าเบี้ยปีต่อ:{0}", cond.HasNotifyPremium)
                                            , string.IsNullOrEmpty(cond.NotifyGrossPremiumMin) ? "" : string.Format("|ค่าเบี้ยปีต่อเริ่มต้น:{0}", cond.NotifyGrossPremiumMin)
                                            , string.IsNullOrEmpty(cond.NotifyGrossPremiumMax) ? "" : string.Format("|ค่าเบี้ยปีต่อสิ้นสุด:{0}", cond.NotifyGrossPremiumMax)
                                            , string.IsNullOrEmpty(cond.PolicyExpirationYear) ? "" : string.Format("|ปีที่คุ้มครอง:{0}", cond.PolicyExpirationYear)
                                            , string.IsNullOrEmpty(cond.PolicyExpirationMonthName) ? "" : string.Format("|เดือนที่คุ้มครอง:{0}", cond.PolicyExpirationMonthName)
                                            , string.IsNullOrEmpty(cond.PeriodYear) ? "" : string.Format("|ปีที่แจก Leads:{0}", cond.PeriodYear)
                                            , string.IsNullOrEmpty(cond.PeriodMonthName) ? "" : string.Format("|เดือนที่แจก Leads:{0}", cond.PeriodMonthName)
                                            , string.IsNullOrEmpty(searchFlag) ? "" : string.Format("|การค้นหา:{0}", searchFlag)
                                            , string.IsNullOrEmpty(cond.ContractNoRefer) ? "" : string.Format("|เลขที่สัญญาที่เคยมีกับธนาคาร:{0}", cond.ContractNoRefer)
                                            , string.IsNullOrEmpty(createdDate) ? "" : string.Format("|วันที่สร้าง Lead:{0}", createdDate)
                                            , string.IsNullOrEmpty(assignedDate) ? "" : string.Format("|วันที่ได้รับมอบหมายล่าสุด:{0}", assignedDate)
                                            , string.IsNullOrEmpty(cond.OwnerBranchName) ? "" : string.Format("|Owner Branch:{0}", cond.OwnerBranchName)
                                            , string.IsNullOrEmpty(cond.OwnerName) ? "" : string.Format("|Owner Lead:{0}", cond.OwnerName)
                                            , string.IsNullOrEmpty(cond.DelegateBranchName) ? "" : string.Format("|Delegate Branch:{0}", cond.DelegateBranchName)
                                            , string.IsNullOrEmpty(cond.DelegateName) ? "" : string.Format("|Delegate Lead:{0}", cond.DelegateName)
                                            , string.IsNullOrEmpty(cond.CreatedByBranchName) ? "" : string.Format("|สาขาผู้สร้าง Lead:{0}", cond.CreatedByBranchName)
                                            , string.IsNullOrEmpty(cond.CreatedByName) ? "" : string.Format("|ผู้สร้าง Lead:{0}", cond.CreatedByName)
                                            , string.IsNullOrEmpty(workFlag) ? "" : string.Format("|Flag การทำงาน:{0}", workFlag)
                                            , string.IsNullOrEmpty(cond.StatusNameList) ? "" : string.Format("|สถานะของ Lead:{0}", cond.StatusNameList)
                                            , string.IsNullOrEmpty(cond.SubStatusNameList) ? "" : string.Format("|สถานะย่อยของ Lead:{0}", cond.SubStatusNameList)
                                        );
                    }

                    kkslm_tr_searchcriteria_log log = new kkslm_tr_searchcriteria_log()
                    {
                        slm_CreatedBy = cond.StaffUsername,
                        slm_CreatedBy_Branch = cond.StaffBranchCode,
                        slm_CreatedDate = DateTime.Now,
                        slm_IPSender = GetIPAddress(),
                        slm_Detail = detail,
                        slm_ScreenCode = cond.ScreenCode
                    };
                    slmdb.kkslm_tr_searchcriteria_log.AddObject(log);
                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logError = "Method SaveActivityLog : " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        protected string GetIPAddress()
        {

            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public List<SearchObtResult> SearchTabFollowUp(SearchObtCondition cond, out string logError)
        {
            try
            {
                logError = "";
                SaveActivityLog(cond, "followup", out logError);

                string sql = @"SELECT A.* FROM ( " + GetFollowUpOutBoundQuery(cond) + @" UNION ALL " + GetFollowUpInBoundQuery(cond) + @" ) A
                            ORDER BY A.NextContactDate ASC ";

                List<SearchObtResult> retList = null;
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    retList = slmdb.ExecuteStoreQuery<SearchObtResult>(sql, GetParameters(cond)).ToList();
                }
                return retList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetFollowUpOutBoundQuery(SearchObtCondition cond)
        {
            string sql = @"SELECT prelead.slm_Prelead_Id AS PreleadId, prelead.slm_Counting AS Counting, prelead.slm_NextContactDate AS NextContactDate, YEAR(prelead.slm_Voluntary_Policy_Exp_Date) AS PolicyExpirationYear, MONTH(prelead.slm_Voluntary_Policy_Exp_Date) AS PolicyExpirationMonth
	                        , prelead.slm_Contract_Number AS ContractNo, prelead.slm_Name AS Firstname, prelead.slm_Lastname AS Lastname, opt.slm_OptionDesc AS StatusDesc, '' AS SubStatusDesc, prelead.slm_Voluntary_Policy_Exp_Date AS PolicyExpireDate
	                        , cardtype.slm_CardTypeName AS CardTypeName, '' AS TicketId, prelead.slm_CitizenId AS CitizenId, campaign.slm_CampaignName AS CampaignName, 'Telesales' AS ChannelDesc
	                        , own.slm_StaffNameTH AS OwnerName, '' AS DelegateName, prelead.slm_CreatedBy AS CreaterName, prelead.slm_CreatedDate AS CreatedDate, prelead.slm_AssignDate AS AssignedDate
	                        , ownerbranch.slm_BranchName AS OwnerBranchName, '' AS DelegateBranchName, '' AS CreaterBranchName, '' AS ContractNoRefer
                            , '' AS NoteFlag, '' AS CalculatorUrl, CONVERT(BIT, 0) AS HasAdamUrl, '' AS AppNo, prelead.slm_Product_Id AS ProductId, '' AS IsCOC
	                        , '' AS COCCurrentTeam, prelead.slm_Status AS StatusCode, prelead.slm_CampaignId AS CampaignId, prelead.slm_SubStatus AS SubStatusCode
                            , prelead.slm_NextSla AS NextSLA, DATEDIFF(minute, GETDATE(), prelead.slm_NextSla) AS NextSLADiffCurrent, prelead.slm_Car_License_No AS LicenseNo
                            , prelead.slm_Grade AS Grade
	                        , NULL AS DelegateDate
	                        , prelead.slm_PeriodYear AS PeriodYear, prelead.slm_PeriodMonth AS PeriodMonth
	                        FROM dbo.kkslm_tr_prelead prelead WITH (NOLOCK)
	                        LEFT JOIN dbo.kkslm_ms_option opt WITH (NOLOCK) ON prelead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
	                        LEFT JOIN dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) ON cardtype.slm_CardTypeId = prelead.slm_CardTypeId
	                        LEFT JOIN dbo.kkslm_ms_campaign campaign WITH (NOLOCK) ON campaign.slm_CampaignId = prelead.slm_CampaignId
	                        LEFT JOIN dbo.kkslm_ms_staff own WITH (NOLOCK) ON own.slm_EmpCode =  prelead.slm_Owner
	                        LEFT JOIN dbo.kkslm_ms_branch ownerbranch WITH (NOLOCK) ON ownerbranch.slm_BranchCode = prelead.slm_OwnerBranch 
	                        " + JoinNotifyPremium(cond, "prelead") + @"
	                        WHERE prelead.is_Deleted = 0 AND prelead.slm_TicketId IS NULL 
                            AND prelead.slm_Status = '16' AND prelead.slm_SubStatus NOT IN ('06', '07', '08', '09') 
                            AND prelead.slm_AssignFlag = '1' AND prelead.slm_Assign_Status = '1' 
	                        AND (prelead.slm_NextContactDate IS NOT NULL AND CONVERT(DATE, prelead.slm_NextContactDate) <= CONVERT(DATE, GETDATE())) ";

            if (cond.StaffTypeId == SLMConstant.StaffType.ITAdministrator.ToString())
            {
                sql += " {0} ";
            }
            else
            {
                sql += " AND prelead.slm_Owner = '" + cond.StaffEmpCode + @"' 
                            {0} ";
            }

            //เงื่อนไขใน where ของ prelead
            //ไม่มีการค้นหาของ TicketId, ช่องทาง, เลขที่สัญญาที่เคยมีกับธนาคาร, delegateBranch, delegate, flag การทำงาน เพราะไม่มีใน Prelead และ ผู้สร้างที่เป็น SYSTEM
            string whr = "";

            if (!string.IsNullOrEmpty(cond.TicketId)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            //Prelead OBT เป็น Channel Telesales เท่านั้น, แต่ใน table Prelead ไม่ได้มีการเก็บ ChannelId
            if (!string.IsNullOrEmpty(cond.ChannelId) && cond.ChannelId.ToUpper().Trim() != "TELESALES") { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (!string.IsNullOrEmpty(cond.ContractNoRefer)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (!string.IsNullOrEmpty(cond.CreateByBranch)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (!string.IsNullOrEmpty(cond.CreateBy)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (cond.CheckWaitCreditForm || cond.CheckWait50Tawi || cond.CheckWaitDriverLicense || cond.CheckPolicyNo || cond.CheckAct || cond.CheckStopClaim || cond.CheckStopClaim_Cancel)
                whr += (whr == "" ? "" : " AND ") + " 1 = 2 ";

            whr += (string.IsNullOrEmpty(cond.ContractNo) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Contract_Number = '" + cond.ContractNo + "' ");
            whr += (string.IsNullOrEmpty(cond.Firstname) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Name LIKE @firstname ");
            whr += (string.IsNullOrEmpty(cond.Lastname) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Lastname LIKE @lastname ");
            whr += (string.IsNullOrEmpty(cond.CardType) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_CardTypeId = '" + cond.CardType + "' ");
            whr += (string.IsNullOrEmpty(cond.CitizenId) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_CitizenId LIKE @citizenId ");
            whr += (string.IsNullOrEmpty(cond.CampaignId) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_CampaignId = '" + cond.CampaignId + "' ");
            whr += (string.IsNullOrEmpty(cond.CarLicenseNo) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Car_License_No LIKE @carlicenseno ");
            whr += (cond.Grade == null ? "" : (whr == "" ? "" : " AND ") + (cond.Grade.Length > 0 ? $" prelead.slm_Grade IN ('{string.Join("','", cond.Grade)}') " : " prelead.slm_Grade IS NULL "));
            whr += (string.IsNullOrEmpty(cond.HasNotifyPremium) ? "" : (whr == "" ? "" : " AND ") + (cond.HasNotifyPremium == "Y" ? " NP.slm_FlagNotifyPremium = 'Y' " : " NP.slm_FlagNotifyPremium IS NULL "));
            whr += (string.IsNullOrEmpty(cond.NotifyGrossPremiumMin) ? "" : (whr == "" ? "" : " AND ") + $" NP.slm_NetGrossPremium >= {cond.NotifyGrossPremiumMin} ");
            whr += (string.IsNullOrEmpty(cond.NotifyGrossPremiumMax) ? "" : (whr == "" ? "" : " AND ") + $" NP.slm_NetGrossPremium <= {cond.NotifyGrossPremiumMax} ");

            if (cond.NextContactDateFrom.Year != 1 && cond.NextContactDateTo.Year != 1)
            {
                string[] nextContactDate = { cond.NextContactDateFrom.Year.ToString() + cond.NextContactDateFrom.ToString("-MM-dd"), cond.NextContactDateTo.Year.ToString() + cond.NextContactDateTo.ToString("-MM-dd") };
                whr += (nextContactDate[0] == "" && nextContactDate[1] == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, prelead.slm_NextContactDate) BETWEEN '" + nextContactDate[0] + "' AND '" + nextContactDate[1] + "' ");
            }
            if (cond.CreatedDate.Year != 1)
            {
                string createdDate = cond.CreatedDate.Year.ToString() + cond.CreatedDate.ToString("-MM-dd");
                whr += (createdDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, prelead.slm_CreatedDate) = '" + createdDate + "' ");
            }
            if (cond.AssignedDate.Year != 1)
            {
                string assignedDate = cond.AssignedDate.Year.ToString() + cond.AssignedDate.ToString("-MM-dd");
                whr += (assignedDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, prelead.slm_AssignDate) = '" + assignedDate + "' ");
            }

            whr += (string.IsNullOrEmpty(cond.PolicyExpirationYear) ? "" : (whr == "" ? "" : " AND ") + " YEAR(prelead.slm_Voluntary_Policy_Exp_Date) = '" + cond.PolicyExpirationYear + "' ");
            whr += (string.IsNullOrEmpty(cond.PolicyExpirationMonth) ? "" : (whr == "" ? "" : " AND ") + " MONTH(prelead.slm_Voluntary_Policy_Exp_Date) = '" + cond.PolicyExpirationMonth + "' ");
            whr += (string.IsNullOrEmpty(cond.PeriodYear) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_PeriodYear = '" + cond.PeriodYear + "' ");
            whr += (string.IsNullOrEmpty(cond.PeriodMonth) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_PeriodMonth = '" + cond.PeriodMonth + "' ");
            whr += (string.IsNullOrEmpty(cond.StatusList) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Status IN (" + cond.StatusList + ") ");
            whr += (string.IsNullOrEmpty(cond.SubStatusList) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_SubStatus IN (" + cond.SubStatusList + ") ");

            //เนื่องจาก Tab Follow Up เป็นงานคน Login โดยตรง จึงไม่จำเป็นต้อง where ที่ OwnerBranch, DelegateBranch เพราะ where จาก username โดยตรง
            //whr += (string.IsNullOrEmpty(cond.OwnerBranch) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_OwnerBranch = '" + cond.OwnerBranch + "' ");
            //if (!string.IsNullOrEmpty(cond.OwnerUsername))
            //{
            //    string ownerEmpCode = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == cond.OwnerUsername).Select(p => p.slm_EmpCode).FirstOrDefault();
            //    whr += (string.IsNullOrEmpty(ownerEmpCode) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Owner = '" + ownerEmpCode + "' ");
            //}

            ////!!! prelead ไม่มี delegate, ถ้ามีการใส่ค่ามาจากหน้าจอ ให้นำค่ามา query ในช่อง Owner ไปเลย
            //whr += (string.IsNullOrEmpty(cond.DelegateBranch) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_OwnerBranch = '" + cond.DelegateBranch + "' ");
            //if (!string.IsNullOrEmpty(cond.DelegateLead))
            //{
            //    string ownerEmpCode = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == cond.DelegateLead).Select(p => p.slm_EmpCode).FirstOrDefault();
            //    whr += (string.IsNullOrEmpty(ownerEmpCode) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Owner = '" + ownerEmpCode + "' ");
            //}

            whr = whr == "" ? "" : " AND " + whr;
            sql = string.Format(sql, whr);

            return sql;
        }

        private string GetFollowUpInBoundQuery(SearchObtCondition cond)
        {
            string sql = @"SELECT NULL AS PreleadId, lead.slm_Counting AS Counting, lead.slm_NextContactDate AS NextContactDate, YEAR(prelead.slm_Voluntary_Policy_Exp_Date) AS PolicyExpirationYear, MONTH(prelead.slm_Voluntary_Policy_Exp_Date) AS PolicyExpirationMonth
	                        , renew.slm_ContractNo AS ContractNo, lead.slm_Name AS Firstname, lead.slm_Lastname AS Lastname, opt.slm_OptionDesc AS StatusDesc, lead.slm_ExternalSubStatusDesc AS SubStatusDesc, prelead.slm_Voluntary_Policy_Exp_Date AS PolicyExpireDate
	                        , cardtype.slm_CardTypeName AS CardTypeName, lead.slm_TicketId AS TicketId, info.slm_CitizenId AS CitizenId, campaign.slm_CampaignName AS CampaignName, channel.slm_ChannelDesc AS ChannelDesc
	                        , CASE WHEN ISNULL(poOwner.slm_PositionNameABB,'99999999') = '99999999' THEN own.slm_StaffNameTH
			                        ELSE poOwner.slm_PositionNameABB + ' - ' + own.slm_StaffNameTH  END OwnerName
	                        , CASE WHEN ISNULL(poDelegate.slm_PositionNameABB,'99999999') = '99999999' THEN delegate.slm_StaffNameTH 
			                        ELSE poDelegate.slm_PositionNameABB + ' - ' + delegate.slm_StaffNameTH  END DelegateName
	                        , CASE WHEN ISNULL(createby.slm_StaffNameTH , lead.slm_CreatedBy) = lead.slm_CreatedBy THEN LEAD.slm_CreatedBy
			                        WHEN ISNULL(poCreateby.slm_PositionNameABB,'99999999') = '99999999' THEN createby.slm_StaffNameTH 
			                        ELSE poCreateby.slm_PositionNameABB + ' - ' + createby.slm_StaffNameTH END CreaterName
	                        , lead.slm_CreatedDate AS CreatedDate, lead.slm_AssignedDate AS AssignedDate
	                        , ownerbranch.slm_BranchName AS OwnerBranchName, Delegatebranch.slm_BranchName AS DelegateBranchName, CreateBybranch.slm_BranchName AS CreaterBranchName, lead.slm_ContractNoRefer AS ContractNoRefer
	                        , lead.slm_NoteFlag AS NoteFlag, campaign.slm_Url AS CalculatorUrl, ISNULL(MP.HasADAMUrl, 0) AS HasAdamUrl, lead.coc_Appno AS AppNo, lead.slm_Product_Id AS ProductId, CONVERT(VARCHAR, lead.coc_IsCOC) AS IsCOC
	                        , lead.coc_CurrentTeam AS COCCurrentTeam, lead.slm_status as StatusCode, lead.slm_CampaignId AS CampaignId, lead.slm_SubStatus AS SubStatusCode
                            , lead.slm_NextSLA AS NextSLA, DATEDIFF(minute, GETDATE(), lead.slm_NextSLA) AS NextSLADiffCurrent, renew.slm_LicenseNo AS LicenseNo
	                        , prelead.slm_Grade AS Grade
	                        , lead.slm_DelegateDate AS DelegateDate
	                        , renew.slm_PeriodYear AS PeriodYear, renew.slm_PeriodMonth AS PeriodMonth
	                        FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) 
	                        LEFT JOIN dbo.kkslm_tr_renewinsurance renew WITH (NOLOCK) ON renew.slm_TicketId = lead.slm_TicketId
	                        LEFT JOIN dbo.kkslm_ms_option opt WITH (NOLOCK) ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
	                        LEFT JOIN dbo.kkslm_tr_cusinfo info WITH (NOLOCK) ON info.slm_TicketId = lead.slm_ticketId
	                        LEFT JOIN dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) ON cardtype.slm_CardTypeId = info.slm_CardType
	                        LEFT JOIN dbo.kkslm_ms_campaign campaign WITH (NOLOCK) ON campaign.slm_CampaignId = lead.slm_CampaignId
	                        LEFT JOIN dbo.kkslm_ms_channel channel WITH (NOLOCK) ON channel.slm_ChannelId = lead.slm_ChannelId
	                        LEFT JOIN dbo.kkslm_ms_staff own WITH (NOLOCK) ON own.slm_UserName = lead.slm_Owner 
	                        LEFT JOIN dbo.kkslm_ms_position poOwner WITH (NOLOCK) ON poOwner.slm_Position_id = lead.slm_Owner_Position
	                        LEFT JOIN DBO.kkslm_ms_staff delegate WITH (NOLOCK) ON delegate.slm_UserName = lead.slm_Delegate
	                        LEFT JOIN DBO.kkslm_ms_position poDelegate WITH (NOLOCK) ON poDelegate.slm_Position_id = lead.slm_Delegate_Position
	                        LEFT JOIN DBO.kkslm_ms_staff createby WITH (NOLOCK) ON createby.slm_UserName = lead.slm_CreatedBy
	                        LEFT JOIN DBO.kkslm_ms_position poCreateby WITH (NOLOCK) ON poCreateby.slm_Position_id = lead.slm_CreatedBy_Position 
	                        LEFT JOIN DBO.kkslm_ms_branch ownerbranch WITH (NOLOCK) ON ownerbranch.slm_BranchCode = lead.slm_Owner_Branch 
	                        LEFT JOIN DBO.kkslm_ms_branch Delegatebranch WITH (NOLOCK) ON Delegatebranch.slm_BranchCode = lead.slm_Delegate_Branch
	                        LEFT JOIN DBO.kkslm_ms_branch CreateBybranch WITH (NOLOCK) ON CreateBybranch.slm_BranchCode = lead.slm_CreatedBy_Branch
	                        LEFT JOIN dbo.CMT_MAPPING_PRODUCT MP WITH (NOLOCK) ON MP.sub_product_id = lead.slm_Product_Id 
	                        LEFT JOIN dbo.kkslm_tr_prelead prelead WITH (NOLOCK) ON prelead.slm_TicketId = lead.slm_TicketId 
	                        " + JoinNotifyPremium(cond, "renew") + @"
	                        WHERE lead.is_Deleted = 0 AND lead.slm_Status NOT IN ('08','09','10') 
	                        AND (lead.slm_NextContactDate IS NOT NULL AND CONVERT(DATE, lead.slm_NextContactDate) <= CONVERT(DATE, GETDATE())) ";

            if (cond.StaffTypeId == SLMConstant.StaffType.ITAdministrator.ToString())
            {
                sql += " {0} ";
            }
            else
            {
                sql += " AND ( (lead.slm_Owner = '" + cond.StaffUsername + "') OR (lead.slm_Delegate = '" + cond.StaffUsername + @"') ) 
                            {0} ";
            }


            //หน้า Tab Follow Up เป็นหน้าใช้หางานของคน Login โดยเฉพาะ ฉะนั้นให้มองหางานที่คน Login เป็น Owner หรือ Delegate จึงใช้ OR
            //ค่า OwnerUsername และ DelegateLead ถูกใส่ค่ามาจากหน้าจอ จะไม่เป็นค่าว่างทั้งคู่

            string whr = "";

            //เนื่องจาก Tab Follow Up เป็นงานคน Login โดยตรง จึงไม่จำเป็นต้อง where ที่ OwnerBranch, DelegateBranch เพราะ where จาก username โดยตรง
            //whr += (string.IsNullOrEmpty(cond.OwnerBranch) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Owner_Branch = '" + cond.OwnerBranch + "' ");
            //whr += (string.IsNullOrEmpty(cond.DelegateBranch) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Delegate_Branch = '" + cond.DelegateBranch + "' ");

            whr += (string.IsNullOrEmpty(cond.ContractNo) ? "" : (whr == "" ? "" : " AND ") + " renew.slm_ContractNo = '" + cond.ContractNo + "' ");
            whr += (string.IsNullOrEmpty(cond.TicketId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_TicketId = '" + cond.TicketId + "' ");
            whr += (string.IsNullOrEmpty(cond.Firstname) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Name LIKE @firstname ");
            whr += (string.IsNullOrEmpty(cond.Lastname) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Lastname LIKE @lastname ");
            whr += (string.IsNullOrEmpty(cond.CardType) ? "" : (whr == "" ? "" : " AND ") + " info.slm_CardType = '" + cond.CardType + "' ");
            whr += (string.IsNullOrEmpty(cond.CitizenId) ? "" : (whr == "" ? "" : " AND ") + " info.slm_CitizenId LIKE @citizenId ");
            whr += (string.IsNullOrEmpty(cond.ChannelId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_ChannelId = '" + cond.ChannelId + "' ");
            whr += (string.IsNullOrEmpty(cond.CampaignId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CampaignId = '" + cond.CampaignId + "' ");
            whr += (string.IsNullOrEmpty(cond.CarLicenseNo) ? "" : (whr == "" ? "" : " AND ") + " renew.slm_LicenseNo LIKE @carlicenseno ");
            whr += (string.IsNullOrEmpty(cond.PolicyExpirationYear) ? "" : (whr == "" ? "" : " AND ") + " YEAR(prelead.slm_Voluntary_Policy_Exp_Date) = '" + cond.PolicyExpirationYear + "' ");
            whr += (string.IsNullOrEmpty(cond.PolicyExpirationMonth) ? "" : (whr == "" ? "" : " AND ") + " MONTH(prelead.slm_Voluntary_Policy_Exp_Date) = '" + cond.PolicyExpirationMonth + "' ");
            whr += (string.IsNullOrEmpty(cond.PeriodYear) ? "" : (whr == "" ? "" : " AND ") + " renew.slm_PeriodYear = '" + cond.PeriodYear + "' ");
            whr += (string.IsNullOrEmpty(cond.PeriodMonth) ? "" : (whr == "" ? "" : " AND ") + " renew.slm_PeriodMonth = '" + cond.PeriodMonth + "' ");
            whr += (string.IsNullOrEmpty(cond.ContractNoRefer) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_ContractNoRefer LIKE @contractnorefer ");
            whr += (cond.Grade == null ? "" : (whr == "" ? "" : " AND ") + (cond.Grade.Length > 0 ? $" prelead.slm_Grade IN ('{string.Join("','", cond.Grade)}') " : " prelead.slm_Grade IS NULL "));
            whr += (string.IsNullOrEmpty(cond.HasNotifyPremium) ? "" : (whr == "" ? "" : " AND ") + (cond.HasNotifyPremium == "Y" ? " NP.slm_FlagNotifyPremium = 'Y' " : " NP.slm_FlagNotifyPremium IS NULL "));
            whr += (string.IsNullOrEmpty(cond.NotifyGrossPremiumMin) ? "" : (whr == "" ? "" : " AND ") + $" NP.slm_NetGrossPremium >= {cond.NotifyGrossPremiumMin} ");
            whr += (string.IsNullOrEmpty(cond.NotifyGrossPremiumMax) ? "" : (whr == "" ? "" : " AND ") + $" NP.slm_NetGrossPremium <= {cond.NotifyGrossPremiumMax} ");

            if (cond.NextContactDateFrom.Year != 1 && cond.NextContactDateTo.Year != 1)
            {
                string[] nextContactDate = { cond.NextContactDateFrom.Year.ToString() + cond.NextContactDateFrom.ToString("-MM-dd"), cond.NextContactDateTo.Year.ToString() + cond.NextContactDateTo.ToString("-MM-dd") };
                whr += (nextContactDate[0] == "" && nextContactDate[1] == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_NextContactDate) BETWEEN '" + nextContactDate[0] + "' AND '" + nextContactDate[1] + "' ");
            }
            if (cond.CreatedDate.Year != 1)
            {
                string createdDate = cond.CreatedDate.Year.ToString() + cond.CreatedDate.ToString("-MM-dd");
                whr += (createdDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_CreatedDate) = '" + createdDate + "' ");
            }
            if (cond.AssignedDate.Year != 1)
            {
                string assignedDate = cond.AssignedDate.Year.ToString() + cond.AssignedDate.ToString("-MM-dd");
                whr += (assignedDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_AssignedDate) = '" + assignedDate + "' ");
            }

            whr += (string.IsNullOrEmpty(cond.CreateByBranch) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CreatedBy_Branch = '" + cond.CreateByBranch + "' ");
            whr += (string.IsNullOrEmpty(cond.CreateBy) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CreatedBy = '" + cond.CreateBy + "' ");
            whr += (string.IsNullOrEmpty(cond.StatusList) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Status IN (" + cond.StatusList + ") ");
            whr += (string.IsNullOrEmpty(cond.SubStatusList) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_SubStatus IN (" + cond.SubStatusList + ") ");

            if (cond.CheckWaitCreditForm || cond.CheckWait50Tawi || cond.CheckWaitDriverLicense || cond.CheckPolicyNo || cond.CheckAct || cond.CheckStopClaim || cond.CheckStopClaim_Cancel)
            {
                string whr2 = "";
                whr2 += (cond.CheckWaitCreditForm == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_CreditFlag = 'Y' ");
                whr2 += (cond.CheckWait50Tawi == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_50TawiFlag = 'Y' ");
                whr2 += (cond.CheckWaitDriverLicense == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_DriverLicenseFlag = 'Y' ");
                whr2 += (cond.CheckPolicyNo == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_PolicyFlag = 'Y' ");
                whr2 += (cond.CheckAct == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_CompulsoryFlag = 'Y' ");

                if (cond.CheckStopClaim && cond.CheckStopClaim_Cancel)
                {
                    whr2 += (whr2 == "" ? "" : " OR ") + " renew.slm_ClaimFlag IN ('0','1') ";
                }
                else if (cond.CheckStopClaim)
                {
                    whr2 += (whr2 == "" ? "" : " OR ") + " renew.slm_ClaimFlag = '1' ";
                }
                else if (cond.CheckStopClaim_Cancel)
                {
                    whr2 += (whr2 == "" ? "" : " OR ") + " renew.slm_ClaimFlag = '0' ";
                }

                whr += (string.IsNullOrEmpty(whr2) ? "" : (whr == "" ? "" : " AND ") + " (" + whr2 + ") ");
            }

            whr = whr == "" ? "" : " AND " + whr;
            sql = string.Format(sql, whr);

            return sql;
        }

        #endregion

        #region Tab All Task

        public List<SearchObtResult> SearchTabAllTask(SearchObtCondition cond, out string logError)
        {
            //Validate Marketing ต้องเสิสเป็นคู่
            //if (cond.StaffTypeId == SLMConstant.StaffType.Marketing.ToString())
            //{
            //    if ((!string.IsNullOrEmpty(cond.CarLicenseNo) && !string.IsNullOrEmpty(cond.CampaignId))
            //        || (!string.IsNullOrEmpty(cond.ContractNo) && !string.IsNullOrEmpty(cond.CampaignId)))
            //    {
            //        //Do Nothing
            //    }
            //    else
            //    {
            //        return new List<SearchObtResult>();
            //    }
            //}

            SaveActivityLog(cond, "alltask", out logError);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = "";
                List<SearchObtResult> followUpResult = new List<SearchObtResult>();
                List<SearchObtResult> outBoundResult = new List<SearchObtResult>();
                List<SearchObtResult> inBoundResult = new List<SearchObtResult>();
                List<SearchObtResult> mainResult = new List<SearchObtResult>();

                if (cond.IsFollowup && cond.IsOutbound == false && cond.IsInbound == false)     //follow up อย่างเดียว
                {
                    sql = "SELECT A.* FROM ( " + GetOutBoundQuery(cond, true) + @" UNION ALL " + GetInBoundQuery(cond, true) + @" ) A
                        ORDER BY A.NextContactDate ASC ";

                    followUpResult = slmdb.ExecuteStoreQuery<SearchObtResult>(sql, GetParameters(cond)).ToList();
                    followUpResult.ForEach(p => p.ResultFlag = "F");
                }
                else if (cond.IsFollowup && cond.IsOutbound && cond.IsInbound)          //เลือกหมด
                {
                    sql = "SELECT A.* FROM ( " + GetOutBoundQuery(cond, false) + @" UNION ALL " + GetInBoundQuery(cond, false) + @" ) A
                        ORDER BY A.NextContactDate ASC ";

                    //Get ทั้งหมดแล้วมาแยก follow up จาก code ด้านล่าง
                    List<SearchObtResult> temp = slmdb.ExecuteStoreQuery<SearchObtResult>(sql, GetParameters(cond)).ToList();
                    followUpResult = temp.Where(p => p.NextContactDate != null).ToList();
                    temp = temp.Except(followUpResult).ToList();

                    inBoundResult = temp.Where(p => !string.IsNullOrEmpty(p.TicketId)).ToList();
                    outBoundResult = temp.Where(p => string.IsNullOrEmpty(p.TicketId)).ToList();
                }
                else
                {
                    if (cond.IsInbound)
                    {
                        sql = "SELECT A.* FROM ( " + GetInBoundQuery(cond, false) + @" ) A 
                            ORDER BY A.Counting DESC
		                            , CASE WHEN A.NextSLA IS NULL THEN 0 ELSE 1 END DESC
		                            , A.NextSLADiffCurrent ASC, A.NextSLA ASC, A.CreatedDate DESC ";

                        //Get ทั้งหมดแล้วมาแยก follow up จาก code ด้านล่าง
                        inBoundResult = slmdb.ExecuteStoreQuery<SearchObtResult>(sql, GetParameters(cond)).ToList();

                        if (cond.IsFollowup)        //ถ้าเลือก Inbound และเลือก FollowUp ด้วย ให้เอา Inbound ที่มีวันนัดติดต่อครั้งถัดไป แยกออกไปอยู่ใน list FollowUp
                        {
                            var tmplist = inBoundResult.Where(p => p.NextContactDate != null).ToList();
                            inBoundResult = inBoundResult.Except(tmplist).ToList();    //เอา follow up ออกจาก inbound
                            followUpResult = followUpResult.Union(tmplist).ToList();

                            //Get followup ของ Outbound มารวมใน list followup ของ Inbound
                            sql = "SELECT A.* FROM ( " + GetOutBoundQuery(cond, true) + " ) A ORDER BY A.NextContactDate ASC ";
                            var outbound_followup = slmdb.ExecuteStoreQuery<SearchObtResult>(sql, GetParameters(cond)).ToList();
                            followUpResult = followUpResult.Union(outbound_followup).ToList();
                        }
                    }

                    if (cond.IsOutbound)
                    {
                        sql = "SELECT A.* FROM ( " + GetOutBoundQuery(cond, false) + " ) A ORDER BY A.NextContactDate ASC, A.CreatedDate ASC ";

                        //Get ทั้งหมดแล้วมาแยก follow up จาก code ด้านล่าง
                        outBoundResult = slmdb.ExecuteStoreQuery<SearchObtResult>(sql, GetParameters(cond)).ToList();

                        if (cond.IsFollowup)        //ถ้าเลือก OutBound และเลือก FollowUp ด้วย ให้เอา OutBound ที่มีวันนัดติดต่อครั้งถัดไป แยกออกไปอยู่ใน list FollowUp
                        {
                            var tmplist = outBoundResult.Where(p => p.NextContactDate != null).ToList();
                            outBoundResult = outBoundResult.Except(tmplist).ToList();    //เอา follow up ออกจาก outbound
                            followUpResult = followUpResult.Union(tmplist).ToList();

                            //Get followup ของ Inbound มารวมใน list followup ของ Outbound
                            sql = "SELECT A.* FROM ( " + GetInBoundQuery(cond, true) + " ) A ORDER BY A.NextContactDate ASC ";
                            var inbound_followup = slmdb.ExecuteStoreQuery<SearchObtResult>(sql, GetParameters(cond)).ToList();
                            followUpResult = followUpResult.Union(inbound_followup).ToList();
                        }
                    }
                }

                //ใส่ Flag เพื่อไว้ hilight background
                if (followUpResult.Count > 0)
                {
                    followUpResult = followUpResult.OrderBy(p => p.NextContactDate).ToList();
                    followUpResult.ForEach(p => p.ResultFlag = "F");
                    mainResult = mainResult.Union(followUpResult).ToList();
                }
                if (inBoundResult.Count > 0)
                {
                    inBoundResult.ForEach(p => p.ResultFlag = "I");
                    mainResult = mainResult.Union(inBoundResult).ToList();
                }
                if (outBoundResult.Count > 0)
                {
                    outBoundResult.ForEach(p => p.ResultFlag = "O");
                    mainResult = mainResult.Union(outBoundResult).ToList();
                }

                return mainResult;
            }
        }

        private string GetOutBoundQuery(SearchObtCondition cond, bool onlyFollowup)
        {
            string sql = @"SELECT prelead.slm_Prelead_Id AS PreleadId, prelead.slm_Counting AS Counting, prelead.slm_NextContactDate AS NextContactDate, YEAR(prelead.slm_Voluntary_Policy_Exp_Date) AS PolicyExpirationYear, MONTH(prelead.slm_Voluntary_Policy_Exp_Date) AS PolicyExpirationMonth
	                        , prelead.slm_Contract_Number AS ContractNo, prelead.slm_Name AS Firstname, prelead.slm_Lastname AS Lastname, opt.slm_OptionDesc AS StatusDesc, '' AS SubStatusDesc, prelead.slm_Voluntary_Policy_Exp_Date AS PolicyExpireDate
	                        , cardtype.slm_CardTypeName AS CardTypeName, '' AS TicketId, prelead.slm_CitizenId AS CitizenId, campaign.slm_CampaignName AS CampaignName, 'Telesales' AS ChannelDesc
	                        , own.slm_StaffNameTH AS OwnerName, '' AS DelegateName, prelead.slm_CreatedBy AS CreaterName, prelead.slm_CreatedDate AS CreatedDate, prelead.slm_AssignDate AS AssignedDate
	                        , ownerbranch.slm_BranchName AS OwnerBranchName, '' AS DelegateBranchName, '' AS CreaterBranchName, '' AS ContractNoRefer
                            , '' AS NoteFlag, '' AS CalculatorUrl, CONVERT(BIT, 0) AS HasAdamUrl, '' AS AppNo, prelead.slm_Product_Id AS ProductId, '' AS IsCOC
	                        , '' AS COCCurrentTeam, prelead.slm_Status AS StatusCode, prelead.slm_CampaignId AS CampaignId, prelead.slm_SubStatus AS SubStatusCode
                            , prelead.slm_NextSla AS NextSLA, DATEDIFF(minute, GETDATE(), prelead.slm_NextSla) AS NextSLADiffCurrent, prelead.slm_Car_License_No AS LicenseNo
                            , prelead.slm_Grade AS Grade
	                        , NULL AS DelegateDate
	                        , prelead.slm_PeriodYear AS PeriodYear, prelead.slm_PeriodMonth AS PeriodMonth
	                        FROM dbo.kkslm_tr_prelead prelead WITH (NOLOCK) 
	                        LEFT JOIN dbo.kkslm_ms_option opt WITH (NOLOCK) ON prelead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
	                        LEFT JOIN dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) ON cardtype.slm_CardTypeId = prelead.slm_CardTypeId
	                        LEFT JOIN dbo.kkslm_ms_campaign campaign WITH (NOLOCK) ON campaign.slm_CampaignId = prelead.slm_CampaignId
	                        LEFT JOIN dbo.kkslm_ms_staff own WITH (NOLOCK) ON own.slm_EmpCode =  prelead.slm_Owner
	                        LEFT JOIN dbo.kkslm_ms_branch ownerbranch WITH (NOLOCK) ON ownerbranch.slm_BranchCode = prelead.slm_OwnerBranch 
                            LEFT JOIN dbo.kkslm_ms_config_product_substatus substatus WITH (NOLOCK) on prelead.slm_SubStatus = substatus.slm_SubStatusCode and prelead.slm_product_ID = substatus.slm_product_id and prelead.slm_campaignId = substatus.slm_campaignId   
	                        " + JoinNotifyPremium(cond, "prelead") + @"
	                        WHERE prelead.is_Deleted = 0 AND prelead.slm_TicketId IS NULL 
                            AND prelead.slm_Status = '16' 
                            AND prelead.slm_AssignFlag = '1' AND prelead.slm_Assign_Status = '1' ";

            //Validate Marketing ต้องเสิสเป็นคู่
            if (
                new[]
                {
                    SLMConstant.StaffType.Marketing,
                    SLMConstant.StaffType.CallReminder,
                    SLMConstant.StaffType.NormalExpired
                }.Contains(decimal.Parse(cond.StaffTypeId))
            )
            {
                if ((!string.IsNullOrEmpty(cond.CitizenId) && !string.IsNullOrEmpty(cond.CarLicenseNo))
                    || (!string.IsNullOrEmpty(cond.CitizenId) && !string.IsNullOrEmpty(cond.ContractNo))
                    || (!string.IsNullOrEmpty(cond.CarLicenseNo) && !string.IsNullOrEmpty(cond.ContractNo))
                    || (!string.IsNullOrEmpty(cond.ContractNo)))
                {
                    //Do Nothing
                }
                else
                {
                    //sql += " AND 1 = 2 ";   //กรณี outbound ถ้า marketing ไม่เสิสเป็นคู่ จะไม่อนุญาตให้ค้นหาเจอ

                    //กรณี outbound ถ้าไม่เสิสเป็นคู่ ให้ค้นหาแบบ hirarachy เหมือน slm เดิม
                    string recusiveList = GetRecursiveStaff(cond.StaffUsername, true);
                    sql += " AND prelead.slm_Owner IN (" + recusiveList + ") ";
                }
            }

            if (onlyFollowup)
            {
                sql += " AND prelead.slm_NextContactDate IS NOT NULL ";
            }

            sql += " {0} ";

            //เงื่อนไขใน where ของ prelead
            //ไม่มีการค้นหาของ TicketId, ช่องทาง, เลขที่สัญญาที่เคยมีกับธนาคาร, delegateBranch, delegate, flag การขาย ผู้สร้าง(SYSTEM) เพราะไม่มีใน Prelead
            string whr = "";

            if (!string.IsNullOrEmpty(cond.TicketId)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            //Prelead OBT เป็น Channel Telesales เท่านั้น, แต่ใน table Prelead ไม่ได้มีการเก็บ ChannelId
            if (!string.IsNullOrEmpty(cond.ChannelId) && cond.ChannelId.ToUpper().Trim() != "TELESALES") { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (!string.IsNullOrEmpty(cond.ContractNoRefer)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (!string.IsNullOrEmpty(cond.DelegateBranch)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (!string.IsNullOrEmpty(cond.DelegateLead)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (!string.IsNullOrEmpty(cond.CreateByBranch)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (!string.IsNullOrEmpty(cond.CreateBy)) { whr += (whr == "" ? "" : " AND ") + " 1 = 2 "; }
            if (cond.CheckWaitCreditForm || cond.CheckWait50Tawi || cond.CheckWaitDriverLicense || cond.CheckPolicyNo || cond.CheckAct || cond.CheckStopClaim || cond.CheckStopClaim_Cancel)
                whr += (whr == "" ? "" : " AND ") + " 1 = 2 ";

            whr += (string.IsNullOrEmpty(cond.PreleadId) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Prelead_Id = '" + cond.PreleadId + "' ");
            whr += (string.IsNullOrEmpty(cond.ContractNo) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Contract_Number = '" + cond.ContractNo + "' ");
            whr += (string.IsNullOrEmpty(cond.Firstname) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Name LIKE @firstname ");
            whr += (string.IsNullOrEmpty(cond.Lastname) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Lastname LIKE @lastname ");
            whr += (string.IsNullOrEmpty(cond.CardType) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_CardTypeId = '" + cond.CardType + "' ");
            whr += (string.IsNullOrEmpty(cond.CampaignId) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_CampaignId = '" + cond.CampaignId + "' ");

            if (
                new[]
                {
                    SLMConstant.StaffType.Marketing,
                    SLMConstant.StaffType.CallReminder,
                    SLMConstant.StaffType.NormalExpired
                }.Contains(decimal.Parse(cond.StaffTypeId))
            )
            {
                whr += (string.IsNullOrEmpty(cond.CarLicenseNo) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Car_License_No = '" + cond.CarLicenseNo + "' ");
                whr += (string.IsNullOrEmpty(cond.CitizenId) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_CitizenId = '" + cond.CitizenId + "' ");
            }
            else
            {
                whr += (string.IsNullOrEmpty(cond.CarLicenseNo) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Car_License_No LIKE @carlicenseno ");
                whr += (string.IsNullOrEmpty(cond.CitizenId) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_CitizenId LIKE @citizenId ");
            }

            whr += (cond.Grade == null ? "" : (whr == "" ? "" : " AND ") + (cond.Grade.Length > 0 ? $" prelead.slm_Grade IN ('{string.Join("','", cond.Grade)}') " : " prelead.slm_Grade IS NULL "));
            whr += (string.IsNullOrEmpty(cond.HasNotifyPremium) ? "" : (whr == "" ? "" : " AND ") + (cond.HasNotifyPremium == "Y" ? " NP.slm_FlagNotifyPremium = 'Y' " : " NP.slm_FlagNotifyPremium IS NULL "));
            whr += (string.IsNullOrEmpty(cond.NotifyGrossPremiumMin) ? "" : (whr == "" ? "" : " AND ") + $" NP.slm_NetGrossPremium >= {cond.NotifyGrossPremiumMin} ");
            whr += (string.IsNullOrEmpty(cond.NotifyGrossPremiumMax) ? "" : (whr == "" ? "" : " AND ") + $" NP.slm_NetGrossPremium <= {cond.NotifyGrossPremiumMax} ");

            if (cond.NextContactDateFrom.Year != 1 && cond.NextContactDateTo.Year != 1)
            {
                string[] nextContactDate = { cond.NextContactDateFrom.Year.ToString() + cond.NextContactDateFrom.ToString("-MM-dd"), cond.NextContactDateTo.Year.ToString() + cond.NextContactDateTo.ToString("-MM-dd") };
                whr += (nextContactDate[0] == "" && nextContactDate[1] == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, prelead.slm_NextContactDate) BETWEEN '" + nextContactDate[0] + "' AND '" + nextContactDate[1] + "' ");
            }
            if (cond.CreatedDate.Year != 1)
            {
                string createdDate = cond.CreatedDate.Year.ToString() + cond.CreatedDate.ToString("-MM-dd");
                whr += (createdDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, prelead.slm_CreatedDate) = '" + createdDate + "' ");
            }
            if (cond.AssignedDate.Year != 1)
            {
                string assignedDate = cond.AssignedDate.Year.ToString() + cond.AssignedDate.ToString("-MM-dd");
                whr += (assignedDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, prelead.slm_AssignDate) = '" + assignedDate + "' ");
            }

            whr += (string.IsNullOrEmpty(cond.PolicyExpirationYear) ? "" : (whr == "" ? "" : " AND ") + " YEAR(prelead.slm_Voluntary_Policy_Exp_Date) = '" + cond.PolicyExpirationYear + "' ");
            whr += (string.IsNullOrEmpty(cond.PolicyExpirationMonth) ? "" : (whr == "" ? "" : " AND ") + " MONTH(prelead.slm_Voluntary_Policy_Exp_Date) = '" + cond.PolicyExpirationMonth + "' ");
            whr += (string.IsNullOrEmpty(cond.PeriodYear) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_PeriodYear = '" + cond.PeriodYear + "' ");
            whr += (string.IsNullOrEmpty(cond.PeriodMonth) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_PeriodMonth = '" + cond.PeriodMonth + "' ");
            whr += (string.IsNullOrEmpty(cond.OwnerBranch) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_OwnerBranch = '" + cond.OwnerBranch + "' ");

            if (!string.IsNullOrEmpty(cond.OwnerUsername))
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string ownerEmpCode = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == cond.OwnerUsername).Select(p => p.slm_EmpCode).FirstOrDefault();
                    whr += (string.IsNullOrEmpty(ownerEmpCode) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Owner = '" + ownerEmpCode + "' ");
                }
            }

            whr += (string.IsNullOrEmpty(cond.StatusList) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Status IN (" + cond.StatusList + ") ");
            whr += (string.IsNullOrEmpty(cond.SubStatusList) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_SubStatus IN (" + cond.SubStatusList + ") ");

            whr = whr == "" ? "" : " AND " + whr;
            sql = string.Format(sql, whr);

            return sql;
        }

        private string GetInBoundQuery(SearchObtCondition cond, bool onlyFollowup)
        {
            string sql = @"SELECT NULL AS PreleadId, lead.slm_Counting AS Counting, lead.slm_NextContactDate AS NextContactDate, YEAR(prelead.slm_Voluntary_Policy_Exp_Date) AS PolicyExpirationYear, MONTH(prelead.slm_Voluntary_Policy_Exp_Date) AS PolicyExpirationMonth
	                        , renew.slm_ContractNo AS ContractNo, lead.slm_Name AS Firstname, lead.slm_Lastname AS Lastname, opt.slm_OptionDesc AS StatusDesc, lead.slm_ExternalSubStatusDesc AS SubStatusDesc, prelead.slm_Voluntary_Policy_Exp_Date AS PolicyExpireDate
	                        , cardtype.slm_CardTypeName AS CardTypeName, lead.slm_TicketId AS TicketId, info.slm_CitizenId AS CitizenId, campaign.slm_CampaignName AS CampaignName, channel.slm_ChannelDesc AS ChannelDesc
	                        , CASE WHEN ISNULL(poOwner.slm_PositionNameABB,'99999999') = '99999999' THEN own.slm_StaffNameTH
			                        ELSE poOwner.slm_PositionNameABB + ' - ' + own.slm_StaffNameTH  END OwnerName
	                        , CASE WHEN ISNULL(poDelegate.slm_PositionNameABB,'99999999') = '99999999' THEN delegate.slm_StaffNameTH 
			                        ELSE poDelegate.slm_PositionNameABB + ' - ' + delegate.slm_StaffNameTH  END DelegateName
	                        , CASE WHEN ISNULL(createby.slm_StaffNameTH , lead.slm_CreatedBy) = lead.slm_CreatedBy THEN LEAD.slm_CreatedBy
			                        WHEN ISNULL(poCreateby.slm_PositionNameABB,'99999999') = '99999999' THEN createby.slm_StaffNameTH 
			                        ELSE poCreateby.slm_PositionNameABB + ' - ' + createby.slm_StaffNameTH END CreaterName
	                        , lead.slm_CreatedDate AS CreatedDate, lead.slm_AssignedDate AS AssignedDate
	                        , ownerbranch.slm_BranchName AS OwnerBranchName, Delegatebranch.slm_BranchName AS DelegateBranchName, CreateBybranch.slm_BranchName AS CreaterBranchName, lead.slm_ContractNoRefer AS ContractNoRefer
	                        , lead.slm_NoteFlag AS NoteFlag, campaign.slm_Url AS CalculatorUrl, ISNULL(MP.HasADAMUrl, 0) AS HasAdamUrl, lead.coc_Appno AS AppNo, lead.slm_Product_Id AS ProductId, CONVERT(VARCHAR, lead.coc_IsCOC) AS IsCOC
	                        , lead.coc_CurrentTeam AS COCCurrentTeam, lead.slm_status as StatusCode, lead.slm_CampaignId AS CampaignId, lead.slm_SubStatus AS SubStatusCode
                            , lead.slm_NextSLA AS NextSLA, DATEDIFF(minute, GETDATE(), lead.slm_NextSLA) AS NextSLADiffCurrent, renew.slm_LicenseNo AS LicenseNo
                            , prelead.slm_Grade AS Grade
	                        , lead.slm_DelegateDate AS DelegateDate
	                        , renew.slm_PeriodYear AS PeriodYear, renew.slm_PeriodMonth AS PeriodMonth
	                        FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) ";

            if (
                new[]
                {
                    SLMConstant.StaffType.Marketing,
                    SLMConstant.StaffType.CallReminder,
                    SLMConstant.StaffType.NormalExpired
                }.Contains(decimal.Parse(cond.StaffTypeId))
            )
            {
            }
            else
            {
                sql += @" INNER JOIN 
                            (
                                SELECT DISTINCT B.slm_CampaignId
								FROM (
                                        SELECT RTRIM(LTRIM(CP.PR_CampaignId)) AS slm_CampaignId 
                                        FROM kkslm_ms_access_right AR WITH (NOLOCK) 
	                                    INNER  JOIN CMT_CAMPAIGN_PRODUCT CP WITH (NOLOCK) ON CP.PR_ProductId = AR.slm_Product_Id
                                        WHERE AR.slm_Product_Id IS NOT NULL AND
                                        AR.slm_BranchCode = '" + cond.StaffBranchCode + @"' AND AR.slm_StaffTypeId = '" + cond.StaffTypeId + @"'
                                        UNION ALL
                                        SELECT RTRIM(LTRIM(AR.slm_CampaignId))  AS slm_CampaignId  
                                        FROM kkslm_ms_access_right AR WITH (NOLOCK) 
                                        WHERE AR.slm_CampaignId IS NOT NULL AND
                                        AR.slm_BranchCode = '" + cond.StaffBranchCode + @"' AND AR.slm_StaffTypeId = '" + cond.StaffTypeId + @"'
                                    ) B
                            ) AS Z ON Z.slm_CampaignId = lead.slm_CampaignId ";
            }

            sql += " LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance renew WITH (NOLOCK) ON renew.slm_TicketId = lead.slm_TicketId
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option opt WITH (NOLOCK) ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_cusinfo info WITH (NOLOCK) ON info.slm_TicketId = lead.slm_ticketId
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) ON cardtype.slm_CardTypeId = info.slm_CardType
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign campaign WITH (NOLOCK) ON campaign.slm_CampaignId = lead.slm_CampaignId
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_channel channel WITH (NOLOCK) ON channel.slm_ChannelId = lead.slm_ChannelId
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff own WITH (NOLOCK) ON own.slm_UserName = lead.slm_Owner 
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position poOwner WITH (NOLOCK) ON poOwner.slm_Position_id = lead.slm_Owner_Position
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".DBO.kkslm_ms_staff delegate WITH (NOLOCK) ON delegate.slm_UserName = lead.slm_Delegate
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".DBO.kkslm_ms_position poDelegate WITH (NOLOCK) ON poDelegate.slm_Position_id = lead.slm_Delegate_Position
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".DBO.kkslm_ms_staff createby WITH (NOLOCK) ON createby.slm_UserName = lead.slm_CreatedBy
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".DBO.kkslm_ms_position poCreateby WITH (NOLOCK) ON poCreateby.slm_Position_id = lead.slm_CreatedBy_Position 
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".DBO.kkslm_ms_branch ownerbranch WITH (NOLOCK) ON ownerbranch.slm_BranchCode = lead.slm_Owner_Branch 
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".DBO.kkslm_ms_branch Delegatebranch WITH (NOLOCK) ON Delegatebranch.slm_BranchCode = lead.slm_Delegate_Branch
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".DBO.kkslm_ms_branch CreateBybranch WITH (NOLOCK) ON CreateBybranch.slm_BranchCode = lead.slm_CreatedBy_Branch
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT MP WITH (NOLOCK) ON MP.sub_product_id = lead.slm_Product_Id 
	                 LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead prelead WITH (NOLOCK) ON prelead.slm_TicketId = lead.slm_TicketId 
	                 " + JoinNotifyPremium(cond, "renew") + @"
	                 WHERE lead.is_Deleted = 0 ";

            if (onlyFollowup)
            {
                sql += " AND lead.slm_NextContactDate IS NOT NULL ";
            }

            if (
                new[]
                {
                    SLMConstant.StaffType.Marketing,
                    SLMConstant.StaffType.CallReminder,
                    SLMConstant.StaffType.NormalExpired
                }.Contains(decimal.Parse(cond.StaffTypeId))
            )
            {
                if ((!string.IsNullOrEmpty(cond.CitizenId) && !string.IsNullOrEmpty(cond.CarLicenseNo))
                    || (!string.IsNullOrEmpty(cond.CitizenId) && !string.IsNullOrEmpty(cond.ContractNo))
                    || (!string.IsNullOrEmpty(cond.CarLicenseNo) && !string.IsNullOrEmpty(cond.ContractNo))
                    || (!string.IsNullOrEmpty(cond.ContractNo)))
                {
                    //ถ้าข้อมูลถูกส่งเข้ามาเป็นคู่ๆ ให้หาได้หมด
                }
                else
                {
                    //กรณี inbound ถ้าไม่เสิสเป็นคู่ ให้ค้นหาแบบ hirarachy เหมือน slm เดิม
                    string recusiveList = new SearchLeadModel.GetRecursiveStaff(cond.StaffUsername, true).JoinedUsername;
                    sql += " AND (lead.slm_Owner IN (" + recusiveList + ") OR lead.slm_Delegate IN (" + recusiveList + @")) ";
                }

                if (!string.IsNullOrEmpty(cond.CarLicenseNo))
                    sql += " AND renew.slm_LicenseNo = '" + cond.CarLicenseNo + "' ";
                if (!string.IsNullOrEmpty(cond.CitizenId))
                    sql += " AND info.slm_CitizenId = '" + cond.CitizenId + "' ";
            }
            else
            {
                if (!string.IsNullOrEmpty(cond.CarLicenseNo))
                    sql += " AND renew.slm_LicenseNo LIKE @carlicenseno ";
                if (!string.IsNullOrEmpty(cond.CitizenId))
                    sql += " AND info.slm_CitizenId LIKE @citizenId ";
            }

            sql += " {0} ";

            string whr = "";

            whr += (string.IsNullOrEmpty(cond.ContractNo) ? "" : (whr == "" ? "" : " AND ") + " renew.slm_ContractNo = '" + cond.ContractNo + "' ");
            whr += (string.IsNullOrEmpty(cond.TicketId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_TicketId = '" + cond.TicketId + "' ");
            whr += (string.IsNullOrEmpty(cond.Firstname) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Name LIKE @firstname ");
            whr += (string.IsNullOrEmpty(cond.Lastname) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Lastname LIKE @lastname ");
            whr += (string.IsNullOrEmpty(cond.CardType) ? "" : (whr == "" ? "" : " AND ") + " info.slm_CardType = '" + cond.CardType + "' ");
            whr += (string.IsNullOrEmpty(cond.ChannelId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_ChannelId = '" + cond.ChannelId + "' ");
            whr += (string.IsNullOrEmpty(cond.CampaignId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CampaignId = '" + cond.CampaignId + "' ");
            whr += (string.IsNullOrEmpty(cond.PolicyExpirationYear) ? "" : (whr == "" ? "" : " AND ") + " YEAR(prelead.slm_Voluntary_Policy_Exp_Date) = '" + cond.PolicyExpirationYear + "' ");
            whr += (string.IsNullOrEmpty(cond.PolicyExpirationMonth) ? "" : (whr == "" ? "" : " AND ") + " MONTH(prelead.slm_Voluntary_Policy_Exp_Date) = '" + cond.PolicyExpirationMonth + "' ");
            whr += (string.IsNullOrEmpty(cond.PeriodYear) ? "" : (whr == "" ? "" : " AND ") + " renew.slm_PeriodYear = '" + cond.PeriodYear + "' ");
            whr += (string.IsNullOrEmpty(cond.PeriodMonth) ? "" : (whr == "" ? "" : " AND ") + " renew.slm_PeriodMonth = '" + cond.PeriodMonth + "' ");
            whr += (string.IsNullOrEmpty(cond.ContractNoRefer) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_ContractNoRefer LIKE @contractnorefer ");
            whr += (cond.Grade == null ? "" : (whr == "" ? "" : " AND ") + (cond.Grade.Length > 0 ? $" prelead.slm_Grade IN ('{string.Join("','", cond.Grade)}') " : " prelead.slm_Grade IS NULL "));
            whr += (string.IsNullOrEmpty(cond.HasNotifyPremium) ? "" : (whr == "" ? "" : " AND ") + (cond.HasNotifyPremium == "Y" ? " NP.slm_FlagNotifyPremium = 'Y' " : " NP.slm_FlagNotifyPremium IS NULL "));
            whr += (string.IsNullOrEmpty(cond.NotifyGrossPremiumMin) ? "" : (whr == "" ? "" : " AND ") + $" NP.slm_NetGrossPremium >= {cond.NotifyGrossPremiumMin} ");
            whr += (string.IsNullOrEmpty(cond.NotifyGrossPremiumMax) ? "" : (whr == "" ? "" : " AND ") + $" NP.slm_NetGrossPremium <= {cond.NotifyGrossPremiumMax} ");

            if (cond.NextContactDateFrom.Year != 1)
            {
                string nextContactDate = cond.NextContactDateFrom.Year.ToString() + cond.NextContactDateFrom.ToString("-MM-dd");
                whr += (nextContactDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_NextContactDate) = '" + nextContactDate + "' ");
            }
            if (cond.CreatedDate.Year != 1)
            {
                string createdDate = cond.CreatedDate.Year.ToString() + cond.CreatedDate.ToString("-MM-dd");
                whr += (createdDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_CreatedDate) = '" + createdDate + "' ");
            }
            if (cond.AssignedDate.Year != 1)
            {
                string assignedDate = cond.AssignedDate.Year.ToString() + cond.AssignedDate.ToString("-MM-dd");
                whr += (assignedDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_AssignedDate) = '" + assignedDate + "' ");
            }

            whr += (string.IsNullOrEmpty(cond.OwnerBranch) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Owner_Branch = '" + cond.OwnerBranch + "' ");
            whr += (string.IsNullOrEmpty(cond.OwnerUsername) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Owner = '" + cond.OwnerUsername + "' ");
            whr += (string.IsNullOrEmpty(cond.DelegateBranch) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Delegate_Branch = '" + cond.DelegateBranch + "' ");
            whr += (string.IsNullOrEmpty(cond.DelegateLead) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Delegate = '" + cond.DelegateLead + "' ");
            whr += (string.IsNullOrEmpty(cond.CreateByBranch) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CreatedBy_Branch = '" + cond.CreateByBranch + "' ");
            whr += (string.IsNullOrEmpty(cond.CreateBy) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CreatedBy = '" + cond.CreateBy + "' ");
            whr += (string.IsNullOrEmpty(cond.StatusList) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Status IN (" + cond.StatusList + ") ");
            whr += (string.IsNullOrEmpty(cond.SubStatusList) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_SubStatus IN (" + cond.SubStatusList + ") ");

            if (cond.CheckWaitCreditForm || cond.CheckWait50Tawi || cond.CheckWaitDriverLicense || cond.CheckPolicyNo || cond.CheckAct || cond.CheckStopClaim || cond.CheckStopClaim_Cancel)
            {
                string whr2 = "";
                whr2 += (cond.CheckWaitCreditForm == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_CreditFlag = 'Y' ");
                whr2 += (cond.CheckWait50Tawi == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_50TawiFlag = 'Y' ");
                whr2 += (cond.CheckWaitDriverLicense == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_DriverLicenseFlag = 'Y' ");
                whr2 += (cond.CheckPolicyNo == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_PolicyFlag = 'Y' ");
                whr2 += (cond.CheckAct == false ? "" : (whr2 == "" ? "" : " OR ") + " renew.slm_Need_CompulsoryFlag = 'Y' ");

                if (cond.CheckStopClaim && cond.CheckStopClaim_Cancel)
                    whr2 += (whr2 == "" ? "" : " OR ") + " renew.slm_ClaimFlag IN ('0','1') ";
                else if (cond.CheckStopClaim)
                    whr2 += (whr2 == "" ? "" : " OR ") + " renew.slm_ClaimFlag = '1' ";
                else if (cond.CheckStopClaim_Cancel)
                    whr2 += (whr2 == "" ? "" : " OR ") + " renew.slm_ClaimFlag = '0' ";

                whr += (string.IsNullOrEmpty(whr2) ? "" : (whr == "" ? "" : " AND ") + " (" + whr2 + ") ");
            }

            whr = whr == "" ? "" : " AND " + whr;
            sql = string.Format(sql, whr);

            return sql;
        }

        public string GetRecursiveStaff(string username, bool includedeleted = false)
        {
            string userList = "";
            KKSlmMsStaffModel sModel = new KKSlmMsStaffModel();

            List<StaffData> staffList = sModel.GetStaffList(includedeleted);

            StaffData sData = sModel.GetStaff(username);
            int? staffId = sData.StaffId;

            ArrayList arr = new ArrayList();
            arr.Add("'" + sData.EmpCode + "'");    //เก็บ login staff

            FindStaffRecusive(staffId, arr, staffList);

            foreach (string tmp_empcode in arr)
            {
                userList += (userList == "" ? "" : ",") + tmp_empcode;
            }

            return userList;
        }

        private void FindStaffRecusive(int? headId, ArrayList arr, List<StaffData> staffList)
        {
            foreach (StaffData staff in staffList)
            {
                if (staff.HeadStaffId == headId)
                {
                    arr.Add("'" + staff.EmpCode + "'");
                    FindStaffRecusive(staff.StaffId, arr, staffList);
                }
            }
        }

        #endregion


        //======================================================================================================================
        //        public List<SearchObtResult> SearchObtData(string ticketId, string firstname, string lastname, string cardTypeId, string citizenId, string campaignId, string channelId, string ownerLeadUsername,
        //            string createDate, string assignDate, string leadStatusList, string username, decimal? stafftype, string ownerBranch, string delegateBranch, string delegateLead, string contractNoRefer,
        //            string createByBranch, string createBy, string orderByFlag, string carLicenseNo, string nextAppointment, string year, string month, bool isFollowup, bool isInbound, bool isOutbound, bool isTaskList, bool isTabFollowup, string subStatusList, string contractNo)
        //        {
        //            string sql = "";
        //            string whr = "";
        //            if (stafftype != SLMConstant.StaffType.Marketing &&
        //                stafftype != SLMConstant.StaffType.ManagerOper &&
        //                stafftype != SLMConstant.StaffType.SupervisorOper &&
        //                stafftype != SLMConstant.StaffType.Oper)
        //            {
        //                KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
        //                string UserLoginBranchCode = staff.GetBrachCode(username);
        //                if (isInbound)
        //                {
        //                    sql += GetInboundSqlQuery(ticketId, firstname, lastname, cardTypeId, citizenId, campaignId, channelId, ownerLeadUsername, createDate, assignDate
        //                        , leadStatusList, username, stafftype, ownerBranch, delegateBranch, delegateLead, contractNoRefer, createByBranch, createBy, orderByFlag
        //                        , carLicenseNo, nextAppointment, year, month, false, isTaskList, isTabFollowup, contractNo);
        //                }
        //                if (isOutbound)
        //                {
        //                    if (sql != "")
        //                        sql += " Union ";
        //                    sql += GetOutboundSqlQuery(ticketId, firstname, lastname, cardTypeId, citizenId, campaignId, channelId, ownerLeadUsername, createDate, assignDate
        //                        , leadStatusList, username, stafftype, ownerBranch, delegateBranch, delegateLead, contractNoRefer, createByBranch, createBy, orderByFlag
        //                        , carLicenseNo, nextAppointment, year, month, false, isTaskList, isTabFollowup, subStatusList, contractNo);
        //                }
        //                if (isFollowup)
        //                {
        //                    if (sql != "")
        //                        sql += " Union ";
        //                    sql += GetInboundSqlQuery(ticketId, firstname, lastname, cardTypeId, citizenId, campaignId, channelId, ownerLeadUsername, createDate, assignDate
        //                        , leadStatusList, username, stafftype, ownerBranch, delegateBranch, delegateLead, contractNoRefer, createByBranch, createBy, orderByFlag
        //                        , carLicenseNo, nextAppointment, year, month, isFollowup, isTaskList, isTabFollowup, contractNo);
        //                    sql += " Union ";
        //                    sql += GetOutboundSqlQuery(ticketId, firstname, lastname, cardTypeId, citizenId, campaignId, channelId, ownerLeadUsername, createDate, assignDate
        //                        , leadStatusList, username, stafftype, ownerBranch, delegateBranch, delegateLead, contractNoRefer, createByBranch, createBy, orderByFlag
        //                        , carLicenseNo, nextAppointment, year, month, isFollowup, isTaskList, isTabFollowup, subStatusList, contractNo);
        //                }
        //            }

        //            sql = string.Format(sql, whr);

        //            object[] param = new object[] 
        //            { 
        //                new SqlParameter("@firstname", "%" + firstname + "%")
        //                , new SqlParameter("@lastname", "%" + lastname + "%")
        //                , new SqlParameter("@contractnorefer", "%" + contractNoRefer + "%")
        //            };
        //            if (sql == "")
        //                return null;
        //            else
        //                return slmdb.ExecuteStoreQuery<SearchObtResult>(sql, param).ToList();
        //        }

        //        public String GetInboundSqlQuery(string ticketId, string firstname, string lastname, string cardTypeId, string citizenId, string campaignId, string channelId, string ownerLeadUsername,
        //            string createDate, string assignDate, string leadStatusList, string username, decimal? stafftype, string ownerBranch, string delegateBranch, string delegateLead, string contractNoRefer,
        //            string createByBranch, string createBy, string orderByFlag, string carLicenseNo, string nextAppointment, string year, string month, bool isFollowup, bool isTaskList, bool isTabFollowup, string contractNo)
        //        {
        //            string sql = "";
        //            string whr = "";
        //            string status = isFollowup ? SLMConstant.ObtRecordType.Followup : SLMConstant.ObtRecordType.Inbound;
        //            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
        //            string UserLoginBranchCode = staff.GetBrachCode(username);
        //            decimal? staffType = staff.GetStaffType(username);

        //            sql += @" SELECT A.* FROM(
        //                            SELECT DISTINCT result.*
        //                            FROM (
        //	                            SELECT lead.slm_ticketId AS TicketId, lead.slm_Counting AS Counting, lead.slm_Name AS Firstname, opt.slm_OptionDesc AS StatusDesc, 
        //			                            cardtype.slm_CardTypeName AS CardTypeDesc,
        //                                        info.slm_CitizenId AS CitizenId, info.slm_LastName AS LastName, campaign.slm_CampaignName AS CampaignName, channel.slm_ChannelDesc AS ChannelDesc, 
        //			                             CASE WHEN ISNULL(poOwner.slm_PositionNameABB,'99999999') = '99999999' THEN staff.slm_StaffNameTH
        //					                            ELSE poOwner.slm_PositionNameABB +' - '+staff.slm_StaffNameTH  END OwnerName, 
        //                                        lead.slm_CreatedDate AS CreatedDate, lead.slm_AssignedDate AS AssignedDate, 
        //                                        lead.slm_NoteFlag AS NoteFlag, staff.slm_StaffId,
        //			                            CASE WHEN ISNULL(poDelegate.slm_PositionNameABB,'99999999') = '99999999' THEN delegate.slm_StaffNameTH 
        //					                        ELSE poDelegate.slm_PositionNameABB+' - '+delegate.slm_StaffNameTH  END DelegateName,
        //                                        ownerbranch.slm_BranchName AS OwnerBranchName,
        //			                            Delegatebranch.slm_BranchName AS DelegateBranchName,
        //                                        CASE WHEN ISNULL(createby.slm_StaffNameTH ,LEAD.slm_CreatedBy) = LEAD.slm_CreatedBy THEN LEAD.slm_CreatedBy
        //					                        WHEN ISNULL(poCreateby.slm_PositionNameABB,'99999999') = '99999999' THEN createby.slm_StaffNameTH 
        //					                            ELSE poCreateby.slm_PositionNameABB+' - '+createby.slm_StaffNameTH END CreateName,
        //			                            CreateBybranch.slm_BranchName AS BranchCreateBranchName, lead.slm_Product_Name AS ProductName, ISNULL(MP.HasADAMUrl, 0) AS HasAdamUrl
        //                                        , lead.slm_CampaignId AS CampaignId, prodinfo.slm_LicenseNo AS LicenseNo, lead.slm_TelNo_1 AS TelNo1, prodinfo.slm_ProvinceRegis AS ProvinceRegis
        //                                        , campaign.slm_Url AS CalculatorUrl, lead.slm_Product_Group_Id AS ProductGroupId, lead.slm_Product_Id AS ProductId, lead.coc_Appno AS AppNo
        //                                        , CONVERT(VARCHAR,lead.coc_IsCOC) AS IsCOC,lead.slm_status as slmStatusCode, lead.slm_ContractNoRefer AS ContractNoRefer, lead.coc_CurrentTeam AS COCCurrentTeam
        //                                        , lead.slm_ExternalSubStatusDesc AS ExternalSubStatusDesc, lead.slm_NextSLA AS NextSLA, DATEDIFF(minute, GETDATE(), lead.slm_NextSLA) AS NextSLADiffCurrent , '" + status + @"'  as recordType
        //	                            FROM " + SLMDBName + @".DBO.kkslm_tr_lead lead INNER JOIN 
        //	                            (
        //                                    SELECT CP.PR_CampaignId AS slm_CampaignId FROM kkslm_ms_access_right AR INNER  JOIN CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
        //                                    WHERE AR.slm_Product_Id IS NOT NULL AND
        //                                    AR.slm_BranchCode = '" + UserLoginBranchCode + @"' AND AR.slm_StaffTypeId = '" + stafftype + @"'
        //                                    UNION ALL
        //                                    SELECT AR.slm_CampaignId  AS slm_CampaignId  FROM kkslm_ms_access_right AR 
        //                                    WHERE AR.slm_CampaignId IS NOT NULL AND
        //                                    AR.slm_BranchCode = '" + UserLoginBranchCode + @"' AND AR.slm_StaffTypeId = '" + stafftype + @"'
        //                                ) AS Z ON Z.slm_CampaignId = lead.slm_CampaignId 
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_cusinfo info ON lead.slm_ticketId = info.slm_TicketId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign campaign ON lead.slm_CampaignId = campaign.slm_CampaignId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_channel channel ON lead.slm_ChannelId = channel.slm_ChannelId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff ON lead.slm_Owner = staff.slm_UserName and staff.is_Deleted = 0  
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position poOwner on lead.slm_Owner_Position = poOwner.slm_Position_id 
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_option opt ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_staff delegate on delegate.slm_UserName = lead.slm_Delegate
        //                                LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_position poDelegate on lead.slm_Delegate_Position = poDelegate.slm_Position_id 
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_branch ownerbranch on lead.slm_Owner_Branch = ownerbranch.slm_BranchCode 
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_branch Delegatebranch on lead.slm_Delegate_Branch = Delegatebranch.slm_BranchCode 
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_staff createby on createby.slm_UserName = lead.slm_CreatedBy
        //                                LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_position poCreateby on lead.slm_CreatedBy_Position = poCreateby.slm_Position_id 
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_branch CreateBybranch on lead.slm_CreatedBy_Branch = CreateBybranch.slm_BranchCode 
        //                                LEFT JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT MP ON lead.slm_Product_Id = MP.sub_product_id
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_productinfo prodinfo ON lead.slm_ticketId = prodinfo.slm_TicketId
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_cardtype cardtype ON info.slm_CardType = cardtype.slm_CardTypeId
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_renewinsurance renew ON lead.slm_TicketId = renew.slm_TicketId
        //	                            WHERE lead.is_Deleted = 0 {0} 
        //	                            UNION ALL
        //	                            SELECT lead.slm_ticketId AS TicketId, lead.slm_Counting AS Counting, lead.slm_Name AS Firstname, opt.slm_OptionDesc AS StatusDesc, 
        //			                            cardtype.slm_CardTypeName AS CardTypeDesc,
        //                                        info.slm_CitizenId AS CitizenId, info.slm_LastName AS LastName, campaign.slm_CampaignName AS CampaignName, channel.slm_ChannelDesc AS ChannelDesc, 
        //			                            CASE WHEN ISNULL(poOwner.slm_PositionNameABB,'99999999') = '99999999' THEN staff.slm_StaffNameTH
        //					                        ELSE poOwner.slm_PositionNameABB +' - '+staff.slm_StaffNameTH  END OwnerName, 
        //                                        lead.slm_CreatedDate AS CreatedDate, 
        //                                        lead.slm_AssignedDate AS AssignedDate, lead.slm_NoteFlag AS NoteFlag, staff.slm_StaffId,
        //                                        CASE WHEN ISNULL(poDelegate.slm_PositionNameABB,'99999999') = '99999999' THEN delegate.slm_StaffNameTH 
        //					                            ELSE poDelegate.slm_PositionNameABB+' - '+delegate.slm_StaffNameTH  END DelegateName,
        //                                        ownerbranch.slm_BranchName AS OwnerBranchName,
        //			                            Delegatebranch.slm_BranchName AS DelegateBranchName,
        //                                        CASE WHEN ISNULL(createby.slm_StaffNameTH ,LEAD.slm_CreatedBy) = LEAD.slm_CreatedBy THEN LEAD.slm_CreatedBy
        //					                            WHEN ISNULL(poCreateby.slm_PositionNameABB,'99999999') = '99999999' THEN createby.slm_StaffNameTH 
        //					                            ELSE poCreateby.slm_PositionNameABB+' - '+createby.slm_StaffNameTH END CreateName,
        //			                            CreateBybranch.slm_BranchName AS BranchCreateBranchName, lead.slm_Product_Name AS ProductName, ISNULL(MP.HasADAMUrl, 0) AS HasAdamUrl
        //                                        , lead.slm_CampaignId AS CampaignId, prodinfo.slm_LicenseNo AS LicenseNo, lead.slm_TelNo_1 AS TelNo1, prodinfo.slm_ProvinceRegis AS ProvinceRegis
        //                                        , campaign.slm_Url AS CalculatorUrl, lead.slm_Product_Group_Id AS ProductGroupId, lead.slm_Product_Id AS ProductId, lead.coc_Appno AS AppNo
        //                                        , CONVERT(VARCHAR,lead.coc_IsCOC) AS IsCOC,lead.slm_status as slmStatusCode, lead.slm_ContractNoRefer AS ContractNoRefer, lead.coc_CurrentTeam AS COCCurrentTeam
        //                                        , lead.slm_ExternalSubStatusDesc AS ExternalSubStatusDesc, lead.slm_NextSLA AS NextSLA, DATEDIFF(minute, GETDATE(), lead.slm_NextSLA) AS NextSLADiffCurrent , '" + status + @"'  as recordType
        //	                            FROM " + SLMDBName + @".dbo.kkslm_tr_lead lead
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_cusinfo info ON lead.slm_ticketId = info.slm_TicketId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign campaign ON lead.slm_CampaignId = campaign.slm_CampaignId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_channel channel ON lead.slm_ChannelId = channel.slm_ChannelId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff ON lead.slm_Owner = staff.slm_UserName and staff.is_Deleted = 0
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position poOwner on lead.slm_Owner_Position = poOwner.slm_Position_id 
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_option opt ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' 
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_staff delegate on delegate.slm_UserName = lead.slm_Delegate
        //                                LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_position poDelegate on lead.slm_Delegate_Position = poDelegate.slm_Position_id
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_branch ownerbranch on lead.slm_Owner_Branch = ownerbranch.slm_BranchCode 
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_branch Delegatebranch on lead.slm_Delegate_Branch = Delegatebranch.slm_BranchCode 
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_staff createby on createby.slm_UserName = lead.slm_CreatedBy
        //                                LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_position poCreateby on lead.slm_CreatedBy_Position = poCreateby.slm_Position_id 
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_branch CreateBybranch on lead.slm_CreatedBy_Branch = CreateBybranch.slm_BranchCode 
        //                                LEFT JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT MP ON lead.slm_Product_Id = MP.sub_product_id
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_productinfo prodinfo ON lead.slm_ticketId = prodinfo.slm_TicketId
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_cardtype cardtype ON info.slm_CardType = cardtype.slm_CardTypeId
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_renewinsurance renew ON lead.slm_TicketId = renew.slm_TicketId
        //	                            WHERE lead.slm_Delegate = '" + username + @"' AND lead.is_Deleted = 0 {0}   
        //                                 ) AS result 
        //                                ) A ";

        //            if (orderByFlag == SLMConstant.SearchOrderBy.SLA)
        //            {
        //                sql += @" ORDER BY A.Counting DESC
        //				                        , CASE WHEN A.NextSLA IS NULL THEN 0 ELSE 1 END DESC
        //				                        , A.NextSLADiffCurrent ASC, A.NextSLA ASC, A.CreatedDate DESC ";
        //            }
        //            else if (orderByFlag == SLMConstant.SearchOrderBy.Note)
        //            {
        //                sql += @" ORDER BY CASE WHEN A.NoteFlag IS NULL THEN '0' 
        //					                    WHEN A.NoteFlag = '0' THEN '0'
        //					                    WHEN A.NoteFlag = '1' THEN '1' END DESC, A.CreatedDate DESC";
        //            }

        //            if (isFollowup)
        //            {
        //                if (isTaskList)
        //                {
        //                    whr += (String.IsNullOrEmpty(leadStatusList) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Status NOT IN (" + leadStatusList + ")" );
        //                    leadStatusList = "";
        //                }
        //                whr += (whr == "" ? "" : " AND ") + " lead.slm_NextContactDate is not null";
        //                if (isTabFollowup)
        //                    whr += (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_NextContactDate) <= CONVERT(DATE, GETDATE()) ";
        //            }
        //            whr += (String.IsNullOrEmpty(ticketId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_ticketId LIKE '%" + ticketId + "%' ");
        //            whr += (String.IsNullOrEmpty(firstname) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Name LIKE @firstname ");
        //            whr += (String.IsNullOrEmpty(lastname) ? "" : (whr == "" ? "" : " AND ") + " info.slm_LastName LIKE @lastname ");
        //            whr += (String.IsNullOrEmpty(contractNoRefer) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_ContractNoRefer LIKE @contractnorefer ");
        //            whr += (String.IsNullOrEmpty(citizenId) ? "" : (whr == "" ? "" : " AND ") + " info.slm_CitizenId LIKE '%" + citizenId + "%' ");
        //            whr += (String.IsNullOrEmpty(cardTypeId) ? "" : (whr == "" ? "" : " AND ") + " info.slm_CardType = '" + cardTypeId + "' ");
        //            whr += (String.IsNullOrEmpty(campaignId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CampaignId = '" + campaignId + "' ");
        //            whr += (String.IsNullOrEmpty(channelId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_ChannelId = '" + channelId + "' ");
        //            whr += (String.IsNullOrEmpty(ownerLeadUsername) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Owner = '" + ownerLeadUsername + "' ");      //Owner Lead
        //            whr += (String.IsNullOrEmpty(createDate) ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_CreatedDate) = '" + createDate + "' ");
        //            whr += (String.IsNullOrEmpty(assignDate) ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_AssignedDate) = '" + assignDate + "' ");
        //            whr += (String.IsNullOrEmpty(ownerBranch) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Owner_Branch = '" + ownerBranch + "' ");           //Owner Branch
        //            whr += (String.IsNullOrEmpty(delegateBranch) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Delegate_Branch = '" + delegateBranch + "' ");  //Delegate Branch
        //            whr += (String.IsNullOrEmpty(delegateLead) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Delegate = '" + delegateLead + "' ");             //Delegate Lead
        //            whr += (String.IsNullOrEmpty(createByBranch) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CreatedBy_Branch = '" + createByBranch + "' "); //CreateBy Branch
        //            whr += (String.IsNullOrEmpty(createBy) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CreatedBy = '" + createBy + "' ");                    //CreateBy Lead
        //            whr += (String.IsNullOrEmpty(leadStatusList) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Status IN (" + leadStatusList + ") ");
        //            whr += (String.IsNullOrEmpty(carLicenseNo) ? "" : (whr == "" ? "" : " AND ") + " renew.slm_LicenseNo = '" + carLicenseNo + "' ");
        //            whr += (nextAppointment == DateTime.MinValue.ToString() ? "" : (whr == "" ? "" : " AND ") + " lead.slm_NextContactDate like '" + nextAppointment + "' ");
        //            whr += (String.IsNullOrEmpty(year) ? "" : (whr == "" ? "" : " AND ") + " Year(renew.slm_PolicyStartCoverDate) = '" + year + "' ");
        //            whr += (String.IsNullOrEmpty(month) ? "" : (whr == "" ? "" : " AND ") + " Month(renew.slm_PolicyStartCoverDate) = '" + month + "' ");

        //            if (!String.IsNullOrEmpty(carLicenseNo))
        //            {
        //                whr += whr == "" ? "" : " AND ";
        //                whr += staffType == 8 ? " renew.slm_LicenseNo = '" + carLicenseNo + "' " : " renew.slm_LicenseNo like '%" + carLicenseNo + "%'";
        //            }
        //            if (!String.IsNullOrEmpty(contractNo))
        //            {
        //                whr += whr == "" ? "" : " AND ";
        //                whr += staffType == 8 ? " renew.slm_ContractNo = '" + contractNo + "' " : " renew.slm_ContractNo like '%" + contractNo + "%'";
        //            }

        //            whr += "";

        //            whr = whr == "" ? "" : " AND " + whr;
        //            sql = string.Format(sql, whr);

        //            return sql;
        //        }

        //        public String GetOutboundSqlQuery(string ticketId, string firstname, string lastname, string cardTypeId, string citizenId, string campaignId, string channelId, string ownerLeadUsername,
        //            string createDate, string assignDate, string leadStatusList, string username, decimal? stafftype, string ownerBranch, string delegateBranch, string delegateLead, string contractNoRefer,
        //            string createByBranch, string createBy, string orderByFlag, string carLicenseNo, string nextAppointment, string year, string month, bool isFollowup, bool isTaskList, bool isTabFollowup, string subStatusList, string contractNo)
        //        {
        //            string sql = "";
        //            string whr = "";
        //            string status = isFollowup ? SLMConstant.ObtRecordType.Followup : SLMConstant.ObtRecordType.Outbound;
        //            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
        //            decimal? staffType = staff.GetStaffType(username);
        //            string UserLoginBranchCode = staff.GetBrachCode(username);
        //            string ownerLeadEmployeeCode = staff.GetEmployeeCode(ownerLeadUsername);
        //            sql += @" SELECT B.* FROM(
        //                            SELECT DISTINCT result.*
        //                            FROM (
        //	                            SELECT lead.slm_ticketId AS TicketId, lead.slm_Counting AS Counting, lead.slm_Name AS Firstname, opt.slm_SubStatusName AS StatusDesc, 
        //			                            cardtype.slm_CardTypeName AS CardTypeDesc,
        //                                        info.slm_CitizenId AS CitizenId, info.slm_LastName AS LastName, campaign.slm_CampaignName AS CampaignName, '' AS ChannelDesc,
        //			                             CASE WHEN ISNULL(poOwner.slm_PositionNameABB,'99999999') = '99999999' THEN staff.slm_StaffNameTH
        //					                            ELSE poOwner.slm_PositionNameABB +' - '+staff.slm_StaffNameTH  END OwnerName, 
        //                                        lead.slm_CreatedDate AS CreatedDate, lead.slm_AssignDate AS AssignedDate, 
        //                                        '' AS NoteFlag, staff.slm_StaffId,'' as DelegateName,
        //                                        '' as OwnerBranchName,
        //			                            '' AS DelegateBranchName,
        //                                        CASE WHEN ISNULL(createby.slm_StaffNameTH ,LEAD.slm_CreatedBy) = LEAD.slm_CreatedBy THEN LEAD.slm_CreatedBy 
        //					                            ELSE createby.slm_StaffNameTH END CreateName,
        //			                            '' AS BranchCreateBranchName, '' AS ProductName, ISNULL(MP.HasADAMUrl, 0) AS HasAdamUrl
        //                                        , lead.slm_CampaignId AS CampaignId, prodinfo.slm_LicenseNo AS LicenseNo, '' AS TelNo1, prodinfo.slm_ProvinceRegis AS ProvinceRegis
        //                                        , campaign.slm_Url AS CalculatorUrl, '' AS ProductGroupId, lead.slm_Product_Id AS ProductId, '' AS AppNo
        //                                        , '' AS IsCOC,lead.slm_status as slmStatusCode, '' AS ContractNoRefer, '' AS COCCurrentTeam
        //                                        , '' AS ExternalSubStatusDesc, lead.slm_NextSLA AS NextSLA, DATEDIFF(minute, GETDATE(), lead.slm_NextSLA) AS NextSLADiffCurrent , '" + status + @"'  as recordType
        //	                            FROM " + SLMDBName + @".DBO.kkslm_tr_prelead lead INNER JOIN 
        //	                            (
        //                                    SELECT CP.PR_CampaignId AS slm_CampaignId FROM kkslm_ms_access_right AR INNER  JOIN CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
        //                                    WHERE AR.slm_Product_Id IS NOT NULL AND
        //                                    AR.slm_BranchCode = '" + UserLoginBranchCode + @"' AND AR.slm_StaffTypeId = '" + stafftype + @"'
        //                                    UNION ALL
        //                                    SELECT AR.slm_CampaignId  AS slm_CampaignId  FROM kkslm_ms_access_right AR 
        //                                    WHERE AR.slm_CampaignId IS NOT NULL AND
        //                                    AR.slm_BranchCode = '" + UserLoginBranchCode + @"' AND AR.slm_StaffTypeId = '" + stafftype + @"'
        //                                ) AS Z ON Z.slm_CampaignId = lead.slm_CampaignId 
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_cusinfo info ON lead.slm_ticketId = info.slm_TicketId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign campaign ON lead.slm_CampaignId = campaign.slm_CampaignId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff ON lead.slm_Owner = staff.slm_EmpCode and staff.is_Deleted = 0  
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position poOwner on lead.slm_Owner_Position = poOwner.slm_Position_id 
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus opt ON opt.slm_SubStatusCode = lead.slm_SubStatus and opt.slm_Product_Id = lead.slm_Product_Id
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_staff createby on createby.slm_UserName = lead.slm_CreatedBy
        //                                LEFT JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT MP ON lead.slm_Product_Id = MP.sub_product_id
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_productinfo prodinfo ON lead.slm_ticketId = prodinfo.slm_TicketId
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_cardtype cardtype ON info.slm_CardType = cardtype.slm_CardTypeId
        //	                            WHERE lead.is_Deleted = 'false' and lead.slm_TicketId is null {0} 
        //	                            UNION ALL
        //	                            SELECT lead.slm_ticketId AS TicketId, lead.slm_Counting AS Counting, lead.slm_Name AS Firstname, opt.slm_SubStatusName AS StatusDesc, 
        //			                            cardtype.slm_CardTypeName AS CardTypeDesc,
        //                                        info.slm_CitizenId AS CitizenId, info.slm_LastName AS LastName, campaign.slm_CampaignName AS CampaignName, '' AS ChannelDesc,
        //			                            CASE WHEN ISNULL(poOwner.slm_PositionNameABB,'99999999') = '99999999' THEN staff.slm_StaffNameTH
        //					                        ELSE poOwner.slm_PositionNameABB +' - '+staff.slm_StaffNameTH  END OwnerName, 
        //                                        lead.slm_CreatedDate AS CreatedDate, 
        //                                        lead.slm_AssignDate AS AssignedDate, '' AS NoteFlag, staff.slm_StaffId,
        //                                        '' as DelegateName,
        //                                        '' AS OwnerBranchName,
        //			                            '' AS DelegateBranchName,
        //                                        CASE WHEN ISNULL(createby.slm_StaffNameTH ,LEAD.slm_CreatedBy) = LEAD.slm_CreatedBy THEN LEAD.slm_CreatedBy
        //					                            ELSE '' END CreateName,
        //			                            '' AS BranchCreateBranchName, '' AS ProductName, ISNULL(MP.HasADAMUrl, 0) AS HasAdamUrl
        //                                        , lead.slm_CampaignId AS CampaignId, prodinfo.slm_LicenseNo AS LicenseNo, '' AS TelNo1, prodinfo.slm_ProvinceRegis AS ProvinceRegis
        //                                        , campaign.slm_Url AS CalculatorUrl, '' AS ProductGroupId, lead.slm_Product_Id AS ProductId, '' AS AppNo
        //                                        , '' AS IsCOC,lead.slm_status as slmStatusCode, '' AS ContractNoRefer, '' AS COCCurrentTeam
        //                                        , '' AS ExternalSubStatusDesc, lead.slm_NextSLA AS NextSLA, DATEDIFF(minute, GETDATE(), lead.slm_NextSLA) AS NextSLADiffCurrent , '" + status + @"'  as recordType
        //	                            FROM " + SLMDBName + @".dbo.kkslm_tr_prelead lead
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_cusinfo info ON lead.slm_ticketId = info.slm_TicketId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign campaign ON lead.slm_CampaignId = campaign.slm_CampaignId
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff ON lead.slm_Owner = staff.slm_EmpCode and staff.is_Deleted = 0  
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position poOwner on lead.slm_Owner_Position = poOwner.slm_Position_id 
        //	                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus opt ON opt.slm_SubStatusCode = lead.slm_SubStatus and opt.slm_Product_Id = lead.slm_Product_Id
        //	                            LEFT JOIN " + SLMDBName + @".DBO.kkslm_ms_staff createby on createby.slm_UserName = lead.slm_CreatedBy
        //                                LEFT JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT MP ON lead.slm_Product_Id = MP.sub_product_id
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_productinfo prodinfo ON lead.slm_ticketId = prodinfo.slm_TicketId
        //                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_cardtype cardtype ON info.slm_CardType = cardtype.slm_CardTypeId
        //	                            WHERE lead.is_Deleted = 'false' and lead.slm_TicketId is null {0}   
        //                                 ) AS result 
        //                                ) B ";
        //            if (orderByFlag == SLMConstant.SearchOrderBy.SLA)
        //            {
        //                sql += @" ORDER BY A.Counting DESC
        //				                        , CASE WHEN A.NextSLA IS NULL THEN 0 ELSE 1 END DESC
        //				                        , A.NextSLADiffCurrent ASC, A.NextSLA ASC, A.CreatedDate DESC ";
        //            }
        //            else if (orderByFlag == SLMConstant.SearchOrderBy.Note)
        //            {
        //                sql += @" ORDER BY CASE WHEN A.NoteFlag IS NULL THEN '0' 
        //					                    WHEN A.NoteFlag = '0' THEN '0'
        //					                    WHEN A.NoteFlag = '1' THEN '1' END DESC, A.CreatedDate DESC";
        //            }

        //            if (isFollowup)
        //            {
        //                whr += (whr == "" ? "" : " AND ") + " lead.slm_NextContactDate is not null";
        ////                whr += (whr == "" ? "" : " AND ") + @" lead.slm_Prelead_Id in(select A.slm_Prelead_Id from (
        ////                    select   prelead.slm_Prelead_Id,case 
        ////                    when camopt.slm_CampaignId is not null then camopt.slm_cp_substatus_id 
        ////                    when prodopt.slm_cp_substatus_id is not null then prodopt.slm_cp_substatus_id else null 
        ////                    end slm_cp_substatus_id
        ////                                    from kkslm_tr_prelead prelead 
        ////                                        left join kkslm_ms_config_product_substatus camopt on camopt.slm_CampaignId = prelead.slm_CampaignId and camopt.slm_SubStatusCode = prelead.slm_SubStatus
        ////                                        left join kkslm_ms_config_product_substatus prodopt on prodopt.slm_Product_Id = prelead.slm_Product_Id and prodopt.slm_SubStatusCode = prelead.slm_SubStatus
        ////                    ) A where slm_cp_substatus_id is not null ) ";
        //                if (isTabFollowup)
        //                    whr += (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_NextContactDate) <= CONVERT(DATE, GETDATE()) ";
        //            }

        //            whr += (String.IsNullOrEmpty(ticketId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_ticketId LIKE '%" + ticketId + "%' ");
        //            whr += (String.IsNullOrEmpty(firstname) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Name LIKE @firstname ");
        //            whr += (String.IsNullOrEmpty(lastname) ? "" : (whr == "" ? "" : " AND ") + " info.slm_LastName LIKE @lastname ");
        //            whr += (String.IsNullOrEmpty(citizenId) ? "" : (whr == "" ? "" : " AND ") + " info.slm_CitizenId LIKE '%" + citizenId + "%' ");
        //            whr += (String.IsNullOrEmpty(cardTypeId) ? "" : (whr == "" ? "" : " AND ") + " info.slm_CardType = '" + cardTypeId + "' ");
        //            whr += (String.IsNullOrEmpty(campaignId) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CampaignId = '" + campaignId + "' ");
        //            whr += (String.IsNullOrEmpty(createDate) ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_CreatedDate) = '" + createDate + "' ");
        //            whr += (String.IsNullOrEmpty(assignDate) ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, lead.slm_AssignDate) = '" + assignDate + "' ");
        //            whr += (String.IsNullOrEmpty(createBy) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_CreatedBy = '" + createBy + "' ");                    //CreateBy Lead
        //            whr += (String.IsNullOrEmpty(subStatusList) ? "" : (whr == "" ? "" : " AND ") + " opt.slm_SubStatusCode IN (" + subStatusList + ") ");
        //            whr += (nextAppointment == DateTime.MinValue.ToString() ? "" : (whr == "" ? "" : " AND ") + " lead.slm_NextContactDate like '" + nextAppointment + "' ");
        //            whr += (String.IsNullOrEmpty(ownerLeadEmployeeCode) ? "" : (whr == "" ? "" : " AND ") + " lead.slm_Owner = '" + ownerLeadEmployeeCode + "' ");      //Owner Lead
        //            whr += (String.IsNullOrEmpty(year) ? "" : (whr == "" ? "" : " AND ") + " Year(lead.slm_Expire_Date) = '" + year + "' ");
        //            whr += (String.IsNullOrEmpty(month) ? "" : (whr == "" ? "" : " AND ") + " Month(lead.slm_Expire_Date) = '" + month + "' ");

        //            if (!String.IsNullOrEmpty(carLicenseNo))
        //            {
        //                whr += whr == "" ? "" : " AND ";
        //                whr += staffType == 8 ? " lead.slm_Car_License_No = '" + carLicenseNo + "' " : " lead.slm_Car_License_No like '%" + carLicenseNo + "%'";
        //            }
        //            if (!String.IsNullOrEmpty(contractNo))
        //            {
        //                whr += whr == "" ? "" : " AND ";
        //                whr += staffType == 8 ? " lead.slm_Contract_Number = '" + contractNo + "' " : " lead.slm_Contract_Number like '%" + contractNo + "%'";
        //            }

        //            whr = whr == "" ? "" : " AND " + whr;
        //            sql = string.Format(sql, whr);

        //            return sql;
        //        }

        //private string GetRecursiveStaff(string username)
        //{
        //    string userList = "";
        //    KKSlmMsStaffModel sModel = new KKSlmMsStaffModel();

        //    List<StaffData> staffList = sModel.GetStaffList();

        //    StaffData sData = sModel.GetStaff(username);
        //    int? staffId = sData.StaffId;

        //    ArrayList arr = new ArrayList();
        //    arr.Add("'" + username + "'");    //เก็บ login staff

        //    FindStaffRecusive(staffId, arr, staffList);

        //    foreach (string tmp_username in arr)
        //    {
        //        userList += (userList == "" ? "" : ",") + tmp_username;
        //    }

        //    return userList;
        //}

        //private void FindStaffRecusive(int? headId, ArrayList arr, List<StaffData> staffList)
        //{
        //    foreach (StaffData staff in staffList)
        //    {
        //        if (staff.HeadStaffId == headId)
        //        {
        //            arr.Add("'" + staff.UserName + "'");
        //            FindStaffRecusive(staff.StaffId, arr, staffList);
        //        }
        //    }
        //}

        public List<UserMonitoringReInsuranceData> SearchUserMonitoringReInsurance(string userList, string productId, string campaign, string branchcode, string active, string assignDateFrom, string asssignDateTo
           , string teamtelesales, string subStatusList)
        {
            string whr = teamtelesales == "" ? "" : " AND tele.slm_TeamTelesales_Id = '" + teamtelesales + "'";
            whr += String.IsNullOrEmpty(subStatusList) ? "" : "  AND Z.slm_SubStatus in (" + subStatusList + ")";
            string whUserList = " WHERE staff.slm_UserName IN (" + userList + ")";
            string whProductId = "";
            string whCampaign = "";
            string whBranchCode = "";
            string whActive = "";
            string whAssignDate = "";

            if (productId != "0")
                whProductId = " AND PRELEAD.slm_Product_Id = '" + productId + "'";

            if (campaign != "")
                whCampaign = " AND PRELEAD.slm_CampaignId = '" + campaign + "'";

            if (branchcode != "")
                whBranchCode = " AND staff.slm_BranchCode = '" + branchcode + "'";

            if (active != "")
                whActive = " AND staff.slm_IsActive IN (" + active + ")";

            if (assignDateFrom != "" && asssignDateTo != "")
                whAssignDate = " AND CONVERT(DATE,PRELEAD.slm_AssignDate) BETWEEN '" + assignDateFrom + "' AND '" + asssignDateTo + "'";


            string sql = @"   SELECT ST.slm_StaffTypeDesc AS RoleName ,staff.slm_UserName AS Username, staff.slm_StaffNameTH as FullnameTH ,
                                    staff.slm_IsActive AS Active, staff.slm_StaffTypeId,
                                    ISNULL(SUM(Z.STATUS_01_Owner),0) AS SUM_STATUS_01,  
	                                ISNULL(SUM(Z.STATUS_02_Owner),0) AS SUM_STATUS_02,
	                                ISNULL(SUM(Z.STATUS_03_Owner),0) AS SUM_STATUS_03,
	                                ISNULL(SUM(Z.STATUS_04_Owner),0) AS SUM_STATUS_04 ,
	                                ISNULL(SUM(Z.STATUS_05_Owner),0) AS SUM_STATUS_05 ,
	                                ISNULL(SUM(Z.STATUS_10_Owner),0) AS SUM_STATUS_10 ,
	                                ISNULL(SUM(Z.STATUS_11_Owner),0) AS SUM_STATUS_11 ,
	                                ISNULL(SUM(Z.STATUS_12_Owner),0) AS SUM_STATUS_12 ,
	                                ISNULL(SUM(Z.STATUS_13_Owner),0) AS SUM_STATUS_13 ,
	                                ISNULL(SUM(Z.STATUS_14_Owner),0) AS SUM_STATUS_14 ,
	                                ISNULL(SUM(Z.STATUS_01_Owner),0) + ISNULL(SUM(Z.STATUS_02_Owner),0) +
	                                ISNULL(SUM(Z.STATUS_03_Owner),0) + ISNULL(SUM(Z.STATUS_04_Owner),0) +
                                    ISNULL(SUM(Z.STATUS_05_Owner),0) + ISNULL(SUM(Z.STATUS_10_Owner),0) +
	                                ISNULL(SUM(Z.STATUS_11_Owner),0) + ISNULL(SUM(Z.STATUS_12_Owner),0) +
	                                ISNULL(SUM(Z.STATUS_13_Owner),0) + ISNULL(SUM(Z.STATUS_14_Owner),0) AS SUM_TOTAL
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff  INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = STAFF.slm_StaffTypeId
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_teamtelesales tele on tele.slm_TeamTelesales_Id = staff.slm_TeamTelesales_Id
								LEFT JOIN (
		                            --*********************************** เสนอขาย ***********************************
		                            -- คำนวนงานที่เสนอขาย และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            COUNT(slm_Prelead_Id) AS STATUS_01_Owner,  
				                            0 AS STATUS_02_Owner, 
				                            0 AS STATUS_03_Owner, 
				                            0 AS STATUS_04_Owner, 
				                            0 AS STATUS_05_Owner,
				                            0 AS STATUS_10_Owner,
				                            0 AS STATUS_11_Owner,
				                            0 AS STATUS_12_Owner,
				                            0 AS STATUS_13_Owner,
				                            0 AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='01' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus
		                            --*********************************** ขอปรึกษาที่บ้านก่อน  ***********************************
		                            UNION ALL
		                            -- คำนวนงานที่ขอปรึกษาที่บ้านก่อน และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            0 AS STATUS_01_Owner,
				                            COUNT(slm_Prelead_Id) AS STATUS_02_Owner,
				                            0 AS STATUS_03_Owner,
				                            0 AS STATUS_04_Owner,
				                            0 AS STATUS_05_Owner,
				                            0 AS STATUS_10_Owner,
				                            0 AS STATUS_11_Owner,
				                            0 AS STATUS_12_Owner,
				                            0 AS STATUS_13_Owner,
				                            0 AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='02' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus
		                            --*********************************** ขอเลือกบริษัทประกันก่อน  ***********************************
		                            UNION ALL
		                            -- คำนวนงานที่ขอเลือกบริษัทประกันก่อน และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            0 AS STATUS_01_Owner,
				                            0 AS STATUS_02_Owner,
				                            COUNT(slm_Prelead_Id) AS STATUS_03_Owner,
				                            0 AS STATUS_04_Owner,
				                            0 AS STATUS_05_Owner,
				                            0 AS STATUS_10_Owner,
				                            0 AS STATUS_11_Owner,
				                            0 AS STATUS_12_Owner,
				                            0 AS STATUS_13_Owner,
				                            0 AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='03' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus
                                      --*********************************** ฝากให้โทรกลับ ***********************************
		                            UNION ALL
                                    -- คำนวนงานที่ฝากให้โทรกลับ และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            0 AS STATUS_01_Owner,
				                            0 AS STATUS_02_Owner,
				                            0 AS STATUS_03_Owner,
				                            COUNT(slm_Prelead_Id) AS STATUS_04_Owner,
				                            0 AS STATUS_05_Owner,
				                            0 AS STATUS_10_Owner,
				                            0 AS STATUS_11_Owner,
				                            0 AS STATUS_12_Owner,
				                            0 AS STATUS_13_Owner,
				                            0 AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='04' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus
		                            --*********************************** อยู่ระหว่างตัดสินใจ ***********************************
		                            UNION ALL
                                    -- คำนวนงานที่อยู่ระหว่างตัดสินใจ และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            0 AS STATUS_01_Owner,
				                            0 AS STATUS_02_Owner,
				                            0 AS STATUS_03_Owner,
				                            0 AS STATUS_04_Owner,
				                            COUNT(slm_Prelead_Id) AS STATUS_05_Owner,
				                            0 AS STATUS_10_Owner,
				                            0 AS STATUS_11_Owner,
				                            0 AS STATUS_12_Owner,
				                            0 AS STATUS_13_Owner,
				                            0 AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='05' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus
		                            --*********************************** ปิดเครื่อง/ไม่มีสัญญาณ ***********************************
		                            UNION ALL
                                    -- คำนวนงานที่ปิดเครื่อง/ไม่มีสัญญาณ และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            0 AS STATUS_01_Owner,
				                            0 AS STATUS_02_Owner,
				                            0 AS STATUS_03_Owner,
				                            0 AS STATUS_04_Owner,
				                            0 AS STATUS_05_Owner,
				                            COUNT(slm_Prelead_Id) AS STATUS_10_Owner,
				                            0 AS STATUS_11_Owner,
				                            0 AS STATUS_12_Owner,
				                            0 AS STATUS_13_Owner,
				                            0 AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='10' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus
		                            --*********************************** สายเสีย ***********************************
		                            UNION ALL
                                    -- คำนวนงานที่สายเสีย และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            0 AS STATUS_01_Owner,
				                            0 AS STATUS_02_Owner,
				                            0 AS STATUS_03_Owner,
				                            0 AS STATUS_04_Owner,
				                            0 AS STATUS_05_Owner,
				                            0 AS STATUS_10_Owner,
				                            COUNT(slm_Prelead_Id) AS STATUS_11_Owner,
				                            0 AS STATUS_12_Owner,
				                            0 AS STATUS_13_Owner,
				                            0 AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='11' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus
		                            --*********************************** สายไม่ว่าง ***********************************
		                            UNION ALL
                                    -- คำนวนงานที่สายไม่ว่าง และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            0 AS STATUS_01_Owner,
				                            0 AS STATUS_02_Owner,
				                            0 AS STATUS_03_Owner,
				                            0 AS STATUS_04_Owner,
				                            0 AS STATUS_05_Owner,
				                            0 AS STATUS_10_Owner,
				                            0 AS STATUS_11_Owner,
				                            COUNT(slm_Prelead_Id) AS STATUS_12_Owner,
				                            0 AS STATUS_13_Owner,
				                            0 AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='12' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus
		                            --*********************************** เบอร์ผิด/เปลี่ยนเบอร์แล้ว ***********************************
		                            UNION ALL
                                    -- คำนวนงานที่เบอร์ผิด/เปลี่ยนเบอร์แล้ว และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            0 AS STATUS_01_Owner,
				                            0 AS STATUS_02_Owner,
				                            0 AS STATUS_03_Owner,
				                            0 AS STATUS_04_Owner,
				                            0 AS STATUS_05_Owner,
				                            0 AS STATUS_10_Owner,
				                            0 AS STATUS_11_Owner,
				                            0 AS STATUS_12_Owner,
				                            COUNT(slm_Prelead_Id) AS STATUS_13_Owner,
				                            0 AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='13' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus

		                            --*********************************** ไม่มีผู้รับสาย ***********************************
		                            UNION ALL
                                    -- คำนวนงานที่ไม่มีผู้รับสาย และเฉพาะที่ตัวเองเป็น Owner
		                            SELECT slm_StaffTypeDesc AS ROLE_NAME,slm_UserName,slm_StaffTypeId as StaffType,  
				                            slm_StaffNameTH as OwnerName,slm_IsActive,
				                            0 AS STATUS_01_Owner,
				                            0 AS STATUS_02_Owner,
				                            0 AS STATUS_03_Owner,
				                            0 AS STATUS_04_Owner,
				                            0 AS STATUS_05_Owner,
				                            0 AS STATUS_10_Owner,
				                            0 AS STATUS_11_Owner,
				                            0 AS STATUS_12_Owner,
				                            0 AS STATUS_13_Owner,
				                            COUNT(slm_Prelead_Id) AS STATUS_14_Owner,slm_SubStatus
                                    FROM ( SELECT DISTINCT PRELEAD.slm_Prelead_Id,ST.slm_StaffTypeDesc,staff.slm_UserName, staff.slm_StaffNameTH ,staff.slm_IsActive,staff.slm_StaffTypeId,PRELEAD.slm_SubStatus 
		                                FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_EmpCode = PRELEAD.slm_Owner
			                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
		                                WHERE PRELEAD.slm_SubStatus ='14' AND PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' and prelead.slm_TicketId is null 
                                                  {2} {3} {4} {5} {6} 
                                    ) A
		                            GROUP BY slm_StaffTypeDesc,slm_UserName, slm_StaffNameTH ,slm_IsActive,slm_StaffTypeId,slm_SubStatus
	                            ) AS Z on z.slm_UserName = staff.slm_UserName
	                         {0} {1} {4} {5} and staff.is_Deleted = 0  
                            GROUP BY ST.slm_StaffTypeDesc ,staff.slm_UserName , staff.slm_StaffNameTH ,
                                    staff.slm_IsActive , staff.slm_StaffTypeId
                            ORDER BY ISNULL(SUM(Z.STATUS_01_Owner),0) +
	                                ISNULL(SUM(Z.STATUS_02_Owner),0) +
                                    ISNULL(SUM(Z.STATUS_03_Owner),0) +
	                                ISNULL(SUM(Z.STATUS_04_Owner),0) +
	                                ISNULL(SUM(Z.STATUS_05_Owner),0) +
	                                ISNULL(SUM(Z.STATUS_10_Owner),0) +
	                                ISNULL(SUM(Z.STATUS_11_Owner),0) +
                                    ISNULL(SUM(Z.STATUS_12_Owner),0) + 
                                    ISNULL(SUM(Z.STATUS_13_Owner),0) + 
                                    ISNULL(SUM(Z.STATUS_14_Owner),0) DESC,
	                                st.slm_StaffTypeDesc ASC,staff.slm_StaffNameTH ASC ";

            sql = string.Format(sql, whUserList, whr, whProductId, whCampaign, whBranchCode, whActive, whAssignDate);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<UserMonitoringReInsuranceData>(sql).ToList();
            }
        }

        private static string BRN001_09_selectOuter()
        {
            return ", Z.slm_IsExportExpired, Z.slm_IsExportExpiredDate "
                   + ", Z.slm_Chassis_No, Z.slm_Engine_No, Z.slm_Car_License_No, Z.slm_ProvinceRegis_Org "
                   + ", Z.slm_BranchCode "
                   + ", Z.slm_FlagNotifyPremium "
                   + ", Z.slm_PolicyGrossPremium "
                   + ", Z.slm_LatestInsExpireDate ";
        }

        private static string BRN001_09_selectInner()
        {
            return ", PRELEAD.slm_IsExportExpired, PRELEAD.slm_IsExportExpiredDate "
                   + ", PRELEAD.slm_Chassis_No, PRELEAD.slm_Engine_No, PRELEAD.slm_Car_License_No, PRELEAD.slm_ProvinceRegis_Org "
                   + ", PRELEAD.slm_BranchCode "
                   + ", slm_FlagNotifyPremium = isnull(NP.slm_FlagNotifyPremium, 'N')"
                   + ", slm_PolicyGrossPremium = (case when NP.slm_FlagNotifyPremium is null then null else NP.slm_NetGrossPremium end) "
                   + ", slm_LatestInsExpireDate = (case when NP.slm_FlagNotifyPremium is null then null else NP.slm_LatestInsExpireDate end) ";
        }

        private static string BRN001_09_fromJoin(string mainTable)
        {
            string clause = string.Format(
                " left outer hash join (SELECT row_number() OVER ( PARTITION BY REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') ORDER BY slm_Id DESC ) rn, slm_VIN, slm_InsExpireDate AS slm_LatestInsExpireDate, slm_CreatedDate AS slm_LatestCreatedDate, slm_NetPremium AS slm_PolicyGrossPremium, slm_GrossPremium AS slm_NetGrossPremium, 'Y' AS slm_FlagNotifyPremium, slm_PeriodMonth, slm_PeriodYear FROM dbo.kkslm_tr_notify_premium WITH (NOLOCK) ) NP " +
                " on REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE({0}.slm_Chassis_No,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(NP.slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') and rn = 1 " +
                " AND {0}.slm_PeriodMonth = NP.slm_PeriodMonth " +
                " AND {0}.slm_PeriodYear = NP.slm_PeriodYear ", mainTable);
            if (mainTable == "renew")
            {
                clause = clause.Replace("slm_Chassis_No", "slm_ChassisNo");
            }
            return clause;
        }

        /// <summary> join notify premium for search criteria </summary>
        /// <remarks>SR6005-224</remarks>
        private static string JoinNotifyPremium(SearchObtCondition cond, string mainTable)
        {
            if (
                string.IsNullOrEmpty(cond.HasNotifyPremium) &&
                string.IsNullOrEmpty(cond.NotifyGrossPremiumMin) &&
                string.IsNullOrEmpty(cond.NotifyGrossPremiumMax)
            )
            {
                return "";
            }

            return BRN001_09_fromJoin(mainTable);
        }

        public List<SearchUserMonitoringOBTResult> GetUserMonitoringReInsuranceListByUser(string userList, string productId, string campaign, string branchcode, string assignDateFrom, string asssignDateTo
            , string teamtelesales, string subStatusCode)
        {
            string whr = " staff.slm_UserName IN (" + userList + ") AND prelead.slm_TicketId is null ";
            whr += teamtelesales == "" ? "" : " AND tele.slm_TeamTelesales_Id = '" + teamtelesales + "'";

            whr += productId == "0" ? "" : " AND PRELEAD.slm_Product_Id = '" + productId + "'";

            whr += campaign == "" ? "" : " AND PRELEAD.slm_CampaignId = '" + campaign + "'";

            whr += branchcode == "" ? "" : " AND staff.slm_BranchCode = '" + branchcode + "'";

            whr += (assignDateFrom == "" && asssignDateTo == "") ? "" : " AND CONVERT(DATE,PRELEAD.slm_AssignDate) BETWEEN '" + assignDateFrom + "' AND '" + asssignDateTo + "'";

            whr = whr == "" ? "" : " AND " + whr;

            string sql = "";
            string whSubStatusCode = "";

            if (subStatusCode == "ALL")
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus IN  ( '" + SLMConstant.SubStatusCode.OfferSale + "', '" + SLMConstant.SubStatusCode.ConsultHome + "','" + SLMConstant.SubStatusCode.Choose + "','" + SLMConstant.SubStatusCode.ContactCall + "','" + SLMConstant.SubStatusCode.OnProcess + "','" + SLMConstant.SubStatusCode.NoSignal + "','" + SLMConstant.SubStatusCode.WasteLine + "','" + SLMConstant.SubStatusCode.BusyLine + "','" + SLMConstant.SubStatusCode.WrongNumber + "','" + SLMConstant.SubStatusCode.NoRecipient + "')";
            else if (subStatusCode == SLMConstant.SubStatusCode.OfferSale)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.OfferSale + "'";
            else if (subStatusCode == SLMConstant.SubStatusCode.ConsultHome)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.ConsultHome + "'";
            else if (subStatusCode == SLMConstant.SubStatusCode.Choose)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.Choose + "'";
            else if (subStatusCode == SLMConstant.SubStatusCode.ContactCall)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.ContactCall + "'";
            else if (subStatusCode == SLMConstant.SubStatusCode.OnProcess)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.OnProcess + "'";
            else if (subStatusCode == SLMConstant.SubStatusCode.NoSignal)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.NoSignal + "'";
            else if (subStatusCode == SLMConstant.SubStatusCode.WasteLine)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.WasteLine + "'";
            else if (subStatusCode == SLMConstant.SubStatusCode.BusyLine)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.BusyLine + "'";
            else if (subStatusCode == SLMConstant.SubStatusCode.WrongNumber)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.WrongNumber + "'";
            else if (subStatusCode == SLMConstant.SubStatusCode.NoRecipient)
                whSubStatusCode = @" AND PRELEAD.slm_SubStatus = '" + SLMConstant.SubStatusCode.NoRecipient + "'";
            else
                whSubStatusCode = subStatusCode == "" ? "" : @" AND PRELEAD.slm_SubStatus IN (" + subStatusCode + ")";

            sql = @"   
                        SELECT CONVERT(int, ROW_NUMBER() OVER(ORDER BY Z.slm_CreatedDate desc)) AS SEQ
                                ,Z.PreleadId
                                ,Z.CitizenId
                                ,Z.Firstname
                                ,Z.Lastname
                                ,Z.SubStatusDesc
                                ,Z.OwnerName
                                ,Z.CreatedDate
                                ,Z.AssignedDate
                                ,Z.CampaignName  
                                ,Z.TranferType 
                                ,Z.CampaignId
                                ,Z.CardTypeDesc
                                ,Z.Counting
                                ,Z.HasAdamUrl
                                ,Z.ContractNo 
                                ,Z.TicketId
                                ,Z.StatusDesc
                                ,Z.TEAMCODE
                                ,Z.EMPCODE
                                ,Z.CusFullName
                                ,Z.INSNAME
                                ,Z.COV_NAME
                                ,Z.GROSS_PREMIUM
                                ,Z.GRADE
                                ,Z.Lot
                                ,Z.EXP_MONTH
                                ,Z.EXPIRE_DATE
                                ,Z.NEXT_CONTRACT_DATE
								,Z.slm_AssignDescription
								,Z.slm_Car_By_Gov_Name_Org
								,Z.slm_Brand_Name_Org 
								,Z.slm_Model_name_Org 								
								,Z.slm_Voluntary_Policy_Number
                                " + BRN001_09_selectOuter() + @"
                        FROM(
                        SELECT DISTINCT PRELEAD.slm_CreatedDate
                                ,PRELEAD.slm_Prelead_Id         AS PreleadId
                                ,CUS.slm_CitizenId              AS CitizenId
                                ,PRELEAD.slm_Name               AS Firstname
                                ,PRELEAD.slm_LastName           AS Lastname
                                ,OP.slm_SubStatusName           AS SubStatusDesc
                                ,STAFF.slm_StaffNameTH          AS OwnerName
                                ,PRELEAD.slm_CreatedDate        AS  CreatedDate
                                ,PRELEAD.slm_AssignDate         AS AssignedDate
                                ,cam.slm_CampaignName           AS CampaignName  
                                ,'Owner Prelead'                AS TranferType 
                                ,PRELEAD.slm_CampaignId         AS CampaignId
                                ,ct.slm_CardTypeName            AS CardTypeDesc
                                , PRELEAD.slm_Counting          AS Counting
                                , ISNULL(MP.HasADAMUrl, 0)      AS HasAdamUrl
                                ,PRELEAD.slm_Contract_Number    AS ContractNo
                                , PRELEAD.slm_TicketId          AS TicketId
                                ,OPT.slm_OptionDesc             AS StatusDesc
                                ,TT.slm_TeamTelesales_Code      AS TEAMCODE
                                ,STAFF.slm_EmpCode              AS EMPCODE
                                ,ISNULL(tl.slm_TitleName,'') +  ISNULL(prelead.slm_Name,'') +' '+ ISNULL(prelead.slm_LastName,'') as CusFullName 
                                ,PRELEAD.slm_Voluntary_Company_Name AS INSNAME
                                ,COV.slm_ConverageTypeName          AS COV_NAME
                                ,PRELEAD.slm_Voluntary_Gross_Premium AS GROSS_PREMIUM
                                ,PRELEAD.slm_Grade              AS GRADE
                                ,prelead.slm_CmtLot             AS Lot
                                ,PRELEAD.slm_Voluntary_Policy_Exp_Month AS EXP_MONTH
                                ,PRELEAD.slm_Voluntary_Policy_Exp_Date  AS EXPIRE_DATE
                                ,PRELEAD.slm_NextContactDate            AS NEXT_CONTRACT_DATE
								,PRELEAD.slm_AssignDescription
								,PRELEAD.slm_Car_By_Gov_Name_Org
								,PRELEAD.slm_Model_name_Org 
								,PRELEAD.slm_Brand_Name_Org 
								,PRELEAD.slm_Voluntary_Policy_Number
                                " + BRN001_09_selectInner() + @"
                        FROM " + SLMDBName + @".dbo.kkslm_tr_prelead PRELEAD 
                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_config_product_substatus OP ON OP.slm_SubStatusCode = PRELEAD.slm_SubStatus 
                                                                                                        AND op.slm_OptionCode = PRELEAD.slm_Status 
                                                                                                        AND (op.slm_CampaignId = PRELEAD.slm_CampaignId  OR  op.slm_Product_Id = PRELEAD.slm_Product_Id )
                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign         cam     ON CAM.slm_CampaignId       = PRELEAD.slm_CampaignId 
	                        INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff            staff   ON staff.slm_EmpCode        = PRELEAD.slm_Owner     AND staff.is_Deleted = 0  
                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type       ST      ON ST.slm_StaffTypeId       = staff.slm_StaffTypeId
                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_option           OPT     ON OPT.slm_OptionCode       = PRELEAD.slm_Status    AND OPT.slm_OptionType = 'lead status'
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_teamtelesales    TT      ON TT.slm_TeamTelesales_Id  = STAFF.slm_TeamTelesales_Id
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_cusinfo          CUS     ON CUS.slm_TicketId         = PRELEAD.slm_ticketId 
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_cardtype         ct      ON cus.slm_CardType         = ct.slm_CardTypeId
                            LEFT JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT       MP      ON PRELEAD.slm_Product_Id   = MP.sub_product_id
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_teamtelesales    tele    ON tele.slm_TeamTelesales_Id = staff.slm_TeamTelesales_Id
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_title            tl      ON tl.slm_TitleId           = prelead.slm_TitleId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_coveragetype     COV     ON COV.slm_CoverageTypeId   = PRELEAD.SLM_Voluntary_Type_Key
                            " + BRN001_09_fromJoin("PRELEAD") + @"
                        WHERE PRELEAD.is_Deleted = 'false' AND prelead.slm_AssignFlag = '1' AND prelead.slm_SubStatus not in ('06','07','08','09')  
                                {0} {1}
                        ) AS Z  
 ";

            sql = string.Format(sql, whSubStatusCode, whr);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SearchUserMonitoringOBTResult>(sql).ToList();
            }
        }


        public SearchInsureSummaryResult SearchInsureSummary(string preleadId)
        {
            string sql = "";
            string whr = "";

            sql += @" select prelead.slm_Grade Grade,prelead.slm_Car_Category CarCategory,prelead.slm_Voluntary_Channel_Key VoluntaryChannelKey
                        ,prelead.slm_Voluntary_Policy_Number VoluntaryPolicyNumber,prelead.slm_Voluntary_Cov_Amt VoluntaryCovAmt,vol.slm_InsNameTh VoluntaryCompanyName
                        ,cov.slm_ConverageTypeName VoluntaryType,prelead.slm_Voluntary_Gross_Premium VoluntaryGrossPremium
                        ,prelead.slm_Voluntary_First_Create_Date VoluntaryFirstCreateDate,prelead.slm_Voluntary_Policy_Eff_Date VoluntaryPolicyEffDate
                        ,prelead.slm_Voluntary_Policy_Exp_Date VoluntaryPolicyExpDate,comp.slm_InsNameTh CompulsoryCompanyName
                        ,prelead.slm_Compulsory_Gross_Premium CompulsoryGrossPremium,prelead.slm_Compulsory_Policy_Eff_Date CompulsoryPolicyEffDate
                        ,prelead.slm_Compulsory_Policy_Exp_Date CompulsoryPolicyExpDate,slm_Compulsory_Policy_Number as actno
                        ,ISNULL(prelead.slm_IsExportExpired, 0) IsExportExpired, prelead.slm_IsExportExpiredDate IsExportExpiredDate
                      From " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead prelead
                      left join " + SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com vol on vol.slm_InsCode = prelead.slm_Voluntary_Company_Code
                      left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_coveragetype cov on cov.slm_CoverageTypeId = prelead.slm_Voluntary_Type_Key
                      left join " + SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com comp on comp.slm_InsCode = prelead.slm_Compulsory_Company_Code ";

            whr += (String.IsNullOrEmpty(preleadId) ? "" : (whr == "" ? "" : " AND ") + " prelead.slm_Prelead_Id = '" + preleadId + "' ");

            whr = whr == "" ? "" : " WHERE " + whr;
            sql += whr;

            if (sql == "")
                return null;
            else
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<SearchInsureSummaryResult>(sql).FirstOrDefault();
                }
            }
        }

    }
}
