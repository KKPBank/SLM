using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class RepairTypeBiz
    {
        public static List<ControlListData> GetRepairTypeList()
        {
            KKSlmMsRepairTypeModel repairtype = new KKSlmMsRepairTypeModel();
            return repairtype.GetRepairTypeList();
        }
    }
}
