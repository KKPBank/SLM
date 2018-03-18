using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class HistoryLeadBiz
    {
        public static List<RenewInsuranceData> GetHistoryMain(string ticketId)
        {
            HistoryLeadModel lead = new HistoryLeadModel();
            return lead.GetHistoryMain(ticketId);
        }
        public static List<PreleadCompareData> GetHistoryDetailPolicy(string RenewInsureId, string version)
        {
            HistoryLeadModel lead = new HistoryLeadModel();
            return lead.GetHistoryDetailPolicy(RenewInsureId,version);
        }
        public static List<PreleadCompareActData> GetHistoryDetailAct(string RenewInsureId, string version)
        {
            HistoryLeadModel lead = new HistoryLeadModel();
            return lead.GetHistoryDetailAct(RenewInsureId,version);
        }
        public static RenewInsuranceData GetHistoryRenewInsurance(string RenewInsureId, string version)
        {
            HistoryLeadModel lead = new HistoryLeadModel();
            return lead.GetHistoryRenewInsurance(RenewInsureId, version);
        }
    }
}
