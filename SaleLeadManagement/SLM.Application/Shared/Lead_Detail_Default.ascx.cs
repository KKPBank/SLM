using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net;
using log4net;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Application.Utilities;
using SLM.Biz;

namespace SLM.Application.Shared
{
    public partial class Lead_Detail_Default : Lead_Detail_Master
    {
        private int baseSalaryMaxLength = 12;
        private int defaultMaxLength = 10;
        private int percentMaxLength = 3;
        private bool useWebservice = Convert.ToBoolean(ConfigurationManager.AppSettings["UseWebservice"]);
        private static readonly ILog _log = LogManager.GetLogger(typeof(LeadInfo));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.SetIntTextBox(txtCitizenId);
            AppUtil.SetIntTextBox(txtTelNo3);
            //AppUtil.SetIntTextBox(txtYearOfCar);
            AppUtil.SetIntTextBox(txtYearOfCarRegis);
            AppUtil.SetIntTextBox(txtAvailableTimeHour);
            AppUtil.SetIntTextBox(txtAvailableTimeMinute);
            AppUtil.SetIntTextBox(txtAvailableTimeSecond);
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

            InitialControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetScript();
            }

            if (string.IsNullOrEmpty(cmbCardType.SelectedValue))
            {
                var matchCodes = new[] { "18", "24", "29" };

                var subscriptionTypeCode = Request["subscriptionTypeCode"];

                if (!string.IsNullOrEmpty(subscriptionTypeCode) && matchCodes.Contains(subscriptionTypeCode))
                {
                    switch (subscriptionTypeCode)
                    {
                        case "18":
                            cmbCardType.SelectedValue = "1";
                            break;
                        case "24":
                            cmbCardType.SelectedValue = "2";
                            break;
                        case "29":
                            cmbCardType.SelectedValue = "3";
                            break;
                    }

                    // Notify Event for Enable TextBox
                    cmbCardType_SelectedIndexChanged(cmbCardType, EventArgs.Empty);

                    if (string.IsNullOrEmpty(txtCitizenId.Text.Trim()))
                    {
                        var subscriptionId = Request["subscriptionId"];
                        if (!string.IsNullOrEmpty(subscriptionId))
                        {
                            txtCitizenId.Text = subscriptionId;
                        }
                    }
                }
            }
        }

        private void InitialControl()
        {
            //แคมเปญ
            //cmbCampaignId.DataSource = SlmScr003Biz.GetCampaignData();
            //cmbCampaignId.DataTextField = "TextField";
            //cmbCampaignId.DataValueField = "ValueField";
            //cmbCampaignId.DataBind();
            //cmbCampaignId.Items.Insert(0, new ListItem("", ""));

            //owner lead
            //cmbOwner.DataSource = SlmScr003Biz.GetOwnerList(HttpContext.Current.User.Identity.Name);
            //cmbOwner.DataTextField = "TextField";
            //cmbOwner.DataValueField = "ValueField";
            //cmbOwner.DataBind();
            //cmbOwner.Items.Insert(0, new ListItem("", ""));

            //Owner Branch
            //cmbOwnerBranch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
            //cmbOwnerBranch.DataTextField = "TextField";
            //cmbOwnerBranch.DataValueField = "ValueField";
            //cmbOwnerBranch.DataBind();
            //cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));

            //ช่องทาง
            //cmbChannelId.DataSource = SlmScr003Biz.GetChannelData();
            //cmbChannelId.DataTextField = "TextField";
            //cmbChannelId.DataValueField = "ValueField";
            //cmbChannelId.DataBind();
            //cmbChannelId.Items.Insert(0, new ListItem("", ""));

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
            cmbProvince.DataSource = SlmScr010Biz.GetProvinceDataNew();
            cmbProvince.DataTextField = "TextField";
            cmbProvince.DataValueField = "ValueField";
            cmbProvince.DataBind();
            cmbProvince.Items.Insert(0, new ListItem("", ""));

            //จังหวัดที่จดทะเบียน
            cmbProvinceRegis.DataSource = SlmScr010Biz.GetProvinceDataNew();
            cmbProvinceRegis.DataTextField = "TextField";
            cmbProvinceRegis.DataValueField = "ValueField";
            cmbProvinceRegis.DataBind();
            cmbProvinceRegis.Items.Insert(0, new ListItem("", ""));

            //ยี่ห้อรถ
            //cmbBrand.DataSource = SlmScr010Biz.GetBrandData();
            //cmbBrand.DataTextField = "TextField";
            //cmbBrand.DataValueField = "ValueField";
            //cmbBrand.DataBind();
            //cmbBrand.Items.Insert(0, new ListItem("", ""));

            // ยี่ห้อ
            BuildCombo(cmbBrand, SlmScr010Biz.GetBrandDataNew());

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

            //ประเทศ
            cmbCountry.DataSource = CountryBiz.GetCountryList();
            cmbCountry.DataTextField = "TextField";
            cmbCountry.DataValueField = "ValueField";
            cmbCountry.DataBind();
            cmbCountry.Items.Insert(0, new ListItem("", ""));
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
        private void BuildCombo(DropDownList cmb, List<ControlListData> lst)
        {
            cmb.DataSource = lst;
            cmb.DataTextField = "TextField";
            cmb.DataValueField = "ValueField";
            cmb.DataBind();
            cmb.Items.Insert(0, new ListItem("", ""));
        }


        protected void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ProvinceSelectedIndexChanged();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void cmbAmphur_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                AmphurSelectedIndexChanged();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void cmbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbBrandSelectedIndexChanged();
                cmbModelSelectedIndexChanged();
                cmbYearGroupSelectedIndexChanged();
                SetDefaultCarType();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbModelSelectedIndexChanged();
                cmbYearGroupSelectedIndexChanged();
               // SetDefaultCarType();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbYearGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbYearGroupSelectedIndexChanged();
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
                lblCountry.Text = "";
                if (cmbCardType.SelectedItem.Value == "")
                {
                    lblCitizenId.Text = "";
                    txtCitizenId.Text = "";
                    vtxtCitizenId.Text = "";
                    lblCountry.Text = "";
                    txtCitizenId.Enabled = false;
                    cmbCountry.Enabled = false;
                    cmbCountry.SelectedValue = "";
                }
                else
                {
                    lblCitizenId.Text = "*";
                    txtCitizenId.Enabled = true;
                    cmbCountry.Enabled = true;
                    vtxtCitizenId.Text = "";
                    lblCountry.Text = "*";
                    AppUtil.SetCardTypeValidation(cmbCardType.SelectedItem.Value, txtCitizenId);
                    AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId);

                    if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Person)
                    {
                        //ถ้าเลือกบัตรประชาชน ให้ Default ประเทศไทย ตาม Config
                        if (cmbCountry.SelectedValue == "")
                            cmbCountry.SelectedValue = AppConstant.CBSLeadThaiCountryId.ToString();
                    }

                    ////ถ้าเลือกชาวต่างชาติ ให้บังคับให้เลือกประเทศด้วย
                    //if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Foreigner)
                    //{
                    //    lblCountry.Text = "*";
                    //    cmbCountry.Enabled = true;
                    //    cmbCountry.SelectedValue = "";
                    //}
                    //else
                    //{
                    //    //ถ้าเลือกบุคคลธรรม หรือนิติบุคคล ให้ Default ประเทศไทย ตาม Config
                    //    cmbCountry.SelectedValue = AppConstant.CBSLeadThaiCountryId.ToString();
                    //}
                }
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
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (SLMUtil.ValidateEmail(txtEmail.Text) == false && txtEmail.Text.Trim() != "")
                    vtxtEmail.Text = "กรุณาระบุ E-mail ให้ถูกต้อง";
                else
                    vtxtEmail.Text = "";
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
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


        private void ProvinceSelectedIndexChanged()
        {
            //เขต/อำเภอ
            cmbAmphur.DataSource = SlmScr010Biz.GetAmphurDataNew(SLMUtil.SafeInt(cmbProvince.SelectedItem.Value));
            cmbAmphur.DataTextField = "TextField";
            cmbAmphur.DataValueField = "ValueField";
            cmbAmphur.DataBind();
            cmbAmphur.Items.Insert(0, new ListItem("", ""));

            //แขวง/ตำบล
            cmbTambol.DataSource = SlmScr010Biz.GetTambolDataNew(SLMUtil.SafeInt(cmbAmphur.SelectedValue));
            cmbTambol.DataTextField = "TextField";
            cmbTambol.DataValueField = "ValueField";
            cmbTambol.DataBind();
            cmbTambol.Items.Insert(0, new ListItem("", ""));
        }
        private void AmphurSelectedIndexChanged()
        {
            //แขวง/ตำบล
            cmbTambol.DataSource = SlmScr010Biz.GetTambolDataNew(SLMUtil.SafeInt(cmbAmphur.SelectedItem.Value));
            cmbTambol.DataTextField = "TextField";
            cmbTambol.DataValueField = "ValueField";
            cmbTambol.DataBind();
            cmbTambol.Items.Insert(0, new ListItem("", ""));
        }
        private void cmbBrandSelectedIndexChanged()
        {
            ////รุ่นรถ
            //cmbModel.DataSource = SlmScr010Biz.GetModelData(cmbBrand.SelectedItem.Value);
            //cmbModel.DataTextField = "TextField";
            //cmbModel.DataValueField = "ValueField";
            //cmbModel.DataBind();
            //cmbModel.Items.Insert(0, new ListItem("", ""));

            ////รุ่นย่อยรถ
            //cmbSubModel.DataSource = SlmScr010Biz.GetSubModelData(cmbBrand.SelectedItem.Value, cmbModel.SelectedItem.Value, useWebservice);
            //cmbSubModel.DataTextField = "TextField";
            //cmbSubModel.DataValueField = "ValueField";
            //cmbSubModel.DataBind();
            //cmbSubModel.Items.Insert(0, new ListItem("", ""));
            BuildCombo(cmbModel, SlmScr010Biz.GetModelDataNew(cmbBrand.SelectedValue));
        }
        private void cmbModelSelectedIndexChanged()
        {
            //รุ่นย่อยรถ
            //cmbSubModel.DataSource = SlmScr010Biz.GetSubModelData(cmbBrand.SelectedItem.Value, cmbModel.SelectedItem.Value, useWebservice);
            //cmbSubModel.DataTextField = "TextField";
            //cmbSubModel.DataValueField = "ValueField";
            //cmbSubModel.DataBind();
            //cmbSubModel.Items.Insert(0, new ListItem("", ""));
            BuildCombo(cmbYearGroup, SlmScr010Biz.GetModelYearNew(cmbBrand.SelectedValue, cmbModel.SelectedValue));
        }
        private void cmbYearGroupSelectedIndexChanged()
        {
            BuildCombo(cmbSubModel, SlmScr010Biz.GetSubModelDataNew(cmbBrand.SelectedValue, cmbModel.SelectedValue, SLMUtil.SafeInt(cmbYearGroup.SelectedValue)));
        }

        protected void cmbCountry_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void SetDefaultCarType()
        {
            var itm = SlmScr010Biz.GetInsuranceCarTypeDataByModel(cmbBrand.SelectedValue, cmbModel.SelectedValue).FirstOrDefault();
            if (itm != null)
                AppUtil.SetComboValue(cmbCarType, itm.ValueField);
        }

        private void InitializeLeadData(string ticketId)
        {
            try
            {
                LeadData lead = SlmScr010Biz.GetLeadData(ticketId);
                if (lead != null)
                {
                    //txtName.Text = lead.Name;
                    //txtLastName.Text = lead.LastName;

                    //if (!string.IsNullOrEmpty(lead.CampaignId))
                    //{
                    //    cmbCampaignId.SelectedIndex = cmbCampaignId.Items.IndexOf(cmbCampaignId.Items.FindByValue(lead.CampaignId));
                    //    cmbOwnerBranch.DataSource = SlmScr010Biz.GetBranchListByAccessRight(SLMConstant.Branch.Active, cmbCampaignId.SelectedItem.Value);
                    //    cmbOwnerBranch.DataTextField = "TextField";
                    //    cmbOwnerBranch.DataValueField = "ValueField";
                    //    cmbOwnerBranch.DataBind();
                    //    cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));
                    //}

                    txtInterestedProd.Text = lead.InterestedProd;

                    //if (!string.IsNullOrEmpty(lead.Owner_Branch))
                    //{
                    //    ListItem item = cmbOwnerBranch.Items.FindByValue(lead.Owner_Branch);
                    //    if (item != null)
                    //        cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(item);
                    //    else
                    //    {
                    //        //check ว่ามีการกำหนด Brach ใน Table kkslm_ms_Access_Right ไหม ถ้ามีจะเท่ากับเป็น Branch ที่ถูกปิด ถ้าไม่มีแปลว่าไม่มีการเซตการมองเห็น
                    //        if (SlmScr011Biz.CheckBranchAccessRightExist(SLMConstant.Branch.All, cmbCampaignId.SelectedItem.Value, lead.Owner_Branch))
                    //        {
                    //            //Branch ที่ถูกปิด
                    //            string branchName = BranchBiz.GetBranchName(lead.Owner_Branch);
                    //            if (!string.IsNullOrEmpty(branchName))
                    //            {
                    //                cmbOwnerBranch.Items.Insert(1, new ListItem(branchName, lead.Owner_Branch));
                    //                cmbOwnerBranch.SelectedIndex = 1;
                    //            }
                    //        }
                    //    }

                    //    cmbOwnerBranchSelectedIndexChanged();   //Bind Combo Owner
                    //}

                    //if (!string.IsNullOrEmpty(lead.Owner))
                    //{
                    //    //comment By Nang 2015-04-18
                    //    //var source = SlmScr010Biz.GetOwnerListByCampaignIdAndBranch(cmbCampaignId.SelectedItem.Value, cmbOwnerBranch.SelectedItem.Value);

                    //    cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(lead.Owner));
                    //}

                    txtTopic.Text = lead.Topic;
                    txtDetail.Text = lead.Detail;

                    //if (!string.IsNullOrEmpty(lead.ChannelId))
                    //    cmbChannelId.SelectedIndex = cmbChannelId.Items.IndexOf(cmbChannelId.Items.FindByValue(lead.ChannelId));

                    if (!string.IsNullOrEmpty(lead.Branch))
                    {
                        ListItem item = cmbBranch.Items.FindByValue(lead.Branch);
                        if (item != null)
                            cmbBranch.SelectedIndex = cmbBranch.Items.IndexOf(item);
                        else
                        {
                            //Branch ที่ถูกปิด
                            //string branchName = BranchBiz.GetBranchName(lead.Branch);
                            //if (!string.IsNullOrEmpty(branchName))
                            //{
                            //    cmbBranch.Items.Insert(1, new ListItem(branchName, lead.Branch));
                            //    cmbBranch.SelectedIndex = 1;
                            //}
                        }
                    }

                    txtCompany.Text = lead.Company;

                    if (!string.IsNullOrEmpty(lead.IsCustomer))
                        cmbIsCustomer.SelectedIndex = cmbIsCustomer.Items.IndexOf(cmbIsCustomer.Items.FindByValue(lead.IsCustomer));

                    txtCusCode.Text = lead.CusCode;
                    txtContractNoRefer.Text = lead.ContractNoRefer;

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

                    if (lead.Birthdate != null)
                        tdBirthdate.DateValue = lead.Birthdate.Value;
                    if (lead.Occupation != null)
                        cmbOccupation.SelectedIndex = cmbOccupation.Items.IndexOf(cmbOccupation.Items.FindByValue(lead.Occupation.ToString()));
                    if (lead.BaseSalary != null)
                        txtBaseSalary.Text = lead.BaseSalary.Value.ToString("#,###.00");

                    //txtTelNo_1.Text = lead.TelNo_1;

                    if (!string.IsNullOrEmpty(lead.ContactBranch))
                    {
                        ListItem item = cmbContactBranch.Items.FindByValue(lead.ContactBranch);
                        if (item != null)
                            cmbContactBranch.SelectedIndex = cmbContactBranch.Items.IndexOf(item);
                        else
                        {
                            //Branch ที่ถูกปิด
                            //string branchName = BranchBiz.GetBranchName(lead.ContactBranch);
                            //if (!string.IsNullOrEmpty(branchName))
                            //{
                            //    cmbContactBranch.Items.Insert(1, new ListItem(branchName, lead.ContactBranch));
                            //    cmbContactBranch.SelectedIndex = 1;
                            //}
                        }
                    }

                    //txtTelNo2.Text = lead.TelNo_2;
                    //txtExt2.Text = lead.Ext_2;

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

                    //if (!string.IsNullOrEmpty(lead.ProvinceCode))
                    //{
                    //    cmbProvince.SelectedIndex = cmbProvince.Items.IndexOf(cmbProvince.Items.FindByValue(lead.ProvinceCode.ToString()));
                    //    ProvinceSelectedIndexChanged();
                    //}
                    //if (!string.IsNullOrEmpty(lead.AmphurCode))
                    //{
                    //    cmbAmphur.SelectedIndex = cmbAmphur.Items.IndexOf(cmbAmphur.Items.FindByValue(lead.AmphurCode.ToString()));
                    //    AmphurSelectedIndexChanged();
                    //}
                    //if (lead.Tambon != null)
                    //    cmbTambol.SelectedIndex = cmbTambol.Items.IndexOf(cmbTambol.Items.FindByValue(lead.Tambon.ToString()));

                    if (lead.Province.HasValue)
                    {
                        cmbProvince.SelectedValue = lead.Province.Value.ToString();
                        ProvinceSelectedIndexChanged();
                    }
                    if (lead.Amphur.HasValue)
                    {
                        cmbAmphur.SelectedValue = lead.Amphur.Value.ToString();
                        AmphurSelectedIndexChanged();
                    }
                    if (lead.Tambon.HasValue)
                    {
                        cmbTambol.SelectedValue = lead.Tambon.Value.ToString();
                    }                        

                    txtPostalCode.Text = lead.PostalCode;

                    if (!string.IsNullOrEmpty(lead.CarType))
                        cmbCarType.SelectedIndex = cmbCarType.Items.IndexOf(cmbCarType.Items.FindByValue(lead.CarType));

                    txtLicenseNo.Text = lead.LicenseNo;

                    if (!string.IsNullOrEmpty(lead.ProvinceRegisCode))
                        cmbProvinceRegis.SelectedIndex = cmbProvinceRegis.Items.IndexOf(cmbProvinceRegis.Items.FindByValue(lead.ProvinceRegisCode.ToString()));

                    //txtYearOfCar.Text = lead.YearOfCar;
                    cmbYearGroup.SelectedIndex = cmbYearGroup.Items.IndexOf(cmbYearGroup.Items.FindByValue(lead.YearOfCar));
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

                    if (!string.IsNullOrEmpty(lead.Invest))
                        txtInvest.Text = Convert.ToDecimal(lead.Invest).ToString("#,##0.00");

                    txtLoanOd.Text = lead.LoanOd;
                    txtLoanOdTerm.Text = lead.LoanOdTerm;

                    if (!string.IsNullOrEmpty(lead.Ebank))
                        cmbEbank.SelectedIndex = cmbEbank.Items.IndexOf(cmbEbank.Items.FindByValue(lead.Ebank));
                    if (!string.IsNullOrEmpty(lead.Atm))
                        cmbAtm.SelectedIndex = cmbAtm.Items.IndexOf(cmbAtm.Items.FindByValue(lead.Atm));

                    txtPathLink.Text = lead.PathLink;
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

            //*******************************************kkslm_tr_lead****************************************************
            LData.TicketId = CommonData.TicketID;
            LData.TitleId = CommonData.TitleID != 0 ? (int?)CommonData.TitleID : null;
            LData.Name = CommonData.FName.Trim();                               //ชื่อ
            LData.TelNo_1 = CommonData.Phone1.Trim();                         //หมายเลขโทรศัพท์1
            if (CommonData.CampaignID != string.Empty)                   //แคมเปญ
                LData.CampaignId = CommonData.CampaignID;

            List<ProductData> prodList = SlmScr003Biz.GetProductCampaignData(LData.CampaignId);
            if (prodList.Count > 0)
            {
                LData.ProductGroupId = prodList[0].ProductGroupId;
                LData.ProductId = prodList[0].ProductId;
                LData.ProductName = prodList[0].ProductName;
                LData.HasAdamsUrl = prodList[0].HasAdamsUrl;
            }

            LData.Owner_Branch = CommonData.Branch;

            if (CommonData.Owner != "")                 //owner lead
            {
                LData.Owner = CommonData.Owner;
                LData.NewOwner = CommonData.Owner;
                LData.NewOwner2 = CommonData.Owner;
                StaffData StaffData = SlmScr010Biz.GetStaffData(CommonData.Owner);
                if (StaffData != null)
                {
                    LData.StaffId = Convert.ToInt32(StaffData.StaffId);
                    //LData.Owner_Branch = StaffData.BranchCode;
                }
            }

            LData.slmOldOwner = CommonData.OldOwner;
            LData.OldOwner = CommonData.OldOwner;
            LData.OldOwner2 = CommonData.OldOwner;
            LData.Type2 = CommonData.Type2;
            LData.Delegate_Branch = CommonData.DelegateBranch;
            LData.Delegate = CommonData.DelegateLead;

            if (!string.IsNullOrEmpty(CommonData.DelegateFlag))
            {
                LData.Delegate_Flag = Convert.ToDecimal(CommonData.DelegateFlag);  //Add
                if (LData.Delegate_Flag == 1)
                {
                    LData.Delegate_Branch = CommonData.DelegateBranch;
                    LData.Delegate = CommonData.DelegateLead;
                    LData.NewDelegate = CommonData.DelegateLead;
                    LData.OldDelegate = CommonData.OldDelegateLead;
                    LData.Type = CommonData.Type;
                }
            }
            else
            {
                LData.Delegate_Flag = 0;
            }

            if (string.IsNullOrEmpty(LData.Delegate))
            {
                LData.Delegate_Flag = 0;
            }

            LData.Status = CommonData.Status;


            if (txtAvailableTimeHour.Text.Trim() != "" && txtAvailableTimeMinute.Text.Trim() != "" && txtAvailableTimeSecond.Text.Trim() != "")
                LData.AvailableTime = txtAvailableTimeHour.Text.Trim() + txtAvailableTimeMinute.Text.Trim() + txtAvailableTimeSecond.Text.Trim();             //เวลาที่สะดวก
            if (CommonData.ChannelID != string.Empty)                      //ช่องทาง
                LData.ChannelId = CommonData.ChannelID;
            else
                LData.ChannelId = LeadInfoBiz.GetChannelId("SLM");

            LData.StatusDate = DateTime.Now;

            //*******************************************Product_Info****************************************************
            if (cmbCarType.SelectedItem.Value != string.Empty)              //ประเภทความสนใจ(รถใหม่/รถเก่า)
                LData.CarType = cmbCarType.SelectedItem.Value;
            LData.InterestedProd = txtInterestedProd.Text.Trim();           //Product ที่สนใจ
            LData.LicenseNo = txtLicenseNo.Text.Trim();                     //เลขทะเบียนรถ
            LData.YearOfCar = cmbYearGroup.SelectedValue;// txtYearOfCar.Text.Trim();                     //ปีรถ
            LData.YearOfCarRegis = txtYearOfCarRegis.Text.Trim();           //ปีที่จดทะเบียนรถยนต์
            //if (cmbBrand.SelectedItem.Value != string.Empty)                //ยี่ห้อ
            //{
            //    //LData.Brand = LeadInfoBiz.GetBrandId(cmbBrand.SelectedItem.Value);
            //    LData.slm_RedbookBrandCode = cmbBrand.SelectedValue;
            //}
            //if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty)                //รุ่น
            //{
            //    //LData.Model = LeadInfoBiz.GetModelId(cmbBrand.SelectedItem.Value, cmbModel.SelectedItem.Value);
            //    LData.slm_RedbookModelCode = cmbModel.SelectedValue;
            //}
            //if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty && cmbSubModel.SelectedItem.Value != string.Empty)             //รุ่นย่อยรถ (รุ่นย่อยรถเก็บ Id เป็น value อยู่แล้ว)
            //    LData.SubModelCode = cmbSubModel.SelectedValue;
            if (cmbBrand.SelectedValue != "") LData.slm_RedbookBrandCode = cmbBrand.SelectedValue;
            if (cmbModel.SelectedValue != "") LData.slm_RedbookModelCode = cmbModel.SelectedValue;
            if (cmbYearGroup.SelectedValue != "") LData.slm_RedbookYearGroup = SLMUtil.SafeInt(cmbYearGroup.SelectedValue);
            if (cmbSubModel.SelectedValue != "") LData.slm_RedbookKKKey = cmbSubModel.SelectedValue;
                //LData.Submodel = Convert.ToInt32(cmbSubModel.SelectedItem.Value);
            if (txtDownPayment.Text.Trim() != string.Empty) LData.DownPayment = decimal.Parse(txtDownPayment.Text.Trim().Replace(",", ""));                 //เงินดาวน์
            if (txtDownPercent.Text.Trim() != string.Empty) LData.DownPercent = decimal.Parse(txtDownPercent.Text.Trim());                 //เปอร์เซ็นต์เงินดาวน์
            if (txtCarPrice.Text.Trim() != string.Empty) LData.CarPrice = decimal.Parse(txtCarPrice.Text.Trim().Replace(",", ""));                       //ราคารถยนต์
            if (txtFinanceAmt.Text.Trim() != string.Empty) LData.FinanceAmt = decimal.Parse(txtFinanceAmt.Text.Trim().Replace(",", ""));                   //ยอดจัด Finance
            LData.PaymentTerm = txtPaymentTerm.Text.Trim();                 //ระยะเวลาที่ผ่อนชำระ
            if (cmbPaymentType.SelectedItem.Value != string.Empty)          //ประเภทการผ่อนชำระ
                LData.PaymentType = cmbPaymentType.SelectedItem.Value;
            if (txtBalloonAmt.Text.Trim() != string.Empty) LData.BalloonAmt = decimal.Parse(txtBalloonAmt.Text.Trim().Replace(",", ""));                   //Balloon Amount
            if (txtBalloonPercent.Text.Trim() != string.Empty) LData.BalloonPercent = decimal.Parse(txtBalloonPercent.Text.Trim());           //Balloon Percent
            if (tdCoverageDate.DateValue.Year != 1)                          //วันที่เริ่มต้นคุ้มครอง
                LData.CoverageDate = tdCoverageDate.DateValue.Year.ToString() + tdCoverageDate.DateValue.ToString("MMdd");
            if (cmbProvinceRegis.SelectedItem.Value != string.Empty)        //จังหวัดที่จดทะเบียน
            {
                //LData.ProvinceRegis = LeadInfoBiz.GetProvinceId(cmbProvinceRegis.SelectedItem.Value);
                LData.ProvinceRegis = Convert.ToInt32(cmbProvinceRegis.SelectedItem.Value);
            }
            if (cmbPlanType.SelectedItem.Value != string.Empty)
                LData.PlanType = cmbPlanType.SelectedItem.Value;                       //ประเภทกรมธรรม์
            LData.Interest = txtInterest.Text.Trim();                       //ประเภทความสนใจ
            if (cmbAccType.SelectedItem.Value != string.Empty)              //ประเภทเงินฝาก
                LData.AccType = Convert.ToInt32("0" + cmbAccType.SelectedItem.Value);
            if (cmbAccPromotion.SelectedItem.Value != string.Empty)              //โปรโมชั่นเงินฝากที่สนใจ
                LData.AccPromotion = Convert.ToInt32("0" + cmbAccPromotion.SelectedItem.Value);
            LData.AccTerm = txtAccTerm.Text.Trim();                         //ระยะเวลาฝาก Term
            LData.Interest = txtInterest.Text.Trim();                       //อัตราดอกเบี้ยที่สนใจ
            LData.Invest = txtInvest.Text.Trim().Replace(",", "");           //เงินฝาก/เงินลงทุน
            LData.LoanOd = txtLoanOd.Text.Trim();                           //สินเชื่อ Over Draft
            LData.LoanOdTerm = txtLoanOdTerm.Text.Trim();                   //ระยะเวลา Over Draft
            if (cmbEbank.SelectedItem.Value != string.Empty)                //E-Banking
                LData.Ebank = cmbEbank.SelectedItem.Value;
            if (cmbAtm.SelectedItem.Value != string.Empty)                  //ATM
                LData.Atm = cmbAtm.SelectedItem.Value;

            //***************************************Cus_Info***************************************************************************
            LData.LastName = CommonData.LName.Trim();                       //นามสกุล
            LData.Email = txtEmail.Text.Trim();                             //E-mail
            LData.TelNo_2 = CommonData.Phone2.Trim();                          //หมายเลขโทรศัพท์2
            LData.TelNo_3 = txtTelNo3.Text.Trim();                          //หมายเลขโทรศัพท์3
            //LData.Ext_2 = txtExt2.Text.Trim();
            LData.Ext_3 = txtExt3.Text.Trim();
            LData.BuildingName = txtBuildingName.Text.Trim();               //ชื่ออาคาร/หมู่บ้าน
            LData.AddressNo = txtAddressNo.Text.Trim();                     //เลขที่
            LData.Floor = txtFloor.Text.Trim();                             //ชั้น
            LData.Soi = txtSoi.Text.Trim();                                 //ซอย
            LData.Street = txtStreet.Text.Trim();                           //ถนน

            if (cmbProvince.SelectedItem.Value != string.Empty)             //จังหวัด
            {
                //LData.Province = LeadInfoBiz.GetProvinceId(cmbProvince.SelectedItem.Value);
                LData.Province = Convert.ToInt32(cmbProvince.SelectedItem.Value);
            }
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty)               //เขต/อำเภอ
            {
                //LData.Amphur = LeadInfoBiz.GetAmphurId(cmbProvince.SelectedItem.Value, cmbAmphur.SelectedItem.Value);
                LData.Amphur = Convert.ToInt32(cmbAmphur.SelectedItem.Value);
            }
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty && cmbTambol.SelectedItem.Value != string.Empty)
            {   
                //แขวง/ตำบล (ตำบลเก็บ Id เป็น value อยู่แล้ว)
                LData.Tambon = Convert.ToInt32(cmbTambol.SelectedItem.Value);
            }
            LData.PostalCode = txtPostalCode.Text.Trim();                   //รหัสไปรษณีย์
            if (cmbCountry.SelectedValue != string.Empty)
                LData.CountryId = Convert.ToInt32(cmbCountry.SelectedValue);                 //ประเทศ

            if (cmbOccupation.SelectedItem.Value != string.Empty)           //อาชีพ
                LData.Occupation = Convert.ToInt32(cmbOccupation.SelectedItem.Value);
            if (txtBaseSalary.Text.Trim() != string.Empty) LData.BaseSalary = decimal.Parse(txtBaseSalary.Text.Trim().Replace(",", ""));          //ฐานเงินเดือน
            if (tdBirthdate.DateValue.Year != 1)                            //วันเกิด
                LData.Birthdate = tdBirthdate.DateValue;

            if (cmbCardType.Items.Count > 0 && cmbCardType.SelectedItem.Value != "")
                LData.CardType = Convert.ToInt32(cmbCardType.SelectedItem.Value);       //ประเภทบุคคล

            if (!string.IsNullOrEmpty(txtCitizenId.Text.Trim()))
                LData.CitizenId = txtCitizenId.Text.Trim();                     //เลขที่บัตร

            LData.CusCode = txtCusCode.Text.Trim();
            LData.Topic = txtTopic.Text.Trim();                             //เรื่อง
            LData.Detail = txtDetail.Text.Trim();                           //รายละเอียด
            LData.PathLink = txtPathLink.Text.Trim();                       //Path Link
            LData.Company = txtCompany.Text.Trim();                         //บริษัท
            if (cmbContactBranch.SelectedItem.Value != string.Empty)        //สาขาที่สะวดติดต่อกลับ
                LData.ContactBranch = cmbContactBranch.SelectedItem.Value;
            //***********************************************Channel Info************************************************************

            if (cmbBranch.SelectedItem.Value != string.Empty)           //สาขา
                LData.Branch = cmbBranch.SelectedItem.Value;
            if (cmbIsCustomer.SelectedItem.Value != string.Empty)           //เป็นลูกค้าหรือเคยเป็นลูกค้า<br />ของธนาคารหรือไม่
                LData.IsCustomer = cmbIsCustomer.SelectedItem.Value;
            StaffData createbyData = SlmScr010Biz.GetStaffData(HttpContext.Current.User.Identity.Name);
            if (createbyData != null)
                LData.CreatedBy_Branch = createbyData.BranchCode;

            if (txtContractNoRefer.Text.Trim() != "")
                LData.ContractNoRefer = txtContractNoRefer.Text.Trim();

            //2016-12-27 --> SR:5905-123
            LData.TicketIdRefer = CommonData.TicketIDRefer;

            return LData;
        }
        private CampaignWSData GetCampaignWSData()
        {
            try
            {
                CampaignWSData cData = new CampaignWSData();
                if (CommonData.CampaignID != string.Empty)                   //แคมเปญ
                {
                    cData.CampaignId = CommonData.CampaignID;
                    cData.CampaignName = CommonData.R_CampaignName;

                    string detail = SlmScr010Biz.GetCampaignDetail(CommonData.CampaignID);
                    if (detail.Trim().Length > AppConstant.TextMaxLength)
                        throw new Exception("ไม่สามารถบันทึกรายละเอียดแคมเปญเกิน " + AppConstant.TextMaxLength.ToString() + " ตัวอักษรได้\\r\\nรบกวนติดต่อผู้ดูแลระบบ CMT เพื่อแก้ไขรายละเอียด");

                    cData.CampaignDetail = detail;
                }
                cData.Action = "01";
                return cData;
            }
            catch
            {
                throw;
            }
        }
       

        private string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }
            return IP4Address;
        }



        // abstract method implement
        public override bool SaveData(string leadID, string UserID)
        {
            bool ret = true;

            try
            {
                if (ValidateData())
                {
                    LeadInfoBiz leadBiz = new LeadInfoBiz();
                    LeadData LData = GetLeadData();
                    string ticketId = CommonData.TicketID;
                    if (ticketId.Trim() == "")
                    {
                        ticketId = LeadInfoBiz.InsertLeadData(LData, GetCampaignWSData(), HttpContext.Current.User.Identity.Name);
                    }  
                    else
                    {
                        //ticketId = LeadInfoBiz.UpdateLeadData(LData, HttpContext.Current.User.Identity.Name, false, CommonData.ActDelegate, CommonData.ActOwner);
                        ticketId = LeadInfoBiz.UpdateLeadData(LData, HttpContext.Current.User.Identity.Name, false, CommonData.ActDelegate, CommonData.ActOwner, CommonData.OldOwner2);
                    }
                        
                    _log.InfoFormat("TicketId=" + ticketId + ",UserName = " + HttpContext.Current.User.Identity.Name + ",IP Address =" + GetIP4Address() + ",Owner=" + LData.Owner + ",AvailableTime=" + LData.AvailableTime + ",Tel1=" + LData.TelNo_1);

                    //ส่ง SMS
                    //SlmRuleService service = new SlmRuleService();
                    //service.executeRuleSmsAsync(ticketId);

                    //btnSave.Enabled = false;

                    CommonData.R_TicketID = ticketId;
                    //CommonData.R_CampaignName = cmbCampaignId.SelectedItem.Text;
                    //lblResultChannel.Text = cmbChannelId.SelectedItem.Text;
                    //if (cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value != "")
                    //{
                    //    lblResultOwnerLead.Text = cmbOwner.SelectedItem.Text.Trim();

                    //    //int index = cmbOwner.SelectedItem.Text.IndexOf("(");
                    //    //if (index > -1)
                    //    //    lblResultOwnerLead.Text = cmbOwner.SelectedItem.Text.Remove(index);
                    //    //else
                    //    //    lblResultOwnerLead.Text = cmbOwner.SelectedItem.Text;
                    //}

                    if (LData.HasAdamsUrl)
                    {
                        CommonData.R_Message = "ต้องการแนบเอกสารต่อใช่หรือไม่?";
                        CommonData.R_HasAdams = true;
                    }
                    else
                    {
                        CommonData.R_Message = "ต้องการไปหน้าแสดงรายละเอียดผู้มุ่งหวังใช่หรือไม่?";
                        CommonData.R_HasAdams = false;
                    }
                }
                else
                {
                    throw new Exception("กรุณาระบุข้อมูลให้ครบถ้วน");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                CommonData.R_Message = message;
                //AppUtil.ClientAlert(Page, message);
                ret = false;
            }
            return ret;
        }
        public override bool LoadData(LeadData lead, bool isCopy = false, string _type = "")
        {
            bool ret = true;

            try
            {

                LeadInfoBiz bz = new LeadInfoBiz();
                if (isCopy)
                {
                    LeadInfoBiz.LeadDetailInsData ins = new LeadInfoBiz.LeadDetailInsData();
                    List<LeadInfoBiz.LeadDetailAddress> addrLst = new List<LeadInfoBiz.LeadDetailAddress>();

                    if (lead == null) lead = new LeadData();
                    if ((_type == "1" || _type == "2" || _type == "3") && bz.GetInsDetail(lead.TicketId, out ins, out addrLst, Page.User.Identity.Name))
                    {
                        lead.CarType = "";

                        lead.IsCustomer = ins.IsCustomer ? "Y" : "N";// = ld.IsCustomer == "Y";
                        lead.CardType = SLMUtil.SafeInt(ins.CardType);// = ld.CardType.ToString();
                        lead.CitizenId = ins.CitizenId;// = ld.CitizenId;
                        lead.Birthdate = ins.Birthdate;// = ld.Birthdate ?? new DateTime();
                        lead.Occupation = SLMUtil.SafeInt(ins.Occupation);// = ld.Occupation.ToString();
                        lead.ContractNo = ins.slm_ContractNo;// = ld.ContractNo;
                        lead.slm_RedbookYearGroup = ins.slm_RedbookYearGroup;// = ld.slm_RedbookYearGroup;
                        lead.slm_RedbookBrandCode = ins.slm_RedbookBrandCode;// = ld.slm_RedbookBrandCode;
                        lead.slm_RedbookKKKey = ins.slm_RedbookKKKey;// = ld.slm_RedbookKKKey;
                        lead.slm_RedbookModelCode = ins.slm_RedbookModelCode;// = ld.slm_RedbookModelCode;
                        lead.IsCustomer = ins.IsCustomer ? "1" : "0";

                        var adr = addrLst.Where(a => a.slm_AddressType == "C").FirstOrDefault();
                        if (adr != null)
                        {
                            lead.AddressNo = adr.slm_AddressNo;// = ld.AddressNo,
                            lead.BuildingName = adr.slm_BuildingName;// = ld.BuildingName,
                            lead.Floor = adr.slm_Floor;// = ld.Floor,
                            lead.Soi = adr.slm_Soi;// = ld.Soi,
                            lead.Street = adr.slm_Street;// = ld.Street,
                            lead.Province = adr.slm_Province;// = ld.Province,
                            lead.Amphur = adr.slm_Amphur;// = ld.Amphur,
                            lead.Tambon = adr.slm_Tambon;// = ld.Tambon,
                            lead.PostalCode = adr.slm_PostalCode;// = ld.PostalCode
                        }
                    }
                    lead.Detail = "";

                }

                if (lead != null)
                {
                    if (!string.IsNullOrEmpty(lead.Branch))
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

                    if (lead.CountryId != null)
                    {
                        cmbCountry.SelectedValue = lead.CountryId.Value.ToString();
                        cmbCountry.Enabled = true;
                        lblCountry.Text = "*";
                    }

                    if (lead.Birthdate != null)
                        tdBirthdate.DateValue = lead.Birthdate.Value;
                    if (lead.Occupation != null)
                        cmbOccupation.SelectedIndex = cmbOccupation.Items.IndexOf(cmbOccupation.Items.FindByValue(lead.Occupation.ToString()));


                    txtAddressNo.Text = lead.AddressNo;
                    txtBuildingName.Text = lead.BuildingName;
                    txtFloor.Text = lead.Floor;
                    txtSoi.Text = lead.Soi;
                    txtStreet.Text = lead.Street;

                    //if (!string.IsNullOrEmpty(lead.ProvinceCode))
                    if (lead.Province.HasValue)
                    {
                        //cmbProvince.SelectedIndex = cmbProvince.Items.IndexOf(cmbProvince.Items.FindByValue(lead.ProvinceCode.ToString()));
                        cmbProvince.SelectedValue = lead.Province.Value.ToString();
                        ProvinceSelectedIndexChanged();

                    }
                    //if (!string.IsNullOrEmpty(lead.AmphurCode))
                    if (lead.Amphur.HasValue)
                    {
                        //cmbAmphur.SelectedIndex = cmbAmphur.Items.IndexOf(cmbAmphur.Items.FindByValue(lead.AmphurCode.ToString()));
                        cmbAmphur.SelectedValue = lead.Amphur.Value.ToString();
                        AmphurSelectedIndexChanged();
                    }
                    if (lead.Tambon.HasValue)
                    {
                        //cmbTambol.SelectedIndex = cmbTambol.Items.IndexOf(cmbTambol.Items.FindByValue(lead.Tambon.ToString()));
                        cmbTambol.SelectedValue = lead.Tambon.ToString();
                    }


                    txtPostalCode.Text = lead.PostalCode;

                    if (!string.IsNullOrEmpty(lead.CarType))
                        cmbCarType.SelectedIndex = cmbCarType.Items.IndexOf(cmbCarType.Items.FindByValue(lead.CarType));

                    txtLicenseNo.Text = lead.LicenseNo;

                    //if (!string.IsNullOrEmpty(lead.ProvinceRegisCode))
                    //    cmbProvinceRegis.SelectedIndex = cmbProvinceRegis.Items.IndexOf(cmbProvinceRegis.Items.FindByValue(lead.ProvinceRegisCode.ToString()));

                    if (lead.ProvinceRegis != null)
                        cmbProvinceRegis.SelectedIndex = cmbProvinceRegis.Items.IndexOf(cmbProvinceRegis.Items.FindByValue(lead.ProvinceRegis.Value.ToString()));

                    AppUtil.SetComboValue(cmbYearGroup, lead.YearOfCar);
                    txtYearOfCarRegis.Text = lead.YearOfCarRegis;

                    if (!string.IsNullOrEmpty(lead.slm_RedbookBrandCode))
                    {
                        //cmbBrand.SelectedIndex = cmbBrand.Items.IndexOf(cmbBrand.Items.FindByValue(lead.BrandCode.ToString()));
                        AppUtil.SetComboValue(cmbBrand, lead.slm_RedbookBrandCode);
                        cmbBrandSelectedIndexChanged();
                    }
                    if (!string.IsNullOrEmpty(lead.slm_RedbookModelCode))
                    {
                        //cmbModel.SelectedIndex = cmbModel.Items.IndexOf(cmbModel.Items.FindByValue(lead.Family.ToString()));
                        AppUtil.SetComboValue(cmbModel, lead.slm_RedbookModelCode);
                        cmbModelSelectedIndexChanged();
                    }
                    if (lead.slm_RedbookYearGroup != null)
                    {
                        AppUtil.SetComboValue(cmbYearGroup, lead.slm_RedbookYearGroup.ToString());
                        cmbYearGroupSelectedIndexChanged();
                    }
                    if (lead.slm_RedbookKKKey != null)
                        AppUtil.SetComboValue(cmbSubModel, lead.slm_RedbookKKKey);// cmbSubModel.SelectedIndex = cmbSubModel.Items.IndexOf(cmbSubModel.Items.FindByValue(lead.Submodel.ToString()));

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

                    if (!string.IsNullOrEmpty(lead.Invest))
                        txtInvest.Text = Convert.ToDecimal(lead.Invest).ToString("#,##0.00");

                    txtLoanOd.Text = lead.LoanOd;
                    txtLoanOdTerm.Text = lead.LoanOdTerm;

                    if (!string.IsNullOrEmpty(lead.Ebank))
                        cmbEbank.SelectedIndex = cmbEbank.Items.IndexOf(cmbEbank.Items.FindByValue(lead.Ebank));
                    if (!string.IsNullOrEmpty(lead.Atm))
                        cmbAtm.SelectedIndex = cmbAtm.Items.IndexOf(cmbAtm.Items.FindByValue(lead.Atm));

                    txtPathLink.Text = lead.PathLink;

                    //if ((string.IsNullOrEmpty(lead.Owner) == true) && (lead.Status == "00") && ((string.IsNullOrEmpty(lead.AvailableTime)) == false) && (lead.AssignedFlag == "0"))
                    //{
                    //    cmbOwnerBranch.Enabled = true;
                    //    cmbOwner.Enabled = true;
                    //}

                    //if (!isCopy)
                    //{

                    txtDealerCode.Text = lead.DealerCode;
                    txtDealerName.Text = lead.DealerName;
                    txtDealerName.ToolTip = lead.DealerName;
                    txtTopic.Text = lead.Topic;
                    txtDetail.Text = lead.Detail;
                    txtInterestedProd.Text = lead.InterestedProd;

                    txtTelNo3.Text = lead.TelNo_3;
                    txtExt3.Text = lead.Ext_3;
                    txtEmail.Text = lead.Email;
                    txtContractNoRefer.Text = lead.ContractNoRefer;

                    // ฐานเงินเดือน
                    if (lead.BaseSalary != null)
                        txtBaseSalary.Text = lead.BaseSalary.Value.ToString("#,###.00");

                    // สาขาติดต่อกลับ
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

                    if (!string.IsNullOrEmpty(lead.AvailableTime))
                    {
                        txtAvailableTimeHour.Text = lead.AvailableTime.Substring(0, 2);
                        txtAvailableTimeMinute.Text = lead.AvailableTime.Substring(2, 2);
                        txtAvailableTimeSecond.Text = lead.AvailableTime.Substring(4, 2);
                    }
                    //}
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
        public override bool ValidateData()
        {
            try
            {
                int i = 0;
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

                if (SLMUtil.ValidateEmail(txtEmail.Text) == false && txtEmail.Text.Trim() != "")
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

                    if (cmbCountry.SelectedValue == "")
                    {
                        vcmbCountry.Text = "กรุณาระบุ ประเทศ";
                        i += 1;
                    }
                    else
                        vcmbCountry.Text = "";

                    //if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Foreigner && cmbCountry.SelectedValue == "")
                    //{
                    //    vcmbCountry.Text = "กรุณาระบุ ประเทศ";
                    //    i += 1;
                    //}
                    //else
                    //    vcmbCountry.Text = "";
                }
                else
                {
                    vtxtCitizenId.Text = "";
                    vcmbCountry.Text = "";
                }

                if (txtBaseSalary.Text.Trim() != "")
                {
                    string result = AppUtil.SetDecimalFormat(baseSalaryMaxLength, txtBaseSalary);
                    if (result == "error")
                    {
                        vtxtBaseSalary.Text = "ฐานเงินเดือนเกิน " + baseSalaryMaxLength + " หลัก กรุณาระบุใหม่";
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
                        vtxtCarPrice.Text = "ราคารถยนต์เกิน " + defaultMaxLength + " หลัก กรุณาระบุใหม่";
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
                        vtxtDownPayment.Text = "เงินดาวน์เกิน " + defaultMaxLength + " หลัก กรุณาระบุใหม่";
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
                        vtxtDownPercent.Text = "เปอร์เซ็นต์เงินดาวน์เกิน " + percentMaxLength + " หลัก กรุณาระบุใหม่";
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
                        vtxtFinanceAmt.Text = "ยอดจัด Finance เกิน " + defaultMaxLength + " หลัก กรุณาระบุใหม่";
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
                        vtxtBalloonAmt.Text = "Balloon Amount เกิน " + defaultMaxLength + " หลัก กรุณาระบุใหม่";
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
                        vtxtBalloonPercent.Text = "Balloon Percent เกิน " + percentMaxLength + " หลัก กรุณาระบุใหม่";
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

                if (txtInvest.Text.Trim() != "")
                {
                    string result = AppUtil.SetDecimalFormat(defaultMaxLength, txtInvest);
                    if (result == "error")
                    {
                        vtxtInvest.Text = "เงินฝาก/เงินลงทุน เกิน " + defaultMaxLength + " หลัก กรุณาระบุใหม่";
                        i += 1;
                    }
                    else
                        vtxtInvest.Text = "";
                }
                else
                    vtxtInvest.Text = "";

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

                if (txtDetail.Text.Trim().Length > AppConstant.TextMaxLength)
                {
                    //throw new Exception("ไม่สามารถบันทึกรายละเอียดเกิน " + AppConstant.TextMaxLength.ToString() + " ตัวอักษรได้");

                    vtxtDetail.Text = String.Format("<br />ไม่สามารถบันทึกรายละเอียดเกิน {0} ตัวอักษรได้", AppConstant.TextMaxLength);
                    i++;
                }
                else vtxtDetail.Text = "";


                if (i > 0)
                    return false;
                else
                    return true;

            }
            catch
            {
                throw;
            }
        }
        public override void SetControlMode(CtlMode ctlMode)
        {
            switch (ctlMode)
            {
                case CtlMode.New:
                    trInfo.Visible = false;
                    break;

                case CtlMode.View:
                    break;

                case CtlMode.Edit:
                    break;
            }
        }
        public override DropDownList GetComboCardType()
        {
            return cmbCardType;
        }
        public override DropDownList GetComboContry()
        {
            return cmbCountry;
        }
        public override TextBox GetTextBoxCitizenId()
        {
            return txtCitizenId;
        }
    }
}