using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SLM.Application.Utilities;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Biz;
using log4net;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Configuration;
using System.Drawing;
using AjaxControlToolkit;
using ToEcm = Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint;
using System.IO;
using System.Net;
using SLM.Application.Services;


namespace SLM.Application.Shared.Obt
{
    public partial class TabActRenewInsure : System.Web.UI.UserControl
    {
        public string SessionPrefix
        {
            get
            {
                var frm004 = this.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent as SLM_SCR_004;
                if (frm004 != null)
                {
                    return frm004.SessionPrefix;
                }
                else
                {
                    return "";
                }
            }
        }

        private bool useWebservice = Convert.ToBoolean(ConfigurationManager.AppSettings["UseWebservice"]);
        public delegate void UpdatedDataEvent(string statusDesc, DateTime contactLatestDate);
        public event UpdatedDataEvent UpdatedDataChanged;
        private static readonly ILog _log = LogManager.GetLogger(typeof(TabActRenewInsure));
        //public event EventHandler UpdateLeadDataTab;

        List<SearchPromotionResult> listPromotion = new List<SearchPromotionResult>();

        public string TicketId { get; set; }
        public string PreLeadId { get; set; }
        public string Type { get; set; }
        public bool PermissionDiscount = true;
        public decimal constVat = AppUtil.SafeInt(ConfigurationManager.AppSettings["VatRate"]);//7
        public decimal constTax = AppUtil.SafeDecimal(ConfigurationManager.AppSettings["TaxRate"]); //0.4m
        public decimal constActPlus = AppUtil.SafeDecimal(ConfigurationManager.AppSettings["ActPlus"]); //0.4m
        public int constDaysOfYear = AppUtil.SafeInt(ConfigurationManager.AppSettings["DaysOfYear"]); //365
        private List<tabControlData> dynamicTab;
        public decimal? SettleClaimReportId { get; set; }

        //Added by Pom 20/05/2016
        public delegate List<PremiumData> SavedDataEvent(string ticketId);
        public event SavedDataEvent SavedDataChanged;
        public delegate void GridviewDataEvent(string ticketId);
        public event GridviewDataEvent GridviewPremiumDataChanged;
        public delegate void UpdatedMainDataEvent(string ticketId, string ownerBranchName, string ownerLeadName, string delegateBranchName, string delegateLeadName, string telNo1, string statusDesc, DateTime contactLatestDate, bool doUpdateOwnerLogging, string externalSubStatusDesc, string cardTypeId, string citizenId, string subStatusCode, DateTime? nextContactDate);
        public event UpdatedMainDataEvent UpdatedMainDataChanged;
        public delegate void GetOBTMainDataEvent(out string contractNo, out string ticketId, out string firstname, out string lastname, out string cardTypeId, out string citizenId, out string campaignId, out string campaignName, out string productId, out string productName, out string productGroupId, out string productGroupName, out string ownerBranchCode, out string ownerBranchName, out string ownerUsername, out string ownerName, out string ownerEmpCode
            , out string delegateBranchCode, out string delegateBranchName, out string delegateUsername, out string delegateName, out string telNo1, out string statusCode, out string statusDesc, out string subStatusCode, out string subStatusDesc, out string channelId, out DateTime? nextContactDate, out string carLicenseNo);
        public event GetOBTMainDataEvent GetOBTMainData;
        private const string kiatnakinBank = "ธนาคารเกียรตินาคิน";
        private bool? PolicyCheckPaid { get; set; }  //ใช้เช็กว่าจ่ายเงินครบหรือยัง
        private bool? ActCheckPaid { get; set; }     //ใช้เช็กว่าจ่ายเงินครบหรือยัง
        private bool? IsOwnerOrDelegate { get; set; }
        private bool? IsSupervisor { get; set; }
        private bool? PolicyPurchasedFlag { get; set; }
        private bool? ActPurchasedFlag { get; set; }
        private bool? ExportNotifyPolicyReport { get; set; }
        private bool? ExportNotifyActReport { get; set; }
        private bool? CheckProblem { get; set; }
        private StaffData CurrentStaff { get; set; }

        public int GetNumOfTabs()
        {
            return tabRenewInsuranceContainer.Tabs.Count;
        }

        public string GetTicketIdActiveTab()
        {
            try
            {
                string ticketId = "";

                if (tabRenewInsuranceContainer.ActiveTab != null)
                {
                    ticketId = tabRenewInsuranceContainer.ActiveTab.ID;
                }
                else
                {
                    ticketId = tabRenewInsuranceContainer.Tabs[0].ID;
                }

                return ticketId;
            }
            catch
            {
                throw;
            }
        }

        public string GetPreleadIdActiveTab()
        {
            try
            {
                string preleadId = string.Empty;

                if (tabRenewInsuranceContainer.ActiveTab != null)
                {
                    preleadId = tabRenewInsuranceContainer.ActiveTab.ID;
                }
                else
                {
                    preleadId = tabRenewInsuranceContainer.Tabs[0].ID;
                }

                return preleadId;

                //PreleadCompareDataCollection preleaddatacollectionsave = new PreleadCompareDataCollection();
                //PreleadCompareDataCollectionGroup pregroup = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                //string ActiveTab_ID;
                //if (tabRenewInsuranceContainer.ActiveTab != null)
                //{
                //    ActiveTab_ID = tabRenewInsuranceContainer.ActiveTab.ID;
                //}
                //else
                //{
                //    ActiveTab_ID = tabRenewInsuranceContainer.Tabs[0].ID;
                //}


                //if (pregroup.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                //{
                //    preleadId = pregroup.PreleadCompareDataCollectionMain.Prelead.slm_Prelead_Id;
                //}
                //else
                //{
                //    foreach (PreleadCompareDataCollection pc in pregroup.PreleadCompareDataCollections)
                //    {
                //        if (pc.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                //        {
                //            preleadId = pc.Prelead.slm_Prelead_Id;
                //            break;
                //        }
                //    }
                //}

                //return (preleadId != null && preleadId != 0) ? preleadId.Value.ToString() : "";
            }
            catch
            {
                throw;
            }
        }

        private void SavePremium(string ticketId)
        {
            try
            {
                if (SavedDataChanged != null)
                {
                    List<PremiumData> list = SavedDataChanged(ticketId);
                    if (list.Count > 0)
                    {
                        new ConfigProductPremiumBiz().InsertPremiumTransaction(list, HttpContext.Current.User.Identity.Name);

                        if (GridviewPremiumDataChanged != null)
                        {
                            GridviewPremiumDataChanged(ticketId);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateStatusDesc(string statusDesc, DateTime contactLatestDate)
        {
            try
            {
                if (UpdatedDataChanged != null) UpdatedDataChanged(statusDesc, contactLatestDate);  //refresh ข้อมูล
            }
            catch
            {
                throw;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //storing the dynamic tab IDs in ViewState to recreate them next page postback

            Session[SessionPrefix+"dynamicTab"] = dynamicTab;
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            try
            {
                AppUtil.SetIntTextBox(txtTelNo1_PreleadPopup);
                AppUtil.SetIntTextBox(txtTelNoSms_PreleadPopup);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify1);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify2);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify3);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify4);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify5);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify6);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify7);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify8);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify9);
                AppUtil.SetMoneyTextBox(txtBPPopup_PolicyAmountSpecify10);
                AppUtil.SetMoneyTextBox(txtBPPopup_ActAmountSpecify);


                lblVoluntary_Policy_Eff_Date_cur.OnTextChanged += new EventHandler(lblVoluntary_Policy_Eff_Date_cur_OnTextChanged);
                lblVoluntary_Policy_Eff_Date_pro1.OnTextChanged += new EventHandler(lblVoluntary_Policy_Eff_Date_pro1_OnTextChanged);
                lblVoluntary_Policy_Eff_Date_pro2.OnTextChanged += new EventHandler(lblVoluntary_Policy_Eff_Date_pro2_OnTextChanged);
                lblVoluntary_Policy_Eff_Date_pro3.OnTextChanged += new EventHandler(lblVoluntary_Policy_Eff_Date_pro3_OnTextChanged);

                txtActStartCoverDateAct_pro1.OnTextChanged += new EventHandler(txtActStartCoverDateAct_pro1_OnTextChanged);
                txtActStartCoverDateAct_pro2.OnTextChanged += new EventHandler(txtActStartCoverDateAct_pro2_OnTextChanged);
                txtActStartCoverDateAct_pro3.OnTextChanged += new EventHandler(txtActStartCoverDateAct_pro3_OnTextChanged);
                //txtActStartCoverDateAct_bla.OnTextChanged += new EventHandler(txtActStartCoverDateAct_bla_OnTextChanged);

                txtActEndCoverDateAct_pro1.OnTextChanged += new EventHandler(txtActEndCoverDateAct_pro1_OnTextChanged);
                txtActEndCoverDateAct_pro2.OnTextChanged += new EventHandler(txtActEndCoverDateAct_pro2_OnTextChanged);
                txtActEndCoverDateAct_pro3.OnTextChanged += new EventHandler(txtActEndCoverDateAct_pro3_OnTextChanged);
                //txtActEndCoverDateAct_bla.OnTextChanged += new EventHandler(txtActEndCoverDateAct_bla_OnTextChanged);

                ucActRenewInsureContact.UpdatedMainDataChanged += UpdateMainData;
                ucActRenewInsureContact.UpdatePhoneCallListChanged += RefreshPhoneCallList;
                //ucActRenewInsureContact.SaveComplete += new EventHandler(ucActRenewInsureContact_OnSaveComplete);

            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                _log.Debug(ex);
                //ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "hideControl", "pageLoad();", true);
                AppUtil.ClientAlertTabRenew(this, message);
            }

        }

        private void SessionExpired()
        {
            if (Request["backtype"] == "2" || Request["backtype"] == "3")
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "redirect",
                   "alert('Session Expired'); window.location='" + ResolveUrl("~/SLM_SCR_029.aspx?backtype=" + Request["backtype"]) + "';", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "redirect",
                    "alert('Session Expired'); window.location='" + ResolveUrl("~/SLM_SCR_003.aspx") + "';", true);

            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            bool doLog = true;
            try
            {
                if (IsPostBack)
                {
                    if (Session[SessionPrefix+"dynamicTab"] != null)
                    {
                        dynamicTab = (List<tabControlData>)Session[SessionPrefix+"dynamicTab"];

                        foreach (tabControlData tabID in dynamicTab)
                        {
                            TabPanel tab = new AjaxControlToolkit.TabPanel();

                            //Setting the ID property of the TabPanel
                            tab.ID = tabID.ID;

                            //setting the TabPanel's HeaderText
                            tab.HeaderText = tabID.HeaderText;
                            tab.Attributes.Add("onclick", "DisplayProcessing();");
                            tabRenewInsuranceContainer.Tabs.Add(tab);
                        }
                    }
                    else
                    {
                        dynamicTab = new List<tabControlData>();

                    }
                }
                else
                {
                    dynamicTab = new List<tabControlData>();
                }

                PreLeadId = Request["preleadid"] != null ? Request["preleadid"] : "";       //61908
                TicketId = Request["ticketid"] != null ? Request["ticketid"] : "";          //160083566900---160083554761---160083566900---160083568065 

                if (!string.IsNullOrEmpty(Request["type"]))
                {
                    Type = Request["type"];
                }
                else
                {
                    Type = new ConfigProductScreenBiz().GetFieldType(TicketId, SLMConstant.ConfigProductScreen.ActionType.View);
                }

                if (!IsPostBack)
                {
                    _log.Debug("");
                    _log.Debug($"Method=Page_Load TabRenewInsurance.ascx, {(TicketId != null && TicketId != "" ? "TicketId=" + TicketId : ("PreleadId=" + PreLeadId))}, Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                    Session[SessionPrefix+"dynamicTab"] = new List<tabControlData>();
                    AppUtil.SetNumbericCharacter(txtOD);
                    AppUtil.SetNumbericCharacter(txtODRanking);
                    AppUtil.SetNumbericCharacter(txtFT);
                    AppUtil.SetNumbericCharacter(txtFTRanking);

                    LeadData leadData = new LeadData();
                    leadData.TicketId = TicketId;

                    if (PreLeadId != "" || TicketId != "")
                    {
                        if (PreLeadId != "")
                        {

                            tabPayment.Visible = false;
                            pnIncentive.Visible = false;
                            divPayment.Visible = false;
                            divProblem.Visible = false;
                            pnClaim.Visible = false;
                        }
                        else if (TicketId != "")
                        {

                            tabPayment.Visible = true;
                            pnIncentive.Visible = true;
                            // divPayment.Visible = true;
                            pnClaim.Visible = true;
                            divProblem.Visible = true;
                        }
                        InitialControl(leadData);
                    }

                    _log.Debug($"Method=Page_Load TabRenewInsurance.ascx, {(TicketId != null && TicketId != "" ? "TicketId=" + TicketId : ("PreleadId=" + PreLeadId))}, End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                }
                else
                {
                    PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                    if (pg == null)
                    {
                        SessionExpired();
                    }
                }
            }
            catch (Exception ex)
            {
                if (doLog)
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _log.Error(message);
                    _log.Debug(ex);
                    AppUtil.ClientAlertTabRenew(this, message);
                }
            }
        }


        //==========================================================================================================
        //Prelead Section
        //==========================================================================================================

        public void HideButtons()
        {
            divLeadAddContact.Visible = false;
            divPreleadAddContact.Visible = false;
            HideDebugControl();
        }

        public void InitialControlPrelead(PreleadViewData data, string loginName, bool updatepopup = false)
        {
            RenewInsureBiz biz = null;
            try
            {
                Session.Remove(SessionPrefix+SLMConstant.SessionName.renewinsure_phonecall_list);
                divLeadAddContact.Visible = false;
                divPreleadAddContact.Visible = true;
                pnIncentive.Visible = false;    //เป็น Prelead ยังไม่เปิด section การขาย

                txtPreleadIdSearch.Text = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
                txtTicketIdSearch.Text = data.TicketId;
                txtLoginName.Text = loginName;

                // added by zz 2017-03-30
                if (updatepopup)
                {
                    txtContractNo_PreleadPopup.Text = data.ContractNo;
                    txtCitizenId_PreleadPopup.Text = data.CitizenId;
                    txtTelNo1_PreleadPopup.Text = data.TelNo1;
                    txtTelNoSms_PreleadPopup.Text = string.IsNullOrWhiteSpace(data.TelNoSms) ? data.TelNo1 : data.TelNoSms;
                    txtFirstname_PreleadPopup.Text = data.Firstname;
                    txtLastname_PreleadPopup.Text = data.Lastname;
                    txtProductId_PreleadPopup.Text = data.ProductId;
                    txtProductName_PreleadPopup.Text = data.ProductName;
                    txtProductGroupId_PreleadPopup.Text = data.ProductGroupId;
                    txtProductGroupName_PreleadPopup.Text = data.ProductGroupName;
                    txtCarLicenseNo_PreleadPopup.Text = data.CarLicenseNo;
                    txtTicketId_PreleadPopup.Text = data.TicketId;
                    txtCampaignId_PreleadPopup.Text = data.CampaignId;
                    txtCampaignName_PreleadPopup.Text = data.CampaignName;
                    txtOwnerBranchName_PreleadPopup.Text = data.OwnerBranchName;
                    txtOwnerName_PreleadPopup.Text = data.OwnerName;
                    AppUtil.SetComboValue(cmbCardType_PreleadPopup, data.CardTypeId.ToString());
                    AppUtil.SetComboValue(cmbSubStatus_PreleadPopup, data.SubStatusCode);
                    tdmNextContactDate_PreleadPopup.DateValue = data.NextContactDate ?? new DateTime();
                    txtStatusCode_PreleadPopup.Text = data.StatusCode;
                    txtStatusDesc_PreleadPopup.Text = data.StatusDesc;
                    txtSubStatusCodeCurrent_PreleadPopup.Text = data.SubStatusCode;
                    txtSubStatusNameCurrent_PreleadPopup.Text = data.SubStatusDesc;
                }

                if (!string.IsNullOrEmpty(data.TicketId))
                {
                    btnAddContractPrelead.Visible = false;
                }
                pcTop.SetVisible = false;

                biz = new RenewInsureBiz();
                cbThisLead.Checked = true;
                DoBindGridview(biz, 0);
                HideDebugControl();     //ซ่อน Control สำหรับ Debug
                lbCasAllActivity.OnClientClick = new Services.CARService().GetCallCASScript((data.TicketId != null ? data.TicketId : ""), data.PreleadId, (data.SubScriptionTypeId != null ? data.SubScriptionTypeId.Value.ToString() : "0"), data.CitizenId, HttpContext.Current.User.Identity.Name);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        private void InitialDropdownlist(PreleadPhoneCallBiz biz, string productId, string campaignId, string statusCode)
        {
            try
            {
                //ประเภทบุคคล
                if (cmbCardType_PreleadPopup.Items.Count <= 1)
                {
                    cmbCardType_PreleadPopup.DataSource = biz.GetCardTypeList();
                    cmbCardType_PreleadPopup.DataTextField = "TextField";
                    cmbCardType_PreleadPopup.DataValueField = "ValueField";
                    cmbCardType_PreleadPopup.DataBind();
                    cmbCardType_PreleadPopup.Items.Insert(0, new ListItem("", ""));
                }

                if (cmbSubStatus_PreleadPopup.Items.Count <= 1)
                {
                    cmbSubStatus_PreleadPopup.DataSource = biz.GetNewSubStatusList(productId, campaignId, statusCode);
                    cmbSubStatus_PreleadPopup.DataTextField = "TextField";
                    cmbSubStatus_PreleadPopup.DataValueField = "ValueField";
                    cmbSubStatus_PreleadPopup.DataBind();
                    cmbSubStatus_PreleadPopup.Items.Insert(0, new ListItem("", ""));
                }
                
            }
            catch
            {
                throw;
            }
        }

        #region Backup 14/09/2016
        //protected void btnAddContractPrelead_Click(object sender, EventArgs e)
        //{
        //    PreleadPhoneCallBiz biz = null;
        //    try
        //    {
        //        if (txtPreleadIdSearch.Text.Trim() == "")
        //        {
        //            //ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "hideControl", "pageLoad();", true);
        //            AppUtil.ClientAlertTabRenew(Page, "ไม่พบข้อมูล PreleadId เพื่อใช้ในการบันทึก");
        //            return;
        //        }

        //        biz = new PreleadPhoneCallBiz();
        //        PreleadViewData data = biz.GetPreleadData(decimal.Parse(txtPreleadIdSearch.Text.Trim()), "", "", "");
        //        if (data != null)
        //        {
        //            InitialDropdownlist(biz, data);

        //            txtContractNo_PreleadPopup.Text = data.ContractNo;
        //            txtFirstname_PreleadPopup.Text = data.Firstname;
        //            txtLastname_PreleadPopup.Text = data.Lastname;

        //            //Hidden
        //            txtCampaignId_PreleadPopup.Text = data.CampaignId;
        //            txtProductId_PreleadPopup.Text = data.ProductId;
        //            txtProductName_PreleadPopup.Text = data.ProductName;
        //            txtProductGroupId_PreleadPopup.Text = data.ProductGroupId;
        //            txtProductGroupName_PreleadPopup.Text = data.ProductGroupName;

        //            if (data.CardTypeId != null)
        //            {
        //                cmbCardType_PreleadPopup.SelectedIndex = cmbCardType_PreleadPopup.Items.IndexOf(cmbCardType_PreleadPopup.Items.FindByValue(data.CardTypeId.Value.ToString()));
        //                lblCitizenId_PreleadPopup.Text = "*";
        //                txtCitizenId_PreleadPopup.Enabled = true;
        //                txtCitizenId_PreleadPopup.Text = data.CitizenId;
        //                AppUtil.SetCardTypeValidation(cmbCardType_PreleadPopup.SelectedItem.Value, txtCitizenId_PreleadPopup);
        //            }
        //            else
        //                txtCitizenId_PreleadPopup.Text = data.CitizenId;

        //            txtCampaignName_PreleadPopup.Text = data.CampaignName;
        //            txtOwnerBranchName_PreleadPopup.Text = data.OwnerBranchName;
        //            txtOwnerName_PreleadPopup.Text = data.OwnerName;
        //            txtTelNo1_PreleadPopup.Text = data.TelNo1;
        //            tdmNextContactDate_PreleadPopup.DateValue = data.NextContactDate != null ? data.NextContactDate.Value : new DateTime();
        //            txtStatusCode_PreleadPopup.Text = data.StatusCode;
        //            txtStatusDesc_PreleadPopup.Text = data.StatusDesc;
        //            txtSubStatusCodeCurrent_PreleadPopup.Text = data.SubStatusCode;
        //            txtSubStatusNameCurrent_PreleadPopup.Text = data.SubStatusDesc;

        //            if (data.SubStatusCode != null)
        //                cmbSubStatus_PreleadPopup.SelectedIndex = cmbSubStatus_PreleadPopup.Items.IndexOf(cmbSubStatus_PreleadPopup.Items.FindByValue(data.SubStatusCode));

        //            cbBlacklist.Enabled = biz.checkBlackList(HttpContext.Current.User.Identity.Name);
        //            string blacklistId = biz.GetBlacklistId(data.ProductId, data.CitizenId);
        //            if (!string.IsNullOrEmpty(blacklistId))
        //            {
        //                hdBlacklistId.Value = blacklistId;
        //                cbBlacklist.Checked = true;
        //            }
        //            else
        //            {
        //                hdBlacklistId.Value = "";
        //                cbBlacklist.Checked = false;
        //            }

        //            upPreleadPopup.Update();
        //            mpePreleadPopup.Show();
        //        }
        //        else
        //        {
        //            //ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "hideControl", "pageLoad();", true);
        //            AppUtil.ClientAlertTabRenew(Page, "ไม่พบ Prelead Id " + txtPreleadIdSearch.Text.Trim() + " ในระบบ");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        _log.Debug(ex);
        //        //ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "hideControl", "pageLoad();", true);
        //        AppUtil.ClientAlertTabRenew(Page, message);
        //    }
        //    finally
        //    {
        //        if (biz != null)
        //            biz.Dispose();
        //    }
        //}
        #endregion

        protected void btnAddContractPrelead_Click(object sender, EventArgs e)
        {
            ClearPreleadData(); // Clear Befor Open
            PreleadPhoneCallBiz biz = null;
            try
            {
                _log.Debug("");
                _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, StartLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                if (txtPreleadIdSearch.Text.Trim() == "")
                {
                    AppUtil.ClientAlertTabRenew(Page, "ไม่พบข้อมูล PreleadId เพื่อใช้ในการบันทึก");
                    return;
                }

                biz = new PreleadPhoneCallBiz();
                string contractNo = "", ticketId = "", firstName = "", lastName = "", cardTypeId = "", citizenId = "", campaignId = "", campaignName = "", productId = "", productName = "", productGroupId = "", productGroupName = "", ownerBranchCode = "", ownerBranchName = "", ownerName = "", ownerUsername = "", ownerEmpCode = ""
                    , telNo1 = "", statusCode = "", statusDesc = "", subStatusCode = "", subStatusDesc = "", delegateBranchCode = "", delegateBranchName = "", delegateUsername = "", delegateName = "", channelId = "", carLicenseNo = "";
                DateTime? nextContactDate = null;

                if (GetOBTMainData != null)
                {
                    _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, GetOBTMainData Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    GetOBTMainData(out contractNo, out ticketId, out firstName, out lastName, out cardTypeId, out citizenId, out campaignId, out campaignName, out productId, out productName, out productGroupId, out productGroupName, out ownerBranchCode, out ownerBranchName, out ownerUsername, out ownerName, out ownerEmpCode
                        , out delegateBranchCode, out delegateBranchName, out delegateUsername, out delegateName, out telNo1, out statusCode, out statusDesc, out subStatusCode, out subStatusDesc, out channelId, out nextContactDate, out carLicenseNo);

                    _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, GetOBTMainData End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                }

                _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, InitialDropdownlist Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                InitialDropdownlist(biz, productId, campaignId, statusCode);
                _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, InitialDropdownlist End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, GetPreleadData Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                PreleadViewData data = new PreleadBiz().GetPreleadData(AppUtil.SafeDecimal(txtPreleadIdSearch.Text), "", "", "");
                _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, GetPreleadData End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                
                //InitialControlPrelead(data, txtLoginName.Text.Trim(), true);

                txtContractNo_PreleadPopup.Text = data.ContractNo;
                txtCitizenId_PreleadPopup.Text = data.CitizenId;
                txtTelNo1_PreleadPopup.Text = data.TelNo1;
                txtTelNoSms_PreleadPopup.Text = string.IsNullOrWhiteSpace(data.TelNoSms) ? data.TelNo1 : data.TelNoSms;
                txtFirstname_PreleadPopup.Text = data.Firstname;
                txtLastname_PreleadPopup.Text = data.Lastname;
                txtProductId_PreleadPopup.Text = data.ProductId;
                txtProductName_PreleadPopup.Text = data.ProductName;
                txtProductGroupId_PreleadPopup.Text = data.ProductGroupId;
                txtProductGroupName_PreleadPopup.Text = data.ProductGroupName;
                txtCarLicenseNo_PreleadPopup.Text = data.CarLicenseNo;
                txtTicketId_PreleadPopup.Text = data.TicketId;
                txtCampaignId_PreleadPopup.Text = data.CampaignId;
                txtCampaignName_PreleadPopup.Text = data.CampaignName;
                txtOwnerBranchName_PreleadPopup.Text = data.OwnerBranchName;
                txtOwnerName_PreleadPopup.Text = data.OwnerName;
                AppUtil.SetComboValue(cmbCardType_PreleadPopup, data.CardTypeId.ToString());
                AppUtil.SetComboValue(cmbSubStatus_PreleadPopup, data.SubStatusCode);
                tdmNextContactDate_PreleadPopup.DateValue = data.NextContactDate ?? new DateTime();
                txtStatusCode_PreleadPopup.Text = data.StatusCode;
                txtStatusDesc_PreleadPopup.Text = data.StatusDesc;
                txtSubStatusCodeCurrent_PreleadPopup.Text = data.SubStatusCode;
                txtSubStatusNameCurrent_PreleadPopup.Text = data.SubStatusDesc;

                cardTypeId = cmbCardType_PreleadPopup.SelectedValue;

                if (cardTypeId != "")
                {
                    lblCitizenId_PreleadPopup.Text = "*";
                    txtCitizenId_PreleadPopup.Enabled = true;
                    AppUtil.SetCardTypeValidation(cmbCardType_PreleadPopup.SelectedItem.Value, txtCitizenId_PreleadPopup);
                }
                else
                {
                    lblCitizenId_PreleadPopup.Text = "";
                    txtCitizenId_PreleadPopup.Enabled = false;
                }

                //cbBlacklist.Enabled = biz.checkBlackList(HttpContext.Current.User.Identity.Name);
                _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, GetBlacklistId Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                string blacklistId = biz.GetBlacklistId(productId, citizenId);
                _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, GetBlacklistId End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                if (!string.IsNullOrEmpty(blacklistId))
                {
                    hdBlacklistId.Value = blacklistId;
                    cbBlacklist.Checked = true;
                }
                else
                {
                    hdBlacklistId.Value = "";
                    cbBlacklist.Checked = false;
                }

                upPreleadPopup.Update();
                mpePreleadPopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                _log.Debug(ex);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
            finally
            {
                _log.Debug($"Method=btnAddContractPrelead_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, EndLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                _log.Debug("");

                if (biz != null)
                {
                    biz.Dispose();
                }  
            }
        }

        protected void cmbCardType_PreleadPopup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCardType_PreleadPopup.SelectedItem.Value == "")
                {
                    vtxtCardType_PreleadPopup.Text = "";
                    lblCitizenId_PreleadPopup.Text = "";
                    vtxtCitizenId_PreleadPopup.Text = "";
                    txtCitizenId_PreleadPopup.Text = "";
                    txtCitizenId_PreleadPopup.Enabled = false;
                }
                else
                {
                    vtxtCardType_PreleadPopup.Text = "";
                    lblCitizenId_PreleadPopup.Text = "*";
                    vtxtCitizenId_PreleadPopup.Text = "";
                    txtCitizenId_PreleadPopup.Enabled = true;
                    AppUtil.SetCardTypeValidation(cmbCardType_PreleadPopup.SelectedItem.Value, txtCitizenId_PreleadPopup);
                    AppUtil.ValidateCardId(cmbCardType_PreleadPopup, txtCitizenId_PreleadPopup, vtxtCitizenId_PreleadPopup);
                }

                mpePreleadPopup.Show();
            }
            catch (Exception ex)
            {
                mpePreleadPopup.Show();

                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void txtCitizenId_PreleadPopup_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AppUtil.ValidateCardId(cmbCardType_PreleadPopup, txtCitizenId_PreleadPopup, vtxtCitizenId_PreleadPopup);
                mpePreleadPopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                _log.Debug(ex);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void txtTelNo1_PreleadPopup_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AppUtil.ValidateTelNo1(cmbCardType_PreleadPopup, txtTelNo1_PreleadPopup, vtxtTelNo1_PreleadPopup);
                mpePreleadPopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                _log.Debug(ex);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void btnSavepop_PreleadPopup_Click(object sender, EventArgs e)
        {
            PreleadPhoneCallBiz biz = null;
            try
            {
                _log.Debug("");
                _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, StartLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                if (ValidatePreleadData())
                {
                    biz = new PreleadPhoneCallBiz();
                    string newSubStatusCode = cmbSubStatus_PreleadPopup.SelectedItem.Value;

                    _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, CheckPerchase Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    //ต่อประกัน
                    if (newSubStatusCode == SLMConstant.SubStatusCode.AcceptRenew)
                    {
                        biz.CheckRenewPurchased(txtPreleadIdSearch.Text.Trim());
                    }
                    //ซื้อ พรบ เดี่ยว
                    if (newSubStatusCode == SLMConstant.SubStatusCode.ActPurchased)
                    {
                        biz.CheckActPurchased(txtPreleadIdSearch.Text.Trim());
                    }
                    _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, CheckPerchase End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    if (SLMConstant.TelNoLog == "Y")
                    {
                        _log.Info(string.Format("PreleadId={0}, TelNo1={1}, TelNoSMS={2}, Username={3}", txtPreleadIdSearch.Text.Trim(), txtTelNo1_PreleadPopup.Text.Trim(), txtTelNoSms_PreleadPopup.Text.Trim(), HttpContext.Current.User.Identity.Name));
                    }

                    _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, InsertData Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    biz.InsertData(decimal.Parse(txtPreleadIdSearch.Text.Trim()), txtContractNo_PreleadPopup.Text.Trim(), txtContactPhone_PreleadPopup.Text.Trim()
                                    , txtContactDetail_PreleadPopup.Text.Trim(), txtStatusCode_PreleadPopup.Text.Trim(), txtSubStatusCodeCurrent_PreleadPopup.Text.Trim(), txtSubStatusNameCurrent_PreleadPopup.Text.Trim(), cmbSubStatus_PreleadPopup.SelectedItem.Value, cmbSubStatus_PreleadPopup.SelectedItem.Text
                                    , tdmNextContactDate_PreleadPopup.DateValue, cmbCardType_PreleadPopup.SelectedItem.Value, txtCitizenId_PreleadPopup.Text.Trim()
                                    , HttpContext.Current.User.Identity.Name.ToLower(), txtTelNo1_PreleadPopup.Text.Trim(), hdBlacklistId.Value, cbBlacklist.Checked, txtTelNoSms_PreleadPopup.Text.Trim());
                    _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, InsertData End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    if (biz.DoRedirect == true)
                    {
                        btnSavepop_PreleadPopup.Enabled = false;
                        btnCancel_PreleadPopup.Enabled = false;
                        AppUtil.ClientAlertAndRedirect(Page, biz.ErrorMessage, GetSearchPageUrl());
                        return;
                    }

                    _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, CreateCASActivityLog Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    CreateCASActivityLog(biz);
                    _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, CreateCASActivityLog End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    if (!string.IsNullOrEmpty(biz.TicketId))
                    {
                        _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, Redirect To Ticket {biz.TicketId} Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                        SetTicketIdToNextLeadList(txtPreleadIdSearch.Text.Trim(), biz.TicketId);
                        string type = biz.GetFieldType(biz.TicketId, SLMConstant.ConfigProductScreen.ActionType.View);
                        Session[SLMConstant.SessionName.CampaignId] = biz.CampaignId;
                        Session[SLMConstant.SessionName.ProductId] = biz.ProductId;
                        Response.Redirect("SLM_SCR_004.aspx?ticketid=" + biz.TicketId + "&type=" + type + "&backtype=" + Request["backtype"], false);
                        _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, Redirect To Ticket {biz.TicketId} End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    }
                    else
                    {
                        _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, UpdatePhoneCallList Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                        UpdatePhoneCallList(biz.CreatedDate, biz.TicketId, txtCampaignName_PreleadPopup.Text.Trim(), txtFirstname_PreleadPopup.Text.Trim(), txtLastname_PreleadPopup.Text.Trim(), txtStatusDesc_PreleadPopup.Text.Trim()
                            , txtContactPhone_PreleadPopup.Text.Trim(), txtOwnerName_PreleadPopup.Text.Trim(), txtContactDetail_PreleadPopup.Text.Trim(), txtLoginName.Text.Trim(), cmbCardType_PreleadPopup.SelectedItem.Text, txtCitizenId_PreleadPopup.Text.Trim()
                            , "", "", "", "", "", "");
                        _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, UpdatePhoneCallList End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                        if (tabRenewInsuranceContainer.ActiveTabIndex == 0)
                        {
                            bool doUpdateLogging = false;
                            //ไม่ต้อง update ownerlogging เพื่อ improve performance
                            //if (cmbSubStatus_PreleadPopup.SelectedItem.Value != txtSubStatusCodeCurrent_PreleadPopup.Text.Trim())
                            //{
                            //    doUpdateLogging = true;
                            //}
                            _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, UpdatedMainDataChanged Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                            if (UpdatedMainDataChanged != null)
                            {
                                UpdatedMainDataChanged("", "", "", "", "", txtTelNo1_PreleadPopup.Text.Trim(), "", biz.CreatedDate, doUpdateLogging, cmbSubStatus_PreleadPopup.SelectedItem.Text, cmbCardType_PreleadPopup.SelectedItem.Value, txtCitizenId_PreleadPopup.Text.Trim(), cmbSubStatus_PreleadPopup.SelectedItem.Value, tdmNextContactDate_PreleadPopup.DateValue);
                            }
                            _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, UpdatedMainDataChanged End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                        }

                        biz.CloseConnection();

                        PreleadViewData preleadData = new PreleadViewData()
                        {
                            TicketId = "",
                            CitizenId = txtCitizenId_PreleadPopup.Text.Trim(),

                        };
                        if (txtPreleadIdSearch.Text.Trim() != "")
                        {
                            preleadData.PreleadId = Convert.ToDecimal(txtPreleadIdSearch.Text.Trim());
                        }
                        if (cmbCardType_PreleadPopup.SelectedItem.Value != "")
                        {
                            preleadData.CardTypeId = Convert.ToInt32(cmbCardType_PreleadPopup.SelectedItem.Value);
                        }

                        _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, UpdateCARScript Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                        UpdateCARScript(preleadData);
                        _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, UpdateCARScript End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                        mpePreleadPopup.Hide();
                        AppUtil.ClientAlertTabRenew(Page, "บันทึกข้อมูลเรียบร้อย");
                    }
                }
                else
                {
                    mpePreleadPopup.Show();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                _log.Debug(ex);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
            finally
            {
                _log.Debug($"Method=btnSavepop_PreleadPopup_Click, PreleadId={txtPreleadIdSearch.Text.Trim()}, EndLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                _log.Debug("");

                if (biz != null)
                {
                    biz.Dispose();
                } 
            }
        }

        private void SetTicketIdToNextLeadList(string preleadId, string ticketId)
        {
            string nextleadlist = SLMConstant.NextLeadList;
            if (Session[nextleadlist] != null)
            {
                var list = (List<NextLeadData>)Session[nextleadlist];
                var item = list.FirstOrDefault(p => p.PreleadId == decimal.Parse(preleadId));
                if (item != null)
                {
                    item.TicketId = ticketId;
                }
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

        private void UpdatePhoneCallList(DateTime createdDate, string ticketId, string campaignName, string firstName, string lastName, string statusDesc, string contactPhone
            , string ownerName, string contactDetail, string createdName, string cardTypeDesc, string citizenId, string creditFilePath, string creditFileName
            , string tawi50FilePath, string tawi50FileName, string driverLicenseFilePath, string driverLicenseFileName)
        {
            try
            {
                if (Session[SessionPrefix+SLMConstant.SessionName.renewinsure_phonecall_list] != null)
                {
                    var list = (List<PhoneCallHistoryData>)Session[SessionPrefix+SLMConstant.SessionName.renewinsure_phonecall_list];

                    int index = ownerName.LastIndexOf('(');
                    if (index > -1)
                    {
                        string strToCut = ownerName.Substring(index);
                        ownerName = ownerName.Replace(strToCut, "");
                    }
                    PhoneCallHistoryData newRow = new PhoneCallHistoryData()
                    {
                        CreatedDate = createdDate,
                        TicketId = ticketId,
                        CampaignName = campaignName,
                        Firstname = firstName,
                        Lastname = lastName,
                        StatusDesc = statusDesc,
                        ContactPhone = contactPhone,
                        OwnerName = ownerName,
                        ContactDetail = contactDetail,
                        CreatedName = createdName,
                        CardTypeDesc = cardTypeDesc,
                        CitizenId = citizenId,
                        CreditFilePath = creditFilePath,
                        CreditFileName = creditFileName,
                        Tawi50FilePath = tawi50FilePath,
                        Tawi50FileName = tawi50FileName,
                        DriverLicenseFilePath = driverLicenseFilePath,
                        DriverLicenseFileName = driverLicenseFileName
                    };
                    list.ForEach(p => p.CardTypeDesc = cardTypeDesc);
                    list.ForEach(p => p.CitizenId = citizenId);
                    list.Insert(0, newRow);

                    BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, list.ToArray(), 0);
                    Session[SessionPrefix+SLMConstant.SessionName.renewinsure_phonecall_list] = list;
                }
            }
            catch
            {
                throw;
            }
        }

        private void CreateCASActivityLog(PreleadPhoneCallBiz biz)
        {
            try
            {
                string nextContactDate = "";
                if (tdmNextContactDate_PreleadPopup.DateValue.Year != 1)
                {
                    nextContactDate = tdmNextContactDate_PreleadPopup.DateValue.ToString("dd/MM/") + tdmNextContactDate_PreleadPopup.DateValue.Year.ToString();
                }

                string carLicenseNo = txtCarLicenseNo_PreleadPopup.Text.Trim();
                string contractNo = txtContractNo_PreleadPopup.Text.Trim();

                //Activity Info
                List<Services.CARService.DataItem> actInfoList = new List<Services.CARService.DataItem>();
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "วันที่นัดหมายครั้งถัดไป", DataValue = nextContactDate });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ลูกค้าติดตามกรมธรรม์", DataValue = "" });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ลูกค้าติดตามเลข พรบ.", DataValue = "" });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 4, DataLabel = "Black List", DataValue = "" });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 5, DataLabel = "ฟอร์มตัดบัตรเครดิต", DataValue = "" });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 6, DataLabel = "ฟอร์มสำเนา 50 ทวิ", DataValue = "" });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 7, DataLabel = "สำเนาใบขับขี่", DataValue = "" });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 8, DataLabel = "รายละเอียด", DataValue = txtContactDetail_PreleadPopup.Text.Trim() });

                //Customer Info
                List<Services.CARService.DataItem> cusInfoList = new List<Services.CARService.DataItem>();
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = txtCitizenId_PreleadPopup.Text.Trim() });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = cmbCardType_PreleadPopup.SelectedItem.Text });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = txtFirstname_PreleadPopup.Text.Trim() + " " + txtLastname_PreleadPopup.Text.Trim() });

                //Product Info
                List<Services.CARService.DataItem> prodInfoList = new List<Services.CARService.DataItem>();
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = txtProductGroupName_PreleadPopup.Text.Trim() });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = txtProductName_PreleadPopup.Text.Trim() });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = txtCampaignName_PreleadPopup.Text.Trim() });

                //Contract Info
                List<Services.CARService.DataItem> contInfoList = new List<Services.CARService.DataItem>();
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = contractNo });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = "HP" });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = carLicenseNo });

                //Officer Info
                StaffData staff = GetCurrentStaff();
                List<Services.CARService.DataItem> offInfoList = new List<Services.CARService.DataItem>();
                offInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff != null ? staff.StaffNameTH : "" });

                Services.CARService.CARServiceData logdata = new Services.CARService.CARServiceData()
                {
                    ReferenceNo = biz.PhoneCallId.ToString(),
                    SecurityKey = txtPreleadIdSearch.Text.Trim() != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey,
                    ServiceName = "CreateActivityLog",
                    SystemCode = txtPreleadIdSearch.Text.Trim() != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM,      //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
                    TransactionDateTime = DateTime.Now,
                    ActivityInfoList = actInfoList,
                    CustomerInfoList = cusInfoList,
                    ProductInfoList = prodInfoList,
                    ContractInfoList = contInfoList,
                    OfficerInfoList = offInfoList,
                    ActivityDateTime = DateTime.Now,
                    CampaignId = txtCampaignId_PreleadPopup.Text.Trim(),
                    ChannelId = SLMConstant.CARLogService.CARPreleadChannelId,  //"TELESALES",
                    PreleadId = txtPreleadIdSearch.Text.Trim(),
                    ProductGroupId = txtProductGroupId_PreleadPopup.Text.Trim(),
                    ProductId = txtProductId_PreleadPopup.Text.Trim(),
                    Status = txtStatusDesc_PreleadPopup.Text.Trim(),
                    SubStatus = cmbSubStatus_PreleadPopup.SelectedItem.Text,
                    TicketId = "",
                    SubscriptionId = txtCitizenId_PreleadPopup.Text.Trim(),
                    TypeId = SLMConstant.CARLogService.Data.TypeId,
                    AreaId = SLMConstant.CARLogService.Data.AreaId,
                    SubAreaId = SLMConstant.CARLogService.Data.SubAreaId,
                    ActivityTypeId = txtPreleadIdSearch.Text.Trim() != "" ? SLMConstant.CARLogService.Data.ActivityType.CallOutboundId : SLMConstant.CARLogService.Data.ActivityType.CallInboundId,
                    ContractNo = contractNo
                };

                if (cmbCardType_PreleadPopup.SelectedItem.Value != "")
                    logdata.CIFSubscriptionTypeId = biz.GetCBSSubScriptionTypeId(Convert.ToInt32(cmbCardType_PreleadPopup.SelectedItem.Value));

                bool ret = Services.CARService.CreateActivityLog(logdata);
                //biz.UpdateCasFlag(phonecallId, ret ? "1" : "2");          //Comment by Pom 06/09/2016
            }
            catch (Exception ex)
            {
                //biz.UpdateCasFlag(phonecallId, "2");          //Comment by Pom 06/09/2016

                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                _log.Debug(ex);
            }
        }

        private void UpdateCARScript(PreleadViewData data)
        {
            try
            {
                lbCasAllActivity.OnClientClick = new Services.CARService().GetCallCASScript(data, HttpContext.Current.User.Identity.Name);
            }
            catch
            {
                throw;
            }
        }

        private void tempAllData()
        {
            PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
            if (pg == null) { return; }

            if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
            {
                pg.PreleadCompareDataCollectionMain = setViewStateData(pg.PreleadCompareDataCollectionMain);
            }
            else
            {
                PreleadCompareDataCollection pp = pg.PreleadCompareDataCollections.Where(p => p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID).FirstOrDefault();

                pp = setViewStateData(pp);
            }

            Session[SessionPrefix+"allData"] = pg;
        }


        #region Event Click Delete Policy
        protected void imgDel_pro1_Click(object sender, EventArgs e)
        {
            try
            {
                var seq = hidSeq1.Value;
                var PromotionId = hidPromotionId1.Value;

                hidPromotionId1.Value = "";
                hidSeq1.Value = "";
                tempAllData();
                if (PromotionId != "0")
                {
                    clearCheckBox(PromotionId);
                    setPromotion(AppUtil.SafeDecimal(PromotionId), false);

                }
                else
                {
                    setPromotion(AppUtil.SafeDecimal(seq), false, "P");
                }

                setNotClear();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }
        protected void imgDel_pro2_Click(object sender, EventArgs e)
        {
            try
            {
                var seq = hidSeq2.Value;
                var PromotionId = hidPromotionId2.Value;

                hidPromotionId2.Value = "";
                hidSeq2.Value = "";
                tempAllData();
                if (PromotionId != "0")
                {
                    clearCheckBox(PromotionId);
                    setPromotion(AppUtil.SafeDecimal(PromotionId), false);
                }
                else
                {
                    setPromotion(AppUtil.SafeDecimal(seq), false, "P");
                }

                setNotClear();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }
        protected void imgDel_pro3_Click(object sender, EventArgs e)
        {
            try
            {
                var seq = hidSeq3.Value;
                var PromotionId = hidPromotionId3.Value;

                hidPromotionId3.Value = "";
                hidSeq3.Value = "";
                tempAllData();
                if (PromotionId != "0")
                {
                    clearCheckBox(PromotionId);
                    setPromotion(AppUtil.SafeDecimal(PromotionId), false);
                }
                else
                {
                    setPromotion(AppUtil.SafeDecimal(seq), false, "P");
                }

                setNotClear();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }
        #endregion

        #region Event Click Delete Act
        protected void imgDelAct_pro1_Click(object sender, EventArgs e)
        {
            try
            {
                var seq = hidSeqAct1.Value;
                var PromotionId = hidPromotionActId1.Value;

                hidPromotionActId1.Value = "";
                hidSeqAct1.Value = "";
                tempAllData();
                if (PromotionId != "0")
                {
                    clearCheckBox(PromotionId);
                    setPromotion(AppUtil.SafeDecimal(PromotionId), false);

                }
                else
                {
                    setPromotion(AppUtil.SafeDecimal(seq), false, "A");
                }

                setNotClear();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }
        protected void imgDelAct_pro2_Click(object sender, EventArgs e)
        {
            try
            {
                var seq = hidSeqAct2.Value;
                var PromotionId = hidPromotionActId2.Value;

                hidPromotionActId2.Value = "";
                hidSeqAct2.Value = "";
                tempAllData();
                if (PromotionId != "0")
                {
                    clearCheckBox(PromotionId);
                    setPromotion(AppUtil.SafeDecimal(PromotionId), false);
                }
                else
                {
                    setPromotion(AppUtil.SafeDecimal(seq), false, "A");
                }

                setNotClear();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }
        protected void imgDelAct_pro3_Click(object sender, EventArgs e)
        {
            try
            {
                var seq = hidSeqAct3.Value;
                var PromotionId = hidPromotionActId3.Value;

                hidPromotionActId3.Value = "";
                hidSeqAct3.Value = "";
                tempAllData();
                if (PromotionId != "0")
                {
                    clearCheckBox(PromotionId);
                    setPromotion(AppUtil.SafeDecimal(PromotionId), false);
                }
                else
                {
                    setPromotion(AppUtil.SafeDecimal(seq), false, "A");
                }

                setNotClear();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }
        #endregion

        private void clearCheckBox(string PromotionId)
        {
            foreach (GridViewRow r in gvResultPromotion.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    if (gvResultPromotion.DataKeys[r.RowIndex].Value.ToString() == PromotionId)
                    {
                        ((CheckBox)((GridViewRow)r).FindControl("chkSelect")).Checked = false;
                        break;
                    }
                }
            }
        }

        private void setNotClear()
        {
            try
            {
                List<string> notClear = new List<string>();

                notClear.Add(hidPromotionId1.Value);
                notClear.Add(hidPromotionId2.Value);
                notClear.Add(hidPromotionId3.Value);
                notClear.Add(hidPromotionId4.Value);

                for (int i = 0; i < gvResultPromotion.Rows.Count; i++)
                {
                    CheckBox chkSelect = (CheckBox)gvResultPromotion.Rows[i].FindControlRecursive("chkSelect");

                    if (notClear.Where(s => s == gvResultPromotion.Rows[i].Cells[1].Text).Count() == 0)
                    {
                        chkSelect.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }

        }

        private bool ValidatePreleadData()
        {
            try
            {
                int i = 0;

                //Validate เลขที่บัตร
                if (cmbCardType_PreleadPopup.SelectedItem.Value != "")
                {
                    if (txtCitizenId_PreleadPopup.Text.Trim() == "")
                    {
                        vtxtCitizenId_PreleadPopup.Text = "กรุณาระบุเลขที่บัตร";
                        i += 1;
                    }
                    else
                    {
                        if (!AppUtil.ValidateCardId(cmbCardType_PreleadPopup, txtCitizenId_PreleadPopup, vtxtCitizenId_PreleadPopup))
                        {
                            i += 1;
                        }
                    }
                }
                else if (cmbCardType_PreleadPopup.SelectedItem.Value == "" && txtCitizenId_PreleadPopup.Text.Trim() != "")
                {
                    vtxtCardType_PreleadPopup.Text = "กรุณาระบุประเภทบุคคล";
                    i += 1;
                }
                else
                {
                    vtxtCardType_PreleadPopup.Text = "";
                    vtxtCitizenId_PreleadPopup.Text = "";
                }

                //TelNo1
                if (!AppUtil.ValidateTelNo1(cmbCardType_PreleadPopup, txtTelNo1_PreleadPopup, vtxtTelNo1_PreleadPopup))
                {
                    i += 1;
                }

                //สถานะย่อย
                if (cmbSubStatus_PreleadPopup.SelectedItem.Value == "")
                {
                    vcmbSubStatus_PreleadPopup.Text = "กรุณาระบุสถานะย่อย";
                    i += 1;
                }
                else
                {
                    vcmbSubStatus_PreleadPopup.Text = "";
                }
                    
                //หมายเลข SMS
                if (txtTelNoSms_PreleadPopup.Text.Trim() != "" && !AppUtil.ValidateTelNo1(cmbCardType_PreleadPopup.SelectedItem.Value, txtTelNoSms_PreleadPopup.Text.Trim()))
                {
                    vTelNoSms_PreleadPopup.Text = "กรุณาระบุหมายเลข SMS ให้ถูกต้อง";
                    i += 1;
                }
                else if (txtTelNoSms_PreleadPopup.Text.Trim() == "")
                {
                    vTelNoSms_PreleadPopup.Text = "กรุณาระบุหมายเลข SMS ให้ถูกต้อง";
                    i += 1;
                }
                else
                {
                    vTelNoSms_PreleadPopup.Text = "";
                }

                //รายละเอียดเพิ่มเติม
                if (txtContactDetail_PreleadPopup.Text.Trim() == "")
                {
                    vtxtContactDetail_PreleadPopup.Text = "กรุณากรอกข้อมูลรายละเอียดก่อนทำการบันทึก";
                    i += 1;
                }
                else
                {
                    vtxtContactDetail_PreleadPopup.Text = "";
                }

                upPreleadPopup.Update();

                return i > 0 ? false : true;
            }
            catch
            {
                throw;
            }
        }

        protected void btnCancel_PreleadPopup_Click(object sender, EventArgs e)
        {
            try
            {
                ClearPreleadData();
                mpePreleadPopup.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        private void ClearPreleadData()
        {
            try
            {
                txtContractNo_PreleadPopup.Text = "";
                txtFirstname_PreleadPopup.Text = "";
                txtLastname_PreleadPopup.Text = "";
                cmbCardType_PreleadPopup.SelectedIndex = -1;
                vtxtCardType_PreleadPopup.Text = "";
                lblCitizenId_PreleadPopup.Text = "";
                txtCitizenId_PreleadPopup.Text = "";
                vtxtCitizenId_PreleadPopup.Text = "";
                txtCampaignName_PreleadPopup.Text = "";
                txtOwnerBranchName_PreleadPopup.Text = "";
                txtOwnerName_PreleadPopup.Text = "";
                txtTelNo1_PreleadPopup.Text = "";
                vtxtTelNo1_PreleadPopup.Text = "";
                txtTelNoSms_PreleadPopup.Text = "";
                vTelNoSms_PreleadPopup.Text = "";
                txtStatusCode_PreleadPopup.Text = "";
                txtStatusDesc_PreleadPopup.Text = "";
                txtSubStatusCodeCurrent_PreleadPopup.Text = "";
                txtSubStatusNameCurrent_PreleadPopup.Text = "";
                cmbSubStatus_PreleadPopup.SelectedIndex = -1;
                vcmbSubStatus_PreleadPopup.Text = "";
                tdmNextContactDate_PreleadPopup.DateValue = new DateTime();
                txtContactPhone_PreleadPopup.Text = "";
                txtContactDetail_PreleadPopup.Text = "";
                vtxtContactDetail_PreleadPopup.Text = "";
                cbBlacklist.Checked = false;
                hdBlacklistId.Value = "";
                txtCarLicenseNo_PreleadPopup.Text = ""; //Hidden
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }

        }

        #region Lead

        public void InitialControlLead(LeadDefaultData data, string loginName)
        {
            PrepareLeadData(data, loginName);              //Added By Pom 13/05/2016
        }

        private void InitialControl(LeadData data)
        {
            try
            {
                PreleadCompareDataCollectionGroup PreLeadDataCollectionGroup = new PreleadCompareDataCollectionGroup();
                PreleadCompareDataCollection PreLeadDataCollectionMain = new PreleadCompareDataCollection();

                if (PreLeadId != "")
                {
                    _log.Debug($"Method=InitialControl, PreleadId={PreLeadId}, GetPreleadMainData and GenTab Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                    ActivityPreLeadBiz preleadBiz = new ActivityPreLeadBiz();
                    preleadBiz.GetPreleadMainData(PreLeadId, PreLeadDataCollectionMain);

                    TabPanel tbPanel = new TabPanel();
                    tbPanel.ID = PreLeadDataCollectionMain.keyTab;

                    tabControlData tData = new tabControlData();
                    tData.ID = PreLeadDataCollectionMain.keyTab;
                    tData.HeaderText = PreLeadDataCollectionMain.Prelead.slm_Contract_Number;
                    dynamicTab.Add(tData);

                    tbPanel.HeaderText = PreLeadDataCollectionMain.Prelead.slm_Contract_Number;
                    tbPanel.TabIndex = 0;
                    slm_Contract_Number.Value = PreLeadDataCollectionMain.Prelead.slm_Contract_Number;
                    slm_BranchId.Value = PreLeadDataCollectionMain.Prelead.slm_ActIssuaBranch;

                    tbPanel.Attributes.Add("onclick", "DisplayProcessing();");
                    this.tabRenewInsuranceContainer.Controls.Add(tbPanel);
                    PreLeadDataCollectionGroup.PreleadCompareDataCollectionMain = PreLeadDataCollectionMain;

                    _log.Debug($"Method=InitialControl, PreleadId={PreLeadId}, GetPreleadMainData and GenTab End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                }
                else if (TicketId != "")
                {
                    _log.Debug($"Method=InitialControl, TicketId={TicketId}, GetLeadMainData and GenTab Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                    ActivityLeadBiz biz = new ActivityLeadBiz();
                    biz.GetLeadMainData(TicketId, PreLeadDataCollectionMain);

                    TabPanel tbPanel = new TabPanel();
                    tbPanel.ID = PreLeadDataCollectionMain.keyTab;

                    tabControlData tData = new tabControlData();
                    tData.ID = PreLeadDataCollectionMain.keyTab;
                    tData.HeaderText = PreLeadDataCollectionMain.RenewIns.slm_ContractNo == "" || PreLeadDataCollectionMain.RenewIns.slm_ContractNo == null ? PreLeadDataCollectionMain.RenewIns.slm_TicketId : PreLeadDataCollectionMain.RenewIns.slm_ContractNo;
                    dynamicTab.Add(tData);

                    tbPanel.HeaderText = PreLeadDataCollectionMain.RenewIns.slm_ContractNo == "" || PreLeadDataCollectionMain.RenewIns.slm_ContractNo == null ? PreLeadDataCollectionMain.RenewIns.slm_TicketId : PreLeadDataCollectionMain.RenewIns.slm_ContractNo;
                    tbPanel.TabIndex = 0;
                    slm_Contract_Number.Value = PreLeadDataCollectionMain.RenewIns.slm_ContractNo;
                    slm_BranchId.Value = PreLeadDataCollectionMain.RenewIns.slm_ActIssueBranch;

                    tbPanel.Attributes.Add("onclick", "DisplayProcessing();");
                    this.tabRenewInsuranceContainer.Controls.Add(tbPanel);
                    PreLeadDataCollectionGroup.PreleadCompareDataCollectionMain = PreLeadDataCollectionMain;

                    _log.Debug($"Method=InitialControl, TicketId={TicketId}, GetLeadMainData and GenTab End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                }

                //Get Data Tab อื่นๆ (กรณีมีหลาย Tabs)
                if (PreLeadId != "")
                {
                    _log.Debug($"Method=InitialControl, Prelead={PreLeadId}, GetPreleads Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    List<PreleadData> Preleads = ActivityPreLeadBiz.GetPreleads(AppUtil.SafeInt(PreLeadId), HttpContext.Current.User.Identity.Name);
                    _log.Debug($"Method=InitialControl, Prelead={PreLeadId}, GetPreleads End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                    if (Preleads.Count() > 0)
                    {
                        short i = 1;
                        List<PreleadCompareDataCollection> preleadcomparedatacollections = new List<PreleadCompareDataCollection>();

                        _log.Debug($"Method=InitialControl, Prelead Many Tabs, Gen PreLeadDataCollection Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                        foreach (PreleadData Prelead in Preleads)
                        {
                            PreleadCompareDataCollection PreLeadDataCollection = new PreleadCompareDataCollection();
                            PreLeadDataCollection.Prelead = Prelead;

                            PreLeadDataCollection.keyTab = Prelead.slm_Prelead_Id.ToString();

                            List<PreleadCompareData> preleadcompare = ActivityPreLeadBiz.GetPreleadCompare((int)Prelead.slm_Prelead_Id);
                            List<PreleadCompareActData> preleadcompareact = ActivityPreLeadBiz.GetPreleadCompareAct((int)Prelead.slm_Prelead_Id);
                            PreLeadDataCollection.Address = ActivityPreLeadBiz.GetPreleadAddress((int)Prelead.slm_Prelead_Id);

                            if (preleadcompare.Count == 0)
                            {
                                //set prev
                                PreLeadDataCollection.ComparePrev = ActivityPreLeadBiz.GetPreleadtoCompare((int)Prelead.slm_Prelead_Id);
                                //set cur
                                PreLeadDataCollection.CompareCurr = ActivityPreLeadBiz.GetNotifyPremium((int)Prelead.slm_Prelead_Id);

                                if (PreLeadDataCollection.ComparePrev != null)
                                {
                                    if (PreLeadDataCollection.ComparePrev.slm_PolicyEndCoverDate != null)
                                    {
                                        if (PreLeadDataCollection.CompareCurr != null)
                                        {
                                            PreLeadDataCollection.CompareCurr.slm_PolicyStartCoverDate = PreLeadDataCollection.ComparePrev.slm_PolicyEndCoverDate;
                                            PreLeadDataCollection.CompareCurr.slm_PolicyEndCoverDate = PreLeadDataCollection.ComparePrev.slm_PolicyEndCoverDate.Value.AddYears(1);
                                        }
                                    }
                                }
                                PreLeadDataCollection.ComparePromoList = new List<PreleadCompareData>();
                            }
                            else
                            {
                                PreLeadDataCollection.ComparePrev = preleadcompare.Where(s => s.slm_Seq == 1).FirstOrDefault();
                                PreLeadDataCollection.CompareCurr = preleadcompare.Where(s => s.slm_Seq == 2).FirstOrDefault();
                                PreLeadDataCollection.ComparePromoList = preleadcompare.Where(s => s.slm_Seq != 2 && s.slm_Seq != 1).OrderBy(p => p.slm_Seq).ToList();
                            }


                            if (preleadcompareact.Count == 0)
                            {
                                PreLeadDataCollection.ActPrev = ActivityPreLeadBiz.GetPreleadtoCompareAct((int)Prelead.slm_Prelead_Id);
                                PreLeadDataCollection.ActPromoList = new List<PreleadCompareActData>();
                            }
                            else
                            {
                                PreLeadDataCollection.ActPrev = preleadcompareact.Where(s => s.slm_Seq == 1).FirstOrDefault();
                                PreLeadDataCollection.ActPromoList = preleadcompareact.Where(s => s.slm_Seq != 1).OrderBy(p => p.slm_Seq).ToList();
                            }

                            preleadcomparedatacollections.Add(PreLeadDataCollection);

                            TabPanel tbPanel = new TabPanel();
                            tbPanel.ID = PreLeadDataCollection.keyTab;

                            tabControlData tData = new tabControlData();
                            tData.ID = PreLeadDataCollection.keyTab;
                            tData.HeaderText = PreLeadDataCollection.Prelead.slm_Contract_Number;
                            dynamicTab.Add(tData);

                            tbPanel.HeaderText = PreLeadDataCollection.Prelead.slm_Contract_Number;
                            tbPanel.TabIndex = i; i++;
                            tbPanel.Attributes.Add("onclick", "DisplayProcessing();");
                            this.tabRenewInsuranceContainer.Controls.Add(tbPanel);
                        }
                        PreLeadDataCollectionGroup.PreleadCompareDataCollections = preleadcomparedatacollections;

                        _log.Debug($"Method=InitialControl, Prelead Many Tabs, Gen PreLeadDataCollection End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    }
                }
                else if (TicketId != "")
                {
                    _log.Debug($"Method=InitialControl, Ticket={TicketId}, GetLeads Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    List<LeadDataForRenewInsure> Leads = ActivityLeadBiz.GetLeads(TicketId, HttpContext.Current.User.Identity.Name);
                    _log.Debug($"Method=InitialControl, Ticket={TicketId}, GetLeads End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                    if (Leads.Count() > 0)
                    {
                        short i = 1;

                        List<PreleadCompareDataCollection> preleadcomparedatacollections = new List<PreleadCompareDataCollection>();

                        _log.Debug($"Method=InitialControl, Ticket Many Tabs, Gen PreLeadDataCollection Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                        foreach (LeadDataForRenewInsure lead in Leads)
                        {
                            PreleadCompareDataCollection PreLeadDataCollection = new PreleadCompareDataCollection();
                            PreLeadDataCollection.lead = lead;

                            PreleadData prelead = ActivityLeadBiz.GetPreleadbyTicket(lead.TicketId);
                            PreLeadDataCollection.Prelead = prelead;

                            RenewInsuranceData RenewIns = ActivityLeadBiz.GetRenewInsurance(lead.TicketId);
                            PreLeadDataCollection.RenewIns = RenewIns;

                            PreLeadDataCollection.keyTab = RenewIns.slm_TicketId.ToString();

                            List<PreleadCompareData> preleadcompare = ActivityLeadBiz.GetLeadCompare(lead.TicketId);
                            List<PreleadCompareActData> preleadcompareact = ActivityLeadBiz.GetLeadCompareAct(lead.TicketId);
                            PreLeadDataCollection.Address = ActivityLeadBiz.GetPreleadAddress(lead.TicketId);
                            PreLeadDataCollection.PaymentTransMainList = ActivityLeadBiz.GetPaymentTransMain(lead.TicketId);
                            PreLeadDataCollection.PaymentTransList = ActivityLeadBiz.GetPaymentTrans(lead.TicketId);
                            PreLeadDataCollection.Address = ActivityLeadBiz.GetPreleadAddress(lead.TicketId);
                            PreLeadDataCollection.PolicyRecAmt = ActivityLeadBiz.GetRecAmount(lead.TicketId, "204");
                            PreLeadDataCollection.ActRecAmt = ActivityLeadBiz.GetRecAmount(lead.TicketId, "205");
                            PreLeadDataCollection.ReceiptList = ActivityLeadBiz.GetReceipt(lead.TicketId);
                            PreLeadDataCollection.ProblemList = ActivityLeadBiz.getProblem(lead.TicketId);
                            PreLeadDataCollection.ReceiptDetailList = ActivityLeadBiz.getReceiptDetail(lead.TicketId);
                            PreLeadDataCollection.ReceiptRevisionDetailList = ActivityLeadBiz.getReceiptRevisionDetail(lead.TicketId);

                            if (preleadcompare.Count == 0)
                            {
                                //set prev
                                PreLeadDataCollection.ComparePrev = ActivityLeadBiz.GetPreleadtoCompare(lead.TicketId);
                                //set cur
                                PreLeadDataCollection.CompareCurr = ActivityLeadBiz.GetNotifyPremium(lead.TicketId);

                                if (PreLeadDataCollection.ComparePrev != null)
                                {
                                    if (PreLeadDataCollection.ComparePrev.slm_PolicyEndCoverDate != null)
                                    {
                                        if (PreLeadDataCollection.CompareCurr != null)
                                        {
                                            PreLeadDataCollection.CompareCurr.slm_PolicyStartCoverDate = PreLeadDataCollection.ComparePrev.slm_PolicyEndCoverDate;
                                            PreLeadDataCollection.CompareCurr.slm_PolicyEndCoverDate = PreLeadDataCollection.ComparePrev.slm_PolicyEndCoverDate.Value.AddYears(1);
                                        }
                                    }
                                }
                                PreLeadDataCollection.ComparePromoList = new List<PreleadCompareData>();
                            }
                            else
                            {
                                PreLeadDataCollection.ComparePrev = preleadcompare.Where(s => s.slm_Seq == 1).FirstOrDefault();
                                PreLeadDataCollection.CompareCurr = preleadcompare.Where(s => s.slm_Seq == 2).FirstOrDefault();
                                PreLeadDataCollection.ComparePromoList = preleadcompare.Where(s => s.slm_Seq != 2 && s.slm_Seq != 1).OrderBy(p => p.slm_Seq).ToList();
                            }

                            if (preleadcompareact.Count == 0)
                            {
                                PreLeadDataCollection.ActPrev = ActivityLeadBiz.GetPreleadtoCompareAct(lead.TicketId);
                                PreLeadDataCollection.ActPromoList = new List<PreleadCompareActData>();
                            }
                            else
                            {
                                PreLeadDataCollection.ActPrev = preleadcompareact.Where(s => s.slm_Seq == 1).FirstOrDefault();
                                PreLeadDataCollection.ActPromoList = preleadcompareact.Where(s => s.slm_Seq != 1).OrderBy(p => p.slm_Seq).ToList();
                            }

                            preleadcomparedatacollections.Add(PreLeadDataCollection);

                            TabPanel tbPanel = new TabPanel();
                            tbPanel.ID = PreLeadDataCollection.keyTab;

                            tabControlData tData = new tabControlData();
                            tData.ID = PreLeadDataCollection.keyTab;
                            tData.HeaderText = PreLeadDataCollection.RenewIns.slm_ContractNo == "" || PreLeadDataCollection.RenewIns.slm_ContractNo == null ? PreLeadDataCollection.RenewIns.slm_TicketId : PreLeadDataCollection.RenewIns.slm_ContractNo;
                            dynamicTab.Add(tData);

                            tbPanel.HeaderText = PreLeadDataCollection.RenewIns.slm_ContractNo == "" || PreLeadDataCollection.RenewIns.slm_ContractNo == null ? PreLeadDataCollection.RenewIns.slm_TicketId : PreLeadDataCollection.RenewIns.slm_ContractNo;
                            tbPanel.Attributes.Add("onclick", "DisplayProcessing();");
                            this.tabRenewInsuranceContainer.Controls.Add(tbPanel);
                            tbPanel.TabIndex = i; i++;
                        }
                        PreLeadDataCollectionGroup.PreleadCompareDataCollections = preleadcomparedatacollections;

                        _log.Debug($"Method=InitialControl, Ticket Many Tabs, Gen PreLeadDataCollection End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    }
                }

                UpdatePanel1.Update();
                Session[SessionPrefix+"allData"] = PreLeadDataCollectionGroup;

                _log.Debug($"Method=InitialControl, bindDropDownList Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                bindDropDownList();
                _log.Debug($"Method=InitialControl, bindDropDownList End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                _log.Debug($"Method=InitialControl, bindControl Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                bindControl(PreLeadDataCollectionMain, false, false);
                _log.Debug($"Method=InitialControl, bindControl End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlertTabRenew(this, ex.Message);
                throw ex;
            }
        }
        private void bindControl(PreleadCompareDataCollection data)
        {
            bindControl(data, true, false);
        }
        private void bindControl(PreleadCompareDataCollection data, bool isPagePostBack, bool fromSaveAction)
        {
            try
            {
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];

                if (pg.PreleadCompareDataCollections == null)
                {
                    chkPrint.Visible = false;
                }
                else
                {
                    chkPrint.Visible = true;

                    if (data.PrintFlag != null && data.PrintFlag != "")
                    {
                        chkPrint.Checked = true;
                    }
                    else
                    {
                        chkPrint.Checked = false;
                    }
                }

                if (string.IsNullOrWhiteSpace(TicketId))
                {
                    hdClientFullname.Value = data.Prelead != null ? data.Prelead.ClientFullname : "";
                    divBeneficiary.Visible = false;
                }
                else
                {
                    hdClientFullname.Value = data.lead != null ? data.lead.ClientFullname : "";
                    divBeneficiary.Visible = true;
                }

                //ข้อมูล Lead/Prelead
                bindPrelead(data);
                //ข้อมูล Lead/Prelead Compare
                bindPreleadCompare(data);

                //ค้นหาโปรโมชั่น
                if (fromSaveAction == false)
                {
                    DoSearchPromotionData(0);
                }

                //ดูผู้คำประกัน
                if (data.Prelead == null)
                {
                    GetSuretyData(0, 0);
                }
                else
                {
                    if (data.Prelead.slm_Prelead_Id != null)
                    {
                        GetSuretyData((int)data.Prelead.slm_Prelead_Id, 0);
                    }
                }

                //ข้อมูล พรบ.
                bindPreleadCompareAct(data);
                //ข้อมูล ที่อยู่
                bindPreleadAddress(data);

                // ย้ายมาไว้ด้านบน เพราะมีการ set วันที่จ่ายเงินตอน bindLead
                bindLead(data);
                if (string.IsNullOrEmpty(PreLeadId))
                {
                    setPaymentControl(false, data.RenewIns.slm_RenewInsureId);
                }

                //Set Policy Remark
                if (data.CompareCurr != null && !string.IsNullOrEmpty(data.CompareCurr.PolicyDiscountRemark))   //ใส่ค่า PolicyDiscountRemark ใน Method ActivityPreleadModel.GetNotifyPremium(), ActivityLeadModel.GetNotifyPremium()
                {
                    string remark = "ปีนี้ ";
                    if (lblInsNameTh_cur.Items.Count > 0)
                    {
                        remark += lblInsNameTh_cur.SelectedItem.Text.Trim();
                    }
                    remark += " ให้ส่วนลดประวัติดี " + data.CompareCurr.PolicyDiscountRemark;

                    txtInsDesc.Text += (txtInsDesc.Text.Trim() != "" ? Environment.NewLine : "") + remark;
                    data.CompareCurr.PolicyDiscountRemark = null;   //Clear เนื่องจากเมื่อกดบันทึก จะเข้า method นี้อีกครั้งทำให้ ข้อมูลแสดงซ้ำบนหน้าจอ
                }

                if (isPagePostBack)
                {
                    calAll(true);
                }

                if (!rbInsNameTh_cur.Checked && !rbInsNameTh_pro1.Checked && !rbInsNameTh_pro2.Checked && !rbInsNameTh_pro3.Checked)
                {
                    rbInsNameTh_cur.Checked = false;
                }
                else
                {
                    if (cmbPaymentmethod.SelectedIndex == 0)
                    {
                        AppUtil.SetComboValue(cmbPaymentmethod, "1");
                        pnPolicyPayment.Update();
                    }
                }

                divimport.Visible = false;
                divEditReceipt.Visible = false;
                setCopy();

                bool savebuttonflag = false;
                SlmScr004Biz biz = new SlmScr004Biz();
                if (PolicyPurchasedFlag == null)
                {
                    PolicyPurchasedFlag = biz.CheckMotorRenewPurchased(GetTicketIdActiveTab());
                }
                if (ActPurchasedFlag == null)
                {
                    ActPurchasedFlag = biz.CheckActPurchased(GetTicketIdActiveTab());
                }

                if (PreLeadId != "")
                {
                    btnCancelPolicy.Visible = false;
                    btnCancelAct.Visible = false;
                    divCancelPolicy.Visible = false;
                    cmbPolicyCancelReason.SelectedValue = "";
                    txtPolicyCancelDate.Text = "";
                    divCancelAct.Visible = false;
                    cmbActCancelReason.SelectedValue = "";
                    txtActCancelDate.Text = "";
                    btnHistoryMain.Visible = false;
                    Button3.Visible = false;
                    divRemarkPolicy.Visible = true;     //divRemarkPolicy.Visible = false;
                    divRemarkAct.Visible = true;       //divRemarkAct.Visible = false;
                    divPaymentAct.Visible = false;
                    setDriverControl(data);
                    savebuttonflag = ActivityLeadBiz.checkSaveButton(data.Prelead.slm_Product_Id, HttpContext.Current.User.Identity.Name);
                }
                else
                {
                    string username = HttpContext.Current.User.Identity.Name;
                    StaffData staff = GetCurrentStaff();

                    if (IsOwnerOrDelegate == null)
                    {
                        IsOwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(data.RenewIns.slm_TicketId, username);
                    }
                    if (IsSupervisor == null)
                    {
                        IsSupervisor = ActivityLeadBiz.CheckSupervisor(username);
                    }
                    
                    btnHistoryMain.Visible = true;
                    divRemarkPolicy.Visible = true;
                    divRemarkAct.Visible = true;
                    divPaymentAct.Visible = true;
                    setReceivePolicyButton(data, username, staff, IsOwnerOrDelegate.Value, IsSupervisor.Value, PolicyPurchasedFlag.Value);
                    setReceiveActButton(data, username, staff, IsOwnerOrDelegate.Value, IsSupervisor.Value, ActPurchasedFlag.Value);
                    GetReceiptData(data);
                    btnCancelPolicy.Visible = true;
                    btnCancelAct.Visible = true;

                    setDivCancel(data);
                    GetResultActBuy(data.RenewIns.slm_TicketId);

                    if (Type == "1")
                    {
                        paymentDetail.Visible = true;
                    }
                    else
                    {
                        paymentDetail.Visible = false;
                    }

                    setColumn();
                    setDivPayment();                   
                    setReceiveIncentiveButton(staff, IsOwnerOrDelegate.Value, IsSupervisor.Value, PolicyPurchasedFlag.Value);         // enable ปุ่ม Receive & Incentive ของประกัน
                    setReceiveIncentiveActButton(staff, IsOwnerOrDelegate.Value, IsSupervisor.Value, ActPurchasedFlag.Value);                                   // enable ปุ่ม Receive & Incentive ของ พรบ
                    setPolicyPayment();
                    setDriverControl(data);
                    setCancelButton(data);
                    
                    if (CheckOutStandingProblem(data.RenewIns.slm_TicketId))
                    {
                        setDisableCurr(true);
                        for (var i = 1; i <= 3; i++)
                        {
                            if (data.ComparePromoList.Count > i - 1)
                            {
                                if (data.ComparePromoList[i - 1].slm_PromotionId == null)
                                {
                                    setDisablePromoPolicy(i, true);
                                }
                                else // not promotion -> ต้อง enable ปุ่มลบ 
                                {
                                    ((Button)this.FindControlRecursive("imgDel_pro" + i)).Enabled = true;
                                } 
                            }
                            if (data.ActPromoList.Count > i - 1)
                            {
                                if (data.ActPromoList[i - 1].slm_PromotionId == null)
                                {
                                    setDisablePromoAct(i, true);
                                }
                                else // not promotion -> ต้อง enable ปุ่มลบ
                                {
                                    ((Button)this.FindControlRecursive("imgDelAct_pro" + i)).Enabled = true;
                                }
                            }
                        }
                        setDisablePayment(false);
                    }
                    else
                    {
                        //bool exportNotifyPolicyReport = SlmScr004Biz.ExportNotifyPolicyReport(data.RenewIns.slm_TicketId);
                        //bool exportNotifyActReport = SlmScr004Biz.ExportNotifyActReport(data.RenewIns.slm_ReceiveNo, data.RenewIns.slm_TicketId);
                        if (!string.IsNullOrEmpty(data.RenewIns.slm_ReceiveNo) || data.RenewIns.slm_ActSendDate != null)
                        {
                            disableReceiptPolicy(data, data.RenewIns.slm_ReceiveNo);
                            disableReceiptAct(data, data.RenewIns.slm_ReceiveNo, data.RenewIns.slm_ActSendDate);
                        }
                    }

                    if (data.lead == null)
                    {
                        savebuttonflag = false;
                    }
                    else
                    {
                        savebuttonflag = ActivityLeadBiz.checkSaveButton(data.lead.slm_Product_Id, HttpContext.Current.User.Identity.Name);
                    }

                    pnPaymentMainPolicy.Visible = false;
                    pnPaymentMainAct.Visible = false;
                }

                btnSave_PreleadData.Enabled = savebuttonflag;
                Button2.Enabled = savebuttonflag;

                btnCancelAct.Enabled = savebuttonflag;
                btnCancelPolicy.Enabled = savebuttonflag;

                btnAddBlankAct.Enabled = savebuttonflag;
                btnAddBlankPolicy.Enabled = savebuttonflag;

                setActIssuePlace();

                btnCancelPolicy.Enabled = PolicyPurchasedFlag.Value;       //เช็กต้องมีการซื้อประกันถึงจะกดยกเลิกประกันได้
                btnCancelAct.Enabled = ActPurchasedFlag.Value;             //เช็กต้องมีการซื้อพรบ.ถึงจะกดยกเลิกพรบ.ได้

                // คิดส่วนลดนิติบุคคล
                chkCardType_OnCheckedChanged(null, null);

                if (TicketId != "")
                {
                    string status = new SlmScr004Biz().GetStatus(GetTicketIdActiveTab());

                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        if (status == SLMConstant.StatusCode.Close
                            || status == SLMConstant.StatusCode.Cancel
                            || status == SLMConstant.StatusCode.Reject)
                        {
                            // disable buttons if lead status in (08, 09, 10) -> no preleadcompareCollection
                            btnSave_PreleadData.Enabled = false;
                            btnAddBlankPolicy.Enabled = false;
                            btnCancelPolicy.Enabled = false;
                            btnAddBlankAct.Enabled = false;
                            btnCancelAct.Enabled = false;
                            Button1.Enabled = false;
                            Button2.Enabled = false;

                        }
                        if (status == SLMConstant.StatusCode.Close)
                        {
                            radioClaim.Enabled = false;
                        }
                    }

                    //LeadDefaultData lead = new SlmScr004Biz().GetLeadDataForInitialPhoneCall(GetTicketIdActiveTab());

                    //if (lead != null)
                    //{
                    //    if (lead.StatusCode == SLMConstant.StatusCode.Close
                    //        || lead.StatusCode == SLMConstant.StatusCode.Cancel
                    //        || lead.StatusCode == SLMConstant.StatusCode.Reject)
                    //    {
                    //        // disable buttons if lead status in (08, 09, 10) -> no preleadcompareCollection
                    //        btnSave_PreleadData.Enabled = false;
                    //        btnAddBlankPolicy.Enabled = false;
                    //        btnCancelPolicy.Enabled = false;
                    //        btnAddBlankAct.Enabled = false;
                    //        btnCancelAct.Enabled = false;
                    //        Button1.Enabled = false;
                    //        Button2.Enabled = false;

                    //    }
                    //    if (lead.StatusCode == SLMConstant.StatusCode.Close)
                    //    {
                    //        radioClaim.Enabled = false;
                    //    }
                    //}
                }

                pnPolicy.Update();
                UpdatePanel2.Update();
                upPopupReceive.Update();
                pnPolicyPayment.Update();
                setPromotion(false, false);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                _log.Debug(ex);
                throw ex;
            }
        }

        private StaffData GetCurrentStaff()
        {
            if (CurrentStaff == null)
            {
                CurrentStaff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
            }
            return CurrentStaff;
        }

        private bool CheckOutStandingProblem(string ticketId)
        {
            if (CheckProblem == null)
            {
                CheckProblem = ActivityLeadBiz.checkProblem(ticketId);
            }
            return CheckProblem.Value;
        }

        private void UpdateDiscountPaymentSection()
        {
            try
            {
                decimal policyGrossPremiumTotal = 0;
                decimal policyGrossPremium = 0;
                decimal policyDiscountPercent = 0;
                decimal policyDiscountAmount = 0;
                decimal actGrossPremiumTotal = 0;
                decimal actGrossPremium = 0;
                decimal actDiscountPercent = 0;
                decimal actDiscountAmount = 0;
                SlmScr004Biz biz = new SlmScr004Biz();
                biz.GetDiscountPayment(GetTicketIdActiveTab(), out policyGrossPremiumTotal, out policyGrossPremium, out policyDiscountPercent, out policyDiscountAmount
                    , out actGrossPremiumTotal, out actGrossPremium, out actDiscountPercent, out actDiscountAmount);

                //ข้อมูลประกัน
                txtPolicyGrossPremiumTotal.Text = policyGrossPremiumTotal.ToString("#,##0.00");
                txtPolicyGrossPremium.Text = policyGrossPremium.ToString("#,##0.00");
                txtDiscountPercent.Text = policyDiscountPercent.ToString("#,##0");
                txtPolicyDiscountAmt.Text = policyDiscountAmount.ToString("#,##0.00");
                txtPolicyDiffAmt.Text = (AppUtil.SafeDecimal(txtPolicyGrossPremium.Text) - AppUtil.SafeDecimal(txtPolicyRecAmount.Text)).ToString("#,##0.00");

                //ข้อมูล พรบ.
                txtActGrossPremiumTotal.Text = actGrossPremiumTotal.ToString("#,##0.00");
                txtActGrossPremium.Text = actGrossPremium.ToString("#,##0.00");
                txtDiscountPercentAct.Text = actDiscountPercent.ToString("#,##0");
                txtActDiscountAmt.Text = actDiscountAmount.ToString("#,##0.00");
                txtActDiffAmt.Text = (AppUtil.SafeDecimal(txtActGrossPremium.Text) - AppUtil.SafeDecimal(txtActRecAmount.Text)).ToString("#,##0.00");

                UpdatePanel2.Update();
            }
            catch
            {
                throw;
            }
        }

        private void setCancelButton(PreleadCompareDataCollection data)
        {
            int countAct = 0;
            int countPolicy = 0;
            if (data.ComparePromoList != null)
            {
                countPolicy = data.ComparePromoList.Where(a => a.slm_Selected != null && a.slm_Selected.Value == true).Count();
            }

            if (data.ActPromoList != null)
            {
                countAct = data.ActPromoList.Where(a => a.slm_ActPurchaseFlag != null && a.slm_ActPurchaseFlag.Value == true).Count();
            }

            if (countPolicy > 0)
            {
                btnCancelPolicy.Enabled = true;
            }
            else
            {
                btnCancelPolicy.Enabled = false;
            }

            if (countAct > 0)
            {
                btnCancelAct.Enabled = true;
            }
            else
            {
                btnCancelAct.Enabled = false;
            }
        }

        private void bindPrelead(PreleadCompareDataCollection data)
        {
            try
            {
                if (data.Prelead != null)
                {
                    lblSlm_Brand_Name_Org.Text = data.Prelead.slm_Brand_Name_Org;
                    lblSlm_Model_Name_Org.Text = data.Prelead.slm_Model_Code_Org;
                    lblSlm_Model_Sub_Name_Org.Text = data.Prelead.slm_Model_name_Org;

                    lblModelYearOrg.Text = data.Prelead.slm_Model_Year_Org;
                    lblInsuranceCarTypeNameOrg.Text = data.Prelead.slm_Car_By_Gov_Name_Org;
                    lblProvinceRegisOrg.Text = data.Prelead.slm_ProvinceRegis_Org;
                    lblSlm_Cc.Text = data.Prelead.slm_Cc_Org;

                    //Added by Pom 15/08/2016
                    txtInsDesc.Text = data.Prelead.slm_RemarkPolicy;
                    txtRemarkAct.Text = data.Prelead.slm_RemarkAct;
                }

                cmbCarBrand2.Attributes.Add("autocomplete", "off");

                if (PreLeadId != "")
                {
                    if (data.Prelead != null)
                    {
                        cmbCarBrand.SelectedValue = data.Prelead.slm_Brand_Code == null ? "" : data.Prelead.slm_Brand_Code;
                        cmbCarBrand2.SelectedValue = data.Prelead.slm_Brand_Code == null ? "" : data.Prelead.slm_Brand_Code;
                    }
                }
                else
                {
                    cmbCarBrand.SelectedValue = data.RenewIns.slm_RedbookBrandCode == null ? "" : data.RenewIns.slm_RedbookBrandCode;
                    cmbCarBrand2.SelectedValue = data.RenewIns.slm_RedbookBrandCode == null ? "" : data.RenewIns.slm_RedbookBrandCode;
                }

                string carBrandValue = cmbCarBrand.SelectedItem.Value;
                var carBrandList = BrandCarBiz.ListCodeCar(carBrandValue);
                cmbCarName.SelectedIndex = -1;
                cmbCarName.DataSource = carBrandList.ToList();  //BrandCarBiz.ListCodeCar(carBrandValue);
                cmbCarName.DataTextField = "ModelName";
                cmbCarName.DataValueField = "ModelCode";
                cmbCarName.DataBind();
                cmbCarName.Items.Insert(0, new ListItem("", ""));

                cmbCarName2.SelectedIndex = -1;
                cmbCarName2.DataSource = carBrandList.ToList();     //BrandCarBiz.ListCodeCar(carBrandValue);
                cmbCarName2.DataTextField = "ModelName";
                cmbCarName2.DataValueField = "ModelCode";
                cmbCarName2.DataBind();
                cmbCarName2.Items.Insert(0, new ListItem("", ""));

                if (PreLeadId != "")
                {
                    if (data.Prelead != null)
                    {
                        AppUtil.SetComboValue(cmbCarName, data.Prelead.slm_Model_Code);
                        AppUtil.SetComboValue(cmbCarName2, data.Prelead.slm_Model_Code);
                        CCCar.Text = data.Prelead.slm_Cc == null ? "" : data.Prelead.slm_Cc.ToString();
                    }
                }
                else
                {
                    AppUtil.SetComboValue(cmbCarName, data.RenewIns.slm_RedbookModelCode);
                    AppUtil.SetComboValue(cmbCarName2, data.RenewIns.slm_RedbookModelCode);
                    CCCar.Text = data.RenewIns.slm_CC == null ? "" : data.RenewIns.slm_CC.ToString();
                }

                //Add 26/07/2017
                cmbModelYear.SelectedIndex = -1;
                cmbModelYear.DataSource = BrandCarBiz.ListModelYear(cmbCarBrand.SelectedItem.Value, cmbCarName.SelectedItem.Value);
                cmbModelYear.DataTextField = "TextField";
                cmbModelYear.DataValueField = "ValueField";
                cmbModelYear.DataBind();
                cmbModelYear.Items.Insert(0, new ListItem("", ""));

                if (PreLeadId != "")
                {
                    if (data.Prelead != null)
                    {
                        if (!string.IsNullOrEmpty(data.Prelead.slm_Model_Year))
                        {
                            AppUtil.SetComboValue(cmbModelYear, data.Prelead.slm_Model_Year);
                        }
                        if (data.Prelead.slm_Car_By_Gov_Id != null)
                        {
                            AppUtil.SetComboValue(cmbInsuranceCarType, data.Prelead.slm_Car_By_Gov_Id.Value.ToString());
                        }
                        if (data.Prelead.slm_ProvinceRegis != null)
                        {
                            AppUtil.SetComboValue(cmbProvinceRegis, data.Prelead.slm_ProvinceRegis.Value.ToString());
                        }

                        AppUtil.SetComboValue(cmbCarBrand2, data.Prelead.slm_Brand_Code);
                        AppUtil.SetComboValue(cmbCarName2, data.Prelead.slm_Model_Code);
                    }
                }
                else
                {
                    if (data.RenewIns.slm_RedbookYearGroup != null)
                    {
                        AppUtil.SetComboValue(cmbModelYear, data.RenewIns.slm_RedbookYearGroup.Value.ToString());
                    }
                    if (data.RenewIns.slm_InsuranceCarTypeId != null)
                    {
                        AppUtil.SetComboValue(cmbInsuranceCarType, data.RenewIns.slm_InsuranceCarTypeId.Value.ToString());
                    }
                    if (data.lead != null && data.lead.slm_ProvinceRegis != null)
                    {
                        AppUtil.SetComboValue(cmbProvinceRegis, data.lead.slm_ProvinceRegis.Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private void bindPreleadCompare(PreleadCompareDataCollection data)
        {
            try
            {
                //ปีเดิม
                if (data.ComparePrev != null)
                {
                    txtInsNameTh_pre.Text = data.ComparePrev.slm_CoverageTypeName != null ? data.ComparePrev.slm_CoverageTypeName : "";
                    lblVoluntary_Policy_Number_pre.Text = data.ComparePrev.slm_OldPolicyNo != null ? data.ComparePrev.slm_OldPolicyNo : "";
                    lblYear_pre.Text = data.ComparePrev.slm_PolicyStartCoverDate != null ? data.ComparePrev.slm_PolicyStartCoverDate.Value.ToString("yyyy") : "";
                    lblInsCom_Id_pre.Text = data.ComparePrev.slm_Ins_Com_Id != null ? data.ComparePrev.slm_Ins_Com_Id.ToString() : "";
                    lblInsNameTh_pre.Text = data.ComparePrev.slm_insnameth == null ? "ไม่ระบุ" : data.ComparePrev.slm_insnameth;
                    lblCoverageType_pre.Text = data.ComparePrev.slm_CoverageTypeName != null ? data.ComparePrev.slm_CoverageTypeName : "";
                    hidCoverageType_pre.Value = data.ComparePrev.slm_CoverageTypeId != null ? data.ComparePrev.slm_CoverageTypeId.ToString() : "";
                    lblVoluntary_Policy_Number_pre.Text = data.ComparePrev.slm_OldPolicyNo;
                    lblDriver_Flag_pre.Text = (data.ComparePrev.slm_Driver_First_Name1 == "" || data.ComparePrev.slm_Driver_First_Name1 == null) && (data.ComparePrev.slm_Driver_First_Name2 == null || data.ComparePrev.slm_Driver_First_Name2 == "") ? "ไม่ระบุ" : "ระบุ";
                    hidDriver_Flag_pre.Value = (data.ComparePrev.slm_Driver_First_Name1 == "" || data.ComparePrev.slm_Driver_First_Name1 == null) && (data.ComparePrev.slm_Driver_First_Name2 == null || data.ComparePrev.slm_Driver_First_Name2 == "") ? "0" : "1";
                    lblTitleName1_pre.Text = data.ComparePrev.slm_TitleName1 == null ? "" : data.ComparePrev.slm_TitleName1;
                    lblDriver_First_Name1_pre.Text = data.ComparePrev.slm_Driver_First_Name1 == null ? "" : data.ComparePrev.slm_Driver_First_Name1;
                    lblDriver_Last_Name1_pre.Text = data.ComparePrev.slm_Driver_Last_Name1 == null ? "" : data.ComparePrev.slm_Driver_Last_Name1;
                    lblDriver_Birthdate1_pre.Text = data.ComparePrev.slm_Driver_Birthdate1 != null ? data.ComparePrev.slm_Driver_Birthdate1.Value.ToString("dd/MM/yyyy") : "";
                    lblTitleName2_pre.Text = data.ComparePrev.slm_TitleName2 == null ? "" : data.ComparePrev.slm_TitleName2;
                    lblDriver_First_Name2_pre.Text = data.ComparePrev.slm_Driver_First_Name2 == null ? "" : data.ComparePrev.slm_Driver_First_Name2;
                    lblDriver_Last_Name2_pre.Text = data.ComparePrev.slm_Driver_Last_Name2 == null ? "" : data.ComparePrev.slm_Driver_Last_Name2;
                    lblInformed_pre.Text = data.ComparePrev.slm_OldReceiveNo;
                    lblDriver_Birthdate2_pre.Text = data.ComparePrev.slm_Driver_Birthdate2 != null ? data.ComparePrev.slm_Driver_Birthdate2.Value.ToString("dd/MM/yyyy") : "";
                    lblVoluntary_Policy_Eff_Date_pre.DateValue = data.ComparePrev.slm_PolicyStartCoverDate == null ? DateTime.MinValue : data.ComparePrev.slm_PolicyStartCoverDate.Value;
                    lblVoluntary_Policy_Exp_Date_pre.DateValue = data.ComparePrev.slm_PolicyEndCoverDate == null ? DateTime.MinValue : data.ComparePrev.slm_PolicyEndCoverDate.Value;
                    lblVoluntary_Cov_Amt_pre.Text = data.ComparePrev.slm_OD != null ? data.ComparePrev.slm_OD.Value.ToString("#,##0.00") : "";
                    lblTotal_Voluntary_Gross_Premium_pre.Text = data.ComparePrev.slm_PolicyGrossPremium != null ? data.ComparePrev.slm_PolicyGrossPremium.Value.ToString("#,##0.00") : "";
                    lblCoverageType2_pre.Text = data.ComparePrev.slm_CoverageTypeName != null ? data.ComparePrev.slm_CoverageTypeName : "";
                    lblNetpremium_pre.Text = data.ComparePrev.slm_PolicyGrossPremium != null ? data.ComparePrev.slm_PolicyGrossPremium.Value.ToString("#,##0.00") : "";
                    lblMaintanance_pre.Text = null;
                    lblPersonType_pre.Text = data.ComparePrev.slm_Vat1PercentBath != null && lblCardTypeId_pre.Text == "นิติบุคคล" ? data.ComparePrev.slm_Vat1PercentBath.Value.ToString("#,##0.00") : "";
                    lblDiscountPercent_pre.Text = data.ComparePrev.slm_DiscountPercent != null ? data.ComparePrev.slm_DiscountPercent.Value.ToString("#,##0") : "";
                    lblDiscountBath_pre.Text = data.ComparePrev.slm_DiscountBath != null ? data.ComparePrev.slm_DiscountBath.Value.ToString("#,##0.00") : "";
                    lblCost_pre.Text = null;
                    lblCostFT_pre.Text = null;
                    lblDuty_pre.Text = null;
                    lblVat_amount_pre.Text = null;
                    lblVoluntary_Gross_Premium_pre.Text = data.ComparePrev.slm_PolicyGrossPremium != null ? data.ComparePrev.slm_PolicyGrossPremium.Value.ToString("#,##0.00") : "";
                    lblExpireDate.Text = (DateTime.Now.Year + 543).ToString();
                }

                if (data.CompareCurr != null && hidHasCurrentYearPolicy.Value == "true"
                    && (data.CompareCurr.slm_NotifyPremiumId ?? 0) > 0)
                {
                    hidHasCurrentYearPolicy.Value = "true";
                    hidNotifyPremiumId.Value = data.CompareCurr.slm_NotifyPremiumId == null ? "" : data.CompareCurr.slm_NotifyPremiumId.Value.ToString();
                    rbInsNameTh_cur.Enabled = (hidNotifyPremiumId.Value != "" && hidNotifyPremiumId.Value != "0");
                    rbInsNameTh_cur.Checked = data.CompareCurr.slm_Selected == null ? false : data.CompareCurr.slm_Selected.Value;
                    lblInsNameTh_cur.Attributes.Add("autocomplete", "off");
                    lblCoverageType_cur.Attributes.Add("autocomplete", "off");

                    //Set Head Column ปีนี้
                    if (!string.IsNullOrEmpty(data.CompareCurr.slm_Year))
                    {
                        lblExpireDate.Text = data.CompareCurr.slm_Year;
                    }
                    else
                    {
                        lblExpireDate.Text = (DateTime.Now.Year + 543).ToString();
                    }

                    var comcur = data.CompareCurr.slm_Ins_Com_Id.ToString();
                    if (comcur != "")
                    {
                        if (lblInsNameTh_cur.Items.FindByValue(comcur) == null)
                        {
                            var insComName = InsComBiz.GetInsComName(data.CompareCurr.slm_Ins_Com_Id.Value);
                            if (!string.IsNullOrEmpty(insComName)) lblInsNameTh_cur.Items.Insert(1, new ListItem { Text = insComName, Value = comcur });
                        }
                    }

                    AppUtil.SetComboValue(lblInsNameTh_cur, comcur);
                    lblInsNameTh_cur.SelectedValue = data.CompareCurr.slm_Ins_Com_Id == null || data.CompareCurr.slm_Ins_Com_Id == 0 ? "" : data.CompareCurr.slm_Ins_Com_Id.ToString();
                    lblCoverageType_cur.SelectedValue = data.CompareCurr.slm_CoverageTypeId != null && data.CompareCurr.slm_CoverageTypeId.Value != 0 ? data.CompareCurr.slm_CoverageTypeId.ToString() : "";
                    lblVoluntary_Policy_Number_cur.Text = data.CompareCurr.slm_OldPolicyNo;

                    if (data.CompareCurr.slm_DriverFlag != null)
                    {
                        rbDriver_Flag_cur1.Checked = data.CompareCurr.slm_DriverFlag == "1" ? true : false;
                        rbDriver_Flag_cur2.Checked = data.CompareCurr.slm_DriverFlag == "1" ? false : true;
                    }
                    else
                    {
                        rbDriver_Flag_cur1.Checked = false;
                        rbDriver_Flag_cur2.Checked = true;
                    }

                    if (data.CompareCurr.slm_Driver_TitleId1 != 0)
                    {
                        cmbTitleName1_cur.SelectedValue = data.CompareCurr.slm_Driver_TitleId1 == null ? "" : data.CompareCurr.slm_Driver_TitleId1.Value.ToString();
                    }
                    else
                    {
                        cmbTitleName1_cur.SelectedValue = "";
                    }
                    txtDriver_First_Name1_cur.Text = data.CompareCurr.slm_Driver_First_Name1 == null ? "" : data.CompareCurr.slm_Driver_First_Name1;
                    txtDriver_Last_Name1_cur.Text = data.CompareCurr.slm_Driver_Last_Name1 == null ? "" : data.CompareCurr.slm_Driver_Last_Name1;
                    tdmDriver_Birthdate1_cur.DateValue = data.CompareCurr.slm_Driver_Birthdate1 != null ? data.CompareCurr.slm_Driver_Birthdate1.Value : DateTime.MinValue;
                    if (data.CompareCurr.slm_Driver_TitleId2 != 0)
                    {
                        cmbTitleName2_cur.SelectedValue = data.CompareCurr.slm_Driver_TitleId2 == null ? "" : data.CompareCurr.slm_Driver_TitleId2.Value.ToString();
                    }
                    else
                    {
                        cmbTitleName2_cur.SelectedValue = "";
                    }
                    txtDriver_First_Name2_cur.Text = data.CompareCurr.slm_Driver_First_Name2 == null ? "" : data.CompareCurr.slm_Driver_First_Name2;
                    txtDriver_Last_Name2_cur.Text = data.CompareCurr.slm_Driver_Last_Name2 == null ? "" : data.CompareCurr.slm_Driver_Last_Name2;
                    tdmDriver_Birthdate2_cur.DateValue = data.CompareCurr.slm_Driver_Birthdate2 != null ? data.CompareCurr.slm_Driver_Birthdate2.Value : DateTime.MinValue;
                    lblInformed_cur.Text = data.CompareCurr.slm_OldReceiveNo;
                    lblVoluntary_Policy_Eff_Date_cur.DateValue = data.CompareCurr.slm_PolicyStartCoverDate == null
                        ? (data.ComparePrev != null && data.ComparePrev.slm_PolicyEndCoverDate != null
                                ? data.ComparePrev.slm_PolicyEndCoverDate.Value
                                : DateTime.MinValue)
                        : data.CompareCurr.slm_PolicyStartCoverDate.Value;
                    lblVoluntary_Policy_Exp_Date_cur.DateValue = data.CompareCurr.slm_PolicyEndCoverDate == null
                        ? (data.ComparePrev != null && data.ComparePrev.slm_PolicyEndCoverDate != null
                                ? data.ComparePrev.slm_PolicyEndCoverDate.Value.AddYears(1)
                                : DateTime.MinValue)
                        : data.CompareCurr.slm_PolicyEndCoverDate.Value;
                    lblVoluntary_Cov_Amt_cur.Text = data.CompareCurr.slm_OD != null ? data.CompareCurr.slm_OD.Value.ToString("#,##0.00") : "";

                    // fix Firefox bug => dropdown selected is not working when press F5
                    lblCoverageType2_cur.Attributes.Add("autocomplete", "off");
                    lblCoverageType2_cur.SelectedValue = data.CompareCurr.slm_CoverageTypeId != null && data.CompareCurr.slm_CoverageTypeId.Value != 0 ? data.CompareCurr.slm_CoverageTypeId.ToString() : "";

                    // fix Firefox bug => dropdown selected is not working when press F5
                    lblMaintanance_cur.Attributes.Add("autocomplete", "off");
                    lblMaintanance_cur.SelectedValue = data.CompareCurr.slm_RepairTypeId == null || data.CompareCurr.slm_RepairTypeId.Value == 0 ? "" : data.CompareCurr.slm_RepairTypeId.Value.ToString();
                    lblPersonType_cur.Text = data.CompareCurr.slm_Vat1PercentBath != null && lblCardTypeId_pre.Text == "นิติบุคคล" ? data.CompareCurr.slm_Vat1PercentBath.Value.ToString("#,##0.00") : "";
                    txtDiscountPercent_cur.Text = data.CompareCurr.slm_DiscountPercent != null ? data.CompareCurr.slm_DiscountPercent.Value.ToString("#,##0") : "";
                    txtDiscountBath_cur.Text = data.CompareCurr.slm_DiscountBath != null ? data.CompareCurr.slm_DiscountBath.Value.ToString("#,##0.00") : "";

                    lblCost_cur.Text = null;

                    if (!string.IsNullOrEmpty(data.CompareCurr.slm_DeDuctibleFlag))
                    {
                        DropDownList cmbDeDuctFlag = (DropDownList)this.FindControlRecursive("cmbDeDuctFlag_cur");
                        cmbDeDuctFlag.SelectedIndex = cmbDeDuctFlag.Items.IndexOf(cmbDeDuctFlag.Items.FindByValue(data.CompareCurr.slm_DeDuctibleFlag));
                    }

                    lblCostFT_cur.Text = null;
                    lblNetpremium_cur.Text = data.CompareCurr.slm_PolicyGrossPremium != null ? data.CompareCurr.slm_PolicyGrossPremium.Value.ToString("#,##0.00") : "";
                    lblDuty_cur.Text = data.CompareCurr.slm_PolicyGrossStamp != null ? data.CompareCurr.slm_PolicyGrossStamp.Value.ToString("#,##0.00") : "";
                    lblVat_amount_cur.Text = null;
                    lblVoluntary_Cov_Amt_cur.Text = data.CompareCurr.slm_OD != null ? data.CompareCurr.slm_OD.Value.ToString("#,##0.00") : "";
                    lblVat_amount_cur.Text = data.CompareCurr.slm_PolicyGrossVat != null ? data.CompareCurr.slm_PolicyGrossVat.Value.ToString("#,##0.00") : "";
                    lblVoluntary_Gross_Premium_cur.Text = data.CompareCurr.slm_NetGrossPremium != null ? data.CompareCurr.slm_NetGrossPremium.Value.ToString("#,##0.00") : "";
                    txtTotal_Voluntary_Gross_Premium_cur.Text = data.CompareCurr.slm_PolicyGrossPremiumPay != null ? data.CompareCurr.slm_PolicyGrossPremiumPay.Value.ToString("#,##0.00") : "";
                    txtSafe_cur.Text = data.CompareCurr.slm_CostSave != null ? data.CompareCurr.slm_CostSave.Value.ToString("#,##0.00") : "";
                    var Saft_PremiumCur = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pre.Text) - AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_cur.Text);
                    txtSafe_cur.Text = Math.Abs(Saft_PremiumCur).ToString("#,##0.00");

                    if (Saft_PremiumCur < 0)
                    {
                        txtSafe_cur.ForeColor = Color.Red;
                    }
                    else
                    {
                        txtSafe_cur.ForeColor = Color.Green;
                    }
                }
                else
                {
                    hidHasCurrentYearPolicy.Value = "false";
                    rbInsNameTh_cur.Enabled = false;
                    rbDriver_Flag_cur1.Enabled = false;
                    rbDriver_Flag_cur2.Enabled = false;
                    lblVoluntary_Policy_Eff_Date_cur.Enabled = false;
                    lblVoluntary_Policy_Exp_Date_cur.Enabled = false;
                    txtDiscountPercent_cur.Enabled = false;
                    txtDiscountBath_cur.Enabled = false;
                    lblExpireDate.Text = (DateTime.Now.Year + 543).ToString();

                    hidNotifyPremiumId.Value = "";
                    rbInsNameTh_cur.Checked = false;
                    lblInsNameTh_cur.SelectedIndex = -1;
                    lblCoverageType_cur.SelectedIndex = -1;
                    lblVoluntary_Policy_Number_cur.Text = "";
                    rbDriver_Flag_cur1.Checked = false;
                    rbDriver_Flag_cur2.Checked = true;
                    setDriverControl(data);

                    cmbTitleName1_cur.SelectedIndex = -1;
                    txtDriver_First_Name1_cur.Text = "";
                    txtDriver_Last_Name1_cur.Text = "";
                    tdmDriver_Birthdate1_cur.DateValue = new DateTime();
                    cmbTitleName2_cur.SelectedIndex = -1;
                    txtDriver_First_Name2_cur.Text = "";
                    txtDriver_Last_Name2_cur.Text = "";
                    tdmDriver_Birthdate2_cur.DateValue = new DateTime();
                    lblInformed_cur.Text = "";
                    lblVoluntary_Policy_Eff_Date_cur.DateValue = new DateTime();
                    lblVoluntary_Policy_Exp_Date_cur.DateValue = new DateTime();
                    lblVoluntary_Cov_Amt_cur.Text = "";
                    lblCoverageType2_cur.SelectedIndex = -1;
                    lblMaintanance_cur.SelectedIndex = -1;
                    lblPersonType_cur.Text = "";
                    txtDiscountPercent_cur.Text = "";
                    txtDiscountBath_cur.Text = "";
                    lblCost_cur.Text = "";
                    cmbDeDuctFlag_cur.SelectedIndex = -1;
                    lblCostFT_cur.Text = "";
                    lblNetpremium_cur.Text = "";
                    lblDuty_cur.Text = "";
                    lblVat_amount_cur.Text = "";
                    lblVoluntary_Cov_Amt_cur.Text = "";
                    lblVat_amount_cur.Text = "";
                    lblVoluntary_Gross_Premium_cur.Text = "";
                    txtTotal_Voluntary_Gross_Premium_cur.Text = "";
                    txtSafe_cur.Text = "";
                    txtSafe_cur.Text = "";
                }

                //Clear Data
                for (int i = 1; i <= 3; i++)
                {
                    ((HiddenField)this.FindControlRecursive("hidPromotionId" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidSeq" + i)).Value = "";
                    ((RadioButton)this.FindControlRecursive("rbInsNameTh_pro" + i)).Checked = false;
                    ((Label)this.FindControlRecursive("lblInsnameth_name_pro" + i)).Text = "";
                    ((DropDownList)this.FindControlRecursive("lblInsNameTh_pro" + i)).SelectedValue = "";
                    //((Label)this.FindControlRecursive("lblInsCom_Id_pro" + i)).Text = "";
                    ((DropDownList)this.FindControlRecursive("lblCoverageType_pro" + i)).SelectedValue = "";
                    //((HiddenField)this.FindControlRecursive("hidCoverageType_pro" + i)).Value = "";
                    ((Label)this.FindControlRecursive("lblVoluntary_Policy_Number_pro" + i)).Text = "";

                    ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "1")).Checked = false;
                    ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "2")).Checked = true;

                    //hidDriver_Flag_pro.Value = pro.slm_Driver_First_Name1 == null && pro.slm_Driver_First_Name2 == null ? "0" : "1";
                    ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + i)).SelectedValue = "";
                    ((TextBox)this.FindControlRecursive("txtDriver_First_Name1_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("txtDriver_Last_Name1_pro" + i)).Text = "";
                    ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate1_pro" + i)).DateValue = DateTime.MinValue;
                    ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + i)).SelectedValue = "";
                    ((TextBox)this.FindControlRecursive("txtDriver_First_Name2_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("txtDriver_Last_Name2_pro" + i)).Text = "";
                    ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate2_pro" + i)).DateValue = DateTime.MinValue;
                    ((Label)this.FindControlRecursive("lblInformed_pro" + i)).Text = "";
                    ((TextDateMaskWithEvent)this.FindControlRecursive("lblVoluntary_Policy_Eff_Date_pro" + i)).DateValue = DateTime.MinValue;
                    ((TextDateMask)this.FindControlRecursive("lblVoluntary_Policy_Exp_Date_pro" + i)).DateValue = DateTime.MinValue;
                    ((TextBox)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text = "";
                    ((DropDownList)this.FindControlRecursive("lblCoverageType2_pro" + i)).SelectedValue = "";
                    ((Label)this.FindControlRecursive("lblCardTypeId_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("lblNetpremium_pro" + i)).Text = "";
                    ((DropDownList)this.FindControlRecursive("lblMaintanance_pro" + i)).SelectedValue = "";
                    //((HiddenField)this.FindControlRecursive("hidMaintanance_pro" + i)).Value = "";
                    ((Label)this.FindControlRecursive("lblPersonType_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("txtDiscountPercent_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("txtDiscountBath_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("lblCost_pro" + i)).Text = "";
                    ((DropDownList)this.FindControlRecursive("cmbDeDuctFlag_pro" + i)).SelectedIndex = -1;
                    ((TextBox)this.FindControlRecursive("lblCostFT_pro" + i)).Text = "";
                    //((TextBox)this.FindControlRecursive("lblNetpremium_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("lblDuty_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("lblVat_amount_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("lblVat_amount_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("lblVoluntary_Gross_Premium_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text = "";

                    ((HiddenField)this.FindControlRecursive("hidInjuryDeath_pro" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidTPPD_pro" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidPersonalAccident_pro" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidPersonalAccidentDriver_pro" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidPersonalAccidentPassenger_pro" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidMedicalFee_pro" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidMedicalFeeDriver_pro" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidMedicalFeePassenger_pro" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidInsuranceDriver_pro" + i)).Value = "";

                    ((TextBox)this.FindControlRecursive("txtSafe_pro" + i)).Text = "";
                }

                if (data.ComparePromoList != null && data.ComparePromoList.Count > 0)
                {
                    var i = 1;
                    bool hasReceiveNo = (data.RenewIns == null ? false : (data.RenewIns.slm_ReceiveNo == null ? false : data.RenewIns.slm_ReceiveNo != ""));
                    bool hasActSendDate = (data.RenewIns == null ? false : data.RenewIns.slm_ActSendDate != null);
                    List<ProblemDetailData> problem = new List<ProblemDetailData>();
                    bool checkProblem = false;
                    if (hasReceiveNo || hasActSendDate)
                    {
                        checkProblem = true;
                        problem = ActivityLeadBiz.getProblem(data.RenewIns.slm_TicketId);
                    }                      

                    foreach (PreleadCompareData pro in data.ComparePromoList)
                    {
                        ((RadioButton)this.FindControlRecursive("rbInsNameTh_pro" + i)).Checked = pro.slm_Selected == null ? false : pro.slm_Selected.Value;
                        ((HiddenField)this.FindControlRecursive("hidPromotionId" + i)).Value = pro.slm_PromotionId.Value.ToString();
                        ((HiddenField)this.FindControlRecursive("hidSeq" + i)).Value = pro.slm_Seq.ToString();

                        if ((pro.slm_PromotionId == null ? "0" : pro.slm_PromotionId.Value.ToString()) == "0")
                        {
                            ((Label)this.FindControlRecursive("lblInsnameth_name_pro" + i)).Text = "บริษัทประกันอื่นๆ";
                            setDisablePromoPolicy(i, true);
                        }
                        else
                        {
                            ((Label)this.FindControlRecursive("lblInsnameth_name_pro" + i)).Text = pro.slm_insnameth == null ? "" : pro.slm_insnameth;
                            setDisablePromoPolicy(i, false);
                        }

                        // enable ปุ่มลบประกัน ก่อนเช็ค 
                        ((Button)this.FindControlRecursive("imgDel_pro" + i)).Enabled = true;
                        //bool hasReceiveNo = (data.RenewIns == null ? false : (data.RenewIns.slm_ReceiveNo == null ? false : data.RenewIns.slm_ReceiveNo != ""));
                        //bool hasActSendDate = (data.RenewIns == null ? false : data.RenewIns.slm_ActSendDate != null);
                        //if (hasReceiveNo || hasActSendDate)

                        if (checkProblem)
                        {
                            //var problem = ActivityLeadBiz.getProblem(data.RenewIns.slm_TicketId);

                            // don't have any pendign problem => freeze as it should 
                            if (problem != null && problem
                                .Where(p => p.slm_FixTypeFlag != "2" || (p.slm_FixTypeFlag == "2" && p.slm_Export_Flag.GetValueOrDefault(false) == false)).Count() > 0)
                            {
                                ((Button)this.FindControlRecursive("imgDel_pro" + i)).Enabled = true;
                                ((Button)this.FindControlRecursive("imgDelAct_pro" + i)).Enabled = true;
                            }
                            else if ((pro.slm_Selected == null ? false : pro.slm_Selected.Value) && hasReceiveNo)
                            {
                                ((Button)this.FindControlRecursive("imgDel_pro" + i)).Enabled = false;
                            }
                            else
                            {
                                if (data.ActPromoList != null)
                                {
                                    foreach (PreleadCompareActData act in data.ActPromoList)
                                    {
                                        if ((act.slm_ActPurchaseFlag == null ? false : act.slm_ActPurchaseFlag.Value)
                                            && (pro.slm_PromotionId == null ? 0 : pro.slm_PromotionId.Value) != 0
                                            && (act.slm_PromotionId == null ? 0 : act.slm_PromotionId.Value) == (pro.slm_PromotionId == null ? 0 : pro.slm_PromotionId.Value)
                                            && data.RenewIns.slm_ActSendDate != null)
                                        {
                                            ((Button)this.FindControlRecursive("imgDel_pro" + i)).Enabled = false;
                                        }
                                    }
                                }
                            }
                        }
                        if ((pro.slm_Selected == null ? false : pro.slm_Selected.Value) && (hasReceiveNo || hasActSendDate))
                        {
                        }

                        DropDownList lblInsNameTh_pro = ((DropDownList)this.FindControlRecursive("lblInsNameTh_pro" + i));
                        if (pro.slm_Ins_Com_Id != null && pro.slm_Ins_Com_Id.Value != 0)
                        {
                            ListItem itm = lblInsNameTh_pro.Items.FindByValue(pro.slm_Ins_Com_Id.Value.ToString());
                            if (itm != null)
                            {
                                lblInsNameTh_pro.SelectedIndex = lblInsNameTh_pro.Items.IndexOf(itm);
                            }
                            else
                            {
                                string insComName = InsComBiz.GetInsComName(pro.slm_Ins_Com_Id.Value);
                                lblInsNameTh_pro.Items.Insert(1, new ListItem { Text = insComName, Value = pro.slm_Ins_Com_Id.Value.ToString() });
                                lblInsNameTh_pro.SelectedIndex = 1;
                            }

                            if (((Label)this.FindControlRecursive("lblInsnameth_name_pro" + i)).Text.Trim() == "")
                            {
                                ((Label)this.FindControlRecursive("lblInsnameth_name_pro" + i)).Text = lblInsNameTh_pro.SelectedItem.Text;
                            }
                        }
                        else
                        {
                            ((DropDownList)this.FindControlRecursive("lblInsNameTh_pro" + i)).SelectedValue = "";
                        }

                        ((DropDownList)this.FindControlRecursive("lblCoverageType_pro" + i)).SelectedValue = pro.slm_CoverageTypeId != null && pro.slm_CoverageTypeId.Value != 0 ? pro.slm_CoverageTypeId.ToString() : "";
                        ((Label)this.FindControlRecursive("lblVoluntary_Policy_Number_pro" + i)).Text = pro.slm_OldPolicyNo;

                        if (pro.slm_DriverFlag != null)
                        {
                            ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "1")).Checked = pro.slm_DriverFlag == "1" ? true : false;
                            ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "2")).Checked = pro.slm_DriverFlag == "1" ? false : true;
                        }
                        else
                        {
                            ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "1")).Checked = false;
                            ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "2")).Checked = true;
                        }

                        rbDriver_Flag_Click(null, null);    //may be removed
                        if (pro.slm_Driver_TitleId1 != 0)
                        {
                            ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + i)).SelectedValue = pro.slm_Driver_TitleId1 == null ? "" : pro.slm_Driver_TitleId1.Value.ToString();
                        }
                        else
                        {
                            ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + i)).SelectedValue = "";
                        }
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name1_pro" + i)).Text = pro.slm_Driver_First_Name1 == null ? "" : pro.slm_Driver_First_Name1;
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name1_pro" + i)).Text = pro.slm_Driver_Last_Name1 == null ? "" : pro.slm_Driver_Last_Name1;
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate1_pro" + i)).DateValue = pro.slm_Driver_Birthdate1 != null ? pro.slm_Driver_Birthdate1.Value : DateTime.MinValue;
                        if (pro.slm_Driver_TitleId2 != 0)
                        {
                            ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + i)).SelectedValue = pro.slm_Driver_TitleId2 == null ? "" : pro.slm_Driver_TitleId2.Value.ToString();
                        }
                        else
                        {
                            ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + i)).SelectedValue = "";
                        }
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name2_pro" + i)).Text = pro.slm_Driver_First_Name2 == null ? "" : pro.slm_Driver_First_Name2;
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name2_pro" + i)).Text = pro.slm_Driver_Last_Name2 == null ? "" : pro.slm_Driver_Last_Name2;
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate2_pro" + i)).DateValue = pro.slm_Driver_Birthdate2 != null ? pro.slm_Driver_Birthdate2.Value : DateTime.MinValue;
                        ((Label)this.FindControlRecursive("lblInformed_pro" + i)).Text = pro.slm_OldReceiveNo;
                        ((TextDateMaskWithEvent)this.FindControlRecursive("lblVoluntary_Policy_Eff_Date_pro" + i)).DateValue = pro.slm_PolicyStartCoverDate == null ? DateTime.MinValue : pro.slm_PolicyStartCoverDate.Value;
                        ((TextDateMask)this.FindControlRecursive("lblVoluntary_Policy_Exp_Date_pro" + i)).DateValue = pro.slm_PolicyEndCoverDate == null ? DateTime.MinValue : pro.slm_PolicyEndCoverDate.Value;
                        ((TextBox)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + i)).Text = pro.slm_OD != null ? pro.slm_OD.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text = pro.slm_PolicyGrossPremium != null ? pro.slm_PolicyGrossPremium.Value.ToString("#,##0.00") : "";

                        ((DropDownList)this.FindControlRecursive("lblCoverageType2_pro" + i)).Attributes.Add("autocomplete", "off");
                        ((DropDownList)this.FindControlRecursive("lblCoverageType2_pro" + i)).SelectedValue = pro.slm_CoverageTypeId != null && pro.slm_CoverageTypeId.Value != 0 ? pro.slm_CoverageTypeId.ToString() : "";

                        ((DropDownList)this.FindControlRecursive("lblMaintanance_pro" + i)).Attributes.Add("autocomplete", "off");
                        ((DropDownList)this.FindControlRecursive("lblMaintanance_pro" + i)).SelectedValue = pro.slm_RepairTypeId == null || pro.slm_RepairTypeId.Value == 0 ? "" : pro.slm_RepairTypeId.ToString();
                        ((Label)this.FindControlRecursive("lblPersonType_pro" + i)).Text = pro.slm_Vat1PercentBath != null && lblCardTypeId_pre.Text == "นิติบุคคล" ? pro.slm_Vat1PercentBath.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("txtDiscountPercent_pro" + i)).Text = pro.slm_DiscountPercent != null ? pro.slm_DiscountPercent.Value.ToString("#,##0") : "";
                        ((TextBox)this.FindControlRecursive("txtDiscountBath_pro" + i)).Text = pro.slm_DiscountBath != null ? pro.slm_DiscountBath.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("lblCost_pro" + i)).Text = pro.slm_DeDuctible != null ? pro.slm_DeDuctible.Value.ToString("#,##0.00") : "";

                        if (!string.IsNullOrEmpty(pro.slm_DeDuctibleFlag))
                        {
                            DropDownList cmbDeDuctFlag = (DropDownList)this.FindControlRecursive("cmbDeDuctFlag_pro" + i);
                            cmbDeDuctFlag.Attributes.Add("autocomplete", "off");
                            cmbDeDuctFlag.SelectedIndex = cmbDeDuctFlag.Items.IndexOf(cmbDeDuctFlag.Items.FindByValue(pro.slm_DeDuctibleFlag));
                        }

                        ((TextBox)this.FindControlRecursive("lblCostFT_pro" + i)).Text = pro.slm_FT != null ? pro.slm_FT.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("lblNetpremium_pro" + i)).Text = pro.slm_PolicyGrossPremium != null ? pro.slm_PolicyGrossPremium.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("lblDuty_pro" + i)).Text = pro.slm_PolicyGrossStamp != null ? pro.slm_PolicyGrossStamp.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("lblVat_amount_pro" + i)).Text = pro.slm_DiscountBath != null ? pro.slm_DiscountBath.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + i)).Text = pro.slm_OD != null ? pro.slm_OD.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("lblVat_amount_pro" + i)).Text = pro.slm_PolicyGrossVat != null ? pro.slm_PolicyGrossVat.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("lblVoluntary_Gross_Premium_pro" + i)).Text = pro.slm_NetGrossPremium != null ? pro.slm_NetGrossPremium.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text = pro.slm_PolicyGrossPremiumPay != null ? pro.slm_PolicyGrossPremiumPay.Value.ToString("#,##0.00") : "";

                        ((HiddenField)this.FindControlRecursive("hidInjuryDeath_pro" + i)).Value = pro.slm_InjuryDeath != null ? pro.slm_InjuryDeath.Value.ToString("#,##0.00") : "";
                        ((HiddenField)this.FindControlRecursive("hidTPPD_pro" + i)).Value = pro.slm_TPPD != null ? pro.slm_TPPD.Value.ToString("#,##0.00") : "";
                        ((HiddenField)this.FindControlRecursive("hidPersonalAccident_pro" + i)).Value = pro.slm_PersonalAccident != null ? pro.slm_PersonalAccident.Value.ToString("#,##0.00") : "";
                        ((HiddenField)this.FindControlRecursive("hidPersonalAccidentDriver_pro" + i)).Value = pro.slm_PersonalAccidentDriver != null ? pro.slm_PersonalAccidentDriver : "";
                        ((HiddenField)this.FindControlRecursive("hidPersonalAccidentPassenger_pro" + i)).Value = pro.slm_PersonalAccidentPassenger != null ? pro.slm_PersonalAccidentPassenger : "";
                        ((HiddenField)this.FindControlRecursive("hidMedicalFee_pro" + i)).Value = pro.slm_MedicalFee != null ? pro.slm_MedicalFee.Value.ToString("#,##0.00") : "";
                        ((HiddenField)this.FindControlRecursive("hidMedicalFeeDriver_pro" + i)).Value = pro.slm_MedicalFeeDriver != null ? pro.slm_MedicalFeeDriver : "";
                        ((HiddenField)this.FindControlRecursive("hidMedicalFeePassenger_pro" + i)).Value = pro.slm_MedicalFeePassenger != null ? pro.slm_MedicalFeePassenger : "";
                        ((HiddenField)this.FindControlRecursive("hidInsuranceDriver_pro" + i)).Value = pro.slm_InsuranceDriver != null ? pro.slm_InsuranceDriver.Value.ToString("#,##0.00") : "";

                        var Saft_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pre.Text) - AppUtil.SafeDecimal(((TextBox)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text);
                        ((TextBox)this.FindControlRecursive("txtSafe_pro" + i)).Text = Math.Abs(Saft_Premium).ToString("#,##0.00");

                        if (Saft_Premium < 0)
                        {
                            ((TextBox)this.FindControlRecursive("txtSafe_pro" + i)).ForeColor = Color.Red;
                        }
                        else
                        {
                            ((TextBox)this.FindControlRecursive("txtSafe_pro" + i)).ForeColor = Color.Green;
                        }
                        i++;
                    }

                    for (int j = i; j <= 3; j++)
                    {
                        ((HiddenField)this.FindControlRecursive("hidPromotionId" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidSeq" + i)).Value = "";
                        ((Label)this.FindControlRecursive("lblInsnameth_name_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("lblInsNameTh_pro" + i)).SelectedValue = "";
                        //((Label)this.FindControlRecursive("lblInsCom_Id_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("lblCoverageType_pro" + i)).SelectedValue = "";
                        //((HiddenField)this.FindControlRecursive("hidCoverageType_pro" + i)).Value = "";
                        ((Label)this.FindControlRecursive("lblVoluntary_Policy_Number_pro" + i)).Text = "";

                        ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "1")).Checked = false;
                        ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "2")).Checked = true;

                        //hidDriver_Flag_pro.Value = pro.slm_Driver_First_Name1 == null && pro.slm_Driver_First_Name2 == null ? "0" : "1";
                        ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + i)).SelectedValue = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name1_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name1_pro" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate1_pro" + i)).DateValue = DateTime.MinValue;
                        ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + i)).SelectedValue = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name2_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name2_pro" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate2_pro" + i)).DateValue = DateTime.MinValue;
                        ((Label)this.FindControlRecursive("lblInformed_pro" + i)).Text = "";
                        ((TextDateMaskWithEvent)this.FindControlRecursive("lblVoluntary_Policy_Eff_Date_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextDateMask)this.FindControlRecursive("lblVoluntary_Policy_Exp_Date_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextBox)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("lblCoverageType2_pro" + i)).SelectedValue = "";
                        ((Label)this.FindControlRecursive("lblCardTypeId_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblNetpremium_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("lblMaintanance_pro" + i)).SelectedValue = "";
                        //((HiddenField)this.FindControlRecursive("hidMaintanance_pro" + i)).Value = "";
                        ((Label)this.FindControlRecursive("lblPersonType_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDiscountPercent_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDiscountBath_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblCost_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("cmbDeDuctFlag_pro" + i)).SelectedIndex = -1;
                        ((TextBox)this.FindControlRecursive("lblCostFT_pro" + i)).Text = "";
                        //((TextBox)this.FindControlRecursive("lblNetpremium_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblDuty_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVat_amount_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVat_amount_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVoluntary_Gross_Premium_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text = "";

                        ((HiddenField)this.FindControlRecursive("hidInjuryDeath_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidTPPD_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidPersonalAccident_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidPersonalAccidentDriver_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidPersonalAccidentPassenger_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidMedicalFee_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidMedicalFeeDriver_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidMedicalFeePassenger_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidInsuranceDriver_pro" + i)).Value = "";

                        ((TextBox)this.FindControlRecursive("txtSafe_pro" + i)).Text = "";
                    }
                }
                else
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        ((HiddenField)this.FindControlRecursive("hidPromotionId" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidSeq" + i)).Value = "";
                        ((RadioButton)this.FindControlRecursive("rbInsNameTh_pro" + i)).Checked = false;
                        ((Label)this.FindControlRecursive("lblInsnameth_name_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("lblInsNameTh_pro" + i)).SelectedValue = "";
                        //((Label)this.FindControlRecursive("lblInsCom_Id_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("lblCoverageType_pro" + i)).SelectedValue = "";
                        //((HiddenField)this.FindControlRecursive("hidCoverageType_pro" + i)).Value = "";
                        ((Label)this.FindControlRecursive("lblVoluntary_Policy_Number_pro" + i)).Text = "";

                        ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "1")).Checked = false;
                        ((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "2")).Checked = true;

                        //hidDriver_Flag_pro.Value = pro.slm_Driver_First_Name1 == null && pro.slm_Driver_First_Name2 == null ? "0" : "1";
                        ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + i)).SelectedValue = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name1_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name1_pro" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate1_pro" + i)).DateValue = DateTime.MinValue;
                        ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + i)).SelectedValue = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name2_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name2_pro" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate2_pro" + i)).DateValue = DateTime.MinValue;
                        ((Label)this.FindControlRecursive("lblInformed_pro" + i)).Text = "";
                        ((TextDateMaskWithEvent)this.FindControlRecursive("lblVoluntary_Policy_Eff_Date_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextDateMask)this.FindControlRecursive("lblVoluntary_Policy_Exp_Date_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextBox)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("lblCoverageType2_pro" + i)).SelectedValue = "";
                        ((Label)this.FindControlRecursive("lblCardTypeId_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblNetpremium_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("lblMaintanance_pro" + i)).SelectedValue = "";
                        //((HiddenField)this.FindControlRecursive("hidMaintanance_pro" + i)).Value = "";
                        ((Label)this.FindControlRecursive("lblPersonType_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDiscountPercent_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDiscountBath_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblCost_pro" + i)).Text = "";
                        ((DropDownList)this.FindControlRecursive("cmbDeDuctFlag_pro" + i)).SelectedIndex = -1;
                        ((TextBox)this.FindControlRecursive("lblCostFT_pro" + i)).Text = "";
                        //((TextBox)this.FindControlRecursive("lblNetpremium_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblDuty_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVat_amount_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVat_amount_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVoluntary_Gross_Premium_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text = "";

                        ((HiddenField)this.FindControlRecursive("hidInjuryDeath_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidTPPD_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidPersonalAccident_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidPersonalAccidentDriver_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidPersonalAccidentPassenger_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidMedicalFee_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidMedicalFeeDriver_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidMedicalFeePassenger_pro" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidInsuranceDriver_pro" + i)).Value = "";

                        ((TextBox)this.FindControlRecursive("txtSafe_pro" + i)).Text = "";
                    }
                }

                if (PreLeadId != "")
                {
                    if (data.Prelead != null)
                    {
                        lblCardTypeId_pre.Text = data.Prelead.slm_CardTypeName;
                        lblCardTypeId_cur.Text = data.Prelead.slm_CardTypeName;

                        lblCardTypeId_pro1.Text = data.Prelead.slm_CardTypeName;
                        lblCardTypeId_pro2.Text = data.Prelead.slm_CardTypeName;
                        lblCardTypeId_pro3.Text = data.Prelead.slm_CardTypeName;

                        if (data.Prelead.slm_CardTypeName != "นิติบุคคล")
                        {
                            chkCardType.Enabled = false;
                            chkCardType2.Enabled = false;
                            chkCardType.Checked = false;
                            chkCardType2.Checked = false;
                        }
                        else
                        {
                            chkCardType.Enabled = true;
                            chkCardType2.Enabled = true;

                            bool chkPolicyFlag = false;
                            if (data.CompareCurr != null)
                            {
                                chkPolicyFlag = data.CompareCurr.slm_Vat1Percent ?? false;
                            }
                                
                            if (data.ComparePromoList != null)
                            {
                                foreach (PreleadCompareData pro in data.ComparePromoList)
                                {
                                    if (pro.slm_Vat1Percent != null && pro.slm_Vat1Percent.Value)
                                    {
                                        chkPolicyFlag = true;
                                    }
                                }
                            }

                            chkCardType.Checked = chkPolicyFlag;
                            if (!chkCardType.Checked)
                            {
                                lblPersonType_cur.Text = "";
                                lblPersonType_pro1.Text = "";
                                lblPersonType_pro2.Text = "";
                                lblPersonType_pro3.Text = "";
                            }

                            bool chkActFlag = false;

                            if (data.ActPromoList != null)
                            {
                                foreach (PreleadCompareActData pro in data.ActPromoList)
                                {
                                    if (pro.slm_Vat1Percent != null && pro.slm_Vat1Percent.Value)
                                    {
                                        chkActFlag = true;
                                    }
                                }
                            }
                            chkCardType2.Checked = chkActFlag;
                            if (!chkCardType2.Checked)
                            {
                                lblActPersonType_pro1.Text = "";
                                lblActPersonType_pro2.Text = "";
                                lblActPersonType_pro3.Text = "";
                            }
                        }
                    }
                }
                else
                {
                    if (data.lead != null)
                    {
                        lblCardTypeId_pre.Text = data.lead.slm_CardTypeName;
                        lblCardTypeId_cur.Text = data.lead.slm_CardTypeName;
                        lblCardTypeId_pro1.Text = data.lead.slm_CardTypeName;
                        lblCardTypeId_pro2.Text = data.lead.slm_CardTypeName;
                        lblCardTypeId_pro3.Text = data.lead.slm_CardTypeName;

                        if (data.lead.slm_CardTypeName != "นิติบุคคล")
                        {
                            chkCardType.Enabled = false;
                            chkCardType2.Enabled = false;
                            chkCardType.Checked = false;
                            chkCardType2.Checked = false;
                        }
                        else
                        {
                            chkCardType.Enabled = true;
                            chkCardType2.Enabled = true;

                            bool chkPolicyFlag = false;
                            bool chkActFlag = false;
                            if (data.CompareCurr != null)
                                chkPolicyFlag = data.CompareCurr.slm_Vat1Percent ?? false;

                            if (data.ComparePromoList != null)
                            {
                                foreach (PreleadCompareData pro in data.ComparePromoList)
                                {
                                    if (pro.slm_Vat1Percent != null && pro.slm_Vat1Percent.Value)
                                    {
                                        chkPolicyFlag = true;
                                    }
                                }
                            }
                            chkCardType.Checked = chkPolicyFlag;
                            if (!chkCardType.Checked)
                            {
                                lblPersonType_cur.Text = "";
                                lblPersonType_pro1.Text = "";
                                lblPersonType_pro2.Text = "";
                                lblPersonType_pro3.Text = "";
                            }

                            if (data.ActPromoList != null)
                            {
                                foreach (PreleadCompareActData pro in data.ActPromoList)
                                {
                                    if (pro.slm_Vat1Percent != null && pro.slm_Vat1Percent.Value)
                                    {
                                        chkActFlag = true;
                                    }
                                }
                            }

                            chkCardType2.Checked = chkActFlag;
                            if (!chkCardType2.Checked)
                            {
                                lblActPersonType_pro1.Text = "";
                                lblActPersonType_pro2.Text = "";
                                lblActPersonType_pro3.Text = "";
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }

        }

        private void bindPreleadCompareAct(PreleadCompareDataCollection data)
        {
            try
            {
                //ปีเดิม
                if (data.ActPrev != null)
                {
                    lblPromoAct_name_pre.Text = data.ActPrev.slm_ActStartCoverDate == null ? "" : data.ActPrev.slm_ActStartCoverDate.Value.ToString("yyyy");

                    if (lblPromoAct_name_pre.Text == "")
                    {
                        lblPromoAct_name_pre.Text = data.ActPrev.slm_Year;
                    }
                    //rbAct_pro
                    lblActIssuePlace_pre.Text = data.ActPrev.slm_ActIssuePlaceDesc + ' ' + data.ActPrev.slm_ActIssueBranchDesc;
                    hidActIssuePlace_pre.Value = data.ActPrev.slm_ActIssuePlace == null ? "" : data.ActPrev.slm_ActIssuePlace.Value.ToString();//cmbActIssuePlace_pro1
                    hidActIssueBranch_pre.Value = data.ActPrev.slm_ActIssueBranch;  //cmbActIssueBranch_pro1

                    lblRegisterAct_pre.Text = data.ActPrev.slm_SendDocTypeDesc;   //rbRegisterAct_pro1  rbRegisterAct_pro2
                    lblCompanyInsuranceAct_pre.Text = data.ActPrev.slm_insnameth == null ? "" : data.ActPrev.slm_insnameth;
                    lblActInsCom_Id_pre.Text = data.ActPrev.slm_Ins_Com_Id == null ? "" : data.ActPrev.slm_Ins_Com_Id.Value.ToString();
                    lbltxtSignNoAct_pre.Text = data.ActPrev.slm_ActSignNo;
                    lblActStartCoverDateAct_pre.Text = data.ActPrev.slm_ActStartCoverDate == null ? "" : data.ActPrev.slm_ActStartCoverDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    lblActEndCoverDateAct_pre.Text = data.ActPrev.slm_ActEndCoverDate == null ? "" : data.ActPrev.slm_ActEndCoverDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);  
                    lblActGrossPremiumAct_pre.Text = data.ActPrev.slm_ActGrossPremium != null ? data.ActPrev.slm_ActGrossPremium.Value.ToString("#,##0.00") : "";
                    lblActGrossStampAct_pre.Text = data.ActPrev.slm_ActGrossStamp != null ? data.ActPrev.slm_ActGrossStamp.Value.ToString("#,##0.00") : "";
                    lblActGrossVatAct_pre.Text = data.ActPrev.slm_ActGrossVat != null ? data.ActPrev.slm_ActGrossVat.Value.ToString("#,##0.00") : "";
                    lblActNetGrossPremiumAct_pre.Text = data.ActPrev.slm_ActNetGrossPremium != null ? data.ActPrev.slm_ActNetGrossPremium.Value.ToString("#,##0.00") : "";
                    chkCardType2.Checked = data.ActPrev.slm_Vat1Percent == null ? false : data.ActPrev.slm_Vat1Percent.Value;
                    lblActPersonType_pre.Text = data.ActPrev.slm_Vat1PercentBath != null && data.ActPrev.slm_Vat1PercentBath != 0 ? data.ActPrev.slm_Vat1PercentBath.Value.ToString("#,##0.00") : "";
                    lblVat1PercentBathAct_pre.Text = data.ActPrev.slm_DiscountPercent != null ? data.ActPrev.slm_DiscountPercent.Value.ToString("#,##0") : "";
                    lblDiscountPercentAct_pre.Text = data.ActPrev.slm_DiscountBath != null ? data.ActPrev.slm_DiscountBath.Value.ToString("#,##0.00") : "";
                    lblDiscountBathAct_pre.Text = data.ActPrev.slm_ActGrossPremiumPay != null ? data.ActPrev.slm_ActGrossPremiumPay.Value.ToString("#,##0.00") : "";
                }

                var insComList = InsComBiz.GetInsComList();

                //Clear Data
                for (var i = 1; i <= 3; i++)
                {
                    ((HiddenField)this.FindControlRecursive("hidPromotionActId" + i)).Value = "";
                    ((HiddenField)this.FindControlRecursive("hidSeqAct" + i)).Value = "";
                    ((RadioButton)this.FindControlRecursive("rbAct_pro" + i)).Checked = false;
                    ((Label)this.FindControlRecursive("lblPromoAct_name_pro" + i)).Text = "";

                    ((DropDownList)this.FindControlRecursive("cmbActIssuePlace_pro" + i)).SelectedValue = "0";

                    ((DropDownList)this.FindControlRecursive("cmbActIssueBranch_pro" + i)).SelectedValue = "";
                    ((DropDownList)this.FindControlRecursive("cmbActIssueBranch_pro" + i)).Visible = false;
                    ((RadioButton)this.FindControlRecursive("rbRegisterAct_pro" + i)).Checked = false;
                    ((RadioButton)this.FindControlRecursive("rbNormalAct_pro" + i)).Checked = false;

                    var cmb = ((DropDownList)this.FindControlRecursive("lblCompanyInsuranceAct_pro" + i));//.SelectedValue = "";
                    //BuildCombo(cmb, InsComBiz.GetInsComList());
                    BuildCombo(cmb, insComList.ToList());
                    AppUtil.SetComboValue(cmb, "");

                    ((TextBox)this.FindControlRecursive("txtSignNoAct_pro" + i)).Text = "";
                    ((TextDateMaskWithEvent)this.FindControlRecursive("txtActStartCoverDateAct_pro" + i)).DateValue = DateTime.MinValue;
                    ((TextDateMaskWithEvent)this.FindControlRecursive("txtActEndCoverDateAct_pro" + i)).DateValue = DateTime.MinValue;
                    ((TextDateMask)this.FindControlRecursive("txtCarTaxExpiredDateAct_pro" + i)).DateValue = DateTime.MinValue;
                    ((TextBox)this.FindControlRecursive("lblActNetGrossPremiumFullAct_pro" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActGrossPremiumAct_pro" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActGrossStampAct_pro" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActGrossVatAct_pro" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActNetGrossPremiumAct_pro" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActPersonType_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("lblVat1PercentBathAct_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("lblDiscountPercentAct_pro" + i)).Text = "";
                    ((TextBox)this.FindControlRecursive("txtDiscountBathAct_pro" + i)).Text = "";
                }

                if (data.ActPromoList != null && data.ActPromoList.Count > 0)
                {
                    var i = 1;
                    foreach (PreleadCompareActData pro in data.ActPromoList)
                    {
                        ((RadioButton)this.FindControlRecursive("rbAct_pro" + i)).Checked = pro.slm_ActPurchaseFlag == null ? false : pro.slm_ActPurchaseFlag.Value;
                        ((HiddenField)this.FindControlRecursive("hidSeqAct" + i)).Value = pro.slm_Seq.ToString();

                        if (((HiddenField)this.FindControlRecursive("hidPromotionActId" + i)).Value == "")
                        {
                            ((HiddenField)this.FindControlRecursive("hidPromotionActId" + i)).Value = pro.slm_PromotionId == null ? "0" : pro.slm_PromotionId.Value.ToString();
                        }

                        if ((pro.slm_PromotionId == null ? "0" : pro.slm_PromotionId.Value.ToString()) == "0")
                        {
                            ((Label)this.FindControlRecursive("lblPromoAct_name_pro" + i)).Text = "บริษัทประกันอื่นๆ";
                            ((Button)this.FindControlRecursive("imgDelAct_pro" + i)).Visible = true;
                            setDisablePromoAct(i, true);
                        }
                        else
                        {
                            ((Label)this.FindControlRecursive("lblPromoAct_name_pro" + i)).Text = pro.slm_insnameth == null ? "" : pro.slm_insnameth.ToString();
                            ((Button)this.FindControlRecursive("imgDelAct_pro" + i)).Visible = false;
                            setDisablePromoAct(i, false);
                        }

                        bool hasActSendDate = (data.RenewIns == null ? false : data.RenewIns.slm_ActSendDate != null);
                        if ((pro.slm_ActPurchaseFlag == null ? false : pro.slm_ActPurchaseFlag.Value) && hasActSendDate)
                        {
                            ((Button)this.FindControlRecursive("imgDelAct_pro" + i)).Enabled = false;
                        }

                        ((DropDownList)this.FindControlRecursive("cmbActIssuePlace_pro" + i)).SelectedValue = pro.slm_ActIssuePlace == null || pro.slm_ActIssuePlace.Value == 0 ? "0" : pro.slm_ActIssuePlace.ToString();

                        ((DropDownList)this.FindControlRecursive("cmbActIssueBranch_pro" + i)).SelectedValue = pro.slm_ActIssueBranch == null || pro.slm_ActIssueBranch == "0" ? "" : pro.slm_ActIssueBranch;
                        if (((DropDownList)this.FindControlRecursive("cmbActIssuePlace_pro" + i)).SelectedIndex == 1)
                        {
                            ((DropDownList)this.FindControlRecursive("cmbActIssueBranch_pro" + i)).Visible = true;
                        }

                        ((RadioButton)this.FindControlRecursive("rbRegisterAct_pro" + i)).Checked = pro.slm_SendDocType == null ? false : pro.slm_SendDocType.Value == 1 ? true : false;
                        ((RadioButton)this.FindControlRecursive("rbNormalAct_pro" + i)).Checked = pro.slm_SendDocType == null ? false : pro.slm_SendDocType.Value == 2 ? true : false;

                        DropDownList lblCompanyInsuranceAct_pro = ((DropDownList)this.FindControlRecursive("lblCompanyInsuranceAct_pro" + i));
                        //BuildCombo(lblCompanyInsuranceAct_pro, InsComBiz.GetInsComListWithOld(pro.slm_Ins_Com_Id ?? 0));

                        if (pro.slm_Ins_Com_Id != null && pro.slm_Ins_Com_Id.Value != 0)
                        {
                            ListItem itm = lblCompanyInsuranceAct_pro.Items.FindByValue(pro.slm_Ins_Com_Id.Value.ToString());
                            if (itm != null)
                            {
                                lblCompanyInsuranceAct_pro.SelectedIndex = lblCompanyInsuranceAct_pro.Items.IndexOf(itm);
                            }
                            else
                            {
                                string insComName = InsComBiz.GetInsComName(pro.slm_Ins_Com_Id.Value);
                                lblCompanyInsuranceAct_pro.Items.Insert(1, new ListItem { Text = insComName, Value = pro.slm_Ins_Com_Id.Value.ToString() });
                                lblCompanyInsuranceAct_pro.SelectedIndex = 1;
                            }

                            if (((Label)this.FindControlRecursive("lblPromoAct_name_pro" + i)).Text.Trim() == "")
                                ((Label)this.FindControlRecursive("lblPromoAct_name_pro" + i)).Text = lblCompanyInsuranceAct_pro.SelectedItem.Text;
                        }
                        else
                        {
                            ((DropDownList)this.FindControlRecursive("lblCompanyInsuranceAct_pro" + i)).SelectedValue = "";
                        }

                        ((TextBox)this.FindControlRecursive("txtSignNoAct_pro" + i)).Text = pro.slm_ActSignNo;
                        ((TextDateMaskWithEvent)this.FindControlRecursive("txtActStartCoverDateAct_pro" + i)).DateValue = pro.slm_ActStartCoverDate == null ? DateTime.MinValue : pro.slm_ActStartCoverDate.Value;
                        ((TextDateMaskWithEvent)this.FindControlRecursive("txtActEndCoverDateAct_pro" + i)).DateValue = pro.slm_ActEndCoverDate == null ? DateTime.MinValue : pro.slm_ActEndCoverDate.Value;
                        ((TextDateMask)this.FindControlRecursive("txtCarTaxExpiredDateAct_pro" + i)).DateValue = pro.slm_CarTaxExpiredDate == null ? DateTime.MinValue : pro.slm_CarTaxExpiredDate.Value;
                        ((TextBox)this.FindControlRecursive("lblActNetGrossPremiumFullAct_pro" + i)).Text = pro.slm_ActNetGrossPremiumFull == null ? "" : pro.slm_ActNetGrossPremiumFull.Value.ToString("#,##0.00");
                        ((Label)this.FindControlRecursive("lblActGrossPremiumAct_pro" + i)).Text = pro.slm_ActGrossPremium != null ? pro.slm_ActGrossPremium.Value.ToString("#,##0.00") : "";
                        ((Label)this.FindControlRecursive("lblActGrossStampAct_pro" + i)).Text = pro.slm_ActGrossStamp != null ? pro.slm_ActGrossStamp.Value.ToString("#,##0.00") : "";
                        ((Label)this.FindControlRecursive("lblActGrossVatAct_pro" + i)).Text = pro.slm_ActGrossVat != null ? pro.slm_ActGrossVat.Value.ToString("#,##0.00") : "";
                        ((Label)this.FindControlRecursive("lblActNetGrossPremiumAct_pro" + i)).Text = pro.slm_ActNetGrossPremium != null ? pro.slm_ActNetGrossPremium.Value.ToString("#,##0.00") : "";
                        ((Label)this.FindControlRecursive("lblActPersonType_pro" + i)).Text = pro.slm_Vat1PercentBath != null ? pro.slm_Vat1PercentBath.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("lblVat1PercentBathAct_pro" + i)).Text = pro.slm_DiscountPercent != null ? pro.slm_DiscountPercent.Value.ToString("#,##0") : "";
                        ((TextBox)this.FindControlRecursive("lblDiscountPercentAct_pro" + i)).Text = pro.slm_DiscountBath != null ? pro.slm_DiscountBath.Value.ToString("#,##0.00") : "";
                        ((TextBox)this.FindControlRecursive("txtDiscountBathAct_pro" + i)).Text = pro.slm_ActGrossPremiumPay != null ? pro.slm_ActGrossPremiumPay.Value.ToString("#,##0.00") : "";

                        if (((TextDateMaskWithEvent)this.FindControlRecursive("txtActEndCoverDateAct_pro" + i)).DateValue != DateTime.MinValue)
                        {
                            if (pro.slm_Vat1PercentBath == null)
                            {
                                if (i == 1)
                                {
                                    CalActPro1(false);
                                }
                                else if (i == 2)
                                {
                                    CalActPro2(false);
                                }
                                else if (i == 3)
                                {
                                    CalActPro3(false);
                                }
                            }
                        }

                        i++;
                    }

                    for (var j = i; j <= 3; j++)
                    {
                        ((HiddenField)this.FindControlRecursive("hidPromotionActId" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidSeqAct" + i)).Value = "";
                        ((RadioButton)this.FindControlRecursive("rbAct_pro" + i)).Checked = false;
                        ((Label)this.FindControlRecursive("lblPromoAct_name_pro" + i)).Text = "";

                        ((DropDownList)this.FindControlRecursive("cmbActIssuePlace_pro" + i)).SelectedValue = "0";

                        ((DropDownList)this.FindControlRecursive("cmbActIssueBranch_pro" + i)).SelectedValue = "";
                        ((RadioButton)this.FindControlRecursive("rbRegisterAct_pro" + i)).Checked = false;
                        ((RadioButton)this.FindControlRecursive("rbNormalAct_pro" + i)).Checked = false;

                        var cmb = ((DropDownList)this.FindControlRecursive("lblCompanyInsuranceAct_pro" + i));//.SelectedValue = "";
                        //BuildCombo(cmb, InsComBiz.GetInsComList());
                        BuildCombo(cmb, insComList.ToList());
                        AppUtil.SetComboValue(cmb, "");

                        ((TextBox)this.FindControlRecursive("txtSignNoAct_pro" + i)).Text = "";
                        ((TextDateMaskWithEvent)this.FindControlRecursive("txtActStartCoverDateAct_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextDateMaskWithEvent)this.FindControlRecursive("txtActEndCoverDateAct_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextDateMask)this.FindControlRecursive("txtCarTaxExpiredDateAct_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextBox)this.FindControlRecursive("lblActNetGrossPremiumFullAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActGrossPremiumAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActGrossStampAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActGrossVatAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActNetGrossPremiumAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActPersonType_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVat1PercentBathAct_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblDiscountPercentAct_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDiscountBathAct_pro" + i)).Text = "";
                    }
                }
                else
                {
                    for (var i = 1; i <= 3; i++)
                    {
                        ((HiddenField)this.FindControlRecursive("hidPromotionActId" + i)).Value = "";
                        ((HiddenField)this.FindControlRecursive("hidSeqAct" + i)).Value = "";
                        ((RadioButton)this.FindControlRecursive("rbAct_pro" + i)).Checked = false;
                        ((Label)this.FindControlRecursive("lblPromoAct_name_pro" + i)).Text = "";

                        ((DropDownList)this.FindControlRecursive("cmbActIssuePlace_pro" + i)).SelectedValue = "0";

                        ((DropDownList)this.FindControlRecursive("cmbActIssueBranch_pro" + i)).SelectedValue = "";
                        ((RadioButton)this.FindControlRecursive("rbRegisterAct_pro" + i)).Checked = false;
                        ((RadioButton)this.FindControlRecursive("rbNormalAct_pro" + i)).Checked = false;

                        var cmb = ((DropDownList)this.FindControlRecursive("lblCompanyInsuranceAct_pro" + i));//.SelectedValue = "";
                        //BuildCombo(cmb, InsComBiz.GetInsComList());
                        BuildCombo(cmb, insComList.ToList());
                        AppUtil.SetComboValue(cmb, "");

                        ((TextBox)this.FindControlRecursive("txtSignNoAct_pro" + i)).Text = "";
                        ((TextDateMaskWithEvent)this.FindControlRecursive("txtActStartCoverDateAct_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextDateMaskWithEvent)this.FindControlRecursive("txtActEndCoverDateAct_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextDateMask)this.FindControlRecursive("txtCarTaxExpiredDateAct_pro" + i)).DateValue = DateTime.MinValue;
                        ((TextBox)this.FindControlRecursive("lblActNetGrossPremiumFullAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActGrossPremiumAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActGrossStampAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActGrossVatAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActNetGrossPremiumAct_pro" + i)).Text = "";
                        ((Label)this.FindControlRecursive("lblActPersonType_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblVat1PercentBathAct_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("lblDiscountPercentAct_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDiscountBathAct_pro" + i)).Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }

        }

        private void bindPreleadAddressOld(PreleadAddressData data)
        {
            if (data != null)
            {
                txtHouseNo.Text = data.slm_House_No;
                txtMoo.Text = data.slm_Moo;
                txtBuilding.Text = data.slm_Building;
                txtHouseName.Text = data.slm_Village;
                txtSoi.Text = data.slm_Soi;
                txtStreet.Text = data.slm_Street;

                if (data.slm_Province_Id != null && data.slm_Province_Id.ToString() != "0")
                {
                    cmbProvince.SelectedIndex = cmbProvince.Items.IndexOf(cmbProvince.Items.FindByValue(data.slm_Province_Id.ToString()));
                }

                if (data.slm_Province_Id != null && data.slm_Province_Id != 0)
                {
                    string provinceValue = cmbProvince.SelectedItem.Value;

                    // Clear Old Data Brfor Bind
                    cmbDistinct.Items.Clear();
                    cmbDistinct.SelectedIndex = -1;

                    cmbDistinct.DataSource = SlmScr999Biz.ListDistinct(provinceValue);
                    cmbDistinct.DataTextField = "AmphurName";
                    cmbDistinct.DataValueField = "AmphurId";
                    cmbDistinct.DataBind();
                    cmbDistinct.Items.Insert(0, new ListItem("", ""));

                    //string distinctValue = cmbDistinct.SelectedItem.Value;

                    // Clear Old Data Brfor Bind
                    cmbThambol.Items.Clear();
                    cmbThambol.Items.Insert(0, new ListItem("", ""));
                    cmbThambol.SelectedIndex = -1;

                    //cmbThambol.DataSource = SlmScr999Biz.ListTambol(distinctValue, provinceValue);
                    //cmbThambol.DataTextField = "TambolNameTh";
                    //cmbThambol.DataValueField = "TambolId";
                    //cmbThambol.DataBind();
                    //cmbThambol.Items.Insert(0, new ListItem("", ""));
                }

                if (data.slm_Amphur_Id != null && data.slm_Amphur_Id.ToString() != "0")
                {
                    cmbDistinct.SelectedIndex = cmbDistinct.Items.IndexOf(cmbDistinct.Items.FindByValue(data.slm_Amphur_Id.ToString()));
                }

                if (data.slm_Amphur_Id != null && data.slm_Amphur_Id != 0)
                {
                    string provinceValue = cmbProvince.SelectedItem.Value;
                    string distinctValue = cmbDistinct.SelectedItem.Value;

                    // Clear Old Data Brfor Bind
                    cmbThambol.Items.Clear();
                    cmbThambol.SelectedIndex = -1;

                    cmbThambol.DataSource = SlmScr999Biz.ListTambol(distinctValue, provinceValue);
                    cmbThambol.DataTextField = "TambolNameTh";
                    cmbThambol.DataValueField = "TambolId";
                    cmbThambol.DataBind();
                    cmbThambol.Items.Insert(0, new ListItem("", ""));
                }

                if (data.slm_TambolId != null && data.slm_TambolId.ToString() != "0")
                {
                    AppUtil.SetComboValue(cmbThambol, data.slm_TambolId.ToString());
                }

                txtZipCode.Text = data.slm_Zipcode;
            }
        }

        private void bindPreleadAddress(PreleadCompareDataCollection data)
        {
            try
            {
                //ที่อยู่จัดส่งเอกสาร
                string slm_SendDocFlag = "";
                if (PreLeadId != "")
                {
                    rdoAddressOld.Checked = data.Prelead.slm_SendDocFlag == null ? true : data.Prelead.slm_SendDocFlag == "1" ? true : false;

                    rdoAddressChange.Checked = data.Prelead.slm_SendDocFlag == "2" ? true : false;
                    rdoAddressBranch.Checked = data.Prelead.slm_SendDocFlag == "3" ? true : false;

                    txtReceiver.Text = data.Prelead.slm_Receiver;
                    slm_SendDocFlag = data.Prelead.slm_SendDocFlag;

                    if (slm_SendDocFlag == "3")
                    {
                        cmbBranchCodeDoc.SelectedValue = data.Prelead.slm_SendDocBrandCode == null ? "" : data.Prelead.slm_SendDocBrandCode.ToString() == "0" ? "" : data.Prelead.slm_SendDocBrandCode.ToString();
                        if (txtReceiver.Text.Trim() == "")
                        {
                            txtReceiver.Text = cmbBranchCodeDoc.SelectedItem.Value != "" ? string.Format("{0} ({1})", kiatnakinBank, cmbBranchCodeDoc.SelectedItem.Text.Trim()) : kiatnakinBank;
                        }
                    }
                    else
                    {
                        if (txtReceiver.Text.Trim() == "")
                        {
                            txtReceiver.Text = data.Prelead.ClientFullname;
                        }
                    }
                }
                else if (TicketId != "")
                {
                    rdoAddressOld.Checked = data.RenewIns.slm_SendDocFlag == null ? true : data.RenewIns.slm_SendDocFlag == "1" ? true : false;

                    rdoAddressChange.Checked = data.RenewIns.slm_SendDocFlag == "2" ? true : false;
                    rdoAddressBranch.Checked = data.RenewIns.slm_SendDocFlag == "3" ? true : false;

                    txtReceiver.Text = data.RenewIns.slm_Receiver;
                    slm_SendDocFlag = data.RenewIns.slm_SendDocFlag;

                    if (slm_SendDocFlag == "3")
                    {
                        cmbBranchCodeDoc.SelectedValue = data.RenewIns.slm_SendDocBrandCode == null ? "" : data.RenewIns.slm_SendDocBrandCode.ToString() == "0" ? "" : data.RenewIns.slm_SendDocBrandCode.ToString();
                        if (txtReceiver.Text.Trim() == "")
                        {
                            txtReceiver.Text = cmbBranchCodeDoc.SelectedItem.Value != "" ? string.Format("{0} ({1})", kiatnakinBank, cmbBranchCodeDoc.SelectedItem.Text.Trim()) : kiatnakinBank;
                        }
                    }
                    else
                    {
                        if (txtReceiver.Text.Trim() == "")
                        {
                            txtReceiver.Text = data.lead != null ? data.lead.ClientFullname : "";
                        }
                    }
                }
                setAddressControl();
                if (data.Address != null)
                {
                    txtHouseNo.Text = data.Address.slm_House_No;
                    txtMoo.Text = data.Address.slm_Moo;
                    txtBuilding.Text = data.Address.slm_Building;
                    txtHouseName.Text = data.Address.slm_Village;
                    txtSoi.Text = data.Address.slm_Soi;
                    txtStreet.Text = data.Address.slm_Street;

                    if (data.Address.slm_Province_Id != null && data.Address.slm_Province_Id.ToString() != "0")
                    {
                        cmbProvince.SelectedIndex = cmbProvince.Items.IndexOf(cmbProvince.Items.FindByValue(data.Address.slm_Province_Id.ToString()));
                    }
                    else
                    {
                        cmbProvince.SelectedIndex = 0;
                    }

                    if (data.Address.slm_Province_Id != null && data.Address.slm_Province_Id != 0)
                    {
                        string provinceValue = cmbProvince.SelectedItem.Value;

                        // Clear Old Data Brfor Bind
                        cmbDistinct.Items.Clear();
                        cmbDistinct.SelectedIndex = -1;

                        cmbDistinct.DataSource = SlmScr999Biz.ListDistinct(provinceValue);
                        cmbDistinct.DataTextField = "AmphurName";
                        cmbDistinct.DataValueField = "AmphurId";
                        cmbDistinct.DataBind();
                        cmbDistinct.Items.Insert(0, new ListItem("", ""));

                        //string distinctValue = cmbDistinct.SelectedItem.Value;
                        // Clear Old Data Brfor Bind
                        cmbThambol.Items.Clear();
                        cmbThambol.Items.Insert(0, new ListItem("", ""));
                        cmbThambol.SelectedIndex = -1;

                        //cmbThambol.DataSource = SlmScr999Biz.ListTambol(distinctValue, provinceValue);
                        //cmbThambol.DataTextField = "TambolNameTh";
                        //cmbThambol.DataValueField = "TambolId";
                        //cmbThambol.DataBind();
                        //cmbThambol.Items.Insert(0, new ListItem("", ""));
                    }
                    else
                    {
                        cmbDistinct.Items.Clear();
                        cmbDistinct.Items.Insert(0, new ListItem("", ""));

                        cmbThambol.Items.Clear();
                        cmbThambol.Items.Insert(0, new ListItem("", ""));
                    }

                    if (data.Address.slm_Amphur_Id != null && data.Address.slm_Amphur_Id.ToString() != "0")
                    {
                        cmbDistinct.SelectedIndex = cmbDistinct.Items.IndexOf(cmbDistinct.Items.FindByValue(data.Address.slm_Amphur_Id.ToString()));
                    }
                        
                    if (data.Address.slm_Amphur_Id != null && data.Address.slm_Amphur_Id != 0)
                    {
                        string provinceValue = cmbProvince.SelectedItem.Value;
                        string distinctValue = cmbDistinct.SelectedItem.Value;

                        cmbThambol.DataSource = SlmScr999Biz.ListTambol(distinctValue, provinceValue);
                        cmbThambol.DataTextField = "TambolNameTh";
                        cmbThambol.DataValueField = "TambolId";
                        cmbThambol.DataBind();
                        cmbThambol.Items.Insert(0, new ListItem("", ""));
                    }

                    if (data.Address.slm_TambolId != null && data.Address.slm_TambolId.ToString() != "0")
                    {
                        cmbThambol.SelectedIndex = cmbThambol.Items.IndexOf(cmbThambol.Items.FindByValue(data.Address.slm_TambolId.ToString()));
                    }

                    txtZipCode.Text = data.Address.slm_Zipcode;
                }
                else
                {
                    txtHouseNo.Text = "";
                    txtMoo.Text = "";
                    txtBuilding.Text = "";
                    txtHouseName.Text = "";
                    txtSoi.Text = "";
                    txtStreet.Text = "";

                    cmbProvince.SelectedValue = "";
                    cmbProvince.SelectedItem.Value = "";
                    cmbDistinct.SelectedValue = "";
                    cmbThambol.SelectedValue = "";
                    txtZipCode.Text = "";
                    cmbBranchCodeDoc.SelectedValue = "";
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void bindLead(PreleadCompareDataCollection data)
        {
            try
            {
                if (TicketId != null && TicketId != "")
                {
                    if (data.RenewIns != null)
                    {
                        if (data.RenewIns.slm_BeneficiaryId != null)
                        {
                            cmbBeneficiary.SelectedIndex = cmbBeneficiary.Items.IndexOf(cmbBeneficiary.Items.FindByValue(data.RenewIns.slm_BeneficiaryId.ToString()));
                        }
                        else
                        {
                            cmbBeneficiary.SelectedIndex = -1;
                        }
                        cmbPaymentmethod.SelectedValue = data.RenewIns.slm_PayOptionId == null || data.RenewIns.slm_PayOptionId == 0 ? "" : data.RenewIns.slm_PayOptionId.ToString();
                        cmbPaymentmethodAct.SelectedValue = data.RenewIns.slm_ActPayOptionId == null || data.RenewIns.slm_ActPayOptionId == 0 ? "" : data.RenewIns.slm_ActPayOptionId.ToString();
                        textPeriod.Text = data.RenewIns.slm_PolicyAmountPeriod == null ? "" : data.RenewIns.slm_PolicyAmountPeriod.ToString() == "0" ? "" : data.RenewIns.slm_PolicyAmountPeriod.ToString();
                        cmbPaymentType.SelectedValue = data.RenewIns.slm_PolicyPayMethodId == null || data.RenewIns.slm_PolicyPayMethodId == 0 ? "" : data.RenewIns.slm_PolicyPayMethodId.Value.ToString();
                        cmbPolicyPayBranchCode.SelectedValue = data.RenewIns.slm_PayBranchCode == null ? "" : data.RenewIns.slm_PayBranchCode.ToString();
                        cmbPaymentTypeAct.SelectedValue = data.RenewIns.slm_ActPayMethodId == null || data.RenewIns.slm_ActPayMethodId == 0 ? "" : data.RenewIns.slm_ActPayMethodId.Value.ToString();
                        cmbPayBranchCodeAct.SelectedValue = data.RenewIns.slm_ActPayBranchCode == null || data.RenewIns.slm_ActPayBranchCode == "0" ? "" : data.RenewIns.slm_ActPayBranchCode.ToString();

                        // set default paymethod
                        if (data.ComparePromoList != null && data.ComparePromoList.Where(c => c.slm_Selected == true).Count() > 0 && cmbPaymentmethod.SelectedIndex == 0) AppUtil.SetComboValue(cmbPaymentmethod, "1");
                        if (data.ActPromoList != null && data.ActPromoList.Where(a => a.slm_ActPurchaseFlag == true).Count() > 0 && cmbPaymentmethodAct.SelectedIndex == 0) AppUtil.SetComboValue(cmbPaymentmethodAct, "1");

                        if (data.PayMainList != null && data.PayMainList.Count > 0)
                        {
                            foreach (RenewInsurancePaymentMainData pt in data.PayMainList)
                            {
                                ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + pt.slm_Seq)).DateValue = pt.slm_PaymentDate == null ? DateTime.MinValue : pt.slm_PaymentDate.Value;
                                ((TextBox)this.FindControlRecursive("txtPeriod" + pt.slm_Seq)).Text = pt.slm_PaymentAmount == null ? "" : pt.slm_PaymentAmount.Value.ToString("#,##0.00");
                            }

                            for (int i = data.PayMainList.Count + 1; i <= 10; i++)
                            {
                                ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                                ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                                ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                                ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                            }

                        }
                        else
                        {
                            cmbPayOption.SelectedValue = "";
                            txtPolicyAmountPeriod.Text = "";

                            tdmPaymentDate1.DateValue = DateTime.MinValue;
                            txtPeriod1.Text = "";
                            tdmPaymentDate2.DateValue = DateTime.MinValue;
                            txtPeriod2.Text = "";
                            tdmPaymentDate3.DateValue = DateTime.MinValue;
                            txtPeriod3.Text = "";
                            tdmPaymentDate4.DateValue = DateTime.MinValue;
                            txtPeriod4.Text = "";
                            tdmPaymentDate5.DateValue = DateTime.MinValue;
                            txtPeriod5.Text = "";
                            tdmPaymentDate6.DateValue = DateTime.MinValue;
                            txtPeriod6.Text = "";
                            tdmPaymentDate7.DateValue = DateTime.MinValue;
                            txtPeriod7.Text = "";
                            tdmPaymentDate8.DateValue = DateTime.MinValue;
                            txtPeriod8.Text = "";
                            tdmPaymentDate9.DateValue = DateTime.MinValue;
                            txtPeriod9.Text = "";
                            tdmPaymentDate10.DateValue = DateTime.MinValue;
                            txtPeriod10.Text = "";


                            trPday1.Visible = false;
                            trPday2.Visible = false;
                            trPday3.Visible = false;
                            trPday4.Visible = false;
                            trPday5.Visible = false;
                            trPday6.Visible = false;
                            trPday7.Visible = false;
                            trPday8.Visible = false;
                            trPday9.Visible = false;
                            trPday10.Visible = false;
                        }

                        var j = 1;

                        //ข้อมูลการชำระเงิน
                        if (data.PaymentTransMainList != null && data.PaymentTransMainList.Count > 0)
                        {
                            PaymentTransMainData ptm = data.PaymentTransMainList.FirstOrDefault();
                            cmbPolicyPayMethod.SelectedValue = ptm.slm_PayOptionId == null || ptm.slm_PayOptionId == 0 ? "" : ptm.slm_PayOptionId.Value.ToString();
                            txtPolicyAmountPeriod.Text = ptm.slm_PolicyAmountPeriod == null ? "" : ptm.slm_PolicyAmountPeriod.Value.ToString();
                            cmbPayOption.SelectedValue = ptm.slm_PolicyPayMethodId == null || ptm.slm_PolicyPayMethodId == 0 ? "" : ptm.slm_PolicyPayMethodId.Value.ToString();
                            cmbPayBranchCode.SelectedValue = data.RenewIns.slm_PayBranchCode;

                            var PolicyGrossPremiumTotal = data.RenewIns.slm_PolicyGrossPremiumTotal == null ? 0M : data.RenewIns.slm_PolicyGrossPremiumTotal.Value;
                            var Vat1PercentBath = data.RenewIns.slm_Vat1PercentBath == null ? 0M : data.RenewIns.slm_Vat1PercentBath.Value;
                            txtPolicyGrossPremiumTotal.Text = (PolicyGrossPremiumTotal - Vat1PercentBath).ToString("#,##0.00");
                            txtPolicyGrossPremium.Text = data.RenewIns.slm_PolicyGrossPremium == null ? "" : data.RenewIns.slm_PolicyGrossPremium.Value.ToString("#,##0.00");
                            txtPolicyRecAmount.Text = data.PolicyRecAmt == null ? "0.00" : data.PolicyRecAmt.Value.ToString("#,##0.00");
                            txtDiscountPercent.Text = data.RenewIns.slm_DiscountPercent == null ? "0.00" : data.RenewIns.slm_DiscountPercent.Value.ToString("#,##0");
                            txtPolicyDiscountAmt.Text = data.RenewIns.slm_PolicyDiscountAmt == null ? "0.00" : data.RenewIns.slm_PolicyDiscountAmt.Value.ToString("#,##0.00");
                            txtPolicyDiffAmt.Text = (AppUtil.SafeDecimal(txtPolicyGrossPremium.Text) - AppUtil.SafeDecimal(txtPolicyRecAmount.Text)).ToString("#,##0.00");

                            var ActGrossPremiumTotal = data.RenewIns.slm_ActNetPremium == null ? 0M : data.RenewIns.slm_ActNetPremium.Value;
                            var Vat1PercentBathAct = data.RenewIns.slm_ActVat1PercentBath == null ? 0M : data.RenewIns.slm_ActVat1PercentBath.Value;
                            var actVat = data.RenewIns.slm_ActVat == null ? 0M : data.RenewIns.slm_ActVat.Value;
                            var actTax = data.RenewIns.slm_ActStamp == null ? 0M : data.RenewIns.slm_ActStamp.Value;

                            txtActGrossPremiumTotal.Text = (ActGrossPremiumTotal + actVat + actTax - Vat1PercentBathAct).ToString("#,##0.00");
                            txtActGrossPremium.Text = data.RenewIns.slm_ActGrossPremium == null ? "" : data.RenewIns.slm_ActGrossPremium.Value.ToString("#,##0.00");

                            txtDiscountPercentAct.Text = data.RenewIns.slm_ActDiscountPercent == null ? "0" : data.RenewIns.slm_ActDiscountPercent.Value.ToString("#,##0");
                            txtActDiscountAmt.Text = data.RenewIns.slm_ActDiscountAmt == null ? "0.00" : data.RenewIns.slm_ActDiscountAmt.Value.ToString("#,##0.00");

                            txtActRecAmount.Text = data.ActRecAmt == null ? "0.00" : data.ActRecAmt.Value.ToString("#,##0.00");
                            txtActDiffAmt.Text = (AppUtil.SafeDecimal(txtActGrossPremium.Text) - AppUtil.SafeDecimal(txtActRecAmount.Text)).ToString("#,##0.00");

                            foreach (PaymentTransData paytrans in data.PaymentTransList)
                            {
                                ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + j)).DateValue = paytrans.slm_PaymentDate == null ? DateTime.MinValue : paytrans.slm_PaymentDate.Value;
                                ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + j)).Text = paytrans.slm_PaymentAmount == null ? "" : paytrans.slm_PaymentAmount.Value.ToString("#,##0.00");

                            }

                            for (int i = data.PaymentTransList.Count + 1; i <= 10; i++)
                            {
                                ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                                ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                                ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                                ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                                ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                            }
                        }
                        else
                        {
                            cmbPolicyPayMethod.SelectedValue = data.RenewIns.slm_PayOptionId == null || data.RenewIns.slm_PayOptionId == 0 ? "" : data.RenewIns.slm_PayOptionId.Value.ToString();
                            txtPolicyAmountPeriod.Text = data.RenewIns.slm_PolicyAmountPeriod == null ? "" : data.RenewIns.slm_PolicyAmountPeriod.Value.ToString();
                            cmbPayOption.SelectedValue = data.RenewIns.slm_PolicyPayMethodId == null || data.RenewIns.slm_PolicyPayMethodId == 0 ? "" : data.RenewIns.slm_PolicyPayMethodId.Value.ToString();
                            cmbPayBranchCode.SelectedValue = data.RenewIns.slm_PayBranchCode;


                            var PolicyGrossPremiumTotal = data.RenewIns.slm_PolicyGrossPremiumTotal == null ? 0M : data.RenewIns.slm_PolicyGrossPremiumTotal.Value;
                            var Vat1PercentBath = data.RenewIns.slm_Vat1PercentBath == null ? 0M : data.RenewIns.slm_Vat1PercentBath.Value;
                            txtPolicyGrossPremiumTotal.Text = (PolicyGrossPremiumTotal - Vat1PercentBath).ToString("#,##0.00");
                            txtPolicyGrossPremium.Text = data.RenewIns.slm_PolicyGrossPremium == null ? "" : data.RenewIns.slm_PolicyGrossPremium.Value.ToString("#,##0.00");
                            txtPolicyRecAmount.Text = data.PolicyRecAmt == null ? "0.00" : data.PolicyRecAmt.Value.ToString("#,##0.00");
                            txtDiscountPercent.Text = data.RenewIns.slm_DiscountPercent == null ? "" : data.RenewIns.slm_DiscountPercent.Value.ToString("#,##0");
                            txtPolicyDiscountAmt.Text = data.RenewIns.slm_PolicyDiscountAmt == null ? "" : data.RenewIns.slm_PolicyDiscountAmt.Value.ToString("#,##0.00");
                            txtPolicyDiffAmt.Text = (AppUtil.SafeDecimal(txtPolicyGrossPremium.Text) - AppUtil.SafeDecimal(txtPolicyRecAmount.Text)).ToString("#,##0.00");

                            var ActGrossPremiumTotal = data.RenewIns.slm_ActNetPremium == null ? 0M : data.RenewIns.slm_ActNetPremium.Value;
                            var Vat1PercentBathAct = data.RenewIns.slm_ActVat1PercentBath == null ? 0M : data.RenewIns.slm_ActVat1PercentBath.Value;

                            var actVat = data.RenewIns.slm_ActVat == null ? 0M : data.RenewIns.slm_ActVat.Value;
                            var actTax = data.RenewIns.slm_ActStamp == null ? 0M : data.RenewIns.slm_ActStamp.Value;

                            txtActGrossPremiumTotal.Text = (ActGrossPremiumTotal + actVat + actTax - Vat1PercentBathAct).ToString("#,##0.00");
                            txtActGrossPremium.Text = data.RenewIns.slm_ActGrossPremium == null ? "0.00" : data.RenewIns.slm_ActGrossPremium.Value.ToString("#,##0.00");

                            txtDiscountPercentAct.Text = data.RenewIns.slm_ActDiscountPercent == null ? "0" : data.RenewIns.slm_ActDiscountPercent.Value.ToString("#,##0");
                            txtActDiscountAmt.Text = data.RenewIns.slm_ActDiscountAmt == null ? "0.00" : data.RenewIns.slm_ActDiscountAmt.Value.ToString("#,##0.00");
                            txtActRecAmount.Text = data.ActRecAmt == null ? "0.00" : data.ActRecAmt.Value.ToString("#,##0.00");
                            txtActDiffAmt.Text = (AppUtil.SafeDecimal(txtActGrossPremium.Text) - AppUtil.SafeDecimal(txtActRecAmount.Text)).ToString("#,##0.00");

                            cmbActPayOption.SelectedValue = data.RenewIns.slm_ActPayOptionId == null || data.RenewIns.slm_ActPayOptionId == 0 ? "" : data.RenewIns.slm_ActPayOptionId.Value.ToString();
                            cmbActPayMethod.SelectedValue = data.RenewIns.slm_ActPayMethodId == null || data.RenewIns.slm_ActPayMethodId == 0 ? "" : data.RenewIns.slm_ActPayMethodId.Value.ToString();
                            cmbActPayBranchCode.SelectedValue = data.RenewIns.slm_PayBranchCode;

                            txtActRecAmount.Text = data.ActRecAmt == null ? "0.00" : data.ActRecAmt.Value.ToString("#,##0.00");
                            txtActDiffAmt.Text = (AppUtil.SafeDecimal(txtActGrossPremium.Text) - AppUtil.SafeDecimal(txtActRecAmount.Text)).ToString("#,##0.00");

                            if (data.PayMainList != null)
                            {
                                foreach (RenewInsurancePaymentMainData paymain in data.PayMainList)
                                {
                                    ((HtmlTableRow)this.FindControlRecursive("trPay" + j)).Visible = true;
                                    ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + j)).Enabled = true;
                                    ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + j)).Text = paymain.slm_PaymentAmount == null || paymain.slm_PaymentAmount == 0 ? "" : paymain.slm_PaymentAmount.Value.ToString("#,##0.00");
                                    ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + j)).Enabled = true;
                                    ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + j)).DateValue = paymain.slm_PaymentDate == null ? DateTime.MinValue : paymain.slm_PaymentDate.Value;

                                    j++;
                                }
                            }
                            for (int i = j; i <= 10; i++)
                            {
                                ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                                ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                                ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                                ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                                ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                            }
                        }

                        txtInsDesc.Text = data.RenewIns.slm_RemarkPolicy;
                        txtPaymentDesc.Text = data.RenewIns.slm_RemarkPayment;
                        txtRemarkAct.Text = data.RenewIns.slm_RemarkAct;

                        //Incentive
                        txtReceiveNo.Text = data.RenewIns.slm_ReceiveNo == null ? "" : data.RenewIns.slm_ReceiveNo;
                        txtReceiveDate.Text = data.RenewIns.slm_ReceiveDate == null ? "" : ConvertToString(data.RenewIns.slm_ReceiveDate.Value);                     
                        txtIncentiveDate.Text = data.RenewIns.slm_IncentiveDate == null ? "" : ConvertToString(data.RenewIns.slm_IncentiveDate.Value);
                        txtActSendDate.Text = data.RenewIns.slm_ActSendDate == null ? "" : ConvertToString(data.RenewIns.slm_ActSendDate.Value);
                        txtActIncentiveDate.Text = data.RenewIns.slm_ActIncentiveDate == null ? "" : ConvertToString(data.RenewIns.slm_ActIncentiveDate.Value);
                        BindClaim(data);

                        if (data.PaymentTransMainList != null)
                        {
                            if (data.PaymentTransMainList.Count() > 0)
                            {
                                cmbActPayOption.SelectedValue = data.PaymentTransMainList[0].slm_ActPayMethodId == null || data.PaymentTransMainList[0].slm_ActPayMethodId == 0 ? "" : data.PaymentTransMainList[0].slm_ActPayMethodId.ToString();
                                cmbActPayMethod.SelectedValue = data.PaymentTransMainList[0].slm_ActPayOptionId == null || data.PaymentTransMainList[0].slm_ActPayOptionId == 0 ? "" : data.PaymentTransMainList[0].slm_ActPayOptionId.ToString(); ;
                                cmbActPayBranchCode.SelectedValue = data.PaymentTransMainList[0].slm_ActPayBranchCode;
                            }
                        }

                        if (data.ProblemList != null)
                        {
                            gvProblem.DataSource = data.ProblemList;
                            gvProblem.DataBind();
                        }

                        //receipt
                        if (data.ReceiptList != null)
                        {
                            gvReceipt.DataSource = data.ReceiptList;
                            gvReceipt.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void checkClaim(ref PreleadCompareDataCollection current)
        {
            //bool Receipt = ActivityLeadBiz.checkPaidPolicy(current.RenewIns.slm_TicketId);
            if (PolicyCheckPaid == null)
            {
                PolicyCheckPaid = ActivityLeadBiz.checkPaidPolicy(current.RenewIns.slm_TicketId);
            }

            if (PolicyCheckPaid.Value && current.RenewIns.slm_ClaimFlag == "1")
            {   // ชำระเงินอยู่ในช่วง config และโดนระงับเคลมอยู่ ให้ "ยกเลิกระงับเคลม"
                current.RenewIns.slm_ClaimFlag = "0";
            }
        }

        private void BindClaim(PreleadCompareDataCollection data)
        {
            // หน้าขาย Type = 3 ต้องไม่มีส่วนระงับเคลม
            if (Type == "3")
            {
                pnClaim.Visible = false;
            }

            radioClaim.Enabled = true;

            if (data.RenewIns.slm_ClaimFlag == null)
            {
                radioClaim.SelectedIndex = 0;
            }
            else
            {
                radioClaim.SelectedValue = data.RenewIns.slm_ClaimFlag;
            }
                
            // enable ถ้ามีเลขที่รับแจ้งแล้ว
            radioClaim.Enabled = data.RenewIns.slm_ReceiveNo != null && data.RenewIns.slm_ReceiveNo != "";

            //4.	กรณีที่ 4 มีการชำระเงินเข้ามา, ส่วนต่างไม่เกินค่า Config หรือมีการ Adjust ส่วนลด ที่หน้าเว็บให้ส่วนต่างไม่เกินค่า Config ,  มีเลขรับแจ้งแล้ว และ มี Flag ระงับเคลม
            //bool policyPaid = ActivityLeadBiz.checkPaid(data.RenewIns.slm_TicketId);
            if (PolicyCheckPaid == null)
            {
                PolicyCheckPaid = ActivityLeadBiz.checkPaid(data.RenewIns.slm_TicketId);
            }
            if (PolicyCheckPaid.Value)
            {
                radioClaim.Enabled = radioClaim.Enabled || false;
            }

            // check payment act 
            //bool actPaid = ActivityLeadBiz.checkPaidAct(data.RenewIns.slm_TicketId);
            if (ActCheckPaid == null)
            {
                ActCheckPaid = ActivityLeadBiz.checkPaidAct(data.RenewIns.slm_TicketId);
            }
            if ((data.RenewIns.slm_ActPurchaseFlag ?? false) && !ActCheckPaid.Value)
            {
                radioClaim.Enabled = radioClaim.Enabled || false;
            }

            // check ถ้าออกรายงานระงับเคลมแล้ว ให้ freeze radio
            if (ActivityLeadBiz.CheckSettleClaimReported(data.RenewIns.slm_TicketId))
            {
                radioClaim.Enabled = false;
            }
                
            // ถ้า "ไม่ระงับเคลม" จากการยกเลิกประกัน => Freeze radio
            if (data.RenewIns.slm_PolicyCancelDate != null && data.RenewIns.slm_PolicyCancelId != null
                && data.RenewIns.slm_ReceiveNo == null)
            {
                radioClaim.Enabled = false;
            }
                
            if (radioClaim.SelectedValue == Resource.SLMConstant.SettleClaimStatus.CancelSettleClaim)
            {
                radioClaim.Enabled = false;
            }

            //UpdateStatusDesc("", new DateTime());     byDev
        }

        private string ConvertToString(DateTime datetime)
        {
            if (datetime != null && datetime.Year != 1)
            {
                return datetime.ToString("dd/MM/") + datetime.Year.ToString() + " " + datetime.ToString("HH:mm:ss");
            }
            else
            {
                return "";
            }
        }

        private void getImportList(string id)
        {
            hidRenewInsuranceReceiptId.Value = id;
            List<RenewInsuranceReceiptDetailData> ImportList = ActivityLeadBiz.GetImportList(id);

            if (ImportList != null)
            {
                gvImport.DataSource = ImportList;
                gvImport.DataBind();
            }
        }

        private void getReceiptList(string id)
        {
            try
            {
                List<RenewInsuranceReceiptDetailData> ReceiptList = ActivityLeadBiz.getReceiptDetail(id);
                hidRenewInsuranceReceiptId.Value = id;
                if (ReceiptList != null)
                {
                    var i = 1;
                    foreach (RenewInsuranceReceiptDetailData r in ReceiptList)
                    {
                        ((TextBox)this.FindControlRecursive("txtPaymentDesc" + i)).Text = r.slm_RecAmount == null ? "" : r.slm_RecAmount.Value.ToString("#,##0.00");

                        if (r.slm_Seq == 6)
                        {
                            ((TextBox)this.FindControlRecursive("txtPaymentOther")).Text = r.slm_PaymentOtherDesc == null ? "" : r.slm_PaymentOtherDesc;
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void BuildCombo(DropDownList dd, List<ControlListData> data, string defaulttext = "", string defaultval = "")
        {
            data.Insert(0, new ControlListData() { TextField = defaulttext, ValueField = defaultval });
            dd.DataSource = data;
            dd.DataTextField = "TextField";
            dd.DataValueField = "ValueField";
            dd.DataBind();
        }

        private void bindDropDownList()
        {
            try
            {
                var carBrandList = BrandCarBiz.ListBrandCar();
                cmbCarBrand.DataSource = carBrandList.ToList();
                cmbCarBrand.DataTextField = "BrandName";
                cmbCarBrand.DataValueField = "BrandCode";
                cmbCarBrand.DataBind();
                cmbCarBrand.Items.Insert(0, new ListItem("", ""));

                cmbInsuranceCarType.DataSource = SlmScr046Biz.GetInsuranceTypeData();
                cmbInsuranceCarType.DataTextField = "TextField";
                cmbInsuranceCarType.DataValueField = "ValueField";
                cmbInsuranceCarType.DataBind();
                cmbInsuranceCarType.Items.Insert(0, new ListItem("", ""));

                cmbProvinceRegis.DataSource = SlmScr010Biz.GetProvinceDataNew();
                cmbProvinceRegis.DataTextField = "TextField";
                cmbProvinceRegis.DataValueField = "ValueField";
                cmbProvinceRegis.DataBind();
                cmbProvinceRegis.Items.Insert(0, new ListItem("", ""));

                cmbBeneficiary.DataSource = BeneficiaryBiz.GetBeneficiaryList();
                cmbBeneficiary.DataTextField = "TextField";
                cmbBeneficiary.DataValueField = "ValueField";
                cmbBeneficiary.DataBind();
                cmbBeneficiary.Items.Insert(0, new ListItem("ไม่ระบุ", ""));

                var coveragetypelist = CoverageTypeBiz.GetCoverageTypeList();

                lblCoverageType_cur.DataSource = coveragetypelist.ToList();
                lblCoverageType_cur.DataTextField = "TextField";
                lblCoverageType_cur.DataValueField = "ValueField";
                lblCoverageType_cur.DataBind();
                lblCoverageType_cur.Items.Insert(0, new ListItem("", ""));

                lblCoverageType_pro1.DataSource = coveragetypelist.ToList();
                lblCoverageType_pro1.DataTextField = "TextField";
                lblCoverageType_pro1.DataValueField = "ValueField";
                lblCoverageType_pro1.DataBind();
                lblCoverageType_pro1.Items.Insert(0, new ListItem("", ""));

                lblCoverageType_pro2.DataSource = coveragetypelist.ToList();
                lblCoverageType_pro2.DataTextField = "TextField";
                lblCoverageType_pro2.DataValueField = "ValueField";
                lblCoverageType_pro2.DataBind();
                lblCoverageType_pro2.Items.Insert(0, new ListItem("", ""));

                lblCoverageType_pro3.DataSource = coveragetypelist.ToList();
                lblCoverageType_pro3.DataTextField = "TextField";
                lblCoverageType_pro3.DataValueField = "ValueField";
                lblCoverageType_pro3.DataBind();
                lblCoverageType_pro3.Items.Insert(0, new ListItem("", ""));

                bindDropDownListCompany();

                //var Title Name
                var listtitlename = TitleNameBiz.ListTitleName();

                cmbTitleName1_cur.DataSource = listtitlename.ToList();
                cmbTitleName1_cur.DataTextField = "TitleName";
                cmbTitleName1_cur.DataValueField = "TitleId";
                cmbTitleName1_cur.DataBind();
                cmbTitleName1_cur.Items.Insert(0, new ListItem("", ""));

                cmbTitleName2_cur.DataSource = listtitlename.ToList();
                cmbTitleName2_cur.DataTextField = "TitleName";
                cmbTitleName2_cur.DataValueField = "TitleId";
                cmbTitleName2_cur.DataBind();
                cmbTitleName2_cur.Items.Insert(0, new ListItem("", ""));

                cmbTitleName1_pro1.DataSource = listtitlename.ToList();
                cmbTitleName1_pro1.DataTextField = "TitleName";
                cmbTitleName1_pro1.DataValueField = "TitleId";
                cmbTitleName1_pro1.DataBind();
                cmbTitleName1_pro1.Items.Insert(0, new ListItem("", ""));

                cmbTitleName1_pro2.DataSource = listtitlename.ToList();
                cmbTitleName1_pro2.DataTextField = "TitleName";
                cmbTitleName1_pro2.DataValueField = "TitleId";
                cmbTitleName1_pro2.DataBind();
                cmbTitleName1_pro2.Items.Insert(0, new ListItem("", ""));

                cmbTitleName1_pro3.DataSource = listtitlename.ToList();
                cmbTitleName1_pro3.DataTextField = "TitleName";
                cmbTitleName1_pro3.DataValueField = "TitleId";
                cmbTitleName1_pro3.DataBind();
                cmbTitleName1_pro3.Items.Insert(0, new ListItem("", ""));

                cmbTitleName2_pro1.DataSource = listtitlename.ToList();
                cmbTitleName2_pro1.DataTextField = "TitleName";
                cmbTitleName2_pro1.DataValueField = "TitleId";
                cmbTitleName2_pro1.DataBind();
                cmbTitleName2_pro1.Items.Insert(0, new ListItem("", ""));

                cmbTitleName2_pro2.DataSource = listtitlename.ToList();
                cmbTitleName2_pro2.DataTextField = "TitleName";
                cmbTitleName2_pro2.DataValueField = "TitleId";
                cmbTitleName2_pro2.DataBind();
                cmbTitleName2_pro2.Items.Insert(0, new ListItem("", ""));

                cmbTitleName2_pro3.DataSource = listtitlename.ToList();
                cmbTitleName2_pro3.DataTextField = "TitleName";
                cmbTitleName2_pro3.DataValueField = "TitleId";
                cmbTitleName2_pro3.DataBind();
                cmbTitleName2_pro3.Items.Insert(0, new ListItem("", ""));

                lblCoverageType2_cur.DataSource = coveragetypelist.ToList();
                lblCoverageType2_cur.DataTextField = "TextField";
                lblCoverageType2_cur.DataValueField = "ValueField";
                lblCoverageType2_cur.DataBind();
                lblCoverageType2_cur.Items.Insert(0, new ListItem("", ""));

                lblCoverageType2_pro1.DataSource = coveragetypelist.ToList();
                lblCoverageType2_pro1.DataTextField = "TextField";
                lblCoverageType2_pro1.DataValueField = "ValueField";
                lblCoverageType2_pro1.DataBind();
                lblCoverageType2_pro1.Items.Insert(0, new ListItem("", ""));

                lblCoverageType2_pro2.DataSource = coveragetypelist.ToList();
                lblCoverageType2_pro2.DataTextField = "TextField";
                lblCoverageType2_pro2.DataValueField = "ValueField";
                lblCoverageType2_pro2.DataBind();
                lblCoverageType2_pro2.Items.Insert(0, new ListItem("", ""));

                lblCoverageType2_pro3.DataSource = coveragetypelist.ToList();
                lblCoverageType2_pro3.DataTextField = "TextField";
                lblCoverageType2_pro3.DataValueField = "ValueField";
                lblCoverageType2_pro3.DataBind();
                lblCoverageType2_pro3.Items.Insert(0, new ListItem("", ""));

                var repairTypeList = RepairTypeBiz.GetRepairTypeList();

                lblMaintanance_cur.DataSource = repairTypeList.ToList();
                lblMaintanance_cur.DataTextField = "TextField";
                lblMaintanance_cur.DataValueField = "ValueField";
                lblMaintanance_cur.DataBind();
                lblMaintanance_cur.Items.Insert(0, new ListItem("", ""));

                lblMaintanance_pro1.DataSource = repairTypeList.ToList();
                lblMaintanance_pro1.DataTextField = "TextField";
                lblMaintanance_pro1.DataValueField = "ValueField";
                lblMaintanance_pro1.DataBind();
                lblMaintanance_pro1.Items.Insert(0, new ListItem("", ""));

                lblMaintanance_pro2.DataSource = repairTypeList.ToList();
                lblMaintanance_pro2.DataTextField = "TextField";
                lblMaintanance_pro2.DataValueField = "ValueField";
                lblMaintanance_pro2.DataBind();
                lblMaintanance_pro2.Items.Insert(0, new ListItem("", ""));

                lblMaintanance_pro3.DataSource = repairTypeList.ToList();
                lblMaintanance_pro3.DataTextField = "TextField";
                lblMaintanance_pro3.DataValueField = "ValueField";
                lblMaintanance_pro3.DataBind();
                lblMaintanance_pro3.Items.Insert(0, new ListItem("", ""));

                var PaymentMethodList = PaymentMethodBiz.GetPaymentMethodList();
                // GetPaymentMethodList
                if (Type == "1")
                {
                    cmbPaymentmethod.DataSource = PaymentMethodList.ToList();
                }
                else
                {
                    cmbPaymentmethod.DataSource = PaymentMethodList.Where(l => l.TextField != "ผ่อนชำระ").ToList();
                }
                cmbPaymentmethod.DataTextField = "TextField";
                cmbPaymentmethod.DataValueField = "ValueField";
                cmbPaymentmethod.DataBind();
                cmbPaymentmethod.Items.Insert(0, new ListItem("", ""));

                cmbPaymentmethodAct.DataSource = PaymentMethodList.Where(l => l.TextField != "ผ่อนชำระ").ToList();
                cmbPaymentmethodAct.DataTextField = "TextField";
                cmbPaymentmethodAct.DataValueField = "ValueField";
                cmbPaymentmethodAct.DataBind();
                cmbPaymentmethodAct.Items.Insert(0, new ListItem("", ""));

                // GetPaymentTypeList Policy
                if (Type == "1")
                {
                    cmbPolicyPayMethod.DataSource = PaymentMethodList.ToList();
                }
                else
                {
                    cmbPolicyPayMethod.DataSource = PaymentMethodList.Where(l => l.TextField != "ผ่อนชำระ").ToList();
                }

                cmbPolicyPayMethod.DataTextField = "TextField";
                cmbPolicyPayMethod.DataValueField = "ValueField";
                cmbPolicyPayMethod.DataBind();
                cmbPolicyPayMethod.Items.Insert(0, new ListItem("", ""));

                cmbActPayOption.DataSource = PaymentMethodList.Where(l => l.TextField != "ผ่อนชำระ").ToList();

                cmbActPayOption.DataTextField = "TextField";
                cmbActPayOption.DataValueField = "ValueField";
                cmbActPayOption.DataBind();
                cmbActPayOption.Items.Insert(0, new ListItem("", ""));

                var PaymentOptionList = PaymentOptionBiz.GetPaymentOptionList();
                cmbPaymentType.DataSource = PaymentOptionList.ToList();
                cmbPaymentType.DataTextField = "TextField";
                cmbPaymentType.DataValueField = "ValueField";
                cmbPaymentType.DataBind();
                cmbPaymentType.Items.Insert(0, new ListItem("", ""));

                cmbPaymentTypeAct.DataSource = PaymentOptionList.ToList();
                cmbPaymentTypeAct.DataTextField = "TextField";
                cmbPaymentTypeAct.DataValueField = "ValueField";
                cmbPaymentTypeAct.DataBind();
                cmbPaymentTypeAct.Items.Insert(0, new ListItem("", ""));

                // GetPaymentTypeList Policy
                cmbActPayMethod.DataSource = PaymentOptionList.ToList();
                cmbActPayMethod.DataTextField = "TextField";
                cmbActPayMethod.DataValueField = "ValueField";
                cmbActPayMethod.DataBind();
                cmbActPayMethod.Items.Insert(0, new ListItem("", ""));

                // GetPaymentMethodList  policy
                cmbPayOption.DataSource = PaymentOptionList.ToList();
                cmbPayOption.DataTextField = "TextField";
                cmbPayOption.DataValueField = "ValueField";
                cmbPayOption.DataBind();
                cmbPayOption.Items.Insert(0, new ListItem("", ""));

                cmbCarBrand2.DataSource = carBrandList.ToList();
                cmbCarBrand2.DataTextField = "BrandName";
                cmbCarBrand2.DataValueField = "BrandCode";
                cmbCarBrand2.DataBind();
                cmbCarBrand2.Items.Insert(0, new ListItem("", ""));

                cmbInsuranceType.DataSource = PromotionBiz.ListInsuranceName();
                cmbInsuranceType.DataTextField = "InsuranceName";
                cmbInsuranceType.DataValueField = "InsuranceValue";
                cmbInsuranceType.DataBind();
                cmbInsuranceType.Items.Insert(0, new ListItem("", ""));

                var BranchList = SlmScr999Biz.ListBranchName();
                var branchCode = slm_BranchId.Value;

                //add by nung 20170505
                if (!string.IsNullOrEmpty(branchCode))
                {
                    if (!BranchList.Any(i => i.BranchCode == branchCode))
                    {
                        var baranchName = SlmScr999Biz.GetBranchName(branchCode);
                        BranchList.Add(new ActData
                        {
                            BranchCode = branchCode,
                            BranchName = baranchName
                        });
                    }
                }
                
                cmbPolicyPayBranchCode.DataSource = BranchList.ToList();
                cmbPolicyPayBranchCode.DataTextField = "BranchName";
                cmbPolicyPayBranchCode.DataValueField = "BranchCode";
                cmbPolicyPayBranchCode.DataBind();
                cmbPolicyPayBranchCode.Items.Insert(0, new ListItem("", ""));

                cmbPayBranchCodeAct.DataSource = BranchList.ToList();
                cmbPayBranchCodeAct.DataTextField = "BranchName";
                cmbPayBranchCodeAct.DataValueField = "BranchCode";
                cmbPayBranchCodeAct.DataBind();
                cmbPayBranchCodeAct.Items.Insert(0, new ListItem("", ""));
                
                cmbActIssueBranch_pro1.DataSource = BranchList.ToList();
                cmbActIssueBranch_pro1.DataTextField = "BranchName";
                cmbActIssueBranch_pro1.DataValueField = "BranchCode";
                cmbActIssueBranch_pro1.DataBind();
                cmbActIssueBranch_pro1.Items.Insert(0, new ListItem("", ""));

                cmbActIssueBranch_pro2.DataSource = BranchList.ToList();
                cmbActIssueBranch_pro2.DataTextField = "BranchName";
                cmbActIssueBranch_pro2.DataValueField = "BranchCode";
                cmbActIssueBranch_pro2.DataBind();
                cmbActIssueBranch_pro2.Items.Insert(0, new ListItem("", ""));

                cmbActIssueBranch_pro3.DataSource = BranchList.ToList();
                cmbActIssueBranch_pro3.DataTextField = "BranchName";
                cmbActIssueBranch_pro3.DataValueField = "BranchCode";
                cmbActIssueBranch_pro3.DataBind();
                cmbActIssueBranch_pro3.Items.Insert(0, new ListItem("", ""));

                cmbBranchCodeDoc.DataSource = BranchList.ToList();
                cmbBranchCodeDoc.DataTextField = "BranchName";
                cmbBranchCodeDoc.DataValueField = "BranchCode";
                cmbBranchCodeDoc.DataBind();
                cmbBranchCodeDoc.Items.Insert(0, new ListItem("", ""));

                cmbPayBranchCode.DataSource = BranchList.ToList();
                cmbPayBranchCode.DataTextField = "BranchName";
                cmbPayBranchCode.DataValueField = "BranchCode";
                cmbPayBranchCode.DataBind();
                cmbPayBranchCode.Items.Insert(0, new ListItem("", ""));

                cmbActPayBranchCode.DataSource = BranchList.ToList();
                cmbActPayBranchCode.DataTextField = "BranchName";
                cmbActPayBranchCode.DataValueField = "BranchCode";
                cmbActPayBranchCode.DataBind();
                cmbActPayBranchCode.Items.Insert(0, new ListItem("", ""));

                cmbProvince.DataSource = SlmScr999Biz.ListProvince();
                cmbProvince.DataTextField = "ProvinceNameTh";
                cmbProvince.DataValueField = "ProvinceId";
                cmbProvince.DataBind();
                cmbProvince.Items.Insert(0, new ListItem("", ""));

                //var InsNameThList = SlmScr999Biz.ListInsNameTh();
                var CancelList = ActivityLeadBiz.getCancelList();

                cmbCancelReason.DataSource = CancelList;
                cmbCancelReason.DataTextField = "TextField";
                cmbCancelReason.DataValueField = "ValueField";
                cmbCancelReason.DataBind();
                cmbCancelReason.Items.Insert(0, new ListItem("กรุณาระบุ", ""));

                cmbPolicyCancelReason.DataSource = CancelList;
                cmbPolicyCancelReason.DataTextField = "TextField";
                cmbPolicyCancelReason.DataValueField = "ValueField";
                cmbPolicyCancelReason.DataBind();
                cmbPolicyCancelReason.Items.Insert(0, new ListItem("", ""));

                cmbActCancelReason.DataSource = CancelList;
                cmbActCancelReason.DataTextField = "TextField";
                cmbActCancelReason.DataValueField = "ValueField";
                cmbActCancelReason.DataBind();
                cmbActCancelReason.Items.Insert(0, new ListItem("", ""));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void bindDropDownListCompany()
        {
            // แยก function ออกมาเพื่อจะใช้ re-bound ตอน save  เพื่อ clear บ.ประกันที่ inactive - OuMz 2017-02-16
            var InsComList = InsComBiz.GetInsComList();

            lblInsNameTh_cur.DataSource = InsComList.ToList();
            lblInsNameTh_cur.DataTextField = "TextField";
            lblInsNameTh_cur.DataValueField = "ValueField";
            lblInsNameTh_cur.DataBind();
            lblInsNameTh_cur.Items.Insert(0, new ListItem("", ""));

            lblInsNameTh_pro1.DataSource = InsComList.ToList();
            lblInsNameTh_pro1.DataTextField = "TextField";
            lblInsNameTh_pro1.DataValueField = "ValueField";
            lblInsNameTh_pro1.DataBind();
            lblInsNameTh_pro1.Items.Insert(0, new ListItem("", ""));

            lblInsNameTh_pro2.DataSource = InsComList.ToList();
            lblInsNameTh_pro2.DataTextField = "TextField";
            lblInsNameTh_pro2.DataValueField = "ValueField";
            lblInsNameTh_pro2.DataBind();
            lblInsNameTh_pro2.Items.Insert(0, new ListItem("", ""));

            lblInsNameTh_pro3.DataSource = InsComList.ToList();
            lblInsNameTh_pro3.DataTextField = "TextField";
            lblInsNameTh_pro3.DataValueField = "ValueField";
            lblInsNameTh_pro3.DataBind();
            lblInsNameTh_pro3.Items.Insert(0, new ListItem("", ""));
        }

        protected void rbDriver_Flag_Click(object sender, EventArgs e)
        {
            try
            {
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix + "allData"];
                PreleadCompareDataCollection current = new PreleadCompareDataCollection();
                if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                {
                    current = pg.PreleadCompareDataCollectionMain;
                }
                else
                {
                    if (pg.PreleadCompareDataCollections != null)
                    {
                        foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                        {
                            if (p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                                current = p;
                        }
                    }
                }

                setDriverControl(current);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private void setDriverControl(PreleadCompareDataCollection data)
        {
            try
            {
                if (rbDriver_Flag_cur2.Checked)
                {
                    cmbTitleName1_cur.Enabled = false;
                    txtDriver_First_Name1_cur.Enabled = false;
                    txtDriver_Last_Name1_cur.Enabled = false;
                    tdmDriver_Birthdate1_cur.Enabled = false;
                    cmbTitleName2_cur.Enabled = false;
                    txtDriver_First_Name2_cur.Enabled = false;
                    txtDriver_Last_Name2_cur.Enabled = false;
                    tdmDriver_Birthdate2_cur.Enabled = false;

                    cmbTitleName1_cur.SelectedValue = "";
                    txtDriver_First_Name1_cur.Text = "";
                    txtDriver_Last_Name1_cur.Text = "";
                    tdmDriver_Birthdate1_cur.DateValue = DateTime.MinValue;
                    cmbTitleName2_cur.SelectedValue = "";
                    txtDriver_First_Name2_cur.Text = "";
                    txtDriver_Last_Name2_cur.Text = "";
                    tdmDriver_Birthdate2_cur.DateValue = DateTime.MinValue;
                }
                else
                {
                    cmbTitleName1_cur.Enabled = true;
                    txtDriver_First_Name1_cur.Enabled = true;
                    txtDriver_Last_Name1_cur.Enabled = true;
                    tdmDriver_Birthdate1_cur.Enabled = true;
                    cmbTitleName2_cur.Enabled = true;
                    txtDriver_First_Name2_cur.Enabled = true;
                    txtDriver_Last_Name2_cur.Enabled = true;
                    tdmDriver_Birthdate2_cur.Enabled = true;
                }

                bool exportNotifyPolicyReport = false;
                if (txtReceiveNo.Text != "")
                {
                    if (ExportNotifyPolicyReport == null)
                    {
                        ExportNotifyPolicyReport = SlmScr004Biz.ExportNotifyPolicyReport(data.RenewIns.slm_TicketId);
                    }

                    exportNotifyPolicyReport = ExportNotifyPolicyReport.Value;
                }

                List<ProblemDetailData> problem = data.ProblemList;
                bool hasPendingProblem = false;
                if (problem != null && problem
                    .Where(p => p.slm_FixTypeFlag != "2" || (p.slm_FixTypeFlag == "2" && p.slm_Export_Flag.GetValueOrDefault(false) == false)).Count() > 0)
                {
                    hasPendingProblem = true;
                }
                    
                for (int i = 1; i < 4; i++)
                {
                    // rbInsNameTh_pro1
                    bool boughtThisPolicy = false;

                    boughtThisPolicy = ((RadioButton)this.FindControlRecursive("rbInsNameTh_pro" + i)).Checked;
                    if (((RadioButton)this.FindControlRecursive("rbDriver_Flag_pro" + i + "2")).Checked)
                    {
                        ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name1_pro" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name1_pro" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate1_pro" + i)).Enabled = false;
                        ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name2_pro" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name2_pro" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate2_pro" + i)).Enabled = false;


                        ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + i)).SelectedValue = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name1_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name1_pro" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate1_pro" + i)).DateValue = DateTime.MinValue;
                        ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + i)).SelectedValue = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name2_pro" + i)).Text = "";
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name2_pro" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate2_pro" + i)).DateValue = DateTime.MinValue;
                    }
                    else if (boughtThisPolicy && exportNotifyPolicyReport && !hasPendingProblem)
                    {
                        ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name1_pro" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name1_pro" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate1_pro" + i)).Enabled = false;
                        ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name2_pro" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name2_pro" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate2_pro" + i)).Enabled = false;

                    }
                    else
                    {
                        ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + i)).Enabled = true;
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name1_pro" + i)).Enabled = true;
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name1_pro" + i)).Enabled = true;
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate1_pro" + i)).Enabled = true;
                        ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + i)).Enabled = true;
                        ((TextBox)this.FindControlRecursive("txtDriver_First_Name2_pro" + i)).Enabled = true;
                        ((TextBox)this.FindControlRecursive("txtDriver_Last_Name2_pro" + i)).Enabled = true;
                        ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate2_pro" + i)).Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void setAddressControl()
        {
            try
            {
                if (rdoAddressOld.Checked)
                {
                    txtHouseNo.Enabled = false;
                    txtMoo.Enabled = false;
                    txtBuilding.Enabled = false;
                    txtHouseName.Enabled = false;
                    txtSoi.Enabled = false;
                    txtStreet.Enabled = false;
                    cmbProvince.Enabled = false;
                    cmbDistinct.Enabled = false;
                    cmbThambol.Enabled = false;
                    txtZipCode.Enabled = false;

                    cmbBranchCodeDoc.Enabled = false;
                    cmbBranchCodeDoc.SelectedIndex = 0;

                    PreleadAddressData Address = null;
                    PreleadCompareDataCollection pg = getSelectTabData();
                    if (PreLeadId != "")
                    {
                        Address = ActivityPreLeadBiz.GetPreleadAddress((int)pg.Prelead.slm_Prelead_Id.Value);
                    }
                    else
                    {
                        Address = ActivityLeadBiz.GetPreleadAddress(pg.RenewIns.slm_TicketId);
                    }

                    bindPreleadAddressOld(Address);
                    if (txtReceiver.Text.Trim() == "" || txtReceiver.Text.Trim().StartsWith(kiatnakinBank))
                    {
                        txtReceiver.Text = hdClientFullname.Value;
                    }
                }
                else if (rdoAddressChange.Checked)
                {
                    txtHouseNo.Enabled = true;
                    txtMoo.Enabled = true;
                    txtBuilding.Enabled = true;
                    txtHouseName.Enabled = true;
                    txtSoi.Enabled = true;
                    txtStreet.Enabled = true;
                    cmbProvince.Enabled = true;
                    cmbDistinct.Enabled = true;
                    cmbThambol.Enabled = true;
                    txtZipCode.Enabled = true;

                    cmbBranchCodeDoc.Enabled = false;
                    cmbBranchCodeDoc.SelectedIndex = 0;

                    if (txtReceiver.Text.Trim() == "" || txtReceiver.Text.Trim().StartsWith(kiatnakinBank))
                    {
                        txtReceiver.Text = hdClientFullname.Value;
                    }
                }
                else if (rdoAddressBranch.Checked)
                {
                    txtHouseNo.Enabled = false;
                    txtMoo.Enabled = false;
                    txtBuilding.Enabled = false;
                    txtHouseName.Enabled = false;
                    txtSoi.Enabled = false;
                    txtStreet.Enabled = false;

                    cmbProvince.Enabled = false;
                    cmbDistinct.Enabled = false;
                    cmbThambol.Enabled = false;
                    txtZipCode.Enabled = false;

                    cmbBranchCodeDoc.Enabled = true;

                    txtHouseNo.Text = "";
                    txtMoo.Text = "";
                    txtBuilding.Text = "";
                    txtHouseName.Text = "";
                    txtSoi.Text = "";
                    txtStreet.Text = "";

                    cmbProvince.SelectedIndex = 0;
                    if (cmbDistinct.Items.Count > 1)
                    {
                        cmbDistinct.SelectedIndex = 0;
                    }
                    if (cmbDistinct.Items.Count > 1)
                    {
                        cmbThambol.SelectedIndex = 0;
                    }
                    txtZipCode.Text = "";

                    if (!txtReceiver.Text.Trim().StartsWith(kiatnakinBank))
                    {
                        txtReceiver.Text = kiatnakinBank;
                    }
                }
                pnDocument.Update();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void GetResultActBuy(string ticketId)
        {
            try
            {
                var count = ActivityLeadBiz.GetResultActBuy(ticketId);

                if (count > 0)
                {
                    txtPolicyGrossPremium.Enabled = false;
                    txtPolicyRecAmount.Enabled = false;

                    txtDiscountPercent.Enabled = false;
                    txtPolicyDiscountAmt.Enabled = false;
                    txtPolicyDiffAmt.Enabled = false;
                    cmbPolicyPayMethod.Enabled = true;
                    txtPolicyAmountPeriod.Enabled = true;
                    cmbPayOption.Enabled = true;
                    cmbPayBranchCode.Enabled = true;
                    txtActGrossPremium.Enabled = false;
                    txtActRecAmount.Enabled = false;
                    txtActDiffAmt.Enabled = false;
                    cmbActPayOption.Enabled = true;
                    cmbActPayMethod.Enabled = true;
                    if (cmbActPayMethod.SelectedValue == "4")
                    {
                        cmbActPayBranchCode.Enabled = true;
                    }
                    else
                    {
                        cmbActPayBranchCode.Enabled = false;
                    }
                }
                else
                {
                    txtPolicyGrossPremium.Enabled = false;
                    txtPolicyRecAmount.Enabled = false;
                    txtDiscountPercent.Enabled = false;
                    txtPolicyDiscountAmt.Enabled = false;
                    txtPolicyDiffAmt.Enabled = false;
                    cmbPolicyPayMethod.Enabled = true;
                    txtPolicyAmountPeriod.Enabled = true;
                    cmbPayOption.Enabled = true;
                    cmbPayBranchCode.Enabled = true;
                    txtActGrossPremium.Enabled = false;
                    txtActRecAmount.Enabled = false;
                    txtActDiffAmt.Enabled = false;
                    cmbActPayOption.Enabled = true;
                    cmbActPayMethod.Enabled = true;
                    cmbActPayBranchCode.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void disableReceiptPolicy(PreleadCompareDataCollection compareCollection, string receiveNo)
        {
            RenewInsuranceData data = compareCollection.RenewIns;
            SlmScr004Biz biz = new SlmScr004Biz();
            //bool exportNotifyPolicyReport = SlmScr004Biz.ExportNotifyPolicyReport(data.slm_TicketId);

            bool exportNotifyPolicyReport;
            if (ExportNotifyPolicyReport == null)
            {
                ExportNotifyPolicyReport = SlmScr004Biz.ExportNotifyPolicyReport(data.slm_TicketId);
            }
            exportNotifyPolicyReport = ExportNotifyPolicyReport.Value;

            bool enabled0 = !exportNotifyPolicyReport ? true : !rbInsNameTh_cur.Checked;
            bool enabled1 = !exportNotifyPolicyReport ? true : !rbInsNameTh_pro1.Checked;
            bool enabled2 = !exportNotifyPolicyReport ? true : !rbInsNameTh_pro2.Checked;
            bool enabled3 = !exportNotifyPolicyReport ? true : !rbInsNameTh_pro3.Checked;
            bool policy0Promotion = true; // current year => เช็คเหมือนมีโปรโมชั่น            
            bool policy1Promotion = compareCollection.ComparePromoList.Count > 0
               && compareCollection.ComparePromoList[0].slm_PromotionId != null && compareCollection.ComparePromoList[0].slm_PromotionId != 0;

            bool policy2Promotion = compareCollection.ComparePromoList.Count > 1
                && compareCollection.ComparePromoList[1].slm_PromotionId != null && compareCollection.ComparePromoList[1].slm_PromotionId != 0;

            bool policy3Promotion = compareCollection.ComparePromoList.Count > 2
                && compareCollection.ComparePromoList[2].slm_PromotionId != null && compareCollection.ComparePromoList[2].slm_PromotionId != 0;

            bool receiveNoIssued = compareCollection.RenewIns.slm_ReceiveDate != null;
            bool enabledWithPromotion0 = enabled0 && !policy0Promotion && receiveNoIssued;

            if (String.IsNullOrEmpty(receiveNo))
            {
                rbInsNameTh_cur.Enabled = (hidNotifyPremiumId.Value != "" && hidNotifyPremiumId.Value != "0");
                rbInsNameTh_pro1.Enabled = true;
                rbInsNameTh_pro2.Enabled = true;
                rbInsNameTh_pro3.Enabled = true;
            }
            else
            {
                rbInsNameTh_cur.Enabled = false;
                rbInsNameTh_pro1.Enabled = false;
                rbInsNameTh_pro2.Enabled = false;
                rbInsNameTh_pro3.Enabled = false;
            }

            lblCoverageType_pro1.Enabled = !(policy1Promotion || (!policy1Promotion && rbInsNameTh_pro1.Checked && receiveNoIssued));
            lblCoverageType_pro2.Enabled = !(policy2Promotion || (!policy2Promotion && rbInsNameTh_pro2.Checked && receiveNoIssued));
            lblCoverageType_pro3.Enabled = !(policy3Promotion || (!policy3Promotion && rbInsNameTh_pro3.Checked && receiveNoIssued));

            lblInsNameTh_pro1.Enabled = !(policy1Promotion || (!policy1Promotion && rbInsNameTh_pro1.Checked && receiveNoIssued));
            lblInsNameTh_pro2.Enabled = !(policy2Promotion || (!policy2Promotion && rbInsNameTh_pro2.Checked && receiveNoIssued));
            lblInsNameTh_pro3.Enabled = !(policy3Promotion || (!policy3Promotion && rbInsNameTh_pro3.Checked && receiveNoIssued));

            rbDriver_Flag_cur1.Enabled = enabled0 && hidHasCurrentYearPolicy.Value == "true";
            rbDriver_Flag_cur2.Enabled = enabled0 && hidHasCurrentYearPolicy.Value == "true";
            rbDriver_Flag_pro11.Enabled = enabled1;
            rbDriver_Flag_pro12.Enabled = enabled1;
            rbDriver_Flag_pro21.Enabled = enabled2;
            rbDriver_Flag_pro22.Enabled = enabled2;
            rbDriver_Flag_pro31.Enabled = enabled3;
            rbDriver_Flag_pro32.Enabled = enabled3;

            if (exportNotifyPolicyReport)
            {
                int index = 0;
                if (rbInsNameTh_pro1.Checked)
                {
                    index = 1;
                }                  
                else if (rbInsNameTh_pro2.Checked)
                {
                    index = 2;
                }                  
                else if (rbInsNameTh_pro3.Checked)
                {
                    index = 3;
                }

                if (index > 0)
                {
                    ((DropDownList)this.FindControlRecursive("cmbTitleName1_pro" + index)).Enabled = false;
                    ((TextBox)this.FindControlRecursive("txtDriver_First_Name1_pro" + index)).Enabled = false;
                    ((TextBox)this.FindControlRecursive("txtDriver_Last_Name1_pro" + index)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate1_pro" + index)).Enabled = false;

                    ((DropDownList)this.FindControlRecursive("cmbTitleName2_pro" + index)).Enabled = false;
                    ((TextBox)this.FindControlRecursive("txtDriver_First_Name2_pro" + index)).Enabled = false;
                    ((TextBox)this.FindControlRecursive("txtDriver_Last_Name2_pro" + index)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmDriver_Birthdate2_pro" + index)).Enabled = false;
                }
            }
            
            lblVoluntary_Policy_Eff_Date_cur.Enabled = enabled0 && hidHasCurrentYearPolicy.Value == "true";
            lblVoluntary_Policy_Eff_Date_pro1.Enabled = enabled1;
            lblVoluntary_Policy_Eff_Date_pro2.Enabled = enabled2;
            lblVoluntary_Policy_Eff_Date_pro3.Enabled = enabled3;

            lblVoluntary_Policy_Exp_Date_cur.Enabled = enabled0 && hidHasCurrentYearPolicy.Value == "true";
            lblVoluntary_Policy_Exp_Date_pro1.Enabled = enabled1;
            lblVoluntary_Policy_Exp_Date_pro2.Enabled = enabled2;
            lblVoluntary_Policy_Exp_Date_pro3.Enabled = enabled3;

            lblMaintanance_pro1.Enabled = policy1Promotion
                ? false
                : exportNotifyPolicyReport ? !rbInsNameTh_pro1.Checked : true;

            lblMaintanance_pro2.Enabled = policy2Promotion
                ? false
                : exportNotifyPolicyReport ? !rbInsNameTh_pro2.Checked : true;
            lblMaintanance_pro3.Enabled = policy3Promotion
                ? false
                : exportNotifyPolicyReport ? !rbInsNameTh_pro3.Checked : true;

            lblCost_cur.Enabled = false;
            lblCost_pro1.Enabled = false;
            lblCost_pro2.Enabled = false;
            lblCost_pro3.Enabled = false;

            cmbDeDuctFlag_cur.Enabled = false;
            cmbDeDuctFlag_pro1.Enabled = !(policy1Promotion || (!policy1Promotion && rbInsNameTh_pro1.Checked && receiveNoIssued));
            cmbDeDuctFlag_pro2.Enabled = !(policy2Promotion || (!policy2Promotion && rbInsNameTh_pro2.Checked && receiveNoIssued));
            cmbDeDuctFlag_pro3.Enabled = !(policy3Promotion || (!policy3Promotion && rbInsNameTh_pro3.Checked && receiveNoIssued));

            lblVoluntary_Cov_Amt_cur.Enabled = enabledWithPromotion0 && hidHasCurrentYearPolicy.Value == "true";
            lblVoluntary_Cov_Amt_pro1.Enabled = !(policy1Promotion || (!policy1Promotion && rbInsNameTh_pro1.Checked && receiveNoIssued));
            lblVoluntary_Cov_Amt_pro2.Enabled = !(policy2Promotion || (!policy2Promotion && rbInsNameTh_pro2.Checked && receiveNoIssued));
            lblVoluntary_Cov_Amt_pro3.Enabled = !(policy3Promotion || (!policy3Promotion && rbInsNameTh_pro3.Checked && receiveNoIssued));

            lblCostFT_cur.Enabled = enabledWithPromotion0 && hidHasCurrentYearPolicy.Value == "true";
            lblCostFT_pro1.Enabled = !(policy1Promotion || (!policy1Promotion && rbInsNameTh_pro1.Checked && receiveNoIssued));
            lblCostFT_pro2.Enabled = !(policy2Promotion || (!policy2Promotion && rbInsNameTh_pro2.Checked && receiveNoIssued));
            lblCostFT_pro3.Enabled = !(policy3Promotion || (!policy3Promotion && rbInsNameTh_pro3.Checked && receiveNoIssued));

            lblNetpremium_cur.Enabled = enabledWithPromotion0 && hidHasCurrentYearPolicy.Value == "true";
            lblNetpremium_pro1.Enabled = !(policy1Promotion || (!policy1Promotion && rbInsNameTh_pro1.Checked && receiveNoIssued));
            lblNetpremium_pro2.Enabled = !(policy2Promotion || (!policy2Promotion && rbInsNameTh_pro2.Checked && receiveNoIssued));
            lblNetpremium_pro3.Enabled = !(policy3Promotion || (!policy3Promotion && rbInsNameTh_pro3.Checked && receiveNoIssued));

            lblVoluntary_Gross_Premium_cur.Enabled = enabledWithPromotion0 && hidHasCurrentYearPolicy.Value == "true";
            lblVoluntary_Gross_Premium_pro1.Enabled = !(policy1Promotion || (!policy1Promotion && rbInsNameTh_pro1.Checked && receiveNoIssued));
            lblVoluntary_Gross_Premium_pro2.Enabled = !(policy2Promotion || (!policy2Promotion && rbInsNameTh_pro2.Checked && receiveNoIssued));
            lblVoluntary_Gross_Premium_pro3.Enabled = !(policy3Promotion || (!policy3Promotion && rbInsNameTh_pro3.Checked && receiveNoIssued));

            chkCardType.Enabled = exportNotifyPolicyReport
                            ? false
                            : lblCardTypeId_pre.Text == "นิติบุคคล" ? true : false;

            txtDiscountPercent_cur.Enabled = hidHasCurrentYearPolicy.Value == "true";
            txtDiscountBath_cur.Enabled = hidHasCurrentYearPolicy.Value == "true";
            txtDiscountPercent_pro1.Enabled = true;
            txtDiscountBath_pro1.Enabled = true;
            txtDiscountPercent_pro2.Enabled = true;
            txtDiscountBath_pro2.Enabled = true;
            txtDiscountPercent_pro3.Enabled = true;
            txtDiscountBath_pro3.Enabled = true;

            txtTotal_Voluntary_Gross_Premium_cur.Enabled = false;
            txtTotal_Voluntary_Gross_Premium_pro1.Enabled = false;
            txtTotal_Voluntary_Gross_Premium_pro2.Enabled = false;
            txtTotal_Voluntary_Gross_Premium_pro3.Enabled = false;
        }

        private void disableReceiptAct(PreleadCompareDataCollection compareCollection, string receiveNo, DateTime? actSendDate)
        {
            //bool exportNotifyActReport = SlmScr004Biz.ExportNotifyActReport(receiveNo
            //    , compareCollection.RenewIns == null ? null : compareCollection.RenewIns.slm_TicketId);

            bool exportNotifyActReport;
            if (ExportNotifyActReport == null)
            {
                ExportNotifyActReport = SlmScr004Biz.ExportNotifyActReport(receiveNo, compareCollection.RenewIns == null ? null : compareCollection.RenewIns.slm_TicketId);
            }
            exportNotifyActReport = ExportNotifyActReport.Value;

            SlmScr004Biz biz = new SlmScr004Biz();

            bool act1Promotion = compareCollection.ActPromoList.Count > 0
                && compareCollection.ActPromoList[0].slm_PromotionId != null
                && compareCollection.ActPromoList[0].slm_PromotionId != 0;
            bool act2Promotion = compareCollection.ActPromoList.Count > 1
                && compareCollection.ActPromoList[1].slm_PromotionId != null
                && compareCollection.ActPromoList[1].slm_PromotionId != 0;
            bool act3Promotion = compareCollection.ActPromoList.Count > 2
                && compareCollection.ActPromoList[2].slm_PromotionId != null
                && compareCollection.ActPromoList[2].slm_PromotionId != 0;

            bool enabled1 = !exportNotifyActReport ? true : !rbAct_pro1.Checked;
            bool enabled2 = !exportNotifyActReport ? true : !rbAct_pro2.Checked;
            bool enabled3 = !exportNotifyActReport ? true : !rbAct_pro3.Checked;

            bool actBought = actSendDate != null;
            rbAct_pro1.Enabled = !actBought;
            rbAct_pro2.Enabled = !actBought;
            rbAct_pro3.Enabled = !actBought;

            cmbActIssuePlace_pro1.Enabled = enabled1;
            cmbActIssuePlace_pro2.Enabled = enabled2;
            cmbActIssuePlace_pro3.Enabled = enabled3;
            cmbActIssueBranch_pro1.Enabled = enabled1;
            cmbActIssueBranch_pro2.Enabled = enabled2;
            cmbActIssueBranch_pro3.Enabled = enabled3;

            rbRegisterAct_pro1.Enabled = enabled1;
            rbRegisterAct_pro2.Enabled = enabled2;
            rbRegisterAct_pro3.Enabled = enabled3;

            rbNormalAct_pro1.Enabled = enabled1;
            rbNormalAct_pro2.Enabled = enabled2;
            rbNormalAct_pro3.Enabled = enabled3;

            lblCompanyInsuranceAct_pro1.Enabled = exportNotifyActReport && rbAct_pro1.Checked ? false
                                                                : act1Promotion ? false
                                                                                : rbAct_pro1.Checked ? !actBought : true;
            lblCompanyInsuranceAct_pro2.Enabled = exportNotifyActReport && rbAct_pro2.Checked ? false
                                                                : act2Promotion ? false
                                                                                : rbAct_pro2.Checked ? !actBought : true;
            lblCompanyInsuranceAct_pro3.Enabled = exportNotifyActReport && rbAct_pro3.Checked ? false
                                                                : act3Promotion ? false
                                                                                : rbAct_pro3.Checked ? !actBought : true;

            txtSignNoAct_pro1.Enabled = !(rbAct_pro1.Checked && actBought);
            txtSignNoAct_pro2.Enabled = !(rbAct_pro2.Checked && actBought);
            txtSignNoAct_pro3.Enabled = !(rbAct_pro3.Checked && actBought);

            txtActStartCoverDateAct_pro1.Enabled = exportNotifyActReport && rbAct_pro1.Checked ? !actBought : true;
            txtActStartCoverDateAct_pro2.Enabled = exportNotifyActReport && rbAct_pro2.Checked ? !actBought : true;
            txtActStartCoverDateAct_pro3.Enabled = exportNotifyActReport && rbAct_pro3.Checked ? !actBought : true;

            txtActEndCoverDateAct_pro1.Enabled = exportNotifyActReport && rbAct_pro1.Checked ? !actBought : true;
            txtActEndCoverDateAct_pro2.Enabled = exportNotifyActReport && rbAct_pro2.Checked ? !actBought : true;
            txtActEndCoverDateAct_pro3.Enabled = exportNotifyActReport && rbAct_pro3.Checked ? !actBought : true;

            txtCarTaxExpiredDateAct_pro1.Enabled = rbAct_pro1.Checked ? !actBought : true;
            txtCarTaxExpiredDateAct_pro2.Enabled = rbAct_pro2.Checked ? !actBought : true;
            txtCarTaxExpiredDateAct_pro3.Enabled = rbAct_pro3.Checked ? !actBought : true;

            // Gross Premium enabled up on if promotion selected too            
            lblActNetGrossPremiumFullAct_pro1.Enabled = !(act1Promotion || (!act1Promotion && rbAct_pro1.Checked && actBought));
            lblActNetGrossPremiumFullAct_pro2.Enabled = !(act2Promotion || (!act2Promotion && rbAct_pro2.Checked && actBought));
            lblActNetGrossPremiumFullAct_pro3.Enabled = !(act3Promotion || (!act3Promotion && rbAct_pro3.Checked && actBought));


            chkCardType2.Enabled = exportNotifyActReport
                            ? false
                            : lblCardTypeId_pre.Text == "นิติบุคคล" ? true : false;

            txtDiscountBathAct_pro1.Enabled = rbAct_pro1.Checked ? !actBought : true; // !(act1Promotion || (!act1Promotion && rbAct_pro1.Checked && actBought));
            txtDiscountBathAct_pro2.Enabled = rbAct_pro2.Checked ? !actBought : true;//  !(act2Promotion || (!act2Promotion && rbAct_pro2.Checked && actBought));
            txtDiscountBathAct_pro3.Enabled = rbAct_pro3.Checked ? !actBought : true; // !(act3Promotion || (!act3Promotion && rbAct_pro3.Checked && actBought));

        }
        private bool CheckDupInsAct(List<PreleadCompareActData> act, string hidvalue)
        {
            bool result = true;
            if (hidvalue != "0")
            {
                foreach (var item in act)
                {
                    if (item.slm_PromotionId.ToString() == hidvalue)
                    {
                        result = false;
                        break;
                    }

                }
                return result;
            }
            else
            {
                return result;
            }
        }

        private void disableReceipt(List<RenewInsuranceReceiptData> dataList, int countPolicy, int countAct, bool clearFlag)
        {
            disableReceipt(dataList, countPolicy, countAct, clearFlag, false, false);
        }

        private void disableReceipt(List<RenewInsuranceReceiptData> dataList, int countPolicy, int countAct, bool clearFlag, bool exportNotifyPolicyReport, bool exportNotifyActReport)
        {
            if (clearFlag)
            {
                #region Clear Data

                rbInsNameTh_cur.Enabled = true;
                rbInsNameTh_pro1.Enabled = true;
                rbInsNameTh_pro2.Enabled = true;
                rbInsNameTh_pro3.Enabled = true;

                rbDriver_Flag_cur1.Enabled = true;
                rbDriver_Flag_cur2.Enabled = true;
                rbDriver_Flag_pro11.Enabled = true;
                rbDriver_Flag_pro12.Enabled = true;
                rbDriver_Flag_pro21.Enabled = true;
                rbDriver_Flag_pro22.Enabled = true;
                rbDriver_Flag_pro31.Enabled = true;
                rbDriver_Flag_pro32.Enabled = true;

                lblCoverageType_pro1.Enabled = true;
                lblCoverageType_pro2.Enabled = true;
                lblCoverageType_pro3.Enabled = false;

                lblInsNameTh_pro1.Enabled = true;
                lblInsNameTh_pro2.Enabled = true;
                lblInsNameTh_pro3.Enabled = true;

                lblMaintanance_pro1.Enabled = true;
                lblMaintanance_pro2.Enabled = true;
                lblMaintanance_pro3.Enabled = true;

                lblCost_pro1.Enabled = true;
                lblCost_pro2.Enabled = true;
                lblCost_pro3.Enabled = true;

                cmbDeDuctFlag_pro1.Enabled = true;
                cmbDeDuctFlag_pro2.Enabled = true;
                cmbDeDuctFlag_pro3.Enabled = true;

                lblVoluntary_Cov_Amt_pro1.Enabled = true;
                lblVoluntary_Cov_Amt_pro2.Enabled = true;
                lblVoluntary_Cov_Amt_pro3.Enabled = true;

                lblCostFT_pro1.Enabled = true;
                lblCostFT_pro2.Enabled = true;
                lblCostFT_pro3.Enabled = true;

                lblNetpremium_pro1.Enabled = true;
                lblNetpremium_pro2.Enabled = true;
                lblNetpremium_pro3.Enabled = true;

                lblVoluntary_Gross_Premium_pro1.Enabled = true;
                lblVoluntary_Gross_Premium_pro2.Enabled = true;
                lblVoluntary_Gross_Premium_pro3.Enabled = true;

                lblVoluntary_Policy_Eff_Date_cur.Enabled = true;
                lblVoluntary_Policy_Eff_Date_pro1.Enabled = true;
                lblVoluntary_Policy_Eff_Date_pro2.Enabled = true;
                lblVoluntary_Policy_Eff_Date_pro3.Enabled = true;

                lblVoluntary_Policy_Exp_Date_cur.Enabled = true;
                lblVoluntary_Policy_Exp_Date_pro1.Enabled = true;
                lblVoluntary_Policy_Exp_Date_pro2.Enabled = true;
                lblVoluntary_Policy_Exp_Date_pro3.Enabled = true;               

                txtDiscountPercent_cur.Enabled = true;
                txtDiscountPercent_pro1.Enabled = true;
                txtDiscountPercent_pro2.Enabled = true;
                txtDiscountPercent_pro3.Enabled = true;

                txtDiscountBath_cur.Enabled = true;
                txtDiscountBath_pro1.Enabled = true;
                txtDiscountBath_pro2.Enabled = true;
                txtDiscountBath_pro3.Enabled = true;

                txtTotal_Voluntary_Gross_Premium_cur.Enabled = true;
                txtTotal_Voluntary_Gross_Premium_pro1.Enabled = true;
                txtTotal_Voluntary_Gross_Premium_pro2.Enabled = true;
                txtTotal_Voluntary_Gross_Premium_pro3.Enabled = true;

                cmbPaymentmethod.Enabled = true;
                textPeriod.Enabled = true;
                cmbPaymentType.Enabled = true;
                cmbPayBranchCode.Enabled = true;

                txtPeriod1.Enabled = true;
                tdmPaymentDate1.Enabled = true;

                txtPeriod2.Enabled = true;
                tdmPaymentDate2.Enabled = true;

                txtPeriod3.Enabled = true;
                tdmPaymentDate3.Enabled = true;

                lblCompanyInsuranceAct_pro1.Enabled = true;
                lblCompanyInsuranceAct_pro2.Enabled = true;
                lblCompanyInsuranceAct_pro3.Enabled = true;

                lblActNetGrossPremiumFullAct_pro1.Enabled = true;
                lblActNetGrossPremiumFullAct_pro2.Enabled = true;
                lblActNetGrossPremiumFullAct_pro3.Enabled = true;
                rbAct_pro1.Enabled = true;
                rbAct_pro2.Enabled = true;
                rbAct_pro3.Enabled = true;

                cmbActIssuePlace_pro1.Enabled = true;
                cmbActIssuePlace_pro2.Enabled = true;
                cmbActIssuePlace_pro3.Enabled = true;

                cmbActIssueBranch_pro1.Enabled = true;
                cmbActIssueBranch_pro2.Enabled = true;
                cmbActIssueBranch_pro3.Enabled = true;

                rbRegisterAct_pro1.Enabled = true;
                rbRegisterAct_pro2.Enabled = true;
                rbRegisterAct_pro3.Enabled = true;

                rbNormalAct_pro1.Enabled = true;
                rbNormalAct_pro2.Enabled = true;
                rbNormalAct_pro3.Enabled = true;

                txtSignNoAct_pro1.Enabled = true;
                txtSignNoAct_pro2.Enabled = true;
                txtSignNoAct_pro3.Enabled = true;

                txtActStartCoverDateAct_pro1.Enabled = true;
                txtActStartCoverDateAct_pro2.Enabled = true;
                txtActStartCoverDateAct_pro3.Enabled = true;

                txtActEndCoverDateAct_pro1.Enabled = true;
                txtActEndCoverDateAct_pro2.Enabled = true;
                txtActEndCoverDateAct_pro3.Enabled = true;

                txtCarTaxExpiredDateAct_pro1.Enabled = true;
                txtCarTaxExpiredDateAct_pro2.Enabled = true;
                txtCarTaxExpiredDateAct_pro3.Enabled = true;

                txtDiscountBathAct_pro1.Enabled = true;
                txtDiscountBathAct_pro2.Enabled = true;
                txtDiscountBathAct_pro3.Enabled = true;

                lblVat1PercentBathAct_pro1.Enabled = true;
                lblVat1PercentBathAct_pro2.Enabled = true;
                lblVat1PercentBathAct_pro3.Enabled = true;

                lblDiscountPercentAct_pro1.Enabled = true;
                lblDiscountPercentAct_pro2.Enabled = true;
                lblDiscountPercentAct_pro3.Enabled = true;

                cmbPaymentmethodAct.Enabled = true;
                cmbPaymentTypeAct.Enabled = true;
                cmbPayBranchCodeAct.Enabled = true;

                if (hidPromotionId1.Value == "0")
                {
                    setDisablePromoPolicy(1, true);
                }
                else
                {
                    setDisablePromoPolicy(1, false);
                }

                if (hidPromotionId2.Value == "0")
                {
                    setDisablePromoPolicy(2, true);
                }
                else
                {
                    setDisablePromoPolicy(2, false);
                }

                if (hidPromotionId3.Value == "0")
                {
                    setDisablePromoPolicy(3, true);
                }
                else
                {
                    setDisablePromoPolicy(3, false);
                }

                if (hidPromotionActId1.Value == "0")
                {
                    setDisablePromoAct(1, true);
                }
                else
                {
                    setDisablePromoAct(1, false);
                }

                if (hidPromotionActId2.Value == "0")
                {
                    setDisablePromoAct(2, true);
                }
                else
                {
                    setDisablePromoAct(2, false);
                }

                if (hidPromotionActId3.Value == "0")
                {
                    setDisablePromoAct(3, true);
                }
                else
                {
                    setDisablePromoAct(3, false);
                }
                #endregion
            }
            else if (dataList != null && dataList.Count > 0)
            {
                if (countPolicy != 0)
                {
                    #region Disable Policy

                    btnAddBlankPolicy.Enabled = false;

                    rbInsNameTh_cur.Enabled = false;
                    rbInsNameTh_pro1.Enabled = false;
                    rbInsNameTh_pro2.Enabled = false;
                    rbInsNameTh_pro3.Enabled = false;

                    lblCoverageType_pro1.Enabled = false;
                    lblCoverageType_pro2.Enabled = false;
                    lblCoverageType_pro3.Enabled = false;

                    lblInsNameTh_pro1.Enabled = false;
                    lblInsNameTh_pro2.Enabled = false;
                    lblInsNameTh_pro3.Enabled = false;

                    bool enabled = !exportNotifyPolicyReport ? rbInsNameTh_cur.Checked : !rbInsNameTh_cur.Checked;
                    bool enabled1 = !exportNotifyPolicyReport ? rbInsNameTh_pro1.Checked : !rbInsNameTh_pro1.Checked;
                    bool enabled2 = !exportNotifyPolicyReport ? rbInsNameTh_pro2.Checked : !rbInsNameTh_pro2.Checked;
                    bool enabled3 = !exportNotifyPolicyReport ? rbInsNameTh_pro3.Checked : !rbInsNameTh_pro3.Checked;

                    rbDriver_Flag_cur1.Enabled = enabled;
                    rbDriver_Flag_cur2.Enabled = enabled;
                    rbDriver_Flag_pro11.Enabled = enabled1;
                    rbDriver_Flag_pro12.Enabled = enabled1;
                    rbDriver_Flag_pro21.Enabled = enabled2;
                    rbDriver_Flag_pro22.Enabled = enabled2;
                    rbDriver_Flag_pro31.Enabled = enabled3;
                    rbDriver_Flag_pro32.Enabled = enabled3;

                    cmbTitleName1_cur.Enabled = enabled;
                    cmbTitleName1_pro1.Enabled = enabled1;
                    cmbTitleName1_pro2.Enabled = enabled2;
                    cmbTitleName1_pro3.Enabled = enabled3;

                    txtDriver_First_Name1_cur.Enabled = enabled;
                    txtDriver_First_Name1_pro1.Enabled = enabled1;
                    txtDriver_First_Name1_pro2.Enabled = enabled2;
                    txtDriver_First_Name1_pro3.Enabled = enabled3;

                    txtDriver_Last_Name1_cur.Enabled = enabled;
                    txtDriver_Last_Name1_pro1.Enabled = enabled1;
                    txtDriver_Last_Name1_pro2.Enabled = enabled2;
                    txtDriver_Last_Name1_pro3.Enabled = enabled3;

                    tdmDriver_Birthdate1_cur.Enabled = enabled;
                    tdmDriver_Birthdate1_pro1.Enabled = enabled1;
                    tdmDriver_Birthdate1_pro2.Enabled = enabled2;
                    tdmDriver_Birthdate1_pro3.Enabled = enabled3;

                    cmbTitleName2_cur.Enabled = enabled;
                    cmbTitleName2_pro1.Enabled = enabled1;
                    cmbTitleName2_pro2.Enabled = enabled2;
                    cmbTitleName2_pro3.Enabled = enabled3;

                    txtDriver_First_Name2_cur.Enabled = enabled;
                    txtDriver_First_Name2_pro1.Enabled = enabled1;
                    txtDriver_First_Name2_pro2.Enabled = enabled2;
                    txtDriver_First_Name2_pro3.Enabled = enabled3;

                    txtDriver_Last_Name2_cur.Enabled = enabled;
                    txtDriver_Last_Name2_pro1.Enabled = enabled1;
                    txtDriver_Last_Name2_pro2.Enabled = enabled2;
                    txtDriver_Last_Name2_pro3.Enabled = enabled3;

                    tdmDriver_Birthdate2_cur.Enabled = enabled;
                    tdmDriver_Birthdate2_pro1.Enabled = enabled1;
                    tdmDriver_Birthdate2_pro2.Enabled = enabled2;
                    tdmDriver_Birthdate2_pro3.Enabled = enabled3;

                    lblVoluntary_Policy_Eff_Date_cur.Enabled = enabled;
                    lblVoluntary_Policy_Eff_Date_pro1.Enabled = enabled1;
                    lblVoluntary_Policy_Eff_Date_pro2.Enabled = enabled2;
                    lblVoluntary_Policy_Eff_Date_pro3.Enabled = enabled3;

                    lblVoluntary_Policy_Exp_Date_cur.Enabled = enabled;
                    lblVoluntary_Policy_Exp_Date_pro1.Enabled = enabled1;
                    lblVoluntary_Policy_Exp_Date_pro2.Enabled = enabled2;
                    lblVoluntary_Policy_Exp_Date_pro3.Enabled = enabled3;

                    lblMaintanance_pro1.Enabled = enabled1;
                    lblMaintanance_pro2.Enabled = enabled2;
                    lblMaintanance_pro3.Enabled = enabled3;

                    lblCost_pro1.Enabled = false;
                    lblCost_pro2.Enabled = false;
                    lblCost_pro3.Enabled = false;

                    cmbDeDuctFlag_pro1.Enabled = false;
                    cmbDeDuctFlag_pro2.Enabled = false;
                    cmbDeDuctFlag_pro3.Enabled = false;

                    lblVoluntary_Cov_Amt_pro1.Enabled = false;
                    lblVoluntary_Cov_Amt_pro2.Enabled = false;
                    lblVoluntary_Cov_Amt_pro3.Enabled = false;

                    lblCostFT_pro1.Enabled = false;
                    lblCostFT_pro2.Enabled = false;
                    lblCostFT_pro3.Enabled = false;

                    lblNetpremium_pro1.Enabled = false;
                    lblNetpremium_pro2.Enabled = false;
                    lblNetpremium_pro3.Enabled = false;

                    lblVoluntary_Gross_Premium_pro1.Enabled = false;
                    lblVoluntary_Gross_Premium_pro2.Enabled = false;
                    lblVoluntary_Gross_Premium_pro3.Enabled = false;

                    if (rbInsNameTh_cur.Checked)
                    {
                        txtDiscountPercent_cur.Enabled = true;
                        txtDiscountBath_cur.Enabled = true;
                    }
                    else
                    {
                        txtDiscountPercent_cur.Enabled = false;
                        txtDiscountBath_cur.Enabled = false;
                    }
                    if (rbInsNameTh_pro1.Checked)
                    {
                        txtDiscountPercent_pro1.Enabled = true;
                        txtDiscountBath_pro1.Enabled = true;
                    }
                    else
                    {
                        txtDiscountPercent_pro1.Enabled = false;
                        txtDiscountBath_pro1.Enabled = false;
                    }
                    if (rbInsNameTh_pro2.Checked)
                    {
                        txtDiscountPercent_pro2.Enabled = true;
                        txtDiscountBath_pro2.Enabled = true;
                    }
                    else
                    {
                        txtDiscountPercent_pro2.Enabled = false;
                        txtDiscountBath_pro2.Enabled = false;
                    }
                    if (rbInsNameTh_pro3.Checked)
                    {
                        txtDiscountPercent_pro3.Enabled = true;
                        txtDiscountBath_pro3.Enabled = true;
                    }
                    else
                    {
                        txtDiscountPercent_pro3.Enabled = false;
                        txtDiscountBath_pro3.Enabled = false;
                    }

                    txtTotal_Voluntary_Gross_Premium_cur.Enabled = false;
                    txtTotal_Voluntary_Gross_Premium_pro1.Enabled = false;
                    txtTotal_Voluntary_Gross_Premium_pro2.Enabled = false;
                    txtTotal_Voluntary_Gross_Premium_pro3.Enabled = false;

                    #endregion
                }

                if (countAct != 0)
                {
                    #region Disable Act

                    bool enabled = !exportNotifyActReport ? rbInsNameTh_cur.Checked : !rbInsNameTh_cur.Checked;
                    bool enabled1 = !exportNotifyActReport ? rbInsNameTh_pro1.Checked : !rbInsNameTh_pro1.Checked;
                    bool enabled2 = !exportNotifyActReport ? rbInsNameTh_pro2.Checked : !rbInsNameTh_pro2.Checked;
                    bool enabled3 = !exportNotifyActReport ? rbInsNameTh_pro3.Checked : !rbInsNameTh_pro3.Checked;

                    btnAddBlankAct.Enabled = false;

                    rbAct_pro1.Enabled = false;
                    rbAct_pro2.Enabled = false;
                    rbAct_pro3.Enabled = false;

                    cmbActIssuePlace_pro1.Enabled = enabled1;
                    cmbActIssuePlace_pro2.Enabled = enabled2;
                    cmbActIssuePlace_pro3.Enabled = enabled3;
                    cmbActIssueBranch_pro1.Enabled = enabled1;
                    cmbActIssueBranch_pro2.Enabled = enabled2;
                    cmbActIssueBranch_pro3.Enabled = enabled3;

                    rbRegisterAct_pro1.Enabled = enabled1;
                    rbRegisterAct_pro2.Enabled = enabled2;
                    rbRegisterAct_pro3.Enabled = enabled3;

                    rbNormalAct_pro1.Enabled = enabled1;
                    rbNormalAct_pro2.Enabled = enabled2;
                    rbNormalAct_pro3.Enabled = enabled3;

                    lblCompanyInsuranceAct_pro1.Enabled = false;
                    lblCompanyInsuranceAct_pro2.Enabled = false;
                    lblCompanyInsuranceAct_pro3.Enabled = false;

                    txtSignNoAct_pro1.Enabled = false;
                    txtSignNoAct_pro2.Enabled = false;
                    txtSignNoAct_pro3.Enabled = false;

                    txtActStartCoverDateAct_pro1.Enabled = enabled1;
                    txtActStartCoverDateAct_pro2.Enabled = enabled2;
                    txtActStartCoverDateAct_pro3.Enabled = enabled3;

                    txtActEndCoverDateAct_pro1.Enabled = enabled1;
                    txtActEndCoverDateAct_pro2.Enabled = enabled2;
                    txtActEndCoverDateAct_pro3.Enabled = enabled3;

                    txtCarTaxExpiredDateAct_pro1.Enabled = false;
                    txtCarTaxExpiredDateAct_pro2.Enabled = false;
                    txtCarTaxExpiredDateAct_pro3.Enabled = false;

                    lblActNetGrossPremiumFullAct_pro1.Enabled = false;
                    lblActNetGrossPremiumFullAct_pro2.Enabled = false;
                    lblActNetGrossPremiumFullAct_pro3.Enabled = false;

                    if (rbAct_pro1.Checked)
                    {
                        lblVat1PercentBathAct_pro1.Enabled = true;
                        lblDiscountPercentAct_pro1.Enabled = true;
                    }
                    else
                    {
                        lblVat1PercentBathAct_pro1.Enabled = false;
                        lblDiscountPercentAct_pro1.Enabled = false;
                    }
                    if (rbAct_pro2.Checked)
                    {
                        lblVat1PercentBathAct_pro2.Enabled = true;
                        lblDiscountPercentAct_pro2.Enabled = true;
                    }
                    else
                    {
                        lblVat1PercentBathAct_pro2.Enabled = false;
                        lblDiscountPercentAct_pro2.Enabled = false;
                    }
                    if (rbAct_pro3.Checked)
                    {

                        lblVat1PercentBathAct_pro3.Enabled = true;
                        lblDiscountPercentAct_pro3.Enabled = true;
                    }
                    else
                    {
                        lblVat1PercentBathAct_pro3.Enabled = false;
                        lblDiscountPercentAct_pro3.Enabled = false;
                    }

                    txtDiscountBathAct_pro1.Enabled = false;
                    txtDiscountBathAct_pro2.Enabled = false;
                    txtDiscountBathAct_pro3.Enabled = true;

                    #endregion
                }
            }
            else
            {
                //if (Type == "1")
                //{
                //    txtReceiveNo.Enabled = false;
                //    btnReceive.Enabled = false;
                //    btnReceiveAct.Enabled = false;
                //    txtReceiveDate.Enabled = false;
                //    txtIncentiveDate.Enabled = false;
                //    btnIncentive.Enabled = false;
                //    txtActSendDate.Enabled = false;
                //    txtActIncentiveDate.Enabled = false;
                //    btnIncentiveAct.Enabled = false;
                //}
                //else
                //{ 
                //    //Type 2,3
                //    var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                //    if (staff == null)
                //    {
                //        btnReceive.Enabled = false;
                //        btnReceiveAct.Enabled = false;
                //        return;
                //    }

                //    if (staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound)
                //    {
                //        btnReceive.Enabled = txtReceiveNo.Text.Trim() == "";
                //        btnReceiveAct.Enabled = txtActSendDate.Text == "";
                //    }
                //    else
                //    {
                //        btnReceive.Enabled = false;
                //        btnReceiveAct.Enabled = false;
                //    }
                //}
            }
        }

        private void PrepareLeadData(LeadDefaultData data, string loginName)
        {
            RenewInsureBiz biz = null;
            try
            {
                Session.Remove(SessionPrefix+SLMConstant.SessionName.renewinsure_phonecall_list);

                divLeadAddContact.Visible = true;
                divPreleadAddContact.Visible = false;

                //For Search Gridview
                txtTicketIdSearch.Text = data.TicketId;
                txtPreleadIdSearch.Text = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
                txtCitizenIdSearch.Text = data.CitizenId;
                txtTelNo1Search.Text = data.TelNo1;
                txtLoginName.Text = loginName;

                string[] cocTeam = new string[] { SLMConstant.COCTeam.Marketing, SLMConstant.COCTeam.Bpel };
                if (data.ISCOC == "1" && !cocTeam.Contains(data.COCCurrentTeam))
                {
                    btnAddResultContact.Visible = false;
                }
                else
                {
                    biz = new RenewInsureBiz();
                    CheckActivityConfig(biz, data.ProductId, data.StatusCode);
                }

                pcTop.SetVisible = false;
                cbThisLead.Checked = true;
                DoBindGridview(0);

                HideDebugControl();     //ซ่อน Control สำหรับ Debug

                string cbsSubscriptionTypeId = "0";
                if (data.CardType != null)
                {
                    cbsSubscriptionTypeId = SlmScr008Biz.GetSubScriptionTypeId(data.CardType.Value);
                }

                lbCasAllActivity.OnClientClick = new Services.CARService().GetCallCASScript(data.TicketId, data.PreleadId, cbsSubscriptionTypeId, data.CitizenId, HttpContext.Current.User.Identity.Name);
                upResult.Update();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        protected void imbDownloadQuotation_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                _log.Info("==================================================================================");
                _log.Info("Start Download");

                var ecm = new ConnectEcm();
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                string filePath = ((Label)gvPhoneCallHistoty.Rows[index].FindControl("lblQuotationFilePath")).Text.Trim();
                string fileName = ((Label)gvPhoneCallHistoty.Rows[index].FindControl("lblQuotationFileName")).Text.Trim();

                string retMessage = ecm.VerifyFileToDownload(filePath, fileName);

                if (retMessage == "")
                {
                    string[] arr = new string[] { filePath, fileName };
                    Session[SLMConstant.SessionName.Tawi50FormFilePath] = arr;
                    string script = "window.open('SLM_SCR_054.aspx?type=2', 'quotationform', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=no');";
                    ScriptManager.RegisterStartupScript(Page, GetType(), "quotation", script, true);
                }
                else
                {
                    AppUtil.ClientAlertTabRenew(Page, retMessage);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void imbDownload50Tawi_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                _log.Info("==================================================================================");
                _log.Info("Start Download");

                var ecm = new ConnectEcm();
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                string filePath = ((Label)gvPhoneCallHistoty.Rows[index].FindControl("lblTawi50FilePath")).Text.Trim();
                string fileName = ((Label)gvPhoneCallHistoty.Rows[index].FindControl("lblTawi50FileName")).Text.Trim();

                string retMessage = ecm.VerifyFileToDownload(filePath, fileName);

                if (retMessage == "")
                {
                    string[] arr = new string[] { filePath, fileName };
                    Session[SLMConstant.SessionName.Tawi50FormFilePath] = arr;
                    string script = "window.open('SLM_SCR_054.aspx?type=2', 'tawi50form', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=no');";
                    ScriptManager.RegisterStartupScript(Page, GetType(), "tawi50form", script, true);
                }
                else
                {
                    AppUtil.ClientAlertTabRenew(Page, retMessage);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void imbDownloadCredit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                _log.Info("==================================================================================");
                _log.Info("Start Download");

                var ecm = new ConnectEcm();
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                string filePath = ((Label)gvPhoneCallHistoty.Rows[index].FindControl("lblCreditFilePath")).Text.Trim();
                string fileName = ((Label)gvPhoneCallHistoty.Rows[index].FindControl("lblCreditFileName")).Text.Trim();

                string retMessage = ecm.VerifyFileToDownload(filePath, fileName);

                if (retMessage == "")
                {
                    string[] arr = new string[] { filePath, fileName };
                    Session[SLMConstant.SessionName.CreditFormFilePath] = arr;
                    string script = "window.open('SLM_SCR_054.aspx?type=1', 'creditform', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=no');";
                    ScriptManager.RegisterStartupScript(Page, GetType(), "creditform", script, true);
                }
                else
                {
                    AppUtil.ClientAlertTabRenew(Page, retMessage);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void imbDownloadDriverLicense_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                _log.Info("==================================================================================");
                _log.Info("Start Download");

                var ecm = new ConnectEcm();
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                string filePath = ((Label)gvPhoneCallHistoty.Rows[index].FindControl("lblDriverLicenseFilePath")).Text.Trim();
                string fileName = ((Label)gvPhoneCallHistoty.Rows[index].FindControl("lblDriverLicenseFileName")).Text.Trim();

                string retMessage = ecm.VerifyFileToDownload(filePath, fileName);

                if (retMessage == "")
                {
                    string[] arr = new string[] { filePath, fileName };
                    Session[SLMConstant.SessionName.DriverLicenseFormFilePath] = arr;
                    string script = "window.open('SLM_SCR_054.aspx?type=3', 'driverform', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=no');";
                    ScriptManager.RegisterStartupScript(Page, GetType(), "driverform", script, true);
                }
                else
                {
                    AppUtil.ClientAlertTabRenew(Page, retMessage);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        private PreleadData preparePreLeadData()
        {
            PreleadData preData = new PreleadData();
            return preData;
        }

        private void HideDebugControl()
        {
            try
            {
                txtPreleadIdSearch.Visible = false;
                txtTicketIdSearch.Visible = false;
                txtCitizenIdSearch.Visible = false;
                txtTelNo1Search.Visible = false;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void CheckActivityConfig(RenewInsureBiz biz, string productId, string leadStatus)
        {
            try
            {
                List<ActivityConfigData> list = biz.GetActivityConfig(productId, leadStatus);
                if (list.Count > 0)
                {
                    bool? rightAdd = list.Select(p => p.HaveRightAdd).FirstOrDefault();
                    btnAddResultContact.Visible = (rightAdd == true ? true : false);
                }
                else
                {
                    btnAddResultContact.Visible = false;
                }
            }
            catch
            {
                throw;
            }
        }

        protected void chkthis_CheckedChanged(object sender, EventArgs e)
        {
            RenewInsureBiz biz = null;
            try
            {
                biz = new RenewInsureBiz();
                DoBindGridview(biz, 0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        public void DoBindGridview(int pageIndex)
        {
            RenewInsureBiz biz = null;
            try
            {
                _log.Debug($"Method=DoBindGridview, TicketId={txtTicketIdSearch.Text.Trim()}, SearchNewPhoneCallHistory Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                biz = new RenewInsureBiz();
                var result = biz.SearchNewPhoneCallHistory(txtCitizenIdSearch.Text.Trim(), txtTicketIdSearch.Text.Trim(), cbThisLead.Checked, txtPreleadIdSearch.Text.Trim());
                biz.CloseConnection();
                _log.Debug($"Method=DoBindGridview, TicketId={txtTicketIdSearch.Text.Trim()}, SearchNewPhoneCallHistory End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), pageIndex);
                Session[SessionPrefix+SLMConstant.SessionName.renewinsure_phonecall_list] = result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        public void DoBindGridview(RenewInsureBiz biz, int pageIndex)
        {
            try
            {
                _log.Debug($"Method=DoBindGridview, PreleadId={txtPreleadIdSearch.Text.Trim()}, TicketId={txtTicketIdSearch.Text.Trim()}, cbThisLead={(cbThisLead.Checked ? "TRUE" : "FALSE")}, SearchNewPhoneCallHistory Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                var result = biz.SearchNewPhoneCallHistory(txtCitizenIdSearch.Text.Trim(), txtTicketIdSearch.Text.Trim(), cbThisLead.Checked, txtPreleadIdSearch.Text.Trim());

                BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), pageIndex);
                Session[SessionPrefix+SLMConstant.SessionName.renewinsure_phonecall_list] = result;
                _log.Debug($"Method=DoBindGridview, PreleadId={txtPreleadIdSearch.Text.Trim()}, TicketId={txtTicketIdSearch.Text.Trim()}, cbThisLead={(cbThisLead.Checked ? "TRUE" : "FALSE")}, SearchNewPhoneCallHistory End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            try
            {
                pageControl.SetGridview(gvPhoneCallHistoty);
                pageControl.Update(items, pageIndex);
                upResult.Update();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            RenewInsureBiz biz = null;
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                biz = new RenewInsureBiz();
                DoBindGridview(biz, pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        #endregion

        protected void btnAddResultContact_Click(object sender, EventArgs e)
        {
            try
            {
                ucActRenewInsureContact.TicketId = txtTicketIdSearch.Text.Trim();   //TicketId;
                ViewState["ContractTicketID"] = txtTicketIdSearch.Text.Trim();      // TicketId;
                ucActRenewInsureContact.show(txtLoginName.Text.Trim());
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void cmbActIssuePlace_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                setActIssuePlace();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private void setActIssuePlace()
        {
            try
            {
                if (cmbActIssuePlace_pro1.SelectedIndex == 1)
                {
                    cmbActIssueBranch_pro1.Visible = true;
                }
                else
                {
                    cmbActIssueBranch_pro1.Visible = false;
                }

                if (cmbActIssuePlace_pro2.SelectedIndex == 1)
                {
                    cmbActIssueBranch_pro2.Visible = true;
                }
                else
                {
                    cmbActIssueBranch_pro2.Visible = false;
                }

                if (cmbActIssuePlace_pro3.SelectedIndex == 1)
                {
                    cmbActIssueBranch_pro3.Visible = true;
                }
                else
                {
                    cmbActIssueBranch_pro3.Visible = false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void setDivCancel(PreleadCompareDataCollection data)
        {

            if (data.RenewIns.slm_PolicyCancelId != null)
            {
                divCancelPolicy.Visible = true;
                cmbPolicyCancelReason.SelectedValue = data.RenewIns.slm_PolicyCancelId == null ? "" : data.RenewIns.slm_PolicyCancelId.Value.ToString();
                txtPolicyCancelDate.Text = data.RenewIns.slm_PolicyCancelDate.ToString();
            }
            else
            {
                divCancelPolicy.Visible = false;
                cmbPolicyCancelReason.SelectedValue = "";
                txtPolicyCancelDate.Text = "";
            }

            if (data.RenewIns.slm_ActCancelId != null)
            {
                divCancelAct.Visible = true;
                cmbActCancelReason.SelectedValue = data.RenewIns.slm_ActCancelId == null ? "" : data.RenewIns.slm_ActCancelId.Value.ToString();
                txtActCancelDate.Text = data.RenewIns.slm_ActCancelDate.ToString();
            }
            else
            {
                divCancelAct.Visible = false;
                cmbActCancelReason.SelectedValue = "";
                txtActCancelDate.Text = "";
            }
        }

        private void setReceivePolicyButton(PreleadCompareDataCollection data, string username, StaffData staff, bool OwnerOrDelegate, bool supervisor, bool policyPurchasedFlag)
        {
            try
            {
                btnReceive.Enabled = false;
                if (!rbInsNameTh_pro1.Checked && !rbInsNameTh_pro2.Checked && !rbInsNameTh_pro3.Checked && !rbInsNameTh_cur.Checked)
                {
                    // case edit from page                 
                    return;
                }

                if (!policyPurchasedFlag)
                {
                    return;
                }

                if (staff == null)
                {
                    return;
                }

                if (Type == "1")    //Motor Renew
                {
                    //bool OwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(data.RenewIns.slm_TicketId, username);
                    //bool supervisor = ActivityLeadBiz.CheckSupervisor(username);
                    //var staff = StaffBiz.GetStaff(username);
                    //bool headerOfOwnerOrDelegate = ActivityLeadBiz.CheckHeaderOwnerOrDelegate(data.RenewIns.slm_TicketId, username);

                    if (cmbPaymentmethod.SelectedValue == "1" || cmbPaymentmethod.SelectedValue == "") //ชำระเต็มจำนวน
                    {
                        if (PolicyCheckPaid == null)
                        {
                            PolicyCheckPaid = ActivityLeadBiz.checkPaidPolicy(data.RenewIns.slm_TicketId);
                        } 

                        //bool Receipt = ActivityLeadBiz.checkPaidPolicy(data.RenewIns.slm_TicketId);
                        if (PolicyCheckPaid == true)
                        {
                            if (OwnerOrDelegate || supervisor
                                || staff.StaffTypeId == SLMConstant.StaffType.OperOutbound
                                || staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound
                                || staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound)
                            {
                                btnReceive.Enabled = txtReceiveNo.Text.Trim() == "";
                            }
                        }
                        else
                        {
                            if (supervisor)
                            {
                                btnReceive.Enabled = txtReceiveNo.Text.Trim() == "";
                            }
                            else
                            {
                                if (staff != null)
                                {
                                    if (staff.StaffTypeId == SLMConstant.StaffType.OperOutbound
                                        || staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound
                                        || staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound)
                                    {
                                        btnReceive.Enabled = txtReceiveNo.Text.Trim() == "";
                                    }    
                                }
                            }
                        }
                    }
                    else //ผ่อนชำระ
                    {
                        if (supervisor)
                        {
                            btnReceive.Enabled = txtReceiveNo.Text.Trim() == "";
                        }   
                        else if (staff != null)
                        {
                            if (supervisor
                                        || staff.StaffTypeId == SLMConstant.StaffType.OperOutbound
                                        || staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound
                                        || staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound)
                            {
                                btnReceive.Enabled = txtReceiveNo.Text.Trim() == "";
                            }
                        }

                        if (!btnReceive.Enabled)
                        {
                            decimal paidAmount = ActivityLeadBiz.GetPaidPolicyAmount(data.RenewIns.slm_TicketId);
                            if (paidAmount > 0)
                            {
                                if (OwnerOrDelegate)
                                {
                                    btnReceive.Enabled = txtReceiveNo.Text.Trim() == "";
                                }
                            }
                        }
                    }
                }
                else if (Type == "2")   //KK MotorBox
                {
                    //var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                    if (staff == null)
                    {
                        return;
                    }

                    if (staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound)
                    {
                        btnReceive.Enabled = txtReceiveNo.Text.Trim() == "";
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void setReceiveActButton(PreleadCompareDataCollection data, string username, StaffData staff, bool OwnerOrDelegate, bool supervisor, bool actPurchaseFlag)
        {
            try
            {
                btnReceiveAct.Enabled = false;
                if (!rbAct_pro1.Checked && !rbAct_pro2.Checked && !rbAct_pro3.Checked)
                {
                    // case edit from page 
                    return;
                }
               
                if (!actPurchaseFlag)
                {
                    return;
                }

                if (staff == null)
                {
                    return;
                }

                //bool OwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(data.RenewIns.slm_TicketId, username);
                //bool supervisor = ActivityLeadBiz.CheckSupervisor(username);
                //var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);

                if (Type == "1")    //Motor Renew
                {
                    if (cmbPaymentmethodAct.SelectedValue == "1" || cmbPaymentmethodAct.SelectedValue == "") //ชำระเต็มจำนวน
                    {
                        if (ActCheckPaid == null)
                        {
                            ActCheckPaid = ActivityLeadBiz.checkPaidAct(data.RenewIns.slm_TicketId);
                        }

                        //bool Receipt = ActivityLeadBiz.checkPaidAct(data.RenewIns.slm_TicketId);
                        if (ActCheckPaid == true)
                        {
                            if (OwnerOrDelegate
                                || supervisor
                                || (staff != null && staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound))
                            {
                                btnReceiveAct.Enabled = txtActSendDate.Text.Trim() == "";
                            }
                        }
                        else
                        {
                            if (staff != null)
                            {
                                if (staff.StaffTypeId == SLMConstant.StaffType.OperOutbound
                                       || staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound
                                       || staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound
                                       || supervisor)
                                {
                                    btnReceiveAct.Enabled = txtActSendDate.Text.Trim() == "";
                                }   
                            }
                        }

                    }
                }
                else if (Type == "2")   //KK MotorBox
                {
                    if (staff == null)
                    {
                        return;
                    }

                    if (staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound)
                    {
                        btnReceiveAct.Enabled = txtActSendDate.Text.Trim() == "";
                    }
                }
                else
                {
                    if (staff != null)
                    {
                        if (staff.StaffTypeId == SLMConstant.StaffType.OperOutbound
                            || staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound
                            || staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound
                            || supervisor)
                        {
                            btnReceiveAct.Enabled = txtActSendDate.Text.Trim() == "";
                        }   
                    }
                    else
                    {
                        bool Receipt = ActivityLeadBiz.checkPaidAct(data.RenewIns.slm_TicketId);
                        if (Receipt)
                        {
                            if (OwnerOrDelegate)
                            {
                                btnReceiveAct.Enabled = txtActSendDate.Text.Trim() == "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Set Incentive Control
        /// </summary>
        private void setReceiveIncentiveButton(StaffData staff, bool OwnerOrDelegate, bool supervisor, bool policyPurchaseFlag)
        {
            try
            {
                var data = getSelectTabData();
                btnIncentive.Enabled = false;                            

                if (txtReceiveNo.Text != "" && txtReceiveDate.Text.Trim() != "" && txtIncentiveDate.Text.Trim() == "" && policyPurchaseFlag == true)
                {
                    if (PolicyCheckPaid == null)
                    {
                        PolicyCheckPaid = ActivityLeadBiz.checkPaidPolicy(data.RenewIns.slm_TicketId);
                    }

                    //bool Receipt = ActivityLeadBiz.checkPaidPolicy(data.RenewIns.slm_TicketId);

                    if (Type == "1")
                    {
                        //bool OwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(data.RenewIns.slm_TicketId, HttpContext.Current.User.Identity.Name);
                        //bool supervisor = ActivityLeadBiz.CheckSupervisor(HttpContext.Current.User.Identity.Name);

                        if (PolicyCheckPaid == true)
                        {
                            if (OwnerOrDelegate || supervisor)
                            {
                                btnIncentive.Enabled = true;
                            }
                            else
                            {
                                //var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                                if (staff != null)
                                {
                                    if (staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound)
                                    {
                                        btnIncentive.Enabled = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (Type == "2")
                    {
                        if (PolicyCheckPaid == true)
                        {
                            //var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                            if (staff != null)
                            {
                                if (staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound)
                                {
                                    btnIncentive.Enabled = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Set Act Incentive Control
        /// </summary>
        private void setReceiveIncentiveActButton(StaffData staff, bool OwnerOrDelegate, bool supervisor, bool actPurchaseFlag)
        {
            try
            {
                var data = getSelectTabData();
                btnIncentiveAct.Enabled = false;

                if (!rbAct_pro1.Checked && !rbAct_pro2.Checked && !rbAct_pro3.Checked)
                {
                    // case edit from page 
                    return;
                }

                //bool supervisor = ActivityLeadBiz.CheckSupervisor(HttpContext.Current.User.Identity.Name);

                if (txtActSendDate.Text.Trim() != "" && txtActIncentiveDate.Text.Trim() == "" && actPurchaseFlag == true)
                {
                    //bool OwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(data.RenewIns.slm_TicketId, HttpContext.Current.User.Identity.Name);

                    if (ActCheckPaid == null)
                    {
                        ActCheckPaid = ActivityLeadBiz.checkPaidAct(data.RenewIns.slm_TicketId);
                    }

                    //bool Receipt = ActivityLeadBiz.checkPaidAct(data.RenewIns.slm_TicketId);
                    if (Type == "1")
                    {
                        btnIncentiveAct.Enabled = false;

                        if (ActCheckPaid == true)
                        {
                            if (OwnerOrDelegate || supervisor)
                            {
                                btnIncentiveAct.Enabled = true;
                            }
                            else
                            {
                                //var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                                if (staff != null)
                                {
                                    if (staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound)
                                    {
                                        btnIncentiveAct.Enabled = true;
                                    } 
                                }
                            }
                        }
                    }
                    else if (Type == "2")
                    {
                        if (ActCheckPaid == true)
                        {
                            //var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                            if (staff != null)
                            {
                                if (staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound)
                                {
                                    btnIncentiveAct.Enabled = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Type 3
                        if (ActCheckPaid == true)
                        {
                            if (cmbPaymentmethodAct.SelectedValue == "1") //ชำระเต็มจำนวน
                            {
                                if (OwnerOrDelegate)
                                {
                                    btnIncentiveAct.Enabled = true;
                                }
                                else
                                {
                                    //var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                                    if (staff != null)
                                    {
                                        if ( // staff.StaffTypeId == SLMConstant.StaffType.SupervisorTelesalesOutbound || 
                                            staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound ||
                                            staff.StaffTypeId == SLMConstant.StaffType.OperOutbound ||
                                            supervisor)
                                        {
                                            btnIncentiveAct.Enabled = true;
                                        } 
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private PreleadCompareDataCollection PrepareForSavePreLead(PreleadCompareDataCollection data)
        {
            PreleadData prelead;

            if (data.Prelead == null)
            {
                prelead = new PreleadData();
            }
            else
            {
                prelead = data.Prelead;
            }

            prelead.slm_Brand_Code = cmbCarBrand.SelectedValue;
            prelead.slm_Model_Code = cmbCarName.SelectedValue;
            prelead.slm_Model_Year = cmbModelYear.SelectedItem.Value;
            prelead.slm_Cc = CCCar.Text.Trim();
            if (cmbInsuranceCarType.SelectedItem.Value != "")
            {
                prelead.slm_Car_By_Gov_Id = int.Parse(cmbInsuranceCarType.SelectedItem.Value);
            }
            prelead.slm_ProvinceRegis = cmbProvinceRegis.SelectedItem.Value != "" ? (int?)int.Parse(cmbProvinceRegis.SelectedItem.Value) : null;

            //Added by Pom 15/08/2016
            prelead.slm_RemarkPolicy = txtInsDesc.Text.Trim();
            prelead.slm_RemarkAct = txtRemarkAct.Text.Trim();
            prelead.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
            data.Prelead = prelead;
            return data;
        }

        private PreleadCompareDataCollection PrepareForSaveCompare(PreleadCompareDataCollection data)
        {
            return PrepareForSaveCompare(data, false);
        }

        private PreleadCompareDataCollection PrepareForSaveCompare(PreleadCompareDataCollection data, bool temp)
        {
            PreleadCompareData preDataPrev;
            if (data.ComparePrev != null)
            {
                preDataPrev = data.ComparePrev;
            }
            else
            {
                preDataPrev = new PreleadCompareData();
            }

            PreleadCompareData preDataCurr;
            if (data.CompareCurr != null)
            {
                preDataCurr = data.CompareCurr;
            }
            else
            {
                preDataCurr = new PreleadCompareData();
            }

            List<PreleadCompareData> promoList;
            if (data.ComparePromoList != null)
            {
                promoList = data.ComparePromoList;
            }
            else
            {
                promoList = new List<PreleadCompareData>();
            }

            if (data.RenewIns != null)
            {
                preDataPrev.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
            }
            preDataPrev.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
            preDataPrev.slm_NotifyPremiumId = null;
            preDataPrev.slm_PromotionId = null;
            preDataPrev.slm_Seq = 1;
            preDataPrev.slm_Year = lblYear_pre.Text;
            preDataPrev.slm_Ins_Com_Id = AppUtil.SafeDecimal(lblInsCom_Id_pre.Text);
            preDataPrev.slm_CoverageTypeId = AppUtil.SafeInt(hidCoverageType_pre.Value);
            preDataPrev.slm_InjuryDeath = null;//
            preDataPrev.slm_TPPD = null;//
            preDataPrev.slm_RepairTypeId = AppUtil.SafeInt(hidMaintanance_pre.Value);
            preDataPrev.slm_OD = lblVoluntary_Cov_Amt_pre.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Cov_Amt_pre.Text);
            preDataPrev.slm_FT = lblCostFT_pre.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCostFT_pre.Text);
            preDataPrev.slm_DeDuctible = lblCost_pre.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCost_pre.Text);
            preDataPrev.slm_PersonalAccident = null;//
            preDataPrev.slm_PersonalAccidentPassenger = null;//
            preDataPrev.slm_PersonalAccidentDriver = null;//
            preDataPrev.slm_MedicalFee = null;//
            preDataPrev.slm_MedicalFeePassenger = null;//
            preDataPrev.slm_MedicalFeeDriver = null;//
            preDataPrev.slm_InsuranceDriver = null;//
            preDataPrev.slm_PolicyGrossStamp = lblDuty_pre.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblDuty_pre.Text);
            preDataPrev.slm_PolicyGrossVat = lblVat_amount_pre.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVat_amount_pre.Text);
            preDataPrev.slm_PolicyGrossPremium = lblVoluntary_Gross_Premium_pre.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pre.Text);
            preDataPrev.slm_NetGrossPremium = lblVoluntary_Gross_Premium_pre.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pre.Text);//
            preDataPrev.slm_PolicyGrossPremiumPay = lblTotal_Voluntary_Gross_Premium_pre.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblTotal_Voluntary_Gross_Premium_pre.Text);//
            preDataPrev.slm_CostSave = txtSafe_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(txtSafe_cur.Text);//
            preDataPrev.slm_Selected = false;//
            preDataPrev.slm_DriverFlag = hidDriver_Flag_pre.Value;
            preDataPrev.slm_OldPolicyNo = lblVoluntary_Policy_Number_pre.Text;
            preDataPrev.slm_DriverFlag = hidDriver_Flag_pre.Value;//
            preDataPrev.slm_Driver_TitleId1 = AppUtil.SafeInt(hidTitleName1_pre.Value);
            preDataPrev.slm_Driver_First_Name1 = lblDriver_First_Name1_pre.Text;
            preDataPrev.slm_Driver_Last_Name1 = lblDriver_Last_Name1_pre.Text;
            preDataPrev.slm_Driver_Birthdate1 = lblDriver_Birthdate1_pre.Text != "" ? (DateTime?)Convert.ToDateTime(lblDriver_Birthdate1_pre.Text) : null;
            preDataPrev.slm_Driver_TitleId2 = AppUtil.SafeInt(hidTitleName2_pre.Value);
            preDataPrev.slm_Driver_First_Name2 = lblDriver_First_Name2_pre.Text;
            preDataPrev.slm_Driver_Last_Name2 = lblDriver_Last_Name2_pre.Text;
            preDataPrev.slm_Driver_Birthdate2 = lblDriver_Birthdate2_pre.Text != "" ? (DateTime?)Convert.ToDateTime(lblDriver_Birthdate2_pre.Text) : null;
            preDataPrev.slm_OldReceiveNo = lblInformed_pre.Text;
            preDataPrev.slm_PolicyStartCoverDate = lblVoluntary_Policy_Eff_Date_pre.DateValue != DateTime.MinValue ? (DateTime?)lblVoluntary_Policy_Eff_Date_pre.DateValue : null; ;
            preDataPrev.slm_PolicyEndCoverDate = lblVoluntary_Policy_Exp_Date_pre.DateValue != DateTime.MinValue ? (DateTime?)lblVoluntary_Policy_Exp_Date_pre.DateValue : null; ;
            preDataPrev.slm_Vat1Percent = chkCardType.Checked ? true : false;
            preDataPrev.slm_DiscountPercent = lblDiscountPercent_pre.Text == "" ? null : (decimal?)AppUtil.SafeInt(lblDiscountPercent_pre.Text);
            preDataPrev.slm_DiscountBath = lblDiscountBath_pre.Text != "" ? (decimal?)Convert.ToDecimal(lblDiscountBath_pre.Text) : null;
            preDataPrev.slm_Vat1PercentBath = lblPersonType_pre.Text != "" && lblCardTypeId_pre.Text == "นิติบุคคล" ? (decimal?)Convert.ToDecimal(lblPersonType_pre.Text) : null;

            preDataPrev.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
            preDataPrev.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;

            if (data.ComparePrev == null)
            {
                data.ComparePrev = preDataPrev;
            }

            if (data.RenewIns != null)
            {
                preDataCurr.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
            }
            preDataCurr.slm_Selected = rbInsNameTh_cur.Checked ? true : false;
            preDataCurr.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
            preDataCurr.slm_NotifyPremiumId = AppUtil.SafeInt(hidNotifyPremiumId.Value);
            preDataCurr.slm_PromotionId = null;
            preDataCurr.slm_Seq = 2;
            preDataCurr.slm_Year = lblExpireDate.Text;
            preDataCurr.slm_Ins_Com_Id = AppUtil.SafeDecimal(lblInsNameTh_cur.SelectedValue);
            preDataCurr.slm_CoverageTypeId = AppUtil.SafeInt(lblCoverageType_cur.SelectedValue);
            preDataCurr.slm_InjuryDeath = null;//
            preDataCurr.slm_TPPD = null;//
            preDataCurr.slm_RepairTypeId = AppUtil.SafeInt(lblMaintanance_cur.SelectedValue);
            preDataCurr.slm_OD = lblVoluntary_Cov_Amt_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Cov_Amt_cur.Text);
            preDataCurr.slm_FT = lblCostFT_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCostFT_cur.Text);
            preDataCurr.slm_DeDuctible = lblCost_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCost_cur.Text);
            preDataCurr.slm_DeDuctibleFlag = cmbDeDuctFlag_cur.SelectedItem.Value;      //Added by Pom 15/08/2016
            preDataCurr.slm_PersonalAccident = null;//
            preDataCurr.slm_PersonalAccidentPassenger = null;//
            preDataCurr.slm_PersonalAccidentDriver = null;//
            preDataCurr.slm_MedicalFee = null;//
            preDataCurr.slm_MedicalFeePassenger = null;//
            preDataCurr.slm_MedicalFeeDriver = null;//
            preDataCurr.slm_InsuranceDriver = null;//
            preDataCurr.slm_PolicyGrossStamp = lblDuty_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblDuty_cur.Text);
            preDataCurr.slm_PolicyGrossVat = lblVat_amount_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVat_amount_cur.Text);
            preDataCurr.slm_PolicyGrossPremium = lblNetpremium_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblNetpremium_cur.Text);
            preDataCurr.slm_NetGrossPremium = lblVoluntary_Gross_Premium_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_cur.Text);
            preDataCurr.slm_PolicyGrossPremiumPay = txtTotal_Voluntary_Gross_Premium_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_cur.Text);//
            preDataCurr.slm_CostSave = txtSafe_cur.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(txtSafe_cur.Text);//

            if (rbInsNameTh_cur.Checked)
            {
                preDataCurr.slm_Selected = true;//
            }
            else
            {
                preDataCurr.slm_Selected = false;
            }

            preDataCurr.slm_OldPolicyNo = lblVoluntary_Policy_Number_cur.Text;

            if (rbDriver_Flag_cur1.Checked)
            {
                preDataCurr.slm_DriverFlag = "1";//
            }
            else
            {
                preDataCurr.slm_DriverFlag = "0";//
            }

            preDataCurr.slm_Driver_TitleId1 = AppUtil.SafeInt(cmbTitleName1_cur.Text);
            preDataCurr.slm_Driver_First_Name1 = txtDriver_First_Name1_cur.Text;
            preDataCurr.slm_Driver_Last_Name1 = txtDriver_Last_Name1_cur.Text;
            preDataCurr.slm_Driver_Birthdate1 = tdmDriver_Birthdate1_cur.DateValue != DateTime.MinValue ? (DateTime?)tdmDriver_Birthdate1_cur.DateValue : null;
            preDataCurr.slm_Driver_TitleId2 = AppUtil.SafeInt(cmbTitleName2_cur.Text);
            preDataCurr.slm_Driver_First_Name2 = txtDriver_First_Name2_cur.Text;
            preDataCurr.slm_Driver_Last_Name2 = txtDriver_Last_Name2_cur.Text;
            preDataCurr.slm_Driver_Birthdate2 = tdmDriver_Birthdate2_cur.DateValue != DateTime.MinValue ? (DateTime?)tdmDriver_Birthdate2_cur.DateValue : null;
            preDataCurr.slm_OldReceiveNo = lblInformed_cur.Text;

            if (lblVoluntary_Policy_Eff_Date_cur.DateValue == DateTime.MinValue)
            {
                preDataCurr.slm_PolicyStartCoverDate = null;
            }
            else
            {
                preDataCurr.slm_PolicyStartCoverDate = lblVoluntary_Policy_Eff_Date_cur.DateValue;
            }

            if (lblVoluntary_Policy_Exp_Date_cur.DateValue == DateTime.MinValue)
            {
                preDataCurr.slm_PolicyEndCoverDate = null;
            }
            else
            {
                preDataCurr.slm_PolicyEndCoverDate = lblVoluntary_Policy_Exp_Date_cur.DateValue;
            }

            preDataCurr.slm_Vat1Percent = chkCardType.Checked ? true : false;
            preDataCurr.slm_DiscountPercent = txtDiscountPercent_cur.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtDiscountPercent_cur.Text);
            preDataCurr.slm_DiscountBath = txtDiscountBath_cur.Text != "" ? (decimal?)Convert.ToDecimal(txtDiscountBath_cur.Text) : null;
            preDataCurr.slm_Vat1PercentBath = lblPersonType_cur.Text != "" && lblCardTypeId_pre.Text == "นิติบุคคล" ? (decimal?)Convert.ToDecimal(lblPersonType_cur.Text) : null;
            preDataCurr.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
            preDataCurr.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;

            if (data.CompareCurr == null)
            {
                data.CompareCurr = preDataCurr;
            }

            var i = 3;

            if (hidPromotionId1.Value != "")
            {
                PreleadCompareData preDataPromo1;

                if (hidPromotionId1.Value == "0")
                {
                    preDataPromo1 = data.ComparePromoList.Where(cp => cp.slm_Seq == decimal.Parse(hidSeq1.Value)).FirstOrDefault();
                }
                else
                {
                    preDataPromo1 = data.ComparePromoList.Where(cp => cp.slm_PromotionId == decimal.Parse(hidPromotionId1.Value)).FirstOrDefault();
                }

                if (preDataPromo1 == null)
                {
                    preDataPromo1 = new PreleadCompareData();
                }
                //Pro1
                if (data.RenewIns != null)
                {
                    preDataPromo1.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                }

                preDataPromo1.slm_Selected = rbInsNameTh_pro1.Checked ? true : false;
                preDataPromo1.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
                preDataPromo1.slm_NotifyPremiumId = null;
                preDataPromo1.slm_PromotionId = AppUtil.SafeDecimal(hidPromotionId1.Value);
                if (!temp)
                {
                    preDataPromo1.slm_Seq = i;
                }
                i++;
                preDataPromo1.slm_Year = lblExpireDate.Text;
                preDataPromo1.slm_Ins_Com_Id = AppUtil.SafeDecimal(lblInsNameTh_pro1.SelectedValue);
                preDataPromo1.slm_CoverageTypeId = AppUtil.SafeInt(lblCoverageType_pro1.SelectedValue);
                preDataPromo1.slm_InjuryDeath = AppUtil.SafeDecimal(hidInjuryDeath_pro1.Value);//
                preDataPromo1.slm_TPPD = AppUtil.SafeDecimal(hidTPPD_pro1.Value);//
                preDataPromo1.slm_RepairTypeId = AppUtil.SafeInt(lblMaintanance_pro1.SelectedValue);
                preDataPromo1.slm_OD = lblVoluntary_Cov_Amt_pro1.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Cov_Amt_pro1.Text);
                preDataPromo1.slm_FT = lblCostFT_pro1.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCostFT_pro1.Text);
                preDataPromo1.slm_DeDuctible = lblCost_pro1.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCost_pro1.Text);
                preDataPromo1.slm_DeDuctibleFlag = cmbDeDuctFlag_pro1.SelectedItem.Value;               //Added by Pom 15/08/2016
                preDataPromo1.slm_PersonalAccident = AppUtil.SafeDecimal(hidPersonalAccident_pro1.Value);//
                preDataPromo1.slm_PersonalAccidentPassenger = hidPersonalAccidentPassenger_pro1.Value;//
                preDataPromo1.slm_PersonalAccidentDriver = hidPersonalAccidentDriver_pro1.Value;//
                preDataPromo1.slm_MedicalFee = AppUtil.SafeDecimal(hidMedicalFee_pro1.Value);//
                preDataPromo1.slm_MedicalFeePassenger = hidMedicalFeePassenger_pro1.Value;//
                preDataPromo1.slm_MedicalFeeDriver = hidMedicalFeeDriver_pro1.Value;//
                preDataPromo1.slm_InsuranceDriver = AppUtil.SafeDecimal(hidInsuranceDriver_pro1.Value);//
                preDataPromo1.slm_PolicyGrossStamp = lblDuty_pro1.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblDuty_pro1.Text);
                preDataPromo1.slm_PolicyGrossVat = lblVat_amount_pro1.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVat_amount_pro1.Text);
                preDataPromo1.slm_PolicyGrossPremium = lblNetpremium_pro1.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblNetpremium_pro1.Text);
                preDataPromo1.slm_NetGrossPremium = lblVoluntary_Gross_Premium_pro1.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro1.Text);//
                preDataPromo1.slm_PolicyGrossPremiumPay = txtTotal_Voluntary_Gross_Premium_pro1.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro1.Text);//
                preDataPromo1.slm_CostSave = txtSafe_pro1.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(txtSafe_pro1.Text);//

                if (rbInsNameTh_pro1.Checked)
                {
                    preDataPromo1.slm_Selected = true;//
                }
                else
                {
                    preDataPromo1.slm_Selected = false;
                }

                preDataPromo1.slm_OldPolicyNo = lblVoluntary_Policy_Number_pro1.Text;

                if (rbDriver_Flag_pro11.Checked)
                {
                    preDataPromo1.slm_DriverFlag = "1";//
                }
                else
                {
                    preDataPromo1.slm_DriverFlag = "0";//
                }

                preDataPromo1.slm_Driver_TitleId1 = AppUtil.SafeInt(cmbTitleName1_pro1.Text);
                preDataPromo1.slm_Driver_First_Name1 = txtDriver_First_Name1_pro1.Text;
                preDataPromo1.slm_Driver_Last_Name1 = txtDriver_Last_Name1_pro1.Text;
                preDataPromo1.slm_Driver_Birthdate1 = tdmDriver_Birthdate1_pro1.DateValue != DateTime.MinValue ? (DateTime?)tdmDriver_Birthdate1_pro1.DateValue : null;
                preDataPromo1.slm_Driver_TitleId2 = AppUtil.SafeInt(cmbTitleName2_pro1.Text);
                preDataPromo1.slm_Driver_First_Name2 = txtDriver_First_Name2_pro1.Text;
                preDataPromo1.slm_Driver_Last_Name2 = txtDriver_Last_Name2_pro1.Text;
                preDataPromo1.slm_Driver_Birthdate2 = tdmDriver_Birthdate2_pro1.DateValue != DateTime.MinValue ? (DateTime?)tdmDriver_Birthdate2_pro1.DateValue : null;
                preDataPromo1.slm_OldReceiveNo = lblInformed_pro1.Text;

                if (lblVoluntary_Policy_Eff_Date_pro1.DateValue == DateTime.MinValue)
                {
                    preDataPromo1.slm_PolicyStartCoverDate = null;
                }
                else
                {
                    preDataPromo1.slm_PolicyStartCoverDate = lblVoluntary_Policy_Eff_Date_pro1.DateValue;
                }

                if (lblVoluntary_Policy_Exp_Date_pro1.DateValue == DateTime.MinValue)
                {
                    preDataPromo1.slm_PolicyEndCoverDate = null;
                }
                else
                {
                    preDataPromo1.slm_PolicyEndCoverDate = lblVoluntary_Policy_Exp_Date_pro1.DateValue;
                }

                preDataPromo1.slm_Vat1Percent = chkCardType.Checked ? true : false;
                preDataPromo1.slm_DiscountPercent = txtDiscountPercent_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtDiscountPercent_pro1.Text);
                preDataPromo1.slm_DiscountBath = txtDiscountBath_pro1.Text != "" ? (decimal?)Convert.ToDecimal(txtDiscountBath_pro1.Text) : null;
                preDataPromo1.slm_Vat1PercentBath = lblPersonType_pro1.Text != "" && lblCardTypeId_pre.Text == "นิติบุคคล" ? (decimal?)Convert.ToDecimal(lblPersonType_pro1.Text) : null;
                preDataPromo1.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                preDataPromo1.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
            }

            if (hidPromotionId2.Value != "")
            {
                PreleadCompareData preDataPromo2;

                if (hidPromotionId2.Value == "0")
                {
                    preDataPromo2 = data.ComparePromoList.Where(cp => cp.slm_Seq == decimal.Parse(hidSeq2.Value)).FirstOrDefault();
                }
                else
                {
                    preDataPromo2 = data.ComparePromoList.Where(cp => cp.slm_PromotionId == decimal.Parse(hidPromotionId2.Value)).FirstOrDefault();
                }

                if (preDataPromo2 == null)
                {
                    preDataPromo2 = new PreleadCompareData();
                }

                //Pro2
                if (data.RenewIns != null)
                {
                    preDataPromo2.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                }

                preDataPromo2.slm_Selected = rbInsNameTh_pro2.Checked ? true : false;
                preDataPromo2.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
                preDataPromo2.slm_NotifyPremiumId = null;
                preDataPromo2.slm_PromotionId = AppUtil.SafeDecimal(hidPromotionId2.Value);
                if (!temp)
                {
                    preDataPromo2.slm_Seq = i;
                }
                i++;
                preDataPromo2.slm_Year = lblExpireDate.Text;
                preDataPromo2.slm_Ins_Com_Id = AppUtil.SafeDecimal(lblInsNameTh_pro2.SelectedValue);
                preDataPromo2.slm_CoverageTypeId = AppUtil.SafeInt(lblCoverageType_pro2.SelectedValue);
                preDataPromo2.slm_InjuryDeath = AppUtil.SafeDecimal(hidInjuryDeath_pro2.Value);//
                preDataPromo2.slm_TPPD = AppUtil.SafeDecimal(hidTPPD_pro2.Value);//
                preDataPromo2.slm_RepairTypeId = AppUtil.SafeInt(lblMaintanance_pro2.SelectedValue);
                preDataPromo2.slm_OD = lblVoluntary_Cov_Amt_pro2.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Cov_Amt_pro2.Text);
                preDataPromo2.slm_FT = lblCostFT_pro2.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCostFT_pro2.Text);
                preDataPromo2.slm_DeDuctible = lblCost_pro2.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCost_pro2.Text);
                preDataPromo2.slm_DeDuctibleFlag = cmbDeDuctFlag_pro2.SelectedItem.Value;    //Added by Pom 15/08/2016
                preDataPromo2.slm_PersonalAccident = AppUtil.SafeDecimal(hidPersonalAccident_pro2.Value);//
                preDataPromo2.slm_PersonalAccidentPassenger = hidPersonalAccidentPassenger_pro2.Value;//
                preDataPromo2.slm_PersonalAccidentDriver = hidPersonalAccidentDriver_pro2.Value;//
                preDataPromo2.slm_MedicalFee = AppUtil.SafeDecimal(hidMedicalFee_pro2.Value);//
                preDataPromo2.slm_MedicalFeePassenger = hidMedicalFeePassenger_pro2.Value;//
                preDataPromo2.slm_MedicalFeeDriver = hidMedicalFeeDriver_pro2.Value;//
                preDataPromo2.slm_InsuranceDriver = AppUtil.SafeDecimal(hidInsuranceDriver_pro2.Value);//
                preDataPromo2.slm_PolicyGrossStamp = lblDuty_pro2.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblDuty_pro2.Text);
                preDataPromo2.slm_PolicyGrossVat = lblVat_amount_pro2.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVat_amount_pro2.Text);
                preDataPromo2.slm_PolicyGrossPremium = lblNetpremium_pro2.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblNetpremium_pro2.Text);
                preDataPromo2.slm_NetGrossPremium = lblVoluntary_Gross_Premium_pro2.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro2.Text);//
                preDataPromo2.slm_PolicyGrossPremiumPay = txtTotal_Voluntary_Gross_Premium_pro2.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro2.Text);//
                preDataPromo2.slm_CostSave = txtSafe_pro2.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(txtSafe_pro2.Text);//

                if (rbInsNameTh_pro2.Checked)
                {
                    preDataPromo2.slm_Selected = true;//
                }
                else
                {
                    preDataPromo2.slm_Selected = false;
                }

                preDataPromo2.slm_OldPolicyNo = lblVoluntary_Policy_Number_pro2.Text;

                if (rbDriver_Flag_pro21.Checked)
                {
                    preDataPromo2.slm_DriverFlag = "1";//
                }
                else
                {
                    preDataPromo2.slm_DriverFlag = "0";//
                }

                preDataPromo2.slm_Driver_TitleId1 = AppUtil.SafeInt(cmbTitleName1_pro2.Text);
                preDataPromo2.slm_Driver_First_Name1 = txtDriver_First_Name1_pro2.Text;
                preDataPromo2.slm_Driver_Last_Name1 = txtDriver_Last_Name1_pro2.Text;
                preDataPromo2.slm_Driver_Birthdate1 = tdmDriver_Birthdate1_pro2.DateValue != DateTime.MinValue ? (DateTime?)tdmDriver_Birthdate1_pro2.DateValue : null;
                preDataPromo2.slm_Driver_TitleId2 = AppUtil.SafeInt(cmbTitleName2_pro2.Text);
                preDataPromo2.slm_Driver_First_Name2 = txtDriver_First_Name2_pro2.Text;
                preDataPromo2.slm_Driver_Last_Name2 = txtDriver_Last_Name2_pro2.Text;
                preDataPromo2.slm_Driver_Birthdate2 = tdmDriver_Birthdate2_pro2.DateValue != DateTime.MinValue ? (DateTime?)tdmDriver_Birthdate2_pro2.DateValue : null;
                preDataPromo2.slm_OldReceiveNo = lblInformed_pro2.Text;
                if (lblVoluntary_Policy_Eff_Date_pro2.DateValue == DateTime.MinValue)
                {
                    preDataPromo2.slm_PolicyStartCoverDate = null;
                }
                else
                {
                    preDataPromo2.slm_PolicyStartCoverDate = Convert.ToDateTime(lblVoluntary_Policy_Eff_Date_pro2.DateValue);
                }

                if (lblVoluntary_Policy_Exp_Date_pro2.DateValue == DateTime.MinValue)
                {
                    preDataPromo2.slm_PolicyEndCoverDate = null;
                }
                else
                {
                    preDataPromo2.slm_PolicyEndCoverDate = Convert.ToDateTime(lblVoluntary_Policy_Exp_Date_pro2.DateValue);
                }
                preDataPromo2.slm_Vat1Percent = chkCardType.Checked ? true : false;
                preDataPromo2.slm_DiscountPercent = txtDiscountPercent_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtDiscountPercent_pro2.Text);
                preDataPromo2.slm_DiscountBath = txtDiscountBath_pro2.Text != "" ? (decimal?)Convert.ToDecimal(txtDiscountBath_pro2.Text) : null;
                preDataPromo2.slm_Vat1PercentBath = lblPersonType_pro2.Text != "" && lblCardTypeId_pre.Text == "นิติบุคคล" ? (decimal?)Convert.ToDecimal(lblPersonType_pro2.Text) : null;
                preDataPromo2.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                preDataPromo2.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
            }

            if (hidPromotionId3.Value != "")
            {
                PreleadCompareData preDataPromo3;
                if (hidPromotionId3.Value == "0")
                {
                    preDataPromo3 = data.ComparePromoList.Where(cp => cp.slm_Seq == decimal.Parse(hidSeq3.Value)).FirstOrDefault();
                }
                else
                {
                    preDataPromo3 = data.ComparePromoList.Where(cp => cp.slm_PromotionId == decimal.Parse(hidPromotionId3.Value)).FirstOrDefault();
                }

                if (preDataPromo3 == null)
                {
                    preDataPromo3 = new PreleadCompareData();
                }
                //Pro3
                if (data.RenewIns != null)
                {
                    preDataPromo3.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                }

                preDataPromo3.slm_Selected = rbInsNameTh_pro3.Checked ? true : false;
                preDataPromo3.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
                preDataPromo3.slm_NotifyPremiumId = null;
                preDataPromo3.slm_PromotionId = AppUtil.SafeDecimal(hidPromotionId3.Value);
                if (!temp)
                {
                    preDataPromo3.slm_Seq = i;
                }
                i++;
                preDataPromo3.slm_Year = lblExpireDate.Text;
                preDataPromo3.slm_Ins_Com_Id = AppUtil.SafeDecimal(lblInsNameTh_pro3.SelectedValue);
                preDataPromo3.slm_CoverageTypeId = AppUtil.SafeInt(lblCoverageType_pro3.SelectedValue);
                preDataPromo3.slm_InjuryDeath = AppUtil.SafeDecimal(hidInjuryDeath_pro3.Value);//
                preDataPromo3.slm_TPPD = AppUtil.SafeDecimal(hidTPPD_pro3.Value);//
                preDataPromo3.slm_RepairTypeId = AppUtil.SafeInt(lblMaintanance_pro3.SelectedValue);
                preDataPromo3.slm_OD = lblVoluntary_Cov_Amt_pro3.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Cov_Amt_pro3.Text);
                preDataPromo3.slm_FT = lblCostFT_pro3.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCostFT_pro3.Text);
                preDataPromo3.slm_DeDuctible = lblCost_pro3.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblCost_pro3.Text);
                preDataPromo3.slm_DeDuctibleFlag = cmbDeDuctFlag_pro3.SelectedItem.Value;       //Added by Pom 15/08/2016
                preDataPromo3.slm_PersonalAccident = AppUtil.SafeDecimal(hidPersonalAccident_pro3.Value);//
                preDataPromo3.slm_PersonalAccidentPassenger = hidPersonalAccidentPassenger_pro3.Value;//
                preDataPromo3.slm_PersonalAccidentDriver = hidPersonalAccidentDriver_pro3.Value;//
                preDataPromo3.slm_MedicalFee = AppUtil.SafeDecimal(hidMedicalFee_pro3.Value);//
                preDataPromo3.slm_MedicalFeePassenger = hidMedicalFeePassenger_pro3.Value;//
                preDataPromo3.slm_MedicalFeeDriver = hidMedicalFeeDriver_pro3.Value;//
                preDataPromo3.slm_InsuranceDriver = AppUtil.SafeDecimal(hidInsuranceDriver_pro3.Value);//
                preDataPromo3.slm_PolicyGrossStamp = lblDuty_pro3.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblDuty_pro3.Text);
                preDataPromo3.slm_PolicyGrossVat = lblVat_amount_pro3.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVat_amount_pro3.Text);
                preDataPromo3.slm_PolicyGrossPremium = lblNetpremium_pro3.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblNetpremium_pro3.Text);
                preDataPromo3.slm_NetGrossPremium = lblVoluntary_Gross_Premium_pro3.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro3.Text);//
                preDataPromo3.slm_PolicyGrossPremiumPay = txtTotal_Voluntary_Gross_Premium_pro3.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro3.Text);//
                preDataPromo3.slm_CostSave = txtSafe_pro3.Text == "" ? null : (Decimal?)AppUtil.SafeDecimal(txtSafe_pro3.Text);//

                if (rbInsNameTh_pro3.Checked)
                {
                    preDataPromo3.slm_Selected = true;//
                }
                else
                {
                    preDataPromo3.slm_Selected = false;
                }

                preDataPromo3.slm_OldPolicyNo = lblVoluntary_Policy_Number_pro3.Text;

                if (rbDriver_Flag_pro31.Checked)
                {
                    preDataPromo3.slm_DriverFlag = "1";//
                }
                else
                {
                    preDataPromo3.slm_DriverFlag = "0";//
                }

                preDataPromo3.slm_Driver_TitleId1 = AppUtil.SafeInt(cmbTitleName1_pro3.Text);
                preDataPromo3.slm_Driver_First_Name1 = txtDriver_First_Name1_pro3.Text;
                preDataPromo3.slm_Driver_Last_Name1 = txtDriver_Last_Name1_pro3.Text;
                preDataPromo3.slm_Driver_Birthdate1 = tdmDriver_Birthdate1_pro3.DateValue != DateTime.MinValue ? (DateTime?)tdmDriver_Birthdate1_pro3.DateValue : null;
                preDataPromo3.slm_Driver_TitleId2 = AppUtil.SafeInt(cmbTitleName2_pro3.Text);
                preDataPromo3.slm_Driver_First_Name2 = txtDriver_First_Name2_pro3.Text;
                preDataPromo3.slm_Driver_Last_Name2 = txtDriver_Last_Name2_pro3.Text;
                preDataPromo3.slm_Driver_Birthdate2 = tdmDriver_Birthdate2_pro3.DateValue != DateTime.MinValue ? (DateTime?)tdmDriver_Birthdate2_pro3.DateValue : null;
                preDataPromo3.slm_OldReceiveNo = lblInformed_pro3.Text;
                if (lblVoluntary_Policy_Eff_Date_pro3.DateValue == DateTime.MinValue)
                {
                    preDataPromo3.slm_PolicyStartCoverDate = null;
                }
                else
                {
                    preDataPromo3.slm_PolicyStartCoverDate = Convert.ToDateTime(lblVoluntary_Policy_Eff_Date_pro3.DateValue);
                }

                if (lblVoluntary_Policy_Exp_Date_pro3.DateValue == DateTime.MinValue)
                {
                    preDataPromo3.slm_PolicyEndCoverDate = null;
                }
                else
                {
                    preDataPromo3.slm_PolicyEndCoverDate = Convert.ToDateTime(lblVoluntary_Policy_Exp_Date_pro3.DateValue);
                }
                preDataPromo3.slm_Vat1Percent = chkCardType.Checked ? true : false;
                preDataPromo3.slm_DiscountPercent = txtDiscountPercent_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtDiscountPercent_pro3.Text);
                preDataPromo3.slm_DiscountBath = txtDiscountBath_pro3.Text != "" ? (decimal?)Convert.ToDecimal(txtDiscountBath_pro3.Text) : null;
                preDataPromo3.slm_Vat1PercentBath = lblPersonType_pro3.Text != "" && lblCardTypeId_pre.Text == "นิติบุคคล" ? (decimal?)Convert.ToDecimal(lblPersonType_pro3.Text) : null;
                preDataPromo3.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                preDataPromo3.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
            }
            return data;
        }

        private PreleadCompareDataCollection PrepareForSaveAct(PreleadCompareDataCollection data)
        {
            PreleadCompareActData preDataPrev;
            if (data.ActPrev != null)
            {
                preDataPrev = data.ActPrev;
            }
            else
            {
                preDataPrev = new PreleadCompareActData();
            }

            List<PreleadCompareActData> promoList;
            if (data.ActPromoList != null)
            {
                promoList = data.ActPromoList;
            }
            else
            {
                promoList = new List<PreleadCompareActData>();
            }

            var i = 2;


            if (data.RenewIns != null)
            {
                preDataPrev.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
            }
            preDataPrev.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
            preDataPrev.slm_PromotionId = null;
            preDataPrev.slm_Seq = 1;
            preDataPrev.slm_Year = lblPromoAct_name_pre.Text;//
            preDataPrev.slm_Ins_Com_Id = AppUtil.SafeInt(lblActInsCom_Id_pre.Text);
            preDataPrev.slm_ActIssuePlace = AppUtil.SafeInt(hidActIssuePlace_pre.Value);

            preDataPrev.slm_ActNo = null;//
            preDataPrev.slm_ActSignNo = lbltxtSignNoAct_pre.Text;
            preDataPrev.slm_ActStartCoverDate = lblActStartCoverDateAct_pre.Text != "" ? (DateTime?)DateTime.ParseExact(lblActStartCoverDateAct_pre.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
            preDataPrev.slm_ActEndCoverDate = lblActEndCoverDateAct_pre.Text != "" ? (DateTime?)DateTime.ParseExact(lblActEndCoverDateAct_pre.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
            preDataPrev.slm_ActIssueBranch = hidActIssueBranch_pre.Value;//
            preDataPrev.slm_CarTaxExpiredDate = lblCarTaxExpiredDateAct_pre.Text != "" ? (DateTime?)Convert.ToDateTime(lblCarTaxExpiredDateAct_pre.Text) : null;
            preDataPrev.slm_ActGrossStamp = lblActGrossStampAct_pre.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossStampAct_pre.Text);
            preDataPrev.slm_ActGrossVat = lblActGrossVatAct_pre.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossVatAct_pre.Text);
            preDataPrev.slm_ActGrossPremium = lblActGrossPremiumAct_pre.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossPremiumAct_pre.Text);
            preDataPrev.slm_ActNetGrossPremium = lblActNetGrossPremiumAct_pre.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pre.Text);
            preDataPrev.slm_ActGrossPremiumPay = lblDiscountBathAct_pre.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblDiscountBathAct_pre.Text);
            preDataPrev.slm_ActPurchaseFlag = null;
            preDataPrev.slm_Vat1Percent = chkCardType2.Checked ? true : false;
            preDataPrev.slm_Vat1PercentBath = lblActPersonType_pre.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActPersonType_pre.Text);
            preDataPrev.slm_DiscountPercent = lblVat1PercentBathAct_pre.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblVat1PercentBathAct_pre.Text);
            preDataPrev.slm_DiscountBath = lblDiscountPercentAct_pre.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblDiscountPercentAct_pre.Text);
            preDataPrev.slm_ActNetGrossPremiumFull = lblActNetGrossPremiumFullAct_pre.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pre.Text);
            preDataPrev.slm_ActPurchaseFlag = false;

            if (data.ActPrev == null)
            {
                data.ActPrev = preDataPrev;
            }

            data.ActPrev.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
            data.ActPrev.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;

            if (hidPromotionActId1.Value != "")
            {
                PreleadCompareActData preDataPromo1;

                if (hidPromotionActId1.Value == "0")
                {
                    preDataPromo1 = data.ActPromoList.Where(cp => cp.slm_Seq == decimal.Parse(hidSeqAct1.Value)).FirstOrDefault();
                }
                else
                {
                    preDataPromo1 = data.ActPromoList.Where(cp => cp.slm_PromotionId == decimal.Parse(hidPromotionActId1.Value)).FirstOrDefault();
                }


                if (preDataPromo1 == null)
                {
                    preDataPromo1 = new PreleadCompareActData();
                }
                if (data.RenewIns != null)
                {
                    preDataPromo1.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                }
                preDataPromo1.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
                preDataPromo1.slm_PromotionId = AppUtil.SafeDecimal(hidPromotionActId1.Value);
                preDataPromo1.slm_Seq = i;
                i++;
                preDataPromo1.slm_Year = lblExpireDate.Text;
                preDataPromo1.slm_Ins_Com_Id = AppUtil.SafeInt(lblCompanyInsuranceAct_pro1.SelectedValue);
                preDataPromo1.slm_ActIssuePlace = AppUtil.SafeInt(cmbActIssuePlace_pro1.SelectedValue);

                preDataPromo1.slm_SendDocType = rbRegisterAct_pro1.Checked ? 1 : rbNormalAct_pro1.Checked ? 2 : 0;

                preDataPromo1.slm_ActNo = null;//
                preDataPromo1.slm_ActSignNo = txtSignNoAct_pro1.Text;
                preDataPromo1.slm_ActStartCoverDate = txtActStartCoverDateAct_pro1.DateValue == DateTime.MinValue ? null : (DateTime?)txtActStartCoverDateAct_pro1.DateValue;
                preDataPromo1.slm_ActEndCoverDate = txtActEndCoverDateAct_pro1.DateValue == DateTime.MinValue ? null : (DateTime?)txtActEndCoverDateAct_pro1.DateValue;
                preDataPromo1.slm_ActIssueBranch = cmbActIssueBranch_pro1.SelectedValue;
                preDataPromo1.slm_CarTaxExpiredDate = txtCarTaxExpiredDateAct_pro1.DateValue == DateTime.MinValue ? null : (DateTime?)txtCarTaxExpiredDateAct_pro1.DateValue;
                preDataPromo1.slm_ActGrossStamp = lblActGrossStampAct_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossStampAct_pro1.Text);
                preDataPromo1.slm_ActGrossVat = lblActGrossVatAct_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossVatAct_pro1.Text);
                preDataPromo1.slm_ActGrossPremium = lblActGrossPremiumAct_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossPremiumAct_pro1.Text);
                preDataPromo1.slm_ActNetGrossPremium = lblActNetGrossPremiumAct_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro1.Text);
                preDataPromo1.slm_ActGrossPremiumPay = txtDiscountBathAct_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtDiscountBathAct_pro1.Text);
                preDataPromo1.slm_Vat1Percent = chkCardType2.Checked ? true : false;
                preDataPromo1.slm_Vat1PercentBath = lblActPersonType_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActPersonType_pro1.Text);
                preDataPromo1.slm_DiscountPercent = lblVat1PercentBathAct_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblVat1PercentBathAct_pro1.Text);
                preDataPromo1.slm_DiscountBath = lblDiscountPercentAct_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblDiscountPercentAct_pro1.Text);
                preDataPromo1.slm_ActNetGrossPremiumFull = lblActNetGrossPremiumFullAct_pro1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro1.Text);
                preDataPromo1.slm_ActPurchaseFlag = rbAct_pro1.Checked;
                preDataPromo1.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                preDataPromo1.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
            }


            if (hidPromotionActId2.Value != "")
            {
                PreleadCompareActData preDataPromo2;

                if (hidPromotionActId2.Value == "0")
                {
                    preDataPromo2 = data.ActPromoList.Where(cp => cp.slm_Seq == decimal.Parse(hidSeqAct2.Value)).FirstOrDefault();
                }
                else
                {
                    preDataPromo2 = data.ActPromoList.Where(cp => cp.slm_PromotionId == decimal.Parse(hidPromotionActId2.Value)).FirstOrDefault();
                }

                if (preDataPromo2 == null)
                {
                    preDataPromo2 = new PreleadCompareActData();
                }
                if (data.RenewIns != null)
                {
                    preDataPromo2.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                }
                preDataPromo2.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
                preDataPromo2.slm_PromotionId = AppUtil.SafeDecimal(hidPromotionActId2.Value);
                preDataPromo2.slm_Seq = i;
                i++;
                preDataPromo2.slm_Year = lblExpireDate.Text;
                preDataPromo2.slm_ActIssuePlace = AppUtil.SafeInt(cmbActIssuePlace_pro2.SelectedValue);
                preDataPromo2.slm_Ins_Com_Id = AppUtil.SafeInt(lblCompanyInsuranceAct_pro2.SelectedValue);
                preDataPromo2.slm_SendDocType = rbRegisterAct_pro2.Checked ? 1 : rbNormalAct_pro2.Checked ? 2 : 0;
                preDataPromo2.slm_ActNo = null;//
                preDataPromo2.slm_ActSignNo = txtSignNoAct_pro2.Text;
                preDataPromo2.slm_ActStartCoverDate = txtActStartCoverDateAct_pro2.DateValue == DateTime.MinValue ? null : (DateTime?)txtActStartCoverDateAct_pro2.DateValue;
                preDataPromo2.slm_ActEndCoverDate = txtActEndCoverDateAct_pro2.DateValue == DateTime.MinValue ? null : (DateTime?)txtActEndCoverDateAct_pro2.DateValue;
                preDataPromo2.slm_ActIssueBranch = cmbActIssueBranch_pro2.SelectedValue;
                preDataPromo2.slm_CarTaxExpiredDate = txtCarTaxExpiredDateAct_pro2.DateValue == DateTime.MinValue ? null : (DateTime?)txtCarTaxExpiredDateAct_pro2.DateValue;
                preDataPromo2.slm_ActGrossStamp = lblActGrossStampAct_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossStampAct_pro2.Text);
                preDataPromo2.slm_ActGrossVat = lblActGrossVatAct_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossVatAct_pro2.Text);
                preDataPromo2.slm_ActGrossPremium = lblActGrossPremiumAct_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossPremiumAct_pro2.Text);
                preDataPromo2.slm_ActNetGrossPremium = lblActNetGrossPremiumAct_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro2.Text);
                preDataPromo2.slm_ActGrossPremiumPay = txtDiscountBathAct_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtDiscountBathAct_pro2.Text);
                preDataPromo2.slm_Vat1Percent = chkCardType2.Checked ? true : false;
                preDataPromo2.slm_Vat1PercentBath = lblActPersonType_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActPersonType_pro2.Text);
                preDataPromo2.slm_DiscountPercent = lblVat1PercentBathAct_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblVat1PercentBathAct_pro2.Text);
                preDataPromo2.slm_DiscountBath = lblDiscountPercentAct_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblDiscountPercentAct_pro2.Text);
                preDataPromo2.slm_ActNetGrossPremiumFull = lblActNetGrossPremiumFullAct_pro2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro2.Text);
                preDataPromo2.slm_ActPurchaseFlag = rbAct_pro2.Checked;
                preDataPromo2.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                preDataPromo2.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
            }

            if (hidPromotionActId3.Value != "")
            {
                PreleadCompareActData preDataPromo3;

                if (hidPromotionActId3.Value == "0")
                {
                    preDataPromo3 = data.ActPromoList.Where(cp => cp.slm_Seq == decimal.Parse(hidSeqAct3.Value)).FirstOrDefault();
                }
                else
                {
                    preDataPromo3 = data.ActPromoList.Where(cp => cp.slm_PromotionId == decimal.Parse(hidPromotionActId3.Value)).FirstOrDefault();
                }

                if (preDataPromo3 == null)
                {
                    preDataPromo3 = new PreleadCompareActData();
                }
                if (data.RenewIns != null)
                {
                    preDataPromo3.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                }
                preDataPromo3.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
                preDataPromo3.slm_PromotionId = AppUtil.SafeDecimal(hidPromotionActId3.Value);
                preDataPromo3.slm_Seq = i;
                i++;
                preDataPromo3.slm_Year = lblExpireDate.Text;
                preDataPromo3.slm_Ins_Com_Id = AppUtil.SafeInt(lblCompanyInsuranceAct_pro3.SelectedValue);
                preDataPromo3.slm_ActIssuePlace = AppUtil.SafeInt(cmbActIssuePlace_pro3.SelectedValue);
                preDataPromo3.slm_SendDocType = rbRegisterAct_pro3.Checked ? 1 : rbNormalAct_pro3.Checked ? 2 : 0;
                preDataPromo3.slm_ActNo = null;//
                preDataPromo3.slm_ActSignNo = txtSignNoAct_pro3.Text;
                preDataPromo3.slm_ActStartCoverDate = txtActStartCoverDateAct_pro3.DateValue == DateTime.MinValue ? null : (DateTime?)txtActStartCoverDateAct_pro3.DateValue;
                preDataPromo3.slm_ActEndCoverDate = txtActEndCoverDateAct_pro3.DateValue == DateTime.MinValue ? null : (DateTime?)txtActEndCoverDateAct_pro3.DateValue;
                preDataPromo3.slm_ActIssueBranch = cmbActIssueBranch_pro3.SelectedValue;
                preDataPromo3.slm_CarTaxExpiredDate = txtCarTaxExpiredDateAct_pro3.DateValue == DateTime.MinValue ? null : (DateTime?)txtCarTaxExpiredDateAct_pro3.DateValue;
                preDataPromo3.slm_ActGrossStamp = lblActGrossStampAct_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossStampAct_pro3.Text);
                preDataPromo3.slm_ActGrossVat = lblActGrossVatAct_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossVatAct_pro3.Text);
                preDataPromo3.slm_ActGrossPremium = lblActGrossPremiumAct_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActGrossPremiumAct_pro3.Text);
                preDataPromo3.slm_ActNetGrossPremium = lblActNetGrossPremiumAct_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro3.Text);
                preDataPromo3.slm_ActGrossPremiumPay = txtDiscountBathAct_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtDiscountBathAct_pro3.Text);
                preDataPromo3.slm_Vat1Percent = chkCardType2.Checked ? true : false;
                preDataPromo3.slm_Vat1PercentBath = lblActPersonType_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActPersonType_pro3.Text);
                preDataPromo3.slm_DiscountPercent = lblVat1PercentBathAct_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblVat1PercentBathAct_pro3.Text);
                preDataPromo3.slm_DiscountBath = lblDiscountPercentAct_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblDiscountPercentAct_pro3.Text);
                preDataPromo3.slm_ActNetGrossPremiumFull = lblActNetGrossPremiumFullAct_pro3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro3.Text);
                preDataPromo3.slm_ActPurchaseFlag = rbAct_pro3.Checked;
                preDataPromo3.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                preDataPromo3.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
            }

            return data;
        }

        private PreleadCompareDataCollection PrepareForSaveAddress(PreleadCompareDataCollection data)
        {
            PreleadAddressData predataprev = new PreleadAddressData();

            if (PreLeadId != "")
            {
                data.Prelead.slm_Receiver = txtReceiver.Text;
            }
            else
            {
                data.RenewIns.slm_Receiver = txtReceiver.Text;
            }
            if (rdoAddressOld.Checked)
            {

                if (PreLeadId != "")
                {
                    data.Prelead.slm_SendDocFlag = "1";
                    data.Prelead.slm_SendDocBrandCode = null;
                }
                else if (TicketId != "")
                {
                    data.RenewIns.slm_SendDocFlag = "1";
                    data.RenewIns.slm_SendDocBrandCode = null;
                }
                predataprev.slm_CmtLot = data.Prelead.slm_CmtLot;
                predataprev.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
                predataprev.slm_Customer_Key = data.Prelead.slm_Customer_Key;
                predataprev.slm_Address_Type = "D";
                predataprev.slm_House_No = txtHouseNo.Text;
                predataprev.slm_Moo = txtMoo.Text;
                predataprev.slm_Building = txtBuilding.Text;
                predataprev.slm_Village = txtHouseName.Text;
                predataprev.slm_Soi = txtSoi.Text;
                predataprev.slm_Street = txtStreet.Text;
                predataprev.slm_TambolId = AppUtil.SafeInt(cmbThambol.SelectedValue);
                predataprev.slm_Amphur_Id = AppUtil.SafeInt(cmbDistinct.SelectedValue);
                predataprev.slm_Province_Id = AppUtil.SafeInt(cmbProvince.SelectedValue);
                predataprev.slm_Zipcode = txtZipCode.Text;

                data.Address = predataprev;
            }
            else if (rdoAddressChange.Checked)
            {
                if (PreLeadId != "")
                {
                    data.Prelead.slm_SendDocFlag = "2";
                    data.Prelead.slm_SendDocBrandCode = null;
                }
                else if (TicketId != "")
                {
                    data.RenewIns.slm_SendDocFlag = "2";
                    data.RenewIns.slm_SendDocBrandCode = null;
                }

                predataprev.slm_CmtLot = data.Prelead.slm_CmtLot;
                predataprev.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
                predataprev.slm_Customer_Key = data.Prelead.slm_Customer_Key;
                predataprev.slm_Address_Type = "D";
                predataprev.slm_House_No = txtHouseNo.Text;
                predataprev.slm_Moo = txtMoo.Text;
                predataprev.slm_Building = txtBuilding.Text;
                predataprev.slm_Village = txtHouseName.Text;
                predataprev.slm_Soi = txtSoi.Text;
                predataprev.slm_Street = txtStreet.Text;
                predataprev.slm_TambolId = AppUtil.SafeInt(cmbThambol.SelectedValue);
                predataprev.slm_Amphur_Id = AppUtil.SafeInt(cmbDistinct.SelectedValue);
                predataprev.slm_Province_Id = AppUtil.SafeInt(cmbProvince.SelectedValue);
                predataprev.slm_Zipcode = txtZipCode.Text;

                data.Address = predataprev;
            }
            else if (rdoAddressBranch.Checked)
            {
                if (PreLeadId != "")
                {
                    data.Prelead.slm_SendDocFlag = "3";
                    data.Prelead.slm_SendDocBrandCode = cmbBranchCodeDoc.SelectedValue;
                }
                else if (TicketId != "")
                {
                    data.RenewIns.slm_SendDocFlag = "3";
                    data.RenewIns.slm_SendDocBrandCode = cmbBranchCodeDoc.SelectedValue;
                }

                predataprev.slm_CmtLot = data.Prelead.slm_CmtLot;
                predataprev.slm_Prelead_Id = data.Prelead.slm_Prelead_Id;
                predataprev.slm_Customer_Key = data.Prelead.slm_Customer_Key;
                predataprev.slm_Address_Type = "D";
                predataprev.slm_House_No = txtHouseNo.Text;
                predataprev.slm_Moo = txtMoo.Text;
                predataprev.slm_Building = txtBuilding.Text;
                predataprev.slm_Village = txtHouseName.Text;
                predataprev.slm_Soi = txtSoi.Text;
                predataprev.slm_Street = txtStreet.Text;
                predataprev.slm_TambolId = AppUtil.SafeInt(cmbThambol.SelectedValue);
                predataprev.slm_Amphur_Id = AppUtil.SafeInt(cmbDistinct.SelectedValue);
                predataprev.slm_Province_Id = AppUtil.SafeInt(cmbProvince.SelectedValue);
                predataprev.slm_Zipcode = txtZipCode.Text;

                data.Address = predataprev;
            }
            return data;
        }

        private PreleadCompareDataCollection PrepareForSaveRenewInsurance(PreleadCompareDataCollection data)
        {

            data.RenewIns.slm_PayOptionId = AppUtil.SafeInt(cmbPaymentmethod.SelectedValue);
            data.RenewIns.slm_PolicyAmountPeriod = AppUtil.SafeInt(textPeriod.Text);
            data.RenewIns.slm_PolicyPayMethodId = AppUtil.SafeInt(cmbPaymentType.SelectedValue);
            data.RenewIns.slm_PayBranchCode = cmbPolicyPayBranchCode.SelectedValue;

            data.RenewIns.slm_ActPayOptionId = AppUtil.SafeInt(cmbPaymentmethodAct.SelectedValue);
            data.RenewIns.slm_ActPayMethodId = AppUtil.SafeInt(cmbPaymentTypeAct.SelectedValue);
            data.RenewIns.slm_ActPayBranchCode = cmbPayBranchCodeAct.SelectedValue;

            data.RenewIns.slm_RedbookBrandCode = cmbCarBrand.SelectedValue;
            data.RenewIns.slm_RedbookModelCode = cmbCarName.SelectedValue;
            data.RenewIns.slm_CC = CCCar.Text.Trim();

            if (cmbBeneficiary.SelectedItem.Value != "")
            {
                data.RenewIns.slm_BeneficiaryId = int.Parse(cmbBeneficiary.SelectedItem.Value);
                data.RenewIns.slm_BeneficiaryName = cmbBeneficiary.SelectedItem.Text.Trim();
            }
            else
            {
                data.RenewIns.slm_BeneficiaryId = null;
                data.RenewIns.slm_BeneficiaryName = null;
            }

            data.RenewIns.slm_RedbookYearGroup = cmbModelYear.SelectedItem.Value != "" ? (int?)int.Parse(cmbModelYear.SelectedItem.Value) : null;

            if (cmbInsuranceCarType.SelectedItem.Value != "")
            {
                data.RenewIns.slm_InsuranceCarTypeId = int.Parse(cmbInsuranceCarType.SelectedItem.Value);
            }
            data.lead.slm_ProvinceRegis = cmbProvinceRegis.SelectedItem.Value != "" ? (int?)int.Parse(cmbProvinceRegis.SelectedItem.Value) : null;

            data.RenewIns.slm_RemarkPolicy = txtInsDesc.Text;
            data.RenewIns.slm_RemarkPayment = txtPaymentDesc.Text;
            data.RenewIns.slm_RemarkAct = txtRemarkAct.Text;

            if (chkCardType.Checked && (rbInsNameTh_cur.Checked || rbInsNameTh_pro1.Checked || rbInsNameTh_pro2.Checked || rbInsNameTh_pro3.Checked))
            {
                data.RenewIns.slm_Need_50TawiFlag = "Y";
            }
            else if (chkCardType2.Checked && (rbAct_pro1.Checked || rbAct_pro2.Checked || rbAct_pro3.Checked))
            {
                data.RenewIns.slm_Need_50TawiFlag = "Y";
            }
            else
            {
                data.RenewIns.slm_Need_50TawiFlag = null;
            }

            data.RenewIns.slm_ActGrossPremium = AppUtil.SafeDecimal(txtActGrossPremium.Text);
            data.RenewIns.slm_PolicyGrossPremium = AppUtil.SafeDecimal(txtPolicyGrossPremium.Text);
            data.RenewIns.slm_PolicyDiscountAmt = AppUtil.SafeDecimal(txtPolicyDiscountAmt.Text);
            data.RenewIns.slm_DiscountPercent = AppUtil.SafeDecimal(txtDiscountPercent.Text);
            data.RenewIns.slm_ActDiscountAmt = AppUtil.SafeDecimal(txtActDiscountAmt.Text);
            data.RenewIns.slm_ActDiscountPercent = AppUtil.SafeDecimal(txtDiscountPercentAct.Text);

            return data;
        }

        private PreleadCompareDataCollection PrepareForSavePaymentMain(PreleadCompareDataCollection data)
        {
            List<RenewInsurancePaymentMainData> payMainList = new List<RenewInsurancePaymentMainData>();
            List<RenewInsurancePaymentMainData> payMainActList = new List<RenewInsurancePaymentMainData>();

            if (cmbPaymentmethod.SelectedValue != "")
            {
                RenewInsurancePaymentMainData payMain1 = new RenewInsurancePaymentMainData();
                if (txtPeriod1.Enabled == true)
                {
                    payMain1.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain1.slm_Type = "1";
                    payMain1.slm_Seq = 1;
                    payMain1.slm_PaymentDate = tdmPaymentDate1.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate1.DateValue;
                    payMain1.slm_Period = 1;
                    payMain1.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod1.Text);
                    payMainList.Add(payMain1);
                }

                RenewInsurancePaymentMainData payMain2 = new RenewInsurancePaymentMainData();
                if (txtPeriod2.Enabled == true)
                {
                    payMain2.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain2.slm_Type = "1";
                    payMain2.slm_Seq = 2;
                    payMain2.slm_PaymentDate = tdmPaymentDate2.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate2.DateValue;
                    payMain2.slm_Period = 2;
                    payMain2.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod2.Text);
                    payMainList.Add(payMain2);
                }

                RenewInsurancePaymentMainData payMain3 = new RenewInsurancePaymentMainData();
                if (txtPeriod3.Enabled == true)
                {
                    payMain3.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain3.slm_Type = "1";
                    payMain3.slm_Seq = 3;
                    payMain3.slm_PaymentDate = tdmPaymentDate3.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate3.DateValue;
                    payMain3.slm_Period = 3;
                    payMain3.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod3.Text);
                    payMainList.Add(payMain3);
                }

                RenewInsurancePaymentMainData payMain4 = new RenewInsurancePaymentMainData();
                if (txtPeriod4.Enabled == true)
                {
                    payMain4.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain4.slm_Type = "1";
                    payMain4.slm_Seq = 4;
                    payMain4.slm_PaymentDate = tdmPaymentDate4.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate4.DateValue;
                    payMain4.slm_Period = 4;
                    payMain4.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod4.Text);
                    payMainList.Add(payMain4);
                }

                RenewInsurancePaymentMainData payMain5 = new RenewInsurancePaymentMainData();
                if (txtPeriod5.Enabled == true)
                {
                    payMain5.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain5.slm_Type = "1";
                    payMain5.slm_Seq = 5;
                    payMain5.slm_PaymentDate = tdmPaymentDate5.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate5.DateValue;
                    payMain5.slm_Period = 5;
                    payMain5.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod5.Text);
                    payMainList.Add(payMain5);
                }

                RenewInsurancePaymentMainData payMain6 = new RenewInsurancePaymentMainData();
                if (txtPeriod6.Enabled == true)
                {
                    payMain6.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain6.slm_Type = "1";
                    payMain6.slm_Seq = 6;
                    payMain6.slm_PaymentDate = tdmPaymentDate6.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate6.DateValue;
                    payMain6.slm_Period = 6;
                    payMain6.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod6.Text);
                    payMainList.Add(payMain6);
                }

                RenewInsurancePaymentMainData payMain7 = new RenewInsurancePaymentMainData();
                if (txtPeriod7.Enabled == true)
                {
                    payMain7.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain7.slm_Type = "1";
                    payMain7.slm_Seq = 7;
                    payMain7.slm_PaymentDate = tdmPaymentDate7.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate7.DateValue;
                    payMain7.slm_Period = 7;
                    payMain7.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod7.Text);
                    payMainList.Add(payMain7);
                }

                RenewInsurancePaymentMainData payMain8 = new RenewInsurancePaymentMainData();
                if (txtPeriod8.Enabled == true)
                {
                    payMain8.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain8.slm_Type = "1";
                    payMain8.slm_Seq = 8;
                    payMain8.slm_PaymentDate = tdmPaymentDate8.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate8.DateValue;
                    payMain8.slm_Period = 8;
                    payMain8.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod8.Text);
                    payMainList.Add(payMain8);
                }

                RenewInsurancePaymentMainData payMain9 = new RenewInsurancePaymentMainData();
                if (txtPeriod9.Enabled == true)
                {
                    payMain9.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain9.slm_Type = "1";
                    payMain9.slm_Seq = 9;
                    payMain9.slm_PaymentDate = tdmPaymentDate9.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate9.DateValue;
                    payMain9.slm_Period = 9;
                    payMain9.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod9.Text);
                    payMainList.Add(payMain9);
                }

                RenewInsurancePaymentMainData payMain10 = new RenewInsurancePaymentMainData();
                if (txtPeriod10.Enabled == true)
                {
                    payMain10.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                    payMain10.slm_Type = "1";
                    payMain10.slm_Seq = 10;
                    payMain10.slm_PaymentDate = tdmPaymentDate10.DateValue == DateTime.MinValue ? null : (DateTime?)tdmPaymentDate10.DateValue;
                    payMain10.slm_Period = 10;
                    payMain10.slm_PaymentAmount = AppUtil.SafeDecimal(txtPeriod10.Text);
                    payMainList.Add(payMain10);
                }
            }
            if (cmbPaymentmethodAct.SelectedValue != "")
            {
                RenewInsurancePaymentMainData payActMain = new RenewInsurancePaymentMainData();

                payActMain.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                payActMain.slm_Seq = 1;
                payActMain.slm_Type = "2";
                payActMain.slm_PaymentDate = null;
                payActMain.slm_Period = 0;

                if (rbAct_pro1.Checked)
                {
                    payActMain.slm_PaymentAmount = AppUtil.SafeDecimal(txtDiscountBathAct_pro1.Text);
                }
                else if (rbAct_pro2.Checked)
                {
                    payActMain.slm_PaymentAmount = AppUtil.SafeDecimal(txtDiscountBathAct_pro2.Text);
                }
                else if (rbAct_pro3.Checked)
                {
                    payActMain.slm_PaymentAmount = AppUtil.SafeDecimal(txtDiscountBathAct_pro3.Text);
                }

                payActMain.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                payActMain.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;

                payMainActList.Add(payActMain);

            }
            data.PayMainList = payMainList;
            data.PayMainActList = payMainActList;
            return data;
        }

        private PreleadCompareDataCollection PrepareForSaveTransMain(PreleadCompareDataCollection data)
        {
            PaymentTransMainData tm = null;
            if (cmbPolicyPayMethod.SelectedValue != "")
            {
                if (tm == null)
                {
                    tm = new PaymentTransMainData();
                }
                if (data.RenewIns != null)
                {
                    tm.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                }
                tm.slm_PayOptionId = cmbPolicyPayMethod.SelectedValue == "" ? null : (int?)AppUtil.SafeInt(cmbPolicyPayMethod.SelectedValue);
                tm.slm_PolicyAmountPeriod = (int?)AppUtil.SafeInt(txtPolicyAmountPeriod.Text);
                tm.slm_PayBranchCode = cmbPayBranchCode.SelectedValue;
                tm.slm_PolicyPayMethodId = cmbPayOption.SelectedValue == "" ? null : (int?)AppUtil.SafeInt(cmbPayOption.SelectedValue);
            }

            if (cmbActPayOption.Text != "")
            {
                if (tm == null)
                {
                    tm = new PaymentTransMainData();
                }
                tm.slm_ActPayMethodId = cmbActPayOption.SelectedValue == "" ? null : (int?)AppUtil.SafeInt(cmbActPayOption.SelectedValue);
                tm.slm_ActamountPeriod = 0;
                tm.slm_ActPayOptionId = (int?)AppUtil.SafeInt(cmbActPayMethod.SelectedValue); ;
                tm.slm_ActPayBranchCode = cmbActPayBranchCode.SelectedValue;
            }

            if (cmbPolicyPayMethod.SelectedValue != "" && txtPolicyGrossPremium.Text != "" && txtActGrossPremium.Text != "")
            {
                if (tm != null)
                {

                    tm.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                    tm.slm_CreatedDate = DateTime.Now;
                    tm.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                    tm.slm_UpdatedDate = DateTime.Now;
                }
            }
            if (tm != null)
            {
                if (data.PaymentTransMainList == null)
                {
                    data.PaymentTransMainList = new List<PaymentTransMainData>();
                }

                data.PaymentTransMainList.Add(tm);
            }

            return data;
        }

        private PreleadCompareDataCollection PrepareForSaveTrans(PreleadCompareDataCollection data)
        {
            List<PaymentTransData> ptList = new List<PaymentTransData>();
            if (tdmPolicyPaymentDate1.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 1;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate1.DateValue;
                pt.slm_Period = 1;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount1.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            if (tdmPolicyPaymentDate2.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 2;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate2.DateValue;
                pt.slm_Period = 2;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount2.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            if (tdmPolicyPaymentDate3.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 3;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate1.DateValue;
                pt.slm_Period = 3;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount3.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            if (tdmPolicyPaymentDate4.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 4;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate4.DateValue;
                pt.slm_Period = 4;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount4.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount4.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            if (tdmPolicyPaymentDate5.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 5;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate5.DateValue;
                pt.slm_Period = 5;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount5.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount5.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            if (tdmPolicyPaymentDate6.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 6;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate6.DateValue;
                pt.slm_Period = 6;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount6.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount6.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            if (tdmPolicyPaymentDate7.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 7;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate7.DateValue;
                pt.slm_Period = 7;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount7.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount7.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            if (tdmPolicyPaymentDate8.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 8;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate8.DateValue;
                pt.slm_Period = 8;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount8.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount8.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            if (tdmPolicyPaymentDate9.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 9;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate9.DateValue;
                pt.slm_Period = 9;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount9.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount9.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            if (tdmPolicyPaymentDate10.DateValue != DateTime.MinValue)
            {
                PaymentTransData pt = new PaymentTransData();
                pt.slm_RenewInsureId = data.RenewIns.slm_RenewInsureId;
                pt.slm_Seq = 10;
                pt.slm_Type = "1";
                pt.slm_PaymentDate = tdmPolicyPaymentDate10.DateValue;
                pt.slm_Period = 10;
                pt.slm_PaymentAmount = txtPolicyPaymentAmount10.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPolicyPaymentAmount10.Text);
                pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_CreatedDate = DateTime.Now;
                pt.slm_UpdatedBy = pt.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                pt.slm_UpdatedDate = DateTime.Now;
                ptList.Add(pt);
            }

            data.PaymentTransList = ptList;

            return data;
        }

        private PreleadCompareDataCollection PrepareForSaveReceipt(PreleadCompareDataCollection data)
        {
            foreach (GridViewRow r in gvReceipt.Rows)
            {
                decimal? key = ((HiddenField)r.FindControl("hidRenewInsuranceReceiptId")).Value == "" ? null : (decimal?)AppUtil.SafeDecimal(((HiddenField)r.FindControl("hidRenewInsuranceReceiptId")).Value);
                RenewInsuranceReceiptData rc = data.ReceiptList.Where(l => l.slm_RenewInsuranceReceiptId == key).FirstOrDefault();

                if (rc != null)
                {
                    rc.slm_Status = ((DropDownList)r.FindControl("cmbStatus")).SelectedValue;
                }
            }

            return data;
        }

        private void SaveChangeForNotifyReport(PreleadCompareDataCollection data)
        {
            var DataChange = data;
        }

        private List<RenewInsuranceReceiptRevisionDetailData> PrepareForSaveReceiptRevision(List<RenewInsuranceReceiptRevisionDetailData> rrdList)
        {
            if (rrdList == null)
            {
                rrdList = new List<RenewInsuranceReceiptRevisionDetailData>();
            }
            else
            {
                List<RenewInsuranceReceiptRevisionDetailData> tmp = rrdList.Where(r => r.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value)).ToList();
                tmp = tmp.Select(c => { c.delflag = "D"; return c; }).ToList();
            }

            if (hidRenewInsuranceReceiptId.Value != "")
            {
                if (divEditReceipt.Visible == true)
                {
                    List<RenewInsuranceReceiptDetailData> rrList = getSelectTabData().ReceiptDetailList.Where(or => or.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value)).ToList();
                    RenewInsuranceReceiptDetailData rr;

                    if (chkPaymentDesc1.Checked)
                    {
                        RenewInsuranceReceiptRevisionDetailData rrd1 = new RenewInsuranceReceiptRevisionDetailData();
                        rrd1.slm_RenewInsuranceReceiptId = hidRenewInsuranceReceiptId.Value == "" ? null : (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value);
                        rrd1.slm_Seq = 1;
                        rrd1.slm_Selected = chkPaymentDesc1.Checked ? "Y" : "N";
                        rrd1.slm_RecAmount = txtPaymentDesc1.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPaymentDesc1.Text);
                        rrd1.delflag = "";
                        rr = rrList.Where(r => r.slm_PaymentCode == "204").FirstOrDefault();
                        rrd1.slm_PaymentCode = "204";
                        if (rr != null)
                        {
                            rrd1.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd1.slm_InstNo = rr.slm_InstNo;
                            rrd1.slm_RecBy = rr.slm_RecBy;
                            rrd1.slm_RecNo = rr.slm_RecNo;
                            rrd1.slm_TransDate = rr.slm_TransDate;
                            rrd1.slm_Status = rr.slm_Status;
                            rrd1.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd1.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }
                        else
                        {
                            rr = rrList.FirstOrDefault();
                            rrd1.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd1.slm_InstNo = rr.slm_InstNo;
                            rrd1.slm_RecBy = rr.slm_RecBy;
                            rrd1.slm_RecNo = rr.slm_RecNo;
                            rrd1.slm_TransDate = rr.slm_TransDate;
                            rrd1.slm_Status = rr.slm_Status;
                            rrd1.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd1.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }

                        var rdd = rrdList.Where(or => or.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value) && or.slm_Seq == 1).FirstOrDefault();
                        if (rdd != null)
                        {
                            rrdList.Remove(rdd);
                            rrdList.Add(rrd1);
                        }
                        else
                        {
                            rrdList.Add(rrd1);
                        }
                    }

                    if (chkPaymentDesc2.Checked)
                    {
                        RenewInsuranceReceiptRevisionDetailData rrd2 = new RenewInsuranceReceiptRevisionDetailData();
                        rrd2.slm_RenewInsuranceReceiptId = hidRenewInsuranceReceiptId.Value == "" ? null : (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value);
                        rrd2.slm_Seq = 2;
                        rrd2.slm_Selected = chkPaymentDesc2.Checked ? "Y" : "N";
                        rrd2.slm_RecAmount = txtPaymentDesc2.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPaymentDesc2.Text);
                        rrd2.delflag = "";
                        rr = rrList.Where(r => r.slm_PaymentCode == "205").FirstOrDefault();
                        rrd2.slm_PaymentCode = "205";
                        if (rr != null)
                        {
                            rrd2.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd2.slm_InstNo = rr.slm_InstNo;
                            rrd2.slm_RecBy = rr.slm_RecBy;
                            rrd2.slm_RecNo = rr.slm_RecNo;
                            rrd2.slm_TransDate = rr.slm_TransDate;
                            rrd2.slm_Status = rr.slm_Status;

                            rrd2.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd2.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }
                        else
                        {
                            rr = rrList.FirstOrDefault();
                            rrd2.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd2.slm_InstNo = rr.slm_InstNo;
                            rrd2.slm_RecBy = rr.slm_RecBy;
                            rrd2.slm_RecNo = rr.slm_RecNo;
                            rrd2.slm_TransDate = rr.slm_TransDate;
                            rrd2.slm_Status = rr.slm_Status;
                            rrd2.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd2.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }

                        var rdd = rrdList.Where(or => or.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value) && or.slm_Seq == 2).FirstOrDefault();
                        if (rdd != null)
                        {
                            rrdList.Remove(rdd);
                            rrdList.Add(rrd2);
                        }
                        else
                        {
                            rrdList.Add(rrd2);
                        }
                    }

                    if (chkPaymentDesc3.Checked)
                    {
                        RenewInsuranceReceiptRevisionDetailData rrd3 = new RenewInsuranceReceiptRevisionDetailData();
                        rrd3.slm_Seq = 3;
                        rrd3.slm_RenewInsuranceReceiptId = hidRenewInsuranceReceiptId.Value == "" ? null : (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value);
                        rrd3.slm_Selected = chkPaymentDesc3.Checked ? "Y" : "N";
                        rrd3.slm_RecAmount = txtPaymentDesc3.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPaymentDesc3.Text);
                        rrd3.delflag = "";
                        rr = rrList.Where(r => r.slm_PaymentCode == "0HP").FirstOrDefault();
                        rrd3.slm_PaymentCode = "0HP";
                        if (rr != null)
                        {
                            rrd3.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd3.slm_InstNo = rr.slm_InstNo;
                            rrd3.slm_RecBy = rr.slm_RecBy;
                            rrd3.slm_RecNo = rr.slm_RecNo;
                            rrd3.slm_TransDate = rr.slm_TransDate;
                            rrd3.slm_Status = rr.slm_Status;
                            rrd3.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd3.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }
                        else
                        {
                            rr = rrList.FirstOrDefault();

                            rrd3.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd3.slm_InstNo = rr.slm_InstNo;
                            rrd3.slm_RecBy = rr.slm_RecBy;
                            rrd3.slm_RecNo = rr.slm_RecNo;
                            rrd3.slm_TransDate = rr.slm_TransDate;
                            rrd3.slm_Status = rr.slm_Status;
                            rrd3.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd3.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }

                        var rdd = rrdList.Where(or => or.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value) && or.slm_Seq == 3).FirstOrDefault();
                        if (rdd != null)
                        {
                            rrdList.Remove(rdd);
                            rrdList.Add(rrd3);
                        }
                        else
                        {
                            rrdList.Add(rrd3);
                        }
                    }

                    if (chkPaymentDesc4.Checked)
                    {
                        RenewInsuranceReceiptRevisionDetailData rrd4 = new RenewInsuranceReceiptRevisionDetailData();
                        rrd4.slm_Seq = 4;
                        rrd4.slm_RenewInsuranceReceiptId = hidRenewInsuranceReceiptId.Value == "" ? null : (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value);
                        rrd4.slm_Selected = chkPaymentDesc4.Checked ? "Y" : "N";
                        rrd4.slm_RecAmount = txtPaymentDesc4.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPaymentDesc4.Text);
                        rrd4.delflag = "";
                        rr = rrList.Where(r => r.slm_PaymentCode == "101").FirstOrDefault();
                        rrd4.slm_PaymentCode = "101";
                        if (rr != null)
                        {

                            rrd4.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd4.slm_InstNo = rr.slm_InstNo;
                            rrd4.slm_RecBy = rr.slm_RecBy;
                            rrd4.slm_RecNo = rr.slm_RecNo;
                            rrd4.slm_TransDate = rr.slm_TransDate;
                            rrd4.slm_Status = rr.slm_Status;
                            rrd4.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd4.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }
                        else
                        {
                            rr = rrList.FirstOrDefault();

                            rrd4.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd4.slm_InstNo = rr.slm_InstNo;
                            rrd4.slm_RecBy = rr.slm_RecBy;
                            rrd4.slm_RecNo = rr.slm_RecNo;
                            rrd4.slm_TransDate = rr.slm_TransDate;
                            rrd4.slm_Status = rr.slm_Status;
                            rrd4.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd4.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }

                        var rdd = rrdList.Where(or => or.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value) && or.slm_Seq == 4).FirstOrDefault();
                        if (rdd != null)
                        {
                            rrdList.Remove(rdd);
                            rrdList.Add(rrd4);
                        }
                        else
                        {
                            rrdList.Add(rrd4);
                        }
                    }

                    if (chkPaymentDesc5.Checked)
                    {
                        RenewInsuranceReceiptRevisionDetailData rrd5 = new RenewInsuranceReceiptRevisionDetailData();
                        rrd5.slm_Seq = 5;
                        rrd5.slm_RenewInsuranceReceiptId = hidRenewInsuranceReceiptId.Value == "" ? null : (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value);
                        rrd5.slm_Selected = chkPaymentDesc5.Checked ? "Y" : "N";
                        rrd5.slm_RecAmount = txtPaymentDesc5.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPaymentDesc5.Text);
                        rrd5.delflag = "";
                        rr = rrList.Where(r => r.slm_PaymentCode == "614").FirstOrDefault();
                        rrd5.slm_PaymentCode = "614";
                        if (rr != null)
                        {

                            rrd5.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd5.slm_InstNo = rr.slm_InstNo;
                            rrd5.slm_RecBy = rr.slm_RecBy;
                            rrd5.slm_RecNo = rr.slm_RecNo;
                            rrd5.slm_TransDate = rr.slm_TransDate;
                            rrd5.slm_Status = rr.slm_Status;
                            rrd5.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd5.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }
                        else
                        {
                            rr = rrList.FirstOrDefault();

                            rrd5.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd5.slm_InstNo = rr.slm_InstNo;
                            rrd5.slm_RecBy = rr.slm_RecBy;
                            rrd5.slm_RecNo = rr.slm_RecNo;
                            rrd5.slm_TransDate = rr.slm_TransDate;
                            rrd5.slm_Status = rr.slm_Status;
                            rrd5.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd5.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }

                        var rdd = rrdList.Where(or => or.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value) && or.slm_Seq == 5).FirstOrDefault();
                        if (rdd != null)
                        {
                            rrdList.Remove(rdd);
                            rrdList.Add(rrd5);
                        }
                        else
                        {
                            rrdList.Add(rrd5);
                        }
                    }

                    if (chkPaymentDesc6.Checked)
                    {
                        RenewInsuranceReceiptRevisionDetailData rrd6 = new RenewInsuranceReceiptRevisionDetailData();
                        rrd6.slm_Seq = 6;
                        rrd6.slm_RenewInsuranceReceiptId = hidRenewInsuranceReceiptId.Value == "" ? null : (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value);
                        rrd6.slm_Selected = chkPaymentDesc6.Checked ? "Y" : "N";
                        rrd6.slm_RecAmount = txtPaymentDesc6.Text == "" ? null : (decimal?)AppUtil.SafeDecimal(txtPaymentDesc6.Text);
                        rrd6.slm_PaymentOtherDesc = txtPaymentOther.Text;
                        rrd6.delflag = "";
                        rr = rrList.Where(r => r.slm_PaymentCode == "OTHER").FirstOrDefault();
                        rrd6.slm_PaymentCode = "OTHER";
                        if (rr != null)
                        {
                            //Nang Comment 14/8/2016
                            //rrd6.slm_PaymentCode = rr.slm_PaymentCode;
                            rrd6.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd6.slm_InstNo = rr.slm_InstNo;
                            rrd6.slm_RecBy = rr.slm_RecBy;
                            rrd6.slm_RecNo = rr.slm_RecNo;
                            rrd6.slm_TransDate = rr.slm_TransDate;
                            rrd6.slm_Status = rr.slm_Status;

                            rrd6.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd6.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }
                        else
                        {
                            rr = rrList.FirstOrDefault();

                            rrd6.slm_InsNoDesc = rr.slm_InsNoDesc;
                            rrd6.slm_InstNo = rr.slm_InstNo;
                            rrd6.slm_RecBy = rr.slm_RecBy;
                            rrd6.slm_RecNo = rr.slm_RecNo;
                            rrd6.slm_TransDate = rr.slm_TransDate;
                            rrd6.slm_Status = rr.slm_Status;
                            rrd6.slm_CreatedBy = HttpContext.Current.User.Identity.Name;
                            rrd6.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                        }

                        var rdd = rrdList.Where(or => or.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value) && or.slm_Seq == 6).FirstOrDefault();
                        if (rdd != null)
                        {
                            rrdList.Remove(rdd);
                            rrdList.Add(rrd6);
                        }
                        else
                        {
                            rrdList.Add(rrd6);
                        }
                    }
                }
            }

            return rrdList;
        }

        private PreleadCompareDataCollection PrepareProblemData(PreleadCompareDataCollection data)
        {
            foreach (GridViewRow r in gvProblem.Rows)
            {
                var id = ((Label)r.FindControl("slm_ProblemDetailId")).Text;
                var value = ((DropDownList)r.FindControl("cmbFixTypeFlag")).SelectedValue;

                ProblemDetailData problem = data.ProblemList.Where(p => p.slm_ProblemDetailId == AppUtil.SafeDecimal(id)).FirstOrDefault();
                problem.slm_FixTypeFlag = value;

                problem.slm_ResponseDetail = ((TextBox)r.FindControl("txtResponseDetail")).Text;
                problem.slm_Remark = ((TextBox)r.FindControl("txtRemark")).Text;

                if (problem.slm_FixTypeFlag == "2")
                {
                    problem.slm_PhoneContact = data.lead.slm_TelNo_1;
                    problem.slm_ResponseDate = DateTime.Now;
                    problem.slm_UpdatedDate = DateTime.Now;
                    problem.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    problem.slm_PhoneContact = null;
                    problem.slm_ResponseDate = null;
                }
            }

            return data;
        }

        protected void cmbPaymentmethod_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                setPaymentControl(true, 0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbPaymentType_TextChanged(object sender, EventArgs e)
        {
            setBranchControl();
        }

        private void setPaymentControlForCopy()
        {
            if (cmbPaymentmethod.SelectedValue == "2")
            {
                textPeriod.Enabled = true;
                cmbPaymentType.Enabled = true;
                cmbPayBranchCode.Enabled = false;
                cmbPayBranchCode.SelectedValue = "";

                if (textPeriod.Text == "")
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "1")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    trPday1.Visible = true;
                    for (int i = 2; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "2")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    trPday1.Visible = true;
                    trPday2.Visible = true;

                    for (int i = 3; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "3")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;

                    for (int i = 4; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "4")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;

                    for (int i = 5; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "5")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;

                    for (int i = 6; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "6")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;

                    for (int i = 7; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "7")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;

                    txtPeriod7.Enabled = true;
                    tdmPaymentDate7.Enabled = true;

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;
                    trPday7.Visible = true;

                    for (int i = 8; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "8")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;

                    txtPeriod7.Enabled = true;
                    tdmPaymentDate7.Enabled = true;

                    txtPeriod8.Enabled = true;
                    tdmPaymentDate8.Enabled = true;

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;
                    trPday7.Visible = true;
                    trPday8.Visible = true;

                    for (int i = 9; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "9")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;

                    txtPeriod7.Enabled = true;
                    tdmPaymentDate7.Enabled = true;

                    txtPeriod8.Enabled = true;
                    tdmPaymentDate8.Enabled = true;

                    txtPeriod9.Enabled = true;
                    tdmPaymentDate9.Enabled = true;

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;
                    trPday7.Visible = true;
                    trPday8.Visible = true;
                    trPday9.Visible = true;

                    for (int i = 10; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "10")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;

                    txtPeriod7.Enabled = true;
                    tdmPaymentDate7.Enabled = true;

                    txtPeriod8.Enabled = true;
                    tdmPaymentDate8.Enabled = true;

                    txtPeriod9.Enabled = true;
                    tdmPaymentDate9.Enabled = true;
                    tdmPaymentDate9.DateValue = DateTime.Now.AddMonths(9);

                    txtPeriod10.Enabled = true;
                    tdmPaymentDate10.Enabled = true;

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;
                    trPday7.Visible = true;
                    trPday8.Visible = true;
                    trPday9.Visible = true;
                    trPday10.Visible = true;
                }
            }
            else if (cmbPaymentmethod.SelectedValue != "0" && cmbPaymentmethod.SelectedValue != "")
            {
                textPeriod.Enabled = false;
                textPeriod.Text = "";
                cmbPaymentType.Enabled = true;
                cmbPayBranchCode.Enabled = false;
                cmbPayBranchCode.SelectedValue = "";

                for (int i = 1; i <= 10; i++)
                {
                    ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                    ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                }
            }
            else
            {
                textPeriod.Enabled = false;
                textPeriod.Text = "";
                cmbPaymentType.Enabled = true;
                cmbPayBranchCode.Enabled = false;
                cmbPayBranchCode.SelectedValue = "";

                for (int i = 1; i <= 10; i++)
                {
                    ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                    ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                }
            }

            if (cmbPaymentType.SelectedValue == "4")
            {
                cmbPolicyPayBranchCode.Enabled = true;
            }
            else
            {
                cmbPolicyPayBranchCode.Enabled = false;
                cmbPolicyPayBranchCode.SelectedValue = "";
            }

            if (cmbPaymentTypeAct.SelectedValue == "4")
            {
                cmbPayBranchCodeAct.Enabled = true;
            }
            else
            {
                cmbPayBranchCodeAct.Enabled = false;
                cmbPayBranchCodeAct.SelectedValue = "";
            }
        }

        private void setPaymentControl(bool doCalculate, decimal renewId)
        {
            if (cmbPaymentmethod.SelectedValue == "2")
            {
                var paymentList = new List<PaymentData>();
                if (doCalculate == false)
                {
                    paymentList = new SlmScr004Biz().GetPaymentMainList(renewId);
                }

                textPeriod.Enabled = true;
                cmbPaymentType.Enabled = true;
                cmbPayBranchCode.Enabled = false;
                cmbPayBranchCode.SelectedValue = "";

                decimal PayAmt = 0m;


                if (rbInsNameTh_cur.Checked)
                {
                    PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_cur.Text);
                }
                else if (rbInsNameTh_pro1.Checked)
                {
                    PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro1.Text);
                }
                else if (rbInsNameTh_pro2.Checked)
                {
                    PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro2.Text);
                }
                else if (rbInsNameTh_pro3.Checked)
                {
                    PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro3.Text);
                }

                if (textPeriod.Text == "")
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "1")
                {
                    txtPeriod1.Enabled = true;

                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    if (doCalculate)
                    {
                        txtPeriod1.Text = PayAmt.ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                    }

                    trPday1.Visible = true;
                    for (int i = 2; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }


                }
                else if (textPeriod.Text == "2")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    trPday1.Visible = true;
                    trPday2.Visible = true;

                    if (doCalculate)
                    {
                        txtPeriod1.Text = (PayAmt / 2m).ToString("#,##0.00");
                        txtPeriod2.Text = (PayAmt - (PayAmt / 2m)).ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                        if (paymentList[1] != null)
                        {
                            txtPeriod2.Text = GetPayAmountStr(paymentList[1].slm_PaymentAmount);
                            tdmPaymentDate2.DateValue = GetDate(paymentList[1].slm_PaymentDate);
                        } 
                    }

                    for (int i = 3; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "3")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;

                    if (doCalculate)
                    {
                        txtPeriod1.Text = (PayAmt / 3m).ToString("#,##0.00");
                        txtPeriod2.Text = (PayAmt / 3m).ToString("#,##0.00");
                        txtPeriod3.Text = (PayAmt - Math.Round((PayAmt / 3m), 2, MidpointRounding.AwayFromZero) * 2m).ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                        if (paymentList[1] != null)
                        {
                            txtPeriod2.Text = GetPayAmountStr(paymentList[1].slm_PaymentAmount);
                            tdmPaymentDate2.DateValue = GetDate(paymentList[1].slm_PaymentDate);
                        }
                        if (paymentList[2] != null)
                        {
                            txtPeriod3.Text = GetPayAmountStr(paymentList[2].slm_PaymentAmount);
                            tdmPaymentDate3.DateValue = GetDate(paymentList[2].slm_PaymentDate);
                        }
                            
                    }

                    for (int i = 4; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "4")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;
                    tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;

                    if (doCalculate)
                    {
                        txtPeriod1.Text = (PayAmt / 4m).ToString("#,##0.00");
                        txtPeriod2.Text = (PayAmt / 4m).ToString("#,##0.00");
                        txtPeriod3.Text = (PayAmt / 4m).ToString("#,##0.00");
                        txtPeriod4.Text = (PayAmt - (PayAmt / 4m * 3m)).ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                        if (paymentList[1] != null)
                        {
                            txtPeriod2.Text = GetPayAmountStr(paymentList[1].slm_PaymentAmount);
                            tdmPaymentDate2.DateValue = GetDate(paymentList[1].slm_PaymentDate);
                        }
                        if (paymentList[2] != null)
                        {
                            txtPeriod3.Text = GetPayAmountStr(paymentList[2].slm_PaymentAmount);
                            tdmPaymentDate3.DateValue = GetDate(paymentList[2].slm_PaymentDate);
                        }
                        if (paymentList[3] != null)
                        {
                            txtPeriod4.Text = GetPayAmountStr(paymentList[3].slm_PaymentAmount);
                            tdmPaymentDate4.DateValue = GetDate(paymentList[3].slm_PaymentDate);
                        }
                            
                    }

                    for (int i = 5; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "5")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;
                    tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;
                    tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;

                    if (doCalculate)
                    {
                        txtPeriod1.Text = (PayAmt / 5m).ToString("#,##0.00");
                        txtPeriod2.Text = (PayAmt / 5m).ToString("#,##0.00");
                        txtPeriod3.Text = (PayAmt / 5m).ToString("#,##0.00");
                        txtPeriod4.Text = (PayAmt / 5m).ToString("#,##0.00");
                        txtPeriod5.Text = (PayAmt - (PayAmt / 5m * 4m)).ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                        if (paymentList[1] != null)
                        {
                            txtPeriod2.Text = GetPayAmountStr(paymentList[1].slm_PaymentAmount);
                            tdmPaymentDate2.DateValue = GetDate(paymentList[1].slm_PaymentDate);
                        }                    
                        if (paymentList[2] != null)
                        {
                            txtPeriod3.Text = GetPayAmountStr(paymentList[2].slm_PaymentAmount);
                            tdmPaymentDate3.DateValue = GetDate(paymentList[2].slm_PaymentDate);
                        }  
                        if (paymentList[3] != null)
                        {
                            txtPeriod4.Text = GetPayAmountStr(paymentList[3].slm_PaymentAmount);
                            tdmPaymentDate4.DateValue = GetDate(paymentList[3].slm_PaymentDate);
                        }  
                        if (paymentList[4] != null)
                        {
                            txtPeriod5.Text = GetPayAmountStr(paymentList[4].slm_PaymentAmount);
                            tdmPaymentDate5.DateValue = GetDate(paymentList[4].slm_PaymentDate);
                        }
                    }

                    for (int i = 6; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "6")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;
                    tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;
                    tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;
                    tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;

                    if (doCalculate)
                    {
                        txtPeriod1.Text = (PayAmt / 6m).ToString("#,##0.00");
                        txtPeriod2.Text = (PayAmt / 6m).ToString("#,##0.00");
                        txtPeriod3.Text = (PayAmt / 6m).ToString("#,##0.00");
                        txtPeriod4.Text = (PayAmt / 6m).ToString("#,##0.00");
                        txtPeriod5.Text = (PayAmt / 6m).ToString("#,##0.00");
                        txtPeriod6.Text = (PayAmt - (PayAmt / 6m * 5m)).ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                        if (paymentList[1] != null)
                        {
                            txtPeriod2.Text = GetPayAmountStr(paymentList[1].slm_PaymentAmount);
                            tdmPaymentDate2.DateValue = GetDate(paymentList[1].slm_PaymentDate);
                        }
                        if (paymentList[2] != null)
                        {
                            txtPeriod3.Text = GetPayAmountStr(paymentList[2].slm_PaymentAmount);
                            tdmPaymentDate3.DateValue = GetDate(paymentList[2].slm_PaymentDate);
                        } 
                        if (paymentList[3] != null)
                        {
                            txtPeriod4.Text = GetPayAmountStr(paymentList[3].slm_PaymentAmount);
                            tdmPaymentDate4.DateValue = GetDate(paymentList[3].slm_PaymentDate);
                        }
                        if (paymentList[4] != null)
                        {
                            txtPeriod5.Text = GetPayAmountStr(paymentList[4].slm_PaymentAmount);
                            tdmPaymentDate5.DateValue = GetDate(paymentList[4].slm_PaymentDate);
                        }
                        if (paymentList[5] != null)
                        {
                            txtPeriod6.Text = GetPayAmountStr(paymentList[5].slm_PaymentAmount);
                            tdmPaymentDate6.DateValue = GetDate(paymentList[5].slm_PaymentDate);
                        }
                            
                    }

                    for (int i = 7; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "7")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;
                    tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;
                    tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;
                    tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                    txtPeriod7.Enabled = true;
                    tdmPaymentDate7.Enabled = true;
                    tdmPaymentDate7.DateValue = DateTime.Now.AddMonths(7);

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;
                    trPday7.Visible = true;

                    if (doCalculate)
                    {
                        txtPeriod1.Text = (PayAmt / 7m).ToString("#,##0.00");
                        txtPeriod2.Text = (PayAmt / 7m).ToString("#,##0.00");
                        txtPeriod3.Text = (PayAmt / 7m).ToString("#,##0.00");
                        txtPeriod4.Text = (PayAmt / 7m).ToString("#,##0.00");
                        txtPeriod5.Text = (PayAmt / 7m).ToString("#,##0.00");
                        txtPeriod6.Text = (PayAmt / 7m).ToString("#,##0.00");
                        txtPeriod7.Text = (PayAmt - (PayAmt / 7m * 6m)).ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                        if (paymentList[1] != null)
                        {
                            txtPeriod2.Text = GetPayAmountStr(paymentList[1].slm_PaymentAmount);
                            tdmPaymentDate2.DateValue = GetDate(paymentList[1].slm_PaymentDate);
                        }
                        if (paymentList[2] != null)
                        {
                            txtPeriod3.Text = GetPayAmountStr(paymentList[2].slm_PaymentAmount);
                            tdmPaymentDate3.DateValue = GetDate(paymentList[2].slm_PaymentDate);
                        }
                        if (paymentList[3] != null)
                        {
                            txtPeriod4.Text = GetPayAmountStr(paymentList[3].slm_PaymentAmount);
                            tdmPaymentDate4.DateValue = GetDate(paymentList[3].slm_PaymentDate);
                        }
                        if (paymentList[4] != null)
                        {
                            txtPeriod5.Text = GetPayAmountStr(paymentList[4].slm_PaymentAmount);
                            tdmPaymentDate5.DateValue = GetDate(paymentList[4].slm_PaymentDate);
                        }
                        if (paymentList[5] != null)
                        {
                            txtPeriod6.Text = GetPayAmountStr(paymentList[5].slm_PaymentAmount);
                            tdmPaymentDate6.DateValue = GetDate(paymentList[5].slm_PaymentDate);
                        }
                        if (paymentList[6] != null)
                        {
                            txtPeriod7.Text = GetPayAmountStr(paymentList[6].slm_PaymentAmount);
                            tdmPaymentDate7.DateValue = GetDate(paymentList[6].slm_PaymentDate);
                        }
                    }

                    for (int i = 8; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "8")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;
                    tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;
                    tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;
                    tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                    txtPeriod7.Enabled = true;
                    tdmPaymentDate7.Enabled = true;
                    tdmPaymentDate7.DateValue = DateTime.Now.AddMonths(7);

                    txtPeriod8.Enabled = true;
                    tdmPaymentDate8.Enabled = true;
                    tdmPaymentDate8.DateValue = DateTime.Now.AddMonths(8);

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;
                    trPday7.Visible = true;
                    trPday8.Visible = true;

                    if (doCalculate)
                    {
                        txtPeriod1.Text = (PayAmt / 8m).ToString("#,##0.00");
                        txtPeriod2.Text = (PayAmt / 8m).ToString("#,##0.00");
                        txtPeriod3.Text = (PayAmt / 8m).ToString("#,##0.00");
                        txtPeriod4.Text = (PayAmt / 8m).ToString("#,##0.00");
                        txtPeriod5.Text = (PayAmt / 8m).ToString("#,##0.00");
                        txtPeriod6.Text = (PayAmt / 8m).ToString("#,##0.00");
                        txtPeriod7.Text = (PayAmt / 8m).ToString("#,##0.00");
                        txtPeriod8.Text = (PayAmt - (PayAmt / 8m * 7m)).ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                        if (paymentList[1] != null)
                        {
                            txtPeriod2.Text = GetPayAmountStr(paymentList[1].slm_PaymentAmount);
                            tdmPaymentDate2.DateValue = GetDate(paymentList[1].slm_PaymentDate);
                        }
                        if (paymentList[2] != null)
                        {
                            txtPeriod3.Text = GetPayAmountStr(paymentList[2].slm_PaymentAmount);
                            tdmPaymentDate3.DateValue = GetDate(paymentList[2].slm_PaymentDate);
                        }
                        if (paymentList[3] != null)
                        {
                            txtPeriod4.Text = GetPayAmountStr(paymentList[3].slm_PaymentAmount);
                            tdmPaymentDate4.DateValue = GetDate(paymentList[3].slm_PaymentDate);
                        }
                        if (paymentList[4] != null)
                        {
                            txtPeriod5.Text = GetPayAmountStr(paymentList[4].slm_PaymentAmount);
                            tdmPaymentDate5.DateValue = GetDate(paymentList[4].slm_PaymentDate);
                        }
                        if (paymentList[5] != null)
                        {
                            txtPeriod6.Text = GetPayAmountStr(paymentList[5].slm_PaymentAmount);
                            tdmPaymentDate6.DateValue = GetDate(paymentList[5].slm_PaymentDate);
                        }
                        if (paymentList[6] != null)
                        {
                            txtPeriod7.Text = GetPayAmountStr(paymentList[6].slm_PaymentAmount);
                            tdmPaymentDate7.DateValue = GetDate(paymentList[6].slm_PaymentDate);
                        }
                        if (paymentList[7] != null)
                        {
                            txtPeriod8.Text = GetPayAmountStr(paymentList[7].slm_PaymentAmount);
                            tdmPaymentDate8.DateValue = GetDate(paymentList[7].slm_PaymentDate);
                        }
                    }

                    for (int i = 9; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (textPeriod.Text == "9")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;
                    tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;
                    tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;
                    tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                    txtPeriod7.Enabled = true;
                    tdmPaymentDate7.Enabled = true;
                    tdmPaymentDate7.DateValue = DateTime.Now.AddMonths(7);

                    txtPeriod8.Enabled = true;
                    tdmPaymentDate8.Enabled = true;
                    tdmPaymentDate8.DateValue = DateTime.Now.AddMonths(8);

                    txtPeriod9.Enabled = true;
                    tdmPaymentDate9.Enabled = true;
                    tdmPaymentDate9.DateValue = DateTime.Now.AddMonths(9);

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;
                    trPday7.Visible = true;
                    trPday8.Visible = true;
                    trPday9.Visible = true;

                    if (doCalculate)
                    {
                        txtPeriod1.Text = (PayAmt / 9m).ToString("#,##0.00");
                        txtPeriod2.Text = (PayAmt / 9m).ToString("#,##0.00");
                        txtPeriod3.Text = (PayAmt / 9m).ToString("#,##0.00");
                        txtPeriod4.Text = (PayAmt / 9m).ToString("#,##0.00");
                        txtPeriod5.Text = (PayAmt / 9m).ToString("#,##0.00");
                        txtPeriod6.Text = (PayAmt / 9m).ToString("#,##0.00");
                        txtPeriod7.Text = (PayAmt / 9m).ToString("#,##0.00");
                        txtPeriod8.Text = (PayAmt / 9m).ToString("#,##0.00");
                        txtPeriod9.Text = (PayAmt - (PayAmt / 9m * 8m)).ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                        if (paymentList[1] != null)
                        {
                            txtPeriod2.Text = GetPayAmountStr(paymentList[1].slm_PaymentAmount);
                            tdmPaymentDate2.DateValue = GetDate(paymentList[1].slm_PaymentDate);
                        }
                        if (paymentList[2] != null)
                        {
                            txtPeriod3.Text = GetPayAmountStr(paymentList[2].slm_PaymentAmount);
                            tdmPaymentDate3.DateValue = GetDate(paymentList[2].slm_PaymentDate);
                        }
                        if (paymentList[3] != null)
                        {
                            txtPeriod4.Text = GetPayAmountStr(paymentList[3].slm_PaymentAmount);
                            tdmPaymentDate4.DateValue = GetDate(paymentList[3].slm_PaymentDate);
                        }
                        if (paymentList[4] != null)
                        {
                            txtPeriod5.Text = GetPayAmountStr(paymentList[4].slm_PaymentAmount);
                            tdmPaymentDate5.DateValue = GetDate(paymentList[4].slm_PaymentDate);
                        }
                        if (paymentList[5] != null)
                        {
                            txtPeriod6.Text = GetPayAmountStr(paymentList[5].slm_PaymentAmount);
                            tdmPaymentDate6.DateValue = GetDate(paymentList[5].slm_PaymentDate);
                        }
                        if (paymentList[6] != null)
                        {
                            txtPeriod7.Text = GetPayAmountStr(paymentList[6].slm_PaymentAmount);
                            tdmPaymentDate7.DateValue = GetDate(paymentList[6].slm_PaymentDate);
                        }
                        if (paymentList[7] != null)
                        {
                            txtPeriod8.Text = GetPayAmountStr(paymentList[7].slm_PaymentAmount);
                            tdmPaymentDate8.DateValue = GetDate(paymentList[7].slm_PaymentDate);
                        } 
                        if (paymentList[8] != null)
                        {
                            txtPeriod9.Text = GetPayAmountStr(paymentList[8].slm_PaymentAmount);
                            tdmPaymentDate9.DateValue = GetDate(paymentList[8].slm_PaymentDate);
                        }
                    }

                    for (int i = 10; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }


                }
                else if (textPeriod.Text == "10")
                {
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    txtPeriod3.Enabled = true;
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    txtPeriod4.Enabled = true;
                    tdmPaymentDate4.Enabled = true;
                    tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                    txtPeriod5.Enabled = true;
                    tdmPaymentDate5.Enabled = true;
                    tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                    txtPeriod6.Enabled = true;
                    tdmPaymentDate6.Enabled = true;
                    tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                    txtPeriod7.Enabled = true;
                    tdmPaymentDate7.Enabled = true;
                    tdmPaymentDate7.DateValue = DateTime.Now.AddMonths(7);

                    txtPeriod8.Enabled = true;
                    tdmPaymentDate8.Enabled = true;
                    tdmPaymentDate8.DateValue = DateTime.Now.AddMonths(8);

                    txtPeriod9.Enabled = true;
                    tdmPaymentDate9.Enabled = true;
                    tdmPaymentDate9.DateValue = DateTime.Now.AddMonths(9);

                    txtPeriod10.Enabled = true;
                    tdmPaymentDate10.Enabled = true;
                    tdmPaymentDate10.DateValue = DateTime.Now.AddMonths(10);

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;
                    trPday7.Visible = true;
                    trPday8.Visible = true;
                    trPday9.Visible = true;
                    trPday10.Visible = true;

                    if (doCalculate)
                    {
                        txtPeriod1.Text = (PayAmt / 10m).ToString("#,##0.00");
                        txtPeriod2.Text = (PayAmt / 10m).ToString("#,##0.00");
                        txtPeriod3.Text = (PayAmt / 10m).ToString("#,##0.00");
                        txtPeriod4.Text = (PayAmt / 10m).ToString("#,##0.00");
                        txtPeriod5.Text = (PayAmt / 10m).ToString("#,##0.00");
                        txtPeriod6.Text = (PayAmt / 10m).ToString("#,##0.00");
                        txtPeriod7.Text = (PayAmt / 10m).ToString("#,##0.00");
                        txtPeriod8.Text = (PayAmt / 10m).ToString("#,##0.00");
                    }
                    else
                    {
                        if (paymentList[0] != null)
                        {
                            txtPeriod1.Text = GetPayAmountStr(paymentList[0].slm_PaymentAmount);
                            tdmPaymentDate1.DateValue = GetDate(paymentList[0].slm_PaymentDate);
                        }
                        if (paymentList[1] != null)
                        {
                            txtPeriod2.Text = GetPayAmountStr(paymentList[1].slm_PaymentAmount);
                            tdmPaymentDate2.DateValue = GetDate(paymentList[1].slm_PaymentDate);
                        }
                        if (paymentList[2] != null)
                        {
                            txtPeriod3.Text = GetPayAmountStr(paymentList[2].slm_PaymentAmount);
                            tdmPaymentDate3.DateValue = GetDate(paymentList[2].slm_PaymentDate);
                        }
                        if (paymentList[3] != null)
                        {
                            txtPeriod4.Text = GetPayAmountStr(paymentList[3].slm_PaymentAmount);
                            tdmPaymentDate4.DateValue = GetDate(paymentList[3].slm_PaymentDate);
                        }
                        if (paymentList[4] != null)
                        {
                            txtPeriod5.Text = GetPayAmountStr(paymentList[4].slm_PaymentAmount);
                            tdmPaymentDate5.DateValue = GetDate(paymentList[4].slm_PaymentDate);
                        }
                        if (paymentList[5] != null)
                        {
                            txtPeriod6.Text = GetPayAmountStr(paymentList[5].slm_PaymentAmount);
                            tdmPaymentDate6.DateValue = GetDate(paymentList[5].slm_PaymentDate);
                        }
                        if (paymentList[6] != null)
                        {
                            txtPeriod7.Text = GetPayAmountStr(paymentList[6].slm_PaymentAmount);
                            tdmPaymentDate7.DateValue = GetDate(paymentList[6].slm_PaymentDate);
                        } 
                        if (paymentList[7] != null)
                        {
                            txtPeriod8.Text = GetPayAmountStr(paymentList[7].slm_PaymentAmount);
                            tdmPaymentDate8.DateValue = GetDate(paymentList[7].slm_PaymentDate);
                        }
                        if (paymentList[8] != null)
                        {
                            txtPeriod9.Text = GetPayAmountStr(paymentList[8].slm_PaymentAmount);
                            tdmPaymentDate9.DateValue = GetDate(paymentList[8].slm_PaymentDate);
                        }  
                        if (paymentList[9] != null)
                        {
                            txtPeriod10.Text = GetPayAmountStr(paymentList[9].slm_PaymentAmount);
                            tdmPaymentDate10.DateValue = GetDate(paymentList[9].slm_PaymentDate);
                        } 
                    }
                }

            }
            else if (cmbPaymentmethod.SelectedValue != "0" && cmbPaymentmethod.SelectedValue != "")
            {
                textPeriod.Enabled = false;
                textPeriod.Text = "";
                cmbPaymentType.Enabled = true;
                cmbPayBranchCode.Enabled = false;
                cmbPayBranchCode.SelectedValue = "";

                for (int i = 1; i <= 10; i++)
                {
                    ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                    ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                }
            }
            else
            {
                textPeriod.Enabled = false;
                textPeriod.Text = "";
                cmbPaymentType.Enabled = true;
                cmbPayBranchCode.Enabled = false;
                cmbPayBranchCode.SelectedValue = "";

                for (int i = 1; i <= 10; i++)
                {
                    ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                    ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                    ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                }
            }

            if (cmbPaymentType.SelectedValue == "4")
            {
                cmbPolicyPayBranchCode.Enabled = true;
            }
            else
            {
                cmbPolicyPayBranchCode.Enabled = false;
                cmbPolicyPayBranchCode.SelectedValue = "";
            }

            if (cmbPaymentTypeAct.SelectedValue == "4")
            {
                cmbPayBranchCodeAct.Enabled = true;
            }
            else
            {
                cmbPayBranchCodeAct.Enabled = false;
                cmbPayBranchCodeAct.SelectedValue = "";
            }
        }

        private string GetPayAmountStr(decimal? amount)
        {
            return amount != null ? amount.Value.ToString("#,##0.00") : "";
        }
        private DateTime GetDate(DateTime? date_time)
        {
            return date_time != null ? date_time.Value : DateTime.MinValue;
        }

        private void setBranchControl()
        {
            if (cmbPaymentType.SelectedValue == "4")
            {
                cmbPayBranchCode.Visible = true;
                cmbPayBranchCode.Text = "";
            }
            else
            {
                cmbPayBranchCode.Text = "";
            }
        }

        protected void cmbPaymentmethodAct_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                setPaymentActControl();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbPaymentTypeAct_TextChanged(object sender, EventArgs e)
        {
            try
            {
                setBranchActControl();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            } 
        }

        private void setPaymentActControl()
        {
            if (cmbPaymentmethodAct.SelectedValue == "2")
            {
                cmbPaymentTypeAct.Enabled = true;
                cmbActPayBranchCode.Enabled = false;
                cmbActPayBranchCode.SelectedValue = "";
            }
            else if (cmbPaymentmethod.SelectedValue != "0" && cmbPaymentmethod.SelectedValue != "")
            {
                cmbPaymentTypeAct.Enabled = true;
                cmbActPayBranchCode.Enabled = false;
                cmbActPayBranchCode.SelectedValue = "";
            }
            else
            {
                cmbPaymentTypeAct.Enabled = true;
                cmbActPayBranchCode.Enabled = false;
                cmbActPayBranchCode.SelectedValue = "";
            }

            if (cmbPaymentType.SelectedValue == "4")
            {
                cmbPayBranchCodeAct.Enabled = true;
            }
            else
            {
                cmbPayBranchCodeAct.Enabled = false;
                cmbPayBranchCodeAct.SelectedValue = "";
            }
        }

        private void setBranchActControl()
        {
            if (cmbPaymentType.SelectedValue == "4")
            {
                cmbActPayBranchCode.Visible = true;
                cmbActPayBranchCode.Text = "";
            }
            else
            {
                cmbActPayBranchCode.Text = "";
            }
        }

        #endregion

        protected void lbAdvanceSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnAdvanceSearch.Style["display"] == "" || pnAdvanceSearch.Style["display"] == "none")
                {
                    lbAdvanceSearch.Text = "[-] <b>ข้อมูลประกันภัย</b>";
                    pnAdvanceSearch.Style["display"] = "block";
                    txtAdvanceSearch.Text = "Y";
                }
                else
                {
                    lbAdvanceSearch.Text = "[+] <b>ข้อมูลประกันภัย</b>";
                    pnAdvanceSearch.Style["display"] = "none";
                    txtAdvanceSearch.Text = "N";
                }
                StaffBiz.SetCollapse(HttpContext.Current.User.Identity.Name, txtAdvanceSearch.Text.Trim() == "N" ? true : false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void detailPromotion_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnPromotion.Style["display"] == "" || pnPromotion.Style["display"] == "none")
                {
                    detailPromotion.Text = "[-] <b>ข้อมูลโปรโมชั่นประกันภัย</b>";
                    pnPromotion.Style["display"] = "block";
                    txtPromotion.Text = "Y";
                }
                else
                {
                    detailPromotion.Text = "[+] <b>ข้อมูลโปรโมชั่นประกันภัย</b>";
                    pnPromotion.Style["display"] = "none";
                    txtPromotion.Text = "N";
                }
                StaffBiz.SetCollapse(HttpContext.Current.User.Identity.Name, txtPromotion.Text.Trim() == "N" ? true : false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void surety_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnSurety.Style["display"] == "" || pnSurety.Style["display"] == "none")
                {
                    surety.Text = "[-] <b>ข้อมูลผู้ค้ำ</b>";
                    pnSurety.Style["display"] = "block";
                    txtSurety.Text = "Y";
                }
                else
                {
                    surety.Text = "[+] <b>ข้อมูลผู้ค้ำ</b>";
                    pnSurety.Style["display"] = "none";
                    txtSurety.Text = "N";
                }
                StaffBiz.SetCollapse(HttpContext.Current.User.Identity.Name, txtSurety.Text.Trim() == "N" ? true : false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void legislation_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnLegislation.Style["display"] == "" || pnLegislation.Style["display"] == "none")
                {
                    legislation.Text = "[-] <b>ข้อมูล พรบ.</b>";
                    pnLegislation.Style["display"] = "block";
                    txtLegislation.Text = "Y";
                }
                else
                {
                    legislation.Text = "[+] <b>ข้อมูล พรบ.</b>";
                    pnLegislation.Style["display"] = "none";
                    txtLegislation.Text = "N";
                }
                StaffBiz.SetCollapse(HttpContext.Current.User.Identity.Name, txtLegislation.Text.Trim() == "N" ? true : false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void sendDocument_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnSendDocument.Style["display"] == "" || pnSendDocument.Style["display"] == "none")
                {
                    sendDocument.Text = "[-] <b>ที่อยู่ในการจัดสี่งเอกสาร</b>";
                    pnSendDocument.Style["display"] = "block";
                    txtSendDocument.Text = "Y";
                }
                else
                {
                    sendDocument.Text = "[+] <b>ที่อยู่ในการจัดสี่งเอกสาร</b>";
                    pnSendDocument.Style["display"] = "none";
                    txtSendDocument.Text = "N";
                }
                StaffBiz.SetCollapse(HttpContext.Current.User.Identity.Name, txtSendDocument.Text.Trim() == "N" ? true : false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void noClaim_Click(object sender, EventArgs e)
        {
            if (pnNoClaim.Style["display"] == "" || pnNoClaim.Style["display"] == "none")
            {
                noClaim.Text = "[-] <b>ระงับเคลมและยกเลิกระงับเคลม</b>";
                pnNoClaim.Style["display"] = "block";
            }
            else
            {
                noClaim.Text = "[+] <b>ระงับเคลมและยกเลิกระงับเคลม</b>";
                pnNoClaim.Style["display"] = "none";
            }
        }

        protected void noIncentive_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnNoIncentive.Style["display"] == "" || pnNoIncentive.Style["display"] == "none")
                {
                    noIncentive.Text = "[-] <b>เลขที่รับแจ้งและ Incentive</b>";
                    pnNoIncentive.Style["display"] = "block";
                    txtNoIncentive.Text = "Y";
                }
                else
                {
                    noIncentive.Text = "[+] <b>เลขที่รับแจ้งและ Incentive</b>";
                    pnNoIncentive.Style["display"] = "none";
                    txtNoIncentive.Text = "N";
                }
                StaffBiz.SetCollapse(HttpContext.Current.User.Identity.Name, txtSendDocument.Text.Trim() == "N" ? true : false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void problem_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnProblem.Style["display"] == "" || pnProblem.Style["display"] == "none")
                {
                    problem.Text = "[-] <b>งานติดปัญหา</b>";
                    pnProblem.Style["display"] = "block";
                    txtproblem.Text = "Y";
                }
                else
                {
                    problem.Text = "[+] <b>งานติดปัญหา</b>";
                    pnProblem.Style["display"] = "none";
                    txtproblem.Text = "N";
                }
                StaffBiz.SetCollapse(HttpContext.Current.User.Identity.Name, txtproblem.Text.Trim() == "N" ? true : false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void payment_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnPayment.Style["display"] == "" || pnPayment.Style["display"] == "none")
                {
                    payment.Text = "[-] <b>ข้อมูลการชำระเงิน</b>";
                    pnPayment.Style["display"] = "block";
                    txtPayment.Text = "Y";
                }
                else
                {
                    payment.Text = "[+] <b>ข้อมูลการชำระเงิน</b>";
                    pnPayment.Style["display"] = "none";
                    txtPayment.Text = "N";
                }
                StaffBiz.SetCollapse(HttpContext.Current.User.Identity.Name, txtPayment.Text.Trim() == "N" ? true : false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchPromotionData(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        private void GetSuretyData(int PreleadId, int pageIndex)
        {
            List<SuretyData> list = SlmScr999Biz.GetSuretyData(PreleadId);

            gvSurety.DataSource = list;
            gvSurety.DataBind();
            pcTop.SetGridview(gvSurety);
            pcTop.GenerateRecordNumber(0, 0);
        }


        private void GetReceiptData(PreleadCompareDataCollection data)
        {
            gvReceipt.DataSource = data.ReceiptList;
            gvReceipt.DataBind();
        }

        public string SortExpressionProperty
        {
            get
            {
                if (ViewState["ExpressionState"] == null)
                {
                    ViewState["ExpressionState"] = string.Empty;
                }
                return ViewState["ExpressionState"].ToString();
            }
            set
            {
                ViewState["ExpressionState"] = value;
            }
        }

        public SortDirection SortDirectionProperty
        {
            get
            {
                if (ViewState["SortingState"] == null)
                {
                    ViewState["SortingState"] = SortDirection.Ascending;
                }
                return (SortDirection)ViewState["SortingState"];
            }
            set
            {
                ViewState["SortingState"] = value;
            }
        }       

        protected void cmbCarBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string carBrandValue = cmbCarBrand.SelectedItem.Value;

                cmbCarName.DataSource = BrandCarBiz.ListCodeCar(carBrandValue);
                cmbCarName.DataTextField = "ModelName";
                cmbCarName.DataValueField = "ModelCode";
                cmbCarName.DataBind();
                cmbCarName.Items.Insert(0, new ListItem("", ""));

                cmbModelYear.DataSource = BrandCarBiz.ListModelYear(cmbCarBrand.SelectedItem.Value, cmbCarName.SelectedItem.Value);
                cmbModelYear.DataTextField = "TextField";
                cmbModelYear.DataValueField = "ValueField";
                cmbModelYear.DataBind();
                cmbModelYear.Items.Insert(0, new ListItem("", ""));

                cmbCarBrand2.SelectedValue = carBrandValue;
                cmbCarName2.DataSource = BrandCarBiz.ListCodeCar(carBrandValue);
                cmbCarName2.DataTextField = "ModelName";
                cmbCarName2.DataValueField = "ModelCode";
                cmbCarName2.DataBind();
                cmbCarName2.Items.Insert(0, new ListItem("", ""));

                pnCar.Update();
                pnCarCondition.Update();

                DoSearchPromotionData(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbCarName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
                string carNameValue = cmbCarName.SelectedItem.Value;

                cmbCarName2.SelectedIndex = cmbCarName2.Items.IndexOf(cmbCarName2.Items.FindByValue(carNameValue));

                cmbModelYear.DataSource = BrandCarBiz.ListModelYear(cmbCarBrand.SelectedItem.Value, carNameValue);
                cmbModelYear.DataTextField = "TextField";
                cmbModelYear.DataValueField = "ValueField";
                cmbModelYear.DataBind();
                cmbModelYear.Items.Insert(0, new ListItem("", ""));

                pnCar.Update();
                pnCarCondition.Update();

                DoSearchPromotionData(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void rdoAddress_OnCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                setAddressControl();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }           
        }

        protected void txtDiscountBath_OnTextChanged(object sender, EventArgs e)
        {
            try
            {

                var textChange = ((TextBox)sender).ID;

                if ("txtDiscountBath_cur" == textChange)
                {
                    txtDiscountPercent_cur.Text = "";
                }
                else if ("txtDiscountBath_pro1" == textChange)
                {
                    txtDiscountPercent_pro1.Text = "";
                }
                else if ("txtDiscountBath_pro2" == textChange)
                {
                    txtDiscountPercent_pro2.Text = "";
                }
                else if ("txtDiscountBath_pro3" == textChange)
                {
                    txtDiscountPercent_pro3.Text = "";
                }

                calAll(true);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void txtDiscountPercent_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                var textChange = ((TextBox)sender).ID;

                if ("txtDiscountPercent_cur" == textChange)
                {
                    txtDiscountBath_cur.Text = "";
                }
                else if ("txtDiscountPercent_pro1" == textChange)
                {
                    txtDiscountBath_pro1.Text = "";
                }
                else if ("txtDiscountPercent_pro2" == textChange)
                {
                    txtDiscountBath_pro2.Text = "";
                }
                else if ("txtDiscountPercent_pro3" == textChange)
                {
                    txtDiscountBath_pro3.Text = "";
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

            calAll(true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
        }       

        protected void txtDiscountBathAct_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                var textChange = ((TextBox)sender).ID;

                if ("txtDiscountBathAct_pro1" == textChange)
                {
                    CalActSumPro1(true);
                }
                else if ("txtDiscountBathAct_pro2" == textChange)
                {
                    CalActSumPro2(true);
                }
                else if ("txtDiscountBathAct_pro3" == textChange)
                {
                    CalActSumPro3(true);

                }

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private void calAll(bool discount)
        {
            try
            {
                decimal PersonTypeDiscount;
                decimal Gross_Premium;
                decimal DiscountPercent;
                decimal DiscountBath;
                decimal Total_Gross_Premium;
                decimal Saft_Premium;
                decimal premium;

                PersonTypeDiscount = AppUtil.SafeDecimal(lblPersonType_cur.Text);

                Gross_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_cur.Text);     //เบี้ยสุทธิรวมภาษีอากร
                premium = AppUtil.SafeDecimal(lblNetpremium_cur.Text);                    //เบีั้ยสุทธิ
                DiscountPercent = AppUtil.SafeDecimal(txtDiscountPercent_cur.Text);
                DiscountBath = AppUtil.SafeDecimal(txtDiscountBath_cur.Text);

                if (DiscountPercent != 0 && DiscountBath == 0)
                {
                    DiscountBath = 0;
                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) - (premium * (DiscountPercent / 100m)));
                }
                else
                {
                    DiscountBath = AppUtil.SafeDecimal(txtDiscountBath_cur.Text);
                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) - DiscountBath);
                }

                if (discount)
                {
                    if (DiscountBath == 0)
                    {
                        txtDiscountBath_cur.Text = Math.Round((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium, 2).ToString("#,##0.00");
                        if (txtDiscountBath_cur.Text == "0.00")
                        {
                            txtDiscountPercent_cur.Text = "0";
                        }
                    }
                    else if (DiscountPercent == 0)
                    {
                        try
                        {
                            txtDiscountBath_cur.Text = DiscountBath.ToString("#,##0.00");
                            txtDiscountPercent_cur.Text = Math.Round(((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium) / (premium - PersonTypeDiscount) * 100, 2).ToString("#,##0");
                        }
                        catch
                        {
                            txtDiscountBath_cur.Text = DiscountBath.ToString("#,##0.00");
                            txtDiscountPercent_cur.Text = "0";
                        }
                    }
                }

                txtTotal_Voluntary_Gross_Premium_cur.Text = Total_Gross_Premium.ToString("#,##0.00");
                Saft_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pre.Text) - AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_cur.Text);
                txtSafe_cur.Text = Math.Abs(Saft_Premium).ToString("#,##0.00");

                if (Saft_Premium < 0)
                {
                    txtSafe_cur.ForeColor = Color.Red;
                }
                else
                {
                    txtSafe_cur.ForeColor = Color.Green;
                }

                PersonTypeDiscount = AppUtil.SafeDecimal(lblPersonType_pro1.Text);
                Gross_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro1.Text);
                premium = AppUtil.SafeDecimal(lblNetpremium_pro1.Text);
                DiscountPercent = AppUtil.SafeDecimal(txtDiscountPercent_pro1.Text);
                DiscountBath = AppUtil.SafeDecimal(txtDiscountBath_pro1.Text);
                if (DiscountPercent != 0 && DiscountBath == 0)
                {
                    DiscountBath = 0;
                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) - (premium * (DiscountPercent / 100m)));
                }
                else
                {
                    DiscountBath = AppUtil.SafeDecimal(txtDiscountBath_pro1.Text);
                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) - DiscountBath);
                }

                if (discount)
                {
                    if (DiscountBath == 0)
                    {
                        txtDiscountBath_pro1.Text = Math.Round((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium, 2).ToString("#,##0.00");
                    }
                    if (DiscountPercent == 0)
                    {
                        try
                        {
                            txtDiscountBath_pro1.Text = DiscountBath.ToString("#,##0.00");
                            txtDiscountPercent_pro1.Text = Math.Round(((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium) / (premium - PersonTypeDiscount) * 100, 2).ToString("#,##0");
                        }
                        catch
                        {
                            txtDiscountBath_pro1.Text = DiscountBath.ToString("#,##0.00");
                            txtDiscountPercent_pro1.Text = "0";
                        }
                    }
                }

                txtTotal_Voluntary_Gross_Premium_pro1.Text = Total_Gross_Premium.ToString("#,##0.00");

                Saft_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pre.Text) - AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro1.Text);
                txtSafe_pro1.Text = Math.Abs(Saft_Premium).ToString("#,##0.00");

                if (Saft_Premium < 0)
                {
                    txtSafe_pro1.ForeColor = Color.Red;
                }
                else
                {
                    txtSafe_pro1.ForeColor = Color.Green;
                }

                PersonTypeDiscount = AppUtil.SafeDecimal(lblPersonType_pro2.Text);

                Gross_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro2.Text);      //เบี้ยรวมภาษีอากร
                premium = AppUtil.SafeDecimal(lblNetpremium_pro2.Text);                         //เบี้ยสุทธิ
                DiscountPercent = AppUtil.SafeDecimal(txtDiscountPercent_pro2.Text);
                DiscountBath = AppUtil.SafeDecimal(txtDiscountBath_pro2.Text);
                if (DiscountPercent != 0 && DiscountBath == 0)
                {

                    DiscountBath = 0;
                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) - (premium * (DiscountPercent / 100m)));
                }
                else
                {
                    DiscountBath = AppUtil.SafeDecimal(txtDiscountBath_pro2.Text);
                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) - DiscountBath);
                }

                if (discount)
                {
                    if (DiscountBath == 0)
                    {
                        txtDiscountBath_pro2.Text = Math.Round((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium, 2).ToString("#,##0.00");
                    }
                    if (DiscountPercent == 0)
                    {
                        try
                        {
                            txtDiscountBath_pro2.Text = DiscountBath.ToString("#,##0.00");
                            txtDiscountPercent_pro2.Text = Math.Round(((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium) / (premium - PersonTypeDiscount) * 100, 2).ToString("#,##0");
                        }
                        catch
                        {
                            txtDiscountBath_pro2.Text = DiscountBath.ToString("#,##0.00");
                            txtDiscountPercent_pro2.Text = "0";
                        }
                    }
                }

                txtTotal_Voluntary_Gross_Premium_pro2.Text = Total_Gross_Premium.ToString("#,##0.00");
                Saft_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pre.Text) - AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro2.Text);
                txtSafe_pro2.Text = Math.Abs(Saft_Premium).ToString("#,##0.00");

                if (Saft_Premium < 0)
                {
                    txtSafe_pro2.ForeColor = Color.Red;
                }
                else
                {
                    txtSafe_pro2.ForeColor = Color.Green;
                }

                PersonTypeDiscount = AppUtil.SafeDecimal(lblPersonType_pro3.Text);
                Gross_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro3.Text);
                premium = AppUtil.SafeDecimal(lblNetpremium_pro3.Text);
                DiscountPercent = AppUtil.SafeDecimal(txtDiscountPercent_pro3.Text);
                DiscountBath = AppUtil.SafeDecimal(txtDiscountBath_pro3.Text);
                if (DiscountPercent != 0 && DiscountBath == 0)
                {

                    DiscountBath = 0;
                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) - (Gross_Premium * (DiscountPercent / 100m))); //zz
                }
                else
                {
                    DiscountBath = AppUtil.SafeDecimal(txtDiscountBath_pro3.Text);
                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) - DiscountBath);
                }

                if (discount)
                {
                    if (DiscountBath == 0)
                    {

                        txtDiscountBath_pro3.Text = Math.Round((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium, 2).ToString("#,##0.00");
                    }
                    if (DiscountPercent == 0)
                    {
                        try
                        {
                            txtDiscountBath_pro3.Text = DiscountBath.ToString("#,##0.00");
                            txtDiscountPercent_pro3.Text = Math.Round(((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium) / (Gross_Premium - PersonTypeDiscount) * 100, 2).ToString("#,##0"); //zz
                        }
                        catch
                        {
                            txtDiscountBath_pro3.Text = DiscountBath.ToString("#,##0.00");
                            txtDiscountPercent_pro3.Text = "0";
                        }
                    }
                }

                txtTotal_Voluntary_Gross_Premium_pro3.Text = Total_Gross_Premium.ToString("#,##0.00");

                Saft_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pre.Text) - AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro3.Text);
                txtSafe_pro3.Text = Math.Abs(Saft_Premium).ToString("#,##0.00");

                if (Saft_Premium < 0)
                {
                    txtSafe_pro3.ForeColor = Color.Red;
                }
                else
                {
                    txtSafe_pro3.ForeColor = Color.Green;
                }
                
                UpdatePanel2.Update();

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        private void calActAll(bool discount)
        {
            try
            {
                decimal PersonTypeDiscount;
                decimal Gross_Premium;
                decimal DiscountPercent;
                decimal DiscountBath;
                decimal Total_Gross_Premium;


                if (CalActPro1(false))
                {
                    lblActNetGrossPremiumAct_pro1.Text = (AppUtil.SafeDecimal(lblActGrossPremiumAct_pro1.Text) + AppUtil.SafeDecimal(lblActGrossStampAct_pre.Text) + AppUtil.SafeDecimal(lblActGrossVatAct_pro1.Text)).ToString("#,##0.00");

                    PersonTypeDiscount = AppUtil.SafeDecimal(lblActPersonType_pro1.Text);

                    Gross_Premium = AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro1.Text);
                    DiscountPercent = AppUtil.SafeDecimal(lblVat1PercentBathAct_pro1.Text);
                    DiscountBath = AppUtil.SafeDecimal(lblDiscountPercentAct_pro1.Text);

                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) * ((100 - DiscountPercent) / 100m)) - DiscountBath;

                    if (discount)
                    {
                        if (DiscountBath == 0)
                        {
                            lblDiscountPercentAct_pro1.Text = Math.Round((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium, 2).ToString("#,##0");
                        }
                        if (DiscountPercent == 0)
                        {
                            try
                            {
                                lblVat1PercentBathAct_pro1.Text = Math.Round(((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium) / (Gross_Premium - PersonTypeDiscount) * 100, 2).ToString("#,##0");
                            }
                            catch
                            {
                                lblVat1PercentBathAct_pro1.Text = "0";
                            }
                        }
                    }
                    txtDiscountBathAct_pro1.Text = Math.Round(Total_Gross_Premium, 2).ToString("#,##0.00");
                }

                if (CalActPro2(false))
                {
                    PersonTypeDiscount = AppUtil.SafeDecimal(lblActPersonType_pro2.Text);

                    Gross_Premium = AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro2.Text);
                    DiscountPercent = AppUtil.SafeDecimal(lblVat1PercentBathAct_pro2.Text);
                    DiscountBath = AppUtil.SafeDecimal(lblDiscountPercentAct_pro2.Text);

                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) * ((100 - DiscountPercent) / 100m)) - DiscountBath;

                    if (discount)
                    {
                        if (DiscountBath == 0)
                        {
                            lblDiscountPercentAct_pro2.Text = Math.Round((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium, 2).ToString("#,##0.00");
                        }
                        if (DiscountPercent == 0)
                        {
                            try
                            {
                                lblVat1PercentBathAct_pro2.Text = Math.Round(((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium) / (Gross_Premium - PersonTypeDiscount) * 100, 2).ToString("#,##0");
                            }
                            catch
                            {
                                lblVat1PercentBathAct_pro2.Text = "0";
                            }
                        }
                    }

                    txtDiscountBathAct_pro2.Text = Math.Round(Total_Gross_Premium, 2).ToString("#,##0.00");
                }

                if (CalActPro3(false))
                {
                    PersonTypeDiscount = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro3.Text);

                    Gross_Premium = AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro3.Text);
                    DiscountPercent = AppUtil.SafeDecimal(lblVat1PercentBathAct_pro3.Text);
                    DiscountBath = AppUtil.SafeDecimal(lblDiscountPercentAct_pro3.Text);

                    Total_Gross_Premium = ((Gross_Premium - PersonTypeDiscount) * ((100 - DiscountPercent) / 100m)) - DiscountBath;

                    if (discount)
                    {
                        if (DiscountBath == 0)
                        {
                            lblDiscountPercentAct_pro3.Text = Math.Round((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium, 2).ToString("#,##0.00");
                        }
                        if (DiscountPercent == 0)
                        {
                            try
                            {
                                lblVat1PercentBathAct_pro3.Text = Math.Round(((Gross_Premium - PersonTypeDiscount) - Total_Gross_Premium) / (Gross_Premium - PersonTypeDiscount) * 100, 2).ToString("#,##0");
                            }
                            catch
                            {
                                lblVat1PercentBathAct_pro3.Text = "0";
                            }
                        }
                    }

                    txtDiscountBathAct_pro3.Text = Math.Round(Total_Gross_Premium, 2).ToString("#,##0.00");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private void calcVoluntary_Gross_Premium()
        {
            try
            {
                PreleadDataCollection insuranceData = SlmScr999Biz.GetExitingPrelead(57313);
                double netPre = Convert.ToDouble(lblNetpremium_pre.Text);//เบี๊ยสุทธิ
                double duty = 0;//อากร
                double vat = 0;//ภาษี
                double voluntary_gross_premium = 0;//เบี๊ยประกันรวมภาษีอากร
                double personType = Convert.ToDouble(lblPersonType_pre.Text);//กรณีนิติบุคคล
                double discountpercent = 0;//ส่วนลด %
                double DiscountBath = 0;//ส่วนลด บาท
                double Total_Voluntary_Gross_Premium = 0;//เบี๊ยประกัยรวมภาษี

                if (insuranceData.Prev.slm_CardTypeId == 1)
                {
                    //ประเภทนิติบุคคล
                    //คำนวณอากร
                    duty = ((netPre * 0.4) / 100D);
                    lblDuty_pre.Text = duty.ToString();
                    //ภาษี
                    vat = ((netPre + duty) * 7) / 100D;
                    lblVat_amount_pre.Text = vat.ToString();
                    //เบี๊ยประกันรวมภาษีอากร
                    voluntary_gross_premium = netPre + vat + duty;

                    //คำนวณส่วนลด
                    //ภาษี 1%
                    personType = (netPre + duty) / 100D;
                    //ส่วนลด %
                    discountpercent = (DiscountBath * 100D) / netPre;
                    //เบี๊ยประกันที่ชำระ
                    Total_Voluntary_Gross_Premium = voluntary_gross_premium - personType;
                }
                else if (insuranceData.Prev.slm_CardTypeId == 2)
                {
                    //ประเภทบุคคลทั่วไป
                    //เบี๊ยสุทธิ
                    netPre = voluntary_gross_premium / 1.07428;
                    //อากร
                    duty = ((netPre * 0.4) / 100D);
                    //ภาษี
                    vat = ((netPre + duty) * 7D) / 100D;
                    //คำนวณส่วนลด
                    discountpercent = (DiscountBath * 100D) / netPre;
                    //เบี๊ยประกันที่ชำระ
                    Total_Voluntary_Gross_Premium = voluntary_gross_premium - DiscountBath;
                }

                else if (insuranceData.Curr.slm_CardTypeId == 1)
                {
                    //ประเภทนิติบุคคล
                    //คำนวณอากร
                    duty = ((netPre * 0.4) / 100D);
                    lblDuty_pre.Text = duty.ToString();
                    //ภาษี
                    vat = ((netPre + duty) * 7D) / 100D;
                    lblVat_amount_pre.Text = vat.ToString();
                    //เบี๊ยประกันรวมภาษีอากร
                    voluntary_gross_premium = netPre + vat + duty;

                    //คำนวณส่วนลด
                    //ภาษี 1%
                    personType = (netPre + duty) / 100D;
                    //ส่วนลด %
                    discountpercent = (DiscountBath * 100D) / netPre;
                    //เบี๊ยประกันที่ชำระ
                    Total_Voluntary_Gross_Premium = voluntary_gross_premium - personType;
                }
                else
                {
                    //ประเภทบุคคลทั่วไป
                    //เบี๊ยสุทธิ
                    netPre = voluntary_gross_premium / 1.07428;
                    //อากร
                    duty = ((netPre * 0.4) / 100D);
                    //ภาษี
                    vat = ((netPre + duty) * 7D) / 100D;
                    //คำนวณส่วนลด
                    discountpercent = (DiscountBath * 100D) / netPre;
                    //เบี๊ยประกันที่ชำระ
                    Total_Voluntary_Gross_Premium = voluntary_gross_premium - DiscountBath;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void chkCardType_OnCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                decimal custTypeDiscount;

                if (chkCardType.Checked)
                {
                    custTypeDiscount = (AppUtil.SafeDecimal(lblNetpremium_cur.Text) + AppUtil.SafeDecimal(lblDuty_cur.Text)) / 100m;
                }
                else
                {
                    custTypeDiscount = 0;
                }

                lblPersonType_cur.Text = custTypeDiscount.ToString("#,##0.00");

                if (chkCardType.Checked)
                {
                    custTypeDiscount = (AppUtil.SafeDecimal(lblNetpremium_pro1.Text) + AppUtil.SafeDecimal(lblDuty_pro1.Text)) / 100m;
                }
                else
                {
                    custTypeDiscount = 0;
                }

                lblPersonType_pro1.Text = custTypeDiscount.ToString("#,##0.00");

                if (chkCardType.Checked)
                {
                    custTypeDiscount = (AppUtil.SafeDecimal(lblNetpremium_pro2.Text) + AppUtil.SafeDecimal(lblDuty_pro2.Text)) / 100m;
                }
                else
                {
                    custTypeDiscount = 0;
                }

                lblPersonType_pro2.Text = custTypeDiscount.ToString("#,##0.00");

                if (chkCardType.Checked)
                {
                    custTypeDiscount = (AppUtil.SafeDecimal(lblNetpremium_pro3.Text) + AppUtil.SafeDecimal(lblDuty_pro3.Text)) / 100m;
                }
                else
                {
                    custTypeDiscount = 0;
                }

                lblPersonType_pro3.Text = custTypeDiscount.ToString("#,##0.00");

                if (!chkCardType.Checked)
                {
                    lblPersonType_cur.Text = "";
                    lblPersonType_pro1.Text = "";
                    lblPersonType_pro2.Text = "";
                    lblPersonType_pro3.Text = "";
                }

                calAll(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void chkCardType2_OnCheckedChanged(object sender, EventArgs e)
        {
            try
            {               
                decimal custTypeDiscount;
                if (chkCardType2.Checked)
                {
                    if (AppUtil.SafeDecimal(lblActGrossPremiumAct_pro1.Text) > 1000)
                    {
                        custTypeDiscount = (AppUtil.SafeDecimal(lblActGrossPremiumAct_pro1.Text) + AppUtil.SafeDecimal(lblActGrossStampAct_pro1.Text)) / 100m;
                    }
                    else
                    {
                        custTypeDiscount = 0;
                    }
                }
                else
                {
                    custTypeDiscount = 0;
                }

                lblActPersonType_pro1.Text = custTypeDiscount.ToString("#,##0.00");

                if (chkCardType2.Checked)
                {
                    if (AppUtil.SafeDecimal(lblActGrossPremiumAct_pro2.Text) > 1000)
                    {
                        custTypeDiscount = (AppUtil.SafeDecimal(lblActGrossPremiumAct_pro2.Text) + AppUtil.SafeDecimal(lblActGrossStampAct_pro2.Text)) / 100m;
                    }
                    else
                    {
                        custTypeDiscount = 0;
                    }
                }
                else
                {
                    custTypeDiscount = 0;
                }

                lblActPersonType_pro2.Text = custTypeDiscount.ToString("#,##0.00");

                if (chkCardType2.Checked)
                {
                    if (AppUtil.SafeDecimal(lblActGrossPremiumAct_pro3.Text) > 1000)
                    {
                        custTypeDiscount = (AppUtil.SafeDecimal(lblActGrossPremiumAct_pro3.Text) + AppUtil.SafeDecimal(lblActGrossStampAct_pro3.Text)) / 100m;
                    }
                    else
                    {
                        custTypeDiscount = 0;
                    }
                }
                else
                {
                    custTypeDiscount = 0;
                }

                lblActPersonType_pro3.Text = custTypeDiscount.ToString("#,##0.00");

                if (!chkCardType2.Checked)
                {
                    lblActPersonType_pro1.Text = "";
                    lblActPersonType_pro2.Text = "";
                    lblActPersonType_pro3.Text = "";
                }

                CalActPro1(false);
                CalActPro2(false);
                CalActPro3(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                
                tempAllData();
                PreleadCompareDataCollection pc = getSelectTabData();
                int i = 0;
                int j = 0;

                if (pc.ComparePromoList != null && pc.ComparePromoList.Count > 0)
                {
                    i = i + pc.ComparePromoList.Count;
                }

                if (pc.ActPromoList != null && pc.ActPromoList.Count > 0)
                {
                    j = j + pc.ActPromoList.Count;
                }

                GridViewRow row = (GridViewRow)(((CheckBox)sender).NamingContainer);
                string rowView = gvResultPromotion.DataKeys[row.DataItemIndex].Value.ToString();

                bool state = ((CheckBox)sender).Checked;
                if (rowView != null)
                {
                    if (state)
                    {
                        if (i < 3 && j < 3)
                        {
                            setPromotion(AppUtil.SafeDecimal(rowView), state);
                        }
                        else
                        {
                            ((CheckBox)sender).Checked = false;
                            AppUtil.ClientAlertTabRenew(Page, "เลือกโปรโมชั่นได้ไม่เกิน 3 โปรโมชั่น");
                        }
                    }
                    else
                    {
                        setPromotion(AppUtil.SafeDecimal(rowView), state);
                    }
                }
                else
                {
                    setPromotion(0, state);
                }
                // คิดส่วนลดนิติบุคคล
                chkCardType_OnCheckedChanged(null, null);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lnkSelect_OnClick(object sender, EventArgs e)
        {
            try
            {                
                PreleadCompareDataCollection pc = getSelectTabData();
                int i = 0;
                int j = 0;

                if (pc.ComparePromoList != null && pc.ComparePromoList.Count > 0)
                {
                    i = i + pc.ComparePromoList.Count;
                }

                if (pc.ActPromoList != null && pc.ActPromoList.Count > 0)
                {
                    j = j + pc.ActPromoList.Count;
                }

                bool state = true;

                if (state)
                {
                    if (i < 3 && j < 3)
                    {
                        setPromotion(0, state);
                    }
                    else
                    {
                        AppUtil.ClientAlertTabRenew(Page, "เลือกโปรโมชั่นได้ไม่เกิน 3 โปรโมชั่น");
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

        private void setPromotion(decimal PromotionId, bool state)
        {
            setPromotion(PromotionId, state, "");
        }

        private void setPromotion(decimal PromotionId, bool state, string blankType)
        {
            try
            {
                PreleadCompareDataCollection p = getSelectTabData();
                if (p.ComparePromoList == null)
                {
                    p.ComparePromoList = new List<PreleadCompareData>();
                }
                if (p.ActPromoList == null)
                {
                    p.ActPromoList = new List<PreleadCompareActData>();
                }

                if (state)
                {
                    PreleadCompareData insuranceDataPromo = ActivityPreLeadBiz.SearchPromotionOrder(PromotionId);
                    PreleadCompareActData insuranceDataActPromo = ActivityPreLeadBiz.SearchPromotionActOrder(PromotionId);

                    if (PromotionId != 0 && !ActivityPreLeadBiz.CheckCompanyInActive(PromotionId))
                    {
                        AppUtil.ClientAlertTabRenew(Page, "ไม่สามารถเลือกขาย Promotion ดังกล่าวได้ เนื่องจากบริษัทประกันภัยรถยนต์มีสถานะ Inactive กรุณาเลือกใหม่หรือติดต่อทีม Product-ประกันภัยรถยนต์");
                        clearCheckBox(PromotionId.ToString());
                    }
                    else
                    {
                        if (insuranceDataPromo == null)
                        {
                            insuranceDataPromo = new PreleadCompareData();

                        }
                        if (insuranceDataActPromo == null)
                        {
                            insuranceDataActPromo = new PreleadCompareActData();
                        }

                        if (p.ComparePrev.slm_PolicyEndCoverDate != null)
                        {
                            insuranceDataPromo.slm_PolicyStartCoverDate = p.ComparePrev.slm_PolicyEndCoverDate;
                            insuranceDataPromo.slm_PolicyEndCoverDate = p.ComparePrev.slm_PolicyEndCoverDate.Value.AddYears(1);
                        }

                        if (p.ActPrev.slm_ActEndCoverDate != null)
                        {
                            insuranceDataActPromo.slm_ActStartCoverDate = p.ActPrev.slm_ActEndCoverDate;
                            insuranceDataActPromo.slm_ActEndCoverDate = p.ActPrev.slm_ActEndCoverDate.Value.AddYears(1);
                        }

                        var rnd = new Random(DateTime.Now.Millisecond);
                        var numberPolicy = rnd.Next();
                        var numberAct = rnd.Next();

                        if (p.ComparePromoList != null && p.ComparePromoList.Count > 0)
                        {

                            if (PromotionId != 0)
                            {
                                var data = p.ComparePromoList.Where(c => c.slm_PromotionId == insuranceDataPromo.slm_PromotionId).FirstOrDefault();
                                if (data != null)
                                {
                                    insuranceDataPromo.slm_Seq = p.ComparePromoList.Count + 3;
                                    data = insuranceDataPromo;
                                }
                                else
                                {
                                    insuranceDataPromo.slm_Seq = p.ComparePromoList.Count + 3;
                                    p.ComparePromoList.Add(insuranceDataPromo);
                                }
                            }
                            else
                            {
                                if (blankType == "P")
                                {
                                    insuranceDataPromo.slm_Seq = p.ComparePromoList.Count + 3;
                                    insuranceDataPromo.slm_PromotionId = 0;
                                    ((HiddenField)this.FindControlRecursive("hidSeq" + (p.ComparePromoList.Count + 1))).Value = (p.ComparePromoList.Count + 3).ToString();
                                    p.ComparePromoList.Add(insuranceDataPromo);
                                }
                            }

                        }
                        else
                        {
                            if (PromotionId != 0)
                            {
                                insuranceDataPromo.slm_Seq = 3;
                                p.ComparePromoList.Add(insuranceDataPromo);
                            }
                            else
                            {
                                if (blankType == "P")
                                {
                                    insuranceDataPromo.slm_Seq = 3;
                                    insuranceDataPromo.slm_PromotionId = 0;
                                    ((HiddenField)this.FindControlRecursive("hidSeq1")).Value = "3";
                                    p.ComparePromoList.Add(insuranceDataPromo);
                                }
                            }
                        }

                        if (p.ActPromoList != null && p.ActPromoList.Count > 0)
                        {
                            if (PromotionId != 0)
                            {
                                var data = p.ActPromoList.Where(c => c.slm_PromotionId == insuranceDataActPromo.slm_PromotionId).FirstOrDefault();
                                if (data != null)
                                {
                                    insuranceDataActPromo.slm_Seq = p.ActPromoList.Count + 2;
                                    data = insuranceDataActPromo;
                                }
                                else
                                {

                                    insuranceDataActPromo.slm_Seq = p.ActPromoList.Count + 2;
                                    ((HiddenField)this.FindControlRecursive("hidSeqAct" + (p.ActPromoList.Count + 2))).Value = (p.ActPromoList.Count + 2).ToString();
                                    p.ActPromoList.Add(insuranceDataActPromo);
                                }
                            }
                            else
                            {
                                if (blankType == "A")
                                {
                                    insuranceDataActPromo.slm_Seq = p.ActPromoList.Count + 2;
                                    insuranceDataActPromo.slm_PromotionId = 0;
                                    p.ActPromoList.Add(insuranceDataActPromo);
                                }
                            }
                        }
                        else
                        {
                            if (PromotionId != 0)
                            {
                                insuranceDataActPromo.slm_Seq = 2;
                                p.ActPromoList.Add(insuranceDataActPromo);
                            }
                            else
                            {
                                if (blankType == "A")
                                {
                                    insuranceDataActPromo.slm_Seq = 2;
                                    insuranceDataActPromo.slm_PromotionId = 0;
                                    hidSeqAct1.Value = "2";
                                    p.ActPromoList.Add(insuranceDataActPromo);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (p.ComparePromoList != null && p.ComparePromoList.Count > 0)
                    {

                        PreleadCompareData data = null;
                        if (blankType != "" && blankType == "P")
                        {
                            data = p.ComparePromoList.Where(c => c.slm_Seq == PromotionId).FirstOrDefault();
                        }
                        else if (blankType == "")
                        {
                            data = p.ComparePromoList.Where(c => c.slm_PromotionId == PromotionId).FirstOrDefault();
                        }

                        if (data != null)
                        {
                            p.ComparePromoList.Remove(data);
                        }
                    }

                    if (p.ActPromoList != null && p.ActPromoList.Count > 0)
                    {
                        PreleadCompareActData data = null;
                        if (blankType != null && blankType == "A")
                        {
                            data = p.ActPromoList.Where(c => c.slm_Seq == PromotionId).FirstOrDefault();
                        }
                        else if (blankType == "")
                        {
                            data = p.ActPromoList.Where(c => c.slm_PromotionId == PromotionId).FirstOrDefault();
                        }
                        if (data != null)
                        {
                            p.ActPromoList.Remove(data);
                        }
                    }
                }

                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                {
                    pg.PreleadCompareDataCollectionMain.ComparePromoList = p.ComparePromoList;
                    pg.PreleadCompareDataCollectionMain.ActPromoList = p.ActPromoList;
                }
                else
                {
                    foreach (PreleadCompareDataCollection pp in pg.PreleadCompareDataCollections)
                    {

                        if (pp.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                        {
                            pp.ComparePromoList = p.ComparePromoList;
                            pp.ActPromoList = p.ActPromoList;
                            break;
                        }
                    }
                }

                Session[SessionPrefix+"allData"] = pg;
                setPromotion();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private void setPromotion(bool reBindPreleadCompare = true, bool reBindPreleadCompareAct = true)
        {

            try
            {
                List<string> promoPolicyClearList = new List<string>();
                List<string> promoActClearList = new List<string>();
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                PreleadCompareDataCollection pc = getSelectTabData();

                if (reBindPreleadCompare)
                {
                    bindPreleadCompare(pc);
                }
                if (reBindPreleadCompareAct)
                {
                    bindPreleadCompareAct(pc);
                }               

                if (pg.PreleadCompareDataCollectionMain.ComparePromoList == null)
                {
                    pg.PreleadCompareDataCollectionMain.ComparePromoList = new List<PreleadCompareData>();
                    pg.PreleadCompareDataCollectionMain.ActPromoList = new List<PreleadCompareActData>();
                }

                if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                {
                    pg.PreleadCompareDataCollectionMain = pc;
                }
                else
                {
                    var temp = pg.PreleadCompareDataCollections.Where(pl => pl.keyTab == tabRenewInsuranceContainer.ActiveTab.ID).FirstOrDefault();
                    if (temp != null)
                    {
                        temp = pc;
                    }
                }

                var compare = pc; // ตั้งไว้เผื่อมีปัญหาอะไรเป็น null อีก 

                if (compare.RenewIns != null && compare.RenewIns.slm_TicketId != null && CheckOutStandingProblem(compare.RenewIns.slm_TicketId))
                {
                    setDisableCurr(true);
                    for (var i = 1; i <= 3; i++)
                    {
                        if (compare.ComparePromoList.Count > i - 1)
                        {
                            if (pc.ComparePromoList[i - 1].slm_PromotionId == null)
                            {
                                setDisablePromoPolicy(i, true);
                            }   
                            else // not promotion -> ต้อง enable ปุ่มลบ 
                            {
                                ((Button)this.FindControlRecursive("imgDel_pro" + i)).Enabled = true;
                            }                             
                        }
                        if (pc.ActPromoList.Count > i - 1)
                        {
                            if (pc.ActPromoList[i - 1].slm_PromotionId == null)
                            {
                                setDisablePromoAct(i, true);
                            }
                            else // not promotion -> ต้อง enable ปุ่มลบ
                            {
                                ((Button)this.FindControlRecursive("imgDelAct_pro" + i)).Enabled = true;
                            }  
                        }
                    }
                }
                else
                {
                    if (pc.RenewIns == null)
                    {
                        pc.RenewIns = new RenewInsuranceData();
                    }
                        
                    disableReceiptAct(pc, pc.RenewIns.slm_ReceiveNo, pc.RenewIns.slm_ActSendDate);
                    disableReceiptPolicy(pc, pc.RenewIns.slm_ReceiveNo);

                    var username = Page.User.Identity.Name;
                    var staff = GetCurrentStaff();

                    //var staff = StaffBiz.GetStaff(Page.User.Identity.Name);
                    //bool OwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(pc.RenewIns.slm_TicketId, username);
                    //bool supervisor = ActivityLeadBiz.CheckSupervisor(username);

                    if (IsOwnerOrDelegate == null)
                    {
                        IsOwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(pc.RenewIns.slm_TicketId, username);
                    }
                    if (IsSupervisor == null)
                    {
                        IsSupervisor = ActivityLeadBiz.CheckSupervisor(username);
                    }

                    SlmScr004Biz biz = new SlmScr004Biz();
                    if (PolicyPurchasedFlag == null)
                    {
                        PolicyPurchasedFlag = biz.CheckMotorRenewPurchased(GetTicketIdActiveTab());
                    }
                    if (ActPurchasedFlag == null)
                    {
                        ActPurchasedFlag = biz.CheckActPurchased(GetTicketIdActiveTab());
                    }

                    setReceiveActButton(pc, username, staff, IsOwnerOrDelegate.Value, IsSupervisor.Value, ActPurchasedFlag.Value);
                    setReceivePolicyButton(pc, username, staff, IsOwnerOrDelegate.Value, IsSupervisor.Value, PolicyPurchasedFlag.Value);
                }
                Session[SessionPrefix+"allData"] = pg;
                calAll(false);
                UpdatePanel2.Update();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setDisableCurr(bool state)
        {

            lblCoverageType_cur.Enabled = state;
            lblInsNameTh_cur.Enabled = state;
            lblMaintanance_cur.Enabled = state;
            lblCost_cur.Enabled = state;
            cmbDeDuctFlag_cur.Enabled = state;
            lblVoluntary_Cov_Amt_cur.Enabled = state;
            lblCostFT_cur.Enabled = state;
            lblNetpremium_cur.Enabled = state;
            lblVoluntary_Gross_Premium_cur.Enabled = state;
        }

        private void setDisablePromoPolicy(int i, bool state)
        {
            ((DropDownList)this.FindControlRecursive("lblCoverageType_pro" + i)).Enabled = state;
            ((DropDownList)this.FindControlRecursive("lblInsNameTh_pro" + i)).Enabled = state;
            ((DropDownList)this.FindControlRecursive("lblMaintanance_pro" + i)).Enabled = state;
            ((TextBox)this.FindControlRecursive("lblCost_pro" + i)).Enabled = state;
            ((DropDownList)this.FindControlRecursive("cmbDeDuctFlag_pro" + i)).Enabled = state;
            ((TextBox)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + i)).Enabled = state;
            ((TextBox)this.FindControlRecursive("lblCostFT_pro" + i)).Enabled = state;
            ((TextBox)this.FindControlRecursive("lblNetpremium_pro" + i)).Enabled = state;
            ((TextBox)this.FindControlRecursive("lblVoluntary_Gross_Premium_pro" + i)).Enabled = state;
        }

        private void setDisablePromoAct(int i, bool state)
        {
            //Act
            ((DropDownList)this.FindControlRecursive("lblCompanyInsuranceAct_pro" + i)).Enabled = state;
            ((TextBox)this.FindControlRecursive("lblActNetGrossPremiumFullAct_pro" + i)).Enabled = state;
            ((Button)this.FindControlRecursive("imgDelAct_pro" + i)).Enabled = state;
        }
        private void setDisablePayment(bool state)
        {
            cmbPolicyPayMethod.Enabled = state;
            txtPolicyAmountPeriod.Enabled = state;
            cmbPayOption.Enabled = state;
            cmbPayBranchCode.Enabled = state;
            tdmPolicyPaymentDate1.Enabled = state;
            txtPolicyPaymentAmount1.Enabled = state;
            tdmPolicyPaymentDate2.Enabled = state;
            txtPolicyPaymentAmount2.Enabled = state;
            tdmPolicyPaymentDate3.Enabled = state;
            txtPolicyPaymentAmount3.Enabled = state;
            tdmPolicyPaymentDate4.Enabled = state;
            txtPolicyPaymentAmount4.Enabled = state;
            tdmPolicyPaymentDate5.Enabled = state;
            txtPolicyPaymentAmount5.Enabled = state;
            tdmPolicyPaymentDate6.Enabled = state;
            txtPolicyPaymentAmount6.Enabled = state;
            tdmPolicyPaymentDate7.Enabled = state;
            txtPolicyPaymentAmount7.Enabled = state;
            tdmPolicyPaymentDate8.Enabled = state;
            txtPolicyPaymentAmount8.Enabled = state;
            tdmPolicyPaymentDate9.Enabled = state;
            txtPolicyPaymentAmount9.Enabled = state;
            tdmPolicyPaymentDate10.Enabled = state;
            txtPolicyPaymentAmount10.Enabled = state;

            cmbActPayOption.Enabled = state;
            cmbActPayMethod.Enabled = state;
            cmbActPayBranchCode.Enabled = state;
        }

        private void clearPromotionPolicyList(List<string> promoPolicyClearList)
        {
            for (int j = 3; j > promoPolicyClearList.Count(); j--)
            {
                clearPromotionPolicyControl(j.ToString());
            }
        }

        private void clearPromotionActList(List<string> promoActClearList)
        {
            for (int j = 3; j > promoActClearList.Count(); j--)
            {
                clearPromotionActControl(j.ToString());
            }
        }       

        private void clearPromotionPolicyControl(string j)
        {
            try
            {
                ((HiddenField)this.FindControlRecursive("hidPromotionId" + j)).Value = "";

                ((RadioButton)this.FindControlRecursive("rbInsNameTh_pro" + j)).Checked = false;
                ((Label)this.FindControlRecursive("lblPromoId_pro" + j)).Text = "";
                ((Label)this.FindControlRecursive("lblInsnameth_name_pro" + j)).Text = "";
                ((DropDownList)this.FindControlRecursive("lblCoverageType_pro" + j)).SelectedValue = "";
                ((DropDownList)this.FindControlRecursive("lblInsNameTh_pro" + j)).SelectedValue = "";
                ((DropDownList)this.FindControlRecursive("lblCoverageType2_pro" + j)).SelectedValue = "";
                ((DropDownList)this.FindControlRecursive("lblMaintanance_pro" + j)).SelectedValue = "";
                ((Label)this.FindControlRecursive("lblVoluntary_Cov_Amt_pro" + j)).Text = "";
                ((Label)this.FindControlRecursive("lblCostFT_pro" + j)).Text = "";
                ((Label)this.FindControlRecursive("lblDuty_pro" + j)).Text = "";
                ((Label)this.FindControlRecursive("lblVat_amount_pro" + j)).Text = "";
                ((Label)this.FindControlRecursive("lblVoluntary_Gross_Premium_pro" + j)).Text = "";

                ((HiddenField)this.FindControlRecursive("hidInjuryDeath_pro" + j)).Value = "";
                ((HiddenField)this.FindControlRecursive("hidTPPD_pro" + j)).Value = "";
                ((HiddenField)this.FindControlRecursive("hidPersonalAccident_pro" + j)).Value = "";
                ((HiddenField)this.FindControlRecursive("hidPersonalAccidentDriver_pro" + j)).Value = "";
                ((HiddenField)this.FindControlRecursive("hidPersonalAccidentPassenger_pro" + j)).Value = "";
                ((HiddenField)this.FindControlRecursive("hidMedicalFee_pro" + j)).Value = "";
                ((HiddenField)this.FindControlRecursive("hidMedicalFeeDriver_pro" + j)).Value = "";
                ((HiddenField)this.FindControlRecursive("hidMedicalFeePassenger_pro" + j)).Value = "";
                ((HiddenField)this.FindControlRecursive("hidInsuranceDriver_pro" + j)).Value = "";
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private void clearPromotionActControl(string j)
        {
            try
            {
                //พรบ.
                ((HiddenField)this.FindControlRecursive("hidPromotionActId" + j)).Value = "";
                ((RadioButton)this.FindControlRecursive("rbAct_pro" + j)).Checked = false;
                ((Label)this.FindControlRecursive("lblPromoAct_name_pro" + j)).Text = "";
                ((DropDownList)this.FindControlRecursive("lblCompanyInsuranceAct_pro" + j)).SelectedValue = "";
                ((Label)this.FindControlRecursive("lblActGrossPremiumAct_pro" + j)).Text = "";
                ((Label)this.FindControlRecursive("lblActGrossStampAct_pro" + j)).Text = "";
                ((Label)this.FindControlRecursive("lblActGrossVatAct_pro" + j)).Text = "";
                ((DropDownList)this.FindControlRecursive("cmbActIssuePlace_pro" + j)).SelectedValue = "";
                ((RadioButton)this.FindControlRecursive("rbRegisterAct_pro" + j)).Checked = false;
                ((RadioButton)this.FindControlRecursive("rbNormalAct_pro" + j)).Checked = false;
                ((TextBox)this.FindControlRecursive("txtSignNoAct_pro" + j)).Text = "";
                ((TextDateMask)this.FindControlRecursive("txtActStartCoverDateAct_pro" + j)).DateValue = DateTime.MinValue;
                ((TextDateMask)this.FindControlRecursive("txtActEndCoverDateAct_pro" + j)).DateValue = DateTime.MinValue;
                ((TextDateMask)this.FindControlRecursive("txtCarTaxExpiredDateAct_pro" + j)).DateValue = DateTime.MinValue;
                ((Label)this.FindControlRecursive("lblActNetGrossPremiumAct_pro" + j)).Text = "";
                ((TextBox)this.FindControlRecursive("lblVat1PercentBathAct_pro1" + j)).Text = "";
                ((TextBox)this.FindControlRecursive("lblDiscountPercentAct_pro1" + j)).Text = "";
                ((TextBox)this.FindControlRecursive("txtDiscountBathAct_pro1" + j)).Text = "";
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void cmbCarBrand2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {             
                string carBrandValue = cmbCarBrand2.SelectedItem.Value;

                cmbCarName2.DataSource = BrandCarBiz.ListCodeCar(carBrandValue);
                cmbCarName2.DataTextField = "ModelName";
                cmbCarName2.DataValueField = "ModelCode";
                cmbCarName2.DataBind();
                cmbCarName2.Items.Insert(0, new ListItem("", ""));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private void setDivPayment()
        {
            try
            {
                PreleadCompareDataCollection data = getSelectTabData();
                if (data.ReceiptList != null && data.ReceiptList.Count > 0)
                {
                    divPayment.Visible = true;

                }
                else
                {
                    divPayment.Visible = false;
                }

                if ((data.RenewIns.slm_PolicyGrossPremiumTotal == null ? 0m : data.RenewIns.slm_PolicyGrossPremiumTotal.Value) == 0)
                {
                    txtDiscountPercent.Enabled = false;
                    txtPolicyDiscountAmt.Enabled = false;
                }
                else
                {
                    txtDiscountPercent.Enabled = false;
                    txtPolicyDiscountAmt.Enabled = false;
                }

                if ((data.RenewIns.slm_ActNetPremium == null ? 0m : data.RenewIns.slm_ActNetPremium.Value) == 0)
                {
                    txtDiscountPercentAct.Enabled = false;
                    txtActDiscountAmt.Enabled = false;
                }
                else
                {
                    txtDiscountPercentAct.Enabled = false;
                    txtActDiscountAmt.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                throw ex;
            }
        }

        protected void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {                
                string provinceValue = cmbProvince.SelectedItem.Value;

                cmbDistinct.DataSource = SlmScr999Biz.ListDistinct(provinceValue);
                cmbDistinct.DataTextField = "AmphurName";
                cmbDistinct.DataValueField = "AmphurId";
                cmbDistinct.DataBind();
                cmbDistinct.Items.Insert(0, new ListItem("", ""));

                string distinctValue = cmbDistinct.SelectedItem.Value;

                cmbThambol.DataSource = SlmScr999Biz.ListTambol(distinctValue, provinceValue);
                cmbThambol.DataTextField = "TambolNameTh";
                cmbThambol.DataValueField = "TambolId";
                cmbThambol.DataBind();
                cmbThambol.Items.Insert(0, new ListItem("", ""));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void cmbDistinct_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
                string distinctValue = cmbDistinct.SelectedItem.Value;
                string provinceValue = cmbProvince.SelectedItem.Value;

                cmbThambol.DataSource = SlmScr999Biz.ListTambol(distinctValue, provinceValue);
                cmbThambol.DataTextField = "TambolNameTh";
                cmbThambol.DataValueField = "TambolId";
                cmbThambol.DataBind();
                cmbThambol.Items.Insert(0, new ListItem("", ""));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void textPeriod_TextChanged(object sender, EventArgs e)
        {
            try
            {
                
                decimal PayAmt = 0m;
                if (AppUtil.SafeInt(textPeriod.Text) <= 10)
                {
                    if (rbInsNameTh_cur.Checked)
                    {
                        PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_cur.Text);
                    }
                    else if (rbInsNameTh_pro1.Checked)
                    {
                        PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro1.Text);
                    }
                    else if (rbInsNameTh_pro2.Checked)
                    {
                        PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro2.Text);
                    }
                    else if (rbInsNameTh_pro3.Checked)
                    {
                        PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro3.Text);
                    }

                    var sMonth = ((TextBox)sender).Text;

                    if (textPeriod.Text == "1")
                    {
                        txtPeriod1.Enabled = true;
                        txtPeriod1.Text = PayAmt.ToString("#,##0.00");
                        tdmPaymentDate1.Enabled = true;
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        trPday1.Visible = true;
                        for (int i = 2; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (textPeriod.Text == "2")
                    {
                        txtPeriod1.Enabled = true;
                        tdmPaymentDate1.Enabled = true;
                        txtPeriod1.Text = (PayAmt / 2m).ToString("#,##0.00");
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        txtPeriod2.Enabled = true;
                        txtPeriod2.Text = (PayAmt - Math.Round((PayAmt / 2m), 2, MidpointRounding.AwayFromZero)).ToString("#,##0.00");
                        tdmPaymentDate2.Enabled = true;
                        tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                        trPday1.Visible = true;
                        trPday2.Visible = true;

                        for (int i = 3; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (textPeriod.Text == "3")
                    {
                        txtPeriod1.Enabled = true;
                        tdmPaymentDate1.Enabled = true;
                        txtPeriod1.Text = (PayAmt / 3m).ToString("#,##0.00");
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        txtPeriod2.Enabled = true;
                        txtPeriod2.Text = (PayAmt / 3m).ToString("#,##0.00");
                        tdmPaymentDate2.Enabled = true;
                        tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                        txtPeriod3.Enabled = true;
                        txtPeriod3.Text = (PayAmt - Math.Round((PayAmt / 3m), 2, MidpointRounding.AwayFromZero) * 2m).ToString("#,##0.00");
                        tdmPaymentDate3.Enabled = true;
                        tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                        trPday1.Visible = true;
                        trPday2.Visible = true;
                        trPday3.Visible = true;

                        for (int i = 4; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (textPeriod.Text == "4")
                    {
                        txtPeriod1.Enabled = true;
                        tdmPaymentDate1.Enabled = true;
                        txtPeriod1.Text = (PayAmt / 4m).ToString("#,##0.00");
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        txtPeriod2.Enabled = true;
                        txtPeriod2.Text = (PayAmt / 4m).ToString("#,##0.00");
                        tdmPaymentDate2.Enabled = true;
                        tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                        txtPeriod3.Enabled = true;
                        txtPeriod3.Text = (PayAmt / 4m).ToString("#,##0.00");
                        tdmPaymentDate3.Enabled = true;
                        tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                        txtPeriod4.Enabled = true;
                        txtPeriod4.Text = (PayAmt - Math.Round((PayAmt / 4m), 2, MidpointRounding.AwayFromZero) * 3m).ToString("#,##0.00");
                        tdmPaymentDate4.Enabled = true;
                        tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                        trPday1.Visible = true;
                        trPday2.Visible = true;
                        trPday3.Visible = true;
                        trPday4.Visible = true;

                        for (int i = 5; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (textPeriod.Text == "5")
                    {
                        txtPeriod1.Enabled = true;
                        tdmPaymentDate1.Enabled = true;
                        txtPeriod1.Text = (PayAmt / 5m).ToString("#,##0.00");
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        txtPeriod2.Enabled = true;
                        txtPeriod2.Text = (PayAmt / 5m).ToString("#,##0.00");
                        tdmPaymentDate2.Enabled = true;
                        tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                        txtPeriod3.Enabled = true;
                        txtPeriod3.Text = (PayAmt / 5m).ToString("#,##0.00");
                        tdmPaymentDate3.Enabled = true;
                        tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                        txtPeriod4.Enabled = true;
                        txtPeriod4.Text = (PayAmt / 5m).ToString("#,##0.00");
                        tdmPaymentDate4.Enabled = true;
                        tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                        txtPeriod5.Enabled = true;
                        txtPeriod5.Text = (PayAmt - Math.Round((PayAmt / 5m), 2, MidpointRounding.AwayFromZero) * 4m).ToString("#,##0.00");
                        tdmPaymentDate5.Enabled = true;
                        tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                        trPday1.Visible = true;
                        trPday2.Visible = true;
                        trPday3.Visible = true;
                        trPday4.Visible = true;
                        trPday5.Visible = true;

                        for (int i = 6; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (textPeriod.Text == "6")
                    {
                        txtPeriod1.Enabled = true;
                        tdmPaymentDate1.Enabled = true;
                        txtPeriod1.Text = (PayAmt / 6m).ToString("#,##0.00");
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        txtPeriod2.Enabled = true;
                        txtPeriod2.Text = (PayAmt / 6m).ToString("#,##0.00");
                        tdmPaymentDate2.Enabled = true;
                        tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                        txtPeriod3.Enabled = true;
                        txtPeriod3.Text = (PayAmt / 6m).ToString("#,##0.00");
                        tdmPaymentDate3.Enabled = true;
                        tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                        txtPeriod4.Enabled = true;
                        txtPeriod4.Text = (PayAmt / 6m).ToString("#,##0.00");
                        tdmPaymentDate4.Enabled = true;
                        tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                        txtPeriod5.Enabled = true;
                        txtPeriod5.Text = (PayAmt / 6m).ToString("#,##0.00");
                        tdmPaymentDate5.Enabled = true;
                        tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                        txtPeriod6.Enabled = true;
                        txtPeriod6.Text = (PayAmt - Math.Round((PayAmt / 6m), 2, MidpointRounding.AwayFromZero) * 5m).ToString("#,##0.00");
                        tdmPaymentDate6.Enabled = true;
                        tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                        trPday1.Visible = true;
                        trPday2.Visible = true;
                        trPday3.Visible = true;
                        trPday4.Visible = true;
                        trPday5.Visible = true;
                        trPday6.Visible = true;

                        for (int i = 7; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (textPeriod.Text == "7")
                    {
                        txtPeriod1.Enabled = true;
                        tdmPaymentDate1.Enabled = true;
                        txtPeriod1.Text = (PayAmt / 7m).ToString("#,##0.00");
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        txtPeriod2.Enabled = true;
                        txtPeriod2.Text = (PayAmt / 7m).ToString("#,##0.00");
                        tdmPaymentDate2.Enabled = true;
                        tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                        txtPeriod3.Enabled = true;
                        txtPeriod3.Text = (PayAmt / 7m).ToString("#,##0.00");
                        tdmPaymentDate3.Enabled = true;
                        tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                        txtPeriod4.Enabled = true;
                        txtPeriod4.Text = (PayAmt / 7m).ToString("#,##0.00");
                        tdmPaymentDate4.Enabled = true;
                        tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                        txtPeriod5.Enabled = true;
                        txtPeriod5.Text = (PayAmt / 7m).ToString("#,##0.00");
                        tdmPaymentDate5.Enabled = true;
                        tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                        txtPeriod6.Enabled = true;
                        txtPeriod6.Text = (PayAmt / 7m).ToString("#,##0.00");
                        tdmPaymentDate6.Enabled = true;
                        tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                        txtPeriod7.Enabled = true;
                        txtPeriod7.Text = (PayAmt - Math.Round((PayAmt / 7m), 2, MidpointRounding.AwayFromZero) * 6m).ToString("#,##0.00");
                        tdmPaymentDate7.Enabled = true;
                        tdmPaymentDate7.DateValue = DateTime.Now.AddMonths(7);

                        trPday1.Visible = true;
                        trPday2.Visible = true;
                        trPday3.Visible = true;
                        trPday4.Visible = true;
                        trPday5.Visible = true;
                        trPday6.Visible = true;
                        trPday7.Visible = true;

                        for (int i = 8; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (textPeriod.Text == "8")
                    {
                        txtPeriod1.Enabled = true;
                        tdmPaymentDate1.Enabled = true;
                        txtPeriod1.Text = (PayAmt / 8m).ToString("#,##0.00");
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        txtPeriod2.Enabled = true;
                        txtPeriod2.Text = (PayAmt / 8m).ToString("#,##0.00");
                        tdmPaymentDate2.Enabled = true;
                        tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                        txtPeriod3.Enabled = true;
                        txtPeriod3.Text = (PayAmt / 8m).ToString("#,##0.00");
                        tdmPaymentDate3.Enabled = true;
                        tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                        txtPeriod4.Enabled = true;
                        txtPeriod4.Text = (PayAmt / 8m).ToString("#,##0.00");
                        tdmPaymentDate4.Enabled = true;
                        tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                        txtPeriod5.Enabled = true;
                        txtPeriod5.Text = (PayAmt / 8m).ToString("#,##0.00");
                        tdmPaymentDate5.Enabled = true;
                        tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                        txtPeriod6.Enabled = true;
                        txtPeriod6.Text = (PayAmt / 8m).ToString("#,##0.00");
                        tdmPaymentDate6.Enabled = true;
                        tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                        txtPeriod7.Enabled = true;
                        txtPeriod7.Text = (PayAmt / 8m).ToString("#,##0.00");
                        tdmPaymentDate7.Enabled = true;
                        tdmPaymentDate7.DateValue = DateTime.Now.AddMonths(7);

                        txtPeriod8.Enabled = true;
                        txtPeriod8.Text = (PayAmt - Math.Round((PayAmt / 8m), 2, MidpointRounding.AwayFromZero) * 7m).ToString("#,##0.00");
                        tdmPaymentDate8.Enabled = true;
                        tdmPaymentDate8.DateValue = DateTime.Now.AddMonths(8);

                        trPday1.Visible = true;
                        trPday2.Visible = true;
                        trPday3.Visible = true;
                        trPday4.Visible = true;
                        trPday5.Visible = true;
                        trPday6.Visible = true;
                        trPday7.Visible = true;
                        trPday8.Visible = true;

                        for (int i = 9; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (textPeriod.Text == "9")
                    {
                        txtPeriod1.Enabled = true;
                        tdmPaymentDate1.Enabled = true;
                        txtPeriod1.Text = (PayAmt / 9m).ToString("#,##0.00");
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        txtPeriod2.Enabled = true;
                        txtPeriod2.Text = (PayAmt / 9m).ToString("#,##0.00");
                        tdmPaymentDate2.Enabled = true;
                        tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                        txtPeriod3.Enabled = true;
                        txtPeriod3.Text = (PayAmt / 9m).ToString("#,##0.00");
                        tdmPaymentDate3.Enabled = true;
                        tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                        txtPeriod4.Enabled = true;
                        txtPeriod4.Text = (PayAmt / 9m).ToString("#,##0.00");
                        tdmPaymentDate4.Enabled = true;
                        tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                        txtPeriod5.Enabled = true;
                        txtPeriod5.Text = (PayAmt / 9m).ToString("#,##0.00");
                        tdmPaymentDate5.Enabled = true;
                        tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                        txtPeriod6.Enabled = true;
                        txtPeriod6.Text = (PayAmt / 9m).ToString("#,##0.00");
                        tdmPaymentDate6.Enabled = true;
                        tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                        txtPeriod7.Enabled = true;
                        txtPeriod7.Text = (PayAmt / 9m).ToString("#,##0.00");
                        tdmPaymentDate7.Enabled = true;
                        tdmPaymentDate7.DateValue = DateTime.Now.AddMonths(7);

                        txtPeriod8.Enabled = true;
                        txtPeriod8.Text = (PayAmt / 9m).ToString("#,##0.00");
                        tdmPaymentDate8.Enabled = true;
                        tdmPaymentDate8.DateValue = DateTime.Now.AddMonths(8);

                        txtPeriod9.Enabled = true;
                        txtPeriod9.Text = (PayAmt - Math.Round((PayAmt / 9m), 2, MidpointRounding.AwayFromZero) * 8m).ToString("#,##0.00");
                        tdmPaymentDate9.Enabled = true;
                        tdmPaymentDate9.DateValue = DateTime.Now.AddMonths(9);

                        trPday1.Visible = true;
                        trPday2.Visible = true;
                        trPday3.Visible = true;
                        trPday4.Visible = true;
                        trPday5.Visible = true;
                        trPday6.Visible = true;
                        trPday7.Visible = true;
                        trPday8.Visible = true;
                        trPday9.Visible = true;

                        for (int i = 10; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPday" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPeriod" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (textPeriod.Text == "10")
                    {
                        txtPeriod1.Enabled = true;
                        tdmPaymentDate1.Enabled = true;
                        txtPeriod1.Text = (PayAmt / 10m).ToString("#,##0.00");
                        tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                        txtPeriod2.Enabled = true;
                        txtPeriod2.Text = (PayAmt / 10m).ToString("#,##0.00");
                        tdmPaymentDate2.Enabled = true;
                        tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                        txtPeriod3.Enabled = true;
                        txtPeriod3.Text = (PayAmt / 10m).ToString("#,##0.00");
                        tdmPaymentDate3.Enabled = true;
                        tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                        txtPeriod4.Enabled = true;
                        txtPeriod4.Text = (PayAmt / 10m).ToString("#,##0.00");
                        tdmPaymentDate4.Enabled = true;
                        tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                        txtPeriod5.Enabled = true;
                        txtPeriod5.Text = (PayAmt / 10m).ToString("#,##0.00");
                        tdmPaymentDate5.Enabled = true;
                        tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                        txtPeriod6.Enabled = true;
                        txtPeriod6.Text = (PayAmt / 10m).ToString("#,##0.00");
                        tdmPaymentDate6.Enabled = true;
                        tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                        txtPeriod7.Enabled = true;
                        txtPeriod7.Text = (PayAmt / 10m).ToString("#,##0.00");
                        tdmPaymentDate7.Enabled = true;
                        tdmPaymentDate7.DateValue = DateTime.Now.AddMonths(7);

                        txtPeriod8.Enabled = true;
                        txtPeriod8.Text = (PayAmt / 10m).ToString("#,##0.00");
                        tdmPaymentDate8.Enabled = true;
                        tdmPaymentDate8.DateValue = DateTime.Now.AddMonths(8);

                        txtPeriod9.Enabled = true;
                        txtPeriod9.Text = (PayAmt / 10m).ToString("#,##0.00");
                        tdmPaymentDate9.Enabled = true;
                        tdmPaymentDate9.DateValue = DateTime.Now.AddMonths(9);

                        txtPeriod10.Enabled = true;
                        txtPeriod10.Text = (PayAmt - Math.Round((PayAmt / 10m), 2, MidpointRounding.AwayFromZero) * 9m).ToString("#,##0.00");
                        tdmPaymentDate10.Enabled = true;
                        tdmPaymentDate10.DateValue = DateTime.Now.AddMonths(10);

                        trPday1.Visible = true;
                        trPday2.Visible = true;
                        trPday3.Visible = true;
                        trPday4.Visible = true;
                        trPday5.Visible = true;
                        trPday6.Visible = true;
                        trPday7.Visible = true;
                        trPday8.Visible = true;
                        trPday9.Visible = true;
                        trPday10.Visible = true;
                    }
                }
                else
                {
                    //แก้ไข เมื่อใส่ค่าเกิน 10 ให้แสดงผลลัพธ์ 10 แถว
                    if (rbInsNameTh_cur.Checked)
                    {
                        PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_cur.Text);
                    }
                    else if (rbInsNameTh_pro1.Checked)
                    {
                        PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro1.Text);
                    }
                    else if (rbInsNameTh_pro2.Checked)
                    {
                        PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro2.Text);
                    }
                    else if (rbInsNameTh_pro3.Checked)
                    {
                        PayAmt = AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro3.Text);
                    }

                    textPeriod.Text = "10";
                    AppUtil.ClientAlert(Page, "เลือกผ่อนชำระได้ไม่เกิน 10 งวด");
                    txtPeriod1.Enabled = true;
                    tdmPaymentDate1.Enabled = true;
                    txtPeriod1.Text = (PayAmt / 10m).ToString("#,##0.00");
                    tdmPaymentDate1.DateValue = DateTime.Now.AddMonths(1);

                    txtPeriod2.Enabled = true;
                    txtPeriod2.Text = (PayAmt / 10m).ToString("#,##0.00");
                    tdmPaymentDate2.Enabled = true;
                    tdmPaymentDate2.DateValue = DateTime.Now.AddMonths(2);

                    txtPeriod3.Enabled = true;
                    txtPeriod3.Text = (PayAmt / 10m).ToString("#,##0.00");
                    tdmPaymentDate3.Enabled = true;
                    tdmPaymentDate3.DateValue = DateTime.Now.AddMonths(3);

                    txtPeriod4.Enabled = true;
                    txtPeriod4.Text = (PayAmt / 10m).ToString("#,##0.00");
                    tdmPaymentDate4.Enabled = true;
                    tdmPaymentDate4.DateValue = DateTime.Now.AddMonths(4);

                    txtPeriod5.Enabled = true;
                    txtPeriod5.Text = (PayAmt / 10m).ToString("#,##0.00");
                    tdmPaymentDate5.Enabled = true;
                    tdmPaymentDate5.DateValue = DateTime.Now.AddMonths(5);

                    txtPeriod6.Enabled = true;
                    txtPeriod6.Text = (PayAmt / 10m).ToString("#,##0.00");
                    tdmPaymentDate6.Enabled = true;
                    tdmPaymentDate6.DateValue = DateTime.Now.AddMonths(6);

                    txtPeriod7.Enabled = true;
                    txtPeriod7.Text = (PayAmt / 10m).ToString("#,##0.00");
                    tdmPaymentDate7.Enabled = true;
                    tdmPaymentDate7.DateValue = DateTime.Now.AddMonths(7);

                    txtPeriod8.Enabled = true;
                    txtPeriod8.Text = (PayAmt / 10m).ToString("#,##0.00");
                    tdmPaymentDate8.Enabled = true;
                    tdmPaymentDate8.DateValue = DateTime.Now.AddMonths(8);

                    txtPeriod9.Enabled = true;
                    txtPeriod9.Text = (PayAmt / 10m).ToString("#,##0.00");
                    tdmPaymentDate9.Enabled = true;
                    tdmPaymentDate9.DateValue = DateTime.Now.AddMonths(9);

                    txtPeriod10.Enabled = true;
                    txtPeriod10.Text = (PayAmt - Math.Round((PayAmt / 10m), 2, MidpointRounding.AwayFromZero) * 9m).ToString("#,##0.00");
                    tdmPaymentDate10.Enabled = true;
                    tdmPaymentDate10.DateValue = DateTime.Now.AddMonths(10);

                    trPday1.Visible = true;
                    trPday2.Visible = true;
                    trPday3.Visible = true;
                    trPday4.Visible = true;
                    trPday5.Visible = true;
                    trPday6.Visible = true;
                    trPday7.Visible = true;
                    trPday8.Visible = true;
                    trPday9.Visible = true;
                    trPday10.Visible = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void txtOD_TextChanged(object sender, EventArgs e)
        {
            try
            {              
                if (txtOD.Text == "")
                {
                    txtODRanking.Enabled = false;
                    txtODRanking.Text = "";
                }
                else
                {
                    txtODRanking.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void txtFT_TextChanged(object sender, EventArgs e)
        {
            try
            {               
                if (txtFT.Text == "")
                {
                    txtFTRanking.Enabled = false;
                    txtFTRanking.Text = "";
                }
                else
                {
                    txtFTRanking.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }


        protected void txtFTRanking_TextChanged(object sender, EventArgs e)
        {
            //if (txtFTRanking.Text != "" && AppUtil.SafeInt(txtFTRanking.Text) > 100) { 

            //}
        }

        protected void txtODRanking_TextChanged(object sender, EventArgs e)
        {
            //if (txtODRanking.Text != "" && AppUtil.SafeInt(txtODRanking.Text) > 100)
            //{

            //}
        }

        private decimal CalculatePolicyDuty(decimal netPremium)
        {
            //คำนวณอากรของเบี้ยประกัน
            try
            {
                //decimal retValue = 0;
                //string duty = (Math.Round((netPremium * constTax) / 100, 2)).ToString();
                //string[] temp = duty.Split('.');
                //if (temp[1] != null)
                //{
                //    string first = temp[1].Substring(0, 1);
                //    if (first != "" && first != "0")
                //    {
                //        retValue = Math.Ceiling((netPremium * 0.4m) / 100);
                //    }
                //    else
                //    {
                //        retValue = Math.Floor((netPremium * 0.4m) / 100);
                //    }
                //}

                //return retValue;

                return Math.Ceiling((netPremium * 0.4m) / 100);
            }
            catch
            {
                throw;
            }
        }

        private decimal CalculateActDuty(decimal netPremium)
        {
            //คำนวณอากรของเบี้ยประกัน
            try
            {
                //Comment on 2017-08-30
                //decimal retValue = 0;
                //string duty = (Math.Round((netPremium * constTax) / 100, 2)).ToString();
                //string[] temp = duty.Split('.');
                //if (temp[1] != null)
                //{
                //    string first = temp[1].Substring(0, 1);
                //    if (first != "" && first != "0")
                //    {
                //        retValue = Math.Ceiling((netPremium * 0.4m) / 100);
                //    }
                //    else
                //    {
                //        retValue = Math.Floor((netPremium * 0.4m) / 100);
                //    }
                //}

                //return retValue;

                return Math.Ceiling((netPremium * 0.4m) / 100);
            }
            catch
            {
                throw;
            }
        }

        protected void txtNetpremium_cur_OnTextChanged(object sender, EventArgs e)
        {
            //อากร = ((เบี้ยสุทธิ * 0.4)/100) ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม 
            //ภาษี = (เบี้ยสุทธิ+อากร)*7)/100 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            //เบี้ยประกันรวมภาษีอากร = เบี้ยสุทธิ +ภาษี +อากร ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 
            try
            {
                
                decimal Netpremium = AppUtil.SafeDecimal(lblNetpremium_cur.Text);
                //decimal Duty = Math.Ceiling((Netpremium * 0.4m) / 100m);
                decimal Duty = CalculatePolicyDuty(Netpremium);
                decimal Vat = Math.Round(((Netpremium + Duty) * constVat) / 100m, 2);
                decimal Gross_Premium = Math.Round(Netpremium + Duty + Vat, 2);

                lblDuty_cur.Text = Duty.ToString("#,##0.00");
                lblVat_amount_cur.Text = Vat.ToString("#,##0.00");
                lblVoluntary_Gross_Premium_cur.Text = Gross_Premium.ToString("#,##0.00");
                lblNetpremium_cur.Text = Netpremium.ToString("#,##0.00");
                calAll(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }

        protected void txtNetpremium_pro1_OnTextChanged(object sender, EventArgs e)
        {
            //อากร = ((เบี้ยสุทธิ * 0.4)/100) ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม 
            //ภาษี = (เบี้ยสุทธิ+อากร)*7)/100 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            //เบี้ยประกันรวมภาษีอากร = เบี้ยสุทธิ +ภาษี +อากร ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 
            try
            {
                
                decimal Netpremium = AppUtil.SafeDecimal(lblNetpremium_pro1.Text);
                //decimal Duty = Math.Ceiling((Netpremium * 0.4m) / 100m);
                decimal Duty = CalculatePolicyDuty(Netpremium);
                decimal Vat = Math.Round(((Netpremium + Duty) * constVat) / 100m, 2);
                decimal Gross_Premium = Math.Round(Netpremium + Duty + Vat, 2);

                lblDuty_pro1.Text = Duty.ToString("#,##0.00");
                lblVat_amount_pro1.Text = Vat.ToString("#,##0.00");
                lblVoluntary_Gross_Premium_pro1.Text = Gross_Premium.ToString("#,##0.00");
                lblNetpremium_pro1.Text = Netpremium.ToString("#,##0.00");
                calAll(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void txtNetpremium_pro2_OnTextChanged(object sender, EventArgs e)
        {
            //อากร = ((เบี้ยสุทธิ * 0.4)/100) ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม 
            //ภาษี = (เบี้ยสุทธิ+อากร)*7)/100 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            //เบี้ยประกันรวมภาษีอากร = เบี้ยสุทธิ +ภาษี +อากร ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 
            try
            {
                decimal Netpremium = AppUtil.SafeDecimal(lblNetpremium_pro2.Text);
                //decimal Duty = Math.Ceiling((Netpremium * 0.4m) / 100m);
                decimal Duty = CalculatePolicyDuty(Netpremium);
                decimal Vat = Math.Round(((Netpremium + Duty) * constVat) / 100m, 2);
                decimal Gross_Premium = Math.Round(Netpremium + Duty + Vat, 2);

                lblDuty_pro2.Text = Duty.ToString("#,##0.00");
                lblVat_amount_pro2.Text = Vat.ToString("#,##0.00");
                lblVoluntary_Gross_Premium_pro2.Text = Gross_Premium.ToString("#,##0.00");
                lblNetpremium_pro2.Text = Netpremium.ToString("#,##0.00");
                calAll(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void txtNetpremium_pro3_OnTextChanged(object sender, EventArgs e)
        {
            //อากร = ((เบี้ยสุทธิ * 0.4)/100) ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม 
            //ภาษี = (เบี้ยสุทธิ+อากร)*7)/100 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            //เบี้ยประกันรวมภาษีอากร = เบี้ยสุทธิ +ภาษี +อากร ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 
            try
            {
                
                decimal Netpremium = AppUtil.SafeDecimal(lblNetpremium_pro3.Text);
                //decimal Duty = Math.Ceiling((Netpremium * 0.4m) / 100m);
                decimal Duty = CalculatePolicyDuty(Netpremium);
                decimal Vat = Math.Round(((Netpremium + Duty) * constVat) / 100m, 2);
                decimal Gross_Premium = Math.Round(Netpremium + Duty + Vat, 2);

                lblDuty_pro3.Text = Duty.ToString("#,##0.00");
                lblVat_amount_pro3.Text = Vat.ToString("#,##0.00");
                lblVoluntary_Gross_Premium_pro3.Text = Gross_Premium.ToString("#,##0.00");
                lblNetpremium_pro3.Text = Netpremium.ToString("#,##0.00");
                calAll(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void txtVoluntary_Gross_Premium_cur_OnTextChanged(object sender, EventArgs e)
        {

            //เบี้ยประกันรวมภาษีอากร ต้องไม่เป็น 0
            //เบี้ยสุทธิ = (เบี้ยประกันรวมภาษีอากร/1.07428)	 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            //อากร = ((เบี้ยสุทธิ * 0.4)/100) ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
            //ภาษี =  ((เบี้ยสุทธิ +อากร)*7)/100 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            try
            {
               
                decimal Gross_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_cur.Text);
                decimal NetPremium = Math.Round(Gross_Premium / 1.07428m, 2);
                //decimal Duty = Math.Ceiling((NetPremium * 0.4m) / 100m);
                decimal Duty = CalculatePolicyDuty(NetPremium);
                decimal Vat = Math.Round(((NetPremium + Duty) * constVat) / 100m, 2);


                lblDuty_cur.Text = Duty.ToString("#,##0.00");
                lblVat_amount_cur.Text = Vat.ToString("#,##0.00");
                lblNetpremium_cur.Text = NetPremium.ToString("#,##0.00");
                lblVoluntary_Gross_Premium_cur.Text = Gross_Premium.ToString("#,##0.00");
                calAll(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }

        protected void txtVoluntary_Gross_Premium_pro1_OnTextChanged(object sender, EventArgs e)
        {

            //เบี้ยประกันรวมภาษีอากร ต้องไม่เป็น 0
            //เบี้ยสุทธิ = (เบี้ยประกันรวมภาษีอากร/1.07428)	 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            //อากร = ((เบี้ยสุทธิ * 0.4)/100) ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
            //ภาษี =  ((เบี้ยสุทธิ +อากร)*7)/100 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            try
            {
               
                decimal Gross_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro1.Text);
                decimal NetPremium = Math.Round(Gross_Premium / 1.07428m, 3);
                //decimal Duty = Math.Ceiling((NetPremium * 0.4m) / 100m);
                decimal Duty = CalculatePolicyDuty(NetPremium);
                decimal Vat = Math.Round(((NetPremium + Duty) * constVat) / 100m, 2);


                lblDuty_pro1.Text = Duty.ToString("#,##0.00");
                lblVat_amount_pro1.Text = Vat.ToString("#,##0.00");
                lblNetpremium_pro1.Text = NetPremium.ToString("#,##0.00");
                lblVoluntary_Gross_Premium_pro1.Text = Gross_Premium.ToString("#,##0.00");
                calAll(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void txtVoluntary_Gross_Premium_pro2_OnTextChanged(object sender, EventArgs e)
        {

            //เบี้ยประกันรวมภาษีอากร ต้องไม่เป็น 0
            //เบี้ยสุทธิ = (เบี้ยประกันรวมภาษีอากร/1.07428)	 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            //อากร = ((เบี้ยสุทธิ * 0.4)/100) ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
            //ภาษี =  ((เบี้ยสุทธิ +อากร)*7)/100 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            try
            {
                
                decimal Gross_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro2.Text);
                decimal NetPremium = Math.Round(Gross_Premium / 1.07428m, 2);
                decimal Duty = CalculatePolicyDuty(NetPremium);
                decimal Vat = Math.Round(((NetPremium + Duty) * constVat) / 100m, 2);


                lblDuty_pro2.Text = Duty.ToString("#,##0.00");
                lblVat_amount_pro2.Text = Vat.ToString("#,##0.00");
                lblNetpremium_pro2.Text = NetPremium.ToString("#,##0.00");
                lblVoluntary_Gross_Premium_pro2.Text = Gross_Premium.ToString("#,##0.00");
                calAll(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void txtVoluntary_Gross_Premium_pro3_OnTextChanged(object sender, EventArgs e)
        {

            //เบี้ยประกันรวมภาษีอากร ต้องไม่เป็น 0
            //เบี้ยสุทธิ = (เบี้ยประกันรวมภาษีอากร/1.07428)	 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            //อากร = ((เบี้ยสุทธิ * 0.4)/100) ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
            //ภาษี =  ((เบี้ยสุทธิ +อากร)*7)/100 ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
            try
            {
                
                decimal Gross_Premium = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro3.Text);
                decimal NetPremium = Math.Round(Gross_Premium / 1.07428m, 2);
                //decimal Duty = Math.Ceiling((NetPremium * 0.4m) / 100m);
                decimal Duty = CalculatePolicyDuty(NetPremium);
                decimal Vat = Math.Round(((NetPremium + Duty) * constVat) / 100m, 2);


                lblDuty_pro3.Text = Duty.ToString("#,##0.00");
                lblVat_amount_pro3.Text = Vat.ToString("#,##0.00");
                lblNetpremium_pro3.Text = NetPremium.ToString("#,##0.00");
                lblVoluntary_Gross_Premium_pro3.Text = Gross_Premium.ToString("#,##0.00");
                calAll(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }

            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void txtActNetGrossPremiumFullAct_pro1_OnTextChanged(object sender, EventArgs e)
        {
            try
            {                
                CalActPro1(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void txtActNetGrossPremiumFullAct_pro2_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                CalActPro2(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void txtActNetGrossPremiumFullAct_pro3_OnTextChanged(object sender, EventArgs e)
        {
            try
            {              
                CalActPro3(false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool checkreport(object sender)
        {
            chkBP.Checked = false;

            if (cmbPaymentTypeAct.SelectedValue == "1" && (rbAct_pro1.Checked || rbAct_pro2.Checked || rbAct_pro3.Checked))
            {
                chkBP.Visible = true;
                SetPopupBPReport();
                upPopupReport.Update();
                mpePopupReport.Show();
                return false;
            }
            else if (cmbPaymentType.SelectedValue == "1" && (rbInsNameTh_cur.Checked || rbInsNameTh_pro1.Checked || rbInsNameTh_pro2.Checked || rbInsNameTh_pro3.Checked))
            {
                chkBP.Visible = true;
                SetPopupBPReport();
                upPopupReport.Update();
                mpePopupReport.Show();
                return false;
            }
            else
            {
                chkBP.Visible = false;
                return true;
            }
        }

        private void SetPopupBPReport()
        {
            ClearBPReportPopup();
            trBPPopup_PolicyAmountDue.Visible = false;
            trBPPopup_PolicyAmountSpecify1.Visible = false;
            trBPPopup_PolicyAmountSpecify2.Visible = false;
            trBPPopup_PolicyAmountSpecify3.Visible = false;
            trBPPopup_PolicyAmountSpecify4.Visible = false;
            trBPPopup_PolicyAmountSpecify5.Visible = false;
            trBPPopup_PolicyAmountSpecify6.Visible = false;
            trBPPopup_PolicyAmountSpecify7.Visible = false;
            trBPPopup_PolicyAmountSpecify8.Visible = false;
            trBPPopup_PolicyAmountSpecify9.Visible = false;
            trBPPopup_PolicyAmountSpecify10.Visible = false;

            trBPPopup_ActAmountDue.Visible = false;
            trBPPopup_ActAmountSpecify.Visible = false;

            //Fill in ประกัน, รูปแบบการชำระ = Bill Payment
            if (cmbPaymentType.SelectedValue == "1")
            {
                lblBPPopup_PolicyAmountSpecify1.Text = "จำนวนเงิน";
                trBPPopup_PolicyAmountDue.Visible = true;
                trBPPopup_PolicyAmountSpecify1.Visible = true;
                if (rbInsNameTh_cur.Checked)
                {
                    txtBPPopup_PolicyAmountDue.Text = txtTotal_Voluntary_Gross_Premium_cur.Text.Trim();
                }
                else if (rbInsNameTh_pro1.Checked)
                {
                    txtBPPopup_PolicyAmountDue.Text = txtTotal_Voluntary_Gross_Premium_pro1.Text.Trim();
                }
                else if (rbInsNameTh_pro2.Checked)
                {
                    txtBPPopup_PolicyAmountDue.Text = txtTotal_Voluntary_Gross_Premium_pro2.Text.Trim();
                }
                else if (rbInsNameTh_pro3.Checked)
                {
                    txtBPPopup_PolicyAmountDue.Text = txtTotal_Voluntary_Gross_Premium_pro3.Text.Trim();
                }

                //ถ้าจ่ายประกันแบบผ่อน
                if (cmbPaymentmethod.SelectedItem.Value == "2")
                {
                    lblBPPopup_PolicyAmountSpecify1.Text = "จำนวนเงินงวดที่ 1";
                    trBPPopup_PolicyAmountSpecify1.Visible = txtPeriod1.Visible;
                    trBPPopup_PolicyAmountSpecify2.Visible = txtPeriod2.Visible;
                    trBPPopup_PolicyAmountSpecify3.Visible = txtPeriod3.Visible;
                    trBPPopup_PolicyAmountSpecify4.Visible = txtPeriod4.Visible;
                    trBPPopup_PolicyAmountSpecify5.Visible = txtPeriod5.Visible;
                    trBPPopup_PolicyAmountSpecify6.Visible = txtPeriod6.Visible;
                    trBPPopup_PolicyAmountSpecify7.Visible = txtPeriod7.Visible;
                    trBPPopup_PolicyAmountSpecify8.Visible = txtPeriod8.Visible;
                    trBPPopup_PolicyAmountSpecify9.Visible = txtPeriod9.Visible;
                    trBPPopup_PolicyAmountSpecify10.Visible = txtPeriod10.Visible;

                    chkBP.Enabled = textPeriod.Text.Trim() != "";
                }
            }

            //Fill in พรบ., รูปแบบการชำระ = Bill Payment
            if (cmbPaymentTypeAct.SelectedValue == "1")
            {
                trBPPopup_ActAmountDue.Visible = true;
                trBPPopup_ActAmountSpecify.Visible = true;
                if (rbAct_pro1.Checked)
                {
                    txtBPPopup_ActAmountDue.Text = txtDiscountBathAct_pro1.Text.Trim();
                }
                else if (rbAct_pro2.Checked)
                {
                    txtBPPopup_ActAmountDue.Text = txtDiscountBathAct_pro2.Text.Trim();
                }
                else if (rbAct_pro3.Checked)
                {
                    txtBPPopup_ActAmountDue.Text = txtDiscountBathAct_pro3.Text.Trim();
                }
            }
            
        }

        protected void btnCloseReport_OnClick(object sender, EventArgs e)
        {
            ClearBPReportPopup();
            mpePopupReport.Hide();
        }

        private void ClearBPReportPopup()
        {
            chkBP.Checked = false;
            chkBP.Enabled = true;
            cbBPPopup_PolicyAmountSpecify.Checked = false;
            cbBPPopup_PolicyAmountSpecify.Enabled = false;
            txtBPPopup_PolicyAmountSpecify1.Text = "";
            txtBPPopup_PolicyAmountSpecify1.Enabled = false;
            txtBPPopup_PolicyAmountSpecify2.Text = "";
            txtBPPopup_PolicyAmountSpecify2.Enabled = false;
            txtBPPopup_PolicyAmountSpecify3.Text = "";
            txtBPPopup_PolicyAmountSpecify3.Enabled = false;
            txtBPPopup_PolicyAmountSpecify4.Text = "";
            txtBPPopup_PolicyAmountSpecify4.Enabled = false;
            txtBPPopup_PolicyAmountSpecify5.Text = "";
            txtBPPopup_PolicyAmountSpecify5.Enabled = false;
            txtBPPopup_PolicyAmountSpecify6.Text = "";
            txtBPPopup_PolicyAmountSpecify6.Enabled = false;
            txtBPPopup_PolicyAmountSpecify7.Text = "";
            txtBPPopup_PolicyAmountSpecify7.Enabled = false;
            txtBPPopup_PolicyAmountSpecify8.Text = "";
            txtBPPopup_PolicyAmountSpecify8.Enabled = false;
            txtBPPopup_PolicyAmountSpecify9.Text = "";
            txtBPPopup_PolicyAmountSpecify9.Enabled = false;
            txtBPPopup_PolicyAmountSpecify10.Text = "";
            txtBPPopup_PolicyAmountSpecify10.Enabled = false;

            cbBPPopup_ActAmountSpecify.Checked = false;
            cbBPPopup_ActAmountSpecify.Enabled = false;
            txtBPPopup_ActAmountSpecify.Text = "";
            txtBPPopup_ActAmountSpecify.Enabled = false;
        }

        protected void btnSave_PreleadData_Click(object sender, EventArgs e)
        {
            if (TicketId != "")
            {
                if (!checkreport(sender))
                {
                    return;
                }
                else
                {
                    SaveAll();
                }
            }
            else
            {
                SaveAll();
            }
        }

        protected void btnSaveAll_Click(object sender, EventArgs e)
        {
            try
            {
                SaveAll();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool SaveAll(bool showalert = true, bool fromReceiveClick = false)
        {
            bool saveSuccess = false;
            string selAct = "";
            string selPolicy = "";
            decimal policyPromoId = 0;
            decimal actPromoId = 0;
            if (rbAct_pro1.Checked)
            {
                selAct = "act1";
                actPromoId = AppUtil.SafeDecimal(hidPromotionActId1.Value);
            }
            else if (rbAct_pro2.Checked)
            {
                selAct = "act2";
                actPromoId = AppUtil.SafeDecimal(hidPromotionActId2.Value);
            }
            else if (rbAct_pro3.Checked)
            {
                selAct = "act3";
                actPromoId = AppUtil.SafeDecimal(hidPromotionActId3.Value);
            }

            if (rbInsNameTh_cur.Checked)
            {
                selPolicy = "cur";
            }
            else if (rbInsNameTh_pro1.Checked)
            {
                selPolicy = "pro1";
                policyPromoId = AppUtil.SafeDecimal(hidPromotionId1.Value);
            }
            else if (rbInsNameTh_pro2.Checked)
            {
                selPolicy = "pro2";
                policyPromoId = AppUtil.SafeDecimal(hidPromotionId2.Value);
            }
            else if (rbInsNameTh_pro3.Checked)
            {
                selPolicy = "pro3";
                policyPromoId = AppUtil.SafeDecimal(hidPromotionId3.Value);
            }

            try
            {
                if (!ValidateCarDetail())
                {
                    return false;
                }

                if (CheckPolicy(selPolicy) && CheckAct(selAct))
                {
                    var ticketOrPreleadID = TicketId == "" ? PreLeadId : TicketId;
                    if (CheckDiscount(selPolicy, selAct, ticketOrPreleadID))
                    {
                        PreleadCompareDataCollection preleaddatacollectionsave = new PreleadCompareDataCollection();
                        PreleadCompareDataCollectionGroup pregroup = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                        string ActiveTab_ID;
                        if (tabRenewInsuranceContainer.ActiveTab != null)
                        {
                            ActiveTab_ID = tabRenewInsuranceContainer.ActiveTab.ID;
                        }
                        else
                        {
                            SessionExpired();
                            return false;
                        }

                        if (pregroup.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                        {
                            preleaddatacollectionsave = pregroup.PreleadCompareDataCollectionMain;
                        }
                        else
                        {
                            foreach (PreleadCompareDataCollection pc in pregroup.PreleadCompareDataCollections)
                            {
                                if (pc.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                                {
                                    preleaddatacollectionsave = pc;
                                    break;
                                }
                            }
                        }

                        preleaddatacollectionsave = PrepareForSavePreLead(preleaddatacollectionsave);
                        preleaddatacollectionsave = PrepareForSaveCompare(preleaddatacollectionsave);
                        preleaddatacollectionsave = PrepareForSaveAct(preleaddatacollectionsave);
                        preleaddatacollectionsave = PrepareForSaveAddress(preleaddatacollectionsave);
                        /* Check Receip */
                        if (txtReceiveNo.Text != null || txtReceiveNo.Text != "") preleaddatacollectionsave.ReceipNo = txtReceiveNo.Text;
                        if (txtActSendDate.Text != null || txtActSendDate.Text != null) preleaddatacollectionsave.StrReceipActDate = txtActSendDate.Text;

                        if (PreLeadId != "")
                        {
                            ActivityPreLeadBiz.SaveData(preleaddatacollectionsave);
                        }
                        else if (TicketId != "")
                        {
                            string errorMessage = "";
                            if (!CheckActPurchaseDateTimeWithActIssueReport(preleaddatacollectionsave.RenewIns.slm_TicketId, out errorMessage))
                            {
                                AppUtil.ClientAlertTabRenew(Page, errorMessage);
                                return false;
                            }

                            //lead     
                            SavePremium(preleaddatacollectionsave.RenewIns.slm_TicketId);

                            preleaddatacollectionsave = PrepareForSaveTransMain(preleaddatacollectionsave);
                            preleaddatacollectionsave = PrepareForSaveTrans(preleaddatacollectionsave);

                            bool PolicyPaymentMainFlag = false;
                            bool PolicyPaymentMainActFlag = false;

                            preleaddatacollectionsave = PrepareForSaveRenewInsurance(preleaddatacollectionsave);
                            preleaddatacollectionsave = PrepareForSavePaymentMain(preleaddatacollectionsave);
                            preleaddatacollectionsave = PrepareProblemData(preleaddatacollectionsave);
                            preleaddatacollectionsave = PrepareForSaveReceipt(preleaddatacollectionsave);

                            string claimFlag = radioClaim.SelectedValue;

                            ActivityLeadBiz acbiz = new ActivityLeadBiz();
                            acbiz.SaveData(preleaddatacollectionsave, PolicyPaymentMainFlag, PolicyPaymentMainActFlag, GetDataForBPReport(), claimFlag, Page.User.Identity.Name, fromReceiveClick);
                            SettleClaimReportId = acbiz.SettleClaimReportId;                           
                        }

                        if (showalert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "บันทึกสำเร็จ");
                        }

                        bindDropDownListCompany();

                        //เมื่อทำการ save ให้ bind สาขาใหม่ โดยนำสาขาที่เป็น isdelete ออก ถ้าเปลี่ยนสาขาไม่ใช่สาขาเดิม 20170508
                        var activeBranchList = SlmScr999Biz.ListBranchName();
                        var branchCode = cmbActIssueBranch_pro1.SelectedValue;

                        if (!string.IsNullOrEmpty(branchCode))
                        {
                            if (!SlmScr999Biz.GetIsActiveBranch(branchCode))
                            {
                                cmbActIssueBranch_pro1.DataSource = activeBranchList;
                                cmbActIssueBranch_pro1.DataTextField = "BranchName";
                                cmbActIssueBranch_pro1.DataValueField = "BranchCode";
                                cmbActIssueBranch_pro1.DataBind();
                                cmbActIssueBranch_pro1.Items.Insert(0, new ListItem("", ""));
                            }
                        }
                        
                        bindControl(preleaddatacollectionsave, true, true);
                        UpdateDiscountPaymentSection();     //Add by Pom 29/07/2016

                        //bool isPaidPolicy = ActivityLeadBiz.checkPaidPolicy(preleaddatacollectionsave.RenewIns.slm_TicketId);
                        if (PolicyCheckPaid == null)
                        {
                            PolicyCheckPaid = ActivityLeadBiz.checkPaidPolicy(preleaddatacollectionsave.RenewIns.slm_TicketId);
                        }
                        if (PolicyCheckPaid == true && TicketId != ""
                            && (preleaddatacollectionsave.RenewIns.slm_ActIncentiveFlag ?? false == true))
                        {
                            UpdateStatusDesc("", preleaddatacollectionsave.RenewIns.slm_UpdatedDate ?? DateTime.Now);
                        }

                        saveSuccess = true;
                    }
                    else
                    {
                        PermissionDiscount = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                _log.Debug(ex);

                AppUtil.ClientAlertTabRenew(Page, "บันทึกไม่สำเร็จ\r\n" + message);
                saveSuccess = true;
            }

            return saveSuccess;
        }

        private BPReportData GetDataForBPReport()
        {
            try
            {
                var bpReport = new BPReportData();
                bpReport.SaveBPReport = chkBP.Checked;
                if (bpReport.SaveBPReport)
                {
                    bpReport.PolicyAmountDue = string.IsNullOrEmpty(txtBPPopup_PolicyAmountDue.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountDue.Text.Trim().Replace(",", ""));
                    bpReport.ActAmountDue = string.IsNullOrEmpty(txtBPPopup_ActAmountDue.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_ActAmountDue.Text.Trim().Replace(",", ""));

                    //Policy
                    bpReport.IssuePolicy = cbBPPopup_PolicyAmountSpecify.Checked;
                    if (cbBPPopup_PolicyAmountSpecify.Checked)
                    {
                        if (!string.IsNullOrWhiteSpace(cmbPaymentmethod.SelectedItem.Value))
                        {
                            bpReport.PolicyPaymentMethodId = int.Parse(cmbPaymentmethod.SelectedItem.Value);
                        }
                        
                        var policyItemList = new List<BPItem>();
                        if (txtBPPopup_PolicyAmountSpecify1.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 1, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify1.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify1.Text.Trim().Replace(",", "")) });
                        }
                        if (txtBPPopup_PolicyAmountSpecify2.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 2, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify2.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify2.Text.Trim().Replace(",", "")) });
                        }
                        if (txtBPPopup_PolicyAmountSpecify3.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 3, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify3.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify3.Text.Trim().Replace(",", "")) });
                        }
                        if (txtBPPopup_PolicyAmountSpecify4.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 4, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify4.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify4.Text.Trim().Replace(",", "")) });
                        }
                        if (txtBPPopup_PolicyAmountSpecify5.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 5, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify5.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify5.Text.Trim().Replace(",", "")) });
                        }
                        if (txtBPPopup_PolicyAmountSpecify6.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 6, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify6.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify6.Text.Trim().Replace(",", "")) });
                        }
                        if (txtBPPopup_PolicyAmountSpecify7.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 7, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify7.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify7.Text.Trim().Replace(",", "")) });
                        }
                        if (txtBPPopup_PolicyAmountSpecify8.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 8, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify8.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify8.Text.Trim().Replace(",", "")) });
                        }
                        if (txtBPPopup_PolicyAmountSpecify9.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 9, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify9.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify9.Text.Trim().Replace(",", "")) });
                        }
                        if (txtBPPopup_PolicyAmountSpecify10.Visible)
                        {
                            policyItemList.Add(new BPItem { No = 10, Amount = string.IsNullOrEmpty(txtBPPopup_PolicyAmountSpecify10.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_PolicyAmountSpecify10.Text.Trim().Replace(",", "")) });
                        }
                        bpReport.PolicyItemList = policyItemList;
                    }

                    //Act
                    bpReport.IssueAct = cbBPPopup_ActAmountSpecify.Checked;
                    if (cbBPPopup_ActAmountSpecify.Checked)
                    {
                        if (!string.IsNullOrWhiteSpace(cmbPaymentmethodAct.SelectedItem.Value))
                        {
                            bpReport.ActPaymentMethodId = int.Parse(cmbPaymentmethodAct.SelectedItem.Value);
                        }
                        bpReport.ActItem = new BPItem { No = 1, Amount = string.IsNullOrEmpty(txtBPPopup_ActAmountSpecify.Text.Trim()) ? 0 : decimal.Parse(txtBPPopup_ActAmountSpecify.Text.Trim().Replace(",", "")) };
                    }
                }

                return bpReport;
            }
            catch
            {
                ClearBPReportPopup();
                throw;
            }
        }

        private bool CheckActPurchaseDateTimeWithActIssueReport(string ticketId, out string errorMessage)
        {
            errorMessage = "";
            DateTime startCoverDate = new DateTime();
            if (rbAct_pro1.Checked)
            {
                startCoverDate = txtActStartCoverDateAct_pro1.DateValue;
            }
            else if (rbAct_pro2.Checked)
            {
                startCoverDate = txtActStartCoverDateAct_pro2.DateValue;
            }
            else if (rbAct_pro3.Checked)
            {
                startCoverDate = txtActStartCoverDateAct_pro3.DateValue;
            }
            else
            {
                return true;
            }

            ActivityLeadBiz biz = new ActivityLeadBiz();
            return biz.CheckActPurchaseDateTimeWithActIssueReport(ticketId, startCoverDate, out errorMessage);
        }

        private bool ValidateAllData()
        {

            if (chkPaymentDesc5.Checked && txtPaymentDesc5.Text == "" || txtPaymentDesc5.Text == "0.00")
            {
                AppUtil.ClientAlertTabRenew(Page, "กรุณาระบุจำนวนเงินของ ค่าเบี้ยประกันภัยปีต่ออายุ ของเลขที่ใบเสร็จ");
                return false;
            }
            if (chkPaymentDesc5.Checked && txtPaymentDesc5.Text == "" || txtPaymentDesc5.Text == "0.00")
            {
                AppUtil.ClientAlertTabRenew(Page, "กรุณาระบุจำนวนเงินของ ค่าเบี้ยพรบ. ปีต่ออายุ ของเลขที่ใบเสร็จ");
                return false;
            }
            if (chkPaymentDesc5.Checked && txtPaymentDesc5.Text == "" || txtPaymentDesc5.Text == "0.00")
            {
                AppUtil.ClientAlertTabRenew(Page, "กรุณาระบุจำนวนเงินของ ค่างวดรถยนต์  ของเลขที่ใบเสร็จ");
                return false;
            }
            if (chkPaymentDesc5.Checked && txtPaymentDesc5.Text == "" || txtPaymentDesc5.Text == "0.00")
            {
                AppUtil.ClientAlertTabRenew(Page, "กรุณาระบุจำนวนเงินของ ค่าต่อภาษีรถยนต์  ของเลขที่ใบเสร็จ");
                return false;
            }
            if (chkPaymentDesc5.Checked && txtPaymentDesc5.Text == "" || txtPaymentDesc5.Text == "0.00")
            {
                AppUtil.ClientAlertTabRenew(Page, "กรุณาระบุจำนวนเงินของ เจ้าหนี้อื่นๆ (ประกัน) ของเลขที่ใบเสร็จ");
                return false;
            }
            if (chkPaymentDesc5.Checked && txtPaymentDesc5.Text == "" || txtPaymentDesc5.Text == "0.00")
            {
                AppUtil.ClientAlertTabRenew(Page, "กรุณาระบุจำนวนเงินของ อื่นๆ ของเลขที่ใบเสร็จ");
                return false;
            }

            if (AppUtil.SafeDecimal(hidRenewInsuranceReceiptAmount.Value) < AppUtil.SafeDecimal(txtPaymentDescTotal.Text))
            {
                AppUtil.ClientAlertTabRenew(Page, "กรุณาระบุจำนวนเงินรวมทั้งสิ้น ไม่เกิน " + AppUtil.SafeDecimal(hidRenewInsuranceReceiptAmount.Value).ToString("#,##0.00") + "");
                return false;
            }
            return true;
        }

        protected void btnCalAct_Click(object sender, EventArgs e)
        {
            //CalAct();
        }       

        private bool CalActPro1(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro1.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro1.Text);
                // เบี้ยสุทธิรวมภาษี
                decimal netgross_premium_total = AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro1.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro1.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro1.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                            txtActEndCoverDateAct_pro1.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActStartCoverDateAct_pro1.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActEndCoverDateAct_pro1.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro1.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro1.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro1.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro1.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {

                            tax1percent = 0;
                        }

                        txtDiscountBathAct_pro1.Text = (NetGrossPremium - tax1percent).ToString("#,##0.00");

                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro1.Text);

                        lblActPersonType_pro1.Text = tax1percent.ToString("#,##0.00");
                        if (NetGrossPremium != 0)
                        {
                            lblVat1PercentBathAct_pro1.Text = ((NetGrossPremium - total_premium - tax1percent) / NetGrossPremium * 100).ToString("#,##0");
                        }

                        lblDiscountPercentAct_pro1.Text = (NetGrossPremium - total_premium - tax1percent).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (alert)
                {
                    AppUtil.ClientAlertTabRenew(this, ex.Message);
                }
                return false;
            }
        }

        private bool CalActPro2(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro2.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro2.Text);
                // เบี้ยสุทธิรวมภาษี
                decimal netgross_premium_total = AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro2.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro2.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro2.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                            txtActEndCoverDateAct_pro2.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActStartCoverDateAct_pro2.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActEndCoverDateAct_pro2.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro2.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro2.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro2.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro2.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {

                            tax1percent = 0;
                        }

                        txtDiscountBathAct_pro2.Text = (NetGrossPremium - tax1percent).ToString("#,##0.00");

                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro2.Text);
                        lblActPersonType_pro2.Text = tax1percent.ToString("#,##0.00");
                        if (NetGrossPremium != 0)
                        {
                            lblVat1PercentBathAct_pro2.Text = ((NetGrossPremium - total_premium - tax1percent) / NetGrossPremium * 100).ToString("#,##0");
                        }

                        lblDiscountPercentAct_pro2.Text = (NetGrossPremium - total_premium - tax1percent).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlertTabRenew(this, ex.Message);
                return false;
            }
        }

        private bool CalActPro3(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro3.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro3.Text);
                // เบี้ยสุทธิรวมภาษี
                decimal netgross_premium_total = AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro3.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro3.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro3.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                            txtActEndCoverDateAct_pro3.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActStartCoverDateAct_pro3.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActEndCoverDateAct_pro3.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro3.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro3.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro3.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro3.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {
                            tax1percent = 0;
                        }

                        txtDiscountBathAct_pro3.Text = (NetGrossPremium - tax1percent).ToString("#,##0.00");

                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro3.Text);
                        lblActPersonType_pro3.Text = tax1percent.ToString("#,##0.00");
                        if (NetGrossPremium != 0)
                        {
                            lblVat1PercentBathAct_pro3.Text = ((NetGrossPremium - total_premium - tax1percent) / NetGrossPremium * 100).ToString("#,##0");
                        }
                        lblDiscountPercentAct_pro3.Text = (NetGrossPremium - total_premium - tax1percent).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (alert)
                {
                    AppUtil.ClientAlertTabRenew(this, ex.Message);
                }
                return false;
            }
        }       

        private bool CalActSumPro1(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro1.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro1.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro1.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro1.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                            txtActEndCoverDateAct_pro1.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActStartCoverDateAct_pro1.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActEndCoverDateAct_pro1.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro1.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro1.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro1.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro1.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {

                            tax1percent = 0;
                        }

                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro1.Text);

                        lblActPersonType_pro1.Text = tax1percent.ToString("#,##0.00");
                        if (NetGrossPremium != 0)
                        {
                            lblVat1PercentBathAct_pro1.Text = ((NetGrossPremium - total_premium - tax1percent) / NetGrossPremium * 100).ToString("#,##0");
                        }

                        lblDiscountPercentAct_pro1.Text = (NetGrossPremium - total_premium - tax1percent).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (alert)
                {
                    AppUtil.ClientAlertTabRenew(this, ex.Message);
                }
                return false;
            }
        }

        private bool CalActSumPro2(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro2.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro2.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro2.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro2.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                            txtActEndCoverDateAct_pro2.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActStartCoverDateAct_pro2.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActEndCoverDateAct_pro2.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro2.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro2.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro2.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro2.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {
                            tax1percent = 0;
                        }

                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro2.Text);
                        lblActPersonType_pro2.Text = tax1percent.ToString("#,##0.00");
                        if (NetGrossPremium != 0)
                        {
                            lblVat1PercentBathAct_pro2.Text = ((NetGrossPremium - total_premium - tax1percent) / NetGrossPremium * 100).ToString("#,##0");
                        }

                        lblDiscountPercentAct_pro2.Text = (NetGrossPremium - total_premium - tax1percent).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlertTabRenew(this, ex.Message);
                return false;
            }
        }

        private bool CalActSumPro3(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro3.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro3.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro3.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro3.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                            txtActEndCoverDateAct_pro3.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActStartCoverDateAct_pro3.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                            txtActEndCoverDateAct_pro3.DateValue = DateTime.MinValue;
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro3.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro3.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro3.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro3.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {
                            tax1percent = 0;
                        }

                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro3.Text);
                        lblActPersonType_pro3.Text = tax1percent.ToString("#,##0.00");
                        if (NetGrossPremium != 0)
                        {
                            lblVat1PercentBathAct_pro3.Text = ((NetGrossPremium - total_premium - tax1percent) / NetGrossPremium * 100).ToString("#,##0");
                        }
                        lblDiscountPercentAct_pro3.Text = (NetGrossPremium - total_premium - tax1percent).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (alert)
                {
                    AppUtil.ClientAlertTabRenew(this, ex.Message);
                }
                return false;
            }
        }

        private void calcPromotion(int PreleadId)
        {

        }

        private void txtActStartCoverDateAct_pro1_OnTextChanged(object sender, EventArgs e)
        {
            txtActEndCoverDateAct_pro1.DateValue = txtActStartCoverDateAct_pro1.DateValue.Year == 1 ? DateTime.MinValue : txtActStartCoverDateAct_pro1.DateValue.AddYears(1);
            CalActPro1(true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
        }
        private void txtActStartCoverDateAct_pro2_OnTextChanged(object sender, EventArgs e)
        {
            txtActEndCoverDateAct_pro2.DateValue = txtActStartCoverDateAct_pro2.DateValue.Year == 1 ? DateTime.MinValue : txtActStartCoverDateAct_pro2.DateValue.AddYears(1);
            CalActPro2(true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
        }
        private void txtActStartCoverDateAct_pro3_OnTextChanged(object sender, EventArgs e)
        {
            txtActEndCoverDateAct_pro3.DateValue = txtActStartCoverDateAct_pro3.DateValue.Year == 1 ? DateTime.MinValue : txtActStartCoverDateAct_pro3.DateValue.AddYears(1);
            CalActPro3(true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
        }

        private void txtActEndCoverDateAct_pro1_OnTextChanged(object sender, EventArgs e)
        {
            CalActPro1(true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
        }
        private void txtActEndCoverDateAct_pro2_OnTextChanged(object sender, EventArgs e)
        {
            CalActPro2(true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
        }
        private void txtActEndCoverDateAct_pro3_OnTextChanged(object sender, EventArgs e)
        {
            CalActPro3(true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
        }

        #region popupPromotion
        protected void lblPositionNameAbb_Click(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                
                if (e.CommandName == "popup")
                {
                    LinkButton lnkView = (LinkButton)e.CommandSource;
                    string slm_PromotionId = lnkView.CommandArgument;

                    if (slm_PromotionId != "0")
                    {
                        PromotionInsuranceData data = new PromotionInsuranceData();

                        data = ActivityPreLeadBiz.getPromotion(AppUtil.SafeInt(slm_PromotionId));

                        lblinsname.Text = data.insname;
                        lblcamname.Text = data.camname;
                        lblslm_DurationYear.Text = data.slm_DurationYear.ToString();
                        lblEffectiveDateFrom.Text = data.EffectiveDateFrom == null ? "" : data.EffectiveDateFrom.Value.ToString("dd/MM/yyyy");
                        lblEffectiveDateTo.Text = data.EffectiveDateTo == null ? "" : data.EffectiveDateTo.Value.ToString("dd/MM/yyyy");
                        lblbrandname.Text = data.brandname;
                        lblmodelname.Text = data.modelname;
                        lblUseCarType.Text = data.slm_UseCarType;
                        lblconveragetypename.Text = data.coveragetypename;
                        lblAgeDrivenFlag.Text = data.slm_AgeDrivenFlag;
                        lblrepairname.Text = data.repairname;
                        lblAgeCarYear.Text = data.slm_AgeCarYear.ToString();
                        //lblEngineSize.Text = data.slm_EngineSize == null ? "" : data.slm_EngineSize.Value.ToString("#,##0.00");
                        lblEngineSize.Text = data.slm_EngineSize == null ? "" : data.slm_EngineSize;
                        lblslm_OD.Text = data.slm_OD == null ? "" : data.slm_OD.Value.ToString("#,##0.00");
                        lblslm_FT.Text = data.slm_FT == null ? "" : data.slm_FT.Value.ToString("#,##0.00");
                        lblDeDuctible.Text = data.slm_DeDuctible;
                        lblGrossPremium.Text = data.slm_GrossPremium == null ? "" : data.slm_GrossPremium.Value.ToString("#,##0.00");
                        lblStamp.Text = data.slm_Stamp == null ? "" : data.slm_Stamp.Value.ToString("#,##0.00");
                        lblslm_Vat.Text = data.slm_Vat == null ? "" : data.slm_Vat.Value.ToString("#,##0.00");
                        lblNetGrossPremium.Text = data.slm_NetGrossPremium == null ? "" : data.slm_NetGrossPremium.Value.ToString("#,##0.00");
                        lblAct.Text = data.slm_Act == null ? "" : data.slm_Act.Value.ToString("#,##0.00");
                        lblInjuryDeath.Text = data.slm_InjuryDeath == null ? "" : data.slm_InjuryDeath.Value.ToString("#,##0.00");
                        lblTPPD.Text = data.slm_TPPD == null ? "" : data.slm_TPPD.Value.ToString("#,##0.00");
                        lblPersonalAccident.Text = data.slm_PersonalAccident == null ? "" : data.slm_PersonalAccident.Value.ToString("#,##0.00");
                        lblPersonalAccidentDriver.Text = data.slm_PersonalAccidentDriver;
                        lblPersonalAccidentPassenger.Text = data.slm_PersonalAccidentPassenger;
                        lblMedicalFee.Text = data.slm_MedicalFee == null ? "" : data.slm_MedicalFee.Value.ToString("#,##0.00");
                        lblMedicalFeeDriver.Text = data.slm_MedicalFeeDriver;
                        lblMedicalFeePassenger.Text = data.slm_MedicalFeePassenger;
                        lblInsuranceDriver.Text = data.slm_InsuranceDriver == null ? "" : data.slm_InsuranceDriver.Value.ToString("#,##0.00");
                        lblRemark.Text = data.slm_Remark;
                        mpePopupPromotion.Show();
                        upPopupPromotion.Update();
                    }
                    else
                    {
                        AppUtil.ClientAlertTabRenew(Page, "ไม่มีข้อมูล");
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void btnCancelPopupPromotion_Click(object sender, EventArgs e)
        {
            try
            {
                mpePopupPromotion.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void tabRenewInsuranceContainer_OnActiveTabChanged(object sender, EventArgs e)
        {
            hidHasCurrentYearPolicy.Value = "true";
            try
            {
                string nextTabId = "";      //Added By Pom 30/05/2016
                var prevId = tabRenewInsuranceContainer.Tabs[AppUtil.SafeInt(hidLasttab.Value)].ID;

                PreleadCompareDataCollectionGroup PreLeadDataCollectionGroup = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];

                int ActiveTab_ID = tabRenewInsuranceContainer.ActiveTabIndex;
                hidLasttab.Value = ActiveTab_ID.ToString();

                if (ActiveTab_ID == 0)
                {
                    var prev = PreLeadDataCollectionGroup.PreleadCompareDataCollections.Where(r => r.keyTab == prevId).FirstOrDefault();
                    prev = setViewStateData(prev);

                    nextTabId = PreLeadDataCollectionGroup.PreleadCompareDataCollectionMain.keyTab;     //Added By Pom 30/05/2016
                    bindControl(PreLeadDataCollectionGroup.PreleadCompareDataCollectionMain);
                }
                else
                {
                    if (prevId == PreLeadDataCollectionGroup.PreleadCompareDataCollectionMain.keyTab)
                    {
                        PreLeadDataCollectionGroup.PreleadCompareDataCollectionMain = setViewStateData(PreLeadDataCollectionGroup.PreleadCompareDataCollectionMain);
                    }

                    foreach (PreleadCompareDataCollection col in PreLeadDataCollectionGroup.PreleadCompareDataCollections)
                    {
                        if (col.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                        {
                            nextTabId = tabRenewInsuranceContainer.ActiveTab.ID;     //Added By Pom 30/05/2016
                            bindControl(col);
                        }
                    }
                }
                Session[SessionPrefix+"allData"] = PreLeadDataCollectionGroup;

                //Added By Pom 30/05/2016
                if (!string.IsNullOrEmpty(Request["ticketid"]))
                {
                    LeadDefaultData data = new SlmScr004Biz().GetLeadDataForInitialPhoneCall(nextTabId);
                    InitialControlLead(data, txtLoginName.Text.Trim());
                }
                else if (!string.IsNullOrEmpty(Request["preleadid"]))
                {
                    PreleadViewData data = new PreleadBiz().GetPreleadData(decimal.Parse(nextTabId), "", "", "");
                    InitialControlPrelead(data, txtLoginName.Text.Trim());
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        public List<string> getPrintValue()
        {
            PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
            PreleadCompareDataCollection current = new PreleadCompareDataCollection();

            List<string> retVal = new List<string>();

            current = pg.PreleadCompareDataCollectionMain;

            if (current.PrintFlag != null && current.PrintFlag != "")
            {
                retVal.Add(current.PrintFlag);
            }

            if (pg.PreleadCompareDataCollections != null)
            {
                if (pg.PreleadCompareDataCollections != null)
                {
                    foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                    {
                        current = p;
                        if (current.PrintFlag != null && current.PrintFlag != "")
                        {
                            retVal.Add(current.PrintFlag);
                        }
                    }
                }
            }

            return retVal;
        }

        protected void chkPrint_OnCheckedChanged(object sender, EventArgs e)
        {
            
            string PrintValue = "";


            PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
            PreleadCompareDataCollection current = new PreleadCompareDataCollection();

            string ActiveTab = tabRenewInsuranceContainer.Tabs[0].ID;
            if (tabRenewInsuranceContainer.ActiveTab.ID != null)
            {
                ActiveTab = tabRenewInsuranceContainer.ActiveTab.ID;
            }
            if (pg.PreleadCompareDataCollectionMain.keyTab == ActiveTab)
            {

                current = pg.PreleadCompareDataCollectionMain;
                if (chkPrint.Checked)
                {
                    if (PreLeadId != "")
                    {
                        current.PrintFlag = current.Prelead.slm_Prelead_Id.ToString();
                    }
                    else if (TicketId != "")
                    {
                        current.PrintFlag = PrintValue = current.RenewIns.slm_TicketId.ToString();
                    }
                }
                else
                {

                    current.PrintFlag = null;
                }
            }
            else
            {
                if (pg.PreleadCompareDataCollections != null)
                {
                    foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                    {
                        current = p;
                        if (p.keyTab == ActiveTab)
                        {
                            if (chkPrint.Checked)
                            {
                                if (PreLeadId != "")
                                {
                                    current.PrintFlag = current.Prelead.slm_Prelead_Id.ToString();
                                }
                                else if (TicketId != "")
                                {
                                    current.PrintFlag = PrintValue = current.RenewIns.slm_TicketId.ToString();
                                }
                            }
                            else
                            {

                                current.PrintFlag = null;
                            }
                        }
                    }
                }
            }
            Session[SessionPrefix+"allData"] = pg;
        }

        private PreleadCompareDataCollection setViewStateData(PreleadCompareDataCollection data)
        {

            try
            {
                data = PrepareForSavePreLead(data);
                data = PrepareForSaveCompare(data);
                data = PrepareForSaveAct(data);
                data = PrepareForSaveAddress(data);

                if (PreLeadId != "")
                {
                    //ActivityPreLeadBiz.SaveData(preleaddatacollectionsave);
                }
                else if (TicketId != "")
                { //lead
                    data = PrepareForSaveTransMain(data);
                    data = PrepareForSaveTrans(data);

                    data = PrepareForSaveRenewInsurance(data);
                    data = PrepareForSavePaymentMain(data);

                    data = PrepareProblemData(data);
                    data = PrepareForSaveReceipt(data);
                }

                return data;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
            return data;
        }
        #endregion

        #region pare
        protected void setPolicyPayment()
        {
            try
            {
                if (cmbPolicyPayMethod.SelectedValue == "2")
                {
                    txtPolicyAmountPeriod.Enabled = true;
                    cmbPayOption.Enabled = true;
                    if (cmbPayBranchCode.SelectedValue != "")
                    {
                        cmbPayBranchCode.Enabled = true;
                    }
                    else
                    {
                        cmbPayBranchCode.Enabled = false;
                    }
                    var payAmt = AppUtil.SafeDecimal(txtPolicyGrossPremium.Text);
                    if (txtPolicyAmountPeriod.Text == "")
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "1")
                    {
                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 1m).ToString("#,##0.00");

                        trPay1.Visible = true;

                        for (int i = 2; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "2")
                    {
                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 2m).ToString("#,##0.00");

                        tdmPolicyPaymentDate2.Enabled = true;
                        tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                        txtPolicyPaymentAmount2.Enabled = true;
                        txtPolicyPaymentAmount2.Text = (payAmt - (payAmt / 2m * 1m)).ToString("#,##0.00");

                        trPay1.Visible = true;
                        trPay2.Visible = true;

                        for (int i = 3; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "3")
                    {
                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 3m).ToString("#,##0.00");

                        tdmPolicyPaymentDate2.Enabled = true;
                        tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount2.Enabled = true;
                        txtPolicyPaymentAmount2.Text = (payAmt / 3m).ToString("#,##0.00");

                        tdmPolicyPaymentDate3.Enabled = true;
                        tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                        txtPolicyPaymentAmount3.Enabled = true;
                        txtPolicyPaymentAmount3.Text = (payAmt - (payAmt / 3m * 2m)).ToString("#,##0.00");

                        trPay1.Visible = true;
                        trPay2.Visible = true;
                        trPay3.Visible = true;

                        for (int i = 4; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "4")
                    {
                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 4m).ToString("#,##0.00");

                        tdmPolicyPaymentDate2.Enabled = true;
                        tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                        txtPolicyPaymentAmount2.Enabled = true;
                        txtPolicyPaymentAmount2.Text = (payAmt / 4m).ToString("#,##0.00");

                        tdmPolicyPaymentDate3.Enabled = true;
                        tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                        txtPolicyPaymentAmount3.Enabled = true;
                        txtPolicyPaymentAmount3.Text = (payAmt / 4m).ToString("#,##0.00");

                        tdmPolicyPaymentDate4.Enabled = true;
                        tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                        txtPolicyPaymentAmount4.Enabled = true;
                        txtPolicyPaymentAmount4.Text = (payAmt - (payAmt / 4m * 3m)).ToString("#,##0.00");

                        trPay1.Visible = true;
                        trPay2.Visible = true;
                        trPay3.Visible = true;
                        trPay4.Visible = true;

                        for (int i = 5; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "5")
                    {
                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 5m).ToString("#,##0.00");

                        tdmPolicyPaymentDate2.Enabled = true;
                        tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                        txtPolicyPaymentAmount2.Enabled = true;
                        txtPolicyPaymentAmount2.Text = (payAmt / 5m).ToString("#,##0.00");

                        tdmPolicyPaymentDate3.Enabled = true;
                        tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                        txtPolicyPaymentAmount3.Enabled = true;
                        txtPolicyPaymentAmount3.Text = (payAmt / 5m).ToString("#,##0.00");

                        tdmPolicyPaymentDate4.Enabled = true;
                        tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                        txtPolicyPaymentAmount4.Enabled = true;
                        txtPolicyPaymentAmount4.Text = (payAmt / 5m).ToString("#,##0.00");

                        tdmPolicyPaymentDate5.Enabled = true;
                        tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                        txtPolicyPaymentAmount5.Enabled = true;
                        txtPolicyPaymentAmount5.Text = (payAmt - (payAmt / 5m * 4m)).ToString("#,##0.00");

                        trPay1.Visible = true;
                        trPay2.Visible = true;
                        trPay3.Visible = true;
                        trPay4.Visible = true;
                        trPay5.Visible = true;
                        trPay6.Visible = true;

                        for (int i = 6; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "6")
                    {
                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 6m).ToString("#,##0.00");

                        tdmPolicyPaymentDate2.Enabled = true;
                        tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                        txtPolicyPaymentAmount2.Enabled = true;
                        txtPolicyPaymentAmount2.Text = (payAmt / 6m).ToString("#,##0.00");

                        tdmPolicyPaymentDate3.Enabled = true;
                        tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                        txtPolicyPaymentAmount3.Enabled = true;
                        txtPolicyPaymentAmount3.Text = (payAmt / 6m).ToString("#,##0.00");

                        tdmPolicyPaymentDate4.Enabled = true;
                        tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                        txtPolicyPaymentAmount4.Enabled = true;
                        txtPolicyPaymentAmount4.Text = (payAmt / 6m).ToString("#,##0.00");

                        tdmPolicyPaymentDate5.Enabled = true;
                        tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                        txtPolicyPaymentAmount5.Enabled = true;
                        txtPolicyPaymentAmount5.Text = (payAmt / 6m).ToString("#,##0.00");

                        tdmPolicyPaymentDate6.Enabled = true;
                        tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                        txtPolicyPaymentAmount6.Enabled = true;
                        txtPolicyPaymentAmount6.Text = (payAmt - (payAmt / 6m * 5m)).ToString("#,##0.00");

                        trPay1.Visible = true;
                        trPay2.Visible = true;
                        trPay3.Visible = true;
                        trPay4.Visible = true;
                        trPay5.Visible = true;
                        trPay6.Visible = true;

                        for (int i = 7; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "7")
                    {
                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 7m).ToString("#,##0.00");

                        tdmPolicyPaymentDate2.Enabled = true;
                        tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                        txtPolicyPaymentAmount2.Enabled = true;
                        txtPolicyPaymentAmount2.Text = (payAmt / 7m).ToString("#,##0.00");

                        tdmPolicyPaymentDate3.Enabled = true;
                        tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                        txtPolicyPaymentAmount3.Enabled = true;
                        txtPolicyPaymentAmount3.Text = (payAmt / 7m).ToString("#,##0.00");

                        tdmPolicyPaymentDate4.Enabled = true;
                        tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                        txtPolicyPaymentAmount4.Enabled = true;
                        txtPolicyPaymentAmount4.Text = (payAmt / 7m).ToString("#,##0.00");

                        tdmPolicyPaymentDate5.Enabled = true;
                        tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                        txtPolicyPaymentAmount5.Enabled = true;
                        txtPolicyPaymentAmount5.Text = (payAmt / 7m).ToString("#,##0.00");

                        tdmPolicyPaymentDate6.Enabled = true;
                        tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                        txtPolicyPaymentAmount6.Enabled = true;
                        txtPolicyPaymentAmount6.Text = (payAmt / 7m).ToString("#,##0.00");

                        tdmPolicyPaymentDate7.Enabled = true;
                        tdmPolicyPaymentDate7.DateValue = DateTime.Now.AddMonths(7);
                        txtPolicyPaymentAmount7.Enabled = true;
                        txtPolicyPaymentAmount7.Text = (payAmt - (payAmt / 7m * 6m)).ToString("#,##0.00");

                        trPay1.Visible = true;
                        trPay2.Visible = true;
                        trPay3.Visible = true;
                        trPay4.Visible = true;
                        trPay5.Visible = true;
                        trPay6.Visible = true;
                        trPay7.Visible = true;

                        for (int i = 8; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "8")
                    {

                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 8m).ToString("#,##0.00");

                        tdmPolicyPaymentDate2.Enabled = true;
                        tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                        txtPolicyPaymentAmount2.Enabled = true;
                        txtPolicyPaymentAmount2.Text = (payAmt / 8m).ToString("#,##0.00");

                        tdmPolicyPaymentDate3.Enabled = true;
                        tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                        txtPolicyPaymentAmount3.Enabled = true;
                        txtPolicyPaymentAmount3.Text = (payAmt / 8m).ToString("#,##0.00");

                        tdmPolicyPaymentDate4.Enabled = true;
                        tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                        txtPolicyPaymentAmount4.Enabled = true;
                        txtPolicyPaymentAmount4.Text = (payAmt / 8m).ToString("#,##0.00");

                        tdmPolicyPaymentDate5.Enabled = true;
                        tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                        txtPolicyPaymentAmount5.Enabled = true;
                        txtPolicyPaymentAmount5.Text = (payAmt / 8m).ToString("#,##0.00");

                        tdmPolicyPaymentDate6.Enabled = true;
                        tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                        txtPolicyPaymentAmount6.Enabled = true;
                        txtPolicyPaymentAmount6.Text = (payAmt / 8m).ToString("#,##0.00");

                        tdmPolicyPaymentDate7.Enabled = true;
                        tdmPolicyPaymentDate7.DateValue = DateTime.Now.AddMonths(7);
                        txtPolicyPaymentAmount7.Enabled = true;
                        txtPolicyPaymentAmount7.Text = (payAmt / 8m).ToString("#,##0.00");

                        tdmPolicyPaymentDate8.Enabled = true;
                        tdmPolicyPaymentDate8.DateValue = DateTime.Now.AddMonths(8);
                        txtPolicyPaymentAmount8.Enabled = true;
                        txtPolicyPaymentAmount8.Text = (payAmt - (payAmt / 8m * 7m)).ToString("#,##0.00");

                        trPay1.Visible = true;
                        trPay2.Visible = true;
                        trPay3.Visible = true;
                        trPay4.Visible = true;
                        trPay5.Visible = true;
                        trPay6.Visible = true;
                        trPay7.Visible = true;
                        trPay8.Visible = true;

                        for (int i = 9; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "9")
                    {
                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 9m).ToString("#,##0.00");

                        tdmPolicyPaymentDate2.Enabled = true;
                        tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                        txtPolicyPaymentAmount2.Enabled = true;
                        txtPolicyPaymentAmount2.Text = (payAmt / 9m).ToString("#,##0.00");

                        tdmPolicyPaymentDate3.Enabled = true;
                        tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                        txtPolicyPaymentAmount3.Enabled = true;
                        txtPolicyPaymentAmount3.Text = (payAmt / 9m).ToString("#,##0.00");

                        tdmPolicyPaymentDate4.Enabled = true;
                        tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                        txtPolicyPaymentAmount4.Enabled = true;
                        txtPolicyPaymentAmount4.Text = (payAmt / 9m).ToString("#,##0.00");

                        tdmPolicyPaymentDate5.Enabled = true;
                        tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                        txtPolicyPaymentAmount5.Enabled = true;
                        txtPolicyPaymentAmount5.Text = (payAmt / 9m).ToString("#,##0.00");

                        tdmPolicyPaymentDate6.Enabled = true;
                        tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                        txtPolicyPaymentAmount6.Enabled = true;
                        txtPolicyPaymentAmount6.Text = (payAmt / 9m).ToString("#,##0.00");

                        tdmPolicyPaymentDate7.Enabled = true;
                        tdmPolicyPaymentDate7.DateValue = DateTime.Now.AddMonths(7);
                        txtPolicyPaymentAmount7.Enabled = true;
                        txtPolicyPaymentAmount7.Text = (payAmt / 9m).ToString("#,##0.00");

                        tdmPolicyPaymentDate8.Enabled = true;
                        tdmPolicyPaymentDate8.DateValue = DateTime.Now.AddMonths(8);
                        txtPolicyPaymentAmount8.Enabled = true;
                        txtPolicyPaymentAmount8.Text = (payAmt / 9m).ToString("#,##0.00");

                        tdmPolicyPaymentDate9.Enabled = true;
                        tdmPolicyPaymentDate9.DateValue = DateTime.Now.AddMonths(9);
                        txtPolicyPaymentAmount9.Enabled = true;
                        txtPolicyPaymentAmount9.Text = (payAmt - (payAmt / 9m * 8m)).ToString("#,##0.00");

                        trPay1.Visible = true;
                        trPay2.Visible = true;
                        trPay3.Visible = true;
                        trPay4.Visible = true;
                        trPay5.Visible = true;
                        trPay6.Visible = true;
                        trPay7.Visible = true;
                        trPay8.Visible = true;
                        trPay9.Visible = true;

                        for (int i = 10; i <= 10; i++)
                        {
                            ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                            ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                            ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                        }
                    }
                    else if (txtPolicyAmountPeriod.Text == "10")
                    {
                        tdmPolicyPaymentDate1.Enabled = true;
                        tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                        txtPolicyPaymentAmount1.Enabled = true;
                        txtPolicyPaymentAmount1.Text = (payAmt / 10m).ToString("#,##0.00");

                        tdmPolicyPaymentDate2.Enabled = true;
                        tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                        txtPolicyPaymentAmount2.Enabled = true;
                        txtPolicyPaymentAmount2.Text = (payAmt / 10m).ToString("#,##0.00");

                        tdmPolicyPaymentDate3.Enabled = true;
                        tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                        txtPolicyPaymentAmount3.Enabled = true;
                        txtPolicyPaymentAmount3.Text = (payAmt / 10m).ToString("#,##0.00");

                        tdmPolicyPaymentDate4.Enabled = true;
                        tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                        txtPolicyPaymentAmount4.Enabled = true;
                        txtPolicyPaymentAmount4.Text = (payAmt / 10m).ToString("#,##0.00");

                        tdmPolicyPaymentDate5.Enabled = true;
                        tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                        txtPolicyPaymentAmount5.Enabled = true;
                        txtPolicyPaymentAmount5.Text = (payAmt / 10m).ToString("#,##0.00");

                        tdmPolicyPaymentDate6.Enabled = true;
                        tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                        txtPolicyPaymentAmount6.Enabled = true;
                        txtPolicyPaymentAmount6.Text = (payAmt / 10m).ToString("#,##0.00");

                        tdmPolicyPaymentDate7.Enabled = true;
                        tdmPolicyPaymentDate7.DateValue = DateTime.Now.AddMonths(7);
                        txtPolicyPaymentAmount7.Enabled = true;
                        txtPolicyPaymentAmount7.Text = (payAmt / 10m).ToString("#,##0.00");

                        tdmPolicyPaymentDate8.Enabled = true;
                        tdmPolicyPaymentDate8.DateValue = DateTime.Now.AddMonths(8);
                        txtPolicyPaymentAmount8.Enabled = true;
                        txtPolicyPaymentAmount8.Text = (payAmt / 10m).ToString("#,##0.00");

                        tdmPolicyPaymentDate9.Enabled = true;
                        tdmPolicyPaymentDate9.DateValue = DateTime.Now.AddMonths(9);
                        txtPolicyPaymentAmount9.Enabled = true;
                        txtPolicyPaymentAmount9.Text = (payAmt / 10m).ToString("#,##0.00");

                        tdmPolicyPaymentDate10.Enabled = true;
                        tdmPolicyPaymentDate10.DateValue = DateTime.Now.AddMonths(10);
                        txtPolicyPaymentAmount10.Enabled = true;
                        txtPolicyPaymentAmount10.Text = (payAmt - (payAmt / 10m * 9m)).ToString("#,##0.00");

                        trPay1.Visible = true;
                        trPay2.Visible = true;
                        trPay3.Visible = true;
                        trPay4.Visible = true;
                        trPay5.Visible = true;
                        trPay6.Visible = true;
                        trPay7.Visible = true;
                        trPay8.Visible = true;
                        trPay9.Visible = true;
                        trPay10.Visible = true;
                    }
                }
                else if (cmbPolicyPayMethod.SelectedValue == "3")
                {
                    txtPolicyAmountPeriod.Enabled = false;
                    cmbPayOption.Enabled = true;
                    if (cmbPayBranchCode.SelectedValue != "")
                    {
                        cmbPayBranchCode.Enabled = true;
                    }
                    else
                    {
                        cmbPayBranchCode.Enabled = false;
                    }
                    tdmPolicyPaymentDate1.Enabled = false;
                    tdmPolicyPaymentDate1.DateValue = DateTime.MinValue;
                    txtPolicyPaymentAmount1.Enabled = false;
                    tdmPolicyPaymentDate2.Enabled = false;
                    tdmPolicyPaymentDate2.DateValue = DateTime.MinValue;
                    txtPolicyPaymentAmount2.Enabled = false;
                    tdmPolicyPaymentDate3.Enabled = false;
                    tdmPolicyPaymentDate3.DateValue = DateTime.MinValue;
                    txtPolicyPaymentAmount3.Enabled = false;


                }
                else if (cmbPolicyPayMethod.SelectedValue == "")
                {
                    txtPolicyAmountPeriod.Enabled = false;
                    txtPolicyAmountPeriod.Text = "";
                    cmbPayOption.Enabled = true;
                    cmbPayBranchCode.Enabled = false;
                    cmbPayBranchCode.SelectedValue = "";
                    tdmPolicyPaymentDate1.Enabled = false;
                    tdmPolicyPaymentDate1.DateValue = DateTime.MinValue;
                    txtPolicyPaymentAmount1.Enabled = false;
                    tdmPolicyPaymentDate2.Enabled = false;
                    tdmPolicyPaymentDate2.DateValue = DateTime.MinValue;
                    txtPolicyPaymentAmount2.Enabled = false;
                    tdmPolicyPaymentDate3.Enabled = false;
                    tdmPolicyPaymentDate3.DateValue = DateTime.MinValue;
                    txtPolicyPaymentAmount3.Enabled = false;
                }
                else
                {
                    txtPolicyAmountPeriod.Enabled = false;
                    txtPolicyAmountPeriod.Text = "";
                    cmbPayOption.Enabled = true;

                    if (cmbPayBranchCode.SelectedValue != "")
                    {
                        cmbPayBranchCode.Enabled = true;
                    }
                    else
                    {
                        cmbPayBranchCode.Enabled = false;
                    }
                    tdmPolicyPaymentDate1.Enabled = false;
                    tdmPolicyPaymentDate1.DateValue = DateTime.MinValue;
                    txtPolicyPaymentAmount1.Enabled = false;
                    tdmPolicyPaymentDate2.Enabled = false;
                    txtPolicyPaymentAmount2.Enabled = false;
                    tdmPolicyPaymentDate3.Enabled = false;
                    txtPolicyPaymentAmount3.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private void setCopy()
        {
            try
            {
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                PreleadCompareDataCollection current = new PreleadCompareDataCollection();
                if (TicketId != null && TicketId != "")
                {
                    if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                    {
                        pnCopy.Visible = false;
                    }
                    else
                    {
                        if (pg.PreleadCompareDataCollections != null)
                        {
                            foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                            {
                                lblMainContractNo.Text = pg.PreleadCompareDataCollectionMain.keyTab;
                                pnCopy.Visible = true;
                            }
                        }
                    }
                }
                else
                {
                    pnCopy.Visible = false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }


        protected void cmbPolicyPayMethod_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {           
                setPolicyPayment();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void txtPolicyAmountPeriod_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                
                var payAmt = AppUtil.SafeDecimal(txtPolicyGrossPremium.Text);
                if (txtPolicyAmountPeriod.Text == "")
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "1")
                {
                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 1m).ToString("#,##0.00");

                    trPay1.Visible = true;

                    for (int i = 2; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "2")
                {
                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 2m).ToString("#,##0.00");

                    tdmPolicyPaymentDate2.Enabled = true;
                    tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                    txtPolicyPaymentAmount2.Enabled = true;
                    txtPolicyPaymentAmount2.Text = (payAmt - (payAmt / 2m * 1m)).ToString("#,##0.00");

                    trPay1.Visible = true;
                    trPay2.Visible = true;

                    for (int i = 3; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "3")
                {
                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 3m).ToString("#,##0.00");

                    tdmPolicyPaymentDate2.Enabled = true;
                    tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount2.Enabled = true;
                    txtPolicyPaymentAmount2.Text = (payAmt / 3m).ToString("#,##0.00");

                    tdmPolicyPaymentDate3.Enabled = true;
                    tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                    txtPolicyPaymentAmount3.Enabled = true;
                    txtPolicyPaymentAmount3.Text = (payAmt - (payAmt / 3m * 2m)).ToString("#,##0.00");

                    trPay1.Visible = true;
                    trPay2.Visible = true;
                    trPay3.Visible = true;

                    for (int i = 4; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "4")
                {
                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 4m).ToString("#,##0.00");

                    tdmPolicyPaymentDate2.Enabled = true;
                    tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                    txtPolicyPaymentAmount2.Enabled = true;
                    txtPolicyPaymentAmount2.Text = (payAmt / 4m).ToString("#,##0.00");

                    tdmPolicyPaymentDate3.Enabled = true;
                    tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                    txtPolicyPaymentAmount3.Enabled = true;
                    txtPolicyPaymentAmount3.Text = (payAmt / 4m).ToString("#,##0.00");

                    tdmPolicyPaymentDate4.Enabled = true;
                    tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                    txtPolicyPaymentAmount4.Enabled = true;
                    txtPolicyPaymentAmount4.Text = (payAmt - (payAmt / 4m * 3m)).ToString("#,##0.00");

                    trPay1.Visible = true;
                    trPay2.Visible = true;
                    trPay3.Visible = true;
                    trPay4.Visible = true;

                    for (int i = 5; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "5")
                {
                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 5m).ToString("#,##0.00");

                    tdmPolicyPaymentDate2.Enabled = true;
                    tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                    txtPolicyPaymentAmount2.Enabled = true;
                    txtPolicyPaymentAmount2.Text = (payAmt / 5m).ToString("#,##0.00");

                    tdmPolicyPaymentDate3.Enabled = true;
                    tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                    txtPolicyPaymentAmount3.Enabled = true;
                    txtPolicyPaymentAmount3.Text = (payAmt / 5m).ToString("#,##0.00");

                    tdmPolicyPaymentDate4.Enabled = true;
                    tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                    txtPolicyPaymentAmount4.Enabled = true;
                    txtPolicyPaymentAmount4.Text = (payAmt / 5m).ToString("#,##0.00");

                    tdmPolicyPaymentDate5.Enabled = true;
                    tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                    txtPolicyPaymentAmount5.Enabled = true;
                    txtPolicyPaymentAmount5.Text = (payAmt - (payAmt / 5m * 4m)).ToString("#,##0.00");

                    trPay1.Visible = true;
                    trPay2.Visible = true;
                    trPay3.Visible = true;
                    trPay4.Visible = true;
                    trPay5.Visible = true;
                    trPay6.Visible = true;

                    for (int i = 6; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "6")
                {
                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 6m).ToString("#,##0.00");

                    tdmPolicyPaymentDate2.Enabled = true;
                    tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                    txtPolicyPaymentAmount2.Enabled = true;
                    txtPolicyPaymentAmount2.Text = (payAmt / 6m).ToString("#,##0.00");

                    tdmPolicyPaymentDate3.Enabled = true;
                    tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                    txtPolicyPaymentAmount3.Enabled = true;
                    txtPolicyPaymentAmount3.Text = (payAmt / 6m).ToString("#,##0.00");

                    tdmPolicyPaymentDate4.Enabled = true;
                    tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                    txtPolicyPaymentAmount4.Enabled = true;
                    txtPolicyPaymentAmount4.Text = (payAmt / 6m).ToString("#,##0.00");

                    tdmPolicyPaymentDate5.Enabled = true;
                    tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                    txtPolicyPaymentAmount5.Enabled = true;
                    txtPolicyPaymentAmount5.Text = (payAmt / 6m).ToString("#,##0.00");

                    tdmPolicyPaymentDate6.Enabled = true;
                    tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                    txtPolicyPaymentAmount6.Enabled = true;
                    txtPolicyPaymentAmount6.Text = (payAmt - (payAmt / 6m * 5m)).ToString("#,##0.00");

                    trPay1.Visible = true;
                    trPay2.Visible = true;
                    trPay3.Visible = true;
                    trPay4.Visible = true;
                    trPay5.Visible = true;
                    trPay6.Visible = true;

                    for (int i = 7; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "7")
                {
                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 7m).ToString("#,##0.00");

                    tdmPolicyPaymentDate2.Enabled = true;
                    tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                    txtPolicyPaymentAmount2.Enabled = true;
                    txtPolicyPaymentAmount2.Text = (payAmt / 7m).ToString("#,##0.00");

                    tdmPolicyPaymentDate3.Enabled = true;
                    tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                    txtPolicyPaymentAmount3.Enabled = true;
                    txtPolicyPaymentAmount3.Text = (payAmt / 7m).ToString("#,##0.00");

                    tdmPolicyPaymentDate4.Enabled = true;
                    tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                    txtPolicyPaymentAmount4.Enabled = true;
                    txtPolicyPaymentAmount4.Text = (payAmt / 7m).ToString("#,##0.00");

                    tdmPolicyPaymentDate5.Enabled = true;
                    tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                    txtPolicyPaymentAmount5.Enabled = true;
                    txtPolicyPaymentAmount5.Text = (payAmt / 7m).ToString("#,##0.00");

                    tdmPolicyPaymentDate6.Enabled = true;
                    tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                    txtPolicyPaymentAmount6.Enabled = true;
                    txtPolicyPaymentAmount6.Text = (payAmt / 7m).ToString("#,##0.00");

                    tdmPolicyPaymentDate7.Enabled = true;
                    tdmPolicyPaymentDate7.DateValue = DateTime.Now.AddMonths(7);
                    txtPolicyPaymentAmount7.Enabled = true;
                    txtPolicyPaymentAmount7.Text = (payAmt - (payAmt / 7m * 6m)).ToString("#,##0.00");

                    trPay1.Visible = true;
                    trPay2.Visible = true;
                    trPay3.Visible = true;
                    trPay4.Visible = true;
                    trPay5.Visible = true;
                    trPay6.Visible = true;
                    trPay7.Visible = true;

                    for (int i = 8; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "8")
                {

                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 8m).ToString("#,##0.00");

                    tdmPolicyPaymentDate2.Enabled = true;
                    tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                    txtPolicyPaymentAmount2.Enabled = true;
                    txtPolicyPaymentAmount2.Text = (payAmt / 8m).ToString("#,##0.00");

                    tdmPolicyPaymentDate3.Enabled = true;
                    tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                    txtPolicyPaymentAmount3.Enabled = true;
                    txtPolicyPaymentAmount3.Text = (payAmt / 8m).ToString("#,##0.00");

                    tdmPolicyPaymentDate4.Enabled = true;
                    tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                    txtPolicyPaymentAmount4.Enabled = true;
                    txtPolicyPaymentAmount4.Text = (payAmt / 8m).ToString("#,##0.00");

                    tdmPolicyPaymentDate5.Enabled = true;
                    tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                    txtPolicyPaymentAmount5.Enabled = true;
                    txtPolicyPaymentAmount5.Text = (payAmt / 8m).ToString("#,##0.00");

                    tdmPolicyPaymentDate6.Enabled = true;
                    tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                    txtPolicyPaymentAmount6.Enabled = true;
                    txtPolicyPaymentAmount6.Text = (payAmt / 8m).ToString("#,##0.00");

                    tdmPolicyPaymentDate7.Enabled = true;
                    tdmPolicyPaymentDate7.DateValue = DateTime.Now.AddMonths(7);
                    txtPolicyPaymentAmount7.Enabled = true;
                    txtPolicyPaymentAmount7.Text = (payAmt / 8m).ToString("#,##0.00");

                    tdmPolicyPaymentDate8.Enabled = true;
                    tdmPolicyPaymentDate8.DateValue = DateTime.Now.AddMonths(8);
                    txtPolicyPaymentAmount8.Enabled = true;
                    txtPolicyPaymentAmount8.Text = (payAmt - (payAmt / 8m * 7m)).ToString("#,##0.00");

                    trPay1.Visible = true;
                    trPay2.Visible = true;
                    trPay3.Visible = true;
                    trPay4.Visible = true;
                    trPay5.Visible = true;
                    trPay6.Visible = true;
                    trPay7.Visible = true;
                    trPay8.Visible = true;

                    for (int i = 9; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "9")
                {
                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 9m).ToString("#,##0.00");

                    tdmPolicyPaymentDate2.Enabled = true;
                    tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                    txtPolicyPaymentAmount2.Enabled = true;
                    txtPolicyPaymentAmount2.Text = (payAmt / 9m).ToString("#,##0.00");

                    tdmPolicyPaymentDate3.Enabled = true;
                    tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                    txtPolicyPaymentAmount3.Enabled = true;
                    txtPolicyPaymentAmount3.Text = (payAmt / 9m).ToString("#,##0.00");

                    tdmPolicyPaymentDate4.Enabled = true;
                    tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                    txtPolicyPaymentAmount4.Enabled = true;
                    txtPolicyPaymentAmount4.Text = (payAmt / 9m).ToString("#,##0.00");

                    tdmPolicyPaymentDate5.Enabled = true;
                    tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                    txtPolicyPaymentAmount5.Enabled = true;
                    txtPolicyPaymentAmount5.Text = (payAmt / 9m).ToString("#,##0.00");

                    tdmPolicyPaymentDate6.Enabled = true;
                    tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                    txtPolicyPaymentAmount6.Enabled = true;
                    txtPolicyPaymentAmount6.Text = (payAmt / 9m).ToString("#,##0.00");

                    tdmPolicyPaymentDate7.Enabled = true;
                    tdmPolicyPaymentDate7.DateValue = DateTime.Now.AddMonths(7);
                    txtPolicyPaymentAmount7.Enabled = true;
                    txtPolicyPaymentAmount7.Text = (payAmt / 9m).ToString("#,##0.00");

                    tdmPolicyPaymentDate8.Enabled = true;
                    tdmPolicyPaymentDate8.DateValue = DateTime.Now.AddMonths(8);
                    txtPolicyPaymentAmount8.Enabled = true;
                    txtPolicyPaymentAmount8.Text = (payAmt / 9m).ToString("#,##0.00");

                    tdmPolicyPaymentDate9.Enabled = true;
                    tdmPolicyPaymentDate9.DateValue = DateTime.Now.AddMonths(9);
                    txtPolicyPaymentAmount9.Enabled = true;
                    txtPolicyPaymentAmount9.Text = (payAmt - (payAmt / 9m * 8m)).ToString("#,##0.00");

                    trPay1.Visible = true;
                    trPay2.Visible = true;
                    trPay3.Visible = true;
                    trPay4.Visible = true;
                    trPay5.Visible = true;
                    trPay6.Visible = true;
                    trPay7.Visible = true;
                    trPay8.Visible = true;
                    trPay9.Visible = true;

                    for (int i = 10; i <= 10; i++)
                    {
                        ((HtmlTableRow)this.FindControlRecursive("trPay" + i)).Visible = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Enabled = false;
                        ((TextBox)this.FindControlRecursive("txtPolicyPaymentAmount" + i)).Text = "";
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).Enabled = false;
                        ((TextDateMask)this.FindControlRecursive("tdmPolicyPaymentDate" + i)).DateValue = DateTime.MinValue;
                    }
                }
                else if (txtPolicyAmountPeriod.Text == "10")
                {
                    tdmPolicyPaymentDate1.Enabled = true;
                    tdmPolicyPaymentDate1.DateValue = DateTime.Now.AddMonths(1);
                    txtPolicyPaymentAmount1.Enabled = true;
                    txtPolicyPaymentAmount1.Text = (payAmt / 10m).ToString("#,##0.00");

                    tdmPolicyPaymentDate2.Enabled = true;
                    tdmPolicyPaymentDate2.DateValue = DateTime.Now.AddMonths(2);
                    txtPolicyPaymentAmount2.Enabled = true;
                    txtPolicyPaymentAmount2.Text = (payAmt / 10m).ToString("#,##0.00");

                    tdmPolicyPaymentDate3.Enabled = true;
                    tdmPolicyPaymentDate3.DateValue = DateTime.Now.AddMonths(3);
                    txtPolicyPaymentAmount3.Enabled = true;
                    txtPolicyPaymentAmount3.Text = (payAmt / 10m).ToString("#,##0.00");

                    tdmPolicyPaymentDate4.Enabled = true;
                    tdmPolicyPaymentDate4.DateValue = DateTime.Now.AddMonths(4);
                    txtPolicyPaymentAmount4.Enabled = true;
                    txtPolicyPaymentAmount4.Text = (payAmt / 10m).ToString("#,##0.00");

                    tdmPolicyPaymentDate5.Enabled = true;
                    tdmPolicyPaymentDate5.DateValue = DateTime.Now.AddMonths(5);
                    txtPolicyPaymentAmount5.Enabled = true;
                    txtPolicyPaymentAmount5.Text = (payAmt / 10m).ToString("#,##0.00");

                    tdmPolicyPaymentDate6.Enabled = true;
                    tdmPolicyPaymentDate6.DateValue = DateTime.Now.AddMonths(6);
                    txtPolicyPaymentAmount6.Enabled = true;
                    txtPolicyPaymentAmount6.Text = (payAmt / 10m).ToString("#,##0.00");

                    tdmPolicyPaymentDate7.Enabled = true;
                    tdmPolicyPaymentDate7.DateValue = DateTime.Now.AddMonths(7);
                    txtPolicyPaymentAmount7.Enabled = true;
                    txtPolicyPaymentAmount7.Text = (payAmt / 10m).ToString("#,##0.00");

                    tdmPolicyPaymentDate8.Enabled = true;
                    tdmPolicyPaymentDate8.DateValue = DateTime.Now.AddMonths(8);
                    txtPolicyPaymentAmount8.Enabled = true;
                    txtPolicyPaymentAmount8.Text = (payAmt / 10m).ToString("#,##0.00");

                    tdmPolicyPaymentDate9.Enabled = true;
                    tdmPolicyPaymentDate9.DateValue = DateTime.Now.AddMonths(9);
                    txtPolicyPaymentAmount9.Enabled = true;
                    txtPolicyPaymentAmount9.Text = (payAmt / 10m).ToString("#,##0.00");

                    tdmPolicyPaymentDate10.Enabled = true;
                    tdmPolicyPaymentDate10.DateValue = DateTime.Now.AddMonths(10);
                    txtPolicyPaymentAmount10.Enabled = true;
                    txtPolicyPaymentAmount10.Text = (payAmt - (payAmt / 10m * 9m)).ToString("#,##0.00");

                    trPay1.Visible = true;
                    trPay2.Visible = true;
                    trPay3.Visible = true;
                    trPay4.Visible = true;
                    trPay5.Visible = true;
                    trPay6.Visible = true;
                    trPay7.Visible = true;
                    trPay8.Visible = true;
                    trPay9.Visible = true;
                    trPay10.Visible = true;

                }
                else
                {
                    AppUtil.ClientAlertTabRenew(Page, "กรุณาเลือกไม่เกิน 10 งวด");
                }
                pnPaymentDetail.Update();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void cmbPaymentType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {           
                if (cmbPaymentType.SelectedValue == "4")
                {
                    cmbPolicyPayBranchCode.Enabled = true;
                }
                else if (cmbPaymentType.SelectedValue == "")
                {
                    cmbPolicyPayBranchCode.Enabled = false;
                    cmbPolicyPayBranchCode.SelectedValue = "";
                }
                else
                {
                    cmbPolicyPayBranchCode.Enabled = false;
                    cmbPolicyPayBranchCode.SelectedValue = "";
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void cmbPaymentTypeAct_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {          
                if (cmbPaymentTypeAct.SelectedValue == "4")
                {
                    cmbPayBranchCodeAct.Enabled = true;
                }
                else if (cmbPaymentType.SelectedValue == "")
                {
                    cmbPayBranchCodeAct.Enabled = false;
                    cmbPayBranchCodeAct.SelectedValue = "";
                }
                else
                {
                    cmbPayBranchCodeAct.Enabled = false;
                    cmbPayBranchCodeAct.SelectedValue = "";

                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }       

        protected void cmbPayOption_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {              
                if (cmbPayOption.SelectedValue == "4")
                {

                    cmbPayBranchCode.Enabled = true;

                }
                else
                {
                    cmbPayBranchCode.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        public void setColumn()
        {
            try
            {
                var sCase = AppUtil.SafeInt(Type);
                if (sCase == 1) // KK Motor Renew
                {
                    hidYear_pre.Value = "Y";
                    hidYear_cur.Value = "Y";
                    divPromotion.Visible = true;
                    divPayment.Visible = true;

                    divPolicy.Visible = true;
                    tabPayment.Visible = true;

                }
                else if (sCase == 2) //KK Motor Box
                {
                    hidYear_pre.Value = "Y";
                    hidYear_cur.Value = "Y"; // "N";

                    divPromotion.Visible = true;
                    divPayment.Visible = true;// false;

                    divPolicy.Visible = true;
                    tabPayment.Visible = true;
                }
                else if (sCase == 3)
                {
                    //Product : พรบ.เดี่ยว , Campaign : All
                    hidYear_pre.Value = "Y";
                    hidYear_cur.Value = "N";
                    hidPromotionActId1.Value = "0";// show blank col
                    lblPromoAct_name_pro1.Text = "บริษัทประกันอื่นๆ";
                    imgDelAct_pro1.Visible = false;
                    setDisablePromoAct(1, true);
                    pnPaymentMainPolicy.Visible = false;
                    divPromotion.Visible = false;
                    divPayment.Visible = false;
                    divPolicy.Visible = false;
                    tabPayment.Visible = false;
                    pnCopy.Visible = false;
                    btnAddBlankPolicy.Visible = false;
                    if (!IsPostBack)
                    {
                        var pg = getSelectTabData();

                        if (pg.ActPromoList.Count == 0)
                        {
                            PreleadCompareActData PreleadCompareAct = new PreleadCompareActData();

                            PreleadCompareAct.slm_PromotionId = 0;
                            PreleadCompareAct.slm_Seq = 2;
                            pg.ActPromoList.Add(PreleadCompareAct);
                        }

                        PreleadCompareDataCollectionGroup p = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                        if (p.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                        {

                            p.PreleadCompareDataCollectionMain = pg;
                        }
                        else
                        {
                            if (p.PreleadCompareDataCollections != null)
                            {
                                var pc = p.PreleadCompareDataCollections.Where(e => e.keyTab == tabRenewInsuranceContainer.ActiveTab.ID).FirstOrDefault();
                                pc = pg;
                            }
                        }
                        Session[SessionPrefix+"allData"] = p;

                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void gvReceipt_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    HiddenField hidStatus = (HiddenField)e.Row.FindControl("hidStatus");
                    DropDownList list = (DropDownList)e.Row.FindControl("cmbStatus");

                    list.SelectedValue = hidStatus.Value;

                    if (list.SelectedValue == "")
                    {
                        ((Button)e.Row.FindControl("btnReceipt")).Enabled = false;
                    }
                    else
                    {
                        if (((HiddenField)e.Row.FindControl("hidcountExport")).Value == "0")
                        {
                            ((Button)e.Row.FindControl("btnReceipt")).Enabled = true;
                        }
                        else
                        {
                            list.Enabled = false;
                            ((Button)e.Row.FindControl("btnReceipt")).Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void gvReceipt_OnPreRender(object sender, EventArgs e)
        {

            //GridViewRow row = gvReceipt.Rows[gvReceipt.Rows.Count - 1];

            //((Button)row.FindControl("btnReceipt")).Visible = true;
        }

        protected void gvReceipt_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "view")
                {
                    ImageButton lnkView = (ImageButton)e.CommandSource;
                    string slm_RenewInsuranceReceiptId = lnkView.CommandArgument;

                    List<RenewInsuranceReceiptDetailData> list = ActivityLeadBiz.GetImportList(slm_RenewInsuranceReceiptId);

                    gvImport.DataSource = list;
                    gvImport.DataBind();

                    if (divimport.Visible)
                    {
                        divimport.Visible = false;
                    }
                    else
                    {
                        divimport.Visible = true;
                    }

                    pnReceipt.Update();
                }
                else if (e.CommandName == "show")
                {
                    Button btnView = (Button)e.CommandSource;
                    string slm_RenewInsuranceReceiptId = btnView.CommandArgument;

                    GridViewRow gvRow = (GridViewRow)btnView.NamingContainer;

                    var status = ((DropDownList)gvRow.FindControl("cmbStatus")).SelectedValue;

                    PreleadCompareDataCollectionGroup PreLeadDataCollectionGroup = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                    PreleadCompareDataCollection current;

                    int ActiveTab_ID = tabRenewInsuranceContainer.ActiveTabIndex;
                    if (ActiveTab_ID == 0)
                    {

                        current = PreLeadDataCollectionGroup.PreleadCompareDataCollectionMain;
                    }
                    else
                    {
                        foreach (PreleadCompareDataCollection col in PreLeadDataCollectionGroup.PreleadCompareDataCollections)
                        {
                            if (col.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                            {
                                current = col;
                            }
                        }
                    }


                    if (status == "")
                    {

                        chkPaymentDesc1.Checked = false;
                        txtPaymentDesc1.Text = "";
                        chkPaymentDesc2.Checked = false;
                        txtPaymentDesc2.Text = "";
                        chkPaymentDesc3.Checked = false;
                        txtPaymentDesc3.Text = "";
                        chkPaymentDesc4.Checked = false;
                        txtPaymentDesc4.Text = "";
                        chkPaymentDesc5.Checked = false;
                        txtPaymentDesc5.Text = "";
                        chkPaymentDesc6.Checked = false;
                        txtPaymentDesc6.Text = "";
                        txtPaymentOther.Text = "";
                        divEditReceipt.Visible = false;
                        hidRenewInsuranceReceiptId.Value = slm_RenewInsuranceReceiptId;
                        hidRenewInsuranceStatus.Value = "D";

                        PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                        List<RenewInsuranceReceiptRevisionDetailData> currentReceiptRevisionList = new List<RenewInsuranceReceiptRevisionDetailData>();
                        if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                        {
                            currentReceiptRevisionList = pg.PreleadCompareDataCollectionMain.ReceiptRevisionDetailList;
                        }
                        else
                        {
                            if (pg.PreleadCompareDataCollections != null)
                            {
                                foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                                {
                                    currentReceiptRevisionList = p.ReceiptRevisionDetailList;
                                }
                            }
                        }

                        if (currentReceiptRevisionList != null && currentReceiptRevisionList.Count > 0)
                        {

                            List<RenewInsuranceReceiptRevisionDetailData> currentReceiptRevision = currentReceiptRevisionList.Where(r => r.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(slm_RenewInsuranceReceiptId)).ToList();
                            if (currentReceiptRevision != null && currentReceiptRevision.Count > 0)
                            {
                                foreach (RenewInsuranceReceiptRevisionDetailData rrrd in currentReceiptRevision)
                                {
                                    rrrd.delflag = "D";
                                }
                            }
                        }

                        pnReceipt.Update();
                    }
                    else
                    {
                        PreleadCompareDataCollection data = getSelectTabData();

                        List<RenewInsuranceReceiptRevisionDetailData> list = data.ReceiptRevisionDetailList.Where(r => r.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(slm_RenewInsuranceReceiptId) && r.delflag != "D").ToList();

                        hidRenewInsuranceReceiptId.Value = slm_RenewInsuranceReceiptId;
                        hidRenewInsuranceStatus.Value = "";
                        hidRenewInsuranceReceiptAmount.Value = AppUtil.SafeDecimal(gvRow.Cells[2].Text).ToString();
                        divEditReceipt.Visible = true;
                        chkPaymentDesc1.Checked = false;
                        txtPaymentDesc1.Text = "";
                        chkPaymentDesc2.Checked = false;
                        txtPaymentDesc2.Text = "";
                        chkPaymentDesc3.Checked = false;
                        txtPaymentDesc3.Text = "";
                        chkPaymentDesc4.Checked = false;
                        txtPaymentDesc4.Text = "";
                        chkPaymentDesc5.Checked = false;
                        txtPaymentDesc5.Text = "";
                        chkPaymentDesc6.Checked = false;
                        txtPaymentDesc6.Text = "";
                        txtPaymentOther.Text = "";
                        if (list != null && list.Count > 0)
                        {
                            RenewInsuranceReceiptRevisionDetailData tmp = list.Where(r => r.slm_Seq == 1).FirstOrDefault();
                            if (tmp != null)
                            {
                                chkPaymentDesc1.Checked = tmp.slm_Selected == "Y" ? true : false;
                                txtPaymentDesc1.Text = tmp.slm_RecAmount == null ? "" : tmp.slm_RecAmount.Value.ToString("#,##0.00");
                                txtPaymentDesc1.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc1.Enabled = false;
                            }

                            tmp = list.Where(r => r.slm_Seq == 2).FirstOrDefault();
                            if (tmp != null)
                            {
                                chkPaymentDesc2.Checked = tmp.slm_Selected == "Y" ? true : false;
                                txtPaymentDesc2.Text = tmp.slm_RecAmount == null ? "" : tmp.slm_RecAmount.Value.ToString("#,##0.00");
                                txtPaymentDesc2.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc2.Enabled = false;
                            }

                            tmp = list.Where(r => r.slm_Seq == 3).FirstOrDefault();
                            if (tmp != null)
                            {
                                chkPaymentDesc3.Checked = tmp.slm_Selected == "Y" ? true : false;
                                txtPaymentDesc3.Text = tmp.slm_RecAmount == null ? "" : tmp.slm_RecAmount.Value.ToString("#,##0.00");
                                txtPaymentDesc3.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc3.Enabled = false;
                            }

                            tmp = list.Where(r => r.slm_Seq == 4).FirstOrDefault();
                            if (tmp != null)
                            {
                                chkPaymentDesc4.Checked = tmp.slm_Selected == "Y" ? true : false;
                                txtPaymentDesc4.Text = tmp.slm_RecAmount == null ? "" : tmp.slm_RecAmount.Value.ToString("#,##0.00");
                                txtPaymentDesc4.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc4.Enabled = false;
                            }

                            tmp = list.Where(r => r.slm_Seq == 5).FirstOrDefault();
                            if (tmp != null)
                            {
                                chkPaymentDesc5.Checked = tmp.slm_Selected == "Y" ? true : false;
                                txtPaymentDesc5.Text = tmp.slm_RecAmount == null ? "" : tmp.slm_RecAmount.Value.ToString("#,##0.00");
                                txtPaymentDesc5.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc5.Enabled = false;
                            }

                            tmp = list.Where(r => r.slm_Seq == 6).FirstOrDefault();
                            if (tmp != null)
                            {
                                chkPaymentDesc6.Checked = tmp.slm_Selected == "Y" ? true : false;
                                txtPaymentDesc6.Text = tmp.slm_RecAmount == null ? "" : tmp.slm_RecAmount.Value.ToString("#,##0.00");
                                txtPaymentOther.Text = tmp.slm_PaymentOtherDesc;
                                txtPaymentDesc6.Enabled = true;
                                txtPaymentOther.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc6.Enabled = false;
                                txtPaymentOther.Enabled = false;
                            }
                        }
                        else
                        {

                            List<RenewInsuranceReceiptDetailData> listr = data.ReceiptDetailList.Where(rd => rd.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(slm_RenewInsuranceReceiptId)).ToList();
                            List<RenewInsuranceReceiptDetailData> tmp = listr.Where(r => r.slm_Seq == 1).ToList();

                            if (tmp != null && tmp.Count > 0)
                            {

                                RenewInsuranceReceiptDetailData toptmp = tmp.FirstOrDefault();

                                chkPaymentDesc1.Checked = true;
                                txtPaymentDesc1.Text = (tmp.Sum(x => x.slm_RecAmount) ?? 0m) == 0m ? "" : (tmp.Sum(x => x.slm_RecAmount) ?? 0m).ToString("#,##0.00");
                                txtPaymentDesc1.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc1.Enabled = false;
                            }

                            tmp = listr.Where(r => r.slm_Seq == 2).ToList();
                            if (tmp != null && tmp.Count > 0)
                            {
                                RenewInsuranceReceiptDetailData toptmp = tmp.FirstOrDefault();

                                chkPaymentDesc2.Checked = true;
                                txtPaymentDesc2.Text = (tmp.Sum(x => x.slm_RecAmount) ?? 0m) == 0m ? "" : (tmp.Sum(x => x.slm_RecAmount) ?? 0m).ToString("#,##0.00");
                                txtPaymentDesc2.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc2.Enabled = false;
                            }

                            tmp = listr.Where(r => r.slm_Seq == 3).ToList();
                            if (tmp != null && tmp.Count > 0)
                            {
                                RenewInsuranceReceiptDetailData toptmp = tmp.FirstOrDefault();

                                chkPaymentDesc3.Checked = true;
                                txtPaymentDesc3.Text = (tmp.Sum(x => x.slm_RecAmount) ?? 0m) == 0m ? "" : (tmp.Sum(x => x.slm_RecAmount) ?? 0m).ToString("#,##0.00");
                                txtPaymentDesc3.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc3.Enabled = false;
                            }

                            tmp = listr.Where(r => r.slm_Seq == 4).ToList();
                            if (tmp != null && tmp.Count > 0)
                            {
                                RenewInsuranceReceiptDetailData toptmp = tmp.FirstOrDefault();

                                chkPaymentDesc4.Checked = true;
                                txtPaymentDesc4.Text = (tmp.Sum(x => x.slm_RecAmount) ?? 0m) == 0m ? "" : (tmp.Sum(x => x.slm_RecAmount) ?? 0m).ToString("#,##0.00");
                                txtPaymentDesc4.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc4.Enabled = false;
                            }

                            tmp = listr.Where(r => r.slm_Seq == 5).ToList();
                            if (tmp != null && tmp.Count > 0)
                            {
                                RenewInsuranceReceiptDetailData toptmp = tmp.FirstOrDefault();

                                chkPaymentDesc5.Checked = true;
                                txtPaymentDesc5.Text = (tmp.Sum(x => x.slm_RecAmount) ?? 0m) == 0m ? "" : (tmp.Sum(x => x.slm_RecAmount) ?? 0m).ToString("#,##0.00");
                                txtPaymentDesc5.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc5.Enabled = false;
                            }

                            tmp = listr.Where(r => r.slm_Seq == 6).ToList();
                            if (tmp != null && tmp.Count > 0)
                            {
                                chkPaymentDesc6.Checked = true;
                                txtPaymentDesc6.Text = (tmp.Sum(x => x.slm_RecAmount) ?? 0m) == 0m ? "" : (tmp.Sum(x => x.slm_RecAmount) ?? 0m).ToString("#,##0.00");
                                txtPaymentDesc6.Enabled = true;
                                txtPaymentOther.Enabled = true;
                            }
                            else
                            {
                                txtPaymentDesc6.Enabled = false;
                                txtPaymentOther.Enabled = false;
                            }
                        }
                    }

                    decimal sum = 0m;
                    sum = AppUtil.SafeDecimal(txtPaymentDesc1.Text) + AppUtil.SafeDecimal(txtPaymentDesc2.Text) + AppUtil.SafeDecimal(txtPaymentDesc3.Text) + AppUtil.SafeDecimal(txtPaymentDesc4.Text) + AppUtil.SafeDecimal(txtPaymentDesc5.Text) + AppUtil.SafeDecimal(txtPaymentDesc6.Text);
                    txtPaymentDescTotal.Text = sum.ToString("#,##0.00");

                    pnReceipt.Update();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void gvProblem_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    DropDownList list = (DropDownList)e.Row.FindControl("cmbFixTypeFlag");
                    HiddenField value = (HiddenField)e.Row.FindControl("hidFixTypeFlag");
                    list.SelectedValue = value.Value.ToString();

                    if (value.Value.ToString() == "")
                    {
                        ((Button)e.Row.FindControl("btnReceipt")).Enabled = false;
                    }

                    if (value.Value.ToString() == "2" && list.Enabled)
                    {
                        ((TextBox)e.Row.FindControl("txtResponseDetail")).Enabled = true;
                        ((TextBox)e.Row.FindControl("txtRemark")).Enabled = true;
                    }
                    else
                    {
                        ((TextBox)e.Row.FindControl("txtResponseDetail")).Enabled = false;
                        ((TextBox)e.Row.FindControl("txtRemark")).Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void cmbStatus_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {              
                DropDownList ddl = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddl.Parent.Parent;

                if (ddl.SelectedValue != "")
                {
                    ((Button)row.FindControl("btnReceipt")).Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void cmbFixTypeFlag_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
                DropDownList ddl = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddl.Parent.Parent;


                if (ddl.SelectedValue != "")
                {
                    if (ddl.SelectedValue == "2")
                    {
                        ((TextBox)row.FindControl("txtResponseDetail")).Enabled = true;
                        ((TextBox)row.FindControl("txtRemark")).Enabled = true;

                    }
                    else
                    {
                        ((TextBox)row.FindControl("txtResponseDetail")).Enabled = false;
                        ((TextBox)row.FindControl("txtRemark")).Enabled = false;
                        ((TextBox)row.FindControl("txtResponseDetail")).Text = "";
                        ((TextBox)row.FindControl("txtRemark")).Text = "";
                    }

                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void txtPaymentDesc_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                
                decimal sum = 0m;
                sum = AppUtil.SafeDecimal(txtPaymentDesc1.Text) + AppUtil.SafeDecimal(txtPaymentDesc2.Text) + AppUtil.SafeDecimal(txtPaymentDesc3.Text) + AppUtil.SafeDecimal(txtPaymentDesc4.Text) + AppUtil.SafeDecimal(txtPaymentDesc5.Text) + AppUtil.SafeDecimal(txtPaymentDesc6.Text);

                var textChange = ((TextBox)sender).ID;

                if ("txtPaymentDesc1" == textChange)
                {
                    txtPaymentDesc1.Text = AppUtil.SafeDecimal(txtPaymentDesc1.Text).ToString("#,##0.00");
                }

                if ("txtPaymentDesc2" == textChange)
                {
                    txtPaymentDesc2.Text = AppUtil.SafeDecimal(txtPaymentDesc2.Text).ToString("#,##0.00");
                }

                if ("txtPaymentDesc3" == textChange)
                {
                    txtPaymentDesc3.Text = AppUtil.SafeDecimal(txtPaymentDesc3.Text).ToString("#,##0.00");
                }

                if ("txtPaymentDesc4" == textChange)
                {
                    txtPaymentDesc4.Text = AppUtil.SafeDecimal(txtPaymentDesc4.Text).ToString("#,##0.00");
                }

                if ("txtPaymentDesc5" == textChange)
                {
                    txtPaymentDesc5.Text = AppUtil.SafeDecimal(txtPaymentDesc5.Text).ToString("#,##0.00");
                }

                if ("txtPaymentDesc6" == textChange)
                {
                    txtPaymentDesc6.Text = AppUtil.SafeDecimal(txtPaymentDesc6.Text).ToString("#,##0.00");
                }

                txtPaymentDescTotal.Text = sum.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }
        #endregion

        protected void btnReceive_Click(object sender, EventArgs e)
        {
            try
            {
                string selPolicy = "";

                if (rbInsNameTh_cur.Checked)
                {
                    selPolicy = "cur";
                }
                else if (rbInsNameTh_pro1.Checked)
                {
                    selPolicy = "pro1";
                }
                else if (rbInsNameTh_pro2.Checked)
                {
                    selPolicy = "pro2";
                }
                else if (rbInsNameTh_pro3.Checked)
                {
                    selPolicy = "pro3";
                }
                if (!CheckPolicy(selPolicy))
                {
                    return;
                }

                //var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);

                var staff = GetCurrentStaff();
                if (Type == "1")        //Motor Renew
                {
                    if (SaveAll(false, true) == false) // refresh ค่าที่เลือกใหม่ (กรณีมีการเปลี่นยแปลงค่าก่อนกดรับแจ้ง)
                    {
                        return;
                    }

                    PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                    PreleadCompareDataCollection current = new PreleadCompareDataCollection();
                    if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                    {
                        current = pg.PreleadCompareDataCollectionMain;
                    }
                    else
                    {
                        if (pg.PreleadCompareDataCollections != null)
                        {
                            foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                            {
                                if (p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                                    current = p;
                            }
                        }
                    }

                    if (IsOwnerOrDelegate == null)
                    {
                        IsOwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(current.RenewIns.slm_TicketId, HttpContext.Current.User.Identity.Name);
                    }
                    if (IsSupervisor == null)
                    {
                        IsSupervisor = ActivityLeadBiz.CheckSupervisor(HttpContext.Current.User.Identity.Name);
                    }

                    if (cmbPaymentmethod.SelectedValue == "1")
                    {
                        bool Receipt = ActivityLeadBiz.checkPaid(current.RenewIns.slm_TicketId);
                        //bool supervisor = ActivityLeadBiz.CheckSupervisor(HttpContext.Current.User.Identity.Name);

                        if (Receipt || IsOverideUser(staff))
                        {
                            // ชำระมาแล้ว หรือ role เทพ อนุญาต
                            SaveReceive(staff, IsOwnerOrDelegate.Value, IsSupervisor.Value);
                            DoBindGridview(0);
                        }
                        else
                        {
                            // ชำระเต็ม และไม่มีการชำระ
                            chkClaim.Checked = false;
                            upPopupReceive.Update();
                        }
                    }
                    else
                    {
                        bool headerOfOwnerOrDelegate = ActivityLeadBiz.CheckHeaderOwnerOrDelegate(current.RenewIns.slm_TicketId, HttpContext.Current.User.Identity.Name);

                        //bool OwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(current.RenewIns.slm_TicketId, HttpContext.Current.User.Identity.Name);
                        //bool supervisor = ActivityLeadBiz.CheckSupervisor(HttpContext.Current.User.Identity.Name);

                        chkClaim.Checked = IsSupervisor.Value;

                        if (IsOwnerOrDelegate.Value || headerOfOwnerOrDelegate || IsSupervisor.Value)
                        {
                            upPopupReceive.Update();
                            SaveReceive(staff, IsOwnerOrDelegate.Value, IsSupervisor.Value);
                            DoBindGridview(0);
                        }
                        else
                        {
                            upPopupReceive.Update();
                            //var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                            if (staff != null)
                            {
                                if (IsOverideUser(staff))
                                {
                                    SaveReceive(staff, IsOwnerOrDelegate.Value, IsSupervisor.Value);
                                    DoBindGridview(0);
                                }
                            }
                        }
                    }

                    //UpdateStatusDesc("", current.RenewIns.slm_UpdatedDate.Value);     byDev
                }
                else
                {
                    //Type 2, 3 (KK MotorBox, พรบ.เดี่ยว)
                    SaveAll(false, true); //Refesh Data 
                    SaveReceive(staff, null, null);
                    DoBindGridview(0);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        protected void btnReceiveAct_Click(object sender, EventArgs e)
        {
            try
            {
                string selAct = "";
                //string selPolicy = "";
                if (rbAct_pro1.Checked)
                {
                    selAct = "act1";
                }
                else if (rbAct_pro2.Checked)
                {
                    selAct = "act2";
                }
                else if (rbAct_pro3.Checked)
                {
                    selAct = "act3";
                }

                if (CheckAct(selAct) && CheckActPurchaseDateTime())
                {
                    SaveReceiveAct();
                    DoBindGridview(0);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        private bool CheckActPurchaseDateTime()
        {
            try
            {
                DateTime startCoverDate = new DateTime();
                if (rbAct_pro1.Checked)
                {
                    startCoverDate = txtActStartCoverDateAct_pro1.DateValue;
                }
                else if (rbAct_pro2.Checked)
                {
                    startCoverDate = txtActStartCoverDateAct_pro2.DateValue;
                }
                else if (rbAct_pro3.Checked)
                {
                    startCoverDate = txtActStartCoverDateAct_pro3.DateValue;
                }
                else
                {
                    return true;
                }

                string temp = SLMConstant.ActPurchaseTime.Replace(":", "");
                int currentTime = int.Parse(DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00"));
                int checkTime = int.Parse(temp);
                DateTime currentDate = DateTime.Now;

                if (currentTime < checkTime)    //ก่อน 16:30
                {
                    if (startCoverDate.Date < currentDate.Date)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('วันเริ่มต้น พรบ.ต้องมากกว่าหรือเท่ากับวันปัจจุบัน');", true);
                        return false;
                    }
                }
                else
                {
                    if (startCoverDate.Date <= currentDate.Date)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('วันเริ่มต้น พรบ.ต้องมากกว่าวันปัจจุบัน');", true);
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        protected void btnSaveReceive_Click(object sender, EventArgs e)
        {
            try
            {
                var staff = GetCurrentStaff();  //StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                SaveReceive(staff, null, null);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCloseReceive_Click(object sender, EventArgs e)
        {
            mpePopupReceive.Hide();
        }

        protected void SaveReceive(StaffData staff, bool? OwnerOrDelegate, bool? supervisor)
        {
            try
            {
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                PreleadCompareDataCollection current = new PreleadCompareDataCollection();
                if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                {
                    current = pg.PreleadCompareDataCollectionMain;
                }
                else
                {
                    if (pg.PreleadCompareDataCollections != null)
                    {
                        foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                        {
                            if (p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                                current = p;
                        }
                    }
                }

                string r = null;
                try
                {
                    r = ActivityLeadBiz.getReceiveNo(current.RenewIns.slm_TicketId, HttpContext.Current.User.Identity.Name);
                    _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + (TicketId != null ? TicketId : (" PreleadId " + PreLeadId)) + " getReceiveNo : " + r ?? "null");
                }
                catch
                {
                    throw new Exception("ไม่พบเลขที่รับแจ้ง");
                }

                if (r == "" || r == null)
                {
                    throw new Exception("ไม่พบเลขที่รับแจ้ง");
                }
                else
                {
                    txtReceiveNo.Text = r;
                    DateTime receiveDate = DateTime.Now;
                    current.RenewIns.slm_ReceiveNo = r;
                    current.RenewIns.slm_ReceiveDate = receiveDate;
                    current.RenewIns.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                    current.FlagFirstReceivePolicy = true;

                    ActivityLeadBiz.SaveReceivePolicy(current, HttpContext.Current.User.Identity.Name);
                    txtReceiveDate.Text = ConvertToString(receiveDate);

                    // update receiveno for act report, settle claim report
                    ActivityLeadBiz.UpdateActNotifyReport(r, current.RenewIns.slm_TicketId);
                    ActivityLeadBiz.UpdateSettleClaimReport(r, receiveDate, current.RenewIns.slm_TicketId, SettleClaimReportId);

                    // อัพเดทสถานะที่หน้าจอหลักก่อน เพื่อ CAR จะได้ไปเอาสถานะมาถูก
                    UpdatedDataChanged("", current.RenewIns.slm_UpdatedDate.Value);

                    // บันทึกข้อมูลลง CAR
                    var plp = current.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();
                    var acp = current.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();

                    AppUtil.CreateCASActivityLog(this.Page, current.RenewIns.slm_TicketId, current.RenewIns.slm_RenewInsureId,
                        current.ComparePromoList != null && current.ComparePromoList.Where(p => p.slm_Selected == true).Count() > 0,
                        current.ActPromoList != null && current.ActPromoList.Where(a => a.slm_ActPurchaseFlag == true).Count() > 0,
                        null, //plp == null ? 0 : plp.Select(a => a.Amount).FirstOrDefault(), // current_paid_policy
                        null, //acp == null ? 0 : acp.Select(a => a.Amount).FirstOrDefault(), // current_paid_act
                        plp == null ? null : (decimal?)plp.Sum(a => a.Amount), // total_paid_policy
                        acp == null ? null : (decimal?)acp.Sum(a => a.Amount), staff); // total_paid_act

                    checkClaim(ref current); // update claim data (object)
                    BindClaim(current);     // update claim section display (UI)

                    btnReceive.Enabled = false;

                    var i = 1;
                    var notFixProblem = current.ProblemList.Where(p => p.slm_FixTypeFlag == null
                        || p.slm_FixTypeFlag != "2"
                        || (p.slm_FixTypeFlag == "2" && p.slm_Export_Flag.GetValueOrDefault(false) == false));

                    foreach (PreleadCompareData pro in current.ComparePromoList)
                    {
                        //var problem = ActivityLeadBiz.getProblem(current.RenewIns.slm_TicketId);
                        if (notFixProblem != null && notFixProblem.Count() > 0)
                        {
                            ((Button)this.FindControlRecursive("imgDel_pro" + i)).Enabled = true;
                        }
                        else if (pro.slm_Selected == null ? false : pro.slm_Selected.Value)
                        {
                            ((Button)this.FindControlRecursive("imgDel_pro" + i)).Enabled = false;
                        }
                        else
                        {
                            if (current.ActPromoList != null)
                            {
                                var problem = ActivityLeadBiz.getProblem(current.RenewIns.slm_TicketId);
                                foreach (PreleadCompareActData act in current.ActPromoList)
                                {
                                    if ((act.slm_ActPurchaseFlag == null ? false : act.slm_ActPurchaseFlag.Value) && (pro.slm_PromotionId == null ? 0 : pro.slm_PromotionId.Value) != 0 && (act.slm_PromotionId == null ? 0 : act.slm_PromotionId.Value) == (pro.slm_PromotionId == null ? 0 : pro.slm_PromotionId.Value))
                                    {
                                        if (problem != null && problem.Where(p => p.slm_FixTypeFlag != "2"
                                            || (p.slm_FixTypeFlag == "2" && p.slm_Export_Flag.GetValueOrDefault(false) == false)).Count() > 0)
                                            // don't have any pendign problem => freeze as it should 
                                            ((Button)this.FindControlRecursive("imgDel_pro" + i)).Enabled = false;
                                    }
                                }
                            }
                        }
                        i++;
                    }

                    // ลง Owner Logging ระงับเคลม 
                    var username = HttpContext.Current.User.Identity.Name;

                    if (current.RenewIns.slm_ClaimFlag == "1") // "1" = ระงับเคลม
                    {
                        ActivityLeadBiz.InsertSettleClaimActivityLog(current, username);
                    }

                    bool? isOwnerDelegate = OwnerOrDelegate;
                    bool? isSupervisor = supervisor;
                    if (isOwnerDelegate == null)
                    {
                        isOwnerDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(current.RenewIns.slm_TicketId, username);
                    }
                    if (isSupervisor == null)
                    {
                        isSupervisor = ActivityLeadBiz.CheckSupervisor(username);
                    }
                    SlmScr004Biz biz = new SlmScr004Biz();
                    if (PolicyPurchasedFlag == null)
                    {
                        PolicyPurchasedFlag = biz.CheckMotorRenewPurchased(GetTicketIdActiveTab());
                    }

                    setReceiveIncentiveButton(staff, isOwnerDelegate.Value, isSupervisor.Value, PolicyPurchasedFlag.Value);

                    bindControl(current);
                    upIncentive.Update();
                    UpdatePanel1.Update();
                    UpdatePanel2.Update();
                    pnPromotionAll.Update();
                    upClaim.Update();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                AppUtil.ClientAlert(Page, ex.Message);
            }
        }

        protected void SaveReceiveAct()
        {
            try
            {
                if (SaveAll() == false)
                {
                    return;
                }

                if (PermissionDiscount)
                {
                    PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                    PreleadCompareDataCollection current = new PreleadCompareDataCollection();
                    if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                    {
                        current = pg.PreleadCompareDataCollectionMain;
                    }
                    else
                    {
                        if (pg.PreleadCompareDataCollections != null)
                        {
                            foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                            {
                                if (p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                                    current = p;
                            }
                        }
                    }

                    DateTime receiveDate = DateTime.Now;
                    current.RenewIns.slm_ActSendDate = receiveDate;
                    current.RenewIns.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                    current.FlagFirstReceiveAct = true;
                    ActivityLeadBiz.SaveReceiveAct(current, HttpContext.Current.User.Identity.Name);
                    var actSendDate = SlmScr004Biz.GetActSendDate(current.RenewIns.slm_TicketId);
                    if (actSendDate != null)
                    {
                        SlmScr004Biz.InsertActIssueReport(current.RenewIns.slm_TicketId, receiveDate, Page.User.Identity.Name);
                        txtActSendDate.Text = ConvertToString(receiveDate);
                    }

                    btnReceiveAct.Enabled = false;
                    UpdatedDataChanged("", current.RenewIns.slm_UpdatedDate.Value);

                    var i = 1;
                    foreach (PreleadCompareActData pro in current.ActPromoList)
                    {
                        if (pro.slm_ActPurchaseFlag == true)
                        {
                            ((Button)this.FindControlRecursive("imgDelAct_pro" + i)).Enabled = false;
                            int j = 1;
                            foreach (var policy in current.ComparePromoList)
                            {
                                if (policy.slm_PromotionId.GetValueOrDefault(0) > 0 && policy.slm_PromotionId == pro.slm_PromotionId)
                                    ((Button)this.FindControlRecursive("imgDel_pro" + j)).Enabled = false;
                                j++;
                            }
                        }

                        i++;
                    }

                    var plp = current.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();
                    var acp = current.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();

                    var staff = GetCurrentStaff();
                    AppUtil.CreateCASActivityLog(this.Page, current.RenewIns.slm_TicketId, current.RenewIns.slm_RenewInsureId,
                            current.ComparePromoList != null && current.ComparePromoList.Where(p => p.slm_Selected == true).Count() > 0,
                            current.ActPromoList != null && current.ActPromoList.Where(a => a.slm_ActPurchaseFlag == true).Count() > 0,
                            null,//plp == null ? 0 : plp.Select(a => a.Amount).FirstOrDefault(),
                            null,//acp == null ? 0 : acp.Select(a => a.Amount).FirstOrDefault(),
                            plp == null ? null : (decimal?)plp.Sum(a => a.Amount),
                            acp == null ? null : (decimal?)acp.Sum(a => a.Amount), staff);

                    bindControl(current);
                    upIncentive.Update();
                    UpdatePanel1.Update();
                    UpdatePanel2.Update();
                    pnPromotionAll.Update();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlert(Page, ex.Message);
            }
        }

        protected void btnIncentive_Click(object sender, EventArgs e)
        {
            try
            {
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                PreleadCompareDataCollection current = new PreleadCompareDataCollection();
                if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                {
                    current = pg.PreleadCompareDataCollectionMain;
                }
                else
                {
                    if (pg.PreleadCompareDataCollections != null)
                    {
                        foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                        {
                            if (p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                                current = p;
                        }
                    }
                }

                DateTime incentiveDate = DateTime.Now;
                current.FlagFirstReceivePolicy = true;

                ActivityLeadBiz.DoProcessIncentivePolicy(current, HttpContext.Current.User.Identity.Name, incentiveDate);
   
                // บันทึกข้อมูลลง CAR
                var plp = current.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();
                var acp = current.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();

                StaffData staff = GetCurrentStaff();
                AppUtil.CreateCASActivityLog(this.Page, current.RenewIns.slm_TicketId, current.RenewIns.slm_RenewInsureId,
                    current.ComparePromoList != null && current.ComparePromoList.Where(p => p.slm_Selected == true).Count() > 0,
                    current.ActPromoList != null && current.ActPromoList.Where(a => a.slm_ActPurchaseFlag == true).Count() > 0,
                    null,//plp == null ? 0 : plp.Select(a => a.Amount).FirstOrDefault(),
                    null,//acp == null ? 0 : acp.Select(a => a.Amount).FirstOrDefault(),
                    plp == null ? null : (decimal?)plp.Sum(a => a.Amount),
                    acp == null ? null : (decimal?)acp.Sum(a => a.Amount), staff);

                txtIncentiveDate.Text = ConvertToString(incentiveDate);
                current.RenewIns.slm_IncentiveDate = incentiveDate;
                UpdateStatusDesc("", current.RenewIns.slm_UpdatedDate.Value);
                btnIncentive.Enabled = false;
                DoBindGridview(new RenewInsureBiz(), 0);
                Session[SessionPrefix+"allData"] = pg;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }

        }

        private void CreateCASActivityLog(string ticketId, string PaymentType)
        {
            try
            {
                var data = CARServiceBiz.GetDataForCARLogService(ticketId, "");
                string preleadId = "";

                if (data != null)
                    preleadId = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";

                string incentiveValue = "";
                string incentiveActValue = "";
                decimal? paidPolicy = null;
                decimal? paidAct = null;

                SlmScr004Biz biz = new SlmScr004Biz();
                if (biz.CheckMotorRenewPurchased(ticketId))
                {
                    incentiveValue = data.IncentiveFlag == true ? "Y" : "N";
                    paidPolicy = new LeadInfoBiz().GetPolicyAmountPaid(ticketId, PaymentType);     //ประกัน
                }
                if (biz.CheckActPurchased(ticketId))
                {
                    incentiveActValue = data.IncentiveFlagAct == true ? "Y" : "N";
                    paidAct = new LeadInfoBiz().GetPolicyAmountPaid(ticketId, PaymentType);        //พรบ
                }

                //Activity Info
                List<Services.CARService.DataItem> actInfoList = new List<Services.CARService.DataItem>();
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Incentive ประกัน", DataValue = incentiveValue });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Incentive พรบ.", DataValue = incentiveActValue });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ค่าเบี้ยประกันที่ลูกค้าชำระรวม", DataValue = paidPolicy != null ? paidPolicy.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 4, DataLabel = "ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม", DataValue = paidAct != null ? paidAct.Value.ToString("#,##0.00") : "" });

                //Customer Info
                List<Services.CARService.DataItem> cusInfoList = new List<Services.CARService.DataItem>();
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = data != null ? data.CitizenId : "" });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = data != null ? data.CardTypeName : "" });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = data != null ? data.CustomerName : "" });

                //Product Info
                List<Services.CARService.DataItem> prodInfoList = new List<Services.CARService.DataItem>();
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = data != null ? data.ProductGroupName : "" });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = data != null ? data.ProductName : "" });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = data != null ? data.CampaignName : "" });

                //Contract Info
                List<Services.CARService.DataItem> contInfoList = new List<Services.CARService.DataItem>();
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = preleadId != "" ? "HP" : "" });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.LicenseNo : "" });

                //Officer Info
                var staff = GetCurrentStaff();
                List<Services.CARService.DataItem> offInfoList = new List<Services.CARService.DataItem>();
                offInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name });

                Services.CARService.CARServiceData logdata = new Services.CARService.CARServiceData()
                {
                    ReferenceNo = ticketId,
                    SecurityKey = preleadId != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey,
                    ServiceName = "CreateActivityLog",
                    SystemCode = preleadId != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM,           //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
                    TransactionDateTime = DateTime.Now,
                    ActivityInfoList = actInfoList,
                    CustomerInfoList = cusInfoList,
                    ProductInfoList = prodInfoList,
                    ContractInfoList = contInfoList,
                    OfficerInfoList = offInfoList,
                    ActivityDateTime = DateTime.Now,
                    CampaignId = data != null ? data.CampaignId : "",
                    ChannelId = data.ChannelId,
                    PreleadId = preleadId,
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
                {
                    logdata.CIFSubscriptionTypeId = data.CBSSubScriptionTypeId;
                }
                    
                Services.CARService.CreateActivityLog(logdata);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
            }
        }

        protected void btnIncentiveAct_Click(object sender, EventArgs e)
        {
            try
            {
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                PreleadCompareDataCollection current = new PreleadCompareDataCollection();
                if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                {
                    current = pg.PreleadCompareDataCollectionMain;
                }
                else
                {
                    if (pg.PreleadCompareDataCollections != null)
                    {
                        foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                        {
                            if (p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                                current = p;
                        }
                    }
                }
                current.FlagManualActIncentive = true;
                current.FlagFirstReceiveAct = false;
                DateTime incentiveDate = DateTime.Now;
                ActivityLeadBiz.DoProcessIncentiveAct(current, HttpContext.Current.User.Identity.Name, incentiveDate);

                var plp = current.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();
                var acp = current.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();

                StaffData staff = GetCurrentStaff();
                AppUtil.CreateCASActivityLog(this.Page, current.RenewIns.slm_TicketId, current.RenewIns.slm_RenewInsureId,
                        current.ComparePromoList != null && current.ComparePromoList.Where(pp => pp.slm_Selected == true).Count() > 0,
                        current.ActPromoList != null && current.ActPromoList.Where(a => a.slm_ActPurchaseFlag == true).Count() > 0,
                        null, //plp == null ? 0 : plp.Select(a => a.Amount).FirstOrDefault(), // current_paid_policy
                        null, //acp == null ? 0 : acp.Select(a => a.Amount).FirstOrDefault(), // current_paid_act
                        plp == null ? null : (decimal?)plp.Sum(a => a.Amount), // total_paid_policy
                        acp == null ? null : (decimal?)acp.Sum(a => a.Amount), staff); // total_paid_act

                txtActIncentiveDate.Text = ConvertToString(incentiveDate);
                current.RenewIns.slm_ActIncentiveDate = incentiveDate;

                btnIncentiveAct.Enabled = false;
                DoBindGridview(new RenewInsureBiz(), 0);
                UpdateStatusDesc("", current.RenewIns.slm_UpdatedDate.Value);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }

        }

        protected void btnHistoryMain_Click(object sender, EventArgs e)
        {
            try
            {
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                string data = "";
                string ActiveTab = "";
                if (tabRenewInsuranceContainer.ActiveTab == null)
                {
                    ActiveTab = tabRenewInsuranceContainer.Tabs[0].ID;

                }
                else
                {
                    ActiveTab = tabRenewInsuranceContainer.ActiveTab.ID;
                }

                if (pg.PreleadCompareDataCollectionMain.keyTab == ActiveTab)
                {

                    data = pg.PreleadCompareDataCollectionMain.RenewIns.slm_TicketId;
                }
                else
                {
                    if (pg.PreleadCompareDataCollections != null)
                    {
                        foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                        {

                            if (p.keyTab == ActiveTab)
                            {
                                data = p.RenewIns.slm_TicketId;
                                break;
                            }
                        }
                    }
                }
                ucActRenewInsureSnap.HistoryTicketId = data;

                ucActRenewInsureSnap.show();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }

        public void UpdateMainData(string ticketId, string ownerBranchName, string ownerLeadName, string delegateBranchName, string delegateLeadName, string telNo1, string statusDesc, DateTime contactLatestDate, bool doUpdateOwnerLogging, string externalSubStatusDesc
            , string cardTypeId, string citizenId, string subStatusCode, DateTime? nextContactDate)
        {
            RenewInsureBiz biz = null;
            try
            {
                biz = new RenewInsureBiz();

                if (ucActRenewInsureContact != null)
                {
                    CheckActivityConfig(biz, ucActRenewInsureContact.ProductId.Trim(), ucActRenewInsureContact.LeadStatus);
                }
                    
                biz.CloseConnection();

                if (UpdatedMainDataChanged != null)
                {
                    UpdatedMainDataChanged(ticketId, ownerBranchName, ownerLeadName, delegateBranchName, delegateLeadName, telNo1, statusDesc, contactLatestDate, doUpdateOwnerLogging, externalSubStatusDesc, cardTypeId, citizenId, subStatusCode, nextContactDate);
                }

                // update save button status
                if (TicketId != "")
                {
                    string status = new SlmScr004Biz().GetStatus(TicketId);

                    if (status == SLMConstant.StatusCode.Close
                        || status == SLMConstant.StatusCode.Cancel
                        || status == SLMConstant.StatusCode.Reject)
                    {
                        // disable buttons if lead status in (08, 09, 10) -> no preleadcompareCollection
                        btnSave_PreleadData.Enabled = false;
                        btnAddBlankPolicy.Enabled = false;
                        btnCancelPolicy.Enabled = false;
                        btnAddBlankAct.Enabled = false;
                        btnCancelAct.Enabled = false;
                        Button1.Enabled = false;
                        Button2.Enabled = false;
                    }
                    if (status == SLMConstant.StatusCode.Close)
                    {
                        radioClaim.Enabled = false;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        public void RefreshPhoneCallList(List<PhoneCallHistoryData> list)
        {
            try
            {
                if (list != null)
                {
                    BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, list.ToArray(), 0);
                }
                else
                {
                    DoBindGridview(0);
                }
            }
            catch
            {
                throw;
            }
        }

        private PreleadCompareDataCollection getSelectTabData()
        {
            PreleadCompareDataCollection data = new PreleadCompareDataCollection();
            try
            {
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];

                if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                {
                    data = pg.PreleadCompareDataCollectionMain;
                }
                else
                {
                    if (pg.PreleadCompareDataCollections != null)
                    {
                        foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                        {

                            if (p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                            {
                                data = p;
                                break;
                            }
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
            return data;
        }

        protected void btnCopyContract_Click(object sender, EventArgs e)
        {
            try
            {
                PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];

                RenewInsuranceData datRenewIns = pg.PreleadCompareDataCollectionMain.RenewIns;
                List<RenewInsurancePaymentMainData> datPay = pg.PreleadCompareDataCollectionMain.PayMainList;

                if (datRenewIns != null)
                {
                    cmbPaymentmethod.SelectedValue = datRenewIns.slm_PayOptionId == null || datRenewIns.slm_PayOptionId == 0 ? "" : datRenewIns.slm_PayOptionId.Value.ToString();
                    textPeriod.Text = datRenewIns.slm_PolicyAmountPeriod == null || datRenewIns.slm_PolicyAmountPeriod == 0 ? "" : datRenewIns.slm_PolicyAmountPeriod.Value.ToString();
                    cmbPaymentType.SelectedValue = datRenewIns.slm_PolicyPayMethodId == null || datRenewIns.slm_PolicyPayMethodId == 0 ? "" : datRenewIns.slm_PolicyPayMethodId.Value.ToString();
                    cmbPolicyPayBranchCode.SelectedValue = datRenewIns.slm_PayBranchCode;

                    foreach (RenewInsurancePaymentMainData d in datPay)
                    {
                        ((TextDateMask)this.FindControlRecursive("tdmPaymentDate" + d.slm_Seq)).DateValue = d.slm_PaymentDate == null ? DateTime.MinValue : d.slm_PaymentDate.Value;
                        ((TextBox)this.FindControlRecursive("txtPeriod" + d.slm_Seq)).Text = d.slm_PaymentAmount == null || d.slm_PaymentAmount == 0 ? "" : d.slm_PaymentAmount.Value.ToString("#,##0.00");
                    }

                    cmbPaymentmethodAct.SelectedValue = datRenewIns.slm_ActPayOptionId == null || datRenewIns.slm_ActPayOptionId == 0 ? "" : datRenewIns.slm_ActPayOptionId.Value.ToString();
                    cmbPaymentTypeAct.SelectedValue = datRenewIns.slm_PolicyPayMethodId == null || datRenewIns.slm_ActPayMethodId == 0 ? "" : datRenewIns.slm_ActPayMethodId.Value.ToString();
                    cmbPayBranchCodeAct.SelectedValue = datRenewIns.slm_ActPayBranchCode;
                }


                setPaymentControlForCopy();
                pnPolicyPayment.Update();
                pnActPayment.Update();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void gvResultPromotion_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    PreleadCompareDataCollection pg = getSelectTabData();
                    SearchPromotionResult rowView = (SearchPromotionResult)e.Row.DataItem;
                    if (rowView.slm_PromotionId.ToString() == "0")
                    {

                        ((LinkButton)((GridViewRow)e.Row).FindControl("lblPositionNameAbb")).Visible = false;
                        ((Label)((GridViewRow)e.Row).FindControl("txtPositionNameAbb")).Visible = true;

                    }
                    else
                    {
                        ((LinkButton)((GridViewRow)e.Row).FindControl("lblPositionNameAbb")).Visible = true;
                        ((Label)((GridViewRow)e.Row).FindControl("txtPositionNameAbb")).Visible = false;
                    }

                    if (pg.ComparePromoList != null && pg.ComparePromoList.Count > 0)
                    {
                        foreach (PreleadCompareData p in pg.ComparePromoList)
                        {
                            if (p.slm_PromotionId != null)
                            {
                                if (p.slm_PromotionId.Value.ToString() == rowView.slm_PromotionId.ToString())
                                {
                                    ((CheckBox)((GridViewRow)e.Row).FindControl("chkSelect")).Checked = true;
                                    ((CheckBox)((GridViewRow)e.Row).FindControl("chkSelect")).Enabled = true; // enable ก่อน (เผื่อกรณียกเลิก)

                                    if (p.slm_Selected == true
                                        && (pg.RenewIns == null ? false : pg.RenewIns.slm_ReceiveDate != null)
                                        && (pg.ProblemList == null ?
                                         true : (pg.ProblemList.Count > 0 && pg.ProblemList.FirstOrDefault(pr => pr.slm_FixTypeFlag != "2") == null)))
                                    {
                                        ((CheckBox)((GridViewRow)e.Row).FindControl("chkSelect")).Enabled = false;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    if (pg.ActPromoList != null && pg.ActPromoList.Count > 0)
                    {
                        foreach (var item in pg.ActPromoList)
                        {
                            if (item.slm_PromotionId != null)
                            {
                                if (item.slm_PromotionId.ToString() == rowView.slm_PromotionId.Value.ToString())
                                {
                                    if (item.slm_ActPurchaseFlag == true
                                       && (pg.RenewIns == null ? false : pg.RenewIns.slm_ActSendDate != null)
                                       && (pg.ProblemList == null ?
                                           true : (pg.ProblemList.Count > 0 && pg.ProblemList.FirstOrDefault(prob => prob.slm_FixTypeFlag != "2") == null)))
                                    {
                                        ((CheckBox)((GridViewRow)e.Row).FindControl("chkSelect")).Enabled = false;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    if (rowView.slm_PromotionId != null)
                    {
                        if (rowView.slm_PromotionId.ToString() == "0")
                        {
                            ((CheckBox)((GridViewRow)e.Row).FindControl("chkSelect")).Visible = false;
                        }
                        else
                        {
                            ((CheckBox)((GridViewRow)e.Row).FindControl("chkSelect")).Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void btnCancelAct_OnClick(object sender, EventArgs e)
        {
            try
            {             
                hidCancelType.Value = "A";
                lblCancelDate.Text = DateTime.Now.ToString();
                cmbCancelReason.SelectedValue = "";
                upPopupCancel.Update();
                mpePopupCancel.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void CancelAct()
        {
            try
            {
                PreleadCompareDataCollectionGroup session = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];

                PreleadCompareDataCollection preComColle = getSelectTabData();

                preComColle.RenewIns.slm_ActPurchaseFlag = null;
                preComColle.RenewIns.slm_ActStartCoverDate = null;
                preComColle.RenewIns.slm_ActEndCoverDate = null;
                preComColle.RenewIns.slm_ActIssuePlace = null;
                preComColle.RenewIns.slm_ActIssueBranch = null;
                preComColle.RenewIns.slm_ActGrossPremium = null;
                preComColle.RenewIns.slm_ActComId = null;
                preComColle.RenewIns.slm_ActNetPremium = null;
                preComColle.RenewIns.slm_ActVat = null;
                preComColle.RenewIns.slm_ActStamp = null;
                preComColle.RenewIns.slm_ActPayMethodId = null;
                preComColle.RenewIns.slm_ActamountPeriod = null;
                preComColle.RenewIns.slm_ActPayOptionId = null;
                preComColle.RenewIns.slm_ActPayBranchCode = null;
                preComColle.RenewIns.slm_ActCancelId = cmbCancelReason.SelectedValue == "" ? null : (int?)AppUtil.SafeDecimal(cmbCancelReason.SelectedValue);
                preComColle.RenewIns.slm_ActCancelDate = Convert.ToDateTime(lblCancelDate.Text);
                preComColle.RenewIns.slm_ActDiscountAmt = null;
                preComColle.RenewIns.slm_ActDiscountPercent = null;
                preComColle.RenewIns.slm_ClaimFlag = null;
                preComColle.RenewIns.slm_ActNo = null;
                preComColle.RenewIns.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                preComColle.RenewIns.slm_UpdatedDate = DateTime.Now;
                preComColle.RenewIns.slm_ActSendDate = null;
                preComColle.RenewIns.slm_ActIncentiveDate = ActivityLeadBiz.ConvertStrDateTimeToDateTime(txtActIncentiveDate.Text);
                var countPolicy = preComColle.ComparePromoList.Where(cp => cp.slm_Selected != null && cp.slm_Selected == true).Count();

                if (countPolicy > 0)
                {
                    if (cmbPaymentTypeAct.SelectedValue == "3")
                    {
                        preComColle.RenewIns.slm_Need_CreditFlag = "Y";
                    }
                    else
                    {
                        preComColle.RenewIns.slm_Need_CreditFlag = null;
                    }

                    if (chkCardType2.Checked)
                    {
                        preComColle.RenewIns.slm_Need_50TawiFlag = "Y";
                    }
                    else
                    {
                        preComColle.RenewIns.slm_Need_50TawiFlag = null;
                    }
                }

                bool revertIncentive = false;
                if ((preComColle.RenewIns.slm_ActIncentiveDate ?? DateTime.MinValue).Month == DateTime.Now.Month
                    && (preComColle.RenewIns.slm_ActIncentiveDate ?? DateTime.MinValue).Year == DateTime.Now.Year)
                {
                    revertIncentive = true;
                    preComColle.RenewIns.slm_ActIncentiveFlag = null;
                    if (preComColle.ActPromoList != null && preComColle.ActPromoList.FirstOrDefault(p => p.slm_ActPurchaseFlag == true) != null)
                    {
                        preComColle.ActPromoList.FirstOrDefault(p => p.slm_ActPurchaseFlag == true).slm_ActPurchaseFlag = null;
                    }
                }

                ActivityLeadBiz.CancelAct(preComColle);

                if (revertIncentive)
                {
                    // บันทึกข้อมูลลง CAR
                    var plp = preComColle.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();
                    var acp = preComColle.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();

                    StaffData staff = GetCurrentStaff();
                    AppUtil.CreateCASActivityLog(this.Page, preComColle.RenewIns.slm_TicketId, preComColle.RenewIns.slm_RenewInsureId,
                        preComColle.ComparePromoList != null && preComColle.ComparePromoList.Where(pp => pp.slm_Selected == true).Count() > 0,
                        false,
                        null, 
                        null,
                        plp == null ? null : (decimal?)plp.Sum(a => a.Amount), // total_paid_policy
                        acp == null ? null : (decimal?)acp.Sum(a => a.Amount), staff); // total_paid_act
                }

                // Session update
                if (session.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                {
                    session.PreleadCompareDataCollectionMain = preComColle;
                }
                else
                {
                    if (session.PreleadCompareDataCollections != null)
                    {
                        var pc = session.PreleadCompareDataCollections.Where(e => e.keyTab == tabRenewInsuranceContainer.ActiveTab.ID).FirstOrDefault();
                        pc = preComColle;
                    }
                }

                Session[SessionPrefix+"allData"] = session;
                bindControl(preComColle);

                rbAct_pro1.Checked = false;
                rbAct_pro2.Checked = false;
                rbAct_pro3.Checked = false;
                txtActSendDate.Text = "";

                if (preComColle.RenewIns.slm_ActIncentiveDate != null
                                && ((DateTime)preComColle.RenewIns.slm_ActIncentiveDate).Month == DateTime.Now.Month
                                && ((DateTime)preComColle.RenewIns.slm_ActIncentiveDate).Year == DateTime.Now.Year)
                {
                    txtActIncentiveDate.Text = "";
                }
                
                preComColle.RenewIns.slm_ActIncentiveDate = null; // clear ค่าใน session ด้วย

                upIncentive.Update();
                btnCancelAct.Enabled = false;
                btnAddBlankAct.Enabled = true;
                disableReceipt(null, 0, 0, true);
                setDriverControl(preComColle);
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('ยกเลิก พรบ. สำเร็จ');", true);
                cmbActCancelReason.SelectedValue = cmbCancelReason.SelectedValue;
                txtActCancelDate.Text = lblCancelDate.Text;
                mpePopupCancel.Hide();
                //UpdateStatusDesc("", preComColle.RenewIns.slm_UpdatedDate.Value);     byDev
                DoBindGridview(new RenewInsureBiz(), 0);

                //if (UpdateLeadDataTab != null)
                //{
                //    UpdateLeadDataTab(this, EventArgs.Empty);
                //}
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void btnCancelPolicy_OnClick(object sender, EventArgs e)
        {
            try
            {            
                hidCancelType.Value = "P";
                lblCancelDate.Text = DateTime.Now.ToString();
                cmbCancelReason.SelectedValue = "";
                upPopupCancel.Update();
                mpePopupCancel.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void CancelPolicy()
        {
            try
            {
                PreleadCompareDataCollectionGroup p = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                bool revertIncentive = false;
                PreleadCompareDataCollection pg = getSelectTabData();

                pg.RenewIns.slm_CoverageTypeId = null;
                pg.RenewIns.slm_InsuranceComId = null;
                pg.RenewIns.slm_PolicyDiscountAmt = null;
                pg.RenewIns.slm_PolicyGrossVat = null;
                pg.RenewIns.slm_PolicyGrossStamp = null;
                pg.RenewIns.slm_PolicyGrossPremium = null;
                pg.RenewIns.slm_PolicyGrossPremiumTotal = null;
                pg.RenewIns.slm_PolicyCost = null;
                pg.RenewIns.slm_RepairTypeId = null;
                pg.RenewIns.slm_PolicyPayMethodId = null;
                pg.RenewIns.slm_PolicyAmountPeriod = null;
                pg.RenewIns.slm_Need_CreditFlag = null;
                pg.RenewIns.slm_Need_50TawiFlag = null;
                pg.RenewIns.slm_Need_DriverLicenseFlag = null;
                pg.RenewIns.slm_RemarkPayment = null;
                pg.RenewIns.slm_PolicyCostSave = null;
                pg.RenewIns.slm_Vat1Percent = null;
                pg.RenewIns.slm_DiscountPercent = null;
                pg.RenewIns.slm_Vat1PercentBath = null;
                pg.RenewIns.slm_PayOptionId = null;
                pg.RenewIns.slm_PayBranchCode = null;
                pg.RenewIns.slm_ClaimFlag = null;
                pg.RenewIns.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                pg.RenewIns.slm_UpdatedDate = DateTime.Now;
                pg.RenewIns.slm_PolicyCancelId = cmbCancelReason.SelectedValue == "" ? null : (int?)AppUtil.SafeDecimal(cmbCancelReason.SelectedValue);
                pg.RenewIns.slm_PolicyCancelDate = Convert.ToDateTime(lblCancelDate.Text);
                pg.RenewIns.slm_IncentiveDate = ActivityLeadBiz.ConvertStrDateTimeToDateTime(txtIncentiveDate.Text);
                foreach (var item in pg.ComparePromoList)
                {
                    item.slm_Selected = null;
                }

                if (pg.RenewIns.slm_IncentiveDate != null
                                && ((DateTime)pg.RenewIns.slm_IncentiveDate).Month == DateTime.Now.Month
                                && ((DateTime)pg.RenewIns.slm_IncentiveDate).Year == DateTime.Now.Year)
                {
                    pg.RenewIns.slm_IncentiveDate = null;
                    pg.RenewIns.slm_IncentiveFlag = null;
                    pg.RenewIns.slm_PolicyComBath = null;
                    pg.RenewIns.slm_PolicyComBathVat = null;
                    pg.RenewIns.slm_PolicyComBathIncentive = null;
                    pg.RenewIns.slm_PolicyOV1Bath = null;
                    pg.RenewIns.slm_PolicyOV1BathIncentive = null;
                    pg.RenewIns.slm_PolicyOV1BathVat = null;
                    pg.RenewIns.slm_PolicyOV2Bath = null;
                    pg.RenewIns.slm_PolicyOV2BathIncentive = null;
                    pg.RenewIns.slm_PolicyOV2BathVat = null;
                    pg.RenewIns.slm_PolicyIncentiveAmount = null;
                    pg.RenewIns.slm_IncentiveCancelDate = DateTime.Now;
                    pg.RenewIns.slm_PolicyReferenceNote = null;
                    pg.FlagFirstReceivePolicy = true;
                    pg.RenewIns.slm_UpdatedBy = HttpContext.Current.User.Identity.Name;
                    pg.RenewIns.slm_UpdatedDate = DateTime.Now;
                    revertIncentive = true;
                }

                if (pg.RenewIns.slm_ReceiveDate != null)
                {
                    pg.RenewIns.slm_ReceiveDate = null;
                    pg.RenewIns.slm_ReceiveNo = null;
                }

                var countAct = pg.ActPromoList.Where(cp => cp.slm_ActPurchaseFlag != null && cp.slm_ActPurchaseFlag == true).Count();

                if (countAct > 0)
                {
                    if (cmbPaymentType.SelectedValue == "3")
                    {
                        pg.RenewIns.slm_Need_CreditFlag = "Y";
                    }
                    else
                    {
                        pg.RenewIns.slm_Need_CreditFlag = null;
                    }

                    if (chkCardType.Checked)
                    {
                        pg.RenewIns.slm_Need_50TawiFlag = "Y";
                    }
                    else
                    {
                        pg.RenewIns.slm_Need_50TawiFlag = null;
                    }
                }
                ActivityLeadBiz.CancelPolicy(pg);

                // 45.6 บันทึกข้อมูลลง CAR
                // บันทึกข้อมูลลง CAR
                // บันทึกข้อมูลลง CAR
                var plp = pg.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();
                var acp = pg.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205").GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate }).Select(g => new { Amount = g.Sum(a => a.slm_RecAmount ?? 0), TranDate = g.Key.slm_TransDate }).OrderByDescending(a => a.TranDate).ToList();

                var staff = GetCurrentStaff();
                if (revertIncentive)
                {
                    AppUtil.CreateCASActivityLog(this.Page, pg.RenewIns.slm_TicketId, pg.RenewIns.slm_RenewInsureId,
                        pg.ComparePromoList != null && pg.ComparePromoList.Where(z => z.slm_Selected == true || z.slm_Selected == null).Count() > 0,
                        pg.ActPromoList.Count != 0 ? pg.ActPromoList.FirstOrDefault().slm_ActPurchaseFlag ?? false : false,
                        null, null,
                        (decimal?)plp.Sum(a => a.Amount),
                        acp == null ? null : (decimal?)acp.Sum(a => a.Amount), staff);
                }

                bindControl(pg);

                //UpdateStatusDesc("", pg.RenewIns.slm_UpdatedDate.Value);      byDev
                DoBindGridview(new RenewInsureBiz(), 0);

                if (p.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                {

                    p.PreleadCompareDataCollectionMain = pg;
                }
                else
                {
                    if (p.PreleadCompareDataCollections != null)
                    {
                        var pc = p.PreleadCompareDataCollections.Where(e => e.keyTab == tabRenewInsuranceContainer.ActiveTab.ID).FirstOrDefault();
                        pc = pg;
                    }
                }

                Session[SessionPrefix+"allData"] = p;

                cmbPolicyCancelReason.SelectedValue = cmbCancelReason.SelectedValue;
                txtPolicyCancelDate.Text = lblCancelDate.Text;
                mpePopupCancel.Hide();
                rbInsNameTh_cur.Checked = false;
                rbInsNameTh_pro1.Checked = false;
                rbInsNameTh_pro2.Checked = false;
                rbInsNameTh_pro3.Checked = false;
                btnCancelPolicy.Enabled = false;
                btnAddBlankPolicy.Enabled = true;


                #region Clear DATA
                txtReceiveNo.Text = "";
                txtReceiveDate.Text = "";
                if (pg.RenewIns.slm_IncentiveDate != null
                                && ((DateTime)pg.RenewIns.slm_IncentiveDate).Month == DateTime.Now.Month
                                && ((DateTime)pg.RenewIns.slm_IncentiveDate).Year == DateTime.Now.Year)
                {
                    txtIncentiveDate.Text = "";
                }
                #endregion
                upIncentive.Update();
                disableReceiptPolicy(pg, pg.RenewIns.slm_ReceiveNo);
                disableReceiptAct(pg, pg.RenewIns.slm_ReceiveNo, pg.RenewIns.slm_ActSendDate);

                var username = HttpContext.Current.User.Identity.Name;
                //bool OwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(pg.RenewIns.slm_TicketId, username);
                //bool supervisor = ActivityLeadBiz.CheckSupervisor(username);

                if (IsOwnerOrDelegate == null)
                {
                    IsOwnerOrDelegate = ActivityLeadBiz.CheckOwnerOrDelegate(pg.RenewIns.slm_TicketId, username);
                }
                if (IsSupervisor == null)
                {
                    IsSupervisor = ActivityLeadBiz.CheckSupervisor(username);
                }

                SlmScr004Biz biz = new SlmScr004Biz();
                if (PolicyPurchasedFlag == null)
                {
                    PolicyPurchasedFlag = biz.CheckMotorRenewPurchased(GetTicketIdActiveTab());
                }
                if (ActPurchasedFlag == null)
                {
                    ActPurchasedFlag = biz.CheckActPurchased(GetTicketIdActiveTab());
                }

                setReceiveActButton(pg, username, staff, IsOwnerOrDelegate.Value, IsSupervisor.Value, ActPurchasedFlag.Value);
                setReceivePolicyButton(pg, username, staff, IsOwnerOrDelegate.Value, IsSupervisor.Value, PolicyPurchasedFlag.Value);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlert(this, ex.Message);
            }
        }


        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (cmbCancelReason.SelectedValue != "")
                {
                    if (hidCancelType.Value == "A")
                    {
                        CancelAct();
                    }
                    else if (hidCancelType.Value == "P")
                    {
                        CancelPolicy();
                    }
                }
                else
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุเหตุผลการยกเลิก");
                    mpePopupCancel.Show();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCancelReceipt_OnClick(object sender, EventArgs e)
        {
            try
            {
                
                divEditReceipt.Visible = false;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCloseCancel_OnClick(object sender, EventArgs e)
        {
            mpePopupCancel.Hide();
        }

        protected void btnSaveReceipt_OnClick(object sender, EventArgs e)
        {
            try
            {
                
                if (ValidateAllData())
                {
                    PreleadCompareDataCollectionGroup pg = (PreleadCompareDataCollectionGroup)Session[SessionPrefix+"allData"];
                    List<RenewInsuranceReceiptRevisionDetailData> datalist = new List<RenewInsuranceReceiptRevisionDetailData>();
                    if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                    {

                        datalist = pg.PreleadCompareDataCollectionMain.ReceiptRevisionDetailList;
                    }
                    else
                    {
                        if (pg.PreleadCompareDataCollections != null)
                        {
                            foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                            {

                                if (p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                                {
                                    datalist = p.ReceiptRevisionDetailList;
                                    break;
                                }
                            }
                        }
                    }

                    if (pg.PreleadCompareDataCollectionMain.ReceiptRevisionDetailList == null)
                    {
                        pg.PreleadCompareDataCollectionMain.ReceiptRevisionDetailList = datalist;
                    }

                    List<RenewInsuranceReceiptRevisionDetailData> rList = new List<RenewInsuranceReceiptRevisionDetailData>();

                    if (datalist != null && datalist.Count > 0)
                    {
                        rList = datalist.Where(rd => rd.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value)).ToList();
                        RenewInsuranceReceiptRevisionDetailData r = new RenewInsuranceReceiptRevisionDetailData();
                        if (rList != null)
                        {
                            rList = PrepareForSaveReceiptRevision(rList);

                            foreach (RenewInsuranceReceiptRevisionDetailData rrrd in rList)
                            {
                                RenewInsuranceReceiptRevisionDetailData rChk = datalist.Where(p => p.slm_Seq == rrrd.slm_Seq && p.slm_RenewInsuranceReceiptId == (decimal?)AppUtil.SafeDecimal(hidRenewInsuranceReceiptId.Value)).FirstOrDefault();
                                if (rChk != null)
                                {
                                    datalist.Remove(rChk);
                                    datalist.Add(rrrd);
                                }
                                else
                                {
                                    datalist.Add(rrrd);
                                }
                            }
                        }
                        else
                        {
                            rList = PrepareForSaveReceiptRevision(rList);
                            foreach (RenewInsuranceReceiptRevisionDetailData rrrd in rList)
                            {
                                datalist.Add(rrrd);
                            }
                        }
                    }
                    else
                    {
                        rList = PrepareForSaveReceiptRevision(rList);
                        datalist = rList;
                    }

                    if (pg.PreleadCompareDataCollectionMain.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                    {
                        pg.PreleadCompareDataCollectionMain.ReceiptRevisionDetailList = datalist;
                        pg.PreleadCompareDataCollectionMain.EditReceiptFlag = true;
                    }
                    else
                    {
                        if (pg.PreleadCompareDataCollections != null)
                        {
                            foreach (PreleadCompareDataCollection p in pg.PreleadCompareDataCollections)
                            {
                                if (p.keyTab == tabRenewInsuranceContainer.ActiveTab.ID)
                                {
                                    p.ReceiptRevisionDetailList = datalist;
                                    p.EditReceiptFlag = true;
                                    break;
                                }
                            }
                        }
                    }

                    Session[SessionPrefix+"allData"] = pg;
                    divEditReceipt.Visible = false;
                    AppUtil.ClientAlertTabRenew(Page, "บันทึกแก้ไขใบเสร็จแล้ว");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void DiscountPer_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                
                int disPerPolicy = AppUtil.SafeInt(txtDiscountPercent.Text);
                decimal disBathPolicy = AppUtil.SafeDecimal(txtPolicyDiscountAmt.Text);
                decimal PolicyGrossPremiumTotalPolicy = AppUtil.SafeDecimal(txtPolicyGrossPremiumTotal.Text);
                int disPerAct = AppUtil.SafeInt(txtDiscountPercentAct.Text);
                decimal disBathAct = AppUtil.SafeDecimal(txtActDiscountAmt.Text);
                decimal PolicyGrossPremiumTotalAct = AppUtil.SafeDecimal(txtActGrossPremiumTotal.Text);

                int? percent = ActivityLeadBiz.getDiscountPercent(HttpContext.Current.User.Identity.Name);

                if (PolicyGrossPremiumTotalPolicy != 0 || PolicyGrossPremiumTotalAct != 0)
                {
                    var username = HttpContext.Current.User.Identity.Name;
                    string err;

                    if (!ActivityLeadBiz.CheckDiscount(username, PolicyGrossPremiumTotalPolicy, disBathPolicy, 0, "204", out err))
                    {
                        AppUtil.ClientAlert(Page, err);
                        txtPolicyDiscountAmt.Text = "";
                        txtDiscountPercent.Text = "";
                    }
                    else
                    {
                        txtPolicyDiscountAmt.Text = (PolicyGrossPremiumTotalPolicy * disPerPolicy / 100m).ToString("#,##0.00");
                        txtPolicyGrossPremium.Text = (PolicyGrossPremiumTotalPolicy - (PolicyGrossPremiumTotalPolicy * disPerPolicy / 100m)).ToString("#,##0.00");
                        txtPolicyDiffAmt.Text = (AppUtil.SafeDecimal(txtPolicyGrossPremium.Text) - AppUtil.SafeDecimal(txtPolicyRecAmount.Text)).ToString("#,##0.00");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlert(Page, ex.Message);
            }
        }

        protected void DiscountBath_OnTextChanged(object sender, EventArgs e)
        {
            try
            {              
                int disPerPolicy = AppUtil.SafeInt(txtDiscountPercent.Text);
                decimal disBathPolicy = AppUtil.SafeDecimal(txtPolicyDiscountAmt.Text);
                decimal PolicyGrossPremiumTotalPolicy = AppUtil.SafeDecimal(txtPolicyGrossPremiumTotal.Text);
                int disPerAct = AppUtil.SafeInt(txtDiscountPercentAct.Text);
                decimal disBathAct = AppUtil.SafeDecimal(txtActDiscountAmt.Text);
                decimal PolicyGrossPremiumTotalAct = AppUtil.SafeDecimal(txtActGrossPremiumTotal.Text);

                int? percent = ActivityLeadBiz.getDiscountPercent(HttpContext.Current.User.Identity.Name);

                if (PolicyGrossPremiumTotalPolicy != 0 || PolicyGrossPremiumTotalAct != 0)
                {
                    int disPerCur = (int)(disBathPolicy / PolicyGrossPremiumTotalPolicy * 100);
                    var username = HttpContext.Current.User.Identity.Name;
                    string err;

                    if (!ActivityLeadBiz.CheckDiscount(username, PolicyGrossPremiumTotalPolicy, disBathPolicy, 0, "204", out err))
                    {
                        AppUtil.ClientAlert(Page, err);
                        txtPolicyDiscountAmt.Text = "";
                        txtDiscountPercent.Text = "";
                    }
                    else
                    {
                        txtDiscountPercent.Text = ((int)disPerCur).ToString();
                        txtPolicyGrossPremium.Text = (PolicyGrossPremiumTotalPolicy - disBathPolicy).ToString("#,##0.00");
                        txtPolicyDiffAmt.Text = (AppUtil.SafeDecimal(txtPolicyGrossPremium.Text) - AppUtil.SafeDecimal(txtPolicyRecAmount.Text)).ToString("#,##0.00");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlert(Page, ex.Message);
            }
        }

        protected void DiscountPerAct_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                int disPerPolicy = AppUtil.SafeInt(txtDiscountPercent.Text);
                decimal disBathPolicy = AppUtil.SafeDecimal(txtPolicyDiscountAmt.Text);
                decimal PolicyGrossPremiumTotalPolicy = AppUtil.SafeDecimal(txtPolicyGrossPremiumTotal.Text);
                int disPerAct = AppUtil.SafeInt(txtDiscountPercentAct.Text);
                decimal disBathAct = AppUtil.SafeDecimal(txtActDiscountAmt.Text);
                decimal PolicyGrossPremiumTotalAct = AppUtil.SafeDecimal(txtActGrossPremiumTotal.Text);

                int? percent = ActivityLeadBiz.getDiscountPercent(HttpContext.Current.User.Identity.Name);

                if (PolicyGrossPremiumTotalAct != 0)
                {
                    var username = HttpContext.Current.User.Identity.Name;
                    string err;

                    if (!ActivityLeadBiz.CheckDiscount(username, PolicyGrossPremiumTotalAct, disBathAct, 0, "205", out err))
                    {
                        AppUtil.ClientAlert(Page, err);
                        txtActDiscountAmt.Text = "";
                        txtDiscountPercentAct.Text = "";
                    }
                    else
                    {
                        txtActDiscountAmt.Text = (PolicyGrossPremiumTotalAct * disPerAct / 100m).ToString("#,##0.00");
                        txtActGrossPremium.Text = (PolicyGrossPremiumTotalAct - (PolicyGrossPremiumTotalAct * disPerAct / 100m)).ToString("#,##0.00");
                        txtActDiffAmt.Text = (AppUtil.SafeDecimal(txtActGrossPremium.Text) - AppUtil.SafeDecimal(txtActRecAmount.Text)).ToString("#,##0.00");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlert(Page, ex.Message);
            }
        }

        protected void DiscountBathAct_OnTextChanged(object sender, EventArgs e)
        {
            try
            {               
                int disPerPolicy = AppUtil.SafeInt(txtDiscountPercent.Text);
                decimal disBathPolicy = AppUtil.SafeDecimal(txtPolicyDiscountAmt.Text);
                decimal PolicyGrossPremiumTotalPolicy = AppUtil.SafeDecimal(txtPolicyGrossPremiumTotal.Text);
                int disPerAct = AppUtil.SafeInt(txtDiscountPercentAct.Text);
                decimal disBathAct = AppUtil.SafeDecimal(txtActDiscountAmt.Text);
                decimal PolicyGrossPremiumTotalAct = AppUtil.SafeDecimal(txtActGrossPremiumTotal.Text);

                int? percent = ActivityLeadBiz.getDiscountPercent(HttpContext.Current.User.Identity.Name);

                if (PolicyGrossPremiumTotalAct != 0)
                {
                    var username = HttpContext.Current.User.Identity.Name;
                    string err;

                    if (!ActivityLeadBiz.CheckDiscount(username, PolicyGrossPremiumTotalAct, disBathAct, 0, "205", out err))
                    {
                        AppUtil.ClientAlert(Page, err);
                        txtActDiscountAmt.Text = "";
                        txtDiscountPercentAct.Text = "";
                    }
                    else
                    {
                        txtDiscountPercentAct.Text = ((int)disPerAct).ToString();
                        txtActGrossPremium.Text = (PolicyGrossPremiumTotalAct - disBathAct).ToString("#,##0.00");
                        txtActDiffAmt.Text = (AppUtil.SafeDecimal(txtActGrossPremium.Text) - AppUtil.SafeDecimal(txtActRecAmount.Text)).ToString("#,##0.00");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlert(Page, ex.Message);
            }
        }

        protected void chkPaymentDesc1_OnCheckedChange(object sender, EventArgs e)
        {
            try
            {               
                if (((CheckBox)sender).Checked)
                {
                    txtPaymentDesc1.Enabled = true;
                }
                else
                {
                    txtPaymentDesc1.Enabled = false;
                    txtPaymentDesc1.Text = "";
                    decimal sum = 0m;
                    sum = AppUtil.SafeDecimal(txtPaymentDesc1.Text) + AppUtil.SafeDecimal(txtPaymentDesc2.Text) + AppUtil.SafeDecimal(txtPaymentDesc3.Text) + AppUtil.SafeDecimal(txtPaymentDesc4.Text) + AppUtil.SafeDecimal(txtPaymentDesc5.Text) + AppUtil.SafeDecimal(txtPaymentDesc6.Text);
                    txtPaymentDescTotal.Text = sum.ToString("#,##0.00");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        protected void chkPaymentDesc2_OnCheckedChange(object sender, EventArgs e)
        {
            try
            {
                
                if (((CheckBox)sender).Checked)
                {
                    txtPaymentDesc2.Enabled = true;
                }
                else
                {
                    txtPaymentDesc2.Enabled = false;
                    txtPaymentDesc2.Text = "";
                    decimal sum = 0m;
                    sum = AppUtil.SafeDecimal(txtPaymentDesc1.Text) + AppUtil.SafeDecimal(txtPaymentDesc2.Text) + AppUtil.SafeDecimal(txtPaymentDesc3.Text) + AppUtil.SafeDecimal(txtPaymentDesc4.Text) + AppUtil.SafeDecimal(txtPaymentDesc5.Text) + AppUtil.SafeDecimal(txtPaymentDesc6.Text);
                    txtPaymentDescTotal.Text = sum.ToString("#,##0.00");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }

        protected void chkPaymentDesc3_OnCheckedChange(object sender, EventArgs e)
        {
            try
            {
                
                if (((CheckBox)sender).Checked)
                {
                    txtPaymentDesc3.Enabled = true;
                }
                else
                {
                    txtPaymentDesc3.Enabled = false;
                    txtPaymentDesc3.Text = "";
                    decimal sum = 0m;
                    sum = AppUtil.SafeDecimal(txtPaymentDesc1.Text) + AppUtil.SafeDecimal(txtPaymentDesc2.Text) + AppUtil.SafeDecimal(txtPaymentDesc3.Text) + AppUtil.SafeDecimal(txtPaymentDesc4.Text) + AppUtil.SafeDecimal(txtPaymentDesc5.Text) + AppUtil.SafeDecimal(txtPaymentDesc6.Text);
                    txtPaymentDescTotal.Text = sum.ToString("#,##0.00");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }

        protected void chkPaymentDesc4_OnCheckedChange(object sender, EventArgs e)
        {
            try
            {
                
                if (((CheckBox)sender).Checked)
                {
                    txtPaymentDesc4.Enabled = true;
                }
                else
                {
                    txtPaymentDesc4.Enabled = false;
                    txtPaymentDesc4.Text = "";
                    decimal sum = 0m;
                    sum = AppUtil.SafeDecimal(txtPaymentDesc1.Text) + AppUtil.SafeDecimal(txtPaymentDesc2.Text) + AppUtil.SafeDecimal(txtPaymentDesc3.Text) + AppUtil.SafeDecimal(txtPaymentDesc4.Text) + AppUtil.SafeDecimal(txtPaymentDesc5.Text) + AppUtil.SafeDecimal(txtPaymentDesc6.Text);
                    txtPaymentDescTotal.Text = sum.ToString("#,##0.00");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }

        protected void chkPaymentDesc5_OnCheckedChange(object sender, EventArgs e)
        {
            try
            {
                if (((CheckBox)sender).Checked)
                {
                    txtPaymentDesc5.Enabled = true;
                }
                else
                {
                    txtPaymentDesc5.Enabled = false;
                    txtPaymentDesc5.Text = "";
                    decimal sum = 0m;
                    sum = AppUtil.SafeDecimal(txtPaymentDesc1.Text) + AppUtil.SafeDecimal(txtPaymentDesc2.Text) + AppUtil.SafeDecimal(txtPaymentDesc3.Text) + AppUtil.SafeDecimal(txtPaymentDesc4.Text) + AppUtil.SafeDecimal(txtPaymentDesc5.Text) + AppUtil.SafeDecimal(txtPaymentDesc6.Text);
                    txtPaymentDescTotal.Text = sum.ToString("#,##0.00");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }

        protected void chkPaymentDesc6_OnCheckedChange(object sender, EventArgs e)
        {

            try
            {
                if (((CheckBox)sender).Checked)
                {
                    txtPaymentDesc6.Enabled = true;
                    txtPaymentOther.Enabled = true;
                }
                else
                {
                    txtPaymentDesc6.Enabled = false;
                    txtPaymentOther.Enabled = false;
                    txtPaymentDesc6.Text = "";
                    txtPaymentOther.Text = "";
                    decimal sum = 0m;
                    sum = AppUtil.SafeDecimal(txtPaymentDesc1.Text) + AppUtil.SafeDecimal(txtPaymentDesc2.Text) + AppUtil.SafeDecimal(txtPaymentDesc3.Text) + AppUtil.SafeDecimal(txtPaymentDesc4.Text) + AppUtil.SafeDecimal(txtPaymentDesc5.Text) + AppUtil.SafeDecimal(txtPaymentDesc6.Text);
                    txtPaymentDescTotal.Text = sum.ToString("#,##0.00");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }

        }

        protected void PromotionPageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchPromotionData(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertTabRenew(Page, message);
            }
        }

        private void DoSearchPromotionData(int pageIndex)
        {
            try
            {
                PreleadCompareDataCollection pg = getSelectTabData();
                List<SearchPromotionResult> data = null;
                if (PreLeadId != null && PreLeadId != "")
                {
                    int slm_PreLeadId = pg.Prelead.slm_Prelead_Id == null ? 0 : (int)pg.Prelead.slm_Prelead_Id.Value;

                    data = SlmScr999Biz.SearchPromotionList(slm_PreLeadId, cmbCarBrand2.SelectedValue, cmbCarName2.SelectedValue, cmbInsuranceType.SelectedValue, AppUtil.SafeDecimal(txtOD.Text), AppUtil.SafeDecimal(txtODRanking.Text), AppUtil.SafeDecimal(txtFT.Text), AppUtil.SafeDecimal(txtFTRanking.Text), txtInsNameSearch.Text);
                }
                else if (TicketId != null && TicketId != "")
                {
                    string slm_TicketId = pg.RenewIns.slm_TicketId;//== null ? 0 : (int)pg.Prelead.slm_TicketId.Value;
                    data = SlmScr999Biz.SearchPromotionListbyTicket(slm_TicketId, cmbCarBrand2.SelectedValue, cmbCarName2.SelectedValue, cmbInsuranceType.SelectedValue, AppUtil.SafeDecimal(txtOD.Text), AppUtil.SafeDecimal(txtODRanking.Text), AppUtil.SafeDecimal(txtFT.Text), AppUtil.SafeDecimal(txtFTRanking.Text), txtInsNameSearch.Text);
                }

                BindGridviewPromotion((SLM.Application.Shared.GridviewPageController)pcTopPromotion, data.ToArray(), pageIndex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindGridviewPromotion(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResultPromotion);
            pageControl.Update(items, pageIndex);
            upPromotion.Update();
        }

        protected void cmbBranchCodeDoc_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbBranchCodeDoc.SelectedValue != "")
                {
                    BranchData data = BranchBiz.GetBranch(cmbBranchCodeDoc.SelectedValue);


                    txtHouseNo.Text = data.slm_House_No;
                    txtMoo.Text = data.slm_Moo;
                    txtBuilding.Text = data.slm_Building;
                    txtHouseName.Text = data.slm_Village;
                    txtSoi.Text = data.slm_Soi;
                    txtStreet.Text = data.slm_Street;

                    cmbProvince.SelectedValue = data.slm_ProvinceId == null ? "" : data.slm_ProvinceId.ToString() == "0" ? "" : data.slm_ProvinceId.ToString();
                    if (data.slm_ProvinceId != null && data.slm_ProvinceId != 0)
                    {
                        string provinceValue = cmbProvince.SelectedItem.Value;

                        cmbDistinct.DataSource = SlmScr999Biz.ListDistinct(provinceValue);
                        cmbDistinct.DataTextField = "AmphurName";
                        cmbDistinct.DataValueField = "AmphurId";
                        cmbDistinct.DataBind();
                        cmbDistinct.Items.Insert(0, new ListItem("", ""));

                        cmbThambol.Items.Clear();
                        cmbThambol.Items.Insert(0, new ListItem("", ""));
                        cmbThambol.SelectedIndex = -1;

                        //string distinctValue = cmbDistinct.SelectedItem.Value;
                        //cmbThambol.DataSource = SlmScr999Biz.ListTambol(distinctValue, provinceValue);
                        //cmbThambol.DataTextField = "TambolNameTh";
                        //cmbThambol.DataValueField = "TambolId";
                        //cmbThambol.DataBind();
                        //cmbThambol.Items.Insert(0, new ListItem("", ""));
                    }

                    AppUtil.SetComboValue(cmbDistinct, data.slm_AmphurId.ToString());
                    if (data.slm_AmphurId != null && data.slm_AmphurId != 0)
                    {
                        string provinceValue = cmbProvince.SelectedItem.Value;
                        string distinctValue = cmbDistinct.SelectedItem.Value;

                        cmbThambol.Items.Clear();
                        cmbThambol.SelectedIndex = -1;
                        cmbThambol.DataSource = SlmScr999Biz.ListTambol(distinctValue, provinceValue);
                        cmbThambol.DataTextField = "TambolNameTh";
                        cmbThambol.DataValueField = "TambolId";
                        cmbThambol.DataBind();
                        cmbThambol.Items.Insert(0, new ListItem("", ""));
                    }

                    AppUtil.SetComboValue(cmbThambol, data.slm_TambolId.ToString());
                    txtZipCode.Text = data.slm_Zipcode;
                    txtReceiver.Text = string.Format("{0} ({1})", kiatnakinBank, cmbBranchCodeDoc.SelectedItem.Text.Trim());
                }
                else
                {
                    txtReceiver.Text = kiatnakinBank;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        /* = = = = = = = = = = = Calculate Act Bath and Percent = = = = = = = = = = = = = */
        protected void calActPercent(object sender, EventArgs e)
        {
            try
            {
                
                var textChange = ((TextBox)sender).ID;

                if ("lblVat1PercentBathAct_pro1" == textChange)
                {
                    CalActPercentPro1(true);
                }
                else if ("lblVat1PercentBathAct_pro2" == textChange)
                {
                    CalActPercentPro2(true);
                }
                else if ("lblVat1PercentBathAct_pro3" == textChange)
                {
                    CalActPercentPro3(true);

                }
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private bool CalActPercentPro1(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro1.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro1.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro1.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro1.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro1.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro1.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro1.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro1.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;

                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro1.Text);

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {
                            tax1percent = 0;
                        }

                        lblActPersonType_pro1.Text = tax1percent.ToString("#,##0.00");

                        decimal discountBath = (netgross_premium - tax1percent) * ((AppUtil.SafeInt(lblVat1PercentBathAct_pro1.Text) / 100m)); // zz คำนวนจากเบี้ยสุทธิ
                        lblDiscountPercentAct_pro1.Text = discountBath.ToString("#,##0.00");

                        txtDiscountBathAct_pro1.Text = ((NetGrossPremium - tax1percent) - discountBath).ToString("#,##0.00");//zz

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (alert)
                {
                    AppUtil.ClientAlertTabRenew(this, ex.Message);
                }
                return false;
            }
        }

        private bool CalActPercentPro2(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro2.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro2.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro2.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro2.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro2.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro2.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro2.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro2.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;
                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro2.Text);

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {

                            tax1percent = 0;
                        }

                        lblActPersonType_pro2.Text = tax1percent.ToString("#,##0.00");

                        var discountBath = (netgross_premium - tax1percent) * ((AppUtil.SafeInt(lblVat1PercentBathAct_pro2.Text) / 100m));//zz คำนวนจากเบี้ยสุทธิ
                        lblDiscountPercentAct_pro2.Text = discountBath.ToString("#,##0.00");

                        txtDiscountBathAct_pro2.Text = ((NetGrossPremium - tax1percent) - discountBath).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlertTabRenew(this, ex.Message);
                return false;
            }
        }

        private bool CalActPercentPro3(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro3.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro3.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro3.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro3.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro3.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro3.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro3.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro3.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;
                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro3.Text);

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {
                            tax1percent = 0;
                        }

                        lblActPersonType_pro3.Text = tax1percent.ToString("#,##0.00");
                        
                        var discountBath = (netgross_premium - tax1percent) * ((AppUtil.SafeInt(lblVat1PercentBathAct_pro3.Text) / 100m)); //zz คำนวนจากเบี้ยสุทธิ
                        lblDiscountPercentAct_pro3.Text = discountBath.ToString("#,##0.00");

                        txtDiscountBathAct_pro3.Text = ((NetGrossPremium - tax1percent) - discountBath).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (alert)
                {
                    AppUtil.ClientAlertTabRenew(this, ex.Message);
                }
                return false;
            }
        }

        protected void calActBath(object sender, EventArgs e)
        {
            try
            {
                
                var textChange = ((TextBox)sender).ID;

                if ("lblDiscountPercentAct_pro1" == textChange)
                {
                    CalActBathPro1(true);
                }
                else if ("lblDiscountPercentAct_pro2" == textChange)
                {
                    CalActBathPro2(true);
                }
                else if ("lblDiscountPercentAct_pro3" == textChange)
                {
                    CalActBathPro3(true);

                }

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();", true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                AppUtil.ClientAlertTabRenew(this, ex.Message);
            }
        }

        private bool CalActBathPro1(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro1.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro1.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro1.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro1.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro1.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro1.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro1.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro1.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;
                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro1.Text);

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {
                            tax1percent = 0;
                        }

                        lblActPersonType_pro1.Text = tax1percent.ToString("#,##0.00");
                        var discountBath = AppUtil.SafeDecimal(lblDiscountPercentAct_pro1.Text);

                        lblVat1PercentBathAct_pro1.Text = (Math.Round((discountBath) / (netgross_premium - tax1percent) * 100, 2, MidpointRounding.AwayFromZero)).ToString("#,##0");
                        txtDiscountBathAct_pro1.Text = ((NetGrossPremium - tax1percent) - discountBath).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (alert)
                {
                    AppUtil.ClientAlertTabRenew(this, ex.Message);
                }
                return false;
            }
        }

        private bool CalActBathPro2(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro2.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro2.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro2.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro2.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro2.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro2.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro2.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro2.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;
                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro2.Text);

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {
                            tax1percent = 0;
                        }

                        lblActPersonType_pro2.Text = tax1percent.ToString("#,##0.00");
                        var discountBath = AppUtil.SafeDecimal(lblDiscountPercentAct_pro2.Text);

                        lblVat1PercentBathAct_pro2.Text = Math.Round((discountBath) / (netgross_premium - tax1percent) * 100, 2, MidpointRounding.AwayFromZero).ToString("#,##0");
                        txtDiscountBathAct_pro2.Text = ((NetGrossPremium - tax1percent) - discountBath).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlertTabRenew(this, ex.Message);
                return false;
            }
        }

        private bool CalActBathPro3(bool alert)
        {

            try
            {
                //เบี๊ยสุทธิเต็มปี
                decimal netgross_premium_full = AppUtil.SafeDecimal(lblActNetGrossPremiumFullAct_pro3.Text);
                //เบี๊ยสุทธิ
                decimal netgross_premium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro3.Text);

                //วันที่เริ่มต้น
                DateTime startDate = Convert.ToDateTime(txtActStartCoverDateAct_pro3.DateValue);
                DateTime endDate = Convert.ToDateTime(txtActEndCoverDateAct_pro3.DateValue);
                DateTime onYear = startDate.AddYears(1);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    if (startDate > endDate)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "วันที่เริ่มต้นต้องไม่มากวันที่สิ้นสุด");
                        }
                        return false;
                    }
                    else if (startDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่เริ่มต้น พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else if (endDate.Year < DateTime.Now.Year)
                    {
                        if (alert)
                        {
                            AppUtil.ClientAlertTabRenew(Page, "ปีที่สิ้นสุด พรบ ต้องมากกว่าเท่ากับปีปัจจุบัน");
                        }
                        return false;
                    }
                    else
                    {
                        int cDay;
                        if (onYear != endDate)
                        {
                            cDay = (endDate - startDate).Days;
                        }
                        else
                        {
                            cDay = 365;
                        }
                        //หาเบี๊ยสุทธิ
                        netgross_premium = ((cDay * netgross_premium_full) / constDaysOfYear);

                        //หาอากร
                        decimal GrossStamp = CalculateActDuty(netgross_premium);

                        //หาภาษี
                        decimal GrossVat = Math.Round(((netgross_premium + GrossStamp) * constVat) / 100m, 2);

                        //เบี๊ย พรบ. รวมภาษีอากร
                        decimal NetGrossPremium = Math.Round(netgross_premium + GrossStamp + GrossVat, 2);

                        lblActGrossPremiumAct_pro3.Text = netgross_premium.ToString("#,##0.00");
                        lblActGrossStampAct_pro3.Text = GrossStamp.ToString("#,##0.00");
                        lblActGrossVatAct_pro3.Text = GrossVat.ToString("#,##0.00");
                        lblActNetGrossPremiumAct_pro3.Text = NetGrossPremium.ToString("#,##0.00");

                        decimal tax1percent = 0;
                        decimal total_premium = AppUtil.SafeDecimal(txtDiscountBathAct_pro3.Text);

                        if (chkCardType2.Checked)
                        {
                            //ภาษี1% = ถ้า เบี้ยสุทธิ > 1000 บาท จะได้ (เบี้ยสุทธิ + อากร) / 100
                            //ถ้า เบี้ยสุทธิ < 1000 บาท จะคิดเป็น 0
                            //จำนวนส่วนลดบาท = (เบี้ย พรบ รวมภาษีอากร - ภาษี1%) – เบี้ย พรบ.ที่ต้องชำระ ถ้าเป็นจุดทศนิยมให้ปัดเศษเอาทศนิยม 2 ตำแหน่ง
                            //ส่วนลด(%) = (จำนวนส่วนลดบาท *100)/เบี้ย พรบ รวมภาษีอากร ถ้าเป็นจุดทศนิยมให้ปัดเศษขึ้นเป็นจำนวนเต็ม
                            if (netgross_premium > 1000)
                            {
                                tax1percent = (netgross_premium + GrossStamp) / 100m;
                            }
                            else
                            {
                                tax1percent = 0;
                            }
                        }
                        else
                        {
                            tax1percent = 0;
                        }

                        lblActPersonType_pro3.Text = tax1percent.ToString("#,##0.00");
                        var discountBath = AppUtil.SafeDecimal(lblDiscountPercentAct_pro3.Text);

                        lblVat1PercentBathAct_pro3.Text = Math.Round((discountBath) / (netgross_premium - tax1percent) * 100, 2, MidpointRounding.AwayFromZero).ToString("#,##0");
                        txtDiscountBathAct_pro3.Text = ((NetGrossPremium - tax1percent) - discountBath).ToString("#,##0.00");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (alert)
                {
                    AppUtil.ClientAlertTabRenew(this, ex.Message);
                }
                return false;
            }
        }


        protected void cmbPayOption_OnTextChanged(object sender, EventArgs e)
        {

            if (cmbActPayMethod.SelectedValue == "4")
            {
                cmbPayBranchCode.Enabled = true;
                cmbPayBranchCode.SelectedValue = "";
            }
            else
            {
                cmbPayBranchCode.Enabled = false;
            }
        }

        private bool ValidateCarDetail()
        {
            try
            {
                if (cmbCarBrand.SelectedItem.Value == "" || cmbCarName.SelectedItem.Value == ""
                    || cmbInsuranceCarType.SelectedItem.Value == "" || cmbProvinceRegis.SelectedItem.Value == "")
                {
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุ ยี่ห้อรถยนต์, รุ่นรถ, ประเภทรถ, จังหวัดที่จดทะเบียน ให้ครบถ้วน');", true);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }

        private bool CheckPolicy(string Type)
        {
            try
            {
                if (Type == "")
                {
                    return true;
                }
                if ("cur" == Type)
                {
                    if (lblCoverageType_cur.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุประเภทประกัน');", true);
                        return false;
                    }
                    else if (lblInsNameTh_cur.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุบริษัทประกัน');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Eff_Date_cur.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันเริ่มคุ้มครอง');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Exp_Date_cur.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันหมดอายุประกัน');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Eff_Date_cur.DateValue >= lblVoluntary_Policy_Exp_Date_cur.DateValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('วันเริ่มคุ้มครองต้องน้อยกว่าวันหมดอายุประกัน');", true);
                        return false;
                    }
                    else if (lblMaintanance_cur.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุประเภทการซ่อม');", true);
                        return false;
                    }
                    else if (lblNetpremium_cur.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยสุทธิ');", true);
                        return false;
                    }
                    else if (lblDuty_cur.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุอากร');", true);
                        return false;
                    }
                    else if (lblVat_amount_cur.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุภาษี');", true);
                        return false;
                    }
                    else if (lblVoluntary_Gross_Premium_cur.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยประกันรวมภาษีอากร');", true);
                        return false;
                    }
                    else if (txtTotal_Voluntary_Gross_Premium_cur.Text == "" || AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_cur.Text) <= 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยประกันที่ต้องชำระ');", true);
                        return false;
                    }
                }
                else if ("pro1" == Type)
                {
                    if (lblCoverageType_pro1.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุประเภทประกัน');", true);
                        return false;
                    }
                    else if (lblInsNameTh_pro1.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุบริษัทประกัน');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Eff_Date_pro1.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันเริ่มคุ้มครอง');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Exp_Date_pro1.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันหมดอายุประกัน');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Eff_Date_pro1.DateValue >= lblVoluntary_Policy_Exp_Date_pro1.DateValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('วันเริ่มคุ้มครองต้องน้อยกว่าวันหมดอายุประกัน');", true);
                        return false;
                    }
                    else if (lblMaintanance_pro1.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุประเภทการซ่อม');", true);
                        return false;
                    }
                    else if (lblNetpremium_pro1.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยสุทธิ');", true);
                        return false;
                    }
                    else if (lblDuty_pro1.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุอากร');", true);
                        return false;
                    }
                    else if (lblVat_amount_pro1.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุภาษี');", true);
                        return false;
                    }
                    else if (lblVoluntary_Gross_Premium_pro1.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยประกันรวมภาษีอากร');", true);
                        return false;
                    }
                    else if (txtTotal_Voluntary_Gross_Premium_pro1.Text == "" || AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro1.Text) <= 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยประกันที่ต้องชำระ');", true);
                        return false;
                    }
                }
                else if ("pro2" == Type)
                {
                    if (lblCoverageType_pro2.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุประเภทประกัน');", true);
                        return false;
                    }
                    else if (lblInsNameTh_pro2.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุบริษัทประกัน');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Eff_Date_pro2.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันเริ่มคุ้มครอง');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Exp_Date_pro2.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันหมดอายุประกัน');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Eff_Date_pro2.DateValue >= lblVoluntary_Policy_Exp_Date_pro2.DateValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('วันเริ่มคุ้มครองต้องน้อยกว่าวันหมดอายุประกัน');", true);
                        return false;
                    }
                    else if (lblMaintanance_pro2.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุประเภทการซ่อม');", true);
                        return false;
                    }
                    else if (lblNetpremium_pro2.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยสุทธิ');", true);
                        return false;
                    }
                    else if (lblDuty_pro2.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุอากร');", true);
                        return false;
                    }
                    else if (lblVat_amount_pro2.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุภาษี');", true);
                        return false;
                    }
                    else if (lblVoluntary_Gross_Premium_pro2.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยประกันรวมภาษีอากร');", true);
                        return false;
                    }
                    else if (txtTotal_Voluntary_Gross_Premium_pro2.Text == "" || AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro2.Text) <= 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยประกันที่ต้องชำระ');", true);
                        return false;
                    }
                }
                else if ("pro3" == Type)
                {
                    if (lblCoverageType_pro3.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุประเภทประกัน');", true);
                        return false;
                    }
                    else if (lblInsNameTh_pro3.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุบริษัทประกัน');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Eff_Date_pro3.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันเริ่มคุ้มครอง');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Exp_Date_pro3.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันหมดอายุประกัน');", true);
                        return false;
                    }
                    else if (lblVoluntary_Policy_Eff_Date_pro3.DateValue >= lblVoluntary_Policy_Exp_Date_pro3.DateValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('วันเริ่มคุ้มครองต้องน้อยกว่าวันหมดอายุประกัน');", true);
                        return false;
                    }
                    else if (lblMaintanance_pro3.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุประเภทการซ่อม');", true);
                        return false;
                    }
                    else if (lblNetpremium_pro3.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยสุทธิ');", true);
                        return false;
                    }
                    else if (lblDuty_pro3.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุอากร');", true);
                        return false;
                    }
                    else if (lblVat_amount_pro3.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุภาษี');", true);
                        return false;
                    }
                    else if (lblVoluntary_Gross_Premium_pro3.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยประกันรวมภาษีอากร');", true);
                        return false;
                    }
                    else if (txtTotal_Voluntary_Gross_Premium_pro3.Text == "" || AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_pro3.Text) <= 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยประกันที่ต้องชำระ');", true);
                        return false;
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }

        private bool CheckAct(string Type)
        {

            try
            {
                if (Type == "")
                {
                    return true;
                }
                if ("act1" == Type)
                {
                    if (cmbActIssuePlace_pro1.SelectedValue == "" || cmbActIssuePlace_pro1.SelectedValue == "0")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุออกที่');", true);
                        return false;
                    }
                    else if (cmbActIssuePlace_pro1.SelectedValue == "1" && cmbActIssueBranch_pro1.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุออกที่สาขา');", true);
                        return false;
                    }

                    else if (lblCompanyInsuranceAct_pro1.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุบริษัท พรบ.');", true);
                        return false;
                    }
                    else if (txtActStartCoverDateAct_pro1.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันเริ่มต้น พรบ.');", true);
                        return false;
                    }

                    else if (txtActEndCoverDateAct_pro1.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันสิ้นสุด พรบ.');", true);
                        return false;
                    }
                    else if (txtActStartCoverDateAct_pro1.DateValue >= txtActEndCoverDateAct_pro1.DateValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('วันสิ้นสุด พรบ.ต้องมากกว่าวันเริ่มต้น พรบ.');", true);
                        return false;
                    }
                    else if (lblActNetGrossPremiumFullAct_pro1.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยสุทธิ (เต็มปี)');", true);
                        return false;
                    }

                    else if (txtDiscountBathAct_pro1.Text == "" && AppUtil.SafeDecimal(txtDiscountBathAct_pro1.Text) <= 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ย พรบ. ที่ต้องชำระ');", true);
                        return false;
                    }
                }
                else if ("act2" == Type)
                {
                    if (cmbActIssuePlace_pro2.SelectedValue == "" || cmbActIssuePlace_pro2.SelectedValue == "0")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุออกที่');", true);
                        return false;
                    }
                    else if (cmbActIssuePlace_pro2.SelectedValue == "1" && cmbActIssueBranch_pro2.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุออกที่สาขา');", true);
                        return false;
                    }

                    else if (lblCompanyInsuranceAct_pro2.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุบริษัท พรบ.');", true);
                        return false;
                    }
                    else if (txtActStartCoverDateAct_pro2.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันเริ่มต้น พรบ.');", true);
                        return false;
                    }

                    else if (txtActEndCoverDateAct_pro2.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันสิ้นสุด พรบ.');", true);
                        return false;
                    }
                    else if (txtActStartCoverDateAct_pro2.DateValue >= txtActEndCoverDateAct_pro2.DateValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('วันสิ้นสุด พรบ.ต้องมากกว่าวันเริ่มต้น พรบ.');", true);
                        return false;
                    }
                    else if (lblActNetGrossPremiumFullAct_pro2.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยสุทธิ (เต็มปี)');", true);
                        return false;
                    }

                    else if (txtDiscountBathAct_pro2.Text == "" && AppUtil.SafeDecimal(txtDiscountBathAct_pro2.Text) <= 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ย พรบ. ที่ต้องชำระ');", true);
                        return false;
                    }
                }
                else if ("act3" == Type)
                {
                    if (cmbActIssuePlace_pro3.SelectedValue == "" || cmbActIssuePlace_pro3.SelectedValue == "0")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุออกที่');", true);
                        return false;
                    }
                    //Nang Edit SelectedValue14/8/2016
                    else if (cmbActIssuePlace_pro3.SelectedValue == "1" && cmbActIssueBranch_pro3.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุออกที่สาขา');", true);
                        return false;
                    }

                    else if (lblCompanyInsuranceAct_pro3.SelectedValue == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุบริษัท พรบ.');", true);
                        return false;
                    }
                    else if (txtActStartCoverDateAct_pro3.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันเริ่มต้น พรบ.');", true);
                        return false;
                    }

                    else if (txtActEndCoverDateAct_pro3.DateValue == DateTime.MinValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุวันสิ้นสุด พรบ.');", true);
                        return false;
                    }
                    else if (txtActStartCoverDateAct_pro3.DateValue >= txtActEndCoverDateAct_pro3.DateValue)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('วันสิ้นสุด พรบ.ต้องมากกว่าวันเริ่มต้น พรบ.');", true);
                        return false;
                    }
                    else if (lblActNetGrossPremiumFullAct_pro3.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ยสุทธิ (เต็มปี)');", true);
                        return false;
                    }

                    else if (txtDiscountBathAct_pro3.Text == "" && AppUtil.SafeDecimal(txtDiscountBathAct_pro3.Text) <= 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('กรุณาระบุเบี้ย พรบ. ที่ต้องชำระ');", true);
                        return false;
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }

        protected bool CheckDiscount(string policyType, string actType , string ticketIdOrPreleadId)
        {
            try
            {
                decimal disPerPolicy = 0;
                decimal disBathPolicy = 0;
                decimal PolicyGrossPremiumTotalPolicy = 0;
                decimal PolicyNetPremium = 0; // เบี้ยสุทธิ

                decimal disPerAct = 0;
                decimal disBathAct = 0;
                decimal PolicyGrossPremiumTotalAct = 0;
                decimal ActNetPremium = 0; // เบี้ยสุทธิ

                if ("cur" == policyType)
                {
                    disPerPolicy = AppUtil.SafeDecimal(txtDiscountPercent_cur.Text);
                    disBathPolicy = AppUtil.SafeDecimal(txtDiscountBath_cur.Text);
                    PolicyGrossPremiumTotalPolicy = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_cur.Text) - AppUtil.SafeDecimal(lblPersonType_cur.Text);
                    PolicyNetPremium = AppUtil.SafeDecimal(lblNetpremium_cur.Text) - AppUtil.SafeDecimal(lblPersonType_cur.Text);
                }
                else if ("pro1" == policyType)
                {
                    disPerPolicy = AppUtil.SafeDecimal(txtDiscountPercent_pro1.Text);
                    disBathPolicy = AppUtil.SafeDecimal(txtDiscountBath_pro1.Text);
                    PolicyGrossPremiumTotalPolicy = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro1.Text) - AppUtil.SafeDecimal(lblPersonType_pro1.Text);
                    PolicyNetPremium = AppUtil.SafeDecimal(lblNetpremium_pro1.Text) - AppUtil.SafeDecimal(lblPersonType_pro1.Text);

                }
                else if ("pro2" == policyType)
                {
                    disPerPolicy = AppUtil.SafeDecimal(txtDiscountPercent_pro2.Text);
                    disBathPolicy = AppUtil.SafeDecimal(txtDiscountBath_pro2.Text);
                    PolicyGrossPremiumTotalPolicy = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro2.Text) - AppUtil.SafeDecimal(lblPersonType_pro2.Text);
                    PolicyNetPremium = AppUtil.SafeDecimal(lblNetpremium_pro2.Text) - AppUtil.SafeDecimal(lblPersonType_pro2.Text);
                }
                else if ("pro3" == policyType)
                {
                    disPerPolicy = AppUtil.SafeDecimal(txtDiscountPercent_pro3.Text);
                    disBathPolicy = AppUtil.SafeDecimal(txtDiscountBath_pro3.Text);
                    PolicyGrossPremiumTotalPolicy = AppUtil.SafeDecimal(lblVoluntary_Gross_Premium_pro3.Text) - AppUtil.SafeDecimal(lblPersonType_pro3.Text);
                    PolicyNetPremium = AppUtil.SafeDecimal(lblNetpremium_pro3.Text) - AppUtil.SafeDecimal(lblPersonType_pro3.Text);
                }

                if ("act1" == actType)
                {
                    disPerAct = AppUtil.SafeDecimal(lblVat1PercentBathAct_pro1.Text);
                    disBathAct = AppUtil.SafeDecimal(lblDiscountPercentAct_pro1.Text);
                    PolicyGrossPremiumTotalAct = AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro1.Text) - AppUtil.SafeDecimal(lblActPersonType_pro1.Text);
                    ActNetPremium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro1.Text) - AppUtil.SafeDecimal(lblActPersonType_pro1.Text);
                }
                else if ("act2" == actType)
                {
                    disPerAct = AppUtil.SafeDecimal(lblVat1PercentBathAct_pro2.Text);
                    disBathAct = AppUtil.SafeDecimal(lblDiscountPercentAct_pro2.Text);
                    PolicyGrossPremiumTotalAct = AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro2.Text) - AppUtil.SafeDecimal(lblActPersonType_pro2.Text);
                    ActNetPremium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro2.Text) - AppUtil.SafeDecimal(lblActPersonType_pro2.Text);
                }
                else if ("act3" == actType)
                {
                    disPerAct = AppUtil.SafeDecimal(lblVat1PercentBathAct_pro3.Text);
                    disBathAct = AppUtil.SafeDecimal(lblDiscountPercentAct_pro3.Text);
                    PolicyGrossPremiumTotalAct = AppUtil.SafeDecimal(lblActNetGrossPremiumAct_pro3.Text) - AppUtil.SafeDecimal(lblActPersonType_pro3.Text);
                    ActNetPremium = AppUtil.SafeDecimal(lblActGrossPremiumAct_pro3.Text) - AppUtil.SafeDecimal(lblActPersonType_pro3.Text);
                }

                decimal totalToPay = PolicyNetPremium + ActNetPremium; // เบี้ยสุทธิ

                if (totalToPay != 0)
                {

                    // หา % ของส่วนลด ปัจจุบัน
                    decimal curDisPercent = Math.Round(Math.Round(((disBathPolicy + disBathAct) * 100m) / (totalToPay), 2, MidpointRounding.AwayFromZero), 0, MidpointRounding.AwayFromZero);
                    var username = HttpContext.Current.User.Identity.Name;
                    string err;
                    string errTxt = "";

                    if (!ActivityLeadBiz.CheckDiscount(username, PolicyNetPremium, disBathPolicy, 0, "204", out err, ticketIdOrPreleadId))
                    {
                        errTxt = err;
                    }
                    if (!ActivityLeadBiz.CheckDiscount(username, ActNetPremium, disBathAct, 0, "205", out err, ticketIdOrPreleadId))
                    {
                        errTxt += (errTxt == "" ? "" : "\\n") + err;
                    }

                    if (errTxt != "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "error", "pageLoad();alert('" + errTxt + "');", true);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        protected void btnAddBlankPolicy_OnClick(object sender, EventArgs e)
        {
            try
            {
             if (Session[SessionPrefix+"allData"] == null)
            {
                SessionExpired();
                return;
            }
            tempAllData();
            PreleadCompareDataCollection pc = getSelectTabData();
            int i = 0;
            int j = 0;

                if (pc.ComparePromoList != null && pc.ComparePromoList.Count > 0)
                {
                    i = i + pc.ComparePromoList.Count;
                }

            bool state = true;

                if (state)
                {
                    if (i < 3 && j < 3)
                    {
                        setPromotion(0, state, "P");
                    }
                    else
                    {
                        AppUtil.ClientAlertTabRenew(Page, "เลือกโปรโมชั่นได้ไม่เกิน 3 โปรโมชั่น");
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

        protected void lblVoluntary_Policy_Eff_Date_cur_OnTextChanged(object sender, EventArgs e)
        {
            if (lblVoluntary_Policy_Eff_Date_cur.DateValue.Year == 1)
            {
                lblVoluntary_Policy_Exp_Date_cur.DateValue = new DateTime();
            }
            else
            {
                lblVoluntary_Policy_Exp_Date_cur.DateValue = lblVoluntary_Policy_Eff_Date_cur.DateValue.AddYears(1);
            }
        }

        protected void lblVoluntary_Policy_Eff_Date_pro1_OnTextChanged(object sender, EventArgs e)
        {
            if (lblVoluntary_Policy_Eff_Date_pro1.DateValue.Year == 1)
            {
                lblVoluntary_Policy_Exp_Date_pro1.DateValue = new DateTime();
            }
            else
            {
                lblVoluntary_Policy_Exp_Date_pro1.DateValue = lblVoluntary_Policy_Eff_Date_pro1.DateValue.AddYears(1);
            }
        }

        protected void lblVoluntary_Policy_Eff_Date_pro2_OnTextChanged(object sender, EventArgs e)
        {
            if (lblVoluntary_Policy_Eff_Date_pro2.DateValue.Year == 1)
            {
                lblVoluntary_Policy_Exp_Date_pro2.DateValue = new DateTime();
            }
            else
            {
                lblVoluntary_Policy_Exp_Date_pro2.DateValue = lblVoluntary_Policy_Eff_Date_pro2.DateValue.AddYears(1);
            }
        }

        protected void lblVoluntary_Policy_Eff_Date_pro3_OnTextChanged(object sender, EventArgs e)
        {
            if (lblVoluntary_Policy_Eff_Date_pro3.DateValue.Year == 1)
            {
                lblVoluntary_Policy_Exp_Date_pro3.DateValue = new DateTime();
            }
            else
            {
                lblVoluntary_Policy_Exp_Date_pro3.DateValue = lblVoluntary_Policy_Eff_Date_pro3.DateValue.AddYears(1);
            }
        }

        protected void btnAddBlankAct_OnClick(object sender, EventArgs e)
        {
            try
            {
                tempAllData();
                PreleadCompareDataCollection pc = getSelectTabData();
                int i = 0;
                int j = 0;

                if (pc.ActPromoList != null && pc.ActPromoList.Count > 0)
                {
                    j = j + pc.ActPromoList.Count;
                }

                bool state = true;

                if (state)
                {
                    if (i < 3 && j < 3)
                    {
                        setPromotion(0, state, "A");
                    }
                    else
                    {
                        AppUtil.ClientAlertTabRenew(Page, "เลือกโปรโมชั่นได้ไม่เกิน 3 โปรโมชั่น");
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

        private void CreateCASActivityLog(PreleadCompareDataCollection data)
        {
            try
            {
                string preleadId = "";
                string actSendDate = "";

                if (data != null)
                {
                    preleadId = data.Prelead.slm_Prelead_Id == null ? "" : data.Prelead.slm_Prelead_Id.ToString();
                    actSendDate = data.RenewIns.slm_ActSendDate != null ? (data.RenewIns.slm_ActSendDate.Value.ToString("dd-MM-") + data.RenewIns.slm_ActSendDate.Value.Year.ToString()) : "";
                }

                string incentiveValue = "";
                string incentiveActValue = "";

                decimal? currentPaidPolicy = null;
                decimal? currentPaidAct = null;

                incentiveValue = data.ComparePromoList.FirstOrDefault(i => i.slm_Selected == true) != null
                    ? (data.RenewIns.slm_IncentiveFlag == true ? "Y" : "N")
                    : "";

                if (data.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204").Count() > 0)
                {
                    var paidPolicy = data.ReceiptDetailList.Where(a => a.slm_PaymentCode == "204")
                                                    .GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate })
                                                    .Select(g => new
                                                    {
                                                        ReceiptId = g.Key.slm_RenewInsuranceReceiptId,
                                                        TransDate = g.Key.slm_TransDate,
                                                        Amount = g.Sum(a => a.slm_RecAmount ?? 0)
                                                    })
                                                    .OrderByDescending(g => g.TransDate)
                                                    .FirstOrDefault();
                    currentPaidPolicy = paidPolicy != null ? (decimal?)null : paidPolicy.Amount;
                }

                incentiveActValue = data.RenewIns.slm_ActPurchaseFlag != null && data.RenewIns.slm_ActPurchaseFlag == true
                    ? (data.RenewIns.slm_ActIncentiveFlag == true ? "Y" : "N")
                    : "";
                if (data.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205").Count() > 0)
                {
                    var paidAct = data.ReceiptDetailList.Where(a => a.slm_PaymentCode == "205")
                                                    .GroupBy(a => new { a.slm_RenewInsuranceReceiptId, a.slm_TransDate })
                                                    .Select(g => new
                                                    {
                                                        ReceiptId = g.Key.slm_RenewInsuranceReceiptId,
                                                        TransDate = g.Key.slm_TransDate,
                                                        Amount = g.Sum(a => a.slm_RecAmount ?? 0)
                                                    })
                                                    .OrderByDescending(g => g.TransDate)
                                                    .FirstOrDefault();
                    currentPaidAct = paidAct != null ? (decimal?)null : paidAct.Amount;
                }

                string contractNo = "", ticketId = "", firstName = "", lastName = "", cardTypeId = "", citizenId = "", campaignId = "", campaignName = "", productId = "", productName = "", productGroupId = "", productGroupName = "", ownerBranchCode = "", ownerBranchName = "", ownerName = "", ownerUsername = "", ownerEmpCode = ""
                    , telNo1 = "", statusCode = "", statusDesc = "", subStatusCode = "", subStatusDesc = "", delegateBranchCode = "", delegateBranchName = "", delegateUsername = "", delegateName = "", channelId = "", carLicenseNo = "";
                DateTime? nextContactDate = null;

                if (GetOBTMainData != null)
                {
                    GetOBTMainData(out contractNo, out ticketId, out firstName, out lastName, out cardTypeId, out citizenId, out campaignId, out campaignName, out productId, out productName, out productGroupId, out productGroupName, out ownerBranchCode, out ownerBranchName, out ownerUsername, out ownerName, out ownerEmpCode
                        , out delegateBranchCode, out delegateBranchName, out delegateUsername, out delegateName, out telNo1, out statusCode, out statusDesc, out subStatusCode, out subStatusDesc, out channelId, out nextContactDate, out carLicenseNo);
                }

                //Activity Info
                List<CARService.DataItem> actInfoList = new List<CARService.DataItem>();
                actInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่รับแจ้ง", DataValue = data.RenewIns.slm_ReceiveNo ?? "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "วันที่ส่งแจ้ง พรบ.", DataValue = actSendDate });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "Incentive ประกัน", DataValue = incentiveValue });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 4, DataLabel = "Incentive พรบ.", DataValue = incentiveActValue });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 5, DataLabel = "ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้", DataValue = currentPaidPolicy != null ? currentPaidPolicy.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 6, DataLabel = "ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้", DataValue = currentPaidAct != null ? currentPaidAct.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 7, DataLabel = "ค่าเบี้ยประกันที่ลูกค้าชำระรวม", DataValue = data.PolicyRecAmt != null ? data.PolicyRecAmt.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 8, DataLabel = "ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม", DataValue = data.ActRecAmt != null ? data.ActRecAmt.Value.ToString("#,##0.00") : "" });

                //Customer Info
                List<CARService.DataItem> cusInfoList = new List<CARService.DataItem>();
                cusInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = data != null ? data.lead.slm_CitizenId : "" });
                cusInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = data != null ? data.lead.slm_CardTypeName : "" });
                cusInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = firstName + " " + lastName });

                //Product Info
                List<CARService.DataItem> prodInfoList = new List<CARService.DataItem>();
                prodInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = productGroupName });
                prodInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = productName });
                prodInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = campaignName });

                //Contract Info
                List<CARService.DataItem> contInfoList = new List<CARService.DataItem>();
                contInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.RenewIns.slm_ContractNo : "" });
                contInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = preleadId != "" ? "HP" : "" });
                contInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.RenewIns.slm_LicenseNo : "" });

                //Officer Info
                List<CARService.DataItem> offInfoList = new List<CARService.DataItem>();
                offInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = HttpContext.Current.User.Identity.Name });

                CARService.CARServiceData logdata = new CARService.CARServiceData()
                {
                    ReferenceNo = data.RenewIns.slm_RenewInsureId.ToString(),
                    SecurityKey = preleadId != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey,
                    ServiceName = "CreateActivityLog",
                    SystemCode = preleadId != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM,        //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
                    TransactionDateTime = DateTime.Now,
                    ActivityInfoList = actInfoList,
                    CustomerInfoList = cusInfoList,
                    ProductInfoList = prodInfoList,
                    ContractInfoList = contInfoList,
                    OfficerInfoList = offInfoList,
                    ActivityDateTime = DateTime.Now,
                    CampaignId = campaignId,
                    ChannelId = SLMConstant.CARLogService.CARPreleadChannelId, // copy from Oum's change
                    PreleadId = preleadId,
                    ProductGroupId = productGroupId,
                    ProductId = productId,
                    Status = statusDesc,
                    SubStatus = subStatusDesc,
                    TicketId = data.lead.TicketId,
                    SubscriptionId = data.lead.slm_CitizenId,
                    TypeId = SLMConstant.CARLogService.Data.TypeId,
                    AreaId = SLMConstant.CARLogService.Data.AreaId,
                    SubAreaId = SLMConstant.CARLogService.Data.SubAreaId,
                    ActivityTypeId = SLMConstant.CARLogService.Data.ActivityType.TodoId,
                    ContractNo = data != null ? data.Prelead.slm_Contract_Number : ""
                };

                // Nui - Cannot find subscription
                if (data.lead.slm_CardTypeId != null)
                {
                    logdata.CIFSubscriptionTypeId = ActivityLeadBiz.GetSubScriptionTypeId(data.lead.slm_CardTypeId.Value);
                }

                bool ret = CARService.CreateActivityLog(logdata);
                // AppUtil.UpdatePhonecallCASFlag(slmdb, phonecall_id, ret ? "1" : "2");
            }
            catch (Exception ex)
            {
                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
            }
        }

        private bool IsOverideUser(StaffData staff)
        {
            // role ของ user ที่มีสิทธิ์ให้ความคุ้มครองก่อนจ่ายเงินกรณีชำระเต็ม
            bool supervisor = ActivityLeadBiz.CheckSupervisor(staff.UserName);
            if (staff == null)
            {
                return false;
            }
            else
            {
                return (supervisor || staff.StaffTypeId == SLMConstant.StaffType.OperOutbound
                                              || staff.StaffTypeId == SLMConstant.StaffType.ProductOutbound
                                              || staff.StaffTypeId == SLMConstant.StaffType.ManagerOutbound);
            }
        }

        protected void rbInsNameTh_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked == true && cmbPaymentmethod.SelectedIndex == 0)
            {
                AppUtil.SetComboValue(cmbPaymentmethod, "1");
                pnPolicyPayment.Update();
            }
        }

        protected void rbAct_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked == true && cmbPaymentmethodAct.SelectedIndex == 0)
            {
                AppUtil.SetComboValue(cmbPaymentmethodAct, "1");
                pnActPayment.Update();
            }
        }
    }
}