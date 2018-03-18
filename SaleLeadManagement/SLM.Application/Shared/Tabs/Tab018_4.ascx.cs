using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Resource.Data;
using log4net;
using SLM.Biz;
using SLM.Resource;
using SLM.Application.Services;
using SLM.Application.CSMSRServiceProxy;

namespace SLM.Application.Shared.Tabs
{
    public partial class Tab018_4 : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Tab018_4));

        public string Username { set { txtusername.Text = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitialControl();
            }
        }

        private void InitialControl()
        {

        }

        public void Update()
        {
            upResult.Update();
        }

        public SearchSRResponseItem[] GetData(int selectedPageIndex, int pageSize)
        {
            int startPageIndex = selectedPageIndex * pageSize;

            var employeeCode = SlmScr018Biz.GetStaffEmployeeCode(txtusername.Text.Trim());
            var searchSrResponse = CSMService.GetSrDelegateDataTab18_4(employeeCode, startPageIndex, pageSize);

            var results = new List<SearchSRResponseItem>();

            for (int i = 0; i < searchSrResponse.TotalRecords; i++)
            {

                if (i >= startPageIndex && i < (pageSize + startPageIndex))
                {
                    results.Add(searchSrResponse.SearchSRResponseItems[i - startPageIndex]);
                }
                else
                {
                    results.Add(new SearchSRResponseItem());
                }
            }

            return results.ToArray();
        }

        public void GetDelegateList()
        {
            try
            {
                BindGridview(pcTop, GetData(0, 10), 0);
                lblErrorMessage.Text = "";
            }
            catch 
            {
                //throw ex;
                lblErrorMessage.Text = "ไม่สามารถเชื่อมต่อระบบ CSM";
            }
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvDelegate);
            pageControl.Update(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridview(pageControl, GetData(pageControl.SelectedPageIndex, 10), pageControl.SelectedPageIndex);
                lblErrorMessage.Text = "";
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                //AppUtil.ClientAlert(Page, message);
                lblErrorMessage.Text = "ไม่สามารถเชื่อมต่อระบบ CSM";
            }
        }

        #endregion
    }
}