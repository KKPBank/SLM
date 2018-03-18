using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class SearchRankingResult
    {
        public int RankingId { get; set; }
        public int Seq { get; set; }

        public string Name { get; set; }
        public Nullable<int> SkipDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
