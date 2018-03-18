using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Dal;

namespace SLM.Biz
{
    public class StaffBiz
    {
        public static string GetBranchCode(int staffId)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetBrachCodeByStaffId(staffId);
        }

        public static string GetBranchCode(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetBrachCode(username);
        }

        public static string GetBranchCodeByEmpCode(string empCode)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetBrachCodeByEmpCode(empCode);
        }

        public static string GetUsernameByEmpCode(string empCode)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetUsernameByEmpCode(empCode);
        }

        public static List<StaffAmountJobOnHand> GetAmountJobOnHandList(string branchCode)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetAmountJobOnHandList(branchCode);
        }

        public static List<ControlListData> GetStaffTypeList()
        {
            KKSlmMsStaffTypeModel st = new KKSlmMsStaffTypeModel();
            return st.GetStaffTyeData();
        }

        public static List<ControlListData> GetStaffList(string branchCode, string userName = null)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffList(branchCode, userName);
        }

        public static List<ControlListData> GetStaffNotDummyList(string branchCode)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffNotDummyList(branchCode);
        } 

        public static List<ControlListData> GetHeadStaffList(string branchCode)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetHeadStaffList(branchCode);
        }

        public static List<ControlListData> GetStaffAllDataByAccessRight(string campaignId, string branch)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffAllDataByAccessRight(campaignId, branch);
        }

        public static List<ControlListData> GetStaffAllDataNotDummyByAccessRight(string campaignId, string branch, string userName = null)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffAllDataNotDummyByAccessRight(campaignId, branch, userName);
        } 

        public static void SetCollapse(string username, bool isCollapse)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            staff.SetCollapse(username, isCollapse);
        }

        public static List<string> GetRecursiveStaffList(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetRecursiveStaffList(username);
        }

        public static string GetActiveStatusByAvailableConfig(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetActiveStatusByAvailableConfig(username);
        }

        public static void SetActiveStatus(string username, int status)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            staff.SetActiveStatus(username, status);
        }

        public static bool CheckEmployeeInPosition(int positionId)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.CheckEmployeeInPosition(positionId);
        }

        public static List<ControlListData> GetStaffTypeAllList()
        {
            KKSlmMsStaffTypeModel st = new KKSlmMsStaffTypeModel();
            return st.GetStaffTyeAllData();
        }

        public static List<ControlListData> GetStaffBranchAndRecursiveList(string branchCode,string recursivelist)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffBranchAndRecursiveList(branchCode, recursivelist);
        }
        public static List<ControlListData> GetStaffBranchAndRecursiveList2(string branchCode,string headStaffUserName)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffBranchAndRecursiveList2(branchCode, headStaffUserName);
        }
        public static StaffData GetStaff(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaff(username);
        }

        public StaffData GetStaffDisplayInfo(string username)
        {
            return new KKSlmMsStaffModel().GetStaffDisplayInfo(username);
        }

        public int GetAmountJobOnHand(string username)
        {
            var data = new KKSlmMsStaffModel().GetAmountJobOnHand(username);
            return (data.AmountOwner != null ? data.AmountOwner.Value : 0) + (data.AmountDelegate != null ? data.AmountDelegate.Value : 0);
        }

        public bool IsStaffResign(string userName)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_staff.Where(s => s.slm_UserName == userName && s.is_Deleted == 1).Count() > 0;
            }
        }

        public List<LicenseData> GetLicenseInfo(string empCode)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                DateTime today = DateTime.Today;

                return (from lic in sdc.kkslm_ms_mapping_staff_license
                        join typ in sdc.kkslm_ms_license_type on lic.slm_LicenseTypeId equals typ.slm_LicenseTypeId
                        where lic.slm_EmpCode == empCode && lic.slm_LicenseExpireDate >= today
                        orderby typ.slm_LicenseTypeDesc
                        select new LicenseData
                        {
                            LicenseNo = lic.slm_LicenseNo,
                            LicenseBeginDate = lic.slm_LicenseBeginDate,
                            LicenseExpireDate = lic.slm_LicenseExpireDate,
                            LicenseTypeDesc = typ.slm_LicenseTypeDesc
                        }).ToList();
            }
        }
    }
}
