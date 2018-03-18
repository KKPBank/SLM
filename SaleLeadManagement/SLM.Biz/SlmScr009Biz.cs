using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;
using SLM.Dal;
using SLM.Resource;

namespace SLM.Biz
{
    public class SlmScr009Biz
    {
        public static List<NoteHistoryData> SearchNoteHistory(string ticketId, string preleadId)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchNoteHistory(ticketId, preleadId);
        }

        public static string InsertNoteHistory(string ticketId, bool sendEmail, string emailSubject, string noteDetail, List<string> emailList, string createBy)
        {
            KKSlmNoteModel note = new KKSlmNoteModel();
            return note.InsertNoteHistory(ticketId, sendEmail, emailSubject, noteDetail, emailList, createBy);
        }

        public static void ChangeNoteFlag(string ticketId, bool noteFlag, string updateBy)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            lead.ChangeNoteFlag(ticketId, noteFlag, updateBy);
        }

        public static bool HasOwnerOrDelegate(string ticketId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.HasOwnerOrDelegate(ticketId);
        }

        public static string InsertPreleadNoteHistory(decimal preleadId, bool sendEmail, string emailSubject, string noteDetail, List<string> emailList, string createBy)
        {
            KKSlmPreleadNoteModel note = new KKSlmPreleadNoteModel();
            return note.InsertPreleadNoteHistory(preleadId, sendEmail, emailSubject, noteDetail, emailList, createBy);
        }


        public void UpdateCasFlag(string ticketId, string preleadId, string noteId, string flag)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    if (!string.IsNullOrEmpty(ticketId))
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
                    else if (!string.IsNullOrEmpty(preleadId))
                    {
                        decimal note_Id = decimal.Parse(noteId);
                        var note = slmdb.kkslm_tr_prelead_note.Where(p => p.slm_NoteId == note_Id).FirstOrDefault();
                        if (note != null)
                        {
                            note.slm_CAS_Flag = flag;
                            note.slm_CAS_Date = DateTime.Now;
                            slmdb.SaveChanges();
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static NoteTabData GetDataForNoteTab(string ticketId)
        {
            try
            {
                string sql = @"SELECT lead.slm_ticketId AS TicketId,lead.slm_Name AS Name, lead.slm_LastName AS LastName, campaign.slm_CampaignName AS CampaignName
                                ,lead.slm_TelNo_1 AS TelNo_1,lead.slm_Ext_1 AS Ext_1, prelead.slm_Prelead_Id AS PreleadId, CONVERT(VARCHAR,lead.coc_IsCOC) AS ISCOC, lead.coc_CurrentTeam AS COCCurrentTeam 
                                ,CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
                                 ELSE posowner.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS OwnerName
                                , lead.slm_NoteFlag AS NoteFlag, renew.slm_ContractNo AS ContractNo
                                FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) 
                                LEFT JOIN dbo.kkslm_tr_renewinsurance renew WITH (NOLOCK) ON renew.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN dbo.kkslm_tr_prelead prelead WITH (NOLOCK) on lead.slm_ticketId = prelead.slm_TicketId
                                LEFT JOIN dbo.kkslm_ms_campaign campaign WITH (NOLOCK) ON lead.slm_CampaignId = campaign.slm_CampaignId
                                LEFT JOIN dbo.kkslm_ms_staff staff WITH (NOLOCK) ON lead.slm_Owner = staff.slm_UserName and staff.is_Deleted = 0
                                LEFT JOIN dbo.kkslm_ms_position posowner WITH (NOLOCK) ON lead.slm_Owner_Position = posowner.slm_Position_id
                                WHERE lead.slm_ticketId = '" + ticketId + "' ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<NoteTabData>(sql).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
