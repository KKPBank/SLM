using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class AccessRightData
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int? StaffTypeId { get; set; }
        public string StaffTypeDesc { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
    }
}
