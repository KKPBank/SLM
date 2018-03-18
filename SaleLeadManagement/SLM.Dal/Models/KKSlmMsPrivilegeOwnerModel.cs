using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsPrivilegeOwnerModel
    {
       public bool GetPrivilegeOwnerData(string optionCode)
       {
           using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
           {
               var privilegeData = slmdb.kkslm_ms_privilege_owner.Where(p => p.slm_OptionCode == optionCode && p.is_Deleted == true).FirstOrDefault();
               if (privilegeData != null)
               {
                   if (privilegeData.slm_IsPrivilege_Owner == true)
                       return true;
                   else
                       return false;
               }
               else
                   return false;
           }
       }
    }
}
