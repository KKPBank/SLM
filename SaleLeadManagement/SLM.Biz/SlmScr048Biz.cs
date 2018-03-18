using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource;


namespace SLM.Biz
{
   public  class SlmScr048Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        public List<SLM048Data> GetDataForScreen(DateTime incFrom, DateTime incTo, DateTime covFrom, DateTime covTo, DateTime leadFrom, DateTime leadTo, string team, string staff, string level)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                string sqlz = @"
select main.* 
,main.suminsurepremium / policy.PolicyCount as AvgInsurePremium
,main.slm_PolicyDiscountAmt / policy.PolicyCount as AvgDiscount
,(policy.PolicyCount * 100 ) /  port.totalPort as PercentSuccess
,(main.PartnerSuccess / main.SumInsureActPremium) * 100 as PercentPartnerSuccses
from
(
select lead.slm_Owner
--,min(pre.slm_owner) as preowner
, min(ren.slm_IncentiveDate) as slm_IncentiveDate
, min(ren.slm_PolicyStartCoverDate) as slm_PolicyStartCoverDate
, min(ren.slm_PolicyEndCoverDate) as slm_PolicyEndCoverDate
, min(pre.slm_AssignDate) as slm_AssignDate
, st.slm_EmpCode
, tt.slm_TeamTelesales_Name
, st.slm_StaffNameTH
, lv.slm_LevelName
, sum(amount.SumInsurePremium) as SumInsurePremium
, sum(amount.SumActPremium) as SumActPremium
, sum(amount.SumInsurePremium + amount.SumActPremium) as SumInsureActPremium
, sum(ren.slm_PolicyDiscountAmt) as slm_PolicyDiscountAmt
, sum(case when ins.slm_instype = '01' then ren.slm_policygrosspremiumTotal + ren.slm_ActNetPremium else 0 end) as PartnerSuccess
from kkslm_tr_lead lead
left join kkslm_tr_prelead pre on lead.slm_ticketID = pre.slm_ticketId
inner join kkslm_tr_renewinsurance ren on lead.slm_ticketid = ren.slm_ticketID
left JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com INS ON REN.slm_InsuranceComId = INS.slm_Ins_Com_Id
inner join kkslm_ms_staff st on st.slm_UserName = lead.slm_owner
left join kkslm_ms_teamtelesales tt on st.slm_TeamTelesales_Id = tt.slm_TeamTelesales_Id
left join kkslm_ms_level lv on lv.slm_LevelId = st.slm_Level
left join  (
				SELECT rr.slm_ticketId
				, isnull(SUM(case when rrd.slm_PaymentCode = '204' then RRD.slm_RecAmount else 0 end),0) as SumInsurePremium
				, isnull(SUM(case when rrd.slm_PaymentCode = '205' then RRD.slm_RecAmount else 0 end),0) as SumActPremium
				FROM kkslm_tr_renewinsurance_receipt as rr
					 INNER JOIN kkslm_tr_renewinsurance_receipt_detail rrd on rr.slm_RenewInsuranceReceiptId = rrd.slm_RenewInsuranceReceiptId 
				WHERE RR.slm_Status IS NULL AND RRD.slm_PaymentCode in ( '204', '205')
				group by rr.slm_ticketId
) amount on lead.slm_ticketId = amount.slm_ticketId
where lead.is_Deleted = 0 {0}
group by lead.slm_owner,tt.slm_TeamTelesales_Name, st.slm_StaffNameTH, lv.slm_LevelName , st.slm_EmpCode
) main
left join 
(
    SELECT lead.slm_owner, COUNT(LEAD.slm_ticketId) AS PolicyCount
    FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId
    WHERE LEAD.slm_Status NOT IN ('08','09','10') AND REN.slm_IncentiveFlag = 1 AND LEAD.is_Deleted = 0 
    group by lead.slm_Owner
) policy on main.slm_Owner = policy.slm_Owner
left join (
    SELECT slm_owner, COUNT(*) as TotalPort
    FROM kkslm_tr_prelead 
    WHERE slm_CmtLot = (SELECT MAX(slm_CmtLot) FROM kkslm_tr_prelead WHERE is_Deleted = 0)
    group by slm_owner
) port on main.slm_EmpCode = port.slm_owner
left join (
    SELECT slm_owner, COUNT(*) as TotalPob
    FROM kkslm_tr_prelead 
    WHERE slm_Status = '16' and slm_SubStatus not in ('01','10','11','12','13','14') and is_Deleted = 0
    group by slm_owner
) pob on main.slm_EmpCode = pob.TotalPob
order by main.SumInsureActPremium desc

";

