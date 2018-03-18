using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Biz;
using SLM.Dal;
using SLM.Application.Utilities;
using SLM.Resource;
using SLM.Resource.Data;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_038 : System.Web.UI.Page
    {
        private ILog _log;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "จัดการข้อมูล Team Telesales";
            Page.Form.DefaultButton = btnSearch.UniqueID;

            ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_038");
            if (priData == null || priData.IsView != 1)
                AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");


            AppUtil.BuildCombo(cmbTeamHeader, SlmScr038Biz.GetStaffsListHeader(), "ทั้งหมด");
            //AppUtil.BuildCombo(cmbDHeader, SlmScr038Biz.GetStaffsList(), "ระบุ");
            _log = LogManager.GetLogger(this.GetType());
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDataList(0);
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtTeamCode.Text = "";
            txtTeamName.Text = "";
            cmbStatus.SelectedIndex = 0;
            cmbTeamHeader.SelectedIndex = 0;
        }
        protected void btnAddTeam_Click(object sender, EventArgs e)
        {
            ClearData();
            zPopDetail.Show();
        }


        private void LoadDataList(int pageIdx)
        {
            try {
                SlmScr038Biz biz = new SlmScr038Biz();
                var lst = biz.GetDataList(txtTeamCode.Text, txtTeamName.Text, cmbTeamHeader.SelectedValue, cmbStatus.SelectedValue);
                BindGridview(pcTop, lst, pageIdx);
                pcTop.Visible = gvResult.PageCount > 0;
                upResult.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void PageChange(object sender, EventArgs e)
        {
            try {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                LoadDataList(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex);
            upResult.Update();
        }



        protected void imbAction_Click(object sender, ImageClickEventArgs e)
        {
            var id = SLMUtil.SafeDecimal(((ImageButton)sender).CommandArgument);
            SlmScr038Biz bz = new SlmScr038Biz();
            var ts = bz.GetTeamData(id);
            if (ts != null)
            {
                SetData(ts);
                zPopDetail.Show();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateData()) { zPopDetail.Show(); return; }

            var ts = GetData();
            SlmScr038Biz bz = new SlmScr038Biz();
            if (!bz.SaveTeamData(ts, Page.User.Identity.Name))
            {
                zPopDetail.Show();
                AppUtil.ClientAlert(this, bz.ErrorMessage);
            }
            else
            {
                zPopDetail.Hide();
                LoadDataList(gvResult.PageIndex);
                string tmp = cmbTeamHeader.SelectedValue;
                AppUtil.BuildCombo(cmbTeamHeader, SlmScr038Biz.GetStaffsListHeader(), "ทั้งหมด");
                AppUtil.SetComboValue(cmbTeamHeader, tmp);
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            zPopDetail.Hide();
        }


        private void SetData(kkslm_ms_teamtelesales ts)
        {
            hdfID.Value = ts.slm_TeamTelesales_Id.ToString();
            txtDCode.Text = ts.slm_TeamTelesales_Code;
            txtDName.Text = ts.slm_TeamTelesales_Name;
            //AppUtil.SetComboValue(cmbDHeader, ts.slm_HeadStaff);
            rdoStatus.SelectedIndex = rdoStatus.Items.IndexOf(rdoStatus.Items.FindByValue(ts.is_Deleted.ToString()));
            hdfStaffID.Value = ts.slm_HeadStaff;
            SlmScr038Biz biz = new SlmScr038Biz();
            string empcode;
            var stf = biz.GetStaffNameFromID(SLMUtil.SafeDecimal(ts.slm_HeadStaff), out empcode);
            if (stf != null)
            {
                txtDHeader.Text = empcode;// stf.slm_EmpCode;
                lblStaffName.Text = stf;// (stf.slm_PositionName == null ? "" : stf.slm_PositionName + " - ") + stf.slm_StaffNameTH ?? "";
            }
            else

            {
                txtDHeader.Text = "";
                lblStaffName.Text = "";
            }
            txtDCode.ReadOnly = true;
            updModal.Update();
        }
        private kkslm_ms_teamtelesales GetData()
        {
            kkslm_ms_teamtelesales ts = new kkslm_ms_teamtelesales();
            ts.slm_TeamTelesales_Id = SLMUtil.SafeDecimal(hdfID.Value);
            ts.slm_TeamTelesales_Code = txtDCode.Text;
            ts.slm_TeamTelesales_Name = txtDName.Text;
            //ts.slm_HeadStaff = cmbDHeader.SelectedValue;
            ts.slm_HeadStaff = hdfStaffID.Value;
            ts.is_Deleted = rdoStatus.SelectedValue == "True";
            return ts;
        }
        private void ClearData()
        {
            hdfID.Value = "";
            txtDCode.Text = "";
            txtDName.Text = "";
            // cmbDHeader.SelectedIndex = -1;
            txtDHeader.Text = "";
            lblStaffName.Text = "";
            hdfStaffID.Value = "";
            rdoStatus.SelectedIndex = 0;
            txtDCode.ReadOnly = false;
            updModal.Update();
        }
        private bool ValidateData()
        {
            string err = "";
            if (txtDCode.Text.Trim() == "") err += (err == "" ? "" : ", ") + "Team Code";
            if (txtDName.Text.Trim() == "") err += (err == "" ? "" : ", ") + "ชื่อทีม";
            //if (cmbDHeader.SelectedValue == "") err += (err == "" ? "" : ", ") + "หัวหน้าทีม";
            if (hdfStaffID.Value.Trim() == "") err += (err == "" ? "" : ", ") + "หัวหน้าทีม";

            if (err != "")
            {
                AppUtil.ClientAlert(this, "กรุณากรอก " + err);
                return false;
            }
            else return true;
        }

        protected void txtDHeader_TextChanged(object sender, EventArgs e)
        {
            var biz = new SlmScr038Biz();
            int? _id;
            string _name;
            biz.GetStaffFromEmpCode(txtDHeader.Text, out _name, out _id);
            if (_id != null)
            {
                hdfStaffID.Value = _id.ToString();// stf.slm_StaffId.ToString();
                lblStaffName.Text = _name;// (stf.slm_PositionName == null ? "" : stf.slm_PositionName + " - ") + stf.slm_StaffNameTH ?? "";
            }
            else
            {
                hdfStaffID.Value = "";
                lblStaffName.Text = "";
                txtDHeader.Text = "";
                lblStaffName.Text = "<span style='color:red;'>รหัสพนักงานไม่ถูกต้อง</span>";
                txtDHeader.Focus();
            }
            zPopDetail.Show();
        }
    }
}