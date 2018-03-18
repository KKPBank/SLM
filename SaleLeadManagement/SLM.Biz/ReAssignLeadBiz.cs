using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Dal;
using SLM.Resource;
using log4net;

namespace SLM.Biz
{
    public class ReAssignLeadBiz : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReAssignLeadBiz));

        public string ErrorMessage { get; set; }

        public bool ValidateData(List<ReAssignLeadData> dataList, out List<ReAssignValidateDataError> errorList)
        {
            errorList = new List<ReAssignValidateDataError>();
            try
            {
                StringBuilder sb = new StringBuilder();
                var teamList = new KKSlmMsTeamtelesalesModel().GetList();
                var userList = new KKSlmMsStaffModel().GetList();
                string errMessage = "";
                var renewList = new List<TicketContract>();
                var preleadList = new List<TicketContract>();

                //List contractNo ที่อยู่ใน Excel ออกมา เพื่อไปหาว่ามีใน prelead เท่าไร
                var dataContractNoList = dataList.Where(p => p.ContractNo != null).Select(p => p.ContractNo).Distinct().ToList();
                
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = @"SELECT renew.slm_TicketId AS TicketId, renew.slm_ContractNo AS ContractNo, pre.slm_Prelead_Id AS PreleadId 
                                    , lead.slm_Status AS [Status], lead.slm_CampaignId AS CampaignId 
                                    FROM dbo.kkslm_tr_renewinsurance renew
                                    INNER JOIN dbo.kkslm_tr_lead lead on lead.slm_TicketId = renew.slm_TicketId
                                    LEFT JOIN dbo.kkslm_tr_prelead pre on pre.slm_TicketId = lead.slm_ticketId
                                    WHERE lead.is_Deleted = 0 ";

                    renewList = slmdb.ExecuteStoreQuery<TicketContract>(sql).ToList();

                    preleadList = slmdb.kkslm_tr_prelead.Where(p => p.is_Deleted == false && dataContractNoList.Contains(p.slm_Contract_Number))
                                        .Select(p => new TicketContract {
                                            ContractNo = p.slm_Contract_Number,
                                            TicketId = p.slm_TicketId,
                                            PreleadId = p.slm_Prelead_Id,
                                            SubStatus = p.slm_SubStatus
                                        }).ToList();
                }

                foreach (var data in dataList)
                {
                    sb.Clear();
                    errMessage = "";

                    if (string.IsNullOrWhiteSpace(data.EmpCode))
                    {
                        sb.Append("Column B : กรุณาระบุรหัสพนักงาน<br />");
                    }
                    else
                    {
                        var owner = userList.FirstOrDefault(p => p.EmpCode == data.EmpCode);
                        if (owner == null)
                        {
                            sb.Append("Column B : ไม่พบรหัสพนักงานในระบบ<br />");
                        }
                        else
                        {
                            data.Username = owner.UserName;
                            data.StaffId = owner.StaffId;
                            data.OwnerBranchCode = owner.BranchCode;
                            data.OwnerPositionId = owner.PositionId;
                            data.TeamTelesalesId = owner.TeamTelesalesId;
                            data.TeamTelesalesCode = owner.TeamTelesalesCode;
                        }
                    }

                    errMessage = ValidateSectionLead(data, renewList, preleadList, dataContractNoList);
                    if (!string.IsNullOrWhiteSpace(errMessage))
                    {
                        sb.Append(errMessage);
                    }

                    if (sb.Length > 0)
                    {
                        errorList.Add(new ReAssignValidateDataError { RowNo = data.RowNo, ErrorMessage = sb.ToString() });
                    }
                }

                return errorList.Count == 0;
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return false;
            }
        }

        private string ValidateSectionLead(ReAssignLeadData data, List<TicketContract> renewList, List<TicketContract> preleadList, List<string> dataContractNoList)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.ContractNo))
                {
                    return "Column F : กรุณาระบุเลขที่สัญญา<br />";
                }

                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<TicketContract> renewByContractNo;

                    renewByContractNo = renewList.Where(p => p.ContractNo == data.ContractNo && p.PreleadId == data.PreleadId).ToList();
                    if (renewByContractNo.Count > 0)  //มีใน renewinsurance
                    {
                        data.LeadState = ReAssignLeadData.State.Lead;

                        if (!string.IsNullOrWhiteSpace(data.TicketId)) //ถ้ามี TicketId ใน Excel ให้เช็กต่อว่าสัมพันธ์กับ ContractNo หรือไม่ 
                        {
                            var lead = renewByContractNo.Where(p => p.TicketId == data.TicketId && p.ContractNo == data.ContractNo && p.PreleadId == data.PreleadId).FirstOrDefault();
                            if (lead == null)
                            {
                                return "Column E, F : TicketId และเลขที่สัญญา ไม่สัมพันธ์กับในระบบ<br />";
                            }
                            if (lead.Status == SLMConstant.StatusCode.Reject || lead.Status == SLMConstant.StatusCode.Cancel || lead.Status == SLMConstant.StatusCode.Close)
                            {
                                return "Ticket ID มีสถานะเป็น Inactive<br />";
                            }

                            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
                            if (!staff.PassPrivilegeCampaign(SLMConstant.Branch.Active, lead.CampaignId, data.Username))
                            {
                                return "Column B, E : พนักงานไม่มีสิทธิในแคมเปญที่ผูกกับ Ticket ID นี้<br />";
                            }
                        }
                        else
                        {
                            //ไม่มีข้อมูล TicketId ใน Excel
                            return "Column E : กรุณาระบุ TicketId<br />";
                        }
                    }
                    else
                    {
                        //ไม่มีใน renew ให้หาต่อใน prelead
                        data.LeadState = ReAssignLeadData.State.Prelead;

                        if (!string.IsNullOrWhiteSpace(data.TicketId))
                        {
                            return "Column E, F, L : TicketId, เลขที่สัญญา, Prelead ID ไม่สัมพันธ์กับในระบบ<br />";
                        }

                        var list = preleadList.Where(p => p.ContractNo == data.ContractNo && p.PreleadId == data.PreleadId).ToList();
                        if (list.Count == 0)
                        {
                            return "Column F, L : เลขที่สัญญาและ Prelead ID ไม่สัมพันธ์กับในระบบ<br />";
                        }
                        if (list.Count > 1)
                        {
                            return "Column F, L : Duplicate เลขที่สัญญากับ Prelead ID<br />";
                        }
                        if (list[0].SubStatus == SLMConstant.SubStatusCode.CarSeized)
                        {
                            return "เลขที่สัญญามีสถานะเป็น Inactive<br />";
                        }
                        if (!string.IsNullOrWhiteSpace(list[0].TicketId))   //ไม่มีใน renew แต่มี Ticket เป็นเคสผิดปกติ
                        {
                            return "Column F : มี Ticket ID ผูกกับเลขที่สัญญาใน Outbound แต่ไม่พบใน Inbound<br />";
                        }
                    }
                }

                return "";
            }
            catch
            {
                throw;
            }
        }

        public bool ReAssignLead(List<ReAssignLeadData> dataList, string reAssignByUsername)
        {
            try
            {
                //Prelead
                var preleadIdList = dataList.Where(p => p.LeadState == ReAssignLeadData.State.Prelead).Select(p => p.PreleadId).ToList();
                 
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    var reAssignByPosition = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == reAssignByUsername).Select(p => p.slm_Position_id).FirstOrDefault();
                    var preleadList = slmdb.kkslm_tr_prelead.Where(p => preleadIdList.Contains(p.slm_Prelead_Id)).ToList();
                    var staffList = slmdb.kkslm_ms_staff.Where(p => p.is_Deleted == 0)
                                        .Select(p => new { EmpCode = p.slm_EmpCode, Username = p.slm_UserName, PositionId = p.slm_Position_id }).ToList();

                    foreach (var prelead in preleadList)
                    {
                        var data = dataList.Where(p => p.PreleadId == prelead.slm_Prelead_Id).FirstOrDefault();
                        if (data != null && data.EmpCode != prelead.slm_Owner)
                        {
                            prelead.slm_OldOwner = prelead.slm_Owner;
                            prelead.slm_Owner = data.EmpCode;

                            if (data.TeamTelesalesId != null)
                            {
                                prelead.slm_Owner_Team = Convert.ToInt32(data.TeamTelesalesId);
                            }
                            else
                            {
                                prelead.slm_Owner_Team = null;
                            }

                            prelead.slm_Owner_Position = data.OwnerPositionId;
                            prelead.slm_OwnerBranch = data.OwnerBranchCode;
                            prelead.slm_UpdatedBy = reAssignByUsername;
                            prelead.slm_UpdatedDate = DateTime.Now;
                            prelead.slm_AssignFlag = "0";
                            prelead.slm_Assign_Status = "4";

                            var oldOwner = staffList.Where(p => p.EmpCode == prelead.slm_OldOwner).FirstOrDefault();
                            //Change Owner OwnerLogging
                            kkslm_tr_prelead_activity act = new kkslm_tr_prelead_activity
                            {
                                slm_Prelead_Id = prelead.slm_Prelead_Id,
                                slm_CreatedDate = DateTime.Now,
                                slm_CreatedBy = reAssignByUsername,
                                slm_CreatedBy_Position = reAssignByPosition,
                                slm_UpdatedDate = DateTime.Now,
                                slm_UpdatedBy = reAssignByUsername,
                                is_Deleted = 0,
                                slm_NewOwner = data.Username,
                                slm_NewOwner_Position = data.OwnerPositionId,
                                slm_OldOwner = oldOwner != null ? oldOwner.Username : null,
                                slm_OldOwner_Position = oldOwner != null ? oldOwner.PositionId : null,
                                slm_Type = SLMConstant.ActionType.ChangeOwner,
                                slm_SystemAction = SLMConstant.SystemNameOBT,
                                slm_SystemActionBy = SLMConstant.SystemNameOBT
                            };
                            slmdb.kkslm_tr_prelead_activity.AddObject(act);

                            //kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                            //{
                            //    slm_Cmt_Product_Id = prelead.slm_Cmt_Product_Id,
                            //    slm_CmtLot = prelead.slm_CmtLot,
                            //    slm_OldAssignType = null, //oldStatus,
                            //    slm_NewAssignType = "UA",
                            //    slm_ContractNo = prelead.slm_Contract_Number,
                            //    slm_CreatedBy = reAssignByUsername,
                            //    slm_CreatedDate = DateTime.Now
                            //};
                            //slmdb.kkslm_tr_prelead_assign_log.AddObject(log);

                            slmdb.SaveChanges();
                        }
                    }
                }

                //Lead
                var ticketIdList = dataList.Where(p => p.LeadState == ReAssignLeadData.State.Lead).Select(p => p.TicketId).ToList();
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    var reAssignByPosition = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == reAssignByUsername).Select(p => p.slm_Position_id).FirstOrDefault();
                    var leadList = slmdb.kkslm_tr_lead.Where(p => ticketIdList.Contains(p.slm_ticketId)).ToList();
                    int? oldOwnerPosition;

                    foreach (var lead in leadList)
                    {
                        var data = dataList.Where(p => p.TicketId == lead.slm_ticketId).FirstOrDefault();
                        if (data != null && data.StaffId != lead.slm_StaffId)       //ใช้ Staff Id แทน slm_Owner
                        {
                            oldOwnerPosition = lead.slm_Owner_Position;

                            lead.slm_OldOwner = lead.slm_Owner;
                            lead.slm_Owner = data.Username;
                            lead.slm_Owner_Branch = data.OwnerBranchCode;
                            lead.slm_Owner_Position = data.OwnerPositionId;
                            lead.slm_StaffId = data.StaffId;
                            lead.slm_AssignedDate = DateTime.Now;
                            lead.slm_AssignedFlag = "1";
                            lead.slm_AssignedBy = "SYSTEM";
                            lead.slm_NextSLA = CalculateNextSla(slmdb, lead);
                            lead.slm_UpdatedBy = reAssignByUsername;
                            lead.slm_UpdatedDate = DateTime.Now;

                            //Change Owner OwnerLogging
                            kkslm_tr_activity act = new kkslm_tr_activity
                            {
                                slm_CreatedDate = DateTime.Now,
                                slm_CreatedBy = reAssignByUsername,
                                slm_CreatedBy_Position = reAssignByPosition,
                                slm_UpdatedDate = DateTime.Now,
                                slm_UpdatedBy = reAssignByUsername,
                                is_Deleted = 0,
                                slm_TicketId = lead.slm_ticketId,
                                slm_NewOwner = lead.slm_Owner,
                                slm_NewOwner_Position = lead.slm_Owner_Position,
                                slm_OldOwner = lead.slm_OldOwner,
                                slm_OldOwner_Position = oldOwnerPosition,
                                slm_Type = SLMConstant.ActionType.ChangeOwner,
                                slm_SystemAction = SLMConstant.SystemName,
                                slm_SystemActionBy = SLMConstant.SystemName,
                            };
                            slmdb.kkslm_tr_activity.AddObject(act);

                            //ทำงานแทน Rule OwnerLogging
                            kkslm_tr_activity actByRule = new kkslm_tr_activity
                            {
                                slm_CreatedDate = DateTime.Now,
                                slm_CreatedBy = "SYSTEM",
                                slm_UpdatedDate = DateTime.Now,
                                slm_UpdatedBy = "SYSTEM",
                                is_Deleted = 0,
                                slm_NewOwner = lead.slm_Owner,
                                slm_NewOwner_Position = lead.slm_Owner_Position,
                                slm_OldOwner = lead.slm_OldOwner,
                                slm_OldOwner_Position = oldOwnerPosition,
                                slm_Type = SLMConstant.ActionType.UserAssign,
                                slm_TicketId = lead.slm_ticketId,
                                slm_WorkDesc = "staff is : false",
                                slm_NewCounting = 0,
                                slm_SystemAction = SLMConstant.SystemName,
                                slm_SystemActionBy = SLMConstant.SystemName
                            };
                            slmdb.kkslm_tr_activity.AddObject(actByRule);

                            slmdb.SaveChanges();
                        }
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                log.Error(string.Format("Method=ReAssignLead, Error={0}",  ErrorMessage));
                return false;
            }
        }

        private bool GetStartEndTime(SLM_DBEntities slmdb, kkslm_tr_lead lead, out int startTimeHour, out int startTimeMin, out int endTimeHour, out int endTimeMin)
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
                    log.Error("ไม่พบข้อมูลสาขาสำหรับคำนวณ Next SLA, BranchCode:" + lead.slm_Owner_Branch);
                    return false;
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
                            log.Error("ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);
                            return false;
                        }
                    }
                    else
                    {
                        log.Error("ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);
                        return false;
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
                            log.Error("ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                            return false;
                        }
                    }
                    else
                    {
                        log.Error("ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                        return false;
                    }
                }
                else
                {
                    startTimeHour = int.Parse(branch.slm_StartTime_Hour);
                    endTimeMin = int.Parse(branch.slm_StartTime_Minute);
                    endTimeHour = int.Parse(branch.slm_EndTime_Hour);
                    endTimeMin = int.Parse(branch.slm_EndTime_Minute);
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        private DateTime? CalculateNextSla(SLM_DBEntities slmdb, kkslm_tr_lead lead)
        {
            try
            {
                int startTimeHour = 0;
                int startTimeMin = 0;
                int endTimeHour = 0;
                int endTimeMin = 0;

                var calendarTab = slmdb.kkslm_ms_calendar_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch && p.is_Deleted == false).ToList();
                bool ret = GetStartEndTime(slmdb, lead, out startTimeHour, out startTimeMin, out endTimeHour, out endTimeMin);
                if (!ret)
                {
                    return null;
                }

                string campaignId = lead.slm_CampaignId;
                string channelId = lead.slm_ChannelId;
                string productId = lead.slm_Product_Id;
                int sla_minute = 480;
                DateTime assigndDate = lead.slm_AssignedDate.Value;
                kkslm_ms_sla slaMaster = null;
                int workingMinPerDay = 0;

                slaMaster = slmdb.kkslm_ms_sla.Where(p => p.slm_CampaignId == campaignId && p.slm_ChannelId == channelId && p.slm_StatusCode == lead.slm_Status && p.is_Deleted == 0).FirstOrDefault();
                if (slaMaster == null)
                {
                    slaMaster = slmdb.kkslm_ms_sla.Where(p => p.slm_ProductId == productId && p.slm_ChannelId == channelId && p.slm_StatusCode == lead.slm_Status && p.is_Deleted == 0).FirstOrDefault();
                }
                if (slaMaster == null)
                {
                    slaMaster = slmdb.kkslm_ms_sla.Where(p => p.slm_CampaignId == "DEFAULT" && p.slm_ProductId == "DEFAULT" && p.is_Deleted == 0).FirstOrDefault();
                }
                if (slaMaster == null)
                {
                    log.Error(string.Format("Method=CalculateNextSla, ไม่พบข้อมูล SLA, CampaignId={0}, ProductId={1}, ChannelId={2}", campaignId, productId, channelId));
                    return null;
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
                    processDate = GetNextDate(processDate, 1);      
                    while (calendarTab.Any(p => p.slm_HolidayDate.Date == processDate.Date))
                    {
                        processDate = GetNextDate(processDate, 1);  
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
                    processDate = GetNextDate(processDate, 1);      //เพิ่มไปวันถัดไป
                    processDate = BuildDateTime(processDate.Year, processDate.Month, processDate.Day, startTimeHour, startTimeMin, 0);   //เซ็ทเวลาเริ่มงาน

                    while (calendarTab.Any(p => p.slm_HolidayDate.Date == processDate.Date))
                    {
                        processDate = GetNextDate(processDate, 1); 
                    }

                    return processDate.AddMinutes(numOfMin);
                }
            }
            catch(Exception ex)
            {
                log.Error(string.Format("Method=CalculateNextSla, Error={0}", ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
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

        #region IDisposable

        bool _disposed;
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
