using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Dal;
using SLM.Resource;

namespace SLM.Biz
{
    public class PreleadPhoneCallBiz : IDisposable
    {
        private SLM_DBEntities slmdb = null;

        public PreleadPhoneCallBiz()
        {
            slmdb = DBUtil.GetSlmDbEntities();
            slmdb.Connection.Open();
        }

        public void Dispose()
        {
            if (slmdb != null)
            {
                slmdb.Connection.Close();
                slmdb.Connection.Dispose();
                slmdb.Dispose();
            }
        }

        public void CloseConnection()
        {
            if (slmdb != null)
            {
                slmdb.Connection.Close();
            }
        }

        public decimal _phoneCallId = 0;
        public decimal PhoneCallId
        {
            get { return _phoneCallId; }
        }

        public string TicketId { get; set; }
        public string CampaignId { get; set; }
        public string ProductId { get; set; }

        private DateTime _createdDate = new DateTime();
        public DateTime CreatedDate
        {
            get { return _createdDate; }
        }

        private bool _doRedirect = false;
        public bool DoRedirect
        {
            get { return _doRedirect; }
        }
        public string ErrorMessage { get; set; }

        //public void InsertData(decimal preleadId, string contractNo, string contactPhone, string contactDetail, string statusCode, string currentSubStatusCode, string currentSubStatusName, string newSubStatusCode, string newSubStatusName
        //    , DateTime appointmentDate, string cardTypeId, string citizenId, string createByUsername, string telNo1, string blacklistId, bool blacklistValue)
        //{
        //    try
        //    {
        //        KKSlmTrPreleadPhoneCallModel model = new KKSlmTrPreleadPhoneCallModel();
        //        model.InsertData(preleadId, contractNo, contactPhone, contactDetail, statusCode, currentSubStatusCode, currentSubStatusName, newSubStatusCode, newSubStatusName, appointmentDate
        //                                                    , cardTypeId, citizenId, createByUsername, telNo1, blacklistId, blacklistValue);

        //        _phoneCallId = model.PhoneCallId;
        //        _ticketId = model.TicketId;
        //        _createdDate = model.CreatedDate;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //Added by Pom 21/05/2016
        public void UpdateCasFlag(decimal phonecallId, string flag)
        {
            new KKSlmTrPreleadPhoneCallModel().UpdateCasFlag(phonecallId, flag);
        }

        public decimal InsertPhonecallEcm(decimal preleadId, string invoiceFilePath, string invoiceFileName, int invoiceFileSize, string createByUsername)
        {
            return new KKSlmTrPreleadPhoneCallModel().InsertPhonecallEcm(preleadId, invoiceFilePath, invoiceFileName, invoiceFileSize, createByUsername);
        }

        public List<preleadPhoneCallHistoryData> InsertPhonecallEcm(string[] preleadIdList, string invoiceFilePath, string invoiceFileName, int invoiceFileSize, string createByUsername, string mainId)
        {
            return new KKSlmTrPreleadPhoneCallModel().InsertPhonecallEcm(preleadIdList, invoiceFilePath, invoiceFileName, invoiceFileSize, createByUsername, mainId);
        }

        //====================================================================================================================

        public void InsertData(decimal preleadId, string contractNo, string contactPhone, string contactDetail, string statusCode, string currentSubStatusCode, string currentSubStatusName, string newSubStatusCode, string newSubStatusName
            , DateTime appointmentDate, string cardTypeId, string citizenId, string createByUsername, string telNo1, string blacklistId, bool blacklistValue, string telNoSms)
        {
            try
            {
                kkslm_tr_prelead_phone_call phcall = null;
                _createdDate = DateTime.Now;
                var creater = GetCreatePersonInfo(createByUsername, slmdb);
                string OldstatusCode = "";

                var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == preleadId).FirstOrDefault();
                if (prelead == null) { throw new Exception("ไม่พบ PreleadId " + preleadId + " ในระบบ"); }

                //Get Owner Infomation
                kkslm_ms_staff owner = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == prelead.slm_Owner).FirstOrDefault();
                if (owner == null) { throw new Exception("ไม่พบข้อมูล Owner ในระบบ, PreleadId " + preleadId); }

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    //Blacklist
                    if (!string.IsNullOrEmpty(blacklistId))
                    {
                        decimal id = Convert.ToDecimal(blacklistId);
                        var blacklist = slmdb.kkslm_ms_config_product_blacklist.Where(p => p.slm_cp_blacklist_id == id).FirstOrDefault();

                        //ถ้ามีการเปลี่ยนค่า blacklist บนหน้าจอ
                        if (blacklist != null && blacklist.slm_IsActive != blacklistValue)
                        {
                            blacklist.slm_IsActive = blacklistValue;
                            blacklist.slm_UpdatedBy = createByUsername;
                            blacklist.slm_UpdatedDate = _createdDate;
                        }
                    }
                    else
                    {
                        //ถ้ายังไม่ติด blacklist (blacklistId = "") แล้วมีการ checkbox blacklist = true ให้สร้าง record blasklist ใหม่
                        if (blacklistValue)
                        {
                            int? temp = slmdb.kkslm_ms_config_product_day.Where(p => p.slm_Product_Id == prelead.slm_Product_Id && p.slm_Type == "BLACKLIST").Select(p => p.slm_Days).FirstOrDefault();
                            double numOfDay = 0;
                            if (temp != null)
                                numOfDay = Convert.ToDouble(temp);

                            DateTime startDate = _createdDate;
                            DateTime endDate = _createdDate.AddDays(numOfDay);

                            kkslm_ms_config_product_blacklist bl = new kkslm_ms_config_product_blacklist()
                            {
                                slm_Product_Id = prelead.slm_Product_Id,
                                slm_CitizenId = citizenId,
                                slm_IsActive = true,
                                slm_Name = prelead.slm_Name,
                                slm_LastName = prelead.slm_LastName,
                                slm_Prelead_Id = prelead.slm_Prelead_Id,
                                slm_StartDate = startDate,
                                slm_EndDate = endDate,
                                slm_CreatedBy = createByUsername,
                                slm_CreatedDate = _createdDate,
                                slm_UpdatedBy = createByUsername,
                                slm_UpdatedDate = _createdDate,
                                slm_ticketId = null
                            };
                            if (!string.IsNullOrEmpty(cardTypeId))
                                bl.slm_CardType = Convert.ToInt32(cardTypeId);

                            slmdb.kkslm_ms_config_product_blacklist.AddObject(bl);
                        }
                    }

                    if (!string.IsNullOrEmpty(cardTypeId))
                    {
                        prelead.slm_CardTypeId = int.Parse(cardTypeId);
                    }
                    else
                    {
                        prelead.slm_CardTypeId = null;
                    }
                        
                    prelead.slm_CitizenId = string.IsNullOrEmpty(citizenId) ? null : citizenId;

                    if (appointmentDate.Year != 1)
                    {
                        prelead.slm_NextContactDate = appointmentDate;
                    }
                    else
                    {
                        prelead.slm_NextContactDate = null;
                    }

                    prelead.slm_TelNoSms = telNoSms;
                    OldstatusCode = prelead.slm_Status;

                    //Update TelNo1
                    var preAddrList = slmdb.kkslm_tr_prelead_address.Where(p => p.slm_Prelead_Id == preleadId && p.slm_Address_Type == "C").ToList();
                    if (preAddrList.Count > 0)
                    {
                        preAddrList.ForEach(p => p.slm_Mobile_Phone = telNo1);
                    }
                    else
                    {
                        kkslm_tr_prelead_address address = new kkslm_tr_prelead_address()
                        {
                            slm_CmtLot = prelead.slm_CmtLot,
                            slm_Prelead_Id = preleadId,
                            slm_Customer_Key = prelead.slm_Customer_Key,
                            slm_Address_Type = "C",
                            slm_Mobile_Phone = telNo1,
                            slm_CreatedBy = createByUsername,
                            slm_CreatedDate = _createdDate,
                            slm_UpdatedBy = createByUsername,
                            slm_UpdatedDate = _createdDate
                        };
                        slmdb.kkslm_tr_prelead_address.AddObject(address);
                    }

                    phcall = new kkslm_tr_prelead_phone_call()
                    {
                        slm_Contract_Number = contractNo,
                        slm_Prelead_Id = preleadId,
                        slm_ContactPhone = contactPhone,
                        slm_ContactDetail = contactDetail,
                        slm_Status = statusCode,
                        slm_SubStatus = newSubStatusCode,
                        slm_SubStatusName = newSubStatusName,
                        slm_Owner = owner != null ? owner.slm_EmpCode : null,
                        slm_Owner_Position = owner != null ? owner.slm_Position_id : null,
                        slm_CreatedBy = createByUsername,
                        slm_CreatedBy_Position = creater != null ? creater.PositionId : null,
                        slm_CreatedDate = _createdDate,
                        slm_CAS_Flag = null
                    };
                    if (appointmentDate.Year != 1)
                    {
                        phcall.slm_Appointment_Date = appointmentDate;
                    }

                    slmdb.kkslm_tr_prelead_phone_call.AddObject(phcall);

                    //Check SubStatusCode Changed
                    if (currentSubStatusCode != newSubStatusCode)
                    {
                        kkslm_tr_prelead_activity act = new kkslm_tr_prelead_activity()
                        {
                            slm_Prelead_Id = preleadId,
                            slm_OldStatus = OldstatusCode,      //OldstatusCode กับ statusCode เป็นค่าเดียวกันเนื่องจากยังเป็น Prelead จึงมีค่าสถานะเป็น Outbound เสมอ
                            slm_NewStatus = statusCode,
                            slm_OldSubStatus = currentSubStatusCode,
                            slm_NewSubStatus = newSubStatusCode,
                            slm_CreatedBy = createByUsername,
                            slm_CreatedBy_Position = creater != null ? creater.PositionId : null,
                            slm_CreatedDate = _createdDate,
                            slm_Type = SLMConstant.ActionType.ChangeStatus,
                            slm_SystemAction = SLMConstant.SystemNameOBT,      //System ที่เข้ามาทำ action (31/03/2016)
                            slm_SystemActionBy = SLMConstant.SystemNameOBT,    //Action เกิดขึ้นที่ระบบอะไร (31/03/2016)
                            slm_ExternalSubStatusDesc_Old = currentSubStatusName,
                            slm_ExternalSubStatusDesc_New = newSubStatusName,       //ใช้ Field แบบเดียวกับ Lead เพื่อให้สามารถคิวรี่รวมกันได้
                        };
                        slmdb.kkslm_tr_prelead_activity.AddObject(act);

                        prelead.slm_SubStatus = newSubStatusCode;
                        prelead.slm_StatusDate = _createdDate;
                        prelead.slm_Counting = 0;
                    }

                    prelead.slm_UpdatedBy = createByUsername;
                    prelead.slm_UpdatedDate = _createdDate;
                    slmdb.SaveChanges();

                    if (newSubStatusCode == SLMConstant.SubStatusCode.ActPurchased || newSubStatusCode == SLMConstant.SubStatusCode.AcceptRenew)
                    {
                        //Check prelead already convert to lead (check only on prelead side)
                        var temp_prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == preleadId && p.slm_TicketId != null && p.slm_TicketId != "").FirstOrDefault();
                        if (temp_prelead != null)
                        {
                            _doRedirect = true;
                            ErrorMessage = string.Format("ไม่สามารถสร้าง Lead ได้ เนื่องจากข้อมูล outbound นี้ มี Ticket Id ({0}) แล้ว", temp_prelead.slm_TicketId);
                            return;
                        }

