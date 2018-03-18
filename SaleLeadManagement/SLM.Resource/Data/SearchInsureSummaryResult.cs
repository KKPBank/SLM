using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class SearchInsureSummaryResult
    {
        public string Grade { get; set; }
        public string CarCategory { get; set; }
        public string VoluntaryChannelKey { get; set; }
        public string VoluntaryPolicyNumber { get; set; }
        public Decimal? VoluntaryCovAmt { get; set; }
        public string VoluntaryCompanyName { get; set; }
        public string VoluntaryType { get; set; }
        public Decimal? VoluntaryGrossPremium { get; set; }
        public DateTime? VoluntaryFirstCreateDate { get; set; }
        public DateTime? VoluntaryPolicyEffDate { get; set; }
        public DateTime? VoluntaryPolicyExpDate { get; set; }
        public string CompulsoryCompanyName { get; set; }
        public Decimal? CompulsoryGrossPremium { get; set; }
        public DateTime? CompulsoryPolicyEffDate { get; set; }
        public DateTime? CompulsoryPolicyExpDate { get; set; }
        public string actno { get; set; }
        public bool IsExportExpired { get; set; }
        public DateTime? IsExportExpiredDate { get; set; }
    }
}
