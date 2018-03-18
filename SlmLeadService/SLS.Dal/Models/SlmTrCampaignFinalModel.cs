using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource.Data;
using SLS.Resource;

namespace SLS.Dal.Models
{
    public class SlmTrCampaignFinalModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmTrCampaignFinalModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public void InsertData(string ticketId, Mandatory mandatory, ChannelInfo channelInfo)
        {
            try
            {
                var campaign = slmdb.kkslm_ms_campaign.Where(p => p.slm_CampaignId == mandatory.Campaign).FirstOrDefault();

                if (campaign != null)
                {
                    DateTime createDate = DateTime.Now;
                    kkslm_tr_campaignfinal camfinal = new kkslm_tr_campaignfinal();
                    camfinal.slm_TicketId = ticketId;
                    camfinal.slm_CampaignId = campaign.slm_CampaignId;
                    camfinal.slm_CampaignName = campaign.slm_CampaignName;

                    if (!string.IsNullOrEmpty(campaign.slm_Offer) && !string.IsNullOrEmpty(campaign.slm_criteria))
                        camfinal.slm_Description = campaign.slm_Offer + " : " + campaign.slm_criteria;
                    else if (!string.IsNullOrEmpty(campaign.slm_Offer))
                        camfinal.slm_Description = campaign.slm_Offer;
                    else if (!string.IsNullOrEmpty(campaign.slm_criteria))
                        camfinal.slm_Description = campaign.slm_criteria;

                    if (channelInfo != null)
                    {
                        if (!string.IsNullOrEmpty(channelInfo.CreateUser))
                        {
                            camfinal.slm_CreatedBy = channelInfo.CreateUser;
                            camfinal.slm_CreatedBy_Position = GetPositionId(channelInfo.CreateUser);
                        }
                        else
                            camfinal.slm_CreatedBy = "SYSTEM";
                    }
                    else
                        camfinal.slm_CreatedBy = "SYSTEM";

                    camfinal.slm_CreatedDate = createDate;
                    camfinal.is_Deleted = 0;
                    slmdb.kkslm_tr_campaignfinal.AddObject(camfinal);

                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new ServiceException(ApplicationResource.INS_INSERT_FAIL_CODE, ApplicationResource.INS_INSERT_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        private int? GetPositionId(string username)
        {
            SlmMsStaffModel staff = new SlmMsStaffModel(slmdb);
            return staff.GetPositionId(username);
        }
    }
}
