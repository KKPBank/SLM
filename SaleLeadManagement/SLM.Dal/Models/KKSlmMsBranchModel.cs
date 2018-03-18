using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;


namespace SLM.Dal.Models
{
    public class KKSlmMsBranchModel : IDisposable
    {
        private SLM_DBEntities slmdb = null;

        public KKSlmMsBranchModel()
        {
            slmdb = new SLM_DBEntities();
        }

        /// <summary>
        /// GetBranchList
        /// </summary>
        /// <param name="flag">Flag 1=Active Branch, 2=Inactive Branch, 3=All</param>
        /// <returns></returns>
        public List<ControlListData> GetBranchList(int flag)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                List<ControlListData> list = null;

                if (flag == SLMConstant.Branch.Active)
                    list =
                        slmdb.kkslm_ms_branch.Where(p => p.is_Deleted == false)
                            .OrderBy(p => p.slm_BranchName)
                            .Select(
                                p => new ControlListData {TextField = p.slm_BranchName, ValueField = p.slm_BranchCode})
                            .ToList();
                else if (flag == SLMConstant.Branch.InActive)
                    list =
                        slmdb.kkslm_ms_branch.Where(p => p.is_Deleted == true)
                            .OrderBy(p => p.slm_BranchName)
                            .Select(
                                p => new ControlListData {TextField = p.slm_BranchName, ValueField = p.slm_BranchCode})
                            .ToList();
                else if (flag == SLMConstant.Branch.All)
                    list =
                        slmdb.kkslm_ms_branch.OrderBy(p => p.slm_BranchName)
                            .Select(
                                p => new ControlListData {TextField = p.slm_BranchName, ValueField = p.slm_BranchCode})
                            .ToList();
                else
                    list = new List<ControlListData>();

