using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsConfigActivityModel
    {
        public List<ActivityConfigData> SearchActivityConfig(string productId, string leadStatus, string rightAdd, string availableStatus)
        {
            string SLMDBName = SLMConstant.SLMDBName;
            string sql = @"SELECT ca.slm_Product_Id AS ProductId, pd.sub_product_name AS ProductName, ca.slm_LeadStatus AS LeadStatusCode, opt.slm_OptionDesc AS LeadStatusDesc
                            , CASE WHEN ca.slm_HaveRightAdd = '0' THEN 'ไม่มีสิทธิ์'
	                               WHEN ca.slm_HaveRightAdd = '1' THEN 'มีสิทธิ์'
                              ELSE '' END AS HaveRightAddDesc, ca.slm_HaveRightAdd AS HaveRightAdd
                            , ca.slm_AvailableStatus AS LeadAvailableStatusCode, opt2.slm_OptionDesc AS LeadAvailableStatusDesc
                            FROM " + SLMDBName + @".dbo.kkslm_ms_config_activity ca
                            LEFT JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT pd ON pd.sub_product_id = ca.slm_Product_Id
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_option opt ON ca.slm_LeadStatus = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status'  
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_option opt2 ON ca.slm_AvailableStatus = opt2.slm_OptionCode AND opt2.slm_OptionType = 'lead status'  
                            WHERE ca.is_Deleted = 0 {0} 
                            ORDER BY pd.sub_product_name, opt.slm_OptionDesc, opt2.slm_OptionDesc ";

            string whr = "";
            whr += (productId == "" ? "" : (whr == "" ? "" : " AND ") + " ca.slm_Product_Id = '" + productId + "' ");
            whr += (leadStatus == "" ? "" : (whr == "" ? "" : " AND ") + " ca.slm_LeadStatus = '" + leadStatus + "' ");
            whr += (rightAdd == "" ? "" : (whr == "" ? "" : " AND ") + " ca.slm_HaveRightAdd = '" + rightAdd + "' ");
            whr += (availableStatus == "" ? "" : (whr == "" ? "" : " AND ") + " ca.slm_AvailableStatus = '" + availableStatus + "' ");

            whr = (whr == "" ? "" : " AND " + whr);
            sql = string.Format(sql, whr);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ActivityConfigData>(sql).ToList();
            }
        }

        public void InsertData(string productId, string leadStatusCode, bool rightAdd, List<string> availableStatusList, string createdBy)
        {
            try
            {
                DateTime createdDate = DateTime.Now;
                List<string> dbAvailableStatusList = null;

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        var config = slmdb.kkslm_ms_config_activity.Where(p => p.slm_Product_Id == productId && p.slm_LeadStatus == leadStatusCode && p.is_Deleted == false).FirstOrDefault();
                        if (config != null)
                        {
                            //ถ้ามีการเปลี่ยนแปลงสิทธิ์ ให้ลบข้อมูลเดิมที่อยู่ในเบส
                            if (config.slm_HaveRightAdd != rightAdd)
                            {
                                string del = "";
                                if (!string.IsNullOrEmpty(productId))
                                    del = @"DELETE FROM " + SLMConstant.SLMDBName + ".dbo.kkslm_ms_config_activity WHERE slm_Product_Id = '" + productId + "' AND slm_LeadStatus = '" + leadStatusCode + "' ";
                                else
                                    throw new Exception("Cannot find productId");

                                if (!string.IsNullOrEmpty(del))
                                    slmdb.ExecuteStoreCommand(del);
                            }
                        }

                        if (rightAdd)
                        {
                            //หา AvailableStatus ที่มีอยู่ในเบส
                            if (!string.IsNullOrEmpty(productId))
                                dbAvailableStatusList = slmdb.kkslm_ms_config_activity.Where(p => p.slm_Product_Id == productId && p.slm_LeadStatus == leadStatusCode && p.is_Deleted == false).Select(p => p.slm_AvailableStatus).ToList();
                            else
                                throw new Exception("Cannot find productId");

                            //เอา branch ที่มีอยู่ในเบสแล้ว ออกจาก list ที่ต้องการ insert
                            if (dbAvailableStatusList.Count > 0)
                                availableStatusList = availableStatusList.Except<string>(dbAvailableStatusList).ToList();

                            if (availableStatusList.Count > 0)
                            {
                                foreach (string status in availableStatusList)
                                {
                                    kkslm_ms_config_activity obj = new kkslm_ms_config_activity()
                                    {
                                        slm_Product_Id = string.IsNullOrEmpty(productId) ? null : productId,
                                        slm_LeadStatus = leadStatusCode,
                                        slm_HaveRightAdd = rightAdd,
                                        slm_AvailableStatus = status,
                                        slm_CreatedBy = createdBy,
                                        slm_CreatedDate = createdDate,
                                        slm_UpdatedBy = createdBy,
                                        slm_UpdatedDate = createdDate,
                                        is_Deleted = false
                                    };
                                    slmdb.kkslm_ms_config_activity.AddObject(obj);
                                }

                                slmdb.SaveChanges();
                            }
                        }
                        else
                        {
                            //ให้หาข้อเดิมในเบสก่อน ถ้ามีอยู่ไม่ต้องบันทึกซ้ำ
                            //เกิดจากการกด Insert แบบไม่มีสิทธิ์สองครั้ง
                            var config2 = slmdb.kkslm_ms_config_activity.Where(p => p.slm_Product_Id == productId && p.slm_LeadStatus == leadStatusCode && p.slm_HaveRightAdd == rightAdd && p.is_Deleted == false).FirstOrDefault();
                            if (config2 == null)
                            {
                                kkslm_ms_config_activity obj = new kkslm_ms_config_activity()
                                {
                                    slm_Product_Id = string.IsNullOrEmpty(productId) ? null : productId,
                                    slm_LeadStatus = leadStatusCode,
                                    slm_HaveRightAdd = rightAdd,
                                    slm_AvailableStatus = null,
                                    slm_CreatedBy = createdBy,
                                    slm_CreatedDate = createdDate,
                                    slm_UpdatedBy = createdBy,
                                    slm_UpdatedDate = createdDate,
                                    is_Deleted = false
                                };
                                slmdb.kkslm_ms_config_activity.AddObject(obj);

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

        public void UpdateData(string productId, string leadStatusCode, bool rightAdd, List<string> availableStatusList, string createdBy)
        {
            try
            {
                DateTime createdDate = DateTime.Now;

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        string del = "";
                        if (!string.IsNullOrEmpty(productId))
                            del = @"DELETE FROM " + SLMConstant.SLMDBName + ".dbo.kkslm_ms_config_activity WHERE slm_Product_Id = '" + productId + "' AND slm_LeadStatus = '" + leadStatusCode + "' ";
                        else
                            throw new Exception("Cannot find productId");

                        if (!string.IsNullOrEmpty(del))
                            slmdb.ExecuteStoreCommand(del);

                        if (rightAdd)
                        {
                            foreach (string status in availableStatusList)
                            {
                                kkslm_ms_config_activity obj = new kkslm_ms_config_activity()
                                {
                                    slm_Product_Id = string.IsNullOrEmpty(productId) ? null : productId,
                                    slm_LeadStatus = leadStatusCode,
                                    slm_HaveRightAdd = rightAdd,
                                    slm_AvailableStatus = status,
                                    slm_CreatedBy = createdBy,
                                    slm_CreatedDate = createdDate,
                                    slm_UpdatedBy = createdBy,
                                    slm_UpdatedDate = createdDate,
                                    is_Deleted = false
                                };
                                slmdb.kkslm_ms_config_activity.AddObject(obj);
                            }
                        }
                        else
                        {
                            kkslm_ms_config_activity obj = new kkslm_ms_config_activity()
                            {
                                slm_Product_Id = string.IsNullOrEmpty(productId) ? null : productId,
                                slm_LeadStatus = leadStatusCode,
                                slm_HaveRightAdd = rightAdd,
                                slm_AvailableStatus = null,
                                slm_CreatedBy = createdBy,
                                slm_CreatedDate = createdDate,
                                slm_UpdatedBy = createdBy,
                                slm_UpdatedDate = createdDate,
                                is_Deleted = false
                            };
                            slmdb.kkslm_ms_config_activity.AddObject(obj);
                        }

                        slmdb.SaveChanges();
                    }
                    
                    ts.Complete();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ActivityConfigData> GetActivityConfig(string productId, string leadStatusCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_config_activity.Where(p => p.slm_Product_Id == productId && p.slm_LeadStatus == leadStatusCode && p.is_Deleted == false)
                        .Select(p => new ActivityConfigData { ProductId = p.slm_Product_Id, LeadStatusCode = p.slm_LeadStatus, HaveRightAdd = p.slm_HaveRightAdd, LeadAvailableStatusCode = p.slm_AvailableStatus }).ToList();
            }
        }
    }
}
