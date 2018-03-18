using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SLM.Application.Utilities;
using SLM.Resource.Data;
using SLM.Biz;
using SLM.Resource;
using log4net;

namespace SLM.Application.Shared.Tabs
{
    public partial class Tab005 : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Tab005));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        public void SetDefaultValue(string citizenId, string telNo1, string ticketId)
        {
            txtTelNo1.Text = telNo1;
            txtCitizenId.Text = citizenId;
            txtTicketId.Text = ticketId;
        }

        public void DisplayContent(bool value)
        {
            upResult.Visible = value;
        }

        public void GetExistingLeadList(string citizenId, string telNo1, string ticketId)
        {
            try
            {
                txtTelNo1.Text = telNo1;
                txtCitizenId.Text = citizenId;
                txtTicketId.Text = ticketId;
                List<SearchLeadResult> result = SlmScr005Biz.SearchExistingLead(txtCitizenId.Text.Trim(), txtTicketId.Text.Trim());
                BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), 0);
                upResult.Update();
                pnTab005.Visible = true;
            }
            catch
            {
                throw;
            }
        }

        protected void imbView_Click(object sender, EventArgs e)
        {
            string type = new ConfigProductScreenBiz().GetFieldType(((ImageButton)sender).CommandArgument, SLMConstant.ConfigProductScreen.ActionType.View);
            Response.Redirect("SLM_SCR_004.aspx?ticketid=" + ((ImageButton)sender).CommandArgument + "&type=" + type + "&backtype=" + Request["backtype"]);
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvExistingLead);
            pageControl.Update(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                List<SearchLeadResult> result = SlmScr005Biz.SearchExistingLead(txtCitizenId.Text.Trim(), txtTicketId.Text.Trim());
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridview(pageControl, result.ToArray(), pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void lbReloadExistingLead_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTicketId.Text.Trim()))
                {
                    txtTicketId.Text = Request["ticketid"];
                }
                GetExistingLeadList(txtCitizenId.Text.Trim(), txtTelNo1.Text.Trim(), txtTicketId.Text.Trim());
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