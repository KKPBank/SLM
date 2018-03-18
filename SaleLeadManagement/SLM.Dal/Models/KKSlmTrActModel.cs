using SLM.Resource.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SLM.Dal.Models
{
    public class KKSlmTrActModel
    {
        private string SLMDBName = ConfigurationManager.AppSettings["SLMDBName"] != null ? ConfigurationManager.AppSettings["SLMDBName"] : "SLMDB";

        public ActDataCollection GetCompanyInsurance(int slm_PromotionId)
        {
            ActDataCollection result = new ActDataCollection();

            string sql = @"SELECT ic.slm_InsNameTh 
                            FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance pis 
                            inner join kkslm_ms_ins_com ic 
                            on pis.slm_Ins_Com_Id = ic.slm_Ins_Com_Id 
                            where slm_PromotionId ="+slm_PromotionId+"";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                result.Prev = slmdb.ExecuteStoreQuery<ActData>(sql).FirstOrDefault();

                return result;
            }
        }

        public List<ActData> ListBranchName()
        {
            string sql = @"select slm_branchname as BranchName,slm_BranchCode as BranchCode from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_branch where is_deleted=0 Order by slm_BranchName asc ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ActData>(sql).ToList();
            }
        }
        //Add 20170505
        public string GetBranchName(string branchCode)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_branch.Select(i => new { i.slm_BranchCode, i.slm_BranchName } ).SingleOrDefault(i => i.slm_BranchCode == branchCode).slm_BranchName;
            }
        }
        //Add 20170508
        public bool GetIsActiveBranch(string branchCode)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_branch.Select(i => new { i.slm_BranchCode, i.slm_BranchName, i.is_Deleted}).SingleOrDefault(i => i.slm_BranchCode == branchCode).is_Deleted;
            }
        }
        public List<ActData> ListInsNameTh()
        {
            string sql = @"select slm_Ins_Com_Id as InsId ,slm_InsNameTh as InsNameTh from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com
                           where is_Deleted = 0 order by slm_InsNameTh";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ActData>(sql).ToList();
            }
        }

        public void savePreleadActData(ActData actData)
        {

            kkslm_tr_prelead_compare_act preComAct = new kkslm_tr_prelead_compare_act();

            //dataSaveAct.slm_Seq = lblCoverageType_pre.Text;
            preComAct.slm_Prelead_Id = 11;
            preComAct.slm_PromotionId = 2;
            //preComAct.slm_Year = actData.slm_Year;
            preComAct.slm_Ins_Com_Id = actData.slm_Ins_Com_Id;
            preComAct.slm_ActIssuePlace = actData.slm_ActIssuePlace;
            preComAct.slm_SendDocType = actData.slm_SendDocType;
            preComAct.slm_ActSignNo = actData.slm_ActSignNo;
            preComAct.slm_ActStartCoverDate = actData.slm_ActStartCoverDate;
            preComAct.slm_ActEndCoverDate = actData.slm_ActEndCoverDate;
            preComAct.slm_CarTaxExpiredDate = actData.slm_CarTaxExpiredDate;
            preComAct.slm_ActNetGrossPremiumFull = actData.slm_ActNetGrossPremiumFull;
            preComAct.slm_ActGrossPremium = actData.slm_ActGrossPremium;
            preComAct.slm_ActGrossStamp = actData.slm_ActGrossStamp;
            preComAct.slm_ActGrossVat = actData.slm_ActGrossVat;
            preComAct.slm_ActNetGrossPremium = actData.slm_ActNetGrossPremium;
            preComAct.slm_CreatedDate = DateTime.Now;
            preComAct.slm_CreatedBy = actData.slm_CreatedBy;
            preComAct.slm_UpdatedDate = DateTime.Now;
            preComAct.slm_UpdatedBy = actData.slm_UpdatedBy;
            preComAct.slm_DiscountPercent = actData.slm_DiscountPercent;
            preComAct.slm_DiscountBath = actData.slm_DiscountBath;
            preComAct.slm_ActGrossPremiumPay = actData.slm_ActGrossPremiumPay;
            preComAct.slm_Vat1PercentBath = actData.slm_Vat1PercentBath;
            preComAct.slm_ActPurchaseFlag = null;
            preComAct.slm_ActNo = null;

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.kkslm_tr_prelead_compare_act.AddObject(preComAct);
                slmdb.SaveChanges();
            }
        }

        public void deletePreleadActData(int PreleadId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var sortConList = slmdb.kkslm_tr_prelead_compare_act.Where(p => p.slm_Prelead_Id == PreleadId).ToList();
                    foreach (kkslm_tr_prelead_compare_act obj in sortConList)
                    {
                        slmdb.kkslm_tr_prelead_compare_act.DeleteObject(obj);
                    }

                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }

        }

        public void updatePreleadActData(ActData actData)
        {

            kkslm_tr_prelead_compare_act preComAct = new kkslm_tr_prelead_compare_act();

            //dataSaveAct.slm_Seq = lblCoverageType_pre.Text;
            preComAct.slm_Prelead_Id = 11;
            preComAct.slm_PromotionId = 2;
            //preComAct.slm_Year = actData.slm_Year;
            preComAct.slm_Ins_Com_Id = actData.slm_Ins_Com_Id;
            preComAct.slm_ActIssuePlace = actData.slm_ActIssuePlace;
            preComAct.slm_SendDocType = actData.slm_SendDocType;
            preComAct.slm_ActSignNo = actData.slm_ActSignNo;
            preComAct.slm_ActStartCoverDate = actData.slm_ActStartCoverDate;
            preComAct.slm_ActEndCoverDate = actData.slm_ActEndCoverDate;
            preComAct.slm_CarTaxExpiredDate = actData.slm_CarTaxExpiredDate;
            preComAct.slm_ActNetGrossPremiumFull = actData.slm_ActNetGrossPremiumFull;
            preComAct.slm_ActGrossPremium = actData.slm_ActGrossPremium;
            preComAct.slm_ActGrossStamp = actData.slm_ActGrossStamp;
            preComAct.slm_ActGrossVat = actData.slm_ActGrossVat;
            preComAct.slm_ActNetGrossPremium = actData.slm_ActNetGrossPremium;
            preComAct.slm_CreatedDate = DateTime.Now;
            preComAct.slm_CreatedBy = actData.slm_CreatedBy;
            preComAct.slm_UpdatedDate = DateTime.Now;
            preComAct.slm_UpdatedBy = actData.slm_UpdatedBy;
            preComAct.slm_DiscountPercent = actData.slm_DiscountPercent;
            preComAct.slm_DiscountBath = actData.slm_DiscountBath;
            preComAct.slm_ActGrossPremiumPay = actData.slm_ActGrossPremiumPay;
            preComAct.slm_Vat1PercentBath = actData.slm_Vat1PercentBath;
            preComAct.slm_ActPurchaseFlag = null;
            preComAct.slm_ActNo = null;

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.SaveChanges();
            }
        }

        public void saveLeadActDataToRenewinsurance_compare_act(ActData actData)
        {

            //kkslm_tr_renewinsurance_compare_act renewCompareAct = new kkslm_tr_renewinsurance_compare_act();

            //dataSaveAct.slm_Seq = lblCoverageType_pre.Text;
            //renewCompareAct.slm_RenewInsureId = 11;
            //renewCompareAct.slm_PromotionId = 2;
            //renewCompareAct.slm_Seq = actData.slm_Seq;
            //renewCompareAct.slm_Year = actData.slm_Year;
            //renewCompareAct.slm_Ins_Com_Id = actData.slm_Ins_Com_Id;
            //renewCompareAct.slm_ActIssuePlace = actData.slm_ActIssuePlace;
            //renewCompareAct.slm_SendDocType = actData.slm_SendDocType;
            //renewCompareAct.slm_ActStartCoverDate = actData.slm_ActStartCoverDate;
            //renewCompareAct.slm_ActEndCoverDate = actData.slm_ActEndCoverDate;
            //renewCompareAct.slm_ActIssueBranch = actData.slm_ActIssueBranch;
            //renewCompareAct.slm_CarTaxExpiredDate = actData.slm_CarTaxExpiredDate;
            //renewCompareAct.slm_ActGrossStamp = actData.slm_ActGrossStamp;
            //renewCompareAct.slm_ActGrossVat = actData.slm_ActGrossVat;
            //renewCompareAct.slm_ActGrossPremium = actData.slm_ActGrossPremium;
            //renewCompareAct.slm_ActNetGrossPremium = actData.slm_ActNetGrossPremium;
            //renewCompareAct.slm_ActGrossPremiumPay = actData.slm_ActGrossPremiumPay;
            //renewCompareAct.slm_CreatedDate = DateTime.Now;
            //renewCompareAct.slm_CreatedBy = actData.slm_CreatedBy;
            //renewCompareAct.slm_UpdatedDate = DateTime.Now; ;
            //renewCompareAct.slm_UpdatedBy = actData.slm_UpdatedBy;
            //renewCompareAct.slm_ActPurchaseFlag = actData.slm_ActPurchaseFlag;
            //renewCompareAct.slm_DiscountPercent = actData.slm_DiscountPercent;
            //renewCompareAct.slm_DiscountBath = actData.slm_DiscountPercent;
            //renewCompareAct.slm_ActSignNo = actData.slm_ActSignNo;
            //renewCompareAct.slm_ActNetGrossPremiumFull = actData.slm_ActNetGrossPremiumFull;
            //renewCompareAct.slm_Vat1Percent = actData.slm_Vat1Percent;
            //renewCompareAct.slm_Vat1PercentBath = actData.slm_Vat1PercentBath;


            //slmdb.kkslm_tr_renewinsurance_compare_act.AddObject(renewCompareAct);
            //slmdb.SaveChanges();
        }

        public void saveLeadActDataToRenewinsurance_compare_act_snap(ActData actData)
        {

            //kkslm_tr_renewinsurance_compare_act_snap renewCompareActSnap = new kkslm_tr_renewinsurance_compare_act_snap();

            //renewCompareActSnap.slm_RenewInsureId = 11;
            //renewCompareActSnap.slm_PromotionId = 2;
            ////renewCompareActSnap.slm_Seq = actData.slm_Seq;
            ////renewCompareActSnap.slm_Year = actData.slm_Year;
            //renewCompareActSnap.slm_Ins_Com_Id = actData.slm_Ins_Com_Id;
            //renewCompareActSnap.slm_ActIssuePlace = actData.slm_ActIssuePlace;
            //renewCompareActSnap.slm_SendDocType = actData.slm_SendDocType;
            //renewCompareActSnap.slm_ActStartCoverDate = actData.slm_ActStartCoverDate;
            //renewCompareActSnap.slm_ActEndCoverDate = actData.slm_ActEndCoverDate;
            //renewCompareActSnap.slm_ActIssueBranch = actData.slm_ActIssueBranch;
            //renewCompareActSnap.slm_CarTaxExpiredDate = actData.slm_CarTaxExpiredDate;
            //renewCompareActSnap.slm_ActGrossStamp = actData.slm_ActGrossStamp;
            //renewCompareActSnap.slm_ActGrossVat = actData.slm_ActGrossVat;
            //renewCompareActSnap.slm_ActGrossPremium = actData.slm_ActGrossPremium;
            //renewCompareActSnap.slm_ActNetGrossPremium = actData.slm_ActNetGrossPremium;
            //renewCompareActSnap.slm_ActGrossPremiumPay = actData.slm_ActGrossPremiumPay;
            //renewCompareActSnap.slm_CreatedDate = DateTime.Now;
            //renewCompareActSnap.slm_CreatedBy = actData.slm_UpdatedBy;
            //renewCompareActSnap.slm_UpdatedDate = DateTime.Now;
            //renewCompareActSnap.slm_UpdatedBy = actData.slm_UpdatedBy;
            //renewCompareActSnap.slm_ActPurchaseFlag = actData.slm_ActPurchaseFlag;
            //renewCompareActSnap.slm_DiscountPercent = actData.slm_DiscountPercent;
            //renewCompareActSnap.slm_DiscountBath = actData.slm_DiscountBath;
            //renewCompareActSnap.slm_ActSignNo = actData.slm_ActSignNo;
            //renewCompareActSnap.slm_ActNetGrossPremiumFull = actData.slm_ActNetGrossPremiumFull;
            //renewCompareActSnap.slm_Vat1Percent = actData.slm_Vat1Percent;
            //renewCompareActSnap.slm_Vat1PercentBath = actData.slm_Vat1PercentBath;
            //renewCompareActSnap.slm_Version = actData.slm_Version;

            //slmdb.kkslm_tr_renewinsurance_compare_act_snap.AddObject(renewCompareActSnap);
            //slmdb.SaveChanges();
        }
    }
}
