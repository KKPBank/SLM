using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using System.Data;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;
using System.Collections;
using SLM.Resource;

namespace SLM.Application.Shared.Obt
{
    public partial class TabInbound : System.Web.UI.UserControl
    {
        private string inboundsearchcondition = "inboundsearchcondition";
        private string _inboundsubstatuslist = "inboundsubstatuslist";
        private string inboundsearchresult = "inboundsearchresult";
        //private string v_inboundsubstatuslist = "v_inboundsubstatuslist";
        private static readonly ILog _log = LogManager.GetLogger(typeof(TabInbound));
        public delegate void ExtendTabEvent();
        public event ExtendTabEvent ExtendTab;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.SetIntTextBox(txtTicketID);
            AppUtil.SetIntTextBox(txtPolicyExpirationYear);
            AppUtil.SetIntTextBox(txtPeriodYear);
            AppUtil.SetMoneyTextBox(txtNotifyGrossPremiumMin, vtxtNotifyGrossPremiumMin);
            AppUtil.SetMoneyTextBox(txtNotifyGrossPremiumMax, vtxtNotifyGrossPremiumMax);
            txtTicketID.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
            txtPolicyExpirationYear.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
            txtPeriodYear.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");

            AppUtil.SetAutoCompleteDropdown(new DropDownList[] { cmbOwnerBranchSearch, cmbOwnerLeadSearch, cmbDelegateBranchSearch, cmbDelegateLeadSearch }, Page, "AUTOCOMPLETESCRIPT");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //เก็บไว้ส่งไปที่ Adam, AolSummaryReport
                    StaffData staff = SlmScr003Biz.GetStaff(HttpContext.Current.User.Identity.Name);
                    txtEmpCode.Text = staff.EmpCode;
                    txtStaffTypeId.Text = staff.StaffTypeId != null ? staff.StaffTypeId.ToString() : "";
                    txtStaffTypeDesc.Text = staff.StaffTypeDesc;
                    txtStaffId.Text = staff.StaffId.ToString();
                    txtStaffBranchCode.Text = staff.BranchCode;


                    //Set AdvanceSearch
                    if (staff.Collapse == null || staff.Collapse == true)
                    {
                        lbAdvanceSearch.Text = "[+] <b>Advance Search</b>";
                        pnAdvanceSearch.Style["display"] = "none";
                        txtAdvanceSearch.Text = "N";
                    }
                    else
                    {
                        lbAdvanceSearch.Text = "[-] <b>Advance Search</b>";
                        pnAdvanceSearch.Style["display"] = "block";
                        txtAdvanceSearch.Text = "Y";
                    }

                    InitialControl();

