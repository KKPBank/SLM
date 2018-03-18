using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Dal;
using System.Data.Objects.SqlClient;
using System.Data.SqlClient;

namespace SLM.Biz
{
    public class SlmScr051Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }

        public static List<ControlListData> GetTeamTelesaleList()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return (from tt in sdc.kkslm_ms_teamtelesales
                        where tt.is_Deleted == false
                        select new ControlListData() { TextField = tt.slm_TeamTelesales_Name, ValueField = SqlFunctions.StringConvert((double)tt.slm_TeamTelesales_Id).Trim() }).ToList();
            }
        }
        public static List<ControlListData> GetTelesaleList(int teamId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return (from tt in sdc.kkslm_ms_staff
                        where tt.is_Deleted == 0 && ( tt.slm_TeamTelesales_Id == teamId || teamId == -1)
                        select new ControlListData() { TextField = tt.slm_StaffNameTH, ValueField = tt.slm_EmpCode }).ToList();
            }
        }

        public static List<SlmScr051SearchResult> GetSearchResult(DateTime renStart, DateTime renEnd, DateTime incentiveStart, DateTime incentiveEnd, DateTime coverStart, DateTime coverEnd, decimal telesaleTeam, decimal telesale, out DataSet resultDs)
        {
            string commandText = @"select Ren.slm_ReceiveDate, TAA = lead.slm_Owner, TLA = '', ren.slm_ContractNo, title.slm_TitleName, lev.slm_LevelName
	, lead.slm_Name, lead.slm_LastName, ren.slm_LicenseNo, slm_RegisProvinceNameTH = province.slm_ProvinceNameTH
	, Policy = '1', cov.slm_ConverageTypeName, com.slm_InsNameTh, ren. slm_ReceiveNo
	, ren.slm_PolicyStartCoverDate, ren. slm_PolicyEndCoverDate, payment.slm_PayMethodNameTH
	, ren. slm_PolicyGrossPremiumTotal, ren. slm_DiscountPercent, ren. slm_PolicyDiscountAmt
	, ren.slm_PolicyGrossPremium, tt.slm_TeamTelesales_Code, redBrand.slm_BrandName
	, ren.slm_RedbookYearGroup, redModel.slm_ModelName, ren. slm_ChassisNo
	, cp.slm_StartDate, cp.slm_EndDate, ra.slm_House_No, ra.slm_Moo, ra.slm_Village
	, ra.slm_BuildingName, ra.slm_Soi, ra.slm_Street, tambol.slm_TambolNameTH
	, amphur.slm_AmphurNameTH, raprovince.slm_ProvinceNameTH, ra.slm_PostalCode
	, lead.slm_TelNo_1, cp.slm_PremiumName
    , slm_PremiumType = case when cp.slm_PremiumType = '001' then 'สินค้า' when cp.slm_PremiumType = '002' then 'Service' else '' end
	, Ren.slm_IncentiveDate
from kkslm_tr_renewinsurance as ren
	left join  kkslm_tr_prelead pl on ren.slm_TicketId = pl.slm_TicketId 
	left join kkslm_ms_staff as staff on pl.slm_owner = staff.slm_EmpCode
	left join kkslm_ms_teamtelesales as tt on staff.slm_TeamTelesales_Id = tt.slm_TeamTelesales_Id
	left join kkslm_ms_level as lev on staff.slm_Level = lev.slm_LevelId
	left join kkslm_tr_lead as lead on pl.slm_TicketId = lead.slm_ticketId
	left join kkslm_tr_productinfo as pro on pl.slm_ticketId = pro.slm_ticketId	
	left join kkslm_ms_province province on pro.slm_ProvinceRegis = province.slm_ProvinceId
	left join kkslm_ms_title as title on lead.slm_TitleId = title.slm_TitleId
	left join kkslm_tr_renewinsurance_address as ra on ren.slm_RenewInsureId = ra.slm_RenewInsureId and ra.slm_AddressType = 'D'
	left join kkslm_tr_renewinsurance_rapremium as pre on ren.slm_RenewInsureId = pre.slm_RenewInsureId
	left join kkslm_ms_payment_methods as payment on ren.slm_PolicyPayMethodId = payment.slm_PayMethodId
	left join kkslm_ms_redbook_brand redBrand on ren.slm_RedbookBrandCode = redBrand.slm_BrandCode
	left join kkslm_ms_redbook_model redModel on ren.slm_RedbookModelCode = redModel.slm_ModelCode
	left join kkslm_ms_tambol tambol on ra.slm_Tambon = tambol.slm_TambolId
	left join kkslm_ms_amphur amphur on ra.slm_Amphur = amphur.slm_AmphurId
	left join kkslm_ms_province raprovince on ra.slm_Province = raprovince.slm_ProvinceId
	left join kkslm_ms_coveragetype cov on ren.slm_CoverageTypeId = cov.slm_CoverageTypeId
	left join kkslm_ms_config_product_premium  as cp on pl.slm_Product_Id = cp.slm_Product_Id  and cp.slm_IsActive = 1
	left join " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com com on ren.slm_InsuranceComId = com.slm_Ins_Com_Id 
    where 1=1 {0} order by cp .slm_PremiumType asc";

            List<string> where = new List<string>();
            if (renStart != DateTime.MinValue && renEnd != DateTime.MinValue)
            {
                where.Add(string.Format("ren.slm_ReceiveDate between '{0:yyyy-MM-dd}' and '{1:yyyy-MM-dd}'", SLMUtil.GetDateString(renStart), SLMUtil.GetDateString(renEnd)));
            }
            if (incentiveStart != DateTime.MinValue && incentiveEnd != DateTime.MinValue)
            {
                where.Add(string.Format("ren.slm_IncentiveDate between '{0:yyyy-MM-dd}' and '{1:yyyy-MM-dd}'", SLMUtil.GetDateString(incentiveStart), SLMUtil.GetDateString(incentiveEnd)));
            }
            if (coverStart != DateTime.MinValue && coverEnd != DateTime.MinValue)
            {
                where.Add(string.Format("ren.slm_PolicyStartCoverDate between '{0:yyyy-MM-dd}' and '{1:yyyy-MM-dd}'", SLMUtil.GetDateString(coverStart), SLMUtil.GetDateString(coverEnd)));
            }
            if (telesaleTeam > 0)
            {
                where.Add(string.Format("staff.slm_TeamTelesales_Id = {0}", telesaleTeam));
            }
            if (telesale > 0)
            {
                where.Add(string.Format("pl.slm_Owner = {0}", telesale));
            }

            string whereClaus = "";
            if (where != null && where.Count > 0)
                whereClaus = " and " + string.Join(" and ", where.ToArray());

            commandText = string.Format(commandText, whereClaus);

            resultDs = new DataSet();
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.Connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, ((System.Data.EntityClient.EntityConnection)slmdb.Connection).StoreConnection.ConnectionString);

                adapter.Fill(resultDs);
                slmdb.Connection.Close();
            }
            if (resultDs != null && resultDs.Tables.Count > 0)
            {
                return parseObject(resultDs.Tables[0]);
            }
            return new List<SlmScr051SearchResult>();
        }

        private static List<SlmScr051SearchResult> parseObject(DataTable table)
        {
            List<SlmScr051SearchResult> result = new List<SlmScr051SearchResult>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                SlmScr051SearchResult item = new SlmScr051SearchResult();
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

        public bool CreateExcel(DateTime renStart, DateTime renEnd, DateTime incentiveStart, DateTime incentiveEnd, DateTime coverStart, DateTime coverEnd, decimal telesaleTeam, decimal telesale, string filename)
        {

            bool ret = true;
            try
            {
                DataSet resultDs;
                List<object[]> data = new List<object[]>();
                SlmScr051Biz.GetSearchResult(renStart, renEnd, incentiveStart, incentiveEnd, coverStart, coverEnd, telesaleTeam, telesale, out resultDs)
                   .ForEach(r =>
                   {
                       data.Add(new object[] {
                        r.slm_ReceiveDate, r.TAA, r.TLA , r.slm_ContractNo, r.slm_TitleName
                       , r.slm_Name, r.slm_LastName, r.slm_LicenseNo, r.slm_RegisProvinceNameTH
	                   , r.Policy, r.slm_ConverageTypeName, r.slm_InsNameTh, r.slm_ReceiveNo, r.slm_PolicyStartCoverDate
                       , r.slm_PolicyEndCoverDate, r.slm_PayMethodNameTH, r.slm_PolicyGrossPremiumTotal, r.slm_DiscountPercent, r.slm_PolicyDiscountAmt
	                   , r.slm_PolicyGrossPremium, r.slm_TeamTelesales_Code, r.slm_BrandName, r.slm_RedbookYearGroup, r.slm_ModelName
                       , r. slm_ChassisNo, r.slm_StartDate, r.slm_EndDate, r.slm_House_No, r.slm_Moo
                       , r.slm_Village, r.slm_BuildingName, r.slm_Soi, r.slm_Street, r.slm_TambolNameTH
                       , r.slm_AmphurNameTH, r.slm_ProvinceNameTH, r.slm_PostalCode, r.slm_TelNo_1
                       , r.slm_PremiumType, r.slm_IncentiveDate
                        });
                   });

                ExcelExportBiz ebz = new ExcelExportBiz();
                if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "RA(พรีเมี่ยม)",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่รับเลขรับแจ้ง", ColumnDataType= ExcelExportBiz.ColumnType.DateTime  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เจ้าหน้าที่ TTA", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เจ้าหน้าที่ TLA", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เลขที่สัญญา", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="คำนำหน้าชื่อ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ชื่อผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="นามสกุลผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เลขทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="จังหวัด", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Policy", ColumnDataType = ExcelExportBiz.ColumnType.Text  },  // 10
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ประเภทประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="บริษัทประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เลขรับแจ้ง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันคุ้มครอง กธ", ColumnDataType = ExcelExportBiz.ColumnType.DateTime  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันสิ้นสุดวันคุ้มครอง กธ ", ColumnDataType = ExcelExportBiz.ColumnType.DateTime  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="การชำระเงิน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ค่าเบี้ยประกันภัย", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ส่วนลด %", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ส่วนลด บาท", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ค่าเบี้ยหลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Number  },  // 20
                                   new ExcelExportBiz.ColumnItem() {ColumnName="TeamCode", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ยี่ห้อรถ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="รุ่นปีที่จดทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="รุ่นรถ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เลขตัวถัง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันเริ่มอายุสมาชิก KK-Touch", ColumnDataType = ExcelExportBiz.ColumnType.DateTime  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันหมดอายุสมาชิก KK-Touch", ColumnDataType = ExcelExportBiz.ColumnType.DateTime  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="บ้านเลขที่", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="หมู่", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="หมู่บ้าน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="อาคาร", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ซอย", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ถนน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ตำบล/แขวง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="อำเภอ/เขต", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="จังหวัด2", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="รหัสไปรษณีย์", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เบอร์โทร", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ประเภทของที่แจก", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่เบิก Incentive", ColumnDataType = ExcelExportBiz.ColumnType.Text  }
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



        // new stored works 

        public bool CreateExcel2(DateTime renStart, DateTime renEnd, DateTime incentiveStart, DateTime incentiveEnd, DateTime coverStart, DateTime coverEnd, decimal telesaleTeam, string telesale, string filename)
        {
            bool ret = true;
            try
            {
                //DataSet resultDs;
                List<object[]> data = new List<object[]>();
                GetSearchResult2(renStart, renEnd, incentiveStart, incentiveEnd, coverStart, coverEnd, telesaleTeam, telesale)
                    .ForEach(r =>
                    {
                        data.Add(new object[]
                        {
                            r.slm_ReceiveDate,
                            r.slm_StaffNameTH,
                            r.slm_TLA_Officer, 
                            r.slm_ContractNo,
                            r.slm_TitleName,
                            r.slm_Name,
                            r.slm_LastName,
                            r.slm_LicenseNo,
                            r.slm_ProvinceNameTH,
                            r.policy,  // 10

                            r.slm_ConverageTypeName,
                            r.slm_InsNameTh,
                            r.slm_ReceiveNo,
                            String.Format("{0:d/M/yyyy}", r.slm_PolicyStartCoverDate),
                            String.Format("{0:d/M/yyyy}", r.slm_PolicyEndCoverDate),
                            r.slm_PayMethodNameTH,
                            r.slm_PolicyGrossPremiumTotal,
                            r.slm_DiscountPercent,
                            r.slm_PolicyDiscountAmt,
                            r.slm_PolicyGrossPremium, //20

                            r.slm_TeamTelesales_Code,
                            r.slm_BrandName,
                            r.slm_RedbookYearGroup,
                            r.slm_ModelName,
                            r.slm_ChassisNo,
                            String.Format("{0:d/M/yyyy}", r.slm_StartDate),
                            String.Format("{0:d/M/yyyy}", r.slm_EndDate),
                            r.slm_House_No,
                            r.slm_Moo,
                            r.slm_Village, //30

                            r.slm_BuildingName,
                            r.slm_Soi,
                            r.slm_Street,
                            r.slm_TambolNameTH,
                            r.slm_AmphurNameTH,
                            r.slm_ProvinceNameTH2,
                            r.slm_PostalCode,
                            r.slm_TelNo_1,
                            r.slm_PremiumName,
                            r.slm_IncentiveDate // 40
                        });
                    });

                if (data.Count <= 0) throw new Exception("ไม่พบข้อมูล");
                ExcelExportBiz ebz = new ExcelExportBiz();
                if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "RA(พรีเมี่ยม)",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่รับเลขรับแจ้ง", ColumnDataType= ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เจ้าหน้าที่ TTA", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เจ้าหน้าที่ TLA", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เลขที่สัญญา", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="คำนำหน้าชื่อ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ชื่อผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="นามสกุลผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เลขทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="จังหวัด", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Policy", ColumnDataType = ExcelExportBiz.ColumnType.Text  },  // 10
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ประเภทประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="บริษัทประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เลขรับแจ้ง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันคุ้มครอง กธ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันสิ้นสุดวันคุ้มครอง กธ ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="การชำระเงิน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ค่าเบี้ยประกันภัย", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ส่วนลด %", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ส่วนลด บาท", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ค่าเบี้ยหลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Number  },  // 20
                                   new ExcelExportBiz.ColumnItem() {ColumnName="TeamCode", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ยี่ห้อรถ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="รุ่นปีที่จดทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="รุ่นรถ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เลขตัวถัง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันเริ่มอายุสมาชิก KK-Touch", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันหมดอายุสมาชิก KK-Touch", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="บ้านเลขที่", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="หมู่", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="หมู่บ้าน", ColumnDataType = ExcelExportBiz.ColumnType.Text  }, //30
                                   new ExcelExportBiz.ColumnItem() {ColumnName="อาคาร", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ซอย", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ถนน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ตำบล/แขวง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="อำเภอ/เขต", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="จังหวัด2", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="รหัสไปรษณีย์", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เบอร์โทร", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ประเภทของที่แจก", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่เบิก Incentive", ColumnDataType = ExcelExportBiz.ColumnType.Text  }
                               }
                           }
                        }))
                    throw new Exception(ebz.ErrorMessage);

            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.Message;
            }
            return ret;
        }

        public List<SP_RPT_003_RA_PREMIUM_Result> GetSearchResult2(DateTime renStart, DateTime renEnd, DateTime incentiveStart, DateTime incentiveEnd, DateTime coverStart, DateTime coverEnd, decimal telesaleTeam, string telesale)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.SP_RPT_003_RA_PREMIUM(
                   GetDateString(renStart),
                   GetDateString(renEnd),
                   GetDateString(incentiveStart),
                   GetDateString(incentiveEnd),
                   GetDateString(coverStart),
                   GetDateString(coverEnd),
                   SLMUtil.SafeInt(telesaleTeam.ToString()),
                   telesale
                ).ToList();
            }
        }

        private string GetDateString(DateTime dt)
        {
            return string.Format(dt.ToString("{0}-MM-dd"), dt.Year.ToString("0000"));
        }
    }

    public class SlmScr051SearchResult
    {
        public DateTime slm_ReceiveDate { get; set; }
        public string TAA { get; set; }
        public string slm_LevelName { get; set; }
        public string TLA { get; set; }
        public string slm_ContractNo { get; set; }
        public string slm_TitleName { get; set; }
        public string slm_Name { get; set; }
        public string slm_LastName { get; set; }
        public string slm_LicenseNo { get; set; }
        public string slm_RegisProvinceNameTH { get; set; }
        public string Policy { get; set; }
        public string slm_ConverageTypeName { get; set; }
        public string slm_InsNameTh { get; set; }
        public string slm_ReceiveNo { get; set; }
        public DateTime slm_PolicyStartCoverDate { get; set; }
        public DateTime slm_PolicyEndCoverDate { get; set; }
        public string slm_PayMethodNameTH { get; set; }
        public decimal slm_PolicyGrossPremiumTotal { get; set; }
        public decimal slm_DiscountPercent { get; set; }
        public decimal slm_PolicyDiscountAmt { get; set; }
        public decimal slm_PolicyGrossPremium { get; set; }
        public string slm_TeamTelesales_Code { get; set; }
        public string slm_BrandName { get; set; }
        public Int32 slm_RedbookYearGroup { get; set; }
        public string slm_ModelName { get; set; }
        public string slm_ChassisNo { get; set; }
        public DateTime slm_StartDate { get; set; }
        public DateTime slm_EndDate { get; set; }
        public string slm_House_No { get; set; }
        public string slm_Moo { get; set; }
        public string slm_Village { get; set; }
        public string slm_BuildingName { get; set; }
        public string slm_Soi { get; set; }
        public string slm_Street { get; set; }
        public string slm_TambolNameTH { get; set; }
        public string slm_AmphurNameTH { get; set; }
        public string slm_ProvinceNameTH { get; set; }
        public string slm_PostalCode { get; set; }
        public string slm_TelNo_1 { get; set; }
        public string slm_PremiumType { get; set; }
        public string slm_PremiumName { get; set; }
        public DateTime slm_IncentiveDate { get; set; }
    }

}
