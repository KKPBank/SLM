using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class SlaData
    {
        public int? SlaId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string ChannelId { get; set; }
        public string ChannelDesc { get; set; }
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string SubStatusCode { get; set; }
        public string SubStatusDesc { get; set; }
        public int? SlaMin { get; set; }
        public int? SlaTime { get; set; }
        public int? SlaDay { get; set; }
    }
}
