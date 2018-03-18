using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmTrRenewinsuranceModel
    {
        public void InsertData(string ticketId, string createByUsername, DateTime createDate)
        {
            try
            {
                kkslm_tr_renewinsurance reinsur = new kkslm_tr_renewinsurance();
                reinsur.slm_TicketId = ticketId;
                reinsur.slm_CreatedBy = createByUsername;
                reinsur.slm_CreatedDate = createDate;
                reinsur.slm_UpdatedBy = createByUsername;
                reinsur.slm_UpdatedDate = createDate;

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    slmdb.kkslm_tr_renewinsurance.AddObject(reinsur);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<LeadData> ListHistorySell(int ticketId)
        {
            string sql = @"SELECT DISTINCT z.slm_RenewInsureId,z.slm_TicketId,z.slm_CreatedDate,Z.slm_CreatedBy,Z.slm_Version
                            FROM(SELECT TR.slm_TicketId,cs.slm_RenewInsureId, cs.slm_CreatedDate,cs.slm_CreatedBy,cs.slm_Version
                            FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance tr inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare_snap cs
	                            on tr.slm_RenewInsureId = cs.slm_RenewInsureId 
                            WHERE TR.slm_TicketId = "+ticketId+""+
                            @"UNION ALL
                            SELECT TR.slm_TicketId,cas.slm_RenewInsureId,cas.slm_CreatedDate,cas.slm_CreatedBy,cas.slm_Version
                            FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance tr inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare_act_snap cas
	                            on tr.slm_RenewInsureId = cas.slm_RenewInsureId 
                            WHERE TR.slm_TicketId = "+ticketId+") AS Z ORDER BY Z.slm_Version DESC, Z.slm_CreatedDate DESC";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<LeadData>(sql).ToList();
            }
        }

//        public List<RenewinsuranceData> ListHistorySellDetailIns(int RenewInsureId)
//        {
//            string sql = @"SELECT RENCN.slm_NotifyPremiumId,RENCN.slm_PromotionId,RENCN.slm_Seq
//	                          ,RENCN.slm_Ins_Com_Id,RENCN.slm_CoverageTypeId
//	                          ,RENCN.slm_InjuryDeath,RENCN.slm_TPPD,RENCN.slm_RepairTypeId
//                              ,RENCN.slm_OD,slm_FT,RENCN.slm_DeDuctible
//                              ,RENCN.slm_PersonalAccident,RENCN.slm_PersonalAccidentMan
//                              ,RENCN.slm_MedicalFee,RENCN.slm_MedicalFeeMan
//                              ,RENCN.slm_InsuranceDriver,RENCN.slm_PolicyGrossStamp
//                              ,RENCN.slm_PolicyGrossVat,RENCN.slm_PolicyGrossPremium
//                              ,RENCN.slm_NetGrossPremium,RENCN.slm_PolicyGrossPremiumPay
//                              ,RENCN.slm_CostSave,RENCN.slm_CreatedDate,RENCN.slm_CreatedBy
//                              ,RENCN.slm_UpdatedDate,RENCN.slm_UpdatedBy,RENCN.slm_Selected
//                              ,RENCN.slm_OldPolicyNo,RENCN.slm_DriverFlag
//                              ,RENCN.slm_Driver_TitleId1,RENCN.slm_Driver_First_Name1
//                              ,RENCN.slm_Driver_Last_Name1,RENCN.slm_Driver_Birthdate1
//                              ,RENCN.slm_Driver_TitleId2,RENCN.slm_Driver_First_Name2
//                              ,RENCN.slm_Driver_Last_Name2,RENCN.slm_Driver_Birthdate2
//                              ,RENCN.slm_OldReceiveNo,RENCN.slm_PolicyStartCoverDate
//                              ,RENCN.slm_PolicyEndCoverDate,RENCN.slm_Vat1Percent
//                              ,RENCN.slm_DiscountPercent,RENCN.slm_DiscountBath
//                              ,RENCN.slm_Vat1PercentBath,RENCN.slm_Version,RENCN.slm_Year
//                              ,REN.slm_PolicyNo
//                        FROM KKSLM_TR_LEAD LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
//                        INNER JOIN kkslm_tr_renewinsurance_compare_snap RENCN ON REN.slm_RenewInsureId = RENCN.slm_RenewInsureId 
//                        WHERE REN.slm_RenewInsureId = " + RenewInsureId + " and RENCN.slm_Version = " + RenewInsureId + "";

//            return slmdb.ExecuteStoreQuery<RenewinsuranceData>(sql).ToList();
//        }

//        public List<RenewinsuranceData> ListHistorySellDetailAct(int RenewInsureId)
//        {
//            string sql = @"SELECT RENCAN.slm_RenewInsureId,RENCAN.slm_PromotionId,RENCAN.slm_Seq
//                              ,RENCAN.slm_Year,RENCAN.slm_Ins_Com_Id,RENCAN.slm_ActIssuePlace
//                              ,RENCAN.slm_SendDocType,RENCAN.slm_ActNo
//                              ,RENCAN.slm_ActStartCoverDate,RENCAN.slm_ActEndCoverDate
//                              ,RENCAN.slm_ActIssueBranch,RENCAN.slm_CarTaxExpiredDate
//                              ,RENCAN.slm_ActGrossStamp,RENCAN.slm_ActGrossVat
//                              ,RENCAN.slm_ActGrossPremium,RENCAN.slm_ActNetGrossPremium
//                              ,RENCAN.slm_ActGrossPremiumPay,RENCAN.slm_CreatedDate
//                              ,RENCAN.slm_CreatedBy,RENCAN.slm_UpdatedDate
//                              ,RENCAN.slm_UpdatedBy,RENCAN.slm_ActPurchaseFlag
//                              ,RENCAN.slm_DiscountPercent,RENCAN.slm_DiscountBath
//                              ,RENCAN.slm_ActSignNo,RENCAN.slm_ActNetGrossPremiumFull
//                              ,RENCAN.slm_Vat1Percent,RENCAN.slm_Vat1PercentBath
//                              ,RENCAN.slm_Version
//                        FROM KKSLM_TR_LEAD LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
//                        INNER JOIN kkslm_tr_renewinsurance_compare_act_snap RENCAN ON REN.slm_RenewInsureId = RENCAN.slm_RenewInsureId 
//                        WHERE REN.slm_RenewInsureId = " + RenewInsureId + " and RENCAN.slm_Version = " + RenewInsureId + "";

//            return slmdb.ExecuteStoreQuery<RenewinsuranceData>(sql).ToList();
//        }
    }
}
