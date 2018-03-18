using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsChannelModel
    {
        public List<ControlListData> GetChannelData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_channel.Where(p => p.is_Deleted == 0).OrderBy(p => p.slm_ChannelDesc).Select(p => new ControlListData { TextField = p.slm_ChannelDesc, ValueField = p.slm_ChannelId }).ToList();
            }
        }
        public string GetChannelId(string channeldesc)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_channel.Where(p => p.slm_ChannelDesc == channeldesc && p.is_Deleted == 0).Select(p => p.slm_ChannelId).FirstOrDefault();
            }
        }

        public bool CheckUserErrorInUse(string username)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var count = slmdb.kkslm_ms_channel.Where(p => p.slm_UserError == username && p.is_Deleted == 0).Count();
                return count > 0 ? true : false;
            }
        }

        public bool CheckUserAdminProductInUse(string empcode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var count = slmdb.kkslm_ms_config_product_admin.Where(p => p.slm_EmpCode == empcode).Count();
                return count > 0 ? true : false;
            }
        }
    }
}
