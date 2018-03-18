using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class BankBiz
    {
        public static string GetBankByNo(string bankNo)
        {
            string ret = "";
            KKSlmMsBankModel b = new KKSlmMsBankModel();
            BankData tmp = b.GetBankDataByNo(bankNo);
            if (tmp != null)
                ret = tmp.BankName + "/";
            return ret;
        }
    }
}
