using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Configuration;
using SLMBatch.Common;
using SLMBatch.WebService;

namespace SLMBatch.Business
{
    public class CARService
    {
        //Added by Pom 20/05/2016
        public static bool CreateActivityLog(CARServiceData data, Int64 batchMonitorId, string batchCode, string ticketId, string logfilename)
        {
            string errMsg = "";
            try
            {
                SLMBatch.WebService.CARServiceProxy.LogServiceHeader header = new SLMBatch.WebService.CARServiceProxy.LogServiceHeader()
                {
                    ReferenceNo = data.ReferenceNo,
                    SecurityKey = data.SecurityKey,
                    ServiceName = AppConstant.CARLogService.CARCreateActivityLogServiceName,
                    SystemCode = data.SystemCode,
                    TransactionDateTime = data.TransactionDateTime
                };

                data.ServiceName = AppConstant.CARLogService.CARCreateActivityLogServiceName;

                //Activity Info
                List<SLMBatch.WebService.CARServiceProxy.DataItem> actInfoList = new List<SLMBatch.WebService.CARServiceProxy.DataItem>();
                foreach (CARService.DataItem info in data.ActivityInfoList)
                {
                    actInfoList.Add(new SLMBatch.WebService.CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                //Customer Info
                List<SLMBatch.WebService.CARServiceProxy.DataItem> cusInfoList = new List<SLMBatch.WebService.CARServiceProxy.DataItem>();
                foreach (CARService.DataItem info in data.CustomerInfoList)
                {
                    cusInfoList.Add(new SLMBatch.WebService.CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                //Product Info
                List<SLMBatch.WebService.CARServiceProxy.DataItem> prodInfoList = new List<SLMBatch.WebService.CARServiceProxy.DataItem>();
                foreach (CARService.DataItem info in data.ProductInfoList)
                {
                    prodInfoList.Add(new SLMBatch.WebService.CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                //Contact Info
                List<SLMBatch.WebService.CARServiceProxy.DataItem> contInfoList = new List<SLMBatch.WebService.CARServiceProxy.DataItem>();
                foreach (CARService.DataItem info in data.ContractInfoList)
                {
                    contInfoList.Add(new SLMBatch.WebService.CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                //Officer Info
                List<SLMBatch.WebService.CARServiceProxy.DataItem> offInfoList = new List<SLMBatch.WebService.CARServiceProxy.DataItem>();
                foreach (CARService.DataItem info in data.OfficerInfoList)
                {
                    offInfoList.Add(new SLMBatch.WebService.CARServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
                }

                SLMBatch.WebService.CARServiceProxy.CreateActivityLogData logdata = new SLMBatch.WebService.CARServiceProxy.CreateActivityLogData()
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
                    SubscriptionTypeID = data.SubscriptionTypeId,
                    SubscriptionID = data.SubscriptionId,
                    TypeID = data.TypeId,
                    AreaID = data.AreaId,
                    SubAreaID = data.SubAreaId,
                    ActivityTypeID = data.ActivityTypeId,
                    ContractID = data.ContractNo
                };

                SLMBatch.WebService.CARServiceProxy.CASLogServiceSoapClient service = new SLMBatch.WebService.CARServiceProxy.CASLogServiceSoapClient();
                service.InnerChannel.OperationTimeout = new TimeSpan(0, 0, AppConstant.CARLogService.CARTimeout);
                var result = service.CreateActivityLog(header, logdata);

                if (result.ResponseStatus.ResponseCode == "CAS-I-000")
                {
                    return true;
                }
                else
                {
                    string message = "Call CAR WS Failure " + result.ResponseStatus.ResponseCode + ": " + result.ResponseStatus.ResponseMessage;
                    BizUtil.InsertLog(batchMonitorId, ticketId, "", message);
                    Util.WriteLogFile(logfilename, batchCode, message);

                    if (!AddCARLog(LogType.Error, result.ResponseStatus.ResponseCode, result.ResponseStatus.ResponseMessage, data, out errMsg))
                    {
                        BizUtil.InsertLog(batchMonitorId, ticketId, "", "Insert kkslm_tr_cas_resend failed: " + errMsg);
                        Util.WriteLogFile(logfilename, batchCode, "Insert kkslm_tr_cas_resend failed: " + errMsg);
                    }

                    return false;

                    //AddCARBatch(data, data.SystemCode, out errMsg, batchMonitorId, batchCode, ticketId);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                message = "Call CAR WS Failure = " + message;

                BizUtil.InsertLog(batchMonitorId, ticketId, "", message);
                Util.WriteLogFile(logfilename, batchCode, message);

                if (!AddCARLog(LogType.Fail, "", message, data, out errMsg))
                {
                    BizUtil.InsertLog(batchMonitorId, ticketId, "", "Insert kkslm_tr_cas_resend failed: " + errMsg);
                    Util.WriteLogFile(logfilename, batchCode, "Insert kkslm_tr_cas_resend failed: " + errMsg);
                }

                return false;

                //AddCARBatch(data, data.SystemCode, out errMsg, batchMonitorId, batchCode, ticketId);
            }
        }

        //private static string GetBatchFilename(string folder, string systemCode, DateTime curdate)
        //{
        //    int running = 1;
        //    string dtStr = String.Format(curdate.ToString("{0}MMdd"), curdate.Year);
        //    string tmpname = systemCode + "_" + dtStr + "_{0}.xml";

        //    // get last running
        //    string cfgfile = "running.cfg";
        //    string fullcfgpath = folder + "\\" + cfgfile;
        //    bool writenewcfg = true;
        //    if (File.Exists(fullcfgpath))
        //    {
        //        var cfgdetail = File.ReadAllText(fullcfgpath);
        //        var tmp = cfgdetail.Split(':');
        //        if (tmp.Length > 1 && tmp[0] == dtStr)
        //        {
        //            running = AppUtil.SafeInt(tmp[1]);
        //            // check file exists
        //            if (File.Exists(folder + "\\" + String.Format(tmpname, running.ToString("000"))))
        //                writenewcfg = false;
        //            else
        //                running++;  // file not exists create new running
        //        }
        //    }

        //    if (writenewcfg)
        //    {
        //        File.WriteAllText(fullcfgpath, dtStr + ":" + running.ToString());
        //    }


        //    // return new filename
        //    return String.Format(tmpname, running.ToString("000"));
        //}

        //public static bool AddCARBatch(CARServiceData data, string systemCode, out string error, Int64 batchMonitorId, string batchCode, string ticketId)
        //{

        //    bool ret = true;
        //    error = "";
        //    try
        //    {
        //        var curdate = DateTime.Now;

        //        // default
        //        string folder = AppConstant.CARLogService.CARBatchFolder;
        //        if (folder == "")
        //        {
        //            BizUtil.InsertLog(batchMonitorId, ticketId, "", "ไม่พบ Folder สำหรับจัดเก็บไฟล์ Excel เพื่อซ่อมการยิง WS CAR");
        //            Util.WriteLogFile(logfilename, batchCode, "ไม่พบ Folder สำหรับจัดเก็บไฟล์ Excel เพื่อซ่อมการยิง WS CAR");
        //            return false;
        //        }

        //        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        //        string workcheck = folder + "\\working.check";

        //        try
        //        {

        //            // ป้องกันการทำงานพร้อมกันหลาย sesion
        //            var startcheck = DateTime.Now;
        //            while (File.Exists(workcheck) && (DateTime.Now - startcheck).TotalSeconds < 10)
        //                System.Threading.Thread.Sleep(100);

        //            // create workcheck 
        //            File.WriteAllText(workcheck, "working..");

        //            string filename = GetBatchFilename(folder, systemCode, curdate); //systemCode + "_" + curdate + "_" + running.ToString("000") + ".xml";
        //            string fullpath = folder + "\\" + filename;

        //            // create batch data
        //            ActivityItem itm = new ActivityItem();
        //            itm.ReferenceNo = data.ReferenceNo;
        //            itm.ActivityDateTime = data.TransactionDateTime;
        //            itm.ChannelID = data.ChannelId;

        //            itm.SubscriptionID = data.SubscriptionId;
        //            itm.SubscriptionTypeID = data.SubscriptionTypeId;
        //            itm.LeadID = data.PreleadId;
        //            itm.TicketID = data.TicketId;
        //            itm.SrID = "";
        //            itm.ContractID = data.ContractNo;

        //            itm.ProductGroupID = data.ProductGroupId;
        //            itm.ProductID = data.ProductId;
        //            itm.CampaignID = data.CampaignId;
        //            itm.TypeName = data.TypeName;
        //            itm.AreaName = data.AreaName;
        //            itm.SubAreaName = data.SubAreaName;
        //            itm.ActivityType = data.ActivityType;

        //            itm.Status = data.Status;
        //            itm.SubStatus = data.SubStatus;

        //            itm.OfficerInfoList = data.OfficerInfoList;
        //            itm.ContractInfoList = data.ContractInfoList;
        //            itm.ProductInfoList = data.ProductInfoList;
        //            itm.CustomerInfoList = data.CustomerInfoList;
        //            itm.ActivityInfoList = data.ActivityInfoList;


        //            // create serializer
        //            XmlSerializer xs = new XmlSerializer(typeof(ActivityData));
        //            ActivityData batchdata;

        //            // check existing file
        //            if (File.Exists(fullpath))
        //            {
        //                // load old file
        //                using (var st = File.Open(fullpath, FileMode.Open))
        //                {
        //                    batchdata = xs.Deserialize(st) as ActivityData;
        //                    st.Close();
        //                }
        //            }
        //            else
        //            {
        //                // create new file
        //                batchdata = new ActivityData();
        //                batchdata.BatchDate = DateTime.Now;
        //                batchdata.SystemCode = systemCode;
        //                batchdata.Items = new List<ActivityItem>();
        //            }

        //            batchdata.BatchDate = DateTime.Now;
        //            batchdata.SystemCode = systemCode;
        //            batchdata.Items.Add(itm);

        //            batchdata.TotalItems = batchdata.Items.Count;

        //            // writeback
        //            if (File.Exists(fullpath)) File.Delete(fullpath);
        //            using (FileStream fs = new FileStream(fullpath, FileMode.Create))
        //            {
        //                xs.Serialize(fs, batchdata);
        //                fs.Close();
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
        //            BizUtil.InsertLog(batchMonitorId, ticketId, "", "ไม่สามารถสร้างไฟล์ xml เพื่อซ่อมการยิง WS CAR : " + error);
        //            Util.WriteLogFile(logfilename, batchCode, "ไม่สามารถสร้างไฟล์ xml เพื่อซ่อมการยิง WS CAR : " + error);
        //            ret = false;
        //        }
        //        File.Delete(workcheck);
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
        //        BizUtil.InsertLog(batchMonitorId, ticketId, "", "ไม่สามารถสร้างไฟล์ xml เพื่อซ่อมการยิง WS CAR : " + error);
        //        Util.WriteLogFile(logfilename, batchCode, "ไม่สามารถสร้างไฟล์ xml เพื่อซ่อมการยิง WS CAR : " + error);
        //        ret = false;
        //    }
        //    return ret;
        //}

        public static bool AddCARLog(LogType _logType, string ResponseCode, string ErrorMessage, CARServiceData data, out string error)
        {
            error = "";
            bool ret = true;
            try
            {
                // get config
                string errorCodeResend = AppConstant.CARLogService.CARErrorCodeResend;
                int maxResend = AppConstant.CARLogService.CARMaxResend;


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
                if (_logType == LogType.Fail) crr.slm_LogStatus = "R";
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
                itm.SubscriptionTypeID = data.SubscriptionTypeId;
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
            public string SubscriptionTypeId { get; set; }
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
            public string SubscriptionTypeID { get; set; }
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
