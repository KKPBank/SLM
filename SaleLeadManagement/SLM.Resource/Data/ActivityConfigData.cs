using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ActivityConfigData
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string LeadStatusCode { get; set; }
        public string LeadStatusDesc { get; set; }
        public bool? HaveRightAdd { get; set; }
        public string HaveRightAddDesc { get; set; }
        public string LeadAvailableStatusCode { get; set; }
        public string LeadAvailableStatusDesc { get; set; }
    }
}
