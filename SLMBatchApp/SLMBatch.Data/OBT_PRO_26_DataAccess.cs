using System.Data;
using System.Linq;

namespace SLMBatch.Data
{
    public class OBT_PRO_26_DataAccess : SQLDataAccess
    {
        public override DataTable LoadData()
        {
            return LoadPaymentPendingData();
        }

        private DataTable LoadPaymentPendingData()
        {
            return GetPaymentPendingType();
        }

        private DataTable GetPaymentPendingType()
        //DuplicateTicketId = 1,          //มี Ticket มากกว่า 1 รายการ
        //ContractNoInPreleadOnly = 2,    //มีเลขที่สัญญาใน Prelead แต่ไม่มีใน Lead
        //InappropriateLeadStatus = 3,    //มี Ticket แต่สถานะเป็น Cancel, Reject, Success
        //PurchaseDetailNotFound = 4,     //มี Ticket แต่ไม่มี Flag การซื้อประกันและพรบ.
        //ContractNoNotFound = 5,         //เลขที่สัญญาไม่มีใน Prelead และ Lead
        //ConfigPaymentNotFound = 6,      //ไม่มี Config ใน Table kkslm_ms_config_product_payment
        {
            string sql = @"SELECT A.* FROM (
                            SELECT CONVERT(VARCHAR(10), GETDATE(),103) + ' ' +
                               CONVERT(VARCHAR(5), GETDATE(), 108)                       AS Created
                               ,CASE WHEN PEN.slm_Type = 1 THEN 'มี Ticket มากกว่า 1 รายการ' 
							         WHEN PEN.slm_Type = 3 AND LEAD.slm_Status = '08' THEN 'มี Ticket แต่สถานะเป็น Reject'
									 WHEN PEN.slm_Type = 3 AND LEAD.slm_Status = '09' THEN 'มี Ticket แต่สถานะเป็น Cancel'
									 WHEN PEN.slm_Type = 3 AND LEAD.slm_Status = '10' THEN 'มี Ticket แต่สถานะเป็น Success'
								     WHEN PEN.SLM_TYPE = 4 THEN 'มี Ticket แต่ไม่มี Flag การซื้อประกันและพรบ'
								     WHEN PEN.SLM_TYPE = 6 THEN 'ไม่มีการ Config การรับเงินเข้าระบบ' ELSE '' END           [Update]
			                  ,CONVERT(VARCHAR(10), PEN.slm_Tran_Date, 103)                AS UpdatedDate
			                  ,HSTAFF.slm_StaffNameTH AS HeadMarketingName
			                  ,REN.slm_TicketId AS TicketId
			                  ,REN.slm_ContractNo AS ContractNo
			                  ,LEAD_TITLE.slm_TitleName AS TitleName
			                  ,LEAD.slm_Name AS CustomerName
			                  ,LEAD.slm_LastName AS CustomerLastName
			                  ,CASE WHEN PEN.slm_Tran_Code = '204' THEN 'ประกัน'
			                        WHEN PEN.slm_Tran_Code = '205' THEN 'พ.ร.บ.' ELSE '' END AS PayType
			                  ,PEN.slm_Inst_No AS InstNo
			                  ,PEN.slm_Rec_by AS RecBy
			                  ,PEN.slm_Rec_No AS RecNo
			                  ,PEN.slm_Rec_Amount AS RecAmount
			                  ,PRE.slm_Voluntary_Company_Name AS CompanyOld
			                  ,PRE.slm_Voluntary_Policy_Number AS PolicyNoOld
			                  ,PRE.slm_BranchCode AS BranchOld
			                  ,BRANCH.slm_BranchName AS BranchOldName
			                  ,ISNULL(PRE.slm_Car_License_No, '') + ISNULL(' ' + PROVINCE.slm_ProvinceNameTH, '') AS LicenseNoProvince
                              ,REN.slm_ReceiveNo AS ReceiveNo
			                  ,STAFF.slm_StaffNameTH AS MarketingName
			                  ,BRAND.slm_BrandName AS BrandName
			                  ,REN.slm_ChassisNo AS ChassisNo
			                  ,REN.slm_EngineNo AS EngineNo
			                  ,PRO.slm_RedbookYearGroup AS YearGroup
			                  ,MODEL.slm_ModelName AS ModelName
			                  ,SUBMODEL.slm_Description AS SubModelDesc
			                  ,CarType.slm_InsurancecarTypeName AS CarTypeName
			                  ,REN.slm_PolicyNetGrossPremium AS PolicyNetGrossPremium
			                  ,REN.slm_PolicyGrossPremiumTotal AS PolicyGrossPremiumTotal
			                  , CONVERT(VARCHAR(10), REN.slm_ActStartCoverDate, 103)    AS ActStartCoverDate
                              , CONVERT(VARCHAR(10), REN.slm_ActEndCoverDate, 103)		AS ActEndCoverDate
                              , ADDR.slm_AddressNo AS AddressNo
			                  ,ADDR.slm_Moo AS Moo
			                  ,ADDR.slm_Village AS Village
			                  ,ADDR.slm_BuildingName AS BuildingName
			                  ,ADDR.slm_Soi AS SOI
			                  ,ADDR.slm_Street AS Street
			                  ,Tambol.slm_TambolNameTH AS TambonName
			                  ,Amphur.slm_AmphurNameTH AS AmphurName
			                  ,ProvinceAddr.slm_ProvinceNameTH AS ProvinceAddrName
			                  ,addr.slm_PostalCode AS PostCode
			                  , CASE WHEN ADDR.slm_AddressNo IS NOT NULL AND ADDR.slm_AddressNo <> '0'  THEN ADDR.slm_AddressNo ELSE '' END +
                            CASE WHEN ADDR.slm_Moo IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_Moo)) <> ''   THEN ' หมู่ ' + ADDR.slm_Moo                        ELSE '' END +
                            CASE WHEN ADDR.slm_BuildingName IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_BuildingName)) <> ''   THEN ' อาคาร ' + ADDR.slm_BuildingName          ELSE '' END +
                            CASE WHEN ADDR.slm_Village      IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_Village)) <> ''   THEN ' หมู่บ้าน ' + ADDR.slm_Village                ELSE '' END +
                            CASE WHEN ADDR.slm_Soi          IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_Soi)) <> ''   THEN ' ซอย ' + ADDR.slm_Soi                     ELSE '' END +
                            CASE WHEN ADDR.slm_Street       IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_Street)) <> ''   THEN ' ถนน ' + ADDR.slm_Street                  ELSE '' END +
                            CASE WHEN Tambol.slm_TambolId   IS NOT NULL  THEN ' ตำบล' + Tambol.slm_TambolNameTH  ELSE '' END +
                            CASE WHEN Amphur.slm_AmphurId   IS NOT NULL  THEN ' อำเภอ' + Amphur.slm_AmphurNameTH  ELSE '' END +
                            CASE WHEN ProvinceAddr.slm_ProvinceId IS NOT NULL   THEN ' จังหวัด' + ProvinceAddr.slm_ProvinceNameTH ELSE '' END + addr.slm_PostalCode AS AddressDescription
                            , LEAD.slm_TelNo_1                                         AS TelContact
                            , CUS.slm_CitizenId                                        AS CitizenId
                            , CONVERT(VARCHAR(10), CUS.slm_Birthdate, 103)             AS Birthdate
                            , OCC.slm_OccupationNameTH                                 AS OccupationName
                            , ms.slm_MaritalStatusName                                 AS MaritalStatusName
                            , CUS.slm_CitizenId                                        AS VatNo
                            , pre.slm_Guarantor_Title_Name1_Org                        AS Guarantor_Title_Name1_Org
                            , PRE.SLM_GUARANTOR_FIRST_NAME1                            AS GUARANTOR_FIRST_NAME1
                            , PRE.SLM_GUARANTOR_LAST_NAME1                             AS GUARANTOR_LAST_NAME1
                            , PRE.slm_Guarantor_Card_Id1                               AS Guarantor_Card_Id1
                            , pre.slm_Guarantor_Title_Name2_Org                        AS Guarantor_Title_Name2_Org
                            , PRE.SLM_GUARANTOR_FIRST_NAME2                            AS GUARANTOR_FIRST_NAME2
                            , PRE.SLM_GUARANTOR_LAST_NAME2                             AS GUARANTOR_LAST_NAME2
                            , PRE.slm_Guarantor_Card_Id2                               AS Guarantor_Card_Id2
                            , pre.slm_Guarantor_Title_Name3_Org                        AS Guarantor_Title_Name3_Org
                            , PRE.SLM_GUARANTOR_FIRST_NAME3                            AS GUARANTOR_FIRST_NAME3
                            , PRE.SLM_GUARANTOR_LAST_NAME3                             AS GUARANTOR_LAST_NAME3
                            , PRE.slm_Guarantor_Card_Id3                               AS Guarantor_Card_Id3
                        FROM kkslm_tr_renewinsurance_hp_payment_pending     AS PEN
                        INNER JOIN kkslm_tr_renewinsurance                  AS REN          ON      PEN.slm_Contract_Number = REN.slm_ContractNo
                        INNER JOIN kkslm_tr_lead                            AS LEAD         ON      REN.slm_TicketId = LEAD.slm_ticketId
                        INNER JOIN kkslm_tr_productinfo                     AS PRO          ON      PRO.slm_TicketId = LEAD.slm_ticketId
                        INNER JOIN kkslm_tr_cusinfo                         AS CUS          ON      CUS.slm_TicketId = LEAD.slm_ticketId
                        LEFT JOIN KKSLM_MS_STAFF                            AS STAFF        ON      STAFF.slm_UserName = LEAD.SLM_OWNER
                        LEFT JOIN kkslm_ms_teamtelesales                    AS TT           ON      TT.slm_TeamTelesales_Id = STAFF.slm_TeamTelesales_Id
                        LEFT JOIN kkslm_ms_staff                            AS HSTAFF       ON      HSTAFF.slm_StaffId = TT.slm_HeadStaff
                        LEFT JOIN kkslm_tr_prelead                          AS PRE          ON      PRE.slm_TicketId = LEAD.slm_ticketId
                        LEFT JOIN kkslm_ms_title                            AS LEAD_TITLE   ON      LEAD_TITLE.slm_TitleId = LEAD.slm_TitleId
                        LEFT JOIN kkslm_ms_branch                           AS BRANCH       ON      BRANCH.slm_BranchCode = PRE.slm_BranchCode
                        LEFT JOIN kkslm_ms_province                         AS PROVINCE     ON      PROVINCE.slm_ProvinceId = PRO.slm_ProvinceRegis
                        LEFT JOIN kkslm_ms_redbook_brand                    AS BRAND        ON      BRAND.slm_BrandCode = PRO.slm_RedbookBrandCode
                        LEFT JOIN kkslm_ms_redbook_model                    AS MODEL        ON      MODEL.slm_ModelCode = PRO.slm_RedbookModelCode
                             AND MODEL.slm_BrandCode = BRAND.slm_BrandCode
                        LEFT JOIN kkslm_ms_redbook_submodel                 AS SUBMODEL     ON      SUBMODEL.slm_KKKey = PRO.slm_RedbookKKKey
                        LEFT JOIN kkslm_ms_insurancecartype                 AS CarType      ON      CarType.slm_InsurancecarTypeId = ren.slm_InsurancecarTypeId
                        LEFT JOIN kkslm_tr_renewinsurance_address           AS ADDR         ON      ADDR.slm_RenewInsureId = REN.slm_RenewInsureId
                            AND ADDR.slm_AddressType = 'D'
                        LEFT JOIN kkslm_ms_tambol                           AS Tambol       ON      Tambol.slm_TambolId = ADDR.slm_Tambon
                        LEFT JOIN kkslm_ms_amphur                           AS Amphur       ON      Amphur.slm_AmphurId = ADDR.slm_Amphur
                        LEFT JOIN KKSLM_MS_PROVINCE                         AS ProvinceAddr ON      ProvinceAddr.slm_ProvinceId = ADDR.slm_Province
                        LEFT JOIN kkslm_ms_occupation                       AS OCC          ON      OCC.slm_OccupationId = cus.slm_Occupation
                        LEFT JOIN kkslm_ms_marital_status                   AS MS           ON      MS.slm_MaritalStatusId = cus.slm_MaritalStatus
                        WHERE PEN.is_Deleted = 0  AND PEN.slm_Type IN (1,4,6) AND lead.is_Deleted = 0 AND lead.slm_Status NOT IN ('08','09','10') 
                        UNION ALL
                        SELECT CONVERT(VARCHAR(10), GETDATE(),103) + ' ' +
                               CONVERT(VARCHAR(5), GETDATE(), 108)                       AS Created
                               ,CASE WHEN PEN.slm_Type = 1 THEN 'มี Ticket มากกว่า 1 รายการ' 
							         WHEN PEN.slm_Type = 3 AND LEAD.slm_Status = '08' THEN 'มี Ticket แต่สถานะเป็น Reject'
									 WHEN PEN.slm_Type = 3 AND LEAD.slm_Status = '09' THEN 'มี Ticket แต่สถานะเป็น Cancel'
									 WHEN PEN.slm_Type = 3 AND LEAD.slm_Status = '10' THEN 'มี Ticket แต่สถานะเป็น Success'
								     WHEN PEN.SLM_TYPE = 4 THEN 'มี Ticket แต่ไม่มี Flag การซื้อประกันและพรบ'
								     WHEN PEN.SLM_TYPE = 6 THEN 'ไม่มีการ Config การรับเงินเข้าระบบ' ELSE '' END           [Update]
			                  ,CONVERT(VARCHAR(10), PEN.slm_Tran_Date, 103)                AS UpdatedDate
			                  ,HSTAFF.slm_StaffNameTH AS HeadMarketingName
			                  ,REN.slm_TicketId AS TicketId
			                  ,REN.slm_ContractNo AS ContractNo
			                  ,LEAD_TITLE.slm_TitleName AS TitleName
			                  ,LEAD.slm_Name AS CustomerName
			                  ,LEAD.slm_LastName AS CustomerLastName
			                  ,CASE WHEN PEN.slm_Tran_Code = '204' THEN 'ประกัน'
                                    WHEN PEN.slm_Tran_Code = '205' THEN 'พ.ร.บ.' ELSE '' END AS PayType
			                  ,PEN.slm_Inst_No AS InstNo
			                  ,PEN.slm_Rec_by AS RecBy
			                  ,PEN.slm_Rec_No AS RecNo
			                  ,PEN.slm_Rec_Amount AS RecAmount
			                  ,PRE.slm_Voluntary_Company_Name AS CompanyOld
			                  ,PRE.slm_Voluntary_Policy_Number AS PolicyNoOld
			                  ,PRE.slm_BranchCode AS BranchOld
			                  ,BRANCH.slm_BranchName AS BranchOldName
			                  ,ISNULL(PRE.slm_Car_License_No, '') + ISNULL(' ' + PROVINCE.slm_ProvinceNameTH, '') AS LicenseNoProvince
                              ,REN.slm_ReceiveNo AS ReceiveNo
			                  ,STAFF.slm_StaffNameTH AS MarketingName
			                  ,BRAND.slm_BrandName AS BrandName
			                  ,REN.slm_ChassisNo AS ChassisNo
			                  ,REN.slm_EngineNo AS EngineNo
			                  ,PRO.slm_RedbookYearGroup AS YearGroup
			                  ,MODEL.slm_ModelName AS ModelName
			                  ,SUBMODEL.slm_Description AS SubModelDesc
			                  ,CarType.slm_InsurancecarTypeName AS CarTypeName
			                  ,REN.slm_PolicyNetGrossPremium AS PolicyNetGrossPremium
			                  ,REN.slm_PolicyGrossPremiumTotal AS PolicyGrossPremiumTotal
			                  , CONVERT(VARCHAR(10), REN.slm_ActStartCoverDate, 103)    AS ActStartCoverDate
                              , CONVERT(VARCHAR(10), REN.slm_ActEndCoverDate, 103)		AS ActEndCoverDate
                              , ADDR.slm_AddressNo AS AddressNo
			                  ,ADDR.slm_Moo AS Moo
			                  ,ADDR.slm_Village AS Village
			                  ,ADDR.slm_BuildingName AS BuildingName
			                  ,ADDR.slm_Soi AS SOI
			                  ,ADDR.slm_Street AS Street
			                  ,Tambol.slm_TambolNameTH AS TambonName
			                  ,Amphur.slm_AmphurNameTH AS AmphurName
			                  ,ProvinceAddr.slm_ProvinceNameTH AS ProvinceAddrName
			                  ,addr.slm_PostalCode AS PostCode
			                  , CASE WHEN ADDR.slm_AddressNo IS NOT NULL AND ADDR.slm_AddressNo <> '0'  THEN ADDR.slm_AddressNo ELSE '' END +
                            CASE WHEN ADDR.slm_Moo IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_Moo)) <> ''   THEN ' หมู่ ' + ADDR.slm_Moo                        ELSE '' END +
                            CASE WHEN ADDR.slm_BuildingName IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_BuildingName)) <> ''   THEN ' อาคาร ' + ADDR.slm_BuildingName          ELSE '' END +
                            CASE WHEN ADDR.slm_Village      IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_Village)) <> ''   THEN ' หมู่บ้าน ' + ADDR.slm_Village                ELSE '' END +
                            CASE WHEN ADDR.slm_Soi          IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_Soi)) <> ''   THEN ' ซอย ' + ADDR.slm_Soi                     ELSE '' END +
                            CASE WHEN ADDR.slm_Street       IS NOT NULL AND LTRIM(RTRIM(ADDR.slm_Street)) <> ''   THEN ' ถนน ' + ADDR.slm_Street                  ELSE '' END +
                            CASE WHEN Tambol.slm_TambolId   IS NOT NULL  THEN ' ตำบล' + Tambol.slm_TambolNameTH  ELSE '' END +
                            CASE WHEN Amphur.slm_AmphurId   IS NOT NULL  THEN ' อำเภอ' + Amphur.slm_AmphurNameTH  ELSE '' END +
                            CASE WHEN ProvinceAddr.slm_ProvinceId IS NOT NULL   THEN ' จังหวัด' + ProvinceAddr.slm_ProvinceNameTH ELSE '' END + addr.slm_PostalCode AS AddressDescription
                            , LEAD.slm_TelNo_1                                         AS TelContact
                            , CUS.slm_CitizenId                                        AS CitizenId
                            , CONVERT(VARCHAR(10), CUS.slm_Birthdate, 103)             AS Birthdate
                            , OCC.slm_OccupationNameTH                                 AS OccupationName
                            , ms.slm_MaritalStatusName                                 AS MaritalStatusName
                            , CUS.slm_CitizenId                                        AS VatNo
                            , pre.slm_Guarantor_Title_Name1_Org                        AS Guarantor_Title_Name1_Org
                            , PRE.SLM_GUARANTOR_FIRST_NAME1                            AS GUARANTOR_FIRST_NAME1
                            , PRE.SLM_GUARANTOR_LAST_NAME1                             AS GUARANTOR_LAST_NAME1
                            , PRE.slm_Guarantor_Card_Id1                               AS Guarantor_Card_Id1
                            , pre.slm_Guarantor_Title_Name2_Org                        AS Guarantor_Title_Name2_Org
                            , PRE.SLM_GUARANTOR_FIRST_NAME2                            AS GUARANTOR_FIRST_NAME2
                            , PRE.SLM_GUARANTOR_LAST_NAME2                             AS GUARANTOR_LAST_NAME2
                            , PRE.slm_Guarantor_Card_Id2                               AS Guarantor_Card_Id2
                            , pre.slm_Guarantor_Title_Name3_Org                        AS Guarantor_Title_Name3_Org
                            , PRE.SLM_GUARANTOR_FIRST_NAME3                            AS GUARANTOR_FIRST_NAME3
                            , PRE.SLM_GUARANTOR_LAST_NAME3                             AS GUARANTOR_LAST_NAME3
                            , PRE.slm_Guarantor_Card_Id3                               AS Guarantor_Card_Id3
                        FROM kkslm_tr_renewinsurance_hp_payment_pending     AS PEN
                        INNER JOIN kkslm_tr_renewinsurance                  AS REN          ON      PEN.slm_Contract_Number = REN.slm_ContractNo
                        INNER JOIN kkslm_tr_lead                            AS LEAD         ON      REN.slm_TicketId = LEAD.slm_ticketId
                        INNER JOIN kkslm_tr_productinfo                     AS PRO          ON      PRO.slm_TicketId = LEAD.slm_ticketId
                        INNER JOIN kkslm_tr_cusinfo                         AS CUS          ON      CUS.slm_TicketId = LEAD.slm_ticketId
                        LEFT JOIN KKSLM_MS_STAFF                            AS STAFF        ON      STAFF.slm_UserName = LEAD.SLM_OWNER
                        LEFT JOIN kkslm_ms_teamtelesales                    AS TT           ON      TT.slm_TeamTelesales_Id = STAFF.slm_TeamTelesales_Id
                        LEFT JOIN kkslm_ms_staff                            AS HSTAFF       ON      HSTAFF.slm_StaffId = TT.slm_HeadStaff
                        LEFT JOIN kkslm_tr_prelead                          AS PRE          ON      PRE.slm_TicketId = LEAD.slm_ticketId
                        LEFT JOIN kkslm_ms_title                            AS LEAD_TITLE   ON      LEAD_TITLE.slm_TitleId = LEAD.slm_TitleId
                        LEFT JOIN kkslm_ms_branch                           AS BRANCH       ON      BRANCH.slm_BranchCode = PRE.slm_BranchCode
                        LEFT JOIN kkslm_ms_province                         AS PROVINCE     ON      PROVINCE.slm_ProvinceId = PRO.slm_ProvinceRegis
                        LEFT JOIN kkslm_ms_redbook_brand                    AS BRAND        ON      BRAND.slm_BrandCode = PRO.slm_RedbookBrandCode
                        LEFT JOIN kkslm_ms_redbook_model                    AS MODEL        ON      MODEL.slm_ModelCode = PRO.slm_RedbookModelCode
                             AND MODEL.slm_BrandCode = BRAND.slm_BrandCode
                        LEFT JOIN kkslm_ms_redbook_submodel                 AS SUBMODEL     ON      SUBMODEL.slm_KKKey = PRO.slm_RedbookKKKey
                        LEFT JOIN kkslm_ms_insurancecartype                 AS CarType      ON      CarType.slm_InsurancecarTypeId = ren.slm_InsurancecarTypeId
                        LEFT JOIN kkslm_tr_renewinsurance_address           AS ADDR         ON      ADDR.slm_RenewInsureId = REN.slm_RenewInsureId
                            AND ADDR.slm_AddressType = 'D'
                        LEFT JOIN kkslm_ms_tambol                           AS Tambol       ON      Tambol.slm_TambolId = ADDR.slm_Tambon
                        LEFT JOIN kkslm_ms_amphur                           AS Amphur       ON      Amphur.slm_AmphurId = ADDR.slm_Amphur
                        LEFT JOIN KKSLM_MS_PROVINCE                         AS ProvinceAddr ON      ProvinceAddr.slm_ProvinceId = ADDR.slm_Province
                        LEFT JOIN kkslm_ms_occupation                       AS OCC          ON      OCC.slm_OccupationId = cus.slm_Occupation
                        LEFT JOIN kkslm_ms_marital_status                   AS MS           ON      MS.slm_MaritalStatusId = cus.slm_MaritalStatus
                        WHERE PEN.is_Deleted = 0  AND PEN.slm_Type = 3 AND lead.is_Deleted = 0 
                        UNION ALL
                        SELECT  CONVERT(VARCHAR(10), GETDATE(),103) + ' ' +
				                CONVERT(VARCHAR(5), GETDATE(),108)				AS Created
			                   ,'ข้อมูลสถานะ Outbound ไม่พบ ticket ID'				AS [Update]
			                   ,CONVERT(VARCHAR(10), PEN.slm_Tran_Date, 103)	AS UpdatedDate
			                   ,HSTAFF.slm_StaffNameTH							AS HeadMarketingName
			                   ,PRE.slm_TicketId								AS TicketId
			                   ,PRE.slm_Contract_Number							AS ContractNo
			                   ,LEAD_TITLE.slm_TitleName AS TitleName
			                   ,PRE.slm_Name AS CustomerName
			                   ,pre.slm_LastName AS CustomerLastName
			                   ,CASE WHEN PEN.slm_Tran_Code = '204' THEN 'ประกัน' 
			                         WHEN PEN.slm_Tran_Code = '205' THEN 'พ.ร.บ.' 
			                         ELSE '' END								AS PayType
			                   ,PEN.slm_Inst_No									AS InstNo
			                   , PEN.slm_Rec_by									AS RecBy
			                   , PEN.slm_Rec_No									AS RecNo
			                   ,PEN.slm_Rec_Amount								AS RecAmount
			                   ,PRE.slm_Voluntary_Company_Name					AS CompanyOld
			                   ,PRE.slm_Voluntary_Policy_Number					AS PolicyNoOld
			                   ,PRE.slm_BranchCode								AS BranchOld
			                   ,BRANCH.slm_BranchName							AS BranchOldName
			                   ,ISNULL(PRE.slm_Car_License_No,'') + ISNULL(' ' + PROVINCE.slm_ProvinceNameTH,'') AS LicenseNoProvince
			                   ,''												AS ReceiveNo
			                   ,STAFF.slm_StaffNameTH							AS MarketingName
			                   ,BRAND.slm_BrandName								AS BrandName
			                   ,PRE.slm_Chassis_No								AS ChassisNo
			                   ,PRE.slm_Engine_No								AS EngineNo
			                   ,PRE.slm_Model_Year								AS YearGroup
			                   ,MODEL.slm_ModelName								AS ModelName
			                   ,SUBMODEL.slm_Description						AS SubModelDesc 
			                   ,CASE WHEN PRE.slm_Car_By_Gov_Id IS NOT NULL THEN CarType.slm_InsurancecarTypeName ELSE pre.slm_Car_By_Gov_Name_Org END AS CarTypeName
			                   ,NULL												AS PolicyNetGrossPremium
			                   ,NULL												AS PolicyGrossPremiumTotal 
			                   ,''												AS slm_ActStartCoverDate 
			                   ,''												AS slm_ActEndCoverDate 
			                   ,ADDR.slm_House_No								AS AddressNo
			                   ,ADDR.slm_Moo									AS Moo
			                   ,ADDR.slm_Village								AS Village
			                   ,ADDR.slm_Building								AS BuildingName
			                   ,ADDR.slm_Soi									AS SOI
			                   ,ADDR.slm_Street									AS Street
			                   ,Tambol.slm_TambolNameTH							AS TambonName
			                   ,Amphur.slm_AmphurNameTH							AS AmphurName
			                   ,ProvinceAddr.slm_ProvinceNameTH					AS ProvinceAddrName
			                   ,addr.slm_Zipcode								AS PostCode
			                   ,CASE WHEN ADDR.slm_House_No		IS NOT NULL OR ADDR.slm_House_No				<> '0'THEN ADDR.slm_House_No				 ELSE '' END +
				                CASE WHEN ADDR.slm_Moo			IS NOT NULL OR LTRIM(RTRIM(ADDR.slm_Moo))		<> '' THEN ' หมู่ '	+ ADDR.slm_Moo			 ELSE '' END +
				                CASE WHEN ADDR.slm_Building		IS NOT NULL OR LTRIM(RTRIM(ADDR.slm_Building))	<> '' THEN ' อาคาร ' + ADDR.slm_Building		 ELSE '' END  +
				                CASE WHEN ADDR.slm_Village		IS NOT NULL OR LTRIM(RTRIM(ADDR.slm_Village))	<> '' THEN ' หมู่บ้าน '+ ADDR.slm_Village		 ELSE '' END  +
				                CASE WHEN ADDR.slm_Soi			IS NOT NULL OR LTRIM(RTRIM(ADDR.slm_Soi))		<> '' THEN ' ซอย '	+ ADDR.slm_Soi			 ELSE '' END  +
				                CASE WHEN ADDR.slm_Street		IS NOT NULL OR LTRIM(RTRIM(ADDR.slm_Street))	<> '' THEN ' ถนน '	+ ADDR.slm_Street		 ELSE '' END  +
				                CASE WHEN Tambol.slm_TambolId	IS NOT NULL											  THEN ' ตำบล'  + Tambol.slm_TambolNameTH ELSE '' END  +
				                CASE WHEN Amphur.slm_AmphurId	IS NOT NULL											  THEN ' อำเภอ'  + Amphur.slm_AmphurNameTH ELSE '' END  +
				                CASE WHEN ProvinceAddr.slm_ProvinceId IS NOT NULL									  THEN ' จังหวัด' + ProvinceAddr.slm_ProvinceNameTH ELSE '' END  + addr.slm_Zipcode AS AddressDescription
			                   ,ADDR_C.slm_Mobile_Phone							AS TelContact
			                   ,PRE.slm_CitizenId								AS CitizenId
			                   , CONVERT(VARCHAR(10), PRE.slm_Birthdate,103)	AS Birthdate
			                   ,OCC.slm_OccupationNameTH						AS OccupationName
			                   ,PRE.slm_Marital_Status							AS MaritalStatusName
			                   ,PRE.slm_CitizenId								AS VatNo
			                   ,pre.slm_Guarantor_Title_Name1_Org				AS Guarantor_Title_Name1_Org	 
			                   ,PRE.SLM_GUARANTOR_FIRST_NAME1					AS GUARANTOR_FIRST_NAME1
			                   ,PRE.SLM_GUARANTOR_LAST_NAME1					AS GUARANTOR_LAST_NAME1
			                   ,PRE.slm_Guarantor_Card_Id1						AS Guarantor_Card_Id1
			                   ,PRE.slm_Guarantor_Title_Name2_Org				AS Guarantor_Title_Name2_Org	 
			                   ,PRE.SLM_GUARANTOR_FIRST_NAME2					AS GUARANTOR_FIRST_NAME2	
			                   ,PRE.SLM_GUARANTOR_LAST_NAME2					AS GUARANTOR_LAST_NAME2
			                   ,PRE.slm_Guarantor_Card_Id2						AS Guarantor_Card_Id2
			                   ,PRE.slm_Guarantor_Title_Name3_Org				AS Guarantor_Title_Name3_Org	 
			                   ,PRE.SLM_GUARANTOR_FIRST_NAME3					AS GUARANTOR_FIRST_NAME3
			                   ,PRE.SLM_GUARANTOR_LAST_NAME3					AS GUARANTOR_LAST_NAME3
			                   ,PRE.slm_Guarantor_Card_Id3						AS Guarantor_Card_Id3
                              
		                FROM  kkslm_tr_renewinsurance_hp_payment_pending	AS PEN 
		                INNER JOIN kkslm_tr_prelead								AS PRE			ON		PEN.slm_Contract_Number			= PRE.slm_Contract_Number
		                LEFT JOIN KKSLM_MS_STAFF								AS STAFF		ON		STAFF.slm_EmpCode				= PRE.SLM_OWNER
		                LEFT JOIN kkslm_ms_teamtelesales						AS TT			ON		TT.slm_TeamTelesales_Id			= STAFF.slm_TeamTelesales_Id
		                LEFT JOIN kkslm_ms_staff								AS HSTAFF		ON		HSTAFF.slm_StaffId				= TT.slm_HeadStaff
		                LEFT JOIN kkslm_ms_title								AS LEAD_TITLE	ON		LEAD_TITLE.slm_TitleId			= PRE.slm_TitleId  
		                LEFT JOIN kkslm_ms_branch								AS BRANCH		ON		BRANCH.slm_BranchCode			= PRE.slm_BranchCode 
		                LEFT JOIN kkslm_ms_province								AS PROVINCE		ON		PROVINCE.slm_ProvinceId			= PRE.slm_ProvinceRegis 
		                LEFT JOIN kkslm_ms_redbook_brand						AS BRAND		ON		BRAND.slm_BrandCode				= PRE.slm_Brand_Code  
		                LEFT JOIN kkslm_ms_redbook_model						AS MODEL		ON		MODEL.slm_ModelCode				= pre.slm_Model_Code  AND MODEL.slm_BrandCode = BRAND.slm_BrandCode 
		                LEFT JOIN kkslm_ms_redbook_submodel						AS SUBMODEL		ON		SUBMODEL.slm_KKKey				= pre.slm_Model_Code_Org 
		                LEFT JOIN kkslm_tr_prelead_address						AS ADDR			ON		ADDR.slm_Prelead_Id				= pre.slm_Prelead_Id  AND ADDR.slm_Address_Type = 'D'
		                LEFT JOIN kkslm_tr_prelead_address						AS ADDR_C		ON		ADDR.slm_Prelead_Id				= pre.slm_Prelead_Id  AND ADDR.slm_Address_Type	= 'C'
		                LEFT JOIN kkslm_ms_insurancecartype						AS CarType		ON		CarType.slm_InsurancecarTypeId	= PRE.slm_Car_By_Gov_Id  
		                LEFT JOIN kkslm_ms_tambol								AS Tambol		ON		Tambol.slm_TambolId				= ADDR.slm_TambolId
		                LEFT JOIN kkslm_ms_amphur								AS Amphur		ON		Amphur.slm_AmphurId				= ADDR.slm_Amphur_Id
		                LEFT JOIN KKSLM_MS_PROVINCE								AS ProvinceAddr ON		ProvinceAddr.slm_ProvinceId		= ADDR.slm_Province_Id
		                LEFT JOIN kkslm_ms_occupation							AS OCC			ON		OCC.slm_OccupationId			= PRE.slm_OccupationId
		                WHERE PEN.is_Deleted	 = 0 
			                AND PRE.is_Deleted	 = 0 
			                AND PRE.slm_Assign_Status = '1'
			                AND PRE.slm_AssignFlag = '1'
                            AND PEN.is_Deleted = 0  AND PEN.slm_Type = 2
                    UNION ALL
                        SELECT  CONVERT(VARCHAR(10), GETDATE(),103) + ' ' +
				            CONVERT(VARCHAR(5), GETDATE(),108)				AS Created
			               ,'ไม่พบข้อมูลใน OBT'								AS [Update]
			               ,CONVERT(VARCHAR(10), PEN.slm_Tran_Date, 103)    AS UpdatedDate
			               ,''												AS HeadMarketingName
			               ,''												AS TicketId 
			               ,PEN.slm_Contract_Number							AS ContractNo
			               ,''												AS TitleName
			               ,''												AS CustomerName
			               ,''												AS CustomerLastName
			               ,CASE WHEN PEN.slm_Tran_Code = '204' THEN 'ประกัน' 
					             WHEN PEN.slm_Tran_Code = '205' THEN 'พ.ร.บ.' 
					             ELSE '' END								AS PayType
			               ,PEN.slm_Inst_No									AS InstNo
			               ,PEN.slm_Rec_by									AS RecBy
			               ,PEN.slm_Rec_No									AS RecNo
			               ,PEN.slm_Rec_Amount								AS RecAmount
			               ,''												AS CompanyOld
			               ,''												AS PolicyNoOld
			               ,''												AS BranchOld
			               ,''												AS BranchOldName
			               ,''												AS LicenseNoProvince
			               ,''												AS ReceiveNo
			               ,''												AS MarketingName
			               ,''												AS BrandName
			               ,''												AS ChassisNo
			               ,''												AS EngineNo
			               ,''												AS YearGroup
			               ,''												AS ModelName
			               ,''												AS SubModelDesc 
			               ,''												AS CarTypeName
			               ,NULL												AS PolicyNetGrossPremium
			               ,NULL												AS PolicyGrossPremiumTotal 
			               ,''												AS ActStartCoverDate
			               ,''												AS ActEndCoverDate 
			               ,''												AS AddressNo
			               ,''												AS Moo
			               ,''												AS Village
			               ,''												AS BuildingName
			               ,''												AS SOI
			               ,''												AS Street
			               ,''												AS TambonName
			               ,''												AS AmphurName
			               ,''												AS ProvinceAddrName
			               ,''												AS PostCode
			               ,''												AS AddressDescription
			               ,''												AS TelContact
			               ,''												AS CitizenId
			               ,''												AS Birthdate
			               ,''												AS OccupationName
			               ,''												AS MaritalStatusName
			               ,''												AS VatNo
			               ,''												AS Guarantor_Title_Name1_Org
			               ,''												AS GUARANTOR_FIRST_NAME1
			               ,''												AS GUARANTOR_LAST_NAME1
			               ,''												AS Guarantor_Card_Id1
			               ,''												AS Guarantor_Title_Name2_Org 
			               ,''												AS GUARANTOR_FIRST_NAME2
			               ,''												AS GUARANTOR_LAST_NAME2
			               ,''												AS Guarantor_Card_Id2
			               ,''												AS Guarantor_Title_Name3_Org 
			               ,''												AS GUARANTOR_FIRST_NAME3
			               ,''												AS GUARANTOR_LAST_NAME3
			               ,''												AS Guarantor_Card_Id3
                           
		                    FROM kkslm_tr_renewinsurance_hp_payment_pending PEN 
		                    WHERE PEN.is_Deleted = 0 AND PEN.slm_Type = 5 
                            ) A
                        ORDER BY A.[Update], A.UpdatedDate DESC ";

            string sqlOther = @"
