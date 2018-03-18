using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class AssignTypeBiz
    {
        public List<ControlListData> GetAssignTypeList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_assign_type.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_Assign_Type_Name).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_Assign_Type_Name, ValueField = p.slm_Assign_Type_Id.ToString() }).ToList();
            }
        }

        public List<ControlListData> GetAssignTypeList(string conditionFlag, string productId, decimal cusGradeId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var assign_type_id_list = slmdb.kkslm_ms_config_assigntype_grade.Where(p => p.slm_Conditon_Flag == conditionFlag && p.slm_Product_Id == productId && p.slm_Customer_Grade_Id == cusGradeId).Select(p => p.slm_Assign_Type_Id).ToList();

                return slmdb.kkslm_ms_assign_type.Where(p => p.is_Deleted == false && assign_type_id_list.Contains(p.slm_Assign_Type_Id) == true).OrderBy(p => p.slm_Assign_Type_Name).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_Assign_Type_Name, ValueField = p.slm_Assign_Type_Id.ToString() }).ToList();
            }
        }
    }
}