                        //Check Access Right
                        if (PassPrivilegeCampaign(SLMConstant.Branch.Active, prelead.slm_CampaignId, owner.slm_UserName))
                        {
                            ConvertToLead(prelead, owner, creater, createByUsername, _createdDate, telNo1, contractNo, newSubStatusCode, newSubStatusName, slmdb);
                        }
                        else
                        {
                            throw new Exception("ไม่สามารถสร้าง Lead ได้ เนื่องจาก Owner Lead ไม่มีสิทธิ์ในแคมเปญนี้");
                        }
                    }

                    ts.Complete();
                }

                _phoneCallId = phcall != null ? phcall.slm_PhoneCallId : 0;
            }
            catch
            {
                throw;
            }
        }

        //private string ConvertToLead(kkslm_tr_prelead prelead, kkslm_ms_staff owner, StaffData creater, string createByUsername, DateTime createDate, string telNo1, string contractNo, string newSubStatusCode, string newSubStatusName, SLM_DBEntities slmdb)
        //{
        //    try
        //    {
        //        string campaignName = "";
        //        string campaignDesc = "";
        //        string statusCode = SLMConstant.StatusCode.Interest;   //สนใจ
        //        string subStatusCode = "";
        //        string subStatusDesc = "";

        //        //KKSlmMsConfigProductSubstatusModel model = new KKSlmMsConfigProductSubstatusModel();
        //        var subStatusList = GetSubStatusList(prelead.slm_Product_Id, prelead.slm_CampaignId, statusCode);
        //        if (newSubStatusCode == SLMConstant.SubStatusCode.ActPurchased)
        //        {
        //            subStatusCode = subStatusList.Where(p => p.ValueField == "15").Select(p => p.ValueField).FirstOrDefault();
        //            subStatusDesc = subStatusList.Where(p => p.ValueField == "15").Select(p => p.TextField).FirstOrDefault();
        //        }
        //        else if (newSubStatusCode == SLMConstant.SubStatusCode.AcceptRenew)
        //        {
        //            subStatusCode = subStatusList.Where(p => p.ValueField == "16").Select(p => p.ValueField).FirstOrDefault();
        //            subStatusDesc = subStatusList.Where(p => p.ValueField == "16").Select(p => p.TextField).FirstOrDefault();
        //        }

        //        StoreProcedure store = new StoreProcedure();
        //        string ticketId = store.GenerateTicketId(slmdb);

        //        prelead.slm_TicketId = ticketId;

        //        //Table kkslm_tr_lead
        //        kkslm_tr_lead lead = new kkslm_tr_lead()
        //        {
        //            slm_ticketId = ticketId,
        //            slm_TitleId = prelead.slm_TitleId,
        //            slm_Name = prelead.slm_Name,
        //            slm_LastName = prelead.slm_LastName,
        //            slm_TelNo_1 = telNo1,
        //            slm_CampaignId = prelead.slm_CampaignId,
        //            slm_Owner_Branch = owner != null ? owner.slm_BranchCode : null,
        //            slm_Owner = owner != null ? owner.slm_UserName : null,
        //            slm_Owner_Position = owner != null ? owner.slm_Position_id : null,
        //            slm_Delegate = null,
        //            slm_Status = statusCode,
        //            slm_SubStatus = subStatusCode,
        //            slm_StatusBy = createByUsername,
        //            slm_StatusDate = createDate,
        //            slm_StatusDateSource = createDate,
        //            slm_ExternalSubStatusDesc = subStatusDesc,
        //            slm_AvailableTime = null,
        //            slm_Counting = 0,
        //            slm_ChannelId = creater != null ? creater.ChannelId : null,
        //            slm_CreatedBy = createByUsername,
        //            slm_CreatedBy_Position = creater != null ? creater.PositionId : null,
        //            slm_CreatedDate = createDate,
        //            slm_CreatedBy_Branch = creater != null ? creater.BranchCode : null,
        //            slm_UpdatedBy = createByUsername,
        //            slm_UpdatedDate = createDate,
        //            slm_AssignedFlag = "0",
        //            slm_Product_Group_Id = "",
        //            slm_Product_Id = prelead.slm_Product_Id,
        //            slm_Product_Name = "",
        //            slm_ThisWork = 0,
        //            slm_TotalAlert = 0,
        //            slm_TotalWork = 0,
        //            is_Deleted = 0
        //        };
        //        if (owner != null) lead.slm_StaffId = owner.slm_StaffId;
        //        //CmtCampaignProductModel campaign = new CmtCampaignProductModel();
        //        List<ProductData> list = GetDataByCampaignId(prelead.slm_CampaignId);
        //        if (list.Count > 0)
        //        {
        //            lead.slm_Product_Group_Id = list[0].ProductGroupId;
        //            lead.slm_Product_Id = list[0].ProductId;
        //            lead.slm_Product_Name = list[0].ProductName;
        //            campaignName = list[0].CampaignName;
        //            campaignDesc = list[0].CampaignDesc;
        //        }
        //        slmdb.kkslm_tr_lead.AddObject(lead);

        //        //Table kkslm_tr_cusinfo
        //        int marital_status = slmdb.kkslm_ms_marital_status.Where(p => p.slm_MaritalStatusName == prelead.slm_Marital_Status).Select(p => p.slm_MaritalStatusId).FirstOrDefault();
        //        kkslm_tr_cusinfo info = new kkslm_tr_cusinfo()
        //        {
        //            slm_TicketId = ticketId,
        //            slm_LastName = prelead.slm_LastName,
        //            slm_Email = prelead.slm_Email,
        //            slm_TelNo_2 = null,
        //            slm_TelNo_3 = null,
        //            slm_Ext_2 = null,
        //            slm_Ext_3 = null,
        //            slm_BuildingName = null,
        //            slm_AddressNo = null,
        //            slm_Floor = null,
        //            slm_Soi = null,
        //            slm_Street = null,
        //            slm_Tambon = null,
        //            slm_Amphur = null,
        //            slm_Province = null,
        //            slm_PostalCode = null,
        //            slm_Occupation = prelead.slm_OccupationId,
        //            slm_BaseSalary = null,
        //            slm_IsCustomer = null,
        //            slm_CusCode = null,
        //            slm_Birthdate = prelead.slm_Birthdate,
        //            slm_CardType = prelead.slm_CardTypeId,
        //            slm_CitizenId = prelead.slm_CitizenId,
        //            slm_MaritalStatus = marital_status != 0 ? marital_status.ToString() : "",
        //            slm_Topic = null,
        //            slm_Detail = null,
        //            slm_PathLink = null,
        //            slm_ContactBranch = null,
        //            slm_CreatedBy = createByUsername,
        //            slm_CreatedDate = createDate,
        //            slm_UpdatedBy = createByUsername,
        //            slm_UpdatedDate = createDate,
        //            is_Deleted = 0
        //        };
        //        slmdb.kkslm_tr_cusinfo.AddObject(info);

        //        //Table kkslm_tr_productinfo
        //        kkslm_tr_productinfo prodInfo = new kkslm_tr_productinfo()
        //        {
        //            slm_TicketId = ticketId,
        //            slm_InterestedProd = null,
        //            slm_LicenseNo = prelead.slm_Car_License_No,
        //            slm_YearOfCar = prelead.slm_Model_Year,
        //            slm_YearOfCarRegis = null,
        //            slm_ProvinceRegis = prelead.slm_ProvinceRegis,
        //            slm_RedbookBrandCode = prelead.slm_Brand_Code,
        //            slm_RedbookBrandCodeExt = prelead.slm_Brand_Code_Org,
        //            slm_RedbookModelCode = prelead.slm_Model_Code,
        //            slm_RedbookModelCodeExt = prelead.slm_Model_Code_Org,
        //            slm_DownPayment = null,
        //            slm_DownPercent = null,
        //            slm_CarPrice = null,
        //            slm_FinanceAmt = null,
        //            slm_PaymentTerm = null,
        //            slm_PaymentType = null,
        //            slm_BalloonAmt = null,
        //            slm_BalloonPercent = null,
        //            slm_PlanType = null,
        //            slm_CoverageDate = null,
        //            slm_AccType = null,
        //            slm_AccPromotion = null,
        //            slm_AccTerm = null,
        //            slm_Interest = null,
        //            slm_Invest = null,
        //            slm_LoanOd = null,
        //            slm_LoanOdTerm = null,
        //            slm_Ebank = null,
        //            slm_Atm = null,
        //            slm_OtherDetail_1 = null,
        //            slm_OtherDetail_2 = null,
        //            slm_OtherDetail_3 = null,
        //            slm_OtherDetail_4 = null,
        //            is_Deleted = 0
        //        };
        //        if (!string.IsNullOrEmpty(prelead.slm_Model_Year))
        //            prodInfo.slm_RedbookYearGroup = int.Parse(prelead.slm_Model_Year);

        //        if (!string.IsNullOrEmpty(prelead.slm_Car_Category))
        //        {
        //            if (prelead.slm_Car_Category == "N")
        //                prodInfo.slm_CarType = "0";
        //            else if (prelead.slm_Car_Category == "U")
        //                prodInfo.slm_CarType = "1";
        //        }

        //        slmdb.kkslm_tr_productinfo.AddObject(prodInfo);

        //        //Table kkslm_tr_channelinfo
        //        kkslm_tr_channelinfo channel = new kkslm_tr_channelinfo()
        //        {
        //            slm_TicketId = ticketId,
        //            slm_ChannelId = creater != null ? creater.ChannelId : null,
        //            slm_IPAddress = null,
        //            slm_Company = null,
        //            slm_Branch = creater != null ? creater.BranchCode : null,
        //            slm_BranchNo = null,
        //            slm_MachineNo = null,
        //            slm_ClientServiceType = null,
        //            slm_DocumentNo = null,
        //            slm_CommPaidCode = null,
        //            slm_Zone = null,
        //            slm_RequestBy = createByUsername,
        //            slm_RequestDate = createDate
        //        };
        //        slmdb.kkslm_tr_channelinfo.AddObject(channel);

        //        //Table kkslm_tr_campaignfinal
        //        kkslm_tr_campaignfinal camfinal = new kkslm_tr_campaignfinal()
        //        {
        //            slm_TicketId = ticketId,
        //            slm_CampaignId = prelead.slm_CampaignId,
        //            slm_CampaignName = campaignName,
        //            slm_Description = campaignDesc,
        //            slm_CreatedBy = createByUsername,
        //            slm_CreatedBy_Position = creater != null ? creater.PositionId : null,
        //            slm_CreatedDate = createDate,
        //            slm_UpdatedBy = createByUsername,
        //            slm_UpdatedDate = createDate,
        //            is_Deleted = 0
        //        };
        //        slmdb.kkslm_tr_campaignfinal.AddObject(camfinal);

        //        //Table kkslm_tr_history
        //        kkslm_tr_history history = new kkslm_tr_history()
        //        {
        //            slm_ticketId = ticketId,
        //            slm_History_Type_Code = SLMConstant.HistoryTypeCode.CreateLead,
        //            slm_OldValue = null,
        //            slm_NewValue = null,
        //            slm_CreatedBy = createByUsername,
        //            slm_CreatedBy_Position = creater != null ? creater.PositionId : null,
        //            slm_CreateBy_Branch = creater != null ? creater.BranchCode : null,
        //            slm_CreatedDate = createDate,
        //            slm_UpdatedBy = createByUsername,
        //            slm_UpdatedDate = createDate,
        //            is_Deleted = false
        //        };
        //        slmdb.kkslm_tr_history.AddObject(history);

        //        //Table kkslm_tr_renewinsurance
        //        kkslm_tr_renewinsurance renew = new kkslm_tr_renewinsurance()
        //        {
        //            slm_TicketId = ticketId,
        //            slm_ContractNo = contractNo,
        //            slm_Grade = prelead.slm_Grade,
        //            slm_RedbookBrandCodeExt = prelead.slm_Brand_Code_Org,
        //            slm_RedbookModelCodeExt = prelead.slm_Model_Code_Org,
        //            slm_RedbookBrandCode = prelead.slm_Brand_Code,
        //            slm_RedbookModelCode = prelead.slm_Model_Code,
        //            slm_LicenseNo = prelead.slm_Car_License_No,
        //            slm_CC = prelead.slm_Cc,
        //            slm_ChassisNo = prelead.slm_Chassis_No,
        //            slm_EngineNo = prelead.slm_Engine_No,
        //            slm_SendDocFlag = prelead.slm_SendDocFlag,
        //            slm_SendDocBrandCode = prelead.slm_SendDocBrandCode,
        //            slm_Receiver = prelead.slm_Receiver,
        //            slm_PolicyDiscountAmt = 0,
        //            slm_PolicyGrossVat = 0,
        //            slm_PolicyGrossStamp = 0,
        //            slm_PolicyNetGrossPremium = 0,          //เบี้ยประกันภัยยังไม่รวมภาษีอากร
        //            slm_PolicyGrossPremiumTotal = 0,        //เบี้ยประกันภัยรวมภาษีอากรยังไม่หักส่วนลด
        //            slm_PolicyGrossPremium = 0,             //เบี้ยประกันภัยที่รวมภาษีอากรและหักส่วนลดแล้ว
        //            slm_PolicyCost = 0,
        //            slm_PolicyCostSave = 0,
        //            slm_Vat1PercentBath = 0,
        //            slm_DiscountPercent = 0,
        //            slm_ActVat = 0,
        //            slm_ActStamp = 0,
        //            slm_ActNetPremium = 0,                  //เบี้ยสุทธิไม่รวมภาษีอากร
        //            slm_ActGrossPremium = 0,                //เบี้ยที่ต้องชำระรวมภาษีอากรและหักส่วนลดแล้ว
        //            slm_RemarkPolicy = prelead.slm_RemarkPolicy,
        //            slm_RemarkAct = prelead.slm_RemarkAct,
        //            slm_CreatedBy = createByUsername,
        //            slm_CreatedDate = createDate,
        //            slm_UpdatedBy = createByUsername,
        //            slm_UpdatedDate = createDate
        //        };
        //        if (!string.IsNullOrEmpty(prelead.slm_Model_Year))
        //            renew.slm_RedbookYearGroup = int.Parse(prelead.slm_Model_Year);

        //        slmdb.kkslm_tr_renewinsurance.AddObject(renew);
        //        slmdb.SaveChanges();            //SaveChange เพื่อให้ได้ slm_RenewInsureId, นำไปใช้ในการ insert table kkslm_tr_renewinsurance_address
        //        decimal renewInsureId = renew.slm_RenewInsureId;

        //        //Table kkslm_tr_renewinsurance_address
        //        var addrList = slmdb.kkslm_tr_prelead_address.Where(p => p.slm_Prelead_Id == prelead.slm_Prelead_Id).ToList();
        //        foreach (kkslm_tr_prelead_address obj in addrList)
        //        {
        //            kkslm_tr_renewinsurance_address addr = new kkslm_tr_renewinsurance_address()
        //            {
        //                slm_RenewInsureId = renewInsureId,
        //                slm_AddressType = obj.slm_Address_Type,
        //                slm_AddressNo = obj.slm_House_No,
        //                slm_House_No = obj.slm_House_No,
        //                slm_BuildingName = obj.slm_Building,
        //                slm_Floor = null,
        //                slm_Soi = obj.slm_Soi,
        //                slm_Street = obj.slm_Street,
        //                slm_Tambon = obj.slm_TambolId,
        //                slm_Amphur = obj.slm_Amphur_Id,
        //                slm_Province = obj.slm_Province_Id,
        //                slm_PostalCode = obj.slm_Zipcode
        //            };
        //            slmdb.kkslm_tr_renewinsurance_address.AddObject(addr);
        //        }

        //        //Table kkslm_tr_renewinsurance_compare
        //        var compareList = slmdb.kkslm_tr_prelead_compare.Where(p => p.slm_Prelead_Id == prelead.slm_Prelead_Id).ToList();
        //        foreach (kkslm_tr_prelead_compare obj in compareList)
        //        {
        //            kkslm_tr_renewinsurance_compare comp = new kkslm_tr_renewinsurance_compare()
        //            {
        //                slm_RenewInsureId = renewInsureId,
        //                slm_NotifyPremiumId = obj.slm_NotifyPremiumId,
        //                slm_PromotionId = obj.slm_PromotionId,
        //                slm_Seq = obj.slm_Seq,
        //                slm_Ins_Com_Id = obj.slm_Ins_Com_Id,
        //                slm_CoverageTypeId = obj.slm_CoverageTypeId,
        //                slm_InjuryDeath = obj.slm_InjuryDeath,
        //                slm_TPPD = obj.slm_TPPD,
        //                slm_RepairTypeId = obj.slm_RepairTypeId,
        //                slm_OD = obj.slm_OD,
        //                slm_FT = obj.slm_FT,
        //                slm_DeDuctible = obj.slm_DeDuctible,
        //                slm_DeDuctibleFlag = obj.slm_DeDuctibleFlag,
        //                slm_PersonalAccident = obj.slm_PersonalAccident,
        //                slm_PersonalAccidentMan = obj.slm_PersonalAccidentMan,
        //                slm_MedicalFee = obj.slm_MedicalFee,
        //                slm_MedicalFeeMan = obj.slm_MedicalFeeMan,
        //                slm_InsuranceDriver = obj.slm_InsuranceDriver,
        //                slm_PolicyGrossStamp = obj.slm_PolicyGrossStamp,
        //                slm_PolicyGrossVat = obj.slm_PolicyGrossVat,
        //                slm_PolicyGrossPremium = obj.slm_PolicyGrossPremium,
        //                slm_NetGrossPremium = obj.slm_NetGrossPremium,
        //                slm_PolicyGrossPremiumPay = obj.slm_PolicyGrossPremiumPay,
        //                slm_CostSave = obj.slm_CostSave,
        //                slm_Selected = obj.slm_Selected,
        //                slm_OldPolicyNo = obj.slm_OldPolicyNo,
        //                slm_DriverFlag = obj.slm_DriverFlag,
        //                slm_Driver_TitleId1 = obj.slm_Driver_TitleId1,
        //                slm_Driver_First_Name1 = obj.slm_Driver_First_Name1,
        //                slm_Driver_Last_Name1 = obj.slm_Driver_Last_Name1,
        //                slm_Driver_Birthdate1 = obj.slm_Driver_Birthdate1,
        //                slm_Driver_TitleId2 = obj.slm_Driver_TitleId2,
        //                slm_Driver_First_Name2 = obj.slm_Driver_First_Name2,
        //                slm_Driver_Last_Name2 = obj.slm_Driver_Last_Name2,
        //                slm_Driver_Birthdate2 = obj.slm_Driver_Birthdate2,
        //                slm_OldReceiveNo = obj.slm_OldReceiveNo,
        //                slm_PolicyStartCoverDate = obj.slm_PolicyStartCoverDate,
        //                slm_PolicyEndCoverDate = obj.slm_PolicyEndCoverDate,
        //                slm_Vat1Percent = obj.slm_Vat1Percent,
        //                slm_DiscountPercent = obj.slm_DiscountPercent,
        //                slm_DiscountBath = obj.slm_DiscountBath,
        //                slm_Vat1PercentBath = obj.slm_Vat1PercentBath,
        //                slm_CreatedDate = createDate,
        //                slm_CreatedBy = createByUsername,
        //                slm_UpdatedDate = createDate,
        //                slm_UpdatedBy = createByUsername
        //            };
        //            slmdb.kkslm_tr_renewinsurance_compare.AddObject(comp);

        //            kkslm_tr_renewinsurance_compare_snap compSnap = new kkslm_tr_renewinsurance_compare_snap()
        //            {
        //                slm_RenewInsureId = renewInsureId,
        //                slm_NotifyPremiumId = obj.slm_NotifyPremiumId,
        //                slm_PromotionId = obj.slm_PromotionId,
        //                slm_Seq = obj.slm_Seq,
        //                slm_Ins_Com_Id = obj.slm_Ins_Com_Id,
        //                slm_CoverageTypeId = obj.slm_CoverageTypeId,
        //                slm_InjuryDeath = obj.slm_InjuryDeath,
        //                slm_TPPD = obj.slm_TPPD,
        //                slm_RepairTypeId = obj.slm_RepairTypeId,
        //                slm_OD = obj.slm_OD,
        //                slm_FT = obj.slm_FT,
        //                slm_DeDuctible = obj.slm_DeDuctible,
        //                slm_PersonalAccident = obj.slm_PersonalAccident,
        //                slm_PersonalAccidentMan = obj.slm_PersonalAccidentMan,
        //                slm_MedicalFee = obj.slm_MedicalFee,
        //                slm_MedicalFeeMan = obj.slm_MedicalFeeMan,
        //                slm_InsuranceDriver = obj.slm_InsuranceDriver,
        //                slm_PolicyGrossStamp = obj.slm_PolicyGrossStamp,
        //                slm_PolicyGrossVat = obj.slm_PolicyGrossVat,
        //                slm_PolicyGrossPremium = obj.slm_PolicyGrossPremium,
        //                slm_NetGrossPremium = obj.slm_NetGrossPremium,
        //                slm_PolicyGrossPremiumPay = obj.slm_PolicyGrossPremiumPay,
        //                slm_CostSave = obj.slm_CostSave,
        //                slm_Selected = obj.slm_Selected,
        //                slm_OldPolicyNo = obj.slm_OldPolicyNo,
        //                slm_DriverFlag = obj.slm_DriverFlag,
        //                slm_Driver_TitleId1 = obj.slm_Driver_TitleId1,
        //                slm_Driver_First_Name1 = obj.slm_Driver_First_Name1,
        //                slm_Driver_Last_Name1 = obj.slm_Driver_Last_Name1,
        //                slm_Driver_Birthdate1 = obj.slm_Driver_Birthdate1,
        //                slm_Driver_TitleId2 = obj.slm_Driver_TitleId2,
        //                slm_Driver_First_Name2 = obj.slm_Driver_First_Name2,
        //                slm_Driver_Last_Name2 = obj.slm_Driver_Last_Name2,
        //                slm_Driver_Birthdate2 = obj.slm_Driver_Birthdate2,
        //                slm_OldReceiveNo = obj.slm_OldReceiveNo,
        //                slm_PolicyStartCoverDate = obj.slm_PolicyStartCoverDate,
        //                slm_PolicyEndCoverDate = obj.slm_PolicyEndCoverDate,
        //                slm_Vat1Percent = obj.slm_Vat1Percent,
        //                slm_DiscountPercent = obj.slm_DiscountPercent,
        //                slm_DiscountBath = obj.slm_DiscountBath,
        //                slm_Vat1PercentBath = obj.slm_Vat1PercentBath,
        //                slm_CreatedDate = createDate,
        //                slm_CreatedBy = createByUsername,
        //                slm_UpdatedDate = createDate,
        //                slm_UpdatedBy = createByUsername,
        //                slm_Version = 1
        //            };
        //            slmdb.kkslm_tr_renewinsurance_compare_snap.AddObject(compSnap);

        //            //Update ข้อมูลโปรโมชั่นที่ถูกเลือก ไปที่ kkslm_tr_renewinsurance
        //            if (obj.slm_Selected == true)
        //            {
        //                renew.slm_CoverageTypeId = obj.slm_CoverageTypeId;
        //                renew.slm_InsuranceComId = obj.slm_Ins_Com_Id;
        //                renew.slm_PolicyStartCoverDate = obj.slm_PolicyStartCoverDate;
        //                renew.slm_PolicyEndCoverDate = obj.slm_PolicyEndCoverDate;
        //                renew.slm_PolicyDiscountAmt = obj.slm_DiscountBath != null ? obj.slm_DiscountBath : 0;
        //                renew.slm_PolicyGrossVat = obj.slm_PolicyGrossVat != null ? obj.slm_PolicyGrossVat : 0;
        //                renew.slm_PolicyGrossStamp = obj.slm_PolicyGrossStamp != null ? obj.slm_PolicyGrossStamp : 0;

        //                //เบี้ยประกันภัยยังไม่รวมภาษีอากร
        //                renew.slm_PolicyNetGrossPremium = obj.slm_PolicyGrossPremium != null ? obj.slm_PolicyGrossPremium : 0;
        //                //เบี้ยประกันภัยรวมภาษีอากรยังไม่หักส่วนลด
        //                renew.slm_PolicyGrossPremiumTotal = obj.slm_NetGrossPremium != null ? obj.slm_NetGrossPremium : 0;
        //                //เบี้ยประกันภัยที่รวมภาษีอากรและหักส่วนลดแล้ว
        //                renew.slm_PolicyGrossPremium = obj.slm_PolicyGrossPremiumPay != null ? obj.slm_PolicyGrossPremiumPay : 0;

        //                renew.slm_PolicyCost = obj.slm_FT != null ? obj.slm_FT : 0;
        //                renew.slm_RepairTypeId = obj.slm_RepairTypeId;
        //                renew.slm_PolicyCostSave = obj.slm_CostSave != null ? obj.slm_CostSave : 0;
        //                renew.slm_Vat1Percent = obj.slm_Vat1Percent;
        //                renew.slm_Vat1PercentBath = obj.slm_Vat1PercentBath != null ? obj.slm_Vat1PercentBath : 0;
        //                renew.slm_DiscountPercent = obj.slm_DiscountPercent != null ? obj.slm_DiscountPercent : 0;

        //                if (obj.slm_Vat1Percent == true) { renew.slm_Need_50TawiFlag = "Y"; }
        //                if (obj.slm_DriverFlag == "1") { renew.slm_Need_DriverLicenseFlag = "Y"; }
        //            }
        //        }

        //        //Table kkslm_tr_renewinsurance_compare_act
        //        var compActList = slmdb.kkslm_tr_prelead_compare_act.Where(p => p.slm_Prelead_Id == prelead.slm_Prelead_Id).ToList();
        //        foreach (kkslm_tr_prelead_compare_act obj in compActList)
        //        {
        //            kkslm_tr_renewinsurance_compare_act compAct = new kkslm_tr_renewinsurance_compare_act()
        //            {
        //                slm_RenewInsureId = renewInsureId,
        //                slm_PromotionId = obj.slm_PromotionId,
        //                slm_Seq = obj.slm_Seq,
        //                slm_Year = obj.slm_Year,
        //                slm_Ins_Com_Id = obj.slm_Ins_Com_Id,
        //                slm_ActIssuePlace = obj.slm_ActIssuePlace,
        //                slm_SendDocType = obj.slm_SendDocType,
        //                slm_ActNo = obj.slm_ActNo,
        //                slm_ActStartCoverDate = obj.slm_ActStartCoverDate,
        //                slm_ActEndCoverDate = obj.slm_ActEndCoverDate,
        //                slm_ActIssueBranch = obj.slm_ActIssueBranch,
        //                slm_CarTaxExpiredDate = obj.slm_CarTaxExpiredDate,
        //                slm_ActGrossStamp = obj.slm_ActGrossStamp,
        //                slm_ActGrossVat = obj.slm_ActGrossVat,
        //                slm_ActGrossPremium = obj.slm_ActGrossPremium,
        //                slm_ActNetGrossPremium = obj.slm_ActNetGrossPremium,
        //                slm_ActGrossPremiumPay = obj.slm_ActGrossPremiumPay,
        //                slm_ActPurchaseFlag = obj.slm_ActPurchaseFlag,
        //                slm_DiscountPercent = obj.slm_DiscountPercent,
        //                slm_DiscountBath = obj.slm_DiscountBath,
        //                slm_ActSignNo = obj.slm_ActSignNo,
        //                slm_ActNetGrossPremiumFull = obj.slm_ActNetGrossPremiumFull,
        //                slm_Vat1Percent = obj.slm_Vat1Percent,
        //                slm_Vat1PercentBath = obj.slm_Vat1PercentBath,
        //                slm_CreatedDate = createDate,
        //                slm_CreatedBy = createByUsername,
        //                slm_UpdatedDate = createDate,
        //                slm_UpdatedBy = createByUsername
        //            };
        //            slmdb.kkslm_tr_renewinsurance_compare_act.AddObject(compAct);

        //            kkslm_tr_renewinsurance_compare_act_snap actSnap = new kkslm_tr_renewinsurance_compare_act_snap()
        //            {
        //                slm_RenewInsureId = renewInsureId,
        //                slm_PromotionId = obj.slm_PromotionId,
        //                slm_Seq = obj.slm_Seq,
        //                slm_Year = obj.slm_Year,
        //                slm_Ins_Com_Id = obj.slm_Ins_Com_Id,
        //                slm_ActIssuePlace = obj.slm_ActIssuePlace,
        //                slm_SendDocType = obj.slm_SendDocType,
        //                slm_ActNo = obj.slm_ActNo,
        //                slm_ActStartCoverDate = obj.slm_ActStartCoverDate,
        //                slm_ActEndCoverDate = obj.slm_ActEndCoverDate,
        //                slm_ActIssueBranch = obj.slm_ActIssueBranch,
        //                slm_CarTaxExpiredDate = obj.slm_CarTaxExpiredDate,
        //                slm_ActGrossStamp = obj.slm_ActGrossStamp,
        //                slm_ActGrossVat = obj.slm_ActGrossVat,
        //                slm_ActGrossPremium = obj.slm_ActGrossPremium,
        //                slm_ActNetGrossPremium = obj.slm_ActNetGrossPremium,
        //                slm_ActGrossPremiumPay = obj.slm_ActGrossPremiumPay,
        //                slm_ActPurchaseFlag = obj.slm_ActPurchaseFlag,
        //                slm_DiscountPercent = obj.slm_DiscountPercent,
        //                slm_DiscountBath = obj.slm_DiscountBath,
        //                slm_ActSignNo = obj.slm_ActSignNo,
        //                slm_ActNetGrossPremiumFull = obj.slm_ActNetGrossPremiumFull,
        //                slm_Vat1Percent = obj.slm_Vat1Percent,
        //                slm_Vat1PercentBath = obj.slm_Vat1PercentBath,
        //                slm_CreatedDate = createDate,
        //                slm_CreatedBy = createByUsername,
        //                slm_UpdatedDate = createDate,
        //                slm_UpdatedBy = createByUsername,
        //                slm_Version = 1
        //            };
        //            slmdb.kkslm_tr_renewinsurance_compare_act_snap.AddObject(actSnap);

        //            //Update ข้อมูล พรบ ที่ถูกเลือกไปที่ kkslm_tr_renewinsurance
        //            if (obj.slm_ActPurchaseFlag == true)
        //            {
        //                renew.slm_ActStartCoverDate = obj.slm_ActStartCoverDate;
        //                renew.slm_ActEndCoverDate = obj.slm_ActEndCoverDate;
        //                renew.slm_ActComId = obj.slm_Ins_Com_Id;
        //                renew.slm_ActVat = obj.slm_ActGrossVat != null ? obj.slm_ActGrossVat : 0;
        //                renew.slm_ActStamp = obj.slm_ActGrossStamp != null ? obj.slm_ActGrossStamp : 0;

        //                //เบี้ยสุทธิไม่รวมภาษีอากร
        //                renew.slm_ActNetPremium = obj.slm_ActGrossPremium;
        //                //เบี้ยที่ต้องชำระรวมภาษีอากรและหักส่วนลดแล้ว
        //                renew.slm_ActGrossPremium = obj.slm_ActGrossPremiumPay != null ? obj.slm_ActGrossPremiumPay : 0;
        //                renew.slm_ActDiscountAmt = obj.slm_DiscountBath != null ? obj.slm_DiscountBath : 0;
        //                renew.slm_ActDiscountPercent = obj.slm_DiscountPercent != null ? obj.slm_DiscountPercent : 0;

        //                if (obj.slm_Vat1Percent == true) { renew.slm_Need_50TawiFlag = "Y"; }
        //            }
        //        }

        //        return ticketId;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private void ConvertToLead(kkslm_tr_prelead prelead, kkslm_ms_staff owner, StaffData creater, string createByUsername, DateTime createDate, string telNo1, string contractNo, string newSubStatusCode, string newSubStatusName, SLM_DBEntities slmdb)
        {
            try
            {
                string campaignName = "";
                string campaignDesc = "";
                string statusCode = SLMConstant.StatusCode.Interest;   //สนใจ
                string subStatusCode = "";
                string subStatusDesc = "";

                //KKSlmMsConfigProductSubstatusModel model = new KKSlmMsConfigProductSubstatusModel();
                var subStatusList = GetSubStatusList(prelead.slm_Product_Id, prelead.slm_CampaignId, statusCode);
                if (newSubStatusCode == SLMConstant.SubStatusCode.ActPurchased)
                {
                    subStatusCode = subStatusList.Where(p => p.ValueField == "15").Select(p => p.ValueField).FirstOrDefault();
                    subStatusDesc = subStatusList.Where(p => p.ValueField == "15").Select(p => p.TextField).FirstOrDefault();
                }
                else if (newSubStatusCode == SLMConstant.SubStatusCode.AcceptRenew)
                {
                    subStatusCode = subStatusList.Where(p => p.ValueField == "16").Select(p => p.ValueField).FirstOrDefault();
                    subStatusDesc = subStatusList.Where(p => p.ValueField == "16").Select(p => p.TextField).FirstOrDefault();
                }

                StoreProcedure store = new StoreProcedure();
                string ticketId = store.GenerateTicketId(slmdb);

                prelead.slm_TicketId = ticketId;

                //Table kkslm_tr_lead
                kkslm_tr_lead lead = new kkslm_tr_lead()
                {
                    slm_ticketId = ticketId,
                    slm_TitleId = prelead.slm_TitleId,
                    slm_Name = prelead.slm_Name,
                    slm_LastName = prelead.slm_LastName,
                    slm_TelNo_1 = telNo1,
                    slm_CampaignId = prelead.slm_CampaignId,
                    slm_Owner_Branch = owner != null ? owner.slm_BranchCode : null,
                    slm_Owner = owner != null ? owner.slm_UserName : null,
                    slm_Owner_Position = owner != null ? owner.slm_Position_id : null,
                    slm_Delegate = null,
                    slm_Status = statusCode,
                    slm_SubStatus = subStatusCode,
                    slm_StatusBy = createByUsername,
                    slm_StatusDate = createDate,
                    slm_StatusDateSource = createDate,
                    slm_ExternalSubStatusDesc = subStatusDesc,
                    slm_AvailableTime = null,
                    slm_Counting = 0,
                    slm_ChannelId = creater != null ? creater.ChannelId : null,
                    slm_CreatedBy = createByUsername,
                    slm_CreatedBy_Position = creater != null ? creater.PositionId : null,
                    slm_CreatedDate = createDate,
                    slm_CreatedBy_Branch = creater != null ? creater.BranchCode : null,
                    slm_UpdatedBy = createByUsername,
                    slm_UpdatedDate = createDate,
                    slm_AssignedFlag = "0",
                    slm_Product_Group_Id = "",
                    slm_Product_Id = prelead.slm_Product_Id,
                    slm_Product_Name = "",
                    slm_ThisWork = 0,
                    slm_TotalAlert = 0,
                    slm_TotalWork = 0,
                    slm_NextContactDate = prelead.slm_NextContactDate,
                    is_Deleted = 0
                };
                if (owner != null) lead.slm_StaffId = owner.slm_StaffId;
                //CmtCampaignProductModel campaign = new CmtCampaignProductModel();
                List<ProductData> list = GetDataByCampaignId(prelead.slm_CampaignId);
                if (list.Count > 0)
                {
                    lead.slm_Product_Group_Id = list[0].ProductGroupId;
                    lead.slm_Product_Id = list[0].ProductId;
                    lead.slm_Product_Name = list[0].ProductName;
                    campaignName = list[0].CampaignName;
                    campaignDesc = list[0].CampaignDesc;
                }
                slmdb.kkslm_tr_lead.AddObject(lead);

                //Table kkslm_tr_cusinfo
                int marital_status = slmdb.kkslm_ms_marital_status.Where(p => p.slm_MaritalStatusName == prelead.slm_Marital_Status).Select(p => p.slm_MaritalStatusId).FirstOrDefault();
                kkslm_tr_cusinfo info = new kkslm_tr_cusinfo()
                {
                    slm_TicketId = ticketId,
                    slm_LastName = prelead.slm_LastName,
                    slm_Email = prelead.slm_Email,
                    slm_TelNo_2 = null,
                    slm_TelNo_3 = null,
                    slm_Ext_2 = null,
                    slm_Ext_3 = null,
                    slm_BuildingName = null,
                    slm_AddressNo = null,
                    slm_Floor = null,
                    slm_Soi = null,
                    slm_Street = null,
                    slm_Tambon = null,
                    slm_Amphur = null,
                    slm_Province = null,
                    slm_PostalCode = null,
                    slm_Occupation = prelead.slm_OccupationId,
                    slm_BaseSalary = null,
                    slm_IsCustomer = null,
                    slm_CusCode = null,
                    slm_Birthdate = prelead.slm_Birthdate,
                    slm_CardType = prelead.slm_CardTypeId,
                    slm_CitizenId = prelead.slm_CitizenId,
                    slm_MaritalStatus = marital_status != 0 ? marital_status.ToString() : "",
                    slm_Topic = null,
                    slm_Detail = null,
                    slm_PathLink = null,
                    slm_ContactBranch = null,
                    slm_TelNoSms = string.IsNullOrWhiteSpace(prelead.slm_TelNoSms) ? telNo1 : prelead.slm_TelNoSms,
                    slm_CreatedBy = createByUsername,
                    slm_CreatedDate = createDate,
                    slm_UpdatedBy = createByUsername,
                    slm_UpdatedDate = createDate,
                    is_Deleted = 0
                };
                slmdb.kkslm_tr_cusinfo.AddObject(info);

                //Table kkslm_tr_productinfo
                kkslm_tr_productinfo prodInfo = new kkslm_tr_productinfo()
                {
                    slm_TicketId = ticketId,
                    slm_InterestedProd = null,
                    slm_LicenseNo = prelead.slm_Car_License_No,
                    slm_YearOfCar = prelead.slm_Model_Year,
                    slm_YearOfCarRegis = null,
                    slm_ProvinceRegis = prelead.slm_ProvinceRegis,
                    slm_RedbookBrandCode = prelead.slm_Brand_Code,
                    slm_RedbookBrandCodeExt = prelead.slm_Brand_Code_Org,
                    slm_RedbookModelCode = prelead.slm_Model_Code,
                    slm_RedbookModelCodeExt = prelead.slm_Model_Code_Org,
                    slm_DownPayment = null,
                    slm_DownPercent = null,
                    slm_CarPrice = null,
                    slm_FinanceAmt = null,
                    slm_PaymentTerm = null,
                    slm_PaymentType = null,
                    slm_BalloonAmt = null,
                    slm_BalloonPercent = null,
                    slm_PlanType = null,
                    slm_CoverageDate = null,
                    slm_AccType = null,
                    slm_AccPromotion = null,
                    slm_AccTerm = null,
                    slm_Interest = null,
                    slm_Invest = null,
                    slm_LoanOd = null,
                    slm_LoanOdTerm = null,
                    slm_Ebank = null,
                    slm_Atm = null,
                    slm_OtherDetail_1 = null,
                    slm_OtherDetail_2 = null,
                    slm_OtherDetail_3 = null,
                    slm_OtherDetail_4 = null,
                    is_Deleted = 0
                };
                if (!string.IsNullOrEmpty(prelead.slm_Model_Year))
                    prodInfo.slm_RedbookYearGroup = int.Parse(prelead.slm_Model_Year);

                if (!string.IsNullOrEmpty(prelead.slm_Car_Category))
                {
                    if (prelead.slm_Car_Category == "N")
                        prodInfo.slm_CarType = "0";
                    else if (prelead.slm_Car_Category == "U")
                        prodInfo.slm_CarType = "1";
                }

                slmdb.kkslm_tr_productinfo.AddObject(prodInfo);

                //Table kkslm_tr_channelinfo
                kkslm_tr_channelinfo channel = new kkslm_tr_channelinfo()
                {
                    slm_TicketId = ticketId,
                    slm_ChannelId = creater != null ? creater.ChannelId : null,
                    slm_IPAddress = null,
                    slm_Company = null,
                    slm_Branch = creater != null ? creater.BranchCode : null,
                    slm_BranchNo = null,
                    slm_MachineNo = null,
                    slm_ClientServiceType = null,
                    slm_DocumentNo = null,
                    slm_CommPaidCode = null,
                    slm_Zone = null,
                    slm_RequestBy = createByUsername,
                    slm_RequestDate = createDate
                };
                slmdb.kkslm_tr_channelinfo.AddObject(channel);

                //Table kkslm_tr_campaignfinal
                kkslm_tr_campaignfinal camfinal = new kkslm_tr_campaignfinal()
                {
                    slm_TicketId = ticketId,
                    slm_CampaignId = prelead.slm_CampaignId,
                    slm_CampaignName = campaignName,
                    slm_Description = campaignDesc,
                    slm_CreatedBy = createByUsername,
                    slm_CreatedBy_Position = creater != null ? creater.PositionId : null,
                    slm_CreatedDate = createDate,
                    slm_UpdatedBy = createByUsername,
                    slm_UpdatedDate = createDate,
                    is_Deleted = 0
                };
                slmdb.kkslm_tr_campaignfinal.AddObject(camfinal);

                //Table kkslm_tr_history
                kkslm_tr_history history = new kkslm_tr_history()
                {
                    slm_ticketId = ticketId,
                    slm_History_Type_Code = SLMConstant.HistoryTypeCode.CreateLead,
                    slm_OldValue = null,
                    slm_NewValue = null,
                    slm_CreatedBy = createByUsername,
                    slm_CreatedBy_Position = creater != null ? creater.PositionId : null,
                    slm_CreateBy_Branch = creater != null ? creater.BranchCode : null,
                    slm_CreatedDate = createDate,
                    slm_UpdatedBy = createByUsername,
                    slm_UpdatedDate = createDate,
                    is_Deleted = false
                };
                slmdb.kkslm_tr_history.AddObject(history);

                //Table kkslm_tr_renewinsurance
                kkslm_tr_renewinsurance renew = new kkslm_tr_renewinsurance()
                {
                    slm_TicketId = ticketId,
                    slm_ContractNo = contractNo,
                    slm_Grade = prelead.slm_Grade,
                    slm_RedbookBrandCodeExt = prelead.slm_Brand_Code_Org,
                    slm_RedbookModelCodeExt = prelead.slm_Model_Code_Org,
                    slm_RedbookBrandCode = prelead.slm_Brand_Code,
                    slm_RedbookModelCode = prelead.slm_Model_Code,
                    slm_LicenseNo = prelead.slm_Car_License_No,
                    slm_CC = prelead.slm_Cc,
                    slm_ChassisNo = prelead.slm_Chassis_No,
                    slm_EngineNo = prelead.slm_Engine_No,
                    slm_SendDocFlag = prelead.slm_SendDocFlag,
                    slm_SendDocBrandCode = prelead.slm_SendDocBrandCode,
                    slm_Receiver = prelead.slm_Receiver,
                    slm_PolicyDiscountAmt = 0,
                    slm_PolicyGrossVat = 0,
                    slm_PolicyGrossStamp = 0,
                    slm_PolicyNetGrossPremium = 0,          //เบี้ยประกันภัยยังไม่รวมภาษีอากร
                    slm_PolicyGrossPremiumTotal = 0,        //เบี้ยประกันภัยรวมภาษีอากรยังไม่หักส่วนลด
                    slm_PolicyGrossPremium = 0,             //เบี้ยประกันภัยที่รวมภาษีอากรและหักส่วนลดแล้ว
                    slm_PolicyCost = 0,
                    slm_PolicyCostSave = 0,
                    slm_Vat1PercentBath = 0,
                    slm_DiscountPercent = 0,
                    slm_ActVat = 0,
                    slm_ActStamp = 0,
                    slm_ActNetPremium = 0,                  //เบี้ยสุทธิไม่รวมภาษีอากร
                    slm_ActGrossPremium = 0,                //เบี้ยที่ต้องชำระรวมภาษีอากรและหักส่วนลดแล้ว
                    slm_RemarkPolicy = prelead.slm_RemarkPolicy,
                    slm_RemarkAct = prelead.slm_RemarkAct,
                    slm_InsurancecarTypeId = prelead.slm_Car_By_Gov_Id, // cartype fix by zz 2017-03-21
                    slm_PeriodMonth = prelead.slm_PeriodMonth,
                    slm_PeriodYear = prelead.slm_PeriodYear,
                    slm_Reference1 = prelead.slm_Reference1,
                    slm_Reference2 = prelead.slm_Reference2,
                    slm_BeneficiaryId = 1,
                    slm_CreatedBy = createByUsername,
                    slm_CreatedDate = createDate,
                    slm_UpdatedBy = createByUsername,
                    slm_UpdatedDate = createDate
                };

                if (!string.IsNullOrEmpty(prelead.slm_Model_Year))
                    renew.slm_RedbookYearGroup = int.Parse(prelead.slm_Model_Year);

                slmdb.kkslm_tr_renewinsurance.AddObject(renew);
                //slmdb.SaveChanges();            //SaveChange เพื่อให้ได้ slm_RenewInsureId, นำไปใช้ในการ insert table kkslm_tr_renewinsurance_address
                //decimal renewInsureId = renew.slm_RenewInsureId;

                List<kkslm_tr_renewinsurance_address> addrInsList = new List<kkslm_tr_renewinsurance_address>();
                List<kkslm_tr_renewinsurance_compare> compInsList = new List<kkslm_tr_renewinsurance_compare>();
                List<kkslm_tr_renewinsurance_compare_snap> compSnapInsList = new List<kkslm_tr_renewinsurance_compare_snap>();
                List<kkslm_tr_renewinsurance_compare_act> compActInsList = new List<kkslm_tr_renewinsurance_compare_act>();
                List<kkslm_tr_renewinsurance_compare_act_snap> compActSnapInsList = new List<kkslm_tr_renewinsurance_compare_act_snap>();

                //Table kkslm_tr_renewinsurance_address
                var addrList = slmdb.kkslm_tr_prelead_address.Where(p => p.slm_Prelead_Id == prelead.slm_Prelead_Id).ToList();
                foreach (kkslm_tr_prelead_address obj in addrList)
                {
                    kkslm_tr_renewinsurance_address addr = new kkslm_tr_renewinsurance_address()
                    {
                        //slm_RenewInsureId = renewInsureId,
                        slm_AddressType = obj.slm_Address_Type,
                        slm_AddressNo = obj.slm_House_No,
                        slm_House_No = obj.slm_House_No,
                        slm_BuildingName = obj.slm_Building,
                        slm_Floor = null,
                        slm_Soi = obj.slm_Soi,
                        slm_Street = obj.slm_Street,
                        slm_Tambon = obj.slm_TambolId,
                        slm_Amphur = obj.slm_Amphur_Id,
                        slm_Province = obj.slm_Province_Id,
                        slm_PostalCode = obj.slm_Zipcode

                        // -- add 2017-02-24 zz
                        ,
                        slm_Moo = obj.slm_Moo
                        ,
                        slm_Village = obj.slm_Village
                    };
                    addrInsList.Add(addr);
                    slmdb.kkslm_tr_renewinsurance_address.AddObject(addr);
                }

                //Table kkslm_tr_renewinsurance_compare
                var compareList = slmdb.kkslm_tr_prelead_compare.Where(p => p.slm_Prelead_Id == prelead.slm_Prelead_Id).ToList();
                foreach (kkslm_tr_prelead_compare obj in compareList)
                {
                    kkslm_tr_renewinsurance_compare comp = new kkslm_tr_renewinsurance_compare()
                    {
                        //slm_RenewInsureId = renewInsureId,
                        slm_NotifyPremiumId = obj.slm_NotifyPremiumId,
                        slm_PromotionId = obj.slm_PromotionId,
                        slm_Seq = obj.slm_Seq,
                        slm_Year = obj.slm_Year,
                        slm_Ins_Com_Id = obj.slm_Ins_Com_Id,
                        slm_CoverageTypeId = obj.slm_CoverageTypeId,
                        slm_InjuryDeath = obj.slm_InjuryDeath,
                        slm_TPPD = obj.slm_TPPD,
                        slm_RepairTypeId = obj.slm_RepairTypeId,
                        slm_OD = obj.slm_OD,
                        slm_FT = obj.slm_FT,
                        slm_DeDuctible = obj.slm_DeDuctible,
                        slm_DeDuctibleFlag = obj.slm_DeDuctibleFlag,
                        slm_PersonalAccident = obj.slm_PersonalAccident,
                        slm_PersonalAccidentMan = obj.slm_PersonalAccidentMan,
                        slm_MedicalFee = obj.slm_MedicalFee,
                        slm_MedicalFeeMan = obj.slm_MedicalFeeMan,
                        slm_InsuranceDriver = obj.slm_InsuranceDriver,
                        slm_PolicyGrossStamp = obj.slm_PolicyGrossStamp,
                        slm_PolicyGrossVat = obj.slm_PolicyGrossVat,
                        slm_PolicyGrossPremium = obj.slm_PolicyGrossPremium,
                        slm_NetGrossPremium = obj.slm_NetGrossPremium,
                        slm_PolicyGrossPremiumPay = obj.slm_PolicyGrossPremiumPay,
                        slm_CostSave = obj.slm_CostSave,
                        slm_Selected = obj.slm_Selected,
                        slm_OldPolicyNo = obj.slm_OldPolicyNo,
                        slm_DriverFlag = obj.slm_DriverFlag,
                        slm_Driver_TitleId1 = obj.slm_Driver_TitleId1,
                        slm_Driver_First_Name1 = obj.slm_Driver_First_Name1,
                        slm_Driver_Last_Name1 = obj.slm_Driver_Last_Name1,
                        slm_Driver_Birthdate1 = obj.slm_Driver_Birthdate1,
                        slm_Driver_TitleId2 = obj.slm_Driver_TitleId2,
                        slm_Driver_First_Name2 = obj.slm_Driver_First_Name2,
                        slm_Driver_Last_Name2 = obj.slm_Driver_Last_Name2,
                        slm_Driver_Birthdate2 = obj.slm_Driver_Birthdate2,
                        slm_OldReceiveNo = obj.slm_OldReceiveNo,
                        slm_PolicyStartCoverDate = obj.slm_PolicyStartCoverDate,
                        slm_PolicyEndCoverDate = obj.slm_PolicyEndCoverDate,
                        slm_Vat1Percent = obj.slm_Vat1Percent,
                        slm_DiscountPercent = obj.slm_DiscountPercent,
                        slm_DiscountBath = obj.slm_DiscountBath,
                        slm_Vat1PercentBath = obj.slm_Vat1PercentBath,
                        slm_CreatedDate = createDate,
                        slm_CreatedBy = createByUsername,
                        slm_UpdatedDate = createDate,
                        slm_UpdatedBy = createByUsername
                    };
                    compInsList.Add(comp);
                    slmdb.kkslm_tr_renewinsurance_compare.AddObject(comp);
                    if ((comp.slm_Selected ?? false) == true)
                        renew.slm_CoverageTypeId = comp.slm_CoverageTypeId;

                    kkslm_tr_renewinsurance_compare_snap compSnap = new kkslm_tr_renewinsurance_compare_snap()
                    {
                        //slm_RenewInsureId = renewInsureId,
                        slm_NotifyPremiumId = obj.slm_NotifyPremiumId,
                        slm_PromotionId = obj.slm_PromotionId,
                        slm_Seq = obj.slm_Seq,
                        slm_Ins_Com_Id = obj.slm_Ins_Com_Id,
                        slm_CoverageTypeId = obj.slm_CoverageTypeId,
                        slm_InjuryDeath = obj.slm_InjuryDeath,
                        slm_TPPD = obj.slm_TPPD,
                        slm_RepairTypeId = obj.slm_RepairTypeId,
                        slm_OD = obj.slm_OD,
                        slm_FT = obj.slm_FT,
                        slm_DeDuctible = obj.slm_DeDuctible,
                        slm_DeDuctibleFlag = obj.slm_DeDuctibleFlag,
                        slm_PersonalAccident = obj.slm_PersonalAccident,
                        slm_PersonalAccidentMan = obj.slm_PersonalAccidentMan,
                        slm_MedicalFee = obj.slm_MedicalFee,
                        slm_MedicalFeeMan = obj.slm_MedicalFeeMan,
                        slm_InsuranceDriver = obj.slm_InsuranceDriver,
                        slm_PolicyGrossStamp = obj.slm_PolicyGrossStamp,
                        slm_PolicyGrossVat = obj.slm_PolicyGrossVat,
                        slm_PolicyGrossPremium = obj.slm_PolicyGrossPremium,
                        slm_NetGrossPremium = obj.slm_NetGrossPremium,
                        slm_PolicyGrossPremiumPay = obj.slm_PolicyGrossPremiumPay,
                        slm_CostSave = obj.slm_CostSave,
                        slm_Selected = obj.slm_Selected,
                        slm_OldPolicyNo = obj.slm_OldPolicyNo,
                        slm_DriverFlag = obj.slm_DriverFlag,
                        slm_Driver_TitleId1 = obj.slm_Driver_TitleId1,
                        slm_Driver_First_Name1 = obj.slm_Driver_First_Name1,
                        slm_Driver_Last_Name1 = obj.slm_Driver_Last_Name1,
                        slm_Driver_Birthdate1 = obj.slm_Driver_Birthdate1,
                        slm_Driver_TitleId2 = obj.slm_Driver_TitleId2,
                        slm_Driver_First_Name2 = obj.slm_Driver_First_Name2,
                        slm_Driver_Last_Name2 = obj.slm_Driver_Last_Name2,
                        slm_Driver_Birthdate2 = obj.slm_Driver_Birthdate2,
                        slm_OldReceiveNo = obj.slm_OldReceiveNo,
                        slm_PolicyStartCoverDate = obj.slm_PolicyStartCoverDate,
                        slm_PolicyEndCoverDate = obj.slm_PolicyEndCoverDate,
                        slm_Vat1Percent = obj.slm_Vat1Percent,
                        slm_DiscountPercent = obj.slm_DiscountPercent,
                        slm_DiscountBath = obj.slm_DiscountBath,
                        slm_Vat1PercentBath = obj.slm_Vat1PercentBath,
                        slm_CreatedDate = createDate,
                        slm_CreatedBy = createByUsername,
                        slm_UpdatedDate = createDate,
                        slm_UpdatedBy = createByUsername,
                        slm_Version = 1
                    };
                    compSnapInsList.Add(compSnap);
                    slmdb.kkslm_tr_renewinsurance_compare_snap.AddObject(compSnap);

                    //Update ข้อมูลโปรโมชั่นที่ถูกเลือก ไปที่ kkslm_tr_renewinsurance
                    if (obj.slm_Selected == true)
                    {
                        //renew.slm_CoverageTypeId = prelead.slm_Car_By_Gov_Id;// modify by z 2017-03-08 obj.slm_CoverageTypeId;
                        renew.slm_InsuranceComId = obj.slm_Ins_Com_Id;
                        renew.slm_PolicyStartCoverDate = obj.slm_PolicyStartCoverDate;
                        renew.slm_PolicyEndCoverDate = obj.slm_PolicyEndCoverDate;
                        renew.slm_PolicyDiscountAmt = obj.slm_DiscountBath != null ? obj.slm_DiscountBath : 0;
                        renew.slm_PolicyGrossVat = obj.slm_PolicyGrossVat != null ? obj.slm_PolicyGrossVat : 0;
                        renew.slm_PolicyGrossStamp = obj.slm_PolicyGrossStamp != null ? obj.slm_PolicyGrossStamp : 0;

                        //เบี้ยประกันภัยยังไม่รวมภาษีอากร
                        renew.slm_PolicyNetGrossPremium = obj.slm_PolicyGrossPremium != null ? obj.slm_PolicyGrossPremium : 0;
                        //เบี้ยประกันภัยรวมภาษีอากรยังไม่หักส่วนลด
                        renew.slm_PolicyGrossPremiumTotal = obj.slm_NetGrossPremium != null ? obj.slm_NetGrossPremium : 0;
                        //เบี้ยประกันภัยที่รวมภาษีอากรและหักส่วนลดแล้ว
                        renew.slm_PolicyGrossPremium = obj.slm_PolicyGrossPremiumPay != null ? obj.slm_PolicyGrossPremiumPay : 0;

                        renew.slm_PolicyCost = obj.slm_FT != null ? obj.slm_FT : 0;
                        renew.slm_RepairTypeId = obj.slm_RepairTypeId;
                        renew.slm_PolicyCostSave = obj.slm_CostSave != null ? obj.slm_CostSave : 0;
                        renew.slm_Vat1Percent = obj.slm_Vat1Percent;
                        renew.slm_Vat1PercentBath = obj.slm_Vat1PercentBath != null ? obj.slm_Vat1PercentBath : 0;
                        renew.slm_DiscountPercent = obj.slm_DiscountPercent != null ? obj.slm_DiscountPercent : 0;

                        if (obj.slm_Vat1Percent == true) { renew.slm_Need_50TawiFlag = "Y"; }
                        if (obj.slm_DriverFlag == "1") { renew.slm_Need_DriverLicenseFlag = "Y"; }
                    }
                }

                //Table kkslm_tr_renewinsurance_compare_act
                var compActList = slmdb.kkslm_tr_prelead_compare_act.Where(p => p.slm_Prelead_Id == prelead.slm_Prelead_Id).ToList();
                foreach (kkslm_tr_prelead_compare_act obj in compActList)
                {
                    kkslm_tr_renewinsurance_compare_act compAct = new kkslm_tr_renewinsurance_compare_act()
                    {
                        //slm_RenewInsureId = renewInsureId,
                        slm_PromotionId = obj.slm_PromotionId,
                        slm_Seq = obj.slm_Seq,
                        slm_Year = obj.slm_Year,
                        slm_Ins_Com_Id = obj.slm_Ins_Com_Id,
                        slm_ActIssuePlace = obj.slm_ActIssuePlace,
                        slm_SendDocType = obj.slm_SendDocType,
                        slm_ActNo = obj.slm_ActNo,
                        slm_ActStartCoverDate = obj.slm_ActStartCoverDate,
                        slm_ActEndCoverDate = obj.slm_ActEndCoverDate,
                        slm_ActIssueBranch = obj.slm_ActIssueBranch,
                        slm_CarTaxExpiredDate = obj.slm_CarTaxExpiredDate,
                        slm_ActGrossStamp = obj.slm_ActGrossStamp,
                        slm_ActGrossVat = obj.slm_ActGrossVat,
                        slm_ActGrossPremium = obj.slm_ActGrossPremium,
                        slm_ActNetGrossPremium = obj.slm_ActNetGrossPremium,
                        slm_ActGrossPremiumPay = obj.slm_ActGrossPremiumPay,
                        slm_ActPurchaseFlag = obj.slm_ActPurchaseFlag,
                        slm_DiscountPercent = obj.slm_DiscountPercent,
                        slm_DiscountBath = obj.slm_DiscountBath,
                        slm_ActSignNo = obj.slm_ActSignNo,
                        slm_ActNetGrossPremiumFull = obj.slm_ActNetGrossPremiumFull,
                        slm_Vat1Percent = obj.slm_Vat1Percent,
                        slm_Vat1PercentBath = obj.slm_Vat1PercentBath,
                        slm_CreatedDate = createDate,
                        slm_CreatedBy = createByUsername,
                        slm_UpdatedDate = createDate,
                        slm_UpdatedBy = createByUsername
                    };
                    compActInsList.Add(compAct);
                    slmdb.kkslm_tr_renewinsurance_compare_act.AddObject(compAct);

                    kkslm_tr_renewinsurance_compare_act_snap actSnap = new kkslm_tr_renewinsurance_compare_act_snap()
                    {
                        //slm_RenewInsureId = renewInsureId,
                        slm_PromotionId = obj.slm_PromotionId,
                        slm_Seq = obj.slm_Seq,
                        slm_Year = obj.slm_Year,
                        slm_Ins_Com_Id = obj.slm_Ins_Com_Id,
                        slm_ActIssuePlace = obj.slm_ActIssuePlace,
                        slm_SendDocType = obj.slm_SendDocType,
                        slm_ActNo = obj.slm_ActNo,
                        slm_ActStartCoverDate = obj.slm_ActStartCoverDate,
                        slm_ActEndCoverDate = obj.slm_ActEndCoverDate,
                        slm_ActIssueBranch = obj.slm_ActIssueBranch,
                        slm_CarTaxExpiredDate = obj.slm_CarTaxExpiredDate,
                        slm_ActGrossStamp = obj.slm_ActGrossStamp,
                        slm_ActGrossVat = obj.slm_ActGrossVat,
                        slm_ActGrossPremium = obj.slm_ActGrossPremium,
                        slm_ActNetGrossPremium = obj.slm_ActNetGrossPremium,
                        slm_ActGrossPremiumPay = obj.slm_ActGrossPremiumPay,
                        slm_ActPurchaseFlag = obj.slm_ActPurchaseFlag,
                        slm_DiscountPercent = obj.slm_DiscountPercent,
                        slm_DiscountBath = obj.slm_DiscountBath,
                        slm_ActSignNo = obj.slm_ActSignNo,
                        slm_ActNetGrossPremiumFull = obj.slm_ActNetGrossPremiumFull,
                        slm_Vat1Percent = obj.slm_Vat1Percent,
                        slm_Vat1PercentBath = obj.slm_Vat1PercentBath,
                        slm_CreatedDate = createDate,
                        slm_CreatedBy = createByUsername,
                        slm_UpdatedDate = createDate,
                        slm_UpdatedBy = createByUsername,
                        slm_Version = 1
                    };
                    compActSnapInsList.Add(actSnap);
                    slmdb.kkslm_tr_renewinsurance_compare_act_snap.AddObject(actSnap);

                    //Update ข้อมูล พรบ ที่ถูกเลือกไปที่ kkslm_tr_renewinsurance
                    if (obj.slm_ActPurchaseFlag == true)
                    {
                        renew.slm_ActStartCoverDate = obj.slm_ActStartCoverDate;
                        renew.slm_ActEndCoverDate = obj.slm_ActEndCoverDate;
                        renew.slm_ActComId = obj.slm_Ins_Com_Id;
                        renew.slm_ActVat = obj.slm_ActGrossVat != null ? obj.slm_ActGrossVat : 0;
                        renew.slm_ActStamp = obj.slm_ActGrossStamp != null ? obj.slm_ActGrossStamp : 0;

                        //เบี้ยสุทธิไม่รวมภาษีอากร
                        renew.slm_ActNetPremium = obj.slm_ActGrossPremium;
                        //เบี้ยที่ต้องชำระรวมภาษีอากรและหักส่วนลดแล้ว
                        renew.slm_ActGrossPremium = obj.slm_ActGrossPremiumPay != null ? obj.slm_ActGrossPremiumPay : 0;
                        renew.slm_ActDiscountAmt = obj.slm_DiscountBath != null ? obj.slm_DiscountBath : 0;
                        renew.slm_ActDiscountPercent = obj.slm_DiscountPercent != null ? obj.slm_DiscountPercent : 0;

                        if (obj.slm_Vat1Percent == true) { renew.slm_Need_50TawiFlag = "Y"; }
                    }
                }

                slmdb.SaveChanges();            //SaveChange เพื่อให้ได้ slm_RenewInsureId, นำไปใช้ในการ insert table kkslm_tr_renewinsurance_address
                decimal renewInsureId = renew.slm_RenewInsureId;

                addrInsList.ForEach(p => p.slm_RenewInsureId = renewInsureId);
                compInsList.ForEach(p => p.slm_RenewInsureId = renewInsureId);
                compSnapInsList.ForEach(p => p.slm_RenewInsureId = renewInsureId);
                compActInsList.ForEach(p => p.slm_RenewInsureId = renewInsureId);
                compActSnapInsList.ForEach(p => p.slm_RenewInsureId = renewInsureId);

                slmdb.SaveChanges();

                TicketId = ticketId;
                CampaignId = lead.slm_CampaignId;
                ProductId = lead.slm_Product_Id;
            }
            catch
            {
                throw;
            }
        }

        private StaffData GetCreatePersonInfo(string username, SLM_DBEntities slmdb)
        {
            string sql = @"SELECT staff.slm_Position_id AS PositionId, staff.slm_BranchCode AS BranchCode, branch.slm_ChannelId AS ChannelId
                            FROM dbo.kkslm_ms_staff staff with (nolock) 
                            LEFT JOIN dbo.kkslm_ms_branch branch with (nolock) on branch.slm_BranchCode = staff.slm_BranchCode 
                            WHERE staff.slm_UserName = '" + username + "' ";

            return slmdb.ExecuteStoreQuery<StaffData>(sql).FirstOrDefault();
        }

        public void CheckRenewPurchased(string preleadId)
        {
            try
            {
                string sql = "";
                int countPolicy = 0;
                //int countAct = 0;

                sql = "SELECT COUNT(*) FROM dbo.kkslm_tr_prelead_compare with (nolock) where slm_Selected = 1 AND slm_Prelead_Id = '" + preleadId + "' ";
                countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                if (countPolicy == 0)
                    throw new Exception("ไม่พบข้อมูลการซื้อประกัน");
            }
            catch
            {
                throw;
            }
        }

        public void CheckActPurchased(string preleadId)
        {
            try
            {
                string sql = "";
                int countPolicy = 0;
                int countAct = 0;

                sql = "SELECT COUNT(*) FROM dbo.kkslm_tr_prelead_compare with (nolock) where slm_Selected = 1 AND slm_Prelead_Id = '" + preleadId + "' ";
                countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                if (countPolicy > 0)
                    throw new Exception("ไม่สามารถเลือกซื้อ พรบ.เดี่ยวได้ เนื่องจากมีข้อมูลการซื้อประกัน");

                sql = @"SELECT COUNT(*) 
                        FROM dbo.kkslm_tr_prelead_compare_act pca with (nolock) 
                        WHERE pca.slm_ActPurchaseFlag = 1  and pca.slm_Prelead_Id = '" + preleadId + "'";

                countAct = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                if (countAct == 0)
                    throw new Exception("ไม่พบข้อมูลการซื้อ พรบ.เดี่ยว");
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetDataForCARLogService(decimal preleadId)
        {
            try
            {
                List<string> list = new List<string>();
                var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == preleadId).FirstOrDefault();
                if (prelead != null)
                {
                    list.Add(prelead.slm_Contract_Number != null ? prelead.slm_Contract_Number : "");
                    list.Add(prelead.slm_Car_License_No != null ? prelead.slm_Car_License_No : "");
                }

                return list;
            }
            catch
            {
                throw;
            }
        }

        public string GetCBSSubScriptionTypeId(int cardTypeId)
        {
            try
            {
                var subId = slmdb.kkslm_ms_cardtype.Where(p => p.slm_CardTypeId == cardTypeId && p.is_Deleted == false).Select(p => p.slm_CIFSubscriptTypeId).FirstOrDefault();
                return subId != null ? subId : "0";
            }
            catch
            {
                throw;
            }
        }

        private bool PassPrivilegeCampaign(int flag, string campaignId, string username)
        {
            string sql = "";

            sql = @"SELECT staff.slm_StaffNameTH AS TextField ,staff.slm_UserName AS ValueField
                    FROM    kkslm_ms_branch branch with (nolock) 
                    inner join kkslm_ms_staff staff with (nolock) on staff.slm_BranchCode = branch.slm_BranchCode 
	                        INNER JOIN (
	                                    SELECT DISTINCT Z.slm_BranchCode,Z.slm_StaffTypeId  
                                        FROM (
			                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
			                                    FROM kkslm_ms_access_right AR with (nolock) 
                                                INNER JOIN kkslm_ms_campaign CAM with (nolock) ON CAM.slm_CampaignId = AR.slm_CampaignId 
			                                    WHERE CAM.slm_CampaignId = '{1}' 
			                                    UNION ALL
			                                    SELECT AR.slm_BranchCode,AR.slm_StaffTypeId 
			                                    FROM kkslm_ms_access_right AR with (nolock) 
                                                INNER JOIN CMT_CAMPAIGN_PRODUCT CP with (nolock) ON CP.PR_ProductId = AR.slm_Product_Id
			                                    WHERE CP.PR_ProductId = (SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT with (nolock) 
			                                    WHERE PR_CAMPAIGNID = '{1}')
			                                 ) AS Z
		                              ) AS A ON A.slm_BranchCode = STAFF.slm_BranchCode AND A.slm_StaffTypeId = STAFF.slm_StaffTypeId     
	                    WHERE {0} STAFF.slm_UserName = '{2}' AND STAFF.slm_StaffTypeId NOT IN ({3})               
                    ";

            string wh = "";

            if (flag == SLMConstant.Branch.Active)
            {
                wh = " branch.is_Deleted = 0 AND ";
            }
            else if (flag == SLMConstant.Branch.InActive)
            {
                wh = " branch.is_Deleted = 1 AND";
            }
            else if (flag == SLMConstant.Branch.All)
            {
                wh = "";
            }
            else
                wh = "";

            sql = string.Format(sql, wh, campaignId, username, SLMConstant.StaffType.ITAdministrator);

            var list = slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            if (list.Count > 0)
                return true;
            else
                return false;
        }

        public List<ControlListData> GetSubStatusList(string productId, string campaignId, string statusCode)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                {
                    string sql = "SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WHERE PR_CampaignId = '" + campaignId + "'";
                    productId = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
                }

                List<ControlListData> list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_CampaignId == campaignId && p.slm_OptionCode == statusCode).OrderBy(p => p.slm_SubStatusName)
                                                    .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();

                if (list.Count == 0)
                {
                    list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_OptionCode == statusCode).OrderBy(p => p.slm_SubStatusName)
                                .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
                }

                return list;
            }
            catch
            {
                throw;
            }
        }

        public List<ProductData> GetDataByCampaignId(string campaignId)
        {
            string sql = @"SELECT PG.product_id AS ProductGroupId, PG.product_name AS ProductGroupName, MP.sub_product_id AS ProductId, MP.sub_product_name AS ProductName
                            , CAM.slm_CampaignId AS CampaignId, CAM.slm_CampaignName AS CampaignName, CAM.slm_Offer + ' : ' + CAM.slm_Criteria AS CampaignDesc, CAM.slm_StartDate AS StartDate, CAM.slm_EndDate AS EndDate
                            , ISNULL(MP.HasADAMUrl, 0) AS HasAdamsUrl
                            FROM dbo.CMT_MS_PRODUCT_GROUP PG with (nolock) 
                            INNER JOIN dbo.CMT_MAPPING_PRODUCT MP with (nolock) ON PG.product_id = MP.product_id
                            INNER JOIN dbo.CMT_CAMPAIGN_PRODUCT CP with (nolock) ON MP.sub_product_id = CP.PR_ProductId AND MP.product_id = CP.PR_ProductGroupId
                            INNER JOIN dbo.kkslm_ms_campaign CAM with (nolock) ON CP.PR_CampaignId = CAM.slm_CampaignId AND CAM.is_Deleted = 0 
                            WHERE CAM.slm_CampaignId = '" + campaignId + "'";

            return slmdb.ExecuteStoreQuery<ProductData>(sql).ToList();
        }

        public string GetFieldType(string ticketId, string actionType)
        {
            try
            {
                var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                if (lead != null)
                    return GetFieldType(lead.slm_CampaignId, lead.slm_Product_Id, actionType);
                else
                    return "";
            }
            catch
            {
                throw;
            }
        }

        private string GetFieldType(string campaignId, string productId, string actionType)
        {
            try
            {
                string type = string.Empty;

                type = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_CampaignId == campaignId && p.slm_ActionType == actionType && p.is_Deleted == false)
                            .Select(p => p.slm_Type).FirstOrDefault();

                if (string.IsNullOrEmpty(type))
                {
                    type = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_Product_Id == productId && p.slm_ActionType == actionType && p.is_Deleted == false)
                            .Select(p => p.slm_Type).FirstOrDefault();
                }

                return type != null ? type : "";
            }
            catch
            {
                throw;
            }
        }

        public PreleadViewData GetPreleadData(decimal preleadId, string licenseNo, string campaignId, string ContractNo)
        {
            try
            {
                int count = 0;
                var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == preleadId).FirstOrDefault();
                if (prelead != null)
                {
                    count = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_CampaignId == prelead.slm_CampaignId).Count();
                    if (count > 0)
                    {
                        return GetPreleadData(preleadId.ToString(), "campaign", licenseNo, campaignId, ContractNo);
                    }
                    else
                    {
                        count = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_Product_Id == prelead.slm_Product_Id).Count();
                        if (count > 0)
                        {
                            return GetPreleadData(preleadId.ToString(), "product", licenseNo, campaignId, ContractNo);
                        }
                        else
                            throw new Exception("ไม่พบข้อมูลสถานะย่อยในตาราง kkslm_ms_config_product_substatus ของ CampaignId " + prelead.slm_CampaignId + ", ProductId " + prelead.slm_Product_Id);
                    }
                }
                else
                    return null;
            }
            catch
            {
                throw;
            }
        }

        private PreleadViewData GetPreleadData(string preleadId, string flag, string licenseNo, string campaignId, string contractNo)
        {
            try
            {
                string sql = @"select pre.slm_Prelead_Id AS PreleadId, pre.slm_Contract_Number AS ContractNo, tt.slm_TeamTelesales_Code as TeamCode , ISNULL(title1.slm_TitleName, '') + pre.slm_Name as Firstname
                                 ,pre.slm_LastName AS Lastname ,contact.slm_Mobile_Phone AS TelNo1, pre.slm_TicketId AS TicketId, mo.slm_OptionDesc AS StatusDesc , cs.slm_SubStatusName AS SubStatusDesc
                                , cam.slm_CampaignName as CampaignName, prephone1.slm_CreatedDate AS PreContactLatestDate
                                , pre.slm_AssignDate AS AssignedDate,OwnerBranch.slm_BranchName AS OwnerBranchName, pre.slm_Owner AS OwnerEmpCode
                                , st.slm_StaffNameTH as OwnerName, pre.slm_CitizenId AS CitizenId, cardtype.slm_CardTypeName AS CardTypeName, pre.slm_CardTypeId AS CardTypeId
                                , pre.slm_CampaignId AS CampaignId, pre.slm_Product_Id AS ProductId, pre.slm_Status AS StatusCode, pre.slm_SubStatus AS SubStatusCode, pre.slm_NextContactDate AS NextContactDate
                                , mapproduct.product_id AS ProductGroupId, mapproduct.sub_product_name AS ProductName, pg.product_name AS ProductGroupName
                                from " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead pre ";

                if (flag == "campaign")
                {
                    sql += " inner join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus cs on cs.slm_CampaignId = pre.slm_CampaignId and pre.slm_Status = cs.slm_OptionCode  and pre.slm_SubStatus = cs.slm_SubStatusCode ";
                }
                else
                {
                    sql += " inner join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus cs on cs.slm_Product_Id = pre.slm_Product_Id and pre.slm_Status = cs.slm_OptionCode  and pre.slm_SubStatus = cs.slm_SubStatusCode ";
                }

                sql += @"   inner join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option mo on mo.slm_OptionCode = pre.slm_Status and mo.slm_OptionType = 'lead status'
                            inner join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam on cam.slm_CampaignId = pre.slm_CampaignId 
                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff st on pre.slm_Owner = st.slm_EmpCode 
                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_teamtelesales tt on st.slm_TeamTelesales_Id = tt.slm_TeamTelesales_Id 
                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_title title1 on title1.slm_TitleId = pre.slm_TitleId AND title1.is_Deleted = 0
                            left join " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT mapproduct on mapproduct.sub_product_id = pre.slm_Product_Id
                            left join " + SLMConstant.SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP pg ON mapproduct.product_id = pg.product_id
                            left join
                            (
	                            select top 1 pa.slm_Prelead_Id,pa.slm_Mobile_Phone,pa.slm_Customer_Key,pa.slm_Address_Type 
	                            from " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_address pa 
	                            where slm_Address_Type = 'C' and slm_Prelead_Id = '" + preleadId + @"') as contact on contact.slm_Prelead_Id = pre.slm_Prelead_Id 
                            left join 
                            (	
	                            select top 1 slm_CreatedDate,slm_Prelead_Id 
	                            from " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_phone_call  
	                            where slm_Prelead_Id = '" + preleadId + @"' 
	                            order by slm_CreatedDate desc) as prephone1 on prephone1.slm_Prelead_Id = pre.slm_Prelead_Id 
                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_branch OwnerBranch on OwnerBranch.slm_BranchCode = st.slm_BranchCode 
                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_cardtype cardtype on cardtype.slm_CardTypeId = pre.slm_CardTypeId
                            where pre.slm_AssignFlag = '1' and pre.slm_Assign_Status = '1' and pre.is_Deleted = 0 
                            and pre.slm_Prelead_Id = '" + preleadId + @"' {0} ";

                string whr = "";
                whr += (licenseNo == "" ? "" : (whr == "" ? "" : " AND ") + " pre.slm_Car_License_No = '" + licenseNo + "' ");
                whr += (campaignId == "" ? "" : (whr == "" ? "" : " AND ") + " pre.slm_CampaignId = '" + campaignId + "' ");
                whr += (contractNo == "" ? "" : (whr == "" ? "" : " AND ") + " pre.slm_Contract_Number = '" + contractNo + "' ");

                whr = (whr == "" ? "" : " AND ") + whr;
                sql = string.Format(sql, whr);

                return slmdb.ExecuteStoreQuery<PreleadViewData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public List<ControlListData> GetCardTypeList()
        {
            return slmdb.kkslm_ms_cardtype.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_CardTypeId).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_CardTypeName, ValueField = p.slm_CardTypeId.ToString() }).ToList();
        }

        public List<ControlListData> GetNewSubStatusList(string productId, string campaignId, string statusCode)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                {
                    string sql = "SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WHERE PR_CampaignId = '" + campaignId + "'";
                    productId = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
                }

                List<ControlListData> list = new List<ControlListData>();

                list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_CampaignId == campaignId && p.slm_OptionCode == statusCode).OrderBy(p => p.slm_SubStatusName)
                                .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();

                if (list.Count == 0)
                {
                    list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_OptionCode == statusCode).OrderBy(p => p.slm_SubStatusName)
                                .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
                }

                return list;
            }
            catch
            {
                throw;
            }
        }

        public bool checkBlackList(string Username)
        {
            string sql = @"SELECT count(1) FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff WHERE slm_StaffTypeId in ('7','15') and is_Deleted = 0 and slm_UserName = '" + Username + "'";

            return slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault() > 0 ? true : false;
        }

        public string GetBlacklistId(string productId, string citizenId)
        {
            try
            {
                string sql = @"SELECT pdb.slm_cp_blacklist_id 
                                FROM dbo.kkslm_ms_config_product_blacklist pdb with (nolock) 
                                WHERE pdb.slm_Product_Id = '" + productId + "' AND pdb.slm_CitizenId = '" + citizenId + @"' AND pdb.slm_IsActive = 1
                                AND CONVERT(DATE,GETDATE()) BETWEEN CONVERT(DATE, pdb.slm_StartDate) AND CONVERT(DATE, pdb.slm_EndDate) ";

                var id = slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
                return id != null ? id.Value.ToString() : "";
            }
            catch
            {
                throw;
            }
        }
    }
}
