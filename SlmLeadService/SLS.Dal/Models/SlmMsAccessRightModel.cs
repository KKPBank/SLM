using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using SLS.Dal.Data;
using System.Collections;
using System.Data.SqlClient;

namespace SLS.Dal.Models
{
    public class SlmMsAccessRightModel : IDisposable
    {
        #region Member
        private SLM_DBEntities slmdb = null;
        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                handle.Dispose();
            }
            disposed = true;
        }
        #endregion

        public SlmMsAccessRightModel()
        {
            slmdb = new SLM_DBEntities();
        }

        public SlmMsAccessRightModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public string GetDBName()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["SLMDBName"];
            }
            catch
            {
                return "SLMDB";
            }
        }

        public bool CheckAccessRight(string campaignId, string username)
        {
            string SLMDBName = GetDBName();
            string sql = @"SELECT staff.slm_UserName
                            FROM  " + SLMDBName + @".dbo.kkslm_ms_branch branch 
                            inner join " + SLMDBName + @".dbo.kkslm_ms_staff staff on staff.slm_BranchCode = branch.slm_BranchCode 
	                        INNER JOIN (
	                                    SELECT DISTINCT Z.slm_BranchCode,Z.slm_StaffTypeId  
                                        FROM (
			                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
			                                    FROM " + SLMDBName + @".dbo.kkslm_ms_access_right AR 
                                                INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign CAM ON CAM.slm_CampaignId = AR.slm_CampaignId 
			                                    WHERE CAM.slm_CampaignId = '" + campaignId + @"' 
			                                    UNION ALL
			                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
			                                    FROM " + SLMDBName + @".dbo.kkslm_ms_access_right AR 
                                                INNER JOIN " + SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
			                                    WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT 
			                                    WHERE PR_CAMPAIGNID = '" + campaignId + @"')
			                                    ) AS Z
		                                ) AS A ON A.slm_BranchCode = STAFF.slm_BranchCode AND A.slm_StaffTypeId = STAFF.slm_StaffTypeId     
	                    WHERE branch.is_Deleted = '0' AND STAFF.slm_UserName = '" + username + @"' AND STAFF.slm_StaffTypeId <> '" + ServiceConstant.StaffType.ITAdministrator + "'";

            var list = slmdb.ExecuteStoreQuery<string>(sql).ToList();
            return list.Count > 0 ? true : false;
        }

        public bool CheckAccessRightByTicketId(string ticketid, string username)
        {
            SlmMsStaffModel staff = new SlmMsStaffModel(new SLM_DBEntities());
            decimal? stafftype = staff.GetStaffType(username);
            string sql = "";

            if (stafftype == ServiceConstant.StaffType.ManagerOper || stafftype == ServiceConstant.StaffType.SupervisorOper
                || stafftype == ServiceConstant.StaffType.Oper || stafftype == ServiceConstant.StaffType.Marketing)
            {
                string recusiveList = "";

                recusiveList = GetRecursiveStaff(username, true);

                sql = @" SELECT lead.slm_ticketId AS TicketId
                        FROM kkslm_tr_lead lead ";
                sql += " WHERE lead.is_Deleted = 0 AND (lead.slm_Owner IN (" + recusiveList + ") OR lead.slm_Delegate IN (" + recusiveList + @")) {0} ";
            }
            else
            {
                //Telesales and the others
                string UserLoginBranchCode = staff.GetBrachCode(username);

                sql = @" SELECT A.* FROM(
                            SELECT DISTINCT result.*
                            FROM (
	                            SELECT lead.slm_ticketId AS TicketId
	                            FROM kkslm_tr_lead lead INNER JOIN 
	                            (
                                    SELECT CP.PR_CampaignId AS slm_CampaignId FROM kkslm_ms_access_right AR INNER  JOIN CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
                                    WHERE AR.slm_Product_Id IS NOT NULL AND
                                    AR.slm_BranchCode = '" + UserLoginBranchCode + @"' AND AR.slm_StaffTypeId = '" + stafftype + @"'
                                    UNION ALL
                                    SELECT AR.slm_CampaignId  AS slm_CampaignId  FROM kkslm_ms_access_right AR 
                                    WHERE AR.slm_CampaignId IS NOT NULL AND
                                    AR.slm_BranchCode = '" + UserLoginBranchCode + @"' AND AR.slm_StaffTypeId = '" + stafftype + @"'
                                ) AS Z ON Z.slm_CampaignId = lead.slm_CampaignId 
	                            WHERE lead.is_Deleted = 0 {0} 
	                            UNION ALL
	                            SELECT lead.slm_ticketId AS TicketId
	                            FROM kkslm_tr_lead lead
	                            WHERE lead.slm_Delegate = '" + username + @"' AND lead.is_Deleted = 0 {0}   
                                 ) AS result 
                                ) A ";
            }

            string whr = "";
            whr += (ticketid == "" ? "" : (whr == "" ? "" : " AND ") + " lead.slm_ticketId = '" + ticketid + "' ");
            whr = whr == "" ? "" : " AND " + whr;
            sql = string.Format(sql, whr);

            object[] param = new object[]
            {
                new SqlParameter("@ticketid", ticketid )
            };

            var list = slmdb.ExecuteStoreQuery<string>(sql, param).ToList();
            return list.Count > 0 ? true : false;
        }

        public bool ValidateTicketId(string ticketId)
        {
            var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId.Equals(ticketId)).FirstOrDefault();
            return (lead != null) ? true : false;
        }

        public string GetRecursiveStaff(string username, bool includedeleted = false)
        {
            string userList = "";
            SlmMsStaffModel sModel = new SlmMsStaffModel(new SLM_DBEntities());

            List<StaffData> staffList = sModel.GetStaffList(includedeleted); ;

            StaffData sData = sModel.GetStaff(username);
            int? staffId = sData.StaffId;

            ArrayList arr = new ArrayList();
            arr.Add("'" + username + "'");    //เก็บ login staff

            FindStaffRecusive(staffId, arr, staffList);

            foreach (string tmp_username in arr)
            {
                userList += (userList == "" ? "" : ",") + tmp_username;
            }

            return userList;
        }

        private void FindStaffRecusive(int? headId, ArrayList arr, List<StaffData> staffList)
        {
            foreach (StaffData staff in staffList)
            {
                if (staff.HeadStaffId == headId)
                {
                    arr.Add("'" + staff.UserName + "'");
                    FindStaffRecusive(staff.StaffId, arr, staffList);
                }
            }
        }

    }
}
