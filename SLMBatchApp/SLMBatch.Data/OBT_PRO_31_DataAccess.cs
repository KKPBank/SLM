using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SLMBatch.Common;

namespace SLMBatch.Data
{
    public class OBT_PRO_31_DataAccess : SQLDataAccess
    {
        private List<kkslm_tr_preleadportmonitor> _portMonList;
        private List<decimal> _selectedList;
        private List<decimal> _rejectedList = new List<decimal>();

        public override DataTable LoadData()
        {
            return LoadLeadsForTKSData();
        }

        private DataTable LoadLeadsForTKSData()
        {
            using (SLMDBEntities slmdb = AppUtil.GetSlmDbEntities())
            {
                _portMonList = slmdb.kkslm_tr_preleadportmonitor
                    .Where(x => x.is_Deleted == false)
                    .Where(x => x.slm_PortStatus == "A")
                    .Where(x => x.slm_TKSFlag == "N" || x.slm_TKSFlag == null)
                    .ToList();
            }
            if (_portMonList.Count > 0)
            {
                return GetLeadsForTKSType();
            }
            else
            {
                _selectedList = new List<decimal>();
                return new DataTable();
            }
        }

        private DataTable GetLeadsForTKSType()
        {
            string tksLotList;
            if (_portMonList.Count > 0)
            {
                tksLotList = string.Join(", ", _portMonList.Select(x => x.slm_CMTLot ?? 0m));
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            #region Backup 2017-08-31
            //            string sql = string.Format(@"SELECT
            //    LEFT(CONVERT(VARCHAR, GetDate(), 112), 6) AS Batch_No,
            //    PRE.slm_Contract_Number                   AS Contract_No,
            //    PRE.slm_Customer_Key                      AS Customer_No,
            //    PRE.slm_BranchCode                        AS Branch,
            //    CASE WHEN NP.slm_VIN IS NOT NULL
            //        THEN '02'
            //    ELSE '01' END                             AS Document_Type,
            //    'Y'                                       AS PrintFlag,
            //    PRE.slm_Title_Name_Org                    AS Title_Name,
            //    PRE.slm_Name                              AS Customer_Firstname,
            //    PRE.slm_LastName                          AS Customer_Lastname,
            //    PREA.slm_House_No                         AS Customer_Contact_Address_No,
            //    PREA.slm_Village                          AS Customer_Contact_Village,
            //    PREA.slm_Building                         AS Customer_Contact_Building,
            //    PREA.slm_Moo                              AS Customer_Contact_Moo,
            //    PREA.slm_Street                           AS Customer_Contact_Street,
            //    PREA.slm_Soi                              AS Customer_Contact_Soi,
            //    PREA.slm_Tambon_Name_Org                  AS Customer_Contact_Sub_District_Name,
            //    PREA.slm_Amphur_Name_Org                  AS Customer_Contact_Distric_Name,
            //    PREA.slm_Province_name_Org                AS Customer_Contact_Province,
            //    PREA.slm_Zipcode                          AS Customer_Contact_Zip_Code,
            //    PRE.slm_Brand_Name_Org                    AS Car_Brand,
            //    PRE.slm_Model_name_Org                    AS Car_Model,
            //    PRE.slm_Chassis_No                        AS Car_Chassis_Number,
            //    PRE.slm_Model_Year                        AS Car_Model_Year,
            //    PRE.slm_Car_License_No                    AS Car_License_Plate,
            //    PRE.slm_ProvinceRegis_Org                 AS Car_License_Plate_Province,
            //    CONVERT(
            //            VARCHAR(10),
            //			CASE WHEN PRE.slm_Grade IN ('A','B1','B2') THEN PRE.slm_Voluntary_Policy_Exp_Date
            //			WHEN PRE.slm_Grade = 'C' THEN NP.slm_InsExpireDate ELSE '' END,
            //            103
            //    )                                         AS Previous_Insurance_Expire_Date,
            //    PRE.slm_Voluntary_Policy_Number           AS Previous_Policy_Number,
            //    CASE WHEN NP.slm_VIN IS NOT NULL
            //        THEN NP.slm_InsNameTh
            //    ELSE '' END                               AS New_Insurance_Company,
            //    CASE WHEN NP.slm_VIN IS NOT NULL
            //        THEN NP.slm_ConverageTypeName
            //    ELSE '' END                               AS Insurance_Type,
            //    CASE WHEN NP.slm_VIN IS NOT NULL
            //        THEN NP.slm_Claim_Center
            //    ELSE 'N' END                              AS Car_Repair_Department_Flag,
            //    CASE WHEN NP.slm_VIN IS NOT NULL
            //        THEN NP.slm_Claim_Garage
            //    ELSE 'N' END                              AS Car_Repair_Garage_Flag,
            //    CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_DriverFlag = 'Y'
            //        THEN 'Y'
            //    WHEN NP.slm_VIN IS NOT NULL AND NP.slm_DriverFlag = 'N'
            //        THEN 'N'
            //    WHEN NP.slm_VIN IS NOT NULL AND NP.slm_DriverFlag NOT IN ('N', 'Y')
            //        THEN 'U'
            //    WHEN NP.slm_VIN IS NULL
            //        THEN 'N'
            //    ELSE '' END                               AS Specific_Driver_Name_Flag,
            //    CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_CostDamageFlag = 1
            //        THEN NP.slm_Sum_Insure
            //    ELSE 0 END                                AS Car_Insurance_Cost_Damage,
            //    CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_CostLostFlag = 1
            //        THEN NP.slm_Sum_Insure
            //    ELSE 0 END                                AS Car_Insurance_Cost_Lost,
            //    CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_Discount_Amount IS NOT NULL
            //        THEN NP.slm_Discount_Amount
            //    ELSE 0.00 END                             AS Discount_Good_Records,
            //    CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_PolicyGrossPremium IS NOT NULL
            //        THEN NP.slm_PolicyGrossPremium
            //    ELSE 0.00 END                             AS Gross_Premium,
            //    0.00                                      AS Discount_KK,
            //    CASE WHEN NP.slm_VIN IS NOT NULL
            //        THEN NP.slm_InsNameTh
            //    ELSE '' END                               AS Compulsory_Insurance_Company,
            //    CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_ActNetPremium IS NOT NULL
            //        THEN NP.slm_ActNetPremium
            //    ELSE 0.00 END                             AS Compulsory_Gross_Premium,
            //    CASE WHEN NP.slm_VIN IS NOT NULL
            //        THEN NP.slm_Remark
            //    ELSE '' END                               AS Remark,
            //    'Job Schedule'                            AS Sent_Type,
            //    'KK'                                      AS Service_Code_Vol,
            //    PRE.slm_Reference1                        AS Ref1_HP_Vol,
            //    CASE WHEN PRE.slm_Reference2 IS NULL OR PRE.slm_Reference2 = ''
            //        THEN ''
            //    ELSE '5' + PRE.slm_Reference2 END            Ref2_HP_Vol,
            //    'KK'                                      AS Service_Code_Com,
            //    PRE.slm_Reference1                        AS Ref1_HP_Com,
            //    CASE WHEN PRE.slm_Reference2 IS NULL OR PRE.slm_Reference2 = ''
            //        THEN ''
            //    ELSE '6' + PRE.slm_Reference2 END            Ref2_HP_Com,
            //    CONVERT(VARCHAR(10), GETDATE(), 103)      AS System_Date,
            //    PREA.slm_Contract_Adrress                 AS Contract_Address,
            //    PRE.slm_Prelead_Id                        AS PreleadId,
            //    PRE.slm_Grade							  AS Grade
            //FROM {0}kkslm_tr_prelead PRE
            //    INNER JOIN {0}CMT_MAPPING_GRADE GRADE
            //        ON PRE.slm_Product_Id = GRADE.sub_product_id AND
            //           PRE.slm_Grade = GRADE.Grade AND
            //           GRADE.Is_Send_TKS = 1
            //    INNER JOIN {0}kkslm_tr_prelead_address PREA
            //        ON PRE.slm_Prelead_Id = PREA.slm_Prelead_Id
            //           AND PREA.slm_Address_Type = 'D'
            //    LEFT JOIN
            //    (
            //        SELECT
            //            row_number()
            //            OVER ( PARTITION BY slm_VIN
            //                ORDER BY slm_Id DESC ) rn,
            //            PREM.slm_PeriodMonth,
            //            PREM.slm_PeriodYear,
            //            PREM.slm_VIN,
            //            PREM.slm_InsExpireDate,
            //            INS.slm_InsNameTh,
            //            PREM.slm_NetPremium   AS   slm_PolicyGrossPremium,
            //            PREM.slm_GrossPremium AS   slm_NetGrossPremium,
            //            COV.slm_ConverageTypeName,
            //            PREM.slm_Claim_Center,
            //            PREM.slm_Claim_Garage,
            //            PREM.slm_DriverFlag,
            //            CC.slm_CostDamageFlag,
            //            CC.slm_CostLostFlag,
            //            PREM.slm_Sum_Insure,
            //            PREM.slm_Discount_Amount,
            //            PREM.slm_ActNetPremium,
            //            PREM.slm_Remark
            //        FROM {0}kkslm_tr_notify_premium PREM LEFT JOIN {1}kkslm_ms_ins_com INS
            //                ON PREM.slm_InsuranceComId = INS.slm_Ins_Com_Id
            //            LEFT JOIN {0}kkslm_ms_coveragetype COV ON COV.slm_CoverageTypeId = PREM.slm_InsuranceCarTypeId
            //            LEFT JOIN {0}kkslm_ms_mapping_coveragetype_cost CC ON CC.slm_CoverageTypeId = PREM.slm_InsuranceCarTypeId
            //    ) AS NP
            //        ON
            //            NP.rn = 1 AND
            //            PRE.slm_Chassis_No = NP.slm_VIN AND
            //            PRE.slm_PeriodMonth = NP.slm_PeriodMonth AND
            //            PRE.slm_PeriodYear = NP.slm_PeriodYear
            //WHERE
            //    PRE.IS_DELETED = 0 AND
            //    PRE.SLM_ASSIGNFLAG = '1' AND
            //    PRE.SLM_ASSIGN_STATUS = '1' AND
            //    (PRE.slm_IsExportTKS = 0 OR PRE.slm_IsExportTKS IS NULL) AND
            //    PRE.slm_IsExportTKSDate IS NULL AND
            //    PRE.slm_CmtLot IN ({2})
            //",
            //                AppConstant.SLMDBName + ".dbo.",
            //                AppConstant.OPERDBName + ".dbo.",
            //                tksLotList
            //            );
            #endregion 

            string sql = string.Format(@"SELECT
                                        LEFT(CONVERT(VARCHAR, GetDate(), 112), 6) AS Batch_No,
                                        PRE.slm_Contract_Number                   AS Contract_No,
                                        PRE.slm_Customer_Key                      AS Customer_No,
                                        PRE.slm_BranchCode                        AS Branch,
                                        CASE WHEN NP.slm_VIN IS NOT NULL
                                            THEN '02'
                                        ELSE '01' END                             AS Document_Type,
                                        'Y'                                       AS PrintFlag,
                                        PRE.slm_Title_Name_Org                    AS Title_Name,
                                        PRE.slm_Name                              AS Customer_Firstname,
                                        PRE.slm_LastName                          AS Customer_Lastname,
                                        PREA.slm_House_No                         AS Customer_Contact_Address_No,
                                        PREA.slm_Village                          AS Customer_Contact_Village,
                                        PREA.slm_Building                         AS Customer_Contact_Building,
                                        PREA.slm_Moo                              AS Customer_Contact_Moo,
                                        PREA.slm_Street                           AS Customer_Contact_Street,
                                        PREA.slm_Soi                              AS Customer_Contact_Soi,
                                        PREA.slm_Tambon_Name_Org                  AS Customer_Contact_Sub_District_Name,
                                        PREA.slm_Amphur_Name_Org                  AS Customer_Contact_Distric_Name,
                                        PREA.slm_Province_name_Org                AS Customer_Contact_Province,
                                        PREA.slm_Zipcode                          AS Customer_Contact_Zip_Code,
                                        PRE.slm_Brand_Name_Org                    AS Car_Brand,
                                        PRE.slm_Model_name_Org                    AS Car_Model,
                                        PRE.slm_Chassis_No                        AS Car_Chassis_Number,
                                        PRE.slm_Model_Year                        AS Car_Model_Year,
                                        PRE.slm_Car_License_No                    AS Car_License_Plate,
                                        PRE.slm_ProvinceRegis_Org                 AS Car_License_Plate_Province,
                                        ''										  AS Previous_Insurance_Expire_Date,
                                        CASE WHEN NP.slm_VIN IS NOT NULL
											THEN NP.slm_PolicyNo
										ELSE PRE.slm_Voluntary_Policy_Number END  AS Previous_Policy_Number,
                                        CASE WHEN NP.slm_VIN IS NOT NULL
                                            THEN NP.slm_InsNameTh
                                        ELSE '' END                               AS New_Insurance_Company,
                                        CASE WHEN NP.slm_VIN IS NOT NULL
                                            THEN 
												CASE WHEN LTRIM(RTRIM(NP.slm_ConverageTypeName)) = 'ประกันภัยชั้น 1' THEN '1'
												WHEN LTRIM(RTRIM(NP.slm_ConverageTypeName)) = 'ประกันภัยชั้น 2' THEN '2'
												WHEN LTRIM(RTRIM(NP.slm_ConverageTypeName)) = 'ประกันภัยชั้น 2 Plus' THEN '2+'
												WHEN LTRIM(RTRIM(NP.slm_ConverageTypeName)) = 'ประกันภัยชั้น 3' THEN '3'
												WHEN LTRIM(RTRIM(NP.slm_ConverageTypeName)) = 'ประกันภัยชั้น 3 Plus' THEN '3+'
												WHEN LTRIM(RTRIM(NP.slm_ConverageTypeName)) = 'ประกันภัยชั้น 4' THEN '4'
												WHEN LTRIM(RTRIM(NP.slm_ConverageTypeName)) = 'ประกันภัยชั้น 5' THEN '5'
												ELSE '9999' END
                                        ELSE '' 
										END										  AS Insurance_Type,
                                        CASE WHEN NP.slm_VIN IS NOT NULL
                                            THEN NP.slm_Claim_Center
                                        ELSE 'N' END                              AS Car_Repair_Department_Flag,
                                        CASE WHEN NP.slm_VIN IS NOT NULL
                                            THEN NP.slm_Claim_Garage
                                        ELSE 'N' END                              AS Car_Repair_Garage_Flag,
                                        CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_DriverFlag = 'Y'
                                            THEN 'Y'
                                        WHEN NP.slm_VIN IS NOT NULL AND NP.slm_DriverFlag = 'N'
                                            THEN 'N'
                                        WHEN NP.slm_VIN IS NOT NULL AND NP.slm_DriverFlag NOT IN ('N', 'Y')
                                            THEN 'U'
                                        WHEN NP.slm_VIN IS NULL
                                            THEN 'N'
                                        ELSE '' END                               AS Specific_Driver_Name_Flag,
                                        CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_CostDamageFlag = 1
                                            THEN NP.slm_Sum_Insure
                                        ELSE 0 END                                AS Car_Insurance_Cost_Damage,
                                        CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_CostLostFlag = 1
                                            THEN NP.slm_Sum_Insure
                                        ELSE 0 END                                AS Car_Insurance_Cost_Lost,
                                        CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_Discount_Amount IS NOT NULL
                                            THEN CONVERT(VARCHAR(50), CAST(NP.slm_Discount_Amount AS money), 1)
                                        ELSE '0.00' END                             AS Discount_Good_Records,
                                        CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_PolicyGrossPremium IS NOT NULL
                                            THEN CONVERT(VARCHAR(50), CAST(NP.slm_PolicyGrossPremium AS money), 1)
                                        ELSE '0.00' END                             AS Gross_Premium,
                                        '0.00'                                      AS Discount_KK,
                                        CASE WHEN NP.slm_VIN IS NOT NULL
                                            THEN NP.slm_InsNameTh
                                        ELSE '' END                               AS Compulsory_Insurance_Company,
                                        CASE WHEN NP.slm_VIN IS NOT NULL AND NP.slm_ActGrossPremium IS NOT NULL
                                            THEN CONVERT(VARCHAR(50), CAST(NP.slm_ActGrossPremium AS money), 1)
                                        ELSE '0.00' END                             AS Compulsory_Gross_Premium,
                                        CASE WHEN NP.slm_VIN IS NOT NULL
                                            THEN NP.slm_Remark
                                        ELSE '' END                               AS Remark,
                                        'Job Schedule'                            AS Sent_Type,
                                        'KK'                                      AS Service_Code_Vol,
                                        PRE.slm_Reference1                        AS Ref1_HP_Vol,
                                        CASE WHEN PRE.slm_Reference2 IS NULL OR PRE.slm_Reference2 = ''
                                            THEN ''
                                        ELSE '5' + PRE.slm_Reference2 END            Ref2_HP_Vol,
                                        'KK'                                      AS Service_Code_Com,
                                        PRE.slm_Reference1                        AS Ref1_HP_Com,
                                        CASE WHEN PRE.slm_Reference2 IS NULL OR PRE.slm_Reference2 = ''
                                            THEN ''
                                        ELSE '6' + PRE.slm_Reference2 END            Ref2_HP_Com,
                                        CONVERT(VARCHAR(10), GETDATE(), 103)      AS System_Date,
                                        PREA.slm_Contract_Adrress                 AS Contract_Address,
                                        PRE.slm_Prelead_Id                        AS PreleadId,
                                        PRE.slm_Grade							  AS Grade,
	                                    ISNULL(NP.slm_VIN, '')					  AS VIN,
	                                    ISNULL(CONVERT(VARCHAR(10), NP.slm_InsExpireDate,103), '') AS InsExpireDate,
	                                    ISNULL(CONVERT(VARCHAR(10), PRE.slm_Voluntary_Policy_Exp_Date,103), '') AS Voluntary_Policy_Exp_Date
                                    FROM {0}kkslm_tr_prelead PRE
                                        INNER JOIN {0}CMT_MAPPING_GRADE GRADE
                                            ON PRE.slm_Product_Id = GRADE.sub_product_id AND
                                               PRE.slm_Grade = GRADE.Grade AND
                                               GRADE.Is_Send_TKS = 1
                                        INNER JOIN {0}kkslm_tr_prelead_address PREA
                                            ON PRE.slm_Prelead_Id = PREA.slm_Prelead_Id
                                               AND PREA.slm_Address_Type = 'D'
                                        LEFT JOIN
                                        (
                                            SELECT
                                                row_number()
                                                OVER ( PARTITION BY REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') 
                                                    ORDER BY slm_Id DESC ) rn,
                                                PREM.slm_PeriodMonth,
                                                PREM.slm_PeriodYear,
                                                PREM.slm_VIN,
                                                PREM.slm_InsExpireDate,
                                                INS.slm_InsNameTh,
                                                PREM.slm_GrossPremium   AS   slm_PolicyGrossPremium,
                                                PREM.slm_NetPremium AS   slm_NetGrossPremium,
                                                COV.slm_ConverageTypeName,
                                                PREM.slm_Claim_Center,
                                                PREM.slm_Claim_Garage,
                                                PREM.slm_DriverFlag,
                                                CC.slm_CostDamageFlag,
                                                CC.slm_CostLostFlag,
                                                PREM.slm_Sum_Insure,
                                                PREM.slm_Discount_Amount,
                                                PREM.slm_ActGrossPremium,
                                                PREM.slm_Remark,
                                                PREM.slm_PolicyNo
                                            FROM {0}kkslm_tr_notify_premium PREM LEFT JOIN {1}kkslm_ms_ins_com INS
                                                    ON PREM.slm_InsuranceComId = INS.slm_Ins_Com_Id
                                                LEFT JOIN {0}kkslm_ms_coveragetype COV ON COV.slm_CoverageTypeId = PREM.slm_InsuranceCarTypeId
                                                LEFT JOIN {0}kkslm_ms_mapping_coveragetype_cost CC ON CC.slm_CoverageTypeId = PREM.slm_InsuranceCarTypeId
                                        ) AS NP
                                            ON
                                                NP.rn = 1 AND
                                                REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(PRE.slm_Chassis_No,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(NP.slm_VIN,' ',''),'-',''),'*',''),'_',''),'\',''),'/','') AND 
                                                PRE.slm_PeriodMonth = NP.slm_PeriodMonth AND
                                                PRE.slm_PeriodYear = NP.slm_PeriodYear
                                    WHERE
                                        PRE.IS_DELETED = 0 AND
                                        PRE.SLM_ASSIGNFLAG = '1' AND
                                        PRE.SLM_ASSIGN_STATUS = '1' AND
                                        (PRE.slm_IsExportTKS = 0 OR PRE.slm_IsExportTKS IS NULL) AND
                                        PRE.slm_IsExportTKSDate IS NULL AND
                                        PRE.slm_CmtLot IN ({2})
                                    ",
                AppConstant.SLMDBName + ".dbo.",
                AppConstant.OPERDBName + ".dbo.",
                tksLotList
            );
            //Console.WriteLine($"Working on lot: {tksLotList}");

