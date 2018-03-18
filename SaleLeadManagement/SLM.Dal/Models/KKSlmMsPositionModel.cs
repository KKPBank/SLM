using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsPositionModel
    {
        /// <summary>
        /// GetPositionList Flag 1=Active Branch, 2=Inactive Branch, 3=All
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<ControlListData> GetPositionList(int flag)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                List<ControlListData> list = null;

                if (flag == SLMConstant.Position.Active)
                    list = slmdb.kkslm_ms_position.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_PositionNameTH).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_PositionNameTH, ValueField = p.slm_Position_id.ToString() }).ToList();
                else if (flag == SLMConstant.Position.InActive)
                    list = slmdb.kkslm_ms_position.Where(p => p.is_Deleted == true).OrderBy(p => p.slm_PositionNameTH).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_PositionNameTH, ValueField = p.slm_Position_id.ToString() }).ToList();
                else if (flag == SLMConstant.Position.All)
                    list = slmdb.kkslm_ms_position.OrderBy(p => p.slm_PositionNameTH).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_PositionNameTH, ValueField = p.slm_Position_id.ToString() }).ToList();
                else
                    list = new List<ControlListData>();

                return list;
            }
        }

        public List<PositionData> SearchPosition(string positionNameAbb, string positionNameEN, string positionNameTH, bool statusActive, bool statusInActive)
        {
            string sql = @"SELECT slm_Position_id AS PositionId, slm_PositionNameAbb AS PositionNameAbb, slm_PositionNameEN AS PositionNameEN
                            , slm_PositionNameTH AS PositionNameTH
                            ,CASE WHEN is_Deleted = '0' THEN 'ใช้งาน'
                                  WHEN is_Deleted = '1' THEN 'ไม่ใช้งาน'
                                  ELSE '' END AS StatusDesc
                            , CASE WHEN is_Deleted = '0' THEN 'Y'
	                               WHEN is_Deleted = '1' THEN 'N'
	                               ELSE '' END AS [Status]
                            FROM " + SLMConstant.SLMDBName + ".dbo.kkslm_ms_position ";

            string whr = "";

            whr += (positionNameAbb == "" ? "" : (whr == "" ? "" : " AND ") + " slm_PositionNameAbb LIKE @name_abb ");
            whr += (positionNameEN == "" ? "" : (whr == "" ? "" : " AND ") + " slm_PositionNameEN LIKE @name_en ");
            whr += (positionNameTH == "" ? "" : (whr == "" ? "" : " AND ") + " slm_PositionNameTH LIKE @name_th ");

            if (statusActive == true && statusInActive == false)
                whr += (whr == "" ? "" : " AND ") + " is_Deleted = '0' ";
            else if (statusActive == false && statusInActive == true)
                whr += (whr == "" ? "" : " AND ") + " is_Deleted = '1' ";

            if (whr != "")
                sql += " WHERE " + whr;

            sql += " ORDER BY slm_PositionNameAbb ";

            object[] param = new object[] 
            { 
                new SqlParameter("@name_abb", "%" + positionNameAbb + "%"),
                new SqlParameter("@name_en", "%" + positionNameEN + "%"),
                new SqlParameter("@name_th", "%" + positionNameTH + "%")
            };

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<PositionData>(sql, param).ToList();
            }
        }

        public void UpdateData(int positionId, string positionNameAbb, string positionNameEN, string positionNameTH, bool isActive, string updatedBy)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var count = slmdb.kkslm_ms_position.Where(p => p.slm_PositionNameAbb == positionNameAbb && p.slm_Position_id != positionId).Count();
                    if (count > 0)
                        throw new Exception("ชื่อย่อ " + positionNameAbb + " มีในระบบแล้ว");

                    count = slmdb.kkslm_ms_position.Where(p => p.slm_PositionNameTH == positionNameTH && p.slm_Position_id != positionId).Count();
                    if (count > 0)
                        throw new Exception("ชื่อตำแหน่ง " + positionNameTH + " มีในระบบแล้ว");

                    var position = slmdb.kkslm_ms_position.Where(p => p.slm_Position_id == positionId).FirstOrDefault();
                    if (position != null)
                    {
                        position.slm_PositionNameAbb = positionNameAbb;
                        position.slm_PositionNameEN = string.IsNullOrEmpty(positionNameEN) ? null : positionNameEN;
                        position.slm_PositionNameTH = positionNameTH;
                        position.slm_UpdatedBy = updatedBy;
                        position.slm_UpdatedDate = DateTime.Now;
                        position.is_Deleted = !isActive;

                        slmdb.SaveChanges();
                    }
                    else
                        throw new Exception("ไม่พบ PositionId " + positionId.ToString() + " ในระบบ");
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertData(string positionNameAbb, string positionNameEN, string positionNameTH, bool isActive, string createdBy)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var count = slmdb.kkslm_ms_position.Where(p => p.slm_PositionNameAbb == positionNameAbb).Count();
                    if (count > 0)
                        throw new Exception("ชื่อย่อ " + positionNameAbb + " มีในระบบแล้ว");

                    count = slmdb.kkslm_ms_position.Where(p => p.slm_PositionNameTH == positionNameTH).Count();
                    if (count > 0)
                        throw new Exception("ชื่อตำแหน่ง " + positionNameTH + " มีในระบบแล้ว");

                    DateTime createdDate = DateTime.Now;
                    kkslm_ms_position position = new kkslm_ms_position()
                    {
                        slm_PositionNameAbb = positionNameAbb,
                        slm_PositionNameEN = positionNameEN,
                        slm_PositionNameTH = positionNameTH,
                        slm_CreatedBy = createdBy,
                        slm_CreatedDate = createdDate,
                        slm_UpdatedBy = createdBy,
                        slm_UpdatedDate = createdDate,
                        is_Deleted = !isActive
                    };
                    slmdb.kkslm_ms_position.AddObject(position);

                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
