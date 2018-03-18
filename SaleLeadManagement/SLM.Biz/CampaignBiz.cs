using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class CampaignBiz
    {
        public static List<ControlListData> GetCampaignList(string campaignType)
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetCampaignList(campaignType);
        }
		public static List<ControlListData> GetCampaignListNew(string campaignType)
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetCampaignListNew(campaignType);
        }
    }
}
