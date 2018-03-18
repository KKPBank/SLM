using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Application.Shared;
using SLM.Application.Utilities;
using SLM.Resource;

namespace SLM.Application.Shared
{
    public partial class Prelead_Detail : System.Web.UI.UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
             BuildCombo();
            AppUtil.SetMoneyTextBox(txtVolCarAmt);
            AppUtil.SetMoneyTextBox(txtVolGrossPreimum);
            AppUtil.SetMoneyTextBox(txtCompulGrossPremium);
            AppUtil.SetMoneyTextBox(txtCompulCovAmt);
            //AppUtil.SetAutoCompleteDropdown(new DropDownList[] {
            //        cmbModel,
            //        cmbBrand
            //    }
            //    , Page
            //    , this.ClientID + "_Autocomplete");


        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void BuildCombo()
        {
            AppUtil.BuildCombo(cmbBranch, SlmScr010Biz.GetBranchData());

            AppUtil.BuildCombo(cmbCardTypeId, CardTypeBiz.GetCardTypeList());
            AppUtil.BuildCombo(cmbMaritalStatus, SlmScr046Biz.GetMaritalDataList());
            AppUtil.BuildCombo(cmbOccupation, SlmScr010Biz.GetOccupationData(false));
            var complst = SlmScr034Biz.GetImportInsComCodeListData();
            //complst.RemoveAt(0);
            AppUtil.BuildCombo(cmbCompulCompCode, complst);
            AppUtil.BuildCombo(cmbVolCompanyCode, complst);
            AppUtil.BuildCombo(cmbVolTypeKey, SlmScr046Biz.GetCoverageType());
            var titlelist = SlmScr046Biz.GetTitleDataList();
            AppUtil.BuildCombo(cmbTitleId, titlelist);
            AppUtil.BuildCombo(cmbBenTitleId, titlelist);
            AppUtil.BuildCombo(cmbVolMktTitle, titlelist);
            AppUtil.BuildCombo(cmbDrv1TitleId, titlelist);
            AppUtil.BuildCombo(cmbDrv2TitleId, titlelist);
            AppUtil.BuildCombo(cmbBenTitleId, titlelist);
            AppUtil.BuildCombo(cmbGuar1Title, titlelist);
            AppUtil.BuildCombo(cmbGuar2Title, titlelist);
            AppUtil.BuildCombo(cmbGuar3Title, titlelist);
            AppUtil.BuildCombo(cmbBrandCode, SlmScr010Biz.GetBrandDataNew());
            AppUtil.BuildCombo(cmbProvinceRegis, SlmScr046Biz.GetProvinceDataId());
            AppUtil.BuildCombo(cmbCarByGovId, SlmScr046Biz.GetInsuranceTypeData());
            var relate = SlmScr046Biz.GetRelationData();
            AppUtil.BuildCombo(cmbGuar1Relation, relate);
            AppUtil.BuildCombo(cmbGuar2Relation, relate);
            AppUtil.BuildCombo(cmbGuar3Relation, relate);

        }

