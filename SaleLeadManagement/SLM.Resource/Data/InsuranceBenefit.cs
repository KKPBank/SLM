using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Resource.Data
{
    public class InsuranceBenefit
    {
        public decimal BenefitId { get; set; }
        public string Product_Id { get; set; }
        public decimal Ins_Com_Id { get; set; }
        public decimal CampaignInsuranceId { get; set; }
        public string ComissionFlag { get; set; }
        public decimal? ComissionPercentValue { get; set; }
        public decimal? ComissionBathValue { get; set; }
        public string OV1Flag { get; set; }
        public decimal? OV1PercentValue { get; set; }
        public decimal? OV1BathValue { get; set; }
        public string OV2Flag { get; set; }
        public decimal? OV2PercentValue { get; set; }
        public decimal? OV2BathValue { get; set; }
        public string VatFlag { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool is_Deleted { get; set; }

        public string InsComName { get; set; }
        public string ProductName { get; set; }
        public string CampaignName { get; set; }

        public int? CoverageTypeId { get; set; }
        public string BenefitTypeCode { get; set; }
        public int? InsurancecarTypeId { get; set; }

        public string BenefitTypeDesc { get; set; }
        public string InsuranceTypeDesc { get; set; }

        private const string NumberFormatDisplay = "#,##0.00";
        public string ComissionPercentValueDisplay
        {            
            get
            {
                string strReturn = string.Empty;
                if (ComissionPercentValue.HasValue)
                {
                    strReturn = ComissionPercentValue.Value.ToString(NumberFormatDisplay);
                }
                return strReturn;
            }
        }
        public string ComissionBathValueDisplay
        {
            get
            {
                string strReturn = string.Empty;
                if (ComissionBathValue.HasValue)
                {
                    strReturn = ComissionBathValue.Value.ToString(NumberFormatDisplay);
                }
                return strReturn;
            }
        }
        public string OV1PercentValueDisplay
        {
            get
            {
                string strReturn = string.Empty;
                if (OV1PercentValue.HasValue)
                {
                    strReturn = OV1PercentValue.Value.ToString(NumberFormatDisplay);
                }
                return strReturn;
            }
        }
        public string OV1BathValueDisplay
        {
            get
            {
                string strReturn = string.Empty;
                if (OV1BathValue.HasValue)
                {
                    strReturn = OV1BathValue.Value.ToString(NumberFormatDisplay);
                }
                return strReturn;
            }
        }
        public string OV2PercentValueDisplay
        {
            get
            {
                string strReturn = string.Empty;
                if (OV2PercentValue.HasValue)
                {
                    strReturn = OV2PercentValue.Value.ToString(NumberFormatDisplay);
                }
                return strReturn;
            }
        }
        public string OV2BathValueDisplay
        {
            get
            {
                string strReturn = string.Empty;
                if (OV2BathValue.HasValue)
                {
                    strReturn = OV2BathValue.Value.ToString(NumberFormatDisplay);
                }
                return strReturn;
            }
        }
    }
}
