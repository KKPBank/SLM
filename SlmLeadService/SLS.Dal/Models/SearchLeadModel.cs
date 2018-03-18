using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SLS.Resource.Data;
using SLS.Resource;

namespace SLS.Dal.Models
{
    public class SearchLeadModel
    {
        private SLM_DBEntities slmdb = null;

        public SearchLeadModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public string GetDBName()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["SLMDBName"];
            }
            catch
            {
                return "SLMDB";
            }
        }

        public SearchLeadData SearchLeadData(string ticketId)
        {
            try
            {
                string SLMDBName = GetDBName();

                string sql = @"SELECT LEAD.slm_ticketId AS TicketId, LEAD.slm_Name AS Firstname, LEAD.slm_TelNo_1 AS TelNo1, LEAD.slm_CampaignId AS Campaign, CAM.slm_CampaignName AS CampaignName, LEAD.slm_Ext_1 AS ExtNo1, LEAD.slm_AvailableTime AS AvailableTime, LEAD.slm_Owner AS TelesaleName, LEAD.slm_Status AS Status
                            , LEAD.slm_Product_Group_Id AS ProductGroupId, PG.product_name AS ProductGroupName, LEAD.slm_Product_Id AS ProductId, LEAD.slm_Product_Name AS ProductName, lead.slm_ContractNoRefer AS ContractNoRefer
                            , CUS.slm_LastName AS Lastname, CUS.slm_Email AS Email, CUS.slm_TelNo_2 AS TelNo2, CUS.slm_TelNo_3 AS TelNo3, CUS.slm_Ext_2 AS ExtNo2
                            , CUS.slm_Ext_3 AS ExtNo3, CUS.slm_BuildingName AS BuildingName, CUS.slm_AddressNo AS AddrNo, CUS.slm_Floor AS Floor, CUS.slm_Soi AS Soi
                            , CUS.slm_Street AS Street, CUS.slm_Tambon AS Tambol, TAM.slm_TambolCode AS TambolCode, CUS.slm_Amphur AS Amphur, AM.slm_AmphurCode AS AmphurCode, CUS.slm_Province AS Province, PRO.slm_ProvinceCode AS ProvinceCode, CUS.slm_PostalCode AS PostalCode
                            , CUS.slm_Occupation AS Occupation, OCC.slm_OccupationCode AS OccupationCode, CUS.slm_BaseSalary AS BaseSalary, CUS.slm_IsCustomer AS IsCustomer, CUS.slm_CusCode AS CustomerCode
                            , CUS.slm_Birthdate AS DateOfBirth, CUS.slm_CitizenId AS Cid, CUS.slm_Topic AS Topic, CUS.slm_Detail AS Detail, CUS.slm_PathLink AS PathLink
                            , CUS.slm_ContactBranch AS ContactBranch, cardtype.slm_CardTypeName AS CardTypeDesc
                            , PROD.slm_InterestedProd AS InterestedProdAndType, PROD.slm_LicenseNo AS LicenseNo, PROD.slm_YearOfCar AS YearOfCar, PROD.slm_YearOfCarRegis AS YearOfCarRegis
                            , PROD.slm_ProvinceRegis AS RegisterProvince, PRO2.slm_ProvinceCode AS RegisterProvinceCode, PROD.slm_Brand AS Brand, PROD.slm_Model AS Model, PROD.slm_Submodel AS Submodel, PROD.slm_DownPayment AS DownPayment
                            , PROD.slm_DownPercent AS DownPercent, PROD.slm_CarPrice AS CarPrice, PROD.slm_CarType AS CarType, PROD.slm_FinanceAmt AS FinanceAmt, PROD.slm_PaymentTerm AS Term
                            , PROD.slm_PaymentType AS PaymentType, PROD.slm_BalloonAmt AS BalloonAmt, PROD.slm_BalloonPercent AS BalloonPercent, PROD.slm_PlanType AS PlanType
                            , PROD.slm_CoverageDate AS CoverageDate, PROD.slm_AccType AS AccType, PROD.slm_AccPromotion AS AccPromotion, PROD.slm_AccTerm AS AccTerm, PROD.slm_Interest AS Interest
                            , PROD.slm_Invest AS Invest, PROD.slm_LoanOd AS LoanOd, PROD.slm_LoanOdTerm AS LoanOdTerm, PROD.slm_Ebank AS SlmBank, PROD.slm_Atm AS SlmAtm
                            , PROD.slm_OtherDetail_1 AS OtherDetail1, PROD.slm_OtherDetail_2 AS OtherDetail2, PROD.slm_OtherDetail_3 AS OtherDetail3, PROD.slm_OtherDetail_4 AS OtherDetail4
                            , CHAN.slm_ChannelId AS ChannelId, CHAN.slm_RequestDate AS RequestDate, CHAN.slm_RequestBy AS CreateUser, CHAN.slm_IPAddress AS Ipaddress, CHAN.slm_Company AS Company
                            , CHAN.slm_Branch AS Branch, CHAN.slm_BranchNo AS BranchNo, CHAN.slm_MachineNo AS MachineNo, CHAN.slm_ClientServiceType AS ClientServiceType
                            , CHAN.slm_DocumentNo AS DocumentNo, CHAN.slm_CommPaidCode AS CommPaidCode, CHAN.slm_Zone AS Zone, CHAN.slm_TransId AS TransId
                            , PROD.slm_RedbookBrandCode AS BrandCode, PROD.slm_RedbookModelCode AS ModelFamily, PROD.slm_RedbookKKKey AS SubmodelRedBookNo, PAYTYPE.slm_PaymentCode AS PaymentCode
                            , CONVERT(VARCHAR, ACCTYPE.slm_ModuleCode) AS AccTypeCode, PROMOTE.slm_PromotionCode AS AccPromotionCode, OPT.slm_OptionDesc AS StatusDesc
                            , CUS.slm_country_id CountryId, cty.slm_CountryCode CountryCode, cty.slm_CountryDescriptionEN CountryDescriptionEN, cty.slm_CountryDescriptionTH CountryDescriptionTH
                            FROM " + SLMDBName + @".dbo.kkslm_tr_lead LEAD
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_cusinfo CUS ON LEAD.slm_ticketId = CUS.slm_TicketId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_productinfo PROD ON LEAD.slm_ticketId = PROD.slm_TicketId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_channelinfo CHAN ON LEAD.slm_ticketId = CHAN.slm_TicketId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_tambol TAM ON CUS.slm_Tambon = TAM.slm_TambolId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_amphur AM ON CUS.slm_Amphur = AM.slm_AmphurId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_province PRO ON CUS.slm_Province = PRO.slm_ProvinceId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_occupation OCC ON CUS.slm_Occupation = OCC.slm_OccupationId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_province PRO2 ON PROD.slm_ProvinceRegis = PRO2.slm_ProvinceId 
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_paymenttype PAYTYPE ON PROD.slm_PaymentType = PAYTYPE.slm_PaymentId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_module ACCTYPE ON PROD.slm_AccType = ACCTYPE.slm_ModuleId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_promotion PROMOTE ON PROD.slm_AccPromotion = PROMOTE.slm_PromotionId
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_option OPT ON OPT.slm_OptionCode = LEAD.slm_Status AND OPT.slm_OptionType = 'lead status'
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_cardtype cardtype ON CUS.slm_CardType = cardtype.slm_CardTypeId 
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_campaign CAM ON CAM.slm_CampaignId = LEAD.slm_CampaignId
                            LEFT JOIN " + SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP PG ON PG.product_id = LEAD.slm_Product_Group_Id
                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_country cty ON cty.slm_country_id=CUS.slm_country_id
                            WHERE LEAD.slm_ticketId = @ticketId ";

                object[] param = new object[] 
                { 
                    new SqlParameter("@ticketId", ticketId)
                };

                return slmdb.ExecuteStoreQuery<SearchLeadData>(sql, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ServiceException(ApplicationResource.SEARCH_SEARCH_FAIL_CODE, ApplicationResource.SEARCH_SEARCH_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        #region Backup

//        public SearchLeadData SearchLeadData(string ticketId)
//        {
//            try
//            {
//                string SLMDBName = GetDBName();

//                string sql = @"SELECT LEAD.slm_ticketId AS TicketId, LEAD.slm_Name AS Firstname, LEAD.slm_TelNo_1 AS TelNo1, LEAD.slm_CampaignId AS Campaign, LEAD.slm_Ext_1 AS ExtNo1, LEAD.slm_AvailableTime AS AvailableTime, LEAD.slm_Owner AS TelesaleName, LEAD.slm_Status AS Status
//                            , LEAD.slm_Product_Group_Id AS ProductGroupId, LEAD.slm_Product_Id AS ProductId, lead.slm_ContractNoRefer AS ContractNoRefer
//                            , CUS.slm_LastName AS Lastname, CUS.slm_Email AS Email, CUS.slm_TelNo_2 AS TelNo2, CUS.slm_TelNo_3 AS TelNo3, CUS.slm_Ext_2 AS ExtNo2
//                            , CUS.slm_Ext_3 AS ExtNo3, CUS.slm_BuildingName AS BuildingName, CUS.slm_AddressNo AS AddrNo, CUS.slm_Floor AS Floor, CUS.slm_Soi AS Soi
//                            , CUS.slm_Street AS Street, CUS.slm_Tambon AS Tambol, TAM.slm_TambolCode AS TambolCode, CUS.slm_Amphur AS Amphur, AM.slm_AmphurCode AS AmphurCode, CUS.slm_Province AS Province, PRO.slm_ProvinceCode AS ProvinceCode, CUS.slm_PostalCode AS PostalCode
//                            , CUS.slm_Occupation AS Occupation, OCC.slm_OccupationCode AS OccupationCode, CUS.slm_BaseSalary AS BaseSalary, CUS.slm_IsCustomer AS IsCustomer, CUS.slm_CusCode AS CustomerCode
//                            , CUS.slm_Birthdate AS DateOfBirth, CUS.slm_CitizenId AS Cid, CUS.slm_Topic AS Topic, CUS.slm_Detail AS Detail, CUS.slm_PathLink AS PathLink
//                            , CUS.slm_ContactBranch AS ContactBranch, cardtype.slm_CardTypeName AS CardTypeDesc
//                            , PROD.slm_InterestedProd AS InterestedProdAndType, PROD.slm_LicenseNo AS LicenseNo, PROD.slm_YearOfCar AS YearOfCar, PROD.slm_YearOfCarRegis AS YearOfCarRegis
//                            , PROD.slm_ProvinceRegis AS RegisterProvince, PRO2.slm_ProvinceCode AS RegisterProvinceCode, PROD.slm_Brand AS Brand, PROD.slm_Model AS Model, PROD.slm_Submodel AS Submodel, PROD.slm_DownPayment AS DownPayment
//                            , PROD.slm_DownPercent AS DownPercent, PROD.slm_CarPrice AS CarPrice, PROD.slm_CarType AS CarType, PROD.slm_FinanceAmt AS FinanceAmt, PROD.slm_PaymentTerm AS Term
//                            , PROD.slm_PaymentType AS PaymentType, PROD.slm_BalloonAmt AS BalloonAmt, PROD.slm_BalloonPercent AS BalloonPercent, PROD.slm_PlanType AS PlanType
//                            , PROD.slm_CoverageDate AS CoverageDate, PROD.slm_AccType AS AccType, PROD.slm_AccPromotion AS AccPromotion, PROD.slm_AccTerm AS AccTerm, PROD.slm_Interest AS Interest
//                            , PROD.slm_Invest AS Invest, PROD.slm_LoanOd AS LoanOd, PROD.slm_LoanOdTerm AS LoanOdTerm, PROD.slm_Ebank AS SlmBank, PROD.slm_Atm AS SlmAtm
//                            , PROD.slm_OtherDetail_1 AS OtherDetail1, PROD.slm_OtherDetail_2 AS OtherDetail2, PROD.slm_OtherDetail_3 AS OtherDetail3, PROD.slm_OtherDetail_4 AS OtherDetail4
//                            , CHAN.slm_ChannelId AS ChannelId, CHAN.slm_RequestDate AS RequestDate, CHAN.slm_RequestBy AS CreateUser, CHAN.slm_IPAddress AS Ipaddress, CHAN.slm_Company AS Company
//                            , CHAN.slm_Branch AS Branch, CHAN.slm_BranchNo AS BranchNo, CHAN.slm_MachineNo AS MachineNo, CHAN.slm_ClientServiceType AS ClientServiceType
//                            , CHAN.slm_DocumentNo AS DocumentNo, CHAN.slm_CommPaidCode AS CommPaidCode, CHAN.slm_Zone AS Zone, CHAN.slm_TransId AS TransId
//                            , BRAND.slm_BrandCode AS BrandCode, MODEL.slm_Family AS ModelFamily, CONVERT(VARCHAR, SUBMODEL.slm_RedBookNo) AS SubmodelRedBookNo, PAYTYPE.slm_PaymentCode AS PaymentCode
//                            , CONVERT(VARCHAR, ACCTYPE.slm_ModuleCode) AS AccTypeCode, PROMOTE.slm_PromotionCode AS AccPromotionCode, OPT.slm_OptionDesc AS StatusDesc
//                            FROM " + SLMDBName + @".dbo.kkslm_tr_lead LEAD
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_cusinfo CUS ON LEAD.slm_ticketId = CUS.slm_TicketId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_productinfo PROD ON LEAD.slm_ticketId = PROD.slm_TicketId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_tr_channelinfo CHAN ON LEAD.slm_ticketId = CHAN.slm_TicketId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_tambol TAM ON CUS.slm_Tambon = TAM.slm_TambolId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_amphur AM ON CUS.slm_Amphur = AM.slm_AmphurId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_province PRO ON CUS.slm_Province = PRO.slm_ProvinceId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_occupation OCC ON CUS.slm_Occupation = OCC.slm_OccupationId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_province PRO2 ON PROD.slm_ProvinceRegis = PRO2.slm_ProvinceId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_brand BRAND ON PROD.slm_Brand = BRAND.slm_BrandId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_model MODEL ON PROD.slm_Model = MODEL.slm_ModelId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_submodel SUBMODEL ON PROD.slm_Submodel = SUBMODEL.slm_SubModelId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_paymenttype PAYTYPE ON PROD.slm_PaymentType = PAYTYPE.slm_PaymentId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_module ACCTYPE ON PROD.slm_AccType = ACCTYPE.slm_ModuleId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_promotion PROMOTE ON PROD.slm_AccPromotion = PROMOTE.slm_PromotionId
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_option OPT ON OPT.slm_OptionCode = LEAD.slm_Status AND OPT.slm_OptionType = 'lead status'
//                            LEFT JOIN " + SLMDBName + @".dbo.kkslm_ms_cardtype cardtype ON CUS.slm_CardType = cardtype.slm_CardTypeId
//                            WHERE LEAD.slm_ticketId = @ticketId ";

//                object[] param = new object[] 
//                { 
//                    new SqlParameter("@ticketId", ticketId)
//                };

//                return slmdb.ExecuteStoreQuery<SearchLeadData>(sql, param).FirstOrDefault();
//            }
//            catch (Exception ex)
//            {
//                throw new ServiceException(ApplicationResource.SEARCH_SEARCH_FAIL_CODE, ApplicationResource.SEARCH_SEARCH_FAIL_DESC, ex.Message, ex.InnerException);
//            }
//        }

        #endregion
    }
}
