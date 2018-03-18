using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class SlmProblemData
    {
        public decimal ProblemId { get; set; }
        public decimal? Ins_Com_Id { get; set; }
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool Deleted { get; set; }
        public List<SlmProblemDetailData> ProblemDetails { get; set; }
    }

    public class SlmProblemDetailData
    {
        public decimal ProblemDetailId { get; set; }
        public decimal? ProblemId { get; set; }
        public DateTime? ProblemDate { get; set; }
        public decimal? Ins_Com_Id { get; set; }
        public string InsType { get; set; }
        public string Contract_Number { get; set; }
        public string Name { get; set; }
        public string TelesaleName { get; set; }
        public string Owner { get; set; }
        public string ProblemDetail { get; set; }
        public string CauseDetail { get; set; }
        public string HeadStaff { get; set; }
        public string ResponseDetail { get; set; }
        public string PhoneContact { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string Remark { get; set; }
        public bool IsAction { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        bool is_Deleted { get; set; }

        // for validation input
        public string InsComName { get; set; }
        public string ProblemDateString { get; set; }
        public string ResponseDateString { get; set; }
        public bool hasError { get; set; }
        public string ErrorMessage { get; set; }
        public string TicketId { get; set; }

        // for CAR service 
        public bool needCARprocess { get; set; }
        public decimal? preleadId { get; set; }
        public decimal RenewInsureId {get; set;}
        public string ReceiveNo { get; set; }
        public DateTime? ActSendDate { get; set; }
        public bool? IncentiveFlag { get; set; }
        public decimal? PolicyGrossPremiumTotal { get; set; }
        public bool? ActIncentiveFlag { get; set; }
        public decimal? ActGrossPremium { get; set; }
        public decimal? PolicyRecAmt { get; set; }
        public decimal? ActRecAmt { get; set; }
        public decimal? PolicyLastRecAmt { get; set; }
        public decimal? ActLastRecAmt { get; set; }
        public bool IsPolicyPurchase { get; set; }
        public bool IsActPurchase { get; set; }
    }

    [Serializable]
    public class ProblemDetailData
    {
        public decimal slm_ProblemDetailId { get; set; }
        public decimal? slm_ProblemId { get; set; }
        public DateTime? slm_ProblemDate { get; set; }
        public decimal? slm_Ins_Com_Id { get; set; }
        public string slm_InsNameTh { get; set; }
        public string slm_InsType { get; set; }
        public string slm_Contract_Number { get; set; }
        public string slm_Name { get; set; }
        public string slm_TelesaleName { get; set; }
        public string slm_Owner { get; set; }
        public string slm_ProblemDetail { get; set; }
        public string slm_CauseDetail { get; set; }
        public string slm_HeadStaff { get; set; }
        public string slm_ResponseDetail { get; set; }
        public string slm_PhoneContact { get; set; }
        public DateTime? slm_ResponseDate { get; set; }
        public string slm_Remark { get; set; }
        public bool slm_IsAction { get; set; }
        public DateTime? slm_CreatedDate { get; set; }
        public string slm_CreatedBy { get; set; }
        public DateTime? slm_UpdatedDate { get; set; }
        public string slm_UpdatedBy { get; set; }
        bool is_Deleted { get; set; }

        public string slm_FixTypeFlag { get; set; }
        public bool? slm_Export_Flag { get; set; }

        // for validation input
        public string InsComName { get; set; }
        public string ProblemDateString { get; set; }
        public string ResponseDateString { get; set; }
        public bool hasError { get; set; }
        public string ErrorMessage { get; set; }

        //public string InsComName;
        //public string ProblemDateString;
        //public string ResponseDateString;
        //public bool hasError = false;
        //public string ErrorMessage { get; set; }
    }

    //public class SlmSearchResultData {
    //    public int RowId { get; set; }
    //    public string InsComName { get; set; }
    //    public string FileName { get; set; }
    //    public int TotalCase { get; set; }

    //}
}