SELECT
  CONVERT(VARCHAR(10), GETDATE(), 103) + ' ' +
  CONVERT(VARCHAR(5), GETDATE(), 108)          AS Created,
  'อื่นๆ'                                        AS [Update],
  CONVERT(VARCHAR(10), PEN.slm_Tran_Date, 103) AS UpdatedDate,
  ''                                           AS HeadMarketingName,
  ''                                           AS TicketId,
  PEN.slm_Contract_Number                      AS ContractNo,
  ''                                           AS TitleName,
  ''                                           AS CustomerName,
  ''                                           AS CustomerLastName,
  CASE WHEN PEN.slm_Tran_Code = '204'
    THEN 'ประกัน'
  WHEN PEN.slm_Tran_Code = '205'
    THEN 'พ.ร.บ.'
  ELSE '' END                                  AS PayType,
  PEN.slm_Inst_No                              AS InstNo,
  PEN.slm_Rec_by                               AS RecBy,
  PEN.slm_Rec_No                               AS RecNo,
  PEN.slm_Rec_Amount                           AS RecAmount,
  ''                                           AS CompanyOld,
  ''                                           AS PolicyNoOld,
  ''                                           AS BranchOld,
  ''                                           AS BranchOldName,
  ''                                           AS LicenseNoProvince,
  ''                                           AS ReceiveNo,
  ''                                           AS MarketingName,
  ''                                           AS BrandName,
  ''                                           AS ChassisNo,
  ''                                           AS EngineNo,
  cast(NULL AS INT)                            AS YearGroup,
  ''                                           AS ModelName,
  ''                                           AS SubModelDesc,
  ''                                           AS CarTypeName,
  cast(NULL AS DECIMAL)                        AS PolicyNetGrossPremium,
  cast(NULL AS DECIMAL)                        AS PolicyGrossPremiumTotal,
  ''                                           AS ActStartCoverDate,
  ''                                           AS ActEndCoverDate,
  ''                                           AS AddressNo,
  ''                                           AS Moo,
  ''                                           AS Village,
  ''                                           AS BuildingName,
  ''                                           AS SOI,
  ''                                           AS Street,
  ''                                           AS TambonName,
  ''                                           AS AmphurName,
  ''                                           AS ProvinceAddrName,
  ''                                           AS PostCode,
  ''                                           AS AddressDescription,
  ''                                           AS TelContact,
  ''                                           AS CitizenId,
  ''                                           AS Birthdate,
  ''                                           AS OccupationName,
  ''                                           AS MaritalStatusName,
  ''                                           AS VatNo,
  ''                                           AS Guarantor_Title_Name1_Org,
  ''                                           AS GUARANTOR_FIRST_NAME1,
  ''                                           AS GUARANTOR_LAST_NAME1,
  ''                                           AS Guarantor_Card_Id1,
  ''                                           AS Guarantor_Title_Name2_Org,
  ''                                           AS GUARANTOR_FIRST_NAME2,
  ''                                           AS GUARANTOR_LAST_NAME2,
  ''                                           AS Guarantor_Card_Id2,
  ''                                           AS Guarantor_Title_Name3_Org,
  ''                                           AS GUARANTOR_FIRST_NAME3,
  ''                                           AS GUARANTOR_LAST_NAME3,
  ''                                           AS Guarantor_Card_Id3
