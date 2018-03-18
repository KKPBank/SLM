using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;
using SLS.Resource.Data;

namespace SLS.Dal.Models
{
    public class SlmTrLeadModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmTrLeadModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

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

        private int? GetPositionIdByEmpCode(string empCode)
        {
            SlmMsStaffModel staff = new SlmMsStaffModel(slmdb);
            return staff.GetPositionIdByEmpCode(empCode);
        }

        public string InsertData(string ticketId, Mandatory mandatory, CustomerInfo cusInfo, CustomerDetail cusDetail, ChannelInfo channelInfo, string channelId, DealerInfo dealerInfo, string reference, AppInfo appInfo)
        {
            try
            {
                DateTime createDate = DateTime.Now;
                kkslm_tr_lead lead = new kkslm_tr_lead();
                lead.slm_ticketId = ticketId;
                lead.slm_Name = mandatory.Firstname;
                lead.slm_TelNo_1 = mandatory.TelNo1;
                lead.slm_CampaignId = mandatory.Campaign;
                lead.slm_AssignedFlag = "0";
                lead.slm_Delegate_Flag = 0;
                lead.slm_Counting = 0;
                lead.slm_Status = "00";
                lead.slm_StatusDate = createDate;
                lead.slm_StatusDateSource = createDate;
                lead.slm_ChannelId = channelId;
                lead.slm_ThisWork = 0;
                lead.slm_TotalAlert = 0;
                lead.slm_TotalWork = 0;

                if (cusInfo != null)
                {
                    if (!string.IsNullOrEmpty(cusInfo.ExtNo1)) lead.slm_Ext_1 = cusInfo.ExtNo1;
                    if (!string.IsNullOrEmpty(cusInfo.Lastname)) lead.slm_LastName = cusInfo.Lastname;
                    if (!string.IsNullOrEmpty(cusInfo.ContractNoRefer)) lead.slm_ContractNoRefer = cusInfo.ContractNoRefer;
                }

                if (cusDetail != null)
                {
                    if (!string.IsNullOrEmpty(cusDetail.TelesaleName))
                    {
                        lead.slm_Owner = cusDetail.TelesaleName;
                        lead.slm_Owner_Position = GetPositionId(cusDetail.TelesaleName);
                        var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == lead.slm_Owner).FirstOrDefault();
                        if (staff != null)
                        {
                            lead.slm_StaffId = staff.slm_StaffId;
                            if (ServiceConstant.DoValidateBranch)
                            {
                                SlmMsBranchModel branch = new SlmMsBranchModel(slmdb);
                                if (!branch.CheckBranchActive(staff.slm_BranchCode))
                                {
                                    throw new ServiceException("", "Telesales " + cusDetail.TelesaleName + " is in inactive branch " + staff.slm_BranchCode);
                                }    
                            }
                            lead.slm_Owner_Branch = staff.slm_BranchCode;
                        }
                    }
                    if (!string.IsNullOrEmpty(cusDetail.AvailableTime)) lead.slm_AvailableTime = cusDetail.AvailableTime;
                }

                if (channelInfo != null)
                {
                    if (!string.IsNullOrEmpty(channelInfo.CreateUser))
                    {
                        lead.slm_StatusBy = channelInfo.CreateUser;
                        lead.slm_CreatedBy = channelInfo.CreateUser;
                        lead.slm_CreatedBy_Position = GetPositionId(channelInfo.CreateUser);
                        lead.slm_UpdatedBy = channelInfo.CreateUser;

                        var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == lead.slm_CreatedBy).FirstOrDefault();
                        if (staff != null)
                        {
                            if (ServiceConstant.DoValidateBranch)
                            {
                                SlmMsBranchModel branch = new SlmMsBranchModel(slmdb);
                                if (!branch.CheckBranchActive(staff.slm_BranchCode))
                                {
                                    throw new ServiceException("", "CreateUser " + lead.slm_CreatedBy + " is in inactive branch " + staff.slm_BranchCode);
                                }
                            }
                            lead.slm_CreatedBy_Branch = staff.slm_BranchCode;
                            
                        }
                    }
                    else
                    {
                        lead.slm_StatusBy = "SYSTEM";
                        lead.slm_CreatedBy = "SYSTEM";
                        lead.slm_UpdatedBy = "SYSTEM";
                    }
                }
                else
                {
                    lead.slm_StatusBy = "SYSTEM";
                    lead.slm_CreatedBy = "SYSTEM";
                    lead.slm_UpdatedBy = "SYSTEM";
                }

                if (dealerInfo != null)
                {
                    if (!string.IsNullOrEmpty(dealerInfo.DealerCode)) lead.slm_DealerCode = dealerInfo.DealerCode;
                    if (!string.IsNullOrEmpty(dealerInfo.DealerName)) lead.slm_DealerName = dealerInfo.DealerName;
                }
                if (!string.IsNullOrEmpty(reference)) lead.slm_Reference = reference;

                List<ProductData> prodList = GetProductCampaignData(lead.slm_CampaignId);
                if (prodList.Count > 0)
                {
                    lead.slm_Product_Group_Id = prodList[0].ProductGroupId;
                    lead.slm_Product_Id = prodList[0].ProductId;
                    lead.slm_Product_Name = prodList[0].ProductName;
                }

