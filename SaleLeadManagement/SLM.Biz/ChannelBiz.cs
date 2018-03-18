using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class ChannelBiz
    {
        public static List<ControlListData> GetChannelList()
        {
            KKSlmMsChannelModel chann = new KKSlmMsChannelModel();
            return chann.GetChannelData();
        }

        public static bool CheckUserErrorInUse(string username)
        {
            KKSlmMsChannelModel chann = new KKSlmMsChannelModel();
            return chann.CheckUserErrorInUse(username);
        }

        public static bool CheckUserAdminProductInUse(string empcode)
        {
            KKSlmMsChannelModel chann = new KKSlmMsChannelModel();
            return chann.CheckUserAdminProductInUse(empcode);
        }
    }
}
