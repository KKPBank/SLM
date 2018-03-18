using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class ConfigProductSideMenuBiz
    {
        public List<ConfigProductTabSideMenuData> GetSideMenuList(string productId, string campaignId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<ConfigProductTabSideMenuData> list = new List<ConfigProductTabSideMenuData>();
                    list = slmdb.kkslm_ms_config_product_sidemenu.Where(p => p.slm_CampaignId == campaignId && p.is_Deleted == false).OrderBy(p => p.slm_Seq).Select(p => new ConfigProductTabSideMenuData()
                    {
                        CampaignId = p.slm_CampaignId,
                        ProductId = p.slm_Product_Id,
                        Seq = p.slm_Seq,
                        MenuCode = p.slm_MenuCode,
                        MenuDescription = p.slm_MenuDescription,
                        ImagePath = p.slm_ImagePath
                    }).ToList();

                    if (list.Count == 0)
                    {
                        list = slmdb.kkslm_ms_config_product_sidemenu.Where(p => p.slm_Product_Id == productId && p.is_Deleted == false).OrderBy(p => p.slm_Seq).Select(p => new ConfigProductTabSideMenuData()
                        {
                            CampaignId = p.slm_CampaignId,
                            ProductId = p.slm_Product_Id,
                            Seq = p.slm_Seq,
                            MenuCode = p.slm_MenuCode,
                            MenuDescription = p.slm_MenuDescription,
                            ImagePath = p.slm_ImagePath
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
