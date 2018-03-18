using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class PhoneCallHistoryData
    {
        public int PhoneCallId { get; set; }
        public string No { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TicketId { get; set; }
        public string CardTypeDesc { get; set; }
        public string CitizenId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int? CountryId { get; set; }
        public string StatusDesc { get; set; }
        public string ContactPhone { get; set; }
        public string OwnerName { get; set; }
        public string ContactDetail { get; set; }
        public string CampaignName { get; set; }
        public string CreatedName { get; set; }
        public string CreditFilePath { get; set; }
        public string Tawi50FilePath { get; set; }
        public string DriverLicenseFilePath { get; set; }
        public string CreditFileName { get; set; }
        public string Tawi50FileName { get; set; }
        public string DriverLicenseFileName { get; set; }
        public string QuotationFilePath { get; set; }
        public string QuotationFileName { get; set; }
    }

    public class preleadPhoneCallHistoryData
    {
        public decimal PhoneCallId { get; set; }
        public string PreleadId { get; set; }
    }
}
