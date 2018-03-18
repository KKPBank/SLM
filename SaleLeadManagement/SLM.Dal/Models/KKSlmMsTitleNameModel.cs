using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsTitleNameModel
    {
        public List<TitleNameData> ListTitleName()
        {
            string sql = @"SELECT slm_TitleId as TitleId, slm_TitleName As TitleName FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_title where is_deleted = 0";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<TitleNameData>(sql).ToList();
            }
        }
    }
}
