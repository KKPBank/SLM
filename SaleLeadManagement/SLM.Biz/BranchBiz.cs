using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class BranchBiz
    {
        public static string GetBranchName(string branchCode)
        {
            KKSlmMsBranchModel branch = null;
            try
            {
                branch = new KKSlmMsBranchModel();
                return branch.GetBranchName(branchCode);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (branch != null)
                {
                    branch.Dispose();
                }
            }
        }

        public static string GetBranchNameNew(string branchCode)
        {
            KKSlmMsBranchModel branch = null;
            try
            {
                branch = new KKSlmMsBranchModel();
                return branch.GetBranchName(branchCode);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (branch != null)
                {
                    branch.Dispose();
                }
            }
        }

        public static bool CheckBranchActive(string branchCode)
        {
            KKSlmMsBranchModel branch = null;
            try
            {
                branch = new KKSlmMsBranchModel();
                return branch.CheckBranchActive(branchCode);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (branch != null)
                {
                    branch.Dispose();
                }
            }
        }

		public static bool CheckNewBranchActive(string branchCode)
        {
            KKSlmMsBranchModel branch = null;
            try
            {
                branch = new KKSlmMsBranchModel();
                return branch.CheckNewBranchActive(branchCode);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (branch != null)
                {
                    branch.Dispose();
                }
            }
        }
		
        /// <summary>
        /// GetBranchList Flag 1=Active Branch, 2=Inactive Branch, 3=All
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static List<ControlListData> GetBranchList(int flag)
        {
            KKSlmMsBranchModel branch = null;
            try
            {
                branch = new KKSlmMsBranchModel();
                return branch.GetBranchList(flag);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (branch != null)
                {
                    branch.Dispose();
                }
            }
        }

		public static string GetBranchCodeNew(string branchCode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetBranchCodeNew(branchCode);
        }
        /// <summary>
		/// GetUpperBranchList Flag 1=Active Branch, 2=Inactive Branch, 3=All
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static List<ControlListData> GetUpperBranchList(int flag)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetUpperBranchList(flag);
        }

        public static List<ControlListData> GetUpperBranchListEdit(string branchCodeNew)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetUpperBranchListEdit(branchCodeNew);
        }

        /// <summary>
        /// GetBranchListByRole
        /// </summary>
        /// <param name="flag">Flag 1=Active Branch, 2=Inactive Branch, 3=All</param>
        /// <param name="staffTypeId"></param>
        /// <returns></returns>
        public static List<ControlListData> GetBranchListByRole(int flag, string staffTypeId)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetBranchListByRole(flag, staffTypeId);
        }

        public static List<ControlListData> GetBranchList(int flag,string[] branchcode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetBranchList(flag);
        }

        public static List<BranchData> SearchBranch(string branchCode, string branchName, string channelId, bool statusActive, bool statusInActive)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.SearchBranch(branchCode, branchName, channelId, statusActive, statusInActive);
        }

        public static string InsertData(string branchCode, string branchName, string startTimeHour, string startTimeMin, string endTimeHour, string endTimeMin, string upperBranchCode, string channelId, bool isActive, string createby
            , string house_no, string moo, string building, string village, string soi, string street, int tamboi, int amphur, int province, string zipcode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.InsertData(branchCode, branchName, startTimeHour, startTimeMin, endTimeHour, endTimeMin, upperBranchCode, channelId, isActive, createby
                    , house_no, moo, building, village, soi, street, tamboi, amphur, province, zipcode);
        }

        public static void UpdateData(string branchCodeOld, string branchCode, string branchName, string startTimeHour, string startTimeMin, string endTimeHour, string endTimeMin, string upperBranchCode, string channelId, bool isActive, string updateby
            , string house_no, string moo, string building, string village, string soi, string street, int tamboi, int amphur, int province, string zipcode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            branch.UpdateData(branchCodeOld, branchCode, branchName, startTimeHour, startTimeMin, endTimeHour, endTimeMin, upperBranchCode, channelId, isActive, updateby
             , house_no, moo, building, village, soi, street, tamboi, amphur, province, zipcode);
        }

        public static BranchData GetBranch(string branchCode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetBranch(branchCode);
        }

        public static BranchData GetBranchNew(string branchCode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetBranchNew(branchCode);
        }

        public static bool CheckEmployeeInBranch(string branchCode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.CheckEmployeeInBranch(branchCode);
        }

        public static List<ControlListData> GetMonitoringBranchList(int flag,string username)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetMonitoringBranchList(flag, username);
        }
        public static List<ControlListData> GetMonitoringBranchList2(int flag,string username)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetMonitoringBranchList2(flag, username);
        }
        public static bool CheckRecurviceBranch(string branchCode, string upperBranchCode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.CheckRecurviceBranch(branchCode, upperBranchCode);
        }

        public static bool CheckDuplicateBranchCode(string branchCode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.CheckDuplicateBranchCode(branchCode);
        }

        public static bool CheckDuplicateBranchName(string branchName)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.CheckDuplicateBranchName(branchName);
        }

        public static bool CheckDuplicateBranchNameForEdit(string branchCode, string branchName)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.CheckDuplicateBranchNameForEdit(branchCode, branchName);
        }

        public static void InsertBranchRole(string branchCode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            branch.InsertBranchRole(branchCode);
        }
    }
}
