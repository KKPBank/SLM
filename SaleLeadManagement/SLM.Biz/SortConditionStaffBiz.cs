using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Biz
{
    public class SortConditionStaffBiz
    {
        public void InsertData(string assignConditionStaffId, string conditionFieldId, string sortBy, string seq, string createByUsername)
        {
            try
            {
                decimal assignCondStaffId = decimal.Parse(assignConditionStaffId);
                decimal conFieldId = decimal.Parse(conditionFieldId);
                decimal obj_seq = decimal.Parse(seq);

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    int count = slmdb.kkslm_tr_sort_condition_staff.Where(p => p.slm_Assign_Condition_Staff_Id == assignCondStaffId && p.slm_Condition_Field_Id == conFieldId && p.is_Deleted == false).Count();
                    if (count > 0)
                        throw new Exception("ไม่สามารถบันทึกได้ เนื่องจาก ฟีลด์เงื่อนไข ซ้ำกับในระบบ");

                    count = slmdb.kkslm_tr_sort_condition_staff.Where(p => p.slm_Assign_Condition_Staff_Id == assignCondStaffId && p.slm_Seq == obj_seq && p.is_Deleted == false).Count();
                    if (count > 0)
                        throw new Exception("ไม่สามารถบันทึกได้ เนื่องจาก sequence ซ้ำกับในระบบ");

                    var conFieldTab = slmdb.kkslm_ms_condition_field.Where(p => p.slm_Condition_Field_Id == conFieldId).FirstOrDefault();

                    kkslm_tr_sort_condition_staff con = new kkslm_tr_sort_condition_staff();
                    con.slm_Assign_Condition_Staff_Id = assignCondStaffId;
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

                    slmdb.kkslm_tr_sort_condition_staff.AddObject(con);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SortConditionStaffData> GetSortConditionStaffList(string assignConditionStaffId)
        {
            try
            {
                string sql = @"SELECT con.slm_Sort_Condition_Staff_Id AS SortConStaffId, con.slm_Condition_Field_Name AS ConditionFieldName,
                                con.slm_Sort_By AS SortBy, con.slm_Seq AS Seq	
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_sort_condition_staff con 
                                WHERE con.slm_Assign_Condition_Staff_Id = '" + assignConditionStaffId + @"' 
                                ORDER BY con.slm_Seq, con.slm_Condition_Field_Name ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<SortConditionStaffData>(sql).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteData(decimal sortConditionStaffId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var obj = slmdb.kkslm_tr_sort_condition_staff.Where(p => p.slm_Sort_Condition_Staff_Id == sortConditionStaffId).FirstOrDefault();
                    if (obj != null)
                    {
                        slmdb.kkslm_tr_sort_condition_staff.DeleteObject(obj);
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
