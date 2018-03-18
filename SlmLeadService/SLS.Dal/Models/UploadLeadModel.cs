using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource.Data;

namespace SLS.Dal.Models
{
    public class UploadLeadModel
    {
        private SLM_DBEntities slmdb = null;

        public UploadLeadModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public void UpdateTempSuccess(int uploadDetailId, string ticketId)
        {
            try
            {
                var item = slmdb.kkslm_tmp_uploadlead_detail.Where(p => p.slm_UploadDetailId == uploadDetailId).FirstOrDefault();
                if (item != null)
                {
                    item.slm_ticketId = ticketId;
                    item.slm_Status = "Success";
                    item.slm_UpdatedBy = "SYSTEM";
                    item.slm_UpdatedDate = DateTime.Now;
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateTempFail(int uploadDetailId, string errorDetail)
        {
            try
            {
                var item = slmdb.kkslm_tmp_uploadlead_detail.Where(p => p.slm_UploadDetailId == uploadDetailId).FirstOrDefault();
                if (item != null)
                {
                    item.slm_Status = "Fail";
                    item.slm_Remark = errorDetail;
                    item.slm_UpdatedBy = "SYSTEM";
                    item.slm_UpdatedDate = DateTime.Now;
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<UploadLeadSummaryJob> GetSendMailList()
        {
            try
            {
                string sql = @"SELECT tu.slm_UploadLeadId AS UploadLeadId, tu.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName, tud.slm_UploadDetailId AS UploadDetailId
                                , tud.slm_OwnerId AS OwnerEmpCode, CONVERT(date, tu.slm_batch_AssignDate) AS AssignDate, tu.slm_batch_AssignDate AS AssignDateTime
                                FROM kkslm_tmp_uploadlead  tu
                                LEFT JOIN kkslm_tmp_uploadlead_detail tud ON tu.slm_UploadLeadId = tud.slm_UploadLeadId
                                LEFT JOIN kkslm_ms_campaign cam ON tu.slm_CampaignId = cam.slm_CampaignId
                                WHERE tu.is_Deleted = 0
                                AND tud.slm_Status = 'Success' AND tud.is_Deleted = 0 AND tud.is_SendEmail = 0 ";

                return slmdb.ExecuteStoreQuery<UploadLeadSummaryJob>(sql).ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<UploadLeadSummaryJob> GetSendMailListz()
        {
            try
            {
                string sql = @"SELECT t.assign_date AS AssignDate, t.slm_OwnerId AS OwnerEmpCode, t.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName, t.ct [COUNT]
                                FROM (
	                                SELECT tu.slm_CampaignId, tud.slm_OwnerId,CONVERT(date, tu.slm_batch_AssignDate) [assign_date],COUNT(*) ct, ROW_NUMBER() OVER (PARTITION by slm_CampaignId ORDER BY COUNT(*) DESC) row_num
	                                FROM kkslm_tmp_uploadlead  tu
	                                LEFT JOIN kkslm_tmp_uploadlead_detail tud
	                                ON tu.slm_UploadLeadId = tud.slm_UploadLeadId
	                                AND tu.is_Deleted = 0
	                                AND tud.slm_Status = 'Success' -- Status
	                                AND tud.is_SendEmail = 0
	                                GROUP BY tu.slm_CampaignId, tud.slm_OwnerId,CONVERT(date, tu.slm_batch_AssignDate)
	                                ) t
                                LEFT JOIN kkslm_ms_campaign cam ON t.slm_CampaignId = cam.slm_CampaignId ";

                return slmdb.ExecuteStoreQuery<UploadLeadSummaryJob>(sql).ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
