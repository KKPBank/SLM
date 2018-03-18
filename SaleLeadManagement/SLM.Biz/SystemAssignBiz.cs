using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class SystemAssignBiz
    {
        public static void InsertData(string productId, string campaignId, int staffTypeId, List<string> branchCodeList, string createdBy)
        {
            KKSlmMsSystemAssignModel assign = new KKSlmMsSystemAssignModel();
            assign.InsertData(productId, campaignId, staffTypeId, branchCodeList, createdBy);
        }

        public static void UpdateData(string productId, string campaignId, int staffTypeId, List<string> branchCodeList, string createdBy)
        {
            KKSlmMsSystemAssignModel assign = new KKSlmMsSystemAssignModel();
            assign.UpdateData(productId, campaignId, staffTypeId, branchCodeList, createdBy);
        }

        public static List<SystemAssignData> SearchSystemAssignConfig(string productId, string campaignId, string staffTypeId, string branchCode)
        {
            KKSlmMsSystemAssignModel assign = new KKSlmMsSystemAssignModel();
            return assign.SearchSystemAssignConfig(productId, campaignId, staffTypeId, branchCode);
        }
    }
}
