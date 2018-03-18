using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SLMBatch.Data;
using SLMBatch.Common;

namespace SLMBatch.Business
{
    public class BizUtil
    {
        private static string errMsg = string.Empty;

        public static Int64 SetStartTime(string batchCode)
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                DateTime runningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);    //BizUtil.GetBatchRunningDate(slmdb);

                var batch = slmdb.kkslm_ms_batch.Where(p => p.slm_BatchCode == batchCode).FirstOrDefault();
                if (batch != null)
                {
                    DateTime actionDate = DateTime.Now;
                    batch.slm_StartTime = actionDate;
                    batch.slm_EndTime = null;
                    batch.slm_Status = AppConstant.InProcess;
                    batch.slm_ProcessTime = null;
                    batch.slm_ActionBy = "SYSTEM";
                    batch.slm_BatchDate = runningDate;

                    kkslm_tr_batchmonitor_overview overview = new kkslm_tr_batchmonitor_overview()
                    {
                        slm_DateStart = actionDate,
                        slm_DateEnd = null,
                        slm_BatchCode = batchCode,
                        slm_ActionBy = "SYSTEM",
                        slm_BatchDate = runningDate
                    };
                    slmdb.kkslm_tr_batchmonitor_overview.AddObject(overview);

                    slmdb.SaveChanges();

                    return overview.slm_BatchMonitor_Id;
                }
                else
                    throw new Exception("ไม่พบ BatchCode " + batchCode + " ในระบบ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SetEndTime(string batchCode, Int64 batchMonitorId, string status, int totalRecord, int totalSuccess, int totalFail)
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                var batch = slmdb.kkslm_ms_batch.Where(p => p.slm_BatchCode == batchCode).FirstOrDefault();
                if (batch != null)
                {
                    DateTime startTime = batch.slm_StartTime.Value;
                    DateTime endTime = DateTime.Now;
                    TimeSpan ts = endTime.Subtract(startTime);

                    //string elapsed_time = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);

                    batch.slm_EndTime = endTime;
                    batch.slm_ProcessTime = Convert.ToInt32(Math.Ceiling(ts.TotalMinutes));
                    batch.slm_Status = status;

                    var overview = slmdb.kkslm_tr_batchmonitor_overview.Where(p => p.slm_BatchMonitor_Id == batchMonitorId).FirstOrDefault();
                    if (overview != null)
                    {
                        overview.slm_DateEnd = endTime;
                        overview.slm_Total = totalRecord;
                        overview.slm_Success = totalSuccess;
                        overview.slm_Fail = totalFail;
                        overview.slm_ActionBy = "SYSTEM";
                        slmdb.SaveChanges();
                    }

                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
            }
        }

        public static void InsertLog(Int64 batchMonitorId, string ticketId, string empCode, string errorDetail)
        {
            try
            {
                using (var slmdb = AppUtil.GetSlmDbEntities())
                {
                    kkslm_tr_batchlog log = new kkslm_tr_batchlog
                    {
                        slm_BatchMonitor_Id = batchMonitorId,
                        slm_ErrorDetail = errorDetail,
                        slm_TicketId = (string.IsNullOrEmpty(ticketId) ? null : ticketId),
                        slm_EmpCode = (string.IsNullOrEmpty(empCode) ? null : empCode)
                    };
                    slmdb.kkslm_tr_batchlog.AddObject(log);
                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
            }
        }

        public static void CheckPrerequisite(string batchCode)
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                var batch = slmdb.kkslm_ms_batch.Where(p => p.slm_BatchCode == batchCode).FirstOrDefault();

                //Check Prerequisite
                if (batch != null && !string.IsNullOrEmpty(batch.slm_PreRequisite))
                {
                    string[] pre_batchs = batch.slm_PreRequisite.Split(',');
                    foreach (string prebatch in pre_batchs)
                    {
                        string pre_batchcode = prebatch.Trim();
                        var obj = slmdb.kkslm_ms_batch.Where(p => p.slm_BatchCode == pre_batchcode).FirstOrDefault();
                        if (obj != null)
                        {
                            if (obj.slm_Status == "1" && obj.slm_EndTime != null)
                            {   //OK, Do Nothing
                            }
                            else
                            {
                                //Prerequisite not finish yet
                                throw new Exception("Prerequisite batch (" + batch.slm_PreRequisite + ") has not finished yet");
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        //public static void SetBatchRunningDate()
        //{
        //    try
        //    {
        //        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

        //        DateTime batchRunningDate = new DateTime();
        //        string readFromConfig = ConfigurationManager.AppSettings["ReadBatchDateFromConfig"];
        //        string batch_date = ConfigurationManager.AppSettings["BatchDate"];

        //        if (readFromConfig != null && readFromConfig.ToUpper() == "Y" && batch_date != null)
        //        {
        //            string[] str = batch_date.Split('-');
        //            batchRunningDate = new DateTime(Convert.ToInt32(str[0]), Convert.ToInt32(str[1]), Convert.ToInt32(str[2]));
        //        }
        //        else
        //        {
        //            int endTimeHour = 0;

        //            var desc = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "cocEndTime").Select(p => p.slm_OptionDesc).FirstOrDefault();
        //            if (desc != null)
        //            {
        //                string[] str = desc.Split(':');
        //                if (str.Count() > 0 && str[0] != "")
        //                {
        //                    endTimeHour = Convert.ToInt32(str[0]);
        //                }
        //                else
        //                    endTimeHour = 22;
        //            }
        //            else
        //                endTimeHour = 22;

        //            DateTime now = DateTime.Now;
        //            DateTime runningDate = new DateTime();
        //            DateTime rangeStart = new DateTime(now.Year, now.Month, now.Day, endTimeHour, 0, 0);
        //            DateTime rangeEnd = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

        //            if (now >= rangeStart && now <= rangeEnd)
        //                runningDate = now;
        //            else
        //                runningDate = now.AddDays(-1);

        //            batchRunningDate = new DateTime(runningDate.Year, runningDate.Month, runningDate.Day);
        //        }

        //        var date = slmdb.kkcoc_ms_date.FirstOrDefault();
        //        if (date != null)
        //        {
        //            date.coc_BatchRunningDate = batchRunningDate;
        //        }
        //        else
        //        {
        //            kkcoc_ms_date tmp = new kkcoc_ms_date();
        //            tmp.coc_BatchRunningDate = batchRunningDate;
        //            slmdb.kkcoc_ms_date.AddObject(tmp);
        //        }

        //        slmdb.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
