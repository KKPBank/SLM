using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Resource.Data;
using SLM.Biz;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_036 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_036));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ข้อมูล Script และ Q&A";
            AppUtil.SetIntTextBox(txtSeqPopup);
            txtSeqPopup.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_036");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    InitialControl();
                    DoSearchSaleScript(0);
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
            //Search
//            cmbProductSearch.DataSource = ProductBiz.GetProductList();
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
//            cmbProductPopup.DataSource = ProductBiz.GetProductList();
//            cmbProductPopup.DataSource = ProductBiz.GetProductListNew();
            cmbProductPopup.DataTextField = "TextField";
            cmbProductPopup.DataValueField = "ValueField";
            cmbProductPopup.DataBind();
            cmbProductPopup.Items.Insert(0, new ListItem("", ""));

            //cmbCampaignPopup.DataSource = CampaignBiz.GetCampaignList("");
//			cmbCampaignPopup.DataSource = CampaignBiz.GetCampaignListNew("");
            cmbCampaignPopup.DataTextField = "TextField";
            cmbCampaignPopup.DataValueField = "ValueField";
            cmbCampaignPopup.DataBind();
            cmbCampaignPopup.Items.Insert(0, new ListItem("", ""));
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                rbProductSearch.Checked = true;
                rbCampaignSearch.Checked = false;
                cmbProductSearch.SelectedIndex = -1;
                cmbProductSearch.Enabled = true;
                cmbCampaignSearch.SelectedIndex = -1;
                cmbCampaignSearch.Enabled = false;
                cmbDataTypeSearch.SelectedIndex = -1;
                txtSubjectSearch.Text = "";
                txtDetailSearch.Text = "";
                cbActive.Checked = false;
                cbInActive.Checked = false;
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
                DoSearchSaleScript(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchSaleScript(int pageIndex)
        {
            try
            {
                ConfigScriptBiz biz = new ConfigScriptBiz();
                List<ConfigProductScriptData> list = biz.SearchConfigProductScript(cmbProductSearch.SelectedItem.Value, cmbCampaignSearch.SelectedItem.Value, cmbDataTypeSearch.SelectedItem.Value, txtSubjectSearch.Text.Trim(), txtDetailSearch.Text.Trim(), cbActive.Checked, cbInActive.Checked);
                BindGridview(pcTop, list.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        protected void btnAddScript_Click(object sender, EventArgs e)
        {
            try
            {
                BindPopupProductCampaignCombo("add");

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

        protected void imbDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                decimal configId = decimal.Parse(((ImageButton)sender).CommandArgument);
                ConfigScriptBiz biz = new ConfigScriptBiz();
                biz.DeleteData(configId);

                DoSearchSaleScript(0);
                AppUtil.ClientAlert(Page, "ลบข้อมูลเรียบร้อย");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
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
                DoSearchSaleScript(pageControl.SelectedPageIndex);
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    ConfigScriptBiz biz = new ConfigScriptBiz();

                    if (txtConfigScriptId.Text.Trim() != "")
                        biz.UpdateData(decimal.Parse(txtConfigScriptId.Text.Trim()), cmbProductPopup.SelectedItem.Value, cmbCampaignPopup.SelectedItem.Value, cmbDataTypePopup.SelectedItem.Value, txtSubjectPopup.Text.Trim(), txtDetailPopup.Text.Trim(), rdActivePopup.Checked, txtSeqPopup.Text.Trim(), HttpContext.Current.User.Identity.Name);
                    else
                        biz.InsertData(cmbProductPopup.SelectedItem.Value, cmbCampaignPopup.SelectedItem.Value, cmbDataTypePopup.SelectedItem.Value, txtSubjectPopup.Text.Trim(), txtDetailPopup.Text.Trim(), rdActivePopup.Checked, txtSeqPopup.Text.Trim(), HttpContext.Current.User.Identity.Name);

                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                    ClearPopupControl();
                    mpePopup.Hide();

                    DoSearchSaleScript(0);
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

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtConfigScriptId.Text = ((ImageButton)sender).CommandArgument;
                BindPopupProductCampaignCombo("edit");

                ConfigScriptBiz biz = new ConfigScriptBiz();
                ConfigProductScriptData data = biz.GetConfigScriptData(decimal.Parse(txtConfigScriptId.Text));

                if (data != null)
                {
                    if (!string.IsNullOrEmpty(data.ProductId))
                    {
                        rbProductPopup.Checked = true;
                        rbProductPopup.Enabled = false;
                        cmbProductPopup.Enabled = false;
                        cmbProductPopup.SelectedIndex = cmbProductPopup.Items.IndexOf(cmbProductPopup.Items.FindByValue(data.ProductId));

                        rbCampaignPopup.Checked = false;
                        rbCampaignPopup.Enabled = false;
                        cmbCampaignPopup.Enabled = false;
                    }
                    if (!string.IsNullOrEmpty(data.CampaignId))
                    {
                        rbCampaignPopup.Checked = true;
                        rbCampaignPopup.Enabled = false;
                        cmbCampaignPopup.Enabled = false;
                        cmbCampaignPopup.SelectedIndex = cmbCampaignPopup.Items.IndexOf(cmbCampaignPopup.Items.FindByValue(data.CampaignId));

                        rbProductPopup.Checked = false;
                        rbProductPopup.Enabled = false;
                        cmbProductPopup.Enabled = false;
                    }
                    if (!string.IsNullOrEmpty(data.DataType))
                        cmbDataTypePopup.SelectedIndex = cmbDataTypePopup.Items.IndexOf(cmbDataTypePopup.Items.FindByValue(data.DataType));

                    txtSubjectPopup.Text = data.Subject;
                    txtDetailPopup.Text = data.Detail;
                    txtSeqPopup.Text = data.Seq != null ? data.Seq.Value.ToString() : "";

                    if (data.IsDeleted != null)
                    {
                        if (data.IsDeleted.Value)
                        {
                            rdActivePopup.Checked = false;
                            rdNoActivePopup.Checked = true;
                        }
                        else
                        {
                            rdActivePopup.Checked = true;
                            rdNoActivePopup.Checked = false;
                        }
                    }

                    upPopup.Update();
                    mpePopup.Show();
                }
                else
                {
                    ClearPopupControl();
                    upPopup.Update();
                    AppUtil.ClientAlert(Page, "ไม่พบข้อมูล ConfigScriptId " + txtConfigScriptId.Text + " ในระบบ");
                }
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
            txtConfigScriptId.Text = "";
            rbProductPopup.Enabled = true;
            rbProductPopup.Checked = true;
            rbCampaignPopup.Enabled = true;
            rbCampaignPopup.Checked = false;
            cmbProductPopup.SelectedIndex = -1;
            cmbProductPopup.Enabled = true;
            cmbCampaignPopup.SelectedIndex = -1;
            cmbCampaignPopup.Enabled = false;
            lblProductStar.Visible = true;
            lblCampaignStar.Visible = false;
            cmbDataTypePopup.SelectedIndex = -1;
            txtSubjectPopup.Text = "";
            //txtSubSubjectPopup.Text = "";
            txtDetailPopup.Text = "";
            rdActivePopup.Checked = true;
            rdNoActivePopup.Checked = false;
            txtSeqPopup.Text = "";
            alertProductCampaignPopup.Text = "";
            alertSubjectPopup.Text = "";
            alertDetailPopup.Text = "";
            alertSeqPopup.Text = "";
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

            if (txtSubjectPopup.Text.Trim() == "")
            {
                alertSubjectPopup.Text = "กรุณาระบุ Subject";
                i += 1;
            }
            else
                alertSubjectPopup.Text = "";

            if (txtDetailPopup.Text.Trim() == "")
            {
                alertDetailPopup.Text = "กรุณาระบุ ข้อความ";
                i += 1;
            }
            else
                alertDetailPopup.Text = "";

            if (txtSeqPopup.Text.Trim() != "")
            {
                int result;
                if (int.TryParse(txtSeqPopup.Text.Trim(), out result))
                {
                    if (result <= 0)
                    {
                        alertSeqPopup.Text = "กรุณาระบุ sequence เป็นตัวเลขที่มากกว่าศูนย์เท่านั้น";
                        i += 1;
                    }
                    else
                        alertSeqPopup.Text = "";
                }
                else
                {
                    alertSeqPopup.Text = "กรุณาระบุ sequence เป็นตัวเลขที่มากกว่าศูนย์เท่านั้น";
                    i += 1;
                }
            }
            else
                alertSeqPopup.Text = "";

            return i > 0 ? false : true;
        }

        #endregion

        
    }
}