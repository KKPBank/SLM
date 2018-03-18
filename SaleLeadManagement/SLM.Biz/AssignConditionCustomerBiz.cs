using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Biz
{
    public class AssignConditionCustomerBiz
    {
        private string _errorMessage = "";

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public bool ValidateData(string productId, decimal cusGradeId, decimal assignTypeId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //validate existing ในเบส
                    var conn = slmdb.kkslm_tr_assign_condition_customer.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_Customer_Grade_Id == cusGradeId
                        && p.slm_Assign_Type_Id == assignTypeId).FirstOrDefault();

                    if (conn != null)
                    {
                        _errorMessage = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากข้อมูลซ้ำกับในระบบ";
                        return false;
                    }

                    //validate member ของ grade ของการจ่ายงานนั้นๆ ซ้ำกับในเบสหรือไม่
                    List<string> current_member = slmdb.kkslm_ms_customer_grade_member.Where(p => p.is_Deleted == false && p.slm_Customer_Grade_Id == cusGradeId).Select(p => p.slm_Value).ToList();

                    decimal?[] exsiting_grade_Ids = slmdb.kkslm_tr_assign_condition_customer.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_Assign_Type_Id == assignTypeId).Select(p => p.slm_Customer_Grade_Id).ToArray();
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

        public void InsertData(string productId, string cusGradeId, string assignTypeId, string createByUsername)
        {
            try
            {
                kkslm_tr_assign_condition_customer con = new kkslm_tr_assign_condition_customer();
                con.slm_Customer_Grade_Id = decimal.Parse(cusGradeId);
                con.slm_Assign_Type_Id = decimal.Parse(assignTypeId);
                con.slm_Product_Id = productId;
                con.is_Deleted = false;

                DateTime createDate = DateTime.Now;
                con.slm_CreatedBy = createByUsername;
                con.slm_CreatedDate = createDate;
                con.slm_UpdatedBy = createByUsername;
                con.slm_UpdatedDate = createDate;

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    slmdb.kkslm_tr_assign_condition_customer.AddObject(con);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<AssignConditionData> SearchAssignConditionCustomer(string productId, string gradeId, string assignTypeId)
        {
            try
            {
                string sql = @"SELECT con.slm_Assign_Condition_Customer_Id AS AssignConditionCusId, MP.sub_product_name AS ProductName, grade.slm_Customer_Grade_Name AS GradeName, ass.slm_Assign_Type_Name AS AssignTypeName
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_assign_condition_customer con
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT MP ON con.slm_Product_Id = MP.sub_product_id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_customer_grade grade ON grade.slm_Customer_Grade_Id = con.slm_Customer_Grade_Id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_assign_type ass ON ass.slm_Assign_Type_Id = con.slm_Assign_Type_Id
                                WHERE con.is_Deleted = 0 {0} 
                                ORDER BY MP.sub_product_name, grade.slm_Customer_Grade_Name, ass.slm_Assign_Type_Name ";

                string whr = "";
                whr += (productId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_Product_Id = '" + productId + "' ");
                whr += (gradeId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_Customer_Grade_Id = '" + gradeId + "' ");
                whr += (assignTypeId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_Assign_Type_Id = '" + assignTypeId + "' ");

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

        public void DeleteData(decimal assignConditionCusId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var sortConList = slmdb.kkslm_tr_sort_condition_customer.Where(p => p.slm_Assign_Condition_Customer_Id == assignConditionCusId).ToList();
                    foreach (kkslm_tr_sort_condition_customer obj in sortConList)
                    {
                        slmdb.kkslm_tr_sort_condition_customer.DeleteObject(obj);
                    }

                    var assignConCus = slmdb.kkslm_tr_assign_condition_customer.Where(p => p.slm_Assign_Condition_Customer_Id == assignConditionCusId).FirstOrDefault();
                    if (assignConCus != null)
                    {
                        slmdb.kkslm_tr_assign_condition_customer.DeleteObject(assignConCus);
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
