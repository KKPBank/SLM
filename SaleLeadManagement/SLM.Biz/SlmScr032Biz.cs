using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using System.Transactions;
using SLM.Dal;
using SLM.Resource.Data;
using System.Data.SqlClient;
using SLM.Resource;

namespace SLM.Biz
{
    public class SlmScr032Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        public static List<ControlListData> GetTeamSalesList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_teamtelesales
                    .Where(t => t.is_Deleted == false)
                    .ToList()
                    .Select(t => new ControlListData() { TextField = t.slm_TeamTelesales_Code, ValueField = t.slm_TeamTelesales_Id.ToString() })
                    .OrderBy(t => t.TextField)
                    .ToList();
            }
        }

        public static List<ControlListData> GetSearchStaffList(int telesaleTeamId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_staff
                    .Where(s => s.slm_TeamTelesales_Id == telesaleTeamId)
                    .Select(s => new ControlListData() { TextField = s.slm_StaffNameTH, ValueField = s.slm_EmpCode })
                    .OrderBy(s => s.TextField)
                    .ToList();
            }
        }

        private static string BRN001_08_select()
        {
            return ", pl.slm_Chassis_No, pl.slm_Engine_No, pl.slm_Car_License_No, pl.slm_ProvinceRegis_Org "
                   + ", pl.slm_Voluntary_Policy_Number, pl.slm_Voluntary_Policy_Exp_Date "
                   + ", slm_FlagNotifyPremium = isnull(NP.slm_FlagNotifyPremium, 'N')"
                   + ", slm_PolicyGrossPremium = (case when NP.slm_FlagNotifyPremium is null then null else NP.slm_NetGrossPremium end) "
                   + ", slm_LatestInsExpireDate = (case when NP.slm_FlagNotifyPremium is null then null else NP.slm_LatestInsExpireDate end) ";
        }

        private static string BRN001_08_selectLite()
        {
            return ", slm_FlagNotifyPremium = isnull(NP.slm_FlagNotifyPremium, 'N')";
        }

        private static string BRN001_08_fromJoin()
        {
            return @" left outer hash join (SELECT row_number() OVER ( PARTITION BY REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') ORDER BY slm_Id DESC ) rn, slm_VIN, slm_InsExpireDate AS slm_LatestInsExpireDate, slm_CreatedDate AS slm_LatestCreatedDate, slm_NetPremium AS slm_PolicyGrossPremium, slm_GrossPremium AS slm_NetGrossPremium, 'Y' AS slm_FlagNotifyPremium, slm_PeriodMonth, slm_PeriodYear 
                                            FROM dbo.kkslm_tr_notify_premium with (nolock) ) NP on REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(NP.slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(pl.slm_Chassis_No,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') and rn = 1 
                                                        AND pl.slm_PeriodMonth = NP.slm_PeriodMonth AND pl.slm_PeriodYear = NP.slm_PeriodYear ";
        }

        public List<SlmScr032SearchResult> GetSuccessList(int lot, string name, string lastname, string cardType, string citizenId, string campaignId, DateTime createDate, string ownerTeam, string owner, out DataSet resultDs)
        {
            resultDs = new DataSet();
            StringBuilder command = new StringBuilder("select Row_Number() OVER(order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc) as No"
                + ", pl.slm_Prelead_Id, team.slm_TeamTelesales_Code, staff.slm_StaffNameTH, staff.slm_EmpCode"
                + ", pl.slm_Contract_Number, slm_Name = isnull(pl.slm_Name, ''), slm_LastName = isnull(pl.slm_LastName, ''), slm_InsNameTh = isnull(com.slm_InsNameTh, ''), cover.slm_ConverageTypeName"
                + ", pl.slm_Voluntary_Gross_Premium, pl.slm_CreatedDate, pl.slm_Grade, typ.slm_Assign_Type_Name"
                + ", pl.slm_AssignDescription, pl.slm_Cmt_Product_Id, pl.slm_CmtLot, mon.slm_MonthNameTh"
                //+ ", CustomerName = isnull(pl.slm_Name, '') + ' ' + isnull(pl.slm_LastName, ''), lg.slm_NewAssignType"
                + ", CustomerName = isnull(pl.slm_Name, '') + ' ' + isnull(pl.slm_LastName, '') "
                + ", pl.SLM_CAR_BY_GOV_NAME_ORG, pl.SLM_MODEL_NAME_ORG, pl.SLM_BRAND_NAME_ORG, pl.SLM_VOLUNTARY_MKT_ID "
                + ", pl.SLM_VOLUNTARY_MKT_FIRST_NAME, pl.SLM_VOLUNTARY_MKT_LAST_NAME, pl.SLM_BRANCHCODE, pl.slm_CitizenId "
                + BRN001_08_select()
                + " from ( "
                        + @" select prelead.slm_Grade, prelead.slm_Voluntary_Gross_Premium, prelead.slm_Prelead_Id, prelead.slm_Contract_Number
                            , prelead.slm_Name, prelead.slm_LastName, prelead.slm_CreatedDate
                            , prelead.slm_AssignDescription, prelead.slm_Cmt_Product_Id, prelead.slm_CmtLot, prelead.SLM_CAR_BY_GOV_NAME_ORG
                            , prelead.SLM_MODEL_NAME_ORG, prelead.SLM_BRAND_NAME_ORG, prelead.SLM_VOLUNTARY_MKT_ID, prelead.SLM_VOLUNTARY_MKT_FIRST_NAME
                            , prelead.SLM_VOLUNTARY_MKT_LAST_NAME, prelead.SLM_BRANCHCODE, prelead.slm_CitizenId, prelead.slm_Chassis_No, prelead.slm_Engine_No
                            , prelead.slm_Car_License_No, prelead.slm_ProvinceRegis_Org, prelead.slm_Voluntary_Policy_Number, prelead.slm_Voluntary_Policy_Exp_Date
                            , prelead.slm_Owner, prelead.slm_Voluntary_Company_Code, prelead.slm_Voluntary_Type_Key, prelead.slm_AssignType, prelead.slm_AssignFlag
                            , pt.slm_PeriodMonth, pt.slm_PeriodYear, prelead.slm_Owner_Team, prelead.slm_CardTypeId, prelead.slm_CampaignId
                            from dbo.kkslm_tr_prelead prelead with (nolock) 
                            left join dbo.kkslm_tr_preleadportmonitor pt on prelead.slm_CmtLot = pt.slm_CMTLot and pt.is_Deleted = 0
                            where pt.slm_CMTLot = '" + lot.ToString() + "' "
                + "      ) pl"
                + " left join dbo.kkslm_ms_staff staff with (nolock) on staff.slm_EmpCode = pl.slm_Owner and staff.is_Deleted = 0 "
                + " left join dbo.kkslm_ms_teamtelesales team on team.slm_TeamTelesales_Id = staff.slm_TeamTelesales_Id"
                + " left join " + SLMConstant.OPERDBName + ".dbo.kkslm_ms_ins_com com on com.slm_InsCode = pl.slm_Voluntary_Company_Code and com.is_Deleted = 0 "
                + " left join dbo.kkslm_ms_coveragetype cover on cover.slm_CoverageTypeId = pl.slm_Voluntary_Type_Key"
                + " left join dbo.kkslm_ms_assign_type typ on typ.slm_Assign_Type_AbbName = pl.slm_AssignType"
                //+ " left join slmdb.dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Eff_Date)"
                + " left join dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Exp_Date)"
                //+ " left join (select slm_cmtlot, slm_cmt_product_id, slm_NewAssignType, row_number() over (partition by slm_cmtlot, slm_cmt_product_id order by slm_createddate desc) as cnt from dbo.kkslm_tr_prelead_assign_log) lg on lg.slm_CmtLot = pl.slm_CmtLot and lg.slm_Cmt_Product_Id = pl.slm_Cmt_Product_Id and cnt = 1 "
                + BRN001_08_fromJoin()
                + " where pl.slm_CmtLot = {0} and pl.slm_AssignFlag = '0'");

            if (name != string.Empty)
            {
                command.AppendFormat(" and pl.slm_Name like '%{0}%'", name);
            }
            if (lastname != string.Empty)
            {
                command.AppendFormat(" and pl.slm_LastName like '%{0}%'", lastname);
            }
            if (cardType != "-1")
            {
                command.AppendFormat(" and pl.slm_CardTypeId = '{0}'", cardType);
            }
            if (citizenId != string.Empty)
            {
                command.AppendFormat(" and pl.slm_CitizenId = '{0}'", citizenId);
            }
            if (campaignId != string.Empty && campaignId != "-1")
            {
                command.AppendFormat(" and pl.slm_CampaignId = '{0}'", campaignId);
            }
            if (createDate.Date != (new DateTime()).Date)
            {
                string date = createDate.Date.Year.ToString() + createDate.Date.ToString("-MM-dd");
                command.AppendFormat(" and CONVERT(DATE, pl.slm_CreatedDate) = '{0}'", date);
            }
            if (ownerTeam != "-1")
            {
                command.AppendFormat(" and pl.slm_Owner_Team = '{0}'", ownerTeam);
            }
            if (owner != "-1" && owner != string.Empty)
            {
                command.AppendFormat(" and pl.slm_Owner = '{0}'", owner);
            }

            command.Append(" order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc ");

            string commandText = String.Format(command.ToString(), lot);

            using (var slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SlmScr032SearchResult>(commandText).ToList();
            }

            //return getSelectResult(commandText, out resultDs);
        }

        public List<SlmScr032SearchResult> GetDedubList(int lot, string name, string lastname, string cardType, string citizenId, string campaignId, DateTime createDate, out DataSet resultDs)
        {
            resultDs = new DataSet();
            StringBuilder command = new StringBuilder("select Row_Number() OVER(order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc) as No"
                + ", pl.slm_TempId, pl.slm_Contract_Number, slm_Name = isnull(pl.slm_Name, ''), slm_LastName = isnull(pl.slm_LastName, ''), slm_InsNameTh = isnull(com.slm_InsNameTh, '')"
                + ", cover.slm_ConverageTypeName, pl.slm_Voluntary_Gross_Premium, pl.slm_CreatedDate, pl.slm_Grade"
                + ", pl.slm_Cmt_Product_Id, pl.slm_CmtLot, mon.slm_MonthNameTh"
                //+ ", CustomerName = isnull(pl.slm_Name, '') + ' ' + isnull(pl.slm_LastName, ''), lg.slm_NewAssignType"
                + ", CustomerName = isnull(pl.slm_Name, '') + ' ' + isnull(pl.slm_LastName, '') "
                + ", pl.SLM_CAR_BY_GOV_NAME_ORG , pl.SLM_MODEL_NAME_ORG, pl.SLM_BRAND_NAME_ORG, pl.SLM_VOLUNTARY_MKT_ID "
                + ", pl.SLM_VOLUNTARY_MKT_FIRST_NAME, pl.SLM_VOLUNTARY_MKT_LAST_NAME, pl.SLM_BRANCHCODE, pl.slm_CitizenId "
                + BRN001_08_select()
                + " from ( "
                + "         select tmp.*, pt.slm_PeriodMonth, pt.slm_PeriodYear from dbo.kkslm_tr_prelead_temp tmp with (nolock) "
                + "         left join dbo.kkslm_tr_preleadportmonitor pt on tmp.slm_CmtLot = pt.slm_CMTLot and pt.is_Deleted = 0 "
                + "         where pt.slm_CMTLot = '" + lot.ToString() + "'"
                + "       ) pl "
                + " left join " + SLMConstant.OPERDBName + ".dbo.kkslm_ms_ins_com com on com.slm_InsCode = pl.slm_Voluntary_Company_Code and com.is_Deleted = 0 "
                + " left join dbo.kkslm_ms_coveragetype cover on cover.slm_CoverageTypeId = pl.slm_Voluntary_Type_Key"
                //+ " left join slmdb.dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Eff_Date)"
                + " left join dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Exp_Date)"
                //+ " left join (select slm_cmtlot, slm_cmt_product_id, slm_NewAssignType, row_number() over (partition by slm_cmtlot, slm_cmt_product_id order by slm_createddate desc) as cnt from dbo.kkslm_tr_prelead_assign_log) lg on lg.slm_CmtLot = pl.slm_CmtLot and lg.slm_Cmt_Product_Id = pl.slm_Cmt_Product_Id and cnt = 1 "
                + BRN001_08_fromJoin()
                + " where pl.slm_CmtLot = {0} and pl.slm_Type = 'DD'");

            if (name != string.Empty)
            {
                command.AppendFormat(" and pl.slm_Name like '%{0}%'", name);
            }
            if (lastname != string.Empty)
            {
                command.AppendFormat(" and pl.slm_LastName like '%{0}%'", lastname);
            }
            if (cardType != "-1")
            {
                command.AppendFormat(" and pl.slm_CardTypeId = '{0}'", cardType);
            }
            if (citizenId != string.Empty)
            {
                command.AppendFormat(" and pl.slm_CitizenId = '{0}'", citizenId);
            }
            if (campaignId != string.Empty && campaignId != "-1")
            {
                command.AppendFormat(" and pl.slm_CampaignId = '{0}'", campaignId);
            }
            if (createDate.Date != (new DateTime()).Date)
            {
                string date = createDate.Date.Year.ToString() + createDate.Date.ToString("-MM-dd");
                command.AppendFormat(" and CONVERT(DATE, pl.slm_CreatedDate) = '{0}'", date);
            }

            command.Append(" order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc ");


            string commandText = String.Format(command.ToString(), lot);
            commandText = String.Format(commandText, lot);

            using (var slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SlmScr032SearchResult>(commandText).ToList();
            }

            //return getSelectResult(commandText, out resultDs);
        }

        public List<SlmScr032SearchResult> GetBlackListList(int lot, string name, string lastname, string cardType, string citizenId, string campaignId, DateTime createDate, out DataSet resultDs)
        {
            resultDs = new DataSet();
            StringBuilder command = new StringBuilder("select Row_Number() OVER(order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc) as No"
                + ", pl.slm_TempId, pl.slm_Contract_Number, slm_Name = isnull(pl.slm_Name, ''), slm_LastName = isnull(pl.slm_LastName, ''), slm_InsNameTh = isnull(com.slm_InsNameTh, '')"
                + ", cover.slm_ConverageTypeName, pl.slm_Voluntary_Gross_Premium, pl.slm_CreatedDate, pl.slm_Grade"
                + ", pl.slm_Cmt_Product_Id, pl.slm_CmtLot, mon.slm_MonthNameTh"
                //+ ", CustomerName = isnull(pl.slm_Name, '') + ' ' + isnull(pl.slm_LastName, ''), lg.slm_NewAssignType"
                + ", CustomerName = isnull(pl.slm_Name, '') + ' ' + isnull(pl.slm_LastName, '') "
                + ", pl.SLM_CAR_BY_GOV_NAME_ORG, pl.SLM_MODEL_NAME_ORG, pl.SLM_BRAND_NAME_ORG, pl.SLM_VOLUNTARY_MKT_ID "
                + ", pl.SLM_VOLUNTARY_MKT_FIRST_NAME, pl.SLM_VOLUNTARY_MKT_LAST_NAME, pl.SLM_BRANCHCODE, pl.slm_CitizenId "
                + BRN001_08_select()
                + " from ( "
                + "         select tmp.*, pt.slm_PeriodMonth, pt.slm_PeriodYear from dbo.kkslm_tr_prelead_temp tmp with (nolock) "
                + "         left join dbo.kkslm_tr_preleadportmonitor pt on tmp.slm_CmtLot = pt.slm_CMTLot and pt.is_Deleted = 0 "
                + "         where pt.slm_CMTLot = '" + lot.ToString() + "'"
                + "       ) pl "
                + " left join " + SLMConstant.OPERDBName + ".dbo.kkslm_ms_ins_com com on com.slm_InsCode = pl.slm_Voluntary_Company_Code and com.is_Deleted = 0 "
                + " left join dbo.kkslm_ms_coveragetype cover on cover.slm_CoverageTypeId = pl.slm_Voluntary_Type_Key"
                //+ " left join slmdb.dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Eff_Date)"
                + " left join dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Exp_Date)"
                //+ " left join (select slm_cmtlot, slm_cmt_product_id, slm_NewAssignType, row_number() over (partition by slm_cmtlot, slm_cmt_product_id order by slm_createddate desc) as cnt from dbo.kkslm_tr_prelead_assign_log) lg on lg.slm_CmtLot = pl.slm_CmtLot and lg.slm_Cmt_Product_Id = pl.slm_Cmt_Product_Id and cnt = 1 "
                + BRN001_08_fromJoin()
                + " where pl.slm_CmtLot = {0} and pl.slm_Type = 'BL'");

            if (name != string.Empty)
            {
                command.AppendFormat(" and pl.slm_Name like '%{0}%'", name);
            }
            if (lastname != string.Empty)
            {
                command.AppendFormat(" and pl.slm_LastName like '%{0}%'", lastname);
            }
            if (cardType != "-1")
            {
                command.AppendFormat(" and pl.slm_CardTypeId = '{0}'", cardType);
            }
            if (citizenId != string.Empty)
            {
                command.AppendFormat(" and pl.slm_CitizenId = '{0}'", citizenId);
            }
            if (campaignId != string.Empty && campaignId != "-1")
            {
                command.AppendFormat(" and pl.slm_CampaignId = '{0}'", campaignId);
            }
            if (createDate.Date != (new DateTime()).Date)
            {
                string date = createDate.Date.Year.ToString() + createDate.Date.ToString("-MM-dd");
                command.AppendFormat(" and CONVERT(DATE, pl.slm_CreatedDate) = '{0}'", date);
            }

            string commandText = String.Format(command.ToString(), lot);
            commandText = String.Format(commandText, lot);

            command.Append(" order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc ");

            using (var slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SlmScr032SearchResult>(commandText).ToList();
            }

            //return getSelectResult(commandText, out resultDs);
        }

        public List<SlmScr032SearchResult> GetExceptionalList(int lot, string name, string lastname, string cardType, string citizenId, string campaignId, DateTime createDate, out DataSet resultDs)
        {
            resultDs = new DataSet();
            StringBuilder command = new StringBuilder("select Row_Number() OVER(order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc) as No"
                + ", pl.slm_Prelead_Id, pl.slm_Contract_Number, slm_Name = isnull(pl.slm_Name, ''), slm_LastName = isnull(pl.slm_LastName, ''), slm_InsNameTh = isnull(com.slm_InsNameTh, '')"
                + ", cover.slm_ConverageTypeName, pl.slm_Voluntary_Gross_Premium, pl.slm_CreatedDate, pl.slm_Grade"
                + ", typ.slm_Assign_Type_Name"
                + ", pl.slm_AssignDescription , pl.slm_Cmt_Product_Id, pl.slm_CmtLot, mon.slm_MonthNameTh"
                //+ ", CustomerName = isnull(pl.slm_Name, '') + ' ' + isnull(pl.slm_LastName, ''), lg.slm_NewAssignType"
                + ", CustomerName = isnull(pl.slm_Name, '') + ' ' + isnull(pl.slm_LastName, '') "
                + ", pl.SLM_CAR_BY_GOV_NAME_ORG, pl.SLM_MODEL_NAME_ORG, pl.SLM_BRAND_NAME_ORG, pl.SLM_VOLUNTARY_MKT_ID "
                + ", pl.SLM_VOLUNTARY_MKT_FIRST_NAME, pl.SLM_VOLUNTARY_MKT_LAST_NAME, pl.SLM_BRANCHCODE, pl.slm_CitizenId "
                + BRN001_08_select()
                + " from ( "
                            + @" select prelead.slm_Grade, prelead.slm_Voluntary_Gross_Premium, prelead.slm_Prelead_Id, prelead.slm_Contract_Number
                                , prelead.slm_Name, prelead.slm_LastName, prelead.slm_CreatedDate
                                , prelead.slm_AssignDescription, prelead.slm_Cmt_Product_Id, prelead.slm_CmtLot, prelead.SLM_CAR_BY_GOV_NAME_ORG
                                , prelead.SLM_MODEL_NAME_ORG, prelead.SLM_BRAND_NAME_ORG, prelead.SLM_VOLUNTARY_MKT_ID, prelead.SLM_VOLUNTARY_MKT_FIRST_NAME
                                , prelead.SLM_VOLUNTARY_MKT_LAST_NAME, prelead.SLM_BRANCHCODE, prelead.slm_CitizenId, prelead.slm_Chassis_No, prelead.slm_Engine_No
                                , prelead.slm_Car_License_No, prelead.slm_ProvinceRegis_Org, prelead.slm_Voluntary_Policy_Number, prelead.slm_Voluntary_Policy_Exp_Date
                                , prelead.slm_Owner, prelead.slm_Voluntary_Company_Code, prelead.slm_Voluntary_Type_Key, prelead.slm_AssignType, prelead.slm_AssignFlag
                                , pt.slm_PeriodMonth, pt.slm_PeriodYear, prelead.slm_Owner_Team, prelead.slm_CardTypeId, prelead.slm_CampaignId
                                from dbo.kkslm_tr_prelead prelead with (nolock) 
                                left join dbo.kkslm_tr_preleadportmonitor pt on prelead.slm_CmtLot = pt.slm_CMTLot and pt.is_Deleted = 0
                                where pt.slm_CMTLot = '" + lot.ToString() + "' "
                + "      ) pl"
                + " left join " + SLMConstant.OPERDBName + ".dbo.kkslm_ms_ins_com com on com.slm_InsCode = pl.slm_Voluntary_Company_Code and com.is_Deleted = 0 "
                + " left join dbo.kkslm_ms_coveragetype cover on cover.slm_CoverageTypeId = pl.slm_Voluntary_Type_Key"
                + " left join dbo.kkslm_ms_assign_type typ on typ.slm_Assign_Type_AbbName = pl.slm_AssignType"
                //+ " left join slmdb.dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Eff_Date)"
                + " left join dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Exp_Date)"
                //+ " left join (select slm_cmtlot, slm_cmt_product_id, slm_NewAssignType, row_number() over (partition by slm_cmtlot, slm_cmt_product_id order by slm_createddate desc) as cnt from dbo.kkslm_tr_prelead_assign_log) lg on lg.slm_CmtLot = pl.slm_CmtLot and lg.slm_Cmt_Product_Id = pl.slm_Cmt_Product_Id and cnt = 1 "
                + BRN001_08_fromJoin()
                + " where pl.slm_CmtLot = {0} and pl.slm_AssignFlag = '2'");

            if (name != string.Empty)
            {
                command.AppendFormat(" and pl.slm_Name like '%{0}%'", name);
            }
            if (lastname != string.Empty)
            {
                command.AppendFormat(" and pl.slm_LastName like '%{0}%'", lastname);
            }
            if (cardType != "-1")
            {
                command.AppendFormat(" and pl.slm_CardTypeId = '{0}'", cardType);
            }
            if (citizenId != string.Empty)
            {
                command.AppendFormat(" and pl.slm_CitizenId = '{0}'", citizenId);
            }
            if (campaignId != string.Empty && campaignId != "-1")
            {
                command.AppendFormat(" and pl.slm_CampaignId = '{0}'", campaignId);
            }
            if (createDate.Date != (new DateTime()).Date)
            {
                string date = createDate.Date.Year.ToString() + createDate.Date.ToString("-MM-dd");
                command.AppendFormat(" and CONVERT(DATE, pl.slm_CreatedDate) = '{0}'", date);
            }

            string commandText = String.Format(command.ToString(), lot);
            commandText = String.Format(commandText, lot);

            command.Append(" order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc ");

            using (var slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SlmScr032SearchResult>(commandText).ToList();
            }

            //return getSelectResult(commandText, out resultDs);
        }

        public List<SlmScr032SearchResult> GetNoLotList(int lot, string name, string lastname, string cardType, string citizenId, string campaignId, DateTime createDate, string ownerTeam, string owner, out DataSet resultDs)
        {
            resultDs = new DataSet();
            StringBuilder command = new StringBuilder("select Row_Number() OVER(order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc) as No"
                + ", pl.slm_Prelead_Id, team.slm_TeamTelesales_Code, staff.slm_StaffNameTH, staff.slm_EmpCode"
                + ", pl.slm_Contract_Number, pl.slm_Name, pl.slm_LastName, com.slm_InsNameTh"
                + ", cover.slm_ConverageTypeName, pl.slm_Voluntary_Gross_Premium, pl.slm_CreatedDate, pl.slm_Grade"
                + ", typ.slm_Assign_Type_Name, pl.slm_AssignDescription, pl.slm_Cmt_Product_Id, pl.slm_CmtLot"
                + ", mon.slm_MonthNameTh, CustomerName = IsNull(pl.slm_Name,'') + ' ' + isnull(pl.slm_LastName, '') "
                + BRN001_08_selectLite()
             + " from dbo.kkslm_tr_prelead pl "
                + " left join dbo.kkslm_ms_staff staff on staff.slm_EmpCode = pl.slm_Owner and staff.is_Deleted = 0 "
                + " left join dbo.kkslm_ms_teamtelesales team on team.slm_TeamTelesales_Id = staff.slm_TeamTelesales_Id"
                + " left join " + SLMConstant.OPERDBName + ".dbo.kkslm_ms_ins_com com on com.slm_InsCode = pl.slm_Voluntary_Company_Code and com.is_Deleted = 0"
                + " left join dbo.kkslm_ms_coveragetype cover on cover.slm_CoverageTypeId = pl.slm_Voluntary_Type_Key"
                + " left join dbo.kkslm_ms_assign_type typ on typ.slm_Assign_Type_AbbName = pl.slm_AssignType"
                //+ " left join slmdb.dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Eff_Date)"
                + " left join dbo.kkslm_ms_month mon on mon.slm_MonthId = MONTH(pl.slm_Voluntary_Policy_Exp_Date)"
                + BRN001_08_fromJoin()
            + " where pl.slm_AssignFlag = '1' and pl. slm_Assign_Status ='2'");

            if (name != string.Empty)
                command.AppendFormat(" and pl.slm_Name like '%{0}%'", name);
            if (lastname != string.Empty)
                command.AppendFormat(" and pl.slm_LastName like '%{0}%'", lastname);
            if (cardType != "-1")
                command.AppendFormat(" and pl.slm_CardTypeId = '{0}'", cardType);
            if (citizenId != string.Empty)
                command.AppendFormat(" and pl.slm_CitizenId = '{0}'", citizenId);
            if (campaignId != string.Empty && campaignId != "-1")
                command.AppendFormat(" and pl.slm_CampaignId = '{0}'", campaignId);
            if (createDate.Date != (new DateTime()).Date)
            {
                //command.AppendFormat(" and CONVERT(DATE, pl.slm_CreatedDate) = '{0:yyyy-MM-dd}'", createDate.Date);

                string date = createDate.Date.Year.ToString() + createDate.Date.ToString("-MM-dd");
                command.AppendFormat(" and CONVERT(DATE, pl.slm_CreatedDate) = '{0}'", date);
            }
            if (ownerTeam != "-1")
                command.AppendFormat(" and pl.slm_Owner_Team = '{0}'", ownerTeam);
            if (owner != "-1" && owner != string.Empty)
                command.AppendFormat(" and pl.slm_Owner = '{0}'", owner);

            command.Append(" order by pl.slm_Grade, pl.slm_Voluntary_Gross_Premium desc ");

            string commandText = String.Format(command.ToString(), lot);

            return getSelectResult(commandText, out resultDs);
        }

        public bool UpdateDelegateList(decimal lot, List<SlmScr032SearchResult> successList, List<SlmScr032SearchResult> dedupList, List<SlmScr032SearchResult> blacklistList, List<SlmScr032SearchResult> exceptionalList, string username)
        {
            bool ret = true;
            _error = "";
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var tempForcecList = slmdb.kkslm_tr_prelead_temp_force.Where(p => p.slm_CmtLot == lot).ToList();
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        #region success
                        //may slow down process
                        //successList.ForEach(data =>
                        //{
                        //    kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                        //    {
                        //        slm_Cmt_Product_Id = data.slm_Cmt_Product_Id,
                        //        slm_CmtLot = data.slm_CmtLot,
                        //        slm_NewAssignType = "CP",
                        //        slm_ContractNo = data.slm_Contract_Number,
                        //        slm_CreatedBy = username,
                        //        slm_CreatedDate = DateTime.Now
                        //    };
                        //    slmdb.kkslm_tr_prelead_assign_log.AddObject(log);
                        //});

                        string succUpdCmd = " update kkslm_tr_prelead set slm_Assign_Status = '1', slm_UpdatedDate = getdate(), slm_updatedby = '" + username + "' where slm_CmtLot = " + lot.ToString() + " and slm_AssignFlag = '0'";
                        #endregion success

                        #region dedup
                        dedupList.ForEach(data =>
                        {
                            kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                            {
                                slm_Cmt_Product_Id = data.slm_Cmt_Product_Id,
                                slm_CmtLot = data.slm_CmtLot,
                                slm_NewAssignType = "CD",
                                slm_ContractNo = data.slm_Contract_Number,
                                slm_CreatedBy = username,
                                slm_CreatedDate = DateTime.Now
                            };
                            slmdb.kkslm_tr_prelead_assign_log.AddObject(log);
                        });

                        string ddpDeleteCmd = " delete from kkslm_tr_prelead_temp where slm_CmtLot = '" + lot.ToString() + "' and slm_Type = 'DD' ";
                        #endregion dedup

                        #region blacklist
                        blacklistList.ForEach(data =>
                        {
                            kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                            {
                                slm_Cmt_Product_Id = data.slm_Cmt_Product_Id,
                                slm_CmtLot = data.slm_CmtLot,
                                slm_NewAssignType = "CB",
                                slm_ContractNo = data.slm_Contract_Number,
                                slm_CreatedBy = username,
                                slm_CreatedDate = DateTime.Now
                            };
                            slmdb.kkslm_tr_prelead_assign_log.AddObject(log);
                        });

                        string bcklDeleteCmd = " delete from kkslm_tr_prelead_temp where slm_CmtLot = '" + lot.ToString() + "' and slm_Type = 'BL'";
                        #endregion blacklist

                        #region Exceptional
                        exceptionalList.ForEach(data =>
                        {
                            kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                            {
                                slm_Cmt_Product_Id = data.slm_Cmt_Product_Id,
                                slm_CmtLot = data.slm_CmtLot,
                                slm_NewAssignType = "CE",
                                slm_ContractNo = data.slm_Contract_Number,
                                slm_CreatedBy = username,
                                slm_CreatedDate = DateTime.Now
                            };
                            slmdb.kkslm_tr_prelead_assign_log.AddObject(log);
                        });

                        string excpDeleteCmd = " delete from kkslm_tr_prelead where slm_CmtLot = " + lot.ToString() + " and slm_AssignFlag = '2' ";
                        #endregion Exceptional

                        string update_sql = "";
                        kkslm_tr_preleadportmonitor port = slmdb.kkslm_tr_preleadportmonitor.FirstOrDefault(p => p.slm_CMTLot == lot);
                        if (port != null)
                        {
                            port.slm_PortStatus = "C";
                            port.slm_UpdatedBy = username;
                            port.slm_UpdatedDate = DateTime.Now;

                            update_sql = "UPDATE kkslm_tr_prelead SET slm_PeriodMonth = '" + port.slm_PeriodMonth + "', slm_PeriodYear = '" + port.slm_PeriodYear + "' WHERE slm_CmtLot = '" + lot.ToString() + "' AND (slm_PeriodMonth IS NULL OR slm_PeriodYear IS NULL) ";
                        }

                        slmdb.SaveChanges();

                        if (successList.Count > 0)
                        {
                            slmdb.ExecuteStoreCommand(succUpdCmd);
                        }

                        //Update PeriodMonth, PeriodYear ของ Prelead ที่ถูก Transfer มาจากการ reject โดย Rule
                        if (!string.IsNullOrEmpty(update_sql))
                        {
                            slmdb.ExecuteStoreCommand(update_sql);
                        }

                        //Update ข้อมูลจาก kkslm_tr_prelead_temp_force ไปที่ kkslm_tr_prelead
                        if (tempForcecList.Count > 0)
                        {
                            var tempIdList = tempForcecList.Select(p => p.slm_TempId).ToList();
                            var preleadList = slmdb.kkslm_tr_prelead.Where(p => p.slm_CmtLot == lot && tempIdList.Contains(p.slm_TempId)).ToList();
                            foreach (var prelead in preleadList)
                            {
                                var tempForce = tempForcecList.Where(p => p.slm_TempId == prelead.slm_TempId).FirstOrDefault();
                                if (tempForce != null)
                                {
                                    prelead.slm_Reference1 = tempForce.slm_Reference1;
                                    prelead.slm_Reference2 = tempForce.slm_Reference2;
                                    prelead.slm_Contract_App_Date = tempForce.slm_Contract_App_Date;
                                }
                            }
                            slmdb.SaveChanges();
                            slmdb.ExecuteStoreCommand(string.Format("delete from kkslm_tr_prelead_temp_force where slm_CmtLot = {0} ", lot.ToString()));
                        }

                        if (dedupList.Count > 0)
                        {
                            slmdb.ExecuteStoreCommand(ddpDeleteCmd);
                        }
                        if (blacklistList.Count > 0)
                        {
                            slmdb.ExecuteStoreCommand(bcklDeleteCmd);
                        }
                        if (exceptionalList.Count > 0)
                        {
                            slmdb.ExecuteStoreCommand(excpDeleteCmd);
                        }

                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ret = false;

            }
            return ret;
        }

        public bool RejectDelegateList(decimal lot, List<SlmScr032SearchResult> successList, List<decimal> dedupIds, List<decimal> blacklistIds, List<SlmScr032SearchResult> exceptionalList, string username)
        {
            bool ret = true;
            _error = "";
            try
            {
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        // เปลี่ยนเป็น reject ยกล็อต Success (0), Exceptional (2)
                        string sql_reject_succ = @" 
                                update kkslm_tr_prelead set 
                                slm_AssignFlag = null, 
                                slm_Owner = null, 
                                slm_Owner_Position = null, 
                                slm_Owner_Team = null, 
                                slm_OwnerBranch = null, 
                                slm_UpdatedBy = @UpdatedBy, 
                                slm_UpdatedDate = getdate() 
                                where slm_CmtLot = @CmtLot and slm_AssignFlag in ('0', '2') ";

                        slmdb.ExecuteStoreCommand(sql_reject_succ, new object[] {
                                new SqlParameter("@UpdatedBy", username),
                                new SqlParameter("@CmtLot", lot)
                                });

                        #region dedup
                        var tmpDedupList = slmdb.kkslm_tr_prelead_temp.Where(d => dedupIds.Contains(d.slm_TempId)).ToList();
                        tmpDedupList.ForEach(data =>
                        {
                            //Comment by Pom 27/07/2016 - may slow down a process
                            //var tmp = slmdb.kkslm_tr_prelead_assign_log
                            //                    .Where(l => l.slm_CmtLot == data.slm_CmtLot && l.slm_Cmt_Product_Id == data.slm_Cmt_Product_Id)
                            //                    .OrderByDescending(l => l.slm_CreatedDate)
                            //                    .FirstOrDefault();
                            //string oldStatus = tmp == null ? "" : tmp.slm_NewAssignType;

                            kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                            {
                                slm_Cmt_Product_Id = data.slm_Cmt_Product_Id,
                                slm_CmtLot = data.slm_CmtLot,
                                slm_OldAssignType = null,       //oldStatus,
                                slm_NewAssignType = "DF",
                                slm_ContractNo = data.slm_Contract_Number,
                                slm_CreatedBy = username,
                                slm_CreatedDate = DateTime.Now
                            };
                            slmdb.kkslm_tr_prelead_assign_log.AddObject(log);

                            kkslm_tr_prelead_temp_force force = new kkslm_tr_prelead_temp_force
                            {
                                slm_TempId = data.slm_TempId,
                                slm_CmtLot = data.slm_CmtLot,
                                slm_Contract_Number = data.slm_Contract_Number,
                                slm_Reference1 = data.slm_Reference1,
                                slm_Reference2 = data.slm_Reference2,
                                slm_Contract_App_Date = data.slm_Contract_App_Date,
                                slm_CreatedBy = username,
                                slm_CreatedDate = DateTime.Now,
                                slm_UpdatedBy = username,
                                slm_UpdatedDate = DateTime.Now
                            };
                            slmdb.kkslm_tr_prelead_temp_force.AddObject(force);

                            data.slm_IsForce = true;
                            data.slm_UpdatedBy = username;
                            data.slm_UpdatedDate = DateTime.Now;
                        });
                        #endregion dedup

                        #region blacklist
                        var blacklistList = slmdb.kkslm_tr_prelead_temp.Where(b => blacklistIds.Contains(b.slm_TempId)).ToList();
                        blacklistList.ForEach(data =>
                        {
                            //Comment by Pom 27/07/2016 - may slow down a process
                            //var tmp = slmdb.kkslm_tr_prelead_assign_log
                            //                    .Where(l => l.slm_CmtLot == data.slm_CmtLot && l.slm_Cmt_Product_Id == data.slm_Cmt_Product_Id)
                            //                    .OrderByDescending(l => l.slm_CreatedDate)
                            //                    .FirstOrDefault();
                            //string oldStatus = tmp == null ? "" : tmp.slm_NewAssignType;

                            kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                            {
                                slm_Cmt_Product_Id = data.slm_Cmt_Product_Id,
                                slm_CmtLot = data.slm_CmtLot,
                                slm_OldAssignType = null,       //oldStatus,
                                slm_NewAssignType = "BF",
                                slm_ContractNo = data.slm_Contract_Number,
                                slm_CreatedBy = username,
                                slm_CreatedDate = DateTime.Now
                            };
                            slmdb.kkslm_tr_prelead_assign_log.AddObject(log);

                            kkslm_tr_prelead_temp_force force = new kkslm_tr_prelead_temp_force
                            {
                                slm_TempId = data.slm_TempId,
                                slm_CmtLot = data.slm_CmtLot,
                                slm_Contract_Number = data.slm_Contract_Number,
                                slm_Reference1 = data.slm_Reference1,
                                slm_Reference2 = data.slm_Reference2,
                                slm_Contract_App_Date = data.slm_Contract_App_Date,
                                slm_CreatedBy = username,
                                slm_CreatedDate = DateTime.Now,
                                slm_UpdatedBy = username,
                                slm_UpdatedDate = DateTime.Now
                            };
                            slmdb.kkslm_tr_prelead_temp_force.AddObject(force);

                            data.slm_IsForce = true;
                            data.slm_UpdatedBy = username;
                            data.slm_UpdatedDate = DateTime.Now;
                        });

                        #endregion blacklist

                        kkslm_tr_preleadportmonitor port = slmdb.kkslm_tr_preleadportmonitor.FirstOrDefault(p => p.slm_CMTLot == lot);
                        if (port != null)
                        {
                            port.slm_PortStatus = "R";
                            port.slm_UpdatedBy = username;
                            port.slm_UpdatedDate = DateTime.Now;
                        }
                        slmdb.SaveChanges();

                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ret = false;

            }
            return ret;
        }

        //public bool ValidateImportData(DataTable dt, out List<SlmScr032SearchResult> result)
        //{
        //    result = new List<SlmScr032SearchResult>();
        //    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
        //    {
        //        slmdb.CommandTimeout = SLMConstant.SLMDBCommandTimeout;

        //        //var staffs = slmdb.kkslm_ms_staff.ToList();
        //        //var teles = slmdb.kkslm_ms_teamtelesales.ToList();

        //        var staffs = slmdb.kkslm_ms_staff.Select(p => p.slm_EmpCode).ToList();
        //        var teles = slmdb.kkslm_ms_teamtelesales.Select(p => p.slm_TeamTelesales_Code).ToList();

        //        int icount = 2;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            SlmScr032SearchResult item = new SlmScr032SearchResult() { RowNo = icount++, ErrorMessage = "" };
        //            if (string.Join("", dr.ItemArray.OfType<string>().ToArray()).Trim() == string.Empty)
        //                continue;
        //            item.slm_TeamTelesales_Code = dr[0].ToString();
        //            item.slm_EmpCode = dr[1].ToString();

        //            decimal id;
        //            if (dt.Columns[10].ColumnName.ToLower() == "prelead id")
        //            {
        //                if (decimal.TryParse(dr[10].ToString(), out id))
        //                {
        //                    item.slm_Prelead_Id = id;
        //                }
        //                if (decimal.TryParse(dr[11].ToString(), out id))
        //                {
        //                    item.slm_CmtLot = id;
        //                }
        //            }
        //            else
        //            {
        //                if (decimal.TryParse(dr[10].ToString(), out id))
        //                {
        //                    item.slm_TempId = id;
        //                }
        //                if (decimal.TryParse(dr[11].ToString(), out id))
        //                {
        //                    item.slm_CmtLot = id;
        //                }
        //            }

        //            var stf = staffs.FirstOrDefault(s => s == item.slm_EmpCode);
        //            var tele = teles.FirstOrDefault(t => t == item.slm_TeamTelesales_Code);

        //            if (item.slm_TeamTelesales_Code != string.Empty && tele == null)
        //            {
        //                item.ErrorMessage = "Column A : ไม่พบข้อมูลชื่อทีมในระบบ";
        //            }
        //            if (item.slm_EmpCode == string.Empty)
        //            {
        //                item.ErrorMessage += (item.ErrorMessage == "" ? "" : ",<br>") + "Column B : ไม่พบข้อมูลรหัสพนักงาน";
        //            }
        //            //if (stf != null && tele != null && stf.slm_TeamTelesales_Id != tele.slm_TeamTelesales_Id)
        //            //    item.ErrorMessage += (item.ErrorMessage == "" ? "" : ",<br>") + "ชื่อทีมกับรหัสพนักงานไม่สัมพันธ์กัน";
        //            if (stf == null)
        //            {
        //                item.ErrorMessage += (item.ErrorMessage == "" ? "" : ",<br>") + "Column B : ไม่พบข้อมูลรหัสพนักงานในระบบ";
        //            }

        //            result.Add(item);
        //        }
        //    }

        //    return result.Count > 0 && (result.FirstOrDefault(r => r.ErrorMessage != null && r.ErrorMessage != string.Empty) == null);
        //}

        public bool ValidateImportData(List<SlmScr032ExcelData> excelData, string excelColumnName, out List<SlmScr032SearchResult> result)
        {
            result = new List<SlmScr032SearchResult>();
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.CommandTimeout = SLMConstant.SLMDBCommandTimeout;

                var staffs = slmdb.kkslm_ms_staff.Select(p => p.slm_EmpCode).ToList();
                var teles = slmdb.kkslm_ms_teamtelesales.Select(p => p.slm_TeamTelesales_Code).ToList();

                int icount = 2;
                foreach (SlmScr032ExcelData data in excelData)
                {
                    SlmScr032SearchResult item = new SlmScr032SearchResult() { RowNo = icount++, ErrorMessage = "" };

                    if (string.IsNullOrWhiteSpace(data.TeamTelesales) && string.IsNullOrWhiteSpace(data.EmpCode) && string.IsNullOrWhiteSpace(data.PreleadId) && string.IsNullOrWhiteSpace(data.Lot))
                    {
                        continue;
                    }

                    item.slm_TeamTelesales_Code = data.TeamTelesales;
                    item.slm_EmpCode = data.EmpCode;

                    decimal id;
                    if (excelColumnName.ToLower() == "prelead id")
                    {
                        if (decimal.TryParse(data.PreleadId, out id))
                        {
                            item.slm_Prelead_Id = id;
                        }
                        if (decimal.TryParse(data.Lot, out id))
                        {
                            item.slm_CmtLot = id;
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(data.PreleadId, out id))
                        {
                            item.slm_TempId = id;
                        }
                        if (decimal.TryParse(data.Lot, out id))
                        {
                            item.slm_CmtLot = id;
                        }
                    }

                    var stf = staffs.FirstOrDefault(s => s == item.slm_EmpCode);
                    var tele = teles.FirstOrDefault(t => t == item.slm_TeamTelesales_Code);

                    if (item.slm_TeamTelesales_Code != string.Empty && tele == null)
                    {
                        item.ErrorMessage = "Column A : ไม่พบข้อมูลชื่อทีมในระบบ";
                    }
                    if (item.slm_EmpCode == string.Empty)
                    {
                        item.ErrorMessage += (item.ErrorMessage == "" ? "" : ",<br>") + "Column B : ไม่พบข้อมูลรหัสพนักงาน";
                    }
                    if (stf == null)
                    {
                        item.ErrorMessage += (item.ErrorMessage == "" ? "" : ",<br>") + "Column B : ไม่พบข้อมูลรหัสพนักงานในระบบ";
                    }

                    result.Add(item);
                }
            }

            return result.Count > 0 && (result.FirstOrDefault(r => r.ErrorMessage != null && r.ErrorMessage != string.Empty) == null);
        }

        public delegate void RaiseLogHandler(string str);
        public event RaiseLogHandler RaiseLog;

        //public void DoImport(List<SlmScr032SearchResult> jobs, string username, decimal lot)
        //{
        //    if (RaiseLog != null) RaiseLog("Start import");

        //    //List<decimal> lot = jobs.Select(j => j.slm_CmtLot).Distinct().ToList();
        //    List<decimal> preleadId = jobs.Select(j => j.slm_Prelead_Id).Distinct().ToList();
        //    List<string> teamCode = jobs.Select(j => j.slm_TeamTelesales_Code).Distinct().ToList();
        //    List<string> empCode = jobs.Select(j => j.slm_EmpCode).Distinct().ToList();
        //    List<decimal> preleadTempId = jobs.Select(j => j.slm_TempId).ToList();

        //    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
        //    {
        //        slmdb.CommandTimeout = SLMConstant.SLMDBCommandTimeout;

        //        if (RaiseLog != null) RaiseLog("Prepare in-memory data");
        //        //var team = slmdb.kkslm_ms_teamtelesales.Where(t => teamCode.Contains(t.slm_TeamTelesales_Code)).ToList();
        //        //var staff = slmdb.kkslm_ms_staff.Where(s => empCode.Contains(s.slm_EmpCode)).ToList();

        //        var team = slmdb.kkslm_ms_teamtelesales.Where(t => teamCode.Contains(t.slm_TeamTelesales_Code))
        //                    .Select(p => new { slm_TeamTelesales_Code = p.slm_TeamTelesales_Code, slm_TeamTelesales_Id = p.slm_TeamTelesales_Id }).ToList();

        //        var staff = slmdb.kkslm_ms_staff.Where(s => empCode.Contains(s.slm_EmpCode))
        //                    .Select(p => new { slm_EmpCode = p.slm_EmpCode, slm_Position_id = p.slm_Position_id, slm_BranchCode = p.slm_BranchCode }).ToList();

        //        var preleadTemp = slmdb.kkslm_tr_prelead_temp.Where(p => preleadTempId.Contains(p.slm_TempId)).ToList();
        //        var records = slmdb.kkslm_tr_prelead.Where(p => preleadId.Contains(p.slm_Prelead_Id)).ToList();


        //        int i = 0;
        //        var start = DateTime.Now;
        //        int found = 0;
        //        int notfound = 0;
        //        if (RaiseLog != null) RaiseLog("Start Loop with " + jobs.Count.ToString() + " data");
        //        foreach (var item in jobs)
        //        {
        //            i += 1;

        //            if (item.slm_TempId == 0)
        //            {
        //                #region no tempId
        //                var t = records.Where(r => r.slm_CmtLot == item.slm_CmtLot && r.slm_Prelead_Id == item.slm_Prelead_Id).FirstOrDefault();
        //                if (t != null)
        //                {
        //                    notfound++;
        //                    t.slm_Owner = item.slm_EmpCode;
        //                    if (team != null)
        //                    {
        //                        var tc = team.FirstOrDefault(x => x.slm_TeamTelesales_Code == item.slm_TeamTelesales_Code);
        //                        if (tc != null)
        //                            t.slm_Owner_Team = (int)tc.slm_TeamTelesales_Id;
        //                    }

        //                    var st = staff.FirstOrDefault(x => x.slm_EmpCode == item.slm_EmpCode);
        //                    if (st != null)
        //                    {
        //                        t.slm_Owner_Position = st.slm_Position_id;
        //                        t.slm_OwnerBranch = st.slm_BranchCode;
        //                    }
        //                    t.slm_UpdatedBy = username;
        //                    t.slm_UpdatedDate = DateTime.Now;

        //                    // insert ตาราง kkslm_tr_prelead_assign_log

        //                    //Comment by Pom 27/07/2016 - may slow down a process
        //                    //var tmp = slmdb.kkslm_tr_prelead_assign_log
        //                    //                    .Where(l => l.slm_CmtLot == t.slm_CmtLot && l.slm_Cmt_Product_Id == t.slm_Cmt_Product_Id)
        //                    //                    .OrderByDescending(l => l.slm_CreatedDate)
        //                    //                    .FirstOrDefault();
        //                    //string oldStatus = tmp == null ? "" : tmp.slm_NewAssignType;

        //                    //kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
        //                    //{
        //                    //    slm_Cmt_Product_Id = t.slm_Cmt_Product_Id,
        //                    //    slm_CmtLot = t.slm_CmtLot,
        //                    //    slm_OldAssignType = null,   //oldStatus,
        //                    //    slm_NewAssignType = "UA",
        //                    //    slm_CreatedBy = username,
        //                    //    slm_CreatedDate = DateTime.Now
        //                    //};
        //                    //slmdb.kkslm_tr_prelead_assign_log.AddObject(log);
        //                }
        //                #endregion no tempId
        //            }
        //            else
        //            {
        //                #region has tempId
        //                var t = preleadTemp.FirstOrDefault(p => p.slm_TempId == item.slm_TempId && p.slm_CmtLot == item.slm_CmtLot);
        //                if (t != null)
        //                {
        //                    found++;
        //                    kkslm_tr_prelead newPrelead = new kkslm_tr_prelead();
        //                    var plPr = t.GetType().GetProperties().ToList();
        //                    newPrelead.GetType().GetProperties().ToList().ForEach(p =>
        //                    {
        //                        if (p.Name.ToLower().StartsWith("slm_") && plPr.FirstOrDefault(pl => pl.Name == p.Name) != null && p.CanWrite)
        //                        {
        //                            var value = plPr.FirstOrDefault(pl => pl.Name == p.Name).GetValue(t, null);
        //                            p.SetValue(newPrelead
        //                                , value
        //                                , null);
        //                        }
        //                    });
        //                    newPrelead.slm_Owner = item.slm_EmpCode;

        //                    if (team != null)
        //                    {
        //                        var tc = team.FirstOrDefault(x => x.slm_TeamTelesales_Code == item.slm_TeamTelesales_Code);
        //                        if (tc != null)
        //                            newPrelead.slm_Owner_Team = (int)tc.slm_TeamTelesales_Id;
        //                    }

        //                    var st = staff.FirstOrDefault(x => x.slm_EmpCode == item.slm_EmpCode);
        //                    if (st != null)
        //                    {
        //                        newPrelead.slm_Owner_Position = st.slm_Position_id;
        //                        newPrelead.slm_OwnerBranch = st.slm_BranchCode;
        //                    }

        //                    newPrelead.slm_AssignFlag = "0";
        //                    newPrelead.slm_Assign_Status = "3";
        //                    newPrelead.slm_CreatedBy = username;
        //                    newPrelead.slm_UpdatedBy = username;
        //                    newPrelead.slm_CreatedDate = DateTime.Now;
        //                    newPrelead.slm_UpdatedDate = DateTime.Now;
        //                    newPrelead.slm_ObtPro03Flag = false;
        //                    newPrelead.is_Deleted = false;
        //                    slmdb.kkslm_tr_prelead.AddObject(newPrelead);

        //                    // insert ตาราง kkslm_tr_prelead_assign_log

        //                    //Comment by Pom 27/07/2016 - may slow down a process
        //                    //var tmp = slmdb.kkslm_tr_prelead_assign_log
        //                    //                    .Where(l => l.slm_CmtLot == t.slm_CmtLot && l.slm_Cmt_Product_Id == t.slm_Cmt_Product_Id)
        //                    //                    .OrderByDescending(l => l.slm_CreatedDate)
        //                    //                    .FirstOrDefault();
        //                    //string oldStatus = tmp == null ? "" : tmp.slm_NewAssignType;

        //                    //kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
        //                    //{
        //                    //    slm_Cmt_Product_Id = t.slm_Cmt_Product_Id,
        //                    //    slm_CmtLot = t.slm_CmtLot,
        //                    //    slm_OldAssignType = null,       //oldStatus,
        //                    //    slm_NewAssignType = "UA",
        //                    //    slm_CreatedBy = username,
        //                    //    slm_CreatedDate = DateTime.Now
        //                    //};
        //                    //slmdb.kkslm_tr_prelead_assign_log.AddObject(log);

        //                    slmdb.kkslm_tr_prelead_temp.DeleteObject(t);
        //                }
        //                #endregion has tempId
        //            }

        //            if ((DateTime.Now - start).TotalSeconds >= 10) { if (RaiseLog != null) RaiseLog(String.Format("total {0}, found {1}, not found {2}", i, found, notfound)); start = DateTime.Now; }
        //        } // end foreach (var item in jobs)

        //        if (RaiseLog != null) RaiseLog("Start saving to DB " + String.Format("total {0}, found {1}, not found {2}", i, found, notfound));
        //        slmdb.SaveChanges();
        //        if (RaiseLog != null) RaiseLog("End saving to DB");

        //        DoUpdatePortMonitor(lot, username);
        //    }
        //}

        public void DoImport(List<SlmScr032SearchResult> jobs, string username, decimal lot)
        {
            if (RaiseLog != null) RaiseLog("Start import");

            //List<decimal> lot = jobs.Select(j => j.slm_CmtLot).Distinct().ToList();
            //List<decimal> preleadId = jobs.Select(j => j.slm_Prelead_Id).Distinct().ToList();
            List<string> teamCode = jobs.Select(j => j.slm_TeamTelesales_Code).Distinct().ToList();
            List<string> empCode = jobs.Select(j => j.slm_EmpCode).Distinct().ToList();
            List<decimal> preleadTempId = jobs.Select(j => j.slm_TempId).ToList();

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.CommandTimeout = SLMConstant.SLMDBCommandTimeout;

                if (RaiseLog != null) RaiseLog("Prepare in-memory data");
                //var team = slmdb.kkslm_ms_teamtelesales.Where(t => teamCode.Contains(t.slm_TeamTelesales_Code)).ToList();
                //var staff = slmdb.kkslm_ms_staff.Where(s => empCode.Contains(s.slm_EmpCode)).ToList();

                var team = slmdb.kkslm_ms_teamtelesales.Where(t => teamCode.Contains(t.slm_TeamTelesales_Code))
                            .Select(p => new { slm_TeamTelesales_Code = p.slm_TeamTelesales_Code, slm_TeamTelesales_Id = p.slm_TeamTelesales_Id }).ToList();

                var staff = slmdb.kkslm_ms_staff.Where(s => empCode.Contains(s.slm_EmpCode))
                            .Select(p => new { slm_EmpCode = p.slm_EmpCode, slm_Position_id = p.slm_Position_id, slm_BranchCode = p.slm_BranchCode }).ToList();

                var preleadTemp = slmdb.kkslm_tr_prelead_temp.Where(p => preleadTempId.Contains(p.slm_TempId)).ToList();
                //var records = slmdb.kkslm_tr_prelead.Where(p => preleadId.Contains(p.slm_Prelead_Id)).ToList();


                int i = 0;
                var start = DateTime.Now;
                int found = 0;
                int notfound = 0;
                if (RaiseLog != null) RaiseLog("Start Loop with " + jobs.Count.ToString() + " data");
                foreach (var item in jobs)
                {
                    i += 1;

                    if (item.slm_TempId == 0)
                    {
                        #region no tempId
                        //var t = records.Where(r => r.slm_CmtLot == item.slm_CmtLot && r.slm_Prelead_Id == item.slm_Prelead_Id).FirstOrDefault();
                        //if (t != null)
                        //{
                        //    notfound++;
                        //    t.slm_Owner = item.slm_EmpCode;
                        //    if (team != null)
                        //    {
                        //        var tc = team.FirstOrDefault(x => x.slm_TeamTelesales_Code == item.slm_TeamTelesales_Code);
                        //        if (tc != null)
                        //            t.slm_Owner_Team = (int)tc.slm_TeamTelesales_Id;
                        //    }

                        //    var st = staff.FirstOrDefault(x => x.slm_EmpCode == item.slm_EmpCode);
                        //    if (st != null)
                        //    {
                        //        t.slm_Owner_Position = st.slm_Position_id;
                        //        t.slm_OwnerBranch = st.slm_BranchCode;
                        //    }
                        //    t.slm_UpdatedBy = username;
                        //    t.slm_UpdatedDate = DateTime.Now;
                        //}

                        string setStr = $" slm_Owner = '{item.slm_EmpCode}', slm_UpdatedBy = '{username}', slm_UpdatedDate = GETDATE() ";

                        if (team != null)
                        {
                            var tc = team.FirstOrDefault(x => x.slm_TeamTelesales_Code == item.slm_TeamTelesales_Code);
                            if (tc != null)
                            {
                                setStr += $", slm_Owner_Team = '{tc.slm_TeamTelesales_Id.ToString()}' ";
                            }
                        }
                        var st = staff.FirstOrDefault(x => x.slm_EmpCode == item.slm_EmpCode);
                        if (st != null)
                        {
                            string ownerpos = st.slm_Position_id != null ? ("'" + st.slm_Position_id.ToString() + "'") : "NULL";
                            string ownerbranch = st.slm_BranchCode != null ? ("'" + st.slm_BranchCode.ToString() + "'") : "NULL";
                            setStr += $", slm_Owner_Position = {ownerpos} ";
                            setStr += $", slm_OwnerBranch = {ownerbranch} ";
                        }

                        string sql = $@"UPDATE kkslm_tr_prelead 
                                        SET {setStr}
                                        WHERE slm_CmtLot = '{item.slm_CmtLot.ToString()}' AND slm_Prelead_Id = '{item.slm_Prelead_Id.ToString()}' ";

                        slmdb.ExecuteStoreCommand(sql);

                        #endregion no tempId
                    }
                    else
                    {
                        #region has tempId
                        var t = preleadTemp.FirstOrDefault(p => p.slm_TempId == item.slm_TempId && p.slm_CmtLot == item.slm_CmtLot);
                        if (t != null)
                        {
                            found++;
                            kkslm_tr_prelead newPrelead = new kkslm_tr_prelead();
                            var plPr = t.GetType().GetProperties().ToList();
                            newPrelead.GetType().GetProperties().ToList().ForEach(p =>
                            {
                                if (p.Name.ToLower().StartsWith("slm_") && plPr.FirstOrDefault(pl => pl.Name == p.Name) != null && p.CanWrite)
                                {
                                    var value = plPr.FirstOrDefault(pl => pl.Name == p.Name).GetValue(t, null);
                                    p.SetValue(newPrelead
                                        , value
                                        , null);
                                }
                            });
                            newPrelead.slm_Owner = item.slm_EmpCode;

                            if (team != null)
                            {
                                var tc = team.FirstOrDefault(x => x.slm_TeamTelesales_Code == item.slm_TeamTelesales_Code);
                                if (tc != null)
                                    newPrelead.slm_Owner_Team = (int)tc.slm_TeamTelesales_Id;
                            }

                            var st = staff.FirstOrDefault(x => x.slm_EmpCode == item.slm_EmpCode);
                            if (st != null)
                            {
                                newPrelead.slm_Owner_Position = st.slm_Position_id;
                                newPrelead.slm_OwnerBranch = st.slm_BranchCode;
                            }

                            newPrelead.slm_AssignFlag = "0";
                            newPrelead.slm_Assign_Status = "3";
                            newPrelead.slm_CreatedBy = username;
                            newPrelead.slm_UpdatedBy = username;
                            newPrelead.slm_CreatedDate = DateTime.Now;
                            newPrelead.slm_UpdatedDate = DateTime.Now;
                            newPrelead.slm_ObtPro03Flag = false;
                            newPrelead.is_Deleted = false;
                            slmdb.kkslm_tr_prelead.AddObject(newPrelead);

                            slmdb.kkslm_tr_prelead_temp.DeleteObject(t);
                        }
                        #endregion has tempId
                    }

                    //if ((DateTime.Now - start).TotalSeconds >= 10) { if (RaiseLog != null) RaiseLog(String.Format("total {0}, found {1}, not found {2}", i, found, notfound)); start = DateTime.Now; }
                } // end foreach (var item in jobs)

                if (RaiseLog != null) RaiseLog("Start saving to DB " + String.Format("total {0}, found {1}, not found {2}", i, found, notfound));
                slmdb.SaveChanges();
                if (RaiseLog != null) RaiseLog("End saving to DB");

                DoUpdatePortMonitor(lot, username);
            }
        }

        private void DoUpdatePortMonitor(decimal lot, string username)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    int success = slmdb.kkslm_tr_prelead.Where(p => p.slm_CmtLot == lot && p.is_Deleted == false).Count();
                    int dedup = slmdb.kkslm_tr_prelead_temp.Where(p => p.slm_CmtLot == lot && p.slm_Type == "DD" && p.is_Deleted == false).Count();
                    int blacklist = slmdb.kkslm_tr_prelead_temp.Where(p => p.slm_CmtLot == lot && p.slm_Type == "BL" && p.is_Deleted == false).Count();

                    var port = slmdb.kkslm_tr_preleadportmonitor.Where(p => p.slm_CMTLot == lot && p.is_Deleted == false).FirstOrDefault();
                    if (port != null)
                    {
                        port.slm_Success = success;
                        port.slm_DeDup = dedup;
                        port.slm_Blaklist = blacklist;
                        port.slm_UpdatedBy = username;
                        port.slm_UpdatedDate = DateTime.Now;
                    }

                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                string errorDetail = "Cannot update port monitor, Error=" + message;
                if (RaiseLog != null) RaiseLog(errorDetail);
            }
        }

        public void DoNoLotImport(List<SlmScr032SearchResult> jobs, string username)
        {
            List<decimal> preleadId = jobs.Select(j => j.slm_Prelead_Id).Distinct().ToList();
            List<string> teamCode = jobs.Select(j => j.slm_TeamTelesales_Code).ToList();
            List<string> empCode = jobs.Select(j => j.slm_EmpCode).ToList();

            //List<decimal> lot = jobs.Select(j => j.slm_CmtLot).Distinct().ToList();
            //List<decimal> preleadTempId = jobs.Select(j => j.slm_TempId).ToList();

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.CommandTimeout = SLMConstant.SLMDBCommandTimeout;

                if (RaiseLog != null) RaiseLog("Preppare in-mem data");

                var team = slmdb.kkslm_ms_teamtelesales.Where(t => teamCode.Contains(t.slm_TeamTelesales_Code)).ToList();
                var staff = slmdb.kkslm_ms_staff.Where(s => empCode.Contains(s.slm_EmpCode)).ToList();
                var prelead = slmdb.kkslm_tr_prelead.Where(pl => preleadId.Contains(pl.slm_Prelead_Id)).ToList();

                //var records = slmdb.kkslm_tr_prelead.Where(p => preleadId.Contains(p.slm_Prelead_Id)).ToList();
                //var preleadTemp = slmdb.kkslm_tr_prelead_temp.Where(p => preleadTempId.Contains(p.slm_TempId)).ToList();
                //var products = prelead.Where(p => p.slm_Product_Id != null).Select(p => (decimal)p.slm_Cmt_Product_Id).Distinct().ToList();
                /* pom said เอาออก 2016-08-02 by OuMz
                var oldStatuses = (from pl in slmdb.kkslm_tr_prelead
                                   from al in slmdb.kkslm_tr_prelead_assign_log
                                   where pl.slm_Cmt_Product_Id == al.slm_Cmt_Product_Id && pl.slm_CmtLot == al.slm_CmtLot
                                        && preleadId.Contains(pl.slm_Prelead_Id)
                                   select new { slm_NewAssignType = al.slm_NewAssignType, preleadId = pl.slm_Prelead_Id, cmtLot = pl.slm_CmtLot, createDate = al.slm_CreatedDate, slm_Cmt_Product_Id = al.slm_Cmt_Product_Id }
                                  ).ToList();
                */

                if (RaiseLog != null) RaiseLog("Start Loop");

                int i = 0;
                DateTime start = DateTime.Now;
                foreach (var item in prelead)
                {
                    var newValues = jobs.FirstOrDefault(j => j.slm_Prelead_Id == item.slm_Prelead_Id);
                    item.slm_OldOwner = item.slm_Owner;

                    var team_temp = team.FirstOrDefault(t => t.slm_TeamTelesales_Code == newValues.slm_TeamTelesales_Code);
                    if (team_temp != null)
                        item.slm_Owner_Team = Convert.ToInt32(team_temp.slm_TeamTelesales_Id);

                    var staff_tmp = staff.FirstOrDefault(s => s.slm_EmpCode == newValues.slm_EmpCode);
                    if (staff_tmp != null)
                    {
                        item.slm_Owner = staff_tmp.slm_EmpCode;
                        item.slm_Owner_Position = staff_tmp.slm_Position_id;
                        item.slm_OwnerBranch = staff_tmp.slm_BranchCode;
                    }

                    item.slm_UpdatedBy = username;
                    item.slm_UpdatedDate = DateTime.Now;
                    item.slm_AssignFlag = "0";
                    item.slm_Assign_Status = "4";

                    /*var tmp = slmdb.kkslm_tr_prelead_assign_log
                                               .Where(l => l.slm_CmtLot == item.slm_CmtLot && l.slm_Cmt_Product_Id == item.slm_Cmt_Product_Id)
                                               .OrderByDescending(l => l.slm_CreatedDate)
                                               .FirstOrDefault();
                    string oldStatus = tmp == null ? "" : tmp.slm_NewAssignType;*/
                    /* 2016-08-02
                    var tmp = oldStatuses.Where(l => l.cmtLot == item.slm_CmtLot && l.slm_Cmt_Product_Id == item.slm_Cmt_Product_Id)
                                        .OrderByDescending(l => l.createDate)
                                        .FirstOrDefault();
                    string oldStatus = tmp == null ? "" : tmp.slm_NewAssignType;
                    */
                    kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                    {
                        slm_Cmt_Product_Id = item.slm_Cmt_Product_Id,
                        slm_CmtLot = item.slm_CmtLot,
                        slm_OldAssignType = null, //oldStatus,
                        slm_NewAssignType = "UA",
                        slm_CreatedBy = username,
                        slm_CreatedDate = DateTime.Now
                    };
                    slmdb.kkslm_tr_prelead_assign_log.AddObject(log);

                    i++;
                    if ((DateTime.Now - start).TotalSeconds > 10)
                    {
                        if (RaiseLog != null)
                            RaiseLog(String.Format("Total processed {0} rows", i));
                    }
                } // end foreach (var item in jobs)

                if (RaiseLog != null) RaiseLog("Start saving to DB");
                slmdb.SaveChanges();
                if (RaiseLog != null) RaiseLog("End saving to DB");
            }
        }

        private List<SlmScr032SearchResult> getSelectResult(string commandText, out DataSet resultDs)
        {
            resultDs = getQueryResult(commandText);

            if (resultDs.Tables.Count == 0 || resultDs.Tables[0].Rows.Count == 0)
            {
                return new List<SlmScr032SearchResult>();
            }

            return parseObject(resultDs.Tables[0]); ;
        }
        private DataSet getQueryResult(string commandText)
        {
            DataSet resultDs = new DataSet();
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(commandText, ((System.Data.EntityClient.EntityConnection)slmdb.Connection).StoreConnection.ConnectionString))
                {
                    adapter.Fill(resultDs);
                }
            }
            return resultDs;
        }
        private List<SlmScr032SearchResult> parseObject(DataTable table)
        {
            List<SlmScr032SearchResult> result = new List<SlmScr032SearchResult>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                SlmScr032SearchResult item = new SlmScr032SearchResult();
                item.GetType().GetProperties().ToList().ForEach(p =>
                {
                    if (row.Table.Columns.Contains(p.Name) && row[p.Name] != DBNull.Value)
                    {
                        p.SetValue(item, row[p.Name], null);
                    }
                });
                result.Add(item);
            }
            return result;
        }

        public bool CheckAccess(decimal lot)
        {
            bool ret = false;
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var port = slmdb.kkslm_tr_preleadportmonitor.Where(p => p.slm_CMTLot == lot && p.is_Deleted == false).FirstOrDefault();
                if (port != null)
                {
                    if (port.slm_PortStatus == "L")
                    {
                        ret = true;
                    }
                    else
                    {
                        _error = "ข้อมูล Lot " + lot.ToString() + " ยังไม่พร้อมใช้งาน";
                        ret = false;
                    }
                }
                else
                {
                    _error = "ไม่พบข้อมูล Lot " + lot.ToString();
                    ret = false;
                }

                return ret;
            }
        }

        public bool CreateExcel(List<SlmScr032SearchResult> list, string filename, string type, string sheetName)
        {
            bool ret = true;
            try
            {
                List<object[]> data = new List<object[]>();

                list.ForEach(p => {
                    data.Add(new object[] {
                            p.slm_TeamTelesales_Code, p.slm_EmpCode, p.slm_MonthNameTh, p.slm_Contract_Number, p.CustomerName
                            , p.slm_InsNameTh, p.slm_ConverageTypeName, p.slm_Voluntary_Gross_Premium, p.slm_CreatedDate, p.slm_Grade
                            , (type == "success" ? p.slm_Prelead_Id : p.slm_TempId), p.slm_CmtLot, p.slm_AssignDescription
                            , p.slm_Car_By_Gov_Name_Org, p.slm_Brand_Name_Org, p.slm_Model_Name_Org, p.slm_Voluntary_Mkt_Id
                            , p.slm_Voluntary_Mkt_First_Name ,p.slm_Voluntary_Mkt_Last_Name, p.slm_BranchCode, p.slm_CitizenId
                            , p.slm_AssignDescription, p.slm_FlagNotifyPremium, p.slm_PolicyGrossPremium, p.slm_LatestInsExpireDate
                            , p.slm_Chassis_No, p.slm_Engine_No, p.slm_Car_License_No, p.slm_ProvinceRegis_Org
                            , p.slm_Voluntary_Policy_Number, p.slm_Voluntary_Policy_Exp_Date
                        });
                });

                ExcelExportBiz ebz = new ExcelExportBiz();
                string columnIdName = "";
                if (type == "success")
                    columnIdName = "Prelead Id";
                else
                    columnIdName = "Temp Id";

                if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = sheetName,
                               RowPerSheet = 0,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ชื่อทีม", ColumnDataType= ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="รหัสพนักงาน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เดือนที่สิ้นสุดการคุ้มครอง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Contract", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ชื่อ-สกุล ลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "บริษัทประกันภัย", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "ประเภทประกันภัย", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "ค่าเบี้ยประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "Transfer Date", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "Grade ลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= columnIdName, ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "Lot", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "หมายเหตุการจ่ายงาน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "ประเภทการใช้งานของรถ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "ยื่ห้อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "รุ่น", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "รหัสการตลาด MKT", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "ชื่อ MKT", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "นามสกุล MKT", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "รหัสสาขา ของสาขาที่สร้างสัญญา", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "รหัสบัตรประชาชนของลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "Remark", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "Flag ค่าเบี้ยปีต่อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "ราคาค่าเบี้ยใบเตือนที่mapping ได้ ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "วันที่หมดอายุกรมธรรม์ที่ mapping ได้", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "เลขตัวถัง", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "เลขตัวเครื่อง", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "ทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "จังหวัดที่จดทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "เลขกรมธรรม์(ปีเดิม) ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= "วันหมดอายุประกัน(ปีล่าสุด)", ColumnDataType = ExcelExportBiz.ColumnType.Text }
                               }
                           }
                        }))
                    throw new Exception(ebz.ErrorMessage);
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }

            return ret;
        }


    }

    public class SlmScr032SearchResult
    {
        public Int64 No { get; set; }
        public decimal slm_Prelead_Id { get; set; }
        public string slm_TeamTelesales_Code { get; set; }
        public string slm_StaffNameTH { get; set; }
        public string slm_EmpCode { get; set; }
        public string slm_Contract_Number { get; set; }
        public string slm_Name { get; set; }
        public string slm_LastName { get; set; }
        public string slm_InsNameTh { get; set; }
        public string slm_ConverageTypeName { get; set; }
        public decimal slm_Voluntary_Gross_Premium { get; set; }
        public DateTime slm_CreatedDate { get; set; }
        public string slm_Grade { get; set; }
        public string slm_Assign_Type_Name { get; set; }
        public string slm_AssignDescription { get; set; }
        public decimal slm_Cmt_Product_Id { get; set; }
        public decimal slm_CmtLot { get; set; }
        public string slm_MonthNameTh { get; set; }
        public string CustomerName { get; set; }
        public decimal slm_TempId { get; set; }
        //public string slm_NewAssignType { get; set; }
        public string slm_Car_By_Gov_Name_Org { get; set; }
        public string slm_Model_Name_Org { get; set; }
        public string slm_Brand_Name_Org { get; set; }
        public string slm_Voluntary_Mkt_Id { get; set; }
        public string slm_Voluntary_Mkt_First_Name { get; set; }
        public string slm_Voluntary_Mkt_Last_Name { get; set; }
        public string slm_BranchCode { get; set; }
        public string slm_CitizenId { get; set; }
        public string slm_FlagNotifyPremium { get; set; }
        public decimal? slm_PolicyGrossPremium { get; set; }
        public DateTime? slm_LatestInsExpireDate { get; set; }
        public string slm_Chassis_No { get; set; }
        public string slm_Engine_No { get; set; }
        public string slm_Car_License_No { get; set; }
        public string slm_ProvinceRegis_Org { get; set; }
        public string slm_Voluntary_Policy_Number { get; set; }
        public DateTime slm_Voluntary_Policy_Exp_Date { get; set; }

        // error for import
        public int RowNo { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SlmScr032ExcelData
    {
        public string TeamTelesales { get; set; }
        public string EmpCode { get; set; }
        public string PreleadId { get; set; }
        public string Lot { get; set; }
    }
}
