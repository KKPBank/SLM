using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_035 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_035));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_035");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    ((Label)Page.Master.FindControl("lblTopic")).Text = "ข้อมูลผลประโยชน์";
                    Page.Form.DefaultButton = btnSearch.UniqueID;

                    InitialControl();
                    //2016-11-17 --> Default to Auto Search
                    BindGridview(pcTop, LoadData().ToArray(), 0);
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
//            var productList = ProductBiz.GetProductList();
            var productList = ProductBiz.GetProductListNew();
            productList.Insert(0, new Resource.Data.ControlListData() { TextField = "", ValueField = "-1" });
            cmbProductEdit.DataSource = productList;
            cmbProductEdit.DataBind();
            cmbProductSearch.DataSource = productList;
            cmbProductSearch.DataBind();

            var insComList = SlmScr035Biz.GetAllInsComListData();
            insComList.Insert(0, new Resource.Data.ControlListData() { TextField = "", ValueField = "-1" });
            cmbInsComNameEdit.DataSource = insComList;
            cmbInsComNameEdit.DataBind();
            cmbInsComName.DataSource = insComList;
            cmbInsComName.DataBind();

            textCommissionPct.Attributes.Add("OnKeyPress", "return validateDecimal(event, this);");
            textCommissionThb.Attributes.Add("OnKeyPress", "return validateDecimal(event, this);");
            textOV1Pct.Attributes.Add("OnKeyPress", "return validateDecimal(event, this);");
            textOV1Thb.Attributes.Add("OnKeyPress", "return validateDecimal(event, this);");
            textOV2Pct.Attributes.Add("OnKeyPress", "return validateDecimal(event, this);");
            textOV2Thb.Attributes.Add("OnKeyPress", "return validateDecimal(event, this);");

            ddlCoverageTypeSearch.Items.Insert(0, new ListItem("", "-1"));
            ddlInsuranceTypeSearch.Items.Insert(0, new ListItem("", "-1"));           
        }

        #region Page Control
        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex);
            pcTop.Visible = true;
            gvResult.Visible = true;
            upResult.Update();
        }
        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    BindGridview(pcTop, LoadData().ToArray(), pcTop.SelectedPageIndex);
                }
                catch (Exception ex)
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _log.Error(message);
                    AppUtil.ClientAlert(Page, message);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                //_log.Debug(message);
                //AppUtil.ClientAlert(Page, message);
            }
        }
        #endregion


        public InsuranceBenefit Benefit
        {
            get
            {                
                return GetBenefit();
            }
            set
            {                
                SetBenefit(value);
            }
        }

        private void SetBenefit(InsuranceBenefit value)
        {
            clearInput();
            hiddenBenefitId.Value = value.BenefitId.ToString();
            cmbInsComNameEdit.SelectedValue = cmbInsComNameEdit.Items.FindByValue(value.Ins_Com_Id.ToString()) == null
                ? "-1"
                : value.Ins_Com_Id.ToString();
            cmbProductEdit.SelectedValue = cmbProductEdit.Items.FindByValue(value.Product_Id) == null
                ? "-1"
                : value.Product_Id;
            updateCmbEditCampaign();
            cmbCampaignEdit.SelectedValue = cmbCampaignEdit.Items.FindByValue(value.CampaignInsuranceId.ToString()) == null
                ? "-1"
                : value.CampaignInsuranceId.ToString();

            if (value.BenefitId > 0)
            {
                cmbInsComNameEdit.Enabled = false;
                cmbProductEdit.Enabled = false;
                cmbCampaignEdit.Enabled = false;

                rdoCoverageType.Enabled = false;
                rdoInsuranceType.Enabled = false;
                ddlInsuranceType.Enabled = false;
            }
            radioCommissionPct.Checked = false;
            radioCommissionThb.Checked = false;
            switch (value.ComissionFlag)
            {
                case "1":
                    radioCommissionPct.Checked = true;
                    textCommissionPct.Text = DecimalDisplayFormat(value.ComissionPercentValue);
                    //textCommissionThb.Text = "";
                    //textCommissionPct.Enabled = true;
                    //textCommissionThb.Enabled = false;
                    break;
                default: // case "2"
                    radioCommissionThb.Checked = true;
                    textCommissionThb.Text = DecimalDisplayFormat(value.ComissionBathValue);
                    //textCommissionPct.Text = "";
                    //textCommissionPct.Enabled = false;
                    //textCommissionThb.Enabled = true;
                    break;
            }
            UpdateCommissionBox();

            

            switch (value.OV1Flag)
            {
                case "1":
                    radioOV1Pct.Checked = true;
                    textOV1Pct.Text = DecimalDisplayFormat(value.OV1PercentValue);
                    textOV1Pct.Enabled = true;
                    textOV1Thb.Text = "";
                    break;
                case "2":
                    radioOV1Thb.Checked = true;
                    textOV1Pct.Text = "";
                    textOV1Thb.Text = DecimalDisplayFormat(value.OV1BathValue);
                    textOV1Thb.Enabled = true;
                    break;
                default:
                    radioOV1Undefine.Checked = true;
                    textOV1Pct.Text = "";
                    textOV1Thb.Text = "";
                    break;
            }

            switch (value.OV2Flag)
            {
                case "1":
                    radioOV2Pct.Checked = true;
                    textOV2Pct.Text = DecimalDisplayFormat(value.OV2PercentValue.Value);
                    textOV2Pct.Enabled = true;
                    textOV2Thb.Text = "";
                    break;
                case "2":
                    radioOV2Thb.Checked = true;
                    textOV2Pct.Text = "";
                    textOV2Thb.Text = DecimalDisplayFormat(value.OV2BathValue.Value);
                    textOV2Thb.Enabled = true;
                    break;
                default:
                    radioOV2Undefine.Checked = true;
                    textOV2Pct.Text = "";
                    textOV2Thb.Text = "";
                    break;
            }

            radioInVat.Checked = true;
            radioExVat.Checked = value.VatFlag == "E";
            radioActive.Checked = !value.is_Deleted;
            radioNoActive.Checked = value.is_Deleted;

            rdoCoverageType.Checked = value.BenefitTypeCode == "204";
            rdoInsuranceType.Checked = value.BenefitTypeCode == "205";
            LoadDropDownListEdit();            
            ddlInsuranceType.SelectedValue = (value.BenefitTypeCode == "204" ? value.CoverageTypeId ?? -1 : value.BenefitTypeCode == "205" ? value.InsurancecarTypeId ?? -1 : -1).ToString();
                        
        }

        private InsuranceBenefit GetBenefit()
        {
            InsuranceBenefit b = new InsuranceBenefit();
            b.BenefitId = decimal.Parse(hiddenBenefitId.Value);
            b.Ins_Com_Id = decimal.Parse(cmbInsComNameEdit.SelectedValue);
            b.InsComName = cmbInsComNameEdit.SelectedItem.Text;
            b.Product_Id = cmbProductEdit.SelectedValue;
            b.ProductName = cmbProductEdit.SelectedItem.Text;
            b.CampaignInsuranceId = decimal.Parse(cmbCampaignEdit.SelectedValue);
            b.CampaignName = cmbCampaignEdit.SelectedItem.Text;
            if (radioCommissionPct.Checked)
            {
                b.ComissionBathValue = null;
                b.ComissionFlag = "1";
                b.ComissionPercentValue = textCommissionPct.Text == "" ? 0 : decimal.Parse(textCommissionPct.Text);
            }
            else
            {
                b.ComissionBathValue = textCommissionThb.Text == "" ? 0 : decimal.Parse(textCommissionThb.Text);
                b.ComissionFlag = "2";
                b.ComissionPercentValue = null;
            }

            if (radioOV1Thb.Checked)
            {
                b.OV1Flag = "2";
                b.OV1PercentValue = null;
                b.OV1BathValue = textOV1Thb.Text == "" ? 0 : decimal.Parse(textOV1Thb.Text);
            }
            else if (radioOV1Pct.Checked)
            {
                b.OV1Flag = "1";
                b.OV1PercentValue = textOV1Pct.Text == "" ? 0 : decimal.Parse(textOV1Pct.Text);
                b.OV1BathValue = null;
            }
            else
            {
                b.OV1Flag = "0";
                b.OV1PercentValue = null;
                b.OV1BathValue = null;
            }

            if (radioOV2Thb.Checked)
            {
                b.OV2Flag = "2";
                b.OV2PercentValue = null;
                b.OV2BathValue = textOV2Thb.Text == "" ? 0 : decimal.Parse(textOV2Thb.Text);
            }
            else if (radioOV2Pct.Checked)
            {
                b.OV2Flag = "1";
                b.OV2PercentValue = textOV2Pct.Text == "" ? 0 : decimal.Parse(textOV2Pct.Text);
                b.OV2BathValue = null;
            }
            else
            {
                b.OV2Flag = "0";
                b.OV2PercentValue = null;
                b.OV2BathValue = null;
            }

            b.VatFlag = radioInVat.Checked ? "I" : "E";
            b.is_Deleted = radioNoActive.Checked ? true : false;
            b.UpdatedBy = HttpContext.Current.User.Identity.Name;
            b.UpdatedDate = DateTime.Now;

            b.BenefitTypeCode = rdoCoverageType.Checked ? "204" : "205";
            b.CoverageTypeId = rdoCoverageType.Checked ? Convert.ToInt32(ddlInsuranceType.SelectedValue) : (int?)null;
            b.InsurancecarTypeId = rdoInsuranceType.Checked ? Convert.ToInt32(ddlInsuranceType.SelectedValue) : (int?)null;

            return b;

        }

        private void clearInput()
        {
            cmbInsComNameEdit.Enabled = true;
            cmbProductEdit.Enabled = true;
            cmbCampaignEdit.Enabled = true;

            cmbCampaignEdit.DataSource = null;
            cmbCampaignEdit.DataBind();

            hiddenBenefitId.Value = "-1";
            // cmbCampaignEdit.SelectedIndex = 0;
            cmbProductEdit.SelectedIndex = 0;
            cmbInsComNameEdit.SelectedIndex = 0;

            //radioCommissionThb.Checked = true;
            textCommissionPct.Text = "";
            textCommissionThb.Text = "";
            //radioOV1Undefine.Checked = true;
            textOV1Pct.Text = "";
            textOV1Thb.Text = "";
            //radioOV2Undefine.Checked = true;
            textOV2Pct.Text = "";
            textOV2Thb.Text = "";
            radioInVat.Checked = true;
            radioActive.Checked = true;

            radioCommissionPct.Checked = false;
            radioCommissionThb.Checked = false;
            requireCommissionThb.Enabled = false;
            requireCommissionPct.Enabled = false;

            radioOV1Pct.Checked = false;
            radioOV1Thb.Checked = false;
            radioOV1Undefine.Checked = false;

            radioOV2Pct.Checked = false;
            radioOV2Thb.Checked = false;
            radioOV2Undefine.Checked = false;

            //radioOV1_CheckedChanged(null, null);
            //radioOV2_CheckedChanged(null, null);

            rdoCoverageType.Checked = true;
            LoadDropDownListEdit();

            rdoCoverageType.Enabled = true;
            rdoInsuranceType.Enabled = true;
            ddlInsuranceType.Enabled = true;

            ClearOV1();
            ClearOV2();
            ClearCommission();
        }


        // button
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                BindGridview(pcTop, LoadData().ToArray(), 0);
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
            cmbProductSearch.SelectedIndex = 0;
            cmbInsComName.SelectedIndex = 0;
            cmbCampaign.DataSource = new ControlListData[] { new ControlListData() { ValueField = "" } };
            cmbCampaign.DataBind();
            upCampaignSearch.Update();
            BindGridview(pcTop, (new List<InsuranceBenefit>()).ToArray(), 0);
            pcTop.Visible = false;
            gvResult.Visible = false;

            chkCoverageTypeSearch.Checked = false;
            chkInsuranceTypeSearch.Checked = false;
            chkActive.Checked = false;
            chkInActive.Checked = false;

            ddlCoverageTypeSearch.Enabled = false;
            ddlCoverageTypeSearch.Items.Clear();
            ddlCoverageTypeSearch.Items.Insert(0, new ListItem("", "-1"));

            ddlInsuranceTypeSearch.Enabled = false;
            ddlInsuranceTypeSearch.Items.Clear();
            ddlInsuranceTypeSearch.Items.Insert(0, new ListItem("", "-1"));      
        }

        private void BindProductListNew()
        {
            var productList = ProductBiz.GetProductListNew();
            productList.Insert(0, new Resource.Data.ControlListData() { TextField = "", ValueField = "-1" });
            cmbProductEdit.DataSource = productList;
            cmbProductEdit.DataBind();
            cmbProductSearch.DataSource = productList;
            cmbProductSearch.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            BindProductListNew();
            //clearInput();
            Benefit = new InsuranceBenefit() { BenefitId = -1 };
            radioCommissionPct.Checked = false;
            radioCommissionThb.Checked = true;
            radioOV1Pct.Checked = false;
            radioOV1Thb.Checked = false;
            radioOV1Undefine.Checked = true;
            radioOV2Pct.Checked = false;
            radioOV2Thb.Checked = false;
            radioOV2Undefine.Checked = true;
            radioInVat.Checked = true;
            radioExVat.Checked = false;
            radioActive.Checked = true;
            radioNoActive.Checked = false;
            updDetail.Update();

            mpePopupReceiveNo.Show();
            //upnPopupReceiveNo.Update();
        }

        protected void btnPopupSave_Click(object sender, EventArgs e)
        {
            try
            {
                string strErrorMessage = string.Empty;

                if(ValidatePerCent(textCommissionPct.Text))
                {
                    strErrorMessage = "ค่าคอมมิชชั่น(%) ต้องไม่เกิน 100%";
                }
                else if (ValidateDecimal(textCommissionPct.Text))
                {
                    strErrorMessage += (strErrorMessage.Trim() != string.Empty ? Environment.NewLine : string.Empty) + "ค่าคอมมิชชั่น(%) ทศนิยมต้องไม่เกิน 2 ตำแหน่ง" ;
                }
                else if(ValidateDecimal(textCommissionThb.Text))
                {
                    strErrorMessage += (strErrorMessage.Trim() != string.Empty ? Environment.NewLine : string.Empty) + "ค่าคอมมิชชั่น(บาท) ทศนิยมต้องไม่เกิน 2 ตำแหน่ง";
                }

                if (ValidatePerCent(textOV1Pct.Text))
                {
                    strErrorMessage += (strErrorMessage.Trim() != string.Empty ? Environment.NewLine : string.Empty) + "OV1(%) ต้องไม่เกิน 100%";
                }
                else if (ValidateDecimal(textOV1Pct.Text))
                {
                    strErrorMessage += (strErrorMessage.Trim() != string.Empty ? Environment.NewLine : string.Empty) + "OV1(%) ทศนิยมต้องไม่เกิน 2 ตำแหน่ง";
                }
                else if (ValidateDecimal(textOV1Thb.Text))
                {
                    strErrorMessage += (strErrorMessage.Trim() != string.Empty ? Environment.NewLine : string.Empty) + "OV1(บาท) ทศนิยมต้องไม่เกิน 2 ตำแหน่ง";
                }

                if (ValidatePerCent(textOV2Pct.Text))
                {
                    strErrorMessage += (strErrorMessage.Trim() != string.Empty ? Environment.NewLine : string.Empty) + "OV2(%) ต้องไม่เกิน 100%";
                }
                else if (ValidateDecimal(textOV2Pct.Text))
                {
                    strErrorMessage += (strErrorMessage.Trim() != string.Empty ? Environment.NewLine : string.Empty) + "OV2(%) ทศนิยมต้องไม่เกิน 2 ตำแหน่ง";
                }
                else if (ValidateDecimal(textOV2Thb.Text))
                {
                    strErrorMessage += (strErrorMessage.Trim() != string.Empty ? Environment.NewLine : string.Empty) + "OV2(บาท) ทศนิยมต้องไม่เกิน 2 ตำแหน่ง";
                }

                if (strErrorMessage.Trim() != string.Empty)
                {
                    AppUtil.ClientAlert(Page, strErrorMessage);
                    return;
                }

                // save
                string saveResponse = "";
                bool saveResult = SlmScr035Biz.Save(Benefit, out saveResponse);

                if (saveResult)
                {
                    mpePopupReceiveNo.Hide();
                    BindGridview(pcTop, LoadData().ToArray(), 0);
                }
                else
                {
                    mpePopupReceiveNo.Show();
                }
                AppUtil.ClientAlert(Page, saveResponse);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnPopupCancel_Click(object sender, EventArgs e)
        {
            clearInput();
            updDetail.Update();
            mpePopupReceiveNo.Hide();
        }

        private void BindProductList()
        {
            var productList = ProductBiz.GetProductList();
            productList.Insert(0, new Resource.Data.ControlListData() { TextField = "", ValueField = "-1" });
            cmbProductEdit.DataSource = productList;
            cmbProductEdit.DataBind();
            cmbProductSearch.DataSource = productList;
            cmbProductSearch.DataBind();
        }

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                BindProductList();
                clearInput();
                ImageButton imb = (ImageButton)sender;
                decimal id = decimal.Parse(imb.CommandArgument);
                //Benefit = SlmScr035Biz.GetSearchBenefitById(id);
                var b = SlmScr035Biz.GetSearchBenefitById(id);
                SetBenefit(b);

                updDetail.Update();

                //upnPopupReceiveNo.Update();
                mpePopupReceiveNo.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }


        // combo
        protected void cmbProductEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                updateCmbEditCampaign();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbInsComNameEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                updateCmbEditCampaign();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbProductSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                updateCmbSearchCampaign();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbInsComName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                updateCmbSearchCampaign();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        

        // radio
        protected void radioCommission_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ClearCommission();
                UpdateCommissionBox();
                //upnCommission.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void radioOV1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ClearOV1();
                if (radioOV1Pct.Checked)
                {
                    textOV1Pct.Enabled = true;
                    requireOV1Pct.Enabled = true;
                    textOV1Pct.Focus();
                }
                else if (radioOV1Thb.Checked)
                {
                    textOV1Thb.Enabled = true;
                    requireOV1Thb.Enabled = true;
                    textOV1Thb.Focus();
                }
                //upOV1.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void radioOV2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ClearOV2();
                if (radioOV2Pct.Checked)
                {
                    textOV2Pct.Enabled = true;
                    requireOV2Pct.Enabled = true;
                    textOV2Pct.Focus();
                }
                else if (radioOV2Thb.Checked)
                {
                    textOV2Thb.Enabled = true;
                    requireOV2Thb.Enabled = true;
                    textOV2Thb.Focus();
                }
                //upOV2.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void chkCoverageTypeSearch_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ddlCoverageTypeSearch.Items.Clear();
                ddlCoverageTypeSearch.Enabled = chkCoverageTypeSearch.Checked;
                if (chkCoverageTypeSearch.Checked)
                {
                    var result = CoverageTypeBiz.GetCoverageTypeForDropdownList();
                    ddlCoverageTypeSearch.DataSource = result;
                    ddlCoverageTypeSearch.DataValueField = "ValueField";
                    ddlCoverageTypeSearch.DataTextField = "TextField";
                    ddlCoverageTypeSearch.DataBind();
                }
                else
                {
                    ddlCoverageTypeSearch.DataSource = null;
                    ddlCoverageTypeSearch.DataBind();
                }
                ddlCoverageTypeSearch.Items.Insert(0, new ListItem("", "-1"));
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void chkInsuranceTypeSearch_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ddlInsuranceTypeSearch.Items.Clear();
                ddlInsuranceTypeSearch.Enabled = chkInsuranceTypeSearch.Checked;
                if (chkInsuranceTypeSearch.Checked)
                {
                    var result = InsuranceCarTypeBiz.GetInsuranceCarTypeForDropdownList();
                    ddlInsuranceTypeSearch.DataSource = result;
                    ddlInsuranceTypeSearch.DataValueField = "ValueField";
                    ddlInsuranceTypeSearch.DataTextField = "TextField";
                    ddlInsuranceTypeSearch.DataBind();                    
                }
                else
                {
                    ddlInsuranceTypeSearch.DataSource = null;
                    ddlInsuranceTypeSearch.DataBind();
                }
                ddlInsuranceTypeSearch.Items.Insert(0, new ListItem("", "-1"));
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void rdoInsuranceType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDropDownListEdit();
                rdoInsuranceType.Attributes.Add("onclick", "javascript: __doPostBack('ctl00$ContentPlaceHolder1$rdoInsuranceType','')");
                //rdoInsuranceType.Attributes.Add("onclick", "javascript: setTimeout('__doPostBack(\\'ctl00$ContentPlaceHolder1$rdoInsuranceType\\',\\'\\')', 0)");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void rdoCoverageType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDropDownListEdit();
                rdoCoverageType.Attributes.Add("onclick", "javascript: __doPostBack('ctl00$ContentPlaceHolder1$rdoCoverageType','')");
                //rdoCoverageType.Attributes.Add("onclick", "javascript: setTimeout('__doPostBack(\\'ctl00$ContentPlaceHolder1$rdoCoverageType\\',\\'\\')', 0)");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
                
        // etcs
        private void updateCmbSearchCampaign()
        {
            cmbCampaign.Items.Clear();

            if (cmbInsComName.SelectedValue != "-1" && cmbProductSearch.SelectedValue != "-1")
            {
                decimal insComId = 0;
                if (decimal.TryParse(cmbInsComName.SelectedValue, out insComId))
                {
                    cmbCampaign.DataSource = SlmScr035Biz.GetCampaignListData(insComId, cmbProductSearch.SelectedValue);
                    cmbCampaign.DataBind();
                    //upCampaignSearch.Update();
                }
            }
        }

        private void updateCmbEditCampaign()
        {
            cmbCampaignEdit.Items.Clear();

            if (cmbInsComNameEdit.SelectedValue != "-1" && cmbProductEdit.SelectedValue != "-1")
            {
                decimal insComId = 0;
                if (decimal.TryParse(cmbInsComNameEdit.SelectedValue, out insComId))
                {
                    cmbCampaignEdit.DataSource = SlmScr035Biz.GetCampaignListData(insComId, cmbProductEdit.SelectedValue);
                    cmbCampaignEdit.DataBind();
                    upCampaignEdit.Update();
                }
            }
        }

        private void ClearOV1()
        {
            textOV1Pct.Text = "";
            textOV1Thb.Text = "";
            textOV1Pct.Enabled = false;
            textOV1Thb.Enabled = false;
            requireOV1Thb.Enabled = false;
            requireOV1Pct.Enabled = false;
        }

        private void ClearOV2()
        {
            textOV2Pct.Text = "";
            textOV2Thb.Text = "";
            textOV2Pct.Enabled = false;
            textOV2Thb.Enabled = false;
            requireOV2Thb.Enabled = false;
            requireOV2Pct.Enabled = false;

        }

        private void ClearCommission()
        {
            textCommissionThb.Text = "";
            textCommissionPct.Text = "";
            textCommissionThb.Enabled = false;
            textCommissionPct.Enabled = false;
            requireCommissionThb.Enabled = false;
            requireCommissionPct.Enabled = false;
        }

        private void UpdateCommissionBox()
        {
            if (radioCommissionPct.Checked)
            {
                textCommissionPct.Enabled = true;
                textCommissionPct.Focus();
                requireCommissionPct.Enabled = true;
            }
            else
            {
                textCommissionThb.Enabled = true;
                textCommissionThb.Focus();
                requireCommissionThb.Enabled = true;
            }
        }

        private List<InsuranceBenefit> LoadData()
        {
            decimal campId = cmbCampaign.SelectedValue == "" ? -1 : decimal.Parse(cmbCampaign.SelectedValue);
            decimal insComId = cmbInsComName.SelectedValue == "" ? -1 : decimal.Parse(cmbInsComName.SelectedValue);
            
            decimal coverageTypeId = decimal.Parse(ddlCoverageTypeSearch.SelectedValue);
            decimal insuranceCarTypeId = decimal.Parse(ddlInsuranceTypeSearch.SelectedValue);

            string BenefitTypeCode = string.Format("{0}{1}", chkCoverageTypeSearch.Checked ? "204" : "", chkInsuranceTypeSearch.Checked ? "205" : "");
            string Status = string.Format("{0}{1}", chkActive.Checked ? "0" : "", chkInActive.Checked ? "1" : "");

            return SlmScr035Biz.GetBenefitSearch(cmbProductSearch.SelectedValue, insComId, campId, BenefitTypeCode, coverageTypeId, insuranceCarTypeId, Status);
        }

        private void LoadDropDownListEdit()
        {
            ddlInsuranceType.Items.Clear();
            var result = rdoCoverageType.Checked ? CoverageTypeBiz.GetCoverageTypeForDropdownList() : rdoInsuranceType.Checked ? InsuranceCarTypeBiz.GetInsuranceCarTypeForDropdownList() : null;
            ddlInsuranceType.DataSource = result;
            ddlInsuranceType.DataValueField = "ValueField";
            ddlInsuranceType.DataTextField = "TextField";
            ddlInsuranceType.DataBind();
            ddlInsuranceType.Items.Insert(0, new ListItem("", "-1"));
            uplBenefitEdit.Update();

        }

        private bool ValidatePerCent(string PercenValue)
        {
            try
            {
                PercenValue = PercenValue.Trim();
                if (PercenValue != string.Empty)
                {
                    if (Convert.ToDecimal(PercenValue) > 100)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        private bool ValidateDecimal(string DecimalValue)
        {
            try
            {
                DecimalValue = DecimalValue.Trim();
                if (DecimalValue != string.Empty)
                {
                    string[] strArrValidate = DecimalValue.Split(new char[] { '.' });
                    if (strArrValidate.Length == 2)
                    {
                        if (strArrValidate[1].Length > 2)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        private string DecimalDisplayFormat(decimal? DecimalValue)
        {
            string strReturn = string.Empty;
            string strNumberFormat = "###0.00";
            if (DecimalValue.HasValue) 
            {
                strReturn = DecimalValue.Value.ToString(strNumberFormat);
            }
            return strReturn;
        }
    }
}