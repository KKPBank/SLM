using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class ConfigBranchPrivilegeBiz
    {
        public static ConfigBranchPrivilegeData GetConfigBranchPrivilege(string branchCode)
        {
            KKSlmMsConfigBranchPrivilegeModel config = new KKSlmMsConfigBranchPrivilegeModel();
            return config.GetConfigBranchPrivilege(branchCode);
        }
    }
}
