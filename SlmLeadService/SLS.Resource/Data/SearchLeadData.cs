using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource.Data
{
    public class SearchLeadData
    {
        //Mandatory
        public string TicketId { get; set; }
        public string Firstname { get; set; }
        public string TelNo1 { get; set; }
        public string Campaign { get; set; }

        //Lead
        public string CampaignName { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ContractNoRefer { get; set; }

        //CustomerInfo
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string TelNo2 { get; set; }
        public string TelNo3 { get; set; }
        public string ExtNo1 { get; set; }
        public string ExtNo2 { get; set; }
        public string ExtNo3 { get; set; }
        public string BuildingName { get; set; }
        public string AddrNo { get; set; }
        public string Floor { get; set; }
        public string Soi { get; set; }
        public string Street { get; set; }
        public int? Tambol { get; set; }
        public string TambolCode { get; set; }
        public int? Amphur { get; set; }
        public string AmphurCode { get; set; }
        public int? Province { get; set; }
        public string ProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public int? Occupation { get; set; }
        public string OccupationCode { get; set; }
        public decimal? BaseSalary { get; set; }
        public string IsCustomer { get; set; }
        public string CustomerCode { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CardTypeDesc { get; set; }
        public string Cid { get; set; }
        public int? CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryDescriptionEN { get; set; }
        public string CountryDescriptionTH { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }

        //CustomerDetail
        public string Topic { get; set; }
        public string Detail { get; set; }
        public string PathLink { get; set; }
        public string TelesaleName { get; set; }
        public string AvailableTime { get; set; }
        public string ContactBranch { get; set; }

        //ProductInfo
        public string InterestedProdAndType { get; set; }
        public string LicenseNo { get; set; }
        public string YearOfCar { get; set; }
        public string YearOfCarRegis { get; set; }
        public int? RegisterProvince { get; set; }
        public string RegisterProvinceCode { get; set; }
        public int? Brand { get; set; }
        public string BrandCode { get; set; }
        public int? Model { get; set; }
        public string ModelFamily { get; set; }
        public int? Submodel { get; set; }
        public string SubmodelRedBookNo { get; set; }
        public decimal? DownPayment { get; set; }
        public decimal? DownPercent { get; set; }
        public decimal? CarPrice { get; set; }
        public string CarType { get; set; }
        public decimal? FinanceAmt { get; set; }
        public string Term { get; set; }
        public string PaymentType { get; set; }
        public string PaymentTypeCode { get; set; }
        public decimal? BalloonAmt { get; set; }
        public decimal? BalloonPercent { get; set; }
        public string Plantype { get; set; }
        public string CoverageDate { get; set; }
        public int? AccType { get; set; }
        public string AccTypeCode { get; set; }
        public int? AccPromotion { get; set; }
        public string AccPromotionCode { get; set; }
        public string AccTerm { get; set; }
        public string Interest { get; set; }
        public string Invest { get; set; }
        public string LoanOd { get; set; }
        public string LoanOdTerm { get; set; }
        public string SlmBank { get; set; }
        public string SlmAtm { get; set; }
        public string OtherDetail1 { get; set; }
        public string OtherDetail2 { get; set; }
        public string OtherDetail3 { get; set; }
        public string OtherDetail4 { get; set; }

        //ChannelInfo
        public string ChannelId { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Time { get; set; }
        public string CreateUser { get; set; }
        public string Ipaddress { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string BranchNo { get; set; }
        public string MachineNo { get; set; }
        public string ClientServiceType { get; set; }
        public string DocumentNo { get; set; }
        public string CommPaidCode { get; set; }
        public string Zone { get; set; }
        public string TransId { get; set; }
    }
}
