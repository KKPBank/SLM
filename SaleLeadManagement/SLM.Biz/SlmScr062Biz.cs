using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using LinqKit;


namespace SLM.Biz
{
    public class SlmScr062Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        public object[] GetDataList(int _role, bool _pol, bool _act, string _status)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var whr = PredicateBuilder.True<kkslm_ms_discount>();
                if (_role != 0) whr = whr.And(d => d.slm_StaffTypeId == _role);
                if (_pol == true && _act == false)
                    whr = whr.And(d => d.slm_InsuranceTypeCode == "204");
                if (_pol == false && _act == true)
                    whr = whr.And(d => d.slm_InsuranceTypeCode == "205");
                if (_pol && _act)
                    whr = whr.And(d => d.slm_InsuranceTypeCode == "204" || d.slm_InsuranceTypeCode == "205");
                if (_status != "") whr = whr.And(d => d.is_Deleted == (_status == "1"));

                return (from d in sdc.kkslm_ms_discount.AsExpandable().Where(whr)
                        join st in sdc.kkslm_ms_staff_type on d.slm_StaffTypeId equals st.slm_StaffTypeId into r1
                        from st in r1.DefaultIfEmpty()
                        where d.slm_StaffTypeId != -1
                        select new
                        {
                            st.slm_StaffTypeId,
                            st.slm_StaffTypeCode,
                            st.slm_StaffTypeDesc,
                            InsuranceType = d.slm_InsuranceTypeCode == "204" ? "ประกันภัยรถยนต์" : d.slm_InsuranceTypeCode == "205" ? "พรบ." : "",
                            d.slm_Discount_Id,
                            d.slm_DiscountPercent,
                            d.slm_DiscountBath,
                            //d.slm_ActDiscountBaht,
                            //d.slm_ActDiscountPercent,
                            d.slm_CreatedBy,
                            d.slm_CreatedDate,
                            d.slm_UpdatedBy,
                            d.slm_UpdatedDate,
                            d.is_Deleted

                        }).OrderBy(d => d.slm_StaffTypeDesc).ToArray();
            }
        }

        public kkslm_ms_discount GetDetail(int id)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_discount.Where(d => d.slm_Discount_Id == id).FirstOrDefault();
            }
        }

        public bool SaveData(kkslm_ms_discount ds, string userID)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var dsu = sdc.kkslm_ms_discount.Where(d => d.slm_Discount_Id == ds.slm_Discount_Id).FirstOrDefault();
                    if (dsu == null)
                    {
                        if (ds.is_Deleted == false && sdc.kkslm_ms_discount.Where(d => d.slm_StaffTypeId == ds.slm_StaffTypeId && d.slm_InsuranceTypeCode == ds.slm_InsuranceTypeCode && d.is_Deleted == false).Count() > 0) throw new Exception("Role และผลตอบแทนที่เลือกมีการตั้งค่าอยู่แล้ว กรุณาเลือกใหม่");
                        dsu = ds;
                        dsu.slm_CreatedBy = userID;
                        dsu.slm_CreatedDate = DateTime.Now;

                        sdc.kkslm_ms_discount.AddObject(dsu);
                    }
                    else
                    {
                        if (ds.is_Deleted == false && dsu.is_Deleted == true && sdc.kkslm_ms_discount.Where(d => d.slm_StaffTypeId == ds.slm_StaffTypeId && d.slm_InsuranceTypeCode == ds.slm_InsuranceTypeCode && d.is_Deleted == false).Count() > 0) throw new Exception("Role และผลตอบแทนที่เลือกมีการตั้งค่าอยู่แล้ว กรุณาเลือกใหม่");

                        dsu.slm_InsuranceTypeCode = ds.slm_InsuranceTypeCode;
                        dsu.slm_StaffTypeId = ds.slm_StaffTypeId;
                        //dsu.slm_ActDiscountBaht = ds.slm_ActDiscountBaht;
                        //dsu.slm_ActDiscountPercent = ds.slm_ActDiscountPercent;
                        dsu.slm_DiscountBath = ds.slm_DiscountBath;
                        dsu.slm_DiscountPercent = ds.slm_DiscountPercent;
                        dsu.is_Deleted = ds.is_Deleted;
                    }

                    dsu.slm_UpdatedBy = userID;
                    dsu.slm_UpdatedDate = DateTime.Now;

                    sdc.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ret = false;
            }
            return ret;

        }

        public static List<ControlListData> GetStaffTypeData(bool onlyActive, decimal oldvalue)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var whr = PredicateBuilder.True<kkslm_ms_staff_type>();
                if (onlyActive)
                {
                    if (oldvalue == 0) whr = whr.And(s => s.is_Deleted == 0);
                    else whr = whr.And(s => s.is_Deleted == 0 || s.slm_StaffTypeId == oldvalue);
                }
                return sdc.kkslm_ms_staff_type.AsExpandable().Where(whr).OrderByDescending(s=>s.is_Deleted).ThenBy(s => s.slm_StaffTypeDesc).ToList().Select(s => new ControlListData() { TextField = s.slm_StaffTypeDesc, ValueField = s.slm_StaffTypeId.ToString() }).ToList();
            }
        }

    }
}
