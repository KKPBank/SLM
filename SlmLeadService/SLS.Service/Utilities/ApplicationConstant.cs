using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SLS.Service.Utilities
{
    public class ApplicationConstant
    {
        public const string LEAD_SERVICE_INSERT_NS = "www.kiatnakinbank.com/services/SlmLeadService/InsertLead";
        public const string LEAD_SERVICE_UPDATE_NS = "www.kiatnakinbank.com/services/SlmLeadService/UpdateLead";
        public const string LEAD_SERVICE_SEARCH_NS = "www.kiatnakinbank.com/services/SlmLeadService/SearchLead";
        public const string LEAD_SERVICE_VALIDATEPERMISSION_NS = "www.kiatnakinbank.com/services/SlmLeadService/ValidatePermission";

        public const string REQUEST_HEADER = "RequestHeader";
        public const string REQUEST_DETAIL = "RequestXml";
        public const string REQUEST_TICKETID = "RequestTicketId";

        public const string RESPONSE_HEADER = "ResponseHeader";
        public const string RESPONSE_STATUS = "ResponseStatus";

        public static int TextMaxLength
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["TextMaxLength"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["TextMaxLength"]) : 4000;
                }
                catch
                {
                    return 4000;
                }
            }
        }

        public static int DealerCodeMaxLength
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["DealerCodeMaxLength"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["DealerCodeMaxLength"]) : 50;
                }
                catch
                {
                    return 50;
                }
            }
        }

        public static int DealerNameMaxLength
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["DealerNameMaxLength"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["DealerNameMaxLength"]) : 1000;
                }
                catch
                {
                    return 1000;
                }
            }
        }

        //Added by Kug 15/01/2018
        public static string CBSLeadThaiCountryCode
        {
            get
            {
                try {
                    return ConfigurationManager.AppSettings["CBSLeadThaiCountryCode"] != null ? ConfigurationManager.AppSettings["CBSLeadThaiCountryCode"].ToString() : "TH";
                }
                catch
                {
                    return "TH";
                }
            }
        }


        //Added by Pom 02/06/2016
        public static class CARLogService
        {
            public static string CARLoginSLM
            {
                get { return ConfigurationManager.AppSettings["CARLoginSLM"] != null ? ConfigurationManager.AppSettings["CARLoginSLM"] : "SLM"; }
            }
            public static string SLMSecurityKey
            {
                get { return ConfigurationManager.AppSettings["CARSecurityKeySLM"] != null ? ConfigurationManager.AppSettings["CARSecurityKeySLM"] : ""; }
            }
            public static string CARLoginOBT
            {
                get { return ConfigurationManager.AppSettings["CARLoginOBT"] != null ? ConfigurationManager.AppSettings["CARLoginOBT"] : "OBT"; }
            }
            public static string OBTSecurityKey
            {
                get { return ConfigurationManager.AppSettings["CARSecurityKeyOBT"] != null ? ConfigurationManager.AppSettings["CARSecurityKeyOBT"] : ""; }
            }
            //public static string CARBatchFolder
            //{
            //    get { return ConfigurationManager.AppSettings["CARBatchFolder"] != null ? ConfigurationManager.AppSettings["CARBatchFolder"] : ""; }
            //}
            public static string CARCreateActivityLogServiceName
            {
                get { return ConfigurationManager.AppSettings["CARCreateActivityLogServiceName"] != null ? ConfigurationManager.AppSettings["CARCreateActivityLogServiceName"] : ""; }
            }
            public static string CARErrorCodeResend
            {
                get { return ConfigurationManager.AppSettings["CARErrorCodeResend"] != null ? ConfigurationManager.AppSettings["CARErrorCodeResend"] : ""; }
            }
            public static int CARMaxResend
            {
                get
                {
                    try { return ConfigurationManager.AppSettings["CARMaxResend"] != null ? int.Parse(ConfigurationManager.AppSettings["CARMaxResend"]) : 0; }
                    catch { return 0; }
                }
            }
            public static int CARTimeout
            {
                get
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["CARTimeout"] != null ? int.Parse(ConfigurationManager.AppSettings["CARTimeout"]) : 10;
                    }
                    catch
                    {
                        return 10;  //seconds
                    }
                }
            }

            public static class Data
            {
                //public const string Type = "Sales";
                //public const string Area = "ผลิตภัณฑ์/บริการ/โปรโมชั่น";
                //public const string SubArea = "การขาย";

                public static decimal TypeId
                {
                    get
                    {
                        try { return ConfigurationManager.AppSettings["CARTypeId"] != null ? decimal.Parse(ConfigurationManager.AppSettings["CARTypeId"]) : 0; }
                        catch { return 0; }
                    }
                }
                public static decimal AreaId
                {
                    get
                    {
                        try { return ConfigurationManager.AppSettings["CARAreaId"] != null ? decimal.Parse(ConfigurationManager.AppSettings["CARAreaId"]) : 0; }
                        catch { return 0; }
                    }
                }
                public static decimal SubAreaId
                {
                    get
                    {
                        try { return ConfigurationManager.AppSettings["CARSubAreaId"] != null ? decimal.Parse(ConfigurationManager.AppSettings["CARSubAreaId"]) : 0; }
                        catch { return 0; }
                    }
                }

                public static class ActivityType
                {
                    //public const string Todo = "Todo";
                    //public const string SMS = "SMS Sending";
                    //public const string FYI = "FYI";
                    //public const string CallOutbound = "Call Outbound";
                    //public const string CallInbound = "Call Inbound";

                    public static decimal FYIId
                    {
                        get
                        {
                            try { return ConfigurationManager.AppSettings["CARActivityTypeFYIId"] != null ? decimal.Parse(ConfigurationManager.AppSettings["CARActivityTypeFYIId"]) : 0; }
                            catch { return 0; }
                        }
                    }
                }
            }
        }
    }
}