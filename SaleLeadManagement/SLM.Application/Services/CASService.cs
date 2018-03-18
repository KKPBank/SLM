using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SLM.Resource;
using SLM.Resource.Data;
using System.IO;
using System.Xml.Serialization;
using System.Configuration;
using log4net;

namespace SLM.Application.Services
{
    public class CASService
    {
        //private static readonly ILog _log = LogManager.GetLogger(typeof(CASService));

        //Added by Pom 20/05/2016
        //public static bool CreateActivityLog(CASServiceData data)
        //{
        //    string errMsg = "";
        //    try
        //    {
        //        CASServiceProxy.LogServiceHeader header = new CASServiceProxy.LogServiceHeader()
        //        {
        //            ReferenceNo = data.ReferenceNo,
        //            SecurityKey = data.SecurityKey,
        //            ServiceName = SLMConstant.CARLogService.CARCreateActivityLogServiceName,
        //            SystemCode = data.SystemCode,
        //            TransactionDateTime = data.TransactionDateTime
        //        };

        //        //Activity Info
        //        List<CASServiceProxy.DataItem> actInfoList = new List<CASServiceProxy.DataItem>();
        //        foreach (Services.CASService.DataItem info in data.ActivityInfoList)
        //        {
        //            actInfoList.Add(new CASServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
        //        }

        //        //Customer Info
        //        List<CASServiceProxy.DataItem> cusInfoList = new List<CASServiceProxy.DataItem>();
        //        foreach (Services.CASService.DataItem info in data.CustomerInfoList)
        //        {
        //            cusInfoList.Add(new CASServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
        //        }

        //        //Product Info
        //        List<CASServiceProxy.DataItem> prodInfoList = new List<CASServiceProxy.DataItem>();
        //        foreach (Services.CASService.DataItem info in data.ProductInfoList)
        //        {
        //            prodInfoList.Add(new CASServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
        //        }

        //        //Contact Info
        //        List<CASServiceProxy.DataItem> contInfoList = new List<CASServiceProxy.DataItem>();
        //        foreach (Services.CASService.DataItem info in data.ContractInfoList)
        //        {
        //            contInfoList.Add(new CASServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
        //        }

        //        //Officer Info
        //        List<CASServiceProxy.DataItem> offInfoList = new List<CASServiceProxy.DataItem>();
        //        foreach (Services.CASService.DataItem info in data.OfficerInfoList)
        //        {
        //            offInfoList.Add(new CASServiceProxy.DataItem() { SeqNo = info.SeqNo, DataLabel = info.DataLabel, DataValue = info.DataValue });
        //        }

        //        CASServiceProxy.CreateActivityLogData logdata = new CASServiceProxy.CreateActivityLogData()
        //        {
        //            ActivityInfoList = actInfoList.ToArray(),
        //            CustomerInfoList = cusInfoList.ToArray(),
        //            ProductInfoList = prodInfoList.ToArray(),
        //            ContractInfoList = contInfoList.ToArray(),
        //            OfficerInfoList = offInfoList.ToArray(),
        //            ActivityDateTime = DateTime.Now,
        //            CampaignID = data.CampaignId,
        //            ChannelID = data.ChannelId,
        //            LeadID = data.PreleadId,
        //            ProductGroupID = data.ProductGroupId,
        //            ProductID = data.ProductId,
        //            Status = data.Status,
        //            SubStatus = data.SubStatus,
        //            TicketID = data.TicketId,
        //            SubscriptionTypeID = data.SubscriptionTypeId,
        //            SubscriptionID = data.SubscriptionId,
        //            TypeName = data.TypeName,
        //            AreaName = data.AreaName,
        //            SubAreaName = data.SubAreaName,
        //            ActivityType = data.ActivityType,
        //            ContractID = data.ContractNo
        //        };

        //        CASServiceProxy.CASLogServiceSoapClient service = new CASServiceProxy.CASLogServiceSoapClient();
        //        service.InnerChannel.OperationTimeout = new TimeSpan(0, 0, SLMConstant.CARLogService.CARTimeout);
        //        var result = service.CreateActivityLog(header, logdata);

        //        if (result.ResponseStatus.ResponseCode == "CAS-I-000")
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            AddCARBatch(data, data.SystemCode, out errMsg);
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Error(message);

        //        AddCARBatch(data, data.SystemCode, out errMsg);
        //        return false;
        //    }
        //}