            DataTable dt = db.ExecuteTable(sql);

            #region convert special date 01/01/1900 to null

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var grade = dt.Rows[i].Field<string>("Grade");
                var vin = dt.Rows[i].Field<string>("VIN");
                var insExpireDate = dt.Rows[i].Field<string>("InsExpireDate");
                var voluntaryPolicyExpDate = dt.Rows[i].Field<string>("Voluntary_Policy_Exp_Date");

                var gradeList = new string[] { "A", "B1", "B2" };
                if (gradeList.Contains(grade))
                {
                    if (!string.IsNullOrWhiteSpace(vin) && !string.IsNullOrWhiteSpace(insExpireDate) && insExpireDate != "01/01/1900")
                    {
                        dt.Rows[i]["Previous_Insurance_Expire_Date"] = insExpireDate;
                    }
                    else
                    {
                        dt.Rows[i]["Previous_Insurance_Expire_Date"] = voluntaryPolicyExpDate;
                    }
                }
                else if (grade == "C")
                {
                    if (string.IsNullOrWhiteSpace(insExpireDate))
                    {
                        _rejectedList.Add(dt.Rows[i].Field<decimal>("PreleadId"));
                        dt.Rows[i].Delete();
                    }
                    else
                    {
                        dt.Rows[i]["Previous_Insurance_Expire_Date"] = (insExpireDate == "01/01/1900" ? "-" : insExpireDate);
                    }
                }
                else
                {
                    _rejectedList.Add(dt.Rows[i].Field<decimal>("PreleadId"));
                    dt.Rows[i].Delete();
                }

