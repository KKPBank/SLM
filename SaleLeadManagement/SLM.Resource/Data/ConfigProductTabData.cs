using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ConfigProductTabData
    {
        public string ProductId { get; set; }
        public string CampaignId { get; set; }
        public string TabCode { get; set; }
        public string TabHeader { get; set; }
        public int? Seq { get; set; }
        public string ScreenPath { get; set; }
    }

    public class ConfigProductTabSideMenuData
    {
        public string ProductId { get; set; }
        public string CampaignId { get; set; }
        public string MenuCode { get; set; }
        public string MenuDescription { get; set; }
        public int? Seq { get; set; }
        public string ImagePath { get; set; }
    }
}
