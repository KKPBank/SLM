using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using log4net;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Application.CSMUserServiceProxy;
using SLM.Application.Services;

namespace SLM.Application
{
    public partial class SLM_SCR_019 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_018));
        private string ss_staffid = "staffid";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "เพิ่มข้อมูลพนักงาน";
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
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_019");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    InitialControl();
                    //SetDept();
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
            BuildCombo(cmbStaffType, SlmScr017Biz.GetStaffTyeData());

            BuildCombo(cmbRoleService, SlmScr017Biz.GetRoleServiceData());
            //Market Branch
            BuildCombo(cmbBranchCode, BranchBiz.GetBranchList(SLMConstant.Branch.Active));

            //Head Staff Branch
            BuildCombo(cmbHeadBranchCode, BranchBiz.GetBranchList(SLMConstant.Branch.Active));

            //Department
            BuildCombo(cmbDepartment, SlmScr017Biz.GetDeptData());

            //Position
            BuildCombo(cmbPosition, PositionBiz.GetPositionList(SLMConstant.Position.Active));

            // level
            BuildCombo(cmbLevel, SlmScr019Biz.GetLevelList());

            // team telesale
            BuildCombo(cmbTeamTelesale, SlmScr019Biz.GetTeamTelesaleList());

            // emp type
            BuildCombo(cmbCategory, SlmScr019Biz.GetStaffCategoryData());

            // company 
            BuildCombo(cmbHost, SlmScr019Biz.GetStaffCategoryHostData());

            AppUtil.BuildCombo(cmbCategory, SlmScr018Biz.GetStaffCategoryData());
            AppUtil.BuildCombo(cmbHost, SlmScr018Biz.GetStaffCategoryHostData());

        }

        private void BuildCombo(DropDownList cmb, List<ControlListData> lst) { BuildCombo(cmb, lst, ""); }
        private void BuildCombo(DropDownList cmb, List<ControlListData> lst, string Blanktext)
        {
            cmb.DataSource = lst;
            cmb.DataTextField = "TextField";
            cmb.DataValueField = "ValueField";
            cmb.DataBind();
            cmb.Items.Insert(0, new ListItem(Blanktext, ""));
        }


        //private void SetDept()
        //{
        //    decimal? stafftype = SlmScr019Biz.GetStaffTypeData(HttpContext.Current.User.Identity.Name);
        //    if (stafftype != null)
        //    {
        //        if (stafftype == SLMConstant.StaffType.ITAdministrator)
        //            cmbDepartment.Enabled = true;
        //        else
        //        {
        //            cmbDepartment.Enabled = false;
        //            int? dept = SlmScr019Biz.GetDeptData(HttpContext.Current.User.Identity.Name);
        //            if (dept != null)
        //            {
        //                cmbDepartment.SelectedIndex =  cmbDepartment.Items.IndexOf(cmbDepartment.Items.FindByValue(dept.ToString()));
        //            }
        //        }
        //    }
        //}


        private bool ValidateData()
        {
            int i = 0;
            //************************************Windows Username********************************************
            if (txtUserName.Text.Trim() == "")
            {
                vtxtUserName.Text = "กรุณาระบุ Windows Username";
                vtxtUserName.ForeColor = System.Drawing.Color.Red;
                i += 1;
            }
            else
            {
                vtxtUserName.Text = "";
                if (SlmScr019Biz.CheckUsernameExist(txtUserName.Text.Trim(), null))
                {
                    vtxtUserName.Text = "Windows Username นี้มีอยู่แล้วในระบบแล้ว";
                    vtxtUserName.ForeColor = System.Drawing.Color.Red;
                    i += 1;
                }
                else
                    vtxtUserName.Text = "";
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
                if (SlmScr019Biz.CheckEmpCodeExist(txtEmpCode.Text.Trim(), null))
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
                if (SlmScr019Biz.CheckMarketingCodeExist(txtMarketingCode.Text.Trim(), null))
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

            //************************************Role Sale*******************************************
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

            //************************************สาขา********************************************
            if (cmbBranchCode.SelectedItem.Value == "")
            {
                vcmbBranchCode.Text = "กรุณาระบุ สาขา";
                i += 1;
            }
            else
            {
                vcmbBranchCode.Text = "";

                if (rbnUserTypeGroup.Checked && vrbnUserType.Text == "")
                {
                    if (SlmScr019Biz.CheckExistGroupInBranch(cmbBranchCode.SelectedItem.Value, null))
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

            //************************************หัวหน้างาน********************************************
            if (cmbHeadBranchCode.Items.Count > 0 && cmbHeadBranchCode.SelectedItem.Value != "" && cmbHeadStaffId.SelectedItem.Value == "")
            {
                vcmbHeadStaffId.Text = "กรุณาระบุ หัวหน้างาน";
                i += 1;
            }
            else
                vcmbHeadStaffId.Text = "";

            if (i > 0)
                return false;
            else
                return true;
        }
        private bool ValidateEmail()
        {
            string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern);
            return reg.IsMatch(txtStaffEmail.Text.Trim());
        }


        protected void cmbHeadBranchCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbHeadStaffId.DataSource = StaffBiz.GetHeadStaffList(cmbHeadBranchCode.SelectedItem.Value);
                cmbHeadStaffId.DataTextField = "TextField";
                cmbHeadStaffId.DataValueField = "ValueField";
                cmbHeadStaffId.DataBind();
                cmbHeadStaffId.Items.Insert(0, new ListItem("", ""));

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
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData())
                {
                    StaffDataManagement data = new StaffDataManagement();
                    data.Username = txtUserName.Text.Trim();
                    data.UserType = rbnUserTypeIndividual.Checked ? "I" : "G";
                    data.EmpCode = txtEmpCode.Text.Trim();
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
                    data.Level = AppUtil.SafeInt(cmbLevel.SelectedItem.Value);
                    data.Category = AppUtil.SafeInt(cmbCategory.SelectedItem.Value);
                    data.Host = AppUtil.SafeInt(cmbHost.SelectedItem.Value);
                    data.TeamTelesale = AppUtil.SafeInt(cmbTeamTelesale.SelectedItem.Value);
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
                    if (cmbHeadStaffId.Items.Count > 0 && !string.IsNullOrEmpty(cmbHeadStaffId.SelectedItem.Value))
                    {
                        data.HeadStaffId = int.Parse(cmbHeadStaffId.SelectedItem.Value);
                    }
                    if (cmbDepartment.Items.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(cmbDepartment.SelectedItem.Value)) { data.DepartmentId = int.Parse(cmbDepartment.SelectedItem.Value); }
                    }

                    var username = HttpContext.Current.User.Identity.Name;

                    if (AppConstant.CSMServiceEnableSyncUser)
                    {
                        try
                        {
                            var response = CSMService.InsertOrUpdateUser(1, data, username);

                            if (!response.IsSuccess)
                            {
                                //AppUtil.ClientAlert(Page, "การบันทึกข้อมูลไม่สำเร็จที่ ระบบ CSM");
								AppUtil.ClientAlert(Page, !string.IsNullOrEmpty(response.ErrorMessage) ? response.ErrorMessage : "การบันทึกข้อมูลไม่สำเร็จที่ ระบบ CSM");
                                return;
                            }
                        }
                        catch
                        {
                            AppUtil.ClientAlert(Page, "การบันทึกข้อมูลไม่สำเร็จเนื่องจากไม่สามารถเชื่อมต่อระบบ CSM");
                            return;
                        }
                    }

                    string staffId = SlmScr019Biz.InsertStaff(data, username);

                    Session[ss_staffid] = staffId;
                    AppUtil.ClientAlertAndRedirect(Page, "บันทึกข้อมูลเจ้าหน้าที่สำเร็จ", "SLM_SCR_018.aspx");
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
        protected void btnCheckUsername_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUserName.Text.Trim() != string.Empty)
                {
                    vtxtUserName.Text = "";

                    if (SlmScr019Biz.CheckUsernameExist(txtUserName.Text.Trim(), null))
                    {
                        vtxtUserName.Text = "Windows Username นี้มีอยู่แล้วในระบบแล้ว";
                        vtxtUserName.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        vtxtUserName.Text = "Windows Username ใช้งานได้";
                        vtxtUserName.ForeColor = System.Drawing.Color.Green;
                    }
                }
                else
                    vtxtUserName.Text = "";
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("SLM_SCR_017.aspx");
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
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