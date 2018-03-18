using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Resource;
using SLM.Resource.Data;
using System.Transactions;

namespace SLM.Dal.Models
{
    public class HistoryLeadModel
    {
        //public List<RenewInsuranceData> GetHistoryMain(string ticketId)
        //{

        //    //slm_Brand_Name_Org
        //    //slm_Model_Code_Org
        //    //slm_Model_Name_Org
        //    //slm_Cc
        //    string sql = @"SELECT DISTINCT z.slm_RenewInsureId,z.slm_TicketId,z.slm_CreatedDate,Z.slm_CreatedBy,Z.slm_Version
        //                    FROM
        //                    (
        //                    SELECT TR.slm_TicketId,cs.slm_RenewInsureId, convert(date,cs.slm_CreatedDate)slm_CreatedDate,s.slm_StaffNameTH slm_CreatedBy,cs.slm_Version
        //                    FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance tr inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare_snap cs
        //                        inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff s on cs.slm_CreatedBy = s.slm_UserName
	       //                     on tr.slm_RenewInsureId = cs.slm_RenewInsureId 
        //                    WHERE TR.slm_TicketId = '" + ticketId +"'"+
        //                    @"UNION ALL
        //                    SELECT TR.slm_TicketId,cas.slm_RenewInsureId,convert(date,cas.slm_CreatedDate)slm_CreatedDate,s.slm_StaffNameTH slm_CreatedBy,cas.slm_Version
        //                    FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance tr inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare_act_snap cas
	       //                     on tr.slm_RenewInsureId = cas.slm_RenewInsureId 
        //                        inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff s on cas.slm_CreatedBy = s.slm_UserName
        //                    WHERE TR.slm_TicketId = '" + ticketId + "') as Z " +	
        //                    @"ORDER BY Z.slm_Version DESC, Z.slm_CreatedDate DESC";

        //    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
        //    {
        //        return slmdb.ExecuteStoreQuery<RenewInsuranceData>(sql).ToList();
        //    }
        //}

        public List<RenewInsuranceData> GetHistoryMain(string ticketId)
        {

            //slm_Brand_Name_Org
            //slm_Model_Code_Org
            //slm_Model_Name_Org
            //slm_Cc
            string sql = @"SELECT DISTINCT z.slm_RenewInsureId,z.slm_TicketId,z.slm_CreatedDate,Z.slm_CreatedBy,Z.slm_Version
                            FROM
                            (
                            SELECT TR.slm_TicketId,cs.slm_RenewInsureId, cs.slm_CreatedDate as slm_CreatedDate,s.slm_StaffNameTH slm_CreatedBy,cs.slm_Version
                            FROM kkslm_tr_renewinsurance tr inner join kkslm_tr_renewinsurance_compare_snap cs
                                inner join kkslm_ms_staff s on cs.slm_CreatedBy = s.slm_UserName
	                            on tr.slm_RenewInsureId = cs.slm_RenewInsureId 
                            WHERE TR.slm_TicketId = '" + ticketId + "'" +
                            @"UNION ALL
                            SELECT TR.slm_TicketId,cas.slm_RenewInsureId, cas.slm_CreatedDate as slm_CreatedDate,s.slm_StaffNameTH slm_CreatedBy,cas.slm_Version
                            FROM kkslm_tr_renewinsurance tr inner join kkslm_tr_renewinsurance_compare_act_snap cas
	                            on tr.slm_RenewInsureId = cas.slm_RenewInsureId 
                                inner join kkslm_ms_staff s on cas.slm_CreatedBy = s.slm_UserName
                            WHERE TR.slm_TicketId = '" + ticketId + "') as Z " +
                            @"ORDER BY Z.slm_Version DESC, Z.slm_CreatedDate DESC";

            List<RenewInsuranceData> newList = new List<RenewInsuranceData>();

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.ExecuteStoreQuery<RenewInsuranceData>(sql).ToList();
                var versions = list.Select(p => p.slm_Version).Distinct().OrderByDescending(p => p).ToList();
                foreach (int? version in versions)
                {
                    var obj = list.Where(p => p.slm_Version == version).FirstOrDefault();
                    if (obj != null)
                    {
                        newList.Add(obj);
                    }
                }
            }

