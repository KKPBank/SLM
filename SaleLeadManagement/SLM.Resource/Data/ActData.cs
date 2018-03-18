using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ActData
    {
        public decimal slm_PreLead_Id { get; set; }

        public decimal slm_PromotionId { get; set; }

        public string slm_PreLeadCompareActId { get; set; }

        public string BranchName { get; set; }

        public string BranchCode { get; set; }

        public decimal? InsId { get; set; }

        public string InsNameTh { get; set; }

        public int? slm_Seq{get;set;}

        public string slm_Year{get;set;}

        public decimal slm_Ins_Com_Id{get;set;}

        public int? slm_ActIssuePlace{get;set;}

        public int? slm_SendDocType { get; set; }
        public string slm_ActNo { get; set; }
        public DateTime? slm_ActStartCoverDate { get; set; }
        public DateTime? slm_ActEndCoverDate { get; set; }
        public string slm_ActIssueBranch { get; set; }
        public DateTime? slm_CarTaxExpiredDate { get; set; }
        public decimal? slm_ActGrossStamp { get; set; }
        public decimal? slm_ActGrossVat { get; set; }
        public decimal slm_ActGrossPremium { get; set; }
        public decimal? slm_ActNetGrossPremium { get; set; }
        public decimal? slm_ActGrossPremiumPay { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public bool? slm_ActPurchaseFlag { get; set; }
        public decimal? slm_DiscountPercent { get; set; }
        public decimal? slm_DiscountBath { get; set; }
        public string slm_ActSignNo { get; set; }
        public decimal? slm_ActNetGrossPremiumFull { get; set; }
        public int slm_Vat1Percent { get; set; }
        public decimal? slm_Vat1PercentBath { get; set; }
        public decimal? slm_RenewInsureId { get; set; }
        //public decimal? slm_PromotionId { get; set; }
        //public decimal? slm_Seq { get; set; }             //Comment on 29/04/2016 ชื่อซ้ำด้านบน
        //public decimal? slm_Year { get; set; }            //Comment on 29/04/2016 ชื่อซ้ำด้านบน
        public int slm_Version { get; set; }
        public decimal? slm_ActComId { get; set; }
        public decimal? slm_ActNetPremium { get; set; }
        public decimal? slm_ActVat { get; set; }
        public decimal? slm_ActStamp { get; set; }
        public int? slm_ActPayMethodId { get; set; }
        public int? slm_ActamountPeriod { get; set; }
        public int? slm_ActPayOptionId { get; set; }
        public string slm_ActPayBranchCode { get; set; }
        public string slm_Type { get; set; }
        public DateTime? slm_PaymentDate { get; set; }
        public int? slm_Period { get; set; }
        public decimal? slm_PaymentAmount { get; set; }
        


    }

    public class ActDataCollection
    {
        public ActData Prev { get; set; }

        public ActData Promo { get; set; }

        public ActData Opt { get; set; }
    }

    public class ActDataCollectionForSave
    {
        public ActData Prev { get; set; }

        public ActData Promo1 { get; set; }

        public ActData Promo2 { get; set; }

        public ActData Promo3 { get; set; }

        public ActData Opt { get; set; }
    }
}
