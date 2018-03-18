using SLMBatch.Common;
using SLMBatch.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SLMBatch.Data
{
    public class SLM_PRO_06_DataAccess : IDisposable
    {
        private SLMDBEntities _DataAccess = AppUtil.GetSlmDbEntities();

        public DataTable LoadWSLogData(string system)
        {
            var data = _DataAccess.kkslm_ws_log.Where(p => !p.slm_ResponseDesc.Contains("Success") &&
                                                        p.slm_InputXml.Contains("<system>" + system + "</system>") &&
                                                        p.slm_OperationDate > DateTime.Today).Select(
                ws => new WSLogData()
                {
                    slm_ws_LogId = ws.slm_ws_LogId,
                    slm_OperationDate = ws.slm_OperationDate,
                    slm_Operation = ws.slm_Operation,
                    slm_ChannelId = ws.slm_ChannelId,
                    slm_InputXml = ws.slm_InputXml,
                    slm_OutputXml = ws.slm_OutputXml,
                    slm_TicketId = ws.slm_TicketId,
                    slm_ResponseCode = ws.slm_ResponseCode,
                    slm_ResponseDesc = ws.slm_ResponseDesc,
                    slm_ResponseDate = ws.slm_ResponseDate,
                    slm_ResponseTime = ws.slm_ResponseTime,
                    slm_CauseError = ws.slm_CauseError
                });
            return Util.ToDataTable(data.ToList());
        }

        #region "IDisposable"
        private bool _disposed = false;
        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _DataAccess = null;
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
