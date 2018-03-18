using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
{
    public class TelesalesPerformanceData
    {
        public string Username { get; set; }
        public string EmpCode { get; set; }
        public string Fullname { get; set; }
        public string TelesalesTeam { get; set; }
        public int? LevelId { get; set; }
        public string LevelName { get; set; }
        public decimal? PerformanceAmount { get; set; }
    }
}
