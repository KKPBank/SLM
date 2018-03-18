using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSLMTrCampaignFinalModel
    {
        public void InsertData(CampaignWSData CamData, string createByUsername, DateTime createDate)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    kkslm_tr_campaignfinal camfinal = new kkslm_tr_campaignfinal();
                    camfinal.slm_TicketId = CamData.TicketId;
                    camfinal.slm_CampaignId = CamData.CampaignId;
                    camfinal.slm_CampaignName = CamData.CampaignName;
                    camfinal.slm_Description = CamData.CampaignDetail;
                    camfinal.slm_CreatedBy = createByUsername;
                    camfinal.slm_CreatedBy_Position = GetPositionId(createByUsername, slmdb);
                    camfinal.slm_CreatedDate = createDate;
                    camfinal.slm_UpdatedBy = createByUsername;
                    camfinal.slm_UpdatedDate = createDate;
                    slmdb.kkslm_tr_campaignfinal.AddObject(camfinal);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertCampaignList(List<CampaignWSData> CampaignListData, string createByUsername)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    if (CampaignListData.Count > 0)
                    {
                        for (int i = 0; i < CampaignListData.Count; i++)
                        {
                            kkslm_tr_campaignfinal camfinal = new kkslm_tr_campaignfinal();
                            camfinal.slm_TicketId = CampaignListData[i].TicketId;
                            camfinal.slm_CampaignId = CampaignListData[i].CampaignId;
                            camfinal.slm_CampaignName = CampaignListData[i].CampaignName;
                            camfinal.slm_Description = CampaignListData[i].CampaignDetail;
                            camfinal.slm_CreatedBy = createByUsername;
                            camfinal.slm_CreatedBy_Position = GetPositionId(createByUsername, slmdb);
                            camfinal.slm_CreatedDate = DateTime.Now;
                            slmdb.kkslm_tr_campaignfinal.AddObject(camfinal);
                        }
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertCampaignList(List<ProductData> productList, string ticketId, string username)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    DateTime createdDate = DateTime.Now;
                    foreach (ProductData campaign in productList)
                    {
                        kkslm_tr_campaignfinal camfinal = new kkslm_tr_campaignfinal();
                        camfinal.slm_TicketId = ticketId;
                        camfinal.slm_CampaignId = campaign.CampaignId;
                        camfinal.slm_CampaignName = campaign.CampaignName;
                        camfinal.slm_Description = campaign.CampaignDesc;
                        camfinal.slm_CreatedBy = username;
                        camfinal.slm_CreatedBy_Position = GetPositionId(username, slmdb);
                        camfinal.slm_CreatedDate = createdDate;
                        slmdb.kkslm_tr_campaignfinal.AddObject(camfinal);

                        KKSlmTrHistoryModel.InsertHistory(slmdb, ticketId, SLMConstant.HistoryTypeCode.AddCampaignFinal, "", campaign.CampaignId, username, createdDate);
                    }
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        private int? GetPositionId(string username, SLM_DBEntities slmdb)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetPositionId(username);
        }

        public void InsertData(SLM_DBEntities slmdb, string new_ticketId, ProductData productData, string username, DateTime createdDate)
        {
            try
            {
                kkslm_tr_campaignfinal camfinal = new kkslm_tr_campaignfinal();
                camfinal.slm_TicketId = new_ticketId;
                camfinal.slm_CampaignId = productData.CampaignId;
                camfinal.slm_CampaignName = productData.CampaignName;
                camfinal.slm_Description = productData.CampaignDesc;
                camfinal.slm_CreatedBy = username;
                camfinal.slm_CreatedBy_Position = GetPositionId(username, slmdb);
                camfinal.slm_CreatedDate = createdDate;
                slmdb.kkslm_tr_campaignfinal.AddObject(camfinal);
            }
            catch
            {
                throw;
            }
        }
    }
}
