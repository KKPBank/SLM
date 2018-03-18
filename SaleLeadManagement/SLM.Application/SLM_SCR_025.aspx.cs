using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Biz;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_025 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_025));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาเงื่อนไขการบันทึกผลการติดต่อ";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_025");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    Page.Form.DefaultButton = btnSearch.UniqueID;
                    InitialControl();
                    DoSearchActivityConfig(0);
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

            cmbLeadStatusSearch.DataSource = OptionBiz.GetOptionListForActivityConfig(AppConstant.OptionType.LeadStatus);
            cmbLeadStatusSearch.DataTextField = "TextField";
            cmbLeadStatusSearch.DataValueField = "ValueField";
            cmbLeadStatusSearch.DataBind();
            cmbLeadStatusSearch.Items.Insert(0, new ListItem("", ""));

            cmbLeadStatusAvailableSearch.DataSource = OptionBiz.GetOptionListForActivityConfig(AppConstant.OptionType.LeadStatus);
            cmbLeadStatusAvailableSearch.DataTextField = "TextField";
            cmbLeadStatusAvailableSearch.DataValueField = "ValueField";
            cmbLeadStatusAvailableSearch.DataBind();
            cmbLeadStatusAvailableSearch.Items.Insert(0, new ListItem("", ""));

            //Popup
