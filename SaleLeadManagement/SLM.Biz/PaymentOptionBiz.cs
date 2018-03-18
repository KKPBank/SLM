using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class PaymentOptionBiz
    {
        public static List<ControlListData> GetPaymentOptionList()
        {
            KKSlmMsPaymentOptionModel inscom = new KKSlmMsPaymentOptionModel();
            return inscom.GetPaymentOptionList();
        }
    }
}
