using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using SLMBatch.Common;
using SLMBatch.Data;

namespace SLMBatch.Business
{
    public class OBT_PRO_24_Biz : ServiceBase
    {
        #region Backup PurgeData 17/01/2017
        //public void PurgeData(string batchCode)
        //{
        //    Int64 batchMonitorId = 0;
        //    int totalRecord = 0;
        //    int totalSuccess = 0;
        //    int totalFail = 0;

        //    try
        //    {
        //        batchMonitorId = BizUtil.SetStartTime(batchCode);
        //        BizUtil.CheckPrerequisite(batchCode);

        //        DateTime purgeDate = DateTime.Today.AddDays(-AppConstant.PurgeNotificationNumOfDay);
        //        string str_purgeDate = purgeDate.Year.ToString() + purgeDate.ToString("-MM-dd");

        //        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
        //        string sql = "SELECT COUNT(slm_NotificationId) FROM " + AppConstant.SLMDBName + ".dbo.kkslm_tr_notification WHERE CONVERT(DATE, slm_CreatedDate) < '" + str_purgeDate + "'";
        //        totalRecord = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

        //        string del = "DELETE FROM " + AppConstant.SLMDBName + ".dbo.kkslm_tr_notification WHERE CONVERT(DATE, slm_CreatedDate) < '" + str_purgeDate + "'";
        //        totalSuccess = slmdb.ExecuteStoreCommand(del);

        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
        //    }
        //    catch (Exception ex)
        //    {
        //        totalFail = totalRecord;

        //        Console.WriteLine("All FAIL");
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

        //        Util.WriteLogFile(logfilename, batchCode, message);
        //        BizUtil.InsertLog(batchMonitorId, "", "", message);
        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
        //    }
        //}
        #endregion

        /// <summary>
        /// Purge Notification Data
        /// </summary>
        /// <param name="batchCode"></param>
        public void PurgeData(string batchCode)
        {
            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;
            int currentTotal = 0;
            DateTime purgeDate = new DateTime();
            string strPurgeDate = "";
            string sql = "";
            string del = "";
            string slmdbName = AppConstant.SLMDBName;
            Dictionary<int, int> totalList = new Dictionary<int, int>();

            try
            {
                batchMonitorId = BizUtil.SetStartTime(batchCode);
                BizUtil.CheckPrerequisite(batchCode);

                using (SLMDBEntities slmdb = AppUtil.GetSlmDbEntities())
                {
                    var configList = slmdb.kkslm_ms_config_purge_day.Where(p => p.slm_BatchCode == batchCode && p.is_Deleted == false).ToList();
                    var typeList = slmdb.kkslm_tr_notification.Where(p => p.slm_NotificationType != null).Select(p => p.slm_NotificationType.Value).Distinct().OrderBy(p => p).ToList();

                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        foreach (int type in typeList)
                        {
                            currentTotal = 0;

                            var numOfDay = configList.Where(p => p.slm_Type == type.ToString()).Select(p => p.slm_NumOfDay).FirstOrDefault();
                            if (numOfDay != null && numOfDay >= 0)
                            {
                                purgeDate = DateTime.Today.AddDays(-numOfDay.Value);
                                strPurgeDate = purgeDate.Year.ToString() + purgeDate.ToString("-MM-dd");
                            }
                            else
                            {
                                purgeDate = DateTime.Today.AddDays(-AppConstant.PurgeNotificationNumOfDay);
                                strPurgeDate = purgeDate.Year.ToString() + purgeDate.ToString("-MM-dd");
                            }

                            sql = "SELECT COUNT(slm_NotificationId) FROM " + slmdbName + ".dbo.kkslm_tr_notification WHERE slm_NotificationType = '" + type.ToString() + "' AND CONVERT(DATE, slm_CreatedDate) < '" + strPurgeDate + "'";
                            totalRecord += slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                            del = "DELETE FROM " + slmdbName + ".dbo.kkslm_tr_notification WHERE slm_NotificationType = '" + type.ToString() + "' AND CONVERT(DATE, slm_CreatedDate) < '" + strPurgeDate + "'";
                            currentTotal = slmdb.ExecuteStoreCommand(del);
                            totalList.Add(type, currentTotal);

                            totalSuccess += currentTotal;
                        }

                        ts.Complete();
                    }
                }

                //Summary Log
                if (totalList.Count > 0)
                {
                    string msg = "";
                    foreach (KeyValuePair<int, int> data in totalList)
                    {
                        msg += (msg != "" ? ", " : "") + "Type " + data.Key.ToString() + " (" + data.Value.ToString("#,##0") + " records)";
                    }
                    msg = "Success : " + msg;
                    Util.WriteLogFile(logfilename, batchCode, msg);
                    BizUtil.InsertLog(batchMonitorId, "", "", msg);
                }

                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                totalFail = totalRecord;
                totalSuccess = 0;

                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Util.WriteLogFile(logfilename, batchCode, message);
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
        }
    }
}
