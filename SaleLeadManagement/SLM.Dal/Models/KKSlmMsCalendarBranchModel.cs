using System;
using System.Data.Objects;
using System.Transactions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsCalendarBranchModel : IDisposable
    {
		private SLM_DBEntities slmdb = null;

        public KKSlmMsCalendarBranchModel()
        {
            slmdb = new SLM_DBEntities();
        }
        public List<CalendarBranchData> SearchCalendarBranch(string holidayDate, string holidayDesc, string branchCode)
        {
            string sql = @"SELECT cb.slm_HolidayDate AS HolidayDate, cb.slm_HolidayDesc AS HolidayDesc, cb.slm_BranchCode AS BranchCode, br.slm_BranchName AS BranchName
                                    FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_calendar_branch cb
                                    LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_branch br ON cb.slm_BranchCode = br.slm_BranchCode
                                    WHERE cb.is_Deleted = 0 {0}
                                    ORDER BY cb.slm_HolidayDate, br.slm_BranchName";

            string whr = "";
            whr += (holidayDate == "" ? "" : (whr == "" ? "" : " AND ") + " CONVERT(DATE, cb.slm_HolidayDate) = '" + holidayDate + "' ");
            whr += (holidayDesc == "" ? "" : (whr == "" ? "" : " AND ") + " cb.slm_HolidayDesc LIKE @holidaydesc ");
            whr += (branchCode == "" ? "" : (whr == "" ? "" : " AND ") + " cb.slm_BranchCode = '" + branchCode + "' ");

            whr = (whr == "" ? "" : " AND " + whr);
            sql = string.Format(sql, whr);

            object[] param = new object[] 
            { 
                new SqlParameter("@holidaydesc", "%" + holidayDesc + "%")
            };

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<CalendarBranchData>(sql, param).ToList();
            }
        }

        public void UpdateData(DateTime holidayDate, string holidayDesc, List<string> branchCodeList, string createdBy)
        {
            try
            {
                string del = "";
                string dateStr = "";

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (holidayDate.Year != 1)
                    {
                        dateStr = holidayDate.Year.ToString() + holidayDate.ToString("-MM-dd");
                        del = @"DELETE FROM " + SLMConstant.SLMDBName + ".dbo.kkslm_ms_calendar_branch WHERE CONVERT(DATE, slm_HolidayDate) = '" + dateStr + "'";
                    }
                    else
                        throw new Exception("Cannot find holiday date");

                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        if (!string.IsNullOrEmpty(del))
                            slmdb.ExecuteStoreCommand(del);

                        DateTime createdDate = DateTime.Now;
                        foreach (string branchcode in branchCodeList)
                        {
                            kkslm_ms_calendar_branch obj = new kkslm_ms_calendar_branch()
                            {
                                slm_HolidayDate = holidayDate,
                                slm_HolidayDesc = holidayDesc,
                                slm_BranchCode = branchcode,
                                slm_CreatedBy = createdBy,
                                slm_CreatedDate = createdDate,
                                slm_UpdatedBy = createdBy,
                                slm_UpdatedDate = createdDate,
                                is_Deleted = false
                            };
                            slmdb.kkslm_ms_calendar_branch.AddObject(obj);
                        }

                        slmdb.SaveChanges();
                    }
                    
                    ts.Complete();
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertData(DateTime holidayDate, string holidayDesc, List<string> branchCodeList, string createdBy)
        {
            try
            {
                DateTime createdDate = DateTime.Now;
                List<string> dbBranchCodeList = null;

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //หา branch ที่มีอยู่ในเบส
                    if (holidayDate.Year != 1)
                        dbBranchCodeList = slmdb.kkslm_ms_calendar_branch.Where(p => EntityFunctions.TruncateTime(p.slm_HolidayDate) == EntityFunctions.TruncateTime(holidayDate) && p.is_Deleted == false).Select(p => p.slm_BranchCode).ToList();
                    else
                        throw new Exception("Cannot find holiday date");

                    //เอา branch ที่มีอยู่ในเบสแล้ว ออกจาก list ที่ต้องการ insert
                    if (dbBranchCodeList.Count > 0)
                        branchCodeList = branchCodeList.Except<string>(dbBranchCodeList).ToList();

                    if (branchCodeList.Count > 0)
                    {
                        foreach (string branchcode in branchCodeList)
                        {
                            kkslm_ms_calendar_branch obj = new kkslm_ms_calendar_branch()
                            {
                                slm_HolidayDate = holidayDate,
                                slm_HolidayDesc = holidayDesc,
                                slm_BranchCode = branchcode,
                                slm_CreatedBy = createdBy,
                                slm_CreatedDate = createdDate,
                                slm_UpdatedBy = createdBy,
                                slm_UpdatedDate = createdDate,
                                is_Deleted = false
                            };
                            slmdb.kkslm_ms_calendar_branch.AddObject(obj);
                        }

                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void Dispose()
        {
            if (slmdb != null)
            {
                slmdb.Dispose();
            }
        }
    }
}
