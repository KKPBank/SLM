using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Dal;
using SLS.Resource;

namespace SLS.Biz
{
    public class BatchStatus
    {
        public const string InProcess = "0";
        public const string Success = "1";
        public const string Fail = "2";
    }

    public class BatchUtil
    {
        public static string ErrorMessage { get; set; }

        public static Int64 SetStartTime(string batchCode)
        {
            try
            {
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    DateTime runningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);    //BizUtil.GetBatchRunningDate(slmdb);

                    var batch = slmdb.kkslm_ms_batch.Where(p => p.slm_BatchCode == batchCode).FirstOrDefault();
                    if (batch != null)
                    {
                        DateTime actionDate = DateTime.Now;
                        batch.slm_StartTime = actionDate;
                        batch.slm_EndTime = null;
                        batch.slm_Status = BatchStatus.InProcess;
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
                    {
                        throw new ServiceException("", "ไม่พบ BatchCode " + batchCode + " ในระบบ");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static void SetEndTime(string batchCode, Int64 batchMonitorId, string status, int totalRecord, int totalSuccess, int totalFail)
        {
            try
            {
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
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
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        public static void InsertLog(Int64 batchMonitorId, string ticketId, string empCode, string errorDetail)
        {
            try
            {
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    kkslm_tr_batchlog log = new kkslm_tr_batchlog()
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
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        public static void CheckPrerequisite(string batchCode)
        {
            try
            {
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
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
                                    throw new ServiceException("", "Prerequisite batch (" + batch.slm_PreRequisite + ") has not finished yet");
                                }
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
    }
}
