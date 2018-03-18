using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsSlaModel
    {
        private string slaDefault = "DEFAULT";

        public List<SlaData> SearchConfigSla(string productId, string campaignId, string channelId, string statusCode)
        {
            string sql = @"SELECT sla.slm_SLAId AS SlaId, sla.slm_ProductId AS ProductId
                            , CASE WHEN sla.slm_ProductId = 'DEFAULT' THEN 'DEFAULT' ELSE pd.sub_product_name END AS ProductName
                            , sla.slm_CampaignId AS CampaignId
                            , CASE WHEN sla.slm_CampaignId = 'DEFAULT' THEN 'DEFAULT' ELSE cam.slm_CampaignName END AS CampaignName
                            , sla.slm_ChannelId AS ChannelId, chan.slm_ChannelDesc AS ChannelDesc, sla.slm_StatusCode AS StatusCode
                            , sla.slm_SubStatusCode as SubStatusCode
                            , CASE WHEN subst.slm_SubStatusName IS NOT NULL 
			                        AND subst2.slm_SubStatusName IS NULL THEN subst.slm_SubStatusName
                                   WHEN subst.slm_SubStatusName IS  NULL 
			                        AND subst2.slm_SubStatusName IS NOT NULL THEN subst2.slm_SubStatusName  ELSE '' END  SubStatusDesc
                            , opt.slm_OptionDesc AS StatusDesc
                            , sla.slm_SLA_Minutes AS SlaMin, sla.slm_SLA_Times AS SlaTime, sla.slm_SLA_Days AS SlaDay
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_sla sla
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT pd ON pd.sub_product_id = sla.slm_ProductId
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam ON cam.slm_CampaignId = sla.slm_CampaignId
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_channel chan ON chan.slm_ChannelId = sla.slm_ChannelId
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option opt ON sla.slm_StatusCode = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status'
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus subst ON subst.slm_SubStatusCode = sla.slm_SubStatusCode AND subst.slm_Product_id = sla.slm_Productid AND subst.slm_CampaignId IS NULL AND sla.slm_StatusCode =  subst.slm_OptionCode
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus subst2 ON subst2.slm_SubStatusCode = sla.slm_SubStatusCode AND subst2.slm_CampaignId = sla.slm_CampaignId AND subst2.slm_Product_id IS NULL AND sla.slm_StatusCode =  subst2.slm_OptionCode
                            WHERE sla.is_Deleted = 0 {0}
                            ORDER BY cam.slm_CampaignName, pd.sub_product_name, chan.slm_ChannelDesc, opt.slm_OptionDesc ";

            string whr = "";
            whr += (productId == "" ? "" : (whr == "" ? "" : " AND ") + " sla.slm_ProductId = '" + productId + "' ");
            whr += (campaignId == "" ? "" : (whr == "" ? "" : " AND ") + " sla.slm_CampaignId = '" + campaignId + "' ");
            whr += (channelId == "" ? "" : (whr == "" ? "" : " AND ") + " sla.slm_ChannelId = '" + channelId + "' ");
            whr += (statusCode == "" ? "" : (whr == "" ? "" : " AND ") + " sla.slm_StatusCode = '" + statusCode + "' ");

            whr = (whr == "" ? "" : " AND " + whr);
            sql = string.Format(sql, whr);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SlaData>(sql).ToList();
            }
        }

        public void InsertData(string productId, string campaignId, string channelId, string statusCode, string subStatusCode, int slaMin, int slaTime, int slaDay, string createBy)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    int count = 0;
                    string slaType = "";

                    if (!string.IsNullOrEmpty(productId))
                    {
                        if (!string.IsNullOrEmpty(subStatusCode))
                            count = slmdb.kkslm_ms_sla.Where(p => p.slm_ProductId == productId && p.slm_ChannelId == channelId && p.slm_StatusCode == statusCode && p.slm_SubStatusCode == subStatusCode && p.is_Deleted == 0).Count();
                        else
                            count = slmdb.kkslm_ms_sla.Where(p => p.slm_ProductId == productId && p.slm_ChannelId == channelId && p.slm_StatusCode == statusCode && p.slm_SubStatusCode == null && p.is_Deleted == 0).Count();

                        slaType = "2";
                    }
                    else if (!string.IsNullOrEmpty(campaignId))
                    {
                        if (!string.IsNullOrEmpty(subStatusCode))
                            count = slmdb.kkslm_ms_sla.Where(p => p.slm_CampaignId == campaignId && p.slm_ChannelId == channelId && p.slm_StatusCode == statusCode && p.slm_SubStatusCode == subStatusCode && p.is_Deleted == 0).Count();
                        else
                            count = slmdb.kkslm_ms_sla.Where(p => p.slm_CampaignId == campaignId && p.slm_ChannelId == channelId && p.slm_StatusCode == statusCode && p.slm_SubStatusCode == null && p.is_Deleted == 0).Count();

                        slaType = "1";
                    }
                    else
                        throw new Exception("ไม่พบ ProductId หรือ CampaignId");

                    if (count > 0)
                        throw new Exception("ข้อมูล SLA นี้มีในระบบแล้ว");

                    DateTime createDate = DateTime.Now;
                    kkslm_ms_sla sla = new kkslm_ms_sla()
                    {
                        slm_ProductId = (string.IsNullOrEmpty(productId) ? null : productId),
                        slm_CampaignId = (string.IsNullOrEmpty(campaignId) ? null : campaignId),
                        slm_ChannelId = channelId,
                        slm_StatusCode = statusCode,
                        slm_SubStatusCode = string.IsNullOrEmpty(subStatusCode) ? null : subStatusCode,
                        slm_SLA_Minutes = slaMin,
                        slm_SLA_Times = slaTime,
                        slm_SLA_Days = slaDay,
                        slm_SLA_Type = slaType,
                        slm_CreatedBy = createBy,
                        slm_CreatedDate = createDate,
                        slm_UpdatedBy = createBy,
                        slm_UpdatedDate = createDate,
                        is_Deleted = 0
                    };

                    //if (productId == slaDefault || campaignId == slaDefault)
                    //{
                    //    sla.slm_ProductId = slaDefault;
                    //    sla.slm_CampaignId = slaDefault;
                    //}

                    slmdb.kkslm_ms_sla.AddObject(sla);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateData(int slaId, string productId, string campaignId, string channelId, string statusCode, string subStatusCode, int slaMin, int slaTime, int slaDay, string updateBy)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    int count = 0;
                    string slaType = "";
                    if (!string.IsNullOrEmpty(productId))
                    {
                        if (!string.IsNullOrEmpty(subStatusCode))
                            count = slmdb.kkslm_ms_sla.Where(p => p.slm_SLAId != slaId && p.slm_ProductId == productId && p.slm_ChannelId == channelId && p.slm_StatusCode == statusCode && p.slm_SubStatusCode == subStatusCode && p.is_Deleted == 0).Count();
                        else
                            count = slmdb.kkslm_ms_sla.Where(p => p.slm_SLAId != slaId && p.slm_ProductId == productId && p.slm_ChannelId == channelId && p.slm_StatusCode == statusCode && p.slm_SubStatusCode == null && p.is_Deleted == 0).Count();

                        slaType = "2";
                    }
                    else if (!string.IsNullOrEmpty(campaignId))
                    {
                        if (!string.IsNullOrEmpty(subStatusCode))
                            count = slmdb.kkslm_ms_sla.Where(p => p.slm_SLAId != slaId && p.slm_CampaignId == campaignId && p.slm_ChannelId == channelId && p.slm_StatusCode == statusCode && p.slm_SubStatusCode == subStatusCode && p.is_Deleted == 0).Count();
                        else
                            count = slmdb.kkslm_ms_sla.Where(p => p.slm_SLAId != slaId && p.slm_CampaignId == campaignId && p.slm_ChannelId == channelId && p.slm_StatusCode == statusCode && p.slm_SubStatusCode == null && p.is_Deleted == 0).Count();

                        slaType = "1";
                    }
                    else
                        throw new Exception("ไม่พบ ProductId หรือ CampaignId");

                    if (count > 0)
                        throw new Exception("ข้อมูล SLA นี้มีในระบบแล้ว");

                    var sla = slmdb.kkslm_ms_sla.Where(p => p.slm_SLAId == slaId).FirstOrDefault();
                    if (sla != null)
                    {
                        if (productId == slaDefault || campaignId == slaDefault)
                        {
                            sla.slm_ProductId = slaDefault;
                            sla.slm_CampaignId = slaDefault;
                            sla.slm_SLA_Minutes = slaMin;
                            sla.slm_SLA_Type = "9999";
                        }
                        else
                        {
                            sla.slm_ProductId = (string.IsNullOrEmpty(productId) ? null : productId);
                            sla.slm_CampaignId = (string.IsNullOrEmpty(campaignId) ? null : campaignId);
                            sla.slm_ChannelId = channelId;
                            sla.slm_StatusCode = statusCode;
                            sla.slm_SubStatusCode = string.IsNullOrEmpty(subStatusCode) ? null : subStatusCode;
                            sla.slm_SLA_Minutes = slaMin;
                            sla.slm_SLA_Times = slaTime;
                            sla.slm_SLA_Days = slaDay;
                            sla.slm_SLA_Type = slaType;
                            sla.slm_UpdatedBy = updateBy;
                            sla.slm_UpdatedDate = DateTime.Now;
                        }

                        slmdb.SaveChanges();
                    }
                    else
                        throw new Exception("ไม่พบ SLA Id " + slaId.ToString() + " ในระบบ");
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteSla(int slaId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var sla = slmdb.kkslm_ms_sla.Where(p => p.slm_SLAId == slaId).FirstOrDefault();
                    if (sla != null)
                    {
                        slmdb.kkslm_ms_sla.DeleteObject(sla);
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
