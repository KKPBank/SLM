using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class NoticeBiz
    {
        public static List<string> InsertData(string topic, string physicalPath, string noticeFolderName, string imageFileName, int imageFileSize, string attachFileName, int attactFileSize, bool isActive, string createdByUsername)
        {
            KKSlmTrNoticeModel notice = new KKSlmTrNoticeModel();
            return notice.InsertData(topic, physicalPath, noticeFolderName, imageFileName, imageFileSize, attachFileName, attactFileSize, isActive, createdByUsername);
        }

        public static List<string> UpdateData(int noticeId, string topic, string physicalPath, string noticeFolderName, string imageFileName, int imageFileSize, string attachFileName, int? attactFileSize, bool isActive, string updatedByUsername, bool updateImage, bool updateFile)
        {
            KKSlmTrNoticeModel notice = new KKSlmTrNoticeModel();
            return notice.UpdateData(noticeId, topic, physicalPath, noticeFolderName, imageFileName, imageFileSize, attachFileName, attactFileSize, isActive, updatedByUsername, updateImage, updateFile);
        }

        public static void DeleteData(int noticeId)
        {
            try
            {
                KKSlmTrNoticeModel notice = new KKSlmTrNoticeModel();
                notice.DeleteData(noticeId);
            }
            catch
            {
                throw;
            }
        }

        public static List<NoticeData> SearchNotice(string topic, string createDateFrom, string createDateTo, bool statusActive, bool statusInActive)
        {
            KKSlmTrNoticeModel notice = new KKSlmTrNoticeModel();
            return notice.SearchNotice(topic, createDateFrom, createDateTo, statusActive, statusInActive);
        }

        public static NoticeData GetNotice(string noticeId)
        {
            KKSlmTrNoticeModel notice = new KKSlmTrNoticeModel();
            return notice.GetNotice(noticeId);
        }

        public static List<int> GetNoticeIdList()
        {
            KKSlmTrNoticeModel notice = new KKSlmTrNoticeModel();
            return notice.GetNoticeIdList();
        }
    }
}
