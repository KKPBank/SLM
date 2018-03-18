using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Resource;
using SLM.Resource.Data;
using System.Transactions;

namespace SLM.Dal.Models
{
    public class ActivityPreLeadModel : IDisposable
    {
        private SLM_DBEntities slmdb = null;

        public ActivityPreLeadModel()
        {
            slmdb = DBUtil.GetSlmDbEntities();
        }

        public ActivityPreLeadModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public void Dispose()
        {
            if (slmdb != null)
            {
                slmdb.Dispose();
            }
        }

        public PreleadData GetPrelead(int PreleadId)
        {
            try
            {
                string sql = @"select p.*, c.slm_CardTypeName, ISNULL(t.slm_TitleName, '') + ISNULL(p.slm_Name, '') + ' ' + ISNULL(p.slm_LastName, '') AS ClientFullname 
                                , p.slm_Cc AS slm_Cc_Org, slm_Model_Year AS slm_Model_Year_Org
                                from kkslm_tr_prelead p WITH (NOLOCK) 
                                left join kkslm_ms_cardtype c WITH (NOLOCK) on p.slm_CardTypeId = c.slm_CardTypeId and c.is_Deleted = 0 
                                left join kkslm_ms_title t WITH (NOLOCK) on t.slm_TitleId = p.slm_TitleId 
                                where  slm_Prelead_Id = " + PreleadId;

                return slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public List<PreleadData> GetPreleads(int PreleadId, string username)
        {
            try
            {
                string sql = @"SELECT pre.slm_CitizenId, c.slm_CardTypeName, pre.slm_CmtLot ,pre.slm_Product_Id ,pre.slm_Owner
                            FROM kkslm_tr_prelead pre WITH (NOLOCK) 
                            inner join kkslm_ms_staff st WITH (NOLOCK) on st.slm_EmpCode = pre.slm_Owner 
                            left join kkslm_ms_cardtype c WITH (NOLOCK) on pre.slm_CardTypeId = c.slm_CardTypeId and c.is_Deleted = 0
                            where slm_SubStatus not in ('06','07','08','09') and pre.slm_TicketId  is null 
                            and pre.is_Deleted = 0 and st.is_Deleted = 0 and slm_Prelead_Id = " + PreleadId +
                                          " and slm_UserName ='" + username + "'";

                var PreleadData = slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();

                if (PreleadData != null && PreleadData.slm_CitizenId != null && PreleadData.slm_CitizenId != "")
                {
                    string sql2 = @"SELECT distinct p.*, c.slm_CardTypeName, ISNULL(t.slm_TitleName, '') + ISNULL(p.slm_Name, '') + ' ' + ISNULL(p.slm_LastName, '') AS ClientFullname 
                                    , p.slm_Cc AS slm_Cc_Org, slm_Model_Year AS slm_Model_Year_Org 
                                    FROM kkslm_tr_prelead p WITH (NOLOCK) 
                                    left join kkslm_ms_cardtype c WITH (NOLOCK) on p.slm_CardTypeId = c.slm_CardTypeId and c.is_Deleted = 0 
                                    left join kkslm_ms_title t WITH (NOLOCK) on t.slm_TitleId = p.slm_TitleId
                                    WHERE slm_SubStatus not in ('06','07','08','09') and p.is_Deleted = 0 
                                    and slm_Owner = " + PreleadData.slm_Owner + @" and slm_Product_Id = '" + PreleadData.slm_Product_Id + "'" +
                                        " and slm_TicketId  is null AND slm_CitizenId = '" + PreleadData.slm_CitizenId + "'" +
                                        " and slm_CmtLot = '" + PreleadData.slm_CmtLot + "'" +
                                        " and slm_Prelead_Id <> " + PreleadId +
                                        " ORDER BY slm_Prelead_Id ASC";

                    return slmdb.ExecuteStoreQuery<PreleadData>(sql2).ToList();
                }
                else
                {
                    return new List<PreleadData>();
                }
            }
            catch
            {
                throw;
            }

        }

        public PreleadCompareData GetPreleadtoCompare(int PreleadId)
        {
            try
            {
                string sql = @"select slm_Voluntary_Policy_Number AS slm_OldPolicyNo
              ,convert(int,slm_Voluntary_Type_Key) AS slm_CoverageTypeId
	          ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = p.slm_Voluntary_Type_Key ) as slm_CoverageTypeName
              ,slm_Voluntary_Company_Name as slm_insnameth
              ,(select slm_Ins_Com_Id from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com where is_deleted = 0 and slm_InsCode  = p.slm_Voluntary_Company_Code)as slm_Ins_Com_Id
	          ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title where slm_TitleId = p.slm_Driver_TitleId1)as slm_TitleName1
	          ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title where slm_TitleId = p.slm_Driver_TitleId2)as slm_TitleName2
	          ,slm_Voluntary_Policy_Eff_Date As slm_PolicyStartCoverDate
              ,slm_Voluntary_Policy_Exp_Date As slm_PolicyEndCoverDate
              ,slm_Voluntary_Cov_Amt As slm_OD
              ,slm_Voluntary_Gross_Premium As slm_PolicyGrossPremium 
              ,slm_Voluntary_Company_Code AS slm_Ins_Com_Id_str
              ,slm_Driver_TitleId1 AS slm_Driver_TitleId1
              ,slm_Driver_First_Name1 As slm_Driver_First_Name1
              ,slm_Driver_Last_Name1 As slm_Driver_Last_Name1
              ,slm_Driver_Birthdate1 As slm_Driver_Birthdate1
              ,slm_Driver_TitleId2 As slm_Driver_TitleId2
              ,slm_Driver_Birthdate2 AS slm_Driver_Birthdate2
	          ,slm_Driver_First_Name2 As slm_Driver_First_Name2
	          ,slm_Driver_Last_Name2 As slm_Driver_Last_Name2
              ,slm_CardTypeId As slm_CardTypeId
              from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead p
                where p.slm_Prelead_Id = " + PreleadId;
                return slmdb.ExecuteStoreQuery<PreleadCompareData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }

        }

