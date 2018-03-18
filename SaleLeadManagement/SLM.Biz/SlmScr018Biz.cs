using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;
using System.Transactions;
using SLM.Dal;

namespace SLM.Biz
{
    public class SlmScr018Biz
    {
        public static List<CampaignWSData> GetCampaignData()
        {
            KKSlmMsCampaignModel cModel = new KKSlmMsCampaignModel();
            return cModel.GetCampaignPopupData();
        }

        public static StaffDataManagement GetStaffData(int staffId)
        {
            KKSlmMsStaffModel sModel = new KKSlmMsStaffModel();
            return sModel.GetStaffDataForInsert(staffId);
        }

        public static string GetStaffEmployeeCode(string username)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var staff = sdc.kkslm_ms_staff.Where(x => x.slm_UserName == username).Select(x => new { x.slm_EmpCode }).FirstOrDefault();
                return staff != null ? staff.slm_EmpCode : string.Empty;
            }
        }

        //public static List<StaffGroupData> GetStaffGroupData(string staffid)
        //{
        //    KKSlmMsStaffGroupModel gModel = new KKSlmMsStaffGroupModel();
        //    return gModel.GetStaffGroupData(int.Parse(staffid));
        //}

        public static List<CampaignWSData> GetCampaignData(string CampaignList)
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetCampaignPopupData(CampaignList);
        }

        //public static void InsertStaffGroup(List<StaffGroupData> ListStaffGroup, string username)
        //{
        //    KKSlmMsStaffGroupModel  sg = new KKSlmMsStaffGroupModel();
        //    sg.InsertStaffGroupList(ListStaffGroup, username);
        //}

        //public static void DeleteStaffGroup(decimal StaffGroupId, string username)
        //{
        //    KKSlmMsStaffGroupModel sg = new KKSlmMsStaffGroupModel();
        //    sg.UpdateStaffGroupList(StaffGroupId, username);
        //}
        public static List<SearchLeadResult> GetLeadOwnerDataTab18_1(string owner)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetLeadOwnerDataTab18_1(owner);
        }
        public static List<SearchLeadResult> GetLeadDelegateDataTab18_2(string DelegateUser)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetLeadDelegateDataTab18_2(DelegateUser);
        }

        public static StaffDataManagement GetStaffDataByEmpcode(string empcode, string dept)
        {
            KKSlmMsStaffModel sModel = new KKSlmMsStaffModel();
            return sModel.GetStaffDataByEmpcode(empcode, dept);
        }

        public static bool CheckExistLeadOnHand(string username)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.CheckExistLeadOnHand(username);
        }

        public static bool CheckExistPreLeadOnHand(string username)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.CheckExistPreLeadOnHand(username);
        }

        public static bool CheckExistsTeamTeleSales(string Username)
        {
            KKSlmMsTeamtelesalesModel telesale = new KKSlmMsTeamtelesalesModel();
            return telesale.CheckExistsTeamTeleSales(Username);
        }

        public static void UpdateTransferOwnerLead(List<string> ticketList, string newowner, int staffid, string username, string branchcode, string Oldowner)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
                    lead.UpdateTransferLeadOwner(ticketList, newowner, staffid, username, branchcode);

                    KKSlmTrActivityModel act = new KKSlmTrActivityModel();
                    act.InsertDataForTransfer(ticketList, Oldowner, newowner, username, "", "");

                    ts.Complete();
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateTransferDelegateLead(List<string> ticketList, string newDelegate, int staffid, string username, string branchcode, string OldDelegate)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
                    lead.UpdateTransferLeadDelegate(ticketList, newDelegate, staffid, username, branchcode);

                    KKSlmTrActivityModel act = new KKSlmTrActivityModel();
                    act.InsertDataForTransfer(ticketList, "", "", username, OldDelegate, newDelegate);

                    ts.Complete();
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object GetLicenseInfo(string empCode)
        {
            using (SLM.Dal.SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return (from lic in sdc.kkslm_ms_mapping_staff_license
                        join typ in sdc.kkslm_ms_license_type on lic.slm_LicenseTypeId equals typ.slm_LicenseTypeId
                        where lic.slm_EmpCode == empCode
                        select new { lic.slm_LicenseNo, lic.slm_LicenseBeginDate, lic.slm_LicenseExpireDate, typ.slm_LicenseTypeDesc }).ToList();
            }
        }


        public static List<ControlListData> GetStaffCategoryData()
        {
            using (SLM.Dal.SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_staff_category.Where(s => s.is_Deleted == false).OrderBy(s => s.slm_Staff_Category_Name).ToList().Select(s => new ControlListData() { TextField = s.slm_Staff_Category_Name, ValueField = s.slm_Staff_Category_Id.ToString() }).ToList();
            }
        }
        public static List<ControlListData> GetStaffCategoryHostData()
        {
            using (SLM.Dal.SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_staff_category_host.Where(s => s.is_Deleted == false).OrderBy(s => s.slm_Staff_Category_Host_Name).ToList().Select(s => new ControlListData() { TextField = s.slm_Staff_Category_Host_Name, ValueField = s.slm_Staff_Category_Host_Id.ToString() }).ToList();
            }
        }

        public static bool CheckIsLoopStructure(int staffId, int headStaffId)
        {
            using (SLM.Dal.SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var tmpHeadStaffId = headStaffId;

                var index = 0;
                var maxLoop = 1000;

                while (true)
                {
                    var headStaff = sdc.kkslm_ms_staff.Where(x => x.slm_StaffId == tmpHeadStaffId).Select(x => new { x.slm_HeadStaffId }).SingleOrDefault();
                    if (headStaff == null || !headStaff.slm_HeadStaffId.HasValue)
                        return false;

                    if (headStaff.slm_HeadStaffId == staffId)
                        return true;

                    tmpHeadStaffId = headStaff.slm_HeadStaffId.Value;

                    if (index > maxLoop)
                        return true;

                    index++;
                }
            }
        }
    }
}
