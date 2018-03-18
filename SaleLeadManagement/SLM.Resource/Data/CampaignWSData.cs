using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class CampaignWSData
    {
        public decimal? CampaignFinalId { get; set; }
        public string CampaignId { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public string CampaignDetail { get; set; }
        public string  ChannelName { get; set; }
        public string OfferName { get; set; }
        public string OfferDate { get; set; }
        public string Interest { get; set; }
        public string TicketId { get; set; }
        public string Action { get; set; }
        public string ActionName { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal  GroupID { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string StartDate { get; set; }
        public string HasOffered { get; set; }
        public string IsInterested { get; set; }
        public string UpdatedBy { get; set; }
        public string BranchName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string  CampaignDetailCut { get; set; }
    }
    public class CampaignDetail
    {
        public string Offer { get; set; }
        public string Criteria { get; set; }
    }

    public class CmtRecommandCampaign
    {
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignDesc { get; set; }
        public string CampaignFullDesc { get; set; }
        public string CampaignOffer { get; set; }
        public string CampaignCriteria { get; set; }
        public DateTime StartDate { get; set; }           
        public DateTime ExpireDate { get; set; }         
        public string ChannelId { get; set; }
        public string ChannelDesc { get; set; }
        public string CampaignScore { get; set; }
        public string HasOffered { get; set; }
        public string IsInterested { get; set; }
        public string SaleToolkitUrl { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Result { get; set; }
        public string HtmlTag { get; set; }
        public string IntegrateSLM { get; set; }
        public string SlaTime { get; set; }
        public string DescCust { get; set; }
        public string Attachments { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string Title { get; set; }
        public string CitizenId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string TelNo1 { get; set; }
        public string Email { get; set; }
        public string ContractNoRefer { get; set; }
        public string ContractOriginalSystem { get; set; }
        public string Remark { get; set; }
        public string Assignment { get; set; }
        public string StaffOfferChannelDesc { get; set; }        //ช่องทางเจ้าหน้าที่ผู้นำเสนอ
        public string StaffOfferBranchName { get; set; }         //สาขาของเจ้าหน้าที่ผู้นำเสนอ
        public string StaffOfferName { get; set; }               //ผู้นำเสนอ
    }
    
}
