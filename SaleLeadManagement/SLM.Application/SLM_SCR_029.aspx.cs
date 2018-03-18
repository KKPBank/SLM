using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;
using System.Collections;
using SLM.Resource;

namespace SLM.Application
{
    public partial class SLM_SCR_029 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_029));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาข้อมูล OBT";
            Page.Form.DefaultButton = btnMainSearch.UniqueID;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    Session.Remove(SLMConstant.SessionName.tabscreenlist);
                    Session.Remove(SLMConstant.SessionName.configscreen);

                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_029");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    if (string.IsNullOrEmpty(Request["backtype"]))
                    {
                        Session.Remove("followupsearchcondition");
                        Session.Remove("inboundsearchcondition");
                    }
                    InitialData();
                    tabMain.ActiveTabIndex = Request["backtype"] == "3" ? 1 : 0;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnMainSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabMain.ActiveTabIndex == 0)
                {
                    tabFollowUpCtl.CallSearchMethod();
                }
                else if (tabMain.ActiveTabIndex == 1)
                {
                    tabInboundCtl.CallSearchMethod();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        public void ExtendTab()
        {
            //tabMain.Width = new Unit(2825, UnitType.Pixel);
            //upTabMain.Update();
        }

        //protected void btnMainSearch_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (tabMain.ActiveTabIndex == 0)
        //        {
        //            tabFollowUpCtl.CallSearchMethod();
        //        }
        //        else if (tabMain.ActiveTabIndex == 1)
        //        {
        //            tabInboundCtl.CallSearchMethod();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        public void InitialData()
        {
            SlmScr029Biz biz = new SlmScr029Biz();
            int amountFollowup = biz.GetAmountJobTabFollowUp(HttpContext.Current.User.Identity.Name);         
            lblTabFollowUp.Text += "(" + amountFollowup.ToString() + " งาน)";

            var list = biz.GetAmountInboundOutBound(HttpContext.Current.User.Identity.Name);
            if (list.Count > 0)
            {
                lblOutbound.Text = list.Where(p => p.TextField == "outbound").Select(p => p.ValueField).FirstOrDefault();
                lblInbound.Text = list.Where(p => p.TextField == "inbound").Select(p => p.ValueField).FirstOrDefault();
            }
        }

    }
}