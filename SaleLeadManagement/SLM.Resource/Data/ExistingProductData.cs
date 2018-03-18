using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ExistingProductData
    {
        public string No { get; set; }
        public string CitizenId { get; set; }
        public string ProductGroup { get; set; }
        public string ProductName { get; set; }
        public string Grade { get; set; }
        public string ContactNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? PaymentTerm { get; set; }
        public string Status { get; set; }
    }
}
