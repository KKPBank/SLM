using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class BeneficiaryBiz
    {
        public static List<ControlListData> GetBeneficiaryList()
        {
            BeneficiaryModel ben = new BeneficiaryModel();
            return ben.GetBeneficiaryList();
        }
    }
}
