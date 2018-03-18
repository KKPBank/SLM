using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ReceiveNoData
    {

        //**************************************TableName : kkslm_ms_receiveno **********************************************
        public decimal? ReceiveNoId { get; set; }
        public string ProductId { get; set; }
        public decimal? InsComId { get; set; }
        public int? Lot { get; set; }
        public string CodeName { get; set; }
        public decimal? ReceiveNoStart { get; set; }
        public decimal? ReceiveNoEnd { get; set; }
        public decimal? ReceiveNoTotal { get; set; }
        public decimal? ReceiveNoUsed { get; set; }
        public decimal? ReceiveNoCancel { get; set; }
        public decimal? ReceiveNoRemain { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }

        // additional data
        public string InsComName { get; set; }
        public string InsComCode { get; set; }
        public string CreaterName { get; set; }
        public string UpdaterName { get; set; }
        public string ProductName { get; set; }
    }

}
