using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class BranchData
    {
        public string IdBranchCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string StartTimeHour { get; set; }
        public string StartTimeMinute { get; set; }
        public string EndTimeHour { get; set; }
        public string EndTimeMinute { get; set; }
        public string ChannelId { get; set; }
        public string ChannelDesc { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public string slm_House_No { get; set; }
        public string slm_Moo { get; set; }
        public string slm_Building { get; set; }
        public string slm_Village { get; set; }
        public string slm_Soi { get; set; }
        public string slm_Street { get; set; }
        public int? slm_TambolId { get; set; }
        public int? slm_AmphurId { get; set; }
        public int? slm_ProvinceId { get; set; }
        public string slm_Zipcode { get; set; }
		public string BranchCodeNew { get; set; }
        public string UpperBranch { get; set; }
    }
}
