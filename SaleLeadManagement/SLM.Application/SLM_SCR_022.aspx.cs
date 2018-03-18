using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_022 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_022));
        private string slaDefault = "DEFAULT";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาข้อมูล SLA";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_022");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    Page.Form.DefaultButton = btnSearch.UniqueID;
                    InitialControl();
                    DoSearchConfigSla(0);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void InitialControl()
        {
            AppUtil.SetIntTextBox(txtSlaMinPopup);
            AppUtil.SetIntTextBox(txtSlaTimePopup);
            AppUtil.SetIntTextBox(txtSlaDayPopup);

            txtSlaMinPopup.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
            txtSlaTimePopup.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
            txtSlaDayPopup.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");

            //Search
//            cmbProductSearch.DataSource = ProductBiz.GetProductList();
            cmbProductSearch.DataSource = ProductBiz.GetProductListNew();
            cmbProductSearch.DataTextField = "TextField";
            cmbProductSearch.DataValueField = "ValueField";
            cmbProductSearch.DataBind();
            cmbProductSearch.Items.Insert(0, new ListItem("", ""));
            cmbProductSearch.Items.Insert(1, new ListItem("DEFAULT", "DEFAULT"));

            //cmbCampaignSearch.DataSource = CampaignBiz.GetCampaignList("");
			cmbCampaignSearch.DataSource = CampaignBiz.GetCampaignListNew("");
            cmbCampaignSearch.DataTextField = "TextField";
            cmbCampaignSearch.DataValueField = "ValueField";
            cmbCampaignSearch.DataBind();
            cmbCampaignSearch.Items.Insert(0, new ListItem("", ""));
            cmbCampaignSearch.Items.Insert(1, new ListItem("DEFAULT", "DEFAULT"));

            cmbChannelSearch.DataSource = ChannelBiz.GetChannelList();
            cmbChannelSearch.DataTextField = "TextField";
            cmbChannelSearch.DataValueField = "ValueField";
            cmbChannelSearch.DataBind();
            cmbChannelSearch.Items.Insert(0, new ListItem("", ""));

            cmbStatusSearch.DataSource = OptionBiz.GetOptionList(AppConstant.OptionType.LeadStatus);
            cmbStatusSearch.DataTextField = "TextField";
            cmbStatusSearch.DataValueField = "ValueField";
            cmbStatusSearch.DataBind();
            cmbStatusSearch.Items.Insert(0, new ListItem("", ""));

            //Popup
            BindPopupProductCampaignCombo("load");

            cmbChannelPopup.DataSource = ChannelBiz.GetChannelList();
            cmbChannelPopup.DataTextField = "TextField";
            cmbChannelPopup.DataValueField = "ValueField";
            cmbChannelPopup.DataBind();
            cmbChannelPopup.Items.Insert(0, new ListItem("", ""));

            cmbStatusPopup.DataSource = OptionBiz.GetOptionList(AppConstant.OptionType.LeadStatus);
            cmbStatusPopup.DataTextField = "TextField";
            cmbStatusPopup.DataValueField = "ValueField";
            cmbStatusPopup.DataBind();
            cmbStatusPopup.Items.Insert(0, new ListItem("", ""));
        }

        private void BindPopupProductCampaignCombo(string mode)
        {
            if (mode == "edit")
            {
                cmbProductPopup.DataSource = ProductBiz.GetProductList();
                cmbCampaignPopup.DataSource = CampaignBiz.GetCampaignList("");
            }
            else
            {
                cmbProductPopup.DataSource = ProductBiz.GetProductListNew();
                cmbCampaignPopup.DataSource = CampaignBiz.GetCampaignListNew("");
            }

            cmbProductPopup.DataTextField = "TextField";
            cmbProductPopup.DataValueField = "ValueField";
            cmbProductPopup.DataBind();
            cmbProductPopup.Items.Insert(0, new ListItem("", ""));
            
            //cmbProductPopup.Items.Insert(1, new ListItem("DEFAULT", "DEFAULT"));
            cmbCampaignPopup.DataTextField = "TextField";
            cmbCampaignPopup.DataValueField = "ValueField";
            cmbCampaignPopup.DataBind();
            cmbCampaignPopup.Items.Insert(0, new ListItem("", ""));
            //cmbCampaignPopup.Items.Insert(1, new ListItem("DEFAULT", "DEFAULT"));
        }

        protected void rbProductSearch_CheckedChanged(object sender, EventArgs e)
        {
            CheckSearchRadioCondition();
        }

        protected void rbCampaignSearch_CheckedChanged(object sender, EventArgs e)
        {
            CheckSearchRadioCondition();
        }

        private void CheckSearchRadioCondition()
        {
            if (rbProductSearch.Checked)
            {
                cmbProductSearch.Enabled = true;
                cmbCampaignSearch.SelectedIndex = -1;
                cmbCampaignSearch.Enabled = false;
            }
            else
            {
                cmbProductSearch.SelectedIndex = -1;
                cmbProductSearch.Enabled = false;
                cmbCampaignSearch.Enabled = true;
            }
        }

        protected void rbProductPopup_CheckedChanged(object sender, EventArgs e)
        {
            CheckSearchRadioConditionPopup();
            mpePopup.Show();
        }

        protected void rbCampaignPopup_CheckedChanged(object sender, EventArgs e)
        {
            CheckSearchRadioConditionPopup();
            mpePopup.Show();
        }

        private void CheckSearchRadioConditionPopup()
        {
            if (rbProductPopup.Checked)
            {
                cmbProductPopup.Enabled = true;
                cmbCampaignPopup.SelectedIndex = -1;
                cmbCampaignPopup.Enabled = false;
                lblProductStar.Visible = true;
                lblCampaignStar.Visible = false;
            }
            else
            {
                cmbProductPopup.SelectedIndex = -1;
                cmbProductPopup.Enabled = false;
                cmbCampaignPopup.Enabled = true;
                lblProductStar.Visible = false;
                lblCampaignStar.Visible = true;
            }
        }

        private void EditsearchRadioConditionPopup()
        {
            if (rbProductPopup.Checked)
            {
                cmbCampaignPopup.SelectedIndex = -1;
                cmbProductPopup.Enabled = false;
                cmbCampaignPopup.Enabled = false;
                lblProductStar.Visible = true;
                lblCampaignStar.Visible = false;
            }
            else
            {
                cmbProductPopup.SelectedIndex = -1;
                cmbProductPopup.Enabled = false;
                cmbCampaignPopup.Enabled = false;
                lblProductStar.Visible = false;
                lblCampaignStar.Visible = true;
            }

            rbProductPopup.Enabled = false;
            rbCampaignPopup.Enabled = false;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchConfigSla(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchConfigSla(int pageIndex)
        {
            try
            {
                List<SlaData> list = SlaBiz.SearchConfigSla(cmbProductSearch.SelectedItem.Value, cmbCampaignSearch.SelectedItem.Value, cmbChannelSearch.SelectedItem.Value, cmbStatusSearch.SelectedItem.Value);
                BindGridview(pcTop, list.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex);
            pageControl.GenerateRecordNumber(1, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchConfigSla(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        #region Popup

        protected void btnAddSla_Click(object sender, EventArgs e)
        {
            try
            {
                BindPopupProductCampaignCombo("add");
                txtSlaId.Text = "";
                rbProductPopup.Enabled = true;
                rbProductPopup.Checked = true;
                rbCampaignPopup.Enabled = true;
                rbCampaignPopup.Checked = false;
                cmbProductPopup.SelectedIndex = -1;
                cmbProductPopup.Enabled = true;
                cmbCampaignPopup.SelectedIndex = -1;
                cmbCampaignPopup.Enabled = false;
                cmbChannelPopup.Enabled = true;
                cmbChannelPopup.SelectedIndex = -1;
                cmbStatusPopup.Enabled = true;
                cmbStatusPopup.SelectedIndex = -1;
                cmbSubStatusPopup.Enabled = true;
                cmbSubStatusPopup.SelectedIndex = -1;
                txtSlaMinPopup.Text = "";
                txtSlaTimePopup.Text = "";
                txtSlaDayPopup.Text = "";
                txtSlaMinPopup.Enabled = true;
                txtSlaTimePopup.Enabled = true;
                txtSlaDayPopup.Enabled = true;
                lblProductStar.Visible = true;
                lblCampaignStar.Visible = false;

                upPopup.Update();
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    if (txtSlaId.Text.Trim() != "")
                        SlaBiz.UpdateData(int.Parse(txtSlaId.Text.Trim()), cmbProductPopup.SelectedItem.Value, cmbCampaignPopup.SelectedItem.Value, cmbChannelPopup.SelectedItem.Value, cmbStatusPopup.SelectedItem.Value, cmbSubStatusPopup.SelectedItem.Value, int.Parse(txtSlaMinPopup.Text.Trim()), int.Parse(txtSlaTimePopup.Text.Trim()), int.Parse(txtSlaDayPopup.Text.Trim()), HttpContext.Current.User.Identity.Name);
                    else
                        SlaBiz.InsertData(cmbProductPopup.SelectedItem.Value, cmbCampaignPopup.SelectedItem.Value, cmbChannelPopup.SelectedItem.Value, cmbStatusPopup.SelectedItem.Value, cmbSubStatusPopup.SelectedItem.Value, int.Parse(txtSlaMinPopup.Text.Trim()), int.Parse(txtSlaTimePopup.Text.Trim()), int.Parse(txtSlaDayPopup.Text.Trim()), HttpContext.Current.User.Identity.Name);

                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                    ClearPopupControl();
                    mpePopup.Hide();

                    DoSearchConfigSla(0);
                }
                else
                    mpePopup.Show();
            }
            catch (Exception ex)
            {
                mpePopup.Show();
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(((ImageButton)sender).CommandArgument);
                txtSlaId.Text = ((Label)gvResult.Rows[index].FindControl("lblSlaId")).Text.Trim();
                string productId = ((Label)gvResult.Rows[index].FindControl("lblProductId")).Text.Trim();
                string campaignId = ((Label)gvResult.Rows[index].FindControl("lblCampaignId")).Text.Trim();
                string channelId = ((Label)gvResult.Rows[index].FindControl("lblChannelId")).Text.Trim();
                string statusCode = ((Label)gvResult.Rows[index].FindControl("lblStatusCode")).Text.Trim();
                string subStatusCode = ((Label)gvResult.Rows[index].FindControl("lblSubStatusCode")).Text.Trim();

                BindPopupProductCampaignCombo("edit");

                if (productId == slaDefault || campaignId == slaDefault)
                {
                    if (productId == slaDefault)
                        cmbProductPopup.Items.Insert(1, new ListItem("DEFAULT", "DEFAULT"));

                    if (campaignId == slaDefault)
                        cmbCampaignPopup.Items.Insert(1, new ListItem("DEFAULT", "DEFAULT"));

                    cmbProductPopup.SelectedIndex = cmbProductPopup.Items.IndexOf(cmbProductPopup.Items.FindByValue(productId));
                    cmbCampaignPopup.SelectedIndex = cmbCampaignPopup.Items.IndexOf(cmbCampaignPopup.Items.FindByValue(campaignId));
                    rbProductPopup.Enabled = false;
                    rbProductPopup.Checked = false;
                    rbCampaignPopup.Enabled = false;
                    rbCampaignPopup.Checked = false;
                    cmbProductPopup.Enabled = false;
                    cmbCampaignPopup.Enabled = false;
                    cmbChannelPopup.Enabled = false;
                    lblChannelIdStar.Visible = false;
                    cmbStatusPopup.Enabled = false;
                    lblProductStar.Visible = false;
                    lblCampaignStar.Visible = false;
                    txtSlaTimePopup.Enabled = false;
                    txtSlaDayPopup.Enabled = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(productId))
                    {
                        cmbProductPopup.SelectedIndex = cmbProductPopup.Items.IndexOf(cmbProductPopup.Items.FindByValue(productId));
                        rbProductPopup.Checked = true;
                        rbCampaignPopup.Checked = false;
                        
//                        CheckSearchRadioConditionPopup();
                    }
                    else if (!string.IsNullOrEmpty(campaignId))
                    {
                        cmbCampaignPopup.SelectedIndex = cmbCampaignPopup.Items.IndexOf(cmbCampaignPopup.Items.FindByValue(campaignId));
                        rbProductPopup.Checked = false;
                        rbCampaignPopup.Checked = true;

//                        CheckSearchRadioConditionPopup();
                    }
                    EditsearchRadioConditionPopup();
                }

                cmbChannelPopup.SelectedIndex = cmbChannelPopup.Items.IndexOf(cmbChannelPopup.Items.FindByValue(channelId));
                cmbStatusPopup.SelectedIndex = cmbStatusPopup.Items.IndexOf(cmbStatusPopup.Items.FindByValue(statusCode));

                //BuildSubstatus(productId, campaignId, statusCode);

                if (!string.IsNullOrEmpty(productId))
                {
                    BuildSubstatusByProductId(productId, statusCode);
                }
                else if (!string.IsNullOrEmpty(campaignId))
                {
                    BuildSubstatusByCampaignId(campaignId, statusCode);
                }

                if (!string.IsNullOrEmpty(subStatusCode))
                    cmbSubStatusPopup.SelectedIndex = cmbSubStatusPopup.Items.IndexOf(cmbSubStatusPopup.Items.FindByValue(subStatusCode));

                txtSlaMinPopup.Text = ((Label)gvResult.Rows[index].FindControl("lblSlaMin")).Text.Trim().Replace(",", "");
                txtSlaTimePopup.Text = ((Label)gvResult.Rows[index].FindControl("lblSlaTime")).Text.Trim().Replace(",", "");
                txtSlaDayPopup.Text = ((Label)gvResult.Rows[index].FindControl("lblSlaDay")).Text.Trim().Replace(",", "");

                upPopup.Update();
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPopupControl();
            mpePopup.Hide();
        }

        private void ClearPopupControl()
        {
            txtSlaId.Text = "";
            rbProductPopup.Enabled = true;
            rbProductPopup.Checked = true;
            rbCampaignPopup.Enabled = true;
            rbCampaignPopup.Checked = false;
            cmbProductPopup.SelectedIndex = -1;
            cmbProductPopup.Enabled = true;
            cmbCampaignPopup.SelectedIndex = -1;
            cmbCampaignPopup.Enabled = false;
            cmbChannelPopup.Enabled = true;
            cmbChannelPopup.SelectedIndex = -1;
            lblChannelIdStar.Visible = true;
            cmbStatusPopup.Enabled = true;
            cmbStatusPopup.SelectedIndex = -1;
            cmbSubStatusPopup.Enabled = true;
            cmbSubStatusPopup.SelectedIndex = -1;
            cmbSubStatusPopup.Items.Clear();
            txtSlaMinPopup.Text = "";
            txtSlaTimePopup.Text = "";
            txtSlaDayPopup.Text = "";
            txtSlaMinPopup.Enabled = true;
            txtSlaTimePopup.Enabled = true;
            txtSlaDayPopup.Enabled = true;
            lblProductStar.Visible = true;
            lblCampaignStar.Visible = false;
            alertProductCampaignPopup.Text = "";
            alertChannelPopup.Text = "";
            alertStatusPopup.Text = "";
            alertSlaMinPopup.Text = "";
            alertSlaTimePopup.Text = "";
            alertSlaDayPopup.Text = "";
        }

        private bool ValidateInput()
        {
            int i = 0;
            if (cmbProductPopup.SelectedItem.Value == "" && cmbCampaignPopup.SelectedItem.Value == "")
            {
                alertProductCampaignPopup.Text = "กรุณาเลือก ผลิตภัณฑ์หรือแคมเปญ";
                i += 1;
            }
            else
                alertProductCampaignPopup.Text = "";

            if (cmbChannelPopup.SelectedItem.Value == "" && cmbProductPopup.SelectedItem.Value != slaDefault && cmbCampaignPopup.SelectedItem.Value != slaDefault)
            {
                alertChannelPopup.Text = "กรุณาเลือก ช่องทาง";
                i += 1;
            }
            else
                alertChannelPopup.Text = "";

            if (cmbStatusPopup.SelectedItem.Value == "")
            {
                alertStatusPopup.Text = "กรุณาเลือก สถานะ";
                i += 1;
            }
            else
                alertStatusPopup.Text = "";

            if (txtSlaMinPopup.Text.Trim() == "")
            {
                alertSlaMinPopup.Text = "กรุณาระบุ SLA (Minute)";
                i += 1;
            }
            else
            {
                if (int.Parse(txtSlaMinPopup.Text.Trim()) == 0)
                {
                    alertSlaMinPopup.Text = "SLA (Minute) ต้องมากกว่า 0";
                    i += 1;
                }
                else
                    alertSlaMinPopup.Text = "";
            }

            if (txtSlaTimePopup.Text.Trim() == "")
            {
                alertSlaTimePopup.Text = "กรุณาระบุ SLA (Times)";
                i += 1;
            }
            else
                alertSlaTimePopup.Text = "";

            if (txtSlaDayPopup.Text.Trim() == "")
            {
                alertSlaDayPopup.Text = "กรุณาระบุ SLA (Days)";
                i += 1;
            }
            else
            {
                if (int.Parse(txtSlaDayPopup.Text.Trim()) == 0)
                {
                    alertSlaDayPopup.Text = "SLA (Days) ต้องมากกว่า 0";
                    i += 1;
                }
                else
                    alertSlaDayPopup.Text = "";
            }

            return i > 0 ? false : true;
        }

        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((Label)e.Row.FindControl("lblProductName")).Text.Trim().ToUpper() == "DEFAULT" || ((Label)e.Row.FindControl("lblCampaignName")).Text.Trim().ToUpper() == "DEFAULT")
                    ((ImageButton)e.Row.FindControl("imbDelete")).Visible = false;
            }
        }

        protected void imbDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                int slaId = Convert.ToInt32(((ImageButton)sender).CommandArgument);
                SlaBiz.DeleteSla(slaId);

                DoSearchConfigSla(0);
                AppUtil.ClientAlert(Page, "ลบข้อมูลเรียบร้อย");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        //private void BuildSubstatus(string productId, string campaignId, string statusCode)
        //{
        //    cmbSubStatusPopup.DataSource = new ConfigProductSubStatusBiz().GetSubStatusList(productId, campaignId, statusCode);
        //    cmbSubStatusPopup.DataTextField = "TextField";
        //    cmbSubStatusPopup.DataValueField = "ValueField";
        //    cmbSubStatusPopup.DataBind();
        //    cmbSubStatusPopup.Items.Insert(0, new ListItem("", ""));
        //}

        private void BuildSubstatusByProductId(string productId, string statusCode)
        {
            cmbSubStatusPopup.DataSource = new ConfigProductSubStatusBiz().GetSubStatusListByProductId(productId, statusCode);
            cmbSubStatusPopup.DataTextField = "TextField";
            cmbSubStatusPopup.DataValueField = "ValueField";
            cmbSubStatusPopup.DataBind();
            cmbSubStatusPopup.Items.Insert(0, new ListItem("", ""));
        }

        protected void cmbProductPopup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BuildSubstatusByProductId(cmbProductPopup.SelectedItem.Value, cmbStatusPopup.SelectedItem.Value);
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void BuildSubstatusByCampaignId(string campaignId, string statusCode)
        {
            cmbSubStatusPopup.DataSource = new ConfigProductSubStatusBiz().GetSubStatusListByCampaignId(campaignId, statusCode);
            cmbSubStatusPopup.DataTextField = "TextField";
            cmbSubStatusPopup.DataValueField = "ValueField";
            cmbSubStatusPopup.DataBind();
            cmbSubStatusPopup.Items.Insert(0, new ListItem("", ""));
        }

        protected void cmbCampaignPopup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BuildSubstatusByCampaignId(cmbCampaignPopup.SelectedItem.Value, cmbStatusPopup.SelectedItem.Value);
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbStatusPopup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //BuildSubstatus(cmbProductPopup.SelectedItem.Value, cmbCampaignPopup.SelectedItem.Value, cmbStatusPopup.SelectedItem.Value);

                if (cmbProductPopup.SelectedItem.Value != "")
                    BuildSubstatusByProductId(cmbProductPopup.SelectedItem.Value, cmbStatusPopup.SelectedItem.Value);
                else if (cmbCampaignPopup.SelectedItem.Value != "")
                    BuildSubstatusByCampaignId(cmbCampaignPopup.SelectedItem.Value, cmbStatusPopup.SelectedItem.Value);

                mpePopup.Show();
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