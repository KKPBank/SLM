using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsConfigProductSubstatusModel
    {
        public List<ControlListData> GetSubOptionList(string productId, string campaignId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (productId == "" || productId == "0")
                {
                    CmtCampaignProductModel cp = new CmtCampaignProductModel();
                    List<ProductData> prod = cp.GetProductCampaignDataForObt(campaignId);
                    if (prod.Count > 0)
                        productId = prod[0].ProductId;
                }
                var query = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_CampaignId == campaignId && p.is_Deleted == false).Select(p => new ControlListData { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
                if (query.Count == 0)
                    query = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_Product_Id == productId && p.is_Deleted == false).Select(p => new ControlListData { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();

                return query.ToList();
            }
        }

        public List<ControlListData> GetOptionList(bool isFollowup, bool isOutbound, string productId, string campaignId)
        {
            var query = new List<ControlListData>();
            IEnumerable<ControlListData> tmpquery = new List<ControlListData>();
            if (productId == "" || productId == "0")
            {
                CmtCampaignProductModel cp = new CmtCampaignProductModel();
                List<ProductData> prod = cp.GetProductCampaignDataForObt(campaignId);
                if (prod.Count > 0)
                    productId = prod[0].ProductId;
            }

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (isOutbound)
                {
                    var queryOutbound = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_CampaignId == campaignId && p.is_Deleted == false).Select(p => new ControlListData { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
                    if (queryOutbound.Count == 0)
                        queryOutbound = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_Product_Id == productId && p.is_Deleted == false).Select(p => new ControlListData { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
                    if (query.Count != 0)
                    {
                        if (queryOutbound.Count != 0)
                            tmpquery = query.Union(queryOutbound);
                    }
                    else
                        query = queryOutbound;
                }
                if (isFollowup)
                {
                    var queryFollow = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_CampaignId == campaignId && p.is_Deleted == false && p.slm_IsFollowup == true).Select(p => new ControlListData { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
                    if (queryFollow.Count == 0)
                        queryFollow = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_Product_Id == productId && p.is_Deleted == false && p.slm_IsFollowup == true).Select(p => new ControlListData { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
                    if (query.Count != 0)
                    {
                        if (queryFollow.Count != 0)
                            tmpquery = query.Union(queryFollow);
                    }
                    else
                        query = queryFollow;
                }

                return query.ToList();
            }
        }

        public List<ControlListData> GetSubStatusList(string productId, string campaignId, string statusCode)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    if (string.IsNullOrEmpty(productId))
                    {
                        string sql = "SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WHERE PR_CampaignId = '" + campaignId + "'";
                        productId = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
                    }

                    List<ControlListData> list = new List<ControlListData>();

                    list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_CampaignId == campaignId && p.slm_OptionCode == statusCode).OrderBy(p => p.slm_SubStatusName)
                                    .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();

                    if (list.Count == 0)
                    {
                        list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_OptionCode == statusCode).OrderBy(p => p.slm_SubStatusName)
                                    .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
                    }

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ConfigProductSubStatusData> GetSubStatusList(string productId, string campaignId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    if (string.IsNullOrEmpty(productId))
                    {
                        string sql = "SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WHERE PR_CampaignId = '" + campaignId + "'";
                        productId = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
                    }

                    List<ConfigProductSubStatusData> list = new List<ConfigProductSubStatusData>();

                    list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_CampaignId == campaignId).OrderBy(p => p.slm_SubStatusName)
                                    .Select(p => new ConfigProductSubStatusData() { StatusCode = p.slm_OptionCode, SubStatusName = p.slm_SubStatusName, SubStatusCode = p.slm_SubStatusCode }).ToList();

                    if (list.Count == 0)
                    {
                        list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId).OrderBy(p => p.slm_SubStatusName)
                                    .Select(p => new ConfigProductSubStatusData() { StatusCode = p.slm_OptionCode, SubStatusName = p.slm_SubStatusName, SubStatusCode = p.slm_SubStatusCode }).ToList();
                    }

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ControlListData> GetSubStatusActiveList(string productId, string campaignId, string statusCode)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    if (string.IsNullOrEmpty(productId))
                    {
                        string sql = "SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WHERE PR_CampaignId = '" + campaignId + "'";
                        productId = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
                    }

                    List<ControlListData> list = new List<ControlListData>();
                    string[] dec = { "06", "07", "08", "09" };

                    list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_CampaignId == campaignId && p.slm_OptionCode == statusCode && dec.Contains(p.slm_SubStatusCode) == false).OrderBy(p => p.slm_SubStatusCode)
                                    .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();

                    if (list.Count == 0)
                    {
                        list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_OptionCode == statusCode && dec.Contains(p.slm_SubStatusCode) == false).OrderBy(p => p.slm_SubStatusCode)
                                    .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
                    }

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ConfigProductSubStatusData> GetSubStatusListByStatusActivityConfig(string productId, string campaignId, List<string> statusValueList)
        {
            try
            {
                string sql = "";
                string strStatusValueList = "";
                List<ConfigProductSubStatusData> list = new List<ConfigProductSubStatusData>();

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    if (statusValueList.Count > 0)
                    {
                        if (string.IsNullOrEmpty(productId))
                        {
                            sql = "SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WHERE PR_CampaignId = '" + campaignId + "'";
                            productId = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
                        }

                        foreach (string data in statusValueList)
                        {
                            strStatusValueList += (strStatusValueList != "" ? "," : "") + "'" + data + "'";
                        }

                        sql = @"SELECT substatus.slm_SubStatusName AS SubStatusName, substatus.slm_SubStatusCode AS SubStatusCode, substatus.slm_OptionCode AS StatusCode
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus substatus
                                WHERE substatus.is_Deleted = 0 AND substatus.slm_CampaignId = '" + campaignId + @"'
                                AND substatus.slm_OptionCode IN (" + strStatusValueList + @")
                                ORDER BY substatus.slm_SubStatusName ";

                        list = slmdb.ExecuteStoreQuery<ConfigProductSubStatusData>(sql).ToList();
                        if (list.Count == 0)
                        {
                            sql = @"SELECT substatus.slm_SubStatusName AS SubStatusName, substatus.slm_SubStatusCode AS SubStatusCode, substatus.slm_OptionCode AS StatusCode
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus substatus
                                WHERE substatus.is_Deleted = 0 AND substatus.slm_Product_Id = '" + productId + @"'
                                AND substatus.slm_OptionCode IN (" + strStatusValueList + @")
                                ORDER BY substatus.slm_SubStatusName ";

                            list = slmdb.ExecuteStoreQuery<ConfigProductSubStatusData>(sql).ToList();
                        }
                    }

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ControlListData> GetSubStatusListByCampaignId(string campaignId, string statusCode)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<ControlListData> list = new List<ControlListData>();

                    list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_CampaignId == campaignId && p.slm_Product_Id == null && p.slm_OptionCode == statusCode).OrderBy(p => p.slm_SubStatusName)
                                    .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ControlListData> GetSubStatusListByProductId(string productId, string statusCode)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<ControlListData> list = new List<ControlListData>();

                    list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_CampaignId == null && p.slm_OptionCode == statusCode).OrderBy(p => p.slm_SubStatusName)
                                    .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

