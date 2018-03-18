using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
{
    public class DeDupGroupByData
    {

    }

    public class DedupData
    {
        public decimal? DedupId { get; set; }
        public string ProductId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EngineNo { get; set; }
        public string ChassisNo { get; set; }
        public string ChassisNoCleaned { get; set; }
    }
}
