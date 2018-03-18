using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;
using SLM.Dal;

namespace SLM.Biz
{
    public class CoverageTypeBiz
    {
        public static List<ControlListData> GetCoverageTypeForDropdownList()
        {
            KKSlmMsCoverageTypeModel coveragetype = new KKSlmMsCoverageTypeModel();
            return coveragetype.GetCoverageTypeForDropdownList();
        }

        public static List<kkslm_ms_coveragetype> LoadAllCoverageType()
        {
            KKSlmMsCoverageTypeModel coveragetype = new KKSlmMsCoverageTypeModel();
            return coveragetype.LoadAllCoverageType();
        }

        public static List<ControlListData> GetCoverageTypeList()
        {
            KKSlmMsCoverageTypeModel coveragetype = new KKSlmMsCoverageTypeModel();
            return coveragetype.GetCoverageTypeList();
        }

    }
}
