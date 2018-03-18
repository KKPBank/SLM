using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Dal.Models
{
    public class KKCocMsAolModel
    {
        public string GetPrivilegeNCB(string productId, decimal staffTypeId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var privilegeNCB = slmdb.kkcoc_ms_aol.Where(p => p.is_Deleted == 0 && p.coc_Product_Id == productId && p.coc_StaffTypeId == staffTypeId).Select(p => p.coc_Privilege_NBC).FirstOrDefault();
                return privilegeNCB != null ? privilegeNCB : "";
            }
        }
    }
}
