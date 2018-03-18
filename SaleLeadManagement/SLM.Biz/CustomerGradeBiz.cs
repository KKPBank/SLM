using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class CustomerGradeBiz
    {
        public List<ControlListData> GetCustomerGradeList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_customer_grade.Where(p => p.is_Deleted == false && p.slm_Ranking != null).OrderBy(p => p.slm_Ranking).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_Customer_Grade_Name, ValueField = p.slm_Customer_Grade_Id.ToString() }).ToList();
            }
        }

        public List<ControlListData> GetCustomerGradeList(string conditionFlag, string productId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var cus_gradeId_list = slmdb.kkslm_ms_config_assigntype_grade.Where(p => p.slm_Conditon_Flag == conditionFlag && p.slm_Product_Id == productId).Select(p => p.slm_Customer_Grade_Id).Distinct().ToList();

                return slmdb.kkslm_ms_customer_grade.Where(p => p.is_Deleted == false && cus_gradeId_list.Contains(p.slm_Customer_Grade_Id) == true).OrderBy(p => p.slm_Customer_Grade_Name).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_Customer_Grade_Name, ValueField = p.slm_Customer_Grade_Id.ToString() }).ToList();
            }
        }
    }
}
