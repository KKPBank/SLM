using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class SearchUserMonitoringCondition
    {
        public string UserList { get; set; }
        public string ProductId { get; set; }
        public string Campaign { get; set; }
        public string Branchcode { get; set; }
        public string Active { get; set; }
        public string AssignDateFrom { get; set; }
        public string AssignDateTo { get; set; }
        public string Teamtelesales { get; set; }
        public string LeadStatusList { get; set; }
        public string SubStatusList { get; set; }
    }
}
