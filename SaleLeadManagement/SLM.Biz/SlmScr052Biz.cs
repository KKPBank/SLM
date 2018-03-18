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
    public class SlmScr052Biz
    {
        //int maxrow = 100;
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
                        where tt.is_Deleted == 0 && tt.slm_TeamTelesales_Id == teamId
                        select new ControlListData() { TextField = tt.slm_StaffNameTH, ValueField = tt.slm_EmpCode }).ToList();
            }
        }
        public static List<ControlListData> GetInsuranceComList()
        {
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                return (from tt in operdb.kkslm_ms_ins_com
                        where tt.is_Deleted == false
                        select new ControlListData() { TextField = tt.slm_InsNameTh, ValueField = SqlFunctions.StringConvert((double)tt.slm_Ins_Com_Id).Trim() }).ToList();
            }
        }
        public static List<ControlListData> GetCoverageTypeList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return (from tt in slmdb.kkslm_ms_coveragetype
                        where tt.is_Deleted == false
                        select new ControlListData() { TextField = tt.slm_ConverageTypeName, ValueField = SqlFunctions.StringConvert((double)tt.slm_CoverageTypeId).Trim() }).ToList();
            }
        }

        public static List<SlmScr052SearchResult> GetSearchResult(DateTime renStart, DateTime renEnd, DateTime incentiveStart, DateTime incentiveEnd, DateTime coverStart, DateTime coverEnd, decimal telesaleTeam, string telesale, decimal insComId, int coverTypeId, string customerName, string customerLastname, string contractNo, out DataSet resultDs)
        {
            string commandText = @"select tt.slm_TeamTelesales_Code, slm_HeadStaff = headstaff.slm_StaffNameTH, lead.slm_Owner
	, slm_SubStatusName = isnull(status1.slm_SubStatusName, status2.slm_SubStatusName)
	, ren.slm_ContractNo, customerName = title.slm_TitleName + lead.slm_Name+ ' ' +lead.slm_LastName
	, remainGrossPremium = isnull(ren.slm_PolicyGrossPremium, 0) - isnull(recpDet.slm_RecAmount, 0)
	, paidGrossPremium = isnull(recpDet.slm_RecAmount, 0)
	, ren.slm_PolicyGrossPremium
	, ren.slm_PolicyGrossPremiumTotal
	, inscom.slm_InsNameTh, ren.slm_PolicyStartCoverDate, ren.slm_ReceiveDate, ren.slm_PolicyGrossPremiumTotal
	, coverType.slm_ConverageTypeName, ren.slm_RemarkPolicy, ren.slm_IncentiveDate, remark = ''
	, conclusion = case when (SELECT COUNT(LEAD2.slm_ticketId) AS CC 
								FROM KKSLM_TR_LEAD LEAD2 INNER JOIN kkslm_tr_renewinsurance REN2 ON REN2.slm_TicketId = LEAD2.slm_TicketId 
									INNER JOIN kkslm_tr_renewinsurance_receipt as rr ON RR.slm_ticketId = LEAD2.slm_TicketId INNER JOIN
									(SELECT  SUM(slm_RecAmount) recamount,slm_RenewInsuranceReceiptId 
										FROM kkslm_tr_renewinsurance_receipt_detail
										where slm_PaymentCode = '204'
										group by  slm_RenewInsuranceReceiptId  ) AS rrd 
									on rr.slm_RenewInsuranceReceiptId = rrd.slm_RenewInsuranceReceiptId 
								WHERE RR.slm_Status = '01' AND rrd.recamount <> 0 and slm_PayOptionId = 2 AND LEAD2.slm_SubStatus = '18' AND LEAD2.is_Deleted = 0
									and ren.slm_RenewInsureId = ren2.slm_renewInsureId
							) > 0 then 'ชำระแล้วรอเงินเข้าระบบ' 
						when (ren.slm_PayOptionId = 2 and isnull(ren.slm_PolicyGrossPremium, 0) <> isnull(recpDet.slm_RecAmount, 0)  
								and getdate() < ren.slm_PolicyStartCoverDate and ren.slm_ReceiveNo is not null) 
							then 'ผ่อนล่วงหน้าก่อนหมดประกัน'
						when (ren.slm_PayOptionId = 2 and isnull(ren.slm_PolicyGrossPremium, 0) <> isnull(recpDet.slm_RecAmount, 0)
								and getdate() >= ren.slm_PolicyStartCoverDate and ren.slm_ReceiveNo is not null) 
							then 'ไม่เข้าเงื่อนไข CBC'
						else '' end
 from kkslm_tr_renewinsurance as ren
	left join kkslm_tr_lead as lead on ren.slm_TicketId = lead.slm_ticketId
	left join kkslm_tr_renewinsurance_receipt as recp on lead.slm_ticketId = recp.slm_ticketId and recp.slm_status is null and recp.Is_Deleted = 0
	left join (select slm_RenewInsuranceReceiptId , slm_RecAmount = sum(isnull(slm_RecAmount, 0))
				from kkslm_tr_renewinsurance_receipt_revision_detail 
				where slm_PaymentCode = '204' and is_Deleted = 0 and slm_Status is null
				group by slm_RenewInsuranceReceiptId
			) as recpDet on recp.slm_RenewInsuranceReceiptId = recpDet.slm_RenewInsuranceReceiptId
	left join kkslm_ms_staff as staff on lead.slm_staffId = staff.slm_StaffId
	left join kkslm_ms_teamtelesales as tt on staff.slm_TeamTelesales_id = tt.slm_TeamTelesales_Id
	left join kkslm_ms_level as lev on staff.slm_Level = lev.slm_LevelId
	left join kkslm_ms_staff as headstaff on tt.slm_HeadStaff = headstaff.slm_StaffId
	left join kkslm_ms_title as title on lead.slm_titleID = title.slm_TitleId
	left join kkslm_tr_renewinsurance_rapremium rap on ren.slm_RenewInsureId = rap.slm_RenewInsureId
	left join kkslm_ms_config_product_premium  as cp on rap.slm_cp_Premium_id = cp.slm_cp_Premium_id
	left join kkslm_tr_renewinsurance_address as renAdd on ren.slm_renewInsureId = renAdd.slm_renewInsureId  and renAdd.slm_AddressType = 'D'
	left join kkslm_ms_redbook_brand as brand on ren.slm_RedbookBrandCode = brand.slm_brandCode
	left join kkslm_ms_redbook_model as model on ren.slm_redbookModelCode = model.slm_modelCode
	left join " + SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com as inscom on ren.slm_InsuranceComId = inscom.slm_Ins_Com_Id
	left join kkslm_ms_coveragetype coverType on ren.slm_CoverageTypeId = coverType.slm_CoverageTypeId
		left join kkslm_ms_config_product_substatus status1 on status1.is_Deleted = 0 and lead.slm_CampaignId = status1.slm_CampaignId and lead.slm_Status = status1.slm_OptionCode and lead.slm_SubStatus = status1.slm_SubStatusCode
	left join kkslm_ms_config_product_substatus status2 on status2.is_Deleted = 0 and lead.slm_Product_Id = status2.slm_CampaignId and lead.slm_Status = status2.slm_OptionCode and lead.slm_SubStatus = status2.slm_SubStatusCode 
    where 1=1 {0} 
    order by tt.slm_TeamTelesales_Code, lead.slm_Owner, conclusion desc
    ";

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
            if (telesale != "0" && telesale != "-1" && telesale != "")
            {
                where.Add(string.Format("lead.slm_Owner = '{0}'", telesale));
            }
            if (insComId > 0)
            {
                where.Add(string.Format("ren.slm_InsuranceComId = {0}", insComId));
            }
            if (coverTypeId > 0)
            {
                where.Add(string.Format("ren.slm_CoverageTypeId = {0}", coverTypeId));
            }
            if (customerName != string.Empty)
            {
                where.Add(string.Format("lead.slm_Name like '%{0}%'", customerName));
            }
            if (customerLastname != string.Empty)
            {
                where.Add(string.Format("lead.slm_LastName like '%{0}%'", customerLastname));
            }
            if (contractNo != string.Empty)
            {
                where.Add(string.Format("ren.slm_ContractNo like '%{0}%'", contractNo));
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
            return new List<SlmScr052SearchResult>();
        }

        public List<SP_RPT_002_PAYMENT_SUMMARY_Result> GetSearchResult2(DateTime renStart, DateTime renEnd, DateTime incentiveStart, DateTime incentiveEnd, DateTime coverStart, DateTime coverEnd, decimal telesaleTeam, string telesale, decimal insComId, int coverTypeId, string customerName, string customerLastname, string contractNo)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.SP_RPT_002_PAYMENT_SUMMARY(
                    SLMUtil.SafeInt(telesaleTeam.ToString()),
                    telesale,
                    SLMUtil.SafeInt(insComId.ToString()),
                    coverTypeId,
                    customerName.Trim(),
                    customerLastname.Trim(),
                    contractNo.Trim(),
                    GetDateString(incentiveStart),
                    GetDateString(incentiveEnd),
                    GetDateString(coverStart),
                    GetDateString(coverEnd),
                    GetDateString(renStart),
                    GetDateString(renEnd)
                    ).ToList();
            }
        }
        private static List<SlmScr052SearchResult> parseObject(DataTable table)
        {
            List<SlmScr052SearchResult> result = new List<SlmScr052SearchResult>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                SlmScr052SearchResult item = new SlmScr052SearchResult();
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

        public bool CreateExcel(DateTime renStart, DateTime renEnd, DateTime incentiveStart, DateTime incentiveEnd, DateTime coverStart, DateTime coverEnd, decimal telesaleTeam, string telesale, decimal insComId, int coverTypeId, string customerName, string customerLastname, string contractNo, string filename)
        {

            bool ret = true;
            try
            {
                DataSet resultDs;
                List<object[]> data = new List<object[]>();
                SlmScr052Biz.GetSearchResult(renStart, renEnd, incentiveStart, incentiveEnd, coverStart, coverEnd, telesaleTeam, telesale, insComId, coverTypeId, customerName, customerLastname, contractNo, out resultDs)
                   .ForEach(r =>
                   {
                       data.Add(new object[] {
                            r.slm_TeamTelesales_Code, r.slm_HeadStaff, r.slm_Owner, r.slm_SubStatusName, r.slm_ContractNo
                            , r.customerName, String.Format("{0:#,##0.00}", r.remainGrossPremium), String.Format("{0:#,##0.00}", r.paidGrossPremium), String.Format("{0:#,##0.00}", r.slm_PolicyGrossPremium), String.Format("{0:#,##0.00}", r.slm_PolicyGrossPremiumTotal)
                            , r.slm_InsNameTh, r.slm_PolicyStartCoverDate, r.slm_ReceiveDate, r.slm_PolicyGrossPremiumTotal, r.slm_ConverageTypeName
                            , r.slm_RemarkPolicy, r.slm_IncentiveDate, r.conclusion, r.remark
                        });
                   });

                ExcelExportBiz ebz = new ExcelExportBiz();
                if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "สรุปรายการเบี้ยประกันภัยค้างชำระ",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Team Code", ColumnDataType= ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ชื่อหัวหน้าทีม", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="MKT", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="สถานะย่อย", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Contract", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ค่าเบี้ยค้างชำระ", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เงินที่รับชำระ", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เบี้ยประกันหลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เบี้ยประกันตามกรมธรรม์", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="บริษัทประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่เริ่มคุ้มครอง", ColumnDataType = ExcelExportBiz.ColumnType.DateTime  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่รับเลขรับแจ้ง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ส่วนลด (บาท)", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ประเภทประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="หมายเหตุประกันภัย", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่รับ Incentive", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="สรุป", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="หมายเหตุอืนๆ", ColumnDataType = ExcelExportBiz.ColumnType.Text  }
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
        public bool CreateExcel2(DateTime renStart, DateTime renEnd, DateTime incentiveStart, DateTime incentiveEnd, DateTime coverStart, DateTime coverEnd, decimal telesaleTeam, string telesale, decimal insComId, int coverTypeId, string customerName, string customerLastname, string contractNo, string filename)
        {

            bool ret = true;
            try
            {
                List<object[]> data = new List<object[]>();
                GetSearchResult2(renStart, renEnd, incentiveStart, incentiveEnd, coverStart, coverEnd, telesaleTeam, telesale, insComId, coverTypeId, customerName, customerLastname, contractNo)
                    .ForEach(r =>
                    {
                        data.Add(new object[]
                        {
                            r.slm_TeamTelesales_Code,
                            r.slm_HeadStaffNameTH,
                            r.slm_StaffNameTH,
                            r.slm_SubStatusName,
                            r.slm_ContractNo,
                            r.slm_CustomerName,
                            r.slm_RemainAmt != null ? Math.Round(r.slm_RemainAmt.Value, 2) : 0,
                            r.slm_ReceivedAmt != null ? Math.Round(r.slm_ReceivedAmt.Value, 2) : 0,
                            r.slm_PolicyGrossPremium != null ? Math.Round(r.slm_PolicyGrossPremium.Value, 2) : 0,
                            r.slm_PolicyGrossPremiumTotal != null ? Math.Round(r.slm_PolicyGrossPremiumTotal.Value, 2) : 0,
                            r.slm_InsNameTh,
                            String.Format("{0:d/M/yyyy}", r.slm_PolicyStartCoverDate),
                            r.slm_ReceiveDate,
                            r.slm_PolicyDiscountAmt,
                            r.slm_ConverageTypeName,
                            r.slm_RemarkPolicy,
                            r.slm_IncentiveDate,
                            r.slm_Summary,
                            r.slm_OtherRemark,
                            r.slm_IsExportExpired.HasValue ? (r.slm_IsExportExpired.Value ? "Y" : "N") : "N",
                            r.slm_IsExportExpiredDate
                        });
                    });
                //data.Add(new object[]
                //{
                //    null,
                //    null,
                //    null,
                //    null,
                //    null,
                //    null,
                //    data.Sum(d=> (decimal)d[6]),
                //    data.Sum(d=> (decimal)d[7]),
                //    data.Sum(d=> (decimal)d[8]),
                //    data.Sum(d=> (decimal)d[9]),
                //    null,
                //    null,
                //    null,
                //    null,
                //    null,
                //    null,
                //    null,
                //    null,
                //    null

                //});

                if (data.Count == 0) { throw new Exception("ไม่พบข้อมูล"); }

                ExcelExportBiz ebz = new ExcelExportBiz();
                if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "รายการเบี้ยประกันภัยค้างชำระ",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Team Code", ColumnDataType= ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ชื่อหัวหน้าทีม", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Telesales", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="สถานะย่อย", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Contract", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ค่าเบี้ยค้างชำระ", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เงินที่รับชำระ", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เบี้ยประกันหลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="เบี้ยประกันตามกรมธรรม์", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="บริษัทประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่เริ่มคุ้มครอง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่รับเลขรับแจ้ง", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ส่วนลด (บาท)", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ประเภทประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="หมายเหตุประกันภัย", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="วันที่รับ Incentive", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="สรุป", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="หมายเหตุอื่นๆ", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() { ColumnName="ออกรายงาน LeadsForTransfer", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName="วันที่ออกรายงาน", ColumnDataType= ExcelExportBiz.ColumnType.Text }
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

        private string GetDateString(DateTime dt)
        {
            return string.Format(dt.ToString("{0}-MM-dd"), dt.Year.ToString("0000"));
        }

    }

    public class SlmScr052SearchResult
    {
        public string slm_TeamTelesales_Code { get; set; }
        public string slm_HeadStaff { get; set; }
        public string slm_Owner { get; set; }
        public string slm_SubStatusName { get; set; }
        public string slm_ContractNo { get; set; }
        public string customerName { get; set; }
        public decimal remainGrossPremium { get; set; }
        public decimal paidGrossPremium { get; set; }
        public decimal slm_PolicyGrossPremium { get; set; }
        public decimal slm_PolicyGrossPremiumTotal { get; set; }
        public string slm_InsNameTh { get; set; }
        public DateTime slm_PolicyStartCoverDate { get; set; }
        public DateTime slm_ReceiveDate { get; set; }
        public string slm_ConverageTypeName { get; set; }
        public string slm_RemarkPolicy { get; set; }
        public DateTime slm_IncentiveDate { get; set; }
        public string remark { get; set; }
        public string conclusion { get; set; }

    }


}
