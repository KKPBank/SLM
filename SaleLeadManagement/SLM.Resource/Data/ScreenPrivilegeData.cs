using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ScreenPrivilegeData
    {
        public int? ScreenId { get; set; }
        public int? StaffTypeId { get; set; }
        public int? IsSave { get; set; }
        public int? IsView { get; set; }
        public string ScreenDesc { get; set; }
    }
}
