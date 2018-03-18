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
    public partial class SLM_SCR_053 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage.SaleLead)this.Master).ShowReportMenu();
            ((Label)Page.Master.FindControl("lblTopic")).Text = "สรุปผลการต่อประกันเทียบ Port รายเดือน";

            if (!IsPostBack)
            {
                //cmbTelesalesTeam.DataSource = SlmScr053Biz.GetTeamTelesaleList();
                //cmbTelesalesTeam.DataBind();
                //cmbTelesalesTeam.Items.Insert(0, new ListItem("ทั้งหมด", "-1"));

                AppUtil.BuildCombo(cmbTelesalesTeam, SlmScr053Biz.GetTeamTelesaleList(), "ทั้งหมด", "-1");

                //for(int i = -5; i <= 5; i++)
                //{
                //    string year = DateTime.Today.AddYears(i).Year.ToString();
                //    cmbYear.Items.Add(new ListItem(year, year));
                //    cmbMonth.Items.Add(new ListItem(year, year));
                //}

                cmbMonth.DataSource = SlmScr053Biz.GetMonthList();
                cmbMonth.DataBind();
                cmbMonth.Items.Insert(0, new ListItem("ระบุ", "0"));

                cmbYear.Items.Clear();
                cmbYear.Items.Add(new ListItem("ระบุ", "0"));
                int curyear = DateTime.Now.Year;
                for (int i = 0; i < 5; i++)
                {
                    int yr = curyear - i;
                    cmbYear.Items.Add(new ListItem(yr.ToString(), yr.ToString()));
                }
            }
        }


        protected void cmbTelesalesTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTelesaleName();           
        }
        private void SetTelesaleName()
        {
            AppUtil.BuildCombo(cmbTelesales, SlmScr051Biz.GetTelesaleList(SLMUtil.SafeInt(cmbTelesalesTeam.SelectedValue)), "ทั้งหมด");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (ValidInput()) 
                SetDataList(1);// PageSearchChange(null, null);
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                if (!ValidInput())
                {                    
                    return;
                }

                //DataSet resultDs;
                //decimal teleTeam = -1;  //, telesale = -1;      //Comment By Pom 17/06/2016 - warning, variable is never used
                //decimal.TryParse(cmbTelesalesTeam.SelectedValue, out teleTeam);
                //// decimal.TryParse(cmbTelesales.SelectedValue, out telesale);

                //int startyear, startmonth, endyear, endmonth;
                //int.TryParse(cmbYear.SelectedValue, out startyear);
                //int.TryParse(cmbStartMonth.SelectedValue, out startmonth);
                //int.TryParse(cmbMonth.SelectedValue, out endyear);
                //int.TryParse(cmbEndMonth.SelectedValue, out endmonth);
                //DateTime startDate = new DateTime(startyear, startmonth, 1);
                //DateTime endDate = new DateTime(endyear, endmonth, 1);

                //var resultList = SlmScr053Biz.GetSearchResult(startDate, endDate, teleTeam, cmbTelesales.SelectedValue, out resultDs);
                //if (sender == null)
                //    BindGridview(pcTop, gvResult, resultList.ToArray(), 1);
                //else
                //    BindGridview(pcTop, gvResult, resultList.ToArray(), pcTop.SelectedPageIndex);
                SetDataList(pcTop.SelectedPageIndex);

            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                // _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
            upResult.Update();
        }

        private void SetDataList(int page)
        {
            try
            {
                var mn = SLMUtil.SafeInt(cmbMonth.SelectedValue);
                gvResult.Columns[5].HeaderText = SlmScr053Biz.GetMonthName(mn - 5);
                gvResult.Columns[6].HeaderText = SlmScr053Biz.GetMonthName(mn - 4);
                gvResult.Columns[7].HeaderText = SlmScr053Biz.GetMonthName(mn - 3);
                gvResult.Columns[8].HeaderText = SlmScr053Biz.GetMonthName(mn - 2);
                gvResult.Columns[9].HeaderText = SlmScr053Biz.GetMonthName(mn - 1);
                gvResult.Columns[10].HeaderText = SlmScr053Biz.GetMonthName(mn);
                gvResult.Columns[11].HeaderText = SlmScr053Biz.GetMonthName(mn + 1);
                gvResult.Columns[12].HeaderText = SlmScr053Biz.GetMonthName(mn + 2);

                SlmScr053Biz bz = new SlmScr053Biz();
                var lst = SlmScr053Biz.GetSearchResult(SLMUtil.SafeInt(cmbYear.SelectedValue), SLMUtil.SafeInt(cmbMonth.SelectedValue), SLMUtil.SafeInt(cmbTelesalesTeam.SelectedValue), cmbTelesales.SelectedValue);
                BindGridview(pcTop, gvResult, lst.ToArray(), page);

            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(this, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
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
            //bool criteriaSelected = false;    //Comment By Pom 17/06/2559 - not used

            //if (cmbEndMonth.SelectedIndex == 0 || cmbMonth.SelectedIndex == 0 
            //    || cmbStartMonth.SelectedIndex == 0 || cmbYear.SelectedIndex == 0)
            //{
            //    AppUtil.ClientAlert(Page, "เลือกปีคุ้มครองเริ่มต้น, ปีคุ้มครองสิ้นสุด, เดือนคุ้มครองเริ่มต้น, เดือนคุ้มครองสิ้นสุด");
            //    return false;
            //}

            //int startyear, startmonth, endyear, endmonth;
            //int.TryParse(cmbYear.SelectedValue, out startyear);
            //int.TryParse(cmbStartMonth.SelectedValue, out startmonth);
            //int.TryParse(cmbMonth.SelectedValue, out endyear);
            //int.TryParse(cmbEndMonth.SelectedValue, out endmonth);
            //DateTime startDate = new DateTime(startyear, startmonth, 1);
            //DateTime endDate = new DateTime(endyear, endmonth, 1);
            //if (startDate.AddMonths(7) < endDate)
            //{
            //    AppUtil.ClientAlert(Page, "ไม่สามารถเลือกปีเดือนคุ้มครอง มากกว่า 7 เดือนได้");
            //    return false;
            //}
            if (cmbYear.SelectedIndex == 0 || cmbMonth.SelectedIndex == 0) { AppUtil.ClientAlert(this, "กรุณาเลือกปีที่แจกงาน, เดือนที่หมดอายุ กธ."); return false; }

            return true;
        }

        protected void gvResult_DataBound(object sender, EventArgs e)
        {
            if (gvResult.HeaderRow != null) gvResult.HeaderRow.Visible = false;
            var mn = SLMUtil.SafeInt(cmbMonth.SelectedValue);
            if (gvResult.Rows.Count == 0) return;
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
            TableHeaderCell cell = new TableHeaderCell();
            cell.Text = "Team Code";
            cell.RowSpan = 3;
            row.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.RowSpan = 3;
            cell.Text = "Telesales";
            row.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.ColumnSpan = 13;
            cell.Text = string.Format("Lead คุ้มครองเดือน {0}-{1}", SlmScr053Biz.GetMonthName(mn - 5), SlmScr053Biz.GetMonthName(mn + 2));
            row.Controls.Add(cell);

            gvResult.Controls[0].Controls.AddAt(0, row);

            row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
            cell = new TableHeaderCell();
            cell.Text = "Port ตั้งต้น";
            cell.RowSpan = 2;
            row.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.RowSpan = 2;
            cell.Text = "Port คงค้าง";
            row.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.RowSpan = 2;
            cell.Text = "Port ที่ไม่ต่อประกัน";
            row.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.ColumnSpan = 8;
            cell.Text = "เดือนที่ชำระประกัน";
            row.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.RowSpan = 2;
            cell.Text = "Grand Total";
            row.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.RowSpan = 2;
            cell.Text = "%Success";
            row.Controls.Add(cell);
            gvResult.Controls[0].Controls.AddAt(1, row);

            int month;
            int.TryParse(cmbMonth.SelectedValue, out month);
            row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
            for (int mnz = month-5; mnz <= month+2; mnz++)
            {
                cell = new TableHeaderCell();
                cell.Text = SlmScr053Biz.GetMonthName(mnz);
                row.Controls.Add(cell);
            }
            gvResult.Controls[0].Controls.AddAt(2, row);
        }

        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Visible = false;                
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            cmbYear.SelectedIndex = 0;
            cmbMonth.SelectedIndex = 0;
            cmbTelesalesTeam.SelectedIndex = 0;
            SetTelesaleName();

            gvResult.DataSource = null;
            gvResult.DataBind();

            gvResult.Visible = false;
            pcTop.Visible = false;

            upResult.Update();
            upSearch.Update();


        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidInput())
                {
                    return;
                }

                //DataSet resultDs;     //Coment by Pom 04/07/2016 - Warning variable is never used
                int teleTeam = 0;
                int.TryParse(cmbTelesalesTeam.SelectedValue, out teleTeam);                

                int year, month;
                int.TryParse(cmbYear.SelectedValue, out year);
                int.TryParse(cmbMonth.SelectedValue, out month);

                var filename = Path.GetTempPath() + "\\tmpexcel_053_" + Page.User.Identity.Name + ".xls";// Path.GetTempFileName();
                var bz = new SlmScr053Biz();

                if (bz.CreateExcelV2(filename, year, month, teleTeam, cmbTelesales.SelectedValue.ToString()))
                {
                    // สรุปผลการต่อประกันเทียบกับ Lead รายเดือน[กรกฎาคม]_[58]_
                    string destFilename = string.Format("สรุปผลการต่อประกันเทียบกับ_Lead_รายเดือน[{0}]_[{1}]_{2:yyyyMMdd_HHmmss}.xls", SlmScr053Biz.GetMonthName(month), year.ToString().Substring(2,2), DateTime.Now);
                    ExportExcel(filename, destFilename);
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