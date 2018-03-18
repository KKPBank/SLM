using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Dal.Models;
using SLM.Resource.Data;
using System.Globalization;

namespace SLM.Biz
{
    public class ObtScr002Biz
    {
        public static List<ControlListData> GetInsCom()
        {
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                return operdb.kkslm_ms_ins_com.Where(kk => kk.is_Deleted == false).ToList().Select(kk => new ControlListData() { TextField = kk.slm_InsNameTh, ValueField = kk.slm_Ins_Com_Id.ToString() }).ToList();
            }
        }

        public static List<ReceiveNoData> GetSearchData(string product, decimal? insComId, bool isEdit)
        {
            var products = new CmtMappingProductModel().GetProductList().FirstOrDefault(p => p.ValueField == product);
            List<ReceiveNoData> result = new List<ReceiveNoData>();
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var query = (from com in operdb.kkslm_ms_ins_com
                             join r in
                                 (
                                     from rr in operdb.kkslm_ms_receiveno
                                     where (rr.slm_Product_Id == product || rr.slm_Product_Id == null)
                                     select rr
                                     ) on com.slm_Ins_Com_Id equals r.slm_Ins_Com_Id into comReceive
                             from c in comReceive.DefaultIfEmpty()
                             where com.is_Deleted == false
                             select new ReceiveNoData()
                             {
                                 CodeName = c.slm_CodeName ?? "",
                                 InsComName = com.slm_InsNameTh,
                                 InsComCode = com.slm_InsCode,
                                 CreatedBy = c.slm_CreatedBy ?? "",
                                 CreatedDate = c.slm_CreatedDate,
                                 // CreaterName =
                                 InsComId = com.slm_Ins_Com_Id,
                                 IsDeleted = c == null ? true : c.is_Deleted,
                                 Lot = c.slm_Lot, // c.slm_Lot == null ? 0 : c.slm_Lot,
                                 ProductId = c.slm_Product_Id == null ? product : c.slm_Product_Id,
                                 ReceiveNoEnd = c.slm_ReceiveNoEnd, // c.slm_ReceiveNoEnd == null ? 0 : c.slm_ReceiveNoEnd,
                                 ReceiveNoId = c.slm_ReceiveNoId, // c.slm_ReceiveNoId == null ? -1 : c.slm_ReceiveNoId,
                                 ReceiveNoStart = c.slm_ReceiveNoStart, // c.slm_ReceiveNoStart == null ? 0 : c.slm_ReceiveNoStart,
                                 //ReceiveNoCancel = c.slm_Cancel ?? 0,                        //Comment by Pom on 29/04/2016
                                 //ReceiveNoRemain = c.slm_Remain ?? 0,                        //Comment  by Pom on 29/04/2016
                                 //ReceiveNoTotal = c.slm_Total == null ? 0 : c.slm_Total,     //Comment  by Pom on 29/04/2016
                                 UpdatedBy = c.slm_UpdatedBy,
                                 UpdatedDate = c.slm_UpdatedDate,
                                 // UpdaterName = 
                                 //ReceiveNoUsed = c.slm_Used ?? 0                             //Comment  by Pom on 29/04/2016
                             }
                            );
                if (insComId != null && insComId > 0)
                    query.Where(q => q.InsComId == insComId);
                var queryResult = query.ToList();

                if (isEdit)
                {
                    result = queryResult;
                }
                else
                {
                    result = queryResult.Where(q => q.IsDeleted == false).ToList();
                    queryResult.Where(q => q.IsDeleted == true)
                        .OrderBy(q => q.InsComId)
                        .ThenByDescending(q => q.UpdatedDate)
                        .ToList()
                        .ForEach(q =>
                        {
                            if (!result.Where(r => r.InsComId == q.InsComId).Any())
                                result.Add(q);
                        });
                }
                // update product name 
                result.ForEach(r => r.ProductName = products.TextField);

                if (insComId != null)
                {
                    result = result.Where(r => r.InsComId == insComId).ToList();
                }

                var receiveNoId = result.Select(r => r.ReceiveNoId).Distinct().ToList();
                //var receiveNoList = operdb.kkslm_ms_receivenolist.Where(r => receiveNoId.Contains((decimal)r.slm_ReceiveNoId)).Select(r => new { r.slm_ReceiveNoListId, r.slm_ReceiveNoId, r.is_Deleted, r.slm_Status }).ToList();
                var receiveNoList = operdb.kkslm_ms_receivenolist
                                    .Where(r => receiveNoId.Contains((decimal)r.slm_ReceiveNoId))
                                    .GroupBy(r => new { r.slm_ReceiveNoId, r.slm_Status })
                                    .Select(g => new { g.Key.slm_ReceiveNoId, g.Key.slm_Status, total = g.Count() })
                                    .ToList();

                List<string> staffID = result.Select(r => r.CreatedBy).Union(result.Select(l => l.UpdatedBy).ToList()).Distinct().ToList();
                var staff = slmdb.kkslm_ms_staff.Where(s => staffID.Contains(s.slm_UserName))
                            .Select(s => new
                            {
                                StaffNameTh = s.slm_StaffNameTH,
                                StaffUserName = s.slm_UserName
                            }).ToList();

                result.ForEach(r =>
                {
                    r.CreaterName = staff.FirstOrDefault(s => s.StaffUserName == r.CreatedBy) == null ? "" : staff.FirstOrDefault(s => s.StaffUserName.ToLower() == r.CreatedBy.ToLower()).StaffNameTh;
                    r.UpdaterName = staff.FirstOrDefault(s => s.StaffUserName == r.UpdatedBy) == null ? "" : staff.FirstOrDefault(s => s.StaffUserName.ToLower() == r.UpdatedBy.ToLower()).StaffNameTh;

                    var rr = receiveNoList.Where(i => r.ReceiveNoId == i.slm_ReceiveNoId).ToList();

                    r.ReceiveNoTotal = 0;
                    r.ReceiveNoUsed = 0;
                    r.ReceiveNoCancel = 0;
                    r.ReceiveNoRemain = 0;
                    if (rr != null)
                    {
                        rr.ForEach(t =>
                        {
                            r.ReceiveNoTotal += t.total;
                            switch (t.slm_Status)
                            {
                                case "0": r.ReceiveNoRemain = t.total; break;
                                case "1": r.ReceiveNoUsed = t.total; break;
                                case "2": r.ReceiveNoCancel = t.total; break;
                                default:
                                    break;
                            }
                        });
                    }
                });
            }

