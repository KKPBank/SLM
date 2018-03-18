using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Data;
using System.Reflection;
using System.Net.Mail;
using System.Net;
using SpreadsheetLight;

namespace SLMBatch.Common
{
    public class AppConstant
    {
        public const string InProcess = "0";
        public const string Success = "1";
        public const string Fail = "2";

        public enum PendingType
        {
            DuplicateTicketId = 1,          //มี Ticket มากกว่า 1 รายการ
            ContractNoInPreleadOnly = 2,    //มีเลขที่สัญญาใน Prelead แต่ไม่มีใน Lead
            InappropriateLeadStatus = 3,    //มี Ticket แต่สถานะเป็น Cancel, Reject, Success
            PurchaseDetailNotFound = 4,     //มี Ticket แต่ไม่มี Flag การซื้อประกันและพรบ.
            ContractNoNotFound = 5,         //เลขที่สัญญาไม่มีใน Prelead และ Lead
            ConfigPaymentNotFound = 6,      //ไม่มี Config ใน Table kkslm_ms_config_product_payment
            ReceiveNoNotFound = 7           //มี Ticket แต่ไม่มีเลขที่รับแจ้ง
        }

        private static string ConfigLookupString(string key, string _default = "")
        {
            // this function allow ""
            return ConfigurationManager.AppSettings[key] ?? _default;
        }

        private static string ConfigLookupStringHateEmpty(string key, string _default)
        {
            // this function consider config value "" invalid
            var cfg = ConfigurationManager.AppSettings[key];
            return string.IsNullOrEmpty(cfg) ? _default : cfg;
        }

        private static int ConfigLookupInt32(string key, int _default)
        {
            var cfg = ConfigurationManager.AppSettings[key];
            int result;
            return string.IsNullOrEmpty(cfg) ? _default : (int.TryParse(cfg, out result) ? result : _default);
        }

        private static bool ConfigLookupBoolean(string key, bool _default)
        {
            var cfg = ConfigurationManager.AppSettings[key];
            bool result;
            if (string.IsNullOrEmpty(cfg))
            {
                result = _default;
            }
            else
            {
                if (bool.TryParse(cfg, out result))
                {
                    // good, proceed with parsed result
                }
                else
                {
                    switch (cfg)
                    {
                        case "0":
                            result = false;
                            break;
                        case "1":
                            result = true;
                            break;
                        default:
                            result = _default;
                            break;
                    }
                }
            }
            return result;
        }

        public static string SLMDBName
        {
            get { return ConfigLookupString("SLMDBName", "SLMDB"); }
        }

        public static string OPERDBName
        {
            get { return ConfigLookupString("OPERDBName", "OPERDB"); }
        }

        public static int HPAOLNumOfDay
        {
            get { return ConfigLookupInt32("HPAOLNumOfDay", 0); }
        }

        public static int DWHNumOfDay
        {
            get { return ConfigLookupInt32("DWHNumOfDay", 1); }
        }

        public static int CommandTimeout
        {
            get { return ConfigLookupInt32("CommandTimeout", 0); }
        }

        [Obsolete("use specific sms template config instead", true)]
        public static string SMSTemplatePath
        {
            get { return ConfigLookupString("SMSTemplatePath"); }
        }

        #region SMS

        public static string SMSTemplatePathPolicyNo => ConfigLookupString("SMSTemplatePathPolicyNo");
        public static string SMSTemplatePathReceiveNo => ConfigLookupString("SMSTemplatePathReceiveNo");
        public static string SMSTemplatePathPaymentDueLong => ConfigLookupString("SMSTemplatePathPaymentDueLong");
        public static string SMSTemplatePathPaymentDueShort => ConfigLookupString("SMSTemplatePathPaymentDueShort");
        public static int SMSPaymentDueLongNotiDay => ConfigLookupInt32("SMSPaymentDueLongNotiDay", 70);
        public static int SMSPaymentDueShortNotiDay => ConfigLookupInt32("SMSPaymentDueShortNotiDay", 8);
        public static bool SendSMSPolicyNo
        {
            get
            {
                string config = ConfigurationManager.AppSettings["SendSMSPolicyNo"];
                if (string.IsNullOrWhiteSpace(config))
                {
                    return true;
                }
                else
                {
                    return config.ToUpper().Trim() == "Y" ? true : false;
                }
            }
        }

