using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using SLM.Biz;
using SLM.Application.Utilities;
using SLM.Resource.Data;
using SLM.Resource;
using log4net;

namespace SLM.Application.MasterPage
{
    public partial class SaleLead : System.Web.UI.MasterPage
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SaleLead));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        DisplayUserFullname();  //must be first called
                        SetAvailableButton();
                        SetMenu();
                        GetAssemblyVersion();
                        tmNotification.Interval = SLMConstant.NotificationInterval;
                        ShowNumberOfNewNotification();
                        SetReportMenu();
                    }
                    else
                    {
                        Response.Redirect(FormsAuthentication.LoginUrl);
                    } 
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("PageLoad " + message);
                _log.Debug("PageLoad ", ex);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DisplayUserFullname()
        {
            FormsIdentity identity = (FormsIdentity)HttpContext.Current.User.Identity;
            FormsAuthenticationTicket ticket = identity.Ticket;
            string[] data = ticket.UserData.Split('|');
            lblUserFullname.Text = data[0];
            lblBranchName.Text = data[1];
            txtStaffTypeId.Text = data[2];
            txtUsername.Text = ticket.Name;

            //HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName);
            //FormsAuthenticationTicket decTicket = FormsAuthentication.Decrypt(cookie.Value);
            //lblUserFullname.Text = decTicket.UserData;
        }

        private void GetAssemblyVersion()
        {
            try
            {
                lblAssemblyVersion.Text = AppUtil.GetAssemblyVersion();
            }
            catch (Exception ex)
            {
                _log.Debug("GetAssemblyVersion " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            }
        }

        #region Backup 17/05/2017
        //private void SetMenu()
        //{
        //    try
        //    {
        //        var staffTypeId = SlmMasterBiz.GetStaffTypeId(HttpContext.Current.User.Identity.Name);
        //        if (staffTypeId != null)
        //        {
        //            List<ScreenPrivilegeData> list = SlmMasterBiz.GetScreenPrivillegeList(Convert.ToInt32(staffTypeId));

        //            menuUserMonitoring.Visible = (list.Where(p => p.ScreenId == 4 && p.IsView == 1).Count() > 0);       //UserMonitoring
        //            menuCampaignRecommend.Visible = (list.Where(p => p.ScreenId == 5 && p.IsView == 1).Count() > 0);    //แนะนำแคมเปญ
        //            menuUserManagement.Visible = (list.Where(p => p.ScreenId == 6 && p.IsView == 1).Count() > 0);       //UserManagement
        //            menuNotice.Visible = (list.Where(p => p.ScreenId == 20 && p.IsView == 1).Count() > 0);              //ข้อมูลประกาศ
        //            menuPosition.Visible = (list.Where(p => p.ScreenId == 21 && p.IsView == 1).Count() > 0);            //ข้อมูลตำแหน่ง
        //            menuSLA.Visible = (list.Where(p => p.ScreenId == 22 && p.IsView == 1).Count() > 0);                 //ข้อมูล SLA
        //            menuAssign.Visible = (list.Where(p => p.ScreenId == 23 && p.IsView == 1).Count() > 0);              //ข้อมูล กำหนดค่าการจ่ายงาน
        //            menuPrivilege.Visible = (list.Where(p => p.ScreenId == 24 && p.IsView == 1).Count() > 0);           //ข้อมูล สิทธิ์การเข้าถึงข้อมูล
        //            menuActivityPrivilege.Visible = (list.Where(p => p.ScreenId == 25 && p.IsView == 1).Count() > 0);   //ข้อมูล กำหนดเงื่อนไขการบันทึกผลการติดต่อ
        //            menuBranchHoliday.Visible = (list.Where(p => p.ScreenId == 26 && p.IsView == 1).Count() > 0);       //ข้อมูล กำหนดวันหยุดสาขา
        //            menuBranch.Visible = (list.Where(p => p.ScreenId == 27 && p.IsView == 1).Count() > 0);              //ข้อมูล ข้อมูลสาขา

        //            //OBT Phase
        //            menuSearchObt.Visible = (list.Where(p => p.ScreenId == 29 && p.IsView == 1).Count() > 0);           //ข้อมูล ค้นหา OBT
        //            menuConfigRule.Visible = (list.Where(p => p.ScreenId == 30 && p.IsView == 1).Count() > 0);          //กำหนดเงื่อนไขการจ่ายงาน OBT
        //            menuReceiveNo.Visible = (list.Where(p => p.ScreenId == 31 && p.IsView == 1).Count() > 0);           //จัดการข้อมูลเลขรับแจ้ง
        //            menuCheckRuleAssign.Visible = (list.Where(p => p.ScreenId == 32 && p.IsView == 1).Count() > 0);     //ตรวจสอบการจ่ายงาน
        //            menuPromotion.Visible = (list.Where(p => p.ScreenId == 33 && p.IsView == 1).Count() > 0);           //นำเข้าข้อมูลโปรโมชั่น
        //            menuImportProblem.Visible = (list.Where(p => p.ScreenId == 34 && p.IsView == 1).Count() > 0);       //นำเข้างานติดปัญหา
        //            menuBenefit.Visible = (list.Where(p => p.ScreenId == 35 && p.IsView == 1).Count() > 0);             //ข้อมูลผลประโยชน์
        //            menuScriptQA.Visible = (list.Where(p => p.ScreenId == 36 && p.IsView == 1).Count() > 0);            //Script QA
        //            menuUserMonitoringReInsurance.Visible = (list.Where(p => p.ScreenId == 37 && p.IsView == 1).Count() > 0);   //User Monitoring RenewInsurance
        //            menuTeamTelesales.Visible = (list.Where(p => p.ScreenId == 38 && p.IsView == 1).Count() > 0);
        //            menuCarType.Visible = (list.Where(p => p.ScreenId == 39 && p.IsView == 1).Count() > 0);
        //            menuNotifyPremium.Visible = (list.Where(p => p.ScreenId == 40 && p.IsView == 1).Count() > 0);       //นำเข้าข้อมูลแจ้งเบี้ย
        //            menuTelesalesPerformance.Visible = (list.Where(p => p.ScreenId == 41 && p.IsView == 1).Count() > 0);    //นำเข้าข้อมูลผลงาน Telesales
        //            menuInsurCom.Visible = (list.Where(p => p.ScreenId == 42 && p.IsView == 1).Count() > 0);            //ข้อมูลบริษัทประกันภัย
        //            menuPremium.Visible = (list.Where(p => p.ScreenId == 43 && p.IsView == 1).Count() > 0);             //ข้อมูล Premium
        //            menuReport.Visible = (list.Where(p => p.ScreenId == 44 && p.IsView == 1).Count() > 0);              //รายงาน
        //            menuRoleDiscount.Visible = (list.Where(p => p.ScreenId == 62 && p.IsView == 1).Count() > 0);
        //            menuPremiumCar.Visible = (list.Where(p => p.ScreenId == 63 && p.IsView == 1).Count() > 0);
        //        }
        //        else
        //        {
        //            menuUserMonitoring.Visible = false;
        //            menuCampaignRecommend.Visible = false;
        //            menuUserManagement.Visible = false;
        //            menuNotice.Visible = false;
        //            menuPosition.Visible = false;
        //            menuSLA.Visible = false;
        //            menuAssign.Visible = false;
        //            menuPrivilege.Visible = false;
        //            menuActivityPrivilege.Visible = false;
        //            menuBranchHoliday.Visible = false;
        //            menuBranch.Visible = false;

        //            //OBT Phase
        //            menuSearchObt.Visible = false;
        //            menuConfigRule.Visible = false;
        //            menuReceiveNo.Visible = false;
        //            menuCheckRuleAssign.Visible = false;
        //            menuPromotion.Visible = false;
        //            menuImportProblem.Visible = false;
        //            menuBenefit.Visible = false;
        //            menuScriptQA.Visible = false;
        //            menuUserMonitoringReInsurance.Visible = false;
        //            menuNotifyPremium.Visible = false;
        //            menuTelesalesPerformance.Visible = false;
        //            menuInsurCom.Visible = false;
        //            menuPremium.Visible = false;
        //            menuReport.Visible = false;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Error("SetMenu " + message);
        //        _log.Debug("SetMenu ", ex);
        //    }
        //}
        #endregion

        private void SetMenu()
        {
            try
            {
                var staffTypeId = txtStaffTypeId.Text.Trim();   //SlmMasterBiz.GetStaffTypeId(HttpContext.Current.User.Identity.Name);
                if (!string.IsNullOrWhiteSpace(staffTypeId))
                {
                    var list = SlmMasterBiz.GetScreenPrivillegeList(Convert.ToInt32(staffTypeId));

                    menuUserMonitoring.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_006" && p.IsView == 1);         //UserMonitoring
                    menuCampaignRecommend.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_007" && p.IsView == 1);      //แนะนำแคมเปญ
                    menuUserManagement.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_008" && p.IsView == 1);         //UserManagement
                    menuNotice.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_021" && p.IsView == 1);                 //ข้อมูลประกาศ
                    menuPosition.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_026" && p.IsView == 1);               //ข้อมูลตำแหน่ง
                    menuSLA.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_022" && p.IsView == 1);                    //ข้อมูล SLA
                    menuAssign.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_023" && p.IsView == 1);                 //ข้อมูล กำหนดค่าการจ่ายงาน
                    menuPrivilege.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_024" && p.IsView == 1);              //ข้อมูล สิทธิ์การเข้าถึงข้อมูล
                    menuActivityPrivilege.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_025" && p.IsView == 1);      //ข้อมูล กำหนดเงื่อนไขการบันทึกผลการติดต่อ
                    menuBranchHoliday.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_027" && p.IsView == 1);          //ข้อมูล กำหนดวันหยุดสาขา
                    menuBranch.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_028" && p.IsView == 1);                 //ข้อมูล ข้อมูลสาขา

                    //OBT Phase
                    menuSearchObt.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_029" && p.IsView == 1);              //ข้อมูล ค้นหา OBT
                    menuConfigRule.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_030" && p.IsView == 1);             //กำหนดเงื่อนไขการจ่ายงาน OBT
                    menuReceiveNo.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_031" && p.IsView == 1);              //จัดการข้อมูลเลขรับแจ้ง
                    menuCheckRuleAssign.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_032" && p.IsView == 1);        //ตรวจสอบการจ่ายงาน
                    menuPromotion.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_033" && p.IsView == 1);              //นำเข้าข้อมูลโปรโมชั่น
                    menuImportProblem.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_034" && p.IsView == 1);          //นำเข้างานติดปัญหา
                    menuBenefit.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_035" && p.IsView == 1);                //ข้อมูลผลประโยชน์
                    menuScriptQA.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_036" && p.IsView == 1);               //Script QA
                    menuUserMonitoringReInsurance.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_037" && p.IsView == 1);      //User Monitoring RenewInsurance
                    menuTeamTelesales.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_038" && p.IsView == 1);
                    menuCarType.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_039" && p.IsView == 1);
                    menuNotifyPremium.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_040" && p.IsView == 1);          //นำเข้าข้อมูลแจ้งเบี้ย
                    menuTelesalesPerformance.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_041" && p.IsView == 1);    //นำเข้าข้อมูลผลงาน Telesales
                    menuInsurCom.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_042" && p.IsView == 1);               //ข้อมูลบริษัทประกันภัย
                    menuPremium.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_043" && p.IsView == 1);                //ข้อมูล Premium
                    menuReport.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_044" && p.IsView == 1);                 //รายงาน
                    menuRoleDiscount.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_062" && p.IsView == 1);
                    menuPremiumCar.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_063" && p.IsView == 1);
                    menuUploadLead.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_055" && p.IsView == 1);             //Upload Lead
                    menuReassignLead.Visible = list.Any(p => p.ScreenDesc == "SLM_SCR_058" && p.IsView == 1);           //Re-Assign Lead
                }
                else
                {
                    menuUserMonitoring.Visible = false;
                    menuCampaignRecommend.Visible = false;
                    menuUserManagement.Visible = false;
                    menuNotice.Visible = false;
                    menuPosition.Visible = false;
                    menuSLA.Visible = false;
                    menuAssign.Visible = false;
                    menuPrivilege.Visible = false;
                    menuActivityPrivilege.Visible = false;
                    menuBranchHoliday.Visible = false;
                    menuBranch.Visible = false;

                    //OBT Phase
                    menuSearchObt.Visible = false;
                    menuConfigRule.Visible = false;
                    menuReceiveNo.Visible = false;
                    menuCheckRuleAssign.Visible = false;
                    menuPromotion.Visible = false;
                    menuImportProblem.Visible = false;
                    menuBenefit.Visible = false;
                    menuScriptQA.Visible = false;
                    menuUserMonitoringReInsurance.Visible = false;
                    menuTeamTelesales.Visible = false;
                    menuCarType.Visible = false;
                    menuNotifyPremium.Visible = false;
                    menuTelesalesPerformance.Visible = false;
                    menuInsurCom.Visible = false;
                    menuPremium.Visible = false;
                    menuReport.Visible = false;
                    menuRoleDiscount.Visible = false;
                    menuPremiumCar.Visible = false;

                    menuUploadLead.Visible = false;
                    menuReassignLead.Visible = false;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("SetMenu " + message);
                _log.Debug("SetMenu ", ex);
            }
        }

        private void SetReportMenu()
        {
            var rBiz = new ReportMenuBiz();
            var lst = rBiz.GetMenuList(HttpContext.Current.User.Identity.Name, string.IsNullOrWhiteSpace(txtStaffTypeId.Text.Trim()) ? 0 : int.Parse(txtStaffTypeId.Text.Trim()));
            rptMain.DataSource = lst;
            rptMain.DataBind();
        }

        protected void imbLogout_Click(object sender, ImageClickEventArgs e)
        {
            ViewState.Clear();
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect(FormsAuthentication.LoginUrl);
            //FormsAuthentication.RedirectToLoginPage();
        }

        protected void btnNotAvailable_Click(object sender, EventArgs e)
        {
            try
            {
                SetUserActiveStatus(0);
                SetUnavailable();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("btnNotAvailable_Click " + message);
                _log.Debug("btnNotAvailable_Click ", ex);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnAvailable_Click(object sender, EventArgs e)
        {
            try
            {
                SetUserActiveStatus(1);
                SetAvailable();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("btnAvailable_Click " + message);
                _log.Debug("btnAvailable_Click", ex);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #region Button

        protected void lbSearchLead_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_003.aspx");
        }

        protected void lbUserMonitoring_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_015.aspx");
        }

        protected void lbCampaignRecommend_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_016.aspx");
        }

        protected void lbUserManagement_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_017.aspx");
        }

        protected void lbSuggesstiont_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_020.aspx");
        }

        protected void lbNotice_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_021.aspx");
        }

        protected void lbSLA_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_022.aspx");
        }

        protected void lbAssign_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_023.aspx");
        }

        protected void lbPrivilege_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_024.aspx");
        }

        protected void lbActivityPrivilege_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_025.aspx");
        }

        protected void lbPosition_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_026.aspx");
        }

        protected void lbBranchHoliday_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_027.aspx");
        }

        protected void lbBranch_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_028.aspx");
        }

        protected void lbSearchObt_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_029.aspx");
        }

        protected void lbUserMonitoringReInsurance_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_037.aspx");
        }

        protected void lbConfigRule_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_030.aspx");
        }

        protected void lbReceiveNo_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_031.aspx");
        }

        protected void lbCheckRuleAssign_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_047.aspx");
        }

        protected void lbPromotion_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_033.aspx");
        }

        protected void lbImportProblem_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_034.aspx");
        }

        protected void lbBenefit_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_035.aspx");
        }

        protected void lbScriptQA_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_036.aspx");
        }

        protected void lbNotifyPremium_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_040.aspx");
        }

        protected void lbTelesalesPerformance_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_041.aspx");
        }

        protected void lbInsurCom_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_042.aspx");
        }

        protected void lbPremium_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_043.aspx");
        }

        protected void lbReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_044.aspx");
        }

        protected void lbTeamTelesales_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_038.aspx");
        }

        protected void lbCarType_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_039.aspx");
        }

        protected void lbRoleDiscount_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_062.aspx");
        }

        protected void lbPremiumCar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_063.aspx");
        }

        protected void lbUploadLead_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_055.aspx");
        }

        protected void lbReassignLead_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_058.aspx");
        }

        #endregion


        private void SetAvailableButton()
        {
            string status = StaffBiz.GetActiveStatusByAvailableConfig(txtUsername.Text.Trim());
            if (status == "0")
            {
                SetUnavailable();
            }
            else if (status == "1")
            {
                SetAvailable();
            }
            else
            {
                HideStatusDetail();
            }
        }

        private void HideStatusDetail()
        {
            imgAvailable.Visible = false;
            imgNotAvailable.Visible = false;
            lblStatusDesc.Text = string.Empty;
            btnAvailable.Visible = false;
            btnNotAvailable.Visible = false;
        }

        private void SetAvailable()
        {
            imgAvailable.Visible = true;
            imgNotAvailable.Visible = false;
            lblStatusDesc.Text = "<b>สถานะ : </b>พร้อมทำงาน (Available)";
            btnAvailable.Visible = false;
            btnNotAvailable.Visible = true;
        }

        private void SetUnavailable()
        {
            imgAvailable.Visible = false;
            imgNotAvailable.Visible = true;
            lblStatusDesc.Text = "<b>สถานะ : </b>ไม่พร้อมทำงาน (Unavailable)";
            btnAvailable.Visible = true;
            btnNotAvailable.Visible = false;
        }

        private void SetUserActiveStatus(int status)
        {
            StaffBiz.SetActiveStatus(txtUsername.Text.Trim(), status);
        }

        #region Notification

        protected void tmNotification_Tick(object sender, EventArgs e)
        {
            try
            {
                if (divNotification.Visible)
                {
                    ShowNotificationList();
                    upRptNotification.Update();
                }
                else
                {
                    ShowNumberOfNewNotification();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("tmNotification_Tick " + message);
                _log.Debug("tmNotification_Tick", ex);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ShowNumberOfNewNotification()
        {
            try
            {
                NotificationBiz nb = new NotificationBiz();
                var nn = nb.GetNewNotification(HttpContext.Current.User.Identity.Name);
                if (nn != 0)
                {
                    lblNotification.Text = "&nbsp;" + nn.ToString("#,##0") + "&nbsp;";
                    lblNotification.Visible = true;
                }
                else
                {
                    lblNotification.Visible = false;
                }
            }
            catch
            {
                throw;
            }
        }

        protected void imbNotification_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (divNotification.Visible)
                {
                    divNotification.Visible = false;
                    rptNotification.DataSource = null;
                    rptNotification.DataBind();
                }
                else
                {
                    ShowNotificationList();
                    divNotification.Visible = true;
                    ShowNumberOfNewNotification();
                }

                upNotification.Update();
                upNotificationList.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("imbNotification_Click " + message);
                _log.Debug("imbNotification_Click", ex);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ShowNotificationList()
        {
            try
            {
                NotificationBiz biz = new NotificationBiz();
                rptNotification.DataSource = biz.GetNotificationList(HttpContext.Current.User.Identity.Name);
                rptNotification.DataBind();
            }
            catch
            {
                throw;
            }
        }

        #endregion

        protected void rptNotification_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    if (((Label)e.Item.FindControl("lblNoticeType")).Text.Trim() == "1")                    //Assign
                        ((Image)e.Item.FindControl("imgNoticeIcon")).ImageUrl = "~/Images/check_user_icon.png";
                    else if (((Label)e.Item.FindControl("lblNoticeType")).Text.Trim() == "2")               //Change Status
                        ((Image)e.Item.FindControl("imgNoticeIcon")).ImageUrl = "~/Images/gear_sidemenu_icon4.png";
                    else if (((Label)e.Item.FindControl("lblNoticeType")).Text.Trim() == "3")               //SLA
                        ((Image)e.Item.FindControl("imgNoticeIcon")).ImageUrl = "~/Images/exclamation_circle_red.png";
                    else if (((Label)e.Item.FindControl("lblNoticeType")).Text.Trim() == "4")               //Note
                        ((Image)e.Item.FindControl("imgNoticeIcon")).ImageUrl = "~/Images/chat_icon.png";

                    string ticketId = ((Label)e.Item.FindControl("lblTicketId")).Text.Trim();
                    LinkButton lbNoticeTitle = (LinkButton)e.Item.FindControl("lbNoticeTitle");

                    string title = lbNoticeTitle.Text.Trim().Replace("SLM:", "").Replace("-[]", "").Replace("-[ ]", "");
                    int index = title.ToLower().IndexOf("ticket");
                    string statusSection = title.Substring(0, index);
                    lbNoticeTitle.Text = title.Replace(statusSection, "<b>" + statusSection + "</b>")
                                                .Replace("Ticket:", "<b>Ticket:</b>")
                                                .Replace(ticketId, "<b>" + ticketId + "</b><br/>");

                    //lbNoticeTitle.Text = lbNoticeTitle.Text.Replace("SLM:", "").Replace(ticketId, ticketId + "<br/>");

                    //lbNoticeTitle.Text = title.Substring(index).Replace("Ticket:", "<b>Ticket:</b>").Replace(ticketId, "<b>" + ticketId + "</b><br/>");
                    //lbNoticeTitle.Text = lbNoticeTitle.Text.Replace("SLM: Ticket:", "<b>SLM: Ticket:</b>").Replace(ticketId, "<b>" + ticketId + "</b><br/>");

                    if (((Label)e.Item.FindControl("lblNoticeStatus")).Text.Trim().ToUpper() != "R")
                    {
                        ((System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("trNotice")).Style.Add("background-color", "#eeeff4");
                        //((System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("trNotice")).Style.Add("cursor", "pointer");
                        //((System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("trNotice")).Attributes.Add("onclick", string.Format("document.location='SLM_SCR_004.aspx?ticketid={0}';", ((Label)e.Item.FindControl("lblNoticeTicketId")).Text.Trim()));
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("rptNotification_ItemDataBound : " + message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbNoticeTitle_Click(object sender, EventArgs e)
        {
            try
            {
                int index = int.Parse(((LinkButton)sender).CommandArgument);
                Label lblTicketId = (Label)rptNotification.Items[index].FindControl("lblTicketId");
                Label lblNotificationId = (Label)rptNotification.Items[index].FindControl("lblNotificationId");

                NotificationBiz biz = new NotificationBiz();
                biz.UpdateStatus(decimal.Parse(lblNotificationId.Text.Trim()), "R");
                Response.Redirect("~/SLM_SCR_004.aspx?ticketid=" + lblTicketId.Text.Trim(), false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlert(Page, message);
            }
        }

        public void ShowReportMenu()
        {
            ulReportMenu.Visible = true;
            ulMainMenu.Visible = false;
        }

        //private void SetCurrentStatus(int status)
        //{
        //    SlmMasterBiz.SetCurrentStatus(txtUsername.Text.Trim(), status);
        //}

        //protected void imbSuggesstion_Click(object sender, ImageClickEventArgs e)
        //{
        //    mpeSuggestion.Show();
        //}

        //protected void btnCancelSuggestion_Click(object sender, EventArgs e)
        //{
        //    mpeSuggestion.Hide();
        //}
    }
}