            return result.OrderBy(r => r.IsDeleted == true ? 1 : 0).ThenByDescending(r => r.UpdatedDate).ToList();
        }

        public static bool SaveReceiveData(ReceiveNoData data, out string message)
        {
            message = "";
            using (OPERDBEntities operdb = new Dal.OPERDBEntities())
            {

                var existing = operdb.kkslm_ms_receiveno.Where(r => r.slm_Product_Id == data.ProductId
                    && r.slm_Ins_Com_Id == data.InsComId
                    // && r.slm_CodeName == data.CodeName -- fix for request 4013-00017 
                    )
                    .ToList();


                // check overlap between start-end receive no
                var dup = existing.Where(e => e.is_Deleted == false && e.slm_CodeName == data.CodeName &&
                         ((e.slm_ReceiveNoStart <= data.ReceiveNoStart && data.ReceiveNoStart <= e.slm_ReceiveNoEnd)
                            || (e.slm_ReceiveNoStart <= data.ReceiveNoEnd && data.ReceiveNoEnd <= e.slm_ReceiveNoEnd))
                        ).ToList();
                if (dup.Any())
                {
                    message = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากซ้ำกับในระบบ";
                    return false;
                }

                //2016-11-03 : don't auto change status
                //var activeItem = existing.Where(e => e.is_Deleted == false).ToList();
                //if (activeItem != null)
                //{
                //    activeItem.ForEach(a =>
                //    {
                //        a.is_Deleted = true;
                //        var list = operdb.kkslm_ms_receivenolist.Where(kk => kk.slm_ReceiveNoId == a.slm_ReceiveNoId).ToList();
                //        list.ForEach(l => l.is_Deleted = true);
                //    });
                //}

                int lot = existing.Count == 0 ? 1 : (existing.Max(e => e.slm_Lot) + 1);
                decimal nextId = operdb.kkslm_ms_receiveno.Select(r => r.slm_ReceiveNoId).DefaultIfEmpty(0).Max();
                nextId++;

                // insert 
                kkslm_ms_receiveno newItem = new Dal.kkslm_ms_receiveno()
                {
                    is_Deleted = true,
                    //slm_Cancel = 0,               //Comment  by Pom on 29/04/2016
                    slm_CodeName = data.CodeName,
                    slm_CreatedBy = data.UpdatedBy,
                    slm_CreatedDate = data.UpdatedDate,
                    slm_Ins_Com_Id = data.InsComId.Value,
                    slm_Lot = lot,
                    slm_Product_Id = data.ProductId,
                    slm_ReceiveNoEnd = data.ReceiveNoEnd.Value,
                    slm_ReceiveNoId = nextId,
                    slm_ReceiveNoStart = data.ReceiveNoStart.Value,
                    //slm_Remain = data.ReceiveNoEnd - data.ReceiveNoStart + 1,         //Comment  by Pom on 29/04/2016
                    //slm_Total = data.ReceiveNoEnd - data.ReceiveNoStart + 1,          //Comment  by Pom on 29/04/2016
                    slm_UpdatedBy = data.UpdatedBy,
                    slm_UpdatedDate = data.UpdatedDate,
                    //slm_Used = 0                                                      //Comment  by Pom on 29/04/2016
                };
                operdb.kkslm_ms_receiveno.AddObject(newItem);
                operdb.SaveChanges();

                for (decimal i = data.ReceiveNoStart.Value; i <= data.ReceiveNoEnd; i++)
                {
                    kkslm_ms_receivenolist newListItem = new kkslm_ms_receivenolist()
                    {
                        slm_ReceiveNoId = newItem.slm_ReceiveNoId,
                        slm_Product_Id = newItem.slm_Product_Id,
                        slm_Ins_Com_Id = newItem.slm_Ins_Com_Id,
                        slm_ReceiveNo = newItem.slm_CodeName + i.ToString(),
                        slm_Status = "0",
                        // slm_TicketId,
                        // slm_UseDate,
                        // slm_UseBy,
                        slm_CreatedBy = data.UpdatedBy,
                        slm_CreatedDate = data.UpdatedDate,
                        slm_UpdatedBy = data.UpdatedBy,
                        slm_UpdatedDate = data.UpdatedDate,
                        is_Deleted = true

                    };
                    operdb.kkslm_ms_receivenolist.AddObject(newListItem);
                    if (i % 500 == 0) operdb.SaveChanges();
                }
                operdb.SaveChanges();
                message = "บันทึกข้อมูลเรียบร้อย";
                return true;
            }

        }

        public static void UpdateDeleleFlag(decimal id, bool isdelete, string username)
        {
            try
            {
                using (OPERDBEntities operdb = new Dal.OPERDBEntities())
                {
                    var item = operdb.kkslm_ms_receiveno.FirstOrDefault(r => r.slm_ReceiveNoId == id);

                    //Comment on 29/04/2016
                    //if (item.slm_Remain == 0 && isdelete == false)                    
                    //{
                    //    return "หมายเลขรับแจ้งถูกใช้งานหมดแล้ว ไม่สามารถเปลี่ยนสถานะได้";
                    //}

                    if (item != null)
                    {
                        DateTime updateTime = DateTime.Now;
                        item.is_Deleted = isdelete;
                        item.slm_UpdatedDate = updateTime;
                        item.slm_UpdatedBy = username;

                        var receiveNoList = operdb.kkslm_ms_receivenolist.FirstOrDefault(r => r.slm_ReceiveNoId == item.slm_ReceiveNoId);
                        if (receiveNoList != null)
                        {
                            operdb.ExecuteStoreCommand(string.Format(" update kkslm_ms_receivenolist set Is_Deleted = {0}, slm_UpdatedBy = '{1}', slm_UpdatedDate = '{2}' where slm_ReceiveNoId = {3}", isdelete == true ? 1 : 0, username, updateTime.Year.ToString() + updateTime.ToString("-MM-dd HH:mm:ss"), item.slm_ReceiveNoId));
                        }
                        else
                        {
                            for (decimal i = item.slm_ReceiveNoStart; i <= item.slm_ReceiveNoEnd; i++)
                            {
                                kkslm_ms_receivenolist newListItem = new kkslm_ms_receivenolist()
                                {
                                    slm_ReceiveNoId = item.slm_ReceiveNoId,
                                    slm_Product_Id = item.slm_Product_Id,
                                    slm_Ins_Com_Id = item.slm_Ins_Com_Id,
                                    slm_ReceiveNo = item.slm_CodeName + i.ToString(),
                                    slm_Status = "0",
                                    // slm_TicketId,
                                    // slm_UseDate,
                                    // slm_UseBy,
                                    slm_CreatedBy = username,
                                    slm_CreatedDate = updateTime,
                                    slm_UpdatedBy = username,
                                    slm_UpdatedDate = updateTime,
                                    is_Deleted = isdelete

                                };
                                operdb.kkslm_ms_receivenolist.AddObject(newListItem);
                            }
                        }
                        operdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static bool HasLotInUse(string productId, decimal insComId)
        {
            try
            {
                using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
                {
                    var count = operdb.kkslm_ms_receiveno.Where(p => p.slm_Product_Id == productId && p.slm_Ins_Com_Id == insComId && p.is_Deleted == false).Count();
                    return count > 0 ? true : false;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
