using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;
namespace SLM.Biz
{
    public class SlmScr016Biz
    {
        public static List<StaffData> GetChannelStaffData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetChannelStaffData(username);
        }

        public static StaffData GetStaffData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffData(username);
        }

        public static List<ProductData> GetProductCampaignData(string campaignId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetProductCampaignData(campaignId);
        }

        public static List<ProductData> GetProductCampaignDataForSuggestCampaign(string campaignId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetProductCampaignDataForSuggestCampaign(campaignId);
        }

        public static string GetTableName(string campaignId, string productId, string actionType)
        {
            KKSlmMsConfigProductScreenModel product = new KKSlmMsConfigProductScreenModel();
            return product.GetTableName(campaignId, productId, actionType);
        }

        public static void InsertRenewInsuranceData(string ticketId, string createByUsername, DateTime createDate)
        {
            KKSlmTrRenewinsuranceModel reinsur = new KKSlmTrRenewinsuranceModel();
            reinsur.InsertData(ticketId, createByUsername, createDate);
        }

        public static List<ControlListData> GetMappingProductNew(string campaignId)
        {
            CmtMappingProductModel product = new CmtMappingProductModel();
            return product.GetMappingProductNew(campaignId);
        }

        //public static List<CampaignReferData> GetCampaignReferMockupData()
        //{
        //    SearchLeadModel search = new SearchLeadModel();
        //    return search.SearchCampaignReferMockup();
        //}

        //public static List<CampaignReferData> GetCampaignReferHistoryMockupData()
        //{
        //    SearchLeadModel search = new SearchLeadModel();
        //    return search.SearchCampaignReferHistoryMockup();
        //}
    }
}
