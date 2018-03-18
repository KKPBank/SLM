using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsProvinceModel
    {
        public List<ControlListData> GetProvinceData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_province.OrderBy(p => p.slm_ProvinceNameTH).Select(p => new ControlListData { TextField = p.slm_ProvinceNameTH, ValueField = p.slm_ProvinceCode }).ToList();
            }
        }
        public int? GetProvinceId(string provinceCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_province.Where(p => p.slm_ProvinceCode == provinceCode).Select(p => p.slm_ProvinceId).FirstOrDefault();
            }
        }

    }
}
