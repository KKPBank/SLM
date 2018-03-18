using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SLM.Resource.Data;
using SLM.Resource;
using System.Collections;

namespace SLM.Dal.Models
{
    public class KKSlmMsStaffModel
    {
        private string SLMDBName = ConfigurationManager.AppSettings["SLMDBName"] != null ? ConfigurationManager.AppSettings["SLMDBName"] : "SLMDB";

        public List<ControlListData> GetStaffData(int staffTypeId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_staff.Where(p => p.slm_StaffTypeId == staffTypeId && p.is_Deleted == 0).OrderBy(p => p.slm_StaffNameTH).Select(p => new ControlListData { TextField = p.slm_StaffNameTH, ValueField = p.slm_UserName }).ToList();
            }
        }

        public List<StaffData> GetStaffList(bool includedelete = false)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                IQueryable<kkslm_ms_staff> entity = slmdb.kkslm_ms_staff.AsQueryable();
                if (!includedelete)
                {
                    entity = entity.Where(p => p.is_Deleted == 0);
                }
                return entity.Select(p => new StaffData
                {
                    UserName = p.slm_UserName,
                    EmpCode = p.slm_EmpCode,
                    StaffId = p.slm_StaffId,
                    HeadStaffId = p.slm_HeadStaffId
                }).ToList();
            }
        }

        public List<ControlListData> GetStaffList(string branchCode, string userName = null)
        {
            string whr = "";
            if (userName != null) whr = " OR ( staff.slm_UserName = '" + userName.Replace("'", "''") + "' ) ";
            string sql = @"SELECT CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
			                            ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS TextField, staff.slm_UserName AS ValueField
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position pos WITH (NOLOCK) ON staff.slm_Position_id = pos.slm_Position_id
                            WHERE ( staff.slm_BranchCode = '" + branchCode + @"' AND staff.is_Deleted = '0'
                            AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator + @"') " + whr + @" 
                            ORDER BY staff.is_Deleted desc, staff.slm_StaffNameTH ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public List<ControlListData> GetStaffNotDummyList(string branchCode, string userName = null)
        {
            string whr = "";
            if (userName != null) whr = " OR ( staff.slm_UserName = '" + userName.Replace("'", "''") + "' ) ";
            string sql = @"SELECT CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
			                            ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS TextField, staff.slm_UserName AS ValueField
                            FROM dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                            LEFT JOIN dbo.kkslm_ms_position pos WITH (NOLOCK) ON staff.slm_Position_id = pos.slm_Position_id
                            WHERE staff.slm_BranchCode = '" + branchCode + @"' AND ( staff.is_Deleted = '0' " + whr + @"  )
                            AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator + @"' 
                            AND (staff.slm_UserType = 'I' OR staff.slm_UserType is null)
                            ORDER BY staff.slm_StaffNameTH ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public string GetStaffNotDummyListByUsername(string username) //Duppicate From Funcrion GetStaffNotDummyList
        {
            string sql = @"
                            SELECT CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
			                            ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS TextField
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id
                            WHERE 1=1
                            AND staff.is_Deleted = '0'
                            AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator + @"'
                            AND staff.slm_UserName  = '" + username + @"'
                            ORDER BY staff.slm_StaffNameTH 

";
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staff = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
                if (staff != null)
                    return staff;
                else
                    return "";
            }
        }

        public List<ControlListData> GetStaffBranchAndRecursiveList(string branchCode, string recursivelist)
        {
            string sql = @"SELECT CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
			                            ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS TextField, staff.slm_UserName AS ValueField
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id
                            WHERE staff.slm_BranchCode = '" + branchCode + @"' AND staff.is_Deleted = '0' 
                            AND staff.slm_UserName IN (" + recursivelist + @") 
                            AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator + @"'
                            ORDER BY staff.slm_StaffNameTH ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }
        public List<ControlListData> GetStaffBranchAndRecursiveList2(string branchCode, string headStaffUserName)
        {
            //string sql = @"SELECT CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
            //                   ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS TextField, staff.slm_UserName AS ValueField
            //                FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
            //                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id
            //                WHERE staff.slm_BranchCode = '" + branchCode + @"' AND staff.is_Deleted = '0' 
            //                AND staff.slm_UserName IN (" + recursivelist + @") 
            //                AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator + @"'
            //                ORDER BY staff.slm_StaffNameTH ";

            string sqlz = @"
with recur (UserName, StaffID, HeadID, BranchCode, zLevel, SuperHead, SuperHeadUserName)
AS
( 
	select m.slm_UserName, m.slm_staffid, m.slm_Headstaffid, m.slm_branchcode, 0 as zLevel, m.slm_StaffId, m.slm_UserName
	from kkslm_ms_staff m where m.is_Deleted = 0
	union all
	select m2.slm_UserName, m2.slm_staffid, m2.slm_headstaffid, m2.slm_branchcode, r.zlevel + 1, r.SuperHead, r.SuperHeadUserName
	from kkslm_ms_staff m2
	inner join recur r on r.StaffID = m2.slm_HeadStaffId and (r.StaffID <> r.HeadID or r.headid is null)
	where m2.is_Deleted = 0
)
                            SELECT CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
			                        ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS TextField, staff.slm_UserName AS ValueField
                            FROM kkslm_ms_staff staff
                            LEFT JOIN kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id
                            WHERE  staff.slm_StaffId in (select StaffId from recur where SuperHeadUserName = '" + headStaffUserName + @"' ) 
                            and staff.slm_BranchCode = '" + branchCode + @"' AND staff.is_Deleted = '0' 
                            AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator + @"'
                            ORDER BY staff.slm_StaffNameTH

";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sqlz).ToList();
            }
        }

        public List<ControlListData> GetHeadStaffList(string branchCode)
        {
            string sql = @"SELECT CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
			                            ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS TextField, CONVERT(VARCHAR, staff.slm_StaffId) AS ValueField
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id
                            WHERE staff.slm_BranchCode = '" + branchCode + @"' AND staff.is_Deleted = '0'
                            AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator + @"'
                            ORDER BY staff.slm_StaffNameTH ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public List<ControlListData> GetStaffAllDataByAccessRight(string campaignId, string branch)
        {
            string sql = @"SELECT CASE WHEN PO.slm_PositionNameAbb IS NULL THEN  staff.slm_StaffNameTH  
                                  ELSE PO.slm_PositionNameAbb + ' - ' + STAFF.slm_StaffNameTH END TextField  ,staff.slm_UserName  AS ValueField 
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position po on staff.slm_Position_id = po.slm_Position_id 
                            INNER JOIN (
                                            SELECT DISTINCT Z.slm_BranchCode,Z.slm_StaffTypeId  
                                            FROM (
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM " + SLMDBName + @".dbo.kkslm_ms_access_right AR INNER JOIN kkslm_ms_campaign CAM ON CAM.slm_CampaignId = AR.slm_CampaignId 
                                                    WHERE CAM.slm_CampaignId = '{1}' 
                                                    UNION ALL
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM " + SLMDBName + @".dbo.kkslm_ms_access_right AR INNER JOIN " + SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
                                                    WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM " + SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT 
													                            WHERE PR_CAMPAIGNID = '{1}')
                                                 ) AS Z
                                          ) AS A ON A.slm_BranchCode = STAFF.slm_BranchCode AND A.slm_StaffTypeId = STAFF.slm_StaffTypeId              
                            where staff.is_Deleted = 0 and staff.slm_BranchCode ='{0}' AND STAFF.slm_StaffTypeId NOT IN ({2})    
                            ORDER BY staff.slm_StaffNameTH";

            sql = string.Format(sql, branch, campaignId, SLMConstant.StaffType.ITAdministrator);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public List<ControlListData> GetStaffAllDataNotDummyByAccessRight(string campaignId, string branch, string userName = null)
        {
            string whr = "";
            if (userName != null) whr = " OR ( staff.slm_UserName = '" + userName.Replace("'", "''") + "' ) ";
            string sql = @"SELECT CASE WHEN PO.slm_PositionNameAbb IS NULL THEN  staff.slm_StaffNameTH  
                                  ELSE PO.slm_PositionNameAbb + ' - ' + STAFF.slm_StaffNameTH END TextField  ,staff.slm_UserName  AS ValueField 
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position po on staff.slm_Position_id = po.slm_Position_id 
                            INNER JOIN (
                                            SELECT DISTINCT Z.slm_BranchCode,Z.slm_StaffTypeId  
                                            FROM (
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM " + SLMDBName + @".dbo.kkslm_ms_access_right AR INNER JOIN kkslm_ms_campaign CAM ON CAM.slm_CampaignId = AR.slm_CampaignId 
                                                    WHERE CAM.slm_CampaignId = '{1}' 
                                                    UNION ALL
                                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
                                                    FROM " + SLMDBName + @".dbo.kkslm_ms_access_right AR INNER JOIN " + SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
                                                    WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM " + SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT 
													                            WHERE PR_CAMPAIGNID = '{1}')
                                                 ) AS Z
                                          ) AS A ON A.slm_BranchCode = STAFF.slm_BranchCode AND A.slm_StaffTypeId = STAFF.slm_StaffTypeId              
                            where (staff.is_Deleted = 0 " + whr + @" ) and staff.slm_BranchCode ='{0}' AND STAFF.slm_StaffTypeId NOT IN ({2}) AND (staff.slm_UserType = 'I' OR staff.slm_UserType is null)
                            ORDER BY staff.slm_StaffNameTH";

            sql = string.Format(sql, branch, campaignId, SLMConstant.StaffType.ITAdministrator);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public bool CheckStaffAccessRightExist(string campaignId, string branch, string username)
        {
            string sql = @"SELECT CASE WHEN PO.slm_PositionNameAbb IS NULL THEN  staff.slm_StaffNameTH  
		                          ELSE PO.slm_PositionNameAbb+' - '+ STAFF.slm_StaffNameTH END TextField  ,staff.slm_UserName  AS ValueField 
                            FROM kkslm_ms_staff staff WITH (NOLOCK) 
                            left join kkslm_ms_position po on staff.slm_Position_id = po.slm_Position_id 
                            where staff.is_Deleted = 0 and staff.slm_BranchCode ='{0}' and
	                            staff.slm_StaffTypeId in 
	                            (	SELECT DISTINCT Z.slm_StaffTypeId 
		                            FROM (
				                            SELECT AR.slm_StaffTypeId
				                            FROM kkslm_ms_access_right AR INNER JOIN kkslm_ms_campaign CAM ON CAM.slm_CampaignId = AR.slm_CampaignId 
				                            WHERE CAM.slm_CampaignId = '{1}'
				                            UNION ALL
				                            SELECT AR.slm_StaffTypeId
				                            FROM kkslm_ms_access_right AR INNER JOIN CMT_CAMPAIGN_PRODUCT CP ON CP.PR_ProductId = AR.slm_Product_Id
				                            WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WHERE PR_CAMPAIGNID = '{1}')
			                              ) AS Z 
                                  ) AND STAFF.slm_StaffTypeId NOT IN ({2}) and staff.slm_UserName = '{3}' 
                            ORDER BY staff.slm_StaffNameTH";

            sql = string.Format(sql, branch, campaignId, SLMConstant.StaffType.ITAdministrator, username);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
                if (list.Count > 0)
                    return true;
                else
                    return false;
            }
        }
        public List<ControlListData> GetStaffHeadData(string branch)
        {
            decimal[] dec = { SLMConstant.StaffType.ITAdministrator };
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_staff.Where(p => p.slm_BranchCode == branch && p.is_Deleted == 0 && dec.Contains(p.slm_StaffTypeId) == false).OrderBy(p => p.slm_StaffNameTH).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_StaffNameTH, ValueField = p.slm_StaffId.ToString() }).ToList();
            }
        }

        public string GetBrachCode(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                //var staffdata = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username && p.is_Deleted == 0).FirstOrDefault();

                string sql = $"SELECT * FROM kkslm_ms_staff WITH (NOLOCK) WHERE slm_UserName = '{username}' AND is_Deleted = 0 ";
                var staffdata = slmdb.ExecuteStoreQuery<kkslm_ms_staff>(sql).FirstOrDefault();
                return staffdata != null ? staffdata.slm_BranchCode : string.Empty;
            }
        }

        public string GetBrachCodeByEmpCode(string empCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staffdata = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == empCode && p.is_Deleted == 0).FirstOrDefault();
                if (staffdata != null)
                    return staffdata.slm_BranchCode;
                else
                    return string.Empty;
            }
        }

        public string GetUsernameByEmpCode(string empCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staffdata = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == empCode && p.is_Deleted == 0).FirstOrDefault();
                if (staffdata != null)
                    return staffdata.slm_UserName;
                else
                    return string.Empty;
            }
        }

        public StaffData GetStaffData(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = $@"SELECT staff.slm_StaffNameTH AS StaffNameTH, br.slm_BranchName AS BranchName, br.slm_BranchCode AS BranchCode
                                , staff.slm_StaffId AS StaffId, staff.slm_Position_id AS PositionId, staff.slm_StaffTypeId AS StaffTypeId 
                                FROM dbo.kkslm_ms_staff staff WITH (NOLOCK)
                                LEFT JOIN dbo.kkslm_ms_branch br WITH (NOLOCK) ON br.slm_BranchCode = staff.slm_BranchCode
                                WHERE staff.slm_UserName = '{username}' AND staff.is_Deleted = 0 ";

                return slmdb.ExecuteStoreQuery<StaffData>(sql).FirstOrDefault();
            }
        }

        public string GetStaffIdData(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staffdata = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username && p.is_Deleted == 0).FirstOrDefault();
                if (staffdata != null)
                    return staffdata.slm_StaffId.ToString();
                else
                    return string.Empty;
            }
        }

        public string GetStaffNameData(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staffdata = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username && p.is_Deleted == 0).FirstOrDefault();
                if (staffdata != null)
                    return staffdata.slm_StaffNameTH;
                else
                    return string.Empty;
            }
        }

        public List<ControlListData> GetOwnerList(string username)
        {
            string sql = @"SELECT DISTINCT staff.slm_StaffNameTH AS TextField, staff.slm_UserName AS ValueField
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_group staffgroup ON staff.slm_StaffId = staffgroup.slm_StaffId
                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_group grp ON grp.slm_GroupId = staffgroup.slm_GroupId
                            WHERE grp.slm_CampaignId IN (
										SELECT DISTINCT B.slm_CampaignId
										FROM
										(
											SELECT SLM_CAMPAIGNID
											FROM " + SLMDBName + @".DBO.kkslm_ms_group [GROUP] 
											WHERE slm_GroupId IN (
													SELECT slm_GroupId 
													FROM " + SLMDBName + @".DBO.kkslm_ms_staff_group 
													WHERE is_Deleted = 0 AND slm_StaffId IN (
														SELECT slm_StaffId
														FROM " + SLMDBName + @".DBO.kkslm_ms_staff staff 
														WHERE staff.slm_UserName = '" + username + @"'))
											UNION ALL
											SELECT SLM_CAMPAIGNID
											FROM " + SLMDBName + @".DBO.kkslm_ms_group [GROUP] 
											WHERE slm_StaffId IN (
														SELECT slm_StaffId
														FROM " + SLMDBName + @".DBO.kkslm_ms_staff staff 
														WHERE staff.slm_UserName =  '" + username + @"')
										) AS B) AND staff.is_Deleted = 0 AND staff.slm_StaffTypeId <> 7 ORDER BY staff.slm_StaffNameTH";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public List<ControlListData> GetOwnerListByCampaignId(string campaignId)
        {
            string sql = @"SELECT DISTINCT staff.slm_StaffNameTH AS TextField, staff.slm_UserName AS ValueField
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_group staffgroup ON staff.slm_StaffId = staffgroup.slm_StaffId
                            WHERE staffgroup.slm_CampaignId IN ('" + campaignId + "') AND staff.is_Deleted = 0  AND staff.slm_StaffTypeId <> 7  ORDER BY staff.slm_StaffNameTH";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        [Obsolete("seems to be unused", true)]
        public List<ControlListData> GetOwnerListByCampaignIdAndBranch(string campaignId, string branchcode)
        {
            string sql = "";
            /*
                        if (campaignId == "")
                        {
                            sql = @"SELECT Z.*
                                    FROM (
                                    SELECT DISTINCT A.*
                                    FROM
                                    (
                                        SELECT slm_PositionName+' - '+staff.slm_StaffNameTH  AS TextField, staff.slm_UserName AS ValueField,staff.slm_StaffNameTH
                                        FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                                        INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_group staffgroup ON staff.slm_StaffId = staffgroup.slm_StaffId
                                        WHERE staffgroup.slm_CampaignId IN ('" + campaignId + @"') AND staff.is_Deleted = 0  AND staff.slm_StaffTypeId <> " + SLMConstant.StaffType.ITAdministrator + @"
                                        UNION ALL
                                        SELECT slm_PositionName+' - '+staff.slm_StaffNameTH  AS TextField, staff.slm_UserName AS ValueField,staff.slm_StaffNameTH
                                        FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                                        INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_group staffgroup ON staff.slm_StaffId = staffgroup.slm_StaffId
                                        WHERE staff.is_Deleted = 0  AND staff.slm_StaffTypeId = " + SLMConstant.StaffType.Marketing + @"      
                                    ) AS A ) AS Z   ORDER BY Z.slm_StaffNameTH ";
                        }
                        else
                        {
                            sql = @"SELECT Z.*
                                    FROM (
                                    SELECT DISTINCT A.*
                                    FROM
                                    (
                                    SELECT slm_PositionName+' - '+staff.slm_StaffNameTH  AS TextField, staff.slm_UserName AS ValueField,staff.slm_StaffNameTH
                                    FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                                    INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_group staffgroup ON staff.slm_StaffId = staffgroup.slm_StaffId
                                    WHERE staffgroup.slm_CampaignId IN ('" + campaignId + @"') AND staff.is_Deleted = 0  AND staff.slm_StaffTypeId <> " + SLMConstant.StaffType.ITAdministrator + @" 
                                            and staff.slm_BranchCode = '" + branchcode + @"' 
                                    UNION ALL
                                    SELECT slm_PositionName+' - '+staff.slm_StaffNameTH  AS TextField, staff.slm_UserName AS ValueField,staff.slm_StaffNameTH
                                    FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                                    INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_group staffgroup ON staff.slm_StaffId = staffgroup.slm_StaffId
                                    WHERE staff.is_Deleted = 0  AND staff.slm_StaffTypeId = " + SLMConstant.StaffType.Marketing + @" and staff.slm_BranchCode = '" + branchcode + @"'      
                                    ) AS A ) AS Z   ORDER BY Z.slm_StaffNameTH 
                                       ";
                        }
            */

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public decimal? GetStaffType(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                //var staffdata = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username && p.is_Deleted == 0).FirstOrDefault();

                string sql = $"SELECT * FROM kkslm_ms_staff WITH (NOLOCK) WHERE slm_UserName = '{username}' AND is_Deleted = 0 ";
                var staffdata = slmdb.ExecuteStoreQuery<kkslm_ms_staff>(sql).FirstOrDefault();

                if (staffdata != null)
                {
                    return staffdata.slm_StaffTypeId;
                }
                else
                {
                    return null;
                }
            }
        }

        //Reference: SlmScr016Biz, SlmScr004Biz
        public List<StaffData> GetChannelStaffData(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (username != "")
                {
                    string sql = @"SELECT staff.slm_UserName AS UserName
                                , CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                                   ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS StaffNameTH
                                , chan.slm_ChannelId AS ChannelId, branch.slm_BranchName AS BranchName,chan.slm_ChannelDesc AS ChannelDesc 
                                FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_branch branch ON staff.slm_BranchCode = branch.slm_BranchCode
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_channel chan ON branch.slm_ChannelId = chan.slm_ChannelId
                                WHERE staff.slm_UserName = '" + username + "' AND staff.is_Deleted = 0 ";

                    return slmdb.ExecuteStoreQuery<StaffData>(sql).ToList();
                }
                else
                {
                    string sql = @"SELECT staff.slm_UserName AS UserName
                                , CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                                   ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS StaffNameTH
                                , chan.slm_ChannelId AS ChannelId, branch.slm_BranchName AS BranchName,chan.slm_ChannelDesc AS ChannelDesc 
                                FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff 
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_branch branch ON staff.slm_BranchCode = branch.slm_BranchCode
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_channel chan ON branch.slm_ChannelId = chan.slm_ChannelId
                                WHERE staff.is_Deleted = 0 ";

                    return slmdb.ExecuteStoreQuery<StaffData>(sql).ToList();
                }
            }
        }

        public int? GetDeptData(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staffdata = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username && p.is_Deleted == 0).FirstOrDefault();
                if (staffdata != null)
                    return staffdata.slm_DepartmentId;
                else
                    return null;
            }
        }

        public bool CheckUsernameExist(string username, int? staffid)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (staffid == null)
                {
                    var user = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username && p.is_Deleted == 0).FirstOrDefault();
                    return user != null ? true : false;
                }
                else
                {
                    var user = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username && p.is_Deleted == 0 && p.slm_StaffId != staffid).FirstOrDefault();
                    return user != null ? true : false;
                }
            }
        }

        public bool CheckEmpCodeExist(string empCode, int? staffid)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (staffid == null)
                {
                    var user = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == empCode && p.is_Deleted == 0).FirstOrDefault();
                    return user != null ? true : false;
                }
                else
                {
                    var user = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == empCode && p.is_Deleted == 0 && p.slm_StaffId != staffid).FirstOrDefault();
                    return user != null ? true : false;
                }
            }
        }

        public bool CheckMarketingCodeExist(string marketingCode, int? staffid)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (staffid == null)
                {
                    return slmdb.kkslm_ms_staff.Any(p => p.slm_MarketingCode == marketingCode && p.is_Deleted == 0);
                }
                else
                {
                    return slmdb.kkslm_ms_staff.Any(p => p.slm_MarketingCode == marketingCode && p.is_Deleted == 0 && p.slm_StaffId != staffid);
                }
            }
        }

        public bool CheckExistGroupInBranch(string branchCode, int? staffid)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (staffid == null)
                {
                    var user = slmdb.kkslm_ms_staff.Where(p => p.slm_BranchCode == branchCode && p.slm_UserType.ToUpper() == "G" && p.is_Deleted == 0).FirstOrDefault();
                    return user != null ? true : false;
                }
                else
                {
                    var user = slmdb.kkslm_ms_staff.Where(p => p.slm_BranchCode == branchCode && p.slm_UserType.ToUpper() == "G" && p.is_Deleted == 0 && p.slm_StaffId != staffid).FirstOrDefault();
                    return user != null ? true : false;
                }
            }
        }

        public string InsertStaff(StaffDataManagement data, string username)
        {
            try
            {
                kkslm_ms_staff staff = new kkslm_ms_staff();
                staff.slm_UserName = data.Username;
                staff.slm_UserType = data.UserType;
                staff.slm_EmpCode = data.EmpCode;

                if (!string.IsNullOrEmpty(data.MarketingCode))
                    staff.slm_MarketingCode = data.MarketingCode;

                staff.slm_marketing = data.IsMarketing;
                staff.slm_StaffNameTH = data.StaffNameTH;

                if (!string.IsNullOrEmpty(data.TelNo))
                    staff.slm_TellNo = data.TelNo;

                if (!string.IsNullOrEmpty(data.TelNo2))
                    staff.slm_telNo2 = data.TelNo2;

                if (!string.IsNullOrEmpty(data.TelNo3))
                    staff.slm_telNo3 = data.TelNo3;

                staff.slm_StaffEmail = data.StaffEmail;
                staff.slm_Position_id = data.PositionId;

                if (!string.IsNullOrEmpty(data.Team))
                    staff.slm_Team = data.Team;
                if (data.StaffTypeId != null)
                    staff.slm_StaffTypeId = data.StaffTypeId.Value;
                if (data.RoleServiceId != null)
                    staff.crm_role_id = data.RoleServiceId.Value;
                if (!string.IsNullOrEmpty(data.BranchCode))
                    staff.slm_BranchCode = data.BranchCode;
                if (data.HeadStaffId != null)
                    staff.slm_HeadStaffId = data.HeadStaffId.Value;
                if (data.DepartmentId != null)
                    staff.slm_DepartmentId = data.DepartmentId.Value;

                staff.slm_Level = data.Level;
                staff.slm_Staff_Categroy_Host = data.Host;
                staff.slm_Staff_Category = data.Category;
                staff.slm_TeamTelesales_Id = data.TeamTelesale;
                //staff.slm_InternalPhone = data.InternalPhone; -*- 

                staff.slm_CreatedBy = username;
                staff.slm_CreatedDate = DateTime.Now;
                staff.is_Deleted = 0;
                staff.slm_IsActive = 0;
                staff.slm_IsLocked = 0;
                staff.slm_UpdateStatusDate = DateTime.Now;
                staff.slm_UpdateStatusBy = username;

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    slmdb.kkslm_ms_staff.AddObject(staff);
                    slmdb.SaveChanges();
                    return staff.slm_StaffId.ToString();
                }
            }
            catch
            {
                throw;
            }
        }

        public string UpdateStaff(StaffDataManagement data, string username, int flag)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_StaffId == data.StaffId).FirstOrDefault();
                if (staff != null)
                {
                    try
                    {
                        staff.slm_UserName = data.Username;
                        staff.slm_UserType = data.UserType;
                        staff.slm_EmpCode = data.EmpCode;
                        staff.slm_MarketingCode = data.MarketingCode;
                        staff.slm_marketing = data.IsMarketing;
                        staff.slm_StaffNameTH = data.StaffNameTH;
                        staff.slm_TellNo = data.TelNo;
                        staff.slm_telNo2 = data.TelNo2;
                        staff.slm_telNo3 = data.TelNo3;
                        staff.slm_StaffEmail = data.StaffEmail;
                        staff.slm_Position_id = data.PositionId;
                        staff.slm_Team = data.Team;
                        if (data.StaffTypeId != null)
                            staff.slm_StaffTypeId = data.StaffTypeId.Value;
                        if (data.RoleServiceId != null)
                            staff.crm_role_id = data.RoleServiceId.Value;
                        if (!string.IsNullOrEmpty(data.BranchCode))
                            staff.slm_BranchCode = data.BranchCode;

                        staff.slm_DepartmentId = data.DepartmentId;
                        staff.slm_Level = data.Level;
                        staff.slm_Staff_Categroy_Host = data.Host;
                        staff.slm_Staff_Category = data.Category;
                        staff.slm_TeamTelesales_Id = data.TeamTelesale;
                        //staff.slm_InternalPhone = data.InternalPhone; -*-

                        staff.slm_HeadStaffId = data.HeadStaffId;
                        staff.slm_UpdatedBy = username;
                        staff.slm_UpdatedDate = DateTime.Now;
                        staff.is_Deleted = data.Is_Deleted.Value;
                        if (flag == 1)
                        {
                            staff.slm_UpdateStatusDate = DateTime.Now;
                            staff.slm_UpdateStatusBy = username;
                        }

                        slmdb.SaveChanges();
                        return data.StaffId.ToString();

                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                    return data.StaffId.ToString();
            }
        }

        public StaffDataManagement GetStaffDataForInsert(int staffId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_staff.Where(p => p.slm_StaffId == staffId).Select(p =>
                    new StaffDataManagement
                    {
                        StaffId = p.slm_StaffId,
                        Username = p.slm_UserName,
                        UserType = p.slm_UserType,
                        EmpCode = p.slm_EmpCode,
                        MarketingCode = p.slm_MarketingCode,
                        IsMarketing = p.slm_marketing ?? false,
                        StaffNameTH = p.slm_StaffNameTH,
                        TelNo = p.slm_TellNo,
                        TelNo2 = p.slm_telNo2,
                        TelNo3 = p.slm_telNo3,
                        StaffEmail = p.slm_StaffEmail,
                        StaffTypeId = p.slm_StaffTypeId,
                        RoleServiceId = p.crm_role_id,
                        Team = p.slm_Team,
                        BranchCode = p.slm_BranchCode,
                        HeadStaffId = p.slm_HeadStaffId,
                        PositionId = p.slm_Position_id,
                        PositionName = p.slm_PositionName,
                        DepartmentId = p.slm_DepartmentId,
                        Level = p.slm_Level,
                        Host = p.slm_Staff_Categroy_Host,
                        Category = p.slm_Staff_Category,
                        TeamTelesale = p.slm_TeamTelesales_Id,
                        //InternalPhone = p.slm_InternalPhone, -*-
                        Is_Deleted = p.is_Deleted
                    }).FirstOrDefault();
            }
        }

        public List<StaffDataManagement> SearchStaffList(string username, string branchCode, string empCode, string marketingCode, string staffNameTH, string positionId
            , string staffTypeId, string team, string departmentId)
        {
            string sql = @"SELECT staff.slm_StaffId AS StaffId, staff.slm_EmpCode AS EmpCode, staff.slm_MarketingCode AS MarketingCode, staff.slm_UserName AS Username, staff.slm_StaffNameTH AS StaffNameTH
                            , staff.slm_Position_id AS PositionId, pos.slm_PositionNameTH AS PositionName, staff.slm_StaffTypeId AS StaffTypeId, st.slm_StaffTypeDesc AS StaffTypeDesc, staff.slm_Team AS Team, staff.slm_BranchCode AS BranchCode, branch.slm_BranchName AS BranchName
                            , staff.slm_DepartmentId AS DepartmentId, dep.slm_DepartmentName AS DepartmentName, staff.is_Deleted AS Is_Deleted,staff.slm_UpdateStatusDate AS UpdateStatusDate
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type st ON staff.slm_StaffTypeId = st.slm_StaffTypeId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_branch branch ON staff.slm_BranchCode = branch.slm_BranchCode
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_department dep ON staff.slm_DepartmentId = dep.slm_DepartmentId 
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id ";

            string whr = "";
            whr += (username == "" ? "" : (whr == "" ? "" : " AND ") + " staff.slm_UserName LIKE @UserName ");
            whr += (branchCode == "" ? "" : (whr == "" ? "" : " AND ") + " staff.slm_BranchCode = '" + branchCode + "' ");
            whr += (empCode == "" ? "" : (whr == "" ? "" : " AND ") + " staff.slm_EmpCode LIKE @EmpCode ");
            whr += (marketingCode == "" ? "" : (whr == "" ? "" : " AND ") + " staff.slm_MarketingCode LIKE @MarketingCode ");
            whr += (staffNameTH == "" ? "" : (whr == "" ? "" : " AND ") + " staff.slm_StaffNameTH LIKE @FullName ");
            whr += (positionId == "" ? "" : (whr == "" ? "" : " AND ") + " staff.slm_Position_id = '" + positionId + "' ");
            whr += (staffTypeId == "" ? "" : (whr == "" ? "" : " AND ") + " staff.slm_StaffTypeId = '" + staffTypeId + "' ");
            whr += (team == "" ? "" : (whr == "" ? "" : " AND ") + " staff.slm_Team LIKE @Team ");
            whr += (departmentId == "" ? "" : (whr == "" ? "" : " AND ") + " staff.slm_DepartmentId = '" + departmentId + "' ");

            sql += (whr == "" ? "" : " WHERE " + whr);
            sql += " ORDER BY staff.slm_EmpCode ";

            object[] param = new object[]
            {
                new SqlParameter("@UserName", (username != null ? "%" + username + "%" : "")),
                new SqlParameter("@EmpCode", (empCode != null ? "%" + empCode + "%" : "")),
                new SqlParameter("@MarketingCode", (marketingCode != null ? "%" + marketingCode + "%" : "")),
                new SqlParameter("@FullName", (staffNameTH != null ? "%" + staffNameTH + "%" : "")),
                new SqlParameter("@Team", (team != null ? "%" + team + "%" : ""))
            };

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<StaffDataManagement>(sql, param).ToList();
            }
        }

        public StaffDataManagement GetStaffDataByEmpcode(string empcode, string dept)
        {
            string sql = @" SELECT staff.slm_StaffId AS StaffId, staff.slm_EmpCode AS EmpCode, staff.slm_MarketingCode AS MarketingCode, staff.slm_UserName AS Username, staff.slm_StaffNameTH AS StaffNameTH
	                            , staff.slm_Position_id AS PositionId, staff.slm_PositionName AS PositionName, staff.slm_StaffTypeId AS StaffTypeId, st.slm_StaffTypeDesc AS StaffTypeDesc, staff.slm_Team AS Team, staff.slm_BranchCode AS BranchCode
	                            , staff.slm_DepartmentId AS DepartmentId, dep.slm_DepartmentName AS DepartmentName, staff.is_Deleted AS Is_Deleted, staff.slm_HeadStaffId AS HeadStaffId
                                , staff.slm_TellNo AS TelNo, staff.slm_StaffEmail As StaffEmail
                            FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff
                            LEFT JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff_type st ON staff.slm_StaffTypeId = st.slm_StaffTypeId
                            LEFT JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_department dep ON staff.slm_DepartmentId = dep.slm_DepartmentId 
                            WHERE staff.is_Deleted = 0 AND staff.slm_EmpCode = '" + empcode + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<StaffDataManagement>(sql).FirstOrDefault();
            }

            //if (dept == "")
            //    return null;
            //else
            //{
            //    sql += (dept == "IT" ? "" : " AND staff.slm_DepartmentId = " + dept);
            //    return slmdb.ExecuteStoreQuery<StaffDataManagement>(sql).FirstOrDefault();
            //}
        }

        public string GetEmployeeCode(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).FirstOrDefault();

                if (staff != null)
                    return staff.slm_EmpCode != null ? staff.slm_EmpCode.Trim() : string.Empty;
                else
                    return string.Empty;
            }
        }

        public StaffData GetStaff(string username)
        {
            string sql = $@"SELECT staff.slm_EmpCode AS EmpCode, staff.slm_StaffTypeId AS StaffTypeId, stafftype.slm_StaffTypeDesc AS StaffTypeDesc,staff.slm_StaffId AS StaffId, staff.slm_Collapse AS Collapse
                            , CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                               ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS StaffNameTH
                            , chan.slm_ChannelId AS ChannelId, chan.slm_ChannelDesc AS ChannelDesc
                            , staff.slm_BranchCode as BranchCode
                            , branch.slm_BranchName as BranchName
                            , slm_UserName as UserName
                            , pos.slm_Position_id as PositionId
                            , pos.slm_PositionNameTH as PositionName
                            , null as HeadStaffId
                            , null as Collapse
                            , null as LicenseNo
                            , null as LicenseExpireDate
                            , team.slm_TeamTelesales_Code as TeamTelesalesCode
                            , CONVERT(NUMERIC, staff.slm_TeamTelesales_Id) as TeamTelesalesId
                            , staff.slm_StaffNameTH as StaffNameTH_NoPosition
                            FROM dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                            LEFT JOIN dbo.kkslm_ms_staff_type stafftype WITH (NOLOCK) ON staff.slm_StaffTypeId = stafftype.slm_StaffTypeId
                            LEFT JOIN dbo.kkslm_ms_position pos WITH (NOLOCK) ON staff.slm_Position_id = pos.slm_Position_id
                            LEFT JOIN dbo.kkslm_ms_branch branch WITH (NOLOCK) ON staff.slm_BranchCode = branch.slm_BranchCode
                            LEFT JOIN dbo.kkslm_ms_channel chan WITH (NOLOCK) ON branch.slm_ChannelId = chan.slm_ChannelId 
                            LEFT JOIN dbo.kkslm_ms_teamtelesales team WITH (NOLOCK) ON team.slm_TeamTelesales_Id = staff.slm_TeamTelesales_Id 
                            WHERE staff.slm_UserName = '{username}' ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<StaffData>(sql).FirstOrDefault();
            }
        }

        public bool PassPrivilegeCampaign(int flag, string campaignId, string username)
        {
            string sql = "";

            #region Code Old 2015-04-17
            //            sql = @"SELECT Z.*
            //                        FROM (
            //                        SELECT DISTINCT A.*
            //                        FROM
            //                        (
            //                            SELECT slm_PositionName+' - '+staff.slm_StaffNameTH  AS TextField, staff.slm_UserName AS ValueField,staff.slm_StaffNameTH
            //                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
            //                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_group staffgroup ON staff.slm_StaffId = staffgroup.slm_StaffId
            //                            WHERE staffgroup.slm_CampaignId IN ('" + campaignId + @"') AND staff.is_Deleted = 0  AND staff.slm_StaffTypeId <> " + SLMConstant.StaffType.ITAdministrator + @" 
            //                                  and staff.slm_BranchCode = '" + branchcode + @"' AND staff.slm_UserName = '" + username + @"'
            //                            UNION ALL
            //                            SELECT slm_PositionName+' - '+staff.slm_StaffNameTH  AS TextField, staff.slm_UserName AS ValueField,staff.slm_StaffNameTH
            //                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
            //                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_group staffgroup ON staff.slm_StaffId = staffgroup.slm_StaffId
            //                            WHERE staff.is_Deleted = 0  AND staff.slm_StaffTypeId = "+ SLMConstant.StaffType.Marketing + @" and staff.slm_BranchCode = '" + branchcode + @"' 
            //                            AND staff.slm_UserName = '" + username + @"'    
            //               ) AS A ) AS Z ";
            #endregion

            sql = @"SELECT staff.slm_StaffNameTH AS TextField ,staff.slm_UserName AS ValueField
                    FROM    kkslm_ms_branch branch inner join kkslm_ms_staff staff on staff.slm_BranchCode = branch.slm_BranchCode 
	                        INNER JOIN (
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
		                              ) AS A ON A.slm_BranchCode = STAFF.slm_BranchCode AND A.slm_StaffTypeId = STAFF.slm_StaffTypeId     
	                    WHERE {0} STAFF.slm_UserName = '{2}' AND STAFF.slm_StaffTypeId NOT IN ({3})               
                    ";

            string wh = "";

            if (flag == SLMConstant.Branch.Active)
            {
                wh = " branch.is_Deleted = 0 AND ";
            }
            else if (flag == SLMConstant.Branch.InActive)
            {
                wh = " branch.is_Deleted = 1 AND";
            }
            else if (flag == SLMConstant.Branch.All)
            {
                wh = "";
            }
            else
                wh = "";

            sql = string.Format(sql, wh, campaignId, username, SLMConstant.StaffType.ITAdministrator);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
                if (list.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        //==================================================== SLM 5 ===============================================================
        public string GetBrachCodeByStaffId(int staffId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var branchCode = slmdb.kkslm_ms_staff.Where(p => p.slm_StaffId == staffId).Select(p => p.slm_BranchCode).FirstOrDefault();
                return branchCode != null ? branchCode : "";
            }
        }

        public List<StaffAmountJobOnHand> GetAmountJobOnHandList(string branchCode)
        {
            string sql = @"SELECT staff.slm_UserName AS Username, 
	                            (
		                            SELECT COUNT(*) AS NUM
		                            FROM " + SLMDBName + @".dbo.kkslm_tr_lead lead
		                            WHERE lead.is_Deleted = 0 AND lead.slm_AssignedFlag = '1' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Owner = staff.slm_UserName AND lead.slm_Delegate IS NULL) AS AmountOwner,
	                            (
		                            SELECT COUNT(*) AS NUM
		                            FROM " + SLMDBName + @".dbo.kkslm_tr_lead lead
		                            WHERE lead.is_Deleted = 0 AND lead.slm_Delegate_Flag = '0' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Delegate = staff.slm_UserName) AS AmountDelegate
                            FROM " + SLMDBName + @".dbo.kkslm_ms_staff staff
                            WHERE staff.is_Deleted = 0 AND staff.slm_StaffTypeId <> '" + SLMConstant.StaffType.ITAdministrator.ToString() + "' AND staff.slm_BranchCode = '" + branchCode + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<StaffAmountJobOnHand>(sql).ToList();
            }
        }

        public StaffAmountJobOnHand GetAmountJobOnHand(string username)
        {
            string sql = @"SELECT staff.slm_UserName AS Username, 
                            (
                                SELECT COUNT(*) AS NUM
                                FROM " + SLMDBName + @".dbo.kkslm_tr_lead lead
                                WHERE lead.is_Deleted = 0 AND lead.slm_AssignedFlag = '1' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Owner = staff.slm_UserName AND lead.slm_Delegate IS NULL) AS AmountOwner,
                            (
                                SELECT COUNT(*) AS NUM
                                FROM " + SLMDBName + @".dbo.kkslm_tr_lead lead
                                WHERE lead.is_Deleted = 0 AND lead.slm_Delegate_Flag = '0' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Delegate = staff.slm_UserName) AS AmountDelegate
                        FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff
                        WHERE staff.slm_UserName = '" + username + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<StaffAmountJobOnHand>(sql).FirstOrDefault();
            }
        }

        public int? GetPositionId(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).Select(p => p.slm_Position_id).FirstOrDefault();
            }
        }

        public int? GetPositionIdByEmpCode(string empCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == empCode).Select(p => p.slm_Position_id).FirstOrDefault();
            }
        }

        public List<string> GetRecursiveStaffList(string username)
        {
            KKSlmMsStaffModel sModel = new KKSlmMsStaffModel();
            List<StaffData> staffList = sModel.GetStaffList();

            StaffData sData = sModel.GetStaff(username);
            int? staffId = sData.StaffId;

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

        public void SetCollapse(string username, bool isCollapse)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).FirstOrDefault();
                if (staff != null)
                {
                    staff.slm_Collapse = isCollapse;
                    slmdb.SaveChanges();
                }
            }
        }

        public string GetActiveStatusByAvailableConfig(string username)
        {
            //string sql = $@"SELECT staff.*
            //                FROM dbo.kkslm_ms_staff staff WITH (NOLOCK) 
            //                WHERE staff.slm_StaffTypeId IN (SELECT slm_StaffTypeId FROM dbo.kkslm_ms_config_available WHERE slm_SetAvailable = '1') 
            //                AND staff.slm_UserName = '{username}' ";

            //using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            //{
            //    var staff = slmdb.ExecuteStoreQuery<kkslm_ms_staff>(sql).FirstOrDefault();
            //    return staff != null ? staff.slm_IsActive.ToString().Trim() : "";
            //}

            string sql = $@"SELECT staff.slm_IsActive
                            FROM dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                            WHERE staff.slm_StaffTypeId IN (SELECT slm_StaffTypeId FROM dbo.kkslm_ms_config_available WHERE slm_SetAvailable = '1') 
                            AND staff.slm_UserName = '{username}' ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var isActive = slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
                return isActive != null ? isActive.ToString() : "";
            }
        }

        public void SetActiveStatus(string username, int status)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                //var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).FirstOrDefault();
                //if (staff != null)
                //{
                //    staff.slm_IsActive = status;
                //    slmdb.SaveChanges();
                //}

                string sql = $"UPDATE dbo.kkslm_ms_staff SET slm_IsActive = '{status.ToString()}' WHERE slm_UserName = '{username}' ";
                slmdb.ExecuteStoreCommand(sql);
            }
        }

        public bool CheckEmployeeInPosition(int positionId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var count = slmdb.kkslm_ms_staff.Where(p => p.slm_Position_id == positionId && p.is_Deleted == 0).Count();
                return count > 0 ? true : false;
            }
        }

        //==============================================================================================================

        //Added By Pom 22/04/2016
        public StaffData GetStaffDisplayInfo(string username)
        {
            try
            {
                string sql = $@"SELECT staff.slm_EmpCode AS EmpCode, license.slm_LicenseNo AS LicenseNo, staff.slm_StaffNameTH AS StaffNameTH, pos.slm_PositionNameTH AS PositionName
                                , team.slm_TeamTelesales_Code AS TeamTelesalesCode, license.slm_LicenseExpireDate AS LicenseExpireDate
                                FROM dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                                LEFT JOIN dbo.kkslm_ms_mapping_staff_license license WITH (NOLOCK) ON license.slm_EmpCode = staff.slm_EmpCode
                                LEFT JOIN dbo.kkslm_ms_position pos WITH (NOLOCK) ON pos.slm_Position_id = staff.slm_Position_id
                                LEFT JOIN dbo.kkslm_ms_teamtelesales team WITH (NOLOCK) ON team.slm_TeamTelesales_Id = staff.slm_TeamTelesales_Id
                                WHERE staff.slm_Username = '{username}' ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<StaffData>(sql).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public StaffData GetStaffRoleProduct(string username)
        {
            try
            {
                string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff WHERE slm_StaffTypeId = '17' AND slm_UserName = '" + username + "'";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<StaffData>(sql).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public StaffData GetStaffRoleAdmin(string username)
        {
            try
            {
                string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff WHERE slm_StaffTypeId IN ('7','9') AND slm_UserName = '" + username + "'";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<StaffData>(sql).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        //Add 02/06/2017
        public List<StaffData> GetList()
        {
            using (var slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"SELECT staff.slm_EmpCode AS EmpCode, staff.slm_UserName AS Username, staff.slm_StaffId AS StaffId, team.slm_TeamTelesales_Code AS TeamTelesalesCode, team.slm_TeamTelesales_Id AS TeamTelesalesId
                                , staff.slm_Position_id AS PositionId, staff.slm_BranchCode AS BranchCode
                                FROM dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                                LEFT JOIN dbo.kkslm_ms_teamtelesales team WITH (NOLOCK) ON staff.slm_TeamTelesales_Id = team.slm_TeamTelesales_Id
                                WHERE staff.is_Deleted = 0 ";

                return slmdb.ExecuteStoreQuery<StaffData>(sql).ToList();
            }
        }
    }
}
