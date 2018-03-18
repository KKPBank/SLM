using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsTeamtelesalesModel
    {
        public List<ControlListData> GetAllActiveTeamtelesalesData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_teamtelesales.Where(t=>t.is_Deleted == false).OrderBy(p => p.slm_TeamTelesales_Name).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_TeamTelesales_Name, ValueField = p.slm_TeamTelesales_Id.ToString() }).ToList();
            }
        }

        public bool CheckExistsTeamTeleSales(string Username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string StaffId = slmdb.kkslm_ms_staff.Where(x => x.slm_UserName == Username).FirstOrDefault().slm_StaffId.ToString();
                return slmdb.kkslm_ms_teamtelesales.Where(x => x.slm_HeadStaff == StaffId && x.is_Deleted == false).Count() > 0;
            }
        }

        public List<TeamTelesalesData> GetList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_teamtelesales.Where(p => p.is_Deleted == false).Select(p => 
                            new TeamTelesalesData {
                                TeamTelesalesId = p.slm_TeamTelesales_Id,
                                TeamTelesalesCode = p.slm_TeamTelesales_Code,
                                TeamTelesalesName = p.slm_TeamTelesales_Name,
                                HeadStaffId = p.slm_HeadStaff
                            }).ToList();
            }
        }
    }
}
