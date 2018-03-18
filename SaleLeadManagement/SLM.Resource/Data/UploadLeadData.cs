using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class UploadLeadData
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string CardTypeDesc { get; set; }
        public string CitizenId { get; set; }
        public string OwnerEmpCode { get; set; }
        public string DelegateEmpCode { get; set; }
        public string TelNo1 { get; set; }
        public string TelNo2 { get; set; }
        public string Detail { get; set; }
        public string StatusDesc { get; set; }
        public string Remark { get; set; }
        public string TicketId { get; set; }
    }

    public class UploadLeadDataError
    {
        public string Row { get; set; }
        public string ErrorDetail { get; set; }
    }

    public class UploadAllData
    {
        public UploadAllData()
        {
            LeadDataList = new List<UploadLeadData>();
        }
        public string ChannelId { get; set; }
        public string CampaignId { get; set; }
        public string UploadFileName { get; set; }
        public List<UploadLeadData> LeadDataList { get; set; }
        public string CreateByUsername { get; set; }
    }

    public class UploadFileInfo
    {
        public int UploadLeadId { get; set; }
        public string Filename { get; set; }
        public int? LeadCount { get; set; }
        public DateTime? LastestUploadDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string UploaderName { get; set; }
        public string FileStatus { get; set; }
    }

    public class UploadLeadSearchCondition
    {
        public int PageIndex { get; set; }
        public string Filename { get; set; }
        public string StatusDesc { get; set; }
    }
}
