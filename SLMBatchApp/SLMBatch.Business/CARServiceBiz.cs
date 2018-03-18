using SLMBatch.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Business
{
    public class CARServiceBiz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        public bool SaveCarResend(CASResendData rs, int maxResendCount)
        {
            bool ret = true;
            try
            {
                using (SLMDBEntities sdc = AppUtil.GetSlmDbEntities())
                {
                    var rsu = new kkslm_tr_cas_resend();

                    rsu.slm_CasReferenceNo = rs.slm_CasReferenceNo;
                    rsu.slm_CasSystemCode = rs.slm_CasSystemCode;
                    rsu.slm_CasServiceName = rs.slm_CasServiceName;
                    rsu.slm_CasLogType = rs.slm_CasLogType;
                    rsu.slm_CasResponse = rs.slm_CasResponse;
                    rsu.slm_CasErrorMessage = rs.slm_CasErrorMessage;
                    rsu.slm_CasDetail = rs.slm_CasDetail;
                    rsu.slm_LogStatus = "R";
                    rsu.slm_LogDate = DateTime.Now;
                    rsu.slm_ResendCount = 0;
                    rsu.slm_CreatedBy = rs.slm_CreatedBy;
                    rsu.slm_CreatedDate = DateTime.Now;
                    rsu.slm_UpdatedBy = rs.slm_UpdatedBy;
                    rsu.slm_UpdatedDate = DateTime.Now;

                    sdc.kkslm_tr_cas_resend.AddObject(rsu);
                    sdc.SaveChanges();

                    #region backup
                    //var rsu = sdc.kkslm_tr_cas_resend.Where(r => r.slm_CasReferenceNo == rs.slm_CasReferenceNo).FirstOrDefault();
                    //if (rsu == null)
                    //{
                    //    // new just add

                    //    rsu = new kkslm_tr_cas_resend();

                    //    rsu.slm_CasReferenceNo = rs.slm_CasReferenceNo;
                    //    rsu.slm_CasSystemCode = rs.slm_CasSystemCode;
                    //    rsu.slm_CasServiceName = rs.slm_CasServiceName;
                    //    rsu.slm_CasLogType = rs.slm_CasLogType;
                    //    rsu.slm_CasResponse = rs.slm_CasResponse;
                    //    rsu.slm_CasErrorMessage = rs.slm_CasErrorMessage;
                    //    rsu.slm_CasDetail = rs.slm_CasDetail;
                    //    rsu.slm_LogStatus = "R";
                    //    rsu.slm_LogDate = DateTime.Now;
                    //    rsu.slm_ResendCount = 0;
                    //    rsu.slm_CreatedBy = rs.slm_CreatedBy;
                    //    rsu.slm_CreatedDate = DateTime.Now;
                    //    rsu.slm_UpdatedBy = rs.slm_UpdatedBy;
                    //    rsu.slm_UpdatedDate = DateTime.Now;

                    //    sdc.kkslm_tr_cas_resend.AddObject(rsu);
                    //}
                    //else
                    //{
                    //    // update, from batch
                    //    rsu.slm_CasSystemCode = rs.slm_CasSystemCode;
                    //    rsu.slm_CasServiceName = rs.slm_CasServiceName;
                    //    rsu.slm_UpdatedBy = rs.slm_CasSystemCode + "CARBatch";
                    //    rsu.slm_UpdatedDate = DateTime.Now;
                    //    rsu.slm_ResendCount++;

                    //    if (rsu.slm_ResendCount >= maxResendCount) rsu.slm_LogStatus = "E";

                    //}
                    //sdc.SaveChanges();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ret = false;
            }
            return ret;
        }

        public partial class CASResendData : kkslm_tr_cas_resend { }
    }
}
