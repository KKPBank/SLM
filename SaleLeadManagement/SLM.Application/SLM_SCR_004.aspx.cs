using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.OleDb;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Application.Utilities;
using log4net;
using SLM.Resource;

namespace SLM.Application
{
    public partial class SLM_SCR_004 : System.Web.UI.Page
    {
        private LeadDefaultData lead = null;
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_004));
        public string CMTUserName = System.Configuration.ConfigurationManager.AppSettings["CMTUserName"];
        public string CMTPassword = System.Configuration.ConfigurationManager.AppSettings["CMTPassword"];
        public string CMTServiceName = System.Configuration.ConfigurationManager.AppSettings["CMTServiceName"];
        public string CMTSystemCode = System.Configuration.ConfigurationManager.AppSettings["CMTSystemCode"];
        public string CMTReferenceNo = System.Configuration.ConfigurationManager.AppSettings["CMTReferenceNo"];
        public int CMTCampaignNo = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CMTCampaignNo"]);
        private string CMTCampaignSession = "CMTCampaignSession";
        private string nextleadlist = SLMConstant.NextLeadList;
        private string excelfilepath = "excelfilepath";
        private string outputfilename = "outputfilename";

        private SLM.Application.Shared.Tabs.Tab004 ctlLeadInfo = null;
        private SLM.Application.Shared.Tabs.Tab005 ctlExistingLead = null;
        private SLM.Application.Shared.Tabs.Tab006 ctlExistingProduct = null;
        private SLM.Application.Shared.Tabs.Tab007 ctlOwnerLogging = null;
        private SLM.Application.Shared.Tabs.Tab008 ctlActivity = null;
        private SLM.Application.Shared.Tabs.Tab009 ctlNote = null;
        private SLM.Application.Shared.Obt.TabActRenewInsure ctlRenewInsurAct = null;
        private SLM.Application.Shared.Obt.TabInsureSummary ctlInsurSummary = null;

        protected override void OnInit(EventArgs e)
        {
            Page.Form.DefaultButton = btnSearchScript.UniqueID;
            if (!IsPostBack)
            {
                Session.Remove(SLMConstant.SessionName.tabscreenlist);
            }
        }

        public string SessionPrefix
        {
            get { if (ViewState["Session.Prefix"] == null) ViewState["Session.Prefix"] = Guid.NewGuid().ToString("N").Substring(0, 16); return ViewState["Session.Prefix"] as string; }
        }

        private void InitialTabAndUserControl()
        {
            try
            {
                if (string.IsNullOrEmpty(txtCampaignId.Text.Trim()) && string.IsNullOrEmpty(txtProductId.Text.Trim()))
                {
                    if (Session[SLMConstant.SessionName.CampaignId] == null || Session[SLMConstant.SessionName.ProductId] == null)
                    {
                        string[] arr = new string[2];
                        if (!string.IsNullOrEmpty(Request["preleadid"]) && string.IsNullOrEmpty(Request["ticketid"]))
                        {
                            arr = new PreleadBiz().GetProductCampaign(decimal.Parse(Request["preleadid"]));
                        }
                        else if (!string.IsNullOrEmpty(Request["ticketid"]))
                        {
                            arr = new LeadInfoBiz().GetProductCampaign(Request["ticketid"]);
                        }

                        txtCampaignId.Text = arr[0];
                        txtProductId.Text = arr[1];
                    }
                    else
                    {
                        txtCampaignId.Text = (string)Session[SLMConstant.SessionName.CampaignId];
                        txtProductId.Text = (string)Session[SLMConstant.SessionName.ProductId];
                        Session.Remove(SLMConstant.SessionName.CampaignId);
                        Session.Remove(SLMConstant.SessionName.ProductId);
                    }
                }

                GenerateTab(txtCampaignId.Text.Trim(), txtProductId.Text.Trim());
            }
            catch
            {
                throw;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                bool doGenRightMenuTabControls = false;

                _log.Debug("");
                _log.Debug($"Method=Page_Load, InitialTabAndUserControl Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                InitialTabAndUserControl();   //Generate Tab and User Controls
                _log.Debug($"Method=Page_Load, InitialTabAndUserControl End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                if (!IsPostBack)
                {
                    //Validate
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_004");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", GetSearchPageUrl());
                        return;
                    }
                    if (string.IsNullOrEmpty(Request["preleadid"]) && string.IsNullOrEmpty(Request["ticketid"]))
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "Prelead Id or Ticket Id not found", GetSearchPageUrl());
                        return;
                    }

                    _log.Debug($"Method=Page_Load, GetStaff Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    StaffData staff = SlmScr003Biz.GetStaff(HttpContext.Current.User.Identity.Name);
                    txtLoginEmpCode.Text = staff.EmpCode;
                    txtLoginStaffTypeId.Text = staff.StaffTypeId != null ? staff.StaffTypeId.ToString() : "";
                    txtLoginStaffTypeDesc.Text = staff.StaffTypeDesc;
                    txtLoginNameTH.Text = staff.StaffNameTH;
                    txtUserLoginChannelId.Text = staff.ChannelId;
                    txtUserLoginChannelDesc.Text = staff.ChannelDesc;
                    ClearSession();
                    _log.Debug($"Method=Page_Load, GetStaff End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                    //Do Process
                    var checkRoles = new decimal[] { SLMConstant.StaffType.Marketing, SLMConstant.StaffType.CallReminder, SLMConstant.StaffType.NormalExpired };

                    if (!string.IsNullOrEmpty(Request["preleadid"]) && string.IsNullOrEmpty(Request["ticketid"]))
                    {
                        lbTicketIdReferSection.Visible = false;
                        txtPreleadId.Text = Request["preleadid"];
                        ((Label)Page.Master.FindControl("lblTopic")).Text = "แสดงข้อมูล Prelead";

                        decimal preleadId;
                        if (!decimal.TryParse(txtPreleadId.Text.Trim(), out preleadId))
                        {
                            AppUtil.ClientAlertAndRedirect(Page, "Prelead Id is not in a correct format", GetSearchPageUrl());
                            return;
                        }

                        if (checkRoles.Contains(staff.StaffTypeId.GetValueOrDefault()))
                        {
                            bool ret = false;
                            if (Session[nextleadlist] != null)
                            {
                                var list = (List<NextLeadData>)Session[nextleadlist];
                                ret = list.Any(p => p.PreleadId == preleadId);
                            }
                            if (!ret)
                            {
                                _log.Debug($"Method=Page_Load, PreleadId={txtPreleadId.Text.Trim()}, CheckTicketIdPrivilegeRenewInsurance Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                                if (!CheckTicketIdPrivilegeRenewInsurance(staff, "", txtPreleadId.Text.Trim()))
                                {
                                    AppUtil.ClientAlertAndRedirect(Page, "ข้อมูลผู้มุ่งหวังรายนี้ ท่านไม่มีสิทธิในการมองเห็น", GetSearchPageUrl());
                                    return;
                                }
                                _log.Debug($"Method=Page_Load, PreleadId={txtPreleadId.Text.Trim()}, CheckTicketIdPrivilegeRenewInsurance End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                            }
                        }

                        _log.Debug($"Method=Page_Load, PreleadId={txtPreleadId.Text.Trim()}, GetPreleadData Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                        if (!GetPreleadData(txtPreleadId.Text.Trim(), "", "", ""))
                        {
                            AppUtil.ClientAlertAndRedirect(Page, "ไม่พบข้อมูลที่ PreleadId " + txtPreleadId.Text.Trim(), GetSearchPageUrl());
                            return;
                        }
                        _log.Debug($"Method=Page_Load, PreleadId={txtPreleadId.Text.Trim()}, GetPreleadData End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    }
                    else if (!string.IsNullOrEmpty(Request["ticketid"]))
                    {
                        lbTicketIdReferSection.Visible = true;
                        txtTicketID.Text = Request["ticketid"];
                        ((Label)Page.Master.FindControl("lblTopic")).Text = "แสดงข้อมูล Lead: " + txtTicketID.Text.Trim() + " (View)";

                        //string type = new ConfigProductScreenBiz().GetFieldType(txtTicketID.Text.Trim(), SLMConstant.ConfigProductScreen.ActionType.View);

                        if (checkRoles.Contains(staff.StaffTypeId.GetValueOrDefault()) && !string.IsNullOrWhiteSpace(Request["type"]))
                        {
                            bool ret = false;
                            if (Session[nextleadlist] != null)
                            {
                                var list = (List<NextLeadData>)Session[nextleadlist];
                                ret = list.Any(p => p.TicketId == txtTicketID.Text.Trim());
                            }
                            if (!ret)
                            {
                                _log.Debug($"Method=Page_Load, TicketId={txtTicketID.Text.Trim()}, CheckTicketIdPrivilegeRenewInsurance Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                                if (!CheckTicketIdPrivilegeRenewInsurance(staff, txtTicketID.Text.Trim(), ""))
                                {
                                    AppUtil.ClientAlertAndRedirect(Page, "ข้อมูลผู้มุ่งหวังรายนี้ ท่านไม่มีสิทธิในการมองเห็น", GetSearchPageUrl());
                                    return;
                                }
                                _log.Debug($"Method=Page_Load, TicketId={txtTicketID.Text.Trim()}, CheckTicketIdPrivilegeRenewInsurance End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                            }
                        }
                        else
                        {
                            _log.Debug($"Method=Page_Load, TicketId={txtTicketID.Text.Trim()}, CheckTicketIdPrivilege Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                            if (!CheckTicketIdPrivilege(txtTicketID.Text.Trim(), staff)) { return; }
                            _log.Debug($"Method=Page_Load, TicketId={txtTicketID.Text.Trim()}, CheckTicketIdPrivilege End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                        }

                        //เก็บไว้ส่งไปที่ Adam, AolSummaryReport (ย้ายไปด้านบน)         
                        //txtLoginEmpCode.Text = staff.EmpCode;
                        //txtLoginStaffTypeId.Text = staff.StaffTypeId != null ? staff.StaffTypeId.ToString() : "";
                        //txtLoginStaffTypeDesc.Text = staff.StaffTypeDesc;
                        //txtLoginNameTH.Text = staff.StaffNameTH;
                        //txtUserLoginChannelId.Text = staff.ChannelId;
                        //txtUserLoginChannelDesc.Text = staff.ChannelDesc;

                        //Check สิทธิ์ภัทรในการเข้าใช้งาน
                        _log.Debug($"Method=Page_Load, TicketId={txtTicketID.Text.Trim()}, GetConfigBranchPrivilege Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                        ConfigBranchPrivilegeData data = ConfigBranchPrivilegeBiz.GetConfigBranchPrivilege(staff.BranchCode);
                        if (data != null)
                        {
                            if (data.IsView != null && data.IsView.Value == false)
                            {
                                AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", GetSearchPageUrl());
                                return;
                            }
                        }
                        _log.Debug($"Method=Page_Load, TicketId={txtTicketID.Text.Trim()}, GetConfigBranchPrivilege End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                        //------------------------------------------------------------------------------------------------------

                        _log.Debug($"Method=Page_Load, TicketId={txtTicketID.Text.Trim()}, GetLeadData Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                        GetLeadData();
                        _log.Debug($"Method=Page_Load, TicketId={txtTicketID.Text.Trim()}, GetLeadData End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                        if (lead.ISCOC == "1" && lead.COCCurrentTeam != SLMConstant.COCTeam.Marketing)
                        {
                            btnAllCampaign.Visible = false;
                        }
                        //if (ctlExistingLead != null) ctlExistingLead.GetExistingLeadList(txtCitizenId.Text.Trim(), txtTelNo1.Text.Trim(), txtTicketID.Text.Trim());
                        //if (ctlExistingProduct != null) ctlExistingProduct.GetExistingProductList(txtCitizenId.Text.Trim());
                        //if (ctlOwnerLogging != null) ctlOwnerLogging.GetOwnerLogingList(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
                        //if (ctlActivity != null) ctlActivity.InitialControl(lead, txtLoginNameTH.Text.Trim());
                        //if (ctlRenewInsurAct != null) ctlRenewInsurAct.InitialControlLead(lead, txtLoginNameTH.Text.Trim());
                        //if (ctlNote != null) ctlNote.InitialControl(txtTicketID.Text.Trim());

                        if (ctlExistingLead != null) ctlExistingLead.SetDefaultValue(txtCitizenId.Text.Trim(), txtTelNo1.Text.Trim(), txtTicketID.Text.Trim());
                        if (ctlExistingProduct != null) ctlExistingProduct.SetDefaultValue(txtCitizenId.Text.Trim(),txtCountryId.Text,txtCardTypeId.Text);
                        if (ctlOwnerLogging != null) ctlOwnerLogging.SetDefaultValue(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
                        if (ctlActivity != null) ctlActivity.InitialControl(lead, txtLoginNameTH.Text.Trim());
                        if (ctlRenewInsurAct != null) ctlRenewInsurAct.InitialControlLead(lead, txtLoginNameTH.Text.Trim());
                        if (ctlNote != null) ctlNote.SetDefaultValue(txtTicketID.Text.Trim(), "");

                        //if (lead.PreleadId != null && ctlInsurSummary != null)
                        //    ctlInsurSummary.GetPreleadInsureSummary(lead.PreleadId.Value.ToString());
                        //if (lead.TicketId != null && ctlInsurSummary != null)
                        //    ctlInsurSummary.GetLeadInsureSummary(lead.TicketId);

                        if (!string.IsNullOrEmpty(Request["tab"]) && Request["tab"] == "008")
                        {
                            tabMain.ActiveTabIndex = 0;
                        }

                        upTabMain.Update();
                    }

                    _log.Debug($"Method=Page_Load, GenerateRightSideMenu Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    //ไว้หลัง InitialTabAndUserControl(), GetLeadData(), GetPreleadData() เสมอ
                    GenerateRightSideMenu(txtCampaignId.Text.Trim(), txtProductId.Text.Trim(), staff);
                    doGenRightMenuTabControls = true;
                    _log.Debug($"Method=Page_Load, GenerateRightSideMenu End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                }

                if (!doGenRightMenuTabControls)
                {
                    _log.Debug($"Method=Page_Load, GenerateRightMenuTabControl Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    GenerateRightMenuTabControl();
                    _log.Debug($"Method=Page_Load, GenerateRightMenuTabControl End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);

                if (Request["backtype"] == "2" || Request["backtype"] == "3")
                {
                    AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_029.aspx?backtype=" + Request["backtype"]);      //ค้นหา OBT
                }
                else
                {
                    AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_003.aspx");        //ค้นหา default
                }
            }
        }

        #region Backup Page_Load 20170821
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        bool doGenRightMenuTabControls = false;
        //        InitialTabAndUserControl();   //Generate Tab and User Controls

        //        if (!IsPostBack)
        //        {
        //            //Validate
        //            ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_004");
        //            if (priData == null || priData.IsView != 1)
        //            {
        //                AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", GetSearchPageUrl());
        //                return;
        //            }
        //            if (string.IsNullOrEmpty(Request["preleadid"]) && string.IsNullOrEmpty(Request["ticketid"]))
        //            {
        //                AppUtil.ClientAlertAndRedirect(Page, "Prelead Id or Ticket Id not found", GetSearchPageUrl());
        //                return;
        //            }

        //            StaffData staff = SlmScr003Biz.GetStaff(HttpContext.Current.User.Identity.Name);
        //            txtLoginEmpCode.Text = staff.EmpCode;
        //            txtLoginStaffTypeId.Text = staff.StaffTypeId != null ? staff.StaffTypeId.ToString() : "";
        //            txtLoginStaffTypeDesc.Text = staff.StaffTypeDesc;
        //            txtLoginNameTH.Text = staff.StaffNameTH;
        //            txtUserLoginChannelId.Text = staff.ChannelId;
        //            txtUserLoginChannelDesc.Text = staff.ChannelDesc;

        //            //เก็บข้อมูลถ้าเกิดการกดเข้ามาจากหน้าค้นหา OBT
        //            string citizenId = Session[SLMConstant.SessionName.tabinbound_citizenid] != null ? Convert.ToString(Session[SLMConstant.SessionName.tabinbound_citizenid]) : "";
        //            string licenseNo = Session[SLMConstant.SessionName.tabinbound_licenseno] != null ? Convert.ToString(Session[SLMConstant.SessionName.tabinbound_licenseno]) : "";
        //            string campaignId = Session[SLMConstant.SessionName.tabinbound_campaignid] != null ? Convert.ToString(Session[SLMConstant.SessionName.tabinbound_campaignid]) : "";
        //            string contractNo = Session[SLMConstant.SessionName.tabinbound_contractno] != null ? Convert.ToString(Session[SLMConstant.SessionName.tabinbound_contractno]) : "";
        //            Session.Remove(SLMConstant.SessionName.tabinbound_citizenid);
        //            Session.Remove(SLMConstant.SessionName.tabinbound_licenseno);
        //            Session.Remove(SLMConstant.SessionName.tabinbound_campaignid);
        //            Session.Remove(SLMConstant.SessionName.tabinbound_contractno);
        //            ClearSession();

        //            //Do Process
        //            if (!string.IsNullOrEmpty(Request["preleadid"]) && string.IsNullOrEmpty(Request["ticketid"]))
        //            {
        //                lbTicketIdReferSection.Visible = false;
        //                txtPreleadId.Text = Request["preleadid"];
        //                ((Label)Page.Master.FindControl("lblTopic")).Text = "แสดงข้อมูล Prelead";

        //                bool ret = false;
        //                if (
        //                    new[]
        //                    {
        //                        SLMConstant.StaffType.Marketing,
        //                        SLMConstant.StaffType.CallReminder,
        //                        SLMConstant.StaffType.NormalExpired
        //                    }.Contains(staff.StaffTypeId.GetValueOrDefault())
        //                )
        //                {
        //                    if ((!string.IsNullOrEmpty(citizenId) && !string.IsNullOrEmpty(licenseNo))
        //                        || (!string.IsNullOrEmpty(citizenId) && !string.IsNullOrEmpty(contractNo))
        //                        || (!string.IsNullOrEmpty(licenseNo) && !string.IsNullOrEmpty(contractNo))
        //                        || (!string.IsNullOrEmpty(contractNo)))
        //                    {
        //                        ret = GetPreleadData(txtPreleadId.Text.Trim(), licenseNo, campaignId, contractNo);
        //                    }
        //                    else
        //                    {
        //                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", GetSearchPageUrl());
        //                        return;
        //                    }
        //                }
        //                else
        //                    ret = GetPreleadData(txtPreleadId.Text.Trim(), "", "", "");

        //                if (!ret)
        //                {
        //                    AppUtil.ClientAlertAndRedirect(Page, "ไม่พบข้อมูลที่ PreleadId " + txtPreleadId.Text.Trim(), GetSearchPageUrl());
        //                    return;
        //                }
        //            }
        //            else if (!string.IsNullOrEmpty(Request["ticketid"]))
        //            {
        //                lbTicketIdReferSection.Visible = true;
        //                txtTicketID.Text = Request["ticketid"].ToString();

        //                bool isRenew = false;
        //                if ((!string.IsNullOrEmpty(citizenId) && !string.IsNullOrEmpty(licenseNo))
        //                        || (!string.IsNullOrEmpty(citizenId) && !string.IsNullOrEmpty(contractNo))
        //                        || (!string.IsNullOrEmpty(licenseNo) && !string.IsNullOrEmpty(contractNo)))
        //                {
        //                    isRenew = true;
        //                }

        //                //ถ้าไม่ใช่ Marketing และ ไม่ได้ส่งทะเบียนรถ + campaignId หรือ เลขที่สัญญา + campaignId ให้เช็ก privilege เหมือน slm เดิม
        //                if (
        //                    !(isRenew &&
        //                      new[]
        //                      {
        //                          SLMConstant.StaffType.Marketing,
        //                          SLMConstant.StaffType.CallReminder,
        //                          SLMConstant.StaffType.NormalExpired
        //                      }.Contains(staff.StaffTypeId.GetValueOrDefault())
        //                    )
        //                )
        //                {
        //                    if (!CheckTicketIdPrivilege(txtTicketID.Text.Trim())) { return; }
        //                }

        //                ((Label)Page.Master.FindControl("lblTopic")).Text = "แสดงข้อมูล Lead: " + txtTicketID.Text.Trim() + " (View)";

        //                //เก็บไว้ส่งไปที่ Adam, AolSummaryReport                       
        //                txtLoginEmpCode.Text = staff.EmpCode;
        //                txtLoginStaffTypeId.Text = staff.StaffTypeId != null ? staff.StaffTypeId.ToString() : "";
        //                txtLoginStaffTypeDesc.Text = staff.StaffTypeDesc;
        //                txtLoginNameTH.Text = staff.StaffNameTH;
        //                txtUserLoginChannelId.Text = staff.ChannelId;
        //                txtUserLoginChannelDesc.Text = staff.ChannelDesc;

        //                //Check สิทธิ์ภัทรในการเข้าใช้งาน
        //                ConfigBranchPrivilegeData data = ConfigBranchPrivilegeBiz.GetConfigBranchPrivilege(staff.BranchCode);
        //                if (data != null)
        //                {
        //                    if (data.IsView != null && data.IsView.Value == false)
        //                    {
        //                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", GetSearchPageUrl());
        //                        return;
        //                    }
        //                }
        //                //------------------------------------------------------------------------------------------------------

        //                GetLeadData();

        //                if (lead.ISCOC == "1" && lead.COCCurrentTeam != SLMConstant.COCTeam.Marketing)
        //                {
        //                    btnAllCampaign.Visible = false;
        //                }
        //                //if (ctlExistingLead != null) ctlExistingLead.GetExistingLeadList(txtCitizenId.Text.Trim(), txtTelNo1.Text.Trim(), txtTicketID.Text.Trim());
        //                //if (ctlExistingProduct != null) ctlExistingProduct.GetExistingProductList(txtCitizenId.Text.Trim());
        //                if (ctlOwnerLogging != null) ctlOwnerLogging.GetOwnerLogingList(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
        //                if (ctlActivity != null) ctlActivity.InitialControl(lead, txtLoginNameTH.Text.Trim());
        //                if (ctlRenewInsurAct != null) ctlRenewInsurAct.InitialControlLead(lead, txtLoginNameTH.Text.Trim());
        //                if (ctlNote != null) ctlNote.InitialControl(txtTicketID.Text.Trim());

        //                //if (lead.PreleadId != null && ctlInsurSummary != null)
        //                //    ctlInsurSummary.GetPreleadInsureSummary(lead.PreleadId.Value.ToString());
        //                //if (lead.TicketId != null && ctlInsurSummary != null)
        //                //    ctlInsurSummary.GetLeadInsureSummary(lead.TicketId);

        //                if (!string.IsNullOrEmpty(Request["tab"]) && Request["tab"] == "008")
        //                    tabMain.ActiveTabIndex = 0;

        //                upTabMain.Update();
        //            }

        //            //ไว้หลัง InitialTabAndUserControl(), GetLeadData(), GetPreleadData() เสมอ
        //            _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + (Request["ticketid"] != null ? Request["ticketid"] : ("PreleadId " + Request["preleadid"])) + " Begin Page_Load (GenerateRightSideMenu)");
        //            GenerateRightSideMenu(txtCampaignId.Text.Trim(), txtProductId.Text.Trim());
        //            _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + (Request["ticketid"] != null ? Request["ticketid"] : ("PreleadId " + Request["preleadid"])) + " End Page_Load (GenerateRightSideMenu)");
        //            doGenRightMenuTabControls = true;
        //        }

        //        if (!doGenRightMenuTabControls)
        //        {
        //            _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + (Request["ticketid"] != null ? Request["ticketid"] : ("PreleadId " + Request["preleadid"])) + " Begin Page_Load (GenerateRightMenuTabControl)");
        //            GenerateRightMenuTabControl();
        //            _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + (Request["ticketid"] != null ? Request["ticketid"] : ("PreleadId " + Request["preleadid"])) + " End Page_Load (GenerateRightMenuTabControl)");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Error(message);

        //        if (Request["backtype"] == "2" || Request["backtype"] == "3")
        //        {
        //            AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_029.aspx?backtype=" + Request["backtype"]);      //ค้นหา OBT
        //        }
        //        else
        //        {
        //            AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_003.aspx");        //ค้นหา default
        //        }
        //    }
        //}
        #endregion

        private bool CheckTicketIdPrivilegeRenewInsurance(StaffData staff, string ticketId, string preleadId)
        {
            SearchObtCondition search_cond = null;

            if (!string.IsNullOrWhiteSpace(ticketId))
            {
                search_cond = new SearchObtCondition()
                {
                    TicketId = ticketId,
                    IsInbound = true,
                    IsOutbound = false,
                    StaffTypeId = staff.StaffTypeId != null ? staff.StaffTypeId.ToString() : "",
                    StaffBranchCode = staff.BranchCode,
                    StaffEmpCode = staff.EmpCode,
                    StaffUsername = staff.UserName
                };
            }
            else if (!string.IsNullOrWhiteSpace(preleadId))
            {
                search_cond = new SearchObtCondition()
                {
                    PreleadId = preleadId,
                    IsInbound = false,
                    IsOutbound = true,
                    StaffTypeId = staff.StaffTypeId != null ? staff.StaffTypeId.ToString() : "",
                    StaffBranchCode = staff.BranchCode,
                    StaffEmpCode = staff.EmpCode,
                    StaffUsername = staff.UserName
                };
            }
            else
            {
                return false;
            }

            string logError = "";
            var result = new SlmScr029Biz().SearchTabAllTask(search_cond, out logError);
            return result.Count > 0;
        }

        protected void btnLoadLeadInfo_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Remove(SLMConstant.SessionName.screen_code);

                if (txtTicketID.Text.Trim() != "")
                {
                    var lead = SlmScr004Biz.SearchSCR004Data(txtTicketID.Text.Trim());
                    if (ctlLeadInfo != null)
                        ctlLeadInfo.GetLeadData(lead);
                }
                else if (txtPreleadId.Text.Trim() != "")
                {
                    if (ctlLeadInfo != null)
                        ctlLeadInfo.GetPreleadData(SLMUtil.SafeDecimal(txtPreleadId.Text.Trim()));
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnLoadExistingLead_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtTicketID.Text.Trim() != "")
                {
                    if (ctlExistingLead != null)
                        ctlExistingLead.GetExistingLeadList(txtCitizenId.Text.Trim(), txtTelNo1.Text.Trim(), txtTicketID.Text.Trim());

                    //if (ctlLeadInfo != null)
                    //{
                    //    var lead = SlmScr004Biz.SearchSCR004Data(Request["ticketid"]);
                    //    ctlLeadInfo.GetLeadData(lead);
                    //}
                }
                else if (txtPreleadId.Text.Trim() != "")
                {
                    if (ctlExistingLead != null)
                        ctlExistingLead.GetExistingLeadList(txtCitizenId.Text.Trim(), "", "");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void LoadExistingProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtTicketID.Text.Trim() != "")
                {
                    if (ctlExistingProduct != null)
                        ctlExistingProduct.GetCBSExistingProductList(txtCitizenId.Text.Trim(),txtCardTypeId.Text,txtCountryId.Text);

                    //if (ctlLeadInfo != null)
                    //{
                    //    var lead = SlmScr004Biz.SearchSCR004Data(Request["ticketid"]);
                    //    ctlLeadInfo.GetLeadData(lead);
                    //}
                }
                else if (txtCitizenId.Text.Trim() != "")
                {
                    if (ctlExistingProduct != null)
                        ctlExistingProduct.GetCBSExistingProductList(txtCitizenId.Text.Trim(),txtCardTypeId.Text,txtCountryId.Text);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnLoadOwnerLogging_Click(object sender, EventArgs e)
        {
            try
            {
                if (ctlOwnerLogging != null)
                {
                    ctlOwnerLogging.GetOwnerLogingList(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
                }

                //if (txtTicketID.Text.Trim() != "")
                //{
                //    if (ctlLeadInfo != null)
                //    {
                //        var lead = SlmScr004Biz.SearchSCR004Data(Request["ticketid"]);
                //        ctlLeadInfo.GetLeadData(lead);
                //    }
                //}
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnLoadNote_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtTicketID.Text.Trim() != "")
                {
                    if (ctlNote != null)
                    {
                        ctlNote.InitialControl(txtTicketID.Text.Trim());
                    }

                    //if (ctlLeadInfo != null)
                    //{
                    //    var lead = SlmScr004Biz.SearchSCR004Data(Request["ticketid"]);
                    //    ctlLeadInfo.GetLeadData(lead);
                    //}
                }
                else if (txtPreleadId.Text.Trim() != "")
                {
                    if (ctlNote != null)
                    {
                        ctlNote.InitialControlPrelead(txtPreleadId.Text.Trim());
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

        protected void btnLoadInsureSummary_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtTicketID.Text.Trim() != "")
                {
                    if (ctlInsurSummary != null)
                    {
                        ctlInsurSummary.GetLeadInsureSummary(txtTicketID.Text.Trim());
                    }

                    //if (ctlLeadInfo != null)
                    //{
                    //    var lead = SlmScr004Biz.SearchSCR004Data(Request["ticketid"]);
                    //    ctlLeadInfo.GetLeadData(lead);
                    //}
                }
                else if (txtPreleadId.Text.Trim() != "")
                {
                    if (ctlInsurSummary != null)
                    {
                        ctlInsurSummary.GetPreleadInsureSummary(txtPreleadId.Text.Trim());
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

        private string GetSearchPageUrl()
        {
            if (Request["backtype"] == "2" || Request["backtype"] == "3")
            {
                return "SLM_SCR_029.aspx?backtype=" + Request["backtype"];      //ค้นหา OBT
            }
            else
            {
                return "SLM_SCR_003.aspx";          //ค้นหา default
            }
        }

        private void GenerateTab(string campaignId, string productId)
        {
            try
            {
                List<ConfigProductTabData> tabScreenList = new List<ConfigProductTabData>();

                if (Session[SLMConstant.SessionName.tabscreenlist] == null)
                {
                    tabScreenList = new ConfigProductTabBiz().GetTabScreenList(productId, campaignId);
                    Session[SLMConstant.SessionName.tabscreenlist] = tabScreenList;
                }
                else
                {
                    tabScreenList = (List<ConfigProductTabData>)Session[SLMConstant.SessionName.tabscreenlist];
                }

                if (tabScreenList.Count == 0)
                {
                    LoadDefaultTabs();
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request["ticketid"]))         //Lead
                    {
                        ConfigProductScreenData data = null;
                        if (Session[SLMConstant.SessionName.configscreen] == null)
                        {
                            data = new ConfigProductScreenBiz().GetData(campaignId, productId, SLMConstant.ConfigProductScreen.ActionType.View);
                            Session[SLMConstant.SessionName.configscreen] = data;
                        }
                        else
                            data = (ConfigProductScreenData)Session[SLMConstant.SessionName.configscreen];

                        if (data != null)
                        {
                            var biz = new LeadInfoBiz();
                            //if (data.TableName == "kkslm_tr_renewinsurance" && biz.HasRenewInsurance(Request["ticketid"]))
                            if (data.TableName == "kkslm_tr_renewinsurance")
                            {
                                LoadConfigTabs(tabScreenList);
                            }
                            else
                            {
                                LoadDefaultTabs();
                            }
                        }
                        else
                        {
                            LoadDefaultTabs();
                        }
                    }
                    else
                    {
                        LoadConfigTabs(tabScreenList);      //prelead
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void LoadDefaultTabs()
        {
            try
            {
                //Generate Default Control
                //1.Activity
                ctlActivity = (SLM.Application.Shared.Tabs.Tab008)LoadControl("~/Shared/Tabs/Tab008.ascx");
                //ctlActivity.UpdatedDataChanged += UpdateStatusDesc;
                ctlActivity.UpdatedMainDataChanged += UpdateMainData;
                ctlActivity.GetMainData += GetMainData;
                AjaxControlToolkit.TabPanel tabPanel = new AjaxControlToolkit.TabPanel();
                tabPanel.HeaderTemplate = new TabHeaderTemplate("Activity", SLMConstant.TabCode.Activity);
                tabPanel.Controls.Add(ctlActivity);
                tabMain.Tabs.Add(tabPanel);

                //2.ข้อมูล Lead
                ctlLeadInfo = (SLM.Application.Shared.Tabs.Tab004)LoadControl("~/Shared/Tabs/Tab004.ascx");
                tabPanel = new AjaxControlToolkit.TabPanel();
                tabPanel.HeaderTemplate = new TabHeaderTemplate("ข้อมูล Lead", SLMConstant.TabCode.LeadInfo);
                tabPanel.Controls.Add(ctlLeadInfo);
                tabMain.Tabs.Add(tabPanel);

                //3.Existing Lead
                ctlExistingLead = (SLM.Application.Shared.Tabs.Tab005)LoadControl("~/Shared/Tabs/Tab005.ascx");
                tabPanel = new AjaxControlToolkit.TabPanel();
                tabPanel.HeaderTemplate = new TabHeaderTemplate("Existing Lead", SLMConstant.TabCode.ExistingLead);
                tabPanel.ID = "003";
                tabPanel.Controls.Add(ctlExistingLead);
                tabMain.Tabs.Add(tabPanel);

                //4.Existing Product
                ctlExistingProduct = (SLM.Application.Shared.Tabs.Tab006)LoadControl("~/Shared/Tabs/Tab006.ascx");
                tabPanel = new AjaxControlToolkit.TabPanel();
                tabPanel.HeaderTemplate = new TabHeaderTemplate("Existing Product", SLMConstant.TabCode.ExistingProduct);
                tabPanel.ID = "004";
                tabPanel.Controls.Add(ctlExistingProduct);
                tabMain.Tabs.Add(tabPanel);

                //5.Owner Logging
                ctlOwnerLogging = (SLM.Application.Shared.Tabs.Tab007)LoadControl("~/Shared/Tabs/Tab007.ascx");
                tabPanel = new AjaxControlToolkit.TabPanel();
                tabPanel.HeaderTemplate = new TabHeaderTemplate("Owner Logging", SLMConstant.TabCode.OwnerLogging);
                tabPanel.Controls.Add(ctlOwnerLogging);
                tabMain.Tabs.Add(tabPanel);

                //6.Note
                ctlNote = (SLM.Application.Shared.Tabs.Tab009)LoadControl("~/Shared/Tabs/Tab009.ascx");
                tabPanel = new AjaxControlToolkit.TabPanel();
                tabPanel.HeaderTemplate = new TabHeaderTemplate("Note History", SLMConstant.TabCode.Note);
                tabPanel.Controls.Add(ctlNote);
                tabMain.Tabs.Add(tabPanel);
            }
            catch
            {
                throw;
            }
        }

        private void LoadConfigTabs(List<ConfigProductTabData> tabScreenList)
        {
            try
            {
                AjaxControlToolkit.TabPanel tabPanel = new AjaxControlToolkit.TabPanel();

                foreach (ConfigProductTabData data in tabScreenList)
                {
                    if (data.TabCode == SLMConstant.TabCode.Activity)
                    {
                        tabPanel = new AjaxControlToolkit.TabPanel();
                        tabPanel.HeaderTemplate = new TabHeaderTemplate(data.TabHeader, data.TabCode);

                        if (!string.IsNullOrEmpty(data.ScreenPath))
                        {
                            ctlRenewInsurAct = (SLM.Application.Shared.Obt.TabActRenewInsure)LoadControl("~" + data.ScreenPath);
                            ctlRenewInsurAct.UpdatedDataChanged += UpdateStatusDesc;
                            ctlRenewInsurAct.GetOBTMainData += GetOBTMainData;
                            ctlRenewInsurAct.UpdatedMainDataChanged += UpdateMainData;
                            ctlRenewInsurAct.SavedDataChanged += SavedDataChange;
                            ctlRenewInsurAct.GridviewPremiumDataChanged += UpdateGridviewPremium;
                            //ctlRenewInsurAct.UpdateLeadDataTab += btnLoadLeadInfo_Click;
                            tabPanel.Controls.Add(ctlRenewInsurAct);
                        }
                        tabMain.Tabs.Add(tabPanel);
                    }
                    else if (data.TabCode == SLMConstant.TabCode.LeadInfo)
                    {
                        tabPanel = new AjaxControlToolkit.TabPanel();
                        tabPanel.HeaderTemplate = new TabHeaderTemplate(data.TabHeader, data.TabCode);

                        if (!string.IsNullOrEmpty(data.ScreenPath))
                        {
                            ctlLeadInfo = (SLM.Application.Shared.Tabs.Tab004)LoadControl("~" + data.ScreenPath);
                            tabPanel.Controls.Add(ctlLeadInfo);
                        }
                        tabMain.Tabs.Add(tabPanel);
                    }
                    else if (data.TabCode == SLMConstant.TabCode.ExistingLead)
                    {
                        tabPanel = new AjaxControlToolkit.TabPanel();
                        tabPanel.HeaderTemplate = new TabHeaderTemplate(data.TabHeader, data.TabCode);

                        if (!string.IsNullOrEmpty(data.ScreenPath))
                        {
                            ctlExistingLead = (SLM.Application.Shared.Tabs.Tab005)LoadControl("~" + data.ScreenPath);
                            tabPanel.Controls.Add(ctlExistingLead);
                        }
                        tabMain.Tabs.Add(tabPanel);
                    }
                    else if (data.TabCode == SLMConstant.TabCode.ExistingProduct)
                    {
                        tabPanel = new AjaxControlToolkit.TabPanel();
                        tabPanel.HeaderTemplate = new TabHeaderTemplate(data.TabHeader, data.TabCode);

                        if (!string.IsNullOrEmpty(data.ScreenPath))
                        {
                            ctlExistingProduct = (SLM.Application.Shared.Tabs.Tab006)LoadControl("~" + data.ScreenPath);
                            tabPanel.Controls.Add(ctlExistingProduct);
                        }
                        tabMain.Tabs.Add(tabPanel);
                    }
                    else if (data.TabCode == SLMConstant.TabCode.OwnerLogging)
                    {
                        tabPanel = new AjaxControlToolkit.TabPanel();
                        tabPanel.HeaderTemplate = new TabHeaderTemplate(data.TabHeader, data.TabCode);

                        if (!string.IsNullOrEmpty(data.ScreenPath))
                        {
                            ctlOwnerLogging = (SLM.Application.Shared.Tabs.Tab007)LoadControl("~" + data.ScreenPath);
                            tabPanel.Controls.Add(ctlOwnerLogging);
                        }
                        tabMain.Tabs.Add(tabPanel);
                    }
                    else if (data.TabCode == SLMConstant.TabCode.Note)
                    {
                        tabPanel = new AjaxControlToolkit.TabPanel();
                        tabPanel.HeaderTemplate = new TabHeaderTemplate(data.TabHeader, data.TabCode);

                        if (!string.IsNullOrEmpty(data.ScreenPath))
                        {
                            ctlNote = (SLM.Application.Shared.Tabs.Tab009)LoadControl("~" + data.ScreenPath);
                            tabPanel.Controls.Add(ctlNote);
                        }
                        tabMain.Tabs.Add(tabPanel);
                    }
                    else if (data.TabCode == SLMConstant.TabCode.InsuranceSummary)
                    {
                        tabPanel = new AjaxControlToolkit.TabPanel();
                        tabPanel.HeaderTemplate = new TabHeaderTemplate(data.TabHeader, data.TabCode);

                        if (!string.IsNullOrEmpty(data.ScreenPath))
                        {
                            ctlInsurSummary = (SLM.Application.Shared.Obt.TabInsureSummary)LoadControl("~" + data.ScreenPath);
                            tabPanel.Controls.Add(ctlInsurSummary);
                        }
                        tabMain.Tabs.Add(tabPanel);
                    }
                    else
                    {
                        tabPanel = new AjaxControlToolkit.TabPanel();
                        tabPanel.HeaderTemplate = new TabHeaderTemplate(data.TabHeader, data.TabCode);
                        tabMain.Tabs.Add(tabPanel);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void GenerateRightSideMenu(string campaignId, string productId, StaffData staff)
        {
            try
            {
                var list = new ConfigProductSideMenuBiz().GetSideMenuList(productId, campaignId);
                foreach (ConfigProductTabSideMenuData data in list)
                {
                    if (data.MenuCode == SLMConstant.RightSideMenu.TabControl)
                    {
                        GenerateRightMenuTabControl();  //Generate ปุ่ม control ให้เหมือนกับ Tabs
                        trMenuTab.Visible = true;
                        imbMenuTab.ImageUrl = "~" + data.ImagePath;
                        imbMenuTab.ToolTip = data.MenuDescription;
                    }
                    else if (data.MenuCode == SLMConstant.RightSideMenu.ButtonControl)
                    {
                        trMenuButton.Visible = true;
                        imbMenuButton.ImageUrl = "~" + data.ImagePath;
                        imbMenuButton.ToolTip = data.MenuDescription;

                        //เช็กสิทธิการมองเห็นปุ่มใน Method GetLeadData(), GetPreleadData()
                        //ปุ่ม Calculator
                        trButtonCalculator.Visible = imbCal.Visible;
                        lbCalculator.OnClientClick = imbCal.OnClientClick + " HideDivMenuButton();";

                        //ปุ่ม แนบเอกสาร ADAMS
                        trButtonDocument.Visible = imbDoc.Visible;
                        lbDocument.OnClientClick = imbDoc.OnClientClick + " HideDivMenuButton();";

                        //ปุ่ม ข้อมูลเพิ่มเติม
                        trButtonOthers.Visible = imbOthers.Visible;
                        lbOthers.OnClientClick = imbOthers.OnClientClick + " HideDivMenuButton();";

                        //ปุ่ม ใบ 50ทวิ, ตัดบัตรเครดิต, แก้ไขใบเสร็จ 
                        if (string.IsNullOrEmpty(Request["ticketid"])) //ฟอร์มต่างๆ พิมพ์ได้เฉพาะตอนเป็น Lead แล้วเท่านั้น
                        {
                            tr50Tawi.Visible = false;
                            trCreditForm.Visible = false;
                            trEditReceipt.Visible = false;
                        }

                        //ปุ่ม แบบสอบถาม
                        string url = new ConfigProductQuestionairBiz().GetUrl(txtProductId.Text.Trim());
                        if (!string.IsNullOrEmpty(url))
                            lbQuestionaire.OnClientClick = "window.open('" + url + "', 'questionair', 'status=yes, toolbar=no, scrollbars=yes, menubar=no, width=800, height=600, resizable=yes'); HideDivMenuButton(); return false;";
                        else
                            lbQuestionaire.OnClientClick = "HideDivMenuButton(); return false;";

                        trButtonCopyLead.Visible = imbCopyLead.Visible;
                    }
                    else if (data.MenuCode == SLMConstant.RightSideMenu.Gift)
                    {
                        if (!string.IsNullOrEmpty(Request["ticketid"])) //ให้ของ premium ได้เฉพาะต้องเป็น Lead แล้วเท่านั้น
                        {
                            ConfigProductPremiumBiz biz = new ConfigProductPremiumBiz();
                            trMenuPremium.Visible = true;
                            imbMenuPremium.ImageUrl = "~" + data.ImagePath;
                            imbMenuPremium.ToolTip = data.MenuDescription;

                            gvPremium.DataSource = biz.GetPremiumByTicketId(Request["ticketid"], txtProductId.Text.Trim(), txtCampaignId.Text.Trim());
                            gvPremium.DataBind();
                        }
                    }
                    else if (data.MenuCode == SLMConstant.RightSideMenu.CusInfo)
                    {
                        trMenuInfo.Visible = true;
                        imbMenuInfo.ImageUrl = "~" + data.ImagePath;
                        imbMenuInfo.ToolTip = data.MenuDescription;

                        //StaffBiz staffbiz = new StaffBiz();
                        //var staff = staffbiz.GetStaffDisplayInfo(HttpContext.Current.User.Identity.Name);
                        //lblInfoStaffLicenseNo.Text = string.IsNullOrEmpty(staff.LicenseNo) ? "-" : staff.LicenseNo;
                        lblInfoStaffName.Text = string.IsNullOrEmpty(staff.StaffNameTH_NoPosition) ? "-" : staff.StaffNameTH_NoPosition;
                        lblInfoStaffPositionName.Text = string.IsNullOrEmpty(staff.PositionName) ? "-" : staff.PositionName;
                        lblInfoStaffTeam.Text = string.IsNullOrEmpty(staff.TeamTelesalesCode) ? "-" : staff.TeamTelesalesCode;
                        //lblInfoStaffLicenseExpireDate.Text = staff.LicenseExpireDate != null ? (staff.LicenseExpireDate.Value.ToString("dd/MM/") + staff.LicenseExpireDate.Value.Year.ToString()) : "-";

                        StaffBiz staffbiz = new StaffBiz();
                        var licenseList = staffbiz.GetLicenseInfo(staff.EmpCode);
                        rptUserLicense.DataSource = licenseList;
                        rptUserLicense.DataBind();
                        trLicenseNotFound.Visible = licenseList.Count == 0;

                        CustomerData cusInfo = null;
                        if (!string.IsNullOrEmpty(Request["ticketid"]))
                            cusInfo = new LeadInfoBiz().GetCustomerData(Request["ticketid"]);
                        else if (!string.IsNullOrEmpty(Request["preleadid"]))
                            cusInfo = new PreleadBiz().GetCustomerData(Request["preleadid"]);

                        if (cusInfo != null)
                        {
                            lblInfoContractNo.Text = string.IsNullOrEmpty(cusInfo.ContractNo) ? "-" : cusInfo.ContractNo;
                            lblInfoCustomerName.Text = string.IsNullOrEmpty(cusInfo.CustomerName) ? "-" : cusInfo.CustomerName;
                            lblInfoTicketId.Text = string.IsNullOrEmpty(cusInfo.TicketId) ? "-" : cusInfo.TicketId;
                            lblInfoCampaignName.Text = string.IsNullOrEmpty(cusInfo.CampaignName) ? "-" : cusInfo.CampaignName;
                            lblInfoCarBrandName.Text = string.IsNullOrEmpty(cusInfo.CarBrandName) ? "-" : cusInfo.CarBrandName;
                            lblInfoCarModelName.Text = string.IsNullOrEmpty(cusInfo.CarModelName) ? "-" : cusInfo.CarModelName;
                            lblInfoCarLicenseNo.Text = string.IsNullOrEmpty(cusInfo.CarLicenseNo) ? "-" : cusInfo.CarLicenseNo;
                        }
                    }
                    else if (data.MenuCode == SLMConstant.RightSideMenu.SaleScript)
                    {
                        LoadSaleScript(txtCampaignId.Text.Trim(), txtProductId.Text.Trim());
                        trMenuSaleScript.Visible = true;
                        imbMenuSaleScript.ImageUrl = "~" + data.ImagePath;
                        imbMenuSaleScript.ToolTip = data.MenuDescription;
                    }
                    else
                    {
                        //Add new menu
                        TableRow tr = new TableRow();
                        TableCell tc = new TableCell();
                        ImageButton imbMenu = new ImageButton();
                        imbMenu.Width = new Unit(36, UnitType.Pixel);
                        imbMenu.ImageUrl = string.IsNullOrEmpty(data.ImagePath) ? "~/Images/newmenu_sidemenu_icon.png" : data.ImagePath;
                        imbMenu.ToolTip = data.MenuDescription;
                        imbMenu.OnClientClick = "return false;";

                        tc.Controls.Add(imbMenu);
                        tr.Cells.Add(tc);
                        tabRightSideMenu.Rows.Add(tr);
                    }
                }


                //                string script = @"$(document).ready(function () {
                //                                            $('#" + imbMenu.ClientID + @"').click(function () {
                //                                                alert('test');
                //                                                return false;
                //                                            });
                //                                        });";

                //                ScriptManager.RegisterStartupScript(Page, GetType(), "load", script, true);
            }
            catch
            {
                throw;
            }
        }

        private void GenerateRightMenuTabControl()
        {
            try
            {
                int tabIndex = 0;
                int menuPerRow = 1;
                int objPerRow = 0;

                TableRow tr = new TableRow();
                tableTabMenu.Rows.Add(tr);

                foreach (AjaxControlToolkit.TabPanel tabPanel in tabMain.Tabs)
                {
                    objPerRow += 1;
                    TabHeaderTemplate tabHeader = (TabHeaderTemplate)tabPanel.HeaderTemplate;

                    //Icon section
                    Image img = new Image();
                    img.Width = new Unit(25, UnitType.Pixel);

                    if (tabHeader.TabCode == SLMConstant.TabCode.Activity)
                        img.ImageUrl = "~/Images/tel_tab_icon.png";
                    else if (tabHeader.TabCode == SLMConstant.TabCode.LeadInfo)
                        img.ImageUrl = "~/Images/info_tab_icon.png";
                    else if (tabHeader.TabCode == SLMConstant.TabCode.ExistingLead)
                        img.ImageUrl = "~/Images/existinglead_tab_icon.png";
                    else if (tabHeader.TabCode == SLMConstant.TabCode.ExistingProduct)
                        img.ImageUrl = "~/Images/existingproduct_tab_icon.png";
                    else if (tabHeader.TabCode == SLMConstant.TabCode.OwnerLogging)
                        img.ImageUrl = "~/Images/ownerlogging_tab_icon.png";
                    else if (tabHeader.TabCode == SLMConstant.TabCode.Note)
                        img.ImageUrl = "~/Images/note_tab_icon.png";
                    else if (tabHeader.TabCode == SLMConstant.TabCode.InsuranceSummary)
                        img.ImageUrl = "~/Images/insurance_sum_tab_icon.png";
                    else
                        img.ImageUrl = "~/Images/newtab_tab_icon.png";

                    TableCell tcIcon = new TableCell();
                    tcIcon.HorizontalAlign = HorizontalAlign.Center;
                    tcIcon.CssClass = "TableCellIcon";
                    tcIcon.Style["cursor"] = "pointer";
                    tcIcon.Attributes.Add("onclick", "TabClick('" + tabHeader.TabCode + "'); SetActiveTab('" + tabMain.ClientID + "', '" + tabIndex + "'); return false;");
                    tcIcon.Controls.Add(img);
                    tr.Cells.Add(tcIcon);

                    //Text section
                    Label lb = new Label();
                    lb.Text = tabHeader.HeaderText;

                    TableCell tcText = new TableCell();
                    tcText.HorizontalAlign = HorizontalAlign.Left;
                    tcText.CssClass = "TableCellText";
                    tcText.Style["cursor"] = "pointer";
                    tcText.Attributes.Add("onclick", "TabClick('" + tabHeader.TabCode + "'); SetActiveTab('" + tabMain.ClientID + "', '" + tabIndex + "'); return false;");
                    tcText.Controls.Add(lb);

                    tr.Attributes.Add("onmouseover", "this.style.background='#dceaf8'");
                    tr.Attributes.Add("onmouseout", "this.style.background='#ffffff'");
                    tr.Cells.Add(tcText);

                    if (objPerRow % menuPerRow == 0)
                    {
                        tr = new TableRow();
                        tableTabMenu.Rows.Add(tr);
                    }

                    tabIndex += 1;
                }
            }
            catch
            {
                throw;
            }
        }

        private void LoadSaleScript(string campaignId, string productId)
        {
            try
            {
                var scriptList = new ConfigScriptBiz().GetConfigScriptList(campaignId, productId);
                rptSaleScript.DataSource = scriptList.Where(p => p.DataType == "001").ToList();
                rptSaleScript.DataBind();
                rptQandA.DataSource = scriptList.Where(p => p.DataType == "002").ToList();
                rptQandA.DataBind();
            }
            catch
            {
                throw;
            }
        }

        protected void lbCopyLead_Click(object sender, EventArgs e)
        {
            RedirectToCreateLead();
        }

        protected void lbPO_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectEcm ecm = new ConnectEcm();
                ecm.VerifyConnection();


                if (ctlRenewInsurAct != null)
                {
                    int numOfTab = ctlRenewInsurAct.GetNumOfTabs();
                    List<string> printList = ctlRenewInsurAct.getPrintValue();

                    if (string.IsNullOrEmpty(Request["ticketid"]) && !string.IsNullOrEmpty(Request["preleadid"]))   //prelead mode
                    {
                        string ecode = "";
                        string preleadId = ctlRenewInsurAct.GetPreleadIdActiveTab();    //Get Main PreleadId

                        PreleadBiz preleadBiz = new PreleadBiz();
                        //preleadBiz.CheckRenewPurchased(preleadId);    //Comment by Pom 13/07/2016

                        string preleadIdList = "";
                        if (numOfTab <= 1 || printList.Count == 1)
                        {
                            // show common
                            ecode = "001";
                            if (printList.Count > 0)
                                preleadIdList = printList[0];
                            else
                                preleadIdList = preleadId;

                        }
                        else if (printList.Count > 1)
                        {
                            // show fleet
                            ecode = "002";
                            preleadIdList = String.Join(",", printList);
                        }
                        else
                        {
                            // error
                            throw new Exception("กรุณาระบุใบเสนอราคาที่ต้องการพิมพ์");
                        }

                        CreateExcel(ecode, preleadIdList, "P", preleadId, "");

                        #region OldCode
                        //bool isFleet = preleadBiz.IsFleet(preleadId, "");
                        //if (isFleet)
                        //{
                        //    ecode = "002";
                        //    if (numOfTab > 1)
                        //    {
                        //        if (printList.Count == 0)
                        //            throw new Exception("กรุณาระบุใบเสนอราคาที่ต้องการพิมพ์");

                        //        foreach (string temp in printList)
                        //        {
                        //            preleadIdList += (preleadIdList != "" ? "," : "") + temp;
                        //        }
                        //    }
                        //    else
                        //        preleadIdList = preleadId;

                        //    CreateExcel(ecode, preleadIdList, "P", preleadId, "");
                        //    //script = "window.open('SLM_SCR_045.aspx?ecode=" + ecode + "&id=" + preleadIdList + "&type=P&mainid=" + preleadId + "', 'purchaseorder', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                        //}
                        //else
                        //{
                        //    ecode = "001";
                        //    CreateExcel(ecode, preleadId, "P", preleadId, "");
                        //    //script = "window.open('SLM_SCR_045.aspx?ecode=" + ecode + "&id=" + preleadId + "&type=P&mainid=" + preleadId + "', 'purchaseorder', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                        //}
                        #endregion
                    }
                    else if (!string.IsNullOrEmpty(Request["ticketid"]) && string.IsNullOrEmpty(Request["preleadid"]))  //lead mode
                    {
                        string ecode = "";
                        string ticketId = ctlRenewInsurAct.GetTicketIdActiveTab();      //Get Main TicketId
                        string ticketIdList = "";

                        //Comment by Pom 13/07/2016
                        //SlmScr004Biz biz = new SlmScr004Biz();
                        //biz.CheckRenewPurchased(ticketId);

                        if (numOfTab <= 1 || printList.Count == 1)
                        {
                            ecode = "001";
                            if (printList.Count > 0)
                                ticketIdList = printList[0];
                            else
                                ticketIdList = ticketId;
                        }
                        else if (printList.Count > 1)
                        {
                            ecode = "002";
                            ticketIdList = String.Join(",", printList);
                        }
                        else
                        {
                            throw new Exception("กรุณาระบุใบเสนอราคาที่ต้องการพิมพ์");
                        }

                        CreateExcel(ecode, ticketIdList, "T", ticketId, "");

                        #region OldCode
                        //bool isFleet = new PreleadBiz().IsFleet("", ticketId);
                        //if (isFleet)
                        //{
                        //    ecode = "002";
                        //    if (numOfTab > 1)
                        //    {
                        //        if (printList.Count == 0)
                        //            throw new Exception("กรุณาระบุใบเสนอราคาที่ต้องการพิมพ์");

                        //        foreach (string temp in printList)
                        //        {
                        //            ticketIdList += (ticketIdList != "" ? "," : "") + temp;
                        //        }
                        //    }
                        //    else
                        //        ticketIdList = ticketId;

                        //    CreateExcel(ecode, ticketIdList, "T", ticketId, "");
                        //    //script = "window.open('SLM_SCR_045.aspx?ecode=" + ecode + "&id=" + ticketIdList + "&type=T&mainid=" + ticketId + "', 'purchaseorder', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                        //}
                        //else
                        //{
                        //    ecode = "001";
                        //    CreateExcel(ecode, ticketId, "T", ticketId, "");
                        //    //script = "window.open('SLM_SCR_045.aspx?ecode=" + ecode + "&id=" + ticketId + "&type=T&mainid=" + ticketId + "', 'purchaseorder', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                        //}
                        #endregion
                    }
                    else
                    {
                        return;
                    }

                    ctlRenewInsurAct.DoBindGridview(0);     //Refresh Gridview บันทึกผลการติดต่อ
                    string key = "phorder_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string script = "window.open('SLM_SCR_045.aspx', '" + key + "', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                    ScriptManager.RegisterStartupScript(Page, GetType(), key, script, true);
                }
            }
            catch (Exception ex)
            {
                Session.Remove(excelfilepath);
                Session.Remove(outputfilename);

                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void CreateExcel(string excelcode, string refid, string reftype, string mainId, string recno)
        {
            //reftype T = Ticket , P = Prelead
            //refid can be TicketId or PreleadId

            try
            {
                if (String.IsNullOrEmpty(refid))
                {
                    throw new Exception("Invalid ID");
                }
                string cfolder = Server.MapPath("~/Upload");
                // check exists
                if (!Directory.Exists(cfolder)) Directory.CreateDirectory(cfolder);

                string templatename = "";
                string sheetname = "";
                string outputname = "";
                string copylocation = "";
                string fileName = "";

                List<ExcelTemplateBiz.ExcelData> lst = new List<ExcelTemplateBiz.ExcelData>();
                ExcelTemplateBiz ebiz = new ExcelTemplateBiz();
                switch (excelcode)
                {
                    case "001": // ฟอร์มใบเสนอราคาเปรียบเทียบ
                        templatename = "Template-ใบเสนอราคาเปรียบเทียบ.xls";
                        sheetname = "ใบเสนอราคา";
                        outputname = "ใบเสนอราคาเปรียบเทียบ.xls";

                        fileName = String.Format("ใบเสนอราคา_{0}{1}.xls", DateTime.Now.Year.ToString("0000") + DateTime.Now.ToString("MMddHHmmss"), new Random().Next(0, 999).ToString("000"));
                        copylocation = cfolder + "\\" + fileName;

                        if (reftype == "T") lst = ebiz.GetExcelData001(refid, HttpContext.Current.User.Identity.Name);
                        else if (reftype == "P") lst = ebiz.GetExcelData001P(SLMUtil.SafeDecimal(refid), HttpContext.Current.User.Identity.Name);
                        else throw new Exception("Invalid Type");

                        if (lst.Count == 0) throw new Exception("No Data");
                        break;

                    case "002": // ฟอร์มใบเสอราคาแบบรถ Fleet
                        if (String.IsNullOrEmpty(reftype)) throw new Exception("Specify type for fleet report, T = TicketID, P = PreleadID");
                        templatename = "Template_ใบเสนอราคา_FLEET.xls";
                        sheetname = "Sheet1";
                        outputname = "ใบเสนอราคา_FLEET.xls";

                        fileName = String.Format("ใบเสนอราคาFleet_{0}{1}.xls", DateTime.Now.Year.ToString("0000") + DateTime.Now.ToString("MMddHHmmss"), new Random().Next(0, 999).ToString("000"));
                        copylocation = cfolder + "\\" + fileName;

                        var allid = refid.Split(',');

                        if (reftype == "T") lst = ebiz.GetExcelData002T(allid, HttpContext.Current.User.Identity.Name);
                        else if (reftype == "P") { var iddec = new List<decimal>(); foreach (var str in allid) iddec.Add(SLMUtil.SafeDecimal(str)); lst = ebiz.GetExcelData002P(iddec.ToArray(), HttpContext.Current.User.Identity.Name); }
                        else throw new Exception("Invalid type");

                        if (lst.Count == 0) throw new Exception("No Data");
                        break;

                    case "003": // ฟอร์มตัดบัตรเครดิต
                        templatename = "Template_ฟอร์มบัตรเครดิต.xls";
                        sheetname = "Sheet1";
                        outputname = "ฟอร์มบัตรเครดิต.xls";
                        lst = ebiz.GetExcelData003(refid);
                        break;

                    case "004": // ฟอร์ม 50 ทวิ
                        templatename = "Template-ทวิ50-หักภาษี_ณ_ที่จ่าย1%(นิติบุคคล).xls";
                        sheetname = "บริษัท";
                        outputname = "ทวิ50-หักภาษี_ณ_ที่จ่าย1%(นิติบุคคล).xls";
                        lst = ebiz.GetExcelData004(refid);
                        break;

                    case "005": // ฟอร์มแก้ไขใบเสร็จ
                        if (String.IsNullOrEmpty(recno)) throw new Exception("Invalid Rec No");
                        templatename = "Template_แบบฟอร์มแก้ไขใบเสร็จ.xls";
                        sheetname = "ชำระผิด";
                        outputname = "แบบฟอร์มแก้ไขใบเสร็จ.xls";
                        lst = ebiz.GetExcelData005(refid, recno);
                        break;

                    default:
                        throw new Exception("Invalid Excel Template Code");

                }

                string excelPath = GetExcel(templatename, sheetname, outputname, lst, copylocation, fileName, excelcode, refid, reftype, mainId);

                //เก็บใส่ Session เพื่อไว้ใช้ในหน้า Export SLM_SCR_045.aspx
                Session[excelfilepath] = excelPath;
                Session[outputfilename] = outputname;
            }
            catch
            {
                throw;
            }
        }

        private string GetExcel(string templatefilename, string sheetname, string outputfilename, List<ExcelTemplateBiz.ExcelData> datalst, string copytolocation, string fileName, string excelcode, string refid, string reftype, string mainId)
        {
            try
            {
                string templatepath = Server.MapPath("~/ExcelTemplate");

                // 1 copy excel to temp
                string tmpfile = Path.GetTempFileName();
                if (File.Exists(tmpfile)) File.Delete(tmpfile);
                File.Copy(templatepath + "\\" + templatefilename, tmpfile);

                // 2 update excel
                using (OleDbConnection oconn = new OleDbConnection(String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=NO\"", tmpfile)))
                {
                    try
                    {
                        oconn.Open();
                        OleDbCommand ocmd = new OleDbCommand();
                        ocmd.Connection = oconn;
                        ocmd.CommandType = System.Data.CommandType.Text;
                        foreach (var data in datalst)
                        {
                            string sqlz = String.Format("UPDATE [{0}${1}:{1}] SET F1 = {2}", sheetname, data.ColumnName, String.IsNullOrEmpty(data.Value) ? "null" : "'" + data.Value + "'");
                            ocmd.CommandText = sqlz;
                            ocmd.ExecuteNonQuery();
                        }
                        oconn.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                // if copy location selected
                if (copytolocation.Trim() != "")
                {
                    try
                    {
                        if (File.Exists(copytolocation)) File.Delete(copytolocation);
                        File.Copy(tmpfile, copytolocation);
                        FileInfo info = new FileInfo(copytolocation);

                        bool ret = SaveFileToEcm(copytolocation, fileName, Convert.ToInt32(info.Length), excelcode, refid, reftype, mainId);
                        if (!ret)
                        {
                            throw new Exception("ไม่สามารถบันทึกไฟล์บนระบบ ECM ได้");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return tmpfile;
            }
            catch
            {
                throw;
            }
        }

        private bool SaveFileToEcm(string fileLocation, string fileName, int fileSize, string excelcode, string refid, string reftype, string mainId)
        {
            bool ret = false;
            try
            {
                //fileLocation ex. D:\Projects\Motif\KK\KK_SLM\SaleLeadManagement\SLM.Application\Upload\ใบเสนอราคา_20160613165258896.xls
                //fileName ex. ใบเสนอราคา_20160613165258896.xls

                ConnectEcm ecm = new ConnectEcm();
                ret = ecm.AddAttachment(fileLocation, fileName, mainId, "fileType", HttpContext.Current.User.Identity.Name);

                if (ret)
                {
                    string filePath = SLMConstant.Ecm.SitePath + mainId.Trim() + "/";

                    if (reftype == "T")         //Lead
                    {
                        if (excelcode == "001")
                        {
                            int phonecallId = SlmScr008Biz.InsertPhonecallEcm(refid, filePath, fileName, fileSize, HttpContext.Current.User.Identity.Name);
                            bool result = CreateCASActivityLog(refid, "", excelcode, false, phonecallId.ToString(), filePath, fileName, mainId);
                            SlmScr008Biz.UpdateCasFlag(phonecallId, result ? "1" : "2");
                        }
                        else if (excelcode == "002")
                        {
                            string[] ticketIdList = refid.Split(',');
                            var idList = SlmScr008Biz.InsertPhonecallEcm(ticketIdList, filePath, fileName, fileSize, HttpContext.Current.User.Identity.Name, mainId);

                            foreach (string ticketId in ticketIdList)
                            {
                                int phonecallId = idList.Where(p => p.TicketId == ticketId).Select(p => p.PhoneCallId).FirstOrDefault();
                                bool result = CreateCASActivityLog(ticketId, "", excelcode, true, phonecallId.ToString(), filePath, fileName, mainId);
                                SlmScr008Biz.UpdateCasFlag(phonecallId, result ? "1" : "2");
                            }
                        }
                    }
                    else if (reftype == "P")        //Prelead
                    {
                        PreleadPhoneCallBiz biz = new PreleadPhoneCallBiz();
                        if (excelcode == "001")
                        {
                            decimal phonecallId = biz.InsertPhonecallEcm(Convert.ToDecimal(refid), filePath, fileName, fileSize, HttpContext.Current.User.Identity.Name);
                            bool result = CreateCASActivityLog("", refid, excelcode, false, phonecallId.ToString(), filePath, fileName, mainId);
                            biz.UpdateCasFlag(phonecallId, result ? "1" : "2");
                        }
                        else if (excelcode == "002")
                        {
                            string[] preleadIdList = refid.Split(',');
                            var idList = biz.InsertPhonecallEcm(preleadIdList, filePath, fileName, fileSize, HttpContext.Current.User.Identity.Name, mainId);

                            foreach (string preleadId in preleadIdList)
                            {
                                decimal phonecallId = idList.Where(p => p.PreleadId == preleadId).Select(p => p.PhoneCallId).FirstOrDefault();
                                bool result = CreateCASActivityLog("", preleadId, excelcode, true, phonecallId.ToString(), filePath, fileName, mainId);
                                biz.UpdateCasFlag(phonecallId, result ? "1" : "2");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
            finally
            {
                DeleteLocalFile(fileLocation);    //ลบไฟล์ temp ที่โฟลเดอร์ Upload
            }

            return ret;
        }

        private void DeleteLocalFile(string fileLocation)
        {
            try
            {
                System.IO.File.Delete(fileLocation);
            }
            catch (Exception ex)
            {
                //ลบ Temp ไฟล์ใน folder Upload , เกิด error ไม่ต้อง throw
                _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        private bool CreateCASActivityLog(string ticketId, string preleadId, string ecode, bool isFleet, string phoneCallId, string invoiceFilePath, string invoiceFileName, string mainId)
        {
            LeadDataForCARLogService data = null;
            string ft = "";
            string od = "";
            string discountBath = "";
            string vat1PercentBath = "";
            string policyGrossPremiumPay = "";
            string str_preleadId = "";
            string systemCode = "";
            string securityKey = "";
            StaffData staff = null;
            string linkECM = "";

            try
            {
                //string url = SLMConstant.Ecm.SiteUrl.Replace("/dept/public", "");

                string ecmUrl = SLMConstant.Ecm.SiteUrl + "/" + SLMConstant.Ecm.ListName + "/" + mainId + "/" + invoiceFileName;
                linkECM = invoiceFileName + " / " + ecmUrl;

                data = CARServiceBiz.GetDataForCARLogService(ticketId, preleadId);

                if (data != null)
                {
                    ft = data.FT != null ? data.FT.Value.ToString("#,##0.00") : "";
                    od = data.OD != null ? data.OD.Value.ToString("#,##0.00") : "";
                    discountBath = data.DiscountBath != null ? data.DiscountBath.Value.ToString("#,##0.00") : "";
                    vat1PercentBath = data.Vat1PercentBath != null ? data.Vat1PercentBath.Value.ToString("#,##0.00") : "";
                    policyGrossPremiumPay = data.PolicyGrossPremiumPay != null ? data.PolicyGrossPremiumPay.Value.ToString("#,##0.00") : "";
                    str_preleadId = data.PreleadId != null ? data.PreleadId.Value.ToString() : preleadId;
                }
                else
                    str_preleadId = preleadId;

                systemCode = str_preleadId != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM;
                securityKey = str_preleadId != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey;

                //Activity Info
                List<Services.CARService.DataItem> act_dataItemList = new List<Services.CARService.DataItem>();
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "พิมพ์ใบเสนอราคาในรูปแบบ", DataValue = isFleet ? "Fleet" : "ปกติ" });
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.LicenseNo : "" });
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 4, DataLabel = "บริษัทประกันภัยรถยนต์", DataValue = data != null ? data.InsuranceCompany : "" });
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 5, DataLabel = "ทุนประกัน กรณีไฟไหม้/สูญหาย", DataValue = ft });
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 6, DataLabel = "ทุนประกัน กรณีชน", DataValue = od });
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 7, DataLabel = "ส่วนลด", DataValue = discountBath });
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 8, DataLabel = "หักภาษี ณ ที่จ่าย 1%", DataValue = vat1PercentBath });
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 9, DataLabel = "ค่าเบี้ยประกันที่ต้องชำระ", DataValue = policyGrossPremiumPay });
                act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 10, DataLabel = "Link ECM", DataValue = linkECM });

                //Customer Info
                List<Services.CARService.DataItem> cus_dataItemList = new List<Services.CARService.DataItem>();
                cus_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = data != null ? data.CitizenId : "" });
                cus_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = data != null ? data.CardTypeName : "" });
                cus_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = data != null ? data.CustomerName : "" });

                //Product Info
                List<Services.CARService.DataItem> prod_dataItemList = new List<Services.CARService.DataItem>();
                prod_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = data != null ? data.ProductGroupName : "" });
                prod_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = data != null ? data.ProductName : "" });
                prod_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = data != null ? data.CampaignName : "" });

                //Contract Info
                List<Services.CARService.DataItem> cont_dataItemList = new List<Services.CARService.DataItem>();
                cont_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
                cont_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = str_preleadId != "" ? "HP" : "" });
                cont_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.LicenseNo : "" });

                //Officer Info
                staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                List<Services.CARService.DataItem> off_dataItemList = new List<Services.CARService.DataItem>();
                off_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name });

                Services.CARService.CARServiceData carData = new Services.CARService.CARServiceData()
                {
                    ReferenceNo = phoneCallId,
                    SecurityKey = securityKey,
                    ServiceName = "CreateActivityLog",
                    SystemCode = systemCode,
                    TransactionDateTime = DateTime.Now,
                    ActivityInfoList = act_dataItemList,
                    CustomerInfoList = cus_dataItemList,
                    ProductInfoList = prod_dataItemList,
                    ContractInfoList = cont_dataItemList,
                    OfficerInfoList = off_dataItemList,
                    ActivityDateTime = DateTime.Now,
                    CampaignId = data != null ? data.CampaignId : "",
                    ChannelId = data != null ? data.ChannelId : "",
                    PreleadId = str_preleadId,
                    ProductGroupId = data != null ? data.ProductGroupId : "",
                    ProductId = data != null ? data.ProductId : "",
                    Status = data != null ? data.StatusName : "",
                    SubStatus = data != null ? data.SubStatusName : "",
                    TicketId = data != null ? data.TicketId : "",
                    SubscriptionId = data != null ? data.CitizenId : "",
                    TypeId = SLMConstant.CARLogService.Data.TypeId,
                    AreaId = SLMConstant.CARLogService.Data.AreaId,
                    SubAreaId = SLMConstant.CARLogService.Data.SubAreaId,
                    ActivityTypeId = SLMConstant.CARLogService.Data.ActivityType.TodoId,
                    ContractNo = data != null ? data.ContractNo : ""
                };

                if (data != null && !string.IsNullOrEmpty(data.CBSSubScriptionTypeId))
                    carData.CIFSubscriptionTypeId = data.CBSSubScriptionTypeId;

                return Services.CARService.CreateActivityLog(carData);
            }
            catch (Exception ex)
            {
                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                return false;
            }
        }

        #region Backup oldcasservice
        //oldcasservice
        //private void CreateCASActivityLog(string ticketId, string preleadId, string ecode, bool isFleet)
        //{
        //    PreleadPhoneCallBiz biz = new PreleadPhoneCallBiz();
        //    LeadDataForCARLogService data = null;
        //    string ft = "";
        //    string od = "";
        //    string discountBath = "";
        //    string vat1PercentBath = "";
        //    string policyGrossPremiumPay = "";
        //    string str_preleadId = "";
        //    string systemCode = "";
        //    string securityKey = "";
        //    StaffData staff = null;

        //    try
        //    {
        //        data = new SlmScr004Biz().GetDataForCARLogService(ticketId, preleadId);

        //        if (data != null)
        //        {
        //            ft = data.FT != null ? data.FT.Value.ToString("#,##0.00") : "";
        //            od = data.OD != null ? data.OD.Value.ToString("#,##0.00") : "";
        //            discountBath = data.DiscountBath != null ? data.DiscountBath.Value.ToString("#,##0.00") : "";
        //            vat1PercentBath = data.Vat1PercentBath != null ? data.Vat1PercentBath.Value.ToString("#,##0.00") : "";
        //            policyGrossPremiumPay = data.PolicyGrossPremiumPay != null ? data.PolicyGrossPremiumPay.Value.ToString("#,##0.00") : "";
        //            str_preleadId = data.PreleadId != null ? data.PreleadId.Value.ToString() : preleadId;
        //        }
        //        else
        //            str_preleadId = preleadId;

        //        systemCode = str_preleadId != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM;
        //        securityKey = str_preleadId != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey;

        //        //Activity Info
        //        List<Services.CASService.DataItem> act_dataItemList = new List<Services.CASService.DataItem>();
        //        act_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "พิมพ์ใบเสนอราคาในรูปแบบ", DataValue = isFleet ? "Fleet" : "ปกติ" });
        //        act_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
        //        act_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.LicenseNo : "" });
        //        act_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 4, DataLabel = "บริษัทประกันภัยรถยนต์", DataValue = data != null ? data.InsuranceCompany : "" });
        //        act_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 5, DataLabel = "ทุนประกัน กรณีไฟไหม้/สูญหาย", DataValue = ft });
        //        act_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 6, DataLabel = "ทุนประกัน กรณีชน", DataValue = od });
        //        act_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 7, DataLabel = "ส่วนลด", DataValue = discountBath });
        //        act_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 8, DataLabel = "หักภาษี ณ ที่จ่าย 1%", DataValue = vat1PercentBath });
        //        act_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 9, DataLabel = "ค่าเบี้ยประกันที่ต้องชำระ", DataValue = policyGrossPremiumPay });

        //        //Customer Info
        //        List<Services.CASService.DataItem> cus_dataItemList = new List<Services.CASService.DataItem>();
        //        cus_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = data != null ? data.CitizenId : "" });
        //        cus_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = data != null ? data.CardTypeName : "" });
        //        cus_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = txtFirstname.Text.Trim() + " " + txtLastname.Text.Trim() });

        //        //Product Info
        //        List<Services.CASService.DataItem> prod_dataItemList = new List<Services.CASService.DataItem>();
        //        prod_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = data != null ? data.ProductGroupName : "" });
        //        prod_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = data != null ? data.ProductName : "" });
        //        prod_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = data != null ? data.CampaignName : "" });

        //        //Contract Info
        //        List<Services.CASService.DataItem> cont_dataItemList = new List<Services.CASService.DataItem>();
        //        cont_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
        //        cont_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = str_preleadId != "" ? "HP" : "" });
        //        cont_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.LicenseNo : "" });

        //        //Officer Info
        //        staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
        //        List<Services.CASService.DataItem> off_dataItemList = new List<Services.CASService.DataItem>();
        //        off_dataItemList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name });

        //        Services.CASService.CASServiceData carData = new Services.CASService.CASServiceData()
        //        {
        //            ReferenceNo = ecode,
        //            SecurityKey = securityKey,
        //            ServiceName = "CreateActivityLog",
        //            SystemCode = systemCode,
        //            TransactionDateTime = DateTime.Now,
        //            ActivityInfoList = act_dataItemList,
        //            CustomerInfoList = cus_dataItemList,
        //            ProductInfoList = prod_dataItemList,
        //            ContractInfoList = cont_dataItemList,
        //            OfficerInfoList = off_dataItemList,
        //            ActivityDateTime = DateTime.Now,
        //            CampaignId = data != null ? data.CampaignId : "",
        //            ChannelId = data != null ? data.ChannelId : "",
        //            PreleadId = str_preleadId,
        //            ProductGroupId = data != null ? data.ProductGroupId : "",
        //            ProductId = data != null ? data.ProductId : "",
        //            Status = data != null ? data.StatusName : "",
        //            SubStatus = data != null ? data.SubStatusName : "",
        //            TicketId = data != null ? data.TicketId : "",
        //            SubscriptionId = data != null ? data.CitizenId : "",
        //            TypeName = SLMConstant.CARLogService.Data.Type,
        //            AreaName = SLMConstant.CARLogService.Data.Area,
        //            SubAreaName = SLMConstant.CARLogService.Data.SubArea,
        //            ActivityType = SLMConstant.CARLogService.Data.ActivityType.Todo,
        //            ContractNo = data != null ? data.ContractNo : ""
        //        };

        //        if (data != null && data.CardTypeId != null)
        //            carData.SubscriptionTypeId = data.CardTypeId.Value;

        //        Services.CASService.CreateActivityLog(carData);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Error ให้ลง Log ไว้ ไม่ต้อง Throw
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //    }
        //}
        #endregion

        protected void lb50Tawi_Click(object sender, EventArgs e)
        {
            try
            {
                if (ctlRenewInsurAct != null)
                {
                    if (!string.IsNullOrEmpty(Request["ticketid"]) && string.IsNullOrEmpty(Request["preleadid"]))  //lead mode เท่านั้น
                    {
                        string ticketId = ctlRenewInsurAct.GetTicketIdActiveTab();

                        CreateExcel("004", ticketId, "", "", "");
                        //script = "window.open('SLM_SCR_045.aspx?ecode=" + ecode + "&id=" + ticketId + "', 'tawi', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";

                        string key = "tawi_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string script = "window.open('SLM_SCR_045.aspx', '" + key + "', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                        ScriptManager.RegisterStartupScript(Page, GetType(), key, script, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Session.Remove(excelfilepath);
                Session.Remove(outputfilename);

                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbCreditForm_Click(object sender, EventArgs e)
        {
            try
            {
                if (ctlRenewInsurAct != null)
                {
                    if (!string.IsNullOrEmpty(Request["ticketid"]) && string.IsNullOrEmpty(Request["preleadid"]))  //lead mode เท่านั้น
                    {
                        string ticketId = ctlRenewInsurAct.GetTicketIdActiveTab();

                        CreateExcel("003", ticketId, "", "", "");
                        //script = "window.open('SLM_SCR_045.aspx?ecode=" + ecode + "&id=" + ticketId + "', 'creditform', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";

                        string key = "cdform_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string script = "window.open('SLM_SCR_045.aspx', '" + key + "', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                        ScriptManager.RegisterStartupScript(Page, GetType(), key, script, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Session.Remove(excelfilepath);
                Session.Remove(outputfilename);

                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbEditReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                string ticketId = ctlRenewInsurAct.GetTicketIdActiveTab();      //Get Main TicketId
                lblTicketIdPopupReceipt.Text = ticketId;
                var list = new SlmScr004Biz().GetReceiptList(ticketId);
                gvEditReceiptList.DataSource = list;
                gvEditReceiptList.DataBind();
                upEditReceiptList.Update();
                mpeEditReceiptList.Show();

                btnPrintReceipt.Enabled = list.Count > 0;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnPrintReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                bool select = false;
                foreach (GridViewRow row in gvEditReceiptList.Rows)
                {
                    if (((CheckBox)row.FindControl("cbSelect")).Checked)
                    {
                        select = true;
                        break;
                    }
                }
                if (select == false)
                {
                    AppUtil.ClientAlert(Page, "กรุณาเลือกใบเสร็จที่ต้องการพิมพ์");
                    mpeEditReceiptList.Show();
                    return;
                }

                if (ctlRenewInsurAct != null)
                {
                    string script = string.Empty;

                    if (!string.IsNullOrEmpty(Request["ticketid"]) && string.IsNullOrEmpty(Request["preleadid"]))  //lead mode เท่านั้น
                    {
                        int index = 0;
                        foreach (GridViewRow row in gvEditReceiptList.Rows)
                        {
                            index += 1;
                            if (((CheckBox)row.FindControl("cbSelect")).Checked)
                            {
                                string ticketId = ((Label)row.FindControl("lblTicketId")).Text.Trim();
                                string recNo = ((Label)row.FindControl("lblRecNo")).Text.Trim();

                                string key = index.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                                script = "window.open('SLM_SCR_045.aspx?ecode=005&ticketid=" + ticketId + "&recno=" + recNo + "', '" + key + "', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                                ScriptManager.RegisterClientScriptBlock(Page, GetType(), key, script, true);

                                //CreateExcel("005", ticketId, "", "", recNo);
                                //script = "window.open('SLM_SCR_045.aspx', '" + index.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                                //ScriptManager.RegisterClientScriptBlock(Page, GetType(), index.ToString(), script, true);
                            }
                        }
                    }
                }

                mpeEditReceiptList.Hide();
            }
            catch (Exception ex)
            {
                Session.Remove(excelfilepath);
                Session.Remove(outputfilename);

                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnClosePopupReceipt_Click(object sender, EventArgs e)
        {
            mpeEditReceiptList.Hide();
        }

        protected void lbBackToSearch_Click(object sender, EventArgs e)
        {
            RedirectToSearchPage();
        }

        protected void btnNextLead_Click(object sender, EventArgs e)
        {
            try
            {
                GoToNextLead();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbNextLead_Click(object sender, EventArgs e)
        {
            try
            {
                GoToNextLead();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ClearSession()
        {
            Session.Remove(SLMConstant.SessionName.default_phonecall_list);
            Session.Remove(SLMConstant.SessionName.renewinsure_phonecall_list);
            Session.Remove("allData");
            Session.Remove("dynamicTab");
            Session.Remove(SLMConstant.SessionName.Tawi50FormFilePath);
            Session.Remove(SLMConstant.SessionName.CreditFormFilePath);
            Session.Remove(SLMConstant.SessionName.DriverLicenseFormFilePath);
            Session.Remove("sessionSubstauslist");
        }

        private void GoToNextLead()
        {
            try
            {
                ClearSession();
                Session.Remove(SLMConstant.SessionName.tabscreenlist);  //ห้ามไว้ใน ClearSession()

                if (Session[nextleadlist] != null)
                {
                    int index = 0;
                    NextLeadData item = null;

                    var list = (List<NextLeadData>)Session[nextleadlist];
                    if (!string.IsNullOrEmpty(txtTicketID.Text.Trim()))
                    {
                        item = list.FirstOrDefault(p => p.TicketId == txtTicketID.Text.Trim());
                        index = list.IndexOf(item);
                    }
                    else
                    {
                        item = list.FirstOrDefault(p => p.PreleadId == decimal.Parse(txtPreleadId.Text.Trim()));
                        index = list.IndexOf(item);
                    }

                    if (index < list.Count - 1)     //ถ้ายังไม่ใช่ item สุดท้าย
                    {
                        NextLeadData obj = list[index + 1];

                        string type = new ConfigProductScreenBiz().GetFieldType(obj.CampaignId, obj.ProductId, SLMConstant.ConfigProductScreen.ActionType.View);

                        Session[SLMConstant.SessionName.CampaignId] = obj.CampaignId;
                        Session[SLMConstant.SessionName.ProductId] = obj.ProductId;

                        if (!string.IsNullOrEmpty(obj.TicketId))
                            Response.Redirect("SLM_SCR_004.aspx?ticketid=" + obj.TicketId + "&type=" + type + "&backtype=" + Request["backtype"], false);
                        else
                            Response.Redirect("SLM_SCR_004.aspx?preleadid=" + obj.PreleadId + "&type=" + type + "&backtype=" + Request["backtype"], false);
                    }
                    else
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "ไม่พบ Lead ถัดไป", GetSearchPageUrl());
                    }
                }
                else
                {
                    AppUtil.ClientAlertAndRedirect(Page, "ไม่พบ Lead ถัดไป", GetSearchPageUrl());
                }
            }
            catch
            {
                throw;
            }
        }

        //private void GoToNextLead()
        //{
        //    try
        //    {
        //        ClearSession();

        //        if (Session[nextleadlist] != null)
        //        {
        //            var list = (List<NextLeadData>)Session[nextleadlist];
        //            NextLeadData obj = list.FirstOrDefault();

        //            if (obj != null)
        //            {
        //                list.Remove(obj);
        //                Session[nextleadlist] = list;

        //                string type = new ConfigProductScreenBiz().GetFieldType(obj.CampaignId, obj.ProductId, SLMConstant.ConfigProductScreen.ActionType.View);

        //                if (!string.IsNullOrEmpty(obj.TicketId))
        //                    Response.Redirect("SLM_SCR_004.aspx?ticketid=" + obj.TicketId + "&type=" + type + "&backtype=" + Request["backtype"], false);
        //                else
        //                    Response.Redirect("SLM_SCR_004.aspx?preleadid=" + obj.PreleadId + "&type=" + type + "&backtype=" + Request["backtype"], false);
        //            }
        //            else
        //            {
        //                AppUtil.ClientAlertAndRedirect(Page, "ไม่พบ Lead ถัดไป", GetSearchPageUrl());
        //            }
        //        }
        //        else
        //            RedirectToSearchPage();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        protected void btnSearchScript_Click(object sender, EventArgs e)
        {
            try
            {
                string word = txtSearchScript.Text.Trim();

                if (!string.IsNullOrEmpty(word))
                {
                    foreach (RepeaterItem item in rptSaleScript.Items)
                    {
                        //Clear ของเก่า
                        string plainText = ((Label)item.FindControl("lblScriptSubject")).Text.Replace("<span style='background-color:Yellow;'>", "").Replace("</span>", "");
                        ((Label)item.FindControl("lblScriptSubject")).Text = plainText;
                        plainText = ((Label)item.FindControl("ltScriptDetail")).Text.Replace("<span style='background-color:Yellow;'>", "").Replace("</span>", "");
                        ((Label)item.FindControl("ltScriptDetail")).Text = plainText;

                        ((Label)item.FindControl("lblScriptSubject")).Text = ((Label)item.FindControl("lblScriptSubject")).Text.Replace(word, "<span style='background-color:Yellow;'>" + word + "</span>");
                        ((Label)item.FindControl("ltScriptDetail")).Text = ((Label)item.FindControl("ltScriptDetail")).Text.Replace(word, "<span style='background-color:Yellow;'>" + word + "</span>");
                    }
                    foreach (RepeaterItem item in rptQandA.Items)
                    {
                        //Clear ของเก่า
                        string plainText = ((Label)item.FindControl("lblQandASubject")).Text.Replace("<span style='background-color:Yellow;'>", "").Replace("</span>", "");
                        ((Label)item.FindControl("lblQandASubject")).Text = plainText;
                        plainText = ((Label)item.FindControl("ltQandADetail")).Text.Replace("<span style='background-color:Yellow;'>", "").Replace("</span>", "");
                        ((Label)item.FindControl("ltQandADetail")).Text = plainText;

                        ((Label)item.FindControl("lblQandASubject")).Text = ((Label)item.FindControl("lblQandASubject")).Text.Replace(word, "<span style='background-color:Yellow;'>" + word + "</span>");
                        ((Label)item.FindControl("ltQandADetail")).Text = ((Label)item.FindControl("ltQandADetail")).Text.Replace(word, "<span style='background-color:Yellow;'>" + word + "</span>");
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

        private bool CheckTicketIdPrivilege(string ticketId, StaffData staff)
        {
            string logError = "";
            if (!RoleBiz.GetTicketIdPrivilege(ticketId, HttpContext.Current.User.Identity.Name, staff.StaffTypeId, staff.BranchCode, "SLM_SCR_004", out logError))
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
                    {
                        message += " ณ ปัจจุบันผู้เป็นเจ้าของ คือ " + data.OwnerName.ToString().Trim() + " และ Delegate คือ " + data.DelegateName.ToString().Trim();
                    }
                    else if (!string.IsNullOrEmpty(data.OwnerName))
                    {
                        message += " ณ ปัจจุบันผู้เป็นเจ้าของ คือ " + data.OwnerName.ToString().Trim();
                    }
                    else if (!string.IsNullOrEmpty(data.DelegateName))
                    {
                        message += " ณ ปัจจุบัน Delegate คือ " + data.DelegateName.ToString().Trim();
                    }
                }
                else
                {
                    message = "ไม่พบ Ticket Id " + Request["ticketid"].ToString() + " ในระบบ";
                }

                AppUtil.ClientAlertAndRedirect(Page, message, GetSearchPageUrl());
                return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(logError))
                {
                    _log.Error(logError);
                }
                return true;
            }
        }

        public void UpdateMainData(string ticketId, string ownerBranchName, string ownerLeadName, string delegateBranchName, string delegateLeadName, string telNo1, string statusDesc, DateTime contactLatestDate, bool doUpdateOwnerLogging, string externalSubStatusDesc, string cardTypeId, string citizenId, string subStatusCode, DateTime? nextContactDate)
        {
            try
            {
                if (txtTicketID.Text.Trim() != "")
                {
                    txtOwnerLead.Text = ownerLeadName;
                    txtOwnerLead.ToolTip = ownerLeadName;
                    txtOwnerBranch.Text = ownerBranchName;
                    txtOwnerBranch.ToolTip = ownerBranchName;
                    txtDelegateLead.Text = delegateLeadName;
                    txtDelegateLead.ToolTip = delegateLeadName;
                    txtDelegateBranch.Text = delegateBranchName;
                    txtDelegateBranch.ToolTip = delegateBranchName;
                    txtstatus.Text = statusDesc;
                    txtExternalSubStatusDesc.Text = externalSubStatusDesc;
                    txtExternalSubStatusDesc.ToolTip = externalSubStatusDesc;
                    txtTelNo_1.Text = telNo1;
                    txtCardTypeId.Text = cardTypeId;
                    txtCitizenId.Text = citizenId;

                    if (contactLatestDate != null && contactLatestDate.Year != 1)
                    {
                        txtContactLatestDate.Text = contactLatestDate.ToString("dd/MM/") + contactLatestDate.Year.ToString() + " " + contactLatestDate.ToString("HH:mm:ss");

                        if (txtContactFirstDate.Text.Trim() == "")
                            txtContactFirstDate.Text = contactLatestDate.ToString("dd/MM/") + contactLatestDate.Year.ToString() + " " + contactLatestDate.ToString("HH:mm:ss");
                    }
                }
                else if (txtPreleadId.Text.Trim() != "")
                {
                    txtSubStatusCode.Text = subStatusCode;
                    txtExternalSubStatusDesc.Text = externalSubStatusDesc;
                    txtExternalSubStatusDesc.ToolTip = externalSubStatusDesc;
                    txtTelNo_1.Text = telNo1;
                    txtCardTypeId.Text = cardTypeId;
                    txtCitizenId.Text = citizenId;

                    if (contactLatestDate != null && contactLatestDate.Year != 1)
                        txtContactLatestDatePrelead.Text = contactLatestDate.ToString("dd/MM/") + contactLatestDate.Year.ToString() + " " + contactLatestDate.ToString("HH:mm:ss");

                    if (nextContactDate != null && nextContactDate.Value.Year != 1)
                        txtNextContactDate.Text = nextContactDate.Value.ToString("dd/MM/") + nextContactDate.Value.Year.ToString();
                }

                //btnLoadLeadInfo_Click(null, null);

                upMainData.Update();
                upMainData2.Update();

                if (doUpdateOwnerLogging == true && ctlOwnerLogging != null)
                {
                    ctlOwnerLogging.GetOwnerLogingList(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
                    cbLoadOwnerLogging.Checked = true;
                }
                upTabMain.Update();
            }
            catch
            {
                throw;
            }
        }

        public void GetMainData(out string ticketId, out string firstname, out string lastname, out string cardTypeId, out string citizenId, out string campaignId, out string campaignName, out string ownerBranchCode, out string ownerLead, out string delegateBranchCode, out string delegateLead
            , out string telNo1, out string statusCode, out string channelId, out string productGroupId, out string productGroupName, out string productId, out string productName)
        {
            try
            {
                ticketId = txtTicketID.Text.Trim();
                firstname = txtFirstname.Text.Trim();
                lastname = txtLastname.Text.Trim();
                cardTypeId = txtCardTypeId.Text.Trim();
                citizenId = txtCitizenId.Text.Trim();
                campaignId = txtCampaignId.Text.Trim();
                campaignName = txtCampaignName.Text.Trim();
                ownerBranchCode = txtOwnerBranchCode.Text.Trim();
                ownerLead = txtOwnerUsername.Text.Trim();
                delegateBranchCode = txtDelegateBranchCode.Text.Trim();
                delegateLead = txtDelegateUsername.Text.Trim();
                telNo1 = txtTelNo_1.Text.Trim();
                statusCode = txtStatusCode.Text.Trim();
                channelId = txtChannelId.Text.Trim();
                productGroupId = txtProductGroupId.Text.Trim();
                productGroupName = txtProductGroupName.Text.Trim();
                productId = txtProductId.Text.Trim();
                productName = txtProductName.Text.Trim();
            }
            catch
            {
                throw;
            }
        }

        //public void UpdateStatusDesc(string statusDesc, DateTime contactLatestDate)
        //{
        //    try
        //    {
        //        if (txtTicketID.Text.Trim() != "")
        //        {
        //            var lead = SlmScr004Biz.GetLeadRenewInsureData(txtTicketID.Text.Trim());
        //            if (lead != null)
        //            {
        //                txtOwnerLead.Text = lead.OwnerName;
        //                txtOwnerLead.ToolTip = lead.OwnerName;
        //                txtOwnerBranch.Text = lead.OwnerBranchName;
        //                txtOwnerBranch.ToolTip = lead.OwnerBranchName;
        //                txtDelegateLead.Text = lead.DelegateName;
        //                txtDelegateLead.ToolTip = lead.DelegateName;
        //                txtDelegateBranch.Text = lead.DelegateBranchName;
        //                txtDelegateBranch.ToolTip = lead.DelegateBranchName;
        //                txtstatus.Text = lead.StatusDesc;
        //                txtExternalSubStatusDesc.Text = lead.ExternalSubStatusDesc;
        //                txtExternalSubStatusDesc.ToolTip = lead.ExternalSubStatusDesc;
        //                txtTelNo_1.Text = lead.TelNo1;

        //                //if (contactLatestDate != null && contactLatestDate.Year != 1)
        //                //    txtContactLatestDate.Text = contactLatestDate.ToString("dd/MM/") + contactLatestDate.Year.ToString() + " " + contactLatestDate.ToString("HH:mm:ss");
        //                if (lead.ContactLatestDate != null)
        //                    txtContactLatestDate.Text = lead.ContactLatestDate.Value.ToString("dd/MM/") + lead.ContactLatestDate.Value.Year.ToString() + " " + lead.ContactLatestDate.Value.ToString("HH:mm:ss");
        //                if (lead.ContactFirstDate != null)
        //                    txtContactFirstDate.Text = lead.ContactFirstDate.Value.ToString("dd/MM/") + lead.ContactFirstDate.Value.Year.ToString() + " " + lead.ContactFirstDate.Value.ToString("HH:mm:ss");

        //            }

        //            if (IsPostBack && ctlLeadInfo != null)
        //            {
        //                if (!string.IsNullOrEmpty(Request["ticketid"]))
        //                {
        //                    ctlLeadInfo.GetLeadData(SlmScr004Biz.SearchSCR004Data(Request["ticketid"]));
        //                }
        //                else if (!string.IsNullOrEmpty(Request["preleadid"]))
        //                {
        //                    ctlLeadInfo.GetPreleadData(SLMUtil.SafeDecimal(Request["preleadid"]));
        //                }
        //            }
        //        }
        //        else if (txtPreleadId.Text.Trim() != "")
        //        {
        //            PreleadViewData prelead = new PreleadBiz().GetPreleadData(decimal.Parse(txtPreleadId.Text.Trim()), "", "", "");
        //            if (prelead != null)
        //            {
        //                txtstatus.Text = prelead.StatusDesc;
        //                txtExternalSubStatusDesc.Text = prelead.SubStatusDesc;
        //                txtTelNo_1.Text = prelead.TelNo1;
        //                txtOwnerBranch.Text = prelead.OwnerBranchName;
        //                txtOwnerBranch.ToolTip = prelead.OwnerBranchName;
        //                txtOwnerLead.Text = prelead.OwnerName;
        //                txtOwnerLead.Text = prelead.OwnerName;

        //                if (prelead.PreContactLatestDate != null)
        //                    txtContactLatestDatePrelead.Text = prelead.PreContactLatestDate.Value.ToString("dd/MM/") + prelead.PreContactLatestDate.Value.Year.ToString() + " " + prelead.PreContactLatestDate.Value.ToString("HH:mm:ss");

        //                if (prelead.AssignedDate != null)
        //                    txtAssignDatePrelead.Text = prelead.AssignedDate.Value.ToString("dd/MM/") + prelead.AssignedDate.Value.Year.ToString() + " " + prelead.AssignedDate.Value.ToString("HH:mm:ss");
        //            }
        //        }

        //        upMainData.Update();
        //        upMainData2.Update();

        //        if (ctlOwnerLogging != null)
        //        {
        //            ctlOwnerLogging.GetOwnerLogingList(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
        //            //cbLoadOwnerLogging.Checked = true;
        //        }
        //        upTabMain.Update();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public void UpdateStatusDesc(string statusDesc, DateTime contactLatestDate)
        {
            try
            {
                if (txtTicketID.Text.Trim() != "")
                {
                    string tmp_statusDesc = "";
                    string tmp_subStatusDesc = "";

                    SlmScr004Biz.GetLeadStatus(txtTicketID.Text.Trim(), out tmp_statusDesc, out tmp_subStatusDesc);
                    txtstatus.Text = tmp_statusDesc;
                    txtExternalSubStatusDesc.Text = tmp_subStatusDesc;
                    txtExternalSubStatusDesc.ToolTip = tmp_subStatusDesc;

                    if (contactLatestDate != null && contactLatestDate.Year != 1)
                    {
                        txtContactLatestDate.Text = contactLatestDate.ToString("dd/MM/") + contactLatestDate.Year.ToString() + " " + contactLatestDate.ToString("HH:mm:ss");
                    }
                }
                else if (txtPreleadId.Text.Trim() != "")
                {
                    if (contactLatestDate != null && contactLatestDate.Year != 1)
                    {
                        txtContactLatestDatePrelead.Text = contactLatestDate.ToString("dd/MM/") + contactLatestDate.Year.ToString() + " " + contactLatestDate.ToString("HH:mm:ss");
                    }
                }

                upMainData.Update();
                upMainData2.Update();
            }
            catch
            {
                throw;
            }
        }

        public void GetOBTMainData(out string contractNo, out string ticketId, out string firstname, out string lastname, out string cardTypeId, out string citizenId, out string campaignId, out string campaignName, out string productId, out string productName, out string productGroupId, out string productGroupName, out string ownerBranchCode, out string ownerBranchName, out string ownerUsername, out string ownerName, out string ownerEmpCode
            , out string delegateBranchCode, out string delegateBranchName, out string delegateUsername, out string delegateName, out string telNo1, out string statusCode, out string statusDesc, out string subStatusCode, out string subStatusDesc, out string channelId, out DateTime? nextContactDate, out string carLicenseNo)
        {
            try
            {
                contractNo = txtContractNo.Text.Trim();
                ticketId = txtTicketID.Text.Trim();
                firstname = txtFirstname.Text.Trim();
                lastname = txtLastname.Text.Trim();
                cardTypeId = txtCardTypeId.Text.Trim();
                citizenId = txtCitizenId.Text.Trim();
                campaignId = txtCampaignId.Text.Trim();
                campaignName = txtCampaignName.Text.Trim();
                ownerBranchCode = txtOwnerBranchCode.Text.Trim();
                ownerBranchName = txtOwnerBranch.Text.Trim();
                ownerUsername = txtOwnerUsername.Text.Trim();
                ownerEmpCode = txtOwnerEmpCode.Text.Trim();
                ownerName = txtOwnerLead.Text.Trim();
                delegateBranchCode = txtDelegateBranchCode.Text.Trim();
                delegateBranchName = txtDelegateBranch.Text.Trim();
                delegateUsername = txtDelegateUsername.Text.Trim();
                delegateName = txtDelegateLead.Text.Trim();
                telNo1 = txtTelNo_1.Text.Trim();
                statusCode = txtStatusCode.Text.Trim();
                statusDesc = txtstatus.Text.Trim();
                subStatusCode = txtSubStatusCode.Text.Trim();
                subStatusDesc = txtExternalSubStatusDesc.Text.Trim();
                channelId = txtChannelId.Text.Trim();
                productGroupId = txtProductGroupId.Text.Trim();
                productGroupName = txtProductGroupName.Text.Trim();
                productId = txtProductId.Text.Trim();
                productName = txtProductName.Text.Trim();
                carLicenseNo = txtCarLicenseNo.Text.Trim();
                if (txtNextContactDate.Text.Trim() != "")
                {
                    string[] strdate = txtNextContactDate.Text.Trim().Split('/');
                    nextContactDate = new DateTime(Convert.ToInt32(strdate[2]), Convert.ToInt32(strdate[1]), Convert.ToInt32(strdate[0]));
                }
                else
                    nextContactDate = null;
            }
            catch
            {
                throw;
            }
        }

        protected void btnReturnPremium_Click(object sender, EventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(((Button)sender).CommandArgument);
                string raId = ((Label)gvPremium.Rows[index].FindControl("lblRaId")).Text.Trim();
                string ticketId = ctlRenewInsurAct.GetTicketIdActiveTab();      //Get Active TicketId

                if (!string.IsNullOrEmpty(raId))
                {
                    new ConfigProductPremiumBiz().ReturnPremium(Convert.ToDecimal(raId));
                    UpdateGridviewPremium(ticketId);
                }
                else
                    AppUtil.ClientAlert(Page, "ไม่พบข้อมูล RaId");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void gvPremium_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox cbSelected = (CheckBox)e.Row.FindControl("cbSelected");
                TextBox txtTotalGive = (TextBox)e.Row.FindControl("txtTotalGive");

                string script = @"if (document.getElementById('" + cbSelected.ClientID + @"').checked){
                                    document.getElementById('" + txtTotalGive.ClientID + @"').disabled = false;
                                }
                                else{
                                    document.getElementById('" + txtTotalGive.ClientID + @"').value = '';
                                    document.getElementById('" + txtTotalGive.ClientID + @"').disabled = true;
                                }";

                cbSelected.Attributes.Add("onchange", script);

                string raId = ((Label)e.Row.FindControl("lblRaId")).Text.Trim();
                if (string.IsNullOrEmpty(raId))
                {
                    ((CheckBox)e.Row.FindControl("cbSelected")).Visible = true;
                    ((TextBox)e.Row.FindControl("txtTotalGive")).Enabled = false;       //checkbox ก่อน ถึงจะ enable = true
                    ((Button)e.Row.FindControl("btnReturnPremium")).Visible = false;
                }
                else
                {
                    ((CheckBox)e.Row.FindControl("cbSelected")).Visible = false;
                    ((TextBox)e.Row.FindControl("txtTotalGive")).Enabled = false;
                    ((Button)e.Row.FindControl("btnReturnPremium")).Visible = true;
                }

                if (((Label)e.Row.FindControl("lblTotalRemain")).Text.Trim() == "0" || ((Label)e.Row.FindControl("lblTotalRemain")).Text.Trim() == "")
                    ((CheckBox)e.Row.FindControl("cbSelected")).Visible = false;
            }
        }

        private List<PremiumData> SavedDataChange(string ticketId)
        {
            try
            {
                List<PremiumData> list = new List<PremiumData>();
                var premiumList = new ConfigProductPremiumBiz().GetPremiumList(txtCampaignId.Text.Trim(), txtProductId.Text.Trim());

                if (!string.IsNullOrEmpty(Request["ticketid"])) //Lead Mode
                {
                    foreach (GridViewRow row in gvPremium.Rows)
                    {
                        CheckBox cbSelected = (CheckBox)row.FindControl("cbSelected");
                        Label lblTotalRemain = (Label)row.FindControl("lblTotalRemain");
                        TextBox txtTotalGive = (TextBox)row.FindControl("txtTotalGive");
                        Label lblPremiumId = (Label)row.FindControl("lblPremiumId");

                        decimal premuimId = string.IsNullOrEmpty(lblPremiumId.Text.Trim()) ? 0 : Convert.ToDecimal(lblPremiumId.Text.Trim().Replace(",", ""));
                        int totalRemain = string.IsNullOrEmpty(lblTotalRemain.Text.Trim()) ? 0 : Convert.ToInt32(lblTotalRemain.Text.Trim().Replace(",", ""));
                        int totalGive = string.IsNullOrEmpty(txtTotalGive.Text.Trim()) ? 0 : Convert.ToInt32(txtTotalGive.Text.Trim().Replace(",", ""));

                        if (cbSelected.Checked && premuimId != 0 && totalGive > 0)
                        {
                            if (totalGive > totalRemain)
                                throw new Exception("กรุณาตรวจสอบจำนวนของ Premium ที่ให้");

                            var premium = premiumList.Where(p => p.PremiumId == premuimId).FirstOrDefault();
                            if (premium != null)
                            {
                                if (premium.TotalRemain == null || totalGive > premium.TotalRemain)
                                {
                                    lblTotalRemain.Text = premium.TotalRemain != null ? premium.TotalRemain.Value.ToString() : "0";
                                    txtTotalGive.Enabled = true;
                                    upPremium.Update();
                                    throw new Exception("กรุณาตรวจสอบจำนวนของ Premium ที่ให้");
                                }
                            }
                            else
                            {
                                gvPremium.DataSource = premiumList;
                                gvPremium.DataBind();
                                upPremium.Update();
                                throw new Exception("ไม่พบข้อมูลของ Premium ที่ระบุ กรุณาตรวจสอบอีกครั้ง");
                            }

                            PremiumData data = new PremiumData()
                            {
                                TicketId = ticketId,
                                PremiumId = premuimId,
                                TotalGive = totalGive
                            };
                            list.Add(data);
                        }
                    }
                }

                return list;
            }
            catch
            {
                throw;
            }
        }

        private void UpdateGridviewPremium(string ticketId)
        {
            try
            {
                ConfigProductPremiumBiz biz = new ConfigProductPremiumBiz();
                var premiumList = biz.GetPremiumByTicketId(ticketId, txtProductId.Text.Trim(), txtCampaignId.Text.Trim());
                gvPremium.DataSource = premiumList;
                gvPremium.DataBind();

                upPremium.Update();
            }
            catch
            {
                throw;
            }
        }

        //ใช้เพื่อรองรับการสั่ง Update data จาก User Control
        //public void UpdatePreleadData(string statusDesc)
        //{
        //    GetPreleadData(txtPreleadId.Text.Trim(), "", "", "");
        //    upMainData.Update();
        //    upMainData2.Update();
        //    tabOwnerLogging.GetOwnerLogingList(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
        //    upTabMain.Update();
        //}

        private void GetLeadData()
        {

            if (ctlActivity != null)
            {
                lead = SlmScr004Biz.GetLeadDefaultData(txtTicketID.Text.Trim());
            }
            else if (ctlRenewInsurAct != null)
            {
                lead = SlmScr004Biz.GetLeadRenewInsureData(txtTicketID.Text.Trim());
            }

            if (lead != null)
            {
                //OBT
                txtPreleadId.Text = lead.PreleadId != null ? lead.PreleadId.Value.ToString() : "";    //Added by Pom 21/03/2016
                txtContractNo.Text = lead.ContractNo;
                txtTeamCode.Text = lead.TeamTelesalesCode;
                if (lead.PreleadContactLatestDate != null)
                    txtContactLatestDatePrelead.Text = lead.PreleadContactLatestDate.Value.ToString("dd/MM/") + lead.PreleadContactLatestDate.Value.Year.ToString() + " " + lead.PreleadContactLatestDate.Value.ToString("HH:mm:ss");
                if (lead.PreleadAssignDate != null)
                    txtAssignDatePrelead.Text = lead.PreleadAssignDate.Value.ToString("dd/MM/") + lead.PreleadAssignDate.Value.Year.ToString() + " " + lead.PreleadAssignDate.Value.ToString("HH:mm:ss");

                //Lead
                //if (ctlLeadInfo != null) ctlLeadInfo.GetLeadData(lead);     //Comment by Pom 01/09/2016
                //tabInsureSummary.GetPreleadInsureSummary(lead.PreleadId.Value.ToString());    //Moved to PageLoad()
                txtstatus.Text = lead.StatusDesc;
                txtStatusCode.Text = lead.StatusCode;
                txtExternalSubStatusDesc.Text = lead.ExternalSubStatusDesc;
                txtExternalSubStatusDesc.ToolTip = lead.ExternalSubStatusDesc;
                txtFirstname.Text = lead.FirstName;
                txtFirstname.ToolTip = lead.FirstName;
                txtLastname.Text = lead.LastName;
                txtLastname.ToolTip = lead.LastName;
                txtCampaignId.Text = lead.CampaignId;
                txtCampaignName.Text = lead.CampaignName;
                txtCampaignName.ToolTip = lead.CampaignName;
                txtChannelId.Text = lead.ChannelId;
                txtProductGroupId.Text = lead.ProductGroupId;
                txtProductGroupName.Text = lead.ProductGroupName;
                txtProductId.Text = lead.ProductId;
                txtProductName.Text = lead.ProductName;
                txtInterestedProd.Text = lead.InterestedProd;
                txtInterestedProd.ToolTip = lead.InterestedProd;
                txtCardTypeId.Text = lead.CardType != null ? lead.CardType.ToString() : "";
                txtCitizenId.Text = lead.CitizenId;
                txtCountryId.Text = (lead.CountryId ?? 0).ToString();
                txtTelNo1.Text = lead.TelNo1;
                if (lead.ContactLatestDate != null)
                    txtContactLatestDate.Text = lead.ContactLatestDate.Value.ToString("dd/MM/") + lead.ContactLatestDate.Value.Year.ToString() + " " + lead.ContactLatestDate.Value.ToString("HH:mm:ss");
                if (lead.AssignedDate != null)
                    txtAssignDate.Text = lead.AssignedDate.Value.ToString("dd/MM/") + lead.AssignedDate.Value.Year.ToString() + " " + lead.AssignedDate.Value.ToString("HH:mm:ss");
                if (lead.ContactFirstDate != null)
                    txtContactFirstDate.Text = lead.ContactFirstDate.Value.ToString("dd/MM/") + lead.ContactFirstDate.Value.Year.ToString() + " " + lead.ContactFirstDate.Value.ToString("HH:mm:ss");

                txtOwnerUsername.Text = lead.OwnerUsername;
                txtOwnerLead.Text = lead.OwnerName;
                txtOwnerLead.ToolTip = lead.OwnerName;
                txtDelegateUsername.Text = lead.DelegateUsername;
                txtDelegateLead.Text = lead.DelegateName;
                txtDelegateLead.ToolTip = lead.DelegateName;
                txtDelegateBranchCode.Text = lead.DelegateBranchCode;
                txtDelegateBranch.Text = lead.DelegateBranchName;
                txtDelegateBranch.ToolTip = lead.DelegateBranchName;
                txtOwnerBranchCode.Text = lead.OwnerBranchCode;
                txtOwnerBranch.Text = lead.OwnerBranchName;
                txtOwnerBranch.ToolTip = lead.OwnerBranchName;
                txtTelNo_1.Text = lead.TelNo1;
                txtTelNo2.Text = lead.TelNo2;
                txtExt2.Text = lead.Ext2;
                txtTelNo3.Text = lead.TelNo3;
                txtExt3.Text = lead.Ext3;
                //txtIsCOC.Text = lead.ISCOC;

                //COC
                if (lead.CocAssignedDate != null)
                    txtCOCAssignDate.Text = lead.CocAssignedDate.Value.ToString("dd/MM/") + lead.CocAssignedDate.Value.Year.ToString() + " " + lead.CocAssignedDate.Value.ToString("HH:mm:ss");

                txtMarketingOwner.Text = lead.MarketingOwnerName;
                txtMarketingOwner.ToolTip = lead.MarketingOwnerName;
                txtLastOwner.Text = lead.LastOwnerName;
                txtLastOwner.ToolTip = lead.LastOwnerName;
                txtCocStatus.Text = lead.CocStatusDesc;
                txtCocStatus.ToolTip = lead.CocStatusDesc;
                txtCocTeam.Text = lead.COCCurrentTeam;
                txtCocTeam.ToolTip = lead.COCCurrentTeam;
                txtDetail.Text = lead.Detail;

                //Icons
                if (lead.HasAdamsUrl)
                {
                    _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + txtTicketID.Text.Trim() + " Begin GetLeadData (GetLeadDataForAdam)");
                    imbDoc.Visible = true;
                    LeadDataForAdam leadData = SlmScr003Biz.GetLeadDataForAdam(txtTicketID.Text.Trim());
                    //convert customerdetail.contactBranch, channelInfo.Branch, to newBranchCode
                    leadData.ContactBranch = BranchBiz.GetBranchCodeNew(leadData.ContactBranch);
                    leadData.Branch = BranchBiz.GetBranchCodeNew(leadData.Branch);
                    imbDoc.OnClientClick = AppUtil.GetCallAdamScript(leadData, HttpContext.Current.User.Identity.Name, txtLoginEmpCode.Text.Trim(), false, "");
                    _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + txtTicketID.Text.Trim() + " End GetLeadData (GetLeadDataForAdam)");
                }
                else
                    imbDoc.Visible = false;

                if (!string.IsNullOrEmpty(lead.CalculatorUrl))
                {
                    imbCal.Visible = true;
                    imbCal.OnClientClick = AppUtil.GetCallCalculatorScript(txtTicketID.Text.Trim(), lead.CalculatorUrl);
                }
                else
                    imbCal.Visible = false;

                if (!string.IsNullOrEmpty(lead.AppNo))
                {
                    decimal staffTypeId = txtLoginStaffTypeId.Text.Trim() != "" ? Convert.ToDecimal(txtLoginStaffTypeId.Text.Trim()) : 0;
                    string privilegeNCB = SlmScr003Biz.GetPrivilegeNCB(lead.ProductId, staffTypeId);

                    if (privilegeNCB != "")
                    {
                        imbOthers.Visible = true;
                        imbOthers.OnClientClick = AppUtil.GetCallAolSummaryReportScript(lead.AppNo, txtLoginEmpCode.Text.Trim(), txtLoginStaffTypeDesc.Text.Trim(), privilegeNCB);
                    }
                    else
                        imbOthers.Visible = false;
                }
                else
                    imbOthers.Visible = false;

                //2016-12-27 --> SR:5905-123
                txtTicketIDRefer.Text = lead.TicketIDRefer;

                GetCampaignList();
                //GetTicketIdReferAndRelate();
            }

        }

        private bool GetPreleadData(string preleadId, string licenseNo, string campaignId, string ContractNo)
        {
            try
            {
                PreleadViewData data = new PreleadBiz().GetPreleadData(decimal.Parse(preleadId), licenseNo, campaignId, ContractNo);
                if (data != null)
                {
                    txtContractNo.Text = data.ContractNo;
                    txtTicketID.Text = data.TicketId;
                    txtFirstname.Text = data.Firstname;
                    txtLastname.Text = data.Lastname;
                    txtCardTypeId.Text = data.CardTypeId != null ? data.CardTypeId.Value.ToString() : "";
                    txtCitizenId.Text = data.CitizenId;
                    txtCampaignId.Text = data.CampaignId;
                    txtCampaignName.Text = data.CampaignName;
                    txtOwnerBranchCode.Text = data.BranchCode;
                    txtOwnerBranch.Text = data.OwnerBranchName;
                    txtOwnerBranch.ToolTip = data.OwnerBranchName;
                    txtOwnerEmpCode.Text = data.OwnerEmpCode;
                    txtOwnerLead.Text = data.OwnerName;
                    txtOwnerLead.Text = data.OwnerName;
                    //ไม่มี Delegate
                    txtTelNo_1.Text = data.TelNo1;
                    txtStatusCode.Text = data.StatusCode;
                    txtstatus.Text = data.StatusDesc;
                    txtSubStatusCode.Text = data.SubStatusCode;
                    txtExternalSubStatusDesc.Text = data.SubStatusDesc;
                    txtChannelId.Text = SLMConstant.CARLogService.CARPreleadChannelId;
                    txtProductGroupId.Text = data.ProductGroupId;
                    txtProductGroupName.Text = data.ProductGroupName;
                    txtProductId.Text = data.ProductId;
                    txtProductName.Text = data.ProductName;
                    txtTeamCode.Text = data.TeamCode;
                    txtCarLicenseNo.Text = data.CarLicenseNo;

                    if (data.PreContactLatestDate != null)
                        txtContactLatestDatePrelead.Text = data.PreContactLatestDate.Value.ToString("dd/MM/") + data.PreContactLatestDate.Value.Year.ToString() + " " + data.PreContactLatestDate.Value.ToString("HH:mm:ss");

                    if (data.AssignedDate != null)
                        txtAssignDatePrelead.Text = data.AssignedDate.Value.ToString("dd/MM/") + data.AssignedDate.Value.Year.ToString() + " " + data.AssignedDate.Value.ToString("HH:mm:ss");

                    if (data.NextContactDate != null)
                        txtNextContactDate.Text = data.NextContactDate.Value.ToString("dd/MM/") + data.NextContactDate.Value.Year.ToString();

                    //if (ctlExistingLead != null) ctlExistingLead.GetExistingLeadList(data.CitizenId, "", "");   //ใช้ร่วมกันระหว่าง Lead กับ Prelead       //Comment by Pom 01/09/2016
                    //if (ctlExistingProduct != null) ctlExistingProduct.GetExistingProductList(data.CitizenId);  //ใช้ร่วมกันระหว่าง Lead กับ Prelead         //Comment by Pom 01/09/2016
                    //if (ctlOwnerLogging != null) ctlOwnerLogging.GetOwnerLogingList(data.TicketId, preleadId);  //ใช้ร่วมกันระหว่าง Lead กับ Prelead         //Comment by Pom 01/09/2016
                    //if (ctlActivity != null) ctlActivity.InitialControlPrelead(data);
                    //if (ctlNote != null) ctlNote.InitialControlPrelead(preleadId);

                    if (ctlExistingLead != null) ctlExistingLead.SetDefaultValue(data.CitizenId, "", "");   //ใช้ร่วมกันระหว่าง Lead กับ Prelead
                    if (ctlExistingProduct != null) ctlExistingProduct.SetDefaultValue(data.CitizenId, "0", data.CardTypeId.ToString());  //ใช้ร่วมกันระหว่าง Lead กับ Prelead
                    if (ctlOwnerLogging != null) ctlOwnerLogging.SetDefaultValue(data.TicketId, preleadId);  //ใช้ร่วมกันระหว่าง Lead กับ Prelead
                    if (ctlRenewInsurAct != null) ctlRenewInsurAct.InitialControlPrelead(data, txtLoginNameTH.Text.Trim());
                    if (ctlNote != null) ctlNote.SetDefaultValue("", preleadId);

                    imbCal.Visible = false;
                    imbDoc.Visible = false;
                    imbOthers.Visible = false;
                    imbCopyLead.Visible = false;
                    upHistory.Visible = false;

                    //ctlLeadInfo.GetPreleadData(SLMUtil.SafeDecimal(preleadId));     //Comment by Pom 01/09/2016
                    //if (ctlLeadInfo != null) ctlLeadInfo.GetLeadData(new LeadData() {
                    //    ContractNo = data.ContractNo,
                    //    Name = data.Firstname,
                    //    LastName = data.Lastname,
                    //    TelNo_1 = data.TelNo1,
                    //    TicketId = data.TicketId,
                    //    Status = data.StatusDesc,
                    //    CampaignId = data.CampaignId,
                    //    ProductId = data.ProductId,
                    //    ContactLatestDate = data.PreContactLatestDate,
                    //    PreleadAssignDate = data.AssignedDate,
                    //    OwnerBranchName = data.OwnerBranchName,
                    //    OwnerName =  data.OwnerName

                    //});

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        private void GetCampaignList()
        {
            try
            {
                List<CampaignWSData> cList = SlmScr004Biz.GetCampaignFinalData(txtTicketID.Text.Trim());
                lbSum.Text = "<font class='hilightGreen'><b>" + cList.Count.ToString("#,##0") + "</b></font>";
                gvCampaign.DataSource = cList;
                gvCampaign.DataBind();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            RedirectToSearchPage();
        }

        private void RedirectToSearchPage()
        {
            ClearSession();

            if (Request["backtype"] == "2" || Request["backtype"] == "3")
            {
                Response.Redirect("~/SLM_SCR_029.aspx?backtype=" + Request["backtype"], false);         //ค้นหา OBT
            }
            else
            {
                Response.Redirect("~/SLM_SCR_003.aspx", false);         //ค้นหา Default
            }
        }

        protected void gvCampaign_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblCampaignDesc = (Label)e.Row.FindControl("lbCampaignDetail");
                    if (lblCampaignDesc.Text.Trim().Length > AppConstant.Campaign.DisplayCampaignDescMaxLength)
                    {
                        lblCampaignDesc.Text = lblCampaignDesc.Text.Trim().Substring(0, AppConstant.Campaign.DisplayCampaignDescMaxLength) + "...";
                        LinkButton lbShowCampaignDesc = (LinkButton)e.Row.FindControl("lbShowCampaignDesc");
                        lbShowCampaignDesc.Visible = true;
                        lbShowCampaignDesc.OnClientClick = AppUtil.GetShowCampaignDescScript(Page, lbShowCampaignDesc.CommandArgument, "004_campaign_campaigndesc_" + lbShowCampaignDesc.CommandArgument);
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

        protected void btnAllCampaign_Click(object sender, EventArgs e)
        {
            try
            {
                Session[CMTCampaignSession] = null;
                DoSearchBundleCampaign();

                rbSearchByCombo.Checked = true;
                rbSearchByText.Checked = false;
                SearchCampaignCheckChanged();
                BindComboProductGroup();
                cmbProduct.Items.Clear();
                cmbProduct.Items.Insert(0, new ListItem("", "0"));
                cmbCampaign.Items.Clear();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));
                if (txtUserLoginChannelId.Text.Trim() == "")
                {
                    lblPopupInfo.Text = "ไม่สามารถสร้างข้อมูลผู้มุ่งหวังได้ เนื่องจากไม่พบข้อมูลช่องทางของผู้ใช้งานระบบ กรุณาติดต่อ Admin เพื่อทำการกำหนดช่องทาง";
                    btnSearchCampaign.Enabled = false;
                }
                else
                {
                    lblPopupInfo.Text = "";
                    btnSearchCampaign.Enabled = true;
                }

                pcGridCampaign.SetVisible = false;
                gvAllCampaign.DataSource = null;
                gvAllCampaign.DataBind();
                gvAllCampaign.Visible = false;

                if (txtScreenHeight.Text.Trim() != "" && txtScreenWidth.Text.Trim() != "")
                {
                    if (Convert.ToInt32(txtScreenHeight.Text.Trim()) > 0 && Convert.ToInt32(txtScreenHeight.Text.Trim()) < 700)
                    {
                        pnPopupSearchCampaign.Height = new Unit(0.6 * Convert.ToDouble(txtScreenHeight.Text.Trim()), UnitType.Pixel);
                        pnPopupSearchCampaign.Width = new Unit(0.8 * Convert.ToDouble(txtScreenWidth.Text.Trim()), UnitType.Pixel);
                    }
                    else if (Convert.ToInt32(txtScreenHeight.Text.Trim()) >= 700 && Convert.ToInt32(txtScreenHeight.Text.Trim()) < 950)
                    {
                        pnPopupSearchCampaign.Height = new Unit(0.75 * Convert.ToDouble(txtScreenHeight.Text.Trim()), UnitType.Pixel);
                        pnPopupSearchCampaign.Width = new Unit(0.75 * Convert.ToDouble(txtScreenWidth.Text.Trim()), UnitType.Pixel);
                    }
                }

                upPopupSearchCampaign.Update();
                mpePopupSearchCampaign.Show();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        //========================================= Search Campaign แบบ All และ Bundle ==================================================
        #region Popup Search All Campaign

        private void BindComboProductGroup()
        {
            cmbProductGroup.DataSource = SlmScr003Biz.GetProductGroupData();
            cmbProductGroup.DataTextField = "TextField";
            cmbProductGroup.DataValueField = "ValueField";
            cmbProductGroup.DataBind();
            cmbProductGroup.Items.Insert(0, new ListItem("", ""));
        }

        protected void cmbProductGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //                cmbProduct.DataSource = SlmScr003Biz.GetProductData(cmbProductGroup.SelectedItem.Value);
                cmbProduct.DataSource = SlmScr003Biz.GetProductDataNew(cmbProductGroup.SelectedItem.Value);
                cmbProduct.DataTextField = "TextField";
                cmbProduct.DataValueField = "ValueField";
                cmbProduct.DataBind();
                cmbProduct.Items.Insert(0, new ListItem("", "0"));  //value = 0 ป้องกันในกรณีส่งค่า ช่องว่างไป where ใน CMT_CAMPAIGN_PRODUCT แล้วค่า PR_ProductId บาง record เป็นช่องว่าง

                cmbCampaign.Items.Clear();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));

                mpePopupSearchCampaign.Show();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            string campaignList = "";
            try
            {
                if (Session[CMTCampaignSession] == null)
                    GetCMTCampaign();

                //List<CmtServiceProxy.CitizenId> cmtCampaignList = (List<CmtServiceProxy.CitizenId>)Session[CMTCampaignSession];
                //foreach (CmtServiceProxy.CitizenId campaign in cmtCampaignList)
                //{
                //    campaignList += (campaignList != "" ? "," : "") + "'" + campaign.CampaignId + "'";
                //}

                List<CmtServiceProxy.CitizenIdCus> cmtCampaignList = (List<CmtServiceProxy.CitizenIdCus>)Session[CMTCampaignSession];
                foreach (CmtServiceProxy.CitizenIdCus campaign in cmtCampaignList)
                {
                    campaignList += (campaignList != "" ? "," : "") + "'" + campaign.CampaignId + "'";
                }

                //                cmbCampaign.DataSource = SlmScr004Biz.GetCampaignDataViewPage(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, campaignList);
                cmbCampaign.DataSource = SlmScr004Biz.GetCampaignDataViewPageNew(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, campaignList);
                cmbCampaign.DataTextField = "TextField";
                cmbCampaign.DataValueField = "ValueField";
                cmbCampaign.DataBind();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));
                cmbCampaign.Items.Remove(cmbCampaign.Items.FindByValue(txtCampaignId.Text.Trim())); //เอาแคมเปญหลักของหน้าออก

                mpePopupSearchCampaign.Show();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            mpePopupSearchCampaign.Hide();
        }

        protected void rbSearchByCombo_CheckedChanged(object sender, EventArgs e)
        {
            SearchCampaignCheckChanged();
            upPopupSearchCampaignInner.Update();
            //upPopupSearchCampaign.Update();
            mpePopupSearchCampaign.Show();
        }

        protected void rbSearchByText_CheckedChanged(object sender, EventArgs e)
        {
            SearchCampaignCheckChanged();
            upPopupSearchCampaignInner.Update();
            //upPopupSearchCampaign.Update();
            mpePopupSearchCampaign.Show();
        }

        private void SearchCampaignCheckChanged()
        {
            cmbProductGroup.Enabled = rbSearchByCombo.Checked;
            cmbProduct.Enabled = rbSearchByCombo.Checked;
            cmbCampaign.Enabled = rbSearchByCombo.Checked;

            txtFullSearchCampaign.Enabled = rbSearchByText.Checked;
            txtFullSearchCampaign.Text = "";
            pcGridCampaign.SetVisible = false;
            gvAllCampaign.DataSource = null;
            gvAllCampaign.DataBind();
            gvAllCampaign.Visible = false;

            if (rbSearchByText.Checked)
            {
                cmbProductGroup.SelectedIndex = -1;
                cmbProduct.Items.Clear();
                cmbProduct.Items.Insert(0, new ListItem("", "0"));
                cmbCampaign.Items.Clear();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));
            }
        }

        protected void imbSelect_Click(object sender, ImageClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSearchCampaign_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                    DoSearchCampaign();

                mpePopupSearchCampaign.Show();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidateInput()
        {
            if (rbSearchByCombo.Checked)
            {
                if (cmbProductGroup.SelectedItem.Value == "")
                {
                    AppUtil.ClientAlert(Page, "กรุณาเลือกข้อมูล กลุ่มผลิตภัณฑ์/บริการ");
                    return false;
                }
                //if (cmbProduct.SelectedItem.Value == "0")
                //{
                //    AppUtil.ClientAlert(Page, "กรุณาเลือกข้อมูล ผลิตภัณฑ์/บริการ");
                //    return false;
                //}
                //if (cmbCampaign.SelectedItem.Value == "")
                //{
                //    AppUtil.ClientAlert(Page, "กรุณาเลือกข้อมูล แคมเปญ");
                //    return false;
                //}
            }
            else
            {
                if (txtFullSearchCampaign.Text.Trim() == "")
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุคำที่ต้องการค้นหา");
                    return false;
                }
            }

            return true;
        }

        private void DoSearchCampaign()
        {
            //********* แก้ใน Method นี้ ให้ดูใน Method PageSearchChangeCampaign ด้วย *********
            try
            {
                List<ProductData> result = null;

                if (rbSearchByCombo.Checked)
                {
                    //string[] bundleCamIdList = txtBundleCampaignIdList.Text.Trim().Replace("'", "").Split(',');
                    //if (bundleCamIdList.Count(p => p == cmbCampaign.SelectedItem.Value) == 0)
                    //    result = SlmScr004Biz.SearchCampaignViewPage(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, cmbCampaign.SelectedItem.Value);
                    //else
                    //    result = new List<ProductData>();

                    //ถ้าเลือก 3 combo และ แคมเปญอยู่ในตาราง Bundle แล้ว ให้แสดง alert แจ้ง
                    if (cmbProductGroup.SelectedItem.Value != "" && cmbProduct.SelectedItem.Value != "0" && cmbCampaign.SelectedItem.Value != "")
                    {
                        if (txtBundleCampaignIdList.Text.Trim() != "")
                        {
                            string[] bundleCamIdList = txtBundleCampaignIdList.Text.Trim().Replace("'", "").Split(',');
                            if (bundleCamIdList.Count(p => p.Trim() == cmbCampaign.SelectedItem.Value) > 0)
                            {
                                result = new List<ProductData>();
                                BindGridviewAllCampaign((SLM.Application.Shared.GridviewPageController)pcGridCampaign, result.ToArray(), 0);
                                gvAllCampaign.Visible = true;
                                AppUtil.ClientAlert(Page, "อยู่ในแคมเปญร่วมแล้ว");
                                return;
                            }
                        }
                    }

                    result = SlmScr004Biz.SearchCampaignViewPageNew(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, cmbCampaign.SelectedItem.Value, txtBundleCampaignIdList.Text.Trim()); // SlmScr004Biz.SearchCampaignViewPage
                }
                else
                    result = SlmScr004Biz.SearchCampaignNew(txtFullSearchCampaign.Text.Trim(), txtBundleCampaignIdList.Text.Trim()); // SlmScr004Biz.SearchCampaign

                if (result.Count > 0)
                {
                    result = CheckCMTAllCampaign(result, rbSearchByCombo.Checked ? "combo" : "text");
                    result = FilterAccessRight(result);
                }

                BindGridviewAllCampaign((SLM.Application.Shared.GridviewPageController)pcGridCampaign, result.ToArray(), 0);
                gvAllCampaign.Visible = true;
                //upPopupSearchCampaignInner.Update();
                //upPopupSearchCampaign.Update();
            }
            catch
            {
                throw;
            }
        }

        private List<ProductData> FilterAccessRight(List<ProductData> list)
        {
            try
            {
                //ส่งข้อมุลแคมเปญและ owner ไปเช็กสิทธิในการสร้าง (คน login = owner)
                List<ProductData> notPassList = new List<ProductData>();
                foreach (ProductData data in list)
                {
                    if (!SlmScr010Biz.PassPrivilegeCampaign(SLMConstant.Branch.Active, data.CampaignId, HttpContext.Current.User.Identity.Name))
                        notPassList.Add(data);
                }

                return list.Except<ProductData>(notPassList).ToList();
            }
            catch
            {
                throw;
            }
        }

        private void DoSearchBundleCampaign()
        {
            try
            {
                List<ProductData> result = CheckCMTBundle(gvBundleCampaign, SlmScr004Biz.GetBundleProductNew(txtCampaignId.Text.Trim())); //SlmScr004Biz.GetBundleProduct
                txtBundleCampaignIdList.Text = "'" + txtCampaignId.Text.Trim() + "'";
                foreach (ProductData product in result)
                {
                    //เก็บไว้เพื่อส่งไป Not In ใน Gridview All Campaign
                    txtBundleCampaignIdList.Text += (txtBundleCampaignIdList.Text.Trim() != "" ? "," : "") + "'" + product.CampaignId + "'";
                }

                BindGridviewBundleCampaign((SLM.Application.Shared.GridviewPageController)pcGridBundleCampaign, result.ToArray(), 0);

                upPopupSearchCampaign.Update();
            }
            catch
            {
                throw;
            }
        }

        private List<ProductData> CheckCMTBundle(GridView gv, List<ProductData> productList)
        {
            try
            {
                if (Session[CMTCampaignSession] == null)
                    GetCMTCampaign();

                //List<CmtServiceProxy.CitizenId> cmtCampaignList = (List<CmtServiceProxy.CitizenId>)Session[CMTCampaignSession];

                //foreach (CmtServiceProxy.CitizenId campaign in cmtCampaignList)
                //{
                //    var product = productList.Where(p => p.CampaignId == campaign.CampaignId).FirstOrDefault();
                //    if (product != null)
                //        product.Recommend = "*";
                //}

                List<CmtServiceProxy.CitizenIdCus> cmtCampaignList = (List<CmtServiceProxy.CitizenIdCus>)Session[CMTCampaignSession];

                foreach (CmtServiceProxy.CitizenIdCus campaign in cmtCampaignList)
                {
                    var product = productList.Where(p => p.CampaignId == campaign.CampaignId).FirstOrDefault();
                    if (product != null)
                        product.Recommend = "*";
                }

                //นำแคมเปญที่ไม่ใช่ Mass และไม่ได้ถูกแนะนำจาก CMT ออกจาก list
                List<ProductData> belowTheLineList = productList.Where(p => p.CampaignType != SLM.Resource.SLMConstant.CampaignType.Mass && p.Recommend != "*").ToList();
                if (belowTheLineList.Count > 0)
                {
                    foreach (ProductData product in belowTheLineList)
                    {
                        productList.Remove(product);
                    }
                }

                productList = productList.OrderByDescending(p => p.Recommend).ToList();

                //Logic เดิม
                //if (cmtCampaignList.Count > 0)
                //{
                //    foreach (ProductData product in productList)
                //    {
                //        if (cmtCampaignList.Count(p => p.CampaignId == product.CampaignId) > 0)
                //            product.Recommend = "*";
                //    }

                //    productList = productList.OrderByDescending(p => p.Recommend).ToList();
                //}

                return productList;
            }
            catch
            {
                throw;
            }
        }

        private List<ProductData> CheckCMTAllCampaign(List<ProductData> productList, string searchFlag)
        {
            try
            {
                if (Session[CMTCampaignSession] == null)
                    GetCMTCampaign();

                //List<CmtServiceProxy.CitizenId> cmtCampaignList = (List<CmtServiceProxy.CitizenId>)Session[CMTCampaignSession];
                List<CmtServiceProxy.CitizenIdCus> cmtCampaignList = (List<CmtServiceProxy.CitizenIdCus>)Session[CMTCampaignSession];


                //List ของ campaign id ที่อยู่ในตาราง Bundle
                string[] bundleCamIdList = txtBundleCampaignIdList.Text.Trim().Replace("'", "").Split(',');

                foreach (CmtServiceProxy.CitizenIdCus campaign in cmtCampaignList)
                {
                    //campaign จาก cmt ต้องไม่อยู่ในตาราง bundle และไม่ใช่แคมเปญหลักของหน้า view
                    if (bundleCamIdList.Count(p => p.Trim() == campaign.CampaignId) == 0 && campaign.CampaignId != txtCampaignId.Text.Trim())
                    {
                        var product = productList.Where(p => p.CampaignId == campaign.CampaignId).FirstOrDefault();

                        //ถ้านำ campaign จาก cmt ไปหาใน result list(productList) ที่ได้จากการค้นหาจากฐานข้อมูล
                        //ถ้าเจอให้ใส่ *, ถ้าไม่เจอให้นำ campaign จาก cmt ไป add เพิ่มใน result list(productList) เพื่อนำไปแสดงผลบนหน้าจอ
                        if (product != null)
                            product.Recommend = "*";
                        else
                        {
                            if (searchFlag == "text")   //campaign ใน cmt add เพิ่มใน result list 
                            {
                                var data = SlmScr004Biz.GetProductCampaignDataForCmt(campaign.CampaignId);  //เอา Campaign จาก CMT ไปหาข้อมูลโดยไม่ต้องส่งใจ Mass
                                if (data.Count > 0)
                                {
                                    //นำ campaign จาก cmt ไปค้นหาข้อมูลในฐานข้อมูล 
                                    //เมื่อได้ข้อมูลมาแล้วให้เช็กด้วยว่า campaign นัั้นมี ProductGroupName, ProductName, CampaignName ที่มีคำ เหมือนคำที่อยู่ใน txtFullSearchCampaign.Text หรือไม่
                                    //ถ้ามีคำเหมือนให้ add เข้า result list(productList) เพื่อนำไปแสดงผลบนหน้าจอ
                                    if (data[0].ProductGroupName.Contains(txtFullSearchCampaign.Text.Trim()) || data[0].ProductName.Contains(txtFullSearchCampaign.Text.Trim())
                                    || data[0].CampaignName.Contains(txtFullSearchCampaign.Text.Trim()))
                                    {
                                        ProductData new_product = new ProductData()
                                        {
                                            CampaignId = campaign.CampaignId,
                                            CampaignName = data[0].CampaignName,
                                            ProductGroupId = data[0].ProductGroupId,
                                            ProductGroupName = data[0].ProductGroupName,
                                            ProductId = data[0].ProductId,
                                            ProductName = data[0].ProductName,
                                            CampaignDesc = data[0].CampaignDesc,
                                            StartDate = data[0].StartDate,
                                            EndDate = data[0].EndDate
                                        };

                                        productList.Add(new_product);
                                    }
                                }
                            }
                            else
                            {
                                //searchFlag == combo, เช็กว่าแคมเปญที่มาจาก Cmt มีค่า ProductGroupId, ProductId, CampaignId ตรงกับค่าใน Combo ที่ถูกเลือกไว้หรือไม่
                                bool isAdd = false;
                                var data = SlmScr004Biz.GetProductCampaignDataForCmt(campaign.CampaignId);  //เอา Campaign จาก CMT ไปหาข้อมูลโดยไม่ต้องส่งใจ Mass
                                if (data.Count > 0)
                                {
                                    //ถ้า Combo ถูกเลือกไว้สามตัว เช็กสามค่า
                                    if (cmbProductGroup.SelectedItem.Value != "" && cmbProduct.SelectedItem.Value != "0" && cmbCampaign.SelectedItem.Value != "")
                                    {
                                        if (cmbProductGroup.SelectedItem.Value == data[0].ProductGroupId && cmbProduct.SelectedItem.Value == data[0].ProductId && cmbCampaign.SelectedItem.Value == data[0].CampaignId)
                                            isAdd = true;
                                    }
                                    else if (cmbProductGroup.SelectedItem.Value != "" && cmbProduct.SelectedItem.Value != "0") //ถ้า Combo ถูกเลือกไว้สองตัว เช็กสองค่า
                                    {
                                        if (cmbProductGroup.SelectedItem.Value == data[0].ProductGroupId && cmbProduct.SelectedItem.Value == data[0].ProductId)
                                            isAdd = true;
                                    }
                                    else if (cmbProductGroup.SelectedItem.Value != "")  //ถ้า Combo ถูกเลือกไว้ตัวเดียว เช็กหนึ่งค่า
                                    {
                                        if (cmbProductGroup.SelectedItem.Value == data[0].ProductGroupId)
                                            isAdd = true;
                                    }

                                    if (isAdd)
                                    {
                                        ProductData new_product = new ProductData()
                                        {
                                            CampaignId = campaign.CampaignId,
                                            CampaignName = data[0].CampaignName,
                                            ProductGroupId = data[0].ProductGroupId,
                                            ProductGroupName = data[0].ProductGroupName,
                                            ProductId = data[0].ProductId,
                                            ProductName = data[0].ProductName,
                                            CampaignDesc = data[0].CampaignDesc,
                                            StartDate = data[0].StartDate,
                                            EndDate = data[0].EndDate
                                        };

                                        productList.Add(new_product);
                                    }
                                }
                            }
                        }
                    }
                }

                //นำแคมเปญที่ไม่ใช่ Mass และไม่ได้ถูกแนะนำจาก CMT ออกจาก list
                List<ProductData> belowTheLineList = productList.Where(p => p.CampaignType != SLM.Resource.SLMConstant.CampaignType.Mass && p.Recommend != "*").ToList();
                if (belowTheLineList.Count > 0)
                {
                    foreach (ProductData product in belowTheLineList)
                    {
                        productList.Remove(product);
                    }
                }

                productList = productList.OrderByDescending(p => p.Recommend).ToList();

                return productList;
            }
            catch
            {
                throw;
            }
        }

        private void GetCMTCampaign()
        {
            try
            {
                if (txtCitizenId.Text.Trim() != "" && txtUserLoginChannelId.Text.Trim() != "")
                {
                    List<CampaignWSData> LData = new List<CampaignWSData>();

                    //********************** Web Services*******************************
                    //CmtServiceProxy.CampaignByCustomerRequest request = new CmtServiceProxy.CampaignByCustomerRequest();

                    CmtServiceProxy.CampaignByCustomersRequest request = new CmtServiceProxy.CampaignByCustomersRequest();
                    CmtServiceProxy.Header2 header = new CmtServiceProxy.Header2();
                    header.transaction_date = DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd");
                    header.user_name = CMTUserName;
                    header.password = CMTPassword;
                    header.service_name = CMTServiceName;
                    header.system_code = CMTSystemCode;
                    header.reference_no = CMTReferenceNo;
                    request.header = header;

                    //CmtServiceProxy.ReqCampByIdEntity body = new CmtServiceProxy.ReqCampByIdEntity();
                    CmtServiceProxy.ReqCampByCusEntity body = new CmtServiceProxy.ReqCampByCusEntity();
                    body.CitizenId = txtCitizenId.Text.Trim();
                    body.RequestDate = DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd");
                    body.Channel = txtUserLoginChannelId.Text.Trim();
                    body.CampaignNum = CMTCampaignNo;
                    body.HasOffered = "N";
                    body.IsInterested = "N";
                    body.CustomerFlag = "AND";
                    body.Command = "CampaignByCustomer";
                    body.ProductTypeId = AppConstant.GetCMTProductType;
                    request.CampByCitizenIdBody = body;

                    CmtServiceProxy.CmtServiceClient cmt_service_client = new CmtServiceProxy.CmtServiceClient();
                    cmt_service_client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, AppConstant.CMTTimeout);

                    CmtServiceProxy.ICmtService service = cmt_service_client;
                    //CmtServiceProxy.CampaignByCustomerResponse response = service.CampaignByCustomer(request);
                    CmtServiceProxy.CampaignByCustomersResponse response = service.CampaignByCustomers(request);

                    if (response.status.status == "SUCCESS")
                    {
                        Session[CMTCampaignSession] = response.detail.CitizenIds.ToList();
                    }
                    else
                    {
                        //Session[CMTCampaignSession] = new List<CmtServiceProxy.CitizenId>();
                        Session[CMTCampaignSession] = new List<CmtServiceProxy.CitizenIdCus>();

                        if (response.status.error_code != "E008")
                            _log.Error("Call CMT: " + response.status.error_code + " : " + response.status.description);
                    }
                }
                else
                {
                    Session[CMTCampaignSession] = new List<CmtServiceProxy.CitizenIdCus>();
                    //Session[CMTCampaignSession] = new List<CmtServiceProxy.CitizenId>();
                }
            }
            catch (Exception ex)
            {
                //Session[CMTCampaignSession] = new List<CmtServiceProxy.CitizenId>();
                Session[CMTCampaignSession] = new List<CmtServiceProxy.CitizenIdCus>();
                _log.Error(ex.Message);
            }
        }

        #endregion

        #region Page Control Popup SearchCampaign

        private void BindGridviewAllCampaign(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvAllCampaign);
            pageControl.Update(items, pageIndex, 5);
            upPopupSearchCampaignInner.Update();
            //upPopupSearchCampaign.Update();
            mpePopupSearchCampaign.Show();
        }

        protected void PageSearchChangeCampaign(object sender, EventArgs e)
        {
            try
            {
                List<ProductData> result = null;

                if (rbSearchByCombo.Checked)
                {
                    //string[] bundleCamIdList = txtBundleCampaignIdList.Text.Trim().Replace("'", "").Split(',');
                    //if (bundleCamIdList.Count(p => p == cmbCampaign.SelectedItem.Value) == 0)
                    //    result = SlmScr004Biz.SearchCampaignViewPage(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, cmbCampaign.SelectedItem.Value);
                    //else
                    //    result = new List<ProductData>();

                    result = SlmScr004Biz.SearchCampaignViewPageNew(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, cmbCampaign.SelectedItem.Value, txtBundleCampaignIdList.Text.Trim()); //SlmScr004Biz.SearchCampaignViewPage
                }
                else
                    result = SlmScr004Biz.SearchCampaignNew(txtFullSearchCampaign.Text.Trim(), txtBundleCampaignIdList.Text.Trim()); // SlmScr004Biz.SearchCampaign

                if (result.Count > 0)
                {
                    result = CheckCMTAllCampaign(result, rbSearchByCombo.Checked ? "combo" : "text");
                    result = FilterAccessRight(result);
                }

                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridviewAllCampaign(pageControl, result.ToArray(), pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        #region Page Control Bundle Campaign

        private void BindGridviewBundleCampaign(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvBundleCampaign);
            pageControl.Update(items, pageIndex, 5);
            upPopupSearchCampaign.Update();
            mpePopupSearchCampaign.Show();
        }

        protected void PageSearchChangeBundleCampaign(object sender, EventArgs e)
        {
            try
            {
                List<ProductData> result = CheckCMTBundle(gvBundleCampaign, SlmScr004Biz.GetBundleProductNew(txtCampaignId.Text.Trim()));//SlmScr004Biz.GetBundleProduct(txtCampaignId.Text.Trim())
                txtBundleCampaignIdList.Text = "'" + txtCampaignId.Text.Trim() + "'";
                foreach (ProductData product in result)
                {
                    //เก็บไว้เพื่อส่งไป Not In ใน Gridview All Campaign
                    txtBundleCampaignIdList.Text += (txtBundleCampaignIdList.Text.Trim() != "" ? "," : "") + "'" + product.CampaignId + "'";
                }
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridviewBundleCampaign(pageControl, result.ToArray(), pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void gvBundleCampaign_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((Label)e.Row.FindControl("lblCmt")).Text.Trim() == "*")
                    e.Row.ForeColor = System.Drawing.Color.Red;

                Label lblCampaignDesc = (Label)e.Row.FindControl("lblCampaignDesc");
                if (lblCampaignDesc.Text.Trim().Length > AppConstant.Campaign.DisplayCampaignDescMaxLength)
                {
                    lblCampaignDesc.Text = lblCampaignDesc.Text.Trim().Substring(0, AppConstant.Campaign.DisplayCampaignDescMaxLength) + "...";
                    LinkButton lbShowCampaignDesc = (LinkButton)e.Row.FindControl("lbShowCampaignDesc");
                    lbShowCampaignDesc.Visible = true;
                    lbShowCampaignDesc.OnClientClick = AppUtil.GetShowCampaignDescScript(Page, lbShowCampaignDesc.CommandArgument, "004_bundle_campaigndesc_" + lbShowCampaignDesc.CommandArgument);
                }
            }
        }

        protected void gvAllCampaign_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((Label)e.Row.FindControl("lblCmt")).Text.Trim() == "*")
                    e.Row.ForeColor = System.Drawing.Color.Red;

                Label lblCampaignDesc = (Label)e.Row.FindControl("lblCampaignDesc");
                if (lblCampaignDesc.Text.Trim().Length > AppConstant.Campaign.DisplayCampaignDescMaxLength)
                {
                    lblCampaignDesc.Text = lblCampaignDesc.Text.Trim().Substring(0, AppConstant.Campaign.DisplayCampaignDescMaxLength) + "...";
                    LinkButton lbShowCampaignDesc = (LinkButton)e.Row.FindControl("lbShowCampaignDesc");
                    lbShowCampaignDesc.Visible = true;
                    lbShowCampaignDesc.OnClientClick = AppUtil.GetShowCampaignDescScript(Page, lbShowCampaignDesc.CommandArgument, "004_all_campaigndesc_" + lbShowCampaignDesc.CommandArgument);
                }
            }
        }

        protected void btnSelectCampaign_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateData())
                {
                    AppUtil.ClientAlert(Page, "กรุณาเลือกอย่างน้อย 1 แคมเปญ");
                    mpePopupSearchCampaign.Show();
                    return;
                }

                List<string> selectedCmtCampaignIdList = GetSelectedCmtCampaignId();
                if (selectedCmtCampaignIdList.Count == 0)
                {
                    DoInsert();
                }
                else
                {
                    CmtServiceProxy.UpdateCustomerFlagsResponse response = UpdateToCmtService(selectedCmtCampaignIdList);
                    if (response.status.status == "SUCCESS")
                    {
                        DoInsert();
                    }
                    else
                    {
                        AppUtil.ClientAlert(Page, response.status.error_code + " : " + response.status.description);
                        mpePopupSearchCampaign.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private List<string> GetSelectedCmtCampaignId()
        {
            List<string> list = new List<string>();
            foreach (GridViewRow row in gvBundleCampaign.Rows)
            {
                if (((CheckBox)row.FindControl("cbSelectCampaign")).Checked && ((Label)row.FindControl("lblCmt")).Text.Trim() == "*")
                    list.Add(((Label)row.FindControl("lblCampaignId")).Text.Trim());
            }

            foreach (GridViewRow row in gvAllCampaign.Rows)
            {
                if (((CheckBox)row.FindControl("cbSelectCampaign")).Checked && ((Label)row.FindControl("lblCmt")).Text.Trim() == "*")
                    list.Add(((Label)row.FindControl("lblCampaignId")).Text.Trim());
            }
            return list;
        }

        private bool ValidateData()
        {
            foreach (GridViewRow row in gvBundleCampaign.Rows)
            {
                if (((CheckBox)row.FindControl("cbSelectCampaign")).Checked)
                    return true;
            }
            foreach (GridViewRow row in gvAllCampaign.Rows)
            {
                if (((CheckBox)row.FindControl("cbSelectCampaign")).Checked)
                    return true;
            }
            return false;
        }

        private void DoInsert()
        {
            try
            {
                //Insert Bundle
                List<ProductData> campList = GetCampaignData(gvBundleCampaign);
                if (campList.Count > 0)
                    SlmScr004Biz.InsertCampaginFinalList(campList, txtTicketID.Text.Trim(), HttpContext.Current.User.Identity.Name);

                //Insert New Lead
                campList = GetCampaignData(gvAllCampaign);
                if (campList.Count > 0)
                {
                    LeadInfoBiz biz = new LeadInfoBiz();
                    List<SaveResultData> resultList = biz.InsertNewLeads(txtTicketID.Text.Trim(), campList, HttpContext.Current.User.Identity.Name, txtLoginNameTH.Text.Trim(), txtUserLoginChannelId.Text.Trim(), txtUserLoginChannelDesc.Text.Trim());

                    rptPopupSaveResult.DataSource = resultList;
                    rptPopupSaveResult.DataBind();
                    upPopupSaveResult.Update();
                    mpePopupSaveResult.Show();

                    if (biz.ErrorList.Count > 0)
                    {
                        //ลง error log
                        _log.Error(" ");
                        _log.Error("Page: View Lead, Process: Create Lead by Campaign");
                        foreach (KeyValuePair<string, string> pair in biz.ErrorList)
                        {
                            _log.Error("CampaignId: " + pair.Key + ", Error: " + pair.Value);
                        }
                    }
                }
                else
                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                GetCampaignList();
                upHistory.Update();
                mpePopupSearchCampaign.Hide();
            }
            catch
            {
                throw;
            }
        }

        protected void btnPopupSaveResultOK_Click(object sender, EventArgs e)
        {
            rptPopupSaveResult.DataSource = null;
            rptPopupSaveResult.DataBind();
            mpePopupSaveResult.Hide();
        }

        private List<ProductData> GetCampaignData(GridView gridview)
        {
            try
            {
                List<ProductData> camplist = new List<ProductData>();

                foreach (GridViewRow row in gridview.Rows)
                {
                    if (((CheckBox)row.FindControl("cbSelectCampaign")).Checked)
                    {
                        if (((Label)row.FindControl("lblCampaignDesc")).Text.Trim().Length > AppConstant.TextMaxLength)
                            throw new Exception("ไม่สามารถบันทึกรายละเอียดแคมเปญเกิน " + AppConstant.TextMaxLength.ToString() + " ตัวอักษรได้\\r\\nรบกวนติดต่อผู้ดูแลระบบ CMT เพื่อแก้ไขรายละเอียด");

                        ProductData data = new ProductData()
                        {
                            ProductGroupId = ((Label)row.FindControl("lblProductGroupId")).Text.Trim(),
                            ProductId = ((Label)row.FindControl("lblProductId")).Text.Trim(),
                            ProductName = ((Label)row.FindControl("lblProductName")).Text.Trim(),
                            CampaignId = ((Label)row.FindControl("lblCampaignId")).Text.Trim(),
                            CampaignName = ((Label)row.FindControl("lblCampaignName")).Text.Trim(),
                            CampaignDesc = ((Label)row.FindControl("lblCampaignDesc")).Text.Trim()
                        };
                        camplist.Add(data);
                    }
                }

                return camplist;
            }
            catch
            {
                throw;
            }
        }

        private CmtServiceProxy.UpdateCustomerFlagsResponse UpdateToCmtService(List<string> selectedCmtCampaignIdList)
        {
            try
            {
                List<CmtServiceProxy.UpdInquiry> inquiries = new List<CmtServiceProxy.UpdInquiry>();

                foreach (string campaignId in selectedCmtCampaignIdList)
                {
                    CmtServiceProxy.UpdInquiry inq = new CmtServiceProxy.UpdInquiry();
                    inq.CampaignId = campaignId;
                    inq.IsInterested = "Y";
                    inq.HasOffered = "Y";
                    inq.UpdatedBy = HttpContext.Current.User.Identity.Name;
                    inq.Command = "UpdateCustomerFlags";

                    inquiries.Add(inq);
                }

                //********************** Web Services*******************************
                CmtServiceProxy.UpdateCustomerFlagsRequest request = new CmtServiceProxy.UpdateCustomerFlagsRequest();

                CmtServiceProxy.Header header = new CmtServiceProxy.Header();
                header.transaction_date = DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd");
                header.user_name = CMTUserName;
                header.password = CMTPassword;
                header.service_name = CMTServiceName;
                header.system_code = CMTSystemCode;
                header.reference_no = CMTReferenceNo;
                request.header = header;

                CmtServiceProxy.ReqUpdFlagEntity body = new CmtServiceProxy.ReqUpdFlagEntity();
                body.CitizenId = txtCitizenId.Text.Trim();
                body.UpdInquiries = inquiries.ToArray();

                request.UpdateCustFlag = body;

                CmtServiceProxy.CmtServiceClient cmt_service_client = new CmtServiceProxy.CmtServiceClient();
                cmt_service_client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, AppConstant.CMTTimeout);

                CmtServiceProxy.ICmtService service = cmt_service_client;
                CmtServiceProxy.UpdateCustomerFlagsResponse response = service.UpdateCustomerFlags(request);
                return response;
            }
            catch (Exception ex)
            {
                _log.Error(" ");
                _log.Error("Page: View Lead, Process: Update data back to cmt service");
                _log.Error("Error: " + ex.InnerException != null ? ex.InnerException.Message : ex.Message);

                throw ex;
            }
        }

        protected void imbCopyLead_Click(object sender, ImageClickEventArgs e)
        {
            RedirectToCreateLead();
        }

        private void RedirectToCreateLead()
        {
            Session.Remove(SLMConstant.SessionName.tabscreenlist);
            Session.Remove(SLMConstant.SessionName.configscreen);
            ClearSession();
            Session["ticket_id"] = txtTicketID.Text.Trim();
            Response.Redirect("~/SLM_SCR_010.aspx");
        }

        protected void lbTicketIdReferSection_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnTicketIdReferSection.Visible)
                {
                    lbTicketIdReferSection.Text = "[+] <b>Ticket ID Refer/Relate</b>";
                    pnTicketIdReferSection.Visible = false;
                }
                else
                {
                    lbTicketIdReferSection.Text = "[-] <b>Ticket ID Refer/Relate</b>";
                    if (!cbTicketReferSectionLoad.Checked)
                    {
                        cbTicketReferSectionLoad.Checked = true;
                        GetTicketIdReferAndRelate();
                    }                                 
                    pnTicketIdReferSection.Visible = true;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void GetTicketIdReferAndRelate()
        {
            gvTicketRefer.DataSource = SlmScr004Biz.GetTicketReferDetail(txtTicketIDRefer.Text.Trim());
            gvTicketRelate.DataSource = SlmScr004Biz.GetTicketRelateDetail(txtTicketID.Text.Trim());
            gvTicketRefer.DataBind();
            gvTicketRelate.DataBind();
            upTicketReferRelate.Update();
            if (gvTicketRelate.Rows.Count > 5)
            {
                divTicketRelate.Attributes.Add("style", "height:150px; overflow-y:auto;");
            }
            else
            {
                divTicketRelate.Attributes.Remove("style");
            }
        }


    }

    //Added By Pom 07/04/2016
    public class TabHeaderTemplate : ITemplate
    {
        string _headerText = "";
        string _tabCode = "";
        public string HeaderText
        {
            get { return _headerText; }
        }
        public string TabCode
        {
            get { return _tabCode; }
        }

        public TabHeaderTemplate(string headerText, string tabCode)
        {
            _headerText = headerText;
            _tabCode = tabCode;
        }

        public void InstantiateIn(Control container)
        {
            Label lbHeader = new Label();
            lbHeader.Text = _headerText;
            lbHeader.Attributes.Add("onclick", "TabClick('" + _tabCode + "');");
            lbHeader.CssClass = "tabHeaderText";
            container.Controls.Add(lbHeader);
        }
    }
}