using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Dal.Models;

namespace SLS.Biz
{
    public class AccessRightBiz
    {
        public static bool CheckAccessRight(string campaignId, string username)
        { 
            SlmMsAccessRightModel ar = new SlmMsAccessRightModel();
            return ar.CheckAccessRight(campaignId, username);
        }

        public static bool CheckAccessRightByTicketId(string ticketId, string username)
        {
            SlmMsAccessRightModel ar = new SlmMsAccessRightModel();
            return ar.CheckAccessRightByTicketId(ticketId, username);
        }
        public static bool ValidateTicketId(string ticketId)
        {
            SlmMsAccessRightModel ar = new SlmMsAccessRightModel();
            return ar.ValidateTicketId(ticketId);
        }
    }
}
