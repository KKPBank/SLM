using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class ConfigProductTabBiz
    {
        public List<ConfigProductTabData> GetTabScreenList(string productId, string campaignId)
        {
            return new KKSlmMsConfigProductTabModel().GetTabScreenList(productId, campaignId);
        }
    }
}
