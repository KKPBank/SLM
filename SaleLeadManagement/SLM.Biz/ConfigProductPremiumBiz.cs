using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data.SqlClient;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Biz
{
    public class ConfigProductPremiumBiz
    {
        private string _errorMessage = "";

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public bool InsertData(string productId, string campaignId, string premiumName, string premiumType, DateTime startDate, DateTime endDate, int total, bool isActive, string createByUsername)
        {
            try
            {
                DateTime createDate = DateTime.Now;
                List<kkslm_ms_config_product_premium> configList = new List<kkslm_ms_config_product_premium>();
                int newLotNo = 0;

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    if (!string.IsNullOrEmpty(productId))
                    {
                        configList = slmdb.kkslm_ms_config_product_premium.Where(p => p.slm_Product_Id == productId && p.slm_CampaignId == null && p.slm_PremiumName == premiumName && p.slm_PremiumType == premiumType && p.is_Deleted == false).ToList();
                    }
                    else if (!string.IsNullOrEmpty(campaignId))
                    {
                        configList = slmdb.kkslm_ms_config_product_premium.Where(p => p.slm_Product_Id == null && p.slm_CampaignId == campaignId && p.slm_PremiumName == premiumName && p.slm_PremiumType == premiumType && p.is_Deleted == false).ToList();
                    }

                    //ถ้าของแถมใหม่เลือกสถานะเป็นใช้งาน ให้ไปของเซ็ทของแถมนั้นที่มีอยู่ในเบสเป็น ไม่ใช้งาน
                    if (isActive)
                        configList.Where(p => p.slm_IsActive == true).ToList().ForEach(p => { p.slm_IsActive = false; p.slm_UpdatedBy = createByUsername; p.slm_UpdatedDate = createDate; });

                    //Get Latest LotNo
                    int? lotNo = configList.OrderByDescending(p => p.slm_CreatedDate).Select(p => p.slm_LotNo).FirstOrDefault();
                    if (lotNo != null)
                        newLotNo = lotNo.Value + 1;
                    else
                        newLotNo += 1;

                    kkslm_ms_config_product_premium config = new kkslm_ms_config_product_premium();
                    if (!string.IsNullOrEmpty(productId)) config.slm_Product_Id = productId;
                    if (!string.IsNullOrEmpty(campaignId)) config.slm_CampaignId = campaignId;
                    config.slm_PremiumName = premiumName;
                    config.slm_PremiumType = premiumType;
                    config.slm_StartDate = startDate;
                    config.slm_EndDate = endDate;
                    config.slm_LotNo = newLotNo;
                    config.slm_Total = total;
                    config.slm_Used = 0;
                    config.slm_Remain = total;
                    config.slm_IsActive = isActive;
                    config.is_Deleted = false;

                    config.slm_CreatedBy = createByUsername;
                    config.slm_CreatedDate = createDate;
                    config.slm_UpdatedBy = createByUsername;
                    config.slm_UpdatedDate = createDate;

                    slmdb.kkslm_ms_config_product_premium.AddObject(config);
                    slmdb.SaveChanges();

                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ConfigProductPremiumData> SearchConfigPremium(string productId, string campaignId, string premiumName, string premiumType, bool statusActive, bool statusInActive)
        {
            try
            {
                string sql = @"SELECT con.slm_cp_Premium_id AS PremiumId, con.slm_PremiumName AS PremiumName, con.slm_PremiumType AS PremiumType, pd.sub_product_name AS ProductName, cam.slm_CampaignName AS CampaignName
                                , con.slm_LotNo AS LotNo, con.slm_StartDate AS StartDate, con.slm_EndDate AS EndDate, con.slm_Total AS TotalAll, con.slm_Used AS TotalUsed
                                , con.slm_Remain AS TotalRemain, con.slm_IsActive AS [Status], con.slm_CreatedDate AS CreatedDate, con.slm_Product_Id AS ProductId, con.slm_CampaignId AS CampaignId
                                , con.slm_PremiumType AS PremiumType
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_premium con
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT pd ON pd.sub_product_id = con.slm_Product_Id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam ON cam.slm_CampaignId = con.slm_CampaignId
                                WHERE con.is_Deleted = 0 {0} 
                                ORDER BY cam.slm_CampaignName, pd.sub_product_name, con.slm_PremiumName, con.slm_LotNo DESC, con.slm_CreatedDate DESC ";

                string whr = "";
                whr += (productId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_Product_Id = '" + productId + "' ");
                whr += (campaignId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_CampaignId = '" + campaignId + "' ");
                whr += (premiumName == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_PremiumName LIKE @premium_name ");
                whr += (premiumType == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_PremiumType = '" + premiumType + "' ");

                if (statusActive == true && statusInActive == false)
                    whr += (whr == "" ? "" : " AND ") + " con.slm_IsActive = '1' ";
                else if (statusActive == false && statusInActive == true)
                    whr += (whr == "" ? "" : " AND ") + " con.slm_IsActive = '0' ";

                whr = (whr == "" ? "" : " AND " + whr);
                sql = string.Format(sql, whr);

                object[] param = new object[] 
                { 
                    new SqlParameter("@premium_name", "%" + premiumName + "%")
                };

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<ConfigProductPremiumData>(sql, param).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ConfigProductPremiumData> SearchConfigPremiumForEdit(string productId, string campaignId, string premiumName, string premiumType, bool statusActive, bool statusInActive)
        {
            try
            {
                string sql = @"SELECT con.slm_cp_Premium_id AS PremiumId, con.slm_PremiumName AS PremiumName, con.slm_PremiumType AS PremiumType, pd.sub_product_name AS ProductName, cam.slm_CampaignName AS CampaignName
                                , con.slm_LotNo AS LotNo, con.slm_StartDate AS StartDate, con.slm_EndDate AS EndDate, con.slm_Total AS TotalAll, con.slm_Used AS TotalUsed
                                , con.slm_Remain AS TotalRemain, con.slm_IsActive AS [Status], con.slm_CreatedDate AS CreatedDate, con.slm_Product_Id AS ProductId, con.slm_CampaignId AS CampaignId
                                , con.slm_PremiumType AS PremiumType
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_premium con
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT pd ON pd.sub_product_id = con.slm_Product_Id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam ON cam.slm_CampaignId = con.slm_CampaignId
                                WHERE con.is_Deleted = 0 {0} 
                                ORDER BY cam.slm_CampaignName, pd.sub_product_name, con.slm_PremiumName, con.slm_LotNo DESC, con.slm_CreatedDate DESC ";

                string whr = "";
                whr += (productId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_Product_Id = '" + productId + "' ");
                whr += (campaignId == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_CampaignId = '" + campaignId + "' ");
                whr += (premiumName == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_PremiumName = '" + premiumName + "' ");
                whr += (premiumType == "" ? "" : (whr == "" ? "" : " AND ") + " con.slm_PremiumType = '" + premiumType + "' ");

                if (statusActive == true && statusInActive == false)
                    whr += (whr == "" ? "" : " AND ") + " con.slm_IsActive = '1' ";
                else if (statusActive == false && statusInActive == true)
                    whr += (whr == "" ? "" : " AND ") + " con.slm_IsActive = '0' ";

                whr = (whr == "" ? "" : " AND " + whr);
                sql = string.Format(sql, whr);

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<ConfigProductPremiumData>(sql).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void ChangeActiveLot(string productId, string campaignId, string premiumName, string premiumTypeCode, decimal currentPremiumId, bool currentActiveChanged, string updateByUsername)
        {
            try
            {
                DateTime updateDate = DateTime.Now;
                List<kkslm_ms_config_product_premium> configList = new List<kkslm_ms_config_product_premium>();

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var current = slmdb.kkslm_ms_config_product_premium.Where(p => p.slm_cp_Premium_id == currentPremiumId).FirstOrDefault();
                    if (current != null)
                    {
                        current.slm_IsActive = currentActiveChanged;
                        current.slm_UpdatedBy = updateByUsername;
                        current.slm_UpdatedDate = updateDate;
                    }

                    //ถ้า current ถูกเซ็ทให้เป็นใช้งาน ให้ทำการเซ็ทตัวที่ใช้งานเดิม ให้เป็นไม่ใช้งาน
                    if (currentActiveChanged)
                    {
                        if (!string.IsNullOrEmpty(productId))
                        {
                            configList = slmdb.kkslm_ms_config_product_premium.Where(p => p.slm_cp_Premium_id != currentPremiumId && p.slm_Product_Id == productId && p.slm_CampaignId == null && p.slm_PremiumName == premiumName
                                && p.slm_PremiumType == premiumTypeCode && p.slm_IsActive == true && p.is_Deleted == false).ToList();
                        }
                        else if (!string.IsNullOrEmpty(campaignId))
                        {
                            configList = slmdb.kkslm_ms_config_product_premium.Where(p => p.slm_cp_Premium_id != currentPremiumId && p.slm_Product_Id == null && p.slm_CampaignId == campaignId && p.slm_PremiumName == premiumName
                                && p.slm_PremiumType == premiumTypeCode && p.slm_IsActive == true && p.is_Deleted == false).ToList();
                        }

                        configList.ForEach(p => { p.slm_IsActive = false; p.slm_UpdatedBy = updateByUsername; p.slm_UpdatedDate = updateDate; });
                    }

                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        //Added By Pom 26/04/2016
        public List<ConfigProductPremiumData> GetPremiumList(string campaignId, string productId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<ConfigProductPremiumData> list = new List<ConfigProductPremiumData>();
                    string sql = @"SELECT slm_cp_Premium_id AS PremiumId, slm_premiumName AS PremiumName , slm_PremiumType AS PremiumType, slm_Remain AS TotalRemain
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_premium
                                WHERE slm_CampaignId = '" + campaignId + @"' AND slm_IsActive = '1' AND is_Deleted = '0' 
                                AND CONVERT(DATE, GETDATE()) BETWEEN CONVERT(DATE, slm_StartDate) AND CONVERT(DATE, slm_EndDate) 
                                ORDER BY slm_PremiumName ";

                    list = slmdb.ExecuteStoreQuery<ConfigProductPremiumData>(sql).ToList();

                    if (list.Count == 0)
                    {
                        sql = @"SELECT slm_cp_Premium_id AS PremiumId, slm_premiumName AS PremiumName , slm_PremiumType AS PremiumType, slm_Remain AS TotalRemain
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_premium
                            WHERE slm_Product_Id = '" + productId + @"' AND slm_IsActive = '1' AND is_Deleted = '0' 
                            AND CONVERT(DATE, GETDATE()) BETWEEN CONVERT(DATE, slm_StartDate) AND CONVERT(DATE, slm_EndDate) 
                            ORDER BY slm_PremiumName ";

                        list = slmdb.ExecuteStoreQuery<ConfigProductPremiumData>(sql).ToList();
                    }

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        //Added By Pom 21/05/2016
        public void InsertPremiumTransaction(List<PremiumData> list, string createByUsername)
        {
            try
            {
                if (list.Count > 0)
                {
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        string ticketId = list[0].TicketId;
                        var renewId = slmdb.kkslm_tr_renewinsurance.Where(p => p.slm_TicketId == ticketId).Select(p => p.slm_RenewInsureId).FirstOrDefault();

                        DateTime createDate = DateTime.Now;
                        foreach (PremiumData data in list)
                        {
                            kkslm_tr_renewinsurance_rapremium ra = new kkslm_tr_renewinsurance_rapremium()
                            {
                                slm_RenewInsureId = renewId,
                                slm_cp_Premium_id = data.PremiumId,
                                slm_Use = data.TotalGive,
                                slm_CreatedBy = createByUsername,
                                slm_CreatedDate = createDate,
                                slm_UpdatedBy = createByUsername,
                                slm_UpdatedDate = createDate,
                                is_Deleted = false
                            };
                            slmdb.kkslm_tr_renewinsurance_rapremium.AddObject(ra);

                            var premium_master = slmdb.kkslm_ms_config_product_premium.Where(p => p.slm_cp_Premium_id == data.PremiumId).FirstOrDefault();
                            if (premium_master != null)
                            {
                                premium_master.slm_Used = (premium_master.slm_Used != null ? premium_master.slm_Used.Value : 0) + data.TotalGive;
                                premium_master.slm_Remain = (premium_master.slm_Remain != null ? premium_master.slm_Remain.Value : 0) - data.TotalGive;
                                premium_master.slm_UpdatedBy = createByUsername;
                                premium_master.slm_UpdatedDate = createDate;
                            }
                        }

                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        //Added By Pom 21/05/2016
        public List<PremiumData> GetPremiumByTicketId(string ticketId, string productId, string campaignId)
        {
            try
            {
                //var renewId = slmdb.kkslm_tr_renewinsurance.Where(p => p.slm_TicketId == ticketId).Select(p => p.slm_RenewInsureId).FirstOrDefault();
                //int count = slmdb.kkslm_tr_renewinsurance_rapremium.Where(p => p.slm_RenewInsureId == renewId).Count();
                //return count > 0 ? true : false;

                List<PremiumData> displayList = new List<PremiumData>();
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //Must use INNER JOIN
                    string sql = @"SELECT ra.slm_RenewInsureRAId AS RaId, premium_mt.slm_PremiumName AS PremiumName, premium_mt.slm_PremiumType AS PremiumType, ra.slm_Use AS TotalGive, ra.slm_CreatedDate AS CreatedDate
                                , premium_mt.slm_Remain AS TotalRemain
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance renew
                                INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_rapremium ra ON ra.slm_RenewInsureId = renew.slm_RenewInsureId
                                INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_premium premium_mt ON premium_mt.slm_cp_Premium_id = ra.slm_cp_Premium_id
                                WHERE renew.slm_TicketId = '" + ticketId + "' AND ra.is_Deleted = 0 ";

                    displayList = slmdb.ExecuteStoreQuery<PremiumData>(sql).ToList();   //ของที่ได้ไปแล้ว ตั้งต้นไว้ใน List
                }

                //Get Master Premium
                var masterList = GetPremiumList(campaignId, productId);
                foreach (ConfigProductPremiumData masterData in masterList)
                {
                    int count = displayList.Where(p => p.PremiumName == masterData.PremiumName && p.PremiumType == masterData.PremiumType).Count();
                    if (count == 0)
                    {
                        //เพิ่ม Master ที่ยังไม่ได้อยู่ใน list ของที่ให้
                        PremiumData temp = new PremiumData()
                        {
                            PremiumId = masterData.PremiumId,
                            PremiumType = masterData.PremiumType,
                            PremiumName = masterData.PremiumName,
                            TotalRemain = masterData.TotalRemain
                        };
                        displayList.Add(temp);
                    }
                }

                displayList = displayList.OrderBy(p => p.PremiumName).ToList();
                return displayList;
            }
            catch
            {
                throw;
            }
        }

        public void ReturnPremium(decimal raId)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        var item = slmdb.kkslm_tr_renewinsurance_rapremium.Where(p => p.slm_RenewInsureRAId == raId).FirstOrDefault();
                        if (item != null)
                        {
                            int use = item.slm_Use != null ? item.slm_Use.Value : 0;
                            var master = slmdb.kkslm_ms_config_product_premium.Where(p => p.slm_cp_Premium_id == item.slm_cp_Premium_id).FirstOrDefault();
                            if (master != null)
                            {
                                if (master.slm_IsActive == false)
                                {
                                    throw new Exception("ไม่สามารถคืนได้ เนื่องจาก " + master.slm_PremiumName + " Lot ที่ " + (master.slm_LotNo != null ? master.slm_LotNo.Value.ToString() : "") + " ได้ถูกยกเลิกการใช้งานแล้ว");
                                }

                                //คืนของกลับไปที่ Master
                                master.slm_Used -= use;
                                master.slm_Remain += use;

                                //ลบ transaction item
                                slmdb.kkslm_tr_renewinsurance_rapremium.DeleteObject(item);

                                slmdb.SaveChanges();
                            }
                        }
                    }

                    ts.Complete();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
