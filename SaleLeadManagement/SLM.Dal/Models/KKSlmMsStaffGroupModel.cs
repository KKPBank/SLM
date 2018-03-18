using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsStaffGroupModel
    {
//        private SLM_DBEntities slmdb = null;
//        private string SLMDBName = System.Configuration.ConfigurationManager.AppSettings["SLMDBName"].ToString();

//        public KKSlmMsStaffGroupModel()
//        {
//            slmdb = new SLM_DBEntities();
//        }
//        public List<StaffGroupData> GetStaffGroupData(int staffid)
//        {
//            string sql = @" SELECT distinct staff.slm_StaffId AS StaffId,sgroup.slm_StaffGroupId AS StaffGroupId,sgroup.slm_CampaignId AS CampaignId,
//	                            cam.slm_CampaignName AS CampaignName
//                            FROM " + SLMDBName + @".DBO.kkslm_ms_staff staff INNER JOIN " + SLMDBName + @".dbo.kkslm_ms_staff_group sgroup on staff.slm_StaffId = sgroup.slm_StaffId 
//                            INNER JOIN " + SLMDBName + @".DBO.kkslm_ms_campaign cam on cam.slm_CampaignId = sgroup.slm_CampaignId 
//                            WHERE cam.slm_Status in ('A','X') and sgroup.is_Deleted = 0 AND cam.is_Deleted = 0 AND STAFF.slm_StaffId =  " + staffid;

//            return slmdb.ExecuteStoreQuery<StaffGroupData>(sql).ToList();
//        }

//        public void InsertStaffGroupList(List<StaffGroupData> StaffGroupListData, string UserId)
//        {
//            try
//            {
//                if (StaffGroupListData.Count > 0)
//                {
//                    for (int i = 0; i < StaffGroupListData.Count; i++)
//                    {
//                        kkslm_ms_staff_group staffgroup = new kkslm_ms_staff_group();
//                        staffgroup.slm_StaffId = StaffGroupListData[i].StaffId;
//                        staffgroup.slm_CampaignId = StaffGroupListData[i].CampaignId;
//                        staffgroup.slm_CreatedBy = UserId;
//                        staffgroup.slm_CreatedDate = DateTime.Now;
//                        staffgroup.is_Deleted = 0;
//                        slmdb.kkslm_ms_staff_group.AddObject(staffgroup);
//                    }
//                    slmdb.SaveChanges();
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        public void UpdateStaffGroupList(decimal StaffGroupId, string UserId)
//        {
//            var staffgroup = slmdb.kkslm_ms_staff_group.Where(p => p.slm_StaffGroupId == StaffGroupId).FirstOrDefault();
//            if (staffgroup != null)
//            {
//                try
//                {
//                    staffgroup.is_Deleted  = 1;
//                    staffgroup.slm_UpdatedBy = UserId;
//                    staffgroup.slm_UpdatedDate = DateTime.Now;
//                    slmdb.SaveChanges();
//                }
//                catch (Exception ex)
//                {
//                    throw ex;
//                }
//            }
//        }
    }
}
