using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Biz;
using System.IO;
using SLM.Resource;
using SLM.Application.Utilities;
using System.Text;

namespace SLM.Application
{
    public partial class SLM_SCR_049 : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.BuildCombo(cmbTelesale, SlmScr032Biz.GetTeamSalesList(), "ทั้งหมด");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage.SaleLead)this.Master).ShowReportMenu();
            ((Label)Page.Master.FindControl("lblTopic")).Text = "รายงานข้อมูล Lead สำหรับ Raw Data";
            if (!IsPostBack)
            {
                tdmIncTo.DateValue = DateTime.Now.Date;
                tdmIncFrom.DateValue = tdmIncTo.DateValue.AddDays(-59);
                //tdmIncFrom.DateValue = tdmIncTo.DateValue.AddMonths(-1).AddDays(1);
            }
        }

        private void SetStaffCombo()
        {
            AppUtil.BuildCombo(cmbTelesaleName, SlmScr032Biz.GetSearchStaffList(cmbTelesale.SelectedValue == "" ? -1 : SLMUtil.SafeInt(cmbTelesale.SelectedValue)), "ทั้งหมด");
        }


        protected void btnExcel1_Click(object sender, EventArgs e)
        {
            if (tdmIncFrom.DateValue.Year == 1 || tdmIncTo.DateValue.Year == 1) { AppUtil.ClientAlert(this, "กรุณาเลือกเลือกช่วงวันที่กดเบิก Incentive ที่ต้องการค้นหา"); return; }
            if (tdmIncFrom.DateValue.Year != 1 && tdmIncTo.DateValue.Year != 1)
            {

                TimeSpan datediff = tdmIncTo.DateValue.Subtract(tdmIncFrom.DateValue);
                double totalday = datediff.TotalDays;
                if (totalday >= 60)
                {
                    AppUtil.ClientAlert(this, "ไม่สามารถค้นหาช่วงวันที่กดรับ Incentive มากกว่าหรือเท่ากับ 60 วันได้");
                    return; 
                }
            }

            SlmScr049Biz bz = new SlmScr049Biz();
            var filename = Path.GetTempPath() + "\\tmpexcel_049_" + Page.User.Identity.Name + ".xls";// Path.GetTempFileName();

            if (bz.CreateExcel1v2(filename, tdmIncFrom.DateValue, tdmIncTo.DateValue, tdmCallFrom.DateValue, tdmCallTo.DateValue, SLMUtil.SafeInt(cmbTelesale.SelectedValue), cmbTelesaleName.SelectedValue))
            {
                ExportExcel(filename, "BSAT_REPORT_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls");
            }
            else
            {
                AppUtil.ClientAlert(this, bz.ErrorMessage);
            }

        }

        protected void btnExcel2_Click(object sender, EventArgs e)
        {
            SlmScr049Biz bz = new SlmScr049Biz();
            var filename = Path.GetTempPath() + "\\tmpexcel_049_" + Page.User.Identity.Name + ".xls";// Path.GetTempFileName();

            if (bz.CreateExcel2(filename, tdmIncFrom.DateValue, tdmIncTo.DateValue, tdmCallFrom.DateValue, tdmCallTo.DateValue, SLMUtil.SafeInt(cmbTelesale.SelectedValue), cmbTelesaleName.SelectedValue))
            {
                ExportExcel(filename, "SLM049_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
            }
            else
            {
                AppUtil.ClientAlert(this, bz.ErrorMessage);
            }
        }

        //private bool VerifyInput()
        //{
        //    bool ret = true;

        //    if (tdmIncFrom.DateValue.Year == 1 || tdmIncTo.DateValue.Year == 1) { AppUtil.ClientAlert(this, "กรุณาระบุวันที่กดเบิก"); ret = false;  }
        //    if (tdmIncFrom.DateValue.Year != 1 && tdmIncTo.DateValue.Year != 1)
        //    {

        //       TimeSpan datediff =   tdmIncTo.DateValue.Subtract(tdmIncFrom.DateValue);
        //       double totalday = datediff.TotalDays;
        //       if (totalday > 3)
        //           ret = false;
        //       else
        //           ret = true;
        //    }
        //    return ret;
        //}

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

        protected void cmbTelesale_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStaffCombo();
        }
    }
}