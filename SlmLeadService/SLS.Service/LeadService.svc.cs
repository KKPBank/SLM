using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using log4net;
using SLS.Service.Request;
using SLS.Service.Response;
using SLS.Service.Utilities;
using SLS.Biz;
using SLS.Resource;
using SLS.Resource.Data;

namespace SLS.Service
{
    ///<summary>
    /// <br>Class Name : LeadService</br>
    /// <br>Purpose    : To manage lead.</br>
    /// <br>Author     : TikumpornA.</br>
    /// 
    /// Version 6.2.2 (SLM) : 1.เพิ่มฟิลด์ kkslm_tr_cusinfo.slm_country_id และให้มีการ Validate กรณีประเภทลูกค้าเป็นชาวต่างชาติ จะต้องมีการระบุ CountryCode ใน XML
    /// Version 6.2.1 (OBT) : 1.เปลี่ยนการ validate tag campaign ให้ require campaign เฉพาะ operation InsertLead เท่านั้น, UpdateLead และ SearchLead ไม่ require tag campaign
    /// Version 6.2.0 (OBT) : 1.เพิ่มสถานะและสถานะย่อยที่ Subject ของ Email ในส่วนของการบันทึก Note
    /// Version 6.1.0 (OBT) : 1.แก้ไขการรับรหัสสาขา โดยรหัสสาขาที่รับมาจากระบบภายนอกจะเป็น BranchCodeNew โดยเมื่อนำมาใช้ในระบบ SLM ให้ mapping กลับมาเป็น BranchCode ตัวเดิมที่ใช้ในระบบ SLM
    /// Version 6.0.0 (OBT) : 1.แก้ไขในส่วนของข้อมูล Redbook (Brand, Model, Submodel) ให้ไปใช้ข้อมูลกลาง
    ///                     : 2.เพิ่มการยิง WebService เพื่อส่ง Note ไปบันทึกที่ระบบ CAR, ในกรณียิงไม่สำเร็จ Insert ข้อมูลลง kkslm_tr_cas_resend เพื่อรอการ resend
    ///                     : 3.ปรับแก้การ Validate Campaign ในส่วนของ Service UpdateLead ให้ Require ค่า CampaignId แต่ไม่ต้อง validate การมีอยู่ของ Campaign ใน DB หรือ วันหมดอายุ 
    /// Version 5.3.0 (SLM) : เพิ่ม tag statusDateSource, statusReSendFlag เพื่อใช้ในการเช็กว่า status ที่ถูกส่งมาจากระบบภายนอกอื่นๆ status ไหนใหม่กว่ากัน
    /// Version 5.2.0 (SLM) : เพิ่ม tag productGroupName, productName, campaignName ใน xml ที่ถูก return ใน method SearchLead
    /// Version 5.1.0 (SLM) : เพิ่มการเก็บ status, subStatus, subStatusDesc, system ของระบบภายนอก เช่น ADAMs ที่ยิงเข้ามาในระบบ SLM
    ///</summary>
    public class LeadService : ILeadService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LeadService));
        private string operationFlag = "";
        private int timeout = ServiceConstant.ConnectionTimeout;
        private string _DCM = "DCM";
        private string _dealerCode = "";
        private string _marketingCode = "";
        private string _reference = "";

        #region Lead
        /// <summary>
        /// <br>Method Name : InsertLead</br>
        /// <br>Purpose     : To insert lead.</br>
        /// </summary>
        /// <param name="request" type="InsertLeadRequest"></param>
        /// <returns type="InsertLeadResponse"></returns>
        public InsertLeadResponse InsertLead(InsertLeadRequest request)
        {
            string ticketId = string.Empty;
            string channelId = string.Empty;
            string username = string.Empty;
            string request_xml = string.Empty;
            string causeError = string.Empty;
            DateTime responseDate = new DateTime();
            DateTime operationDate = DateTime.Now;
            InsertLeadResponse response = new InsertLeadResponse();
            operationFlag = ApplicationResource.INS_OPERATION;

            try
            {
                log.Debug("===================== Start Log =====================");
                log.Debug("Call Insert Operation at " + operationDate.ToString());
                SetHeaderLog(request.RequestHeader, operationFlag);

                if (request == null)
                {
                    throw new ServiceException(ApplicationResource.INS_REQUIRED_FIELDS_CODE, ApplicationResource.INS_REQUIRED_FIELDS_DESC);
                }

                response.ResponseHeader = request.RequestHeader;

                if (!ValidateHeader(request.RequestHeader))
                {
                    throw new ServiceException(ApplicationResource.INS_INVALID_ID_PASSWORD_CODE, ApplicationResource.INS_INVALID_ID_PASSWORD_DESC);
                }

                if (!ValidateXml(request.RequestXml))
                {
                    throw new ServiceException(ApplicationResource.INS_REQUIRED_FIELDS_CODE, ApplicationResource.INS_REQUIRED_FIELDS_DESC);
                }
                
                XDocument doc = XDocument.Parse(request.RequestXml);
                if (request.RequestHeader.ChannelID == _DCM)
                {
                    var root = doc.Root;

                    //เก็บ DealerCode ไว้ใช้ใน CreateResponseXml
                    var dealerInfo = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "dealerinfo").Take(1).Select(p => new
                    {
                        DealerCode = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "dealercode") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "dealercode").Value.Trim() : null
                    }).FirstOrDefault();

                    if (dealerInfo != null)
                    {
                        _dealerCode = dealerInfo.DealerCode != null ? dealerInfo.DealerCode : "";
                    } 
                        
                    //เก็บ Reference ไว้ใช้ใน CreateResponseXml
                    var reference = root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "reference") != null ? root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "reference").Value.Trim() : null;
                    if (reference != null)
                    {
                        _reference = reference;
                    }                       
                }

                if (!AuthenticateHeader(request.RequestHeader, ApplicationResource.INS_OPERATION))
                {
                    throw new ServiceException(ApplicationResource.INS_INVALID_ID_PASSWORD_CODE, ApplicationResource.INS_INVALID_ID_PASSWORD_DESC);
                }

                ticketId = InsertLeadData(doc, request.RequestHeader.ChannelID, _dealerCode);

                //Send SMS
                //SlmRuleService service = new SlmRuleService();
                //service.executeRuleSmsAsync(ticketId);

                responseDate = DateTime.Now;
                response.ResponseXml = CreateResponseXml(ticketId, request.RequestHeader.ChannelID, ApplicationResource.INS_SUCCESS_CODE, ApplicationResource.INS_SUCCESS_DESC, responseDate);

                log.Debug("ResponseCode : " + ApplicationResource.INS_SUCCESS_CODE + ", ResponseDesc : " + ApplicationResource.INS_SUCCESS_DESC);
                log.Debug("Xml : " + request.RequestXml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, request.RequestHeader.ChannelID, request.RequestHeader.Username, request.RequestXml, response.ResponseXml
                    , ApplicationResource.INS_SUCCESS_CODE, ApplicationResource.INS_SUCCESS_DESC, responseDate, responseDate.ToString("HH:mm:ss"), ""); 
            }
            catch (ServiceException ex)
            {
                causeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                responseDate = DateTime.Now;
                if (request != null)
                {
                    request_xml = request.RequestXml;
                }
                if (request != null && request.RequestHeader != null) 
                { 
                    channelId = request.RequestHeader.ChannelID;
                    username = request.RequestHeader.Username;
                }

                response.ResponseXml = CreateResponseXml(ticketId, channelId, ex.ResponseCode, ex.ResponseDesc, responseDate);

                log.Debug("ResponseCode : " + ex.ResponseCode + ", ResponseDesc : " + ex.ResponseDesc);
                log.Debug("Cause : " + causeError);
                log.Debug("Xml : " + request_xml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, channelId, username, request_xml, response.ResponseXml
                    , ex.ResponseCode, ex.ResponseDesc, responseDate, responseDate.ToString("HH:mm:ss"), causeError); 
            }
            catch (Exception ex)
            {
                causeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                responseDate = DateTime.Now;
                if (request != null)
                {
                    request_xml = request.RequestXml;
                }
                if (request != null && request.RequestHeader != null)
                {
                    channelId = request.RequestHeader.ChannelID;
                    username = request.RequestHeader.Username;
                }

                response.ResponseXml = CreateResponseXml(ticketId, channelId, ApplicationResource.INS_OTHER_CONDITION_ERROR_CODE, ApplicationResource.INS_OTHER_CONDITION_ERROR_DESC, responseDate);

                log.Debug("ResponseCode : " + ApplicationResource.INS_OTHER_CONDITION_ERROR_CODE + ", ResponseDesc : " + ApplicationResource.INS_OTHER_CONDITION_ERROR_DESC);
                log.Debug("Cause : " + causeError);
                log.Debug("Xml : " + request_xml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, channelId, username, request_xml, response.ResponseXml
                    , ApplicationResource.INS_OTHER_CONDITION_ERROR_CODE, ApplicationResource.INS_OTHER_CONDITION_ERROR_DESC, responseDate, responseDate.ToString("HH:mm:ss"), causeError); 
            }

            return response;
        }

        /// <summary>
        /// <br>Method Name : UpdateLead</br>
        /// <br>Purpose     : To update lead.</br>
        /// </summary>
        /// <param name="request" type="UpdateLeadRequest"></param>
        /// <returns type="UpdateLeadResponse"></returns>
        public UpdateLeadResponse UpdateLead(UpdateLeadRequest request)
        {
            string ticketId = string.Empty;
            string channelId = string.Empty;
            string username = string.Empty;
            string request_xml = string.Empty;
            string causeError = string.Empty;
            DateTime responseDate = new DateTime();
            DateTime operationDate = DateTime.Now;
            UpdateLeadResponse response = new UpdateLeadResponse();
            operationFlag = ApplicationResource.UPD_OPERATION;

            try
            {
                log.Debug("===================== Start Log =====================");
                log.Debug("Call Update Operation at " + operationDate.ToString());
                SetHeaderLog(request.RequestHeader, operationFlag);

                if (request == null)
                {
                    throw new ServiceException(ApplicationResource.UPD_REQUIRED_FIELDS_CODE, ApplicationResource.UPD_REQUIRED_FIELDS_DESC);
                }

                response.ResponseHeader = request.RequestHeader;

                if (!ValidateXml(request.RequestXml))
                {
                    throw new ServiceException(ApplicationResource.UPD_REQUIRED_FIELDS_CODE, ApplicationResource.UPD_REQUIRED_FIELDS_DESC);
                }

                XDocument doc = XDocument.Parse(request.RequestXml);
                ticketId = ReadTicketId(doc);

                if (!ValidateHeader(request.RequestHeader))
                {
                    throw new ServiceException(ApplicationResource.UPD_INVALID_ID_PASSWORD_CODE, ApplicationResource.UPD_INVALID_ID_PASSWORD_DESC);
                }

                if (!AuthenticateHeader(request.RequestHeader, ApplicationResource.UPD_OPERATION))
                {
                    throw new ServiceException(ApplicationResource.UPD_INVALID_ID_PASSWORD_CODE, ApplicationResource.UPD_INVALID_ID_PASSWORD_DESC);
                }

                UpdateLeadData(doc, ticketId, request.RequestHeader.ChannelID.ToUpper());
                responseDate = DateTime.Now;
                response.ResponseXml = CreateResponseXml(ticketId, request.RequestHeader.ChannelID, ApplicationResource.UPD_SUCCESS_CODE, ApplicationResource.UPD_SUCCESS_DESC, responseDate);

                log.Debug("ResponseCode : " + ApplicationResource.UPD_SUCCESS_CODE + ", ResponseDesc : " + ApplicationResource.UPD_SUCCESS_DESC);
                log.Debug("Xml : " + request.RequestXml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, request.RequestHeader.ChannelID, request.RequestHeader.Username, request.RequestXml, response.ResponseXml
                    , ApplicationResource.UPD_SUCCESS_CODE, ApplicationResource.UPD_SUCCESS_DESC, responseDate, responseDate.ToString("HH:mm:ss"), ""); 
            }
            catch (ServiceException ex)
            {
                causeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                responseDate = DateTime.Now;
                if (request != null)
                {
                    request_xml = request.RequestXml;
                }
                if (request != null && request.RequestHeader != null)
                {
                    channelId = request.RequestHeader.ChannelID;
                    username = request.RequestHeader.Username;
                }

                response.ResponseXml = CreateResponseXml(ticketId, channelId, ex.ResponseCode, ex.ResponseDesc, responseDate);
                
                log.Debug("ResponseCode : " + ex.ResponseCode + ", ResponseDesc : " + ex.ResponseDesc);
                log.Debug("Cause : " + causeError);
                log.Debug("Xml : " + request_xml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, channelId, username, request_xml, response.ResponseXml
                    , ex.ResponseCode, ex.ResponseDesc, responseDate, responseDate.ToString("HH:mm:ss"), causeError); 
            }
            catch (Exception ex)
            {
                causeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                responseDate = DateTime.Now;
                if (request != null)
                {
                    request_xml = request.RequestXml;
                }
                if (request != null && request.RequestHeader != null)
                {
                    channelId = request.RequestHeader.ChannelID;
                    username = request.RequestHeader.Username;
                }

                response.ResponseXml = CreateResponseXml(ticketId, channelId, ApplicationResource.UPD_OTHER_CONDITION_ERROR_CODE, ApplicationResource.UPD_OTHER_CONDITION_ERROR_DESC, responseDate);

                log.Debug("ResponseCode : " + ApplicationResource.UPD_OTHER_CONDITION_ERROR_CODE + ", ResponseDesc : " + ApplicationResource.UPD_OTHER_CONDITION_ERROR_DESC);
                log.Debug("Cause : " + causeError);
                log.Debug("Xml : " + request_xml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, channelId, username, request_xml, response.ResponseXml
                    , ApplicationResource.UPD_OTHER_CONDITION_ERROR_CODE, ApplicationResource.UPD_OTHER_CONDITION_ERROR_DESC, responseDate, responseDate.ToString("HH:mm:ss"), causeError); 
            }

            return response;
        }

        private void SetHeaderLog(Header header, string operation)
        {
            if (header != null)
            {
                log.Debug("Operation: " + operation);
                log.Debug("ChannelID: " + (!string.IsNullOrEmpty(header.ChannelID) ? header.ChannelID : string.Empty));
                log.Debug("Username: " + (!string.IsNullOrEmpty(header.Username)  ? header.Username : string.Empty));
            }
            else
            {
                log.Debug("Operation: " + operation);
                log.Debug("ChannelID: Null");
                log.Debug("Username: Null");
            }
        }

        /// <summary>
        /// <br>Method Name : ValidateHeader</br>
        /// <br>Purpose     : To validate header.</br>
        /// </summary>
        /// <param name="header" type="Header"></param>
        /// <returns type="bool"></returns>
        private bool ValidateHeader(Header header)
        {
            if (header != null)
            {
                return !string.IsNullOrEmpty(header.Username) && !string.IsNullOrEmpty(header.Password) && !string.IsNullOrEmpty(header.ChannelID);
            }
            else
            {
                return false;
            } 
        }

        /// <summary>
        /// <br>Method Name : ValidateXml</br>
        /// <br>Purpose     : To validate xml string.</br>
        /// </summary>
        /// <param name="xml" type="string"></param>
        /// <returns type="bool"></returns>
        private bool ValidateXml(string xml)
        {
            return !string.IsNullOrEmpty(xml) && !string.IsNullOrWhiteSpace(xml);
        }

        /// <summary>
        /// <br>Method Name : AuthenticateHeader</br>
        /// <br>Purpose     : To authenticate header.</br>
        /// </summary>
        /// <param name="header" type="Header"></param>
        /// <returns type="bool"></returns>
        private bool AuthenticateHeader(Header header, string operationFlag)
        {
            return LeadServiceBiz.AuthenticateHeader(header.Username, header.Password, header.ChannelID, operationFlag, timeout);
        }

        /// <summary>
        /// <br>Method Name : InsertLeadData</br>
        /// <br>Purpose     : To read xml string and insert data to database.</br>
        /// </summary>
        /// <param name="doc" type="XDocument"></param>
        /// <returns type="string">TicketId</returns>
        private string InsertLeadData(XDocument doc, string channelId, string dealerCode)
        {
            try
            {
                var root = doc.Root;
                if (root == null || root.Name.LocalName.ToLower().Trim() != "ticket")
                {
                    throw RequireField("ticket");
                }
                    
                AppInfo appInfo = ReadAppInfo(root);
                string systemName = string.IsNullOrEmpty(appInfo.System) ? ReadSystem(root) : appInfo.System;

                Mandatory mandatory = ReadMandatory(root, systemName);
                CustomerDetail cusDetail = ReadCustomerDetail(root);
                DealerInfo dealerinfo = ReadDealerInfo(root);
                string reference = ReadReference(root);

                //Check use of AOL service if channelId = DCM ======================================================
                if (channelId == _DCM)
                {
                    if (LeadServiceBiz.CheckUseOfAolService(channelId, mandatory.Campaign, timeout, ApplicationResource.INS_OPERATION))
                    {
                        if (string.IsNullOrEmpty(dealerCode))
                        {
                            throw new ServiceException(ApplicationResource.INS_INVALID_DEALERCODE_CODE, ApplicationResource.INS_INVALID_DEALERCODE_DESC);
                        }
                        cusDetail.TelesaleName = GetMarketingCode(dealerCode);
                    }
                }
                //===================================================================================================

                //Check Access Right
                if (!string.IsNullOrEmpty(mandatory.Campaign) && !string.IsNullOrEmpty(cusDetail.TelesaleName))
                {
                    if (!AccessRightBiz.CheckAccessRight(mandatory.Campaign, cusDetail.TelesaleName))
                    {
                        throw InvalidParameter("telesaleName");
                    }     
                }

                return LeadServiceBiz.InsertLeadData(mandatory, ReadCustomerInfo(root, systemName), cusDetail, ReadProductInfo(root), ReadChannelInfo(root), dealerinfo, reference, appInfo, channelId, timeout);
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <br>Method Name : UpdateLeadData</br>
        /// <br>Purpose     : To read xml string and update data to database.</br>
        /// </summary>
        /// <param name="doc" type="XDocument"></param>
        /// <param name="ticketId" type="string"></param>
        /// <returns type="string">TicketId</returns>
        private void UpdateLeadData(XDocument doc, string ticketId, string channelId)
        {
            try
            {
                var root = doc.Root;
                List<string> sendEmailPerson = new List<string>();
                string sendEmailFlag = ReadSendEmailFlag(root);

                if (sendEmailFlag != null && sendEmailFlag.Trim().ToUpper() == "Y")
                {
                    sendEmailPerson = ReadSendEmailPerson(root);
                }
                    
                AppInfo appInfo = ReadAppInfo(root);
                DealerInfo dealerinfo = ReadDealerInfo(root);

                string noteId = "";
                CustomerDetail cusDetail = null;
                ChannelInfo channelInfo = null;
                if (string.IsNullOrEmpty(appInfo.System))
                {
                    string systemName = ReadSystem(root);
                    cusDetail = ReadCustomerDetail(root);
                    channelInfo = ReadChannelInfo(root);

                    string calculateTotalSlaError = "";
                    noteId = LeadServiceBiz.UpdateLeadData(ticketId, ReadMandatory(root, systemName), ReadCustomerInfo(root, systemName), cusDetail, ReadProductInfo(root), channelInfo, channelId, sendEmailFlag, sendEmailPerson, timeout, systemName, out calculateTotalSlaError);
                    if (!string.IsNullOrEmpty(calculateTotalSlaError))
                    {
                        log.Error(calculateTotalSlaError);
                    }
                }
                else
                {
                    cusDetail = ReadCustomerDetail(root);
                    channelInfo = ReadChannelInfo(root);

                    if (appInfo.System == ServiceConstant.System.HPGABLE)
                    {
                        noteId = LeadServiceBiz.UpdateLeadByHpaofl(ticketId, ReadMandatoryForHpGable(root, appInfo.System), ReadCustomerInfo(root, appInfo.System), cusDetail, ReadProductInfo(root), channelInfo, channelId, sendEmailFlag, sendEmailPerson, appInfo, dealerinfo, timeout);
                    }
                    else
                    {
                        noteId = LeadServiceBiz.UpdateLeadByHpaofl(ticketId, ReadMandatory(root, appInfo.System), ReadCustomerInfo(root, appInfo.System), cusDetail, ReadProductInfo(root), channelInfo, channelId, sendEmailFlag, sendEmailPerson, appInfo, dealerinfo, timeout);
                    }  
                }

                //Call CAR WebService
                if (!string.IsNullOrEmpty(noteId))
                {
                    CreateCASActivityLog(ticketId, noteId, cusDetail, channelInfo, sendEmailFlag, sendEmailPerson);
                }
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method Name : CreateCASActivityLog
        /// Purpose     : To send note to CAR System via web service
        /// </summary>
        /// <param name="noteId"></param>
        private void CreateCASActivityLog(string ticketId, string noteId, CustomerDetail cusDetail, ChannelInfo channelInfo, string sendEmailFlag, List<string> sendEmailPerson)
        {
            try
            {
                string noteCreateBy = "";
                string sendEmail = "";
                string emailTo = "";
                string subject = "";
                string preleadId = "";

                if (cusDetail != null && !string.IsNullOrEmpty(cusDetail.Note))
                {
                    //ผู้สร้าง note
                    if (channelInfo != null)
                    {
                        noteCreateBy = string.IsNullOrEmpty(channelInfo.CreateUser) ? "SYSTEM" : channelInfo.CreateUser;
                    }
                    else
                    {
                        noteCreateBy = "SYSTEM";
                    }
                        
                    LeadDataForCARLogService data = LeadServiceBiz.GetDataForCARLogService(ticketId);

                    //ส่ง email หรือไม่
                    if (sendEmailFlag != null && sendEmailFlag.Trim().ToUpper() == "Y")
                    {
                        sendEmail = "Y";
                        subject = "SLM: Ticket: " + ticketId + (string.IsNullOrEmpty(cusDetail.Subject) ? "" : " " + cusDetail.Subject);

                        //Get email owner delegate
                        if (data != null)
                        {
                            subject = "[" + data.StatusName + "]" + (string.IsNullOrEmpty(data.SubStatusName) ? "" : "-[" + data.SubStatusName + "]") + " " + subject;

                            if (!string.IsNullOrEmpty(data.Owner) && !string.IsNullOrEmpty(data.OwnerEmail))
                            {
                                emailTo += (emailTo != "" ? ", " : "") + data.OwnerEmail;
                            }
                                
                            if (!string.IsNullOrEmpty(data.Delegate) && !string.IsNullOrEmpty(data.DelegateEmail))
                            {
                                emailTo += (emailTo != "" ? ", " : "") + data.DelegateEmail;
                            }
                                
                            if (!string.IsNullOrEmpty(data.MarketingOwner) && !string.IsNullOrEmpty(data.MarketingOwnerEmail))
                            {
                                emailTo += (emailTo != "" ? ", " : "") + data.MarketingOwnerEmail;
                            }
                                
                            if (!string.IsNullOrEmpty(data.LastOwner) && !string.IsNullOrEmpty(data.LastOwnerEmail))
                            {
                                string flag = System.Configuration.ConfigurationManager.AppSettings["SendEmailLastOwner"];
                                if (flag == null || flag.Trim().ToUpper() == "Y")
                                {
                                    emailTo += (emailTo != "" ? ", " : "") + data.LastOwnerEmail;
                                }
                            }
                        }

                        foreach (string email in sendEmailPerson)
                        {
                            emailTo += (emailTo != "" ? ", " : "") + email;
                        }
                    }
                    else
                    {
                        sendEmail = "N";
                    }

                    if (noteCreateBy != "SYSTEM")
                    {
                        string strName = LeadServiceBiz.GetStaffName(noteCreateBy);
                        if (!string.IsNullOrEmpty(strName))
                        {
                            noteCreateBy = strName;
                        }
                    }

                    preleadId = data != null ? (data.PreleadId != null ? data.PreleadId.Value.ToString() : "") : "";
                    
                    //Activity Info
                    List<Services.CARService.DataItem> actInfoList = new List<Services.CARService.DataItem>();
                    actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "ผู้สร้าง Note", DataValue = noteCreateBy });
                    actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ส่งอีเมล", DataValue = sendEmail });
                    actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "To", DataValue = emailTo });
                    actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 4, DataLabel = "Subject", DataValue = subject });
                    actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 5, DataLabel = "บันทึก Note", DataValue = cusDetail.Note });

                    //Customer Info
                    List<Services.CARService.DataItem> cusInfoList = new List<Services.CARService.DataItem>();
                    cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = (data != null ? data.CitizenId : "") });
                    cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = (data != null ? data.CardTypeName : "") });
                    cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = data != null ? data.CustomerName : "" });

                    //Product Info
                    List<Services.CARService.DataItem> prodInfoList = new List<Services.CARService.DataItem>();
                    prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = (data != null ? data.ProductGroupName : "") });
                    prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = (data != null ? data.ProductName : "") });
                    prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = (data != null ? data.CampaignName : "") });

                    //Contract Info
                    List<Services.CARService.DataItem> contInfoList = new List<Services.CARService.DataItem>();
                    contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = (data != null ? data.ContractNo : "") });
                    contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = preleadId != "" ? "HP" : "" });
                    contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = (data != null ? data.LicenseNo : "") });

                    //Officer Info
                    List<Services.CARService.DataItem> offInfoList = new List<Services.CARService.DataItem>();
                    offInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = noteCreateBy });

                    Services.CARService.CARServiceData logdata = new Services.CARService.CARServiceData()
                    {
                        ReferenceNo = noteId,
                        SecurityKey = preleadId != "" ? ApplicationConstant.CARLogService.OBTSecurityKey : ApplicationConstant.CARLogService.SLMSecurityKey,
                        ServiceName = "CreateActivityLog",
                        SystemCode = preleadId != "" ? ApplicationConstant.CARLogService.CARLoginOBT : ApplicationConstant.CARLogService.CARLoginSLM,      //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
                        TransactionDateTime = DateTime.Now,
                        ActivityInfoList = actInfoList,
                        CustomerInfoList = cusInfoList,
                        ProductInfoList = prodInfoList,
                        ContractInfoList = contInfoList,
                        OfficerInfoList = offInfoList,
                        ActivityDateTime = DateTime.Now,
                        CampaignId = data != null ? data.CampaignId : "",
                        ChannelId = data != null ? data.ChannelId : "",
                        PreleadId = preleadId,
                        ProductGroupId = data != null ? data.ProductGroupId : "",
                        ProductId = data != null ? data.ProductId : "",
                        Status = data != null ? data.StatusName : "",
                        SubStatus = data != null ? data.SubStatusName : "",
                        TicketId = ticketId,
                        SubscriptionId = data != null ? data.CitizenId : "",
                        TypeId = ApplicationConstant.CARLogService.Data.TypeId,
                        AreaId = ApplicationConstant.CARLogService.Data.AreaId,
                        SubAreaId = ApplicationConstant.CARLogService.Data.SubAreaId,
                        ActivityTypeId = ApplicationConstant.CARLogService.Data.ActivityType.FYIId,
                        ContractNo = data != null ? data.ContractNo : ""
                    };

                    if (data != null && data.SubScriptionTypeId != null)
                    {
                        logdata.SubscriptionTypeId = data.SubScriptionTypeId.Value;
                    }
                        
                    bool ret = Services.CARService.CreateActivityLog(logdata, ticketId);
                    LeadServiceBiz.UpdateCasFlag(ticketId, noteId, ret ? "1" : "2");
                }
            }
            catch (Exception ex)
            {
                LeadServiceBiz.UpdateCasFlag(ticketId, noteId, "2");

                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                log.Debug(message);
            }
        }

        /// <summary>
        /// <br>Method Name : ReadTicketId</br>
        /// <br>Purpose     : To read ticketId from xml.</br>
        /// </summary>
        /// <param name="doc"></param>
        /// <returns type="string"></returns>
        private string ReadTicketId(XDocument doc)
        {
            var root = doc.Root;
            if (root == null || root.Name.LocalName.ToLower().Trim() != "ticket")
            {
                if (operationFlag == ApplicationResource.UPD_OPERATION)
                {
                    throw new ServiceException(ApplicationResource.UPD_REQUIRED_FIELDS_CODE, ApplicationResource.UPD_REQUIRED_FIELDS_DESC);
                }
                else
                {
                    throw new ServiceException(ApplicationResource.SEARCH_REQUIRED_FIELDS_CODE, ApplicationResource.SEARCH_REQUIRED_FIELDS_DESC);
                }
            }

            var ticket_attribute = root.Attributes().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "id");
            if (ticket_attribute == null)
            {
                if (operationFlag == ApplicationResource.UPD_OPERATION)
                {
                    throw new ServiceException(ApplicationResource.UPD_REQUIRED_FIELDS_CODE, ApplicationResource.UPD_REQUIRED_FIELDS_DESC);
                }
                else
                {
                    throw new ServiceException(ApplicationResource.SEARCH_REQUIRED_FIELDS_CODE, ApplicationResource.SEARCH_REQUIRED_FIELDS_DESC);
                }  
            }

            string ticketId = ticket_attribute.Value.Trim();

            if (ticketId != string.Empty)
            {
                return ticketId;
            }
            else
            {
                if (operationFlag == ApplicationResource.UPD_OPERATION)
                {
                    throw new ServiceException(ApplicationResource.UPD_REQUIRED_FIELDS_CODE, ApplicationResource.UPD_REQUIRED_FIELDS_DESC);
                }
                else if (operationFlag == ApplicationResource.VAL_PER_OPERATION)
                {
                    throw new ServiceException(ApplicationResource.VAL_PER_REQUIRED_FIELDS_CODE, ApplicationResource.VAL_PER_REQUIRED_FIELDS_DESC);
                }
                else
                {
                    throw new ServiceException(ApplicationResource.SEARCH_REQUIRED_FIELDS_CODE, ApplicationResource.SEARCH_REQUIRED_FIELDS_DESC);
                }
            }
        }

        /// <summary>
        /// <br>Method Name : ReadMandatory</br>
        /// <br>Purpose     : To read collection of the descendant mandatory element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="Mandatory"></returns>
        private Mandatory ReadMandatory(XElement root, string systemName)
        {
            //Check mandatory tag
            var mantElement = root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "mandatory");
            if (mantElement == null)
            {
                throw RequireField("mandatory");
            }

            var firstname = mantElement.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "firstname");
            var telNo1 = mantElement.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "telno1");
            var campaign = mantElement.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "campaign");

            //if (firstname == null || telNo1 == null || campaign == null) EDIT 20170224
            //if (firstname == null || telNo1 == null) EDIT 20171207
            if (firstname == null)
            {
                throw RequireField("firstname");
            }

            //Check mandatory value
            var firstnameValue = firstname.Value.Trim();
            var telNo1Value = telNo1 != null ? telNo1.Value.Trim() : string.Empty;
            var campaignValue = campaign != null ? campaign.Value.Trim() : string.Empty;

            //if (firstnameValue == string.Empty || telNo1Value == string.Empty || campaignValue == string.Empty)   EDIT 20170224
            //if (firstnameValue == string.Empty || telNo1Value == string.Empty)  EDIT 20171207
            if (firstnameValue == string.Empty)
            {
                throw RequireField("firstname");
            }

            //Validate TelNo1
            if (telNo1Value != string.Empty)
            {
                if (!ValidateTelNo1(ReadCardType(root), telNo1Value, systemName))
                {
                    throw InvalidParameter("telNo1");
                }
            }

            //Validate Campaign เฉพาะ Operation Insert เท่านั้น
            if (operationFlag == ApplicationResource.INS_OPERATION)
            {
                if (telNo1Value == string.Empty || telNo1 == null) // ย้ายการ Validate telNo1 โดยจะ Validate เฉพาะ Function Insert เท่านั้น
                {
                    throw RequireField("telNo1");
                }
                if (campaignValue == string.Empty || campaign == null) // ย้ายการ Validate Campaign โดยจะ Validate เฉพาะ Function Insert เท่านั้น
                {
                    throw RequireField("campaign");
                }
                var campaign_data = LeadServiceBiz.GetGampaign(campaignValue);
                if (campaign_data == null)
                {
                    throw new ServiceException(ApplicationResource.INS_CAMPAIGN_NOTFOUND_CODE, ApplicationResource.INS_CAMPAIGN_NOTFOUND_DESC);
                }
                else
                {
                    if (campaign_data.Status.ToUpper().Trim() == "X")
                    {
                        throw new ServiceException(ApplicationResource.INS_CAMPAIGN_EXPIRED_CODE, ApplicationResource.INS_CAMPAIGN_EXPIRED_DESC);
                    }
                    if (campaign_data.Status.ToUpper().Trim() != "A")
                    {
                        throw new ServiceException(ApplicationResource.INS_CAMPAIGN_INVALID_CODE, ApplicationResource.INS_CAMPAIGN_INVALID_DESC);
                    }
                    if (campaign_data.CampaignDesc.Trim().Length > ApplicationConstant.TextMaxLength)
                    {
                        throw new ServiceException(ApplicationResource.INS_CAMPAIGNDETAIL_EXCEED_MAX_LENGTH_CODE, ApplicationResource.INS_CAMPAIGNDETAIL_EXCEED_MAX_LENGTH_DESC);
                    }
                }
            }

            Mandatory mann = new Mandatory
            {
                Firstname = firstnameValue,
                Campaign = campaignValue,
                TelNo1 = telNo1Value
            };
            return mann;
        }

        private bool ValidateTelNo1(string cardType, string telNo1Value, string systemName)
        {
            decimal result;
            if (!decimal.TryParse(telNo1Value, out result))
            {
                return false;
            }

            if (cardType == ServiceConstant.CardType.JuristicPerson || string.IsNullOrEmpty(cardType))
            {
                if (systemName == ServiceConstant.System.HPAOFL || systemName == ServiceConstant.System.HPGABLE)
                {
                    if (telNo1Value.Length < 9 || telNo1Value.Length > 10 || !telNo1Value.StartsWith("0"))
                    {
                        return false;
                    }
                }
                else
                {
                    if (result == 0 || telNo1Value.Length < 9 || telNo1Value.Length > 10 || !telNo1Value.StartsWith("0"))
                    {
                        return false;
                    }   
                }
            }
            else
            {
                if (systemName == ServiceConstant.System.HPAOFL || systemName == ServiceConstant.System.HPGABLE)
                {
                    if (telNo1Value.Length != 10 || !telNo1Value.StartsWith("0"))
                    {
                        return false;
                    }
                }
                else
                {
                    if (result == 0 || telNo1Value.Length != 10 || !telNo1Value.StartsWith("0"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private string ReadCardType(XElement root)
        {
            var customerInfo = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "customerinfo").Take(1).Select(p => new
            {
                CardType = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cardtype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cardtype").Value.Trim() : null
            }).FirstOrDefault();

            if (customerInfo != null)
            {
                string[] arrCardType = { ServiceConstant.CardType.Person, ServiceConstant.CardType.JuristicPerson, ServiceConstant.CardType.Foreigner };

                if (!string.IsNullOrEmpty(customerInfo.CardType) && !arrCardType.Contains(customerInfo.CardType))
                {
                    throw InvalidParameter("cardType");
                }                    

                return string.IsNullOrEmpty(customerInfo.CardType) ? "" : customerInfo.CardType;
            }
            else
            {
                return "";
            }      
        }

        //Method ชั่วคราวสำหรับ HPGABLE จะไม่ validate campaign
        private Mandatory ReadMandatoryForHpGable(XElement root, string systemName)
        {
            //Check mandatory tag
            var mantElement = root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "mandatory");
            if (mantElement == null)
            {
                throw RequireField("mandatory");
            }
                
            var firstname = mantElement.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "firstname");
            var telNo1 = mantElement.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "telno1");

            if (firstname == null || telNo1 == null)
            {
                throw RequireField("firstname, telNo1");
            }
                
            //Check mandatory value
            var firstnameValue = firstname.Value.Trim();
            var telNo1Value = telNo1.Value.Trim();

            if (firstnameValue == string.Empty || telNo1Value == string.Empty)
            {
                throw RequireField("firstname, telNo1");
            }               

            //Validate TelNo1
            if (!ValidateTelNo1(ReadCardType(root), telNo1Value, systemName))
            {
                throw InvalidParameter("telNo1");
            }
                
            Mandatory mann = new Mandatory
            {
                Firstname = firstnameValue,
                TelNo1 = telNo1Value
            };
            return mann;
        }

        private ServiceException RequireField(string tagName)
        {
            if (operationFlag == ApplicationResource.INS_OPERATION)
            {
                return new ServiceException(ApplicationResource.INS_REQUIRED_FIELDS_CODE, ApplicationResource.INS_REQUIRED_FIELDS_DESC, "<" + tagName + "> is missing or has blank value", null);
            }  
            else if (operationFlag == ApplicationResource.VAL_PER_OPERATION)
            {
                return new ServiceException(ApplicationResource.VAL_PER_REQUIRED_FIELDS_CODE, ApplicationResource.VAL_PER_REQUIRED_FIELDS_DESC, "<" + tagName + "> is missing or has blank value", null);
            }
            else
            {
                return new ServiceException(ApplicationResource.UPD_REQUIRED_FIELDS_CODE, ApplicationResource.UPD_REQUIRED_FIELDS_DESC, "<" + tagName + "> is missing or has blank value", null);
            }
        }

        private ServiceException InvalidParameter(string tagName)
        {
            if (operationFlag == ApplicationResource.INS_OPERATION)
            {
                return new ServiceException(ApplicationResource.INS_INVALID_PARAMETERS_CODE, ApplicationResource.INS_INVALID_PARAMETERS_DESC, "<" + tagName + "> has invalid value", null);
            } 
            else
            {
                return new ServiceException(ApplicationResource.UPD_INVALID_PARAMETERS_CODE, ApplicationResource.UPD_INVALID_PARAMETERS_DESC, "<" + tagName + "> has invalid value", null);
            }
        }

        private ServiceException RequireCardNumber()
        {
            if (operationFlag == ApplicationResource.INS_OPERATION)
            {
                return new ServiceException(ApplicationResource.INS_REQUIRED_CARDID_CODE, ApplicationResource.INS_REQUIRED_CARDID_DESC);
            }
            else
            {
                return new ServiceException(ApplicationResource.UPD_REQUIRED_CARDID_CODE, ApplicationResource.UPD_REQUIRED_CARDID_DESC);
            }
        }

        /// <summary>
        /// <br>Method Name : ReadCustomerInfo</br>
        /// <br>Purpose     : To read collection of the descendant customerInfo element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="CustomerInfo"></returns>
        private CustomerInfo ReadCustomerInfo(XElement root, string systemName)
        {
            var customerInfo = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "customerinfo").Take(1).Select(p => new
            {
                Lastname = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "lastname") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "lastname").Value.Trim() : null,
                Email = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "email") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "email").Value.Trim() : null,
                TelNo2 = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "telno2") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "telno2").Value.Trim() : null,
                TelNo3 = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "telno3") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "telno3").Value.Trim() : null,
                ExtNo1 = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "extno1") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "extno1").Value.Trim() : null,
                ExtNo2 = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "extno2") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "extno2").Value.Trim() : null,
                ExtNo3 = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "extno3") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "extno3").Value.Trim() : null,
                BuildingName = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "buildingname") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "buildingname").Value.Trim() : null,
                AddrNo = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "addrno") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "addrno").Value.Trim() : null,
                Floor = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "floor") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "floor").Value.Trim() : null,
                Soi = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "soi") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "soi").Value.Trim() : null,
                Street = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "street") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "street").Value.Trim() : null,
                Tambom = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "tambol") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "tambol").Value.Trim() : null,
                Amphur = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "amphur") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "amphur").Value.Trim() : null,
                Province = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "province") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "province").Value.Trim() : null,
                PostalCode = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "postalcode") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "postalcode").Value.Trim() : null,
                Occupation = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "occupation") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "occupation").Value.Trim() : null,
                BaseSalary = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "basesalary") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "basesalary").Value.Trim() : null,
                IsCustomer = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "iscustomer") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "iscustomer").Value.Trim() : null,
                CustomerCode = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "customercode") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "customercode").Value.Trim() : null,
                DateOfBirth = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "dateofbirth") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "dateofbirth").Value.Trim() : null,
                Cid = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cid") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cid").Value.Trim() : null,
                Status = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "status") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "status").Value.Trim() : null,
                SubStatus = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "substatus") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "substatus").Value.Trim() : null,
                SubStatusDesc = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "substatusdesc") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "substatusdesc").Value.Trim() : null,
                StatusDateSource = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "statusdatesource") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "statusdatesource").Value.Trim() : null,
                StatusReSendFlag = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "statusresendflag") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "statusresendflag").Value.Trim() : null,
                ContractNoRefer = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "contractnorefer") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "contractnorefer").Value.Trim() : null,
                CardType = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cardtype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cardtype").Value.Trim() : null,
                CountryCode = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "country") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "country").Value.Trim() : null
            }).FirstOrDefault();

            if (customerInfo != null)
            {
                //Validate Email
                if (!string.IsNullOrEmpty(customerInfo.Email) && !ValidateEmail(customerInfo.Email))
                {
                    throw InvalidParameter("Email");
                }

                //Validate TelNo2
                decimal result;
                string telNo2 = customerInfo.TelNo2 != null ? customerInfo.TelNo2 : "";
                if (!string.IsNullOrEmpty(telNo2))
                {
                    if (!decimal.TryParse(telNo2, out result))
                    {
                        throw InvalidParameter("telNo2");
                    }

                    if (systemName == ServiceConstant.System.HPAOFL || systemName == ServiceConstant.System.HPGABLE)
                    {
                        if ((telNo2.Length < 9 || telNo2.Length > 10) || !telNo2.StartsWith("0"))
                        {
                            throw InvalidParameter("telNo2");
                        }
                    }
                    else
                    {
                        if (result == 0 || (telNo2.Length < 9 || telNo2.Length > 10) || !telNo2.StartsWith("0"))
                        {
                            throw InvalidParameter("telNo2");
                        }  
                    }
                }

                //Validate TelNo3
                string telNo3 = customerInfo.TelNo3 != null ? customerInfo.TelNo3 : "";
                if (!string.IsNullOrEmpty(telNo3))
                {
                    if (!decimal.TryParse(telNo3, out result))
                    {
                        throw InvalidParameter("telNo3");
                    }

                    if (systemName == ServiceConstant.System.HPAOFL || systemName == ServiceConstant.System.HPGABLE)
                    {
                        if ((telNo3.Length < 9 || telNo3.Length > 10) || !telNo3.StartsWith("0"))
                        {
                            throw InvalidParameter("telNo3");
                        }
                    }
                    else
                    {
                        if (result == 0 || (telNo3.Length < 9 || telNo3.Length > 10) || !telNo3.StartsWith("0"))
                        {
                            throw InvalidParameter("telNo3");
                        }   
                    }
                }

                //Validate DateOfBirth
                if (!string.IsNullOrEmpty(customerInfo.DateOfBirth))
                {
                    ApplicationUtil.ValidateDate(customerInfo.DateOfBirth, operationFlag, "dateOfBirth");
                }
                    
                //Validate Cid
                string cardType;
                string[] arrCardType = { ServiceConstant.CardType.Person, ServiceConstant.CardType.JuristicPerson, ServiceConstant.CardType.Foreigner };
                
                if (!string.IsNullOrEmpty(customerInfo.CardType) && !arrCardType.Contains(customerInfo.CardType))
                {
                    throw InvalidParameter("cardType");
                }  

                if (!string.IsNullOrEmpty(customerInfo.CardType) && string.IsNullOrEmpty(customerInfo.Cid))
                {
                    throw RequireCardNumber();
                }
                    
                if (string.IsNullOrEmpty(customerInfo.CardType) && !string.IsNullOrEmpty(customerInfo.Cid))
                {
                    cardType = "";
                }
                else
                {
                    cardType = customerInfo.CardType;
                }    

                if (!string.IsNullOrEmpty(cardType) && !string.IsNullOrEmpty(customerInfo.Cid))
                {
                    if (cardType == ServiceConstant.CardType.Person && !VerifyCitizenId(customerInfo.Cid))
                    {
                        throw InvalidParameter("cid");
                    }
                    else if (cardType == ServiceConstant.CardType.JuristicPerson && !VerifyJuristicPersonId(customerInfo.Cid))
                    {
                        throw InvalidParameter("cid");
                    }
                    else if (cardType == ServiceConstant.CardType.Foreigner && customerInfo.Cid.Trim().Length > 50)
                    {
                        throw InvalidParameter("cid");
                    }
                }
                else if (string.IsNullOrEmpty(cardType) && !string.IsNullOrEmpty(customerInfo.Cid))
                {
                    if (customerInfo.Cid.Trim().Length > 50)
                    {
                        throw InvalidParameter("cid");
                    }
                }

                //Kug 2018-01-05 เพิ่ม Validate Country กรณีประเภทลูกค้าเป็นชาวต่างชาติ
                if (cardType == ServiceConstant.CardType.Foreigner && string.IsNullOrEmpty(customerInfo.CountryCode))
                {
                    throw RequireField("country");
                }
                else if (cardType == ServiceConstant.CardType.Foreigner && !string.IsNullOrEmpty(customerInfo.CountryCode))
                {
                    if (!LeadServiceBiz.CheckCountryCodeExist(customerInfo.CountryCode))
                        throw InvalidParameter("country");
                }

                //Validate Status, SubStatus, SubStatusDesc มีเฉพาะขา Update เท่านั้น, systemName ต้องไม่ใช่ช่องว่าง และไม่ใช่ HPAOFL เพราะ HPAOFL จะส่งเข้ามาทาง tag cocStatus, cocSubStatus
                //Tag systemName ปัจจุบันมีระบบ HPAOFL, HPGABLE, ADAMS, FMS
                DateTime statusDateSource = new DateTime();
                if (operationFlag == ApplicationResource.UPD_OPERATION && !string.IsNullOrEmpty(systemName) && systemName != ServiceConstant.System.HPAOFL && systemName != ServiceConstant.System.HPGABLE)
                {
                    //เช็ก null อย่างเดียวไม่ต้องเช็กข้อมูลเป็นค่าว่าง เพราะ require tag, ส่วนข้อมูลจะนำไปเช็กในตาราง kkslm_ms_mapping_status
                    if (customerInfo.Status == null)
                    {
                        throw RequireField("status");
                    }
                    if (customerInfo.SubStatus == null)
                    {
                        throw RequireField("subStatus");
                    }
                    if (customerInfo.SubStatusDesc == null)
                    {
                        throw RequireField("subStatusDesc");
                    }

                    //Check StatusDateSource, Added by Pom 15/03/2016
                    if (!string.IsNullOrEmpty(customerInfo.StatusReSendFlag) && customerInfo.StatusReSendFlag.Trim().ToUpper() == "Y" && string.IsNullOrEmpty(customerInfo.StatusDateSource))
                    {
                        //ถ้ามี Flag การซ่อม = Y ต้องมี statusDateSource ด้วย
                        throw RequireField("statusDateSource");
                    }

                    string[] tmpStatusDateSource = null;
                    if (!string.IsNullOrEmpty(customerInfo.StatusDateSource))
                    {
                        tmpStatusDateSource = customerInfo.StatusDateSource.Trim().Split(' ');
                        if (tmpStatusDateSource.Count() != 2)
                        {
                            throw InvalidParameter("statusDateSource");
                        }
                        else
                        {
                            ApplicationUtil.ValidateDate(tmpStatusDateSource[0], operationFlag, "statusDateSource");
                            ApplicationUtil.ValidateTime(tmpStatusDateSource[1].Replace(":", ""), operationFlag, "statusDateSource");

                            statusDateSource = ApplicationUtil.ConvertToDateTime(tmpStatusDateSource[0], tmpStatusDateSource[1].Replace(":", ""));
                            if (statusDateSource.Year == 1)
                            {
                                throw InvalidParameter("statusDateSource");
                            } 
                        }
                    }
                }

                CustomerInfo cusInfo = new CustomerInfo
                {
                    AddrNo = customerInfo.AddrNo,
                    Amphur = customerInfo.Amphur,
                    BaseSalary = customerInfo.BaseSalary,
                    BuildingName = customerInfo.BuildingName,
                    CardType = cardType,
                    Cid = customerInfo.Cid,
                    CustomerCode = customerInfo.CustomerCode,
                    CountryCode=customerInfo.CountryCode,
                    DateOfBirth = customerInfo.DateOfBirth != null ? ApplicationUtil.ConvertToDateTime(customerInfo.DateOfBirth, string.Empty) : new DateTime(),
                    Email = customerInfo.Email,
                    ExtNo1 = customerInfo.ExtNo1,
                    ExtNo2 = customerInfo.ExtNo2,
                    ExtNo3 = customerInfo.ExtNo3,
                    Floor = customerInfo.Floor,
                    IsCustomer = customerInfo.IsCustomer,
                    Lastname = customerInfo.Lastname,
                    Occupation = customerInfo.Occupation,
                    PostalCode = customerInfo.PostalCode,
                    Province = customerInfo.Province,
                    Soi = customerInfo.Soi,
                    Street = customerInfo.Street,
                    Status = customerInfo.Status,
                    SubStatus = customerInfo.SubStatus,
                    SubStatusDesc = customerInfo.SubStatusDesc,
                    StatusDateSource = string.IsNullOrEmpty(customerInfo.StatusDateSource) ? new DateTime() : statusDateSource,
                    StatusReSendFlag = string.IsNullOrEmpty(customerInfo.StatusReSendFlag) ? null : customerInfo.StatusReSendFlag.Trim().ToUpper(),
                    Tambom = customerInfo.Tambom,
                    TelNo2 = customerInfo.TelNo2,
                    TelNo3 = customerInfo.TelNo3,
                    ContractNoRefer = customerInfo.ContractNoRefer
                };

                if ((cardType == ServiceConstant.CardType.Person || cardType == ServiceConstant.CardType.JuristicPerson) && string.IsNullOrEmpty(customerInfo.CountryCode))
                {
                    cusInfo.CountryCode = ApplicationConstant.CBSLeadThaiCountryCode;
                }
                
                return cusInfo;
            }
            else
            {
                return null;
            }            
        }

        /// <summary>
        /// <br>Method Name : ReadCustomerDetail</br>
        /// <br>Purpose     : To read collection of the descendant customerDetail element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="CustomerDetail"></returns>
        private CustomerDetail ReadCustomerDetail(XElement root)
        {
            CustomerDetail cusDetail = new CustomerDetail();
            string username = "";

            var customerDetail = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "customerdetail").Take(1).Select(p => new
            {
                Topic = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "topic") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "topic").Value.Trim() : null,
                Detail = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "detail") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "detail").Value.Trim() : null,
                PathLink = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "pathlink") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "pathlink").Value.Trim() : null,
                TelesaleName = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "telesalename") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "telesalename").Value.Trim() : null,
                AvailableTime = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "availabletime") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "availabletime").Value.Trim() : null,
                ContactBranch = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "contactbranch") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "contactbranch").Value.Trim() : null,
                Subject = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "subject") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "subject").Value.Trim() : null,
                Note = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "note") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "note").Value.Trim() : null
            }).FirstOrDefault();

            if (customerDetail != null)
            {
                if (!string.IsNullOrEmpty(customerDetail.AvailableTime))
                {
                    ApplicationUtil.ValidateTime(customerDetail.AvailableTime, operationFlag, "availableTime");
                }
                    
                if (!string.IsNullOrEmpty(customerDetail.Detail) && customerDetail.Detail.Trim().Length > ApplicationConstant.TextMaxLength)
                {
                    throw new ServiceException(ApplicationResource.INS_LEADDETAIL_EXCEED_MAX_LENGTH_CODE, ApplicationResource.INS_LEADDETAIL_EXCEED_MAX_LENGTH_DESC);
                }

                if (!string.IsNullOrEmpty(customerDetail.TelesaleName))
                {
                    int result;
                    if (int.TryParse(customerDetail.TelesaleName, out result))  //ถ้าเป็นตัวเลขหมด ให้ถือว่าเป็น empcode, ให้นำไปหา username ต่อ
                    {
                        username = LeadServiceBiz.GetUsername(customerDetail.TelesaleName);
                        if (string.IsNullOrEmpty(username))
                        {
                            throw new ServiceException(ApplicationResource.INS_INVALID_TELESALENAME_CODE, ApplicationResource.INS_INVALID_TELESALENAME_DESC);
                        } 
                    }
                    else
                    {
                        username = customerDetail.TelesaleName;
                        if (!LeadServiceBiz.CheckUsernameExist(customerDetail.TelesaleName))
                        {
                            throw new ServiceException(ApplicationResource.INS_INVALID_TELESALENAME_CODE, ApplicationResource.INS_INVALID_TELESALENAME_DESC);
                        }  
                    }
                }

                //Validate Branch ที่ถูกปิด
                if (!string.IsNullOrEmpty(customerDetail.ContactBranch) && ServiceConstant.DoValidateBranch)
                {
                    if (!BranchBiz.CheckOldBranchActive(customerDetail.ContactBranch))//edit by nung 20161124 ==> if (!BranchBiz.CheckBranchActive(customerDetail.ContactBranch))
                    {
                        throw InvalidParameter("contactBranch");
                    }   
                }

                //Insert Note มีเฉพาะขา Update เท่านั้น
                if (operationFlag == ApplicationResource.UPD_OPERATION && !string.IsNullOrEmpty(customerDetail.Note))
                {
                    if (customerDetail.Note.Trim().Length > ApplicationConstant.TextMaxLength)
                    {
                        throw new ServiceException(ApplicationResource.UPD_NOTE_EXCEED_MAX_LENGTH_CODE, ApplicationResource.UPD_NOTE_EXCEED_MAX_LENGTH_DESC);
                    } 
                }

                cusDetail.AvailableTime = string.IsNullOrEmpty(customerDetail.AvailableTime) ? "" : (LeadServiceBiz.CheckAvailableTime(customerDetail.AvailableTime, operationFlag) ? customerDetail.AvailableTime : string.Empty);
                cusDetail.ContactBranch = BranchBiz.GetOldBranchCode(customerDetail.ContactBranch); //edit by nung แปลงค่า ContactBranch(code ใหม่) เป็น branchCode เก่า cusDetail.ContactBranch = customerDetail.ContactBranch;
                cusDetail.Detail = customerDetail.Detail;
                cusDetail.PathLink = customerDetail.PathLink;
                cusDetail.TelesaleName = username;
                cusDetail.Topic = customerDetail.Topic;
                cusDetail.Subject = customerDetail.Subject;
                cusDetail.Note = customerDetail.Note;
                return cusDetail;
            }
            else
            {
                return cusDetail;
            }
        }

        /// <summary>
        /// <br>Method Name : ReadProductInfo</br>
        /// <br>Purpose     : To read collection of the descendant productInfo element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="ProductInfo"></returns>
        private ProductInfo ReadProductInfo(XElement root)
        {
            var productInfo = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "productinfo").Take(1).Select(p => new
            {
                InterestedProdAndType = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "interestedprodandtype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "interestedprodandtype").Value.Trim() : null,
                LicenseNo = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "licenseno") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "licenseno").Value.Trim() : null,
                YearOfCar = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "yearofcar") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "yearofcar").Value.Trim() : null,
                YearOfCarRegis = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "yearofcarregis") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "yearofcarregis").Value.Trim() : null,
                RegisterProvince = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "registerprovince") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "registerprovince").Value.Trim() : null,
                Brand = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "brand") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "brand").Value.Trim() : null,
                Model = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "model") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "model").Value.Trim() : null,
                Submodel = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "submodel") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "submodel").Value.Trim() : null,
                DownPayment = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "downpayment") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "downpayment").Value.Trim() : null,
                DownPercent = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "downpercent") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "downpercent").Value.Trim() : null,
                CarPrice = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "carprice") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "carprice").Value.Trim() : null,
                CarType = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cartype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cartype").Value.Trim() : null,
                FinanceAmt = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "financeamt") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "financeamt").Value.Trim() : null,
                Term = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "term") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "term").Value.Trim() : null,
                PaymentType = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "paymenttype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "paymenttype").Value.Trim() : null,
                BalloonAmt = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "balloonamt") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "balloonamt").Value.Trim() : null,
                BalloonPercent = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "balloonpercent") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "balloonpercent").Value.Trim() : null,
                Plantype = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "plantype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "plantype").Value.Trim() : null,
                CoverageDate = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "coveragedate") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "coveragedate").Value.Trim() : null,
                AccType = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "acctype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "acctype").Value.Trim() : null,
                AccPromotion = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "accpromotion") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "accpromotion").Value.Trim() : null,
                AccTerm = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "accterm") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "accterm").Value.Trim() : null,
                Interest = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "interest") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "interest").Value.Trim() : null,
                Invest = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "invest") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "invest").Value.Trim() : null,
                LoanOd = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "loanod") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "loanod").Value.Trim() : null,
                LoanOdTerm = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "loanodterm") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "loanodterm").Value.Trim() : null,
                SlmBank = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "slmbank") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "slmbank").Value.Trim() : null,
                SlmAtm = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "slmatm") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "slmatm").Value.Trim() : null,
                OtherDetail1 = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "otherdetail1") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "otherdetail1").Value.Trim() : null,
                OtherDetail2 = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "otherdetail2") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "otherdetail2").Value.Trim() : null,
                OtherDetail3 = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "otherdetail3") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "otherdetail3").Value.Trim() : null,
                OtherDetail4 = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "otherdetail4") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "otherdetail4").Value.Trim() : null
            }).FirstOrDefault();

            if (productInfo != null)
            {
                //Validate CoverageDate
                if (!string.IsNullOrEmpty(productInfo.CoverageDate))
                {
                    ApplicationUtil.ValidateDate(productInfo.CoverageDate, operationFlag, "coverageDate");
                }
                    
                ProductInfo prodInfo = new ProductInfo
                {
                    AccPromotion = productInfo.AccPromotion,
                    AccTerm = productInfo.AccTerm,
                    AccType = productInfo.AccType,
                    BalloonAmt = productInfo.BalloonAmt,
                    BalloonPercent = productInfo.BalloonPercent,
                    Brand = productInfo.Brand,
                    CarPrice = productInfo.CarPrice,
                    CarType = productInfo.CarType == "0" || productInfo.CarType == "1" ? productInfo.CarType : string.Empty,
                    CoverageDate = productInfo.CoverageDate,
                    DownPayment = productInfo.DownPayment,
                    DownPercent = productInfo.DownPercent,
                    FinanceAmt = productInfo.FinanceAmt,
                    Interest = productInfo.Interest,
                    InterestedProdAndType = productInfo.InterestedProdAndType,
                    Invest = productInfo.Invest,
                    LicenseNo = productInfo.LicenseNo,
                    LoanOd = productInfo.LoanOd,
                    LoanOdTerm = productInfo.LoanOdTerm,
                    Model = productInfo.Model,
                    OtherDetail1 = productInfo.OtherDetail1,
                    OtherDetail2 = productInfo.OtherDetail2,
                    OtherDetail3 = productInfo.OtherDetail3,
                    OtherDetail4 = productInfo.OtherDetail4,
                    PaymentType = productInfo.PaymentType,
                    Plantype = productInfo.Plantype,
                    RegisterProvince = productInfo.RegisterProvince,
                    SlmAtm = productInfo.SlmAtm,
                    SlmBank = productInfo.SlmBank,
                    Submodel = productInfo.Submodel,
                    Term = productInfo.Term,
                    YearOfCar = productInfo.YearOfCar,
                    YearOfCarRegis = productInfo.YearOfCarRegis
                };
                return prodInfo;
            }
            else
            {
                return null;
            } 
        }

        /// <summary>
        /// <br>Method Name : ReadChannelInfo</br>
        /// <br>Purpose     : To read collection of the descendant channelInfo element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="ChannelInfo"></returns>
        private ChannelInfo ReadChannelInfo(XElement root)
        {
            var channelInfo = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "channelinfo").Take(1).Select(p => new
            {
                ChannelId = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "channelid") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "channelid").Value.Trim() : null,
                Date = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "date") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "date").Value.Trim() : null,
                Time = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "time") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "time").Value.Trim() : null,
                CreateUser = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "createuser") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "createuser").Value.Trim() : null,
                Ipaddress = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "ipaddress") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "ipaddress").Value.Trim() : null,
                Company = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "company") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "company").Value.Trim() : null,
                Branch = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "branch") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "branch").Value.Trim() : null,
                BranchNo = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "branchno") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "branchno").Value.Trim() : null,
                MachineNo = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "machineno") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "machineno").Value.Trim() : null,
                ClientServiceType = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "clientservicetype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "clientservicetype").Value.Trim() : null,
                DocumentNo = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "documentno") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "documentno").Value.Trim() : null,
                CommPaidCode = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "commpaidcode") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "commpaidcode").Value.Trim() : null,
                TransId = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "transid") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "transid").Value.Trim() : null,
                Zone = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "zone") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "zone").Value.Trim() : null
            }).FirstOrDefault();

            if (channelInfo != null)
            {
                //Validate Date
                if (!string.IsNullOrEmpty(channelInfo.Date))
                {
                    ApplicationUtil.ValidateDate(channelInfo.Date, operationFlag, "date");
                }

                //Validate Time
                if (!string.IsNullOrEmpty(channelInfo.Time))
                {
                    ApplicationUtil.ValidateTime(channelInfo.Time, operationFlag, "time");
                }
                    
                //Validate Branch ที่ถูกปิด
                if (!string.IsNullOrEmpty(channelInfo.Branch) && ServiceConstant.DoValidateBranch)
                {
                    if (!BranchBiz.CheckOldBranchActive(channelInfo.Branch))    //edit by nung 20161124 ==> if (!BranchBiz.CheckBranchActive(channelInfo.Branch))
                    {
                        throw InvalidParameter("branch");
                    }    
                }

                ChannelInfo channInfo = new ChannelInfo
                {
                    Branch = BranchBiz.GetOldBranchCode(channelInfo.Branch),    //edit by nung 20161124 ==> Branch = channelInfo.Branch
                    BranchNo = channelInfo.BranchNo,
                    ChannelId = channelInfo.ChannelId,
                    ClientServiceType = channelInfo.ClientServiceType,
                    CommPaidCode = channelInfo.CommPaidCode,
                    Company = channelInfo.Company,
                    CreateUser = channelInfo.CreateUser,
                    Date = channelInfo.Date != null ? ApplicationUtil.ConvertToDateTime(channelInfo.Date, channelInfo.Time) : new DateTime(),
                    DocumentNo = channelInfo.DocumentNo,
                    Ipaddress = channelInfo.Ipaddress,
                    MachineNo = channelInfo.MachineNo,
                    Time = channelInfo.Time,
                    TransId = channelInfo.TransId,
                    Zone = channelInfo.Zone
                };

                //ถ้าส่งตัวเลขเข้ามาให้ถือว่าเป็น empCode, ให้นำไปหา username เพื่อมาใช้งานในระบบ slm
                int result;
                if (int.TryParse(channInfo.CreateUser, out result))
                {
                    channInfo.CreateUser = LeadServiceBiz.GetUsername(channInfo.CreateUser);
                }
                    
                return channInfo;
            }
            else
            {
                return null;
            }   
        }

        /// <summary>
        /// <br>Method Name : ReadDealerInfo</br>
        /// <br>Purpose     : To read collection of the descendant dealerinfo element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="DealerInfo"></returns>
        private DealerInfo ReadDealerInfo(XElement root)
        {
            var dealerInfo = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "dealerinfo").Take(1).Select(p => new
            {
                DealerCode = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "dealercode") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "dealercode").Value.Trim() : null,
                DealName = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "dealername") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "dealername").Value.Trim() : null
            }).FirstOrDefault();

            if (dealerInfo != null)
            {
                if (!string.IsNullOrEmpty(dealerInfo.DealerCode) && dealerInfo.DealerCode.Trim().Length > ApplicationConstant.DealerCodeMaxLength)
                {
                    if (operationFlag == ApplicationResource.INS_OPERATION)
                    {
                        throw new ServiceException(ApplicationResource.INS_DEALERCODE_EXCEED_MAX_LENGTH_CODE, ApplicationResource.INS_DEALERCODE_EXCEED_MAX_LENGTH_DESC);
                    }
                    else
                    {
                        throw new ServiceException(ApplicationResource.UPD_DEALERCODE_EXCEED_MAX_LENGTH_CODE, ApplicationResource.UPD_DEALERCODE_EXCEED_MAX_LENGTH_DESC);
                    }
                }

                if (!string.IsNullOrEmpty(dealerInfo.DealName) && dealerInfo.DealName.Trim().Length > ApplicationConstant.DealerNameMaxLength)
                {
                    if (operationFlag == ApplicationResource.INS_OPERATION)
                    {
                        throw new ServiceException(ApplicationResource.INS_DEALERNAME_EXCEED_MAX_LENGTH_CODE, ApplicationResource.INS_DEALERNAME_EXCEED_MAX_LENGTH_DESC);
                    }
                    else
                    {
                        throw new ServiceException(ApplicationResource.UPD_DEALERNAME_EXCEED_MAX_LENGTH_CODE, ApplicationResource.UPD_DEALERNAME_EXCEED_MAX_LENGTH_DESC);
                    }
                }

                DealerInfo info = new DealerInfo
                {
                    DealerCode = dealerInfo.DealerCode,
                    DealerName = dealerInfo.DealName
                };
                _dealerCode = dealerInfo.DealerCode;
                return info;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// <br>Method Name : ReadReference</br>
        /// <br>Purpose     : To read collection of the descendant Reference element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="string"></returns>
        private string ReadReference(XElement root)
        {
            var reference = root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "reference") != null ? root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "reference").Value.Trim() : null;
            if (reference != null)
            {
                _reference = reference;
                return reference;
            }
            else
            {
                return null;
            } 
        }

        /// <summary>
        /// <br>Method Name : ReadSendEmailFlag</br>
        /// <br>Purpose     : To sendEmailFlag element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="string"></returns>
        private string ReadSendEmailFlag(XElement root)
        {
            var flag = root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "sendemailflag") != null ? root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "sendemailflag").Value.Trim() : null;
            return flag;
        }

        /// <summary>
        /// <br>Method Name : ReadSendEmailPerson</br>
        /// <br>Purpose     : To read sendEmailPerson element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="string"></returns>
        private List<string> ReadSendEmailPerson(XElement root)
        {
            var emails = root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "sendemailperson") != null ? root.Elements().FirstOrDefault(p => p.Name.LocalName.ToLower().Trim() == "sendemailperson").Value.Trim() : null;
            if (string.IsNullOrEmpty(emails))
            {
                return null;
            }
            else
            {
                List<string> emailList = new List<string>();
                string[] arr_email = emails.Split(',');
                foreach (string email in arr_email)
                {
                    if (ValidateEmail(email.Trim()))
                    {
                        if (!emailList.Contains(email.Trim()))
                        {
                            emailList.Add(email.Trim());
                        } 
                    }
                    else
                    {
                        throw new ServiceException(ApplicationResource.UPD_INVALID_SENDEMAILPERSON_CODE, ApplicationResource.UPD_INVALID_SENDEMAILPERSON_DESC);
                    }  
                }
                return emailList;
            }
        }

        /// <summary>
        /// <br>Method Name : ReadSystem</br>
        /// <br>Purpose     : To read system element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="string"></returns>
        private string ReadSystem(XElement root)
        { 
            var system = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "system").FirstOrDefault();
            return (system != null ? system.Value.Trim().ToUpper() : "");
        }

        /// <summary>
        /// <br>Method Name : ReadAppInfo</br>
        /// <br>Purpose     : To read AppInfo element.</br>
        /// </summary>
        /// <param name="root" type="XElement"></param>
        /// <returns type="string"></returns>
        private AppInfo ReadAppInfo(XElement root)
        {
            AppInfo data = new AppInfo();

            var system = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "system").FirstOrDefault();
            if (system != null && !string.IsNullOrEmpty(system.Value.Trim()))
            {
                if (LeadServiceBiz.ValidateSystem(system.Value.Trim().ToUpper()))
                {
                    data.System = system.Value.Trim().ToUpper();
                }
                else
                {
                    return data;
                } 
            }
            else
            {
                return data;   //ถ้าไม่มี tag system ไม่ต้อง insert appinfo
            }

            var appInfoData = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "appinfo").Take(1).Select(p => new
            {
                AppNo = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "appno") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "appno").Value.Trim() : null,
                LastOwner = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "lastowner") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "lastowner").Value.Trim() : null,
                StatusBy = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "statusby") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "statusby").Value.Trim() : null,
                StatusDate = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "statusdate") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "statusdate").Value.Trim() : null,
                CurrentTeam = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "currentteam") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "currentteam").Value.Trim() : null,
                FlowType = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "flowtype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "flowtype").Value.Trim() : null,
                COCType = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "coctype") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "coctype").Value.Trim() : null,
                RoutebackTeam = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "routebackteam") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "routebackteam").Value.Trim() : null,
                MarketingOwner = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "marketingowner") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "marketingowner").Value.Trim() : null,
                COCStatus = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cocstatus") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cocstatus").Value.Trim() : null,
                COCSubStatus = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cocsubstatus") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "cocsubstatus").Value.Trim() : null,
                ContactDetail = p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "contactdetail") != null ? p.Elements().FirstOrDefault(a => a.Name.LocalName.ToLower().Trim() == "contactdetail").Value.Trim() : null
            }).FirstOrDefault();

            if (appInfoData != null)
            {
                //string[] tmpStatusDate = null;

                if (string.IsNullOrEmpty(appInfoData.AppNo))
                {
                    throw RequireField("AppNo");
                }

                data.AppNo = appInfoData.AppNo;
                data.UpdateAppInfoFlag = true;      //ใช้เช็กว่า HPAOFL ต้องการ update ข้อมูล lead อย่างเดียวหรือทั้งข้อมูล lead และ appinfo

                if (operationFlag == ApplicationResource.INS_OPERATION)
                {
                    if (string.IsNullOrEmpty(appInfoData.COCStatus))
                    {
                        throw RequireField("cocStatus");
                    }
                    if (string.IsNullOrEmpty(appInfoData.StatusBy))
                    {
                        throw RequireField("statusBy");
                    }
                    if (string.IsNullOrEmpty(appInfoData.StatusDate))
                    {
                        throw RequireField("statusDate");
                    }

                    string[] tmpStatusDate = appInfoData.StatusDate.Trim().Split(' ');
                    if (tmpStatusDate.Count() != 2)
                    {
                        throw InvalidParameter("statusDate");
                    }
                    else
                    {
                        ApplicationUtil.ValidateDate(tmpStatusDate[0], operationFlag, "statusDate");
                        ApplicationUtil.ValidateTime(tmpStatusDate[1].Replace(":", ""), operationFlag, "statusDate");
                    }

                    data.CurrentTeam = appInfoData.CurrentTeam;
                    data.StatusBy = appInfoData.StatusBy;
                    data.StatusDate = appInfoData.StatusDate != null ? ApplicationUtil.ConvertToDateTime(tmpStatusDate[0], tmpStatusDate[1].Replace(":", "")) : new DateTime();
                    data.COCStatus = appInfoData.COCStatus;
                    data.COCSubStatus = appInfoData.COCSubStatus != null ? appInfoData.COCSubStatus : "";   //ถ้า substatus เป็น null จะแปลงค่าให้เป็นช่องว่างเพื่อใช้เช็กใน mathod update
                }
                else if (operationFlag == ApplicationResource.UPD_OPERATION)
                {
                    if (!string.IsNullOrEmpty(appInfoData.FlowType))
                    {
                        if (appInfoData.FlowType.Trim().ToUpper() != "F" && appInfoData.FlowType.Trim().ToUpper() != "R")
                        {
                            throw InvalidParameter("flowType");
                        }
                    }
                    if (!string.IsNullOrEmpty(appInfoData.COCType))
                    {
                        if (appInfoData.COCType.Trim().ToUpper() != "F" && appInfoData.COCType.Trim().ToUpper() != "P")
                        {
                            throw InvalidParameter("cocType");
                        }
                    }

                    if (string.IsNullOrEmpty(appInfoData.COCStatus))
                    {
                        throw RequireField("cocStatus");
                    }
                    if (string.IsNullOrEmpty(appInfoData.StatusBy))
                    {
                        throw RequireField("statusBy");
                    }
                    if (string.IsNullOrEmpty(appInfoData.StatusDate))
                    {
                        throw RequireField("statusDate");
                    }

                    string[] tmpStatusDate = appInfoData.StatusDate.Trim().Split(' ');
                    if (tmpStatusDate.Count() != 2)
                    {
                        throw InvalidParameter("statusDate");
                    }
                    else
                    {
                        ApplicationUtil.ValidateDate(tmpStatusDate[0], operationFlag, "statusDate");
                        ApplicationUtil.ValidateTime(tmpStatusDate[1].Replace(":", ""), operationFlag, "statusDate");
                    }

                    if (!string.IsNullOrEmpty(appInfoData.ContactDetail) && appInfoData.ContactDetail.Trim().Length > ApplicationConstant.TextMaxLength)
                    {
                        throw new ServiceException(ApplicationResource.UPD_CONTACTDETAIL_EXCEED_MAX_LENGTH_CODE, ApplicationResource.UPD_CONTACTDETAIL_EXCEED_MAX_LENGTH_DESC);
                    }

                    data.LastOwner = appInfoData.LastOwner;
                    data.StatusBy = appInfoData.StatusBy;
                    data.StatusDate = appInfoData.StatusDate != null ? ApplicationUtil.ConvertToDateTime(tmpStatusDate[0], tmpStatusDate[1].Replace(":", "")) : new DateTime();
                    data.CurrentTeam = appInfoData.CurrentTeam;
                    data.FlowType = appInfoData.FlowType != null ? appInfoData.FlowType.Trim().ToUpper() : null;
                    data.COCType = appInfoData.COCType != null ? appInfoData.COCType.Trim().ToUpper() : null;
                    data.RoutebackTeam = appInfoData.RoutebackTeam;
                    data.MarketingOwner = appInfoData.MarketingOwner;
                    data.COCStatus = appInfoData.COCStatus;
                    data.COCSubStatus = appInfoData.COCSubStatus != null ? appInfoData.COCSubStatus : "";   //ถ้า substatus เป็น null จะแปลงค่าให้เป็นช่องว่างเพื่อใช้เช็กใน mathod update
                    data.ContactDetail = appInfoData.ContactDetail;
                }
            }
            else
            {
                if (operationFlag == ApplicationResource.INS_OPERATION)
                {
                    throw RequireField("AppInfo");
                }
                else if (operationFlag == ApplicationResource.UPD_OPERATION)
                {
                    data.UpdateAppInfoFlag = false;
                }
            }

            return data;
        }

        private bool FromWeb(XElement root)
        {
            var fromweb = root.Elements().Where(p => p.Name.LocalName.ToLower().Trim() == "fromweb").FirstOrDefault();
            if (fromweb != null)
            {
                return fromweb.Value.Trim() == "1" ? true : false;
            }
            else
            {
                return false;
            }  
        }

        /// <summary>
        /// <br>Method Name : GetMarketingCode</br>
        /// <br>Purpose     : To get marketing code from AOL web service.</br>
        /// </summary>
        /// <param name="dealerCode" type="string"></param>
        /// <returns type="string"></returns>
        private string GetMarketingCode(string dealerCode)
        {
            try
            {
                int timeout = 0;

                string aolUser = System.Configuration.ConfigurationManager.AppSettings["AolUser"];
                if (aolUser == null)
                {
                    throw new ServiceException(ApplicationResource.INS_OTHER_CONDITION_ERROR_CODE, ApplicationResource.INS_OTHER_CONDITION_ERROR_DESC, "ไม่พบ AolUser ใน Configuation File", null);
                }

                string aolPassword = System.Configuration.ConfigurationManager.AppSettings["AolPassword"];
                if (aolPassword == null)
                {
                    throw new ServiceException(ApplicationResource.INS_OTHER_CONDITION_ERROR_CODE, ApplicationResource.INS_OTHER_CONDITION_ERROR_DESC, "ไม่พบ AolPassword ใน Configuation File", null);
                }

                string aolServiceName = System.Configuration.ConfigurationManager.AppSettings["AolServiceName"];
                if (aolServiceName == null)
                {
                    throw new ServiceException(ApplicationResource.INS_OTHER_CONDITION_ERROR_CODE, ApplicationResource.INS_OTHER_CONDITION_ERROR_DESC, "ไม่พบ AolServiceName ใน Configuation File", null);
                }

                string aolSystemCode = System.Configuration.ConfigurationManager.AppSettings["AolSystemCode"];
                if (aolSystemCode == null)
                {
                    throw new ServiceException(ApplicationResource.INS_OTHER_CONDITION_ERROR_CODE, ApplicationResource.INS_OTHER_CONDITION_ERROR_DESC, "ไม่พบ AolSystemCode ใน Configuation File", null);
                }

                if (System.Configuration.ConfigurationManager.AppSettings["AolTimeout"] == null)
                {
                    throw new ServiceException(ApplicationResource.INS_OTHER_CONDITION_ERROR_CODE, ApplicationResource.INS_OTHER_CONDITION_ERROR_DESC, "ไม่พบ AolTimeout ใน Configuation File", null);
                }
                else
                {
                    timeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AolTimeout"]);
                }

                AolServiceProxy.HpaoflExtServiceClient service = new AolServiceProxy.HpaoflExtServiceClient();
                AolServiceProxy.Header header = new AolServiceProxy.Header()
                {
                    TransactionDate = DateTime.Now,
                    UserName = aolUser,
                    Password = aolPassword,
                    ServiceName = aolServiceName,
                    SystemCode = aolSystemCode
                };
                AolServiceProxy.GetMainMarketingByDealerRequestDetail body = new AolServiceProxy.GetMainMarketingByDealerRequestDetail()
                {
                    DealerCode = dealerCode
                };
                AolServiceProxy.GetMainMarketingByDealerRequest request = new AolServiceProxy.GetMainMarketingByDealerRequest()
                {
                    Header = header,
                    Body = body
                };

                service.InnerChannel.OperationTimeout = new TimeSpan(0, 0, timeout);
                AolServiceProxy.GetMainMarketingByDealerResponse response = service.GetMainMarketingByDealer(request);
                if (response.Status.Status == "SUCCESS")
                {
                    _marketingCode = response.Body.MktCode;
                    string username = LeadServiceBiz.GetUsername(response.Body.MktCode);
                    if (username == string.Empty)
                    {
                        throw new ServiceException(ApplicationResource.INS_MARKETINGCODE_MISMATCH_CODE, ApplicationResource.INS_MARKETINGCODE_MISMATCH_DESC);
                    }
                    else
                    {
                        return username;
                    }
                }
                else
                {
                    if (response.Status.ErrorCode == "E042")
                    {
                        throw new ServiceException(ApplicationResource.INS_NO_MARKETINGCODE_FOUND_CODE, ApplicationResource.INS_NO_MARKETINGCODE_FOUND_DESC, response.Status.Description, null);
                    }
                    else
                    {
                        throw new ServiceException(ApplicationResource.INS_OTHER_CONDITION_ERROR_CODE, ApplicationResource.INS_OTHER_CONDITION_ERROR_DESC, response.Status.Description, null);
                    }
                }
            }
            catch (TimeoutException ex)
            {
                throw new ServiceException(ApplicationResource.INS_CALL_AOL_TIMEOUT_CODE, ApplicationResource.INS_CALL_AOL_TIMEOUT_DESC, ex.Message, ex.InnerException);
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <br>Method Name : CreateResponseXml</br>
        /// <br>Purpose     : To create response xml string.</br>
        /// </summary>
        /// <param name="ticketId" type="string"></param>
        /// <param name="responseCode" type="string"></param>
        /// <param name="responseMsg" type="string"></param>
        /// <returns type="string"></returns>
        private string CreateResponseXml(string ticketId, string channelId, string responseCode, string responseMsg, DateTime responseDate)
        {
            if (operationFlag == ApplicationResource.INS_OPERATION && channelId == _DCM)
            {
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("ticket", new XAttribute("id", ticketId),
                        new XElement("ticket", ticketId),
                        new XElement("responseCode", responseCode),
                        new XElement("responseMessage", responseMsg),
                        new XElement("responseDate", responseDate.ToString("dd-MM-") + responseDate.Year.ToString()),
                        new XElement("responseTime", responseDate.ToString("HH:mm:ss")),
                        new XElement("dealerCode", _dealerCode != null ? _dealerCode : ""),
                        new XElement("marketingCode", _marketingCode),
                        new XElement("reference", _reference != null ? _reference : "")
                        )
                    );

                return "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + doc;
            }
            else
            {
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("ticket", new XAttribute("id", ticketId),
                        new XElement("ticket", ticketId),
                        new XElement("responseCode", responseCode),
                        new XElement("responseMessage", responseMsg),
                        new XElement("responseDate", responseDate.ToString("dd-MM-") + responseDate.Year.ToString()),
                        new XElement("responseTime", responseDate.ToString("HH:mm:ss"))
                        )
                    );

                return "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + doc;
            }
        }

        /// <summary>
        /// <br>Method Name : UpdateChannelInfoResponse</br>
        /// <br>Purpose     : To update response data to table channelInfo.</br>
        /// </summary>
        /// <param name="ticketId" type="string"></param>
        /// <param name="responseCode" type="string"></param>
        /// <param name="responseMsg" type="string"></param>
        /// <param name="responseDate" type="DateTime"></param>
        /// <returns type="void"></returns>
        //private void UpdateChannelInfoResponse(string ticketId, string responseCode, string responseMsg, DateTime responseDate)
        //{
        //    try
        //    {
        //        LeadServiceBiz.UpdateChannelInfoResponse(ticketId, responseCode, responseMsg, responseDate, operationFlag, timeout);
        //    }
        //    catch(Exception ex)
        //    {
        //        log.Debug(ex.Message);
        //    }
        //}

        /// <summary>
        /// <br>Method Name : ValidateEmail</br>
        /// <br>Purpose     : To validate email.</br>
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool ValidateEmail(string email)
        {
            string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern);
            return reg.IsMatch(email);
        }

        /// <summary>
        /// <br>Method Name : VerifyCitizenId</br>
        /// <br>Purpose     : To validate citizen Id</br>
        /// </summary>
        /// <param name="citizenId"></param>
        /// <returns></returns>
        private bool VerifyCitizenId(string citizenId)
        {
            try
            {
                //ตรวจสอบว่าข้อมูลมีทั้งหมด 13 ตัวอักษร
                if (citizenId.Trim().Length != 13)
                {
                    return false;
                }
                    
                //ตรวจสอบต้องเป็นตัวเลขทั้งหมด
                decimal result;
                if (!decimal.TryParse(citizenId.Trim(), out result))
                {
                    return false;
                }
                    
                int sumValue = 0;
                for (int i = 0; i < citizenId.Length - 1; i++)
                    sumValue += int.Parse(citizenId[i].ToString()) * (13 - i);

                int v = 11 - (sumValue % 11);

                string digit = "";
                if (v.ToString().Length == 2)
                {
                    digit = v.ToString().Substring(1, 1);
                }
                else
                {
                    digit = v.ToString();
                }
                    
                return citizenId[12].ToString() == digit;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// <br>Method Name : VerifyJuristicPersonId</br>
        /// <br>Purpose     : To validate juristic Id</br>
        /// </summary>
        /// <param name="juristicId"></param>
        /// <returns></returns>
        private bool VerifyJuristicPersonId(string juristicId)
        {
            try
            {
                ////ตรวจสอบว่าข้อมูลมีทั้งหมด 13 ตัวอักษร
                //if (juristicId.Trim().Length != 13)
                //    return false;

                ////ตรวจสอบต้องเป็นตัวเลขทั้งหมด
                //decimal result;
                //if (!decimal.TryParse(juristicId.Trim(), out result))
                //    return false;

                if (juristicId.Trim().Length > 50)
                {
                    return false;
                }                  

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// <br>Method Name : SearchLead</br>
        /// <br>Purpose     : To search lead.</br>
        /// </summary>
        /// <param name="request" type="SearchLeadRequest"></param>
        /// <returns type="SearchLeadResponse"></returns>
        public SearchLeadResponse SearchLead(SearchLeadRequest request)
        {
            string ticketId = string.Empty;
            string channelId = string.Empty;
            string username = string.Empty;
            string request_xml = string.Empty;
            string causeError = string.Empty;
            DateTime responseDate = new DateTime();
            DateTime operationDate = DateTime.Now;
            operationFlag = ApplicationResource.SEARCH_OPERATION;
            SearchLeadResponse response = new SearchLeadResponse();

            try
            {
                log.Debug("===================== Start Log =====================");
                log.Debug("Call Search Operation at " + operationDate.ToString());
                SetHeaderLog(request.RequestHeader, operationFlag);

                if (request == null)
                {
                    //throw new Exception("Required Fields");
                    throw new ServiceException(ApplicationResource.SEARCH_REQUIRED_FIELDS_CODE, ApplicationResource.SEARCH_REQUIRED_FIELDS_DESC);
                }

                response.ResponseHeader = request.RequestHeader;

                if (!ValidateXml(request.RequestXml))
                {
                    //throw new Exception("Required Fields");
                    throw new ServiceException(ApplicationResource.SEARCH_REQUIRED_FIELDS_CODE, ApplicationResource.SEARCH_REQUIRED_FIELDS_DESC);
                }

                XDocument doc = XDocument.Parse(request.RequestXml);
                ticketId = ReadTicketId(doc);

                if (!ValidateHeader(request.RequestHeader))
                {
                    //throw new Exception("Invalid System ID or Password");
                    throw new ServiceException(ApplicationResource.SEARCH_INVALID_ID_PASSWORD_CODE, ApplicationResource.SEARCH_INVALID_ID_PASSWORD_DESC);
                }

                if (!AuthenticateHeader(request.RequestHeader, ApplicationResource.SEARCH_OPERATION))
                {
                    //throw new Exception("Invalid System ID or Password");
                    throw new ServiceException(ApplicationResource.SEARCH_INVALID_ID_PASSWORD_CODE, ApplicationResource.SEARCH_INVALID_ID_PASSWORD_DESC);
                }

                response.ResponseXml = SearchLeadData(ticketId, operationDate, request);

                log.Debug("ResponseCode : " + ApplicationResource.SEARCH_SUCCESS_CODE + ", ResponseDesc : " + ApplicationResource.SEARCH_SUCCESS_DESC);
                log.Debug("Xml : " + request.RequestXml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");
            }
            catch (ServiceException ex)
            {
                causeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                responseDate = DateTime.Now;
                if (request != null)
                {
                    request_xml = request.RequestXml;
                }
                if (request != null && request.RequestHeader != null)
                {
                    channelId = request.RequestHeader.ChannelID;
                    username = request.RequestHeader.Username;
                }

                response.ResponseXml = ApplicationUtil.GenerateXml(null, ticketId, ex.ResponseCode, ex.ResponseDesc, responseDate);

                log.Debug("ResponseCode : " + ex.ResponseCode + ", ResponseDesc : " + ex.ResponseDesc);
                log.Debug("Cause : " + causeError);
                log.Debug("Xml : " + request_xml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, channelId, username, request_xml, response.ResponseXml
                    , ex.ResponseCode, ex.ResponseDesc, responseDate, responseDate.ToString("HH:mm:ss"), causeError);
            }
            catch (Exception ex)
            {
                causeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                responseDate = DateTime.Now;
                if (request != null)
                {
                    request_xml = request.RequestXml;
                }
                if (request != null && request.RequestHeader != null)
                {
                    channelId = request.RequestHeader.ChannelID;
                    username = request.RequestHeader.Username;
                }

                response.ResponseXml = ApplicationUtil.GenerateXml(null, ticketId, ApplicationResource.SEARCH_OTHER_CONDITION_ERROR_CODE, ApplicationResource.SEARCH_OTHER_CONDITION_ERROR_DESC, responseDate);
                
                log.Debug("ResponseCode : " + ApplicationResource.SEARCH_OTHER_CONDITION_ERROR_CODE + ", ResponseDesc : " + ApplicationResource.SEARCH_OTHER_CONDITION_ERROR_DESC);
                log.Debug("Cause : " + causeError);
                log.Debug("Xml : " + request_xml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, channelId, username, request_xml, response.ResponseXml
                    , ApplicationResource.SEARCH_OTHER_CONDITION_ERROR_CODE, ApplicationResource.SEARCH_OTHER_CONDITION_ERROR_DESC, responseDate, responseDate.ToString("HH:mm:ss"), causeError);
            }

            return response;
        }

        /// <summary>
        /// <br>Method Name : SearchLeadData</br>
        /// <br>Purpose     : To read xml string and search data to database.</br>
        /// </summary>
        /// <param name="doc" type="XDocument"></param>
        /// <param name="ticketId" type="string"></param>
        /// <returns type="string">TicketId</returns>
        private string SearchLeadData(string ticketId, DateTime operationDate, SearchLeadRequest request)
        {
            try
            {
                string responseCode = string.Empty;
                string responseMessage = string.Empty;
                string response_xml = string.Empty;

                SearchLeadData data = LeadServiceBiz.SearchLeadData(ticketId, timeout);

                if (data != null)
                {
                    //convert customerdetail.contactBranch, channelInfo.Branch, to newBranchCode
                    data.ContactBranch = BranchBiz.GetNewBranchCode(data.ContactBranch);
                    data.Branch = BranchBiz.GetNewBranchCode(data.Branch);

                    responseCode = ApplicationResource.SEARCH_SUCCESS_CODE;
                    responseMessage = ApplicationResource.SEARCH_SUCCESS_DESC;
                }
                else
                {
                    responseCode = ApplicationResource.SEARCH_NO_RECORD_FOUND_CODE;
                    responseMessage = ApplicationResource.SEARCH_NO_RECORD_FOUND_DESC;
                }

                DateTime responseDate = DateTime.Now;
                response_xml = ApplicationUtil.GenerateXml(data, ticketId, responseCode, responseMessage, responseDate);

                InsertLogData(ticketId, operationFlag, operationDate, request.RequestHeader.ChannelID, request.RequestHeader.Username, request.RequestXml, response_xml
                    , responseCode, responseMessage, responseDate, responseDate.ToString("HH:mm:ss"), "");

                return response_xml;
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertLogData(string ticketId, string operation, DateTime operationDate, string channelId, string username, string inputXml, string outputXml
            , string responseCode, string responseDesc, DateTime responseDate, string responseTime, string causeError)
        {
            try
            {
                LeadServiceBiz.InsertLogData(ticketId, operation, operationDate, channelId, username, inputXml, outputXml, responseCode, responseDesc, responseDate, responseTime, causeError);
            }
            catch(Exception ex)
            { 
                log.Debug(ex.Message);
            }
        }

        /// <summary>
        /// <br>Method Name : ValidatePermission</br>
        /// <br>Purpose     : To read xml string and Validate Permission of user by ticketid in database.</br>
        /// </summary>
        /// <param name="doc" type="XDocument"></param>
        /// <param name="ticketId" type="string"></param>
        /// <returns type="string">ValidatePermission</returns>
        public ValidatePermissionResponse ValidatePermission(ValidatePermissionRequest request)
        {
            string ticketId = string.Empty;
            string channelId = string.Empty;
            string username = string.Empty;
            string request_xml = string.Empty;
            string causeError = string.Empty;
            DateTime responseDate = new DateTime();
            DateTime operationDate = DateTime.Now;
            operationFlag = ApplicationResource.VAL_PER_OPERATION;
            ValidatePermissionResponse response = new ValidatePermissionResponse();

            try
            {
                log.Debug("===================== Start Log =====================");
                log.Debug("Call Validate Operation at " + operationDate.ToString());
                SetHeaderLog(request.RequestHeader, operationFlag);

                if (request == null)
                {
                    //throw new Exception("Required Fields");
                    throw new ServiceException(ApplicationResource.VAL_PER_REQUIRED_FIELDS_CODE, ApplicationResource.VAL_PER_REQUIRED_FIELDS_DESC);
                }

                response.ResponseHeader = request.RequestHeader;

                if (!ValidateXml(request.RequestXml))
                {
                    //throw new Exception("Required Fields");
                    throw new ServiceException(ApplicationResource.VAL_PER_REQUIRED_FIELDS_CODE, ApplicationResource.VAL_PER_REQUIRED_FIELDS_DESC);
                }

                XDocument doc = XDocument.Parse(request.RequestXml);
                ticketId = ReadTicketId(doc);

                if (!ValidateHeader(request.RequestHeader))
                {
                    //throw new Exception("Invalid System ID or Password");
                    throw new ServiceException(ApplicationResource.VAL_PER_INVALID_ID_PASSWORD_CODE, ApplicationResource.VAL_PER_INVALID_ID_PASSWORD_DESC);
                }

                if (!AuthenticateHeader(request.RequestHeader, ApplicationResource.VAL_PER_OPERATION))
                {
                    //throw new Exception("Invalid System ID or Password");
                    throw new ServiceException(ApplicationResource.VAL_PER_INVALID_ID_PASSWORD_CODE, ApplicationResource.VAL_PER_INVALID_ID_PASSWORD_DESC);
                }

                response.ResponseXml = ValidatePermissionData(ticketId, operationDate, request, doc);

                log.Debug("ResponseCode : " + ApplicationResource.VAL_PER_SUCCESS_CODE + ", ResponseDesc : " + ApplicationResource.VAL_PER_SUCCESS_DESC);
                log.Debug("Xml : " + request.RequestXml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");
            }
            catch (ServiceException ex)
            {
                causeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                responseDate = DateTime.Now;
                if (request != null)
                {
                    request_xml = request.RequestXml;
                }
                if (request != null && request.RequestHeader != null)
                {
                    channelId = request.RequestHeader.ChannelID;
                    username = request.RequestHeader.Username;
                }

                response.ResponseXml = CreateResponseXml(ticketId, channelId, ex.ResponseCode, ex.ResponseDesc, responseDate);
                
                log.Debug("ResponseCode : " + ex.ResponseCode + ", ResponseDesc : " + ex.ResponseDesc);
                log.Debug("Cause : " + causeError);
                log.Debug("Xml : " + request_xml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, channelId, username, request_xml, response.ResponseXml
                    , ex.ResponseCode, ex.ResponseDesc, responseDate, responseDate.ToString("HH:mm:ss"), causeError);
            }
            catch (Exception ex)
            {
                causeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                responseDate = DateTime.Now;
                if (request != null)
                {
                    request_xml = request.RequestXml;
                }
                if (request != null && request.RequestHeader != null)
                {
                    channelId = request.RequestHeader.ChannelID;
                    username = request.RequestHeader.Username;
                }

                response.ResponseXml = CreateResponseXml(ticketId, channelId, ApplicationResource.VAL_PER_OTHER_CONDITION_ERROR_CODE, ApplicationResource.VAL_PER_OTHER_CONDITION_ERROR_DESC, responseDate);
                
                log.Debug("ResponseCode : " + ApplicationResource.VAL_PER_OTHER_CONDITION_ERROR_CODE + ", ResponseDesc : " + ApplicationResource.VAL_PER_OTHER_CONDITION_ERROR_DESC);
                log.Debug("Cause : " + causeError);
                log.Debug("Xml : " + request_xml);
                log.Debug("Xml Result : " + response.ResponseXml);
                log.Debug("===================== End Log ======================");
                log.Debug("");

                InsertLogData(ticketId, operationFlag, operationDate, channelId, username, request_xml, response.ResponseXml
                    , ApplicationResource.VAL_PER_OTHER_CONDITION_ERROR_CODE, ApplicationResource.VAL_PER_OTHER_CONDITION_ERROR_DESC, responseDate, responseDate.ToString("HH:mm:ss"), causeError);
            }

            return response;
        }

        /// <summary>
        /// <br>Method Name : ValidatePermissionData</br>
        /// <br>Purpose     : To read xml string and validate permission data to database.</br>
        /// </summary>
        /// <param name="doc" type="XDocument"></param>
        /// <param name="ticketId" type="string"></param>
        /// <returns type="string">TicketId</returns>
        private string ValidatePermissionData(string ticketId, DateTime operationDate, ValidatePermissionRequest request, XDocument doc)
        {
            try
            {
                string responseCode = string.Empty;
                string responseMessage = string.Empty;
                string response_xml = string.Empty;

                var root = doc.Root;
                if (root == null || root.Name.LocalName.ToLower().Trim() != "ticket")
                {
                    throw RequireField("ticket");
                }
                
                CustomerDetail cusDetail = ReadCustomerDetail(root);

                //Validate TicketId & Access Right By TicketId
                if (!string.IsNullOrEmpty(cusDetail.TelesaleName))
                {
                    if (AccessRightBiz.ValidateTicketId(ticketId))
                    {
                        if (AccessRightBiz.CheckAccessRightByTicketId(ticketId, cusDetail.TelesaleName))
                        {
                            responseCode = ApplicationResource.VAL_PER_SUCCESS_CODE;
                            responseMessage = ApplicationResource.VAL_PER_SUCCESS_DESC;
                        }
                        else
                        {
                            responseCode = ApplicationResource.VAL_PER_NO_PERMISSON_CODE;
                            responseMessage = ApplicationResource.VAL_PER_NO_PERMISSON_DESC;
                        }
                    } else
                    {
                        responseCode = ApplicationResource.VAL_INVALID_TICKET_ID_CODE;
                        responseMessage = ApplicationResource.VAL_INVALID_TICKET_ID_DESC;
                    }
                }
                else
                {
                    throw RequireField("telesaleName");
                }

                DateTime responseDate = DateTime.Now;
                response_xml = CreateResponseXml(ticketId, request.RequestHeader.ChannelID, responseCode, responseMessage, responseDate);
                
                InsertLogData(ticketId, operationFlag, operationDate, request.RequestHeader.ChannelID, request.RequestHeader.Username, request.RequestXml, response_xml
                    , responseCode, responseMessage, responseDate, responseDate.ToString("HH:mm:ss"), "");

                return response_xml;
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

            #endregion

        }
}
