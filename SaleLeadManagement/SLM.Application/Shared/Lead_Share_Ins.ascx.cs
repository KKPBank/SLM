using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using log4net;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Application.Utilities;
using SLM.Biz;

namespace SLM.Application.Shared
{
    public partial class Lead_Share_Ins : System.Web.UI.UserControl
    {
        private bool useWebservice = Convert.ToBoolean(ConfigurationManager.AppSettings["UseWebservice"]);
        private static readonly ILog _log = LogManager.GetLogger(typeof(LeadInfo));
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            BuildCombo(cmbOccupation, SlmScr010Biz.GetOccupationData(false));
            BuildCombo(cmbBrand, SlmScr010Biz.GetBrandDataNew(), "กรุณาระบุ");
            BuildCombo(cmbCarType, SlmScr010Biz.GetInsuranceCarTypeDataNew(), "กรุณาระบุ");
            BuildCombo(cmbProvince, SlmScr010Biz.GetProvinceDataNew());
            BuildCombo(cmbOProvince, SlmScr010Biz.GetProvinceDataNew());
            BuildCombo(cmbDProvince, SlmScr010Biz.GetProvinceDataNew());
            BuildCombo(cmbCardType, CardTypeBiz.GetCardTypeList());
            BuildCombo(cmbDocBranch, BranchBiz.GetBranchList(1));
            BuildCombo(cmbProvinceRegis, SlmScr010Biz.GetProvinceDataNew());
            BuildCombo(cmbCountry, CountryBiz.GetCountryList());

            AppUtil.SetAutoCompleteDropdown(new DropDownList[] {
                    cmbBrand,
                    cmbModel,
                    cmbCountry
                }
                                , Page
                                , this.ClientID + "_Autocomplete");

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private void BuildCombo(DropDownList cmb, List<ControlListData> lst) { BuildCombo(cmb, lst, ""); }
        private void BuildCombo(DropDownList cmb, List<ControlListData> lst, string Blanktext)
        {
            cmb.DataSource = lst;
            cmb.DataTextField = "TextField";
            cmb.DataValueField = "ValueField";
            cmb.DataBind();
            cmb.Items.Insert(0, new ListItem(Blanktext, ""));
        }
        private void SetModelCombo()
        {
            BuildCombo(cmbModel, SlmScr010Biz.GetModelDataNew(cmbBrand.SelectedValue), "กรุณาระบุ");
        }
        private void SetYeargroupCombo()
        {
            BuildCombo(cmbYearGroup, SlmScr010Biz.GetModelYearNew(cmbBrand.SelectedValue, cmbModel.SelectedValue), "กรุณาระบุ");
        }
        private void SetSubModelCombo()
        {
            BuildCombo(cmbSubModel, SlmScr010Biz.GetSubModelDataNew(cmbBrand.SelectedValue, cmbModel.SelectedValue, SLMUtil.SafeInt(cmbYearGroup.SelectedValue)), "กรุณาระบุ");
        }
        private void SetAmphur()
        {
            BuildCombo(cmbAmphur, SlmScr010Biz.GetAmphurDataNew(SLMUtil.SafeInt(cmbProvince.SelectedValue)));
        }
        private void SetTambol()
        {
            BuildCombo(cmbTambol, SlmScr010Biz.GetTambolDataNew(SLMUtil.SafeInt(cmbAmphur.SelectedValue)));
        }
        private void SetOAmphur()
        {
            BuildCombo(cmbOAmphur, SlmScr010Biz.GetAmphurDataNew(SLMUtil.SafeInt(cmbOProvince.SelectedValue)));
        }
        private void SetOTambol()
        {
            BuildCombo(cmbOTambol, SlmScr010Biz.GetTambolDataNew(SLMUtil.SafeInt(cmbOAmphur.SelectedValue)));
        }
        private void SetDAmphur()
        {
            BuildCombo(cmbDAmphur, SlmScr010Biz.GetAmphurDataNew(SLMUtil.SafeInt(cmbDProvince.SelectedValue)));
        }
        private void SetDTambol()
        {
            BuildCombo(cmbDTambol, SlmScr010Biz.GetTambolDataNew(SLMUtil.SafeInt(cmbDAmphur.SelectedValue)));
        }
        private void SetCC()
        {
            txtCarCC.Text = SlmScr010Biz.GetCCFromSubModel(cmbSubModel.SelectedValue);
        }
        private void SetCarTypeCombo()
        {
            var itm = SlmScr010Biz.GetInsuranceCarTypeDataByModel(cmbBrand.SelectedValue, cmbModel.SelectedValue).FirstOrDefault();
            if (itm != null)
                AppUtil.SetComboValue(cmbCarType, itm.ValueField);
        }

