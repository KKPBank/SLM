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
    public partial class SLM_SCR_062 : System.Web.UI.Page
    {
        private ILog _log;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.SetNumbericCharacter(txtDDiscountValue);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ข้อมูลกำหนดส่วนลดตาม Role";
            Page.Form.DefaultButton = btnSearch.UniqueID;

            ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_062");
            if (priData == null || priData.IsView != 1)
                AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");

            AppUtil.BuildCombo(cmbSRole, SlmScr062Biz.GetStaffTypeData(false, 0), "ทั้งหมด");

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
            //txtTeamCode.Text = "";
            //txtTeamName.Text = "";
            //cmbStatus.SelectedIndex = 0;
            //cmbTeamHeader.SelectedIndex = 0;
            cmbSRole.SelectedIndex = 0;
            chkSAct.Checked = true;
            chkSPolicy.Checked = true;
            cmbSStatus.SelectedIndex = 0;
            //pcTop.Visible = false;
           // gvResult.Visible = false;
           // upResult.Update();

        }
        protected void btnAddTeam_Click(object sender, EventArgs e)
        {
            ClearData();
            zPopDetail.Show();
        }


        private void LoadDataList(int pageIdx)
        {
            try
            {
                SlmScr062Biz biz = new SlmScr062Biz();
                var lst = biz.GetDataList(SLMUtil.SafeInt(cmbSRole.SelectedValue), chkSPolicy.Checked, chkSAct.Checked, cmbSStatus.SelectedValue);// biz.GetDataList(txtTeamCode.Text, txtTeamName.Text, cmbTeamHeader.SelectedValue, cmbStatus.SelectedValue);
                BindGridview(pcTop, lst, pageIdx);
                pcTop.Visible = gvResult.PageCount > 0;
                gvResult.Visible = true;
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
            try
            {
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
            var id = SLMUtil.SafeInt(((ImageButton)sender).CommandArgument);
            SlmScr062Biz bz = new SlmScr062Biz();
            var ds = bz.GetDetail(id);
            if (ds != null)
            {
                SetData(ds);
                zPopDetail.Show();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try {
                if (!ValidateData()) { zPopDetail.Show(); return; }

                var ts = GetData();
                SlmScr062Biz bz = new SlmScr062Biz();
                if (!bz.SaveData(ts, Page.User.Identity.Name))
                {
                    zPopDetail.Show();
                    AppUtil.ClientAlert(this, bz.ErrorMessage);
                }
                else
                {
                    zPopDetail.Hide();
                    LoadDataList(gvResult.PageIndex);
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(this, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                _log.Error(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            zPopDetail.Hide();
        }


        private void SetData(kkslm_ms_discount ds)
        {
            AppUtil.BuildCombo(cmbDRole, SlmScr062Biz.GetStaffTypeData(true, ds.slm_StaffTypeId.Value), "กรุณาระบุ");
            hdfID.Value = ds.slm_Discount_Id.ToString();
            AppUtil.SetComboValue(cmbDRole, ds.slm_StaffTypeId.ToString());
            rdoInsuranceType.SelectedIndex = rdoInsuranceType.Items.IndexOf(rdoInsuranceType.Items.FindByValue(ds.slm_InsuranceTypeCode));
            #region Add by First 21/11/2559
            rdoInsuranceType.Enabled = false;
            #endregion


            if (ds.slm_DiscountPercent > 0)
            {
                rdoDiscount.SelectedIndex = 0;
                txtDDiscountValue.Text = ds.slm_DiscountPercent.Value.ToString();
            }
            else if (ds.slm_DiscountBath > 0)
            {
                rdoDiscount.SelectedIndex = 1;
                txtDDiscountValue.Text = ds.slm_DiscountBath.Value.ToString();
            }
            else
            {
                rdoDiscount.SelectedIndex = 0;
                txtDDiscountValue.Text = "0";
            }
            rdoDStatus.SelectedIndex = rdoDStatus.Items.IndexOf(rdoDStatus.Items.FindByValue(ds.is_Deleted.ToString()));
            cmbDRole.Enabled = false;
            updModal.Update();
        }
        private kkslm_ms_discount GetData()
        {
            kkslm_ms_discount ds = new kkslm_ms_discount();
            ds.slm_Discount_Id = SLMUtil.SafeInt(hdfID.Value);
            ds.slm_StaffTypeId = SLMUtil.SafeDecimal(cmbDRole.SelectedValue);
            ds.slm_DiscountBath = rdoDiscount.SelectedValue == "2" ? (decimal?) SLMUtil.SafeDecimal(txtDDiscountValue.Text) : null;
            ds.slm_DiscountPercent = rdoDiscount.SelectedValue == "1" ? (int?) SLMUtil.SafeInt(txtDDiscountValue.Text) : null ;
            ds.slm_InsuranceTypeCode = rdoInsuranceType.SelectedValue;
            //ds.slm_ActDiscountBaht = null;
            //ds.slm_ActDiscountPercent = null;
            ds.is_Deleted = rdoDStatus.SelectedValue == "True";
            return ds;
        }
        private void ClearData()
        {
            AppUtil.BuildCombo(cmbDRole, SlmScr062Biz.GetStaffTypeData(true, 0), "กรุณาระบุ");
            hdfID.Value = "";
            cmbDRole.SelectedIndex = -1;
            rdoInsuranceType.SelectedIndex = -1;
            rdoInsuranceType.Enabled = true;
            rdoDiscount.SelectedIndex = 0;
            txtDDiscountValue.Text = "";
            rdoDStatus.SelectedIndex = 0;
            cmbDRole.Enabled = true;
            rdoInsuranceType.Enabled = true;
            updModal.Update();
        }
        private bool ValidateData()
        {
            string err = "";
            //if (txtDCode.Text.Trim() == "") err += (err == "" ? "" : ", ") + "Team Telesales Code";
            //if (txtDName.Text.Trim() == "") err += (err == "" ? "" : ", ") + "ชื่อทีม Telesales";
            //if (cmbDHeader.SelectedValue == "") err += (err == "" ? "" : ", ") + "หัวหน้าทีม";
            if (cmbDRole.SelectedIndex == 0) err += (err == "" ? "" : ", ") + "Role";
            if (rdoInsuranceType.SelectedIndex < 0) err += (err == "" ? "" : ", ") + "ผลตอบแทนธนาคาร";
            if (txtDDiscountValue.Text.Trim() == "") err += (err == "" ? "" : ", ") + "ส่วนลด";


            if (err != "")
            {
                AppUtil.ClientAlert(this, "กรุณากรอก " + err);
                return false;
            }

            #region Add by First -- 20/11/2016
            decimal temp;
            if (!decimal.TryParse(txtDDiscountValue.Text, out temp))
            {
                AppUtil.ClientAlert(this, "กรุณากรอกตัวเลขเท่านั้น");
                return false;
            }
            // by PPP, 21/11/2016, หน้าจอป้องกันการคีย์ตัวอักษรแล้ว และ มีเช็คด้านล่างแล้ว
            //if (rdoDiscount.SelectedValue == "1" && SLMUtil.SafeInt(txtDDiscountValue.Text) == 0)
            //{
            //    AppUtil.ClientAlert(this, "กรุณากรอก ส่วนลด (%) เป็นตัวเลขจำนวนเต็มและไม่เกิน 100% เท่านั้น");//by PPP, 21/11/2016, เพิ่ม Wording
            //    return false;
            //}
            #endregion

            if (rdoDiscount.SelectedValue == "2" && SLMUtil.SafeDecimal(txtDDiscountValue.Text) > (decimal)9999999999999999.99)
            {
                AppUtil.ClientAlert(this, "ส่วนลด (บาท) ต้องไม่เกิน 9,999,999,999,999,999.99");
                return false;
            }

            if (SLMUtil.SafeDecimal(txtDDiscountValue.Text) >100 && rdoDiscount.SelectedValue == "1")
            {
                AppUtil.ClientAlert(this, "กรอกส่วนลดได้ไม่เกิน 100%");
                return false;
            }


            return true;
        }

        private void BuildModelCombo(DropDownList cmbB, DropDownList cmbM, string defaulttxt)
        {
            AppUtil.BuildCombo(cmbM, SlmScr039Biz.GetModelDataList(cmbB.SelectedValue), defaulttxt);
        }

        protected void rdoDiscount_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDDiscountValue.Text = "";
            txtDDiscountValue.Focus();
            zPopDetail.Show();
        }
    }
}