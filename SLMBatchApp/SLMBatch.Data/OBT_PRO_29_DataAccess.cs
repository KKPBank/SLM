using System.Data;

namespace SLMBatch.Data
{
    public class OBT_PRO_29_DataAccess : SQLDataAccess
    {
        public override DataTable LoadData()
        {
            return LoadPolicyNoActNoPendingData();
        }

        private DataTable LoadPolicyNoActNoPendingData()
        {
            return Getpolicyno_actno_pendingType();
        }

        private DataTable Getpolicyno_actno_pendingType()
        {
            string sql = @"SELECT CONVERT(VARCHAR(10), GETDATE(),103) + ' ' +
                                   CONVERT(VARCHAR(5), GETDATE(), 108)                       AS Created
                                   ,CASE WHEN PEN.slm_Type = 1 THEN 'มี Ticket มากกว่า 1 รายการ ' 
							             WHEN PEN.slm_Type = 3 AND LEAD.slm_Status = '08' THEN 'มี Ticket แต่สถานะเป็น Reject'
										 WHEN PEN.slm_Type = 3 AND LEAD.slm_Status = '09' THEN 'มี Ticket แต่สถานะเป็น Cancel'
										 WHEN PEN.slm_Type = 3 AND LEAD.slm_Status = '10' THEN 'มี Ticket แต่สถานะเป็น Success'
								         WHEN PEN.SLM_TYPE = 4 THEN 'มี Ticket แต่ไม่มี Flag การซื้อประกันและพรบ'
									     WHEN PEN.SLM_TYPE = 7 THEN 'มี Ticket แต่ไม่มีเลขที่รับแจ้ง' ELSE '' END           [Update]
							      ,PEN.slm_Contract_Number										AS ContractNoBDW
							      ,PEN.slm_Policy_No											AS PolicyNoBDW
							      ,PEN.slm_Create_Date											AS PolicyDateBDW
							      ,REN.slm_ContractNo											AS ContractNo
							      ,ISNULL(LEAD_TITLE.slm_TitleName,'')+ ISNULL(LEAD.slm_Name,'')+' '+ ISNULL(LEAD.slm_LastName,'') AS CustomerName
							      ,INSPO.slm_InsCode											AS PolicyInsCode
							      ,'98'															AS Agent
							      ,CONVERT(VARCHAR(10), REN.slm_PolicyStartCoverDate , 103)    AS PolicyStartCoverDate
                                  ,CONVERT(VARCHAR(10), REN.slm_PolicyEndCoverDate, 103)		AS PolicyEndCoverDate
							      ,REN.slm_PolicyNo												AS PolicyNo
							      ,REN.slm_PolicyNetGrossPremium								AS PolicyNetGrossPremium
							      ,REN.slm_PolicyGrossStamp										AS PolicyGrossStamp	
							      ,REN.slm_PolicyGrossVat										AS PolicyGrossVat
							      ,''															AS PolicyNoDate
							      ,''															AS EmsNo
                            FROM kkslm_tr_renewinsurance_hp_policyno_actno_pending			AS PEN
                            INNER JOIN kkslm_tr_renewinsurance                  AS REN          ON      PEN.slm_Contract_Number = REN.slm_ContractNo
                            INNER JOIN kkslm_tr_lead                            AS LEAD         ON      REN.slm_TicketId = LEAD.slm_ticketId
						    LEFT JOIN OPERDB.DBO.kkslm_ms_ins_com				AS INSPO		ON		REN.slm_InsuranceComId = INSPO.slm_Ins_Com_Id
                            LEFT JOIN kkslm_tr_prelead                          AS PRE          ON      PRE.slm_TicketId = LEAD.slm_ticketId
                            LEFT JOIN kkslm_ms_title                            AS LEAD_TITLE   ON      LEAD_TITLE.slm_TitleId = LEAD.slm_TitleId
                            WHERE PEN.is_Deleted = 0  AND PEN.slm_Type IN (1,3,4,7) AND lead.is_Deleted = 0 
                            UNION ALL
                            SELECT  CONVERT(VARCHAR(10), GETDATE(),103) + ' ' +
				                    CONVERT(VARCHAR(5), GETDATE(),108)				AS Created
			                       ,'ข้อมูลสถานะ Outbound ไม่พบ ticket ID'				AS [Update]
			                        ,PEN.slm_Contract_Number										AS ContractNoBDW
							      ,PEN.slm_Policy_No											AS PolicyNoBDW
							      ,PEN.slm_Create_Date											AS PolicyDateBDW
							      ,''											AS ContractNo
							      ,ISNULL(LEAD_TITLE.slm_TitleName,'')+ ISNULL(PRE.slm_Name,'')+' '+ ISNULL(PRE.slm_LastName,'') AS CustomerName
							      ,PRE.slm_Voluntary_Company_Code								AS PolicyInsCode
							      ,'98'															AS Agent
							      ,CONVERT(VARCHAR(10),PRE.slm_Voluntary_Policy_Eff_Date , 103)	AS PolicyStartCoverDate
                                  ,CONVERT(VARCHAR(10),PRE.slm_Voluntary_Policy_Exp_Date, 103)	AS PolicyEndCoverDate
							      ,PRE.slm_Voluntary_Policy_Number								AS PolicyNo
							      ,PRE.slm_Voluntary_Gross_Premium								AS PolicyNetGrossPremium
							      ,NULL															AS PolicyGrossStamp	
							      ,NULL															AS PolicyGrossVat
							      ,NULL															AS PolicyNoDate
							      ,''															AS EmsNo
		                    FROM  kkslm_tr_renewinsurance_hp_policyno_actno_pending	AS PEN 
		                    INNER JOIN kkslm_tr_prelead								AS PRE			ON		PEN.slm_Contract_Number			= PRE.slm_Contract_Number
		                    LEFT JOIN kkslm_ms_title								AS LEAD_TITLE	ON		LEAD_TITLE.slm_TitleId			= PRE.slm_TitleId  
		                    WHERE PEN.is_Deleted	 = 0 
			                    AND PRE.is_Deleted	 = 0 
			                    AND PRE.slm_Assign_Status = '1'
			                    AND PRE.slm_AssignFlag = '1'
                                AND PEN.is_Deleted = 0  AND PEN.slm_Type = 2
						   UNION ALL
                           SELECT  CONVERT(VARCHAR(10), GETDATE(),103) + ' ' +
								    CONVERT(VARCHAR(5), GETDATE(),108)	AS Created
							       ,'ไม่พบข้อมูลใน OBT'	AS [Update]
			                       ,PEN.slm_Contract_Number		AS ContractNoBDW
							      ,PEN.slm_Policy_No			AS PolicyNoBDW
							      ,PEN.slm_Create_Date			AS PolicyDateBDW
							      ,''							AS ContractNo
							      ,''							AS CustomerName
							      ,''							AS PolicyInsCode
							      ,''							AS Agent
							      ,NULL							AS PolicyStartCoverDate
                                  ,NULL							AS PolicyEndCoverDate
							      ,''							AS PolicyNo
							      ,NULL							AS PolicyNetGrossPremium
							      ,NULL							AS PolicyGrossStamp	
							      ,NULL							AS PolicyGrossVat
							      ,NULL							AS PolicyNoDate
							      ,''							AS EmsNo
		                FROM kkslm_tr_renewinsurance_hp_policyno_actno_pending PEN 
		                WHERE PEN.is_Deleted = 0 AND PEN.slm_Type = 5  ";

            return db.ExecuteTable(sql);
        }
    }
}
