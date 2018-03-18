using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class AccountStatusBiz
    {
        public static List<AccountStatusData> GetAccountStatusList()
        {
            KKSlmMsAccountStatusModel acc = new KKSlmMsAccountStatusModel();
            return acc.GetAccountStatusList();
        }

        public static AccountStatusData GetAccountStatusByCode(string accountStatus)
        {
            KKSlmMsAccountStatusModel acc = new KKSlmMsAccountStatusModel();
            return acc.GetAccountStatusByCode(accountStatus);
        }
    }
}
