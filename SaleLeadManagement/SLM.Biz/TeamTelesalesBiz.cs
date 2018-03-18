using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Biz
{
    public class TeamTelesalesBiz
    {
        public List<ControlListData> GetTeamTelesalesList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_teamtelesales.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_TeamTelesales_Name).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_TeamTelesales_Name, ValueField = p.slm_TeamTelesales_Id.ToString() }).ToList();
            }
        }
    }
}
