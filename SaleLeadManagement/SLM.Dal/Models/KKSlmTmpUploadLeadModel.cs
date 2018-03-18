using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Transactions;
using SLM.Resource.Data;
using SLM.Dal;

namespace SLM.Dal.Models
{
    public class KKSlmTmpUploadLeadModel : IDisposable
    {
        public string ErrorMessage { get; set; }
        public bool RedirectToSearch { get; set; }
        public bool RedirectToView { get; set; }

        public int SaveNewUpload(UploadAllData allData)
        {
            try
            {
                string fileName = allData.UploadFileName;
                string campaignId = allData.CampaignId;
                string channelId = allData.ChannelId;
                string createbyUsername = allData.CreateByUsername;
                List<UploadLeadData> dataList = allData.LeadDataList;
                string status = "Submit";
                int uploadLeadId = 0;
                List<kkslm_ms_cardtype> cardTypeList = new List<kkslm_ms_cardtype>();

                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    cardTypeList = slmdb.kkslm_ms_cardtype.Where(p => p.is_Deleted == false).ToList();
                }

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    using (var slmdb = DBUtil.GetSlmDbEntities())
                    {
                        //Insert Upload Main
                        kkslm_tmp_uploadlead upload = new kkslm_tmp_uploadlead
                        {
                            slm_FileName = fileName,
                            slm_CampaignId = campaignId,
                            slm_ChannelId = channelId,
                            slm_LeadCount = dataList.Count,
                            slm_batch_AssignDate = null,
                            slm_CreatedBy = createbyUsername,
                            slm_CreatedDate = DateTime.Now,
                            slm_UpdatedBy = createbyUsername,
                            slm_UpdatedDate = DateTime.Now,
                            slm_Status = status,
                            is_Deleted = 0
                        };
                        slmdb.kkslm_tmp_uploadlead.AddObject(upload);
                        slmdb.SaveChanges();
                        uploadLeadId = upload.slm_UploadLeadId;

                        //Insert Upload Detail
                        int? cardtypeId = null;
                        foreach (var data in dataList)
                        {
                            cardtypeId = null;
                            if (cardTypeList.Any(p => p.slm_CardTypeName == data.CardTypeDesc))
                            {
                                cardtypeId = cardTypeList.Where(p => p.slm_CardTypeName == data.CardTypeDesc).Select(p => p.slm_CardTypeId).FirstOrDefault();
                            }

                            kkslm_tmp_uploadlead_detail upload_detail = new kkslm_tmp_uploadlead_detail
                            {
                                slm_UploadLeadId = uploadLeadId,
                                slm_Name = TrimString(data.Firstname),
                                slm_LastName = TrimString(data.Lastname),
                                slm_CardTypeId = cardtypeId,
                                slm_CitizenId = TrimString(data.CitizenId),
                                slm_OwnerId = TrimString(data.OwnerEmpCode),
                                slm_DelegateId = TrimString(data.DelegateEmpCode),
                                slm_TelNo_1 = TrimString(data.TelNo1),
                                slm_TelNo_2 = TrimString(data.TelNo2),
                                slm_Detail = TrimString(data.Detail),
                                slm_ticketId = null,
                                slm_Status = status,
                                slm_Remark = null,
                                slm_CreatedBy = createbyUsername,
                                slm_CreatedDate = DateTime.Now,
                                slm_UpdatedBy = createbyUsername,
                                slm_UpdatedDate = DateTime.Now,
                                is_SendEmail = 0,
                                is_Deleted = 0
                            };
                            slmdb.kkslm_tmp_uploadlead_detail.AddObject(upload_detail);
                        }

                        //Insert History
                        kkslm_tmp_uploadlead_history history = new kkslm_tmp_uploadlead_history
                        {
                            slm_UploadLeadId = uploadLeadId,
                            slm_FileName = fileName,
                            slm_CampaignId = campaignId,
                            slm_ChannelId = channelId,
                            slm_LeadCount = dataList.Count,
                            slm_batch_AssignDate = null,
                            slm_Status = status,
                            slm_Operation = "Insert",
                            slm_CreatedBy = createbyUsername,
                            slm_CreatedDate = DateTime.Now,
                            slm_UpdatedBy = createbyUsername,
                            slm_UpdatedDate = DateTime.Now,
                            is_Deleted = 0
                        };
                        slmdb.kkslm_tmp_uploadlead_history.AddObject(history);
                        slmdb.SaveChanges();
                    }

                    ts.Complete();
                }

