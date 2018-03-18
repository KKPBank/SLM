using SLMBatch.Common;

namespace SLMBatch.Data
{
    public class OBT_PRO_33_DataAccess : OBT_PRO_32_DataAccess
    {
        public OBT_PRO_33_DataAccess()
        {
            SMSNotiDay = AppConstant.SMSPaymentDueShortNotiDay;
            FieldFlag = "slm_IsSMS8";
            SqlCommand = @"SELECT	slm_Car_License_No as LicenseNo, CONVERT(DATE,slm_Voluntary_Policy_Exp_Date) AS ExpiredDate, null as TicketId,
                                    CASE WHEN PRE.slm_TelNoSms IS NOT NULL AND LTRIM(RTRIM(PRE.slm_TelNoSms)) <> '' THEN PRE.slm_TelNoSms ELSE PREA.slm_Mobile_Phone END AS TelNo
		                            , PRE.slm_Prelead_Id AS PreleadId 
                            FROM {0}kkslm_tr_prelead PRE 
                            INNER JOIN {0}kkslm_tr_prelead_address PREA ON PRE.slm_Prelead_Id = PREA.slm_Prelead_Id AND PREA.slm_Address_Type = 'C' 
                            WHERE slm_AssignFlag = '1' AND slm_Assign_Status = '1' AND PRE.is_Deleted = 0 AND PRE.{2} IS NULL 
	                            AND slm_SubStatus NOT IN ('07','06')
	                            AND slm_TicketId IS NULL
	                            AND slm_Voluntary_Policy_Exp_Date IS NOT NULL 
                                AND 
                                (
                                   slm_Voluntary_Policy_Exp_Date >= dateadd(DAY, {1}, cast(getdate() AS DATE)) AND
                                   slm_Voluntary_Policy_Exp_Date < dateadd(DAY, {1} + 1, cast(getdate() AS DATE))
                                ) AND
                                1 = 1
                            UNION ALL
                            SELECT REN.slm_LicenseNo AS LicenseNo, CONVERT(DATE,slm_Voluntary_Policy_Exp_Date) AS ExpiredDate, ren.slm_TicketId AS TicketId,
                                   CASE WHEN CUS.slm_TelNoSms IS NOT NULL AND LTRIM(RTRIM(CUS.slm_TelNoSms)) <> '' THEN CUS.slm_TelNoSms ELSE lead.slm_TelNo_1 END AS TelNo
		                            , PRE.slm_Prelead_Id AS PreleadId 
                            FROM {0}kkslm_tr_prelead PRE 
                            INNER JOIN {0}kkslm_tr_lead LEAD ON PRE.slm_TicketId = LEAD.slm_ticketId 
                            INNER JOIN {0}kkslm_tr_cusinfo CUS ON LEAD.slm_ticketId = CUS.slm_TicketId 
	                        INNER JOIN {0}kkslm_tr_renewinsurance REN ON REN.slm_TicketId = LEAD.slm_ticketId 
                            WHERE slm_AssignFlag = '1' AND slm_Assign_Status = '1' AND LEAD.is_Deleted = 0 AND PRE.is_Deleted = 0 AND PRE.{2} IS NULL 
	                            AND LEAD.slm_Status NOT IN ('08','09','10')
	                            AND	NOT EXISTS
                                (
                                    SELECT RR.slm_ticketId
                                    FROM kkslm_tr_renewinsurance_receipt RR
                                    WHERE RR.slm_TicketId = REN.slm_TicketId
                                ) 
                                AND slm_Voluntary_Policy_Exp_Date IS NOT NULL 
                                AND 
                                (
                                   slm_Voluntary_Policy_Exp_Date >= dateadd(DAY, {1}, cast(getdate() AS DATE)) AND
                                   slm_Voluntary_Policy_Exp_Date < dateadd(DAY, {1} + 1, cast(getdate() AS DATE))
                                ) AND
                                1 = 1 ";
        }
    }
}
