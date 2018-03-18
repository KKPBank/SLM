using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;

namespace SLM.Application
{
	public partial class SLM_SCR_030 : System.Web.UI.Page
	{
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_030));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาเงื่อนไขการจ่ายงาน";
            //Page.Form.DefaultButton = btnMainSearch.UniqueID;
        }

		protected void Page_Load(object sender, EventArgs e)
		{
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_030");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    InitialControl();

                    DoSearchConfigCustomer(0);      //Gridview Assign Config Customer
                    DoSearchConfigStaff(0);         //Gridview Assign Config Staff
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
            //Customer Section
            AppUtil.SetIntTextBox(txtEditSeq);

            //Staff Section
            AppUtil.SetIntTextBox(txtEditSeq_Staff);

            cmbProductSearch.DataSource = ProductBiz.GetProductList();
            cmbProductSearch.DataTextField = "TextField";
            cmbProductSearch.DataValueField = "ValueField";
            cmbProductSearch.DataBind();
            cmbProductSearch.Items.Insert(0, new ListItem("", ""));

            cmbGradeSearch.DataSource = new CustomerGradeBiz().GetCustomerGradeList();
            cmbGradeSearch.DataTextField = "TextField";
            cmbGradeSearch.DataValueField = "ValueField";
            cmbGradeSearch.DataBind();
            cmbGradeSearch.Items.Insert(0, new ListItem("", ""));

            cmbAssignTypeSearch.DataSource = new AssignTypeBiz().GetAssignTypeList();
            cmbAssignTypeSearch.DataTextField = "TextField";
            cmbAssignTypeSearch.DataValueField = "ValueField";
            cmbAssignTypeSearch.DataBind();
            cmbAssignTypeSearch.Items.Insert(0, new ListItem("", ""));

            //Officer Section
            cmbProductSearchStaff.DataSource = ProductBiz.GetProductList();
            cmbProductSearchStaff.DataTextField = "TextField";
            cmbProductSearchStaff.DataValueField = "ValueField";
            cmbProductSearchStaff.DataBind();
            cmbProductSearchStaff.Items.Insert(0, new ListItem("", ""));

            cmbGradeSearchStaff.DataSource = new CustomerGradeBiz().GetCustomerGradeList();
            cmbGradeSearchStaff.DataTextField = "TextField";
            cmbGradeSearchStaff.DataValueField = "ValueField";
            cmbGradeSearchStaff.DataBind();
            cmbGradeSearchStaff.Items.Insert(0, new ListItem("", ""));

            cmbAssignTypeSearchStaff.DataSource = new AssignTypeBiz().GetAssignTypeList();
            cmbAssignTypeSearchStaff.DataTextField = "TextField";
            cmbAssignTypeSearchStaff.DataValueField = "ValueField";
            cmbAssignTypeSearchStaff.DataBind();
            cmbAssignTypeSearchStaff.Items.Insert(0, new ListItem("", ""));

            cmbTeamTeleSearchStaff.DataSource = new TeamTelesalesBiz().GetTeamTelesalesList();
            cmbTeamTeleSearchStaff.DataTextField = "TextField";
            cmbTeamTeleSearchStaff.DataValueField = "ValueField";
            cmbTeamTeleSearchStaff.DataBind();
            cmbTeamTeleSearchStaff.Items.Insert(0, new ListItem("", ""));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchConfigCustomer(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchConfigCustomer(int pageIndex)
        {
            try
            {
                AssignConditionCustomerBiz biz = new AssignConditionCustomerBiz();
                List<AssignConditionData> list = biz.SearchAssignConditionCustomer(cmbProductSearch.SelectedItem.Value, cmbGradeSearch.SelectedItem.Value, cmbAssignTypeSearch.SelectedItem.Value);
                BindGridview_AddConfigCustomer(pcTop, list.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                cmbProductSearch.SelectedIndex = -1;
                cmbGradeSearch.SelectedIndex = -1;
                cmbAssignTypeSearch.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnAddConfigCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                cmbAddProduct_Cus.DataSource = ProductBiz.GetProductList();
                cmbAddProduct_Cus.DataTextField = "TextField";
                cmbAddProduct_Cus.DataValueField = "ValueField";
                cmbAddProduct_Cus.DataBind();
                cmbAddProduct_Cus.Items.Insert(0, new ListItem("", ""));

                //cmbAddCustomerGrade_Cus.DataSource = new CustomerGradeBiz().GetCustomerGradeList();
                //cmbAddCustomerGrade_Cus.DataTextField = "TextField";
                //cmbAddCustomerGrade_Cus.DataValueField = "ValueField";
                //cmbAddCustomerGrade_Cus.DataBind();
                //cmbAddCustomerGrade_Cus.Items.Insert(0, new ListItem("", ""));

                //cmbAddAssignType_Cus.DataSource = new AssignTypeBiz().GetAssignTypeList();
                //cmbAddAssignType_Cus.DataTextField = "TextField";
                //cmbAddAssignType_Cus.DataValueField = "ValueField";
                //cmbAddAssignType_Cus.DataBind();
                //cmbAddAssignType_Cus.Items.Insert(0, new ListItem("", ""));

                mpePopupAddConfigCustomer.Show();
                upPopupAddConfigCustomer.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbAddProduct_Cus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbAddProduct_Cus.SelectedItem.Value != "")
                {
                    cmbAddCustomerGrade_Cus.DataSource = new CustomerGradeBiz().GetCustomerGradeList("C", cmbAddProduct_Cus.SelectedItem.Value);
                    cmbAddCustomerGrade_Cus.DataTextField = "TextField";
                    cmbAddCustomerGrade_Cus.DataValueField = "ValueField";
                    cmbAddCustomerGrade_Cus.DataBind();
                    cmbAddCustomerGrade_Cus.Items.Insert(0, new ListItem("", ""));

                    cmbAddAssignType_Cus.Items.Clear();
                }
                else
                {
                    cmbAddCustomerGrade_Cus.Items.Clear();
                    cmbAddAssignType_Cus.Items.Clear();
                }

                mpePopupAddConfigCustomer.Show();
                upPopupAddConfigCustomer.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbAddCustomerGrade_Cus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbAddCustomerGrade_Cus.SelectedItem.Value != "")
                {
                    cmbAddAssignType_Cus.DataSource = new AssignTypeBiz().GetAssignTypeList("C", cmbAddProduct_Cus.SelectedItem.Value, decimal.Parse(cmbAddCustomerGrade_Cus.SelectedItem.Value));
                    cmbAddAssignType_Cus.DataTextField = "TextField";
                    cmbAddAssignType_Cus.DataValueField = "ValueField";
                    cmbAddAssignType_Cus.DataBind();
                    cmbAddAssignType_Cus.Items.Insert(0, new ListItem("", ""));
                }
                else
                {
                    //เนื่องจากมีการ convert to decimal
                    cmbAddAssignType_Cus.Items.Clear();
                }

                mpePopupAddConfigCustomer.Show();
                upPopupAddConfigCustomer.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbEditConfigCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                cmbEditConditionField.DataSource = new ConditionFieldBiz().GetConditionFieldList("C");  //C=Customer
                cmbEditConditionField.DataTextField = "TextField";
                cmbEditConditionField.DataValueField = "ValueField";
                cmbEditConditionField.DataBind();
                cmbEditConditionField.Items.Insert(0, new ListItem("", ""));

                int index = int.Parse(((ImageButton)sender).CommandArgument);
                txtEditProductName.Text = ((Label)gvAddConfigCustomer.Rows[index].FindControl("lblProductName")).Text.Trim();
                txtEditCustomerGrade.Text = ((Label)gvAddConfigCustomer.Rows[index].FindControl("lblGradeName")).Text.Trim();
                txtEditAssignType.Text = ((Label)gvAddConfigCustomer.Rows[index].FindControl("lblAssignTypeName")).Text.Trim();
                txtEditAssignConditionCusId.Text = ((Label)gvAddConfigCustomer.Rows[index].FindControl("lblAssignConditionCusId")).Text.Trim();

                DoSearchEditConfigCustomer(0);

                mpePopupEditConfigCustomer.Show();
                upPopupEditConfigCustomer.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbDeleteMainConfigCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                decimal assignConditionCusId = decimal.Parse(((ImageButton)sender).CommandArgument);
                new AssignConditionCustomerBiz().DeleteData(assignConditionCusId);

                DoSearchConfigCustomer(0);
                AppUtil.ClientAlert(Page, "ลบข้อมูลเรียบร้อย");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchEditConfigCustomer(int pageIndex)
        {
            try
            {
                SortConditionCustomerBiz biz = new SortConditionCustomerBiz();
                List<SortConditionCustomerData> list = biz.GetSortConditionCustomerList(txtEditAssignConditionCusId.Text.Trim());
                BindGridview_EditConfigCustomer(pcTopEditConfigCustomer, list.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        #region Page Control Gridview EditConfigCustomer

        private void BindGridview_EditConfigCustomer(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvEditConfigCustomer);
            pageControl.Update(items, pageIndex);
            pageControl.GenerateRecordNumber(1, pageIndex);
            upEditConfigCusButton_Inner.Update();
        }

        protected void PageSearchChange_EditConfigCustomer(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchEditConfigCustomer(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        //เพิ่มเงื่อนไขการจ่ายงานส่วนลูกค้า
        #region Popup Add Config Customer

        private void ClearPopupAddConfigCustomer()
        {
            lblAlertAddProduct_Cus.Text = "";
            lblAlertAddCustomerGrade_Cus.Text = "";
            lblAlertAddAssignType_Cus.Text = "";

            cmbAddCustomerGrade_Cus.Items.Clear();
            cmbAddAssignType_Cus.Items.Clear();
        }

        protected void btnCancelPopupAddConfigCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                ClearPopupAddConfigCustomer();
                mpePopupAddConfigCustomer.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSavePopupAddConfigCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidatePopupAddConfigCustomer())
                {
                    AssignConditionCustomerBiz biz = new AssignConditionCustomerBiz();
                    if (biz.ValidateData(cmbAddProduct_Cus.SelectedItem.Value, decimal.Parse(cmbAddCustomerGrade_Cus.SelectedItem.Value), decimal.Parse(cmbAddAssignType_Cus.SelectedItem.Value)))
                    {
                        biz.InsertData(cmbAddProduct_Cus.SelectedItem.Value, cmbAddCustomerGrade_Cus.SelectedItem.Value, cmbAddAssignType_Cus.SelectedItem.Value, HttpContext.Current.User.Identity.Name.ToLower());

                        ClearPopupAddConfigCustomer();
                        mpePopupAddConfigCustomer.Hide();

                        DoSearchConfigCustomer(0);
                        AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
                    }
                    else
                    {
                        AppUtil.ClientAlert(Page, biz.ErrorMessage);
                        mpePopupAddConfigCustomer.Show();
                    }
                }
                else
                {
                    mpePopupAddConfigCustomer.Show();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidatePopupAddConfigCustomer()
        {
            if (cmbAddProduct_Cus.SelectedItem.Value == "" || cmbAddCustomerGrade_Cus.SelectedItem.Value == "" || cmbAddAssignType_Cus.SelectedItem.Value == "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน ก่อนการบันทึก");
                return false;
            }

            return true;
        }

        #endregion

        #region Page Control Gridview AddConfigCustomer

        private void BindGridview_AddConfigCustomer(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvAddConfigCustomer);
            pageControl.Update(items, pageIndex);
            pageControl.GenerateRecordNumber(1, pageIndex);
            upResultCustomer.Update();
        }

        protected void PageSearchChange_AddConfigCustomer(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchConfigCustomer(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        //แก้ไขเงื่อนไขการจ่ายงานส่วนลูกค้า
        #region Popup Edit Config Customer

        protected void btnEditConfigCusCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ClearPopupEditConfigCustomer();
                mpePopupEditConfigCustomer.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ClearPopupEditConfigCustomer()
        {
            txtEditAssignConditionCusId.Text = "";
            txtEditProductName.Text = "";
            txtEditCustomerGrade.Text = "";
            txtEditAssignType.Text = "";

            lblAlertEditConditionField.Text = "";
            lblAlertEditSeq.Text = "";

            cmbEditConditionField.SelectedIndex = -1;
            cmbEditOrder.SelectedIndex = -1;
            txtEditSeq.Text = "";
        }

        protected void btnEditConfigCusSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateEditConfigCustomer())
                {
                    SortConditionCustomerBiz biz = new SortConditionCustomerBiz();
                    biz.InsertData(txtEditAssignConditionCusId.Text, cmbEditConditionField.SelectedItem.Value, cmbEditOrder.SelectedItem.Value, txtEditSeq.Text.Trim(), HttpContext.Current.User.Identity.Name.ToLower());

                    cmbEditConditionField.SelectedIndex = -1;
                    cmbEditOrder.SelectedIndex = -1;
                    txtEditSeq.Text = "";

                    DoSearchEditConfigCustomer(0);

                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                mpePopupEditConfigCustomer.Show();
                upEditConfigCusButton_Inner.Update();
            }
        }

        private bool ValidateEditConfigCustomer()
        {
            if (cmbEditConditionField.SelectedItem.Value == "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน ก่อนการบันทึก");
                return false;
            }

            if (txtEditSeq.Text.Trim() == "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน ก่อนการบันทึก");
                return false;
            }
            else
            { 
                int result;
                if (!int.TryParse(txtEditSeq.Text.Trim(), out result))
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุ sequence เป็นตัวเลขที่มากกว่าศูนย์เท่านั้น");
                    return false;
                }
                if (result <= 0)
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุ sequence เป็นตัวเลขที่มากกว่าศูนย์เท่านั้น");
                    return false;
                }
            }

            return true;
        }

        protected void imbDeleteConfigCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                decimal sortConditionCusId = decimal.Parse(((ImageButton)sender).CommandArgument);
                new SortConditionCustomerBiz().DeleteData(sortConditionCusId);

                DoSearchEditConfigCustomer(0);
                AppUtil.ClientAlert(Page, "ลบข้อมูลเรียบร้อย");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        //====================================== Officer Section =======================================================================

        protected void btnSearchStaff_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchConfigStaff(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchConfigStaff(int pageIndex)
        {
            try
            {
                AssignConditionStaffBiz biz = new AssignConditionStaffBiz();
                List<AssignConditionData> list = biz.SearchAssignConditionStaff(cmbProductSearchStaff.SelectedItem.Value, cmbGradeSearchStaff.SelectedItem.Value, cmbAssignTypeSearchStaff.SelectedItem.Value, cmbTeamTeleSearchStaff.SelectedItem.Value);
                BindGridview_AddConfigStaff(pcTopAddConfigStaff, list.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        protected void btnClearStaff_Click(object sender, EventArgs e)
        {
            try
            {
                cmbProductSearchStaff.SelectedIndex = -1;
                cmbGradeSearchStaff.SelectedIndex = -1;
                cmbAssignTypeSearchStaff.SelectedIndex = -1;
                cmbTeamTeleSearchStaff.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnAddConfigStaff_Click(object sender, EventArgs e)
        {
            try
            {
                cmbAddProduct_Staff.DataSource = ProductBiz.GetProductList();
                cmbAddProduct_Staff.DataTextField = "TextField";
                cmbAddProduct_Staff.DataValueField = "ValueField";
                cmbAddProduct_Staff.DataBind();
                cmbAddProduct_Staff.Items.Insert(0, new ListItem("", ""));

                //cmbAddGrade_Staff.DataSource = new CustomerGradeBiz().GetCustomerGradeList();
                //cmbAddGrade_Staff.DataTextField = "TextField";
                //cmbAddGrade_Staff.DataValueField = "ValueField";
                //cmbAddGrade_Staff.DataBind();
                //cmbAddGrade_Staff.Items.Insert(0, new ListItem("", ""));

                //cmbAddAssignType_Staff.DataSource = new AssignTypeBiz().GetAssignTypeList();
                //cmbAddAssignType_Staff.DataTextField = "TextField";
                //cmbAddAssignType_Staff.DataValueField = "ValueField";
                //cmbAddAssignType_Staff.DataBind();
                //cmbAddAssignType_Staff.Items.Insert(0, new ListItem("", ""));

                cmbAddTeamTeles_Staff.DataSource = new TeamTelesalesBiz().GetTeamTelesalesList();
                cmbAddTeamTeles_Staff.DataTextField = "TextField";
                cmbAddTeamTeles_Staff.DataValueField = "ValueField";
                cmbAddTeamTeles_Staff.DataBind();
                cmbAddTeamTeles_Staff.Items.Insert(0, new ListItem("", ""));

                mpePopupAddConfigStaff.Show();
                upPopupAddConfigStaff.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbAddProduct_Staff_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbAddProduct_Staff.SelectedItem.Value != "")
                {
                    cmbAddGrade_Staff.DataSource = new CustomerGradeBiz().GetCustomerGradeList("S", cmbAddProduct_Staff.SelectedItem.Value);
                    cmbAddGrade_Staff.DataTextField = "TextField";
                    cmbAddGrade_Staff.DataValueField = "ValueField";
                    cmbAddGrade_Staff.DataBind();
                    cmbAddGrade_Staff.Items.Insert(0, new ListItem("", ""));

                    cmbAddAssignType_Staff.Items.Clear();
                }
                else
                {
                    cmbAddGrade_Staff.Items.Clear();
                    cmbAddAssignType_Staff.Items.Clear();
                }

                mpePopupAddConfigStaff.Show();
                upPopupAddConfigStaff.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbAddGrade_Staff_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbAddGrade_Staff.SelectedItem.Value != "")
                {
                    cmbAddAssignType_Staff.DataSource = new AssignTypeBiz().GetAssignTypeList("S", cmbAddProduct_Staff.SelectedItem.Value, decimal.Parse(cmbAddGrade_Staff.SelectedItem.Value));
                    cmbAddAssignType_Staff.DataTextField = "TextField";
                    cmbAddAssignType_Staff.DataValueField = "ValueField";
                    cmbAddAssignType_Staff.DataBind();
                    cmbAddAssignType_Staff.Items.Insert(0, new ListItem("", ""));
                }
                else
                {
                    //เนื่องจากมีการ convert to decimal
                    cmbAddAssignType_Staff.Items.Clear();
                }

                mpePopupAddConfigStaff.Show();
                upPopupAddConfigStaff.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbEditConfigStaff_Click(object sender, EventArgs e)
        {
            try
            {
                cmbEditConditionField_Staff.DataSource = new ConditionFieldBiz().GetConditionFieldList("S");  //S=Staff
                cmbEditConditionField_Staff.DataTextField = "TextField";
                cmbEditConditionField_Staff.DataValueField = "ValueField";
                cmbEditConditionField_Staff.DataBind();
                cmbEditConditionField_Staff.Items.Insert(0, new ListItem("", ""));

                int index = int.Parse(((ImageButton)sender).CommandArgument);
                txtEditProductName_Staff.Text = ((Label)gvAddConfigStaff.Rows[index].FindControl("lblProductName")).Text.Trim();
                txtEditGradeName_Staff.Text = ((Label)gvAddConfigStaff.Rows[index].FindControl("lblGradeName")).Text.Trim();
                txtEditAssignTypeName_Staff.Text = ((Label)gvAddConfigStaff.Rows[index].FindControl("lblAssignTypeName")).Text.Trim();
                txtEditAssignConditionStaffId.Text = ((Label)gvAddConfigStaff.Rows[index].FindControl("lblAssignConditionStaffId")).Text.Trim();
                txtEditTeamTeles_Staff.Text = ((Label)gvAddConfigStaff.Rows[index].FindControl("lblTeamTelesalesName")).Text.Trim();

                DoSearchEditConfigStaff(0);

                mpePopupEditConfigStaff.Show();
                upPopupEditConfigStaff.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbDeleteMainConfigStaff_Click(object sender, EventArgs e)
        {
            try
            {
                decimal assignConditionStaffId = decimal.Parse(((ImageButton)sender).CommandArgument);
                new AssignConditionStaffBiz().DeleteData(assignConditionStaffId);

                DoSearchConfigStaff(0);
                AppUtil.ClientAlert(Page, "ลบข้อมูลเรียบร้อย");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchEditConfigStaff(int pageIndex)
        {
            try
            {
                SortConditionStaffBiz biz = new SortConditionStaffBiz();
                List<SortConditionStaffData> list = biz.GetSortConditionStaffList(txtEditAssignConditionStaffId.Text.Trim());
                BindGridview_EditConfigStaff(pcTopEditConfigStaff, list.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        //เพิ่มเงื่อนไขการจ่ายงานส่วนลูกค้า
        #region Popup Add Config Staff

        private void ClearPopupAddConfigStaff()
        {
            lblAlertAddProduct_Staff.Text = "";
            lblAlertAddGrade_Staff.Text = "";
            lblAlertAddAssignType_Staff.Text = "";
            lblAlertAddTeamTeles_Staff.Text = "";

            cmbAddGrade_Staff.Items.Clear();
            cmbAddAssignType_Staff.Items.Clear();
        }

        protected void btnCancelPopupAddConfigStaff_Click(object sender, EventArgs e)
        {
            try
            {
                ClearPopupAddConfigStaff();
                mpePopupAddConfigStaff.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSavePopupAddConfigStaff_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidatePopupAddConfigStaff())
                {
                    AssignConditionStaffBiz biz = new AssignConditionStaffBiz();
                    if (biz.ValidateData(cmbAddProduct_Staff.SelectedItem.Value, decimal.Parse(cmbAddGrade_Staff.SelectedItem.Value), decimal.Parse(cmbAddAssignType_Staff.SelectedItem.Value), decimal.Parse(cmbAddTeamTeles_Staff.SelectedItem.Value)))
                    {
                        biz.InsertData(cmbAddProduct_Staff.SelectedItem.Value, cmbAddGrade_Staff.SelectedItem.Value, cmbAddAssignType_Staff.SelectedItem.Value, cmbAddTeamTeles_Staff.SelectedItem.Value, HttpContext.Current.User.Identity.Name.ToLower());

                        ClearPopupAddConfigStaff();
                        mpePopupAddConfigStaff.Hide();

                        DoSearchConfigStaff(0);
                        AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
                    }
                    else
                    {
                        AppUtil.ClientAlert(Page, biz.ErrorMessage);
                        mpePopupAddConfigStaff.Show();
                    }
                }
                else
                {
                    mpePopupAddConfigStaff.Show();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidatePopupAddConfigStaff()
        {
            if (cmbAddProduct_Staff.SelectedItem.Value == "" || cmbAddGrade_Staff.SelectedItem.Value == "" || cmbAddAssignType_Staff.SelectedItem.Value == "" || cmbAddTeamTeles_Staff.SelectedItem.Value == "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน ก่อนการบันทึก");
                return false;
            }

            return true;

            //int count = 0;
            //if (cmbAddProduct_Staff.SelectedItem.Value == "")
            //{
            //    lblAlertAddProduct_Staff.Text = "กรุณาระบุข้อมูล";
            //    count += 1;
            //}
            //else
            //    lblAlertAddProduct_Staff.Text = "";

            //if (cmbAddGrade_Staff.SelectedItem.Value == "")
            //{
            //    lblAlertAddGrade_Staff.Text = "กรุณาระบุข้อมูล";
            //    count += 1;
            //}
            //else
            //    lblAlertAddGrade_Staff.Text = "";

            //if (cmbAddAssignType_Staff.SelectedItem.Value == "")
            //{
            //    lblAlertAddAssignType_Staff.Text = "กรุณาระบุข้อมูล";
            //    count += 1;
            //}
            //else
            //    lblAlertAddAssignType_Staff.Text = "";

            //if (cmbAddTeamTeles_Staff.SelectedItem.Value == "")
            //{
            //    lblAlertAddTeamTeles_Staff.Text = "กรุณาระบุข้อมูล";
            //    count += 1;
            //}
            //else
            //    lblAlertAddTeamTeles_Staff.Text = "";

            //return count > 0 ? false : true;
        }

        #endregion

        #region Page Control Gridview AddConfigStaff

        private void BindGridview_AddConfigStaff(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvAddConfigStaff);
            pageControl.Update(items, pageIndex);
            pageControl.GenerateRecordNumber(1, pageIndex);
            upResultStaff.Update();
        }

        protected void PageSearchChange_AddConfigStaff(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchConfigStaff(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        #region Popup Edit Config Staff

        protected void btnEditConfigStaffCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ClearPopupEditConfigStaff();
                mpePopupEditConfigStaff.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ClearPopupEditConfigStaff()
        {
            txtEditAssignConditionStaffId.Text = "";
            txtEditProductName_Staff.Text = "";
            txtEditGradeName_Staff.Text = "";
            txtEditAssignTypeName_Staff.Text = "";
            txtEditTeamTeles_Staff.Text = "";

            cmbEditConditionField_Staff.SelectedIndex = -1;
            cmbEditSortBy_Staff.SelectedIndex = -1;
            txtEditSeq_Staff.Text = "";
        }

        protected void btnEditConfigStaffSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateEditConfigStaff())
                {
                    SortConditionStaffBiz biz = new SortConditionStaffBiz();
                    biz.InsertData(txtEditAssignConditionStaffId.Text, cmbEditConditionField_Staff.SelectedItem.Value, cmbEditSortBy_Staff.SelectedItem.Value, txtEditSeq_Staff.Text.Trim(), HttpContext.Current.User.Identity.Name.ToLower());

                    cmbEditConditionField_Staff.SelectedIndex = -1;
                    cmbEditSortBy_Staff.SelectedIndex = -1;
                    txtEditSeq_Staff.Text = "";

                    DoSearchEditConfigStaff(0);

                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                mpePopupEditConfigStaff.Show();
                upEditConfigStaffButton_Inner.Update();
            }
        }

        private bool ValidateEditConfigStaff()
        {
            if (cmbEditConditionField_Staff.SelectedItem.Value == "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน ก่อนการบันทึก");
                return false;
            }

            if (txtEditSeq_Staff.Text.Trim() == "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน ก่อนการบันทึก");
                return false;
            }
            else
            {
                int result;
                if (!int.TryParse(txtEditSeq_Staff.Text.Trim(), out result))
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุ sequence เป็นตัวเลขที่มากกว่าศูนย์เท่านั้น");
                    return false;
                }
                if (result <= 0)
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุ sequence เป็นตัวเลขที่มากกว่าศูนย์เท่านั้น");
                    return false;
                }
            }

            return true;
        }

        protected void imbDeleteConfigStaff_Click(object sender, EventArgs e)
        {
            try
            {
                decimal sortConditionStaffId = decimal.Parse(((ImageButton)sender).CommandArgument);
                new SortConditionStaffBiz().DeleteData(sortConditionStaffId);

                DoSearchEditConfigStaff(0);
                AppUtil.ClientAlert(Page, "ลบข้อมูลเรียบร้อย");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        #region Page Control Gridview EditConfigStaff

        private void BindGridview_EditConfigStaff(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvEditConfigStaff);
            pageControl.Update(items, pageIndex);
            pageControl.GenerateRecordNumber(1, pageIndex);
            upEditConfigStaffButton_Inner.Update();
        }

        protected void PageSearchChange_EditConfigStaff(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchEditConfigStaff(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        
    }
}