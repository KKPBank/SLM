using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Biz
{
    public class ConfigProductScreenBiz
    {
        public string GetFieldType(string ticketId, string actionType)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();

                    string sql = $"SELECT slm_CampaignId AS CampaignId, slm_Product_Id AS ProductId FROM kkslm_tr_lead WITH (NOLOCK) WHERE slm_ticketId = '{ticketId}' ";
                    var lead = slmdb.ExecuteStoreQuery<LeadDefaultData>(sql).FirstOrDefault();
                    if (lead != null)
                    {
                        return GetFieldType(lead.CampaignId, lead.ProductId, actionType);
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public string GetFieldType(string campaignId, string productId, string actionType)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string type = string.Empty;

                    type = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_CampaignId == campaignId && p.slm_ActionType == actionType && p.is_Deleted == false)
                                .Select(p => p.slm_Type).FirstOrDefault();

                    if (string.IsNullOrEmpty(type))
                    {
                        type = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_Product_Id == productId && p.slm_ActionType == actionType && p.is_Deleted == false)
                                .Select(p => p.slm_Type).FirstOrDefault();
                    }

                    return type != null ? type : "";
                }
            }
            catch
            {
                throw;
            }
        }

        public ConfigProductScreenData GetData(string campaignId, string productId, string actionType)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    ConfigProductScreenData data = null;

                    data = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_CampaignId == campaignId && p.slm_ActionType == actionType && p.is_Deleted == false)
                                    .Select(p => new ConfigProductScreenData
                                    {
                                        ScreenCode = p.slm_ScreenCode,
                                        TableName = p.slm_TableName,
                                        Type = p.slm_Type
                                    }).FirstOrDefault();

                    if (data == null)
                    {
                        data = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_Product_Id == productId && p.slm_ActionType == actionType && p.is_Deleted == false)
                                    .Select(p => new ConfigProductScreenData
                                    {
                                        ScreenCode = p.slm_ScreenCode,
                                        TableName = p.slm_TableName,
                                        Type = p.slm_Type
                                    }).FirstOrDefault();
                    }

                    return data;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