FROM kkslm_tr_renewinsurance_hp_payment_pending PEN
WHERE PEN.is_Deleted = 0 AND PEN.slm_Contract_Number NOT IN ({0})
ORDER BY UpdatedDate DESC
";
            DataTable dtMain = db.ExecuteTable(sql);

            // fetch contract that's not present in main
            string[] contractNos = dtMain.AsEnumerable().Select(x => $"'{x.Field<string>("ContractNo")}'").Distinct().ToArray();
            sqlOther = string.Format(sqlOther, contractNos.Length > 0 ? string.Join(",", contractNos) : "'-z1234567890z'");
            DataTable dtOther = db.ExecuteTable(sqlOther);
            dtMain.Merge(dtOther);

            return dtMain;
        }

        //public class PendingPaymentReport
        //{
        //    public string Created { get; set; }
        //    public string Update { get; set; }
        //    public string UpdatedDate { get; set; }
        //    public string TicketId { get; set; }
        //    public string ContractNo { get; set; }
        //    public string TitleName { get; set; }
        //    public string CustomerName { get; set; }
        //    public string CustomerLastName { get; set; }
        //    public string PayType { get; set; }
        //    public string InstNo { get; set; }
        //    public string RecBy { get; set; }
        //    public string RecNo { get; set; }
        //    public decimal? RecAmount { get; set; }
        //    public string CompanyOld { get; set; }
        //    public string PolicyNoOld { get; set; }
        //    public string BranchOld { get; set; }
        //    public string BranchOldName { get; set; }
        //    public string LicenseNoProvince { get; set; }
        //    public string ReceiveNo { get; set; }
        //    public string MarketingName { get; set; }
        //    public string BrandName { get; set; }
        //    public string ChassisNo { get; set; }
        //    public string EngineNo { get; set; }
        //    public int? YearGroup { get; set; }
        //    public string ModelName { get; set; }
        //    public string SubModelDesc { get; set; }
        //    public string CarTypeName { get; set; }
        //    public decimal? PolicyNetGrossPremium { get; set; }
        //    public decimal? PolicyGrossPremiumTotal { get; set; }
        //    public string ActStartCoverDate { get; set; }
        //    public string ActEndCoverDate { get; set; }
        //    public string AddressNo { get; set; }
        //    public string Moo { get; set; }
        //    public string Village { get; set; }
        //    public string BuildingName { get; set; }
        //    public string SOI { get; set; }
        //    public string Street { get; set; }
        //    public string TambonName { get; set; }
        //    public string AmphurName { get; set; }
        //    public string ProvinceAddrName { get; set; }
        //    public string PostCode { get; set; }
        //    public string AddressDescription { get; set; }
        //    public string TelContact { get; set; }
        //    public string CitizenId { get; set; }
        //    public string Birthdate { get; set; }
        //    public string OccupationName { get; set; }
        //    public string MaritalStatusName { get; set; }
        //    public string VatNo { get; set; }
        //    public string Guarantor_Title_Name1_Org { get; set; }
        //    public string GUARANTOR_FIRST_NAME1 { get; set; }
        //    public string GUARANTOR_LAST_NAME1 { get; set; }
        //    public string Guarantor_Card_Id1 { get; set; }
        //    public string Guarantor_Title_Name2_Org { get; set; }
        //    public string GUARANTOR_FIRST_NAME2 { get; set; }
        //    public string GUARANTOR_LAST_NAME2 { get; set; }
        //    public string Guarantor_Card_Id2 { get; set; }
        //    public string Guarantor_Title_Name3_Org { get; set; }
        //    public string GUARANTOR_FIRST_NAME3 { get; set; }
        //    public string GUARANTOR_LAST_NAME3 { get; set; }
        //    public string Guarantor_Card_Id3 { get; set; }
        //}
    }
}
