using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsCardTypeModel
    {
        public List<ControlListData> GetCardTypeList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_cardtype.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_CardTypeId).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_CardTypeName, ValueField = p.slm_CardTypeId.ToString() }).ToList();
            }
        }

        //Added by Pom 29/06/2016
        public string GetSubScriptionTypeId(int cardTypeId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var subId = slmdb.kkslm_ms_cardtype.Where(p => p.slm_CardTypeId == cardTypeId && p.is_Deleted == false).Select(p => p.slm_CIFSubscriptTypeId).FirstOrDefault();
                    return subId != null ? subId : "0";
                }
            }
            catch
            {
                throw;
            }
        }


        //Added by Kug 17/01/2018
        public string GetCardTypeCIF(int CardTypdId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var subId = slmdb.kkslm_ms_cardtype.Where(p => p.slm_CardTypeId == CardTypdId && p.is_Deleted == false).Select(p => p.slm_CIFSubscriptTypeId).FirstOrDefault();
                    return subId != null ? subId : "";
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
