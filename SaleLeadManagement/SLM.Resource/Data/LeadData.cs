using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    [Serializable]
    public class LeadData
    {
        public string TicketId { get; set; }
        public string ChannelId { get; set; }

        //**************************************TableName : kkslm_tr_lead **********************************************
        public int? TitleId { get; set; }
        public string Name { get; set; }
        public string TelNo_1 { get; set; }
        public string Ext_1 { get; set; }
        public string CampaignId { get; set; }
        public string Owner { get; set; }
        public string Delegate { get; set; }
        public string Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public string StatusBy { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string AssignedBy { get; set; }
        public string AvailableTime { get; set; }
        public string AssignedFlag { get; set; }
        public int? StaffId { get; set; }
        public decimal? counting { get; set; }
        public string EmailFlag { get; set; }
        public string SmsFlag { get; set; }
        public DateTime? LeadCreateDate { get; set; }
        public decimal? Delegate_Flag { get; set; }
        public string NoteFlag { get; set; }
        public DateTime? Delegate_Date { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Reference { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string slmOldOwner { get; set; }
        public bool HasAdamsUrl { get; set; }
        public string CalculatorUrl { get; set; }
        public string AppNo { get; set; }
        public string ContractNoRefer { get; set; }
        public string ExternalSubStatusDesc { get; set; }
        public DateTime? NextContactDate { get; set; }
        public bool HasNewDelegate { get; set; }
        public bool HasNewOwner { get; set; }
        public string TicketIdRefer { get; set; }

        //**************************************TableName : kkslm_tr_cusinfo **********************************************
        public string CusInfo_Id { get; set; }
        public string CusCode { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string TelNo_2 { get; set; }
        public string TelNo_3 { get; set; }
        public string Ext_2 { get; set; }
        public string Ext_3 { get; set; }
        public string BuildingName { get; set; }
        public string AddressNo { get; set; }
        public string Floor { get; set; }
        public string Soi { get; set; }
        public string Street { get; set; }
        public int? Tambon { get; set; }
        public int? Amphur { get; set; }
        public int? Province { get; set; }
        public string PostalCode { get; set; }
        public int? Occupation { get; set; }
        public string OccupationCode { get; set; }
        public decimal? BaseSalary { get; set; }
        public string IsCustomer { get; set; }
        public DateTime? Birthdate { get; set; }
        public int? CardType { get; set; }
        public string CardTypeDesc { get; set; }
        public string CitizenId { get; set; }
        public string Topic { get; set; }
        public string Detail { get; set; }
        public string Note { get; set; }
        public string PathLink { get; set; }
        public string TelesaleId { get; set; }
        public string TelesaleName { get; set; }
        public string ContactTime { get; set; }
        public string ContactBranch { get; set; }
        public int? CountryId { get; set; }

        //**************************************TableName : kkslm_tr_channelinfo **********************************************
        public string ChannelInfo_Id { get; set; }
        public string IPAddress { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string BranchNo { get; set; }
        public string MachineNo { get; set; }
        public string ClientServiceType { get; set; }
        public string DocumentNo { get; set; }
        public string CommPaidCode { get; set; }
        public string Zone { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestTime { get; set; }
        public string RequestBy { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string TransId { get; set; }

        //**************************************TableName : kkslm_tr_productinfo **********************************************
        public string ProductInfo_Id { get; set; }
        public string InterestedProd { get; set; }
        public string LicenseNo { get; set; }
        public string YearOfCar { get; set; }
        public string YearOfCarRegis { get; set; }
        public int? Brand { get; set; }
        public int? Model { get; set; }
        public string ModelFamily { get; set; }
        public int? Submodel { get; set; }
        public string SubModelCode { get; set; }
        public decimal? DownPayment { get; set; }
        public decimal? DownPercent { get; set; }
        public decimal? CarPrice { get; set; }
        public decimal? FinanceAmt { get; set; }
        public string PaymentTerm { get; set; }
        public string PaymentType { get; set; }
        public string PaymentTypeCode { get; set; }
        public decimal? BalloonAmt { get; set; }
        public decimal? BalloonPercent { get; set; }
        public string CoverageDate { get; set; }
        public int? ProvinceRegis { get; set; }
        public string PlanType { get; set; }
        public int? AccType { get; set; }
        public string AccTypeCode { get; set; }
        public int? AccPromotion { get; set; }
        public string AccPromotionCode { get; set; }
        public string AccTerm { get; set; }
        public string Interest { get; set; }
        public string Invest { get; set; }
        public string LoanOd { get; set; }
        public string LoanOdTerm { get; set; }
        public string Ebank { get; set; }
        public string Atm { get; set; }
        public string OtherDetail_1 { get; set; }
        public string OtherDetail_2 { get; set; }
        public string OtherDetail_3 { get; set; }
        public string OtherDetail_4 { get; set; }
        public string CarType { get; set; }

        //**************************************TableName : kkslm_tr_activity **********************************************
        public string NewOwner { get; set; }
        public string NewStatus { get; set; }
        public string OldOwner { get; set; }
        public string OldStatus { get; set; }
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string WorkId { get; set; }
        public string WorkDesc { get; set; }

        //**************************************TableName : kkslm_tr_activity 2 (ใช้กรณีมีการเปลี่ยน owner ที่หน้าจอ Edit Lead********************************
        public string NewOwner2 { get; set; }
        public string OldOwner2 { get; set; }
        public string Type2 { get; set; }
        public string CreatedBy2 { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public string UpdatedBy2 { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public string WorkId2 { get; set; }
        public string WorkDesc2 { get; set; }
        //************************************** View Data **********************************************
        public string StatusName { get; set; }
        public string CampaignName { get; set; }
        public string ChannelDesc { get; set; }
        public string BranchName { get; set; }
        public string OwnerName { get; set; }
        public string DelegateName { get; set; }
        public DateTime? CreatedDateView { get; set; }
        public DateTime? AssignedDateView { get; set; }
        public string TambolName { get; set; }
        public string AmphurName { get; set; }
        public string ProvinceName { get; set; }
        public string OccupationName { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string SubModelName { get; set; }
        public string ProvinceRegisName { get; set; }
        public string AccTypeName { get; set; }
        public string PromotionName { get; set; }
        public DateTime? ContactLatestDate { get; set; }
        public DateTime? ContactFirstDate { get; set; }
        public string Branchprod { get; set; }
        public string ProvinceCode { get; set; }
        public string AmphurCode { get; set; }
        public string TambolCode { get; set; }
        public string ProvinceRegisCode { get; set; }
        public string BrandCode { get; set; }
        public string Family { get; set; }
        public string DelegatebranchName { get; set; }
        public string PaymentName { get; set; }
        public string PlanBancName { get; set; }
        public string LeadCreateBy { get; set; }
        //public string DelegateOwnerBranch {get; set; }
        public string Description { get; set; }
        public string OldDelegate { get; set; }
        public string NewDelegate { get; set; }
        public string Owner_Branch { get; set; }
        public string Delegate_Branch { get; set; }
        public string CreatedBy_Branch { get; set; }
        public int? CreatedByPositionId { get; set; }
        public string OwnerBranchName { get; set; }
        public int? OwnerPositionId { get; set; }

        //COC
        public string ISCOC { get; set; }
        public string COCCurrentTeam { get; set; }
        public string MarketingOwnerName { get; set; }
        public string LastOwnerName { get; set; }
        public string CocStatusDesc { get; set; }
        public DateTime? CocAssignedDate { get; set; }

        //New
        public string ContractNo { get; set; }
        public string SubProdName { get; set; }

        //OBT
        public decimal? PreleadId { get; set; }
        public string TeamTelesalesCode { get; set; }
        public DateTime? PreleadAssignDate { get; set; }
        public DateTime? PreleadContactLatestDate { get; set; }

        public decimal? slm_RenewInsureId { get; set; }
        public int? slm_NotifyPremiumId { get; set; }
        public decimal? slm_PromotionId { get; set; }
        public int? slm_Seq { get; set; }
        public decimal slm_Ins_Com_Id { get; set; }
        public int? slm_CoverageTypeId { get; set; }
        public decimal? slm_InjuryDeath { get; set; }
        public decimal? slm_TPPD { get; set; }
        public int? slm_RepairTypeId { get; set; }
        public decimal? slm_OD { get; set; }
        public decimal? slm_FT { get; set; }
        public decimal? slm_DeDuctible { get; set; }
        public decimal? slm_PersonalAccident { get; set; }
        public string slm_PersonalAccidentMan { get; set; }
        public decimal? slm_MedicalFee { get; set; }
        public string slm_MedicalFeeMan { get; set; }
        public decimal? slm_InsuranceDriver { get; set; }
        public decimal? slm_PolicyGrossStamp { get; set; }
        public decimal? slm_PolicyGrossVat { get; set; }
        public decimal? slm_PolicyGrossPremium { get; set; }
        public decimal? slm_NetGrossPremium { get; set; }
        public decimal? slm_PolicyGrossPremiumPay { get; set; }
        public decimal? slm_CostSave { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public bool? slm_Selected { get; set; }
        public string slm_OldPolicyNo { get; set; }
        public string slm_DriverFlag { get; set; }
        public int? slm_Driver_TitleId1 { get; set; }
        public string slm_Driver_First_Name1 { get; set; }
        public string slm_Driver_Last_Name1 { get; set; }
        public DateTime? slm_Driver_Birthdate1 { get; set; }
        public int? slm_Driver_TitleId2 { get; set; }
        public string slm_Driver_First_Name2 { get; set; }
        public string slm_Driver_Last_Name2 { get; set; }
        public DateTime? slm_Driver_Birthdate2 { get; set; }
        public string slm_OldReceiveNo { get; set; }
        public DateTime? slm_PolicyStartCoverDate { get; set; }
        public DateTime? slm_PolicyEndCoverDate { get; set; }
        public bool? slm_Vat1Percent { get; set; }
        public decimal? slm_DiscountPercent { get; set; }
        public decimal? slm_DiscountBath { get; set; }
        public decimal? slm_Vat1PercentBath { get; set; }
        public string slm_Year { get; set; }
        public string slm_RedbookBrandCode { get; set; }
        public string slm_RedbookModelCode { get; set; }
        public string slm_RedbookKKKey { get; set; }
        public int? slm_RedbookYearGroup { get; set; }
        public decimal? slm_InsuranceComId { get; set; }
        public decimal? slm_PolicyDiscountAmt { get; set; }
        public decimal? slm_PolicyGrossPremiumTotal { get; set; }
        public decimal? slm_PolicyCost { get; set; }
        public int? slm_PolicyPayMethodId { get; set; }
        public int? slm_PolicyAmountPeriod { get; set; }
        public string slm_Need_CreditFlag { get; set; }
        public string slm_Need_50TawiFlag { get; set; }
        public string slm_Need_DriverLicenseFlag { get; set; }
        public string slm_RemarkPayment { get; set; }
        public decimal? slm_PolicyCostSave { get; set; }
        public int? slm_PayOptionId { get; set; }
        public string slm_PayBranchCode { get; set; }
        public string slm_RemarkPolicy { get; set; }
        public int slm_Version { get; set; }
        public string slm_Type { get; set; }
        public DateTime? slm_PaymentDate { get; set; }
        public int? slm_Period { get; set; }
        public decimal? slm_PaymentAmount { get; set; }
        public string slm_CC { get; set; }
        public string slm_Status { get; set; }
        public string slm_SubStatus { get; set; }
        public string slm_ExternalSubStatusDesc { get; set; }
        public DateTime? slm_StatusDateSource { get; set; }
        public DateTime? slm_StatusDate { get; set; }
        public decimal? slm_CampaignInsuranceId { get; set; }
        public string slm_TelNo_1 { get; set; }
        //Extend
        public string slm_CitizenId { get; set; }
        public int? slm_CardTypeId { get; set; }
        public string slm_CardTypeName { get; set; }
        public string slm_Product_Id { get; set; }
        public string slm_Owner { get; set; }
        public string slm_TicketId { get; set; }

        public string slm_ExternalStatus { get; set; }
        public string slm_ExternalSubStatus { get; set; }

        public string slm_ExternalSystem { get; set; }
    }

    public class LeadDataForUpdate
    {
        public string TicketId { get; set; }
        public string CardTypeDesc { get; set; }
        public int? CardType { get; set; }
        public string CitizenID { get; set; }
        public string TelNo1 { get; set; }
        public string OwnerName { get; set; }
        public string DelegateName { get; set; }
        public string StatusName { get; set; }
        public string Status { get; set; }
        public string Owner { get; set; }
        public string Delegate { get; set; }
        public string OwnerBranchName { get; set; }
        public string OwnerBranch { get; set; }
        public string DelegateBranchName { get; set; }
        public string DelegateBranch { get; set; }
        public string ExternalSubStatusDesc { get; set; }
        public DateTime? ContactLatestDate { get; set; }
    }

    public class LeadDataCollection
    {
        public PreleadData Prev { get; set; }

        public PreleadData Curr { get; set; }

        public PreleadData Promo { get; set; }

        public PreleadData Opt { get; set; }
    }

    public class LeadDataCollectionSaveData
    {
        public PreleadData Prev { get; set; }

        public PreleadData Curr { get; set; }

        public PreleadData Promo1 { get; set; }

        public PreleadData Promo2 { get; set; }

        public PreleadData Promo3 { get; set; }

        public PreleadData Opt { get; set; }
    }

    public class LeadOwnerDelegateData
    {
        public string TicketId { get; set; }
        public string OwnerName { get; set; }
        public string DelegateName { get; set; }
    }

    public class EmailTemplateData
    {
        public string TicketId { get; set; }
        public string CampaignName { get; set; }
        public string Channel { get; set; }
        public string StatusDesc { get; set; }
        public string SubStatusDesc { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string OwnerName { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string DelegateName { get; set; }
        public DateTime? DelegateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string AvailableTime { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string TelNo1 { get; set; }
        public string LicenseNo { get; set; }
        public string CocStatusDesc { get; set; }
        public string MarketingOwnerName { get; set; }
        public string LastOwnerName { get; set; }
    }

    public class LeadDataForAdam
    {
        //LeadInfo
        public string TicketId { get; set; }
        public string Firstname { get; set; }
        public string TelNo1 { get; set; }
        public string Campaign { get; set; }
        public string CampaignName { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }

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
        public string TambolName { get; set; }
        public int? Amphur { get; set; }
        public string AmphurCode { get; set; }
        public string AmphurName { get; set; }
        public int? Province { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public string PostalCode { get; set; }
        public int? Occupation { get; set; }
        public string OccupationCode { get; set; }
        public decimal? BaseSalary { get; set; }
        public string IsCustomer { get; set; }
        public string CustomerCode { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Cid { get; set; }
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
        public int? RegisterProvince { get; set; }          //ProvinceId
        public string RegisterProvinceCode { get; set; }    //ProvinceCode
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

        //Others
        public string AdamsUrl { get; set; }
    }

    public class LeadDataPhoneCallHistory
    {
        public string TicketId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string OwnerBranch { get; set; }
        public string Owner { get; set; }
        public string DelegateBranch { get; set; }
        public string Delegate { get; set; }
        public string TelNo1 { get; set; }
        public string LeadStatus { get; set; }
        public string LeadSubStatus { get; set; }
        public int? CardType { get; set; }
        public string CitizenId { get; set; }
        public int? CountryId { get; set; }
        public string AssignedFlag { get; set; }
        public decimal? DelegateFlag { get; set; }
        public decimal? PreleadId { get; set; }         //Added by Pom 21/05/2016
        public string ChannelId { get; set; }           //Added by Pom 20/05/2016
        public string ProductGroupId { get; set; }      //Added by Pom 20/05/2016
        public string ProductId { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string ContractNo { get; set; }

        public DateTime? NextContactDate { get; set; }
        public string NeedCreditFlag { get; set; }
        public string CreditFileName { get; set; }
        public string Need_50TawiFlag { get; set; }
        public string Tawi50FileName { get; set; }
        public string NeedDriverLicense { get; set; }
        public string DriverLicenseiFileName { get; set; }
        public string NeedPolicyFlag { get; set; }
        public string NeedCompulsoryFlag { get; set; }

        public decimal? blacklist { get; set; }
        public string TelNoSms { get; set; }
    }

    public class EmailTemplateOBTData
    {
        public string ContractNo { get; set; }
        public string TicketId { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Channel { get; set; }
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string SubStatusCode { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string OwnerName { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string TelNo1 { get; set; }
        public DateTime? NextContactDate { get; set; }
    }

    public class NextLeadData
    {
        public string TicketId { get; set; }
        public decimal? PreleadId { get; set; }
        public string CampaignId { get; set; }
        public string ProductId { get; set; }
    }

    public class LeadDataInitialPhoneCall
    {
        public string TicketId { get; set; }
        public decimal? PreleadId { get; set; }
        public int? CardType { get; set; }
        public string CitizenId { get; set; }
        public string TelNo1 { get; set; }
        public string ProductId { get; set; }
        public string Status { get; set; }
        public string ISCOC { get; set; }
        public string COCCurrentTeam { get; set; }
    }

    public class LeadDataForCARLogService
    {
        public string TicketId { get; set; }
        public decimal? PreleadId { get; set; }
        public string ContractNo { get; set; }
        public string ChannelId { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductId { get; set; }
        public string StatusCode { get; set; }
        public string ProductName { get; set; }
        public string StatusName { get; set; }
        public string SubStatusCode { get; set; }
        public string SubStatusName { get; set; }
        public string CitizenId { get; set; }
        public int? CardTypeId { get; set; }
        public string CBSSubScriptionTypeId { get; set; }
        public string CardTypeName { get; set; }
        public string CustomerName { get; set; }
        public string LicenseNo { get; set; }
        public string Owner { get; set; }
        public string OwnerEmail { get; set; }
        public string Delegate { get; set; }
        public string DelegateEmail { get; set; }
        //Insurance
        public string InsuranceCompany { get; set; }
        public decimal? OD { get; set; }                  //ทุนประกันกรณีชน
        public decimal? FT { get; set; }                  //ทุนประกันกรณีไฟไหม้ สูญหาย
        public decimal? DiscountBath { get; set; }
        public decimal? Vat1PercentBath { get; set; }
        public decimal? PolicyGrossPremiumPay { get; set; }
        public bool? IncentiveFlag { get; set; }
        public bool? IncentiveFlagAct { get; set; }

        public DateTime? ActSendDate { get; set; }
        public string ReceiveNo { get; set; }
    }

    public class LeadDataForCARScript
    {
        public string TicketId { get; set; }
        public decimal? PreleadId { get; set; }
        public int? CardType { get; set; }
        public string CitizenId { get; set; }
    }

    [Serializable]
    public class LeadDataForRenewInsure
    {
        public string TicketId { get; set; }
        public string slm_Product_Id { get; set; }
        public int? slm_CardTypeId { get; set; }
        public string slm_CardTypeName { get; set; }
        public string slm_TelNo_1 { get; set; }
        public string slm_CitizenId { get; set; }
        public string slm_Owner { get; set; }
        public int? slm_ProvinceRegis { get; set; }
        public string ClientFullname { get; set; }
    }

    public class LeadDefaultData
    {
        public decimal? PreleadId { get; set; }
        public string TicketId { get; set; }
        public string ContractNo { get; set; }
        public string TeamTelesalesCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TelNo1 { get; set; }
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string ExternalSubStatusDesc { get; set; }
        public string ChannelId { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string InterestedProd { get; set; }
        public string TelNo2 { get; set; }
        public string Ext2 { get; set; }
        public string TelNo3 { get; set; }
        public string Ext3 { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string AssignedFlag { get; set; }
        public decimal? DelegateFlag { get; set; }
        public DateTime? ContactLatestDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? ContactFirstDate { get; set; }
        public DateTime? CocAssignedDate { get; set; }
        public DateTime? PreleadContactLatestDate { get; set; }
        public DateTime? PreleadAssignDate { get; set; }
        public string OwnerBranchCode { get; set; }
        public string OwnerBranchName { get; set; }
        public string OwnerUsername { get; set; }
        public string OwnerName { get; set; }
        public string DelegateBranchCode { get; set; }
        public string DelegateBranchName { get; set; }
        public string DelegateUsername { get; set; }
        public string DelegateName { get; set; }
        public string MarketingOwnerName { get; set; }
        public string LastOwnerName { get; set; }
        public string CocStatusDesc { get; set; }
        public string COCCurrentTeam { get; set; }
        public string Detail { get; set; }
        public int? CardType { get; set; }
        public string CitizenId { get; set; }
        public int? CountryId { get; set; }
        public bool HasAdamsUrl { get; set; }
        public string CalculatorUrl { get; set; }
        public string AppNo { get; set; }
        public string ISCOC { get; set; }
        public int? SubscriptionTypeId { get; set; }
        public string TicketIDRefer { get; set; }
    }
}
