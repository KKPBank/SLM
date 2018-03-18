using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;
using System.Configuration;
using System.IO;
using SLM.Resource;
using SLM.Biz;
using SLM.Dal;
using log4net;

namespace SLM.CARBatch
{

    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        const string ProgramID = "CARBatchResend";

        static void Main(string[] args)
        {
            CARServiceBiz bz = new CARServiceBiz();
            DateTime starttime = DateTime.Now;

            try
            {
                _log.Info("===================== Start Log =====================");
                _log.Info("Start at " + DateTime.Now.ToString("dd/MM/") + DateTime.Now.Year.ToString() + " " + DateTime.Now.ToString("HH:mm:ss"));

                var batch = bz.StartBatch(ProgramID);
                try {

                    _log.Info("SLM.CARBatch Resend v" + typeof(Program).Assembly.GetName().Version.ToString());
                    Console.WriteLine("SLM.CARBatch Resend v" + typeof(Program).Assembly.GetName().Version.ToString());
                    Console.WriteLine("");

                    List<kkslm_tr_batchlog> errList = new List<kkslm_tr_batchlog>();

                    // default config
                    int? maxresend = null;
                    int? maxexecute = null;
                    string errcoderesend = "";

                    // retrieve config
                    if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["CARMaxResend"])) maxresend = SLMUtil.SafeInt(ConfigurationManager.AppSettings["CARMaxResend"]);
                    if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["CARMaxExcuteTime"])) maxexecute = SLMUtil.SafeInt(ConfigurationManager.AppSettings["CARMaxExcuteTime"]);
                    if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["CARErrorCodeResend"])) errcoderesend = ConfigurationManager.AppSettings["CARErrorCodeResend"];


                    // get list of item to resend
                    AddLog("Retriving data to resend..");
                    var lst = bz.GetResendList();

                    _log.Info(String.Format("Found {0} items to resend", lst.Count));
                    AddLog(String.Format("Found {0} items to resend", lst.Count));

                    _log.Info("Start resend data at " + DateTime.Now.ToString("dd/MM/") + DateTime.Now.Year.ToString() + " " + DateTime.Now.ToString("HH:mm:ss"));
                    AddLog("Start resend data...");

                    var ErrToResend = errcoderesend.Split(',');

                    int i = 0;
                    int ff = 0;
                    int ss = 0;
                    while (i < lst.Count && (maxexecute == null || (DateTime.Now - starttime).TotalMinutes <= maxexecute))
                    {
                        var cas = lst[i];
                        bool doresend = false;

                        //inital
                        if (cas.slm_ResendCount == null) cas.slm_ResendCount = 0;
                        if (cas.slm_ResendCount == 0) cas.slm_ResendDate = starttime;

                        // verify valid cas
                        if (maxresend != null && cas.slm_ResendCount >= maxresend)
                        {
                            cas.slm_LogStatus = "E";
                        }
                        else
                        {
                            bool isresendcode = false;

                            if (cas.slm_CasLogType == "F" && cas.slm_ResendCount == 0) isresendcode = true;     // Resend ครั้งแรกเชค Type เป็น Fail ให้ resend ต่อ
                            else if (cas.slm_ResendCount == 0)                                                  // Resend ครั้งแรกเชคจาก CasResponse
                            {
                                foreach (var err in ErrToResend)
                                    if (cas.slm_CasResponse.Contains(err)) isresendcode = true;
                            }
                            else if (cas.slm_LastResendRespone == "") isresendcode = true;      // resend ไปแล้ว แต่ Fail ให้ resend ต่อ
                            else foreach (var err in ErrToResend)                               // resend ไปแล้ว แต่ Error เชคจาก LastResendResponse
                                    if (cas.slm_LastResendRespone.Contains(err)) isresendcode = true;


                            if (!isresendcode)
                                cas.slm_LogStatus = "F";
                            else
                                doresend = true;
                        }

                        if (doresend)
                        {
                            cas.slm_ResendCount += 1;
                            cas.slm_LastResendDate = starttime;
                            ActivityItem casdata = null;

                            //string systemCode = "SLM";
                            string OBTsystemKey = ConfigurationManager.AppSettings["CARSecurityKeyOBT"] ?? "12345";
                            string SLMsystemKey = ConfigurationManager.AppSettings["CARSecurityKeySLM"] ?? "12345";
                            string serviceName = ConfigurationManager.AppSettings["CARCreateServieName"] ?? "CreateActivityLog";



                            try
                            {
                                string systemKey = "";
                                // check systemcode to get security key
                                switch (cas.slm_CasSystemCode.ToUpper())
                                {
                                    case "OBT": systemKey = OBTsystemKey; break;
                                    case "SLM": systemKey = SLMsystemKey; break;
                                    default:
                                        {
                                            // invalid systemcode no more resend
                                            cas.slm_LogStatus = "F";
                                            throw new Exception("Invalid System Code");
                                        }
                                }

                                try
                                {
                                    // try decode xml, if fail no more resend
                                    var xml = cas.slm_CasDetail;
                                    XmlSerializer xs = new XmlSerializer(typeof(ActivityItem));

                                    StringReader srd = new StringReader(xml);
                                    casdata = xs.Deserialize(srd) as ActivityItem;
                                    if (casdata == null)
                                        throw new Exception("Invalid XML Detail");

                                }
                                catch (Exception ex)
                                {
                                    cas.slm_LogStatus = "F";
                                    throw ex;
                                }


                                // call service to resend log, if fail try nex time
                                CARWebservice.CASLogServiceSoapClient client = new CARWebservice.CASLogServiceSoapClient();
                                var header = new CARWebservice.LogServiceHeader()
                                {
                                    SystemCode = cas.slm_CasSystemCode,
                                    ServiceName = serviceName,
                                    ReferenceNo = cas.slm_CasReferenceNo,
                                    SecurityKey = systemKey,
                                    TransactionDateTime = DateTime.Now
                                };
                                var detail = new CARWebservice.CreateActivityLogData()
                                {
                                    ActivityDateTime = casdata.ActivityDateTime,
                                    ChannelID = casdata.ChannelID,

                                    SubscriptionTypeID = casdata.SubscriptionTypeID,
                                    SubscriptionID = casdata.SubscriptionID,
                                    LeadID = casdata.LeadID,
                                    TicketID = casdata.TicketID,
                                    SrID = casdata.SrID,
                                    ContractID = casdata.ContractID,

                                    ProductGroupID = casdata.ProductGroupID,
                                    ProductID = casdata.ProductID,
                                    CampaignID = casdata.CampaignID,
                                    TypeID = casdata.TypeID,
                                    AreaID = casdata.AreaID,
                                    SubAreaID = casdata.SubAreaID,
                                    ActivityTypeID = casdata.ActivityTypeID,
                                    TypeName = casdata.TypeName,
                                    AreaName = casdata.AreaName,
                                    SubAreaName = casdata.SubAreaName,
                                    ActivityTypeName = casdata.ActivityTypeName,

                                    KKCISID = casdata.KKCISID,
                                    CISID = casdata.CISID,
                                    TrxSeqID = casdata.TrxSeqID,
                                    NoncustomerID = casdata.NoncustomerID,
                                    Status = casdata.Status,
                                    SubStatus = casdata.SubStatus,

                                    OfficerInfoList = casdata.OfficerInfoList.Select(l => new CARWebservice.DataItem() { SeqNo = l.SeqNo, DataLabel = l.DataLabel, DataValue = l.DataValue }).ToArray(),
                                    ContractInfoList = casdata.ContractInfoList.Select(l => new CARWebservice.DataItem() { SeqNo = l.SeqNo, DataLabel = l.DataLabel, DataValue = l.DataValue }).ToArray(),
                                    ProductInfoList = casdata.ProductInfoList.Select(l => new CARWebservice.DataItem() { SeqNo = l.SeqNo, DataLabel = l.DataLabel, DataValue = l.DataValue }).ToArray(),
                                    CustomerInfoList = casdata.CustomerInfoList.Select(l => new CARWebservice.DataItem() { SeqNo = l.SeqNo, DataLabel = l.DataLabel, DataValue = l.DataValue }).ToArray(),
                                    ActivityInfoList = casdata.ActivityInfoList.Select(l => new CARWebservice.DataItem() { SeqNo = l.SeqNo, DataLabel = l.DataLabel, DataValue = l.DataValue }).ToArray()

                                };

                                _log.Info(String.Format("Resending data ID: {0} ..", cas.slm_CasLogId));

                                // write header to log
                                XmlSerializer sr = new XmlSerializer(typeof(CARWebservice.LogServiceHeader));
                                using (StringWriter sw = new StringWriter())
                                {
                                    sr.Serialize(sw, header);
                                    _log.Info("Data header: " + sw.ToString());
                                }

                                // write data to log
                                sr = new XmlSerializer(typeof(CARWebservice.CreateActivityLogData));
                                using (StringWriter sw = new StringWriter())
                                {
                                    sr.Serialize(sw, detail);
                                    _log.Info("Data details: " + sw.ToString());
                                }

                                var response = client.CreateActivityLog(header, detail);
                                cas.slm_LastResendRespone = response.ResponseStatus.ResponseCode;
                                cas.slm_LastResendMessage = response.ResponseStatus.ResponseMessage;
                                if (response.ResponseStatus.ResponseCode == "CAS-I-000") cas.slm_LogStatus = "S";

                                // write response to log
                                sr = new XmlSerializer(typeof(CARWebservice.CreateActivityLogResponse));
                                using (StringWriter sw = new StringWriter())
                                {
                                    sr.Serialize(sw, response);
                                    _log.Info("Resend successful with response : " + sw.ToString());
                                }

                                ss++;
                            }
                            catch (Exception ex)
                            {
                                ff++;
                                cas.slm_LastResendRespone = "";
                                cas.slm_LastResendMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                                errList.Add(new kkslm_tr_batchlog() { slm_ErrorDetail = "CasLogId=" + cas.slm_CasLogId.ToString() + ", " + cas.slm_LastResendMessage, slm_WorkDetailId = i });
                                _log.Error(String.Format("Error process item ID {0}", cas.slm_CasLogId), ex);
                            }

                        }
                        else ss++;

                        // update cas resend log
                        if (!bz.UpdateCarResend(new CARServiceBiz.CASResendData()
                        {
                            slm_CasLogId = cas.slm_CasLogId,
                            slm_LogStatus = cas.slm_LogStatus,
                            slm_ResendDate = cas.slm_ResendDate,
                            slm_ResendCount = cas.slm_ResendCount,
                            slm_LastResendDate = cas.slm_LastResendDate,
                            slm_LastResendRespone = cas.slm_LastResendRespone,
                            slm_LastResendMessage = cas.slm_LastResendMessage,
                            slm_BatchVersion = typeof(Program).Assembly.GetName().Version.ToString()
                        }, ProgramID))
                        {
                            _log.Error("Error while Update resend data - " + bz.ErrorMessage);
                            AddLog("Error while Update resend data - " + bz.ErrorMessage);
                        }

                        i++;
                    }

                    if (i == lst.Count)
                    {
                        _log.Info("Process finished at " + DateTime.Now.ToString("dd/MM/") + DateTime.Now.Year.ToString() + " " + DateTime.Now.ToString("HH:mm:ss"));
                        AddLog("Process finished");
                    }
                    else
                    {
                        _log.Error("Process stop due to exceed time limit at " + DateTime.Now.ToString("dd/MM/") + DateTime.Now.Year.ToString() + " " + DateTime.Now.ToString("HH:mm:ss"));
                        AddLog("Process stop due to exceed time limit");
                    }

                    // save batch (success)
                    bz.FinishBatch(i, ss, ff, (ff > 0 ? "2" : "1"), errList, ProgramID);
                    

                }
                catch (Exception ex)
                {
                    // save batch (fail)
                    bz.FinishBatch(0, 0, 0, "2",  new List<Dal.kkslm_tr_batchlog>() { new Dal.kkslm_tr_batchlog() { slm_ErrorDetail = ex.InnerException == null ? ex.Message : ex.InnerException.Message, slm_WorkDetailId = 0 } }, ProgramID);
                    throw ex;
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException == null ? ex.Message : ex.InnerException.Message, ex);
                AddLog("Error Exception - " + ex.Message);
            }

            _log.Info("End at " + DateTime.Now.ToString("dd/MM/") + DateTime.Now.Year.ToString() + " " + DateTime.Now.ToString("HH:mm:ss"));
            _log.Info("===================== End Log ======================");
            _log.Debug("");
        }

        private static void AddLog(string str)
        {
            Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy - ") + str);
        }
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

}
