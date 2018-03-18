using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class ActivityConfigBiz
    {
        public static void InsertData(string productId, string leadStatusCode, bool rightAdd, List<string> availableStatusList, string createdBy)
        {
            KKSlmMsConfigActivityModel config = new KKSlmMsConfigActivityModel();
            config.InsertData(productId, leadStatusCode, rightAdd, availableStatusList, createdBy);
        }

        public static void UpdateData(string productId, string leadStatusCode, bool rightAdd, List<string> availableStatusList, string createdBy)
        {
            KKSlmMsConfigActivityModel config = new KKSlmMsConfigActivityModel();
            config.UpdateData(productId, leadStatusCode, rightAdd, availableStatusList, createdBy);
        }

        public static List<ActivityConfigData> SearchActivityConfig(string productId, string leadStatus, string rightAdd, string availableStatus)
        {
            KKSlmMsConfigActivityModel config = new KKSlmMsConfigActivityModel();
            return config.SearchActivityConfig(productId, leadStatus, rightAdd, availableStatus);
        }

        public static List<ActivityConfigData> GetActivityConfig(string productId, string leadStatusCode)
        {
            KKSlmMsConfigActivityModel config = new KKSlmMsConfigActivityModel();
            return config.GetActivityConfig(productId, leadStatusCode);
        }
    }
}
