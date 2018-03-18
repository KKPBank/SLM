using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class SlmScr011Biz
    {
        public static LeadOwnerDelegateData GetOwnerAndDelegateName(string ticketId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetOwnerAndDelegateName(ticketId);
        }

        public static List<ControlListData> GetCampaignEditData()
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetCampaignEditData();
        }

        public static List<ControlListData> GetCampaignEditDataNew()
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetCampaignEditDataNew();
        } 

        public static bool GetPrivilegeOwner(string optionCode)
        {
            KKSlmMsPrivilegeOwnerModel privilege = new KKSlmMsPrivilegeOwnerModel();
            return privilege.GetPrivilegeOwnerData(optionCode);
        }

        public static bool CheckBranchAccessRightExist(int flag, string campaignId, string branchcode)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.CheckBranchAccessRightExist(flag, campaignId, branchcode);
        }
    }
}
