using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Dal.Models
{
    public class KKSlmMsConfigCloseJobModel
    {
        public bool CheckRequireCardId(string statusCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var require = slmdb.kkslm_ms_config_close_job.Where(p => p.slm_Status == statusCode && p.is_Deleted == false).Select(p => p.slm_RequireCardId).FirstOrDefault();
                return require != null ? require.Value : false;
            }
        }
    }
}
