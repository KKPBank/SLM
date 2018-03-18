using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Transactions;
using SLMBatch.Common;
using SLMBatch.Data;

namespace SLMBatch.Business
{
    public class OBT_PRO_32_Biz : ServiceSMSBase
    {
        protected OBT_PRO_32_DataAccess DA;
        protected string SMSTemplatePath;

        public OBT_PRO_32_Biz()
        {
            DA = new OBT_PRO_32_DataAccess();
            SMSTemplatePath = AppConstant.SMSTemplatePathPaymentDueLong;
        }

        ~OBT_PRO_32_Biz()
        {
            DA.Dispose();
        }

        public void GenerateSMS(string batchCode)
        {
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;

            try
            {
                BatchCode = batchCode;
                BatchMonitorId = BizUtil.SetStartTime(BatchCode);

                BizUtil.CheckPrerequisite(BatchCode);

                List<OBT_PRO_32_DataAccess.SMSPaymentDueMessage> dueList = DA.LoadRenewForSMS();
                totalRecord = dueList.Count;

                foreach (OBT_PRO_32_DataAccess.SMSPaymentDueMessage due in dueList)
                {
                    string smsMessage = "";
                    try
                    {
                        if (string.IsNullOrWhiteSpace(due.TelNo))
                        {
                            throw new ArgumentNullException("TelNo", "TelNo is null or empty");
                        }

                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                        {
                            using (SLMDBEntities slmdb = AppUtil.GetSlmDbEntities())
                            {
                                smsMessage = InsertPrepareSMS(slmdb, due);
                                slmdb.SaveChanges();

                                if (due.PreleadId != null)
                                {
                                    UpdatePreleadSMSFlag(slmdb, due.PreleadId.Value, true);
                                }

                                totalSuccess += 1;

                                //Debug.WriteLine($"smsMessage: {smsMessage}");
                                //Console.WriteLine("TicketId " + due.slm_TicketId + ": SUCCESS");
                            }

                            ts.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        totalFail += 1;
                        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        string errorDetail = string.Format("PreleadId={0}, TicketId={1}, ExpiredDate={2}, LicenseNo={3}, TelNo={4}, SMSMessage={5}, Error={6}",
                                                            due.PreleadId != null ? due.PreleadId.ToString() : "",
                                                            due.TicketId,
                                                            due.ExpiredDate != null ? due.ExpiredDate.Value.ToString("dd/MM/") + due.ExpiredDate.Value.Year.ToString() : "",
                                                            due.LicenseNo,
                                                            due.TelNo,
                                                            smsMessage,
                                                            message);

                        BizUtil.InsertLog(BatchMonitorId, due.TicketId, "", errorDetail);

                        if (AppConstant.OBT_PRO_32_LogSwitch != "Y")
                        {
                            errorDetail = string.Format("PreleadId={0}, TicketId={1}, Error={2}",
                                                        due.PreleadId != null ? due.PreleadId.ToString() : "",
                                                        due.TicketId,
                                                        message);
                        }

                        Util.WriteLogFile(logfilename, BatchCode, errorDetail);
                    }
                }

                BizUtil.SetEndTime(BatchCode, BatchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Util.WriteLogFile(logfilename, BatchCode, message);
                BizUtil.InsertLog(BatchMonitorId, "", "", message);

                totalSuccess = 0;
                totalFail = totalRecord;

                BizUtil.SetEndTime(BatchCode, BatchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
        }

        private string InsertPrepareSMS(SLMDBEntities slmdb, OBT_PRO_32_DataAccess.SMSPaymentDueMessage item)
        {
            try
            {
                if (string.IsNullOrEmpty(SMSTemplatePath))
                    throw new Exception($"ไม่พบ Config {nameof(SMSTemplatePath)} ใน Configuration File");

                string template = File.ReadAllText(SMSTemplatePath, Encoding.UTF8);
                string smsMessage = template.Replace("%CarLicenseNo%", string.IsNullOrWhiteSpace(item.LicenseNo) ? "" : item.LicenseNo.Trim())
                                            .Replace("%ExpiredDate%", item.ExpiredDate != null ? item.ExpiredDate.Value.ToString("dd/MM/") + item.ExpiredDate.Value.Year.ToString() : "");

                //kkslm_tr_prelead_prepare_sms sms = InitSMS(smsMessage, item.slm_TelNo, item.slm_TicketId, _dbNow, item.slm_Prelead_Id);

                var sms = new kkslm_tr_prelead_prepare_sms
                {
                    slm_ticketId = string.IsNullOrWhiteSpace(item.TicketId) ? null : item.TicketId,
                    slm_Prelead_Id = string.IsNullOrWhiteSpace(item.TicketId) ? item.PreleadId : null,
                    slm_Message = smsMessage,
                    slm_Message_CreatedBy = "SYSTEM",
                    slm_MessageStatusId = "1",
                    slm_PhoneNumber = item.TelNo.Trim(),
                    slm_QueueId = "6",
                    slm_RequestDate = DateTime.Now,
                    slm_RuleActivityId = 0,
                    slm_ExportStatus = "0",
                    slm_RefId = null,
                    slm_SendingStatusCode = null,
                    slm_SendingStatus = null,
                    slm_ErrorCode = null,
                    slm_ErrorReason = null,
                    slm_CAS_Flag = null,
                    slm_CAS_Date = null,
                    slm_FlagType = "1",
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedDate = DateTime.Now,
                    slm_UpdatedBy = "SYSTEM",
                    slm_UpdatedDate = DateTime.Now,
                    is_Deleted = 0
                };
                slmdb.kkslm_tr_prelead_prepare_sms.AddObject(sms);
                return smsMessage;
            }
            catch
            {
                throw;
            }
        }

        private void UpdatePreleadSMSFlag(SLMDBEntities slmdb, decimal preleadId, bool flag)
        {
            string sql = string.Format("UPDATE kkslm_tr_prelead SET {0} = {1} WHERE slm_Prelead_Id = {2}"
                                            , (BatchCode == "OBT_PRO_32" ? "slm_IsSMS70" : "slm_IsSMS8")
                                            , (flag ? "1" : "0")
                                            , preleadId.ToString());
            slmdb.ExecuteStoreCommand(sql);
        }
    }
}
