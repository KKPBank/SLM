using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_047 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_047));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_047");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    ((Label)Page.Master.FindControl("lblTopic")).Text = "ตรวจสอบข้อมูลการจ่ายงาน";
                    Page.Form.DefaultButton = btnSearch.UniqueID;
                    pcTop.SetVisible = false;
                    tdmTransferDateEnd.DateValue = DateTime.Now;
                    tdmTransferDateStart.DateValue = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }


        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex);
            pageControl.GenerateRecordNumber(0, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                var result = SlmScr047Biz.GetTransferList(tdmTransferDateStart.DateValue, tdmTransferDateEnd.DateValue);
                BindGridview(pageControl, result.ToArray(), pageControl.SelectedPageIndex);
                //DoSearchLeadData(pageControl.SelectedPageIndex, SortExpressionProperty, SortDirectionProperty);
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
                if (tdmTransferDateEnd.DateValue.Year == 1 || tdmTransferDateStart.DateValue.Year == 1)
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุ Transfer Date ให้ครบถ้วน");
                    return;
                }
                if (tdmTransferDateEnd.DateValue < tdmTransferDateStart.DateValue)
                {
                    AppUtil.ClientAlert(Page, "Transfer Date สิ้นสุด ต้องมากกว่าหรือเท่ากับ Transfer เริ่มต้น");
                    return;
                }
                var result = SlmScr047Biz.GetTransferList(tdmTransferDateStart.DateValue, tdmTransferDateEnd.DateValue);
                BindGridview(pcTop, result.ToArray(), 0);
                upResult.Update();
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
            DateTime date = new DateTime();
            date = date.AddYears(-date.Year + 1);
            tdmTransferDateStart.DateValue = date;
            tdmTransferDateEnd.DateValue = date;
            gvResult.DataSource = null;
            gvResult.DataBind();
            upResult.Update();
        }

        protected void btnLink_Click(object sender, EventArgs e)
        {            
            Button button = (Button)sender;
            int lotNo = 0;
            if (int.TryParse( button.CommandArgument, out lotNo))
            {
                Response.Redirect(String.Format("~/SLM_SCR_032.aspx?lot={0}", lotNo));   
            }            
        }


    }
}