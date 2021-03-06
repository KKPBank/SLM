﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class InsuranceData
    {
        public string TypeInsurance { get; set; }
        public string slm_TicketId { get; set; }
        public string slm_PolicyNo{ get; set; }
        public string slm_PolicySaleType{ get; set; }
        public DateTime? slm_PolicyStartCoverDate{ get; set; }
        public DateTime? slm_PolicyEndCoverDate{ get; set; }
        public int? slm_PolicyIssuePlace{ get; set; }
        public string slm_PolicyIssueBranch{ get; set; }
        public bool? slm_ActPurchaseFlag{ get; set; }
        public string slm_ActNo{ get; set; }
        public string slm_ActStartCoverDate{ get; set; }
        public string slm_ActEndCoverDate{ get; set; }
        public string slm_ActIssuePlace{ get; set; }
        public string slm_ActIssueBranch{ get; set; }
        public string slm_ContractNo{ get; set; }
        public string slm_CC{ get; set; }
        public string slm_RedbookBrandCode{ get; set; }
        public string slm_RedbookModelCode{ get; set; }
        public int? slm_RedbookYearGroup{ get; set; }
        public string slm_RedbookKKKey{ get; set; }
        public string slm_RedbookBrandCodeExt{ get; set; }
        public string slm_RedbookModelCodeExt{ get; set; }
        public int? slm_RedbookYearGroupExt{ get; set; }
        public string slm_RedbookKKKeyExt{ get; set; }
        public int slm_CoverageTypeId{ get; set; }
        public string slm_LicenseNo{ get; set; }
        public string slm_ChassisNo{ get; set; }
        public string slm_EngineNo{ get; set; }
        public string slm_CreatedBy{ get; set; }
        public DateTime? slm_CreatedDate{ get; set; }
        public string slm_UpdatedBy{ get; set; }
        public DateTime? slm_UpdatedDate{ get; set; }
        public string  slm_ReceiveNo{ get; set; }
        public DateTime? slm_ReceiveDate{ get; set; }
        public decimal? slm_InsuranceComId{ get; set; }
        public decimal? slm_PolicyDiscountAmt { get; set; }
        public decimal? slm_PolicyGrossVat { get; set; }
        public decimal? slm_PolicyGrossStamp { get; set; }
        public decimal? slm_PolicyGrossPremium { get; set; }
        public decimal? slm_PolicyGrossPremiumTotal { get; set; }
        public decimal? slm_PolicyNetGrossPremium { get; set; }
        public decimal? slm_PolicyCost { get; set; }
        public int? slm_RepairTypeId{ get; set; }
        public int? slm_PolicyPayMethodId{ get; set; }
        public int? slm_PolicyAmountPeriod{ get; set; }
        public decimal? slm_ActGrossPremium { get; set; }
        public decimal? slm_ActComId { get; set; }
        public decimal? slm_ActNetPremium { get; set; }
        public decimal? slm_ActVat { get; set; }
        public decimal? slm_ActStamp { get; set; }
        public int? slm_ActPayMethodId{ get; set; }
        public int? slm_ActamountPeriod{ get; set; }
        public bool? slm_ObtPro08Flag{ get; set; }
        public bool? slm_IncentiveFlag{ get; set; }
        public DateTime? slm_IncentiveDate{ get; set; }
        public string slm_Need_CreditFlag{ get; set; }
        public string slm_Need_50TawiFlag{ get; set; }
        public string slm_Need_DriverLicenseFlag{ get; set; }
        public string slm_CreditFileName{ get; set; }
        public string slm_CreditFilePath{ get; set; }
        public int? slm_CreditFileSize{ get; set; }
        public string slm_50TawiFileName{ get; set; }
        public string slm_50TawiFilePath{ get; set; }
        public int? slm_50TawiFileSize{ get; set; }
        public string slm_DriverLicenseiFileName{ get; set; }
        public string slm_DriverLicenseFilePath{ get; set; }
        public int? slm_DriverLicenseFileSize{ get; set; }
        public string slm_Need_CompulsoryFlag{ get; set; }
        public DateTime? slm_Need_CompulsoryFlagDate{ get; set; }
        public string slm_Need_PolicyFlag{ get; set; }
        public DateTime? slm_Need_PolicyFlagDate{ get; set; }
        public string slm_RemarkPayment{ get; set; }
        public DateTime? slm_Voluntary_First_Create_Date{ get; set; }
        public decimal? slm_PolicyCostSave { get; set; }
        public bool? slm_Vat1Percent{ get; set; }
        public decimal? slm_DiscountPercent { get; set; }
        public decimal? slm_Vat1PercentBath { get; set; }
        public string slm_Grade{ get; set; }
        public string slm_SendDocFlag{ get; set; }
        public string slm_SendDocBrandCode{ get; set; }
        public string slm_Receiver{ get; set; }
        public int? slm_PayOptionId{ get; set; }
        public string slm_PayBranchCode{ get; set; }
        public string slm_RemarkPolicy{ get; set; }
        public string slm_RemarkAct{ get; set; }
        public int? slm_ActPayOptionId{ get; set; }
        public string slm_ActPayBranchCode{ get; set; }
        public bool? slm_ActIncentiveFlag{ get; set; }
        public DateTime? slm_ActIncentiveDate{ get; set; }
        public decimal? slm_PolicyComBath { get; set; }
        public decimal? slm_PolicyComBathVat { get; set; }
        public decimal? slm_PolicyComBathIncentive { get; set; }
        public decimal? slm_PolicyOV1Bath { get; set; }
        public decimal? slm_PolicyOV1BathVat { get; set; }
        public decimal? slm_PolicyOV1BathIncentive { get; set; }
        public decimal? slm_PolicyOV2Bath { get; set; }
        public decimal? slm_PolicyOV2BathVat { get; set; }
        public decimal? slm_PolicyOV2BathIncentive { get; set; }
        public decimal? slm_PolicyIncentiveAmount { get; set; }
        public decimal? slm_ActComBath { get; set; }
        public decimal? slm_ActComBathVat { get; set; }
        public decimal? slm_ActComBathIncentive { get; set; }
        public decimal? slm_ActOV1Bath { get; set; }
        public decimal? slm_ActOV1BathVat { get; set; }
        public decimal? slm_ActOV1BathIncentive { get; set; }
        public decimal? slm_ActOV2Bath { get; set; }
        public decimal? slm_ActOV2BathVat { get; set; }
        public decimal? slm_ActOV2BathIncentive { get; set; }
        public decimal? slm_ActIncentiveAmount { get; set; }
        public string slm_ClaimFlag{ get; set; }
        public DateTime? slm_ActSendDate { get; set; }

    }
}