                if (appInfo != null)
                {
                    if (!string.IsNullOrEmpty(appInfo.System))
                    {
                        if (!string.IsNullOrEmpty(appInfo.COCStatus))
                        {
                            CocMsMappingStatusModel mapping = new CocMsMappingStatusModel(slmdb);
                            string slmStatusCode = mapping.GetSlmStatusCode(appInfo.System.Trim().ToUpper(), appInfo.COCStatus, appInfo.COCSubStatus);
                            if (slmStatusCode == "")
                            {
                                throw new ServiceException("", "Cannot find slmStatusCode from the specified coc status code and sub status code");
                            }

                            lead.slm_Status = slmStatusCode;
                        }

                        lead.coc_System = appInfo.System;
                        if (!string.IsNullOrEmpty(appInfo.AppNo)) lead.coc_Appno = appInfo.AppNo;
                        if (!string.IsNullOrEmpty(appInfo.CurrentTeam)) lead.coc_CurrentTeam = appInfo.CurrentTeam;
                        if (!string.IsNullOrEmpty(appInfo.COCStatus)) lead.coc_Status = appInfo.COCStatus;
                        if (!string.IsNullOrEmpty(appInfo.COCSubStatus)) lead.coc_SubStatus = appInfo.COCSubStatus;
                        if (!string.IsNullOrEmpty(appInfo.StatusBy)) lead.coc_StatusBy = appInfo.StatusBy;
                        if (appInfo.StatusDate.Year != 1) lead.coc_StatusDate = appInfo.StatusDate;
                    }
                }

                lead.slm_CreatedDate = createDate;
                lead.slm_UpdatedDate = createDate;

                //2017-03-29 Auto Rule Assign
                if (mandatory.AutoRuleAssign)
                {
                    DoAutoRuleAssign(lead, cusDetail, createDate);
                }

                slmdb.kkslm_tr_lead.AddObject(lead);
                slmdb.SaveChanges();

