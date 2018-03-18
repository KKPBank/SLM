using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SLMBatch.Business
{
    public class SLM_PRO_02_Biz : ServiceBase
    {
        private int TotalExportExcel = 0;
        private string ErrorStep = string.Empty;
        private string ErrorDetail = string.Empty;

        public void UpdateStatusBackEnd(string batchCode)
        {
            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;

            DataTable dtSuccess = new DataTable("Success");
            DataTable dtFail = new DataTable("Fail");

            dtSuccess.Columns.AddRange(new DataColumn[] {
                new DataColumn("TicketID", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Surname", typeof(string)),
                new DataColumn("Channel", typeof(string)),
                new DataColumn("Campaign", typeof(string)),
                new DataColumn("Detail", typeof(string)),
                new DataColumn("Status", typeof(string)),
                new DataColumn("SubStatus", typeof(string)),
                new DataColumn("UpdatedDate", typeof(DateTime)),
                new DataColumn("System", typeof(string))
            });
            dtFail.Columns.AddRange(new DataColumn[] {
                new DataColumn("TicketID", typeof(string)),
                new DataColumn("Status", typeof(string)),
                new DataColumn("Reason", typeof(string)),
                new DataColumn("UpdatedDate", typeof(DateTime)),
                new DataColumn("System", typeof(string))
            });

            try
            {
                ErrorStep = "Set Start Time";

                batchMonitorId = BizUtil.SetStartTime(batchCode);

                ErrorStep = "GetExtSystemCurrentStatusViewList";

                var list = GetExtSystemCurrentStatusViewList();
                totalRecord = list.Count;

                foreach (ExtSystemCurrentStatusData data in list)
                {
                    try
                    {
                        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

                        ErrorStep = "Find TicketId = " + data.TicketId;
                        var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == data.TicketId && p.is_Deleted == 0).FirstOrDefault();
                        if (lead == null)
                        {
                            throw new Exception("Ticket Id " + data.TicketId + " not found in SLM");
                        }
                        ErrorStep = "CheckStatus TicketId = " + data.TicketId;
                        CheckStatus(slmdb, lead, data, batchCode);

                        ErrorStep = "CheckPhoneCallHistory TicketId = " + data.TicketId;
                        CheckPhoneCallHistory(slmdb, lead, data);

                        slmdb.SaveChanges();

                        dtSuccess.Rows.Add("'" + data.TicketId, data.Name, data.LastName, data.Channel, data.Campaign, data.StatusDesc, data.SlmStatus, String.IsNullOrEmpty(data.SubStatusCode) ? data.StatusName : data.StatusName + " - " + data.SubStatusCode, data.StatusDate, data.StatusSystem);

                        totalSuccess += 1;
                        Console.WriteLine("TicketId " + data.TicketId + ": SUCCESS");
                    }
                    catch (Exception ex)
                    {
                        totalFail += 1;
                        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        string errorDetail = "TicketId=" + data.TicketId + ", Error=" + message;

                        dtFail.Rows.Add("'" + data.TicketId, "Update Fail", "\"" + message + "\"", data.StatusDate, data.StatusSystem);

                        BizUtil.InsertLog(batchMonitorId, data.TicketId, "", errorDetail);
                        Util.WriteLogFile(logfilename, batchCode, errorDetail);

                        Console.WriteLine("TicketId " + data.TicketId + ": FAIL");
                    }
                }

                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);

                ErrorStep = "Export CSV Success";
                if (dtSuccess.Rows.Count > 0)
                {
                    SaveCSVToSharePath(GenerateString(dtSuccess), "SLM-LotusNotes_Report_Success_" + String.Format("{0:yyyyMMdd }", DateTime.Now));
                }
                if (dtFail.Rows.Count > 0)
                {
                    SaveCSVToSharePath(GenerateString(dtFail), "SLM-LotusNotes_Report_Fail_" + String.Format("{0:yyyyMMdd }", DateTime.Now));
                }

                TotalExportExcel = totalSuccess + totalFail;

                ErrorStep = "Send Mail";
                SendMail();

                dtSuccess.Dispose();
                dtFail.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                dtFail.Rows.Add("#", "Fail", "Fail", DateTime.Now);

                Util.WriteLogFile(logfilename, batchCode, message);
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);

                dtSuccess.Dispose();
                dtFail.Dispose();
            }

        }

        //public static void UpdateStatusBackEnd(string batchCode)
        //{
        //    Int64 batchMonitorId = 0;
        //    int totalRecord = 0;
        //    int totalSuccess = 0;
        //    int totalFail = 0;
        //    int i = 1;
        //    int countCheck = GetCountCheck();
        //    int interval_second = (GetInterval() * 60) * 1000;
        //    bool success = false;

        //    try
        //    {
        //        while (i <= countCheck && success == false)
        //        {
        //            if (CheckViewReady())
        //            {
        //                batchMonitorId = BizUtil.SetStartTime(batchCode);

        //                var list = GetExtSystemCurrentStatusViewList();
        //                totalRecord = list.Count;

        //                foreach (ExtSystemCurrentStatusData data in list)
        //                {
        //                    try
        //                    {
        //                        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
        //                        var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == data.TicketId && p.is_Deleted == 0).FirstOrDefault();
        //                        if (lead == null) { throw new Exception("Ticket Id " + data.TicketId + " not found in SLM"); }

        //                        CheckStatus(slmdb, lead, data);
        //                        slmdb.SaveChanges();

        //                        totalSuccess += 1;
        //                        Console.WriteLine("TicketId " + data.TicketId + ": SUCCESS");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        totalFail += 1;
        //                        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                        string errorDetail = "TicketId=" + data.TicketId + ", Error=" + message;

        //                        BizUtil.InsertLog(batchMonitorId, data.TicketId, "", errorDetail);
        //                        Util.WriteLogFile(logfilename, batchCode, errorDetail);

        //                        Console.WriteLine("TicketId " + data.TicketId + ": FAIL");
        //                    }
        //                }

        //                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);

        //                success = true;
        //            }
        //            else
        //            {
        //                System.Threading.Thread.Sleep(interval_second);
        //            }

        //            i += 1;
        //        }

        //        if (success == false)
        //        {
        //            batchMonitorId = BizUtil.SetStartTime(batchCode);
        //            BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);

        //            BizUtil.InsertLog(batchMonitorId, "", "", "View v_kkslm_ext_sys_status is not ready");
        //            Util.WriteLogFile(logfilename, batchCode, "View v_kkslm_ext_sys_status is not ready");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("All FAIL");
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

        //        Util.WriteLogFile(logfilename, batchCode, message);
        //        BizUtil.InsertLog(batchMonitorId, "", "", message);
        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
        //        throw ex;
        //    }
        //}

        private void CheckStatus(SLMDBEntities slmdb, kkslm_tr_lead lead, ExtSystemCurrentStatusData data, string batchCode)
        {
            try
            {
                string newExternalSystem = "";
                string newExternalStatus = "";
                string newExternalSubStatus = "";
                string newExternalSubStatusDesc = "";
                string oldExternalSystem = "";
                string oldExternalStatus = "";
                string oldExternalSubStatus = "";
                string oldExternalSubStatusDesc = "";
                string slmOldStatus = "";

                slmOldStatus = lead.slm_Status;
                oldExternalSystem = lead.slm_ExternalSystem;                                                                    //add 15/10/2015
                oldExternalStatus = lead.slm_ExternalStatus;                                                                    //add 15/10/2015
                oldExternalSubStatus = string.IsNullOrEmpty(lead.slm_ExternalSubStatus) ? null : lead.slm_ExternalSubStatus;    //add 15/10/2015
                oldExternalSubStatusDesc = lead.slm_ExternalSubStatusDesc;                                                      //add 15/10/2015

                if (string.IsNullOrEmpty(data.StatusSystem)) { throw new Exception("System Name in view is null or blank"); }

                string slmNewStatus = GetSlmStatusCode(slmdb, data.StatusSystem, data.StatusCode, data.SubStatusCode, lead.slm_Product_Id);
                if (slmNewStatus == "") throw new Exception("Cannot find slmStatusCode from the specified " + data.StatusSystem + " mapping statusCode=" + data.StatusCode + ", subStatusCode=" + data.SubStatusCode + ", slmProductId=" + lead.slm_Product_Id);

                var statusBy_username = slmdb.kkslm_ms_staff.Where(p => p.slm_EmpCode == data.StatusBy && p.is_Deleted == 0).Select(p => p.slm_UserName).FirstOrDefault();

                //Change 15/10/2015
                newExternalSystem = data.StatusSystem.Trim().ToUpper();
                newExternalStatus = data.StatusCode;
                newExternalSubStatus = string.IsNullOrEmpty(data.SubStatusCode) ? null : data.SubStatusCode;

                if (!string.IsNullOrEmpty(data.StatusName) && !string.IsNullOrEmpty(data.SubStatusCode))
                    newExternalSubStatusDesc = data.StatusName + " - " + data.SubStatusCode;
                else if (!string.IsNullOrEmpty(data.StatusName))
                    newExternalSubStatusDesc = data.StatusName;
                else if (!string.IsNullOrEmpty(data.SubStatusCode))
                    newExternalSubStatusDesc = data.SubStatusCode;

                if (slmNewStatus != slmOldStatus || newExternalStatus != oldExternalStatus || newExternalSubStatus != oldExternalSubStatus || newExternalSystem != oldExternalSystem)
                {
                    DateTime updateDate = DateTime.Now;

                    kkslm_tr_activity activity = new kkslm_tr_activity()
                    {
                        slm_TicketId = data.TicketId,
                        slm_OldStatus = slmOldStatus,
                        slm_NewStatus = slmNewStatus,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedBy_Position = null,
                        slm_CreatedDate = updateDate,
                        slm_Type = "09",
                        slm_SystemAction = data.StatusSystem.Trim().ToUpper(),      //System ที่เข้ามาทำ action (19/03/2015)
                        slm_SystemActionBy = "SLM",                                 //action เกิดขึ้นที่ระบบอะไร (19/03/2015)
                        slm_ExternalSystem_Old = oldExternalSystem,                 //add 15/10/2015
                        slm_ExternalStatus_Old = oldExternalStatus,                 //add 15/10/2015
                        slm_ExternalSubStatus_Old = oldExternalSubStatus,           //add 15/10/2015
                        slm_ExternalSubStatusDesc_Old = oldExternalSubStatusDesc,   //add 15/10/2015
                        slm_ExternalSystem_New = newExternalSystem,                 //add 15/10/2015
                        slm_ExternalStatus_New = newExternalStatus,                 //add 15/10/2015
                        slm_ExternalSubStatus_New = newExternalSubStatus,           //add 15/10/2015
                        slm_ExternalSubStatusDesc_New = newExternalSubStatusDesc    //add 15/10/2015
                    };
                    slmdb.kkslm_tr_activity.AddObject(activity);

                    AppUtil.CalculateTotalSLA(slmdb, lead, activity, data.StatusDate.Value, logfilename, batchCode);           //add 25/03/2016
                    lead.slm_Status = slmNewStatus;
                    lead.slm_StatusBy = (!string.IsNullOrEmpty(statusBy_username) ? statusBy_username : data.StatusBy);
                    lead.slm_StatusDate = data.StatusDate;
                    lead.slm_StatusDateSource = data.StatusDate;
                    lead.slm_Counting = 0;
                    lead.slm_UpdatedBy = "SYSTEM";
                    lead.slm_UpdatedDate = updateDate;

                    lead.slm_ExternalSystem = newExternalSystem;
                    lead.slm_ExternalStatus = newExternalStatus;
                    lead.slm_ExternalSubStatus = newExternalSubStatus;
                    lead.slm_ExternalSubStatusDesc = newExternalSubStatusDesc;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void CheckPhoneCallHistory(SLMDBEntities slmdb, kkslm_tr_lead lead, ExtSystemCurrentStatusData data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.StatusDesc))
                {
                    if (data.StatusDesc.Trim().Length > 4000) throw new Exception("Field kkslm_ext_StatusDesc exceeds 4000 character limitation");

                    kkslm_phone_call pc = new kkslm_phone_call()
                    {
                        slm_TicketId = lead.slm_ticketId,
                        slm_ContactDetail = data.StatusDesc.Trim(),
                        slm_CreateBy = "SYSTEM",
                        slm_CreatedBy_Position = null,
                        slm_CreateDate = DateTime.Now,
                        slm_Status = lead.slm_Status,
                        slm_Owner = lead.slm_Owner,
                        slm_Owner_Position = lead.slm_Owner_Position,
                        slm_ContactPhone = null,
                        is_Deleted = 0
                    };
                    slmdb.kkslm_phone_call.AddObject(pc);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetSlmStatusCode(SLMDBEntities slmdb, string systemName, string statusCode, string subStatusCode, string productId)
        {
            string slmStatus = "";

            if (string.IsNullOrEmpty(subStatusCode))
                slmStatus = slmdb.kkslm_ms_mapping_status.Where(p => p.is_Deleted == false && p.slm_System == systemName && p.slm_Product_Id == productId && p.slm_Mapping_Status_Code == statusCode && p.slm_Mapping_SubStatus_Code == null).Select(p => p.slm_Status_Code).FirstOrDefault();
            else
                slmStatus = slmdb.kkslm_ms_mapping_status.Where(p => p.is_Deleted == false && p.slm_System == systemName && p.slm_Product_Id == productId && p.slm_Mapping_Status_Code == statusCode && p.slm_Mapping_SubStatus_Code == subStatusCode).Select(p => p.slm_Status_Code).FirstOrDefault();

            return string.IsNullOrEmpty(slmStatus) ? "" : slmStatus;
        }

        private List<ExtSystemCurrentStatusData> GetExtSystemCurrentStatusViewList()
        {
            try
            {
                DateTime batchDate = GetBatchRunningDate();

                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                int numOfDay = AppConstant.HPAOLNumOfDay - 1;
                DateTime startDate = new DateTime();

                if (numOfDay > 0)
                    startDate = batchDate.AddDays(-numOfDay);
                else
                    startDate = batchDate;

                string end_date = batchDate.Year.ToString() + batchDate.ToString("-MM-dd");
                string start_date = startDate.Year.ToString() + startDate.ToString("-MM-dd");

                //                string sql = @"SELECT Ticket_Id AS TicketId, Campaign_Id AS CampaignId, Status_System AS StatusSystem, [Status] AS StatusCode, SubStatus AS SubStatusCode
                //                                , Status_Name AS StatusName, Status_By AS StatusBy, Status_Date AS StatusDate
                //                                FROM " + AppConstant.SLMDBName + @".dbo.V_EXT_System_Current_Status
                //                                WHERE CONVERT(DATE, Status_Date) BETWEEN '" + start_date + "' AND '" + end_date + @"'
                //                                AND Ticket_Id IS NOT NULL";

                //string sql = @"SELECT kkslm_ext_TicketId AS TicketId, kkslm_ext_CampaignId AS CampaignId, kkslm_ext_SystemName AS StatusSystem, kkslm_ext_Status AS StatusCode, kkslm_ext_SubStatus AS SubStatusCode
                //                , kkslm_ext_StatusName AS StatusName, kkslm_ext_UpdatedBy AS StatusBy, kkslm_ext_UpdatedDate AS StatusDate, kkslm_ext_StatusDesc AS StatusDesc
                //                FROM " + AppConstant.SLMDBName + @".dbo.v_kkslm_ext_sys_status
                //                WHERE CONVERT(DATE, kkslm_ext_UpdatedDate) BETWEEN '" + start_date + "' AND '" + end_date + @"'
                //                AND kkslm_ext_TicketId IS NOT NULL";

                string sql = @"SELECT kkslm_ext_TicketId AS TicketId, kkslm_ext_CampaignId AS CampaignId, kkslm_ext_SystemName AS StatusSystem, kkslm_ext_Status AS StatusCode, kkslm_ext_SubStatus AS SubStatusCode
                                    , kkslm_ext_StatusName AS StatusName, kkslm_ext_UpdatedBy AS StatusBy, kkslm_ext_UpdatedDate AS StatusDate, REPLACE(REPLACE(kkslm_ext_StatusDesc, CHAR(13), ''), CHAR(10), '') AS StatusDesc
                                    , cam.slm_CampaignName AS Campaign, cha.slm_ChannelDesc AS Channel , opt.slm_OptionDesc AS SlmStatus, lead.slm_Name AS Name, cus.slm_LastName AS LastName
                                    FROM " + AppConstant.SLMDBName + @".dbo.v_kkslm_ext_sys_status 
                                    LEFT join " + AppConstant.SLMDBName + @".dbo.kkslm_tr_lead lead ON kkslm_ext_TicketId = lead.slm_ticketId
                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam ON lead.slm_CampaignId = cam.slm_CampaignId
                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_mapping_status map ON kkslm_ext_Status = map.slm_Mapping_Status_Code
                                    AND isnull(kkslm_ext_SubStatus,'zz') = isnull(map.slm_Mapping_SubStatus_Code,'zz')
                                    AND map.slm_Product_Id = lead.slm_Product_Id
                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_option opt ON map.slm_Status_Code = opt.slm_OptionCode 
                                    AND opt.slm_OptionType = 'lead status'
                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_cusinfo cus ON kkslm_ext_TicketId = cus.slm_TicketId
                                    LEFT JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_ms_channel cha ON lead.slm_ChannelId = cha.slm_ChannelId
                                    WHERE CONVERT(DATE, kkslm_ext_UpdatedDate) BETWEEN '" + start_date + "' AND '" + end_date + @"'
                                    AND kkslm_ext_TicketId IS NOT NULL";

                return slmdb.ExecuteStoreQuery<ExtSystemCurrentStatusData>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DateTime GetBatchRunningDate()
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

                DateTime batchRunningDate = new DateTime();
                string readFromConfig = AppConstant.ReadBatchDateFromConfig;
                string batch_date = AppConstant.BatchDate;

                if (readFromConfig != null && readFromConfig.ToUpper() == "Y" && batch_date != null)
                {
                    string[] str = batch_date.Split('-');
                    batchRunningDate = new DateTime(Convert.ToInt32(str[0]), Convert.ToInt32(str[1]), Convert.ToInt32(str[2]));
                }
                else
                {
                    int endTimeHour = 0;

                    var desc = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "endTime").Select(p => p.slm_OptionDesc).FirstOrDefault();
                    if (desc != null)
                    {
                        string[] str = desc.Split(':');
                        if (str.Count() > 0 && str[0] != "")
                        {
                            endTimeHour = Convert.ToInt32(str[0]);
                        }
                        else
                            endTimeHour = 22;
                    }
                    else
                        endTimeHour = 22;

                    DateTime now = DateTime.Now;
                    DateTime runningDate = new DateTime();
                    DateTime rangeStart = new DateTime(now.Year, now.Month, now.Day, endTimeHour, 0, 0);
                    DateTime rangeEnd = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

                    if (now >= rangeStart && now <= rangeEnd)
                        runningDate = now;
                    else
                        runningDate = now.AddDays(-1);

                    batchRunningDate = new DateTime(runningDate.Year, runningDate.Month, runningDate.Day);
                }

                return batchRunningDate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveCSVToSharePath(StringBuilder sb, string FileName)
        {
            try
            {
                foreach (var tempPath in AppConstant.SLM_PRO_02_PATH.Split(','))
                {
                    using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                    {

                        if (unc.NetUseWithCredentials(tempPath, AppConstant.SLM_PRO_02_USERNAME, AppConstant.SLM_PRO_02_DOMAIN, AppConstant.SLM_PRO_02_PASSWORD))
                        {
                            File.WriteAllText(tempPath + FileName + ".csv", sb.ToString(), Encoding.GetEncoding(AppConstant.SLM_PRO_02_ENCODE));
                        }
                        else
                        {
                            throw new Exception("Cannot access path " + tempPath);
                        }

                        unc.Dispose();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private StringBuilder GenerateString(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            sb.AppendLine(string.Join(",", columnNames));
            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                sb.AppendLine(string.Join(",", fields));
            }
            return sb;
        }

        private void SendMail()
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();

            MailAddress fromAddress = new MailAddress(AppConstant.WSLogEmailFromAddress, AppConstant.WSLogEmailDisplayName.Trim() ?? "");

            client.Host = AppConstant.WSLogEmailHostIP;
            client.Port = AppConstant.WSLogEmailPort;
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(AppConstant.WSLogEmailFromAddress, AppConstant.WSLogEmailFromPassword);
            client.Timeout = 300000;

            mail.From = fromAddress;

            foreach (string mailto in AppConstant.WSLogEmailToAddress.Split(';'))
            {
                mail.To.Add(new MailAddress(mailto));
            }

            mail.Subject = ErrorDetail.Trim() == string.Empty ? "KK SLM Service: ออกรายงาน Batch Update Status สำเร็จ" : "KK SLM Service: ออกรายงาน Batch Update Status ไม่สำเร็จ";
            mail.IsBodyHtml = true;
            mail.Body = CreateTemplateEmail().ToString();

            client.Send(mail);
        }

        private StringBuilder CreateTemplateEmail()
        {
            StringBuilder builder = new StringBuilder();
            using (StreamReader reader = new StreamReader(string.Format("{0}\\Template\\Mail\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "SLM_PRO_06_EmailTemplate.html")))
            {
                builder.Append(reader.ReadToEnd());
                builder.Replace("{MAIL_SUBJECT}", ErrorDetail.Trim() == string.Empty ? "KK SLM Service: ออกรายงาน Batch Update Status สำเร็จ" : "KK SLM Service: ออกรายงาน Batch Update Status ไม่สำเร็จ");
                builder.Replace("{DATE_TO_PROCESS}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                builder.Replace("{PURPOSE}", ErrorDetail.Trim() == string.Empty ? "Batch สำหรับออกรายงาน Update Status สำเร็จ" : "Batch สำหรับออกรายงาน Update Status สำเร็จ");
                builder.Replace("{TOTAL_EXPORT_ROW}", TotalExportExcel.ToString());
                builder.Replace("{STEP_NAME}", ErrorDetail.Trim() == string.Empty ? string.Empty : ErrorStep);
                builder.Replace("{ERROR_DESCRIPTION}", ErrorDetail);
            }
            return builder;
        }

        //private static bool CheckViewReady()
        //{
        //    return true;
        //}

        //private static int GetCountCheck()
        //{
        //    try
        //    {
        //        return ConfigurationManager.AppSettings["CountCheck"] != null ? int.Parse(ConfigurationManager.AppSettings["CountCheck"]) : 5;
        //    }
        //    catch
        //    {
        //        return 5;
        //    }
        //}

        //private static int GetInterval()
        //{
        //    try
        //    {
        //        return ConfigurationManager.AppSettings["Interval"] != null ? int.Parse(ConfigurationManager.AppSettings["Interval"]) : 10;
        //    }
        //    catch
        //    {
        //        return 10;  //Minutes
        //    }
        //}
    }
}