//        public List<ControlListData> GetStatus(string productId, string campaignId, string subStatusCode)
//        {
//            try
//            {
//                string sql = "";
//                List<ControlListData> list = new List<ControlListData>();

//                if (string.IsNullOrEmpty(productId))
//                {
//                    sql = "SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WHERE PR_CampaignId = '" + campaignId + "'";
//                    productId = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
//                }

//                sql = @"SELECT opt.slm_OptionCode AS ValueField, opt.slm_OptionDesc AS TextField
//                        FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus sub
//                        INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option opt ON sub.slm_OptionCode = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status'
//                        WHERE sub.slm_CampaignId = '" + campaignId + @"' AND sub.slm_SubStatusCode = '" + subStatusCode + "' ";

//                list = slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
//                if (list.Count == 0)
//                {
//                    sql = @"SELECT opt.slm_OptionCode AS ValueField, opt.slm_OptionDesc AS TextField
//                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus sub
//                            INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option opt ON sub.slm_OptionCode = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status'
//                            WHERE sub.slm_Product_Id = '" + productId + @"' AND sub.slm_SubStatusCode = '" + subStatusCode + "' ";

//                    list = slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
//                }

//                return list;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        public List<ControlListData> GetInboundSubStatusList(string productId, string campaignId)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(productId))
//                {
//                    string sql = "SELECT PR_ProductId FROM CMT_CAMPAIGN_PRODUCT WHERE PR_CampaignId = '" + campaignId + "'";
//                    productId = slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
//                }

//                List<ControlListData> list = new List<ControlListData>();

//                list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_CampaignId == campaignId && p.slm_OptionCode != "16").OrderBy(p => p.slm_SubStatusName)
//                                .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();

//                if (list.Count == 0)
//                {
//                    list = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_OptionCode != "16").OrderBy(p => p.slm_SubStatusName)
//                                .Select(p => new ControlListData() { TextField = p.slm_SubStatusName, ValueField = p.slm_SubStatusCode }).ToList();
//                }

//                return list;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
    }
}
