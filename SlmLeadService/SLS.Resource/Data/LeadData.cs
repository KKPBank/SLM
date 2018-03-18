using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource.Data
{
    public class LeadData
    {
    }

    public class LeadDataForCARLogService
    {
        public string TicketId { get; set; }
        public decimal? PreleadId { get; set; }
        public string ContractNo { get; set; }
        public string ChannelId { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductId { get; set; }
        public string StatusCode { get; set; }
        public string ProductName { get; set; }
        public string StatusName { get; set; }
        public string SubStatusCode { get; set; }
        public string SubStatusName { get; set; }
        public string CitizenId { get; set; }
        public int? CardTypeId { get; set; }
        public string CardTypeName { get; set; }
        public int? SubScriptionTypeId { get; set; }
        public string LicenseNo { get; set; }
        public string Owner { get; set; }
        public string OwnerEmail { get; set; }
        public string Delegate { get; set; }
        public string DelegateEmail { get; set; }
        public string CustomerName { get; set; }

        //COC
        public string MarketingOwner { get; set; }
        public string MarketingOwnerEmail { get; set; }
        public string LastOwner { get; set; }
        public string LastOwnerEmail { get; set; }
    }
}