                return uploadLeadId;
            }
            catch
            {
                throw;
            }
        }

        public bool SaveUpdateUpload(int uploadLeadId, UploadAllData allData)
        {
            try
            {
                string status = "Submit";
                string createbyUsername = allData.CreateByUsername;
                List<kkslm_ms_cardtype> cardTypeList = new List<kkslm_ms_cardtype>();

                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    cardTypeList = slmdb.kkslm_ms_cardtype.Where(p => p.is_Deleted == false).ToList();
                }

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    using (var slmdb = DBUtil.GetSlmDbEntities())
                    {
                        var mainUpload = slmdb.kkslm_tmp_uploadlead.Where(p => p.slm_UploadLeadId == uploadLeadId && p.is_Deleted == 0).FirstOrDefault();
                        if (mainUpload == null)
                        {
                            ErrorMessage = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากไม่พบข้อมูลที่ต้องการบันทึก";
                            RedirectToSearch = true;
                            return false;
                        }

                        if (string.IsNullOrWhiteSpace(mainUpload.slm_Status) || mainUpload.slm_Status.ToLowerInvariant() != "submit")
                        {
                            ErrorMessage = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากสถานะไม่ใช่ Submit";
                            RedirectToView = true;
                            return false;
                        }

                        //Update Upload Main
                        mainUpload.slm_ChannelId = allData.ChannelId;
                        mainUpload.slm_CampaignId = allData.CampaignId;
                        mainUpload.slm_FileName = allData.UploadFileName;
                        mainUpload.slm_LeadCount = allData.LeadDataList.Count;
                        mainUpload.slm_Status = status;
                        mainUpload.slm_UpdatedBy = allData.CreateByUsername;
                        mainUpload.slm_UpdatedDate = DateTime.Now;

                        //Insert Upload Hisitory
                        kkslm_tmp_uploadlead_history history = new kkslm_tmp_uploadlead_history
                        {
                            slm_UploadLeadId = uploadLeadId,
                            slm_FileName = allData.UploadFileName,
                            slm_CampaignId = allData.CampaignId,
                            slm_ChannelId = allData.ChannelId,
                            slm_LeadCount = allData.LeadDataList.Count,
                            slm_batch_AssignDate = null,
                            slm_Status = status,
                            slm_Operation = "Update",
                            slm_CreatedBy = allData.CreateByUsername,
                            slm_CreatedDate = DateTime.Now,
                            slm_UpdatedBy = allData.CreateByUsername,
                            slm_UpdatedDate = DateTime.Now,
                            is_Deleted = 0
                        };
                        slmdb.kkslm_tmp_uploadlead_history.AddObject(history);
                        slmdb.SaveChanges();

                        //Delete Upload Detail
                        string del = string.Format("DELETE FROM kkslm_tmp_uploadlead_detail WHERE slm_UploadLeadId = '{0}'", uploadLeadId.ToString());
                        slmdb.ExecuteStoreCommand(del);

                        //Insert Upload Detail
                        int? cardtypeId = null;
                        foreach (var data in allData.LeadDataList)
                        {
                            cardtypeId = null;
                            if (cardTypeList.Any(p => p.slm_CardTypeName == data.CardTypeDesc))
                            {
                                cardtypeId = cardTypeList.Where(p => p.slm_CardTypeName == data.CardTypeDesc).Select(p => p.slm_CardTypeId).FirstOrDefault();
                            }

                            kkslm_tmp_uploadlead_detail upload_detail = new kkslm_tmp_uploadlead_detail
                            {
                                slm_UploadLeadId = uploadLeadId,
                                slm_Name = TrimString(data.Firstname),
                                slm_LastName = TrimString(data.Lastname),
                                slm_CardTypeId = cardtypeId,
                                slm_CitizenId = TrimString(data.CitizenId),
                                slm_OwnerId = TrimString(data.OwnerEmpCode),
                                slm_DelegateId = TrimString(data.DelegateEmpCode),
                                slm_TelNo_1 = TrimString(data.TelNo1),
                                slm_TelNo_2 = TrimString(data.TelNo2),
                                slm_Detail = TrimString(data.Detail),
                                slm_ticketId = null,
                                slm_Status = status,
                                slm_Remark = null,
                                slm_CreatedBy = createbyUsername,
                                slm_CreatedDate = DateTime.Now,
                                slm_UpdatedBy = createbyUsername,
                                slm_UpdatedDate = DateTime.Now,
                                is_SendEmail = 0,
                                is_Deleted = 0
                            };
                            slmdb.kkslm_tmp_uploadlead_detail.AddObject(upload_detail);
                        }

                        slmdb.SaveChanges();
                    }

                    ts.Complete();
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        private string TrimString(string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str.Trim();
        }

        public bool DeleteUpload(int uploadLeadId, string deleteByUsername)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    using (var slmdb = DBUtil.GetSlmDbEntities())
                    {
                        var mainUpload = slmdb.kkslm_tmp_uploadlead.Where(p => p.slm_UploadLeadId == uploadLeadId && p.is_Deleted == 0).FirstOrDefault();
                        if (mainUpload == null)
                        {
                            ErrorMessage = "ไม่สามารถลบข้อมูลได้ เนื่องจากไม่พบข้อมูลที่ต้องการลบ";
                            RedirectToSearch = true;
                            return false;
                        }

                        if (string.IsNullOrWhiteSpace(mainUpload.slm_Status) || mainUpload.slm_Status.ToLowerInvariant() != "submit")
                        {
                            ErrorMessage = "ไม่สามารถลบข้อมูลได้ เนื่องจากสถานะไม่ใช่ Submit";
                            RedirectToView = true;
                            return false;
                        }

                        //Set Delete Flag
                        mainUpload.is_Deleted = 1;
                        mainUpload.slm_UpdatedBy = deleteByUsername;
                        mainUpload.slm_UpdatedDate = DateTime.Now;

                        //Insert Upload Hisitory
                        kkslm_tmp_uploadlead_history history = new kkslm_tmp_uploadlead_history
                        {
                            slm_UploadLeadId = uploadLeadId,
                            slm_FileName = mainUpload.slm_FileName,
                            slm_CampaignId = mainUpload.slm_CampaignId,
                            slm_ChannelId = mainUpload.slm_ChannelId,
                            slm_LeadCount = mainUpload.slm_LeadCount,
                            slm_batch_AssignDate = mainUpload.slm_batch_AssignDate,
                            slm_Status = mainUpload.slm_Status,
                            slm_Operation = "Delete",
                            slm_CreatedBy = deleteByUsername,
                            slm_CreatedDate = DateTime.Now,
                            slm_UpdatedBy = deleteByUsername,
                            slm_UpdatedDate = DateTime.Now,
                            is_Deleted = 0
                        };
                        slmdb.kkslm_tmp_uploadlead_history.AddObject(history);
                        slmdb.SaveChanges();

                        //Delete Upload Detail
                        string del = string.Format("DELETE FROM kkslm_tmp_uploadlead_detail WHERE slm_UploadLeadId = '{0}'", uploadLeadId.ToString());
                        slmdb.ExecuteStoreCommand(del);
                    }

                    ts.Complete();
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        public UploadAllData GetUploadListById(int uploadLeadId)
        {
            try
            {
                UploadAllData allData = new UploadAllData();
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    var mainUpload = slmdb.kkslm_tmp_uploadlead.Where(p => p.slm_UploadLeadId == uploadLeadId && p.is_Deleted == 0).FirstOrDefault();
                    if (mainUpload == null)
                    {
                        ErrorMessage = "ไม่พบข้อมูลที่ต้องการ";
                        RedirectToSearch = true;
                        return null;
                    }

                    allData.ChannelId = mainUpload.slm_ChannelId;
                    allData.CampaignId = mainUpload.slm_CampaignId;
                    allData.UploadFileName = mainUpload.slm_FileName;

                    var leadList = (from d in slmdb.kkslm_tmp_uploadlead_detail
                                    from ct in slmdb.kkslm_ms_cardtype.Where(p => p.slm_CardTypeId == d.slm_CardTypeId).DefaultIfEmpty()
                                    where d.slm_UploadLeadId == uploadLeadId && d.is_Deleted == 0
                                    orderby d.slm_UploadDetailId
                                    select new UploadLeadData
                                    {
                                        Firstname = d.slm_Name,
                                        Lastname = d.slm_LastName,
                                        CardTypeDesc = ct.slm_CardTypeName,
                                        CitizenId = d.slm_CitizenId,
                                        OwnerEmpCode = d.slm_OwnerId,
                                        DelegateEmpCode = d.slm_DelegateId,
                                        TelNo1 = d.slm_TelNo_1,
                                        TelNo2 = d.slm_TelNo_2,
                                        Detail = d.slm_Detail,
                                        StatusDesc = d.slm_Status,
                                        Remark = d.slm_Remark,
                                        TicketId = d.slm_ticketId
                                    }).ToList();

                    allData.LeadDataList = leadList;
                }

                return allData;
            }
            catch
            {
                throw;
            }
        }

        public List<UploadFileInfo> SearchData(string fileName, string statusDesc)
        {
            try
            {
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = @"SELECT u.slm_UploadLeadId AS UploadLeadId, u.slm_FileName AS [Filename], u.slm_LeadCount AS LeadCount
                                    , u.slm_UpdatedDate AS LastestUploadDate, u.slm_batch_AssignDate AS AssignedDate
                                    , CASE WHEN p.slm_PositionNameAbb IS NULL THEN s.slm_StaffNameTH 
                                        ELSE p.slm_PositionNameAbb + ' - ' + s.slm_StaffNameTH END AS UploaderName
                                    , u.slm_Status AS FileStatus
                                    FROM kkslm_tmp_uploadlead u
                                    LEFT JOIN kkslm_ms_staff s ON UPPER(u.slm_UpdatedBy) = UPPER(s.slm_UserName)
                                    LEFT JOIN kkslm_ms_position p ON s.slm_Position_id = p.slm_Position_id
                                    WHERE u.is_Deleted = 0 {0} 
                                    ORDER BY u.slm_UpdatedDate DESC ";

                    string whr = "";

                    whr += (fileName == "" ? "" : (whr == "" ? "" : " AND ") + " u.slm_FileName LIKE @filename ");
                    whr += (statusDesc == "" ? "" : (whr == "" ? "" : " AND ") + " u.slm_Status = '" + statusDesc + "' ");

                    whr = (whr == "") ? "" : " AND " + whr;
                    sql = string.Format(sql, whr);

                    object[] param = new object[]
                    {
                        new SqlParameter("@filename", "%" + fileName + "%")
                    };

                    return slmdb.ExecuteStoreQuery<UploadFileInfo>(sql, param).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CanEdit(int uploadLeadId)
        {
            try
            {
                using (var slmdb = DBUtil.GetSlmDbEntities())
                {
                    var upload = slmdb.kkslm_tmp_uploadlead.Where(p => p.slm_UploadLeadId == uploadLeadId && p.is_Deleted == 0).FirstOrDefault();
                    if (upload != null)
                    {
                        if (!string.IsNullOrEmpty(upload.slm_Status) && upload.slm_Status.ToLowerInvariant() == "submit")
                        {
                            return true;
                        }
                        else
                        {
                            ErrorMessage = "ไม่สามารถแสดงหน้าจอแก้ไขได้ เนื่องจากสถานะไม่ใช่ Submit";
                            RedirectToView = true;
                            return false;
                        }
                    }
                    else
                    {
                        ErrorMessage = "ไม่พบข้อมูลที่ต้องการ";
                        RedirectToSearch = true;
                        return false;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        #region IDisposable

        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
            }

            _disposed = true;
        }

        #endregion
    }
}
