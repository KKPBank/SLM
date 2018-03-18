using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class SlmScr035Biz
    {
        public static List<InsuranceBenefit> GetBenefitSearch(string productId, decimal insComId, decimal campaignId, string BenefitTypeCode, decimal coverageTypeId, decimal insuranceCarTypeId, string status)
        {
            List<InsuranceBenefit> result = new List<InsuranceBenefit>();
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                IQueryable<kkslm_ms_benefit> query = operdb.kkslm_ms_benefit;
                if (productId != "" && productId != "-1")
                {
                    query = query.Where(b => b.slm_Product_Id == productId);
                }
                if (insComId != -1)
                {
                    query = query.Where(b => b.slm_Ins_Com_Id == insComId);
                }
                if (campaignId != -1)
                {
                    query = query.Where(b => b.slm_CampaignInsuranceId == campaignId);
                }
                
                result = (from b in query
                          join ic in operdb.kkslm_ms_ins_com on b.slm_Ins_Com_Id equals ic.slm_Ins_Com_Id
                          join cam in operdb.kkslm_ms_campaigninsurance on b.slm_CampaignInsuranceId equals cam.slm_CampaignInsuranceId into qCam                          
                          from c in qCam.DefaultIfEmpty()
                          where (
                                   (coverageTypeId == -1 && insuranceCarTypeId == -1) ||
                                   (b.slm_BenefitTypeCode == "204" &&
                                    (coverageTypeId == -1 || (b.slm_CoverageTypeId != null &&
                                                               b.slm_InsurancecarTypeId == null &&
                                                               b.slm_CoverageTypeId == coverageTypeId))) ||
                                   (b.slm_BenefitTypeCode == "205" &&
                                    (insuranceCarTypeId == -1 || (b.slm_CoverageTypeId == null &&
                                                                    b.slm_InsurancecarTypeId != null &&
                                                                    b.slm_InsurancecarTypeId == insuranceCarTypeId)))
                                )
                               && (BenefitTypeCode == string.Empty || BenefitTypeCode.IndexOf(b.slm_BenefitTypeCode) >= 0)
                               && (status == string.Empty || status.IndexOf(b.is_Deleted ? "1" : "0") >=0)
                          select new InsuranceBenefit()
                          {
                              BenefitId = b.slm_BenefitId,
                              CampaignInsuranceId = b.slm_CampaignInsuranceId ?? -1,
                              CampaignName = c.slm_CampaignName ?? "",
                              ComissionBathValue = b.slm_ComissionBathValue,
                              ComissionFlag = b.slm_ComissionFlag,
                              ComissionPercentValue = b.slm_ComissionPercentValue,
                              CreatedBy = b.slm_CreatedBy,
                              CreatedDate = b.slm_CreatedDate,
                              Ins_Com_Id = b.slm_Ins_Com_Id ?? -1,
                              InsComName = ic.slm_InsNameTh,
                              is_Deleted = b.is_Deleted,
                              OV1BathValue = b.slm_OV1BathValue,
                              OV1Flag = b.slm_OV1Flag,
                              OV1PercentValue = b.slm_OV1PercentValue,
                              OV2BathValue = b.slm_OV2BathValue,
                              OV2Flag = b.slm_OV2Flag,
                              OV2PercentValue = b.slm_OV2PercentValue,
                              Product_Id = b.slm_Product_Id,
                              // ProductName
                              UpdatedBy = b.slm_UpdatedBy,
                              UpdatedDate = b.slm_UpdatedDate,
                              VatFlag = b.slm_VatFlag,

                              CoverageTypeId = b.slm_CoverageTypeId,
                              InsurancecarTypeId = b.slm_InsurancecarTypeId,
                              BenefitTypeCode = b.slm_BenefitTypeCode
                          }).ToList();                

                var products = new CmtMappingProductModel().GetProductList();
                var con = CoverageTypeBiz.LoadAllCoverageType();
                var ins = InsuranceCarTypeBiz.LoadAllInsuranceCarType();

                result.ForEach(r =>
                {
                    var v = products.Where(p => p.ValueField == r.Product_Id).FirstOrDefault();
                    var c = con.Where(x => x.slm_CoverageTypeId == r.CoverageTypeId).SingleOrDefault();
                    var i = ins.Where(x => x.slm_InsurancecarTypeId == r.InsurancecarTypeId).SingleOrDefault();
                    r.ProductName = v == null ? "" : v.TextField;

                    string InsuranceTypeCode = string.Empty;
                    string strBenefitTypeDesc = string.Empty;
                    if (!string.IsNullOrEmpty(r.BenefitTypeCode))
                    {
                        if (r.BenefitTypeCode == "204")
                        {
                            InsuranceTypeCode = "204";
                            strBenefitTypeDesc = "ประกันภัยรถยนต์";
                        }
                        else if (r.BenefitTypeCode == "205")
                        {
                            InsuranceTypeCode = "205";
                            strBenefitTypeDesc = "พรบ.";
                        }
                    }

                    r.BenefitTypeDesc = strBenefitTypeDesc;
                    r.InsuranceTypeDesc = InsuranceTypeCode == "204" ? (c != null ? c.slm_ConverageTypeName : "")  : InsuranceTypeCode == "205" ? (i != null ? i.slm_InsurancecarTypeName : "") : "";
                });
            }
            return result.OrderByDescending(r=>r.CreatedDate).ToList();
        }

        public static InsuranceBenefit GetSearchBenefitById(decimal benefitId)
        {
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                return operdb.kkslm_ms_benefit.Where(b => b.slm_BenefitId == benefitId)
                        .Select(b => new InsuranceBenefit()
                          {
                              BenefitId = b.slm_BenefitId,
                              CampaignInsuranceId = b.slm_CampaignInsuranceId ?? -1,
                              // CampaignName = c.slm_CampaignName,
                              ComissionBathValue = b.slm_ComissionBathValue,
                              ComissionFlag = b.slm_ComissionFlag,
                              ComissionPercentValue = b.slm_ComissionPercentValue,
                              CreatedBy = b.slm_CreatedBy,
                              CreatedDate = b.slm_CreatedDate,
                              Ins_Com_Id = b.slm_Ins_Com_Id ?? -1,
                              // InsComName = ic.slm_InsNameTh,
                              is_Deleted = b.is_Deleted,
                              OV1BathValue = b.slm_OV1BathValue,
                              OV1Flag = b.slm_OV1Flag == null ? "" : b.slm_OV1Flag.Trim(),
                              OV1PercentValue = b.slm_OV1PercentValue,
                              OV2BathValue = b.slm_OV2BathValue,
                              OV2Flag = b.slm_OV2Flag == null ? "" : b.slm_OV2Flag.Trim(),
                              OV2PercentValue = b.slm_OV2PercentValue,
                              Product_Id = b.slm_Product_Id,
                              // ProductName
                              UpdatedBy = b.slm_UpdatedBy,
                              UpdatedDate = b.slm_UpdatedDate,
                              VatFlag = b.slm_VatFlag,
                              CoverageTypeId = b.slm_CoverageTypeId,
                              InsurancecarTypeId = b.slm_InsurancecarTypeId,
                              BenefitTypeCode = b.slm_BenefitTypeCode
                          })
                          .FirstOrDefault();
            }
        }

        public static List<ControlListData> GetAllInsComListData()
        {
            List<ControlListData> result = new List<ControlListData>();
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                result = operdb.kkslm_ms_ins_com.Where(k => k.is_Deleted == false)
                    .Select(k => new { k.slm_InsNameTh, k.slm_Ins_Com_Id })
                    .ToList()
                    .Select(k => new ControlListData() { TextField = k.slm_InsNameTh, ValueField = k.slm_Ins_Com_Id.ToString() })
                    .ToList();
            }
            return result;
        }
        
        public static List<ControlListData> GetSearchInsComListData()
        {
            List<ControlListData> result = new List<ControlListData>();
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                result = (from b in operdb.kkslm_ms_benefit
                          join c in operdb.kkslm_ms_ins_com on b.slm_Ins_Com_Id equals c.slm_Ins_Com_Id
                          select new ControlListData() { TextField = c.slm_InsNameTh, ValueField = c.slm_Ins_Com_Id.ToString() })
                          .ToList();
                result.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
            }
            return result;
        }

        public static List<ControlListData> GetCampaignListData(decimal insComId, string productId)
        {
            List<ControlListData> result = new List<ControlListData>();
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                //var campaignId = slmdb.kkslm_ms_mapping_campaign_insurance
                //    .Where(mapping => mapping.slm_Ins_Com_Id == insComId && mapping.slm_Product_Id == productId)
                //    .Select(mapping => mapping.slm_CampaignInsuranceId)
                //    .Distinct()
                //    .ToList<decimal>();

				var campaignId = (from campaign_insurance in slmdb.kkslm_ms_mapping_campaign_insurance
                                  from mapping_product in 
                                        slmdb.CMT_MAPPING_PRODUCT.Where(x => x.sub_product_id == campaign_insurance.slm_Product_Id)
                                  where
                                      (campaign_insurance.slm_Ins_Com_Id == insComId && campaign_insurance.slm_Product_Id == productId &&
                                       (mapping_product.sub_product_type == 0 || mapping_product.sub_product_type == 2))
                                  select (campaign_insurance.slm_CampaignInsuranceId)
                    ).ToList<decimal>();

                result = operdb.kkslm_ms_campaigninsurance
                    .Where(c => c.is_Deleted == false && campaignId.Contains(c.slm_CampaignInsuranceId))
                    .Select(c => new { c.slm_CampaignName, c.slm_CampaignInsuranceId })
                    .ToList()
                    .Select(c => new ControlListData()
                    {
                        TextField = c.slm_CampaignName
                        ,
                        ValueField = c.slm_CampaignInsuranceId.ToString()
                    })
                    .ToList();
                result.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
            }
            return result;
        }
        
        public static bool Save(InsuranceBenefit benefit, out string responseMessage)
        {
            responseMessage = "บันทึกข้อมูลเรียบร้อย";
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                benefit.CoverageTypeId = benefit.CoverageTypeId == -1 ? null : benefit.CoverageTypeId;
                benefit.InsurancecarTypeId = benefit.InsurancecarTypeId == -1 ? null : benefit.InsurancecarTypeId;
                decimal? CampaignInsuranceId = benefit.CampaignInsuranceId == -1 ? null : (decimal?)benefit.CampaignInsuranceId;

                if (!benefit.is_Deleted)
                {
                    // check duplicate
                    var dup = operdb.kkslm_ms_benefit.Where(kk => kk.slm_Product_Id == benefit.Product_Id
                                                            && kk.slm_Ins_Com_Id == benefit.Ins_Com_Id
                                                            && (kk.slm_CampaignInsuranceId == null && CampaignInsuranceId == null
                                                                || kk.slm_CampaignInsuranceId == CampaignInsuranceId)
                                                            && kk.slm_BenefitId != benefit.BenefitId
                                                            && kk.slm_BenefitTypeCode == benefit.BenefitTypeCode
                                                            && (
                                                                    (benefit.BenefitTypeCode == "204" && (benefit.CoverageTypeId == null && kk.slm_CoverageTypeId == null ||
                                                                                                          kk.slm_CoverageTypeId == benefit.CoverageTypeId)) ||
                                                                    (benefit.BenefitTypeCode == "205" && (benefit.InsurancecarTypeId == null && kk.slm_InsurancecarTypeId == null ||
                                                                                                          kk.slm_InsurancecarTypeId == benefit.InsurancecarTypeId))
                                                               )
                                                            && kk.slm_VatFlag == benefit.VatFlag

                                                            && kk.is_Deleted == benefit.is_Deleted)
                        .ToList();

                    if (dup != null && dup.Count > 0)
                    {
                        responseMessage = "ข้อมูลซ้ำ กรุณาตรวจสอบ";
                        return false;
                    }
                }

                kkslm_ms_benefit b;
                if (benefit.BenefitId == -1)
                {
                    b = new kkslm_ms_benefit();
                    var lastItem = operdb.kkslm_ms_benefit.OrderByDescending(kk => kk.slm_BenefitId).FirstOrDefault();
                    b.slm_BenefitId = lastItem == null ? 1 : lastItem.slm_BenefitId + 1;
                    operdb.kkslm_ms_benefit.AddObject(b);
                    b.slm_CreatedBy = benefit.UpdatedBy;
                    b.slm_CreatedDate = benefit.UpdatedDate;
                }
                else
                {
                    b = operdb.kkslm_ms_benefit.Where(kk => kk.slm_BenefitId == benefit.BenefitId).FirstOrDefault();
                }

                if (benefit.CampaignInsuranceId == -1)
                    b.slm_CampaignInsuranceId = null;
                else 
                    b.slm_CampaignInsuranceId = benefit.CampaignInsuranceId;
                b.slm_ComissionBathValue = benefit.ComissionBathValue;
                b.slm_ComissionFlag = benefit.ComissionFlag;
                b.slm_ComissionPercentValue = benefit.ComissionPercentValue;
                b.slm_Ins_Com_Id = benefit.Ins_Com_Id;
                b.slm_OV1BathValue = benefit.OV1BathValue;
                b.slm_OV1Flag = benefit.OV1Flag;
                b.slm_OV1PercentValue = benefit.OV1PercentValue;
                b.slm_OV2BathValue = benefit.OV2BathValue;
                b.slm_OV2Flag = benefit.OV2Flag;
                b.slm_OV2PercentValue = benefit.OV2PercentValue;
                b.slm_Product_Id = benefit.Product_Id;
                b.slm_UpdatedBy = benefit.UpdatedBy;
                b.slm_UpdatedDate = benefit.UpdatedDate;
                b.slm_VatFlag = benefit.VatFlag;
                b.is_Deleted = benefit.is_Deleted;

                b.slm_BenefitTypeCode = benefit.BenefitTypeCode;
                b.slm_CoverageTypeId = benefit.CoverageTypeId;
                b.slm_InsurancecarTypeId = benefit.InsurancecarTypeId;
                
                // check duplicate 

                operdb.SaveChanges();
                return true;
            }
        }
    }
}
