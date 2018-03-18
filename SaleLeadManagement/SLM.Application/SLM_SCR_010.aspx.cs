using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Application.Shared;
using SLM.Application.Utilities;
using SLM.Resource;

namespace SLM.Application
{
    public partial class SLM_SCR_010x : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_010x));
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ctlCommon.SetAddCampaignCombo();
            if (IsPostBack)
                LoadDetailControl();
        }
        Lead_Detail_Master ctlLead;
        string CampaignId
        {
            get { return Session["SCR010.CAMPAIGNID"] as string; }
            set { Session["SCR010.CAMPAIGNID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        { 
            try
            {
                if (!IsPostBack)
                {
                    CampaignId = "";
                    ((Label)Page.Master.FindControl("lblTopic")).Text = "เพิ่มข้อมูล Lead (Add)";

                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_010");
                    if (priData == null || priData.IsView != 1)
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");


                    //รับค่า Seaaion มากจากหน้า viewlead, ปุ่ม คัดลอกข้อมูลผู้มุ่งหวัง
                    if (Session["ticket_id"] != null)
                    {
                        hdfCopyTicket.Value = Session["ticket_id"] as string;
                        Session.Remove("ticket_id");

                        // load default data
                        //var ticketid = Session["ticket_id"] as string;
                        //LeadData lead = SlmScr010Biz.GetLeadData(ticketid);
                        // Session["ticket_id"] = null;
                        LeadData lead = SlmScr010Biz.GetLeadData(hdfCopyTicket.Value);
                        lead.Delegate = "";
                        lead.Delegate_Branch = "";
                        lead.Delegate_Flag = 0;
                        
                        if (lead != null)
                        {
                            //CampaignId = lead.CampaignId;
                            ctlCommon.SetLeadData(lead, true, true);

                            //LoadDetailControl();
                            //ctlLead.LoadData(lead);
                        }
                    }
                    ctlCommon.SetControlMode(Lead_Detail_Master.CtlMode.New);

                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void LoadDetailControl()
        {
            if (!String.IsNullOrEmpty(CampaignId))
            {
                //หา Detail Control โดยใช้เลข CampaignId ไปหาใน Table kkslm_ms_config_product_screen
                SlmScr010Biz bz = new SlmScr010Biz();
                var ctl = bz.GetControlname(CampaignId, "I");
                if (ctl != "")
                    ctlLead = (Lead_Detail_Master)LoadControl("~/Shared/" + ctl);
                else
                    ctlLead = (Lead_Detail_Master)LoadControl("~/Shared/Lead_Detail_Default.ascx");

                if (ctlLead != null)
                {
                    ctlLead.CommonData = ctlCommon.GetCommonData();
                    plcControl.Controls.Add(ctlLead);
                }
                else
                    AppUtil.ClientAlert(this, "Invalid Control Name, Please contact administrator");

            }
        }

        // ----- button action

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CampaignId == null) AppUtil.ClientAlertAndRedirect(this, "Session Expired", Request.Url.PathAndQuery);

                var ticketid = "";
                ctlLead.CommonData = ctlCommon.GetCommonData();
                var check1 = ctlCommon.ValidateData();
                var check2 = ctlLead.ValidateData();

                if (!check1 || !check2)
                {
                    AppUtil.ClientAlert(this.Page, "ข้อมูลไม่ครบถ้วน");
                }
                else
                {
                    DropDownList cmbCardType = ctlLead.GetComboCardType();
                    TextBox txtCitizenId = ctlLead.GetTextBoxCitizenId();
                    TextBox txtTelNo1 = ctlCommon.GetTextBoxTelNo1();
                    Label lblAlertTelNo1 = ctlCommon.GetAlertTelNo1();

                    if (cmbCardType.SelectedItem.Value != "" && txtCitizenId.Text.Trim() != "" && txtTelNo1.Text.Trim() != "")
                    {
                        if (!AppUtil.ValidateTelNo1(cmbCardType, txtTelNo1, lblAlertTelNo1))
                        {
                            AppUtil.ClientAlert(this.Page, "ข้อมูลไม่ครบถ้วน");
                            return;
                        }
                    }

                    if (ctlLead.SaveData(ticketid, Page.User.Identity.Name))
                    {
                        //Session.Remove("ticket_id");
                        hdfCopyTicket.Value = "";
                        var cm = ctlLead.CommonData;

                        lblResultTicketId.Text = cm.R_TicketID;
                        lblResultMessage.Text = cm.R_Message;
                        lblResultOwnerLead.Text = cm.R_Owner;
                        lblResultCampaign.Text = cm.R_CampaignName;
                        lblResultChannel.Text = cm.R_ChannelName;
                        cbResultHasAdamsUrl.Checked = cm.R_HasAdams;

                        mpePopupSaveResult.Show();
                        upPopupSaveResult.Update();

                        btnSave.Enabled = false;
                    }
                    else
                    {
                        AppUtil.ClientAlert(this.Page, ctlLead.CommonData.R_Message);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_003.aspx");
            //Response.Redirect(Request.Url.PathAndQuery);
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ctlCommon.ValidateData())
                {
                    AppUtil.ClientAlert(this, "ข้อมูลไม่ครบถ้วน");
                    return;
                }
                if (ctlCommon.CampaignID != "")
                {
                    CampaignId = ctlCommon.CampaignID;
                    LoadDetailControl();
                    ctlLead.SetControlMode(Lead_Detail_Master.CtlMode.New);
                    tbNext.Visible = false;
                    tbSave.Visible = true;
                    ctlCommon.SetEnableCampaign(false);

                    // copy lead
                    //if (Session["ticket_id"] != null)
                    if (hdfCopyTicket.Value.Trim() != "") 
                    {
                        //var ticketId = Session["ticket_id"] as string;
                        var lead = SlmScr010Biz.GetLeadData(hdfCopyTicket.Value);
                        string type = new ConfigProductScreenBiz().GetFieldType(lead.CampaignId, lead.ProductId, SLMConstant.ConfigProductScreen.ActionType.View);

                        // remove unsed data

                        // load data from old lead
                        ctlLead.LoadData(lead, true, type);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCancelNext_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_003.aspx");
        }


        protected void btnAttachDocYes_Click(object sender, EventArgs e)
        {
            try
            {
                btnAttachDocYes.Enabled = false;
                btnAttachDocNo.Enabled = false;

                string type = new ConfigProductScreenBiz().GetFieldType(lblResultTicketId.Text.Trim(), SLMConstant.ConfigProductScreen.ActionType.View);

                if (cbResultHasAdamsUrl.Checked)
                {
                    LeadDataForAdam leadData = SlmScr003Biz.GetLeadDataForAdam(lblResultTicketId.Text.Trim());
                    StaffData staff = SlmScr003Biz.GetStaff(HttpContext.Current.User.Identity.Name);

                    string script = AppUtil.GetCallAdamScript(leadData, HttpContext.Current.User.Identity.Name, (staff.EmpCode != null ? staff.EmpCode : ""), true, type);

                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "calladam", script, true);
                }
                else
                    Response.Redirect("SLM_SCR_004.aspx?ticketid=" + lblResultTicketId.Text.Trim() + "&type=" + type);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_003.aspx");
            }
        }

        protected void btnAttachDocNo_Click(object sender, EventArgs e)
        {
            try
            {
                CampaignId = "";
                lblResultTicketId.Text = "";
                plcControl.Controls.Clear();
                btnAttachDocYes.Enabled = false;
                btnAttachDocNo.Enabled = false;
                mpePopupSaveResult.Hide();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            Response.Redirect("SLM_SCR_003.aspx");
        }


    }
}