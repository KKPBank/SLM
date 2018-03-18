using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;

namespace SLS.Dal.Models
{
    public class SlmMsChannelModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmMsChannelModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public bool AuthenticateHeader(string username, string password, string channelId)
        {
            var channel = slmdb.kkslm_ms_channel.Where(p => p.slm_ChannelId.Equals(channelId) && p.slm_UserChannel.Equals(username) 
                && p.slm_PassChannel.Equals(password) && p.is_Deleted.Equals(0)).FirstOrDefault();

            return channel == null ? false : true;
        }
    }
}
