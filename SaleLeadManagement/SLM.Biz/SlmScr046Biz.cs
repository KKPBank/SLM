using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class SlmScr046Biz
    {
        string _error = "";
        public string ErrorMessage {  get { return _error; } }

        public PreleadDetails GetPreleadDetail(decimal preleadId)
        {
            PreleadDetails pd = null;
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var pr = sdc.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == preleadId).FirstOrDefault();
                if (pr != null)
                {
                    pd = new PreleadDetails();
                    pd.slm_Prelead_Id = pr.slm_Prelead_Id;
                    pd.slm_TempId = pr.slm_TempId;
                    pd.slm_Cmt_Product_Id = pr.slm_Cmt_Product_Id;
                    pd.slm_CmtLot = pr.slm_CmtLot;
                    pd.slm_Contract_Number = pr.slm_Contract_Number;
                    pd.slm_Product_Id = pr.slm_Product_Id;
                    pd.slm_CampaignId = pr.slm_CampaignId;
                    pd.slm_BranchCode = pr.slm_BranchCode;
                    pd.slm_Contract_Year = pr.slm_Contract_Year;
                    pd.slm_Contract_Status = pr.slm_Contract_Status;
                    pd.slm_Car_Category = pr.slm_Car_Category;
                    pd.slm_Customer_Key = pr.slm_Customer_Key;
                    pd.slm_TitleId = pr.slm_TitleId;
                    pd.slm_Title_Name_Org = pr.slm_Title_Name_Org;
                    pd.slm_Name = pr.slm_Name;
                    pd.slm_LastName = pr.slm_LastName;
                    pd.slm_CitizenId = pr.slm_CitizenId;
                    pd.slm_Marital_Status = pr.slm_Marital_Status;
                    pd.slm_CardTypeId = pr.slm_CardTypeId;
                    pd.slm_CardType_Org = pr.slm_CardType_Org;
                    pd.slm_Birthdate = pr.slm_Birthdate;
                    pd.slm_Tax_Id = pr.slm_Tax_Id;
                    pd.slm_OccupationId = pr.slm_OccupationId;
                    pd.slm_Career_Key_Org = pr.slm_Career_Key_Org;
                    pd.slm_Career_Desc_Org = pr.slm_Career_Desc_Org;
                    pd.slm_Car_By_Gov_Id = pr.slm_Car_By_Gov_Id;
                    pd.slm_Car_By_Gov_Name_Org = pr.slm_Car_By_Gov_Name_Org;
                    pd.slm_Brand_Code = pr.slm_Brand_Code;
                    pd.slm_Brand_Code_Org = pr.slm_Brand_Code_Org;
                    pd.slm_Brand_Name_Org = pr.slm_Brand_Name_Org;
                    pd.slm_Model_Code = pr.slm_Model_Code;
                    pd.slm_Model_Code_Org = pr.slm_Model_Code_Org;
                    pd.slm_Model_name_Org = pr.slm_Model_name_Org;
                    pd.slm_Engine_No = pr.slm_Engine_No;
                    pd.slm_Chassis_No = pr.slm_Chassis_No;
                    pd.slm_Model_Year = pr.slm_Model_Year;
                    pd.slm_Car_License_No = pr.slm_Car_License_No;
                    pd.slm_ProvinceRegis = pr.slm_ProvinceRegis;
                    pd.slm_ProvinceRegis_Org = pr.slm_ProvinceRegis_Org;
                    pd.slm_Cc = pr.slm_Cc;
                    pd.slm_Expire_Date = pr.slm_Expire_Date;
                    pd.slm_Voluntary_Mkt_Id = pr.slm_Voluntary_Mkt_Id;
                    pd.slm_Voluntary_Mkt_Id_Org = pr.slm_Voluntary_Mkt_Id_Org;
                    pd.slm_Voluntary_Mkt_TitleId = pr.slm_Voluntary_Mkt_TitleId;
                    pd.slm_Voluntary_Mkt_Title_Org = pr.slm_Voluntary_Mkt_Title_Org;
                    pd.slm_Voluntary_Mkt_First_Name = pr.slm_Voluntary_Mkt_First_Name;
                    pd.slm_Voluntary_Mkt_Last_Name = pr.slm_Voluntary_Mkt_Last_Name;
                    pd.slm_Voluntary_Policy_Number = pr.slm_Voluntary_Policy_Number;
                    pd.slm_Voluntary_Type_Key = pr.slm_Voluntary_Type_Key;
                    pd.slm_Voluntary_Type_Name = pr.slm_Voluntary_Type_Name;
                    pd.slm_Voluntary_Policy_Year = pr.slm_Voluntary_Policy_Year;
                    pd.slm_Voluntary_Policy_Eff_Date = pr.slm_Voluntary_Policy_Eff_Date;
                    pd.slm_Voluntary_Policy_Exp_Date = pr.slm_Voluntary_Policy_Exp_Date;
                    pd.slm_Voluntary_Policy_Exp_Year = pr.slm_Voluntary_Policy_Exp_Year;
                    pd.slm_Voluntary_Policy_Exp_Month = pr.slm_Voluntary_Policy_Exp_Month;
                    pd.slm_Voluntary_Cov_Amt = pr.slm_Voluntary_Cov_Amt;
                    pd.slm_Voluntary_Gross_Premium = pr.slm_Voluntary_Gross_Premium;
                    pd.slm_Voluntary_Channel_Key = pr.slm_Voluntary_Channel_Key;
                    pd.slm_Voluntary_Company_Code = pr.slm_Voluntary_Company_Code;
                    pd.slm_Voluntary_Company_Name = pr.slm_Voluntary_Company_Name;
                    pd.slm_Benefit_TitleId = pr.slm_Benefit_TitleId;
                    pd.slm_Benefit_Title_Name_Org = pr.slm_Benefit_Title_Name_Org;
                    pd.slm_Benefit_First_Name = pr.slm_Benefit_First_Name;
                    pd.slm_Benefit_Last_Name = pr.slm_Benefit_Last_Name;
                    pd.slm_Benefit_Telno = pr.slm_Benefit_Telno;
                    pd.slm_Driver_TitleId1 = pr.slm_Driver_TitleId1;
                    pd.slm_Driver_Title_Name1_Org = pr.slm_Driver_Title_Name1_Org;
                    pd.slm_Driver_First_Name1 = pr.slm_Driver_First_Name1;
                    pd.slm_Driver_Last_Name1 = pr.slm_Driver_Last_Name1;
                    pd.slm_Driver_Telno1 = pr.slm_Driver_Telno1;
                    pd.slm_Driver_Birthdate1 = pr.slm_Driver_Birthdate1;
                    pd.slm_Driver_TitleId2 = pr.slm_Driver_TitleId2;
                    pd.slm_Driver_Title_Name2_Org = pr.slm_Driver_Title_Name2_Org;
                    pd.slm_Driver_First_Name2 = pr.slm_Driver_First_Name2;
                    pd.slm_Driver_Last_Name2 = pr.slm_Driver_Last_Name2;
                    pd.slm_Driver_Telno2 = pr.slm_Driver_Telno2;
                    pd.slm_Driver_Birthdate2 = pr.slm_Driver_Birthdate2;
                    pd.slm_Compulsory_Policy_Number = pr.slm_Compulsory_Policy_Number;
                    pd.slm_Compulsory_Policy_Year = pr.slm_Compulsory_Policy_Year;
                    pd.slm_Compulsory_Policy_Eff_Date = pr.slm_Compulsory_Policy_Eff_Date;
                    pd.slm_Compulsory_Policy_Exp_Date = pr.slm_Compulsory_Policy_Exp_Date;
                    pd.slm_Compulsory_Cov_Amt = pr.slm_Compulsory_Cov_Amt;
                    pd.slm_Compulsory_Gross_Premium = pr.slm_Compulsory_Gross_Premium;
                    pd.slm_Compulsory_Company_Code = pr.slm_Compulsory_Company_Code;
                    pd.slm_Compulsory_Company_Name = pr.slm_Compulsory_Company_Name;
                    pd.slm_Guarantor_Code1 = pr.slm_Guarantor_Code1;
                    pd.slm_Guarantor_TitleId1 = pr.slm_Guarantor_TitleId1;
                    pd.slm_Guarantor_Title_Name1_Org = pr.slm_Guarantor_Title_Name1_Org;
                    pd.slm_Guarantor_First_Name1 = pr.slm_Guarantor_First_Name1;
                    pd.slm_Guarantor_Last_Name1 = pr.slm_Guarantor_Last_Name1;
                    pd.slm_Guarantor_Card_Id1 = pr.slm_Guarantor_Card_Id1;
                    pd.slm_Guarantor_RelationId1 = pr.slm_Guarantor_RelationId1;
                    pd.slm_Guarantor_Relation1_Org = pr.slm_Guarantor_Relation1_Org;
                    pd.slm_Guarantor_Telno1 = pr.slm_Guarantor_Telno1;
                    pd.slm_Guarantor_Code2 = pr.slm_Guarantor_Code2;
                    pd.slm_Guarantor_TitleId2 = pr.slm_Guarantor_TitleId2;
                    pd.slm_Guarantor_Title_Name2_Org = pr.slm_Guarantor_Title_Name2_Org;
                    pd.slm_Guarantor_First_Name2 = pr.slm_Guarantor_First_Name2;
                    pd.slm_Guarantor_Last_Name2 = pr.slm_Guarantor_Last_Name2;
                    pd.slm_Guarantor_Card_Id2 = pr.slm_Guarantor_Card_Id2;
                    pd.slm_Guarantor_RelationId2 = pr.slm_Guarantor_RelationId2;
                    pd.slm_Guarantor_Relation2_Org = pr.slm_Guarantor_Relation2_Org;
                    pd.slm_Guarantor_Telno2 = pr.slm_Guarantor_Telno2;
                    pd.slm_Guarantor_Code3 = pr.slm_Guarantor_Code3;
                    pd.slm_Guarantor_TitleId3 = pr.slm_Guarantor_TitleId3;
                    pd.slm_Guarantor_Title_Name3_Org = pr.slm_Guarantor_Title_Name3_Org;
                    pd.slm_Guarantor_First_Name3 = pr.slm_Guarantor_First_Name3;
                    pd.slm_Guarantor_Last_Name3 = pr.slm_Guarantor_Last_Name3;
                    pd.slm_Guarantor_Card_Id3 = pr.slm_Guarantor_Card_Id3;
                    pd.slm_Guarantor_RelationId3 = pr.slm_Guarantor_RelationId3;
                    pd.slm_Guarantor_Relation3_Org = pr.slm_Guarantor_Relation3_Org;
                    pd.slm_Guarantor_Telno3 = pr.slm_Guarantor_Telno3;
                    pd.slm_Grade = pr.slm_Grade;
                    pd.slm_View_UpdateDate = pr.slm_View_UpdateDate;
                    pd.slm_Email = pr.slm_Email;
                    pd.slm_sms = pr.slm_sms;
                    pd.slm_Owner = pr.slm_Owner;
                    pd.slm_OwnerBranch = pr.slm_OwnerBranch;
                    pd.slm_Owner_Position = pr.slm_Owner_Position;
                    pd.slm_Owner_Team = pr.slm_Owner_Team;
                    pd.slm_AssignDate = pr.slm_AssignDate;
                    pd.slm_AssignFlag = pr.slm_AssignFlag;
                    pd.slm_AssignType = pr.slm_AssignType;
                    pd.slm_AssignDescription = pr.slm_AssignDescription;
                    pd.slm_Assign_Status = pr.slm_Assign_Status;
                    pd.slm_Status = pr.slm_Status;
                    pd.slm_SubStatus = pr.slm_SubStatus;
                    pd.slm_Counting = pr.slm_Counting;
                    pd.slm_NextSLA = pr.slm_NextSLA;
                    pd.slm_TicketId = pr.slm_TicketId;
                    pd.slm_CreatedDate = pr.slm_CreatedDate;
                    pd.slm_CreatedBy = pr.slm_CreatedBy;
                    pd.slm_UpdatedDate = pr.slm_UpdatedDate;
                    pd.slm_UpdatedBy = pr.slm_UpdatedBy;
                    pd.is_Deleted = pr.is_Deleted;
                    pd.slm_NextContactDate = pr.slm_NextContactDate;
                    pd.slm_ObtPro03Flag = pr.slm_ObtPro03Flag;
                    pd.slm_AssignBy = pr.slm_AssignBy;
                    pd.slm_Current_Status = pr.slm_Current_Status;
                    pd.slm_Curent_Substatus = pr.slm_Curent_Substatus;
                    pd.slm_Voluntary_First_Create_Date = pr.slm_Voluntary_First_Create_Date;
                    pd.slm_Type = pr.slm_Type;
                    pd.slm_RemarkPayment = pr.slm_RemarkPayment;
                    pd.slm_ThisWork = pr.slm_ThisWork;
                    pd.slm_TotalAlert = pr.slm_TotalAlert;
                    pd.slm_TotalWork = pr.slm_TotalWork;
                    pd.slm_CurrentSLA = pr.slm_CurrentSLA;
                    pd.slm_SendDocFlag = pr.slm_SendDocFlag;
                    pd.slm_SendDocBrandCode = pr.slm_SendDocBrandCode;
                    pd.slm_Receiver = pr.slm_Receiver;

                    pd.prelead_address = sdc.kkslm_tr_prelead_address.Where(a => a.slm_Prelead_Id == preleadId).Select(a =>
                    new PreleadAddress()
                    {
                        slm_Prelead_Address_Id = a.slm_Prelead_Address_Id,
                        slm_CmtLot = a.slm_CmtLot,
                        slm_Prelead_Id = a.slm_Prelead_Id,
                        slm_Customer_Key = a.slm_Customer_Key,
                        slm_Address_Type = a.slm_Address_Type,
                        slm_Contract_Adrress = a.slm_Contract_Adrress,
                        slm_House_No = a.slm_House_No,
                        slm_Moo = a.slm_Moo,
                        slm_Building = a.slm_Building,
                        slm_Village = a.slm_Village,
                        slm_Soi = a.slm_Soi,
                        slm_Street = a.slm_Street,
                        slm_TambolId = a.slm_TambolId,
                        slm_Tambon_Name_Org = a.slm_Tambon_Name_Org,
                        slm_Amphur_Id = a.slm_Amphur_Id,
                        slm_Amphur_Name_Org = a.slm_Amphur_Name_Org,
                        slm_Province_Id = a.slm_Province_Id,
                        slm_Province_name_Org = a.slm_Province_name_Org,
                        slm_Zipcode = a.slm_Zipcode,
                        slm_Home_Phone = a.slm_Home_Phone,
                        slm_Mobile_Phone = a.slm_Mobile_Phone,
                        slm_CreatedDate = a.slm_CreatedDate,
                        slm_CreatedBy = a.slm_CreatedBy,
                        slm_UpdatedDate = a.slm_UpdatedDate,
                        slm_UpdatedBy = a.slm_UpdatedBy,
                        is_Deleted = a.is_Deleted,
                    }).ToList();
                }
            }
            return pd;
        }
        public bool SavePrelead(PreleadDetails pl, string userId)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    var plu = sdc.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == pl.slm_Prelead_Id).FirstOrDefault();
                    if (plu == null) throw new Exception("Invalid Prelead Id");

                    plu.slm_Contract_Number = pl.slm_Contract_Number;
                    plu.slm_BranchCode = pl.slm_BranchCode;
                    plu.slm_Contract_Year = pl.slm_Contract_Year;
                    plu.slm_Contract_Status = pl.slm_Contract_Status;
                    plu.slm_Car_Category = pl.slm_Car_Category;
                    plu.slm_Customer_Key = pl.slm_Customer_Key;
                    plu.slm_CardTypeId = pl.slm_CardTypeId;
                    plu.slm_CitizenId = pl.slm_CitizenId;
                    plu.slm_Marital_Status = pl.slm_Marital_Status;
                    plu.slm_TitleId = pl.slm_TitleId;
                    plu.slm_Name = pl.slm_Name;
                    plu.slm_LastName = pl.slm_LastName;
                    plu.slm_OccupationId = pl.slm_OccupationId;
                    plu.slm_Grade = pl.slm_Grade;
                    plu.slm_Birthdate = pl.slm_Birthdate;
                    plu.slm_Car_By_Gov_Id = pl.slm_Car_By_Gov_Id;
                    plu.slm_Brand_Code = pl.slm_Brand_Code;
                    plu.slm_Model_Code = pl.slm_Model_Code;
                    plu.slm_Model_Year = pl.slm_Model_Year;
                    plu.slm_Engine_No = pl.slm_Engine_No;
                    plu.slm_Chassis_No = pl.slm_Chassis_No;
                    plu.slm_Car_License_No = pl.slm_Car_License_No;
                    plu.slm_ProvinceRegis = pl.slm_ProvinceRegis;
                    plu.slm_Cc = pl.slm_Cc;
                    plu.slm_Expire_Date = pl.slm_Expire_Date;
                    plu.slm_Voluntary_Mkt_Id = pl.slm_Voluntary_Mkt_Id;
                    plu.slm_Voluntary_Mkt_TitleId = pl.slm_Voluntary_Mkt_TitleId;
                    plu.slm_Voluntary_Mkt_First_Name = pl.slm_Voluntary_Mkt_First_Name;
                    plu.slm_Voluntary_Mkt_Last_Name = pl.slm_Voluntary_Mkt_Last_Name;
                    plu.slm_Voluntary_Company_Code = pl.slm_Voluntary_Company_Code;
                    plu.slm_Voluntary_Policy_Number = pl.slm_Voluntary_Policy_Number;
                    plu.slm_Voluntary_Type_Key = pl.slm_Voluntary_Type_Key;
                    plu.slm_Voluntary_Cov_Amt = pl.slm_Voluntary_Cov_Amt;
                    plu.slm_Voluntary_Policy_Eff_Date = pl.slm_Voluntary_Policy_Eff_Date;
                    plu.slm_Voluntary_Policy_Exp_Date = pl.slm_Voluntary_Policy_Exp_Date;
                    plu.slm_Voluntary_Policy_Year = pl.slm_Voluntary_Policy_Year;
                    plu.slm_Voluntary_Gross_Premium = pl.slm_Voluntary_Gross_Premium;
                    plu.slm_Voluntary_Policy_Exp_Year = pl.slm_Voluntary_Policy_Exp_Year;
                    plu.slm_Voluntary_Policy_Exp_Month = pl.slm_Voluntary_Policy_Exp_Month;
                    plu.slm_Voluntary_Channel_Key = pl.slm_Voluntary_Channel_Key;
                    plu.slm_Benefit_TitleId = pl.slm_Benefit_TitleId;
                    plu.slm_Benefit_First_Name = pl.slm_Benefit_First_Name;
                    plu.slm_Benefit_Last_Name = pl.slm_Benefit_Last_Name;
                    plu.slm_Benefit_Telno = pl.slm_Benefit_Telno;
                    plu.slm_Driver_TitleId1 = pl.slm_Driver_TitleId1;
                    plu.slm_Driver_First_Name1 = pl.slm_Driver_First_Name1;
                    plu.slm_Driver_Last_Name1 = pl.slm_Driver_Last_Name1;
                    plu.slm_Driver_Birthdate1 = pl.slm_Driver_Birthdate1;
                    plu.slm_Driver_Telno1 = pl.slm_Driver_Telno1;
                    plu.slm_Driver_TitleId2 = pl.slm_Driver_TitleId2;
                    plu.slm_Driver_First_Name2 = pl.slm_Driver_First_Name2;
                    plu.slm_Driver_Last_Name2 = pl.slm_Driver_Last_Name2;
                    plu.slm_Driver_Telno2 = pl.slm_Driver_Telno2;
                    plu.slm_Driver_Birthdate2 = pl.slm_Driver_Birthdate2;
                    plu.slm_Compulsory_Company_Code = pl.slm_Compulsory_Company_Code;
                    plu.slm_Compulsory_Policy_Number = pl.slm_Compulsory_Policy_Number;
                    plu.slm_Compulsory_Policy_Year = pl.slm_Compulsory_Policy_Year;
                    plu.slm_Compulsory_Policy_Eff_Date = pl.slm_Compulsory_Policy_Eff_Date;
                    plu.slm_Compulsory_Policy_Exp_Date = pl.slm_Compulsory_Policy_Exp_Date;
                    plu.slm_Compulsory_Gross_Premium = pl.slm_Compulsory_Gross_Premium;
                    plu.slm_Compulsory_Cov_Amt = pl.slm_Compulsory_Cov_Amt;
                    plu.slm_Guarantor_Code1 = pl.slm_Guarantor_Code1;
                    plu.slm_Guarantor_TitleId1 = pl.slm_Guarantor_TitleId1;
                    plu.slm_Guarantor_First_Name1 = pl.slm_Guarantor_First_Name1;
                    plu.slm_Guarantor_Last_Name1 = pl.slm_Guarantor_Last_Name1;
                    plu.slm_Guarantor_Card_Id1 = pl.slm_Guarantor_Card_Id1;
                    plu.slm_Guarantor_RelationId1 = pl.slm_Guarantor_RelationId1;
                    plu.slm_Guarantor_Telno1 = pl.slm_Guarantor_Telno1;
                    plu.slm_Guarantor_Code2 = pl.slm_Guarantor_Code2;
                    plu.slm_Guarantor_TitleId2 = pl.slm_Guarantor_TitleId2;
                    plu.slm_Guarantor_First_Name2 = pl.slm_Guarantor_First_Name2;
                    plu.slm_Guarantor_Last_Name2 = pl.slm_Guarantor_Last_Name2;
                    plu.slm_Guarantor_Card_Id2 = pl.slm_Guarantor_Card_Id2;
                    plu.slm_Guarantor_RelationId2 = pl.slm_Guarantor_RelationId2;
                    plu.slm_Guarantor_Telno2 = pl.slm_Guarantor_Telno2;
                    plu.slm_Guarantor_Code3 = pl.slm_Guarantor_Code3;
                    plu.slm_Guarantor_TitleId3 = pl.slm_Guarantor_TitleId3;
                    plu.slm_Guarantor_First_Name3 = pl.slm_Guarantor_First_Name3;
                    plu.slm_Guarantor_Last_Name3 = pl.slm_Guarantor_Last_Name3;
                    plu.slm_Guarantor_Card_Id3 = pl.slm_Guarantor_Card_Id3;
                    plu.slm_Guarantor_RelationId3 = pl.slm_Guarantor_RelationId3;
                    plu.slm_Guarantor_Telno3 = pl.slm_Guarantor_Telno3;

                    sdc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _error = ex.InnerException != null ? ex.Message : ex.InnerException.Message;
                ret = false;
            }
            return ret;
        }


        public partial class PreleadDetails : kkslm_tr_prelead
        {
            public List<PreleadAddress> prelead_address { get; set; }
        }
        public partial class PreleadAddress : kkslm_tr_prelead_address
        {

        }

        // combo source
        public static List<ControlListData> GetMaritalDataList()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_marital_status.Where(m => m.is_Deleted == false).ToList().Select(m => new ControlListData() { TextField = m.slm_MaritalStatusName, ValueField = m.slm_MaritalStatusName /* use only in prelead m.slm_MaritalStatusId.ToString() edited 2016-07-14 */ }).ToList();
            }
        }
        public static List<ControlListData> GetTitleDataList()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_title.Where(t => t.is_Deleted == false).OrderBy(p => p.slm_TitleName).ToList().Select(t => new ControlListData() { TextField = t.slm_TitleName, ValueField = t.slm_TitleId.ToString() }).ToList();
            }
        }
        public static List<ControlListData> GetInsuranceTypeData()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_insurancecartype.Where(i => i.is_Deleted == false).ToList().Select(i => new ControlListData() { TextField = i.slm_InsurancecarTypeName, ValueField = i.slm_InsurancecarTypeId.ToString() }).ToList();
            }
        }
        public static List<ControlListData> GetRelationData()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_relate.Where(r => r.is_Deleted == false).ToList().Select(r => new ControlListData() { TextField = r.slm_RelateDesc, ValueField = r.slm_RelateId.ToString() }).ToList();
            }
        }
        public static List<ControlListData> GetProvinceDataId()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_province.OrderBy(p => p.slm_ProvinceNameTH).ToList().Select(p => new ControlListData { TextField = p.slm_ProvinceNameTH, ValueField = p.slm_ProvinceId.ToString() }).ToList();
            }
        }
        public static List<ControlListData> GetCoverageType()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_coveragetype.Where(c => c.is_Deleted == false).OrderBy(c => c.slm_ConverageTypeName).ToList().Select(c => new ControlListData() { TextField = c.slm_ConverageTypeName, ValueField = c.slm_CoverageTypeId.ToString() }).ToList();
            }
        }

    }
}
