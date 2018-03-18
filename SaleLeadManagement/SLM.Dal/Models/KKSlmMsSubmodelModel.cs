using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsSubmodelModel
    {
        public List<ControlListData> GetSubModelData(string BrandCode, string Family, bool useWebservice)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_submodel.Where(p => p.slm_BrandCode == BrandCode && p.slm_Family == Family).OrderBy(p => p.slm_SubModel).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_SubModel + ':' + p.slm_Description, ValueField = useWebservice ? p.slm_RedBookNo.ToString() : p.slm_SubModelId.ToString() }).ToList();
            }
        }
    }
}
