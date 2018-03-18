using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Configuration;
using System.IO;
using SLS.Dal;
using SLS.Dal.Models;
using SLS.Resource;
using SLS.Resource.Data;
using log4net;

namespace SLS.Biz
{
    public class UploadLeadBiz : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UploadLeadBiz));

        private const string generalErrorMessage = "เกิดข้อผิดพลาดกับระบบ";

        private string BatchCode { get; set; }
        private Int64 BatchMonitorId { get; set; }
        private int TotalRecord { get; set; }
        private int TotalSuccess { get; set; }
        private int TotalFail { get; set; }

        private string OwnerUsername { get; set; }
        private string DelegateUsername { get; set; }
        private string ErrorDetail { get; set; }

        public UploadLeadBiz(string batchCode)
        {
            BatchCode = batchCode;
        }

        public bool CreateLead()
        {
            try
            {
                BatchMonitorId = BatchUtil.SetStartTime(BatchCode);
                BatchUtil.CheckPrerequisite(BatchCode);

                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    var mainList = slmdb.kkslm_tmp_uploadlead.Where(p => p.slm_Status == "Submit" && p.is_Deleted == 0).ToList();
                    foreach (var item in mainList)
                    {
                        var detailList = slmdb.kkslm_tmp_uploadlead_detail.Where(p => p.slm_UploadLeadId == item.slm_UploadLeadId && p.slm_Status == "Submit" && p.is_Deleted == 0).ToList();
                        TotalRecord += detailList.Count;

                        string logDetail = string.Format("UploadLeadId: {0}, FileName: {1}, ChannelId: {2}, CampaignId: {3}, NumOfDetail: {4}", item.slm_UploadLeadId.ToString(), item.slm_FileName, item.slm_ChannelId, item.slm_CampaignId, detailList.Count.ToString());
                        log.Info(logDetail);
                        BatchUtil.InsertLog(BatchMonitorId, "", "", logDetail);

                        var ret = SaveUploadDetail(detailList, item.slm_ChannelId, item.slm_CampaignId);

                        item.slm_batch_AssignDate = DateTime.Now;
                        item.slm_Status = ret == true ? "Success" : "Fail";
                        //เอาออกเนื่องจากไปทับกับชื่อผู้อัพโหลดและวันที่อัพโหลดล่าสุด
                        //item.slm_UpdatedBy = "SYSTEM";
                        //item.slm_UpdatedDate = DateTime.Now;
                        slmdb.SaveChanges();
                    }
                }

                SendEmail();

                BatchUtil.SetEndTime(BatchCode, BatchMonitorId, BatchStatus.Success, TotalRecord, TotalSuccess, TotalFail);
                log.Info(string.Format("Result: Success, Total: {0}, Success: {1}, Fail: {2}", TotalRecord, TotalSuccess, TotalFail));

                return true;
            }
            catch(Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                log.Error(message);
                BatchUtil.InsertLog(BatchMonitorId, "", "", message);

                TotalSuccess = 0;
                TotalFail = TotalRecord;

                BatchUtil.SetEndTime(BatchCode, BatchMonitorId, BatchStatus.Fail, TotalRecord, TotalSuccess, TotalFail);
                log.Info(string.Format("Result: Fail, Total: {0}, Success: {1}, Fail: {2}", TotalRecord, TotalSuccess, TotalFail));

                return false;
            }
        }

        private bool SaveUploadDetail(List<kkslm_tmp_uploadlead_detail> detailList, string channelId, string campaignId)
        {
            try
            {
                bool ret = false;
                int insertFailCount = 0;
                List<UploadLeadData> existingList = new List<UploadLeadData>();

                foreach (var detail in detailList)
                {
                    ret = false;
                    OwnerUsername = "";
                    DelegateUsername = "";

                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        if (ValidateData(detail, existingList, campaignId))
                        {
                            ret = InsertLead(detail, channelId, campaignId, OwnerUsername, DelegateUsername);
                            if (!ret)
                            {
                                //Update temp fail
                                insertFailCount += 1;
                                TotalFail += 1;
                                UpdateTempFail(detail.slm_UploadDetailId, ErrorDetail);
                            }
                            else
                            {
                                TotalSuccess += 1;
                            }
                        }
                        else
                        {
                            //Update temp fail
                            insertFailCount += 1;
                            TotalFail += 1;
                            UpdateTempFail(detail.slm_UploadDetailId, ErrorDetail);
                        }

                        ts.Complete();
                    }
                }

                return insertFailCount == 0 ? true : false;
            }
            catch(Exception ex)
            {
                TotalFail += detailList.Count;
                ErrorDetail = "FailAll";
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                log.Error(message);
                BatchUtil.InsertLog(BatchMonitorId, "", "", "FailAll Detail, " + message);
                return false;
            }
        }

        private bool ValidateData( kkslm_tmp_uploadlead_detail detail, List<UploadLeadData> existingList, string campaignId)
        {
            try
            {
                int count = existingList.Count(p => p.Firstname == detail.slm_Name && p.Lastname == detail.slm_LastName && p.TelNo1 == detail.slm_TelNo_1);
                if (count == 0)
                {
                    using (var slmdb = DBUtil.GetSlmDbEntities())
                    {
                        //Check Campaign
                        var campaign_data = LeadServiceBiz.GetGampaign(campaignId);
                        if (campaign_data == null)
                        {
                            ErrorDetail = "หา Campaign ไม่พบ";
                            InsertLog(string.Format("UploadDetailId: {0}, Error: หา Campaign ไม่พบ, CampaignId: {1}", detail.slm_UploadDetailId.ToString(), campaignId));
                            return false;
                        }
                        else
                        {
                            if (campaign_data.Status.ToUpperInvariant().Trim() == "X")
                            {
                                ErrorDetail = "Campaign หมดอายุ";
                                InsertLog(string.Format("UploadDetailId: {0}, Error: Campaign หมดอายุ, CampaignId: {1}", detail.slm_UploadDetailId.ToString(), campaignId));
                                return false;
                            }
                            if (campaign_data.Status.ToUpperInvariant().Trim() != "A")
                            {
                                ErrorDetail = "Campaign ยังไม่ได้รับการอนุมัติ";
                                InsertLog(string.Format("UploadDetailId: {0}, Error: Campaign ยังไม่ได้รับการอนุมัติ, CampaignId: {1}", detail.slm_UploadDetailId.ToString(), campaignId));
                                return false;
                            }
                        }

                        //Check Owner
                        string owner_username = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == detail.slm_OwnerId && p.is_Deleted == 0).Select(p => p.slm_UserName).FirstOrDefault();
                        if (string.IsNullOrEmpty(owner_username))
                        {
                            ErrorDetail = "ไม่พบ Owner ในระบบ";
                            InsertLog(string.Format("UploadDetailId: {0}, Error: ไม่พบ Owner ในระบบ, EmpCode: {1}", detail.slm_UploadDetailId.ToString(), detail.slm_OwnerId));
                            return false;
                        }
                        else
                        {
                            //Check Access Right
                            if (!AccessRightBiz.CheckAccessRight(campaignId, owner_username))
                            {
                                ErrorDetail = "Owner ไม่มีสิทธิใน Campaign";
                                InsertLog(string.Format("UploadDetailId: {0}, Error: Owner ไม่มีสิทธิใน Campaign, EmpCode: {1}, CampaignId: {2}", detail.slm_UploadDetailId.ToString(), detail.slm_OwnerId, campaignId));
                                return false;
                            }
                        }

                        OwnerUsername = owner_username;

                        string del_username = "";
                        if (!string.IsNullOrEmpty(detail.slm_DelegateId))
                        {
                            del_username = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == detail.slm_DelegateId && p.is_Deleted == 0).Select(p => p.slm_UserName).FirstOrDefault();
                            if (string.IsNullOrEmpty(del_username))
                            {
                                ErrorDetail = "ไม่พบ Delegate ในระบบ";
                                InsertLog(string.Format("UploadDetailId: {0}, Error: ไม่พบ Delegate ในระบบ, EmpCode: {1}", detail.slm_UploadDetailId.ToString(), detail.slm_DelegateId));
                                return false;
                            }
                            else
                            {
                                //Check Access Right
                                if (!AccessRightBiz.CheckAccessRight(campaignId, del_username))
                                {
                                    ErrorDetail = "Delegate ไม่มีสิทธิใน Campaign";
                                    InsertLog(string.Format("UploadDetailId: {0}, Error: Delegate ไม่มีสิทธิใน Campaign, EmpCode: {1}, CampaignId: {2}", detail.slm_UploadDetailId, detail.slm_DelegateId, campaignId));
                                    return false;
                                }
                            }

                            DelegateUsername = del_username;
                        }

                        existingList.Add(new UploadLeadData()
                        {
                            UploadDetailId = detail.slm_UploadDetailId,
                            Firstname = detail.slm_Name,
                            Lastname = detail.slm_LastName,
                            CardTypeId = detail.slm_CardTypeId,
                            CitizenId = detail.slm_CitizenId,
                            OwnerUsername = owner_username,
                            DelegateUsername = del_username,
                            TelNo1 = detail.slm_TelNo_1,
                            TelNo2 = detail.slm_TelNo_2,
                            Detail = detail.slm_Detail
                        });
                    }

                    return true;
                }
                else
                {
                    ErrorDetail = "Duplicated Lead";
                    InsertLog(string.Format("UploadDetailId: {0}, Error: Duplicated Lead, Name: {1}, Lastname: {2}, TelNo1: {3}", detail.slm_UploadDetailId.ToString(), detail.slm_Name, detail.slm_LastName, detail.slm_TelNo_1));
                    return false;
                }
            }
            catch(Exception ex)
            {
                ErrorDetail = generalErrorMessage;
                string message = string.Format("Validate Data Fail: {0}", (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                log.Error(message);
                BatchUtil.InsertLog(BatchMonitorId, "", "", ErrorDetail + ", " + message);
                return false;
            }
        }

        private void InsertLog(string logMessage)
        {
            BatchUtil.InsertLog(BatchMonitorId, "", "", logMessage);
            log.Error(logMessage);
        }

        private bool InsertLead(kkslm_tmp_uploadlead_detail detail, string channelId, string campaignId, string ownerUsername, string delegateUsername)
        {
            string ticketId = "";
            try
            {
                Mandatory mandatory = new Mandatory()
                {
                    Firstname = detail.slm_Name,
                    Campaign = campaignId,
                    TelNo1 = detail.slm_TelNo_1,
                    AutoRuleAssign = true
                };
                CustomerInfo cusInfo = new CustomerInfo()
                {
                    Lastname = detail.slm_LastName,
                    CardType = detail.slm_CardTypeId != null ? detail.slm_CardTypeId.ToString() : null,
                    Cid = detail.slm_CitizenId,
                    TelNo2 = detail.slm_TelNo_2,
                };
                CustomerDetail cusDetail = new CustomerDetail()
                {
                    TelesaleName = ownerUsername,
                    DelegateUsername = delegateUsername,
                    Detail = detail.slm_Detail
                };

                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    StoreProcedure store = new StoreProcedure(slmdb);
                    ticketId = store.GenerateTicketId();

                    var lead = new SlmTrLeadModel(slmdb);
                    lead.InsertData(ticketId, mandatory, cusInfo, cusDetail, null, channelId, null, "", null);

                    var customerInfo = new SlmTrCusInfoModel(slmdb);
                    customerInfo.InsertData(ticketId, cusInfo, cusDetail, null);

                    var productInfo = new SlmTrProductInfoModel(slmdb);
                    productInfo.InsertData(ticketId, null);

                    var channelInfo = new SlmTrChannelInfoModel(slmdb);
                    channelInfo.InsertData(ticketId, null, channelId);

                    var campaignFinal = new SlmTrCampaignFinalModel(slmdb);
                    campaignFinal.InsertData(ticketId, mandatory, null);

                    var upload = new UploadLeadModel(slmdb);
                    upload.UpdateTempSuccess(detail.slm_UploadDetailId, ticketId);
                }

                return true;
            }
            catch(Exception ex)
            {
                ErrorDetail = generalErrorMessage;

                string message = string.Format("Insert Lead Fail: {0}", (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                log.Error(message);
                BatchUtil.InsertLog(BatchMonitorId, "", "", ErrorDetail + ", " + message);

                return false;
            }
        }

        private bool UpdateTempFail(int uploadDetailId, string errorDetail)
        {
            try
            {
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    var upload = new UploadLeadModel(slmdb);
                    upload.UpdateTempFail(uploadDetailId, errorDetail);
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorDetail = generalErrorMessage;
                log.Error(string.Format("Update Temp Fail: {0}", (ex.InnerException != null ? ex.InnerException.Message : ex.Message)));
                return false;
            }
        }

        private bool SendEmail()
        {
            try
            {
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    UploadLeadModel biz = new UploadLeadModel(slmdb);
                    var mainList = biz.GetSendMailList();

                    var assignDateList = mainList.Select(p => p.AssignDate).Distinct().ToList();

                    foreach (DateTime? assignDate in assignDateList)
                    {
                        var listByAssignDate = mainList.Where(p => p.AssignDate == assignDate).ToList();
                        InsertPrepareEmail(assignDate, listByAssignDate);
                    }
                }

                return true;    
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                log.Error(message);
                BatchUtil.InsertLog(BatchMonitorId, "", "", message);
                return false;
            }
        }

        private void InsertPrepareEmail(DateTime? assignDate, List<UploadLeadSummaryJob> listByAssignDate)
        {
            try
            {
                kkslm_prepare_email email = null;
                string emailTemplate = "";
                string emailTo = "";
                var empcodeList = listByAssignDate.Select(p => p.OwnerEmpCode).Distinct().ToList();

                foreach (var empcode in empcodeList)
                {
                    emailTo = "";
                    emailTemplate = "";
                    var jobsByEmpCode = listByAssignDate.Where(p => p.OwnerEmpCode == empcode).ToList();
                    var detailIdlist = jobsByEmpCode.Select(p => p.UploadDetailId).ToList();
                    List<kkslm_tmp_uploadlead_detail> updateList = null;

                    try
                    {
                        using (var slmdb = DBUtil.GetSlmDbEntities())
                        {
                            emailTo = GetStaffEmail(slmdb, empcode);
                            emailTemplate = GetEmailTemplate(jobsByEmpCode);

                            if (!string.IsNullOrEmpty(emailTo) && !string.IsNullOrEmpty(emailTemplate))
                            {
                                emailTemplate = GetEmailTemplate(jobsByEmpCode);
                                email = new kkslm_prepare_email()
                                {
                                    slm_EmailAddress = emailTo,
                                    slm_EmailContent = emailTemplate,
                                    slm_EmailSubject = GetEmailSubject(assignDate, jobsByEmpCode),
                                    slm_EmailSender = "SYSTEM",
                                    slm_Email_CreatedBy = "SYSTEM",
                                    slm_ticketId = "999999999999",
                                    slm_ExportStatus = "0",
                                    is_Deleted = 0,
                                    slm_CreatedBy = "SYSTEM",
                                    slm_CreatedDate = DateTime.Now,
                                    slm_UpdatedBy = "SYSTEM",
                                    slm_UpdatedDate = DateTime.Now
                                };
                                slmdb.kkslm_prepare_email.AddObject(email);

                                updateList = slmdb.kkslm_tmp_uploadlead_detail.Where(p => detailIdlist.Contains(p.slm_UploadDetailId) == true).ToList();
                                updateList.ForEach(p =>
                                {
                                    p.is_SendEmail = 1;
                                });
                            }
                            else
                            {
                                updateList = slmdb.kkslm_tmp_uploadlead_detail.Where(p => detailIdlist.Contains(p.slm_UploadDetailId) == true).ToList();
                                updateList.ForEach(p =>
                                {
                                    p.is_SendEmail = 2;
                                });

                                string assignDateStr = assignDate != null ? (assignDate.Value.ToString("dd/MM/") + assignDate.Value.Year.ToString()) : "";
                                string message = string.Format("Method InsertPrepareEmail Fail, AssignDate: {0} EmpCode: {1}, Error: {2}", assignDateStr, empcode, ErrorDetail);
                                BatchUtil.InsertLog(BatchMonitorId, "", "", message);
                            }

                            slmdb.SaveChanges();
                        }
                    }
                    catch(Exception ex)
                    {
                        try
                        {
                            using (var slmdb = DBUtil.GetSlmDbEntities())
                            {
                                updateList = slmdb.kkslm_tmp_uploadlead_detail.Where(p => detailIdlist.Contains(p.slm_UploadDetailId) == true).ToList();
                                updateList.ForEach(p =>
                                {
                                    p.is_SendEmail = 2;
                                });
                                slmdb.SaveChanges();
                            }
                        }
                        catch(Exception exinner)
                        {
                            log.Error(string.Format("Update Send Emai Flag(2) Fail: {0}", (exinner.InnerException != null ? exinner.InnerException.Message : exinner.Message)));
                        }

                        string assignDateStr = assignDate != null ? (assignDate.Value.ToString("dd/MM/") + assignDate.Value.Year.ToString()) : "";
                        string message = string.Format("Method InsertPrepareEmail Fail, AssignDate={0} EmpCode={1}, Error={2}", assignDateStr, empcode, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                        log.Error(message);
                        BatchUtil.InsertLog(BatchMonitorId, "", "", message);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private string GetStaffEmail(SLM_DBEntities slmdb, string empcode)
        {
            var email = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == empcode).Select(p => p.slm_StaffEmail).FirstOrDefault();
            if (!string.IsNullOrEmpty(email))
            {
                return email.Trim();
            }
            else
            {
                ErrorDetail = "Staff email not found";
                return string.Empty;
            }
        }

        private string GetEmailSubject(DateTime? assignDate, List<UploadLeadSummaryJob> jobsByEmpCode)
        {
            try
            {
                string assignDateStr = assignDate != null ? (assignDate.Value.ToString("dd/MM/") + assignDate.Value.Year.ToString()) : "";
                string subject = "SLM: มีงานที่ได้มอบหมายให้คุณดำเนินการจำนวน %TotalJob% งาน เมื่อวันที่ %AssignDate%";
                return subject.Replace("%TotalJob%", jobsByEmpCode.Count.ToString("#,##0")).Replace("%AssignDate%", assignDateStr);
            }
            catch
            {
                throw;
            }
        }

        private string GetEmailTemplate(List<UploadLeadSummaryJob> jobsByEmpCode)
        {
            try
            {
                string template = "";
                string filePath = ConfigurationManager.AppSettings["EmailTemplateAutoAssignPath"];
                if (filePath == null)
                {
                    ErrorDetail = "ไม่พบ Config EmailTemplateAutoAssignPath ใน Configuration File";
                    return string.Empty;
                }

                var assignDateTime = jobsByEmpCode.Min(p => p.AssignDateTime);
                var groupList = jobsByEmpCode.GroupBy(p => p.CampaignName).Select(p => new { CampaignName = p.Key, Count = p.Count() }).OrderBy(p => p.CampaignName).ToList();

                string campaignSummary = "";
                groupList.ForEach(p => {
                    campaignSummary += string.Format("{0}:     {1} {2}{3}", p.CampaignName, p.Count.ToString("#,##0"), "Job", Environment.NewLine);
                });

                //jobsByEmpCode = jobsByEmpCode.OrderBy(p => p.CampaignName).ToList();
                //jobsByEmpCode.ForEach(p => {
                //    campaignSummary += string.Format("{0}\t\t\t\t{1} {2}{3}", p.CampaignName, p.Count.ToString("#,##0"), "Job", Environment.NewLine);
                //});

                template = File.ReadAllText(filePath, Encoding.UTF8);
                template = template.Replace("%TotalJob%", groupList.Sum(p => p.Count).ToString("#,##0"))
                                    .Replace("%AssignDateTime%", assignDateTime != null ? (assignDateTime.Value.ToString("dd/MM/") + assignDateTime.Value.Year.ToString() + " " + assignDateTime.Value.ToString("HH:mm:ss")) : "")
                                    .Replace("%CampaignSummary%", campaignSummary);

                return template;
            }
            catch
            {
                throw;
            }
        }

        #region IDisposable

        bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
            }

            _disposed = true;
        }

        #endregion
    }

}