//            cmbProductPopup.DataSource = ProductBiz.GetProductList();
            cmbProductPopup.DataSource = ProductBiz.GetProductListNew();
            cmbProductPopup.DataTextField = "TextField";
            cmbProductPopup.DataValueField = "ValueField";
            cmbProductPopup.DataBind();
            cmbProductPopup.Items.Insert(0, new ListItem("", ""));

            cmbLeadStatusPopup.DataSource = OptionBiz.GetOptionListForActivityConfig(AppConstant.OptionType.LeadStatus);
            cmbLeadStatusPopup.DataTextField = "TextField";
            cmbLeadStatusPopup.DataValueField = "ValueField";
            cmbLeadStatusPopup.DataBind();
            cmbLeadStatusPopup.Items.Insert(0, new ListItem("", ""));
        }

        private void BindListBox(ListBox listBox, List<ControlListData> source)
        {
            listBox.DataSource = source;
            listBox.DataTextField = "TextField";
            listBox.DataValueField = "ValueField";
            listBox.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchActivityConfig(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchActivityConfig(int pageIndex)
        {
            try
            {
                List<ActivityConfigData> list = ActivityConfigBiz.SearchActivityConfig(cmbProductSearch.SelectedItem.Value, cmbLeadStatusSearch.SelectedItem.Value, cmbActivityRightSearch.SelectedItem.Value, cmbLeadStatusAvailableSearch.SelectedItem.Value);
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
                DoSearchActivityConfig(pageControl.SelectedPageIndex);
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

        protected void btnAddCondition_Click(object sender, EventArgs e)
        {
            try
            {
                BindProductNew();
                cbEdit.Checked = false;
                cmbProductPopup.SelectedIndex = -1;
                cmbLeadStatusPopup.SelectedIndex = -1;
                cmbActivityRightPopup.SelectedIndex = -1;

                cmbProductPopup.Enabled = true;
                cmbLeadStatusPopup.Enabled = true;
                cmbActivityRightPopup.Enabled = true;

                lboxLeadStatusAll.Enabled = false;
                lboxLeadStatusSelected.Enabled = false;
                btnSelectAll.Enabled = false;
                btnSelect.Enabled = false;
                btnDeselect.Enabled = false;
                btnDeselectAll.Enabled = false;

                BindListBox(lboxLeadStatusAll, OptionBiz.GetOptionListForActivityConfig(AppConstant.OptionType.LeadStatus).OrderBy(p => p.TextField).ToList());
                lblLeadStatusAllTotal.Text = lboxLeadStatusAll.Items.Count.ToString();

                lboxLeadStatusSelected.Items.Clear();
                lblLeadStatusSelectedTotal.Text = lboxLeadStatusSelected.Items.Count.ToString();

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

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                lboxLeadStatusSelected.Items.AddRange(lboxLeadStatusAll.Items.OfType<ListItem>().ToArray());
                List<ControlListData> list = lboxLeadStatusSelected.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                BindListBox(lboxLeadStatusSelected, list);
                lblLeadStatusSelectedTotal.Text = lboxLeadStatusSelected.Items.Count.ToString();

                lboxLeadStatusAll.Items.Clear();
                lblLeadStatusAllTotal.Text = lboxLeadStatusAll.Items.Count.ToString();

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
                if (lboxLeadStatusAll.GetSelectedIndices().Count() > 0)
                {
                    List<ListItem> selectedList = new List<ListItem>();

                    List<int> indices = lboxLeadStatusAll.GetSelectedIndices().ToList();
                    foreach (int index in indices)
                    {
                        selectedList.Add(lboxLeadStatusAll.Items[index]);
                    }

                    //Remove selecteditems from lboxBranchAll
                    foreach (ListItem item in selectedList)
                    {
                        lboxLeadStatusAll.Items.Remove(item);
                    }

                    lboxLeadStatusSelected.Items.AddRange(selectedList.ToArray());
                    List<ControlListData> list = lboxLeadStatusSelected.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                    BindListBox(lboxLeadStatusSelected, list);

                    lblLeadStatusAllTotal.Text = lboxLeadStatusAll.Items.Count.ToString();
                    lblLeadStatusSelectedTotal.Text = lboxLeadStatusSelected.Items.Count.ToString();
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
                DoDeSelectAll();

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

        private void DoDeSelectAll()
        {
            try
            {
                lboxLeadStatusAll.Items.AddRange(lboxLeadStatusSelected.Items.OfType<ListItem>().ToArray());
                List<ControlListData> list = lboxLeadStatusAll.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                BindListBox(lboxLeadStatusAll, list);
                lblLeadStatusAllTotal.Text = lboxLeadStatusAll.Items.Count.ToString();

                lboxLeadStatusSelected.Items.Clear();
                lblLeadStatusSelectedTotal.Text = lboxLeadStatusSelected.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnDeselect_Click(object sender, EventArgs e)
        {
            try
            {
                if (lboxLeadStatusSelected.GetSelectedIndices().Count() > 0)
                {
                    List<ListItem> deSelectedList = new List<ListItem>();

                    List<int> indices = lboxLeadStatusSelected.GetSelectedIndices().ToList();
                    foreach (int index in indices)
                    {
                        deSelectedList.Add(lboxLeadStatusSelected.Items[index]);
                    }

                    //Remove deSelecteditems from lboxBranchSelected
                    foreach (ListItem item in deSelectedList)
                    {
                        lboxLeadStatusSelected.Items.Remove(item);
                    }

                    lboxLeadStatusAll.Items.AddRange(deSelectedList.ToArray());
                    List<ControlListData> list = lboxLeadStatusAll.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                    BindListBox(lboxLeadStatusAll, list);

                    lblLeadStatusAllTotal.Text = lboxLeadStatusAll.Items.Count.ToString();
                    lblLeadStatusSelectedTotal.Text = lboxLeadStatusSelected.Items.Count.ToString();
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
                    bool rightAdd = cmbActivityRightPopup.SelectedItem.Value == "1" ? true : false;

                    if (cbEdit.Checked)
                        ActivityConfigBiz.UpdateData(cmbProductPopup.SelectedItem.Value, cmbLeadStatusPopup.SelectedItem.Value, rightAdd, GetSelectedAvailableStatus(), HttpContext.Current.User.Identity.Name);
                    else
                        ActivityConfigBiz.InsertData(cmbProductPopup.SelectedItem.Value, cmbLeadStatusPopup.SelectedItem.Value, rightAdd, GetSelectedAvailableStatus(), HttpContext.Current.User.Identity.Name);

                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                    ClearPopupControl();
                    upPopup.Update();
                    mpePopup.Hide();

                    DoSearchActivityConfig(0);
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

        private List<string> GetSelectedAvailableStatus()
        {
            try
            {
                List<string> statusList = new List<string>();
                foreach (ListItem item in lboxLeadStatusSelected.Items)
                {
                    statusList.Add(item.Value);
                }
                return statusList;
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
                BindProduct();
                cbEdit.Checked = true;
                
                int index = Convert.ToInt32(((ImageButton)sender).CommandArgument);
                string productId = ((Label)gvResult.Rows[index].FindControl("lblProductId")).Text.Trim();
                string LeadStatusCode = ((Label)gvResult.Rows[index].FindControl("lblLeadStatusCode")).Text.Trim();
                string HaveRightAdd = ((Label)gvResult.Rows[index].FindControl("lblHaveRightAdd")).Text.Trim();

                cmbProductPopup.SelectedIndex = cmbProductPopup.Items.IndexOf(cmbProductPopup.Items.FindByValue(productId));
                cmbLeadStatusPopup.SelectedIndex = cmbLeadStatusPopup.Items.IndexOf(cmbLeadStatusPopup.Items.FindByValue(LeadStatusCode));
                cmbActivityRightPopup.SelectedIndex = cmbActivityRightPopup.Items.IndexOf(cmbActivityRightPopup.Items.FindByValue(HaveRightAdd));

                cmbProductPopup.Enabled = false;
                cmbLeadStatusPopup.Enabled = false;
                if (HaveRightAdd == "0")
                {
                    lboxLeadStatusAll.Enabled = false;
                    lboxLeadStatusSelected.Enabled = false;
                    btnSelectAll.Enabled = false;
                    btnSelect.Enabled = false;
                    btnDeselect.Enabled = false;
                    btnDeselectAll.Enabled = false;
                }
                else
                {
                    lboxLeadStatusAll.Enabled = true;
                    lboxLeadStatusSelected.Enabled = true;
                    btnSelectAll.Enabled = true;
                    btnSelect.Enabled = true;
                    btnDeselect.Enabled = true;
                    btnDeselectAll.Enabled = true;
                }

                SetAvailableStatusForEdit(productId, LeadStatusCode, HaveRightAdd);

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

        private void SetAvailableStatusForEdit(string productId, string leadStatus, string rightAdd)
        {
            try
            {
                var list = ActivityConfigBiz.SearchActivityConfig(productId, leadStatus, rightAdd, "");
                List<ControlListData> selectedList = list.Where(p => !string.IsNullOrEmpty(p.LeadAvailableStatusCode) && !string.IsNullOrEmpty(p.LeadAvailableStatusDesc)).Select(p => new ControlListData { TextField = p.LeadAvailableStatusDesc, ValueField = p.LeadAvailableStatusCode }).OrderBy(p => p.TextField).ToList();
                BindListBox(lboxLeadStatusSelected, selectedList);

                List<ControlListData> allAvailableStatusList = OptionBiz.GetOptionListForActivityConfig(AppConstant.OptionType.LeadStatus).OrderBy(p => p.TextField).ToList();
                foreach (ControlListData data in selectedList)
                {
                    ControlListData obj = allAvailableStatusList.Where(p => p.ValueField == data.ValueField).FirstOrDefault();
                    if (obj != null)
                        allAvailableStatusList.Remove(obj);
                }

                BindListBox(lboxLeadStatusAll, allAvailableStatusList);

                lblLeadStatusAllTotal.Text = lboxLeadStatusAll.Items.Count.ToString();
                lblLeadStatusSelectedTotal.Text = lboxLeadStatusSelected.Items.Count.ToString();
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
            cmbLeadStatusPopup.SelectedIndex = -1;
            cmbActivityRightPopup.SelectedIndex = -1;
            lboxLeadStatusAll.Items.Clear();
            lboxLeadStatusSelected.Items.Clear();

            cmbProductPopup.Enabled = true;
            cmbLeadStatusPopup.Enabled = true;
            cmbActivityRightPopup.Enabled = true;

            lboxLeadStatusAll.Enabled = true;
            lboxLeadStatusSelected.Enabled = true;
            btnSelectAll.Enabled = true;
            btnSelect.Enabled = true;
            btnDeselect.Enabled = true;
            btnDeselectAll.Enabled = true;

            alertActivityRightPopup.Text = "";
            alertLeadStatusPopup.Text = "";
            alertLeadStatusSelected.Text = "";
            alertProductPopup.Text = "";
        }

        private bool ValidateInput()
        {
            int i = 0;
            if (cmbProductPopup.SelectedItem.Value == "")
            {
                alertProductPopup.Text = "กรุณาเลือก ผลิตภัณฑ์/บริการ";
                i += 1;
            }
            else
                alertProductPopup.Text = "";

            if (cmbLeadStatusPopup.SelectedItem.Value == "")
            {
                alertLeadStatusPopup.Text = "กรุณาเลือก สถานะ Lead";
                i += 1;
            }
            else
                alertLeadStatusPopup.Text = "";

            if (cmbActivityRightPopup.SelectedItem.Value == "")
            {
                alertActivityRightPopup.Text = "กรุณาเลือก สิทธิ์การบันทึกผลการติดต่อ";
                i += 1;
            }
            else
                alertActivityRightPopup.Text = "";

            if (cmbActivityRightPopup.SelectedItem.Value == "1" && lboxLeadStatusSelected.Items.Count == 0)
            {
                alertLeadStatusSelected.Text = "กรุณาเลือก สถานะ";
                i += 1;
            }
            else
                alertLeadStatusSelected.Text = "";

            upPopup.Update();

            return i > 0 ? false : true;
        }

        protected void cmbActivityRightPopup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbActivityRightPopup.SelectedItem.Value == "1")
                {
                    lboxLeadStatusAll.Enabled = true;
                    lboxLeadStatusSelected.Enabled = true;
                    btnSelectAll.Enabled = true;
                    btnSelect.Enabled = true;
                    btnDeselect.Enabled = true;
                    btnDeselectAll.Enabled = true;
                }
                else
                {
                    DoDeSelectAll();
                    lboxLeadStatusAll.Enabled = false;
                    lboxLeadStatusSelected.Enabled = false;
                    btnSelectAll.Enabled = false;
                    btnSelect.Enabled = false;
                    btnDeselect.Enabled = false;
                    btnDeselectAll.Enabled = false;
                    alertLeadStatusSelected.Text = "";
                }

                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void BindProduct()
        {
            cmbProductPopup.DataSource = ProductBiz.GetProductList();
            cmbProductPopup.DataTextField = "TextField";
            cmbProductPopup.DataValueField = "ValueField";
            cmbProductPopup.DataBind();
            cmbProductPopup.Items.Insert(0, new ListItem("", ""));
        }

        private void BindProductNew()
        {
            cmbProductPopup.DataSource = ProductBiz.GetProductListNew();
            cmbProductPopup.DataTextField = "TextField";
            cmbProductPopup.DataValueField = "ValueField";
            cmbProductPopup.DataBind();
            cmbProductPopup.Items.Insert(0, new ListItem("", ""));
        }
    }
}