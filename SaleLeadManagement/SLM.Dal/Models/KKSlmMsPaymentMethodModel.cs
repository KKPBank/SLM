using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsPaymentMethodModel
    {
        /// <summary>
        /// GetPositionList Flag 1=Active Branch, 2=Inactive Branch, 3=All
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<ControlListData> GetPaymentMethodList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var list = slmdb.kkslm_ms_payment_methods.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_PayMethodNameTH).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_PayMethodNameTH, ValueField = p.slm_PayMethodId.ToString() }).ToList();
                return list;
            }
        }

//        public PaymentData GetResultActBuy(int ticketId) {
 
//            string sql=@"SELECT SUM(A.CNT) AS RESULT
//                            FROM(
//                            SELECT COUNT(*) AS CNT FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare 
//                            WHERE slm_Selected = 1 and slm_RenewInsureId = 
//                            (	SELECT	slm_RenewInsureId 
//	                            FROM	" + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance 
//	                            WHERE slm_ticketId = "+ticketId+" )UNION ALL"+
//                          @"SELECT COUNT(*) AS CNT  FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare_act  
//                            WHERE slm_ActPurchaseFlag = 1 and slm_RenewInsureId = 
//                            (	SELECT	slm_RenewInsureId 
//	                            FROM	" + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance 
//	                            WHERE slm_ticketId = "+ticketId+")) AS A";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).FirstOrDefault(); 
//        }

//        public PaymentData GetTotalPay(int ticketId)
//        {

//            string sql = @"select SUM(slm_RecAmount) AS ReAmount from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail 
//                            where slm_PaymentCode = '204' and renewinsurance_paymenttransmain  = 
//                            (select MAX(slm_RenewInsuranceReceiptId) from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt  where slm_ticketId =" + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).FirstOrDefault();
//        }

//        public PaymentData GetDiscountPercentInConfig(string username)
//        {

//            string sql = @"select disc.slm_DiscountPercent AS DiscountPercent
//                            from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff left join kkslm_ms_discount disc on
//                            staff.slm_StaffTypeId = disc.slm_StaffTypeId 
//                            where staff.is_Deleted = 0 and  slm_UserName ='" + username + "'";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).FirstOrDefault();
//        }

//        public List<PaymentData> GetDataDifference(int ticketId) {

//            string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymenttransmain where slm_RenewInsureId = 
//                            (select slm_RenewInsureId from kkslm_tr_renewinsurance where slm_TicketId = " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public List<PaymentData> HaveDataDifference(int ticketId)
//        {

//            string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymenttransmain where slm_RenewInsureId = 
//                            (select slm_RenewInsureId from kkslm_tr_renewinsurance where slm_TicketId = " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public List<PaymentData> NotHaveDataDifference(int ticketId)
//        {

//            string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance  where slm_type= '1' and slm_RenewInsureId = 
//                            (select slm_RenewInsureId from kkslm_tr_renewinsurance where slm_TicketId = " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public List<PaymentData> GetDataMajor(int ticketId)
//        {

//            string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymenttrans where slm_type= '1' and slm_RenewInsureId = 
//                           (select slm_RenewInsureId from kkslm_tr_renewinsurance where slm_TicketId = " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public List<PaymentData> HaveDataMajor(int ticketId)
//        {

//            string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymenttrans where slm_type= '1' and slm_RenewInsureId = 
//                            (select slm_RenewInsureId from kkslm_tr_renewinsurance where slm_TicketId =  " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public List<PaymentData> NotHaveDataMajor(int ticketId)
//        {

//            string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymentmain where slm_type= '1' and slm_RenewInsureId = 
//                            (select slm_RenewInsureId from kkslm_tr_renewinsurance where slm_TicketId = " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public PaymentData GetTotalPayAct(int ticketId)
//        {

//            string sql = @"select SUM(slm_RecAmount) AS ReAmount from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail 
//                            where slm_PaymentCode = '205' and slm_RenewInsuranceReceiptId = 
//                            (select MAX(slm_RenewInsuranceReceiptId) from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt  where slm_ticketId = " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).FirstOrDefault();
//        }

//        public List<PaymentData> GetDataDifferenceAct(int ticketId)
//        {

//            string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymenttransmain where slm_RenewInsureId = 
//                            (select slm_RenewInsureId from kkslm_tr_renewinsurance where slm_TicketId =  " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public List<PaymentData> HaveDataDifferenceAct(int ticketId)
//        {

//            string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_paymenttransmain where slm_RenewInsureId = 
//                            (select slm_RenewInsureId from kkslm_tr_renewinsurance where slm_TicketId = " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public List<PaymentData> NotHaveDataDifferenceAct(int ticketId)
//        {

//            string sql = @"SELECT * FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance  where slm_RenewInsureId = 
//                            (select slm_RenewInsureId from kkslm_tr_renewinsurance where slm_TicketId = " + ticketId + ")";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public List<PaymentData> GetReceiptDetail(int ticketId)
//        {

//            string sql = @"SELECT rr.slm_RecNo, sum(rrd.slm_RecAmount) as total, rr.slm_Status As slm_Status
//                              FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_Tr_lead lead 
//                              inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt rr on rr.slm_ticketId = lead.slm_ticketId
//                              left join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail rrd on rrd.slm_RenewInsuranceReceiptId = rr.slm_RenewInsuranceReceiptId
//                              where lead.is_Deleted = 0 and lead.slm_ticketId = " + ticketId + "group by rr.slm_RecNo,rrd.slm_Statusorder by rr.slm_CreatedDate  desc ";

//            return slmdb.ExecuteStoreQuery<PaymentData>(sql).ToList();
//        }

//        public List<ReceiptData> ApplyByReceiptNo(int recieiptNo)
//        {

//            string sql = @"SELECT slm_PaymentCode,slm_InsNoDesc,slm_InstNo,slm_RecNo,slm_RecAmount,slm_TransDate
//                                   , slm_CreatedDate, slm_RecBy 
//                            FROM   " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail
//                            WHERE  slm_RenewInsuranceReceiptId = " + recieiptNo + " ";

//            return slmdb.ExecuteStoreQuery<ReceiptData>(sql).ToList();
//        }

//        public List<ReceiptData> EditReceiptDetail()
//        {

//            string sql = @"SELECT CASE WHEN slm_PaymentCode = '204' THEN 1 
//			                              WHEN slm_PaymentCode = '205' THEN 2 
//			                              WHEN slm_PaymentCode = '0HP' THEN 3 
//			                              WHEN slm_PaymentCode = '101' THEN 4 
//			                              WHEN slm_PaymentCode = '614' THEN 5 
//			                              WHEN slm_PaymentCode = 'OTHER' THEN 6 ELSE 9999 END SEQ
//		                            ,SLM_PAYMENTDESC	  
//                            FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_payment
//                            WHERE slm_PaymentCode IN ( '204','205','0HP','101','614','OTHER')
//                            ORDER BY CASE WHEN slm_PaymentCode = '204' THEN 1 
//			                              WHEN slm_PaymentCode = '205' THEN 2 
//			                              WHEN slm_PaymentCode = '0HP' THEN 3 
//			                              WHEN slm_PaymentCode = '101' THEN 4 
//			                              WHEN slm_PaymentCode = '614' THEN 5 
//			                              WHEN slm_PaymentCode = 'OTHER' THEN 6 ELSE 9999 END ASC
//";

//            return slmdb.ExecuteStoreQuery<ReceiptData>(sql).ToList();
//        }
    }
}
