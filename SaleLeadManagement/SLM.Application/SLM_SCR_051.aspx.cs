using SLM.Application.Utilities;
using SLM.Biz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Resource;
using System.IO;
using System.Text;

namespace SLM.Application
{
    public partial class SLM_SCR_051 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage.SaleLead)this.Master).ShowReportMenu();
            ((Label)Page.Master.FindControl("lblTopic")).Text = "รายงาน RA (พรีเมี่ยม)";

            if (!IsPostBack)
            {
                cmbTelesalesTeam.DataSource = SlmScr051Biz.GetTeamTelesaleList();
                cmbTelesalesTeam.DataBind();
                cmbTelesalesTeam.Items.Insert(0, new ListItem("ทั้งหมด", "-1"));
            }
        }

        protected void cmbTelesalesTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTelesaleName();
            //int teamId = 0;
            //if (int.TryParse(cmbTelesalesTeam.SelectedValue, out teamId))
            //{
            //    AppUtil.BuildCombo(cmbTelesales, SlmScr051Biz.GetTelesaleList(teamId));
            //    //if (teamId != -1)
            //    //{
            //    //    cmbTelesales.DataSource = SlmScr051Biz.GetTelesaleList(teamId);
            //    //    cmbTelesales.DataBind();
            //    //}
            //    //else
            //    //{
            //    //    cmbTelesales.DataSource = null;
            //    //    cmbTelesales.DataBind();
            //    //}
            //}            
        }

        private void SetTelesaleName()
        {
            AppUtil.BuildCombo(cmbTelesales, SlmScr051Biz.GetTelesaleList(SLMUtil.SafeInt(cmbTelesalesTeam.SelectedValue)), "ทั้งหมด");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {            
            PageSearchChange(null, null);
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                if (!ValidInput())
                {
                    //AppUtil.ClientAlert(Page, "กรุณาเลือกวันที่กดรับเลขรับแจ้งเริ่มต้น และ วันที่กดรับเลขรับแจ้งสิ้นสุด");
                    return;
                }

                decimal teleTeam;
                string telesale;
                decimal.TryParse(cmbTelesalesTeam.SelectedValue, out teleTeam);
                telesale = cmbTelesales.SelectedValue;

                var biz = new SlmScr051Biz();
                var resultList = biz.GetSearchResult2(tdStartDate.DateValue, tdEndDate.DateValue, tdIncentiveStartDate.DateValue, tdIncentiveEndDate.DateValue, tdProtectStartDate.DateValue, tdProtectEndDate.DateValue, teleTeam, telesale).OrderBy(r=>r.slm_PremiumName).ToList();
                if (sender == null)
                    BindGridview(pcTop, gvResult, resultList.ToArray(), 0);
                else
                    BindGridview(pcTop, gvResult, resultList.ToArray(), pcTop.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                // _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
            upResult.Update();
        }

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, GridView gv, object[] items, int pageIndex)
        {            
            pageControl.SetGridview(gv);
            pageControl.Update(items, pageIndex, 100);
            pageControl.Visible = gvResult.Rows.Count > 0;
            gvResult.Visible = true;
        }

        private bool ValidInput()
        {
            //bool criteriaSelected = true;            
            //if (tdStartDate.DateValue.AddMonths(1) < tdEndDate.DateValue)
            //{
            //    AppUtil.ClientAlert(Page, "ไม่สามารถเลือกวันที่กดรับเลขรับแจ้ง มากกว่า 1 เดือนได้");
            //    return false;
            //}
            //else if (tdStartDate.DateValue != DateTime.MinValue && tdEndDate.DateValue != DateTime.MinValue)
            //    criteriaSelected = true;

            //if (tdIncentiveStartDate.DateValue.AddMonths(1) < tdIncentiveEndDate.DateValue)
            //{
            //    AppUtil.ClientAlert(Page, "ไม่สามารถเลือกวันที่กดเบิก Incentive มากกว่า 1 เดือนได้");
            //    return false;
            //}
            //else if (tdIncentiveStartDate.DateValue != DateTime.MinValue && tdIncentiveEndDate.DateValue != DateTime.MinValue)
            //    criteriaSelected = true;

            //if (tdProtectStartDate.DateValue.AddMonths(1) < tdProtectEndDate.DateValue)
            //{
            //    AppUtil.ClientAlert(Page, "ไม่สามารถเลือกวันที่คุ้มครอง กธ มากกว่า 1 เดือนได้");
            //    return false;
            //}
            //else if (tdProtectStartDate.DateValue != DateTime.MinValue && tdProtectEndDate.DateValue != DateTime.MinValue)
            //    criteriaSelected = true;

            //return criteriaSelected || cmbTelesales.SelectedIndex > 0 || cmbTelesalesTeam.SelectedIndex > 0;


            string err = "";
            if (tdIncentiveStartDate.DateValue.Year == 1 || tdIncentiveEndDate.DateValue.Year == 1)
                err = "กรุณาเลือกวันที่กดเบิก Incentive เริ่มต้น และสิ้นสุด";
            else if (tdIncentiveStartDate.DateValue.AddDays(60) < tdIncentiveEndDate.DateValue)
                err = "ไม่สามารถค้นหาช่วงวันที่กดเบิก Incentive มากกว่า 60 วันได้";
            else if ((tdStartDate.DateValue.Year == 1 && tdEndDate.DateValue.Year != 1) ||
                     (tdStartDate.DateValue.Year != 1 && tdEndDate.DateValue.Year == 1))
                err = "กรุณาเลือกวันที่กดเลขรับแจ้ง เริ่มต้น และสิ้นสุด";
            else if ((tdProtectStartDate.DateValue.Year == 1 && tdProtectEndDate.DateValue.Year != 1) ||
                     (tdProtectStartDate.DateValue.Year != 1 && tdProtectEndDate.DateValue.Year == 1))
                err = "กรุณาเลือกวันที่คุ้มครอง กธ เริ่มต้น และสิ้นสุด";
            
            if (err != "")
            {
                AppUtil.ClientAlert(this, err);
                return false;
            }
            else return true;

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tdIncentiveStartDate.DateValue = new DateTime();
            tdIncentiveEndDate.DateValue = new DateTime();
            tdProtectStartDate.DateValue = new DateTime();
            tdProtectEndDate.DateValue = new DateTime();
            cmbTelesalesTeam.SelectedIndex = 0;
            SetTelesaleName();
            gvResult.DataSource = null;
            gvResult.DataBind();

            gvResult.Visible = false;
            pcTop.Visible = false;

            upResult.Update();
            
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (!ValidInput())
            {
                AppUtil.ClientAlert(Page, "กรุณาเลือกวันที่กดรับเลขรับแจ้งเริ่มต้น และ วันที่กดรับเลขรับแจ้งสิ้นสุด");
                return;
            }

            //DataSet resultDs; //Coment by Pom 04/07/2016 - Warning variable is never used
            decimal teleTeam;
            string telesale;
            decimal.TryParse(cmbTelesalesTeam.SelectedValue, out teleTeam);
            telesale = cmbTelesales.SelectedValue;


            var filename = Path.GetTempPath() + "\\tmpexcel_051_" + Page.User.Identity.Name + ".xls";// Path.GetTempFileName();
            var bz = new SlmScr051Biz();

            if (bz.CreateExcel2(tdStartDate.DateValue, tdEndDate.DateValue, tdIncentiveStartDate.DateValue, tdIncentiveEndDate.DateValue, tdProtectStartDate.DateValue, tdProtectEndDate.DateValue, teleTeam, telesale, filename))
            {
                ExportExcel(filename, "REPORT_PRIVILEGE_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls");                
            }
            else
            {
                AppUtil.ClientAlert(this, bz.ErrorMessage);
            }           
        }

        private void ExportExcel(string sourcefile, string exportfilename)
        {
            // 3 send excel to response
            Response.ClearHeaders();
            Response.HeaderEncoding = System.Text.Encoding.UTF8;
            Response.ContentEncoding = System.Text.Encoding.Default;
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", HttpUtility.UrlEncode(exportfilename, System.Text.Encoding.UTF8)));
            Response.ContentType = "application/ms-excel";

            Response.BinaryWrite(File.ReadAllBytes(sourcefile));

            Response.End();
        }
    }
}