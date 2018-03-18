using System;
using System.Collections.Generic;
using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;
using System.Linq;

namespace SLMBatch.Business
{
    public class SLM_PRO_01_Biz : ServiceBase
    {
        public void CalculateJobOnHand(string batchCode)
        {
            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;

            try
            {
                batchMonitorId = BizUtil.SetStartTime(batchCode);

                List<JobOnHandData> list = GetJobOnHand();
                totalRecord = list.Count;

                foreach (JobOnHandData data in list)
                {
                    try
                    {
                        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

                        string sql = @"UPDATE " + AppConstant.SLMDBName + @".dbo.kkslm_ms_staff
                                        SET slm_JobOnHand = '" + (data.AmountOwnerJob + data.AmountDelegateJob).ToString() + @"'
                                        WHERE slm_UserName = '" + data.Username + "'";

                        slmdb.ExecuteStoreCommand(sql);

                        totalSuccess += 1;
                        Console.WriteLine("Usrename " + data.Username + ": SUCCESS");
                    }
                    catch (Exception ex)
                    {
                        totalFail += 1;
                        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        string errorDetail = "Username=" + data.Username + ", Error=" + message;

                        BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
                        Util.WriteLogFile(logfilename, batchCode, errorDetail);

                        Console.WriteLine("Usrename " + data.Username + ": FAIL");
                    }
                }

                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Util.WriteLogFile(logfilename, batchCode, message);
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
        }

        private List<JobOnHandData> GetJobOnHand()
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

                string sql = @"SELECT staff.slm_UserName AS Username, 
	                        (
                                SELECT COUNT(*) AS NUM
                                FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
                                WHERE lead.is_Deleted = 0 AND lead.slm_AssignedFlag = '1' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Owner = staff.slm_UserName AND lead.slm_Delegate IS NULL) AS AmountOwnerJob,
                            (
                                SELECT COUNT(*) AS NUM
                                FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_lead lead
                                WHERE lead.is_Deleted = 0 AND lead.slm_Delegate_Flag = '0' AND lead.slm_Status NOT IN ('08','09','10') AND lead.slm_Delegate = staff.slm_UserName) AS AmountDelegateJob
                        FROM " + AppConstant.SLMDBName + @".dbo.kkslm_ms_staff staff
                        WHERE staff.is_Deleted = 0 ";

                return slmdb.ExecuteStoreQuery<JobOnHandData>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
