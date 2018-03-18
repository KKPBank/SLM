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
    public partial class SLM_SCR_063 : System.Web.UI.Page
    {
        private ILog _log;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ข้อมูลกำหนดรถ Premium";
            Page.Form.DefaultButton = btnSearch.UniqueID;

            ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_063");
            if (priData == null || priData.IsView != 1)
                AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");

            AppUtil.BuildCombo(cmbDBrand, SlmScr063Biz.GetBrandData(), "กรุณาระบุ");

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
            txtSBrand.Text = "";
            cmbSStatus.SelectedIndex = 0;
            //pcTop.Visible = false;
            //gvResult.Visible = false;
            //upResult.Update();

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
                SlmScr063Biz biz = new SlmScr063Biz();
                var lst = biz.GetDataList(txtSBrand.Text, cmbSStatus.SelectedValue);
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
            SlmScr063Biz bz = new SlmScr063Biz();
            var cp = bz.GetDetail(id);
            if (cp != null)
            {
                SetData(cp);
                zPopDetail.Show();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateData()) { zPopDetail.Show(); return; }

            var cp = GetData();
            SlmScr063Biz bz = new SlmScr063Biz();
            if (!bz.SaveData(cp, Page.User.Identity.Name))
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
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            zPopDetail.Hide();
        }


        private void SetData(kkslm_ms_config_car_premium cp)
        {
            hdfID.Value = cp.slm_cc_pm_id.ToString();
            AppUtil.SetComboValue(cmbDBrand, cp.slm_Brand_Code);
            rdoDStatus.SelectedIndex = rdoDStatus.Items.IndexOf(rdoDStatus.Items.FindByValue(cp.is_Deleted.ToString()));
            cmbDBrand.Enabled = false;
            updModal.Update();
        }
        private kkslm_ms_config_car_premium GetData()
        {
            kkslm_ms_config_car_premium cp = new kkslm_ms_config_car_premium();
            cp.slm_cc_pm_id = SLMUtil.SafeDecimal(hdfID.Value);
            cp.slm_Brand_Code = cmbDBrand.SelectedValue;
            cp.is_Deleted = rdoDStatus.SelectedValue == "True";
            return cp;
        }
        private void ClearData()
        {
            hdfID.Value = "";
            cmbDBrand.SelectedIndex = 0;
            rdoDStatus.SelectedIndex = 0;
            cmbDBrand.Enabled = true;
            updModal.Update();
        }
        private bool ValidateData()
        {
            string err = "";
            //if (txtDCode.Text.Trim() == "") err += (err == "" ? "" : ", ") + "Team Telesales Code";
            //if (txtDName.Text.Trim() == "") err += (err == "" ? "" : ", ") + "ชื่อทีม Telesales";
            //if (cmbDHeader.SelectedValue == "") err += (err == "" ? "" : ", ") + "หัวหน้าทีม";
            if (cmbDBrand.SelectedIndex == 0) err += (err == "" ? "" : ", ") + "ชื่อยี่ห้อรถ";

            if (err != "")
            {
                AppUtil.ClientAlert(this, "กรุณากรอก " + err);
                return false;
            }
            else return true;
        }
        
    }
}