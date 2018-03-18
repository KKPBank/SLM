using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SLMBatch.Common;

namespace SLMBatch.Data
{
    public class OBT_PRO_30_DataAccess : SQLDataAccess
    {
        private List<decimal> _selectedList;

        public override DataTable LoadData()
        {
            return LoadLeadsForTransferData();
        }

        private DataTable LoadLeadsForTransferData()
        {
            return GetLeadsForTransferType();
        }

        private DataTable GetLeadsForTransferType()
        {
            bool isLateThanCutoff = IsLateThanCutoff();
            int leadsForTransferExpNotiDay = AppConstant.LeadsForTransfer.ExpNotiDay + (isLateThanCutoff ? 1 : 0);

            const int debugLookbackDay = 0;
            string sql = string.Format(@"SELECT DISTINCT
  TT.slm_TeamTelesales_Name                                                                            AS TeamName,
  STAFF.slm_EmpCode                                                                                    AS EmployeeCode,
  OPT.slm_OptionDesc                                                                                   AS StatusDesc,
  OP.slm_SubStatusName                                                                                 AS SubStatusDesc,
  PRELEAD.slm_TicketId                                                                                 AS TickerID,
  PRELEAD.slm_Contract_Number                                                                          AS ContractNo,
  ISNULL(tl.slm_TitleName, '') + ISNULL(prelead.slm_Name, '') + ' ' + ISNULL(prelead.slm_LastName, '') AS CustFullName,
  PRELEAD.slm_Voluntary_Company_Name                                                                   AS InsuranceCompanyName,
  COV.slm_ConverageTypeName                                                                            AS CoverageTypeName,
  PRELEAD.slm_Voluntary_Gross_Premium                                                                  AS PolicyGrossPremium,
  PRELEAD.slm_Grade                                                                                    AS Grade,
  PRELEAD.slm_Prelead_Id                                                                               AS PreleadId,
  prelead.slm_CmtLot                                                                                   AS CmtLot,
  getdate()                                                                                            AS TransferDate,
  STAFF.slm_StaffNameTH                                                                                AS EmployeeName,
  PRELEAD.slm_Voluntary_Policy_Exp_Month                                                               AS PolicyExpireMonth,
  PRELEAD.slm_Voluntary_Policy_Exp_Date                                                                AS PolicyExpireDate,
  PRELEAD.slm_NextContactDate                                                                          AS NextContactDate,
  PRELEAD.slm_AssignDescription                                                                        AS AssignDescription,
  PRELEAD.slm_Car_By_Gov_Name_Org                                                                      AS Car_By_Gov_Name_Org,
  PRELEAD.slm_Brand_Name_Org                                                                           AS BrandName,
  PRELEAD.slm_Model_name_Org                                                                           AS Modelname,
  PRELEAD.slm_Voluntary_Policy_Number                                                                  AS PolicyNumber,
  PRELEAD.slm_BranchCode                                                                               AS BranchCode,
  isnull(NP.slm_FlagNotifyPremium, 'N')                                                                AS NotifyFlag,
  isnull(NP.slm_PolicyGrossPremium, 0.00)                                                              AS PriceNotify,
  PRELEAD.slm_Chassis_No                                                                               AS ChassisNo,
  PRELEAD.slm_Engine_No                                                                                AS EngineNo,
  PRELEAD.slm_Car_License_No                                                                           AS CarLicenseNo,
  PRELEAD.slm_ProvinceRegis_Org                                                                        AS ProvinceRegis
FROM {0}..kkslm_tr_prelead PRELEAD
  INNER JOIN {0}..kkslm_ms_config_product_substatus OP ON
                                                    OP.slm_OptionCode = PRELEAD.slm_Status AND
                                                    OP.slm_SubStatusCode = PRELEAD.slm_SubStatus AND
                                                    (
                                                      OP.slm_CampaignId = PRELEAD.slm_CampaignId OR
                                                      OP.slm_Product_Id = PRELEAD.slm_Product_Id
                                                    )
  INNER JOIN {0}..kkslm_ms_staff staff ON staff.slm_EmpCode = PRELEAD.slm_Owner 
  INNER JOIN {0}..kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
  INNER JOIN {0}..kkslm_ms_option OPT ON OPT.slm_OptionCode = PRELEAD.slm_Status AND OPT.slm_OptionType = 'lead status'
  LEFT JOIN {0}..kkslm_ms_teamtelesales TT ON TT.slm_TeamTelesales_Id = STAFF.slm_TeamTelesales_Id
  LEFT JOIN {0}..kkslm_ms_title tl ON tl.slm_TitleId = prelead.slm_TitleId
  LEFT JOIN {0}..kkslm_ms_coveragetype COV ON COV.slm_CoverageTypeId = PRELEAD.SLM_Voluntary_Type_Key
  LEFT JOIN (SELECT
               row_number()
               OVER ( PARTITION BY REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') 
                 ORDER BY slm_Id DESC ) rn,
               slm_VIN,
               slm_InsExpireDate AS     slm_LatestInsExpireDate,
               slm_CreatedDate   AS     slm_LatestCreatedDate,
               slm_NetPremium    AS     slm_PolicyGrossPremium,
               slm_GrossPremium  AS     slm_NetGrossPremium,
               'Y'               AS     slm_FlagNotifyPremium,
               slm_PeriodMonth,
               slm_PeriodYear
             FROM {0}..kkslm_tr_notify_premium) NP
    ON REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(NP.slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(PRELEAD.slm_Chassis_No,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') AND NP.rn = 1 
       AND NP.slm_PeriodMonth = PRELEAD.slm_PeriodMonth
       AND NP.slm_PeriodYear = PRELEAD.slm_PeriodYear
WHERE PRELEAD.is_Deleted = 0 AND prelead.slm_AssignFlag = '1' AND PRELEAD.slm_Assign_Status = '1'
      AND prelead.slm_SubStatus NOT IN ('07')
      AND prelead.slm_TicketId IS NULL
      AND prelead.slm_IsExportExpired IS NULL
      AND ST.slm_StaffTypeId = {3}
      AND PRELEAD.slm_Voluntary_Policy_Exp_Date >= cast(getdate() - {2} AS DATE)
      AND PRELEAD.slm_Voluntary_Policy_Exp_Date < cast(dateadd(DAY, {1}, getdate()) AS DATE)
UNION ALL
SELECT DISTINCT
  TT.slm_TeamTelesales_Name                                                                      AS TeamName,
  STAFF.slm_EmpCode                                                                              AS EmployeeCode,
  OPT.slm_OptionDesc                                                                             AS StatusDesc,
  LEAD.slm_ExternalSubStatusDesc                                                                 AS SubStatusDesc,
  REN.slm_TicketId                                                                               AS TicketID,
  REN.slm_ContractNo                                                                             AS ContractNo,
  ISNULL(tl.slm_TitleName, '') + ISNULL(LEAD.slm_Name, '') + ' ' + ISNULL(LEAD.slm_LastName, '') AS CustFullName,
  PRELEAD.slm_Voluntary_Company_Name                                                             AS InsuranceCompanyName,
  COV.slm_ConverageTypeName                                                                      AS CoverageTypeName,
  PRELEAD.slm_Voluntary_Gross_Premium                                                            AS PolicyGrossPremium,
  PRELEAD.slm_Grade                                                                              AS Grade,
  PRELEAD.slm_Prelead_Id                                                                         AS PreleadId,
  prelead.slm_CmtLot                                                                             AS CmtLot,
  getdate()                                                                                      AS TransferDate,
  STAFF.slm_StaffNameTH                                                                          AS EmployeeName,
  PRELEAD.slm_Voluntary_Policy_Exp_Month                                                         AS PolicyExpireMonth,
  PRELEAD.slm_Voluntary_Policy_Exp_Date                                                          AS PolicyExpireDate,
  LEAD.slm_NextContactDate                                                                       AS NextContactDate,
  PRELEAD.slm_AssignDescription                                                                  AS AssignDescription,
  PRELEAD.slm_Car_By_Gov_Name_Org                                                                AS Car_By_Gov_Name_Org,
  PRELEAD.slm_Brand_Name_Org                                                                     AS BrandName,
  PRELEAD.slm_Model_name_Org                                                                     AS Modelname,
  PRELEAD.slm_Voluntary_Policy_Number                                                            AS PolicyNumber,
  PRELEAD.slm_BranchCode                                                                         AS BranchCode,
  isnull(NP.slm_FlagNotifyPremium, 'N')                                                          AS NotifyFlag,
  isnull(NP.slm_PolicyGrossPremium, 0.00)                                                        AS PriceNotify,
  PRELEAD.slm_Chassis_No                                                                         AS ChassisNo,
  PRELEAD.slm_Engine_No                                                                          AS EngineNo,
  PRELEAD.slm_Car_License_No                                                                     AS CarLicenseNo,
  PRELEAD.slm_ProvinceRegis_Org                                                                  AS ProvinceRegis
FROM {0}..kkslm_tr_prelead PRELEAD INNER JOIN {0}..kkslm_tr_renewinsurance REN ON REN.SLM_TICKETID = PRELEAD.slm_TicketId
  INNER JOIN {0}..KKSLM_tR_LEAD LEAD ON LEAD.SLM_TICKETID = REN.SLM_TICKETID
  INNER JOIN {0}..kkslm_tr_productinfo PRO ON PRO.slm_TicketId = LEAD.slm_ticketId
  INNER JOIN {0}..kkslm_ms_staff staff ON staff.slm_UserName = LEAD.slm_Owner 
  INNER JOIN {0}..kkslm_ms_staff_type ST ON ST.slm_StaffTypeId = staff.slm_StaffTypeId
  INNER JOIN {0}..kkslm_ms_option OPT ON OPT.slm_OptionCode = LEAD.slm_Status AND OPT.slm_OptionType = 'lead status'
  LEFT JOIN {0}..kkslm_ms_teamtelesales TT ON TT.slm_TeamTelesales_Id = STAFF.slm_TeamTelesales_Id
  LEFT JOIN {0}..kkslm_ms_title tl ON tl.slm_TitleId = LEAD.slm_TitleId
  LEFT JOIN {0}..kkslm_ms_coveragetype COV ON COV.slm_CoverageTypeId = PRO.slm_CarType
  LEFT JOIN (SELECT
               row_number()
               OVER ( PARTITION BY REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') 
                 ORDER BY slm_Id DESC ) rn,
               slm_VIN,
               slm_InsExpireDate AS     slm_LatestInsExpireDate,
               slm_CreatedDate   AS     slm_LatestCreatedDate,
               slm_NetPremium    AS     slm_PolicyGrossPremium,
               slm_GrossPremium  AS     slm_NetGrossPremium,
               'Y'               AS     slm_FlagNotifyPremium,
               slm_PeriodMonth,
               slm_PeriodYear
             FROM {0}..kkslm_tr_notify_premium) NP
    ON REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(NP.slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REN.slm_ChassisNo,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') AND NP.rn = 1 
       AND NP.slm_PeriodMonth = REN.slm_PeriodMonth
       AND NP.slm_PeriodYear = REN.slm_PeriodYear
WHERE
  PRELEAD.is_Deleted = 0 AND LEAD.is_Deleted = 0 AND prelead.slm_AssignFlag = '1' AND PRELEAD.slm_Assign_Status = '1'
  AND prelead.slm_SubStatus NOT IN ('07')
  AND LEAD.slm_Status NOT IN ('08', '09', '10') 
  AND PRELEAD.slm_TicketId IS NOT NULL
  AND prelead.slm_IsExportExpired IS NULL
  AND ren.slm_receiveNo IS NULL
  AND ST.slm_StaffTypeId = {3}
  AND PRELEAD.slm_Voluntary_Policy_Exp_Date >= cast(getdate() - {2} AS DATE)
  AND PRELEAD.slm_Voluntary_Policy_Exp_Date < cast(dateadd(DAY, {1}, getdate()) AS DATE)",
                AppConstant.SLMDBName, leadsForTransferExpNotiDay, debugLookbackDay, AppConstant.LeadsForTransfer.ResponsibleRoleNumber);
            Console.WriteLine($"Look forward for {leadsForTransferExpNotiDay} days, look back for {debugLookbackDay} days");

            DataTable dt = db.ExecuteTable(sql);
            _selectedList = dt.AsEnumerable().Select(x => x.Field<decimal>("PreleadId")).ToList<decimal>();

            Console.WriteLine($"DataTable contain {dt.Rows.Count} rows");

            return dt;
        }

        public override void FeedbackData()
        {
            var feedback = 0;
            if (_selectedList.Count > 0)
            {
                const int pageSize = 10000;
                var sqls = new List<string>();
                for (var i = 0; i < _selectedList.Count; i += pageSize)
                {
                    sqls.Add(string.Format(@"
UPDATE {0}..kkslm_tr_prelead
SET slm_IsExportExpired = 1, slm_IsExportExpiredDate = @now
WHERE slm_Prelead_Id IN ({1});
"
                        , AppConstant.SLMDBName, string.Join(",", _selectedList.Skip(i).Take(pageSize))));
                }
                string sql = $@"
BEGIN
  DECLARE @now AS DATETIME = getdate()
{string.Join("\n", sqls)}
END;
";
                feedback = db.ExecuteNonQuery(sql);
            }
            System.Diagnostics.Debug.Assert(!_selectedList.GroupBy(x => x).Any(x => x.Count() > 1));
            Console.WriteLine($"DataTable contain {_selectedList.Count} rows, feedback = {feedback}");
        }
    }
}
