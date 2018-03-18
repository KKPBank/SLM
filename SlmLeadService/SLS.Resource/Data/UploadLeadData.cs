using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource.Data
{
    public class UploadLeadData
    {
        public int UploadDetailId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int? CardTypeId { get; set; }
        public string CitizenId { get; set; }
        public string OwnerUsername { get; set; }
        public string DelegateUsername { get; set; }
        public string TelNo1 { get; set; }
        public string TelNo2 { get; set; }
        public string Detail { get; set; }
        public string StatusDesc { get; set; }
        public string Remark { get; set; }      //สาเหตุที่ fail
    }

    public class UploadLeadSummaryJobz
    {
        public DateTime? AssignDate { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string OwnerEmpCode { get; set; }
        public int Count { get; set; }
    }

    public class UploadLeadSummaryJob
    {
        public int UploadLeadId { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int UploadDetailId { get; set; }
        public DateTime? AssignDate { get; set; }
        public DateTime? AssignDateTime { get; set; }
        public string OwnerEmpCode { get; set; }
    }
}
