using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class SlmScr007Biz
    {
        public static List<OwnerLoggingData> SearchOwnerLogging(string ticketId, string preleadId)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchOwnerLogging(ticketId, preleadId);
        }
    }
}
