using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
{
    public class BatchCARInsertStatusData
    {
        ////Header
        //public string ReferenceCode { get; set; }
        //public string FileName { get; set; }
        //public string CreateDate { get; set; }
        //public int? CurrentSequence { get; set; }
        //public int? TotalSequence { get; set; }
        //public int? TotalRecord { get; set; }
        //public string SystemCode { get; set; }

        public BatchCARInsertStatusHeaderData HeaderData { get; set; }


        //Detail
        public string ReferenceNo { get; set; }
        public string ChannelID { get; set; }
        public string StatusDateTime { get; set; }
        //public DateTime StatusDateTimeValue
        //{
        //    get {
        //        return SLMBatch. (StatusDateTime);
        //    }
        //}
        public string SubscriptionID { get; set; }
        public string SubscriptionCusType { get; set; }
        public string SubscriptionCardType { get; set; }
        public string OwnerSystemId { get; set; }
        public string OwnerSystemCode { get; set; }
        public string RefSystemId { get; set; }
        public string RefSystemCode { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
    }
    public class BatchCARInsertStatusHeaderData
    {
        public string ReferenceCode { get; set; }
        public string FileName { get; set; }
        public string CreateDate { get; set; }
        public int? CurrentSequence { get; set; }
        public int? TotalSequence { get; set; }
        public int? TotalRecord { get; set; }
        public string SystemCode { get; set; }
    }

    //public class BatchCARInsertStatusDetailData
    //{
    //    public string ReferenceNo { get; set; }
    //    public string ChannelID { get; set; }
    //    public string StatusDateTime { get; set; }
    //    public string SubscriptionID { get; set; }
    //    public string SubscriptionCusType { get; set; }
    //    public string SubscriptionCardType { get; set; }
    //    public string OwnerSystemId { get; set; }
    //    public string OwnerSystemCode { get; set; }
    //    public string RefSystemId { get; set; }
    //    public string RefSystemCode { get; set; }
    //    public string Status { get; set; }
    //    public string SystusName { get; set; }
    //}

    public class ValidateTextfileData
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
