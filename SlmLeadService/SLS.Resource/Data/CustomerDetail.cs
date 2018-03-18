using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource.Data
{
    public class CustomerDetail
    {
        public string Topic { get; set; }
        public string Detail { get; set; }
        public string PathLink { get; set; }
        public string TelesaleName { get; set; }
        public string AvailableTime { get; set; }
        public string ContactBranch { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }

        //2017-03-29 ใช้เพื่อรับ delegate empcode เข้ามา ในกรณีทำ auto assign แทน rule
        public string DelegateUsername { get; set; }
    }
}
