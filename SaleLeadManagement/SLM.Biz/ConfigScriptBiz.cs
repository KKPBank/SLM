using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Net.Sockets;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Dal;

namespace SLM.Biz
{
    public class ConfigScriptBiz
    {
        public List<ConfigProductScriptData> SearchConfigProductScript(string productId, string campaignId, string dataType, string subject, string detail, bool statusActive, bool statusInActive)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = @"SELECT SC.slm_cp_scriptqa_Id AS ConfigScriptId, SC.slm_Product_Id AS ProductId, MP.sub_product_name AS ProductName, SC.slm_CampaignId AS CampaignId, CAM.slm_CampaignName AS CampaignName
                                , SC.slm_DataType AS DataType, SC.slm_Subject AS [Subject], SC.slm_SubSubject AS SubSubject, SC.slm_Detail AS Detail, SC.is_Deleted AS IsDeleted, SC.slm_Seq AS Seq
                                , CASE WHEN SC.is_Deleted = 1 THEN 'ไม่ใช้งาน'
		                                ELSE 'ใช้งาน' END AS Status
                                FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_scriptqa SC
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT MP ON MP.sub_product_id = SC.slm_Product_Id
                                LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign CAM ON CAM.slm_CampaignId = SC.slm_CampaignId
                                {0}
                                ORDER BY CAM.slm_CampaignName, MP.sub_product_name, SC.slm_DataType, SC.slm_Seq ";

                    string whr = "";
                    whr += (productId == "" ? "" : (whr == "" ? "" : " AND ") + " SC.slm_Product_Id = '" + productId + "' ");
                    whr += (campaignId == "" ? "" : (whr == "" ? "" : " AND ") + " SC.slm_CampaignId = '" + campaignId + "' ");
                    whr += (dataType == "" ? "" : (whr == "" ? "" : " AND ") + " SC.slm_DataType = '" + dataType + "' ");
                    whr += (subject == "" ? "" : (whr == "" ? "" : " AND ") + " SC.slm_Subject LIKE @subject ");
                    whr += (detail == "" ? "" : (whr == "" ? "" : " AND ") + " SC.slm_Detail LIKE @detail ");

                    if (statusActive == true && statusInActive == false)
                        whr += (whr == "" ? "" : " AND ") + " SC.is_Deleted = '0' ";
                    else if (statusActive == false && statusInActive == true)
                        whr += (whr == "" ? "" : " AND ") + " SC.is_Deleted = '1' ";

                    whr = (whr != "" ? " WHERE " + whr : "");
                    sql = string.Format(sql, whr);

                    object[] param = new object[] 
                    { 
                        new SqlParameter("@subject", "%" + subject + "%")
                        , new SqlParameter("@detail", "%" + detail + "%")
                    };

                    return slmdb.ExecuteStoreQuery<ConfigProductScriptData>(sql, param).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public ConfigProductScriptData GetConfigScriptData(decimal configScriptId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_cp_scriptqa_Id == configScriptId).Select(p => new ConfigProductScriptData
                    {
                        ConfigScriptId = p.slm_cp_scriptqa_Id,
                        ProductId = p.slm_Product_Id,
                        CampaignId = p.slm_CampaignId,
                        DataType = p.slm_DataType,
                        Subject = p.slm_Subject,
                        Detail = p.slm_Detail,
                        IsDeleted = p.is_Deleted,
                        Seq = p.slm_Seq
                    }).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertData(string productId, string campaignId, string dataType, string subject, string detail, bool isActive, string seq, string createByusername)
        {
            try
            {
                int count = 0;
                int? sequence = string.IsNullOrEmpty(seq) ? -1 : int.Parse(seq);

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //เช็กข้อมูลซ้ำ
                    if (!string.IsNullOrEmpty(productId))
                    {
                        count = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_Product_Id == productId && p.slm_CampaignId == null && p.slm_DataType == dataType && p.slm_Subject == subject && p.is_Deleted == false).Count();
                        if (count > 0)
                            throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจากซ้ำกับในระบบ");

                        if (sequence != -1)
                        {
                            count = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_Product_Id == productId && p.slm_CampaignId == null && p.slm_DataType == dataType && p.slm_Seq == sequence && p.is_Deleted == false).Count();
                            if (count > 0)
                                throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจาก sequence ซ้ำกับในระบบ");
                        }
                    }
                    else if (!string.IsNullOrEmpty(campaignId))
                    {
                        count = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_Product_Id == null && p.slm_CampaignId == campaignId && p.slm_DataType == dataType && p.slm_Subject == subject && p.is_Deleted == false).Count();
                        if (count > 0)
                            throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจากซ้ำกับในระบบ");

