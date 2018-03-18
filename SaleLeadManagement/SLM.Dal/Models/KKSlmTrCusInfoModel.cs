using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmTrCusInfoModel
    {

        public void InsertData(LeadData leadData, string createByUsername, DateTime createDate)
        {
            try
            {
                kkslm_tr_cusinfo info = new kkslm_tr_cusinfo();
                info.slm_TicketId = leadData.TicketId;
                info.slm_LastName = leadData.LastName;
                info.slm_Email = leadData.Email;
                info.slm_TelNoSms = leadData.TelNo_1;
                info.slm_TelNo_2 = leadData.TelNo_2;
                info.slm_TelNo_3 = leadData.TelNo_3;
                info.slm_Ext_2 = leadData.Ext_2;
                info.slm_Ext_3 = leadData.Ext_3;
                info.slm_BuildingName = leadData.BuildingName;
                info.slm_AddressNo = leadData.AddressNo;
                info.slm_Floor = leadData.Floor;
                info.slm_Soi = leadData.Soi;
                info.slm_Street = leadData.Street;
                info.slm_Tambon = leadData.Tambon;
                info.slm_Amphur = leadData.Amphur;
                info.slm_Province = leadData.Province;
                info.slm_PostalCode = leadData.PostalCode;
                info.slm_country_id = leadData.CountryId;
                info.slm_Occupation = leadData.Occupation;
                info.slm_BaseSalary = leadData.BaseSalary;
                info.slm_IsCustomer = leadData.IsCustomer;
                info.slm_CusCode = leadData.CusCode;
                if (leadData.Birthdate != null) info.slm_Birthdate = leadData.Birthdate;
                info.slm_CardType = leadData.CardType;
                info.slm_CitizenId = leadData.CitizenId;
                info.slm_Topic = leadData.Topic;
                info.slm_Detail = leadData.Detail;
                info.slm_PathLink = leadData.PathLink;
                info.slm_ContactBranch = leadData.ContactBranch;
                info.slm_CreatedBy = createByUsername;
                info.slm_CreatedDate = createDate;
                info.slm_UpdatedBy = createByUsername;
                info.slm_UpdatedDate = createDate;
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    slmdb.kkslm_tr_cusinfo.AddObject(info);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateData(LeadData leadData, string updateByUsername, DateTime updateDate)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var info = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId.Equals(leadData.TicketId)).FirstOrDefault();
                    if (info != null)
                    {
                        string oldCitizenId = info.slm_CitizenId;
                        info.slm_LastName = leadData.LastName;
                        info.slm_Email = leadData.Email;
                        info.slm_TelNo_2 = leadData.TelNo_2;
                        info.slm_TelNo_3 = leadData.TelNo_3;
                        info.slm_Ext_2 = leadData.Ext_2;
                        info.slm_Ext_3 = leadData.Ext_3;
                        info.slm_BuildingName = leadData.BuildingName;
                        info.slm_AddressNo = leadData.AddressNo;
                        info.slm_Floor = leadData.Floor;
                        info.slm_Soi = leadData.Soi;
                        info.slm_Street = leadData.Street;
                        info.slm_Tambon = leadData.Tambon;
                        info.slm_Amphur = leadData.Amphur;
                        info.slm_Province = leadData.Province;
                        info.slm_PostalCode = leadData.PostalCode;
                        info.slm_country_id = leadData.CountryId;
                        info.slm_Occupation = leadData.Occupation;
                        info.slm_BaseSalary = leadData.BaseSalary;
                        info.slm_IsCustomer = leadData.IsCustomer;
                        info.slm_CusCode = leadData.CusCode;
                        info.slm_Birthdate = leadData.Birthdate;
                        info.slm_CardType = leadData.CardType;
                        info.slm_CitizenId = string.IsNullOrEmpty(leadData.CitizenId) ? null : leadData.CitizenId;
                        info.slm_Topic = leadData.Topic;
                        info.slm_Detail = leadData.Detail;
                        info.slm_PathLink = leadData.PathLink;
                        info.slm_ContactBranch = leadData.ContactBranch;
                        info.slm_UpdatedBy = updateByUsername;
                        info.slm_UpdatedDate = updateDate;

                        if (oldCitizenId != leadData.CitizenId)
                            KKSlmTrHistoryModel.InsertHistory(slmdb, leadData.TicketId, SLMConstant.HistoryTypeCode.UpdateCardId, oldCitizenId, leadData.CitizenId, updateByUsername, updateDate);

                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertData(SLM_DBEntities slmdb, string ticketId, string new_ticketId, string username, DateTime createdDate)
        {
            try
            {
                var cusinfo = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId == ticketId).FirstOrDefault();
                if (cusinfo != null)
                {
                    kkslm_tr_cusinfo new_cusinfo = new kkslm_tr_cusinfo()
                    {
                        slm_TicketId = new_ticketId,
                        slm_LastName = cusinfo.slm_LastName,
                        slm_Email = cusinfo.slm_Email,
                        slm_TelNo_2 = cusinfo.slm_TelNo_2,
                        slm_TelNo_3 = cusinfo.slm_TelNo_3,
                        slm_Ext_2 = cusinfo.slm_Ext_2,
                        slm_Ext_3 = cusinfo.slm_Ext_3,
                        slm_BuildingName = cusinfo.slm_BuildingName,
                        slm_AddressNo = cusinfo.slm_AddressNo,
                        slm_Floor = cusinfo.slm_Floor,
                        slm_Soi = cusinfo.slm_Soi,
                        slm_Street = cusinfo.slm_Street,
                        slm_Tambon = cusinfo.slm_Tambon,
                        slm_Amphur = cusinfo.slm_Amphur,
                        slm_Province = cusinfo.slm_Province,
                        slm_PostalCode = cusinfo.slm_PostalCode,
                        slm_Occupation = cusinfo.slm_Occupation,
                        slm_BaseSalary = cusinfo.slm_BaseSalary,
                        slm_IsCustomer = cusinfo.slm_IsCustomer,
                        slm_CusCode = cusinfo.slm_CusCode,
                        slm_Birthdate = cusinfo.slm_Birthdate,
                        slm_CardType = cusinfo.slm_CardType,
                        slm_CitizenId = cusinfo.slm_CitizenId,
                        slm_Topic = cusinfo.slm_Topic,
                        slm_Detail = cusinfo.slm_Detail,
                        slm_PathLink = cusinfo.slm_PathLink,
                        slm_ContactBranch = cusinfo.slm_ContactBranch,
                        slm_CreatedBy = username,
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = username,
                        slm_UpdatedDate = createdDate
                    };

                    slmdb.kkslm_tr_cusinfo.AddObject(new_cusinfo);
                }
                else
                    throw new Exception("ไม่พบ Ticket Id " + ticketId + " ใน Table kkslm_tr_cusinfo");
            }
            catch
            {
                throw;
            }
        }
    }
}
