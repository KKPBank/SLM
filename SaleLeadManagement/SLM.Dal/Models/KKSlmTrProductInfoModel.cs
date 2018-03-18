using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmTrProductInfoModel
    {
        public void InsertData(LeadData leadData, string UserId)
        {
            try
            {
                kkslm_tr_productinfo prodInfo = new kkslm_tr_productinfo();
                prodInfo.slm_TicketId = leadData.TicketId;

                prodInfo.slm_InterestedProd = leadData.InterestedProd;
                prodInfo.slm_LicenseNo = leadData.LicenseNo;
                prodInfo.slm_YearOfCar = leadData.YearOfCar;
                prodInfo.slm_YearOfCarRegis = leadData.YearOfCarRegis;
                prodInfo.slm_ProvinceRegis = leadData.ProvinceRegis;
                prodInfo.slm_Brand = leadData.Brand;
                prodInfo.slm_Model = leadData.Model;
                prodInfo.slm_Submodel = leadData.Submodel;

                prodInfo.slm_RedbookBrandCode = leadData.slm_RedbookBrandCode;
                prodInfo.slm_RedbookModelCode = leadData.slm_RedbookModelCode;
                prodInfo.slm_RedbookKKKey = leadData.slm_RedbookKKKey;
                prodInfo.slm_RedbookYearGroup = leadData.slm_RedbookYearGroup;
                
                prodInfo.slm_DownPayment = leadData.DownPayment;
                prodInfo.slm_DownPercent = leadData.DownPercent;
                prodInfo.slm_CarPrice = leadData.CarPrice;
                prodInfo.slm_FinanceAmt = leadData.FinanceAmt;
                prodInfo.slm_PaymentTerm = leadData.PaymentTerm;
                prodInfo.slm_PaymentType = leadData.PaymentType;
                prodInfo.slm_BalloonAmt = leadData.BalloonAmt;
                prodInfo.slm_BalloonPercent = leadData.BalloonPercent;
                prodInfo.slm_PlanType = leadData.PlanType;
                prodInfo.slm_CoverageDate = leadData.CoverageDate;
                prodInfo.slm_AccType = leadData.AccType;
                prodInfo.slm_AccPromotion = leadData.AccPromotion;
                prodInfo.slm_AccTerm = leadData.AccTerm;
                prodInfo.slm_Interest = leadData.Interest;
                prodInfo.slm_Invest = leadData.Invest;
                prodInfo.slm_LoanOd = leadData.LoanOd;
                prodInfo.slm_LoanOdTerm = leadData.LoanOdTerm;
                prodInfo.slm_Ebank = leadData.Ebank;
                prodInfo.slm_Atm = leadData.Atm;
                prodInfo.slm_OtherDetail_1 = leadData.OtherDetail_1;
                prodInfo.slm_OtherDetail_2 = leadData.OtherDetail_2;
                prodInfo.slm_OtherDetail_3 = leadData.OtherDetail_3;
                prodInfo.slm_OtherDetail_4 = leadData.OtherDetail_4;
                prodInfo.slm_CarType = leadData.CarType;

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    slmdb.kkslm_tr_productinfo.AddObject(prodInfo);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateData(LeadData leadData, string UserId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var prodInfo = slmdb.kkslm_tr_productinfo.Where(p => p.slm_TicketId.Equals(leadData.TicketId)).FirstOrDefault();
                    if (prodInfo != null)
                    {
                        prodInfo.slm_InterestedProd = leadData.InterestedProd;
                        prodInfo.slm_LicenseNo = leadData.LicenseNo;
                        prodInfo.slm_YearOfCar = leadData.YearOfCar;
                        prodInfo.slm_YearOfCarRegis = leadData.YearOfCarRegis;
                        prodInfo.slm_ProvinceRegis = leadData.ProvinceRegis;
                        prodInfo.slm_Brand = leadData.Brand;
                        prodInfo.slm_Model = leadData.Model;
                        prodInfo.slm_Submodel = leadData.Submodel;

                        prodInfo.slm_RedbookBrandCode = leadData.slm_RedbookBrandCode;
                        prodInfo.slm_RedbookModelCode = leadData.slm_RedbookModelCode;
                        prodInfo.slm_RedbookKKKey = leadData.slm_RedbookKKKey;
                        prodInfo.slm_RedbookYearGroup = leadData.slm_RedbookYearGroup;

                        prodInfo.slm_DownPayment = leadData.DownPayment;
                        prodInfo.slm_DownPercent = leadData.DownPercent;
                        prodInfo.slm_CarPrice = leadData.CarPrice;
                        prodInfo.slm_FinanceAmt = leadData.FinanceAmt;
                        prodInfo.slm_PaymentTerm = leadData.PaymentTerm;
                        prodInfo.slm_PaymentType = leadData.PaymentType;
                        prodInfo.slm_BalloonAmt = leadData.BalloonAmt;
                        prodInfo.slm_BalloonPercent = leadData.BalloonPercent;
                        prodInfo.slm_PlanType = leadData.PlanType;
                        prodInfo.slm_CoverageDate = leadData.CoverageDate;
                        prodInfo.slm_AccType = leadData.AccType;
                        prodInfo.slm_AccPromotion = leadData.AccPromotion;
                        prodInfo.slm_AccTerm = leadData.AccTerm;
                        prodInfo.slm_Interest = leadData.Interest;
                        prodInfo.slm_Invest = leadData.Invest;
                        prodInfo.slm_LoanOd = leadData.LoanOd;
                        prodInfo.slm_LoanOdTerm = leadData.LoanOdTerm;
                        prodInfo.slm_Ebank = leadData.Ebank;
                        prodInfo.slm_Atm = leadData.Atm;
                        //prodInfo.slm_OtherDetail_1 = leadData.OtherDetail_1;
                        //prodInfo.slm_OtherDetail_2 = leadData.OtherDetail_2;
                        //prodInfo.slm_OtherDetail_3 = leadData.OtherDetail_3;
                        //prodInfo.slm_OtherDetail_4 = leadData.OtherDetail_4;
                        prodInfo.slm_CarType = leadData.CarType;

                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertData(SLM_DBEntities slmdb, string ticketId, string new_ticketId, string username, DateTime createdDate)
        {
            try
            {
                var prodInfo = slmdb.kkslm_tr_productinfo.Where(p => p.slm_TicketId == ticketId).FirstOrDefault();
                if (prodInfo != null)
                {
                    kkslm_tr_productinfo new_prodInfo = new kkslm_tr_productinfo()
                    {
                        slm_TicketId = new_ticketId,
                        slm_InterestedProd = prodInfo.slm_InterestedProd,
                        slm_LicenseNo = prodInfo.slm_LicenseNo,
                        slm_YearOfCar = prodInfo.slm_YearOfCar,
                        slm_YearOfCarRegis = prodInfo.slm_YearOfCarRegis,
                        slm_ProvinceRegis = prodInfo.slm_ProvinceRegis,
                        slm_Brand = prodInfo.slm_Brand,
                        slm_Model = prodInfo.slm_Model,
                        slm_Submodel = prodInfo.slm_Submodel,
                        slm_DownPayment = prodInfo.slm_DownPayment,
                        slm_DownPercent = prodInfo.slm_DownPercent,
                        slm_CarPrice = prodInfo.slm_CarPrice,
                        slm_FinanceAmt = prodInfo.slm_FinanceAmt,
                        slm_PaymentTerm = prodInfo.slm_PaymentTerm,
                        slm_PaymentType = prodInfo.slm_PaymentType,
                        slm_BalloonAmt = prodInfo.slm_BalloonAmt,
                        slm_BalloonPercent = prodInfo.slm_BalloonPercent,
                        slm_PlanType = prodInfo.slm_PlanType,
                        slm_CoverageDate = prodInfo.slm_CoverageDate,
                        slm_AccType = prodInfo.slm_AccType,
                        slm_AccPromotion = prodInfo.slm_AccPromotion,
                        slm_AccTerm = prodInfo.slm_AccTerm,
                        slm_Interest = prodInfo.slm_Interest,
                        slm_Invest = prodInfo.slm_Invest,
                        slm_LoanOd = prodInfo.slm_LoanOd,
                        slm_LoanOdTerm = prodInfo.slm_LoanOdTerm,
                        slm_Ebank = prodInfo.slm_Ebank,
                        slm_Atm = prodInfo.slm_Atm,
                        slm_OtherDetail_1 = prodInfo.slm_OtherDetail_1,
                        slm_OtherDetail_2 = prodInfo.slm_OtherDetail_2,
                        slm_OtherDetail_3 = prodInfo.slm_OtherDetail_3,
                        slm_OtherDetail_4 = prodInfo.slm_OtherDetail_4,
                        slm_CarType = prodInfo.slm_CarType
                    };

                    slmdb.kkslm_tr_productinfo.AddObject(new_prodInfo);
                }
                else
                    throw new Exception("ไม่พบ Ticket Id " + ticketId + " ใน Table kkslm_tr_productinfo");
            }
            catch
            {
                throw;
            }
        }

        public void updateLeadDataToProductInfo()
        {

            //kkslm_tr_productinfo productInfo = new kkslm_tr_productinfo();

            //productInfo.slm_RedbookBrandCode = pre.slm_Prelead_Id;
            //productInfo.slm_RedbookModelCode = pre.slm_Prelead_Id;

            //slmdb.kkslm_tr_productinfo.AddObject(productInfo);
            //slmdb.SaveChanges();
        }
    }
}