        protected void cmbAddType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPanelAddress();
        }

        private void SetPanelAddress()
        {
            pnlAddressContract.Visible = false;
            pnlAddressFact.Visible = false;
            pnlAddressSendDoc.Visible = false;

            switch (cmbAddrType.SelectedValue)
            {
                case "a1": pnlAddressContract.Visible = true; break;
                case "a2": pnlAddressFact.Visible = true; break;
                case "a3": pnlAddressSendDoc.Visible = true; break;
            }
            updAddress.Update();
        }

        protected void cmbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetModelCombo();
            SetYeargroupCombo();
            SetSubModelCombo();
            SetCarTypeCombo();
            ValidateData("brand");
        }
        protected void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetYeargroupCombo();
            SetSubModelCombo();
            SetCarTypeCombo();
            ValidateData("model");
        }
        protected void cmbYearGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetSubModelCombo();
            ValidateData("yeargroup");
        }
        protected void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAmphur();
            SetTambol();
        }
        protected void cmbAmphur_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTambol();
        }
        protected void cmbOProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetOAmphur();
            SetOTambol();
        }
        protected void cmbOAmphur_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetOTambol();
        }
        protected void cmbDProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDAmphur();
            SetDTambol();
        }
        protected void cmbDAmphur_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDTambol();
        }
        protected void chkCopyAddressContract1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCopyAddressContract1.Checked)
            {
                txtOAddressNo.Text = txtAddressNo.Text.Trim();
                txtOMoo.Text = txtMoo.Text.Trim();
                txtOSoi.Text = txtSoi.Text.Trim();
                txtOBuildingname.Text = txtBuildingName.Text.Trim();
                txtOStreet.Text = txtStreet.Text.Trim();
                txtOFloor.Text = txtFloor.Text.Trim();
                txtOPostCode.Text = txtPostalCode.Text.Trim();
                cmbOProvince.SelectedIndex = cmbProvince.SelectedIndex;
                SetOAmphur();
                cmbOAmphur.SelectedIndex = cmbAmphur.SelectedIndex;
                SetOTambol();
                cmbOTambol.SelectedIndex = cmbTambol.SelectedIndex;
            }
        }
        protected void chkCopyAddressContact2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCopyAddressContact2.Checked)
            {
                txtDAddressno.Text = txtAddressNo.Text.Trim();
                txtDMoo.Text = txtMoo.Text.Trim();
                txtDSoi.Text = txtSoi.Text.Trim();
                txtDBuilding.Text = txtBuildingName.Text.Trim();
                txtDStreet.Text = txtStreet.Text.Trim();
                txtDFloor.Text = txtFloor.Text.Trim();
                txtDPostCode.Text = txtPostalCode.Text.Trim();
                cmbDProvince.SelectedIndex = cmbProvince.SelectedIndex;
                SetDAmphur();
                cmbDAmphur.SelectedIndex = cmbAmphur.SelectedIndex;
                SetDTambol();
                cmbDTambol.SelectedIndex = cmbTambol.SelectedIndex;
            }
        }
        protected void cmbCardType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //cmbCountry.SelectedValue = "";
                lblCountryId.Text = "";
                //cmbCountry.Enabled = false;

                if (cmbCardType.SelectedItem.Value == "")
                {
                    lblCitizenId.Text = "";
                    txtCitizenId.Text = "";
                    vtxtCitizenId.Text = "";
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
                    lblCountryId.Text = "*";
                    AppUtil.SetCardTypeValidation(cmbCardType.SelectedItem.Value, txtCitizenId);
                    AppUtil.ValidateCardId(cmbCardType, txtCitizenId, vtxtCitizenId);

                    if (cmbCardType.SelectedItem.Value == AppConstant.CardType.Person)
                    {
                        //ถ้าเลือกบัตรประชาชน ให้ Default ประเทศไทย ตาม Config
                        if (cmbCountry.SelectedValue == "")
                            cmbCountry.SelectedValue = AppConstant.CBSLeadThaiCountryId.ToString();
                    }

                    //if (cmbCardType.SelectedValue == AppConstant.CardType.Foreigner)
                    //{
                    //    cmbCountry.Enabled = true;
                    //    lblCountryId.Text = "*";
                    //}
                    //else
                    //    cmbCountry.SelectedValue = AppConstant.CBSLeadThaiCountryId.ToString();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void cmbSubModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateData("submodel");
            if (cmbSubModel.SelectedIndex > 0) SetCC();
        }
        protected void cmbCarType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateData("cartype");
        }
        protected void cmbDocBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDAddress();
        }
        protected void cmbProvinceRegis_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateData("provinceregis");
        }
        private void SetDAddress()
        {
            var brn = BranchBiz.GetBranch(cmbDocBranch.SelectedValue);
            bool enb = (brn == null);
            if (brn == null) brn = new BranchData();

            txtDAddressno.Text = brn.slm_House_No;
            txtDBuilding.Text = brn.slm_Building;
            txtDFloor.Text = brn.slm_Village;
            txtDSoi.Text = brn.slm_Soi;
            txtDStreet.Text = brn.slm_Street;
            AppUtil.SetComboValue(cmbDProvince, brn.slm_ProvinceId.ToString());
            SetDAmphur();
            AppUtil.SetComboValue(cmbDAmphur, brn.slm_AmphurId.ToString());
            SetDTambol();
            AppUtil.SetComboValue(cmbDTambol, brn.slm_TambolId.ToString());
            txtDPostCode.Text = brn.slm_Zipcode;

            txtDAddressno.ReadOnly = !enb;
            txtDBuilding.ReadOnly = !enb;
            txtDFloor.ReadOnly = !enb;
            txtDSoi.ReadOnly = !enb;
            txtDStreet.ReadOnly = !enb;
            cmbDProvince.Enabled = enb;
            cmbDAmphur.Enabled = enb;
            cmbDTambol.Enabled = enb;
            txtDPostCode.ReadOnly = !enb;

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
        protected void txtContractNo_TextChanged(object sender, EventArgs e)
        {
            ValidateData("contractno");
        }
        protected void txtCarCC_TextChanged(object sender, EventArgs e)
        {
            ValidateData("cc");
        }
        protected void txtCarLicense_TextChanged(object sender, EventArgs e)
        {
            ValidateData("carlicense");
        }
        protected void txtCarChassie_TextChanged(object sender, EventArgs e)
        {
            ValidateData("chassie");
        }
        protected void txtCarEngine_TextChanged(object sender, EventArgs e)
        {
            ValidateData("carengine");
        }


        public LeadInfoBiz.LeadDetailInsData GetInsData()
        {
            var ins = new LeadInfoBiz.LeadDetailInsData();
            ins.slm_RenewInsureId = SLMUtil.SafeDecimal(hdfID.Value);
            ins.slm_ContractNo = txtContractNo.Text.Trim();
            if (cmbCardType.SelectedIndex > 0) ins.CardType = cmbCardType.SelectedValue;
            if (cmbCountry.SelectedIndex > 0) ins.CountryId = Convert.ToInt32(cmbCountry.SelectedValue);
            ins.IsCustomer = chkOldCustomer.Checked;
            ins.CitizenId = txtCitizenId.Text.Trim();
            if (tdBirthdate.DateValue.Year != 1) ins.Birthdate = tdBirthdate.DateValue;
            ins.MaritalStatus = rdoMarital.SelectedValue;
            if (cmbOccupation.SelectedIndex > 0) ins.Occupation = cmbOccupation.SelectedValue;
            ins.slm_CC = txtCarCC.Text.Trim();
            if (cmbBrand.SelectedIndex > 0) ins.slm_RedbookBrandCode = cmbBrand.SelectedValue;
            if (cmbModel.SelectedIndex > 0) ins.slm_RedbookModelCode = cmbModel.SelectedValue;
            if (cmbYearGroup.SelectedIndex > 0) ins.slm_RedbookYearGroup = SLMUtil.SafeInt(cmbYearGroup.SelectedValue);
            if (cmbSubModel.SelectedIndex > 0) ins.slm_RedbookKKKey = cmbSubModel.SelectedValue;
            //ins.slm_CoverageTypeId = SLMUtil.SafeInt(cmbCarType.SelectedValue);
            if (cmbCarType.SelectedIndex > 0) ins.slm_InsurancecarTypeId = SLMUtil.SafeInt(cmbCarType.SelectedValue);
            ins.slm_LicenseNo = txtCarLicense.Text.Trim();
            if (cmbProvinceRegis.SelectedItem.Value != "")
            {
                ins.ProvinceRegis = cmbProvinceRegis.SelectedItem.Value;
            }
            ins.slm_ChassisNo = txtCarChassie.Text.Trim();
            ins.slm_EngineNo = txtCarEngine.Text.Trim();
            ins.slm_SendDocFlag = chkSendToBranch.Checked ? "3" : "1";
            if (chkSendToBranch.Checked) ins.slm_SendDocBrandCode = cmbDocBranch.SelectedValue;

            // default?
            ins.slm_PolicyDiscountAmt = SLMUtil.SafeInt(hdfPolicyDiscountAmt.Value);
            ins.slm_PolicyGrossVat = SLMUtil.SafeInt(hdfPolicyGrossVat.Value);
            ins.slm_PolicyGrossStamp = SLMUtil.SafeInt(hdfPolicyGrossStamp.Value);
            ins.slm_PolicyGrossPremium = SLMUtil.SafeInt(hdfPolicyGrossPremium.Value);
            ins.slm_PolicyGrossPremiumTotal = SLMUtil.SafeInt(hdfPolicyGrossPremiumTotal.Value);
            ins.slm_ActGrossPremium = SLMUtil.SafeInt(hdfActGrossPremium.Value);
            ins.slm_ActNetPremium = SLMUtil.SafeInt(hdfActNetPremium.Value);
            ins.slm_ActVat = SLMUtil.SafeInt(hdfActVat.Value);
            ins.slm_ActStamp = SLMUtil.SafeInt(hdfActStamp.Value);

            return ins;
        }
        public void SetInsData(LeadInfoBiz.LeadDetailInsData ins, bool isCopy = false)
        {
            if (!isCopy)
                hdfID.Value = ins.slm_RenewInsureId.ToString();

            txtContractNo.Text = ins.slm_ContractNo;
            chkOldCustomer.Checked = ins.IsCustomer;
            AppUtil.SetComboValue(cmbCardType, ins.CardType);
            if (cmbCardType.SelectedValue != "")
            {
                txtCitizenId.Text = ins.CitizenId;
                txtCitizenId.ReadOnly = false;
                txtCitizenId.Enabled = true;
                lblCitizenId.Text = "*";
                lblCountryId.Text = "*";

                //if (cmbCardType.SelectedValue == AppConstant.CardType.Foreigner)
                //{
                //    lblCountryId.Text = "*";
                //    cmbCountry.Enabled = true;
                //}
            }
            tdBirthdate.DateValue = ins.Birthdate;
            AppUtil.SetComboValue(cmbCountry, ins.CountryId.ToString());
            rdoMarital.SelectedIndex = rdoMarital.Items.IndexOf(rdoMarital.Items.FindByValue(ins.MaritalStatus));

            AppUtil.SetComboValue(cmbOccupation, ins.Occupation);
            txtCarCC.Text = ins.slm_CC;
            AppUtil.SetComboValue(cmbBrand, ins.slm_RedbookBrandCode);
            SetModelCombo();
            AppUtil.SetComboValue(cmbModel, ins.slm_RedbookModelCode);
            SetYeargroupCombo();
            AppUtil.SetComboValue(cmbYearGroup, ins.slm_RedbookYearGroup.ToString());
            SetSubModelCombo();
            AppUtil.SetComboValue(cmbSubModel, ins.slm_RedbookKKKey);
            AppUtil.SetComboValue(cmbCarType, ins.slm_InsurancecarTypeId.ToString());
            txtCarLicense.Text = ins.slm_LicenseNo;
            if (!string.IsNullOrEmpty(ins.ProvinceRegis))
            {
                cmbProvinceRegis.SelectedIndex = cmbProvinceRegis.Items.IndexOf(cmbProvinceRegis.Items.FindByValue(ins.ProvinceRegis));
            }
            txtCarChassie.Text = ins.slm_ChassisNo;
            txtCarEngine.Text = ins.slm_EngineNo;
            if (ins.slm_SendDocFlag != null) chkSendToBranch.Checked = ins.slm_SendDocFlag == "3";
            if (ins.slm_SendDocBrandCode != null) AppUtil.SetComboValue(cmbDocBranch, ins.slm_SendDocBrandCode);
            cmbDocBranch.Visible = chkSendToBranch.Checked;
            SetDAddress();

            hdfPolicyDiscountAmt.Value = ins.slm_PolicyDiscountAmt.ToString();
            hdfPolicyGrossVat.Value = ins.slm_PolicyGrossVat.ToString();
            hdfPolicyGrossStamp.Value = ins.slm_PolicyGrossStamp.ToString();
            hdfPolicyGrossPremium.Value = ins.slm_PolicyGrossPremium.ToString();
            hdfPolicyGrossPremiumTotal.Value = ins.slm_PolicyGrossPremiumTotal.ToString();
            hdfActGrossPremium.Value = ins.slm_ActGrossPremium.ToString();
            hdfActNetPremium.Value = ins.slm_ActNetPremium.ToString();
            hdfActVat.Value = ins.slm_ActVat.ToString();
            hdfActStamp.Value = ins.slm_ActStamp.ToString();

            updMain.Update();
            updAddress.Update();
        }
        public List<LeadInfoBiz.LeadDetailAddress> GetInsAddrData()
        {
            List<LeadInfoBiz.LeadDetailAddress> adrLst = new List<LeadInfoBiz.LeadDetailAddress>();

            if (txtAddressNo.Text.Trim() != "" || cmbProvince.SelectedIndex >= 0)
            {
                adrLst.Add(new LeadInfoBiz.LeadDetailAddress()
                {
                    slm_RenewInsureAddressId = SLMUtil.SafeDecimal(hdfAddrID.Value),
                    slm_AddressType = "C",
                    slm_AddressNo = txtAddressNo.Text.Trim(),
                    slm_Moo = txtMoo.Text.Trim(),
                    slm_BuildingName = txtBuildingName.Text.Trim(),
                    slm_Street = txtStreet.Text.Trim(),
                    slm_Floor = txtFloor.Text.Trim(),
                    slm_Soi = txtSoi.Text.Trim(),
                    slm_Province = SLMUtil.SafeInt(cmbProvince.SelectedValue),
                    slm_Amphur = SLMUtil.SafeInt(cmbAmphur.SelectedValue),
                    slm_Tambon = SLMUtil.SafeInt(cmbTambol.SelectedValue),
                    slm_PostalCode = txtPostalCode.Text.Trim()
                });
            }

            if (txtOAddressNo.Text.Trim() != "" || cmbOProvince.SelectedIndex >= 0)
            {
                adrLst.Add(new LeadInfoBiz.LeadDetailAddress()
                {
                    slm_RenewInsureAddressId = SLMUtil.SafeDecimal(hdfOAddrID.Value),
                    slm_AddressType = "O",
                    slm_AddressNo = txtOAddressNo.Text.Trim(),
                    slm_Moo = txtOMoo.Text.Trim(),
                    slm_BuildingName = txtOBuildingname.Text.Trim(),
                    slm_Street = txtOStreet.Text.Trim(),
                    slm_Floor = txtOFloor.Text.Trim(),
                    slm_Soi = txtOSoi.Text.Trim(),
                    slm_Province = SLMUtil.SafeInt(cmbOProvince.SelectedValue),
                    slm_Amphur = SLMUtil.SafeInt(cmbOAmphur.SelectedValue),
                    slm_Tambon = SLMUtil.SafeInt(cmbOTambol.SelectedValue),
                    slm_PostalCode = txtOPostCode.Text.Trim()
                });
            }

            if (txtDAddressno.Text.Trim() != "" || cmbDProvince.SelectedIndex >= 0)
            {
                adrLst.Add(new LeadInfoBiz.LeadDetailAddress()
                {
                    slm_RenewInsureAddressId = SLMUtil.SafeDecimal(hdfDAddrID.Value),
                    slm_AddressType = "D",
                    slm_AddressNo = txtDAddressno.Text.Trim(),
                    slm_Moo = txtDMoo.Text.Trim(),
                    slm_BuildingName = txtDBuilding.Text.Trim(),
                    slm_Street = txtDStreet.Text.Trim(),
                    slm_Floor = txtDFloor.Text.Trim(),
                    slm_Soi = txtDSoi.Text.Trim(),
                    slm_Province = SLMUtil.SafeInt(cmbDProvince.SelectedValue),
                    slm_Amphur = SLMUtil.SafeInt(cmbDAmphur.SelectedValue),
                    slm_Tambon = SLMUtil.SafeInt(cmbDTambol.SelectedValue),
                    slm_PostalCode = txtDPostCode.Text.Trim(),
                });
            }


            return adrLst;
        }
        public void SetInsAddrData(List<LeadInfoBiz.LeadDetailAddress> adrLst)
        {
            var adr = adrLst.Where(a => a.slm_AddressType == "C").FirstOrDefault();
            var adrO = adrLst.Where(a => a.slm_AddressType == "O").FirstOrDefault();
            var adrD = adrLst.Where(a => a.slm_AddressType == "D").FirstOrDefault();

            if (adr != null)
            {
                txtAddressNo.Text = adr.slm_AddressNo;
                txtMoo.Text = adr.slm_Moo;
                txtBuildingName.Text = adr.slm_BuildingName;
                txtFloor.Text = adr.slm_Floor;
                txtSoi.Text = adr.slm_Soi;
                txtStreet.Text = adr.slm_Street;
                AppUtil.SetComboValue(cmbProvince, adr.slm_Province.ToString());
                SetAmphur();
                AppUtil.SetComboValue(cmbAmphur, adr.slm_Amphur.ToString());
                SetTambol();
                AppUtil.SetComboValue(cmbTambol, adr.slm_Tambon.ToString());
                txtPostalCode.Text = adr.slm_PostalCode;
            }

            if (adrO != null)
            {
                txtOAddressNo.Text = adrO.slm_AddressNo;
                txtOMoo.Text = adrO.slm_Moo;
                txtOBuildingname.Text = adrO.slm_BuildingName;
                txtOFloor.Text = adrO.slm_Floor;
                txtOSoi.Text = adrO.slm_Soi;
                txtOStreet.Text = adrO.slm_Street;
                AppUtil.SetComboValue(cmbOProvince, adrO.slm_Province.ToString());
                SetOAmphur();
                AppUtil.SetComboValue(cmbOAmphur, adrO.slm_Amphur.ToString());
                SetOTambol();
                AppUtil.SetComboValue(cmbOTambol, adrO.slm_Tambon.ToString());
                txtOPostCode.Text = adrO.slm_PostalCode;
            }

            if (adrD != null)
            {
                txtDAddressno.Text = adrD.slm_AddressNo;
                txtDMoo.Text = adrD.slm_Moo;
                txtDBuilding.Text = adrD.slm_BuildingName;
                txtDFloor.Text = adrD.slm_Floor;
                txtDSoi.Text = adrD.slm_Soi;
                txtDStreet.Text = adrD.slm_Street;
                AppUtil.SetComboValue(cmbDProvince, adrD.slm_Province.ToString());
                SetDAmphur();
                AppUtil.SetComboValue(cmbDAmphur, adrD.slm_Amphur.ToString());
                SetDTambol();
                AppUtil.SetComboValue(cmbDTambol, adrD.slm_Tambon.ToString());
                txtDPostCode.Text = adrD.slm_PostalCode;
            }

            updAddress.Update();
        }


        public bool ValidateData() { return ValidateData(""); }
        public bool ValidateData(string valtype)
        {
            int i = 0;

            //Validate เลขที่บัตร
            if (valtype == "" || valtype == "cardtype")
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

                    //if (cmbCardType.SelectedValue == AppConstant.CardType.Foreigner)
                    //{
                    //    if (cmbCountry.SelectedValue == "")
                    //    {
                    //        i += 1;
                    //        vcmbCountry.Text = "กรุณาเลือกประเทศ";
                    //    }
                    //    else
                    //        vcmbCountry.Text = "";
                    //}
                }
                else
                {
                    vtxtCitizenId.Text = "";
                }
            
            if (valtype == "" || valtype == "contractno") { if (txtContractNo.Text.Trim() == "") { i++; vtxtContract.Text = "กรุณาระบุเลขที่สัญญา"; } else  vtxtContract.Text = ""; }
            if (valtype == "" || valtype == "chassie") { if (txtCarChassie.Text.Trim() == "") { i++; vtxtChassis.Text = "กรุณาระบุเลขตัวถัง"; } else vtxtChassis.Text = ""; }
            if (valtype == "" || valtype == "cc") { if (txtCarCC.Text.Trim() == "") { i++; vtxtCC.Text = "กรุณาระบุซีซีรถ"; } else vtxtCC.Text = ""; }
            if (valtype == "" || valtype == "carlicense") { if (txtCarLicense.Text.Trim() == "") { i++; vtxtLicenseNo.Text = "กรุณาระบุทะเบียนรถ"; } else vtxtLicenseNo.Text = ""; }
            if (valtype == "" || valtype == "carengine") { if (txtCarEngine.Text.Trim() == "") { i++; vtxtEngineNo.Text = "กรุณาระบุเลขเครื่อง"; } else vtxtEngineNo.Text = ""; }
            if (valtype == "" || valtype == "brand") { if (cmbBrand.SelectedIndex <= 0) { i++; vtxtBrand.Text = "กรุณาระบุยี่ห้อรถ"; } else vtxtBrand.Text = ""; }
            if (valtype == "" || valtype == "model") { if (cmbModel.SelectedIndex <= 0) { i++; vtxtModel.Text = "กรุณาระบุรุ่น"; } else vtxtModel.Text = ""; }
            if (valtype == "" || valtype == "yeargroup") { if (cmbYearGroup.SelectedIndex <= 0) { i++; vtxtYearGroup.Text = "กรุณาระบุปีรถ"; } else vtxtYearGroup.Text = ""; }
            //if (valtype == "" || valtype == "submodel") if (cmbSubModel.SelectedIndex <= 0) { i++; vtxtSubModel.Text = "กรุณาระบุรุ่นย่อยรถ"; } else vtxtSubModel.Text = "";
            if (valtype == "" || valtype == "cartype") { if (cmbCarType.SelectedIndex <= 0) { i++; vtxtCartype.Text = "กรุณาระบุประเภทรถ"; } else vtxtCartype.Text = ""; }
            if (valtype == "" || valtype == "provinceregis") { if (cmbProvinceRegis.SelectedIndex <= 0) { i++; vcmbProvinceRegis.Text = "กรุณาระบุจังหวัดที่จดทะเบียน"; } else vcmbProvinceRegis.Text = ""; }

            updMain.Update();

            return i == 0;
        }
        public void SetControlMode(Lead_Detail_Master.CtlMode ctMode)
        {
            switch(ctMode)
            {
                case Lead_Detail_Master.CtlMode.New:
                    hdfID.Value = "";
                    break;

                case Lead_Detail_Master.CtlMode.Edit:
                    break;

                case Lead_Detail_Master.CtlMode.View:
                    //List<TextBox> tLst = new List<TextBox>();
                    //GetControlList<TextBox>(this.Controls, tLst);
                    //foreach (TextBox tb in tLst)
                    //{
                    //    tb.ReadOnly = true;
                    //    if (tb.CssClass != "Hidden") tb.CssClass = "TextboxView";
                    //}

                    //List<DropDownList> dLst = new List<DropDownList>();
                    //GetControlList<DropDownList>(this.Controls, dLst);
                    //foreach (DropDownList dd in dLst) dd.Enabled = false;

                    //List<CheckBox> cLst = new List<CheckBox>();
                    //GetControlList<CheckBox>(this.Controls, )

                    pnlAddressContract.Visible = true;
                    pnlAddressFact.Visible = true;
                    pnlAddressSendDoc.Visible = true;
                    trSelectAddress.Visible = false;
                    chkCopyAddressContract1.Visible = false;
                    chkCopyAddressContact2.Visible = false;

                    break;
            }
        }
        private void GetControlList<T>(ControlCollection controlCollection, List<T> resultCollection)
where T : Control
        {
            foreach (Control control in controlCollection)
            {
                //if (control.GetType() == typeof(T))
                if (control is T) // This is cleaner
                    resultCollection.Add((T)control);

                if (control.HasControls())
                    GetControlList(control.Controls, resultCollection);
            }
        }

        protected void chkSendToBranch_CheckedChanged(object sender, EventArgs e)
        {
            cmbDocBranch.Visible = chkSendToBranch.Checked;
            if (!chkSendToBranch.Checked)
            {
                cmbDocBranch.SelectedIndex = 0;
                SetDAddress();
            }
            updAddress.Update();
        }

        public DropDownList GetComboCardType()
        {
            return cmbCardType;
        }
        public DropDownList GetComboCountry()
        {
            return cmbCountry;
        }
        public TextBox GetTextBoxCitizenId()
        {
            return txtCitizenId;
        }
         
    }
}