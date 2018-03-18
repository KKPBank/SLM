using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SLM.Resource;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmPreleadNoteModel
    {
        public string InsertPreleadNoteHistory(decimal preleadId, bool sendEmail, string emailSubject, string noteDetail, List<string> emailList, string createBy)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    kkslm_tr_prelead_prepare_email email = null;
                    string emailTemplate = "";
                    DateTime createDate = DateTime.Now;
                    kkslm_tr_prelead_note note = new kkslm_tr_prelead_note();
                    note.slm_Prelead_Id = preleadId;
                    note.slm_NoteDetail = noteDetail;
                    note.slm_CreatedBy = createBy;
                    note.slm_CreatedBy_Position = GetPositionId(createBy);
                    note.slm_CreatedDate = createDate;
                    note.slm_SendEmailFlag = sendEmail;

                    if (sendEmail)
                    {
                        emailSubject = SetEmailSubject(slmdb, preleadId, emailSubject);
                        note.slm_EmailSubject = emailSubject;
                        OwnerDelegateEmailData data = GetOwnerEmail(preleadId, slmdb);
                        if (data != null)
                        {
                            if (!string.IsNullOrEmpty(data.Owner) && !string.IsNullOrEmpty(data.OwnerEmail))
                            {
                                emailTemplate = GetEmailTemplate(preleadId, noteDetail, slmdb);
                                email = new kkslm_tr_prelead_prepare_email();
                                email.slm_EmailAddress = data.OwnerEmail;
                                email.slm_EmailContent = emailTemplate;
                                email.slm_EmailSubject = emailSubject;
                                email.slm_EmailSender = createBy;
                                email.slm_Prelead_Id = preleadId;
                                email.slm_ExportStatus = "0";
                                email.is_Deleted = false;
                                email.slm_CreatedBy = createBy;
                                email.slm_CreatedDate = createDate;
                                slmdb.kkslm_tr_prelead_prepare_email.AddObject(email);
                            }
                        }

                        foreach (string email_address in emailList)
                        {
                            kkslm_tr_prelead_prepare_email prepare_eamil = new kkslm_tr_prelead_prepare_email();
                            prepare_eamil.slm_EmailAddress = email_address;
                            if (emailTemplate == "")
                                emailTemplate = GetEmailTemplate(preleadId, noteDetail, slmdb);
                            prepare_eamil.slm_EmailContent = emailTemplate;
                            prepare_eamil.slm_EmailSubject = emailSubject;
                            prepare_eamil.slm_EmailSender = createBy;
                            prepare_eamil.slm_Prelead_Id = preleadId;
                            prepare_eamil.slm_ExportStatus = "0";
                            prepare_eamil.is_Deleted = false;
                            prepare_eamil.slm_CreatedBy = createBy;
                            prepare_eamil.slm_CreatedDate = createDate;
                            slmdb.kkslm_tr_prelead_prepare_email.AddObject(prepare_eamil);
                        }
                    }

                    slmdb.kkslm_tr_prelead_note.AddObject(note);
                    slmdb.SaveChanges();
                    return note.slm_NoteId.ToString();
                }
            }
            catch
            {
                throw;
            }
        }

        private string SetEmailSubject(SLM_DBEntities slmdb, decimal preleadId, string subject)
        {
            try
            {
                var data = (from l in slmdb.kkslm_tr_prelead
                            from o in slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == l.slm_Status && p.slm_OptionType == "lead status" && p.is_Deleted == 0)
                            where l.slm_Prelead_Id == preleadId
                            select new { l.slm_Prelead_Id, l.slm_Status, l.slm_SubStatus, o.slm_OptionDesc, l.slm_Product_Id, l.slm_CampaignId }).FirstOrDefault();

                if (data != null)
                {
                    var list = new KKSlmMsConfigProductSubstatusModel().GetSubStatusList(data.slm_Product_Id, data.slm_CampaignId, data.slm_Status);
                    string subStatusName = list.Where(p => p.ValueField == data.slm_SubStatus).Select(p => p.TextField).FirstOrDefault();

                    string str = "[" + data.slm_OptionDesc + "]" + (string.IsNullOrEmpty(subStatusName) ? "" : "-[" + subStatusName + "]");
                    return str + " " + subject;
                }
                else
                {
                    return subject;
                }
            }
            catch
            {
                return subject;
            }
        }

        public OwnerDelegateEmailData GetOwnerEmail(decimal preleadId, SLM_DBEntities slmdb)
        {
            try
            {
                string sql = @"SELECT prelead.slm_Owner AS Owner, staff.slm_StaffEmail AS OwnerEmail
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead prelead
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff ON prelead.slm_Owner = staff.slm_EmpCode AND staff.is_Deleted = 0
                                WHERE prelead.slm_Prelead_Id = '" + preleadId.ToString() + "'";

                return slmdb.ExecuteStoreQuery<OwnerDelegateEmailData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        private string GetEmailTemplate(decimal preleadId, string noteDetail, SLM_DBEntities slmdb)
        {
            try
            {
                string template = "";
                string filePath = System.Configuration.ConfigurationManager.AppSettings["EmailTemplatePathOBT"];
                if (filePath == null) { throw new Exception("ไม่พบ Config EmailTemplatePathOBT ใน Configuration File"); }

                string sql = @"SELECT pre.slm_Contract_Number AS ContractNo, '' AS TicketId, '' AS Channel, cam.slm_CampaignName AS CampaignName, mainstatus.slm_OptionDesc AS StatusDesc
                                , pre.slm_Name AS Firstname, pre.slm_LastName AS Lastname, phone.slm_Mobile_Phone AS TelNo1, pg.product_name AS ProductGroupName, mp.sub_product_name AS ProductName
                                , own.slm_StaffNameTH AS OwnerName, pre.slm_AssignDate AS AssignedDate, ISNULL(cre.slm_StaffNameTH, pre.slm_CreatedBy) AS CreatedBy, pre.slm_CreatedDate AS CreatedDate 
                                , pre.slm_NextContactDate AS NextContactDate, pre.slm_Product_Id AS ProductId, pre.slm_CampaignId AS CampaignId, pre.slm_Status AS StatusCode, pre.slm_SubStatus AS SubStatusCode
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead pre
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam ON cam.slm_CampaignId = pre.slm_CampaignId
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option mainstatus ON mainstatus.slm_OptionCode = pre.slm_Status AND mainstatus.slm_OptionType = 'lead status' 
                                LEFT JOIN
                                (
	                                select top 1 pa.slm_Prelead_Id, pa.slm_Mobile_Phone
	                                from " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_address pa 
	                                where slm_Address_Type = 'C' and slm_Prelead_Id = '" + preleadId.ToString() + @"') as phone on phone.slm_Prelead_Id = pre.slm_Prelead_Id 
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT cp ON cp.PR_CampaignId = pre.slm_CampaignId
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP pg ON pg.product_id = cp.PR_ProductGroupId
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT mp ON mp.sub_product_id = cp.PR_ProductId 
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff own ON own.slm_EmpCode = pre.slm_Owner
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff cre ON cre.slm_UserName = pre.slm_CreatedBy
                                WHERE pre.slm_Prelead_Id = '" + preleadId.ToString() + "' ";

                var data = slmdb.ExecuteStoreQuery<EmailTemplateOBTData>(sql).FirstOrDefault();
                if (data != null)
                {
                    var substatusList = new KKSlmMsConfigProductSubstatusModel().GetSubStatusList(data.ProductId, data.CampaignId, data.StatusCode);
                    string substatusName = substatusList.Where(p => p.ValueField == data.SubStatusCode).Select(p => p.TextField).FirstOrDefault();

                    template = File.ReadAllText(filePath, Encoding.UTF8);
                    template = template.Replace("%Note%", noteDetail)
                                        .Replace("%ContractNo%", data.ContractNo)
                                        .Replace("%TicketId%", data.TicketId)
                                        .Replace("%Campaign%", data.CampaignName)
                                        .Replace("%Channel%", data.Channel)
                                        .Replace("%Status%", data.StatusDesc)
                                        .Replace("%SubStatus%", substatusName != null ? substatusName : "")
                                        .Replace("%CustomerName%", data.Firstname)
                                        .Replace("%CustomerLastName%", data.Lastname)
                                        .Replace("%TelNo1%", data.TelNo1)
                                        .Replace("%ProductGroupName%", data.ProductGroupName)
                                        .Replace("%ProductName%", data.ProductName)
                                        .Replace("%OwnerName%", data.OwnerName)
                                        .Replace("%AssignedDate%", data.AssignedDate != null ? data.AssignedDate.Value.Year.ToString() + data.AssignedDate.Value.ToString("-MM-dd HH:mm:ss") : "")
                                        .Replace("%CreatedBy%", data.CreatedBy)
                                        .Replace("%CreatedDate%", data.CreatedDate != null ? data.CreatedDate.Value.Year.ToString() + data.CreatedDate.Value.ToString("-MM-dd HH:mm:ss") : "")
                                        .Replace("%NextContactDate%", data.NextContactDate != null ? data.NextContactDate.Value.Year.ToString() + data.NextContactDate.Value.ToString("-MM-dd") : "");
                }

                return template != "" ? template : noteDetail;
            }
            catch
            {
                throw;
            }
        }

        private int? GetPositionId(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetPositionId(username);
        }
    }
}
