using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
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
        public string SubScriptionTypeId { get; set; }
        public string CustomerName { get; set; }
        public string LicenseNo { get; set; }
        public string Owner { get; set; }
        public string Delegate { get; set; }
        //Insurance
        public string InsuranceCompany { get; set; }
        public DateTime? PolicyStartCoverDate { get; set; }
        public DateTime? PolicyEndCoverDate { get; set; }
        public string PolicyNo { get; set; }
        public string ActNo { get; set; }
        public bool? IncentiveFlag { get; set; }
        public bool? IncentiveFlagAct { get; set; }
        public string ReceiveNo { get; set; }
        public DateTime? ActSendDate { get; set; }
        public DateTime? ActStartCoverDate { get; set; }
        public DateTime? ActEndCoverDate { get; set; }
    }
}
