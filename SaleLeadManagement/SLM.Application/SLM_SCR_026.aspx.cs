using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Resource.Data;
using SLM.Biz;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_026 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_026));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาข้อมูลตำแหน่ง";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_026");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    Page.Form.DefaultButton = btnSearch.UniqueID;
                    DoSearchPosition(0);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchPosition(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchPosition(int pageIndex)
        {
            try
            {
                List<PositionData> list = PositionBiz.SearchPosition(txtPositionNameAbbSearch.Text.Trim(), txtPositionNameENSearch.Text.Trim(), txtPositionNameTHSearch.Text.Trim(), cbActiveSearch.Checked, cbInActiveSearch.Checked);
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
                DoSearchPosition(pageControl.SelectedPageIndex);
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

        protected void btnAddPosition_Click(object sender, EventArgs e)
        {
            try
            {
                txtPositionId.Text = "";
                txtPositionNameAbb.Text = "";
                txtPositionNameEN.Text = "";
                txtPositionNameTH.Text = "";
                rbActive.Checked = true;
                rbInActive.Checked = false;

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

        #endregion

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    if (txtPositionId.Text.Trim() != "")
                        PositionBiz.UpdateData(int.Parse(txtPositionId.Text.Trim()), txtPositionNameAbb.Text.Trim(), txtPositionNameEN.Text.Trim(), txtPositionNameTH.Text.Trim(), rbActive.Checked, HttpContext.Current.User.Identity.Name);
                    else
                        PositionBiz.InsertData(txtPositionNameAbb.Text.Trim(), txtPositionNameEN.Text.Trim(), txtPositionNameTH.Text.Trim(), rbActive.Checked, HttpContext.Current.User.Identity.Name);

                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                    ClearPopupControl();
                    mpePopup.Hide();

                    DoSearchPosition(0);
                }
                else
                    mpePopup.Show();
            }
            catch (Exception ex)
            {
                mpePopup.Show();
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(((ImageButton)sender).CommandArgument);

                txtPositionId.Text = ((Label)gvResult.Rows[index].FindControl("lblPositionId")).Text.Trim();
                txtPositionNameAbb.Text = ((Label)gvResult.Rows[index].FindControl("lblPositionNameAbb")).Text.Trim();
                txtPositionNameEN.Text = ((Label)gvResult.Rows[index].FindControl("lblPositionNameEN")).Text.Trim();
                txtPositionNameTH.Text = ((Label)gvResult.Rows[index].FindControl("lblPositionNameTH")).Text.Trim();
                string status = ((Label)gvResult.Rows[index].FindControl("lblStatus")).Text.Trim();

                rbActive.Checked = status == "Y" ? true : false;
                rbInActive.Checked = status == "Y" ? false : true;

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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPopupControl();
            mpePopup.Hide();
        }

        private void ClearPopupControl()
        {
            txtPositionId.Text = "";
            txtPositionNameAbb.Text = "";
            txtPositionNameEN.Text = "";
            txtPositionNameTH.Text = "";
            rbActive.Checked = true;
            rbInActive.Checked = false;
            alertPositionNameAbb.Text = "";
            alertPositionNameEN.Text = "";
            alertPositionNameTH.Text = "";
            alertStatus.Text = "";
        }

        private bool ValidateInput()
        {
            int i = 0;
            int nameAbbMaxLength = 100;
            int nameENMaxlength = 500;
            int nameTHMaxlength = 500;

            if (txtPositionNameAbb.Text.Trim() == "")
            {
                alertPositionNameAbb.Text = "กรุณาระบุ ชื่อย่อ";
                i += 1;
            }
            else
            {
                if (txtPositionNameAbb.Text.Trim().Length > nameAbbMaxLength)
                {
                    alertPositionNameAbb.Text = "กรุณาระบุ ชื่อย่อไม่เกิน " + nameAbbMaxLength.ToString() + " ตัวอักษร";
                    i += 1;
                }
                else
                    alertPositionNameAbb.Text = "";
            }

            if (txtPositionNameEN.Text.Trim() != "" && txtPositionNameEN.Text.Trim().Length > nameENMaxlength)
            {
                alertPositionNameEN.Text = "กรุณาระบุ ชื่อเต็มไม่เกิน " + nameENMaxlength.ToString() + " ตัวอักษร";
                i += 1;
            }
            else
                alertPositionNameEN.Text = "";

            if (txtPositionNameTH.Text.Trim() == "")
            {
                alertPositionNameTH.Text = "กรุณาระบุ ชื่อตำแหน่ง";
                i += 1;
            }
            else
            {
                if (txtPositionNameTH.Text.Trim().Length > nameTHMaxlength)
                {
                    alertPositionNameTH.Text = "กรุณาระบุ ชื่อตำแหน่งไม่เกิน " + nameTHMaxlength.ToString() + " ตัวอักษร";
                    i += 1;
                }
                else
                    alertPositionNameTH.Text = "";
            }

            if (txtPositionId.Text.Trim() != "" && rbInActive.Checked)
            {
                if (StaffBiz.CheckEmployeeInPosition(int.Parse(txtPositionId.Text.Trim())))
                {
                    alertStatus.Text = "ไม่สามารถยกเลิกการใช้งานได้ เนื่องจากยังมีพนักงานอยู่ในตำแหน่ง";
                    i += 1;
                }
                else
                    alertStatus.Text = "";
            }
            else
                alertStatus.Text = "";

            return i > 0 ? false : true;
        }
    }
}