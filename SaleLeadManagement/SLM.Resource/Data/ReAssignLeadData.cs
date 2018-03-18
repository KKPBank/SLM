using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class ReAssignLeadData
    {
        public enum State
        {
            Prelead = 0,
            Lead
        }
        public string RowNo { get; set; }
        public string TeamTelesalesCode { get; set; }
        public decimal? TeamTelesalesId { get; set; }
        public int? StaffId { get; set; }
        public string EmpCode { get; set; }
        public string Username { get; set; }
        public int? OwnerPositionId { get; set; }
        public string OwnerBranchCode { get; set; }
        public string TicketId { get; set; }
        public string ContractNo { get; set; }
        public decimal? PreleadId { get; set; }
        public State LeadState { get; set; }
    }

    public class ReAssignValidateDataError
    {
        public string RowNo { get; set; }
        public string ErrorMessage { get; set; }
    }
}
