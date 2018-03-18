using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLMBatch.Entity;
using SLMBatch.Common;
using SLMBatch.Data;

namespace SLMBatch.Business
{
    public class OBT_PRO_10_Biz : ServiceBase
    {
        #region Member
        private OBT_PRO_10_DataAccess _db = null;
        #endregion

        #region Properties
        private OBT_PRO_10_DataAccess database
        {
            get
            {
                return _db = _db ?? new OBT_PRO_10_DataAccess();
            }
        }
        #endregion

        #region Backup 05-06-2016 Classic Process
        //public void CalculatePerformance(string batchCode)
        //{
        //    Int64 batchMonitorId = 0;
        //    int totalRecord = 0;
        //    int totalSuccess = 0;
        //    int totalFail = 0;

        //    try
        //    {
        //        batchMonitorId = BizUtil.SetStartTime(batchCode);
        //        BizUtil.CheckPrerequisite(batchCode);

        //        int day = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //        DateTime lastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);

        //        string year = lastDay.Year.ToString();
        //        string month = lastDay.Month.ToString("00");

        //        List<TelesalesPerformanceData> list = GetPerformanceData(year, month, lastDay);
        //        totalRecord = list.Count;

        //        foreach (TelesalesPerformanceData data in list)
        //        {
        //            try
        //            {
        //                DateTime createDate = DateTime.Now;
        //                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
        //                kkslm_tr_performance perform = new kkslm_tr_performance()
        //                {
        //                    slm_TeamTelesales_Code = data.TelesalesTeam,
        //                    slm_EmpCode = data.EmpCode,
        //                    slm_EmpName = data.Fullname,
        //                    slm_Performance = data.PerformanceAmount,
        //                    slm_Year = year,
        //                    slm_Month = month,
        //                    slm_Level = data.LevelId,
        //                    slm_CreatedBy = "SYSTEM",
        //                    slm_CreatedDate = createDate,
        //                    slm_UpdatedBy = "SYSTEM",
        //                    slm_UpdatedDate = createDate,
        //                    is_Deleted = false
        //                };
        //                slmdb.kkslm_tr_performance.AddObject(perform);
        //                slmdb.SaveChanges();

        //                totalSuccess += 1;
        //                Console.WriteLine("Owner " + data.Username + ": SUCCESS");
        //            }
        //            catch (Exception ex)
        //            {
        //                totalFail += 1;
        //                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                string errorDetail = "Owner=" + data.Username + ", Error=" + message;

        //                BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
        //                Util.WriteLogFile(logfilename, batchCode, errorDetail);

        //                Console.WriteLine("Owner " + data.Username + ": FAIL");
        //            }
        //        }

        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("All FAIL");
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

        //        Util.WriteLogFile(logfilename, batchCode, message);
        //        BizUtil.InsertLog(batchMonitorId, "", "", message);
        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
        //        throw ex;
        //    }
        //}
        #endregion
            
        #region Backup 2017-03-22
        //        /// <summary>
        //        /// คำนวณ Performance รายเดือนของ Telesales
        //        /// </summary>
        //        /// <param name="batchCode"></param>
        //        public void CalculatePerformance(string batchCode)
        //        {
        //            Int64 batchMonitorId = 0;
        //            int totalRecord = 0;
        //            int totalSuccess = 0;
        //            int totalFail = 0;
        //            bool successFlag = true;
        //            List<decimal> tempList = new List<decimal>();

        //            try
        //            {
        //                batchMonitorId = BizUtil.SetStartTime(batchCode);
        //                BizUtil.CheckPrerequisite(batchCode);

        //                int day = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //                DateTime lastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);

        //                string year = lastDay.Year.ToString();
        //                string month = lastDay.Month.ToString("00");

        //                List<TelesalesPerformanceData> list = GetPerformanceData(year, month, lastDay);
        //                totalRecord = list.Count;

        //                foreach (TelesalesPerformanceData data in list)
        //                {
        //                    try
        //                    {
        //                        DateTime createDate = DateTime.Now;
        //                        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
        //                        kkslm_tr_performance perform = new kkslm_tr_performance()
        //                        {
        //                            slm_TeamTelesales_Code = data.TelesalesTeam,
        //                            slm_EmpCode = data.EmpCode,
        //                            slm_EmpName = data.Fullname,
        //                            slm_Performance = data.PerformanceAmount,
        //                            slm_Year = year,
        //                            slm_Month = month,
        //                            slm_Level = data.LevelId,
        //                            slm_CreatedBy = "SYSTEM",
        //                            slm_CreatedDate = createDate,
        //                            slm_UpdatedBy = "SYSTEM",
        //                            slm_UpdatedDate = createDate,
        //                            is_Deleted = false
        //                        };
        //                        slmdb.kkslm_tr_performance.AddObject(perform);
        //                        slmdb.SaveChanges();
        //                        tempList.Add(perform.slm_Performance_Id);

        //                        totalSuccess += 1;
        //                        Console.WriteLine("Owner " + data.Username + ": SUCCESS");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        successFlag = false;
        //                        Rollback(batchMonitorId, batchCode, tempList);

        //                        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                        string errorDetail = "Owner=" + data.Username + ", Error=" + message;

