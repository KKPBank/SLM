using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class SlmScr003Biz
    {
        //Reference SlmScr003Biz.cs
        public static List<ControlListData> GetAllActiveCampaignData()
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetAllActiveCampaignData();
        }

		public static List<ControlListData> GetSaleAndBothCampaignData()
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetSaleAndBothCampaignData();
        }
        //Reference LeadInfo.aspx.cs
        public static List<ControlListData> GetCampaignData()
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetCampaignData();
        }

		public static List<ControlListData> GetCampaignDataNew()
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetCampaignDataNew();
        }
        public static List<ControlListData> GetChannelData()
        {
            KKSlmMsChannelModel channel = new KKSlmMsChannelModel();
            return channel.GetChannelData();
        }

        public static List<ControlListData> GetStaffData(int staffTypeId)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffData(staffTypeId);
        }

        public static List<ControlListData> GetOwnerList(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetOwnerList(username);
        }

        public static List<ControlListData> GetOwnerListByCampaignId(string campaignId)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetOwnerListByCampaignId(campaignId);
        }

        public static List<ControlListData> GetOptionList(string optionType)
        {
            KKSlmMsOptionModel option = new KKSlmMsOptionModel();
            return option.GetOptionList(optionType);
        }

        //public static List<SearchLeadResult> SearchLeadData(SearchLeadCondition data, string username, string orderByFlag, out string logError)
        //{
        //    SearchLeadModel search = new SearchLeadModel();
        //    string createDate = data.CreatedDate.Year != 1 ? data.CreatedDate.Year + data.CreatedDate.ToString("-MM-dd") : string.Empty;
        //    string assignDate = data.AssignedDate.Year != 1 ? data.AssignedDate.Year + data.AssignedDate.ToString("-MM-dd") : string.Empty;
        //    string changeStatusDate = data.ChangeStatusDate.Year != 1 ? data.ChangeStatusDate.Year + data.ChangeStatusDate.ToString("-MM-dd") : string.Empty;

        //    return search.SearchLeadData(data.TicketId, data.Firstname, data.Lastname, data.CardType, data.CitizenId, data.CampaignId, data.ChannelId, data.OwnerUsername,
        //        createDate, assignDate, data.StatusList, username, data.StaffType, data.OwnerBranch, data.DelegateBranch, data.DelegateLead, data.ContractNoRefer, 
        //        data.CreateByBranch, data.CreateBy, orderByFlag, data.SubStatus, changeStatusDate, data.ScreenCode, out logError);
        //}

        public static List<SearchLeadResult> SearchLeadSeeAllData(SearchLeadCondition data, string username, string orderByFlag, out string logError)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchLeadSeeAllData(data, username, orderByFlag, out logError);
        }
        
        public static decimal? GetStaffType(string Username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffType(Username);
        }

        public static List<ControlListData> GetBranchData()
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetBranchData();
        }

        public static List<ControlListData> GetProductGroupData()
        {
            CmtMsProductGroupModel productGroup = new CmtMsProductGroupModel();
            return productGroup.GetProductGroupData();
        }

        public static List<ControlListData> GetProductData(string productGroupId)
        {
            CmtMappingProductModel product = new CmtMappingProductModel();
            return product.GetProductData(productGroupId);
        }

		public static List<ControlListData> GetProductDataNew(string productGroupId)
        {
            CmtMappingProductModel product = new CmtMappingProductModel();
            return product.GetProductDataNew(productGroupId);
        } 
        public static List<ControlListData> GetCampaignData(string productGroupId, string productId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetCampaignData(productGroupId, productId);
        }
		public static List<ControlListData> GetCampaignDataNew(string productGroupId, string productId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetCampaignDataNew(productGroupId, productId);
        }

        public static List<ProductData> SearchCampaign(string productGroupId, string productId, string campaignId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.SearchCampaign(productGroupId, productId, campaignId);
        }

        public static List<ProductData> SearchCampaign(string searchWord)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.SearchCampaign(searchWord);
        }

		public static List<ProductData> SearchCampaignNew(string productGroupId, string productId, string campaignId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.SearchCampaignNew(productGroupId, productId, campaignId);
        }

        public static List<ProductData> SearchCampaignNew(string searchWord)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.SearchCampaignNew(searchWord);
        }
        public static List<ProductData> GetProductCampaignData(string campaignId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetProductCampaignData(campaignId);
        }

        public static LeadDataForAdam GetLeadDataForAdam(string ticketId)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.GetLeadDataForAdam(ticketId);
        }

        public static string GetEmployeeCode(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetEmployeeCode(username);
        }

        public static string GetPrivilegeNCB(string productId, decimal staffTypeId)
        {
            KKCocMsAolModel aol = new KKCocMsAolModel();
            return aol.GetPrivilegeNCB(productId, staffTypeId);
        }

        public static StaffData GetStaff(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaff(username);
        }

        public static List<StaffData> GetStaffList()
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffList();
        }

        public static List<ControlListData> GetStaffTypeData()
        {
            KKSlmMsStaffTypeModel  st = new KKSlmMsStaffTypeModel();
            return st.GetStaffTyeData();
        }

        public static bool ValidateTicketIDRefer(string TicketIdRefer)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.ValidateTicketIDRefer(TicketIdRefer);
        }

        public static List<SearchLeadResult> SearchLeadDataNew(SearchLeadCondition data, string username, string orderByFlag, string orderByDirection, out string logError, out int totalRecord)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchLeadDataNew(data, username, orderByFlag, orderByDirection, true, out logError, out totalRecord);
        }
    }
}
