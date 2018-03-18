using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Application.Utilities;

namespace SLM.Application.Shared
{
    public partial class CampaignDesc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Request["campaignid"] != null)
                    {
                        CampaignWSData campaign = SlmMasterBiz.GetCampaign(Request["campaignid"]);
                        if (campaign != null)
                        {
                            lblCampaignName.Text = campaign.CampaignName;
                            ltCampaignDesc.Text = campaign.CampaignDetail;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.Message);
            }
        }
    }
}