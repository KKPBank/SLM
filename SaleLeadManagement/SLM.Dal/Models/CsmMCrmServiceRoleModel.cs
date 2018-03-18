using System.Collections.Generic;
using System.Linq;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class CsmMCrmServiceRoleModel
    {
        public List<ControlListData> GetRoleServiceData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.CSM_M_CRMSERVICE_ROLE.OrderBy(p => p.CRMSERVICE_ROLE_NAME).AsEnumerable().Select(p => new ControlListData { TextField = p.CRMSERVICE_ROLE_NAME, ValueField = p.CRMSERVICE_ROLE_ID.ToString() }).ToList();
            }
        }
    }
}