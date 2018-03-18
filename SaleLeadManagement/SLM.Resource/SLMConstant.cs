using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource
{
    public class SLMConstant
    {
        public const string SystemName = "SLM";
        public const string SystemNameOBT = "OBT";
        public const string NextLeadList = "nextleadlist";
        public const string FollowupLeadList = "FollowupLeadList";
        public const string InboundLeadList = "InboundLeadList";

        public static string SLMDBName
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["SLMDBName"] != null ? ConfigurationManager.AppSettings["SLMDBName"] : "SLMDB";
                }
                catch
                {
                    return "SLMDB";
                }
            }
        }

        public static string OPERDBName
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["OPERDBName"] != null ? ConfigurationManager.AppSettings["OPERDBName"] : "OPERDB";
                }
                catch
                {
                    return "OPERDB";
                }
            }
        }

        public const string SessionExport = "exportdata";

        //Add 19/4/2016
        public static int NotificationInterval
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["NotificationInterval"] != null ? (int.Parse(ConfigurationManager.AppSettings["NotificationInterval"]) * 1000) : 300000;
                }
                catch
                {
                    return 300000;  //5 minutes
                }
            }
        }

        public static int VatRate
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["VatRate"] != null ? (int.Parse(ConfigurationManager.AppSettings["VatRate"])) : 7;
                }
                catch
                {
                    return 7;
                }
            }
        }
        public static int SLMDBCommandTimeout
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["SLMDBCommandTimeout"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["SLMDBCommandTimeout"]) : 0;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static int GridviewPageSize
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["GridviewPageSize"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["GridviewPageSize"]) : 10;
                }
                catch
                {
                    return 10;
                }
            }
        }

        public static int GridviewPageSizeUploadLead
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["GridviewPageSizeUploadLead"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["GridviewPageSizeUploadLead"]) : 20;
                }
                catch
                {
                    return 20;
                }
            }
        }

        public static int ExcelRowPerSheet
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ExcelRowPerSheet"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["ExcelRowPerSheet"]) : 50000;
                }
                catch
                {
                    return 50000;
                }
            }
        }

        public static string TelNoLog
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["TelNoLog"] != null ? ConfigurationManager.AppSettings["TelNoLog"] : "Y";
                }
                catch
                {
                    return "Y";
                }
            }
        }

        public static string ActPurchaseTime
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ActPurchaseTime"] != null ? ConfigurationManager.AppSettings["ActPurchaseTime"] : "16:30";
                }
                catch
                {
                    return "16:30";
                }
            }
        }

        public static int ExcelMaxRowNotifyPremium
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ExcelMaxRowNotifyPremium"] != null ? int.Parse(ConfigurationManager.AppSettings["ExcelMaxRowNotifyPremium"]) : 100;
                }
                catch
                {
                    return 100;
                }
            }
        }

        public static class StaffType
        {
            public const decimal Manager = 1;
            public const decimal Supervisor = 2;
            public const decimal UserAdministrator = 3;
            public const decimal Telesales = 4;
            public const decimal CallCenter = 5;
            public const decimal Leader = 6;
            public const decimal ITAdministrator = 7;
            public const decimal Marketing = 8;
            public const decimal ManagerOper = 10;
            public const decimal SupervisorOper = 11;
            public const decimal Oper = 12;
            public const decimal SupervisorTelesalesOutbound = 15;
            public const decimal OperOutbound = 16;
            public const decimal ProductOutbound = 17;
            public const decimal ManagerOutbound = 18;
            public const decimal CallReminder = 19;
            public const decimal NormalExpired = 20;
        }
        public static class StatusCode
        {
            public const string Interest = "00";        //สนใจ
            public const string NoContact = "01";       //ติดต่อไม่ได้ >>> ยกเลิก...ให้ใช้ 15 แทน
            public const string ContactDoc = "02";      //ติดต่อได้ รอเอกสาร >>>ยกเลิก...ให้ใช้ 14 แทน
            public const string ContactCall = "03";     //ติดต่อได้ใ ห้โทรกลับ >>>ยกเลิก...ให้ใช้ 14 แทน
            public const string FollowDoc = "04";       //ติดตามเอกสาร >>>ยกเลิก...ให้ใช้ 14 แทน
            public const string WaitConsider = "05";    //รอผลการพิจารณา 
            public const string ApproveAccept = "06";   //อนุมัติ - ลูกค้าตกลง
            public const string ApproveEdit = "07";     //อนุมัติ - ส่งกลับแก้ไข >>>ยกเลิก...ให้ใช้ 11 แทน
            public const string Reject = "08";          //Reject
            public const string Cancel = "09";          //Cancel
            public const string Close = "10";           //ปิดงาน
            public const string RoutebackEdit = "11";           //ส่งกลับแก้ไข
            public const string COCWaitingConsider = "12";        //รอผลพิจารณา COC
            public const string COCReturn = "13";        //ส่งกลับแก้ไข COC
            public const string OnProcess = "14";       //อยู่ระหว่างดำเนินการ
            public const string WaitContact = "15";     //รอติดต่อลูกค้า 
        }
        public static class CampaignType
        {
            public const string Mass = "01";
            public const string BelowTheLine = "02";
        }
        public static class ChannelId
        {
            public const string Branch = "BRANCH";
            public const string CallCenter = "CALLCENTER";
            public const string Telesales = "TELESALES";
            public const string PriorityBanking = "PB";
        }
        public static class ActionType
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
            public const string RevertSettleClaim = "18";
            public const string SettleClaim = "19";
            public const string CancelSettleClaim = "20";
            public const string ReferTicket = "21";
            public const string RevertReceiveNo = "22";
            public const string RevertActSendDate = "23";
        }

        public static class Branch
        {
            public const int Active = 1;
            public const int InActive = 2;
            public const int All = 3;
        }

        public static class Position
        {
            public const int Active = 1;
            public const int InActive = 2;
            public const int All = 3;
        }

        public static class COCTeam
        {
            public const string Marketing = "MARKETING";
            public const string Bpel = "BPEL";
        }

        public static class HistoryTypeCode
        {
            public const string CreateLead = "001";
            public const string UpdateOwner = "002";
            public const string UpdateDelegate = "003";
            public const string AddCampaignFinal = "004";
            public const string UpdateStatus = "005";
            public const string UpdateLead = "006";
            public const string InsertNote = "007";
            public const string UpdateCardId = "008";
            public const string UpdateContractNumber = "009";
            public const string UpdateActNo = "010";
            public const string UpdatePolicyNo = "011";
            public const string UpdateInsuranceCarType = "012";
            public const string UpdateTelNoSms = "013";
            public const string UpdateDocReceiver = "014";
        }

        public static class ObtRecordType
        {
            public const string Followup = "Followup";
            public const string Inbound = "lead status";
            public const string Outbound = "obt status";
        }

        public static class SearchOrderBy
        {
            public const string None = "";
            public const string SLA = "SLA";
            public const string Note = "Note";
            public const string CampaignName = "CampaignName";
            public const string StatusDesc = "StatusDesc";
            public const string AssignedDate = "AssignedDate";
            public const string DelegateDate = "DelegateDate";
        }

        public static class SubStatusCode
        {
            public const string OfferSale = "01";        //เสนอขาย
            public const string ConsultHome = "02";       //ขอปรึกษาที่บ้านก่อน
            public const string Choose = "03";      //ขอเลือกบริษัทประกันก่อน
            public const string ContactCall = "04";     //ฝากให้โทรกลับ
            public const string OnProcess = "05";       //อยู่ระหว่างตัดสินใจ
            public const string NotRenew = "06";        //ไม่ต่อประกัน
            public const string CarSeized = "07";        //รถยึดแล้ว
            public const string ActPurchased = "08";        //ซื้อ พรบ เดี่ยว
            public const string AcceptRenew = "09";        //ต่อประกัน
            public const string NoSignal = "10";    //ปิดเครื่อง/ไม่มีสัญญาณ 
            public const string WasteLine = "11";   //สายเสีย
            public const string BusyLine = "12";     //สายไม่ว่าง
            public const string WrongNumber = "13";          //เบอร์ผิด/เปลี่ยนเบอร์แล้ว
            public const string NoRecipient = "14";          //ไม่มีผู้รับสาย
            public const string GetInsNo = "22";     //ออกเลขรับแจ้ง 
            public const string WaitDocIns = "26";          //รอเลขเล่มกรมธรรม์
            public const string WaitDocAct = "27";  // รอเลขที่ พรบ
            public const string CancelPolicy = "28"; // ลูกค้ายกเลิกกรมธรรม์
            public const string CancelAct = "29"; // ลูกค้ายกเลิก พ.ร.บ.
            public const string ReceiveActNo = "31";          //ได้เลข พ.ร.บ.
            public const string ProblemPending = "33";          //งานติดปัญหา
            public const string InformAct = "35";     //ส่งแจ้งพรบ. 
        }

        public static class SettleClaimStatus
        {
            public const string SettleClaim = "-1";       // settle claim (ไม่ระงับเคลม)
            public const string RevertSettleClaim = "1";  // revert settle claim (ระงับเคลม)
            public const string CancelSettleClaim = "0";  // cancel settle claim (ยกเลิกระงับเคลม)
        }
        //Added by Pom 21/04/2016
        public static class RightSideMenu
        {
            public const string TabControl = "001";
            public const string ButtonControl = "002";
            public const string Gift = "003";
            public const string CusInfo = "004";
            public const string SaleScript = "005";
        }

        //Added by Pom 22/04/2016
        public static class TabCode
        {
            public const string Activity = "001";
            public const string LeadInfo = "002";
            public const string ExistingLead = "003";
            public const string ExistingProduct = "004";
            public const string OwnerLogging = "005";
            public const string Note = "006";
            public const string InsuranceSummary = "007";
        }

        //Added by Pom 20/05/2016
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
            public static string CARActivityUrl
            {
                get { return ConfigurationManager.AppSettings["CARActivityUrl"] != null ? ConfigurationManager.AppSettings["CARActivityUrl"] : ""; }
            }
            public static string CARCreateActivityLogServiceName
            {
                get { return ConfigurationManager.AppSettings["CARCreateActivityLogServiceName"] != null ? ConfigurationManager.AppSettings["CARCreateActivityLogServiceName"] : ""; }
            }
            public static string CARErrorCodeResend
            {
                get { return ConfigurationManager.AppSettings["CARErrorCodeResend"] != null ? ConfigurationManager.AppSettings["CARErrorCodeResend"] : ""; }
            }
            public static string CARPreleadChannelId
            {
                get { return ConfigurationManager.AppSettings["CARPreleadChannelId"] != null ? ConfigurationManager.AppSettings["CARPreleadChannelId"] : ""; }
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
                        return ConfigurationManager.AppSettings["CARTimeout"] != null ? int.Parse(ConfigurationManager.AppSettings["CARTimeout"]) : 20;
                    }
                    catch
                    {
                        return 20;  //seconds
                    }
                }
            }

            public static class Data
            {
                //oldcasservice
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
                    //oldcasservice
                    //public const string Todo = "Todo";
                    //public const string SMS = "SMS Sending";
                    //public const string FYI = "FYI";
                    //public const string CallOutbound = "Call Outbound";
                    //public const string CallInbound = "Call Inbound";

                    public static decimal TodoId
                    {
                        get
                        {
                            try { return ConfigurationManager.AppSettings["CARActivityTypeToDoId"] != null ? decimal.Parse(ConfigurationManager.AppSettings["CARActivityTypeToDoId"]) : 0; }
                            catch { return 0; }
                        }
                    }
                    public static decimal SMSSendingId
                    {
                        get
                        {
                            try { return ConfigurationManager.AppSettings["CARActivityTypeSMSSendingId"] != null ? decimal.Parse(ConfigurationManager.AppSettings["CARActivityTypeSMSSendingId"]) : 0; }
                            catch { return 0; }
                        }
                    }
                    public static decimal FYIId
                    {
                        get
                        {
                            try { return ConfigurationManager.AppSettings["CARActivityTypeFYIId"] != null ? decimal.Parse(ConfigurationManager.AppSettings["CARActivityTypeFYIId"]) : 0; }
                            catch { return 0; }
                        }
                    }
                    public static decimal CallOutboundId
                    {
                        get
                        {
                            try { return ConfigurationManager.AppSettings["CARActivityTypeCallOutboundId"] != null ? decimal.Parse(ConfigurationManager.AppSettings["CARActivityTypeCallOutboundId"]) : 0; }
                            catch { return 0; }
                        }
                    }
                    public static decimal CallInboundId
                    {
                        get
                        {
                            try { return ConfigurationManager.AppSettings["CARActivityTypeCallInboundId"] != null ? decimal.Parse(ConfigurationManager.AppSettings["CARActivityTypeCallInboundId"]) : 0; }
                            catch { return 0; }
                        }
                    }
                }
            }
        }

        public static class Ecm
        {
            public static string SiteUrl
            {
                get { return ConfigurationManager.AppSettings["EcmSiteUrl"] != null ? ConfigurationManager.AppSettings["EcmSiteUrl"] : ""; }
            }
            public static string SitePath
            {
                get { return ConfigurationManager.AppSettings["EcmSitePath"] != null ? ConfigurationManager.AppSettings["EcmSitePath"] : ""; }
            }
            public static string ListName
            {
                get { return ConfigurationManager.AppSettings["EcmListName"] != null ? ConfigurationManager.AppSettings["EcmListName"] : ""; }
            }
            public static string Domain
            {
                get { return ConfigurationManager.AppSettings["EcmDomain"] != null ? ConfigurationManager.AppSettings["EcmDomain"] : ""; }
            }
            public static string Username
            {
                get { return ConfigurationManager.AppSettings["EcmUsername"] != null ? ConfigurationManager.AppSettings["EcmUsername"] : ""; }
            }
            public static string Password
            {
                get { return ConfigurationManager.AppSettings["EcmPassword"] != null ? ConfigurationManager.AppSettings["EcmPassword"] : ""; }
            }
        }

        public static class ConfigProductScreen
        {
            public static class ActionType
            {
                public const string Insert = "I";
                public const string Edit = "E";
                public const string View = "V";
            }
        }

        public static class SessionName
        {
            public const string CreditFormFilePath = "CreditFormFilePath";
            public const string Tawi50FormFilePath = "Tawi50FormFilePath";
            public const string DriverLicenseFormFilePath = "DriverLicenseFormFilePath";
            //public const string tabinbound_contractno = "tabinbound_contractno";
            //public const string tabinbound_licenseno = "tabinbound_licenseno";
            //public const string tabinbound_campaignid = "tabinbound_campaignid";
            //public const string tabinbound_citizenid = "tabinbound_citizenid";
            public const string tabscreenlist = "tabscreenlist";
            public const string configscreen = "configscreen";
            public const string default_phonecall_list = "default_phonecall_list";
            public const string renewinsure_phonecall_list = "renewinsure_phonecall_list";
            public const string screen_code = "screen_code";
            public const string RedirectData = "RedirectData";
            public const string UploadLead = "UploadLead";
            public const string UploadLeadSearchCondition = "UploadLeadSearchCondition";
            public const string CampaignId = "CampaignId";
            public const string ProductId = "ProductId";
        }

        public static class UploadLead
        {
            public static int UploadLeadMaxRow
            {
                get
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["UploadLeadMaxRow"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["UploadLeadMaxRow"]) : 5000;
                    }
                    catch
                    {
                        return 5000;
                    }
                }
            }

            public static int FieldFirstNameSize
            {
                get
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["FieldFirstNameSize"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["FieldFirstNameSize"]) : 100;
                    }
                    catch
                    {
                        return 100;
                    }
                }
            }

            public static int FieldLastNameSize
            {
                get
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["FieldLastNameSize"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["FieldLastNameSize"]) : 120;
                    }
                    catch
                    {
                        return 120;
                    }
                }
            }

            public static int FieldCardIdSize
            {
                get
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["FieldCardIdSize"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["FieldCardIdSize"]) : 50;
                    }
                    catch
                    {
                        return 50;
                    }
                }
            }

            public static int FieldDetailSize
            {
                get
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["FieldDetailSize"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["FieldDetailSize"]) : 4000;
                    }
                    catch
                    {
                        return 4000;
                    }
                }
            }
        }

        public static class ReAssignLead
        {
            public static int ReAssignLeadMaxRow
            {
                get
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["ReAssignLeadMaxRow"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["ReAssignLeadMaxRow"]) : 1000;
                    }
                    catch
                    {
                        return 1000;
                    }
                }
            }
            public static string ReAssignFileType
            {
                get
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["ReAssignFileType"] != null ? ConfigurationManager.AppSettings["ReAssignFileType"] : "xls,xlsx";
                    }
                    catch
                    {
                        return "xls,xlsx";
                    }
                }
            }
        }
    }
}
