using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class RenewPhoneCallHistoryData
    {
        public string ticketId { get; set; }
        public string cardType { get; set; }
        public string cardId { get; set; }
        public string countryId { get; set; }
        public string leadStatusCode { get; set; }
        public string oldstatus { get; set; }
        public string ownerBranch { get; set; }
        public string owner { get; set; }
        public string oldOwner { get; set; }
        public string delegateLeadBranch { get; set; }
        public string delegateLead { get; set; }
        public string oldDelegateLead { get; set; }
        public string contactPhone { get; set; }
        public string contactDetail { get; set; }
        public string createBy { get; set; }
        public string subStatusCode { get; set; }
        public string subStatusDesc { get; set; }
        public bool Need_CompulsoryFlag { get; set; }
        public bool Need_PolicyFlag { get; set; }
        public bool Need_CompulsoryFlagOld { get; set; }
        public bool Need_PolicyFlagOld { get; set; }
        public string slm_CreditFileName { get; set; }
        public string slm_CreditFilePath { get; set; }
        public int? slm_CreditFileSize { get; set; }
        public string slm_50TawiFileName { get; set; }
        public string slm_50TawiFilePath { get; set; }
        public int? slm_50TawiFileSize { get; set; }
        public string slm_DriverLicenseiFileName { get; set; }
        public string slm_DriverLicenseFilePath { get; set; }
        public int slm_RepairTypeId { get; set; }
        public int? slm_DriverLicenseFileSize { get; set; }
        public DateTime? slm_NextContactDate { get; set; }
        public decimal? slm_cp_blacklist_id { get; set; }
        public DateTime? slm_ActStartCoverDate { get; set; }
        public DateTime? slm_ActEndCoverDate { set; get; }
        public string Remark { get; set; }
        public bool Blacklist { get; set; }
        public DateTime createdDate { get; set; }
        public string externalSubStatusDesc { get; set; }
        public AddressData Address { get; set; }
        public string TelNoSms { get; set; }
    }
   
}
