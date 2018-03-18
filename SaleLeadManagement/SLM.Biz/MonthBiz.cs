using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class MonthBiz
    {
        public List<ControlListData> GetMonthList()
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.kkslm_ms_month.Select(p => new ControlListData { TextField = p.slm_MonthNameTh, ValueField = p.slm_MonthId }).ToList();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