        public void SetData(SlmScr046Biz.PreleadDetails pl)
        {
            hdfPreleadId.Value = pl.slm_Prelead_Id.ToString();
            txtContractNo.Text = pl.slm_Contract_Number;
            AppUtil.SetComboValue(cmbBranch,pl.slm_BranchCode);
            txtContractYear.Text = pl.slm_Contract_Year;
            AppUtil.SetComboValue(cmbContractStatus,pl.slm_Contract_Status);
            AppUtil.SetComboValue(cmbCarCategory,pl.slm_Car_Category);
            txtCustomerKey.Text = pl.slm_Customer_Key;
            AppUtil.SetComboValue(cmbCardTypeId, pl.slm_CardTypeId.ToString());
            lblCardTypeOrg.Text = pl.slm_CardType_Org == "Y" ? "บุคคลธรรมดา" : pl.slm_CardType_Org == "N" ? "นิติบุคคล" : "";
            txtCitizenId.Text = pl.slm_CitizenId;
            AppUtil.SetComboValue(cmbMaritalStatus, pl.slm_Marital_Status);
            AppUtil.SetComboValue(cmbTitleId, pl.slm_TitleId.ToString());
            lblTitleNameOrg.Text = pl.slm_Title_Name_Org;
            txtName.Text = pl.slm_Name;
            txtLastName.Text = pl.slm_LastName;
            AppUtil.SetComboValue(cmbOccupation, pl.slm_OccupationId.ToString());
            lblCareerDescOrg.Text = pl.slm_Career_Desc_Org;
            txtGrade.Text = pl.slm_Grade;
            if (pl.slm_Birthdate != null) tdBirthDate.DateValue = pl.slm_Birthdate.Value;

            AppUtil.SetComboValue(cmbCarByGovId, pl.slm_Car_By_Gov_Id.ToString());
            lblCarByGovNameOrg.Text = pl.slm_Car_By_Gov_Name_Org;
            AppUtil.SetComboValue(cmbBrandCode, pl.slm_Brand_Code);
            BuildModelCombo();
            lblBrandNameOrg.Text = pl.slm_Brand_Name_Org;
            AppUtil.SetComboValue(cmbModel, pl.slm_Model_Code);
            BuildYearGroupCombo();
            lblModelNameOrg.Text = pl.slm_Model_name_Org;
            AppUtil.SetComboValue(cmbYearGroup, pl.slm_Model_Year);
            lblYearGroupOrg.Text = pl.slm_Model_Year;
            txtEngineNo.Text = pl.slm_Engine_No;
            txtChassisNo.Text = pl.slm_Chassis_No;
            txtLicenseNo.Text = pl.slm_Car_License_No;
            AppUtil.SetComboValue(cmbProvinceRegis, pl.slm_ProvinceRegis.ToString());
            lblProvinceRegisOrg.Text = pl.slm_ProvinceRegis_Org;
            txtCC.Text = pl.slm_Cc;
            if (pl.slm_Expire_Date != null) tdExpireDate.DateValue = pl.slm_Expire_Date.Value;

            // voluntary
            txtVolMktId.Text = pl.slm_Voluntary_Mkt_Id;
            lblVolMktIdOrg.Text = pl.slm_Voluntary_Mkt_Id_Org;
            AppUtil.SetComboValue(cmbVolMktTitle, pl.slm_Voluntary_Mkt_TitleId.ToString());
            lblVolMktTitleOrg.Text = pl.slm_Voluntary_Mkt_Title_Org;
            txtVolMktFName.Text = pl.slm_Voluntary_Mkt_First_Name;
            txtVolMktLName.Text = pl.slm_Voluntary_Mkt_Last_Name;
            AppUtil.SetComboValue(cmbVolCompanyCode, pl.slm_Voluntary_Company_Code);
            txtVolPolicyNo.Text = pl.slm_Voluntary_Policy_Number;
            AppUtil.SetComboValue(cmbVolTypeKey, pl.slm_Voluntary_Type_Key);
            txtVolCarAmt.Text = pl.slm_Voluntary_Cov_Amt == null ? "" : pl.slm_Voluntary_Cov_Amt.Value.ToString("#,##0.00");
            if (pl.slm_Voluntary_Policy_Eff_Date != null) tdVolPolicyEFDate.DateValue = pl.slm_Voluntary_Policy_Eff_Date.Value;
            if (pl.slm_Voluntary_Policy_Exp_Date != null) tdVolPolicyEPDate.DateValue = pl.slm_Voluntary_Policy_Exp_Date.Value;
            txtVolPolicyYear.Text = pl.slm_Voluntary_Policy_Year;
            txtVolGrossPreimum.Text = pl.slm_Voluntary_Gross_Premium == null ? "" : pl.slm_Voluntary_Gross_Premium.Value.ToString("#,##0.00");
            txtVolPolicyExpYear.Text = pl.slm_Voluntary_Policy_Exp_Year;
            txtVolPolicyExpMonth.Text = pl.slm_Voluntary_Policy_Exp_Month;
            cmbVolChannelKey.Text = pl.slm_Voluntary_Channel_Key;

            // benefit
            AppUtil.SetComboValue(cmbBenTitleId, pl.slm_Benefit_TitleId.ToString());
            lblBenTitleNameOrg.Text = pl.slm_Benefit_Title_Name_Org;
            txtBenFName.Text = pl.slm_Benefit_First_Name;
            txtBenLName.Text = pl.slm_Benefit_Last_Name;
            txtBenTelNo.Text = pl.slm_Benefit_Telno;

            // driver
            AppUtil.SetComboValue(cmbDrv1TitleId, pl.slm_Driver_TitleId1.ToString());
            lblDrv1TitleNameOrg.Text = pl.slm_Driver_Title_Name1_Org;
            txtDrv1FName.Text = pl.slm_Driver_First_Name1;
            txtDrv1LName.Text = pl.slm_Driver_Last_Name1;
            txtDrv1TelNo.Text = pl.slm_Driver_Telno1;
            if (pl.slm_Driver_Birthdate1 != null) tdDrv1Birthday.DateValue = pl.slm_Driver_Birthdate1.Value;
            AppUtil.SetComboValue(cmbDrv2TitleId, pl.slm_Driver_TitleId2.ToString());
            lblDrv2TitleNameOrg.Text = pl.slm_Driver_Title_Name2_Org;
            txtDrv2FName.Text = pl.slm_Driver_First_Name2;
            txtDrv2LName.Text = pl.slm_Driver_Last_Name2;
            txtDrv2TelNo.Text = pl.slm_Driver_Telno2;
            if (pl.slm_Driver_Birthdate2 != null) tdDrv2Birthday.DateValue = pl.slm_Driver_Birthdate2.Value;

            // compulsory
            AppUtil.SetComboValue(cmbCompulCompCode, pl.slm_Compulsory_Company_Code);
            txtCompulPolicyNo.Text = pl.slm_Compulsory_Policy_Number;
            txtCompulPolicyYear.Text = pl.slm_Compulsory_Policy_Year;
            if (pl.slm_Compulsory_Policy_Eff_Date != null) tdCompulPolicyEFDate.DateValue = pl.slm_Compulsory_Policy_Eff_Date.Value;
            if (pl.slm_Compulsory_Policy_Exp_Date != null) tdCompulPolicyEPDate.DateValue = pl.slm_Compulsory_Policy_Exp_Date.Value;
            txtCompulGrossPremium.Text = pl.slm_Compulsory_Gross_Premium == null ? "" : pl.slm_Compulsory_Gross_Premium.Value.ToString("#,##0.00");
            txtCompulCovAmt.Text = pl.slm_Compulsory_Cov_Amt == null ? "" : pl.slm_Compulsory_Cov_Amt.Value.ToString("#,##0.00");

            // guarantor
            txtGuar1Code.Text = pl.slm_Guarantor_Code1;
            AppUtil.SetComboValue(cmbGuar1Title, pl.slm_Guarantor_TitleId1.ToString());
            lblGuar1TitleNameOrg.Text = pl.slm_Guarantor_Title_Name1_Org;
            txtGuar1FName.Text = pl.slm_Guarantor_First_Name1;
            txtGuar1LName.Text = pl.slm_Guarantor_Last_Name1;
            txtGuar1CitizenId.Text = pl.slm_Guarantor_Card_Id1;
            AppUtil.SetComboValue(cmbGuar1Relation, pl.slm_Guarantor_RelationId1.ToString());
            lblGuar1RelationOrg.Text = pl.slm_Guarantor_Relation1_Org;
            txtGuar1TelNo.Text = pl.slm_Guarantor_Telno1;
            txtGuar2Code.Text = pl.slm_Guarantor_Code2;
            AppUtil.SetComboValue(cmbGuar2Title, pl.slm_Guarantor_TitleId2.ToString());
            lblGuar2TitleNameOrg.Text = pl.slm_Guarantor_Title_Name2_Org;
            txtGuar2FName.Text = pl.slm_Guarantor_First_Name2;
            txtGuar2LName.Text = pl.slm_Guarantor_Last_Name2;
            txtGuar2CitizenId.Text = pl.slm_Guarantor_Card_Id2;
            AppUtil.SetComboValue(cmbGuar2Relation, pl.slm_Guarantor_RelationId2.ToString());
            lblGuar2RelationOrg.Text = pl.slm_Guarantor_Relation2_Org;
            txtGuar2TelNo.Text = pl.slm_Guarantor_Telno2;
            txtGuar3Code.Text = pl.slm_Guarantor_Code3;
            AppUtil.SetComboValue(cmbGuar3Title, pl.slm_Guarantor_TitleId3.ToString());
            lblGuar3TitleNameOrg.Text = pl.slm_Guarantor_Title_Name3_Org;
            txtGuar3FName.Text = pl.slm_Guarantor_First_Name3;
            txtGuar3LName.Text = pl.slm_Guarantor_Last_Name3;
            txtGuar3CitizenId.Text = pl.slm_Guarantor_Card_Id3;
            AppUtil.SetComboValue(cmbGuar3Relation, pl.slm_Guarantor_RelationId3.ToString());
            lblGuar3RelationOrg.Text = pl.slm_Guarantor_Relation3_Org;
            txtGuar3TelNo.Text = pl.slm_Guarantor_Telno3;
        }
        public SlmScr046Biz.PreleadDetails GetData()
        {
            var pl = new SlmScr046Biz.PreleadDetails();
            pl.slm_Prelead_Id = SLMUtil.SafeDecimal(hdfPreleadId.Value);
            pl.slm_Contract_Number = txtContractNo.Text;
            if (cmbBranch.SelectedIndex > 0) pl.slm_BranchCode = cmbBranch.SelectedValue;
            pl.slm_Contract_Year = txtContractYear.Text;
            if (cmbContractStatus.SelectedIndex > 0) pl.slm_Contract_Status = cmbContractStatus.SelectedValue;
            if (cmbCarCategory.SelectedIndex > 0) pl.slm_Car_Category = cmbCarCategory.SelectedValue;
            pl.slm_Customer_Key = txtCustomerKey.Text;
            if (cmbCardTypeId.SelectedIndex > 0) pl.slm_CardTypeId = SLMUtil.SafeInt(cmbCardTypeId.SelectedValue);
            pl.slm_CitizenId = txtCitizenId.Text;
            if (cmbMaritalStatus.SelectedIndex > 0) pl.slm_Marital_Status = cmbMaritalStatus.SelectedValue;
            if (cmbTitleId.SelectedIndex > 0) pl.slm_TitleId = SLMUtil.SafeInt(cmbTitleId.SelectedValue);
            pl.slm_Name = txtName.Text;
            pl.slm_LastName = txtLastName.Text;
            if (cmbOccupation.SelectedIndex > 0) pl.slm_OccupationId = SLMUtil.SafeInt(cmbOccupation.SelectedValue);
            pl.slm_Grade = txtGrade.Text;
            if (tdBirthDate.DateValue.Year != 1) pl.slm_Birthdate = tdBirthDate.DateValue;

            if (cmbCarByGovId.SelectedIndex > 0) pl.slm_Car_By_Gov_Id = SLMUtil.SafeInt(cmbCarByGovId.SelectedValue);
            if (cmbBrandCode.SelectedIndex > 0) pl.slm_Brand_Code = cmbBrandCode.SelectedValue;
            if (cmbModel.SelectedIndex > 0) pl.slm_Model_Code = cmbModel.SelectedValue;
            if (cmbYearGroup.SelectedIndex > 0) pl.slm_Model_Year = cmbYearGroup.SelectedValue;
            pl.slm_Engine_No = txtEngineNo.Text;
            pl.slm_Chassis_No = txtChassisNo.Text;
            pl.slm_Car_License_No = txtLicenseNo.Text;
            if (cmbProvinceRegis.SelectedIndex > 0) pl.slm_ProvinceRegis = SLMUtil.SafeInt(cmbProvinceRegis.SelectedValue);
            pl.slm_Cc = txtCC.Text;
            if (tdExpireDate.DateValue.Year != 1) pl.slm_Expire_Date = tdExpireDate.DateValue;

            // voluntary
            pl.slm_Voluntary_Mkt_Id = txtVolMktId.Text;
            if (cmbVolMktTitle.SelectedIndex > 0) pl.slm_Voluntary_Mkt_TitleId = SLMUtil.SafeInt(cmbVolMktTitle.SelectedValue);
            pl.slm_Voluntary_Mkt_First_Name = txtVolMktFName.Text;
            pl.slm_Voluntary_Mkt_Last_Name = txtVolMktLName.Text;
            if (cmbVolCompanyCode.SelectedIndex > 0) pl.slm_Voluntary_Company_Code = cmbVolCompanyCode.SelectedValue;
            pl.slm_Voluntary_Policy_Number = txtVolPolicyNo.Text;
            if (cmbVolTypeKey.SelectedIndex > 0) pl.slm_Voluntary_Type_Key = cmbVolTypeKey.SelectedValue;
            pl.slm_Voluntary_Cov_Amt = SLMUtil.SafeDecimal(txtVolCarAmt.Text);
            if (tdVolPolicyEFDate.DateValue.Year != 1) pl.slm_Voluntary_Policy_Eff_Date = tdVolPolicyEFDate.DateValue;
            if (tdVolPolicyEPDate.DateValue.Year != 1) pl.slm_Voluntary_Policy_Exp_Date = tdVolPolicyEPDate.DateValue;
            pl.slm_Voluntary_Policy_Year = txtVolPolicyYear.Text;
            pl.slm_Voluntary_Gross_Premium = SLMUtil.SafeDecimal(txtVolGrossPreimum.Text);
            pl.slm_Voluntary_Policy_Exp_Year = txtVolPolicyExpYear.Text;
            pl.slm_Voluntary_Policy_Exp_Month = txtVolPolicyExpMonth.Text;
            pl.slm_Voluntary_Channel_Key = cmbVolChannelKey.Text;

            // benefit
            if (cmbBenTitleId.SelectedIndex > 0) pl.slm_Benefit_TitleId = SLMUtil.SafeInt(cmbBenTitleId.SelectedValue);
            pl.slm_Benefit_First_Name = txtBenFName.Text;
            pl.slm_Benefit_Last_Name = txtBenLName.Text;
            pl.slm_Benefit_Telno = txtBenTelNo.Text;

            // driver
            if (cmbDrv1TitleId.SelectedIndex > 0) pl.slm_Driver_TitleId1 = SLMUtil.SafeInt(cmbDrv1TitleId.SelectedValue);
            pl.slm_Driver_First_Name1 = txtDrv1FName.Text;
            pl.slm_Driver_Last_Name1 = txtDrv1LName.Text;
            if (tdDrv1Birthday.DateValue.Year != 1) pl.slm_Driver_Birthdate1 = tdDrv1Birthday.DateValue;
            pl.slm_Driver_Telno1 = txtDrv1TelNo.Text;
            if (cmbDrv2TitleId.SelectedIndex > 0) pl.slm_Driver_TitleId2 = SLMUtil.SafeInt(cmbDrv2TitleId.SelectedValue);
            pl.slm_Driver_First_Name2 = txtDrv2FName.Text;
            pl.slm_Driver_Last_Name2 = txtDrv2LName.Text;
            pl.slm_Driver_Telno2 = txtDrv2TelNo.Text;
            if (tdDrv2Birthday.DateValue.Year != 1) pl.slm_Driver_Birthdate2 = tdDrv2Birthday.DateValue;

            // compulsory
            if (cmbCompulCompCode.SelectedIndex > 0) pl.slm_Compulsory_Company_Code = cmbCompulCompCode.SelectedValue;
            pl.slm_Compulsory_Policy_Number = txtCompulPolicyNo.Text;
            pl.slm_Compulsory_Policy_Year = txtCompulPolicyYear.Text;
            if (tdCompulPolicyEFDate.DateValue.Year != 1) pl.slm_Compulsory_Policy_Eff_Date = tdCompulPolicyEFDate.DateValue;
            if (tdCompulPolicyEPDate.DateValue.Year != 1) pl.slm_Compulsory_Policy_Exp_Date = tdCompulPolicyEPDate.DateValue;
            pl.slm_Compulsory_Gross_Premium = SLMUtil.SafeDecimal(txtCompulGrossPremium.Text);
            pl.slm_Compulsory_Cov_Amt = SLMUtil.SafeDecimal(txtCompulCovAmt.Text);

            // guarantor
            pl.slm_Guarantor_Code1 = txtGuar1Code.Text;
            if (cmbGuar1Title.SelectedIndex > 0) pl.slm_Guarantor_TitleId1 = SLMUtil.SafeInt(cmbGuar1Title.SelectedValue);
            pl.slm_Guarantor_First_Name1 = txtGuar1FName.Text;
            pl.slm_Guarantor_Last_Name1 = txtGuar1LName.Text;
            pl.slm_Guarantor_Card_Id1 = txtGuar1CitizenId.Text;
            if (cmbGuar1Relation.SelectedIndex > 0) pl.slm_Guarantor_RelationId1 = SLMUtil.SafeInt(cmbGuar1Relation.SelectedValue);
            pl.slm_Guarantor_Telno1 = txtGuar1TelNo.Text;

            pl.slm_Guarantor_Code2 = txtGuar2Code.Text;
            if (cmbGuar2Title.SelectedIndex > 0) pl.slm_Guarantor_TitleId2 = SLMUtil.SafeInt(cmbGuar2Title.SelectedValue);
            pl.slm_Guarantor_First_Name2 = txtGuar2FName.Text;
            pl.slm_Guarantor_Last_Name2 = txtGuar2LName.Text;
            pl.slm_Guarantor_Card_Id2 = txtGuar2CitizenId.Text;
            if (cmbGuar2Relation.SelectedIndex > 0) pl.slm_Guarantor_RelationId2 = SLMUtil.SafeInt(cmbGuar2Relation.SelectedValue);
            pl.slm_Guarantor_Telno2 = txtGuar2TelNo.Text;

            pl.slm_Guarantor_Code3 = txtGuar3Code.Text;
            if (cmbGuar3Title.SelectedIndex > 0) pl.slm_Guarantor_TitleId3 = SLMUtil.SafeInt(cmbGuar3Title.SelectedValue);
            pl.slm_Guarantor_First_Name3 = txtGuar3FName.Text;
            pl.slm_Guarantor_Last_Name3 = txtGuar3LName.Text;
            pl.slm_Guarantor_Card_Id3 = txtGuar3CitizenId.Text;
            if (cmbGuar3Relation.SelectedIndex > 0) pl.slm_Guarantor_RelationId3 = SLMUtil.SafeInt(cmbGuar3Relation.SelectedValue);
            pl.slm_Guarantor_Telno3 = txtGuar3TelNo.Text;

            return pl;
        }
        public bool ValidateData(string val)
        {
            int err = 0;

            if ((val == "" || val == "contractno") && txtContractNo.Text.Trim() == "") { err++; vtxtName.Text = "กรุณาระบุเลขที่สัญญา"; }

            return err == 0;
        }


        protected void txtContractNo_TextChanged(object sender, EventArgs e)
        {
            ValidateData("contractno");
        }
        protected void cmbBrandCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildModelCombo();
        }
        protected void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildYearGroupCombo();
        }
        private void BuildModelCombo()
        {
            AppUtil.BuildCombo(cmbModel, SlmScr010Biz.GetModelDataNew(cmbBrandCode.SelectedValue));
        }
        private void BuildYearGroupCombo()
        {
            AppUtil.BuildCombo(cmbYearGroup, SlmScr010Biz.GetModelYearNew(cmbBrandCode.SelectedValue, cmbModel.SelectedValue));
        }
        private void BuildSubModelCombo()
        {

        }


    }
}