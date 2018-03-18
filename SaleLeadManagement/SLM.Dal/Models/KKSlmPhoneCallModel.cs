using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using SLM.Resource;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmPhoneCallModel
    {
        private DateTime _createdDate = new DateTime();
        private string _calculateTotalSlaError = "";

        public DateTime CreatedDate
        {
            get { return _createdDate; }
        }
        public string CalculateTotalSlaError
        {
            get { return _calculateTotalSlaError; }
        }

        public int InsertPhoneCallHistory(string ticketId, string cardType, string cardId, string leadStatusCode, string oldstatus, string ownerBranch, string owner, string oldOwner, string delegateLeadBranch, string delegateLead, string oldDelegateLead, string contactPhone, string contactDetail, string createBy)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    kkslm_tr_lead lead = null;
                    DateTime createDate = DateTime.Now;
                    int? createdByPositionId = null;
                    int? ownerPositionId = null;

                    var cusinfo = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId == ticketId).FirstOrDefault();
                    if (cusinfo != null)
                    {
                        if (cardType != "")
                            cusinfo.slm_CardType = int.Parse(cardType);
                        else
                            cusinfo.slm_CardType = null;

                        string oldCitizenId = string.IsNullOrEmpty(cusinfo.slm_CitizenId) ? null : cusinfo.slm_CitizenId;
                        cusinfo.slm_CitizenId = cardId != "" ? cardId : null;

                        if (oldCitizenId != cusinfo.slm_CitizenId)
                            KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateCardId, oldCitizenId, cusinfo.slm_CitizenId, createBy, createDate);
                    }

                    kkslm_phone_call phone = new kkslm_phone_call();
                    phone.slm_TicketId = ticketId;
                    phone.slm_ContactPhone = contactPhone;
                    phone.slm_ContactDetail = contactDetail;
                    phone.slm_Status = leadStatusCode;
                    phone.slm_Owner = owner;
                    ownerPositionId = GetPositionId(owner);
                    phone.slm_Owner_Position = ownerPositionId;
                    phone.slm_CreateDate = createDate;
                    phone.slm_CreateBy = createBy;
                    createdByPositionId = GetPositionId(createBy);
                    phone.slm_CreatedBy_Position = createdByPositionId;
                    phone.is_Deleted = 0;
                    slmdb.kkslm_phone_call.AddObject(phone);

                    if (lead == null)
                        lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();

                    if (leadStatusCode != oldstatus)
                    {
                        kkslm_tr_activity activity = new kkslm_tr_activity();
                        activity.slm_TicketId = ticketId;
                        activity.slm_OldStatus = oldstatus;
                        activity.slm_NewStatus = leadStatusCode;
                        activity.slm_CreatedBy = createBy;
                        activity.slm_CreatedBy_Position = createdByPositionId;
                        activity.slm_CreatedDate = createDate;
                        activity.slm_Type = SLMConstant.ActionType.ChangeStatus;                //02
                        activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                        activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)

                        activity.slm_ExternalSystem_Old = lead.slm_ExternalSystem;                //add 14/10/2015
                        activity.slm_ExternalStatus_Old = lead.slm_ExternalStatus;                //add 14/10/2015
                        activity.slm_ExternalSubStatus_Old = lead.slm_ExternalSubStatus;          //add 14/10/2015
                        activity.slm_ExternalSubStatusDesc_Old = lead.slm_ExternalSubStatusDesc;  //add 14/10/2015

                        lead.slm_ExternalSystem = null;             //add 14/10/2015
                        lead.slm_ExternalStatus = null;             //add 14/10/2015
                        lead.slm_ExternalSubStatus = null;          //add 14/10/2015
                        lead.slm_ExternalSubStatusDesc = null;      //add 14/10/2015

                        slmdb.kkslm_tr_activity.AddObject(activity);

                        if (lead == null)
                            lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();

                        if (lead != null)
                        {
                            CalculateTotalSLA(slmdb, lead, activity, createDate);        //Method นี้ต้องอยู่ก่อนโค๊ดที่มีการเปลี่ยนแปลงค่าข้อมูล lead
                            lead.slm_Status = leadStatusCode;
                            lead.slm_StatusBy = createBy;
                            lead.slm_StatusDate = createDate;
                            lead.slm_StatusDateSource = createDate;     //add 16/03/2016
                            lead.slm_Counting = 0;
                            lead.slm_UpdatedBy = createBy;
                            lead.slm_UpdatedDate = createDate;
                        }

                        KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateStatus, oldstatus, leadStatusCode, createBy, createDate);
                    }

                    if (owner != oldOwner)
                    {
                        kkslm_tr_activity activity = new kkslm_tr_activity();
                        activity.slm_TicketId = ticketId;
                        if (!string.IsNullOrEmpty(oldOwner))
                        {
                            activity.slm_OldOwner = oldOwner;
                            activity.slm_OldOwner_Position = GetPositionId(oldOwner);
                        }
                        activity.slm_NewOwner = owner;
                        activity.slm_NewOwner_Position = ownerPositionId;
                        activity.slm_CreatedBy = createBy;
                        activity.slm_CreatedBy_Position = createdByPositionId;
                        activity.slm_CreatedDate = createDate;
                        activity.slm_Type = SLMConstant.ActionType.ChangeOwner;
                        activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                        activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                        slmdb.kkslm_tr_activity.AddObject(activity);

                        if (lead == null)
                            lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();

                        if (lead != null)
                        {
                            lead.slm_StaffId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == owner).Select(p => p.slm_StaffId).FirstOrDefault();
                            lead.slm_Owner = owner;
                            lead.slm_Owner_Branch = ownerBranch;
                            lead.slm_Owner_Position = ownerPositionId;
                            lead.slm_AssignedFlag = "0";
                            lead.slm_AssignedDate = null;
                            lead.slm_AssignedBy = null;
                            lead.slm_OldOwner = oldOwner;
                        }

                        KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateOwner, oldOwner, owner, createBy, createDate);
                    }

                    if (delegateLead != oldDelegateLead)
                    {
                        kkslm_tr_activity activity = new kkslm_tr_activity();
                        activity.slm_TicketId = ticketId;
                        if (!string.IsNullOrEmpty(oldDelegateLead))
                        {
                            activity.slm_OldDelegate = oldDelegateLead;
                            activity.slm_OldDelegate_Position = GetPositionId(oldDelegateLead);
                        }
                        activity.slm_NewDelegate = delegateLead;
                        activity.slm_NewDelegate_Position = GetPositionId(delegateLead);
                        activity.slm_CreatedBy = createBy;
                        activity.slm_CreatedBy_Position = createdByPositionId;
                        activity.slm_CreatedDate = createDate;
                        activity.slm_Type = SLMConstant.ActionType.Delegate;
                        activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                        activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                        slmdb.kkslm_tr_activity.AddObject(activity);

                        if (lead == null)
                            lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();

                        if (lead != null)
                        {
                            lead.slm_Delegate_Flag = string.IsNullOrEmpty(delegateLead) ? 0 : 1;
                            if (!string.IsNullOrEmpty(delegateLead))
                                lead.slm_DelegateDate = createDate;
                            else
                                lead.slm_DelegateDate = null;

                            lead.slm_Delegate = string.IsNullOrEmpty(delegateLead) ? null : delegateLead;
                            lead.slm_Delegate_Branch = string.IsNullOrEmpty(delegateLead) ? null : delegateLeadBranch;
                            lead.slm_Delegate_Position = string.IsNullOrEmpty(delegateLead) ? null : GetPositionId(delegateLead);
                        }

                        KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateDelegate, oldDelegateLead, delegateLead, createBy, createDate);
                    }

                    slmdb.SaveChanges();
                    return phone.slm_PhoneCallId;
                }
            }
            catch
            {
                throw;
            }
        }

        public int InsertRenewPhoneCallHistory(string ticketId, string cardType, string cardId, string leadStatusCode, string oldstatus, string ownerBranch, string owner, string oldOwner, string delegateLeadBranch, string delegateLead, string oldDelegateLead, string contactPhone, string contactDetail, string createBy, string subStatusCode, string subStatusDesc, bool Need_CompulsoryFlag, bool Need_PolicyFlag, bool Need_CompulsoryFlagOld, bool Need_PolicyFlagOld
                    , string slm_CreditFileName,
                    string slm_CreditFilePath,
                    int? slm_CreditFileSize,
                    string slm_50TawiFileName,
                    string slm_50TawiFilePath,
                    int? slm_50TawiFileSize,
                    string slm_DriverLicenseiFileName,
                    string slm_DriverLicenseFilePath,
                    int? slm_DriverLicenseFileSize,
                    DateTime? slm_NextContactDate,
                    decimal? slm_cp_blacklist_id,
                      bool Blacklist)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    try
                    {
                        slmdb.Connection.Open();

                        kkslm_tr_lead lead = null;
                        DateTime createDate = DateTime.Now;
                        _createdDate = createDate;
                        int? createdByPositionId = null;
                        int? ownerPositionId = null;
                        string oldSubStatus = string.Empty;
                        string oldExternalSystem = string.Empty;
                        string oldExternalStatus = string.Empty;
                        string oldExternalSubStatus = string.Empty;
                        string oldExternalSubStatusDesc = string.Empty;

                        lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                        if (lead == null) { throw new Exception("ไม่พบข้อมูลผู้มุ่งหวัง Ticket Id=" + ticketId + " ในระบบ"); }

                        var cusinfo = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId == ticketId).FirstOrDefault();
                        if (cusinfo != null)
                        {
                            if (cardType != "")
                                cusinfo.slm_CardType = int.Parse(cardType);
                            else
                                cusinfo.slm_CardType = null;

                            string oldCitizenId = string.IsNullOrEmpty(cusinfo.slm_CitizenId) ? null : cusinfo.slm_CitizenId;
                            cusinfo.slm_CitizenId = cardId != "" ? cardId : null;

                            if (oldCitizenId != cusinfo.slm_CitizenId)
                                KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateCardId, oldCitizenId, cusinfo.slm_CitizenId, createBy, createDate);
                        }

                        kkslm_phone_call phone = new kkslm_phone_call();
                        phone.slm_TicketId = ticketId;
                        phone.slm_ContactPhone = contactPhone;
                        phone.slm_ContactDetail = contactDetail;
                        phone.slm_Status = leadStatusCode;
                        phone.slm_SubStatus = subStatusCode;
                        phone.slm_SubStatusName = subStatusDesc;            //Added By Pom 23/05/2016
                        phone.slm_Owner = owner;
                        ownerPositionId = GetPositionId(owner);
                        phone.slm_Owner_Position = ownerPositionId;
                        phone.slm_CreateDate = createDate;
                        phone.slm_CreateBy = createBy;
                        createdByPositionId = GetPositionId(createBy);
                        phone.slm_CreatedBy_Position = createdByPositionId;
                        phone.is_Deleted = 0;
                        phone.slm_CAS_Flag = null;

                        var reins = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == ticketId).FirstOrDefault();

                        phone.slm_Need_CompulsoryFlag = Need_CompulsoryFlag ? "Y" : "N";
                        if (reins != null)
                        {
                            reins.slm_Need_CompulsoryFlag = phone.slm_Need_CompulsoryFlag;
                        }
                        if (Need_CompulsoryFlag != Need_CompulsoryFlagOld)
                        {
                            phone.slm_Need_CompulsoryFlagDate = DateTime.Now;

                            //report
                            ActivityLeadModel leadModel = new ActivityLeadModel();
                            if (Need_CompulsoryFlag)
                            {
                                leadModel.insertkkslm_tr_actnew_report(reins.slm_TicketId, createBy);
                            }
                            else
                            {
                                leadModel.deletekkslm_tr_actnew_report(reins.slm_TicketId, createBy);
                            }

                            //report

                            if (reins != null)
                            {
                                reins.slm_Need_CompulsoryFlagDate = DateTime.Now;
                            }
                        }

                        phone.slm_Need_PolicyFlag = Need_PolicyFlag ? "Y" : "N";
                        if (reins != null)
                        {
                            reins.slm_Need_PolicyFlag = phone.slm_Need_PolicyFlag;
                        }
                        if (Need_PolicyFlag != Need_PolicyFlagOld)
                        {
                            ActivityLeadModel leadModel = new ActivityLeadModel();
                            //report
                            if (Need_PolicyFlag)
                            {


                                leadModel.insertkkslm_tr_policynew_report(reins.slm_TicketId, createBy);
                            }
                            else
                            {
                                leadModel.deletekkslm_tr_policynew_report(reins.slm_TicketId, createBy);
                            }

                            //report
                            phone.slm_Need_PolicyFlagDate = DateTime.Now;
                            if (reins != null)
                            {
                                reins.slm_Need_PolicyFlagDate = DateTime.Now;
                            }
                        }

                        if (!string.IsNullOrEmpty(slm_CreditFileName))
                        {
                            phone.slm_CreditFileName = slm_CreditFileName;
                            phone.slm_CreditFilePath = slm_CreditFilePath;
                            if (slm_CreditFileSize != null)
                            {
                                phone.slm_CreditFileSize = slm_CreditFileSize;
                            }
                            if (reins != null)
                            {
                                reins.slm_CreditFileName = slm_CreditFileName;
                                reins.slm_CreditFilePath = slm_CreditFilePath;
                                if (slm_CreditFileSize != null)
                                {
                                    reins.slm_CreditFileSize = slm_CreditFileSize;
                                }
                                reins.slm_Need_CreditFlag = "N";
                            }
                        }

                        if (!string.IsNullOrEmpty(slm_50TawiFileName))
                        {
                            phone.slm_50TawiFileName = slm_50TawiFileName;
                            phone.slm_50TawiFilePath = slm_50TawiFilePath;
                            if (slm_50TawiFileSize != null)
                            {
                                phone.slm_50TawiFileSize = slm_50TawiFileSize;
                            }
                            if (reins != null)
                            {
                                reins.slm_50TawiFileName = slm_50TawiFileName;
                                reins.slm_50TawiFilePath = slm_50TawiFilePath;
                                if (slm_50TawiFileSize != null)
                                {
                                    reins.slm_50TawiFileSize = slm_50TawiFileSize;
                                }
                                reins.slm_Need_50TawiFlag = "N";
                            }
                        }

                        if (!string.IsNullOrEmpty(slm_DriverLicenseiFileName))
                        {
                            phone.slm_DriverLicenseiFileName = slm_DriverLicenseiFileName;
                            phone.slm_DriverLicenseFilePath = slm_DriverLicenseFilePath;
                            if (slm_DriverLicenseFileSize != null)
                            {
                                phone.slm_DriverLicenseFileSize = slm_DriverLicenseFileSize;
                            }
                            if (reins != null)
                            {
                                reins.slm_DriverLicenseiFileName = slm_DriverLicenseiFileName;
                                reins.slm_DriverLicenseFilePath = slm_DriverLicenseFilePath;
                                if (slm_DriverLicenseFileSize != null)
                                {
                                    reins.slm_DriverLicenseFileSize = slm_DriverLicenseFileSize;
                                }
                                reins.slm_Need_DriverLicenseFlag = "N";

                            }
                        }

                        if (reins != null)
                        {
                            reins.slm_UpdatedBy = createBy;
                            reins.slm_UpdatedDate = DateTime.Now;
                        }

                        slmdb.kkslm_phone_call.AddObject(phone);
                        slmdb.SaveChanges();

                        //black List
                        if (slm_cp_blacklist_id != null)
                        {
                            kkslm_ms_config_product_blacklist dataBlacklist = slmdb.kkslm_ms_config_product_blacklist.Where(p => p.slm_cp_blacklist_id == slm_cp_blacklist_id).FirstOrDefault();

                            dataBlacklist.slm_IsActive = false;
                            dataBlacklist.slm_UpdatedBy = createBy;
                            dataBlacklist.slm_UpdatedDate = DateTime.Now;
                            slmdb.SaveChanges();
                        }

                        if (Blacklist)
                        {
                            kkslm_ms_config_product_blacklist dataBlacklist = new kkslm_ms_config_product_blacklist();

                            kkslm_tr_lead dataPrelead = slmdb.kkslm_tr_lead.Where(r => r.slm_ticketId == ticketId).FirstOrDefault();

                            kkslm_tr_cusinfo cus = slmdb.kkslm_tr_cusinfo.Where(r => r.slm_TicketId == ticketId).FirstOrDefault();
                            if (dataPrelead != null)
                            {
                                kkslm_ms_config_product_day day = slmdb.kkslm_ms_config_product_day.Where(cd => cd.slm_Product_Id == dataPrelead.slm_Product_Id && cd.slm_Type == "BLACKLIST").FirstOrDefault();
                                dataBlacklist.slm_Product_Id = lead.slm_Product_Id;
                                dataBlacklist.slm_Prelead_Id = null;
                                dataBlacklist.slm_ticketId = ticketId;
                                dataBlacklist.slm_Name = dataPrelead.slm_Name;
                                dataBlacklist.slm_LastName = dataPrelead.slm_LastName;
                                dataBlacklist.slm_CardType = cus.slm_CardType;
                                dataBlacklist.slm_CitizenId = cus.slm_CitizenId;
                                dataBlacklist.slm_StartDate = DateTime.Now;
                                dataBlacklist.slm_EndDate = (DateTime?)DateTime.Now.AddDays(day.slm_Days.Value);
                                dataBlacklist.slm_IsActive = true;
                                dataBlacklist.slm_CreatedDate = DateTime.Now;
                                dataBlacklist.slm_CreatedBy = createBy;
                                dataBlacklist.slm_UpdatedDate = DateTime.Now;
                                dataBlacklist.slm_UpdatedBy = createBy;

                                slmdb.kkslm_ms_config_product_blacklist.AddObject(dataBlacklist);
                            }
                        }

                        //================= Update Lead Data Section ================================================================

                        lead.slm_NextContactDate = slm_NextContactDate;

                        //Add By Pom 23/05/2016 - เก็บค่าเก่า
                        oldstatus = lead.slm_Status;
                        oldSubStatus = lead.slm_SubStatus;
                        oldExternalSystem = lead.slm_ExternalSystem;
                        oldExternalStatus = lead.slm_ExternalStatus;
                        oldExternalSubStatus = string.IsNullOrEmpty(lead.slm_ExternalSubStatus) ? null : lead.slm_ExternalSubStatus;
                        oldExternalSubStatusDesc = lead.slm_ExternalSubStatusDesc;

                        if (leadStatusCode != oldstatus || subStatusCode != oldSubStatus)
                        {
                            kkslm_tr_activity activity = new kkslm_tr_activity()
                            {
                                slm_TicketId = ticketId,
                                slm_OldStatus = oldstatus,
                                slm_NewStatus = leadStatusCode,
                                slm_OldSubStatus = oldSubStatus,
                                slm_NewSubStatus = subStatusCode,
                                slm_CreatedBy = createBy,
                                slm_CreatedBy_Position = createdByPositionId,
                                slm_CreatedDate = createDate,
                                slm_Type = SLMConstant.ActionType.ChangeStatus,                //02
                                slm_SystemAction = SLMConstant.SystemName,                     //System ที่เข้ามาทำ action (23/05/2015)
                                slm_SystemActionBy = SLMConstant.SystemName,                   //action เกิดขึ้นที่ระบบอะไร (23/05/2015)
                                slm_ExternalSystem_Old = oldExternalSystem,                //add 23/05/2015
                                slm_ExternalStatus_Old = oldExternalStatus,                //add 23/05/2015
                                slm_ExternalSubStatus_Old = oldExternalSubStatus,          //add 23/05/2015
                                slm_ExternalSubStatusDesc_Old = oldExternalSubStatusDesc,  //add 23/05/2015
                                slm_ExternalSystem_New = null,
                                slm_ExternalStatus_New = null,
                                slm_ExternalSubStatus_New = null,
                                slm_ExternalSubStatusDesc_New = subStatusDesc
                            };
                            slmdb.kkslm_tr_activity.AddObject(activity);

                            CalculateTotalSLA(slmdb, lead, activity, createDate);        //Method นี้ต้องอยู่ก่อนโค๊ดที่มีการเปลี่ยนแปลงค่าข้อมูล lead
                            lead.slm_Status = leadStatusCode;
                            lead.slm_SubStatus = subStatusCode;
                            lead.slm_StatusBy = createBy;
                            lead.slm_StatusDate = createDate;
                            lead.slm_StatusDateSource = createDate;
                            lead.slm_Counting = 0;
                            lead.slm_ExternalSystem = null;                     //add 23/05/2015
                            lead.slm_ExternalStatus = null;                     //add 23/05/2015
                            lead.slm_ExternalSubStatus = null;                  //add 23/05/2015
                            lead.slm_ExternalSubStatusDesc = subStatusDesc;     //add 23/05/2015
                            lead.slm_UpdatedBy = createBy;
                            lead.slm_UpdatedDate = createDate;
                            slmdb.SaveChanges();

                            KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateStatus, oldstatus, leadStatusCode, createBy, createDate);
                        }

                        if (owner != oldOwner)
                        {
                            kkslm_tr_activity activity = new kkslm_tr_activity();
                            activity.slm_TicketId = ticketId;
                            if (!string.IsNullOrEmpty(oldOwner))
                            {
                                activity.slm_OldOwner = oldOwner;
                                activity.slm_OldOwner_Position = GetPositionId(oldOwner);
                            }
                            activity.slm_NewOwner = owner;
                            activity.slm_NewOwner_Position = ownerPositionId;
                            activity.slm_CreatedBy = createBy;
                            activity.slm_CreatedBy_Position = createdByPositionId;
                            activity.slm_CreatedDate = createDate;
                            activity.slm_Type = SLMConstant.ActionType.ChangeOwner;
                            activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                            activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                            slmdb.kkslm_tr_activity.AddObject(activity);

                            lead.slm_StaffId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == owner).Select(p => p.slm_StaffId).FirstOrDefault();
                            lead.slm_Owner = owner;
                            lead.slm_Owner_Branch = ownerBranch;
                            lead.slm_Owner_Position = ownerPositionId;
                            lead.slm_AssignedFlag = "0";
                            lead.slm_AssignedDate = null;
                            lead.slm_AssignedBy = null;
                            lead.slm_OldOwner = oldOwner;

                            KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateOwner, oldOwner, owner, createBy, createDate);
                        }
                        slmdb.SaveChanges();

                        if (delegateLead != oldDelegateLead)
                        {
                            kkslm_tr_activity activity = new kkslm_tr_activity();
                            activity.slm_TicketId = ticketId;
                            if (!string.IsNullOrEmpty(oldDelegateLead))
                            {
                                activity.slm_OldDelegate = oldDelegateLead;
                                activity.slm_OldDelegate_Position = GetPositionId(oldDelegateLead);
                            }
                            activity.slm_NewDelegate = delegateLead;
                            activity.slm_NewDelegate_Position = GetPositionId(delegateLead);
                            activity.slm_CreatedBy = createBy;
                            activity.slm_CreatedBy_Position = createdByPositionId;
                            activity.slm_CreatedDate = createDate;
                            activity.slm_Type = SLMConstant.ActionType.Delegate;
                            activity.slm_SystemAction = SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                            activity.slm_SystemActionBy = SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                            slmdb.kkslm_tr_activity.AddObject(activity);

                            lead.slm_Delegate_Flag = string.IsNullOrEmpty(delegateLead) ? 0 : 1;
                            if (!string.IsNullOrEmpty(delegateLead))
                                lead.slm_DelegateDate = createDate;
                            else
                                lead.slm_DelegateDate = null;

                            lead.slm_Delegate = string.IsNullOrEmpty(delegateLead) ? null : delegateLead;
                            lead.slm_Delegate_Branch = string.IsNullOrEmpty(delegateLead) ? null : delegateLeadBranch;
                            lead.slm_Delegate_Position = string.IsNullOrEmpty(delegateLead) ? null : GetPositionId(delegateLead);

                            KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.UpdateDelegate, oldDelegateLead, delegateLead, createBy, createDate);
                        }

                        slmdb.SaveChanges();

                        return phone != null ? phone.slm_PhoneCallId : 0;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (slmdb != null)
                        {
                            slmdb.Connection.Close();
                            slmdb.Connection.Dispose();
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void CalculateTotalSLA(SLM_DBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_activity act, DateTime createdDate)
        {
            try
            {
                //Initial variables
                DateTime currentSla = new DateTime();
                DateTime currentDate = DeleteSeconds(createdDate);
                int thisWork = lead.slm_ThisWork != null ? Convert.ToInt32(lead.slm_ThisWork) : 0;
                int slaCounting = lead.slm_Counting != null ? Convert.ToInt32(lead.slm_Counting.Value) : 0;

                int workingMinPerDay = 0;
                int startTimeHour = 0;
                int startTimeMin = 0;
                int endTimeHour = 0;
                int endTimeMin = 0;

                var calendarTab = slmdb.kkslm_ms_calendar_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch && p.is_Deleted == false).ToList();
                var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch).FirstOrDefault();

                if (branch == null) { throw new Exception("ไม่พบข้อมูลสาขา BranchCode:" + lead.slm_Owner_Branch); }

                if (string.IsNullOrEmpty(branch.slm_StartTime_Hour) || string.IsNullOrEmpty(branch.slm_StartTime_Minute)
                    || string.IsNullOrEmpty(branch.slm_EndTime_Hour) || string.IsNullOrEmpty(branch.slm_EndTime_Minute))
                {
                    string start = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "startTime").Select(p => p.slm_OptionDesc).FirstOrDefault();
                    string end = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "endTime").Select(p => p.slm_OptionDesc).FirstOrDefault();

                    if (start != null)
                    {
                        string[] str = start.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            startTimeHour = Convert.ToInt32(str[0]);
                            startTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                            throw new Exception("ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);
                    }
                    else
                        throw new Exception("ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);

                    if (end != null)
                    {
                        string[] str = end.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            endTimeHour = Convert.ToInt32(str[0]);
                            endTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                            throw new Exception("ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                    }
                    else
                        throw new Exception("ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                }
                else
                {
                    startTimeHour = int.Parse(branch.slm_StartTime_Hour);
                    endTimeMin = int.Parse(branch.slm_StartTime_Minute);
                    endTimeHour = int.Parse(branch.slm_EndTime_Hour);
                    endTimeMin = int.Parse(branch.slm_EndTime_Minute);

                    DateTime tmpStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startTimeHour, startTimeMin, 0);
                    DateTime tmpEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endTimeHour, endTimeMin, 0);

                    TimeSpan tmpTs = tmpEnd.Subtract(tmpStart);
                    workingMinPerDay = Convert.ToInt32(tmpTs.TotalMinutes);     //ได้เวลาที่ต้องทำงานในแต่ละวันของสาขา หน่วยเป็นนาที
                }

                if (slaCounting == 0)    //กรณีทำงานเสร็จก่อน sla เตือน
                {
                    //เผื่อในกรณี slm_StatusDate เป็น null, ซึ่งน่าจะไม่มีโอกาสเกิดเคสนี้ ยกเว้น คีย์ข้อมูลลงเบสตรง
                    currentSla = lead.slm_StatusDate != null ? DeleteSeconds(lead.slm_StatusDate.Value) : currentDate;
                }
                else
                {
                    currentSla = lead.slm_CurrentSLA != null ? DeleteSeconds(lead.slm_CurrentSLA.Value) : currentDate;
                }

                //ปรับเวลาในกรณี currentSla อยู่นอกเวลาทำงาน, หรือถ้าปรับแล้วยังไปตกวันหยุดก็ต้องให้เลื่อนไปจนถึงวันทำงาน
                int checkStartTime = Convert.ToInt32(startTimeHour.ToString("00") + startTimeMin.ToString("00"));
                int checkEndTime = Convert.ToInt32(endTimeHour.ToString("00") + endTimeMin.ToString("00"));
                int timeToCheck = Convert.ToInt32(currentSla.Hour.ToString("00") + currentSla.Minute.ToString("00"));

                if (timeToCheck < checkStartTime || timeToCheck > checkEndTime)     //ถ้าเวลาของ currentSla ไม่ได้อยู่ในช่วงเวลาทำงาน
                {
                    if (timeToCheck >= checkEndTime && timeToCheck <= 2359)
                    {
                        currentSla = currentSla.AddDays(1);
                        currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, startTimeHour, startTimeMin, 0);
                    }
                    else
                        currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, startTimeHour, startTimeMin, 0);
                }

                while (calendarTab.Where(p => p.slm_HolidayDate.Date == currentSla.Date).Count() > 0)
                {
                    currentSla = currentSla.AddDays(1);
                }

                //ปรับเวลาในกรณี currentDate อยู่นอกเวลาทำงาน, หรือถ้าปรับแล้วยังไปตกวันหยุดก็ต้องให้เลื่อนไปจนถึงวันทำงาน
                timeToCheck = Convert.ToInt32(currentDate.Hour.ToString("00") + currentDate.Minute.ToString("00"));

                if (timeToCheck < checkStartTime || timeToCheck > checkEndTime)     //ถ้าเวลาของ currentSla ไม่ได้อยู่ในช่วงเวลาทำงาน
                {
                    if (timeToCheck >= checkEndTime && timeToCheck <= 2359)
                    {
                        currentDate = currentDate.AddDays(1);
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);
                    }
                    else
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);
                }

                while (calendarTab.Where(p => p.slm_HolidayDate.Date == currentDate.Date).Count() > 0)
                {
                    currentDate = currentDate.AddDays(1);
                }

                //คำนวณ TotalAlert, TotalWork
                if (currentSla.Date == currentDate.Date)
                {
                    TimeSpan ts = currentDate.Subtract(currentSla);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);
                }
                else
                {
                    //หาวันที่อยู่ตรงกลางระหว่าง currentSla กับ currentDate
                    //เช่น currentSla = 12/04/2016, currentDate = 16/04/2016 ให้หาวันที่ 13, 14, 15 ออกมาเพื่อเช็กว่าเป็นวันทำงานหรือวันหยุด ถ้าเป็นวันทำงานให้เก็บจำนวนนาทีที่ต้องทำงานต่อวัน
                    DateTime startDate = currentSla.Date;
                    DateTime endDate = currentDate.Date;

                    DateTime tmpDate = startDate.AddDays(1);
                    while (tmpDate < endDate)
                    {
                        if (calendarTab.Where(p => p.slm_HolidayDate == tmpDate).Count() == 0)
                        {
                            thisWork += workingMinPerDay;
                        }
                        tmpDate = tmpDate.AddDays(1);
                    }

                    //ให้คำนวณเวลาที่เหลือในส่วนของวัน startDate และ endDate
                    DateTime endWorkTime_for_currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, endTimeHour, endTimeMin, 0);
                    DateTime startWorkTime_for_currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);

                    TimeSpan ts = endWorkTime_for_currentSla.Subtract(currentSla);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);

                    ts = currentDate.Subtract(startWorkTime_for_currentDate);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);
                }

                lead.slm_TotalAlert = (lead.slm_TotalAlert != null ? Convert.ToInt32(lead.slm_TotalAlert) : 0) + slaCounting;
                lead.slm_TotalWork = (lead.slm_TotalWork != null ? Convert.ToInt32(lead.slm_TotalWork) : 0) + thisWork;
                lead.slm_Counting = 0;
                lead.slm_ThisWork = 0;
                lead.slm_CurrentSLA = null;

                //OwnerLoggin
                act.slm_ThisAlert = slaCounting;
                act.slm_ThisWork = thisWork;
            }
            catch (Exception ex)
            {
                _calculateTotalSlaError = "CalculateTotalSLA Error=" + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        private DateTime DeleteSeconds(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
        }

        private int? GetPositionId(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetPositionId(username);
        }

        //Added by Pom 21/05/2016
        public void UpdateCasFlag(int phonecallId, string flag)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var phonecall = slmdb.kkslm_phone_call.Where(p => p.slm_PhoneCallId == phonecallId).FirstOrDefault();
                    if (phonecall != null)
                    {
                        phonecall.slm_CAS_Flag = flag;
                        phonecall.slm_CAS_Date = DateTime.Now;
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public int InsertPhonecallEcm(string ticketId, string invoiceFilePath, string invoiceFileName, int invoiceFileSize, string createByUsername)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                    var positionId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == createByUsername).Select(p => p.slm_Position_id).FirstOrDefault();
                    if (lead != null)
                    {
                        DateTime createDate = DateTime.Now;
                        //string url = SLMConstant.Ecm.SiteUrl.Replace("/dept/public", "");

                        string ecmUrl = SLMConstant.Ecm.SiteUrl + "/" + SLMConstant.Ecm.ListName + "/" + ticketId + "/" + invoiceFileName;

                        kkslm_phone_call phone = new kkslm_phone_call()
                        {
                            slm_TicketId = ticketId,
                            slm_Status = lead.slm_Status,
                            slm_SubStatus = lead.slm_SubStatus,
                            slm_SubStatusName = lead.slm_ExternalSubStatusDesc,
                            slm_Owner = lead.slm_Owner,
                            slm_Owner_Position = lead.slm_Owner_Position,
                            slm_ContactDetail = ecmUrl,
                            slm_InvoiceFilePath = invoiceFilePath,
                            slm_InvoiceFileName = invoiceFileName,
                            slm_InvoiceFileSize = invoiceFileSize,
                            slm_CreateBy = createByUsername,
                            slm_CreatedBy_Position = positionId,
                            slm_CreateDate = createDate,
                            is_Deleted = 0
                        };
                        slmdb.kkslm_phone_call.AddObject(phone);
                        slmdb.SaveChanges();
                        return phone.slm_PhoneCallId;
                    }
                    else
                        throw new Exception("ไม่พบ TicketId " + ticketId + " ในระบบ");
                }
            }
            catch
            {
                throw;
            }
        }

        public List<PhoneCallHistoryData> InsertPhonecallEcm(string[] ticketIdList, string invoiceFilePath, string invoiceFileName, int invoiceFileSize, string createByUsername, string mainId)
        {
            try
            {
                List<PhoneCallHistoryData> idList = new List<PhoneCallHistoryData>();
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        foreach (string ticketId in ticketIdList)
                        {
                            var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                            var positionId = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == createByUsername).Select(p => p.slm_Position_id).FirstOrDefault();
                            if (lead != null)
                            {
                                DateTime createDate = DateTime.Now;
                                //string url = SLMConstant.Ecm.SiteUrl.Replace("/dept/public", "");

                                string ecmUrl = SLMConstant.Ecm.SiteUrl + "/" + SLMConstant.Ecm.ListName + "/" + mainId + "/" + invoiceFileName;

                                kkslm_phone_call phone = new kkslm_phone_call()
                                {
                                    slm_TicketId = ticketId,
                                    slm_Status = lead.slm_Status,
                                    slm_SubStatus = lead.slm_SubStatus,
                                    slm_SubStatusName = lead.slm_ExternalSubStatusDesc,
                                    slm_Owner = lead.slm_Owner,
                                    slm_Owner_Position = lead.slm_Owner_Position,
                                    slm_ContactDetail = ecmUrl,
                                    slm_InvoiceFilePath = invoiceFilePath,
                                    slm_InvoiceFileName = invoiceFileName,
                                    slm_InvoiceFileSize = invoiceFileSize,
                                    slm_CreateBy = createByUsername,
                                    slm_CreatedBy_Position = positionId,
                                    slm_CreateDate = createDate,
                                    is_Deleted = 0
                                };
                                slmdb.kkslm_phone_call.AddObject(phone);
                                slmdb.SaveChanges();

                                idList.Add(new PhoneCallHistoryData() { PhoneCallId = phone.slm_PhoneCallId, TicketId = ticketId });
                            }
                            else
                                throw new Exception("ไม่พบ TicketId " + ticketId + " ในระบบ");
                        }
                    }
                    
                    ts.Complete();
                }

                return idList;
            }
            catch
            {
                throw;
            }
        }
    }
}
