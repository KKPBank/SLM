using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Dal.Models
{
    public class SlmWsConfigModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmWsConfigModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public bool CheckUseOfAolService(string channelId, string campaignId)
        {
            try
            {
                var config = slmdb.kkslm_ws_config.Where(p => p.slm_ChannelId == channelId && p.slm_CampaignId == campaignId).FirstOrDefault();
                return config != null ? config.IS_AOL : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
