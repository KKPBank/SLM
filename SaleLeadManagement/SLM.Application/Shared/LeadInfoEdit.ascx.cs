using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Application.Utilities;
using SLM.Biz;
using System.Configuration;
using log4net;

namespace SLM.Application.Shared
{
    public partial class LeadInfoEdit : System.Web.UI.UserControl
    {
        private int baseSalaryMaxLength = 12;
        private int defaultMaxLength = 10;
        private int percentMaxLength = 3;
        private bool useWebservice = ConfigurationManager.AppSettings["UseWebservice"] != null ? Convert.ToBoolean(ConfigurationManager.AppSettings["UseWebservice"]) : false;
        private string newassignflag = "";
        private string newDelegateFlag = "";
        private static readonly ILog _log = LogManager.GetLogger(typeof(LeadInfoEdit));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            AppUtil.SetIntTextBox(txtCitizenId);
            AppUtil.SetIntTextBox(txtTelNo_1);
            AppUtil.SetIntTextBox(txtTelNo2);
            AppUtil.SetIntTextBox(txtTelNo3);
            AppUtil.SetIntTextBox(txtYearOfCar);
            AppUtil.SetIntTextBox(txtYearOfCarRegis);
            AppUtil.SetIntTextBox(txtAvailableTimeHour);
            AppUtil.SetIntTextBox(txtAvailableTimeMinute);
            AppUtil.SetIntTextBox(txtAvailableTimeSecond);
            AppUtil.SetIntTextBox(txtExt2);
            AppUtil.SetIntTextBox(txtExt3);
            AppUtil.SetNotThaiCharacter(txtEmail);

            AppUtil.SetMoneyTextBox(txtBaseSalary, vtxtBaseSalary.ClientID, "ฐานเงินเดือนเกิน " + baseSalaryMaxLength.ToString() + " หลัก กรุณาระบุใหม่", baseSalaryMaxLength);
            AppUtil.SetMoneyTextBox(txtCarPrice, vtxtCarPrice.ClientID, "ราคารถยนต์เกิน " + defaultMaxLength.ToString() + " หลัก กรุณาระบุใหม่", defaultMaxLength);
            AppUtil.SetMoneyTextBox(txtDownPayment, vtxtDownPayment.ClientID, "เงินดาวน์เกิน " + defaultMaxLength.ToString() + " หลัก กรุณาระบุใหม่", defaultMaxLength);
            AppUtil.SetMoneyTextBox(txtFinanceAmt, vtxtFinanceAmt.ClientID, "ยอดจัด Finance เกิน " + defaultMaxLength.ToString() + " หลัก กรุณาระบุใหม่", defaultMaxLength);
            AppUtil.SetMoneyTextBox(txtBalloonAmt, vtxtBalloonAmt.ClientID, "Balloon Amount เกิน " + defaultMaxLength.ToString() + " หลัก กรุณาระบุใหม่", defaultMaxLength);
            AppUtil.SetPercentTextBox(txtDownPercent, vtxtDownPercent.ClientID, "เปอร์เซ็นต์เงินดาวน์เกิน 100 กรุณาระบุใหม่");
            AppUtil.SetPercentTextBox(txtBalloonPercent, vtxtBalloonPercent.ClientID, "Balloon Percentเกิน 100 กรุณาระบุใหม่");
            AppUtil.SetMoneyTextBox(txtInvest, vtxtInvest.ClientID, "เงินฝาก/เงินลงทุน " + defaultMaxLength.ToString() + " หลัก กรุณาระบุใหม่", defaultMaxLength);

            txtDetail.MaxLength = AppConstant.TextMaxLength;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
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

                    Page.Form.DefaultButton = btnSave.UniqueID;

