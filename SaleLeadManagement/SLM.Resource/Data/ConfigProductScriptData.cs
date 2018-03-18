using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ConfigProductScriptData
    {
        public decimal? ConfigScriptId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string DataType { get; set; }
        public string Subject { get; set; }
        public string SubSubject { get; set; }
        public string Detail { get; set; }
        public bool? IsDeleted { get; set; }
        public int? Seq { get; set; }
        public string Status { get; set; }
    }
}
