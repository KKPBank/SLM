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

namespace SLM.Application
{
    public partial class SLM_SCR_011x : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_011x));
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
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
                    ((Label)Page.Master.FindControl("lblTopic")).Text = "แก้ไขข้อมูล Lead (Edit)";
                    Page.Form.DefaultButton = btnSave.UniqueID;

                    ctlCommon.SetControlMode(Lead_Detail_Master.CtlMode.Edit);

                    if (Request["ticketid"] != null && Request["ticketid"].ToString().Trim() != string.Empty)
                    {
                        CheckTicketIdPrivilege(Request["ticketid"].ToString().Trim());
                    }
                    else
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "Ticket Id not found", "SLM_SCR_003.aspx");
                        return;
                    }

                    if (!string.IsNullOrEmpty(Request["ticketid"]))
                    {
                        var ticketid = Request["ticketid"];
                        LeadData lead = SlmScr010Biz.GetLeadData(ticketid);
                        if (lead == null)
                        {
                            AppUtil.ClientAlertAndRedirect(Page, "ไม่พบ Ticket Id " + ticketid + " ในระบบ", "SLM_SCR_003.aspx");
                            return;
                        }


                        if (!CheckTicketCloseOrTicketCOC(lead)) { return; }

                        CampaignId = lead.CampaignId;
                        ctlCommon.SetLeadData(lead);
                        LoadDetailControl();
                        ctlLead.LoadData(lead);

                        //InitialControl();
                        //SetScript();
                    }
                    else
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "Ticket Id not found", "SLM_SCR_003.aspx");
                        return;
                    }

                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_011");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    //Check สิทธิ์ภัทรในการเข้าใช้งาน
                    StaffData staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                    ConfigBranchPrivilegeData data = ConfigBranchPrivilegeBiz.GetConfigBranchPrivilege(staff.BranchCode);
                    if (data != null)
                    {
                        if (data.IsEdit != null && data.IsEdit.Value == false)
                        {
                            AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                            return;
                        }
                    }
                    //------------------------------------------------------------------------------------------------------

                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var ticketid = "";
                ctlLead.CommonData = ctlCommon.GetCommonData();              

                var check1 = ctlCommon.ValidateData(false);
                var check2 = ctlLead.ValidateData();

                if (!check1 || !check2)
                {
                    AppUtil.ClientAlert(this.Page, "ข้อมูลไม่ครบถ้วน");
                }
                else
                {
                    if (ctlLead.SaveData(ticketid, Page.User.Identity.Name))
                    {
                        btnSave.Enabled = false;

                        var bk = Request["backtype"];
                        string url = "SLM_SCR_003.aspx";
                        if (bk == "2" || bk == "3") url = "SLM_SCR_029.aspx?backtype=" + bk;

                        AppUtil.ClientAlertAndRedirect(Page, "บันทึกข้อมูลผู้มุ่งหวังสำเร็จ", url);
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
            //Response.Redirect("~/SLM_SCR_003.aspx");
            var bk = Request["backtype"];
            string url = "~/SLM_SCR_003.aspx";
            if (bk == "2" || bk == "3") url = "SLM_SCR_029.aspx?backtype=" + bk;

            Response.Redirect(url, false);
        }

        private void CheckTicketIdPrivilege(string ticketId)
        {
            string logError = "";
            if (!RoleBiz.GetTicketIdPrivilege(ticketId, HttpContext.Current.User.Identity.Name, SlmScr003Biz.GetStaffType(HttpContext.Current.User.Identity.Name), "", "SLM_SCR_011", out logError))
            {
                if (!string.IsNullOrEmpty(logError))
                {
                    _log.Error(logError);
                }

                string message = "ข้อมูลผู้มุ่งหวังรายนี้ ท่านไม่มีสิทธิในการมองเห็น";
                LeadOwnerDelegateData data = SlmScr011Biz.GetOwnerAndDelegateName(ticketId);
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(data.OwnerName) && !string.IsNullOrEmpty(data.DelegateName))
                        message += " ณ ปัจจุบันผู้เป็นเจ้าของ คือ " + data.OwnerName.ToString().Trim() + " และ Delegate คือ " + data.DelegateName.ToString().Trim();
                    else if (!string.IsNullOrEmpty(data.OwnerName))
                        message += " ณ ปัจจุบันผู้เป็นเจ้าของ คือ " + data.OwnerName.ToString().Trim();
                    else if (!string.IsNullOrEmpty(data.DelegateName))
                        message += " ณ ปัจจุบัน Delegate คือ " + data.DelegateName.ToString().Trim();
                }
                else
                    message = "ไม่พบ Ticket Id " + Request["ticketid"].ToString() + " ในระบบ";

                AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_003.aspx");
            }
            else
            {
                if (!string.IsNullOrEmpty(logError))
                {
                    _log.Error(logError);
                }
            }
        }

        private bool CheckTicketCloseOrTicketCOC(LeadData lead)
        {
            if (lead.ISCOC == "1" && lead.COCCurrentTeam != SLMConstant.COCTeam.Marketing)
            {
                string message = "ข้อมูลผู้มุ่งหวังรายนี้ ไม่สามารถแก้ไขได้เนื่องจากเข้าระบบ COC แล้ว";
                AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_003.aspx");
                return false;
            }

            if (lead.Status == "08" || lead.Status == "09" || lead.Status == "10")
            {
                string message = "ข้อมูลผู้มุ่งหวังรายนี้ ไม่สามารถแก้ไขได้เนื่องจากอยู่ในสถานะ" + lead.StatusName;
                AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_003.aspx");
                return false;
            }

            return true;
        }

        private void LoadDetailControl()
        {
            if (!String.IsNullOrEmpty(CampaignId))
            {
                SlmScr010Biz bz = new SlmScr010Biz();
                var ctl = bz.GetControlname(CampaignId, "E");
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
        
    }
}