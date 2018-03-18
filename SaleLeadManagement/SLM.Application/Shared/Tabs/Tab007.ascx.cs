using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Resource.Data;
using SLM.Biz;
using SLM.Application.Utilities;
using log4net;

namespace SLM.Application.Shared.Tabs
{
    public partial class Tab007 : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Tab007));

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void SetDefaultValue(string ticketId, string preleadId)
        {
            txtTicketId.Text = ticketId;
            txtPreleadId.Text = preleadId;
        }

        public void GetOwnerLogingList(string ticketId, string preleadId)
        {
            try
            {
                txtTicketId.Text = ticketId;
                txtPreleadId.Text = preleadId;
                List<OwnerLoggingData> result = SlmScr007Biz.SearchOwnerLogging(ticketId, preleadId);
                BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), 0);
                upResult.Update();

                pnTab007.Visible = true;
            }
            catch
            {
                throw;
            }
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvOwnerLogging);
            pageControl.Update(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                List<OwnerLoggingData> result = SlmScr007Biz.SearchOwnerLogging(txtTicketId.Text.Trim(), txtPreleadId.Text.Trim());
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

        protected void lbReloadLogging_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTicketId.Text.Trim()))
                {
                    txtTicketId.Text = Request["ticketid"];
                }
                if (string.IsNullOrWhiteSpace(txtPreleadId.Text.Trim()))
                {
                    txtPreleadId.Text = Request["preleadid"];
                }
                    
                GetOwnerLogingList(txtTicketId.Text.Trim(), txtPreleadId.Text.Trim());
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