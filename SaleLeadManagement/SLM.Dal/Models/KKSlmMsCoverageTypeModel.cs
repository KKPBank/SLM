using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsCoverageTypeModel
    {
        /// <summary>
        /// GetPositionList Flag 1=Active Branch, 2=Inactive Branch, 3=All
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<ControlListData> GetCoverageTypeForDropdownList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.kkslm_ms_coveragetype.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_ConverageTypeName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_ConverageTypeName, ValueField = p.slm_CoverageTypeId.ToString() }).ToList();
                return list;
            }
        }

        public List<kkslm_ms_coveragetype> LoadAllCoverageType()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.kkslm_ms_coveragetype.Where(p => p.is_Deleted == false).ToList();
                return list;
            }
        }

        public List<ControlListData> GetCoverageTypeList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.kkslm_ms_coveragetype.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_ConverageTypeName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_ConverageTypeName, ValueField = p.slm_CoverageTypeId.ToString() }).ToList();
                return list;
            }
        }


    }
}
