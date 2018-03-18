using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class UserManagementData
    {
        public string  BankCode { get; set; }
        public string MarketCode { get; set; }
        public string UserId { get; set; }
        public string FullNameTh { get; set; }
        public string Position { get; set; }
        public string RoleName { get; set; }
        public string  MarketTeam { get; set; }
        public string BranchName { get; set; }
        public string StatusName { get; set; }
        public int? StaffId { get; set; }
    }

    public class UserMonitoringMKTData
    {
        public string No { get; set; }
        public string RoleName { get; set; }
        public string FullnameTH { get; set; }
        public string Username { get; set; }
        public decimal? Active { get; set; }
        public int? SUM_STATUS_00 { get; set; }
        public int? SUM_STATUS_05 { get; set; }
        public int? SUM_STATUS_06 { get; set; }
        public int? SUM_STATUS_07 { get; set; }
        public int? SUM_STATUS_11 { get; set; }
        public int? SUM_STATUS_14 { get; set; }
        public int? SUM_STATUS_15 { get; set; }
        public int? SUM_TOTAL { get; set; }
    }
}
