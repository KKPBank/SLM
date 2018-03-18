using log4net;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Excel;
using SLM.Resource;

namespace SLM.Application
{
    public partial class SLM_SCR_032 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_032));

        private string ExcelColumnName { get; set; }
        enum ExcelColName
        {
            TeamTelesales = 0,
            EmpCode = 1,
            PreleadId = 10,
            Lot = 11
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ตรวจสอบข้อมูลการจ่ายงาน";
            Page.Form.DefaultButton = btnSearch.UniqueID;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_032");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    int lot = 0;
                    int.TryParse(Request["lot"], out lot);

                    initControl();
                    
                    if (lot <= 0)
                    {
                        tabMain.Visible = false;
                        upNoLot.Visible = true;
                        pcNoLot_PageChange(null, null);
                        btnReject.Visible = false;
                        btnSavePopupChangeOwner.Visible = false;
                        btnExportExcel.Visible = false;
                        btnConfirm.Visible = false;
                    }
                    else
                    {
                        SlmScr032Biz biz = new SlmScr032Biz();
                        if (!biz.CheckAccess(lot))
                        {
                            AppUtil.ClientAlertAndRedirect(Page, biz.ErrorMessage, "SLM_SCR_047.aspx");
                            return;
                        }

                        tabMain.Visible = true;
                        upNoLot.Visible = false;
                    }
                    pcSuccess_PageChange(null, null);
                    pcDedup_PageChange(null, null);
                    pcBlacklist_PageChange(null, null);
                    pcExceptional_PageChange(null, null);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void initControl()
        {
            var cardType = CardTypeBiz.GetCardTypeList();
            cardType.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
            cmbCardType.DataSource = cardType;
            cmbCardType.DataBind();

            //var campaign = CampaignBiz.GetCampaignList("01");
			var campaign = CampaignBiz.GetCampaignListNew("01");
            campaign.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
            cmbCampaign.DataSource = campaign;
            cmbCampaign.DataBind();

            var saleTeam = SlmScr032Biz.GetTeamSalesList();
            saleTeam.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
            cmbTelesalesTeam.DataSource = saleTeam;
            cmbTelesalesTeam.DataBind();

            txtFirstname.Text = "";
            txtLastname.Text = "";
            txtCitizenId.Text = "";
            tdmTransferDate.DateValue = DateTime.MinValue;
        }
        protected void cmbTelesalesTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                List<ControlListData> staff = new List<ControlListData>();
                int saleTeamId = 0;
                if (cmbTelesalesTeam.SelectedValue != "-1" && int.TryParse(cmbTelesalesTeam.SelectedValue, out saleTeamId))
                {
                    staff = SlmScr032Biz.GetSearchStaffList(saleTeamId);
                }
                staff.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
                cmbOwnerSearch.DataSource = staff;
                cmbOwnerSearch.DataBind();
                upOwnerSearch.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                int lot = 0;
                int.TryParse(Request["lot"], out lot);

                DedupCheckedList = null;
                BlacklistCheckedList = null;

                if (lot > 0)
                {
                    pcBlacklist.UpdateMonitoring((new List<SlmScr032SearchResult>()).ToArray(), 0);
                    pcDedup.UpdateMonitoring((new List<SlmScr032SearchResult>()).ToArray(), 0);
                    pcSuccess.UpdateMonitoring((new List<SlmScr032SearchResult>()).ToArray(), 0);
                    pcExceptional.UpdateMonitoring((new List<SlmScr032SearchResult>()).ToArray(), 0);

                    pcBlacklist_PageChange(null, null);
                    pcDedup_PageChange(null, null);
                    pcExceptional_PageChange(null, null);
                    pcSuccess_PageChange(null, null);

                    upBlacklist.Update();
                    upDedup.Update();
                    upExceptional.Update();
                    upSuccess.Update();

                    upSearch.Update();
                    //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "enableConfirmButton", "$('#ContentPlaceHolder1_btnConfirm')[0].disabled = true;");
                    upTabMain.Update();
                }
                else {
                    pcNoLot.UpdateMonitoring((new List<SlmScr032SearchResult>()).ToArray(), 0);
                    pcNoLot_PageChange(null, null);
                    upNoLot.Update();                    
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }     

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtFirstname.Text = "";
            txtLastname.Text = "";
            txtCitizenId.Text = "";
            tdmTransferDate.DateValue = new DateTime();
            cmbCampaign.SelectedValue = "-1";
            cmbCardType.SelectedValue = "-1";
            cmbTelesalesTeam.SelectedValue = "-1";
            cmbTelesalesTeam_SelectedIndexChanged(null, null);
            cmbRecordPerPage.SelectedIndex = 0;
            upSearch.Update();
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, GridView gv, object[] items, int pageIndex)
        {
            int recPerPage = int.Parse(cmbRecordPerPage.SelectedValue);
            pageControl.SetGridview(gv);            
            pageControl.Update(items, pageIndex, recPerPage);            
        }
        protected void pcSuccess_PageChange(object sender, EventArgs e)
        {
            try
            {
                DataSet resultDs;
                int lot = 0;
                int.TryParse(Request["lot"], out lot);
                SlmScr032Biz biz = new SlmScr032Biz();
                var successResult = biz.GetSuccessList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, cmbTelesalesTeam.SelectedValue, cmbOwnerSearch.SelectedValue, out resultDs );
                BindGridview(pcSuccess, gvSuccess, successResult.ToArray(), pcSuccess.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void pcDedup_PageChange(object sender, EventArgs e)
        {
            try
            {
                //sender = null มาจากการกดปุ่ม Search, PageLoad
                if (sender != null)
                {
                    SaveCheckedDedup();
                }

                DataSet resultDs;
                int lot = 0;
                int.TryParse(Request["lot"], out lot);
                SlmScr032Biz biz = new SlmScr032Biz();
                var dedupResult = biz.GetDedubList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, out resultDs);
                BindGridview(pcDedup, gvDedup, dedupResult.ToArray(), pcDedup.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        public List<KeyValueData> DedupCheckedList
        {
            get
            {
                if (ViewState["DedupCheckedList"] == null)
                {
                    ViewState["DedupCheckedList"] = new List<KeyValueData>();
                }
                return (List<KeyValueData>)ViewState["DedupCheckedList"];
            }
            set
            {
                ViewState["DedupCheckedList"] = value;
            }
        }

        private void SaveCheckedDedup()
        {
            try
            {
                var checkedList = DedupCheckedList;

                foreach (GridViewRow row in gvDedup.Rows)
                {
                    string tempId = ((HiddenField)row.FindControl("hiddenTempId")).Value;
                    CheckBox checkDedup = (CheckBox)row.FindControl("checkDedup");

                    var record = checkedList.Where(p => p.Key == tempId).FirstOrDefault();
                    if (record != null)
                    {
                        record.Value = checkDedup.Checked;

                        if (!checkDedup.Checked)
                            checkedList.Remove(record);
                    }
                    else
                    {
                        if (checkDedup.Checked)
                        {
                            checkedList.Add(new KeyValueData() { Key = tempId, Value = true });
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        protected void gvDedup_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var checkedList = DedupCheckedList;
                    string tempId = ((HiddenField)e.Row.FindControl("hiddenTempId")).Value;

                    var record = checkedList.Where(p => p.Key == tempId).FirstOrDefault();
                    if (record != null)
                    {
                        ((CheckBox)e.Row.FindControl("checkDedup")).Checked = record.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        protected void pcBlacklist_PageChange(object sender, EventArgs e)
        {
            try
            {
                //sender = null มาจากการกดปุ่ม Search, PageLoad
                if (sender != null)
                {
                    SaveCheckedBlacklist();
                }

                DataSet resultDs;
                int lot = 0;
                int.TryParse(Request["lot"], out lot);
                SlmScr032Biz biz = new SlmScr032Biz();
                var blackListResult = biz.GetBlackListList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, out resultDs);
                BindGridview(pcBlacklist, gvBlacklist, blackListResult.ToArray(), pcBlacklist.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        public List<KeyValueData> BlacklistCheckedList
        {
            get
            {
                if (ViewState["BlacklistCheckedList"] == null)
                {
                    ViewState["BlacklistCheckedList"] = new List<KeyValueData>();
                }
                return (List<KeyValueData>)ViewState["BlacklistCheckedList"];
            }
            set
            {
                ViewState["BlacklistCheckedList"] = value;
            }
        }

        private void SaveCheckedBlacklist()
        {
            try
            {
                var checkedList = BlacklistCheckedList;

                foreach (GridViewRow row in gvBlacklist.Rows)
                {
                    string tempId = ((HiddenField)row.FindControl("hiddenTempId")).Value;
                    CheckBox checkBlacklist = (CheckBox)row.FindControl("checkBlacklist");

                    var record = checkedList.Where(p => p.Key == tempId).FirstOrDefault();
                    if (record != null)
                    {
                        record.Value = checkBlacklist.Checked;

                        if (!checkBlacklist.Checked)
                            checkedList.Remove(record);
                    }
                    else
                    {
                        if (checkBlacklist.Checked)
                        {
                            checkedList.Add(new KeyValueData() { Key = tempId, Value = true });
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        protected void gvBlacklist_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var checkedList = BlacklistCheckedList;
                    string tempId = ((HiddenField)e.Row.FindControl("hiddenTempId")).Value;

                    var record = checkedList.Where(p => p.Key == tempId).FirstOrDefault();
                    if (record != null)
                    {
                        ((CheckBox)e.Row.FindControl("checkBlacklist")).Checked = record.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        protected void pcExceptional_PageChange(object sender, EventArgs e)
        {
            try
            {
                DataSet resultDs;
                int lot = 0;
                int.TryParse(Request["lot"], out lot);
                SlmScr032Biz biz = new SlmScr032Biz();
                var exceptionalResult = biz.GetExceptionalList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, out resultDs);
                BindGridview(pcExceptional, gvExceptional, exceptionalResult.ToArray(), pcExceptional.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void pcNoLot_PageChange(object sender, EventArgs e)
        {
            try
            {
                DataSet resultDs;
                int lot = 0;
                int.TryParse(Request["lot"], out lot);
                SlmScr032Biz biz = new SlmScr032Biz();
                var noLotResult = biz.GetNoLotList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, cmbTelesalesTeam.SelectedValue, cmbOwnerSearch.SelectedValue, out resultDs);
                BindGridview(pcNoLot, gvNoLot, noLotResult.ToArray(), pcNoLot.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        #endregion

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet resultDs;
                int lot = 0;
                int.TryParse(Request["lot"], out lot);
                SlmScr032Biz biz = new SlmScr032Biz();
                string date = DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMddHHmmss");
                string filename = "";
                string outputFilename = "";
                bool doExport = false;

                switch (tabMain.ActiveTabIndex)
                {
                    case 0: // tab success
                        var successList = biz.GetSuccessList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, cmbTelesalesTeam.SelectedValue, cmbOwnerSearch.SelectedValue, out resultDs);
                        //if (resultDs.Tables[0].Rows.Count > 0)
                        if (successList.Count > 0)
                        {
                            filename = Path.Combine(Path.GetTempPath(), Page.User.Identity.Name + "_" + date + ".xls");
                            biz.CreateExcel(successList, filename, "success", "jobDelegateSuccessTab");
                            outputFilename = "jobDelegateSuccessTab.xls";
                            doExport = true;
                        }
                        break;
                    case 1: // tab dedup
                        var dedubList = biz.GetDedubList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, out resultDs);
                        if (dedubList.Count > 0)
                        {
                            filename = Path.Combine(Path.GetTempPath(), Page.User.Identity.Name + "_" + date + ".xls");
                            biz.CreateExcel(dedubList, filename, "dedup", "jobDelegateDedupTab");
                            outputFilename = "jobDelegateDedupTab.xls";
                            doExport = true;
                        }
                        break;
                    case 2: // tab blacklist
                        var blacklistList = biz.GetBlackListList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, out resultDs);
                        if (blacklistList.Count > 0)
                        {
                            filename = Path.Combine(Path.GetTempPath(), Page.User.Identity.Name + "_" + date + ".xls");
                            biz.CreateExcel(blacklistList, filename, "blacklist", "jobDelegateBlacklistTab");
                            outputFilename = "jobDelegateBlacklistTab.xls";
                            doExport = true;
                        }
                        break;
                    case 3: break; // not allow to export tab Exceptional
                    default: break;
                }

                if (doExport)
                {
                    Session["excelfilepath"] = filename;
                    Session["outputfilename"] = outputFilename;

                    string script = "window.open('SLM_SCR_045.aspx', 'exporttab', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                    ScriptManager.RegisterStartupScript(Page, GetType(), "exporttab", script, true);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void confirm()
        {
            DataSet resultDs;            
            int lot = 0;
            int.TryParse(Request["lot"], out lot);
            SlmScr032Biz biz = new SlmScr032Biz();

            List<SlmScr032SearchResult> successList = biz.GetSuccessList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, cmbTelesalesTeam.SelectedValue, cmbOwnerSearch.SelectedValue, out resultDs);            

            List<SlmScr032SearchResult> dedupList = biz.GetDedubList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, out resultDs);            

            List<SlmScr032SearchResult> blacklistList = biz.GetBlackListList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, out resultDs);
            
            List<SlmScr032SearchResult> exceptionalList = biz.GetExceptionalList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, out resultDs);


            if (biz.UpdateDelegateList(lot, successList, dedupList, blacklistList, exceptionalList, HttpContext.Current.User.Identity.Name.ToLower()))
            {
                AppUtil.ClientAlertAndRedirect(this, "Confirm ข้อมูลการจ่ายงานแล้ว", ResolveUrl("~/SLM_SCR_047.aspx"));
            }
            else
            {
                AppUtil.ClientAlert(this, biz.ErrorMessage);
            }
        }

        private void reject()
        {
            int lot = 0;
            int.TryParse(Request["lot"], out lot);
            SlmScr032Biz biz = new SlmScr032Biz();

            List<SlmScr032SearchResult> successList = null;// ไม่ใช้แล้ว biz.GetSuccessList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, cmbTelesalesTeam.SelectedValue, cmbOwnerSearch.SelectedValue, out resultDs);

            //Dedup==================================================================
            SaveCheckedDedup();

            List<decimal> dedupTempId = new List<decimal>();
            var dedup_checkedlist = DedupCheckedList;
            dedup_checkedlist.ForEach(p => {
                if (p.Value)
                {
                    dedupTempId.Add(decimal.Parse(p.Key));
                }
            });

            //Blacklist==================================================================
            SaveCheckedBlacklist();

            List<decimal> blacklistTempId = new List<decimal>();
            var bl_checkedlist = BlacklistCheckedList;
            bl_checkedlist.ForEach(p => {
                if (p.Value)
                {
                    blacklistTempId.Add(decimal.Parse(p.Key));
                }
            });

            List<SlmScr032SearchResult> exceptionalList = null;  //ไม่ใช่แล้ว biz.GetExceptionalList(lot, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCardType.SelectedValue, txtCitizenId.Text.Trim(), cmbCampaign.SelectedValue, tdmTransferDate.DateValue, out resultDs);

            if (biz.RejectDelegateList(lot, successList, dedupTempId, blacklistTempId, exceptionalList, HttpContext.Current.User.Identity.Name.ToLower()))
            {
                AppUtil.ClientAlertAndRedirect(this, "Reject ข้อมูลการจ่ายงานแล้ว", ResolveUrl("~/SLM_SCR_047.aspx"));
            }
            else
            {
                AppUtil.ClientAlert(this, biz.ErrorMessage);
            }
        }

        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (cmbRecordPerPage.SelectedItem.Text == "50")
                {
                    if (((Label)e.Row.FindControl("lblNo")).Text == "3" || ((Label)e.Row.FindControl("lblNo")).Text == "6")
                    {
                        e.Row.BackColor = System.Drawing.Color.FromArgb(249, 204, 204);
                        e.Row.BorderStyle = BorderStyle.Solid;
                        e.Row.BorderWidth = new Unit(1, UnitType.Pixel);
                        e.Row.BorderColor = System.Drawing.Color.FromArgb(249, 162, 162);
                    }
                }
            }
        }

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            mpePopupChangeOwner.Show();
        }

        protected void btnImportExcel_Click(object sender, EventArgs e)
        {
            gvUploadError.Visible = false;
            mpePopupImportExcel.Show();
        }

        //protected void btnPopupDoImportExcel_Click(object sender, EventArgs e)
        //{
        //    if (fuData.HasFile)
        //    {
        //        var ext = Path.GetExtension(fuData.FileName).ToLower();
        //        if (ext != ".xls")
        //        {
        //            lblUploadError.Text = "กรุณาระบุไฟล์ให้ถูก format (xls)";
        //            return;
        //        }

        //        try
        //        {
        //            using (OleDbConnection conn = new OleDbConnection())
        //            {
        //                DataTable dt = new DataTable();
        //                string filename = Path.GetTempFileName();
        //                fuData.SaveAs(filename);
        //                //conn.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=0;'", filename);
        //                conn.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1;'", filename);

        //                conn.Open();
        //                DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //                string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();

        //                OleDbCommand cmd = new OleDbCommand();
        //                cmd.Connection = conn;
        //                cmd.CommandType = CommandType.Text;                        
        //                cmd.CommandText = $"SELECT * FROM [{firstSheetName}]";

        //                using (OleDbDataAdapter adp = new OleDbDataAdapter(cmd))
        //                {
        //                    adp.Fill(dt);
        //                }

        //                List<SlmScr032SearchResult> data;
        //                SlmScr032Biz biz = new SlmScr032Biz();
        //                biz.RaiseLog += (str) =>
        //                {
        //                    if (!string.IsNullOrEmpty(str))
        //                        _log.Debug(str);
        //                };

        //                _log.Debug("Start Verify 032");
        //                if (biz.ValidateImportData(dt, out data))
        //                {
        //                    _log.Debug("Finish Verify 032");
        //                    if (tabMain.Visible)
        //                    {
        //                        decimal lot = 0;
        //                        decimal.TryParse(Request["lot"], out lot);

        //                        _log.Debug("Start Import 032");
        //                        biz.DoImport(data, HttpContext.Current.User.Identity.Name.ToLower(), lot);
        //                    }
        //                    else
        //                    {
        //                        _log.Debug("Start Do No Lot Import 032");
        //                        biz.DoNoLotImport(data, HttpContext.Current.User.Identity.Name.ToLower());
        //                    }
        //                    _log.Debug("Finish Import 032");

        //                    AppUtil.ClientAlertAndRedirect(this, "Import ข้อมูลเรียบร้อย", Request.Url.AbsoluteUri);                            
        //                }                        
        //                else
        //                {
        //                    gvUploadError.DataSource = data.Where(p => p.ErrorMessage != null && p.ErrorMessage != string.Empty).ToList();
        //                    gvUploadError.DataBind();
        //                    gvUploadError.Visible = true;
        //                    mpePopupImportExcel.Show();
        //                    upPopupImportExcel.Update();
        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //            _log.Error(message);
        //            AppUtil.ClientAlert(Page, message);
        //        }
        //    }
        //    else
        //    {
        //        mpePopupImportExcel.Show();
        //    }

        //}

        protected void btnPopupDoImportExcel_Click(object sender, EventArgs e)
        {
            if (fuData.HasFile)
            {
                var ext = Path.GetExtension(fuData.FileName).ToLower();
                if (ext != ".xls")
                {
                    lblUploadError.Text = "กรุณาระบุไฟล์ให้ถูก format (xls)";
                    return;
                }

                try
                {
                    var excelData = ReadExcel();

                    List<SlmScr032SearchResult> data;
                    SlmScr032Biz biz = new SlmScr032Biz();
                    biz.RaiseLog += (str) =>
                    {
                        if (!string.IsNullOrEmpty(str))
                            _log.Debug(str);
                    };

                    _log.Debug("Start Verify 032");
                    if (biz.ValidateImportData(excelData, ExcelColumnName, out data))
                    {
                        _log.Debug("Finish Verify 032");
                        if (tabMain.Visible)
                        {
                            decimal lot = 0;
                            decimal.TryParse(Request["lot"], out lot);

                            _log.Debug("Start Import 032");
                            biz.DoImport(data, HttpContext.Current.User.Identity.Name.ToLower(), lot);
                        }
                        else
                        {
                            _log.Debug("Start Do No Lot Import 032");
                            biz.DoNoLotImport(data, HttpContext.Current.User.Identity.Name.ToLower());
                        }
                        _log.Debug("Finish Import 032");

                        AppUtil.ClientAlertAndRedirect(this, "Import ข้อมูลเรียบร้อย", Request.Url.AbsoluteUri);
                    }
                    else
                    {
                        gvUploadError.DataSource = data.Where(p => p.ErrorMessage != null && p.ErrorMessage != string.Empty).ToList();
                        gvUploadError.DataBind();
                        gvUploadError.Visible = true;
                        mpePopupImportExcel.Show();
                        upPopupImportExcel.Update();
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _log.Error(message);
                    AppUtil.ClientAlert(Page, message);
                }
            }
            else
            {
                mpePopupImportExcel.Show();
            }

        }

        private List<SlmScr032ExcelData> ReadExcel()
        {
            IExcelDataReader excelReader = null;
            try
            {
                string ext = System.IO.Path.GetExtension(fuData.PostedFile.FileName);
                excelReader = ext == ".xls" ? ExcelReaderFactory.CreateBinaryReader(fuData.FileContent) : ExcelReaderFactory.CreateOpenXmlReader(fuData.FileContent);
                excelReader.IsFirstRowAsColumnNames = true;

                bool isHeader = true;
                var dataList = new List<SlmScr032ExcelData>();

                while (excelReader.Read())
                {
                    if (isHeader)
                    {
                        ExcelColumnName = excelReader.GetString((int)ExcelColName.PreleadId) != null ? excelReader.GetString((int)ExcelColName.PreleadId).Trim() : "";
                        isHeader = false;
                        continue;
                    }

                    SlmScr032ExcelData data = new SlmScr032ExcelData
                    {
                        TeamTelesales = excelReader.GetString((int)ExcelColName.TeamTelesales) != null ? excelReader.GetString((int)ExcelColName.TeamTelesales).Trim() : "",
                        EmpCode = excelReader.GetString((int)ExcelColName.EmpCode) != null ? excelReader.GetString((int)ExcelColName.EmpCode).Trim() : "",
                        PreleadId = excelReader.GetString((int)ExcelColName.PreleadId) != null ? excelReader.GetString((int)ExcelColName.PreleadId).Trim() : "",
                        Lot = excelReader.GetString((int)ExcelColName.Lot) != null ? excelReader.GetString((int)ExcelColName.Lot).Trim() : ""
                    };
                    dataList.Add(data);
                }

                return dataList;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (excelReader != null)
                {
                    excelReader.Close();
                    excelReader.Dispose();
                }
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            lblReject.Visible = true;
            lblConfirm.Visible = false;
            mpePopupConfirmReject.Show();
            upPopupConfirmReject.Update();
        }
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            lblReject.Visible = false;
            lblConfirm.Visible = true;
            mpePopupConfirmReject.Show();
            upPopupConfirmReject.Update();
        }
        protected void btnConfirm2_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblReject.Visible)
                {
                    reject();
                }
                else
                {
                    confirm();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnReject2_Click(object sender, EventArgs e)
        {
            mpePopupConfirmReject.Hide();
            upPopupConfirmReject.Update();
        }

    }

    [Serializable]
    public class KeyValueData
    {
        public string Key { get; set; }
        public bool Value { get; set; }
    }

}