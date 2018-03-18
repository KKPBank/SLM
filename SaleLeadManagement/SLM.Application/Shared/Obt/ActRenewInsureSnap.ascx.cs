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
using System.Collections;

namespace SLM.Application.Shared.Obt
{
    public partial class ActRenewInsureSnap : System.Web.UI.UserControl
    {

        public string HistoryTicketId;
        private static readonly ILog _log = LogManager.GetLogger(typeof(ActRenewInsureSnap));

        public void show()
        {
            lbErrorMsg.Visible = false;
            mpePopupHistoryMain.Show();
            hidTicketId.Value = HistoryTicketId;
            DoSearchLeadData(0);
        }

        private void BindGridviewHistory(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvHistoryMain);
            pageControl.Update(items, pageIndex);
            UpHistoryMain.Update();
        }

        protected void HistoryPageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                mpePopupHistoryMain.Show();
                DoSearchLeadData(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchLeadData(int pageIndex)
        {
            try
            {
                List<RenewInsuranceData> data = HistoryLeadBiz.GetHistoryMain(hidTicketId.Value);

                BindGridviewHistory((SLM.Application.Shared.GridviewPageController)pcTopHistory, data.ToArray(), pageIndex);
                UpHistoryMain.Update();
            }
            catch
            {
                throw;
            }
        }

        protected void btnCancelPopupHistoryMain_Click(object sender, EventArgs e)
        {
            try
            {
                mpePopupHistoryMain.Hide();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lblHistoryMain_Click(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "popup")
                {
                    List<string> agrList = new List<string>();
                    agrList = e.CommandArgument.ToString().Split('|').ToList();
                    var RenewInsuranceId = agrList[0];
                    var Version = agrList[1];

                    _log.Debug(String.Format("RenewInsuranceId:{0} Version:{1}", RenewInsuranceId, Version));

                    var renewData = HistoryLeadBiz.GetHistoryRenewInsurance(RenewInsuranceId, Version);
                    List<PreleadCompareData> policyData = HistoryLeadBiz.GetHistoryDetailPolicy(RenewInsuranceId, Version);
                    List<PreleadCompareActData> actData = HistoryLeadBiz.GetHistoryDetailAct(RenewInsuranceId, Version);

                    BindRenewHistory(renewData);
                    bindHistoryDetail(policyData);
                    HistoryDetailAct(actData);

                    if (Request["type"] == "3")
                    {
                        pnHistoryPolicy.Visible = false;
                    }

                    ModalPopupHistoryDetail.Show();
                    UpHistoryDetail.Update();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void BindRenewHistory(RenewInsuranceData data)
        {
            txtBeneficiaryName.Text = "";

            if (data != null)
            {
                txtBeneficiaryName.Text = data.slm_BeneficiaryName;
            }
        }

        private void HistoryDetailAct(List<PreleadCompareActData> actData)
        {
            try
            {
                for (var i = 1; i <= 5; i++)
                {
                    ((HiddenField)this.FindControlRecursive("hdActHist" + i)).Value = "";
                    ((Label)this.FindControlRecursive("lblPromoAct_name_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("rbAct_his" + i)).Text = "";
                    //((Label)this.FindControlRecursive("lblPromoAct_name_his" + i)).Text = "";

                    ((Label)this.FindControlRecursive("lblActIssuePlace_his" + i)).Text = "";

                    ((Label)this.FindControlRecursive("lblActIssueBranch_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("rbRegisterAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblCompanyInsuranceAct_his" + i)).Text = "";
                    //((Label)this.FindControlRecursive("lblActInsCom_Id_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtSignNoAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtActStartCoverDateAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtActEndCoverDateAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtCarTaxExpiredDateAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActNetGrossPremiumFullAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActGrossPremiumAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActGrossStampAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActGrossVatAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActNetGrossPremiumAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblActPersonType_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblVat1PercentBathAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblDiscountPercentAct_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtDiscountBathAct_his" + i)).Text = "";

                    CheckBox chk2 = (CheckBox)this.FindControlRecursive("chkCardType_his2");
                    if (chk2 != null)
                    {
                        chk2.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            if (actData != null && actData.Count > 0)
            {
                Hashtable placeMap = new Hashtable();
                placeMap[0] = "";
                placeMap[1] = "ธนาคาร";
                placeMap[2] = "บริษัทประกัน";
                placeMap[3] = "สาขาออก";
                placeMap[4] = "รับเบี้ย";

                var i = 1;


                foreach (PreleadCompareActData pro in actData)
                {
                    if (pro != null)
                    {
                        if (i == 1)
                        {
                            ((Label)this.FindControlRecursive("lblPromoAct_name_his" + i)).Text = pro.slm_Year;
                            ((HiddenField)this.FindControlRecursive("hdActHist" + i)).Value = "0";
                        }
                        else
                        {
                            ((Label)this.FindControlRecursive("lblPromoAct_name_his" + i)).Text = pro.slm_insnameth;
                            ((HiddenField)this.FindControlRecursive("hdActHist" + i)).Value = "0";
                        }

                        ((Label)this.FindControlRecursive("rbAct_his" + i)).Text = pro.slm_ActPurchaseFlag == null ? "" : pro.slm_ActPurchaseFlag.Value == true ? "ซื้อบริษัทนี้" : "";
                        //((Label)this.FindControlRecursive("lblPromoAct_name_his" + i)).Text = pro.slm_ActStartCoverDate == null ? "" : pro.slm_ActStartCoverDate.Value.ToString("yyyy");

                        ((Label)this.FindControlRecursive("lblActIssuePlace_his" + i)).Text = pro.slm_ActIssuePlace == null ? "" : placeMap[pro.slm_ActIssuePlace].ToString();

                        ((Label)this.FindControlRecursive("lblActIssueBranch_his" + i)).Text = pro.slm_ActIssueBranch == null ? "" : pro.slm_BranchName;

                        ((Label)this.FindControlRecursive("rbRegisterAct_his" + i)).Text = pro.slm_SendDocType == null || pro.slm_SendDocType.Value == 0 ? "" : pro.slm_SendDocType.Value == 1 ? "ลงทะเบียน" : "ธรรมดา";
                        ((Label)this.FindControlRecursive("lblCompanyInsuranceAct_his" + i)).Text = pro.slm_insnameth == null ? "ไม่ระบุ" : pro.slm_insnameth;
                        //((Label)this.FindControlRecursive("lblActInsCom_Id_his" + i)).Text = pro.slm_Ins_Com_Id == null ? "" : pro.slm_Ins_Com_Id.ToString();
                        ((Label)this.FindControlRecursive("txtSignNoAct_his" + i)).Text = pro.slm_ActSignNo;
                        //if (i == 1)
                        //{
                        //    ((Label)this.FindControlRecursive("txtActStartCoverDateAct_his" + i)).Text = pro.slm_ActStartCoverDate == null ? "" : pro.slm_ActStartCoverDate.Value.ToString("dd/MM/yyyy");
                        //    ((Label)this.FindControlRecursive("txtActEndCoverDateAct_his" + i)).Text = pro.slm_ActEndCoverDate == null ? "" : pro.slm_ActEndCoverDate.Value.ToString("dd/MM/yyyy");
                        //    ((Label)this.FindControlRecursive("txtCarTaxExpiredDateAct_his" + i)).Text = pro.slm_CarTaxExpiredDate == null ? "" : pro.slm_CarTaxExpiredDate.Value.ToString("dd/MM/yyyy");
                        //}
                        //else
                        //{
                        ((Label)this.FindControlRecursive("txtActStartCoverDateAct_his" + i)).Text = pro.slm_ActStartCoverDate == null ? "" : pro.slm_ActStartCoverDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ((Label)this.FindControlRecursive("txtActEndCoverDateAct_his" + i)).Text = pro.slm_ActEndCoverDate == null ? "" : pro.slm_ActEndCoverDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ((Label)this.FindControlRecursive("txtCarTaxExpiredDateAct_his" + i)).Text = pro.slm_CarTaxExpiredDate == null ? "" : pro.slm_CarTaxExpiredDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //}
                        ((Label)this.FindControlRecursive("lblActNetGrossPremiumFullAct_his" + i)).Text = pro.slm_ActNetGrossPremiumFull == null ? "" : pro.slm_ActNetGrossPremiumFull.Value.ToString("#,##0.00");
                        ((Label)this.FindControlRecursive("lblActGrossPremiumAct_his" + i)).Text = pro.slm_ActGrossPremium != null ? pro.slm_ActGrossPremium.Value.ToString("#,##0.00") : "";
                        ((Label)this.FindControlRecursive("lblActGrossStampAct_his" + i)).Text = pro.slm_ActGrossStamp != null ? pro.slm_ActGrossStamp.Value.ToString("#,##0.00") : "";
                        ((Label)this.FindControlRecursive("lblActGrossVatAct_his" + i)).Text = pro.slm_ActGrossVat != null ? pro.slm_ActGrossVat.Value.ToString("#,##0.00") : "";
                        ((Label)this.FindControlRecursive("lblActNetGrossPremiumAct_his" + i)).Text = pro.slm_ActNetGrossPremium != null ? pro.slm_ActNetGrossPremium.Value.ToString("#,##0.00") : "";
                        CheckBox chk2 = (CheckBox)this.FindControlRecursive("chkCardType_his2");
                        chk2.Checked = pro.slm_Vat1Percent.Value;
                        ((Label)this.FindControlRecursive("lblActPersonType_his" + i)).Text = pro.slm_Vat1PercentBath != null ? pro.slm_Vat1PercentBath.Value.ToString("#,##0.00") : "";
                        ((Label)this.FindControlRecursive("lblVat1PercentBathAct_his" + i)).Text = pro.slm_DiscountPercent != null ? pro.slm_DiscountPercent.Value.ToString("#,##0") : "";
                        ((Label)this.FindControlRecursive("lblDiscountPercentAct_his" + i)).Text = pro.slm_DiscountBath != null ? pro.slm_DiscountBath.Value.ToString("#,##0.00") : "";
                        ((Label)this.FindControlRecursive("txtDiscountBathAct_his" + i)).Text = pro.slm_ActGrossPremiumPay != null ? pro.slm_ActGrossPremiumPay.Value.ToString("#,##0.00") : "";
                        i++;
                    }
                }
            }
        }

        private void bindHistoryDetail(List<PreleadCompareData> PLlist)
        {
            try
            {
                for (var i = 1; i <= 6; i++)
                {
                    ((Label)this.FindControlRecursive("lblYear_his" + i)).Text = "";
                    ((HiddenField)this.FindControlRecursive("hidHistId" + i)).Value = "";
                    ((Label)this.FindControlRecursive("rbInsNameTh_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblInsNameTh_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblCoverageType_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblVoluntary_Policy_Number_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("rbDriver_Flag_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("cmbTitleName1_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtDriver_First_Name1_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtDriver_Last_Name1_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("tdmDriver_Birthdate1_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("cmbTitleName2_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtDriver_First_Name2_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtDriver_Last_Name2_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("tdmDriver_Birthdate2_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblInformed_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblVoluntary_Policy_Eff_Date_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblVoluntary_Policy_Exp_Date_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblVoluntary_Cov_Amt_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblCoverageType2_his" + i)).Text = "";

                    ((Label)this.FindControlRecursive("lblCardTypeId_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblNetpremium_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("hidMaintanance_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblCost_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblPersonType_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtDiscountPercent_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtDiscountBath_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblCostFT_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblDuty_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblVat_amount_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("lblVoluntary_Gross_Premium_his" + i)).Text = "";
                    ((Label)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_his" + i)).Text = "";

                    CheckBox chk2 = (CheckBox)this.FindControlRecursive("chkCardType_his");
                    if (chk2 != null)
                    {
                        chk2.Checked = false;
                    }

                    Label safethis = (Label)this.FindControlRecursive("txtSafe_his" + i);
                    if (safethis != null)
                    {
                        safethis.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            if (PLlist != null && PLlist.Count > 0)
            {
                var i = 1;
                foreach (PreleadCompareData pro in PLlist)
                {
                    if (i == 1)
                    {
                        ((Label)this.FindControlRecursive("lblYear_his" + i)).Text = pro.slm_Year;
                        ((HiddenField)this.FindControlRecursive("hidHistId" + i)).Value = "0";
                    }
                    else if (i == 2)
                    {
                        ((Label)this.FindControlRecursive("lblYear_his" + i)).Text = pro.slm_Year;
                        ((HiddenField)this.FindControlRecursive("hidHistId" + i)).Value = "0";
                    }
                    else
                    {
                        ((Label)this.FindControlRecursive("lblYear_his" + i)).Text = pro.slm_insnameth != null ? pro.slm_insnameth : "";
                        ((HiddenField)this.FindControlRecursive("hidHistId" + i)).Value = pro.slm_insnameth != null ? pro.slm_insnameth : "0";
                    }
                    if (i == 1)
                    {
                        ((Label)this.FindControlRecursive("rbInsNameTh_his" + i)).Text = pro.slm_CoverageTypeName;
                    }
                    else
                    {
                        ((Label)this.FindControlRecursive("rbInsNameTh_his" + i)).Text = pro.slm_Selected == false ? "" : "ซื้อบริษัทนี้";
                    }
                    ((Label)this.FindControlRecursive("lblInsNameTh_his" + i)).Text = pro.slm_insnameth == null ? "ไม่ระบุ" : pro.slm_insnameth;
                    //((Label)this.FindControlRecursive("lblInsCom_Id_pro" + i)).Text = pro.slm_Ins_Com_Id == null ? "" : pro.slm_Ins_Com_Id.ToString();
                    ((Label)this.FindControlRecursive("lblCoverageType_his" + i)).Text = pro.slm_CoverageTypeName;
                    //((HiddenField)this.FindControlRecursive("hidCoverageType_pro" + i)).Value = pro.slm_CoverageTypeId != null ? pro.slm_CoverageTypeId.ToString() : "";
                    ((Label)this.FindControlRecursive("lblVoluntary_Policy_Number_his" + i)).Text = pro.slm_OldPolicyNo;
                    ((Label)this.FindControlRecursive("rbDriver_Flag_his" + i)).Text = pro.slm_DriverFlag == "1" ? "ระบุ" : "ไม่ระบุ";
                    //hidDriver_Flag_pro.Value = pro.slm_Driver_First_Name1 == null && pro.slm_Driver_First_Name2 == null ? "0" : "1";
                    ((Label)this.FindControlRecursive("cmbTitleName1_his" + i)).Text = pro.slm_Driver_TitleId1 == null ? "" : pro.slm_TitleName1;
                    ((Label)this.FindControlRecursive("txtDriver_First_Name1_his" + i)).Text = pro.slm_Driver_First_Name1 == null ? "" : pro.slm_Driver_First_Name1;
                    ((Label)this.FindControlRecursive("txtDriver_Last_Name1_his" + i)).Text = pro.slm_Driver_Last_Name1 == null ? "" : pro.slm_Driver_Last_Name1;
                    ((Label)this.FindControlRecursive("tdmDriver_Birthdate1_his" + i)).Text = pro.slm_Driver_Birthdate1 != null ? pro.slm_Driver_Birthdate1.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                    ((Label)this.FindControlRecursive("cmbTitleName2_his" + i)).Text = pro.slm_Driver_TitleId2 == null ? "" : pro.slm_TitleName2;
                    ((Label)this.FindControlRecursive("txtDriver_First_Name2_his" + i)).Text = pro.slm_Driver_First_Name2 == null ? "" : pro.slm_Driver_First_Name2;
                    ((Label)this.FindControlRecursive("txtDriver_Last_Name2_his" + i)).Text = pro.slm_Driver_Last_Name2 == null ? "" : pro.slm_Driver_Last_Name2;
                    ((Label)this.FindControlRecursive("tdmDriver_Birthdate2_his" + i)).Text = pro.slm_Driver_Birthdate2 != null ? pro.slm_Driver_Birthdate2.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                    ((Label)this.FindControlRecursive("lblInformed_his" + i)).Text = pro.slm_OldReceiveNo;
                    ((Label)this.FindControlRecursive("lblVoluntary_Policy_Eff_Date_his" + i)).Text = pro.slm_PolicyStartCoverDate == null ? "" : pro.slm_PolicyStartCoverDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    ((Label)this.FindControlRecursive("lblVoluntary_Policy_Exp_Date_his" + i)).Text = pro.slm_PolicyEndCoverDate == null ? "" : pro.slm_PolicyEndCoverDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    ((Label)this.FindControlRecursive("lblVoluntary_Cov_Amt_his" + i)).Text = pro.slm_OD != null ? pro.slm_OD.Value.ToString("#,##0.00") : "";
                    //((Label)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_pro" + i)).Text = pro.slm_PolicyGrossPremium != null ? pro.slm_PolicyGrossPremium.Value.ToString("#,##0.00") : "";
                    ((Label)this.FindControlRecursive("lblCoverageType2_his" + i)).Text = pro.slm_CoverageTypeName;

                    ((Label)this.FindControlRecursive("lblCardTypeId_his" + i)).Text = pro.slm_CardTypeName == null ? "" : pro.slm_CardTypeName;
                    ((Label)this.FindControlRecursive("lblNetpremium_his" + i)).Text = pro.slm_PolicyGrossPremium != null ? pro.slm_PolicyGrossPremium.Value.ToString("#,##0.00") : "";
                    //((Label)this.FindControlRecursive("lblMaintanance_pro" + i)).Text = pro.slm_RepairTypeName == null ? "" : pro.slm_RepairTypeName;
                    ((Label)this.FindControlRecursive("hidMaintanance_his" + i)).Text = pro.slm_RepairTypeName == null ? "" : pro.slm_RepairTypeName;
                    //((Label)this.FindControlRecursive("lblCost_his" + i)).Text = pro.slm_DeDuctible != null ? pro.slm_DeDuctible.Value.ToString("#,##0.00") : "";
                    ((Label)this.FindControlRecursive("lblCost_his" + i)).Text = pro.slm_DeDuctibleFlag != null ? pro.slm_DeDuctibleFlag : "";
                    ((Label)this.FindControlRecursive("lblPersonType_his" + i)).Text = pro.slm_Vat1PercentBath != null ? pro.slm_Vat1PercentBath.Value.ToString("#,##0.00") : "";
                    ((Label)this.FindControlRecursive("txtDiscountPercent_his" + i)).Text = pro.slm_DiscountPercent != null ? pro.slm_DiscountPercent.Value.ToString("#,##0") : "";
                    ((Label)this.FindControlRecursive("txtDiscountBath_his" + i)).Text = pro.slm_DiscountBath != null ? pro.slm_DiscountBath.Value.ToString("#,##0.00") : "";
                    //((Label)this.FindControlRecursive("lblCost_his" + i)).Text = null;
                    ((Label)this.FindControlRecursive("lblCostFT_his" + i)).Text = pro.slm_FT != null ? pro.slm_FT.Value.ToString("#,##0.00") : "";
                    ((Label)this.FindControlRecursive("lblDuty_his" + i)).Text = pro.slm_PolicyGrossStamp != null ? pro.slm_PolicyGrossStamp.Value.ToString("#,##0.00") : "";
                    ((Label)this.FindControlRecursive("lblVat_amount_his" + i)).Text = pro.slm_PolicyGrossVat != null ? pro.slm_PolicyGrossVat.Value.ToString("#,##0.00") : "";
                    ((Label)this.FindControlRecursive("lblVoluntary_Gross_Premium_his" + i)).Text = pro.slm_NetGrossPremium != null ? pro.slm_NetGrossPremium.Value.ToString("#,##0.00") : "";
                    ((Label)this.FindControlRecursive("txtTotal_Voluntary_Gross_Premium_his" + i)).Text = pro.slm_PolicyGrossPremiumPay != null ? pro.slm_PolicyGrossPremiumPay.Value.ToString("#,##0.00") : "";

                    CheckBox chk2 = (CheckBox)this.FindControlRecursive("chkCardType_his");
                    chk2.Checked = pro.slm_Vat1Percent.Value;

                    if (i > 1)
                    {
                        if (AppUtil.SafeDecimal(txtTotal_Voluntary_Gross_Premium_his1.Text) > (pro.slm_PolicyGrossPremiumPay != null ? pro.slm_PolicyGrossPremiumPay.Value : 0m))
                        {
                            ((Label)this.FindControlRecursive("txtSafe_his" + (i - 1))).ForeColor = Color.Green;
                        }
                        else
                        {
                            ((Label)this.FindControlRecursive("txtSafe_his" + (i - 1))).ForeColor = Color.Red;
                        }
                        ((Label)this.FindControlRecursive("txtSafe_his" + (i - 1))).Text = pro.slm_CostSave != null ? pro.slm_CostSave.Value.ToString("#,##0.00") : "";
                    }
                    i++;
                }
            }
        }

    }
}