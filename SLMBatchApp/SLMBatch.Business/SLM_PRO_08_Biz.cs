using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Globalization;
using System.Data;
using System.Transactions;
using Microsoft.VisualBasic;
using log4net;
using System.Net;
using System.Net.Mail;

namespace SLMBatch.Business
{
    public class SLM_PRO_08_Biz : ServiceBase
    {
        private readonly ILog _logger;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();

        public SLM_PRO_08_Biz(){
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            _logger = LogManager.GetLogger(typeof(SLM_PRO_08_Biz));
        }

        #region Member
        //private string ErrorStep = string.Empty;
        private string ErrorDetail = string.Empty;
        private string ErrorStep = string.Empty;
        #endregion


        public bool UpdateBatchCARInsertStatus(string batchCode) {
            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;
            bool ret = false;

            try {
                ErrorStep = "Set start time";
                _logger.Info("I:-- Start Batch --:--UpdateBatchCARInsertStatus--" + batchCode);
                batchMonitorId = BizUtil.SetStartTime(batchCode);

                ErrorStep = "GetFileList";
                _logger.Info(_logMsg.Clear().Add("GetFileList", batchCode).ToInputLogString());
                //1. ดึง List ของไฟล์ที่มีใน FTPs โดยเอาไฟล์ที่สร้างมาตั้งแต่ เมื่อวาน วันนี้ และวันพรุ่งนี้ โดยดูจาก FileName
                IEnumerable<string> files = GetFileList();

                if (files.Count<string>() > 0)
                {
                    ret = true;

                    ErrorStep = "Validate Text History " + files.Count<string>().ToString() + " file(s)";
                    //2. เก็บชื่อไฟล์ลง DB เพื่อเตรียมทำการ Process กรณีมีไฟล์อยู่แล้ว ก็ไม่ต้องทำซ้ำ
                    List<kkslm_ext_sys_status_cbs_file> fileList = new List<kkslm_ext_sys_status_cbs_file>();
                    foreach (string f in files)
                    {
                        FileInfo fInfo = new FileInfo(f);
                        ErrorStep = "Validate Text History " + fInfo.Name;
                        _logger.Info(_logMsg.Clear().Add("Validate Text History", fInfo.Name).ToInputLogString());
                        
                        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                        var file = checkFileHistory(fInfo.Name, slmdb);
                        if (file != null)
                        {
                            file.kkslm_filename = fInfo.Name;
                            file.kkslm_filepath = f;
                            file.kkslm_file_created_date = fInfo.CreationTime;
                            file.kkslm_file_process_time = DateTime.Now;
                            file.kkslm_process_status = AppConstant.InProcess;

                            if (file.kkslm_ext_sys_status_cbs_file_id == 0)
                            {
                                slmdb.kkslm_ext_sys_status_cbs_file.AddObject(file);
                                _logger.Info(_logMsg.Clear().Add("Process New File", fInfo.Name).ToInputLogString());
                            }
                            slmdb.SaveChanges();
                            fileList.Add(file);
                        }
                    }

                    foreach (kkslm_ext_sys_status_cbs_file file in fileList)
                    {
                        //3. เช็ค Format TextFile ทุก Record
                        ErrorStep = "Validate Text Format " + file.kkslm_filename;
                        _logger.Info(_logMsg.Clear().Add("Validate Text Format", file.kkslm_filename).ToInputLogString());
                        ValidateTextfileData validTextFormat = ValidateTextFileFormat(file.kkslm_filepath);
                        if (validTextFormat.IsValid == false)
                        {
                            ErrorStep = "Invalid Textfile " + file.kkslm_filename;
                            _logger.Error(_logMsg.Clear().Add("Invalid Textfile", file.kkslm_filename).Add("ErrorMessage", validTextFormat.ErrorMessage).ToInputLogString());
                            UpdateFileStatus(file.kkslm_ext_sys_status_cbs_file_id, AppConstant.Fail, "Invalid Textfile " + file.kkslm_filename + Environment.NewLine + validTextFormat.ErrorMessage);
                            break;
                        }

                        //4. Process Text File ตาม Requirement
                        ErrorStep = "GetEBatchCARInsertStatusList " + file.kkslm_filename;
                        _logger.Info(_logMsg.Clear().Add("GetEBatchCARInsertStatusList", "").Add("FileName", file.kkslm_filename).ToInputLogString());
                        var lists = GetEBatchCARInsertStatusList(file.kkslm_filepath, file.kkslm_ext_sys_status_cbs_file_id);
                        if (lists != null)
                        {
                            bool isSuccess = true;
                            totalRecord = lists.Count;
                            foreach (BatchCARInsertStatusData data in lists)
                            {
                                try
                                {
                                    SLMDBEntities slmdb = new SLMDBEntities();
                                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                                    {
                                        ErrorStep = "Find TicketId = " + data.RefSystemId;
                                        var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == data.RefSystemId && p.is_Deleted == 0).FirstOrDefault();
                                        if (lead == null)
                                        {
                                            _logger.ErrorFormat("Ticket Id {0} not found in SLM", data.RefSystemId);
                                            throw new Exception("Ticket Id " + data.RefSystemId + " not found in SLM");
                                        }

                                        ErrorStep = "CheckStatus TicketId = " + data.RefSystemId;
                                        _logger.Info(_logMsg.Clear().Add("Check Ticket Status", " Ticket Id:" + data.RefSystemId));
                                        CheckStatus(slmdb, lead, data, batchCode);

                                        ErrorStep = "CheckPhoneCallHistory TicketId = " + data.RefSystemId;
                                        _logger.Info(_logMsg.Clear().Add("Check Phone Call History", " Ticket Id:" + data.RefSystemId));
                                        CheckPhoneCallHistory(slmdb, lead, data);

                                        slmdb.SaveChanges();

                                        totalSuccess += 1;
                                        _logger.Info(_logMsg.Clear().Add("TicketId", data.RefSystemId + ": SUCCESS").ToInputLogString());

                                        ts.Complete();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    totalFail += 1;
                                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                                    string errorDetail = "TicketId=" + data.RefSystemId + ", Error=" + message;

                                    BizUtil.InsertLog(batchMonitorId, data.RefSystemId, "", errorDetail);

                                    _logger.Error(_logMsg.Clear().Add("TicketId", data.ReferenceNo + ": FAIL").ToInputLogString());
                                }
                            }

                            ErrorStep = "Set end time";
                            BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);


                            //5. นำข้อมูลใน Text File เก็บลงใน Archives
                            int dataRow = 0;
                            ErrorStep = "InsertArchiveData";
                            _logger.Info(_logMsg.Clear().Add("InsertArchiveData", "TABLE : kkslm_ext_sys_status_cbs_file_data").ToInputLogString());
                            foreach (BatchCARInsertStatusData list in lists)
                            {
                                SLMDBEntities slmdb = new SLMDBEntities();
                                kkslm_ext_sys_status_cbs_file_data data = new kkslm_ext_sys_status_cbs_file_data
                                {
                                    kkslm_ext_sys_status_cbs_file_id = file.kkslm_ext_sys_status_cbs_file_id,
                                    kkslm_reference_code = list.HeaderData.ReferenceCode,
                                    kkslm_file_name = list.HeaderData.FileName,
                                    kkslm_create_date = list.HeaderData.CreateDate,
                                    kkslm_current_sequence = Convert.ToInt16(list.HeaderData.CurrentSequence),
                                    kkslm_total_sequence = Convert.ToInt16(list.HeaderData.TotalSequence),
                                    kkslm_total_record = Convert.ToInt16(list.HeaderData.TotalRecord),
                                    kkslm_system_code = list.HeaderData.SystemCode,
                                    kkslm_reference_no = list.ReferenceNo,
                                    kkslm_channel_id = list.ChannelID,
                                    kkslm_status_date_time = list.StatusDateTime,
                                    kkslm_subscription_id = list.SubscriptionID,
                                    kkslm_subscription_cus_type = list.SubscriptionCusType,
                                    kkslm_subscription_card_type = list.SubscriptionCardType,
                                    kkslm_owner_system_id = list.OwnerSystemId,
                                    kkslm_owner_system_code = list.OwnerSystemCode,
                                    kkslm_ref_system_id = list.RefSystemId,
                                    kkslm_ref_system_code = list.RefSystemCode,
                                    kkslm_status = list.Status,
                                    kkslm_status_name = list.StatusName
                                };
                                dataRow += 1;

                                slmdb.kkslm_ext_sys_status_cbs_file_data.AddObject(data);
                                isSuccess = (slmdb.SaveChanges() > 0);
                                if (isSuccess == false)
                                {
                                    ErrorStep = "Error InsertArchiveData at row " + dataRow.ToString();
                                    break;
                                }
                            }

                            if (isSuccess == true)
                            {
                                ErrorStep = "MoveBatchFileToArchive";
                                _logger.Info(_logMsg.Clear().Add("MoveBatchFileToArchive", "Filename:" + file.kkslm_filename));
                                if (MoveBatchFileToArchive(file.kkslm_filepath) == true)
                                {
                                    ErrorStep = "UpdateSuccessStatus Filename:" + file.kkslm_filename;
                                    _logger.Info(_logMsg.Clear().Add("UpdateSuccessStatus", "Filename:" + file.kkslm_filename));
                                    UpdateFileStatus(file.kkslm_ext_sys_status_cbs_file_id, AppConstant.Success, "");
                                }
                            }
                        }
                    }

                    ErrorStep = "Send Mail";
                    SendMail(totalRecord);
                }
                else {
                    ret = false;
                }
            }
            catch (Exception ex) {
                _logger.Error("Exception occur:\n", ex);
                ret = false;
            }

