using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class SlmMasterBiz
    {
        public static StaffData GetStaffData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffData(username);
        }

        public static decimal? GetStaffTypeId(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffType(username);
        }

        public static List<ScreenPrivilegeData> GetScreenPrivillegeList(int staffTypeId)
        {
            KKSlmScreenModel screen = new KKSlmScreenModel();
            return screen.GetScreenPrivilegeList(staffTypeId);
        }

        public static CampaignWSData GetCampaign(string campaignId)
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetCampaign(campaignId);
        }
    }
}
