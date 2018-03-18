using System;
using System.Collections.Generic;
using System.Data;
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
using System.Net;

namespace SLM.Application.Shared
{
    public partial class LeadInfo : System.Web.UI.UserControl
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
                    Page.Form.DefaultButton = btnSave.UniqueID;
                    InitialControl();
                    SetScript();

                    SearchCampaignCheckChanged();
                    GetBranchOwnerDefault();
                    GetChannelDefault();

                    //รับค่า Seaaion มากจากหน้า viewlead, ปุ่ม คัดลอกข้อมูลผู้มุ่งหวัง
                    if (Session["ticket_id"] != null)
                    {
                        InitializeLeadData(Session["ticket_id"].ToString());
                        Session["ticket_id"] = null;
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
        private void InitialControl()
        {
            //แคมเปญ
            cmbCampaignId.DataSource = SlmScr003Biz.GetCampaignData();
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

            //Owner Branch
            cmbOwnerBranch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
            cmbOwnerBranch.DataTextField = "TextField";
            cmbOwnerBranch.DataValueField = "ValueField";
            cmbOwnerBranch.DataBind();
            cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));

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

        private LeadData GetLeadData()
        {
            LeadData LData = new LeadData();

            //*******************************************kkslm_tr_lead****************************************************
            LData.Name = txtName.Text.Trim();                               //ชื่อ
            LData.TelNo_1 = txtTelNo_1.Text.Trim();                         //หมายเลขโทรศัพท์1
            if (cmbCampaignId.SelectedItem.Value != string.Empty)                   //แคมเปญ
                LData.CampaignId = cmbCampaignId.SelectedItem.Value;

            List<ProductData> prodList = SlmScr003Biz.GetProductCampaignData(LData.CampaignId);
            if (prodList.Count > 0)
            {
                LData.ProductGroupId = prodList[0].ProductGroupId;
                LData.ProductId = prodList[0].ProductId;
                LData.ProductName = prodList[0].ProductName;
                LData.HasAdamsUrl = prodList[0].HasAdamsUrl;
            }

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

            if (txtAvailableTimeHour.Text.Trim() != "" && txtAvailableTimeMinute.Text.Trim() != "" && txtAvailableTimeSecond.Text.Trim() != "")
                LData.AvailableTime = txtAvailableTimeHour.Text.Trim() + txtAvailableTimeMinute.Text.Trim() + txtAvailableTimeSecond.Text.Trim();             //เวลาที่สะดวก
            if (cmbChannelId.SelectedItem.Value != string.Empty)                      //ช่องทาง
                LData.ChannelId = cmbChannelId.SelectedItem.Value;
            else
                LData.ChannelId = LeadInfoBiz.GetChannelId("SLM");

            LData.StatusDate = DateTime.Now;

            //*******************************************Product_Info****************************************************
            if (cmbCarType.SelectedItem.Value != string.Empty)              //ประเภทความสนใจ(รถใหม่/รถเก่า)
                LData.CarType = cmbCarType.SelectedItem.Value;
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
            if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty && cmbSubModel.SelectedItem.Value != string.Empty)             //รุ่นย่อยรถ (รุ่นย่อยรถเก็บ Id เป็น value อยู่แล้ว)
                LData.Submodel = Convert.ToInt32(cmbSubModel.SelectedItem.Value);
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
                LData.ProvinceRegis = LeadInfoBiz.GetProvinceId(cmbProvinceRegis.SelectedItem.Value);
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
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty && cmbTambol.SelectedItem.Value != string.Empty)                  //แขวง/ตำบล (ตำบลเก็บ Id เป็น value อยู่แล้ว)
                LData.Tambon = Convert.ToInt32(cmbTambol.SelectedItem.Value);

            LData.PostalCode = txtPostalCode.Text.Trim();                   //รหัสไปรษณีย์
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

            return LData;
        }

        private CampaignWSData GetCampaignWSData()
        {
            try
            {
                CampaignWSData cData = new CampaignWSData();
                if (cmbCampaignId.SelectedItem.Value != string.Empty)                   //แคมเปญ
                {
                    cData.CampaignId = cmbCampaignId.SelectedItem.Value;
                    cData.CampaignName = cmbCampaignId.SelectedItem.Text;

                    string detail = SlmScr010Biz.GetCampaignDetail(cmbCampaignId.SelectedItem.Value);
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData())
                {
                    LeadInfoBiz leadBiz = new LeadInfoBiz();
                    LeadData LData = GetLeadData();

                    string ticketId = LeadInfoBiz.InsertLeadData(LData, GetCampaignWSData(), HttpContext.Current.User.Identity.Name);
                    txtslm_TicketId.Text = ticketId;
                    _log.InfoFormat("TicketId=" + ticketId + ",UserName = " + HttpContext.Current.User.Identity.Name + ",IP Address =" + GetIP4Address() + ",Owner=" + LData.Owner + ",AvailableTime=" + LData.AvailableTime + ",Tel1=" + LData.TelNo_1);
                    
                    //ส่ง SMS
                    //SlmRuleService service = new SlmRuleService();
                    //service.executeRuleSmsAsync(ticketId);

                    btnSave.Enabled = false;

                    lblResultTicketId.Text = ticketId;
                    lblResultCampaign.Text = cmbCampaignId.SelectedItem.Text;
                    lblResultChannel.Text = cmbChannelId.SelectedItem.Text;
                    if (cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value != "")
                    {
                        lblResultOwnerLead.Text = cmbOwner.SelectedItem.Text.Trim();

                        //int index = cmbOwner.SelectedItem.Text.IndexOf("(");
                        //if (index > -1)
                        //    lblResultOwnerLead.Text = cmbOwner.SelectedItem.Text.Remove(index);
                        //else
                        //    lblResultOwnerLead.Text = cmbOwner.SelectedItem.Text;
                    }

                    if (LData.HasAdamsUrl)
                    {
                        lblResultMessage.Text = "ต้องการแนบเอกสารต่อใช่หรือไม่?";
                        cbResultHasAdamsUrl.Checked = true;
                    }
                    else
                    {
                        lblResultMessage.Text = "ต้องการไปหน้าแสดงรายละเอียดผู้มุ่งหวังใช่หรือไม่?";
                        cbResultHasAdamsUrl.Checked = false;
                    }

                    upPopupSaveResult.Update();
                    mpePopupSaveResult.Show();
                }
                else
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุข้อมูลให้ครบถ้วน");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
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

                if (cmbChannelId.SelectedItem.Value == string.Empty)
                {
                    vcmbChannelId.Text = "กรุณาระบุช่องทาง";
                    i += 1;
                }
                else
                {
                    vcmbChannelId.Text = "";
                }
                //****************************Owner**********************************************************
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ Owner Lead";
                    i += 1;
                }
                else
                {
                    vcmbOwner.Text = "";
                    if (cmbCampaignId.SelectedItem.Value != string.Empty && cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value != string.Empty)
                    {
                        if (!SlmScr010Biz.PassPrivilegeCampaign(SLMConstant.Branch.Active, cmbCampaignId.SelectedItem.Value, cmbOwner.SelectedItem.Value))
                        {
                            vcmbOwner.Text = "Owner Lead ไม่มีสิทธิ์ในแคมเปญนี้";
                            i += 1;
                        }
                        else
                            vcmbOwner.Text = "";
                    }
                }


                //****************************หมายเลขโทรศัพท์ 1********************************************
                decimal result1;
                if (txtTelNo_1.Text.Trim() == string.Empty)
                {
                    vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                    i += 1;
                }
                else if (cmbCardType.SelectedItem.Value == "" && (txtTelNo_1.Text.Trim().Length < 9 || txtTelNo_1.Text.Trim().Length > 10))
                {
                    vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                    i += 1;
                }
                else if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Person && txtTelNo_1.Text.Trim().Length != 10)
                {
                    vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                    i += 1;
                }
                else if (cmbCardType.SelectedItem.Value == AppConstant.CardType.JuristicPerson && (txtTelNo_1.Text.Trim().Length < 9 || txtTelNo_1.Text.Trim().Length > 10))
                {
                    vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                    i += 1;
                }
                else if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Foreigner && txtTelNo_1.Text.Trim().Length != 10)
                {
                    vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                    i += 1;
                }
                else if (txtTelNo_1.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo_1.Text.Trim(), out result1))
                {
                    vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องเป็นตัวเลขเท่านั้น";
                    i += 1;
                }
                else if (txtTelNo_1.Text.Trim().StartsWith("0") == false)
                {
                    vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                    i += 1;
                }
                else
                {
                    vtxtTelNo_1.Text = "";
                }

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
                else
                    vtxtCitizenId.Text = "";

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
        //        throw ex;
        //    }
        //}

        protected void txtslm_TelNo_1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal result;
                if (txtTelNo_1.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo_1.Text.Trim(), out result))
                {
                    vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องเป็นตัวเลขเท่านั้น";
                    return;
                }

                if (cmbCardType.SelectedItem.Value == "" || cmbCardType.SelectedItem.Value == AppConstant.CardType.JuristicPerson)
                {
                    if (txtTelNo_1.Text.Trim() == string.Empty || txtTelNo_1.Text.Trim().Length < 9 || txtTelNo_1.Text.Trim().Length > 10)
                    {
                        vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                        return;
                    }
                }
                else
                {
                    if (txtTelNo_1.Text.Trim() == string.Empty || txtTelNo_1.Text.Trim().Length != 10)
                    {
                        vtxtTelNo_1.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                        return;
                    }
                }

                if (txtTelNo_1.Text.Trim().StartsWith("0") == false)
                {
                    vtxtTelNo_1.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                    return;
                }

                vtxtTelNo_1.Text = "";
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

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
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
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
                ProvinceSelectedIndexChanged();
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
            cmbAmphur.DataSource = SlmScr010Biz.GetAmphurData(cmbProvince.SelectedItem.Value);
            cmbAmphur.DataTextField = "TextField";
            cmbAmphur.DataValueField = "ValueField";
            cmbAmphur.DataBind();
            cmbAmphur.Items.Insert(0, new ListItem("", ""));

            //แขวง/ตำบล
            cmbTambol.DataSource = SlmScr010Biz.GetTambolData(cmbProvince.SelectedItem.Value, cmbAmphur.SelectedItem.Value, useWebservice);
            cmbTambol.DataTextField = "TextField";
            cmbTambol.DataValueField = "ValueField";
            cmbTambol.DataBind();
            cmbTambol.Items.Insert(0, new ListItem("", ""));
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

        private void AmphurSelectedIndexChanged()
        {
            //แขวง/ตำบล
            cmbTambol.DataSource = SlmScr010Biz.GetTambolData(cmbProvince.SelectedItem.Value, cmbAmphur.SelectedItem.Value, useWebservice);
            cmbTambol.DataTextField = "TextField";
            cmbTambol.DataValueField = "ValueField";
            cmbTambol.DataBind();
            cmbTambol.Items.Insert(0, new ListItem("", ""));
        }

        //protected void cmbCampaignId_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cmbCampaignId.SelectedItem.Value == string.Empty)
        //        {
        //            vcmbCampaignId.Text = "กรุณาระบุแคมเปญ";
        //            cmbOwner.Enabled = false;
        //        }
        //        else
        //        {
        //            vcmbCampaignId.Text = "";
        //            GetOwnerLead(cmbCampaignId.SelectedItem.Value);
        //            cmbOwner.Enabled = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}


        //private void GetOwnerLead(string campaignId,string branchcode)
        //{
        //    cmbOwner.DataSource = SlmScr003Biz.GetOwnerListByCampaignId(campaignId);
        //    cmbOwner.DataTextField = "TextField";
        //    cmbOwner.DataValueField = "ValueField";
        //    cmbOwner.DataBind();
        //    cmbOwner.Items.Insert(0, new ListItem("", ""));
        //}

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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("SLM_SCR_003.aspx");
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        protected void cmbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbBrandSelectedIndexChanged();
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
            //รุ่นรถ
            cmbModel.DataSource = SlmScr010Biz.GetModelData(cmbBrand.SelectedItem.Value);
            cmbModel.DataTextField = "TextField";
            cmbModel.DataValueField = "ValueField";
            cmbModel.DataBind();
            cmbModel.Items.Insert(0, new ListItem("", ""));

            //รุ่นย่อยรถ
            cmbSubModel.DataSource = SlmScr010Biz.GetSubModelData(cmbBrand.SelectedItem.Value, cmbModel.SelectedItem.Value, useWebservice);
            cmbSubModel.DataTextField = "TextField";
            cmbSubModel.DataValueField = "ValueField";
            cmbSubModel.DataBind();
            cmbSubModel.Items.Insert(0, new ListItem("", ""));
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
            //รุ่นย่อยรถ
            cmbSubModel.DataSource = SlmScr010Biz.GetSubModelData(cmbBrand.SelectedItem.Value, cmbModel.SelectedItem.Value, useWebservice);
            cmbSubModel.DataTextField = "TextField";
            cmbSubModel.DataValueField = "ValueField";
            cmbSubModel.DataBind();
            cmbSubModel.Items.Insert(0, new ListItem("", ""));
        }

        protected void cmbChannelId_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbChannelId.SelectedItem.Value == string.Empty)
                {
                    vcmbChannelId.Text = "กรุณาระบุช่องทาง";
                }
                else
                {
                    vcmbChannelId.Text = "";
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        private LeadData GetLeadDataForWebService()
        {
            LeadData LData = new LeadData();

            //*******************************************kkslm_tr_lead****************************************************
            LData.Name = txtName.Text.Trim();                               //ชื่อ
            LData.TelNo_1 = txtTelNo_1.Text.Trim();                         //หมายเลขโทรศัพท์1
            if (cmbCampaignId.SelectedItem.Value != string.Empty)                   //แคมเปญ
                LData.CampaignId = cmbCampaignId.SelectedItem.Value;
            if (cmbOwner.SelectedItem.Value != string.Empty)                 //owner lead
            {
                LData.Owner = cmbOwner.SelectedItem.Value;
                if (SlmScr010Biz.GetStaffIdData(cmbOwner.SelectedItem.Value) != "")
                    LData.StaffId = Convert.ToInt32(SlmScr010Biz.GetStaffIdData(cmbOwner.SelectedItem.Value));
            }
            if (txtAvailableTimeHour.Text.Trim() != "" && txtAvailableTimeMinute.Text.Trim() != "" && txtAvailableTimeSecond.Text.Trim() != "")
                LData.AvailableTime = txtAvailableTimeHour.Text.Trim() + txtAvailableTimeMinute.Text.Trim() + txtAvailableTimeSecond.Text.Trim();             //เวลาที่สะดวก
            if (cmbChannelId.SelectedItem.Value != string.Empty)                      //ช่องทาง
                LData.ChannelId = cmbChannelId.SelectedItem.Value;
            else
                LData.ChannelId = LeadInfoBiz.GetChannelId("SLM");
            LData.StatusDate = DateTime.Now;

            //*******************************************Product_Info****************************************************
            if (cmbCarType.SelectedItem.Value != string.Empty)              //ประเภทความสนใจ(รถใหม่/รถเก่า)
                LData.CarType = cmbCarType.SelectedItem.Value;
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
            if (cmbBrand.SelectedItem.Value != string.Empty && cmbModel.SelectedItem.Value != string.Empty && cmbSubModel.SelectedItem.Value != string.Empty)             //รุ่นย่อยรถ (รุ่นย่อยรถเก็บ Id เป็น value อยู่แล้ว)
            {
                LData.SubModelCode = cmbSubModel.SelectedItem.Value;
                //LData.Submodel = Convert.ToInt32(cmbSubModel.SelectedItem.Value);
            }
            if (txtDownPayment.Text.Trim() != string.Empty) LData.DownPayment = decimal.Parse(txtDownPayment.Text.Trim().Replace(",", ""));                 //เงินดาวน์
            if (txtDownPercent.Text.Trim() != string.Empty) LData.DownPercent = decimal.Parse(txtDownPercent.Text.Trim());                 //เปอร์เซ็นต์เงินดาวน์
            if (txtCarPrice.Text.Trim() != string.Empty) LData.CarPrice = decimal.Parse(txtCarPrice.Text.Trim().Replace(",", ""));                       //ราคารถยนต์
            if (txtFinanceAmt.Text.Trim() != string.Empty) LData.FinanceAmt = decimal.Parse(txtFinanceAmt.Text.Trim().Replace(",", ""));                   //ยอดจัด Finance
            LData.PaymentTerm = txtPaymentTerm.Text.Trim();                 //ระยะเวลาที่ผ่อนชำระ
            if (cmbPaymentType.SelectedItem.Value != string.Empty)          //ประเภทการผ่อนชำระ
            {
                LData.PaymentTypeCode = cmbPaymentType.SelectedItem.Value;
                //LData.PaymentType = cmbPaymentType.SelectedItem.Value;
            }
            if (txtBalloonAmt.Text.Trim() != string.Empty) LData.BalloonAmt = decimal.Parse(txtBalloonAmt.Text.Trim().Replace(",", ""));                   //Balloon Amount
            if (txtBalloonPercent.Text.Trim() != string.Empty) LData.BalloonPercent = decimal.Parse(txtBalloonPercent.Text.Trim());           //Balloon Percent
            if (tdCoverageDate.DateValue.Year != 1)                          //วันที่เริ่มต้นคุ้มครอง
                LData.CoverageDate = tdCoverageDate.DateValue.Year.ToString() + tdCoverageDate.DateValue.ToString("MMdd");
            if (cmbProvinceRegis.SelectedItem.Value != string.Empty)        //จังหวัดที่จดทะเบียน
            {
                LData.ProvinceRegisCode = cmbProvinceRegis.SelectedItem.Value;
                //LData.ProvinceRegis = LeadInfoBiz.GetProvinceId(cmbProvinceRegis.SelectedItem.Value);
            }
            if (cmbPlanType.SelectedItem.Value != string.Empty)
                LData.PlanType = cmbPlanType.SelectedItem.Value;                       //ประเภทกรมธรรม์
            LData.Interest = txtInterest.Text.Trim();                       //ประเภทความสนใจ
            if (cmbAccType.SelectedItem.Value != string.Empty)              //ประเภทเงินฝาก
            {
                LData.AccTypeCode = cmbAccType.SelectedItem.Value;
                //LData.AccType = Convert.ToInt32("0" + cmbAccType.SelectedItem.Value);
            }
            if (cmbAccPromotion.SelectedItem.Value != string.Empty)              //โปรโมชั่นเงินฝากที่สนใจ
            {
                LData.AccPromotionCode = cmbAccPromotion.SelectedItem.Value;
                //LData.AccPromotion = Convert.ToInt32("0" + cmbAccPromotion.SelectedItem.Value);
            }
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
            if (cmbProvince.SelectedItem.Value != string.Empty && cmbAmphur.SelectedItem.Value != string.Empty && cmbTambol.SelectedItem.Value != string.Empty)   //แขวง/ตำบล (ตำบลเก็บ Id เป็น value อยู่แล้ว)
            {
                LData.TambolCode = cmbTambol.SelectedItem.Value;
                //LData.Tambon = Convert.ToInt32(cmbTambol.SelectedItem.Value);
            }

            LData.PostalCode = txtPostalCode.Text.Trim();                   //รหัสไปรษณีย์
            if (cmbOccupation.SelectedItem.Value != string.Empty)           //อาชีพ
            {
                LData.OccupationCode = cmbOccupation.SelectedItem.Value;
                //LData.Occupation = Convert.ToInt32(cmbOccupation.SelectedItem.Value);
            }
            if (txtBaseSalary.Text.Trim() != string.Empty) LData.BaseSalary = decimal.Parse(txtBaseSalary.Text.Trim().Replace(",", ""));          //ฐานเงินเดือน
            if (tdBirthdate.DateValue.Year != 1)                            //วันเกิด
                LData.Birthdate = tdBirthdate.DateValue;

            if (!string.IsNullOrEmpty(txtCitizenId.Text.Trim()))
                LData.CitizenId = txtCitizenId.Text.Trim();                     //รหัสบัตรประชาชน

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
            return LData;
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
                txtOwnerBranchBefore.Text = cmbOwnerBranch.SelectedItem.Value;
                txtOwnerLeadBefore.Text = "";
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        private void cmbOwnerBranchSelectedIndexChanged()
        {
            try
            {
                //Owner Lead
                List<ControlListData> source = null;
                if (cmbOwnerBranch.SelectedItem != null)
                {
                    if(cmbCampaignId.SelectedItem.Value == "")
                        source = StaffBiz.GetStaffList(cmbOwnerBranch.SelectedItem.Value);   //SlmScr010Biz.GetStaffAllData(cmbOwnerBranch.SelectedItem.Value);
                    else
                        source = StaffBiz.GetStaffAllDataByAccessRight(cmbCampaignId.SelectedItem.Value, cmbOwnerBranch.SelectedItem.Value);

                    #region ไม่ได้ใช้แล้ว เนื่องจากมีการเปลี่ยนการมองเห็นงานเป็นแบบ Productหรือแคมเปญ และ สาขา แทน
                    //if (cmbCampaignId.SelectedItem.Value == "")
                    //source = SlmScr010Biz.GetStaffAllData(cmbOwnerBranch.SelectedItem.Value);
                    //else
                    //    source = SlmScr010Biz.GetOwnerListByCampaignIdAndBranch(cmbCampaignId.SelectedItem.Value, cmbOwnerBranch.SelectedItem.Value);
                    #endregion

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
                    txtOwnerLeadBefore.Text = "";
                }
                else
                {
                    vcmbOwner.Text = "";
                    txtOwnerLeadBefore.Text = cmbOwner.SelectedItem.Value;
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        #region Popup Search Campaign

        protected void btnClose_Click(object sender, EventArgs e)
        {
            mpePopupSearchCampaign.Hide();
        }

        protected void imbSearchCampaign_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                rbSearchByCombo.Checked = true;
                rbSearchByText.Checked = false;
                SearchCampaignCheckChanged();
                BindComboProductGroup();
                cmbProduct.Items.Clear();
                cmbProduct.Items.Insert(0, new ListItem("", "0"));
                cmbCampaign.Items.Clear();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));

                pcGridCampaign.SetVisible = false;
                gvCampaign.DataSource = null;
                gvCampaign.DataBind();
                gvCampaign.Visible = false;

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
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

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
                cmbProduct.DataSource = SlmScr003Biz.GetProductData(cmbProductGroup.SelectedItem.Value);
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
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbCampaign.DataSource = SlmScr003Biz.GetCampaignData(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value);
                cmbCampaign.DataTextField = "TextField";
                cmbCampaign.DataValueField = "ValueField";
                cmbCampaign.DataBind();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));

                mpePopupSearchCampaign.Show();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void rbSearchByCombo_CheckedChanged(object sender, EventArgs e)
        {
            SearchCampaignCheckChanged();
            upPopupSearchCampaign.Update();
            mpePopupSearchCampaign.Show();
        }

        protected void rbSearchByText_CheckedChanged(object sender, EventArgs e)
        {
            SearchCampaignCheckChanged();
            upPopupSearchCampaign.Update();
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
            gvCampaign.DataSource = null;
            gvCampaign.DataBind();
            gvCampaign.Visible = false;

            if (rbSearchByText.Checked)
            {
                cmbProductGroup.SelectedIndex = -1;
                cmbProduct.Items.Clear();
                cmbProduct.Items.Insert(0, new ListItem("", "0"));
                cmbCampaign.Items.Clear();
                cmbCampaign.Items.Insert(0, new ListItem("", ""));
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
                _log.Debug(message);
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

        protected void gvCampaign_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((Label)e.Row.FindControl("lblCampaignId")).Text.Trim() == "")
                {
                    ((CheckBox)e.Row.FindControl("cbSelect")).Visible = false;
                }

                Label lblCampaignDesc = (Label)e.Row.FindControl("lblCampaignDesc");
                if (lblCampaignDesc.Text.Trim().Length > AppConstant.Campaign.DisplayCampaignDescMaxLength)
                {
                    lblCampaignDesc.Text = lblCampaignDesc.Text.Trim().Substring(0, AppConstant.Campaign.DisplayCampaignDescMaxLength) + "...";
                    LinkButton lbShowCampaignDesc = (LinkButton)e.Row.FindControl("lbShowCampaignDesc");
                    lbShowCampaignDesc.Visible = true;
                    lbShowCampaignDesc.OnClientClick = AppUtil.GetShowCampaignDescScript(Page, lbShowCampaignDesc.CommandArgument, "leadinfo_campaigndesc_" + lbShowCampaignDesc.CommandArgument);
                }
            }
        }

        protected void cbSelect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                int index = ((CheckBox)sender).TabIndex;
                string campaignId = ((Label)gvCampaign.Rows[index].FindControl("lblCampaignId")).Text.Trim();
                cmbCampaignId.SelectedIndex = cmbCampaignId.Items.IndexOf(cmbCampaignId.Items.FindByValue(campaignId));
                SetOwnerBranchAccessRight();
                upHeader1.Update();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchCampaign()
        {
            List<ProductData> result = null;

            if (rbSearchByCombo.Checked)
                result = SlmScr003Biz.SearchCampaign(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, cmbCampaign.SelectedItem.Value);
            else
                result = SlmScr003Biz.SearchCampaign(txtFullSearchCampaign.Text.Trim());

            BindGridview((SLM.Application.Shared.GridviewPageController)pcGridCampaign, result.ToArray(), 0);
            gvCampaign.Visible = true;
            upPopupSearchCampaign.Update();
        }

        #endregion

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvCampaign);
            pageControl.Update(items, pageIndex);
            upPopupSearchCampaign.Update();
            mpePopupSearchCampaign.Show();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                List<ProductData> result = null;

                if (rbSearchByCombo.Checked)
                    result = SlmScr003Biz.SearchCampaign(cmbProductGroup.SelectedItem.Value, cmbProduct.SelectedItem.Value, cmbCampaign.SelectedItem.Value);
                else
                    result = SlmScr003Biz.SearchCampaign(txtFullSearchCampaign.Text.Trim());

                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridview(pageControl, result.AsEnumerable().ToArray(), pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        private void GetBranchOwnerDefault()
        {
            try
            {
                List<StaffData> stafflist = SlmScr016Biz.GetChannelStaffData(HttpContext.Current.User.Identity.Name);
                if (stafflist.Count > 0)
                {
                    if (!string.IsNullOrEmpty(stafflist.FirstOrDefault().ChannelId))
                    {
                        if ((stafflist.FirstOrDefault().ChannelId.ToUpper() == SLM.Resource.SLMConstant.ChannelId.Branch) ||
                            (stafflist.FirstOrDefault().ChannelId.ToUpper() == SLM.Resource.SLMConstant.ChannelId.Telesales) || 
                            (stafflist.FirstOrDefault().ChannelId.ToUpper() == SLM.Resource.SLMConstant.ChannelId.PriorityBanking))
                        {
                            string branch = SlmScr010Biz.GetBranchData(HttpContext.Current.User.Identity.Name);
                            if (branch != "")
                            {
                                //cmbChannelId.SelectedIndex = cmbChannelId.Items.IndexOf(cmbChannelId.Items.FindByValue(stafflist.FirstOrDefault().ChannelId));
                                cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(cmbOwnerBranch.Items.FindByValue(branch));
                                cmbOwnerBranchSelectedIndexChanged();

                                if (cmbOwner.Items.Count > 0)
                                {
                                    ListItem item = cmbOwner.Items.OfType<ListItem>().Where(p => p.Value.Trim().ToLower() == HttpContext.Current.User.Identity.Name.Trim().ToLower()).FirstOrDefault();
                                    if (item != null)
                                        cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(item);
                                }

                                //cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(HttpContext.Current.User.Identity.Name));
                                //txtOwnerBranchUserLogin.Text = branch;
                                txtOwnerBranchBefore.Text = branch;
                                txtOwnerLeadBefore.Text = cmbOwner.SelectedItem.Value;
                            }
                        }
                        else
                        {
                            cmbOwner.Items.Clear();
                            cmbOwner.Enabled = false;
                        }
                    }
                    else
                    {
                        cmbOwner.Items.Clear();
                        cmbOwner.Enabled = false;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        protected void btnAttachDocYes_Click(object sender, EventArgs e)
        {
            try
            {
                btnAttachDocYes.Enabled = false;
                btnAttachDocNo.Enabled = false;

                string type = new ConfigProductScreenBiz().GetFieldType(lblResultTicketId.Text.Trim(), SLMConstant.ConfigProductScreen.ActionType.View);

                if (cbResultHasAdamsUrl.Checked)
                {
                    LeadDataForAdam leadData = SlmScr003Biz.GetLeadDataForAdam(lblResultTicketId.Text.Trim());
                    StaffData staff = SlmScr003Biz.GetStaff(HttpContext.Current.User.Identity.Name);

                    string script = AppUtil.GetCallAdamScript(leadData, HttpContext.Current.User.Identity.Name, (staff.EmpCode != null ? staff.EmpCode : ""), true, type);

                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "calladam", script, true);
                }
                else
                    Response.Redirect("SLM_SCR_004.aspx?ticketid=" + lblResultTicketId.Text.Trim() + "&type=" + type);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_003.aspx");
            }
        }

        protected void btnAttachDocNo_Click(object sender, EventArgs e)
        {
            btnAttachDocYes.Enabled = false;
            btnAttachDocNo.Enabled = false;
            Response.Redirect("SLM_SCR_003.aspx");
        }

        protected void cmbCardType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCardType.SelectedItem.Value == "")
                {
                    lblCitizenId.Text = "";
                    txtCitizenId.Text = "";
                    vtxtCitizenId.Text = "";
                    txtCitizenId.Enabled = false;
                }
                else
                {
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
        //        string result = AppUtil.SetDecimalFormat(baseSalaryMaxLength, txtBaseSalary);
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
        //        string result = AppUtil.SetDecimalFormat(defaultMaxLength, txtCarPrice);
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
        //        string result = AppUtil.SetDecimalFormat(defaultMaxLength, txtDownPayment);
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
        //        string result = AppUtil.SetPercentFormat(percentMaxLength, txtDownPercent);
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
        //        string result = AppUtil.SetDecimalFormat(defaultMaxLength, txtFinanceAmt);
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
        //        string result = AppUtil.SetDecimalFormat(defaultMaxLength, txtBalloonAmt);
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
        //        string result = AppUtil.SetPercentFormat(percentMaxLength, txtBalloonPercent);
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


        private void GetChannelDefault()
        {
            try
            {
                string channelId = SlmScr010Biz.GetChannelDefault(HttpContext.Current.User.Identity.Name);
                if (!string.IsNullOrEmpty(channelId))
                {
                    cmbChannelId.SelectedIndex = cmbChannelId.Items.IndexOf(cmbChannelId.Items.FindByValue(channelId));
                }
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
                vcmbOwner.Text = "";
                SetOwnerBranchAccessRight();
                if (cmbOwnerBranch.SelectedItem != null)
                    txtOwnerBranchBefore.Text = cmbOwnerBranch.SelectedItem.Value;
                else
                    txtOwnerBranchBefore.Text = "";

                if (cmbOwner.SelectedItem != null)
                    txtOwnerLeadBefore.Text = cmbOwner.SelectedItem.Value;
                else
                    txtOwnerLeadBefore.Text = "";
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void SetOwnerBranchAccessRight()
        {
            try
            {
                List<BranchData> branch = new List<BranchData>();
                if (cmbCampaignId.SelectedItem.Value == "")
                {
                    txtOwnerBranchBefore.Text = "";
                    txtOwnerLeadBefore.Text = "";
                    //Owner Branch
                    cmbOwnerBranch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
                    cmbOwnerBranch.DataTextField = "TextField";
                    cmbOwnerBranch.DataValueField = "ValueField";
                    cmbOwnerBranch.DataBind();
                    cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));
                    cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(cmbOwnerBranch.Items.FindByValue(txtOwnerBranchBefore.Text.Trim()));
                    cmbOwnerBranchSelectedIndexChanged();
                    cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(txtOwnerLeadBefore.Text.Trim()));
                }
                else
                {
                    cmbOwnerBranch.DataSource = SlmScr010Biz.GetBranchListByAccessRight(SLMConstant.Branch.Active, cmbCampaignId.SelectedItem.Value);
                    cmbOwnerBranch.DataBind();
                    cmbOwnerBranch.Items.Insert(0, new ListItem("", ""));

                    if (SlmScr010Biz.CheckStaffAccessRightExist(cmbCampaignId.SelectedItem.Value, txtOwnerBranchBefore.Text.Trim(), txtOwnerLeadBefore.Text.Trim()))
                    {
                        if (cmbOwnerBranch.Items.Count > 1)
                        {
                            if (txtOwnerBranchBefore.Text.Trim() != "")
                                cmbOwnerBranch.SelectedIndex = cmbOwnerBranch.Items.IndexOf(cmbOwnerBranch.Items.FindByValue(txtOwnerBranchBefore.Text.Trim()));
                            else
                                cmbOwnerBranch.SelectedIndex = -1;

                        }
                        cmbOwnerBranchSelectedIndexChanged();
                        cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(cmbOwner.Items.FindByValue(txtOwnerLeadBefore.Text.Trim()));
                    }
                    else
                    {
                        cmbOwnerBranchSelectedIndexChanged();
                        txtOwnerBranchBefore.Text = "";
                        txtOwnerLeadBefore.Text = "";
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void InitializeLeadData(string ticketId)
        {
            try
            {
                LeadData lead = SlmScr010Biz.GetLeadData(ticketId);
                if (lead != null)
                {
                    txtName.Text = lead.Name;
                    txtLastName.Text = lead.LastName;

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

                    txtTelNo_1.Text = lead.TelNo_1;

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
                        ProvinceSelectedIndexChanged();
                    }
                    if (!string.IsNullOrEmpty(lead.AmphurCode))
                    {
                        cmbAmphur.SelectedIndex = cmbAmphur.Items.IndexOf(cmbAmphur.Items.FindByValue(lead.AmphurCode.ToString()));
                        AmphurSelectedIndexChanged();
                    }
                    if (lead.Tambon != null)
                        cmbTambol.SelectedIndex = cmbTambol.Items.IndexOf(cmbTambol.Items.FindByValue(lead.Tambon.ToString()));

                    txtPostalCode.Text = lead.PostalCode;

                    if (!string.IsNullOrEmpty(lead.CarType))
                        cmbCarType.SelectedIndex = cmbCarType.Items.IndexOf(cmbCarType.Items.FindByValue(lead.CarType));

                    txtLicenseNo.Text = lead.LicenseNo;

                    if (!string.IsNullOrEmpty(lead.ProvinceRegisCode))
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
    }
}