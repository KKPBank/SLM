using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Resource.Data;
using SLM.Biz;
using System.Globalization;
using SLM.Application.Utilities;
using log4net;

namespace SLM.Application.Shared.Obt
{
    public partial class TabInsureSummary : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(TabInsureSummary));

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void GetPreleadInsureSummary(String preleadId)
        {
            SearchInsureSummaryResult prelead = SlmScr004Biz.SearchPreleadInsureSummaryData(preleadId);
            if (prelead != null)
            {
                txtGrade.Text = prelead.Grade;
                txtCarCategory.Text = prelead.CarCategory;
                txtVoluntaryChannelKey.Text = prelead.VoluntaryChannelKey;
                txtVoluntaryPolicyNumber.Text = prelead.VoluntaryPolicyNumber;
                txtVoluntaryCovAmt.Text = prelead.VoluntaryCovAmt != null ? Convert.ToDecimal(prelead.VoluntaryCovAmt).ToString("#,##0.00") : "0.00";
                txtVoluntaryCompanyName.Text = prelead.VoluntaryCompanyName;
                txtVoluntaryType.Text = prelead.VoluntaryType;
                txtVoluntaryGrossPremium.Text = prelead.VoluntaryGrossPremium != null ? Convert.ToDecimal(prelead.VoluntaryGrossPremium).ToString("#,##0.00") : "0.00";
                txtVoluntaryFirstCreateDate.Text = prelead.VoluntaryFirstCreateDate != null ? Convert.ToDateTime(prelead.VoluntaryFirstCreateDate).ToString("dd/MM/yyyy") : "";
                txtVoluntaryPolicyEffDate.Text = prelead.VoluntaryPolicyEffDate != null ? Convert.ToDateTime(prelead.VoluntaryPolicyEffDate).ToString("dd/MM/yyyy") : "";
                txtVoluntaryPolicyExpDate.Text = prelead.VoluntaryPolicyExpDate != null ? Convert.ToDateTime(prelead.VoluntaryPolicyExpDate).ToString("dd/MM/yyyy") : "";
                txtCompulsoryCompanyName.Text = prelead.CompulsoryCompanyName;
                txtCompulsoryGrossPremium.Text = prelead.CompulsoryGrossPremium != null ? Convert.ToDecimal(prelead.CompulsoryGrossPremium).ToString("#,##0.00") : "0.00";
                txtCompulsoryPolicyEffDate.Text = prelead.CompulsoryPolicyEffDate != null ? Convert.ToDateTime(prelead.CompulsoryPolicyEffDate).ToString("dd/MM/yyyy") : "";
                txtCompulsoryPolicyExpDate.Text = prelead.CompulsoryPolicyExpDate != null ? Convert.ToDateTime(prelead.CompulsoryPolicyExpDate).ToString("dd/MM/yyyy") : "";
                txtactno.Text = prelead.actno;
                cbIsExportExpired.Checked = prelead.IsExportExpired;
                txtIsExportExpiredDate.Text = prelead.IsExportExpiredDate != null ? Convert.ToDateTime(prelead.IsExportExpiredDate).ToString("dd/MM/yyyy HH:mm:ss") : "";
            }
            pnTabInsureSummary.Visible = true;
            upResult.Update();
        }

        public void GetLeadInsureSummary(String ticketId)
        {
            SearchInsureSummaryResult lead = SlmScr004Biz.SearchLeadInsureSummaryData(ticketId);
            if (lead != null)
            {
                txtGrade.Text = lead.Grade;
                txtCarCategory.Text = lead.CarCategory;
                txtVoluntaryChannelKey.Text = lead.VoluntaryChannelKey;
                txtVoluntaryPolicyNumber.Text = lead.VoluntaryPolicyNumber;
                txtVoluntaryCovAmt.Text = lead.VoluntaryCovAmt != null ? Convert.ToDecimal(lead.VoluntaryCovAmt).ToString("#,##0.00") : "0.00";
                txtVoluntaryCompanyName.Text = lead.VoluntaryCompanyName;
                txtVoluntaryType.Text = lead.VoluntaryType;
                txtVoluntaryGrossPremium.Text = lead.VoluntaryGrossPremium != null ? Convert.ToDecimal(lead.VoluntaryGrossPremium).ToString("#,##0.00") : "0.00";
                txtVoluntaryFirstCreateDate.Text = lead.VoluntaryFirstCreateDate != null ? Convert.ToDateTime(lead.VoluntaryFirstCreateDate).ToString("dd/MM/yyyy") : "";
                txtVoluntaryPolicyEffDate.Text = lead.VoluntaryPolicyEffDate != null ? Convert.ToDateTime(lead.VoluntaryPolicyEffDate).ToString("dd/MM/yyyy") : "";
                txtVoluntaryPolicyExpDate.Text = lead.VoluntaryPolicyExpDate != null ? Convert.ToDateTime(lead.VoluntaryPolicyExpDate).ToString("dd/MM/yyyy") : "";
                txtCompulsoryCompanyName.Text = lead.CompulsoryCompanyName;
                txtCompulsoryGrossPremium.Text = lead.CompulsoryGrossPremium != null ? Convert.ToDecimal(lead.CompulsoryGrossPremium).ToString("#,##0.00") : "0.00";
                txtCompulsoryPolicyEffDate.Text = lead.CompulsoryPolicyEffDate != null ? Convert.ToDateTime(lead.CompulsoryPolicyEffDate).ToString("dd/MM/yyyy") : "";
                txtCompulsoryPolicyExpDate.Text = lead.CompulsoryPolicyExpDate != null ? Convert.ToDateTime(lead.CompulsoryPolicyExpDate).ToString("dd/MM/yyyy") : "";
                txtactno.Text = lead.actno;
                cbIsExportExpired.Checked = lead.IsExportExpired;
                txtIsExportExpiredDate.Text = lead.IsExportExpiredDate != null ? Convert.ToDateTime(lead.IsExportExpiredDate).ToString("dd/MM/yyyy HH:mm:ss") : "";
            }
            pnTabInsureSummary.Visible = true;
            upResult.Update();
        }

        protected void lbReloadSummary_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["ticketid"]))
                {
                    GetLeadInsureSummary(Request["ticketid"]);
                }
                else if (!string.IsNullOrEmpty(Request["preleadid"]))
                {
                    GetPreleadInsureSummary(Request["preleadid"]);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
    }
}