using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class CmtMsProductGroupModel
    {
        public List<ControlListData> GetProductGroupData()
        {
            string sql = @"SELECT product_id AS ValueField, product_name AS TextField FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP 
                            WHERE (is_deleted = 0 OR is_deleted IS NULL)
                            ORDER BY product_name";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }

        public List<ControlListData> GetProductGroupData(string productGroupId)
        {
            string sql = @"SELECT product_id AS ValueField, product_name AS TextField FROM " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP 
                            WHERE (is_deleted = 0 OR is_deleted IS NULL) AND product_id = '" + productGroupId + @"'
                            ORDER BY product_name";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }
    }
}
