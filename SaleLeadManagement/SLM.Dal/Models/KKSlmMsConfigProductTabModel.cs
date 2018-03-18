using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsConfigProductTabModel
    {
        public List<ConfigProductTabData> GetTabScreenList(string productId, string campaignId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<ConfigProductTabData> list = new List<ConfigProductTabData>();
                    list = slmdb.kkslm_ms_config_product_tab.Where(p => p.slm_CampaignId == campaignId && p.is_Deleted == false).OrderBy(p => p.slm_Seq).Select(p => new ConfigProductTabData()
                    {
                        CampaignId = p.slm_CampaignId,
                        ProductId = p.slm_Product_Id,
                        Seq = p.slm_Seq,
                        ScreenPath = p.slm_ScreenPath,
                        TabCode = p.slm_TabCode,
                        TabHeader = p.slm_TabHeader
                    }).ToList();

                    if (list.Count == 0)
                    {
                        list = slmdb.kkslm_ms_config_product_tab.Where(p => p.slm_Product_Id == productId && p.is_Deleted == false).OrderBy(p => p.slm_Seq).Select(p => new ConfigProductTabData()
                        {
                            CampaignId = p.slm_CampaignId,
                            ProductId = p.slm_Product_Id,
                            Seq = p.slm_Seq,
                            ScreenPath = p.slm_ScreenPath,
                            TabCode = p.slm_TabCode,
                            TabHeader = p.slm_TabHeader
                        }).ToList();
                    }

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
