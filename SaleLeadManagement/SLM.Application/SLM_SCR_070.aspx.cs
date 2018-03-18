using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Text;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;
using System.Collections;
using SLM.Resource;

namespace SLM.Application
{
    public partial class SLM_SCR_070 : System.Web.UI.Page
    {
        #region Member
        private string searchcondition = "searchcondition";
        private string nextleadlist = SLMConstant.NextLeadList;
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_070));
        #endregion

        #region Properties
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "See All";
            Page.Form.DefaultButton = btnSearch.UniqueID;
            AppUtil.SetIntTextBox(txtTicketID);
            txtTicketID.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + (Request["ticketid"] != null ? Request["ticketid"] : (" PreleadId " + Request["preleadid"])) + " Begin Page_Load(SLM_SCR_070)");
                    Session.Remove(SLMConstant.SessionName.tabscreenlist);
                    Session.Remove(SLMConstant.SessionName.configscreen);

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
                    if (Session[searchcondition] != null)
                    {
                        SetSearchCondition((SearchLeadCondition)Session[searchcondition]);  //Page Load กลับมาจากหน้าอื่น
                        Session[searchcondition] = null;
                    }
                    _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + (Request["ticketid"] != null ? Request["ticketid"] : (" PreleadId " + Request["preleadid"])) + " End Page_Load(SLM_SCR_070)");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #region Event Handler
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
        protected void cmbCreatebyBranchSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindCreateByLead();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData())
                {
                    SortExpressionProperty = string.Empty;
                    SortDirectionProperty = SortDirection.Ascending;
                    DoSearchLeadData(0, SortExpressionProperty, SortDirectionProperty);
                }
                else
                    AppUtil.ClientAlert(Page, "กรุณาเลือกเงื่อนไขอย่างน้อย 1 อย่าง");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                txtTicketID.Text = string.Empty;
                txtFirstname.Text = string.Empty;
                txtLastname.Text = string.Empty;
                cmbCardType.SelectedIndex = -1;
                txtCitizenId.Text = string.Empty;
                cmbCampaign.SelectedIndex = -1;
                cmbChannel.SelectedIndex = -1;
                txtContractNoRefer.Text = string.Empty;               
                ((SLM.Application.Shared.TextDateMask)tdmCreateDate).DateValue = new DateTime();
                ((SLM.Application.Shared.TextDateMask)tdmAssignDate).DateValue = new DateTime();
                //2016-12-26 --> SR:5905-123
                ((SLM.Application.Shared.TextDateMask)tdmChangeStatusDate).DateValue = new DateTime();
                txtSubStatus.Text = string.Empty;

                foreach (ListItem li in cbOptionList.Items)
                {
                    li.Selected = false;
                }
                cbOptionAll.Checked = false;
                cmbOwnerBranchSearch.SelectedIndex = -1;
                BindOwnerLead();
                cmbDelegateBranchSearch.SelectedIndex = -1;
                BindDelegateLead();
                cmbCreatebyBranchSearch.SelectedIndex = -1;
                BindCreateByLead();

                upSearch.Update();
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
                Session[searchcondition] = GetSearchCondition();

                string ticketId = ((Label)gvResult.Rows[index].FindControl("lblTicketId")).Text.Trim();
                string campaignId = ((Label)gvResult.Rows[index].FindControl("lblCampaignId")).Text.Trim();
                string productId = ((Label)gvResult.Rows[index].FindControl("lblProductId")).Text.Trim();

                string type = new ConfigProductScreenBiz().GetFieldType(campaignId, productId, SLMConstant.ConfigProductScreen.ActionType.View);

                if (Session[nextleadlist] != null)
                {
                    var list = (List<NextLeadData>)Session[nextleadlist];
                    NextLeadData obj = list.Where(p => p.TicketId == ticketId).FirstOrDefault();
                    list.Remove(obj);
                    Session[nextleadlist] = list;
                }

                Response.Redirect("SLM_SCR_004.aspx?ticketid=" + ticketId + "&type=" + type + "&backtype=1", false);
                //Response.Redirect("SLM_SCR_004.aspx?ticketid=" + ((ImageButton)sender).CommandArgument + "&ReturnUrl=" + Server.UrlEncode(Request.Url.AbsoluteUri));
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void imbEdit_Click(object sender, EventArgs e)
        {
            try
            {
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                Session[searchcondition] = GetSearchCondition();

                string ticketId = ((Label)gvResult.Rows[index].FindControl("lblTicketId")).Text.Trim();
                Response.Redirect("SLM_SCR_011.aspx?ticketid=" + ticketId + "&backtype=1", false);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void cbOptionAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbOptionAll.Checked)
            {
                foreach (ListItem li in cbOptionList.Items)
                {
                    li.Selected = true;
                }
            }
            else
            {
                foreach (ListItem li in cbOptionList.Items)
                {
                    li.Selected = false;
                }
            }
        }
        protected void cbOptionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAllCondition();
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
                //convert customerdetail.contactBranch, channelInfo.Branch, to newBranchCode
                leadData.ContactBranch = BranchBiz.GetBranchCodeNew(leadData.ContactBranch);
                leadData.Branch = BranchBiz.GetBranchCodeNew(leadData.Branch);

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
        protected void txtOwnerBranchSearch_TextChanged(object sender, EventArgs e)
        {
            BindOwnerLead();
        }
        protected void txtDelegateBranchSearch_TextChanged(object sender, EventArgs e)
        {
            BindDelegateLead();
        }
        #endregion

        #region Generic Function
        private void InitialControl()
        {
            //ประเภทบุคคล
            cmbCardType.DataSource = CardTypeBiz.GetCardTypeList();
            cmbCardType.DataTextField = "TextField";
            cmbCardType.DataValueField = "ValueField";
            cmbCardType.DataBind();
            cmbCardType.Items.Insert(0, new ListItem("", ""));
            
            cmbCampaign.DataSource = SlmScr003Biz.GetSaleAndBothCampaignData();
            cmbCampaign.DataTextField = "TextField";
            cmbCampaign.DataValueField = "ValueField";
            cmbCampaign.DataBind();
            cmbCampaign.Items.Insert(0, new ListItem("", ""));
            
            cmbChannel.DataSource = SlmScr003Biz.GetChannelData();
            cmbChannel.DataTextField = "TextField";
            cmbChannel.DataValueField = "ValueField";
            cmbChannel.DataBind();
            cmbChannel.Items.Insert(0, new ListItem("", ""));
            
            var branchList = BranchBiz.GetBranchList(SLMConstant.Branch.All);
            //Owner Branch
            cmbOwnerBranchSearch.DataSource = branchList;
            cmbOwnerBranchSearch.DataTextField = "TextField";
            cmbOwnerBranchSearch.DataValueField = "ValueField";
            cmbOwnerBranchSearch.DataBind();
            cmbOwnerBranchSearch.Items.Insert(0, new ListItem("", ""));
            cmbOwnerLeadSearch.Items.Insert(0, new ListItem("", ""));

            //Delegate Branch
            cmbDelegateBranchSearch.DataSource = branchList;
            cmbDelegateBranchSearch.DataTextField = "TextField";
            cmbDelegateBranchSearch.DataValueField = "ValueField";
            cmbDelegateBranchSearch.DataBind();
            cmbDelegateBranchSearch.Items.Insert(0, new ListItem("", ""));
            cmbDelegateLeadSearch.Items.Insert(0, new ListItem("", ""));

            //CreateBy Branch
            cmbCreatebyBranchSearch.DataSource = branchList;
            cmbCreatebyBranchSearch.DataTextField = "TextField";
            cmbCreatebyBranchSearch.DataValueField = "ValueField";
            cmbCreatebyBranchSearch.DataBind();
            cmbCreatebyBranchSearch.Items.Insert(0, new ListItem("", ""));
            cmbCreatebySearch.Items.Insert(0, new ListItem("", ""));

            var statusList = SlmScr003Biz.GetOptionList(AppConstant.OptionType.LeadStatus);
            ControlListData data = statusList.Where(p => p.ValueField == "16").FirstOrDefault();
            if (data != null)
                statusList.Remove(data);

            cbOptionList.DataSource = statusList;
            cbOptionList.DataTextField = "TextField";
            cbOptionList.DataValueField = "ValueField";
            cbOptionList.DataBind();
            
            pcTop.SetVisible = false;
        }
        private void BindOwnerLead()
        {
            cmbOwnerLeadSearch.DataSource = StaffBiz.GetStaffNotDummyList(cmbOwnerBranchSearch.SelectedItem.Value);
            cmbOwnerLeadSearch.DataTextField = "TextField";
            cmbOwnerLeadSearch.DataValueField = "ValueField";
            cmbOwnerLeadSearch.DataBind();
            cmbOwnerLeadSearch.Items.Insert(0, new ListItem("", ""));
        }
        private void BindDelegateLead()
        {
            cmbDelegateLeadSearch.DataSource = StaffBiz.GetStaffNotDummyList(cmbDelegateBranchSearch.SelectedItem.Value);
            cmbDelegateLeadSearch.DataTextField = "TextField";
            cmbDelegateLeadSearch.DataValueField = "ValueField";
            cmbDelegateLeadSearch.DataBind();
            cmbDelegateLeadSearch.Items.Insert(0, new ListItem("", ""));
        }
        private void BindCreateByLead()
        {
            cmbCreatebySearch.DataSource = StaffBiz.GetStaffList(cmbCreatebyBranchSearch.SelectedItem.Value);
            cmbCreatebySearch.DataTextField = "TextField";
            cmbCreatebySearch.DataValueField = "ValueField";
            cmbCreatebySearch.DataBind();
            cmbCreatebySearch.Items.Insert(0, new ListItem("", ""));
        }
        private void SetSearchCondition(SearchLeadCondition conn)
        {
            bool dosearch = false;
            try
            {
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
                if (!string.IsNullOrEmpty(conn.CardType))
                {
                    cmbCardType.SelectedIndex = cmbCardType.Items.IndexOf(cmbCardType.Items.FindByValue(conn.CardType));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CitizenId))
                {
                    txtCitizenId.Text = conn.CitizenId;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CampaignId))
                {
                    cmbCampaign.SelectedIndex = cmbCampaign.Items.IndexOf(cmbCampaign.Items.FindByValue(conn.CampaignId));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.ChannelId))
                {
                    cmbChannel.SelectedIndex = cmbChannel.Items.IndexOf(cmbChannel.Items.FindByValue(conn.ChannelId));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.ContractNoRefer))
                {
                    txtContractNoRefer.Text = conn.ContractNoRefer;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.OwnerBranch))
                {
                    cmbOwnerBranchSearch.SelectedIndex = cmbOwnerBranchSearch.Items.IndexOf(cmbOwnerBranchSearch.Items.FindByValue(conn.OwnerBranch));
                    BindOwnerLead();
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.OwnerUsername))
                {
                    cmbOwnerLeadSearch.SelectedIndex = cmbOwnerLeadSearch.Items.IndexOf(cmbOwnerLeadSearch.Items.FindByValue(conn.OwnerUsername));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.DelegateBranch))
                {
                    cmbDelegateBranchSearch.SelectedIndex = cmbDelegateBranchSearch.Items.IndexOf(cmbDelegateBranchSearch.Items.FindByValue(conn.DelegateBranch));
                    BindDelegateLead();
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.DelegateLead))
                {
                    cmbDelegateLeadSearch.SelectedIndex = cmbDelegateLeadSearch.Items.IndexOf(cmbDelegateLeadSearch.Items.FindByValue(conn.DelegateLead));
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CreateByBranch))
                {
                    cmbCreatebyBranchSearch.SelectedIndex = cmbCreatebyBranchSearch.Items.IndexOf(cmbCreatebyBranchSearch.Items.FindByValue(conn.CreateByBranch));
                    BindCreateByLead();
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.CreateBy))
                {
                    cmbCreatebySearch.SelectedIndex = cmbCreatebySearch.Items.IndexOf(cmbCreatebySearch.Items.FindByValue(conn.CreateBy));
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
                //2016-12-26 --> SR:5905-123
                if (conn.ChangeStatusDate.Year != 1)
                {
                    tdmChangeStatusDate.DateValue = conn.ChangeStatusDate;
                    dosearch = true;
                }
                if (!string.IsNullOrEmpty(conn.SubStatus))
                {
                    txtSubStatus.Text = conn.SubStatus;
                    dosearch = true;
                }

                foreach (ListItem lst in cbOptionList.Items)
                {
                    lst.Selected = false;
                }

                if (!string.IsNullOrEmpty(conn.StatusList))
                {
                    string[] vals = conn.StatusList.Split(',');
                    foreach (string val in vals)
                    {
                        ListItem lst = cbOptionList.Items.FindByValue(val.Replace("'", ""));
                        if (lst != null) lst.Selected = true;
                        dosearch = true;
                    }
                }

                CheckAllCondition();

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

                if (dosearch)
                    DoSearchLeadData(conn.PageIndex, SortExpressionProperty, SortDirectionProperty);
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
                string orderByFlag = "";

                if (sortExpression == "Notice" && sortDirection == SortDirection.Ascending)
                    orderByFlag = SLMConstant.SearchOrderBy.Note;
                else if (sortExpression == "CampaignName" || sortExpression == "StatusDesc")
                    orderByFlag = SLMConstant.SearchOrderBy.None;
                else
                    orderByFlag = SLMConstant.SearchOrderBy.SLA;    //Default

                string logError = "";
                List<SearchLeadResult> result = SlmScr003Biz.SearchLeadSeeAllData(GetSearchCondition(), HttpContext.Current.User.Identity.Name, orderByFlag, out logError);
                if (!string.IsNullOrEmpty(logError))
                {
                    _log.Error(logError);
                }

                if (sortExpression == "CampaignName")
                {
                    if (sortDirection == SortDirection.Ascending)
                        result = result.OrderBy(p => p.CampaignName).ToList();
                    else
                        result = result.OrderByDescending(p => p.CampaignName).ToList();
                }
                else if (sortExpression == "StatusDesc")
                {
                    if (sortDirection == SortDirection.Ascending)
                        result = result.OrderBy(p => p.StatusDesc).ToList();
                    else
                        result = result.OrderByDescending(p => p.StatusDesc).ToList();
                }

                //Session[nextleadlist] = AppUtil.GetNextLeadList(result);
                BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), pageIndex);
                upResult.Update();
            }
            catch
            {
                throw;
            }
        }
        private SearchLeadCondition GetSearchCondition()
        {
            SearchLeadCondition data = new SearchLeadCondition();
            data.TicketId = txtTicketID.Text.Trim();
            data.Firstname = txtFirstname.Text.Trim();
            data.Lastname = txtLastname.Text.Trim();
            data.CardType = cmbCardType.Items.Count > 0 ? cmbCardType.SelectedItem.Value : string.Empty;    //ประเภทบุคคล
            data.CitizenId = txtCitizenId.Text.Trim();
            data.CampaignId = cmbCampaign.SelectedItem.Value;
            data.ChannelId = cmbChannel.SelectedItem.Value;
            data.OwnerUsername = cmbOwnerLeadSearch.Items.Count > 0 ? cmbOwnerLeadSearch.SelectedItem.Value : string.Empty;     //Owner Lead
            data.OwnerBranch = cmbOwnerBranchSearch.Items.Count > 0 ? cmbOwnerBranchSearch.SelectedItem.Value : string.Empty;   //Owner Branch
            data.DelegateBranch = cmbDelegateBranchSearch.Items.Count > 0 ? cmbDelegateBranchSearch.SelectedItem.Value : string.Empty;  //Delegate Branch
            data.DelegateLead = cmbDelegateLeadSearch.Items.Count > 0 ? cmbDelegateLeadSearch.SelectedItem.Value : string.Empty;    //Delegate Lead
            data.CreateByBranch = cmbCreatebyBranchSearch.Items.Count > 0 ? cmbCreatebyBranchSearch.SelectedItem.Value : string.Empty;  //CreateBy Branch
            data.CreateBy = cmbCreatebySearch.Items.Count > 0 ? cmbCreatebySearch.SelectedItem.Value : string.Empty;    //CreateBy
            data.CreatedDate = tdmCreateDate.DateValue;
            data.AssignedDate = tdmAssignDate.DateValue;
            //2016-12-26 --> SR:5905-123
            data.SubStatus = txtSubStatus.Text.Trim();
            data.ChangeStatusDate = tdmChangeStatusDate.DateValue;
            ////////////////////////////////////////////////////////
            data.StatusList = GetStatusList();
            data.PageIndex = pcTop.SelectedPageIndex > -1 ? pcTop.SelectedPageIndex : 0;
            data.StaffType = SlmScr003Biz.GetStaffType(HttpContext.Current.User.Identity.Name);
            data.SortExpression = SortExpressionProperty;
            data.SortDirection = SortDirectionProperty.ToString();
            data.AdvancedSearch = txtAdvanceSearch.Text.Trim() == "Y" ? true : false;
            data.ContractNoRefer = txtContractNoRefer.Text.Trim();
            data.ScreenCode = "SLM_SCR_070";
            return data;
        }
        private string GetStatusList()
        {
            string list = string.Empty;
            foreach (ListItem li in cbOptionList.Items)
            {
                if (li.Selected)
                    list += (list == string.Empty ? "" : ",") + "'" + li.Value + "'";
            }
            return list;
        }
        private bool ValidateData()
        {
            bool selected = false;
            foreach (ListItem li in cbOptionList.Items)
            {
                if (li.Selected)
                {
                    selected = true;
                    break;
                }
            }

            if (txtTicketID.Text.Trim() == string.Empty && txtFirstname.Text.Trim() == string.Empty && txtLastname.Text.Trim() == string.Empty
                && txtCitizenId.Text.Trim() == string.Empty && cmbCardType.SelectedIndex == 0 && txtContractNoRefer.Text.Trim() == string.Empty
                && cmbCampaign.SelectedIndex == 0 && cmbChannel.SelectedIndex == 0
                && cmbOwnerBranchSearch.SelectedIndex == 0 && cmbOwnerLeadSearch.SelectedIndex == 0
                && cmbDelegateBranchSearch.SelectedIndex == 0 && cmbDelegateLeadSearch.SelectedIndex == 0
                && cmbCreatebyBranchSearch.SelectedIndex == 0 && cmbCreatebySearch.SelectedIndex == 0
                && tdmCreateDate.DateValue.Year == 1 && tdmAssignDate.DateValue.Year == 1
                //2016-12-26 --> SR:5905-123
                && txtSubStatus.Text.Trim() == string.Empty && tdmChangeStatusDate.DateValue.Year == 1 && selected == false)
            {
                return false;
            }
            else
                return true;
        }
        public string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SearchLeadCondition));
            MemoryStream ms = new MemoryStream();

            xmlSerializer.Serialize(ms, toSerialize);
            byte[] b = ms.GetBuffer();
            return Convert.ToBase64String(b);
        }
        private void CheckAllCondition()
        {
            int count = 0;
            foreach (ListItem li in cbOptionList.Items)
            {
                if (!li.Selected) { count += 1; }
            }

            cbOptionAll.Checked = count > 0 ? false : true;
        }
        #endregion
        
        #region Grid Control   
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
        #endregion
    }
}