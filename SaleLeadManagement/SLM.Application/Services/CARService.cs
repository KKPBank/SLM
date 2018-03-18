using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Biz;
using System.IO;
using System.Xml.Serialization;
using System.Configuration;
using log4net;

namespace SLM.Application.Services
{
    public class CARService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CARService));

        //Added by Pom 29/06/2016
        public static bool CreateActivityLog(CARServiceData data)
        {
            string errMsg = "";
            try
            {
                CARServiceProxy.LogServiceHeader header = new CARServiceProxy.LogServiceHeader()
                {
                    ReferenceNo = data.ReferenceNo,
                    SecurityKey = data.SecurityKey,
                    ServiceName = SLMConstant.CARLogService.CARCreateActivityLogServiceName,
                    SystemCode = data.SystemCode,
                    TransactionDateTime = data.TransactionDateTime
                };

                data.ServiceName = SLMConstant.CARLogService.CARCreateActivityLogServiceName;

                //Activity Info
                List<CARServiceProxy.DataItem> actInfoList = new List<CARServiceProxy.DataItem>();
                foreach (Services.CARService.DataItem info in data.ActivityInfoList)
                {
                    actInfoList.Add(new CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                //Customer Info
                List<CARServiceProxy.DataItem> cusInfoList = new List<CARServiceProxy.DataItem>();
                foreach (Services.CARService.DataItem info in data.CustomerInfoList)
                {
                    cusInfoList.Add(new CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                //Product Info
                List<CARServiceProxy.DataItem> prodInfoList = new List<CARServiceProxy.DataItem>();
                foreach (Services.CARService.DataItem info in data.ProductInfoList)
                {
                    prodInfoList.Add(new CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                //Contact Info
                List<CARServiceProxy.DataItem> contInfoList = new List<CARServiceProxy.DataItem>();
                foreach (Services.CARService.DataItem info in data.ContractInfoList)
                {
                    contInfoList.Add(new CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                //Officer Info
                List<CARServiceProxy.DataItem> offInfoList = new List<CARServiceProxy.DataItem>();
                foreach (Services.CARService.DataItem info in data.OfficerInfoList)
                {
                    offInfoList.Add(new CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                CARServiceProxy.CreateActivityLogData logdata = new CARServiceProxy.CreateActivityLogData()
                {
                    ActivityInfoList = actInfoList.ToArray(),
                    CustomerInfoList = cusInfoList.ToArray(),
                    ProductInfoList = prodInfoList.ToArray(),
                    ContractInfoList = contInfoList.ToArray(),
                    OfficerInfoList = offInfoList.ToArray(),
                    ActivityDateTime = DateTime.Now,
                    CampaignID = data.CampaignId,
                    ChannelID = data.ChannelId,
                    LeadID = data.PreleadId,
                    ProductGroupID = data.ProductGroupId,
                    ProductID = data.ProductId,
                    Status = data.Status,
                    SubStatus = data.SubStatus,
                    TicketID = data.TicketId,
                    SubscriptionTypeID = data.CIFSubscriptionTypeId,
                    SubscriptionID = data.SubscriptionId,
                    TypeID = data.TypeId,
                    AreaID = data.AreaId,
                    SubAreaID = data.SubAreaId,
                    ActivityTypeID = data.ActivityTypeId,
                    ContractID = data.ContractNo
                };

                CARServiceProxy.CASLogServiceSoapClient service = new CARServiceProxy.CASLogServiceSoapClient();
                service.InnerChannel.OperationTimeout = new TimeSpan(0, 0, SLMConstant.CARLogService.CARTimeout);
                var result = service.CreateActivityLog(header, logdata);

                if (result.ResponseStatus.ResponseCode == "CAS-I-000")
                {
                    return true;
                }
                else
                {
                    if (!AddCARLog(LogType.Error, result.ResponseStatus.ResponseCode, result.ResponseStatus.ResponseMessage, data, out errMsg))
                    {
                        _log.Error("Insert kkslm_tr_cas_resend failed: " + errMsg);
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);

                if (!AddCARLog(LogType.Fail, "", message, data, out errMsg))
                {
                    _log.Error("Insert kkslm_tr_cas_resend failed: " + errMsg);
                }
                    
                return false;
            }
        }

        public string GetCallCASScript(string ticketId, decimal? preleadId, string cbsSubscriptionTypeId, string citizenId, string userId)
        {
            string url = SLMConstant.CARLogService.CARActivityUrl;
            string script = @"var form = document.createElement('form');
                                var input_system = document.createElement('input');
                                var input_ticket = document.createElement('input');
                                var input_subscriptiontype = document.createElement('input');
                                var input_subscription = document.createElement('input');
                                var input_lead = document.createElement('input');
                                var input_userid = document.createElement('input');
            
                                form.action = '" + url + @"';
                                form.method = 'post';
                                form.setAttribute('target', 'searchcas');
            
                                input_system.name = 'system';
                                input_system.value = '" + (preleadId != null ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM) + @"';
                                form.appendChild(input_system);
            
                                input_ticket.name = 'ticket';
                                input_ticket.value = '" + ticketId + @"';
                                form.appendChild(input_ticket);
            
                                input_subscriptiontype.name = 'subscriptiontype';
                                input_subscriptiontype.value = '" + cbsSubscriptionTypeId + @"';
                                form.appendChild(input_subscriptiontype);
            
                                input_subscription.name = 'subscription';
                                input_subscription.value = '" + citizenId + @"';
                                form.appendChild(input_subscription);
            
                                input_lead.name = 'lead';
                                input_lead.value = '" + (preleadId != null ? preleadId.Value.ToString() : "") + @"';
                                form.appendChild(input_lead);
            
                                input_userid.name = 'userid'
                                input_userid.value = '" + userId + @"';
                                form.appendChild(input_userid);

                                document.body.appendChild(form);
                                form.submit();
            
                                document.body.removeChild(form);";

            return script;
        }

        public string GetCallCASScript(LeadData data, string userId)
        {
            string subScriptionTypeId = "0";
            if (data.CardType != null)
            {
                subScriptionTypeId = SlmScr008Biz.GetSubScriptionTypeId(data.CardType.Value);
            }

            string url = SLMConstant.CARLogService.CARActivityUrl;
            string script = @"var form = document.createElement('form');
                                var input_system = document.createElement('input');
                                var input_ticket = document.createElement('input');
                                var input_subscriptiontype = document.createElement('input');
                                var input_subscription = document.createElement('input');
                                var input_lead = document.createElement('input');
                                var input_userid = document.createElement('input');
            
                                form.action = '" + url + @"';
                                form.method = 'post';
                                form.setAttribute('target', 'searchcas');
            
                                input_system.name = 'system';
                                input_system.value = '" + (data.PreleadId != null ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM) + @"';
                                form.appendChild(input_system);
            
                                input_ticket.name = 'ticket';
                                input_ticket.value = '" + data.TicketId + @"';
                                form.appendChild(input_ticket);
            
                                input_subscriptiontype.name = 'subscriptiontype';
                                input_subscriptiontype.value = '" + subScriptionTypeId + @"';
                                form.appendChild(input_subscriptiontype);
            
                                input_subscription.name = 'subscription';
                                input_subscription.value = '" + data.CitizenId + @"';
                                form.appendChild(input_subscription);
            
                                input_lead.name = 'lead';
                                input_lead.value = '" + (data.PreleadId != null ? data.PreleadId.Value.ToString() : "") + @"';
                                form.appendChild(input_lead);

                                input_userid.name = 'userid'
                                input_userid.value = '" + userId + @"';
                                form.appendChild(input_userid);
            
                                document.body.appendChild(form);
                                form.submit();
            
                                document.body.removeChild(form);";

            return script;
        }

        public string GetCallCASScript(PreleadViewData data, string userId)
        {
            string subScriptionTypeId = "0";
            if (data.CardTypeId != null)
            {
                subScriptionTypeId = SlmScr008Biz.GetSubScriptionTypeId(data.CardTypeId.Value);
            }

            string url = SLMConstant.CARLogService.CARActivityUrl;
            string script = @"var form = document.createElement('form');
                                var input_system = document.createElement('input');
                                var input_ticket = document.createElement('input');
                                var input_subscriptiontype = document.createElement('input');
                                var input_subscription = document.createElement('input');
                                var input_lead = document.createElement('input');
                                var input_userid = document.createElement('input');
            
                                form.action = '" + url + @"';
                                form.method = 'post';
                                form.setAttribute('target', 'searchcas');
            
                                input_system.name = 'system';
                                input_system.value = '" + (data.PreleadId != null ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM) + @"';
                                form.appendChild(input_system);
            
                                input_ticket.name = 'ticket';
                                input_ticket.value = '" + data.TicketId + @"';
                                form.appendChild(input_ticket);
            
                                input_subscriptiontype.name = 'subscriptiontype';
                                input_subscriptiontype.value = '" + subScriptionTypeId + @"';
                                form.appendChild(input_subscriptiontype);
            
                                input_subscription.name = 'subscription';
                                input_subscription.value = '" + data.CitizenId + @"';
                                form.appendChild(input_subscription);
            
                                input_lead.name = 'lead';
                                input_lead.value = '" + (data.PreleadId != null ? data.PreleadId.Value.ToString() : "") + @"';
                                form.appendChild(input_lead);

                                input_userid.name = 'userid'
                                input_userid.value = '" + userId + @"';
                                form.appendChild(input_userid);
            
                                document.body.appendChild(form);
                                form.submit();
            
                                document.body.removeChild(form);";

            return script;
        }

        public static bool AddCARLog(LogType _logType, string ResponseCode, string ErrorMessage, CARServiceData data, out string error)
        {
            error = "";
            bool ret = true;
            try
            {
                // get config
                string errorCodeResend = SLMConstant.CARLogService.CARErrorCodeResend;
                int maxResend = SLMConstant.CARLogService.CARMaxResend;


                // create cas resend item
                CARServiceBiz.CASResendData crr = new CARServiceBiz.CASResendData();
                crr.slm_CasReferenceNo = data.ReferenceNo;
                crr.slm_CasSystemCode = data.SystemCode;
                crr.slm_CasServiceName = data.ServiceName;
                crr.slm_CasLogType = _logType == LogType.Error ? "E" : "F";
                crr.slm_CasResponse = ResponseCode;
                crr.slm_CasErrorMessage = ErrorMessage;
                crr.slm_ResendCount = 0;
                crr.slm_CreatedBy = data.SystemCode;
                crr.slm_UpdatedBy = data.SystemCode;


                // set log status
                if (_logType == LogType.Fail)
                {
                    crr.slm_LogStatus = "R";
                }
                else
                {
                    crr.slm_LogStatus = "F";
                    if (!String.IsNullOrEmpty(errorCodeResend))
                    {
                        var codes = errorCodeResend.Replace(" ", "").Split(',');
                        foreach (var code in codes)
                            if (ResponseCode.Contains(code)) crr.slm_LogStatus = "R";
                    }
                }


                // create batch data
                ActivityItem itm = new ActivityItem();
                itm.ReferenceNo = data.ReferenceNo;
                itm.ActivityDateTime = data.TransactionDateTime;
                itm.ChannelID = data.ChannelId;

                itm.SubscriptionID = data.SubscriptionId;
                itm.SubscriptionTypeID = Convert.ToInt32(data.CIFSubscriptionTypeId);
                itm.LeadID = data.PreleadId;
                itm.TicketID = data.TicketId;
                itm.SrID = "";
                itm.ContractID = data.ContractNo;       //Added by Pom 16/06/2016
                itm.ProductGroupID = data.ProductGroupId;
                itm.ProductID = data.ProductId;
                itm.CampaignID = data.CampaignId;
                itm.ActivityTypeID = data.ActivityTypeId;                    // ** new 
                itm.ActivityTypeName = ""; // data.ActivityType;   // ** new
                itm.TypeID = data.TypeId;
                itm.TypeName = ""; // data.TypeName;
                itm.AreaID = data.AreaId;
                itm.AreaName = ""; // data.AreaName;
                itm.SubAreaID = data.SubAreaId;
                itm.SubAreaName = ""; // data.SubAreaName;

                itm.Status = data.Status;
                itm.SubStatus = data.SubStatus;

                itm.OfficerInfoList = data.OfficerInfoList;
                itm.ContractInfoList = data.ContractInfoList;
                itm.ProductInfoList = data.ProductInfoList;
                itm.CustomerInfoList = data.CustomerInfoList;
                itm.ActivityInfoList = data.ActivityInfoList;

                XmlSerializer xs = new XmlSerializer(typeof(ActivityItem));
                using (StringWriter sw = new StringWriter())
                {
                    xs.Serialize(sw, itm);
                    crr.slm_CasDetail = sw.ToString();
                }

                CARServiceBiz cbz = new CARServiceBiz();
                if (!cbz.SaveCarResend(crr, maxResend))
                {
                    throw new Exception(cbz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ret = false;
            }
            return ret;
        }

        public enum LogType
        {
            /// <summary>
            /// Error return from CAR Service
            /// </summary>
            Error,
            /// <summary>
            /// Fail to call Create Service
            /// 
            /// </summary>
            Fail
        }

        public class CARServiceData
        {
            public CARServiceData()
            {
                ActivityInfoList = new List<DataItem>();
                CustomerInfoList = new List<DataItem>();
                ProductInfoList = new List<DataItem>();
                ContractInfoList = new List<DataItem>();
                OfficerInfoList = new List<DataItem>();
            }

            public string ReferenceNo { get; set; }
            public string SecurityKey { get; set; }
            public string ServiceName { get; set; }
            public string SystemCode { get; set; }
            public DateTime TransactionDateTime { get; set; }
            public List<DataItem> ActivityInfoList { get; set; }
            public List<DataItem> CustomerInfoList { get; set; }
            public List<DataItem> ProductInfoList { get; set; }
            public List<DataItem> ContractInfoList { get; set; }
            public List<DataItem> OfficerInfoList { get; set; }
            public DateTime? ActivityDateTime { get; set; }
            public string CampaignId { get; set; }
            public string ChannelId { get; set; }
            public string PreleadId { get; set; }
            public string TicketId { get; set; }
            public string ProductGroupId { get; set; }
            public string ProductId { get; set; }
            public string Status { get; set; }
            public string SubStatus { get; set; }
            public string CIFSubscriptionTypeId { get; set; }
            public string SubscriptionId { get; set; }
            public decimal TypeId { get; set; }
            public decimal AreaId { get; set; }
            public decimal SubAreaId { get; set; }
            public decimal ActivityTypeId { get; set; }
            public string ContractNo { get; set; }
        }
        [Serializable]
        public class DataItem
        {
            public int SeqNo { get; set; }
            public string DataLabel { get; set; }
            public string DataValue { get; set; }
        }
        [Serializable]
        public class ActivityItem
        {
            public string ReferenceNo { get; set; }
            // required
            public DateTime ActivityDateTime { get; set; }
            public string ChannelID { get; set; }

            // required once
            public int SubscriptionTypeID { get; set; }
            public string SubscriptionID { get; set; }
            public string LeadID { get; set; }
            public string TicketID { get; set; }
            public string SrID { get; set; }
            public string ContractID { get; set; }

            // not required
            public string ProductGroupID { get; set; }
            public string ProductID { get; set; }
            public string CampaignID { get; set; }
            public decimal TypeID { get; set; }
            public decimal AreaID { get; set; }
            public decimal SubAreaID { get; set; }
            public decimal ActivityTypeID { get; set; }
            public string TypeName { get; set; }
            public string AreaName { get; set; }
            public string SubAreaName { get; set; }
            public string ActivityTypeName { get; set; }

            // Others
            public string KKCISID { get; set; }
            public string CISID { get; set; }
            public string TrxSeqID { get; set; }
            public string NoncustomerID { get; set; }
            public string Status { get; set; }
            public string SubStatus { get; set; }


            // list of data item

            public List<DataItem> OfficerInfoList { get; set; }
            public List<DataItem> ContractInfoList { get; set; }
            public List<DataItem> ProductInfoList { get; set; }
            public List<DataItem> CustomerInfoList { get; set; }
            public List<DataItem> ActivityInfoList { get; set; }
        }
        [Serializable]
        public class ActivityData
        {
            public string SystemCode { get; set; }
            public DateTime BatchDate { get; set; }
            public int TotalItems { get; set; }

            public List<ActivityItem> Items { get; set; }
        }
    }
}