                return list;
            }
        }

        public string GetBranchCodeNew(string branchCode)
        {
            //var branchCodeNew =
            //    slmdb.kkslm_ms_branch.Where(i => i.slm_BranchCode == branchCode)
            //        .Select(i => new {i.slm_BranchCodeNew})
            //        .SingleOrDefault();
            //return branchCodeNew != null ? branchCodeNew.slm_BranchCodeNew : string.Empty;

            string sql = $"SELECT slm_BranchCodeNew FROM kkslm_ms_branch WITH (NOLOCK) WHERE slm_BranchCode = '{branchCode}' ";
            var branchCodeNew = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
            return string.IsNullOrWhiteSpace(branchCodeNew) ? string.Empty : branchCodeNew;
        }

        /// <summary>
        /// GetUpperBranchList
        /// </summary>
        /// <param name="flag">Flag 1=Active Branch, 2=Inactive Branch, 3=All</param>
        /// <returns></returns>
        public List<ControlListData> GetUpperBranchList(int flag)
        {
            List<ControlListData> list = null;

            if (flag == SLMConstant.Branch.Active)
                list =
                    slmdb.kkslm_ms_branch.Where(p => p.is_Deleted == false)
                        .OrderBy(p => p.slm_BranchName)
                        .Select(
                            p =>
                                new ControlListData
                                {
                                    TextField = (p.slm_BranchCodeNew ?? "") + " " + p.slm_BranchName,
                                    ValueField = p.slm_BranchCode
                                })
                        .ToList();
            else if (flag == SLMConstant.Branch.InActive)
                list =
                    slmdb.kkslm_ms_branch.Where(p => p.is_Deleted == true)
                        .OrderBy(p => p.slm_BranchName)
                        .Select(
                            p =>
                                new ControlListData
                                {
                                    TextField = (p.slm_BranchCodeNew ?? "") + " " + p.slm_BranchName,
                                    ValueField = p.slm_BranchCode
                                })
                        .ToList();
            else if (flag == SLMConstant.Branch.All)
                list =
                    slmdb.kkslm_ms_branch.OrderBy(p => p.slm_BranchName)
                        .Select(
                            p =>
                                new ControlListData
                                {
                                    TextField = (p.slm_BranchCodeNew ?? "") + " " + p.slm_BranchName,
                                    ValueField = p.slm_BranchCode
                                })
                        .ToList();
            else
                list = new List<ControlListData>();

            return list;
        }

        public List<ControlListData> GetUpperBranchListEdit(string branchCodeNew)
        {
            try
            {
                List<ControlListData> list = null;
                if (string.IsNullOrEmpty(branchCodeNew))
                {
                    list =
                        slmdb.kkslm_ms_branch.Where(p => !p.is_Deleted)
                            .OrderBy(p => p.slm_BranchName)
                            .Select(
                                p =>
                                    new ControlListData
                                    {
                                        TextField = (p.slm_BranchCodeNew ?? "") + " " + p.slm_BranchName,
                                        ValueField = p.slm_BranchCode
                                    })
                            .ToList();
                }
                else
                {
                    list =
                        slmdb.kkslm_ms_branch.Where(p => !p.is_Deleted && p.slm_BranchCode != branchCodeNew)
                            .OrderBy(p => p.slm_BranchName)
                            .Select(
                                p =>
                                    new ControlListData
                                    {
                                        TextField = (p.slm_BranchCodeNew ?? "") + " " + p.slm_BranchName,
                                        ValueField = p.slm_BranchCode
                                    })
                            .ToList();
                }


                return list;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// GetBranchList
        /// </summary>
        /// <param name="flag">Flag 1=Active Branch, 2=Inactive Branch, 3=All</param>
        /// <returns></returns>
        public List<ControlListData> GetBranchAccessRightList(int flag, string campaignId)
        {
            string sql = @"SELECT branch.slm_BranchName AS TextField, branch.slm_BranchCode AS ValueField 
                            FROM kkslm_ms_branch branch  INNER JOIN (
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

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        /// <summary>
        /// GetBranchListByRole
        /// </summary>
        /// <param name="flag">Flag 1=Active Branch, 2=Inactive Branch, 3=All</param>
        /// <param name="staffTypeId"></param>
        /// <returns></returns>
        public List<ControlListData> GetBranchListByRole(int flag, string staffTypeId)
        {
            string sql = @"SELECT branch.slm_BranchName AS TextField, branch.slm_BranchCode AS ValueField 
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_branch branch
                            WHERE branch.slm_BranchCode IN (
						                            SELECT DISTINCT slm_BranchCode FROM " + SLMConstant.SLMDBName +
                         @".dbo.kkslm_ms_branch_role br
						                            WHERE br.slm_StaffTypeId = '" + staffTypeId + @"')
                            {0}
                            ORDER BY branch.slm_BranchName ";

            string condition = "";
            if (flag == SLMConstant.Branch.Active)
                condition = " AND branch.is_Deleted = '0' ";
            else if (flag == SLMConstant.Branch.InActive)
                condition = " AND branch.is_Deleted = '1' ";
            else if (flag == SLMConstant.Branch.All)
                condition = "";
            else
                condition = "";

            sql = string.Format(sql, condition);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public List<ControlListData> GetBranchData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return
                    slmdb.kkslm_ms_branch.Where(p => p.is_Deleted == false)
                        .OrderBy(p => p.slm_BranchName)
                        .Select(p => new ControlListData {TextField = p.slm_BranchName, ValueField = p.slm_BranchCode})
                        .ToList();
            }
        }

        public string GetBranchName(string branchCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var name =
                    slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == branchCode)
                        .Select(p => p.slm_BranchName)
                        .FirstOrDefault();
                return name != null ? name : "";
            }
        }

        public string GetBranchNameNew(string branchCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var name =
                    slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCodeNew == branchCode)
                        .Select(p => p.slm_BranchName)
                        .FirstOrDefault();
                return name != null ? name : "";
            }
        }

        public bool CheckBranchActive(string branchCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == branchCode).FirstOrDefault();
                return branch != null ? (branch.is_Deleted ? false : true) : false;
            }
        }

        public bool CheckNewBranchActive(string branchCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCodeNew == branchCode).FirstOrDefault();
                return branch != null ? (branch.is_Deleted ? false : true) : false;
            }
        }

        public string GetChannelStaffData(string username)
        {
            string sql = @" SELECT  BRANCH.slm_ChannelId AS ChannelId
                            FROM    kkslm_ms_staff staff inner join kkslm_ms_branch branch on branch.slm_BranchCode = staff.slm_BranchCode 
                            WHERE   branch.is_Deleted = 0 AND STAFF.is_Deleted = 0 AND STAFF.slm_UserName = '" +
                         username + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
            }
        }

        public BranchData GetBranch(string branchCode)
        {
            string sql = @"SELECT br.slm_BranchCode AS BranchCode, br.slm_BranchName AS BranchName, slm_StartTime_Hour AS StartTimeHour, slm_StartTime_Minute AS StartTimeMinute
                            , slm_EndTime_Hour AS EndTimeHour, slm_EndTime_Minute AS EndTimeMinute, br.slm_ChannelId AS ChannelId
                            , CASE WHEN br.is_Deleted = '0' THEN 'Y'
                                   WHEN br.is_Deleted = '1' THEN 'N'
                                   ELSE '' END AS [Status]
                                  ,[slm_House_No] ,[slm_Moo] ,[slm_Building] ,[slm_Village] ,[slm_Soi] ,[slm_Street] ,[slm_TambolId] ,[slm_AmphurId] ,[slm_ProvinceId] ,[slm_Zipcode]
                            FROM dbo.kkslm_ms_branch br WITH (NOLOCK) 
                            WHERE br.slm_BranchCode = '" + branchCode + "' ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<BranchData>(sql).FirstOrDefault();
            }
        }

        public BranchData GetBranchNew(string branchCode)
        {
//return branchCodeNew เพิ่ม
            string sql = @"SELECT br.slm_BranchCode AS BranchCode, br.slm_BranchCodeNew AS BranchCodeNew, br.slm_BranchName AS BranchName, slm_StartTime_Hour AS StartTimeHour, slm_StartTime_Minute AS StartTimeMinute
                            , slm_EndTime_Hour AS EndTimeHour, slm_EndTime_Minute AS EndTimeMinute, br.slm_ChannelId AS ChannelId, slm_UpperBranch AS UpperBranch
                            , CASE WHEN br.is_Deleted = '0' THEN 'Y'
                                   WHEN br.is_Deleted = '1' THEN 'N'
                                   ELSE '' END AS [Status]
                                  ,[slm_House_No] ,[slm_Moo] ,[slm_Building] ,[slm_Village] ,[slm_Soi] ,[slm_Street] ,[slm_TambolId] ,[slm_AmphurId] ,[slm_ProvinceId] ,[slm_Zipcode]
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_branch br 
                            WHERE br.slm_BranchCode = '" + branchCode + "' ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<BranchData>(sql).FirstOrDefault();
            }
        }

        public List<BranchData> SearchBranch(string branchCode, string branchName, string channelId, bool statusActive,
            bool statusInActive)
        {
            //sql ก่อนปรับ branchCodeNew
//            string sql = @"SELECT br.slm_BranchCode AS BranchCode, br.slm_BranchName AS BranchName, slm_StartTime_Hour AS StartTimeHour, slm_StartTime_Minute AS StartTimeMinute
//                            , slm_EndTime_Hour AS EndTimeHour, slm_EndTime_Minute AS EndTimeMinute, br.slm_ChannelId AS ChannelId, ch.slm_ChannelDesc AS ChannelDesc
//                            , CASE WHEN br.is_Deleted = '0' THEN 'ใช้งาน'
//                                  WHEN br.is_Deleted = '1' THEN 'ปิดสาขา'
//                                  ELSE '' END AS StatusDesc
//                            , CASE WHEN br.is_Deleted = '0' THEN 'Y'
//                                   WHEN br.is_Deleted = '1' THEN 'N'
//                                   ELSE '' END AS [Status]
//                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_branch br
//                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_channel ch ON br.slm_ChannelId = ch.slm_ChannelId ";

            string sql = @"SELECT br.slm_BranchCode AS IdBranchCode, br.slm_BranchCodeNew AS BranchCode, br.slm_BranchName AS BranchName, slm_StartTime_Hour AS StartTimeHour, slm_StartTime_Minute AS StartTimeMinute
                            , slm_EndTime_Hour AS EndTimeHour, slm_EndTime_Minute AS EndTimeMinute, br.slm_ChannelId AS ChannelId, ch.slm_ChannelDesc AS ChannelDesc
                            , CASE WHEN br.is_Deleted = '0' THEN 'ใช้งาน'
                                  WHEN br.is_Deleted = '1' THEN 'ปิดสาขา'
                                  ELSE '' END AS StatusDesc
                            , CASE WHEN br.is_Deleted = '0' THEN 'Y'
                                   WHEN br.is_Deleted = '1' THEN 'N'
                                   ELSE '' END AS [Status]
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_branch br
                            LEFT JOIN " + SLMConstant.SLMDBName +
                         @".dbo.kkslm_ms_channel ch ON br.slm_ChannelId = ch.slm_ChannelId ";

            string whr = "";

//            whr += (branchCode == "" ? "" : (whr == "" ? "" : " AND ") + " br.slm_BranchCode LIKE @branchcode ");
            whr += (branchCode == "" ? "" : (whr == "" ? "" : " AND ") + " br.slm_BranchCodeNew LIKE @branchcode ");
            whr += (branchName == "" ? "" : (whr == "" ? "" : " AND ") + " br.slm_BranchName LIKE @branchname ");
            whr += (channelId == "" ? "" : (whr == "" ? "" : " AND ") + " br.slm_ChannelId = '" + channelId + "' ");

            if (statusActive == true && statusInActive == false)
                whr += (whr == "" ? "" : " AND ") + " br.is_Deleted = '0' ";
            else if (statusActive == false && statusInActive == true)
                whr += (whr == "" ? "" : " AND ") + " br.is_Deleted = '1' ";

            if (whr != "")
                sql += " WHERE " + whr;

            sql += " ORDER BY br.slm_BranchCode ";

            object[] param = new object[]
            {
                new SqlParameter("@branchcode", "%" + branchCode + "%"),
                new SqlParameter("@branchname", "%" + branchName + "%")
            };

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<BranchData>(sql, param).ToList();
            }
        }

        public string InsertData(string branchCode, string branchName, string startTimeHour, string startTimeMin,
            string endTimeHour, string endTimeMin, string upperBranchCode, string channelId, bool isActive,
            string createby
            , string house_no, string moo, string building, string village, string soi, string street, int tamboi,
            int amphur, int province, string zipcode)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
