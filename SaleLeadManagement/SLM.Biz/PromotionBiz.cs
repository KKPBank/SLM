using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Dal;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class PromotionBiz
    {
        public static List<PromotionData> ListInsuranceName()
        {
            KKSlmMsPromotionModel listInsurance = new KKSlmMsPromotionModel();
            return listInsurance.ListInsuranceName();
        }
    }
}