                string whr = "";
                if (incFrom.Year != 1) whr += string.Format(" AND ren.slm_IncentiveDate >= '{0}'" , SLMUtil.GetDateString(incFrom));
                if (incTo.Year != 1) whr += string.Format(" AND ren.slm_IncentiveDate < '{0}'", SLMUtil.GetDateString(incTo));
                if (covFrom.Year != 1) whr += string.Format(" AND ren.slm_PolicyStartCoverDate = '{0}'", SLMUtil.GetDateString(covFrom));
                if (covTo.Year != 1) whr += string.Format(" AND ren.slm_PolicyEndCoverDate = '{0}'", SLMUtil.GetDateString(covTo));
                if (leadFrom.Year != 1) whr += string.Format(" AND pre.slm_AssignDate >= '{0}'", SLMUtil.GetDateString(leadFrom));
                if (leadTo.Year != 1) whr += string.Format(" AND pre.slm_AssignDate < '{0}'", SLMUtil.GetDateString(leadTo.AddDays(1)));
                if (!string.IsNullOrEmpty(team)) whr += string.Format(" AND st.slm_TeamTelesales_Id = '{0}' ", team);
                if (!string.IsNullOrEmpty(staff)) whr += string.Format(" AND st.slm_EmpCode = '{0}' ", staff);
                if (!string.IsNullOrEmpty(level)) whr += string.Format(" AND st.slm_Level = '{0}' ", level);


                sqlz = String.Format(sqlz, whr);
                var obj = sdc.ExecuteStoreQuery<SLM048Data>(sqlz).ToList();