        //                        BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
        //                        Util.WriteLogFile(logfilename, batchCode, errorDetail);

        //                        successFlag = false;
        //                        totalSuccess = 0;
        //                        totalFail = totalRecord;
        //                        break;
        //                        //Console.WriteLine("Owner " + data.Username + ": FAIL");
        //                    }
        //                }

        //                BizUtil.SetEndTime(batchCode, batchMonitorId, (successFlag ? AppConstant.Success : AppConstant.Fail), totalRecord, totalSuccess, totalFail);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine("All FAIL");
        //                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

        //                Util.WriteLogFile(logfilename, batchCode, message);
        //                BizUtil.InsertLog(batchMonitorId, "", "", message);
        //                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
        //            }
        //        }

        //        private List<TelesalesPerformanceData> GetPerformanceData(string year, string month, DateTime lastDay)
        //        {
        //            try
        //            {
        //                if (lastDay == DateTime.Today)
        //                {
        ////                    string sql = @"SELECT lead.slm_Owner AS [Username], staff.slm_EmpCode AS EmpCode, staff.slm_StaffNameTH AS Fullname
        ////                                    , staff.slm_Team AS TelesalesTeam, staff.slm_Level AS LevelId, lev.slm_LevelName AS LevelName 
        ////                                    , SUM(reins.slm_PolicyGrossPremium + reins.slm_ActNetPremium) AS PerformanceAmount
        ////                                    FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
        ////                                    INNER JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance reins ON reins.slm_TicketId = lead.slm_ticketId
        ////                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_staff staff ON staff.slm_UserName = lead.slm_Owner
        ////                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_level lev ON lev.slm_LevelId = staff.slm_Level
        ////                                    WHERE lead.is_Deleted = 0 AND reins.slm_IncentiveFlag = 1 AND YEAR(reins.slm_IncentiveDate) = " + year + @" AND MONTH(reins.slm_IncentiveDate) = " + month + @"
        ////                                    GROUP BY lead.slm_Owner, staff.slm_EmpCode, staff.slm_StaffNameTH, staff.slm_Team, staff.slm_Level, lev.slm_LevelName ";

        //                    string sql = @"SELECT lead.slm_Owner AS [Username], staff.slm_EmpCode AS EmpCode, staff.slm_StaffNameTH AS Fullname
        //                                    , team.slm_TeamTelesales_Code AS TelesalesTeam, staff.slm_Level AS LevelId, lev.slm_LevelName AS LevelName 
        //                                    , SUM(ISNULL(reins.slm_PolicyGrossPremium, 0) + ISNULL(reins.slm_ActGrossPremium, 0)) AS PerformanceAmount
        //                                    FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
        //                                    INNER JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance reins ON reins.slm_TicketId = lead.slm_ticketId
        //                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_staff staff ON staff.slm_UserName = lead.slm_Owner
        //                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_level lev ON lev.slm_LevelId = staff.slm_Level
        //                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_teamtelesales team ON team.slm_TeamTelesales_Id = staff.slm_TeamTelesales_Id
        //                                    WHERE lead.is_Deleted = 0 AND 
        //                                    (
        //                                        (reins.slm_IncentiveFlag = 1 AND YEAR(reins.slm_IncentiveDate) = '" + year + @"' AND MONTH(reins.slm_IncentiveDate) = '" + month + @"') OR
        //	                                    (reins.slm_ActIncentiveFlag = 1 AND YEAR(reins.slm_ActIncentiveDate) = '" + year + @"' AND MONTH(reins.slm_ActIncentiveDate) = '" + month + @"')
        //                                    )
        //                                    GROUP BY lead.slm_Owner, staff.slm_EmpCode, staff.slm_StaffNameTH, team.slm_TeamTelesales_Code, staff.slm_Level, lev.slm_LevelName ";

        //                    SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
        //                    return slmdb.ExecuteStoreQuery<TelesalesPerformanceData>(sql).ToList();
        //                }
        //                else
        //                    return new List<TelesalesPerformanceData>();
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }

        //        private void Rollback(Int64 batchMonitorId, string batchCode, List<decimal> tempList)
        //        {
        //            try
        //            {
        //                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
        //                string sql = "";
        //                foreach (decimal id in tempList)
        //                {
        //                    sql = "DELETE FROM " + AppConstant.SLMDBName + ".dbo.kkslm_tr_performance WHERE slm_Performance_Id = '" + id.ToString() + "'";
        //                    slmdb.ExecuteStoreCommand(sql);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                BizUtil.InsertLog(batchMonitorId, "", "", message);
        //                Util.WriteLogFile(logfilename, batchCode, message);
        //            }
        //        } 
        #endregion

        public void CalculatePerformance(string batchCode)
        {
            Int64 batchMonitorId = 0;
            bool successFlag = true;
            try
            {
                batchMonitorId = BizUtil.SetStartTime(batchCode);
                BizUtil.CheckPrerequisite(batchCode);
                int iDataCount = database.CalculatePerformance();
                BizUtil.SetEndTime(batchCode, batchMonitorId, (successFlag ? AppConstant.Success : AppConstant.Fail), iDataCount, iDataCount, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Util.WriteLogFile(logfilename, batchCode, message);
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, 0, 0, 0);
            }
        }
    }
}
