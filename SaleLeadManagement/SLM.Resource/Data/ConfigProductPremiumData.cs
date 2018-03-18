using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ConfigProductPremiumData
    {
        public decimal? PremiumId { get; set; }
        public string PremiumName { get; set; }
        public string PremiumType { get; set; }
        public string ProductName { get; set; }
        public string CampaignName { get; set; }
        public int? LotNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TotalAll { get; set; }
        public int? TotalUsed { get; set; }
        public int? TotalRemain { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ProductId { get; set; }
        public string CampaignId { get; set; }
    }
}
