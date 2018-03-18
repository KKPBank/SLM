using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Text;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;
using SLM.Resource;

namespace SLM.Application
{
    public partial class SLM_SCR_017 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_017));
        private string ss_staffid = "staffid";
        private string ss_searchcondition = "staffsearchcondition";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาข้อมูลพนักงาน";
            Page.Form.DefaultButton = btnSearch.UniqueID;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_017");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    InitialControl();
                    //SetDept();
                    if (Session[ss_searchcondition] != null)
                    {
                        SetSearchCondition((StaffDataManagement)Session[ss_searchcondition]);  //Page Load กลับมาจากหน้าอื่น
                        Session.Remove(ss_searchcondition);
                    }
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
            //Role
            cmbStaffTypeSearch.DataSource = SlmScr017Biz.GetStaffTyeData();
            cmbStaffTypeSearch.DataTextField = "TextField";
            cmbStaffTypeSearch.DataValueField = "ValueField";
            cmbStaffTypeSearch.DataBind();
            cmbStaffTypeSearch.Items.Insert(0, new ListItem("", ""));

            //Market Branch
            cmbBranchSearch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.All);
            cmbBranchSearch.DataTextField = "TextField";
            cmbBranchSearch.DataValueField = "ValueField";
            cmbBranchSearch.DataBind();
            cmbBranchSearch.Items.Insert(0, new ListItem("", ""));

            //Department
            cmbDepartmentSearch.DataSource = SlmScr017Biz.GetDeptData();
            cmbDepartmentSearch.DataTextField = "TextField";
            cmbDepartmentSearch.DataValueField = "ValueField";
            cmbDepartmentSearch.DataBind();
            cmbDepartmentSearch.Items.Insert(0, new ListItem("", ""));

            //Position
            cmbPositionSearch.DataSource = PositionBiz.GetPositionList(SLMConstant.Position.All);
            cmbPositionSearch.DataTextField = "TextField";
            cmbPositionSearch.DataValueField = "ValueField";
            cmbPositionSearch.DataBind();
            cmbPositionSearch.Items.Insert(0, new ListItem("", ""));

            upSearch.Update();
        }

        //private void SetDept()
        //{
        //    decimal? stafftype = SlmScr019Biz.GetStaffTypeData(HttpContext.Current.User.Identity.Name);
        //    if (stafftype != null)
        //    {
        //        if (stafftype == SLMConstant.StaffType.ITAdministrator)
        //        {
        //            cmbDepartmentSearch.Enabled = true;
        //            cmbDepartmentSearch.SelectedIndex = -1;
        //        }
        //        else
        //        {
        //            cmbDepartmentSearch.Enabled = false;
        //            int? dept = SlmScr019Biz.GetDeptData(HttpContext.Current.User.Identity.Name);
        //            if (dept != null)
        //            {
        //                cmbDepartmentSearch.SelectedIndex = cmbDepartmentSearch.Items.IndexOf(cmbDepartmentSearch.Items.FindByValue(dept.ToString()));
        //            }
        //        }
        //    }
        //}

        private void SetSearchCondition(StaffDataManagement conn)
        {
            bool dosearch = false;
            try
            {
                if (!string.IsNullOrEmpty(conn.Username))
                {
                    txtUsernameSearch.Text = conn.Username;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.BranchCode))
                {
                    cmbBranchSearch.SelectedIndex = cmbBranchSearch.Items.IndexOf(cmbBranchSearch.Items.FindByValue(conn.BranchCode));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.EmpCode))
                {
                    txtEmpCodeSearch.Text = conn.EmpCode;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.MarketingCode))
                {
                    txtMarketingCodeSearch.Text = conn.MarketingCode;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.StaffNameTH))
                {
                    txtStaffNameTHSearch.Text = conn.StaffNameTH;
                    dosearch = true;
                }
                if (conn.PositionId != null)
                {
                    cmbPositionSearch.SelectedIndex = cmbPositionSearch.Items.IndexOf(cmbPositionSearch.Items.FindByValue(conn.PositionId.ToString()));
                    dosearch = true;
                }
                if (conn.StaffTypeId != null)
                {
                    cmbStaffTypeSearch.SelectedIndex = cmbStaffTypeSearch.Items.IndexOf(cmbStaffTypeSearch.Items.FindByValue(conn.StaffTypeId.Value.ToString()));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.Team))
                {
                    txtTeamSearch.Text = conn.Team;
                    dosearch = true;
                }
                if (conn.DepartmentId != null)
                {
                    cmbDepartmentSearch.SelectedIndex = cmbDepartmentSearch.Items.IndexOf(cmbDepartmentSearch.Items.FindByValue(conn.DepartmentId.Value.ToString()));
                    dosearch = true;
                }

                if (dosearch)
                    DoSearchData(conn.PageIndex != null ? conn.PageIndex.Value : 0);
            }
            catch
            {
                throw;
            }
        }

        private void DoSearchData(int pageIndex)
        {
            try
            {
                List<StaffDataManagement> list = SlmScr017Biz.SearchStaffList(txtUsernameSearch.Text.Trim(), cmbBranchSearch.SelectedItem.Value, txtEmpCodeSearch.Text.Trim(), txtMarketingCodeSearch.Text.Trim()
                    , txtStaffNameTHSearch.Text.Trim(), cmbPositionSearch.SelectedItem.Value, cmbStaffTypeSearch.SelectedItem.Value, txtTeamSearch.Text.Trim(), cmbDepartmentSearch.SelectedItem.Value);

                BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, list.ToArray(), pageIndex);
                pcTop.Visible = true;
                upResult.Update();
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
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchData(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData())
                    DoSearchData(0);
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
            try
            {
                txtUsernameSearch.Text = string.Empty;
                cmbBranchSearch.SelectedIndex = -1;
                txtEmpCodeSearch.Text = string.Empty;
                txtMarketingCodeSearch.Text = string.Empty;
                txtStaffNameTHSearch.Text = string.Empty;
                cmbPositionSearch.SelectedIndex = -1;
                cmbStaffTypeSearch.SelectedIndex = -1;
                txtTeamSearch.Text = string.Empty;
                cmbDepartmentSearch.SelectedIndex = -1;
                //SetDept();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidateData()
        {
            if (txtUsernameSearch.Text.Trim() == string.Empty && cmbBranchSearch.SelectedItem.Value == string.Empty && txtEmpCodeSearch.Text.Trim() == string.Empty
                && txtMarketingCodeSearch.Text.Trim() == string.Empty && txtStaffNameTHSearch.Text.Trim() == string.Empty && cmbPositionSearch.SelectedItem.Value == string.Empty
                && cmbStaffTypeSearch.SelectedItem.Value == string.Empty && txtTeamSearch.Text.Trim() == string.Empty && cmbDepartmentSearch.SelectedItem.Value == string.Empty)
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุเงื่อนไขอย่างน้อย 1 อย่าง");
                return false;
            }
            else
                return true;
        }

        private StaffDataManagement GetSearchCondition()
        {
            StaffDataManagement data = new StaffDataManagement();
            data.Username = txtUsernameSearch.Text.Trim();
            data.BranchCode = cmbBranchSearch.Items.Count > 0 ? cmbBranchSearch.SelectedItem.Value : string.Empty;
            data.EmpCode = txtEmpCodeSearch.Text.Trim();
            data.MarketingCode = txtMarketingCodeSearch.Text.Trim();
            data.StaffNameTH = txtStaffNameTHSearch.Text.Trim();
            if (cmbPositionSearch.SelectedItem.Value != "")
                data.PositionId = int.Parse(cmbPositionSearch.SelectedItem.Value);

            data.Team = txtTeamSearch.Text.Trim();
            if (cmbStaffTypeSearch.Items.Count > 0 && cmbStaffTypeSearch.SelectedItem.Value != string.Empty)
                data.StaffTypeId = decimal.Parse(cmbStaffTypeSearch.SelectedItem.Value);
            if (cmbDepartmentSearch.Items.Count > 0 && cmbDepartmentSearch.SelectedItem.Value != string.Empty)
                data.DepartmentId = int.Parse(cmbDepartmentSearch.SelectedItem.Value);

            data.PageIndex = pcTop.SelectedPageIndex;

            return data;
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                Session[ss_searchcondition] = GetSearchCondition();
                Response.Redirect("SLM_SCR_019.aspx");
                //mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbAction_Click(object sender, EventArgs e)
        {
            try
            {
                Session[ss_staffid] = ((ImageButton)sender).CommandArgument;
                Session[ss_searchcondition] = GetSearchCondition();
                Response.Redirect("SLM_SCR_018.aspx");
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            //Response.Redirect("SLM_SCR_011.aspx?ticketid=" + ((ImageButton)sender).CommandArgument + "&ReturnUrl=" + Server.UrlEncode(Request.Url.AbsoluteUri));
        }
    }
}