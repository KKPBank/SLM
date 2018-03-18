using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Biz
{
    public class SortConditionCustomerBiz
    {
        public void InsertData(string assignConditionCusId, string conditionFieldId, string sortBy, string seq, string createByUsername)
        {
            try
            {
                decimal assignCondCusId = decimal.Parse(assignConditionCusId);
                decimal conFieldId = decimal.Parse(conditionFieldId);
                decimal obj_seq = decimal.Parse(seq);

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    int count = slmdb.kkslm_tr_sort_condition_customer.Where(p => p.slm_Assign_Condition_Customer_Id == assignCondCusId && p.slm_Condition_Field_Id == conFieldId && p.is_Deleted == false).Count();
                    if (count > 0)
                        throw new Exception("ไม่สามารถบันทึกได้ เนื่องจาก ฟีลด์เงื่อนไข ซ้ำกับในระบบ");

                    count = slmdb.kkslm_tr_sort_condition_customer.Where(p => p.slm_Assign_Condition_Customer_Id == assignCondCusId && p.slm_Seq == obj_seq && p.is_Deleted == false).Count();
                    if (count > 0)
                        throw new Exception("ไม่สามารถบันทึกได้ เนื่องจาก sequence ซ้ำกับในระบบ");

                    var conFieldTab = slmdb.kkslm_ms_condition_field.Where(p => p.slm_Condition_Field_Id == conFieldId).FirstOrDefault();

                    kkslm_tr_sort_condition_customer con = new kkslm_tr_sort_condition_customer();
                    con.slm_Assign_Condition_Customer_Id = assignCondCusId;
                    con.slm_Condition_Field_Id = conFieldId;
                    con.slm_Sort_By = decimal.Parse(sortBy);
                    con.slm_Seq = decimal.Parse(seq);
                    con.is_Deleted = false;

                    DateTime createDate = DateTime.Now;
                    con.slm_CreatedBy = createByUsername;
                    con.slm_CreatedDate = createDate;
                    con.slm_UpdatedBy = createByUsername;
                    con.slm_UpdatedDate = createDate;

                    if (conFieldTab != null)
                    {
                        con.slm_Condition_Field_Name = conFieldTab.slm_Condition_Field_Name;
                        con.slm_Field_Name = conFieldTab.slm_Field_Name;
                        con.slm_Table_Name = conFieldTab.slm_Table_Name;
                    }

                    slmdb.kkslm_tr_sort_condition_customer.AddObject(con);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SortConditionCustomerData> GetSortConditionCustomerList(string assignConditionCusId)
        {
            try
            {
                string sql = @"SELECT con.slm_Sort_Condition_Customer_Id AS SortConCusId, con.slm_Condition_Field_Name AS ConditionFieldName,
                                con.slm_Sort_By AS SortBy, con.slm_Seq AS Seq	
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_sort_condition_customer con 
                                WHERE con.slm_Assign_Condition_Customer_Id = '" + assignConditionCusId + @"' 
                                ORDER BY con.slm_Seq, con.slm_Condition_Field_Name ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<SortConditionCustomerData>(sql).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteData(decimal sortConditionCusId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var obj = slmdb.kkslm_tr_sort_condition_customer.Where(p => p.slm_Sort_Condition_Customer_Id == sortConditionCusId).FirstOrDefault();
                    if (obj != null)
                    {
                        slmdb.kkslm_tr_sort_condition_customer.DeleteObject(obj);
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
