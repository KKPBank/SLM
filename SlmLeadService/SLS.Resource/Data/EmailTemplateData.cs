using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource.Data
{
    public class EmailTemplateData
    {
        public string TicketId { get; set; }
        public string CampaignName { get; set; }
        public string Channel { get; set; }
        public string StatusDesc { get; set; }
        public string SubStatusDesc { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string OwnerName { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string DelegateName { get; set; }
        public DateTime? DelegateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string AvailableTime { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string TelNo1 { get; set; }
        public string LicenseNo { get; set; }
        public string CocStatusDesc { get; set; }
        public string MarketingOwnerName { get; set; }
        public string LastOwnerName { get; set; }
    }
}
