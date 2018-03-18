using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Resource;
using SLM.Resource.Data;
using System.Transactions;
using System.Globalization;

namespace SLM.Dal.Models
{
    public class ActivityLeadModel : IDisposable
    {
        private SLM_DBEntities slmdb = null;
        private OPERDBEntities operdb = null;

        public ActivityLeadModel()
        {
            slmdb = DBUtil.GetSlmDbEntities();
            operdb = DBUtil.GetOperDbEntities();
        }

        public ActivityLeadModel(SLM_DBEntities db1, OPERDBEntities db2)
        {
            slmdb = db1;
            operdb = db2;
        }

        public void Dispose()
        {
            if (slmdb != null)
            {
                slmdb.Dispose();
            }
            if (operdb != null)
            {
                operdb.Dispose();
            }

        }

        public decimal? SettleClaimReportId { get; set; }

        public PreleadData GetLead(string ticketId)
        {

            //slm_Brand_Name_Org
            //slm_Model_Code_Org
            //slm_Model_Name_Org
            //slm_Cc
            try
            {
                string sql = @"select p.* , c.slm_CardTypeName from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead p WITH (NOLOCK)
                                left join kkslm_ms_cardtype c WITH (NOLOCK) on p.slm_CardTypeId = c.slm_CardTypeId and c.is_Deleted = 0
                                where  p.slm_TicketId = '" + ticketId + "'";
                return slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public PreleadData GetPreleadbyTicket(string ticketId)
        {
            try
            {
                string sql = @"select p.*, c.slm_CardTypeName, p.slm_Cc AS slm_Cc_Org, slm_Model_Year AS slm_Model_Year_Org 
                                from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead p WITH (NOLOCK) 
                                inner join kkslm_tr_cusinfo cus WITH (NOLOCK) on p.slm_ticketId = cus.slm_TicketId 
                                left join kkslm_ms_cardtype c WITH (NOLOCK) on p.slm_CardTypeId = c.slm_CardTypeId and c.is_Deleted = 0
                                where  p.slm_ticketId = '" + ticketId + "'";
                return slmdb.ExecuteStoreQuery<PreleadData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }


        public RenewInsuranceData GetRenewInsurance(string ticketId)
        {
            try
            {
                string sql = @"select * from dbo.kkslm_tr_renewinsurance WITH (NOLOCK) where  slm_ticketId = '" + ticketId + "'";
                return slmdb.ExecuteStoreQuery<RenewInsuranceData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }

        }

        public PreleadCompareData GetPreleadCompareById(string preleadId)
        {
            try
            {
                string sql = @"select * from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare where  slm_Prelead_Id = '" + preleadId + "' and slm_Selected = 1";
                return slmdb.ExecuteStoreQuery<PreleadCompareData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }

        }

        public PreleadCompareActData GetPreleadCompareActById(string preleadId)
        {

            try
            {
                string sql = @"select * from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_compare_act where  slm_Prelead_Id = '" + preleadId + "' and slm_ActPurchaseFlag = 1";
                return slmdb.ExecuteStoreQuery<PreleadCompareActData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public LeadDataForRenewInsure GetLeadbyTicketId(string ticketId)
        {
            //            string sql = @"select lead.*, c.slm_CardTypeName, c.slm_CardTypeId ,cus.slm_CitizenId, lead.slm_Product_Id  ,slm_Owner 
            //                        FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead left join kkslm_tr_cusinfo cus on lead.slm_ticketId = cus.slm_TicketId 
            //                        left join kkslm_ms_cardtype c on cus.slm_CardType = c.slm_CardTypeId and c.is_Deleted = 0
            //                        where lead.is_Deleted = 0 and lead.slm_ticketId = '" + ticketId + "'" +
            //                    " and lead.slm_Status not in ('08','09','10')";

            //            return slmdb.ExecuteStoreQuery<LeadData>(sql).FirstOrDefault();

            string sql = @"select lead.slm_ticketId AS TicketId, c.slm_CardTypeName, c.slm_CardTypeId, cus.slm_CitizenId, lead.slm_Product_Id, lead.slm_Owner, lead.slm_TelNo_1 
                            , prod.slm_ProvinceRegis, ISNULL(t.slm_TitleName, '') + ISNULL(lead.slm_Name, '') + ' ' + ISNULL(lead.slm_LastName, '') AS ClientFullname  
                            FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) 
                            left join kkslm_tr_cusinfo cus WITH (NOLOCK) on lead.slm_ticketId = cus.slm_TicketId 
                            left join kkslm_tr_productinfo prod WITH (NOLOCK) on lead.slm_ticketId = prod.slm_TicketId 
                            left join kkslm_ms_cardtype c WITH (NOLOCK) on cus.slm_CardType = c.slm_CardTypeId and c.is_Deleted = 0 
                            left join kkslm_ms_title t WITH (NOLOCK) on lead.slm_TitleId = t.slm_TitleId
                            where lead.is_Deleted = 0 and lead.slm_ticketId = '" + ticketId + "' ";

            return slmdb.ExecuteStoreQuery<LeadDataForRenewInsure>(sql).FirstOrDefault();
        }

        public List<LeadDataForRenewInsure> GetLeads(string ticketId, string username)
        {
            try
            {
                //string sql = @"select lead.slm_ticketId AS TicketId, c.slm_CardTypeName, c.slm_CardTypeId, cus.slm_CitizenId,lead.slm_Product_Id, lead.slm_Owner, lead.slm_TelNo_1
                //                FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead left join kkslm_tr_cusinfo cus on lead.slm_ticketId = cus.slm_TicketId 
                //                left join kkslm_ms_cardtype c on cus.slm_CardType = c.slm_CardTypeId and c.is_Deleted = 0
                //                where lead.is_Deleted = 0 and lead.slm_ticketId = '" + ticketId + "' and slm_Owner = '" + username + "' ";

                //var leadData = slmdb.ExecuteStoreQuery<LeadDataForRenewInsure>(sql).FirstOrDefault();

                //if (leadData != null && leadData.slm_CitizenId != null && leadData.slm_CitizenId != "")
                //{
                //    string sql2 = @"select lead.slm_ticketId AS TicketId, c.slm_CardTypeName, c.slm_CardTypeId, cus.slm_CitizenId, lead.slm_Product_Id, lead.slm_Owner, lead.slm_TelNo_1
                //                    FROM kkslm_tr_lead lead 
                //                        left join kkslm_tr_cusinfo cus on lead.slm_ticketId = cus.slm_TicketId 
                //                     inner join kkslm_tr_renewinsurance ren on ren.slm_TicketId = lead.slm_ticketId 
                //                     left join kkslm_tr_prelead pre on pre.slm_TicketId = lead.slm_ticketId 
                //                        left join kkslm_ms_cardtype c on cus.slm_CardType = c.slm_CardTypeId and c.is_Deleted = 0
                //                    where lead.slm_Status not in ('08','09','10') and lead.is_Deleted = 0 and 
                //                    lead.slm_Product_Id = '" + leadData.slm_Product_Id + "' and cus.slm_CitizenId = '" + leadData.slm_CitizenId + "'" +
                //                        " and lead.slm_Owner = '" + leadData.slm_Owner + "'" +
                //                        " and lead.slm_ticketId <> " + ticketId +
                //                        " order by lead.slm_CreatedDate asc";

                //    return slmdb.ExecuteStoreQuery<LeadDataForRenewInsure>(sql2).ToList();
                //}
                //else
                //{
                //    return new List<LeadDataForRenewInsure>();
                //}

                string sql = @"select lead.slm_ticketId AS TicketId, c.slm_CardTypeName, c.slm_CardTypeId, cus.slm_CitizenId,lead.slm_Product_Id, lead.slm_Owner, lead.slm_TelNo_1 
                                , prod.slm_ProvinceRegis 
                                FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) 
                                left join kkslm_tr_cusinfo cus WITH (NOLOCK) on lead.slm_ticketId = cus.slm_TicketId 
                                left join kkslm_tr_productinfo prod WITH (NOLOCK) on lead.slm_ticketId = prod.slm_TicketId 
                                left join kkslm_ms_cardtype c WITH (NOLOCK) on cus.slm_CardType = c.slm_CardTypeId and c.is_Deleted = 0
                                where lead.is_Deleted = 0 and lead.slm_ticketId = '" + ticketId + "' and slm_Owner = '" + username + "' ";

                var leadData = slmdb.ExecuteStoreQuery<LeadDataForRenewInsure>(sql).FirstOrDefault();

                if (leadData != null && leadData.slm_CitizenId != null && leadData.slm_CitizenId != "")
                {
                    string sql2 = @"select lead.slm_ticketId AS TicketId, c.slm_CardTypeName, c.slm_CardTypeId, cus.slm_CitizenId, lead.slm_Product_Id, lead.slm_Owner, lead.slm_TelNo_1 
                                    , prod.slm_ProvinceRegis, ISNULL(t.slm_TitleName, '') + ISNULL(lead.slm_Name, '') + ' ' + ISNULL(lead.slm_LastName, '') AS ClientFullname 
                                    FROM kkslm_tr_lead lead WITH (NOLOCK) 
                                        left join kkslm_tr_cusinfo cus WITH (NOLOCK) on lead.slm_ticketId = cus.slm_TicketId 
                                        left join kkslm_tr_productinfo prod WITH (NOLOCK) on lead.slm_ticketId = prod.slm_TicketId 
	                                    inner join kkslm_tr_renewinsurance ren WITH (NOLOCK) on ren.slm_TicketId = lead.slm_ticketId 
	                                    left join kkslm_tr_prelead pre WITH (NOLOCK) on pre.slm_TicketId = lead.slm_ticketId 
                                        left join kkslm_ms_cardtype c WITH (NOLOCK) on cus.slm_CardType = c.slm_CardTypeId and c.is_Deleted = 0 
                                        left join kkslm_ms_title t WITH (NOLOCK) on lead.slm_TitleId = t.slm_TitleId 
                                    where lead.slm_Status not in ('08','09','10') and lead.is_Deleted = 0 and 
                                    lead.slm_Product_Id = '" + leadData.slm_Product_Id + "' and cus.slm_CitizenId = '" + leadData.slm_CitizenId + "'" +
                                        " and lead.slm_Owner = '" + leadData.slm_Owner + "'" +
                                        " and lead.slm_ticketId <> " + ticketId +
                                        " order by lead.slm_CreatedDate asc";

                    return slmdb.ExecuteStoreQuery<LeadDataForRenewInsure>(sql2).ToList();
                }
                else
                {
                    return new List<LeadDataForRenewInsure>();
                }
            }
            catch
            {
                throw;
            }
        }

        public DateTime? GetStatusDateByTicketId(string ticketId)
        {
            try
            {
                string sql = @"select lead.slm_StatusDate 
                            FROM dbo.kkslm_tr_lead lead WITH (NOLOCK) 
                            where lead.is_Deleted = 0 and lead.slm_ticketId = '" + ticketId + "' ";

                return slmdb.ExecuteStoreQuery<DateTime?>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public PreleadCompareData GetPreleadtoCompare(string ticketId)
        {
            try
            {
                string sql = @"select slm_Voluntary_Policy_Number AS slm_OldPolicyNo
              
              ,convert(int,slm_Voluntary_Type_Key) AS slm_CoverageTypeId
	          ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = p.slm_Voluntary_Type_Key ) as slm_CoverageTypeName
              , slm_Voluntary_Company_Name as slm_insnameth
              ,(select slm_Ins_Com_Id from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com WITH (NOLOCK) where is_deleted = 0 and slm_InsCode  = p.slm_Voluntary_Company_Code)as slm_Ins_Com_Id
	          ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title WITH (NOLOCK) where slm_TitleId = p.slm_Driver_TitleId1)as slm_TitleName1
	          ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title WITH (NOLOCK) where slm_TitleId = p.slm_Driver_TitleId2)as slm_TitleName2
	          ,slm_Voluntary_Policy_Eff_Date As slm_PolicyStartCoverDate
              ,slm_Voluntary_Policy_Exp_Date As slm_PolicyEndCoverDate
              ,slm_Voluntary_Cov_Amt As slm_OD
              ,slm_Voluntary_Gross_Premium As   slm_PolicyGrossPremium
              ,slm_Voluntary_Company_Code AS slm_Ins_Com_Id_str
              ,slm_Driver_TitleId1 AS slm_Driver_TitleId1
              ,slm_Driver_First_Name1 As slm_Driver_First_Name1
              ,slm_Driver_Last_Name1 As slm_Driver_Last_Name1
              ,slm_Driver_Birthdate1 As slm_Driver_Birthdate1
              ,slm_Driver_TitleId2 As slm_Driver_TitleId2
              ,slm_Driver_Birthdate2 AS slm_Driver_Birthdate2
	          ,slm_Driver_First_Name2 As slm_Driver_First_Name2
	          ,slm_Driver_Last_Name2 As slm_Driver_Last_Name2
              ,slm_CardTypeId As slm_CardTypeId
              ,slm_Compulsory_Policy_Eff_Date AS slm_ActStartCoverDate
			  ,slm_Compulsory_Policy_Exp_Date AS slm_ActEndCoverDate
              ,slm_Compulsory_Gross_Premium AS slm_Compulsory_Gross_Premium
              ,(DATEDIFF(day,slm_Compulsory_Policy_Eff_Date,slm_Compulsory_Policy_Exp_Date)) AS slm_DateCount
              from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead p WITH (NOLOCK) 
                where p.slm_TicketId = '" + ticketId + "'";
                return slmdb.ExecuteStoreQuery<PreleadCompareData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }

        }

        public PreleadCompareActData GetPreleadtoCompareAct(string ticketId)
        {
            try
            {
                string sql = @"select slm_Voluntary_Policy_Number AS slm_OldPolicyNo
              ,convert(int,slm_Voluntary_Type_Key) AS slm_CoverageTypeId
	          ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = p.slm_Voluntary_Type_Key ) as slm_ConverageTypename
              ,(select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com WITH (NOLOCK) where  is_deleted = 0 and slm_InsCode  = p.slm_Voluntary_Company_Code)as slm_insnameth
              ,(select slm_Ins_Com_Id from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com WITH (NOLOCK) where  is_deleted = 0 and slm_InsCode  = p.slm_Voluntary_Company_Code)as slm_Ins_Com_Id
	          ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title WITH (NOLOCK) where slm_TitleId = p.slm_Driver_TitleId1)as slm_TitleName1
	          ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title WITH (NOLOCK) where slm_TitleId = p.slm_Driver_TitleId2)as slm_TitleName2
	          ,slm_Voluntary_Policy_Eff_Date As slm_PolicyStartCoverDate
              ,slm_Voluntary_Policy_Exp_Date As slm_PolicyEndCoverDate
              ,slm_Voluntary_Cov_Amt As slm_OD
              ,slm_Voluntary_Gross_Premium As slm_PolicyGrossPremium 
              ,slm_Voluntary_Company_Code AS slm_Ins_Com_Id_str
              ,slm_Driver_TitleId1 AS slm_Driver_TitleId1
              ,slm_Driver_First_Name1 As slm_Driver_First_Name1
              ,slm_Driver_Last_Name1 As slm_Driver_Last_Name1
              ,slm_Driver_Birthdate1 As slm_Driver_Birthdate1
              ,slm_Driver_TitleId2 As slm_Driver_TitleId2
              ,slm_Driver_Birthdate2 AS slm_Driver_Birthdate2
	          ,slm_Driver_First_Name2 As slm_Driver_First_Name2
	          ,slm_Driver_Last_Name2 As slm_Driver_Last_Name2
              ,slm_CardTypeId As slm_CardTypeId
              ,slm_Compulsory_Policy_Eff_Date AS slm_ActStartCoverDate
			  ,slm_Compulsory_Policy_Exp_Date AS slm_ActEndCoverDate
              
              ,slm_Compulsory_Gross_Premium AS slm_ActGrossPremium
              ,slm_Compulsory_Gross_Premium AS slm_ActNetGrossPremium
              ,slm_Compulsory_Gross_Premium AS slm_ActGrossPremiumPay
              ,(DATEDIFF(day,slm_Compulsory_Policy_Eff_Date,slm_Compulsory_Policy_Exp_Date)) AS slm_DateCount
              from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead p WITH (NOLOCK) 
                where p.slm_TicketId = " + ticketId;
                return slmdb.ExecuteStoreQuery<PreleadCompareActData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }

        }