            return ret;
        }


        #region Update LEAD Status
        private void CheckStatus(SLMDBEntities slmdb, kkslm_tr_lead lead, BatchCARInsertStatusData data, string batchCode)
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
                oldExternalSystem = lead.slm_ExternalSystem;
                oldExternalStatus = lead.slm_ExternalStatus;
                oldExternalSubStatus = string.IsNullOrEmpty(lead.slm_ExternalSubStatus) ? null : lead.slm_ExternalSubStatus;
                oldExternalSubStatusDesc = lead.slm_ExternalSubStatusDesc;

                if (string.IsNullOrEmpty(data.OwnerSystemCode)) { throw new Exception("System Name in view is null or blank"); }

                string slmNewStatus = GetSlmStatusCode(slmdb, data.OwnerSystemCode, data.Status, "", lead.slm_Product_Id);
                if (slmNewStatus == "") throw new Exception("Cannot find slmStatusCode from the specified " + data.Status + " mapping statusCode=" + data.Status + ", slmProductId=" + lead.slm_Product_Id);

                newExternalSystem = data.OwnerSystemCode.Trim().ToUpper();
                newExternalStatus = data.Status;
                newExternalSubStatus = data.Status;
                newExternalSubStatusDesc = data.StatusName;

                if (slmNewStatus != slmOldStatus || newExternalStatus != oldExternalStatus  || newExternalSubStatus != oldExternalSubStatus || newExternalSystem != oldExternalSystem)
                {
                    DateTime updateDate = DateTime.Now;

                    kkslm_tr_activity activity = new kkslm_tr_activity()
                    {
                        slm_TicketId = data.RefSystemId,
                        slm_OldStatus = slmOldStatus,
                        slm_NewStatus = slmNewStatus,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedBy_Position = null,
                        slm_CreatedDate = updateDate,
                        slm_Type = "09",
                        slm_SystemAction = data.OwnerSystemCode.Trim().ToUpper(),      //System ที่เข้ามาทำ action (18/10/2017)
                        slm_SystemActionBy = "SLM",                                 //action เกิดขึ้นที่ระบบอะไร (18/10/2017)
                        slm_ExternalSystem_Old = oldExternalSystem,                 
                        slm_ExternalStatus_Old = oldExternalStatus,                 
                        slm_ExternalSubStatus_Old = oldExternalSubStatus,           
                        slm_ExternalSubStatusDesc_Old = oldExternalSubStatusDesc,   
                        slm_ExternalSystem_New = newExternalSystem,                 
                        slm_ExternalStatus_New = newExternalStatus,
                        slm_ExternalSubStatus_New = newExternalSubStatus,
                        slm_ExternalSubStatusDesc_New = newExternalSubStatusDesc              
                    };
                    slmdb.kkslm_tr_activity.AddObject(activity);

                    DateTime StatusDateTime = data.StatusDateTime.ConvertStringToStatusDateTime();

                    AppUtil.CalculateTotalSLA(slmdb, lead, activity, StatusDateTime, logfilename, batchCode);
                    lead.slm_Status = slmNewStatus;
                    lead.slm_StatusDate = StatusDateTime;
                    lead.slm_StatusDateSource = StatusDateTime;
                    lead.slm_Counting = 0;
                    lead.slm_UpdatedBy = "SYSTEM";
                    lead.slm_UpdatedDate = updateDate;

                    lead.slm_ExternalSystem = newExternalSystem;
                    lead.slm_ExternalStatus = newExternalStatus;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void CheckPhoneCallHistory(SLMDBEntities slmdb, kkslm_tr_lead lead, BatchCARInsertStatusData data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.StatusName))
                {
                    if (data.StatusName.Trim().Length > 4000) throw new Exception("Field kkslm_ext_StatusDesc exceeds 4000 character limitation");

                    kkslm_phone_call pc = new kkslm_phone_call()
                    {
                        slm_TicketId = lead.slm_ticketId,
                        slm_ContactDetail = data.StatusName.Trim(),
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
        #endregion


        private void SendMail(int totalRecord)
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();

            MailAddress fromAddress = new MailAddress(AppConstant.SLM_PRO_08_EmailFromAddress, AppConstant.SLM_PRO_08_EmailDisplayName.Trim() ?? "");

            client.Host = AppConstant.SLM_PRO_08_EmailHostIP;
            client.Port = AppConstant.SLM_PRO_08_EmailPort;
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(AppConstant.WSLogEmailFromAddress, AppConstant.WSLogEmailFromPassword);
            client.Timeout = 300000;

            mail.From = fromAddress;

            foreach (string mailto in AppConstant.SLM_PRO_08_EmailToAddress.Split(';'))
            {
                mail.To.Add(new MailAddress(mailto));
            }

            mail.Subject = ErrorDetail.Trim() == string.Empty ? "KK SLM Service: Process Textfile BatchCARInsertStatus สำเร็จ" : "KK SLM Service: Process Textfile BatchCARInsertStatus ไม่สำเร็จ";
            mail.IsBodyHtml = true;
            mail.Body = CreateTemplateEmail(totalRecord).ToString();

            client.Send(mail);
        }

        private StringBuilder CreateTemplateEmail(int TotalRow)
        {
            StringBuilder builder = new StringBuilder();
            using (StreamReader reader = new StreamReader(string.Format("{0}\\Template\\Mail\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "SLM_PRO_08_EmailTemplate.html")))
            {
                builder.Append(reader.ReadToEnd());
                builder.Replace("{MAIL_SUBJECT}", ErrorDetail.Trim() == string.Empty ? "KK SLM Service: Process Textfile BatchCARInsertStatus สำเร็จ" : "KK SLM Service: Process Textfile BatchCARInsertStatus ไม่สำเร็จ");
                builder.Replace("{DATE_TO_PROCESS}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                builder.Replace("{PURPOSE}", ErrorDetail.Trim() == string.Empty ? "Batch CAR Insert Status สำเร็จ" : "Batch CAR Insert Status ไม่สำเร็จ");
                builder.Replace("{TOTAL_PROCESS_ROW}", TotalRow.ToString());
                builder.Replace("{STEP_NAME}", ErrorDetail.Trim() == string.Empty ? string.Empty : ErrorStep);
                builder.Replace("{ERROR_DESCRIPTION}", ErrorDetail);
            }
            return builder;
        }

        #region Text File Data
        private void UpdateFileStatus(int kkslmExtSysStatusCbsFileID, string statusName, string errorMessage) {
            try {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                var query = slmdb.kkslm_ext_sys_status_cbs_file.Where(f => f.kkslm_ext_sys_status_cbs_file_id == kkslmExtSysStatusCbsFileID).FirstOrDefault();
                if (query != null)
                {
                    query.kkslm_process_status = statusName;
                    query.kkslm_process_error_step = errorMessage;

                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        private ValidateTextfileData ValidateTextFileFormat(string filePath)
        {
            ValidateTextfileData ret = new ValidateTextfileData();
            ret.IsValid = true;
            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    string line;
                    int HeaderTotalRecord = 0;
                    int DetailTotalRecord = 0;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.Trim() == "") continue;

                        string[] tmp = line.Split('|');
                        if (tmp.Length == 9)
                        {  //Header Row Digit แรกต้องเป็นตัว H
                            if (tmp[0] == AppConstant.BatchCARInsertStatus.TYPE_OF_DATA_HEAD)  //TYPE OF DATA
                            {
                                if (tmp[1].Trim() == "") {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "ReferenceCode is blank";
                                    break;
                                }


                                if (tmp[2].Trim() == ""){
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "FileName is blank";
                                    break;
                                }

                                if (tmp[3].Trim() == "")
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "CreateDate is blank";
                                    break;
                                }

                                if (tmp[4].Trim() == "")
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "CurrentSequence is blank";
                                    break;
                                }
                                if (Information.IsNumeric(tmp[4]) == false)
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "CurrentSequence is not numberic";
                                    break;
                                }

                                if (tmp[5].Trim() == "")
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "TotalSequence is blank";
                                    break;
                                }
                                if (Information.IsNumeric(tmp[5]) == false)
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "TotalSequence is not numberic";
                                    break;
                                }

                                if (tmp[6].Trim() == "")
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "TotalRecord is blank";
                                    break;
                                }
                                if (Information.IsNumeric(tmp[6]) == false)
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "TotalRecord is not numberic";
                                    break;
                                }

                                if (tmp[7].Trim() == "") {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "SystemCode is blank";
                                    break;
                                }
                                
                                HeaderTotalRecord = Convert.ToInt16(tmp[6]);
                            }
                            else {
                                ret.IsValid = false;
                                ret.ErrorMessage = "Header row is invalid format";
                                break;
                            }
                        }
                        else if (tmp.Length == 13) //Data Row Digit แรกต้องเป็นตัว D
                        {
                            if (tmp[0] == AppConstant.BatchCARInsertStatus.TYPE_OF_DATA_DETAIL)
                            {
                                if (tmp[1] == "") //ข้อมูลที่ส่งมาเป็น " " ก็เลยไม่ต้อง Trim
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "ReferenceNo at row " + DetailTotalRecord + " is blank";
                                    break;
                                }

                                if (tmp[2].Trim() == "")
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "ChannelID at row " + DetailTotalRecord + " is blank";
                                    break;
                                }

                                if (tmp[3].Trim() == "")
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "StatusDateTime at row " + DetailTotalRecord + " is blank";
                                    break;
                                }

