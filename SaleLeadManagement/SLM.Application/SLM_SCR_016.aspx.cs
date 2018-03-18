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
using System.ServiceModel;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Resource;
using log4net;
using System.Configuration;

namespace SLM.Application
{
    public partial class SLM_SCR_016 : System.Web.UI.Page
    {
        public string CMTUserName = System.Configuration.ConfigurationManager.AppSettings["CMTUserName"];
        public string CMTPassword = System.Configuration.ConfigurationManager.AppSettings["CMTPassword"];
        public string CMTServiceName = System.Configuration.ConfigurationManager.AppSettings["CMTServiceName"];
        public string CMTSystemCode = System.Configuration.ConfigurationManager.AppSettings["CMTSystemCode"];
        public string CMTReferenceNo = System.Configuration.ConfigurationManager.AppSettings["CMTReferenceNo"];
        public int CMTCampaignNo = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CMTCampaignNo"]);
        public int CMTCampaignNoHistory = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CMTCampaignNoHistory"]);
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_016));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "แนะนำแคมเปญ";
            Page.Form.DefaultButton = btnSearch.UniqueID;
            AppUtil.SetIntTextBox(txtSearchCitizenId);
            AppUtil.SetIntTextBox(txtSearchTelNo1);
            AppUtil.SetIntTextBox(txtTelNo1);
            AppUtil.SetIntTextBox(txtAvailableTimeHour);
            AppUtil.SetIntTextBox(txtAvailableTimeMinute);
            AppUtil.SetIntTextBox(txtAvailableTimeSecond);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            { 
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_016");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    InitialControl();
                    SetScript();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void InitialControl()
        {
            //Owner Branch
            cmbOwnerBranch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
            cmbOwnerBranch.DataTextField = "TextField";
            cmbOwnerBranch.DataValueField = "ValueField";
            cmbOwnerBranch.DataBind();
            cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));

            List<StaffData> stafflist = SlmScr016Biz.GetChannelStaffData(HttpContext.Current.User.Identity.Name);
            if (stafflist.Count > 0)
            {
                lblChannel.Text = stafflist.FirstOrDefault().ChannelDesc;
                lblChannelId.Text = stafflist.FirstOrDefault().ChannelId;
            }

        }

        private void SetScript()
        {
            string script = "";
            //==================================txtAvailableTimeHour========================================================
            script = @" var hour = document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value; 
                        if(hour.length > 0)
                        {
                            while(hour.length < 2)
                            {
                                hour = '0' + hour;
                            }

                            if (hour < 8 || hour > 17)
                            { 
                                alert('กรุณากรอกเวลาอยู่ระหว่าง 8-17 น.'); document.getElementById('" + txtAvailableTimeHour.ClientID + @"').focus(); 
                                 document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value = ''
                            }
                            else
                            {
                                document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value = hour;
                            }
                        }
                        if(document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value !='' && 
                                document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value != '' &&
                                document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value != '')
                        {
                            document.getElementById('" + vtxtAvailableTime.ClientID + @"').innerHTML = ''
                        }
                        else
                        {
                             if(document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value == '' && 
                            document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value == '' && 
                            document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value == '')
                            {
                                document.getElementById('" + vtxtAvailableTime.ClientID + @"').innerHTML = ''
                            }
                            else
                            {
                                document.getElementById('" + vtxtAvailableTime.ClientID + @"').innerHTML = 'กรุณาระบุเวลาที่สะดวกให้ติดต่อกลับให้ครบถ้วน'
                            }
                        }";

            txtAvailableTimeHour.Attributes.Add("onblur", script);

            script = "if (document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value.length == 2 && document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value < 23)
                             {document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').focus(); }";
            txtAvailableTimeHour.Attributes.Add("onkeyup", script);

            //===================================txtAvailableTimeMinute=======================================================
            script = @" var hour = document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value;
                        var minute = document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value; 
                        if(minute.length > 0)
                        {
                            while(minute.length < 2)
                            {
                                minute = '0' + minute;
                            }
                            
                            if(hour == 8 && minute < 30)
                            {
                                alert('กรอกได้ตั้งแต่ 8.30 น.เท่านั้น'); document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').focus(); 
                                document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value = ''; 
                            }
                            else if(hour == 17 && minute > 30)
                            {
                                alert('กรอกได้ไม่เกิน 17.30 น.เท่านั้น'); document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').focus();
                                document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value = ''; 
                            }
                            else
                            {
                                if (minute > 59)
                                { 
                                    alert('กรุณากรอกเวลาอยู่ระหว่าง 0-59 นาที'); document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').focus();
                                    document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value = ''; 
                                }
                                else
                                {
                                    document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value = minute;
                                }
                            }
                        }
                        if(document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value !='' && 
                                document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value != '' &&
                                document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value != '')
                        {
                            document.getElementById('" + vtxtAvailableTime.ClientID + @"').innerHTML = ''
                        }
                        else
                        {
                             if(document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value == '' && 
                            document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value == '' && 
                            document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value == '')
                            {
                                document.getElementById('" + vtxtAvailableTime.ClientID + @"').innerHTML = ''
                            }
                            else
                            {
                                document.getElementById('" + vtxtAvailableTime.ClientID + @"').innerHTML = 'กรุณาระบุเวลาที่สะดวกให้ติดต่อกลับให้ครบถ้วน'
                            }
                        }";
            txtAvailableTimeMinute.Attributes.Add("onblur", script);

            script = "if (document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value.length == 2 && document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value < 59)
                            {
                                if (document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value == '')
                                 { document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value = '00'; }

                                document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').focus(); 
                            }";
            txtAvailableTimeMinute.Attributes.Add("onkeyup", script);

            //===================================txtAvailableTimeSecond=======================================================
            script = @" var hour = document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value;
                        var minute = document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value; 
                        var second = document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value; 
                        if(second.length > 0)
                        {
                            while(second.length < 2)
                            {
                                second = '0' + second;
                            }
                            
                            if (hour == 17 && minute == 30 && second > 0)
                            {
                                alert('กรอกได้ไม่เกิน 17.30.00 น.เท่านั้น'); document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').focus(); 
                                document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value = '00'; 
                            }
                            else
                            {
                                if (second > 59)
                                { 
                                    alert('กรุณากรอกเวลาอยู่ระหว่าง 0-59 นาที'); 
                                    document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value = '';
                                    document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').focus(); 
                                }
                                else
                                {
                                    document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value = second;
                                }
                            }
                        }
                        if(document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value !='' && 
                                document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value != '' &&
                                document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value != '')
                        {
                            document.getElementById('" + vtxtAvailableTime.ClientID + @"').innerHTML = ''
                        }
                        else
                        {
                             if(document.getElementById('" + txtAvailableTimeHour.ClientID + @"').value == '' && 
                            document.getElementById('" + txtAvailableTimeMinute.ClientID + @"').value == '' && 
                            document.getElementById('" + txtAvailableTimeSecond.ClientID + @"').value == '')
                            {
                                document.getElementById('" + vtxtAvailableTime.ClientID + @"').innerHTML = ''
                            }
                            else
                            {
                                document.getElementById('" + vtxtAvailableTime.ClientID + @"').innerHTML = 'กรุณาระบุเวลาที่สะดวกให้ติดต่อกลับให้ครบถ้วน'
                            }
                        }";
            txtAvailableTimeSecond.Attributes.Add("onblur", script);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSearch())
                {
                    List<CmtRecommandCampaign> listData = new List<CmtRecommandCampaign>();

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
                    body.CitizenId = txtSearchCitizenId.Text.Trim();
                    body.FirstName = txtSearchFirstname.Text.Trim();
                    body.LastName = txtSearchLastname.Text.Trim();
                    body.Phone = txtSearchTelNo1.Text.Trim();
                    body.ContractNo = txtSearchContractNoRefer.Text.Trim();
                    body.RequestDate = DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd");
                    body.Channel = lblChannelId.Text.Trim();
                    body.CampaignNum = CMTCampaignNo;
                    body.HasOffered = "N";
                    body.IsInterested = "N";
                    body.CustomerFlag = "AND";
                    body.Command = "CampaignByCustomer";
                    body.ProductTypeId = AppConstant.GetCMTProductType;
                    request.CampByCitizenIdBody = body;
                    //request.CampByCitizenIdBody = body;

                    CmtServiceProxy.CmtServiceClient cmt_service_client = new CmtServiceProxy.CmtServiceClient();
                    cmt_service_client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, AppConstant.CMTTimeout);

                    CmtServiceProxy.ICmtService service = cmt_service_client;
                    CmtServiceProxy.CampaignByCustomersResponse response = service.CampaignByCustomers(request);
                    //CmtServiceProxy.CampaignByCustomerResponse response = service.CampaignByCustomer(request);

                    if (response.status.status == "SUCCESS")
                    {
                        //foreach (CmtServiceProxy.CitizenId result in response.detail.CitizenIds)
                        foreach (CmtServiceProxy.CitizenIdCus result in response.detail.CitizenIds)
                        {
                            //กรอง result ที่มี campaign type เป็น sale และ both เท่านั้น
                            var campaignResult = SlmScr016Biz.GetMappingProductNew(result.CampaignId);
                            if (campaignResult.Count > 0)
                            {
                                CmtRecommandCampaign cData = new CmtRecommandCampaign()
                                {
                                    CitizenId = result.CitizenIds,
                                    Firstname = result.FirstName,
                                    Lastname = result.LastName,
                                    TelNo1 = result.Phone,
                                    Email = result.Email,
                                    ContractNoRefer = result.ContractNo,
                                    Remark = result.Remark,
                                    Assignment = result.Assignment,
                                    CampaignId = result.CampaignId,
                                    CampaignName = result.CampaignName,
                                    CampaignDesc = result.CampaignDescription,
                                    CampaignFullDesc = result.CampaignDescription,
                                    ExpireDate = AppUtil.ConvertToDateTime(result.ExpireDate, ""),
                                    ChannelDesc = result.Channel
                                };
                                listData.Add(cData);
                            }
                        }

                        gvResult.DataSource = listData;
                        gvResult.DataBind();

                        if (gvResult.Rows.Count > 0)
                        {
                            imgResult.Visible = true;
                            pnlResult.Visible = true;
                            pnlResult.CssClass = "NoneHidden";

                            imgHistory.Visible = true;
                            btnHistory.Visible = true;
                            gvHistory.DataSource = null;
                            gvHistory.DataBind();
                            pnlHistory.CssClass = "NoneHidden";
                        }
                    }
                    else
                    {
                        if (response.status.error_code == "E008")
                            AppUtil.ClientAlert(Page, "ไม่พบข้อมูล");
                        else
                            AppUtil.ClientAlert(Page, response.status.error_code + " : " + response.status.description);

                        imgResult.Visible = false;
                        gvResult.DataSource = null;
                        gvResult.DataBind();
                        pnlResult.CssClass = "Hidden";

                        imgHistory.Visible = false;
                        btnHistory.Visible = false;
                        gvHistory.DataSource = null;
                        gvHistory.DataBind();
                        pnlHistory.CssClass = "Hidden";
                    }
                }
                else
                {
                    imgResult.Visible = false;
                    gvResult.DataSource = null;
                    gvResult.DataBind();
                    pnlResult.CssClass = "Hidden";

                    imgHistory.Visible = false;
                    btnHistory.Visible = false;
                    gvHistory.DataSource = null;
                    gvHistory.DataBind();
                    pnlHistory.CssClass = "Hidden";
                }

                upResult.Update();
                upHistory.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidateSearch()
        {
            //if (txtSearchCitizenId.Text.Trim() == "")
            //{
            //    AppUtil.ClientAlert(Page, "กรุณาระบุรหัสบัตรประชาชน");
            //    return false;
            //}
            if (lblChannel.Text.Trim() == "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุชื่อช่องทาง");
                return false;
            }
            if (txtSearchFirstname.Text.Trim() != "" && txtSearchLastname.Text.Trim() == "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุชื่อและนามสกุลให้ครบถ้วน");
                return false;
            }
            if (txtSearchFirstname.Text.Trim() == "" && txtSearchLastname.Text.Trim() != "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุชื่อและนามสกุลให้ครบถ้วน");
                return false;
            }
            if (txtSearchCitizenId.Text.Trim() == "" && txtSearchFirstname.Text.Trim() == "" && txtSearchLastname.Text.Trim() == ""
                && txtSearchTelNo1.Text.Trim() == "" && txtSearchContractNoRefer.Text.Trim() == "")
            {
                AppUtil.ClientAlert(Page, "กรุณาระบุเงื่อนไขการค้นหาอย่างน้อย 1 อย่าง");
                return false;
            }

            return true;
        }

        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblCampaignDesc = (Label)e.Row.FindControl("lblCampaignDesc");
                if (lblCampaignDesc.Text.Trim().Length > AppConstant.Campaign.DisplayCampaignDescMaxLength)
                {
                    lblCampaignDesc.Text = lblCampaignDesc.Text.Trim().Substring(0, AppConstant.Campaign.DisplayCampaignDescMaxLength) + "...";
                    LinkButton lbShowCampaignDesc = (LinkButton)e.Row.FindControl("lbShowCampaignDesc");
                    lbShowCampaignDesc.Visible = true;
                    lbShowCampaignDesc.OnClientClick = AppUtil.GetShowCampaignDescScript(Page, lbShowCampaignDesc.CommandArgument, "016_result_campaigndesc_" + lbShowCampaignDesc.CommandArgument);
                }
            }
        }

        protected void gvHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblCampaignDesc = (Label)e.Row.FindControl("lblCampaignDesc");
                if (lblCampaignDesc.Text.Trim().Length > AppConstant.Campaign.DisplayCampaignDescMaxLength)
                {
                    lblCampaignDesc.Text = lblCampaignDesc.Text.Trim().Substring(0, AppConstant.Campaign.DisplayCampaignDescMaxLength) + "...";
                    LinkButton lbShowCampaignDesc = (LinkButton)e.Row.FindControl("lbShowCampaignDesc");
                    lbShowCampaignDesc.Visible = true;
                    lbShowCampaignDesc.OnClientClick = AppUtil.GetShowCampaignDescScript(Page, lbShowCampaignDesc.CommandArgument, "016_history_campaigndesc_" + lbShowCampaignDesc.CommandArgument);
                }
            }
        }

        protected void imbInsertLead_Click(object sender, EventArgs e)
        {
            try
            {
                int index = int.Parse(((ImageButton)sender).CommandArgument);
                txtFirstname.Text = ((Label)gvResult.Rows[index].FindControl("lblFirstname")).Text.Trim();
                txtLastname.Text = ((Label)gvResult.Rows[index].FindControl("lblLastname")).Text.Trim();
                txtTelNo1.Text = ((Label)gvResult.Rows[index].FindControl("lblTelNo1")).Text.Trim();
                txtCampaignName.Text = ((Label)gvResult.Rows[index].FindControl("lblCampaignName")).Text.Trim();
                txtContractNoRefer.Text = ((Label)gvResult.Rows[index].FindControl("lblContractNoRefer")).Text.Trim();
                txtCampaignIdHidden.Text = ((Label)gvResult.Rows[index].FindControl("lblCompaignId")).Text.Trim();      //ต้องไว้ก่อน cmbOwnerBranch, cmbOwner
                txtCampaignFullDescHidden.Text = ((Label)gvResult.Rows[index].FindControl("lblCampaignFullDesc")).Text.Trim();
                txtCitizenIdHidden.Text = ((Label)gvResult.Rows[index].FindControl("lblCitizenId")).Text.Trim();
                txtEmail.Text = ((Label)gvResult.Rows[index].FindControl("lblEmail")).Text.Trim();
                txtDetail.Text = ((Label)gvResult.Rows[index].FindControl("lblRemark")).Text.Trim();

                string owner_empcode = ((Label)gvResult.Rows[index].FindControl("lblAssignment")).Text.Trim();
                string owner_branch = StaffBiz.GetBranchCodeByEmpCode(owner_empcode);

                if (!string.IsNullOrEmpty(owner_branch))
                {
                    ListItem item = cmbOwnerBranch.Items.FindByValue(owner_branch);
                    if (item != null)
                        cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(item);
                    else
                    {
                        //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
                        if (SlmScr011Biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, txtCampaignIdHidden.Text, owner_branch))
                        {
                            //Branch ที่ถูกปิด
                            string branchName = BranchBiz.GetBranchName(owner_branch);
                            if (!string.IsNullOrEmpty(branchName))
                            {
                                cmbOwnerBranch.Items.Insert(1, new ListItem(branchName, owner_branch));
                                cmbOwnerBranch.SelectedIndex = 1;
                            }
                        }
                    }

                    cmbOwnerBranchSelectedIndexChanged();
                }

                string owner_username = StaffBiz.GetUsernameByEmpCode(owner_empcode);
                cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(owner_username));
                
                upPopup.Update();
                mpePopup.Show();
            }
            catch(Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbOwnerBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbOwnerBranchSelectedIndexChanged();
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ owner lead";
                }
                else
                {
                    vcmbOwner.Text = "";
                }
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                mpePopup.Show();
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        private void cmbOwnerBranchSelectedIndexChanged()
        {
            try
            {
                List<ControlListData> source = null;
                if (cmbOwnerBranch.SelectedItem != null)
                {
                    if (txtCampaignIdHidden.Text.Trim() == "")
                        source = StaffBiz.GetStaffList(cmbOwnerBranch.SelectedItem.Value);  
                    else
                        source = StaffBiz.GetStaffAllDataByAccessRight(txtCampaignIdHidden.Text.Trim(), cmbOwnerBranch.SelectedItem.Value);

                    //คำนวณงานในมือ
                    AppUtil.CalculateAmountJobOnHandForDropdownlist(cmbOwnerBranch.SelectedItem.Value, source);
                    cmbOwner.DataSource = source;
                    cmbOwner.DataTextField = "TextField";
                    cmbOwner.DataValueField = "ValueField";
                    cmbOwner.DataBind();
                    cmbOwner.Items.Insert(0, new ListItem("", ""));

                    if (cmbOwnerBranch.SelectedItem.Value != string.Empty)
                        cmbOwner.Enabled = true;
                    else
                        cmbOwner.Enabled = false;
                }
                else
                {
                    cmbOwner.Items.Clear();
                    cmbOwner.Enabled = false;
                }
            }
            catch
            {
                mpePopup.Show();
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
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                mpePopup.Show();
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        protected void rdInterest_CheckedChanged(object sender, EventArgs e)
        {
            CheckInsertestCondition();
            mpePopup.Show();
        }

        protected void rdNoInterest_CheckedChanged(object sender, EventArgs e)
        {
            CheckInsertestCondition();
            mpePopup.Show();
        }

        private void CheckInsertestCondition()
        {
            if (rdInterest.Checked)
            {
                pnlLeadInfo.Enabled = true;
                btnSaveNoInterest.Visible = false;
                btnCreateLead.Visible = true;
            }
            else
            {
                pnlLeadInfo.Enabled = false;
                btnSaveNoInterest.Visible = true;
                btnCreateLead.Visible = false;
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            rdInterest.Checked = false;
            rdNoInterest.Checked = false;
            pnlLeadInfo.Enabled = false;
            txtFirstname.Text = "";
            vtxtFirstname.Text = "";
            txtLastname.Text = "";
            txtTelNo1.Text = "";
            vtxtTelNo1.Text = "";
            txtAvailableTimeHour.Text = "";
            txtAvailableTimeMinute.Text = "";
            txtAvailableTimeSecond.Text = "";
            vtxtAvailableTime.Text = "";
            cmbOwnerBranch.SelectedIndex = -1;
            cmbOwner.SelectedIndex = -1;
            vcmbOwner.Text = "";
            txtContractNoRefer.Text = "";
            txtCampaignIdHidden.Text = "";          //Hidden
            txtCitizenIdHidden.Text = "";           //Hidden
            txtCampaignFullDescHidden.Text = "";    //Hidden
            txtDetail.Text = "";
            vtxtDetail.Text = "";
            btnSaveNoInterest.Visible = false;
            btnCreateLead.Visible = false;
        }

        protected void txtTelNo1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal result;
                if (txtTelNo1.Text.Trim() == string.Empty)
                {
                    vtxtTelNo1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)";
                    return;
                }

                if (txtTelNo1.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo1.Text.Trim(), out result))
                {
                    vtxtTelNo1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องเป็นตัวเลขเท่านั้น";
                    return;
                }

                if (txtTelNo1.Text.Trim().Length != 10)
                {
                    vtxtTelNo1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                    return;
                }

                if (txtTelNo1.Text.Trim().StartsWith("0") == false)
                {
                    vtxtTelNo1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                    return;
                }

                vtxtTelNo1.Text = "";
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.Message);
            }
            finally
            {
                mpePopup.Show();
            }
        }

        protected void btnSaveNoInterest_Click(object sender, EventArgs e)
        {
            try
            {
                CmtServiceProxy.UpdateCustomerFlagsResponse response = CallCmtService();
                if (response.status.status == "SUCCESS")
                {
                    btnSearch.Enabled = false;
                    btnHistory.Enabled = false;
                    upSearch.Update();
                    upResult.Update();
                    upHistory.Update();
                    AppUtil.ClientAlertAndRedirect(Page, "บันทึกการแนะนำแคมเปญเรียบร้อยแล้ว", "SLM_SCR_016.aspx");
                }
                else
                {
                    AppUtil.ClientAlert(Page, response.status.error_code + " : " + response.status.description);
                    mpePopup.Show();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCreateLead_Click(object sender, EventArgs e)
        {
            CmtServiceProxy.UpdateCustomerFlagsResponse response = null;
            string ticketId = "";

            try
            {
                if (ValidateData())
                {
                    try
                    {
                        response = CallCmtService();
                    }
                    catch (Exception exx)
                    {
                        string message = exx.InnerException != null ? exx.InnerException.Message : exx.Message;
                        _log.Debug(message);
                        btnSearch.Enabled = false;
                        btnHistory.Enabled = false;
                        upSearch.Update();
                        upResult.Update();
                        upHistory.Update();
                        AppUtil.ClientAlertAndRedirect(Page, "ไม่สามารถบันทึกผลการเลือกแคมเปญได้ รบกวนค้นหาและเลือกแคมเปญใหม่อีกครั้ง", "SLM_SCR_016.aspx");
                        return;
                    }

                    if (response.status.status == "SUCCESS")
                    {
                        ticketId = InserLead();
                        if (!string.IsNullOrEmpty(ticketId))
                        {
                            btnSearch.Enabled = false;
                            btnHistory.Enabled = false;
                            upSearch.Update();
                            upResult.Update();
                            upHistory.Update();

                            lblResultTicketId.Text = ticketId;
                            lblResultCampaign.Text = txtCampaignName.Text.Trim();
                            lblResultChannel.Text = lblChannel.Text.Trim();
                            if (cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value != "")
                            {
                                lblResultOwnerLead.Text = cmbOwner.SelectedItem.Text;

                                //int index = cmbOwner.SelectedItem.Text.IndexOf("(");
                                //if (index > -1)
                                //    lblResultOwnerLead.Text = cmbOwner.SelectedItem.Text.Remove(index);
                                //else
                                //    lblResultOwnerLead.Text = cmbOwner.SelectedItem.Text;
                            }

                            mpePopup.Hide();
                            btnOK.Focus();
                            upPopupSaveResult.Update();
                            mpePopupSaveResult.Show();
                        }
                        else
                        {
                            btnSearch.Enabled = false;
                            btnHistory.Enabled = false;
                            upSearch.Update();
                            upResult.Update();
                            upHistory.Update();
                            AppUtil.ClientAlertAndRedirect(Page, "ไม่สามารถบันทึกข้อมูลได้ รบกวนสร้างข้อมูลผู้มุ่งหวังใหม่ ที่หน้าจอค้นหา lead แล้วกดปุ่ม เพิ่ม lead", "SLM_SCR_016.aspx");
                            return;
                        }
                    }
                    else
                    {
                        AppUtil.ClientAlert(Page, response.status.error_code + " : " + response.status.description);
                        mpePopup.Show();
                    }
                }
                else
                {
                    mpePopup.Show();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                btnSearch.Enabled = false;
                btnHistory.Enabled = false;
                upSearch.Update();
                upResult.Update();
                upHistory.Update();
                AppUtil.ClientAlertAndRedirect(Page, ex.Message, "SLM_SCR_016.aspx");
            }
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("SLM_SCR_016.aspx");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlertAndRedirect(Page, ex.Message, "SLM_SCR_016.aspx");
            }
        }

        private CmtServiceProxy.UpdateCustomerFlagsResponse CallCmtService()
        {
            try
            {
                List<CmtServiceProxy.UpdInquiry> inquiries = new List<CmtServiceProxy.UpdInquiry>();

                CmtServiceProxy.UpdInquiry inq = new CmtServiceProxy.UpdInquiry();
                inq.CampaignId = txtCampaignIdHidden.Text.Trim();
                inq.IsInterested = rdInterest.Checked ? "Y" : "N";
                inq.HasOffered = "Y";
                inq.UpdatedBy = HttpContext.Current.User.Identity.Name;
                inq.Command = "UpdateCustomerFlags";

                inquiries.Add(inq);

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
                body.CitizenId = txtCitizenIdHidden.Text.Trim();
                body.UpdInquiries = inquiries.ToArray();

                request.UpdateCustFlag = body;

                CmtServiceProxy.CmtServiceClient cmt_service_client = new CmtServiceProxy.CmtServiceClient();
                cmt_service_client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, AppConstant.CMTTimeout);

                CmtServiceProxy.ICmtService service = cmt_service_client;
                CmtServiceProxy.UpdateCustomerFlagsResponse response = service.UpdateCustomerFlags(request);
                return response;
            }
            catch
            {
                throw;
            }
        }

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            try
            {
                gvHistory.DataSource = null;
                gvHistory.DataBind();
                gvHistory.Visible = true;
                upHistory.Update();

                //if (txtSearchCitizenId.Text.Trim() != "" && lblChannel.Text.Trim() != "")
                if (ValidateSearch())
                {
                    List<CmtRecommandCampaign> listData = new List<CmtRecommandCampaign>();
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
                    body.CitizenId = txtSearchCitizenId.Text.Trim();
                    body.FirstName = txtSearchFirstname.Text.Trim();
                    body.LastName = txtSearchLastname.Text.Trim();
                    body.Phone = txtSearchTelNo1.Text.Trim();
                    body.ContractNo = txtSearchContractNoRefer.Text.Trim();
                    body.RequestDate = DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd");
                    body.Channel = lblChannelId.Text.Trim();
                    body.CampaignNum = CMTCampaignNoHistory;
                    body.HasOffered = "Y";
                    body.IsInterested = "Y";
                    body.CustomerFlag = "OR";
                    body.Command = "CampaignByCustomer";
                    request.CampByCitizenIdBody = body;

                    CmtServiceProxy.CmtServiceClient cmt_service_client = new CmtServiceProxy.CmtServiceClient();
                    cmt_service_client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, AppConstant.CMTTimeout);

                    CmtServiceProxy.ICmtService service = cmt_service_client;
                    //CmtServiceProxy.CampaignByCustomerResponse response = service.CampaignByCustomer(request);
                    CmtServiceProxy.CampaignByCustomersResponse response = service.CampaignByCustomers(request);

                    if (response.status.status == "SUCCESS")
                    {
                        List<StaffData> stafflist = SlmScr016Biz.GetChannelStaffData("");
                        //foreach (CmtServiceProxy.CitizenId result in response.detail.CitizenIds)
                        foreach (CmtServiceProxy.CitizenIdCus result in response.detail.CitizenIds)
                        {
                            CmtRecommandCampaign cData = new CmtRecommandCampaign();
                            cData.CitizenId = result.CitizenIds;
                            cData.Firstname = result.FirstName;
                            cData.Lastname = result.LastName;
                            cData.TelNo1 = result.Phone;
                            cData.ContractNoRefer = result.ContractNo;
                            cData.Remark = result.Remark;
                            cData.CampaignId = result.CampaignId;
                            cData.CampaignName = result.CampaignName;
                            cData.CampaignDesc = result.CampaignDescription;
                            cData.ExpireDate = AppUtil.ConvertToDateTime(result.ExpireDate, "");
                            cData.ChannelDesc = result.Channel;
                            cData.IsInterested = result.IsInterested != null ? (result.IsInterested.Trim().ToUpper() == "Y" ? "สนใจ" : "ไม่สนใจ") : "";
                            cData.UpdatedDate = AppUtil.ConvertToDateTime(result.UpdateDate, "");

                            var staff = stafflist.Where(p => p.UserName == result.UpdatedBy).FirstOrDefault();
                            if (staff != null)
                            {
                                cData.StaffOfferBranchName = staff.BranchName;
                                cData.StaffOfferChannelDesc = staff.ChannelDesc;
                                cData.StaffOfferName = staff.StaffNameTH;
                            }

                            listData.Add(cData);
                        }

                        gvHistory.DataSource = listData;
                        gvHistory.DataBind();

                        if (gvHistory.Rows.Count > 0)
                        {
                            pnlHistory.CssClass = "NoneHidden";
                            gvHistory.Visible = true;
                        }

                        gvHistory.Style["display"] = "block";
                        upHistory.Update();
                    }
                    else
                    {
                        if (response.status.error_code == "E008")
                            AppUtil.ClientAlert(Page, "ไม่พบข้อมูล");
                        else
                            AppUtil.ClientAlert(Page, response.status.error_code + " : " + response.status.description);
                    }
                }
                //else
                //{
                //    AppUtil.ClientAlert(Page, "กรุณาระบุเลขที่บัตรประชาชนและช่องทาง");
                //}
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
            int i = 0;
            if (txtFirstname.Text.Trim() == string.Empty)
            {
                vtxtFirstname.Text = "กรุณาระบุชื่อ";
                i += 1;
            }
            else
            {
                vtxtFirstname.Text = "";
            }

            //****************************หมายเลขโทรศัพท์ 1********************************************
            decimal result1;
            if (txtTelNo1.Text.Trim() == string.Empty || txtTelNo1.Text.Trim().Length != 10)
            {
                vtxtTelNo1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                i += 1;
            }
            else if (txtTelNo1.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo1.Text.Trim(), out result1))
            {
                vtxtTelNo1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องเป็นตัวเลขเท่านั้น";
                i += 1;
            }
            else if (txtTelNo1.Text.Trim().StartsWith("0") == false)
            {
                vtxtTelNo1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                i += 1;
            }
            else
            {
                vtxtTelNo1.Text = "";
            }

            //Branch ที่ถูกปิด
            if (cmbOwnerBranch.Items.Count > 0 && cmbOwnerBranch.SelectedItem.Value != "" && !BranchBiz.CheckBranchActive(cmbOwnerBranch.SelectedItem.Value))
            {
                vcmbOwnerBranch.Text = "สาขานี้ถูกปิดแล้ว";
                i += 1;
            }
            else
                vcmbOwnerBranch.Text = "";

            //****************************Owner**********************************************************
            if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.SelectedItem.Value == string.Empty)
            {
                vcmbOwner.Text = "กรุณาระบุ Owner Lead";
                i += 1;
            }
            else
            {
                vcmbOwner.Text = "";
            }
            
            //****************************เวลาสะดวกให้ติดต่อกลับ********************************************
            if (txtAvailableTimeHour.Text.Trim() != "" || txtAvailableTimeMinute.Text.Trim() != "" || txtAvailableTimeSecond.Text.Trim() != "")
            {
                if (txtAvailableTimeHour.Text.Trim() != "" && txtAvailableTimeMinute.Text.Trim() != "" && txtAvailableTimeSecond.Text.Trim() != "")
                {
                    vtxtAvailableTime.Text = "";
                    string tmptime = txtAvailableTimeHour.Text.Trim() + txtAvailableTimeMinute.Text.Trim() + txtAvailableTimeSecond.Text.Trim();
                    if ((Convert.ToInt32(tmptime) < 083000 || Convert.ToInt32(tmptime) > 173000) == true)
                    {
                        vtxtAvailableTime.Text = "กรุณาระบุเวลาให้อยู่ระหว่าง 08.30-17.30 น.";
                        i += 1;
                    }
                }
                else
                {
                    vtxtAvailableTime.Text = "กรุณาระบุเวลาที่สะดวกให้ติดต่อกลับให้ครบถ้วน";
                    i += 1;
                }
            }
            else
                vtxtAvailableTime.Text = "";

            if (!string.IsNullOrEmpty(txtEmail.Text.Trim()) && !ValidateEmail(txtEmail.Text.Trim()))
            {
                vtxtEmail.Text = "กรุณาระบุ Email ให้ถูกต้อง";
                i += 1;
            }
            else
                vtxtEmail.Text = "";

            //รายละเอียด
            if (txtDetail.Text.Trim().Length > AppConstant.TextMaxLength)
            {
                vtxtDetail.Text = "ไม่สามารถบันทึกรายละเอียดเกิน " + AppConstant.TextMaxLength.ToString() + " ตัวอักษรได้";
                i += 1;
            }
            else
                vtxtDetail.Text = "";

            if (i > 0)
                return false;
            else
            {
                if (txtCampaignFullDescHidden.Text.Trim().Length > AppConstant.TextMaxLength)
                {
                    AppUtil.ClientAlert(Page, "ไม่สามารถบันทึกรายละเอียดแคมเปญเกิน " + AppConstant.TextMaxLength.ToString() + " ตัวอักษรได้\\r\\nรบกวนติดต่อผู้ดูแลระบบ CMT เพื่อแก้ไขรายละเอียด");
                    return false;
                }

                return true;
            }

        }

        private bool ValidateEmail(string email)
        {
            string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern);
            return reg.IsMatch(email);
        }

        private string InserLead()
        {
            string ticketId = "";
            LeadData lead = new LeadData();
            List<CampaignWSData> campaignList = new List<CampaignWSData>();

            try
            {
                lead.CardType = int.Parse(AppConstant.CardType.Person);
                lead.CitizenId = txtCitizenIdHidden.Text.Trim();
                if (!AppUtil.VerifyCitizenId(txtCitizenIdHidden.Text.Trim()))
                    throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจากรหัสบัตรประชาชนผิด รบกวนสร้างข้อมูลผู้มุ่งหวังใหม่ ที่หน้าจอค้นหา lead แล้วกดปุ่ม เพิ่ม lead");
                
                lead.CampaignId = txtCampaignIdHidden.Text.Trim();
                lead.Detail = txtDetail.Text.Trim();
                lead.ChannelId = lblChannelId.Text.Trim();  //คน ChannelId คน Login
                lead.Name = txtFirstname.Text.Trim();
                lead.LastName = txtLastname.Text.Trim();
                lead.TelNo_1 = txtTelNo1.Text.Trim();
                lead.ContractNoRefer = txtContractNoRefer.Text.Trim();
                lead.Email = txtEmail.Text.Trim();

                if (txtAvailableTimeHour.Text.Trim() != "" && txtAvailableTimeMinute.Text.Trim() != "" && txtAvailableTimeSecond.Text.Trim() != "")
                    lead.AvailableTime = txtAvailableTimeHour.Text.Trim() + txtAvailableTimeMinute.Text.Trim() + txtAvailableTimeSecond.Text.Trim();

                if (cmbOwnerBranch.Items.Count > 0)
                    lead.Owner_Branch = cmbOwnerBranch.SelectedItem.Value;

                if (cmbOwner.Items.Count > 0)                 //owner lead
                {
                    lead.Owner = cmbOwner.SelectedItem.Value;
                    StaffData StaffData = SlmScr016Biz.GetStaffData(cmbOwner.SelectedItem.Value);
                    if (StaffData != null && StaffData.StaffId != null)
                    {
                        lead.StaffId = StaffData.StaffId;
                        lead.Owner_Branch = StaffData.BranchCode;
                        lead.OwnerPositionId = StaffData.PositionId;
                    }
                }

                StaffData createbyData = SlmScr016Biz.GetStaffData(HttpContext.Current.User.Identity.Name);
                if (createbyData != null)
                {
                    lead.CreatedBy_Branch = createbyData.BranchCode;
                    lead.CreatedByPositionId = createbyData.PositionId;
                }

                CampaignWSData cpdata = new CampaignWSData();
                cpdata.CampaignId = txtCampaignIdHidden.Text.Trim();
                cpdata.CampaignName = txtCampaignName.Text.Trim();
                cpdata.CampaignDetail = txtCampaignFullDescHidden.Text.Trim();

                LeadInfoBiz leadbiz = new LeadInfoBiz();
                ticketId = leadbiz.InsertLeadSuggestCampaign(lead, cpdata, HttpContext.Current.User.Identity.Name);

                return ticketId;
            }
            catch
            {
                throw;
            }
        }

        //Move to LeadInfoBiz
        //private void InsertRenewInsurance(string ticketId)
        //{
        //    string campaignId = txtCampaignIdHidden.Text.Trim();
        //    List<ProductData> pData = SlmScr016Biz.GetProductCampaignData(campaignId);
        //    string productId = "";
        //    if (pData != null)
        //    {
        //        productId = pData[0].ProductId;
        //    }
        //    string tableName = SlmScr016Biz.GetTableName(campaignId, productId, "I");
        //    switch (tableName){
        //        case "kkslm_tr_renewinsurance": SlmScr016Biz.InsertRenewInsuranceData(ticketId, HttpContext.Current.User.Identity.Name, DateTime.Now);
        //            break;
        //    }
            

        //}

        //=============================================================================================================================

        #region Old Code

        //private void GenerateData(int RowsSize)
        //{
        //    try
        //    {
        //        List<CampaignWSData> LData = new List<CampaignWSData>();
        //        for (int i = 0; i < RowsSize; i++)
        //        {
        //            CampaignWSData cData = new CampaignWSData();
        //            cData.CampaignId = Convert.ToString(i + 1);
        //            cData.CampaignCode = "000" + Convert.ToString(i + 1);
        //            cData.CampaignName = "Campaign Interest" + Convert.ToString(i + 1);
        //            cData.CampaignDetail = "Campaign Description" + Convert.ToString(i + 1);
        //            cData.ChannelName = "TeleSale";
        //            cData.OfferName = "paopong.wat";
        //            cData.OfferDate = "01/1/2014";
        //            cData.Interest = "ไม่สนใจ";
        //            LData.Add(cData);
        //        }
        //        gvHistory.DataSource = LData;
        //        gvHistory.DataBind();
        //        if (gvHistory.Rows.Count > 0)
        //        {
        //            pnlHistory.CssClass = "NoneHidden";
        //            gvHistory.Visible = true; 
        //        }

        //        gvHistory.Style["display"] = "block";
        //        upHistory.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        int z = 0;
        //        bool interest = false;

        //        if (gvResult.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < gvResult.Rows.Count; i++)
        //            {
        //                RadioButton rdInterest = (RadioButton)gvResult.Rows[i].FindControl("rdInterest");
        //                RadioButton rdNoInterest = (RadioButton)gvResult.Rows[i].FindControl("rdNoInterest");
        //                if (rdInterest.Checked == false && rdNoInterest.Checked == false)
        //                {
        //                    //Do Nothing
        //                }
        //                else
        //                    z = z + 1;

        //                if (rdInterest.Checked) { interest = true; }
        //            }

        //            if (z == 0)
        //            {
        //                AppUtil.ClientAlert(Page, "กรุณาเลือกแคมเปญที่สนใจ/ไม่สนใจ อย่างน้อย 1 รายการ");
        //                return;
        //            }

        //            if (interest)
        //            {
        //                //cmbChannel.SelectedIndex = 0;
        //                upPopup.Update();
        //                mpePopup.Show();
        //            }
        //            else
        //            {
        //                CmtServiceProxy.UpdateCustomerFlagsResponse response = CallCmtService();
        //                if (response.status.status == "SUCCESS")
        //                {
        //                    btnSearch.Enabled = false;
        //                    btnHistory.Enabled = false;
        //                    //btnSave.Enabled = false;
        //                    upSearch.Update();
        //                    upResult.Update();
        //                    upHistory.Update();
        //                    AppUtil.ClientAlertAndRedirect(Page, "บันทึกการแนะนำแคมเปญเรียบร้อยแล้ว", "SLM_SCR_016.aspx");
        //                }
        //                else
        //                {
        //                    AppUtil.ClientAlert(Page, response.status.error_code + " : " + response.status.description);
        //                    mpePopup.Show();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            AppUtil.ClientAlert(Page, "กรุณาเลือกแคมเปญที่สนใจ/ไม่สนใจ อย่างน้อย 1 รายการ");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //protected void imbClear_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        int index = int.Parse(((ImageButton)sender).CommandArgument);
        //        ((RadioButton)gvResult.Rows[index].FindControl("rdInterest")).Checked = false;
        //        ((RadioButton)gvResult.Rows[index].FindControl("rdNoInterest")).Checked = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //private bool InserLead(List<string> ticketIdList)
        //{
        //    bool findfirst = false;
        //    bool ret = true;
        //    int count = 0;
        //    LeadData lead = new LeadData();
        //    List<CampaignWSData> campaignList = new List<CampaignWSData>();

        //    try
        //    {
        //        foreach (GridViewRow row in gvResult.Rows)
        //        {
        //            if (((RadioButton)row.FindControl("rdInterest")).Checked)
        //            {
        //                count += 1;
        //                if (findfirst == false)
        //                {
        //                    findfirst = true;
        //                    lead.CitizenId = txtSearchCitizenId.Text.Trim();
        //                    lead.CampaignId = ((Label)row.FindControl("lblCompaignId")).Text.Trim();

        //                    //List<ProductData> prodList = SlmScr016Biz.GetProductCampaignData(lead.CampaignId);
        //                    //if (prodList.Count > 0)
        //                    //{
        //                    //    lead.ProductGroupId = prodList[0].ProductGroupId;
        //                    //    lead.ProductId = prodList[0].ProductId;
        //                    //    lead.ProductName = prodList[0].ProductName;
        //                    //}

        //                    lead.Name = txtFirstname.Text.Trim();
        //                    lead.LastName = txtLastname.Text.Trim();
        //                    lead.TelNo_1 = txtTelNo1.Text.Trim();
        //                    if (txtAvailableTimeHour.Text.Trim() != "" && txtAvailableTimeMinute.Text.Trim() != "" && txtAvailableTimeSecond.Text.Trim() != "")
        //                        lead.AvailableTime = txtAvailableTimeHour.Text.Trim() + txtAvailableTimeMinute.Text.Trim() + txtAvailableTimeSecond.Text.Trim();

        //                    if (cmbOwnerBranch.Items.Count > 0)
        //                        lead.Owner_Branch = cmbOwnerBranch.SelectedItem.Value;

        //                    if (cmbOwner.Items.Count > 0)                 //owner lead
        //                    {
        //                        lead.Owner = cmbOwner.SelectedItem.Value;
        //                        StaffData StaffData = SlmScr016Biz.GetStaffData(cmbOwner.SelectedItem.Value);
        //                        if (StaffData != null && StaffData.StaffId != null)
        //                        {
        //                            lead.StaffId = Convert.ToInt32(StaffData.StaffId);
        //                        }
        //                    }

        //                    StaffData createbyData = SlmScr016Biz.GetStaffData(HttpContext.Current.User.Identity.Name);
        //                    if (createbyData != null)
        //                        lead.CreatedBy_Branch = createbyData.BranchCode;

        //                    lead.Detail = txtDetail.Text.Trim();
        //                    lead.ChannelId = lblChannelId.Text.Trim();
        //                    lead.StatusDate = DateTime.Now;
        //                }

        //                CampaignWSData cpdata = new CampaignWSData();
        //                cpdata.CampaignId = ((Label)row.FindControl("lblCompaignId")).Text.Trim();
        //                cpdata.CampaignName = ((Label)row.FindControl("lblCampaignName")).Text.Trim();
        //                cpdata.CampaignDetail = ((Label)row.FindControl("lbCampaignDetail")).Text.Trim();
        //                campaignList.Add(cpdata);
        //            }
        //        }


        //        if (count > 0)
        //        {
        //            LeadInfoBiz leadbiz = new LeadInfoBiz();
        //            ret = leadbiz.InsertLeadData2(lead, campaignList, HttpContext.Current.User.Identity.Name, ticketIdList);
        //            if (ret == false)
        //            {
        //                string message = leadbiz.ErrorMessage;
        //                _log.Debug(message);
        //            }
        //        }
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        return false;
        //    }
        //}

        #endregion

    }
}