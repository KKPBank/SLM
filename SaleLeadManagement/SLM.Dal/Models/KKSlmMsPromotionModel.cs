using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsPromotionModel
    {
        public List<ControlListData> GetPromotionData(bool useWebservice)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_promotion.OrderBy(p => p.slm_PromotionName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_PromotionName, ValueField = useWebservice ? p.slm_PromotionCode : p.slm_PromotionId.ToString() }).ToList();
            }
        }

        public List<SearchPromotionResult> SearchPromotionData(decimal PreleadId)
        {

            string sql = @"SELECT 0 as slm_PromotionId
                          ,null as slm_Product_Id
                          ,'เลือกบริษัทประกันอื่นๆ' as insname
                          ,'' as camname
                          ,null as EffectiveDateFrom
                          ,null as EffectiveDateTo
                          ,'' as  brandname
                          ,'' as  modelname
                          ,'' as  converagetypename
                          ,'' as  repairname
                          ,null as slm_OD
                          ,null as slm_FT
                          ,null as slm_DeDuctible
                          ,null as slm_GrossPremium
                          ,null as slm_Stamp
                          ,null as slm_Vat
                          ,null as slm_NetGrossPremium
                          ,null as slm_Act
                          ,null as slm_InjuryDeath
                          ,null as slm_TPPD
                    union all
                    SELECT slm_PromotionId
                          ,slm_Product_Id
                          ,ins.slm_InsNameTh as insname
                          ,camins.slm_CampaignName as camname
                          ,slm_EffectiveDateFrom as EffectiveDateFrom
                          ,slm_EffectiveDateTo as EffectiveDateTo
                          ,brand.slm_BrandName as brandname
                          ,model.slm_ModelName as modelname
                          ,ct.slm_ConverageTypeName as converagetypename
                          ,rt.slm_RepairTypeName as repairname
                          ,slm_OD
                          ,slm_FT
                          ,slm_DeDuctible
                          ,slm_GrossPremium
                          ,slm_Stamp
                          ,slm_Vat
                          ,slm_NetGrossPremium
                          ,slm_Act
                          ,slm_InjuryDeath
                          ,slm_TPPD
                      FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance pis inner join " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com ins on ins.slm_Ins_Com_Id = pis.slm_Ins_Com_Id
		                    INNER JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_campaigninsurance camins on camins.slm_CampaignInsuranceId = pis.slm_CampaignInsuranceId
                            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_brand brand on brand.slm_BrandCode = pis.slm_Brand_Code 
		                    INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_model model on model.slm_ModelCode = pis.slm_Model_Code and model.slm_BrandCode = pis.slm_Brand_Code
		                    LEFT JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_repairtype rt on rt.slm_RepairTypeId = pis.slm_RepairTypeId 
		                    LEFT JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_coveragetype ct on ct.slm_CoverageTypeId = pis.slm_CoverageTypeId 
                      WHERE pis.is_Deleted = 0 and convert(date, GETDATE()) between CONVERT(date,slm_EffectiveDateFrom) and CONVERT(date,slm_EffectiveDateTo)
	                    and pis.slm_Product_Id = (SELECT slm_Product_Id FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead WHERE slm_Prelead_Id = " + PreleadId + ")";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SearchPromotionResult>(sql).ToList();
            }
        }

        public PreleadCompareData SearchPromotionOrder(decimal PromotionId)
        {
            string sql = @"select slm_PromotionId
                          ,slm_Product_Id
                          ,b.slm_Ins_Com_Id
                          ,b.slm_InsNameTh as slm_InsNameTh
                          --,camins.slm_CampaignName as slm_CampaignName
                          ,slm_EffectiveDateFrom as slm_EffectiveDateFrom
                          ,slm_EffectiveDateTo as slm_EffectiveDateTo
                          --,brand.slm_BrandCode as slm_BrandCode
                          --,model.slm_ModelCode as slm_ModelCode
                          --,brand.slm_BrandName as slm_BrandName
                          --,model.slm_ModelName as slm_ModelName
                          ,a.slm_CoverageTypeId as slm_CoverageTypeId
                          ,a.slm_ConverageTypeName as slm_CoverageTypeName
                          ,c.slm_RepairTypeID as slm_RepairTypeId
                          ,c.slm_RepairTypeName as slm_RepairTypeName
                          ,slm_OD
                          ,slm_FT
                          ,0.00 slm_DeDuctible
                          ,slm_GrossPremium slm_PolicyGrossPremium
                          ,slm_Stamp slm_PolicyGrossStamp
                          ,slm_Vat slm_PolicyGrossVat
                          ,slm_NetGrossPremium
                          ,slm_Act
                          ,slm_InjuryDeath
                            ,slm_TPPD
                            ,slm_PersonalAccident
                            ,slm_PersonalAccidentDriver
                            ,slm_PersonalAccidentPassenger
                            ,slm_MedicalFee
                            ,slm_MedicalFeeDriver
                            ,slm_MedicalFeePassenger
                            ,slm_InsuranceDriver
                            ,slm_DeDuctible AS DeDuctibleFlag
                            from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance p
                            left join kkslm_ms_coveragetype a on a.slm_CoverageTypeId = p.slm_CoverageTypeId
                            left join " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com b on b.slm_Ins_Com_Id =  p.slm_Ins_Com_Id
                            left join kkslm_ms_repairtype c on c.slm_RepairTypeId = p.slm_RepairTypeId
                            where slm_PromotionId = " + PromotionId + @" and p.is_Deleted = 0";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<PreleadCompareData>(sql).FirstOrDefault();
            }
        }

        public PreleadCompareActData SearchPromotionActOrder(decimal PromotionId)
        {
            string sql = @"select slm_PromotionId
                          ,slm_Product_Id
                          ,b.slm_Ins_Com_Id
                          ,b.slm_InsNameTh as slm_InsNameTh
                          --,camins.slm_CampaignName as slm_CampaignName
                          ,slm_EffectiveDateFrom as slm_EffectiveDateFrom
                          ,slm_EffectiveDateTo as slm_EffectiveDateTo
                          --,brand.slm_BrandCode as slm_BrandCode
                          --,model.slm_ModelCode as slm_ModelCode
                          --,brand.slm_BrandName as slm_BrandName
                          --,model.slm_ModelName as slm_ModelName
                          ,a.slm_CoverageTypeId as slm_CoverageTypeId
                          ,a.slm_ConverageTypeName as slm_CoverageTypeName
                          ,c.slm_RepairTypeID as slm_RepairTypeId
                          ,c.slm_RepairTypeName as slm_RepairTypeName
                          ,slm_OD
                          ,slm_FT
                          ,0.00 slm_DeDuctible

                          ,slm_GrossPremium slm_PolicyGrossPremium
                          ,slm_Stamp slm_PolicyGrossStamp
                          ,slm_Vat slm_PolicyGrossVat
                          ,slm_NetGrossPremium
                          ,slm_Act slm_ActNetGrossPremiumFull
                          ,slm_Act slm_ActGrossPremium
                          ,slm_InjuryDeath
                          ,slm_TPPD 
                            from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance p
                            left join kkslm_ms_coveragetype a on a.slm_CoverageTypeId = p.slm_CoverageTypeId
                            left join " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com b on b.slm_Ins_Com_Id =  p.slm_Ins_Com_Id
                            left join kkslm_ms_repairtype c on c.slm_RepairTypeId = p.slm_RepairTypeId
                            where slm_PromotionId = " + PromotionId + "";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<PreleadCompareActData>(sql).FirstOrDefault();
            }
        }

        public PreleadDataCollection SearchPromotionOrderAct(int PreleadId)
        {
            PreleadDataCollection result = new PreleadDataCollection();

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"select * from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare_act where slm_Seq > 1 and slm_PromotionId <> 0 and slm_Prelead_Id = " + PreleadId + "";
                result.Promo = slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();
                return result;
            }
        }

        public List<PromotionData> ListInsuranceName()
        {
            string sql = @"SELECT slm_CoverageTypeId as InsuranceValue,slm_ConverageTypeName as InsuranceName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_coveragetype Where is_Deleted = 0";
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<PromotionData>(sql).ToList();
            }
        }

        public List<SearchPromotionResult> SearchPromotionList(int PreleadId, string CarBrand, string CarName, string coverageType, decimal OD, decimal ODRanking, decimal FT, decimal FTRanking, string InsName)
        {
            string sql = @"
                    SELECT slm_PromotionId
                          ,slm_Product_Id
                          ,ins.slm_InsNameTh as insname
                          ,camins.slm_CampaignName as camname
                          ,slm_EffectiveDateFrom as EffectiveDateFrom
                          ,slm_EffectiveDateTo as EffectiveDateTo
                          ,brand.slm_BrandName as brandname
                          ,model.slm_ModelName as modelname
                          ,ct.slm_ConverageTypeName as converagetypename
                          ,rt.slm_RepairTypeName as repairname
                          ,slm_OD
                          ,slm_FT
                          ,slm_DeDuctible
                          ,slm_GrossPremium
                          ,slm_Stamp
                          ,slm_Vat
                          ,slm_NetGrossPremium
                          ,slm_Act
                          ,slm_InjuryDeath
                          ,slm_TPPD
                          ,slm_AgeCarYear
                      FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance pis inner join " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com ins on ins.slm_Ins_Com_Id = pis.slm_Ins_Com_Id
		                    INNER JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_campaigninsurance camins on camins.slm_CampaignInsuranceId = pis.slm_CampaignInsuranceId
		                    LEFT JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_repairtype rt on rt.slm_RepairTypeId = pis.slm_RepairTypeId 
		                    INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_coveragetype ct on ct.slm_CoverageTypeId = pis.slm_CoverageTypeId 
		                    INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_brand brand on brand.slm_BrandCode = pis.slm_Brand_Code 
		                    INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_model model on model.slm_ModelCode = pis.slm_Model_Code and model.slm_BrandCode = pis.slm_Brand_Code
                      WHERE pis.is_Deleted = 0 and convert(date, GETDATE()) between CONVERT(date,slm_EffectiveDateFrom) and CONVERT(date,slm_EffectiveDateTo)
	                    and pis.slm_Product_Id = (SELECT slm_Product_Id FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead WHERE slm_Prelead_Id = " + PreleadId + ")";

            if (InsName != "")
            {
                sql += " and ins.slm_InsNameTh like '%" + InsName + "%'";
            }

            if (CarBrand != "" && CarBrand != "0")
            {
                sql += " and brand.slm_BrandCode ='" + CarBrand + "'";
            }

            if (CarName != "" && CarName != "0")
            {
                sql += " and model.slm_ModelCode ='" + CarName + "'";
            }

            if (coverageType != "" && coverageType != "0")
            {
                sql += " and ct.slm_CoverageTypeId ='" + coverageType + "'";
            }


            if (OD != 0)
            {

                if (ODRanking == 0)
                {
                    sql += " and slm_OD = " + OD;
                }
                else
                {
                    sql += " and slm_OD <= " + (OD * ((100.00m + ODRanking) / 100.00m)).ToString();
                    sql += " and slm_OD >= " + (OD * ((100.00m - ODRanking) / 100.00m)).ToString();
                }
            }

            if (FT != 0)
            {

                if (FTRanking == 0)
                {
                    sql += " and slm_FT = " + FT;
                }
                else
                {
                    sql += " and slm_FT <= " + (FT * ((100m + FTRanking) / 100m)).ToString();
                    sql += " and slm_FT >= " + (FT * ((100m - FTRanking) / 100m)).ToString();
                }
            }

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SearchPromotionResult>(sql).ToList();
            }
        }

        public List<SearchPromotionResult> SearchPromotionListbyTicket(string ticketId, string CarBrand, string CarName, string coverageType, decimal OD, decimal ODRanking, decimal FT, decimal FTRanking, string InsName)
        {
            string sql = @"
            SELECT slm_PromotionId
                  ,slm_Product_Id
                  ,ins.slm_InsNameTh as insname
                  ,camins.slm_CampaignName as camname
                  ,slm_EffectiveDateFrom as EffectiveDateFrom
                  ,slm_EffectiveDateTo as EffectiveDateTo
                  ,brand.slm_BrandName as brandname
                  ,model.slm_ModelName as modelname
                  ,ct.slm_ConverageTypeName as converagetypename
                  ,rt.slm_RepairTypeName as repairname
                  ,slm_OD
                  ,slm_FT
                  ,slm_DeDuctible
                  ,slm_GrossPremium
                  ,slm_Stamp
                  ,slm_Vat
                  ,slm_NetGrossPremium
                  ,slm_Act
                  ,slm_InjuryDeath
                  ,slm_TPPD
                  ,slm_AgeCarYear
              FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance pis inner join " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com ins on ins.slm_Ins_Com_Id = pis.slm_Ins_Com_Id
		            INNER JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_campaigninsurance camins on camins.slm_CampaignInsuranceId = pis.slm_CampaignInsuranceId
		            LEFT JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_repairtype rt on rt.slm_RepairTypeId = pis.slm_RepairTypeId 
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_coveragetype ct on ct.slm_CoverageTypeId = pis.slm_CoverageTypeId 
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_brand brand on brand.slm_BrandCode = pis.slm_Brand_Code 
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_model model on model.slm_ModelCode = pis.slm_Model_Code and model.slm_BrandCode = pis.slm_Brand_Code
              WHERE pis.is_Deleted = 0 and convert(date, GETDATE()) between CONVERT(date,slm_EffectiveDateFrom) and CONVERT(date,slm_EffectiveDateTo)
	            and pis.slm_Product_Id = (SELECT slm_Product_Id FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead WITH (NOLOCK) WHERE slm_ticketId = '" + ticketId + "')";

            if (InsName != "")
            {
                sql += " and ins.slm_InsNameTh like '%" + InsName + "%'";
            }

            if (CarBrand != "" && CarBrand != "0")
            {
                sql += " and brand.slm_BrandCode ='" + CarBrand + "'";
            }

            if (CarName != "" && CarName != "0")
            {
                sql += " and model.slm_ModelCode ='" + CarName + "'";
            }

            if (coverageType != "" && coverageType != "0")
            {
                sql += " and ct.slm_CoverageTypeId ='" + coverageType + "'";
            }


            if (OD != 0)
            {

                if (ODRanking == 0)
                {
                    sql += " and slm_OD = " + OD;
                }
                else
                {
                    sql += " and slm_OD <= " + (OD * ((100.00m + ODRanking) / 100.00m)).ToString();
                    sql += " and slm_OD >= " + (OD * ((100.00m - ODRanking) / 100.00m)).ToString();
                }
            }

            if (FT != 0)
            {

                if (FTRanking == 0)
                {
                    sql += " and slm_FT = " + FT;
                }
                else
                {
                    sql += " and slm_FT <= " + (FT * ((100m + FTRanking) / 100m)).ToString();
                    sql += " and slm_FT >= " + (FT * ((100m - FTRanking) / 100m)).ToString();
                }
            }

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SearchPromotionResult>(sql).ToList();
            }
        }

        public bool IsPromotionValid(decimal promotionID)
        {
            string sql = @"
            SELECT slm_PromotionId
              FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance pis 
              WHERE pis.is_Deleted = 0 and pis.slm_PromotionId = " + promotionID.ToString();

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var result = slmdb.ExecuteStoreQuery<decimal>(sql).ToList();
                if (result.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public bool Func_CheckCompanyInAct(decimal promotionID)
        {
            string sql = @"Select prom.slm_PromotionId 
                FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com comp
                INNER JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance prom on prom.slm_Ins_Com_Id = comp.slm_Ins_Com_Id
                WHERE comp.is_Deleted = 0 and prom.slm_PromotionId = " + promotionID.ToString();
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var result = slmdb.ExecuteStoreQuery<decimal>(sql).ToList();
                return (result.Count() > 0 ? true : false);
            }
        }
    }
}