                //var previousInsuranceExpireDate = dt.Rows[i].Field<string>(colName);
                //var grade = dt.Rows[i].Field<string>(colGrade);

                //if (string.IsNullOrWhiteSpace(previousInsuranceExpireDate) && grade == "C")
                //{
                //    // if record didn't contain insurance expire date something went wrong
                //    // this record will not goes to export excel
                //    // and will have flag slm_IsExportTKS stamp 0
                //    _rejectedList.Add(dt.Rows[i].Field<decimal>("PreleadId"));
                //    dt.Rows[i].Delete();
                //}
                //else if (previousInsuranceExpireDate == "01/01/1900" && grade == "C")
                //{
                //    // if you frown upon this
                //    // please known that 01/01/1900 is intentional, magic string used in operation
                //    dt.Rows[i][colName] = "-";
                //}
            }
            dt.AcceptChanges();

            #endregion

            _selectedList = dt.AsEnumerable().Select(x => x.Field<decimal>("PreleadId")).ToList<decimal>();
            dt.Columns.Remove("PreleadId");
            dt.Columns.Remove("Grade");
            dt.Columns.Remove("VIN");
            dt.Columns.Remove("InsExpireDate");
            dt.Columns.Remove("Voluntary_Policy_Exp_Date");

            //Console.WriteLine($"DataTable contain {dt.Rows.Count} rows, rejected {_rejectedList.Count} rows");

