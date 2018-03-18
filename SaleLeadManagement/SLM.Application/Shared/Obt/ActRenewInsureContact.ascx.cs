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

namespace SLM.Application.Shared.Obt
{
    public partial class ActRenewInsureContact : System.Web.UI.UserControl
    {
        public string TicketId;
        public string ProductId;
        public string LeadStatus;
        public DateTime CreatedDate = new DateTime();

        private string _currentAssignedFlag = "";
        private string _currentDelegateFlag = "";

        private static readonly ILog _log = LogManager.GetLogger(typeof(TabActRenewInsure));
        public int fileSize = AppUtil.SafeInt(ConfigurationManager.AppSettings["EcmFileUploadSize"]);

        //public event EventHandler SaveComplete;
        public delegate void UpdatedMainDataEvent(string ticketId, string ownerBranchName, string ownerLeadName, string delegateBranchName, string delegateLeadName, string telNo1, string statusDesc, DateTime contactLatestDate, bool doUpdateOwnerLogging, string externalSubStatusDesc, string cardTypeId, string citizenId, string subStatusCode, DateTime? nextContactDate);
        public event UpdatedMainDataEvent UpdatedMainDataChanged;
        public delegate void UpdatePhoneCallListChangedEvent(List<PhoneCallHistoryData> list);
        public event UpdatePhoneCallListChangedEvent UpdatePhoneCallListChanged;

