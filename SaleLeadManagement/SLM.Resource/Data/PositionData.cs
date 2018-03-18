using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class PositionData
    {
        public int? PositionId { get; set; }
        public string PositionNameAbb { get; set; }
        public string PositionNameEN { get; set; }
        public string PositionNameTH { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
    }
}
