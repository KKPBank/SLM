using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource.Data
{
    public class ChannelInfo
    {
        public string ChannelId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string CreateUser { get; set; }
        public string Ipaddress { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string BranchNo { get; set; }
        public string MachineNo { get; set; }
        public string ClientServiceType { get; set; }
        public string DocumentNo { get; set; }
        public string CommPaidCode { get; set; }
        public string Zone { get; set; }
        public string TransId { get; set; }
    }
}
