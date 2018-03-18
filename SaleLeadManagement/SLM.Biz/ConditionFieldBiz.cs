using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Biz
{
    public class ConditionFieldBiz
    {
        public List<ControlListData> GetConditionFieldList(string conditionFlag)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_condition_field.Where(p => p.is_Deleted == false && p.slm_Conditon_Flag == conditionFlag).OrderBy(p => p.slm_Condition_Field_Name).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_Condition_Field_Name, ValueField = p.slm_Condition_Field_Id.ToString() }).ToList();
            }
        }
    }
}
