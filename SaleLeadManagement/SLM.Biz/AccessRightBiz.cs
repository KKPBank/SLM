using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class AccessRightBiz
    {
        public static void InsertData(string productId, string campaignId, int staffTypeId, List<string> branchCodeList, string createdBy)
        {
            KKSlmMsAccessRightModel right = new KKSlmMsAccessRightModel();
            right.InsertData(productId, campaignId, staffTypeId, branchCodeList, createdBy);
        }

        public static void UpdateData(string productId, string campaignId, int staffTypeId, List<string> branchCodeList, string createdBy)
        {
            KKSlmMsAccessRightModel right = new KKSlmMsAccessRightModel();
            right.UpdateData(productId, campaignId, staffTypeId, branchCodeList, createdBy);
        }

        public static List<AccessRightData> SearchAccessRight(string productId, string campaignId, string staffTypeId, string branchCode)
        {
            KKSlmMsAccessRightModel right = new KKSlmMsAccessRightModel();
            return right.SearchAccessRight(productId, campaignId, staffTypeId, branchCode);
        }
    }
}