        //Added By Pom 21/05/2016
        //public delegate void UpdatedDataEvent(string statusDesc);
        //public event UpdatedDataEvent UpdatedDataChanged;
        string _creditFormFilename = "";
        string _tawi50FormFilename = "";
        string _driverLicenseFilename = "";
        string _sessionSubstauslist = "sessionSubstauslist";
        public string SessionPrefix
        {
            get
            {
                var tabact = this.Parent as TabActRenewInsure;
                if (tabact != null)
                {
                    return tabact.SessionPrefix;
                }
                else
                {
                    return "";
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.SetIntTextBox(txtTicketID);
            AppUtil.SetIntTextBox(txtTelNoSms);
            AppUtil.SetIntTextBox(txtContactPhone);
            txtContactPhone.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
            AppUtil.SetMultilineMaxLength(txtContactDetail, vtxtContactDetail.ClientID, "500");
            AppUtil.SetIntTextBox(txtCitizenId);

            txtAppointment.OnTextChanged += new EventHandler(txtAppointment_OnTextChanged);

            AppUtil.SetAutoCompleteDropdown(new DropDownList[] { cmbOwnerBranch, cmbOwner, cmbDelegateBranch, cmbDelegate, cmbCountry }, Page, "AUTOCOMPLETESCRIPT");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && WarningAccept == null) WarningAccept = false;
            if (TicketId == null)
            {
                TicketId = (string)ViewState["ContractTicketID"];
            }
            else
            {
                ViewState["ContractTicketID"] = TicketId;
            }
        }

        public void show(string loginName)
        {
            RenewInsureBiz biz = null;
            try
            {
                _log.Debug("");
                _log.Debug($"Method=show, TicketId=FromViewState, StartLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                if (TicketId == null)
                {
                    TicketId = (string)ViewState["ContractTicketID"];
                }
                else
                {
                    ViewState["ContractTicketID"] = TicketId;
                }

                _log.Debug($"Method=show, TicketId={TicketId}, GetLeadDataPhoneCallHistory Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                biz = new RenewInsureBiz();
                LeadDataPhoneCallHistory data = biz.GetLeadDataPhoneCallHistory(TicketId.Trim());
                _log.Debug($"Method=show, TicketId={TicketId}, GetLeadDataPhoneCallHistory End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                _log.Debug($"Method=show, TicketId={TicketId}, GetRenewInsuranceData Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                var renewIns = biz.GetRenewInsuranceData(TicketId);
                _log.Debug($"Method=show, TicketId={TicketId}, GetRenewInsuranceData End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                if (data != null)
                {
                    txtLoginName.Text = loginName;
                    txtAssignedFlag.Visible = false;
                    txtDelegateFlag.Visible = false;
                    txtCampaignId.Visible = false;
                    txtPreleadId.Visible = false;
                    txtChannelId.Visible = false;
                    txtProductGroupId.Visible = false;
                    txtProductId.Visible = false;
                    txtProductGroupName.Visible = false;
                    txtProductName.Visible = false;
                    txtOldOwnerBranch.Visible = false;
                    txtOldOwner.Visible = false;
                    txtOldDelegateBranch.Visible = false;
                    txtOldDelegate.Visible = false;
                    txtOldStatus.Visible = false;
                    txtOldSubStatus.Visible = false;

                    ViewState["CampaignId"] = data.CampaignId;
                    ViewState["ProductId"] = data.ProductId;

                    _log.Debug($"Method=show, TicketId={TicketId}, checkBlackList Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    bool BlackListShow = biz.checkBlackList(HttpContext.Current.User.Identity.Name);
                    _log.Debug($"Method=show, TicketId={TicketId}, checkBlackList End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    txtContractNo.Text = data.ContractNo;
                    txtTicketID.Text = data.TicketId;
                    txtPreleadId.Text = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";  //Added By Pom 21/05/2016
                    txtFirstname.Text = data.Name;
                    txtLastname.Text = data.LastName;
                    txtChannelId.Text = data.ChannelId;                 //Added By Pom 21/05/2016
                    txtProductGroupId.Text = data.ProductGroupId;       //Added By Pom 21/05/2016
                    txtProductId.Text = data.ProductId;                 //Added By Pom 21/05/2016
                    txtProductGroupName.Text = data.ProductGroupName;
                    txtProductName.Text = data.ProductName;
                    txtCampaignId.Text = data.CampaignId;
                    txtCampaign.Text = data.CampaignName;
                    txtTelNo1.Text = data.TelNo1;
                    txtTelNoSms.Text = data.TelNoSms;
                    txtCitizenId.Text = data.CitizenId;

                    txtAppointment.DateValue = data.NextContactDate == null ? DateTime.MinValue : data.NextContactDate.Value;

                    if (data.NeedCreditFlag != null && data.NeedCreditFlag.Trim() != "")    //if (data.NeedCreditFlag != null && data.NeedCreditFlag.Trim().ToUpper() == "Y")
                    {
                        divCreditCard.Visible = true;
                        lblCreditCard.Visible = false;
                        imgCreditCard.Visible = false;
                        fuCreditCard.Visible = true;
                        clearCreditCard.Visible = true;
                    }
                    else
                    {
                        divCreditCard.Visible = false;
                        lblCreditCard.Visible = false;
                        imgCreditCard.Visible = false;
                        fuCreditCard.Visible = false;
                        clearCreditCard.Visible = false;
                    }

                    if (data.Need_50TawiFlag != null && data.Need_50TawiFlag.Trim() != "")  //if (data.Need_50TawiFlag != null && data.Need_50TawiFlag.Trim().ToUpper() == "Y")
                    {
                        div50tawi.Visible = true;
                        lbl50tawi.Visible = false;
                        img50tawi.Visible = false;
                        fu50tawi.Visible = true;
                        clear50tawi.Visible = true;
                    }
                    else
                    {
                        div50tawi.Visible = false;
                        lbl50tawi.Visible = false;
                        img50tawi.Visible = false;
                        fu50tawi.Visible = false;
                        clear50tawi.Visible = false;
                    }

                    if (data.NeedDriverLicense != null && data.NeedDriverLicense.Trim() != "")  //if (data.NeedDriverLicense != null && data.NeedDriverLicense.Trim().ToUpper() == "Y")
                    {
                        divDriverlicense.Visible = true;
                        lblDriverlicense.Visible = false;
                        imgDriverlicense.Visible = false;
                        fuDriverlicense.Visible = true;
                        clearDriverlicense.Visible = true;
                    }
                    else
                    {
                        divDriverlicense.Visible = false;
                        lblDriverlicense.Visible = false;
                        imgDriverlicense.Visible = false;
                        fuDriverlicense.Visible = false;
                        clearDriverlicense.Visible = false;
                    }

                    chkBlacklist.Checked = data.blacklist == null ? false : true;
                    hidBlacklist.Value = data.blacklist == null ? "" : data.blacklist.ToString();
                    chkBlacklist.Enabled = BlackListShow;
                    //if (BlackListShow)
                    //{
                    //    chkBlacklist.Enabled = true;
                    //}
                    //else
                    //{
                    //    chkBlacklist.Enabled = false;
                    //}

                    chkFollowpolicy.Enabled = renewIns.Count > 7 && renewIns[7] != "" ? true : false;
                    chkFollowAct.Enabled = renewIns.Count > 8 && renewIns[8] != "";
                    chkFollowpolicy.Checked = data.NeedPolicyFlag == "Y" ? true : false;
                    chkFollowpolicyOld.Checked = data.NeedPolicyFlag == "Y" ? true : false;
                    chkFollowAct.Checked = data.NeedCompulsoryFlag == "Y" ? true : false;
                    chkFollowActOld.Checked = data.NeedCompulsoryFlag == "Y" ? true : false;

                    txtAssignedFlag.Text = data.AssignedFlag;
                    txtDelegateFlag.Text = data.DelegateFlag != null ? data.DelegateFlag.ToString() : "";

                    _log.Debug($"Method=show, TicketId={TicketId}, InitialDropdownlist Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    InitialDropdownlist(biz, data);
                    _log.Debug($"Method=show, TicketId={TicketId}, InitialDropdownlist End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    cmbCountry.SelectedValue = "";
                    lblCountryId.Text = "";

                    if (data.CardType != null)
                    {
                        cmbCardType.SelectedIndex = cmbCardType.Items.IndexOf(cmbCardType.Items.FindByValue(data.CardType.Value.ToString()));
                        lblCitizenId.Text = "*";
                        txtCitizenId.Enabled = true;
                        txtCitizenId.Text = data.CitizenId;
                        lblCountryId.Text = "*";
                        AppUtil.SetCardTypeValidation(cmbCardType.SelectedItem.Value, txtCitizenId);
                    }
                    else
                    {
                        txtCitizenId.Text = data.CitizenId;
                    }

                    if (data.CountryId != null)
                        cmbCountry.SelectedValue = data.CountryId.ToString();

                    _log.Debug($"Method=show, TicketId={TicketId}, Check OwnerBranch, Owner AccessRight Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    //Owner Branch
                    if (!string.IsNullOrEmpty(data.OwnerBranch))
                    {
                        txtOldOwnerBranch.Text = data.OwnerBranch;
                        ListItem item = cmbOwnerBranch.Items.FindByValue(data.OwnerBranch);
                        if (item != null)
                        {
                            cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(item);
                        }
                        else
                        {
                            //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
                            if (biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, txtCampaignId.Text.Trim(), data.OwnerBranch))
                            {
                                //Branch ที่ถูกปิด
                                string branchName = biz.GetBranchName(data.OwnerBranch);
                                if (!string.IsNullOrEmpty(branchName))
                                {
                                    cmbOwnerBranch.Items.Insert(1, new ListItem(branchName, data.OwnerBranch));
                                    cmbOwnerBranch.SelectedIndex = 1;
                                }
                            }
                        }

                        cmbOwnerBranchSelectedIndexChanged(biz);   //Bind Combo Owner
                    }

                    //Owner
                    if (!string.IsNullOrEmpty(data.Owner))
                    {
                        txtOldOwner.Text = data.Owner;
                        cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(data.Owner));
                    }
                    _log.Debug($"Method=show, TicketId={TicketId}, Check OwnerBranch, Owner AccessRight End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    _log.Debug($"Method=show, TicketId={TicketId}, Check DelegateBranch, Delegate AccessRight Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    //Delegate Branch
                    if (!string.IsNullOrEmpty(data.DelegateBranch))
                    {
                        txtOldDelegateBranch.Text = data.DelegateBranch;
                        ListItem item = cmbDelegateBranch.Items.FindByValue(data.DelegateBranch);
                        if (item != null)
                        {
                            cmbDelegateBranch.SelectedIndex = cmbDelegateBranch.Items.IndexOf(item);
                        }
                        else
                        {
                            //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
                            if (biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, txtCampaignId.Text.Trim(), data.DelegateBranch))
                            {
                                //Branch ที่ถูกปิด
                                string branchName = biz.GetBranchName(data.DelegateBranch);
                                if (!string.IsNullOrEmpty(branchName))
                                {
                                    cmbDelegateBranch.Items.Insert(1, new ListItem(branchName, data.DelegateBranch));
                                    cmbDelegateBranch.SelectedIndex = 1;
                                }
                            }
                        }

                        cmbDelegateBranchSelectedIndexChanged(biz);    //Bind Combo Delegate
                    }

                    if (!string.IsNullOrEmpty(data.Delegate))
                    {
                        txtOldDelegate.Text = data.Delegate;
                        cmbDelegate.SelectedIndex = cmbDelegate.Items.IndexOf(cmbDelegate.Items.FindByValue(data.Delegate));
                    }
                    _log.Debug($"Method=show, TicketId={TicketId}, Check DelegateBranch, Delegate AccessRight End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");


                    _log.Debug($"Method=show, TicketId={TicketId}, The remaining Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    //Lead SubStatus
                    if (cmbLeadSubStatus.Items.Count > 0)
                    {
                        cmbLeadSubStatus.SelectedIndex = cmbLeadSubStatus.Items.IndexOf(cmbLeadSubStatus.Items.FindByValue(data.LeadSubStatus));
                        txtOldSubStatus.Text = data.LeadSubStatus;
                        hdfSubStatusType.Value = biz.GetSubStatusType(cmbLeadSubStatus.SelectedValue).ToString();
                    }

                    //Lead Status
                    if (cmbLeadStatus.Items.Count > 0)
                    {
                        cmbLeadStatus.SelectedIndex = cmbLeadStatus.Items.IndexOf(cmbLeadStatus.Items.FindByValue(data.LeadStatus));
                        txtOldStatus.Text = data.LeadStatus;
                    }

                    cmbLeadSubStatus.Enabled = data.AssignedFlag == "1" ? true : false;

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
                        biz.CheckOwnerPrivilege(data.Owner, data.Delegate, cmbOwnerBranch, cmbOwner, cmbDelegateBranch, cmbDelegate, HttpContext.Current.User.Identity.Name);
                    }

                    // prepare data for client validate
                    hdfPaidPolicy.Value = ActivityLeadBiz.checkPaid(TicketId.Trim()) ? "Y" : "N";
                    hdfPaidAct.Value = ActivityLeadBiz.checkPaidAct(TicketId.Trim()) ? "Y" : "N";

                    UpdateAllPanel();
                    mpePopup.Show();

                    _log.Debug($"Method=show, TicketId={TicketId}, The remaining End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                }
                else
                {
                    AppUtil.ClientAlert(Page, "ไม่พบ Ticket Id " + TicketId.Trim() + " ในระบบ");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                _log.Debug(ex);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                _log.Debug($"Method=show, TicketId=FromViewState, EndLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                if (biz != null)
                {
                    biz.Dispose();
                }
            }
        }

        protected void cmbLeadSubStatus_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var biz = new RenewInsureBiz();
                hdfSubStatusType.Value = biz.GetSubStatusType(cmbLeadSubStatus.SelectedValue).ToString();

                if (cmbLeadSubStatus.SelectedItem.Value == "")
                {
                    vcmbLeadSubStatus.Text = "กรุณาระบุสถานะย่อยของ Lead";
                }
                else
                {
                    vcmbLeadSubStatus.Text = "";

                    if (Session[SessionPrefix+_sessionSubstauslist] != null)
                    {
                        List<ConfigProductSubStatusData> data = (List<ConfigProductSubStatusData>)Session[SessionPrefix+_sessionSubstauslist];
                        string statusCode = data.Where(p => p.SubStatusCode == cmbLeadSubStatus.SelectedItem.Value).Select(p => p.StatusCode).FirstOrDefault();
                        if (!string.IsNullOrEmpty(statusCode))
                        {
                            cmbLeadStatus.SelectedIndex = cmbLeadStatus.Items.IndexOf(cmbLeadStatus.Items.FindByValue(statusCode));
                        }
                    }
                    else
                    {
                        List<string> statusValueList = new List<string>();
                        foreach (ListItem item in cmbLeadStatus.Items)
                        {
                            statusValueList.Add(item.Value);
                        }
                        List<ConfigProductSubStatusData> tempSubList = new ConfigProductSubStatusBiz().GetSubStatusListByStatusActivityConfig(txtProductId.Text.Trim(), txtCampaignId.Text.Trim(), statusValueList);
                        Session[SessionPrefix+_sessionSubstauslist] = tempSubList;        //เก็บไว้ใช้ใน Method cmbLeadSubStatus_OnSelectedIndexChanged()
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

        protected void cmbLeadStatus_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var CampaignId = (string)ViewState["CampaignId"];
                var ProductId = (string)ViewState["ProductId"];
                var LeadStatus = cmbLeadStatus.SelectedValue;
                List<ControlListData> SubStatusList = LeadInfoBiz.getSubStatusLead(CampaignId, ProductId, LeadStatus);
                cmbLeadSubStatus.DataSource = SubStatusList;
                cmbLeadSubStatus.DataTextField = "TextField";
                cmbLeadSubStatus.DataValueField = "ValueField";
                cmbLeadSubStatus.DataBind();
                cmbLeadSubStatus.Items.Insert(0, new ListItem("", ""));

                if (cmbLeadStatus.SelectedValue == "")
                {
                    vcmbLeadStatus.Text = "กรุณาระบุสถานะของ Lead";
                }
                else
                {
                    vcmbLeadStatus.Text = "";
                }

                if (cmbLeadSubStatus.SelectedValue == "")
                {
                    vcmbLeadSubStatus.Text = "กรุณาระบุสถานะย่อยของ Lead";
                }
                else
                {
                    vcmbLeadSubStatus.Text = "";
                }

                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void InitialDropdownlist(RenewInsureBiz biz, LeadDataPhoneCallHistory data)
        {
            try
            {
                //ประเภทบุคคล
                cmbCardType.DataSource = biz.GetCardTypeList();
                cmbCardType.DataTextField = "TextField";
                cmbCardType.DataValueField = "ValueField";
                cmbCardType.DataBind();
                cmbCardType.Items.Insert(0, new ListItem("", ""));

                var branchList = biz.GetBranchAccessRightList(SLMConstant.Branch.Active, data.CampaignId);
                cmbOwnerBranch.DataSource = branchList;
                cmbOwnerBranch.DataTextField = "TextField";
                cmbOwnerBranch.DataValueField = "ValueField";
                cmbOwnerBranch.DataBind();
                cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));
                cmbOwner.Items.Insert(0, new ListItem("", ""));

                if (cmbOwnerBranch.SelectedItem.Value != string.Empty)
                {
                    cmbOwner.Enabled = true;
                }
                else
                {
                    cmbOwner.Enabled = false;
                }

                cmbDelegateBranch.DataSource = branchList;
                cmbDelegateBranch.DataTextField = "TextField";
                cmbDelegateBranch.DataValueField = "ValueField";
                cmbDelegateBranch.DataBind();
                cmbDelegateBranch.Items.Insert(0, new ListItem("", ""));
                cmbDelegate.Items.Insert(0, new ListItem("", ""));

                if (cmbDelegateBranch.SelectedItem.Value != string.Empty)
                {
                    cmbDelegate.Enabled = true;
                }
                else
                {
                    cmbDelegate.Enabled = false;
                }

                //Status
                List<ControlListData> statusList = biz.GetStatusListByActivityConfig(data.ProductId, data.LeadStatus);
                cmbLeadStatus.DataSource = statusList;
                cmbLeadStatus.DataTextField = "TextField";
                cmbLeadStatus.DataValueField = "ValueField";
                cmbLeadStatus.DataBind();
                cmbLeadStatus.Items.Insert(0, new ListItem("", ""));

                //SubStatus
                List<string> statusValueList = statusList.Select(p => p.ValueField).ToList();
                List<ConfigProductSubStatusData> tempSubList = biz.GetSubStatusListByStatusActivityConfig(data.ProductId, data.CampaignId, statusValueList);
                Session[SessionPrefix+_sessionSubstauslist] = tempSubList;        //เก็บไว้ใช้ใน Method cmbLeadSubStatus_OnSelectedIndexChanged()

                cmbLeadSubStatus.DataSource = tempSubList.Select(p => new ControlListData() { TextField = p.SubStatusName, ValueField = p.SubStatusCode }).ToList();
                cmbLeadSubStatus.DataTextField = "TextField";
                cmbLeadSubStatus.DataValueField = "ValueField";
                cmbLeadSubStatus.DataBind();
                cmbLeadSubStatus.Items.Insert(0, new ListItem("", ""));

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

        private void BindOwnerLead()
        {
            cmbOwner.DataSource = StaffBiz.GetStaffList(cmbOwnerBranch.SelectedItem.Value);
            cmbOwner.DataTextField = "TextField";
            cmbOwner.DataValueField = "ValueField";
            cmbOwner.DataBind();
            cmbOwner.Items.Insert(0, new ListItem("", ""));
        }

        protected void cmbOwnerBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenewInsureBiz biz = null;
            try
            {
                biz = new RenewInsureBiz();
                cmbOwnerBranchSelectedIndexChanged(biz);
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ Owner Lead";
                }
                else
                {
                    vcmbOwner.Text = "";
                }
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

        private void cmbOwnerBranchSelectedIndexChanged(RenewInsureBiz biz)
        {
            try
            {
                //Owner Lead
                List<ControlListData> source = biz.GetStaffAllDataByAccessRight(txtCampaignId.Text.Trim(), cmbOwnerBranch.SelectedItem.Value);
                //คำนวณงานในมือ
                biz.CalculateAmountJobOnHandForDropdownlist(cmbOwnerBranch.SelectedItem.Value, source);
                cmbOwner.DataSource = source;
                cmbOwner.DataTextField = "TextField";
                cmbOwner.DataValueField = "ValueField";
                cmbOwner.DataBind();
                cmbOwner.Items.Insert(0, new ListItem("", ""));

                if (cmbOwnerBranch.SelectedItem.Value != string.Empty)
                {
                    cmbOwner.Enabled = true;
                }
                else
                {
                    cmbOwner.Enabled = false;
                }
            }
            catch
            {
                throw;
            }
        }

        protected void cmbDelegateBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenewInsureBiz biz = null;
            try
            {
                biz = new RenewInsureBiz();
                cmbDelegateBranchSelectedIndexChanged(biz);
                if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegate.SelectedItem.Value == string.Empty)
                {
                    vcmbDelegate.Text = "กรุณาระบุ Delegate Lead";
                }
                else
                {
                    vcmbDelegate.Text = "";
                }
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

        private void cmbDelegateBranchSelectedIndexChanged(RenewInsureBiz biz)
        {
            try
            {
                //Delegate Lead
                List<ControlListData> source = biz.GetStaffAllDataByAccessRight(txtCampaignId.Text.Trim(), cmbDelegateBranch.SelectedItem.Value);
                biz.CalculateAmountJobOnHandForDropdownlist(cmbDelegateBranch.SelectedItem.Value, source);
                cmbDelegate.DataSource = source;
                cmbDelegate.DataTextField = "TextField";
                cmbDelegate.DataValueField = "ValueField";
                cmbDelegate.DataBind();
                cmbDelegate.Items.Insert(0, new ListItem("", ""));

                if (cmbDelegateBranch.SelectedItem.Value != string.Empty)
                {
                    cmbDelegate.Enabled = true;
                }
                else
                {
                    cmbDelegate.Enabled = false;
                }
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
                Session.Remove(_sessionSubstauslist);
                ClearData();
                UpdateAllPanel();

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
            txtCampaign.Text = "";
            txtCampaignId.Text = "";            //Hidden
            txtPreleadId.Text = "";             //Hidden
            txtChannelId.Text = "";             //Hidden
            txtProductGroupId.Text = "";        //Hidden
            txtProductId.Text = "";             //Hidden
            txtProductGroupName.Text = "";
            txtProductName.Text = "";
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
            txtOldSubStatus.Text = "";
            txtContactPhone.Text = "";
            txtContactDetail.Text = "";

            lblCitizenId.Text = "";
            vtxtCitizenId.Text = "";
            vcmbOwner.Text = "";
            vcmbOwnerBranch.Text = "";
            vcmbDelegate.Text = "";
            vcmbDelegateBranch.Text = "";
            vtxtContactDetail.Text = "";
            vcmbLeadSubStatus.Text = "";
            vTelNoSms.Text = "";
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            doSaveStatus();
        }

        private void doSaveStatus()
        {
            RenewInsureBiz biz = null;
            RenewPhoneCallHistoryData renewPhoneCall = new RenewPhoneCallHistoryData();
            try
            {
                _log.Debug("");
                _log.Debug($"Method=btnSave_Click, TicketId={TicketId}, StartLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                biz = new RenewInsureBiz();
                List<string> flagList = biz.GetAssignedFlagAndDelegateFlag(TicketId.Trim());
                _currentAssignedFlag = flagList[0];
                _currentDelegateFlag = flagList[1];

                if (cmbOwnerBranch.Items.Count > 0 && cmbOwner.Items.Count > 0)
                {
                    if (cmbOwnerBranch.SelectedItem.Value != txtOldOwnerBranch.Text.Trim() || cmbOwner.SelectedItem.Value != txtOldOwner.Text.Trim())
                    {
                        if (_currentAssignedFlag != txtAssignedFlag.Text.Trim())
                        {
                            AppUtil.ClientAlertAndRedirect(Page, "ไม่สามารถบันทึกผลการติดต่อได้ เนื่องจากมีคนเปลี่ยน Owner รบกวนรอ 1 นาที แล้วกลับมาบันทึกผลการติดต่อได้", "SLM_SCR_004.aspx?ticketid=" + TicketId.Trim() + "&tab=008");
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
                            AppUtil.ClientAlertAndRedirect(Page, "ไม่สามารถบันทึกผลการติดต่อได้ เนื่องจากมีคนเปลี่ยน Delegate รบกวนรอ 1 นาที แล้วกลับมาบันทึกผลการติดต่อได้", "SLM_SCR_004.aspx?ticketid=" + TicketId.Trim() + "&tab=008");
                            return;
                        }
                    }
                }

                if (!ValidateData(biz))
                {
                    mpePopup.Show();
                }
                else
                {
                    _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, PrepareData Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    string slm_CreditFileName = null;
                    string slm_CreditFilePath = null;
                    int? slm_CreditFileSize = null;
                    string slm_50TawiFileName = null;
                    string slm_50TawiFilePath = null;
                    int? slm_50TawiFileSize = null;
                    string slm_DriverLicenseiFileName = null;
                    string slm_DriverLicenseFilePath = null;
                    int? slm_DriverLicenseFileSize = null;
                    DateTime? slm_NextContactDate = null;
                    decimal? slm_cp_blacklist_id = hidBlacklist.Value == "" ? null : (decimal?)AppUtil.SafeDecimal(hidBlacklist.Value);

                    // เปลี่ยนไปดึงค่าจาก viewstate แทน
                    if (fuCreditCard.HasFile)
                    {
                        slm_CreditFileName = _creditFormFilename;           //ได้จาก Method ValidateData()
                        slm_CreditFilePath = SLMConstant.Ecm.SitePath + TicketId.Trim() + "/";
                        slm_CreditFileSize = fuCreditCard.FileBytes.Length;
                    }

                    if (fu50tawi.HasFile)
                    {
                        slm_50TawiFileName = _tawi50FormFilename;           //ได้จาก Method ValidateData()
                        slm_50TawiFilePath = SLMConstant.Ecm.SitePath + TicketId.Trim() + "/";
                        slm_50TawiFileSize = fu50tawi.FileBytes.Length;
                    }

                    if (fuDriverlicense.HasFile)
                    {
                        slm_DriverLicenseiFileName = _driverLicenseFilename;
                        slm_DriverLicenseFilePath = SLMConstant.Ecm.SitePath + TicketId.Trim() + "/";
                        slm_DriverLicenseFileSize = fuDriverlicense.FileBytes.Length;
                    }

                    var blacklist = chkBlacklist.Checked;

                    slm_NextContactDate = txtAppointment.DateValue == DateTime.MinValue ? null : (DateTime?)txtAppointment.DateValue;

                    //Modified By Pom 21/05/2016
                    //เพิ่ม return phoneCallId
                    DateTime createdDate = DateTime.Now;
                    string externalSubStatusDesc = cmbLeadSubStatus.SelectedItem.Text;

                    renewPhoneCall.ticketId = TicketId.Trim();
                    renewPhoneCall.cardType = cmbCardType.SelectedItem.Value;
                    renewPhoneCall.cardId = txtCitizenId.Text.Trim();
                    renewPhoneCall.countryId = cmbCountry.SelectedValue;
                    renewPhoneCall.leadStatusCode = cmbLeadStatus.SelectedItem.Value;
                    renewPhoneCall.oldstatus = txtOldStatus.Text.Trim();
                    renewPhoneCall.ownerBranch = cmbOwnerBranch.SelectedItem.Value;
                    renewPhoneCall.owner = cmbOwner.SelectedItem.Value;
                    renewPhoneCall.oldOwner = txtOldOwner.Text.Trim();
                    renewPhoneCall.delegateLeadBranch = cmbDelegateBranch.SelectedItem.Value;
                    renewPhoneCall.delegateLead = cmbDelegate.SelectedItem.Value;
                    renewPhoneCall.oldDelegateLead = txtOldDelegate.Text.Trim();
                    renewPhoneCall.contactPhone = txtContactPhone.Text.Trim();
                    renewPhoneCall.contactDetail = txtContactDetail.Text.Trim();
                    renewPhoneCall.createBy = HttpContext.Current.User.Identity.Name;
                    renewPhoneCall.subStatusCode = cmbLeadSubStatus.SelectedValue;
                    renewPhoneCall.subStatusDesc = cmbLeadSubStatus.SelectedItem.Text;
                    renewPhoneCall.Need_CompulsoryFlag = chkFollowAct.Checked;
                    renewPhoneCall.Need_PolicyFlag = chkFollowpolicy.Checked;
                    renewPhoneCall.Need_CompulsoryFlagOld = chkFollowActOld.Checked;
                    renewPhoneCall.Need_PolicyFlagOld = chkFollowpolicyOld.Checked;
                    renewPhoneCall.slm_CreditFileName = slm_CreditFileName;
                    renewPhoneCall.slm_CreditFilePath = slm_CreditFilePath;
                    renewPhoneCall.slm_CreditFileSize = slm_CreditFileSize;
                    renewPhoneCall.slm_50TawiFileName = slm_50TawiFileName;
                    renewPhoneCall.slm_50TawiFilePath = slm_50TawiFilePath;
                    renewPhoneCall.slm_50TawiFileSize = slm_50TawiFileSize;
                    renewPhoneCall.slm_DriverLicenseiFileName = slm_DriverLicenseiFileName;
                    renewPhoneCall.slm_DriverLicenseFilePath = slm_DriverLicenseFilePath;
                    renewPhoneCall.slm_DriverLicenseFileSize = slm_DriverLicenseFileSize;
                    renewPhoneCall.slm_NextContactDate = slm_NextContactDate;
                    renewPhoneCall.slm_cp_blacklist_id = slm_cp_blacklist_id;
                    renewPhoneCall.Blacklist = blacklist;
                    renewPhoneCall.createdDate = createdDate;
                    renewPhoneCall.externalSubStatusDesc = externalSubStatusDesc;
                    renewPhoneCall.TelNoSms = txtTelNoSms.Text.Trim();
                    _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, PrepareData End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, InsertRenewPhoneCallHistory Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    int phoneCallId = biz.InsertRenewPhoneCallHistory(renewPhoneCall);
                    _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, InsertRenewPhoneCallHistory End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    //เก็บ log error calculate total sla
                    if (!string.IsNullOrEmpty(biz.CalculateTotalSlaError))
                    {
                        _log.Error(biz.CalculateTotalSlaError);
                    }

                    //Added By Pom 21/05/2016
                    _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, CreateCASActivityLog Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    CreateCASActivityLog(biz, phoneCallId, slm_CreditFilePath, slm_CreditFileName, slm_50TawiFilePath, slm_50TawiFileName, slm_DriverLicenseFilePath, slm_DriverLicenseiFileName);
                    _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, CreateCASActivityLog End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    ProductId = txtProductId.Text.Trim();
                    LeadStatus = cmbLeadStatus.Text.Trim();
                    CreatedDate = createdDate;
                    biz.CloseConnection();

                    _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, UpdatePhoneCallList Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    UpdatePhoneCallList(createdDate, TicketId.Trim(), txtCampaign.Text.Trim(), txtFirstname.Text.Trim(), txtLastname.Text.Trim(), cmbCountry.SelectedValue, cmbLeadStatus.SelectedItem.Text, txtContactPhone.Text.Trim(), cmbOwner.SelectedItem.Text, txtContactDetail.Text.Trim(), txtLoginName.Text.Trim()
                            , cmbCardType.SelectedItem.Text, txtCitizenId.Text.Trim(), slm_CreditFilePath, slm_CreditFileName, slm_50TawiFilePath, slm_50TawiFileName, slm_DriverLicenseFilePath, slm_DriverLicenseiFileName);
                    _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, UpdatePhoneCallList End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    if (TicketId == Request["ticketid"])
                    {
                        bool doUpdateLogging = false;
                        //ไม่ต้อง update ownerlogging เพื่อ improve performance
                        //if (cmbLeadStatus.SelectedItem.Value != txtOldStatus.Text.Trim() || cmbOwner.SelectedItem.Value != txtOldOwner.Text.Trim() || cmbDelegate.SelectedItem.Value != txtOldDelegate.Text.Trim() || cmbLeadSubStatus.SelectedItem.Value != txtOldSubStatus.Text.Trim())
                        //{
                        //    doUpdateLogging = true;
                        //}
                        if (UpdatedMainDataChanged != null)
                        {
                            _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, UpdatedMainDataChanged Start={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                            UpdatedMainDataChanged(txtTicketID.Text.Trim(), cmbOwnerBranch.SelectedItem.Text, cmbOwner.SelectedItem.Text, cmbDelegateBranch.SelectedItem.Text, cmbDelegate.SelectedItem.Text
                            , txtTelNo1.Text.Trim(), cmbLeadStatus.SelectedItem.Text, createdDate, doUpdateLogging, externalSubStatusDesc, cmbCardType.SelectedItem.Value, txtCitizenId.Text.Trim(), cmbLeadSubStatus.SelectedItem.Value, txtAppointment.DateValue);
                            _log.Debug($"Method=btnSave_Click, TicketId={TicketId.Trim()}, UpdatedMainDataChanged End={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                        }
                    }

                    ClearData();
                    UpdateAllPanel();
                    mpePopup.Hide();
                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                _log.Debug(ex);
                mpePopup.Show(); // Call panel
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                _log.Debug($"Method=btnSave_Click, TicketId={txtTicketID.Text.Trim()}, EndLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                _log.Debug("");

                if (biz != null)
                {
                    biz.Dispose();
                } 
            }
        }

        private void UpdatePhoneCallList(DateTime createdDate, string ticketId, string campaignName, string firstName, string lastName, string countryid, string statusDesc, string contactPhone
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
                        CountryId = Convert.ToInt32(countryid),
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

                    Session[SessionPrefix+SLMConstant.SessionName.renewinsure_phonecall_list] = list;
                    if (UpdatePhoneCallListChanged != null)
                    {
                        UpdatePhoneCallListChanged(list);
                    }
                }
                else
                {
                    if (UpdatePhoneCallListChanged != null)
                    {
                        UpdatePhoneCallListChanged(null);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void UpdateAllPanel()
        {
            upSection1.Update();
            upDropdownlist.Update();
            upStatusSection.Update();
            upBrowseFile.Update();
        }

        private void CreateCASActivityLog(RenewInsureBiz biz, int phonecallId, string creditFilePath, string creditFileName, string tawi50FilePath, string tawi50FileName, string driverLicenseFilePath, string driverLicenseFileName)
        {
            try
            {
                string nextContactDate = "";
                if (txtAppointment.DateValue.Year != 1)
                {
                    nextContactDate = txtAppointment.DateValue.ToString("dd/MM/") + txtAppointment.DateValue.Year.ToString();
                }

                var list = biz.GetRenewInsuranceData(txtTicketID.Text.Trim());
                string policyFlag = "";
                string actFlag = "";
                string creditForm = "";
                string tawi50Form = "";
                string driverForm = "";
                string carLicenseNo = "";
                string contractNo = "";

                if (list.Count > 0)
                {
                    string ecmUrl = SLMConstant.Ecm.SiteUrl + "/" + SLMConstant.Ecm.ListName + "/" + txtTicketID.Text.Trim() + "/";
                    policyFlag = list[0];
                    actFlag = list[1];
                    creditForm = string.IsNullOrEmpty(creditFileName) ? "" : (creditFileName + " / " + ecmUrl + creditFileName);
                    tawi50Form = string.IsNullOrEmpty(tawi50FileName) ? "" : (tawi50FileName + " / " + ecmUrl + tawi50FileName);
                    driverForm = string.IsNullOrEmpty(driverLicenseFileName) ? "" : (driverLicenseFileName + " / " + ecmUrl + driverLicenseFileName);
                    carLicenseNo = list[5];
                    contractNo = list[6];
                }

                //Activity Info
                List<Services.CARService.DataItem> actInfoList = new List<Services.CARService.DataItem>();
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "วันที่นัดหมายครั้งถัดไป", DataValue = nextContactDate });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ลูกค้าติดตามกรมธรรม์", DataValue = policyFlag });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ลูกค้าติดตามเลข พรบ.", DataValue = actFlag });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 4, DataLabel = "Black List", DataValue = chkBlacklist.Checked ? "Y" : "N" });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 5, DataLabel = "ฟอร์มตัดบัตรเครดิต", DataValue = creditForm });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 6, DataLabel = "ฟอร์มสำเนา 50 ทวิ", DataValue = tawi50Form });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 7, DataLabel = "สำเนาใบขับขี่", DataValue = driverForm });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 8, DataLabel = "รายละเอียด", DataValue = txtContactDetail.Text.Trim() });

                //Customer Info
                List<Services.CARService.DataItem> cusInfoList = new List<Services.CARService.DataItem>();
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = txtCitizenId.Text.Trim() });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = cmbCardType.SelectedItem.Text });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = txtFirstname.Text.Trim() + " " + txtLastname.Text.Trim() });

                //Product Info
                List<Services.CARService.DataItem> prodInfoList = new List<Services.CARService.DataItem>();
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = txtProductGroupName.Text.Trim() });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = txtProductName.Text.Trim() });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = txtCampaign.Text.Trim() });

