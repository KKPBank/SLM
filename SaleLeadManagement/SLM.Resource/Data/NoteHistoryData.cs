using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class NoteHistoryData
    {
        public string No { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TicketId { get; set; }
        public string CreateBy { get; set; }
        public string NoteDetail { get; set; }
        public string EmailSubject { get; set; }
        public bool? SendEmailFlag { get; set; }
        public string ContractNo { get; set; }          //Add By Pom 23/03/2016
    }

    public class NoteTabData
    {
        public string TicketId { get; set;}
        public string ISCOC { get; set;}
        public string COCCurrentTeam { get; set;}
        public string ContractNo { get; set;}
        public decimal? PreleadId { get; set;}
        public string Name { get; set;}
        public string LastName { get; set;}
        public string OwnerName { get; set;}
        public string CampaignName { get; set;}
        public string TelNo_1 { get; set;}
        public string Ext_1 { get; set;}
        public string NoteFlag { get; set;}
    }
}
