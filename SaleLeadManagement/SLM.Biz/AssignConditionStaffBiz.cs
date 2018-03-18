using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Biz
{
    public class AssignConditionStaffBiz
    {
        private string _errorMessage = "";

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public bool ValidateData(string productId, decimal cusGradeId, decimal assignTypeId, decimal teamTelesalesId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //validate existing ในเบส
                    var conn = slmdb.kkslm_tr_assign_condition_staff.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_Customer_Grade_Id == cusGradeId
                        && p.slm_Assign_Type_Id == assignTypeId && p.slm_TeamTelesales_Id == teamTelesalesId).FirstOrDefault();

                    if (conn != null)
                    {
                        _errorMessage = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากข้อมูลซ้ำกับในระบบ";
                        return false;
                    }

                    //validate member ของ grade ของการจ่ายงานนั้นๆ ซ้ำกับในเบสหรือไม่
                    List<string> current_member = slmdb.kkslm_ms_customer_grade_member.Where(p => p.is_Deleted == false && p.slm_Customer_Grade_Id == cusGradeId).Select(p => p.slm_Value).ToList();

                    decimal?[] exsiting_grade_Ids = slmdb.kkslm_tr_assign_condition_staff.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_Assign_Type_Id == assignTypeId).Select(p => p.slm_Customer_Grade_Id).ToArray();
                    List<string> existing_member = slmdb.kkslm_ms_customer_grade_member.Where(p => p.is_Deleted == false && exsiting_grade_Ids.Contains(p.slm_Customer_Grade_Id)).Select(p => p.slm_Value).ToList();

                    List<string> result = current_member.Intersect(existing_member).ToList();
                    if (result.Count == 0)
                        return true;
                    else
                    {
                        _errorMessage = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากข้อมูลซ้ำกับในระบบ";
                        return false;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertData(string productId, string cusGradeId, string assignTypeId, string teamTelesalesId, string createByUsername)
        {
            try
            {
                kkslm_tr_assign_condition_staff con = new kkslm_tr_assign_condition_staff();
                con.slm_Customer_Grade_Id = decimal.Parse(cusGradeId);
                con.slm_Assign_Type_Id = decimal.Parse(assignTypeId);
                con.slm_Product_Id = productId;
                con.slm_TeamTelesales_Id = decimal.Parse(teamTelesalesId);
                con.is_Deleted = false;

                DateTime createDate = DateTime.Now;
                con.slm_CreatedBy = createByUsername;
                con.slm_CreatedDate = createDate;
                con.slm_UpdatedBy = createByUsername;
                con.slm_UpdatedDate = createDate;

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    slmdb.kkslm_tr_assign_condition_staff.AddObject(con);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<AssignConditionData> SearchAssignConditionStaff(string productId, string gradeId, string assignTypeId, string teamTelesalesId)
        {
            try
            {
                string sql = @"SELECT con.slm_Assign_Condition_Staff_Id AS AssignConditionStaffId, MP.sub_product_name AS ProductName, grade.slm_Customer_Grade_Name AS GradeName
                                , ass.slm_Assign_Type_Name AS AssignTypeName, team.slm_TeamTelesales_Name AS TeamTelesalesName
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_assign_condition_staff con
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT MP ON con.slm_Product_Id = MP.sub_product_id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_customer_grade grade ON grade.slm_Customer_Grade_Id = con.slm_Customer_Grade_Id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_assign_type ass ON ass.slm_Assign_Type_Id = con.slm_Assign_Type_Id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_teamtelesales team ON team.slm_TeamTelesales_Id = con.slm_TeamTelesales_Id
                                WHERE con.is_Deleted = 0 {0} 
                                ORDER BY MP.sub_product_name, grade.slm_Customer_Grade_Name, ass.slm_Assign_Type_Name ";

                string whr = "";
                whr += (productId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_Product_Id = '" + productId + "' ");
                whr += (gradeId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_Customer_Grade_Id = '" + gradeId + "' ");
                whr += (assignTypeId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_Assign_Type_Id = '" + assignTypeId + "' ");
                whr += (teamTelesalesId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_TeamTelesales_Id = '" + teamTelesalesId + "' ");

                whr = whr == "" ? "" : " AND " + whr;
                sql = string.Format(sql, whr);

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<AssignConditionData>(sql).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteData(decimal assignConditionStaffId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var sortConList = slmdb.kkslm_tr_sort_condition_staff.Where(p => p.slm_Assign_Condition_Staff_Id == assignConditionStaffId).ToList();
                    foreach (kkslm_tr_sort_condition_staff obj in sortConList)
                    {
                        slmdb.kkslm_tr_sort_condition_staff.DeleteObject(obj);
                    }

                    var assignConStaff = slmdb.kkslm_tr_assign_condition_staff.Where(p => p.slm_Assign_Condition_Staff_Id == assignConditionStaffId).FirstOrDefault();
                    if (assignConStaff != null)
                    {
                        slmdb.kkslm_tr_assign_condition_staff.DeleteObject(assignConStaff);
                    }

                    slmdb.SaveChanges();
                }
                
            }
            catch
            {
                throw;
            }
        }
    }
}
