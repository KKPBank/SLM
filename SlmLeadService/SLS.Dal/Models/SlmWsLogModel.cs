using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Dal.Models
{
    public class SlmWsLogModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmWsLogModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public void InsertLogData(string ticketId, string operation, DateTime operationDate, string channelId, string username, string inputXml, string outputXml
            , string responseCode, string responseDesc, DateTime responseDate, string responseTime, string causeError)
        {
            try
            {
                kkslm_ws_log log = new kkslm_ws_log()
                {
                    slm_TicketId = ticketId != "" ? ticketId : null,
                    slm_Operation = operation,
                    slm_OperationDate = operationDate,
                    slm_ChannelId = string.IsNullOrEmpty(channelId) ? null : channelId,
                    slm_Username = string.IsNullOrEmpty(username) ? null : username,
                    slm_InputXml = string.IsNullOrEmpty(inputXml) ? null : inputXml,
                    slm_OutputXml = outputXml,
                    slm_ResponseCode = responseCode,
                    slm_ResponseDesc = responseDesc,
                    slm_ResponseDate = responseDate,
                    slm_ResponseTime = responseTime,
                    slm_CauseError = string.IsNullOrEmpty(causeError) ? null : causeError
                };

                slmdb.kkslm_ws_log.AddObject(log);
                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
