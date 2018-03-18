using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Biz;
using SLM.Application.Services;
using SLM.Dal;
using log4net;

namespace SLM.Application.Utilities
{
    public class AppUtil
    {
        public static void ClientAlert(Control control, string message)
        {
            if (control != null && message != null)
            {
                ScriptManager.RegisterClientScriptBlock(control, control.GetType(), "error", "alert('" + message.Replace("'", "\\'").Replace("\r\n", "\\n") + "');", true);
            }
        }

        public static void ClientAlertTabRenew(Control control, string message)
        {
            if (control != null && message != null)
            {
                ScriptManager.RegisterStartupScript(control, control.GetType(), "error", "pageLoad();alert('" + message.Replace("'", "\\'").Replace("\r\n", "\\n") + "');", true);
            }
        }

        public static void ClientAlertAndRedirect(Control control, string message, string url)
        {
            if (control != null && message != null)
            {
                ScriptManager.RegisterClientScriptBlock(control, control.GetType(), "error", "alert('" + message.Replace("'", "\\'").Replace("\r\n", "\\n") + "'); document.location='" + url + "';", true);
            }
        }

        public static string GetShowCampaignDescScript(Page page, string campaignId, string scriptName)
        {
            return page != null ? ("window.open('" + page.ResolveUrl("~/Shared/CampaignDesc.aspx?campaignid=" + campaignId) + "', '" + scriptName + "', 'status=yes, toolbar=no, scrollbars=yes, menubar=no, width=800, height=600, resizable=yes'); return false;") : "";
        }

        public static string GetNoticeDownloadScript(Page page, string virtualPath, string scriptName)
        {
            return page != null ? ("window.open('" + page.ResolveUrl("~" + virtualPath) + "', '" + scriptName + "', 'status=yes, toolbar=no, scrollbars=yes, menubar=no, width=800, height=600, resizable=yes'); return false;") : "";
        }

        public static void SetIntTextBox(TextBox textbox)
        {
            if (textbox != null)
            {
                textbox.Attributes.Add("OnKeyPress", "return ChkInt(event)");
                //textbox.Attributes.Add("OnBlur", "ChkIntOnBlurClear(this)");
            }
        }

        public static void SetIntTextBox(TextBox textbox, Label label, string errorMsg)
        {
            if (textbox != null && label != null)
            {
                textbox.Attributes.Add("OnKeyPress", "return ChkInt(event)");
                textbox.Attributes.Add("OnBlur", "ChkIntOnBlur(this, '" + label.ClientID + "', '" + errorMsg + "')");
            }
        }

        public static void SetMoneyTextBox(TextBox textbox)
        {
            if (textbox != null)
            {
                textbox.Attributes.Add("OnKeyPress", "return ChkDbl(event, this)");
                textbox.Attributes.Add("OnBlur", "valDbl(this)");
                textbox.Attributes.Add("OnFocus", "prepareNum(this)");
            }
        }

        public static void SetMoneyTextBox(TextBox textbox, string label_clientId, string errMsg, int maxlength)
        {
            if (textbox != null)
            {
                textbox.Attributes.Add("OnKeyPress", "return ChkDbl(event, this)");
                textbox.Attributes.Add("OnBlur", "valDbl2(this, '" + label_clientId + "', '" + errMsg + "', " + maxlength.ToString() + ")");
                textbox.Attributes.Add("OnFocus", "prepareNum(this)");
            }
        }

        public static void SetMoneyTextBox(TextBox textBox, Label label)
        {
            if (textBox != null && label != null)
            {
                SetMoneyTextBox(textBox, label.ClientID, $"จำนวนเงินเกิน {textBox.MaxLength - 3} หลัก กรุณาระบุใหม่", textBox.MaxLength - 3);
            }
        }

        public static void SetPercentTextBox(TextBox textbox, string label_clientId, string errMsg)
        {
            if (textbox != null)
            {
                textbox.Attributes.Add("OnKeyPress", "return ChkDbl(event, this)");
                textbox.Attributes.Add("OnBlur", "valPercent(this, '" + label_clientId + "', '" + errMsg + "')");
                textbox.Attributes.Add("OnFocus", "prepareNum(this)");
            }
        }

        public static void SetMultilineMaxLength(TextBox textbox, string labelId, string maxlength)
        {
            if (textbox != null)
            {
                textbox.Attributes.Add("onkeyup", "return validateLimit(this, '" + labelId + "', " + maxlength + ")");
                textbox.Attributes.Add("onblur", "return validateLimit(this, '" + labelId + "', " + maxlength + ")");
            }
        }

        public static void SetNotThaiCharacter(TextBox textbox)
        {
            if (textbox != null)
            {
                textbox.Attributes.Add("OnKeyPress", "return ChkNotThaiCharacter(event)");
            }
        }
        public static void SetNotEnglishCharacter(TextBox textbox)
        {
            if (textbox != null)
            {
                textbox.Attributes.Add("OnKeyPress", "return ChkNotEnglishCharacter(event)");
            }
        }
        public static void SetNumbericCharacter(TextBox textbox)
        {
            if (textbox != null)
            {
                textbox.Attributes.Add("OnKeyPress", "return ChkNumeric(event)");
            }
        }

        public static string SetDecimalFormat(int maxlength, TextBox textbox)
        {
            string value = textbox.Text.Trim().Replace(",", "");

            if (value.IndexOf(".") >= 0)
            {
                string[] val = value.Split('.');
                if (val[0].Trim().Length == 0 && val[1].Trim().Length == 0)
                {
                    return "0.00";
                }

                if (val[0].Trim().Length > maxlength)
                {
                    return "error";
                }

                return Convert.ToDecimal(value).ToString("#,##0.00");
            }
            else
            {
                if (value == string.Empty)
                {
                    return string.Empty;
                }
                else
                {
                    return value.Length > maxlength ? "error" : Convert.ToDecimal(value).ToString("#,##0.00");
                }
            }
        }
        public static string SetPercentFormat(int maxlength, TextBox textbox)
        {
            string value = textbox.Text.Trim().Replace(",", "");

            if (value.IndexOf(".") >= 0)
            {
                string[] val = value.Split('.');
                if (val[0].Trim().Length == 0 && val[1].Trim().Length == 0)
                {
                    return "0.00";
                }

                if (val[0].Trim().Length > maxlength)
                {
                    return "error";
                }

                if (Convert.ToDecimal(value) > 100)
                {
                    return "error100";
                }

                return Convert.ToDecimal(value).ToString("0.00");
            }
            else
            {
                if (value == string.Empty)
                {
                    return string.Empty;
                }
                else
                {
                    if (Convert.ToDecimal(value) > 100)
                    {
                        return "error100";
                    }

                    return value.Length > maxlength ? "error" : Convert.ToDecimal(value).ToString("0.00");
                }
            }
        }