                    if (Session[inboundsearchcondition] != null)
                    {
                        SetSearchCondition((SearchObtCondition)Session[inboundsearchcondition]);  //Page Load กลับมาจากหน้าอื่น
                        Session.Remove(inboundsearchcondition);
                    }
                    else
                    {
                        //Default Login User - First Load
                        if (cmbOwnerBranchSearch.Items.Count > 0)
                            cmbOwnerBranchSearch.SelectedIndex = cmbOwnerBranchSearch.Items.IndexOf(cmbOwnerBranchSearch.Items.FindByValue(staff.BranchCode));

                        BindOwnerLead();
                        if (cmbOwnerLeadSearch.Items.Count > 0)
                            cmbOwnerLeadSearch.SelectedIndex = cmbOwnerLeadSearch.Items.IndexOf(cmbOwnerLeadSearch.Items.FindByValue(HttpContext.Current.User.Identity.Name.ToLower()));

                        //DoSearchLeadData(0, string.Empty, SortDirection.Ascending);    //First Load ครั้งแรก
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

        private void SetSearchCondition(SearchObtCondition conn)
        {
            bool dosearch = false;
            try
            {
                if (!string.IsNullOrEmpty(conn.ContractNo))
                {
                    txtContractNo.Text = conn.ContractNo;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.TicketId))
                {
                    txtTicketID.Text = conn.TicketId;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.Firstname))
                {
                    txtFirstname.Text = conn.Firstname;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.Lastname))
                {
                    txtLastname.Text = conn.Lastname;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CardType) && cmbCardType.Items.Count > 0)
                {
                    cmbCardType.SelectedIndex = cmbCardType.Items.IndexOf(cmbCardType.Items.FindByValue(conn.CardType));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CitizenId))
                {
                    txtCitizenId.Text = conn.CitizenId;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.ChannelId) && cmbChannel.Items.Count > 0)
                {
                    cmbChannel.SelectedIndex = cmbChannel.Items.IndexOf(cmbChannel.Items.FindByValue(conn.ChannelId));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CampaignId) && cmbCampaign.Items.Count > 0)
                {
                    cmbCampaign.SelectedIndex = cmbCampaign.Items.IndexOf(cmbCampaign.Items.FindByValue(conn.CampaignId));
                    //GetOwnerLead(cmbCampaign.SelectedItem.Value);
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CarLicenseNo))
                {
                    txtLicenseNo.Text = conn.CarLicenseNo;
                    dosearch = true;
                }
                if (conn.Grade != null && cmbGradeSearch.Items.Count > 0)
                {
                    cmbGradeSearch.SelectedIndex = cmbGradeSearch.Items.IndexOf(cmbGradeSearch.Items.FindByText(conn.Grade.Length == 0 ? "ไม่มี Grade" : string.Join("+", conn.Grade)));
                    dosearch = true;
                }
                if (conn.NextContactDateFrom.Year != 1)
                {
                    tdmNextAppointmentFrom.DateValue = conn.NextContactDateFrom;
                    dosearch = true;
                }
                if (conn.NextContactDateTo.Year != 1)
                {
                    tdmNextAppointmentTo.DateValue = conn.NextContactDateTo;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.HasNotifyPremium) && cmbHasNotifyPremium.Items.Count > 0)
                {
                    cmbHasNotifyPremium.SelectedIndex = cmbHasNotifyPremium.Items.IndexOf(cmbHasNotifyPremium.Items.FindByValue(conn.HasNotifyPremium));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.NotifyGrossPremiumMin))
                {
                    txtNotifyGrossPremiumMin.Text = conn.NotifyGrossPremiumMin;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.NotifyGrossPremiumMax))
                {
                    txtNotifyGrossPremiumMax.Text = conn.NotifyGrossPremiumMax;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.PolicyExpirationYear))
                {
                    txtPolicyExpirationYear.Text = conn.PolicyExpirationYear;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.PolicyExpirationMonth) && cmbPolicyExpirationMonth.Items.Count > 0)
                {
                    cmbPolicyExpirationMonth.SelectedIndex = cmbPolicyExpirationMonth.Items.IndexOf(cmbPolicyExpirationMonth.Items.FindByValue(conn.PolicyExpirationMonth));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.PeriodYear))
                {
                    txtPeriodYear.Text = conn.PeriodYear;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.PeriodMonth) && cmbPeriodMonth.Items.Count > 0)
                {
                    cmbPeriodMonth.SelectedIndex = cmbPeriodMonth.Items.IndexOf(cmbPeriodMonth.Items.FindByValue(conn.PeriodMonth));
                    dosearch = true;
                }
                if (conn.CheckFollowUp)
                {
                    cbFollowUp.Checked = conn.CheckFollowUp;
                    dosearch = true;
                }
                if (conn.CheckInbound)
                {
                    cbInbound.Checked = conn.CheckInbound;
                    dosearch = true;
                }
                if (conn.CheckOutbound)
                {
                    cbOutbound.Checked = conn.CheckOutbound;
                    dosearch = true;
                }

                if (!string.IsNullOrEmpty(conn.ContractNoRefer))
                {
                    txtContractNoRefer.Text = conn.ContractNoRefer;
                    dosearch = true;
                }
                if (conn.CreatedDate.Year != 1)
                {
                    tdmCreateDate.DateValue = conn.CreatedDate;
                    dosearch = true;
                }
                if (conn.AssignedDate.Year != 1)
                {
                    tdmAssignDate.DateValue = conn.AssignedDate;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.OwnerBranch) && cmbOwnerBranchSearch.Items.Count > 0)
                {
                    cmbOwnerBranchSearch.SelectedIndex = cmbOwnerBranchSearch.Items.IndexOf(cmbOwnerBranchSearch.Items.FindByValue(conn.OwnerBranch));
                    BindOwnerLead();
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.OwnerUsername) && cmbOwnerLeadSearch.Items.Count > 0)
                {
                    cmbOwnerLeadSearch.SelectedIndex = cmbOwnerLeadSearch.Items.IndexOf(cmbOwnerLeadSearch.Items.FindByValue(conn.OwnerUsername));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.DelegateBranch) && cmbDelegateBranchSearch.Items.Count > 0)
                {
                    cmbDelegateBranchSearch.SelectedIndex = cmbDelegateBranchSearch.Items.IndexOf(cmbDelegateBranchSearch.Items.FindByValue(conn.DelegateBranch));
                    BindDelegateLead();
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.DelegateLead) && cmbDelegateLeadSearch.Items.Count > 0)
                {
                    cmbDelegateLeadSearch.SelectedIndex = cmbDelegateLeadSearch.Items.IndexOf(cmbDelegateLeadSearch.Items.FindByValue(conn.DelegateLead));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CreateByBranch) && cmbCreatebyBranchSearch.Items.Count > 0)
                {
                    cmbCreatebyBranchSearch.SelectedIndex = cmbCreatebyBranchSearch.Items.IndexOf(cmbCreatebyBranchSearch.Items.FindByValue(conn.CreateByBranch));
                    BindCreateByLead();
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CreateBy) && cmbCreatebySearch.Items.Count > 0)
                {
                    cmbCreatebySearch.SelectedIndex = cmbCreatebySearch.Items.IndexOf(cmbCreatebySearch.Items.FindByValue(conn.CreateBy));
                    dosearch = true;
                }

                if (conn.CheckWaitCreditForm)
                {
                    cbWaitCreditForm.Checked = true;
                    dosearch = true;
                }
                if (conn.CheckWait50Tawi)
                {
                    cbWait50Tawi.Checked = true;
                    dosearch = true;
                }
                if (conn.CheckWaitDriverLicense)
                {
                    cbWaitDriverLicense.Checked = true;
                    dosearch = true;
                }
                if (conn.CheckPolicyNo)
                {
                    cbPolicyNo.Checked = true;
                    dosearch = true;
                }
                if (conn.CheckAct)
                {
                    cbAct.Checked = true;
                    dosearch = true;
                }
                if (conn.CheckStopClaim)
                {
                    cbStopClaim.Checked = true;
                    dosearch = true;
                }
                if (conn.CheckStopClaim_Cancel)
                {
                    cbStopClaim_Cancel.Checked = true;
                    dosearch = true;
                }
                if (conn.CheckWaitCreditForm && conn.CheckWait50Tawi && conn.CheckWaitDriverLicense && conn.CheckPolicyNo && conn.CheckStopClaim && conn.CheckStopClaim_Cancel)
                {
                    cbWorkFlagAll.Checked = true;
                    dosearch = true;
                }

                //Fill in status
                if (!string.IsNullOrEmpty(conn.StatusList))
                {
                    string[] vals = conn.StatusList.Split(',');
                    int count = 0;
                    foreach (string val in vals)
                    {
                        foreach (RepeaterItem item in rptStatus.Items)
                        {
                            string statuCode = val.Replace("'", "");
                            if (((HiddenField)item.FindControl("hiddenStatusCode")).Value == statuCode)
                            {
                                ((CheckBox)item.FindControl("cbStatus")).Checked = true;
                                dosearch = true;
                                count += 1;
                                break;
                            }
                        }
                    }

                    if (rptStatus.Items.Count == count)
                    {
                        CheckBox cbStatusAll = (CheckBox)rptStatus.Controls[0].Controls[0].FindControl("cbStatusAll");
                        if (cbStatusAll != null)
                            cbStatusAll.Checked = true;
                    }
                }

                //Fill in substatus
                Session.Remove(_inboundsubstatuslist);
                if (!string.IsNullOrEmpty(conn.SubStatusList) && !string.IsNullOrEmpty(conn.StatusList) && cmbCampaign.SelectedItem.Value != "")
                {
                    Session[_inboundsubstatuslist] = new ConfigProductSubStatusBiz().GetSubStatusList("", cmbCampaign.SelectedItem.Value);
                    string[] vals = conn.SubStatusList.Split(',');

                    foreach (RepeaterItem item in rptStatus.Items)
                    {
                        if (((CheckBox)item.FindControl("cbStatus")).Checked)       //ถ้ามีการเลือกสถานะหลัก
                        {
                            BindSubStatus(item, (List<ConfigProductSubStatusData>)Session[_inboundsubstatuslist]); //Bind สถานะย่อย
                            CheckBoxList cblSubStatus = (CheckBoxList)item.FindControl("cblSubStatus");

                            //นำสถานะย่อย มาเช็กลงบน checkbox ของสถานะย่อย
                            foreach (string val in vals)
                            {
                                ListItem li = cblSubStatus.Items.FindByValue(val.Replace("'", ""));
                                if (li != null)
                                {
                                    li.Selected = true;
                                    dosearch = true;
                                }
                            }
                        }
                    }
                }

                if (conn.AdvancedSearch)
                {
                    lbAdvanceSearch.Text = "[-] <b>Advance Search</b>";
                    pnAdvanceSearch.Style["display"] = "block";
                    txtAdvanceSearch.Text = "Y";
                }
                else
                {
                    lbAdvanceSearch.Text = "[+] <b>Advance Search</b>";
                    pnAdvanceSearch.Style["display"] = "none";
                    txtAdvanceSearch.Text = "N";
                }

                SortExpressionProperty = conn.SortExpression;
                SortDirectionProperty = conn.SortDirection == SortDirection.Ascending.ToString() ? SortDirection.Ascending : SortDirection.Descending;

                Session.Remove(inboundsearchresult);
                if (dosearch)
                {
                    DoSearchLeadData(conn.PageIndex, SortExpressionProperty, SortDirectionProperty);
                    if (ExtendTab != null) ExtendTab();
                }
            }
            catch
            {
                throw;
            }
        }

        #region Sorting

        protected void gvResult_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                if (SortExpressionProperty != e.SortExpression)         //เมื่อเปลี่ยนคอลัมน์ในการ sort
                    SortDirectionProperty = SortDirection.Ascending;
                else
                {
                    if (SortDirectionProperty == SortDirection.Ascending)
                        SortDirectionProperty = SortDirection.Descending;
                    else
                        SortDirectionProperty = SortDirection.Ascending;
                }

                SortExpressionProperty = e.SortExpression;
                DoSearchLeadData(0, SortExpressionProperty, SortDirectionProperty);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
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

        #endregion

        public void CallSearchMethod()
        {
            try
            {
                ExecuteSearchMethod();
            }
            catch
            {
                throw;
            }
        }

        private void InitialControl()
        {
            //ประเภทบุคคล
            cmbCardType.DataSource = CardTypeBiz.GetCardTypeList();
            cmbCardType.DataTextField = "TextField";
            cmbCardType.DataValueField = "ValueField";
            cmbCardType.DataBind();
            cmbCardType.Items.Insert(0, new ListItem("", ""));

            cmbCampaign.DataSource = SlmScr003Biz.GetSaleAndBothCampaignData();     //cmbCampaign.DataSource = SlmScr003Biz.GetAllActiveCampaignData();
            cmbCampaign.DataTextField = "TextField";
            cmbCampaign.DataValueField = "ValueField";
            cmbCampaign.DataBind();
            cmbCampaign.Items.Insert(0, new ListItem("", ""));

            //Grade ลูกค้า
            cmbGradeSearch.DataSource = new CustomerGradeBiz().GetCustomerGradeList();
            cmbGradeSearch.DataTextField = "TextField";
            cmbGradeSearch.DataValueField = "ValueField";
            cmbGradeSearch.DataBind();
            cmbGradeSearch.Items.Insert(0, new ListItem("", ""));
            cmbGradeSearch.Items.Insert(1, new ListItem("ไม่มี Grade", "-1"));

            // ค่าเบี้ยปีต่อ
            cmbHasNotifyPremium.Items.Insert(0, new ListItem("", ""));
            cmbHasNotifyPremium.Items.Insert(1, new ListItem("มีค่าเบี้ย", "Y"));
            cmbHasNotifyPremium.Items.Insert(2, new ListItem("ไม่มีค่าเบี้ย", "N"));

            cmbPolicyExpirationMonth.DataSource = new MonthBiz().GetMonthList();
            cmbPolicyExpirationMonth.DataTextField = "TextField";
            cmbPolicyExpirationMonth.DataValueField = "ValueField";
            cmbPolicyExpirationMonth.DataBind();
            cmbPolicyExpirationMonth.Items.Insert(0, new ListItem("", ""));

            cmbPeriodMonth.DataSource = new MonthBiz().GetMonthList();
            cmbPeriodMonth.DataTextField = "TextField";
            cmbPeriodMonth.DataValueField = "ValueField";
            cmbPeriodMonth.DataBind();
            cmbPeriodMonth.Items.Insert(0, new ListItem("", ""));

            //GetOwnerLead(cmbCampaign.SelectedItem.Value);

            cmbChannel.DataSource = SlmScr003Biz.GetChannelData();
            cmbChannel.DataTextField = "TextField";
            cmbChannel.DataValueField = "ValueField";
            cmbChannel.DataBind();
            cmbChannel.Items.Insert(0, new ListItem("", ""));

            //Owner Lead
            //cmbOwnerLeadSearch.DataSource = ObtScr003Biz.GetOwnerList(HttpContext.Current.User.Identity.Name);
            //cmbOwnerLeadSearch.DataTextField = "TextField";
            //cmbOwnerLeadSearch.DataValueField = "ValueField";
            //cmbOwnerLeadSearch.DataBind();
            //cmbOwnerLeadSearch.Items.Insert(0, new ListItem("", ""));

            var branchList = BranchBiz.GetBranchList(SLMConstant.Branch.All);
            //Owner Branch
            cmbOwnerBranchSearch.DataSource = branchList;
            cmbOwnerBranchSearch.DataTextField = "TextField";
            cmbOwnerBranchSearch.DataValueField = "ValueField";
            cmbOwnerBranchSearch.DataBind();
            cmbOwnerBranchSearch.Items.Insert(0, new ListItem("", ""));
            cmbOwnerLeadSearch.Items.Insert(0, new ListItem("", ""));
            //BindOwnerLead();

            //Delegate Branch
            cmbDelegateBranchSearch.DataSource = branchList;
            cmbDelegateBranchSearch.DataTextField = "TextField";
            cmbDelegateBranchSearch.DataValueField = "ValueField";
            cmbDelegateBranchSearch.DataBind();
            cmbDelegateBranchSearch.Items.Insert(0, new ListItem("", ""));
            cmbDelegateLeadSearch.Items.Insert(0, new ListItem("", ""));
            //BindDelegateLead();

            //CreateBy Branch
            cmbCreatebyBranchSearch.DataSource = branchList;
            cmbCreatebyBranchSearch.DataTextField = "TextField";
            cmbCreatebyBranchSearch.DataValueField = "ValueField";
            cmbCreatebyBranchSearch.DataBind();
            cmbCreatebyBranchSearch.Items.Insert(0, new ListItem("", ""));
            cmbCreatebySearch.Items.Insert(0, new ListItem("", ""));
            //BindCreateByLead();

            //Status
            BindStatus();

            pcTop.SetVisible = false;
        }

        private void BindStatus()
        {
            var list = SlmScr003Biz.GetOptionList(AppConstant.OptionType.LeadStatus);
            //string[] value = new string[] { "08", "09", "10" };
            //list = list.Where(p => value.Contains(p.ValueField) == false).ToList();
            rptStatus.DataSource = list;
            rptStatus.DataBind();
        }

        private void BindSubStatus(RepeaterItem item, List<ConfigProductSubStatusData> subStatusList)
        {
            try
            {
                string[] value = new string[] { "08", "09" };   //ไม่เอาสถานะ ต่อประกัน, ซื้อ พรบ.เดี่ยว ของสถานะ Outbound เพราะจะไปใช้ในส่วนของสถานะย่อย ต่อประกัน, ซื้อ พรบ.เดี่ยว ของสถานะ สนใจ
                string statusCode = ((HiddenField)item.FindControl("hiddenStatusCode")).Value;
                subStatusList = subStatusList.Where(p => p.StatusCode == statusCode && value.Contains(p.SubStatusCode) == false).ToList();
                //subStatusList = subStatusList.Where(p => p.StatusCode == statusCode).ToList();

                ((CheckBoxList)item.FindControl("cblSubStatus")).DataSource = subStatusList;
                ((CheckBoxList)item.FindControl("cblSubStatus")).DataTextField = "SubStatusName";
                ((CheckBoxList)item.FindControl("cblSubStatus")).DataValueField = "SubStatusCode";
                ((CheckBoxList)item.FindControl("cblSubStatus")).DataBind();
            }
            catch
            {
                throw;
            }
        }

        protected void cbStatusAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox cbStatusAll = (CheckBox)sender;
                foreach (RepeaterItem item in rptStatus.Items)
                {
                    ((CheckBox)item.FindControl("cbStatus")).Checked = cbStatusAll.Checked;
                    if (((CheckBox)item.FindControl("cbStatus")).Checked)
                    {
                        if (cmbCampaign.SelectedItem.Value != "")
                        {
                            if (Session[_inboundsubstatuslist] == null)
                                Session[_inboundsubstatuslist] = new ConfigProductSubStatusBiz().GetSubStatusList("", cmbCampaign.SelectedItem.Value);

                            BindSubStatus(item, (List<ConfigProductSubStatusData>)Session[_inboundsubstatuslist]);
                        }
                    }
                    else
                    {
                        ((CheckBoxList)item.FindControl("cblSubStatus")).Items.Clear();
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

        protected void cbStatus_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                int count = 0;
                var rptItem = ((CheckBox)sender).Parent as RepeaterItem;

                if (((CheckBox)sender).Checked)
                {
                    if (cmbCampaign.SelectedItem.Value != "")
                    {
                        if (Session[_inboundsubstatuslist] == null)
                            Session[_inboundsubstatuslist] = new ConfigProductSubStatusBiz().GetSubStatusList("", cmbCampaign.SelectedItem.Value);

                        BindSubStatus(rptItem, (List<ConfigProductSubStatusData>)Session[_inboundsubstatuslist]);
                    }
                }
                else
                    ((CheckBoxList)rptItem.FindControl("cblSubStatus")).Items.Clear();

                foreach (RepeaterItem item in rptStatus.Items)
                {
                    if (((CheckBox)item.FindControl("cbStatus")).Checked == false)
                        count += 1;
                }

                CheckBox cbStatusAll = (CheckBox)rptStatus.Controls[0].Controls[0].FindControl("cbStatusAll");
                if (cbStatusAll != null)
                    cbStatusAll.Checked = count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbCampaign_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCampaign.SelectedItem.Value != "")
                {
                    Session[_inboundsubstatuslist] = new ConfigProductSubStatusBiz().GetSubStatusList("", cmbCampaign.SelectedItem.Value);

                    foreach (RepeaterItem item in rptStatus.Items)
                    {
                        if (((CheckBox)item.FindControl("cbStatus")).Checked)
                            BindSubStatus(item, (List<ConfigProductSubStatusData>)Session[_inboundsubstatuslist]);

                    }
                }
                else
                {
                    foreach (RepeaterItem item in rptStatus.Items)
                    {
                        if (((CheckBox)item.FindControl("cbStatus")).Checked)
                            ((CheckBoxList)item.FindControl("cblSubStatus")).Items.Clear();
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

        protected void cmbOwnerBranchSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindOwnerLead();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void BindOwnerLead()
        {
            var list = StaffBiz.GetStaffList(cmbOwnerBranchSearch.SelectedItem.Value);
            list.ForEach(p => p.ValueField = p.ValueField.ToLower());
            cmbOwnerLeadSearch.DataSource = list;                   //ObtScr003Biz.GetStaffAllData(cmbOwnerBranchSearch.SelectedItem.Value);
            cmbOwnerLeadSearch.DataTextField = "TextField";
            cmbOwnerLeadSearch.DataValueField = "ValueField";
            cmbOwnerLeadSearch.DataBind();
            cmbOwnerLeadSearch.Items.Insert(0, new ListItem("", ""));
        }

        protected void cmbDelegateBranchSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindDelegateLead();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void BindDelegateLead()
        {
            cmbDelegateLeadSearch.DataSource = StaffBiz.GetStaffList(cmbDelegateBranchSearch.SelectedItem.Value);     //ObtScr003Biz.GetStaffAllData(cmbDelegateBranchSearch.SelectedItem.Value);
            cmbDelegateLeadSearch.DataTextField = "TextField";
            cmbDelegateLeadSearch.DataValueField = "ValueField";
            cmbDelegateLeadSearch.DataBind();
            cmbDelegateLeadSearch.Items.Insert(0, new ListItem("", ""));
        }

        protected void cmbCreatebyBranchSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindCreateByLead();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void BindCreateByLead()
        {
            cmbCreatebySearch.DataSource = StaffBiz.GetStaffList(cmbCreatebyBranchSearch.SelectedItem.Value);   //ObtScr003Biz.GetStaffAllData(cmbCreatebyBranchSearch.SelectedItem.Value);
            cmbCreatebySearch.DataTextField = "TextField";
            cmbCreatebySearch.DataValueField = "ValueField";
            cmbCreatebySearch.DataBind();
            cmbCreatebySearch.Items.Insert(0, new ListItem("", ""));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Remove(inboundsearchresult);   //clear session
                ExecuteSearchMethod();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidateData()
        {
            bool statusSelected = false;
            foreach (RepeaterItem item in rptStatus.Items)
            {
                if (((CheckBox)item.FindControl("cbStatus")).Checked)
                {
                    statusSelected = true;
                    break;
                }
            }

            if (txtTicketID.Text.Trim() == string.Empty && txtFirstname.Text.Trim() == string.Empty && txtLastname.Text.Trim() == string.Empty
                && txtCitizenId.Text.Trim() == string.Empty && cmbCardType.SelectedIndex == 0 && txtContractNoRefer.Text.Trim() == string.Empty
                && cmbCampaign.SelectedIndex == 0 && cmbChannel.SelectedIndex == 0
                && cmbOwnerBranchSearch.SelectedIndex == 0 && cmbOwnerLeadSearch.SelectedIndex == 0
                && cmbDelegateBranchSearch.SelectedIndex == 0 && cmbDelegateLeadSearch.SelectedIndex == 0
                && cmbCreatebyBranchSearch.SelectedIndex == 0 && cmbCreatebySearch.SelectedIndex == 0
                && tdmCreateDate.DateValue.Year == 1 && tdmAssignDate.DateValue.Year == 1 && txtContractNo.Text.Trim() == string.Empty
                && cmbPolicyExpirationMonth.SelectedIndex == 0 && txtPolicyExpirationYear.Text.Trim() == string.Empty
                && cmbPeriodMonth.SelectedIndex == 0 && txtPeriodYear.Text.Trim() == string.Empty
                && txtLicenseNo.Text.Trim() == string.Empty && statusSelected == false
                && cbFollowUp.Checked == false && cbInbound.Checked == false && cbOutbound.Checked == false
                && cbWaitCreditForm.Checked == false && cbWait50Tawi.Checked == false && cbWaitDriverLicense.Checked == false && cbPolicyNo.Checked == false && cbAct.Checked == false

                && cbStopClaim.Checked == false && cbStopClaim_Cancel.Checked == false
                && tdmNextAppointmentFrom.DateValue.Year == 1 && tdmNextAppointmentTo.DateValue.Year == 1 && cmbGradeSearch.SelectedIndex == 0
                && cmbHasNotifyPremium.SelectedIndex == 0 && txtNotifyGrossPremiumMin.Text.Trim() == string.Empty && txtNotifyGrossPremiumMax.Text.Trim() == string.Empty)
            {
                return false;
            }
            else
                return true;
        }

        private void ExecuteSearchMethod()
        {
            try
            {
                Session.Remove(inboundsearchresult);   //clear session

                if (
                    tdmNextAppointmentFrom.DateValue.Year == 1 && tdmNextAppointmentTo.DateValue.Year != 1 ||
                    tdmNextAppointmentFrom.DateValue.Year != 1 && tdmNextAppointmentTo.DateValue.Year == 1
                )
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุวันที่นัดโทรหาลูกค้าเริ่มต้นและสิ้นสุดให้ครบถ้วน");
                    return;
                }
                if (tdmNextAppointmentFrom.DateValue > tdmNextAppointmentTo.DateValue)
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุวันที่นัดโทรหาลูกค้าเริ่มต้นต้องน้อยกว่าวันที่นัดโทรหาลูกค้าสิ้นสุด");
                    return;
                }
                if (
                    !string.IsNullOrEmpty(txtNotifyGrossPremiumMin.Text) &&
                    !string.IsNullOrEmpty(txtNotifyGrossPremiumMax.Text) &&
                    decimal.Parse(txtNotifyGrossPremiumMin.Text.Trim().Replace(",", "")) > decimal.Parse(txtNotifyGrossPremiumMax.Text.Trim().Replace(",", ""))
                )
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุค่าเบี้ยปีต่อเริ่มต้นต้องน้อยกว่าค่าเบี้ยปีต่อสิ้นสุด");
                    return;
                }
                if (ValidateData())
                {
                    SortExpressionProperty = string.Empty;
                    SortDirectionProperty = SortDirection.Ascending;
                    DoSearchLeadData(0, SortExpressionProperty, SortDirectionProperty);

                    if (ExtendTab != null) ExtendTab();
                }
                else
                    AppUtil.ClientAlert(Page, "กรุณาเลือกเงื่อนไขอย่างน้อย 1 อย่าง");
            }
            catch
            {
                throw;
            }
        }

        private void DoSearchLeadData(int pageIndex, string sortExpression, SortDirection sortDirection)
        {
            try
            {
                List<SearchObtResult> result = null;

                //if (Session[inboundsearchresult] == null)
                //{
                //    result = new SlmScr029Biz().SearchTabAllTask(GetSearchCondition());
                //    Session[inboundsearchresult] = result;
                //}
                //else
                //    result = (List<SearchObtResult>)Session[inboundsearchresult];

                string logError = "";
                result = new SlmScr029Biz().SearchTabAllTask(GetSearchCondition(), out logError);
                if (!string.IsNullOrEmpty(logError))
                {
                    _log.Error(logError);
                }

                switch (sortExpression)
                {
                    case "CampaignName":
                        if (sortDirection == SortDirection.Ascending)
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderBy(p => p.CampaignName).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderBy(p => p.CampaignName).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderBy(p => p.CampaignName).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        else
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderByDescending(p => p.CampaignName).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderByDescending(p => p.CampaignName).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderByDescending(p => p.CampaignName).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        break;
                    case "StatusDesc":
                        if (sortDirection == SortDirection.Ascending)
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderBy(p => p.StatusDesc).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderBy(p => p.StatusDesc).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderBy(p => p.StatusDesc).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        else
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderByDescending(p => p.StatusDesc).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderByDescending(p => p.StatusDesc).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderByDescending(p => p.StatusDesc).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        break;
                    case "NextContactDate":
                        if (sortDirection == SortDirection.Ascending)
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderBy(p => p.NextContactDate).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderBy(p => p.NextContactDate).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderBy(p => p.NextContactDate).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        else
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderByDescending(p => p.NextContactDate).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderByDescending(p => p.NextContactDate).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderByDescending(p => p.NextContactDate).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        break;
                    case "AssignedDate":
                        if (sortDirection == SortDirection.Ascending)
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderBy(p => p.AssignedDate).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderBy(p => p.AssignedDate).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderBy(p => p.AssignedDate).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        else
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderByDescending(p => p.AssignedDate).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderByDescending(p => p.AssignedDate).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderByDescending(p => p.AssignedDate).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        break;
                    case "DelegateDate":
                        if (sortDirection == SortDirection.Ascending)
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderBy(p => p.DelegateDate).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderBy(p => p.DelegateDate).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderBy(p => p.DelegateDate).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        else
                        {
                            var followup = result.Where(p => p.ResultFlag == "F").OrderByDescending(p => p.DelegateDate).ToList();
                            var inbound = result.Where(p => p.ResultFlag == "I").OrderByDescending(p => p.DelegateDate).ToList();
                            var outbound = result.Where(p => p.ResultFlag == "O").OrderByDescending(p => p.DelegateDate).ToList();
                            result = followup.Union(inbound).Union(outbound).ToList();
                        }
                        break;
                }

                Session[SLMConstant.InboundLeadList] = AppUtil.GetNextLeadList(result, pageIndex);
                BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), pageIndex);
                upResult.Update();
            }
            catch
            {
                throw;
            }
        }

        private SearchObtCondition GetSearchCondition()
        {
            SearchObtCondition data = new SearchObtCondition();
            data.ContractNo = txtContractNo.Text.Trim();
            data.TicketId = txtTicketID.Text.Trim();
            data.Firstname = txtFirstname.Text.Trim();
            data.Lastname = txtLastname.Text.Trim();
            data.CardType = cmbCardType.Items.Count > 0 ? cmbCardType.SelectedItem.Value : string.Empty;    //ประเภทบุคคล
            data.CitizenId = txtCitizenId.Text.Trim();
            data.ChannelId = cmbChannel.Items.Count > 0 ? cmbChannel.SelectedItem.Value : string.Empty;
            data.CampaignId = cmbCampaign.Items.Count > 0 ? cmbCampaign.SelectedItem.Value : string.Empty;
            data.CarLicenseNo = txtLicenseNo.Text.Trim();

            data.Grade = cmbGradeSearch.SelectedIndex == 0 ? null : (cmbGradeSearch.SelectedIndex == 1 ? new string[] { } : cmbGradeSearch.Items[cmbGradeSearch.SelectedIndex].Text.Split('+'));
            data.NextContactDateFrom = tdmNextAppointmentFrom.DateValue;
            data.NextContactDateTo = tdmNextAppointmentTo.DateValue;
            data.HasNotifyPremium = cmbHasNotifyPremium.Items.Count > 0 ? cmbHasNotifyPremium.SelectedItem.Value : string.Empty;
            data.NotifyGrossPremiumMin = txtNotifyGrossPremiumMin.Text.Trim().Replace(",", "");
            data.NotifyGrossPremiumMax = txtNotifyGrossPremiumMax.Text.Trim().Replace(",", "");
            data.PolicyExpirationYear = txtPolicyExpirationYear.Text.Trim();
            data.PolicyExpirationMonth = cmbPolicyExpirationMonth.Items.Count > 0 ? cmbPolicyExpirationMonth.SelectedItem.Value : string.Empty;
            data.PeriodYear = txtPeriodYear.Text.Trim();
            data.PeriodMonth = cmbPeriodMonth.Items.Count > 0 ? cmbPeriodMonth.SelectedItem.Value : string.Empty;
            data.ContractNoRefer = txtContractNoRefer.Text.Trim();
            data.CreatedDate = tdmCreateDate.DateValue;
            data.AssignedDate = tdmAssignDate.DateValue;
            data.OwnerBranch = cmbOwnerBranchSearch.Items.Count > 0 ? cmbOwnerBranchSearch.SelectedItem.Value : string.Empty;           //Owner Branch
            data.OwnerUsername = cmbOwnerLeadSearch.Items.Count > 0 ? cmbOwnerLeadSearch.SelectedItem.Value : string.Empty;             //Owner Lead
            data.DelegateBranch = cmbDelegateBranchSearch.Items.Count > 0 ? cmbDelegateBranchSearch.SelectedItem.Value : string.Empty;  //Delegate Branch
            data.DelegateLead = cmbDelegateLeadSearch.Items.Count > 0 ? cmbDelegateLeadSearch.SelectedItem.Value : string.Empty;        //Delegate Lead
            data.CreateByBranch = cmbCreatebyBranchSearch.Items.Count > 0 ? cmbCreatebyBranchSearch.SelectedItem.Value : string.Empty;  //CreateBy Branch
            data.CreateBy = cmbCreatebySearch.Items.Count > 0 ? cmbCreatebySearch.SelectedItem.Value : string.Empty;                    //CreateBy

            data.CheckWaitCreditForm = cbWaitCreditForm.Checked;
            data.CheckWait50Tawi = cbWait50Tawi.Checked;
            data.CheckWaitDriverLicense = cbWaitDriverLicense.Checked;
            data.CheckPolicyNo = cbPolicyNo.Checked;
            data.CheckAct = cbAct.Checked;
            data.CheckStopClaim = cbStopClaim.Checked;
            data.CheckStopClaim_Cancel = cbStopClaim_Cancel.Checked;

            var list = GetStatusList();
            data.StatusList = list[0];
            data.SubStatusList = list[1];
            data.StatusNameList = list[2];
            data.SubStatusNameList = list[3];

            data.PageIndex = pcTop.SelectedPageIndex > -1 ? pcTop.SelectedPageIndex : 0;
            data.SortExpression = SortExpressionProperty;
            data.SortDirection = SortDirectionProperty.ToString();
            data.AdvancedSearch = txtAdvanceSearch.Text.Trim() == "Y" ? true : false;

            //ข้อมูลคน Login
            data.StaffTypeId = txtStaffTypeId.Text.Trim();
            data.StaffBranchCode = txtStaffBranchCode.Text.Trim();
            data.StaffEmpCode = txtEmpCode.Text.Trim();
            data.StaffUsername = HttpContext.Current.User.Identity.Name;

            //กรณีไม่ติ๊ก checkbox followup inbound outbound
            if (cbFollowUp.Checked == false && cbInbound.Checked == false && cbOutbound.Checked == false)
            {
                //ถ้าไม่การติ๊ก checkbox followup inbound outbound ให้เสมือนว่าเป็นการเลือกทั้งสามแบบ เพราะจะมีการค้นหาจาก condition อื่นๆแทน แต่ยังต้องแยกออกมาเป็น 3 สีของประเภทของมูล
                data.IsFollowup = true;
                data.IsInbound = true;
                data.IsOutbound = true;
            }
            else
            {
                data.IsFollowup = cbFollowUp.Checked;
                data.IsInbound = cbInbound.Checked;
                data.IsOutbound = cbOutbound.Checked;
            }

            //ใช้เก็บค่าใน session กรณีกดไปหน้า view หรือ edit เมื่อ back กลับมาให้ fill ลงหน้าจอ
            data.CheckFollowUp = cbFollowUp.Checked;
            data.CheckInbound = cbInbound.Checked;
            data.CheckOutbound = cbOutbound.Checked;

            //Search Logging
            data.ScreenCode = "SLM_SCR_029";
            data.CardTypeDesc = cmbCardType.Items.Count > 0 ? cmbCardType.SelectedItem.Text : string.Empty;
            data.CampaignName = cmbCampaign.Items.Count > 0 ? cmbCampaign.SelectedItem.Text : string.Empty;
            data.PolicyExpirationMonthName = cmbPolicyExpirationMonth.Items.Count > 0 ? cmbPolicyExpirationMonth.SelectedItem.Text : string.Empty;
            data.PeriodMonthName = cmbPeriodMonth.Items.Count > 0 ? cmbPeriodMonth.SelectedItem.Text : string.Empty;
            data.CreatedByBranchName = cmbCreatebyBranchSearch.Items.Count > 0 ? cmbCreatebyBranchSearch.SelectedItem.Text : string.Empty;
            data.CreatedByName = cmbCreatebySearch.Items.Count > 0 ? cmbCreatebySearch.SelectedItem.Text : string.Empty;
            data.CheckWaitCreditFormDesc = cbWaitCreditForm.Text.Trim();
            data.CheckWait50TawiDesc = cbWait50Tawi.Text.Trim();
            data.CheckWaitDriverLicenseDesc = cbWaitDriverLicense.Text.Trim();
            data.CheckPolicyNoDesc = cbPolicyNo.Text.Trim();
            data.CheckActDesc = cbAct.Text.Trim();
            data.CheckStopClaimDesc = cbStopClaim.Text.Trim();
            data.CheckStopClaim_CancelDesc = cbStopClaim_Cancel.Text.Trim();
            data.OwnerBranchName = cmbOwnerBranchSearch.Items.Count > 0 ? cmbOwnerBranchSearch.SelectedItem.Text : string.Empty;
            data.OwnerName = cmbOwnerLeadSearch.Items.Count > 0 ? cmbOwnerLeadSearch.SelectedItem.Text : string.Empty;
            data.DelegateBranchName = cmbDelegateBranchSearch.Items.Count > 0 ? cmbDelegateBranchSearch.SelectedItem.Text : string.Empty;
            data.DelegateName = cmbDelegateLeadSearch.Items.Count > 0 ? cmbDelegateLeadSearch.SelectedItem.Text : string.Empty;

            return data;
        }

        private List<string> GetStatusList()
        {
            string statusCodelist = "";
            string subStatusCodeList = "";
            string statusNameList = "";
            string subStatusNameList = "";
            foreach (RepeaterItem item in rptStatus.Items)
            {
                if (((CheckBox)item.FindControl("cbStatus")).Checked)
                {
                    statusCodelist += (statusCodelist == "" ? "" : ",") + "'" + ((HiddenField)item.FindControl("hiddenStatusCode")).Value + "'";
                    statusNameList += (statusNameList == "" ? "" : ",") + ((CheckBox)item.FindControl("cbStatus")).Text.Trim();

                    //หาว่ามีการเลือกสถานะย่อยอะไรบ้าง
                    foreach (ListItem lt in ((CheckBoxList)item.FindControl("cblSubStatus")).Items)
                    {
                        if (lt.Selected)
                        {
                            subStatusCodeList += (subStatusCodeList == "" ? "" : ",") + "'" + lt.Value + "'";
                            subStatusNameList += (subStatusNameList == "" ? "" : ",") + lt.Text;
                        }
                    }
                }
            }

            return new List<string>() { statusCodelist, subStatusCodeList, statusNameList, subStatusNameList };
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchLeadData(pageControl.SelectedPageIndex, SortExpressionProperty, SortDirectionProperty);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((Label)e.Row.FindControl("lblCalculatorUrl")).Text.Trim() != "")
                {
                    ((ImageButton)e.Row.FindControl("imbCal")).Visible = true;
                    ((ImageButton)e.Row.FindControl("imbCal")).OnClientClick = AppUtil.GetCallCalculatorScript(((Label)e.Row.FindControl("lblTicketId")).Text.Trim()
                                                                                                            , ((Label)e.Row.FindControl("lblCalculatorUrl")).Text.Trim());
                }
                else
                    ((ImageButton)e.Row.FindControl("imbCal")).Visible = false;


                if (((Label)e.Row.FindControl("lblHasAdamUrl")).Text.Trim().ToUpper() == "Y")
                {
                    ((ImageButton)e.Row.FindControl("imbDoc")).Visible = true;
                }
                else
                    ((ImageButton)e.Row.FindControl("imbDoc")).Visible = false;


                //ปุ่ม Others
                if (((Label)e.Row.FindControl("lblAppNo")).Text.Trim() != "")
                {
                    string privilegeNCB = SlmScr003Biz.GetPrivilegeNCB(((Label)e.Row.FindControl("lblProductId")).Text.Trim(), (txtStaffTypeId.Text.Trim() != "" ? Convert.ToDecimal(txtStaffTypeId.Text.Trim()) : 0));
                    ((ImageButton)e.Row.FindControl("imbOthers")).Visible = privilegeNCB != "" ? true : false;
                }
                else
                    ((ImageButton)e.Row.FindControl("imbOthers")).Visible = false;

                //กรณีเข้า COC และ COCCurrentTeam ไม่ใช่ Marketing หรือ งานปิดไปแล้ว แล้วจะซ่อนปุ่ม Edit
                if ((((Label)e.Row.FindControl("lblIsCOC")).Text.Trim() == "1" && ((Label)e.Row.FindControl("lblCOCCurrentTeam")).Text.Trim() != SLMConstant.COCTeam.Marketing)
                    || ((Label)e.Row.FindControl("lbslmStatusCode")).Text.Trim() == SLMConstant.StatusCode.Reject
                    || ((Label)e.Row.FindControl("lbslmStatusCode")).Text.Trim() == SLMConstant.StatusCode.Cancel
                    || ((Label)e.Row.FindControl("lbslmStatusCode")).Text.Trim() == SLMConstant.StatusCode.Close)
                {
                    ((ImageButton)e.Row.FindControl("imbEdit")).Visible = false;
                }
                else
                {
                    ((ImageButton)e.Row.FindControl("imbEdit")).Visible = true;
                }

                //Get SubStatusDesc
                //List<ConfigProductSubStatusData> list = null;
                //if (ViewState[v_inboundsubstatuslist] == null)
                //{
                //    list = new ConfigProductSubStatusBiz().GetSubStatusList(((Label)e.Row.FindControl("lblProductId")).Text.Trim(), ((Label)e.Row.FindControl("lblCampaignId")).Text.Trim());
                //    ViewState[v_inboundsubstatuslist] = list;      //เก็บไว้ใช้ row ถัดไป
                //}
                //else
                //    list = (List<ConfigProductSubStatusData>)ViewState[v_inboundsubstatuslist];

                //string subStatusDesc = list.Where(p => p.StatusCode == ((Label)e.Row.FindControl("lbslmStatusCode")).Text.Trim()
                //                                    && p.SubStatusCode == ((Label)e.Row.FindControl("lblSubStatusCode")).Text.Trim()
                //                                    ).Select(p => p.SubStatusName).FirstOrDefault();

                var list = new ConfigProductSubStatusBiz().GetSubStatusList(((Label)e.Row.FindControl("lblProductId")).Text.Trim(), ((Label)e.Row.FindControl("lblCampaignId")).Text.Trim(), ((Label)e.Row.FindControl("lbslmStatusCode")).Text.Trim());
                string subStatusDesc = list.Where(p => p.ValueField == ((Label)e.Row.FindControl("lblSubStatusCode")).Text.Trim()).Select(p => p.TextField).FirstOrDefault();






                ((Label)e.Row.FindControl("lblSubStatusDesc")).Text = subStatusDesc;

                //Hilight Color
                if (((Label)e.Row.FindControl("lblResultFlag")).Text.Trim() == "O")
                {
                    e.Row.BackColor = System.Drawing.Color.FromArgb(213, 252, 200);
                    e.Row.BorderWidth = new Unit(1, UnitType.Pixel);
                    e.Row.BorderStyle = BorderStyle.Solid;
                    e.Row.BorderColor = System.Drawing.Color.FromArgb(163, 251, 133);
                }
                else if (((Label)e.Row.FindControl("lblResultFlag")).Text.Trim() == "I")
                {
                    e.Row.BackColor = System.Drawing.Color.FromArgb(252, 251, 192);
                    e.Row.BorderWidth = new Unit(1, UnitType.Pixel);
                    e.Row.BorderStyle = BorderStyle.Solid;
                    e.Row.BorderColor = System.Drawing.Color.FromArgb(251, 250, 143);
                }
                else if (((Label)e.Row.FindControl("lblResultFlag")).Text.Trim() == "F")
                {
                    e.Row.BackColor = System.Drawing.Color.FromArgb(249, 210, 210);
                    e.Row.BorderWidth = new Unit(1, UnitType.Pixel);
                    e.Row.BorderStyle = BorderStyle.Solid;
                    e.Row.BorderColor = System.Drawing.Color.FromArgb(249, 190, 190);
                }
            }
        }

        protected void gvResult_DataBound(object sender, EventArgs e)
        {
            try
            {
                //ViewState.Remove(v_inboundsubstatuslist);  //ลบ viewstate ที่เกิดตอนทำ rowdatabound

                ConfigBranchPrivilegeData data = ConfigBranchPrivilegeBiz.GetConfigBranchPrivilege(txtStaffBranchCode.Text.Trim());
                if (data != null)
                {
                    if (data.IsView != null && data.IsView.Value == false)
                    {
                        foreach (GridViewRow row in gvResult.Rows)
                        {
                            ((ImageButton)row.FindControl("imbView")).Visible = false;
                        }
                    }

                    if (data.IsEdit != null && data.IsEdit.Value == false)
                    {
                        foreach (GridViewRow row in gvResult.Rows)
                        {
                            ((ImageButton)row.FindControl("imbEdit")).Visible = false;
                        }
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

        protected void imbView_Click(object sender, EventArgs e)
        {
            try
            {
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                Session[inboundsearchcondition] = GetSearchCondition();

                string preleadId = ((Label)gvResult.Rows[index].FindControl("lblPreleadId")).Text.Trim();
                string ticketId = ((Label)gvResult.Rows[index].FindControl("lblTicketId")).Text.Trim();
                string campaignId = ((Label)gvResult.Rows[index].FindControl("lblCampaignId")).Text.Trim();
                string productId = ((Label)gvResult.Rows[index].FindControl("lblProductId")).Text.Trim();

                string type = new ConfigProductScreenBiz().GetFieldType(campaignId, productId, SLMConstant.ConfigProductScreen.ActionType.View);

                Session[SLMConstant.SessionName.CampaignId] = campaignId;
                Session[SLMConstant.SessionName.ProductId] = productId;
                Session[SLMConstant.NextLeadList] = Session[SLMConstant.InboundLeadList];
                Session.Remove(SLMConstant.InboundLeadList);
                Session.Remove(SLMConstant.FollowupLeadList);

                if (!string.IsNullOrEmpty(ticketId))
                {
                    Response.Redirect("SLM_SCR_004.aspx?ticketid=" + ticketId + "&type=" + type + "&backtype=3", false);
                }
                else
                {
                    Response.Redirect("SLM_SCR_004.aspx?preleadid=" + preleadId + "&type=" + type + "&backtype=3", false);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #region Backup imbView_Click 20170821
        //protected void imbView_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        int index = int.Parse(((ImageButton)sender).CommandArgument);
        //        Session[inboundsearchcondition] = GetSearchCondition();

        //        string preleadId = ((Label)gvResult.Rows[index].FindControl("lblPreleadId")).Text.Trim();
        //        string ticketId = ((Label)gvResult.Rows[index].FindControl("lblTicketId")).Text.Trim();
        //        string campaignId = ((Label)gvResult.Rows[index].FindControl("lblCampaignId")).Text.Trim();
        //        string productId = ((Label)gvResult.Rows[index].FindControl("lblProductId")).Text.Trim();

        //        string type = new ConfigProductScreenBiz().GetFieldType(campaignId, productId, SLMConstant.ConfigProductScreen.ActionType.View);

        //        if (Session[nextleadlist] != null)
        //        {
        //            var list = (List<NextLeadData>)Session[nextleadlist];    //ได้ค่า session ใน method DoSearchLeadData()
        //            var removeList = new List<NextLeadData>();

        //            foreach (var item in list)
        //            {
        //                if (!string.IsNullOrEmpty(ticketId))
        //                {
        //                    removeList.Add(item);
        //                    if (item.TicketId == ticketId) { break; }
        //                }
        //                else if (!string.IsNullOrEmpty(preleadId))
        //                {
        //                    removeList.Add(item);
        //                    if (item.PreleadId == decimal.Parse(preleadId)) { break; }
        //                }
        //            }

        //            removeList.ForEach(p => {
        //                list.Remove(p);
        //            });

        //            Session[nextleadlist] = list;

        //            //NextLeadData obj = null;

        //            //if (!string.IsNullOrEmpty(ticketId))
        //            //    obj = list.Where(p => p.TicketId == ticketId).FirstOrDefault();
        //            //else if (!string.IsNullOrEmpty(preleadId))
        //            //    obj = list.Where(p => p.PreleadId == decimal.Parse(preleadId)).FirstOrDefault();

        //            //list.Remove(obj);
        //            //Session[nextleadlist] = list;
        //        }

        //        Session[SLMConstant.SessionName.tabinbound_citizenid] = ((Label)gvResult.Rows[index].FindControl("lblCitizenId")).Text.Trim();
        //        Session[SLMConstant.SessionName.tabinbound_contractno] = ((Label)gvResult.Rows[index].FindControl("lblContractNo")).Text.Trim();
        //        Session[SLMConstant.SessionName.tabinbound_licenseno] = ((Label)gvResult.Rows[index].FindControl("lblLicenseNo")).Text.Trim();
        //        Session[SLMConstant.SessionName.tabinbound_campaignid] = campaignId;


        //        if (!string.IsNullOrEmpty(ticketId))
        //        {
        //            Response.Redirect("SLM_SCR_004.aspx?ticketid=" + ticketId + "&type=" + type + "&backtype=3", false);
        //        }
        //        else
        //        {
        //            Response.Redirect("SLM_SCR_004.aspx?preleadid=" + preleadId + "&type=" + type + "&backtype=3", false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Error(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}
        #endregion

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                txtContractNo.Text = "";
                txtTicketID.Text = "";
                txtFirstname.Text = "";
                txtLastname.Text = "";
                cmbCardType.SelectedIndex = -1;
                txtCitizenId.Text = "";
                cmbCampaign.SelectedIndex = -1;
                cmbChannel.SelectedIndex = -1;
                txtLicenseNo.Text = "";
                cmbGradeSearch.SelectedIndex = -1;
                tdmNextAppointmentFrom.DateValue = new DateTime();
                tdmNextAppointmentTo.DateValue = new DateTime();
                cmbHasNotifyPremium.SelectedIndex = -1;
                txtNotifyGrossPremiumMin.Text = "";
                txtNotifyGrossPremiumMax.Text = "";
                txtPolicyExpirationYear.Text = "";
                cmbPolicyExpirationMonth.SelectedIndex = -1;
                txtPeriodYear.Text = "";
                cmbPeriodMonth.SelectedIndex = -1;
                txtContractNoRefer.Text = "";
                tdmCreateDate.DateValue = new DateTime();
                tdmAssignDate.DateValue = new DateTime();
                cmbOwnerBranchSearch.SelectedIndex = -1;
                BindOwnerLead();
                cmbDelegateBranchSearch.SelectedIndex = -1;
                BindDelegateLead();
                cmbCreatebyBranchSearch.SelectedIndex = -1;
                BindCreateByLead();

                cbFollowUp.Checked = false;
                cbInbound.Checked = false;
                cbOutbound.Checked = false;

                //Flag การทำงาน
                cbWorkFlagAll.Checked = false;
                cbWaitCreditForm.Checked = false;
                cbWait50Tawi.Checked = false;
                cbWaitDriverLicense.Checked = false;
                cbPolicyNo.Checked = false;
                cbAct.Checked = false;
                cbStopClaim.Checked = false;
                cbStopClaim_Cancel.Checked = false;

                //สถานะ
                BindStatus();

                upSearch.Update();
                //upAdvanceSearch.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbAolSummaryReport_Click(object sender, EventArgs e)
        {
            try
            {
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                string appNo = ((Label)gvResult.Rows[index].FindControl("lblAppNo")).Text.Trim();   //"1002363";
                string productId = ((Label)gvResult.Rows[index].FindControl("lblProductId")).Text.Trim();
                string privilegeNCB = "";

                if (txtStaffTypeId.Text.Trim() != "")
                    privilegeNCB = SlmScr003Biz.GetPrivilegeNCB(productId, Convert.ToDecimal(txtStaffTypeId.Text.Trim()));

                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "callaolsummaryreport", AppUtil.GetCallAolSummaryReportScript(appNo, txtEmpCode.Text.Trim(), txtStaffTypeDesc.Text.Trim(), privilegeNCB), true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbDocument_Click(object sender, EventArgs e)
        {
            try
            {
                LeadDataForAdam leadData = SlmScr003Biz.GetLeadDataForAdam(((ImageButton)sender).CommandArgument);
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "calladam", AppUtil.GetCallAdamScript(leadData, HttpContext.Current.User.Identity.Name, txtEmpCode.Text.Trim(), false, ""), true);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbAdvanceSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnAdvanceSearch.Style["display"] == "" || pnAdvanceSearch.Style["display"] == "none")
                {
                    lbAdvanceSearch.Text = "[-] <b>Advance Search</b>";
                    pnAdvanceSearch.Style["display"] = "block";
                    txtAdvanceSearch.Text = "Y";
                }
                else
                {
                    lbAdvanceSearch.Text = "[+] <b>Advance Search</b>";
                    pnAdvanceSearch.Style["display"] = "none";
                    txtAdvanceSearch.Text = "N";
                }
                StaffBiz.SetCollapse(HttpContext.Current.User.Identity.Name, txtAdvanceSearch.Text.Trim() == "N" ? true : false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbCardType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCardType.SelectedItem.Value == "")
                lblCardNo.Text = "เลขที่บัตร";
            else if (cmbCardType.SelectedItem.Value == "1")
                lblCardNo.Text = "เลขบัตรประชาชน";
            else if (cmbCardType.SelectedItem.Value == "2")
                lblCardNo.Text = "เลขนิติบุคคล";
            else if (cmbCardType.SelectedItem.Value == "3")
                lblCardNo.Text = "เลขพาสปอร์ต";
        }

        protected void btnAddLead_Click(object sender, EventArgs e)
        {
            Session[inboundsearchcondition] = GetSearchCondition();
            Response.Redirect("SLM_SCR_010.aspx");
            //Response.Redirect("SLM_SCR_010.aspx?ReturnUrl=" + Server.UrlEncode(Request.Url.AbsoluteUri));
        }

        protected void imbEdit_Click(object sender, EventArgs e)
        {
            //Session[inboundsearchcondition] = GetSearchCondition();
            //Response.Redirect("SLM_SCR_011.aspx?ticketid=" + ((ImageButton)sender).CommandArgument);

            try
            {
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                Session[inboundsearchcondition] = GetSearchCondition();

                string preleadId = ((Label)gvResult.Rows[index].FindControl("lblPreleadId")).Text.Trim();
                string ticketId = ((Label)gvResult.Rows[index].FindControl("lblTicketId")).Text.Trim();

                string url = "";
                if (!string.IsNullOrEmpty(ticketId))
                    url = "SLM_SCR_011.aspx?ticketid=" + ticketId + "&backtype=3";
                else
                    url = "SLM_SCR_046.aspx?preleadid=" + preleadId + "&backtype=3";

                Response.Redirect(url, false);
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