//                    var count = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCodeNew == branchCode).Count();
//                    if (count > 0)
//                        throw new Exception("SLM : รหัสสาขา " + branchCode + " มีในระบบแล้ว");

//                    count = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchName == branchName).Count();
//                    if (count > 0)
//                        throw new Exception("SLM : " + branchName + " มีในระบบแล้ว");

                    //find max branch code
                    var maxBranchCode = slmdb.kkslm_ms_branch.Max(p => p.slm_BranchCode);
                    maxBranchCode = (Convert.ToInt32(maxBranchCode) + 1).ToString();

                    DateTime createdDate = DateTime.Now;
                    kkslm_ms_branch branch = new kkslm_ms_branch()
                    {
                        slm_BranchCode = maxBranchCode,
                        slm_BranchCodeNew = branchCode,
                        slm_UpperBranch = upperBranchCode,
                        slm_BranchName = branchName,
                        slm_Branch_CreatedBy = branchName,
                        slm_StartTime_Hour = startTimeHour,
                        slm_StartTime_Minute = startTimeMin,
                        slm_EndTime_Hour = endTimeHour,
                        slm_EndTime_Minute = endTimeMin,
                        slm_ChannelId = (string.IsNullOrEmpty(channelId) ? null : channelId),
                        slm_CreatedBy = createby,
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = createby,
                        slm_UpdatedDate = createdDate,
                        is_Deleted = !isActive,
                        slm_House_No = house_no,
                        slm_Street = street,
                        slm_Soi = soi,
                        slm_Village = village,
                        slm_Building = building,
                        slm_TambolId = tamboi,
                        slm_AmphurId = amphur,
                        slm_ProvinceId = province,
                        slm_Zipcode = zipcode,
                        slm_Moo = moo,

                    };
                    slmdb.kkslm_ms_branch.AddObject(branch);
                    slmdb.SaveChanges();
                    return maxBranchCode;
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateData(string branchCodeOld, string branchCodeNew, string branchName, string startTimeHour,
            string startTimeMin, string endTimeHour, string endTimeMin, string upperBranchCode, string channelId,
            bool isActive, string updateBy
            , string house_no, string moo, string building, string village, string soi, string street, int tamboi,
            int amphur, int province, string zipcode)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
