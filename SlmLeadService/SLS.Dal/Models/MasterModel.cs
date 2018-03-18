using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Dal.Models
{
    public class MasterModel
    {
        private SLM_DBEntities slmdb = null;

        public MasterModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public int? GetProvinceId(string provinceCode)
        {
            var id = slmdb.kkslm_ms_province.Where(p => p.slm_ProvinceCode == provinceCode).Select(p => p.slm_ProvinceId).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }

        public int? GetAmphurId(string provinceCode, string amphurCode)
        {
            var id = slmdb.kkslm_ms_amphur.Where(p => p.slm_ProvinceCode == provinceCode && p.slm_AmphurCode == amphurCode).Select(p => p.slm_AmphurId).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }

        public int? GetTambolId(string provinceCode, string amphurCode, string tambolCode)
        {
            var id = slmdb.kkslm_ms_tambol.Where(p => p.slm_ProvinceCode == provinceCode && p.slm_AmphurCode == amphurCode && p.slm_TambolCode == tambolCode).Select(p => p.slm_TambolId).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }

        public int? GetOccupationId(string occupationCode)
        {
            var id = slmdb.kkslm_ms_occupation.Where(p => p.slm_OccupationCode == occupationCode).Select(p => p.slm_OccupationId).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }

        public int? GetBrandId(string brandCode)
        {
            var id = slmdb.kkslm_ms_brand.Where(p => p.slm_BrandCode == brandCode).Select(p => p.slm_BrandId).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }

        public int? GetModelId(string brandCode, string familyCode)
        {
            var id = slmdb.kkslm_ms_model.Where(p => p.slm_BrandCode == brandCode && p.slm_Family == familyCode).Select(p => p.slm_ModelId).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }

        public int? GetSubModelId(decimal redBookNo)
        {
            var id = slmdb.kkslm_ms_submodel.Where(p => p.slm_RedBookNo == redBookNo).Select(p => p.slm_SubModelId).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }

        public int? GetModuleId(decimal moduleCode)
        {
            var id = slmdb.kkslm_ms_module.Where(p => p.slm_ModuleCode == moduleCode).Select(p => p.slm_ModuleId).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }

        public int? GetPromotionId(string promotionCode)
        {
            var id = slmdb.kkslm_ms_promotion.Where(p => p.slm_PromotionCode == promotionCode).Select(p => p.slm_PromotionId).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }

        public string GetPaymentTypeId(string paymentCode)
        {
            var id = slmdb.kkslm_ms_paymenttype.Where(p => p.slm_PaymentCode == paymentCode).Select(p => p.slm_PaymentId).FirstOrDefault();
            if (id != 0)
                return id.ToString();
            else
                return null;
        }

        public string GetSlmStatusFromMappingCode(string systemName, string productId, string mappingStatusCode, string mappingSubStatusCode)
        {
            string slmStatus;

            if (string.IsNullOrEmpty(mappingSubStatusCode))
                slmStatus = slmdb.kkslm_ms_mapping_status.Where(p => p.is_Deleted == false && p.slm_System == systemName && p.slm_Product_Id == productId && p.slm_Mapping_Status_Code == mappingStatusCode && p.slm_Mapping_SubStatus_Code == null).Select(p => p.slm_Status_Code).FirstOrDefault();
            else
                slmStatus = slmdb.kkslm_ms_mapping_status.Where(p => p.is_Deleted == false && p.slm_System == systemName && p.slm_Product_Id == productId && p.slm_Mapping_Status_Code == mappingStatusCode && p.slm_Mapping_SubStatus_Code == mappingSubStatusCode).Select(p => p.slm_Status_Code).FirstOrDefault();

            return slmStatus;
        }

        public int? GetCountryId(string countryCode)
        {
            var id = slmdb.kkslm_ms_country.Where(p => p.slm_CountryCode == countryCode && p.is_delete == 0).Select(p => p.slm_country_id).FirstOrDefault();
            if (id != 0)
                return id;
            else
                return null;
        }
    }
}