                return lead.slm_Product_Id;
            }
            catch(Exception ex)
            {
                throw new ServiceException(ApplicationResource.INS_INSERT_FAIL_CODE, ApplicationResource.INS_INSERT_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// UpdateData ปัจจุบันใช้เพื่อ update status ที่ส่งมาจาก ADAMS และรองรับ System อื่นๆ
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="cusInfo"></param>
        /// <param name="channInfo"></param>
        /// <param name="systemName"></param>
        public void UpdateData(string ticketId, CustomerInfo cusInfo, ChannelInfo channInfo, string systemName, out string calculateTotalSlaError)
        {
            string newStatus = "";
            string newExternalStatus = "";
            string newExternalSubStatus = "";
            string newExternalSubStatusDesc = "";
            string oldStatus = "";
            string oldExternalSystem = "";
            string oldExternalStatus = "";
            string oldExternalSubStatus = "";
            string oldExternalSubStatusDesc = "";
            string updatedUser = "";

            try
            {
                calculateTotalSlaError = "";
                if (string.IsNullOrEmpty(systemName))   //ใส่ไว้รองรับ Channel เดิม ที่ไม่มี tag system
                {
                    return;
                }
                    
                var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId.Equals(ticketId)).FirstOrDefault();
                if (lead != null)
                {
                    oldStatus = lead.slm_Status;
                    oldExternalSystem = lead.slm_ExternalSystem;                                                                    //add 14/10/2015
                    oldExternalStatus = lead.slm_ExternalStatus;                                                                    //add 14/10/2015
                    oldExternalSubStatus = string.IsNullOrEmpty(lead.slm_ExternalSubStatus) ? null : lead.slm_ExternalSubStatus;    //add 14/10/2015
                    oldExternalSubStatusDesc = lead.slm_ExternalSubStatusDesc;                                                      //add 14/10/2015

                    DateTime updateDate = DateTime.Now;

                    if (channInfo != null)
                    {
                        if (!string.IsNullOrEmpty(channInfo.CreateUser))
                            updatedUser = channInfo.CreateUser;
                        else
                            updatedUser = "System";
                    }
                    else
                        updatedUser = "System";

                    if (cusInfo != null)
                    {
                        if (lead.coc_IsCOC == true && !string.IsNullOrEmpty(cusInfo.Status))
                        {
                            throw new ServiceException("", "ข้อมูลผู้มุ่งหวัง TicketId " + ticketId + " เข้า COC แล้ว, ไม่สามารถอัพเดทสถานะได้");
                        }

                        if (!string.IsNullOrEmpty(cusInfo.Status) && !string.IsNullOrEmpty(systemName))
                        {
                            MasterModel master = new MasterModel(slmdb);
                            newStatus = master.GetSlmStatusFromMappingCode(systemName, lead.slm_Product_Id, cusInfo.Status, cusInfo.SubStatus);
                            if (string.IsNullOrEmpty(newStatus))
                            {
                                throw new ServiceException("", "Cannot find slmStatusCode from the specified " + systemName + " mapping status code");
                            }

                            newExternalStatus = cusInfo.Status;
                            newExternalSubStatus = string.IsNullOrEmpty(cusInfo.SubStatus) ? null : cusInfo.SubStatus;
                            newExternalSubStatusDesc = string.IsNullOrEmpty(cusInfo.SubStatusDesc) ? null : cusInfo.SubStatusDesc;

                            //Changed by Pom 16/03/2016
                            if (!string.IsNullOrEmpty(cusInfo.StatusReSendFlag) && cusInfo.StatusReSendFlag == "Y")
                            {
                                //มีการส่ง flag เพื่อต้องการซ่อม
                                if (lead.slm_StatusDateSource == null)
                                    lead.slm_StatusDateSource = lead.slm_StatusDate;

                                if (cusInfo.StatusDateSource > lead.slm_StatusDateSource)
                                {
                                    CheckStatusChanged(ticketId, lead, cusInfo, oldStatus, newStatus, oldExternalStatus, newExternalStatus, oldExternalSubStatus, newExternalSubStatus
                                        , oldExternalSystem, oldExternalSubStatusDesc, newExternalSubStatusDesc, systemName, updatedUser, updateDate, out calculateTotalSlaError);
                                }
                                else
                                {
                                    //ถ้า statusDateSource ที่ส่งเข้ามา เท่ากับหรือน้อยกว่า statusDateSource ใน lead ให้ลงแต่ OwnerLogging
                                    kkslm_tr_activity activity = new kkslm_tr_activity()
                                    {
                                        slm_TicketId = ticketId,
                                        slm_OldStatus = null,
                                        slm_NewStatus = newStatus,
                                        slm_CreatedBy = updatedUser,
                                        slm_CreatedBy_Position = GetPositionId(updatedUser),
                                        slm_CreatedDate = cusInfo.StatusDateSource.Year != 1 ? cusInfo.StatusDateSource : updateDate,
                                        slm_Type = "13",                                           //Repair Logs
                                        slm_SystemAction = systemName,                             //System ที่เข้ามาทำ action 
                                        slm_SystemActionBy = "SLM",                                //action เกิดขึ้นที่ระบบอะไร 
                                        slm_ExternalSystem_Old = null,
                                        slm_ExternalStatus_Old = null,
                                        slm_ExternalSubStatus_Old = null,
                                        slm_ExternalSubStatusDesc_Old = null,
                                        slm_ExternalSystem_New = systemName,
                                        slm_ExternalStatus_New = newExternalStatus,
                                        slm_ExternalSubStatus_New = newExternalSubStatus,
                                        slm_ExternalSubStatusDesc_New = newExternalSubStatusDesc,
                                    };
                                    slmdb.kkslm_tr_activity.AddObject(activity);
                                }
                            }
                            else
                            {
                                //ไม่มี flag การซ่อม ให้ยึดการทำงานแบบเดิม แต่เพิ่มในส่วนของการเช็ก StatusDateSource
                                CheckStatusChanged(ticketId, lead, cusInfo, oldStatus, newStatus, oldExternalStatus, newExternalStatus, oldExternalSubStatus, newExternalSubStatus
                                        , oldExternalSystem, oldExternalSubStatusDesc, newExternalSubStatusDesc, systemName, updatedUser, updateDate, out calculateTotalSlaError);
                            }
                        }
                    }

                    slmdb.SaveChanges();
                }
                else
                {
                    throw new ServiceException(ApplicationResource.UPD_NO_RECORD_FOUND_CODE, ApplicationResource.UPD_NO_RECORD_FOUND_DESC);
                }    
            }
            catch (Exception ex)
            {
                throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        private void CheckStatusChanged(string ticketId, kkslm_tr_lead lead, CustomerInfo cusInfo, string oldStatus, string newStatus, string oldExternalStatus, string newExternalStatus, string oldExternalSubStatus, string newExternalSubStatus
            , string oldExternalSystem, string oldExternalSubStatusDesc, string newExternalSubStatusDesc, string systemName, string updatedUser, DateTime updateDate, out string calculateTotalSlaError)
        {
            try
            {
                calculateTotalSlaError = "";
                if (oldStatus != newStatus || oldExternalStatus != newExternalStatus || oldExternalSubStatus != newExternalSubStatus || oldExternalSystem != systemName)
                {
                    kkslm_tr_activity activity = new kkslm_tr_activity()
                    {
                        slm_TicketId = ticketId,
                        slm_OldStatus = oldStatus,
                        slm_NewStatus = newStatus,
                        slm_CreatedBy = updatedUser,
                        slm_CreatedBy_Position = GetPositionId(updatedUser),
                        slm_CreatedDate = cusInfo.StatusDateSource.Year != 1 ? cusInfo.StatusDateSource : updateDate,  //add 16/03/2016
                        slm_Type = "02",                                           //Change Status
                        slm_SystemAction = systemName,                             //System ที่เข้ามาทำ action (19/03/2015)
                        slm_SystemActionBy = "SLM",                                //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                        slm_ExternalSystem_Old = oldExternalSystem,                //add 14/10/2015
                        slm_ExternalStatus_Old = oldExternalStatus,                //add 14/10/2015
                        slm_ExternalSubStatus_Old = oldExternalSubStatus,          //add 14/10/2015
                        slm_ExternalSubStatusDesc_Old = oldExternalSubStatusDesc,  //add 14/10/2015
                        slm_ExternalSystem_New = systemName,                       //add 14/10/2015
                        slm_ExternalStatus_New = newExternalStatus,                //add 14/10/2015
                        slm_ExternalSubStatus_New = newExternalSubStatus,          //add 14/10/2015
                        slm_ExternalSubStatusDesc_New = newExternalSubStatusDesc,  //add 14/10/2015
                    };
                    slmdb.kkslm_tr_activity.AddObject(activity);

                    calculateTotalSlaError = CalculateTotalSLA(slmdb, lead, activity, updateDate);           //add 24/03/2559
                    lead.slm_Status = newStatus;
                    lead.slm_StatusBy = updatedUser;
                    lead.slm_StatusDate = updateDate;
                    lead.slm_StatusDateSource = cusInfo.StatusDateSource.Year != 1 ? cusInfo.StatusDateSource : updateDate; //add 16/03/2016
                    lead.slm_Counting = 0;
                    lead.slm_UpdatedBy = updatedUser;
                    lead.slm_UpdatedDate = updateDate;

                    lead.slm_ExternalSystem = systemName;                           //add 14/10/2015
                    lead.slm_ExternalStatus = newExternalStatus;                    //add 14/10/2015
                    lead.slm_ExternalSubStatus = newExternalSubStatus;              //add 14/10/2015
                    lead.slm_ExternalSubStatusDesc = newExternalSubStatusDesc;      //add 14/10/2015
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string CalculateTotalSLA(SLM_DBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_activity act, DateTime createdDate)
        {
            string error = "";
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

                if (branch == null)
                {
                    throw new ServiceException("", "ไม่พบข้อมูลสาขาสำหรับคำนวณ Total SLA, BranchCode:" + lead.slm_Owner_Branch);
                }

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
                        {
                            throw new ServiceException("", "ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);
                        }

                    }
                    else
                    {
                        throw new ServiceException("", "ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);
                    }

                    if (end != null)
                    {
                        string[] str = end.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            endTimeHour = Convert.ToInt32(str[0]);
                            endTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                        {
                            throw new ServiceException("", "ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                        }
                    }
                    else
                    {
                        throw new ServiceException("", "ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                    }
                        
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
                error = "CalculateTotalSla Error=" + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return error;
        }

        private DateTime DeleteSeconds(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
        }

        private List<ProductData> GetProductCampaignData(string campaignId)
        {
//            string sql = @"SELECT PG.product_id AS ProductGroupId, PG.product_name AS ProductGroupName, MP.sub_product_id AS ProductId, MP.sub_product_name AS ProductName
//                            , CAM.slm_CampaignId AS CampaignId, CAM.slm_CampaignName AS CampaignName, CAM.slm_Offer + ' : ' + CAM.slm_Criteria AS CampaignDesc, CAM.slm_StartDate AS StartDate, CAM.slm_EndDate AS EndDate
//                            FROM SLMDB.dbo.CMT_MS_PRODUCT_GROUP PG
//                            INNER JOIN SLMDB.dbo.CMT_MAPPING_PRODUCT MP ON PG.product_id = MP.product_id
//                            INNER JOIN SLMDB.dbo.CMT_CAMPAIGN_PRODUCT CP ON MP.sub_product_id = CP.PR_ProductId AND MP.product_id = CP.PR_ProductGroupId
//                            INNER JOIN SLMDB.dbo.kkslm_ms_campaign CAM ON CP.PR_CampaignId = CAM.slm_CampaignId AND CAM.is_Deleted = 0 AND CAM.slm_Status = 'A' AND CAM.slm_CampaignType = '" + ApplicationResource.CAMPAIGNTYPE_MASS + @"'
//                            WHERE CAM.slm_CampaignId = '" + campaignId + "'";

            string SLMDBName = GetDBName();

            string sql = @"SELECT PG.product_id AS ProductGroupId, PG.product_name AS ProductGroupName, MP.sub_product_id AS ProductId, MP.sub_product_name AS ProductName
                            , CAM.slm_CampaignId AS CampaignId, CAM.slm_CampaignName AS CampaignName, CAM.slm_Offer + ' : ' + CAM.slm_Criteria AS CampaignDesc, CAM.slm_StartDate AS StartDate, CAM.slm_EndDate AS EndDate
                            FROM " + SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP PG
                            INNER JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT MP ON PG.product_id = MP.product_id
                            INNER JOIN " + SLMDBName + @".dbo.CMT_CAMPAIGN_PRODUCT CP ON MP.sub_product_id = CP.PR_ProductId AND MP.product_id = CP.PR_ProductGroupId
                            INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign CAM ON CP.PR_CampaignId = CAM.slm_CampaignId AND CAM.is_Deleted = 0 AND CAM.slm_Status = 'A' 
                            WHERE CAM.slm_CampaignId = '" + campaignId + "'";

            return slmdb.ExecuteStoreQuery<ProductData>(sql).ToList();
        }

        private int? GetStaffId(string username)
        {
            int? id = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).Select(p => p.slm_StaffId).FirstOrDefault();
            return id != 0 ? id : null;
        }

        public string UpdateDataByHpaolf(string ticketId, Mandatory mandatory, CustomerInfo cusInfo, CustomerDetail cusDetail, ChannelInfo channelInfo, string channelId, AppInfo appInfo, DealerInfo dealerInfo)
        {
            try
            {
                var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId.Equals(ticketId)).FirstOrDefault();
                if (lead != null)
                {
                    try
                    {
                        DateTime updateDate = DateTime.Now;
                        string noteDetail = "";

                        lead.slm_Name = mandatory.Firstname;
                        lead.slm_TelNo_1 = mandatory.TelNo1;

                        //lead.slm_CampaignId = mandatory.Campaign;
                        //lead.slm_ChannelId = channelId;

                        if (cusInfo != null)
                        {
                            if (cusInfo.ExtNo1 != null) lead.slm_Ext_1 = cusInfo.ExtNo1;
                            if (cusInfo.Lastname != null) lead.slm_LastName = cusInfo.Lastname;
                            if (cusInfo.ContractNoRefer != null) lead.slm_ContractNoRefer = cusInfo.ContractNoRefer;
                        }

                        if (cusDetail != null)
                        {
                            //if (!string.IsNullOrEmpty(cusDetail.TelesaleName))
                            //{
                            //    lead.slm_Owner = cusDetail.TelesaleName;
                            //    lead.slm_StaffId = GetStaffId(cusDetail.TelesaleName);
                            //}
                            if (cusDetail.AvailableTime != null) lead.slm_AvailableTime = cusDetail.AvailableTime;
                            if (cusDetail.Note != null) noteDetail = cusDetail.Note;
                        }

                        if (channelInfo != null)
                        {
                            if (!string.IsNullOrEmpty(channelInfo.CreateUser))
                            {
                                lead.slm_UpdatedBy = channelInfo.CreateUser;
                            }
                            else
                            {
                                lead.slm_UpdatedBy = "SYSTEM";
                            }
                        }
                        else
                        {
                            lead.slm_UpdatedBy = "SYSTEM";
                        }

                        if (dealerInfo != null)
                        {
                            if (!string.IsNullOrEmpty(dealerInfo.DealerCode)) lead.slm_DealerCode = dealerInfo.DealerCode;
                            if (!string.IsNullOrEmpty(dealerInfo.DealerName)) lead.slm_DealerName = dealerInfo.DealerName;
                        }

                        //HPAOFL
                        if (appInfo != null && !string.IsNullOrEmpty(appInfo.System) && appInfo.UpdateAppInfoFlag == true)
                        {
                            string cocStatusOld = lead.coc_Status != null ? lead.coc_Status : "";
                            string cocSubStatusOld = lead.coc_SubStatus != null ? lead.coc_SubStatus : "";
                            string slmStatusCode = "";

                            //Get slmStatusCode By specify coc status code and sub status code
                            CocMsMappingStatusModel mapping = new CocMsMappingStatusModel(slmdb);
                            slmStatusCode = mapping.GetSlmStatusCode(appInfo.System.Trim().ToUpper(), appInfo.COCStatus, appInfo.COCSubStatus);
                            if (slmStatusCode == "")
                            {
                                throw new ServiceException("", "Cannot find slmStatusCode from the specified coc status code and sub status code");
                            }

                            //หา Username จาก statusBy(empCode) ที่ hpaofl ส่งมา
                            SlmMsStaffModel staff = new SlmMsStaffModel(slmdb);
                            string statusBy_username = staff.GetUsername(appInfo.StatusBy);
                            statusBy_username = (statusBy_username != "" ? statusBy_username : appInfo.StatusBy);  //ถ้าหา username ไม่เจอในเบส ให้ใช้ empcode แทน

                            //ไม่ update AppNo
                            if (!string.IsNullOrEmpty(appInfo.System)) lead.coc_System = appInfo.System;
                            if (appInfo.LastOwner != null)
                            {
                                lead.coc_LastOwner = appInfo.LastOwner;
                                lead.coc_LastOwner_Position = GetPositionIdByEmpCode(appInfo.LastOwner);
                            }
                            if (appInfo.CurrentTeam != null) lead.coc_CurrentTeam = appInfo.CurrentTeam;
                            if (appInfo.FlowType != null) lead.coc_FlowType = appInfo.FlowType;
                            if (appInfo.COCType != null) lead.coc_Type = appInfo.COCType;
                            if (appInfo.RoutebackTeam != null) lead.coc_RouteBackTeam = appInfo.RoutebackTeam;
                            if (appInfo.MarketingOwner != null)
                            {
                                lead.coc_MarketingOwner = appInfo.MarketingOwner;
                                lead.coc_MarketingOwner_Position = GetPositionIdByEmpCode(appInfo.MarketingOwner);
                            }
                            if (appInfo.COCStatus != null) lead.coc_Status = appInfo.COCStatus;

                            //cocSubStatus ถ้าค่าที่ส่งเข้ามาเปนช่องว่างให้ลง NULL ในเบสเพื่อใช้เช็กสถานะใน table kkslm_ms_option เพราะในคอลัม slm_optionSubCode มีการใส่ค่า null
                            //ถ้าใส่ช่องว่างจะทำให้ join กันไม่เจอ
                            if (!string.IsNullOrEmpty(appInfo.COCSubStatus))
                                lead.coc_SubStatus = appInfo.COCSubStatus;
                            else
                                lead.coc_SubStatus = null;

                            if (appInfo.StatusBy != null) lead.coc_StatusBy = appInfo.StatusBy;
                            if (appInfo.StatusDate.Year != 1) lead.coc_StatusDate = appInfo.StatusDate;

                            //กรณีเข้ากล่อง COC1 เท่านั้น(วนกี่ครั้งก็จะ update coc_FirstAssign)
                            if (appInfo.COCSubStatus == "01") lead.coc_FirstAssign = updateDate;

                            //ถ้าระบบ HPGABLE เรียก ให้เช็ท coc_AssignedFlag = 1
                            if (appInfo.System == ServiceConstant.System.HPGABLE)
                            {
                                lead.coc_AssignedFlag = "1";
                                lead.coc_NextSLA = null;
                            }

                            //Insert PhoneCallHistory
                            if (!string.IsNullOrEmpty(appInfo.ContactDetail))
                            {
                                kkslm_phone_call phone = new kkslm_phone_call()
                                {
                                    slm_TicketId = ticketId,
                                    slm_ContactPhone = lead.slm_TelNo_1,
                                    slm_ContactDetail = appInfo.ContactDetail,
                                    slm_Status = slmStatusCode,
                                    slm_Owner = lead.slm_Owner,
                                    slm_Owner_Position = GetPositionId(lead.slm_Owner),
                                    slm_CreateDate = appInfo.StatusDate,
                                    slm_CreateBy = statusBy_username,
                                    slm_CreatedBy_Position = GetPositionId(statusBy_username),
                                    is_Deleted = 0
                                };
                                slmdb.kkslm_phone_call.AddObject(phone);
                            }

                            //Insert OwnerLogging
                            if (cocStatusOld != appInfo.COCStatus || cocSubStatusOld != appInfo.COCSubStatus)
                            {
                                kkslm_tr_activity activity = new kkslm_tr_activity()
                                {
                                    slm_TicketId = ticketId,
                                    slm_OldStatus = cocStatusOld,
                                    slm_NewStatus = appInfo.COCStatus,
                                    slm_OldSubStatus = (string.IsNullOrEmpty(cocSubStatusOld) ? null : cocSubStatusOld),
                                    slm_NewSubStatus = (string.IsNullOrEmpty(appInfo.COCSubStatus) ? null : appInfo.COCSubStatus),
                                    slm_CreatedBy = statusBy_username,
                                    slm_CreatedBy_Position = GetPositionId(statusBy_username),
                                    slm_CreatedDate = appInfo.StatusDate,
                                    slm_UpdatedBy = statusBy_username,
                                    slm_UpdatedDate = appInfo.StatusDate,
                                    slm_Type = "02",
                                    //coc_Team = appInfo.CurrentTeam,
                                    coc_Team = (appInfo.System.Trim().ToUpper() == "HPGABLE" ? "COC" : appInfo.CurrentTeam),
                                    //slm_SystemAction = "SLM",
                                    slm_SystemAction = appInfo.System.Trim().ToUpper(),
                                    slm_SystemActionBy = "SLM"
                                };
                                slmdb.kkslm_tr_activity.AddObject(activity);

                                //update lead status
                                lead.slm_Status = slmStatusCode;
                                lead.slm_StatusBy = statusBy_username;
                                lead.slm_StatusDate = appInfo.StatusDate;
                                lead.slm_Counting = 0;
                                lead.coc_Counting = 0;

                                //Insert Log WorkDetail
                                kkcoc_tr_workdetail logDetail = new kkcoc_tr_workdetail()
                                {
                                    coc_System_From = appInfo.System.Trim().ToUpper(),
                                    coc_Team_From = appInfo.CurrentTeam,
                                    coc_EmpCode_From = appInfo.StatusBy,
                                    coc_TicketId = ticketId,
                                    coc_AppNo = appInfo.AppNo,
                                    coc_ProductGroupId = lead.slm_Product_Group_Id,
                                    coc_ProductId = lead.slm_Product_Id,
                                    coc_CampaignId = lead.slm_CampaignId,
                                    coc_FirstTimeAssign = lead.coc_FirstAssign,
                                    coc_NextSLA = lead.coc_NextSLA,
                                    coc_ActionDate = appInfo.StatusDate,
                                    coc_Status = appInfo.COCStatus,
                                    coc_SubStatus = (string.IsNullOrEmpty(appInfo.COCSubStatus) ? null : appInfo.COCSubStatus),
                                    //coc_System_To = "",                   ต้องเป็น rule หรือ batch มาลงให้
                                    //coc_Team_To = "",                     ต้องเป็น rule หรือ batch มาลงให้
                                    //coc_EmpCode_To = ""                   ต้องเป็น rule หรือ batch มาลงให้
                                    coc_Note = noteDetail,
                                    coc_RouteBackTeam = (string.IsNullOrEmpty(appInfo.RoutebackTeam) ? null : appInfo.RoutebackTeam),
                                    coc_Action = (appInfo.System.Trim().ToUpper() == ServiceConstant.System.HPAOFL ? lead.coc_FlowType : null),
                                    //coc_RouteBackEmp = ""                 ต้องเป็น rule หรือ batch มาลงให้
                                    coc_Type = lead.coc_Type,
                                    //coc_CreatedDate = updateDate          ต้องเป็น rule หรือ batch มาลงให้
                                };
                                slmdb.kkcoc_tr_workdetail.AddObject(logDetail);
                            }
                        }

                        //ถ้าไม่ส่ง tag AppInfo เข้ามา แต่มีค่า system, ให้ถือว่าไม่อยู่ใน Loop ของ COC จึงเซ็ทค่า FlowType กลับให้เป็น Forward
                        if (appInfo != null && !string.IsNullOrEmpty(appInfo.System) && appInfo.UpdateAppInfoFlag == false)
                            lead.coc_FlowType = "F";

                        lead.slm_UpdatedDate = updateDate;
                        slmdb.SaveChanges();

                        return lead.slm_Product_Id;
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

        public LeadDataForCARLogService GetDataForCARLogService(string ticketId)
        {
            try
            {
                string slmdbName = GetDBName();
                string sql = @"SELECT lead.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName, lead.slm_ChannelId AS ChannelId, lead.slm_Product_Group_Id AS ProductGroupId, lead.slm_Product_Id AS ProductId
                                , lead.slm_Status AS StatusCode, opt.slm_OptionDesc AS StatusName, lead.slm_SubStatus AS SubStatusCode, lead.slm_ExternalSubStatusDesc AS SubStatusName
                                , cus.slm_CitizenId AS CitizenId, cus.slm_CardType AS CardTypeId, cardtype.slm_CardTypeName AS CardTypeName, pg.product_name AS ProductGroupName, mp.sub_product_name AS ProductName
                                , renew.slm_LicenseNo AS LicenseNo, renew.slm_ContractNo AS ContractNo
                                , LEAD.slm_Owner AS Owner, STAFF.slm_StaffEmail AS OwnerEmail, LEAD.slm_Delegate AS Delegate, STAFF2.slm_StaffEmail AS DelegateEmail
                                , ISNULL(title.slm_TitleName, '') + lead.slm_Name + ' ' + ISNULL(lead.slm_LastName, '') AS CustomerName
                                , STAFF3.slm_UserName AS MarketingOwner, STAFF3.slm_StaffEmail AS MarketingOwnerEmail
                                , STAFF4.slm_UserName AS LastOwner, STAFF4.slm_StaffEmail AS LastOwnerEmail, slm_Prelead_Id AS PreleadId, cardtype.slm_CARSubscriptionTypeId AS SubScriptionTypeId 
                                FROM " + slmdbName + @".dbo.kkslm_tr_lead LEAD
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_cusinfo cus ON cus.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_renewinsurance renew ON renew.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_prelead prelead ON prelead.slm_TicketId = LEAD.slm_ticketId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_option opt ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' AND opt.is_Deleted = '0'
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_campaign cam ON cam.slm_CampaignId = lead.slm_CampaignId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_cardtype cardtype ON cardtype.slm_CardTypeId = cus.slm_CardType
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_staff STAFF ON LEAD.slm_Owner = STAFF.slm_UserName AND STAFF.is_Deleted = 0
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_staff STAFF2 ON LEAD.slm_Delegate = STAFF2.slm_UserName AND STAFF2.is_Deleted = 0
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_staff STAFF3 ON LEAD.coc_MarketingOwner = STAFF3.slm_EmpCode AND STAFF3.is_Deleted = 0
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_staff STAFF4 ON LEAD.coc_LastOwner = STAFF4.slm_EmpCode AND STAFF4.is_Deleted = 0
                                LEFT JOIN " + slmdbName + @".dbo.CMT_MS_PRODUCT_GROUP pg ON lead.slm_Product_Group_Id = pg.product_id
                                LEFT JOIN " + slmdbName + @".dbo.CMT_MAPPING_PRODUCT mp ON mp.sub_product_id = lead.slm_Product_Id
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId AND title.is_Deleted = 0
                                WHERE LEAD.slm_ticketId = '" + ticketId + "' ";

                return slmdb.ExecuteStoreQuery<LeadDataForCARLogService>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoAutoRuleAssign(kkslm_tr_lead lead, CustomerDetail cusDetail, DateTime createdDate)
        {
            try
            {
                int startTimeHour = 0;
                int startTimeMin = 0;
                int endTimeHour = 0;
                int endTimeMin = 0;

                var calendarTab = slmdb.kkslm_ms_calendar_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch && p.is_Deleted == false).ToList();
                GetStartEndTime(lead, out startTimeHour, out startTimeMin, out endTimeHour, out endTimeMin);

                if (!string.IsNullOrEmpty(lead.slm_Owner))
                {
                    lead.slm_AssignedFlag = "1";
                    lead.slm_AssignedDate = GetAssignDate(lead, calendarTab, startTimeHour, startTimeMin, endTimeHour, endTimeMin, createdDate);
                    lead.slm_AssignType = "05";
                    lead.slm_CurrentRuleState = "AddOwner";
                    lead.slm_Counting = 0;
                    lead.slm_EmailFlag = lead.slm_Status;
                    lead.slm_NextSLA = CalculateNextSla(lead, calendarTab, startTimeHour, startTimeMin, endTimeHour, endTimeMin);

                    kkslm_tr_activity act = new kkslm_tr_activity()
                    {
                        slm_TicketId = lead.slm_ticketId,
                        slm_Type = lead.slm_AssignType,
                        slm_NewOwner = lead.slm_Owner,
                        slm_NewOwner_Position = lead.slm_Owner_Position,
                        slm_WorkDesc = "staff is : false",
                        slm_SystemAction = "SLM",
                        slm_SystemActionBy = "SLM",
                        slm_CreatedBy = "SYSTEM BATCH",
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = "SYSTEM BATCH",
                        slm_UpdatedDate = createdDate,
                        is_Deleted = 0
                    };
                    slmdb.kkslm_tr_activity.AddObject(act);
                }

                if (cusDetail != null && !string.IsNullOrEmpty(cusDetail.DelegateUsername))
                {
                    var delegateStaaff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == cusDetail.DelegateUsername).FirstOrDefault();
                    if (delegateStaaff != null)
                    {
                        lead.slm_Delegate = delegateStaaff.slm_UserName;
                        lead.slm_Delegate_Position = delegateStaaff.slm_Position_id;
                        lead.slm_DelegateDate = createdDate;
                        lead.slm_Delegate_Flag = 0;         //delegate, 0 คือ assign แล้ว
                        lead.slm_Delegate_Branch = delegateStaaff.slm_BranchCode;
                    }
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        private void GetStartEndTime(kkslm_tr_lead lead, out int startTimeHour, out int startTimeMin, out int endTimeHour, out int endTimeMin)
        {
            try
            {
                startTimeHour = 0;
                startTimeMin = 0;
                endTimeHour = 0;
                endTimeMin = 0;

                var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch).FirstOrDefault();

                if (branch == null)
                {
                    throw new ServiceException("", "ไม่พบข้อมูลสาขาสำหรับคำนวณ Next SLA, BranchCode:" + lead.slm_Owner_Branch);
                }

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
                        {
                            throw new ServiceException("", "ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);
                        }  
                    }
                    else
                    {
                        throw new ServiceException("", "ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);
                    }

                    if (end != null)
                    {
                        string[] str = end.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            endTimeHour = Convert.ToInt32(str[0]);
                            endTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                        {
                            throw new ServiceException("", "ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                        }
                    }
                    else
                    {
                        throw new ServiceException("", "ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                    } 
                }
                else
                {
                    startTimeHour = int.Parse(branch.slm_StartTime_Hour);
                    endTimeMin = int.Parse(branch.slm_StartTime_Minute);
                    endTimeHour = int.Parse(branch.slm_EndTime_Hour);
                    endTimeMin = int.Parse(branch.slm_EndTime_Minute);
                }
            }
            catch
            {
                throw;
            }
        }

        private DateTime GetAssignDate(kkslm_tr_lead lead, List<kkslm_ms_calendar_branch> calendarTab, int startTimeHour, int startTimeMin, int endTimeHour, int endTimeMin, DateTime createdDate)
        {
            try
            {
                int checkStartTime = Convert.ToInt32(startTimeHour.ToString("00") + startTimeMin.ToString("00"));
                int checkEndTime = Convert.ToInt32(endTimeHour.ToString("00") + endTimeMin.ToString("00"));
                int timeToCheck = Convert.ToInt32(createdDate.Hour.ToString("00") + createdDate.Minute.ToString("00"));

                //เช็กว่าวันที่ทำการรัน batch เป็นวันหยุดหรือไม่
                if (calendarTab.Any(p => p.slm_HolidayDate.Date == createdDate.Date))
                {
                    createdDate = GetNextDate(createdDate, 1);
                    createdDate = BuildDateTime(createdDate.Year, createdDate.Month, createdDate.Day, startTimeHour, startTimeMin, 0);

                    while (calendarTab.Any(p => p.slm_HolidayDate.Date == createdDate.Date))
                    {
                        createdDate = GetNextDate(createdDate, 1);
                    }
                }
                else
                {
                    if (timeToCheck < checkStartTime || timeToCheck > checkEndTime)     //ถ้าเวลาไม่ได้อยู่ในช่วงเวลาทำงาน
                    {
                        if (timeToCheck >= checkEndTime && timeToCheck <= 2359)
                        {
                            createdDate = GetNextDate(createdDate, 1); //createdDate.AddDays(1);
                            createdDate = BuildDateTime(createdDate.Year, createdDate.Month, createdDate.Day, startTimeHour, startTimeMin, 0);
                        }
                        else
                        {
                            createdDate = BuildDateTime(createdDate.Year, createdDate.Month, createdDate.Day, startTimeHour, startTimeMin, 0);
                        }

                        while (calendarTab.Any(p => p.slm_HolidayDate.Date == createdDate.Date))
                        {
                            createdDate = GetNextDate(createdDate, 1);
                        }
                    }
                }

                return createdDate;
            }
            catch
            {
                throw;
            }
        }

        private DateTime GetNextDate(DateTime date, int numOfNextDate)
        {
            return date.AddDays(numOfNextDate);
        }

        private DateTime BuildDateTime(int year, int month, int day, int hour, int min, int sec)
        {
            return new DateTime(year, month, day, hour, min, sec);
        }

        private DateTime CalculateNextSla(kkslm_tr_lead lead, List<kkslm_ms_calendar_branch> calendarTab, int startTimeHour, int startTimeMin, int endTimeHour, int endTimeMin)
        {
            try
            {
                string campaignId = lead.slm_CampaignId;
                string channelId = lead.slm_ChannelId;
                string productId = lead.slm_Product_Id;
                int sla_minute = 480;
                DateTime assigndDate = lead.slm_AssignedDate.Value;
                kkslm_ms_sla slaMaster = null;
                int workingMinPerDay = 0;

                slaMaster = slmdb.kkslm_ms_sla.Where(p => p.slm_CampaignId == campaignId && p.slm_ChannelId == channelId && p.slm_StatusCode == "00" && p.is_Deleted == 0).FirstOrDefault();
                if (slaMaster == null)
                {
                    slaMaster = slmdb.kkslm_ms_sla.Where(p => p.slm_ProductId == productId && p.slm_ChannelId == channelId && p.slm_StatusCode == "00" && p.is_Deleted == 0).FirstOrDefault();
                }
                if (slaMaster == null)
                {
                    slaMaster = slmdb.kkslm_ms_sla.Where(p => p.slm_CampaignId == "DEFAULT" && p.slm_ProductId == "DEFAULT" && p.is_Deleted == 0).FirstOrDefault();
                }
                if (slaMaster == null)
                {
                    throw new ServiceException("", string.Format("ไม่พบข้อมูล SLA, CampaignId={0}, ProductId={1}, ChannelId={2}", campaignId, productId, channelId));
                }

                sla_minute = slaMaster.slm_SLA_Minutes;

                //หาเวลาทำงานใน 1 วัน
                DateTime tmpStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startTimeHour, startTimeMin, 0);
                DateTime tmpEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endTimeHour, endTimeMin, 0);

                TimeSpan tmpTs = tmpEnd.Subtract(tmpStart);
                workingMinPerDay = Convert.ToInt32(tmpTs.TotalMinutes);     //ได้เวลาที่ต้องทำงานในแต่ละวันของสาขา หน่วยเป็นนาที

                //นำ sla minute ที่เหลือไปคำนวณหาว่า ต้อ next sla ไปกี่วันกี่นาที
                int numOfDay = sla_minute / workingMinPerDay;       //จำนวนวันที่เหลือ
                int numOfMin = sla_minute % workingMinPerDay;       //จำนวนนาทีที่เหลือ

                DateTime processDate = assigndDate;
                for (int i = 1; i <= numOfDay; i++)
                {
                    processDate = GetNextDate(processDate, 1);      //processDate.AddDays(1);
                    while(calendarTab.Any(p => p.slm_HolidayDate.Date == processDate.Date))
                    {
                        processDate = GetNextDate(processDate, 1);  //processDate.AddDays(1);
                    }
                }

                DateTime endTimeProcessDate = new DateTime(processDate.Year, processDate.Month, processDate.Day, endTimeHour, endTimeMin, 0);
                int remainMinInDay = Convert.ToInt32(endTimeProcessDate.Subtract(processDate).TotalMinutes);
                if (numOfMin < remainMinInDay)      //ถ้าจำนวนนาทีที่เหลือน้อยกว่า เวลางานที่เหลือภายในวัน
                {
                    return processDate.AddMinutes(numOfMin);
                }
                else
                {
                    numOfMin -= remainMinInDay;
                    processDate = GetNextDate(processDate, 1);    //processDate.AddDays(1);       //เพิ่มไปวันถัดไป
                    processDate = BuildDateTime(processDate.Year, processDate.Month, processDate.Day, startTimeHour, startTimeMin, 0);   //เซ็ทเวลาเริ่มงาน

                    while (calendarTab.Any(p => p.slm_HolidayDate.Date == processDate.Date))
                    {
                        processDate = GetNextDate(processDate, 1);    //processDate.AddDays(1);
                    }

                    return processDate.AddMinutes(numOfMin);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