//                    var count =
//                        slmdb.kkslm_ms_branch.Where(
//                            p => p.slm_BranchName == branchName && p.slm_BranchCodeNew != branchCodeNew).Count();
//                    if (count > 0)
//                        throw new Exception(branchName + " มีในระบบแล้ว");

//                    if (!CheckRecurviceBranch(branchCodeNew, upperBranchCode))
//                        throw new Exception("SLM : การบันทึกข้อมูลไม่สำเร็จเนื่องจากพบ Recursive Upper Branch");

                    var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCodeNew == branchCodeNew).FirstOrDefault();
                    if (branch != null)
                    {
                        branch.slm_BranchName = branchName;
                        branch.slm_Branch_CreatedBy = branchName;
                        branch.slm_StartTime_Hour = startTimeHour;
                        branch.slm_StartTime_Minute = startTimeMin;
                        branch.slm_EndTime_Hour = endTimeHour;
                        branch.slm_EndTime_Minute = endTimeMin;
                        branch.slm_ChannelId = (string.IsNullOrEmpty(channelId) ? null : channelId);
                        branch.slm_UpdatedBy = updateBy;
                        branch.slm_UpdatedDate = DateTime.Now;
                        branch.is_Deleted = !isActive;
                        branch.slm_UpperBranch = upperBranchCode;

                        branch.slm_House_No = house_no;
                        branch.slm_Street = street;
                        branch.slm_Soi = soi;
                        branch.slm_Village = village;
                        branch.slm_Building = building;
                        branch.slm_TambolId = tamboi;
                        branch.slm_AmphurId = amphur;
                        branch.slm_ProvinceId = province;
                        branch.slm_Zipcode = zipcode;
                        branch.slm_Moo = moo;


                        slmdb.SaveChanges();
                    }
                    else
                        throw new Exception("ไม่พบรหัสสาขา " + branchCodeNew + " ในระบบ");

                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckEmployeeInBranch(string branchCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var count = slmdb.kkslm_ms_staff.Where(p => p.slm_BranchCode == branchCode && p.is_Deleted == 0).Count();
                return count > 0 ? true : false;
            }
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

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
                if (list.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public List<ControlListData> GetMonitoringBranchList2(int flag, string username)
        {

            string wh = "";
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
SELECT branch.slm_BranchName AS TextField, branch.slm_BranchCode AS ValueField FROM kkslm_ms_branch branch where {0} branch.slm_branchcode in (
select distinct BranchCode from recur where SuperHeadUserName = '{1}'  
) 

";

            if (flag == SLMConstant.Branch.Active)
            {
                wh = " branch.is_Deleted = 0 AND ";
            }
            else if (flag == SLMConstant.Branch.InActive)
            {
                wh = " branch.is_Deleted = 1 AND ";
            }
            else if (flag == SLMConstant.Branch.All)
            {
                wh = "";
            }
            else
                wh = "";

            var sql = string.Format(sqlz, wh, username);
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }
        public List<ControlListData> GetMonitoringBranchList(int flag, string username)
        {
            string sql = @"SELECT branch.slm_BranchName AS TextField, branch.slm_BranchCode AS ValueField 
                            FROM kkslm_ms_branch branch  
                            WHERE {0} slm_BranchCode IN 
                                (
                                    SELECT DISTINCT Z.slm_BranchCode 
									FROM 
									(
										select slm_BranchCode from kkslm_ms_staff  
										where {2} is_deleted =  0 and slm_HeadStaffId = 
                                        (select slm_StaffId from kkslm_ms_staff where slm_UserName = '{1}')
                                        UNION ALL
										select distinct slm_BranchCode from kkslm_ms_staff  
										where {2} slm_UserName = '{1}') AS Z)";
            string wh = "";

            if (flag == SLMConstant.Branch.Active)
            {
                wh = " branch.is_Deleted = 0 AND ";
            }
            else if (flag == SLMConstant.Branch.InActive)
            {
                wh = " branch.is_Deleted = 1 AND ";
            }
            else if (flag == SLMConstant.Branch.All)
            {
                wh = "";
            }
            else
                wh = "";

            sql = string.Format(sql, wh, username, wh);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public bool CheckRecurviceBranch(string branchCode, string upperBranchCode)
        {
            //check for recursive upperCode
            var itemUpper = slmdb.kkslm_ms_branch.SingleOrDefault(i => i.slm_BranchCode == upperBranchCode);

            if (itemUpper == null || itemUpper.slm_UpperBranch == null ||
                string.IsNullOrEmpty(itemUpper.slm_UpperBranch))
            {
                return true;
            }
            else
            {
                //has upper branch in itemUpper
                if (itemUpper.slm_UpperBranch == branchCode)
                {
                    return false;
                }

                return CheckRecurviceBranch(branchCode, itemUpper.slm_UpperBranch);

            }
        }

        public bool CheckDuplicateBranchCode(string branchCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var count = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCodeNew == branchCode).Count();
                return count <= 0;
            }
        }

        public bool CheckDuplicateBranchName(string branchName)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var count = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchName == branchName).Count();
                return count <= 0;
            }
        }

        public bool CheckDuplicateBranchNameForEdit(string branchCode, string branchName)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var count = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchName == branchName && p.slm_BranchCode != branchCode).Count();
                return count <= 0;
            }
        }

        public void InsertBranchRole(string branchCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                List<int> staffTypeIdList = slmdb.kkslm_ms_staff_type.Where(p => p.is_Deleted == 0).Select(p => p.slm_StaffTypeId).ToList();
                foreach (int id in staffTypeIdList)
                {
                    kkslm_ms_branch_role br = new kkslm_ms_branch_role
                    {
                        slm_BranchCode = branchCode,
                        slm_StaffTypeId = id
                    };
                    slmdb.kkslm_ms_branch_role.AddObject(br);
                }
                slmdb.SaveChanges();
            }
        }

    public void Dispose()
        {
            if (slmdb != null)
            {
                slmdb.Dispose();
            }
        }
    }
}
