using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    [Serializable]
    public class LeadControlCommonData
    {
        public string TicketID { get; set; }
        public string TicketIDRefer { get; set; }
        public int TitleID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string ChannelID { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string CampaignID { get; set; }
        public string Branch { get; set; }
        public string Owner { get; set; }
        public string OldOwner { get; set; }
        public bool ActDelegate { get; set; }
        public bool ActOwner { get; set; }
        public string DelegateBranch { get; set; }
        public string DelegateLead { get; set; }
        public string DelegateFlag { get; set; }    //add
        public string OldDelegateLead { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Type2 { get; set; }
    

        //---- result
        public string R_TicketID { get; set; }
        public string R_CampaignName { get; set; }
        public string R_ChannelName { get; set; }
        public string R_Owner { get; set; }
        public string R_Message { get; set; }
        public bool R_HasAdams { get; set; }

        public string OldOwner2 { get; set; }
    }
}
