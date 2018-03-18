using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource;
using SLM.Resource.Data;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_055 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_055));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.Form.DefaultButton = btnSearch.UniqueID;
            ((Label)Page.Master.FindControl("lblTopic")).Text = "Upload Lead Search";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_055");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    InitialControl();
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
            pcTop.SetVisible = false;

            if (Session[SLMConstant.SessionName.UploadLeadSearchCondition] != null)
            {
                var searchCondition = Session[SLMConstant.SessionName.UploadLeadSearchCondition] as UploadLeadSearchCondition;
                if (!string.IsNullOrEmpty(searchCondition.Filename))
                {
                    txtFilename.Text = searchCondition.Filename;
                }
                if (!string.IsNullOrEmpty(searchCondition.StatusDesc))
                {
                    cmbFileStatus.SelectedIndex = cmbFileStatus.Items.IndexOf(cmbFileStatus.Items.FindByValue(searchCondition.StatusDesc));
                }

                Session.Remove(SLMConstant.SessionName.UploadLeadSearchCondition);
                DoSearchData(searchCondition.PageIndex);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchData(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchData(int pageIndex)
        {
            try
            {
                UploadLeadBiz biz = new UploadLeadBiz();
                var result = biz.SearchData(txtFilename.Text.Trim(), cmbFileStatus.SelectedItem.Value);
                BindGridview(pcTop, result.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void btnUploadNewLead_Click(object sender, EventArgs e)
        {
            Response.Redirect("SLM_SCR_056.aspx");
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex, SLMConstant.GridviewPageSizeUploadLead);
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

        protected void imbView_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                UploadLeadSearchCondition condition = new UploadLeadSearchCondition
                {
                    Filename = txtFilename.Text.Trim(),
                    StatusDesc = cmbFileStatus.SelectedItem.Value,
                    PageIndex = pcTop.SelectedPageIndex
                };
                Session[SLMConstant.SessionName.UploadLeadSearchCondition] = condition;

                ImageButton ib = sender as ImageButton;
                Response.Redirect(string.Format("SLM_SCR_057.aspx?uploadleadid={0}", ib.CommandArgument), false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                UploadLeadSearchCondition condition = new UploadLeadSearchCondition
                {
                    Filename = txtFilename.Text.Trim(),
                    StatusDesc = cmbFileStatus.SelectedItem.Value,
                    PageIndex = pcTop.SelectedPageIndex
                };
                Session[SLMConstant.SessionName.UploadLeadSearchCondition] = condition;

                ImageButton ib = sender as ImageButton;
                Response.Redirect(string.Format("SLM_SCR_056.aspx?uploadleadid={0}", ib.CommandArgument), false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
    }
}