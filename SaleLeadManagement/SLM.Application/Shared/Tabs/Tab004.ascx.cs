using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls; 
using log4net;
using SLM.Biz;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Application.Shared;
using SLM.Application.Utilities;

namespace SLM.Application.Shared.Tabs
{
    public partial class Tab004 : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Tab004));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (IsPostBack)
                LoadDetailControl();
        }
        Lead_Detail_Master ctlLead;
        private string SessionPrefix
        {
            get
            {
                var frm004 = this.Parent as SLM_SCR_004;
                if (frm004 != null)
                {
                    return frm004.SessionPrefix;
                }
                else
                    return "";
            }
        }
        string CampaignId
        {
            get { return Session["TAB004.CAMPAIGNID"] as string; }
            set { Session["TAB004.CAMPAIGNID"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["ticketid"]))
            {
                lbReloadInfo.Visible = true;
                lbReloadPreleadInfo.Visible = false;
            }
            else if (!string.IsNullOrEmpty(Request["preleadid"]))
            {
                lbReloadInfo.Visible = false;
                lbReloadPreleadInfo.Visible = true;
            }
            else
            {
                lbReloadInfo.Visible = false;
                lbReloadPreleadInfo.Visible = false;
            }
        }

        public void ReLoadDetailControl()
        {
            if (pnlLead.Visible == false) return;
            if (!String.IsNullOrEmpty(CampaignId))
            {
                string ctl = "";
                if (Session[SLMConstant.SessionName.screen_code] != null)
                    ctl = Convert.ToString(Session[SLMConstant.SessionName.screen_code]);
                else
                {
                    SlmScr010Biz bz = new SlmScr010Biz();
                    ctl = bz.GetControlname(CampaignId, "V");
                }

                
                if (ctl != "")
                    ctlLead = (Lead_Detail_Master)LoadControl("~/Shared/" + ctl);
                else
                    ctlLead = (Lead_Detail_Master)LoadControl("~/Shared/Lead_Detail_Default.ascx");

                if (ctlLead != null)
                {
                    if (plcControl.Controls.Count == 0)
                    {
                        ctlLead.CommonData = ctlCommon.GetCommonData();
                        plcControl.Controls.Add(ctlLead);
                    }
                }
                else
                    AppUtil.ClientAlert(this, "Invalid Control Name, Please contact administrator");

            }
        }

        public void GetLeadData(LeadData lead)
        {
            try
            {
                pnlLead.Visible = true;

                CampaignId = lead.CampaignId;
                ctlCommon.SetLeadData(lead);
                LoadDetailControl();
                ctlLead.LoadData(lead);

            }
            catch(Exception ex) 
            {
                _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            finally
            {
                ctlCommon.SetControlMode(Lead_Detail_Master.CtlMode.View);
                ctlLead.SetControlMode(Lead_Detail_Master.CtlMode.View);
                SetAllView();
            }
        }
        private void SetAllView()
        {
            List<TextBox> tLst = new List<TextBox>();
            GetControlList<TextBox>(this.Controls, tLst);
            foreach (TextBox tb in tLst) { tb.ReadOnly = true; if (tb.CssClass != "Hidden") tb.CssClass = "TextboxView"; }

            List<DropDownList> dLst = new List<DropDownList>();
            GetControlList<DropDownList>(this.Controls, dLst);
            foreach (DropDownList dd in dLst) { dd.Enabled = false; if (dd.CssClass != "Hidden") dd.CssClass = "TextboxView"; }

            List<CheckBox> cLst = new List<CheckBox>();
            GetControlList<CheckBox>(this.Controls, cLst);
            foreach (var cb in cLst) cb.Enabled = false;

            List<RadioButton> rLst = new List<RadioButton>();
            GetControlList<RadioButton>(this.Controls, rLst);
            foreach (var rdo in rLst) rdo.Enabled = false;

            List<TextDateMask> tdmLst = new List<TextDateMask>();
            GetControlList<TextDateMask>(this.Controls, tdmLst);
            foreach (var tdm in tdmLst) tdm.Enabled = false;


            List<RadioButtonList> rrLst = new List<RadioButtonList>();
            GetControlList<RadioButtonList>(this.Controls, rrLst);
            foreach (var rdo in rrLst) rdo.Enabled = false;
        }
        public void GetPreleadData(decimal preleadId)
        {
            try
            {
                pnlPrelead.Visible = true;
                SlmScr046Biz bz = new SlmScr046Biz();
                var pl = bz.GetPreleadDetail(preleadId);
                if (pl != null)
                    ctlPrelead.SetData(pl);
            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            finally { SetAllView(); }
        }
        private void GetControlList<T>(ControlCollection controlCollection, List<T> resultCollection)
where T : Control
        {
            foreach (Control control in controlCollection)
            {
                //if (control.GetType() == typeof(T))
                if (control is T) // This is cleaner
                    resultCollection.Add((T)control);

                if (control.HasControls())
                    GetControlList(control.Controls, resultCollection);
            }
        }

        private void LoadDetailControl()
        {
            if (pnlLead.Visible == false) return;
            if (!String.IsNullOrEmpty(CampaignId))
            {
                SlmScr010Biz bz = new SlmScr010Biz();
                var ctl = bz.GetControlname(CampaignId, "V");
                Session[SLMConstant.SessionName.screen_code] = ctl;

                if (ctl != "")
                    ctlLead = (Lead_Detail_Master)LoadControl("~/Shared/" + ctl);
                else
                    ctlLead = (Lead_Detail_Master)LoadControl("~/Shared/Lead_Detail_Default.ascx");

                if (ctlLead != null)
                {
                    if (plcControl.Controls.Count == 0)
                    {
                        ctlLead.CommonData = ctlCommon.GetCommonData();
                        plcControl.Controls.Add(ctlLead);
                    }
                }
                else
                {
                    AppUtil.ClientAlert(this, "Invalid Control Name, Please contact administrator");
                }
            }
        }


        // old code ------>
        public void xGetLeadData(LeadData lead)
        {
            try
            {
                txtAddressNo.Text = lead.AddressNo;
                txtBuildingName.Text = lead.BuildingName;
                txtFloor.Text = lead.Floor;
                txtSoi.Text = lead.Soi;
                txtStreet.Text = lead.Street;
                txtTambon.Text = lead.TambolName;
                txtAmphur.Text = lead.AmphurName;
                txtProvince.Text = lead.ProvinceName;
                txtPostalCode.Text = lead.PostalCode;
                txtIsCustomer.Text = (string.IsNullOrEmpty(lead.IsCustomer) ? "": (lead.IsCustomer.Trim() == "1" ? "เคย" : "ไม่เคย"));
                txtCusCode.Text = lead.CusCode;
                txtOccupation.Text = lead.OccupationName;
                if (lead.BaseSalary != null)
                    txtBaseSalary.Text = lead.BaseSalary.Value.ToString("#,##0.00");
                txtLicenseNo.Text = lead.LicenseNo;
                txtYearOfCar.Text = lead.YearOfCar;
                txtYearOfCarRegis.Text = lead.YearOfCarRegis;
                txtBrand.Text = lead.BrandName;
                if (lead.CarPrice != null)
                    txtCarPrice.Text = lead.CarPrice.Value.ToString("#,##0.00");
                txtModel.Text = lead.ModelName;
                txtSubmodel.Text = lead.SubModelName;
                if (lead.DownPayment != null)
                    txtDownPayment.Text = lead.DownPayment.Value.ToString("#,##0.00");
                txtDownPercent.Text = lead.DownPercent.ToString();
                if (lead.FinanceAmt != null)
                    txtFinanceAmt.Text = lead.FinanceAmt.Value.ToString("#,##0.00");
                txtPaymentTerm.Text = lead.PaymentTerm;
                txtPaymentType.Text = lead.PaymentName;
                if (lead.BalloonAmt != null)
                    txtBalloonAmt.Text = lead.BalloonAmt.Value.ToString("#,##0.00");
                txtBalloonPercent.Text = lead.BalloonPercent.ToString();
                txtProvinceRegis.Text = lead.ProvinceRegisName;
                if (!string.IsNullOrEmpty(lead.CoverageDate))
                {
                    if (lead.CoverageDate.Trim().Length == 8)
                        txtCoverageDate.Text = lead.CoverageDate.Substring(6, 2) + "/" + lead.CoverageDate.Substring(4, 2) + "/" + lead.CoverageDate.Substring(0, 4);
                }
                txtPlanType.Text = lead.PlanBancName;
                txtAccType.Text = lead.AccTypeName;
                txtAccPromotion.Text = lead.PromotionName;
                txtAccTerm.Text = lead.AccTerm;
                txtInterest.Text = lead.Interest;
                if(!string.IsNullOrEmpty(lead.Invest))
                    txtInvest.Text = Convert.ToDecimal(lead.Invest).ToString("#,##0.00");
                txtLoanOd.Text = lead.LoanOd;
                txtLoanOdTerm.Text = lead.LoanOdTerm;
                txtEbank.Text = (string.IsNullOrEmpty(lead.Ebank) ? "": (lead.Ebank.Trim() == "1" ? "ใช่" : "ไม่ใช่"));
                txtAtm.Text = (string.IsNullOrEmpty(lead.Atm) ? "": (lead.Atm.Trim() == "1" ? "ใช่" : "ไม่ใช่"));
                txtCompany.Text = lead.Company;

                lbPathLink.Text = lead.PathLink;
                if (!string.IsNullOrEmpty(lead.PathLink))
                {
                    if (lead.PathLink.IndexOf("http") < 0)
                        lbPathLink.OnClientClick = "window.open('http://" + lead.PathLink + "'), '_blank'";
                    else
                        lbPathLink.OnClientClick = "window.open('" + lead.PathLink + "'), '_blank'";
                }
                
                if (lead.Birthdate != null)
                    txtBitrhDate.Text = lead.Birthdate.Value.ToString("dd/MM/") + lead.Birthdate.Value.Year.ToString();

                txtCardType.Text = lead.CardTypeDesc;
                txtCitizenId.Text = lead.CitizenId;
                txtTopic.Text = lead.Topic;
                txtDetail.Text = lead.Detail;
                txtChannelName.Text = lead.ChannelDesc;
                if (lead.CreatedDateView != null)
                    txtCreateDate.Text = lead.CreatedDateView.Value.ToString("dd/MM/") + lead.CreatedDateView.Value.Year.ToString();
                txtBranchName.Text = lead.BranchName;
                txtBranchprod.Text = lead.Branchprod;
                if (lead.CreatedDateView != null)
                    txtCreateTime.Text = lead.CreatedDateView.Value.ToString("HH:mm:ss");

                if (!string.IsNullOrEmpty(lead.AvailableTime) && lead.AvailableTime.Trim().Length == 6)
                {
                    txtAvailableTime.Text = lead.AvailableTime.Substring(0, 2) + ":" + lead.AvailableTime.Substring(2, 2) + ":" + lead.AvailableTime.Substring(4, 2);
                }
                txtEmail.Text = lead.Email;
                txtInterestedProd.Text = (string.IsNullOrEmpty(lead.CarType) ? "" : (lead.CarType.Trim() == "0" ? "รถใหม่" :(lead.CarType.Trim() == "1"?"รถเก่า":"")));
                txtCreateBy.Text = lead.LeadCreateBy;
                txtDealerCode.Text = lead.DealerCode;
                txtDealerName.Text = lead.DealerName;
                txtDealerName.ToolTip = lead.DealerName;
                txtContractNoRefer.Text = lead.ContractNoRefer;
            }
            catch
            {
                throw;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/SLM_SCR_003.aspx");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbReloadInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["ticketid"]))
                {
                    var lead = SlmScr004Biz.SearchSCR004Data(Request["ticketid"]);
                    GetLeadData(lead);
                }
                else if (!string.IsNullOrEmpty(Request["preleadid"]))
                {
                    GetPreleadData(SLMUtil.SafeDecimal(Request["preleadid"]));
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbReloadPreleadInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["ticketid"]))
                {
                    var lead = SlmScr004Biz.SearchSCR004Data(Request["ticketid"]);
                    GetLeadData(lead);
                }
                else if (!string.IsNullOrEmpty(Request["preleadid"]))
                {
                    GetPreleadData(SLMUtil.SafeDecimal(Request["preleadid"]));
                }
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