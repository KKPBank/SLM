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
using System.Globalization;

namespace SLM.Biz
{
    public class SlmScr053Biz
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
                        where tt.is_Deleted == 0 && tt.slm_TeamTelesales_Id == teamId
                        select new ControlListData() { TextField = tt.slm_StaffNameTH, ValueField = tt.slm_EmpCode }).ToList();
            }
        }

        public static List<ControlListData> GetMonthList()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return (from tt in sdc.kkslm_ms_month
                        select new ControlListData() { TextField = tt.slm_MonthNameTh, ValueField = tt.slm_MonthId }).ToList();
            }
        }

        public static List<SlmScr053SearchResult> GetSearchResult(DateTime startDate, DateTime endDate, decimal telesaleTeam, string telesale, out DataSet resultDs)
        {
            string commandText = @"
select distinct slm_TeamTelesales_Code, slm_StaffNameTH, pl.slm_Owner
    , startPort = isnull(startPort.startPort, 0)
	, pendingPort = isnull(pendingPort.pendingPort, 0)
	, notContinue = ISNULL(notContinue.notContinue, 0)
from kkslm_tr_prelead pl 
	join kkslm_ms_teamtelesales	tt on pl.slm_Owner_Team = tt.slm_TeamTelesales_Id
	join kkslm_ms_staff as staff on pl.slm_owner = staff.slm_empCode
	left join (	
		SELECT COUNT(*) as startPort ,slm_Owner 
		FROM kkslm_tr_prelead 
		WHERE slm_AssignFlag = '1' AND slm_Assign_Status = '1' AND is_Deleted = 0 and slm_Status = '16' and slm_SubStatus not in ('06','07','08','09') and slm_TicketId is null
		group by slm_Owner) as startPort on pl.slm_owner = startPort.slm_Owner
	left join (
		SELECT SUM(Z.CNT) as pendingPort, Z.slm_Owner
		FROM(
		SELECT COUNT(*) CNT ,slm_Owner 
		FROM kkslm_tr_prelead 
		WHERE slm_AssignFlag = '1' AND slm_Assign_Status = '1' AND is_Deleted = 0 and slm_Status = '16' and slm_SubStatus not in ('06','07','08','09') and slm_TicketId is null
		group by slm_Owner
		union all
		SELECT COUNT(lead.slm_TitleId) CNT ,lead.slm_Owner 
		FROM kkslm_tr_prelead pre inner join kkslm_tr_lead lead on pre.slm_TicketId = lead.slm_ticketId
		WHERE pre.is_Deleted = 0 and lead.is_Deleted = 0 and lead.slm_Status not in ('08','09','10') and pre.slm_TicketId is not null
		group by lead.slm_Owner ) AS Z 
		GROUP BY Z.slm_Owner) as pendingPort on pl.slm_Owner = pendingPort.slm_Owner
	left join (
		SELECT SUM(Z.CNT) as notContinue , Z.slm_Owner
		FROM(
		SELECT COUNT(*) CNT ,slm_Owner 
		FROM kkslm_tr_prelead 
		WHERE slm_AssignFlag = '1' AND slm_Assign_Status = '1' AND is_Deleted = 0 and slm_Status = '16' and slm_SubStatus in ('06','07','08','09') and slm_TicketId is null
		group by slm_Owner
		union all
		SELECT COUNT(lead.slm_TitleId) CNT ,lead.slm_Owner 
		FROM kkslm_tr_prelead pre inner join kkslm_tr_lead lead on pre.slm_TicketId = lead.slm_ticketId
		WHERE pre.is_Deleted = 0 and lead.is_Deleted = 0 and lead.slm_Status in ('08','09','10') and pre.slm_TicketId is not null
		group by lead.slm_Owner ) AS Z 
		GROUP BY Z.slm_Owner) as notContinue on pl.slm_Owner = notContinue.slm_Owner
    where 1=1 {0} order by slm_TeamTelesales_Code, slm_StaffNameTH ";

            List<string> where = new List<string>();
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                where.Add(string.Format("pl.slm_CreatedDate between '{0}' and '{1}'", startDate.ToString(CultureInfo.InvariantCulture), endDate.ToString(CultureInfo.InvariantCulture)));
            }

            if (telesaleTeam > 0)
            {
                where.Add(string.Format("staff.slm_TeamTelesales_Id = {0}", telesaleTeam));
            }
            if (telesale != "")
            {
                where.Add(string.Format("pl.slm_Owner = '{0}'", telesale));
            }

            string whereClaus = "";
            if (where != null && where.Count > 0)
                whereClaus = " and " + string.Join(" and ", where.ToArray());

            commandText = string.Format(commandText, whereClaus);

            List<SlmScr053SearchResult> result = new List<SlmScr053SearchResult>();
            resultDs = new DataSet();
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.Connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, ((System.Data.EntityClient.EntityConnection)slmdb.Connection).StoreConnection.ConnectionString);

                adapter.Fill(resultDs);
                slmdb.Connection.Close();


                if (resultDs != null && resultDs.Tables.Count > 0)
                {
                    result = parseObject(resultDs.Tables[0]);
                }

                DateTime startMonth = endDate.AddMonths(-7);
                for (DateTime dt = startMonth; dt <= endDate; dt = dt.AddMonths(1))
                {
                    commandText = @"
SELECT COUNT(Z.slm_TitleId) as total, Z.slm_Owner, Z.COUNT_MONTH
FROM (
SELECT distinct lead.slm_TitleId,lead.slm_Owner,month(RRD.slm_TransDate ) COUNT_MONTH
FROM kkslm_tr_prelead pre inner join kkslm_tr_lead lead on pre.slm_TicketId = lead.slm_ticketId
INNER JOIN kkslm_tr_renewinsurance_receipt RR ON RR.slm_ticketId = LEAD.slm_ticketId 
INNER JOIN kkslm_tr_renewinsurance_receipt_detail RRD ON RRD.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId 
WHERE pre.is_Deleted = 0 and lead.is_Deleted = 0 and lead.slm_Status not in ('08','09','10') 
and pre.slm_TicketId is not null AND RRD.slm_TransDate between '{0}' and '{1}' ) AS Z
GROUP BY Z.slm_Owner, Z.COUNT_MONTH
";
                    resultDs.Clear();
                    resultDs.Tables.Clear();
                    commandText = string.Format(commandText, dt.ToString(CultureInfo.InvariantCulture), dt.AddMonths(1).AddMinutes(-1).ToString(CultureInfo.InvariantCulture));

                    slmdb.Connection.Open();
                    adapter = new SqlDataAdapter(commandText, ((System.Data.EntityClient.EntityConnection)slmdb.Connection).StoreConnection.ConnectionString);

                    adapter.Fill(resultDs);
                    slmdb.Connection.Close();

                    int i = dt.Month - startMonth.Month;


                    if (resultDs.Tables.Count > 0)
                    {
                        // dummy
                        //resultDs.Tables[0].Rows.Add(1, "100066", 1);

                        resultDs.Tables[0].Columns["total"].ColumnName = string.Format("prev{0}Count", i);
                        foreach (DataRow row in resultDs.Tables[0].Rows)
                        {
                            var item = result.FirstOrDefault(r => r.slm_Owner == row["slm_Owner"].ToString());
                            if (item != null)
                            {
                                item.GetType().GetProperties().ToList().ForEach(p =>
                                {
                                    if (p.Name != "slm_Owner" && row.Table.Columns.Contains(p.Name) && row[p.Name] != DBNull.Value)
                                    {
                                        p.SetValue(item, row[p.Name], null);
                                    }
                                });
                            }
                        }
                    }

                }

                result.ForEach(r =>
                {
                    r.grandTotal = r.prev0Count + r.prev1Count + r.prev2Count + r.prev3Count + r.prev4Count + r.prev5Count + r.prev6Count;
                    r.success = r.startPort == 0 ? 0 : r.grandTotal * 100 / r.startPort;
                });
            }

            return result;
        }

        public static List<SP_RPT_005_POLICY_CONT_COMPARE_MONTHLY_PORT_Result> GetSearchResult(int year, int month, int teamtelesale, string empCode)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.SP_RPT_005_POLICY_CONT_COMPARE_MONTHLY_PORT(year, month, teamtelesale, empCode).ToList();
            }
        }


        private static List<SlmScr053SearchResult> parseObject(DataTable table)
        {
            List<SlmScr053SearchResult> result = new List<SlmScr053SearchResult>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                SlmScr053SearchResult item = new SlmScr053SearchResult();
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

        public bool CreateExcel(DateTime startDate, DateTime endDate, decimal telesaleTeam, string telesale, string filename)
        {

            bool ret = true;
            try
            {
                DataSet resultDs;
                List<object[]> data = new List<object[]>();
                SlmScr053Biz.GetSearchResult(startDate, endDate, telesaleTeam, telesale, out resultDs)
                   .ForEach(r =>
                   {
                       data.Add(new object[] {
                            r.slm_TeamTelesales_Code, r.slm_StaffNameTH, r.startPort, r.pendingPort, r.notContinue
                            , r.prev0Count, r.prev1Count, r.prev2Count, r.prev3Count, r.prev4Count
                            , r.prev5Count, r.prev6Count, r.grandTotal, r.success
                        });
                   });

                ExcelExportBiz ebz = new ExcelExportBiz();
                if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "สรุปผลการต่อประกัน",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Team Code", ColumnDataType= ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="ชื่อเจ้าหน้าที่ MKT", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Port ตั้งต้น", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Port คงค้าง", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Port ที่ไม่ต่อประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= endDate.AddMonths(-6).ToString("MMM"), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= endDate.AddMonths(-5).ToString("MMM"), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= endDate.AddMonths(-4).ToString("MMM"), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= endDate.AddMonths(-3).ToString("MMM"), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= endDate.AddMonths(-2).ToString("MMM"), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= endDate.AddMonths(-1).ToString("MMM"), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= endDate.ToString("MMM"), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="grand Total", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="% Success", ColumnDataType = ExcelExportBiz.ColumnType.Number  }
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
        public bool CreateExcelV2(string filename, int year, int month, int teamtelesale, string empCode)
        {

            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var lst = sdc.SP_RPT_005_POLICY_CONT_COMPARE_MONTHLY_PORT(year, month, teamtelesale, empCode).ToList();

                    //DataSet resultDs;
                    //List<object[]> data = new List<object[]>();
                    //SlmScr053Biz.GetSearchResult(startDate, endDate, telesaleTeam, telesale, out resultDs)
                    //   .ForEach(r =>
                    //   {
                    //       data.Add(new object[] {
                    //            r.slm_TeamTelesales_Code, r.slm_StaffNameTH, r.startPort, r.pendingPort, r.notContinue
                    //            , r.prev0Count, r.prev1Count, r.prev2Count, r.prev3Count, r.prev4Count
                    //            , r.prev5Count, r.prev6Count, r.grandTotal, r.success
                    //        });
                    //   });


                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");
                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[]
                        {
                           itm.slm_TeamTelesales_Code,
                           itm.slm_StaffNameTH,
                           itm.slm_Grade,
                           itm.startPort,
                           itm.pendingPort,
                           itm.notContinue,
                           itm.CountMonthB5,
                           itm.CountMonthB4,
                           itm.CountMonthB3,
                           itm.CountMonthB2,
                           itm.CountMonthB1,
                           itm.CountMonthCur,
                           itm.CountMonthA1,
                           itm.CountMonthA2,
                           IntFormat(itm.GrandTotal),
                           DecimalFormat(itm.PercentSuccess) + "%"
                        });
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "สรุปผลการต่อประกัน",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Team Code", ColumnDataType= ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Telesales", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Grade", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Port ตั้งต้น", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Port คงค้าง", ColumnDataType = ExcelExportBiz.ColumnType.Number  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Port ที่ไม่ต่อประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= GetMonthName(month-5), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= GetMonthName(month-4), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= GetMonthName(month-3), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= GetMonthName(month-2), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= GetMonthName(month-1), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= GetMonthName(month), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= GetMonthName(month+1), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName= GetMonthName(month+2), ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="Grand Total (ที่รับ Incentive แล้ว)", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                                   new ExcelExportBiz.ColumnItem() {ColumnName="% Success", ColumnDataType = ExcelExportBiz.ColumnType.Text  }
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
        static string[] monthname = { "ม.ค.", "ก.พ.", "มี.ค.", "เม.ย.", "พ.ค.", "มิ.ย.", "ก.ค.", "ส.ค.", "ก.ย.", "ต.ค.", "พ.ย.", "ธ.ค." };
        public static string GetMonthName(int month)
        {
            var realmonth = (month+12) % 12;
            if (realmonth == 0) realmonth = 12;

            return monthname[realmonth - 1];
        }

        public string DecimalFormat(decimal? val)
        {
            if (val == null) return "";
            else return val.Value.ToString("#,##0.00");
        }
        public string IntFormat(int? val)
        {
            if (val == null) return "";
            else return val.Value.ToString("#,##0");
        }

    }


    public class SlmScr053SearchResult
    {
        public string slm_Owner { get; set; }
        public string slm_TeamTelesales_Code { get; set; }
        public string slm_StaffNameTH { get; set; }
        public int startPort { get; set; }
        public int pendingPort { get; set; }
        public int notContinue { get; set; }
        public int prev0Count { get; set; }
        public int prev1Count { get; set; }
        public int prev2Count { get; set; }
        public int prev3Count { get; set; }
        public int prev4Count { get; set; }
        public int prev5Count { get; set; }
        public int prev6Count { get; set; }
        public int grandTotal { get; set; }
        public int success { get; set; }
    }

}
