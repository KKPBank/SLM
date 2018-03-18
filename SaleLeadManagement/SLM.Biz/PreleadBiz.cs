using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Dal;
using SLM.Resource;

namespace SLM.Biz
{
    public class PreleadBiz
    {
        public PreleadViewData GetPreleadData(decimal preleadId, string licenseNo, string campaignId, string ContractNo)
        {
            return new KKSlmTrPreleadInfoModel().GetPreleadData(preleadId, licenseNo, campaignId, ContractNo);
        }

        public string[] GetProductCampaign(decimal preleadId)
        {
            return new KKSlmTrPreleadInfoModel().GetProductCampaign(preleadId);
        }

        //Added by Pom 22/04/2016
        public CustomerData GetCustomerData(string preleadId)
        {
            return new KKSlmTrPreleadInfoModel().GetCustomerData(preleadId);
        }

        //public List<PreleadViewData> GetPreleadOnHand(string username)
        //{
        //    return new KKSlmTrPreleadInfoModel().GetPreleadOnHand(username);
        //}

        public bool IsFleet(string preleadid, string ticketId)
        {
            return new KKSlmTrPreleadInfoModel().IsFleet(preleadid, ticketId);
        }

        public List<string> GetDataForCARLogService(decimal preleadId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<string> list = new List<string>();
                    var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == preleadId).FirstOrDefault();
                    if (prelead != null)
                    {
                        list.Add(prelead.slm_Contract_Number != null ? prelead.slm_Contract_Number : "");
                        list.Add(prelead.slm_Car_License_No != null ? prelead.slm_Car_License_No : "");
                    }

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        public void CheckRenewPurchased(string preleadId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = "";
                    int countPolicy = 0;
                    //int countAct = 0;

                    sql = "SELECT COUNT(*) FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare where slm_Selected = 1 AND slm_Prelead_Id = '" + preleadId + "' ";
                    countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                    if (countPolicy == 0)
                        throw new Exception("ไม่พบข้อมูลการซื้อประกัน");
                }

//                sql = @"SELECT COUNT(*) 
//                        FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare pc 
//                        INNER join " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare_act pca on pc.slm_prelead_id = pca.slm_prelead_id
//                        WHERE pc.slm_Selected = 1 and pca.slm_ActPurchaseFlag = 1  and pc.slm_Prelead_Id = '" + preleadId + "' ";
//                countAct = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

//                if (countPolicy == 0 && countAct == 0)
//                    throw new Exception("ไม่พบข้อมูลของการซื้อประกันหรือพรบ.");
            }
            catch
            {
                throw;
            }
        }

        public void CheckActPurchased(string preleadId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = "";
                    int countPolicy = 0;
                    int countAct = 0;

                    sql = "SELECT COUNT(*) FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare where slm_Selected = 1 AND slm_Prelead_Id = '" + preleadId + "' ";
                    countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                    if (countPolicy > 0)
                        throw new Exception("ไม่สามารถเลือกซื้อ พรบ.เดี่ยวได้ เนื่องจากมีข้อมูลการซื้อประกัน");

                    sql = @"SELECT COUNT(*) 
                        FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare_act pca 
                        WHERE pca.slm_ActPurchaseFlag = 1  and pca.slm_Prelead_Id = '" + preleadId + "'";

                    countAct = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                    if (countAct == 0)
                        throw new Exception("ไม่พบข้อมูลการซื้อ พรบ.เดี่ยว");
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
