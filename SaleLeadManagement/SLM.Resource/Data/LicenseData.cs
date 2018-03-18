using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class LicenseData
    {
        public string LicenseTypeDesc { get; set; }
        public string LicenseNo { get; set; }
        public DateTime? LicenseBeginDate { get; set; }
        public DateTime? LicenseExpireDate { get; set; }
    }
}
