using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;
using SLS.Resource.Data;

namespace SLS.Dal.Models
{
    public class SlmTrRenewInsuranceModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmTrRenewInsuranceModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public void InsertData(string ticketId, ProductInfo prodInfo, ChannelInfo chanInfo)
        {
            try
            {
                kkslm_tr_renewinsurance reins = new kkslm_tr_renewinsurance();
                reins.slm_TicketId = ticketId;

                if (!string.IsNullOrEmpty(prodInfo.Brand)) reins.slm_RedbookBrandCodeExt = prodInfo.Brand;
                if (!string.IsNullOrEmpty(prodInfo.Model)) reins.slm_RedbookModelCodeExt = prodInfo.Model;
                if (!string.IsNullOrEmpty(prodInfo.Submodel))
                {
                    reins.slm_RedbookKKKeyExt = prodInfo.Submodel;
                    var submodel = slmdb.kkslm_ms_redbook_submodel.Where(p => p.slm_KKKey == prodInfo.Submodel).FirstOrDefault();
                    if (submodel != null)
                    {
                        reins.slm_RedbookBrandCode = submodel.slm_BrandCode;
                        reins.slm_RedbookModelCode = submodel.slm_ModelCode;
                        reins.slm_RedbookKKKey = submodel.slm_KKKey;
                        reins.slm_RedbookYearGroup = submodel.slm_YearGroup;
                    }
                }

                if (!string.IsNullOrEmpty(prodInfo.LicenseNo)) reins.slm_LicenseNo = prodInfo.LicenseNo;

                if (chanInfo != null)
                {
                    if (!string.IsNullOrEmpty(chanInfo.CreateUser))
                    {
                        reins.slm_CreatedBy = chanInfo.CreateUser;
                        reins.slm_UpdatedBy = chanInfo.CreateUser;
                    }
                    else
                    {
                        reins.slm_CreatedBy = "SYSTEM";
                        reins.slm_UpdatedBy = "SYSTEM";
                    }
                }
                else
                {
                    reins.slm_CreatedBy = "SYSTEM";
                    reins.slm_UpdatedBy = "SYSTEM";
                }

                DateTime createDate = DateTime.Now;
                reins.slm_CreatedDate = createDate;
                reins.slm_UpdatedDate = createDate;

                slmdb.kkslm_tr_renewinsurance.AddObject(reins);
                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ServiceException(ApplicationResource.INS_INSERT_FAIL_CODE, ApplicationResource.INS_INSERT_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        public void UpdateDataByHpaofl(string ticketId, ProductInfo prodInfo, ChannelInfo chanInfo)
        {
            try
            {
                var reins = slmdb.kkslm_tr_renewinsurance.Where(p => p.slm_TicketId.Equals(ticketId)).FirstOrDefault();
                if (reins != null)
                {
                    if (prodInfo.Brand != null)
                        reins.slm_RedbookBrandCodeExt = prodInfo.Brand.Trim() != "" ? prodInfo.Brand.Trim() : null;

                    if (prodInfo.Model != null)
                        reins.slm_RedbookModelCodeExt = prodInfo.Model.Trim() != "" ? prodInfo.Model.Trim() : null;

                    if (prodInfo.Submodel != null)
                    {
                        reins.slm_RedbookKKKeyExt = prodInfo.Submodel.Trim() != "" ? prodInfo.Submodel.Trim() : null;
                        var submodel = slmdb.kkslm_ms_redbook_submodel.Where(p => p.slm_KKKey == prodInfo.Submodel).FirstOrDefault();
                        if (submodel != null)
                        {
                            reins.slm_RedbookBrandCode = submodel.slm_BrandCode;
                            reins.slm_RedbookModelCode = submodel.slm_ModelCode;
                            reins.slm_RedbookKKKey = submodel.slm_KKKey;
                            reins.slm_RedbookYearGroup = submodel.slm_YearGroup;
                        }
                        else
                        {
                            reins.slm_RedbookBrandCode = null;
                            reins.slm_RedbookModelCode = null;
                            reins.slm_RedbookKKKey = null;
                            reins.slm_RedbookYearGroup = null;
                        }
                    }

                    if (prodInfo.LicenseNo != null) reins.slm_LicenseNo = prodInfo.LicenseNo;

                    if (chanInfo != null)
                    {
                        if (!string.IsNullOrEmpty(chanInfo.CreateUser))
                            reins.slm_UpdatedBy = chanInfo.CreateUser;
                        else
                            reins.slm_UpdatedBy = "SYSTEM";
                    }
                    else
                        reins.slm_UpdatedBy = "SYSTEM";

                    reins.slm_UpdatedDate = DateTime.Now;
                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }
    }
}
