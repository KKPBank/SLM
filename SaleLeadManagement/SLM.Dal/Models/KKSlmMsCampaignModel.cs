using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsCampaignModel
    {
        public List<ControlListData> GetAllActiveCampaignData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && p.slm_Status == "A").OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();
				//return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0).OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();
            }
        }

        public List<ControlListData> GetSaleAndBothCampaignData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return (from campaign in slmdb.kkslm_ms_campaign
                        from campaignProduct in
                            slmdb.CMT_CAMPAIGN_PRODUCT.Where(x => x.PR_CampaignId == campaign.slm_CampaignId)
                                .DefaultIfEmpty()
                        from mappingProduct in
                            slmdb.CMT_MAPPING_PRODUCT.Where(x => x.sub_product_id == campaignProduct.PR_ProductId)
                                .DefaultIfEmpty()
                        where
                            (campaign.is_Deleted == 0 && campaign.slm_Status == "A" &&
                             (mappingProduct.sub_product_type == 0 || mappingProduct.sub_product_type == 2))
                        orderby (campaign.slm_Seq)
                        select new ControlListData
                        {
                            TextField = campaign.slm_CampaignName,
                            ValueField = campaign.slm_CampaignId
                        }).ToList();
            }
        }

        public List<ControlListData> GetCampaignData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                //return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && DateTime.Today >= p.slm_StartDate && DateTime.Today <= p.slm_EndDate && p.slm_Status == "A").OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();
                return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && p.slm_Status == "A" && p.slm_CampaignType == SLMConstant.CampaignType.Mass).OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();
            }
        }
		public List<ControlListData> GetCampaignDataNew()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return (from campaign in slmdb.kkslm_ms_campaign
                        from campaignProduct in
                            slmdb.CMT_CAMPAIGN_PRODUCT.Where(x => x.PR_CampaignId == campaign.slm_CampaignId)
                                .DefaultIfEmpty()
                        from mappingProduct in
                            slmdb.CMT_MAPPING_PRODUCT.Where(x => x.sub_product_id == campaignProduct.PR_ProductId)
                                .DefaultIfEmpty()
                        where
                            (campaign.is_Deleted == 0 && campaign.slm_Status == "A" && campaign.slm_CampaignType == SLMConstant.CampaignType.Mass &&
                             (mappingProduct.sub_product_type == 0 || mappingProduct.sub_product_type == 2))
                        orderby (campaign.slm_Seq)
                        select new ControlListData
                        {
                            TextField = campaign.slm_CampaignName,
                            ValueField = campaign.slm_CampaignId
                        }).ToList();
            }
        }

        public List<ControlListData> GetCampaignEditData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                //return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && DateTime.Today >= p.slm_StartDate && DateTime.Today <= p.slm_EndDate && p.slm_Status == "A").OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();
                return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0).OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();
            }
        }
		public List<ControlListData> GetCampaignEditDataNew()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                //return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && DateTime.Today >= p.slm_StartDate && DateTime.Today <= p.slm_EndDate && p.slm_Status == "A").OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();
                //return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0).OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();
                return (from campaign in slmdb.kkslm_ms_campaign
                        from campaignProduct in
                            slmdb.CMT_CAMPAIGN_PRODUCT.Where(x => x.PR_CampaignId == campaign.slm_CampaignId)
                                .DefaultIfEmpty()
                        from mappingProduct in
                            slmdb.CMT_MAPPING_PRODUCT.Where(x => x.sub_product_id == campaignProduct.PR_ProductId)
                                .DefaultIfEmpty()
                        where
                            (campaign.is_Deleted == 0 &&
                             (mappingProduct.sub_product_type == 0 || mappingProduct.sub_product_type == 2))
                        orderby (campaign.slm_Seq)
                        select new ControlListData
                        {
                            TextField = campaign.slm_CampaignName,
                            ValueField = campaign.slm_CampaignId
                        }).ToList();
            }
        }

        public List<CampaignWSData> GetCampaignPopupData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var data = (from campaign in slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && DateTime.Today >= p.slm_StartDate && DateTime.Today <= p.slm_EndDate && p.slm_Status == "A").OrderBy(p => p.slm_Seq)
                            select new CampaignWSData { CampaignId = campaign.slm_CampaignId, CampaignName = campaign.slm_CampaignName, CampaignDetail = campaign.slm_Offer + ": " + campaign.slm_criteria }).ToList();
                return data;
            }
        }
        public List<CampaignWSData> GetCampaignPopupData(string CampaignList)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string[] camList = CampaignList.Split(',');
                string[] statusList = { "A", "X" };
                //string[] statusList = { "A" };
                var data = (from campaign in slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && camList.Contains(p.slm_CampaignId) == false && statusList.Contains(p.slm_Status) == true).OrderBy(p => p.slm_Seq)
                            select new CampaignWSData { CampaignId = campaign.slm_CampaignId, CampaignName = campaign.slm_CampaignName, CampaignDetail = campaign.slm_Offer + campaign.slm_criteria }).ToList();
                return data;
            }
        }

        public CampaignDetail GetCampaignDetail(string CampaignId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_campaign.Where(p => p.slm_CampaignId == CampaignId && p.is_Deleted == 0).OrderBy(p => p.slm_Seq).Select(p => new CampaignDetail { Offer = p.slm_Offer, Criteria = p.slm_criteria }).FirstOrDefault();
            }
        }

        public CampaignWSData GetCampaign(string campaignId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_campaign.Where(p => p.slm_CampaignId == campaignId).Select(p => new CampaignWSData { CampaignName = p.slm_CampaignName, CampaignDetail = p.slm_Offer + " : " + p.slm_criteria }).FirstOrDefault();
            }
        }

        //========================================= SLM 5 ==============================================================================
        public List<ControlListData> GetCampaignList(string campaignType)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (!string.IsNullOrEmpty(campaignType))
                    //return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && p.slm_Status == "A" && p.slm_CampaignType == campaignType).OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();
					return
                        slmdb.kkslm_ms_campaign.Where(
                            p => p.is_Deleted == 0 && p.slm_Status == "A" && p.slm_CampaignType == campaignType)
                            .OrderBy(p => p.slm_Seq)
                            .Select(
                                p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId })
                            .ToList();
                return
                    slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && p.slm_Status == "A")
                        .OrderBy(p => p.slm_Seq)
                        .Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId })
                        .ToList();
            }
        }
        public List<ControlListData> GetCampaignListNew(string campaignType)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                if (!string.IsNullOrEmpty(campaignType))

                    return (from campaign in slmdb.kkslm_ms_campaign 
                            from campaignProduct in
                                slmdb.CMT_CAMPAIGN_PRODUCT.Where(x => x.PR_CampaignId == campaign.slm_CampaignId)
                                    .DefaultIfEmpty()
                            from mappingProduct in
                                slmdb.CMT_MAPPING_PRODUCT.Where(x => x.sub_product_id == campaignProduct.PR_ProductId)
                                    .DefaultIfEmpty()
                            where
                                (campaign.is_Deleted == 0 && campaign.slm_Status == "A" && campaign.slm_CampaignType == campaignType &&
                                 (mappingProduct.sub_product_type == 0 || mappingProduct.sub_product_type == 2))
                            orderby (campaign.slm_Seq)
                            select new ControlListData
                            {
                                TextField = campaign.slm_CampaignName,
                                ValueField = campaign.slm_CampaignId
                            }).ToList();
                else
                    //return slmdb.kkslm_ms_campaign.Where(p => p.is_Deleted == 0 && p.slm_Status == "A").OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_CampaignName, ValueField = p.slm_CampaignId }).ToList();

                    return (from campaign in slmdb.kkslm_ms_campaign
                            from campaignProduct in slmdb.CMT_CAMPAIGN_PRODUCT.Where(x => x.PR_CampaignId == campaign.slm_CampaignId)
                                    .DefaultIfEmpty()
                            from mappingProduct in
                                slmdb.CMT_MAPPING_PRODUCT.Where(x => x.sub_product_id == campaignProduct.PR_ProductId)
                                    .DefaultIfEmpty()
                            where
                                (campaign.is_Deleted == 0 && campaign.slm_Status == "A" &&
                                 (mappingProduct.sub_product_type == 0 || mappingProduct.sub_product_type == 2))
                            orderby (campaign.slm_Seq)
                            select new ControlListData
                            {
                                TextField = campaign.slm_CampaignName,
                                ValueField = campaign.slm_CampaignId
                            }).ToList();
            }
        }
    }
}
