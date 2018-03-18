using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Dal;
using System.Data.Objects.SqlClient;

namespace SLM.Biz
{
    public class SlmScr019Biz
    {
        public static decimal? GetStaffTypeData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffType(username);
        }

        public static int? GetDeptData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetDeptData(username);
        }

        public static bool CheckUsernameExist(string username, int? staffid)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.CheckUsernameExist(username, staffid);
        }

        public static bool CheckEmpCodeExist(string empCode, int? staffid)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.CheckEmpCodeExist(empCode, staffid);
        }

        public static bool CheckMarketingCodeExist(string marketingCode, int? staffid)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.CheckMarketingCodeExist(marketingCode, staffid);
        }

        public static bool CheckExistGroupInBranch(string branchCode, int? staffid)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.CheckExistGroupInBranch(branchCode, staffid);
        }

        public static string InsertStaff(StaffDataManagement data, string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.InsertStaff(data, username);
        }

        public static string UpdateStaff(StaffDataManagement data, string username, int flag)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.UpdateStaff(data, username, flag);
        }

        public static List<ControlListData> GetStaffHeadData(string branch)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffHeadData(branch);
        }

        // level
        public static List<ControlListData> GetLevelList()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return (from level in sdc.kkslm_ms_level
                        where level.is_Deleted == false
                        select new ControlListData() { TextField = level.slm_LevelName, ValueField = SqlFunctions.StringConvert((double)level.slm_LevelId).Trim() }).ToList();
            }
        }
        public static List<ControlListData> GetStaffCategoryHostData()
        {
            return new List<ControlListData>();
        }

        public static List<ControlListData> GetStaffCategoryData()
        {
            return new List<ControlListData>();
        }

        // staffcategory

        // host

        // teamtelesale
        public static List<ControlListData> GetTeamTelesaleList()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return (from tt in sdc.kkslm_ms_teamtelesales
                        where tt.is_Deleted == false
                        select new ControlListData() { TextField = tt.slm_TeamTelesales_Name, ValueField = SqlFunctions.StringConvert((double)tt.slm_TeamTelesales_Id).Trim() }).ToList();
            }
        }

        public static string GetStaffTypeDesc(decimal staffTypeId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var staffType = sdc.kkslm_ms_staff_type.Where(x => x.slm_StaffTypeId == staffTypeId).Select(x => new { x.slm_StaffTypeDesc }).SingleOrDefault();
                return staffType != null ? staffType.slm_StaffTypeDesc : string.Empty;
            }
        }

        public static string GetRoleServiceCode(int roleId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var roleService = sdc.CSM_M_CRMSERVICE_ROLE.Where(x => x.CRMSERVICE_ROLE_ID == roleId).Select(x => new { x.CRMSERVICE_ROLE_CODE }).SingleOrDefault();
                return roleService != null ? roleService.CRMSERVICE_ROLE_CODE : string.Empty;
            }
        }

        public static string GetPositionNameAbb(int positionId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var position = sdc.kkslm_ms_position.Where(x => x.slm_Position_id == positionId).Select(x => new { x.slm_PositionNameAbb }).SingleOrDefault();
                return position != null ? position.slm_PositionNameAbb : string.Empty;
            }
        }

        public static string GetStaffEmployeeCode(int staffId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var staff = sdc.kkslm_ms_staff.Where(x => x.slm_StaffId == staffId).Select(x => new { x.slm_EmpCode }).SingleOrDefault();
                return staff != null ? staff.slm_EmpCode : string.Empty;
            }
        }

        public static string GetDepartmentName(int departmentId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var department = sdc.kkslm_ms_department.Where(x => x.slm_DepartmentId == departmentId).Select(x => new { x.slm_DepartmentName }).SingleOrDefault();
                return department != null ? department.slm_DepartmentName : string.Empty;
            }
        }

        public static string GetLevelName(int levelId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var level = sdc.kkslm_ms_level.Where(x => x.slm_LevelId == levelId).Select(x => new { x.slm_LevelName }).SingleOrDefault();
                return level != null ? level.slm_LevelName : string.Empty;
            }
        }

        public static string GetCategoryName(int categoryId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var category = sdc.kkslm_ms_staff_category.Where(x => x.slm_Staff_Category_Id == categoryId).Select(x => new { x.slm_Staff_Category_Name }).SingleOrDefault();
                return category != null ? category.slm_Staff_Category_Name : string.Empty;
            }
        }

        public static string GetHostName(int hostId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var host = sdc.kkslm_ms_staff_category_host.Where(x => x.slm_Staff_Category_Host_Id == hostId).Select(x => new { x.slm_Staff_Category_Host_Name }).SingleOrDefault();
                return host != null ? host.slm_Staff_Category_Host_Name : string.Empty;
            }
        }

        public static string GetTeamTelesaleName(int teamTelesaleId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var teamTelesale = sdc.kkslm_ms_teamtelesales.Where(x => x.slm_TeamTelesales_Id == teamTelesaleId).Select(x => new { x.slm_TeamTelesales_Name }).SingleOrDefault();
                return teamTelesale != null ? teamTelesale.slm_TeamTelesales_Name : string.Empty;
            }
        }

        public static string GetBranchCodeNew(string branchCode)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var branchCodeNew = sdc.kkslm_ms_branch.Where(i => i.slm_BranchCode == branchCode).Select(i => new {i.slm_BranchCodeNew}).SingleOrDefault();
                return branchCodeNew != null ? branchCodeNew.slm_BranchCodeNew : string.Empty;
            }
        }
    }
}
