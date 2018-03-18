using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Dal.Models
{
    public class KKSlmTrHistoryModel
    {

        public void InsertData(string ticketId, string historyTypeCode, string oldValue, string newValue, string createByUsername, DateTime createDate)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == createByUsername).FirstOrDefault();
                    kkslm_tr_history history = new kkslm_tr_history()
                    {
                        slm_ticketId = ticketId,
                        slm_History_Type_Code = historyTypeCode,
                        slm_OldValue = string.IsNullOrEmpty(oldValue) ? null : oldValue,
                        slm_NewValue = string.IsNullOrEmpty(newValue) ? null : newValue,
                        slm_CreatedBy = createByUsername,
                        slm_CreatedBy_Position = staff != null ? staff.slm_Position_id : null,
                        slm_CreateBy_Branch = staff != null ? staff.slm_BranchCode : null,
                        slm_CreatedDate = createDate,
                        slm_UpdatedBy = createByUsername,
                        slm_UpdatedDate = createDate,
                        is_Deleted = false
                    };
                    slmdb.kkslm_tr_history.AddObject(history);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public static void InsertHistory(SLM_DBEntities slmdb, string ticketId, string historyTypeCode, string oldValue, string newValue, string createByUsername, DateTime createDate)
        {
            try
            {
                var staff = slmdb.kkslm_ms_staff.Where(p => p.slm_UserName == createByUsername).FirstOrDefault();
                kkslm_tr_history history = new kkslm_tr_history()
                {
                    slm_ticketId = ticketId,
                    slm_History_Type_Code = historyTypeCode,
                    slm_OldValue = string.IsNullOrEmpty(oldValue) ? null : oldValue,
                    slm_NewValue = string.IsNullOrEmpty(newValue) ? null : newValue,
                    slm_CreatedBy = createByUsername,
                    slm_CreatedBy_Position = staff != null ? staff.slm_Position_id : null,
                    slm_CreateBy_Branch = staff != null ? staff.slm_BranchCode : null,
                    slm_CreatedDate = createDate,
                    slm_UpdatedBy = createByUsername,
                    slm_UpdatedDate = createDate,
                    is_Deleted = false
                };
                slmdb.kkslm_tr_history.AddObject(history);
            }
            catch
            {
                throw;
            }
        }
    }
}
