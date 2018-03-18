using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
{
    public class RenewInsuranceViewData
    {
        public string TicketId { get; set; }
        public string CampaignId { get; set; }
        public string ProductId { get; set; }
        public string TitleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TelNo1 { get; set; }
        public string CardTypeName { get; set; }
        public string CitizenId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string MaritalStatus { get; set; }
        public string OccupationName { get; set; }
        public string ContractNo { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? PolicyStartDate { get; set; }
        public DateTime? PolicyEndDate { get; set; }
        public decimal? PolicyCost { get; set; }
        public decimal? PolicyGrossPremium { get; set; }
        public decimal? PolicyDiscount { get; set; }
        public string ActNo { get; set; }
        public DateTime? ActStartDate { get; set; }
        public DateTime? ActEndDate { get; set; }
        public decimal? ActGrossPremium { get; set; }
        public decimal? ActVat { get; set; }
        public decimal? ActStamp { get; set; }
        public decimal? ActNetPremium { get; set; }
    }
}
