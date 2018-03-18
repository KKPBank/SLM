using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    //public class PreleadData
    //{
    //    public decimal? slm_Prelead_Id { get; set; }
    //    public decimal? slm_TempId { get; set; }
    //    public decimal? slm_Cmt_Product_Id { get; set; }
    //    public decimal? slm_CmtLot { get; set; }
    //    public string slm_Contract_Number { get; set; }
    //    public string slm_Product_Id { get; set; }
    //    public string slm_CampaignId { get; set; }
    //    public string slm_BranchCode { get; set; }
    //    public string slm_Contract_Year { get; set; }
    //    public string slm_Contract_Status { get; set; }
    //    public int slm_TitleId { get; set; }
    //    public string slm_TitleName { get; set; }
    //    public string slm_Car_Category { get; set; }


    //    //public string slm_Customer_Key { get; set; }
    //    //public string slm_Title_Name_Org { get; set; }
    //    //public string slm_Name { get; set; }
    //    //public string slm_LastName { get; set; }
    //    //public string slm_CitizenId { get; set; }
    //    //public string slm_Marital_Status { get; set; }
    //    public int? slm_CardTypeId { get; set; }
    //    //public string slm_CardType_Org { get; set; }
    //    //public string slm_Birthdate { get; set; }
    //    //public string slm_Tax_Id { get; set; }
    //    //public string slm_OccupationId { get; set; }
    //    //public string slm_Career_Key_Org { get; set; }
    //    //public string slm_Career_Desc_Org { get; set; }
    //    //public string slm_Car_By_Gov_Id { get; set; }
    //    //public string slm_Car_By_Gov_Name_Org { get; set; }
    //    //public string slm_Brand_Code { get; set; }
    //    //public string slm_Brand_Code_Org { get; set; }
    //    //public string slm_Brand_Name_Org { get; set; }
    //    //public string slm_Model_Code { get; set; }
    //    //public string slm_Model_Code_Org { get; set; }
    //    //public string slm_Model_name_Org { get; set; }
    //    //public string slm_Engine_No { get; set; }
    //    //public string slm_Chassis_No { get; set; }
    //    //public string slm_Model_Year { get; set; }
    //    //public string slm_Car_License_No { get; set; }
    //    //public string slm_ProvinceRegis { get; set; }

    //    //public string slm_ProvinceRegis_Org { get; set; }
    //    //public string slm_Cc { get; set; }
    //    //public string slm_Expire_Date { get; set; }
    //    //public string slm_Voluntary_Mkt_Id { get; set; }
    //    //public string slm_Voluntary_Mkt_Id_Org { get; set; }
    //    //public string slm_Voluntary_Mkt_TitleId { get; set; }
    //    //public string slm_Voluntary_Mkt_Title_Org { get; set; }
    //    //public string slm_Voluntary_Mkt_First_Name { get; set; }
    //    //public string slm_Voluntary_Mkt_Last_Name { get; set; }
    //    public string slm_Voluntary_Policy_Number { get; set; }
    //    public string slm_Voluntary_Type_Key { get; set; }

    //    public string slm_ConverageTypename { get; set; }
    //    //public string slm_Voluntary_Type_Name { get; set; }
    //    //public string slm_Voluntary_Policy_Year { get; set; }

    //    public DateTime? slm_Voluntary_Policy_Exp_Date { get; set; }
    //    //public string slm_Voluntary_Policy_Exp_Year { get; set; }
    //    //public string slm_Voluntary_Policy_Exp_Month { get; set; }
    //    public decimal? slm_Voluntary_Cov_Amt { get; set; }
    //    public decimal? slm_Voluntary_Gross_Premium { get; set; }
    //    //public string slm_Voluntary_Channel_Key { get; set; }
    //    public string slm_Voluntary_Company_Code { get; set; }

    //    public string slm_insnameth { get; set; }
    //    //public string slm_Voluntary_Company_Name { get; set; }
    //    //public string slm_Benefit_TitleId { get; set; }
    //    //public string slm_Benefit_Title_Name_Org { get; set; }
    //    //public string slm_Benefit_First_Name { get; set; }
    //    //public string slm_Benefit_Last_Name { get; set; }
    //    //public string slm_Benefit_Telno { get; set; }
    //    public string slm_Driver_TitleId1 { get; set; }
    //    public string TitleName1 { get; set; }
    //    //public string slm_Driver_Title_Name1_Org { get; set; }
    //    public string slm_Driver_First_Name1 { get; set; }
    //    public string slm_Driver_Last_Name1 { get; set; }
    //    //public string slm_Driver_Telno1 { get; set; }
    //    public DateTime? slm_Driver_Birthdate1 { get; set; }
    //    public string slm_Driver_TitleId2 { get; set; }
    //    public string TitleName2 { get; set; }
    //    //public string slm_Driver_Title_Name2_Org { get; set; }
    //    public string slm_Driver_First_Name2 { get; set; }
    //    public string slm_Driver_Last_Name2 { get; set; }
    //    //public string slm_Driver_Telno2 { get; set; }
    //    public DateTime? slm_Driver_Birthdate2 { get; set; }
    //    //public string slm_Compulsory_Policy_Number { get; set; }
    //    //public string slm_Compulsory_Policy_Year { get; set; }
    //    //public string slm_Compulsory_Policy_Eff_Date { get; set; }
    //    //public string slm_Compulsory_Policy_Exp_Date { get; set; }
    //    //public string slm_Compulsory_Cov_Amt { get; set; }

    //    //public string slm_Compulsory_Gross_Premium { get; set; }
    //    //public string slm_Compulsory_Company_Code { get; set; }
    //    //public string slm_Compulsory_Company_Name { get; set; }
    //    //public string slm_Guarantor_Code1 { get; set; }
    //    //public string slm_Guarantor_TitleId1 { get; set; }
    //    //public string slm_Guarantor_Title_Name1_Org { get; set; }
    //    //public string slm_Guarantor_First_Name1 { get; set; }
    //    //public string slm_Guarantor_Last_Name1 { get; set; }
    //    //public string slm_Guarantor_Card_Id1 { get; set; }
    //    //public string slm_Guarantor_RelationId1 { get; set; }
    //    //public string slm_Guarantor_Relation1_Org { get; set; }
    //    //public string slm_Guarantor_Telno1 { get; set; }
    //    //public string slm_Guarantor_Code2 { get; set; }
    //    //public string slm_Guarantor_TitleId2 { get; set; }
    //    //public string slm_Guarantor_Title_Name2_Org { get; set; }
    //    //public string slm_Guarantor_First_Name2 { get; set; }
    //    //public string slm_Guarantor_Last_Name2 { get; set; }
    //    //public string slm_Guarantor_Card_Id2 { get; set; }
    //    //public string slm_Guarantor_RelationId2 { get; set; }
    //    //public string slm_Guarantor_Relation2_Org { get; set; }
    //    //public string slm_Guarantor_Telno2 { get; set; }
    //    //public string slm_Guarantor_Code3 { get; set; }
    //    //public string slm_Guarantor_TitleId3 { get; set; }
    //    //public string slm_Guarantor_Title_Name3_Org { get; set; }
    //    //public string slm_Guarantor_First_Name3 { get; set; }
    //    //public string slm_Guarantor_Last_Name3 { get; set; }
    //    //public string slm_Guarantor_Card_Id3 { get; set; }
    //    //public string slm_Guarantor_RelationId3 { get; set; }
    //    //public string slm_Guarantor_Relation3_Org { get; set; }
    //    //public string slm_Guarantor_Telno3 { get; set; }
    //    //public string slm_Grade { get; set; }
    //    //public string slm_View_UpdateDate { get; set; }
    //    //public string slm_Email { get; set; }
    //    //public string slm_sms { get; set; }
    //    //public string slm_Owner { get; set; }
    //    //public string slm_OwnerBranch { get; set; }
    //    //public string slm_Owner_Position { get; set; }

    //    //public string slm_Owner_Team { get; set; }
    //    //public string slm_AssignDate { get; set; }
    //    //public string slm_AssignFlag { get; set; }
    //    //public string slm_AssignType { get; set; }
    //    //public string slm_AssignDescription { get; set; }
    //    //public string slm_Assign_Status { get; set; }
    //    //public string slm_Status { get; set; }
    //    //public string slm_SubStatus { get; set; }
    //    //public string slm_Counting { get; set; }
    //    //public string slm_NextSLA { get; set; }
    //    //public string slm_TicketId { get; set; }
    //    //public string slm_CreatedDate { get; set; }
    //    //public string slm_CreatedBy { get; set; }
    //    //public string slm_UpdatedDate { get; set; }
    //    //public string slm_UpdatedBy { get; set; }
    //    //public string is_Deleted { get; set; }
    //    //public string slm_NextContactDate { get; set; }
    //    //public string slm_ObtPro03Flag { get; set; }
    //    //public string slm_AssignBy { get; set; }
    //    //public string slm_Current_Status { get; set; }
    //    //public string slm_Curent_Substatus { get; set; }
    //    //public string slm_Voluntary_First_Create_Date { get; set; }
    //    //public string slm_Type { get; set; }
    //    //public string slm_RemarkPayment { get; set; }




    //}

    #region beer
    [Serializable]
    public class PreleadData
    {
        public decimal slm_PreLeadCompareId { get; set; }

        public decimal? slm_Prelead_Id { get; set; }

        public long? slm_NotifyPremiumId { get; set; }

        public decimal? slm_PromotionId { get; set; }

        public int? slm_Seq { get; set; }

        public string slm_Year { get; set; }

        public string slm_Ins_Com_Id_str { get; set; }

        public decimal? slm_Ins_Com_Id_get { get; set; }

        public decimal? slm_Ins_Com_Id { get; set; }

        public string slm_CoverageTypeId { get; set; }

        public decimal? slm_InjuryDeath { get; set; }

        public decimal? slm_TPPD { get; set; }

        public int? slm_RepairTypeId { get; set; }

        public decimal? slm_OD { get; set; }

        public decimal? slm_FT { get; set; }

        public decimal? slm_DeDuctible { get; set; }

        public decimal? slm_PersonalAccident { get; set; }

        public string slm_PersonalAccidentMan { get; set; }

        public decimal? slm_MedicalFee { get; set; }

        public string slm_MedicalFeeMan { get; set; }

        public decimal? slm_InsuranceDriver { get; set; }

        public decimal? slm_PolicyGrossStamp { get; set; }

        public decimal? slm_PolicyGrossVat { get; set; }

        public decimal? slm_PolicyGrossPremium { get; set; }

        public decimal? slm_NetGrossPremium { get; set; }

        public decimal? slm_PolicyGrossPremiumPay { get; set; }

        public decimal? slm_CostSave { get; set; }

        public DateTime? slm_CreatedDate { get; set; }

        public string slm_CreatedBy { get; set; }

        public DateTime? slm_UpdatedDate { get; set; }

        public string slm_UpdatedBy { get; set; }

        public bool? slm_Selected { get; set; }

        public string slm_OldPolicyNo { get; set; }

        public string slm_DriverFlag { get; set; }

        public int? slm_Driver_TitleId1 { get; set; }

        public string slm_Driver_First_Name1 { get; set; }

        public string slm_Driver_Last_Name1 { get; set; }

        public DateTime? slm_Driver_Birthdate1 { get; set; }

        public int? slm_Driver_TitleId2 { get; set; }

        public string slm_Driver_First_Name2 { get; set; }

        public string slm_Driver_Last_Name2 { get; set; }

        public DateTime? slm_Driver_Birthdate2 { get; set; }

        public string slm_OldReceiveNo { get; set; }

        public DateTime? slm_PolicyStartCoverDate { get; set; }

        public DateTime? slm_PolicyEndCoverDate { get; set; }

        public bool? slm_Vat1Percent { get; set; }

        public int? slm_DiscountPercent { get; set; }

        public decimal? slm_DiscountBath { get; set; }

        public decimal? slm_Vat1PercentBath { get; set; }

        public string slm_CoverageTypename { get; set; }

        public string slm_insnameth { get; set; }

        public string slm_TitleName1 { get; set; }

        public string slm_TitleName2 { get; set; }

        public int? slm_CardTypeId { get; set; }

        public String slm_CardTypeName { get; set; }

        public DateTime? slm_ActStartCoverDate { get; set; }

        public DateTime? slm_ActEndCoverDate { get; set; }

        public decimal? slm_Compulsory_Gross_Premium { get; set; }

        public int? slm_DateCount { get; set; }

        public string slm_DeDuctiblePro { get; set; }

        public string slm_Cc { get; set; }

        public decimal? slm_NetPremium { get; set; }

        public string slm_Model_name_Org { get; set; }

        public string slm_Brand_Name_Org { get; set; }

        public decimal? slm_GrossPremium { get; set; }

        public decimal? slm_Stamp { get; set; }

        public decimal? slm_Vat { get; set; }

        public DateTime? slm_InsExpireDate { get; set; }

        public string slm_RepairTypeName { get; set; }

        public int? slm_SendDocType { get; set; }

        public int? slm_ActIssuePlace { get; set; }

        public String slm_ActNo { get; set; }

        public string slm_ActIssuaBranch { get; set; }

        public DateTime? slm_CarTaxExpiredDate { get; set; }

        public decimal? slm_Act { get; set; }

        public decimal? slm_ActGrossStamp { get; set; }

        public decimal? slm_ActGrossVat { get; set; }

        public decimal? slm_ActGrossPremium { get; set; }

        public decimal? slm_ActNetGrossPremium { get; set; }

        public decimal? slm_ActGrossPremiumPay { get; set; }

        public string slm_ActSignNo { get; set; }

        public decimal? slm_ActNetGrossPremiumFull { get; set; }

        public string slm_House_No { get; set; }

        public string slm_Floor { get; set; }

        public string slm_House_Name { get; set; }

        public string slm_Moo { get; set; }

        public string slm_Building { get; set; }

        public string slm_Village { get; set; }

        public string slm_Soi { get; set; }

        public string slm_Street { get; set; }

        public int? slm_TambolId { get; set; }

        public int? slm_Amphur_Id { get; set; }

        public int? slm_Province_Id { get; set; }

        public string slm_Zipcode { get; set; }

        public decimal? slm_Voluntary_Gross_Premium { get; set; }

        public DateTime? slm_Voluntary_Policy_Eff_Date { get; set; }

        public string slm_Model_Code_Org { get; set; }

        public decimal? slm_CmtLot { get; set; }
        public string slm_Contract_Number { get; set; }
        public string slm_Customer_Key { get; set; }
        public decimal? slm_PersonalAccidentPassenger { get; set; }
        public decimal? slm_PersonalAccidentDriver { get; set; }
        public decimal? slm_MedicalFeeDriver { get; set; }
        public decimal? slm_MedicalFeePassenger { get; set; }
        public string slm_CitizenId { get; set; }
        public string slm_Owner { get; set; }

        public string slm_Product_Id { get; set; }
        public string slm_Ticket_Id { get; set; }

        public string slm_Brand_Code { get; set; }
        public string slm_Model_Code { get; set; }
        
        public string slm_SendDocFlag { get; set; }
        public string slm_SendDocBrandCode { get; set; }

        public string slm_Receiver { get; set; }
        public string slm_RemarkPayment { get; set; }

        public string slm_Voluntary_Type_Name { get; set; }

        //Added by Pom 15/08/2016
        public string slm_RemarkPolicy { get; set; }
        public string slm_RemarkAct { get; set; }
        public string slm_Chassis_No { get; set; }

        // added by zz 2017-05-31
        public string slm_BranchCode { get; set; }
        //Added 2017-07-26
        public string slm_Model_Year { get; set; }
        public int? slm_Car_By_Gov_Id { get; set; }
        public string slm_Car_By_Gov_Name_Org { get; set; }
        public int? slm_ProvinceRegis { get; set; }
        public string slm_ProvinceRegis_Org { get; set; }
        public string ClientFullname { get; set; }
        public string slm_Cc_Org { get; set; }
        public string slm_Model_Year_Org { get; set; }
    }

    [Serializable]
    public class PreleadDataCollection
    {
        public PreleadData Prev { get; set; }

        public PreleadData Curr { get; set; }

        public PreleadData Promo { get; set; }

        public PreleadData Opt { get; set; }
    }

    public class PreleadDataCollectionSave
    {
        public PreleadData Prev { get; set; }

        public PreleadData Curr { get; set; }

        public PreleadData Promo1 { get; set; }

        public PreleadData Promo2 { get; set; }

        public PreleadData Promo3 { get; set; }

        public PreleadData Opt { get; set; }

        public PreleadData PreleadAddress { get; set; }
    }

    [Serializable]
    public class PreleadDataForAct
    {
        public decimal slm_Prelead_Id { get; set; }
        public Nullable<decimal> slm_TempId { get; set; }
        public Nullable<decimal> slm_Cmt_Product_Id { get; set; }
        public Nullable<decimal> slm_CmtLot { get; set; }
        public string slm_Contract_Number { get; set; }
        public string slm_Product_Id { get; set; }
        public string slm_CampaignId { get; set; }
        public string slm_BranchCode { get; set; }
        public string slm_Contract_Year { get; set; }
        public string slm_Contract_Status { get; set; }
        public string slm_Car_Category { get; set; }
        public string slm_Customer_Key { get; set; }
        public Nullable<int> slm_TitleId { get; set; }
        public string slm_Title_Name_Org { get; set; }
        public string slm_Name { get; set; }
        public string slm_LastName { get; set; }
        public string slm_CitizenId { get; set; }
        public string slm_Marital_Status { get; set; }
        public Nullable<int> slm_CardTypeId { get; set; }
        public string slm_CardType_Org { get; set; }
        public Nullable<System.DateTime> slm_Birthdate { get; set; }
        public string slm_Tax_Id { get; set; }
        public Nullable<int> slm_OccupationId { get; set; }
        public string slm_Career_Key_Org { get; set; }
        public string slm_Career_Desc_Org { get; set; }
        public Nullable<int> slm_Car_By_Gov_Id { get; set; }
        public string slm_Car_By_Gov_Name_Org { get; set; }
        public string slm_Brand_Code { get; set; }
        public string slm_Brand_Code_Org { get; set; }
        public string slm_Brand_Name_Org { get; set; }
        public string slm_Model_Code { get; set; }
        public string slm_Model_Code_Org { get; set; }
        public string slm_Model_name_Org { get; set; }
        public string slm_Engine_No { get; set; }
        public string slm_Chassis_No { get; set; }
        public string slm_Model_Year { get; set; }
        public string slm_Car_License_No { get; set; }
        public Nullable<int> slm_ProvinceRegis { get; set; }
        public string slm_ProvinceRegis_Org { get; set; }
        public string slm_Cc { get; set; }
        public Nullable<System.DateTime> slm_Expire_Date { get; set; }
        public string slm_Voluntary_Mkt_Id { get; set; }
        public string slm_Voluntary_Mkt_Id_Org { get; set; }
        public Nullable<int> slm_Voluntary_Mkt_TitleId { get; set; }
        public string slm_Voluntary_Mkt_Title_Org { get; set; }
        public string slm_Voluntary_Mkt_First_Name { get; set; }
        public string slm_Voluntary_Mkt_Last_Name { get; set; }
        public string slm_Voluntary_Policy_Number { get; set; }
        public string slm_Voluntary_Type_Key { get; set; }
        public string slm_Voluntary_Type_Name { get; set; }
        public string slm_Voluntary_Policy_Year { get; set; }
        public Nullable<System.DateTime> slm_Voluntary_Policy_Eff_Date { get; set; }
        public Nullable<System.DateTime> slm_Voluntary_Policy_Exp_Date { get; set; }
        public string slm_Voluntary_Policy_Exp_Year { get; set; }
        public string slm_Voluntary_Policy_Exp_Month { get; set; }
        public Nullable<decimal> slm_Voluntary_Cov_Amt { get; set; }
        public Nullable<decimal> slm_Voluntary_Gross_Premium { get; set; }
        public string slm_Voluntary_Channel_Key { get; set; }
        public string slm_Voluntary_Company_Code { get; set; }
        public string slm_Voluntary_Company_Name { get; set; }
        public Nullable<int> slm_Benefit_TitleId { get; set; }
        public string slm_Benefit_Title_Name_Org { get; set; }
        public string slm_Benefit_First_Name { get; set; }
        public string slm_Benefit_Last_Name { get; set; }
        public string slm_Benefit_Telno { get; set; }
        public Nullable<int> slm_Driver_TitleId1 { get; set; }
        public string slm_Driver_Title_Name1_Org { get; set; }
        public string slm_Driver_First_Name1 { get; set; }
        public string slm_Driver_Last_Name1 { get; set; }
        public string slm_Driver_Telno1 { get; set; }
        public Nullable<System.DateTime> slm_Driver_Birthdate1 { get; set; }
        public Nullable<int> slm_Driver_TitleId2 { get; set; }
        public string slm_Driver_Title_Name2_Org { get; set; }
        public string slm_Driver_First_Name2 { get; set; }
        public string slm_Driver_Last_Name2 { get; set; }
        public string slm_Driver_Telno2 { get; set; }
        public Nullable<System.DateTime> slm_Driver_Birthdate2 { get; set; }
        public string slm_Compulsory_Policy_Number { get; set; }
        public string slm_Compulsory_Policy_Year { get; set; }
        public Nullable<System.DateTime> slm_Compulsory_Policy_Eff_Date { get; set; }
        public Nullable<System.DateTime> slm_Compulsory_Policy_Exp_Date { get; set; }
        public Nullable<decimal> slm_Compulsory_Cov_Amt { get; set; }
        public Nullable<decimal> slm_Compulsory_Gross_Premium { get; set; }
        public string slm_Compulsory_Company_Code { get; set; }
        public string slm_Compulsory_Company_Name { get; set; }
        public string slm_Guarantor_Code1 { get; set; }
        public Nullable<int> slm_Guarantor_TitleId1 { get; set; }
        public string slm_Guarantor_Title_Name1_Org { get; set; }
        public string slm_Guarantor_First_Name1 { get; set; }
        public string slm_Guarantor_Last_Name1 { get; set; }
        public string slm_Guarantor_Card_Id1 { get; set; }
        public Nullable<int> slm_Guarantor_RelationId1 { get; set; }
        public string slm_Guarantor_Relation1_Org { get; set; }
        public string slm_Guarantor_Telno1 { get; set; }
        public string slm_Guarantor_Code2 { get; set; }
        public Nullable<int> slm_Guarantor_TitleId2 { get; set; }
        public string slm_Guarantor_Title_Name2_Org { get; set; }
        public string slm_Guarantor_First_Name2 { get; set; }
        public string slm_Guarantor_Last_Name2 { get; set; }
        public string slm_Guarantor_Card_Id2 { get; set; }
        public Nullable<int> slm_Guarantor_RelationId2 { get; set; }
        public string slm_Guarantor_Relation2_Org { get; set; }
        public string slm_Guarantor_Telno2 { get; set; }
        public string slm_Guarantor_Code3 { get; set; }
        public Nullable<int> slm_Guarantor_TitleId3 { get; set; }
        public string slm_Guarantor_Title_Name3_Org { get; set; }
        public string slm_Guarantor_First_Name3 { get; set; }
        public string slm_Guarantor_Last_Name3 { get; set; }
        public string slm_Guarantor_Card_Id3 { get; set; }
        public Nullable<int> slm_Guarantor_RelationId3 { get; set; }
        public string slm_Guarantor_Relation3_Org { get; set; }
        public string slm_Guarantor_Telno3 { get; set; }
        public string slm_Grade { get; set; }
        public Nullable<System.DateTime> slm_View_UpdateDate { get; set; }
        public string slm_Email { get; set; }
        public string slm_sms { get; set; }
        public string slm_Owner { get; set; }
        public string slm_OwnerBranch { get; set; }
        public Nullable<int> slm_Owner_Position { get; set; }
        public Nullable<int> slm_Owner_Team { get; set; }
        public Nullable<System.DateTime> slm_AssignDate { get; set; }
        public string slm_AssignFlag { get; set; }
        public string slm_AssignType { get; set; }
        public string slm_AssignDescription { get; set; }
        public string slm_Assign_Status { get; set; }
        public string slm_Status { get; set; }
        public string slm_SubStatus { get; set; }
        public Nullable<decimal> slm_Counting { get; set; }
        public Nullable<System.DateTime> slm_NextSLA { get; set; }
        public string slm_TicketId { get; set; }
        public Nullable<System.DateTime> slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public Nullable<System.DateTime> slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public Nullable<bool> is_Deleted { get; set; }
        public Nullable<System.DateTime> slm_NextContactDate { get; set; }
        public Nullable<bool> slm_ObtPro03Flag { get; set; }
        public string slm_AssignBy { get; set; }
        public string slm_Current_Status { get; set; }
        public string slm_Curent_Substatus { get; set; }
        public Nullable<System.DateTime> slm_Voluntary_First_Create_Date { get; set; }
        public string slm_Type { get; set; }
        public string slm_RemarkPayment { get; set; }
        public Nullable<int> slm_ThisWork { get; set; }
        public Nullable<int> slm_TotalAlert { get; set; }
        public Nullable<int> slm_TotalWork { get; set; }
        public Nullable<System.DateTime> slm_CurrentSLA { get; set; }
        public string slm_SendDocFlag { get; set; }
        public string slm_SendDocBrandCode { get; set; }
        public string slm_Receiver { get; set; }
    }

    [Serializable]
    public class PreleadDataForActCollection
    {
        public PreleadDataForAct Prev { get; set; }
        public PreleadDataForAct Curr { get; set; }
        public PreleadDataForAct Promo1 { get; set; }
        public PreleadDataForAct Promo2 { get; set; }
        public PreleadDataForAct Promo3 { get; set; }
        public PreleadDataForAct Opt { get; set; }
    }
    #endregion
    [Serializable]
    public class PreleadCompareDataCollectionGroup
    {
        public PreleadCompareDataCollection PreleadCompareDataCollectionMain { get; set; }
        public List<PreleadCompareDataCollection> PreleadCompareDataCollections { get; set; }
    }
    [Serializable]
    public class PreleadCompareDataCollection
    {
        public RenewInsuranceData RenewIns { get; set; }

        public LeadDataForRenewInsure lead { get; set; }

        public PreleadData Prelead { get; set; }

        public PreleadCompareData ComparePrev { get; set; }

        public PreleadCompareData CompareCurr { get; set; }

        public List<PreleadCompareData> ComparePromoList { get; set; }

        //public PreleadCompareData CompareOpt { get; set; }

        public PreleadCompareActData ActPrev { get; set; }

        public List<PreleadCompareActData> ActPromoList { get; set; }

        //public PreleadCompareActData ActOpt { get; set; }

        public PreleadAddressData Address { get; set; }

        public List<RenewInsurancePaymentMainData> PayMainList { get; set; }

        public List<RenewInsurancePaymentMainData> PayMainActList { get; set; }

        public List<PaymentTransMainData> PaymentTransMainList { get; set; }

        public List<PaymentTransData> PaymentTransList { get; set; }

        public List<ProblemDetailData> ProblemList { get; set; }

        public List<RenewInsuranceReceiptData> ReceiptList { get; set; }

        public List<RenewInsuranceReceiptDetailData> ReceiptDetailList { get; set; }

        public List<RenewInsuranceReceiptRevisionDetailData> ReceiptRevisionDetailList { get; set; }

        public string keyTab { get; set; }
        public decimal? PolicyRecAmt { get; set; }
        public decimal? ActRecAmt { get; set; }

        public bool EditReceiptFlag { get; set; }
        public string PrintFlag { get; set; }
        public string ReceipNo { get; set; }
        public string StrReceipActDate { get; set; }
        public bool FlagFirstReceivePolicy { get; set; }
        public bool FlagFirstReceiveAct { get; set; }
        public bool FlagManualActIncentive { get; set;}
    }

    [Serializable]
    public class PreleadCompareData
    {
        public decimal slm_PreLeadCompareId { get; set; }
        public Nullable<decimal> slm_Prelead_Id { get; set; }
        public Nullable<long> slm_NotifyPremiumId { get; set; }
        public Nullable<decimal> slm_PromotionId { get; set; }
        public Nullable<int> slm_Seq { get; set; }
        public string slm_Year { get; set; }
        public Nullable<decimal> slm_Ins_Com_Id { get; set; }
        public Nullable<int> slm_CoverageTypeId { get; set; }
        public Nullable<decimal> slm_InjuryDeath { get; set; }
        public Nullable<decimal> slm_TPPD { get; set; }
        public Nullable<int> slm_RepairTypeId { get; set; }
        public string slm_RepairTypeName { get; set; }
        public Nullable<decimal> slm_OD { get; set; }
        public Nullable<decimal> slm_FT { get; set; }
        public Nullable<decimal> slm_DeDuctible { get; set; }
        public Nullable<decimal> slm_PersonalAccident { get; set; }
        public string slm_PersonalAccidentPassenger { get; set; }
        public string slm_PersonalAccidentDriver { get; set; }
        public Nullable<decimal> slm_MedicalFee { get; set; }
        public string slm_MedicalFeePassenger { get; set; }
        public string slm_MedicalFeeDriver { get; set; }
        public Nullable<decimal> slm_InsuranceDriver { get; set; }
        public Nullable<decimal> slm_PolicyGrossStamp { get; set; }
        public Nullable<decimal> slm_PolicyGrossVat { get; set; }
        public Nullable<decimal> slm_PolicyGrossPremium { get; set; }
        public Nullable<decimal> slm_NetGrossPremium { get; set; }
        public Nullable<decimal> slm_PolicyGrossPremiumPay { get; set; }
        public Nullable<decimal> slm_CostSave { get; set; }
        public Nullable<System.DateTime> slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public Nullable<System.DateTime> slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public Nullable<bool> slm_Selected { get; set; }
        public string slm_OldPolicyNo { get; set; }
        public string slm_DriverFlag { get; set; }
        public Nullable<int> slm_Driver_TitleId1 { get; set; }
        public string slm_Driver_First_Name1 { get; set; }
        public string slm_Driver_Last_Name1 { get; set; }
        public Nullable<System.DateTime> slm_Driver_Birthdate1 { get; set; }
        public Nullable<int> slm_Driver_TitleId2 { get; set; }
        public string slm_Driver_First_Name2 { get; set; }
        public string slm_Driver_Last_Name2 { get; set; }
        public Nullable<System.DateTime> slm_Driver_Birthdate2 { get; set; }
        public string slm_OldReceiveNo { get; set; }
        public Nullable<System.DateTime> slm_PolicyStartCoverDate { get; set; }
        public Nullable<System.DateTime> slm_PolicyEndCoverDate { get; set; }
        public Nullable<bool> slm_Vat1Percent { get; set; }
        public Nullable<decimal> slm_DiscountPercent { get; set; }
        public Nullable<decimal> slm_DiscountBath { get; set; }
        public Nullable<decimal> slm_Vat1PercentBath { get; set; }
        public Nullable<int> slm_CardTypeId { get; set; }
        public string PolicyDiscountRemark { get; set; }

        //extened
        public string slm_CoverageTypeName { get; set; }
        public string slm_insnameth { get; set; }
        public string slm_TitleName1 { get; set; }
        public string slm_TitleName2 { get; set; }
        public Nullable<int> slm_DateCount { get; set; }
        public string slm_CardTypeName { get; set; }

        //for renewInsure
        public Nullable<decimal> slm_RenewInsureId { get; set; }

        //Added by Pom 15/08/2016
        public string slm_DeDuctibleFlag { get; set; }
        //Added 23/05/2017
        public string slm_PeriodYear { get; set; }
    }
    [Serializable]
    public class PreleadCompareActData
    {
        public decimal slm_PreLeadCompareActId { get; set; }
        public Nullable<decimal> slm_Prelead_Id { get; set; }
        public Nullable<decimal> slm_PromotionId { get; set; }
        public Nullable<int> slm_Seq { get; set; }
        public string slm_Year { get; set; }
        public Nullable<decimal> slm_Ins_Com_Id { get; set; }
        public Nullable<int> slm_ActIssuePlace { get; set; }
        public Nullable<int> slm_SendDocType { get; set; }
        public string slm_ActNo { get; set; }
        public Nullable<System.DateTime> slm_ActStartCoverDate { get; set; }
        public Nullable<System.DateTime> slm_ActEndCoverDate { get; set; }
        public string slm_ActIssueBranch { get; set; }
        public Nullable<System.DateTime> slm_CarTaxExpiredDate { get; set; }
        public Nullable<decimal> slm_ActGrossStamp { get; set; }
        public Nullable<decimal> slm_ActGrossVat { get; set; }
        public Nullable<decimal> slm_ActGrossPremium { get; set; }
        public Nullable<decimal> slm_ActNetGrossPremium { get; set; }
        public Nullable<decimal> slm_ActGrossPremiumPay { get; set; }
        public Nullable<System.DateTime> slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public Nullable<System.DateTime> slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public Nullable<bool> slm_ActPurchaseFlag { get; set; }
        public Nullable<decimal> slm_DiscountPercent { get; set; }
        public Nullable<decimal> slm_DiscountBath { get; set; }
        public string slm_ActSignNo { get; set; }
        public Nullable<decimal> slm_ActNetGrossPremiumFull { get; set; }
        public Nullable<bool> slm_Vat1Percent { get; set; }
        public Nullable<decimal> slm_Vat1PercentBath { get; set; }

        //Extend
        public string slm_insnameth { get; set; }
        public string slm_ActIssuePlaceDesc { get; set; }
        public string slm_ActIssueBranchDesc { get; set; }
        public string slm_SendDocTypeDesc { get; set; }
        public string slm_BranchName { get; set; }

        //for renewInsure
        public Nullable<decimal> slm_RenewInsureId { get; set; }
    }
    [Serializable]
    public class PreleadAddressData
    {
        public decimal slm_Prelead_Address_Id { get; set; }
        public Nullable<decimal> slm_CmtLot { get; set; }
        public Nullable<decimal> slm_Prelead_Id { get; set; }
        public string slm_Customer_Key { get; set; }
        public string slm_Address_Type { get; set; }
        public string slm_Contract_Adrress { get; set; }
        public string slm_House_No { get; set; }
        public string slm_Moo { get; set; }
        public string slm_Building { get; set; }
        public string slm_Village { get; set; }
        public string slm_Soi { get; set; }
        public string slm_Street { get; set; }
        public Nullable<int> slm_TambolId { get; set; }
        public string slm_Tambon_Name_Org { get; set; }
        public Nullable<int> slm_Amphur_Id { get; set; }
        public string slm_Amphur_Name_Org { get; set; }
        public Nullable<int> slm_Province_Id { get; set; }
        public string slm_Province_name_Org { get; set; }
        public string slm_Zipcode { get; set; }
        public string slm_Home_Phone { get; set; }
        public string slm_Mobile_Phone { get; set; }
        public Nullable<System.DateTime> slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public Nullable<System.DateTime> slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public bool is_Deleted { get; set; }
    }
    [Serializable]
    public class PromotionInsuranceData
    {
        public decimal slm_PromotionId { get; set; }
        public string slm_Product_Id { get; set; }
        public decimal? slm_Ins_Com_Id { get; set; }
        public decimal? slm_CampaignInsuranceId { get; set; }
        public int? slm_DurationYear { get; set; }
        public DateTime? slm_EffectiveDateFrom { get; set; }
        public DateTime? slm_EffectiveDateTo { get; set; }
        public string slm_Brand_Code { get; set; }
        public string slm_Model_Code { get; set; }
        public string slm_UseCarType { get; set; }
        public int? slm_CoverageTypeId { get; set; }
        public string slm_AgeDrivenFlag { get; set; }
        public int? slm_RepairTypeId { get; set; }
        public int? slm_AgeCarYear { get; set; }
        //public decimal? slm_EngineSize { get; set; }
        public string slm_EngineSize { get; set; }
        public decimal? slm_OD { get; set; }
        public decimal? slm_FT { get; set; }
        public string slm_DeDuctible { get; set; }
        public decimal? slm_GrossPremium { get; set; }
        public decimal? slm_Stamp { get; set; }
        public decimal? slm_Vat { get; set; }
        public decimal? slm_NetGrossPremium { get; set; }
        public decimal? slm_Act { get; set; }
        public decimal? slm_InjuryDeath { get; set; }
        public decimal? slm_TPPD { get; set; }
        public decimal? slm_PersonalAccident { get; set; }
        public string slm_PersonalAccidentDriver { get; set; }
        public string slm_PersonalAccidentPassenger { get; set; }
        public decimal? slm_MedicalFee { get; set; }
        public string slm_MedicalFeeDriver { get; set; }
        public string slm_MedicalFeePassenger { get; set; }
        public decimal? slm_InsuranceDriver { get; set; }
        public string slm_Remark { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public string slm_UpdatedDate { get; set; }
        public DateTime? slm_UpdatedBy { get; set; }
        public string is_Deleted { get; set; }

        ///extend
        public string insname { get; set; }
        public string camname { get; set; }
        public string brandname { get; set; }
        public string modelname { get; set; }
        public string coveragetypename { get; set; }
        public string repairname { get; set; }
        public DateTime? EffectiveDateFrom { get; set; }
        public DateTime? EffectiveDateTo { get; set; }
    }
    //[Serializable]
    //public class PaymentMain
    //{
    //    public decimal slm_RenewPaymentMainId { get; set; }
    //    public decimal slm_RenewInsureId { get; set; }
    //    public string slm_Type { get; set; }
    //    public int? slm_Seq { get; set; }
    //    public DateTime? slm_PaymentDate { get; set; }
    //    public int? slm_Period { get; set; }
    //    public decimal? slm_PaymentAmount { get; set; }
    //    public string slm_CreatedBy { get; set; }
    //    public DateTime? slm_CreatedDate { get; set; }
    //    public string slm_UpdatedBy { get; set; }
    //    public DateTime? slm_UpdatedDate { get; set; }
    //}
    [Serializable]
    public class PaymentTransMainData
    {
        public decimal? slm_RenewPaymenttransMainId { get; set; }
        public decimal? slm_RenewInsureId { get; set; }
        public int? slm_PayOptionId { get; set; }
        public int? slm_PolicyAmountPeriod { get; set; }
        public string slm_PayBranchCode { get; set; }
        public int? slm_PolicyPayMethodId { get; set; }
        public int? slm_ActPayMethodId { get; set; }
        public int? slm_ActamountPeriod { get; set; }
        public int? slm_ActPayOptionId { get; set; }
        public string slm_ActPayBranchCode { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
    }
    [Serializable]
    public class PaymentTransData
    {
        public decimal? slm_RenewPaymenttransId { get; set; }
        public decimal? slm_RenewInsureId { get; set; }
        public string slm_Type { get; set; }
        public int? slm_Seq { get; set; }
        public DateTime? slm_PaymentDate { get; set; }
        public int? slm_Period { get; set; }
        public decimal? slm_PaymentAmount { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
    }


    [Serializable]
    public class RenewInsuranceReceiptData
    {
        public decimal? slm_RenewInsuranceReceiptId { get; set; }
        public string slm_ticketId { get; set; }
        public string slm_RecNo { get; set; }
        public string slm_Status { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public decimal? slm_UpdatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
        public bool is_Deleted { get; set; }

        public decimal? total { get; set; }
        public int countExport { get; set; }
    }

    [Serializable]
    public class RenewInsuranceReceiptDetailData
    {
        public decimal? slm_RenewInsuranceReceiptRevisionDetailId { get; set; }
        public decimal? slm_RenewInsuranceReceiptDetailId { get; set; }
        public decimal? slm_RenewInsuranceReceiptId { get; set; }
        public string slm_PaymentCode { get; set; }
        public string slm_PaymentOtherDesc { get; set; }
        public string slm_InsNoDesc { get; set; }
        public string slm_InstNo { get; set; }
        public string slm_RecBy { get; set; }
        public string slm_RecNo { get; set; }
        public decimal? slm_RecAmount { get; set; }
        public DateTime? slm_TransDate { get; set; }
        public string slm_Status { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public bool is_Deleted { get; set; }
        public string slm_PaymentDesc { get; set; }
        public string slm_Selected { get; set; }
        public int? slm_Seq { get; set; }
        public bool slm_Export_Flag { get; set; }
        public DateTime? slm_Export_Date { get; set; }

    }

    [Serializable]
    public class RenewInsuranceReceiptRevisionDetailData
    {
        public decimal? slm_RenewInsuranceReceiptRevisionDetailId { get; set; }
        public decimal? slm_RenewInsuranceReceiptId { get; set; }
        public string slm_PaymentCode { get; set; }
        public string slm_PaymentOtherDesc { get; set; }
        public string slm_InsNoDesc { get; set; }
        public string slm_InstNo { get; set; }
        public string slm_RecBy { get; set; }
        public string slm_RecNo { get; set; }
        public decimal? slm_RecAmount { get; set; }
        public DateTime? slm_TransDate { get; set; }
        public string slm_Status { get; set; }
        public string slm_Selected { get; set; }
        public int? slm_Seq { get; set; }
        public bool? slm_Export_Flag { get; set; }
        public DateTime? slm_Export_Date { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public bool? is_Deleted { get; set; }

        public string delflag { get; set; }
    }
    [Serializable]
    public class PhoneCallData
    {
        public int? slm_PhoneCallId { get; set; }
        public string slm_ContactDetail { get; set; }
        public string slm_ContactPhone { get; set; }
        public string slm_CreateBy { get; set; }
        public DateTime? slm_CreateDate { get; set; }
        public decimal is_Deleted { get; set; }
        public string slm_TicketId { get; set; }
        public string slm_Status { get; set; }
        public string slm_Owner { get; set; }
        public int? slm_Owner_Position { get; set; }
        public int? slm_CreatedBy_Position { get; set; }
        public bool? slm_CAS_Flag { get; set; }
        public string slm_SubStatus { get; set; }
        public string slm_SubStatusName { get; set; }
        public string slm_CreditFileName { get; set; }
        public string slm_CreditFilePath { get; set; }
        public int? slm_CreditFileSize { get; set; }
        public string slm_50TawiFileName { get; set; }
        public string slm_50TawiFilePath { get; set; }
        public int? slm_50TawiFileSize { get; set; }
        public string slm_DriverLicenseiFileName { get; set; }
        public string slm_DriverLicenseFilePath { get; set; }
        public int? slm_DriverLicenseFileSize { get; set; }
        public string slm_Need_CompulsoryFlag { get; set; }
        public DateTime? slm_Need_CompulsoryFlagDate { get; set; }
        public string slm_Need_PolicyFlag { get; set; }
        public DateTime? slm_Need_PolicyFlagDate { get; set; }


    }

    public class ConfigProductBlacklistData
    {
        public decimal? slm_cp_blacklist_id { get; set; }
        public string slm_Product_Id { get; set; }
        public decimal? slm_Prelead_Id { get; set; }
        public string slm_ticketId { get; set; }
        public string slm_Name { get; set; }
        public string slm_LastName { get; set; }
        public int? slm_CardType { get; set; }
        public string slm_CitizenId { get; set; }
        public DateTime? slm_StartDate { get; set; }
        public DateTime? slm_EndDate { get; set; }
        public bool slm_IsActive { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public bool? is_Deleted { get; set; }
    }

    public class ReceiveNoListData
    {

        public decimal? slm_ReceiveNoListId { get; set; }
        public decimal? slm_ReceiveNoId { get; set; }
        public string slm_Product_Id { get; set; }
        public decimal? slm_Ins_Com_Id { get; set; }
        public string slm_ReceiveNo { get; set; }
        public string slm_Status { get; set; }
        public string slm_TicketId { get; set; }
        public DateTime? slm_UseDate { get; set; }
        public string slm_UseBy { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        public bool? is_Deleted { get; set; }
    }

    public class tempData
    {

        public string slm_Product_Id { get; set; }

        public decimal? slm_InsuranceComId { get; set; }
        public decimal? slm_Ins_Com_Id { get; set; }
        public string slm_CampaignId { get; set; }
        public decimal? slm_CampaignInsuranceId { get; set; }
        public decimal? slm_PromotionId { get; set; }
        public int? slm_CoverageTypeId { get; set; }


        public string slm_ComissionFlag { get; set; }
        public decimal? slm_ComissionPercentValue { get; set; }
        public decimal? slm_ComissionBathValue { get; set; }
        public string slm_OV1Flag { get; set; }
        public decimal? slm_OV1PercentValue { get; set; }
        public decimal? slm_OV1BathValue { get; set; }
        public string slm_OV2Flag { get; set; }
        public decimal? slm_OV2PercentValue { get; set; }
        public decimal? slm_OV2BathValue { get; set; }
        public string slm_VatFlag { get; set; }
    }

    public class tabControlData
    {
        public string ID { get; set; }
        public string HeaderText { get; set; }
    }

    public class FlagData
    {
        public string slm_PayOptionIdOld { get; set; }
        public string slm_PolicyPayMethodIdOld { get; set; }
        public string slm_ActPayMethodIdOld { get; set; }
    }
}
