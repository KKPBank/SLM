using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal;

namespace SLM.Dal.Models
{
    public class BeneficiaryModel
    {
        public List<ControlListData> GetBeneficiaryList()
        {
            using (var slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_beneficiary.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_Name).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_Name, ValueField = p.slm_Id.ToString() }).ToList();
            }
        }
    }
}
