using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class CalendarBranchBiz
    {
        public static void InsertData(DateTime holidayDate, string holidayDesc, List<string> branchCodeList, string createdBy)
        {
            KKSlmMsCalendarBranchModel holiday = new KKSlmMsCalendarBranchModel();
            holiday.InsertData(holidayDate, holidayDesc, branchCodeList, createdBy);
        }

        public static void UpdateData(DateTime holidayDate, string holidayDesc, List<string> branchCodeList, string createdBy)
        {
            KKSlmMsCalendarBranchModel holiday = new KKSlmMsCalendarBranchModel();
            holiday.UpdateData(holidayDate, holidayDesc, branchCodeList, createdBy);
        }

        public static List<CalendarBranchData> SearchCalendarBranch(string holidayDate, string holidayDesc, string branchCode)
        {
            KKSlmMsCalendarBranchModel holiday = new KKSlmMsCalendarBranchModel();
            return holiday.SearchCalendarBranch(holidayDate, holidayDesc, branchCode);
        }
    }
}
