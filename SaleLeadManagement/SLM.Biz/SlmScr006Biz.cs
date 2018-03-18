using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class SlmScr006Biz
    {
        public static List<ExistingProductData> SearchExistingProduct(string citizenId)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchExistingProduct(citizenId);
        }
    }
}
