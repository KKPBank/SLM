using SLMBatch.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Business
{
    public class ServiceBase
    {
        public string logfilename;

        public string SetLogFileName
        {
            set { logfilename = value; }
        }

        public Int64 BatchMonitorId { get; set; }
        public string BatchCode { get; set; }
        public string ErrorMessage { get; set; }

        public string SLMDBConnectionString
        {
            get
            {
                SLMDBEntities slmdb = new SLMDBEntities();
                return ((System.Data.EntityClient.EntityConnection)slmdb.Connection).StoreConnection.ConnectionString;
            }
        }

        public void UpdateStatusAndOwnerLogging(SLMDBEntities slmdb, kkslm_tr_lead lead, DateTime createdDate, string newStatusCode, string newSubStatusCode, string loggingType)
        {
            try
            {
                string oldStatus = lead.slm_Status;
                string oldSubStatus = string.IsNullOrEmpty(lead.slm_SubStatus) ? null : lead.slm_SubStatus;
                string oldExternalSubStatusDesc = string.IsNullOrEmpty(lead.slm_ExternalSubStatusDesc) ? null : lead.slm_ExternalSubStatusDesc;
                string oldExternalSystem = lead.slm_ExternalSystem;
                string oldExternalStatus = lead.slm_ExternalStatus;
                string oldExternalSubStatus = lead.slm_ExternalSubStatus;

                newSubStatusCode = string.IsNullOrEmpty(newSubStatusCode) ? null : newSubStatusCode;

                if (oldStatus != newStatusCode || oldSubStatus != newSubStatusCode)
                {
                    string newSubStatusDesc = GetSubStatusName(slmdb, lead.slm_CampaignId, lead.slm_Product_Id, newStatusCode, newSubStatusCode);

                    kkslm_tr_activity act = new kkslm_tr_activity()
                    {
                        slm_TicketId = lead.slm_ticketId,
                        slm_OldStatus = oldStatus,
                        slm_OldSubStatus = oldSubStatus,
                        slm_NewStatus = newStatusCode,
                        slm_NewSubStatus = newSubStatusCode,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedBy_Position = null,
                        slm_CreatedDate = createdDate,
                        slm_Type = loggingType,
                        slm_SystemAction = "SLM",
                        slm_SystemActionBy = "SLM",
                        slm_ExternalSystem_Old = oldExternalSystem,
                        slm_ExternalStatus_Old = oldExternalStatus,
                        slm_ExternalSubStatus_Old = oldExternalSubStatus,
                        slm_ExternalSubStatusDesc_Old = oldExternalSubStatusDesc,
                        slm_ExternalStatus_New = null,
                        slm_ExternalSubStatus_New = null,
                        slm_ExternalSystem_New = null,
                        slm_ExternalSubStatusDesc_New = newSubStatusDesc
                    };
                    slmdb.kkslm_tr_activity.AddObject(act);

                    AppUtil.CalculateTotalSLA(slmdb, lead, act, createdDate, logfilename, BatchCode);
                    lead.slm_Status = newStatusCode;
                    lead.slm_SubStatus = newSubStatusCode;
                    lead.slm_StatusDate = createdDate;
                    lead.slm_StatusDateSource = createdDate;
                    lead.slm_StatusBy = "SYSTEM";
                    lead.slm_Counting = 0;
                    lead.slm_ExternalSystem = null;
                    lead.slm_ExternalStatus = null;
                    lead.slm_ExternalSubStatus = null;
                    lead.slm_ExternalSubStatusDesc = newSubStatusDesc;
                    lead.slm_UpdatedBy = "SYSTEM";
                    lead.slm_UpdatedDate = createdDate;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetSubStatusName(SLMDBEntities slmdb, string campaignId, string productId, string statusCode, string substatusCode)
        {
            try
            {
                string substatusName = "";
                substatusName = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_CampaignId == campaignId && p.slm_Product_Id == null
                                    && p.slm_OptionCode == statusCode && p.slm_SubStatusCode == substatusCode).Select(p => p.slm_SubStatusName).FirstOrDefault();

                if (string.IsNullOrEmpty(substatusName))
                {
                    substatusName = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_CampaignId == null
                                    && p.slm_OptionCode == statusCode && p.slm_SubStatusCode == substatusCode).Select(p => p.slm_SubStatusName).FirstOrDefault();
                }

                return substatusName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