        public List<PreleadCompareData> GetLeadCompare(string ticketId)
        {
            try
            {
                string sql = @"select rc.*  
                            ,(select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com WITH (NOLOCK) where  is_deleted = 0 and slm_Ins_Com_Id  = rc.slm_Ins_Com_Id)as slm_insnameth
                            , (select slm_ConverageTypeName from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_coveragetype WITH (NOLOCK) where slm_CoverageTypeId  = rc.slm_CoverageTypeId)as slm_CoverageTypeName
                            ,(select rt.slm_RepairTypeName from kkslm_ms_repairtype rt WITH (NOLOCK) where rt.slm_RepairTypeId = rc.slm_RepairTypeId ) as slm_RepairTypeName
                            ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title WITH (NOLOCK) where slm_TitleId = rc.slm_Driver_TitleId1)as slm_TitleName1
	                        ,(SELECT slm_TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title WITH (NOLOCK) where slm_TitleId = rc.slm_Driver_TitleId2)as slm_TitleName2
                            from
                            " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance as ren WITH (NOLOCK) 
                            inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare rc WITH (NOLOCK) on ren.slm_RenewInsureId = rc.slm_RenewInsureId
                            where  slm_TicketId = " + ticketId;
                return slmdb.ExecuteStoreQuery<PreleadCompareData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        public List<PreleadCompareActData> GetLeadCompareAct(string ticketId)
        {
            try
            {
                string sql = @"select rca.*
                            ,(select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com WITH (NOLOCK) where  is_deleted = 0 and slm_Ins_Com_Id  = rca.slm_Ins_Com_Id)as slm_insnameth
                            from 
                            " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance as ren WITH (NOLOCK) 
                            inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare_act rca WITH (NOLOCK) on ren.slm_RenewInsureId = rca.slm_RenewInsureId
                            where  slm_TicketId = " + ticketId;
                return slmdb.ExecuteStoreQuery<PreleadCompareActData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        public PreleadCompareData GetNotifyPremium(string ticketId)
        {
            try
            {
                //           string sql = @"SELECT top 1
                //               slm_Id as slm_NotifyPremiumId
                //,slm_Sum_Insure  as slm_OD
                //               ,slm_InsuranceCarTypeId  as slm_coveragetypeId
                //               ,slm_NetPremium as slm_PolicyGrossPremium
                //,slm_GrossPremium  as slm_NetGrossPremium
                //               ,convert(decimal(18,0),slm_Discount_Percent)  as slm_DiscountPercent
                //,convert(decimal(18,0),slm_Discount_Amount)  as slm_DiscountBath
                //,slm_GrossPremium  - convert(decimal(18,0),slm_Discount_Amount)  slm_PolicyGrossPremiumPay
                //               ,slm_Stamp  as slm_PolicyGrossStamp
                //               ,slm_Vat_Amount  as slm_PolicyGrossVat
                //               ,isnull(slm_InsuranceComId,0)  as slm_Ins_Com_Id
                //               ,(select slm_insnameth from OPERDB.dbo.kkslm_ms_ins_com where  is_deleted = 0 and slm_Ins_Com_Id  = slm_InsuranceComId)as slm_insnameth
                //            ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = dbo.kkslm_tr_notify_premium.slm_InsuranceCarTypeId) as slm_ConverageTypename
                //               ,slm_RepairTypeId  as slm_RepairTypeId
                //               ,slm_InsExpireDate As slm_InsExpireDate,
                //            (select a.slm_RepairTypeName from kkslm_ms_repairtype a where a.slm_RepairTypeId  = dbo.kkslm_tr_notify_premium.slm_repairTypeId) as slm_RepairTypeId
                //           FROM SLMDB.dbo.kkslm_tr_notify_premium
                //           inner join kkslm_tr_prelead on slm_Chassis_No = slm_VIN
                //           where  slm_TicketId = " + ticketId +
                //                " order by slm_InsExpireDate desc, SLMDB.dbo.kkslm_tr_notify_premium.slm_CreatedDate desc";

                //           string sql = @"SELECT top 1
                //               slm_Id as slm_NotifyPremiumId
                //            ,slm_Sum_Insure  as slm_OD
                //               ,renew.slm_InsuranceCarTypeId  as slm_coveragetypeId
                //               ,slm_NetPremium as slm_PolicyGrossPremium
                //            ,slm_GrossPremium  as slm_NetGrossPremium
                //               ,convert(decimal(18,0),slm_Discount_Percent)  as slm_DiscountPercent
                //            ,convert(decimal(18,0),slm_Discount_Amount)  as slm_DiscountBath
                //,slm_GrossPremium  as slm_PolicyGrossPremiumPay
                //               ,slm_Stamp  as slm_PolicyGrossStamp
                //               ,slm_Vat_Amount  as slm_PolicyGrossVat
                //               ,isnull(dbo.kkslm_tr_notify_premium.slm_InsuranceComId,0)  as slm_Ins_Com_Id
                //               ,(select slm_insnameth from " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com where  is_deleted = 0 and slm_Ins_Com_Id  = dbo.kkslm_tr_notify_premium.slm_InsuranceComId)as slm_insnameth
                //            ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a where a.slm_CoverageTypeId = dbo.kkslm_tr_notify_premium.slm_InsuranceCarTypeId) as slm_ConverageTypename
                //               ,dbo.kkslm_tr_notify_premium.slm_RepairTypeId  as slm_RepairTypeId
                //               ,slm_InsExpireDate As slm_InsExpireDate,
                //            (select a.slm_RepairTypeName from kkslm_ms_repairtype a where a.slm_RepairTypeId  = dbo.kkslm_tr_notify_premium.slm_repairTypeId) as slm_RepairTypeId
                //               , dbo.kkslm_tr_notify_premium.slm_CreatedDate
                //           FROM kkslm_tr_notify_premium
                //           inner join kkslm_tr_renewinsurance renew on renew.slm_ChassisNo = slm_VIN
                //           where  renew.slm_TicketId = " + ticketId +
                //                " order by slm_InsExpireDate desc, kkslm_tr_notify_premium.slm_CreatedDate desc";

                string sql = @"SELECT top 1
                                    slm_Id as slm_NotifyPremiumId
	                                ,slm_Sum_Insure  as slm_OD
                                    ,renew.slm_InsuranceCarTypeId  as slm_coveragetypeId
                                    ,slm_NetPremium as slm_PolicyGrossPremium
	                                ,slm_GrossPremium  as slm_NetGrossPremium
                                    ,convert(decimal(18,0),slm_Discount_Percent)  as slm_DiscountPercent
	                                ,convert(decimal(18,0),slm_Discount_Amount)  as slm_DiscountBath
	                                ,slm_GrossPremium  as slm_PolicyGrossPremiumPay
                                    ,slm_Stamp  as slm_PolicyGrossStamp
                                    ,slm_Vat_Amount  as slm_PolicyGrossVat
                                    ,isnull(np.slm_InsuranceComId,0)  as slm_Ins_Com_Id
                                    ,(select slm_insnameth from " + SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com WITH (NOLOCK) where  is_deleted = 0 and slm_Ins_Com_Id  = np.slm_InsuranceComId)as slm_insnameth
	                                ,(select a.slm_ConverageTypeName from kkslm_ms_coveragetype a WITH (NOLOCK) where a.slm_CoverageTypeId = np.slm_InsuranceCarTypeId) as slm_ConverageTypename
                                    ,np.slm_RepairTypeId  as slm_RepairTypeId
                                    ,slm_InsExpireDate As slm_InsExpireDate,
	                                (select a.slm_RepairTypeName from kkslm_ms_repairtype a where a.slm_RepairTypeId  = np.slm_repairTypeId) as slm_RepairTypeId
                                    , np.slm_CreatedDate, np.slm_PeriodYear
                                FROM kkslm_tr_notify_premium np WITH (NOLOCK) 
                                inner join kkslm_tr_renewinsurance renew WITH (NOLOCK) on REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(renew.slm_ChassisNo,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') and renew.slm_PeriodMonth = np.slm_PeriodMonth and renew.slm_PeriodYear = np.slm_PeriodYear 
                                where  renew.slm_TicketId = '" + ticketId + @"'
                                order by slm_InsExpireDate desc, np.slm_CreatedDate desc ";

                var data = slmdb.ExecuteStoreQuery<PreleadCompareData>(sql).FirstOrDefault();
                if (data != null)
                {
                    string remark = "";
                    if (data.slm_DiscountPercent != null && data.slm_DiscountBath != null)
                    {
                        remark += data.slm_DiscountPercent.Value.ToString("#,##0") + "%";
                        remark += " เป็นเงิน " + data.slm_DiscountBath.Value.ToString("#,##0.00") + " บาท";
                    }
                    else if (data.slm_DiscountPercent != null)
                    {
                        remark += data.slm_DiscountPercent.Value.ToString("#,##0") + "%";
                    }
                    else if (data.slm_DiscountBath != null)
                    {
                        remark += data.slm_DiscountBath.Value.ToString("#,##0.00") + " บาท";
                    }

                    if (data.slm_CreatedDate != null)
                        remark += " นำเข้าวันที่ " + data.slm_CreatedDate.Value.ToString("dd/MM/") + data.slm_CreatedDate.Value.Year.ToString() + " " + data.slm_CreatedDate.Value.ToString("HH:mm:ss");

                    data.PolicyDiscountRemark = remark;
                    data.slm_DiscountBath = null;
                    data.slm_DiscountPercent = null;
                }

                return data;
            }
            catch
            {
                throw;
            }

        }

        public PreleadAddressData GetPreleadAddress(string ticketId)
        {
            try
            {
                string sql = @"SELECT *
                                ,slm_BuildingName slm_Building
                                ,slm_Tambon slm_TambolId 
                                ,slm_Amphur slm_Amphur_Id
                                ,slm_Province slm_Province_Id
                                ,slm_PostalCode  slm_Zipcode
                                FROM kkslm_tr_renewinsurance pre WITH (NOLOCK) 
                        inner join kkslm_tr_renewinsurance_address pa WITH (NOLOCK) on pre.slm_RenewInsureId = pa.slm_RenewInsureId  WHERE SLM_TICKETID = " + ticketId + " AND slm_AddressType = 'D'";
                return slmdb.ExecuteStoreQuery<PreleadAddressData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }

        }

        public PromotionInsuranceData getPromotion(int PromotionId)
        {
            try
            {
                string sql = @"SELECT slm_PromotionId
                  ,slm_Product_Id
                  ,ins.slm_InsNameTh as insname
                  ,camins.slm_CampaignName as camname
	              ,slm_DurationYear
                  ,slm_EffectiveDateFrom as EffectiveDateFrom
                  ,slm_EffectiveDateTo as EffectiveDateTo
                  ,brand.slm_BrandName as brandname
                  ,model.slm_ModelName as modelname
	              ,slm_UseCarType
                  ,ct.slm_ConverageTypeName as converagetypename
	              ,slm_AgeDrivenFlag
                  ,rt.slm_RepairTypeName as repairname
	              ,slm_AgeCarYear
	              ,slm_EngineSize
                  ,slm_OD
                  ,slm_FT
                  ,slm_DeDuctible
                  ,slm_GrossPremium
                  ,slm_Stamp
                  ,slm_Vat
                  ,slm_NetGrossPremium
                  ,slm_Act
                  ,slm_InjuryDeath
                  ,slm_TPPD
	              ,slm_PersonalAccident
                  ,slm_PersonalAccidentDriver
                  ,slm_PersonalAccidentPassenger
                  ,slm_MedicalFee
                  ,slm_MedicalFeeDriver
                  ,slm_MedicalFeePassenger
                  ,slm_InsuranceDriver
                  ,slm_Remark
              FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance pis inner join " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com ins on  ins.is_deleted = 0 and ins.slm_Ins_Com_Id = pis.slm_Ins_Com_Id
		            INNER JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_campaigninsurance camins on camins.slm_CampaignInsuranceId = pis.slm_CampaignInsuranceId
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_repairtype rt on rt.slm_RepairTypeId = pis.slm_RepairTypeId 
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_coveragetype ct on ct.slm_CoverageTypeId = pis.slm_CoverageTypeId 
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_brand brand on brand.slm_BrandCode = pis.slm_Brand_Code 
		            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_redbook_model model on model.slm_ModelCode = pis.slm_Model_Code and model.slm_BrandCode = pis.slm_Brand_Code
              WHERE slm_PromotionId = " + PromotionId;
                return slmdb.ExecuteStoreQuery<PromotionInsuranceData>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }

        }



        public List<ProblemDetailData> getProblem(string ticketId)
        {
            try
            {
                var sql = @"select pro.slm_ProblemDetailId, convert(date,pro.slm_ProblemDate) slm_ProblemDate ,pro.slm_InsType,pro.slm_Contract_Number,
		        pro.slm_Name, pro.slm_ProblemDetail,pro.slm_CauseDetail,pro.slm_IsAction,
		                slm_FixTypeFlag,ins.slm_InsNameTh ,pro.slm_ResponseDetail, pro.slm_Remark, isnull(slm_Export_Flag, 0) slm_Export_Flag 
                FROM kkslm_tr_problemdetail pro WITH (NOLOCK) inner join " + Resource.SLMConstant.OPERDBName + ".dbo.kkslm_ms_ins_com ins WITH (NOLOCK) on " + /*  ins.is_deleted = 0 and */ @" ins.slm_Ins_Com_Id = pro.slm_Ins_Com_Id 
                        left join kkslm_tr_problemcomeback_report report WITH (NOLOCK) on pro.slm_ProblemDetailId = report.slm_ProblemDetailId
                WHERE pro.is_Deleted = 0 AND pro.slm_ticketId = '" + ticketId + "'";
                return slmdb.ExecuteStoreQuery<ProblemDetailData>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<RenewInsurancePaymentMainData> getRenewInsurancePaymentMain(string ticketId)
        {
            try
            {
                var sql = @"SELECT ren.slm_RenewInsureId,slm_Seq,slm_Type,slm_PaymentDate,
                      slm_Period,slm_PaymentAmount
                FROM kkslm_tr_renewinsurance ren WITH (NOLOCK) 
                    inner join kkslm_tr_renewinsurance_paymentmain repm WITH (NOLOCK) on  ren.slm_RenewInsureId = repm.slm_RenewInsureId
                where  slm_ticketId = '" + ticketId + "'" +
                                " order by slm_Seq asc";
                return slmdb.ExecuteStoreQuery<RenewInsurancePaymentMainData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        //ตรวจสอบว่ามีการซื้อประกันหรือ พรบ. หรือไม่ 
        public int GetResultActBuy(string ticketId)
        {

            try
            {
                string sql = @"SELECT SUM(A.CNT) AS RESULT
                            FROM(
                            SELECT COUNT(*) AS CNT FROM dbo.kkslm_tr_renewinsurance_compare WITH (NOLOCK) 
                            WHERE slm_Selected = 1 and slm_RenewInsureId = 
                            (	SELECT	slm_RenewInsureId 
	                            FROM	dbo.kkslm_tr_renewinsurance WITH (NOLOCK) 
	                            WHERE slm_ticketId = " + ticketId + " )" +
                                         " UNION ALL " +
                                         @"SELECT COUNT(*) AS CNT  FROM dbo.kkslm_tr_renewinsurance_compare_act WITH (NOLOCK) 
                            WHERE slm_ActPurchaseFlag = 1 and slm_RenewInsureId = 
                            (	SELECT	slm_RenewInsureId 
	                            FROM	dbo.kkslm_tr_renewinsurance WITH (NOLOCK) 
	                            WHERE slm_ticketId = " + ticketId + ")) AS A";

                return slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public decimal? GetRecAmount(string ticketId, string paymentCode)
        {
            try
            {
                string sql = @"select SUM(slm_RecAmount) slm_RecAmount from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail WITH (NOLOCK) 
                where slm_PaymentCode = '" + paymentCode + "' and slm_RenewInsuranceReceiptId in" +
                                "(select slm_RenewInsuranceReceiptId from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt WITH (NOLOCK) where slm_ticketId = '" + ticketId + "')";
                return slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }

        }

        public decimal? GetDiscountPercentInConfig(string username)
        {
            try
            {
                string sql = @"select disc.slm_DiscountPercent AS DiscountPercent
                            from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff left join kkslm_ms_discount disc on
                            staff.slm_StaffTypeId = disc.slm_StaffTypeId 
                            where staff.is_Deleted = 0 and  slm_UserName ='" + username + "'";

                return slmdb.ExecuteStoreQuery<decimal?>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }



        public List<PaymentTransMainData> GetPaymentTransMain(string ticketId)
        {
            try
            {
                string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymenttransmain WITH (NOLOCK) where slm_RenewInsureId = 
                            (select slm_RenewInsureId from kkslm_tr_renewinsurance WITH (NOLOCK) where slm_TicketId = '" + ticketId + "')";

                return slmdb.ExecuteStoreQuery<PaymentTransMainData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        public List<RenewInsurancePaymentMainData> GetPaymentMain(string ticketId)
        {
            try
            {
                string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymentmain
                            where slm_type= '1' and slm_RenewInsureId = 
                                (select slm_RenewInsureId from kkslm_tr_renewinsurance 
                            where slm_TicketId = '" + ticketId + "')";
                return slmdb.ExecuteStoreQuery<RenewInsurancePaymentMainData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        public List<PaymentTransMainData> GetPreLeadToPaymentTransMain(string ticketId)
        {
            try
            {
                string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymentmain
                            where slm_type= '1' and slm_RenewInsureId = 
                                (select slm_RenewInsureId from kkslm_tr_renewinsurance 
                            where slm_TicketId = '" + ticketId + "')";
                return slmdb.ExecuteStoreQuery<PaymentTransMainData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        public List<PaymentTransData> GetPaymentTrans(string ticketId)
        {
            try
            {
                string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymenttrans WITH (NOLOCK) where slm_type= '1' and slm_RenewInsureId = 
                           (select slm_RenewInsureId from kkslm_tr_renewinsurance WITH (NOLOCK) where slm_TicketId = " + ticketId + ")";

                return slmdb.ExecuteStoreQuery<PaymentTransData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        public List<PaymentTransData> GetPreLeadToPaymentTrans(string ticketId)
        {
            try
            {
                string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymentmain 
                            where slm_type= '1' and slm_RenewInsureId = (
                            select slm_RenewInsureId 
                        from kkslm_tr_renewinsurance where slm_TicketId = '" + ticketId + "')";
                return slmdb.ExecuteStoreQuery<PaymentTransData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }


        public List<RenewInsuranceReceiptData> GetReceipt(string ticketId)
        {
            try
            {
                string sql = @"SELECT rr.slm_RenewInsuranceReceiptId, rr.slm_RecNo, sum(rrd.slm_RecAmount) as total, rr.slm_Status As slm_Status
                              ,(select count(1) from kkslm_tr_renewinsurance_receipt_revision_detail rrd WITH (NOLOCK) 
							  where rrd.slm_Export_Flag = 1 and rrd.slm_Export_Date is not null  
							  and rrd.slm_RenewInsuranceReceiptId = rr.slm_RenewInsuranceReceiptId)  as countExport
                              FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_Tr_lead lead WITH (NOLOCK) 
                              inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt rr WITH (NOLOCK) on rr.slm_ticketId = lead.slm_ticketId
                              left join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail rrd WITH (NOLOCK) on rrd.slm_RenewInsuranceReceiptId = rr.slm_RenewInsuranceReceiptId
                              where lead.is_Deleted = 0 and lead.slm_ticketId = '" + ticketId + @"' 
                              group by rr.slm_RenewInsuranceReceiptId, rr.slm_RecNo,rr.slm_Status,rr.slm_CreatedDate 
                              order by rr.slm_CreatedDate  desc ";

                return slmdb.ExecuteStoreQuery<RenewInsuranceReceiptData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }

        public List<RenewInsuranceReceiptDetailData> GetImportList(string id)
        {
            try
            {
                string sql = @"SELECT rd.slm_PaymentCode, SLM_PAYMENTDESC,slm_InsNoDesc,slm_InstNo,slm_RecNo,slm_RecAmount,slm_TransDate
                                   , rd.slm_CreatedDate, slm_RecBy 
                            FROM   " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail rd
                            inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_payment p on p.slm_PaymentCode = rd.slm_PaymentCode
                            WHERE  slm_RenewInsuranceReceiptId = " + id + " ";

                return slmdb.ExecuteStoreQuery<RenewInsuranceReceiptDetailData>(sql).ToList();
            }
            catch
            {
                throw;
            }

        }


        public List<RenewInsuranceReceiptDetailData> getReceiptDetail(string ticketId)
        {
            try
            {
                string sql = @"SELECT rr.slm_RenewInsuranceReceiptId, rd.slm_RenewInsuranceReceiptDetailId
		                    ,CASE WHEN p.slm_PaymentCode = '204' THEN 1 
			                        WHEN p.slm_PaymentCode = '205' THEN 2 
			                        WHEN p.slm_PaymentCode = '0HP' THEN 3 
			                        WHEN p.slm_PaymentCode = '101' THEN 4 
			                        WHEN p.slm_PaymentCode = '614' THEN 5 
			                        WHEN p.slm_PaymentCode = 'OTHER' THEN 6 ELSE 9999 END slm_Seq
				
		                    ,SLM_PAYMENTDESC	
		                    ,rd.slm_RecAmount
                            ,rd.slm_PaymentCode
                            ,rd.slm_InsNoDesc
                            ,rd.slm_InstNo
                            ,rd.slm_RecBy
                            ,rd.slm_RecNo
                            ,rd.slm_Status
                        FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_payment p WITH (NOLOCK) 
	                        left join dbo.kkslm_tr_renewinsurance_receipt_detail rd WITH (NOLOCK) on p.slm_PaymentCode = rd.slm_PaymentCode
                            left join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt rr WITH (NOLOCK) on rr.slm_RenewInsuranceReceiptId = rd.slm_RenewInsuranceReceiptId
                        WHERE p.slm_PaymentCode IN ( '204','205','0HP','101','614','OTHER') and rr.slm_TicketId = '" + ticketId + "'" +
                                       @" ORDER BY CASE WHEN p.slm_PaymentCode = '204' THEN 1 
			                            WHEN p.slm_PaymentCode = '205' THEN 2 
			                            WHEN p.slm_PaymentCode = '0HP' THEN 3 
			                            WHEN p.slm_PaymentCode = '101' THEN 4 
			                            WHEN p.slm_PaymentCode = '614' THEN 5 
			                            WHEN p.slm_PaymentCode = 'OTHER' THEN 6 ELSE 9999 END ASC";
                return slmdb.ExecuteStoreQuery<RenewInsuranceReceiptDetailData>(sql).ToList();
            }
            catch
            {
                throw;
            }


        }

        public List<RenewInsuranceReceiptRevisionDetailData> getReceiptRevisionDetail(string ticketId)
        {
            try
            {
                string sql = @"SELECT rrrd.slm_RenewInsuranceReceiptRevisionDetailId
                              ,rrrd.slm_RenewInsuranceReceiptId
                              ,rrrd.slm_PaymentCode
                              ,rrrd.slm_InsNoDesc
                              ,rrrd.slm_InstNo
                              ,rrrd.slm_RecBy
                              ,rrrd.slm_RecNo
                              ,rrrd.slm_RecAmount
                              ,rrrd.slm_TransDate
                              ,rrrd.slm_Status
                              ,rrrd.slm_Selected
                              ,rrrd.slm_Seq
                              ,rrrd.slm_PaymentOtherDesc
                              ,rrrd.slm_CreatedDate
                              ,rrrd.slm_CreatedBy
                        FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_revision_detail rrrd WITH (NOLOCK) 
                            inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt rr WITH (NOLOCK) on rr.slm_RenewInsuranceReceiptId = rrrd.slm_RenewInsuranceReceiptId
                        where slm_ticketId=" + ticketId;
                return slmdb.ExecuteStoreQuery<RenewInsuranceReceiptRevisionDetailData>(sql).ToList();
            }
            catch
            {
                throw;
            }
        }

        public string GetPhonecallLogContent(string ticketId)
        {
            try
            {
                string sql = @"
                           select 'เลขที่รับแจ้ง:' + isnull(renew.slm_ReceiveNo, '')
                                 + ', วันที่ส่งแจ้ง พรบ.: ' + ISNULL(convert(varchar(10), renew.slm_ActSendDate, 103), '')	
                                 + ', Incentive ประกัน:' + case when compare.slm_RenewInsureId is not null
                                		                 then case when renew.slm_IncentiveFlag is null or renew.slm_IncentiveFlag = 0 
                                                            then 'N'
                                				            else 'Y' end
                                		                 else '' end
                                  + ', Incentive พรบ.:' + case when renew.slm_ActPurchaseFlag = 1 and renew.slm_ActIncentiveFlag = 1 then 'Y' 
							                                   else case when renew.slm_ActPurchaseFlag = 1 and  ( renew.slm_ActIncentiveFlag = 0 or renew.slm_ActIncentiveFlag is null)  then 'N' 
								                                    else case when  renew.slm_ActIncentiveFlag = 1 and (renew.slm_ActPurchaseFlag = 0 or renew.slm_ActPurchaseFlag is null) then 'Y'
									                                     else '' end
							                                   end 
						                                  end
                                  + ', ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้: '
                                  + ', ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้: ' 
                                  + ', ค่าเบี้ยประกันที่ลูกค้าชำระรวม: ' + isnull( (select convert(nvarchar(30), cast(sum(rcptd.slm_RecAmount) as money), 1)
                                  		                                  from [dbo].[kkslm_tr_renewinsurance_receipt] rcpt 
                                  		                                  join [dbo].[kkslm_tr_renewinsurance_receipt_detail] rcptd on rcpt.slm_RenewInsuranceReceiptId = rcptd.slm_RenewInsuranceReceiptId
                                  		                                  where rcpt.slm_ticketId = renew.slm_TicketId and rcptd.slm_PaymentCode = '204'
                                  		                              ), '')
                                  + ', ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม: '  + isnull( (select convert(nvarchar(30), cast(sum(rcptd.slm_RecAmount) as money), 1)
                                			                               from [dbo].[kkslm_tr_renewinsurance_receipt] rcpt 
                                			                               join [dbo].[kkslm_tr_renewinsurance_receipt_detail] rcptd on rcpt.slm_RenewInsuranceReceiptId = rcptd.slm_RenewInsuranceReceiptId
                                			                               where rcpt.slm_ticketId = renew.slm_TicketId and rcptd.slm_PaymentCode = '205'
                                			                          ), '')
                           from [dbo].[kkslm_tr_renewinsurance] renew 
                           left join [dbo].[kkslm_tr_renewinsurance_compare] compare on renew.slm_RenewInsureId = compare.slm_RenewInsureId and compare.slm_Selected = 1 
                           where renew.slm_TicketID = '" + ticketId + @"'	
                           group by renew.slm_TicketID, renew.slm_ReceiveNo, renew.slm_ActSendDate, compare.slm_RenewInsureId
                                  , renew.slm_PolicyCancelDate, renew.slm_IncentiveFlag
                                  , renew.slm_ActCancelDate, renew.slm_ActPurchaseFlag, renew.slm_ActIncentiveFlag";
                var result = slmdb.ExecuteStoreQuery<string>(sql);
                return result != null ? (result.FirstOrDefault() ?? "") : "";
            }
            catch
            {
                throw;
            }
        }

        public void SaveData(PreleadCompareDataCollection data, bool PolicyPaymentMainFlag, bool PolicyPaymentMainActFlag, BPReportData bpReport, string claimFlag, string userName, out kkslm_tr_activity actForSLA, bool fromReceiveClick)
        {
            try
            {
                //int notcloseBefore = 0;
                //int notcloseAfter = 0;
                List<decimal> problemDetailIdList = new List<decimal>();

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //Lead

                    //get last version for snap
                    int? OldActPayMethodId = 0;
                    int? OldPolicyPayMethodId = 0;

                    var prod = slmdb.kkslm_tr_productinfo.Where(p => p.slm_TicketId == data.RenewIns.slm_TicketId).FirstOrDefault();
                    if (prod != null)
                    {
                        prod.slm_ProvinceRegis = data.lead.slm_ProvinceRegis;
                    }

                    kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_RenewInsureId == data.RenewIns.slm_RenewInsureId).FirstOrDefault();
                    if (obj != null)
                    {
                        OldActPayMethodId = data.RenewIns.slm_ActPayMethodId;
                        OldPolicyPayMethodId = data.RenewIns.slm_PolicyPayMethodId;
                    }


                    deleteRenewinsuranceCompare(data.RenewIns.slm_RenewInsureId);
                    int lastCompareVersion = getRenewinsuranceCompareVersion(data.RenewIns.slm_RenewInsureId);
                    int lastActVersion = getRenewinsuranceCompareActVersion(data.RenewIns.slm_RenewInsureId);
                    int countBuyPolicy = 0;
                    int countBuyAct = 0;

                    if (data.CompareCurr != null && data.CompareCurr.slm_Selected == true)
                        countBuyPolicy = 1;
                    else if (data.ComparePromoList != null)
                    {
                        countBuyPolicy = data.ComparePromoList.Where(a => a.slm_Selected != null && a.slm_Selected.Value == true).Count();
                    }
                    if (data.ActPromoList != null)
                    {
                        countBuyAct = data.ActPromoList.Where(a => a.slm_ActPurchaseFlag != null && a.slm_ActPurchaseFlag.Value == true).Count();
                    }

                    if (data.RenewIns != null)
                    {
                        SaveRenewInsuranceSnap(data.RenewIns, lastCompareVersion);
                    }
                    data.RenewIns = UpdateRenewinsurancePayment(data.RenewIns, PolicyPaymentMainFlag, countBuyPolicy, countBuyAct, claimFlag, userName);

                    bool clearPolicyRenew = false;
                    bool clearActRenew = false;

                    if (data.ComparePrev != null)
                    {
                        SaveRenewinsuranceCompare(data.ComparePrev);
                        SaveRenewinsuranceCompareSnap(data.ComparePrev, lastCompareVersion);

                    }
                    if (data.CompareCurr != null)
                    {
                        SaveRenewinsuranceCompare(data.CompareCurr);
                        SaveRenewinsuranceCompareSnap(data.CompareCurr, lastCompareVersion);
                        if (data.CompareCurr.slm_Selected != null)
                        {
                            if (data.CompareCurr.slm_Selected.Value)
                            {
                                UpdateRenewinsurance(data.RenewIns, data.CompareCurr, PolicyPaymentMainFlag);
                                clearPolicyRenew = true;
                            }
                        }

                    }
                    if (data.ComparePromoList != null)
                    {
                        foreach (PreleadCompareData pro in data.ComparePromoList)
                        {
                            SaveRenewinsuranceCompare(pro);
                            SaveRenewinsuranceCompareSnap(pro, lastCompareVersion);
                            if (pro.slm_Selected != null)
                            {
                                if (pro.slm_Selected != null && pro.slm_Selected.Value)
                                {
                                    UpdateRenewinsurance(data.RenewIns, pro, PolicyPaymentMainFlag);
                                    clearPolicyRenew = true;
                                    data.RenewIns = GetRenewInsurance(data.RenewIns.slm_TicketId);
                                }
                            }
                        }
                    }

                    //ข้อมูลรับชำระเงิน
                    DeleteRenewinsurancePaymentmain(data.RenewIns.slm_RenewInsureId);
                    if (data.PayMainList != null)
                    {
                        foreach (RenewInsurancePaymentMainData Pay in data.PayMainList)
                        {
                            SaveRenewinsurancePaymentmain(Pay);
                        }
                    }


                    if (data.Address != null)
                    {
                        SaveRenewinsuranceAddress(data);
                    }

                    deleteRenewinsuranceCompareAct(data.RenewIns.slm_RenewInsureId);
                    if (data.ActPromoList != null)
                    {
                        foreach (PreleadCompareActData proAct in data.ActPromoList)
                        {
                            SaveRenewinsuranceCompareAct(proAct);
                            SaveRenewinsuranceCompareActSnap(proAct, lastActVersion);
                            if (proAct.slm_ActPurchaseFlag != null && proAct.slm_ActPurchaseFlag.Value)
                            {
                                UpdateRenewinsuranceAct(data.RenewIns, proAct, PolicyPaymentMainActFlag);
                                clearActRenew = true;
                            }
                        }
                    }

                    GenerateBPReport(data.RenewIns, PolicyPaymentMainFlag, countBuyPolicy, countBuyAct, bpReport, OldActPayMethodId, OldPolicyPayMethodId);

                    if (!clearPolicyRenew)
                    {
                        CancelPolicy(data, false);
                    }

                    if (!clearActRenew)
                    {
                        CancelAct(data, false);
                    }

                    if (data.ActPrev != null)
                    {
                        SaveRenewinsuranceCompareAct(data.ActPrev);
                        SaveRenewinsuranceCompareActSnap(data.ActPrev, lastActVersion);
                    }

                    if (data.PayMainActList != null && data.PayMainActList.Count > 0)
                    {
                        foreach (RenewInsurancePaymentMainData payMain in data.PayMainActList)
                        {
                            SaveRenewinsurancePaymentmainAct(payMain);
                        }
                    }


                    if (PolicyPaymentMainFlag)
                    {
                        UpdateRenewinsurancePay(data.RenewIns);
                    }

                    if (data.ReceiptList != null && data.ReceiptList.Count > 0)
                    {
                        foreach (RenewInsuranceReceiptData r in data.ReceiptList)
                        {
                            UpdateRenewinsuranceReceipt(r);
                            UpdateRenewinsuranceReceiptDetail(r);
                        }
                    }

                    if (data.ReceiptRevisionDetailList != null && data.ReceiptRevisionDetailList.Count > 0)
                    {
                        DeleteRenewinsuranceReceiptRevisionDetail(data.RenewIns.slm_TicketId);

                        foreach (RenewInsuranceReceiptRevisionDetailData rrd in data.ReceiptRevisionDetailList)
                        {
                            SaveRenewinsuranceReceiptRevisionDetail(rrd);
                        }

                        if (data.EditReceiptFlag)
                        {
                            insertkkslm_tr_amendmentbill_report(data.RenewIns.slm_TicketId, data.RenewIns.slm_UpdatedBy);
                        }
                    }

                    //ข้อมูลงานติดปัญหา 
                    if (data.ProblemList != null && data.ProblemList.Count > 0)
                    {
                        problemDetailIdList = GetProblemDetailIdList(data.RenewIns.slm_TicketId);     //Get detail Id ของงานที่ยังติดปัญหา
                        foreach (ProblemDetailData p in data.ProblemList)
                        {
                            UpdateProblemdetail(p);
                        }
                    }

                    #region Update 5 Log Notify Report

                    if (data.ReceipNo != null && data.ReceipNo != "")
                    {
                        kkslm_tr_notify_renewinsurance_report update = slmdb.kkslm_tr_notify_renewinsurance_report.Where(item => item.slm_TicketId == data.RenewIns.slm_TicketId && (item.slm_Export_Flag == false || item.slm_Export_Flag == null) && item.is_Deleted != true).FirstOrDefault();
                        kkslm_tr_policynew_report updatePolicynewReport = slmdb.kkslm_tr_policynew_report.Where(e => e.slm_TicketId == data.RenewIns.slm_TicketId && (e.slm_Export_Flag == false || e.slm_Export_Flag == null) && e.is_Deleted != true).FirstOrDefault();
                        kkslm_tr_setle_claim_report updateSetleClaimReport = slmdb.kkslm_tr_setle_claim_report.Where(a => a.slm_TicketId == data.RenewIns.slm_TicketId && (a.slm_Export_Flag == false || a.slm_Export_Flag == null) && a.is_Deleted != true).FirstOrDefault();
                        PreleadCompareData preData = new PreleadCompareData();
                        if (data.CompareCurr.slm_Selected == true)
                            preData = data.CompareCurr;
                        else
                        {
                            foreach (var item in data.ComparePromoList)
                            {
                                if (item.slm_Selected == true && item.slm_Selected != null)
                                    preData = item;
                            }
                        }

                        // Policy
                        #region 1). Table kkslm_tr_notify_renewinsurance_report
                        if (update != null)
                        {
                            update.slm_RepairTypeId = preData.slm_RepairTypeId;
                            update.slm_CoverageStartDate = preData.slm_PolicyStartCoverDate;
                            update.slm_CoverageEndDate = preData.slm_PolicyEndCoverDate;
                            if (preData.slm_DriverFlag == "1")
                            {
                                update.slm_Birthdate_Driver1 = preData.slm_TitleName1 + preData.slm_Driver_First_Name1 + " " + preData.slm_Driver_Last_Name1 + " " + (preData.slm_Driver_Birthdate1.HasValue ? preData.slm_Driver_Birthdate1.Value.ToString("dd/MM/yyyy") : "");
                                update.slm_Birthdate_Driver2 = preData.slm_TitleName2 + preData.slm_Driver_First_Name2 + " " + preData.slm_Driver_Last_Name2 + " " + (preData.slm_Driver_Birthdate2.HasValue ? preData.slm_Driver_Birthdate2.Value.ToString("dd/MM/yyyy") : "");
                            }
                            else
                            {
                                update.slm_Birthdate_Driver1 = null;
                                update.slm_Birthdate_Driver2 = null;
                            }
                            update.slm_Remark = data.Prelead.slm_RemarkPolicy;
                            //--Address
                            update.slm_House_No = data.Address.slm_House_No;
                            update.slm_Moo = data.Address.slm_Moo;
                            update.slm_Village = data.Address.slm_Village;
                            update.slm_Building = data.Address.slm_Building;
                            update.slm_Soi = data.Address.slm_Soi;
                            update.slm_Street = data.Address.slm_Street;
                            update.slm_TambolId = data.Address.slm_TambolId;
                            update.slm_AmphurId = data.Address.slm_Amphur_Id;
                            update.slm_ProvinceId = data.Address.slm_Province_Id;
                            update.slm_Zipcode = data.Address.slm_Zipcode;
                            update.slm_UpdatedBy = preData.slm_UpdatedBy;
                            update.slm_UpdatedDate = DateTime.Now;

                            // slmdb.kkslm_tr_notify_renewinsurance_report.AddObject(update);
                        }
                        else
                        {
                            if (slmdb.kkslm_tr_notify_renewinsurance_report.FirstOrDefault(r => r.slm_TicketId == data.RenewIns.slm_TicketId && (r.is_Deleted == null || r.is_Deleted == false)) == null)
                            {
                                insertkkslm_tr_notify_renewinsurance_report(data.RenewIns, data.RenewIns.slm_TicketId, data.RenewIns.slm_UpdatedBy);
                            }
                        }
                        #endregion
                        #region 2). Table kkslm_tr_policynew_report
                        if (updatePolicynewReport != null)
                        {
                            var selectedAct = data.ActPromoList.FirstOrDefault(a => a.slm_ActPurchaseFlag == true);
                            updatePolicynewReport.slm_RepairTypeId = preData.slm_RepairTypeId;
                            updatePolicynewReport.slm_ActStartCoverDate = selectedAct == null
                                ? null
                                : selectedAct.slm_ActStartCoverDate;  //preData.slm_PolicyStartCoverDate;
                            updatePolicynewReport.slm_ActEndCoverDate = selectedAct == null
                                ? null
                                : selectedAct.slm_ActEndCoverDate;    //preData.slm_PolicyEndCoverDate;
                            updatePolicynewReport.slm_Remark = data.RenewIns.slm_RemarkPolicy;
                            //--Address
                            updatePolicynewReport.slm_House_No = data.Address.slm_House_No;
                            updatePolicynewReport.slm_Moo = data.Address.slm_Moo;
                            updatePolicynewReport.slm_Village = data.Address.slm_Village;
                            updatePolicynewReport.slm_Building = data.Address.slm_Building;
                            updatePolicynewReport.slm_Soi = data.Address.slm_Soi;
                            updatePolicynewReport.slm_Street = data.Address.slm_Street;
                            updatePolicynewReport.slm_TambolId = data.Address.slm_TambolId;
                            updatePolicynewReport.slm_AmphurId = data.Address.slm_Amphur_Id;
                            updatePolicynewReport.slm_ProvinceId = data.Address.slm_Province_Id;
                            updatePolicynewReport.slm_Zipcode = data.Address.slm_Zipcode;
                            updatePolicynewReport.slm_UpdatedBy = preData.slm_UpdatedBy;
                            updatePolicynewReport.slm_UpdatedDate = DateTime.Now;
                        }
                        #endregion
                        #region 3). Table kkslm_tr_setle_claim_report
                        if (updateSetleClaimReport != null && data.RenewIns.slm_ClaimFlag != null && preData.slm_PolicyStartCoverDate != null && preData.slm_PolicyEndCoverDate != null)
                        {
                            updateSetleClaimReport.slm_CoverageStartDate = preData.slm_PolicyStartCoverDate;
                            updateSetleClaimReport.slm_CoverageEndDate = preData.slm_PolicyEndCoverDate;
                            updateSetleClaimReport.slm_UpdatedDate = DateTime.Now;
                            updateSetleClaimReport.slm_UpdatedBy = preData.slm_UpdatedBy;
                        }
                        #endregion

                    }
                    // Act
                    if (data.StrReceipActDate != null || data.StrReceipActDate != "")
                    {
                        kkslm_tr_actnew_report updateActNewReport = slmdb.kkslm_tr_actnew_report.Where(item => item.slm_TicketId == data.RenewIns.slm_TicketId && (item.slm_Export_Flag == false || item.slm_Export_Flag == null) && item.is_Deleted != true).FirstOrDefault();
                        kkslm_tr_notify_act_report updateNotifyActReport = slmdb.kkslm_tr_notify_act_report.Where(item => item.slm_TicketId == data.RenewIns.slm_TicketId && (item.slm_Export_Flag == false || item.slm_Export_Flag == null) && item.is_Deleted != true).FirstOrDefault();
                        PreleadCompareActData preActData = new PreleadCompareActData();

                        preActData = data.ActPromoList.FirstOrDefault(a => a.slm_ActPurchaseFlag == true);

                        #region 1). Table kkslm_tr_notify_act_report
                        if (updateNotifyActReport != null)
                        {
                            var act = data.ActPromoList.FirstOrDefault(a => a.slm_ActPurchaseFlag == true);

                            updateNotifyActReport.slm_StartDateAct = preActData.slm_ActStartCoverDate;
                            updateNotifyActReport.slm_EndDateAct = preActData.slm_ActEndCoverDate;
                            updateNotifyActReport.slm_Remark = data.Prelead.slm_RemarkAct;
                            updateNotifyActReport.slm_NetPremiumIncludeVat = act == null
                                ? data.Prelead.slm_ActNetGrossPremium
                                : act.slm_ActNetGrossPremium;
                            //updateNotifyActReport.slm_BranchCode = act == null ? null : act.slm_ActIssueBranch;
                            //updateNotifyActReport.slm_BranchCode = act == null ? null : data.Prelead.slm_Brand_Code;
                            updateNotifyActReport.slm_BranchCode = data.Prelead == null ? null : data.Prelead.slm_BranchCode;// .Prelead.slm_Brand_Code; zz 2017-05-31

                            //-- Address
                            updateNotifyActReport.slm_House_No = data.Address.slm_House_No;
                            updateNotifyActReport.slm_Moo = data.Address.slm_Moo;
                            updateNotifyActReport.slm_Village = data.Address.slm_Village;
                            updateNotifyActReport.slm_Building = data.Address.slm_Building;
                            updateNotifyActReport.slm_Soi = data.Address.slm_Soi;
                            updateNotifyActReport.slm_Street = data.Address.slm_Street;
                            updateNotifyActReport.slm_TambolId = data.Address.slm_TambolId;
                            updateNotifyActReport.slm_AmphurId = data.Address.slm_Amphur_Id;
                            updateNotifyActReport.slm_ProvinceId = data.Address.slm_Province_Id;
                            updateNotifyActReport.slm_Zipcode = data.Address.slm_Zipcode;
                            updateNotifyActReport.slm_UpdatedDate = DateTime.Now;
                            updateNotifyActReport.slm_UpdatedBy = preActData.slm_UpdatedBy;
                        }
                        #endregion
                        #region 2). Table kkslm_tr_actnew_report
                        if (updateActNewReport != null)
                        {
                            updateActNewReport.slm_ActStartCoverDate = preActData.slm_ActStartCoverDate;
                            updateActNewReport.slm_ActEndCoverDate = preActData.slm_ActEndCoverDate;
                            updateActNewReport.slm_Remark = data.Prelead.slm_RemarkAct;
                            if (data.RenewIns.slm_Need_PolicyFlag != null && data.RenewIns.slm_Need_PolicyFlag == "Y")
                            {
                                var policy = data.ComparePromoList.FirstOrDefault(p => (p.slm_Selected ?? false) == true);
                                updateActNewReport.slm_PolicyGrossPremium = policy == null
                                    ? data.RenewIns.slm_PolicyGrossPremium
                                    : policy.slm_PolicyGrossPremium;
                            }
                            // -- Address
                            updateActNewReport.slm_House_No = data.Address.slm_House_No;
                            updateActNewReport.slm_Moo = data.Address.slm_Moo;
                            updateActNewReport.slm_Village = data.Address.slm_Village;
                            updateActNewReport.slm_Building = data.Address.slm_Building;
                            updateActNewReport.slm_Soi = data.Address.slm_Soi;
                            updateActNewReport.slm_Street = data.Address.slm_Street;
                            updateActNewReport.slm_TambolId = data.Address.slm_TambolId;
                            updateActNewReport.slm_AmphurId = data.Address.slm_Amphur_Id;
                            updateActNewReport.slm_ProvinceId = data.Address.slm_Province_Id;
                            updateActNewReport.slm_Zipcode = data.Address.slm_Zipcode;
                            updateActNewReport.slm_UpdatedDate = DateTime.Now;
                            updateActNewReport.slm_UpdatedBy = preActData.slm_UpdatedBy;
                        }
                        #endregion

                    }
                    #endregion

                    slmdb.SaveChanges();
                    scope.Complete();
                }

                // check payment adjusted & claim                   
                actForSLA = null;

                // check original selected claim flag first.
                kkslm_tr_renewinsurance reins = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_RenewInsureId == data.RenewIns.slm_RenewInsureId).FirstOrDefault();
                data.RenewIns.slm_ClaimFlag = reins.slm_ClaimFlag;

                bool isPaid = checkPaidPolicy(data.RenewIns.slm_TicketId);
                if (fromReceiveClick && !isPaid)
                {
                    claimFlag = SLMConstant.SettleClaimStatus.RevertSettleClaim;
                }

                CheckClaim(data.RenewIns, claimFlag, userName);
                reins.slm_ClaimFlag = data.RenewIns.slm_ClaimFlag;
                slmdb.SaveChanges();

                #region เช็คจ่ายเงินประกัน แล้วปรับสถานะ ถ้ามีการกด incentive แล้ว
                //bool isPaid = checkPaidPolicy(data.RenewIns.slm_TicketId);
                if (isPaid)
                {
                    if (claimFlag == SLMConstant.SettleClaimStatus.RevertSettleClaim) // && data.RenewIns.slm_IncentiveDate != null)
                    {
                        // ถ้าจ่ายเงินอยู่ในช่วง config value และโดนระงับเคลมอยู่ ให้ยกเลิกระงับเคลม
                        claimFlag = SLMConstant.SettleClaimStatus.CancelSettleClaim;
                        CheckClaim(data.RenewIns, claimFlag, userName);
                        reins.slm_ClaimFlag = data.RenewIns.slm_ClaimFlag;
                        slmdb.SaveChanges();
                    }

                    if (data.RenewIns.slm_IncentiveDate != null && data.RenewIns.slm_ReceiveDate != null)
                    {
                        var lead = slmdb.kkslm_tr_lead.FirstOrDefault(l => l.slm_ticketId == data.RenewIns.slm_TicketId
                            && (
                                (l.slm_Status == SLM.Resource.SLMConstant.StatusCode.WaitConsider && l.slm_SubStatus == SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns)
                                ||
                                (l.slm_Status == SLM.Resource.SLMConstant.StatusCode.RoutebackEdit && l.slm_SubStatus == SLM.Resource.SLMConstant.SubStatusCode.ProblemPending)
                            )
                            );

                        if (lead == null)
                        {
                            actForSLA = InsertLog(data.RenewIns, data.RenewIns.slm_UpdatedBy, SLM.Resource.SLMConstant.ActionType.ChangeStatus, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);

                            UpdateLeadIncentive(data.RenewIns, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);
                        }
                    }
                }
                #endregion

                #region เช็คจ่ายพรบ แล้วปรับสถานะ ถ้ามีการกด incentive แล้ว
                bool policyPurchase = (data.CompareCurr != null && data.CompareCurr.slm_Selected == true) || (data.ComparePromoList != null && data.ComparePromoList.Where(p => p.slm_Selected == true).Count() > 0);  //  FirstOrDefault(p => (p.slm_Selected ?? false) == true) != null;
                bool FlagPolicyPurchase = data.ComparePromoList == null
                    ? false
                    : policyPurchase;//data.ComparePromoList.FirstOrDefault(e => e.slm_Selected.GetValueOrDefault(false) == true) != null;
                bool isActPaid = checkPaidAct(data.RenewIns.slm_TicketId);
                if (isActPaid && !FlagPolicyPurchase)
                {
                    // ถ้าไม่ซื้อประกัน ค่อยปรับสถานะ
                    if (!policyPurchase && data.RenewIns.slm_ActSendDate != null && data.RenewIns.slm_ActIncentiveDate != null)
                    {
                        var lead = slmdb.kkslm_tr_lead.FirstOrDefault(l => l.slm_ticketId == data.RenewIns.slm_TicketId
                            && (
                                (l.slm_Status == SLM.Resource.SLMConstant.StatusCode.WaitConsider && l.slm_SubStatus == SLM.Resource.SLMConstant.SubStatusCode.WaitDocAct)
                                ||
                                (l.slm_Status == SLM.Resource.SLMConstant.StatusCode.WaitConsider && l.slm_SubStatus == SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns)
                                ||
                                (l.slm_Status == SLM.Resource.SLMConstant.StatusCode.RoutebackEdit && l.slm_SubStatus == SLM.Resource.SLMConstant.SubStatusCode.ProblemPending)
                            )
                            );

                        if (lead == null)
                        {
                            actForSLA = InsertLog(data.RenewIns, data.RenewIns.slm_UpdatedBy, SLM.Resource.SLMConstant.ActionType.ChangeStatus, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocAct);

                            UpdateLeadIncentive(data.RenewIns, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocAct);
                        }
                    }
                }
                #endregion

                InsertProblemComebackReport(data.RenewIns.slm_TicketId, problemDetailIdList, userName);

            }
            catch
            {
                throw;
            }
        }

        //private void GenerateBPReport(RenewInsuranceData renew, bool PolicyPaymentMainFlag, int countPolicy, int countAct, bool savebp, int? OldActPayMethodId, int? OldPolicyPayMethodId)
        //{
        //    if (savebp)
        //    {
        //        insertkkslm_tr_bp_report(renew.slm_TicketId, renew.slm_UpdatedBy, countPolicy > 0 && renew.slm_PolicyPayMethodId == 1, countAct > 0 && renew.slm_ActPayMethodId == 1);
        //    }
        //    else
        //    {
        //        if ((countPolicy == 0 || renew.slm_PolicyPayMethodId != 1) && (countAct == 0 || renew.slm_ActPayMethodId != 1))
        //        {
        //            deletekkslm_tr_bp_report(renew.slm_TicketId, renew.slm_UpdatedBy);
        //        }
        //        else if ((countPolicy == 0 || renew.slm_PolicyPayMethodId != 1) && (countAct > 0 && OldActPayMethodId != renew.slm_ActPayMethodId && (renew.slm_ActPayMethodId == null ? 0 : renew.slm_ActPayMethodId.Value) != 1))
        //        {
        //            deletekkslm_tr_bp_report(renew.slm_TicketId, renew.slm_UpdatedBy);
        //        }
        //        else if ((countAct == 0 || renew.slm_ActPayMethodId != 1) && (countPolicy > 0 && OldPolicyPayMethodId != renew.slm_PolicyPayMethodId && (renew.slm_PolicyPayMethodId == null ? 0 : renew.slm_PolicyPayMethodId.Value) != 1))
        //        {
        //            deletekkslm_tr_bp_report(renew.slm_TicketId, renew.slm_UpdatedBy);
        //        }
        //        else if ((countPolicy > 0 && OldPolicyPayMethodId != renew.slm_PolicyPayMethodId && (renew.slm_PolicyPayMethodId == null ? 0 : renew.slm_PolicyPayMethodId.Value) != 1)
        //        && (countAct > 0 && OldActPayMethodId != renew.slm_ActPayMethodId && (renew.slm_ActPayMethodId == null ? 0 : renew.slm_ActPayMethodId.Value) != 1))
        //        {
        //            deletekkslm_tr_bp_report(renew.slm_TicketId, renew.slm_UpdatedBy);
        //        }
        //    }
        //}

        private void GenerateBPReport(RenewInsuranceData renew, bool PolicyPaymentMainFlag, int countPolicy, int countAct, BPReportData bpReport, int? OldActPayMethodId, int? OldPolicyPayMethodId)
        {
            if (bpReport.SaveBPReport)
            {
                insertkkslm_tr_bp_report(renew, bpReport, countPolicy > 0 && renew.slm_PolicyPayMethodId == 1, countAct > 0 && renew.slm_ActPayMethodId == 1);
            }
            else
            {
                if ((countPolicy == 0 || renew.slm_PolicyPayMethodId != 1) && (countAct == 0 || renew.slm_ActPayMethodId != 1))
                {
                    deletekkslm_tr_bp_report(renew.slm_TicketId, renew.slm_UpdatedBy);
                }
                else if ((countPolicy == 0 || renew.slm_PolicyPayMethodId != 1) && (countAct > 0 && OldActPayMethodId != renew.slm_ActPayMethodId && (renew.slm_ActPayMethodId == null ? 0 : renew.slm_ActPayMethodId.Value) != 1))
                {
                    deletekkslm_tr_bp_report(renew.slm_TicketId, renew.slm_UpdatedBy);
                }
                else if ((countAct == 0 || renew.slm_ActPayMethodId != 1) && (countPolicy > 0 && OldPolicyPayMethodId != renew.slm_PolicyPayMethodId && (renew.slm_PolicyPayMethodId == null ? 0 : renew.slm_PolicyPayMethodId.Value) != 1))
                {
                    deletekkslm_tr_bp_report(renew.slm_TicketId, renew.slm_UpdatedBy);
                }
                else if ((countPolicy > 0 && OldPolicyPayMethodId != renew.slm_PolicyPayMethodId && (renew.slm_PolicyPayMethodId == null ? 0 : renew.slm_PolicyPayMethodId.Value) != 1)
                && (countAct > 0 && OldActPayMethodId != renew.slm_ActPayMethodId && (renew.slm_ActPayMethodId == null ? 0 : renew.slm_ActPayMethodId.Value) != 1))
                {
                    deletekkslm_tr_bp_report(renew.slm_TicketId, renew.slm_UpdatedBy);
                }
            }
        }

        private List<decimal> GetProblemDetailIdList(string ticketId)
        {
            return slmdb.kkslm_tr_problemdetail.Where(p => p.slm_ticketId == ticketId && p.is_Deleted == false && p.slm_FixTypeFlag == "1").Select(p => p.slm_ProblemDetailId).ToList();
        }

        private void InsertProblemComebackReport(string ticketId, List<decimal> problemDetailIdList, string username)
        {
            var list = slmdb.kkslm_tr_problemdetail.Where(p => problemDetailIdList.Contains(p.slm_ProblemDetailId) == true).ToList();
            if (list.Count > 0)
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    foreach (kkslm_tr_problemdetail detail in list)
                    {
                        if (detail.slm_FixTypeFlag == "2")
                        {
                            insertkkslm_tr_problemcomeback_report(ticketId, detail.slm_ProblemDetailId.ToString(), username);
                        }
                    }

                    ts.Complete();
                }
            }
        }

        public int getProblemNotClose(string ticketId)
        {

            var sql = @"SELECT count(slm_ProblemDetailId)
                FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_problemdetail pro inner join " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com ins on  ins.is_deleted = 0 and ins.slm_Ins_Com_Id = pro.slm_Ins_Com_Id 
                WHERE pro.is_Deleted = 0 and pro.slm_FixTypeFlag = '1' AND slm_ticketId = '" + ticketId + "'";
            return slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
        }

        public void deleteRenewinsuranceCompare(decimal? id)
        {
            try
            {
                List<kkslm_tr_renewinsurance_compare> rl = slmdb.kkslm_tr_renewinsurance_compare.Where(r => r.slm_RenewInsureId == id.Value).ToList();

                foreach (kkslm_tr_renewinsurance_compare r in rl)
                {
                    slmdb.kkslm_tr_renewinsurance_compare.DeleteObject(r);
                }
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        public void SaveRenewinsuranceCompare(PreleadCompareData data)
        {
            try
            {
                kkslm_tr_renewinsurance_compare obj = new kkslm_tr_renewinsurance_compare();
                obj.slm_RenewInsureId = data.slm_RenewInsureId;
                obj.slm_NotifyPremiumId = data.slm_NotifyPremiumId;
                obj.slm_PromotionId = data.slm_PromotionId;
                obj.slm_Seq = data.slm_Seq;
                obj.slm_Year = data.slm_Year;
                obj.slm_Ins_Com_Id = data.slm_Ins_Com_Id == null ? 0 : data.slm_Ins_Com_Id.Value;
                obj.slm_CoverageTypeId = data.slm_CoverageTypeId == null ? 0 : data.slm_CoverageTypeId.Value;
                obj.slm_InjuryDeath = data.slm_InjuryDeath;
                obj.slm_TPPD = data.slm_TPPD;
                obj.slm_RepairTypeId = data.slm_RepairTypeId;
                obj.slm_OD = data.slm_OD;
                obj.slm_FT = data.slm_FT;
                obj.slm_DeDuctible = data.slm_DeDuctible;
                obj.slm_PersonalAccident = data.slm_PersonalAccident;
                obj.slm_PersonalAccidentPassenger = data.slm_PersonalAccidentPassenger;
                obj.slm_PersonalAccidentDriver = data.slm_PersonalAccidentDriver;
                obj.slm_MedicalFee = data.slm_MedicalFee;
                obj.slm_MedicalFeeDriver = data.slm_MedicalFeeDriver;
                obj.slm_MedicalFeePassenger = data.slm_MedicalFeePassenger;
                obj.slm_InsuranceDriver = data.slm_InsuranceDriver;
                obj.slm_PolicyGrossStamp = data.slm_PolicyGrossStamp;
                obj.slm_PolicyGrossVat = data.slm_PolicyGrossVat;
                obj.slm_PolicyGrossPremium = data.slm_PolicyGrossPremium;
                obj.slm_NetGrossPremium = data.slm_NetGrossPremium;
                obj.slm_PolicyGrossPremiumPay = data.slm_PolicyGrossPremiumPay;
                obj.slm_CostSave = data.slm_CostSave;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_CreatedBy = data.slm_CreatedBy;
                obj.slm_UpdatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.slm_Selected = data.slm_Selected;
                obj.slm_OldPolicyNo = data.slm_OldPolicyNo;
                obj.slm_DriverFlag = data.slm_DriverFlag;
                obj.slm_Driver_TitleId1 = data.slm_Driver_TitleId1;
                obj.slm_Driver_First_Name1 = data.slm_Driver_First_Name1;
                obj.slm_Driver_Last_Name1 = data.slm_Driver_Last_Name1;
                obj.slm_Driver_Birthdate1 = data.slm_Driver_Birthdate1;
                obj.slm_Driver_TitleId2 = data.slm_Driver_TitleId2;
                obj.slm_Driver_First_Name2 = data.slm_Driver_First_Name2;
                obj.slm_Driver_Last_Name2 = data.slm_Driver_Last_Name2;
                obj.slm_Driver_Birthdate2 = data.slm_Driver_Birthdate2;
                obj.slm_OldReceiveNo = data.slm_OldReceiveNo;
                obj.slm_PolicyStartCoverDate = data.slm_PolicyStartCoverDate;
                obj.slm_PolicyEndCoverDate = data.slm_PolicyEndCoverDate;
                obj.slm_Vat1Percent = data.slm_Vat1Percent;
                obj.slm_DiscountPercent = data.slm_DiscountPercent == null ? 0 : data.slm_DiscountPercent;
                obj.slm_DiscountBath = data.slm_DiscountBath == null ? 0 : data.slm_DiscountBath;
                obj.slm_Vat1PercentBath = data.slm_Vat1PercentBath;
                obj.slm_DeDuctibleFlag = data.slm_DeDuctibleFlag;

                slmdb.kkslm_tr_renewinsurance_compare.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }        

        public int getRenewinsuranceCompareVersion(decimal? id)
        {
            try
            {
                kkslm_tr_renewinsurance_compare_snap rl = slmdb.kkslm_tr_renewinsurance_compare_snap.Where(r => r.slm_RenewInsureId == id.Value).OrderByDescending(r => r.slm_Version).FirstOrDefault();

                if (rl != null)
                {
                    return rl.slm_Version + 1;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                throw;
            }

        }

        private void SaveRenewInsuranceSnap(RenewInsuranceData data, int lastVersion)
        {
            try
            {
                kkslm_tr_renewinsurance_snap obj = new kkslm_tr_renewinsurance_snap();
                obj.slm_RenewInsureId = data.slm_RenewInsureId;
                obj.slm_Version = lastVersion;
                obj.slm_BeneficiaryId = data.slm_BeneficiaryId;
                obj.slm_BeneficiaryName = data.slm_BeneficiaryName;
                obj.slm_CreatedBy = data.slm_UpdatedBy;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.slm_UpdatedDate = DateTime.Now;
                obj.is_Deleted = false;
                slmdb.kkslm_tr_renewinsurance_snap.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void SaveRenewinsuranceCompareSnap(PreleadCompareData data, int lastVersion)
        {

            try
            {
                kkslm_tr_renewinsurance_compare_snap obj = new kkslm_tr_renewinsurance_compare_snap();


                obj.slm_RenewInsureId = data.slm_RenewInsureId;
                obj.slm_NotifyPremiumId = data.slm_NotifyPremiumId;
                obj.slm_PromotionId = data.slm_PromotionId;
                obj.slm_Seq = data.slm_Seq;
                obj.slm_Year = data.slm_Year;
                obj.slm_Ins_Com_Id = data.slm_Ins_Com_Id == null ? 0 : data.slm_Ins_Com_Id.Value;
                obj.slm_CoverageTypeId = data.slm_CoverageTypeId == null ? 0 : data.slm_CoverageTypeId.Value;
                obj.slm_InjuryDeath = data.slm_InjuryDeath;
                obj.slm_TPPD = data.slm_TPPD;
                obj.slm_RepairTypeId = data.slm_RepairTypeId;
                obj.slm_OD = data.slm_OD;
                obj.slm_FT = data.slm_FT;
                obj.slm_DeDuctible = data.slm_DeDuctible;
                obj.slm_PersonalAccident = data.slm_PersonalAccident;
                obj.slm_PersonalAccidentPassenger = data.slm_PersonalAccidentPassenger;
                obj.slm_PersonalAccidentDriver = data.slm_PersonalAccidentDriver;
                obj.slm_MedicalFee = data.slm_MedicalFee;
                obj.slm_MedicalFeeDriver = data.slm_MedicalFeeDriver;
                obj.slm_MedicalFeePassenger = data.slm_MedicalFeePassenger;
                obj.slm_InsuranceDriver = data.slm_InsuranceDriver;
                obj.slm_PolicyGrossStamp = data.slm_PolicyGrossStamp;
                obj.slm_PolicyGrossVat = data.slm_PolicyGrossVat;
                obj.slm_PolicyGrossPremium = data.slm_PolicyGrossPremium;
                obj.slm_NetGrossPremium = data.slm_NetGrossPremium;
                obj.slm_PolicyGrossPremiumPay = data.slm_PolicyGrossPremiumPay;
                obj.slm_CostSave = data.slm_CostSave;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_CreatedBy = data.slm_CreatedBy;
                obj.slm_UpdatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.slm_Selected = data.slm_Selected; ;
                obj.slm_OldPolicyNo = data.slm_OldPolicyNo;
                obj.slm_DriverFlag = data.slm_DriverFlag;
                obj.slm_Driver_TitleId1 = data.slm_Driver_TitleId1;
                obj.slm_Driver_First_Name1 = data.slm_Driver_First_Name1;
                obj.slm_Driver_Last_Name1 = data.slm_Driver_Last_Name1;
                obj.slm_Driver_Birthdate1 = data.slm_Driver_Birthdate1;
                obj.slm_Driver_TitleId2 = data.slm_Driver_TitleId2;
                obj.slm_Driver_First_Name2 = data.slm_Driver_First_Name2;
                obj.slm_Driver_Last_Name2 = data.slm_Driver_Last_Name2;
                obj.slm_Driver_Birthdate2 = data.slm_Driver_Birthdate2;
                obj.slm_OldReceiveNo = data.slm_OldReceiveNo;
                obj.slm_PolicyStartCoverDate = data.slm_PolicyStartCoverDate;
                obj.slm_PolicyEndCoverDate = data.slm_PolicyEndCoverDate;
                obj.slm_Vat1Percent = data.slm_Vat1Percent;
                obj.slm_DiscountPercent = data.slm_DiscountPercent == null ? 0 : data.slm_DiscountPercent; ;
                obj.slm_DiscountBath = data.slm_DiscountBath == null ? 0 : data.slm_DiscountBath; ;
                obj.slm_Vat1PercentBath = data.slm_Vat1PercentBath;
                obj.slm_Version = lastVersion;
                obj.slm_DeDuctibleFlag = data.slm_DeDuctibleFlag;
                slmdb.kkslm_tr_renewinsurance_compare_snap.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        public RenewInsuranceData UpdateRenewinsurancePayment(RenewInsuranceData renew, bool PolicyPaymentMainFlag, int countPolicy, int countAct, string claimFlag, string userName)
        {
            try
            {
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_RenewInsureId == renew.slm_RenewInsureId).FirstOrDefault();
                int? slm_PayOptionIdOld = null;
                if (obj != null)
                {

                    slm_PayOptionIdOld = obj.slm_PayOptionId;

                    if ((countPolicy > 0 && renew.slm_PolicyPayMethodId != null && renew.slm_PolicyPayMethodId.Value == 3) || (countAct > 0 && renew.slm_ActPayMethodId != null && renew.slm_ActPayMethodId.Value == 3))
                    {
                        obj.slm_Need_CreditFlag = "Y";
                        renew.slm_Need_CreditFlag = "Y";
                    }
                    else
                    {
                        obj.slm_Need_CreditFlag = null;
                        renew.slm_Need_CreditFlag = null;
                    }

                    //obj.slm_Need_CreditFlag = renew.slm_Need_CreditFlag;
                    obj.slm_Need_50TawiFlag = renew.slm_Need_50TawiFlag;
                    //obj.slm_Need_DriverLicenseFlag = renew.slm_Need_DriverLicenseFlag;


                    obj.slm_PolicyPayMethodId = renew.slm_PolicyPayMethodId;
                    obj.slm_PolicyAmountPeriod = renew.slm_PolicyAmountPeriod;
                    obj.slm_PayOptionId = renew.slm_PayOptionId;
                    obj.slm_PayBranchCode = renew.slm_PayBranchCode;

                    obj.slm_ActPayMethodId = renew.slm_ActPayMethodId;
                    obj.slm_ActPayOptionId = renew.slm_ActPayOptionId;
                    obj.slm_ActPayBranchCode = renew.slm_ActPayBranchCode;

                    obj.slm_PolicyDiscountAmt = renew.slm_PolicyDiscountAmt;
                    obj.slm_DiscountPercent = renew.slm_DiscountPercent;
                    obj.slm_RemarkPayment = renew.slm_RemarkPayment;
                    obj.slm_RemarkPolicy = renew.slm_RemarkPolicy;

                    obj.slm_RemarkAct = renew.slm_RemarkAct;
                    obj.slm_RedbookBrandCode = renew.slm_RedbookBrandCode;
                    obj.slm_RedbookModelCode = renew.slm_RedbookModelCode;
                    obj.slm_RedbookYearGroup = renew.slm_RedbookYearGroup;
                    obj.slm_CC = renew.slm_CC;
                    obj.slm_InsurancecarTypeId = renew.slm_InsuranceCarTypeId;

                    obj.slm_BeneficiaryId = renew.slm_BeneficiaryId;

                    string old_receiver = string.IsNullOrWhiteSpace(obj.slm_Receiver) ? "" : obj.slm_Receiver.Trim().ToUpper();
                    string new_receiver = string.IsNullOrWhiteSpace(renew.slm_Receiver) ? "" : renew.slm_Receiver.Trim().ToUpper();
                    if (old_receiver != new_receiver)
                    {
                        KKSlmTrHistoryModel.InsertHistory(slmdb, obj.slm_TicketId, SLMConstant.HistoryTypeCode.UpdateDocReceiver, obj.slm_Receiver, renew.slm_Receiver, userName, DateTime.Now);
                    }

                    obj.slm_Receiver = renew.slm_Receiver;
                    obj.slm_SendDocBrandCode = renew.slm_SendDocBrandCode;
                    obj.slm_SendDocFlag = renew.slm_SendDocFlag;

                    obj.slm_UpdatedBy = userName;   //renew.slm_UpdatedBy;
                    obj.slm_UpdatedDate = DateTime.Now;

                    slmdb.SaveChanges();
                    // refresh renewinsurance data
                    renew = GetRenewInsurance(renew.slm_TicketId);
                }
                return renew;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckSettleClaimReported(string ticketID)
        {
            return slmdb.kkslm_tr_setle_claim_report
                    .Where(r => r.slm_TicketId == ticketID && r.slm_Export_Flag == true && r.is_Deleted == false)
                    .Count() > 0;

        }
        private void CheckClaim(RenewInsuranceData renew, string claimFlag, string userName)
        {
            var localRenew = renew;
            // check claim
            var claim = slmdb.kkslm_tr_setle_claim_report
                            .Where(r => r.slm_InsReceiveNo == localRenew.slm_ReceiveNo
                                    && (r.slm_Export_Flag == false || r.slm_Export_Flag == null)
                                    && r.is_Deleted == false
                                  ).ToList();

            var staff = slmdb.kkslm_ms_staff.FirstOrDefault(s => s.slm_UserName == userName);

            kkslm_tr_activity newLog = new kkslm_tr_activity()
            {
                slm_TicketId = renew.slm_TicketId,
                slm_CreatedDate = DateTime.Now,
                slm_UpdatedBy = staff.slm_UserName,
                slm_UpdatedDate = DateTime.Now
            };

            if (renew.slm_ClaimFlag == claimFlag
                || (renew.slm_ClaimFlag == null && claimFlag == null)
                || (renew.slm_ClaimFlag == null && claimFlag == SLMConstant.SettleClaimStatus.SettleClaim))
                // flat not change => do nothing
                return;
            else if (claimFlag == SLMConstant.SettleClaimStatus.RevertSettleClaim) // ระงับเคลม
            {
                if (claim != null)
                {

                    renew.slm_ClaimFlag = claimFlag;
                    insertkkslm_tr_setle_claim(renew.slm_TicketId, renew.slm_UpdatedBy);

                    newLog.slm_CreatedBy = staff.slm_UserName;
                    newLog.slm_CreatedBy_Position = staff.slm_Position_id;
                    newLog.slm_Type = SLMConstant.ActionType.SettleClaim; // "19"; // Settle claim
                    newLog.slm_SystemAction = "SLM";
                    newLog.slm_SystemActionBy = "SLM";
                }
            }
            else if (claimFlag == SLMConstant.SettleClaimStatus.SettleClaim && renew.slm_ClaimFlag != (string)null) // ไม่ระงับเคลม
            {
                renew.slm_ClaimFlag = (string)null;
                deletekkslm_tr_setle_claim(renew.slm_TicketId, renew.slm_UpdatedBy);

                newLog.slm_CreatedBy = userName;
                newLog.slm_CreatedBy_Position = staff == null ? 0 : staff.slm_Position_id;
                newLog.slm_Type = SLMConstant.ActionType.RevertSettleClaim; // "18"; //Revert Settle Claim
                newLog.slm_SystemAction = "SLM";
                newLog.slm_SystemActionBy = "SLM";
            }
            else if (claimFlag == SLMConstant.SettleClaimStatus.CancelSettleClaim) // ยกเลิกระงับเคลม
            {
                renew.slm_ClaimFlag = claimFlag;
                // deletekkslm_tr_setle_claim(renew.slm_TicketId, renew.slm_UpdatedBy);
                deletekkslm_tr_policynew_report(renew.slm_TicketId, renew.slm_UpdatedBy);
                insertkkslm_tr_setle_claim_cancel(renew.slm_TicketId, renew.slm_UpdatedBy);

                newLog.slm_CreatedBy = userName;
                newLog.slm_CreatedBy_Position = staff == null ? 0 : staff.slm_Position_id;
                newLog.slm_Type = SLMConstant.ActionType.CancelSettleClaim;
                newLog.slm_SystemAction = "SLM";
                newLog.slm_SystemActionBy = "SLM";
            }

            slmdb.kkslm_tr_activity.AddObject(newLog);

        }

        public void UpdateRenewinsurance(RenewInsuranceData renew, PreleadCompareData data, bool PolicyPaymentMainFlag)
        {
            //update
            try
            {
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_RenewInsureId == renew.slm_RenewInsureId).FirstOrDefault();

                if (obj != null)
                {
                    if (!PolicyPaymentMainFlag)
                    {
                        obj.slm_PolicyDiscountAmt = data.slm_DiscountBath == null ? 0 : data.slm_DiscountBath;
                        obj.slm_DiscountPercent = data.slm_DiscountPercent == null ? 0 : data.slm_DiscountPercent;

                        obj.slm_PolicyNetGrossPremium = data.slm_PolicyGrossPremium;
                        obj.slm_PolicyGrossPremiumTotal = data.slm_NetGrossPremium;
                        obj.slm_PolicyGrossPremium = data.slm_PolicyGrossPremiumPay;
                    }

                    obj.slm_RedbookBrandCode = renew.slm_RedbookBrandCode;
                    obj.slm_RedbookModelCode = renew.slm_RedbookModelCode;
                    obj.slm_CC = renew.slm_CC;
                    obj.slm_CoverageTypeId = data.slm_CoverageTypeId;
                    obj.slm_InsuranceComId = data.slm_Ins_Com_Id;
                    obj.slm_PolicyGrossVat = data.slm_PolicyGrossVat;
                    obj.slm_PolicyGrossStamp = data.slm_PolicyGrossStamp;
                    obj.slm_PolicyGrossPremium = data.slm_PolicyGrossPremiumPay;

                    obj.slm_PolicyCost = data.slm_OD;
                    obj.slm_RepairTypeId = data.slm_RepairTypeId;

                    obj.slm_RemarkPayment = renew.slm_RemarkPayment;
                    obj.slm_PolicyCostSave = data.slm_CostSave;
                    obj.slm_Vat1Percent = data.slm_Vat1Percent;

                    obj.slm_Need_DriverLicenseFlag = data.slm_DriverFlag == "1" ? "Y" : null;

                    obj.slm_PolicyStartCoverDate = data.slm_PolicyStartCoverDate;
                    obj.slm_PolicyEndCoverDate = data.slm_PolicyEndCoverDate;

                    obj.slm_Vat1PercentBath = data.slm_Vat1PercentBath;

                    obj.slm_UpdatedBy = renew.slm_UpdatedBy;
                    obj.slm_UpdatedDate = DateTime.Now;

                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteRenewinsurancePaymentmain(decimal? id)
        {
            try
            {
                List<kkslm_tr_renewinsurance_paymentmain> pmList = slmdb.kkslm_tr_renewinsurance_paymentmain.Where(p => p.slm_RenewInsureId == id).ToList();

                if (pmList != null && pmList.Count > 0)
                {
                    foreach (kkslm_tr_renewinsurance_paymentmain pm in pmList)
                    {
                        slmdb.kkslm_tr_renewinsurance_paymentmain.DeleteObject(pm);
                    }
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        public void SaveRenewinsurancePaymentmain(RenewInsurancePaymentMainData data)
        {
            try
            {
                kkslm_tr_renewinsurance_paymentmain obj = new kkslm_tr_renewinsurance_paymentmain();
                obj.slm_RenewInsureId = data.slm_RenewInsureId;
                obj.slm_Seq = data.slm_Seq;
                obj.slm_Type = data.slm_Type;
                obj.slm_PaymentDate = data.slm_PaymentDate;
                obj.slm_Period = data.slm_Period;
                obj.slm_PaymentAmount = data.slm_PaymentAmount;
                obj.slm_CreatedBy = data.slm_CreatedBy;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.slm_UpdatedDate = DateTime.Now;


                slmdb.kkslm_tr_renewinsurance_paymentmain.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }
        public void UpdateProductInfo(RenewInsuranceData data)
        {
            try
            {
                ///update
                kkslm_tr_productinfo obj = slmdb.kkslm_tr_productinfo.Where(p => p.slm_TicketId == data.slm_TicketId).FirstOrDefault();

                if (obj != null)
                {
                    obj.slm_RedbookBrandCode = data.slm_RedbookBrandCode;
                    obj.slm_RedbookModelCode = data.slm_RedbookModelCode;
                }
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        //บันทึกผลการติดต่อ
        public void SavePhoneCall(kkslm_phone_call data)
        {
            try
            {
                kkslm_phone_call obj = new kkslm_phone_call();
                obj.slm_PhoneCallId = data.slm_PhoneCallId;
                obj.slm_CreateBy = data.slm_CreateBy;
                obj.slm_CreateDate = data.slm_CreateDate;
                obj.is_Deleted = data.is_Deleted;
                obj.slm_Owner_Position = data.slm_Owner_Position;
                obj.slm_CreatedBy_Position = data.slm_CreatedBy_Position;
                obj.slm_CAS_Flag = data.slm_CAS_Flag;
                obj.slm_SubStatusName = data.slm_SubStatusName;
                obj.slm_Need_CompulsoryFlag = data.slm_Need_CompulsoryFlag;
                obj.slm_Need_CompulsoryFlagDate = data.slm_Need_CompulsoryFlagDate;
                obj.slm_Need_PolicyFlag = data.slm_Need_PolicyFlag;
                obj.slm_Need_PolicyFlagDate = data.slm_Need_PolicyFlagDate;

                slmdb.kkslm_phone_call.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        public void SaveBlacklist(ConfigProductBlacklistData data)
        {
            try
            {
                kkslm_ms_config_product_blacklist obj = new kkslm_ms_config_product_blacklist();
                obj.slm_cp_blacklist_id = data.slm_cp_blacklist_id == null ? 0 : data.slm_cp_blacklist_id.Value;
                obj.slm_Product_Id = data.slm_Product_Id;
                obj.slm_Prelead_Id = data.slm_Prelead_Id;
                obj.slm_ticketId = data.slm_ticketId;
                obj.slm_Name = data.slm_Name;
                obj.slm_LastName = data.slm_LastName;
                obj.slm_CardType = data.slm_CardType;
                obj.slm_CitizenId = data.slm_CitizenId;
                obj.slm_StartDate = data.slm_StartDate;
                obj.slm_EndDate = data.slm_EndDate;
                obj.slm_IsActive = data.slm_IsActive;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_CreatedBy = data.slm_CreatedBy;
                obj.slm_UpdatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.is_Deleted = false;

                slmdb.kkslm_ms_config_product_blacklist.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }
        public void UpdeatLead(LeadData data)
        {
            try
            {
                kkslm_tr_lead obj = slmdb.kkslm_tr_lead.Where(l => l.slm_ticketId == data.slm_TicketId).FirstOrDefault();

                if (obj != null)
                {
                    obj.slm_ExternalStatus = data.slm_ExternalStatus;
                    obj.slm_ExternalSubStatus = data.slm_ExternalSubStatus;
                    obj.slm_ExternalSubStatusDesc = data.slm_ExternalSubStatusDesc;
                    obj.slm_ExternalSystem = data.slm_ExternalSystem;
                    obj.slm_UpdatedBy = data.slm_UpdatedBy;
                    obj.slm_UpdatedDate = DateTime.Now;
                }
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        //1.1.3.33.	การบันทึกผลการติดต่อของการกดปุ่มเลขรับแจ้ง, ส่งแจ้งพรบ., Incentive ประกัน และ Incentive พรบ.
        public void insertPhoneCall(PreleadCompareDataCollection data, kkslm_tr_activity act)
        {
            try
            {
                DateTime createdDate = DateTime.Now;

                data.RenewIns.slm_UpdatedDate = createdDate;    //ใส่เพื่อส่งเวลาออกไป Update บนหน้าจอด้านนอก
                kkslm_phone_call phone = new kkslm_phone_call()
                {
                    is_Deleted = 0m,
                    slm_CreateBy = act.slm_CreatedBy,
                    slm_CreateDate = createdDate,
                    slm_CreatedBy_Position = act.slm_CreatedBy_Position,
                    slm_Owner = data.lead.slm_Owner,
                    slm_Owner_Position = GetPositionId(data.lead.slm_Owner),
                    slm_Status = act.slm_NewStatus,
                    slm_SubStatus = act.slm_NewSubStatus,
                    slm_SubStatusName = act.slm_ExternalSubStatus_New,
                    slm_TicketId = data.RenewIns.slm_TicketId,
                    slm_ContactDetail = GetPhonecallLogContent(data)
                };

                if (phone.slm_SubStatusName == null || phone.slm_Status == null)
                {
                    var lead = slmdb.kkslm_tr_lead.FirstOrDefault(l => l.slm_ticketId == data.lead.TicketId);
                    if (lead != null)
                    {
                        phone.slm_Status = phone.slm_Status ?? lead.slm_Status;
                        phone.slm_SubStatus = phone.slm_SubStatus ?? lead.slm_SubStatus;
                        phone.slm_SubStatusName = phone.slm_SubStatusName ?? lead.slm_ExternalSubStatusDesc;
                    }
                }
                slmdb.kkslm_phone_call.AddObject(phone);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        //public void UpdateRenewinsurance(RenewInsuranceData data)
        //{
        //    try
        //    {
        //        kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(l => l.slm_TicketId == data.slm_TicketId).FirstOrDefault();

        //        if (obj != null)
        //        {
        //            obj.slm_Need_CompulsoryFlag = data.slm_Need_CompulsoryFlag;
        //            obj.slm_Need_CompulsoryFlagDate = data.slm_Need_CompulsoryFlagDate;
        //            obj.slm_Need_PolicyFlag = data.slm_Need_PolicyFlag;
        //            obj.slm_Need_PolicyFlagDate = data.slm_Need_PolicyFlagDate;
        //            obj.slm_Need_CreditFlag = data.slm_Need_CreditFlag;
        //            obj.slm_Need_50TawiFlag = data.slm_Need_50TawiFlag;
        //            obj.slm_Need_DriverLicenseFlag = data.slm_Need_DriverLicenseFlag;
        //            obj.slm_UpdatedBy = data.slm_UpdatedBy;
        //            obj.slm_UpdatedDate = DateTime.Now;
        //        }
        //        slmdb.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        //Act
        public void deleteRenewinsuranceCompareAct(decimal? id)
        {
            try
            {
                List<kkslm_tr_renewinsurance_compare_act> rl = slmdb.kkslm_tr_renewinsurance_compare_act.Where(r => r.slm_RenewInsureId == id.Value).ToList();

                foreach (kkslm_tr_renewinsurance_compare_act r in rl)
                {
                    slmdb.kkslm_tr_renewinsurance_compare_act.DeleteObject(r);
                }
            }
            catch
            {
                throw;
            }

        }

        public void SaveRenewinsuranceCompareAct(PreleadCompareActData data)
        {
            try
            {
                kkslm_tr_renewinsurance_compare_act obj = new kkslm_tr_renewinsurance_compare_act();
                obj.slm_RenewInsureId = data.slm_RenewInsureId;
                obj.slm_PromotionId = data.slm_PromotionId;
                obj.slm_Seq = data.slm_Seq;
                obj.slm_Year = data.slm_Year;
                obj.slm_Ins_Com_Id = data.slm_Ins_Com_Id == null ? 0 : data.slm_Ins_Com_Id.Value;
                obj.slm_ActIssuePlace = data.slm_ActIssuePlace;
                obj.slm_SendDocType = data.slm_SendDocType;
                obj.slm_ActStartCoverDate = data.slm_ActStartCoverDate;
                obj.slm_ActEndCoverDate = data.slm_ActEndCoverDate;
                obj.slm_ActIssueBranch = data.slm_ActIssueBranch;
                obj.slm_CarTaxExpiredDate = data.slm_CarTaxExpiredDate;
                obj.slm_ActGrossStamp = data.slm_ActGrossStamp;
                obj.slm_ActGrossVat = data.slm_ActGrossVat;
                obj.slm_ActGrossPremium = data.slm_ActGrossPremium == null ? 0 : data.slm_ActGrossPremium.Value;
                obj.slm_ActNetGrossPremium = data.slm_ActNetGrossPremium;
                obj.slm_ActGrossPremiumPay = data.slm_ActGrossPremiumPay;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_CreatedBy = data.slm_CreatedBy;
                obj.slm_UpdatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.slm_ActPurchaseFlag = data.slm_ActPurchaseFlag;
                obj.slm_DiscountPercent = data.slm_DiscountPercent == null ? 0 : data.slm_DiscountPercent;
                obj.slm_DiscountBath = data.slm_DiscountBath == null ? 0 : data.slm_DiscountBath;
                obj.slm_ActSignNo = data.slm_ActSignNo;
                obj.slm_ActNetGrossPremiumFull = data.slm_ActNetGrossPremiumFull;
                obj.slm_Vat1Percent = data.slm_Vat1Percent;
                obj.slm_Vat1PercentBath = data.slm_Vat1PercentBath;


                slmdb.kkslm_tr_renewinsurance_compare_act.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        public int getRenewinsuranceCompareActVersion(decimal? id)
        {
            try
            {
                kkslm_tr_renewinsurance_compare_act_snap rl = slmdb.kkslm_tr_renewinsurance_compare_act_snap.Where(r => r.slm_RenewInsureId == id.Value).OrderByDescending(r => r.slm_Version).FirstOrDefault();

                if (rl != null)
                {
                    return rl.slm_Version + 1;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                throw;
            }

        }

        public void SaveRenewinsuranceCompareActSnap(PreleadCompareActData data, int lastVersion)
        {
            try
            {
                kkslm_tr_renewinsurance_compare_act_snap obj = new kkslm_tr_renewinsurance_compare_act_snap();
                obj.slm_RenewInsureId = data.slm_RenewInsureId;
                obj.slm_PromotionId = data.slm_PromotionId;
                obj.slm_Seq = data.slm_Seq;
                obj.slm_Year = data.slm_Year;
                obj.slm_Ins_Com_Id = data.slm_Ins_Com_Id == null ? 0 : data.slm_Ins_Com_Id.Value;
                obj.slm_ActIssuePlace = data.slm_ActIssuePlace;
                obj.slm_SendDocType = data.slm_SendDocType;
                obj.slm_ActStartCoverDate = data.slm_ActStartCoverDate;
                obj.slm_ActEndCoverDate = data.slm_ActEndCoverDate;
                obj.slm_ActIssueBranch = data.slm_ActIssueBranch;
                obj.slm_CarTaxExpiredDate = data.slm_CarTaxExpiredDate;
                obj.slm_ActGrossStamp = data.slm_ActGrossStamp;
                obj.slm_ActGrossVat = data.slm_ActGrossVat;
                obj.slm_ActGrossPremium = data.slm_ActGrossPremium == null ? 0 : data.slm_ActGrossPremium.Value;
                obj.slm_ActNetGrossPremium = data.slm_ActNetGrossPremium;
                obj.slm_ActGrossPremiumPay = data.slm_ActGrossPremiumPay;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_CreatedBy = data.slm_CreatedBy;
                obj.slm_UpdatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.slm_ActPurchaseFlag = data.slm_ActPurchaseFlag;
                obj.slm_DiscountPercent = data.slm_DiscountPercent == null ? 0 : data.slm_DiscountPercent;
                obj.slm_DiscountBath = data.slm_DiscountBath == null ? 0 : data.slm_DiscountBath;
                obj.slm_ActSignNo = data.slm_ActSignNo;
                obj.slm_ActNetGrossPremiumFull = data.slm_ActNetGrossPremiumFull;
                obj.slm_Vat1Percent = data.slm_Vat1Percent;
                obj.slm_Vat1PercentBath = data.slm_Vat1PercentBath;
                obj.slm_Version = lastVersion;

                slmdb.kkslm_tr_renewinsurance_compare_act_snap.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }
        public void UpdateRenewinsuranceAct(RenewInsuranceData data, PreleadCompareActData compareAct, bool PolicyPaymentMainFlag)
        {
            try
            {
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == data.slm_TicketId).FirstOrDefault();

                if (obj != null)
                {
                    if (!PolicyPaymentMainFlag)
                    {
                        obj.slm_ActPayMethodId = data.slm_ActPayMethodId;
                        obj.slm_ActPayOptionId = data.slm_ActPayOptionId;
                        obj.slm_ActPayBranchCode = data.slm_ActPayBranchCode;

                        obj.slm_ActDiscountAmt = compareAct.slm_DiscountBath == null ? 0 : compareAct.slm_DiscountBath;
                        obj.slm_ActDiscountPercent = compareAct.slm_DiscountPercent == null ? 0 : compareAct.slm_DiscountPercent;
                    };
                    obj.slm_ActPurchaseFlag = compareAct.slm_ActPurchaseFlag;
                    obj.slm_ActStartCoverDate = compareAct.slm_ActStartCoverDate;
                    obj.slm_ActEndCoverDate = compareAct.slm_ActEndCoverDate;
                    obj.slm_ActIssuePlace = compareAct.slm_ActIssuePlace;
                    obj.slm_ActIssueBranch = compareAct.slm_ActIssueBranch;
                    obj.slm_ActGrossPremium = compareAct.slm_ActGrossPremiumPay;
                    obj.slm_ActComId = compareAct.slm_Ins_Com_Id;
                    obj.slm_ActNetPremium = compareAct.slm_ActGrossPremium;
                    obj.slm_ActVat = compareAct.slm_ActGrossVat;
                    obj.slm_ActStamp = compareAct.slm_ActGrossStamp;

                    obj.slm_ActVat1Percent = compareAct.slm_Vat1Percent;
                    obj.slm_ActVat1PercentBath = compareAct.slm_Vat1PercentBath;
                    //obj.slm_ActDiscountAmt = compareAct.slm_DiscountBath == null ? 0 : compareAct.slm_DiscountBath;
                    //obj.slm_ActDiscountPercent = compareAct.slm_DiscountPercent == null ? 0 : compareAct.slm_DiscountPercent;


                    obj.slm_ActamountPeriod = 0;//data.slm_ActamountPeriod;

                    obj.slm_UpdatedBy = data.slm_UpdatedBy;
                    obj.slm_UpdatedDate = DateTime.Now;
                }
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        public void SaveRenewinsurancePaymentmainAct(RenewInsurancePaymentMainData data)
        {
            try
            {
                kkslm_tr_renewinsurance_paymentmain obj = new kkslm_tr_renewinsurance_paymentmain();
                obj.slm_RenewInsureId = data.slm_RenewInsureId;
                obj.slm_Seq = data.slm_Seq;
                obj.slm_Type = "2";
                obj.slm_PaymentDate = data.slm_PaymentDate;
                obj.slm_Period = data.slm_Period;
                obj.slm_PaymentAmount = data.slm_PaymentAmount;
                obj.slm_CreatedBy = data.slm_CreatedBy;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.slm_UpdatedDate = DateTime.Now;

                slmdb.kkslm_tr_renewinsurance_paymentmain.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void SaveRenewinsuranceAddress(PreleadCompareDataCollection data)
        {
            try
            {
                kkslm_tr_renewinsurance renew = slmdb.kkslm_tr_renewinsurance.Where(a => a.slm_RenewInsureId == data.RenewIns.slm_RenewInsureId).FirstOrDefault();

                renew.slm_SendDocFlag = data.RenewIns.slm_SendDocFlag;
                renew.slm_SendDocBrandCode = data.RenewIns.slm_SendDocBrandCode;
                renew.slm_Receiver = data.RenewIns.slm_Receiver;
                kkslm_tr_renewinsurance_address renewAddress = slmdb.kkslm_tr_renewinsurance_address.Where(a => a.slm_RenewInsureId == data.RenewIns.slm_RenewInsureId && a.slm_AddressType == "D").FirstOrDefault();

                if (renewAddress == null)
                {
                    renewAddress = new kkslm_tr_renewinsurance_address();

                    renewAddress.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;//ได้มาจาก preleadId
                    //renewAddress.slm_CmtLot = data.Prelead.slm_CmtLot;
                    //renewAddress.slm_Customer_Key = data.Prelead.slm_Customer_Key;
                    renewAddress.slm_AddressType = "D";
                    renewAddress.slm_House_No = data.Address.slm_House_No;
                    renewAddress.slm_Moo = data.Address.slm_Moo;
                    renewAddress.slm_BuildingName = data.Address.slm_Building;
                    renewAddress.slm_Village = data.Address.slm_Village;
                    renewAddress.slm_Soi = data.Address.slm_Soi;
                    renewAddress.slm_Street = data.Address.slm_Street;
                    renewAddress.slm_Tambon = data.Address.slm_TambolId;
                    renewAddress.slm_Amphur = data.Address.slm_Amphur_Id;
                    renewAddress.slm_Province = data.Address.slm_Province_Id;
                    renewAddress.slm_PostalCode = data.Address.slm_Zipcode;


                    slmdb.kkslm_tr_renewinsurance_address.AddObject(renewAddress);
                }
                else
                {
                    renewAddress.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;//ได้มาจาก preleadId
                    //renewAddress.slm_CmtLot = data.Address.slm_CmtLot;
                    //renewAddress.slm_Customer_Key = data.Address.slm_Customer_Key;
                    renewAddress.slm_AddressType = "D";
                    renewAddress.slm_House_No = data.Address.slm_House_No;
                    renewAddress.slm_Moo = data.Address.slm_Moo;
                    renewAddress.slm_BuildingName = data.Address.slm_Building;
                    renewAddress.slm_Village = data.Address.slm_Village;
                    renewAddress.slm_Soi = data.Address.slm_Soi;
                    renewAddress.slm_Street = data.Address.slm_Street;
                    renewAddress.slm_Tambon = data.Address.slm_TambolId;
                    renewAddress.slm_Amphur = data.Address.slm_Amphur_Id;
                    renewAddress.slm_Province = data.Address.slm_Province_Id;
                    renewAddress.slm_PostalCode = data.Address.slm_Zipcode;
                }


                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //ข้อมูลรับชำระเงิน
        public void UpdateRenewinsurancePay(RenewInsuranceData data)
        {
            try
            {
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == data.slm_TicketId).FirstOrDefault();

                if (obj != null)
                {
                    obj.slm_PolicyDiscountAmt = data.slm_PolicyDiscountAmt;
                    obj.slm_DiscountPercent = data.slm_DiscountPercent;
                    obj.slm_PolicyGrossPremium = data.slm_PolicyGrossPremium;

                    obj.slm_ActDiscountAmt = data.slm_ActDiscountAmt;
                    obj.slm_ActDiscountPercent = data.slm_ActDiscountPercent;
                    obj.slm_ActGrossPremium = data.slm_ActGrossPremium;


                    obj.slm_UpdatedBy = data.slm_UpdatedBy;
                    obj.slm_UpdatedDate = DateTime.Now;
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }
        public void UpdateRenewinsuranceReceipt(RenewInsuranceReceiptData data)
        {
            try
            {
                kkslm_tr_renewinsurance_receipt obj = slmdb.kkslm_tr_renewinsurance_receipt.Where(r => r.slm_RenewInsuranceReceiptId == data.slm_RenewInsuranceReceiptId).FirstOrDefault();

                if (obj != null)
                {
                    obj.slm_Status = data.slm_Status;
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }
        public void UpdateRenewinsuranceReceiptDetail(RenewInsuranceReceiptData data)
        {
            try
            {
                List<kkslm_tr_renewinsurance_receipt_detail> objlist = slmdb.kkslm_tr_renewinsurance_receipt_detail.Where(r => r.slm_RenewInsuranceReceiptId == data.slm_RenewInsuranceReceiptId).ToList();

                if (objlist != null && objlist.Count > 0)
                {
                    foreach (kkslm_tr_renewinsurance_receipt_detail obj in objlist)
                    {
                        obj.slm_Status = data.slm_Status;
                    }
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }
        public void DeleteRenewinsuranceReceiptRevisionDetail(string slm_TicketId)
        {

            try
            {
                var sql = @"delete from kkslm_tr_renewinsurance_receipt_revision_detail
								 where 	slm_renewinsurancereceiptid in 
										 (select slm_renewinsurancereceiptid from
										 kkslm_tr_renewinsurance_receipt
										  where slm_ticketid ='" + slm_TicketId + "')";

                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }
        }

        public void SaveRenewinsuranceReceiptRevisionDetail(RenewInsuranceReceiptRevisionDetailData data)
        {

            try
            {
                kkslm_tr_renewinsurance_receipt_revision_detail obj = new kkslm_tr_renewinsurance_receipt_revision_detail();

                kkslm_tr_renewinsurance_receipt_revision_detail old = slmdb.kkslm_tr_renewinsurance_receipt_revision_detail.Where(r => r.slm_RenewInsuranceReceiptId == data.slm_RenewInsuranceReceiptId && r.slm_PaymentCode == data.slm_PaymentCode).FirstOrDefault();

                if (old != null)
                {
                    if (data.delflag == "D")
                    {
                        slmdb.kkslm_tr_renewinsurance_receipt_revision_detail.DeleteObject(old);
                    }
                    else
                    {
                        old.slm_RenewInsuranceReceiptId = data.slm_RenewInsuranceReceiptId;
                        old.slm_PaymentCode = data.slm_PaymentCode;
                        old.slm_PaymentOtherDesc = data.slm_PaymentOtherDesc;
                        old.slm_InsNoDesc = data.slm_InsNoDesc;
                        old.slm_InstNo = data.slm_InstNo;
                        old.slm_RecBy = data.slm_RecBy;
                        old.slm_RecNo = data.slm_RecNo;
                        old.slm_RecAmount = data.slm_RecAmount;
                        old.slm_TransDate = data.slm_TransDate;
                        old.slm_Status = data.slm_Status;
                        old.slm_Selected = data.slm_Selected;
                        old.slm_Seq = data.slm_Seq;
                        old.slm_CreatedDate = DateTime.Now;
                        old.slm_CreatedBy = data.slm_CreatedBy;
                        old.slm_UpdatedDate = DateTime.Now;
                        old.slm_UpdatedBy = data.slm_UpdatedBy;
                        old.is_Deleted = false;
                    }
                }
                else
                {
                    if (data.delflag != "D")
                    {
                        obj.slm_RenewInsuranceReceiptId = data.slm_RenewInsuranceReceiptId;
                        obj.slm_PaymentCode = data.slm_PaymentCode;
                        obj.slm_PaymentOtherDesc = data.slm_PaymentOtherDesc;
                        obj.slm_InsNoDesc = data.slm_InsNoDesc;
                        obj.slm_InstNo = data.slm_InstNo;
                        obj.slm_RecBy = data.slm_RecBy;
                        obj.slm_RecNo = data.slm_RecNo;
                        obj.slm_RecAmount = data.slm_RecAmount;
                        obj.slm_TransDate = data.slm_TransDate;
                        obj.slm_Status = data.slm_Status;
                        obj.slm_Selected = data.slm_Selected;
                        obj.slm_Seq = data.slm_Seq;
                        obj.slm_CreatedDate = DateTime.Now;
                        obj.slm_CreatedBy = data.slm_CreatedBy;
                        obj.slm_UpdatedDate = DateTime.Now;
                        obj.slm_UpdatedBy = data.slm_UpdatedBy;
                        obj.is_Deleted = false;
                        slmdb.kkslm_tr_renewinsurance_receipt_revision_detail.AddObject(obj);
                    }


                }
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        public void DeleteRenewinsurancePaymenttrans(decimal? id)
        {
            try
            {
                List<kkslm_tr_renewinsurance_paymenttrans> ptList = slmdb.kkslm_tr_renewinsurance_paymenttrans.Where(p => p.slm_RenewInsureId == id).ToList();

                if (ptList != null && ptList.Count > 0)
                {
                    foreach (kkslm_tr_renewinsurance_paymenttrans pt in ptList)
                    {
                        slmdb.kkslm_tr_renewinsurance_paymenttrans.DeleteObject(pt);
                    }
                }
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        public void SaveRenewinsurancePaymenttrans(PaymentTransData data)
        {
            try
            {
                kkslm_tr_renewinsurance_paymenttrans obj = new kkslm_tr_renewinsurance_paymenttrans();
                obj.slm_RenewInsureId = data.slm_RenewInsureId;
                obj.slm_Seq = data.slm_Seq;
                obj.slm_Type = data.slm_Type;
                obj.slm_PaymentDate = data.slm_PaymentDate;
                obj.slm_Period = data.slm_Period;
                obj.slm_PaymentAmount = data.slm_PaymentAmount;
                obj.slm_CreatedBy = data.slm_CreatedBy;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.slm_UpdatedDate = DateTime.Now;

                slmdb.kkslm_tr_renewinsurance_paymenttrans.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }
        public void DeleteRenewinsurancePaymenttransMain(decimal? id)
        {
            try
            {
                List<kkslm_tr_renewinsurance_paymenttransmain> ptmList = slmdb.kkslm_tr_renewinsurance_paymenttransmain.Where(p => p.slm_RenewInsureId == id).ToList();

                if (ptmList != null && ptmList.Count > 0)
                {
                    foreach (kkslm_tr_renewinsurance_paymenttransmain ptm in ptmList)
                    {
                        slmdb.kkslm_tr_renewinsurance_paymenttransmain.DeleteObject(ptm);
                    }
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void SaveRenewinsurancePaymenttransMain(PaymentTransMainData data)
        {
            try
            {
                kkslm_tr_renewinsurance_paymenttransmain obj = new kkslm_tr_renewinsurance_paymenttransmain();
                obj.slm_RenewInsureId = data.slm_RenewInsureId;
                obj.slm_PayOptionId = data.slm_PayOptionId;
                obj.slm_PolicyAmountPeriod = data.slm_PolicyAmountPeriod;
                obj.slm_PayBranchCode = data.slm_PayBranchCode;
                obj.slm_PolicyPayMethodId = data.slm_PolicyPayMethodId;
                obj.slm_ActPayMethodId = data.slm_ActPayMethodId;
                obj.slm_ActamountPeriod = data.slm_ActamountPeriod;
                obj.slm_ActPayOptionId = data.slm_ActPayOptionId;
                obj.slm_ActPayBranchCode = data.slm_ActPayBranchCode;
                obj.slm_CreatedBy = data.slm_CreatedBy;
                obj.slm_CreatedDate = DateTime.Now;
                obj.slm_UpdatedBy = data.slm_UpdatedBy;
                obj.slm_UpdatedDate = DateTime.Now;

                slmdb.kkslm_tr_renewinsurance_paymenttransmain.AddObject(obj);
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Incentive

        private string GetSubStatusName(string campaignId, string productId, string statusCode, string substatusCode)
        {
            try
            {
                string substatusName = "";
                substatusName = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_CampaignId == campaignId && p.slm_Product_Id == null
                                    && p.slm_OptionCode == statusCode && p.slm_SubStatusCode == substatusCode).Select(p => p.slm_SubStatusName).FirstOrDefault();

                if (string.IsNullOrEmpty(substatusName))
                {
                    substatusName = slmdb.kkslm_ms_config_product_substatus.Where(p => p.is_Deleted == false && p.slm_Product_Id == productId && p.slm_CampaignId == null
                                    && p.slm_OptionCode == statusCode && p.slm_SubStatusCode == substatusCode).Select(p => p.slm_SubStatusName).FirstOrDefault();
                }

                return substatusName;
            }
            catch
            {
                throw;
            }
        }

        public int? GetPositionId(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetPositionId(username);
        }

        public void UpdateRenewinsuranceReceivePolicy(RenewInsuranceData data)
        {
            try
            {
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == data.slm_TicketId).FirstOrDefault();

                if (obj != null)
                {
                    //string sql = @"SELECT COUNT(compare.slm_RenewInsureCompareActId)
                    //                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance renew
                    //                INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare_act compare ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
                    //                WHERE renew.slm_TicketId = '" + data.slm_TicketId + "' AND compare.slm_ActPurchaseFlag = 1 ";

                    //int countAct = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                    obj.slm_ReceiveNo = data.slm_ReceiveNo;
                    obj.slm_ReceiveDate = data.slm_ReceiveDate;
                    //if (countAct > 0)
                    //    obj.slm_ActSendDate = data.slm_ReceiveDate;

                    obj.slm_UpdatedBy = data.slm_UpdatedBy;
                    obj.slm_UpdatedDate = DateTime.Now;
                    obj.slm_ClaimFlag = data.slm_ClaimFlag;
                    obj.slm_GenSMSReceiveNo = null;
                    obj.slm_GenSMSReceiveNoDate = null;
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateRenewinsuranceReceiveAct(RenewInsuranceData data)
        {
            try
            {
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == data.slm_TicketId).FirstOrDefault();

                if (obj != null)
                {
                    obj.slm_ActSendDate = data.slm_ActSendDate;

                    obj.slm_UpdatedBy = data.slm_UpdatedBy;
                    obj.slm_UpdatedDate = DateTime.Now;
                    obj.slm_ClaimFlag = data.slm_ClaimFlag;
                }
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateRenewinsurancePolicyIncentive(RenewInsuranceData data)
        {
            try
            {
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == data.slm_TicketId).FirstOrDefault();

                if (obj != null)
                {

                    obj.slm_IncentiveFlag = data.slm_IncentiveFlag;
                    obj.slm_IncentiveDate = data.slm_IncentiveDate;
                    obj.slm_PolicyComBath = data.slm_PolicyComBath;
                    obj.slm_PolicyComBathVat = data.slm_PolicyComBathVat;
                    obj.slm_PolicyComBathIncentive = data.slm_PolicyComBathIncentive;
                    obj.slm_PolicyOV1Bath = data.slm_PolicyOV1Bath;
                    obj.slm_PolicyOV1BathVat = data.slm_PolicyOV1BathVat;
                    obj.slm_PolicyOV1BathIncentive = data.slm_PolicyOV1BathIncentive;
                    obj.slm_PolicyOV2Bath = data.slm_PolicyOV2Bath;
                    obj.slm_PolicyOV2BathVat = data.slm_PolicyOV2BathVat;
                    obj.slm_PolicyOV2BathIncentive = data.slm_PolicyOV2BathIncentive;
                    obj.slm_PolicyIncentiveAmount = data.slm_PolicyIncentiveAmount;

                }
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateRenewinsuranceActIncentive(RenewInsuranceData data)
        {
            try
            {
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == data.slm_TicketId).FirstOrDefault();

                if (obj != null)
                {
                    obj.slm_ActIncentiveFlag = data.slm_ActIncentiveFlag;
                    obj.slm_ActIncentiveDate = data.slm_ActIncentiveDate;
                    obj.slm_ActComBath = data.slm_ActComBath;
                    obj.slm_ActComBathVat = data.slm_ActComBathVat;
                    obj.slm_ActComBathIncentive = data.slm_ActComBathIncentive;
                    obj.slm_ActOV1Bath = data.slm_ActOV1Bath;
                    obj.slm_ActOV1BathVat = data.slm_ActOV1BathVat;
                    obj.slm_ActOV1BathIncentive = data.slm_ActOV1BathIncentive;
                    obj.slm_ActOV2Bath = data.slm_ActOV2Bath;
                    obj.slm_ActOV2BathVat = data.slm_ActOV2BathVat;
                    obj.slm_ActOV2BathIncentive = data.slm_ActOV2BathIncentive;
                    obj.slm_ActIncentiveAmount = data.slm_ActIncentiveAmount;
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateReceivenolist(decimal? ReceiveNoListId, string TicketId, string username)//, decimal? companyId, string productId)
        {
            try
            {
                kkslm_ms_receivenolist obj = operdb.kkslm_ms_receivenolist.Where(r => r.slm_ReceiveNoListId == ReceiveNoListId).FirstOrDefault();

                if (obj != null)
                {
                    obj.slm_TicketId = TicketId;
                    obj.slm_UseDate = DateTime.Now;
                    obj.slm_UseBy = username;
                    obj.slm_Status = "1";
                    obj.slm_UpdatedBy = username;
                    obj.slm_UpdatedDate = DateTime.Now;
                }

                operdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }
        public void UpdateLeadIncentive(RenewInsuranceData data, string OptionCode, string SubStatusCode)
        {
            try
            {
                kkslm_tr_lead obj = slmdb.kkslm_tr_lead.Where(l => l.slm_ticketId == data.slm_TicketId).FirstOrDefault();

                string sql1 = @"select slm_Product_Id,slm_CampaignId  from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead where slm_ticketId ='" + data.slm_TicketId + "'";
                tempData r = slmdb.ExecuteStoreQuery<tempData>(sql1).FirstOrDefault();

                string sql2 = @"SELECT slm_SubStatusName  
              FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus 
              WHERE slm_OptionCode = '" + OptionCode + "' AND slm_CampaignId = '" + r.slm_CampaignId + "' and slm_Product_Id is null and slm_SubStatusCode ='" + SubStatusCode + "'";

                string ExternalSubStatusDesc = slmdb.ExecuteStoreQuery<string>(sql2).FirstOrDefault();
                if (ExternalSubStatusDesc == null || ExternalSubStatusDesc == "")
                {
                    string sql3 = @"SELECT slm_SubStatusName  
                                                  FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus 
                                  WHERE slm_OptionCode = '" + OptionCode + "' AND slm_CampaignId is null and slm_Product_Id = '" + r.slm_Product_Id + "' and slm_SubStatusCode  ='" + SubStatusCode + "'";
                    ExternalSubStatusDesc = slmdb.ExecuteStoreQuery<string>(sql3).FirstOrDefault();
                }

                if (obj != null)
                {
                    obj.slm_Status = OptionCode;
                    obj.slm_SubStatus = SubStatusCode;
                    obj.slm_ExternalSubStatusDesc = ExternalSubStatusDesc;
                    obj.slm_StatusDateSource = DateTime.Now;
                    obj.slm_UpdatedBy = data.slm_UpdatedBy;
                    obj.slm_UpdatedDate = DateTime.Now;
                    obj.slm_StatusBy = data.slm_UpdatedBy;

                    obj.slm_StatusDate = DateTime.Now;
                    obj.slm_ExternalStatus = null;
                    // obj.slm_ExternalSubStatusDesc = null;
                    obj.slm_ExternalSystem = null;
                    obj.slm_Counting = obj.slm_Counting.HasValue ? obj.slm_Counting : 0;
                }

                var reins = slmdb.kkslm_tr_renewinsurance.FirstOrDefault(re => re.slm_TicketId == data.slm_TicketId);
                if (reins != null)
                {
                    reins.slm_UpdatedBy = data.slm_UpdatedBy;
                    reins.slm_UpdatedDate = DateTime.Now;
                }

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        //ข้อมูลงานติดปัญหา  
        public void UpdateProblemdetail(ProblemDetailData data)
        {
            try
            {
                kkslm_tr_problemdetail obj = slmdb.kkslm_tr_problemdetail.Where(p => p.slm_ProblemDetailId == data.slm_ProblemDetailId).FirstOrDefault();

                if (obj != null)
                {
                    string currentFixTypeFlag = obj.slm_FixTypeFlag;  //เก็บค่าปัจจุบัน

                    if (data.slm_FixTypeFlag == "2")
                    {
                        obj.slm_IsAction = true;
                        obj.slm_ResponseDate = data.slm_ResponseDate;

                    }
                    else
                    {
                        obj.slm_IsAction = false;
                        obj.slm_ResponseDate = null;
                    }
                    obj.slm_FixTypeFlag = data.slm_FixTypeFlag;

                    obj.slm_ResponseDetail = data.slm_ResponseDetail;
                    obj.slm_Remark = data.slm_Remark;

                    obj.slm_UpdatedDate = data.slm_UpdatedDate;
                    obj.slm_UpdatedBy = data.slm_UpdatedBy;
                    obj.slm_PhoneContact = data.slm_PhoneContact;

                    if (currentFixTypeFlag == "2" && data.slm_FixTypeFlag == "1")  //แก้ไขแล้ว ถูกเปลี่ยนกลับเป็น อยู่ระหว่างดำเนินการ ให้ลบ report ออก
                    {
                        var sqldel = @"delete from [dbo].[kkslm_tr_problemcomeback_report]
                            where  slm_ProblemDetailId = '" + data.slm_ProblemDetailId + "' ";

                        slmdb.ExecuteStoreCommand(sqldel);
                    }

                }
                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //public bool CheckOwnerOrDelegate(string ticketId, string username)
        //{
        //    try
        //    {
        //        bool result = false;
        //        kkslm_tr_lead lead = slmdb.kkslm_tr_lead.Where(l => l.slm_ticketId == ticketId && l.is_Deleted == 0).FirstOrDefault();
        //        string ownerName = "";
        //        string delegateName = "";

        //        //kkslm_ms_staff thisStaff = slmdb.kkslm_ms_staff.Where(s => s.slm_UserName == username).FirstOrDefault();

        //        if (lead != null)
        //        {
        //            ownerName = lead.slm_Owner;
        //            delegateName = lead.slm_Delegate;
        //            if ((lead.slm_Owner == "" || lead.slm_Owner == null) && (lead.slm_Delegate == "" || lead.slm_Delegate == null))
        //            {
        //                return false;
        //            }
        //            else if (lead.slm_Owner.ToLower() == username.ToLower() || (lead.slm_Delegate == null ? "" : lead.slm_Delegate.ToLower()) == username.ToLower())
        //            {
        //                return true;
        //            }
        //            //                    else
        //            //                    {
        //            //                        string sql = @"SELECT T.*
        //            //                                        FROM KKSLM_MS_TEAMTELESALES T, KKSLM_MS_STAFF S
        //            //                                        WHERE T.SLM_HEADSTAFF = S.SLM_STAFFID 
        //            //                                        AND T.IS_DELETED = '0'
        //            //                                        AND S.IS_DELETED = '0' AND S.SLM_Username = '" + ownerName + "'";

        //            //                        var head = slmdb.ExecuteStoreQuery<kkslm_ms_teamtelesales>(sql, null);
        //            //                        if (head != null && head.Count() > 0)
        //            //                        {
        //            //                            return true;
        //            //                        }

        //            //                        sql = @"SELECT T.*
        //            //                                        FROM KKSLM_MS_TEAMTELESALES T, KKSLM_MS_STAFF S
        //            //                                        WHERE T.SLM_HEADSTAFF = S.SLM_STAFFID 
        //            //                                        AND T.IS_DELETED = '0'
        //            //                                        AND S.IS_DELETED = '0' AND S.SLM_Username = '" + delegateName + "'";

        //            //                        var delegateHead = slmdb.ExecuteStoreQuery<kkslm_ms_teamtelesales>(sql, null);
        //            //                        if (delegateHead != null && delegateHead.Count() > 0)
        //            //                        {
        //            //                            return true;
        //            //                        }
        //            //                    }

        //        }
        //        return result;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public bool CheckOwnerOrDelegate(string ticketId, string username)
        {
            try
            {
                string sql = $@"SELECT COUNT(slm_ticketId) 
                                FROM kkslm_tr_lead WITH (NOLOCK) 
                                WHERE slm_ticketId = '{ticketId}' AND is_Deleted = 0
                                AND (UPPER(slm_Owner) = '{username.ToUpper()}' OR UPPER(slm_Delegate) = '{username.ToUpper()}') ";

                int count = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
                return count > 0;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckSupervisor(string username)
        {
            try
            {
                //slmdb.dbo.kkslm_ms_staff.slm_StaffTypeId = “18  ” (Manager Telesales Outbound) และslmdb.dbo.kkslm_ms_staff.Is_Deleted = 0
                // slmdb.dbo.kkslm_ms_staff.slm_StaffTypeId = “16  ” (Operation Telesales Outbound)
                // slmdb.dbo.kkslm_ms_staff.slm_StaffTypeId = “17  ” (Product Telesales Outbound) 

                string sql = @"SELECT COUNT(*)
                                FROM KKSLM_MS_TEAMTELESALES T WITH (NOLOCK) 
                                INNER JOIN KKSLM_MS_STAFF S WITH (NOLOCK) ON T.slm_HeadStaff = S.slm_StaffId
                                WHERE S.SLM_Username = '" + username + "' AND T.IS_DELETED = '0' AND S.IS_DELETED = '0' ";

                int count = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
                return count > 0;
            }
            catch
            {
                throw;
            }
        }

        //public bool CheckReceipt(RenewInsuranceData data, decimal? policyRecAmt)
        //{
        //    try
        //    {
        //        bool result = false;

        //        if (data.slm_PayOptionId == 3)
        //        {
        //            int countRec = slmdb.kkslm_tr_renewinsurance_receipt.Where(r => r.slm_ticketId == data.slm_TicketId && r.slm_Status == null).Count();

        //            if (countRec > 0)
        //            {
        //                return true;
        //            }
        //        }
        //        else
        //        {
        //            List<kkslm_tr_renewinsurance_receipt_detail> payAmtlist = (from r in slmdb.kkslm_tr_renewinsurance_receipt
        //                                                                       join rd in slmdb.kkslm_tr_renewinsurance_receipt_detail
        //                                                                      on r.slm_RenewInsuranceReceiptId equals rd.slm_RenewInsuranceReceiptId
        //                                                                       where r.slm_ticketId == r.slm_ticketId
        //                                                                       && rd.slm_PaymentCode == "204"
        //                                                                       select rd).ToList();

        //            decimal payAmt = payAmtlist.Sum(p => p.slm_RecAmount).Value;

        //            if (policyRecAmt.Value <= payAmt)
        //            {
        //                return true;
        //            }
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        public bool CheckHeaderOwnerOrDelegate(string ticketId, string username)
        {
            try
            {
                string sql = @"SELECT COUNT(A.SLM_TICKETID) CNT
                                FROM dbo.KKSLM_TR_LEAD A WITH (NOLOCK) 
                                 LEFT JOIN dbo.KKSLM_MS_STAFF BO WITH (NOLOCK) ON A.SLM_OWNER=BO.SLM_USERNAME
                                 LEFT JOIN dbo.KKSLM_MS_TEAMTELESALES SO WITH (NOLOCK) ON BO.SLM_TEAMTELESALES_ID=SO.SLM_TEAMTELESALES_ID
                                 LEFT JOIN dbo.KKSLM_MS_STAFF HO WITH (NOLOCK) ON SO.SLM_HEADSTAFF=HO.SLM_STAFFID
                                 LEFT JOIN dbo.KKSLM_MS_STAFF BD WITH (NOLOCK) ON A.SLM_DELEGATE=BD.SLM_USERNAME
                                 LEFT JOIN dbo.KKSLM_MS_TEAMTELESALES SD WITH (NOLOCK) ON BD.SLM_TEAMTELESALES_ID=SD.SLM_TEAMTELESALES_ID
                                 LEFT JOIN dbo.KKSLM_MS_STAFF HD WITH (NOLOCK) ON SD.SLM_HEADSTAFF=HD.SLM_STAFFID
                                WHERE A.SLM_TICKETID='" + ticketId + "' AND (HO.SLM_USERNAME='" + username + "' OR HD.SLM_USERNAME='" + username + "')";

                int cnt = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
                return cnt > 0;
            }
            catch
            {
                throw;
            }
        }

        public string getReceiveNo(string ticketId, string username)
        {
            try
            {
                string receiveNo = "";
                string sql = @"SELECT LEAD.slm_Product_Id,REN.slm_InsuranceComId 
                    FROM dbo.kkslm_tr_lead LEAD WITH (NOLOCK) 
                    INNER JOIN dbo.kkslm_tr_renewinsurance REN WITH (NOLOCK) ON LEAD.slm_ticketId = REN.slm_TicketId 
                    WHERE LEAD.is_Deleted = 0 AND LEAD.slm_ticketId = '" + ticketId + "'";
                tempData r = slmdb.ExecuteStoreQuery<tempData>(sql).FirstOrDefault();

                if (r != null)
                {
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {

                        sql = @"SELECT top 1 SLM_RECEIVENO, slm_ReceiveNoListId, slm_ReceiveNoId
                            FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_receivenolist
                            WHERE is_Deleted = 0 and slm_Status = '0' and SLM_PRODUCT_ID = '" + r.slm_Product_Id +
                                "' AND SLM_INS_COM_ID = " + r.slm_InsuranceComId +
                                " Order by slm_ReceiveNoListId";

                        var data = operdb.ExecuteStoreQuery<ReceiveNoListData>(sql).FirstOrDefault();
                        if (data != null)
                        {
                            receiveNo = data.slm_ReceiveNo;
                            UpdateReceivenolist(data.slm_ReceiveNoListId, ticketId, username);
                        }

                        ts.Complete();

                        return receiveNo;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public void DoProcessIncentivePolicy(PreleadCompareDataCollection dataCollection, string createBy, DateTime createdDate)
        {
            try
            {
                RenewInsuranceData data = dataCollection.RenewIns;
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == data.slm_TicketId).FirstOrDefault();
                if (obj.slm_IncentiveFlag == null ? true : !obj.slm_IncentiveFlag.Value)
                {
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == obj.slm_TicketId).FirstOrDefault();
                    bool incentiveFlag = CalculateIncentivePolicy(obj, createdDate);

                    if (incentiveFlag)
                    {
                        dataCollection.RenewIns.slm_IncentiveFlag = incentiveFlag;

                        var lastAct = InsertLog(data, createBy, SLM.Resource.SLMConstant.ActionType.ChangeStatus, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);
                        UpdateLeadIncentive(data, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);

                        var act = InsertLog(data.slm_TicketId, createBy, SLMConstant.ActionType.GetIncentiveIns, null, null, false);
                        insertPhoneCall(dataCollection, lastAct);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CalculateIncentivePolicy(kkslm_tr_renewinsurance renew_ins, DateTime createdDate)
        {
            try
            {
                //คำนวณ Incentive ประกัน
                string sql1 = @"SELECT LEAD.SLM_PRODUCT_ID, renc.slm_Ins_Com_Id , PRO.slm_CampaignInsuranceId ,RENC.slm_PromotionId, REN.slm_CoverageTypeId
                FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.KKSLM_TR_LEAD LEAD INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance REN ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
                INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare renc on ren.slm_RenewInsureId = renc.slm_RenewInsureId 
                LEFT JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance PRO ON PRO.slm_PromotionId = RENC.slm_PromotionId 
                WHERE LEAD.IS_DELETED = 0 AND RENC.slm_Selected = 1 AND LEAD.slm_TicketId = '" + renew_ins.slm_TicketId + "'";

                tempData prod_com = slmdb.ExecuteStoreQuery<tempData>(sql1).FirstOrDefault();
                if (prod_com == null)
                {
                    throw new Exception("ไม่มีรายการซื้อประกัน");
                }

                string productId = string.IsNullOrEmpty(prod_com.slm_Product_Id) ? "" : prod_com.slm_Product_Id;
                decimal insComId = prod_com.slm_Ins_Com_Id != null ? prod_com.slm_Ins_Com_Id.Value : 0;
                decimal campaignInsuranceId = prod_com.slm_CampaignInsuranceId != null ? prod_com.slm_CampaignInsuranceId.Value : 0;
                int coverageTypeId = prod_com.slm_CoverageTypeId != null ? prod_com.slm_CoverageTypeId.Value : 0;

                var benefit = slmdb.SP_INQ_POLICY_BENEFIT(productId, insComId, campaignInsuranceId, coverageTypeId).FirstOrDefault();
                //var benefit = slmdb.ExecuteStoreQuery<BenefitData>(string.Format("exec dbo.SP_INQ_POLICY_BENEFIT '{0}', {1}, {2}, {3}", productId, insComId, campaignInsuranceId, coverageTypeId)).FirstOrDefault();
                if (benefit == null)
                {
                    throw new Exception("ไม่สามารถกดรับ Incentive ได้ เนื่องจากไม่ได้ set up ข้อมูลผลประโยชน์ กรุณาติดต่อทีม Product Insurance Motor");
                }

                //Validate VatFlag
                if (string.IsNullOrEmpty(benefit.slm_VatFlag))
                {
                    throw new Exception("ไม่พบข้อมูล Include, Exclude Vat ใน Benefit");
                }
                else
                {
                    if (benefit.slm_VatFlag != "I" && benefit.slm_VatFlag != "E")
                    {
                        return false;
                    }
                }

                int vat = SLM.Resource.SLMConstant.VatRate;
                decimal policyComBath = 0;
                decimal policyComBathVat = 0;
                decimal policyComBathIncentive = 0;
                decimal policyOv1Bath = 0;
                decimal policyOv1BathVat = 0;
                decimal policyOv1BathIncentive = 0;
                decimal policyOv2Bath = 0;
                decimal policyOv2BathVat = 0;
                decimal policyOv2BathIncentive = 0;
                decimal policyIncentiveAmount = 0;

                //1.หาค่าคอม
                if (benefit.slm_ComissionFlag == "1")
                {
                    //decimal policyPremiumTotal = renew_ins.slm_PolicyGrossPremiumTotal != null ? renew_ins.slm_PolicyGrossPremiumTotal.Value : 0;
                    decimal policyPremiumTotal = renew_ins.slm_PolicyNetGrossPremium != null ? renew_ins.slm_PolicyNetGrossPremium.Value : 0;
                    decimal comPercent = benefit.slm_ComissionPercentValue != null ? benefit.slm_ComissionPercentValue.Value : 0;
                    policyComBath = Math.Round((policyPremiumTotal * comPercent) / 100, 2);
                }
                else
                {
                    policyComBath = benefit.slm_ComissionBathValue != null ? benefit.slm_ComissionBathValue.Value : 0;
                }

                //2.หา VAT ของค่าคอม
                policyComBathVat = Math.Round((policyComBath * vat) / 100, 2);

                //3.หา Incentive สุทธิของค่าคอม
                policyComBathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((policyComBath - policyComBathVat), 2) : Math.Round((policyComBath + policyComBathVat), 2);

                //คำนวณ OV1
                if (benefit.slm_OV1Flag == "1" || benefit.slm_OV1Flag == "2")
                {
                    //4.หา OV1
                    if (benefit.slm_OV1Flag == "1")
                    {
                        //decimal policyPremiumTotal = renew_ins.slm_PolicyGrossPremiumTotal != null ? renew_ins.slm_PolicyGrossPremiumTotal.Value : 0;
                        decimal policyPremiumTotal = renew_ins.slm_PolicyNetGrossPremium != null ? renew_ins.slm_PolicyNetGrossPremium.Value : 0;
                        decimal ov1Percent = benefit.slm_OV1PercentValue != null ? benefit.slm_OV1PercentValue.Value : 0;
                        policyOv1Bath = Math.Round((policyPremiumTotal * ov1Percent) / 100, 2);
                    }
                    else
                    {
                        policyOv1Bath = benefit.slm_OV1BathValue != null ? benefit.slm_OV1BathValue.Value : 0;
                    }

                    //5.หา Vat ของ OV1
                    policyOv1BathVat = Math.Round((policyOv1Bath * vat) / 100, 2);

                    //6.หา Incentive ของค่า OV1
                    policyOv1BathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((policyOv1Bath - policyOv1BathVat), 2) : Math.Round((policyOv1Bath + policyOv1BathVat), 2);
                }

                //คำนวณ OV2
                if (benefit.slm_OV2Flag == "1" || benefit.slm_OV2Flag == "2")
                {
                    //7.หา OV2
                    if (benefit.slm_OV2Flag == "1")
                    {
                        //decimal policyPremiumTotal = renew_ins.slm_PolicyGrossPremiumTotal != null ? renew_ins.slm_PolicyGrossPremiumTotal.Value : 0;
                        decimal policyPremiumTotal = renew_ins.slm_PolicyNetGrossPremium != null ? renew_ins.slm_PolicyNetGrossPremium.Value : 0;
                        decimal ov2Percent = benefit.slm_OV2PercentValue != null ? benefit.slm_OV2PercentValue.Value : 0;
                        policyOv2Bath = Math.Round((policyPremiumTotal * ov2Percent) / 100, 2);
                    }
                    else
                    {
                        policyOv2Bath = benefit.slm_OV2BathValue != null ? benefit.slm_OV2BathValue.Value : 0;
                    }

                    //8.หา Vat ของ OV2
                    policyOv2BathVat = Math.Round((policyOv2Bath * vat) / 100, 2);

                    //9.หา Incentive ของค่า OV2
                    policyOv2BathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((policyOv2Bath - policyOv2BathVat), 2) : Math.Round((policyOv2Bath + policyOv2BathVat), 2);
                }

                //10.คำนวณ policyIncentiveAmount
                policyIncentiveAmount = Math.Round(policyComBathIncentive + policyOv1BathIncentive + policyOv2BathIncentive, 2);

                renew_ins.slm_PolicyComBath = policyComBath;
                renew_ins.slm_PolicyComBathVat = policyComBathVat;
                renew_ins.slm_PolicyComBathIncentive = policyComBathIncentive;
                renew_ins.slm_PolicyOV1Bath = policyOv1Bath;
                renew_ins.slm_PolicyOV1BathVat = policyOv1BathVat;
                renew_ins.slm_PolicyOV1BathIncentive = policyOv1BathIncentive;
                renew_ins.slm_PolicyOV2Bath = policyOv2Bath;
                renew_ins.slm_PolicyOV2BathVat = policyOv2BathVat;
                renew_ins.slm_PolicyOV2BathIncentive = policyOv2BathIncentive;
                renew_ins.slm_PolicyIncentiveAmount = policyIncentiveAmount;
                renew_ins.slm_IncentiveFlag = true;
                renew_ins.slm_IncentiveDate = createdDate;
                renew_ins.slm_PolicyReferenceNote = "Scenario = " + benefit.Scenario + "|" + benefit.slm_PolicyReferenceNote;

                slmdb.SaveChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public kkslm_tr_activity DoProcessIncentiveAct(PreleadCompareDataCollection collection, string createBy, DateTime createdDate)
        {
            try
            {
                kkslm_tr_activity act = null;
                RenewInsuranceData data = collection.RenewIns;
                kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == data.slm_TicketId).FirstOrDefault();
                if (obj.slm_ActIncentiveFlag == null ? true : !obj.slm_ActIncentiveFlag.Value)
                {
                    var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == obj.slm_TicketId).FirstOrDefault();
                    bool incentiveFlag = CalculateIncentiveAct(obj, createBy, createdDate);

                    if (incentiveFlag)
                    {

                        // ตรวจสอบว่าเป็น พรบเดี่ยว หรือ ประกัน+พรบ
                        string sql = @"SELECT COUNT(ren.slm_RenewInsureId) AS CNT
                            FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.KKSLM_TR_LEAD LEAD INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance REN ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
                            INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare renc on ren.slm_RenewInsureId = renc.slm_RenewInsureId 
                            WHERE LEAD.IS_DELETED = 0 AND RENC.slm_Selected = 1 AND LEAD.slm_TicketId = '" + data.slm_TicketId + "'";

                        int countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
                        if (countPolicy == 0) // พรบ เดี่ยว
                        {
                            // ถ้าเป็นพรบ เดี่ยวให้ลงสถานะ และเปลี่ยนสถานะของ Lead ด้วย (กรณีถ้ามีประกัน ให้ใช้สถานะของประกันแทน)
                            //UpdateLeadIncentive(data, SLM.Resource.SLMConstant.StatusCode.Close, SLM.Resource.SLMConstant.SubStatusCode.ReceiveActNo);
                            act = InsertLog(data, createBy, SLM.Resource.SLMConstant.ActionType.ChangeStatus, SLM.Resource.SLMConstant.StatusCode.WaitConsider
                                , SLM.Resource.SLMConstant.SubStatusCode.WaitDocAct);

                            UpdateLeadIncentive(data, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocAct);

                            //var actWithStatus = slmdb.kkslm_tr_activity
                            //    .Where(a => a.slm_TicketId == data.slm_TicketId && a.slm_NewStatus != null)
                            //    .OrderByDescending(a => a.slm_UpdatedDate).FirstOrDefault();
                            //act = actWithStatus ?? act;
                            InsertHistory(act);

                            // insertPhoneCall(collection, act);
                            slmdb.SaveChanges();

                        }
                        collection.RenewIns.slm_ActIncentiveFlag = incentiveFlag; // IF Incentive Flag Is pass all Check 
                        // มีการ กด incentive act ต้องลง Log Incentive เสมอ (พรบเดี่ยว และ พรบ+ประกัน)
                        var actIncentive = InsertLog(data.slm_TicketId, createBy, SLM.Resource.SLMConstant.ActionType.GetIncentiveAct
                            , null, null, false);
                        insertPhoneCall(collection, act ?? actIncentive);

                    }
                }
                return act;
            }
            catch
            {
                throw;
            }
        }

        private bool CalculateIncentiveAct(kkslm_tr_renewinsurance renew_ins, String createBy, DateTime createdDate)
        {
            try
            {
                string sql1 = @"SELECT LEAD.SLM_PRODUCT_ID AS slm_Product_Id, rencc.slm_Ins_Com_Id AS slm_Ins_Com_Id , PRO.slm_CampaignInsuranceId AS slm_CampaignInsuranceId, rencc.slm_PromotionId, 
                        convert(int, REN.slm_InsurancecarTypeId) AS slm_CoverageTypeId
                        FROM " + SLM.Resource.SLMConstant.SLMDBName + @".DBO.KKSLM_TR_LEAD LEAD 
                        INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".DBO.kkslm_tr_renewinsurance REN ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
                        LEFT JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_productinfo prod ON prod.slm_TicketId = lead.slm_ticketId
                        INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".DBO.kkslm_tr_renewinsurance_compare_act rencc on ren.slm_RenewInsureId = rencc.slm_RenewInsureId 
                        LEFT JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".DBO.kkslm_ms_promotioninsurance PRO ON PRO.slm_PromotionId = rencc.slm_PromotionId 
                        WHERE LEAD.IS_DELETED = 0 AND rencc.slm_ActPurchaseFlag = 1 AND LEAD.slm_ticketId = '" + renew_ins.slm_TicketId + "' ";

                tempData prod_com = slmdb.ExecuteStoreQuery<tempData>(sql1).FirstOrDefault();
                if (prod_com == null)
                {
                    throw new Exception("ไม่มีรายการซื้อ พรบ.");
                }

                string productId = string.IsNullOrEmpty(prod_com.slm_Product_Id) ? "" : prod_com.slm_Product_Id;
                decimal insComId = prod_com.slm_Ins_Com_Id != null ? prod_com.slm_Ins_Com_Id.Value : 0;
                decimal campaignInsuranceId = prod_com.slm_CampaignInsuranceId != null ? prod_com.slm_CampaignInsuranceId.Value : 0;
                int insurancecarTypeId = prod_com.slm_CoverageTypeId != null ? prod_com.slm_CoverageTypeId.Value : 0;

                var benefit = slmdb.SP_INQ_ACT_BENEFIT(productId, insComId, campaignInsuranceId, insurancecarTypeId).FirstOrDefault();

                if (benefit == null)
                {
                    throw new Exception("ไม่สามารถกดรับ Incentive ได้ เนื่องจากไม่ได้ set up ข้อมูลผลประโยชน์ กรุณาติดต่อทีม Product Insurance Motor");
                }

                //Validate VatFlag
                if (string.IsNullOrEmpty(benefit.slm_VatFlag))
                {
                    throw new Exception("ไม่พบข้อมูล Include, Exclude Vat ใน Benefit");
                }
                else
                {
                    if (benefit.slm_VatFlag != "I" && benefit.slm_VatFlag != "E")
                    {
                        //Util.WriteLogFile(logfilename, BatchCode, "SUCCESS โดยข้อมูล VatFlag Error, VatFlag=" + benefit.slm_VatFlag + " (พรบ.)");
                        //BizUtil.InsertLog(BatchMonitorId, ticketId, "", "SUCCESS โดยข้อมูล VatFlag Error, VatFlag=" + benefit.slm_VatFlag + " (พรบ.)");
                        return false;
                    }
                }

                int vat = SLM.Resource.SLMConstant.VatRate;
                decimal actComBath = 0;
                decimal actComBathVat = 0;
                decimal actComBathIncentive = 0;
                decimal actOv1Bath = 0;
                decimal actOv1BathVat = 0;
                decimal actOv1BathIncentive = 0;
                decimal actOv2Bath = 0;
                decimal actOv2BathVat = 0;
                decimal actOv2BathIncentive = 0;
                decimal actIncentiveAmount = 0;

                //1.หาค่าคอม
                if (benefit.slm_ComissionFlag == "1")
                {
                    decimal actNetPremium = renew_ins.slm_ActNetPremium != null ? renew_ins.slm_ActNetPremium.Value : 0;
                    decimal comPercent = benefit.slm_ComissionPercentValue != null ? benefit.slm_ComissionPercentValue.Value : 0;
                    actComBath = Math.Round((actNetPremium * comPercent) / 100, 2);
                }
                else
                {
                    actComBath = benefit.slm_ComissionBathValue != null ? benefit.slm_ComissionBathValue.Value : 0;
                }

                //2.หา VAT ของค่าคอม
                actComBathVat = Math.Round((actComBath * vat) / 100, 2);

                //3.หา Incentive สุทธิของค่าคอม
                actComBathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((actComBath - actComBathVat), 2) : Math.Round((actComBath + actComBathVat), 2);

                //คำนวณ OV1
                if (benefit.slm_OV1Flag == "1" || benefit.slm_OV1Flag == "2")
                {
                    //4.หา OV1
                    if (benefit.slm_OV1Flag == "1")
                    {
                        decimal actNetPremium = renew_ins.slm_ActNetPremium != null ? renew_ins.slm_ActNetPremium.Value : 0;
                        decimal ov1Percent = benefit.slm_OV1PercentValue != null ? benefit.slm_OV1PercentValue.Value : 0;
                        actOv1Bath = Math.Round((actNetPremium * ov1Percent) / 100, 2);
                    }
                    else
                    {
                        actOv1Bath = benefit.slm_OV1BathValue != null ? benefit.slm_OV1BathValue.Value : 0;
                    }

                    //5.หา Vat ของ OV1
                    actOv1BathVat = Math.Round((actOv1Bath * vat) / 100, 2);

                    //6.หา Incentive ของค่า OV1
                    actOv1BathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((actOv1Bath - actOv1BathVat), 2) : Math.Round((actOv1Bath + actOv1BathVat), 2);
                }

                //คำนวณ OV2
                if (benefit.slm_OV2Flag == "1" || benefit.slm_OV2Flag == "2")
                {
                    //7.หา OV2
                    if (benefit.slm_OV2Flag == "1")
                    {
                        decimal actNetPremium = renew_ins.slm_ActNetPremium != null ? renew_ins.slm_ActNetPremium.Value : 0;
                        decimal ov2Percent = benefit.slm_OV2PercentValue != null ? benefit.slm_OV2PercentValue.Value : 0;
                        actOv2Bath = Math.Round((actNetPremium * ov2Percent) / 100, 2);
                    }
                    else
                    {
                        actOv2Bath = benefit.slm_OV2BathValue != null ? benefit.slm_OV2BathValue.Value : 0;
                    }

                    //8.หา Vat ของ OV2
                    actOv2BathVat = Math.Round((actOv2Bath * vat) / 100, 2);

                    //9.หา Incentive ของค่า OV2
                    actOv2BathIncentive = benefit.slm_VatFlag == "I" ? Math.Round((actOv2Bath - actOv2BathVat), 2) : Math.Round((actOv2Bath + actOv2BathVat), 2);
                }

                //10.คำนวณ policyIncentiveAmount
                actIncentiveAmount = Math.Round(actComBathIncentive + actOv1BathIncentive + actOv2BathIncentive, 2);

                renew_ins.slm_ActComBath = actComBath;
                renew_ins.slm_ActComBathVat = actComBathVat;
                renew_ins.slm_ActComBathIncentive = actComBathIncentive;
                renew_ins.slm_ActOV1Bath = actOv1Bath;
                renew_ins.slm_ActOV1BathVat = actOv1BathVat;
                renew_ins.slm_ActOV1BathIncentive = actOv1BathIncentive;
                renew_ins.slm_ActOV2Bath = actOv2Bath;
                renew_ins.slm_ActOV2BathVat = actOv2BathVat;
                renew_ins.slm_ActOV2BathIncentive = actOv2BathIncentive;
                renew_ins.slm_ActIncentiveAmount = actIncentiveAmount;
                renew_ins.slm_ActIncentiveFlag = true;
                renew_ins.slm_ActIncentiveDate = createdDate;
                renew_ins.slm_ActReferenceNote = "Scenario = " + benefit.Scenario + "|" + benefit.slm_ActReferenceNote;
                renew_ins.slm_UpdatedBy = createBy;
                renew_ins.slm_UpdatedDate = DateTime.Now;
                slmdb.SaveChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public tempData checkPolicyVat(string ticketId)
        {
            try
            {
                tempData result = new tempData();

                string sql1 = @"SELECT LEAD.SLM_PRODUCT_ID, renc.slm_Ins_Com_Id , PRO.slm_CampaignInsuranceId ,RENC.slm_PromotionId, PRO.slm_CoverageTypeId
                FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.KKSLM_TR_LEAD LEAD INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance REN ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
                INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare renc on ren.slm_RenewInsureId = renc.slm_RenewInsureId 
                LEFT JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance PRO ON PRO.slm_PromotionId = RENC.slm_PromotionId 
                WHERE LEAD.IS_DELETED = 0 AND RENC.slm_Selected = 1 AND LEAD.slm_TicketId = '" + ticketId + "'";

                tempData r = slmdb.ExecuteStoreQuery<tempData>(sql1).FirstOrDefault();

                if (r != null)
                {
                    var ret = slmdb.SP_INQ_POLICY_BENEFIT(r.slm_Product_Id, r.slm_Ins_Com_Id, r.slm_CampaignInsuranceId, r.slm_CoverageTypeId).FirstOrDefault();
                    if (ret != null)
                    {
                        result.slm_CampaignId = r.slm_CampaignId;
                        result.slm_CampaignInsuranceId = r.slm_CampaignInsuranceId;
                        result.slm_ComissionBathValue = ret.slm_ComissionBathValue;
                        result.slm_ComissionFlag = ret.slm_ComissionFlag;
                        result.slm_ComissionPercentValue = ret.slm_ComissionPercentValue;
                        result.slm_CoverageTypeId = ret.slm_CoverageTypeId;
                        result.slm_InsuranceComId = r.slm_InsuranceComId;
                        result.slm_Ins_Com_Id = ret.slm_Ins_Com_Id;
                        result.slm_OV1BathValue = ret.slm_OV1BathValue;
                        result.slm_OV1Flag = ret.slm_OV1Flag;
                        result.slm_OV1PercentValue = ret.slm_OV1PercentValue;
                        result.slm_OV2BathValue = ret.slm_OV2BathValue;
                        result.slm_OV2Flag = ret.slm_OV2Flag;
                        result.slm_OV2PercentValue = ret.slm_OV2PercentValue;
                        result.slm_Product_Id = ret.slm_Product_Id;
                        result.slm_PromotionId = r.slm_PromotionId;
                        result.slm_VatFlag = ret.slm_VatFlag;
                    }
                    //string sql2_1 = @"SELECT top 1 slm_Product_Id,slm_Ins_Com_Id,slm_CampaignInsuranceId,slm_ComissionFlag ,slm_ComissionPercentValue,slm_ComissionBathValue,slm_OV1Flag   ,slm_OV1PercentValue,slm_OV1BathValue,slm_OV2Flag,slm_OV2PercentValue,slm_OV2BathValue,slm_VatFlag
                    //            FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_benefit
                    //            WHERE is_Deleted = 0 AND slm_Ins_Com_Id = " + r.slm_Ins_Com_Id + " AND slm_Product_Id = '" + r.slm_Product_Id + "'AND slm_CampaignInsuranceId is null";


                    //if (r.slm_PromotionId == null || r.slm_PromotionId == 0)
                    //{
                    //    result = slmdb.ExecuteStoreQuery<tempData>(sql2_1).FirstOrDefault();
                    //}
                    //else
                    //{
                    //    string sql2_2 = @"SELECT top 1 slm_Product_Id,slm_Ins_Com_Id,slm_CampaignInsuranceId,slm_ComissionFlag
                    //      ,slm_ComissionPercentValue,slm_ComissionBathValue,slm_OV1Flag
                    //      ,slm_OV1PercentValue,slm_OV1BathValue,slm_OV2Flag,slm_OV2PercentValue
                    //      ,slm_OV2BathValue,slm_VatFlag
                    //  FROM " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_benefit
                    //  WHERE is_Deleted = 0 AND slm_Ins_Com_Id = " + r.slm_Ins_Com_Id + " AND slm_Product_Id = '" + r.slm_Product_Id + "' AND slm_CampaignInsuranceId = " + r.slm_CampaignInsuranceId;
                    //    result = slmdb.ExecuteStoreQuery<tempData>(sql2_2).FirstOrDefault();

                    //    if (result == null)
                    //    {
                    //        result = slmdb.ExecuteStoreQuery<tempData>(sql2_1).FirstOrDefault();
                    //    }

                    //}

                    if (result == null)
                        throw new Exception("ไม่สามารถกดรับ Incentive ได้ เนื่องจากไม่ได้ set up ข้อมูลผลประโยชน์ กรุณาติดต่อทีม Product Insurance Motor");

                    return result;
                }
                else
                {
                    throw new Exception("ไม่มีรายการซื้อประกัน");
                }
            }
            catch
            {
                throw;
            }
        }

        public tempData checkActVat(string ticketId)
        {
            try
            {
                tempData result = new tempData();

                string sql1 = @"SELECT LEAD.SLM_PRODUCT_ID, rencc.slm_Ins_Com_Id , PRO.slm_CampaignInsuranceId ,rencc.slm_PromotionId, PRO.slm_CoverageTypeId
                FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.KKSLM_TR_LEAD LEAD INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance REN ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
                INNER JOIN " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare_act rencc on ren.slm_RenewInsureId = rencc.slm_RenewInsureId 
                LEFT JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance PRO ON PRO.slm_PromotionId = rencc.slm_PromotionId 
                WHERE LEAD.IS_DELETED = 0 AND rencc.slm_ActPurchaseFlag = 1 AND LEAD.slm_TicketId = '" + ticketId + "'";

                tempData r = slmdb.ExecuteStoreQuery<tempData>(sql1).FirstOrDefault();

                if (r != null)
                {
                    var ret = slmdb.SP_INQ_ACT_BENEFIT(r.slm_Product_Id, r.slm_Ins_Com_Id, r.slm_CampaignInsuranceId, r.slm_CoverageTypeId).FirstOrDefault();
                    if (ret != null)
                    {
                        result.slm_CampaignId = r.slm_CampaignId;
                        result.slm_CampaignInsuranceId = r.slm_CampaignInsuranceId;
                        result.slm_ComissionBathValue = ret.slm_ComissionBathValue;
                        result.slm_ComissionFlag = ret.slm_ComissionFlag;
                        result.slm_ComissionPercentValue = ret.slm_ComissionPercentValue;
                        result.slm_CoverageTypeId = ret.slm_CoverageTypeId;
                        result.slm_InsuranceComId = r.slm_InsuranceComId;
                        result.slm_Ins_Com_Id = ret.slm_Ins_Com_Id;
                        result.slm_OV1BathValue = ret.slm_OV1BathValue;
                        result.slm_OV1Flag = ret.slm_OV1Flag;
                        result.slm_OV1PercentValue = ret.slm_OV1PercentValue;
                        result.slm_OV2BathValue = ret.slm_OV2BathValue;
                        result.slm_OV2Flag = ret.slm_OV2Flag;
                        result.slm_OV2PercentValue = ret.slm_OV2PercentValue;
                        result.slm_Product_Id = ret.slm_Product_Id;
                        result.slm_PromotionId = r.slm_PromotionId;
                        result.slm_VatFlag = ret.slm_VatFlag;
                    }

                    if (result == null)
                        throw new Exception("ไม่สามารถกดรับ Incentive ได้ เนื่องจากไม่ได้ set up ข้อมูลผลประโยชน์ กรุณาติดต่อทีม Product Insurance Motor");

                    return result;
                }
                else
                {
                    throw new Exception("ไม่มีรายการซื้อ พรบ.");
                }
            }
            catch
            {
                throw;
            }
        }


        public kkslm_tr_activity InsertLog(RenewInsuranceData data, string createBy, string slm_Type, string newStatus, string newSubStatus)
        {
            return InsertLog(data.slm_TicketId, createBy, slm_Type, newStatus, newSubStatus);
        }

        public kkslm_tr_activity InsertLog(string ticketID, string createBy, string slm_Type, string newStatus, string newSubStatus)
        {
            return InsertLog(ticketID, createBy, slm_Type, newStatus, newSubStatus, true);
        }

        public kkslm_tr_activity InsertLog(string ticketID, string createBy, string slm_Type, string newStatus, string newSubStatus, bool findOldStatus)
        {
            string oldStatus = null;
            string oldSubStatus = null;
            string oldExternalSubStatus = null;
            string oldExternalSystem = null;
            string oldExternalStatus = null;
            string oldExternalSubStatusDesc = null;
            string newSubStatusDesc = null;

            kkslm_tr_renewinsurance obj = slmdb.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == ticketID).FirstOrDefault();
            var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == obj.slm_TicketId).FirstOrDefault();

            if (findOldStatus)
            {
                oldStatus = lead.slm_Status;
                oldSubStatus = string.IsNullOrEmpty(lead.slm_SubStatus) ? null : lead.slm_SubStatus;
                oldExternalSubStatusDesc = string.IsNullOrEmpty(lead.slm_ExternalSubStatusDesc) ? null : lead.slm_ExternalSubStatusDesc;
                oldExternalSystem = lead.slm_ExternalSystem;
                oldExternalStatus = lead.slm_ExternalStatus;
                oldExternalSubStatus = lead.slm_ExternalSubStatus;
            }

            if (newStatus != null && newSubStatus != null)
                newSubStatusDesc = GetSubStatusName(lead.slm_CampaignId, lead.slm_Product_Id, newStatus, newSubStatus);

            //4.Insert OwnerLogging
            kkslm_tr_activity act = new kkslm_tr_activity()
            {
                slm_TicketId = obj.slm_TicketId,
                slm_OldStatus = oldStatus,
                slm_OldSubStatus = oldSubStatus,
                slm_NewStatus = newStatus,
                slm_NewSubStatus = newSubStatus,
                slm_CreatedBy = createBy,
                slm_CreatedBy_Position = GetPositionId(createBy),
                slm_CreatedDate = DateTime.Now,
                slm_UpdatedBy = createBy,
                slm_UpdatedDate = DateTime.Now,
                slm_Type = slm_Type,                    //EODUpdateCurrent = 09
                slm_SystemAction = "SLM",           //System ที่เข้ามาทำ action
                slm_SystemActionBy = "SLM",         //action เกิดขึ้นที่ระบบอะไร
                slm_ExternalSystem_Old = oldExternalSystem,
                slm_ExternalStatus_Old = oldExternalStatus,
                slm_ExternalSubStatus_Old = oldExternalSubStatus,
                slm_ExternalSubStatusDesc_Old = oldExternalSubStatusDesc,
                slm_ExternalStatus_New = null,
                slm_ExternalSubStatus_New = null,
                slm_ExternalSystem_New = null,
                slm_ExternalSubStatusDesc_New = newSubStatusDesc
            };
            slmdb.kkslm_tr_activity.AddObject(act);

            slmdb.SaveChanges();
            return act;
        }

        public void InsertHistory(kkslm_tr_activity activity)
        {
            var staff = slmdb.kkslm_ms_staff.First(s => s.slm_UserName == activity.slm_UpdatedBy);
            var hist = new kkslm_tr_history()
            {
                slm_ticketId = activity.slm_TicketId,
                slm_History_Type_Code = Resource.SLMConstant.HistoryTypeCode.UpdateStatus,
                slm_OldValue = (activity.slm_OldStatus ?? "") + "," + (activity.slm_OldSubStatus ?? ""),
                slm_NewValue = (activity.slm_NewStatus ?? "") + "," + (activity.slm_NewSubStatus ?? ""),
                slm_CreatedBy = activity.slm_CreatedBy,
                slm_CreatedBy_Position = activity.slm_CreatedBy_Position,
                slm_CreateBy_Branch = staff == null ? "" : staff.slm_BranchCode,
                slm_CreatedDate = DateTime.Now,
                slm_UpdatedBy = activity.slm_UpdatedBy,
                slm_UpdatedDate = DateTime.Now,
                is_Deleted = false
            };

            slmdb.kkslm_tr_history.AddObject(hist);
            slmdb.SaveChanges();
        }

        public kkslm_tr_activity SaveReceivePolicy(PreleadCompareDataCollection data, string crateBy)
        {
            try
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                //Incentive
                var newStatus = SLMConstant.StatusCode.OnProcess;
                var newSubStatus = SLMConstant.SubStatusCode.GetInsNo;

                var staff = slmdb.kkslm_ms_staff.Where(s => s.slm_UserName == crateBy).FirstOrDefault();
                var lead = slmdb.kkslm_tr_lead.Where(l => l.slm_ticketId == data.RenewIns.slm_TicketId).FirstOrDefault();
                if (lead != null)
                {
                    slmdb.kkslm_tr_history.AddObject(new kkslm_tr_history()
                    {
                        slm_ticketId = lead.slm_ticketId,
                        slm_History_Type_Code = SLMConstant.HistoryTypeCode.UpdateStatus,
                        slm_OldValue = String.Format("{0},{1}", lead.slm_Status, lead.slm_SubStatus),
                        slm_NewValue = String.Format("{0},{1}", newStatus, newSubStatus),
                        slm_CreatedBy = crateBy,
                        slm_CreateBy_Branch = staff != null ? staff.slm_BranchCode : "",
                        slm_CreatedBy_Position = staff != null ? staff.slm_Position_id : 0,
                        slm_CreatedDate = DateTime.Now,
                        slm_UpdatedBy = crateBy,
                        slm_UpdatedDate = DateTime.Now,
                        is_Deleted = false
                    });
                    slmdb.SaveChanges();
                }


                UpdateRenewinsuranceReceivePolicy(data.RenewIns);
                kkslm_tr_activity act = InsertLog(data.RenewIns, crateBy, SLM.Resource.SLMConstant.ActionType.ChangeStatus, newStatus, newSubStatus);
                UpdateLeadIncentive(data.RenewIns, newStatus, newSubStatus);
                insertkkslm_tr_notify_renewinsurance_report(data.RenewIns, data.RenewIns.slm_TicketId, data.RenewIns.slm_UpdatedBy);
                insertPhoneCall(data, act);
                return act;
            }
            catch
            {
                throw;
            }

        }

        public string GetPhonecallLogContent(PreleadCompareDataCollection data)
        {
            //insert phonecall
            string content = "";

            content += "เลขที่รับแจ้ง:" + (string.IsNullOrEmpty(data.RenewIns.slm_ReceiveNo) ? "" : (" " + data.RenewIns.slm_ReceiveNo));
            if (data.RenewIns.slm_ActSendDate != null)
            {
                content += ", วันที่ส่งแจ้ง พรบ.: " + data.RenewIns.slm_ActSendDate.Value.ToString("dd/MM/") + data.RenewIns.slm_ActSendDate.Value.Year.ToString();
            }
            else
            {
                content += ", วันที่ส่งแจ้ง พรบ.:";
            }
            #region
            ///*--------------------------------------------------------------------------------*/
            //bool policyPurchased = data.ComparePromoList.FirstOrDefault(p => p.slm_Selected != null && p.slm_Selected == true) != null;
            //if (policyPurchased)
            //    content += ", Incentive ประกัน: " + (data.RenewIns.slm_IncentiveFlag != null && data.RenewIns.slm_IncentiveFlag.Value == true ? "Y" : "N");
            //else
            //    content += ", Incentive ประกัน:";
            ///*--------------------------------------------------------------------------------*/
            //bool actPurchased = (data.ActPromoList.FirstOrDefault(a => a.slm_ActPurchaseFlag != null && a.slm_ActPurchaseFlag == true) != null)
            //        || data.RenewIns.slm_ActSendDate != null; // F or T = T
            //if (actPurchased)
            //{
            //    content += ", Incentive พรบ.: " + (data.RenewIns.slm_ActIncentiveFlag == true
            //        ? "Y"
            //        : data.FlagFirstReceiveAct ? "N" : data.FlagManualActIncentive ? "Y" : "N");
            //}
            //else
            //{
            //    if (!data.FlagFirstReceiveAct)
            //    {
            //        if (data.FlagManualActIncentive == true) content += ", Incentive พรบ.: Y";
            //        else content += ", Incentive พรบ.:"; // cancel same mouth and Year 
            //    }
            //    else
            //    {
            //        content += ", Incentive พรบ.: N";
            //    }
            //}
            #endregion
            bool? FlagPolicyPurchase = data.ComparePromoList.Select(e => e.slm_Selected).FirstOrDefault();
            bool? FlagActPurchase = data.ActPromoList.Select(e => e.slm_ActPurchaseFlag).FirstOrDefault();
            content += ", Incentive ประกัน: " + (FlagPolicyPurchase == null || FlagPolicyPurchase == false
                ? data.RenewIns.slm_IncentiveFlag == true ? "Y" : ""
                : data.RenewIns.slm_IncentiveFlag == true ? "Y" : "N");

            content += ", Incentive พรบ.: " + (FlagActPurchase == null || FlagActPurchase == false
                ? data.RenewIns.slm_ActIncentiveFlag == true ? "Y" : ""
                : data.RenewIns.slm_ActIncentiveFlag == true ? "Y" : "N");
            #region
            //ค่าเบี้ยประกันที่ลูกค้าชำระ
            //if (data.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204").Count() > 0)
            //{
            //    if (!data.FlagFirstReceivePolicy)
            //    {
            //        var paidPolicy = data.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204")
            //                                        .GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate })
            //                                        .Select(g => new
            //                                        {
            //                                            ReceiptId = g.Key.slm_RenewInsuranceReceiptId,
            //                                            TransDate = g.Key.slm_TransDate,
            //                                            Amount = g.Sum(a => a.slm_RecAmount ?? 0)
            //                                        })
            //                                        .OrderByDescending(g => g.TransDate)
            //                                        .FirstOrDefault();

            //        content += ", ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้: " + (paidPolicy == null ? "" : paidPolicy.Amount.ToString("#,##0.00"));

            //    }
            //    else
            //        content += ", ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้:";
            //}
            //else

            #endregion
            content += ", ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้:";
            #region
            //ค่าเบี้ยพรบ.ที่ลูกค้าชำระ
            //if (data.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205").Count() > 0)
            //{
            //    if (!data.FlagFirstReceiveAct && !data.FlagManualActIncentive )
            //    {
            //        var paidAct = data.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205")
            //                                        .GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate })
            //                                        .Select(g => new
            //                                        {
            //                                            ReceiptId = g.Key.slm_RenewInsuranceReceiptId,
            //                                            TransDate = g.Key.slm_TransDate,
            //                                            Amount = g.Sum(a => a.slm_RecAmount ?? 0)
            //                                        })
            //                                        .OrderByDescending(g => g.TransDate)
            //                                        .FirstOrDefault();

            //        content += ", ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้: " + (paidAct == null ? "" : paidAct.Amount.ToString("#,##0.00"));
            //    }
            //    else
            //        content += ", ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้:";
            //}
            //else
            #endregion
            content += ", ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้:";
            //ค่าเบี้ยชำระรวม                
            content += ", ค่าเบี้ยประกันที่ลูกค้าชำระรวม: " + (data.PolicyRecAmt != null && data.PolicyRecAmt.Value != 0 ? data.PolicyRecAmt.Value.ToString("#,##0.00") : "");
            content += ", ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม: " + (data.ActRecAmt != null && data.ActRecAmt.Value != 0 ? data.ActRecAmt.Value.ToString("#,##0.00") : "");
            return content;
        }

        // move to Biz
        //        public void SaveReceiveAct(RenewInsuranceData data, string crateBy)
        //        {
        //            try
        //            {
        //                UpdateRenewinsuranceReceiveAct(data);

        //                string sql = @"SELECT COUNT(compare.slm_RenewInsureCompareId)
        //                                    FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance renew
        //                                    INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare compare ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
        //                                    WHERE renew.slm_TicketId = '" + data.slm_TicketId + "' AND compare.slm_Selected = 1 ";

        //                int countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
        //                if (countPolicy == 0)
        //                {
        //                    InsertLog(data, crateBy, SLM.Resource.SLMConstant.ActionType.ChangeStatus, SLM.Resource.SLMConstant.StatusCode.OnProcess, SLM.Resource.SLMConstant.SubStatusCode.InformAct);
        //                    UpdateLeadIncentive(data, SLM.Resource.SLMConstant.StatusCode.OnProcess, SLM.Resource.SLMConstant.SubStatusCode.InformAct);
        //                }

        //                //insertkkslm_tr_notify_renewinsurance_report(data, data.slm_TicketId, data.slm_UpdatedBy);

        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }

        //        }
        public void SavePolicyIncentiveData(RenewInsuranceData data, string createBy)
        {
            try
            {
                UpdateRenewinsurancePolicyIncentive(data);
                var oldAct = InsertLog(data, createBy, SLM.Resource.SLMConstant.ActionType.GetIncentiveIns, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);
                UpdateLeadIncentive(data, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);
            }
            catch
            {
                throw;
            }
        }

        public void SaveActIncentiveData(RenewInsuranceData data, string createBy)
        {
            try
            {
                UpdateRenewinsuranceActIncentive(data);
                InsertLog(data, createBy, SLM.Resource.SLMConstant.ActionType.GetIncentiveIns, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);
                UpdateLeadIncentive(data, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);
            }
            catch
            {
                throw;
            }
        }

        public void CancelPolicy(PreleadCompareDataCollection data)
        {
            CancelPolicy(data, true);
        }

        public void CancelPolicy(PreleadCompareDataCollection data, bool cancel)
        {
            bool revertReceive = false;
            bool revertIncentive = false;
            bool revertClaim = false;
            var RenewInsure = data.RenewIns;
            try
            {
                kkslm_tr_renewinsurance reins = slmdb.kkslm_tr_renewinsurance.Where(re => re.slm_RenewInsureId == RenewInsure.slm_RenewInsureId).FirstOrDefault();
                if (reins != null)
                {
                    if (cancel)
                    {
                        if (reins.slm_ClaimFlag != null) revertClaim = true;
                        reins.slm_PolicyCancelDate = RenewInsure.slm_PolicyCancelDate;
                        reins.slm_PolicyCancelId = RenewInsure.slm_PolicyCancelId;
                        reins.slm_ClaimFlag = RenewInsure.slm_ClaimFlag;
                        reins.slm_UpdatedBy = RenewInsure.slm_UpdatedBy;
                        reins.slm_UpdatedDate = RenewInsure.slm_UpdatedDate;
                    }
                    else
                    {
                        reins.slm_PolicyNo = null;
                        reins.slm_PolicyStartCoverDate = null;
                        reins.slm_PolicyEndCoverDate = null;
                        reins.slm_PolicyIssuePlace = null;
                        reins.slm_PolicyIssueBranch = null;
                        reins.slm_CoverageTypeId = null;
                        reins.slm_InsuranceComId = null;
                        reins.slm_PolicyDiscountAmt = null;
                        reins.slm_PolicyGrossVat = null;
                        reins.slm_PolicyGrossStamp = null;
                        reins.slm_PolicyGrossPremium = null;
                        reins.slm_PolicyGrossPremiumTotal = null;
                        reins.slm_PolicyCost = null;
                        reins.slm_RepairTypeId = null;
                        reins.slm_PolicyPayMethodId = null;
                        reins.slm_PolicyAmountPeriod = null;
                        reins.slm_Need_CreditFlag = RenewInsure.slm_Need_CreditFlag;
                        reins.slm_Need_50TawiFlag = RenewInsure.slm_Need_50TawiFlag;
                        reins.slm_Need_DriverLicenseFlag = null;
                        reins.slm_RemarkPayment = null;
                        reins.slm_PolicyCostSave = null;
                        reins.slm_Vat1Percent = null;
                        reins.slm_DiscountPercent = null;
                        reins.slm_Vat1PercentBath = null;
                        reins.slm_PayOptionId = null;
                        reins.slm_PayBranchCode = null;
                        reins.slm_ClaimFlag = RenewInsure.slm_ClaimFlag;
                    }
                }

                slmdb.SaveChanges();

                if (cancel)
                {
                    var renewInsReport = slmdb.kkslm_tr_notify_renewinsurance_report.FirstOrDefault(r => r.slm_TicketId == RenewInsure.slm_TicketId && r.is_Deleted == false);
                    if (renewInsReport != null)
                    {
                        renewInsReport.is_Deleted = true;
                        renewInsReport.slm_UpdatedBy = RenewInsure.slm_UpdatedBy;
                        renewInsReport.slm_UpdatedDate = RenewInsure.slm_UpdatedDate;
                    }
                    insertkkslm_tr_cancel_policy_report(reins.slm_TicketId, reins.slm_UpdatedBy);
                    deletekkslm_tr_setle_claim(reins.slm_TicketId, reins.slm_UpdatedBy);
                    // deletekkslm_tr_setle_claim_cancel_report(reins.slm_TicketId, reins.slm_UpdatedBy);

                    if (reins.slm_ActPayMethodId != null && reins.slm_ActPayMethodId.Value != 1)
                    {
                        deletekkslm_tr_bp_report(reins.slm_TicketId, reins.slm_UpdatedBy);
                    }
                    // 2.6.	ตรวจสอบว่ามีข้อมูลเตรียมออกรายงานขอออกกรมธรรม์ใหม่หรือไม่?
                    string sql = @"delete [dbo].[kkslm_tr_policynew_report] 
                                   where slm_TicketId = '" + RenewInsure.slm_TicketId + "' and (slm_Export_Flag = 0 or slm_Export_Flag is null)";
                    slmdb.ExecuteStoreCommand(sql);

                    var updatesql = @"update [dbo].[kkslm_tr_policynew_report] set is_Deleted = '1' 
                                      where slm_Export_Flag = '1' and slm_TicketId  = '" + RenewInsure.slm_TicketId + "'";
                    slmdb.ExecuteStoreCommand(updatesql);

                    sql = @"update [dbo].[kkslm_tr_renewinsurance_compare] 
                            set slm_Selected = null 
                            where slm_RenewInsureId = " + RenewInsure.slm_RenewInsureId;
                    slmdb.ExecuteStoreCommand(sql);
                    if (data.CompareCurr != null)
                        data.CompareCurr.slm_Selected = null;
                    if (data.ComparePrev != null)
                        data.ComparePrev.slm_Selected = null;
                    if (data.ComparePromoList.Count > 0 && data.ComparePromoList.FirstOrDefault(p => p.slm_Selected == true) != null)
                        data.ComparePromoList.FirstOrDefault(p => p.slm_Selected == true).slm_Selected = null;

                    reins = slmdb.kkslm_tr_renewinsurance.Where(re => re.slm_RenewInsureId == RenewInsure.slm_RenewInsureId).FirstOrDefault();

                    if (reins != null)
                    {
                        using (OPERDBEntities operdb = new OPERDBEntities())
                        {
                            string ticketId = reins.slm_TicketId;
                            string receiveNo = reins.slm_ReceiveNo;
                            var obj = operdb.kkslm_ms_receivenolist.Where(p => p.slm_TicketId == ticketId && p.slm_ReceiveNo == receiveNo).FirstOrDefault();
                            if (obj != null) obj.slm_Status = "2";

                            /*-----*/
                            reins.slm_PolicyStartCoverDate = null;
                            reins.slm_PolicyEndCoverDate = null;
                            reins.slm_CoverageTypeId = null;
                            reins.slm_InsuranceComId = null;
                            reins.slm_PolicyDiscountAmt = null;
                            reins.slm_PolicyGrossVat = null;
                            reins.slm_PolicyGrossStamp = null;
                            reins.slm_PolicyGrossPremium = null;
                            reins.slm_PolicyGrossPremiumTotal = null;
                            reins.slm_PolicyNetGrossPremium = null;
                            reins.slm_PolicyCost = null;
                            reins.slm_RepairTypeId = null;
                            reins.slm_PolicyPayMethodId = null;
                            reins.slm_PolicyAmountPeriod = null;
                            reins.slm_Need_CreditFlag = RenewInsure.slm_Need_CreditFlag;
                            reins.slm_Need_50TawiFlag = RenewInsure.slm_Need_50TawiFlag;
                            reins.slm_Need_DriverLicenseFlag = null;
                            reins.slm_RemarkPayment = null;
                            reins.slm_PolicyCostSave = null;
                            reins.slm_Vat1Percent = null;
                            reins.slm_DiscountPercent = null;
                            reins.slm_Vat1PercentBath = null;
                            reins.slm_PayOptionId = null;
                            reins.slm_PayBranchCode = null;
                            if (reins.slm_ReceiveDate != null)
                            {
                                revertReceive = true;
                            }
                            reins.slm_ReceiveDate = null;
                            reins.slm_ReceiveNo = null;
                            if (reins.slm_IncentiveFlag != null && reins.slm_IncentiveFlag.Value == true)
                            {
                                reins.slm_IncentiveCancelDate = RenewInsure.slm_PolicyCancelDate;
                            }
                            //reins.slm_IncentiveFlag = null;   จะทำการเคลียค่าก็ต่อเมื่อมี เดือน และ ปี เดี๊ยวกัน กับวันที่กด
                            //reins.slm_IncentiveDate = null;   จะทำการเคลียค่าก็ต่อเมื่อมี เดือน และ ปี เดี๊ยวกัน กับวันที่กด
                            reins.slm_GenSMSReceiveNo = null;
                            reins.slm_GenSMSReceiveNoDate = null;
                            reins.slm_Need_PolicyFlag = null;
                            reins.slm_Need_PolicyFlagDate = null;
                            if (reins.slm_IncentiveDate != null
                                && ((DateTime)reins.slm_IncentiveDate).Month == DateTime.Now.Month
                                && ((DateTime)reins.slm_IncentiveDate).Year == DateTime.Now.Year)
                            {
                                reins.slm_IncentiveDate = null;
                                reins.slm_IncentiveFlag = null;
                                reins.slm_PolicyComBath = RenewInsure.slm_PolicyComBath;
                                reins.slm_PolicyComBathVat = RenewInsure.slm_PolicyComBathVat;
                                reins.slm_PolicyComBathIncentive = RenewInsure.slm_PolicyComBathIncentive;
                                reins.slm_PolicyOV1Bath = RenewInsure.slm_PolicyOV1Bath;
                                reins.slm_PolicyOV1BathIncentive = RenewInsure.slm_PolicyOV1BathIncentive;
                                reins.slm_PolicyOV1BathVat = RenewInsure.slm_PolicyOV1BathVat;
                                reins.slm_PolicyOV2Bath = RenewInsure.slm_PolicyOV2Bath;
                                reins.slm_PolicyOV2BathIncentive = RenewInsure.slm_PolicyOV2BathIncentive;
                                reins.slm_PolicyOV2BathVat = RenewInsure.slm_PolicyOV2BathVat;
                                reins.slm_PolicyIncentiveAmount = RenewInsure.slm_PolicyIncentiveAmount;
                                reins.slm_IncentiveCancelDate = RenewInsure.slm_UpdatedDate;
                                reins.slm_PolicyReferenceNote = null;
                                reins.slm_UpdatedBy = RenewInsure.slm_UpdatedBy;
                                reins.slm_UpdatedDate = RenewInsure.slm_UpdatedDate;
                                revertIncentive = true;
                            }
                            operdb.SaveChanges();
                        }
                        var staff = slmdb.kkslm_ms_staff.FirstOrDefault(s => s.slm_UserName == RenewInsure.slm_UpdatedBy);
                        if (revertReceive)
                        {
                            // insert activity log Revert Receive No.
                            //var actRevertReceiveNo = new kkslm_tr_activity()
                            //{
                            //    slm_TicketId = reins.slm_TicketId,
                            //    slm_CreatedBy = RenewInsure.slm_UpdatedBy,
                            //    slm_CreatedBy_Position = staff.slm_Position_id,
                            //    slm_CreatedDate = DateTime.Now,
                            //    slm_Type = SLMConstant.ActionType.RevertReceiveNo, // "22", // (Revert Receive No)
                            //    slm_SystemAction = "SLM",
                            //    slm_SystemActionBy = "SLM",
                            //    slm_UpdatedBy = RenewInsure.slm_UpdatedBy,
                            //    slm_UpdatedDate = RenewInsure.slm_UpdatedDate
                            //};
                            //slmdb.kkslm_tr_activity.AddObject(actRevertReceiveNo);
                            InsertLog(RenewInsure.slm_TicketId, RenewInsure.slm_UpdatedBy, SLMConstant.ActionType.RevertReceiveNo, null, null, false);
                        }
                        if (revertClaim)
                        {
                            InsertLog(RenewInsure.slm_TicketId, RenewInsure.slm_UpdatedBy, SLMConstant.ActionType.RevertSettleClaim, null, null, false);
                        }
                        if (revertIncentive)
                        {
                            // 45.5 Owner Logging (Revert Incentive INS.)
                            //var actRevertIncentive = new kkslm_tr_activity()
                            //{
                            //    slm_TicketId = reins.slm_TicketId,
                            //    slm_CreatedBy = RenewInsure.slm_UpdatedBy,
                            //    slm_CreatedBy_Position = staff.slm_Position_id,
                            //    slm_CreatedDate = RenewInsure.slm_UpdatedDate,
                            //    slm_Type = SLMConstant.ActionType.RevertIncentiveIns, // "16", // (RevertRevert   Incentive INS.)
                            //    slm_SystemAction = "SLM",
                            //    slm_SystemActionBy = "SLM",
                            //    slm_UpdatedBy = RenewInsure.slm_UpdatedBy,
                            //    slm_UpdatedDate = RenewInsure.slm_UpdatedDate,
                            //};
                            //slmdb.kkslm_tr_activity.AddObject(actRevertIncentive);
                            var insAct = InsertLog(RenewInsure.slm_TicketId, RenewInsure.slm_UpdatedBy, SLMConstant.ActionType.RevertIncentiveIns, null, null, false);
                            kkslm_tr_activity updateInsActStatus = new kkslm_tr_activity();
                            var prevStatus = slmdb.kkslm_tr_activity.Where(l => l.slm_TicketId == renewInsReport.slm_TicketId && l.slm_NewStatus != null).OrderByDescending(l => l.slm_CreatedDate).FirstOrDefault();
                            var lastStatusLead = slmdb.kkslm_tr_lead.Where(e => e.slm_ticketId == renewInsReport.slm_TicketId).Select(e => e.slm_Status).FirstOrDefault();
                            if (prevStatus != null)
                            {
                                updateInsActStatus.slm_NewStatus = lastStatusLead; //prevStatus.slm_NewStatus;
                                updateInsActStatus.slm_NewSubStatus = prevStatus.slm_NewStatus;
                                updateInsActStatus.slm_OldStatus = prevStatus.slm_OldStatus;
                                updateInsActStatus.slm_OldSubStatus = prevStatus.slm_OldSubStatus;
                                updateInsActStatus.slm_ExternalSubStatus_New = prevStatus.slm_ExternalSubStatus_New;
                                updateInsActStatus.slm_CreatedBy = insAct.slm_CreatedBy;
                                updateInsActStatus.slm_CreatedBy_Position = insAct.slm_CreatedBy_Position;
                            }
                            insertPhoneCall(data, updateInsActStatus);
                        }
                    }
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void CancelAct(PreleadCompareDataCollection data)
        {
            CancelAct(data, true);
        }

        public void CancelAct(PreleadCompareDataCollection data, bool cancel)
        {
            try
            {
                bool hasActSendDate = false;
                RenewInsuranceData RenewInsure = data.RenewIns;
                kkslm_tr_renewinsurance reins = slmdb.kkslm_tr_renewinsurance.Where(re => re.slm_RenewInsureId == RenewInsure.slm_RenewInsureId).FirstOrDefault();
                if (reins != null)
                {
                    hasActSendDate = reins.slm_ActSendDate.HasValue;

                    if (cancel)
                    {
                        reins.slm_ActCancelDate = RenewInsure.slm_ActCancelDate;
                        reins.slm_ActCancelId = RenewInsure.slm_ActCancelId;
                        reins.slm_ClaimFlag = RenewInsure.slm_ClaimFlag;

                    }
                    else
                    {

                        reins.slm_ActPurchaseFlag = null;
                        reins.slm_ActStartCoverDate = null;
                        reins.slm_ActEndCoverDate = null;
                        reins.slm_ActIssuePlace = null;
                        reins.slm_ActIssueBranch = null;
                        reins.slm_ActGrossPremium = null;
                        reins.slm_ActComId = null;
                        reins.slm_ActNetPremium = null;
                        reins.slm_ActVat = null;
                        reins.slm_ActStamp = null;
                        reins.slm_ActPayMethodId = null;
                        reins.slm_ActamountPeriod = null;
                        reins.slm_ActPayOptionId = null;
                        reins.slm_ActPayBranchCode = null;
                        reins.slm_ActNo = null;
                        reins.slm_Need_CreditFlag = RenewInsure.slm_Need_CreditFlag;
                        reins.slm_Need_50TawiFlag = RenewInsure.slm_Need_50TawiFlag;
                        //reins.slm_Need_DriverLicenseFlag = RenewInsure.slm_Need_CreditFlag;
                        reins.slm_ActVat1Percent = null;
                        reins.slm_ActVat1PercentBath = null;
                        reins.slm_ActDiscountAmt = null;
                        reins.slm_ActDiscountPercent = null;
                    }

                }


                slmdb.SaveChanges();

                if (cancel)
                {
                    reins = slmdb.kkslm_tr_renewinsurance.Where(re => re.slm_RenewInsureId == RenewInsure.slm_RenewInsureId).FirstOrDefault();

                    if (hasActSendDate) // เชคว่าถ้าส่งแจ้งแล้ว ถึงออกรายงาน ยกเลิก
                        insertkkslm_tr_cancel_act_report(reins.slm_TicketId, reins.slm_UpdatedBy);

                    var notifyReport = slmdb.kkslm_tr_notify_act_report
                        .FirstOrDefault(r => r.slm_TicketId == reins.slm_TicketId && (r.is_Deleted ?? false) == false);
                    if (notifyReport != null)
                    {
                        notifyReport.is_Deleted = true;
                        notifyReport.slm_UpdatedBy = reins.slm_UpdatedBy;
                        notifyReport.slm_UpdatedDate = DateTime.Now;

                    }
                    // 2.5.	ตรวจสอบว่ามีข้อมูลเตรียมออกรายงานขอออกสำเนาพรบ.หรือไม่
                    var actnewreport = slmdb.kkslm_tr_actnew_report.FirstOrDefault(r => r.slm_TicketId == reins.slm_TicketId
                            && (r.is_Deleted ?? false) == false);
                    if (actnewreport != null)
                    {
                        reins.slm_Need_CompulsoryFlag = null;
                        reins.slm_Need_CompulsoryFlagDate = null;
                        reins.slm_UpdatedBy = RenewInsure.slm_UpdatedBy;
                        reins.slm_UpdatedDate = DateTime.Now;
                        slmdb.kkslm_tr_actnew_report.DeleteObject(actnewreport);
                    }

                    if (reins.slm_PayOptionId != null && reins.slm_PayOptionId.Value != 2)
                    {
                        deletekkslm_tr_setle_claim(reins.slm_TicketId, reins.slm_UpdatedBy);
                    }

                    // clear slm_ActPurchaseFlag
                    string sql = @"update [dbo].[kkslm_tr_renewinsurance_compare_act]
                                        set slm_ActPurchaseFlag = null
                                        where slm_RenewInsureId = " + RenewInsure.slm_RenewInsureId;
                    slmdb.ExecuteStoreCommand(sql);
                    if (data.ActPromoList != null && data.ActPromoList.FirstOrDefault(a => a.slm_ActPurchaseFlag == true) != null)
                        data.ActPromoList.FirstOrDefault(a => a.slm_ActPurchaseFlag == true).slm_ActPurchaseFlag = null;
                    if (data.ActPrev != null)
                        data.ActPrev.slm_ActPurchaseFlag = null;

                    #region clear data
                    if (reins != null)
                    {
                        reins.slm_ActNo = null;
                        reins.slm_ActPurchaseFlag = null;
                        reins.slm_ActStartCoverDate = null;
                        reins.slm_ActEndCoverDate = null;
                        reins.slm_ActIssuePlace = null;
                        reins.slm_ActIssueBranch = null;
                        reins.slm_ActGrossPremium = null;
                        reins.slm_ActComId = null;
                        reins.slm_ActNetPremium = null;
                        reins.slm_ActVat = null;
                        reins.slm_ActStamp = null;
                        reins.slm_ActPayMethodId = null;
                        reins.slm_ActamountPeriod = null;
                        reins.slm_ActPayOptionId = null;
                        reins.slm_ActPayBranchCode = null;
                        reins.slm_ActNo = null;
                        reins.slm_Need_CreditFlag = RenewInsure.slm_Need_CreditFlag;
                        reins.slm_Need_50TawiFlag = RenewInsure.slm_Need_50TawiFlag;
                        reins.slm_Need_DriverLicenseFlag = null;
                        reins.slm_ActVat1Percent = null;
                        reins.slm_ActVat1PercentBath = null;
                        reins.slm_ActDiscountAmt = null;
                        reins.slm_ActDiscountPercent = null;
                        reins.slm_ActSendDate = null;
                        if (reins.slm_ActIncentiveFlag != null && reins.slm_ActIncentiveFlag.Value == true)
                        {
                            reins.slm_ActIncentiveCancelDate = RenewInsure.slm_ActCancelDate;
                        }
                        // reins.slm_ActIncentiveFlag = null; IncentiveFlag เคลียก็ต่อเมื่อมีค่า ปี และ เดือน เดียวกับ วันที่ทำการกดยกเลิก
                        // reins.slm_ActIncentiveDate = null; IncentiveDate เคลียก็ต่อเมื่อมีค่า ปี และ เดือน เดียวกับ วันที่ทำการกดยกเลิก
                    }
                    #endregion clear data
                    slmdb.SaveChanges();

                    if (hasActSendDate)
                        InsertLog(RenewInsure.slm_TicketId, RenewInsure.slm_UpdatedBy, SLM.Resource.SLMConstant.ActionType.RevertActSendDate, null, null, false);
                    /*=========   CLEAR  INCENTIVE   ==========*/
                    #region clear incentive
                    if ((reins.slm_ActIncentiveDate ?? DateTime.MinValue).Month == DateTime.Now.Month
                        && (reins.slm_ActIncentiveDate ?? DateTime.MinValue).Year == DateTime.Now.Year)
                    {
                        reins.slm_ActIncentiveFlag = null;
                        reins.slm_ActIncentiveDate = null;
                        reins.slm_ActIncentiveCancelDate = DateTime.Now;
                        reins.slm_ActReferenceNote = null;
                        reins.slm_UpdatedBy = RenewInsure.slm_UpdatedBy;
                        reins.slm_UpdatedDate = DateTime.Now;
                        slmdb.SaveChanges();

                        var act = InsertLog(RenewInsure.slm_TicketId, RenewInsure.slm_UpdatedBy, SLM.Resource.SLMConstant.ActionType.RevertIncentiveAct, null, null, false);

                        insertPhoneCall(data, act);
                    }
                    #endregion clear incentive

                }
            }
            catch
            {
                throw;
            }
        }

        public int? getDiscountPercent(string username)
        {
            try
            {
                string sql = @"select disc.slm_DiscountPercent
                from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff left join kkslm_ms_discount disc on
                staff.slm_StaffTypeId = disc.slm_StaffTypeId 
                where staff.is_Deleted = 0 and  slm_UserName = '" + username + "'";
                return slmdb.ExecuteStoreQuery<int?>(sql).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public bool checkSaveButton(string slm_ProductId, string username)
        {
            try
            {
                //var staffType = (from s in slmdb.kkslm_ms_staff
                //                    join st in slmdb.kkslm_ms_staff_type on s.slm_StaffTypeId equals st.slm_StaffTypeId
                //                    where s.slm_UserName == username
                //                    select st).FirstOrDefault();

                string sql_staff = $@"SELECT st.*
                                    FROM kkslm_ms_staff s WITH (NOLOCK)
                                    INNER JOIN kkslm_ms_staff_type st WITH (NOLOCK) ON s.slm_StaffTypeId = st.slm_StaffTypeId
                                    WHERE s.slm_UserName = '{username}' ";

                var staffType = slmdb.ExecuteStoreQuery<kkslm_ms_staff_type>(sql_staff).FirstOrDefault();

                if (staffType == null)
                {
                    return false;
                }
                if (staffType.slm_RequiredLicenseNoFlag == null || staffType.slm_RequiredLicenseNoFlag == false)
                {
                    return true;
                }

                string sql = @"SELECT convert(varchar,'''' + slm_LicenseTypeId + '''') slm_LicenseTypeId
                        FROM dbo.kkslm_ms_mapping_product_license
                        where is_Deleted = 0 and slm_Product_Id = '" + slm_ProductId + "'";
                List<string> LicenseTypeId = slmdb.ExecuteStoreQuery<string>(sql).ToList();

                if (LicenseTypeId != null)
                {

                    if (LicenseTypeId.Count > 0)
                    {
                        var slm_LicenseTypeIdTostring = LicenseTypeId.Select(i => i).Aggregate((i, j) => i + "," + j);

                        string sql2 = @"SELECT COUNT(ST.slm_EmpCode)
                            FROM dbo.kkslm_ms_staff ST WITH (NOLOCK) 
                            INNER JOIN dbo.kkslm_ms_mapping_staff_license SL WITH (NOLOCK) ON SL.slm_EmpCode = ST.slm_EmpCode 
                            WHERE ST.is_Deleted = 0 AND SL.is_Deleted = 0 AND ST.slm_UserName = '" + username + "' AND CONVERT(DATE,GETDATE()) BETWEEN SL.slm_LicenseBeginDate AND SL.slm_LicenseExpireDate AND SL.slm_LicenseTypeId IN (" + slm_LicenseTypeIdTostring + ")";
                        int cEmpCode = slmdb.ExecuteStoreQuery<int>(sql2).FirstOrDefault();

                        if (cEmpCode == 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool checkProblem(string ticketId)
        {
            try
            {
                string sql = @"SELECT COUNT(*) 
                            FROM dbo.kkslm_tr_problemdetail p WITH (NOLOCK) 
                                   left join dbo.kkslm_tr_problemcomeback_report r WITH (NOLOCK) on p.slm_problemDetailId = r.slm_problemDetailId  
                            WHERE (slm_FixTypeFlag <> '2' or (isnull(slm_export_flag, 0) = 0 and slm_FixTypeFlag = '2'))
                                 AND p.slm_ticketId = '" + ticketId + "'";
                int cProblem = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                if (cProblem == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ControlListData> getCancelList()
        {
            try
            {
                List<ControlListData> list = null;


                list = slmdb.kkslm_ms_cancel.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_CancelName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_CancelName, ValueField = p.slm_CancelId.ToString() }).ToList();


                return list;
            }
            catch
            {
                throw;
            }
        }

        public bool checkPaid(string ticketId)
        {
            try
            {
                string sql1 = @"SELECT CASE WHEN slm_PolicyGrossPremium IS NULL THEN 0
                            WHEN slm_PolicyGrossPremium = 0.00 THEN 0 ELSE slm_PolicyGrossPremium  END + 
                            CASE WHEN slm_ActGrossPremium IS NULL THEN 0 WHEN slm_ActGrossPremium = 0.00 THEN 0 ELSE slm_ActGrossPremium END TOTALPAY
                            FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance 
                            WHERE slm_TicketId = '" + ticketId + "'";
                decimal dTOTALPAY = slmdb.ExecuteStoreQuery<decimal>(sql1).FirstOrDefault();

                string sql2 = @"SELECT isnull(slm_DiscountBath,0)
                        FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_discount
                         where slm_StaffTypeId = -1 and is_Deleted = 0";
                decimal DiscountBath = slmdb.ExecuteStoreQuery<decimal>(sql2).FirstOrDefault();

                string sql3 = @"select sum(isnull(rrd.slm_RecAmount,0)) RecAmount
                                from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt rr inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail rrd on rr.slm_RenewInsuranceReceiptId = rrd.slm_RenewInsuranceReceiptId
                                 where rr.slm_ticketId = '" + ticketId + "'"
                                 + " group by rr.slm_ticketId ";
                decimal dRecAmount = slmdb.ExecuteStoreQuery<decimal>(sql3).FirstOrDefault();

                if (dRecAmount >= dTOTALPAY - DiscountBath && dRecAmount <= dTOTALPAY + DiscountBath)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        public decimal GetPaidPolicyAmount(string ticketId)
        {
            decimal dRecAmount = 0;
            try
            {
                string sql3 = @"select sum(isnull(rrd.slm_RecAmount,0)) RecAmount
                                from dbo.kkslm_tr_renewinsurance_receipt rr WITH (NOLOCK) 
                                inner join dbo.kkslm_tr_renewinsurance_receipt_detail rrd WITH (NOLOCK) on rr.slm_RenewInsuranceReceiptId = rrd.slm_RenewInsuranceReceiptId
                                 where rrd.slm_PaymentCode='204' and rr.slm_ticketId = '" + ticketId + "'"
                                 + " group by rr.slm_ticketId ";
                dRecAmount = slmdb.ExecuteStoreQuery<decimal>(sql3).FirstOrDefault();
            }
            catch
            {
                throw;
            }
            return dRecAmount;
        }

        public bool checkPaidPolicy(string ticketId)
        {
            try
            {
                string sql1 = @"SELECT CASE WHEN slm_PolicyGrossPremium IS NULL THEN 0 WHEN slm_PolicyGrossPremium = 0.00 THEN 0 ELSE slm_PolicyGrossPremium END TOTALPAY
                            FROM dbo.kkslm_tr_renewinsurance WITH (NOLOCK) 
                            WHERE slm_TicketId = '" + ticketId + "'";
                decimal dTOTALPAY = slmdb.ExecuteStoreQuery<decimal>(sql1).FirstOrDefault();

                string sql2 = @"SELECT isnull(slm_DiscountBath,0)
                        FROM dbo.kkslm_ms_discount WITH (NOLOCK) 
                         where slm_StaffTypeId = -1 and is_Deleted = 0 and slm_insurancetypecode = '204'";
                decimal DiscountBath = slmdb.ExecuteStoreQuery<decimal>(sql2).FirstOrDefault();

                decimal dRecAmount = GetPaidPolicyAmount(ticketId);

                if (dRecAmount >= dTOTALPAY - DiscountBath && dRecAmount <= dTOTALPAY + DiscountBath)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        public decimal GetPaidActAmount(string ticketId)
        {
            decimal dRecAmount = 0;
            try
            {
                string sql3 = @"select sum(isnull(rrd.slm_RecAmount,0)) RecAmount
                                from dbo.kkslm_tr_renewinsurance_receipt rr WITH (NOLOCK) 
                                inner join dbo.kkslm_tr_renewinsurance_receipt_detail rrd WITH (NOLOCK) on rr.slm_RenewInsuranceReceiptId = rrd.slm_RenewInsuranceReceiptId
                                 where rrd.slm_PaymentCode='205' and rr.slm_ticketId = '" + ticketId + "'"
                                 + " group by rr.slm_ticketId ";
                dRecAmount = slmdb.ExecuteStoreQuery<decimal>(sql3).FirstOrDefault();
            }
            catch
            {
                throw;
            }
            return dRecAmount;
        }

        public bool checkPaidAct(string ticketId)
        {
            try
            {
                string sql1 = @"SELECT CASE WHEN slm_ActGrossPremium IS NULL THEN 0 WHEN slm_ActGrossPremium = 0.00 THEN 0 ELSE slm_ActGrossPremium END TOTALPAY
                            FROM dbo.kkslm_tr_renewinsurance WITH (NOLOCK) 
                            WHERE slm_TicketId = '" + ticketId + "'";
                decimal dTOTALPAY = slmdb.ExecuteStoreQuery<decimal>(sql1).FirstOrDefault();

                string sql2 = @"SELECT isnull(slm_DiscountBath,0)
                        FROM dbo.kkslm_ms_discount WITH (NOLOCK) 
                         where slm_StaffTypeId = -1 and is_Deleted = 0 and slm_insurancetypecode='205'";
                decimal DiscountBath = slmdb.ExecuteStoreQuery<decimal>(sql2).FirstOrDefault();

                decimal dRecAmount = GetPaidActAmount(ticketId);

                if (dRecAmount >= dTOTALPAY - DiscountBath && dRecAmount <= dTOTALPAY + DiscountBath)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        public void deletekkslm_tr_bp_report(string ticketId, string username)
        {
            try
            {
                //remove when found
                var sqldel = @"delete from [dbo].[kkslm_tr_bp_report]
                    where  convert(date,slm_createddate) = convert(date,getdate()) and slm_Export_Flag is null 
                    and slm_Contract_Number in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                slmdb.ExecuteStoreCommand(sqldel);
                //

            }
            catch
            {
                throw;
            }
        }

        public void insertkkslm_tr_bp_report(RenewInsuranceData renew, BPReportData bpReport, bool policy = true, bool act = true)
        {
            try
            {
                string createdBy = renew.slm_UpdatedBy;
                string ticketId = renew.slm_TicketId;
                Random rnd = new Random();
                int ranValue = rnd.Next(0, 999);
                string refNo = string.Format("{0}{1}{2}", DateTime.Now.Year.ToString(), DateTime.Now.ToString("MMddHHmmss"), ranValue.ToString("000"));

                //save policy data
                if (bpReport.IssuePolicy && bpReport.PolicyItemList != null)
                {
                    foreach (var item in bpReport.PolicyItemList)
                    {
                        kkslm_tr_bp_data bp = new kkslm_tr_bp_data
                        {
                            slm_RefNo = refNo,
                            slm_RenewInsureId = renew.slm_RenewInsureId,
                            slm_PolicyGrossPremium = bpReport.PolicyAmountDue,
                            slm_Type = "P",
                            slm_PaymentMethodId = bpReport.PolicyPaymentMethodId,
                            slm_Installment = item.No,
                            slm_Amount = item.Amount,
                            slm_CreatedBy = createdBy,
                            slm_CreatedDate = DateTime.Now,
                            slm_UpdatedBy = createdBy,
                            slm_UpdatedDate = DateTime.Now,
                            is_Deleted = false
                        };
                        slmdb.kkslm_tr_bp_data.AddObject(bp);
                    }
                }
                
                //save act data
                if (bpReport.IssueAct && bpReport.ActItem != null)
                {
                    kkslm_tr_bp_data bpAct = new kkslm_tr_bp_data
                    {
                        slm_RefNo = refNo,
                        slm_RenewInsureId = renew.slm_RenewInsureId,
                        slm_ActGrossPremium = bpReport.ActAmountDue,
                        slm_Type = "A",
                        slm_PaymentMethodId = bpReport.ActPaymentMethodId,
                        slm_Installment = bpReport.ActItem.No,
                        slm_Amount = bpReport.ActItem.Amount,
                        slm_CreatedBy = createdBy,
                        slm_CreatedDate = DateTime.Now,
                        slm_UpdatedBy = createdBy,
                        slm_UpdatedDate = DateTime.Now,
                        is_Deleted = false
                    };
                    slmdb.kkslm_tr_bp_data.AddObject(bpAct);
                }

                slmdb.SaveChanges();

                //remove when found
                var sqldel = @"delete from [dbo].[kkslm_tr_bp_report]
                    where  (slm_Export_Flag is null or slm_Export_Flag = 0) 
                    and slm_Contract_Number in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                slmdb.ExecuteStoreCommand(sqldel);

                #region Backup Insert Policy
                //Insert Policy
                //var sql = @"insert into kkslm_tr_bp_report([slm_Contract_Number]      ,[slm_Title_Id]
                //              ,[slm_Name]      ,[slm_LastName]      ,[slm_House_No]      ,[slm_Moo]
                //              ,[slm_Building]      ,[slm_Village]      ,[slm_Soi]      ,[slm_Street]
                //              ,[slm_TambolId]      ,[slm_AmphurId]      ,[slm_ProvinceId]      ,[slm_Zipcode]
                //              ,[slm_Address_Type]      ,[slm_InsCost]      ,[slm_ActCost]      ,[slm_Send_Type]
                //              ,[slm_Owner]      ,[slm_Export_Flag]      ,[slm_Export_Date]      ,[slm_CreatedBy]
                //              ,[slm_CreatedDate]      ,[slm_UpdatedBy]      ,[slm_UpdatedDate] , [slm_Reference1], [slm_Reference2])
                //            SELECT  REN.slm_ContractNo AS slm_Contract_Number,
                //                LEAD.slm_TitleId ,LEAD.slm_Name,LEAD.slm_LastName,RENA.slm_House_No, RENA.SLM_MOO,
                //                RENA.slm_BuildingName slm_Building,RENA.slm_Village,RENA.slm_Soi,RENA.slm_Street,RENA.slm_Tambon slm_TambolId,
                //                RENA.slm_Amphur slm_AmphurId,RENA.slm_Province slm_ProvinceId,RENA.slm_PostalCode AS slm_Zipcode, 
                //                CASE WHEN REN.slm_SendDocFlag = '2' THEN '001' 
                //                WHEN REN.slm_SendDocFlag = '1' THEN '002' 
                //                WHEN REN.slm_SendDocFlag = '3' THEN '002' ELSE '' END slm_Address_Type,
                //                    CASE WHEN REN.slm_PolicyPayMethodId = '1' AND REN.slm_PayOptionId <> '2' THEN REN.slm_PolicyGrossPremium
                //                  WHEN REN.slm_PolicyPayMethodId = '1' AND REN.slm_PayOptionId = '2' THEN PM.slm_PaymentAmount  END AS slm_InsCost,
                //             NULL AS slm_ActCost, NULL AS slm_Send_Type,
                //                ST.slm_EmpCode AS slm_Owner,null slm_Export_Flag,null slm_Export_Date,
                //                '" + createdBy + @"' slm_CreatedBy, getdate() as slm_CreatedDate, '" + createdBy + @"' slm_UpdatedBy, getdate() as slm_UpdatedBy,
                //                REN.slm_Reference1, REN.slm_Reference2 
                //            FROM kkslm_tr_lead LEAD 
                //            INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                //            LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner AND ST.is_Deleted = 0
                //            LEFT JOIN kkslm_tr_renewinsurance_address RENA ON RENA.slm_RenewInsureId = REN.slm_RenewInsureId AND RENA.slm_AddressType = 'D'
                //            LEFT JOIN kkslm_tr_renewinsurance_paymentmain PM ON PM.slm_RenewInsureId = REN.slm_RenewInsureId 
                //            WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status not in ('08','09','10') and REN.slm_ContractNo is not null and lead.slm_ticketId= '" + ticketId + "'";
                #endregion

                var sql = @"insert into kkslm_tr_bp_report([slm_Contract_Number]      ,[slm_Title_Id]
                              ,[slm_Name]      ,[slm_LastName]      ,[slm_House_No]      ,[slm_Moo]
                              ,[slm_Building]      ,[slm_Village]      ,[slm_Soi]      ,[slm_Street]
                              ,[slm_TambolId]      ,[slm_AmphurId]      ,[slm_ProvinceId]      ,[slm_Zipcode]
                              ,[slm_Address_Type]      ,[slm_InsCost]      ,[slm_ActCost]      ,[slm_Send_Type]
                              ,[slm_Owner]      ,[slm_Export_Flag]      ,[slm_Export_Date]      ,[slm_CreatedBy]
                              ,[slm_CreatedDate]      ,[slm_UpdatedBy]      ,[slm_UpdatedDate] , [slm_Reference1], [slm_Reference2], [slm_BPDataId])
                            SELECT  REN.slm_ContractNo AS slm_Contract_Number,
                                LEAD.slm_TitleId ,LEAD.slm_Name,LEAD.slm_LastName,RENA.slm_House_No, RENA.SLM_MOO,
                                RENA.slm_BuildingName slm_Building,RENA.slm_Village,RENA.slm_Soi,RENA.slm_Street,RENA.slm_Tambon slm_TambolId,
                                RENA.slm_Amphur slm_AmphurId,RENA.slm_Province slm_ProvinceId,RENA.slm_PostalCode AS slm_Zipcode, 
                                CASE WHEN REN.slm_SendDocFlag = '2' THEN '001' 
                                WHEN REN.slm_SendDocFlag = '1' THEN '002' 
                                WHEN REN.slm_SendDocFlag = '3' THEN '002' ELSE '' END slm_Address_Type,
                                bp.slm_Amount  AS slm_InsCost,
	                            NULL AS slm_ActCost, NULL AS slm_Send_Type,
                                ST.slm_EmpCode AS slm_Owner,null slm_Export_Flag,null slm_Export_Date,
                                '" + createdBy + @"' slm_CreatedBy, getdate() as slm_CreatedDate, '" + createdBy + @"' slm_UpdatedBy, getdate() as slm_UpdatedBy,
                                REN.slm_Reference1, 
                                CASE WHEN REN.slm_Reference2 IS NULL OR LTRIM(RTRIM(REN.slm_Reference2)) = '' THEN NULL
								ELSE '5' + REN.slm_Reference2 END AS slm_Reference2, 
								bp.slm_Id AS slm_BPDataId 
                            FROM kkslm_tr_lead LEAD 
                            INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                            LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner AND ST.is_Deleted = 0
                            LEFT JOIN kkslm_tr_renewinsurance_address RENA ON RENA.slm_RenewInsureId = REN.slm_RenewInsureId AND RENA.slm_AddressType = 'D'
                            LEFT JOIN kkslm_tr_bp_data bp ON bp.slm_RenewInsureId = REN.slm_RenewInsureId 
                            WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status not in ('08','09','10') and REN.slm_ContractNo is not null 
                            AND bp.slm_RefNo = '" + refNo + @"' AND bp.slm_Type = 'P' AND bp.slm_Amount <> 0 AND bp.is_Deleted = 0 
                            AND lead.slm_ticketId= '" + ticketId + "'";

                if (policy)
                {
                    slmdb.ExecuteStoreCommand(sql);
                }

                #region Backup Insert Act
                //Insert Act
                //sql = @"insert into kkslm_tr_bp_report([slm_Contract_Number]      ,[slm_Title_Id]
                //            ,[slm_Name]      ,[slm_LastName]      ,[slm_House_No]      ,[slm_Moo]
                //            ,[slm_Building]      ,[slm_Village]      ,[slm_Soi]      ,[slm_Street]
                //            ,[slm_TambolId]      ,[slm_AmphurId]      ,[slm_ProvinceId]      ,[slm_Zipcode]
                //            ,[slm_Address_Type]      ,[slm_InsCost]      ,[slm_ActCost]      ,[slm_Send_Type]
                //            ,[slm_Owner]      ,[slm_Export_Flag]      ,[slm_Export_Date]      ,[slm_CreatedBy]
                //            ,[slm_CreatedDate]      ,[slm_UpdatedBy]      ,[slm_UpdatedDate], [slm_Reference1], [slm_Reference2])
                //        SELECT  REN.slm_ContractNo AS slm_Contract_Number,
                //            LEAD.slm_TitleId ,LEAD.slm_Name,LEAD.slm_LastName,RENA.slm_House_No, RENA.SLM_MOO,
                //            RENA.slm_BuildingName slm_Building,RENA.slm_Village,RENA.slm_Soi,RENA.slm_Street,RENA.slm_Tambon slm_TambolId,
                //            RENA.slm_Amphur slm_AmphurId,RENA.slm_Province slm_ProvinceId,RENA.slm_PostalCode AS slm_Zipcode, 
                //            CASE WHEN REN.slm_SendDocFlag = '2' THEN '001' 
                //            WHEN REN.slm_SendDocFlag = '1' THEN '002' 
                //            WHEN REN.slm_SendDocFlag = '3' THEN '002' ELSE '' END slm_Address_Type,
                //         NULL AS slm_InsCost,
                //         CASE WHEN REN.slm_ActPayMethodId = '1' THEN REN.slm_ActGrossPremium ELSE NULL END AS slm_ActCost,
                //            CASE WHEN RCA.slm_SendDocType = '1' THEN '001'
                //            WHEN RCA.slm_SendDocType = '2' THEN '002' ELSE '' END slm_Send_Type,
                //            ST.slm_EmpCode AS slm_Owner, null slm_Export_Flag, null slm_Export_Date,
                //            '" + createdBy + @"' slm_CreatedBy, getdate() as slm_CreatedDate, '" + createdBy + @"' slm_UpdatedBy,getdate() as slm_UpdatedBy,
                //            REN.slm_Reference1, REN.slm_Reference2 
                //        FROM kkslm_tr_lead LEAD 
                //        INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                //        LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner AND ST.is_Deleted = 0
                //        LEFT JOIN kkslm_tr_renewinsurance_address RENA ON RENA.slm_RenewInsureId = REN.slm_RenewInsureId AND RENA.slm_AddressType = 'D'
                //        LEFT JOIN kkslm_tr_renewinsurance_compare_act RCA ON RCA.slm_RenewInsureId =REN.slm_RenewInsureId AND RCA.slm_ActPurchaseFlag = 1
                //        WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status not in ('08','09','10') and REN.slm_ContractNo is not null and lead.slm_ticketId= '" + ticketId + "'";
                #endregion

                sql = @"insert into kkslm_tr_bp_report([slm_Contract_Number]      ,[slm_Title_Id]
                            ,[slm_Name]      ,[slm_LastName]      ,[slm_House_No]      ,[slm_Moo]
                            ,[slm_Building]      ,[slm_Village]      ,[slm_Soi]      ,[slm_Street]
                            ,[slm_TambolId]      ,[slm_AmphurId]      ,[slm_ProvinceId]      ,[slm_Zipcode]
                            ,[slm_Address_Type]      ,[slm_InsCost]      ,[slm_ActCost]      ,[slm_Send_Type]
                            ,[slm_Owner]      ,[slm_Export_Flag]      ,[slm_Export_Date]      ,[slm_CreatedBy]
                            ,[slm_CreatedDate]      ,[slm_UpdatedBy]      ,[slm_UpdatedDate], [slm_Reference1], [slm_Reference2], [slm_BPDataId])
                        SELECT  REN.slm_ContractNo AS slm_Contract_Number,
                            LEAD.slm_TitleId ,LEAD.slm_Name,LEAD.slm_LastName,RENA.slm_House_No, RENA.SLM_MOO,
                            RENA.slm_BuildingName slm_Building,RENA.slm_Village,RENA.slm_Soi,RENA.slm_Street,RENA.slm_Tambon slm_TambolId,
                            RENA.slm_Amphur slm_AmphurId,RENA.slm_Province slm_ProvinceId,RENA.slm_PostalCode AS slm_Zipcode, 
                            CASE WHEN REN.slm_SendDocFlag = '2' THEN '001' 
                            WHEN REN.slm_SendDocFlag = '1' THEN '002' 
                            WHEN REN.slm_SendDocFlag = '3' THEN '002' ELSE '' END slm_Address_Type,
	                        NULL AS slm_InsCost,
	                        bp.slm_Amount AS slm_ActCost,
                            CASE WHEN RCA.slm_SendDocType = '1' THEN '001'
                            WHEN RCA.slm_SendDocType = '2' THEN '002' ELSE '' END slm_Send_Type,
                            ST.slm_EmpCode AS slm_Owner, null slm_Export_Flag, null slm_Export_Date,
                            '" + createdBy + @"' slm_CreatedBy, getdate() as slm_CreatedDate, '" + createdBy + @"' slm_UpdatedBy,getdate() as slm_UpdatedBy,
                            REN.slm_Reference1, 
                            CASE WHEN REN.slm_Reference2 IS NULL OR LTRIM(RTRIM(REN.slm_Reference2)) = '' THEN NULL
	                        ELSE '6' + REN.slm_Reference2 END AS slm_Reference2, 
	                        bp.slm_Id AS slm_BPDataId 
                        FROM kkslm_tr_lead LEAD 
                        INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                        LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner AND ST.is_Deleted = 0
                        LEFT JOIN kkslm_tr_renewinsurance_address RENA ON RENA.slm_RenewInsureId = REN.slm_RenewInsureId AND RENA.slm_AddressType = 'D'
                        LEFT JOIN kkslm_tr_renewinsurance_compare_act RCA ON RCA.slm_RenewInsureId =REN.slm_RenewInsureId AND RCA.slm_ActPurchaseFlag = 1
                        LEFT JOIN kkslm_tr_bp_data bp ON bp.slm_RenewInsureId = REN.slm_RenewInsureId 
                        WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status not in ('08','09','10') and REN.slm_ContractNo is not null 
                        AND bp.slm_RefNo = '" + refNo + @"' AND bp.slm_Type = 'A' AND bp.slm_Amount <> 0 AND bp.is_Deleted = 0 
                        AND lead.slm_ticketId= '" + ticketId + "'";

                if (act)
                {
                    slmdb.ExecuteStoreCommand(sql);
                }
            }
            catch
            {
                throw;
            }
        }

        public void insertkkslm_tr_notify_renewinsurance_report(RenewInsuranceData renew_ins, string ticketId, string username)
        {
            try
            {

                //remove when found
                //                var sqldel = @"delete from [dbo].[kkslm_tr_notify_renewinsurance_report]
                //                    where  convert(date,slm_createddate) = convert(date,getdate()) and (slm_Export_Flag is null or slm_export_flag = 0)
                //                    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                //                slmdb.ExecuteStoreCommand(sqldel);
                //

                var data = (from a in slmdb.kkslm_tr_lead
                            join b in slmdb.kkslm_tr_cusinfo on a.slm_ticketId equals b.slm_TicketId
                            join c in slmdb.kkslm_tr_productinfo on a.slm_ticketId equals c.slm_TicketId
                            where a.slm_ticketId == ticketId
                            select new
                            {
                                slm_TitleId = a.slm_TitleId,
                                slm_Name = a.slm_Name,
                                slm_LastName = a.slm_LastName,
                                slm_Owner = a.slm_Owner,
                                slm_TelNo_1 = a.slm_TelNo_1,
                                CitizenId = b.slm_CitizenId,
                                CardType = b.slm_CardType,
                                BirthDate = b.slm_Birthdate,
                                Occupation = b.slm_Occupation,
                                MaritalStatus = b.slm_MaritalStatus,
                                CarType = c.slm_CarType,
                                ProvinceRegis = c.slm_ProvinceRegis
                            }).FirstOrDefault();

                var renew_addr = slmdb.kkslm_tr_renewinsurance_address.Where(p => p.slm_RenewInsureId == renew_ins.slm_RenewInsureId && p.slm_AddressType == "D").FirstOrDefault();
                var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_TicketId == ticketId).FirstOrDefault();

                string sql = @"SELECT cp.slm_OldPolicyNo AS OldPolicyNo,  ci.slm_CampaignName AS CampaignName, cpOld.slm_FT as SumInsured_Year1
                                    ,cpOld.slm_NetGrossPremium as NetPremium_Year1, cp.slm_OD as SumInsured_Year2
                                    ,cp.slm_NetGrossPremium as NetPremium_Year2, cp.slm_Driver_Birthdate1 as Birthdate_Driver1
                                    ,cp.slm_Driver_Birthdate2 as Birthdate_Driver2,cpNotice.slm_Discount_Amount as DiscountGoodHistory
                                    ,cp.FullName_Driver1, cp.FullName_Driver2, renew.slm_CoverageTypeId AS CoverageTypeId
	                                ,cp.slm_FT AS SumInsured_Year2_FT, cpNotice.slm_Discount_Percent AS DiscountFeed
                                    , renew.slm_ReceiveNo AS ReceiveNo, renew.slm_ReceiveDate AS ReceiveDate, renew.slm_InsuranceComId AS InsuranceComId
	                                , renew.slm_ContractNo AS ContractNo, renew.slm_PolicyStartCoverDate AS PolicyStartCoverDate
	                                , renew.slm_PolicyEndCoverDate AS PolicyEndCoverDate, renew.slm_RepairTypeId AS RepairTypeId, renew.slm_RedbookBrandCode AS RedbookBrandCode
	                                , renew.slm_RedbookModelCode AS RedbookModelCode, renew.slm_RedbookKKKey AS RedbookKKKey, renew.slm_LicenseNo AS LicenseNo
	                                , renew.slm_ChassisNo AS ChassisNo, renew.slm_EngineNo AS EngineNo, renew.slm_RedbookYearGroup AS RedbookYearGroup, renew.slm_CC AS CC
	                                , renew.slm_RemarkPolicy AS RemarkPolicy, renew.slm_InsurancecarTypeId as InsuranceCarTypeId
                                FROM dbo.kkslm_tr_renewinsurance renew
                                LEFT JOIN 
	                                (SELECT slm_OldPolicyNo,slm_RenewInsureId,slm_PromotionId,slm_FT, slm_OD,slm_NetGrossPremium,slm_Driver_Birthdate1,
	                                    slm_Driver_Birthdate2, ISNULL(T1.slm_TitleName, '') + ISNULL(RC.slm_Driver_First_Name1, '') +' '+ ISNULL(RC.slm_Driver_Last_Name1, '') AS FullName_Driver1,
	                                    ISNULL(T2.slm_TitleName, '') + ISNULL(RC.slm_Driver_First_Name2, '') + ' ' + ISNULL(RC.slm_Driver_Last_Name2, '')  AS FullName_Driver2
	                                FROM  kkslm_tr_renewinsurance_compare RC 
                                    LEFT JOIN KKSLM_MS_TITLE T1 ON T1.slm_TitleId = RC.slm_Driver_TitleId1
	                                LEFT JOIN KKSLM_MS_TITLE T2 ON T2.slm_TitleId = RC.slm_Driver_TitleId2
	                                WHERE slm_Selected = '1') as cp ON cp.slm_RenewInsureId = renew.slm_RenewInsureId
                                LEFT JOIN 
	                                (SELECT slm_OldPolicyNo,slm_RenewInsureId,slm_PromotionId,slm_FT,slm_NetGrossPremium
	                                FROM  kkslm_tr_renewinsurance_compare 
	                                Where slm_Seq = 1 and slm_NotifyPremiumId is null and slm_PromotionId is null ) AS cpOld ON cpOld.slm_RenewInsureId = renew.slm_RenewInsureId
                                LEFT JOIN 
	                                (SELECT rc.slm_RenewInsureId,np.slm_Discount_Amount, np.slm_Discount_Percent
	                                FROM  kkslm_tr_renewinsurance_compare rc inner join kkslm_tr_notify_premium np on np.slm_Id = rc.slm_NotifyPremiumId
	                                WHERE slm_Seq = 2 and slm_NotifyPremiumId is not null and slm_PromotionId is null) as cpNotice ON cpNotice.slm_RenewInsureId = renew.slm_RenewInsureId 
                                LEFT JOIN " + SLMConstant.OPERDBName + @".dbo.kkslm_ms_promotioninsurance pro ON pro.slm_PromotionId = cp.slm_PromotionId
                                LEFT JOIN " + SLMConstant.OPERDBName + @".dbo.kkslm_ms_campaigninsurance ci ON ci.slm_CampaignInsuranceId = pro.slm_CampaignInsuranceId
                                WHERE renew.slm_RenewInsureId = " + renew_ins.slm_RenewInsureId + @" ";

                var data2 = slmdb.ExecuteStoreQuery<ReportData>(sql).FirstOrDefault();

                kkslm_tr_notify_renewinsurance_report report = new kkslm_tr_notify_renewinsurance_report()
                {
                    slm_InsReceiveNo = data2.ReceiveNo,         //renew_ins.slm_ReceiveNo,
                    slm_InsReceiveDate = data2.ReceiveDate,     //renew_ins.slm_ReceiveDate,
                    slm_Ins_Com_Id = data2.InsuranceComId,      //renew_ins.slm_InsuranceComId,
                    slm_Contract_Number = data2.ContractNo,     //renew_ins.slm_ContractNo,
                    //slm_PolicyNoOld = data2 != null ? data2.OldPolicyNo : null,
                    slm_PolicyNoOld = prelead != null ? prelead.slm_Voluntary_Policy_Number : null,
                    slm_BranchCode = prelead != null ? prelead.slm_BranchCode : null,
                    slm_CardTypeId = data != null ? data.CardType : null,
                    slm_CampaignName = data2 != null ? data2.CampaignName : null,
                    slm_SubCampaignName = null,
                    slm_Title_Id = data.slm_TitleId,
                    slm_Name = data.slm_Name,
                    slm_LastName = data.slm_LastName,
                    slm_House_No = renew_addr != null ? renew_addr.slm_House_No : null,
                    slm_Moo = renew_addr != null ? renew_addr.slm_Moo : null,
                    slm_Village = renew_addr != null ? renew_addr.slm_Village : null,
                    slm_Building = renew_addr != null ? renew_addr.slm_BuildingName : null,
                    slm_Soi = renew_addr != null ? renew_addr.slm_Soi : null,
                    slm_Street = renew_addr != null ? renew_addr.slm_Street : null,
                    slm_TambolId = renew_addr != null ? renew_addr.slm_Tambon : null,
                    slm_AmphurId = renew_addr != null ? renew_addr.slm_Amphur : null,
                    slm_ProvinceId = renew_addr != null ? renew_addr.slm_Province : null,
                    slm_Zipcode = renew_addr != null ? renew_addr.slm_PostalCode : null,
                    slm_CoverageTypeId = data2.CoverageTypeId,          //renew_ins.slm_CoverageTypeId,
                    slm_CoverageStartDate = data2.PolicyStartCoverDate, //renew_ins.slm_PolicyStartCoverDate,
                    slm_CoverageEndDate = data2.PolicyEndCoverDate,     //renew_ins.slm_PolicyEndCoverDate,
                    slm_RepairTypeId = data2.RepairTypeId,              //renew_ins.slm_RepairTypeId,
                    slm_VehicleNo = "",
                    //slm_InsuranceCarTypeId = prelead != null ? prelead.slm_Car_By_Gov_Id : null,
                    slm_InsuranceCarTypeId = data2 != null ? data2.InsuranceCarTypeId : null,
                    slm_BrandCode = data2.RedbookBrandCode,         //renew_ins.slm_RedbookBrandCode,
                    slm_ModelCode = data2.RedbookModelCode,         //renew_ins.slm_RedbookModelCode,
                    slm_KKKey = data2.RedbookKKKey,                 //renew_ins.slm_RedbookKKKey,
                    slm_CarLicenseNo = data2.LicenseNo + (data != null ? (" " + GetProvinceName(data.ProvinceRegis)) : ""),
                    slm_VIN = data2.ChassisNo,                      //renew_ins.slm_ChassisNo,
                    slm_EngineNo = data2.EngineNo,                  //renew_ins.slm_EngineNo,
                    slm_ModelYearId = data2.RedbookYearGroup,       //renew_ins.slm_RedbookYearGroup,
                    slm_Cc = data2.CC,                              //renew_ins.slm_CC,
                    slm_WeightPerTon = null,
                    slm_Owner = data.slm_Owner,
                    slm_Remark = data2.RemarkPolicy,                //renew_ins != null ? renew_ins.slm_RemarkPolicy : null,
                    slm_Discouont_Good_History = data2 != null ? data2.DiscountGoodHistory : null,
                    slm_Discount_Fleet = data2 != null ? data2.DiscountFeed : null,
                    slm_TelNo_1 = data.slm_TelNo_1,
                    slm_CitizenId = data != null ? data.CitizenId : null,
                    slm_Birthdate = data != null ? data.BirthDate : null,
                    slm_Occupation = data != null ? data.Occupation : null,
                    slm_MaritalStatus = data != null ? data.MaritalStatus : null,
                    slm_TaxNo = prelead != null ? prelead.slm_Tax_Id : null,
                    slm_Title_Id_Committee1 = prelead != null ? prelead.slm_Guarantor_TitleId1 : null,
                    slm_Name_Committee1 = prelead != null ? prelead.slm_Guarantor_First_Name1 : null,
                    slm_LastName_Committee1 = prelead != null ? prelead.slm_Guarantor_Last_Name1 : null,
                    slm_CitizenId_Committe1 = prelead != null ? prelead.slm_Guarantor_Card_Id1 : null,
                    slm_Title_Id_Committee2 = prelead != null ? prelead.slm_Guarantor_TitleId2 : null,
                    slm_Name_Committee2 = prelead != null ? prelead.slm_Guarantor_First_Name2 : null,
                    slm_LastName_Committee2 = prelead != null ? prelead.slm_Guarantor_Last_Name2 : null,
                    slm_CitizenId_Committe2 = prelead != null ? prelead.slm_Guarantor_Card_Id2 : null,
                    slm_Title_Id_Committee3 = prelead != null ? prelead.slm_Guarantor_TitleId3 : null,
                    slm_Name_Committee3 = prelead != null ? prelead.slm_Guarantor_First_Name3 : null,
                    slm_LastName_Committee3 = prelead != null ? prelead.slm_Guarantor_Last_Name3 : null,
                    slm_CitizenId_Committe3 = prelead != null ? prelead.slm_Guarantor_Card_Id3 : null,
                    slm_TicketId = renew_ins.slm_TicketId,
                    is_Deleted = false,
                    slm_Export_Flag = false,
                    slm_Export_Date = null,
                    slm_CreatedBy = username,
                    slm_CreatedDate = DateTime.Now,
                    slm_UpdatedBy = username,
                    slm_UpdatedDate = DateTime.Now
                };

                if (prelead != null && prelead.slm_Voluntary_Cov_Amt != null)
                    report.slm_SumInsured_Year1 = prelead.slm_Voluntary_Cov_Amt;
                if (prelead != null && prelead.slm_Voluntary_Gross_Premium != null)
                    report.slm_NetPremium_Year1 = prelead.slm_Voluntary_Gross_Premium;

                var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                if (data != null)
                {
                    if (data.CarType == "0")
                        report.slm_Cartype = "New";
                    else if (data.CarType == "1")
                        report.slm_Cartype = "Used";

                    if (data.CardType == 2)
                    {
                        if (lead != null) report.slm_Title_Id_Receipt = lead.slm_TitleId;
                        if (lead != null) report.slm_Name_Receipt = lead.slm_Name;
                        if (renew_addr != null)
                        {
                            report.slm_House_No_Receipt = renew_addr.slm_House_No;
                            report.slm_Village_Receipt = renew_addr.slm_Village;
                            report.slm_Building_Receipt = renew_addr.slm_BuildingName;
                            report.slm_Soi_Receipt = renew_addr.slm_Soi;
                            report.slm_Street_Receipt = renew_addr.slm_Street;
                            report.slm_TambolId_Receipt = renew_addr.slm_Tambon;
                            report.slm_AmphurId_Receipt = renew_addr.slm_Amphur;
                            report.slm_ProvinceId_Receipt = renew_addr.slm_Province;
                            report.slm_Zipcode_Receipt = renew_addr.slm_PostalCode;
                        }
                    }
                }
                if (data2 != null)
                {
                    if (data2.SumInsured_Year2 != null) report.slm_SumInsured_Year2 = data2.SumInsured_Year2;
                    if (data2.SumInsured_Year2_FT != null) report.slm_SumInsured_Year2_FT = data2.SumInsured_Year2_FT;
                    if (data2.NetPremium_Year2 != null) report.slm_NetPremium_Year2 = data2.NetPremium_Year2;

                    report.slm_Birthdate_Driver1 = (string.IsNullOrEmpty(data2.FullName_Driver1) ? "" : data2.FullName_Driver1 + " ") + (data2.Birthdate_Driver1 != null ? (data2.Birthdate_Driver1.Value.ToString("dd/MM/yyyy", new CultureInfo("th-TH"))) : "");
                    report.slm_Birthdate_Driver2 = (string.IsNullOrEmpty(data2.FullName_Driver2) ? "" : data2.FullName_Driver2 + " ") + (data2.Birthdate_Driver2 != null ? (data2.Birthdate_Driver2.Value.ToString("dd/MM/yyyy", new CultureInfo("th-TH"))) : "");
                }

                var tran_date = (from r in slmdb.kkslm_tr_renewinsurance_receipt
                                 join d in slmdb.kkslm_tr_renewinsurance_receipt_detail on r.slm_RenewInsuranceReceiptId equals d.slm_RenewInsuranceReceiptId
                                 where r.slm_ticketId == ticketId
                                 orderby d.slm_RenewInsuranceReceiptDetailId descending
                                 select d.slm_TransDate).FirstOrDefault();

                if (tran_date != null)
                    report.slm_InsPayDate = tran_date;

                slmdb.kkslm_tr_notify_renewinsurance_report.AddObject(report);

                slmdb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        private string GetProvinceName(int? provinceId)
        {
            var name = slmdb.kkslm_ms_province.Where(p => p.slm_ProvinceId == provinceId).Select(p => p.slm_ProvinceNameTH).FirstOrDefault();
            return name != null ? name : "";
        }

        public void deletekkslm_tr_setle_claim(string ticketId, string username)
        {
            try
            {
                //remove when found
                var sqldel = @"delete from [dbo].[kkslm_tr_setle_claim_report]
                    where  convert(date,slm_createddate) = convert(date,getdate()) and (slm_Export_Flag is null or slm_Export_Flag = 0 ) 
                    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                var sqlupdate = @";update [dbo].[kkslm_tr_setle_claim_report]  set Is_Deleted = 1 where slm_ticketID = '" + ticketId + "' and (slm_Export_Flag is not null and slm_Export_Flag = 1 ) ";
                slmdb.ExecuteStoreCommand(sqldel);
                slmdb.ExecuteStoreCommand(sqlupdate);

            }
            catch
            {
                throw;
            }
        }

        public void deletekkslm_tr_setle_claim_cancel_report(string ticketId, string username)
        {
            try
            {
                //remove when found
                var sqldel = @"delete from [dbo].[kkslm_tr_setle_claim_report]
                    where  convert(date,slm_createddate) = convert(date,getdate()) and (slm_Export_Flag is null or slm_Export_Flag = 0 ) 
                    and slm_TicketId  = '" + ticketId + "'";

                var sqlupdate = @";update [dbo].[kkslm_tr_setle_claim_report]  set Is_Deleted = 1 where slm_ticketID = '" + ticketId + "' and (slm_Export_Flag is not null and slm_Export_Flag = 1 ) ";
                slmdb.ExecuteStoreCommand(sqldel);
                slmdb.ExecuteStoreCommand(sqlupdate);

            }
            catch
            {
                throw;
            }
        }

        public void insertkkslm_tr_setle_claim(string ticketId, string username)
        {
            try
            {

                deletekkslm_tr_setle_claim(ticketId, username);
                //

                var sql = @"insert into [kkslm_tr_setle_claim_report]([slm_InsReceiveNo]
                              ,[slm_Ins_Com_Id]      ,[slm_InsReceiveDate]      ,[slm_Contract_Number]
                              ,[slm_Title_Id]      ,[slm_Name]      ,[slm_LastName]
                              ,[slm_CoverageStartDate]      ,[slm_CoverageEndDate]      ,[slm_CarLicenseNo]
                              ,[slm_Owner]      ,[slm_Export_Flag]      ,[slm_Export_Date]
                              ,[slm_CreatedBy]      ,[slm_CreatedDate]      ,[slm_UpdatedBy]      ,[slm_UpdatedDate]
                              ,[slm_TicketId], [is_Deleted])
                              SELECT REN.slm_ReceiveNo  AS slm_InsReceiveNo , REN.slm_InsuranceComId AS slm_Ins_Com_Id,
                             REN.slm_ReceiveDate AS slm_InsReceiveDate, REN.slm_ContractNo AS slm_Contract_Number,
                             LEAD.slm_TitleId AS slm_Title_Id,LEAD.slm_Name, LEAD.slm_LastName,
                             REN.slm_PolicyStartCoverDate AS slm_CoverageStartDate,REN.slm_PolicyEndCoverDate AS slm_CoverageEndDate,
                             ISNULL(REN.slm_LicenseNo, '') + ' ' + ISNULL(prov.slm_ProvinceNameTH, '') AS slm_CarLicenseNo, ST.slm_EmpCode AS slm_Owner,null [slm_Export_Flag],null [slm_Export_Date],
                            '" + username + @"' [slm_CreatedBy],getdate() as [slm_CreatedDate],'" + username + @"' [slm_UpdatedBy],getdate() as [slm_UpdatedDate], REN.slm_TicketId, 0
                            FROM kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON REN.slm_TicketId = LEAD.slm_ticketId
                             LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner
                            LEFT JOIN kkslm_tr_productinfo prod ON prod.slm_TicketId = lead.slm_ticketId
                            LEFT JOIN kkslm_ms_province prov ON prov.slm_ProvinceId = prod.slm_ProvinceRegis
                            WHERE  lead.is_deleted = 0  and  lead.slm_ticketId= '" + ticketId + "'";

                slmdb.ExecuteStoreCommand(sql);

                var id = slmdb.kkslm_tr_setle_claim_report.Where(p => p.slm_TicketId == ticketId && p.is_Deleted == false)
                                .OrderByDescending(p => p.slm_Id).Select(p => p.slm_Id).FirstOrDefault();

                SettleClaimReportId = id;
            }
            catch
            {
                throw;
            }
        }

        public void insertkkslm_tr_setle_claim_cancel(string ticketId, string username)
        {
            try
            {

                // deletekkslm_tr_setle_claim(ticketId, username);
                //

                var sql = @"INSERT INTO [dbo].[kkslm_tr_setle_claim_cancel_report]
                           ([slm_InsReceiveNo]       ,[slm_Ins_Com_Id]      ,[slm_InsReceiveDate]
                           ,[slm_Contract_Number]    ,[slm_Title_Id]        ,[slm_Name]
                           ,[slm_LastName]           ,[slm_CoverageStartDate]      ,[slm_CoverageEndDate]
                           ,[slm_CarLicenseNo]      
		                   ,[slm_Owner]        ,[slm_Export_Flag]		,[slm_Export_Date]        
		                   ,[slm_CreatedBy]        ,[slm_CreatedDate]
                           ,[slm_UpdatedBy]         ,[slm_UpdatedDate])
                SELECT REN.slm_ReceiveNo  AS slm_InsReceiveNo , REN.slm_InsuranceComId AS slm_Ins_Com_Id, REN.slm_ReceiveDate AS slm_InsReceiveDate, 
		                REN.slm_ContractNo AS slm_Contract_Number, LEAD.slm_TitleId AS slm_Title_Id,LEAD.slm_Name, 
		                LEAD.slm_LastName, REN.slm_PolicyStartCoverDate AS slm_CoverageStartDate,REN.slm_PolicyEndCoverDate AS slm_CoverageEndDate,
                        ISNULL(REN.slm_LicenseNo, '') + ' ' + ISNULL(prov.slm_ProvinceNameTH, '') AS slm_CarLicenseNo, 
		                ST.slm_EmpCode AS slm_Owner  ,null [slm_Export_Flag]   ,null [slm_Export_Date],
                    '" + username + @"' [slm_CreatedBy],getdate() as [slm_CreatedDate],
	                '" + username + @"' [slm_UpdatedBy],getdate() as [slm_UpdatedDate]
                            FROM kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON REN.slm_TicketId = LEAD.slm_ticketId
                             LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner
                            LEFT JOIN kkslm_tr_productinfo prod ON prod.slm_TicketId = lead.slm_ticketId
                            LEFT JOIN kkslm_ms_province prov ON prov.slm_ProvinceId = prod.slm_ProvinceRegis
                            WHERE  lead.is_deleted = 0  and  lead.slm_ticketId= '" + ticketId + "'";

                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }
        }

        public void insertkkslm_tr_cancel_policy_report(string ticketId, string username)
        {
            try
            {
                //var sqldel = @"delete from [dbo].[kkslm_tr_cancel_policy_report]
                //    where  convert(date,slm_createddate) = convert(date,getdate()) and slm_Export_Flag is null 
                //    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                //slmdb.ExecuteStoreCommand(sqldel);

                var sql = @"INSERT INTO [dbo].[kkslm_tr_cancel_policy_report]
                           ([slm_Ins_Com_Id]           ,[slm_InsReceiveNo]
                           ,[slm_Contract_Number]           ,[slm_Title_Id]
                           ,[slm_Name]           ,[slm_LastName]
                           ,[slm_CarLicenseNo]           ,[slm_CoverageTypeId]
                           ,[slm_CoverageStartDate]           ,[slm_CoverageEndDate]
                           ,[slm_Owner]           ,[slm_CancelId]
                           ,[slm_Remark]           ,[slm_Export_Flag]
                           ,[slm_Export_Date]           ,[slm_CreatedBy]
                           ,[slm_CreatedDate]           ,[slm_UpdatedBy]           ,[slm_UpdatedDate])

                SELECT REN.slm_InsuranceComId AS slm_Ins_Com_Id, REN.slm_ReceiveNo AS slm_InsReceiveNo, 
                 REN.slm_ContractNo AS slm_Contract_Number, LEAD.slm_TitleId AS slm_Title_Id, LEAD.slm_Name,
                 LEAD.slm_LastName,ISNULL(REN.slm_LicenseNo,'')+' '+ ISNULL(PV.slm_ProvinceNameTH,'')  AS slm_CarLicenseNo, 
                 REN.slm_CoverageTypeId ,
                 REN.slm_PolicyStartCoverDate AS slm_CoverageStartDate, REN.slm_PolicyEndCoverDate AS slm_CoverageEndDate,
                 ST.slm_EmpCode, mc.slm_CancelId as slm_CancelId, REN.slm_RemarkPolicy AS slm_Remark
                ,null [slm_Export_Flag],null [slm_Export_Date]
                ,'" + username + @"' [slm_CreatedBy],getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy], REN.slm_PolicyCancelDate AS [slm_UpdatedDate]
                FROM kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                INNER JOIN kkslm_tr_productinfo PRO ON PRO.slm_TicketId = LEAD.slm_ticketId 
                INNER JOIN kkslm_ms_cancel mc on mc.slm_CancelId = ren.slm_PolicyCancelId
                LEFT JOIN kkslm_ms_province PV ON PV.slm_ProvinceId = PRO.slm_ProvinceRegis
                LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner 
                WHERE lead.is_deleted = 0 and ren.slm_ReceiveNo is not null and lead.slm_ticketId= '" + ticketId + "' ";

                //                var sql = @"INSERT INTO [dbo].[kkslm_tr_cancel_policy_report]
                //                           ([slm_Ins_Com_Id]           ,[slm_InsReceiveNo]
                //                           ,[slm_Contract_Number]           ,[slm_Title_Id]
                //                           ,[slm_Name]           ,[slm_LastName]
                //                           ,[slm_CarLicenseNo]           ,[slm_CoverageTypeId]
                //                           ,[slm_CoverageStartDate]           ,[slm_CoverageEndDate]
                //                           ,[slm_Owner]           ,[slm_CancelId]
                //                           ,[slm_Remark]           ,[slm_Export_Flag]
                //                           ,[slm_Export_Date]           ,[slm_CreatedBy]
                //                           ,[slm_CreatedDate]           ,[slm_UpdatedBy]           ,[slm_UpdatedDate])
                //
                //                SELECT REN.slm_InsuranceComId AS slm_Ins_Com_Id, REN.slm_ReceiveNo AS slm_InsReceiveNo, 
                //                 REN.slm_ContractNo AS slm_Contract_Number, LEAD.slm_TitleId AS slm_Title_Id, LEAD.slm_Name,
                //                 LEAD.slm_LastName, REN.slm_LicenseNo AS slm_CarLicenseNo, REN.slm_CoverageTypeId ,
                //                 REN.slm_PolicyStartCoverDate AS slm_CoverageStartDate, REN.slm_PolicyEndCoverDate AS slm_CoverageEndDate,
                //                 ST.slm_EmpCode, mc.slm_CancelId as slm_CancelId,REN.slm_RemarkPayment AS slm_Remark
                //                ,null [slm_Export_Flag],null [slm_Export_Date]
                //                ,'" + username + @"' [slm_CreatedBy],getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy],getdate() [slm_UpdatedDate]
                //                FROM kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                //                INNER JOIN kkslm_ms_cancel mc on mc.slm_CancelId = ren.slm_PolicyCancelId
                //                 LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner 
                //                WHERE lead.is_deleted = 0 and ren.slm_ReceiveNo is not null and lead.slm_ticketId= '" + ticketId + "'";

                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }
        }

        public void insertkkslm_tr_cancel_act_report(string ticketId, string username)
        {
            try
            {
                //var sqldel = @"delete from [dbo].[kkslm_tr_cancel_act_report]
                //    where  convert(date,slm_createddate) = convert(date,getdate()) and slm_Export_Flag is null 
                //    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                //slmdb.ExecuteStoreCommand(sqldel);

                var sql = @"INSERT INTO [dbo].[kkslm_tr_cancel_act_report]
                               ([slm_CancelDate]           ,[slm_Ins_Com_Id]           ,[slm_Title_Id]           ,[slm_Name]
                               ,[slm_LastName]           ,[slm_Contract_Number]           ,[slm_ActNo]
                               ,[slm_LicenseNo]           ,[slm_ProvinceRegis]           ,[slm_ReceiveDate]
                               ,[slm_ActStartCoverDate]           ,[slm_ActEndCoverDate]           ,[slm_Owner]
                               ,[slm_ReasonCancel]           ,[slm_Remark]           ,[slm_Export_Flag]
                               ,[slm_Export_Date]           ,[slm_CreatedBy]           ,[slm_CreatedDate]
                               ,[slm_UpdatedBy]           ,[slm_UpdatedDate])
                        SELECT getdate() as [slm_CancelDate], /*ren.slm_ReceiveNo as slm_InsReceiveNo,*/ ren.slm_ActComId as slm_Ins_Com_Id, 
                         lead.slm_TitleId as slm_Title_Id, lead.slm_Name, lead.slm_LastName, ren.slm_ContractNo as slm_Contract_Number,REN.slm_ActNo [slm_ActNo],
                         ISNULL(ren.slm_LicenseNo, '') + ' ' + ISNULL(province.slm_ProvinceNameTH, '') as slm_CarLicenseNo, prod.slm_ProvinceRegis [slm_ProvinceRegis],ren.slm_ActSendDate as slm_ReceiveDate,
                         ren.slm_ActStartCoverDate as [slm_ActStartCoverDate], ren.slm_ActEndCoverDate as  [slm_ActEndCoverDate],
                          ST.slm_EmpCode AS slm_Owner,mc.slm_CancelName [slm_ReasonCancel],ren.slm_RemarkAct [slm_Remark],
                        null [slm_Export_Flag],null [slm_Export_Date],
                        '" + username + @"' [slm_CreatedBy],getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy],getdate() [slm_UpdatedDate]
                        FROM kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId
                        INNER JOIN kkslm_ms_cancel mc on mc.slm_CancelId = ren.slm_ActCancelId
                        LEFT JOIN  kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner 
                        LEFT JOIN kkslm_tr_productinfo prod ON prod.slm_TicketId = lead.slm_ticketId
                        LEFT JOIN kkslm_ms_province province ON province.slm_ProvinceId = prod.slm_ProvinceRegis
                        WHERE lead.is_deleted = 0  and lead.slm_ticketId= '" + ticketId + "'";

                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }
        }


        //        public void insertkkslm_tr_problemcomeback_report(string ticketId, string username)
        //        {
        //            try
        //            {
        //                var sqldel = @"delete from [dbo].[kkslm_tr_problemcomeback_report]
        //                    where  convert(date,slm_createddate) = convert(date,getdate()) and slm_Export_Flag is null 
        //                    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

        //                slmdb.ExecuteStoreCommand(sqldel);

        //                var sql = @"INSERT INTO [dbo].[kkslm_tr_problemcomeback_report]
        //                                        ([slm_Ins_Com_Id]           ,[slm_InsType]           ,[slm_Contract_Number]
        //                                        ,[slm_Name]           ,[slm_Owner]           ,[slm_ProblemDetail]
        //                                        ,[slm_CauseDetail]           ,[slm_HeadStaff]           ,[slm_ResponseDetail]
        //                                        ,[slm_PhoneContact]           ,[slm_ResponseDate]           ,[slm_Remark]
        //                                        ,[slm_ProblemDetailId]           ,[slm_ProblemDate]           ,[slm_Export_Flag]
        //                                        ,[slm_Export_Date]           ,[slm_CreatedBy]           ,[slm_CreatedDate]
        //                                        ,[slm_UpdatedBy]           ,[slm_UpdatedDate])
        //
        //                            SELECT REN.slm_InsuranceComId AS slm_Ins_Com_Id, PD.slm_InsType AS slm_InsType,
        //                                REN.slm_ContractNo AS slm_Contract_Number, ISNULL(title.slm_TitleName, '') + lead.slm_Name + ' ' + ISNULL(lead.slm_LastName, '') AS slm_Name, ST.slm_EmpCode AS slm_Owner,
        //                                PD.slm_ProblemDetail AS slm_ProblemDetail, PD.slm_CauseDetail AS slm_CauseDetail,
        //                                HST.slm_StaffNameTH AS slm_HeadStaff, PD.slm_ResponseDetail AS slm_ResponseDetail,
        //                                PD.slm_PhoneContact AS slm_PhoneContact, getdate() AS slm_ResponseDate,
        //                                PD.slm_Remark AS slm_Remark , PD.slm_ProblemDetailId AS slm_ProblemDetailId,
        //                                PD.slm_ProblemDate AS slm_ProblemDate,
        //                                null [slm_Export_Flag],null [slm_Export_Date],'" + username + @"' [slm_CreatedBy],
        //                                getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy],getdate() [slm_UpdatedDate]
        //                            FROM kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId
        //                                --INNER JOIN kkslm_tr_renewinsurance_receipt RR ON RR.slm_ticketId = LEAD.slm_ticketId
        //                                INNER JOIN kkslm_tr_problemdetail PD ON PD.slm_ticketId = LEAD.slm_ticketId
        //                                --INNER JOIN kkslm_tr_renewinsurance_receipt_revision_detail RRD ON RRD.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
        //                                LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner
        //                                LEFT JOIN kkslm_ms_staff HST ON HST.slm_StaffId = ST.slm_HeadStaffId
        //	                            LEFT JOIN kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId
        //                            WHERE lead.is_deleted = 0 and lead.slm_ticketId= '" + ticketId + "'";

        //                slmdb.ExecuteStoreCommand(sql);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }

        //        }

        public void insertkkslm_tr_problemcomeback_report(string ticketId, string problemDetailId, string username)
        {
            try
            {

                var sql = @"INSERT INTO [dbo].[kkslm_tr_problemcomeback_report]
                                        ([slm_Ins_Com_Id]           ,[slm_InsType]           ,[slm_Contract_Number]
                                        ,[slm_Name]           ,[slm_Owner]           ,[slm_ProblemDetail]
                                        ,[slm_CauseDetail]           ,[slm_HeadStaff]           ,[slm_ResponseDetail]
                                        ,[slm_PhoneContact]           ,[slm_ResponseDate]           ,[slm_Remark]
                                        ,[slm_ProblemDetailId]           ,[slm_ProblemDate]           ,[slm_Export_Flag]
                                        ,[slm_Export_Date]           ,[slm_CreatedBy]           ,[slm_CreatedDate]
                                        ,[slm_UpdatedBy]           ,[slm_UpdatedDate])

                            SELECT PD.slm_Ins_Com_Id AS slm_Ins_Com_Id, PD.slm_InsType AS slm_InsType,
                                REN.slm_ContractNo AS slm_Contract_Number, ISNULL(title.slm_TitleName, '') + lead.slm_Name + ' ' + ISNULL(lead.slm_LastName, '') AS slm_Name, ST.slm_EmpCode AS slm_Owner,
                                PD.slm_ProblemDetail AS slm_ProblemDetail, PD.slm_CauseDetail AS slm_CauseDetail,
                                HST.slm_StaffNameTH AS slm_HeadStaff, PD.slm_ResponseDetail AS slm_ResponseDetail,
                                PD.slm_PhoneContact AS slm_PhoneContact, getdate() AS slm_ResponseDate,
                                PD.slm_Remark AS slm_Remark , PD.slm_ProblemDetailId AS slm_ProblemDetailId,
                                PD.slm_ProblemDate AS slm_ProblemDate,
                                null [slm_Export_Flag],null [slm_Export_Date],'" + username + @"' [slm_CreatedBy],
                                getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy],getdate() [slm_UpdatedDate]
                            FROM kkslm_tr_lead LEAD 
                            INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId
                            INNER JOIN kkslm_tr_problemdetail PD ON PD.slm_ticketId = LEAD.slm_ticketId
                            LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner
                            LEFT JOIN kkslm_ms_staff HST ON HST.slm_StaffId = ST.slm_HeadStaffId
                            LEFT JOIN kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId
                            WHERE PD.slm_ProblemDetailId = '" + problemDetailId + "' ";

                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }

        }

        public void insertkkslm_tr_policynew_report(string ticketId, string username)
        {
            try
            {
                string loginEmpCode = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == username).Select(p => p.slm_EmpCode).FirstOrDefault();
                loginEmpCode = loginEmpCode == null ? "" : loginEmpCode;

                var sql = @"INSERT INTO [dbo].[kkslm_tr_policynew_report]
                               ([slm_Ins_Com_Id]           ,[slm_PolicyNo]           ,[slm_BrachCode]
                               ,[slm_Contract_Number]           ,[slm_Title_Id]           ,[slm_Name]
                               ,[slm_LastName]           ,[slm_InsReceiveNo]           ,[slm_ReceiveDate]
                               ,[slm_CarLicenseNo]           ,[slm_BrandCode]           ,[slm_ChassisNo]
                               ,[slm_Owner]           ,[slm_StaffRequest]           ,[slm_CoverageTypeId]
                               ,[slm_RepairTypeId]           ,[slm_PolicyCost]           ,[slm_PolicyGrossPremium]
                               ,[slm_ActStartCoverDate]           ,[slm_ActEndCoverDate]           ,[slm_ReceiverName]
                               ,[slm_IsAddressBranch]           ,[slm_House_No]           ,[slm_Moo]
                               ,[slm_Village]           ,[slm_Building]           ,[slm_Soi]
                               ,[slm_Street]           ,[slm_TambolId]           ,[slm_AmphurId]
                               ,[slm_ProvinceId]           ,[slm_Zipcode]           ,[slm_Remark]
                               ,[slm_PhoneContact]           ,[slm_Export_Flag]           ,[slm_Export_Date]
                               ,[slm_CreatedBy]           ,[slm_CreatedDate]           ,[slm_UpdatedBy]
                               ,[slm_UpdatedDate])
                        SELECT top 1 REN.slm_InsuranceComId AS slm_Ins_Com_Id,REN.slm_PolicyNo , LEAD.slm_Owner_Branch ,
                         REN.slm_ContractNo AS slm_Contract_Number, lead.slm_TitleId  as slm_Title_Id, lead.slm_Name,
                         LEAD.slm_LastName, REN.slm_ReceiveNo AS slm_InsReceiveNo ,REN.slm_ReceiveDate ,
                         ISNULL(REN.slm_LicenseNo, '') + ' ' + ISNULL(proregis.slm_ProvinceNameTH, '') AS slm_CarLicenseNo, REN.slm_RedbookBrandCode AS slm_BrandCode,
                         REN.slm_ChassisNo , ST.slm_EmpCode AS slm_Owner, '" + loginEmpCode + @"' AS slm_StaffRequest,
                         REN.slm_CoverageTypeId ,REN.slm_RepairTypeId ,RENC.slm_FT AS slm_PolicyCost, 
                         REN.slm_PolicyGrossPremiumTotal AS slm_PolicyGrossPremium, REN.slm_ActStartCoverDate,
                         REN.slm_ActEndCoverDate AS slm_ActEndCoverDate, REN.slm_Receiver AS slm_ReceiverName,
                         CASE WHEN REN.slm_SendDocFlag = '3' THEN '1' ELSE '2' END slm_IsAddressBranch,
                         RENA.slm_House_No, RENA.slm_Moo, RENA.slm_Village, RENA.slm_BuildingName, RENA.slm_Soi,
                         RENA.slm_Street, RENA.slm_Tambon AS slm_TambolId, RENA.slm_Amphur AS slm_AmphurId , 
                         RENA.slm_Province AS slm_ProvinceId,RENA.slm_PostalCode AS slm_Zipcode, 
                         CASE WHEN REN.slm_SendDocFlag = '3' THEN 'รับเอกสารสาขา '+ BH.slm_BranchName
                          ELSE 'ที่อยู่ตามระบบ' END slm_Remark,LEAD.slm_TelNo_1 AS slm_PhoneContact,
                         null [slm_Export_Flag],null [slm_Export_Date],'" + username + @"' [slm_CreatedBy],
                         getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy],getdate() [slm_UpdatedDate]
                        FROM kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId
                         LEFT JOIN kkslm_tr_renewinsurance_compare RENC ON RENC.slm_RenewInsureId = REN.slm_RenewInsureId AND RENC.slm_Selected = 1 
                         LEFT JOIN kkslm_tr_renewinsurance_address RENA ON RENA.slm_RenewInsureId = REN.slm_RenewInsureId AND slm_AddressType = 'D'
                         LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner
                         LEFT JOIN kkslm_ms_staff RQ ON RQ.slm_UserName = lead.slm_UpdatedBy 
                         LEFT JOIN kkslm_ms_branch BH ON BH.slm_BranchCode = REN.slm_SendDocBrandCode
                         LEFT JOIN kkslm_tr_productinfo prod ON prod.slm_TicketId = LEAD.slm_TicketId
                         LEFT JOIN kkslm_ms_province proregis ON proregis.slm_ProvinceId = prod.slm_ProvinceRegis
                        WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status NOT IN ('08','09') and REN.slm_ReceiveDate is not null and lead.slm_ticketId= '" + ticketId + "'";

                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }
        }

        public void deletekkslm_tr_policynew_report(string ticketId, string username)
        {
            try
            {
                //remove when found
                var sql = @"delete from [dbo].[kkslm_tr_policynew_report]
                    where  convert(date,slm_createddate) = convert(date,getdate()) and (slm_Export_Flag is null or slm_Export_Flag = 0)  
                    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                var sqlupdate = @"update  [dbo].[kkslm_tr_policynew_report]
                    set Is_Deleted = 1  
                    where  (slm_Export_Flag is not null and slm_Export_Flag = 0)  
                    and slm_TicketId  = '" + ticketId + "'";

                slmdb.ExecuteStoreCommand(sql);
                slmdb.ExecuteStoreCommand(sqlupdate);
                //insertkkslm_tr_policynew_report(ticketId, username);
            }
            catch
            {
                throw;
            }
        }

        public void insertkkslm_tr_actnew_report(string ticketId, string username)
        {
            try
            {
                var sql = @"INSERT INTO [dbo].[kkslm_tr_actnew_report]
                           ([slm_Ins_Com_Id]           ,[slm_ActNo]           ,[slm_BrachCode]
                           ,[slm_Contract_Number]           ,[slm_Title_Id]           ,[slm_Name]
                           ,[slm_LastName]           ,[slm_InsReceiveNo]           ,[slm_CarLicenseNo]
                           ,[slm_BrandCode]           ,[slm_ChassisNo]           ,[slm_Owner]
                           ,[slm_StaffRequest]           ,[slm_ActGrossPremium]           ,[slm_PolicyGrossPremium]
                           ,[slm_ActStartCoverDate]           ,[slm_ActEndCoverDate]           ,[slm_ReceiveDate]
                           ,[slm_ReceiverName]           ,[slm_House_No]           ,[slm_Moo]
                           ,[slm_Village]           ,[slm_Building]           ,[slm_Soi]
                           ,[slm_Street]           ,[slm_TambolId]           ,[slm_AmphurId]
                           ,[slm_ProvinceId]           ,[slm_Zipcode]           ,[slm_PhoneContact]
                           ,[slm_Remark]           ,[slm_IsAddressBranch]           ,[slm_Export_Flag]
                           ,[slm_Export_Date]           ,[slm_CreatedBy]           ,[slm_CreatedDate]
                           ,[slm_UpdatedBy]           ,[slm_UpdatedDate], [slm_ticketId], [is_Deleted])
                        SELECT top 1 REN.slm_ActComId AS slm_Ins_Com_Id,REN.slm_ActNo , LEAD.slm_Owner_Branch ,
                            REN.slm_ContractNo AS slm_Contract_Number, lead.slm_TitleId  as slm_Title_Id, lead.slm_Name,
                            LEAD.slm_LastName, REN.slm_ReceiveNo AS slm_InsReceiveNo ,
                            ISNULL(REN.slm_LicenseNo, '') + ' ' + ISNULL(proregis.slm_ProvinceNameTH, '') AS slm_CarLicenseNo, REN.slm_RedbookBrandCode AS slm_BrandCode,
                            REN.slm_ChassisNo , ST.slm_EmpCode AS slm_Owner, RQ.slm_EmpCode AS slm_StaffRequest,
                            --REN.slm_CoverageTypeId ,REN.slm_RepairTypeId,
                            REN.slm_ActNetPremium AS slm_ActGrossPremium, ISNULL(REN.slm_ActNetPremium, 0) + ISNULL(REN.slm_ActVat, 0) + ISNULL(REN.slm_ActStamp, 0) AS slm_PolicyGrossPremium, 
                            REN.slm_ActStartCoverDate,REN.slm_ActEndCoverDate AS slm_ActEndCoverDate,REN.slm_ReceiveDate AS slm_ReceiveDate,
                            REN.slm_Receiver AS slm_ReceiverName,
                            RENA.slm_House_No, RENA.slm_Moo, RENA.slm_Village, RENA.slm_BuildingName, RENA.slm_Soi,
                            RENA.slm_Street, RENA.slm_Tambon AS slm_TambolId, RENA.slm_Amphur AS slm_AmphurId , 
                            RENA.slm_Province AS slm_ProvinceId,RENA.slm_PostalCode AS slm_Zipcode, LEAD.slm_TelNo_1 AS slm_PhoneContact,
                            CASE WHEN REN.slm_SendDocFlag = '3' THEN 'รับเอกสารสาขา '+ BH.slm_BranchName
                            ELSE 'ที่อยู่ตามระบบ' END slm_Remark,CASE WHEN REN.slm_SendDocFlag = '3' THEN '1' ELSE '2' END slm_IsAddressBranch, 
                            
                            null [slm_Export_Flag],null [slm_Export_Date],'" + username + @"' [slm_CreatedBy],
                            getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy],getdate() [slm_UpdatedDate],
                            lead.slm_ticketId, 0 
                        FROM kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId
                            LEFT JOIN kkslm_tr_renewinsurance_address RENA ON RENA.slm_RenewInsureId = REN.slm_RenewInsureId AND slm_AddressType = 'D'
                            LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner
                            LEFT JOIN kkslm_ms_staff RQ ON RQ.slm_UserName = lead.slm_UpdatedBy 
                            LEFT JOIN kkslm_ms_branch BH ON BH.slm_BranchCode = REN.slm_SendDocBrandCode
                            LEFT JOIN kkslm_tr_productinfo prod ON prod.slm_TicketId = LEAD.slm_TicketId
                            LEFT JOIN kkslm_ms_province proregis ON proregis.slm_ProvinceId = prod.slm_ProvinceRegis
                        WHERE LEAD.is_Deleted = 0 AND REN.slm_ActComId is not null AND LEAD.slm_Status NOT IN ('08','09') 
                        and lead.slm_ticketId= '" + ticketId + "'";
                //AND REN.slm_ReceiveDate is not null
                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }

        }

        public void deletekkslm_tr_actnew_report(string ticketId, string username)
        {
            try
            {
                //remove when found

                var sql = @"delete from [dbo].[kkslm_tr_actnew_report]
                    where  convert(date,slm_createddate) = convert(date,getdate()) and slm_Export_Flag is null 
                    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                slmdb.ExecuteStoreCommand(sql);

                //insertkkslm_tr_actnew_report(ticketId, username);
            }
            catch
            {
                throw;
            }
        }

        public void insertkkslm_tr_amendmentbill_report(string ticketId, string username)
        {
            try
            {
                var sqldel = @"delete from [dbo].[kkslm_tr_amendmentbill_report]
                    where  convert(date,slm_createddate) = convert(date,getdate()) and slm_Export_Flag is null 
                    and slm_Contract_Number  in (select slm_ContractNo from kkslm_tr_renewinsurance where slm_TicketId  = '" + ticketId + "')";

                slmdb.ExecuteStoreCommand(sqldel);

                var sql = @"INSERT INTO [dbo].[kkslm_tr_amendmentbill_report]
                                ([slm_Title_Id]           ,[slm_Name]           ,[slm_LastName]
                                ,[slm_Contract_Number]           ,[slm_CarLicenseNo]           ,[slm_ReceiptNo]
                                ,[slm_ReceiptDate]           ,[slm_PaymentCodeOld1]           ,[slm_PaymentAmtOld1]
                                ,[slm_PaymentCodeNew1]           ,[slm_PaymentAmtNew1]           ,[slm_PaymentCodeNew2]
                                ,[slm_PaymentAmtNew2]           ,[slm_PaymentCodeNew3]           ,[slm_PaymentAmtNew3]
                                ,[slm_PaymentCodeNew4]          ,[slm_PaymentAmtNew4]           ,[slm_PaymentCodeNew5]
                                ,[slm_PaymentAmtNew5]           ,[slm_PaymentCodeNew6]           ,[slm_PaymentAmtNew6]    
                                ,[slm_PaymentAmtTotal]          ,[slm_Owner]
                                ,[slm_Export_Flag]              ,[slm_Export_Date]           ,[slm_CreatedBy]
                                ,[slm_CreatedDate]              ,[slm_UpdatedBy]           ,[slm_UpdatedDate]
                                ,[slm_RenewInsureId])
                            SELECT DISTINCT LEAD.slm_TitleId AS slm_Title_Id, LEAD.slm_Name , LEAD.slm_LastName , REN.slm_ContractNo AS slm_Contract_Number,
                                    ISNULL(REN.slm_LicenseNo, '') + ' ' + ISNULL(province.slm_ProvinceNameTH, '') AS slm_CarLicenseNo, RRD.slm_RecNo [slm_ReceiptNo], RRD.slm_TransDate AS  slm_ReceiptDate,
                                    P2.PAYMENT_DESC AS slm_PaymentCodeOld1,P2.REC_AMOUNT,
                                    R1.P1 AS slm_PaymentCodeNew1, R1.RM1 AS slm_PaymentAmtNew1,
                                    R2.P2 AS slm_PaymentCodeNew2, R2.RM2 AS slm_PaymentAmtNew2,
                                    R3.P3 AS slm_PaymentCodeNew3, R3.RM3 AS slm_PaymentAmtNew3,
                                    R4.P4 AS slm_PaymentCodeNew4, R4.RM4 AS slm_PaymentAmtNew4,
                                    R5.P5 AS slm_PaymentCodeNew5, R5.RM5 AS slm_PaymentAmtNew5,
                                 R6.P6 + 
                                 CASE WHEN R6.PaymentOtherDesc IS NULL OR R6.PaymentOtherDesc = '' THEN ''
                                 ELSE ' - ' + R6.PaymentOtherDesc END AS slm_PaymentCodeNew6, R6.RM6 AS slm_PaymentAmtNew6,
                                    ISNULL(R1.RM1, 0) + ISNULL(R2.RM2, 0) + ISNULL(R3.RM3, 0) + ISNULL(R4.RM4, 0) + ISNULL(R5.RM5, 0) + ISNULL(R6.RM6, 0) AS [slm_PaymentAmtTotal], ST.slm_EmpCode AS slm_Owner,
                                    null [slm_Export_Flag],null [slm_Export_Date],'" + username + @"' [slm_CreatedBy],
                                    getdate() [slm_CreatedDate] ,'" + username + @"' [slm_UpdatedBy],getdate() [slm_UpdatedDate],
                                    REN.slm_RenewInsureId 
                                FROM kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                                    LEFT JOIN kkslm_tr_productinfo prod ON lead.slm_ticketId = prod.slm_TicketId
                                    LEFT JOIN kkslm_ms_province province ON prod.slm_ProvinceRegis = province.slm_ProvinceId
                                    INNER JOIN kkslm_tr_renewinsurance_receipt RR ON RR.slm_ticketId = LEAD.slm_ticketId AND (RR.slm_Status IS NOT NULL OR RR.slm_Status <> '')
                                    INNER JOIN kkslm_tr_renewinsurance_receipt_detail RRD ON RRD.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId 
                                    LEFT JOIN 
                                    (
	                                SELECT CASE WHEN A.slm_PaymentDesc IS NULL OR A.slm_PaymentDesc = '' THEN '' ELSE A.slm_PaymentDesc END +
		                                 ISNULL(CASE WHEN A.slm_PaymentDesc IS NULL OR A.slm_PaymentDesc = '' THEN '' ELSE ' / ' END + Z.slm_PaymentDesc,'')  PAYMENT_DESC,
		                                ISNULL(A.slm_RecAmount,0) + ISNULL(Z.slm_RecAmount,0)  AS REC_AMOUNT ,
                                         CASE WHEN A.SLM_TICKETID IS NOT NULL THEN A.SLM_TICKETID
											WHEN z.SLM_TICKETID IS NOT NULL THEN z.SLM_TICKETID END SLM_TICKETID,
		                               CASE WHEN A.slm_RenewInsuranceReceiptId IS NOT NULL THEN A.slm_RenewInsuranceReceiptId
											WHEN z.slm_RenewInsuranceReceiptId IS NOT NULL THEN z.slm_RenewInsuranceReceiptId END slm_RenewInsuranceReceiptId  
		                                FROM (
                                         SELECT RR.slm_ticketId, P.slm_PaymentDesc , RRD.slm_RecAmount,RR.slm_RenewInsuranceReceiptId    
                                         FROM kkslm_tr_renewinsurance_receipt RR INNER JOIN kkslm_tr_renewinsurance_receipt_detail RRD 
                                                ON RR.slm_RenewInsuranceReceiptId = RRD.slm_RenewInsuranceReceiptId 
                                                INNER JOIN kkslm_ms_payment P ON P.slm_PaymentCode = RRD.slm_PaymentCode 
                                         WHERE P.slm_PaymentCode = '204' AND slm_ticketId = '" + ticketId + @"') AS A FULL JOIN
		                                (
                                         SELECT RR.slm_ticketId, P.slm_PaymentDesc , RRD.slm_RecAmount ,RR.slm_RenewInsuranceReceiptId   
                                         FROM kkslm_tr_renewinsurance_receipt RR INNER JOIN kkslm_tr_renewinsurance_receipt_detail RRD 
                                                ON RR.slm_RenewInsuranceReceiptId = RRD.slm_RenewInsuranceReceiptId 
                                                INNER JOIN kkslm_ms_payment P ON P.slm_PaymentCode = RRD.slm_PaymentCode 
                                         WHERE P.slm_PaymentCode = '205' AND slm_ticketId = '" + ticketId + @"') AS Z ON A.slm_ticketId = Z.slm_ticketId 
                                                and  a.slm_RenewInsuranceReceiptId = z.slm_RenewInsuranceReceiptId
                                     ) AS P2 ON P2.slm_ticketId = LEAD.slm_ticketId and P2.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
                                    INNER JOIN kkslm_tr_renewinsurance_receipt_revision_detail RRRD 
                                    ON RRRD.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId 
                                    LEFT JOIN ( SELECT P.slm_PaymentDesc AS P1, RRRD.slm_RecAmount AS RM1, RRRD.slm_RenewInsuranceReceiptId
                                    FROM kkslm_tr_renewinsurance_receipt_revision_detail RRRD INNER JOIN kkslm_ms_payment P ON P.slm_PaymentCode = RRRD.slm_PaymentCode 
                                    WHERE slm_Seq = 1) R1 
                                    ON R1.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
                                    LEFT JOIN ( SELECT P.slm_PaymentDesc AS P2, RRRD.slm_RecAmount AS RM2, RRRD.slm_RenewInsuranceReceiptId
                                    FROM kkslm_tr_renewinsurance_receipt_revision_detail RRRD INNER JOIN kkslm_ms_payment P ON P.slm_PaymentCode = RRRD.slm_PaymentCode 
                                    WHERE slm_Seq = 2) R2 
                                    ON R2.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
                                    LEFT JOIN ( SELECT P.slm_PaymentDesc AS P3, RRRD.slm_RecAmount AS RM3, RRRD.slm_RenewInsuranceReceiptId
                                    FROM kkslm_tr_renewinsurance_receipt_revision_detail RRRD INNER JOIN kkslm_ms_payment P ON P.slm_PaymentCode = RRRD.slm_PaymentCode 
                                    WHERE slm_Seq = 3) R3 
                                    ON R3.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
                                    LEFT JOIN ( SELECT P.slm_PaymentDesc AS P4, RRRD.slm_RecAmount AS RM4, RRRD.slm_RenewInsuranceReceiptId
                                    FROM kkslm_tr_renewinsurance_receipt_revision_detail RRRD INNER JOIN kkslm_ms_payment P ON P.slm_PaymentCode = RRRD.slm_PaymentCode 
                                    WHERE slm_Seq = 4) R4 
                                    ON R4.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
                                    LEFT JOIN ( SELECT P.slm_PaymentDesc AS P5, RRRD.slm_RecAmount AS RM5, RRRD.slm_RenewInsuranceReceiptId
                                    FROM kkslm_tr_renewinsurance_receipt_revision_detail RRRD INNER JOIN kkslm_ms_payment P ON P.slm_PaymentCode = RRRD.slm_PaymentCode 
                                    WHERE slm_Seq = 5) R5 
                                    ON R5.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
                                 LEFT JOIN ( SELECT P.slm_PaymentDesc AS P6, RRRD.slm_RecAmount AS RM6, RRRD.slm_RenewInsuranceReceiptId, RRRD.slm_PaymentOtherDesc AS PaymentOtherDesc
                                    FROM kkslm_tr_renewinsurance_receipt_revision_detail RRRD INNER JOIN kkslm_ms_payment P ON P.slm_PaymentCode = RRRD.slm_PaymentCode 
                                    WHERE slm_Seq = 6) R6 
                                    ON R6.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
                                    LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner
                                WHERE LEAD.is_Deleted = 0 AND LEAD.slm_Status NOT IN ('08','09','10') and lead.slm_ticketId =  '" + ticketId + @"'";

                slmdb.ExecuteStoreCommand(sql);
            }
            catch
            {
                throw;
            }
        }

        public string GetNotifyPremiumRemark(string chassisNo)
        {
            try
            {
                var remark = slmdb.kkslm_tr_notify_premium.Where(p => p.slm_VIN == chassisNo).OrderByDescending(p => p.slm_CreatedDate).Select(p => p.slm_Remark).FirstOrDefault();
                return remark != null ? remark : "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public kkslm_tr_prelead_prepare_sms InsertPrepareSMS(string smsTemplate, PreleadCompareDataCollection data)
        {
            try
            {
                // sms format
                // เริ่มคุ้มครองกรมธรรม์รถยนต์ %CarLicenseNo% %InsuranceCompany% เลขที่รับแจ้ง %ReceiveNo% แจ้งเหตุ %InsuranceTelNo%

                var insCom = operdb.kkslm_ms_ins_com.Where(i => i.is_Deleted == false && i.slm_Ins_Com_Id == data.RenewIns.slm_InsuranceComId).FirstOrDefault();

                smsTemplate = smsTemplate.Replace("%CarLicenseNo%", data.RenewIns.slm_LicenseNo)
                                    .Replace("%InsuranceCompany%", insCom == null ? "" : insCom.slm_InsNameTh)
                                    .Replace("%ReceiveNo%", data.RenewIns.slm_ReceiveNo)
                                    .Replace("%InsuranceTelNo%", insCom == null ? insCom.slm_TelContact : "");

                DateTime createdDate = DateTime.Now;

                kkslm_tr_prelead_prepare_sms sms = new kkslm_tr_prelead_prepare_sms()
                {
                    slm_ticketId = data.RenewIns.slm_TicketId,
                    slm_Message = smsTemplate,
                    slm_Message_CreatedBy = "SYSTEM",
                    slm_MessageStatusId = "1",
                    slm_PhoneNumber = data != null ? data.lead.slm_TelNo_1 : null,
                    slm_QueueId = "6",
                    slm_RequestDate = createdDate,
                    slm_RuleActivityId = 0,
                    slm_ExportStatus = "0",
                    slm_RefId = null,
                    slm_SendingStatusCode = null,
                    slm_SendingStatus = null,
                    slm_ErrorCode = null,
                    slm_ErrorReason = null,
                    slm_CAS_Flag = null,
                    slm_CAS_Date = null,
                    slm_FlagType = "2",
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedDate = createdDate,
                    slm_UpdatedBy = "SYSTEM",
                    slm_UpdatedDate = createdDate,
                    is_Deleted = 0
                };
                slmdb.kkslm_tr_prelead_prepare_sms.AddObject(sms);
                return sms;
            }
            catch
            {
                throw;
            }
        }

        public void InsertSettleClaimActivityLog(PreleadCompareDataCollection data, string createBy)
        {
            if (data.RenewIns.slm_ClaimFlag == "1") // ระงับเคลม
            {
                var claimReport = slmdb.kkslm_tr_setle_claim_report.Any(r => r.slm_TicketId == data.RenewIns.slm_TicketId && r.slm_Export_Flag == true);
                if (claimReport)
                {
                    kkslm_tr_activity act = new kkslm_tr_activity()
                    {
                        slm_TicketId = data.RenewIns.slm_TicketId,
                        slm_CreatedBy = createBy,
                        slm_CreatedBy_Position = GetPositionId(createBy),
                        slm_CreatedDate = DateTime.Now,
                        slm_Type = SLMConstant.ActionType.SettleClaim, // "19",                    //Settle Claim = 19
                        slm_SystemAction = "SLM",           //System ที่เข้ามาทำ action
                        slm_SystemActionBy = "SLM",         //action เกิดขึ้นที่ระบบอะไร
                        slm_UpdatedBy = createBy,
                        slm_UpdatedDate = DateTime.Now
                    };
                    slmdb.kkslm_tr_activity.AddObject(act);

                    slmdb.SaveChanges();
                }
            }
        }

        public bool IsTeamBoss(string staffId)
        {
            try
            {
                var count = slmdb.kkslm_ms_teamtelesales.Where(p => p.slm_HeadStaff == staffId && p.is_Deleted == false).Count();
                return count > 0 ? true : false;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckPolicyPurchased(string ticketId)
        {
            try
            {
                var count = (from a in slmdb.kkslm_tr_renewinsurance
                             join b in slmdb.kkslm_tr_renewinsurance_compare on a.slm_RenewInsureId equals b.slm_RenewInsureId
                             where a.slm_TicketId == ticketId && b.slm_Selected == true
                             select a).Count();

                return count > 0 ? true : false;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckActPurchaseDateTimeWithActIssueReport(string ticketId, DateTime startCoverDate, out string errorMessage)
        {
            try
            {
                errorMessage = "";
                bool hasReport = slmdb.kkslm_tr_notify_act_report.Any(p => p.slm_TicketId == ticketId && p.is_Deleted == false && (p.slm_Export_Flag == null || p.slm_Export_Flag == false));
                if (!hasReport)
                {
                    return true;
                }

                var compare = (from renew in slmdb.kkslm_tr_renewinsurance
                               join com_act in slmdb.kkslm_tr_renewinsurance_compare_act on renew.slm_RenewInsureId equals com_act.slm_RenewInsureId
                               where renew.slm_TicketId == ticketId && com_act.slm_ActPurchaseFlag == true
                               select com_act).FirstOrDefault();

                if (compare == null)
                {
                    return true;
                }
                if (compare.slm_ActStartCoverDate == null)
                {
                    return true;
                }

                if (startCoverDate.Date != compare.slm_ActStartCoverDate.Value.Date)
                {
                    string temp = SLMConstant.ActPurchaseTime.Replace(":", "");
                    int currentTime = int.Parse(DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00"));
                    int checkTime = int.Parse(temp);
                    DateTime currentDate = DateTime.Now;

                    if (currentTime < checkTime)    //ก่อน 16:30
                    {
                        if (startCoverDate.Date < currentDate.Date)
                        {
                            errorMessage = "วันเริ่มต้น พรบ.ต้องมากกว่าหรือเท่ากับวันปัจจุบัน";
                            return false;
                        }
                    }
                    else
                    {
                        if (startCoverDate.Date <= currentDate.Date)
                        {
                            errorMessage = "วันเริ่มต้น พรบ.ต้องมากกว่าวันปัจจุบัน";
                            return false;
                        }
                    }
                }

                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
