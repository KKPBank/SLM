using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class SlmScr029Biz
    {
        public static List<ControlListData> GetSubOptionList(string productId, string campaignId)
        {
            KKSlmMsConfigProductSubstatusModel option = new KKSlmMsConfigProductSubstatusModel();
            return option.GetSubOptionList(productId,campaignId);
        }

        public static List<ControlListData> GetSubOptionList(bool isFollowup, bool isOutbound, string productId, string campaignId)
        {
            KKSlmMsConfigProductSubstatusModel option = new KKSlmMsConfigProductSubstatusModel();
            return option.GetOptionList(isFollowup, isOutbound, productId, campaignId);
        }

        //public static List<SearchObtResult> SearchObtData(SearchObtCondition data, string username, string orderByFlag)
        //{
        //    SearchObtModel search = new SearchObtModel();
        //    string createDate = data.CreatedDate.Year != 1 ? data.CreatedDate.Year + data.CreatedDate.ToString("-MM-dd") : string.Empty;
        //    string assignDate = data.AssignedDate.Year != 1 ? data.AssignedDate.Year + data.AssignedDate.ToString("-MM-dd") : string.Empty;

        //    return search.SearchObtData(data.TicketId, data.Firstname, data.Lastname, data.CardType, data.CitizenId, data.CampaignId, data.ChannelId, data.OwnerUsername,
        //        createDate, assignDate, data.StatusList, username, data.StaffType, data.OwnerBranch, data.DelegateBranch, data.DelegateLead, data.ContractNoRefer, data.CreateByBranch, data.CreateBy, orderByFlag
        //        , data.CarLicenseNo, data.NextContactDate.ToString(), data.PolicyEffectiveYear, data.PolicyEffectiveMonth, data.IsFollowup, data.IsInbound, data.IsOutbound, data.IsTaskList, data.IsTabFollowup, data.SubStatusList, data.ContractNo);
        //}

        public int GetAmountJobTabFollowUp(string username)
        {
            return new SearchObtModel().GetAmountJobTabFollowUp(username);
        }

        public List<ControlListData> GetAmountInboundOutBound(string username)
        {
            return new SearchObtModel().GetAmountInboundOutBound(username);
        }

        public List<SearchObtResult> SearchTabFollowUp(SearchObtCondition cond, out string logError)
        {
            return new SearchObtModel().SearchTabFollowUp(cond, out logError);
        }

        public List<SearchObtResult> SearchTabAllTask(SearchObtCondition cond, out string logError)
        {
            return new SearchObtModel().SearchTabAllTask(cond, out logError);
        }

        //public int GetAmountOutboundOnHand(string username)
        //{
        //    return new SearchObtModel().GetAmountOutboundOnHand(username);
        //}

        //public int GetAmountInboundOnHand(string username)
        //{
        //    return new SearchObtModel().GetAmountInboundOnHand(username);
        //}
    }
}