        /// <summary>
        /// <br>Method Name : ConvertToDateTime</br>
        /// <br>Purpose     : To convert date and time string to class DateTime.</br>
        /// </summary>
        /// <param name="date">string format yyyyMMdd</param>
        /// <param name="time">string format HHmmss</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string date, string time)
        {
            try
            {
                if (string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date) || date.Length != 8)
                {
                    return new DateTime();
                }
                else
                {
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);
                    string day = date.Substring(6, 2);

                    if (string.IsNullOrEmpty(time) || string.IsNullOrWhiteSpace(time) || time.Length != 6)
                    {
                        return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                    }
                    else
                    {
                        string hour = time.Substring(0, 2);
                        string min = time.Substring(2, 2);
                        string sec = time.Substring(4, 2);
                        return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(min), int.Parse(sec));
                    }
                }
            }
            catch
            {
                return new DateTime();
            }
        }

        public static void SetTimeControlScript(TextBox textboxHour, TextBox textboxMinute)
        {
            string script = "";
            //Hour
            script = @" var hour = document.getElementById('" + textboxHour.ClientID + @"').value; 
                        if(hour.length > 0)
                        {
                            if (isNaN(hour))
                            {
                                document.getElementById('" + textboxHour.ClientID + @"').value = '';
                                document.getElementById('" + textboxHour.ClientID + @"').focus();
                                return;
                            }

                            while(hour.length < 2)
                            {
                                hour = '0' + hour;
                            }

                            if (hour < 0 || hour > 23)
                            { 
                                alert('กรุณาระบุชั่วโมงระหว่าง 0-23 น.'); 
                                document.getElementById('" + textboxHour.ClientID + @"').focus(); 
                                document.getElementById('" + textboxHour.ClientID + @"').value = '';
                            }
                            else
                            {
                                document.getElementById('" + textboxHour.ClientID + @"').value = hour;
                            }
                        }";

            textboxHour.Attributes.Add("onblur", script);

            script = "if (document.getElementById('" + textboxHour.ClientID + @"').value.length == 2 && document.getElementById('" + textboxHour.ClientID + @"').value <= 23)
                             {document.getElementById('" + textboxMinute.ClientID + @"').focus(); }";

            textboxHour.Attributes.Add("onkeyup", script);

            //Minute
            script = @" var minute = document.getElementById('" + textboxMinute.ClientID + @"').value; 
                        if(minute.length > 0)
                        {
                            if (isNaN(minute))
                            {
                                document.getElementById('" + textboxMinute.ClientID + @"').value = '';
                                document.getElementById('" + textboxMinute.ClientID + @"').focus();
                                return;
                            }

                            while(minute.length < 2)
                            {
                                minute = '0' + minute;
                            }
                            
                            if (minute > 59)
                            { 
                                alert('กรุณาระบุนาทีระหว่าง 0-59 นาที');
                                document.getElementById('" + textboxMinute.ClientID + @"').focus();
                                document.getElementById('" + textboxMinute.ClientID + @"').value = ''; 
                            }
                            else
                            {
                                document.getElementById('" + textboxMinute.ClientID + @"').value = minute;
                            }
                        }";

            textboxMinute.Attributes.Add("onblur", script);
        }

        #region CardType-CitizenId

        /// <summary>
        /// ตรวจสอบรหัสบัตรประชาชน
        /// </summary>
        /// <param name="citizenId"></param>
        /// <returns></returns>
        public static bool VerifyCitizenId(String citizenId)
        {
            try
            {
                //ตรวจสอบว่าข้อมูลมีทั้งหมด 13 หลัก
                if (citizenId.Trim().Length != 13)
                    return false;

                //ตรวจสอบว่าข้อมูลต้องเป็นตัวเลขเท่านั้น
                decimal result;
                if (!decimal.TryParse(citizenId, out result))
                    return false;

                int sumValue = 0;
                for (int i = 0; i < citizenId.Length - 1; i++)
                    sumValue += int.Parse(citizenId[i].ToString()) * (13 - i);

                int v = 11 - (sumValue % 11);

                string digit = "";
                if (v.ToString().Length == 2)
                    digit = v.ToString().Substring(1, 1);
                else
                    digit = v.ToString();

                return citizenId[12].ToString() == digit;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// เซ็ทค่าการ validate ประเภทบุคคลใน TextBox ที่กำหนด
        /// </summary>
        public static void SetCardTypeValidation(string cardTypeValue, TextBox textboxCardId)
        {
            if (cardTypeValue == AppConstant.CardType.JuristicPerson || cardTypeValue == AppConstant.CardType.Foreigner)
            {
                textboxCardId.MaxLength = 50;
                textboxCardId.Attributes.Add("OnKeyPress", "");
            }
            else
            {
                textboxCardId.MaxLength = 13;           //บุคคลธรรม
                AppUtil.SetIntTextBox(textboxCardId);
            }
        }

        /// <summary>
        /// ตรวจความถูกต้องประเภทลูกค้าและเลขที่บัตร
        /// </summary>
        /// <param name="cmbCardType"></param>
        /// <param name="txtCitizenId"></param>
        /// <param name="vtxtCitizenId"></param>
        /// <returns></returns>
        public static bool ValidateCardId(DropDownList cmbCardType, TextBox txtCitizenId, Label vtxtCitizenId)
        {
            int i = 0;
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

            return i > 0 ? false : true;
        }

        #endregion

        #region TelNo1

        public static bool ValidateTelNo1(DropDownList cmbCardType, TextBox txtTelNo1, Label lblAlert)
        {
            int i = 0;
            decimal result1;
            if (txtTelNo1.Text.Trim() == string.Empty)
            {
                lblAlert.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                i += 1;
            }
            else if (cmbCardType.SelectedItem.Value == "" && (txtTelNo1.Text.Trim().Length < 9 || txtTelNo1.Text.Trim().Length > 10))
            {
                lblAlert.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                i += 1;
            }
            else if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Person && txtTelNo1.Text.Trim().Length != 10)
            {
                lblAlert.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                i += 1;
            }
            else if (cmbCardType.SelectedItem.Value == AppConstant.CardType.JuristicPerson && (txtTelNo1.Text.Trim().Length < 9 || txtTelNo1.Text.Trim().Length > 10))
            {
                lblAlert.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                i += 1;
            }
            else if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Foreigner && txtTelNo1.Text.Trim().Length != 10)
            {
                lblAlert.Text = "กรุณาระบุหมายเลขโทรศัพท์ 1(มือถือ)ให้ถูกต้อง";
                i += 1;
            }
            else if (txtTelNo1.Text.Trim() != string.Empty && !decimal.TryParse(txtTelNo1.Text.Trim(), out result1))
            {
                lblAlert.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องเป็นตัวเลขเท่านั้น";
                i += 1;
            }
            else if (txtTelNo1.Text.Trim().StartsWith("0") == false)
            {
                lblAlert.Text = "หมายเลขโทรศัพท์ 1(มือถือ)ต้องขึ้นต้นด้วยเลข 0 เท่านั้น";
                i += 1;
            }
            else
            {
                lblAlert.Text = "";
            }

            return i > 0 ? false : true;
        }

        public static bool ValidateTelNo1(string cardTypeId, string telNo1)
        {
            decimal result1;

            if (telNo1.Trim() == string.Empty)
            {
                return false;
            }
            else if (cardTypeId == "" && (telNo1.Trim().Length < 9 || telNo1.Trim().Length > 10))
            {
                return false;
            }
            else if (cardTypeId == AppConstant.CardType.Person && telNo1.Trim().Length != 10)
            {
                return false;
            }
            else if (cardTypeId == AppConstant.CardType.JuristicPerson && (telNo1.Trim().Length < 9 || telNo1.Trim().Length > 10))
            {
                return false;
            }
            else if (cardTypeId == AppConstant.CardType.Foreigner && telNo1.Trim().Length != 10)
            {
                return false;
            }
            else if (telNo1.Trim() != string.Empty && !decimal.TryParse(telNo1.Trim(), out result1))
            {
                return false;
            }
            else if (telNo1.Trim().StartsWith("0") == false)
            {
                return false;
            }

            return true;
        }

        public static bool ValidateTelNo2(string telNo2)
        {
            decimal result;

            if (!decimal.TryParse(telNo2, out result))
            {
                return false;
            }
            else if (result == 0 || (telNo2.Trim().Length < 9 || telNo2.Trim().Length > 10) || telNo2.Trim().StartsWith("0") == false)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Owner and Delegate

        public static bool ValidateOwner(string assignedFlag, DropDownList cmbOwnerBranch, Label vcmbOwnerBranch, DropDownList cmbOwner, Label vcmbOwner, string campaignId, ref string clearOwnerBranchText)
        {
            int i = 0;
            if (assignedFlag == "0")  //กรณียังไม่จ่ายงาน
            {
                if (cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ Owner Lead";
                    i += 1;
                }
                else
                {
                    vcmbOwner.Text = "";
                    if (campaignId != string.Empty && cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value != string.Empty)
                    {
                        if (!SlmScr010Biz.PassPrivilegeCampaign(SLMConstant.Branch.All, campaignId, cmbOwner.SelectedItem.Value))
                        {
                            vcmbOwner.Text = "Owner Lead ไม่มีสิทธิ์ในแคมเปญนี้";
                            i += 1;
                        }
                        else
                            vcmbOwner.Text = "";
                    }
                }
            }
            else if (assignedFlag == "1") //กรณีจ่ายงานแล้ว
            {
                if (cmbOwnerBranch.SelectedItem.Value == string.Empty || cmbOwner.SelectedItem.Value == string.Empty)
                {
                    if (cmbOwnerBranch.SelectedItem.Value == string.Empty)
                    {
                        vcmbOwnerBranch.Text = "กรุณาระบุ Owner Branch";
                        i += 1;
                        clearOwnerBranchText = "N";
                    }

                    if (cmbOwner.SelectedItem.Value == string.Empty)
                    {
                        vcmbOwner.Text = "กรุณาระบุ Owner Lead";
                        i += 1;
                    }
                }
                else
                {
                    vcmbOwnerBranch.Text = "";
                    vcmbOwner.Text = "";
                    if (campaignId != string.Empty && cmbOwnerBranch.SelectedItem.Value != string.Empty && cmbOwner.Items.Count > 0 && cmbOwner.SelectedItem.Value != string.Empty)
                    {
                        if (!SlmScr010Biz.PassPrivilegeCampaign(SLMConstant.Branch.All, campaignId, cmbOwner.SelectedItem.Value))
                        {
                            vcmbOwner.Text = "Owner Lead ไม่มีสิทธิ์ในแคมเปญนี้";
                            i += 1;
                        }
                        else
                            vcmbOwner.Text = "";
                    }
                }
            }

            return i > 0 ? false : true;
        }

        #endregion


        /// <summary>
        /// คำนวณงานในมือของพนักงานตามสาขาที่ส่งเข้ามา
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="?"></param>
        public static void CalculateAmountJobOnHandForDropdownlist(string branchCode, List<ControlListData> source)
        {
            var amountJobList = StaffBiz.GetAmountJobOnHandList(branchCode);
            foreach (ControlListData owner in source)
            {
                var staff = amountJobList.Where(p => p.Username == owner.ValueField).FirstOrDefault();
                if (staff != null)
                    owner.TextField += " (" + (staff.AmountOwner + staff.AmountDelegate).ToString() + " งาน)";
            }
        }

        /// <summary>
        /// เช็กสิทธิในการแก้ไข Owner และ Delegate
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="delegateLead"></param>
        /// <param name="cmbOwnerBranch"></param>
        /// <param name="cmbOwner"></param>
        /// <param name="cmbDelegateBranch"></param>
        /// <param name="cmbDelegateLead"></param>
        public static void CheckOwnerPrivilege(string owner, string delegateLead, DropDownList cmbOwnerBranch, DropDownList cmbOwner, DropDownList cmbDelegateBranch, DropDownList cmbDelegateLead)
        {
            try
            {
                bool privilegeOwner = false;
                cmbDelegateBranch.Enabled = false;
                cmbDelegateLead.Enabled = false;
                cmbOwnerBranch.Enabled = false;
                cmbOwner.Enabled = false;

                List<string> recusiveList = StaffBiz.GetRecursiveStaffList(HttpContext.Current.User.Identity.Name);
                if (cmbOwner.Items.Count > 0 && !string.IsNullOrEmpty(owner))
                {
                    //ถ้าเป็น Owner หรือหัวหน้า Owner จะเปิดให้สามารถแก้ไข Owner และ Delegate ได้
                    if (recusiveList.Contains(owner.ToLower()))
                    {
                        privilegeOwner = true;
                        cmbOwner.Enabled = true;
                        cmbOwnerBranch.Enabled = true;
                        cmbDelegateLead.Enabled = true;
                        cmbDelegateBranch.Enabled = true;
                    }
                    else
                    {
                        cmbOwner.Enabled = false;
                        cmbOwnerBranch.Enabled = false;
                    }
                }

                if (!privilegeOwner)
                {
                    if (cmbDelegateLead.Items.Count > 0 && !string.IsNullOrEmpty(delegateLead))
                    {
                        //ถ้าเป็น Delegate หรือหัวหน้า Delegate จะเปิดให้สามารถแก้ไข Delegate ได้
                        if (recusiveList.Contains(delegateLead.ToLower()))
                        {
                            cmbDelegateLead.Enabled = true;
                            cmbDelegateBranch.Enabled = true;
                        }
                        else
                        {
                            cmbDelegateLead.Enabled = false;
                            cmbDelegateBranch.Enabled = false;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static string GetCallCalculatorScript(string ticketId, string url)
        {
            string script = @"var form = document.createElement('form');
                                    var input_ticketid = document.createElement('input');
                                    var input_username = document.createElement('input');

                                    form.action = '" + url + @"';
                                    form.method = 'post';
                                    form.setAttribute('target', '_blank');

                                    input_ticketid.name = 'ticketid';
                                    input_ticketid.value = '" + ticketId + @"';
                                    form.appendChild(input_ticketid);

                                    input_username.name = 'username';
                                    input_username.value = '" + HttpContext.Current.User.Identity.Name + @"';
                                    form.appendChild(input_username);

                                    document.body.appendChild(form);
                                    form.submit();

                                    document.body.removeChild(form);";

            return script;
        }

        public static string GetCallAdamScript(LeadDataForAdam leadData, string username, string userId, bool selfRedirectToViewLead, string configProductScreenType)
        {
            string script = @"var popUp = window.open('test.aspx');
                                if (popUp == null || typeof(popUp)=='undefined') { 	
	                                    alert('ไม่สามารถแนบเอกสารต่อได้ รบกวนติดต่อ IT Help Desk (Popup Blocker)'); 
                                ";

            if (selfRedirectToViewLead)
                script += " document.location='SLM_SCR_003.aspx'; }";
            else
                script += " }";


            script += @" else
                        {
                            popUp.close(); 

                            var form = document.createElement('form');
                            var ticketid = document.createElement('input');
                            var username = document.createElement('input');
                            var userid = document.createElement('input');
                            var productgroupid = document.createElement('input');
                            var productgroupname = document.createElement('input');
                            var productid = document.createElement('input');
                            var productname = document.createElement('input');
                            var campaign = document.createElement('input');
                            var campaignname = document.createElement('input');
                            var firstname = document.createElement('input');
                            var lastname = document.createElement('input');
                            var telno1 = document.createElement('input');
                            var telno2 = document.createElement('input');
                            var telno3 = document.createElement('input');
                            var extno1 = document.createElement('input');
                            var extno2 = document.createElement('input');
                            var extno3 = document.createElement('input');
                            var email = document.createElement('input');
                            var buildingname = document.createElement('input');
                            var addrno = document.createElement('input');
                            var floor = document.createElement('input');
                            var soi = document.createElement('input');
                            var street = document.createElement('input');
                            var tambol = document.createElement('input');
                            var amphur = document.createElement('input');
                            var province = document.createElement('input');
                            var postalcode = document.createElement('input');
                            var occupation = document.createElement('input');
                            var basesalary = document.createElement('input');
                            var iscustomer = document.createElement('input');
                            var customercode = document.createElement('input');
                            var dateofbirth = document.createElement('input');
                            var cid = document.createElement('input');
                            var leadstatus = document.createElement('input');
                            var topic = document.createElement('input');
                            var detail = document.createElement('input');
                            var pathlink = document.createElement('input');
                            var telesalename = document.createElement('input');
                            var availabletime = document.createElement('input');
                            var contactbranch = document.createElement('input');
                            var interestedprodandtype = document.createElement('input');
                            var licenseno = document.createElement('input');
                            var yearofcar = document.createElement('input');
                            var yearofcarregis = document.createElement('input');
                            var registerprovince = document.createElement('input');
                            var brand = document.createElement('input');
                            var model = document.createElement('input');
                            var submodel = document.createElement('input');
                            var downpayment = document.createElement('input');
                            var downpercent = document.createElement('input');
                            var carprice = document.createElement('input');
                            var financeamt = document.createElement('input');
                            var term = document.createElement('input');
                            var paymenttype = document.createElement('input');
                            var balloonamt = document.createElement('input');
                            var balloonpercent = document.createElement('input');
                            var plantype = document.createElement('input');
                            var coveragedate = document.createElement('input');
                            var acctype = document.createElement('input');
                            var accpromotion = document.createElement('input');
                            var accterm = document.createElement('input');
                            var interest = document.createElement('input');
                            var invest = document.createElement('input');
                            var loanod = document.createElement('input');
                            var loanodterm = document.createElement('input');
                            var slmbank = document.createElement('input');
                            var slmatm = document.createElement('input');
                            var otherdetail1 = document.createElement('input');
                            var otherdetail2 = document.createElement('input');
                            var otherdetail3 = document.createElement('input');
                            var otherdetail4 = document.createElement('input');
                            var cartype = document.createElement('input');
                            var channelid = document.createElement('input');
                            var date = document.createElement('input');
                            var time = document.createElement('input');
                            var createuser = document.createElement('input');
                            var ipaddress = document.createElement('input');
                            var company = document.createElement('input');
                            var branch = document.createElement('input');
                            var branchno = document.createElement('input');
                            var machineno = document.createElement('input');
                            var clientservicetype = document.createElement('input');
                            var documentno = document.createElement('input');
                            var commpaidcode = document.createElement('input');
                            var zone = document.createElement('input');
                            var transid = document.createElement('input');
                            var tambolname = document.createElement('input');
                            var amphurname = document.createElement('input');
                            var provincename = document.createElement('input');

                            form.action = '" + leadData.AdamsUrl + @"';
                            form.method = 'post';
                            form.setAttribute('target', '" + leadData.TicketId + @"');

                            ticketid.name = 'ticketid';
                            ticketid.value = '" + leadData.TicketId + @"';
                            form.appendChild(ticketid);

                            username.name = 'username';
                            username.value = '" + username + @"';
                            form.appendChild(username);

                            userid.name = 'userid';
                            userid.value = '" + userId + @"';
                            form.appendChild(userid);

                            productgroupid.name = 'productgroupid';
                            productgroupid.value = '" + leadData.ProductGroupId + @"';
                            form.appendChild(productgroupid);

                            productgroupname.name = 'productgroupname';
                            productgroupname.value = '" + leadData.ProductGroupName + @"';
                            form.appendChild(productgroupname);

                            productid.name = 'productid';
                            productid.value = '" + leadData.ProductId + @"';
                            form.appendChild(productid);

                            productname.name = 'productname';
                            productname.value = '" + leadData.ProductName + @"';
                            form.appendChild(productname);

                            campaign.name = 'campaign';
                            campaign.value = '" + leadData.Campaign + @"';
                            form.appendChild(campaign);

                            campaignname.name = 'campaignname';
                            campaignname.value = '" + leadData.CampaignName + @"';
                            form.appendChild(campaignname);

                            firstname.name = 'firstname';
                            firstname.value = '" + (leadData.Firstname != null ? leadData.Firstname.Replace("'", "") : "") + @"';
                            form.appendChild(firstname);

                            lastname.name = 'lastname';
                            lastname.value = '" + (leadData.Lastname != null ? leadData.Lastname.Replace("'", "") : "") + @"';
                            form.appendChild(lastname);

                            telno1.name = 'telno1';
                            telno1.value = '" + leadData.TelNo1 + @"';
                            form.appendChild(telno1);

                            telno2.name = 'telno2';
                            telno2.value = '" + leadData.TelNo2 + @"';
                            form.appendChild(telno2);

                            telno3.name = 'telno3';
                            telno3.value = '" + leadData.TelNo3 + @"';
                            form.appendChild(telno3);

                            extno1.name = 'extno1';
                            extno1.value = '" + leadData.ExtNo1 + @"';
                            form.appendChild(extno1);

                            extno2.name = 'extno2';
                            extno2.value = '" + leadData.ExtNo2 + @"';
                            form.appendChild(extno2);

                            extno3.name = 'extno3';
                            extno3.value = '" + leadData.ExtNo3 + @"';
                            form.appendChild(extno3);

                            email.name = 'email';
                            email.value = '" + leadData.Email + @"';
                            form.appendChild(email);

                            buildingname.name = 'buildingname';
                            buildingname.value = '" + leadData.BuildingName + @"';
                            form.appendChild(buildingname);

                            addrno.name = 'addrno';
                            addrno.value = '" + leadData.AddrNo + @"';
                            form.appendChild(addrno);
    
                            floor.name = 'floor';
                            floor.value = '" + leadData.Floor + @"';
                            form.appendChild(floor);

                            soi.name = 'soi';
                            soi.value = '" + leadData.Soi + @"';
                            form.appendChild(soi);

                            street.name = 'street';
                            street.value = '" + leadData.Street + @"';
                            form.appendChild(street);

                            tambol.name = 'tambol';
                            tambol.value = '" + leadData.TambolCode + @"';
                            form.appendChild(tambol);

                            tambolname.name = 'tambolname';
                            tambolname.value = '" + leadData.TambolName + @"';
                            form.appendChild(tambolname);

                            amphur.name = 'amphur';
                            amphur.value = '" + leadData.AmphurCode + @"';
                            form.appendChild(amphur);

                            amphurname.name = 'amphurname';
                            amphurname.value = '" + leadData.AmphurName + @"';
                            form.appendChild(amphurname);

                            province.name = 'province';
                            province.value = '" + leadData.ProvinceCode + @"';
                            form.appendChild(province);

                            provincename.name = 'provincename';
                            provincename.value = '" + leadData.ProvinceName + @"';
                            form.appendChild(provincename);

                            postalcode.name = 'postalcode';
                            postalcode.value = '" + leadData.PostalCode + @"';
                            form.appendChild(postalcode);

                            occupation.name = 'occupation';
                            occupation.value = '" + leadData.OccupationCode + @"';
                            form.appendChild(occupation);

                            basesalary.name = 'basesalary';
                            basesalary.value = '" + (leadData.BaseSalary != null ? leadData.BaseSalary.Value.ToString() : "") + @"';
                            form.appendChild(basesalary);

                            iscustomer.name = 'iscustomer';
                            iscustomer.value = '" + leadData.IsCustomer + @"';
                            form.appendChild(iscustomer);
    
                            customercode.name = 'customercode';
                            customercode.value = '" + leadData.CustomerCode + @"';
                            form.appendChild(customercode);

                            dateofbirth.name = 'dateofbirth';
                            dateofbirth.value = '" + (leadData.DateOfBirth != null ? leadData.DateOfBirth.Value.Year.ToString() + leadData.DateOfBirth.Value.ToString("MMdd") : "") + @"';
                            form.appendChild(dateofbirth);

                            cid.name = 'cid';
                            cid.value = '" + leadData.Cid + @"';
                            form.appendChild(cid);

                            leadstatus.name = 'status';
                            leadstatus.value = '" + leadData.Status + @"';
                            form.appendChild(leadstatus);

                            topic.name = 'topic';
                            topic.value = '" + leadData.Topic + @"';
                            form.appendChild(topic);

                            detail.name = 'detail';
                            detail.value = '" + (leadData.Detail != null ? leadData.Detail.Replace("\n", "").Replace("'", "") : "") + @"';
                            form.appendChild(detail);

                            pathlink.name = 'pathlink';
                            pathlink.value = '" + leadData.PathLink + @"';
                            form.appendChild(pathlink);

                            telesalename.name = 'telesalename';
                            telesalename.value = '" + leadData.TelesaleName + @"';
                            form.appendChild(telesalename);

                            availabletime.name = 'availabletime';
                            availabletime.value = '" + leadData.AvailableTime + @"';
                            form.appendChild(availabletime);

                            contactbranch.name = 'contactbranch';
                            contactbranch.value = '" + leadData.ContactBranch + @"';
                            form.appendChild(contactbranch);

                            interestedprodandtype.name = 'interestedprodandtype';
                            interestedprodandtype.value = '" + leadData.InterestedProdAndType + @"';
                            form.appendChild(interestedprodandtype);

                            licenseno.name = 'licenseno';
                            licenseno.value = '" + leadData.LicenseNo + @"';
                            form.appendChild(licenseno);

                            yearofcar.name = 'yearofcar';
                            yearofcar.value = '" + leadData.YearOfCar + @"';
                            form.appendChild(yearofcar);
                                
                            yearofcarregis.name = 'yearofcarregis';
                            yearofcarregis.value = '" + leadData.YearOfCarRegis + @"';
                            form.appendChild(yearofcarregis);

                            registerprovince.name = 'registerprovince';
                            registerprovince.value = '" + leadData.RegisterProvinceCode + @"';
                            form.appendChild(registerprovince);

                            brand.name = 'brand';
                            brand.value = '" + leadData.BrandCode + @"';
                            form.appendChild(brand);

                            model.name = 'model';
                            model.value = '" + leadData.ModelFamily + @"';
                            form.appendChild(model);

                            submodel.name = 'submodel';
                            submodel.value = '" + leadData.SubmodelRedBookNo + @"';
                            form.appendChild(submodel);
            
                            downpayment.name = 'downpayment';
                            downpayment.value = '" + (leadData.DownPayment != null ? leadData.DownPayment.Value.ToString() : "") + @"';
                            form.appendChild(downpayment);

                            downpercent.name = 'downpercent';
                            downpercent.value = '" + (leadData.DownPercent != null ? leadData.DownPercent.Value.ToString() : "") + @"';
                            form.appendChild(downpercent);

                            carprice.name = 'carprice';
                            carprice.value = '" + (leadData.CarPrice != null ? leadData.CarPrice.Value.ToString() : "") + @"';
                            form.appendChild(carprice);

                            financeamt.name = 'financeamt';
                            financeamt.value = '" + (leadData.FinanceAmt != null ? leadData.FinanceAmt.Value.ToString() : "") + @"';
                            form.appendChild(financeamt);

                            term.name = 'term';
                            term.value = '" + leadData.Term + @"';
                            form.appendChild(term);

                            paymenttype.name = 'paymenttype';
                            paymenttype.value = '" + leadData.PaymentTypeCode + @"';
                            form.appendChild(paymenttype);

                            balloonamt.name = 'balloonamt';
                            balloonamt.value = '" + (leadData.BalloonAmt != null ? leadData.BalloonAmt.Value.ToString() : "") + @"';
                            form.appendChild(balloonamt);

                            balloonpercent.name = 'balloonpercent';
                            balloonpercent.value = '" + (leadData.BalloonPercent != null ? leadData.BalloonPercent.Value.ToString() : "") + @"';
                            form.appendChild(balloonpercent);

                            plantype.name = 'plantype';
                            plantype.value = '" + leadData.Plantype + @"';
                            form.appendChild(plantype);

                            coveragedate.name = 'coveragedate';
                            coveragedate.value = '" + leadData.CoverageDate + @"';
                            form.appendChild(coveragedate);

                            acctype.name = 'acctype';
                            acctype.value = '" + leadData.AccTypeCode + @"';
                            form.appendChild(acctype);

                            accpromotion.name = 'accpromotion';
                            accpromotion.value = '" + leadData.AccPromotionCode + @"';
                            form.appendChild(accpromotion);
                
                            accterm.name = 'accterm';
                            accterm.value = '" + leadData.AccTerm + @"';
                            form.appendChild(accterm);
                        
                            interest.name = 'interest';
                            interest.value = '" + leadData.Interest + @"';
                            form.appendChild(interest);

                            invest.name = 'invest';
                            invest.value = '" + leadData.Invest + @"';
                            form.appendChild(invest);

                            loanod.name = 'loanod';
                            loanod.value = '" + leadData.LoanOd + @"';
                            form.appendChild(loanod);

                            loanodterm.name = 'loanodterm';
                            loanodterm.value = '" + leadData.LoanOdTerm + @"';
                            form.appendChild(loanodterm);

                            slmbank.name = 'slmbank';
                            slmbank.value = '" + leadData.SlmBank + @"';
                            form.appendChild(slmbank);
        
                            slmatm.name = 'slmatm';
                            slmatm.value = '" + leadData.SlmAtm + @"';
                            form.appendChild(slmatm);

                            otherdetail1.name = 'otherdetail1';
                            otherdetail1.value = '" + leadData.OtherDetail1 + @"';
                            form.appendChild(otherdetail1);

                            otherdetail2.name = 'otherdetail2';
                            otherdetail2.value = '" + leadData.OtherDetail2 + @"';
                            form.appendChild(otherdetail2);

                            otherdetail3.name = 'otherdetail3';
                            otherdetail3.value = '" + leadData.OtherDetail3 + @"';
                            form.appendChild(otherdetail3);

                            otherdetail4.name = 'otherdetail4';
                            otherdetail4.value = '" + leadData.OtherDetail4 + @"';
                            form.appendChild(otherdetail4);

                            cartype.name = 'cartype';
                            cartype.value = '" + leadData.CarType + @"';
                            form.appendChild(cartype);

                            channelid.name = 'channelid';
                            channelid.value = '" + leadData.ChannelId + @"';
                            form.appendChild(channelid);

                            date.name = 'date';
                            date.value = '" + (leadData.RequestDate != null ? leadData.RequestDate.Value.Year.ToString() + leadData.RequestDate.Value.ToString("MMdd") : "") + @"';
                            form.appendChild(date);

                            time.name = 'time';
                            time.value = '" + (leadData.RequestDate != null ? leadData.RequestDate.Value.ToString("HHmmss") : "") + @"';
                            form.appendChild(time);

                            createuser.name = 'createuser';
                            createuser.value = '" + leadData.CreateUser + @"';
                            form.appendChild(createuser);
                        
                            ipaddress.name = 'ipaddress';
                            ipaddress.value = '" + leadData.Ipaddress + @"';
                            form.appendChild(ipaddress);

                            company.name = 'company';
                            company.value = '" + leadData.Company + @"';
                            form.appendChild(company);

                            branch.name = 'branch';
                            branch.value = '" + leadData.Branch + @"';
                            form.appendChild(branch);

                            branchno.name = 'branchno';
                            branchno.value = '" + leadData.BranchNo + @"';
                            form.appendChild(branchno);

                            machineno.name = 'machineno';
                            machineno.value = '" + leadData.MachineNo + @"';
                            form.appendChild(machineno);

                            clientservicetype.name = 'clientservicetype';
                            clientservicetype.value = '" + leadData.ClientServiceType + @"';
                            form.appendChild(clientservicetype);

                            documentno.name = 'documentno';
                            documentno.value = '" + leadData.DocumentNo + @"';
                            form.appendChild(documentno);

                            commpaidcode.name = 'commpaidcode';
                            commpaidcode.value = '" + leadData.CommPaidCode + @"';
                            form.appendChild(commpaidcode);

                            zone.name = 'zone';
                            zone.value = '" + leadData.Zone + @"';
                            form.appendChild(zone);

                            transid.name = 'transid';
                            transid.value = '" + leadData.TransId + @"';
                            form.appendChild(transid);

                            document.body.appendChild(form);
                            form.submit();

                            document.body.removeChild(form); ";

            if (selfRedirectToViewLead)
                script += " document.location='SLM_SCR_004.aspx?ticketid=" + leadData.TicketId + "&type=" + configProductScreenType + @"'; }";
            else
                script += " } ";

            return script;
        }

        public static string GetCallAolSummaryReportScript(string appNo, string empCode, string empTitle, string privilegeNCB)
        {
            string script = @"var form = document.createElement('form');
                                    var app_no = document.createElement('input');
                                    var emp_code = document.createElement('input');
                                    var emp_title = document.createElement('input');
                                    var privilege_ncb = document.createElement('input');

                                    form.action = '" + System.Configuration.ConfigurationManager.AppSettings["AolSummaryReportlUrl"] + @"';
                                    form.method = 'post';
                                    form.setAttribute('target', '_blank');

                                    app_no.name = 'app_no';
                                    app_no.value = '" + appNo + @"';
                                    form.appendChild(app_no);

                                    emp_code.name = 'emp_code';
                                    emp_code.value = '" + empCode + @"';
                                    form.appendChild(emp_code);

                                    emp_title.name = 'emp_title';
                                    emp_title.value = '" + empTitle + @"';
                                    form.appendChild(emp_title);

                                    privilege_ncb.name = 'privilege_ncb';
                                    privilege_ncb.value = '" + privilegeNCB + @"';
                                    form.appendChild(privilege_ncb);

                                    document.body.appendChild(form);
                                    form.submit();

                                    document.body.removeChild(form);";

            return script;
        }

        public static string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".htm":
                case ".html":
                case ".log":
                    return "text/HTML";
                case ".txt":
                    return "text/plain";
                case ".doc":
                case ".docx":
                    return "application/ms-word";
                case ".tiff":
                case ".tif":
                    return "image/tiff";
                case ".asf":
                    return "video/x-ms-asf";
                case ".avi":
                    return "video/avi";
                case ".zip":
                    return "application/zip";
                case ".xls":
                case ".xlsx":
                case ".csv":
                    return "application/vnd.ms-excel";
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".bmp":
                    return "image/bmp";
                case "png":
                    return "image/png";
                case ".wav":
                    return "audio/wav";
                case ".mp3":
                    return "audio/mpeg3";
                case ".mpg":
                case "mpeg":
                    return "video/mpeg";
                case ".rtf":
                    return "application/rtf";
                case ".asp":
                    return "text/asp";
                case ".pdf":
                    return "application/pdf";
                case ".fdf":
                    return "application/vnd.fdf";
                case ".ppt":
                    return "application/mspowerpoint";
                case ".dwg":
                    return "image/vnd.dwg";
                case ".msg":
                    return "application/msoutlook";
                case ".xml":
                case ".sdxl":
                    return "application/xml";
                case ".xdp":
                    return "application/vnd.adobe.xdp+xml";
                default:
                    return "application/octet-stream";
            }
        }

        public static string GenerateXml(LeadData data)
        {
            XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("ticket", new XAttribute("id", data.TicketId != null ? data.TicketId : string.Empty),
                        new XElement("mandatory", new XElement("firstname", data.Name != null ? data.Name : string.Empty),
                                                    new XElement("telNo1", data.TelNo_1 != null ? data.TelNo_1 : string.Empty),
                                                    new XElement("campaign", data.CampaignId != null ? data.CampaignId : string.Empty)),
                        new XElement("customerInfo", new XElement("lastname", data.LastName != null ? data.LastName : string.Empty),
                                                        new XElement("email", data.Email != null ? data.Email : string.Empty),
                                                        new XElement("telNo2", data.TelNo_2 != null ? data.TelNo_2 : string.Empty),
                                                        new XElement("telNo3", data.TelNo_3 != null ? data.TelNo_3 : string.Empty),
                                                        new XElement("extNo1", data.Ext_1 != null ? data.Ext_1 : string.Empty),
                                                        new XElement("extNo2", data.Ext_2 != null ? data.Ext_2 : string.Empty),
                                                        new XElement("extNo3", data.Ext_3 != null ? data.Ext_3 : string.Empty),
                                                        new XElement("BuildingName", data.BuildingName != null ? data.BuildingName : string.Empty),
                                                        new XElement("addrNo", data.AddressNo != null ? data.AddressNo : string.Empty),
                                                        new XElement("floor", data.Floor != null ? data.Floor : string.Empty),
                                                        new XElement("soi", data.Soi != null ? data.Soi : string.Empty),
                                                        new XElement("street", data.Street != null ? data.Street : string.Empty),
                                                        new XElement("tambol", data.TambolCode != null ? data.TambolCode : string.Empty),
                                                        new XElement("amphur", data.AmphurCode != null ? data.AmphurCode : string.Empty),
                                                        new XElement("province", data.ProvinceCode != null ? data.ProvinceCode : string.Empty),
                                                        new XElement("postalCode", data.PostalCode != null ? data.PostalCode : string.Empty),
                                                        new XElement("occupation", data.OccupationCode != null ? data.OccupationCode : string.Empty),
                                                        new XElement("baseSalary", data.BaseSalary != null ? data.BaseSalary.Value.ToString("0.00") : string.Empty),
                                                        new XElement("isCustomer", data.IsCustomer != null ? data.IsCustomer : string.Empty),
                                                        new XElement("customerCode", data.CusCode != null ? data.CusCode : string.Empty),
                                                        new XElement("dateOfBirth", data.Birthdate != null ? data.Birthdate.Value.Year + data.Birthdate.Value.ToString("MMdd") : string.Empty),
                                                        new XElement("cid", data.CitizenId != null ? data.CitizenId : string.Empty)),
                        new XElement("customerDetail", new XElement("topic", data.Topic != null ? data.Topic : string.Empty),
                                                        new XElement("detail", data.Detail != null ? data.Detail : string.Empty),
                                                        new XElement("pathLink", data.PathLink != null ? data.PathLink : string.Empty),
                                                        new XElement("telesaleName", data.Owner != null ? data.Owner : string.Empty),
                                                        new XElement("availableTime", data.AvailableTime != null ? data.AvailableTime : string.Empty),
                                                        new XElement("contactBranch", data.ContactBranch != null ? data.ContactBranch : string.Empty)),
                        new XElement("productInfo", new XElement("interestedProdAndType", data.InterestedProd != null ? data.InterestedProd : string.Empty),
                                                    new XElement("licenseNo", data.LicenseNo != null ? data.LicenseNo : string.Empty),
                                                    new XElement("yearOfCar", data.YearOfCar != null ? data.YearOfCar : string.Empty),
                                                    new XElement("yearOfCarRegis", data.YearOfCarRegis != null ? data.YearOfCarRegis : string.Empty),
                                                    new XElement("registerProvince", data.ProvinceRegisCode != null ? data.ProvinceRegisCode : string.Empty),
                                                    new XElement("brand", data.BrandCode != null ? data.BrandCode : string.Empty),
                                                    new XElement("model", data.ModelFamily != null ? data.ModelFamily : string.Empty),
                                                    new XElement("submodel", data.SubModelCode != null ? data.SubModelCode : string.Empty),
                                                    new XElement("downPayment", data.DownPayment != null ? data.DownPayment.Value.ToString("0.00") : string.Empty),
                                                    new XElement("downPercent", data.DownPercent != null ? data.DownPercent.Value.ToString("0.00") : string.Empty),
                                                    new XElement("carPrice", data.CarPrice != null ? data.CarPrice.Value.ToString("0.00") : string.Empty),
                                                    new XElement("financeAmt", data.FinanceAmt != null ? data.FinanceAmt.Value.ToString("0.00") : string.Empty),
                                                    new XElement("term", data.PaymentTerm != null ? data.PaymentTerm : string.Empty),
                                                    new XElement("paymentType", data.PaymentTypeCode != null ? data.PaymentTypeCode : string.Empty),
                                                    new XElement("balloonAmt", data.BalloonAmt != null ? data.BalloonAmt.Value.ToString("0.00") : string.Empty),
                                                    new XElement("balloonPercent", data.BalloonPercent != null ? data.BalloonPercent.Value.ToString("0.00") : string.Empty),
                                                    new XElement("plantype", data.PlanType != null ? data.PlanType : string.Empty),
                                                    new XElement("coverageDate", data.CoverageDate != null ? data.CoverageDate : string.Empty),
                                                    new XElement("accType", data.AccTypeCode != null ? data.AccTypeCode : string.Empty),
                                                    new XElement("accPromotion", data.AccPromotionCode != null ? data.AccPromotionCode : string.Empty),
                                                    new XElement("accTerm", data.AccTerm != null ? data.AccTerm : string.Empty),
                                                    new XElement("interest", data.Interest != null ? data.Interest : string.Empty),
                                                    new XElement("invest", data.Invest != null ? data.Invest : string.Empty),
                                                    new XElement("loanOd", data.LoanOd != null ? data.LoanOd : string.Empty),
                                                    new XElement("loanOdTerm", data.LoanOdTerm != null ? data.LoanOdTerm : string.Empty),
                                                    new XElement("slmBank", data.Ebank != null ? data.Ebank : string.Empty),
                                                    new XElement("slmAtm", data.Atm != null ? data.Atm : string.Empty),
                                                    new XElement("otherDetail1", data.OtherDetail_1 != null ? data.OtherDetail_1 : string.Empty),
                                                    new XElement("otherDetail2", data.OtherDetail_2 != null ? data.OtherDetail_2 : string.Empty),
                                                    new XElement("otherDetail3", data.OtherDetail_3 != null ? data.OtherDetail_3 : string.Empty),
                                                    new XElement("otherDetail4", data.OtherDetail_4 != null ? data.OtherDetail_4 : string.Empty),
                                                    new XElement("carType", data.CarType != null ? data.CarType : string.Empty)),
                        new XElement("channelInfo", new XElement("channelId", data.ChannelId != null ? data.ChannelId : string.Empty),
                                                    new XElement("date", data.RequestDate != null ? data.RequestDate.Value.Year + data.RequestDate.Value.ToString("MMdd") : string.Empty),
                                                    new XElement("time", data.RequestTime != null ? data.RequestTime : string.Empty),
                                                    new XElement("createUser", HttpContext.Current.User.Identity.Name),
                                                    new XElement("ipaddress", data.IPAddress != null ? data.IPAddress : string.Empty),
                                                    new XElement("company", data.Company != null ? data.Company : string.Empty),
                                                    new XElement("branch", data.Branch != null ? data.Branch : string.Empty),
                                                    new XElement("branchNo", data.BranchNo != null ? data.BranchNo : string.Empty),
                                                    new XElement("machineNo", data.MachineNo != null ? data.MachineNo : string.Empty),
                                                    new XElement("clientServiceType", data.ClientServiceType != null ? data.ClientServiceType : string.Empty),
                                                    new XElement("documentNo", data.DocumentNo != null ? data.DocumentNo : string.Empty),
                                                    new XElement("commPaidCode", data.CommPaidCode != null ? data.CommPaidCode : string.Empty),
                                                    new XElement("zone", data.Zone != null ? data.Zone : string.Empty),
                                                    new XElement("transid", data.TransId != null ? data.TransId : string.Empty))
                        )
                    );

            return doc.ToString();
        }

        public static string GetAssemblyVersion()
        {
            try
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString();
            }
            catch
            {
                throw;
            }
        }

        public static int SafeInt(string val)
        {
            int d; int.TryParse(val, out d);
            return d;
        }

        public static decimal SafeDecimal(string val)
        {
            decimal d; decimal.TryParse(val, out d);
            return d;
        }

        public static List<SLM.Resource.Data.NextLeadData> GetNextLeadList(List<SearchLeadResult> list, int pageIndex)
        {
            try
            {
                List<SLM.Resource.Data.NextLeadData> ticketIdList = new List<SLM.Resource.Data.NextLeadData>();
                var newList = list.Skip(pageIndex * SLMConstant.GridviewPageSize).Take(200).ToList();

                newList.ForEach(lead => {
                    if (!string.IsNullOrEmpty(lead.TicketId))
                    {
                        ticketIdList.Add(new NextLeadData() { TicketId = lead.TicketId, PreleadId = lead.PreleadId, CampaignId = lead.CampaignId, ProductId = lead.ProductId });
                    }
                });

                return ticketIdList;
            }
            catch
            {
                return null;
            }
        }

        public static List<SLM.Resource.Data.NextLeadData> GetNextLeadList(List<SearchObtResult> list, int pageIndex)
        {
            try
            {
                List<SLM.Resource.Data.NextLeadData> idList = new List<SLM.Resource.Data.NextLeadData>();
                var newList = list.Skip(pageIndex * SLMConstant.GridviewPageSize).Take(200).ToList();

                newList.ForEach(result => {
                    idList.Add(new NextLeadData() { TicketId = result.TicketId, PreleadId = result.PreleadId, CampaignId = result.CampaignId, ProductId = result.ProductId });
                });

                return idList;
            }
            catch
            {
                return null;
            }
        }

        // added by OUMz 26/5/2016
        public static void SetComboValue(DropDownList cmb, string value)
        {
            cmb.SelectedIndex = cmb.Items.IndexOf(cmb.Items.FindByValue(value));
        }

        // added by OuMz 1/6/2016
        public static void BuildCombo(DropDownList cmb, List<ControlListData> lst) { BuildCombo(cmb, lst, "", ""); }
        public static void BuildCombo(DropDownList cmb, List<ControlListData> lst, string Blanktext) { BuildCombo(cmb, lst, Blanktext, ""); }
        public static void BuildCombo(DropDownList cmb, List<ControlListData> lst, string Blanktext, string Value)
        {
            cmb.DataSource = lst;
            cmb.DataTextField = "TextField";
            cmb.DataValueField = "ValueField";
            cmb.DataBind();
            cmb.Items.Insert(0, new ListItem(Blanktext, Value));
        }

        public static void SetAutoCompleteDropdown(DropDownList[] lst, Page pg, string scriptkey)
        {
            string script = "";
            foreach (var dd in lst)
            {
                script += @"
    Sys.Application.add_load(function () {
        $('#" + dd.ClientID + @"').combobox({ select: function(event, ui) { DoPostBack(this); }
            , notfound: function (event) { " + pg.ClientScript.GetPostBackEventReference(dd, "-1") + @" }
            , cleared: function () { " + pg.ClientScript.GetPostBackEventReference(dd, "-1") + @" }
        });
    });
";
            }

            if (script != "")
            {
                ScriptManager.RegisterStartupScript(pg, pg.GetType(), scriptkey, script, true);
                //ScriptManager.RegisterClientScriptBlock(pg, pg.GetType(), scriptkey, script, true);
            }

            //if (script != "" && !pg.ClientScript.IsClientScriptBlockRegistered(scriptkey))
            //{
            //    //pg.ClientScript.RegisterClientScriptBlock(pg.GetType(), scriptkey, script, true);
            //}
        }

        public static void CreateCASActivityLog(Page sender, string ticketID, decimal rewnewInsurID, bool policyPurchased, bool actPurchased, decimal? current_paid_policy, decimal? current_paid_act, decimal? total_paid_policy, decimal? total_paid_act, StaffData staff)
        {
            try
            {
                //var staff = StaffBiz.GetStaff(sender.User.Identity.Name);
                var data = CARServiceBiz.GetDataForCARLogService(ticketID, "");
                string preleadId = "";
                string actSendDate = "";

                if (data != null)
                {
                    preleadId = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
                    actSendDate = data.ActSendDate != null ? (data.ActSendDate.Value.ToString("dd-MM-") + data.ActSendDate.Value.Year.ToString()) : "";
                }

                string incentiveValue = "";
                string incentiveActValue = "";
                //decimal? paidPolicy = null;
                //decimal? paidAct = null;
                decimal? currentPaidPolicy = null;
                decimal? currentPaidAct = null;
                decimal? totalPaidPolicy = total_paid_policy;
                decimal? totalPaidAct = total_paid_act;

                if (policyPurchased)
                {
                    incentiveValue = data != null ? (data.IncentiveFlag == true ? "Y" : "N") : "N";
                    currentPaidPolicy = current_paid_policy;
                    // totalPaidPolicy = total_paid_policy;
                    if (current_paid_act == -1) incentiveValue = null;
                    //paidPolicy = GetPolicyAmountPaid(slmdb, reins.slm_TicketId, AppConstant.PaymentCode.Policy);     //ประกัน
                }
                if (actPurchased)
                {
                    incentiveActValue = data != null ? (data.IncentiveFlagAct == true ? "Y" : "N") : "N";
                    currentPaidAct = current_paid_act;
                    // totalPaidAct = total_paid_act;
                    if (current_paid_policy == -1) incentiveActValue = null;
                    //paidAct = GetPolicyAmountPaid(slmdb, reins.slm_TicketId, AppConstant.PaymentCode.Act);        //พรบ
                }

                //Activity Info
                List<CARService.DataItem> actInfoList = new List<CARService.DataItem>();
                actInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่รับแจ้ง", DataValue = data != null ? data.ReceiveNo : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "วันที่ส่งแจ้ง พรบ.", DataValue = actSendDate });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "Incentive ประกัน", DataValue = incentiveValue });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 4, DataLabel = "Incentive พรบ.", DataValue = incentiveActValue });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 5, DataLabel = "ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้", DataValue = totalPaidPolicy <= 0 ? "" : currentPaidPolicy != null ? currentPaidPolicy.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 6, DataLabel = "ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้", DataValue = totalPaidAct <= 0 ? "" : currentPaidAct != null ? currentPaidAct.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 7, DataLabel = "ค่าเบี้ยประกันที่ลูกค้าชำระรวม", DataValue = totalPaidPolicy <= 0 ? "" : totalPaidPolicy != null ? totalPaidPolicy.Value.ToString("#,##0.00") : "" });
                actInfoList.Add(new CARService.DataItem() { SeqNo = 8, DataLabel = "ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม", DataValue = totalPaidAct <= 0 ? "" : totalPaidAct != null ? totalPaidAct.Value.ToString("#,##0.00") : "" });

                //Customer Info
                List<CARService.DataItem> cusInfoList = new List<CARService.DataItem>();
                cusInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = data != null ? data.CitizenId : "" });
                cusInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = data != null ? data.CardTypeName : "" });
                cusInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = data != null ? data.CustomerName : "" });

                //Product Info
                List<CARService.DataItem> prodInfoList = new List<CARService.DataItem>();
                prodInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = data != null ? data.ProductGroupName : "" });
                prodInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = data != null ? data.ProductName : "" });
                prodInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = data != null ? data.CampaignName : "" });

                //Contract Info
                List<CARService.DataItem> contInfoList = new List<CARService.DataItem>();
                contInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
                contInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = preleadId != "" ? "HP" : "" });
                contInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.LicenseNo : "" });

                //Officer Info
                List<CARService.DataItem> offInfoList = new List<CARService.DataItem>();
                offInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff != null ? staff.StaffNameTH : "SYSTEM" });

                CARService.CARServiceData logdata = new CARService.CARServiceData()
                {
                    ReferenceNo = rewnewInsurID.ToString(),// reins.slm_RenewInsureId.ToString(),
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
                    ChannelId = data != null ? data.ChannelId : "",
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
                    logdata.CIFSubscriptionTypeId = data.CBSSubScriptionTypeId;

                bool ret = CARService.CreateActivityLog(logdata);//, BatchMonitorId, BatchCode, reins.slm_TicketId, logfilename);
                //AppUtil.UpdatePhonecallCASFlag(slmdb, phonecall_id, ret ? "1" : "2");
            }
            catch (Exception ex)
            {
                //AppUtil.UpdatePhonecallCASFlag(slmdb, phonecall_id, "2");

                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                var _log = LogManager.GetLogger(sender.GetType());
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("Error in Method CreateCASActivityLog: " + message, ex);

            }

        }

    }
}