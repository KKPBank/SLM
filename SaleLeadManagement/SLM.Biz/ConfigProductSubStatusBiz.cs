using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class ConfigProductSubStatusBiz
    {
        public List<ControlListData> GetSubStatusList(string productId, string campaignId, string statusCode)
        {
            return new KKSlmMsConfigProductSubstatusModel().GetSubStatusList(productId, campaignId, statusCode);
        }

        public List<ConfigProductSubStatusData> GetSubStatusList(string productId, string campaignId)
        {
            return new KKSlmMsConfigProductSubstatusModel().GetSubStatusList(productId, campaignId);
        }

        public List<ConfigProductSubStatusData> GetSubStatusListByStatusActivityConfig(string productId, string campaignId, List<string> statusValueList)
        {
            return new KKSlmMsConfigProductSubstatusModel().GetSubStatusListByStatusActivityConfig(productId, campaignId, statusValueList);
        }

        public List<ControlListData> GetSubStatusListByCampaignId(string campaignId, string statusCode)
        {
            return new KKSlmMsConfigProductSubstatusModel().GetSubStatusListByCampaignId(campaignId, statusCode);
        }

        public List<ControlListData> GetSubStatusListByProductId(string productId, string statusCode)
        {
            return new KKSlmMsConfigProductSubstatusModel().GetSubStatusListByProductId(productId, statusCode);
        }

        //public List<ControlListData> GetStatus(string productId, string campaignId, string subStatusCode)
        //{
        //    return new KKSlmMsConfigProductSubstatusModel().GetStatus(productId, campaignId, subStatusCode);
        //}

        //public List<ControlListData> GetInboundSubStatusList(string productId, string campaignId)
        //{
        //    return new KKSlmMsConfigProductSubstatusModel().GetInboundSubStatusList(productId, campaignId);
        //}
    }
}
