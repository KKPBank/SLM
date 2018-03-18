using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class PaymentMethodBiz
    {
        public static List<ControlListData> GetPaymentMethodList()
        {
            KKSlmMsPaymentMethodModel inscom = new KKSlmMsPaymentMethodModel();
            return inscom.GetPaymentMethodList();
        }
    }
}
