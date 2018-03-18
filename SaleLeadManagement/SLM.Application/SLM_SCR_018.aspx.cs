using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using log4net;
using SLM.Application.CSMUserServiceProxy;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Application.Services;
using System.Configuration;

namespace SLM.Application
{
    public partial class SLM_SCR_018 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_018));
        private string ss_staffid = "staffid";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "แก้ไขข้อมูลพนักงาน";
            Page.Form.DefaultButton = btnSave.UniqueID;
            AppUtil.SetIntTextBox(txtEmpCode);
            AppUtil.SetIntTextBox(txtMarketingCode);
            AppUtil.SetIntTextBox(txtTellNo);
            AppUtil.SetIntTextBox(txtTellNo2);
            AppUtil.SetIntTextBox(txtTellNo3);
            txtEmpCode.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
            txtMarketingCode.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
            txtTellNo.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_018");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    if (Session[ss_staffid] != null)
                    {
                        txtStaffId.Text = Session[ss_staffid].ToString();
                        InitialControl();
                        LoadStaffData();
                        //GetCampaignData();
                        tabOwner.Username = txtUsername.Text.Trim();
                        tabOwner.Update();

                        tabDelegate.Username = txtUsername.Text.Trim();
                        tabDelegate.Update();

                        tabOwnerSr.Username = txtUsername.Text.Trim();
                        tabOwnerSr.Update();

                        tabDelegateSr.Username = txtUsername.Text.Trim();
                        tabDelegateSr.Update();

                        tabOwner.GetOwnerList(); // << อันนี้
                        tabDelegate.GetDelegateList(); // << อันนี้

                        tabOwnerSr.GetOwnerList();
                        tabDelegateSr.GetDelegateList();
                        //SetDept();
                    }
                    else
                    {
                        if (txtStaffId.Text.Trim() == string.Empty)
                            AppUtil.ClientAlertAndRedirect(Page, "Staff Id not found", "SLM_SCR_017.aspx");
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

        //private void SetDept()
        //{
        //    //decimal? stafftype = SlmScr019Biz.GetStaffTypeData(HttpContext.Current.User.Identity.Name);
        //    //if (stafftype != null)
        //    //{
        //    //    if (stafftype == SLMConstant.StaffType.ITAdministrator)
        //    //        cmbDepartment.Enabled = true;
        //    //    else
        //    //    {
        //    cmbDepartment.Enabled = false;
        //        //}
        //    //}
        //}

        private void InitialControl()
        {
            //Role Sale
            cmbStaffType.DataSource = SlmScr017Biz.GetStaffTyeData();
            cmbStaffType.DataTextField = "TextField";
            cmbStaffType.DataValueField = "ValueField";
            cmbStaffType.DataBind();
            cmbStaffType.Items.Insert(0, new ListItem("", ""));

            //Role Service
            cmbRoleService.DataSource = SlmScr017Biz.GetRoleServiceData();
            cmbRoleService.DataTextField = "TextField";
            cmbRoleService.DataValueField = "ValueField";
            cmbRoleService.DataBind();
            cmbRoleService.Items.Insert(0, new ListItem("", ""));

            //Market Branch
            cmbBranchCode.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
            cmbBranchCode.DataTextField = "TextField";
            cmbBranchCode.DataValueField = "ValueField";
            cmbBranchCode.DataBind();
            cmbBranchCode.Items.Insert(0, new ListItem("", ""));

            //Market Branch
            cmbHeadBranchCode.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
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
            cmbPosition.DataSource = PositionBiz.GetPositionList(SLMConstant.Position.Active);
            cmbPosition.DataTextField = "TextField";
            cmbPosition.DataValueField = "ValueField";
            cmbPosition.DataBind();
            cmbPosition.Items.Insert(0, new ListItem("", ""));

            // level
            cmbLevel.DataSource = SlmScr019Biz.GetLevelList();
            cmbLevel.DataTextField = "TextField";
            cmbLevel.DataValueField = "ValueField";
            cmbLevel.DataBind();
            cmbLevel.Items.Insert(0, new ListItem("", ""));

            // team telesale
            cmbTeamTelesale.DataSource = SlmScr019Biz.GetTeamTelesaleList();
            cmbTeamTelesale.DataTextField = "TextField";
            cmbTeamTelesale.DataValueField = "ValueField";
            cmbTeamTelesale.DataBind();
            cmbTeamTelesale.Items.Insert(0, new ListItem("", ""));

            AppUtil.BuildCombo(cmbCategory, SlmScr018Biz.GetStaffCategoryData());
            AppUtil.BuildCombo(cmbHost, SlmScr018Biz.GetStaffCategoryHostData());
        }

        private void LoadStaffData()
        {
            try
            {
                StaffDataManagement staff = new StaffDataManagement();
                if (txtStaffId.Text.Trim() != "")
                    staff = SlmScr018Biz.GetStaffData(int.Parse(txtStaffId.Text.Trim()));

                if (staff != null)
                {
                    txtUsername.Text = staff.Username;
                    lblUsername.Text = staff.Username;

                    if (!string.IsNullOrEmpty(staff.UserType))
                    {
                        if (staff.UserType.ToUpper() == "I")
                            rbnUserTypeIndividual.Checked = true;
                        else if (staff.UserType.ToUpper() == "G")
                            rbnUserTypeGroup.Checked = true;
                    }
                    rbnUserTypeIndividual.Enabled = false;
                    rbnUserTypeGroup.Enabled = false;

                    txtEmpCode.Text = staff.EmpCode;
                    txtEmpCodeOld.Value = staff.EmpCode;
                    txtMarketingCode.Text = staff.MarketingCode;
                    chkMarketing.Checked = staff.IsMarketing;
                    txtStaffNameTH.Text = staff.StaffNameTH;
                    txtTellNo.Text = staff.TelNo;
                    txtTellNo2.Text = staff.TelNo2;
                    txtTellNo3.Text = staff.TelNo3;
                    txtStaffEmail.Text = staff.StaffEmail;
                    //txtInternalPhone.Text = staff.InternalPhone;
                    if (staff.PositionId != null)
                        cmbPosition.SelectedIndex = cmbPosition.Items.IndexOf(cmbPosition.Items.FindByValue(staff.PositionId.ToString()));
                    if (staff.StaffTypeId != null)
                        cmbStaffType.SelectedIndex = cmbStaffType.Items.IndexOf(cmbStaffType.Items.FindByValue(staff.StaffTypeId.ToString()));
                    if (staff.RoleServiceId != null)
                        cmbRoleService.SelectedIndex = cmbRoleService.Items.IndexOf(cmbRoleService.Items.FindByValue(staff.RoleServiceId.ToString()));

                    txtTeam.Text = staff.Team;

                    if (!string.IsNullOrEmpty(staff.BranchCode))
                    {
                        ListItem item = cmbBranchCode.Items.FindByValue(staff.BranchCode);
                        if (item != null)
                            cmbBranchCode.SelectedIndex = cmbBranchCode.Items.IndexOf(item);
                        else
                        {
                            //Branch ที่ถูกปิด
                            string branchName = BranchBiz.GetBranchName(staff.BranchCode);
                            if (!string.IsNullOrEmpty(branchName))
                            {
                                cmbBranchCode.Items.Insert(1, new ListItem(branchName, staff.BranchCode));
                                cmbBranchCode.SelectedIndex = 1;
                            }
                        }
                    }

                    txtOldBranchCode.Text = staff.BranchCode;

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

                        cmbHeadStaffId.SelectedIndex = cmbHeadStaffId.Items.IndexOf(cmbHeadStaffId.Items.FindByValue(staff.HeadStaffId.Value.ToString()));
                        cmbHeadStaffId.Enabled = true;
                        lblHeadStaffId.Text = "*";
                    }

                    if (staff.DepartmentId != null)
                        cmbDepartment.SelectedIndex = cmbDepartment.Items.IndexOf(cmbDepartment.Items.FindByValue(staff.DepartmentId.ToString()));

                    if (staff.Is_Deleted != null)
                    {
                        txtOldIsDeleted.Text = staff.Is_Deleted.ToString();
                        txtNewIsDeleted.Text = staff.Is_Deleted.ToString();
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

                    // level
                    if (staff.Level != null) AppUtil.SetComboValue(cmbLevel, staff.Level.ToString());

                    // ประเภทพนักงาน
                    if (staff.Category != null) AppUtil.SetComboValue(cmbCategory, staff.Category.ToString());
                    SetHostVisible();

                    // ชื่อบริษัท
                    if (staff.Host != null) AppUtil.SetComboValue(cmbHost, staff.Host.ToString());

                    // ทีม Telesale
                    if (staff.TeamTelesale != null) AppUtil.SetComboValue(cmbTeamTelesale, staff.TeamTelesale.ToString());


                    // load license data
                    gvLicense.DataSource = SlmScr018Biz.GetLicenseInfo(staff.EmpCode);
                    gvLicense.DataBind();
                }
                upInfo.Update();

            }
            catch
            {
                throw;
            }
        }

        protected void cmbHeadBranchCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbHeadBranchCode_SelectedIndexChanged();
                if (cmbHeadBranchCode.SelectedItem.Value != "")
                {
                    lblHeadStaffId.Text = "*";
                    cmbHeadStaffId.Enabled = true;
                }
                else
                {
                    vcmbHeadStaffId.Text = "";
                    lblHeadStaffId.Text = "";
                    cmbHeadStaffId.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void cmbHeadBranchCode_SelectedIndexChanged()
        {
            var list = StaffBiz.GetHeadStaffList(cmbHeadBranchCode.SelectedItem.Value);
            var editedStaff = list.Where(p => p.ValueField == txtStaffId.Text.Trim()).FirstOrDefault();
            list.Remove(editedStaff);

            cmbHeadStaffId.DataSource = list;
            cmbHeadStaffId.DataTextField = "TextField";
            cmbHeadStaffId.DataValueField = "ValueField";
            cmbHeadStaffId.DataBind();
            cmbHeadStaffId.Items.Insert(0, new ListItem("", ""));
        }


        protected void btnSavePopup_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData())
                {
                }
                else
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        private bool ValidateData()
        {
            int i = 0;
            //************************************Windows Username********************************************
            if (txtUsername.Text.Trim() == "")
            {
                vtxtUsername.Text = "กรุณาระบุ Windows Username";
                vtxtUsername.ForeColor = System.Drawing.Color.Red;
                i += 1;
            }
            else
            {
                vtxtUsername.Text = "";
                if (SlmScr019Biz.CheckUsernameExist(txtUsername.Text.Trim(), int.Parse(txtStaffId.Text.Trim())))
                {
                    vtxtUsername.Text = "Windows Username นี้มีอยู่แล้วในระบบแล้ว";
                    vtxtUsername.ForeColor = System.Drawing.Color.Red;
                    i += 1;
                }
                else
                    vtxtUsername.Text = "";
            }

            //************************************ User Type *******************************************
            if (!rbnUserTypeIndividual.Checked && !rbnUserTypeGroup.Checked)
            {
                vrbnUserType.Text = "กรุณาระบุประเภท User";
                i += 1;
            }
            else
                vrbnUserType.Text = "";

            //************************************รหัสพนักงานธนาคาร********************************************
            if (txtEmpCode.Text.Trim() == "")
            {
                vtxtEmpCode.Text = "กรุณาระบุรหัสพนักงานธนาคาร";
                i += 1;
            }
            else
            {
                vtxtEmpCode.Text = "";
                if (SlmScr019Biz.CheckEmpCodeExist(txtEmpCode.Text.Trim(), int.Parse(txtStaffId.Text.Trim())))
                {
                    vtxtEmpCode.Text = "รหัสพนักงานธนาคารนี้มีอยู่แล้วในระบบแล้ว";
                    i += 1;
                }
                else
                    vtxtEmpCode.Text = "";
            }

            //************************************รหัสเจ้าหน้าที่การตลาด********************************************
            if (txtMarketingCode.Text.Trim() == "")
            {
                //vtxtMarketingCode.Text = "กรุณาระบุรหัสเจ้าหน้าที่การตลาด";
                //i += 1;
            }
            else
            {
                vtxtMarketingCode.Text = "";
                if (SlmScr019Biz.CheckMarketingCodeExist(txtMarketingCode.Text.Trim(), int.Parse(txtStaffId.Text.Trim())))
                {
                    vtxtMarketingCode.Text = "รหัสเจ้าหน้าที่การตลาดนี้มีอยู่แล้วในระบบแล้ว";
                    i += 1;
                }
                else
                    vtxtMarketingCode.Text = "";
            }

            //************************************ชื่อ-นามสกุลพนักงาน********************************************
            if (txtStaffNameTH.Text.Trim() == "")
            {
                vtxtStaffNameTH.Text = "กรุณาระบุชื่อ-นามสกุลพนักงาน";
                i += 1;
            }
            else
                vtxtStaffNameTH.Text = "";

            //************************************E-mail********************************************
            if (txtStaffEmail.Text.Trim() == "")
            {
                vtxtStaffEmail.Text = "กรุณาระบุ E-mail";
                i += 1;
            }
            else
            {
                if (!ValidateEmail())
                {
                    vtxtStaffEmail.Text = "กรุณาระบุ E-mail ให้ถูกต้อง";
                    i += 1;
                }
                else
                    vtxtStaffEmail.Text = "";
            }

            //************************************ตำแหน่ง********************************************
            if (cmbPosition.SelectedItem.Value == "")
            {
                vtxtPositionName.Text = "กรุณาระบุ ตำแหน่ง";
                i += 1;
            }
            else
                vtxtPositionName.Text = "";

            //************************************Role Sale********************************************
            if (cmbStaffType.SelectedItem.Value == "")
            {
                vcmbStaffType.Text = "กรุณาระบุ Role Sale";
                i += 1;
            }
            else
                vcmbStaffType.Text = "";

            //************************************Role Service****************************************
            if (cmbRoleService.SelectedItem.Value == "")
            {
                vcmbRoleService.Text = "กรุณาระบุ Role Service";
                i += 1;
            }
            else
                vcmbRoleService.Text = "";

            //************************************ทีมการตลาด********************************************
            //if (txtTeam.Text.Trim() == "")
            //{
            //    vtxtTeam.Text = "กรุณาระบุ ทีมการตลาด";
            //    i += 1;
            //}
            //else
            //    vtxtTeam.Text = "";

            //************************************สาขาพนักงาน********************************************

            if (cmbBranchCode.SelectedItem.Value == "")
            {
                vcmbBranchCode.Text = "กรุณาระบุ สาขา";
                i += 1;
            }
            else
            {
                if (cmbBranchCode.Items.Count > 0 && cmbBranchCode.SelectedItem.Value != "" && !BranchBiz.CheckBranchActive(cmbBranchCode.SelectedItem.Value))
                {
                    vcmbBranchCode.Text = "สาขานี้ถูกปิดแล้ว";
                    i += 1;
                }
                else
                {
                    vcmbBranchCode.Text = "";

                    if (rbnUserTypeGroup.Checked && vrbnUserType.Text == "")
                    {
                        if (SlmScr019Biz.CheckExistGroupInBranch(cmbBranchCode.SelectedItem.Value, int.Parse(txtStaffId.Text.Trim())))
                        {
                            vrbnUserType.Text = "ผู้ใช้คนนี้ไม่สามารถเป็นประเภท Group (Dummy) เพราะสาขานี้มี User ประเภท Group อยู่แล้ว";
                            i += 1;
                        }
                        else
                        {
                            vrbnUserType.Text = "";
                        }
                    }
                }
            }

            //************************************สาขาหัวหน้างาน********************************************

            if (cmbHeadBranchCode.Items.Count > 0 && cmbHeadBranchCode.SelectedItem.Value != "" && !BranchBiz.CheckBranchActive(cmbHeadBranchCode.SelectedItem.Value))
            {
                vcmbHeadBranchCode.Text = "สาขานี้ถูกปิดแล้ว";
                i += 1;
            }
            else
                vcmbHeadBranchCode.Text = "";

            //************************************หัวหน้างาน********************************************

            if (cmbHeadBranchCode.Items.Count > 0 && cmbHeadBranchCode.SelectedItem.Value != "")
            {
                if (cmbHeadStaffId.SelectedItem.Value == "")
                {
                    vcmbHeadStaffId.Text = "กรุณาระบุ หัวหน้างาน";
                    i += 1;
                }
                else
                {
                    var staffId = int.Parse(txtStaffId.Text);
                    var headStaffId = int.Parse(cmbHeadStaffId.SelectedItem.Value);

                    if (SlmScr018Biz.CheckIsLoopStructure(staffId, headStaffId))
                    {
                        vcmbHeadStaffId.Text = "การบันทึกข้อมูลไม่สำเร็จเนื่องจากพบ Recursive หัวหน้างาน";
                        vcmbHeadStaffId.ForeColor = System.Drawing.Color.Red;
                        i += 1;
                    }
                    else
                    {
                        vcmbHeadStaffId.Text = "";
                    }
                }
            }
            else
            {
                vcmbHeadStaffId.Text = "";
            }

            if (i > 0)
                return false;
            else
                return true;
        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (ValidateEmail() == false && txtStaffEmail.Text.Trim() != "")
                    vtxtStaffEmail.Text = "กรุณาระบุ E-mail ให้ถูกต้อง";
                else
                    vtxtStaffEmail.Text = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ValidateEmail()
        {
            string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern);
            return reg.IsMatch(txtStaffEmail.Text.Trim());

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                vcmbBranchCode.Text = "";
                vcmbHeadBranchCode.Text = "";

                if (cmbBranchCode.SelectedItem.Value != txtOldBranchCode.Text.Trim())
                {
                    if (SlmScr018Biz.CheckExistLeadOnHand(txtUsername.Text.Trim()))
                    {
                        AppUtil.ClientAlert(Page, "ไม่สามารถเปลี่ยนข้อมูลสาขาได้ เนื่องจากยังมีงานค้างอยู่");
                        return;
                    }
                    if (SlmScr018Biz.CheckExistPreLeadOnHand(txtUsername.Text.Trim()))
                    {
                        AppUtil.ClientAlert(Page, "ไม่สามารถเปลี่ยนข้อมูลสาขาได้ เนื่องจากยังมีงานค้างอยู่");
                        return;
                    }
                    //new 2016-11-09
                    if (SlmScr018Biz.CheckExistsTeamTeleSales(txtUsername.Text.Trim()))
                    {
                        AppUtil.ClientAlert(Page, "ไม่สามารถเปลี่ยนข้อมูลสาขาได้ เนื่องจากพนักงานรายนี้ถูกกำหนดเป็นหัวหน้าทีม Telesale Outbound อยู่");
                        return;
                    }
                }
                else if (txtOldIsDeleted.Text.Trim() != txtNewIsDeleted.Text.Trim())
                {
                    if (SlmScr018Biz.CheckExistLeadOnHand(txtUsername.Text.Trim()))
                    {
                        AppUtil.ClientAlert(Page, "ไม่สามารถเปลี่ยนสถานะพนักงานได้ เนื่องจากยังมีงานค้างอยู่");
                        return;
                    }
                    if (SlmScr018Biz.CheckExistPreLeadOnHand(txtUsername.Text.Trim()))
                    {
                        AppUtil.ClientAlert(Page, "ไม่สามารถเปลี่ยนข้อมูลสาขาได้ เนื่องจากยังมีงานค้างอยู่");
                        return;
                    }
                    if (ChannelBiz.CheckUserErrorInUse(txtUsername.Text.Trim()))
                    {
                        AppUtil.ClientAlert(Page, "ไม่สามารถเปลี่ยนสถานะพนักงานได้ เนื่องจากพนักงานนี้ถูกกำหนดเป็นผู้รับผิดชอบ Channel (Table: kkslm_ms_channel) กรุณาแจ้ง IT เพื่อทำการเซ็ทค่าออก");
                        return;
                    }
                    if (ChannelBiz.CheckUserAdminProductInUse(txtEmpCode.Text.Trim()))
                    {
                        AppUtil.ClientAlert(Page, "ไม่สามารถเปลี่ยนสถานะพนักงานได้ เนื่องจากพนักงานนี้ถูกกำหนดเป็นผู้รับผิดชอบ Product (Table: kkslm_ms_config_product_admin) กรุณาแจ้ง IT เพื่อทำการเซ็ทค่าออก");
                        return;
                    }
                    //new 2016-11-09
                    if (SlmScr018Biz.CheckExistsTeamTeleSales(txtUsername.Text.Trim()))
                    {
                        AppUtil.ClientAlert(Page, "ไม่สามารถเปลี่ยนสถานะพนักงานได้ เนื่องจากพนักงานรายนี้ถูกกำหนดเป็นหัวหน้าทีม Telesale Outbound อยู่");
                        return;
                    }
                }

                SaveData();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void SaveData()
        {
            try
            {
                if (ValidateData())
                {
                    int flag = 0;
                    StaffDataManagement data = new StaffDataManagement();
                    data.Username = txtUsername.Text.Trim();
                    data.UserType = rbnUserTypeIndividual.Checked ? "I" : "G";
                    data.EmpCode = txtEmpCode.Text.Trim();
                    data.EmpCodeOld = txtEmpCodeOld.Value.Trim();
                    data.MarketingCode = txtMarketingCode.Text.Trim();
                    data.IsMarketing = chkMarketing.Checked;
                    data.StaffNameTH = txtStaffNameTH.Text.Trim();
                    data.TelNo = txtTellNo.Text.Trim();
                    data.TelNo2 = txtTellNo2.Text.Trim();
                    data.TelNo3 = txtTellNo3.Text.Trim();
                    data.StaffEmail = txtStaffEmail.Text.Trim();
                    data.PositionId = int.Parse(cmbPosition.SelectedItem.Value);
                    data.StaffTypeId = decimal.Parse(cmbStaffType.SelectedItem.Value);
                    data.RoleServiceId = int.Parse(cmbRoleService.SelectedItem.Value);
                    data.Team = txtTeam.Text.Trim();
                    data.BranchCode = cmbBranchCode.SelectedItem.Value;

                    data.StaffId = int.Parse(txtStaffId.Text.Trim());
                    data.Level = AppUtil.SafeInt(cmbLevel.SelectedValue);
                    data.Category = AppUtil.SafeInt(cmbCategory.SelectedValue);
                    data.Host = AppUtil.SafeInt(cmbHost.SelectedValue);
                    data.TeamTelesale = AppUtil.SafeInt(cmbTeamTelesale.SelectedValue);
                    //data.InternalPhone = txtInternalPhone.Text;

                    //check if user = individual telno1 must not null
                    if (data.UserType == "I")
                    {
                        if (string.IsNullOrEmpty(data.TelNo))
                        {
                            AppUtil.ClientAlert(Page, "กรุณาระบุเบอร์โทรศัพท์ #1");
                            return;
                        }
                    }

                    //telphone must not duplicate
                    if (!string.IsNullOrEmpty(data.TelNo) && !string.IsNullOrEmpty(data.TelNo2))
                    {
                        if (data.TelNo == data.TelNo2)
                        {
                            AppUtil.ClientAlert(Page, "กรุณาระบุเบอร์โทรศัพท์ ไม่ซ้ำกัน");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(data.TelNo) && !string.IsNullOrEmpty(data.TelNo3))
                    {
                        if (data.TelNo == data.TelNo3)
                        {
                            AppUtil.ClientAlert(Page, "กรุณาระบุเบอร์โทรศัพท์ ไม่ซ้ำกัน");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(data.TelNo2) && !string.IsNullOrEmpty(data.TelNo3))
                    {
                        if (data.TelNo2 == data.TelNo3)
                        {
                            AppUtil.ClientAlert(Page, "กรุณาระบุเบอร์โทรศัพท์ ไม่ซ้ำกัน");
                            return;
                        }
                    }
                    if (rdNormal.Checked == true)
                        data.Is_Deleted = 0;
                    else if (rdRetire.Checked == true)
                        data.Is_Deleted = 1;

                    if (cmbHeadStaffId.Items.Count > 0 && cmbHeadStaffId.SelectedItem.Value != "")
                        data.HeadStaffId = int.Parse(cmbHeadStaffId.SelectedItem.Value);
                    else
                        data.HeadStaffId = null;

                    if (cmbDepartment.Items.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(cmbDepartment.SelectedItem.Value)) { data.DepartmentId = int.Parse(cmbDepartment.SelectedItem.Value); }
                    }

                    if (txtOldIsDeleted.Text.Trim() != txtNewIsDeleted.Text.Trim())
                    {
                        flag = 1;
                    }

                    var username = HttpContext.Current.User.Identity.Name;

                    if (AppConstant.CSMServiceEnableSyncUser)
                    {
                        try
                        {
                            var response = CSMService.InsertOrUpdateUser(2, data, username);

                            if (!response.IsSuccess)
                            {
                                if (response.ErrorCode == "7")
                                {
                                    if (txtOldIsDeleted.Text.Trim() != txtNewIsDeleted.Text.Trim())
                                    {
                                        AppUtil.ClientAlert(Page, "บันทึกข้อมูลไม่สำเร็จ กรุณาโอนย้ายงาน SR ค้างที่ระบบ CSM ก่อน");
                                        return;
                                    }
                                    if (data.BranchCode != txtOldBranchCode.Text)
                                    {
                                        AppUtil.ClientAlert(Page, "บันทึกข้อมูลไม่สำเร็จ กรุณาโอนย้ายงาน SR ค้างที่ระบบ CSM ก่อน");
                                        return;
                                    }

                                    AppUtil.ClientAlert(Page, "การบันทึกข้อมูลไม่สำเร็จที่ ระบบ CSM");
                                    return;
                                }
                                else
                                {
                                    //AppUtil.ClientAlert(Page, "การบันทึกข้อมูลไม่สำเร็จที่ ระบบ CSM");
									AppUtil.ClientAlert(Page, string.Format("{0} \r\nError Message : {1}", "การบันทึกข้อมูลไม่สำเร็จที่ ระบบ CSM", !string.IsNullOrEmpty(response.ErrorMessage) ? response.ErrorMessage : string.Empty));
                                    return;
                                }
                            }
                        }
                        catch
                        {
                            AppUtil.ClientAlert(Page, "การบันทึกข้อมูลไม่สำเร็จเนื่องจากไม่สามารถเชื่อมต่อระบบ CSM");
                            return;
                        }
                    }

                    string staffId = SlmScr019Biz.UpdateStaff(data, username, flag);
                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเจ้าหน้าที่สำเร็จ");
                    txtStaffId.Text = staffId;
                    InitialControl();
                    LoadStaffData();
                    //SetDept();
                    upInfo.Update();
                }
                else
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน");
                }
            }
            catch
            {
                throw;
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Remove(ss_staffid);
                Response.Redirect("~/SLM_SCR_017.aspx");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlert(Page, message);
            }
        }



        protected void rdNormal_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtNewIsDeleted.Text = "0";
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void rdRetire_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtNewIsDeleted.Text = "1";
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlert(Page, message);
            }
        }

        //public void UpdateData(string tabName)
        //{
        //    if (tabName == "tabOwner")
        //        tabOwner.GetOwnerList();
        //    else
        //        tabDelegate.GetDelegateList();
        //}

        #region Backup

        //private void GetCampaignData()
        //{
        //    List<StaffGroupData> sList = SlmScr018Biz.GetStaffGroupData(txtStaffId.Text.Trim());
        //    gvCampaign.DataSource = sList;
        //    gvCampaign.DataBind();
        //    upCampaign.Update();
        //}
        //protected void btnAdd_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        mpePopup.Show();
        //        GetCampaignDataPopup(0);
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}
        //private void GetCampaignDataPopup(int pageIndex)
        //{
        //    List<CampaignWSData> campaign = SlmScr018Biz.GetCampaignData(txtCampaignIdList.Text.Trim());
        //    BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, campaign.ToArray(), pageIndex);
        //    upCampaignPopup.Update();
        //}

        //private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        //{
        //    pageControl.SetGridview(gvSearchCampaign);
        //    pageControl.Update(items, pageIndex);
        //    upCampaignPopup.Update();
        //}
        //protected void PageSearchChange(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
        //        GetCampaignDataPopup(pageControl.SelectedPageIndex);
        //        mpePopup.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //protected void gvCampaign_DataBound(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string camList = "";
        //        if (gvCampaign.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < gvCampaign.Rows.Count; i++)
        //            {
        //                Label lblCampaignId = (Label)gvCampaign.Rows[i].FindControl("lblCampaignId");
        //                if (lblCampaignId != null)
        //                {
        //                    camList = (camList == "" ? camList : camList + ",") + lblCampaignId.Text.Trim();
        //                }
        //            }
        //        }
        //        txtCampaignIdList.Text = camList;
        //        upCampaign.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}
        //protected void btnOK_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        List<StaffGroupData> ListStaffGroup = new List<StaffGroupData>();
        //        if (gvSearchCampaign.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < gvSearchCampaign.Rows.Count; i++)
        //            {
        //                CheckBox chkSelect = (CheckBox)gvSearchCampaign.Rows[i].FindControl("chkSelect");
        //                if (chkSelect != null)
        //                {
        //                    if (chkSelect.Checked == true)
        //                    {
        //                        StaffGroupData sData = new StaffGroupData();
        //                        sData.StaffId = int.Parse(txtStaffId.Text.Trim());
        //                        sData.CampaignId = gvSearchCampaign.Rows[i].Cells[1].Text;
        //                        ListStaffGroup.Add(sData);
        //                    }
        //                }
        //            }
        //            if (ListStaffGroup.Count == 0)
        //            {
        //                AppUtil.ClientAlert(Page, "กรุณาระบุแคมเปญอย่างน้อย 1 รายการ");
        //                mpePopup.Show();
        //                return;
        //            }
        //            else
        //            {
        //                SlmScr018Biz.InsertStaffGroup(ListStaffGroup, HttpContext.Current.User.Identity.Name);
        //                GetCampaignData();
        //                upCampaign.Update();
        //                AppUtil.ClientAlert(Page, "บันทึกข้อมูลแคมเปญเรียบร้อย");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        mpePopup.Show();
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //protected void imbDelete_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        ImageButton imbDelete = (ImageButton)sender;
        //        SlmScr018Biz.DeleteStaffGroup(decimal.Parse(imbDelete.CommandArgument), HttpContext.Current.User.Identity.Name);
        //        GetCampaignData();
        //        upCampaign.Update();
        //        AppUtil.ClientAlert(Page, "ลบข้อมูลแคมเปญเรียบร้อย");
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }

        //}

        #endregion

        protected void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetHostVisible();
        }

        private void SetHostVisible()
        {
            if (cmbCategory.SelectedValue == "2")
            {
                trCompany.Visible = true;
            }
            else
            {
                trCompany.Visible = false;
                cmbHost.SelectedIndex = 0;
            }
        }
    }
}