            newList = newList.OrderByDescending(p => p.slm_Version).ToList();
            return newList;
        }
        public List<PreleadCompareData> GetHistoryDetailPolicy(string RenewInsureId, string version)
        {

            //slm_Brand_Name_Org
            //slm_Model_Code_Org
            //slm_Model_Name_Org
            //slm_Cc
            string sql = @"SELECT top 6
                           (select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com IC where ic.slm_Ins_Com_Id =RENCN.slm_Ins_Com_Id and is_Deleted='0')as slm_insnameth,
                           CVT.slm_ConverageTypeName as slm_CoverageTypeName,
                           (select slm_TitleName  from kkslm_ms_title TLT where TLT.slm_TitleId = RENCN.slm_Driver_TitleId1 and is_Deleted='0') as slm_TitleName1,
	                       (select slm_TitleName  from kkslm_ms_title TLT where TLT.slm_TitleId = RENCN.slm_Driver_TitleId2 and is_Deleted='0') as slm_TitleName2,
	                       (select slm_RepairTypeName from kkslm_ms_repairtype where slm_RepairTypeId = RENCN.slm_RepairTypeId and is_Deleted='0' )as slm_RepairTypeName,
	                       (select slm_CardTypeName from kkslm_ms_cardtype where slm_CardTypeId = cus.slm_CardType and is_Deleted='0') as slm_CardTypeName,
                           RENCN.slm_NotifyPremiumId,RENCN.slm_PromotionId,RENCN.slm_Seq
	                      ,RENCN.slm_Ins_Com_Id,RENCN.slm_CoverageTypeId
	                      ,RENCN.slm_InjuryDeath,RENCN.slm_TPPD,RENCN.slm_RepairTypeId
                          ,RENCN.slm_OD,slm_FT,RENCN.slm_DeDuctibleFlag
                          ,RENCN.slm_PersonalAccident,RENCN.slm_PersonalAccidentMan
                          ,RENCN.slm_MedicalFee,RENCN.slm_MedicalFeeMan
                          ,RENCN.slm_InsuranceDriver,RENCN.slm_PolicyGrossStamp
                          ,RENCN.slm_PolicyGrossVat,RENCN.slm_PolicyGrossPremium
                          ,RENCN.slm_NetGrossPremium,RENCN.slm_PolicyGrossPremiumPay
                          ,RENCN.slm_CostSave,RENCN.slm_CreatedDate,RENCN.slm_CreatedBy
                          ,RENCN.slm_UpdatedDate,RENCN.slm_UpdatedBy,RENCN.slm_Selected
                          ,RENCN.slm_OldPolicyNo,RENCN.slm_DriverFlag
                          ,RENCN.slm_Driver_TitleId1,RENCN.slm_Driver_First_Name1
                          ,RENCN.slm_Driver_Last_Name1,RENCN.slm_Driver_Birthdate1
                          ,RENCN.slm_Driver_TitleId2,RENCN.slm_Driver_First_Name2
                          ,RENCN.slm_Driver_Last_Name2,RENCN.slm_Driver_Birthdate2
                          ,RENCN.slm_OldReceiveNo,RENCN.slm_PolicyStartCoverDate
                          ,RENCN.slm_PolicyEndCoverDate,RENCN.slm_Vat1Percent
                          ,RENCN.slm_DiscountPercent,RENCN.slm_DiscountBath
                          ,RENCN.slm_Vat1PercentBath,RENCN.slm_Version,RENCN.slm_Year
                          ,REN.slm_PolicyNo
                        FROM KKSLM_TR_LEAD LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                        left join kkslm_tr_prelead PRE on pre.slm_TicketId = ren.slm_TicketId
                        INNER JOIN kkslm_tr_renewinsurance_compare_snap RENCN ON REN.slm_RenewInsureId = RENCN.slm_RenewInsureId 
                        left join kkslm_ms_coveragetype CVT on cvt.slm_CoverageTypeId = RENCN.slm_CoverageTypeId
                        join kkslm_tr_cusinfo cus on cus.slm_TicketId = LEAD.slm_ticketId                         
                        WHERE REN.slm_RenewInsureId = " + RenewInsureId + @"and RENCN.slm_Version = " + version 
                        +@"order by RENCN.slm_Seq"    
                        ;

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<PreleadCompareData>(sql).ToList();
            }
        }
        public List<PreleadCompareActData> GetHistoryDetailAct(string RenewInsureId, string version)
        {

            //slm_Brand_Name_Org
            //slm_Model_Code_Org
            //slm_Model_Name_Org
            //slm_Cc
            string sql = @"SELECT 
                           (select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com IC where ic.slm_Ins_Com_Id =RENCAN.slm_Ins_Com_Id and is_Deleted='0')as slm_insnameth, 
                           RENCAN.slm_RenewInsureId,RENCAN.slm_PromotionId,RENCAN.slm_Seq
                          ,RENCAN.slm_Year,RENCAN.slm_Ins_Com_Id,RENCAN.slm_ActIssuePlace
                          ,RENCAN.slm_SendDocType,RENCAN.slm_ActNo
                          ,RENCAN.slm_ActStartCoverDate,RENCAN.slm_ActEndCoverDate
                          ,RENCAN.slm_ActIssueBranch,RENCAN.slm_CarTaxExpiredDate
                          ,RENCAN.slm_ActGrossStamp,RENCAN.slm_ActGrossVat
                          ,RENCAN.slm_ActGrossPremium,RENCAN.slm_ActNetGrossPremium
                          ,RENCAN.slm_ActGrossPremiumPay,RENCAN.slm_CreatedDate
                          ,RENCAN.slm_CreatedBy,RENCAN.slm_UpdatedDate
                          ,RENCAN.slm_UpdatedBy,RENCAN.slm_ActPurchaseFlag
                          ,RENCAN.slm_DiscountPercent,RENCAN.slm_DiscountBath
                          ,RENCAN.slm_ActSignNo,RENCAN.slm_ActNetGrossPremiumFull
                          ,RENCAN.slm_Vat1Percent,RENCAN.slm_Vat1PercentBath
                          ,RENCAN.slm_Version
                    FROM KKSLM_TR_LEAD LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                    INNER JOIN kkslm_tr_renewinsurance_compare_act_snap RENCAN ON REN.slm_RenewInsureId = RENCAN.slm_RenewInsureId 
                    WHERE REN.slm_RenewInsureId = " + RenewInsureId + @"and RENCAN.slm_Version  = " + version
                    + @"order by RENCAN.slm_Seq";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<PreleadCompareActData>(sql).ToList();
            }
        }

        public RenewInsuranceData GetHistoryRenewInsurance(string RenewInsureId, string version)
        {
            string sql = string.Format("SELECT slm_BeneficiaryName FROM kkslm_tr_renewinsurance_snap WHERE slm_RenewInsureId = '{0}' AND slm_Version = '{1}' ", RenewInsureId, version);
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<RenewInsuranceData>(sql).FirstOrDefault();
            }
        } 
    }
}
