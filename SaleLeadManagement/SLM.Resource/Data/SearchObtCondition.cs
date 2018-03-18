using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class SearchObtCondition
    {
        public string PreleadId { get; set; }
        public string ContractNo { get; set; }
        public string TicketId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string CardType { get; set; }
        public string CitizenId { get; set; }
        public string ChannelId { get; set; }
        public string CampaignId { get; set; }
        public string CarLicenseNo { get; set; }
        public string[] Grade { get; set; }
        public DateTime NextContactDateFrom { get; set; }
        public DateTime NextContactDateTo { get; set; }
        public string HasNotifyPremium { get; set; }
        public string NotifyGrossPremiumMin { get; set; }
        public string NotifyGrossPremiumMax { get; set; }
        public string PolicyExpirationYear { get; set; }
        public string PolicyExpirationMonth { get; set; }
        public string PeriodYear { get; set; }
        public string PeriodMonth { get; set; }
        public string ContractNoRefer { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime AssignedDate { get; set; }
        public string OwnerBranch { get; set; }         //Owner Branch
        public string OwnerUsername { get; set; }       //Owner Lead
        public string DelegateBranch { get; set; }      //Delegate Branch
        public string DelegateLead { get; set; }        //Delegate Lead
        public string CreateByBranch { get; set; }      //CreateBy Branch
        public string CreateBy { get; set; }            //CreateBy Lead

        //Flag การทำงาน
        public bool CheckWaitCreditForm { get; set; }
        public bool CheckWait50Tawi { get; set; }
        public bool CheckWaitDriverLicense { get; set; }
        public bool CheckPolicyNo { get; set; }
        public bool CheckAct { get; set; }
        public bool CheckStopClaim { get; set; }
        public bool CheckStopClaim_Cancel { get; set; }

        public bool CheckEditReceipt_Cancel { get; set; }
        public bool CheckEditReceipt_Revise { get; set; }

        public string StatusList { get; set; }
        public string SubStatusList { get; set; }
        public string StatusNameList { get; set; }
        public string SubStatusNameList { get; set; }

        public int PageIndex { get; set; }
        public string SortExpression { get; set; }
        public string SortDirection { get; set; }
        public bool AdvancedSearch { get; set; }

        //ข้อมูลคน Login
        public string StaffTypeId { get; set; }
        public string StaffBranchCode { get; set; }
        public string StaffEmpCode { get; set; }
        public string StaffUsername { get; set; }

        //เพิ่มใน Tab All Task ใช้ในการค้นหาข้อมูล
        public bool IsFollowup { get; set; }
        public bool IsInbound { get; set; }
        public bool IsOutbound { get; set; }

        //ใช้ในการเก็บไว้ใน session กรณี back กลับมาต้อง fill ข้อมูลให้ condition บนหน้าเว็บ
        public bool CheckFollowUp { get; set; }
        public bool CheckInbound { get; set; }
        public bool CheckOutbound { get; set; }

        //Logging
        public string ScreenCode { get; set; }      //เก็บ Code ของ Screen ที่ใช้ Search
        public string CardTypeDesc { get; set; }
        public string CampaignName { get; set; }
        public string PolicyExpirationMonthName { get; set; }
        public string PeriodMonthName { get; set; }
        public string CreatedByBranchName { get; set; }
        public string CreatedByName { get; set; }
        public string CheckWaitCreditFormDesc { get; set; }
        public string CheckWait50TawiDesc { get; set; }
        public string CheckWaitDriverLicenseDesc { get; set; }
        public string CheckPolicyNoDesc { get; set; }
        public string CheckActDesc { get; set; }
        public string CheckStopClaimDesc { get; set; }
        public string CheckStopClaim_CancelDesc { get; set; }
        public string OwnerBranchName { get; set; }
        public string OwnerName { get; set; }
        public string DelegateBranchName { get; set; }
        public string DelegateName { get; set; }
    }
}
