using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Dal;
using SLM.Biz;
using SLM.Application.Shared;
using SLM.Application.Utilities;
using SLM.Resource;
using System.IO;
using System.Text;

namespace SLM.Application
{
    public partial class SLM_SCR_050 : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.BuildCombo(cmbTeamTelesale, SlmScr032Biz.GetTeamSalesList(), "ทั้งหมด");
            AppUtil.BuildCombo(cmbIncentiveFlat, SlmScr050Biz.GetGradeDataList(), "ทั้งหมด");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                tdmIncFrom.DateValue = DateTime.Now.Date.AddMonths(-1).AddDays(1);
                tdmIncTo.DateValue = DateTime.Now.Date;
            }
            ((MasterPage.SaleLead)this.Master).ShowReportMenu();
            ((Label)Page.Master.FindControl("lblTopic")).Text = "รายงานค่าน้ำมันเหมาจ่าย";
        }

        bool CheckCondition()
        {
            bool ret = true;
            if (tdmIncFrom.DateValue.Year == 1 || tdmIncTo.DateValue.Year == 1) { AppUtil.ClientAlert(this, "กรุณาระบุวันที่กดเบิก Incentive เริ่มต้น และ สิ้นสุด"); return false; }
            if (tdmIncFrom.DateValue.AddDays(60) < tdmIncTo.DateValue) { AppUtil.ClientAlert(this, "ไม่สามารถเลือกวันที่กดเบิก Incentive มากกว่า 60 วันได้"); return false; }
           // if (tdmFrom.DateValue.Year != 1 && tdmTo.DateValue.Year != 1 && tdmFrom.DateValue.AddDays(60) < tdmTo.DateValue) { AppUtil.ClientAlert(this, "ไม่สามารถเลือกวันคุ้มครอง กธ มากกว่า 60 วันได้"); return false; }
            if (tdmIncFrom.DateValue.Year == 1 && tdmIncTo.DateValue.Year == 1 && tdmFrom.DateValue.Year == 1 && tdmTo.DateValue.Year == 1 && cmbTeamTelesale.SelectedIndex == 0 && cmbTelsameName.SelectedIndex == 0 && cmbStaffCategory.SelectedIndex == 0 && cmbIncentiveFlat.SelectedIndex == 0)
            { AppUtil.ClientAlert(this, "กรุณาระบุเงื่อนไขอย่างน้อย 1 อย่าง"); return false; }
            return ret;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //if (tdmIncFrom.DateValue.Year == 1 || tdmIncTo.DateValue.Year == 1) { AppUtil.ClientAlert(this, "กรุณาระบุวันที่กดเบิก Incentive เริ่มต้น และ สิ้นสุด"); return; }
            //if (tdmIncFrom.DateValue.AddMonths(1) < tdmIncTo.DateValue) { AppUtil.ClientAlert(this, "ไม่สามารถเลือกวันที่กดเบิก Incentive มากกว่า 1 เดือนได้"); return; }
            //if (tdmFrom.DateValue.Year != 1 && tdmTo.DateValue.Year != 1 && tdmFrom.DateValue.AddMonths(1) < tdmTo.DateValue) { AppUtil.ClientAlert(this, "ไม่สามารถเลือกวันคุ้มครอง กธ มากกว่า 1 เดือนได้"); return; }
            //if (tdmFrom.DateValue.Year == 1 && tdmTo.DateValue.Year == 1 && cmbTeamTelesale.SelectedIndex == 0 && cmbTelsameName.SelectedIndex == 0 && cmbStaffCategory.SelectedIndex == 0 && cmbIncentiveFlat.SelectedIndex ==0 )
            //{ AppUtil.ClientAlert(this, "กรุณาระบุเงื่อนไขอย่างน้อย 1 อย่าง");return; }
            if (!CheckCondition()) return;
            GetSearchData(0);
        }

        private void GetSearchData(int pageidx)
        {
            SlmScr050Biz bz = new SlmScr050Biz();
            var lst = bz.GetDataForScreen(tdmIncFrom.DateValue, tdmIncTo.DateValue, tdmFrom.DateValue, tdmTo.DateValue, cmbTeamTelesale.SelectedValue, cmbTelsameName.SelectedValue, cmbStaffCategory.SelectedValue, cmbIncentiveFlat.SelectedValue);
            BindData(pgTop, pgBot, lst.ToArray(), pageidx);
        }

        private void BindData(GridviewPageController gvTop, GridviewPageController gvBot, object[] data, int pageidx)
        {
            gvTop.SetGridview(gvMain);
            gvBot.SetGridview(gvMain);
            gvTop.Update(data, pageidx, 10);
            gvBot.Update(data, pageidx, 10);

            bool vis = gvMain.Rows.Count > 0;
            pgTop.Visible = vis;
            //pgBot.Visible = vis;
        }

        protected void pg_PageChange(object sender, EventArgs e)
        {
            var pg = sender as GridviewPageController;
            if (pg != null)
                GetSearchData(pg.SelectedPageIndex);            
        }

        private void SetStaffCombo()
        {
            AppUtil.BuildCombo(cmbTelsameName, SlmScr032Biz.GetSearchStaffList(SLMUtil.SafeInt(cmbTeamTelesale.SelectedValue)), "ทั้งหมด");
        }

        protected void cmbTeamTelesale_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStaffCombo();
        }

        protected void btnExcel1_Click(object sender, EventArgs e)
        {
            if (!CheckCondition()) return;
            SlmScr050Biz bz = new SlmScr050Biz();
            var filename = Path.GetTempPath() + "tmpexcel_050_" + Page.User.Identity.Name + ".csv";// Path.GetTempFileName();

            if (bz.CreateExcel1V2(filename, tdmIncFrom.DateValue, tdmIncTo.DateValue, tdmFrom.DateValue, tdmTo.DateValue, cmbTeamTelesale.SelectedValue, cmbTelsameName.SelectedValue, cmbStaffCategory.SelectedValue, cmbIncentiveFlat.SelectedValue))
            {
                ExportExcel(filename, "รายการเบิกค่าน้ำมันเหมาจ่าย_ประกันภัย_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
            }
            else
            {
                AppUtil.ClientAlert(this, bz.ErrorMessage);
            }
        }

        protected void btnExcel2_Click(object sender, EventArgs e)
        {
            if (!CheckCondition()) return;
            SlmScr050Biz bz = new SlmScr050Biz();
            var filename = Path.GetTempPath() + "tmpexcel_050_" + Page.User.Identity.Name + ".csv";// Path.GetTempFileName();

            if (bz.CreateExcel2V2(filename, tdmIncFrom.DateValue, tdmIncTo.DateValue, tdmFrom.DateValue, tdmTo.DateValue, cmbTeamTelesale.SelectedValue, cmbTelsameName.SelectedValue, cmbStaffCategory.SelectedValue, cmbIncentiveFlat.SelectedValue))
            {
                ExportExcel(filename, "รายการเบิกค่าน้ำมันเหมาจ่าย_พรบ_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
            }
            else
            {
                AppUtil.ClientAlert(this, bz.ErrorMessage);
            }
        }

        private void ExportExcel(string sourcefile, string exportfilename)
        {
            //// 3 send excel to response
            //Response.ClearHeaders();
            //Response.HeaderEncoding = Request.Browser.IsBrowser("CHROME") ? Encoding.UTF8 : Encoding.Default;
            //Response.ContentEncoding = System.Text.Encoding.Default;
            //Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", (Request.Browser.IsBrowser("IE") ? HttpUtility.UrlEncode(exportfilename) : exportfilename)));
            //Response.ContentType = "application/ms-excel";

            //Response.BinaryWrite(File.ReadAllBytes(sourcefile));

            //Response.End();

            Session["_downloadsource"] = sourcefile;
            //Response.Redirect(ResolveUrl("~/Shared/Download.aspx") + "?fname=" + exportfilename);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DOWNLOAD", "window.open('" + ResolveUrl("~/Shared/Download.aspx") + "?fname=" + exportfilename + "', 'DOWNLOAD', 'width=100,height=100');", true);
        }

        protected void btnExcel3_Click(object sender, EventArgs e)
        {
            if (!CheckCondition()) return;
            SlmScr050Biz bz = new SlmScr050Biz();
            var filename = Path.GetTempPath() + "tmpexcel_050_" + Page.User.Identity.Name + ".csv";// Path.GetTempFileName();

            if (bz.CreateExcel3V2(filename, tdmIncFrom.DateValue, tdmIncTo.DateValue, tdmFrom.DateValue, tdmTo.DateValue, cmbTeamTelesale.SelectedValue, cmbTelsameName.SelectedValue, cmbStaffCategory.SelectedValue, cmbIncentiveFlat.SelectedValue))
            {
                ExportExcel(filename, "สรุปรายการเบิกค่าน้ำมันเหมาจ่าย_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
            }
            else
            {
                AppUtil.ClientAlert(this, bz.ErrorMessage);
            }
        }
    }
}