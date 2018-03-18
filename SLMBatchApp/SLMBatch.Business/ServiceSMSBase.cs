using System;
using SLMBatch.Data;

namespace SLMBatch.Business
{
    public class ServiceSMSBase : ServiceBase, IDisposable
    {
        //protected static kkslm_tr_prelead_prepare_sms InitSMS(string smsMessage, string smsPhoneNo, string refTicketId, DateTime dbNow, decimal? preleadId = null)
        //{
        //    var sms = new kkslm_tr_prelead_prepare_sms
        //    {
        //        slm_ticketId = refTicketId,
        //        slm_Prelead_Id = preleadId,
        //        slm_Message = smsMessage,
        //        slm_Message_CreatedBy = "SYSTEM",
        //        slm_MessageStatusId = "1",
        //        slm_PhoneNumber = smsPhoneNo,
        //        slm_QueueId = "6",
        //        slm_RequestDate = dbNow,
        //        slm_RuleActivityId = 0,
        //        slm_ExportStatus = "0",
        //        slm_RefId = null,
        //        slm_SendingStatusCode = null,
        //        slm_SendingStatus = null,
        //        slm_ErrorCode = null,
        //        slm_ErrorReason = null,
        //        slm_CAS_Flag = null,
        //        slm_CAS_Date = null,
        //        slm_FlagType = "1",
        //        slm_CreatedBy = "SYSTEM",
        //        slm_CreatedDate = dbNow,
        //        slm_UpdatedBy = "SYSTEM",
        //        slm_UpdatedDate = dbNow,
        //        is_Deleted = 0
        //    };
        //    return sms;
        //}

        #region "IDisposable"

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