        //public string GetCallCASScript(LeadData data)
        //{
        //    string url = SLMConstant.CARLogService.CARActivityUrl;
        //    string param = "";
        //    param += (param != "" ? "&" : "") + "system=" + (data.PreleadId != null ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM);
        //    param += (param != "" ? "&" : "") + "ticket=" + data.TicketId;
        //    param += (param != "" ? "&" : "") + "subscriptiontype=" + (data.CardType != null ? data.CardType.Value.ToString() : "");
        //    param += (param != "" ? "&" : "") + "subscription=" + data.CitizenId;
        //    param += (param != "" ? "&" : "") + "lead=" + (data.PreleadId != null ? data.PreleadId.Value.ToString() : "");

        //    url = url + (param != "" ? "?" + param : "");
        //    return "window.open('" + url + "', 'SearchCAS', 'status=yes, toolbar=no, scrollbars=yes, menubar=no, width=1000, height=600, resizable=yes'); return false;";

        //    //            string url = SLMConstant.CARLogService.CARActivityUrl;
        //    //            string script = @"var form = document.createElement('form');
        //    //                                    var input_system = document.createElement('input');
        //    //                                    var input_ticket = document.createElement('input');
        //    //                                    var input_subscriptiontype = document.createElement('input');
        //    //                                    var input_subscription = document.createElement('input');
        //    //                                    var input_lead = document.createElement('input');
        //    //
        //    //                                    form.action = '" + url + @"';
        //    //                                    form.method = 'post';
        //    //                                    form.setAttribute('target', 'searchcas');
        //    //
        //    //                                    input_system.name = 'system';
        //    //                                    input_system.value = '" + (data.PreleadId != null ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM) + @"';
        //    //                                    form.appendChild(input_system);
        //    //
        //    //                                    input_ticket.name = 'ticket';
        //    //                                    input_ticket.value = '" + data.TicketId + @"';
        //    //                                    form.appendChild(input_ticket);
        //    //
        //    //                                    input_subscriptiontype.name = 'subscriptiontype';
        //    //                                    input_subscriptiontype.value = '" + (data.CardType != null ? data.CardType.Value.ToString() : "") + @"';
        //    //                                    form.appendChild(input_subscriptiontype);
        //    //
        //    //                                    input_subscription.name = 'subscription';
        //    //                                    input_subscription.value = '" + data.CitizenId + @"';
        //    //                                    form.appendChild(input_subscription);
        //    //
        //    //                                    input_lead.name = 'lead';
        //    //                                    input_lead.value = '" + (data.PreleadId != null ? data.PreleadId.Value.ToString() : "") + @"';
        //    //                                    form.appendChild(input_lead);
        //    //
        //    //                                    document.body.appendChild(form);
        //    //                                    form.submit();
        //    //
        //    //                                    document.body.removeChild(form);";

        //    //            return script;
        //}

        //public string GetCallCASScript(PreleadViewData data)
        //{
        //    string url = SLMConstant.CARLogService.CARActivityUrl;
        //    string param = "";
        //    param += (param != "" ? "&" : "") + "system=" + (data.PreleadId != null ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM);
        //    param += (param != "" ? "&" : "") + "ticket=" + data.TicketId;
        //    param += (param != "" ? "&" : "") + "subscriptiontype=" + (data.CardTypeId != null ? data.CardTypeId.Value.ToString() : "");
        //    param += (param != "" ? "&" : "") + "subscription=" + data.CitizenId;
        //    param += (param != "" ? "&" : "") + "lead=" + (data.PreleadId != null ? data.PreleadId.Value.ToString() : "");

        //    url = url + (param != "" ? "?" + param : "");
        //    return "window.open('" + url + "', 'SearchCAS', 'status=yes, toolbar=no, scrollbars=yes, menubar=no, width=1000, height=600, resizable=yes'); return false;";
        //}

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
        //            running = SLMUtil.SafeInt(tmp[1]);
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

        //public static bool AddCARBatch(CASServiceData data, string systemCode, out string error)
        //{

        //    bool ret = true;
        //    error = "";
        //    try
        //    {
        //        var curdate = DateTime.Now;

        //        // default
        //        string folder = HttpContext.Current.Server.MapPath("~/Batch");

        //        if (ConfigurationManager.AppSettings["CARBatchFolder"] != null)
        //            folder = ConfigurationManager.AppSettings["CARBatchFolder"];

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
        //            itm.ContractID = data.ContractNo;       //Added by Pom 16/06/2016

