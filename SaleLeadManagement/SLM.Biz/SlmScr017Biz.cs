using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class SlmScr017Biz
    {
        public static List<ControlListData> GetStaffTyeData()
        {
            KKSlmMsStaffTypeModel stafftype = new KKSlmMsStaffTypeModel();
            return stafftype.GetStaffTyeData();
        }
        public static List<ControlListData> GetRoleServiceData()
        {
            CsmMCrmServiceRoleModel roleService = new CsmMCrmServiceRoleModel();
            return roleService.GetRoleServiceData();
        }
        public static List<ControlListData> GetDeptData()
        {
            KKSlmMsDepartmentModel dept = new KKSlmMsDepartmentModel();
            return dept.GetDepartmentData();
        }

        public static List<StaffDataManagement> SearchStaffList(string username, string branchCode, string empCode, string marketingCode, string staffNameTH, string positionId
            , string staffTypeId, string team, string departmentId)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.SearchStaffList(username, branchCode, empCode, marketingCode, staffNameTH, positionId, staffTypeId, team, departmentId);
        }
    }
}
