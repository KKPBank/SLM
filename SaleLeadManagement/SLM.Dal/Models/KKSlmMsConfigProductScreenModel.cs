using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsConfigProductScreenModel
    {
        public string GetTableName(string campaignId, string productId, string actionType)
        {
            try
            {
                string tableName = "";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var config_cam = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_CampaignId == campaignId && p.slm_ActionType == actionType && p.is_Deleted == false).FirstOrDefault();
                    if (config_cam != null)
                        tableName = config_cam.slm_TableName;
                    else
                    {
                        var config_pro = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_Product_Id == productId && p.slm_ActionType == actionType && p.is_Deleted == false).FirstOrDefault();
                        if (config_pro != null)
                            tableName = config_pro.slm_TableName;
                    }
                }

                return tableName;
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetCampaignsFromConfigProductScreen()
        {
            try
            {
                string sql = @"select distinct RTRIM(LTRIM(A.CampaignId)) AS CampaignId
                                from (
	                                select distinct p.PR_CampaignId AS CampaignId
	                                from kkslm_ms_config_product_screen c
	                                inner join CMT_CAMPAIGN_PRODUCT p on c.slm_Product_Id = p.PR_ProductId
	                                where c.slm_Product_Id is not null
	                                union
	                                select slm_CampaignId AS CampaignId
	                                from kkslm_ms_config_product_screen
	                                where slm_CampaignId is not null) A ";

                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<string>(sql).ToList();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
