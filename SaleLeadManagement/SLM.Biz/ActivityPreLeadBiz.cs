using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;
using SLM.Dal;

namespace SLM.Biz
{
    public class ActivityPreLeadBiz
    {
        public static PreleadData GetPrelead(int PreleadId)
        {
            ActivityPreLeadModel preLead = null;
            try
            {
                preLead = new ActivityPreLeadModel();
                return preLead.GetPrelead(PreleadId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (preLead != null)
                {
                    preLead.Dispose();
                }
            }
        }

        public static List<PreleadData> GetPreleads(int PreleadId, string username)
        {
            ActivityPreLeadModel preLead = null;
            try
            {
                preLead = new ActivityPreLeadModel();
                return preLead.GetPreleads(PreleadId, username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (preLead != null)
                {
                    preLead.Dispose();
                }
            }
        }

        public static PreleadCompareData GetPreleadtoCompare(int PreleadId)
        {
            ActivityPreLeadModel preLead = null;
            try
            {
                preLead = new ActivityPreLeadModel();
                return preLead.GetPreleadtoCompare(PreleadId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (preLead != null)
                {
                    preLead.Dispose();
                }
            }
        }


        public static PreleadCompareActData GetPreleadtoCompareAct(int PreleadId)
        {
            ActivityPreLeadModel preLead = null;
            try
            {
                preLead = new ActivityPreLeadModel();
                return preLead.GetPreleadtoCompareAct(PreleadId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (preLead != null)
                {
                    preLead.Dispose();
                }
            }
        }

        public static List<PreleadCompareData> GetPreleadCompare(int PreleadId)
        {
            ActivityPreLeadModel preLead = null;
            try
            {
                preLead = new ActivityPreLeadModel();
                return preLead.GetPreleadCompare(PreleadId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (preLead != null)
                {
                    preLead.Dispose();
                }
            }
        }

        public static List<PreleadCompareActData> GetPreleadCompareAct(int PreleadId)
        {
            ActivityPreLeadModel preLead = null;
            try
            {
                preLead = new ActivityPreLeadModel();
                return preLead.GetPreleadCompareAct(PreleadId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (preLead != null)
                {
                    preLead.Dispose();
                }
            }
        }

        public static PreleadCompareData GetNotifyPremium(int PreleadId)
        {
            ActivityPreLeadModel preLead = null;
            try
            {
                preLead = new ActivityPreLeadModel();
                return preLead.GetNotifyPremium(PreleadId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (preLead != null)
                {
                    preLead.Dispose();
                }
            }
        }

        public static PreleadAddressData GetPreleadAddress(int PreleadId)
        {
            ActivityPreLeadModel preLead = null;
            try
            {
                preLead = new ActivityPreLeadModel();
                return preLead.GetPreleadAddress(PreleadId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (preLead != null)
                {
                    preLead.Dispose();
                }
            }
        }

        public static PromotionInsuranceData getPromotion(int PromotionId)
        {
            ActivityPreLeadModel promotion = null;
            try
            {
                promotion = new ActivityPreLeadModel();
                return promotion.getPromotion(PromotionId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (promotion != null)
                {
                    promotion.Dispose();
                }
            }
        }

        public static void UpdatePrelead(PreleadDataCollection preleadDataCollection)
        {
            //todo
        }

        public static List<SearchPromotionResult> SearchPromotionData(int PreleadId)
        {
            try
            {
                KKSlmMsPromotionModel search = new KKSlmMsPromotionModel();
                return search.SearchPromotionData(PreleadId);
            }
            catch
            {
                throw;
            }
        }

        public static PreleadCompareData SearchPromotionOrder(decimal PromotionId)
        {
            try
            {
                KKSlmMsPromotionModel searchPromo = new KKSlmMsPromotionModel();
                PreleadCompareData result = searchPromo.SearchPromotionOrder(PromotionId);
                return result;
            }
            catch
            {
                throw;
            }

        }
        public static bool CheckCompanyInActive(decimal? PromotionId)
        {
            try
            {
                KKSlmMsPromotionModel searchPromo = new KKSlmMsPromotionModel();
                bool result = searchPromo.Func_CheckCompanyInAct(PromotionId.Value);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public static PreleadCompareActData SearchPromotionActOrder(decimal PromotionId)
        {
            try
            {
                KKSlmMsPromotionModel searchPromo = new KKSlmMsPromotionModel();
                PreleadCompareActData result = searchPromo.SearchPromotionActOrder(PromotionId);
                return result;
            }
            catch
            {
                throw;
            }

        }

        public static PreleadDataCollection SearchPromotionOrderAct(int PreleadId)
        {
            try
            {
                KKSlmMsPromotionModel searchPromoAct = new KKSlmMsPromotionModel();
                PreleadDataCollection result = searchPromoAct.SearchPromotionOrderAct(PreleadId);
                return result;
            }
            catch
            {
                throw;
            }

        }

        public static List<SuretyData> GetSuretyData(int PreleadId)
        {

            try
            {
                KKSlmTrSuretyModel data = new KKSlmTrSuretyModel();
                return data.GetSuretyData(PreleadId);
            }
            catch
            {
                throw;
            }

        }

        public static PreleadData GetInsNameTh(string Voluntary_Company_Code)
        {

            try
            {
                KKSlmTrPreleadModel data = new KKSlmTrPreleadModel();
                return data.GetInsNameTh(Voluntary_Company_Code);
            }
            catch
            {
                throw;
            }

        }
        public static PreleadData GetCoverageType(string Voluntary_Type_Key)
        {

            try
            {
                KKSlmTrPreleadModel data = new KKSlmTrPreleadModel();
                return data.GetCoverageType(Voluntary_Type_Key);
            }
            catch
            {
                throw;
            }

        }

        public static PreleadData GetTitleName1(string Driver_TitleId1)
        {
            try
            {
                KKSlmTrPreleadModel data = new KKSlmTrPreleadModel();
                return data.GetTitleName1(Driver_TitleId1);
            }
            catch
            {
                throw;
            }

        }

        public static PreleadData GetTitleName2(string Driver_TitleId2)
        {
            try
            {
                KKSlmTrPreleadModel data = new KKSlmTrPreleadModel();
                return data.GetTitleName2(Driver_TitleId2);

            }
            catch
            {
                throw;
            }

        }

        public static List<SearchPromotionResult> SearchPromotionList(int PreleadId, string CarBrand, string CarName, string coverageType, decimal OD, decimal ODRanking, decimal FT, decimal FTRanking, string insName)
        {
            try
            {
                KKSlmMsPromotionModel promo = new KKSlmMsPromotionModel();
                return promo.SearchPromotionList(PreleadId, CarBrand, CarName, coverageType, OD, ODRanking, FT, FTRanking, insName);
            }
            catch
            {
                throw;
            }

        }

        public static List<ActData> ListBranchName()
        {
            try
            {
                KKSlmTrActModel branch = new KKSlmTrActModel();
                return branch.ListBranchName();
            }
            catch
            {
                throw;
            }

        }

        public static List<AddressData> ListProvince()
        {
            try
            {
                KKSlmTrAddressModel province = new KKSlmTrAddressModel();
                return province.ListProvince();
            }
            catch
            {
                throw;
            }

        }

        public static List<AddressData> ListDistinct(string provinceValue)
        {

            try
            {
                KKSlmTrAddressModel distinct = new KKSlmTrAddressModel();
                return distinct.ListDistinct(provinceValue);
            }
            catch
            {
                throw;
            }

        }

        public static List<ActData> ListInsNameTh()
        {
            try
            {
                KKSlmTrActModel nameTh = new KKSlmTrActModel();
                return nameTh.ListInsNameTh();
            }
            catch
            {
                throw;
            }

        }

        public static void SaveData(PreleadCompareDataCollection data)
        {
            ActivityPreLeadModel prelead = null;
            try
            {
                prelead = new ActivityPreLeadModel();
                prelead.SaveData(data);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (prelead != null)
                {
                    prelead.Dispose();
                }
            }
        }

        #region Tuning

        public void GetPreleadMainData(string preleadId, PreleadCompareDataCollection PreLeadDataCollectionMain)
        {
            try
            {
                int prelead_id = Convert.ToInt32(preleadId);
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    slmdb.Connection.Open();
                    ActivityPreLeadModel preLead = new ActivityPreLeadModel(slmdb);

                    PreleadData preleadData = preLead.GetPrelead(prelead_id);
                    PreLeadDataCollectionMain.Prelead = preleadData;

                    List<PreleadCompareData> preleadcompare = preLead.GetPreleadCompare(prelead_id);
                    List<PreleadCompareActData> preleadcompareact = preLead.GetPreleadCompareAct(prelead_id);

                    PreLeadDataCollectionMain.Address = preLead.GetPreleadAddress(prelead_id);
                    PreLeadDataCollectionMain.keyTab = preleadData.slm_Prelead_Id.ToString();

                    if (preleadcompare.Count == 0)
                    {
                        PreLeadDataCollectionMain.ComparePrev = preLead.GetPreleadtoCompare(prelead_id);
                        PreLeadDataCollectionMain.CompareCurr = preLead.GetNotifyPremium(prelead_id);

                        if (preleadData != null && string.IsNullOrEmpty(preleadData.slm_RemarkPolicy))
                            preleadData.slm_RemarkPolicy = preLead.GetNotifyPremiumRemark(preleadData.slm_Chassis_No);

                        //set cur
                        if (PreLeadDataCollectionMain.ComparePrev != null)
                        {
                            if (PreLeadDataCollectionMain.ComparePrev.slm_PolicyEndCoverDate != null)
                            {
                                if (PreLeadDataCollectionMain.CompareCurr != null)
                                {
                                    PreLeadDataCollectionMain.CompareCurr.slm_PolicyStartCoverDate = PreLeadDataCollectionMain.ComparePrev.slm_PolicyEndCoverDate;
                                    PreLeadDataCollectionMain.CompareCurr.slm_PolicyEndCoverDate = PreLeadDataCollectionMain.ComparePrev.slm_PolicyEndCoverDate.Value.AddYears(1);
                                }
                            }
                        }

                        PreLeadDataCollectionMain.ComparePromoList = new List<PreleadCompareData>();
                    }
                    else
                    {
                        PreLeadDataCollectionMain.ComparePrev = preleadcompare.Where(s => s.slm_Seq == 1).FirstOrDefault();
                        PreLeadDataCollectionMain.CompareCurr = preleadcompare.Where(s => s.slm_Seq == 2).FirstOrDefault();
                        PreLeadDataCollectionMain.ComparePromoList = preleadcompare.Where(s => s.slm_Seq != 2 && s.slm_Seq != 1).OrderBy(p => p.slm_Seq).ToList();
                    }

                    if (preleadcompareact.Count == 0)
                    {
                        PreLeadDataCollectionMain.ActPrev = preLead.GetPreleadtoCompareAct(prelead_id);
                        PreLeadDataCollectionMain.ActPromoList = new List<PreleadCompareActData>();
                    }
                    else
                    {
                        PreLeadDataCollectionMain.ActPrev = preleadcompareact.Where(s => s.slm_Seq == 1).FirstOrDefault();
                        PreLeadDataCollectionMain.ActPromoList = preleadcompareact.Where(s => s.slm_Seq != 1).OrderBy(p => p.slm_Seq).ToList();
                    }

                    slmdb.Connection.Close();
                    slmdb.Connection.Dispose();
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}
