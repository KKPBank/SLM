using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class NotificationData
    {
        public decimal? NotificationId { get; set; }
        public string TicketId { get; set; }
        public int? NotificationType { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime? NotificationDate { get; set; }
    }
}
