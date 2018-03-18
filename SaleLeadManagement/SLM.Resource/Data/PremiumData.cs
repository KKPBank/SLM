using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class PremiumData
    {
        public string TicketId { get; set; }
        public decimal? PremiumId { get; set; }         //Id ของ Master
        public decimal? RaId { get; set; }              //Id ของ Trnsaction
        public string PremiumName { get; set; }
        public string PremiumType { get; set; }
        public int? TotalGive { get; set; }
        public int? TotalRemain { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