                //Contract Info
                List<Services.CARService.DataItem> contInfoList = new List<Services.CARService.DataItem>();
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = contractNo });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = "HP" });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = carLicenseNo });

                //Officer Info
                var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                List<Services.CARService.DataItem> offInfoList = new List<Services.CARService.DataItem>();
                offInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name });

                Services.CARService.CARServiceData carData = new Services.CARService.CARServiceData()
                {
                    ReferenceNo = phonecallId.ToString(),
                    SecurityKey = txtPreleadId.Text.Trim() != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey,
                    ServiceName = "CreateActivityLog",
                    SystemCode = txtPreleadId.Text.Trim() != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM,      //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
                    TransactionDateTime = DateTime.Now,
                    ActivityInfoList = actInfoList,
                    CustomerInfoList = cusInfoList,
                    ProductInfoList = prodInfoList,
                    ContractInfoList = contInfoList,
                    OfficerInfoList = offInfoList,
                    ActivityDateTime = DateTime.Now,
                    CampaignId = txtCampaignId.Text.Trim(),
                    ChannelId = txtChannelId.Text.Trim(),
                    PreleadId = txtPreleadId.Text.Trim(),
                    ProductGroupId = txtProductGroupId.Text.Trim(),
                    ProductId = txtProductId.Text.Trim(),
                    Status = cmbLeadStatus.SelectedItem.Text,
                    SubStatus = cmbLeadSubStatus.SelectedItem.Text,
                    TicketId = txtTicketID.Text.Trim(),
                    SubscriptionId = txtCitizenId.Text.Trim(),
                    TypeId = SLMConstant.CARLogService.Data.TypeId,
                    AreaId = SLMConstant.CARLogService.Data.AreaId,
                    SubAreaId = SLMConstant.CARLogService.Data.SubAreaId,
                    ActivityTypeId = txtPreleadId.Text.Trim() != "" ? SLMConstant.CARLogService.Data.ActivityType.CallOutboundId : SLMConstant.CARLogService.Data.ActivityType.CallInboundId,
                    ContractNo = contractNo
                };

                if (cmbCardType.SelectedItem.Value != "")
                    carData.CIFSubscriptionTypeId = biz.GetSubScriptionTypeId(Convert.ToInt32(cmbCardType.SelectedItem.Value));

                bool ret = Services.CARService.CreateActivityLog(carData);
                //SlmScr008Biz.UpdateCasFlag(phonecallId, ret ? "1" : "2");     //Comment by Pom 06/09/2016
            }
            catch (Exception ex)
            {
                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
            }
        }

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
                    lblCountryId.Text = "*";
                    AppUtil.SetCardTypeValidation(cmbCardType.SelectedItem.Value, txtCitizenId);
                    AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId);

                    if (cmbCardType.SelectedValue == AppConstant.CardType.Person)
                    {
                        if (cmbCountry.SelectedValue == "")
                            cmbCountry.SelectedValue = AppConstant.CBSLeadThaiCountryId.ToString();
                    }

                    //if (cmbCardType.SelectedValue == AppConstant.CardType.Foreigner)
                    //{
                    //    cmbCountry.Enabled = true;
                    //    lblCountryId.Text = "*";
                    //}
                    //else
                    //    cmbCountry.SelectedValue = AppConstant.CBSLeadThaiCountryId.ToString();
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

        private string GenFileName(string ticketId, string fileNameWithoutExt, string extension, string flag)
        {
            try
            {
                Random rnd = new Random();
                int random = rnd.Next(0, 999);
                string date = DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd") + DateTime.Now.ToString("HHmmss");
                fileNameWithoutExt = fileNameWithoutExt.Replace(" ", "_").Replace("%","_");

                return fileNameWithoutExt + "_" + date + random.ToString("000") + extension;
            }
            catch
            {
                throw;
            }
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
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
            }
        }

        private bool ValidateData(RenewInsureBiz biz)
        {
            _log.Debug($"Method=ValidateData, TicketId={TicketId.Trim()}, StartLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

            try
            {
                int i = 0;
                //ถ้าสถานะหลักเป็น cancel หรือ reject ไม่ต้อง require การซื้อประกันหรือพรบ.
                if (cmbLeadStatus.SelectedItem.Value != SLMConstant.StatusCode.Cancel && cmbLeadStatus.SelectedItem.Value != SLMConstant.StatusCode.Reject)
                {
                    if (biz.CheckRenewOrActPurchased(TicketId.Trim()) == false)
                    {
                        throw new Exception("ไม่สามารถบันทึกได้ เนื่องจากไม่พบข้อมูลการซื้อประกันหรือพรบ.");
                    }
                }

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

                    }
                    if (cmbCardType.SelectedItem.Value != "" && txtCitizenId.Text.Trim() != "")
                    {
                        if (!AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId))
                        {
                            i += 1;
                        }
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
                            {
                                i += 1;
                            }
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
                    }
                }

                if (cmbCardType.SelectedValue != "" && cmbCountry.SelectedValue == "")
                {
                    i += 1;
                    vcmbCountry.Text = "กรุณาเลือกประเทศ";
                }
                else
                    vcmbCountry.Text = "";

                //OwnerBranch, Owner
                string clearOwnerBranchText = "Y";
                if (cmbOwnerBranch.SelectedItem.Value != txtOldOwnerBranch.Text.Trim() || cmbOwner.SelectedItem.Value != txtOldOwner.Text.Trim())
                {
                    if (!AppUtil.ValidateOwner(_currentAssignedFlag, cmbOwnerBranch, vcmbOwnerBranch, cmbOwner, vcmbOwner, txtCampaignId.Text.Trim(), ref clearOwnerBranchText))
                    {
                        i += 1;
                    }

                    //Branch ที่ถูกปิด
                    if (cmbOwnerBranch.Items.Count > 0 && cmbOwnerBranch.SelectedItem.Value != "" && !biz.CheckBranchActive(cmbOwnerBranch.SelectedItem.Value))
                    {
                        vcmbOwnerBranch.Text = "สาขานี้ถูกปิดแล้ว";
                        i += 1;
                    }
                    else
                    {
                        if (clearOwnerBranchText == "Y")
                        {
                            vcmbOwnerBranch.Text = "";
                        }
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
                    {
                        vcmbDelegateBranch.Text = "";
                    }
                }
                else
                {
                    vcmbDelegateBranch.Text = "";
                    vcmbDelegate.Text = "";
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
                {
                    vtxtContactDetail.Text = "";
                }

                if (cmbLeadStatus.SelectedItem.Value == "" && cmbLeadStatus.Text.Trim() == "")
                {
                    vcmbLeadStatus.Text = "กรุณาระบุสถานะของ Lead";
                    i += 1;
                }
                else
                {
                    vcmbLeadStatus.Text = "";
                }

                if (cmbLeadSubStatus.SelectedItem.Value == "" && cmbLeadSubStatus.Text.Trim() == "")
                {
                    vcmbLeadSubStatus.Text = "กรุณาระบุสถานะย่อยของ Lead";
                    i += 1;
                }
                else
                {
                    vcmbLeadStatus.Text = "";
                }

                bool ecmConnection = false;
                if ((fuCreditCard.Visible && fuCreditCard.HasFile) || (fu50tawi.Visible && fu50tawi.HasFile) || (fuDriverlicense.Visible && fuDriverlicense.HasFile))
                {
                    ConnectEcm ecm = new ConnectEcm();
                    ecmConnection = ecm.VerifyConnection();
                }

                if (fuCreditCard.Visible)
                {
                    if (fuCreditCard.HasFile)
                    {
                        if (fuCreditCard.FileBytes.Length > fileSize)
                        {
                            i += 1;
                            vlblCreditCard.Text = "กรุณาแนบไฟล์ฟอร์มตัดบัตรเครดิตขนาดไม่เกิน 3 MB";
                        }
                        else
                        {
                            try
                            {
                                string ext = System.IO.Path.GetExtension(this.fuCreditCard.PostedFile.FileName);
                                string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fuCreditCard.PostedFile.FileName);

                                _creditFormFilename = GenFileName(TicketId.Trim(), fileNameWithoutExt, ext, "1");

                                string filePath = Server.MapPath("Upload") + "\\";
                                string fileLocation = filePath + _creditFormFilename;
                                string paramFileName = fuCreditCard.PostedFile.FileName.Trim();

                                ConnectEcm fn_Upload = new ConnectEcm();
                                fuCreditCard.SaveAs(fileLocation);

                                try
                                {
                                    bool ret = fn_Upload.AddAttachment(fileLocation, _creditFormFilename, TicketId, "fileType", HttpContext.Current.User.Identity.Name);
                                    if (ret)
                                    {
                                        //Success

                                        //Database.SaveCampaignPicture(txtCampaignId.Text, cboFileType.SelectedValue, fname, Session[SessionPrefix+"UserName"].ToString().Trim());
                                        //LoadCampaignPicture();
                                        //LoadCampaignFileType();
                                        //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Complete", "SaveComplete('Upload ไฟล์','เสร็จสมบูรณ์');", true);
                                    }
                                    else
                                    {
                                        //DisplayError("Error", "ไม่สามารถ Upload ไฟล์ได้", "กรุณาลองอีกครั้ง");
                                        //vlblCreditCard.Text = state + " ไม่สามารถ Upload ไฟล์ได้";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _log.Error(ex.Message);
                                    throw ex;
                                }
                                finally
                                {
                                    DeleteLocalFile(fileLocation);    //ลบไฟล์ temp ที่โฟลเดอร์ Upload
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                                //vlblCreditCard.Text = state + " " + ex.Message;
                            }
                        }
                    }
                    else
                    {
                        vlblCreditCard.Text = "";
                    }
                }

                if (fu50tawi.Visible)
                {
                    if (fu50tawi.HasFile)
                    {
                        if (fu50tawi.FileBytes.Length > fileSize)
                        {
                            i += 1;
                            vlbl50tawi.Text = "กรุณาแนบไฟล์ฟอร์มสำเนา 50 ทวิขนาดไม่เกิน 3 MB";
                        }
                        else
                        {
                            try
                            {
                                string ext = System.IO.Path.GetExtension(this.fu50tawi.PostedFile.FileName);
                                string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fu50tawi.PostedFile.FileName);

                                _tawi50FormFilename = GenFileName(TicketId.Trim(), fileNameWithoutExt, ext, "2");
                                string filePath = Server.MapPath("Upload") + "\\";
                                string fileLocation = filePath + _tawi50FormFilename;

                                ConnectEcm fn_Upload = new ConnectEcm();
                                fu50tawi.SaveAs(fileLocation);

                                try
                                {
                                    bool ret = fn_Upload.AddAttachment(fileLocation, _tawi50FormFilename, TicketId, "fileType", HttpContext.Current.User.Identity.Name);
                                    if (ret)
                                    {
                                        //Success

                                        //Database.SaveCampaignPicture(txtCampaignId.Text, cboFileType.SelectedValue, fname, Session[SessionPrefix+"UserName"].ToString().Trim());
                                        //LoadCampaignPicture();
                                        //LoadCampaignFileType();
                                        //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Complete", "SaveComplete('Upload ไฟล์','เสร็จสมบูรณ์');", true);
                                    }
                                    else
                                    {
                                        //vlbl50tawi.Text = state + " ไม่สามารถ Upload ไฟล์ได้";
                                        //DisplayError("Error", "ไม่สามารถ Upload ไฟล์ได้", "กรุณาลองอีกครั้ง");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _log.Error(ex.Message);
                                    throw ex;
                                }
                                finally
                                {
                                    DeleteLocalFile(fileLocation);    //ลบไฟล์ temp ที่โฟลเดอร์ Upload
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                                //vlbl50tawi.Text = state + " " + ex.Message;
                            }
                        }
                    }
                    else
                    {
                        vlbl50tawi.Text = "";
                    }
                }

                if (fuDriverlicense.Visible)
                {
                    if (fuDriverlicense.HasFile)
                    {
                        if (fuDriverlicense.FileBytes.Length > fileSize)
                        {
                            i += 1;
                            vlblDriverlicense.Text = "กรุณาแนบไฟล์สำเนาใบขับขี่ขนาดไม่เกิน 3 MB";
                        }
                        else
                        {
                            try
                            {
                                string ext = System.IO.Path.GetExtension(this.fuDriverlicense.PostedFile.FileName);
                                string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fuDriverlicense.PostedFile.FileName);

                                _driverLicenseFilename = GenFileName(TicketId.Trim(), fileNameWithoutExt, ext, "3");
                                string filePath = Server.MapPath("Upload") + "\\";
                                string fileLocation = filePath + _driverLicenseFilename;

                                ConnectEcm fn_Upload = new ConnectEcm();
                                fuDriverlicense.SaveAs(fileLocation);

                                try
                                {
                                    bool ret = fn_Upload.AddAttachment(fileLocation, _driverLicenseFilename, TicketId, "fileType", HttpContext.Current.User.Identity.Name);
                                    if (ret)
                                    {
                                        //Success

                                        //Database.SaveCampaignPicture(txtCampaignId.Text, cboFileType.SelectedValue, fname, Session[SessionPrefix+"UserName"].ToString().Trim());
                                        //LoadCampaignPicture();
                                        //LoadCampaignFileType();
                                        //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Complete", "SaveComplete('Upload ไฟล์','เสร็จสมบูรณ์');", true);
                                    }
                                    else
                                    {
                                        //vlblDriverlicense.Text = state + " ไม่สามารถ Upload ไฟล์ได้";
                                        //DisplayError("Error", "ไม่สามารถ Upload ไฟล์ได้", "กรุณาลองอีกครั้ง");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _log.Error(ex.Message);
                                    throw ex;
                                }
                                finally
                                {
                                    DeleteLocalFile(fileLocation);    //ลบไฟล์ temp ที่โฟลเดอร์ Upload
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                                //vlblDriverlicense.Text = state + " " + ex.Message;
                            }
                        }
                    }
                    else
                    {
                        vlblDriverlicense.Text = "";
                    }
                }

                //====================== Backup Process การลบ (ปัจจุบันไม่มี process การลบ) =======================================
                if (lblCreditCard.Visible == false && lblCreditCard.Text != "")
                {
                    try
                    {
                        ConnectEcm ecm = new ConnectEcm();
                        if (!ecmConnection)
                        {
                            ecmConnection = ecm.VerifyConnection();
                        }

                        bool flag = ecm.DeleteFile(lblCreditCard.Text.Trim(), TicketId.Trim());
                    }
                    catch
                    {
                        throw;
                    }
                }

                if (lbl50tawi.Visible == false && lbl50tawi.Text != "")
                {
                    try
                    {
                        ConnectEcm ecm = new ConnectEcm();
                        if (!ecmConnection)
                        {
                            ecmConnection = ecm.VerifyConnection();
                        }

                        bool flag = ecm.DeleteFile(lbl50tawi.Text.Trim(), TicketId.Trim());
                    }
                    catch
                    {
                        throw;
                    }
                }

                if (lblDriverlicense.Visible == false && lblDriverlicense.Text != "")
                {
                    try
                    {
                        ConnectEcm ecm = new ConnectEcm();
                        if (!ecmConnection)
                        {
                            ecmConnection = ecm.VerifyConnection();
                        }

                        bool flag = ecm.DeleteFile(lblDriverlicense.Text.Trim(), TicketId.Trim());
                    }
                    catch
                    {
                        throw;
                    }
                }
                //============================================================================================
                if (txtOldStatus.Text != cmbLeadStatus.SelectedValue && (cmbLeadStatus.SelectedValue == "08" || cmbLeadStatus.SelectedValue == "09" || cmbLeadStatus.SelectedValue == "10"))
                {
                    if (!ActivityLeadBiz.checkPaid(TicketId.Trim()))
                    {
                        i += 1;
                        // AppUtil.ClientAlertTabRenew(this, "ไม่สามารถบันทึกข้อมูลได้เนื่องจากเงินที่ต้องชำระกับเงินที่รับชำระไม่เท่ากัน");
                        throw new Exception("ไม่สามารถบันทึกข้อมูลได้เนื่องจากเงินที่ต้องชำระกับเงินที่รับชำระไม่เท่ากัน.");
                    }
                    else if (!ActivityLeadBiz.checkPaidAct(TicketId.Trim()))
                    {
                        i += 1;
                        // AppUtil.ClientAlertTabRenew(this, "ไม่สามารถบันทึกข้อมูลได้เนื่องจากเงินที่ต้องชำระกับเงินที่รับชำระไม่เท่ากัน");
                        throw new Exception("ไม่สามารถบันทึกข้อมูลได้เนื่องจากเงินที่ต้องชำระกับเงินที่รับชำระไม่เท่ากัน.");
                    }
                }


                UpdateAllPanel();

                return i > 0 ? false : true;
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.Message);
                return false;
            }
            finally
            {
                _log.Debug($"Method=ValidateData, TicketId={TicketId.Trim()}, EndLog={DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
            }

        }

        private bool ValidateWarning(RenewInsureBiz biz)
        {
            bool ret = true;
            string err = "";

            int cntAct, cntPol;
            biz.CheckRenewOrActPurchased(TicketId, out cntPol, out cntAct);
            var typ = biz.GetSubStatusType(cmbLeadSubStatus.SelectedValue);

            // 1 ถ้าซื้อประกันด้วย แต่เลือกสถานะ พรบ
            if (cntPol > 0 && typ == 2)
            {
                err = "สถานะที่เลือกไม่ตรงกับข้อมูลการเลือกซื้อ ต้องการบันทึกข้อมูลใช่หรือไม่?";
            }

            // 2 ถ้าซื้อ พรบ อย่างเดียว แต่เลือกสถานะ ประกัน
            if (cntPol == 0 && cntAct > 0 && typ == 1)
            {
                err = "สถานะที่เลือกไม่ตรงกับข้อมูลการเลือกซื้อ ต้องการบันทึกข้อมูลใช่หรือไม่?";
            }

            if (err != "")
            {
                lblWarningLabel.Text = err;
                //updWarning.Update();
                ret = false;
            }
            return ret;
        }

        bool? WarningAccept
        {
            get { return ViewState["WARNING"] as bool?; }
            set { ViewState["WARNING"] = value; }
        }

        protected void imgDriverlicense_OnClick(object sender, EventArgs e)
        {
            lblDriverlicense.Visible = false;
            imgDriverlicense.Visible = false;
            fuDriverlicense.Visible = true;
            clearDriverlicense.Visible = true;      //Added By Pom 24/05/2016
            mpePopup.Show();
        }


        protected void img50tawi_OnClick(object sender, EventArgs e)
        {

            lbl50tawi.Visible = false;
            img50tawi.Visible = false;
            fu50tawi.Visible = true;
            clear50tawi.Visible = true;         //Added By Pom 24/05/2016
            mpePopup.Show();
        }


        protected void imgCreditCard_OnClick(object sender, EventArgs e)
        {

            lblCreditCard.Visible = false;
            imgCreditCard.Visible = false;
            fuCreditCard.Visible = true;
            clearCreditCard.Visible = true;     //Added By Pom 24/05/2016
            mpePopup.Show();

        }

        protected void lblCreditCard_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConnectEcm fn_Upload = new ConnectEcm();
                fn_Upload.DownloadFile(Response, TicketId.Trim() + "/" + lblCreditCard.Text);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        protected void lbl50tawi_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConnectEcm fn_Upload = new ConnectEcm();
                fn_Upload.DownloadFile(Response, TicketId.Trim() + "/" + lbl50tawi.Text);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        protected void lblDriverlicense_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConnectEcm fn_Upload = new ConnectEcm();
                fn_Upload.DownloadFile(Response, TicketId.Trim() + "/" + lblDriverlicense.Text);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        protected void cmbOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ Owner Lead";
                }
                else
                {
                    vcmbOwner.Text = "";
                }
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
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
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void txtAppointment_OnTextChanged(object sender, EventArgs e)
        {
            mpePopup.Show();
        }

        protected void btnWarningOK_Click(object sender, EventArgs e)
        {
            WarningAccept = true;
            doSaveStatus();
        }

        protected void btnWarningCancel_Click(object sender, EventArgs e)
        {
            WarningAccept = false;
            zPopWarning.Hide();
            UpdateAllPanel();
        }
    }
}