using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;
using SLS.Dal.Data;

namespace SLS.Dal.Models
{
    public class SlmMsStaffModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmMsStaffModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public string GetUsername(string empCode)
        {
            var username = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == empCode && p.is_Deleted == 0).Select(p => p.slm_UserName).FirstOrDefault();
            return username != null ? username : string.Empty;
        }

        public bool CheckUsernameExist(string username)
        {
            var num = slmdb.kkslm_ms_staff.Count(p => p.slm_UserName == username && p.is_Deleted == 0);
            return num > 0 ? true : false;
        }

        public bool CheckEmpCodeExist(string empCode)
        {
            var num = slmdb.kkslm_ms_staff.Count(p => p.slm_EmpCode == empCode && p.is_Deleted == 0);
            return num > 0 ? true : false;
        }

        public int? GetPositionId(string username)
        {
            return slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).Select(p => p.slm_Position_id).FirstOrDefault();
        }

        public int? GetPositionIdByEmpCode(string empCode)
        {
            return slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == empCode).Select(p => p.slm_Position_id).FirstOrDefault();
        }

        public decimal? GetStaffType(string username)
        {
            return slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).Select(p => p.slm_StaffTypeId).FirstOrDefault();
        }

        public string GetStaffName(string username)
        {
            string sql = @"SELECT CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                                ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS StaffNameTH
                            FROM " + ServiceConstant.SLMDBName + @".dbo.kkslm_ms_staff staff
                            LEFT JOIN " + ServiceConstant.SLMDBName + @".dbo.kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id
                            WHERE staff.slm_UserName = '" + username + "' ";

            string name = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
            return string.IsNullOrEmpty(name) ? "" : name;
        }

        public StaffData GetStaff(string username)
        {
            string sql = @"SELECT staff.slm_EmpCode AS EmpCode, staff.slm_StaffTypeId AS StaffTypeId, stafftype.slm_StaffTypeDesc AS StaffTypeDesc,staff.slm_StaffId AS StaffId, staff.slm_Collapse AS Collapse
                            , CASE WHEN pos.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                               ELSE pos.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS StaffNameTH
                            , chan.slm_ChannelId AS ChannelId, chan.slm_ChannelDesc AS ChannelDesc,staff.slm_BranchCode as BranchCode
                            FROM " + ServiceConstant.SLMDBName + @".dbo.kkslm_ms_staff staff WITH (NOLOCK) 
                            LEFT JOIN " + ServiceConstant.SLMDBName + @".dbo.kkslm_ms_staff_type stafftype ON staff.slm_StaffTypeId = stafftype.slm_StaffTypeId
                            LEFT JOIN " + ServiceConstant.SLMDBName + @".dbo.kkslm_ms_position pos ON staff.slm_Position_id = pos.slm_Position_id
                            LEFT JOIN " + ServiceConstant.SLMDBName + @".dbo.kkslm_ms_branch branch ON staff.slm_BranchCode = branch.slm_BranchCode
                            LEFT JOIN " + ServiceConstant.SLMDBName + @".dbo.kkslm_ms_channel chan ON branch.slm_ChannelId = chan.slm_ChannelId
                            WHERE staff.slm_UserName = '" + username + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<StaffData>(sql).FirstOrDefault();
            }
        }

        public string GetBrachCode(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var staffdata = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username && p.is_Deleted == 0).FirstOrDefault();
                if (staffdata != null)
                    return staffdata.slm_BranchCode;
                else
                    return string.Empty;
            }
        }

        public List<StaffData> GetStaffList(bool includedelete = false)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (includedelete)
                    return slmdb.kkslm_ms_staff.Select(p => new StaffData { UserName = p.slm_UserName, StaffId = p.slm_StaffId, HeadStaffId = p.slm_HeadStaffId }).ToList();
                else
                    return slmdb.kkslm_ms_staff.Where(p => p.is_Deleted == 0).Select(p => new StaffData { UserName = p.slm_UserName, StaffId = p.slm_StaffId, HeadStaffId = p.slm_HeadStaffId }).ToList();
            }
        }
    }
}
