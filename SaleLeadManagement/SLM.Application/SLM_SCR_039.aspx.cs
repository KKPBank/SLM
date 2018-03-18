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
    public partial class SLM_SCR_039 : System.Web.UI.Page
    {
        private ILog _log;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "กำหนดประเภทการใช้งานรถ";
            Page.Form.DefaultButton = btnSearch.UniqueID;

            ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_039");
            if (priData == null || priData.IsView != 1)
                AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");


            AppUtil.BuildCombo(cmbSBrand, SlmScr039Biz.GetBrandDataList(), "ทั้งหมด");
            AppUtil.BuildCombo(cmbDBrand, SlmScr039Biz.GetBrandDataList(), "กรุณาระบุ");
            AppUtil.BuildCombo(cmbSType, SlmScr039Biz.GetTypeDataList(), "ทั้งหมด");
            AppUtil.BuildCombo(cmbDType, SlmScr039Biz.GetTypeDataList(), "กรุณาระบุ");

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
            cmbSBrand.SelectedIndex = 0;
            BuildModelCombo(cmbSBrand, cmbSModel, "ทั้งหมด");
            //cmbSModel.SelectedIndex = 0;
            cmbSType.SelectedIndex = 0;
            cmbSStatus.SelectedIndex = 0;
            //gvResult.Visible = false;
            //pcTop.Visible = false;
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
                SlmScr039Biz biz = new SlmScr039Biz();
                var lst = biz.GetDataList(cmbSBrand.SelectedValue, cmbSModel.SelectedValue, SLMUtil.SafeInt(cmbSType.SelectedValue), cmbSStatus.SelectedValue);
                BindGridview(pcTop, lst, pageIdx);
                gvResult.Visible = true;
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
            var id = ((ImageButton)sender).CommandArgument;
            SlmScr039Biz bz = new SlmScr039Biz();
            var mm = bz.GetDetails(id);
            if (mm != null)
            {
                SetData(mm);
                zPopDetail.Show();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateData()) { zPopDetail.Show(); return; }

            var ts = GetData();
            SlmScr039Biz bz = new SlmScr039Biz();
            if (!bz.SaveData(ts, Page.User.Identity.Name, hdfID.Value == "1"))
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


        private void SetData(kkslm_ms_mapping_brand_model_inscartype mm)
        {
            hdfID.Value = "1";
            AppUtil.SetComboValue(cmbDBrand, mm.slm_BrandCode);
            BuildModelCombo(cmbDBrand, cmbDModel, "กรุณาระบุ");
            AppUtil.SetComboValue(cmbDModel, mm.slm_ModelCode);
            AppUtil.SetComboValue(cmbDType, mm.slm_InsuranceCarTypeId.ToString());
            rdoDStatus.SelectedIndex = rdoDStatus.Items.IndexOf(rdoDStatus.Items.FindByValue(mm.is_Deleted.ToString()));

            cmbDBrand.Enabled = false;
            cmbDModel.Enabled = false;
            updModal.Update();
        }
        private kkslm_ms_mapping_brand_model_inscartype GetData()
        {
            kkslm_ms_mapping_brand_model_inscartype mm = new kkslm_ms_mapping_brand_model_inscartype();
            mm.slm_BrandCode = cmbDBrand.SelectedValue;
            mm.slm_ModelCode = cmbDModel.SelectedValue;
            mm.slm_InsuranceCarTypeId = SLMUtil.SafeInt(cmbDType.SelectedValue);
            mm.is_Deleted = rdoDStatus.SelectedValue == "True";
            return mm;
        }
        private void ClearData()
        {
            hdfID.Value = "";
            cmbDBrand.SelectedIndex = 0;
            BuildModelCombo(cmbDBrand, cmbDModel, "กรุณาระบุ");
            cmbDModel.SelectedIndex = 0;
            cmbDType.SelectedIndex = 0;

            cmbDBrand.Enabled = true;
            cmbDModel.Enabled = true;
            rdoDStatus.SelectedIndex = 0;
            updModal.Update();
        }
        private bool ValidateData()
        {
            string err = "";
            //if (txtDCode.Text.Trim() == "") err += (err == "" ? "" : ", ") + "Team Telesales Code";
            //if (txtDName.Text.Trim() == "") err += (err == "" ? "" : ", ") + "ชื่อทีม Telesales";
            //if (cmbDHeader.SelectedValue == "") err += (err == "" ? "" : ", ") + "หัวหน้าทีม";

            if (cmbDBrand.SelectedIndex == 0) err += (err == "" ? "" : ", ") + "ยี่ห้อรถ";
            if (cmbDModel.SelectedIndex == 0) err += (err == "" ? "" : ", ") + "รุ่นรถ";
            if (cmbDType.SelectedIndex == 0) err += (err == "" ? "" : ", ") + "ประเภทการใช้งาน";

            if (err != "")
            {
                AppUtil.ClientAlert(this, "กรุณากรอก " + err);
                return false;
            }
            else return true;
        }

        private void BuildModelCombo(DropDownList cmbB, DropDownList cmbM, string defaulttxt)
        {
            AppUtil.BuildCombo(cmbM, SlmScr039Biz.GetModelDataList(cmbB.SelectedValue), defaulttxt );
        }

        protected void cmbSBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildModelCombo(cmbSBrand, cmbSModel, "ทั้งหมด");
        }

        protected void cmbDBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildModelCombo(cmbDBrand, cmbDModel, "กรุณาระบุ");
            zPopDetail.Show();
        }
    }
}