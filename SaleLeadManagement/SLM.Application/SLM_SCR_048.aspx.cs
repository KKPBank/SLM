using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Resource;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Application.Services;
using System.Xml.Serialization;
using System.IO;
using SLM.Application.Shared;
using System.Text;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_048 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_048));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.BuildCombo(cmbTelesale, SlmScr032Biz.GetTeamSalesList(), "ทั้งหมด");
            AppUtil.BuildCombo(cmbType, SlmScr041Biz.GetLevelData(), "ทั้งหมด");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage.SaleLead)this.Master).ShowReportMenu();
            ((Label)Page.Master.FindControl("lblTopic")).Text = "รายงานประจำวัน";

            if (!IsPostBack)
            {
                var curdate = DateTime.Now.Date;
                tdmIncFrom.DateValue = new DateTime(curdate.Year, curdate.Month, 1);
                tdmIncTo.DateValue = curdate;
                //tdmIncFrom.DateValue = new DateTime(2016, 08, 1);
                //tdmIncTo.DateValue = new DateTime(2016, 08, 31);
                tdmIncFrom.Enabled = false;
                tdmIncTo.Enabled = false;
            }
        }

        private void SetStaffCombo()
        {
            AppUtil.BuildCombo(cmbTelesaleName, SlmScr032Biz.GetSearchStaffList(cmbTelesale.SelectedValue == "" ? -1 : SLMUtil.SafeInt(cmbTelesale.SelectedValue)), "ทั้งหมด");
        }

        protected void cmbTeamTelesale_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SetStaffCombo();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void GetSearchData(int pageidx)
        {
            SlmScr048Biz bz = new SlmScr048Biz();
            //var lst = bz.GetDataForScreen(tdmIncFrom.DateValue, tdmIncTo.DateValue, tdmCovFrom.DateValue, tdmCovTo.DateValue, tdmLeadFrom.DateValue, tdmLeadTo.DateValue, cmbTelesale.SelectedValue, cmbTelesaleName.SelectedValue, cmbType.SelectedValue);// tdmIncFrom.DateValue, tdmIncTo.DateValue, tdmFrom.DateValue, tdmTo.DateValue, cmbTeamTelesale.SelectedValue, cmbTelsameName.SelectedValue, cmbStaffCategory.SelectedValue, cmbIncentiveFlat.SelectedValue);
            var lst = bz.GetDataForScreen2(tdmIncFrom.DateValue, tdmIncTo.DateValue, cmbTelesale.SelectedValue, cmbTelesaleName.SelectedValue, cmbType.SelectedValue);
            BindData(pgTop, pgBot, lst.ToArray(), pageidx);
        }

        private void BindData(GridviewPageController gvTop, GridviewPageController gvBot, object[] data, int pageidx)
        {
            gvTop.SetGridview(gvMain);
            gvBot.SetGridview(gvMain);
            gvTop.Update(data, pageidx, 100);
            gvBot.Update(data, pageidx, 100);
            //gvBot.GenerateRecordNumber(0, pageidx);

            bool vis = gvMain.Rows.Count > 0;
            pgTop.Visible = vis;
            pgBot.Visible = vis;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckCondition())
                    GetSearchData(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void pg_PageChange(object sender, EventArgs e)
        {
            var pg = sender as GridviewPageController;
            if (pg != null)
                GetSearchData(pg.SelectedPageIndex);
        }

        bool CheckCondition()
        {
            bool ret = true;
            //if (tdmLeadFrom.DateValue.Year == 1 || tdmLeadTo.DateValue.Year == 1) { AppUtil.ClientAlert(this, "กรุณาระบุวันที่แจก lead เริ่มต้น และ สิ้นสุด"); return false; }
            //if (tdmIncFrom.DateValue.Year == 1 || tdmIncTo.DateValue.Year == 1) { AppUtil.ClientAlert(this, "กรุณาระบุวันที่กดเบิก Incentive เริ่มต้น และ สิ้นสุด"); return false; }
            //if (tdmLeadFrom.DateValue != tdmLeadTo.DateValue) { AppUtil.ClientAlert(this, "ไม่สามารถเลือกวันที่แจก lead มากกว่า 1 วันได้"); return false; }

            //if (tdmIncFrom.DateValue.Year == 1 || tdmIncTo.DateValue.Year == 1) { AppUtil.ClientAlert(this, "กรุณาระบุวันที่กดเบิก Incentive เริ่มต้น และ สิ้นสุด"); return false; }
            //if (tdmIncFrom.DateValue.AddMonths(1) < tdmIncTo.DateValue) { AppUtil.ClientAlert(this, "ไม่สามารถเลือกวันที่กดเบิก Incentive มากกว่า 1 เดือนได้"); return false; }
            //if (tdmCovFrom.DateValue.AddMonths(1) < tdmCovTo.DateValue) { AppUtil.ClientAlert(this, "ไม่สามารถเลือกวันคุ้มครอง กธ. มากกว่า 1 เดือนได้"); return false; }
            //if (tdmFrom.DateValue.Year != 1 && tdmTo.DateValue.Year != 1 && tdmFrom.DateValue.AddMonths(1) < tdmTo.DateValue) { AppUtil.ClientAlert(this, "ไม่สามารถเลือกวันคุ้มครอง กธ มากกว่า 1 เดือนได้"); return false; }
            //if (tdmIncFrom.DateValue.Year == 1 && tdmIncTo.DateValue.Year == 1 && tdmFrom.DateValue.Year == 1 && tdmTo.DateValue.Year == 1 && cmbTeamTelesale.SelectedIndex == 0 && cmbTelsameName.SelectedIndex == 0 && cmbStaffCategory.SelectedIndex == 0 && cmbIncentiveFlat.SelectedIndex == 0)
            //{ AppUtil.ClientAlert(this, "กรุณาระบุเงื่อนไขอย่างน้อย 1 อย่าง"); return false; }
            return ret;
        }

        private void ExportExcel(string sourcefile, string exportfilename)
        {
            // 3 send excel to response
            Response.ClearHeaders();
            Response.HeaderEncoding = Request.Browser.IsBrowser("CHROME") ? Encoding.UTF8 : Encoding.Default;
            Response.ContentEncoding = System.Text.Encoding.Default;
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", (Request.Browser.IsBrowser("IE") ? HttpUtility.UrlEncode(exportfilename) : exportfilename)));
            Response.ContentType = "application/ms-excel";

            Response.BinaryWrite(File.ReadAllBytes(sourcefile));

            Response.End();
        }



        protected void btnTestCAS_Click(object sender, EventArgs e)
        {
            //string error;
            //Services.CASService.AddCARBatch(new Services.CASService.CASServiceData()
            //{
            //    ReferenceNo = "1111",
            //    TransactionDateTime = DateTime.Now,
            //    ChannelId = "Call Centest",

            //    SubscriptionId = "1111111111111",
            //    SubscriptionTypeId = 1,
            //    TicketId = "5555555",
            //    ProductGroupId = "1",
            //    ProductId = "10",
            //    TypeName = "ทดสอบ",
            //    AreaName = "ทดแสบ",
            //    SubAreaName = "ทดเสิบ",

            //    Status="ใหม่",
            //    SubStatus="มากๆ",

            //}
            //    , "OBT"
            //    , out error);

            //if (error != "") AppUtil.ClientAlert(this, error);
            //else AppUtil.ClientAlert(this, "SUCCESS");
        }

        protected void TestExcel_Click(object sender, EventArgs e)
        {
            ExcelExportBiz ebz = new ExcelExportBiz();


            if (!ebz.CreateExcel("D:\\test.xls", new List<ExcelExportBiz.SheetItem>()
            {
                new ExcelExportBiz.SheetItem()
                {
                  SheetName = "ทดสอบ",
                  RowPerSheet = 0,
                  Columns = new List<ExcelExportBiz.ColumnItem>()
                  {
                        new ExcelExportBiz.ColumnItem() { ColumnName = "ID", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                        new ExcelExportBiz.ColumnItem() { ColumnName = "Name", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                        new ExcelExportBiz.ColumnItem() { ColumnName = "Birth", ColumnDataType = ExcelExportBiz.ColumnType.DateTime }
                  },
                  Data = new List<object[]>()
                  {
                      new object[] { 1, "ทดสอบ1", new DateTime(1980,01,23) },
                      new object[] { 2, "ทดสอบ2zzz", new DateTime(1982,01,7) },
                      new object[] { 3, "ทดสอบ3 haha", new DateTime(1981,01,05) },
                      new object[] { 4, "ทดสอบ4 abcdef", new DateTime(1980,07,14) },
                      new object[] { 5, "ทดสอบ5 haheha", new DateTime(1986,04,6) },
                      new object[] { 6, "ทดสอบ6 jajaja", new DateTime(1984,01,3) },
                      new object[] { 7, "ทดสอบ7 jrrjrj", new DateTime(1989,02,2) },
                      new object[] { 8, "ทดสอบ8 lallaa", new DateTime(1980,03,25) },
                      new object[] { 9, "ทดสอบ9 sorry", new DateTime(1980,01,05) },
                      new object[] { 10, "ทดสอบ10 nega nega", new DateTime(1980,01,05) },
                      new object[] { 11, "ทดสอบ11", new DateTime(1985,01,15) }
                  }

                }
                ,
                new ExcelExportBiz.SheetItem()
                {
                  SheetName = "ไหนลองดู",
                  RowPerSheet = 0,
                  Columns = new List<ExcelExportBiz.ColumnItem>()
                  {
                        new ExcelExportBiz.ColumnItem() { ColumnName = "ID", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                        new ExcelExportBiz.ColumnItem() { ColumnName = "Full Name", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                        new ExcelExportBiz.ColumnItem() { ColumnName = "Birthday", ColumnDataType = ExcelExportBiz.ColumnType.DateTime }
                  },
                  Data = new List<object[]>()
                  {
                      new object[] { 1, "ทดสอบ1", new DateTime(1980,01,23) },
                      new object[] { 2, "ทดสอบ2zzz", new DateTime(1982,01,7) },
                      new object[] { 3, "ทดสอบ3 haha", new DateTime(1981,01,05) },
                      new object[] { 4, "ทดสอบ4 abcdef", new DateTime(1980,07,14) },
                      new object[] { 5, "ทดสอบ5 haheha", new DateTime(1986,04,6) },
                      new object[] { 6, "ทดสอบ6 jajaja", new DateTime(1984,01,3) },
                      new object[] { 7, "ทดสอบ7 jrrjrj", new DateTime(1989,02,2) },
                      new object[] { 8, "ทดสอบ8 lallaa", new DateTime(1980,03,25) },
                      new object[] { 9, "ทดสอบ9 sorry", new DateTime(1980,01,05) },
                      new object[] { 10, "ทดสอบ10 nega nega", new DateTime(1980,01,05) },
                      new object[] { 11, "ทดสอบ11", new DateTime(1985,01,15) }
                  }

                }

            })) lblResult.Text = ebz.ErrorMessage;
            else
                lblResult.Text = "OK!";
        }

        protected void XMLTest_Click(object sender, EventArgs e)
        {
            CARService c = new CARService();
            var xs = new XmlSerializer(typeof(CARService.ActivityItem));
            var act = new CARService.ActivityItem()
            {
                ReferenceNo = "string",
                ActivityDateTime = new DateTime(2016,1,1),
                ChannelID = "string",

                SubscriptionTypeID=0,
                SubscriptionID ="string",
                LeadID = "string",
                TicketID = "string", 
                SrID = "string",
                ContractID ="string",

                ProductGroupID = "string",
                ProductID = "string",
                CampaignID = "string",
                TypeID=0,
                AreaID = 0,
                SubAreaID=0,
                ActivityTypeID=0,
                TypeName = "string",
                AreaName="string",
                SubAreaName="string",
                ActivityTypeName="string",

                KKCISID="string",
                CISID="string",
                TrxSeqID="string",
                NoncustomerID="string",
                Status="string",
                SubStatus="string",

               OfficerInfoList= new List<CARService.DataItem>()
               {
                   new CARService.DataItem()
                   {
                       SeqNo = 1,
                       DataLabel = "string1",
                       DataValue = "string1"
                   },
                   new CARService.DataItem()
                   {
                       SeqNo =2,
                       DataLabel = "string2",
                       DataValue = "string2"
                   }
               },
               ContractInfoList = new List<CARService.DataItem>()
               {
                   new CARService.DataItem()
                   {
                       SeqNo = 1,
                       DataLabel = "string1",
                       DataValue = "string1"
                   },
                   new CARService.DataItem()
                   {
                       SeqNo =2,
                       DataLabel = "string2",
                       DataValue = "string2"
                   }
               },
               ProductInfoList = new List<CARService.DataItem>()
               {
                   new CARService.DataItem()
                   {
                       SeqNo = 1,
                       DataLabel = "string1",
                       DataValue = "string1"
                   },
                   new CARService.DataItem()
                   {
                       SeqNo =2,
                       DataLabel = "string2",
                       DataValue = "string2"
                   }
               },
               CustomerInfoList = new List<CARService.DataItem>()
               {
                   new CARService.DataItem()
                   {
                       SeqNo = 1,
                       DataLabel = "string1",
                       DataValue = "string1"
                   },
                   new CARService.DataItem()
                   {
                       SeqNo =2,
                       DataLabel = "string2",
                       DataValue = "string2"
                   }
               },
               ActivityInfoList = new List<CARService.DataItem>()
               {
                   new CARService.DataItem()
                   {
                       SeqNo = 1,
                       DataLabel = "string1",
                       DataValue = "string1"
                   },
                   new CARService.DataItem()
                   {
                       SeqNo =2,
                       DataLabel = "string2",
                       DataValue = "string2"
                   }
               }
            };

            using (StringWriter sw = new StringWriter())
            {
                xs.Serialize(sw, act);
                txtXML.Text = sw.ToString();
            }


        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SlmScr048Biz bz = new SlmScr048Biz();
                var filename = Path.GetTempPath() + "\\tmpexcel_048_" + Page.User.Identity.Name + ".xls";// Path.GetTempFileName();

                if (bz.CreateExcel2(filename, tdmIncFrom.DateValue, tdmIncTo.DateValue, tdmCovFrom.DateValue, tdmCovTo.DateValue, tdmLeadFrom.DateValue, tdmLeadTo.DateValue, cmbTelesale.SelectedValue, cmbTelesaleName.SelectedValue, cmbType.SelectedValue))
                {
                    ExportExcel(filename, "ผลงานประจำวันที่_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
                }
                else
                {
                    AppUtil.ClientAlert(this, bz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
    }
}