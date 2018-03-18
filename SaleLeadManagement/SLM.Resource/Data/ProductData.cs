using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ProductData
    {
        public string ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignDesc { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Recommend { get; set; }   //แนะนำแคมเปญจาก CMT
        public string CampaignType { get; set; }
        public bool HasAdamsUrl { get; set; }
    }

    public class ProductBundleConfigData
    {
        public int? ProductBundleId { get; set; }
        public string ProductId { get; set; }
        public string ProductBundle { get; set; }
        public string BundleDescription { get; set; }
    }
}
