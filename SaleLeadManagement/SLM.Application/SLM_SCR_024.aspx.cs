using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource;
using SLM.Resource.Data;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_024 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_024));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาสิทธิ์การเข้าถึงข้อมูล";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_024");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    Page.Form.DefaultButton = btnSearch.UniqueID;
                    InitialControl();
                    DoSearchAccessRightList(0);
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

            cmbBranchSearch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.All);
            cmbBranchSearch.DataTextField = "TextField";
            cmbBranchSearch.DataValueField = "ValueField";
            cmbBranchSearch.DataBind();
            cmbBranchSearch.Items.Insert(0, new ListItem("", ""));

            cmbStaffTypeSearch.DataSource = StaffBiz.GetStaffTypeAllList();
            cmbStaffTypeSearch.DataTextField = "TextField";
            cmbStaffTypeSearch.DataValueField = "ValueField";
            cmbStaffTypeSearch.DataBind();
            cmbStaffTypeSearch.Items.Insert(0, new ListItem("", ""));

            //Popup
//            cmbProductPopup.DataSource = ProductBiz.GetProductList();
            cmbProductPopup.DataSource = ProductBiz.GetProductListNew();
            cmbProductPopup.DataTextField = "TextField";
            cmbProductPopup.DataValueField = "ValueField";
            cmbProductPopup.DataBind();
            cmbProductPopup.Items.Insert(0, new ListItem("", ""));

            //cmbCampaignPopup.DataSource = CampaignBiz.GetCampaignList("");
			cmbCampaignPopup.DataSource = CampaignBiz.GetCampaignListNew("");
            cmbCampaignPopup.DataTextField = "TextField";
            cmbCampaignPopup.DataValueField = "ValueField";
            cmbCampaignPopup.DataBind();
            cmbCampaignPopup.Items.Insert(0, new ListItem("", ""));

            cmbStaffTypePopup.DataSource = StaffBiz.GetStaffTypeAllList();
            cmbStaffTypePopup.DataTextField = "TextField";
            cmbStaffTypePopup.DataValueField = "ValueField";
            cmbStaffTypePopup.DataBind();
            cmbStaffTypePopup.Items.Insert(0, new ListItem("", ""));
        }

        private void BindListBox(ListBox listBox, List<ControlListData> source)
        {
            listBox.DataSource = source;
            listBox.DataTextField = "TextField";
            listBox.DataValueField = "ValueField";
            listBox.DataBind();
        }

        protected void rbProduct_CheckedChanged(object sender, EventArgs e)
        {
            CheckSearchRadioCondition();
        }

        protected void rbCampaign_CheckedChanged(object sender, EventArgs e)
        {
            CheckSearchRadioCondition();
        }

        private void CheckSearchRadioCondition()
        {
            if (rbProduct.Checked)
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchAccessRightList(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchAccessRightList(int pageIndex)
        {
            try
            {
                List<AccessRightData> list = AccessRightBiz.SearchAccessRight(cmbProductSearch.SelectedItem.Value, cmbCampaignSearch.SelectedItem.Value, cmbStaffTypeSearch.SelectedItem.Value, cmbBranchSearch.SelectedItem.Value);
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
                DoSearchAccessRightList(pageControl.SelectedPageIndex);
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

        protected void btnAddConfig_Click(object sender, EventArgs e)
        {
            try
            {
                BindProductAndCampaignListNew();
                cbEdit.Checked = false;
                cmbProductPopup.SelectedIndex = -1;
                cmbCampaignPopup.SelectedIndex = -1;
                cmbStaffTypePopup.SelectedIndex = -1;

                rbProductPopup.Enabled = true;
                rbProductPopup.Checked = true;
                cmbProductPopup.Enabled = true;
                rbCampaignPopup.Enabled = true;
                rbCampaignPopup.Checked = false;
                cmbCampaignPopup.Enabled = false;
                cmbStaffTypePopup.Enabled = true;
                lblProductStar.Visible = true;
                lblCampaignStar.Visible = false;

                //BindListBox(lboxBranchAll, BranchBiz.GetBranchList(SLMConstant.Branch.All));
                //lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();

                //lboxBranchSelected.Items.Clear();
                //lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();

                lboxBranchAll.Items.Clear();
                lblBranchAllTotal.Text = "0";
                lboxBranchSelected.Items.Clear();
                lblBranchSelectedTotal.Text = "0";

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

        protected void cmbStaffTypePopup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindListBox(lboxBranchAll, BranchBiz.GetBranchListByRole(SLMConstant.Branch.All, cmbStaffTypePopup.SelectedItem.Value));
                lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();

                lboxBranchSelected.Items.Clear();
                lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();

                upPopupBranchSection.Update();
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                lboxBranchSelected.Items.AddRange(lboxBranchAll.Items.OfType<ListItem>().ToArray());
                List<ControlListData> list = lboxBranchSelected.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                BindListBox(lboxBranchSelected, list);
                lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();

                lboxBranchAll.Items.Clear();
                lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();

                //upPopup.Update();
                upPopupBranchSection.Update();
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                if (lboxBranchAll.GetSelectedIndices().Count() > 0)
                {
                    List<ListItem> selectedList = new List<ListItem>();

                    List<int> indices = lboxBranchAll.GetSelectedIndices().ToList();
                    foreach (int index in indices)
                    {
                        selectedList.Add(lboxBranchAll.Items[index]);
                    }

                    //Remove selecteditems from lboxBranchAll
                    foreach (ListItem item in selectedList)
                    {
                        lboxBranchAll.Items.Remove(item);
                    }

                    lboxBranchSelected.Items.AddRange(selectedList.ToArray());
                    List<ControlListData> list = lboxBranchSelected.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                    BindListBox(lboxBranchSelected, list);

                    lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();
                    lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();
                }

                //upPopup.Update();
                upPopupBranchSection.Update();
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnDeselectAll_Click(object sender, EventArgs e)
        {
            try
            {
                lboxBranchAll.Items.AddRange(lboxBranchSelected.Items.OfType<ListItem>().ToArray());
                List<ControlListData> list = lboxBranchAll.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                BindListBox(lboxBranchAll, list);
                lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();

                lboxBranchSelected.Items.Clear();
                lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();

                //upPopup.Update();
                upPopupBranchSection.Update();
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnDeselect_Click(object sender, EventArgs e)
        {
            try
            {
                if (lboxBranchSelected.GetSelectedIndices().Count() > 0)
                {
                    List<ListItem> deSelectedList = new List<ListItem>();

                    List<int> indices = lboxBranchSelected.GetSelectedIndices().ToList();
                    foreach (int index in indices)
                    {
                        deSelectedList.Add(lboxBranchSelected.Items[index]);
                    }

                    //Remove deSelecteditems from lboxBranchSelected
                    foreach (ListItem item in deSelectedList)
                    {
                        lboxBranchSelected.Items.Remove(item);
                    }

                    lboxBranchAll.Items.AddRange(deSelectedList.ToArray());
                    List<ControlListData> list = lboxBranchAll.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                    BindListBox(lboxBranchAll, list);

                    lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();
                    lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();
                }

                //upPopup.Update();
                upPopupBranchSection.Update();
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }


        #endregion

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    int staffTypeId = Convert.ToInt32(cmbStaffTypePopup.SelectedItem.Value);

                    if (cbEdit.Checked)
                        AccessRightBiz.UpdateData(cmbProductPopup.SelectedItem.Value, cmbCampaignPopup.SelectedItem.Value, staffTypeId, GetSelectedBranchCode(), HttpContext.Current.User.Identity.Name);
                    else
                        AccessRightBiz.InsertData(cmbProductPopup.SelectedItem.Value, cmbCampaignPopup.SelectedItem.Value, staffTypeId, GetSelectedBranchCode(), HttpContext.Current.User.Identity.Name);
                    
                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                    ClearPopupControl();
                    upPopup.Update();
                    mpePopup.Hide();

                    DoSearchAccessRightList(0);
                }
                else
                    mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private List<string> GetSelectedBranchCode()
        {
            try
            {
                List<string> branchCodeList = new List<string>();
                foreach (ListItem item in lboxBranchSelected.Items)
                {
                    branchCodeList.Add(item.Value);
                }
                return branchCodeList;
            }
            catch
            {
                throw;
            }
        }

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                BindProductAndCampaignList();
                cbEdit.Checked = true;

                int index = Convert.ToInt32(((ImageButton)sender).CommandArgument);
                string productId = ((Label)gvResult.Rows[index].FindControl("lblProductId")).Text.Trim();
                string campaignId = ((Label)gvResult.Rows[index].FindControl("lblCampaignId")).Text.Trim();
                string staffTypeId = ((Label)gvResult.Rows[index].FindControl("lblStaffTypeId")).Text.Trim();

                if (!string.IsNullOrEmpty(productId))
                {
                    rbProductPopup.Checked = true;
                    lblProductStar.Visible = true;
                    lblCampaignStar.Visible = false;
                    cmbProductPopup.SelectedIndex = cmbProductPopup.Items.IndexOf(cmbProductPopup.Items.FindByValue(productId));
                }
                else if (!string.IsNullOrEmpty(campaignId))
                {
                    rbCampaignPopup.Checked = true;
                    lblProductStar.Visible = false;
                    lblCampaignStar.Visible = true;
                    cmbCampaignPopup.SelectedIndex = cmbCampaignPopup.Items.IndexOf(cmbCampaignPopup.Items.FindByValue(campaignId));
                }

                cmbStaffTypePopup.SelectedIndex = cmbStaffTypePopup.Items.IndexOf(cmbStaffTypePopup.Items.FindByValue(staffTypeId));

                rbProductPopup.Enabled = false;
                rbCampaignPopup.Enabled = false;
                cmbProductPopup.Enabled = false;
                cmbCampaignPopup.Enabled = false;
                cmbStaffTypePopup.Enabled = false;

                SetBranchForEdit(productId, campaignId, staffTypeId);

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

        private void SetBranchForEdit(string productId, string campaignId, string staffTypeId)
        {
            try
            {
                var list = AccessRightBiz.SearchAccessRight(productId, campaignId, staffTypeId, "");
                List<ControlListData> selectedList = list.Select(p => new ControlListData { TextField = p.BranchName, ValueField = p.BranchCode }).OrderBy(p => p.TextField).ToList();
                BindListBox(lboxBranchSelected, selectedList);

                List<ControlListData> allBranchList = BranchBiz.GetBranchListByRole(SLMConstant.Branch.All, staffTypeId);
                foreach (ControlListData data in selectedList)
                {
                    ControlListData obj = allBranchList.Where(p => p.ValueField == data.ValueField).FirstOrDefault();
                    if (obj != null)
                        allBranchList.Remove(obj);
                }

                BindListBox(lboxBranchAll, allBranchList);

                lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();
                lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();
            }
            catch
            {
                throw;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPopupControl();
            upPopup.Update();
            mpePopup.Hide();
        }

        private void ClearPopupControl()
        {
            cbEdit.Checked = false;
            cmbProductPopup.SelectedIndex = -1;
            cmbCampaignPopup.SelectedIndex = -1;
            cmbStaffTypePopup.SelectedIndex = -1;
            rbProductPopup.Checked = true;
            rbCampaignPopup.Checked = false;
            lboxBranchAll.Items.Clear();
            lboxBranchSelected.Items.Clear();

            rbProductPopup.Enabled = true;
            rbCampaignPopup.Enabled = true;
            cmbProductPopup.Enabled = true;
            cmbCampaignPopup.Enabled = true;
            cmbStaffTypePopup.Enabled = true;
            lblProductStar.Visible = true;
            lblCampaignStar.Visible = false;

            alertProductCampaign.Text = "";
            alertStaffTypePopup.Text = "";
            alertBranchSelected.Text = "";
        }

        private bool ValidateInput()
        {
            int i = 0;
            if (cmbProductPopup.SelectedItem.Value == "" && cmbCampaignPopup.SelectedItem.Value == "")
            {
                alertProductCampaign.Text = "กรุณาเลือก ผลิตภัณฑ์หรือแคมเปญ";
                i += 1;
            }
            else
                alertProductCampaign.Text = "";

            if (cmbStaffTypePopup.SelectedItem.Value == "")
            {
                alertStaffTypePopup.Text = "กรุณาเลือก Role";
                i += 1;
            }
            else
                alertStaffTypePopup.Text = "";

            if (cbEdit.Checked == false && lboxBranchSelected.Items.Count == 0)
            {
                alertBranchSelected.Text = "กรุณาเลือก สาขา";
                i += 1;
            }
            else
                alertBranchSelected.Text = "";

            upPopup.Update();

            return i > 0 ? false : true;
        }

        private void BindProductAndCampaignList()
        {
            cmbProductPopup.DataSource = ProductBiz.GetProductList();
            cmbProductPopup.DataTextField = "TextField";
            cmbProductPopup.DataValueField = "ValueField";
            cmbProductPopup.DataBind();
            cmbProductPopup.Items.Insert(0, new ListItem("", ""));

            cmbCampaignPopup.DataSource = CampaignBiz.GetCampaignList("");
            cmbCampaignPopup.DataTextField = "TextField";
            cmbCampaignPopup.DataValueField = "ValueField";
            cmbCampaignPopup.DataBind();
            cmbCampaignPopup.Items.Insert(0, new ListItem("", ""));
        }

        private void BindProductAndCampaignListNew()
        {
            cmbProductPopup.DataSource = ProductBiz.GetProductListNew();
            cmbProductPopup.DataTextField = "TextField";
            cmbProductPopup.DataValueField = "ValueField";
            cmbProductPopup.DataBind();
            cmbProductPopup.Items.Insert(0, new ListItem("", ""));

            cmbCampaignPopup.DataSource = CampaignBiz.GetCampaignListNew("");
            cmbCampaignPopup.DataTextField = "TextField";
            cmbCampaignPopup.DataValueField = "ValueField";
            cmbCampaignPopup.DataBind();
            cmbCampaignPopup.Items.Insert(0, new ListItem("", ""));
        }
    }
}