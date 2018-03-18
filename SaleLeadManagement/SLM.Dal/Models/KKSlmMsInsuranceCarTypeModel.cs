using System.Collections.Generic;
using System.Linq;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsInsuranceCarTypeModel
    {
        public List<ControlListData> GetInsuranceCarTypeForDropdownList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.kkslm_ms_insurancecartype.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_InsurancecarTypeName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_InsurancecarTypeName, ValueField = p.slm_InsurancecarTypeId.ToString() }).ToList();
                return list;
            }
        }

        public List<kkslm_ms_insurancecartype> LoadAllInsuranceCarType()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.kkslm_ms_insurancecartype.Where(p => p.is_Deleted == false).ToList();
                return list;
            }
        }

    }
}
