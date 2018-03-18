using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class SearchObtResult
    {
        public Decimal? PreleadId { get; set; }
        public decimal? Counting { get; set; }
        public DateTime? NextContactDate { get; set; }
        public int? PolicyExpirationYear { get; set; }
        public int? PolicyExpirationMonth { get; set; }
        public string PeriodYear { get; set; }
        public string PeriodMonth { get; set; }
        public string ContractNo { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string StatusDesc { get; set; }
        public string SubStatusDesc { get; set; }
        public DateTime? PolicyExpireDate { get; set; }
        public string CardTypeName { get; set; }
        public string TicketId { get; set; }
        public string CitizenId { get; set; }
        public string CampaignName { get; set; }
        public string ChannelDesc { get; set; }
        public string OwnerName { get; set; }
        public string DelegateName { get; set; }
        public string CreaterName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? DelegateDate { get; set; }
        public string OwnerBranchName { get; set; }
        public string DelegateBranchName { get; set; }
        public string CreaterBranchName { get; set; }
        public string ContractNoRefer { get; set; }
        public string NoteFlag { get; set; }
        public string CalculatorUrl { get; set; }
        public bool? HasAdamUrl { get; set; }
        public string AppNo { get; set; }
        public string ProductId { get; set; }
        public string IsCOC { get; set; }
        public string COCCurrentTeam { get; set; }
        public string StatusCode { get; set; }
        public string SubStatusCode { get; set; }
        public string CampaignId { get; set; }
        public string LicenseNo { get; set; }
        public DateTime? NextSLA { get; set; }
        public string ResultFlag { get; set; }
        public string Grade { get; set; }

        //may not use
        //public int SEQ { get; set; }
        //public string ProductGroupId { get; set; }
        //
        //public string ProductName { get; set; }
        //
        //
        //
        //public string TelNo1 { get; set; }
        //public int? ProvinceRegis { get; set; }
        //
        //
        //
        //
        //
        //public string ExternalSubStatusDesc { get; set; }
        //public string TranferType { get; set; }


        //public string RecordType { get; set; }
    }

    public class SearchUserMonitoringOBTResult
    {
        public Decimal? PreleadId { get; set; }
        public decimal? Counting { get; set; }
        public DateTime? NextContactDate { get; set; }
        public int? PolicyEffectiveYear { get; set; }
        public int? PolicyEffectiveMonth { get; set; }
        public string ContractNo { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string StatusDesc { get; set; }
        public string SubStatusDesc { get; set; }
        public DateTime? PolicyExpireDate { get; set; }
        public string CardTypeName { get; set; }
        public string TicketId { get; set; }
        public string CitizenId { get; set; }
        public string CampaignName { get; set; }
        public string ChannelDesc { get; set; }
        public string OwnerName { get; set; }
        public string DelegateName { get; set; }
        public string CreaterName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string OwnerBranchName { get; set; }
        public string DelegateBranchName { get; set; }
        public string CreaterBranchName { get; set; }
        public string ContractNoRefer { get; set; }
        public string NoteFlag { get; set; }
        public string CalculatorUrl { get; set; }
        public bool? HasAdamUrl { get; set; }
        public string AppNo { get; set; }
        public string ProductId { get; set; }
        public string IsCOC { get; set; }
        public string COCCurrentTeam { get; set; }
        public string StatusCode { get; set; }
        public string SubStatusCode { get; set; }
        public string CampaignId { get; set; }
        public string LicenseNo { get; set; }
        public DateTime? NextSLA { get; set; }
        public string ResultFlag { get; set; }
        public string TranferType { get; set; }
        public string CardTypeDesc { get; set; }
        public string TEAMCODE { get; set; }
        public string EMPCODE { get; set; }
        public string CusFullName { get; set; }
        public string INSNAME { get; set; }
        public string COV_NAME { get; set; }
        public decimal? GROSS_PREMIUM { get; set; }
        public String GRADE { get; set; }
        public decimal? Lot { get; set; }
        public string EXP_MONTH { get; set; }
        public DateTime? EXPIRE_DATE { get; set; }
        public DateTime? NEXT_CONTRACT_DATE { get; set; }


        public string slm_AssignDescription { get; set; }
        public string slm_Car_By_Gov_Name_Org { get; set; }
        public string slm_Brand_Name_Org { get; set; }
        public string slm_Model_name_Org { get; set; }
        public string slm_Voluntary_Policy_Number { get; set; }


        public bool? slm_IsExportExpired { get; set; }
        public DateTime? slm_IsExportExpiredDate { get; set; }
        public string slm_BranchCode { get; set; }
        public string slm_FlagNotifyPremium { get; set; }
        public decimal? slm_PolicyGrossPremium { get; set; }
        public DateTime? slm_LatestInsExpireDate { get; set; }
        public string slm_Chassis_No { get; set; }
        public string slm_Engine_No { get; set; }
        public string slm_Car_License_No { get; set; }

        public string slm_ProvinceRegis_Org { get; set; }
        //may not use
        //public int SEQ { get; set; }
        //public string ProductGroupId { get; set; }
        //
        //public string ProductName { get; set; }
        //
        //
        //
        //public string TelNo1 { get; set; }
        //public int? ProvinceRegis { get; set; }
        //
        //
        //
        //
        //
        //public string ExternalSubStatusDesc { get; set; }


        //public string RecordType { get; set; }
    }
}