                return obj;

            }
        }

        public bool CreateExcel(string filename, DateTime incFrom, DateTime incTo, DateTime covFrom, DateTime covTo, DateTime leadFrom, DateTime leadTo, string team, string staff, string level)
        {
            bool ret = true;
            try
            {
                string sqlz = @"
select main.* 
,main.suminsurepremium / policy.PolicyCount as AvgInsurePremium
,main.slm_PolicyDiscountAmt / policy.PolicyCount as AvgDiscount
,(policy.PolicyCount * 100 ) /  port.totalPort as PercentSuccess
,(main.PartnerTotal / main.SumInsureActPremium) * 100 as PercentPartnerSuccses
,port.TotalPort
,pob.TotalPob, pob.TotalNoPob, upgrade.UpgradeCount, upgrade.UpgradeAmt
, upgrade.DowngradeCount, upgrade.DowngradeAmt
, waitmoney.LeadPaidWaitMoney
from
(
select lead.slm_Owner
--,min(pre.slm_owner) as preowner
, min(ren.slm_IncentiveDate) as slm_IncentiveDate
, min(ren.slm_PolicyStartCoverDate) as slm_PolicyStartCoverDate
, min(ren.slm_PolicyEndCoverDate) as slm_PolicyEndCoverDate
, min(pre.slm_AssignDate) as slm_AssignDate
, st.slm_EmpCode
, tt.slm_TeamTelesales_Name
, st.slm_StaffNameTH
, lv.slm_LevelName
, sum(amount.SumInsurePremium) as SumInsurePremium
, sum(amount.SumActPremium) as SumActPremium
, sum(amount.SumInsurePremium + amount.SumActPremium) as SumInsureActPremium
, sum(ren.slm_PolicyDiscountAmt) as slm_PolicyDiscountAmt
, sum(case when ins.slm_instype = '01' then ren.slm_policygrosspremiumTotal + ren.slm_ActNetPremium else 0 end) as PartnerTotal
, sum(case when ren.slm_ContractNo is null then 1 else 0 end) as NewCustomer
, sum(ra.RACount) as RACount
, sum(case when ren.slm_ReceiveNo is not null then ren.slm_PolicyGrossPremium else 0 end) as TotalPremium
, sum(case when ren.slm_ReceiveNo is not null and ren.slm_PolicyPayMethodId = 2 then ren.slm_PolicyGrossPremium else 0 end) as TotalPremiumWithTax
from kkslm_tr_lead lead
inner join kkslm_tr_renewinsurance ren on lead.slm_ticketid = ren.slm_ticketID
left join kkslm_tr_prelead pre on lead.slm_ticketID = pre.slm_ticketId
left JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com INS ON REN.slm_InsuranceComId = INS.slm_Ins_Com_Id
inner join kkslm_ms_staff st on st.slm_UserName = lead.slm_owner
left join kkslm_ms_teamtelesales tt on st.slm_TeamTelesales_Id = tt.slm_TeamTelesales_Id
left join kkslm_ms_level lv on lv.slm_LevelId = st.slm_Level
left join (select slm_renewinsureid, count(*) as RACount from kkslm_tr_renewinsurance_rapremium group by slm_renewinsureid) ra on ren.slm_renewinsureid = ra.slm_renewinsureid
left join  (
				SELECT rr.slm_ticketId
				, isnull(SUM(case when rrd.slm_PaymentCode = '204' then RRD.slm_RecAmount else 0 end),0) as SumInsurePremium
				, isnull(SUM(case when rrd.slm_PaymentCode = '205' then RRD.slm_RecAmount else 0 end),0) as SumActPremium
				FROM kkslm_tr_renewinsurance_receipt as rr
					 INNER JOIN kkslm_tr_renewinsurance_receipt_detail rrd on rr.slm_RenewInsuranceReceiptId = rrd.slm_RenewInsuranceReceiptId 
				WHERE RR.slm_Status IS NULL AND RRD.slm_PaymentCode in ( '204', '205')
				group by rr.slm_ticketId
) amount on lead.slm_ticketId = amount.slm_ticketId

where lead.is_Deleted = 0 {0}
group by lead.slm_owner,tt.slm_TeamTelesales_Name, st.slm_StaffNameTH, lv.slm_LevelName , st.slm_EmpCode
) main
left join 
(
    SELECT lead.slm_owner, COUNT(LEAD.slm_ticketId) AS PolicyCount
    FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId
    WHERE LEAD.slm_Status NOT IN ('08','09','10') AND REN.slm_IncentiveFlag = 1 AND LEAD.is_Deleted = 0 
    group by lead.slm_Owner
) policy on main.slm_Owner = policy.slm_Owner
left join (
    SELECT slm_owner, COUNT(*) as TotalPort
    FROM kkslm_tr_prelead 
    WHERE slm_CmtLot = (SELECT MAX(slm_CmtLot) FROM kkslm_tr_prelead WHERE is_Deleted = 0)
    group by slm_owner
) port on main.slm_EmpCode = port.slm_owner
left join (
    SELECT slm_owner
	,sum(case when slm_SubStatus not in ('01','10','11','12','13','14') then 1 else 0 end) as TotalPob
	,sum(case when slm_SubStatus in ('01','10','11','12','13','14') then 1 else 0 end) as TotalNoPob
    FROM kkslm_tr_prelead 
    WHERE slm_Status = '16' and is_Deleted = 0
    group by slm_owner
) pob on main.slm_EmpCode = pob.slm_owner
left join (
	select l.slm_Owner
	, sum(case when rc.slm_CostSave > 0 then 1 else 0 end) as UpgradeCount
	, sum(Case when rc.slm_CostSave > 0 then slm_CostSave else 0 end) as UpgradeAmt
	, sum(case when rc.slm_CostSave < 0 then 1 else 0 end) as DowngradeCount
	, sum(Case when rc.slm_CostSave < 0 then slm_CostSave else 0 end) as DowngradeAmt
	FROM kkslm_tr_prelead p
	inner join kkslm_tr_lead l on l.slm_ticketId = p.slm_TicketId
	inner join kkslm_tr_renewinsurance r on r.slm_TicketId = l.slm_ticketId
	inner join kkslm_tr_renewinsurance_Compare rc on rc.slm_RenewInsureId = r.slm_RenewInsureId 
	where l.is_deleted = 0 and rc.slm_selected = 1
	group by l.slm_Owner

) upgrade on upgrade.slm_Owner = main.slm_Owner
left join (
SELECT lead.slm_Owner, COUNT(LEAD.slm_ticketId) AS LeadPaidWaitMoney 
FROM KKSLM_TR_LEAD LEAD 
INNER JOIN kkslm_tr_renewinsurance REN ON REN.slm_TicketId = LEAD.slm_TicketId 
INNER JOIN kkslm_tr_renewinsurance_receipt as rr ON RR.slm_ticketId = LEAD.slm_ticketId 
INNER JOIN (SELECT  SUM(slm_RecAmount) recamount, slm_RenewInsuranceReceiptId 
	FROM kkslm_tr_renewinsurance_receipt_detail
	where slm_PaymentCode = '204'
	group by  slm_RenewInsuranceReceiptId  ) AS rrd on rr.slm_RenewInsuranceReceiptId = rrd.slm_RenewInsuranceReceiptId 
WHERE RR.slm_Status = '01' AND rrd.recamount <> 0 and slm_PayOptionId = 2 AND LEAD.slm_SubStatus = '18' AND LEAD.is_Deleted = 0
group by lead.slm_Owner
) waitmoney on waitmoney.slm_Owner = main.slm_Owner
order by main.SumInsureActPremium desc 
";

                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    string whr = "";
                    if (incFrom.Year != 1) whr += string.Format(" AND ren.slm_IncentiveDate >= '{0}'", SLMUtil.GetDateString(incFrom));
                    if (incTo.Year != 1) whr += string.Format(" AND ren.slm_IncentiveDate < '{0}'", SLMUtil.GetDateString(incTo));
                    if (covFrom.Year != 1) whr += string.Format(" AND ren.slm_PolicyStartCoverDate = '{0}'", SLMUtil.GetDateString(covFrom));
                    if (covTo.Year != 1) whr += string.Format(" AND ren.slm_PolicyEndCoverDate = '{0}'", SLMUtil.GetDateString(covTo));
                    if (leadFrom.Year != 1) whr += string.Format(" AND pre.slm_AssignDate >= '{0}'", SLMUtil.GetDateString(leadFrom));
                    if (leadTo.Year != 1) whr += string.Format(" AND pre.slm_AssignDate < '{0}'", SLMUtil.GetDateString(leadTo.AddDays(1)));
                    if (!string.IsNullOrEmpty(team)) whr += string.Format(" AND st.slm_TeamTelesales_Id = '{0}' ", team);
                    if (!string.IsNullOrEmpty(staff)) whr += string.Format(" AND st.slm_EmpCode = '{0}' ", staff);
                    if (!string.IsNullOrEmpty(level)) whr += string.Format(" AND st.slm_Level = '{0}' ", level);


                    sqlz = String.Format(sqlz, whr);
                    var lst = sdc.ExecuteStoreQuery<SLM048Data>(sqlz).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    int i = 1;
                    foreach (var itm in lst)
                    {
                        data.Add(new object[]
                        {
                            i,
                            itm.slm_TeamTelesales_Name,
                            itm.slm_StaffNameTH,
                            itm.slm_LevelName,
                            itm.PolicyCount,
                            itm.TotalPob,
                            itm.TotalNoPob,
                            itm.SumInsurePremium,
                            itm.SumActPremium,
                            itm.SumInsureActPremium,   //10
                            itm.AvgInsurePremium,
                            itm.AvgDiscount,
                            null,
                            itm.TotalPort,
                            itm.PercentSuccess,
                            itm.UpgradeCount,
                            itm.UpgradeAmt,
                            itm.DowngradeCount,
                            itm.NewCustomer,
                            itm.RACount,            //20
                            itm.PartnerTotal,
                            itm.PartnerSuccess,
                            itm.TotalPremium - itm.SumInsureActPremium,
                            itm.TotalPremium,
                            itm.TotalPremiumWithTax,
                            itm.LeadPaidWaitMoney,
                            null,
                            null,
                            null,
                            null   //30

                        });
                        i++;
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                            new ExcelExportBiz.SheetItem()
                            {
                                SheetName = "รายงานประจำวัน",
                                RowPerSheet = 100,
                                Data = data,
                                Columns = new List<ExcelExportBiz.ColumnItem>()
                                {
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ลำดับที่", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Team", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Telesales", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Level", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวนกรมธรรม์", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="พบ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ไม่พบ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ย พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประกัน+พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //10
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Avg ค่าเบี้ยประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Avg %ส่วนลด", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="RunRate", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Port (ที่แจก Lead)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="%Success (ของเดือนที่ทำงาน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Upgrade (Pol)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Upgrade (Amt)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Downgrade (Pol)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ลูกค้าใหม่", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="RA", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //20
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Partner Premium", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="%success of Partner", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยประกันภัยค้างชำระ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยประกันภัยที่รับชำระ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยประกันภัยรวมภาษีอากร", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวน Lead ชำระแล้วรอเงินเข้าระบบ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวน Lead ผ่อนล่วงหน้าก่อนหมดอายุ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวน Lead ไม่เข้าเงื่อนไข NCB", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Working Day", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Now", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //30
                                }
                            }
                    }))
                        throw new Exception(ebz.ErrorMessage);
                }

            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ret = false;
            }
            return ret;
        }

        public List<SP_RPT_001_GENERAL_Result> GetDataForScreen2(DateTime incFrom, DateTime incTo, string team, string staff, string level)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return GetReportData(sdc, incFrom, incTo, team, staff, level);
            }
        }
        private List<SP_RPT_001_GENERAL_Result> GetReportData(SLM_DBEntities sdc, DateTime incFrom, DateTime incTo, string team, string staff, string level)
        {
            var sp = sdc.SP_RPT_001_GENERAL(incFrom, incTo, null, null, null, null).ToList();

            if (!string.IsNullOrEmpty(team) && SLMUtil.SafeInt(team) != 0) sp = sp.Where(t => t.slm_TeamTelesales_Id == SLMUtil.SafeInt(team)).ToList();
            if (!string.IsNullOrEmpty(staff)) sp = sp.Where(t => t.slm_EmpCode == staff).ToList();
            if (!string.IsNullOrEmpty(level) && SLMUtil.SafeInt(level) != 0) sp = sp.Where(t => t.slm_Level == SLMUtil.SafeInt(level)).ToList();

            return sp;
        }

        public bool CreateExcel2(string filename, DateTime incFrom, DateTime incTo, DateTime covFrom, DateTime covTo, DateTime leadFrom, DateTime leadTo, string team, string staff, string level)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    //var sp = sdc.SP_RPT_001_GENERAL(incFrom, incTo, covFrom.Year == 1 ? null : (DateTime?)covFrom, covTo.Year == 1 ? null : (DateTime?)covTo, leadFrom.Year == 1 ? null : (DateTime?)leadFrom, leadTo.Year == 1 ? null : (DateTime?)leadTo).ToList();

                    //if (!string.IsNullOrEmpty(team) && SLMUtil.SafeInt(team) != 0) sp = sp.Where(t => t.slm_TeamTelesales_Id == SLMUtil.SafeInt(team)).ToList();
                    //if (!string.IsNullOrEmpty(staff)) sp = sp.Where(t => t.slm_EmpCode == staff).ToList();
                    //if (!string.IsNullOrEmpty(level) && SLMUtil.SafeInt(level) != 0) sp = sp.Where(t => t.slm_Level == SLMUtil.SafeInt(level)).ToList();
                    var sp = GetReportData(sdc, incFrom, incTo, team, staff, level);

                    if (sp.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    for (int i = 0; i < sp.Count; i++)
                    {
                        var itm = sp[i];
                        data.Add(new object[]
                        {
                            i+1,
                            itm.Team,
                            itm.StaffName,
                            itm.Level,
                            itm.NumberOfPolicyNotThisMonth,
                            itm.TotalLead,
                            itm.NumberOfActNotThisMonth,
                            itm.NumberOfActThisMonth,
                            itm.Pob,
                            itm.NoPob, //10
                            Math.Round(itm.TotalPremium, 2, MidpointRounding.AwayFromZero),
                            Math.Round(itm.TotalAct, 2, MidpointRounding.AwayFromZero),
                            itm.PremiumAndAct != null ? Math.Round(itm.PremiumAndAct.Value, 2) : (decimal)0,
                            itm.AvgPremium != null ? Math.Round( itm.AvgPremium.Value, 2, MidpointRounding.AwayFromZero) : 0,
                            itm.RunRate != null ? Math.Round(itm.RunRate.Value, 2, MidpointRounding.AwayFromZero) : (decimal)0,
                            itm.Port,
                            itm.PortB2C,
                            itm.PercentSuccess != null ? Math.Round(itm.PercentSuccess.Value, 2) : (decimal)0,
                            itm.NewCustomer,
                            itm.RA, //20
                            Math.Round(itm.PremiumInArrears, 2, MidpointRounding.AwayFromZero),
                            Math.Round(itm.PremiumPaid, 2, MidpointRounding.AwayFromZero),
                            Math.Round(itm.PolicyGrossPremium, 2, MidpointRounding.AwayFromZero),
                            itm.NumberOfLeadPaid,
                            itm.NumberOfLeadPayByInstalment,
                            itm.NumberOfLeadNotInCBC,
                            itm.WokingDay,
                            itm.DayNow

                        });

                    }

                    var sumlead = Convert.ToDecimal(data.Sum(d => (int)d[4]) + data.Sum(d => (int)d[5]));
                    var sumttpremium = data.Sum(d => (decimal)d[10]);
                    var sumport = Convert.ToDecimal(data.Sum(d => (int)d[15]) + data.Sum(d => (int)d[16]));

                    data.Add(new object[]
                    {
                        null,
                        null,
                        "Grand Total",
                        null,
                        sp.Sum(d=>d.NumberOfPolicyNotThisMonth),
                        sp.Sum(d=>d.TotalLead),
                        sp.Sum(d=>d.NumberOfActNotThisMonth),
                        sp.Sum(d=>d.NumberOfActThisMonth),
                        sp.Sum(d=>d.Pob),
                        sp.Sum(d=>d.NoPob),
                        data.Sum(d=> (decimal)d[10]),
                        data.Sum(d=> (decimal)d[11]),
                        data.Sum(d=> (decimal)d[12]),
                        //Math.Round(sp.Sum(d=>d.TotalLead + d.NumberOfActNotThisMonth) == 0 ? 0 : data.Sum(d=>(decimal)d[10]) / sp.Sum(d=>d.TotalLead  + d.NumberOfActNotThisMonth), 2), // Math.Round(data.Average(d=> (decimal?)d[13]) ?? 0,2),
                        Math.Round(sumlead == 0 ? 0 : sumttpremium / sumlead , 2, MidpointRounding.AwayFromZero),
                        data.Sum(d=> (decimal)d[14]),
                        sp.Sum(d=>d.Port),
                        sp.Sum(d=>d.PortB2C),
                        //Math.Round( sp.Sum(d=>d.Port + d.PortB2C) == 0 ? 0 : ((decimal)sp.Sum(d=>d.TotalLead + d.NumberOfActNotThisMonth) * 100)/sp.Sum(d=>d.Port + d.PortB2C), 2) , // data.Average(d=>(decimal?)d[17]),// 0, //sp.Sum(d=>d.Port) != 0 ? sp.Sum(d=>d.TotalLead) * 100 / sp.Sum(d=>d.Port) : 0,
                        Math.Round(sumport == 0 ? 0 : (sumlead * 100) / sumport, 2, MidpointRounding.AwayFromZero),
                        sp.Sum(d=>d.NewCustomer),
                        sp.Sum(d=>d.RA),
                        data.Sum(d => (decimal)d[20]),
                        data.Sum(d => (decimal)d[21]),
                        data.Sum(d => (decimal)d[22]),
                        sp.Sum(d => d.NumberOfLeadPaid),
                        sp.Sum(d => d.NumberOfLeadPayByInstalment),
                        sp.Sum(d => d.NumberOfLeadNotInCBC),
                        null,
                        null

                    });

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                    {
                        new ExcelExportBiz.SheetItem()
                        {
                            SheetName = "รายงานประจำวัน",
                            RowPerSheet = SLMConstant.ExcelRowPerSheet,
                            Data = data,
                            Columns = new List<ExcelExportBiz.ColumnItem>()
                            {
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ลำดับที่", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Team", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Telesales", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Level", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวนกรมธรรม์ (เดือนที่ไม่ใช่ ณ ปัจจุบัน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวนกรมธรรม์ (เดือนปัจจุบัน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวน พรบ.(เดือนที่ไม่ใช่ ณ ปัจจุบัน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวน พรบ.(เดือนปัจจุบัน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Call (พบ)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Call (ไม่พบ)", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //10
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยหลังหักส่วนลด", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ย พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประกัน+พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Avg ค่าเบี้ยประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="RunRate", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Port (ที่แจก Lead) (A,B1)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Port (ที่แจก Lead) (B2,C)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="%Success (ของเดือนที่ทำงาน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ลูกค้านอกใหม่", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="RA", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //20
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยประกันภัยค้างชำระ (บาท) ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยประกันภัยที่รับชำระ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยประกันภัยหลังหักส่วนลด", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวน Lead ชำระแล้วรอเงินเข้าระบบ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวน Lead ผ่อนล่วงหน้าก่อนหมดประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวน Lead ไม่เข้าเงื่อนไข CBC", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Working Day", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Now", ColumnDataType= ExcelExportBiz.ColumnType.Number }, 
                            }
                        }
                    }))
                        throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }

        public partial class SLM048Data
        {
            public string slm_Owner { get; set; }
            public DateTime? slm_IncentiveDate { get; set; }
            public DateTime? slm_PolicyStartCoverDate { get; set; }
            public DateTime? slm_PolicyEndCoverDate { get; set; }
            public DateTime? slm_AssignDate { get; set; }
            public string slm_EmpCode { get; set; }
            public string slm_TeamTelesales_Name { get; set; }
            public string slm_StaffNameTH { get; set; }
            public string slm_LevelName { get; set; }
            public decimal? SumInsurePremium { get; set; }
            public decimal? SumActPremium { get; set; }
            public decimal? SumInsureActPremium { get; set; }
            public decimal? slm_PolicyDiscountAmt { get; set; }
            public decimal? PartnerSuccess { get; set; }
            public int? PolicyCount { get; set; }
            public int? TotalPort { get; set; }
            public int? TotalPob { get; set; }
            public decimal? AvgInsurePremium { get; set; }
            public decimal? AvgDiscount { get; set; }
            public int? PercentSuccess { get; set; }
            public decimal? PercentPartnerSuccses { get; set; }
            public decimal? PartnerTotal { get; set; }
            public int? NewCustomer { get; set; }
            public int? RACount { get; set; }
            public decimal? TotalPremium { get; set; }
            public decimal? TotalPremiumWithTax { get; set; }
            public int? TotalNoPob { get; set; }
            public int? UpgradeCount { get; set; }
            public decimal? UpgradeAmt { get; set; }
            public int? DowngradeCount { get; set; }
            public decimal? DowngradeAmt { get; set; }
            public int? LeadPaidWaitMoney { get; set; }
        }
    }
}