            return dt;
        }

        public override void FeedbackData()
        {
            var feedback = 0;
            if (_selectedList.Count > 0)
            {
                const int pageSize = 10000;
                var sqlsStamp0 = new List<string>();
                for (var i = 0; i < _rejectedList.Count; i += pageSize)
                {
                    sqlsStamp0.Add(string.Format(@"
UPDATE {0}kkslm_tr_prelead
SET slm_IsExportTKS = 0, slm_IsExportTKSDate = @now, slm_UpdatedDate = @now, slm_UpdatedBy = 'SYSTEM'
WHERE slm_Prelead_Id IN ({1});
",
                        AppConstant.SLMDBName + ".dbo.",
                        string.Join(",", _rejectedList.Skip(i).Take(pageSize))));
                }

                var sqlsStamp1 = new List<string>();
                for (var i = 0; i < _selectedList.Count; i += pageSize)
                {
                    sqlsStamp1.Add(string.Format(@"
UPDATE {0}kkslm_tr_prelead
SET slm_IsExportTKS = 1, slm_IsExportTKSDate = @now, slm_UpdatedDate = @now, slm_UpdatedBy = 'SYSTEM'
WHERE slm_Prelead_Id IN ({1});
",
                        AppConstant.SLMDBName + ".dbo.",
                        string.Join(",", _selectedList.Skip(i).Take(pageSize))));
                }
                string sql = $@"
BEGIN
  DECLARE @now AS DATETIME = getdate();
{string.Join("\n", sqlsStamp0)}
{string.Join("\n", sqlsStamp1)}
END;
";
                feedback = db.ExecuteNonQuery(sql);
            }
            System.Diagnostics.Debug.Assert(!_selectedList.GroupBy(x => x).Any(x => x.Count() > 1));
            Console.WriteLine($"DataTable contain {_selectedList.Count} rows, rejected {_rejectedList.Count} rows, feedback = {feedback}, prelead");

            feedback = 0;
            if (_portMonList.Count > 0)
            {
                using (SLMDBEntities slmdb = AppUtil.GetSlmDbEntities())
                {
                    DateTime dbNow = slmdb.DBNow();
                    foreach (kkslm_tr_preleadportmonitor portmon in _portMonList)
                    {
                        slmdb.kkslm_tr_preleadportmonitor.Attach(portmon);
                        portmon.slm_TKSFlag = "Y";
                        portmon.slm_UpdatedDate = dbNow;
                        portmon.slm_UpdatedBy = "SYSTEM";
                    }
                    feedback += slmdb.SaveChanges();
                }
            }
            System.Diagnostics.Debug.Assert(!_portMonList.GroupBy(x => x.slm_CMTLot).Any(x => x.Count() > 1));
            Console.WriteLine($"DataTable contain {_portMonList.Count} rows, feedback = {feedback}, portMon");
        }
    }
}