        //            itm.ProductGroupID = data.ProductGroupId;
        //            itm.ProductID = data.ProductId;
        //            itm.CampaignID = data.CampaignId;
        //            itm.TypeName = data.TypeName;
        //            itm.AreaName = data.AreaName;
        //            itm.SubAreaName = data.SubAreaName;
        //            itm.ActivityTypeID = 0;                    // ** new 
        //            itm.ActivityTypeName = data.ActivityType;   // ** new

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
        //            _log.Error(error);
        //            ret = false;
        //        }
        //        File.Delete(workcheck);
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
        //        _log.Error(error);
        //        ret = false;
        //    }
        //    return ret;
        //}

        //public class CASServiceData
        //{
        //    public CASServiceData()
        //    {
        //        ActivityInfoList = new List<DataItem>();
        //        CustomerInfoList = new List<DataItem>();
        //        ProductInfoList = new List<DataItem>();
        //        ContractInfoList = new List<DataItem>();
        //        OfficerInfoList = new List<DataItem>();
        //    }

        //    public string ReferenceNo { get; set; }
        //    public string SecurityKey { get; set; }
        //    public string ServiceName { get; set; }
        //    public string SystemCode { get; set; }
        //    public DateTime TransactionDateTime { get; set; }
        //    public List<DataItem> ActivityInfoList { get; set; }
        //    public List<DataItem> CustomerInfoList { get; set; }
        //    public List<DataItem> ProductInfoList { get; set; }
        //    public List<DataItem> ContractInfoList { get; set; }
        //    public List<DataItem> OfficerInfoList { get; set; }
        //    public DateTime? ActivityDateTime { get; set; }
        //    public string CampaignId { get; set; }
        //    public string ChannelId { get; set; }
        //    public string PreleadId { get; set; }
        //    public string TicketId { get; set; }
        //    public string ProductGroupId { get; set; }
        //    public string ProductId { get; set; }
        //    public string Status { get; set; }
        //    public string SubStatus { get; set; }
        //    public int SubscriptionTypeId { get; set; }
        //    public string SubscriptionId { get; set; }
        //    public string TypeName { get; set; }
        //    public string AreaName { get; set; }
        //    public string SubAreaName { get; set; }
        //    public string ActivityType { get; set; }
        //    public string ContractNo { get; set; }
        //}
        //[Serializable]
        //public class DataItem
        //{
        //    public int SeqNo { get; set; }
        //    public string DataLabel { get; set; }
        //    public string DataValue { get; set; }
        //}
        //[Serializable]
        //public class ActivityItem
        //{
        //    public string ReferenceNo { get; set; }
        //    // required
        //    public DateTime ActivityDateTime { get; set; }
        //    public string ChannelID { get; set; }

        //    // required once
        //    public int SubscriptionTypeID { get; set; }
        //    public string SubscriptionID { get; set; }
        //    public string LeadID { get; set; }
        //    public string TicketID { get; set; }
        //    public string SrID { get; set; }
        //    public string ContractID { get; set; }

        //    // not required

        //    public string ProductGroupID { get; set; }
        //    public string ProductID { get; set; }
        //    public string CampaignID { get; set; }
        //    public decimal TypeID { get; set; }
        //    public decimal AreaID { get; set; }
        //    public decimal SubAreaID { get; set; }
        //    public decimal ActivityTypeID { get; set; }
        //    public string TypeName { get; set; }
        //    public string AreaName { get; set; }
        //    public string SubAreaName { get; set; }
        //    public string ActivityTypeName { get; set; }

        //    // Others
        //    public string KKCISID { get; set; }
        //    public string CISID { get; set; }
        //    public string TrxSeqID { get; set; }
        //    public string NoncustomerID { get; set; }
        //    public string Status { get; set; }
        //    public string SubStatus { get; set; }


        //    // list of data item

        //    public List<DataItem> OfficerInfoList { get; set; }
        //    public List<DataItem> ContractInfoList { get; set; }
        //    public List<DataItem> ProductInfoList { get; set; }
        //    public List<DataItem> CustomerInfoList { get; set; }
        //    public List<DataItem> ActivityInfoList { get; set; }
        //}
        //[Serializable]
        //public class ActivityData
        //{
        //    public string SystemCode { get; set; }
        //    public DateTime BatchDate { get; set; }
        //    public int TotalItems { get; set; }

        //    public List<ActivityItem> Items { get; set; }
        //}

    }

}   