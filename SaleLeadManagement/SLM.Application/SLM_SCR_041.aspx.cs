using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.OleDb;
using SLM.Biz;
using SLM.Application.Shared;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Application.Utilities;
using log4net;
using System.Reflection;

namespace SLM.Application
{
    public partial class SLM_SCR_041 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_041));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //int yr = DateTime.Now.Year;
            //cmbYearStart.Items.Add(new ListItem("ทั้งหมด", "0"));
            //cmbYearEnd.Items.Add(new ListItem("ทั้งหมด", "0"));
            //for (int i = yr; i > yr-10 ;i--)
            //{
            //    cmbYearStart.Items.Add(new ListItem(i.ToString(), i.ToString()));
            //    cmbYearEnd.Items.Add(new ListItem(i.ToString(), i.ToString()));
            //}
            BuildCombo(cmbTeamCode, SlmScr032Biz.GetTeamSalesList(), "ทั้งหมด");
            BuildCombo(cmbMonthStart, new MonthBiz().GetMonthList(), "ทั้งหมด");
            BuildCombo(cmbMonthEnd, new MonthBiz().GetMonthList(), "ทั้งหมด");
            BuildCombo(cmbMonth, new MonthBiz().GetMonthList(), "ทั้งหมด");
            BuildCombo(cmbLevel, SlmScr041Biz.GetLevelData(), "ทั้งหมด");
            SetTTACombo();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_041");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    ((Label)Page.Master.FindControl("lblTopic")).Text = "นำเข้าข้อมูลผลงาน Telesales";
                    int yr = DateTime.Now.Year;
                    cmbYear.Items.Clear();
                    cmbYearStart.Items.Clear();
                    cmbYearEnd.Items.Clear();

                    cmbYear.Items.Add(new ListItem() { Text = "กรุณาระบุปี", Value = "0" });
                    cmbYearStart.Items.Add(new ListItem() { Text = "ทั้งหมด", Value = "0" });
                    cmbYearEnd.Items.Add(new ListItem() { Text = "ทั้งหมด", Value = "0" });

                    for (int i = yr; i > yr - 10; i--)
                    {
                        cmbYear.Items.Add(i.ToString());
                        cmbYearStart.Items.Add(i.ToString());
                        cmbYearEnd.Items.Add(i.ToString());
                    }

                    //disable all controls;
                    SetActiveInActiveSearchCriteria(true);
                    //Default Search
                    pnlResult.Visible = true;
                    doGetList(0);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void BuildCombo(DropDownList cmb, List<ControlListData> lst) { BuildCombo(cmb, lst, ""); }
        private void BuildCombo(DropDownList cmb, List<ControlListData> lst, string Blanktext)
        {
            cmb.Items.Clear();
            cmb.DataSource = lst;
            cmb.DataTextField = "TextField";
            cmb.DataValueField = "ValueField";
            cmb.DataBind();
            cmb.Items.Insert(0, new ListItem(Blanktext, ""));
        }
        private bool ValidateData(string val)
        {
            int err = 0;
            if ((val == "" || val == "yearup") && cmbYear.SelectedIndex == 0) { err++; lblvyear.Text = "กรุณาระบุปีนำเข้า"; } else { lblvyear.Text = ""; }
            if ((val == "" || val == "monup") && cmbMonth.SelectedIndex == 0) { err++; lblvmonth.Text = "กรุณาระบุเดือนนำเข้า"; } else { lblvmonth.Text = ""; }
            return err == 0;

        }



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            pnlResult.Visible = true;
            doGetList(0);
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllData();
            pnlResult.Visible = false;
            SetActiveInActiveSearchCriteria(true);
        }
        protected void btnImport_Click(object sender, EventArgs e)
        {
            cmbYear.SelectedIndex = 0;
            cmbMonth.SelectedIndex = 0;
            gvUploadResult.DataSource = null;
            gvUploadResult.DataBind();
            tbResult.Visible = false;
            pnlResult.Visible = false;
            pnlError.Visible = false;

            //lblResult.Text = "";

            popImport.Show();
        }
        protected void cmbTeamCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTTACombo();
        }


        private void doGetList(int page)
        {
            SlmScr041Biz bz = new SlmScr041Biz();
            var lst = bz.GetPerformanceList(cmbYearStart.SelectedValue
                , cmbMonthStart.SelectedValue
                , cmbYearEnd.SelectedValue
                , cmbMonthEnd.SelectedValue
                , cmbTeamCode.SelectedIndex == 0 ? "" : cmbTeamCode.SelectedItem.Text
                , cmbTTAName.SelectedValue
                , cmbLevel.SelectedValue, chkIsNew.Checked);
            BindData(lst, page);
        }
        private void BindData(object[] dt, int page)
        {
            pcTop.SetGridview(gvResult);
            pcTop.Update(dt, page);
            pcTop.GenerateRecordNumber(0, page);
            pcTop.Visible = gvResult.Rows.Count > 0;
        }
        protected void PageSearchChange(object sender, EventArgs e)
        {
            doGetList(((GridviewPageController)sender).SelectedPageIndex);
        }


        private void SetTTACombo()
        {
            BuildCombo(cmbTTAName, SlmScr032Biz.GetSearchStaffList(SLMUtil.SafeInt(cmbTeamCode.SelectedValue)), "ทั้งหมด");
        }

        protected void chkIsNew_CheckedChanged(object sender, EventArgs e)
        {
            if(chkIsNew.Checked)
            {
                ClearAllData();
            }
            SetActiveInActiveSearchCriteria(chkIsNew.Checked);
        }

        // upload popup
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            tbResult.Visible = false;
            pnlResult.Visible = false;
            pnlError.Visible = false;
            if (!ValidateData(""))
            {
                popImport.Show();
                return;
            }

            string[] monabb = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
            string moncheck = monabb[cmbMonth.SelectedIndex - 1];

            if (fuData.HasFile)
            {
                try
                {
                    var ext = Path.GetExtension(fuData.FileName).ToLower();
                    if (ext != ".xls")
                    {
                        throw new Exception("กรุณาระบุไฟล์ให้ถูกต้อง (.xls)");
                    }

                    using (OleDbConnection conn = new OleDbConnection())
                    {

                        DataTable dt = new DataTable();
                        string filename = Path.GetTempFileName();
                        fuData.SaveAs(filename);
                        lblFilename.Text = fuData.FileName;
                        fuData.ClearAllFilesFromPersistedStore();


                        // อ่านข้อมูล excel
                        conn.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=0;'", filename);
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM [Sheet1$]";
                        OleDbDataAdapter adp = new OleDbDataAdapter(cmd);
                        adp.Fill(dt);

                        // ตรวจสอบ column เดือน
                        if (dt.Columns[3].ColumnName.ToLower() != moncheck)
                            throw new Exception("ไม่พบข้อมูลสำหรับเดือนที่ระบุ");


                        // สร้าง data objects (ตัด row ที่มีค่าว่างทิ้ง)
                        List<SlmScr041Biz.ExcelData> lst = new List<SlmScr041Biz.ExcelData>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            if (row[0].ToString() == "" && row[1].ToString() == "") continue;

                            lst.Add(new SlmScr041Biz.ExcelData()
                            {
                                TeamCode = row[0].ToString(),
                                EmpCode = row[1].ToString(),
                                TAAName = row[2].ToString(),
                                Performance = SLMUtil.SafeDecimal(row[3].ToString()),
                                Level = row[4].ToString()
                            });
                        }

                        if (lst.Count == 0) throw new Exception("ไม่พบข้อมูลสำหรับนำเข้า");
                        int succ, fail;
                        List<ControlListData> rs;
                        tbResult.Visible = false;

                        // นำเข้าข้อมูล
                        SlmScr041Biz bz = new SlmScr041Biz();
                        if (!bz.ImportTelesaleData(lst, cmbYear.SelectedValue, cmbMonth.SelectedValue, Page.User.Identity.Name, out succ, out fail, out rs))
                            throw new Exception(bz.ErrorMessage);
                        else
                        {
                            if (rs.Count == 0)
                            {
                                pnlError.Visible = false;
                                AppUtil.ClientAlert(this, "นำเข้าข้อมูลเรียบร้อย");
                            }
                            else
                            {
                                pnlError.Visible = true;
                                gvUploadResult.DataSource = rs;
                                gvUploadResult.DataBind();
                                AppUtil.ClientAlert(this, "ข้อมูลไม่ถูกนำเข้า กรุณาตรวจสอบข้อผิดพลาด");
                            }

                            tbResult.Visible = true;
                            lblTotal.Text = (succ + fail).ToString("#,##0");
                            lblSucc.Text = succ.ToString("#,##0");
                            lblFail.Text = fail.ToString("#,##0");
                            //lblResult.Text = String.Format("<b>ผลการนำเข้าข้อมูล :</b> ทั้งหมด {0} รายการ, สำเร็จ {1} รายการ, ผิดพลาด {2} รายการ ", succ + fail, succ, fail);
                        }
                    }
                }
                catch (Exception ex)
                {
                    fuData.ClearAllFilesFromPersistedStore();
                    if(ex.Message.Contains("Sheet1"))
                    {
                        lblUploadError.Text = "ไม่สามารถ Import ข้อมูลได้เนื่องจากไม่พบ Sheet : 'Sheet1'";
                    }
                    else if (ex.Message.Contains("External table is not in the expected format."))
                    {
                        lblUploadError.Text = "ไม่สามารถ Import ข้อมูลได้เนื่องจากรูปแบบไฟล์ไม่ถูกต้อง";
                    }
                    else
                    {
                        lblUploadError.Text = ex.Message;
                    }                    
                    _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
            }
            else
            {
                lblUploadError.Text = "กรุณาระบุไฟล์ที่ต้องการนำเข้า";
            }
            popImport.Show();
        }

        protected void lbnClear_Click(object sender, EventArgs e)
        {
            fuData.ClearAllFilesFromPersistedStore();
            popImport.Show();
        }

        protected void btnExceleExport_Click(object sender, EventArgs e)
        {
            //Searc data again
            pnlResult.Visible = true;
            doGetList(0);

            //prepare data for export
            SlmScr041Biz bz = new SlmScr041Biz();
            dynamic lst = bz.GetPerformanceList(cmbYearStart.SelectedValue
                , cmbMonthStart.SelectedValue
                , cmbYearEnd.SelectedValue
                , cmbMonthEnd.SelectedValue
                , cmbTeamCode.SelectedIndex == 0 ? "" : cmbTeamCode.SelectedItem.Text
                , cmbTTAName.SelectedValue
                , cmbLevel.SelectedValue, chkIsNew.Checked);            
            
            Session["DatableExcelExport"] = ToDataTable(lst);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openexport", "window.open('./SLM_SCR_041_Export.aspx')", true);
        }

        public DataTable ToDataTable(dynamic items)
        {            
            DataTable table = new DataTable();
            int iRowCount = 1;
            if (items != null && items.Length > 0)
            {
                var infos = items[0].GetType().GetProperties();
                table.Columns.Add("No", typeof(string));
                foreach (var info in infos)
                {
                    DataColumn column = new DataColumn(info.Name, typeof(string));
                    table.Columns.Add(column);
                }
                foreach (var record in items)
                {
                    DataRow row = table.NewRow();
                    row["No"] = iRowCount;
                    for (int i = 1; i < table.Columns.Count; i++)
                    {
                        if((infos[i-1].PropertyType).FullName.IndexOf("Decimal") >= 0)
                        {
                            if(infos[i - 1].GetValue(record) != null && !string.IsNullOrWhiteSpace(infos[i - 1].GetValue(record).ToString()))
                            {
                                row[i] =  Convert.ToDecimal(infos[i - 1].GetValue(record)).ToString("#,##0.00");
                            }
                        }
                        else
                        {
                            row[i] = infos[i - 1].GetValue(record);
                        }                        
                    }
                    iRowCount ++; 
                    table.Rows.Add(row);
                }
               
                table.Columns.Remove("slm_Level");
                table.Columns.Remove("yearmonth");
                table.Columns.Remove("slm_Month");
            }

            return table;
        }        

        private void ClearAllData()
        {            
            cmbYearStart.SelectedIndex = 0;
            cmbMonthStart.SelectedIndex = 0;
            cmbYearEnd.SelectedIndex = 0;
            cmbMonthEnd.SelectedIndex = 0;
            cmbTeamCode.SelectedIndex = 0;
            SetTTACombo();
            cmbTTAName.SelectedIndex = 0;
            cmbLevel.SelectedIndex = 0;            
            chkIsNew.Checked = true;            
        }

        private void SetActiveInActiveSearchCriteria(bool IsActive)
        {
            cmbYearStart.Enabled = !IsActive;
            cmbMonthStart.Enabled = !IsActive;
            cmbYearEnd.Enabled = !IsActive;
            cmbMonthEnd.Enabled = !IsActive;
            cmbTeamCode.Enabled = !IsActive;
            cmbTTAName.Enabled = !IsActive;
            cmbLevel.Enabled = !IsActive;
        }
    }
}