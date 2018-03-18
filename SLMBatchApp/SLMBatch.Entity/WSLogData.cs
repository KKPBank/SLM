using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
{
    public class WSLogData
    {
        public int slm_ws_LogId { get; set; }
        public DateTime? slm_OperationDate { get; set; }
        public string slm_Operation { get; set; }
        public string slm_ChannelId { get; set; }
        public string slm_Username { get; set; }
        public string slm_InputXml { get; set; }
        public string slm_OutputXml { get; set; }
        public string slm_TicketId { get; set; }
        public string slm_ResponseCode { get; set; }
        public string slm_ResponseDesc { get; set; }
        public DateTime? slm_ResponseDate { get; set; }
        public string slm_ResponseTime { get; set; }
        public string slm_CauseError { get; set; }

    }
}
