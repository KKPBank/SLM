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
    public partial class SLM_SCR_052 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage.SaleLead)this.Master).ShowReportMenu();
            ((Label)Page.Master.FindControl("lblTopic")).Text = "สรุปข้อมูลการชำระ";

            if (!IsPostBack)
            {
                cmbTelesalesTeam.DataSource = SlmScr052Biz.GetTeamTelesaleList();
                cmbTelesalesTeam.DataBind();
                cmbTelesalesTeam.Items.Insert(0, new ListItem("ทั้งหมด", "-1"));

                cmbInsuranceCom.DataSource = SlmScr052Biz.GetInsuranceComList();
                cmbInsuranceCom.DataBind();
                cmbInsuranceCom.Items.Insert(0, new ListItem("ทั้งหมด", "-1"));

                cmbCoverageType.DataSource = SlmScr052Biz.GetCoverageTypeList();
                cmbCoverageType.DataBind();
                cmbCoverageType.Items.Insert(0, new ListItem("ทั้งหมด", "-1"));
            }
        }
        protected void cmbTelesalesTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTelesaleName();
            //int teamId = 0;
            //if (int.TryParse(cmbTelesalesTeam.SelectedValue, out teamId))
            //{
            //    if (teamId != -1)
            //    {
            //        cmbTelesales.DataSource = SlmScr052Biz.GetTelesaleList(teamId);
            //        cmbTelesales.DataBind();
            //    }
            //    else
            //    {
            //        cmbTelesales.DataSource = null;
            //        cmbTelesales.DataBind();
            //    }
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
                    AppUtil.ClientAlert(Page, "กรุณาเลือกวันที่กดรับเลขรับแจ้งเริ่มต้น และ วันที่กดรับเลขรับแจ้งสิ้นสุด");
                    return;
                }

                //DataSet resultDs;
                decimal teleTeam = -1, telesale = -1, insComId = -1;;
                decimal.TryParse(cmbTelesalesTeam.SelectedValue, out teleTeam);
                decimal.TryParse(cmbTelesales.SelectedValue, out telesale);
                decimal.TryParse(cmbInsuranceCom.SelectedValue, out insComId);

                int coverTypeId = -1;
                int.TryParse(cmbCoverageType.SelectedValue, out coverTypeId);

                //var resultList = SlmScr052Biz.GetSearchResult(tdStartDate.DateValue, tdEndDate.DateValue, tdIncentiveStartDate.DateValue, tdIncentiveEndDate.DateValue, tdProtectStartDate.DateValue, tdProtectEndDate.DateValue, teleTeam, cmbTelesales.SelectedValue, insComId, coverTypeId, txtCustomerName.Text, txtCustomerLastName.Text, txtContractNo.Text , out resultDs);
                SlmScr052Biz bz = new SlmScr052Biz();
                var resultList = bz.GetSearchResult2(tdStartDate.DateValue, tdEndDate.DateValue, tdIncentiveStartDate.DateValue, tdIncentiveEndDate.DateValue, tdProtectStartDate.DateValue, tdProtectEndDate.DateValue, teleTeam, cmbTelesales.SelectedValue, insComId, coverTypeId, txtCustomerName.Text, txtCustomerLastName.Text, txtContractNo.Text);
                if (sender == null)
                    BindGridview(pcTop, gvResult, resultList.ToArray(), 1);
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
            bool criteriaSelected = false;
            if (tdStartDate.DateValue.AddMonths(1) < tdEndDate.DateValue)
            {
                AppUtil.ClientAlert(Page, "ไม่สามารถเลือกวันที่กดรับเลขรับแจ้ง มากกว่า 1 เดือนได้");
                return false;
            }
            else if (tdStartDate.DateValue != DateTime.MinValue && tdEndDate.DateValue != DateTime.MinValue)
                criteriaSelected = true;

            if (tdIncentiveStartDate.DateValue.AddMonths(1) < tdIncentiveEndDate.DateValue)
            {
                AppUtil.ClientAlert(Page, "ไม่สามารถเลือกวันที่กดเบิก Incentive มากกว่า 1 เดือนได้");
                return false;
            }
            else if (tdIncentiveStartDate.DateValue != DateTime.MinValue && tdIncentiveEndDate.DateValue != DateTime.MinValue)
                criteriaSelected = true;

            if (tdProtectStartDate.DateValue.AddMonths(1) < tdProtectEndDate.DateValue)
            {
                AppUtil.ClientAlert(Page, "ไม่สามารถเลือกวันที่คุ้มครอง กธ มากกว่า 1 เดือนได้");
                return false;
            }
            else if (tdProtectStartDate.DateValue != DateTime.MinValue && tdProtectEndDate.DateValue != DateTime.MinValue)
                criteriaSelected = true;

            return criteriaSelected || cmbTelesales.SelectedIndex > 0 || cmbTelesalesTeam.SelectedIndex > 0 || cmbCoverageType.SelectedIndex > 0 || cmbInsuranceCom.SelectedIndex > 0 || txtContractNo.Text != string.Empty || txtCustomerLastName.Text != string.Empty || txtCustomerName.Text != string.Empty;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            cmbTelesalesTeam.SelectedIndex = 0;
            SetTelesaleName();
            cmbInsuranceCom.SelectedIndex = 0;
            cmbCoverageType.SelectedIndex = 0;
            txtContractNo.Text = "";
            txtCustomerName.Text = "";
            txtCustomerLastName.Text = "";
            tdIncentiveStartDate.DateValue = new DateTime();
            tdIncentiveEndDate.DateValue = new DateTime();
            tdProtectStartDate.DateValue = new DateTime();
            tdProtectEndDate.DateValue = new DateTime();

            gvResult.DataSource = null;
            gvResult.DataBind();

            gvResult.Visible = false;
            pcTop.Visible = false;

            upResult.Update();

        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidInput())
                {
                    AppUtil.ClientAlert(Page, "กรุณาเลือกวันที่กดรับเลขรับแจ้งเริ่มต้น และ วันที่กดรับเลขรับแจ้งสิ้นสุด");
                    return;
                }

                //DataSet resultDs;     //Coment by Pom 04/07/2016 - Warning variable is never used
                decimal teleTeam = -1, telesale = -1, insComId = -1; ;
                decimal.TryParse(cmbTelesalesTeam.SelectedValue, out teleTeam);
                decimal.TryParse(cmbTelesales.SelectedValue, out telesale);
                decimal.TryParse(cmbInsuranceCom.SelectedValue, out insComId);

                int coverTypeId = -1;
                int.TryParse(cmbCoverageType.SelectedValue, out coverTypeId);

                var filename = Path.GetTempPath() + "\\tmpexcel_052_" + Page.User.Identity.Name + ".xls";// Path.GetTempFileName();
                var bz = new SlmScr052Biz();

                if (bz.CreateExcel2(tdStartDate.DateValue, tdEndDate.DateValue, tdIncentiveStartDate.DateValue, tdIncentiveEndDate.DateValue, tdProtectStartDate.DateValue, tdProtectEndDate.DateValue, teleTeam, cmbTelesales.SelectedValue, insComId, coverTypeId, txtCustomerName.Text, txtCustomerLastName.Text, txtContractNo.Text, filename))
                {
                    ExportExcel(filename, "สรุปข้อมูลการชำระ_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls");
                }
                else
                {
                    AppUtil.ClientAlert(this, bz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                // _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
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
    }

}