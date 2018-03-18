using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Resource.Data;
using log4net;
using SLM.Biz;
using SLM.Resource;

namespace SLM.Application.Shared.Tabs
{
    public partial class Tab018_2 : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Tab018_2));
        //public delegate void UpdatedDataEvent(string tabName);
        //public event UpdatedDataEvent UpdatedDataChanged;

        public string Username { set { txtusername.Text = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitialControl();
            }
        }
        private void InitialControl()
        {
            AppUtil.SetIntTextBox(txtEmpCodePopup);
            txtEmpCodePopup.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");

            //Role
            cmbStaffType.DataSource = SlmScr017Biz.GetStaffTyeData();
            cmbStaffType.DataTextField = "TextField";
            cmbStaffType.DataValueField = "ValueField";
            cmbStaffType.DataBind();
            cmbStaffType.Items.Insert(0, new ListItem("", ""));

            //Market Branch
            cmbBranchCode.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.All);
            cmbBranchCode.DataTextField = "TextField";
            cmbBranchCode.DataValueField = "ValueField";
            cmbBranchCode.DataBind();
            cmbBranchCode.Items.Insert(0, new ListItem("", ""));

            //Head Staff Branch
            cmbHeadBranchCode.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.All);
            cmbHeadBranchCode.DataTextField = "TextField";
            cmbHeadBranchCode.DataValueField = "ValueField";
            cmbHeadBranchCode.DataBind();
            cmbHeadBranchCode.Items.Insert(0, new ListItem("", ""));

            //Department
            cmbDepartment.DataSource = SlmScr017Biz.GetDeptData();
            cmbDepartment.DataTextField = "TextField";
            cmbDepartment.DataValueField = "ValueField";
            cmbDepartment.DataBind();
            cmbDepartment.Items.Insert(0, new ListItem("", ""));

            //Position
            cmbPosition.DataSource = PositionBiz.GetPositionList(SLMConstant.Position.All);
            cmbPosition.DataTextField = "TextField";
            cmbPosition.DataValueField = "ValueField";
            cmbPosition.DataBind();
            cmbPosition.Items.Insert(0, new ListItem("", ""));
        }

        public void Update()
        {
            upResult.Update();
        }

        public void GetDelegateList()
        {
            try
            {
                List<SearchLeadResult> resultList = SlmScr018Biz.GetLeadDelegateDataTab18_2(txtusername.Text.Trim());
                BindGridview(pcTop, resultList.ToArray(), 0);
            }
            catch
            {
                throw;
            }
        }

        protected void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvDelegate.Rows.Count > 0)
                {
                    int ischeck = 0;
                    for (int i = 0; i < gvDelegate.Rows.Count; i++)
                    {
                        CheckBox cbSelect = (CheckBox)gvDelegate.Rows[i].FindControl("cbSelect");
                        if (cbSelect != null)
                        {
                            if (cbSelect.Checked == true)
                            {
                                ischeck += 1;
                            }
                        }
                    }
                    if (ischeck == 0)
                    {
                        AppUtil.ClientAlert(Page, "กรุณาระบุงานค้างในมือ Delegate อย่างน้อย 1 รายการ");
                    }
                    else
                    {
                        mpePopupTransfer.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            mpePopupTransfer.Hide();
        }

        protected void btnSavePopup_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtStaffId.Text.Trim() == "")
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุพนักงานที่ต้องการโอนงาน Owner");
                    mpePopupTransfer.Show();
                    return;
                }
                else if (cmbBranchCode.SelectedItem.Value == "")
                {
                    AppUtil.ClientAlert(Page, "ไม่สามารถโอนงานได้ เนื่องจากไม่มีข้อมูลสาขาของพนักงานที่ต้องการโอนงาน");
                    mpePopupTransfer.Show();
                    return;
                }
                else
                {
                    if (gvDelegate.Rows.Count > 0)
                    {
                        List<string> ticketlist = new List<string>();
                        List<string> notPassList = new List<string>();

                        for (int i = 0; i < gvDelegate.Rows.Count; i++)
                        {
                            CheckBox cbSelect = (CheckBox)gvDelegate.Rows[i].FindControl("cbSelect");
                            if (cbSelect != null && cbSelect.Checked == true)
                            {
                                //Check Access Right
                                string ticketId = ((Label)gvDelegate.Rows[i].FindControl("lbTicketId")).Text.Trim();
                                string campaignId = ((Label)gvDelegate.Rows[i].FindControl("lblCampaignId")).Text.Trim();

                                if (!SlmScr010Biz.PassPrivilegeCampaign(SLMConstant.Branch.Active, campaignId, txtUsernamePopup.Text.Trim()))
                                    notPassList.Add(ticketId);
                                else
                                    ticketlist.Add(ticketId);
                            }
                        }

                        SlmScr018Biz.UpdateTransferDelegateLead(ticketlist, txtUsernamePopup.Text.Trim(), int.Parse(txtStaffId.Text.Trim()), HttpContext.Current.User.Identity.Name, cmbBranchCode.SelectedItem.Value, txtusername.Text.Trim());
                        
                        txtEmpCodePopup.Text = "";
                        txtStaffId.Text = "";
                        txtUsernamePopup.Text = "";
                        txtMarketingCode.Text = "";
                        txtStaffNameTH.Text = "";
                        txtTellNo.Text = "";
                        txtStaffEmail.Text = "";
                        cmbPosition.SelectedIndex = -1;
                        cmbStaffType.SelectedIndex = -1;
                        txtTeam.Text = "";
                        cmbBranchCode.SelectedIndex = -1;
                        cmbHeadBranchCode.SelectedIndex = -1;
                        cmbHeadBranchCode_SelectedIndexChanged();
                        cmbHeadStaffId.SelectedIndex = -1;
                        cmbDepartment.SelectedIndex = -1;
                        rdNormal.Checked = false;
                        rdRetire.Checked = false;
                        GetDelegateList();
                        upResult.Update();

                        //if (UpdatedDataChanged != null) UpdatedDataChanged("tabOwner");

                        string alertTicketIdList = "";
                        foreach (string ticketId in notPassList)
                        {
                            alertTicketIdList += (alertTicketIdList != "" ? ", " : "") + ticketId;
                        }

                        if (alertTicketIdList == "")
                            AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อยแล้ว");
                        else
                            AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อยแล้ว โดยมี Ticket Id ที่โอนไม่ได้ดังนี้" + Environment.NewLine + alertTicketIdList);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlert(Page, message);
                mpePopupTransfer.Show();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                StaffDataManagement staff = new StaffDataManagement();
                if (txtEmpCodePopup.Text.Trim() != "")
                {
                    //staff = SlmScr018Biz.GetStaffDataByEmpcode(txtEmpCodePopup.Text.Trim(), SetDept());

                    staff = SlmScr018Biz.GetStaffDataByEmpcode(txtEmpCodePopup.Text.Trim(), "");

                    if (staff != null)
                    {
                        txtStaffId.Text = staff.StaffId.ToString();
                        txtUsernamePopup.Text = staff.Username;
                        txtMarketingCode.Text = staff.MarketingCode;
                        txtStaffNameTH.Text = staff.StaffNameTH;
                        txtTellNo.Text = staff.TelNo;
                        txtStaffEmail.Text = staff.StaffEmail;

                        if (staff.PositionId != null)
                            cmbPosition.SelectedIndex = cmbPosition.Items.IndexOf(cmbPosition.Items.FindByValue(staff.PositionId.ToString()));

                        if (staff.StaffTypeId != null)
                            cmbStaffType.SelectedIndex = cmbStaffType.Items.IndexOf(cmbStaffType.Items.FindByValue(staff.StaffTypeId.ToString()));

                        txtTeam.Text = staff.Team;
                        cmbBranchCode.SelectedIndex = cmbBranchCode.Items.IndexOf(cmbBranchCode.Items.FindByValue(staff.BranchCode));

                        if (staff.HeadStaffId != null)
                        {
                            string branchCode = StaffBiz.GetBranchCode(staff.HeadStaffId.Value);
                            if (!string.IsNullOrEmpty(branchCode))
                            {
                                ListItem item = cmbHeadBranchCode.Items.FindByValue(branchCode);
                                if (item != null)
                                    cmbHeadBranchCode.SelectedIndex = cmbHeadBranchCode.Items.IndexOf(item);
                                else
                                {
                                    //Branch ที่ถูกปิด
                                    string branchName = BranchBiz.GetBranchName(branchCode);
                                    if (!string.IsNullOrEmpty(branchName))
                                    {
                                        cmbHeadBranchCode.Items.Insert(1, new ListItem(branchName, branchCode));
                                        cmbHeadBranchCode.SelectedIndex = 1;
                                    }
                                }
                            }

                            cmbHeadBranchCode_SelectedIndexChanged();
                            cmbHeadStaffId.SelectedIndex = cmbHeadStaffId.Items.IndexOf(cmbHeadStaffId.Items.FindByValue(staff.HeadStaffId.ToString()));
                        }

                        if (staff.DepartmentId != null)
                            cmbDepartment.SelectedIndex = cmbDepartment.Items.IndexOf(cmbDepartment.Items.FindByValue(staff.DepartmentId.ToString()));
                        else
                            cmbDepartment.SelectedIndex = -1;

                        if (staff.Is_Deleted != null)
                        {
                            if (staff.Is_Deleted == 0)
                            {
                                rdNormal.Checked = true;
                            }
                            else if (staff.Is_Deleted == 1)
                            {
                                rdRetire.Checked = true;
                            }
                            else
                            {
                                rdNormal.Checked = false;
                                rdRetire.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        txtStaffId.Text = "";
                        txtUsernamePopup.Text = "";
                        txtMarketingCode.Text = "";
                        txtStaffNameTH.Text = "";
                        txtTellNo.Text = "";
                        txtStaffEmail.Text = "";
                        cmbPosition.SelectedIndex = -1;
                        cmbStaffType.SelectedIndex = -1;
                        txtTeam.Text = "";
                        cmbBranchCode.SelectedIndex = -1;
                        cmbHeadBranchCode.SelectedIndex = -1;
                        cmbHeadBranchCode_SelectedIndexChanged();
                        cmbHeadStaffId.SelectedIndex = -1;
                        cmbDepartment.SelectedIndex = -1;
                        rdNormal.Checked = false;
                        rdRetire.Checked = false;
                        AppUtil.ClientAlert(Page, "ไม่พบข้อมูลพนักงานเจ้าหน้าที่");
                    }
                }
                else
                    AppUtil.ClientAlert(Page, "กรุณาระบุรหัสพนักงานธนาคาร");
                upPopup.Update();
                mpePopupTransfer.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlert(Page, message);
                mpePopupTransfer.Show();
            }
        }

        //private string SetDept()
        //{
        //    decimal? stafftype = SlmScr019Biz.GetStaffTypeData(HttpContext.Current.User.Identity.Name);
        //    if (stafftype != null)
        //    {
        //        if (stafftype == SLMConstant.StaffType.ITAdministrator)
        //            return "IT";
        //        else
        //        {
        //            int? dept = SlmScr019Biz.GetDeptData(HttpContext.Current.User.Identity.Name);
        //            return dept.ToString();
        //        }
        //    }
        //    else
        //        return "";
        //}

        private void cmbHeadBranchCode_SelectedIndexChanged()
        {
            cmbHeadStaffId.DataSource = StaffBiz.GetHeadStaffList(cmbHeadBranchCode.SelectedItem.Value);
            cmbHeadStaffId.DataTextField = "TextField";
            cmbHeadStaffId.DataValueField = "ValueField";
            cmbHeadStaffId.DataBind();
            cmbHeadStaffId.Items.Insert(0, new ListItem("", ""));
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvDelegate);
            pageControl.Update(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                List<SearchLeadResult> resultList = SlmScr018Biz.GetLeadDelegateDataTab18_2(txtusername.Text.Trim());
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridview(pageControl, resultList.ToArray(), pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion
    }
}