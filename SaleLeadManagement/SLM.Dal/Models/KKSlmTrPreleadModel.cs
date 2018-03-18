using SLM.Resource.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SLM.Dal.Models
{
    public class KKSlmTrPreleadModel
    {
        private string SLMDBName = ConfigurationManager.AppSettings["SLMDBName"] != null ? ConfigurationManager.AppSettings["SLMDBName"] : "SLMDB";

        public PreleadDataCollection GetExitingPrelead(int PreleadId)
        {
            PreleadDataCollection result = new PreleadDataCollection();

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"select * from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare where slm_Seq = 1 and slm_NotifyPremiumId is null and slm_PromotionId is null and slm_Prelead_Id = " + PreleadId;
                result.Prev = slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();

                sql = @"select * from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare where slm_Seq = 2 and slm_NotifyPremiumId is null and slm_PromotionId is null and slm_Prelead_Id = " + PreleadId;
                result.Curr = slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();

                sql = @"select * from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare where slm_Seq = 3 and slm_NotifyPremiumId is null and slm_PromotionId is null and slm_Prelead_Id = " + PreleadId;
                result.Promo = slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();

                sql = @"select * from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare where slm_Seq = 4 and slm_NotifyPremiumId is null and slm_PromotionId is null and slm_Prelead_Id = " + PreleadId;
                result.Opt = slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();


                return result;
            }
        }
        public PreleadDataCollection GetNewPrelead(int PreleadId)
        {
            PreleadDataCollection result = new PreleadDataCollection();

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"select slm_Voluntary_Policy_Number AS slm_OldPolicyNo
                                ,slm_Brand_Name_Org
                                ,slm_Model_Code_Org
                                ,slm_Model_Name_Org
                                ,slm_Cc
                                  ,slm_Voluntary_Type_Key AS slm_CoverageTypeId
	                              ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = p.slm_Voluntary_Type_Key ) as slm_ConverageTypename
                                  ,(select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com where slm_InsCode  = p.slm_Voluntary_Company_Code)as slm_insnameth
                                  ,(select slm_Ins_Com_Id from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com where slm_InsCode  = p.slm_Voluntary_Company_Code)as slm_Ins_Com_Id
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
                                  ,slm_Compulsory_Policy_Eff_Date AS slm_ActStartCoverDate
			                      ,slm_Compulsory_Policy_Exp_Date AS slm_ActEndCoverDate
                                  ,slm_Compulsory_Gross_Premium AS slm_Compulsory_Gross_Premium
                                  ,(DATEDIFF(day,slm_Compulsory_Policy_Eff_Date,slm_Compulsory_Policy_Exp_Date)) AS slm_DateCount
                                  ,slm_House_No
                                  ,slm_Moo
                                  ,slm_Building
                                  ,slm_Village
                                  ,slm_Soi
                                  ,slm_Street
                                  ,slm_TambolId
                                  ,slm_Amphur_Id
                                  ,slm_Province_Id
                                  ,slm_Zipcode
                                  from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead p
                                    left join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.KKSLM_TR_PRELEAD_ADDRESS pa on p.slm_Prelead_Id = pa.slm_Prelead_Id
                                    where p.slm_Prelead_Id = " + PreleadId;

                result.Prev = slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();


                sql = @"SELECT 
                      slm_InsuranceCarTypeId  as slm_coveragetypeld
                      
                      ,slm_NetPremium as slm_NetPremium
                      ,slm_Sum_Insure  as slm_OD
                      ,slm_Discount_Percent  as slm_DiscountPercent
                      ,slm_Stamp  as slm_PolicyGrossStamp
                      ,slm_Vat_Amount  as slm_PolicyGrossVat
                      ,slm_GrossPremium  as slm_PolicyGrossPremium
                      ,slm_InsuranceComId  as slm_Ins_Com_Id
	                  ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = dbo .kkslm_tr_notify_premium .slm_InsuranceCarTypeId) as slm_ConverageTypename
                      ,slm_RepairTypeId  as slm_RepairTypeId
                      ,(select top 1 slm_InsExpireDate from kkslm_tr_notify_premium where slm_VIN = (select slm_Chassis_No from kkslm_tr_prelead where slm_Prelead_Id =" + PreleadId + ")order by slm_InsExpireDate desc) As slm_InsExpireDate" +
                          @",(select a.slm_RepairTypeName from kkslm_ms_repairtype a where a.slm_RepairTypeId  = dbo .kkslm_tr_notify_premium .slm_repairTypeId) as slm_RepairTypeId
                  FROM dbo .kkslm_tr_notify_premium";

                result.Curr = slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();

                return result;
            }
        }
        public void UpdatePrelead(PreleadDataCollection preleadDataCollection)
        {
            //todo
        }
        public PreleadData GetCoverageType(string Voluntary_Type_Key)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"select slm_ConverageTypename from kkslm_ms_coveragetype where slm_CoverageTypeId = '" + Voluntary_Type_Key + "'";
                return slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();
            }
        }

        public PreleadData GetInsNameTh(string Voluntary_Company_Code)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com where slm_InsCode = '" + Voluntary_Company_Code + "'";
                return slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();
            }
        }

        public PreleadData GetTitleName1(string Driver_TitleId1)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"SELECT slm_TitleName AS TitleName1 FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title where slm_TitleId = '" + Driver_TitleId1 + "'";
                return slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();
            }
        }

        public PreleadData GetTitleName2(string Driver_TitleId2)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"SELECT slm_TitleName AS TitleName2 FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title where slm_TitleId = '" + Driver_TitleId2 + "'";
                return slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();
            }
        }

        public PreleadData GetPreleadAddress(int PreleadId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"SELECT *  FROM KKSLM_TR_PRELEAD_ADDRESS WHERE SLM_PRELEAD_ID = " + PreleadId + " AND SLM_ADDRESS_TYPE = 'D'";
                return slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();
            }
        }

        public void SaveData(PreleadDataCollectionSave data, ActDataCollectionForSave act, AddressData address)
        {
            try
            {
                KKSlmTrActModel actModel = new KKSlmTrActModel();
                KKSlmTrAddressModel addressModel = new KKSlmTrAddressModel();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //saveData

                    //Prev
                    //Prelead Data
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        savePreleadData(data.Prev, slmdb);
                        savePreleadData(data.Curr, slmdb);
                        if (data.Promo1.slm_PromotionId != null)
                        {
                            savePreleadData(data.Promo1, slmdb);
                        }
                        if (data.Promo2.slm_PromotionId != null)
                        {
                            savePreleadData(data.Promo2, slmdb);
                        }
                        if (data.Promo3.slm_PromotionId != null)
                        {
                            savePreleadData(data.Promo3, slmdb);
                        }

                        if (data.Opt.slm_PromotionId != null)
                        {
                            savePreleadData(data.Opt, slmdb);
                        }
                    }
                    //ActData

                    actModel.savePreleadActData(act.Prev);

                    if (data.Promo1.slm_PromotionId != null)
                    {
                        actModel.savePreleadActData(act.Promo1);
                    }

                    if (data.Promo2.slm_PromotionId != null)
                    {
                        actModel.savePreleadActData(act.Promo2);
                    }

                    if (data.Promo3.slm_PromotionId != null)
                    {
                        actModel.savePreleadActData(act.Promo3);
                    }

                    if (data.Opt.slm_PromotionId != null)
                    {
                        actModel.savePreleadActData(act.Opt);
                    }


                    addressModel.saveAddressData(address);
                    //savePreleadData
                    scope.Complete();
                }
            }
            catch
            {
                throw;
            }
        }

        public void updateData(PreleadDataCollectionSave data, ActDataCollectionForSave act, AddressData address)
        {
            try
            {
                KKSlmTrActModel actModel = new KKSlmTrActModel();
                KKSlmTrAddressModel addressModel = new KKSlmTrAddressModel();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    DeleteData(0);

                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        savePreleadData(data.Prev, slmdb);
                        savePreleadData(data.Curr, slmdb);
                        if (data.Promo1.slm_PromotionId != 0)
                        {
                            savePreleadData(data.Promo1, slmdb);
                        }
                        if (data.Promo2.slm_PromotionId != 0)
                        {
                            savePreleadData(data.Promo2, slmdb);
                        }
                        if (data.Promo3.slm_PromotionId != 0)
                        {
                            savePreleadData(data.Promo3, slmdb);
                        }
                        savePreleadData(data.Opt, slmdb);

                        //ActData

                        actModel.savePreleadActData(act.Prev);
                        actModel.savePreleadActData(act.Promo1);
                        actModel.savePreleadActData(act.Promo2);
                        actModel.savePreleadActData(act.Promo3);
                        actModel.savePreleadActData(act.Opt);


                        addressModel.saveAddressData(address);

                        slmdb.SaveChanges();
                    }

                    scope.Complete();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteData(int preleadId)
        {
            try
            {
                KKSlmTrActModel actModel = new KKSlmTrActModel();
                KKSlmTrAddressModel addressModel = new KKSlmTrAddressModel();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        deletePreleadData(preleadId, slmdb);
                        addressModel.deletePreleadAddressData(preleadId);
                        actModel.deletePreleadActData(preleadId);
                        slmdb.SaveChanges();
                    }

                    scope.Complete();
                }
            }
            catch
            {
                throw;
            }
        }

        public void savePreleadData(PreleadData pre, SLM_DBEntities slmdb)
        {

            kkslm_tr_prelead_compare preCom = new kkslm_tr_prelead_compare();

            preCom.slm_Prelead_Id = pre.slm_Prelead_Id;
            preCom.slm_NotifyPremiumId = pre.slm_NotifyPremiumId;
            preCom.slm_PromotionId = pre.slm_PromotionId;
            preCom.slm_Year = pre.slm_Year;
            preCom.slm_Ins_Com_Id = pre.slm_Ins_Com_Id.Value;
            if (pre.slm_CoverageTypeId == null)
            {
                preCom.slm_CoverageTypeId = 0;
            }
            else
            {
                preCom.slm_CoverageTypeId = pre.slm_CoverageTypeId == "" ? 0 : int.Parse(pre.slm_CoverageTypeId);
            }
            preCom.slm_InjuryDeath = pre.slm_InjuryDeath;
            preCom.slm_TPPD = pre.slm_TPPD;
            preCom.slm_RepairTypeId = pre.slm_RepairTypeId;
            preCom.slm_OD = pre.slm_OD;
            preCom.slm_FT = pre.slm_FT;
            preCom.slm_DeDuctible = pre.slm_DeDuctible;
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
            preCom.slm_CreatedDate = pre.slm_CreatedDate;
            preCom.slm_CreatedBy = pre.slm_CreatedBy;
            preCom.slm_UpdatedDate = pre.slm_UpdatedDate;
            preCom.slm_UpdatedBy = pre.slm_UpdatedBy;
            preCom.slm_Selected = pre.slm_Selected;
            preCom.slm_OldPolicyNo = pre.slm_OldPolicyNo;
            preCom.slm_DriverFlag = pre.slm_DriverFlag;
            preCom.slm_Driver_TitleId1 = pre.slm_Driver_TitleId1;
            preCom.slm_Driver_First_Name1 = pre.slm_Driver_First_Name1;
            preCom.slm_Driver_Last_Name1 = pre.slm_Driver_Last_Name1;
            if (pre.slm_Driver_Birthdate1 == DateTime.MinValue){
            preCom.slm_Driver_Birthdate1 = null;
            }else{
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


            slmdb.kkslm_tr_prelead_compare.AddObject(preCom);
            slmdb.SaveChanges();
        }

        public void deletePreleadData(int PreleadId, SLM_DBEntities slmdb)
        {
            try
            {
                var sortConList = slmdb.kkslm_tr_prelead_compare.Where(p => p.slm_Prelead_Id == PreleadId).ToList();
                foreach (kkslm_tr_prelead_compare obj in sortConList)
                {
                    slmdb.kkslm_tr_prelead_compare.DeleteObject(obj);
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        public void updatePreleadData(PreleadData pre, SLM_DBEntities slmdb)
        {

            kkslm_tr_prelead_compare preCom = new kkslm_tr_prelead_compare();

            preCom.slm_Prelead_Id = pre.slm_Prelead_Id;
            preCom.slm_NotifyPremiumId = pre.slm_NotifyPremiumId;
            preCom.slm_PromotionId = pre.slm_PromotionId;
            preCom.slm_Year = pre.slm_Year;
            preCom.slm_Ins_Com_Id = pre.slm_Ins_Com_Id.Value;
            //preCom.slm_CoverageTypeId = pre.slm_CoverageTypeId;
            preCom.slm_InjuryDeath = pre.slm_InjuryDeath;
            preCom.slm_TPPD = pre.slm_TPPD;
            preCom.slm_RepairTypeId = pre.slm_RepairTypeId;
            preCom.slm_OD = pre.slm_OD;
            preCom.slm_FT = pre.slm_FT;
            preCom.slm_DeDuctible = pre.slm_DeDuctible;
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
            preCom.slm_CreatedDate = pre.slm_CreatedDate;
            preCom.slm_CreatedBy = pre.slm_CreatedBy;
            preCom.slm_UpdatedDate = pre.slm_UpdatedDate;
            preCom.slm_UpdatedBy = pre.slm_UpdatedBy;
            preCom.slm_Selected = pre.slm_Selected;
            preCom.slm_OldPolicyNo = pre.slm_OldPolicyNo;
            preCom.slm_DriverFlag = pre.slm_DriverFlag;
            preCom.slm_Driver_TitleId1 = pre.slm_Driver_TitleId1;
            preCom.slm_Driver_First_Name1 = pre.slm_Driver_First_Name1;
            preCom.slm_Driver_Last_Name1 = pre.slm_Driver_Last_Name1;
            preCom.slm_Driver_Birthdate1 = pre.slm_Driver_Birthdate1;
            preCom.slm_Driver_TitleId2 = pre.slm_Driver_TitleId2;
            preCom.slm_Driver_First_Name2 = pre.slm_Driver_First_Name2;
            preCom.slm_Driver_Last_Name2 = pre.slm_Driver_Last_Name2;
            preCom.slm_Driver_Birthdate2 = pre.slm_Driver_Birthdate2;
            preCom.slm_OldReceiveNo = pre.slm_OldReceiveNo;
            preCom.slm_PolicyStartCoverDate = pre.slm_PolicyStartCoverDate;
            preCom.slm_PolicyEndCoverDate = pre.slm_PolicyEndCoverDate;
            preCom.slm_Vat1Percent = pre.slm_Vat1Percent;
            preCom.slm_DiscountPercent = pre.slm_DiscountPercent;
            preCom.slm_DiscountBath = pre.slm_DiscountBath;
            preCom.slm_Vat1PercentBath = pre.slm_Vat1PercentBath;

            slmdb.SaveChanges();
        }
    }
}
