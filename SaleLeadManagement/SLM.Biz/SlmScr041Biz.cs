using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Biz;
using SLM.Resource;
using SLM.Resource.Data;
using System.Data.Objects.SqlClient;

namespace SLM.Biz
{
    public class SlmScr041Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }



        public partial class ExcelData
        {
            public string TeamCode { get; set; }
            public string EmpCode { get; set; }
            public string TAAName { get; set; }
            public decimal Performance { get; set; }
            public string Level { get; set; }

        }

        public static List<ControlListData> GetLevelData()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_level.Where(l => l.is_Deleted == false).ToList().Select(l => new ControlListData() { TextField = l.slm_LevelName, ValueField = l.slm_LevelId.ToString() }).ToList();
            }
        }


        public bool ImportTelesaleData(List<ExcelData> lst, string year, string month, string userID, out int succ, out int fail, out List<ControlListData> result)
        {
            bool ret = true;

            result = null;
            succ = 0;
            fail = 0;

            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    var lvl = sdc.kkslm_ms_level.ToList();
                    var ttclst = sdc.kkslm_ms_teamtelesales.ToList();
                    var stflst = sdc.kkslm_ms_staff.ToList();

                    result = new List<ControlListData>();
                    int i = 1;

                    foreach (var excel in lst)
                    {
                        var lv = lvl.Where(l => l.slm_LevelCode == excel.Level.Trim()).FirstOrDefault();
                        var stf = stflst.Where(s => s.slm_EmpCode == excel.EmpCode.Trim()).FirstOrDefault();
                        var tt = ttclst.Where(t => t.slm_TeamTelesales_Code == excel.TeamCode.Trim()).FirstOrDefault();
                       
                        string err = "";

                        //Comment By Pom 28/07/2016
                        //if (tt == null) err += (err == "" ? "" : ", <br />") + " Column A : ไม่พบข้อมูล Team Code ในระบบ";   
                        //if (stf == null) err += (err == "" ? "" : ", <br />") + " Column B : ไม่พบข้อมูล Emp Code ในระบบ";
                        //if (lv == null) err += (err == "" ? "" : ", <br />") + " Column E : ไม่พบข้อมูล Level ในระบบ";      
                        //if (stf != null && lv != null && tt != null && (stf.slm_Level != lv.slm_LevelId || stf.slm_TeamTelesales_Id != tt.slm_TeamTelesales_Id)) 
                        //    err += (err == "" ? "" : ", <br />") + "ข้อมูล Team Code, Emp Code, Level ไม่สัมพันธ์กัน";

                        if (tt == null) err += (err == "" ? "" : ", <br />") + " Column A : ไม่พบข้อมูล Team Code ในระบบ";
                        if (stf == null) err += (err == "" ? "" : ", <br />") + " Column B : ไม่พบข้อมูล Emp Code ในระบบ";
                        if (excel.Level.Trim() != "" && lv == null) err += (err == "" ? "" : ", <br />") + " Column E : ไม่พบข้อมูล Level ในระบบ";

                        if (stf != null)
                        {
                            var headList = ttclst.Where(p => p.slm_HeadStaff == stf.slm_StaffId.ToString()).ToList();
                            if (headList.Count == 0)     //ไม่ใช่หัวหน้า
                            {
                                string teamCode = ttclst.Where(p => p.slm_TeamTelesales_Id == stf.slm_TeamTelesales_Id).Select(p => p.slm_TeamTelesales_Code).FirstOrDefault();
                                if (teamCode != null)
                                    teamCode = teamCode.ToUpper().Trim();
                                else
                                    teamCode = "";

                                if (excel.EmpCode.Trim() != stf.slm_EmpCode || excel.TeamCode.Trim().ToUpper() != teamCode)
                                    err += (err == "" ? "" : ", <br />") + "ข้อมูล Team Code, Emp Code ไม่สัมพันธ์กัน";
                            }
                            else
                            {
                                int count = headList.Where(p => p.slm_TeamTelesales_Code == excel.TeamCode.Trim().ToUpper()).Count();
                                if (count == 0)
                                    err += (err == "" ? "" : ", <br />") + "ข้อมูล Team Code, Emp Code ไม่สัมพันธ์กัน";
                            }
                        }

                        if (err != "")
                        {
                            fail++;
                            result.Add(new ControlListData() { ValueField = (i+2).ToString(), TextField = err });
                        }
                        else {
                            succ++;
                            //Comment by Pom 28/07/2016
                            //var pf = sdc.kkslm_tr_performance.Where(p => p.slm_Year == year && p.slm_Month == month && p.slm_TeamTelesales_Code == excel.TeamCode && p.slm_EmpCode == excel.EmpCode && p.slm_Level == lv.slm_LevelId).FirstOrDefault();

                            var pf = sdc.kkslm_tr_performance.Where(p => p.slm_Year == year && p.slm_Month == month && p.slm_EmpCode == excel.EmpCode && p.slm_TeamTelesales_Code == excel.TeamCode && p.is_Deleted == false).FirstOrDefault();
                            if (pf == null)
                            {
                                //fail++;
                                //result.Add(new ControlListData() { ValueField = i.ToString(), TextField = "ไม่พบข้อมูล" });

                                pf = new kkslm_tr_performance();
                                pf.slm_Year = year;
                                pf.slm_Month = month;
                                pf.slm_TeamTelesales_Code = tt != null ? tt.slm_TeamTelesales_Code : null;
                                pf.slm_EmpCode = stf != null ? stf.slm_EmpCode : null;
                                if (lv != null)
                                    pf.slm_Level = lv.slm_LevelId;
                                pf.slm_Performance = excel.Performance;
                                pf.slm_CreatedBy = userID;
                                pf.slm_CreatedDate = DateTime.Now;
                                pf.slm_UpdatedBy = userID;
                                pf.slm_UpdatedDate = DateTime.Now;
                                //Add Nang Code 2016/07/12
                                pf.slm_EmpName = excel.TAAName;
                                pf.slm_PolicyActPremiumTotal = null;
                                pf.slm_Discount = null;
                                pf.slm_CountPolicy = null;
                                pf.slm_CountAct = null;

                                sdc.kkslm_tr_performance.AddObject(pf);
                            }
                            else
                            {
                                
                                pf.slm_Performance = excel.Performance;
                                pf.slm_UpdatedBy = userID;
                                pf.slm_UpdatedDate = DateTime.Now;

                                pf.slm_PolicyActPremiumTotal = null;
                                pf.slm_Discount = null;
                                pf.slm_CountPolicy = null;
                                pf.slm_CountAct = null;
                            }
                        }
                        i++;

                    }

                    if (result.Count == 0)
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

        public object[] GetPerformanceList(string yearstart, string monstart, string yearend, string monend, string teamcode, string ttaname, string level, bool isNew)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var qry = from tf in sdc.kkslm_tr_performance
                          join tt in sdc.kkslm_ms_teamtelesales on tf.slm_TeamTelesales_Code equals tt.slm_TeamTelesales_Code into r1
                          from tt in r1.DefaultIfEmpty()
                          join ll in sdc.kkslm_ms_level on tf.slm_Level equals ll.slm_LevelId into r2
                          from ll in r2.DefaultIfEmpty()
                          join mm in sdc.kkslm_ms_month on tf.slm_Month equals mm.slm_MonthId into r3
                          from mm in r3.DefaultIfEmpty()
                          select new
                          {
                              tf.slm_Level,
                              TeamCode = tf.slm_TeamTelesales_Code,
                              EmpCode = tf.slm_EmpCode,
                              TTAName = tf.slm_EmpName,
                              LEVEL = ll.slm_LevelCode,
                              tf.slm_Year,
                              tf.slm_Month,
                              mm.slm_MonthNameTh,
                              Performance = tf.slm_Performance,
                              yearmonth = tf.slm_Year + tf.slm_Month,
                              PolicyActPremiumTotal = tf.slm_PolicyActPremiumTotal,
                              Discount = tf.slm_Discount,
                              CountPolicy = tf.slm_CountPolicy,
                              CountAct = tf.slm_CountAct
                          };

                // สร้าง ปี+เดือน สำหรับ where
                var start = SLMUtil.SafeInt(yearstart + SLMUtil.SafeInt(monstart).ToString("00"));
                int end;
                int valye = SLMUtil.SafeInt(yearend);
                int valme = SLMUtil.SafeInt(monend);

                if (valme == 0 && valye != 0)
                    end = SLMUtil.SafeInt((valye + 1).ToString() + "00");
                else if (valye !=0 && valme!=0)
                    end = SLMUtil.SafeInt(valye.ToString() + (valme+1).ToString("00"));
                else
                    end = SLMUtil.SafeInt(yearend + SLMUtil.SafeInt(monend).ToString("00"));


                var lvid = SLMUtil.SafeInt(level);
                if (teamcode.Trim() != "") qry = qry.Where(q => q.TeamCode == teamcode);
                if (ttaname.Trim() != "") qry = qry.Where(q => q.EmpCode == ttaname.Trim());
                if (level != "")  qry = qry.Where(q => q.slm_Level == lvid);


                // iqueryable ไม่รองรับ convert.toint32 ต้องแปลงเป็น List ก่อน
                var tmpqry = qry.ToList();
                if (start > 0) tmpqry = tmpqry.Where(q => Convert.ToInt32(q.yearmonth) >= start).ToList();
                if (end > 0) tmpqry = tmpqry.Where(q => Convert.ToInt32(q.yearmonth) < end).ToList();

                //2016-11-18
                //ไม่มีการระบุ Search Criteria
                //ตาม Requirement ของ BA ให้ กรองเอาเฉพาะ เดือนล่าสุดของคนนั้นๆ ออกมา
                if (isNew)
                {
                    var tmp1 = (from t in tmpqry
                                group t by new { t.TeamCode, t.EmpCode}
                                    into tt
                                    select new
                                    {
                                        TeamCode = tt.Key.TeamCode,
                                        EmpCode = tt.Key.EmpCode,
                                        SLMYear = tt.Max(x => x.slm_Year),
                                        SLMMonth = tt.Max(x => x.slm_Month)
                                    }).ToList();

                    var tmp2 = (from t in tmpqry
                                from s in tmp1.Where(x => x.TeamCode == t.TeamCode &&
                                                          x.EmpCode == t.EmpCode &&
                                                          x.SLMYear == t.slm_Year &&
                                                          x.SLMMonth == t.slm_Month)
                                select t
                               );

                    tmpqry = tmp2.ToList();
                }

                return tmpqry.OrderByDescending(q => q.slm_Year).ThenByDescending(q => q.slm_Month).ThenByDescending(q => q.Performance).ThenBy(q => q.TTAName).ToArray();
            }

        }
    }
}
