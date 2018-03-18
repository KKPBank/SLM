using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;
using SLM.Resource;

namespace SLM.Biz
{
    public class RoleBiz
    {
        public static ScreenPrivilegeData GetScreenPrivilege(string username, string screenDesc)
        {
            KKSlmScreenModel screen = new KKSlmScreenModel();
            return screen.GetScreenPrivilege(username, screenDesc);
        }

        public static bool GetTicketIdPrivilege(string ticketId, string username, decimal? staffType, string staffBranchCode, string screenCode, out string logError)
        {
            logError = "";
            SearchLeadCondition conn = new SearchLeadCondition
            {
                TicketId = ticketId,
                StaffType = staffType,
                StaffBranchCode = staffBranchCode,
                ScreenCode = screenCode
            };

            int totalCount;
            SearchLeadModel search = new SearchLeadModel();
            search.SearchLeadDataNew(conn, username, SLMConstant.SearchOrderBy.None, "", false, out logError, out totalCount, true);
            return totalCount > 0 ? true : false;
        }
    }
}
