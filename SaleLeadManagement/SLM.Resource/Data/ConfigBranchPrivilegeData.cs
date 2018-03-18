using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ConfigBranchPrivilegeData
    {
        public string BranchCode { get; set; }
        public bool? IsView { get; set; }
        public bool? IsEdit { get; set; }
    }
}
