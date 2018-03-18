using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Application.Utilities;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_999 : System.Web.UI.Page
    {

        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_999));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //var lead = SlmScr004Biz.SearchSCR004Data("150083079023");
                    //tabActRenewInsure.InitialControl(lead);

                    //LeadData leadData = new LeadData();
                    //tabActRenewInsure.InitialControl(leadData);

                    //if (!string.IsNullOrEmpty(Request["preleadid"]))
                    //{
                    //    var data = new PreleadBiz().GetPreleadData(decimal.Parse(Request["preleadid"]), "", "", "");
                    //    tabActRenewInsure.InitialControlPrelead(data);
                    //    return;
                    //}

                    //if (!string.IsNullOrEmpty(Request["ticketid"]))
                    //{
                    //    var data = SlmScr004Biz.SearchSCR004Data(Request["ticketid"]);
                    //    tabActRenewInsure.InitialControlLead(data);
                    //    return;
                    //}

                    //tabActRenewInsure.HideButtons();        //Add By Pom 13/05/2016 ชั่วคราว
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                GetCampaignDataPopup(0);
                //mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void GetCampaignDataPopup(int pageIndex)
        {
            List<CampaignWSData> campaign = SlmScr010Biz.GetCampaignData();

        }

        public void UpdateStatusDesc(string statusDesc)
        {
            //txtstatus.Text = statusDesc;
            //GetLeadData();
            //upMainData.Update();
            //tabOwnerLogging.GetOwnerLogingList(txtTicketID.Text.Trim());
            upTabMain.Update();
        }
    }
}