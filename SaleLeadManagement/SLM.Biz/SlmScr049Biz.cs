using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource;

namespace SLM.Biz
{
    public class SlmScr049Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }

        public bool CreateExcel1(string filename, DateTime incFrom, DateTime incTo, DateTime callFrom, DateTime callTo, int Telesale, string staff)
        {
            bool ret = true;
            try
            {
                string sqlz = @"
select ren.slm_IncentiveDate, ren.slm_ReceiveNo, isnull(ren.slm_Grade, pre.slm_grade) as slm_Grade, ren.slm_ContractNo
    ,ren.slm_PolicyStartCoverDate, ren.slm_PolicyEndCoverDate, lead.slm_Owner, st.slm_StaffNameTh
    ,t.slm_TitleName, lead.slm_Name, lead.slm_LastName, pre.slm_BranchCode, brn.slm_BranchName, ren.slm_LicenseNo
    ,brand.slm_BrandName, ren.slm_RedbookYearGroup, cov.slm_ConverageTypeName, pre.slm_Voluntary_Channel_Key, pre.slm_Voluntary_Type_Name
    ,ren.slm_PolicyGrossPremiumTotal, ren.slm_PolicyNetGrossPremium, ren.slm_DiscountPercent, ren.slm_PolicyDiscountAmt
    ,ren.slm_PolicyGrossPremium, ren.slm_PolicyIncentiveAmount, ren.slm_RemarkPolicy, st.slm_MarketingCode
    ,tt.slm_TeamTelesales_Name, pre.slm_Voluntary_Gross_Premium, cus.slm_CitizenId 
    ,DateDiff(day, ren.slm_PolicyStartCoverDate, ren.slm_ReceiveDate) as DaybeforeCover
    , case when pre.slm_Voluntary_Company_Code = convert(varchar, ren.slm_InsuranceComId) then 'บริษัทเดิม' else 'บริษัทใหม่' end as CompanyCompare 
    ,pre.slm_Voluntary_Company_Code, ren.slm_InsuranceComId, comp.slm_InsNameTh, renc.slm_FT, st.slm_EmpCode
    ,case when renc.slm_CostSave > 0 then 'Up' else '' end as UpgradeCount, comp.slm_InsCode, tt.slm_TeamTelesales_Code
    ,case when lead.slm_ContractNoRefer is not null or pre.slm_Contract_Number is not null or ren.slm_ContractNo is not null then 'Yes' else '' end as IsNew
    ,case when comp.slm_InsType = '01' then 'Yes' else '' end IsPartner, ra.RACount

from kkslm_tr_lead lead
inner join kkslm_tr_renewinsurance ren on lead.slm_ticketId = ren.slm_TicketId
left join kkslm_tr_cusinfo cus on cus.slm_ticketId = lead.slm_ticketId
left join kkslm_tr_prelead pre on lead.slm_ticketId = pre.slm_TicketId
left join kkslm_ms_staff st on lead.slm_Owner = st.slm_Username
left join kkslm_ms_title t on lead.slm_TitleId = t.slm_TitleId
left join kkslm_ms_branch brn on pre.slm_BranchCode = brn.slm_BranchCode
left join kkslm_ms_redbook_brand brand on ren.slm_RedbookBrandCode = brand.slm_BrandCode 
left join kkslm_ms_coveragetype cov on ren.slm_CoverageTypeId = cov.slm_CoverageTypeId
left join kkslm_ms_teamtelesales tt on st.slm_teamtelesales_id = tt.slm_teamtelesales_id
left join operdb.dbo.kkslm_ms_ins_com comp on ren.slm_InsuranceComId = comp.slm_ins_com_id
left join kkslm_tr_renewinsurance_compare renc on ren.slm_renewinsureid = renc.slm_renewinsureid and renc.slm_selected = 1
left join (select slm_renewinsureid, count(*) as RACount from kkslm_tr_renewinsurance_rapremium group by slm_renewinsureid) ra on ren.slm_renewinsureid = ra.slm_renewinsureid
--left join kkslm_tr_prelead_phone_call ppc on pre.slm_prelead_Id = ppc.slm_prelead_Id
--left join kkslm_phone_call pc on lead.slm_ticketId = pc.slm_ticketId
where lead.is_Deleted = 0 and lead.slm_ticketId in (
	select distinct l.slm_ticketId
	from kkslm_tr_lead l
	inner join kkslm_tr_renewinsurance r on l.slm_ticketId = r.slm_ticketId
	left join kkslm_tr_prelead pr on l.slm_ticketId = pr.slm_ticketId
	left join kkslm_tr_prelead_phone_call ppc on pr.slm_prelead_Id = ppc.slm_prelead_Id
	left join kkslm_phone_call pc on l.slm_ticketId = pc.slm_ticketId
	{0}
)

";
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    string whr = "";
                    if (incFrom.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" r.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                    if (incTo.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" r.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                    if (callFrom.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" ( pc.slm_CreatedDate >= '{0}' OR ppc.slm_CreatedDate >= '{0}' ) ", SLMUtil.GetDateString(callFrom));
                    if (callTo.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" ( pc.slm_CreatedDate < '{0}' OR ppc.slm_CreatedDate < '{0}' ) ", SLMUtil.GetDateString(callTo.AddDays(1)));
                    if (Telesale != 0) whr += (whr == "" ? "" : " and ") + String.Format(" l.slm_Owner in ( select slm_username from kkslm_ms_staff where slm_teamTelesales_id = {0} )", Telesale);
                    if (!String.IsNullOrEmpty(staff)) whr += (whr == "" ? "" : " and ") + String.Format(" l.slm_Owner = '{0}' ", staff);

                    sqlz = String.Format(sqlz, (whr == "" ? "" : " where " + whr));
                    var lst = sdc.ExecuteStoreQuery<ExcelSheet049_1>(sqlz).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[] {
                        itm.slm_IncentiveDate == null ? "" : itm.slm_IncentiveDate.Value.ToString("MMM-yy"),
                        itm.slm_ReceiveNo,
                        itm.slm_Grade,
                        itm.slm_PolicyStartCoverDate,
                        itm.slm_PolicyEndCoverDate,
                        itm.slm_StaffNameTh,
                        itm.slm_ContractNo,
                        itm.slm_TitleName,
                        itm.slm_Name,
                        itm.slm_LastName,   // 10
                        itm.slm_BranchCode,
                        itm.slm_BranchName,
                        itm.slm_LicenseNo,
                        itm.slm_BrandName,
                        itm.slm_RedbookYearGroup,
                        itm.slm_ConverageTypeName,
                        itm.slm_InsNameTh,
                        itm.slm_Voluntary_Type_Name,
                        itm.slm_ConverageTypeName,
                        itm.slm_Voluntary_Channel_Key,
                        itm.slm_PolicyGrossPremiumTotal,
                        itm.slm_PolicyNetGrossPremium,
                        0,
                        itm.slm_DiscountPercent,
                        itm.slm_PolicyDiscountAmt,
                        0,
                        itm.slm_PolicyGrossPremium,
                        0,
                        itm.slm_PolicyIncentiveAmount,
                        itm.slm_PolicyIncentiveAmount - itm.slm_PolicyDiscountAmt, // 30
                        0,
                        itm.slm_RemarkPolicy,
                        itm.slm_MarketingCode,
                        itm.slm_TeamTelesales_Name,
                        itm.slm_Voluntary_Gross_Premium,
                        itm.slm_CitizenId,
                        itm.slm_PolicyEndCoverDate,
                        null,   // **38
                        null,   // **39
                        itm.DaybeforeCover, //40
                        itm.slm_InsNameTh,
                        itm.IsNew,
                        itm.slm_Voluntary_Gross_Premium,
                        itm.slm_PolicyEndCoverDate,
                        itm.slm_FT,
                        null,
                        null,
                        itm.UpgradeCount,
                        itm.slm_EmpCode,
                        itm.slm_InsCode,    //50
                        null,
                        itm.slm_TeamTelesales_Code,
                        itm.IsNew,
                        itm.IsPartner,
                        itm.slm_Grade,
                        itm.RACount
                        });
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                            new ExcelExportBiz.SheetItem()
                            {
                                SheetName = "สมัครใจ",
                                RowPerSheet = 100,
                                Data = data,
                                Columns = new List<ExcelExportBiz.ColumnItem>()
                                {
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ผลงานในเดือน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขรับแจ้ง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ปีที่หมดอายุ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่เริ่มคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.DateTime },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันสิ้นสุดคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.DateTime },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อเจ้าหน้าที่ MKT", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขที่สัญญาเช่าซื้อ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="คำนำหน้าชื่อ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อผู้เอาประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="นามสกุลผู้เอาประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Text }, //10
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สาขา (Code)", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สาขา", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขทะเบียน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อยี่ห้อรถ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ปีรถยนต์", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทประกันภัยรถยนต์", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รายชื่อบริษัทประกันภัย", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทความคุ้มครองเดิม", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทความคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วิธีการต่ออายุประกันภัย", ColumnDataType= ExcelExportBiz.ColumnType.Text }, // 20
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยประกันภัย (เบี้ยเต็ม)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยสุทธิ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ย พรบ (เบี้ยเต็ม)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ส่วนลด (%)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ส่วนลดค่าเบี้ย (จำนวนเงิน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ส่วนลดค่าเบี้ย พรบ (จำนวนเงิน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยหลังหักส่วนลด", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่า พรบ หลังหักส่วนลด", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวนเงินที่ KK ได้รับจริงประกันภัย", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รายได้ที่ KK ได้รับหลังหักส่วนลด (ประกันภัย)", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //30
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รายได้ที่ KK ได้รับหลังหักส่วนลด (พรบ)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="หมายเหตุ (ประกันภัย)", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Marketing Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Head Team", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยประกันปีที่แล้ว (รวมภาษีอากร)", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="บัตรประชาชนเลขที่/จดทะเบียนนิติบุคคล", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันสิ้นสุดความคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.DateTime },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันชำระเงินครั้งแรก", ColumnDataType= ExcelExportBiz.ColumnType.DateTime },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="การชำระเบี้ยประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวนวันก่อนเริ่มคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //40
                                    new ExcelExportBiz.ColumnItem() { ColumnName="บริษัทประกันเดิม", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="บริษัทเดิม/ใหม่", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยเดิม", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันสิ้นสุดกรมธรรม์", ColumnDataType= ExcelExportBiz.ColumnType.DateTime },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ทุนประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สังกัด", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="y", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Pol Upgrade", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Emp Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Code บริษัท", ColumnDataType= ExcelExportBiz.ColumnType.Text }, //50
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Team Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="New", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Partner", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เกรดลูกค้า", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="RA", ColumnDataType= ExcelExportBiz.ColumnType.Number }
                                }
                            }
                        }))
                        throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.Message;
            }
            return ret;
        }

        public bool CreateExcel2(string filename, DateTime incFrom, DateTime incTo, DateTime callFrom, DateTime callTo, int Telesale, string staff)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    using (OPERDBEntities odc = DBUtil.GetOperDbEntities())
                    {
                        string sqlz = @"
SELECT * FROM (
SELECT LEAD.slm_ticketId, REN.slm_IncentiveDate,LEAD.slm_Owner, REN.slm_ContractNo, REN.slm_CoverageTypeId,
	RC.slm_NetGrossPremium AS NetGrossPremium,RC.slm_DiscountBath,RC.slm_PolicyGrossPremiumPay AS GrossPremiumPay,
	REN.slm_PolicyIncentiveAmount - RC.slm_DiscountBath AS KK_INCOME, RC.slm_Ins_Com_Id, pr.slm_Grade
FROM kkslm_tr_lead LEAD 
INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId
INNER JOIN kkslm_tr_renewinsurance_compare RC ON REN.slm_RenewInsureId = RC.slm_RenewInsureId
LEFT JOIN kkslm_tr_prelead pr on lead.slm_ticketid = pr.slm_ticketid
WHERE RC.slm_Selected = 1-- and lead.slm_ticketId in ()
UNION ALL
SELECT LEAD.slm_ticketId,REN.slm_IncentiveDate,LEAD.slm_Owner, REN.slm_ContractNo, REN.slm_CoverageTypeId,
	RCA.slm_ActNetGrossPremium AS NetGrossPremium, RCA.slm_DiscountBath,RCA.slm_ActGrossPremiumPay AS GrossPremiumPay,
	REN.slm_ActIncentiveAmount - RCA.slm_DiscountBath AS KK_INCOME,RCA.slm_Ins_Com_Id, pr.slm_Grade 
FROM kkslm_tr_lead LEAD 
INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId
INNER JOIN kkslm_tr_renewinsurance_compare_act RCA ON REN.slm_RenewInsureId = RCA.slm_RenewInsureId
LEFT JOIN kkslm_tr_prelead pr on lead.slm_ticketid = pr.slm_ticketid
WHERE RCA.slm_ActPurchaseFlag = 1  
) Z
WHERE slm_ticketId in (
	select distinct l.slm_ticketId
	from kkslm_tr_lead l
	inner join kkslm_tr_renewinsurance r on l.slm_ticketId = r.slm_ticketId
	inner join kkslm_tr_prelead pr on l.slm_ticketId = pr.slm_ticketId
	left join kkslm_tr_prelead_phone_call ppc on pr.slm_prelead_Id = ppc.slm_prelead_Id
	left join kkslm_phone_call pc on l.slm_ticketId = pc.slm_ticketId
    {0}
)
order by slm_ticketid
";
                        string whr = "";
                        if (incFrom.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" r.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                        if (incTo.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" r.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                        if (callFrom.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" ( pc.slm_CreatedDate >= '{0}' OR ppc.slm_CreatedDate >= '{0}' ) ", SLMUtil.GetDateString(callFrom));
                        if (callTo.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" ( pc.slm_CreatedDate < '{0}' OR ppc.slm_CreatedDate < '{0}' ) ", SLMUtil.GetDateString(callTo.AddDays(1)));
                        if (Telesale != 0) whr += (whr == "" ? "" : " and ") + String.Format(" l.slm_Owner in ( select slm_username from kkslm_ms_staff where slm_teamTelesales_id = {0} )", Telesale);
                        if (!String.IsNullOrEmpty(staff)) whr += (whr == "" ? "" : " and ") + String.Format(" l.slm_Owner = '{0}' ", staff);

                        sqlz = String.Format(sqlz, (whr == "" ? "" : " where " + whr));
                        var lst = sdc.ExecuteStoreQuery<ExcelSheet049_3>(sqlz).ToList();
                        if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                        var stfl = sdc.kkslm_ms_staff.ToList();
                        var ttl = sdc.kkslm_ms_teamtelesales.ToList();
                        var coml = odc.kkslm_ms_ins_com.ToList();
                        var covl = sdc.kkslm_ms_coveragetype.ToList();

                        List<object[]> data = new List<object[]>();
                        foreach (var itm in lst)
                        {
                            var stf = stfl.Where(s => s.slm_UserName == itm.slm_Owner).FirstOrDefault();
                            var tt = (stf == null ? null : ttl.Where(t => t.slm_TeamTelesales_Id == stf.slm_TeamTelesales_Id).FirstOrDefault());
                            var com = coml.Where(c => c.slm_Ins_Com_Id == itm.slm_Ins_Com_Id).FirstOrDefault();
                            var cov = covl.Where(c => c.slm_CoverageTypeId == itm.slm_CoverageTypeId).FirstOrDefault();

                            data.Add(new object[]
                            {
                                itm.slm_IncentiveDate == null ? "" : itm.slm_IncentiveDate.Value.ToString("MMM-yy"),
                                stf == null ? "" :  stf.slm_StaffNameTH,
                                itm.slm_ContractNo,
                                cov == null ? "" : cov.slm_ConverageTypeName,
                                itm.NetGrossPremium,
                                itm.slm_DiscountBath,
                                itm.GrossPremiumPay,
                                itm.KK_INCOME,
                                tt == null ? "" : tt.slm_TeamTelesales_Name,
                                stf == null ? "" : stf.slm_EmpCode,
                                com == null ? "" : com.slm_InsCode,
                                tt == null ? "" : tt.slm_TeamTelesales_Code,
                                "",
                                String.IsNullOrEmpty(itm.slm_ContractNo) ? "" : "Yes",
                                com == null ? "" : com.slm_InsType == "01" ? "Yes" : "",
                                itm.slm_Grade
                            });
                        }


                        ExcelExportBiz ebz = new ExcelExportBiz();
                        if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "สมัครใจ_พรบ",
                               RowPerSheet = 100,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ผลงานในเดือน", ColumnDataType= ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ชื่อเจ้าหน้าที่ MKT", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เลขที่สัญญาเช่าซื้อ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ประเภทความคุ้มครอง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ค่าเบี้ย(เบี้ยเต็ม)", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ส่วนลดค่าเบี้ย (จำนวนเงิน)", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ค่าเบี้ยหลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="รายได้ที่ KK ได้รับหลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Head Team", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Emp ID", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Code บริษัท", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Team Code", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Code", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="New", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Partner", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Grade", ColumnDataType = ExcelExportBiz.ColumnType.Text  }
                               }
                           }
                        }))
                            throw new Exception(ebz.ErrorMessage);

                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }

            return ret;
        }

        private partial class ExcelSheet049_1
        {
            public DateTime? slm_IncentiveDate { get; set; }
            public string slm_ReceiveNo { get; set; }
            public string slm_Grade { get; set; }
            public string slm_ContractNo { get; set; }
            public DateTime? slm_PolicyStartCoverDate { get; set; }
            public DateTime? slm_PolicyEndCoverDate { get; set; }
            public string slm_Owner { get; set; }
            public string slm_StaffNameTh { get; set; }
            public string slm_TitleName { get; set; }
            public string slm_Name { get; set; }
            public string slm_LastName { get; set; }
            public string slm_BranchCode { get; set; }
            public string slm_BranchName { get; set; }
            public string slm_LicenseNo { get; set; }
            public string slm_BrandName { get; set; }
            public int? slm_RedbookYearGroup { get; set; }
            public string slm_ConverageTypeName { get; set; }
            public string slm_Voluntary_Channel_Key { get; set; }
            public string slm_Voluntary_Type_Name { get; set; }
            public decimal? slm_PolicyGrossPremiumTotal { get; set; }
            public decimal? slm_PolicyNetGrossPremium { get; set; }
            public decimal? slm_DiscountPercent { get; set; }
            public decimal? slm_PolicyDiscountAmt { get; set; }
            public decimal? slm_PolicyGrossPremium { get; set; }
            public decimal? slm_PolicyIncentiveAmount { get; set; }
            public string slm_RemarkPolicy { get; set; }
            public string slm_MarketingCode { get; set; }
            public string slm_TeamTelesales_Name { get; set; }
            public decimal? slm_Voluntary_Gross_Premium { get; set; }
            public string slm_CitizenId { get; set; }
            public int? DaybeforeCover { get; set; }
            public string CompanyCompare { get; set; }
            public string slm_Voluntary_Company_Code { get; set; }
            public decimal? slm_InsuranceComId { get; set; }
            public string slm_InsNameTh { get; set; }
            public decimal? slm_FT { get; set; }
            public string slm_EmpCode { get; set; }
            public string UpgradeCount { get; set; }
            public string slm_InsCode { get; set; }
            public string slm_TeamTelesales_Code { get; set; }
            public string IsNew { get; set; }
            public string IsPartner { get; set; }
            public int? RACount { get; set; }
        }

        private partial class ExcelSheet049_3
        {
            public string slm_ticketId { get; set; }
            public DateTime? slm_IncentiveDate { get; set; }
            public string slm_Owner { get; set; }
            public string slm_ContractNo { get; set; }
            public int? slm_CoverageTypeId { get; set; }
            public decimal? NetGrossPremium { get; set; }
            public decimal? slm_DiscountBath { get; set; }
            public decimal? GrossPremiumPay { get; set; }
            public decimal? KK_INCOME { get; set; }
            public decimal? slm_Ins_Com_Id { get; set; }
            public string slm_Grade { get; set; }
        }

        public bool CreateExcel1v2(string filename, DateTime incFrom, DateTime incTo, DateTime callFrom, DateTime callTo, int Telesale, string staff)
        {
            bool ret = true;
            try
            {
                //                string sqlz = @"
                //select ren.slm_IncentiveDate, ren.slm_ReceiveNo, isnull(ren.slm_Grade, pre.slm_grade) as slm_Grade, ren.slm_ContractNo
                //    ,ren.slm_PolicyStartCoverDate, ren.slm_PolicyEndCoverDate, lead.slm_Owner, st.slm_StaffNameTh
                //    ,t.slm_TitleName, lead.slm_Name, lead.slm_LastName, pre.slm_BranchCode, brn.slm_BranchName, ren.slm_LicenseNo
                //    ,brand.slm_BrandName, ren.slm_RedbookYearGroup, cov.slm_ConverageTypeName, pre.slm_Voluntary_Channel_Key, pre.slm_Voluntary_Type_Name
                //    ,ren.slm_PolicyGrossPremiumTotal, ren.slm_PolicyNetGrossPremium, ren.slm_DiscountPercent, ren.slm_PolicyDiscountAmt
                //    ,ren.slm_PolicyGrossPremium, ren.slm_PolicyIncentiveAmount, ren.slm_RemarkPolicy, st.slm_MarketingCode
                //    ,tt.slm_TeamTelesales_Name, pre.slm_Voluntary_Gross_Premium, cus.slm_CitizenId 
                //    ,DateDiff(day, ren.slm_PolicyStartCoverDate, ren.slm_ReceiveDate) as DaybeforeCover
                //    , case when pre.slm_Voluntary_Company_Code = convert(varchar, ren.slm_InsuranceComId) then 'บริษัทเดิม' else 'บริษัทใหม่' end as CompanyCompare 
                //    ,pre.slm_Voluntary_Company_Code, ren.slm_InsuranceComId, comp.slm_InsNameTh, renc.slm_FT, st.slm_EmpCode
                //    ,case when renc.slm_CostSave > 0 then 'Up' else '' end as UpgradeCount, comp.slm_InsCode, tt.slm_TeamTelesales_Code
                //    ,case when lead.slm_ContractNoRefer is not null or pre.slm_Contract_Number is not null or ren.slm_ContractNo is not null then 'Yes' else '' end as IsNew
                //    ,case when comp.slm_InsType = '01' then 'Yes' else '' end IsPartner, ra.RACount

                //from kkslm_tr_lead lead
                //inner join kkslm_tr_renewinsurance ren on lead.slm_ticketId = ren.slm_TicketId
                //left join kkslm_tr_cusinfo cus on cus.slm_ticketId = lead.slm_ticketId
                //left join kkslm_tr_prelead pre on lead.slm_ticketId = pre.slm_TicketId
                //left join kkslm_ms_staff st on lead.slm_Owner = st.slm_Username
                //left join kkslm_ms_title t on lead.slm_TitleId = t.slm_TitleId
                //left join kkslm_ms_branch brn on pre.slm_BranchCode = brn.slm_BranchCode
                //left join kkslm_ms_redbook_brand brand on ren.slm_RedbookBrandCode = brand.slm_BrandCode 
                //left join kkslm_ms_coveragetype cov on ren.slm_CoverageTypeId = cov.slm_CoverageTypeId
                //left join kkslm_ms_teamtelesales tt on st.slm_teamtelesales_id = tt.slm_teamtelesales_id
                //left join operdb.dbo.kkslm_ms_ins_com comp on ren.slm_InsuranceComId = comp.slm_ins_com_id
                //left join kkslm_tr_renewinsurance_compare renc on ren.slm_renewinsureid = renc.slm_renewinsureid and renc.slm_selected = 1
                //left join (select slm_renewinsureid, count(*) as RACount from kkslm_tr_renewinsurance_rapremium group by slm_renewinsureid) ra on ren.slm_renewinsureid = ra.slm_renewinsureid
                //--left join kkslm_tr_prelead_phone_call ppc on pre.slm_prelead_Id = ppc.slm_prelead_Id
                //--left join kkslm_phone_call pc on lead.slm_ticketId = pc.slm_ticketId
                //where lead.is_Deleted = 0 and lead.slm_ticketId in (
                //	select distinct l.slm_ticketId
                //	from kkslm_tr_lead l
                //	inner join kkslm_tr_renewinsurance r on l.slm_ticketId = r.slm_ticketId
                //	left join kkslm_tr_prelead pr on l.slm_ticketId = pr.slm_ticketId
                //	left join kkslm_tr_prelead_phone_call ppc on pr.slm_prelead_Id = ppc.slm_prelead_Id
                //	left join kkslm_phone_call pc on l.slm_ticketId = pc.slm_ticketId
                //	{0}
                //)

                //";
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    //string whr = "";
                    //if (incFrom.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" r.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                    //if (incTo.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" r.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                    //if (callFrom.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" ( pc.slm_CreatedDate >= '{0}' OR ppc.slm_CreatedDate >= '{0}' ) ", SLMUtil.GetDateString(callFrom));
                    //if (callTo.Year != 1) whr += (whr == "" ? "" : " and ") + String.Format(" ( pc.slm_CreatedDate < '{0}' OR ppc.slm_CreatedDate < '{0}' ) ", SLMUtil.GetDateString(callTo.AddDays(1)));
                    //if (Telesale != 0) whr += (whr == "" ? "" : " and ") + String.Format(" l.slm_Owner in ( select slm_username from kkslm_ms_staff where slm_teamTelesales_id = {0} )", Telesale);
                    //if (!String.IsNullOrEmpty(staff)) whr += (whr == "" ? "" : " and ") + String.Format(" l.slm_Owner = '{0}' ", staff);

                    //sqlz = String.Format(sqlz, (whr == "" ? "" : " where " + whr));
                    //var lst = sdc.ExecuteStoreQuery<ExcelSheet049_1>(sqlz).ToList();


                    int maxrow = 50000;

                    var lst = sdc.SP_RPT_004_POLICY_RAW_DATA(
                        incFrom.Year == 1 ? null : (DateTime?)incFrom,
                        incTo.Year == 1 ? null : (DateTime?)incTo
                        ).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    if (Telesale != 0) lst = lst.Where(t => t.slm_TeamTelesales_Id == Telesale).ToList();
                    if (!String.IsNullOrEmpty(staff)) lst = lst.Where(t => t.slm_EmpCode == staff).ToList();

                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[] {
                        itm.INCENTIVE_MONTH_YEAR, //slm_IncentiveDate == null ? "" : itm.INCENTIVE_MONTH_YEAR.Value.ToString("MMM-yy"),
                        itm.RECEIVE_NO,
                        itm.YEAR_EXPIRED,
                        itm.POLICY_START_DATE,
                        itm.POLICY_END_DATE,
                        itm.STAFF_NAME,
                        itm.CONTRACT_NO,
                        itm.TITLE_NAME,
                        itm.CUS_NAME,
                        itm.CUS_LAST_NAME,   // 10
                        itm.BRANCH_CODE,
                        itm.BRANCH_NAME,
                        itm.LICENSE_NO,
                        itm.BRAND_NAME,
                        itm.CAR_YEAR,
                        itm.COVERAGE_TYPE_NAME,
                        itm.INS_NAME,
                        itm.INS_OLD_CAR_TYPE_NAME,
                        itm.INS_NEW_CAR_TYPE_NAME,
                        itm.PROCESS_TYPE_RENEW, //20
                        itm.POLICY_GROSS_PREMIUM_TOTAL,
                        itm.POLICY_GROSS_PREMIUM,
                        itm.ACT_GROSS_PREMIUM_FULL,
                        itm.POLICY_DISCOUNT_PERCENT,
                        itm.POLICY_DISCOUNT_BATH,
                        itm.ACT_DISCOUNT_BATH,
                        itm.POLICY_NET_GROSS_PREMIUM,
                        itm.ACT_NET_GROSS_PREMIUM,
                        itm.KK_FULL_RECEIVE,
                        itm.KK_NET_RECEIVE_DISCOUNT_POLICY, //  30
                        itm.KK_NET_RECEIVE_DISCOUNT_ACT,
                        itm.REMARK_POLICY,
                        itm.MARKETING_CODE,
                        itm.HEAD_TEAM,
                        itm.POLICY_OLD_NET_GROSS_PREMIUM,
                        itm.CITIZEN_ID,
                        itm.POLICY_OLD_START_DATE,
                        itm.KOB_DATE,   // **38
                        itm.PAYMENT_TYPE,   // **39
                        itm.DAY_BEFORE_KOB, //40
                        itm.BUY_INS_OLD_NAME_TYPE,
                        itm.BUY_INS_TYPE,
                        itm.POLICY_OLD_NET_GROSS_PREMIUM,
                        itm.POLICY_EXPIRE_DATE,
                        itm.INSURANCE_OLD,
                        itm.EMP_GROUP_NAME,
                        itm.Y,
                        itm.POL_UPGRADE,
                        itm.EMP_CODE,
                        itm.INS_NAME2,    //50
                        itm.CODE,
                        itm.TEAM_CODE,
                        itm.CUS_NEW,
                        itm.PARTNER_FLAG,
                        itm.GRADE,
                        itm.RA,
                        itm.IS_EXPORT_EXPIRED.HasValue ? (itm.IS_EXPORT_EXPIRED.Value ? "Y" : "N") : "N",
                        itm.IS_EXPORT_EXPIRED_DATE,
                        itm.CMT_LOT,
                        itm.CHASSIS_NO,     //60
                        itm.INCENTIVE_DATE,
                        itm.RECEIVE_DATE,
                        itm.CHANNEL_ID,
                        itm.TICKET_ID
                        });
                    }


                    var lst2 = sdc.SP_RPT_004_ACT_RAW_DATA(
                        incFrom.Year == 1 ? null : (DateTime?)incFrom,
                        incTo.Year == 1 ? null : (DateTime?)incTo).ToList();

                    if (Telesale != 0) lst2 = lst2.Where(t => t.slm_TeamTelesales_Id == Telesale).ToList();
                    if (!String.IsNullOrEmpty(staff)) lst2 = lst2.Where(t => t.slm_EmpCode == staff).ToList();


                    List<object[]> data2 = new List<object[]>();
                    foreach (var itm in lst2)
                    {
                        data2.Add(new object[]
                        {
                            itm.INCENTIVE_MONTH_YEAR,  // 1
                            itm.RECEIVE_NO,
                            itm.YEAR_EXPIRED,
                            itm.ACT_START_DATE,
                            itm.ACT_END_DATE,
                            itm.STAFF_NAME,
                            itm.CONTRACT_NO,
                            itm.TITLE_NAME,
                            itm.CUS_NAME,
                            itm.CUS_LAST_NAME,  // 10
                            itm.BRANCH_CODE,
                            itm.BRANCH_NAME,
                            itm.LICENSE_NO,
                            itm.BRAND_NAME,
                            itm.CAR_YEAR,
                            itm.COVERAGE_TYPE_NAME,
                            itm.ACT_INS_NAME,
                            itm.INS_CAR_TYPE_NAME,
                            itm.PROCESS_TYPE_RENEW,
                            itm.POLICY_GROSS_PREMIUM_TOTAL,   // 20
                            itm.POLICY_GROSS_PREMIUM,
                            itm.ACT_GROSS_PREMIUM_FULL,
                            itm.POLICY_DISCOUNT_PERCENT,
                            itm.POLICY_DISCOUNT_BATH,
                            itm.ACT_DISCOUNT_BATH,
                            itm.POLICY_NET_GROSS_PREMIUM,
                            itm.ACT_NET_GROSS_PREMIUM,
                            itm.KK_FULL_RECEIVE,
                            itm.KK_NET_RECEIVE_DISCOUNT_POLICY,
                            itm.KK_NET_RECEIVE_DISCOUNT_ACT,    // 30
                            itm.ACT_OFFER,
                            itm.HEAD_TEAM,
                            itm.Z,
                            itm.EMP_ID,
                            itm.CODE_INS,
                            itm.TEAM_CODE,
                            itm.CODE,
                            itm.CUS_NEW,
                            itm.PARTNER_FLAG,
                            itm.GRADE,   // 40
                            itm.CMT_LOT,
                            itm.CHASSIS_NO,
                            itm.SEND_DATE,
                            itm.INCENTIVE_DATE,
                            itm.CHANNEL_ID,
                            itm.TICKET_ID
                        });
                    }

                    if (data.Count == 0 && data2.Count == 0) { throw new Exception("ไม่พบข้อมูล"); }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                            new ExcelExportBiz.SheetItem()
                            {
                                SheetName = "สมัครใจ",
                                RowPerSheet = maxrow,
                                Data = data,
                                Columns = new List<ExcelExportBiz.ColumnItem>()
                                {
                                    #region column
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ผลงานในเดือน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขรับแจ้ง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ปีที่หมดอายุ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่เริ่มคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันสิ้นสุดคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อเจ้าหน้าที่", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขที่สัญญาเช่าซื้อ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="คำนำหน้าชื่อ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อผู้เอาประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="นามสกุลผู้เอาประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Text }, //10
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สาขา (Code)", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สาขา", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขทะเบียน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อยี่ห้อรถ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ปีรถยนต์", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทประกันภัยรถยนต์", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รายชื่อบริษัทประกันภัย", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทความคุ้มครองเดิม", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทความคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วิธีการต่ออายุประกันภัย", ColumnDataType= ExcelExportBiz.ColumnType.Text }, // 20
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยประกันภัย (เบี้ยเต็ม)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยสุทธิ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ย พรบ (เบี้ยเต็ม)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ส่วนลด (%)", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ส่วนลดค่าเบี้ย (จำนวนเงิน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ส่วนลดค่าเบี้ย พรบ (จำนวนเงิน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยหลังหักส่วนลด", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่า พรบ หลังหักส่วนลด", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวนเงินที่ KK ได้รับจริงประกันภัย", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รายได้ที่ KK ได้รับหลังหักส่วนลด (ประกันภัย)", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //30
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รายได้ที่ KK ได้รับหลังหักส่วนลด (พรบ)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="หมายเหตุ (ประกันภัย)", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Marketing Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Head Team", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยประกันปีที่แล้ว (รวมภาษีอากร)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="บัตรประชาชนเลขที่/จดทะเบียนนิติบุคคล", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันสิ้นสุดความคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่ชำระเงินครบ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="การชำระเบี้ยประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวนวันก่อนเริ่มคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //40
                                    new ExcelExportBiz.ColumnItem() { ColumnName="บริษัทประกันเดิม", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="บริษัทเดิม/ใหม่", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยเดิม", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันสิ้นสุดกรมธรรม์", ColumnDataType= ExcelExportBiz.ColumnType.DateTime },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ทุนประกันเดิม", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สังกัด", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="y", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Pol Upgrade", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Emp Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อบริษัท", ColumnDataType= ExcelExportBiz.ColumnType.Text }, //50
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Team Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="New", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Partner", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เกรดลูกค้า", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="RA", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ออกรายงาน LeadsForTransfer", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่ออกรายงาน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Lot", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ตัวถัง", ColumnDataType= ExcelExportBiz.ColumnType.Text },  // 60
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่รับ Incentive", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่กดเลขรับแจ้ง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ช่องทาง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Ticket ID", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    #endregion
                                }
                            }
                            ,
                            new ExcelExportBiz.SheetItem()
                            {
                                SheetName = "พรบ",
                                RowPerSheet = maxrow,
                                Data = data2,
                                Columns = new List<ExcelExportBiz.ColumnItem>()
                                {
                                    #region column
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ผลงานในเดือน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขรับแจ้ง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ปีที่หมดอายุ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่เริ่มต้น พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันสิ้นสุดคุ้ม พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อเจ้าหน้าที่", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขที่สัญญาเช่าซื้อ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="คำนำหน้าชื่อ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อผู้เอาประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="นามสกุลผู้เอาประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Text }, //10
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สาขา (Code)", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สาขา", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขทะเบียน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อยี่ห้อรถ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ปีรถยนต์", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทประกันภัยรถยนต์", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รายชื่อบริษัท พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทความคุ้มครอง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วิธีการต่ออายุประกันภัย", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยประกันภัย (เบี้ยเต็ม)", ColumnDataType= ExcelExportBiz.ColumnType.Number },// 20
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ยสุทธิ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เบี้ย พรบ (เบี้ยเต็ม)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ส่วนลด (%)", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ส่วนลดค่าเบี้ย (จำนวนเงิน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ส่วนลดค่าเบี้ย พรบ (จำนวนเงิน)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยหลังหักส่วนลด", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่า พรบ หลังหักส่วนลด", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="จำนวนเงินที่ KK ได้รับจริง พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รายได้ที่ KK ได้รับหลังหักส่วนลด (ประกันภัย)", ColumnDataType= ExcelExportBiz.ColumnType.Number },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รายได้ที่ KK ได้รับหลังหักส่วนลด (พรบ)", ColumnDataType= ExcelExportBiz.ColumnType.Number }, //30
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สถานที่ออก พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Head Team", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Z", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Emp ID", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Code บริษัท", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Team Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Code", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="New", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Partner", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เกรดลูกค้า", ColumnDataType= ExcelExportBiz.ColumnType.Text },  //40
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Lot", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ตัวถัง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่ส่งแจ้ง พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่รับ Incentive พรบ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ช่องทาง", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Ticket ID", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    #endregion
                                }
                            }
                        }))
                        throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.Message;
            }
            return ret;
        }


    }
}