        public PreleadCompareActData GetPreleadtoCompareAct(int PreleadId)
        {
            try
            {
                string sql = @"select 
              slm_Compulsory_Policy_Eff_Date AS slm_ActStartCoverDate
			  ,slm_Compulsory_Policy_Exp_Date AS slm_ActEndCoverDate
              ,slm_Compulsory_Gross_Premium AS slm_ActGrossPremium
              ,slm_Compulsory_Gross_Premium AS slm_ActNetGrossPremium
              ,slm_Compulsory_Gross_Premium AS slm_ActGrossPremiumPay
              ,(select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com where is_deleted = 0 and slm_InsCode  = p.slm_Compulsory_Company_Code)as slm_insnameth
              ,(select slm_Ins_Com_Id from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com where is_deleted = 0 and slm_InsCode  = p.slm_Compulsory_Company_Code)as slm_Ins_Com_Id
              ,(DATEDIFF(day,slm_Compulsory_Policy_Eff_Date,slm_Compulsory_Policy_Exp_Date)) AS slm_DateCount
              from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead p
                where p.slm_Prelead_Id = " + PreleadId;
                return slmdb.ExecuteStoreQuery<PreleadCompareActData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }

        }

        public List<PreleadCompareData> GetPreleadCompare(int PreleadId)
        {
            try
            {
                string sql = @"select *
                               ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a WITH (NOLOCK) where a.slm_CoverageTypeId = p.slm_CoverageTypeId ) as slm_CoverageTypeName
                              ,(select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com WITH (NOLOCK) where is_deleted = 0 and slm_Ins_Com_Id  = p.slm_Ins_Com_Id)as slm_insnameth
                              ,slm_Ins_Com_Id as slm_Ins_Com_Id
	                          ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title WITH (NOLOCK) where slm_TitleId = p.slm_Driver_TitleId1)as slm_TitleName1
	                          ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title WITH (NOLOCK) where slm_TitleId = p.slm_Driver_TitleId2)as slm_TitleName2
                            from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare p WITH (NOLOCK) where  slm_Prelead_Id = " + PreleadId;
                return slmdb.ExecuteStoreQuery<PreleadCompareData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        public List<PreleadCompareActData> GetPreleadCompareAct(int PreleadId)
        {
            try
            {
                string sql = @"select * 
                            ,(select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com WITH (NOLOCK) where  is_deleted = 0 and slm_Ins_Com_Id  = pca.slm_Ins_Com_Id)as slm_insnameth
                            from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare_act pca WITH (NOLOCK) where  slm_Prelead_Id = " + PreleadId;
                return slmdb.ExecuteStoreQuery<PreleadCompareActData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        public PreleadCompareData GetNotifyPremium(int PreleadId)
        {
            try
            {
                //           string sql = @"SELECT top 1
                //               slm_Id as slm_NotifyPremiumId
                //,slm_Sum_Insure  as slm_OD
                //               ,slm_InsuranceCarTypeId  as slm_coveragetypeId
                //               ,slm_NetPremium as slm_PolicyGrossPremium
                //,slm_GrossPremium  as slm_NetGrossPremium
                //               ,convert(decimal(18,0),slm_Discount_Percent)  as slm_DiscountPercent
                //,convert(decimal(18,0),slm_Discount_Amount)  as slm_DiscountBath
                //,slm_GrossPremium  - convert(decimal(18,0),slm_Discount_Amount)  slm_PolicyGrossPremiumPay
                //               ,slm_Stamp  as slm_PolicyGrossStamp
                //               ,slm_Vat_Amount  as slm_PolicyGrossVat
                //               ,isnull(slm_InsuranceComId,0)  as slm_Ins_Com_Id
                //               ,(select slm_insnameth from OPERDB.dbo.kkslm_ms_ins_com where  is_deleted = 0 and slm_Ins_Com_Id  = slm_InsuranceComId)as slm_insnameth
                //            ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = dbo.kkslm_tr_notify_premium.slm_InsuranceCarTypeId) as slm_CoverageTypename
                //               ,slm_RepairTypeId  as slm_RepairTypeId
                //               ,slm_InsExpireDate As slm_InsExpireDate
                //            ,(select a.slm_RepairTypeName from kkslm_ms_repairtype a where a.slm_RepairTypeId  = dbo.kkslm_tr_notify_premium.slm_repairTypeId) as slm_RepairTypeName
                //           FROM SLMDB.dbo.kkslm_tr_notify_premium
                //           inner join kkslm_tr_prelead on slm_Chassis_No = slm_VIN
                //           where  slm_Prelead_Id = " + PreleadId +
                //                           " order by slm_InsExpireDate desc, SLMDB.dbo.kkslm_tr_notify_premium.slm_CreatedDate desc";

                //Coment 23/05/2017
                //           string sql = @"SELECT top 1
                //               slm_Id as slm_NotifyPremiumId
                //,slm_Sum_Insure  as slm_OD
                //               ,slm_InsuranceCarTypeId  as slm_coveragetypeId
                //               ,slm_NetPremium as slm_PolicyGrossPremium
                //,slm_GrossPremium  as slm_NetGrossPremium
                //               ,convert(decimal(18,0),slm_Discount_Percent)  as slm_DiscountPercent
                //,convert(decimal(18,0),slm_Discount_Amount)  as slm_DiscountBath
                //,slm_GrossPremium  as slm_PolicyGrossPremiumPay
                //               ,slm_Stamp  as slm_PolicyGrossStamp
                //               ,slm_Vat_Amount  as slm_PolicyGrossVat
                //               ,isnull(slm_InsuranceComId,0)  as slm_Ins_Com_Id
                //               ,(select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com where  is_deleted = 0 and slm_Ins_Com_Id  = slm_InsuranceComId)as slm_insnameth
                //            ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = kkslm_tr_notify_premium.slm_InsuranceCarTypeId) as slm_CoverageTypename
                //               ,slm_RepairTypeId  as slm_RepairTypeId
                //               ,slm_InsExpireDate As slm_InsExpireDate
                //            ,(select a.slm_RepairTypeName from kkslm_ms_repairtype a where a.slm_RepairTypeId  = dbo.kkslm_tr_notify_premium.slm_repairTypeId) as slm_RepairTypeName
                //               , dbo.kkslm_tr_notify_premium.slm_CreatedDate
                //           FROM kkslm_tr_notify_premium
                //           inner join kkslm_tr_prelead on slm_Chassis_No = slm_VIN
                //           where  slm_Prelead_Id = " + PreleadId +
                //                           " order by slm_InsExpireDate desc, kkslm_tr_notify_premium.slm_CreatedDate desc";

                string sql = @"SELECT top 1
                                slm_Id as slm_NotifyPremiumId
	                            ,slm_Sum_Insure  as slm_OD
                                ,slm_InsuranceCarTypeId  as slm_coveragetypeId
                                ,slm_NetPremium as slm_PolicyGrossPremium
	                            ,slm_GrossPremium  as slm_NetGrossPremium
                                ,convert(decimal(18,0),slm_Discount_Percent)  as slm_DiscountPercent
	                            ,convert(decimal(18,0),slm_Discount_Amount)  as slm_DiscountBath
	                            ,slm_GrossPremium  as slm_PolicyGrossPremiumPay
                                ,slm_Stamp  as slm_PolicyGrossStamp
                                ,slm_Vat_Amount  as slm_PolicyGrossVat
                                ,isnull(slm_InsuranceComId,0)  as slm_Ins_Com_Id
                                ,(select slm_insnameth from " + SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com where  is_deleted = 0 and slm_Ins_Com_Id  = slm_InsuranceComId)as slm_insnameth
	                            ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = np.slm_InsuranceCarTypeId) as slm_CoverageTypename
                                ,slm_RepairTypeId  as slm_RepairTypeId
                                ,slm_InsExpireDate As slm_InsExpireDate
	                            ,(select a.slm_RepairTypeName from kkslm_ms_repairtype a where a.slm_RepairTypeId  = np.slm_repairTypeId) as slm_RepairTypeName
                                , np.slm_CreatedDate, np.slm_PeriodYear
                                , CASE WHEN np.slm_DriverFlag = 'Y' THEN '1' ELSE '0' END AS slm_DriverFlag
                            FROM kkslm_tr_notify_premium np WITH (NOLOCK) 
                            inner join kkslm_tr_prelead pre WITH (NOLOCK) on REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(slm_Chassis_No,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') and np.slm_PeriodMonth = pre.slm_PeriodMonth and np.slm_PeriodYear = pre.slm_PeriodYear 
                            where  slm_Prelead_Id = '" + PreleadId + @"' 
                            order by slm_InsExpireDate desc, np.slm_CreatedDate desc ";

                var data = slmdb.ExecuteStoreQuery<PreleadCompareData>(sql).FirstOrDefault();
                if (data != null)
                {
                    string remark = "";
                    if (data.slm_DiscountPercent != null && data.slm_DiscountBath != null)
                    {
                        remark += data.slm_DiscountPercent.Value.ToString("#,##0") + "%";
                        remark += " เป็นเงิน " + data.slm_DiscountBath.Value.ToString("#,##0.00") + " บาท";
                    }
                    else if (data.slm_DiscountPercent != null)
                    {
                        remark += data.slm_DiscountPercent.Value.ToString("#,##0") + "%";
                    }
                    else if (data.slm_DiscountBath != null)
                    {
                        remark += data.slm_DiscountBath.Value.ToString("#,##0.00") + " บาท";
                    }

                    if (data.slm_CreatedDate != null)
                        remark += " นำเข้าวันที่ " + data.slm_CreatedDate.Value.ToString("dd/MM/") + data.slm_CreatedDate.Value.Year.ToString() + " " + data.slm_CreatedDate.Value.ToString("HH:mm:ss");

                    data.PolicyDiscountRemark = remark;
                    data.slm_DiscountBath = null;
                    data.slm_DiscountPercent = null;
                }
                
                return data;
            }
            catch
            {
                throw;
            }

        }

        public PreleadAddressData GetPreleadAddress(int PreleadId)
        {
            try
            {
                string sql = @"SELECT * FROM KKSLM_TR_PRELEAD_ADDRESS WITH (NOLOCK)  WHERE SLM_PRELEAD_ID = " + PreleadId + " AND SLM_ADDRESS_TYPE = 'D'";
                return slmdb.ExecuteStoreQuery<PreleadAddressData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public PromotionInsuranceData getPromotion(int PromotionId)
        {
            try
            {
                string sql = @"SELECT slm_PromotionId
                  ,slm_Product_Id
                  ,ins.slm_InsNameTh as insname
                  ,camins.slm_CampaignName as camname
	              ,slm_DurationYear
                  ,slm_EffectiveDateFrom as EffectiveDateFrom
                  ,slm_EffectiveDateTo as EffectiveDateTo
                  ,brand.slm_BrandName as brandname
                  ,model.slm_ModelName as modelname
	              ,slm_UseCarType
                  ,ct.slm_ConverageTypeName as coveragetypename
	              ,slm_AgeDrivenFlag
                  ,rt.slm_RepairTypeName as repairname
	              ,slm_AgeCarYear
	              ,slm_EngineSize
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
	              ,slm_PersonalAccident
                  ,slm_PersonalAccidentDriver
                  ,slm_PersonalAccidentPassenger
                  ,slm_MedicalFee
                  ,slm_MedicalFeeDriver
                  ,slm_MedicalFeePassenger
                  ,slm_InsuranceDriver
                  ,slm_Remark
              FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance pis inner join " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com ins on ins.slm_Ins_Com_Id = pis.slm_Ins_Com_Id
		            INNER JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_campaigninsurance camins on camins.slm_CampaignInsuranceId = pis.slm_CampaignInsuranceId
		            LEFT JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_repairtype rt on rt.slm_RepairTypeId = pis.slm_RepairTypeId 
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_coveragetype ct on ct.slm_CoverageTypeId = pis.slm_CoverageTypeId 
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_brand brand on brand.slm_BrandCode = pis.slm_Brand_Code 
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_model model on model.slm_ModelCode = pis.slm_Model_Code and model.slm_BrandCode = pis.slm_Brand_Code
              WHERE slm_PromotionId = " + PromotionId;
                return slmdb.ExecuteStoreQuery<PromotionInsuranceData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }


        public void SaveData(PreleadCompareDataCollection data)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    updatePrelead(data.Prelead);

                    ClearDataPreleadCompare(data.Prelead.slm_Prelead_Id);

                    SaveDataPreleadCompare(data.ComparePrev);

                    if (data.CompareCurr != null)
                    {
                        SaveDataPreleadCompare(data.CompareCurr);
                    }

                    if (data.ComparePromoList != null)
                    {
                        foreach (PreleadCompareData pro in data.ComparePromoList)
                        {
                            SaveDataPreleadCompare(pro);
                        }
                    }

                    //if (data.CompareOpt != null)
                    //{
                    //    SaveDataPreleadCompare(data.CompareOpt);
                    //}
                    //ActData
                    ClearDataPreleadCompareAct(data.Prelead.slm_Prelead_Id);

                    savePreleadActData(data.ActPrev);

                    if (data.ActPromoList != null)
                    {
                        foreach (PreleadCompareActData pro in data.ActPromoList)
                        {
                            savePreleadActData(pro);
                        }
                    }

                    //if (data.ActOpt != null)
                    //{
                    //    savePreleadActData(data.ActOpt);
                    //}

                    if (data.Address != null)
                    {
                        savePreleadAddressData(data);
                    }
                    //savePreleadData
                    scope.Complete();
                }
            }
            catch
            {
                throw;
            }
        }

        public void updatePrelead(PreleadData pre)
        {
            try
            {
                kkslm_tr_prelead preOld = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == pre.slm_Prelead_Id).FirstOrDefault();

                preOld.slm_Brand_Code = pre.slm_Brand_Code;
                preOld.slm_Model_Code = pre.slm_Model_Code;
                preOld.slm_Model_Year = pre.slm_Model_Year;
                preOld.slm_Cc = pre.slm_Cc;
                preOld.slm_Car_By_Gov_Id = pre.slm_Car_By_Gov_Id;
                preOld.slm_ProvinceRegis = pre.slm_ProvinceRegis;
                preOld.slm_Receiver = pre.slm_Receiver;
                preOld.slm_SendDocFlag = pre.slm_SendDocFlag;

                preOld.slm_SendDocBrandCode = pre.slm_SendDocBrandCode;

                //Added by Pom 15/08/2016
                preOld.slm_RemarkPolicy = pre.slm_RemarkPolicy;
                preOld.slm_RemarkAct = pre.slm_RemarkAct;

                preOld.slm_UpdatedBy = pre.slm_UpdatedBy;
                preOld.slm_UpdatedDate = DateTime.Now;

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void ClearDataPreleadCompare(decimal? id)
        {
            try
            {
                List<kkslm_tr_prelead_compare> preCompareOld = slmdb.kkslm_tr_prelead_compare.Where(p => p.slm_Prelead_Id == id.Value).ToList();

                foreach (kkslm_tr_prelead_compare pc in preCompareOld)
                {

                    slmdb.kkslm_tr_prelead_compare.DeleteObject(pc);
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void SaveDataPreleadCompare(PreleadCompareData pre)
        {
            try
            {
                kkslm_tr_prelead_compare preCom = new kkslm_tr_prelead_compare();

                preCom.slm_Prelead_Id = pre.slm_Prelead_Id;
                preCom.slm_NotifyPremiumId = pre.slm_NotifyPremiumId;
                preCom.slm_PromotionId = pre.slm_PromotionId;
                preCom.slm_Seq = pre.slm_Seq;
                preCom.slm_Year = pre.slm_Year;
                preCom.slm_Ins_Com_Id = pre.slm_Ins_Com_Id == null ? 0 : pre.slm_Ins_Com_Id.Value;

                preCom.slm_CoverageTypeId = pre.slm_CoverageTypeId == null ? 0 : pre.slm_CoverageTypeId.Value;

                preCom.slm_InjuryDeath = pre.slm_InjuryDeath;
                preCom.slm_TPPD = pre.slm_TPPD;
                preCom.slm_RepairTypeId = pre.slm_RepairTypeId;
                preCom.slm_OD = pre.slm_OD;
                preCom.slm_FT = pre.slm_FT;
                preCom.slm_DeDuctible = pre.slm_DeDuctible;
                preCom.slm_DeDuctibleFlag = pre.slm_DeDuctibleFlag;
                preCom.slm_PersonalAccident = pre.slm_PersonalAccident;
                //PersonalAccidentPassenger ไม่มี field นี้
                //PersonalAccidentDriver ไม่มี field นี้
                preCom.slm_MedicalFee = pre.slm_MedicalFee;
                //MedicalFeeDriver ไม่มี field นี้
                //MedicalFeePassenger ไม่มี field นี้
                //InsuranceDriver ไม่มี field นี้
                preCom.slm_PolicyGrossStamp = pre.slm_PolicyGrossStamp;
                preCom.slm_PolicyGrossVat = pre.slm_PolicyGrossVat;
                preCom.slm_PolicyGrossPremium = pre.slm_PolicyGrossPremium;
                preCom.slm_NetGrossPremium = pre.slm_NetGrossPremium;
                preCom.slm_PolicyGrossPremiumPay = pre.slm_PolicyGrossPremiumPay;
                preCom.slm_CostSave = pre.slm_CostSave;
                preCom.slm_CreatedDate = DateTime.Now;
                preCom.slm_CreatedBy = pre.slm_CreatedBy;
                preCom.slm_UpdatedDate = DateTime.Now;
                preCom.slm_UpdatedBy = pre.slm_UpdatedBy;
                preCom.slm_Selected = pre.slm_Selected;
                preCom.slm_OldPolicyNo = pre.slm_OldPolicyNo;
                preCom.slm_DriverFlag = pre.slm_DriverFlag;
                preCom.slm_Driver_TitleId1 = pre.slm_Driver_TitleId1;
                preCom.slm_Driver_First_Name1 = pre.slm_Driver_First_Name1;
                preCom.slm_Driver_Last_Name1 = pre.slm_Driver_Last_Name1;
                if (pre.slm_Driver_Birthdate1 == DateTime.MinValue)
                {
                    preCom.slm_Driver_Birthdate1 = null;
                }
                else
                {
                    preCom.slm_Driver_Birthdate1 = pre.slm_Driver_Birthdate1;
                }

                preCom.slm_Driver_TitleId2 = pre.slm_Driver_TitleId2;
                preCom.slm_Driver_First_Name2 = pre.slm_Driver_First_Name2;
                preCom.slm_Driver_Last_Name2 = pre.slm_Driver_Last_Name2;

                if (pre.slm_Driver_Birthdate2 == DateTime.MinValue)
                {
                    preCom.slm_Driver_Birthdate2 = null;
                }
                else
                {
                    preCom.slm_Driver_Birthdate2 = pre.slm_Driver_Birthdate2;
                }

                preCom.slm_OldReceiveNo = pre.slm_OldReceiveNo;
                preCom.slm_PolicyStartCoverDate = pre.slm_PolicyStartCoverDate;
                preCom.slm_PolicyEndCoverDate = pre.slm_PolicyEndCoverDate;
                preCom.slm_Vat1Percent = pre.slm_Vat1Percent;
                preCom.slm_DiscountPercent = pre.slm_DiscountPercent;
                preCom.slm_DiscountBath = pre.slm_DiscountBath;
                preCom.slm_Vat1PercentBath = pre.slm_Vat1PercentBath;


                //preCom.slm_InjuryDeath = pre.slm_InjuryDeath;
                //preCom.slm_TPPD = pre.slm_TPPD;
                //preCom.slm_PersonalAccident = pre.slm_PersonalAccident;
                //preCom.slm_PersonalAccidentDriver = pre.slm_PersonalAccidentDriver;
                //preCom.slm_PersonalAccidentPassenger = pre.slm_PersonalAccidentPassenger;
                //preCom.slm_MedicalFee = pre.slm_MedicalFee;
                //preCom.slm_MedicalFeeDriver = pre.slm_MedicalFeeDriver;
                //preCom.slm_MedicalFeePassenger = pre.slm_MedicalFeePassenger;
                preCom.slm_InsuranceDriver = pre.slm_InsuranceDriver;

                slmdb.kkslm_tr_prelead_compare.AddObject(preCom);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void ClearDataPreleadCompareAct(decimal? id)
        {
            try
            {
                List<kkslm_tr_prelead_compare_act> preCompareOld = slmdb.kkslm_tr_prelead_compare_act.Where(p => p.slm_Prelead_Id == id.Value).ToList();

                foreach (kkslm_tr_prelead_compare_act pc in preCompareOld)
                {

                    slmdb.kkslm_tr_prelead_compare_act.DeleteObject(pc);
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void savePreleadActData(PreleadCompareActData actData)
        {
            try
            {
                kkslm_tr_prelead_compare_act preComAct = new kkslm_tr_prelead_compare_act();

                preComAct.slm_Seq = actData.slm_Seq;
                preComAct.slm_Prelead_Id = actData.slm_Prelead_Id;
                preComAct.slm_PromotionId = actData.slm_PromotionId;
                preComAct.slm_Year = actData.slm_Year;
                preComAct.slm_Ins_Com_Id = actData.slm_Ins_Com_Id.Value;
                preComAct.slm_ActIssuePlace = actData.slm_ActIssuePlace;
                preComAct.slm_ActIssueBranch = actData.slm_ActIssueBranch;
                preComAct.slm_SendDocType = actData.slm_SendDocType;
                preComAct.slm_ActSignNo = actData.slm_ActSignNo;
                preComAct.slm_ActStartCoverDate = actData.slm_ActStartCoverDate;
                preComAct.slm_ActEndCoverDate = actData.slm_ActEndCoverDate;
                preComAct.slm_CarTaxExpiredDate = actData.slm_CarTaxExpiredDate;
                preComAct.slm_ActNetGrossPremiumFull = actData.slm_ActNetGrossPremiumFull;
                preComAct.slm_ActGrossPremium = actData.slm_ActGrossPremium == null ? 0 : actData.slm_ActGrossPremium.Value; ;
                preComAct.slm_ActGrossStamp = actData.slm_ActGrossStamp;
                preComAct.slm_ActGrossVat = actData.slm_ActGrossVat;
                preComAct.slm_ActNetGrossPremium = actData.slm_ActNetGrossPremium;
                preComAct.slm_CreatedDate = DateTime.Now;
                preComAct.slm_CreatedBy = actData.slm_CreatedBy;
                preComAct.slm_UpdatedDate = DateTime.Now;
                preComAct.slm_UpdatedBy = actData.slm_UpdatedBy;
                preComAct.slm_DiscountPercent = actData.slm_DiscountPercent;
                preComAct.slm_DiscountBath = actData.slm_DiscountBath;
                preComAct.slm_ActGrossPremiumPay = actData.slm_ActGrossPremiumPay;
                preComAct.slm_Vat1Percent = actData.slm_Vat1Percent;
                preComAct.slm_Vat1PercentBath = actData.slm_Vat1PercentBath;
                preComAct.slm_ActPurchaseFlag = actData.slm_ActPurchaseFlag;
                preComAct.slm_ActNo = null;


                slmdb.kkslm_tr_prelead_compare_act.AddObject(preComAct);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void savePreleadAddressData(PreleadCompareDataCollection data)
        {
            try
            {
                kkslm_tr_prelead_address renewAddress = slmdb.kkslm_tr_prelead_address.Where(a => a.slm_Prelead_Id == data.Prelead.slm_Prelead_Id && a.slm_Address_Type == "D").FirstOrDefault();

                if (renewAddress == null)
                {
                    renewAddress = new kkslm_tr_prelead_address();

                    renewAddress.slm_Prelead_Id = data.Prelead.slm_Prelead_Id.Value;//ได้มาจาก preleadId
                    renewAddress.slm_CmtLot = data.Prelead.slm_CmtLot;
                    renewAddress.slm_Customer_Key = data.Prelead.slm_Customer_Key;
                    renewAddress.slm_Address_Type = "D";
                    renewAddress.slm_House_No = data.Address.slm_House_No;
                    renewAddress.slm_Moo = data.Address.slm_Moo;
                    renewAddress.slm_Building = data.Address.slm_Building;
                    renewAddress.slm_Village = data.Address.slm_Village;
                    renewAddress.slm_Soi = data.Address.slm_Soi;
                    renewAddress.slm_Street = data.Address.slm_Street;
                    renewAddress.slm_TambolId = data.Address.slm_TambolId;
                    renewAddress.slm_Amphur_Id = data.Address.slm_Amphur_Id;
                    renewAddress.slm_Province_Id = data.Address.slm_Province_Id;
                    renewAddress.slm_Zipcode = data.Address.slm_Zipcode;


                    slmdb.kkslm_tr_prelead_address.AddObject(renewAddress);
                }
                else
                {
                    //renewAddress.slm_Prelead_Id = data.Address.slm_Prelead_Id.Value;//ได้มาจาก preleadId
                    //renewAddress.slm_CmtLot = data.Address.slm_CmtLot;
                    //renewAddress.slm_Customer_Key = data.Address.slm_Customer_Key;
                    renewAddress.slm_Address_Type = "D";
                    renewAddress.slm_House_No = data.Address.slm_House_No;
                    renewAddress.slm_Moo = data.Address.slm_Moo;
                    renewAddress.slm_Building = data.Address.slm_Building;
                    renewAddress.slm_Village = data.Address.slm_Village;
                    renewAddress.slm_Soi = data.Address.slm_Soi;
                    renewAddress.slm_Street = data.Address.slm_Street;
                    renewAddress.slm_TambolId = data.Address.slm_TambolId;
                    renewAddress.slm_Amphur_Id = data.Address.slm_Amphur_Id;
                    renewAddress.slm_Province_Id = data.Address.slm_Province_Id;
                    renewAddress.slm_Zipcode = data.Address.slm_Zipcode;
                }


                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public string GetNotifyPremiumRemark(string chassisNo)
        {
            try
            {
                var remark = slmdb.kkslm_tr_notify_premium.Where(p => p.slm_VIN == chassisNo).OrderByDescending(p => p.slm_CreatedDate).Select(p => p.slm_Remark).FirstOrDefault();
                return remark != null ? remark : "";
            }
            catch
            {
                throw;
            }
        }


        
    }
}
