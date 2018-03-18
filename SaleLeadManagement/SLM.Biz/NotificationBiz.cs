using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Biz
{
    public class NotificationBiz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }

        public int GetNewNotification(string username)
        {
            int count = 0;
            using (SLM_DBEntities sdb = DBUtil.GetSlmDbEntities())
            {
                string sql = $"SELECT COUNT(*) FROM dbo.kkslm_tr_notification WITH (NOLOCK) WHERE slm_Username = '{username}' AND slm_Status = 'N' ";
                count = sdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
            }
            return count;
        }

        public List<NotificationData> GetNotificationList(string username)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //var list = slmdb.kkslm_tr_notification.Where(p => p.slm_Username == username && p.slm_Status == "N").ToList();
                    //list.ForEach(p => p.slm_Status = "U");
                    //slmdb.SaveChanges();

                    string sql = $"UPDATE kkslm_tr_notification SET slm_Status = 'U' WHERE slm_Username = '{username}' AND slm_Status = 'N' ";
                    slmdb.ExecuteStoreCommand(sql);

                    return slmdb.kkslm_tr_notification.Where(p => p.slm_Username == username).OrderByDescending(p => p.slm_CreatedDate).Take(10).Select(p => new NotificationData()
                    {
                        NotificationId = p.slm_NotificationId,
                        TicketId = p.slm_TicketId,
                        NotificationType = p.slm_NotificationType,
                        Title = p.slm_Title,
                        Status = p.slm_Status,
                        NotificationDate = p.slm_NotificationDate
                    }).ToList();

                    //sql = $@"SELECT TOP 10 p.slm_NotificationId AS NotificationId, p.slm_TicketId AS TicketId, p.slm_NotificationType AS NotificationType
                    //        , p.slm_Title AS Title, p.slm_Status AS [Status], p.slm_NotificationDate AS NotificationDate
                    //        FROM dbo.kkslm_tr_notification p WITH (NOLOCK) 
                    //        WHERE p.slm_Username = '{username}'
                    //        ORDER BY p.slm_CreatedDate DESC ";

                    //return slmdb.ExecuteStoreQuery<NotificationData>(sql).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateStatus(decimal notificationId, string status)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //var obj = slmdb.kkslm_tr_notification.Where(p => p.slm_NotificationId == notificationId).FirstOrDefault();
                    //if (obj != null)
                    //{
                    //    obj.slm_Status = status;
                    //    slmdb.SaveChanges();
                    //}

                    string sql = $"UPDATE dbo.kkslm_tr_notification SET slm_Status = '{status}' WHERE slm_NotificationId = '{notificationId.ToString()}' ";
                    slmdb.ExecuteStoreCommand(sql);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
