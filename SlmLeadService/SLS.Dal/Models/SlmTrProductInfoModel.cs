using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;
using SLS.Resource.Data;

namespace SLS.Dal.Models
{
    public class SlmTrProductInfoModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmTrProductInfoModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public void InsertData(string ticketId, ProductInfo product)
        {
            try
            {
                MasterModel master = new MasterModel(slmdb);
                kkslm_tr_productinfo prodInfo = new kkslm_tr_productinfo();
                prodInfo.slm_TicketId = ticketId;

                if (product != null)
                {
                    if (!string.IsNullOrEmpty(product.InterestedProdAndType)) prodInfo.slm_InterestedProd = product.InterestedProdAndType;
                    if (!string.IsNullOrEmpty(product.LicenseNo)) prodInfo.slm_LicenseNo = product.LicenseNo;
                    if (!string.IsNullOrEmpty(product.YearOfCar)) prodInfo.slm_YearOfCar = product.YearOfCar;
                    if (!string.IsNullOrEmpty(product.YearOfCarRegis)) prodInfo.slm_YearOfCarRegis = product.YearOfCarRegis;
                    if (!string.IsNullOrEmpty(product.RegisterProvince)) prodInfo.slm_ProvinceRegis = master.GetProvinceId(product.RegisterProvince);   //Foreign Key

                    //Modified By Pom 2016/01/05, Change Redbook
                    if (!string.IsNullOrEmpty(product.Brand)) prodInfo.slm_RedbookBrandCodeExt = product.Brand;
                    if (!string.IsNullOrEmpty(product.Model)) prodInfo.slm_RedbookModelCodeExt = product.Model;
                    if (!string.IsNullOrEmpty(product.Submodel))
                    {
                        prodInfo.slm_RedbookKKKeyExt = product.Submodel;
                        var submodel = slmdb.kkslm_ms_redbook_submodel.Where(p => p.slm_KKKey == product.Submodel).FirstOrDefault();
                        if (submodel != null)
                        {
                            prodInfo.slm_RedbookBrandCode = submodel.slm_BrandCode;
                            prodInfo.slm_RedbookModelCode = submodel.slm_ModelCode;
                            prodInfo.slm_RedbookKKKey = submodel.slm_KKKey;
                            prodInfo.slm_RedbookYearGroup = submodel.slm_YearGroup;
                        }
                    }

                    if (!string.IsNullOrEmpty(product.DownPayment)) prodInfo.slm_DownPayment = decimal.Parse(product.DownPayment);
                    if (!string.IsNullOrEmpty(product.DownPercent)) prodInfo.slm_DownPercent = decimal.Parse(product.DownPercent);
                    if (!string.IsNullOrEmpty(product.CarPrice)) prodInfo.slm_CarPrice = decimal.Parse(product.CarPrice);
                    if (!string.IsNullOrEmpty(product.CarType)) prodInfo.slm_CarType = product.CarType;
                    if (!string.IsNullOrEmpty(product.FinanceAmt)) prodInfo.slm_FinanceAmt = decimal.Parse(product.FinanceAmt);
                    if (!string.IsNullOrEmpty(product.Term)) prodInfo.slm_PaymentTerm = product.Term;
                    if (!string.IsNullOrEmpty(product.PaymentType)) prodInfo.slm_PaymentType = master.GetPaymentTypeId(product.PaymentType);            //Foreign Key
                    if (!string.IsNullOrEmpty(product.BalloonAmt)) prodInfo.slm_BalloonAmt = decimal.Parse(product.BalloonAmt);
                    if (!string.IsNullOrEmpty(product.BalloonPercent)) prodInfo.slm_BalloonPercent = decimal.Parse(product.BalloonPercent);
                    if (!string.IsNullOrEmpty(product.Plantype)) prodInfo.slm_PlanType = product.Plantype;                                              //Foreign Key
                    if (!string.IsNullOrEmpty(product.CoverageDate)) prodInfo.slm_CoverageDate = product.CoverageDate;
                    if (!string.IsNullOrEmpty(product.AccType)) prodInfo.slm_AccType = master.GetModuleId(decimal.Parse(product.AccType));              //Foreign Key
                    if (!string.IsNullOrEmpty(product.AccPromotion)) prodInfo.slm_AccPromotion = master.GetPromotionId(product.AccPromotion);           //Foreign Key
                    if (!string.IsNullOrEmpty(product.AccTerm)) prodInfo.slm_AccTerm = product.AccTerm;
                    if (!string.IsNullOrEmpty(product.Interest)) prodInfo.slm_Interest = product.Interest;
                    if (!string.IsNullOrEmpty(product.Invest)) prodInfo.slm_Invest = product.Invest;
                    if (!string.IsNullOrEmpty(product.LoanOd)) prodInfo.slm_LoanOd = product.LoanOd;
                    if (!string.IsNullOrEmpty(product.LoanOdTerm)) prodInfo.slm_LoanOdTerm = product.LoanOdTerm;
                    if (!string.IsNullOrEmpty(product.SlmBank)) prodInfo.slm_Ebank = product.SlmBank;
                    if (!string.IsNullOrEmpty(product.SlmAtm)) prodInfo.slm_Atm = product.SlmAtm;
                    if (!string.IsNullOrEmpty(product.OtherDetail1)) prodInfo.slm_OtherDetail_1 = product.OtherDetail1;
                    if (!string.IsNullOrEmpty(product.OtherDetail2)) prodInfo.slm_OtherDetail_2 = product.OtherDetail2;
                    if (!string.IsNullOrEmpty(product.OtherDetail3)) prodInfo.slm_OtherDetail_3 = product.OtherDetail3;
                    if (!string.IsNullOrEmpty(product.OtherDetail4)) prodInfo.slm_OtherDetail_4 = product.OtherDetail4;

                    //Modified By Pom 2016/01/05
                    //if (!string.IsNullOrEmpty(product.Brand)) prodInfo.slm_Brand = master.GetBrandId(product.Brand);    //Foreign Key
                    //if (!string.IsNullOrEmpty(product.Brand) && !string.IsNullOrEmpty(product.Model))                   
                    //    prodInfo.slm_Model = master.GetModelId(product.Brand, product.Model);                           //Foreign Key

                    //if (!string.IsNullOrEmpty(product.Submodel))
                    //{
                    //    decimal result;
                    //    if (decimal.TryParse(product.Submodel, out result))
                    //        prodInfo.slm_Submodel = master.GetSubModelId(decimal.Parse(product.Submodel));         //Foreign Key
                    //}
                }

                slmdb.kkslm_tr_productinfo.AddObject(prodInfo);
                slmdb.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new ServiceException(ApplicationResource.INS_INSERT_FAIL_CODE, ApplicationResource.INS_INSERT_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        public void UpdateDataByHpaofl(string ticketId, ProductInfo product)
        {
            try
            {
                MasterModel master = new MasterModel(slmdb);
                var prodInfo = slmdb.kkslm_tr_productinfo.Where(p => p.slm_TicketId.Equals(ticketId)).FirstOrDefault();
                if (prodInfo != null)
                {
                    try
                    {
                        if (product != null)
                        {
                            if (product.InterestedProdAndType != null) prodInfo.slm_InterestedProd = product.InterestedProdAndType;
                            if (product.LicenseNo != null) prodInfo.slm_LicenseNo = product.LicenseNo;
                            if (product.YearOfCar != null) prodInfo.slm_YearOfCar = product.YearOfCar;
                            if (product.YearOfCarRegis != null) prodInfo.slm_YearOfCarRegis = product.YearOfCarRegis;
                            if (!string.IsNullOrEmpty(product.RegisterProvince))
                                prodInfo.slm_ProvinceRegis = master.GetProvinceId(product.RegisterProvince);                    //Foreign Key

                            //Modified By Pom 2016/01/05, Change Redbook
                            if (product.Brand != null) 
                                prodInfo.slm_RedbookBrandCodeExt = product.Brand.Trim() != "" ? product.Brand.Trim() : null;

                            if (product.Model != null) 
                                prodInfo.slm_RedbookModelCodeExt = product.Model.Trim() != "" ? product.Model.Trim() : null;

                            if (product.Submodel != null)
                            {
                                prodInfo.slm_RedbookKKKeyExt = product.Submodel.Trim() != "" ? product.Submodel.Trim() : null;
                                var submodel = slmdb.kkslm_ms_redbook_submodel.Where(p => p.slm_KKKey == product.Submodel).FirstOrDefault();
                                if (submodel != null)
                                {
                                    prodInfo.slm_RedbookBrandCode = submodel.slm_BrandCode;
                                    prodInfo.slm_RedbookModelCode = submodel.slm_ModelCode;
                                    prodInfo.slm_RedbookKKKey = submodel.slm_KKKey;
                                    prodInfo.slm_RedbookYearGroup = submodel.slm_YearGroup;
                                }
                                else
                                {
                                    prodInfo.slm_RedbookBrandCode = null;
                                    prodInfo.slm_RedbookModelCode = null;
                                    prodInfo.slm_RedbookKKKey = null;
                                    prodInfo.slm_RedbookYearGroup = null;
                                }
                            }
                            
                            if (!string.IsNullOrEmpty(product.DownPayment)) prodInfo.slm_DownPayment = decimal.Parse(product.DownPayment);
                            if (!string.IsNullOrEmpty(product.DownPercent)) prodInfo.slm_DownPercent = decimal.Parse(product.DownPercent);
                            if (!string.IsNullOrEmpty(product.CarPrice)) prodInfo.slm_CarPrice = decimal.Parse(product.CarPrice);
                            if (!string.IsNullOrEmpty(product.CarType)) prodInfo.slm_CarType = product.CarType;
                            if (!string.IsNullOrEmpty(product.FinanceAmt)) prodInfo.slm_FinanceAmt = decimal.Parse(product.FinanceAmt);
                            if (product.Term != null) prodInfo.slm_PaymentTerm = product.Term;
                            if (!string.IsNullOrEmpty(product.PaymentType)) prodInfo.slm_PaymentType = master.GetPaymentTypeId(product.PaymentType);    //Foreign Key
                            if (!string.IsNullOrEmpty(product.BalloonAmt)) prodInfo.slm_BalloonAmt = decimal.Parse(product.BalloonAmt);
                            if (!string.IsNullOrEmpty(product.BalloonPercent)) prodInfo.slm_BalloonPercent = decimal.Parse(product.BalloonPercent);
                            if (product.Plantype != null) prodInfo.slm_PlanType = product.Plantype;                                  //Foreign Key
                            if (product.CoverageDate != null) prodInfo.slm_CoverageDate = product.CoverageDate;
                            if (!string.IsNullOrEmpty(product.AccType)) prodInfo.slm_AccType = master.GetModuleId(decimal.Parse(product.AccType));  //Foreign Key
                            if (!string.IsNullOrEmpty(product.AccPromotion)) prodInfo.slm_AccPromotion = master.GetPromotionId(product.AccPromotion);   //Foreign Key
                            if (product.AccTerm != null) prodInfo.slm_AccTerm = product.AccTerm;
                            if (product.Interest != null) prodInfo.slm_Interest = product.Interest;
                            if (product.Invest != null) prodInfo.slm_Invest = product.Invest;
                            if (product.LoanOd != null) prodInfo.slm_LoanOd = product.LoanOd;
                            if (product.LoanOdTerm != null) prodInfo.slm_LoanOdTerm = product.LoanOdTerm;
                            if (product.SlmBank != null) prodInfo.slm_Ebank = product.SlmBank;
                            if (product.SlmAtm != null) prodInfo.slm_Atm = product.SlmAtm;
                            if (product.OtherDetail1 != null) prodInfo.slm_OtherDetail_1 = product.OtherDetail1;
                            if (product.OtherDetail2 != null) prodInfo.slm_OtherDetail_2 = product.OtherDetail2;
                            if (product.OtherDetail3 != null) prodInfo.slm_OtherDetail_3 = product.OtherDetail3;
                            if (product.OtherDetail4 != null) prodInfo.slm_OtherDetail_4 = product.OtherDetail4;

                            //Modified By Pom 2016/01/05
                            //if (!string.IsNullOrEmpty(product.Brand)) prodInfo.slm_Brand = master.GetBrandId(product.Brand);    //Foreign Key                       
                            //if (!string.IsNullOrEmpty(product.Brand) && !string.IsNullOrEmpty(product.Model))
                            //    prodInfo.slm_Model = master.GetModelId(product.Brand, product.Model);                           //Foreign Key

                            //if (!string.IsNullOrEmpty(product.Submodel))
                            //{
                            //    decimal result;
                            //    if (decimal.TryParse(product.Submodel, out result))
                            //        prodInfo.slm_Submodel = master.GetSubModelId(decimal.Parse(product.Submodel));                  //Foreign Key          
                            //}
                        }

                        slmdb.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
                    }
                }
                else
                    throw new ServiceException(ApplicationResource.UPD_NO_RECORD_FOUND_CODE, ApplicationResource.UPD_NO_RECORD_FOUND_DESC);
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }
    }
}
