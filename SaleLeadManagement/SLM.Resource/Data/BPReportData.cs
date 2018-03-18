using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class BPReportData
    {
        public BPReportData()
        {
            PolicyItemList = new List<BPItem>();
        }

        public bool SaveBPReport { get; set; }
        public bool IssuePolicy { get; set; }
        public decimal? PolicyAmountDue { get; set; }
        public int? PolicyPaymentMethodId { get; set; }
        public List<BPItem> PolicyItemList { get; set; }
        public bool IssueAct { get; set; }
        public decimal? ActAmountDue { get; set; }
        public int? ActPaymentMethodId { get; set; }
        public BPItem ActItem { get; set; }
    }

    public class BPItem
    {
        public int? No { get; set; }
        public decimal? Amount { get; set; }
    }
}
