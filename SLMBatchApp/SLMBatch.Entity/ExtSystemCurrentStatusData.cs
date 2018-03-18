using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
{
    public class ExtSystemCurrentStatusData
    {
        public string TicketId { get; set; }
        public string CampaignId { get; set; }
        public string StatusSystem { get; set; }
        public string StatusCode { get; set; }
        public string SubStatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusBy { get; set; }
        public DateTime? StatusDate { get; set; }
        public string StatusDesc { get; set; }
        public string Campaign { get; set; }
        public string Channel { get; set; }
        public string SlmStatus { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}
