using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsConfigBranchPrivilegeModel
    {
        public ConfigBranchPrivilegeData GetConfigBranchPrivilege(string branchCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_config_branch_privilege.Where(p => p.slm_BranchCode == branchCode).Select(p => new ConfigBranchPrivilegeData
                        {
                            BranchCode = p.slm_BranchCode,
                            IsEdit = p.slm_IsEdit,
                            IsView = p.slm_IsView
                        }).FirstOrDefault();
            }
        }
    }
}
