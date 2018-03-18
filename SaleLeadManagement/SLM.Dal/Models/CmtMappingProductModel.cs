using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class CmtMappingProductModel
    {
        public List<ControlListData> GetProductData(string productGroupId)
        {
            string sql = @"SELECT sub_product_id AS ValueField, sub_product_name AS TextField FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT
                            WHERE (is_deleted = 0 OR is_deleted IS NULL) AND product_id = '" + productGroupId + @"'
                            ORDER BY sub_product_name";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

		public List<ControlListData> GetProductDataNew(string productGroupId)
        {
            string sql = @"SELECT sub_product_id AS ValueField, sub_product_name AS TextField FROM CMT_MAPPING_PRODUCT
                            WHERE (is_deleted = 0 OR is_deleted IS NULL) AND product_id = '" + productGroupId + @"' AND (sub_product_type = 0 OR sub_product_type = 2)
                            ORDER BY sub_product_name";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        } 
        public List<ControlListData> GetProductList()
        {
            string sql = @"SELECT sub_product_id AS ValueField, sub_product_name AS TextField FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT
                            WHERE (is_deleted = 0 OR is_deleted IS NULL) 
                            ORDER BY sub_product_name";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

		public List<ControlListData> GetProductListNew()
        {
            string sql = @"SELECT sub_product_id AS ValueField, sub_product_name AS TextField FROM CMT_MAPPING_PRODUCT
                            WHERE (is_deleted = 0 OR is_deleted IS NULL) AND (sub_product_type = 0 OR sub_product_type = 2)
                            ORDER BY sub_product_name";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        } 
        public List<ControlListData> GetProductDataFromSubProduct(string subProductGroupId)
        {
            string sql = @"SELECT sub_product_id AS ValueField, sub_product_name AS TextField FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT
                            WHERE (is_deleted = 0 OR is_deleted IS NULL) AND sub_product_id = '" + subProductGroupId + @"'
                            ORDER BY sub_product_name";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public List<ControlListData> GetMappingProductNew(string campaignId)
        {
            string sql = @"SELECT PRO.PR_CampaignId, PRO.PR_ProductId FROM CMT_CAMPAIGN_PRODUCT AS PRO
                            INNER JOIN CMT_MAPPING_PRODUCT AS MP ON PRO.PR_ProductId = MP.sub_product_id
                            WHERE PRO.PR_CampaignId = '"+ campaignId + @"' AND (is_deleted = 0 OR is_deleted IS NULL) AND (MP.sub_product_type = 0 OR MP.sub_product_type = 2)";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }
    }
}
