using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using SLS.Resource.Data;
using SLS.Resource;

namespace SLS.Dal.Models
{
    public class SlmNoteModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmNoteModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public string ErrorMessage { get; set; }

        public string GetDBName()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["SLMDBName"];
            }
            catch
            {
                return "SLMDB";
            }
        }

        private int? GetPositionId(string username)
        {
            SlmMsStaffModel staff = new SlmMsStaffModel(slmdb);
            return staff.GetPositionId(username);
        }

        public string InsertData(string ticketId, CustomerDetail cusDetail, ChannelInfo channelInfo, string sendEmailFlag, List<string> sendEmailPerson)
        {
            try
            {
                var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId.Equals(ticketId)).FirstOrDefault();
                if (lead != null)
                {
                    try
                    {
                        DateTime createDate = DateTime.Now;
                        if (cusDetail != null && !string.IsNullOrEmpty(cusDetail.Note))
                        {
                            kkslm_note note = new kkslm_note();
                            note.slm_TicketId = ticketId;
                            note.slm_NoteDetail = cusDetail.Note;
                            note.slm_CreateDate = createDate;

                            if (channelInfo != null)
                            {
                                if (!string.IsNullOrEmpty(channelInfo.CreateUser))
                                {
                                    note.slm_CreateBy = channelInfo.CreateUser;
                                    note.slm_CreatedBy_Position = GetPositionId(channelInfo.CreateUser);
                                    lead.slm_UpdatedBy = channelInfo.CreateUser;
                                }
                                else
                                {
                                    note.slm_CreateBy = "SYSTEM";
                                    lead.slm_UpdatedBy = "SYSTEM";
                                }
                            }
                            else
                            {
                                note.slm_CreateBy = "SYSTEM";
                                lead.slm_UpdatedBy = "SYSTEM";
                            }

                            if (!string.IsNullOrEmpty(sendEmailFlag))
                            {
                                note.slm_SendEmailFlag = sendEmailFlag.ToUpper() == "Y" ? true : false;
                                note.slm_EmailSubject = sendEmailFlag.ToUpper() == "Y" ? SetEmailSubject(ticketId, cusDetail.Subject) : null;
                                if (sendEmailFlag.ToUpper() == "Y")
                                {
                                    InsertPrepareEmail(ticketId, note.slm_EmailSubject, note.slm_NoteDetail, createDate, note.slm_CreateBy, sendEmailPerson);
                                }
                            }
                            else
                            {
                                note.slm_SendEmailFlag = false;
                            }
                                
                            slmdb.kkslm_note.AddObject(note);

                            lead.slm_NoteFlag = "1";
                            lead.slm_UpdatedDate = createDate;

                            slmdb.SaveChanges();

                            return note.slm_NoteId.ToString();
                        }
                        else
                        {
                            return "";
                        }    
                    }
                    catch (Exception ex)
                    {
                        throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
                    }
                }
                else
                {
                    throw new ServiceException(ApplicationResource.UPD_NO_RECORD_FOUND_CODE, ApplicationResource.UPD_NO_RECORD_FOUND_DESC);
                }
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        private string SetEmailSubject(string ticketId, string subject)
        {
            string defaultSubject = "SLM: Ticket: " + ticketId + (string.IsNullOrEmpty(subject) ? "" : " " + subject);
            try
            {
                var data = (from l in slmdb.kkslm_tr_lead
                            from o in slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == l.slm_Status && p.slm_OptionType == "lead status" && p.is_Deleted == 0)
                            where l.slm_ticketId == ticketId
                            select new { l.slm_ticketId, o.slm_OptionDesc, l.slm_ExternalSubStatusDesc }).FirstOrDefault();

                if (data != null)
                {
                    string str = "[" + data.slm_OptionDesc + "]" + (string.IsNullOrEmpty(data.slm_ExternalSubStatusDesc) ? "" : "-[" + data.slm_ExternalSubStatusDesc + "]");
                    return str + " " + defaultSubject;
                }
                else
                {
                    return defaultSubject;
                }
            }
            catch
            {
                return defaultSubject;
            }
        }

        private void InsertPrepareEmail(string ticketId, string emailSubject, string emailContent, DateTime createdDate, string createdBy, List<string> sendEmailPerson)
        {
            try
            {
                kkslm_prepare_email email = null;
                string emailTemplate = "";
                OwnerDelegateEmailData data = GetOwnerOrDelegateEmail(ticketId);
                if (data != null)
                {
                    if (data.Owner != null && !string.IsNullOrEmpty(data.OwnerEmail))
                    {
                        emailTemplate = GetEmailTemplate(ticketId, emailContent);
                        email = new kkslm_prepare_email();
                        email.slm_EmailAddress = data.OwnerEmail;
                        email.slm_EmailContent = emailTemplate;
                        email.slm_EmailSubject = emailSubject;
                        email.slm_EmailSender = createdBy;
                        email.slm_ticketId = ticketId;
                        email.slm_ExportStatus = "0";
                        email.is_Deleted = 0;
                        email.slm_CreatedBy = createdBy;
                        email.slm_CreatedDate = createdDate;
                        slmdb.kkslm_prepare_email.AddObject(email);
                    }
                    if (data.Delegate != null && !string.IsNullOrEmpty(data.DelegateEmail))
                    {
                        email = new kkslm_prepare_email();
                        email.slm_EmailAddress = data.DelegateEmail;
                        if (emailTemplate == "")
                        {
                            emailTemplate = GetEmailTemplate(ticketId, emailContent);
                        }                            
                        email.slm_EmailContent = emailTemplate;
                        email.slm_EmailSubject = emailSubject;
                        email.slm_EmailSender = createdBy;
                        email.slm_ticketId = ticketId;
                        email.slm_ExportStatus = "0";
                        email.is_Deleted = 0;
                        email.slm_CreatedBy = createdBy;
                        email.slm_CreatedDate = createdDate;
                        slmdb.kkslm_prepare_email.AddObject(email);
                    }
                    if (data.MarketingOwner != null && !string.IsNullOrEmpty(data.MarketingOwnerEmail))
                    {
                        email = new kkslm_prepare_email();
                        email.slm_EmailAddress = data.MarketingOwnerEmail;
                        if (emailTemplate == "")
                        {
                            emailTemplate = GetEmailTemplate(ticketId, emailContent);
                        }                           
                        email.slm_EmailContent = emailTemplate;
                        email.slm_EmailSubject = emailSubject;
                        email.slm_EmailSender = createdBy;
                        email.slm_ticketId = ticketId;
                        email.slm_ExportStatus = "0";
                        email.is_Deleted = 0;
                        email.slm_CreatedBy = createdBy;
                        email.slm_CreatedDate = createdDate;
                        slmdb.kkslm_prepare_email.AddObject(email);
                    }
                    if (data.LastOwner != null && !string.IsNullOrEmpty(data.LastOwnerEmail))
                    {
                        string flag = ConfigurationManager.AppSettings["SendEmailLastOwner"];
                        if (flag == null || flag.Trim().ToUpper() == "Y")
                        {
                            email = new kkslm_prepare_email();
                            email.slm_EmailAddress = data.LastOwnerEmail;
                            if (emailTemplate == "")
                            {
                                emailTemplate = GetEmailTemplate(ticketId, emailContent);
                            }                               
                            email.slm_EmailContent = emailTemplate;
                            email.slm_EmailSubject = emailSubject;
                            email.slm_EmailSender = createdBy;
                            email.slm_ticketId = ticketId;
                            email.slm_ExportStatus = "0";
                            email.is_Deleted = 0;
                            email.slm_CreatedBy = createdBy;
                            email.slm_CreatedDate = createdDate;
                            slmdb.kkslm_prepare_email.AddObject(email);
                        }
                    }
                }

                if (sendEmailPerson != null)
                {
                    foreach (string email_address in sendEmailPerson)
                    {
                        kkslm_prepare_email prepare_eamil = new kkslm_prepare_email();
                        prepare_eamil.slm_EmailAddress = email_address;
                        if (emailTemplate == "")
                        {
                            emailTemplate = GetEmailTemplate(ticketId, emailContent);
                        }                           
                        prepare_eamil.slm_EmailContent = emailTemplate;
                        prepare_eamil.slm_EmailSubject = emailSubject;
                        prepare_eamil.slm_EmailSender = createdBy;
                        prepare_eamil.slm_ticketId = ticketId;
                        prepare_eamil.slm_ExportStatus = "0";
                        prepare_eamil.is_Deleted = 0;
                        prepare_eamil.slm_CreatedBy = createdBy;
                        prepare_eamil.slm_CreatedDate = createdDate;
                        slmdb.kkslm_prepare_email.AddObject(prepare_eamil);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private OwnerDelegateEmailData GetOwnerOrDelegateEmail(string ticketId)
        {
            try
            {
                string SLMDBName = GetDBName();

                string sql = @"SELECT LEAD.slm_Owner AS Owner, STAFF.slm_StaffEmail AS OwnerEmail, LEAD.slm_Delegate AS Delegate, STAFF2.slm_StaffEmail AS DelegateEmail
                                , STAFF3.slm_UserName AS MarketingOwner, STAFF3.slm_StaffEmail AS MarketingOwnerEmail
                                , STAFF4.slm_UserName AS LastOwner, STAFF4.slm_StaffEmail AS LastOwnerEmail
                                FROM " + SLMDBName + @".dbo.kkslm_tr_lead LEAD
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff STAFF ON LEAD.slm_Owner = STAFF.slm_UserName AND STAFF.is_Deleted = 0
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff STAFF2 ON LEAD.slm_Delegate = STAFF2.slm_UserName AND STAFF2.is_Deleted = 0
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff STAFF3 ON LEAD.coc_MarketingOwner = STAFF3.slm_EmpCode AND STAFF3.is_Deleted = 0
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff STAFF4 ON LEAD.coc_LastOwner = STAFF4.slm_EmpCode AND STAFF4.is_Deleted = 0
                                WHERE LEAD.slm_ticketId = '" + ticketId + "' AND LEAD.is_Deleted = 0";

                return slmdb.ExecuteStoreQuery<OwnerDelegateEmailData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        private string GetEmailTemplate(string ticketId, string emailContent)
        {
            try
            {
                string template = "";
                string filePath = ConfigurationManager.AppSettings["EmailTemplatePath"];
                if (filePath == null)
                {
                    throw new ServiceException("", "ไม่พบ Config EmailTemplatePath ใน Configuration File");
                }

                string SLMDBName = GetDBName();

                string sql = @"SELECT lead.slm_ticketId AS TicketId, cam.slm_CampaignName AS CampaignName, lead.slm_ChannelId AS Channel, lead.slm_Name AS Firstname, lead.slm_LastName AS Lastname, own.slm_StaffNameTH AS OwnerName
                                , lead.slm_AssignedDate AS AssignedDate, del.slm_StaffNameTH AS DelegateName, lead.slm_DelegateDate AS DelegateDate, ISNULL(cre.slm_StaffNameTH, lead.slm_CreatedBy) AS CreatedBy, lead.slm_CreatedDate AS CreatedDate
                                , lead.slm_AvailableTime AS AvailableTime, opt.slm_OptionDesc AS StatusDesc, lead.slm_Product_Name AS ProductName, pg.product_name AS ProductGroupName, lead.slm_TelNo_1 AS TelNo1, prod.slm_LicenseNo AS LicenseNo
                                , opt2.slm_OptionDesc AS CocStatusDesc, mktowner.slm_StaffNameTH AS MarketingOwnerName, lastowner.slm_StaffNameTH AS LastOwnerName
                                , lead.slm_ExternalSubStatusDesc AS SubStatusDesc
                                FROM " + SLMDBName + @".dbo.kkslm_tr_lead lead
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign cam ON cam.slm_CampaignId = lead.slm_CampaignId
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff own ON own.slm_UserName = lead.slm_Owner
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff del ON del.slm_UserName = lead.slm_Delegate
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff cre ON cre.slm_UserName = lead.slm_CreatedBy
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_option opt ON opt.slm_OptionCode = lead.slm_Status AND opt.slm_OptionType = 'lead status'
                                LEFT JOIN " + SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP pg ON pg.product_id = lead.slm_Product_Group_Id
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_productinfo prod ON prod.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_option opt2 ON opt2.slm_OptionCode = lead.coc_Status AND ISNULL(opt2.slm_OptionSubCode, '0123456789') = ISNULL(lead.coc_SubStatus, '0123456789') AND opt2.slm_OptionType = 'coc_status'
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff mktowner ON mktowner.slm_EmpCode = lead.coc_MarketingOwner
                                LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff lastowner ON lastowner.slm_EmpCode = lead.coc_LastOwner
                                WHERE lead.slm_ticketId = '" + ticketId + "'";

                var data = slmdb.ExecuteStoreQuery<EmailTemplateData>(sql).FirstOrDefault();
                if (data != null)
                {
                    template = File.ReadAllText(filePath, Encoding.UTF8);
                    template = template.Replace("%Note%", emailContent)
                                        .Replace("%TicketId%", ticketId)
                                        .Replace("%Campaign%", data.CampaignName)
                                        .Replace("%Channel%", data.Channel)
                                        .Replace("%ProductGroupName%", data.ProductGroupName)
                                        .Replace("%ProductName%", data.ProductName)
                                        .Replace("%LeadStatus%", data.StatusDesc)
                                        .Replace("%SubStatus%", data.SubStatusDesc)
                                        .Replace("%CustomerName%", data.Firstname)
                                        .Replace("%CustomerLastName%", data.Lastname)
                                        .Replace("%OwnerName%", data.OwnerName)
                                        .Replace("%AssignedDate%", data.AssignedDate != null ? data.AssignedDate.Value.Year.ToString() + data.AssignedDate.Value.ToString("-MM-dd HH:mm:ss") : "")
                                        .Replace("%DelegateName%", data.DelegateName)
                                        .Replace("%DelegateDate%", data.DelegateDate != null ? data.DelegateDate.Value.Year.ToString() + data.DelegateDate.Value.ToString("-MM-dd HH:mm:ss") : "")
                                        .Replace("%CreatedBy%", data.CreatedBy)
                                        .Replace("%CreatedDate%", data.CreatedDate != null ? data.CreatedDate.Value.Year.ToString() + data.CreatedDate.Value.ToString("-MM-dd HH:mm:ss") : "")
                                        .Replace("%TelNo1%", data.TelNo1)
                                        .Replace("%LicenseNo%", data.LicenseNo)
                                        .Replace("%CocStatusDesc%", data.CocStatusDesc)
                                        .Replace("%MarketingOwnerName%", data.MarketingOwnerName)
                                        .Replace("%LastOwnerName%", data.LastOwnerName);

                    if (data.AvailableTime != null && data.AvailableTime.Length == 6)
                        template = template.Replace("%AvailableTime%", data.AvailableTime.Substring(0, 2) + ":" + data.AvailableTime.Substring(2, 2) + ":" + data.AvailableTime.Substring(4, 2));
                    else
                        template = template.Replace("%AvailableTime%", "");
                }

                return template != "" ? template : emailContent;
            }
            catch
            {
                throw;
            }
        }

        public void UpdateCasFlag(string ticketId, string noteId, string flag)
        {
            try
            {
                int note_Id = int.Parse(noteId);
                var note = slmdb.kkslm_note.Where(p => p.slm_NoteId == note_Id).FirstOrDefault();
                if (note != null)
                {
                    note.slm_CAS_Flag = flag;
                    note.slm_CAS_Date = DateTime.Now;
                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
