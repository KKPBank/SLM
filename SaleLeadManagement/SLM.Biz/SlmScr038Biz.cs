using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.SqlClient;
using SLM.Dal;
using SLM.Resource.Data;
using LinqKit;

namespace SLM.Biz
{
    public class SlmScr038Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }

        public object[] GetDataList(string _code, string _name, string _header, string _status)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var whr = PredicateBuilder.True<kkslm_ms_teamtelesales>();
                if (_code.Trim() != "") whr = whr.And(t => t.slm_TeamTelesales_Code.Contains(_code.Trim()));
                if (_name.Trim() != "") whr = whr.And(t => t.slm_TeamTelesales_Name.Contains(_name.Trim()));
                if (_header.Trim() != "") whr = whr.And(t => t.slm_HeadStaff == _header.Trim());
                if (_status != "") whr = whr.And(t => t.is_Deleted == (_status == "1"));

                return (from t in sdc.kkslm_ms_teamtelesales.AsExpandable().Where(whr)
                        join s in sdc.kkslm_ms_staff on t.slm_HeadStaff equals SqlFunctions.StringConvert((double)s.slm_StaffId).Trim() into j1
                        from s in j1.DefaultIfEmpty()
                        join p in sdc.kkslm_ms_position on s.slm_Position_id equals p.slm_Position_id into j2 from p in j2.DefaultIfEmpty()
                        select new
                        {
                            t.slm_TeamTelesales_Id,
                            t.slm_TeamTelesales_Code,
                            t.slm_TeamTelesales_Name,
                            t.slm_HeadStaff,
                            t.slm_CreatedBy,
                            t.slm_CreatedDate,
                            t.slm_UpdatedBy,
                            t.slm_UpdatedDate,
                            t.is_Deleted,
                            slm_StaffNameTH = (p == null ? "" : p.slm_PositionNameTH + " - ") + s.slm_StaffNameTH ?? "" // (s.slm_PositionName ?? "") + " - " + (s.slm_StaffNameTH ?? "")
                        }
                        ).OrderBy(t=>t.slm_TeamTelesales_Code).ThenBy(t=>t.slm_StaffNameTH).ToArray();
            }
        }

        public bool SaveTeamData(kkslm_ms_teamtelesales ts, string userID)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var tsu = sdc.kkslm_ms_teamtelesales.Where(t => t.slm_TeamTelesales_Id == ts.slm_TeamTelesales_Id).FirstOrDefault();
                    if (sdc.kkslm_ms_teamtelesales.Where(t => t.slm_TeamTelesales_Id != ts.slm_TeamTelesales_Id && t.slm_TeamTelesales_Name == ts.slm_TeamTelesales_Name).Count() > 0) throw new Exception("\"Team Code\" หรือ \"ชื่อทีม\" ซ้ำ");
                    if (tsu == null)
                    {
                        if (sdc.kkslm_ms_teamtelesales.Where(t => t.slm_TeamTelesales_Code == ts.slm_TeamTelesales_Code).Count() > 0
                            || sdc.kkslm_ms_teamtelesales.Where(t=>t.slm_TeamTelesales_Name == ts.slm_TeamTelesales_Name).Count() > 0) throw new Exception("\"Team Code\" หรือ \"ชื่อทีม\" ซ้ำ");

                        tsu = ts;
                        tsu.slm_CreatedBy = userID;
                        tsu.slm_CreatedDate = DateTime.Now;

                        sdc.kkslm_ms_teamtelesales.AddObject(tsu);
                    }
                    else
                    {
                        if (tsu.is_Deleted == false && ts.is_Deleted == true && sdc.kkslm_ms_staff.Where(s => s.slm_TeamTelesales_Id == tsu.slm_TeamTelesales_Id).Count() > 0) throw new Exception("ไม่สามารถแก้ไขสถานะเป็นไม่ใช้งานได้ เนื่องจากมี User อยู่ในทีม");
                        tsu.slm_TeamTelesales_Name = ts.slm_TeamTelesales_Name;
                        tsu.slm_TeamTelesales_Code = ts.slm_TeamTelesales_Code;
                        tsu.slm_HeadStaff = ts.slm_HeadStaff;
                        tsu.is_Deleted = ts.is_Deleted;
                    }

                    tsu.slm_UpdatedBy = userID;
                    tsu.slm_UpdatedDate = DateTime.Now;

                    sdc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.Message;
            }
            return ret;
        }

        public kkslm_ms_teamtelesales GetTeamData(decimal id)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_teamtelesales.Where(t => t.slm_TeamTelesales_Id == id).FirstOrDefault();
            }
        }

        public static List<ControlListData> GetStaffsList()
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_staff.Where(s => s.is_Deleted == 0 ).OrderBy(s => s.slm_StaffNameTH).ToList().Select(s => new ControlListData() { TextField = s.slm_StaffNameTH, ValueField = s.slm_StaffId.ToString() }).ToList();
            }
        }
        public static List<ControlListData> GetStaffsListWithPos()
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_staff.Where(s => s.is_Deleted == 0).OrderBy(s => s.slm_StaffNameTH).ToList().Select(s => new ControlListData() { TextField = s.slm_PositionName + " - " + s.slm_StaffNameTH, ValueField = s.slm_StaffId.ToString() }).ToList();
            }
        }
        public static List<ControlListData> GetStaffsListHeader()
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return (from t in sdc.kkslm_ms_teamtelesales
                         join s in sdc.kkslm_ms_staff on t.slm_HeadStaff equals SqlFunctions.StringConvert((double)s.slm_StaffId).Trim()
                         select new ControlListData()
                         {
                              TextField= s.slm_StaffNameTH,
                              ValueField=t.slm_HeadStaff
                         }
                        ).Distinct().ToList();
                
            }
        }

        public void GetStaffFromEmpCode(string empCode, out string name, out int? id)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var stf = (from s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode == empCode.Trim() && s.is_Deleted == 0)
                           join p in sdc.kkslm_ms_position on s.slm_Position_id equals p.slm_Position_id into j1
                           from p in j1.DefaultIfEmpty()
                           select new
                           {
                               s.slm_StaffId,
                               slm_StaffNameTH = (p == null ? "" : p.slm_PositionNameTH + " - ") + s.slm_StaffNameTH
                           }
                     ).FirstOrDefault();

                if (stf == null)
                {
                    id = null;
                    name = "";
                }
                else
                {
                    id = stf.slm_StaffId;
                    name = stf.slm_StaffNameTH;
                }
            }
        }

        public string GetStaffNameFromID(decimal staffId,out string empcode)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var stf = (from s in sdc.kkslm_ms_staff
                           join p in sdc.kkslm_ms_position on s.slm_Position_id equals p.slm_Position_id into j1
                           from p in j1.DefaultIfEmpty()
                           where s.slm_StaffId == staffId
                           select new
                           {
                               slm_StaffNameTH = (p == null ? "" : p.slm_PositionNameTH + " - ") + s.slm_StaffNameTH
                               , s.slm_EmpCode
                           }
                            ).FirstOrDefault();
                empcode = "";
                if (stf != null)
                {
                    empcode = stf.slm_EmpCode;
                    return stf.slm_StaffNameTH;
                }
                else return null;

                //return sdc.kkslm_ms_staff.Where(s => s.slm_StaffId == staffId).FirstOrDefault();
            }
        }
    }
}
