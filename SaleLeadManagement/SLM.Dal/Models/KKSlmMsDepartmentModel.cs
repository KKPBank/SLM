using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsDepartmentModel
    {
        public List<ControlListData> GetDepartmentData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_department.Where(p => p.is_Deleted == 0).OrderBy(p => p.slm_DepartmentName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_DepartmentName, ValueField = p.slm_DepartmentId.ToString() }).ToList();
            }
        }
    }
}
