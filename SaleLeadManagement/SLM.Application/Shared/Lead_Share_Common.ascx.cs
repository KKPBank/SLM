using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Application.Utilities;
using SLM.Biz;

namespace SLM.Application.Shared
{
    public partial class Lead_Share_Common : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LeadInfo));
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //แคมเปญ
            AppUtil.BuildCombo(cmbCampaignId, SlmScr011Biz.GetCampaignEditData());
            //ช่องทาง
            AppUtil.BuildCombo(cmbChannelId, SlmScr003Biz.GetChannelData());
            //status
            AppUtil.BuildCombo(cmbStatus, SlmScr003Biz.GetOptionList("lead status"));
            //Owner Branch
            AppUtil.BuildCombo(cmbOwnerBranch, BranchBiz.GetBranchList(SLMConstant.Branch.Active));
            //Title
            AppUtil.BuildCombo(cmbTitle, SlmScr046Biz.GetTitleDataList());

            AppUtil.SetIntTextBox(txtTelNo_1);
            AppUtil.SetIntTextBox(txtTelNo2);
            if (cmbCampaignId.Enabled == true) AppUtil.SetAutoCompleteDropdown(new DropDownList[] {
                    cmbCampaignId,
                    cmbOwnerBranch,
                    cmbOwner,
                    cmbDelegateLead,
                    cmbDelegateBranch,
                    cmbTitle
                }
            , Page
            , this.ClientID + "_Autocomplete");
        }
        
        public string CampaignID { get { return cmbCampaignId.SelectedValue; } }

        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request["username"]))
                {
                    try
                    {
                        var usernameEncrypt = HttpUtility.UrlDecode(Request["username"]).Replace(" ", "+");
                        var username = StringCipher.Decrypt(usernameEncrypt, AppConstant.CSMEncryptPassword);
                        if (HttpContext.Current.User.Identity == null
                            || HttpContext.Current.User.Identity.Name == null
                            || HttpContext.Current.User.Identity.Name.ToLower() != username.ToLower())
                        {
                            ForceLogout();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Cannot Decrypt Username from URL", ex);
                    }
                }

                if (!string.IsNullOrEmpty(Request["name"]))
                {
                    txtName.Text = HttpUtility.UrlDecode(Request["name"].Trim());
                }

                if (!string.IsNullOrEmpty(Request["lastName"]))
                {
                    txtLastName.Text = HttpUtility.UrlDecode(Request["lastName"].Trim());
                }

                if (!string.IsNullOrEmpty(Request["campaignCode"]))
                {
                    cmbCampaignId.SelectedValue = HttpUtility.UrlDecode(Request["campaignCode"].Trim());
                }

                if (!string.IsNullOrEmpty(Request["channelCode"]))
                {
                    cmbChannelId.SelectedValue = HttpUtility.UrlDecode(Request["channelCode"].Trim());
                }

                if (!string.IsNullOrEmpty(Request["telNo"]))
                {
                    txtTelNo_1.Text = HttpUtility.UrlDecode(Request["telNo"].Trim());
                }
            }
        }

        private void ForceLogout()
        {
            ViewState.Clear();
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            var returnUrl = Request.Url.PathAndQuery;

            var url = string.Format("{0}?ReturnUrl={1}", FormsAuthentication.LoginUrl, Server.UrlEncode(returnUrl));
            Response.Redirect(url, false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected void txtName_TextChanged(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                vtxtName.Text = "กรุณาระบุชื่อ";
            }
            else
            {
                vtxtName.Text = "";
            }
        }
        protected void txtTelNo_1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal result1;
                if (txtTelNo_1.Text.Trim() == string.Empty)
                {
                    vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                    return;
                }
                else if (txtTelNo_1.Text.Trim().Length < 9 || txtTelNo_1.Text.Trim().Length > 10)
                {
                    vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                    return;
                }
                else if (txtTelNo_1.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo_1.Text.Trim(), out result1))
                {
                    vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องเป็นตัวเลขเท่านั้น";
                    return;
                }
                else if (txtTelNo_1.Text.Trim().StartsWith("0") == false)
                {
                    vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                    return;
                }
                else
                {
                    vtxtTelNo_1.Text = "";
                }

                //decimal result;
                //if (txtTelNo_1.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo_1.Text.Trim(), out result))
                //{
                //    vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องเป็นตัวเลขเท่านั้น";
                //    return;
                //}

                ////if (cmbCardType.SelectedItem.Value == "" || cmbCardType.SelectedItem.Value == AppConstant.CardType.JuristicPerson)
                ////{
                ////    if (txtTelNo_1.Text.Trim() == string.Empty || txtTelNo_1.Text.Trim().Length < 9 || txtTelNo_1.Text.Trim().Length > 10)
                ////    {
                ////        vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                ////        return;
                ////    }
                ////}
                ////else
                ////{
                //if (txtTelNo_1.Text.Trim() == string.Empty || txtTelNo_1.Text.Trim().Length != 10)
                //{
                //    vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                //    return;
                //}
                ////}

                //if (txtTelNo_1.Text.Trim().StartsWith("0") == false)
                //{
                //    vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                //    return;
                //}

                //vtxtTelNo_1.Text = "";
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        protected void txtTelNo2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal result2;
                if (txtTelNo2.Text.Trim() != string.Empty && txtTelNo2.Text.Trim().Length < 9)
                {
                    vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องมีอย่างน้อย 9 หลัก";
                    return;
                }

                if (txtTelNo2.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo2.Text.Trim(), out result2))
                {
                    vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องเป็นตัวเลขเท่านั้น";
                    return;
                }

                if (txtTelNo2.Text.Trim() != string.Empty && txtTelNo2.Text.Trim().StartsWith("0") == false)
                {
                    vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                    return;
                }

                vtxtTelNo2.Text = "";
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        protected void cmbChannelId_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbChannelId.SelectedItem.Value == string.Empty)
                {
                    vcmbChannelId.Text = "กรุณาระบุช่องทาง";
                }
                else
                {
                    vcmbChannelId.Text = "";
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        protected void cmbCampaignId_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                vcmbOwner.Text = "";
                SetOwnerBranchAccessRight();
                if (cmbOwnerBranch.SelectedItem != null)
                    txtOwnerBranchBefore.Text = cmbOwnerBranch.SelectedItem.Value;
                else
                    txtOwnerBranchBefore.Text = "";

                if (cmbOwner.SelectedItem != null)
                    txtOwnerLeadBefore.Text = cmbOwner.SelectedItem.Value;
                else
                    txtOwnerLeadBefore.Text = "";
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }

        }
        protected void cmbOwnerBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbOwnerBranchSelectedIndexChanged();
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ owner lead";
                }
                else
                {
                    vcmbOwner.Text = "";
                }
                txtOwnerBranchBefore.Text = cmbOwnerBranch.SelectedItem.Value;
                txtOwnerLeadBefore.Text = "";
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        protected void cmbOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ owner lead";
                    txtOwnerLeadBefore.Text = "";
                }
                else
                {
                    vcmbOwner.Text = "";
                    txtOwnerLeadBefore.Text = cmbOwner.SelectedItem.Value;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void cmbDelegateLead_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegateLead.SelectedItem.Value == string.Empty)
                {
                    vcmbDelegateLead.Text = "กรุณาระบุ Delegate Lead";
                }
                else
                {
                    vcmbDelegateLead.Text = "";
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void cmbDelegateBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbDelegateBranchSelectedIndexChanged();
                if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegateLead.SelectedItem.Value == string.Empty)
                {
                    vcmbDelegateLead.Text = "กรุณาระบุ Delegate Lead";
                }
                else
                {
                    vcmbDelegateLead.Text = "";
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }

        }
        private void cmbOwnerBranchSelectedIndexChanged(string userName=null)
        {
            try
            {
                //Owner Lead
                List<ControlListData> source = null;
                if (cmbOwnerBranch.SelectedItem != null)
                {
                    if (cmbCampaignId.SelectedItem.Value == "")
//                        source = StaffBiz.GetStaffList(cmbOwnerBranch.SelectedItem.Value);   //SlmScr010Biz.GetStaffAllData(cmbOwnerBranch.SelectedItem.Value);
                        source = StaffBiz.GetStaffNotDummyList(cmbOwnerBranch.SelectedItem.Value);
                    else
//                        source = StaffBiz.GetStaffAllDataByAccessRight(cmbCampaignId.SelectedItem.Value, cmbOwnerBranch.SelectedItem.Value);
                        source = StaffBiz.GetStaffAllDataNotDummyByAccessRight(cmbCampaignId.SelectedItem.Value, cmbOwnerBranch.SelectedItem.Value, userName);

                    //คำนวณงานในมือ
                    AppUtil.CalculateAmountJobOnHandForDropdownlist(cmbOwnerBranch.SelectedItem.Value, source);

                    AppUtil.BuildCombo(cmbOwner, source);

                    if (cmbOwnerBranch.SelectedItem.Value != string.Empty)
                        cmbOwner.Enabled = true;
                    else
                        cmbOwner.Enabled = false;
                }
                else
                {
                    cmbOwner.Items.Clear();
                    cmbOwner.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void cmbDelegateBranchSelectedIndexChanged(string owner = null)
        {
            try
            {
                //Delegate Lead
//                List<ControlListData> source = StaffBiz.GetStaffAllDataByAccessRight(cmbCampaignId.SelectedItem.Value, cmbDelegateBranch.SelectedItem.Value);
                List<ControlListData> source = StaffBiz.GetStaffAllDataNotDummyByAccessRight(cmbCampaignId.SelectedItem.Value, cmbDelegateBranch.SelectedItem.Value, owner);
                //คำนวณงานในมือ
                AppUtil.CalculateAmountJobOnHandForDropdownlist(cmbDelegateBranch.SelectedItem.Value, source);
                AppUtil.BuildCombo(cmbDelegateLead, source);

                if (cmbDelegateBranch.SelectedItem.Value != string.Empty)
                    cmbDelegateLead.Enabled = true;
                else
                    cmbDelegateLead.Enabled = false;
            }
            catch
            {
                throw;
            }
        }
        private void SetComboOwnerBranch()
        {
            var lst = SlmScr010Biz.GetBranchListByAccessRight(SLMConstant.Branch.Active, cmbCampaignId.SelectedItem.Value);
            AppUtil.BuildCombo(cmbOwnerBranch, lst);
            AppUtil.BuildCombo(cmbDelegateBranch, lst);
        }


        #region Popup Search Campaign

        protected void btnClose_Click(object sender, EventArgs e)
        {
            mpePopupSearchCampaign.Hide();
        }

        protected void imbSearchCampaign_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                rbSearchByCombo.Checked = true;
                rbSearchByText.Checked = false;
                SearchCampaignCheckChanged();
                BindComboProductGroup();
                cmbProduct.Items.Clear();
                cmbProduct.Items.Insert(0, new ListItem("", "0"));
                cmbCampaign.Items.Clear();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));

                pcGridCampaign.SetVisible = false;
                gvCampaign.DataSource = null;
                gvCampaign.DataBind();
                gvCampaign.Visible = false;

                if (txtScreenHeight.Text.Trim() != "" && txtScreenWidth.Text.Trim() != "")
                {
                    if (Convert.ToInt32(txtScreenHeight.Text.Trim()) > 0 && Convert.ToInt32(txtScreenHeight.Text.Trim()) < 700)
                    {
                        pnPopupSearchCampaign.Height = new Unit(0.6 * Convert.ToDouble(txtScreenHeight.Text.Trim()), UnitType.Pixel);
                        pnPopupSearchCampaign.Width = new Unit(0.8 * Convert.ToDouble(txtScreenWidth.Text.Trim()), UnitType.Pixel);
                    }
                    else if (Convert.ToInt32(txtScreenHeight.Text.Trim()) >= 700 && Convert.ToInt32(txtScreenHeight.Text.Trim()) < 950)
                    {
                        pnPopupSearchCampaign.Height = new Unit(0.75 * Convert.ToDouble(txtScreenHeight.Text.Trim()), UnitType.Pixel);
                        pnPopupSearchCampaign.Width = new Unit(0.75 * Convert.ToDouble(txtScreenWidth.Text.Trim()), UnitType.Pixel);
                    }
                }

                upPopupSearchCampaign.Update();
                mpePopupSearchCampaign.Show();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
            btnSearchCampaign.Focus();
        }

        private void BindComboProductGroup()
        {
            cmbProductGroup.DataSource = SlmScr003Biz.GetProductGroupData();
            cmbProductGroup.DataTextField = "TextField";
            cmbProductGroup.DataValueField = "ValueField";
            cmbProductGroup.DataBind();
            cmbProductGroup.Items.Insert(0, new ListItem("", ""));
        }

        protected void cmbProductGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //cmbProduct.DataSource = SlmScr003Biz.GetProductData(cmbProductGroup.SelectedItem.Value);
				cmbProduct.DataSource = SlmScr003Biz.GetProductDataNew(cmbProductGroup.SelectedItem.Value);
                cmbProduct.DataTextField = "TextField";
                cmbProduct.DataValueField = "ValueField";
                cmbProduct.DataBind();
                cmbProduct.Items.Insert(0, new ListItem("", "0"));  //value = 0 ป้องกันในกรณีส่งค่า ช่องว่างไป where ใน CMT_CAMPAIGN_PRODUCT แล้วค่า PR_ProductId บาง record เป็นช่องว่าง

                cmbCampaign.Items.Clear();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));

                mpePopupSearchCampaign.Show();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
            btnSearchCampaign.Focus();
        }

        protected void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbCampaign.DataSource = SlmScr003Biz.GetCampaignDataNew(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value);
                cmbCampaign.DataTextField = "TextField";
                cmbCampaign.DataValueField = "ValueField";
                cmbCampaign.DataBind();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));

                mpePopupSearchCampaign.Show();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
            btnSearchCampaign.Focus();
        }

        protected void rbSearchByCombo_CheckedChanged(object sender, EventArgs e)
        {
            SearchCampaignCheckChanged();
            upPopupSearchCampaign.Update();
            mpePopupSearchCampaign.Show();
        }

        protected void rbSearchByText_CheckedChanged(object sender, EventArgs e)
        {
            SearchCampaignCheckChanged();
            upPopupSearchCampaign.Update();
            mpePopupSearchCampaign.Show();
        }

        private void SearchCampaignCheckChanged()
        {
            cmbProductGroup.Enabled = rbSearchByCombo.Checked;
            cmbProduct.Enabled = rbSearchByCombo.Checked;
            cmbCampaign.Enabled = rbSearchByCombo.Checked;

            txtFullSearchCampaign.Enabled = rbSearchByText.Checked;

            txtFullSearchCampaign.Text = "";
            pcGridCampaign.SetVisible = false;
            gvCampaign.DataSource = null;
            gvCampaign.DataBind();
            gvCampaign.Visible = false;

            if (rbSearchByText.Checked)
            {
                cmbProductGroup.SelectedIndex = -1;
                cmbProduct.Items.Clear();
                cmbProduct.Items.Insert(0, new ListItem("", "0"));
                cmbCampaign.Items.Clear();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));
            }
        }

        protected void btnSearchCampaign_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                    DoSearchCampaign();

                mpePopupSearchCampaign.Show();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidateInput()
        {
            if (rbSearchByCombo.Checked)
            {
                if (cmbProductGroup.SelectedItem.Value == "")
                {
                    AppUtil.ClientAlert(Page, "กรุณาเลือกข้อมูล กลุ่มผลิตภัณฑ์/บริการ");
                    return false;
                }
                //if (cmbProduct.SelectedItem.Value == "0")
                //{
                //    AppUtil.ClientAlert(Page, "กรุณาเลือกข้อมูล ผลิตภัณฑ์/บริการ");
                //    return false;
                //}
                //if (cmbCampaign.SelectedItem.Value == "")
                //{
                //    AppUtil.ClientAlert(Page, "กรุณาเลือกข้อมูล แคมเปญ");
                //    return false;
                //}
            }
            else
            {
                if (txtFullSearchCampaign.Text.Trim() == "")
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุคำที่ต้องการค้นหา");
                    return false;
                }
            }


            return true;
        }

        protected void gvCampaign_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((Label)e.Row.FindControl("lblCampaignId")).Text.Trim() == "")
                {
                    ((CheckBox)e.Row.FindControl("cbSelect")).Visible = false;
                }

                Label lblCampaignDesc = (Label)e.Row.FindControl("lblCampaignDesc");
                if (lblCampaignDesc.Text.Trim().Length > AppConstant.Campaign.DisplayCampaignDescMaxLength)
                {
                    lblCampaignDesc.Text = lblCampaignDesc.Text.Trim().Substring(0, AppConstant.Campaign.DisplayCampaignDescMaxLength) + "...";
                    LinkButton lbShowCampaignDesc = (LinkButton)e.Row.FindControl("lbShowCampaignDesc");
                    lbShowCampaignDesc.Visible = true;
                    lbShowCampaignDesc.OnClientClick = AppUtil.GetShowCampaignDescScript(Page, lbShowCampaignDesc.CommandArgument, "leadinfo_campaigndesc_" + lbShowCampaignDesc.CommandArgument);
                }
            }
        }

        protected void cbSelect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                int index = ((CheckBox)sender).TabIndex;
                string campaignId = ((Label)gvCampaign.Rows[index].FindControl("lblCampaignId")).Text.Trim();
                cmbCampaignId.SelectedIndex = cmbCampaignId.Items.IndexOf(cmbCampaignId.Items.FindByValue(campaignId));
                SetOwnerBranchAccessRight();
                updCommon.Update();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchCampaign()
        {
            List<ProductData> result = null;

            if (rbSearchByCombo.Checked)
                result = SlmScr003Biz.SearchCampaignNew(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, cmbCampaign.SelectedItem.Value);
            else
                result = SlmScr003Biz.SearchCampaignNew(txtFullSearchCampaign.Text.Trim());

            BindGridview((SLM.Application.Shared.GridviewPageController)pcGridCampaign, result.ToArray(), 0);
            gvCampaign.Visible = true;
            upPopupSearchCampaign.Update();
        }        
        #endregion

        #region Page Control
        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvCampaign);
            pageControl.Update(items, pageIndex);
            upPopupSearchCampaign.Update();
            mpePopupSearchCampaign.Show();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                List<ProductData> result = null;

                if (rbSearchByCombo.Checked)
                    result = SlmScr003Biz.SearchCampaignNew(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, cmbCampaign.SelectedItem.Value); //SlmScr003Biz.SearchCampaign
                else
                    result = SlmScr003Biz.SearchCampaignNew(txtFullSearchCampaign.Text.Trim()); // SlmScr003Biz.SearchCampaign

                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridview(pageControl, result.AsEnumerable().ToArray(), pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        public void SetLeadData(LeadData lead) { SetLeadData(lead, false); }
        public void SetLeadData(LeadData lead, bool isnew, bool iscopy = false)
        {
            // setdata
            if (lead.TitleId != null)
            {
                cmbTitle.SelectedIndex = cmbTitle.Items.IndexOf(cmbTitle.Items.FindByValue(lead.TitleId.Value.ToString()));
            }
            txtName.Text = lead.Name;
            txtLastName.Text = lead.LastName;
            txtslm_TicketId.Text = lead.TicketId;
            txtTelNo_1.Text = lead.TelNo_1;
            txtTelNo2.Text = lead.TelNo_2;
            txtContactFirstDate.Text = lead.ContactFirstDate == null ? "" : GetDateTimeString(lead.ContactFirstDate.Value);
            txtContactLatestDate.Text = lead.ContactLatestDate == null ? "" : GetDateTimeString(lead.ContactLatestDate.Value);
            txtAssignDate.Text = lead.AssignedDateView == null ? "" : GetDateTimeString(lead.AssignedDateView.Value);
            txtCreateDate.Text = lead.LeadCreateDate == null ? "" : GetDateString(lead.LeadCreateDate.Value);
            txtCreateTime.Text = lead.LeadCreateDate == null ? "" : lead.LeadCreateDate.Value.ToString("HH:mm:ss");
            txtCreateBy.Text = lead.LeadCreateBy;
            txtAssignFlag.Text = lead.AssignedFlag;
            txtDelegateFlag.Text = lead.Delegate_Flag != null ? lead.Delegate_Flag.ToString() : "";
            if (!iscopy) txtOldOwner.Text = lead.Owner;
            txtTicketIdRefer.Text = lead.TicketIdRefer;

            AppUtil.SetComboValue(cmbStatus, lead.Status);
            AppUtil.SetComboValue(cmbCampaignId, lead.CampaignId);
            SetComboOwnerBranch();
            AppUtil.SetComboValue(cmbChannelId, lead.ChannelId);
            AppUtil.SetComboValue(cmbOwnerBranch, lead.Owner_Branch);
            cmbOwnerBranchSelectedIndexChanged(lead.Owner=="" ? null : lead.Owner);
            AppUtil.SetComboValue(cmbOwner, lead.Owner);


            if (isnew) return;

            // delegate branch/lead
            if (!string.IsNullOrEmpty(lead.Delegate_Branch))
            {
                ListItem item = cmbDelegateBranch.Items.FindByValue(lead.Delegate_Branch);
                if (item != null)
                    cmbDelegateBranch.SelectedIndex = cmbDelegateBranch.Items.IndexOf(item);
                else
                {
                    //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
                    if (SlmScr011Biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, cmbCampaignId.SelectedItem.Value, lead.Delegate_Branch))
                    {
                        //Branch ที่ถูกปิด
                        string branchName = BranchBiz.GetBranchName(lead.Delegate_Branch);
                        if (!string.IsNullOrEmpty(branchName))
                        {
                            cmbDelegateBranch.Items.Insert(1, new ListItem(branchName, lead.Delegate_Branch));
                            cmbDelegateBranch.SelectedIndex = 1;
                        }
                    }
                }

                cmbDelegateBranchSelectedIndexChanged(lead.Delegate);    //Bind Combo Delegate
            }

            if (!string.IsNullOrEmpty(lead.Delegate))
            {
                txtoldDelegate.Text = lead.Delegate;
                cmbDelegateLead.SelectedIndex = cmbDelegateLead.Items.IndexOf(cmbDelegateLead.Items.FindByValue(lead.Delegate));
            }



            if (lead.AssignedFlag == "0" || lead.Delegate_Flag == 1)   //ยังไม่จ่ายงาน assignedFlag = 0, delegateFlag = 1
            {
                cmbDelegateBranch.Enabled = false;
                cmbDelegateLead.Enabled = false;
                cmbOwnerBranch.Enabled = false;
                cmbOwner.Enabled = false;
                lblAlert.Text = "ไม่สามารถแก้ไข Owner และ Delegate ได้ เนื่องจากอยู่ระหว่างรอระบบจ่ายงาน กรุณารอ 1 นาที";
            }
            else
                AppUtil.CheckOwnerPrivilege(lead.Owner, lead.Delegate, cmbOwnerBranch, cmbOwner, cmbDelegateBranch, cmbDelegateLead);
        }
        
        private void SetOwnerBranchAccessRight()
        {
            try
            {
                List<BranchData> branch = new List<BranchData>();
                if (cmbCampaignId.SelectedItem.Value == "")
                {
                    txtOwnerBranchBefore.Text = "";
                    txtOwnerLeadBefore.Text = "";
                    //Owner Branch
                    //SetComboOwnerBranch();
                    AppUtil.BuildCombo(cmbOwnerBranch, BranchBiz.GetBranchList(SLMConstant.Branch.Active));

                    cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(cmbOwnerBranch.Items.FindByValue(txtOwnerBranchBefore.Text.Trim()));
                    cmbOwnerBranchSelectedIndexChanged();
                    cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(txtOwnerLeadBefore.Text.Trim()));
                }
                else
                {
                    SetComboOwnerBranch();

                    if (SlmScr010Biz.CheckStaffAccessRightExist(cmbCampaignId.SelectedItem.Value, txtOwnerBranchBefore.Text.Trim(), txtOwnerLeadBefore.Text.Trim()))
                    {
                        if (cmbOwnerBranch.Items.Count > 1)
                        {
                            if (txtOwnerBranchBefore.Text.Trim() != "")
                                cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(cmbOwnerBranch.Items.FindByValue(txtOwnerBranchBefore.Text.Trim()));
                            else
                                cmbOwnerBranch.SelectedIndex = -1;

                        }
                        cmbOwnerBranchSelectedIndexChanged();
                        cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(txtOwnerLeadBefore.Text.Trim()));
                    }
                    else
                    {
                        cmbOwnerBranchSelectedIndexChanged();
                        txtOwnerBranchBefore.Text = "";
                        txtOwnerLeadBefore.Text = "";
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        private void GetBranchOwnerDefault()
        {
            try
            {
                List<StaffData> stafflist = SlmScr016Biz.GetChannelStaffData(HttpContext.Current.User.Identity.Name);
                if (stafflist.Count > 0)
                {
                    if (!string.IsNullOrEmpty(stafflist.FirstOrDefault().ChannelId))
                    {
                        if ((stafflist.FirstOrDefault().ChannelId.ToUpper() == SLM.Resource.SLMConstant.ChannelId.Branch) ||
                            (stafflist.FirstOrDefault().ChannelId.ToUpper() == SLM.Resource.SLMConstant.ChannelId.Telesales) ||
                            (stafflist.FirstOrDefault().ChannelId.ToUpper() == SLM.Resource.SLMConstant.ChannelId.PriorityBanking))
                        {
                            string branch = SlmScr010Biz.GetBranchData(HttpContext.Current.User.Identity.Name);
                            if (branch != "")
                            {
                                //cmbChannelId.SelectedIndex = cmbChannelId.Items.IndexOf(cmbChannelId.Items.FindByValue(stafflist.FirstOrDefault().ChannelId));
                                cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(cmbOwnerBranch.Items.FindByValue(branch));
                                cmbOwnerBranchSelectedIndexChanged();

                                if (cmbOwner.Items.Count > 0)
                                {
                                    ListItem item = cmbOwner.Items.OfType<ListItem>().Where(p => p.Value.Trim().ToLower() == HttpContext.Current.User.Identity.Name.Trim().ToLower()).FirstOrDefault();
                                    if (item != null)
                                        cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(item);
                                }

                                //cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(HttpContext.Current.User.Identity.Name));
                                //txtOwnerBranchUserLogin.Text = branch;
                                txtOwnerBranchBefore.Text = branch;
                                if (cmbOwner.SelectedIndex >= 0)
                                    txtOwnerLeadBefore.Text = cmbOwner.SelectedItem.Value;
                            }
                        }
                        else
                        {
                            cmbOwner.Items.Clear();
                            cmbOwner.Enabled = false;
                        }
                    }
                    else
                    {
                        cmbOwner.Items.Clear();
                        cmbOwner.Enabled = false;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        private void GetChannelDefault()
        {
            try
            {
                string channelId = SlmScr010Biz.GetChannelDefault(HttpContext.Current.User.Identity.Name);
                if (!string.IsNullOrEmpty(channelId))
                {
                    cmbChannelId.SelectedIndex = cmbChannelId.Items.IndexOf(cmbChannelId.Items.FindByValue(channelId));
                }
            }
            catch
            {
                throw;
            }
        }
        
        private string GetDateTimeString(DateTime dt)
        {
            return String.Format(dt.ToString("dd/MM/{0} HH:mm:ss"), dt.Year);
        }
        private string GetDateString(DateTime dt)
        {
            return String.Format(dt.ToString("dd/MM/{0}"), dt.Year);
        }
        public void SetEnableCampaign(bool b)
        {
            cmbCampaignId.Enabled = b;
            imbSearchCampaign.Visible = b;
        }

        private string newassignflag = "";
        private string newDelegateFlag = "";
        private bool IsLastestOwnerDelegate()
        {
            try
            {
                List<string> flagList = LeadInfoBiz.GetAssignedFlagAndDelegateFlag(txtslm_TicketId.Text.Trim());
                newassignflag = flagList[0];
                newDelegateFlag = flagList[1];

                if (newassignflag != txtAssignFlag.Text.Trim())
                    return false;
                if (newDelegateFlag != txtDelegateFlag.Text.Trim())
                    return false;

                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool ValidateData(bool isValidateNewLead = true)
        {
            if (!isValidateNewLead)     //หน้า Edit
            {
                if (!IsLastestOwnerDelegate())
                {
                    AppUtil.ClientAlertAndRedirect(Page, "ข้อมูลผู้มุ่งหวังนี้ถูกจ่ายงานให้พนักงานท่านอื่นแล้ว ระบบจะทำการ Refresh หน้าจอให้", "SLM_SCR_011.aspx?ticketid=" + txtslm_TicketId.Text.Trim());
                    return false;
                }
            }

            int i = 0;
            if (txtName.Text.Trim() == string.Empty)
            {
                vtxtName.Text = "กรุณาระบุชื่อ";
                i += 1;
            }
            else
            {
                vtxtName.Text = "";
            }

            if (cmbCampaignId.SelectedItem.Value == string.Empty)
            {
                vcmbCampaignId.Text = "กรุณาระบุแคมเปญ";
                i += 1;
            }
            else
            {
                vcmbCampaignId.Text = "";
            }

            if (cmbChannelId.SelectedItem.Value == string.Empty)
            {
                vcmbChannelId.Text = "กรุณาระบุช่องทาง";
                i += 1;
            }
            else
            {
                vcmbChannelId.Text = "";
            }
            //****************************Owner**********************************************************
            if (isValidateNewLead)
            {
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ Owner Lead";
                    i += 1;
                }
                else
                {
                    vcmbOwner.Text = "";
                    if (cmbCampaignId.SelectedItem.Value != string.Empty && cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value != string.Empty)
                    {
                        if (!SlmScr010Biz.PassPrivilegeCampaign(SLMConstant.Branch.Active, cmbCampaignId.SelectedItem.Value, cmbOwner.SelectedItem.Value))
                        {
                            vcmbOwner.Text = "Owner Lead ไม่มีสิทธิ์ในแคมเปญนี้";
                            i += 1;
                        }
                        else
                            vcmbOwner.Text = "";
                    }
                }
            }
            else
            { 
                //หน้า Edit
                //OwnerBranch, Owner
                string clearOwnerBranchText = "Y";
                if (!AppUtil.ValidateOwner(newassignflag, cmbOwnerBranch, vcmbOwnerBranch, cmbOwner, vcmbOwner, cmbCampaignId.SelectedItem.Value, ref clearOwnerBranchText))
                    i += 1;

                //Branch ที่ถูกปิด
                if (cmbOwnerBranch.Items.Count > 0 && cmbOwnerBranch.SelectedItem.Value != "" && !BranchBiz.CheckBranchActive(cmbOwnerBranch.SelectedItem.Value))
                {
                    vcmbOwnerBranch.Text = "สาขานี้ถูกปิดแล้ว";
                    i += 1;
                }
                else
                {
                    if (clearOwnerBranchText == "Y")
                        vcmbOwnerBranch.Text = "";
                }
            }


            //****************************Delegate**********************************************************
            //Delegate มีเฉพาะหน้า Edit
            if (!isValidateNewLead)
            {
                if (cmbDelegateBranch.Items.Count > 0 && cmbDelegateBranch.SelectedItem.Value != "" && !BranchBiz.CheckBranchActive(cmbDelegateBranch.SelectedItem.Value))
                {
                    vcmbDelegateBranch.Text = "สาขานี้ถูกปิดแล้ว";
                    i += 1;
                }
                else
                    vcmbDelegateBranch.Text = "";

                if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegateLead.SelectedItem.Value == string.Empty)
                {
                    vcmbDelegateLead.Text = "กรุณาระบุ Delegate Lead";
                    i += 1;
                }
                else
                    vcmbDelegateLead.Text = "";
            }

            //****************************หมายเลขโทรศัพท์ 1********************************************
            decimal result1;
            if (txtTelNo_1.Text.Trim() == string.Empty)
            {
                vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                i += 1;
            }
            else if (txtTelNo_1.Text.Trim().Length < 9 || txtTelNo_1.Text.Trim().Length > 10)
            {
                vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                i += 1;
            }
            else if (txtTelNo_1.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo_1.Text.Trim(), out result1))
            {
                vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องเป็นตัวเลขเท่านั้น";
                i += 1;
            }
            else if (txtTelNo_1.Text.Trim().StartsWith("0") == false)
            {
                vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                i += 1;
            }
            else
            {
                vtxtTelNo_1.Text = "";
            }

            //****************************หมายเลขโทรศัพท์ 2********************************************
            decimal result2;
            if (txtTelNo2.Text.Trim() != string.Empty && txtTelNo2.Text.Trim().Length < 9)
            {
                vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องมีอย่างน้อย 9 หลัก";
                i += 1;
            }
            else if (txtTelNo2.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo2.Text.Trim(), out result2))
            {
                vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องเป็นตัวเลขเท่านั้น";
                i += 1;
            }
            else if (txtTelNo2.Text.Trim() != string.Empty && txtTelNo2.Text.Trim().StartsWith("0") == false)
            {
                vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                i += 1;
            }
            else
                vtxtTelNo2.Text = "";
            //****************************หมายเลขโทรศัพท์ 3********************************************

            //****************************Ticket ID Refer********************************************
            SlmScr003Biz biz = new SlmScr003Biz();
            if (txtTicketIdRefer.Text.Trim() != string.Empty && txtslm_TicketId.Text == txtTicketIdRefer.Text.Trim())
            {
                vtxtTicketIdRefer.Text = "Ticket ID Refer ต้องไม่เท่ากับ Ticket ID";
                i += 1;
            }
            else if(SlmScr003Biz.ValidateTicketIDRefer(txtTicketIdRefer.Text.Trim()))
            {
                vtxtTicketIdRefer.Text = "ไม่พบ Ticket ID Refer ที่ระบุ";
                i += 1;
            }
            //ValidateTicketIDRefer

            return i == 0;
        }
        public LeadControlCommonData GetCommonData()
        {
            LeadControlCommonData ret = new LeadControlCommonData();
            ret.TicketID = txtslm_TicketId.Text.Trim();
            if (cmbTitle.SelectedItem.Value != "")
            {
                ret.TitleID = int.Parse(cmbTitle.SelectedItem.Value);
            }           
            ret.FName = txtName.Text.Trim();
            ret.LName = txtLastName.Text.Trim();
            ret.Phone1 = txtTelNo_1.Text.Trim();
            ret.Phone2 = txtTelNo2.Text.Trim();
            ret.ChannelID = cmbChannelId.SelectedValue;
            ret.CampaignID = cmbCampaignId.SelectedValue;
            ret.Branch = cmbOwnerBranch.SelectedValue;
            ret.Owner = cmbOwner.SelectedValue;
            ret.ActDelegate = cmbDelegateLead.SelectedValue != txtoldDelegate.Text;
            ret.TicketIDRefer = txtTicketIdRefer.Text.Trim();
            //add
            if (ret.ActDelegate)
            {
                ret.DelegateFlag = "1";
                ret.Type = "03";
            }
            else
            {
                ret.DelegateFlag = "0";
            }


            ret.ActOwner = cmbOwner.SelectedValue != txtOldOwner.Text;
            if (ret.ActOwner)
            {
                ret.OldOwner = txtOldOwner.Text;
                ret.Type2 = "10";
            }

            ret.DelegateBranch = cmbDelegateBranch.SelectedValue;
            ret.DelegateLead = cmbDelegateLead.SelectedValue;
            ret.OldDelegateLead = txtoldDelegate.Text;
            ret.Status = cmbStatus.SelectedValue;

            ret.R_CampaignName = cmbCampaignId.SelectedItem == null ? "" : cmbCampaignId.SelectedItem.Text;
            ret.R_ChannelName = cmbChannelId.SelectedItem == null ? "" : cmbChannelId.SelectedItem.Text;
            ret.R_Owner = cmbOwner.SelectedItem == null ? "" : cmbOwner.SelectedItem.Text;

            ret.OldOwner2 = txtOldOwner.Text;

            return ret;
        }
        public void SetControlMode(Lead_Detail_Master.CtlMode ctlMode)
        {
            switch (ctlMode)
            {
                case Lead_Detail_Master.CtlMode.New:
                    trInfo1.Visible = false;
                    trInfo2.Visible = false;
                    tbLeadInfo.Visible = false;
                    trTicketIdRefer.Visible = false;
                    if (!IsPostBack)
                    {
                        SearchCampaignCheckChanged();
                        GetBranchOwnerDefault();
                        GetChannelDefault();

                        // clear 
                        hdfTicketID.Value = "";
                        txtslm_TicketId.Text = "";
                        txtTicketIdRefer.Text = "";
                        cmbStatus.SelectedIndex = 0;
                        cmbCampaignId.SelectedIndex = 0;

                    }
                    break;

                case Lead_Detail_Master.CtlMode.Edit:
                    trInfo1.Visible = true;
                    trInfo2.Visible = true;
                    tbLeadInfo.Visible = true;
                    trTicketIdRefer.Visible = true;

                    txtslm_TicketId.Enabled = false;
                    txtTelNo_1.Enabled = false;
                    SetEnableCampaign(false);
                    cmbStatus.Enabled = false;
                    cmbChannelId.Enabled = false;
                    break;

                case Lead_Detail_Master.CtlMode.View:
                    trInfo1.Visible = true;
                    trInfo2.Visible = true;
                    tbLeadInfo.Visible = true;
                    trTicketIdRefer.Visible = true;

                    trInfo1.Visible = true;
                    trInfo2.Visible = true;
                    tbLeadInfo.Visible = true;

                    lblAlert.Visible = false;
                    SetEnableCampaign(false);
                    break;
            }
        }
        public void SetAddCampaignCombo()
        {
//            AppUtil.BuildCombo(cmbCampaignId, SlmScr003Biz.GetCampaignData());
            AppUtil.BuildCombo(cmbCampaignId, SlmScr003Biz.GetCampaignDataNew());
        }

        public TextBox GetTextBoxTelNo1()
        {
            return txtTelNo_1;
        }
        public Label GetAlertTelNo1()
        {
            return vtxtTelNo_1;
        }
    }
}