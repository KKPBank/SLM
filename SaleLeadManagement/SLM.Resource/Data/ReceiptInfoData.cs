using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ReceiptInfoData
    {
        public string TicketId { get; set; }
        public string RecNo { get; set; }
        public decimal? RecAmount { get; set; }
        public DateTime? TransDate { get; set; }
    }
}