                    if (!string.IsNullOrEmpty(Request["ticketid"]))
                    {
                        txtTicketId.Text = Request["ticketid"].ToString();

                        if (!CheckTicketIdPrivilege(txtTicketId.Text.Trim())) { return; }

                        LeadData lead = SlmScr010Biz.GetLeadData(txtTicketId.Text.Trim());
                        if (lead == null)
                        {
                            AppUtil.ClientAlertAndRedirect(Page, "ไม่พบ Ticket Id " + txtTicketId.Text.Trim() + " ในระบบ", "SLM_SCR_003.aspx");
                            return;
                        }

                        if (!CheckTicketCloseOrTicketCOC(lead)) { return; }

                        InitialControl();
                        SetScript();

                        if (useWebservice)
                            LoadLeadDataForWebservice();
                        else
                            LoadLeadData(lead);

                        if (txtAssignFlag.Text.Trim() == "0" || txtDelegateFlag.Text.Trim() == "1")   //ยังไม่จ่ายงาน assignedFlag = 0, delegateFlag = 1
                        {
                            cmbDelegateBranch.Enabled = false;
                            cmbDelegateLead.Enabled = false;
                            cmbOwnerBranch.Enabled = false;
                            cmbOwner.Enabled = false;
                            lblAlert.Text = "ไม่สามารถแก้ไข Owner และ Delegate ได้ เนื่องจากอยู่ระหว่างรอระบบจ่ายงาน กรุณารอ 1 นาที";
                        }
                        else
                            AppUtil.CheckOwnerPrivilege(lead.Owner, lead.Delegate, cmbOwnerBranch, cmbOwner, cmbDelegateBranch, cmbDelegateLead);
                    }
                    else
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "Ticket Id not found", "SLM_SCR_003.aspx");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }



        private bool CheckTicketIdPrivilege(string ticketId)
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

        private void InitialControl()
        {
            //Status
            cmbStatus.DataSource = SlmScr003Biz.GetOptionList("lead status");
            cmbStatus.DataTextField = "TextField";
            cmbStatus.DataValueField = "ValueField";
            cmbStatus.DataBind();
            cmbStatus.Items.Insert(0, new ListItem("", ""));

            //Delegate Branch
            //******comment by Nang 2015-04-18
            //cmbDelegateBranch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
            //cmbDelegateBranch.DataTextField = "TextField";
            //cmbDelegateBranch.DataValueField = "ValueField";
            //cmbDelegateBranch.DataBind();
            //cmbDelegateBranch.Items.Insert(0, new ListItem("", ""));

            //Owner Branch
            //******comment by Nang 2015-04-18
            //cmbOwnerBranch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);  
            //cmbOwnerBranch.DataTextField = "TextField";
            //cmbOwnerBranch.DataValueField = "ValueField";
            //cmbOwnerBranch.DataBind();
            //cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));

            //แคมเปญ
            cmbCampaignId.DataSource = SlmScr011Biz.GetCampaignEditData();
            cmbCampaignId.DataTextField = "TextField";
            cmbCampaignId.DataValueField = "ValueField";
            cmbCampaignId.DataBind();
            cmbCampaignId.Items.Insert(0, new ListItem("", ""));

            //owner lead
            //cmbOwner.DataSource = SlmScr003Biz.GetOwnerList(HttpContext.Current.User.Identity.Name);
            //cmbOwner.DataTextField = "TextField";
            //cmbOwner.DataValueField = "ValueField";
            //cmbOwner.DataBind();
            //cmbOwner.Items.Insert(0, new ListItem("", ""));

            //ช่องทาง
            cmbChannelId.DataSource = SlmScr003Biz.GetChannelData();
            cmbChannelId.DataTextField = "TextField";
            cmbChannelId.DataValueField = "ValueField";
            cmbChannelId.DataBind();
            cmbChannelId.Items.Insert(0, new ListItem("", ""));

            //สาขา
            cmbBranch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
            cmbBranch.DataTextField = "TextField";
            cmbBranch.DataValueField = "ValueField";
            cmbBranch.DataBind();
            cmbBranch.Items.Insert(0, new ListItem("", ""));

            //อาชีพ
            cmbOccupation.DataSource = SlmScr010Biz.GetOccupationData(useWebservice);
            cmbOccupation.DataTextField = "TextField";
            cmbOccupation.DataValueField = "ValueField";
            cmbOccupation.DataBind();
            cmbOccupation.Items.Insert(0, new ListItem("", ""));

            //สาขาที่สะดวกให้ติดต่อกลับ
            cmbContactBranch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
            cmbContactBranch.DataTextField = "TextField";
            cmbContactBranch.DataValueField = "ValueField";
            cmbContactBranch.DataBind();
            cmbContactBranch.Items.Insert(0, new ListItem("", ""));

            //จังหวัด
            cmbProvince.DataSource = SlmScr010Biz.GetProvinceData();
            cmbProvince.DataTextField = "TextField";
            cmbProvince.DataValueField = "ValueField";
            cmbProvince.DataBind();
            cmbProvince.Items.Insert(0, new ListItem("", ""));

            //จังหวัดที่จดทะเบียน
            cmbProvinceRegis.DataSource = SlmScr010Biz.GetProvinceData();
            cmbProvinceRegis.DataTextField = "TextField";
            cmbProvinceRegis.DataValueField = "ValueField";
            cmbProvinceRegis.DataBind();
            cmbProvinceRegis.Items.Insert(0, new ListItem("", ""));

            //ยี่ห้อรถ
            cmbBrand.DataSource = SlmScr010Biz.GetBrandData();
            cmbBrand.DataTextField = "TextField";
            cmbBrand.DataValueField = "ValueField";
            cmbBrand.DataBind();
            cmbBrand.Items.Insert(0, new ListItem("", ""));

            //ประเภทการผ่อนชำระ
            cmbPaymentType.DataSource = SlmScr010Biz.GetPaymentTypeData(useWebservice);
            cmbPaymentType.DataTextField = "TextField";
            cmbPaymentType.DataValueField = "ValueField";
            cmbPaymentType.DataBind();
            cmbPaymentType.Items.Insert(0, new ListItem("", ""));

            //Acc Type
            cmbAccType.DataSource = SlmScr010Biz.GetAccTypeData(useWebservice);
            cmbAccType.DataTextField = "TextField";
            cmbAccType.DataValueField = "ValueField";
            cmbAccType.DataBind();
            cmbAccType.Items.Insert(0, new ListItem("", ""));

            //Acc Promotion
            cmbAccPromotion.DataSource = SlmScr010Biz.GetAccPromotionData(useWebservice);
            cmbAccPromotion.DataTextField = "TextField";
            cmbAccPromotion.DataValueField = "ValueField";
            cmbAccPromotion.DataBind();
            cmbAccPromotion.Items.Insert(0, new ListItem("", ""));

            //ประเภทกรมธรรม์
            cmbPlanType.DataSource = SlmScr010Biz.GetPlanBancData();
            cmbPlanType.DataTextField = "TextField";
            cmbPlanType.DataValueField = "ValueField";
            cmbPlanType.DataBind();
            cmbPlanType.Items.Insert(0, new ListItem("", ""));

            //ประเภทบุคคล
            cmbCardType.DataSource = CardTypeBiz.GetCardTypeList();
            cmbCardType.DataTextField = "TextField";
            cmbCardType.DataValueField = "ValueField";
            cmbCardType.DataBind();
            cmbCardType.Items.Insert(0, new ListItem("", ""));
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

        private void LoadLeadData(LeadData lead)
        {
            try
            {
                if (lead != null)
                {
                    txtName.Text = lead.Name;
                    txtOldStatus.Text = lead.Status;

                    if (!string.IsNullOrEmpty(lead.Status))
                        cmbStatus.SelectedIndex = cmbStatus.Items.IndexOf(cmbStatus.Items.FindByValue(lead.Status));

                    txtLastName.Text = lead.LastName;

                    if (!string.IsNullOrEmpty(lead.CampaignId))
                    {
                        cmbCampaignId.SelectedIndex = cmbCampaignId.Items.IndexOf(cmbCampaignId.Items.FindByValue(lead.CampaignId));
                        //cmbOwner.DataSource = SlmScr003Biz.GetOwnerListByCampaignId(lead.CampaignId);
                        //cmbOwner.DataTextField = "TextField";
                        //cmbOwner.DataValueField = "ValueField";
                        //cmbOwner.DataBind();
                        //cmbOwner.Items.Insert(0, new ListItem("", ""));
                        cmbOwnerBranch.DataSource = SlmScr010Biz.GetBranchListByAccessRight(SLMConstant.Branch.Active, cmbCampaignId.SelectedItem.Value);
                        cmbOwnerBranch.DataTextField = "TextField";
                        cmbOwnerBranch.DataValueField = "ValueField";
                        cmbOwnerBranch.DataBind();
                        cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));

                        cmbDelegateBranch.DataSource = SlmScr010Biz.GetBranchListByAccessRight(SLMConstant.Branch.Active, cmbCampaignId.SelectedItem.Value);
                        cmbDelegateBranch.DataTextField = "TextField";
                        cmbDelegateBranch.DataValueField = "ValueField";
                        cmbDelegateBranch.DataBind();
                        cmbDelegateBranch.Items.Insert(0, new ListItem("", ""));
                    }

                    txtInterestedProd.Text = lead.InterestedProd;

                    if (lead.ContactLatestDate != null)
                        txtContactLatestDate.Text = lead.ContactLatestDate.Value.ToString("dd/MM/") + lead.ContactLatestDate.Value.Year.ToString() + " " + lead.ContactLatestDate.Value.ToString("HH:mm:ss");
                    if (lead.AssignedDateView != null)
                        txtAssignDate.Text = lead.AssignedDateView.Value.ToString("dd/MM/") + lead.AssignedDateView.Value.Year.ToString() + " " + lead.AssignedDateView.Value.ToString("HH:mm:ss");
                    if (lead.ContactFirstDate != null)
                        txtContactFirstDate.Text = lead.ContactFirstDate.Value.ToString("dd/MM/") + lead.ContactFirstDate.Value.Year.ToString() + " " + lead.ContactFirstDate.Value.ToString("HH:mm:ss");

                    if (!string.IsNullOrEmpty(lead.Owner_Branch))
                    {
                        ListItem item = cmbOwnerBranch.Items.FindByValue(lead.Owner_Branch);
                        if (item != null)
                            cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(item);
                        else
                        {
                            //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
                            if(SlmScr011Biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, cmbCampaignId.SelectedItem.Value,lead.Owner_Branch))
                            {
                                //Branch ที่ถูกปิด
                                string branchName = BranchBiz.GetBranchName(lead.Owner_Branch);
                                if (!string.IsNullOrEmpty(branchName))
                                {
                                    cmbOwnerBranch.Items.Insert(1, new ListItem(branchName, lead.Owner_Branch));
                                    cmbOwnerBranch.SelectedIndex = 1;
                                }
                            }
                        }

                        cmbOwnerBranchSelectedIndexChanged();   //Bind Combo Owner
                    }

                    if (!string.IsNullOrEmpty(lead.Owner))
                    {
                        //comment By Nang 2015-04-18
                        //var source = SlmScr010Biz.GetOwnerListByCampaignIdAndBranch(cmbCampaignId.SelectedItem.Value, cmbOwnerBranch.SelectedItem.Value);
                        
                        txtOldOwner.Text = lead.Owner;
                        cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(lead.Owner));
                    }

                    if (!string.IsNullOrEmpty(lead.Delegate_Branch))
                    {
                        ListItem item = cmbDelegateBranch.Items.FindByValue(lead.Delegate_Branch);
                        if (item != null)
                            cmbDelegateBranch.SelectedIndex = cmbDelegateBranch.Items.IndexOf(item);
                        else
                        {
                            //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
                            if (SlmScr011Biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, cmbCampaignId.SelectedItem.Value, lead.Delegate_Branch))
                            {
                                //Branch ที่ถูกปิด
                                string branchName = BranchBiz.GetBranchName(lead.Delegate_Branch);
                                if (!string.IsNullOrEmpty(branchName))
                                {
                                    cmbDelegateBranch.Items.Insert(1, new ListItem(branchName, lead.Delegate_Branch));
                                    cmbDelegateBranch.SelectedIndex = 1;
                                }
                            }
                        }

                        cmbDelegateBranchSelectedIndexChanged();    //Bind Combo Delegate
                    }
                  
                    if (!string.IsNullOrEmpty(lead.Delegate))
                    {
                        txtoldDelegate.Text = lead.Delegate;
                        cmbDelegateLead.SelectedIndex = cmbDelegateLead.Items.IndexOf(cmbDelegateLead.Items.FindByValue(lead.Delegate));
                    }

                    txtDealerCode.Text = lead.DealerCode;
                    txtDealerName.Text = lead.DealerName;
                    txtDealerName.ToolTip = lead.DealerName;
                    txtTopic.Text = lead.Topic;
                    txtDetail.Text = lead.Detail;

                    if (!string.IsNullOrEmpty(lead.ChannelId))
                        cmbChannelId.SelectedIndex = cmbChannelId.Items.IndexOf(cmbChannelId.Items.FindByValue(lead.ChannelId));
                    if (lead.LeadCreateDate != null)
                        txtCreateDate.Text = lead.LeadCreateDate.Value.ToString("dd/MM/") + lead.LeadCreateDate.Value.Year.ToString();

                    if(!string.IsNullOrEmpty(lead.Branch))
                    {
                        ListItem item = cmbBranch.Items.FindByValue(lead.Branch);
                        if (item != null)
                            cmbBranch.SelectedIndex = cmbBranch.Items.IndexOf(item);
                        else
                        {
                            //Branch ที่ถูกปิด
                            string branchName = BranchBiz.GetBranchName(lead.Branch);
                            if (!string.IsNullOrEmpty(branchName))
                            {
                                cmbBranch.Items.Insert(1, new ListItem(branchName, lead.Branch));
                                cmbBranch.SelectedIndex = 1;
                            }
                        }
                    }

                    if (lead.LeadCreateDate != null)
                        txtCreateTime.Text = lead.LeadCreateDate.Value.ToString("HH:mm:ss");

                    txtCompany.Text = lead.Company;

                    if (!string.IsNullOrEmpty(lead.IsCustomer))
                        cmbIsCustomer.SelectedIndex = cmbIsCustomer.Items.IndexOf(cmbIsCustomer.Items.FindByValue(lead.IsCustomer));

                    txtCusCode.Text = lead.CusCode;

                    if (lead.CardType != null)
                    {
                        cmbCardType.SelectedIndex = cmbCardType.Items.IndexOf(cmbCardType.Items.FindByValue(lead.CardType.Value.ToString()));
                        if (cmbCardType.SelectedItem.Value != "")
                        {
                            lblCitizenId.Text = "*";
                            txtCitizenId.Enabled = true;
                            txtCitizenId.Text = lead.CitizenId;
                            AppUtil.SetCardTypeValidation(cmbCardType.SelectedItem.Value, txtCitizenId);
                        }
                    }
                    else
                        txtCitizenId.Text = lead.CitizenId;
 
                    
                    if (lead.Birthdate != null)
                        tdBirthdate.DateValue = lead.Birthdate.Value;
                    if (lead.Occupation != null)
                        cmbOccupation.SelectedIndex = cmbOccupation.Items.IndexOf(cmbOccupation.Items.FindByValue(lead.Occupation.ToString()));
                    if (lead.BaseSalary != null)
                        txtBaseSalary.Text = lead.BaseSalary.Value.ToString("#,###.00");

                    txtTelNo_1.Text = lead.TelNo_1;

                    if (!string.IsNullOrEmpty(lead.ContactBranch))
                    {
                        ListItem item = cmbContactBranch.Items.FindByValue(lead.ContactBranch);
                        if (item != null)
                            cmbContactBranch.SelectedIndex = cmbContactBranch.Items.IndexOf(item);
                        else
                        {
                            //Branch ที่ถูกปิด
                            string branchName = BranchBiz.GetBranchName(lead.ContactBranch);
                            if (!string.IsNullOrEmpty(branchName))
                            {
                                cmbContactBranch.Items.Insert(1, new ListItem(branchName, lead.ContactBranch));
                                cmbContactBranch.SelectedIndex = 1;
                            }
                        }
                    }

                    txtTelNo2.Text = lead.TelNo_2;
                    txtExt2.Text = lead.Ext_2;

                    if (!string.IsNullOrEmpty(lead.AvailableTime))
                    {
                        txtAvailableTimeHour.Text = lead.AvailableTime.Substring(0, 2);
                        txtAvailableTimeMinute.Text = lead.AvailableTime.Substring(2, 2);
                        txtAvailableTimeSecond.Text = lead.AvailableTime.Substring(4, 2);
                    }

                    txtTelNo3.Text = lead.TelNo_3;
                    txtExt3.Text = lead.Ext_3;
                    txtEmail.Text = lead.Email;
                    txtAddressNo.Text = lead.AddressNo;
                    txtBuildingName.Text = lead.BuildingName;
                    txtFloor.Text = lead.Floor;
                    txtSoi.Text = lead.Soi;
                    txtStreet.Text = lead.Street;

                    if (!string.IsNullOrEmpty(lead.ProvinceCode))
                    {
                        cmbProvince.SelectedIndex = cmbProvince.Items.IndexOf(cmbProvince.Items.FindByValue(lead.ProvinceCode.ToString()));
                        ProvinceSeletedIndexChange();
                    }
                    if (!string.IsNullOrEmpty(lead.AmphurCode))
                    {
                        cmbAmphur.SelectedIndex = cmbAmphur.Items.IndexOf(cmbAmphur.Items.FindByValue(lead.AmphurCode.ToString()));
                        AmphurSelectIndexChange();
                    }
                    if (lead.Tambon != null)
                        cmbTambol.SelectedIndex = cmbTambol.Items.IndexOf(cmbTambol.Items.FindByValue(lead.Tambon.ToString()));

                    txtPostalCode.Text = lead.PostalCode;

                    if (!string.IsNullOrEmpty(lead.CarType))
                        cmbCarType.SelectedIndex = cmbCarType.Items.IndexOf(cmbCarType.Items.FindByValue(lead.CarType));

                    txtLicenseNo.Text = lead.LicenseNo;

                    if(!string.IsNullOrEmpty(lead.ProvinceRegisCode))
                        cmbProvinceRegis.SelectedIndex = cmbProvinceRegis.Items.IndexOf(cmbProvinceRegis.Items.FindByValue(lead.ProvinceRegisCode.ToString()));
                    
                    txtYearOfCar.Text = lead.YearOfCar;
                    txtYearOfCarRegis.Text = lead.YearOfCarRegis;

                    if (!string.IsNullOrEmpty(lead.BrandCode))
                    {
                        cmbBrand.SelectedIndex = cmbBrand.Items.IndexOf(cmbBrand.Items.FindByValue(lead.BrandCode.ToString()));
                        cmbBrandSelectedIndexChanged();
                    }
                    if (!string.IsNullOrEmpty(lead.Family))
                    {
                        cmbModel.SelectedIndex = cmbModel.Items.IndexOf(cmbModel.Items.FindByValue(lead.Family.ToString()));
                        cmbModelSelectedIndexChanged();
                    }
                    if (lead.Submodel != null)
                        cmbSubModel.SelectedIndex = cmbSubModel.Items.IndexOf(cmbSubModel.Items.FindByValue(lead.Submodel.ToString()));
                    if (lead.CarPrice != null)
                        txtCarPrice.Text = lead.CarPrice.Value.ToString("#,##0.00");
                    if (lead.DownPayment != null)
                        txtDownPayment.Text = lead.DownPayment.Value.ToString("#,##0.00");
                    if (lead.DownPercent != null)
                        txtDownPercent.Text = lead.DownPercent.Value.ToString();
                    if (lead.FinanceAmt != null)
                        txtFinanceAmt.Text = lead.FinanceAmt.Value.ToString("#,##0.00");

                    txtPaymentTerm.Text = lead.PaymentTerm;

                    if (!string.IsNullOrEmpty(lead.PaymentType))
                        cmbPaymentType.SelectedIndex = cmbPaymentType.Items.IndexOf(cmbPaymentType.Items.FindByValue(lead.PaymentType));
                    if (lead.BalloonAmt != null)
                        txtBalloonAmt.Text = lead.BalloonAmt.Value.ToString("#,##0.00");
                    if (lead.BalloonPercent != null)
                        txtBalloonPercent.Text = lead.BalloonPercent.Value.ToString();
                    if (!string.IsNullOrEmpty(lead.PlanType))
                        cmbPlanType.SelectedIndex = cmbPlanType.Items.IndexOf(cmbPlanType.Items.FindByValue(lead.PlanType));
                    if (!string.IsNullOrEmpty(lead.CoverageDate))
                    {
                        if (lead.CoverageDate.Trim().Length == 8)
                        {
                            DateTime tmpdate = new DateTime(Convert.ToInt32(lead.CoverageDate.Substring(0, 4)), Convert.ToInt32(lead.CoverageDate.Substring(4, 2)), Convert.ToInt32(lead.CoverageDate.Substring(6, 2)));
                            tdCoverageDate.DateValue = tmpdate;
                        }
                    }
                    if (lead.AccType != null)
                        cmbAccType.SelectedIndex = cmbAccType.Items.IndexOf(cmbAccType.Items.FindByValue(lead.AccType.ToString()));
                    if (lead.AccPromotion != null)
                        cmbAccPromotion.SelectedIndex = cmbAccPromotion.Items.IndexOf(cmbAccPromotion.Items.FindByValue(lead.AccPromotion.ToString()));

                    txtAccTerm.Text = lead.AccTerm;
                    txtInterest.Text = lead.Interest;

                    if(!string.IsNullOrEmpty(lead.Invest))
                        txtInvest.Text = Convert.ToDecimal(lead.Invest).ToString("#,##0.00");

                    txtLoanOd.Text = lead.LoanOd;
                    txtLoanOdTerm.Text = lead.LoanOdTerm;

                    if (!string.IsNullOrEmpty(lead.Ebank))
                        cmbEbank.SelectedIndex = cmbEbank.Items.IndexOf(cmbEbank.Items.FindByValue(lead.Ebank));
                    if (!string.IsNullOrEmpty(lead.Atm))
                        cmbAtm.SelectedIndex = cmbAtm.Items.IndexOf(cmbAtm.Items.FindByValue(lead.Atm));

                    txtPathLink.Text = lead.PathLink;
                    txtCreateBy.Text = lead.LeadCreateBy;
                    txtAssignFlag.Text = lead.AssignedFlag;
                    txtDelegateFlag.Text = lead.Delegate_Flag != null ? lead.Delegate_Flag.ToString() : "";
                    txtContractNoRefer.Text = lead.ContractNoRefer;

                    //if ((string.IsNullOrEmpty(lead.Owner) == true) && (lead.Status == "00") && ((string.IsNullOrEmpty(lead.AvailableTime)) == false) && (lead.AssignedFlag == "0"))
                    //{
                    //    cmbOwnerBranch.Enabled = true;
                    //    cmbOwner.Enabled = true;
                    //}
                }
            }
            catch
            {
                throw;
            }
        }
        private LeadData GetLeadData()
        {
            LeadData LData = new LeadData();
            LData.TicketId = txtTicketId.Text.Trim();
            //*******************************************kkslm_tr_lead****************************************************
            LData.Name = txtName.Text.Trim();                               //ชื่อ
            LData.TelNo_1 = txtTelNo_1.Text.Trim();                         //หมายเลขโทรศัพท์1
            if (!string.IsNullOrEmpty(cmbCampaignId.SelectedItem.Value))                   //แคมเปญ
                LData.CampaignId = cmbCampaignId.SelectedItem.Value;

            LData.Owner_Branch = cmbOwnerBranch.SelectedItem.Value;

            if (cmbOwner.Items.Count > 0)                 //owner lead
            {
                LData.Owner = cmbOwner.SelectedItem.Value;
                StaffData StaffData = SlmScr010Biz.GetStaffData(cmbOwner.SelectedItem.Value);
                if (StaffData != null)
                {
                    LData.StaffId = Convert.ToInt32(StaffData.StaffId);
                    //LData.Owner_Branch = StaffData.BranchCode;
                }
            }

            //if (cmbDelegateLead.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateLead.SelectedItem.Value))                     
            if (cmbDelegateLead.Items.Count > 0)//delegate lead
                LData.Delegate = cmbDelegateLead.SelectedItem.Value;
            //if (cmbDelegateBranch.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateBranch.SelectedItem.Value))
            if (cmbDelegateBranch.Items.Count > 0 )
                LData.Delegate_Branch = cmbDelegateBranch.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbStatus.SelectedItem.Value))                       //สถานะของLead
                LData.Status = cmbStatus.SelectedItem.Value;
            if (txtAvailableTimeHour.Text.Trim() != "" && txtAvailableTimeMinute.Text.Trim() != "" && txtAvailableTimeSecond.Text.Trim() != "")
                LData.AvailableTime = txtAvailableTimeHour.Text.Trim() + txtAvailableTimeMinute.Text.Trim() + txtAvailableTimeSecond.Text.Trim();             //เวลาที่สะดวก
            if (!string.IsNullOrEmpty(cmbChannelId.SelectedItem.Value))                      //ช่องทาง
                LData.ChannelId = cmbChannelId.SelectedItem.Value;
            else
                LData.ChannelId = LeadInfoBiz.GetChannelId("SLM");
            if (cmbDelegateLead.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateLead.SelectedItem.Value))
            {
                if (txtoldDelegate.Text.Trim() != cmbDelegateLead.SelectedItem.Value)
                {
                    LData.Delegate_Flag = 1;
                }
                else
                    LData.Delegate_Flag = 0;
            }
            else
                LData.Delegate_Flag = 0;
            //*******************************************Product_Info****************************************************
            LData.InterestedProd = txtInterestedProd.Text.Trim();           //Product ที่สนใจ
            LData.LicenseNo = txtLicenseNo.Text.Trim();                     //เลขทะเบียนรถ
            LData.YearOfCar = txtYearOfCar.Text.Trim();                     //ปีรถ
            LData.YearOfCarRegis = txtYearOfCarRegis.Text.Trim();           //ปีที่จดทะเบียนรถยนต์
            if (cmbBrand.SelectedItem.Value != string.Empty)                //ยี่ห้อ
            {
                LData.Brand = LeadInfoBiz.GetBrandId(cmbBrand.SelectedItem.Value);
            }
            if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty)                //รุ่น
            {
                LData.Model = LeadInfoBiz.GetModelId(cmbBrand.SelectedItem.Value, cmbModel.SelectedItem.Value);
            }
            if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty && cmbSubModel.SelectedItem.Value != string.Empty)      //รุ่นย่อยรถ (รุ่นย่อยรถเก็บ Id เป็น value อยู่แล้ว)
                LData.Submodel = Convert.ToInt32(cmbSubModel.SelectedItem.Value);        
            if (txtDownPayment.Text.Trim() != string.Empty) LData.DownPayment = decimal.Parse(txtDownPayment.Text.Trim().Replace(",", ""));                 //เงินดาวน์
            if (txtDownPercent.Text.Trim() != string.Empty) LData.DownPercent = decimal.Parse(txtDownPercent.Text.Trim());                 //เปอร์เซ็นต์เงินดาวน์
            if (txtCarPrice.Text.Trim() != string.Empty) LData.CarPrice = decimal.Parse(txtCarPrice.Text.Trim().Replace(",", ""));                       //ราคารถยนต์
            if (txtFinanceAmt.Text.Trim() != string.Empty) LData.FinanceAmt = decimal.Parse(txtFinanceAmt.Text.Trim().Replace(",", ""));                   //ยอดจัด Finance
            LData.PaymentTerm = txtPaymentTerm.Text.Trim();                 //ระยะเวลาที่ผ่อนชำระ
            if (!string.IsNullOrEmpty(cmbPaymentType.SelectedItem.Value))          //ประเภทการผ่อนชำระ
                LData.PaymentType = cmbPaymentType.SelectedItem.Value;
            if (txtBalloonAmt.Text.Trim() != string.Empty) LData.BalloonAmt = decimal.Parse(txtBalloonAmt.Text.Trim().Replace(",", ""));                   //Balloon Amount
            if (txtBalloonPercent.Text.Trim() != string.Empty) LData.BalloonPercent = decimal.Parse(txtBalloonPercent.Text.Trim());           //Balloon Percent
            if (tdCoverageDate.DateValue.Year != 1)                          //วันที่เริ่มต้นคุ้มครอง
                LData.CoverageDate = tdCoverageDate.DateValue.Year.ToString() + tdCoverageDate.DateValue.ToString("MMdd");
            if (!string.IsNullOrEmpty(cmbProvinceRegis.SelectedItem.Value))        //จังหวัดที่จดทะเบียน
                LData.ProvinceRegis = LeadInfoBiz.GetProvinceId(cmbProvinceRegis.SelectedItem.Value);
            if (cmbPlanType.SelectedItem.Value != string.Empty)
                LData.PlanType = cmbPlanType.SelectedItem.Value;                       //ประเภทกรมธรรม์
            LData.Interest = txtInterest.Text.Trim();                       //ประเภทความสนใจ
            if (!string.IsNullOrEmpty(cmbAccType.SelectedItem.Value))              //ประเภทเงินฝาก
                LData.AccType =  Convert.ToInt32("0" + cmbAccType.SelectedItem.Value);
            if (!string.IsNullOrEmpty(cmbAccPromotion.SelectedItem.Value))              //โปรโมชั่นเงินฝากที่สนใจ
                LData.AccPromotion =  Convert.ToInt32("0" + cmbAccPromotion.SelectedItem.Value);
            LData.AccTerm = txtAccTerm.Text.Trim();                         //ระยะเวลาฝาก Term
            LData.Interest = txtInterest.Text.Trim();                       //อัตราดอกเบี้ยที่สนใจ
            LData.Invest = txtInvest.Text.Trim().Replace(",", "");                          //เงินฝาก/เงินลงทุน
            LData.LoanOd = txtLoanOd.Text.Trim();                           //สินเชื่อ Over Draft
            LData.LoanOdTerm = txtLoanOdTerm.Text.Trim();                   //ระยะเวลา Over Draft
            if (!string.IsNullOrEmpty(cmbEbank.SelectedItem.Value))                //E-Banking
                LData.Ebank = cmbEbank.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbAtm.SelectedItem.Value))                  //ATM
                LData.Atm = cmbAtm.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbCarType.SelectedItem.Value))                  //ATM
                LData.CarType = cmbCarType.SelectedItem.Value;
            //**************************************Cus_Info***************************************************************************
            LData.LastName = txtLastName.Text.Trim();                       //นามสกุล
            LData.Email = txtEmail.Text.Trim();                             //E-mail
            LData.TelNo_2 = txtTelNo2.Text.Trim();                          //หมายเลขโทรศัพท์2
            LData.TelNo_3 = txtTelNo3.Text.Trim();                          //หมายเลขโทรศัพท์3
            LData.Ext_2 = txtExt2.Text.Trim();
            LData.Ext_3 = txtExt3.Text.Trim();
            LData.BuildingName = txtBuildingName.Text.Trim();               //ชื่ออาคาร/หมู่บ้าน
            LData.AddressNo = txtAddressNo.Text.Trim();                     //เลขที่
            LData.Floor = txtFloor.Text.Trim();                             //ชั้น
            LData.Soi = txtSoi.Text.Trim();                                 //ซอย
            LData.Street = txtStreet.Text.Trim();                           //ถนน
            if (cmbProvince.SelectedItem.Value != string.Empty)             //จังหวัด
            {
                LData.Province = LeadInfoBiz.GetProvinceId(cmbProvince.SelectedItem.Value);
            }
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty)               //เขต/อำเภอ
            {
                LData.Amphur = LeadInfoBiz.GetAmphurId(cmbProvince.SelectedItem.Value, cmbAmphur.SelectedItem.Value);
            }
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty && cmbTambol.SelectedItem.Value != string.Empty)               //แขวง/ตำบล (ตำบลเก็บ Id เป็น value อยู่แล้ว)
                LData.Tambon = Convert.ToInt32(cmbTambol.SelectedItem.Value);

            LData.PostalCode = txtPostalCode.Text.Trim();                   //รหัสไปรษณีย์
            if (!string.IsNullOrEmpty(cmbOccupation.SelectedItem.Value))           //อาชีพ
                LData.Occupation = Convert.ToInt32(cmbOccupation.SelectedItem.Value);
            if (txtBaseSalary.Text.Trim() != string.Empty) LData.BaseSalary = decimal.Parse(txtBaseSalary.Text.Trim().Replace(",", ""));          //ฐานเงินเดือน
            if (tdBirthdate.DateValue.Year != 1)                            //วันเกิด
                LData.Birthdate = tdBirthdate.DateValue;

            if (cmbCardType.Items.Count > 0 && cmbCardType.SelectedItem.Value != "")
                LData.CardType = Convert.ToInt32(cmbCardType.SelectedItem.Value);       //ประเภทบุคคล

            if (!string.IsNullOrEmpty(txtCitizenId.Text.Trim()))            //รหัสบัตรประชาชน
                LData.CitizenId = txtCitizenId.Text.Trim();

            LData.CusCode = txtCusCode.Text.Trim();                         
            LData.Topic = txtTopic.Text.Trim();                             //เรื่อง
            LData.Detail = txtDetail.Text.Trim();                           //รายละเอียด
            LData.PathLink = txtPathLink.Text.Trim();                       //Path Link
            LData.Company = txtCompany.Text.Trim();                         //บริษัท
            if (!string.IsNullOrEmpty(cmbContactBranch.SelectedItem.Value))        //สาขาที่สะวดติดต่อกลับ
                LData.ContactBranch = cmbContactBranch.SelectedItem.Value;
            //***********************************************Channel Info************************************************************

            if (!string.IsNullOrEmpty(cmbBranch.SelectedItem.Value))           //สาขา
                LData.Branch = cmbBranch.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbIsCustomer.SelectedItem.Value))           //เป็นลูกค้าหรือเคยเป็นลูกค้า<br />ของธนาคารหรือไม่
                LData.IsCustomer = cmbIsCustomer.SelectedItem.Value;

            if (cmbDelegateLead.Items.Count > 0)
            {
                if (cmbDelegateLead.SelectedItem.Value != txtoldDelegate.Text.Trim())
                {
                    //***********************************************kkslm_tr_activity************************************************************
                    if (cmbDelegateLead.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateLead.SelectedItem.Value))
                    {
                        LData.NewDelegate  = cmbDelegateLead.SelectedItem.Value;
                    }
                    LData.OldDelegate = txtoldDelegate.Text.Trim();
                    LData.Type = "03";
                    //LData.OldStatus = txtOldStatus.Text.Trim();
                    //LData.NewStatus = cmbStatus.SelectedItem.Value;
                }
            }

            if (cmbOwner.Items.Count > 0)
            {
                if (cmbOwner.SelectedItem.Value != txtOldOwner.Text.Trim())
                {
                    //***********************************************kkslm_tr_activity************************************************************
                    if (cmbOwner.SelectedItem != null && !string.IsNullOrEmpty(cmbOwner.SelectedItem.Value))
                    {
                        LData.NewOwner2 = cmbOwner.SelectedItem.Value;
                    }
                    LData.OldOwner2 = txtOldOwner.Text.Trim();
                    LData.Type2 = "10";
                    //LData.OldStatus = txtOldStatus.Text.Trim();
                    //LData.NewStatus = cmbStatus.SelectedItem.Value;
                }
            }
            LData.slmOldOwner = txtOldOwner.Text.Trim();
            LData.ContractNoRefer = txtContractNoRefer.Text.Trim();
            return LData;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool ActStatus = false;
            bool Actdelegate = false;
            bool ActOwner = false;
            try
            {
                if (IsLastestOwnerDelegate())
                {
                    if (ValidateData())
                    {
                        LeadInfoBiz leadBiz = new LeadInfoBiz();
                        //if (cmbStatus.SelectedItem.Value != txtOldStatus.Text.Trim())
                        //    ActStatus = true;

                        if (cmbDelegateLead.Items.Count > 0)
                        {
                            if (cmbDelegateLead.SelectedItem.Value != txtoldDelegate.Text.Trim())
                                Actdelegate = true;
                        }

                        if (cmbOwner.Items.Count > 0)
                        {
                            if (cmbOwner.SelectedItem.Value != txtOldOwner.Text.Trim())
                                ActOwner = true;
                        }

                        if (useWebservice)
                        {
                            //CallWebservice(AppUtil.GenerateXml(GetLeadDataForWebService()));
                        }
                        else
                        {
                            //string ticketId = LeadInfoBiz.UpdateLeadData(GetLeadData(), HttpContext.Current.User.Identity.Name, ActStatus, Actdelegate, ActOwner);
                            string ticketId = LeadInfoBiz.UpdateLeadData(GetLeadData(), HttpContext.Current.User.Identity.Name, ActStatus, Actdelegate, ActOwner, string.Empty);
                            txtTicketId.Text = ticketId;
                        }

                        btnSave.Enabled = false;
                        AppUtil.ClientAlertAndRedirect(Page, "บันทึกข้อมูลผู้มุ่งหวังสำเร็จ", "SLM_SCR_003.aspx");
                    }
                    else
                    {
                        AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน");
                    }
                }
                else
                {
                    //string staffname = SlmScr010Biz.GetStaffNameData(lastest_Owner);
                    AppUtil.ClientAlertAndRedirect(Page, "ข้อมูลผู้มุ่งหวังนี้ถูกจ่ายงานให้พนักงานท่านอื่นแล้ว ระบบจะทำการ Refresh หน้าจอให้", "SLM_SCR_011.aspx?ticketid=" + txtTicketId.Text.Trim());
                    return;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "ไม่พบข้อมูลผู้มุ่งหวัง กรุณาระบุใหม่")
                {
                    _log.Debug(ex.Message);
                    AppUtil.ClientAlertAndRedirect(Page, ex.Message, "SLM_SCR_003.aspx");
                }
                else
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _log.Debug(message);
                    AppUtil.ClientAlert(Page, message);
                }
            }
        }

        private LeadData GetLeadDataWebService()
        {
            LeadData LData = new LeadData();
            LData.TicketId = txtTicketId.Text.Trim();
            //*******************************************kkslm_tr_lead****************************************************
            LData.Name = txtName.Text.Trim();                               //ชื่อ
            LData.TelNo_1 = txtTelNo_1.Text.Trim();                         //หมายเลขโทรศัพท์1
            if (!string.IsNullOrEmpty(cmbCampaignId.SelectedItem.Value))                   //แคมเปญ
                LData.CampaignId = cmbCampaignId.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbOwner.SelectedItem.Value))                        //owner lead
            {
                LData.Owner = cmbOwner.SelectedItem.Value;
                if (SlmScr010Biz.GetStaffIdData(cmbOwner.SelectedItem.Value) != "")
                    LData.StaffId = Convert.ToInt32(SlmScr010Biz.GetStaffIdData(cmbOwner.SelectedItem.Value));
            }
            if (cmbDelegateLead.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateLead.SelectedItem.Value))                     //delegate lead
                LData.Delegate = cmbDelegateLead.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbStatus.SelectedItem.Value))                       //สถานะของLead
                LData.Status = cmbStatus.SelectedItem.Value;
            if (txtAvailableTimeHour.Text.Trim() != "" && txtAvailableTimeMinute.Text.Trim() != "" && txtAvailableTimeSecond.Text.Trim() != "")
                LData.AvailableTime = txtAvailableTimeHour.Text.Trim() + txtAvailableTimeMinute.Text.Trim() + txtAvailableTimeSecond.Text.Trim();             //เวลาที่สะดวก
            if (!string.IsNullOrEmpty(cmbChannelId.SelectedItem.Value))                      //ช่องทาง
                LData.ChannelId = cmbChannelId.SelectedItem.Value;
            else
                LData.ChannelId = LeadInfoBiz.GetChannelId("SLM");
            if (cmbDelegateLead.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateLead.SelectedItem.Value))
            {
                if (txtoldDelegate.Text.Trim() != cmbDelegateLead.SelectedItem.Value)
                {
                    LData.Delegate_Flag = 1;
                }
                else
                    LData.Delegate_Flag = 0;
            }
            else
                LData.Delegate_Flag = 0;
            //*******************************************Product_Info****************************************************
            LData.InterestedProd = txtInterestedProd.Text.Trim();           //Product ที่สนใจ
            LData.LicenseNo = txtLicenseNo.Text.Trim();                     //เลขทะเบียนรถ
            LData.YearOfCar = txtYearOfCar.Text.Trim();                     //ปีรถ
            LData.YearOfCarRegis = txtYearOfCarRegis.Text.Trim();           //ปีที่จดทะเบียนรถยนต์
            if (cmbBrand.SelectedItem.Value != string.Empty)                //ยี่ห้อ
            {
                LData.BrandCode = cmbBrand.SelectedItem.Value;
                //LData.Brand = LeadInfoBiz.GetBrandId(cmbBrand.SelectedItem.Value);
            }
            if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty)                //รุ่น
            {
                LData.ModelFamily = cmbModel.SelectedItem.Value;
                //LData.Model = LeadInfoBiz.GetModelId(cmbBrand.SelectedItem.Value, cmbModel.SelectedItem.Value);
            }
            if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty && cmbSubModel.SelectedItem.Value != string.Empty)      //รุ่นย่อยรถ (รุ่นย่อยรถเก็บ Id เป็น value อยู่แล้ว)
            {
                LData.SubModelCode = cmbSubModel.SelectedItem.Value;
                //LData.Submodel = Convert.ToInt32(cmbSubModel.SelectedItem.Value);
            }
            if (txtDownPayment.Text.Trim() != string.Empty) LData.DownPayment = decimal.Parse(txtDownPayment.Text.Trim().Replace(",", ""));                 //เงินดาวน์
            if (txtDownPercent.Text.Trim() != string.Empty) LData.DownPercent = decimal.Parse(txtDownPercent.Text.Trim());                 //เปอร์เซ็นต์เงินดาวน์
            if (txtCarPrice.Text.Trim() != string.Empty) LData.CarPrice = decimal.Parse(txtCarPrice.Text.Trim().Replace(",", ""));                       //ราคารถยนต์
            if (txtFinanceAmt.Text.Trim() != string.Empty) LData.FinanceAmt = decimal.Parse(txtFinanceAmt.Text.Trim().Replace(",", ""));                   //ยอดจัด Finance
            LData.PaymentTerm = txtPaymentTerm.Text.Trim();                 //ระยะเวลาที่ผ่อนชำระ
            if (!string.IsNullOrEmpty(cmbPaymentType.SelectedItem.Value))          //ประเภทการผ่อนชำระ
            {
                LData.PaymentTypeCode = cmbPaymentType.SelectedItem.Value;
                //LData.PaymentType = cmbPaymentType.SelectedItem.Value;
            }
            if (txtBalloonAmt.Text.Trim() != string.Empty) LData.BalloonAmt = decimal.Parse(txtBalloonAmt.Text.Trim().Replace(",", ""));                   //Balloon Amount
            if (txtBalloonPercent.Text.Trim() != string.Empty) LData.BalloonPercent = decimal.Parse(txtBalloonPercent.Text.Trim());           //Balloon Percent
            if (tdCoverageDate.DateValue.Year != 1)                          //วันที่เริ่มต้นคุ้มครอง
                LData.CoverageDate = tdCoverageDate.DateValue.Year.ToString() + tdCoverageDate.DateValue.ToString("MMdd");
            if (!string.IsNullOrEmpty(cmbProvinceRegis.SelectedItem.Value))        //จังหวัดที่จดทะเบียน
            {
                LData.ProvinceRegisCode = cmbProvinceRegis.SelectedItem.Value;
                //LData.ProvinceRegis = LeadInfoBiz.GetProvinceId(cmbProvinceRegis.SelectedItem.Value);
            }
            if (cmbPlanType.SelectedItem.Value != string.Empty)
            {
                LData.PlanType = cmbPlanType.SelectedItem.Value;                       //ประเภทกรมธรรม์
            }
            LData.Interest = txtInterest.Text.Trim();                       //ประเภทความสนใจ
            if (!string.IsNullOrEmpty(cmbAccType.SelectedItem.Value))              //ประเภทเงินฝาก
            {
                LData.AccTypeCode = cmbAccType.SelectedItem.Value;
                //LData.AccType = Convert.ToInt32("0" + cmbAccType.SelectedItem.Value);
            }
            if (!string.IsNullOrEmpty(cmbAccPromotion.SelectedItem.Value))              //โปรโมชั่นเงินฝากที่สนใจ
            {
                LData.AccPromotionCode = cmbAccPromotion.SelectedItem.Value;
                //LData.AccPromotion = Convert.ToInt32("0" + cmbAccPromotion.SelectedItem.Value);
            }
            LData.AccTerm = txtAccTerm.Text.Trim();                         //ระยะเวลาฝาก Term
            LData.Interest = txtInterest.Text.Trim();                       //อัตราดอกเบี้ยที่สนใจ
            LData.Invest = txtInvest.Text.Trim().Replace(",", "");                          //เงินฝาก/เงินลงทุน
            LData.LoanOd = txtLoanOd.Text.Trim();                           //สินเชื่อ Over Draft
            LData.LoanOdTerm = txtLoanOdTerm.Text.Trim();                   //ระยะเวลา Over Draft
            if (!string.IsNullOrEmpty(cmbEbank.SelectedItem.Value))                //E-Banking
                LData.Ebank = cmbEbank.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbAtm.SelectedItem.Value))                  //ATM
                LData.Atm = cmbAtm.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbCarType.SelectedItem.Value))                  //ATM
                LData.CarType = cmbCarType.SelectedItem.Value;
            //**************************************Cus_Info***************************************************************************
            LData.LastName = txtLastName.Text.Trim();                       //นามสกุล
            LData.Email = txtEmail.Text.Trim();                             //E-mail
            LData.TelNo_2 = txtTelNo2.Text.Trim();                          //หมายเลขโทรศัพท์2
            LData.TelNo_3 = txtTelNo3.Text.Trim();                          //หมายเลขโทรศัพท์3
            LData.Ext_2 = txtExt2.Text.Trim();
            LData.Ext_3 = txtExt3.Text.Trim();
            LData.BuildingName = txtBuildingName.Text.Trim();               //ชื่ออาคาร/หมู่บ้าน
            LData.AddressNo = txtAddressNo.Text.Trim();                     //เลขที่
            LData.Floor = txtFloor.Text.Trim();                             //ชั้น
            LData.Soi = txtSoi.Text.Trim();                                 //ซอย
            LData.Street = txtStreet.Text.Trim();                           //ถนน
            if (cmbProvince.SelectedItem.Value != string.Empty)             //จังหวัด
            {
                LData.ProvinceCode = cmbProvince.SelectedItem.Value;
                //LData.Province = LeadInfoBiz.GetProvinceId(cmbProvince.SelectedItem.Value);
            }
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty)               //เขต/อำเภอ
            {
                LData.AmphurCode = cmbAmphur.SelectedItem.Value;
                //LData.Amphur = LeadInfoBiz.GetAmphurId(cmbProvince.SelectedItem.Value, cmbAmphur.SelectedItem.Value);
            }
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty && cmbTambol.SelectedItem.Value != string.Empty)               //แขวง/ตำบล (ตำบลเก็บ Id เป็น value อยู่แล้ว)
            {
                LData.TambolCode = cmbTambol.SelectedItem.Value;
                //LData.Tambon = Convert.ToInt32(cmbTambol.SelectedItem.Value);
            }

            LData.PostalCode = txtPostalCode.Text.Trim();                   //รหัสไปรษณีย์
            if (!string.IsNullOrEmpty(cmbOccupation.SelectedItem.Value))           //อาชีพ
            {
                LData.OccupationCode = cmbOccupation.SelectedItem.Value;
                //LData.Occupation = Convert.ToInt32(cmbOccupation.SelectedItem.Value);
            }
            if (txtBaseSalary.Text.Trim() != string.Empty) LData.BaseSalary = decimal.Parse(txtBaseSalary.Text.Trim().Replace(",", ""));          //ฐานเงินเดือน
            if (tdBirthdate.DateValue.Year != 1)                            //วันเกิด
                LData.Birthdate = tdBirthdate.DateValue;
            if (!string.IsNullOrEmpty(txtCitizenId.Text.Trim()))            //รหัสบัตรประชาชน
                LData.CitizenId = txtCitizenId.Text.Trim();

            LData.CusCode = txtCusCode.Text.Trim();
            LData.Topic = txtTopic.Text.Trim();                             //เรื่อง
            LData.Detail = txtDetail.Text.Trim();                           //รายละเอียด
            LData.PathLink = txtPathLink.Text.Trim();                       //Path Link
            LData.Company = txtCompany.Text.Trim();                         //บริษัท
            if (!string.IsNullOrEmpty(cmbContactBranch.SelectedItem.Value))        //สาขาที่สะวดติดต่อกลับ
                LData.ContactBranch = cmbContactBranch.SelectedItem.Value;
            //***********************************************Channel Info************************************************************

            if (!string.IsNullOrEmpty(cmbBranch.SelectedItem.Value))           //สาขา
                LData.Branch = cmbBranch.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbIsCustomer.SelectedItem.Value))           //เป็นลูกค้าหรือเคยเป็นลูกค้า<br />ของธนาคารหรือไม่
                LData.IsCustomer = cmbIsCustomer.SelectedItem.Value;


            if (cmbDelegateLead.Items.Count > 0)
            {
                if (cmbDelegateLead.SelectedItem.Value != txtoldDelegate.Text.Trim())
                {
                    //***********************************************kkslm_tr_activity************************************************************
                    if (cmbDelegateLead.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateLead.SelectedItem.Value))
                    {
                        LData.NewOwner = cmbDelegateLead.SelectedItem.Value;
                    }
                    LData.OldOwner = txtoldDelegate.Text.Trim();
                    LData.Type = "01";
                    //LData.OldStatus = txtOldStatus.Text.Trim();
                    //LData.NewStatus = cmbStatus.SelectedItem.Value;
                }
            }
            return LData;
        }

        private bool ValidateData()
        {
            try
            {
                int i = 0;
                if (txtName.Text.Trim() == string.Empty)
                {
                    vtxtName.Text = "กรุณาระบุชื่อ";
                    i += 1;
                }
                else
                {
                    vtxtName.Text = "";
                }

                if (cmbCampaignId.SelectedItem.Value == string.Empty)
                {
                    vcmbCampaignId.Text = "กรุณาระบุแคมเปญ";
                    i += 1;
                }
                else
                {
                    vcmbCampaignId.Text = "";
                }

                //****************************Delegate**********************************************************

                if (cmbDelegateBranch.Items.Count > 0 && cmbDelegateBranch.SelectedItem.Value != "" && !BranchBiz.CheckBranchActive(cmbDelegateBranch.SelectedItem.Value))
                {
                    vcmbDelegateBranch.Text = "สาขานี้ถูกปิดแล้ว";
                    i += 1;
                }
                else
                    vcmbDelegateBranch.Text = "";

                if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegateLead.SelectedItem.Value == string.Empty)
                {
                    vcmbDelegateLead.Text = "กรุณาระบุ Delegate Lead";
                    i += 1;
                }
                else
                    vcmbDelegateLead.Text = "";

                //OwnerBranch, Owner
                string clearOwnerBranchText = "Y";
                if (!AppUtil.ValidateOwner(newassignflag, cmbOwnerBranch, vcmbOwnerBranch, cmbOwner, vcmbOwner, cmbCampaignId.SelectedItem.Value, ref clearOwnerBranchText))
                    i += 1;

                //Branch ที่ถูกปิด
                if (cmbOwnerBranch.Items.Count > 0 && cmbOwnerBranch.SelectedItem.Value != "" && !BranchBiz.CheckBranchActive(cmbOwnerBranch.SelectedItem.Value))
                {
                    vcmbOwnerBranch.Text = "สาขานี้ถูกปิดแล้ว";
                    i += 1;
                }
                else
                {
                    if (clearOwnerBranchText == "Y")
                        vcmbOwnerBranch.Text = "";
                }

                //****************************Branch**********************************************************

                if (cmbBranch.Items.Count > 0 && cmbBranch.SelectedItem.Value != "" && !BranchBiz.CheckBranchActive(cmbBranch.SelectedItem.Value))
                {
                    vcmbBranch.Text = "สาขานี้ถูกปิดแล้ว";
                    i += 1;
                }
                else
                    vcmbBranch.Text = "";

                //****************************Contact Branch**********************************************************

                if (cmbContactBranch.Items.Count > 0 && cmbContactBranch.SelectedItem.Value != "" && !BranchBiz.CheckBranchActive(cmbContactBranch.SelectedItem.Value))
                {
                    vcmbContactBranch.Text = "สาขานี้ถูกปิดแล้ว";
                    i += 1;
                }
                else
                    vcmbContactBranch.Text = "";

                //****************************หมายเลขโทรศัพท์ 2********************************************
                decimal result2;
                if (txtTelNo2.Text.Trim() != string.Empty && txtTelNo2.Text.Trim().Length < 9)
                {
                    vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องมีอย่างน้อย 9 หลัก";
                    i += 1;
                }
                else if (txtTelNo2.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo2.Text.Trim(), out result2))
                {
                    vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องเป็นตัวเลขเท่านั้น";
                    i += 1;
                }
                else if (txtTelNo2.Text.Trim() != string.Empty && txtTelNo2.Text.Trim().StartsWith("0") == false)
                {
                    vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                    i += 1;
                }
                else
                    vtxtTelNo2.Text = "";

                //****************************หมายเลขโทรศัพท์ 3********************************************
                decimal result3;
                if (txtTelNo3.Text.Trim() != string.Empty && txtTelNo3.Text.Trim().Length < 9)
                {
                    vtxtTelNo3.Text = "หมายเลขโทรศัพท์ 3 ต้องมีอย่างน้อย 9 หลัก";
                    i += 1;
                }
                else if (txtTelNo3.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo3.Text.Trim(), out result3))
                {
                    vtxtTelNo3.Text = "หมายเลขโทรศัพท์ 3 ต้องเป็นตัวเลขเท่านั้น";
                    i += 1;
                }
                else if (txtTelNo3.Text.Trim() != string.Empty && txtTelNo3.Text.Trim().StartsWith("0") == false)
                {
                    vtxtTelNo3.Text = "หมายเลขโทรศัพท์ 3 ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                    i += 1;
                }
                else
                    vtxtTelNo3.Text = "";

                if (ValidateEmail() == false && txtEmail.Text.Trim() != "")
                {
                    vtxtEmail.Text = "กรุณาระบุ E-mail ให้ถูกต้อง";
                    i += 1;
                }
                else
                {
                    vtxtEmail.Text = "";
                }

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
                        if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Person && txtCitizenId.Text.Trim() != "" && AppUtil.VerifyCitizenId(txtCitizenId.Text.Trim()) == false)
                        {
                            vtxtCitizenId.Text = "รหัสบัตรประชาชนไม่ถูกต้อง";
                            i += 1;
                        }
                        else if ((cmbCardType.SelectedItem.Value == AppConstant.CardType.JuristicPerson || cmbCardType.SelectedItem.Value == AppConstant.CardType.Foreigner) && txtCitizenId.Text.Trim().Length > 50)
                        {
                            vtxtCitizenId.Text = "เลข" + cmbCardType.SelectedItem.Text + "ห้ามเกิน 50 หลัก";
                            i += 1;
                        }
                        else
                            vtxtCitizenId.Text = "";
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


                if (txtBaseSalary.Text.Trim() != "")
                {
                    string result = AppUtil.SetDecimalFormat(baseSalaryMaxLength, txtBaseSalary);
                    if (result == "error")
                    {
                        vtxtBaseSalary.Text = "ฐานเงินเดือนเกิน " + baseSalaryMaxLength.ToString() + " หลัก กรุณาระบุใหม่";
                        i += 1;
                    }
                    else
                    {
                        vtxtBaseSalary.Text = "";
                    }
                }
                else
                    vtxtBaseSalary.Text = "";

                if (txtCarPrice.Text.Trim() != "")
                {
                    string result = AppUtil.SetDecimalFormat(defaultMaxLength, txtCarPrice);
                    if (result == "error")
                    {
                        vtxtCarPrice.Text = "ราคารถยนต์เกิน " + defaultMaxLength.ToString() + " หลัก กรุณาระบุใหม่";
                        i += 1;
                    }
                    else
                        vtxtCarPrice.Text = "";
                }
                else
                    vtxtCarPrice.Text = "";

                if (txtDownPayment.Text.Trim() != "")
                {
                    string result = AppUtil.SetDecimalFormat(defaultMaxLength, txtDownPayment);
                    if (result == "error")
                    {
                        vtxtDownPayment.Text = "เงินดาวน์เกิน " + defaultMaxLength.ToString() + " หลัก กรุณาระบุใหม่";
                        i += 1;
                    }
                    else
                        vtxtDownPayment.Text = "";
                }
                else
                    vtxtDownPayment.Text = "";

                if (txtDownPercent.Text.Trim() != "")
                {
                    string result = AppUtil.SetPercentFormat(percentMaxLength, txtDownPercent);
                    vtxtDownPercent.Text = "";
                    if (result == "error")
                    {
                        vtxtDownPercent.Text = "เปอร์เซ็นต์เงินดาวน์เกิน " + percentMaxLength.ToString() + " หลัก กรุณาระบุใหม่";
                        i += 1;
                    }
                    else if (result == "error100")
                    {
                        vtxtDownPercent.Text = "เปอร์เซ็นต์เงินดาวน์เกิน 100 กรุณาระบุใหม่";
                        i += 1;
                    }
                    else
                    {
                        vtxtDownPercent.Text = "";
                    }
                }
                else
                    vtxtDownPercent.Text = "";

                if (txtFinanceAmt.Text.Trim() != "")
                {
                    string result = AppUtil.SetDecimalFormat(defaultMaxLength, txtFinanceAmt);
                    if (result == "error")
                    {
                        vtxtFinanceAmt.Text = "ยอดจัด Finance เกิน " + defaultMaxLength.ToString() + " หลัก กรุณาระบุใหม่";
                        i += 1;
                    }
                    else
                        vtxtFinanceAmt.Text = "";
                }
                else
                    vtxtFinanceAmt.Text = "";

                if (txtBalloonAmt.Text.Trim() != "")
                {
                    string result = AppUtil.SetDecimalFormat(defaultMaxLength, txtBalloonAmt);
                    if (result == "error")
                    {
                        vtxtBalloonAmt.Text = "Balloon Amount เกิน " + defaultMaxLength.ToString() + " หลัก กรุณาระบุใหม่";
                        i += 1;
                    }
                    else
                        vtxtBalloonAmt.Text = "";
                }
                else
                    vtxtBalloonAmt.Text = "";

                if (txtBalloonPercent.Text.Trim() != "")
                {
                    string result = AppUtil.SetPercentFormat(percentMaxLength, txtBalloonPercent);
                    if (result == "error")
                    {
                        vtxtBalloonPercent.Text = "Balloon Percent เกิน " + percentMaxLength.ToString() + " หลัก กรุณาระบุใหม่";
                        i += 1;
                    }
                    else if (result == "error100")
                    {
                        vtxtBalloonPercent.Text = "Balloon Percent เกิน 100 กรุณาระบุใหม่";
                        i += 1;
                    }
                    else
                        vtxtBalloonPercent.Text = "";
                }
                else
                    vtxtBalloonPercent.Text = "";

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

                upHeader1.Update();
                upHeader2.Update();
                upHeader3.Update();

                if (i > 0)
                    return false;
                else
                {
                    if (txtDetail.Text.Trim().Length > AppConstant.TextMaxLength)
                    {
                        throw new Exception("ไม่สามารถบันทึกรายละเอียดเกิน " + AppConstant.TextMaxLength.ToString() + " ตัวอักษรได้");
                    }
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        //protected void txtslm_Name_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (txtName.Text.Trim() == string.Empty)
        //            vtxtName.Text = "กรุณาระบุชื่อ";
        //        else
        //            vtxtName.Text = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //}

        private bool ValidateEmail()
        {
            string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern);
            return reg.IsMatch(txtEmail.Text.Trim());
        }

        protected void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ProvinceSeletedIndexChange();
                AmphurSelectIndexChange();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ProvinceSeletedIndexChange()
        {
            try
            {
                //เขต/อำเภอ
                cmbAmphur.DataSource = SlmScr010Biz.GetAmphurData(cmbProvince.SelectedItem.Value);
                cmbAmphur.DataTextField = "TextField";
                cmbAmphur.DataValueField = "ValueField";
                cmbAmphur.DataBind();
                cmbAmphur.Items.Insert(0, new ListItem("", ""));
            }
            catch
            {
                throw;
            }
        }
        protected void cmbAmphur_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                AmphurSelectIndexChange();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void AmphurSelectIndexChange()
        {
            try
            {
                //แขวง/ตำบล
                cmbTambol.DataSource = SlmScr010Biz.GetTambolData(cmbProvince.SelectedItem.Value, cmbAmphur.SelectedItem.Value, useWebservice);
                cmbTambol.DataTextField = "TextField";
                cmbTambol.DataValueField = "ValueField";
                cmbTambol.DataBind();
                cmbTambol.Items.Insert(0, new ListItem("", ""));
            }
            catch
            {
                throw;
            }
        }

        protected void cmbCampaignId_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCampaignId.SelectedItem.Value == string.Empty)
                    vcmbCampaignId.Text = "กรุณาระบุแคมเปญ";
                else
                    vcmbCampaignId.Text = "";
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (ValidateEmail() == false && txtEmail.Text.Trim() != "")
                    vtxtEmail.Text = "กรุณาระบุ E-mail ให้ถูกต้อง";
                else
                    vtxtEmail.Text = "";
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void txtCitizenId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Request["backtype"] == "2" || Request["backtype"] == "3")
                Response.Redirect("SLM_SCR_029.aspx?backtype=" + Request["backtype"], false);
            else
                Response.Redirect("SLM_SCR_003.aspx", false);
        }

        protected void cmbDelegateBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbDelegateBranchSelectedIndexChanged();
                if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegateLead.SelectedItem.Value == string.Empty)
                {
                    vcmbDelegateLead.Text = "กรุณาระบุ Delegate Lead";
                }
                else
                {
                    vcmbDelegateLead.Text = "";
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        private void cmbDelegateBranchSelectedIndexChanged()
        {
            try
            {
                //Delegate Lead
                List<ControlListData> source = StaffBiz.GetStaffAllDataByAccessRight(cmbCampaignId.SelectedItem.Value, cmbDelegateBranch.SelectedItem.Value);
                //คำนวณงานในมือ
                AppUtil.CalculateAmountJobOnHandForDropdownlist(cmbDelegateBranch.SelectedItem.Value, source);
                cmbDelegateLead.DataSource = source;
                cmbDelegateLead.DataTextField = "TextField";
                cmbDelegateLead.DataValueField = "ValueField";
                cmbDelegateLead.DataBind();
                cmbDelegateLead.Items.Insert(0, new ListItem("", ""));

                if (cmbDelegateBranch.SelectedItem.Value != string.Empty)
                    cmbDelegateLead.Enabled = true;
                else
                    cmbDelegateLead.Enabled = false;
            }
            catch
            {
                throw;
            }
        }

        protected void cmbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbBrandSelectedIndexChanged();
                cmbModelSelectedIndexChanged();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        private void cmbBrandSelectedIndexChanged()
        {
            try
            {
                //รุ่นรถ
                cmbModel.DataSource = SlmScr010Biz.GetModelData(cmbBrand.SelectedItem.Value);
                cmbModel.DataTextField = "TextField";
                cmbModel.DataValueField = "ValueField";
                cmbModel.DataBind();
                cmbModel.Items.Insert(0, new ListItem("", ""));
            }
            catch
            {
                throw;
            }
        }
        protected void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbModelSelectedIndexChanged();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        private void cmbModelSelectedIndexChanged()
        {
            try
            {
                //รุ่นย่อยรถ
                cmbSubModel.DataSource = SlmScr010Biz.GetSubModelData(cmbBrand.SelectedItem.Value, cmbModel.SelectedItem.Value, useWebservice);
                cmbSubModel.DataTextField = "TextField";
                cmbSubModel.DataValueField = "ValueField";
                cmbSubModel.DataBind();
                cmbSubModel.Items.Insert(0, new ListItem("", ""));
            }
            catch
            {
                throw;
            }
        }

        protected void cmbDelegateLead_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbDelegateBranch.SelectedItem.Value != string.Empty && cmbDelegateLead.SelectedItem.Value == string.Empty)
                {
                    vcmbDelegateLead.Text = "กรุณาระบุ Delegate Lead";
                }
                else
                {
                    vcmbDelegateLead.Text = "";
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private LeadData GetLeadDataForWebService()
        {
            LeadData LData = new LeadData();
            LData.TicketId = txtTicketId.Text.Trim();
            //*******************************************kkslm_tr_lead****************************************************
            LData.Name = txtName.Text.Trim();                               //ชื่อ
            LData.TelNo_1 = txtTelNo_1.Text.Trim();                         //หมายเลขโทรศัพท์1
            if (!string.IsNullOrEmpty(cmbCampaignId.SelectedItem.Value))                   //แคมเปญ
                LData.CampaignId = cmbCampaignId.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbOwner.SelectedItem.Value))                        //owner lead
            {
                LData.Owner = cmbOwner.SelectedItem.Value;
                if (SlmScr010Biz.GetStaffIdData(cmbOwner.SelectedItem.Value) != "")
                    LData.StaffId = Convert.ToInt32(SlmScr010Biz.GetStaffIdData(cmbOwner.SelectedItem.Value));
            }
            if (cmbDelegateLead.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateLead.SelectedItem.Value))                     //delegate lead
                LData.Delegate = cmbDelegateLead.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbStatus.SelectedItem.Value))                       //สถานะของLead
                LData.Status = cmbStatus.SelectedItem.Value;
            if (txtAvailableTimeHour.Text.Trim() != "" && txtAvailableTimeMinute.Text.Trim() != "" && txtAvailableTimeSecond.Text.Trim() != "")
                LData.AvailableTime = txtAvailableTimeHour.Text.Trim() + txtAvailableTimeMinute.Text.Trim() + txtAvailableTimeSecond.Text.Trim();             //เวลาที่สะดวก
            if (!string.IsNullOrEmpty(cmbChannelId.SelectedItem.Value))                      //ช่องทาง
                LData.ChannelId = cmbChannelId.SelectedItem.Value;
            else
                LData.ChannelId = LeadInfoBiz.GetChannelId("SLM");
            if (cmbDelegateLead.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateLead.SelectedItem.Value))
            {
                if (txtoldDelegate.Text.Trim() != cmbDelegateLead.SelectedItem.Value)
                {
                    LData.Delegate_Flag = 1;
                }
                else
                    LData.Delegate_Flag = 0;
            }
            else
                LData.Delegate_Flag = 0;
            //*******************************************Product_Info****************************************************
            LData.InterestedProd = txtInterestedProd.Text.Trim();           //Product ที่สนใจ
            LData.LicenseNo = txtLicenseNo.Text.Trim();                     //เลขทะเบียนรถ
            LData.YearOfCar = txtYearOfCar.Text.Trim();                     //ปีรถ
            LData.YearOfCarRegis = txtYearOfCarRegis.Text.Trim();           //ปีที่จดทะเบียนรถยนต์
            if (cmbBrand.SelectedItem.Value != string.Empty)                //ยี่ห้อ
            {
                LData.BrandCode = cmbBrand.SelectedItem.Value;
                //LData.Brand = LeadInfoBiz.GetBrandId(cmbBrand.SelectedItem.Value);
            }
            if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty)                //รุ่น
            {
                LData.ModelFamily = cmbModel.SelectedItem.Value;
                //LData.Model = LeadInfoBiz.GetModelId(cmbBrand.SelectedItem.Value, cmbModel.SelectedItem.Value);
            }
            if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty && cmbSubModel.SelectedItem.Value != string.Empty)      //รุ่นย่อยรถ (รุ่นย่อยรถเก็บ Id เป็น value อยู่แล้ว)
            {
                LData.SubModelCode = cmbSubModel.SelectedItem.Value;
                //LData.Submodel = Convert.ToInt32(cmbSubModel.SelectedItem.Value);
            }
            if (txtDownPayment.Text.Trim() != string.Empty) LData.DownPayment = decimal.Parse(txtDownPayment.Text.Trim().Replace(",", ""));                 //เงินดาวน์
            if (txtDownPercent.Text.Trim() != string.Empty) LData.DownPercent = decimal.Parse(txtDownPercent.Text.Trim());                 //เปอร์เซ็นต์เงินดาวน์
            if (txtCarPrice.Text.Trim() != string.Empty) LData.CarPrice = decimal.Parse(txtCarPrice.Text.Trim().Replace(",", ""));                       //ราคารถยนต์
            if (txtFinanceAmt.Text.Trim() != string.Empty) LData.FinanceAmt = decimal.Parse(txtFinanceAmt.Text.Trim().Replace(",", ""));                   //ยอดจัด Finance
            LData.PaymentTerm = txtPaymentTerm.Text.Trim();                 //ระยะเวลาที่ผ่อนชำระ
            if (!string.IsNullOrEmpty(cmbPaymentType.SelectedItem.Value))          //ประเภทการผ่อนชำระ
            {
                LData.PaymentTypeCode = cmbPaymentType.SelectedItem.Value;
                //LData.PaymentType = cmbPaymentType.SelectedItem.Value;
            }
            if (txtBalloonAmt.Text.Trim() != string.Empty) LData.BalloonAmt = decimal.Parse(txtBalloonAmt.Text.Trim().Replace(",", ""));                   //Balloon Amount
            if (txtBalloonPercent.Text.Trim() != string.Empty) LData.BalloonPercent = decimal.Parse(txtBalloonPercent.Text.Trim());           //Balloon Percent
            if (tdCoverageDate.DateValue.Year != 1)                          //วันที่เริ่มต้นคุ้มครอง
                LData.CoverageDate = tdCoverageDate.DateValue.Year.ToString() + tdCoverageDate.DateValue.ToString("MMdd");
            if (!string.IsNullOrEmpty(cmbProvinceRegis.SelectedItem.Value))        //จังหวัดที่จดทะเบียน
            {
                LData.ProvinceRegisCode = cmbProvinceRegis.SelectedItem.Value;
                //LData.ProvinceRegis = LeadInfoBiz.GetProvinceId(cmbProvinceRegis.SelectedItem.Value);
            }
            if (cmbPlanType.SelectedItem.Value != string.Empty)
            {
                LData.PlanType = cmbPlanType.SelectedItem.Value;                       //ประเภทกรมธรรม์
            }
            LData.Interest = txtInterest.Text.Trim();                       //ประเภทความสนใจ
            if (!string.IsNullOrEmpty(cmbAccType.SelectedItem.Value))              //ประเภทเงินฝาก
            {
                LData.AccTypeCode = cmbAccType.SelectedItem.Value;
                //LData.AccType = Convert.ToInt32("0" + cmbAccType.SelectedItem.Value);
            }
            if (!string.IsNullOrEmpty(cmbAccPromotion.SelectedItem.Value))              //โปรโมชั่นเงินฝากที่สนใจ
            {
                LData.AccPromotionCode = cmbAccPromotion.SelectedItem.Value;
                //LData.AccPromotion = Convert.ToInt32("0" + cmbAccPromotion.SelectedItem.Value);
            }
            LData.AccTerm = txtAccTerm.Text.Trim();                         //ระยะเวลาฝาก Term
            LData.Interest = txtInterest.Text.Trim();                       //อัตราดอกเบี้ยที่สนใจ
            LData.Invest = txtInvest.Text.Trim().Replace(",", "");                          //เงินฝาก/เงินลงทุน
            LData.LoanOd = txtLoanOd.Text.Trim();                           //สินเชื่อ Over Draft
            LData.LoanOdTerm = txtLoanOdTerm.Text.Trim();                   //ระยะเวลา Over Draft
            if (!string.IsNullOrEmpty(cmbEbank.SelectedItem.Value))                //E-Banking
                LData.Ebank = cmbEbank.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbAtm.SelectedItem.Value))                  //ATM
                LData.Atm = cmbAtm.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbCarType.SelectedItem.Value))                  //ATM
                LData.CarType = cmbCarType.SelectedItem.Value;
            //**************************************Cus_Info***************************************************************************
            LData.LastName = txtLastName.Text.Trim();                       //นามสกุล
            LData.Email = txtEmail.Text.Trim();                             //E-mail
            LData.TelNo_2 = txtTelNo2.Text.Trim();                          //หมายเลขโทรศัพท์2
            LData.TelNo_3 = txtTelNo3.Text.Trim();                          //หมายเลขโทรศัพท์3
            LData.Ext_2 = txtExt2.Text.Trim();
            LData.Ext_3 = txtExt3.Text.Trim();
            LData.BuildingName = txtBuildingName.Text.Trim();               //ชื่ออาคาร/หมู่บ้าน
            LData.AddressNo = txtAddressNo.Text.Trim();                     //เลขที่
            LData.Floor = txtFloor.Text.Trim();                             //ชั้น
            LData.Soi = txtSoi.Text.Trim();                                 //ซอย
            LData.Street = txtStreet.Text.Trim();                           //ถนน
            if (cmbProvince.SelectedItem.Value != string.Empty)             //จังหวัด
            {
                LData.ProvinceCode = cmbProvince.SelectedItem.Value;
                //LData.Province = LeadInfoBiz.GetProvinceId(cmbProvince.SelectedItem.Value);
            }
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty)               //เขต/อำเภอ
            {
                LData.AmphurCode = cmbAmphur.SelectedItem.Value;
                //LData.Amphur = LeadInfoBiz.GetAmphurId(cmbProvince.SelectedItem.Value, cmbAmphur.SelectedItem.Value);
            }
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty && cmbTambol.SelectedItem.Value != string.Empty)               //แขวง/ตำบล (ตำบลเก็บ Id เป็น value อยู่แล้ว)
            {
                LData.TambolCode = cmbTambol.SelectedItem.Value;
                //LData.Tambon = Convert.ToInt32(cmbTambol.SelectedItem.Value);
            }

            LData.PostalCode = txtPostalCode.Text.Trim();                   //รหัสไปรษณีย์
            if (!string.IsNullOrEmpty(cmbOccupation.SelectedItem.Value))           //อาชีพ
            {
                LData.OccupationCode = cmbOccupation.SelectedItem.Value;
                //LData.Occupation = Convert.ToInt32(cmbOccupation.SelectedItem.Value);
            }
            if (txtBaseSalary.Text.Trim() != string.Empty) LData.BaseSalary = decimal.Parse(txtBaseSalary.Text.Trim().Replace(",", ""));          //ฐานเงินเดือน
            if (tdBirthdate.DateValue.Year != 1)                            //วันเกิด
                LData.Birthdate = tdBirthdate.DateValue;
            if (!string.IsNullOrEmpty(txtCitizenId.Text.Trim()))            //รหัสบัตรประชาชน
                LData.CitizenId = txtCitizenId.Text.Trim();

            LData.CusCode = txtCusCode.Text.Trim();
            LData.Topic = txtTopic.Text.Trim();                             //เรื่อง
            LData.Detail = txtDetail.Text.Trim();                           //รายละเอียด
            LData.PathLink = txtPathLink.Text.Trim();                       //Path Link
            LData.Company = txtCompany.Text.Trim();                         //บริษัท
            if (!string.IsNullOrEmpty(cmbContactBranch.SelectedItem.Value))        //สาขาที่สะวดติดต่อกลับ
                LData.ContactBranch = cmbContactBranch.SelectedItem.Value;
            //***********************************************Channel Info************************************************************

            if (!string.IsNullOrEmpty(cmbBranch.SelectedItem.Value))           //สาขา
                LData.Branch = cmbBranch.SelectedItem.Value;
            if (!string.IsNullOrEmpty(cmbIsCustomer.SelectedItem.Value))           //เป็นลูกค้าหรือเคยเป็นลูกค้า<br />ของธนาคารหรือไม่
                LData.IsCustomer = cmbIsCustomer.SelectedItem.Value;


            if (cmbDelegateLead.Items.Count > 0)
            {
                if (cmbDelegateLead.SelectedItem.Value != txtoldDelegate.Text.Trim())
                {
                    //***********************************************kkslm_tr_activity************************************************************
                    if (cmbDelegateLead.SelectedItem != null && !string.IsNullOrEmpty(cmbDelegateLead.SelectedItem.Value))
                    {
                        LData.NewDelegate = cmbDelegateLead.SelectedItem.Value;
                    }
                    LData.OldDelegate = txtoldDelegate.Text.Trim();
                    LData.Type = "01";
                    //LData.OldStatus = txtOldStatus.Text.Trim();
                    //LData.NewStatus = cmbStatus.SelectedItem.Value;
                }
            }
            return LData;
        }

        private void LoadLeadDataForWebservice()
        {
            try
            {
                //LeadData lead = new LeadData();
                //lead = SlmScr010Biz.GetLeadData(txtTicketId.Text.Trim());
                //if (lead != null)
                //{
                //    txtName.Text = lead.Name;
                //    txtOldStatus.Text = lead.Status;
                //    if (!string.IsNullOrEmpty(lead.Status))
                //        cmbStatus.SelectedIndex = cmbStatus.Items.IndexOf(cmbStatus.Items.FindByValue(lead.Status));
                //    txtLastName.Text = lead.LastName;
                //    if (!string.IsNullOrEmpty(lead.CampaignId))
                //    {
                //        cmbCampaignId.SelectedIndex = cmbCampaignId.Items.IndexOf(cmbCampaignId.Items.FindByValue(lead.CampaignId));
                //        cmbOwner.DataSource = SlmScr003Biz.GetOwnerListByCampaignId(lead.CampaignId);
                //        cmbOwner.DataTextField = "TextField";
                //        cmbOwner.DataValueField = "ValueField";
                //        cmbOwner.DataBind();
                //        cmbOwner.Items.Insert(0, new ListItem("", ""));
                //    }
                //    txtInterestedProd.Text = lead.InterestedProd;
                //    if (lead.ContactLatestDate != null)
                //        txtContactLatestDate.Text = lead.ContactLatestDate.Value.ToString("dd/MM/") + lead.ContactLatestDate.Value.Year.ToString() + " " + lead.ContactLatestDate.Value.ToString("HH:mm:ss");
                //    if (lead.AssignedDateView != null)
                //        txtAssignDate.Text = lead.AssignedDateView.Value.ToString("dd/MM/") + lead.AssignedDateView.Value.Year.ToString() + " " + lead.AssignedDateView.Value.ToString("HH:mm:ss");
                //    if (lead.ContactFirstDate != null)
                //        txtContactFirstDate.Text = lead.ContactFirstDate.Value.ToString("dd/MM/") + lead.ContactFirstDate.Value.Year.ToString() + " " + lead.ContactFirstDate.Value.ToString("HH:mm:ss");
                //    if (!string.IsNullOrEmpty(lead.Owner))
                //        cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(lead.Owner));
                //    txtoldDelegate.Text = lead.Delegate;

                //    if (!string.IsNullOrEmpty(lead.Delegate))
                //    {
                //        string branchcode = SlmScr010Biz.GetBranchData(lead.Delegate);
                //        if (!string.IsNullOrEmpty(branchcode))
                //        {
                //            cmbDelegateBranch.SelectedIndex = cmbDelegateBranch.Items.IndexOf(cmbDelegateBranch.Items.FindByValue(branchcode));
                //            cmbDelegateBranchSelectedIndexChanged();
                //            cmbDelegateLead.SelectedIndex = cmbDelegateLead.Items.IndexOf(cmbDelegateLead.Items.FindByValue(lead.Delegate));
                //        }
                //    }

                //    txtTopic.Text = lead.Topic;
                //    txtDetail.Text = lead.Detail;
                //    if (!string.IsNullOrEmpty(lead.ChannelId))
                //        cmbChannelId.SelectedIndex = cmbChannelId.Items.IndexOf(cmbChannelId.Items.FindByValue(lead.ChannelId));
                //    if (lead.LeadCreateDate != null)
                //        txtCreateDate.Text = lead.LeadCreateDate.Value.ToString("dd/MM/") + lead.LeadCreateDate.Value.Year.ToString();
                //    if (!string.IsNullOrEmpty(lead.Branch))
                //        cmbBranch.SelectedIndex = cmbBranch.Items.IndexOf(cmbBranch.Items.FindByValue(lead.Branch.ToString()));
                //    if (lead.LeadCreateDate != null)
                //        txtCreateTime.Text = lead.LeadCreateDate.Value.ToString("HH:mm:ss");
                //    txtCompany.Text = lead.Company;
                //    if (!string.IsNullOrEmpty(lead.IsCustomer))
                //        cmbIsCustomer.SelectedIndex = cmbIsCustomer.Items.IndexOf(cmbIsCustomer.Items.FindByValue(lead.IsCustomer));
                //    txtCusCode.Text = lead.CusCode;
                //    txtCitizenId.Text = lead.CitizenId;
                //    if (lead.Birthdate != null)
                //        tdBirthdate.DateValue = lead.Birthdate.Value;
                //    if (lead.OccupationCode != null)
                //        cmbOccupation.SelectedIndex = cmbOccupation.Items.IndexOf(cmbOccupation.Items.FindByValue(lead.OccupationCode.ToString()));
                //    if (lead.BaseSalary != null)
                //        txtBaseSalary.Text = lead.BaseSalary.Value.ToString("#,###.00");
                //    txtTelNo_1.Text = lead.TelNo_1;
                //    if (lead.ContactBranch != null)
                //        cmbContactBranch.SelectedIndex = cmbContactBranch.Items.IndexOf(cmbContactBranch.Items.FindByValue(lead.ContactBranch));
                //    txtTelNo2.Text = lead.TelNo_2;
                //    txtExt2.Text = lead.Ext_2;
                //    if (!string.IsNullOrEmpty(lead.AvailableTime))
                //    {
                //        txtAvailableTimeHour.Text = lead.AvailableTime.Substring(0, 2);
                //        txtAvailableTimeMinute.Text = lead.AvailableTime.Substring(2, 2);
                //        txtAvailableTimeSecond.Text = lead.AvailableTime.Substring(4, 2);
                //    }
                //    txtTelNo3.Text = lead.TelNo_3;
                //    txtExt3.Text = lead.Ext_3;
                //    txtEmail.Text = lead.Email;
                //    txtAddressNo.Text = lead.AddressNo;
                //    txtBuildingName.Text = lead.BuildingName;
                //    txtFloor.Text = lead.Floor;
                //    txtSoi.Text = lead.Soi;
                //    txtStreet.Text = lead.Street;
                //    if (!string.IsNullOrEmpty(lead.ProvinceCode))
                //    {
                //        cmbProvince.SelectedIndex = cmbProvince.Items.IndexOf(cmbProvince.Items.FindByValue(lead.ProvinceCode.ToString()));
                //        ProvinceSeletedIndexChange();
                //    }
                //    if (!string.IsNullOrEmpty(lead.AmphurCode))
                //    {
                //        cmbAmphur.SelectedIndex = cmbAmphur.Items.IndexOf(cmbAmphur.Items.FindByValue(lead.AmphurCode.ToString()));
                //        AmphurSelectIndexChange();
                //    }
                //    if (!string.IsNullOrEmpty(lead.TambolCode))
                //        cmbTambol.SelectedIndex = cmbTambol.Items.IndexOf(cmbTambol.Items.FindByValue(lead.TambolCode.ToString()));
                //    txtPostalCode.Text = lead.PostalCode;
                //    if (!string.IsNullOrEmpty(lead.CarType))
                //        cmbCarType.SelectedIndex = cmbCarType.Items.IndexOf(cmbCarType.Items.FindByValue(lead.CarType));
                //    txtLicenseNo.Text = lead.LicenseNo;
                //    if (!string.IsNullOrEmpty(lead.ProvinceRegisCode))
                //        cmbProvinceRegis.SelectedIndex = cmbProvinceRegis.Items.IndexOf(cmbProvinceRegis.Items.FindByValue(lead.ProvinceRegisCode.ToString()));
                //    txtYearOfCar.Text = lead.YearOfCar;
                //    txtYearOfCarRegis.Text = lead.YearOfCarRegis;
                //    if (!string.IsNullOrEmpty(lead.BrandCode))
                //    {
                //        cmbBrand.SelectedIndex = cmbBrand.Items.IndexOf(cmbBrand.Items.FindByValue(lead.BrandCode.ToString()));
                //        cmbBrandSelectedIndexChanged();
                //    }
                //    if (!string.IsNullOrEmpty(lead.ModelFamily))
                //    {
                //        cmbModel.SelectedIndex = cmbModel.Items.IndexOf(cmbModel.Items.FindByValue(lead.ModelFamily.ToString()));
                //        cmbModelSelectedIndexChanged();
                //    }
                //    if (lead.SubModelCode != null)
                //        cmbSubModel.SelectedIndex = cmbSubModel.Items.IndexOf(cmbSubModel.Items.FindByValue(lead.SubModelCode.ToString()));
                //    if (lead.CarPrice != null)
                //        txtCarPrice.Text = lead.CarPrice.Value.ToString("#,##0.00");
                //    if (lead.DownPayment != null)
                //        txtDownPayment.Text = lead.DownPayment.Value.ToString("#,##0.00");
                //    if (lead.DownPercent != null)
                //        txtDownPercent.Text = lead.DownPercent.Value.ToString();
                //    if (lead.FinanceAmt != null)
                //        txtFinanceAmt.Text = lead.FinanceAmt.Value.ToString("#,##0.00");
                //    txtPaymentTerm.Text = lead.PaymentTerm;
                //    if (!string.IsNullOrEmpty(lead.PaymentTypeCode))
                //        cmbPaymentType.SelectedIndex = cmbPaymentType.Items.IndexOf(cmbPaymentType.Items.FindByValue(lead.PaymentTypeCode));
                //    if (lead.BalloonAmt != null)
                //        txtBalloonAmt.Text = lead.BalloonAmt.Value.ToString("#,##0.00");
                //    if (lead.BalloonPercent != null)
                //        txtBalloonPercent.Text = lead.BalloonPercent.Value.ToString();
                //    if (!string.IsNullOrEmpty(lead.PlanType))
                //        cmbPlanType.SelectedIndex = cmbPlanType.Items.IndexOf(cmbPlanType.Items.FindByValue(lead.PlanType));
                //    if (!string.IsNullOrEmpty(lead.CoverageDate))
                //    {
                //        if (lead.CoverageDate.Trim().Length == 8)
                //        {
                //            DateTime tmpdate = new DateTime(Convert.ToInt32(lead.CoverageDate.Substring(0, 4)), Convert.ToInt32(lead.CoverageDate.Substring(4, 2)), Convert.ToInt32(lead.CoverageDate.Substring(6, 2)));
                //            tdCoverageDate.DateValue = tmpdate;
                //        }
                //    }
                //    if (lead.AccTypeCode != null)
                //        cmbAccType.SelectedIndex = cmbAccType.Items.IndexOf(cmbAccType.Items.FindByValue(lead.AccTypeCode.ToString()));
                //    if (lead.AccPromotionCode != null)
                //        cmbAccPromotion.SelectedIndex = cmbAccPromotion.Items.IndexOf(cmbAccPromotion.Items.FindByValue(lead.AccPromotionCode.ToString()));
                //    txtAccTerm.Text = lead.AccTerm;
                //    txtInterest.Text = lead.Interest;
                //    if (!string.IsNullOrEmpty(lead.Invest))
                //        txtInvest.Text = Convert.ToDecimal(lead.Invest).ToString("#,##0.00");
                //    txtLoanOd.Text = lead.LoanOd;
                //    txtLoanOdTerm.Text = lead.LoanOdTerm;
                //    if (!string.IsNullOrEmpty(lead.Ebank))
                //        cmbEbank.SelectedIndex = cmbEbank.Items.IndexOf(cmbEbank.Items.FindByValue(lead.Ebank));
                //    if (!string.IsNullOrEmpty(lead.Atm))
                //        cmbAtm.SelectedIndex = cmbAtm.Items.IndexOf(cmbAtm.Items.FindByValue(lead.Atm));
                //    txtPathLink.Text = lead.PathLink;
                //    txtCreateBy.Text = lead.LeadCreateBy;
                //    txtAssignFlag.Text = lead.AssignedFlag;

                //    if ((string.IsNullOrEmpty(lead.Owner) == true) && (lead.Status == "00") && ((string.IsNullOrEmpty(lead.AvailableTime)) == false) && (lead.AssignedFlag == "0"))
                //    {
                //        cmbOwner.Enabled = true;
                //    }
                //}
            }
            catch
            {
                throw;
            }
        }

        private bool IsLastestOwnerDelegate()
        {
            try
            {
                List<string> flagList = LeadInfoBiz.GetAssignedFlagAndDelegateFlag(txtTicketId.Text.Trim());
                newassignflag = flagList[0];
                newDelegateFlag = flagList[1];

                if (newassignflag != txtAssignFlag.Text.Trim())
                    return false;
                if (newDelegateFlag != txtDelegateFlag.Text.Trim())
                    return false;

                return true;
            }
            catch
            {
                throw;
            }
        }

        //private string IsOwnerLastest()
        //{
        //    lastest_Owner = SlmScr010Biz.GetOwner(txtTicketId.Text.Trim());
        //    newassignflag = SlmScr010Biz.GetAssignFlag(txtTicketId.Text.Trim());
        //    if (newassignflag != txtAssignFlag.Text.Trim())
        //    {
        //        return "error";
        //    }
        //    else
        //    {
        //        //if (cmbOwner.Items.Count > 0)
        //        //{
        //        //    if (lastest_Owner == null)  // ไม่พบข้อมูล Lead
        //        //    {
        //        //        throw new Exception("ไม่พบข้อมูลผู้มุ่งหวัง กรุณาระบุใหม่");
        //        //    }
        //        //    else if (lastest_Owner == "")  //field slm_owner = null
        //        //    {
        //        //        return "";
        //        //    }
        //        //    else if (lastest_Owner != "")
        //        //    {
        //        //        if (cmbOwner.SelectedItem.Value != lastest_Owner)
        //        //            return "error";
        //        //        else
        //        //            return "";
        //        //    }
        //        //    else
        //        //        return "";
        //        //}
        //        //else
        //            return "";
        //    }
        //}

        protected void txtTelNo2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal result2;
                if (txtTelNo2.Text.Trim() != string.Empty && txtTelNo2.Text.Trim().Length < 9)
                {
                    vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องมีอย่างน้อย 9 หลัก";
                    return;
                }

                if (txtTelNo2.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo2.Text.Trim(), out result2))
                {
                    vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องเป็นตัวเลขเท่านั้น";
                    return;
                }

                if (txtTelNo2.Text.Trim() != string.Empty && txtTelNo2.Text.Trim().StartsWith("0") == false)
                {
                    vtxtTelNo2.Text = "หมายเลขโทรศัพท์ 2 ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                    return;
                }

                vtxtTelNo2.Text = "";
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void txtTelNo3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal result3;
                if (txtTelNo3.Text.Trim() != string.Empty && txtTelNo3.Text.Trim().Length < 9)
                {
                    vtxtTelNo3.Text = "หมายเลขโทรศัพท์ 3 ต้องมีอย่างน้อย 9 หลัก";
                    return;
                }

                if (txtTelNo3.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo3.Text.Trim(), out result3))
                {
                    vtxtTelNo3.Text = "หมายเลขโทรศัพท์ 3 ต้องเป็นตัวเลขเท่านั้น";
                    return;
                }

                if (txtTelNo3.Text.Trim() != string.Empty && txtTelNo3.Text.Trim().StartsWith("0") == false)
                {
                    vtxtTelNo3.Text = "หมายเลขโทรศัพท์ 3 ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                    return;
                }

                vtxtTelNo3.Text = "";
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
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
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        private void cmbOwnerBranchSelectedIndexChanged()
        {
            try
            {
                 //Owner Lead
                List<ControlListData> source = source = StaffBiz.GetStaffAllDataByAccessRight(cmbCampaignId.SelectedItem.Value, cmbOwnerBranch.SelectedItem.Value);
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
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbCardType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCardType.SelectedItem.Value == "")
                {
                    vtxtCardType.Text = "";
                    lblCitizenId.Text = "";
                    txtCitizenId.Text = "";
                    vtxtCitizenId.Text = "";
                    txtCitizenId.Enabled = false;
                }
                else
                {
                    vtxtCardType.Text = "";
                    lblCitizenId.Text = "*";
                    txtCitizenId.Enabled = true;
                    vtxtCitizenId.Text = "";
                    AppUtil.SetCardTypeValidation(cmbCardType.SelectedItem.Value, txtCitizenId);
                    AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #region Backup

        //protected void txtBaseSalary_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string result = AppUtil.SetDecimalFormat(12, txtBaseSalary);
        //        vtxtBaseSalary.Text = "";
        //        if (result == "error")
        //        {
        //            vtxtBaseSalary.Text = "ฐานเงินเดือนไม่เกิน 12 หลัก กรุณาระบุใหม่";
        //        }
        //        else
        //        {
        //            vtxtBaseSalary.Text = "";
        //            txtBaseSalary.Text = result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //}

        //protected void txtCarPrice_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string result = AppUtil.SetDecimalFormat(10, txtCarPrice);
        //        vtxtCarPrice.Text = "";
        //        if (result == "error")
        //        {
        //            vtxtCarPrice.Text = "ราคารถยนต์ไม่เกิน 10 หลัก กรุณาระบุใหม่";
        //        }
        //        else
        //        {
        //            vtxtCarPrice.Text = "";
        //            txtCarPrice.Text = result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //}

        //protected void txtDownPayment_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string result = AppUtil.SetDecimalFormat(10, txtDownPayment);
        //        vtxtDownPayment.Text = "";
        //        if (result == "error")
        //        {
        //            vtxtDownPayment.Text = "เงินดาวน์ไม่เกิน 10 หลัก กรุณาระบุใหม่";
        //        }
        //        else
        //        {
        //            vtxtDownPayment.Text = "";
        //            txtDownPayment.Text = result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //}

        //protected void txtDownPercent_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string result = AppUtil.SetPercentFormat(3, txtDownPercent);
        //        vtxtDownPercent.Text = "";
        //        if (result == "error")
        //        {
        //            vtxtDownPercent.Text = "เปอร์เซ็นต์เงินดาวน์ไม่เกิน 3 หลัก กรุณาระบุใหม่";
        //        }
        //        else if (result == "error100")
        //        {
        //            vtxtDownPercent.Text = "เปอร์เซ็นต์เงินดาวน์เกิน 100 กรุณาระบุใหม่";
        //        }
        //        else
        //        {
        //            vtxtDownPercent.Text = "";
        //            txtDownPercent.Text = result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //}

        //protected void txtFinanceAmt_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string result = AppUtil.SetDecimalFormat(10, txtFinanceAmt);
        //        vtxtFinanceAmt.Text = "";
        //        if (result == "error")
        //        {
        //            vtxtFinanceAmt.Text = "ยอดจัด Financeไม่เกิน 10 หลัก กรุณาระบุใหม่";
        //        }
        //        else
        //        {
        //            vtxtFinanceAmt.Text = "";
        //            txtFinanceAmt.Text = result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //}

        //protected void txtBalloonAmt_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string result = AppUtil.SetDecimalFormat(10, txtBalloonAmt);
        //        vtxtBalloonAmt.Text = "";
        //        if (result == "error")
        //        {
        //            vtxtBalloonAmt.Text = "Balloon Amount ไม่เกิน 10 หลัก กรุณาระบุใหม่";
        //        }
        //        else
        //        {
        //            vtxtBalloonAmt.Text = "";
        //            txtBalloonAmt.Text = result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //}

        //protected void txtBalloonPercent_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string result = AppUtil.SetPercentFormat(3, txtBalloonPercent);
        //        vtxtBalloonPercent.Text = "";
        //        if (result == "error")
        //        {
        //            vtxtBalloonPercent.Text = "Balloon Percentไม่เกิน 3 หลัก กรุณาระบุใหม่";
        //        }
        //        else if (result == "error100")
        //        {
        //            vtxtBalloonPercent.Text = "Balloon Percentเกิน 100 กรุณาระบุใหม่";
        //        }
        //        else
        //        {
        //            vtxtBalloonPercent.Text = "";
        //            txtBalloonPercent.Text = result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //}

        #endregion
    }
}