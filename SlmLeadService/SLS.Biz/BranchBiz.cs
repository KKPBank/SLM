using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Dal.Models;

namespace SLS.Biz
{
    public class BranchBiz
    {
        public static bool CheckBranchActive(string branchCode)
        {
            SlmMsBranchModel branch = new SlmMsBranchModel();
            return branch.CheckBranchActive(branchCode);
        }

        //add by nung 20161124
        public static bool CheckOldBranchActive(string branchCode)
        {
            SlmMsBranchModel branch = new SlmMsBranchModel();
            return branch.CheckOldBranchActive(branchCode);
        }

        public static string GetOldBranchCode(string branchCode)
        {
            SlmMsBranchModel branch = new SlmMsBranchModel();
            return branch.GetOldBranchCode(branchCode);
        }

        public static string GetNewBranchCode(string branchCode)
        {
            SlmMsBranchModel branch = new SlmMsBranchModel();
            return branch.GetNewBranchCode(branchCode);
        }
        //end add by nung 20161124
    }
}
