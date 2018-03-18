using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class SlmScr005Biz
    {
        public static List<SearchLeadResult> SearchExistingLead(string citizenId, string ticketId)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchExistingLead(citizenId, ticketId);
        }
    }
}
