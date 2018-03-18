using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class SlmScr999Biz
    {
        public static PreleadDataCollection GetExitingPrelead(int PreleadId)
        {
            KKSlmTrPreleadModel preLead = new KKSlmTrPreleadModel();
            PreleadDataCollection result = preLead.GetExitingPrelead(PreleadId);
            if (result.Curr != null)
            {
                return result;
            }
            else
            {
                result = preLead.GetNewPrelead(PreleadId);
                return result;
            }

        }
        public static PreleadDataCollection GetNewPrelead(int PreleadId)
        {
            PreleadDataCollection result = null;

            return result;
        }
        public static void UpdatePrelead(PreleadDataCollection preleadDataCollection)
        {
            //todo
        }

        public static List<SearchPromotionResult> SearchPromotionData(int PreleadId)
        {
            KKSlmMsPromotionModel search = new KKSlmMsPromotionModel();
            return search.SearchPromotionData(PreleadId);
        }

        public static PreleadCompareData SearchPromotionOrder(int PromotionId)
        {
            KKSlmMsPromotionModel searchPromo = new KKSlmMsPromotionModel();
            PreleadCompareData result = searchPromo.SearchPromotionOrder(PromotionId);
            return result;
        }

        public static PreleadDataCollection SearchPromotionOrderAct(int PreleadId)
        {
            KKSlmMsPromotionModel searchPromoAct = new KKSlmMsPromotionModel();
            PreleadDataCollection result = searchPromoAct.SearchPromotionOrderAct(PreleadId);
            return result;
        }

        public static List<SuretyData> GetSuretyData(int PreleadId)
        {
            KKSlmTrSuretyModel data = new KKSlmTrSuretyModel();
            return data.GetSuretyData(PreleadId);
        }

        public static PreleadData GetInsNameTh(string Voluntary_Company_Code)
        {
            KKSlmTrPreleadModel data = new KKSlmTrPreleadModel();
            return data.GetInsNameTh(Voluntary_Company_Code);
        }
        public static PreleadData GetCoverageType(string Voluntary_Type_Key)
        {
            KKSlmTrPreleadModel data = new KKSlmTrPreleadModel();
            return data.GetCoverageType(Voluntary_Type_Key);
        }

        public static PreleadData GetTitleName1(string Driver_TitleId1)
        {
            KKSlmTrPreleadModel data = new KKSlmTrPreleadModel();
            return data.GetTitleName1(Driver_TitleId1);
        }

        public static PreleadData GetTitleName2(string Driver_TitleId2)
        {
            KKSlmTrPreleadModel data = new KKSlmTrPreleadModel();
            return data.GetTitleName2(Driver_TitleId2);
        }

        public static List<SearchPromotionResult> SearchPromotionList(int PreleadId, string CarBrand, string CarName, string coverageType, decimal OD, decimal ODRanking, decimal FT, decimal FTRanking,string InsName)
        {
            KKSlmMsPromotionModel promo = new KKSlmMsPromotionModel();
            return promo.SearchPromotionList(PreleadId, CarBrand, CarName, coverageType, OD, ODRanking, FT, FTRanking,InsName);
        }

        public static List<SearchPromotionResult> SearchPromotionListbyTicket(string TicketId, string CarBrand, string CarName, string coverageType, decimal OD, decimal ODRanking, decimal FT, decimal FTRanking, string InsName)
        {
            KKSlmMsPromotionModel promo = new KKSlmMsPromotionModel();
            return promo.SearchPromotionListbyTicket(TicketId, CarBrand, CarName, coverageType, OD, ODRanking, FT, FTRanking,InsName);
        }

        public static bool IsPromotionValid(decimal PromotionID)
        {
            KKSlmMsPromotionModel promo = new KKSlmMsPromotionModel();
            return promo.Func_CheckCompanyInAct(PromotionID);
        }
        public static List<ActData> ListBranchName()
        {
            KKSlmTrActModel branch = new KKSlmTrActModel();
            return branch.ListBranchName();
        }
        //Add 20170505
        public static string GetBranchName(string branchCode)
        {
            KKSlmTrActModel branch = new KKSlmTrActModel();
            return branch.GetBranchName(branchCode);
        }
        //Add 20170508
        public static bool GetIsActiveBranch(string branchCode)
        {
            KKSlmTrActModel branch = new KKSlmTrActModel();
            return branch.GetIsActiveBranch(branchCode);
        }

        public static List<AddressData> ListProvince()
        {
            KKSlmTrAddressModel province = new KKSlmTrAddressModel();
            return province.ListProvince();
        }

        public static List<AddressData> ListDistinct(string provinceValue)
        {
            KKSlmTrAddressModel distinct = new KKSlmTrAddressModel();
            return distinct.ListDistinct(provinceValue);
        }

        public static List<ActData> ListInsNameTh()
        {
            KKSlmTrActModel nameTh = new KKSlmTrActModel();
            return nameTh.ListInsNameTh();
        }

        public static List<AddressData> ListTambol(string distinctValue, string provinceValue)
        {
            KKSlmTrAddressModel tambol = new KKSlmTrAddressModel();
            return tambol.ListTambol(distinctValue, provinceValue);
        }


        public static void SaveData(PreleadDataCollectionSave data)
        {
            KKSlmTrPreleadModel prelead = new KKSlmTrPreleadModel();

            ActDataCollectionForSave act = new ActDataCollectionForSave();
            AddressData address = new AddressData();
            prelead.SaveData(data, act, address);

        }
        public static void SaveData(PreleadDataCollectionSave data, ActDataCollectionForSave act, AddressData address)
        {
            KKSlmTrPreleadModel prelead = new KKSlmTrPreleadModel();
            prelead.SaveData(data, act, address);

        }

        //public static LeadDataCollection GetExitingLeadData(int ticketId)
        //{
        //    KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
        //    LeadDataCollection result = lead.GetExitingLeadData(ticketId);

        //    return result;

        //}

        //public static List<LeadData> ListGetDatePayment(int ticketId)
        //{
        //    KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
        //    return lead.ListGetDatePayment(ticketId);
        //}

        //public static PaymentData GetResultActBuy(int ticketId)
        //{
        //    KKSlmMsPaymentMethodModel actBuy = new KKSlmMsPaymentMethodModel();
        //    return actBuy.GetResultActBuy(ticketId);
        //}

        //public static PaymentData GetTotalPay(int ticketId)
        //{
        //    KKSlmMsPaymentMethodModel totalPay = new KKSlmMsPaymentMethodModel();
        //    return totalPay.GetTotalPay(ticketId);
        //}

        //public static PaymentData GetDiscountPercentInConfig(string username)
        //{
        //    KKSlmMsPaymentMethodModel dataDiscount = new KKSlmMsPaymentMethodModel();
        //    return dataDiscount.GetDiscountPercentInConfig(username);
        //}

        //public static List<PaymentData> GetDataDifference(int ticketId)
        //{
        //    KKSlmMsPaymentMethodModel lead = new KKSlmMsPaymentMethodModel();
        //    List<PaymentData> dataDiff = lead.GetDataDifference(ticketId);
        //    if (dataDiff != null)
        //    {
        //        List<PaymentData> payData = lead.HaveDataDifference(ticketId);
        //        return payData;
        //    }
        //    else {

        //        List<PaymentData> insData = lead.NotHaveDataDifference(ticketId);
        //        return insData;
        //    }

        //}
        //public static List<PaymentData> GetDataMajor(int ticketId)
        //{
        //    KKSlmMsPaymentMethodModel lead = new KKSlmMsPaymentMethodModel();
        //    List<PaymentData> dataMjor = lead.GetDataMajor(ticketId);
        //    if (dataMjor != null)
        //    {
        //        List<PaymentData> paytran = lead.HaveDataMajor(ticketId);
        //        return paytran;
        //    }
        //    else
        //    {

        //        List<PaymentData> paymain = lead.NotHaveDataMajor(ticketId);
        //        return paymain;
        //    }

        //}

        //public static PaymentData GetTotalPayAct(int ticketId)
        //{
        //    KKSlmMsPaymentMethodModel totalPatAct = new KKSlmMsPaymentMethodModel();
        //    return totalPatAct.GetTotalPayAct(ticketId);
        //}

        //public static List<PaymentData> GetDataDifferenceAct(int ticketId)
        //{
        //    KKSlmMsPaymentMethodModel lead = new KKSlmMsPaymentMethodModel();
        //    List<PaymentData> dataDiffAct = lead.GetDataDifferenceAct(ticketId);
        //    if (dataDiffAct != null)
        //    {
        //        List<PaymentData> payDataAct = lead.HaveDataDifferenceAct(ticketId);
        //        return payDataAct;
        //    }
        //    else
        //    {

        //        List<PaymentData> insDataAct = lead.NotHaveDataDifferenceAct(ticketId);
        //        return insDataAct;
        //    }

        //}

        //public static List<ReceiptData> ApplyByReceiptNo(int ticketId)
        //{
        //    KKSlmMsPaymentMethodModel receipt = new KKSlmMsPaymentMethodModel();
        //    return receipt.ApplyByReceiptNo(ticketId);
        //}

        //public static List<ReceiptData> EditReceiptDetail()
        //{
        //    KKSlmMsPaymentMethodModel editReceipt = new KKSlmMsPaymentMethodModel();
        //    return editReceipt.EditReceiptDetail();
        //}

        //public static LeadData checkOwnerOrDelegate(int ticketId,string username)
        //{
        //    KKSlmTrLeadModel totalPatAct = new KKSlmTrLeadModel();
        //    LeadData data = totalPatAct.checkOwnerOrDelegate(ticketId, username);
        //    if (data != null)
        //    {
        //        data = totalPatAct.checkOwnerOrDelegateMsStaff(username);
        //        return data;
        //    }
        //    else
        //    {
        //        return data;
        //    }
        //}

        //public static LeadData FindProductIdAndInComId(int ticketId)
        //{
        //    KKSlmTrLeadModel insAndcom = new KKSlmTrLeadModel();
        //    LeadData getData = insAndcom.FindProductIdAndInComId(ticketId);
        //    if (getData.slm_PromotionId == null || getData.slm_PromotionId == 0)
        //    {
        //        LeadData getDatabenefit = insAndcom.FindBenefitByPromotionIdisnull(getData.ProductId);
        //        return getDatabenefit;
        //    }
        //    else if (getData.slm_PromotionId != null || getData.slm_PromotionId != 0)
        //    {
        //        int campaign = Convert.ToInt32(getData.slm_CampaignInsuranceId);
        //        LeadData getDatabenefit = insAndcom.FindBenefitByPromotionIdisnotnull(getData.ProductId,campaign);
        //        if (getDatabenefit == null)
        //        {
        //            LeadData Databenefit = insAndcom.FindBenefitByPromotionIdisnull(getData.ProductId);
        //            return Databenefit;
        //        }
        //        else
        //        {
        //            return getDatabenefit;
        //        }
        //    }

        //    return getData;
        //}

        //public static LeadData FindProductIdAndInComIdAct(int ticketId)
        //{
        //    KKSlmTrLeadModel insAndcom = new KKSlmTrLeadModel();
        //    LeadData getData = insAndcom.FindProductIdAndInComIdAct(ticketId);
        //    if (getData.slm_PromotionId == null || getData.slm_PromotionId == 0)
        //    {
        //        LeadData getDatabenefit = insAndcom.FindBenefitByPromotionIdisnullAct(getData.ProductId);
        //        return getDatabenefit;
        //    }
        //    else if (getData.slm_PromotionId != null || getData.slm_PromotionId != 0)
        //    {
        //        int campaign = Convert.ToInt32(getData.slm_CampaignInsuranceId);
        //        LeadData getDatabenefit = insAndcom.FindBenefitByPromotionIdisnotnullAct(getData.ProductId, campaign);
        //        if (getDatabenefit == null)
        //        {
        //            LeadData Databenefit = insAndcom.FindBenefitByPromotionIdisnullAct(getData.ProductId);
        //            return Databenefit;
        //        }
        //        else
        //        {
        //            return getDatabenefit;
        //        }
        //    }

        //    return getData;
        //}

        //public static void ReceiptNumberIncentiveFindProductIdAndInsComId(int ticketId)
        //{
        //    KKSlmTrIncentiveModel incen = new KKSlmTrIncentiveModel();
        //    IncentiveData data1 = incen.findProductIdAndInsComId(ticketId);
        //    IncentiveData data2 = incen.findReceiptNo(data1.slm_Product_Id,data1.slm_InsuranceComId);

        //}

        public static StaffData GetStaffRoleProduct(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffRoleProduct(username);
        }

        public static StaffData GetStaffRoleAdmin(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffRoleAdmin(username);
        }

        //public static List<ProblemDetailData> ApplyByReceiptNo(int ticketId)
        //{
        //    KKSlmTrProblemDetailModel prob = new KKSlmTrProblemDetailModel();
        //    return prob.ListProblemDetail(ticketId);
        //}

        //public static List<LeadData> ListHistorySell(int ticketId)
        //{
        //    KKSlmTrRenewinsuranceModel renew = new KKSlmTrRenewinsuranceModel();
        //    return renew.ListHistorySell(ticketId);
        //}

        //public static List<RenewinsuranceData> ListHistorySellDetailIns(int RenewInsureId)
        //{
        //    KKSlmTrRenewinsuranceModel renew = new KKSlmTrRenewinsuranceModel();
        //    return renew.ListHistorySellDetailIns(RenewInsureId);
        //}

        //public static List<RenewinsuranceData> ListHistorySellDetailAct(int RenewInsureId)
        //{
        //    KKSlmTrRenewinsuranceModel renew = new KKSlmTrRenewinsuranceModel();
        //    return renew.ListHistorySellDetailAct(RenewInsureId);
        //}
    }
}
