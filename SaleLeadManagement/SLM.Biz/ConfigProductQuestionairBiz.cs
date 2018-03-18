using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;

namespace SLM.Biz
{
    public class ConfigProductQuestionairBiz
    {
        public string GetUrl(string productId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.kkslm_ms_config_product_questionair.Where(p => p.slm_Product_Id == productId).Select(p => p.slm_Url).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