                        if (sequence != -1)
                        {
                            count = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_Product_Id == null && p.slm_CampaignId == campaignId && p.slm_DataType == dataType && p.slm_Seq == sequence && p.is_Deleted == false).Count();
                            if (count > 0)
                                throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจาก sequence ซ้ำกับในระบบ");
                        }
                    }

                    kkslm_ms_config_product_scriptqa script = new kkslm_ms_config_product_scriptqa();
                    if (!string.IsNullOrEmpty(productId)) script.slm_Product_Id = productId;
                    if (!string.IsNullOrEmpty(campaignId)) script.slm_CampaignId = campaignId;
                    script.slm_DataType = dataType;
                    script.slm_Subject = subject;
                    script.slm_Detail = detail;
                    script.slm_Seq = sequence == -1 ? null : sequence;

                    DateTime createDate = DateTime.Now;
                    script.slm_CreatedBy = createByusername;
                    script.slm_CreatedDate = createDate;
                    script.slm_UpdatedBy = createByusername;
                    script.slm_UpdatedDate = createDate;
                    script.is_Deleted = !isActive;

                    slmdb.kkslm_ms_config_product_scriptqa.AddObject(script);
                    slmdb.SaveChanges();
                }
                
            }
            catch
            {
                throw;
            }
        }

        public void UpdateData(decimal configScriptId, string productId, string campaignId, string dataType, string subject, string detail, bool isActive, string seq, string updateByusername)
        {
            try
            {
                int count = 0;
                int? sequence = string.IsNullOrEmpty(seq) ? -1 : int.Parse(seq);

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    if (!string.IsNullOrEmpty(productId))
                    {
                        count = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_cp_scriptqa_Id != configScriptId && p.slm_Product_Id == productId && p.slm_CampaignId == null && p.slm_DataType == dataType && p.slm_Subject == subject).Count();
                        if (count > 0)
                            throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจากซ้ำกับในระบบ");

                        if (sequence != -1)
                        {
                            count = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_cp_scriptqa_Id != configScriptId && p.slm_Product_Id == productId && p.slm_CampaignId == null && p.slm_DataType == dataType && p.slm_Seq == sequence).Count();
                            if (count > 0)
                                throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจาก sequence ซ้ำกับในระบบ");
                        }
                    }
                    else if (!string.IsNullOrEmpty(campaignId))
                    {
                        count = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_cp_scriptqa_Id != configScriptId && p.slm_Product_Id == null && p.slm_CampaignId == campaignId && p.slm_DataType == dataType && p.slm_Subject == subject).Count();
                        if (count > 0)
                            throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจากซ้ำกับในระบบ");

                        if (sequence != -1)
                        {
                            count = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_cp_scriptqa_Id != configScriptId && p.slm_Product_Id == null && p.slm_CampaignId == campaignId && p.slm_DataType == dataType && p.slm_Seq == sequence).Count();
                            if (count > 0)
                                throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจาก sequence ซ้ำกับในระบบ");
                        }
                    }

                    var script = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_cp_scriptqa_Id == configScriptId).FirstOrDefault();
                    if (script != null)
                    {
                        script.slm_DataType = dataType;
                        script.slm_Subject = subject;
                        script.slm_Detail = detail;
                        script.is_Deleted = !isActive;
                        script.slm_Seq = sequence == -1 ? null : sequence;

                        script.slm_UpdatedBy = updateByusername;
                        script.slm_UpdatedDate = DateTime.Now;

                        slmdb.SaveChanges();
                    }
                    else
                        throw new Exception("ไม่พบข้อมูล ConfigScriptId " + configScriptId + " ในระบบ");
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteData(decimal configScriptId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var script = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_cp_scriptqa_Id == configScriptId).FirstOrDefault();
                    if (script != null)
                    {
                        slmdb.kkslm_ms_config_product_scriptqa.DeleteObject(script);
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ConfigProductScriptData> GetConfigScriptList(string campaignId, string productId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<ConfigProductScriptData> list = new List<ConfigProductScriptData>();
                    list = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_CampaignId == campaignId && p.is_Deleted == false).OrderBy(p => p.slm_Seq).Select(p => new ConfigProductScriptData()
                    {
                        DataType = p.slm_DataType,
                        Subject = p.slm_Subject,
                        SubSubject = p.slm_SubSubject,
                        Detail = p.slm_Detail
                    }).ToList();

                    if (list.Count == 0)
                    {
                        list = slmdb.kkslm_ms_config_product_scriptqa.Where(p => p.slm_Product_Id == productId && p.is_Deleted == false).OrderBy(p => p.slm_Seq).Select(p => new ConfigProductScriptData()
                        {
                            DataType = p.slm_DataType,
                            Subject = p.slm_Subject,
                            SubSubject = p.slm_SubSubject,
                            Detail = p.slm_Detail
                        }).ToList();
                    }

                    return list;
                } 
            }
            catch
            {
                throw;
            }
        }
    }
}
