using System;
using System.Transactions;
using SLMBatch.Common;
using SLMBatch.Data;
using System.Linq;

namespace SLMBatch.Business
{
    /// <summary>
    /// Batch reset runnning 7 digit of TicketId
    /// reset to 0000000 at 1 january
    /// </summary>
    public class SLM_PRO_05_Biz : ServiceBase
    {        
        public void ResetRunningOfTicketId(string batchCode)
        {
            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;

            try
            {
                totalRecord = 1;
                batchMonitorId = BizUtil.SetStartTime(batchCode);
                
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                var curr_year = slmdb.kkslm_ms_current_year.FirstOrDefault();
                if (curr_year != null)
                {
                    int year = curr_year.slm_year != null ? curr_year.slm_year.Value : 0;
                    if (year != 0)
                    {
                        if (DateTime.Now.Year > year)
                        {
                            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                            {
                                curr_year.slm_year = DateTime.Now.Year;
                                slmdb.SaveChanges();

                                slmdb.ExecuteStoreCommand("DBCC CHECKIDENT ('dbo.kkslm_gen_identity', RESEED, 0)");

                                ts.Complete();
                            }             
                        }

                        totalSuccess = 1;
                        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
                    }
                    else
                        throw new Exception("ไม่พบข้อมูล current year ในตาราง kkslm_ms_current_year");                  
                }
                else
                    throw new Exception("ไม่พบข้อมูล current year ในตาราง kkslm_ms_current_year");
            }
            catch (Exception ex)
            {
                totalFail = 1;
                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Util.WriteLogFile(logfilename, batchCode, message);
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
        }
    }
}
