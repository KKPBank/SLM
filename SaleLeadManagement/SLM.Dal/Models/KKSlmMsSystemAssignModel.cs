using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsSystemAssignModel
    {
        public List<SystemAssignData> SearchSystemAssignConfig(string productId, string campaignId, string staffTypeId, string branchCode)
        {
            string SLMDBName = SLMConstant.SLMDBName;
            string sql = @"SELECT assign.slm_Product_Id AS ProductId, pd.sub_product_name AS ProductName, assign.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName
                            , assign.slm_StaffTypeId AS StaffTypeId, st.slm_StaffTypeDesc AS StaffTypeDesc, assign.slm_BranchCode AS BranchCode, branch.slm_BranchName AS BranchName
                            FROM " + SLMDBName + @".dbo.kkslm_ms_system_assign assign
                            LEFT JOIN " + SLMDBName + @".dbo.CMT_MAPPING_PRODUCT pd ON pd.sub_product_id = assign.slm_Product_Id
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign cam ON cam.slm_CampaignId = assign.slm_CampaignId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_type st ON st.slm_StaffTypeId = assign.slm_StaffTypeId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_branch branch ON branch.slm_BranchCode = assign.slm_BranchCode
                            WHERE assign.is_Deleted = 0 {0} 
                            ORDER BY cam.slm_CampaignName, pd.sub_product_name, branch.slm_BranchName, st.slm_StaffTypeDesc ";

            string whr = "";
            whr += (productId == "" ? "" : (whr == "" ? "" : " AND ") + " assign.slm_Product_Id = '" + productId + "' ");
            whr += (campaignId == "" ? "" : (whr == "" ? "" : " AND ") + " assign.slm_CampaignId = '" + campaignId + "' ");
            whr += (staffTypeId == "" ? "" : (whr == "" ? "" : " AND ") + " assign.slm_StaffTypeId = '" + staffTypeId + "' ");
            whr += (branchCode == "" ? "" : (whr == "" ? "" : " AND ") + " assign.slm_BranchCode = '" + branchCode + "' ");

            whr = (whr == "" ? "" : " AND " + whr);
            sql = string.Format(sql, whr);

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SystemAssignData>(sql).ToList();
            }
        }

        public void UpdateData(string productId, string campaignId, int staffTypeId, List<string> branchCodeList, string createdBy)
        {
            try
            {
                string del = "";

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        if (!string.IsNullOrEmpty(productId))
                            del = @"DELETE FROM " + SLMConstant.SLMDBName + ".dbo.kkslm_ms_system_assign WHERE slm_Product_Id = '" + productId + "' AND slm_StaffTypeId = '" + staffTypeId.ToString() + "' ";
                        else if (!string.IsNullOrEmpty(campaignId))
                            del = @"DELETE FROM " + SLMConstant.SLMDBName + ".dbo.kkslm_ms_system_assign WHERE slm_CampaignId = '" + campaignId + "' AND slm_StaffTypeId = '" + staffTypeId.ToString() + "' ";
                        else
                            throw new Exception("Cannot find productId or campaignId");

                        if (!string.IsNullOrEmpty(del))
                            slmdb.ExecuteStoreCommand(del);

                        DateTime createdDate = DateTime.Now;
                        foreach (string branchcode in branchCodeList)
                        {
                            kkslm_ms_system_assign obj = new kkslm_ms_system_assign()
                            {
                                slm_Product_Id = string.IsNullOrEmpty(productId) ? null : productId,
                                slm_CampaignId = string.IsNullOrEmpty(campaignId) ? null : campaignId,
                                slm_StaffTypeId = staffTypeId,
                                slm_BranchCode = branchcode,
                                slm_CreatedBy = createdBy,
                                slm_CreatedDate = createdDate,
                                slm_UpdatedBy = createdBy,
                                slm_UpdatedDate = createdDate,
                                is_Deleted = false
                            };
                            slmdb.kkslm_ms_system_assign.AddObject(obj);
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

        public void InsertData(string productId, string campaignId, int staffTypeId, List<string> branchCodeList, string createdBy)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    DateTime createdDate = DateTime.Now;
                    List<string> dbBranchCodeList = null;

                    //หา branch ที่มีอยู่ในเบส
                    if (!string.IsNullOrEmpty(productId))
                        dbBranchCodeList = slmdb.kkslm_ms_system_assign.Where(p => p.slm_Product_Id == productId && p.slm_StaffTypeId == staffTypeId && p.is_Deleted == false).Select(p => p.slm_BranchCode).ToList();
                    else if (!string.IsNullOrEmpty(campaignId))
                        dbBranchCodeList = slmdb.kkslm_ms_system_assign.Where(p => p.slm_CampaignId == campaignId && p.slm_StaffTypeId == staffTypeId && p.is_Deleted == false).Select(p => p.slm_BranchCode).ToList();
                    else
                        throw new Exception("Cannot find productId or campaignId");

                    //เอา branch ที่มีอยู่ในเบสแล้ว ออกจาก list ที่ต้องการ insert
                    if (dbBranchCodeList.Count > 0)
                        branchCodeList = branchCodeList.Except<string>(dbBranchCodeList).ToList();

                    if (branchCodeList.Count > 0)
                    {
                        foreach (string branchcode in branchCodeList)
                        {
                            kkslm_ms_system_assign obj = new kkslm_ms_system_assign()
                            {
                                slm_Product_Id = string.IsNullOrEmpty(productId) ? null : productId,
                                slm_CampaignId = string.IsNullOrEmpty(campaignId) ? null : campaignId,
                                slm_StaffTypeId = staffTypeId,
                                slm_BranchCode = branchcode,
                                slm_CreatedBy = createdBy,
                                slm_CreatedDate = createdDate,
                                slm_UpdatedBy = createdBy,
                                slm_UpdatedDate = createdDate,
                                is_Deleted = false
                            };
                            slmdb.kkslm_ms_system_assign.AddObject(obj);
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
    }
}
