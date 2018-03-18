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
using ToEcm = Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint;
using System.IO;
using System.Net;

namespace SLM.Application.Shared.Tabs
{
    public partial class Tab008 : System.Web.UI.UserControl
    {
        //public delegate void UpdatedDataEvent(string statusDesc, DateTime contactLatestDate);
        //public event UpdatedDataEvent UpdatedDataChanged;
        public delegate void UpdatedMainDataEvent(string ticketId, string ownerBranchName, string ownerLeadName, string delegateBranchName, string delegateLeadName, string telNo1, string statusDesc, DateTime contactLatestDate, bool doUpdateOwnerLogging, string externalSubStatusDesc, string cardTypeId, string citizenId, string subStatusCode, DateTime? nextContactDate);
        public event UpdatedMainDataEvent UpdatedMainDataChanged;
        public delegate void GetMainDataEvent(out string ticketId, out string firstname, out string lastname, out string cardTypeId, out string citizenId, out string campaignId, out string campaignName, out string ownerBranchCode, out string ownerLead, out string delegateBranchCode, out string delegateLead
            , out string telNo1, out string status, out string channelId, out string productGroupId, out string productGroupName, out string productId, out string productName);
        public event GetMainDataEvent GetMainData;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Tab008));
        private string _currentAssignedFlag = "";
        private string _currentDelegateFlag = "";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.SetIntTextBox(txtTicketID);
            AppUtil.SetIntTextBox(txtContactPhone);
            txtContactPhone.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
            AppUtil.SetMultilineMaxLength(txtContactDetail, vtxtContactDetail.ClientID, "500");
            AppUtil.SetIntTextBox(txtCitizenId);
            AppUtil.SetIntTextBox(txtTelNoSms);

            //AppUtil.SetIntTextBox(txtTelNo1_PreleadPopup);      //move to TabActRenewInsure.ascx
            AppUtil.SetAutoCompleteDropdown(new DropDownList[] { cmbOwnerBranch, cmbOwner, cmbDelegateBranch, cmbDelegate,cmbCountry }, Page, "AUTOCOMPLETESCRIPT");
        }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        public void InitialControl(LeadDefaultData data, string loginName)
        {
            SlmScr008Biz biz = null;
            try
            {
                Session.Remove(SessionPrefix+SLMConstant.SessionName.default_phonecall_list);
                biz = new SlmScr008Biz();
                txtTicketIdSearch.Text = data.TicketId;
                txtCitizenIdSearch.Text = data.CitizenId;
                txtTelNo1Search.Text = data.TelNo1;
                txtPreleadIdSearch.Text = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
                txtLoginName.Text = loginName;

                string[] cocTeam = new string[] { SLMConstant.COCTeam.Marketing, SLMConstant.COCTeam.Bpel };
                if (data.ISCOC == "1" && !cocTeam.Contains(data.COCCurrentTeam))
                {
                    btnAddResultContact.Visible = false;
                }
                else
                {
                    CheckActivityConfig(biz, data.ProductId, data.StatusCode);
                }

                //Comment on 2017-08-28
                //if (data.ISCOC == "1" && data.COCCurrentTeam != SLMConstant.COCTeam.Marketing)
                //    btnAddResultContact.Visible = false;
                //else
                //    CheckActivityConfig(biz, data.ProductId, data.StatusCode);

                pcTop.SetVisible = false;
                cbThisLead.Checked = true;
                DoBindGridview(biz, 0);

                HideDebugControl();     //ซ่อน Control สำหรับ Debug

                string cbsSubscriptionTypeId = "0";
                if (data.CardType != null)
                {
                    cbsSubscriptionTypeId = SlmScr008Biz.GetSubScriptionTypeId(data.CardType.Value);
                }

                lbCasAllActivity.OnClientClick = new Services.CARService().GetCallCASScript(data.TicketId, data.PreleadId, cbsSubscriptionTypeId, data.CitizenId, HttpContext.Current.User.Identity.Name);

            }
            catch (Exception ex)
            {
                _log.Debug(ex);
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

        private void HideDebugControl()
        {
            txtLoginName.Visible = false;
            txtPreleadIdSearch.Visible = false;
            txtTicketIdSearch.Visible = false;
            txtCitizenIdSearch.Visible = false;
            txtTelNo1Search.Visible = false;
            txtAssignedFlag.Visible = false;
            txtDelegateFlag.Visible = false;
            txtCampaignId.Visible = false;
            txtChannelId.Visible = false;
            txtProductGroupId.Visible = false;
            txtProductGroupName.Visible = false;
            txtProductId.Visible = false;
            txtProductName.Visible = false;
            txtOldOwnerBranch.Visible = false;
            txtOldOwner.Visible = false;
            txtOldDelegateBranch.Visible = false;
            txtOldDelegate.Visible = false;
            txtOldStatus.Visible = false;
        }

        private void CheckActivityConfig(SlmScr008Biz biz, string productId, string leadStatus)
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
                    btnAddResultContact.Visible = false;
            }
            catch
            {
                throw;
            }
        }

        protected void chkthis_CheckedChanged(object sender, EventArgs e)
        {
            SlmScr008Biz biz = null;
            try
            {
                biz = new SlmScr008Biz();
                DoBindGridview(biz, 0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        private void DoBindGridview(SlmScr008Biz biz, int pageIndex)
        {
            try
            {
                _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + txtTicketIdSearch.Text.Trim() + " Begin DoBindGridview (Get Data from DB) and cbThisLead=" + (cbThisLead.Checked ? "TRUE" : "FALSE"));
                var result = biz.GetDefaultPhoneCallHistory(txtCitizenIdSearch.Text.Trim(), txtTicketIdSearch.Text.Trim(), cbThisLead.Checked);

                _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + txtTicketIdSearch.Text.Trim() + " Begin DoBindGridview (BindGridview)");
                BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), pageIndex);

                //Avoid session conflict
                //Session[SessionPrefix+SLMConstant.SessionName.default_phonecall_list] = result;

                _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + txtTicketIdSearch.Text.Trim() + " End DoBindGridview");
            }
            catch
            {
                throw;
            }
        }

        protected void imbDownload50Tawi_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Session[SLMConstant.SessionName.Tawi50FormFilePath] = ((ImageButton)sender).CommandArgument;
                string script = "window.open('SLM_SCR_054.aspx?type=2', 'tawi50form', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=no');";
                ScriptManager.RegisterStartupScript(Page, GetType(), "tawi50form", script, true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbDownloadCredit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Session[SLMConstant.SessionName.CreditFormFilePath] = ((ImageButton)sender).CommandArgument;
                string script = "window.open('SLM_SCR_054.aspx?type=1', 'creditform', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=no');";
                ScriptManager.RegisterStartupScript(Page, GetType(), "creditform", script, true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbDownloadDriverLicense_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Session[SLMConstant.SessionName.DriverLicenseFormFilePath] = ((ImageButton)sender).CommandArgument;
                string script = "window.open('SLM_SCR_054.aspx?type=3', 'driverform', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=no');";
                ScriptManager.RegisterStartupScript(Page, GetType(), "driverform", script, true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvPhoneCallHistoty);
            pageControl.Update(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            SlmScr008Biz biz = null;
            try
            {
                biz = new SlmScr008Biz();
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoBindGridview(biz, pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                if (biz != null)
                    biz.Dispose();
            }
        }

        #endregion

        private void InitialDropdownlist(SlmScr008Biz biz, string camapaignId, string productId, string statusCode)
        {
            try
            {
                //ประเภทบุคคล
                cmbCardType.DataSource = biz.GetCardTypeList();
                cmbCardType.DataTextField = "TextField";
                cmbCardType.DataValueField = "ValueField";
                cmbCardType.DataBind();
                cmbCardType.Items.Insert(0, new ListItem("", ""));

                var branchList = biz.GetBranchAccessRightList(SLMConstant.Branch.Active, camapaignId);
                cmbOwnerBranch.DataSource = branchList;
                cmbOwnerBranch.DataTextField = "TextField";
                cmbOwnerBranch.DataValueField = "ValueField";
                cmbOwnerBranch.DataBind();
                cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));
                cmbOwner.Items.Insert(0, new ListItem("", ""));
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty)
                    cmbOwner.Enabled = true;
                else
                    cmbOwner.Enabled = false;

                //cmbOwnerBranchSelectedIndexChanged(biz);       

                cmbDelegateBranch.DataSource = branchList;
                cmbDelegateBranch.DataTextField = "TextField";
                cmbDelegateBranch.DataValueField = "ValueField";
                cmbDelegateBranch.DataBind();
                cmbDelegateBranch.Items.Insert(0, new ListItem("", ""));
                cmbDelegate.Items.Insert(0, new ListItem("", ""));

                if (cmbDelegateBranch.SelectedItem.Value != string.Empty)
                    cmbDelegate.Enabled = true;
                else
                    cmbDelegate.Enabled = false;

                //cmbDelegateBranchSelectedIndexChanged(biz);    

                //Status
                cmbLeadStatus.DataSource = biz.GetStatusListByActivityConfig(productId, statusCode);
                cmbLeadStatus.DataTextField = "TextField";
                cmbLeadStatus.DataValueField = "ValueField";
                cmbLeadStatus.DataBind();
                cmbLeadStatus.Items.Insert(0, new ListItem("", ""));    //Do not remove

                //ประเทศ
                cmbCountry.DataSource = CountryBiz.GetCountryList();
                cmbCountry.DataTextField = "TextField";
                cmbCountry.DataValueField = "ValueField";
                cmbCountry.DataBind();
                cmbCountry.Items.Insert(0, new ListItem("", ""));
            }
            catch
            {
                throw;
            }
        }

        #region Backup btnAddResultContact_Click 07/09/2016
        //protected void btnAddResultContact_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var data = LeadInfoBiz.GetLeadDataPhoneCallHistoryDefault(txtTicketIdSearch.Text.Trim());
        //        if (data != null)
        //        {
        //            txtTicketID.Text = data.TicketId;
        //            txtFirstname.Text = data.Name;
        //            txtLastname.Text = data.LastName;
        //            txtChannelId.Text = data.ChannelId;
        //            txtProductGroupId.Text = data.ProductGroupId;
        //            txtProductGroupName.Text = data.ProductGroupName;
        //            txtProductId.Text = data.ProductId;
        //            txtProductName.Text = data.ProductName;
        //            txtCampaignId.Text = data.CampaignId;
        //            txtCampaign.Text = data.CampaignName;
        //            txtTelNo1.Text = data.TelNo1;                  
        //            txtCitizenId.Text = data.CitizenId;
        //            txtAssignedFlag.Text = data.AssignedFlag;
        //            txtDelegateFlag.Text = data.DelegateFlag != null ? data.DelegateFlag.ToString() : "";

        //            InitialDropdownlist(data);

        //            if (data.CardType != null)
        //            {
        //                cmbCardType.SelectedIndex = cmbCardType.Items.IndexOf(cmbCardType.Items.FindByValue(data.CardType.Value.ToString()));
        //                lblCitizenId.Text = "*";
        //                txtCitizenId.Enabled = true;
        //                txtCitizenId.Text = data.CitizenId;
        //                AppUtil.SetCardTypeValidation(cmbCardType.SelectedItem.Value, txtCitizenId);
        //            }
        //            else
        //                txtCitizenId.Text = data.CitizenId;

        //            //Owner Branch
        //            if (!string.IsNullOrEmpty(data.OwnerBranch))
        //            {
        //                txtOldOwnerBranch.Text = data.OwnerBranch;
        //                ListItem item = cmbOwnerBranch.Items.FindByValue(data.OwnerBranch);
        //                if (item != null)
        //                    cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(item);
        //                else
        //                {
        //                    //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
        //                    if (SlmScr011Biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, txtCampaignId.Text.Trim(), data.OwnerBranch))
        //                    {
        //                        //Branch ที่ถูกปิด
        //                        string branchName = BranchBiz.GetBranchName(data.OwnerBranch);
        //                        if (!string.IsNullOrEmpty(branchName))
        //                        {
        //                            cmbOwnerBranch.Items.Insert(1, new ListItem(branchName, data.OwnerBranch));
        //                            cmbOwnerBranch.SelectedIndex = 1;
        //                        }
        //                    }
        //                }

        //                cmbOwnerBranchSelectedIndexChanged();   //Bind Combo Owner
        //            }

        //            //Owner
        //            if (!string.IsNullOrEmpty(data.Owner))
        //            {
        //                txtOldOwner.Text = data.Owner;
        //                cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(data.Owner));
        //            }

        //            //Delegate Branch
        //            if (!string.IsNullOrEmpty(data.DelegateBranch))
        //            {
        //                txtOldDelegateBranch.Text = data.DelegateBranch;
        //                ListItem item = cmbDelegateBranch.Items.FindByValue(data.DelegateBranch);
        //                if (item != null)
        //                    cmbDelegateBranch.SelectedIndex = cmbDelegateBranch.Items.IndexOf(item);
        //                else
        //                {
        //                    //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
        //                    if (SlmScr011Biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, txtCampaignId.Text.Trim(), data.DelegateBranch))
        //                    {
        //                        //Branch ที่ถูกปิด
        //                        string branchName = BranchBiz.GetBranchName(data.DelegateBranch);
        //                        if (!string.IsNullOrEmpty(branchName))
        //                        {
        //                            cmbDelegateBranch.Items.Insert(1, new ListItem(branchName, data.DelegateBranch));
        //                            cmbDelegateBranch.SelectedIndex = 1;
        //                        }
        //                    }
        //                }

        //                cmbDelegateBranchSelectedIndexChanged();    //Bind Combo Delegate
        //            }

        //            if (!string.IsNullOrEmpty(data.Delegate))
        //            {
        //                txtOldDelegate.Text = data.Delegate;
        //                cmbDelegate.SelectedIndex = cmbDelegate.Items.IndexOf(cmbDelegate.Items.FindByValue(data.Delegate));
        //            }

        //            //Lead Status
        //            if (cmbLeadStatus.Items.Count > 0)
        //            {
        //                cmbLeadStatus.SelectedIndex = cmbLeadStatus.Items.IndexOf(cmbLeadStatus.Items.FindByValue(data.LeadStatus));
        //                txtOldStatus.Text = data.LeadStatus;
        //            }

        //            cmbLeadStatus.Enabled = data.AssignedFlag == "1" ? true : false;
        //            if (data.AssignedFlag == "1" && data.LeadStatus != "00")
        //            {
        //                cmbLeadStatus.Items.Remove(cmbLeadStatus.Items.FindByValue("00"));  //ถ้าจ่ายงานแล้ว และสถานะปัจจุบันไม่ใช่สนใจ ให้เอาสถานะ สนใจ ออก
        //            }

        //            //เช็กสิทธิการแก้ไขข้อมูล
        //            if (txtAssignedFlag.Text.Trim() == "0" || txtDelegateFlag.Text.Trim() == "1")   //ยังไม่จ่ายงาน assignedFlag = 0, delegateFlag = 1
        //            {
        //                cmbDelegateBranch.Enabled = false;
        //                cmbDelegate.Enabled = false;
        //                cmbOwnerBranch.Enabled = false;
        //                cmbOwner.Enabled = false;
        //                lblTab008Info.Text = "ไม่สามารถแก้ไข Owner และ Delegate ได้ เนื่องจากอยู่ระหว่างรอระบบจ่ายงาน กรุณารอ 1 นาที";
        //            }
        //            else
        //                AppUtil.CheckOwnerPrivilege(data.Owner, data.Delegate, cmbOwnerBranch, cmbOwner, cmbDelegateBranch, cmbDelegate);

        //            upPopup.Update();
        //            mpePopup.Show();
        //        }
        //        else
        //            AppUtil.ClientAlert(Page, "ไม่พบ Ticket Id " + txtTicketID.Text.Trim() + " ในระบบ");
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Error(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}
        #endregion

        protected void btnAddResultContact_Click(object sender, EventArgs e)
        {
            SlmScr008Biz biz = null;
            try
            {
                biz = new SlmScr008Biz();

                string ticketId = "", firstName = "", lastName = "", citizenId = "", campaignId = "", campaignName = "", ownerBranchCode = "", ownerLeadUsername = "", delegateBranchCode = "", delegateLeadUsername = "", telNo1 = ""
                    , statusCode = "", channelId = "", productGroupId = "", productGroupName = "", productId = "", productName = "", cardTypeId = "", assignedFlag = "", delegateFlag = "", telNoSMS = ""
                    , countryId = "";

                if (GetMainData != null)
                {
                    GetMainData(out ticketId, out firstName, out lastName, out cardTypeId, out citizenId, out campaignId, out campaignName, out ownerBranchCode, out ownerLeadUsername, out delegateBranchCode, out delegateLeadUsername
                        , out telNo1, out statusCode, out channelId, out productGroupId, out productGroupName, out productId, out productName);
                }                  

                SlmScr004Biz.GetUpdatedData(ticketId, out assignedFlag, out delegateFlag, out ownerBranchCode, out ownerLeadUsername, out delegateBranchCode, out delegateLeadUsername, out statusCode, out telNoSMS, out countryId);

                txtTicketID.Text = ticketId;
                txtFirstname.Text = firstName;
                txtLastname.Text = lastName;
                txtChannelId.Text = channelId;
                txtProductGroupId.Text = productGroupId;
                txtProductGroupName.Text = productGroupName;
                txtProductId.Text = productId;
                txtProductName.Text = productName;
                txtCampaignId.Text = campaignId;
                txtCampaign.Text = campaignName;
                txtTelNo1.Text = telNo1;
                //var telNoSms = biz.GetTelNoSms(ticketId);
                txtTelNoSms.Text = string.IsNullOrWhiteSpace(telNoSMS) ? telNo1 : telNoSMS;
                txtCitizenId.Text = citizenId;
                txtAssignedFlag.Text = assignedFlag;
                txtDelegateFlag.Text = delegateFlag;

                InitialDropdownlist(biz, campaignId, productId, statusCode);

                cmbCountry.SelectedValue = "";
                lblCountryId.Text = "";
                //cmbCountry.Enabled = false;

                if (cardTypeId != "")
                {
                    cmbCardType.SelectedIndex = cmbCardType.Items.IndexOf(cmbCardType.Items.FindByValue(cardTypeId));
                    lblCitizenId.Text = "*";
                    txtCitizenId.Enabled = true;
                    txtCitizenId.Text = citizenId;
                    lblCountryId.Text = "*";
                    cmbCountry.SelectedValue = countryId;
                    AppUtil.SetCardTypeValidation(cmbCardType.SelectedItem.Value, txtCitizenId);

                    //if (cardTypeId == AppConstant.CardType.Foreigner)
                    //{
                    //    lblCountryId.Text = "*";
                    //    cmbCountry.Enabled = true;
                    //}
                }
                else
                {
                    txtCitizenId.Text = citizenId;
                }

                //Owner Branch
                if (!string.IsNullOrEmpty(ownerBranchCode))
                {
                    txtOldOwnerBranch.Text = ownerBranchCode;
                    ListItem item = cmbOwnerBranch.Items.FindByValue(ownerBranchCode);
                    if (item != null)
                        cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(item);
                    else
                    {
                        //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
                        if (biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, txtCampaignId.Text.Trim(), ownerBranchCode))
                        {
                            //Branch ที่ถูกปิด
                            string branchName = biz.GetBranchName(ownerBranchCode);
                            if (!string.IsNullOrEmpty(branchName))
                            {
                                cmbOwnerBranch.Items.Insert(1, new ListItem(branchName, ownerBranchCode));
                                cmbOwnerBranch.SelectedIndex = 1;
                            }
                        }
                    }

                    cmbOwnerBranchSelectedIndexChanged(biz, ownerLeadUsername);   //Bind Combo Owner
                }

                //Owner
                if (!string.IsNullOrEmpty(ownerLeadUsername))
                {
                    //List<ControlListData> source = biz.GetStaffNotDummyAllDataByAccessRight(txtCampaignId.Text.Trim(), cmbOwnerBranch.SelectedItem.Value, ownerLeadUsername);
                    //คำนวณงานในมือ
                    //biz.CalculateAmountJobOnHandForDropdownlist(cmbOwnerBranch.SelectedItem.Value, source);
                    //AppUtil.BuildCombo(cmbOwner, source);
                    txtOldOwner.Text = ownerLeadUsername;
                    cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(ownerLeadUsername));
                }

                //Delegate Branch
                if (!string.IsNullOrEmpty(delegateBranchCode))
                {
                    txtOldDelegateBranch.Text = delegateBranchCode;
                    ListItem item = cmbDelegateBranch.Items.FindByValue(delegateBranchCode);
                    if (item != null)
                        cmbDelegateBranch.SelectedIndex = cmbDelegateBranch.Items.IndexOf(item);
                    else
                    {
                        //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
                        if (biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, txtCampaignId.Text.Trim(), delegateBranchCode))
                        {
                            //Branch ที่ถูกปิด
                            string branchName = biz.GetBranchName(delegateBranchCode);
                            if (!string.IsNullOrEmpty(branchName))
                            {
                                cmbDelegateBranch.Items.Insert(1, new ListItem(branchName, delegateBranchCode));
                                cmbDelegateBranch.SelectedIndex = 1;
                            }
                        }
                    }

                    cmbDelegateBranchSelectedIndexChanged(biz, delegateLeadUsername);    //Bind Combo Delegate
                }

                if (!string.IsNullOrEmpty(delegateLeadUsername))
                {
                    txtOldDelegate.Text = delegateLeadUsername;
                    cmbDelegate.SelectedIndex = cmbDelegate.Items.IndexOf(cmbDelegate.Items.FindByValue(delegateLeadUsername));
                }

                //Lead Status
                if (cmbLeadStatus.Items.Count > 0)
                {
                    cmbLeadStatus.SelectedIndex = cmbLeadStatus.Items.IndexOf(cmbLeadStatus.Items.FindByValue(statusCode));
                    txtOldStatus.Text = statusCode;
                }

                cmbLeadStatus.Enabled = assignedFlag == "1" ? true : false;
                if (assignedFlag == "1" && statusCode != "00")
                {
                    cmbLeadStatus.Items.Remove(cmbLeadStatus.Items.FindByValue("00"));  //ถ้าจ่ายงานแล้ว และสถานะปัจจุบันไม่ใช่สนใจ ให้เอาสถานะ สนใจ ออก
                }

                //เช็กสิทธิการแก้ไขข้อมูล
                if (txtAssignedFlag.Text.Trim() == "0" || txtDelegateFlag.Text.Trim() == "1")   //ยังไม่จ่ายงาน assignedFlag = 0, delegateFlag = 1
                {
                    cmbDelegateBranch.Enabled = false;
                    cmbDelegate.Enabled = false;
                    cmbOwnerBranch.Enabled = false;
                    cmbOwner.Enabled = false;
                    lblTab008Info.Text = "ไม่สามารถแก้ไข Owner และ Delegate ได้ เนื่องจากอยู่ระหว่างรอระบบจ่ายงาน กรุณารอ 1 นาที";
                }
                else
                {
                    biz.CheckOwnerPrivilege(ownerLeadUsername, delegateLeadUsername, cmbOwnerBranch, cmbOwner, cmbDelegateBranch, cmbDelegate, HttpContext.Current.User.Identity.Name);
                }

                upPopup.Update();
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        private void BindOwnerLead()
        {
			AppUtil.BuildCombo(cmbOwner, StaffBiz.GetStaffList(cmbOwnerBranch.SelectedValue));
            //cmbOwner.DataSource = StaffBiz.GetStaffList(cmbOwnerBranch.SelectedItem.Value);
            //cmbOwner.DataTextField = "TextField";
            //cmbOwner.DataValueField = "ValueField";
            //cmbOwner.DataBind();
            //cmbOwner.Items.Insert(0, new ListItem("", ""));
        }

        protected void cmbOwnerBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            SlmScr008Biz biz = null;
            try
            {
                biz = new SlmScr008Biz();
                cmbOwnerBranchSelectedIndexChanged(biz);
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ owner lead";
                }
                else
                    vcmbOwner.Text = "";

                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        private void cmbOwnerBranchSelectedIndexChanged(SlmScr008Biz biz, string ownerLead = null)
        {
            try
            {
                //Owner Lead
//                List<ControlListData> source = biz.GetStaffAllDataByAccessRight(txtCampaignId.Text.Trim(), cmbOwnerBranch.SelectedItem.Value);
                List<ControlListData> source = biz.GetStaffNotDummyAllDataByAccessRight(txtCampaignId.Text.Trim(), cmbOwnerBranch.SelectedItem.Value, ownerLead);
                //คำนวณงานในมือ
                biz.CalculateAmountJobOnHandForDropdownlist(cmbOwnerBranch.SelectedItem.Value, source);
				AppUtil.BuildCombo(cmbOwner, source);
                //cmbOwner.DataSource = source;
                //cmbOwner.DataTextField = "TextField";
                //cmbOwner.DataValueField = "ValueField";
                //cmbOwner.DataBind();
                //cmbOwner.Items.Insert(0, new ListItem("", ""));

                if (cmbOwnerBranch.SelectedItem.Value != string.Empty)
                    cmbOwner.Enabled = true;
                else
                    cmbOwner.Enabled = false;
            }
            catch
            {
                throw;
            }
        }

        protected void cmbDelegateBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            SlmScr008Biz biz = null;
            try
            {
                biz = new SlmScr008Biz();
                cmbDelegateBranchSelectedIndexChanged(biz);
                if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegate.SelectedItem.Value == string.Empty)
                {
                    vcmbDelegate.Text = "กรุณาระบุ Delegate Lead";
                }
                else
                    vcmbDelegate.Text = "";

                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        private void cmbDelegateBranchSelectedIndexChanged(SlmScr008Biz biz, string delegateOwner = null)
        {
            try
            {
                //Delegate Lead
//                List<ControlListData> source = biz.GetStaffAllDataByAccessRight(txtCampaignId.Text.Trim(), cmbDelegateBranch.SelectedItem.Value);
                List<ControlListData> source = biz.GetStaffNotDummyAllDataByAccessRight(txtCampaignId.Text.Trim(), cmbDelegateBranch.SelectedItem.Value, delegateOwner);
                biz.CalculateAmountJobOnHandForDropdownlist(cmbDelegateBranch.SelectedItem.Value, source);
				AppUtil.BuildCombo(cmbDelegate, source);
                //cmbDelegate.DataSource = source;
                //cmbDelegate.DataTextField = "TextField";
                //cmbDelegate.DataValueField = "ValueField";
                //cmbDelegate.DataBind();
                //cmbDelegate.Items.Insert(0, new ListItem("", ""));

                if (cmbDelegateBranch.SelectedItem.Value != string.Empty)
                    cmbDelegate.Enabled = true;
                else
                    cmbDelegate.Enabled = false;
            }
            catch
            {
                throw;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ClearData();
                mpePopup.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ClearData()
        {
            lblTab008Info.Text = "";
            txtTicketID.Text = "";
            txtAssignedFlag.Text = "";          //Hidden
            txtDelegateFlag.Text = "";          //Hidden
            txtFirstname.Text = "";
            txtLastname.Text = "";
            cmbCardType.SelectedIndex = -1;
            txtCitizenId.Text = "";
            cmbCountry.SelectedValue = "";
            txtCampaign.Text = "";
            txtCampaignId.Text = "";            //Hidden
            txtProductId.Text = "";             //Hidden
            cmbOwnerBranch.SelectedIndex = -1;
            txtOldOwnerBranch.Text = "";        //Hidden
            cmbOwner.SelectedIndex = -1;
            txtOldOwner.Text = "";              //Hidden
            cmbDelegateBranch.SelectedIndex = -1;
            txtOldDelegateBranch.Text = "";     //Hidden
            cmbDelegate.SelectedIndex = -1;
            txtOldDelegate.Text = "";           //Hidden
            txtTelNo1.Text = "";
            txtTelNoSms.Text = "";
            cmbLeadStatus.SelectedIndex = -1;
            txtOldStatus.Text = "";             //Hidden
            txtContactPhone.Text = "";
            txtContactDetail.Text = "";

            lblCitizenId.Text = "";
            vtxtCitizenId.Text = "";
            lblCountryId.Text = "";
            vcmbOwner.Text = "";
            vcmbOwnerBranch.Text = "";
            vcmbDelegate.Text = "";
            vcmbDelegateBranch.Text = "";
            vcmbLeadStatus.Text = "";
            vtxtContactDetail.Text = "";
            vTelNoSms.Text = "";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SlmScr008Biz biz = null;
            try
            {
                biz = new SlmScr008Biz();

                List<string> flagList = biz.GetAssignedFlagAndDelegateFlag(txtTicketID.Text.Trim());
                _currentAssignedFlag = flagList[0];
                _currentDelegateFlag = flagList[1];

				// check staff regisn
                StaffBiz sb = new StaffBiz();
                if (sb.IsStaffResign(cmbOwner.SelectedValue))
                {
                    AppUtil.ClientAlert(Page, "ไม่สามารถบันทึกผลการติดต่อได้เนื่องจาก Owner ลาออกแล้ว");
                    mpePopup.Show();
                    return;
                }
                if (cmbOwnerBranch.Items.Count > 0 && cmbOwner.Items.Count > 0)
                {
                    if (cmbOwnerBranch.SelectedItem.Value != txtOldOwnerBranch.Text.Trim() || cmbOwner.SelectedItem.Value != txtOldOwner.Text.Trim())
                    {
                        if (_currentAssignedFlag != txtAssignedFlag.Text.Trim())
                        {
                            AppUtil.ClientAlertAndRedirect(Page, "ไม่สามารถบันทึกผลการติดต่อได้ เนื่องจากมีคนเปลี่ยน Owner รบกวนรอ 1 นาที แล้วกลับมาบันทึกผลการติดต่อได้", "SLM_SCR_004.aspx?ticketid=" + txtTicketID.Text.Trim() + "&tab=008");
                            return;
                        }
                    }
                }

                if (cmbDelegateBranch.Items.Count > 0 && cmbDelegate.Items.Count > 0)
                {
                    if (cmbDelegateBranch.SelectedItem.Value != txtOldDelegateBranch.Text.Trim() || cmbDelegate.SelectedItem.Value != txtOldDelegate.Text.Trim())
                    {
                        if (_currentDelegateFlag != txtDelegateFlag.Text.Trim())
                        {
                            AppUtil.ClientAlertAndRedirect(Page, "ไม่สามารถบันทึกผลการติดต่อได้ เนื่องจากมีคนเปลี่ยน Delegate รบกวนรอ 1 นาที แล้วกลับมาบันทึกผลการติดต่อได้", "SLM_SCR_004.aspx?ticketid=" + txtTicketID.Text.Trim() + "&tab=008");
                            return;
                        }
                    }
                }

                if (ValidateData(biz))
                {
                    string externalSubStatusDesc = "";
                    int phonecallId = biz.InsertPhoneCallHistory(txtTicketID.Text.Trim(), cmbCardType.SelectedItem.Value, txtCitizenId.Text.Trim(), cmbLeadStatus.SelectedItem.Value, txtOldStatus.Text.Trim(), cmbOwnerBranch.SelectedItem.Value, cmbOwner.SelectedItem.Value, txtOldOwner.Text.Trim()
                                        , cmbDelegateBranch.SelectedItem.Value, cmbDelegate.SelectedItem.Value, txtOldDelegate.Text.Trim(), txtContactPhone.Text.Trim(), txtContactDetail.Text.Trim(), HttpContext.Current.User.Identity.Name, out externalSubStatusDesc, txtTelNoSms.Text.Trim(), cmbCountry.SelectedItem.Value);

                    //เก็บ log error calculate total sla
                    if (!string.IsNullOrEmpty(biz.CalculateTotalSlaError))
                    {
                        _log.Error(biz.CalculateTotalSlaError);
                    }

                    txtTicketIdSearch.Text = txtTicketID.Text.Trim();
                    txtCitizenIdSearch.Text = txtCitizenId.Text.Trim();
                    txtTelNo1Search.Text = txtTelNo1.Text.Trim();

                    string cbsSubscriptionTypeId ="";
                    if (cmbCardType.SelectedItem.Value != "")
                    {
                        cbsSubscriptionTypeId = biz.GetNewSubScriptionTypeId(Convert.ToInt32(cmbCardType.SelectedItem.Value));
                    }
                    
                    CreateCASActivityLog(cbsSubscriptionTypeId, phonecallId);

                    //DoBindGridview(biz, 0);
                    UpdatePhoneCallList(biz.CreatedDate, txtTicketID.Text.Trim(), txtCampaign.Text.Trim(), txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCountry.SelectedValue, cmbLeadStatus.SelectedItem.Text
                        , txtContactPhone.Text.Trim(), cmbOwner.SelectedItem.Text, txtContactDetail.Text.Trim(), txtLoginName.Text.Trim(), cmbCardType.SelectedItem.Text, txtCitizenId.Text.Trim(), biz);

                    if (cmbLeadStatus.SelectedItem.Value != txtOldStatus.Text.Trim())
                    {
                        CheckActivityConfig(biz, txtProductId.Text.Trim(), cmbLeadStatus.SelectedItem.Value);
                    }

                    //if (UpdatedDataChanged != null) UpdatedDataChanged(cmbLeadStatus.SelectedItem.Text, biz.CreatedDate);
                    bool doUpdateLogging = false;
                    if (cmbLeadStatus.SelectedItem.Value != txtOldStatus.Text.Trim() || cmbOwner.SelectedItem.Value != txtOldOwner.Text.Trim() || cmbDelegate.SelectedItem.Value != txtOldDelegate.Text.Trim())
                    {
                        doUpdateLogging = true;
                    }
                    if (UpdatedMainDataChanged != null)
                    {
                        UpdatedMainDataChanged(txtTicketID.Text.Trim(), cmbOwnerBranch.SelectedItem.Text, cmbOwner.SelectedItem.Text, cmbDelegateBranch.SelectedItem.Text, cmbDelegate.SelectedItem.Text
                          , txtTelNo1.Text.Trim(), cmbLeadStatus.SelectedItem.Text, biz.CreatedDate, doUpdateLogging, externalSubStatusDesc, cmbCardType.SelectedItem.Value, txtCitizenId.Text.Trim(), "", null);
                    }

                    UpdateCARScript(txtTicketID.Text.Trim(), "", cbsSubscriptionTypeId, txtCitizenId.Text.Trim());
                    
                    ClearData();
                    mpePopup.Hide();
                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
                }
                else
                    mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("TicketId " + txtTicketID.Text.Trim() + ", " + message);
                _log.Debug(ex);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        private void UpdatePhoneCallList(DateTime createdDate, string ticketId, string campaignName, string firstName, string lastName, string countryId, string statusDesc, string contactPhone
            , string ownerName, string contactDetail, string createdName, string cardTypeDesc, string citizenId, SlmScr008Biz biz)
        {
            try
            {
                if (Session[SessionPrefix+SLMConstant.SessionName.default_phonecall_list] != null)
                {
                    var list = (List<PhoneCallHistoryData>)Session[SessionPrefix + SLMConstant.SessionName.default_phonecall_list];

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
                        CountryId = Convert.ToInt32(countryId),
                        StatusDesc = statusDesc,
                        ContactPhone = contactPhone,
                        OwnerName = ownerName,
                        ContactDetail = contactDetail,
                        CreatedName = createdName,
                        CardTypeDesc = cardTypeDesc,
                        CitizenId = citizenId,
                    };
                    list.ForEach(p => p.CardTypeDesc = cardTypeDesc);
                    list.ForEach(p => p.CitizenId = citizenId);
                    list.Insert(0, newRow);

                    BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, list.ToArray(), 0);
                    Session[SessionPrefix + SLMConstant.SessionName.default_phonecall_list] = list;
                }
                else
                {
                    var result = biz.GetDefaultPhoneCallHistory(txtCitizenIdSearch.Text.Trim(), txtTicketIdSearch.Text.Trim(), cbThisLead.Checked);
                    BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), 0);
                }
            }
            catch
            {
                throw;
            }
        }

        private void UpdateCARScript(string ticketId, string preleadId, string cbsSubscriptionTypeId, string citizenId)
        {
            try
            {
                decimal? prelead_id = null;
                if (!string.IsNullOrEmpty(preleadId))
                    prelead_id = Convert.ToDecimal(preleadId);

                lbCasAllActivity.OnClientClick = new Services.CARService().GetCallCASScript(ticketId, prelead_id, cbsSubscriptionTypeId, citizenId, HttpContext.Current.User.Identity.Name);
            }
            catch
            {
                throw;
            }
        }

        private void CreateCASActivityLog(string cbsSubscriptionTypeId, int phonecallId)
        {
            try
            {
                List<Services.CARService.DataItem> actInfoList = new List<Services.CARService.DataItem>();
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "รายละเอียด", DataValue = txtContactDetail.Text.Trim() });

                List<Services.CARService.DataItem> cusInfoList = new List<Services.CARService.DataItem>();
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = txtCitizenId.Text.Trim() });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = cmbCardType.SelectedItem.Text });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = txtFirstname.Text.Trim() + " " + txtLastname.Text.Trim() });

                List<Services.CARService.DataItem> prodInfoList = new List<Services.CARService.DataItem>();
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = txtProductGroupName.Text.Trim() });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = txtProductName.Text.Trim() });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = txtCampaign.Text.Trim() });

                List<Services.CARService.DataItem> contInfoList = new List<Services.CARService.DataItem>();
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = "" });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = "" });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = "" });

                var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                List<Services.CARService.DataItem> offInfoList = new List<Services.CARService.DataItem>();
                offInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name });

                Services.CARService.CARServiceData carData = new Services.CARService.CARServiceData()
                {
                    ReferenceNo = phonecallId.ToString(),
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
                    CampaignId = txtCampaignId.Text.Trim(),
                    ChannelId = txtChannelId.Text.Trim(),
                    PreleadId = txtPreleadIdSearch.Text.Trim(),
                    ProductGroupId = txtProductGroupId.Text.Trim(),
                    ProductId = txtProductId.Text.Trim(),
                    Status = cmbLeadStatus.SelectedItem.Text,
                    SubStatus = "",
                    TicketId = txtTicketID.Text.Trim(),
                    SubscriptionId = txtCitizenId.Text.Trim(),
                    TypeId = SLMConstant.CARLogService.Data.TypeId,
                    AreaId = SLMConstant.CARLogService.Data.AreaId,
                    SubAreaId = SLMConstant.CARLogService.Data.SubAreaId,
                    ActivityTypeId = txtPreleadIdSearch.Text.Trim() != "" ? SLMConstant.CARLogService.Data.ActivityType.CallOutboundId : SLMConstant.CARLogService.Data.ActivityType.CallInboundId,
                    ContractNo = ""
                };

                //if (cmbCardType.SelectedItem.Value != "")
                //    carData.SubscriptionTypeId = biz.GetNewSubScriptionTypeId(Convert.ToInt32(cmbCardType.SelectedItem.Value));

                if (!string.IsNullOrEmpty(cbsSubscriptionTypeId))
                    carData.CIFSubscriptionTypeId = cbsSubscriptionTypeId;

                bool ret = Services.CARService.CreateActivityLog(carData);
                //SlmScr008Biz.UpdateCasFlag(phonecallId, ret ? "1" : "2");     //Comment by Pom 06/09/2016
            }
            catch (Exception ex)
            {
                //SlmScr008Biz.UpdateCasFlag(phonecallId, "2");     //Comment by Pom 06/09/2016

                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                _log.Debug(ex);
            }
        }

        //oldcasservice
        //private void CreateCASActivityLog(int phonecallId)
        //{
        //    try
        //    {
        //        List<Services.CASService.DataItem> actInfoList = new List<Services.CASService.DataItem>();
        //        actInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "รายละเอียด", DataValue = txtContactDetail.Text.Trim() });

        //        List<Services.CASService.DataItem> cusInfoList = new List<Services.CASService.DataItem>();
        //        cusInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = txtCitizenId.Text.Trim() });
        //        cusInfoList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = cmbCardType.SelectedItem.Text });
        //        cusInfoList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = txtFirstname.Text.Trim() + " " + txtLastname.Text.Trim() });

        //        List<Services.CASService.DataItem> prodInfoList = new List<Services.CASService.DataItem>();
        //        prodInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = txtProductGroupName.Text.Trim() });
        //        prodInfoList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = txtProductName.Text.Trim() });
        //        prodInfoList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = txtCampaign.Text.Trim() });

        //        List<Services.CASService.DataItem> contInfoList = new List<Services.CASService.DataItem>();
        //        contInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = "" });
        //        contInfoList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = "" });
        //        contInfoList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = "" });

        //        var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
        //        List<Services.CASService.DataItem> offInfoList = new List<Services.CASService.DataItem>();
        //        offInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name });

        //        Services.CASService.CASServiceData carData = new Services.CASService.CASServiceData()
        //        {
        //            ReferenceNo = phonecallId.ToString(),
        //            SecurityKey = txtPreleadIdSearch.Text.Trim() != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey,
        //            ServiceName = "CreateActivityLog",
        //            SystemCode = txtPreleadIdSearch.Text.Trim() != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM,      //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
        //            TransactionDateTime = DateTime.Now,
        //            ActivityInfoList = actInfoList,
        //            CustomerInfoList = cusInfoList,
        //            ProductInfoList = prodInfoList,
        //            ContractInfoList = contInfoList,
        //            OfficerInfoList = offInfoList,
        //            ActivityDateTime = DateTime.Now,
        //            CampaignId = txtCampaignId.Text.Trim(),
        //            ChannelId = txtChannelId.Text.Trim(),
        //            PreleadId = txtPreleadIdSearch.Text.Trim(),
        //            ProductGroupId = txtProductGroupId.Text.Trim(),
        //            ProductId = txtProductId.Text.Trim(),
        //            Status = cmbLeadStatus.SelectedItem.Text,
        //            SubStatus = "",
        //            TicketId = txtTicketID.Text.Trim(),
        //            SubscriptionId = txtCitizenId.Text.Trim(),
        //            TypeName = SLMConstant.CARLogService.Data.Type,
        //            AreaName = SLMConstant.CARLogService.Data.Area,
        //            SubAreaName = SLMConstant.CARLogService.Data.SubArea,
        //            ActivityType = txtPreleadIdSearch.Text.Trim() != "" ? SLMConstant.CARLogService.Data.ActivityType.CallOutbound : SLMConstant.CARLogService.Data.ActivityType.CallInbound,
        //            ContractNo = ""
        //        };

        //        if (cmbCardType.SelectedItem.Value != "")
        //            carData.SubscriptionTypeId = int.Parse(cmbCardType.SelectedItem.Value);

        //        bool ret = Services.CASService.CreateActivityLog(carData);
        //        SlmScr008Biz.UpdateCasFlag(phonecallId, ret ? "1" : "2");
        //    }
        //    catch (Exception ex)
        //    {
        //        SlmScr008Biz.UpdateCasFlag(phonecallId, "2");

        //        //Error ให้ลง Log ไว้ ไม่ต้อง Throw
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //    }
        //}

        protected void cmbCardType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lblCountryId.Text = "";
                if (cmbCardType.SelectedItem.Value == "")
                {
                    vtxtCardType.Text = "";
                    lblCitizenId.Text = "";
                    vtxtCitizenId.Text = "";
                    txtCitizenId.Text = "";
                    txtCitizenId.Enabled = false;
                    cmbCountry.Enabled = false;
                    cmbCountry.SelectedValue = "";
                }
                else
                {
                    vtxtCardType.Text = "";
                    lblCitizenId.Text = "*";
                    vtxtCitizenId.Text = "";
                    txtCitizenId.Enabled = true;
                    cmbCountry.Enabled = true;
                    AppUtil.SetCardTypeValidation(cmbCardType.SelectedItem.Value, txtCitizenId);
                    AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId);

                    //ถ้าเลือกบุคคลธรรม หรือนิติบุคคล ให้ Default ประเทศไทย ตาม Config
                    if (cmbCardType.SelectedValue == AppConstant.CardType.Person)
                    {
                        if (cmbCountry.SelectedValue == "")
                            cmbCountry.SelectedValue = AppConstant.CBSLeadThaiCountryId.ToString();
                    }

                    ////ถ้าเลือกชาวต่างชาติ ให้บังคับให้เลือกประเทศด้วย
                    //if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Foreigner)
                    //{
                    //    lblCountryId.Text = "*";
                    //    cmbCountry.Enabled = true;
                    //    cmbCountry.SelectedValue = "";
                    //}
                    //else
                    //{
                    //    //ถ้าเลือกบุคคลธรรม หรือนิติบุคคล ให้ Default ประเทศไทย ตาม Config
                    //    cmbCountry.SelectedValue = AppConstant.CBSLeadThaiCountryId.ToString();
                    //}
                }

                mpePopup.Show();
            }
            catch (Exception ex)
            {
                mpePopup.Show();

                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void txtCitizenId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId);
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidateData(SlmScr008Biz biz)
        {
            try
            {
                int i = 0;

                //เช็กสถานะ require cardId
                if (biz.CheckRequireCardId(cmbLeadStatus.SelectedItem.Value))
                {
                    if (cmbCardType.SelectedItem.Value == "")
                    {
                        i += 1;
                        vtxtCardType.Text = "กรุณาระบุประเภทบุคคล";
                    }
                    if (txtCitizenId.Text.Trim() == "")
                    {
                        i += 1;
                        vtxtCitizenId.Text = "กรุณาระบุเลขที่บัตร";
                    }

                    if (cmbCardType.SelectedItem.Value != "" && txtCitizenId.Text.Trim() != "")
                    {
                        if (!AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId))
                            i += 1;
                    }
                }
                else
                {
                    //Validate เลขที่บัตร
                    if (cmbCardType.SelectedItem.Value != "")
                    {
                        if (txtCitizenId.Text.Trim() == "")
                        {
                            vtxtCitizenId.Text = "กรุณาระบุเลขที่บัตร";
                            i += 1;
                        }
                        else
                        {
                            if (!AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId))
                                i += 1;
                        }

                        if (cmbCountry.SelectedValue == "")
                        {
                            vcmbCountry.Text = "กรุณาเลือกประเทศ";
                            i += 1;
                        }
                    }
                    else if (cmbCardType.SelectedItem.Value == "" && txtCitizenId.Text.Trim() != "")
                    {
                        vtxtCardType.Text = "กรุณาระบุประเภทบุคคล";
                        i += 1;
                    }
                    else
                    {
                        vtxtCardType.Text = "";
                        vtxtCitizenId.Text = "";
                        vcmbCountry.Text = "";
                    }
                }

                //OwnerBranch, Owner
                string clearOwnerBranchText = "Y";
                if (cmbOwnerBranch.SelectedItem.Value != txtOldOwnerBranch.Text.Trim() || cmbOwner.SelectedItem.Value != txtOldOwner.Text.Trim())
                {
                    if (!AppUtil.ValidateOwner(_currentAssignedFlag, cmbOwnerBranch, vcmbOwnerBranch, cmbOwner, vcmbOwner, txtCampaignId.Text.Trim(), ref clearOwnerBranchText))
                        i += 1;

                    //Branch ที่ถูกปิด
                    if (cmbOwnerBranch.Items.Count > 0 && cmbOwnerBranch.SelectedItem.Value != "" && !biz.CheckBranchActive(cmbOwnerBranch.SelectedItem.Value))
                    {
                        vcmbOwnerBranch.Text = "สาขานี้ถูกปิดแล้ว";
                        i += 1;
                    }
                    else
                    {
                        if (clearOwnerBranchText == "Y")
                            vcmbOwnerBranch.Text = "";
                    }
                }
                else
                {
                    vcmbOwnerBranch.Text = "";
                    vcmbOwner.Text = "";
                }

                //DelegateBranch, Delegate
                if (cmbDelegateBranch.SelectedItem.Value != txtOldDelegateBranch.Text.Trim() || cmbDelegate.SelectedItem.Value != txtOldDelegate.Text.Trim())
                {
                    if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegate.SelectedItem.Value == string.Empty)
                    {
                        vcmbDelegate.Text = "กรุณาระบุ Delegate Lead";
                        i += 1;
                    }
                    else
                        vcmbDelegate.Text = "";

                    if (cmbDelegateBranch.Items.Count > 0 && cmbDelegateBranch.SelectedItem.Value != "" && !biz.CheckBranchActive(cmbDelegateBranch.SelectedItem.Value))
                    {
                        vcmbDelegateBranch.Text = "สาขานี้ถูกปิดแล้ว";
                        i += 1;
                    }
                    else
                        vcmbDelegateBranch.Text = "";
                }
                else
                {
                    vcmbDelegateBranch.Text = "";
                    vcmbDelegate.Text = "";
                }

                //Added by Pom 29/06/2016
                if (cmbLeadStatus.Items.Count == 0 || cmbLeadStatus.SelectedItem.Value == "")
                {
                    vcmbLeadStatus.Text = "กรุณาเลือกสถานะ";
                    i += 1;
                }
                else
                {
                    vcmbLeadStatus.Text = "";
                }

                //หมายเลข SMS
                if (txtTelNoSms.Text.Trim() != "" && !AppUtil.ValidateTelNo1(cmbCardType.SelectedItem.Value, txtTelNoSms.Text.Trim()))
                {
                    vTelNoSms.Text = "กรุณาระบุหมายเลข SMS ให้ถูกต้อง";
                    i += 1;
                }
                else if (txtTelNoSms.Text.Trim() == "")
                {
                    vTelNoSms.Text = "กรุณาระบุหมายเลข SMS ให้ถูกต้อง";
                    i += 1;
                }
                else
                {
                    vTelNoSms.Text = "";
                }

                //รายละเอียดเพิ่มเติม
                if (txtContactDetail.Text.Trim() == "")
                {
                    vtxtContactDetail.Text = "กรุณากรอกข้อมูลรายละเอียดก่อนทำการบันทึก";
                    i += 1;
                }
                else
                    vtxtContactDetail.Text = "";

                upPopup.Update();

                return i > 0 ? false : true;
            }
            catch
            {
                throw;
            }
        }
        protected void cmbOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ owner lead";
                }
                else
                {
                    vcmbOwner.Text = "";
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void cmbDelegateLead_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegate.SelectedItem.Value == string.Empty)
                {
                    vcmbDelegate.Text = "กรุณาระบุ Delegate Lead";
                }
                else
                {
                    vcmbDelegate.Text = "";
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