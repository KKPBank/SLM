using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class OwnerLoggingData
    {
        public string No { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TicketId { get; set; }
        public string CitizenId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string OldStatusDesc { get; set; }
        public string OldSubStatusDesc { get; set; }
        public string OldOwnerName { get; set; }
        public string NewStatusDesc { get; set; }
        public string NewSubStatusDesc { get; set; }
        public string NewOwnerName { get; set; }
        public string OldDelegateName { get; set; }
        public string NewDelegateName { get; set; }
        public string Action { get; set; }
        public string CreateBy { get; set; }
        public string SystemAction { get; set; }
        public decimal? ActivityId { get; set; }        //Add by Pom 01/09/2016
    }
    
}
