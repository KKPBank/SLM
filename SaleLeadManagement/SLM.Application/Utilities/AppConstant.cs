using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SLM.Application.Utilities
{
    public static class AppConstant
    {
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

        public static int MaximumImageUploadSize
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["MaximumImageUploadSize"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["MaximumImageUploadSize"]) : 5242880;
                }
                catch
                {
                    return 5242880; //5MB
                }
            }
        }

        public static int MaximumFileUploadSize
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["MaximumFileUploadSize"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["MaximumFileUploadSize"]) : 5242880;
                }
                catch
                {
                    return 5242880; //5MB
                }
            }
        }

        public static int MaximumEcmFileUploadSize
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["EcmFileUploadSize"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["EcmFileUploadSize"]) : 3145728;
                }
                catch
                {
                    return 3145728; //3MB
                }
            }
        }

        public static string NoticeFolder
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["NoticeFolder"] != null ? ConfigurationManager.AppSettings["NoticeFolder"] : "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public static int CMTTimeout
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CMTTimeout"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["CMTTimeout"]) : 10;
                }
                catch
                {
                    return 10;
                }
            }
        }

        public static class Campaign
        {
            public static int DisplayCampaignDescMaxLength
            {
                get 
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["DisplayCampaignDescMaxLength"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["DisplayCampaignDescMaxLength"]) : 100;
                    }
                    catch { return 100; }
                }
            }
        }

        public static class CardType
        {
            public const string Person = "1";
            public const string JuristicPerson = "2";
            public const string Foreigner = "3";
        }

        public static class OptionType
        {
            public const string LeadStatus = "lead status";
            public const string ObtStatus = "obt status";
        }

        public static bool CSMServiceEnableSyncUser
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CSMServiceEnableSyncUser"] == "1";
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool CSMServiceEnableSyncBranch
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CSMServiceEnableSyncBranch"] == "1";
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool CSMServiceEnableSyncCalendar
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CSMServiceEnableSyncCalendar"] == "1";
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static string CSMEncryptPassword
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CSMEncryptPassword"];
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static string GetCMTProductType
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CMTProductType"];
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static string GetCSMServiceName
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CSMServiceName"];
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static string GetCSMUsername
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CSMUsername"];
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static string GetCSMSystemCode
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CSMSystemCode"];
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static string GetCSMPassword
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CSMPassword"];
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static string UploadFileType
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["UploadFileType"] != null ? ConfigurationManager.AppSettings["UploadFileType"] : "xls,xlsx";
                }
                catch
                {
                    return "xls,xlsx";
                }
            }
        }


        public static int CBSTimeout
        {
            //milisecond
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CBSTimeout"] != null ? Convert.ToInt16(ConfigurationManager.AppSettings["CBSTimeout"]) * 1000 : 10000;
                }
                catch
                {
                    return 10000;
                }
            }
        }
        public static int CBSPageSize
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CBSPageSize"] != null ? Convert.ToInt16(ConfigurationManager.AppSettings["CBSPageSize"]) : 100;
                }
                catch
                {
                    return 100;
                }
            }
        }
        public static string CBSInquiryCustomerSingleView
        {
            get
            {
                try {
                    return ConfigurationManager.AppSettings["CBSInquiryCustomerSingleView"] != null ? ConfigurationManager.AppSettings["CBSInquiryCustomerSingleView"].ToString() : "InquiryCustomerSingleView";
                }
                catch
                {
                    return "InquiryCustomerSingleView";
                }
            }
        }
        public static string CBSSystemCode
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CBSSystemCode"] != null ? ConfigurationManager.AppSettings["CBSSystemCode"].ToString() : "SLM";
                }
                catch
                {
                    return "SLM";
                }
            }
        }
        public static string CBSChannelID
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["CBSChannelID"] != null ? ConfigurationManager.AppSettings["CBSChannelID"].ToString() : "SLM";
                }
                catch
                {
                    return "SLM";
                }
            }
        }

        public static int CBSLeadThaiCountryId
        {
            get {
                try
                {
                    return ConfigurationManager.AppSettings["CBSLeadThaiCountryId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["CBSLeadThaiCountryId"]) : 220;
                }
                catch
                {
                    return 220;
                }
            }
        }

    }
}