using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmTrNoticeModel
    {
        public List<NoticeData> SearchNotice(string topic, string createDateFrom, string createDateTo, bool statusActive, bool statusInActive)
        {
            string sql = @"SELECT notice.slm_Notice_Id AS NoticeId, notice.slm_Subject AS Topic, notice.slm_CreatedDate AS CreatedDate, slm_ImageVirtualPath AS ImageVirtualPath, slm_FileVirtualPath AS FileVirtualPath
                            ,CASE WHEN notice.is_Deleted = '0' THEN 'ใช้งาน'
	                              WHEN notice.is_Deleted = '1' THEN 'ไม่ใช้งาน'
	                              ELSE '' END AS StatusDesc
                            FROM " + SLMConstant.SLMDBName + ".dbo.kkslm_tr_notice notice ";

            string whr = "";

            whr += (topic == "" ? "" : (whr == "" ? "" : " AND ") + " notice.slm_Subject LIKE @topic ");

            if (statusActive == true && statusInActive == false)
                whr += (whr == "" ? "" : " AND ") + " notice.is_Deleted = '0' ";
            else if (statusActive == false && statusInActive == true)
                whr += (whr == "" ? "" : " AND ") + " notice.is_Deleted = '1' ";

            if (!string.IsNullOrEmpty(createDateFrom) && !string.IsNullOrEmpty(createDateTo))
                whr += (whr == "" ? "" : " AND ") + " CONVERT(DATE, notice.slm_CreatedDate) BETWEEN '" + createDateFrom + "' AND '" + createDateTo + "' ";

            if (whr != "")
                sql += " WHERE " + whr;

            sql += " ORDER BY notice.slm_CreatedDate DESC ";

            object[] param = new object[] 
            { 
                new SqlParameter("@topic", "%" + topic + "%")
            };

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<NoticeData>(sql, param).ToList();
            }
        }

        public NoticeData GetNotice(string noticeId)
        {
            string sql = @"SELECT notice.slm_Notice_Id AS NoticeId, notice.slm_Subject AS Topic, notice.slm_CreatedDate AS CreatedDate, slm_ImageVirtualPath AS ImageVirtualPath, slm_FileVirtualPath AS FileVirtualPath
                            , notice.slm_ImageName AS ImageName, notice.slm_FileName AS [FileName], notice.slm_ImagePath AS ImagePhysicalPath, notice.slm_FilePath AS FilePhysicalPath
                            ,CASE WHEN notice.is_Deleted = '0' THEN 'ใช้งาน'
                                  WHEN notice.is_Deleted = '1' THEN 'ไม่ใช้งาน'
                                  ELSE '' END AS StatusDesc
                            ,CASE WHEN notice.is_Deleted = '0' THEN 'Y'
                                  WHEN notice.is_Deleted = '1' THEN 'N'
                                  ELSE '' END AS ActiveStatus
                            FROM dbo.kkslm_tr_notice notice
                            WHERE notice.slm_Notice_Id = '" + noticeId + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<NoticeData>(sql).FirstOrDefault();
            }
        }

        public List<string> InsertData(string topic, string physicalPath, string noticeFolderName, string imageFileName, int imageFileSize, string attachFileName, int? attactFileSize, bool isActive, string createdByUsername)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<string> results = new List<string>();
                    string noticeId = "";
                    DateTime createdDate = DateTime.Now;

                    kkslm_tr_notice notice = new kkslm_tr_notice()
                    {
                        slm_Subject = topic,
                        slm_ImageName = imageFileName,
                        slm_ImageSize = imageFileSize,
                        slm_FileName = string.IsNullOrEmpty(attachFileName) ? null : attachFileName,
                        slm_FileSize = attactFileSize == 0 ? null : attactFileSize,
                        slm_CreatedBy = createdByUsername,
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = createdByUsername,
                        slm_UpdatedDate = createdDate,
                        is_Deleted = !isActive
                    };
                    slmdb.kkslm_tr_notice.AddObject(notice);
                    slmdb.SaveChanges();

                    noticeId = notice.slm_Notice_Id.ToString();

                    //Save Physical Path
                    notice.slm_ImagePath = string.IsNullOrEmpty(imageFileName) ? null : Path.Combine(physicalPath, noticeId, imageFileName);
                    notice.slm_FilePath = string.IsNullOrEmpty(attachFileName) ? null : Path.Combine(physicalPath, noticeId, attachFileName);

                    //Save virtual Path
                    notice.slm_ImageVirtualPath = string.IsNullOrEmpty(imageFileName) ? null : "/" + noticeFolderName + "/" + noticeId + "/" + imageFileName;
                    notice.slm_FileVirtualPath = string.IsNullOrEmpty(attachFileName) ? null : "/" + noticeFolderName + "/" + noticeId + "/" + attachFileName;

                    results.Add(noticeId);
                    results.Add(notice.slm_ImagePath);
                    results.Add(notice.slm_FilePath);

                    slmdb.SaveChanges();
                    return results;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<string> UpdateData(int noticeId, string topic, string physicalPath, string noticeFolderName, string imageFileName, int imageFileSize, string attachFileName, int? attactFileSize, bool isActive, string updatedByUsername, bool updateImage, bool updateFile)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    List<string> results = new List<string>();
                    DateTime updatedDate = DateTime.Now;
                    var noticeEnt = slmdb.kkslm_tr_notice.Where(p => p.slm_Notice_Id == noticeId).FirstOrDefault();
                    if (noticeEnt != null)
                    {
                        noticeEnt.slm_Subject = topic;

                        noticeEnt.slm_UpdatedBy = updatedByUsername;
                        noticeEnt.slm_UpdatedDate = updatedDate;
                        noticeEnt.is_Deleted = !isActive;

                        if (updateImage)
                        {
                            noticeEnt.slm_ImageName = imageFileName;
                            noticeEnt.slm_ImageSize = imageFileSize;
                            noticeEnt.slm_ImagePath = string.IsNullOrEmpty(imageFileName) ? null : Path.Combine(physicalPath, noticeId.ToString(), imageFileName);
                            noticeEnt.slm_ImageVirtualPath = string.IsNullOrEmpty(imageFileName) ? null : "/" + noticeFolderName + "/" + noticeId.ToString() + "/" + imageFileName;
                        }
                        if (updateFile)
                        {
                            noticeEnt.slm_FileName = string.IsNullOrEmpty(attachFileName) ? null : attachFileName;
                            noticeEnt.slm_FileSize = attactFileSize == 0 ? null : attactFileSize;
                            noticeEnt.slm_FilePath = string.IsNullOrEmpty(attachFileName) ? null : Path.Combine(physicalPath, noticeId.ToString(), attachFileName);
                            noticeEnt.slm_FileVirtualPath = string.IsNullOrEmpty(attachFileName) ? null : "/" + noticeFolderName + "/" + noticeId.ToString() + "/" + attachFileName;
                        }

                        results.Add(noticeId.ToString());
                        results.Add(noticeEnt.slm_ImagePath);
                        results.Add(noticeEnt.slm_FilePath);

                        slmdb.SaveChanges();
                        return results;
                    }
                    else
                        throw new Exception("ไม่พบประกาศ NoticeId = " + noticeId.ToString());
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteData(int noticeId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var noticeEntity = slmdb.kkslm_tr_notice.Where(p => p.slm_Notice_Id == noticeId).FirstOrDefault();
                    if (noticeEntity != null)
                    {
                        slmdb.kkslm_tr_notice.DeleteObject(noticeEntity);
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public List<int> GetNoticeIdList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_tr_notice.Where(p => p.is_Deleted == false).Select(p => p.slm_Notice_Id).ToList();
            }
        }
    }
}
