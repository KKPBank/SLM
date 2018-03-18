using System;
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
    public partial class SLM_SCR_043 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_043));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_043");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาข้อมูล Premium";
                    InitialControl();
                    DoSearchPremium(0);
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
            AppUtil.SetIntTextBox(txtTotalAdd);

            cmbProductSearch.DataSource = ProductBiz.GetProductListNew();
            cmbProductSearch.DataTextField = "TextField";
            cmbProductSearch.DataValueField = "ValueField";
            cmbProductSearch.DataBind();
            cmbProductSearch.Items.Insert(0, new ListItem("", ""));

            //cmbCampaignSearch.DataSource = CampaignBiz.GetCampaignList("");
			cmbCampaignSearch.DataSource = CampaignBiz.GetCampaignListNew("");
            cmbCampaignSearch.DataTextField = "TextField";
            cmbCampaignSearch.DataValueField = "ValueField";
            cmbCampaignSearch.DataBind();
            cmbCampaignSearch.Items.Insert(0, new ListItem("", ""));
        }

        private void InitialPopup()
        {
            cmbProductAdd.DataSource = ProductBiz.GetProductListNew();
            cmbProductAdd.DataTextField = "TextField";
            cmbProductAdd.DataValueField = "ValueField";
            cmbProductAdd.DataBind();
            cmbProductAdd.Items.Insert(0, new ListItem("", ""));

            //cmbCampaignAdd.DataSource = CampaignBiz.GetCampaignList("");
			cmbCampaignAdd.DataSource = CampaignBiz.GetCampaignListNew("");
            cmbCampaignAdd.DataTextField = "TextField";
            cmbCampaignAdd.DataValueField = "ValueField";
            cmbCampaignAdd.DataBind();
            cmbCampaignAdd.Items.Insert(0, new ListItem("", ""));
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

        protected void rbProductAdd_CheckedChanged(object sender, EventArgs e)
        {
            CheckSearchRadioConditionPopup();
            mpePopupPremium.Show();
        }

        protected void rbCampaignAdd_CheckedChanged(object sender, EventArgs e)
        {
            CheckSearchRadioConditionPopup();
            mpePopupPremium.Show();
        }

        private void CheckSearchRadioConditionPopup()
        {
            if (rbProductAdd.Checked)
            {
                cmbProductAdd.Enabled = true;
                cmbCampaignAdd.SelectedIndex = -1;
                cmbCampaignAdd.Enabled = false;
                lblProductStar.Visible = true;
                lblCampaignStar.Visible = false;
            }
            else
            {
                cmbProductAdd.SelectedIndex = -1;
                cmbProductAdd.Enabled = false;
                cmbCampaignAdd.Enabled = true;
                lblProductStar.Visible = false;
                lblCampaignStar.Visible = true;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                rbProductSearch.Checked = true;
                cmbProductSearch.SelectedIndex = -1;
                cmbProductSearch.Enabled = true;

                rbCampaignSearch.Checked = false;
                cmbCampaignSearch.SelectedIndex = -1;
                cmbCampaignSearch.Enabled = false;

                cmbPremiumTypeSearch.SelectedIndex = -1;
                txtPremiumSearch.Text = "";
                cbActiveSearch.Checked = false;
                cbInActiveSearch.Checked = false;
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
                DoSearchPremium(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchPremium(int pageIndex)
        {
            try
            {
                ConfigProductPremiumBiz biz = new ConfigProductPremiumBiz();
                List<ConfigProductPremiumData> list = biz.SearchConfigPremium(cmbProductSearch.SelectedItem.Value, cmbCampaignSearch.SelectedItem.Value, txtPremiumSearch.Text.Trim(), cmbPremiumTypeSearch.SelectedItem.Value, cbActiveSearch.Checked, cbInActiveSearch.Checked );
                BindGridview(pcTop, list.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        protected void btnAddPremium_Click(object sender, EventArgs e)
        {
            try
            {
                InitialPopup();
                upnPopupPremium.Update();
                mpePopupPremium.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                txtEditPremiumName.Text = ((Label)gvResult.Rows[index].FindControl("lblPremiumName")).Text.Trim();

                txtEditProductId.Text = ((Label)gvResult.Rows[index].FindControl("lblProductId")).Text.Trim();
                txtEditCampaignId.Text = ((Label)gvResult.Rows[index].FindControl("lblCampaignId")).Text.Trim();
                txtEditPremiumTypeCode.Text = ((Label)gvResult.Rows[index].FindControl("lblPremiumType")).Text.Trim();

                if (!string.IsNullOrEmpty(txtEditProductId.Text))
                {
                    lblEditTitle.Text = "ผลิตภัณฑ์/บริการ";
                    txtEditTitle.Text = ((Label)gvResult.Rows[index].FindControl("lblProductName")).Text.Trim();
                }
                else if (!string.IsNullOrEmpty(txtEditCampaignId.Text))
                {
                    lblEditTitle.Text = "แคมเปญ";
                    txtEditTitle.Text = ((Label)gvResult.Rows[index].FindControl("lblCampaignName")).Text.Trim();
                }

                DoSearcEdithPremium(0, txtEditPremiumName.Text.Trim(), txtEditProductId.Text.Trim(), txtEditCampaignId.Text.Trim(), txtEditPremiumTypeCode.Text.Trim());

                upnPopupEditPremium.Update();
                mpePopupEditPremium.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #region Page Control Gridview AddConfigCustomer

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
                DoSearchPremium(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        #region Popup Add Premium

        protected void btnPopupSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    ConfigProductPremiumBiz biz = new ConfigProductPremiumBiz();
                    if (biz.InsertData(cmbProductAdd.SelectedItem.Value, cmbCampaignAdd.SelectedItem.Value, txtPremiumNameAdd.Text.Trim(), cmbPremiumTypeAdd.SelectedItem.Value
                        , tdmStartDateAdd.DateValue, tdmEndDateAdd.DateValue, int.Parse(txtTotalAdd.Text.Trim()), rbActiveAdd.Checked, HttpContext.Current.User.Identity.Name.ToLower()))
                    {
                        ClearPopupAddControl();
                        upnPopupPremium.Update();
                        mpePopupPremium.Hide();

                        DoSearchPremium(0);
                        AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
                    }
                    else
                    {
                        AppUtil.ClientAlert(Page, biz.ErrorMessage);
                        mpePopupPremium.Show();
                    }
                }
                else
                {
                    mpePopupPremium.Show();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnPopupCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ClearPopupAddControl();
                upnPopupPremium.Update();
                mpePopupPremium.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidateInput()
        {
            int count = 0;
            if (cmbProductAdd.SelectedItem.Value == "" && cmbCampaignAdd.SelectedItem.Value == "")
                count += 1;

            if (txtPremiumNameAdd.Text.Trim() == "")
                count += 1;

            if (cmbPremiumTypeAdd.SelectedItem.Value == "")
                count += 1;

            if (tdmStartDateAdd.DateValue.Year == 1 || tdmEndDateAdd.DateValue.Year == 1)
                count += 1;
            else
            {
                if (tdmStartDateAdd.DateValue > tdmEndDateAdd.DateValue)
                {
                    alrtStartDateAdd.Text = "วันที่เริ่มต้นต้องน้อยกว่าหรือเท่ากับวันที่สิ้นสุด";
                    count += 1;
                }
                else
                    alrtStartDateAdd.Text = "";
            }

            if (txtTotalAdd.Text.Trim() == "")
                count += 1;
            else
            {
                int result;
                if (int.TryParse(txtTotalAdd.Text.Trim(), out result))
                {
                    if (result <= 0)
                    {
                        alertTotalAdd.Text = "กรุณาระบุตัวเลขจำนวนเต็มที่มากกว่าศุนย์เท่านั้น";
                        count += 1;
                    }
                    else
                    {
                        alertTotalAdd.Text = "";
                    }
                }
                else
                {
                    alertTotalAdd.Text = "กรุณาระบุตัวเลขจำนวนเต็มที่มากกว่าศุนย์เท่านั้น";
                    count += 1;
                }
            }

            if (count > 0)
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน");
                return false;
            }
            else
                return true;
        }

        private void ClearPopupAddControl()
        {
            txtPremiumNameAdd.Text = "";
            rbProductAdd.Checked = true;
            rbCampaignAdd.Checked = false;
            cmbProductAdd.SelectedIndex = -1;
            cmbCampaignAdd.SelectedIndex = -1;
            CheckSearchRadioConditionPopup();
            cmbPremiumTypeAdd.SelectedIndex = -1;
            tdmStartDateAdd.DateValue = new DateTime();
            tdmEndDateAdd.DateValue = new DateTime();
            txtTotalAdd.Text = "";
            alertTotalAdd.Text = "";
            rbActiveAdd.Checked = true;
            rbNoActiveAdd.Checked = false;
        }

        #endregion

        #region Edit Popup Premium

        protected void btnEditPopupClose_Click(object sender, EventArgs e)
        {
            try
            {
                ClearEditPopupPremium();
                gvEditPremium.DataSource = null;
                gvEditPremium.DataBind();
                mpePopupEditPremium.Hide();

                DoSearchPremium(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ClearEditPopupPremium()
        {
            txtEditPremiumName.Text = "";
            lblEditTitle.Text = "";
            txtEditTitle.Text = "";
            txtEditPremiumTypeCode.Text = "";
            txtEditProductId.Text = "";
            txtEditCampaignId.Text = "";
        }

        private void DoSearcEdithPremium(int pageIndex, string premiumName, string productId, string campaignId, string premiumTypeCode)
        {
            try
            {
                ConfigProductPremiumBiz biz = new ConfigProductPremiumBiz();
                List<ConfigProductPremiumData> list = biz.SearchConfigPremiumForEdit(productId, campaignId, premiumName, premiumTypeCode, true, true);
                BindGridviewForEdit(pcTopForEdit, list.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        protected void rdActive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow row = ((RadioButton)sender).Parent.Parent as GridViewRow;
                txtEditRowIndex.Text = row.RowIndex.ToString();
                upConfirm.Update();
                mpePopupConfirm.Show();

                //RadioButton rb = (RadioButton)sender;
                //if (rb.Checked)
                //    ChangeActiveLot(rb.ValidationGroup, true);
                //else
                //    ChangeActiveLot(rb.ValidationGroup, false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void rdNoActive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow row = ((RadioButton)sender).Parent.Parent as GridViewRow;
                txtEditRowIndex.Text = row.RowIndex.ToString();
                upConfirm.Update();
                mpePopupConfirm.Show();

                //RadioButton rb = (RadioButton)sender;
                //if (rb.Checked)
                //    ChangeActiveLot(rb.ValidationGroup, false);
                //else
                //    ChangeActiveLot(rb.ValidationGroup, true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ChangeActiveLot(string premiumId, bool isActive)
        {
            try
            {
                ConfigProductPremiumBiz biz = new ConfigProductPremiumBiz();
                biz.ChangeActiveLot(txtEditProductId.Text.Trim(), txtEditCampaignId.Text.Trim(), txtEditPremiumName.Text.Trim(), txtEditPremiumTypeCode.Text.Trim(), decimal.Parse(premiumId), isActive, HttpContext.Current.User.Identity.Name.ToLower());
                DoSearcEdithPremium(0, txtEditPremiumName.Text.Trim(), txtEditProductId.Text.Trim(), txtEditCampaignId.Text.Trim(), txtEditPremiumTypeCode.Text.Trim());
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Page Control Gridview EditConfigCustomer

        private void BindGridviewForEdit(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvEditPremium);
            pageControl.Update(items, pageIndex);
            pageControl.GenerateRecordNumber(1, pageIndex);
            upnPopupEditPremium.Update();
        }

        protected void PageSearchChangeForEdit(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearcEdithPremium(pageControl.SelectedPageIndex, txtEditPremiumName.Text.Trim(), txtEditProductId.Text.Trim(), txtEditCampaignId.Text.Trim(), txtEditPremiumTypeCode.Text.Trim());
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void btnConfirmYes_Click(object sender, EventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(txtEditRowIndex.Text.Trim());
                RadioButton rdActive = gvEditPremium.Rows[index].FindControl("rdActive") as RadioButton;
                RadioButton rdNoActive = gvEditPremium.Rows[index].FindControl("rdNoActive") as RadioButton;

                ChangeActiveLot(rdActive.ValidationGroup, rdActive.Checked ? true : false);
                mpePopupConfirm.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnConfirmNo_Click(object sender, EventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(txtEditRowIndex.Text.Trim());
                RadioButton rdActive = gvEditPremium.Rows[index].FindControl("rdActive") as RadioButton;
                RadioButton rdNoActive = gvEditPremium.Rows[index].FindControl("rdNoActive") as RadioButton;

                if (rdActive.Checked)
                {
                    rdActive.Checked = false;
                    rdNoActive.Checked = true;
                }
                else if (rdNoActive.Checked)
                {
                    rdActive.Checked = true;
                    rdNoActive.Checked = false;
                }

                upnPopupEditPremium.Update();
                mpePopupConfirm.Hide();
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