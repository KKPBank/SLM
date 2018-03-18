using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsRepairTypeModel
    {
        /// <summary>
        /// GetPositionList Flag 1=Active Branch, 2=Inactive Branch, 3=All
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<ControlListData> GetRepairTypeList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.kkslm_ms_repairtype.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_RepairTypeName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_RepairTypeName, ValueField = p.slm_RepairTypeId.ToString() }).ToList();
                return list;
            }
        }

    }
}
