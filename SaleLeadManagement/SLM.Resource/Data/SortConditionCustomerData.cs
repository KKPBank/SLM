using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class SortConditionCustomerData
    {
        public decimal? SortConCusId { get; set; }
        public string ConditionFieldName { get; set; }
        public decimal? SortBy { get; set; }
        public decimal? Seq { get; set; }
    }

    public class SortConditionStaffData
    {
        public decimal? SortConStaffId { get; set; }
        public string ConditionFieldName { get; set; }
        public decimal? SortBy { get; set; }
        public decimal? Seq { get; set; }
    }
}
