using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class PreleadViewData
    {
        public decimal? PreleadId { get; set; }
        public string ContractNo { get; set; }
        public string TeamCode { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string TelNo1 { get; set; }
        public string TicketId { get; set; }
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string SubStatusCode { get; set; }
        public string SubStatusDesc { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public DateTime? PreContactLatestDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string BranchCode { get; set; }
        public string OwnerBranchName { get; set; }
        public string OwnerEmpCode { get; set; }
        public string OwnerName { get; set; }
        public int? CardTypeId { get; set; }
        public string CardTyeName { get; set; }
        public string CitizenId { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime? NextContactDate { get; set; }
        public int? SubScriptionTypeId { get; set; }
        public string CarLicenseNo { get; set; }
        public string TelNoSms { get; set; }
    }
}