                                if (tmp[7].Trim() == "") {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "OwnerSystemId at row " + DetailTotalRecord + " is blank";
                                    break;
                                }

                                if (tmp[8].Trim() == "")
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "OwnerSystemCode at row " + DetailTotalRecord + " is blank";
                                    break;
                                }

                                if (tmp[11].Trim() == "")
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "Status at row " + DetailTotalRecord + " is blank";
                                    break;
                                }

                                if (tmp[12].Trim() == "")
                                {
                                    ret.IsValid = false;
                                    ret.ErrorMessage = "StatusName at row " + DetailTotalRecord + " is blank";
                                    break;
                                }
                            }
                            else {
                                ret.IsValid = false;
                                ret.ErrorMessage = "Type of data row " + DetailTotalRecord.ToString() + " is invalid";
                                break;
                            }

                            DetailTotalRecord += 1;
                        }
                        else {
                            //Invalid File Format
                            ret.IsValid = false;
                            ret.ErrorMessage = "Detail row " + DetailTotalRecord.ToString() + " is invalid format";
                            break;
                        }
                    }

                    //if (ret.IsValid == true)
                    //{
                    //    if (HeaderTotalRecord != DetailTotalRecord)
                    //    {
                    //        //ถ้าข้อมูลจำนวน Record ใน Header กับจำนวน Record ใน Detail ไม่เท่ากัน
                    //        ret.IsValid = false;
                    //        ret.ErrorMessage = "Header total record not match with detail record";
                    //    }
                    //}
                }
            }
            catch (Exception ex) {
                ret.IsValid = false;
                ret.ErrorMessage = "Exception : " + ex.Message.ToString();
            }
            
            return ret;
        }

        private List<string> GetFileList(){

            List<string> ret = new List<string>();
            string fiPrefix = AppConstant.SLM_PRO_08_File_Prefix;   //BchCARInsertSts_
            string NameYesterday = fiPrefix + DateTime.Now.AddDays(0 - AppConstant.SLM_PRO_08_IntervalDay).ToString("yyyyMMdd");
            string NameTomorrow = fiPrefix + DateTime.Now.AddDays(AppConstant.SLM_PRO_08_IntervalDay).ToString("yyyyMMdd");

            string host = AppConstant.SLM_PRO_08_SSH_Server;
            int port = AppConstant.SLM_PRO_08_SSH_Port;
            string username = AppConstant.SLM_PRO_08_SSH_Username;
            string password = AppConstant.SLM_PRO_08_SSH_Password;

            string localPath = AppConstant.SLM_PRO_08_PathImport;
            if (Directory.Exists(localPath) == false)
                Directory.CreateDirectory(localPath);
            // . always refers to the current directory.
            string remoteDirectory = AppConstant.SLM_PRO_08_SSH_RemoteDir;


            using (var sftp = new SftpClient(host, port, username, password))
            {
                sftp.Connect();
                if (IsDirectoryExists(sftp, remoteDirectory))
                {
                    var files = sftp.ListDirectory(remoteDirectory)
                                .Where(x => !x.IsDirectory && !x.IsSymbolicLink && x.Name != "." && x.Name != ".."
                                && x.Name.ToUpperInvariant().StartsWith(fiPrefix.ToUpperInvariant()));
                    
                    bool isFileFound = files.Any();
                    if (isFileFound)
                    {
                        List<string> filterFileList = new List<string>();
                        DateTime startDate = DateTime.Now.AddDays(0 - AppConstant.SLM_PRO_08_IntervalDay);
                        DateTime endDate = DateTime.Now.AddDays(AppConstant.SLM_PRO_08_IntervalDay);
                        DateTime currDate = startDate;
                        do
                        {
                            filterFileList.Add((fiPrefix + currDate.ToString("yyyyMMdd")).ToUpperInvariant());
                            currDate = currDate.AddDays(1);
                        } while (currDate <= endDate);


                        // Download file to local via SFTP
                        foreach (var file in files)
                        {
                            if (filterFileList.Contains(file.Name.ToUpperInvariant().Substring(0, NameYesterday.Length)))
                            {
                                string fInfo = DownloadFile(sftp, file, localPath);
                                if (fInfo != null)
                                    ret.Add(fInfo);
                            }
                        }

                        _logger.Info(_logMsg.Clear().SetPrefixMsg("Download Files Via FTP").ToSuccessLogString());
                    }
                    else
                    {
                        _logger.Info(_logMsg.Clear().SetPrefixMsg("Download Files Via FTP").Add("Error Message", "File Not Found").ToFailLogString());
                    }
                }

                sftp.Disconnect();
            }

            return ret;
        }

        private string DownloadFile(SftpClient client, Renci.SshNet.Sftp.SftpFile file, string directory)
        {
            string ret = "";
            try
            {
                _logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").Add("FileName", file.FullName).ToInputLogString());

                string PathFile = Path.Combine(directory, file.Name);
                using (var fileStream = File.OpenWrite(PathFile))
                {
                    client.DownloadFile(file.FullName, fileStream);
                    if (File.Exists(PathFile) == true)
                        ret = PathFile;
                }

                _logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").ToSuccessLogString());
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
                _logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").Add("Error Message", ex.Message).ToInputLogString());
                throw;
            }

            return ret;
        }

        private bool IsDirectoryExists(SftpClient client, string path)
        {
            bool isDirectoryExist = false;

            try
            {
                client.ChangeDirectory(path);
                isDirectoryExist = true;
            }
            catch (SftpPathNotFoundException)
            {
                return false;
            }

            return isDirectoryExist;
        }

        private kkslm_ext_sys_status_cbs_file checkFileHistory(string FileName, SLMDBEntities slmdb)
        {
            kkslm_ext_sys_status_cbs_file ret = null;
            try
            {
                var query = from f in slmdb.kkslm_ext_sys_status_cbs_file
                            where f.kkslm_filename == FileName
                            select f;

                if (query.Any() == true)
                {
                    ret = query.Take(1).Where(x => x.kkslm_process_status == AppConstant.Fail).FirstOrDefault();
                }
                else {
                    ret = new kkslm_ext_sys_status_cbs_file();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;

        }

        #endregion
        private List<BatchCARInsertStatusData> GetEBatchCARInsertStatusList(string filePath, int cbsFileId)
        {
            List<BatchCARInsertStatusData> ret = null;
            try
            {
                BatchCARInsertStatusHeaderData head = null;
                using (var streamReader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.Trim() == "") continue;

                        string[] tmp = line.Split('|');
                        if (tmp.Length == 9)
                        {  //Header Row Digit แรกต้องเป็นตัว H
                            if (tmp[0] == AppConstant.BatchCARInsertStatus.TYPE_OF_DATA_HEAD)  //TYPE OF DATA
                            {
                                head = new BatchCARInsertStatusHeaderData
                                {
                                    ReferenceCode = tmp[1],
                                    FileName = tmp[2],
                                    CreateDate = tmp[3],
                                    CurrentSequence = Convert.ToInt16(tmp[4]),
                                    TotalSequence = Convert.ToInt16(tmp[5]),
                                    TotalRecord = Convert.ToInt16(tmp[6]),
                                    SystemCode = tmp[7]
                                };

                                ret = new List<BatchCARInsertStatusData>();
                            }
                        }
                        else if (tmp.Length == 13) //Data Row Digit แรกต้องเป็นตัว D
                        {
                            if (tmp[0] == AppConstant.BatchCARInsertStatus.TYPE_OF_DATA_DETAIL)
                            {
                                BatchCARInsertStatusData data = new BatchCARInsertStatusData
                                {
                                    HeaderData = head,
                                    ReferenceNo = tmp[1],
                                    ChannelID = tmp[2],
                                    StatusDateTime = tmp[3],
                                    SubscriptionID = tmp[4],
                                    SubscriptionCardType = tmp[5],
                                    SubscriptionCusType = tmp[6],
                                    OwnerSystemId = tmp[7],
                                    OwnerSystemCode = tmp[8],
                                    RefSystemId = tmp[9],
                                    RefSystemCode = tmp[10],
                                    Status = tmp[11],
                                    StatusName = tmp[12]
                                };

                                ret.Add(data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateFileStatus(cbsFileId, "FAIL", "Exception : " + ex.Message);
            }

            return ret;
        }


        private bool MoveBatchFileToArchive(string filePath) {
            bool ret = false;
            try {
                FileInfo fInfo = new FileInfo(filePath);
                File.Copy(filePath, AppConstant.SLM_PRO_08_PathArchives + "\\" + fInfo.Name, true);

                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                ret = true;
            }
            catch (Exception ex) {
                throw ex;
            }

            return ret;
        }
    }
}
