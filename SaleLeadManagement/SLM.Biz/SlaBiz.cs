using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class SlaBiz
    {
        public static List<SlaData> SearchConfigSla(string productId, string campaignId, string channelId, string statusCode)
        {
            KKSlmMsSlaModel sla = new KKSlmMsSlaModel();
            return sla.SearchConfigSla(productId, campaignId, channelId, statusCode);
        }

        public static void InsertData(string productId, string campaignId, string channelId, string statusCode, string subStatusCode, int slaMin, int slaTime, int slaDay, string createBy)
        {
            KKSlmMsSlaModel sla = new KKSlmMsSlaModel();
            sla.InsertData(productId, campaignId, channelId, statusCode, subStatusCode, slaMin, slaTime, slaDay, createBy);
        }

        public static void UpdateData(int slaId, string productId, string campaignId, string channelId, string statusCode, string subStatusCode, int slaMin, int slaTime, int slaDay, string updateBy)
        {
            KKSlmMsSlaModel sla = new KKSlmMsSlaModel();
            sla.UpdateData(slaId, productId, campaignId, channelId, statusCode, subStatusCode, slaMin, slaTime, slaDay, updateBy);
        }

        public static void DeleteSla(int slaId)
        {
            KKSlmMsSlaModel sla = new KKSlmMsSlaModel();
            sla.DeleteSla(slaId);
        }
    }
}