        #endregion

        public static string EmailTemplatePaymentPath
        {
            get { return ConfigLookupString("EmailTemplatePaymentPath"); }
        }

        public static string DWHConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DWHConnectionString"].ConnectionString; }
        }

        public static string DWHSchema
        {
            get { return ConfigLookupString("DWHSchema", "DWHINF"); }
        }

        public static string HPConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["HPConnectionString"].ConnectionString; }
        }

        public static int PurgeNotificationNumOfDay
        {
            get { return ConfigLookupInt32("PurgeNotificationNumOfDay", 14); }
        }

        public static string ReadBatchDateFromConfig
        {
            get { return ConfigLookupString("ReadBatchDateFromConfig"); }
        }

        public static string BatchDate
        {
            get { return ConfigLookupString("BatchDate"); }
        }

        public static int Vat
        {
            get { return ConfigLookupInt32("Vat", 7); }
        }

        #region Email Config Common

        public static string EmailDisplayName
        {
            get { return ConfigLookupString("EmailDisplayName"); }
        }

        public static string EmailFromAddress
        {
            get { return ConfigLookupString("EmailFromAddress"); }
        }

        public static string EmailToAddress
        {
            get { return ConfigLookupString("EmailToAddress"); }
        }

        public static string EmailFromPassword
        {
            get { return ConfigLookupString("EmailFromPassword"); }
        }

        public static string EmailHostIP
        {
            get { return ConfigLookupString("EmailHostIP"); }
        }

        public static int EmailPort
        {
            get { return ConfigLookupInt32("EmailPort", 25); }
        }

        #endregion

        #region ExportConfigPath

        public static string ExportPath
        {
            get { return ConfigLookupString("ExportPath"); }
        }

        public static string ExportDomainName
        {
            get { return ConfigLookupString("ExportDomainName"); }
        }

        public static string ExportUsername
        {
            get { return ConfigLookupString("ExportUsername"); }
        }

        public static string ExportPassword
        {
            get { return ConfigLookupString("ExportPassword"); }
        }

        public static string MaxExcelRowPerFile
        {
            get { return ConfigLookupString("MaxExcelRowPerFile"); }
        }

        #endregion

        #region SLM_PRO_06 : ExportWSLog

        public static string WSLogEmailSubjectError
        {
            get { return ConfigLookupString("WSLogEmailSubjectError"); }
        }

        public static string WSLogEmailSubjectComplete
        {
            get { return ConfigLookupString("WSLogEmailSubjectComplete"); }
        }

        public static string WSLogEmailDisplayName
        {
            get { return ConfigLookupString("WSLogEmailDisplayName"); }
        }

        public static string WSLogEmailFromAddress
        {
            get { return ConfigLookupString("WSLogEmailFromAddress"); }
        }

        public static string WSLogEmailToAddress
        {
            get { return ConfigLookupString("WSLogEmailToAddress"); }
        }

        public static string WSLogEmailFromPassword
        {
            get { return ConfigLookupString("WSLogEmailFromPassword"); }
        }

        public static string WSLogEmailHostIP
        {
            get { return ConfigLookupString("WSLogEmailHostIP"); }
        }

        public static int WSLogEmailPort
        {
            get { return ConfigLookupInt32("WSLogEmailPort", 25); }
        }

        public static string WSLogEmailPurposeComplete
        {
            get { return ConfigLookupString("WSLogEmailPurposeComplete"); }
        }

        public static string WSLogEmailPurposeError
        {
            get { return ConfigLookupString("WSLogEmailPurposeError"); }
        }

        public static string ExportWSLogPath
        {
            get { return ConfigLookupString("ExportWSLogPath"); }
        }

        public static string ExportWSLogDomainName
        {
            get { return ConfigLookupString("ExportWSLogDomainName"); }
        }

        public static string ExportWSLogUsername
        {
            get { return ConfigLookupString("ExportWSLogUsername"); }
        }

        public static string ExportWSLogPassword
        {
            get { return ConfigLookupString("ExportWSLogPassword"); }
        }

        #endregion

        #region OBT_PRO_26 : ExportExcelPaymentPending

        public class PaymentPending : BatchExportConfigSet
        {
            public static EmailConfigSet Email { get; } = new EmailConfigSet
            {
                SubjectComplete = ConfigLookupString("PaymentPendingEmailSubjectComplete"),
                SubjectError = ConfigLookupString("PaymentPendingEmailSubjectError"),
                PurposeComplete = ConfigLookupString("PaymentPendingEmailPurposeComplete"),
                PurposeError = ConfigLookupString("PaymentPendingEmailPurposeError")
            };

            public static ExportConfigSet Export { get; } = new ExportConfigSet
            {
                Path = ConfigLookupStringHateEmpty("PaymentPendingExportPath", ExportPath),
                DomainName = ConfigLookupStringHateEmpty("PaymentPendingExportDomainName", ExportDomainName),
                Username = ConfigLookupStringHateEmpty("PaymentPendingExportUsername", ExportUsername),
                Password = ConfigLookupStringHateEmpty("PaymentPendingExportPassword", ExportPassword)
            };
        }

        #endregion

        #region OBT_PRO_29 : ExportExcel PolicyNo and ActNo Pending

        public class PolicyNoActNoPending : BatchExportConfigSet
        {
            public static EmailConfigSet Email { get; } = new EmailConfigSet
            {
                SubjectComplete = ConfigLookupString("PolicyNoActNoPendingEmailSubjectComplete"),
                SubjectError = ConfigLookupString("PolicyNoActNoPendingEmailSubjectError"),
                PurposeComplete = ConfigLookupString("PolicyNoActNoPendingEmailPurposeComplete"),
                PurposeError = ConfigLookupString("PolicyNoActNoPendingEmailPurposeError")
            };

            public static ExportConfigSet Export { get; } = new ExportConfigSet
            {
                Path = ConfigLookupStringHateEmpty("PolicyNoActNoPendingExportPath", ExportPath),
                DomainName = ConfigLookupStringHateEmpty("PolicyNoActNoPendingExportDomainName", ExportDomainName),
                Username = ConfigLookupStringHateEmpty("PolicyNoActNoPendingExportUsername", ExportUsername),
                Password = ConfigLookupStringHateEmpty("PolicyNoActNoPendingExportPassword", ExportPassword)
            };
        }

        #endregion

        #region OBT_PRO_30 : ExportExcel Leads For Transfer

        public class LeadsForTransfer : BatchExportConfigSet
        {
            public static EmailConfigSet Email { get; } = new EmailConfigSet
            {
                SubjectComplete = ConfigLookupString("LeadsForTransferEmailSubjectComplete"),
                SubjectError = ConfigLookupString("LeadsForTransferEmailSubjectError"),
                PurposeComplete = ConfigLookupString("LeadsForTransferEmailPurposeComplete"),
                PurposeError = ConfigLookupString("LeadsForTransferEmailPurposeError")
            };

            public static ExportConfigSet Export { get; } = new ExportConfigSet
            {
                Path = ConfigLookupStringHateEmpty("LeadsForTransferExportPath", ExportPath),
                DomainName = ConfigLookupStringHateEmpty("LeadsForTransferExportDomainName", ExportDomainName),
                Username = ConfigLookupStringHateEmpty("LeadsForTransferExportUsername", ExportUsername),
                Password = ConfigLookupStringHateEmpty("LeadsForTransferExportPassword", ExportPassword)
            };

            public static int ExpNotiDay
            {
                get { return ConfigLookupInt32("LeadsForTransferExpNotiDay", 8); }
            }

            public static int ResponsibleRoleNumber
            {
                get { return ConfigLookupInt32("LeadsForTransferResponsibleRoleNumber", 19); }
            }
        }

        #endregion

        #region OBT_PRO_31 : ExportExcel Leads For TKS

        public class LeadsForTKS : BatchExportConfigSet
        {
            public static EmailConfigSet Email { get; } = new EmailConfigSet
            {
                SubjectComplete = ConfigLookupString("LeadsForTKSEmailSubjectComplete"),
                SubjectError = ConfigLookupString("LeadsForTKSEmailSubjectError"),
                PurposeComplete = ConfigLookupString("LeadsForTKSEmailPurposeComplete"),
                PurposeError = ConfigLookupString("LeadsForTKSEmailPurposeError")
            };

            public static ExportConfigSet Export { get; } = new ExportConfigSet
            {
                Path = ConfigLookupStringHateEmpty("LeadsForTKSExportPath", ExportPath),
                DomainName = ConfigLookupStringHateEmpty("LeadsForTKSExportDomainName", ExportDomainName),
                Username = ConfigLookupStringHateEmpty("LeadsForTKSExportUsername", ExportUsername),
                Password = ConfigLookupStringHateEmpty("LeadsForTKSExportPassword", ExportPassword)
            };
        }

        #endregion

        #region OBT_PRO_08 : Export Excel

        public static string ExcelSharePath
        {
            get { return ConfigLookupString("ExcelSharePath"); }
        }

        public static string ExcelSharePathDomainName
        {
            get { return ConfigLookupString("ExcelSharePathDomainName"); }
        }

        public static string ExcelSharePathUsername
        {
            get { return ConfigLookupString("ExcelSharePathUsername"); }
        }

        public static string ExcelSharePathPassword
        {
            get { return ConfigLookupString("ExcelSharePathPassword"); }
        }

        #endregion

        #region SLM_PRO_02 : ExportCSV

        public static string SLM_PRO_02_PATH
        {
            get { return ConfigLookupString("SLM_PRO_02_PATH"); }
        }

        public static string SLM_PRO_02_DOMAIN
        {
            get { return ConfigLookupString("SLM_PRO_02_DOMAIN"); }
        }

        public static string SLM_PRO_02_USERNAME
        {
            get { return ConfigLookupString("SLM_PRO_02_USERNAME"); }
        }

        public static string SLM_PRO_02_PASSWORD
        {
            get { return ConfigLookupString("SLM_PRO_02_PASSWORD"); }
        }
        public static string SLM_PRO_02_ENCODE
        {
            get { return ConfigLookupString("SLM_PRO_02_ENCODE"); }
        }

        #endregion

        #region SLM_PRO_08 : BatchCARInsertStatus
        public static string SLM_PRO_08_PathImport
        {
            get {
                if (Directory.Exists(ConfigLookupString("SLM_PRO_08_PathImport")) == false)
                {
                    Directory.CreateDirectory(ConfigLookupString("SLM_PRO_08_PathImport"));
                }
                return ConfigLookupString("SLM_PRO_08_PathImport");
            }
        }
        public static string SLM_PRO_08_PathArchives
        {
            get {
                if (Directory.Exists(ConfigLookupString("SLM_PRO_08_PathArchives")) == false)
                {
                    Directory.CreateDirectory(ConfigLookupString("SLM_PRO_08_PathArchives"));
                }

                return ConfigLookupString("SLM_PRO_08_PathArchives");
            }
        }
        public static string SLM_PRO_08_File_Prefix
        {
            get { return ConfigLookupString("SLM_PRO_08_File_Prefix", "BchCARInsertSts_"); }
        }
        public static int SLM_PRO_08_IntervalDay
        {
            get { return ConfigLookupInt32("SLM_PRO_08_IntervalDay", 1); }
        }
        public static string SLM_PRO_08_SSH_Server
        {
            get { return ConfigLookupString("SLM_PRO_08_SSH_Server"); }
        }
        public static int SLM_PRO_08_SSH_Port
        {
            get { return ConfigLookupInt32("SLM_PRO_08_SSH_Port", 22); }
        }
        public static string SLM_PRO_08_SSH_Username
        {
            get { return ConfigLookupString("SLM_PRO_08_SSH_Username"); }
        }
        public static string SLM_PRO_08_SSH_Password
        {
            get { return ConfigLookupString("SLM_PRO_08_SSH_Password"); }
        }
        public static string SLM_PRO_08_SSH_RemoteDir
        {
            get { return ConfigLookupString("SLM_PRO_08_SSH_RemoteDir"); }
        }

        public class BatchCARInsertStatus
        {
            public const string  TYPE_OF_DATA_HEAD = "H";
            public const string TYPE_OF_DATA_DETAIL = "D";
        }

        public static string SLM_PRO_08_EmailDisplayName
        {
            get { return ConfigLookupString("SLM_PRO_08_EmailDisplayName"); }
        }

        public static string SLM_PRO_08_EmailFromAddress
        {
            get { return ConfigLookupString("SLM_PRO_08_EmailFromAddress"); }
        }

        public static string SLM_PRO_08_EmailToAddress
        {
            get { return ConfigLookupString("SLM_PRO_08_EmailToAddress"); }
        }

        public static string SLM_PRO_08_EmailFromPassword
        {
            get { return ConfigLookupString("SLM_PRO_08_EmailFromPassword"); }
        }

        public static string SLM_PRO_08_EmailHostIP
        {
            get { return ConfigLookupString("SLM_PRO_08_EmailHostIP"); }
        }

        public static int SLM_PRO_08_EmailPort
        {
            get { return ConfigLookupInt32("SLM_PRO_08_EmailPort", 25); }
        }
        #endregion

        public static string OBT_PRO_32_LogSwitch
        {
            get { return ConfigLookupStringHateEmpty("OBT_PRO_32_LogSwitch", "Y"); }
        }

        public class DateTimeFormat
        {
            /// <summary>
            /// YYYYMMDD
            /// </summary>
            public const string Format1 = "YYYYMMDD";

            /// <summary>
            /// YYYY/MM/DD
            /// </summary>
            public const string Format2 = "YYYY/MM/DD";
        }

        public class PreleadAssignType
        {
            public const string Normal = "NM";
            public const string Blacklist = "BL";
            public const string BlacklistForce = "BF";
            public const string Dedup = "DD";
            public const string DedupForce = "DF";
            public const string Exception = "EX";
            public const string SystemAssign = "SA";
            public const string UserAssign = "UA";
            public const string ConfirmPending = "CP";
            public const string ConfirmSuccess = "CS";
            public const string ConfirmBlacklist = "CB";
            public const string ConfirmDedup = "CD";
            public const string ConfirmException = "CE";
        }

        public class PaymentCode
        {
            public const string Policy = "204";
            public const string Act = "205";
        }

        public static class LoggingType
        {
            public const string SystemAssign = "01";
            public const string ChangeStatus = "02";
            public const string Delegate = "03";
            public const string Transfer = "04";
            public const string UserAssign = "05";
            public const string Consolidate = "06";
            public const string ResetOwner = "07";
            public const string UpdateOwner = "08";
            public const string EODUpdateCurrent = "09";
            public const string ChangeOwner = "10";
            public const string EODHistoryLogs = "11";
            public const string UserError = "12";
            public const string RepairStatus = "13";
            public const string GetIncentiveIns = "14";
            public const string GetIncentiveAct = "15";
            public const string RevertIncentiveIns = "16";
            public const string RevertIncentiveAct = "17";
            public const string CancelSuspendedClaim = "20";    //ยกเลิกระงับเคลม
        }

        //Added By Pom 03/06/2016
        public static class CARLogService
        {
            public static string CARLoginSLM
            {
                get { return ConfigLookupString("CARLoginSLM", ""); }
            }

            public static string CARLoginOBT
            {
                get { return ConfigLookupString("CARLoginOBT", ""); }
            }

            public static string SLMSecurityKey
            {
                get { return ConfigLookupString("CARSecurityKeySLM", ""); }
            }

            public static string OBTSecurityKey
            {
                get { return ConfigLookupString("CARSecurityKeyOBT", ""); }
            }

            //public static string CARBatchFolder
            //{
            //    get { return ConfigurationManager.AppSettings["CARBatchFolder"] != null ? ConfigurationManager.AppSettings["CARBatchFolder"] : ""; }
            //}
            public static string CARCreateActivityLogServiceName
            {
                get { return ConfigLookupString("CARCreateActivityLogServiceName", ""); }
            }

            public static string CARErrorCodeResend
            {
                get { return ConfigLookupString("CARErrorCodeResend", ""); }
            }

            public static int CARMaxResend
            {
                get { return ConfigLookupInt32("CARMaxResend", 0); }
            }

            public static int CARTimeout
            {
                get
                {
                    return ConfigLookupInt32("CARTimeout", 10); // seconds
                }
            }

            public static class Data
            {
                //public const string Type = "Sales";
                //public const string Area = "ผลิตภัณฑ์/บริการ/โปรโมชั่น";
                //public const string SubArea = "การขาย";

                public static decimal TypeId
                {
                    get { return ConfigLookupInt32("CARTypeId", 0); }
                }

                public static decimal AreaId
                {
                    get { return ConfigLookupInt32("CARAreaId", 0); }
                }

                public static decimal SubAreaId
                {
                    get { return ConfigLookupInt32("CARSubAreaId", 0); }
                }

                public static class ActivityType
                {
                    //public const string Todo = "Todo";
                    //public const string SMS = "SMS Sending";
                    //public const string FYI = "FYI";
                    //public const string CallOutbound = "CallOutbound";
                    //public const string CallInbound = "CallInbound";

                    public static decimal TodoId
                    {
                        get { return ConfigLookupInt32("CARActivityTypeToDoId", 0); }
                    }
                }
            }
        }
    }

    public class Util
    {
        public enum eExportModule
        {
            PaymentPending = 0,
            PolicyNoActNoPending,
            LeadsForTransfer,
            LeadsForTKS
        }

        public static void WriteLogFile(string logfilename, string batchCode, string errMsg)
        {
            StreamWriter sw = null;
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), batchCode);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (logfilename == "") logfilename = "TempLog.txt";

                sw = new StreamWriter(Path.Combine(path, logfilename), true);
                sw.WriteLine(errMsg);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            catch
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static void SendMail(string MailSubject, string MailBody)
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            MailAddress fromAddress = new MailAddress(AppConstant.EmailFromAddress, AppConstant.EmailDisplayName.Trim() ?? "");

            client.Host = AppConstant.EmailHostIP;
            client.Port = AppConstant.EmailPort;
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(AppConstant.EmailFromAddress, AppConstant.EmailFromPassword);
            client.Timeout = 300000;

            foreach (string mailto in AppConstant.EmailToAddress.Split(';'))
            {
                if (!string.IsNullOrWhiteSpace(mailto?.Trim()))
                {
                    mail.To.Add(new MailAddress(mailto.Trim()));
                }
            }

            mail.From = fromAddress;
            mail.Subject = MailSubject;
            mail.IsBodyHtml = true;
            mail.Body = MailBody;

            client.Send(mail);

            mail = null;
            client = null;
            fromAddress = null;
        }

        public static void SaveExportExcel(eExportModule ExportModule, BatchExportConfigSet.ExportConfigSet exportConfig, string ExcelTemplateFileName, DataTable DataExport, int ExcelStartRow, string ExcelStartColumn, Action<SLDocument> styleExcel = null)
        {
            MemoryStream TemplateStream = new System.IO.MemoryStream();
            SLDocument report = null;
            try
            {
                string strDateTimeFileName = DateTime.Now.ToString("yyyyMMdd_HHmm");
                string strFileName = string.Empty;
                string tmpPath = string.Format("{0}\\Template\\Excel\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, ExcelTemplateFileName);
                Console.WriteLine(string.Format("Read {0} template file", ExportModule));
                using (var TemplateBase = new System.IO.FileStream(tmpPath, System.IO.FileMode.Open))
                {
                    TemplateBase.CopyTo(TemplateStream);
                    TemplateBase.Close();
                    TemplateStream.Seek(0, SeekOrigin.Begin);
                }

                Console.WriteLine(string.Format("Prepare {0} data", ExportModule));

                int iAllExcelRow = DataExport.Rows.Count;
                int iMaxRowPerFile = Convert.ToInt32(AppConstant.MaxExcelRowPerFile);
                int iNoOfExcelFile = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(iAllExcelRow) / Convert.ToDouble(iMaxRowPerFile)));

                Console.WriteLine(string.Format("All Excel row(s) : {0}", iAllExcelRow));
                Console.WriteLine(string.Format("Max Row per file : {0}", iMaxRowPerFile));
                Console.WriteLine(string.Format("Number of Excel file : {0}", iNoOfExcelFile));

                for (int i = 0; i < iNoOfExcelFile; i++)
                {
                    if (iNoOfExcelFile == 1)
                    {
                        if (ExportModule == eExportModule.LeadsForTKS)
                        {
                            strFileName = string.Format("KK_INS_T1_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
                        }
                        else
                        {
                            strFileName = string.Format("{0}_{1}.xlsx", ExportModule, strDateTimeFileName);
                        }
                    }
                    else
                    {
                        if (ExportModule == eExportModule.LeadsForTKS)
                        {
                            strFileName = string.Format("KK_INS_T1_{0}_{1}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"), i + 1);
                        }
                        else
                        {
                            strFileName = string.Format("{0}_{1}_{2}.xlsx", ExportModule, strDateTimeFileName, i + 1);
                        }  
                    }

                    DataTable dt = DataExport.AsEnumerable().Select(x => x).Skip(i * iMaxRowPerFile).Take(iMaxRowPerFile).CopyToDataTable();

                    report = new SLDocument(TemplateStream, ExportModule.ToString());
                    report.InsertRow(ExcelStartRow, dt.Rows.Count - 1);
                    report.ImportDataTable(ExcelStartColumn + ExcelStartRow, dt, false);

                    styleExcel?.Invoke(report);

                    Console.WriteLine(string.Format("Save {0} file", ExportModule));
                    using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                    {
                        string exportPath = exportConfig.Path;
                        string exportUsername = exportConfig.Username;
                        string exportDomainName = exportConfig.DomainName;
                        string exportPassword = exportConfig.Password;
                        if (unc.NetUseWithCredentials(exportPath, exportUsername, exportDomainName, exportPassword))
                        {
                            report.SaveAs(string.Format("{0}{1}", exportPath, strFileName));
                        }
                        else
                        {
                            string strError = string.Format("Cannot access path '{0}'", exportPath);
                            Console.WriteLine(strError);
                            unc.Dispose();
                            throw new Exception(strError);
                        }
                        unc.Dispose();
                    }

                    report.Dispose();
                    report = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (report != null)
                {
                    report.Dispose();
                }
                report = null;
                TemplateStream = null;
            }
        }
    }

    public static class ExtensionMethods
    {
        [Obsolete("rendered useless by null-conditional operator introduced in c# 6.0", true)]
        public static string TrimIfNotNull(this string value)
        {
            if (value != null)
            {
                return value.Trim();
            }
            return null;
        }
    }
}
