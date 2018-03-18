using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;
using SLM.Dal;

namespace SLM.Biz
{
    public class InsuranceCarTypeBiz
    {
        public static List<ControlListData> GetInsuranceCarTypeForDropdownList()
        {
            KKSlmMsInsuranceCarTypeModel model = new KKSlmMsInsuranceCarTypeModel();
            return model.GetInsuranceCarTypeForDropdownList();
        }
        public static List<kkslm_ms_insurancecartype> LoadAllInsuranceCarType()
        {
            KKSlmMsInsuranceCarTypeModel model = new KKSlmMsInsuranceCarTypeModel();
            return model.LoadAllInsuranceCarType();
        }

    }
}
