using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class UserMonitoringData
    {
        public string No { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public string FullnameTH { get; set; }
        public string CampaignName { get; set; }
        public decimal? Active { get; set; }
        public int? Amount { get; set; }
    }

    public class UserMonitoringReInsuranceData
    {
        public string No { get; set; }
        public string RoleName { get; set; }
        public string FullnameTH { get; set; }
        public string Username { get; set; }
        public decimal? Active { get; set; }
        public int? SUM_STATUS_01 { get; set; }
        public int? SUM_STATUS_02 { get; set; }
        public int? SUM_STATUS_03 { get; set; }
        public int? SUM_STATUS_04 { get; set; }
        public int? SUM_STATUS_05 { get; set; }
        public int? SUM_STATUS_10 { get; set; }
        public int? SUM_STATUS_11 { get; set; }
        public int? SUM_STATUS_12 { get; set; }
        public int? SUM_STATUS_13 { get; set; }
        public int? SUM_STATUS_14 { get; set; }
        public int? SUM_TOTAL { get; set; }
    }
}
