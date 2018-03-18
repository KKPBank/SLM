using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Dal;

namespace SLM.Biz
{
    public class OptionBiz
    {
        public static List<ControlListData> GetOptionList(string optionType)
        {
            KKSlmMsOptionModel option = new KKSlmMsOptionModel();
            return option.GetOptionList(optionType);
        }

        public static List<ControlListData> GetOptionListForActivityConfig(string optionType)
        {
            KKSlmMsOptionModel option = new KKSlmMsOptionModel();
            return option.GetOptionListForActivityConfig(optionType);
        }

        public static List<ControlListData> GetStatusListByActivityConfig(string productId, string leadStatus)
        {
            KKSlmMsOptionModel option = new KKSlmMsOptionModel();
            return option.GetStatusListByActivityConfig(productId, leadStatus);
        }


        public static object GetSubStatusList(string productid, string campaignId)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return (from sub in sdc.kkslm_ms_config_product_substatus
                        where sub.slm_CampaignId == campaignId || sub.slm_Product_Id == productid
                        select new { ValueField = sub.slm_SubStatusCode, TextField = sub.slm_SubStatusName }
                        ).ToList();
            }
        }
    }
}
