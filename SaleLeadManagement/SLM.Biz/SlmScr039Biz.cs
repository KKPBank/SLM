using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using LinqKit;

namespace SLM.Biz
{
    public class SlmScr039Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }


        public object[] GetDataList(string _brand, string _model, int _type, string _status)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var whr = PredicateBuilder.True<kkslm_ms_mapping_brand_model_inscartype>();
                if (_brand.Trim() != "") whr = whr.And(m => m.slm_BrandCode == _brand.Trim());
                if (_model.Trim() != "") whr = whr.And(m => m.slm_ModelCode == _model.Trim());
                if (_type != 0) whr = whr.And(m => m.slm_InsuranceCarTypeId == _type);
                if (_status != "") whr = whr.And(c => c.is_Deleted == (_status == "1"));

                return (from mm in sdc.kkslm_ms_mapping_brand_model_inscartype.AsExpandable().Where(whr)
                        join br in sdc.kkslm_ms_redbook_brand on mm.slm_BrandCode equals br.slm_BrandCode
                        join md in sdc.kkslm_ms_redbook_model on new { mm.slm_ModelCode, br.slm_BrandCode } equals new { md.slm_ModelCode, md.slm_BrandCode }
                        join ct in sdc.kkslm_ms_insurancecartype on mm.slm_InsuranceCarTypeId equals ct.slm_InsurancecarTypeId
                        select new
                        {
                            mm.slm_BrandCode,
                            mm.slm_ModelCode,
                            mm.slm_InsuranceCarTypeId,
                            mm.slm_CreatedBy,
                            mm.slm_CreatedDate,
                            mm.slm_UpdatedBy,
                            mm.slm_UpdatedDate,
                            mm.is_Deleted,
                            br.slm_BrandName,
                            md.slm_ModelName,
                            ct.slm_InsurancecarTypeName

                        }).OrderBy(m=>m.slm_BrandName).ThenBy(m=>m.slm_ModelName).ToArray();
            }
        }

        public bool SaveData(kkslm_ms_mapping_brand_model_inscartype map, string userID, bool isedit = false)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    if (!isedit && sdc.kkslm_ms_mapping_brand_model_inscartype.Where(m => m.slm_ModelCode == map.slm_ModelCode).Count() > 0) throw new Exception("รุ่นรถถูกกำหนดแล้ว");
                    var mapu = sdc.kkslm_ms_mapping_brand_model_inscartype.Where(m => m.slm_ModelCode == map.slm_ModelCode).FirstOrDefault();
                    if (mapu == null)
                    {
                        mapu = map;
                        mapu.slm_CreatedBy = userID;
                        mapu.slm_CreatedDate = DateTime.Now;

                        sdc.kkslm_ms_mapping_brand_model_inscartype.AddObject(mapu);
                    }
                    else
                    {
                        mapu.slm_ModelCode = map.slm_ModelCode;
                        mapu.slm_BrandCode = map.slm_BrandCode;
                        mapu.slm_InsuranceCarTypeId = map.slm_InsuranceCarTypeId;
                        mapu.is_Deleted = map.is_Deleted;
                    }

                    mapu.slm_UpdatedBy = userID;
                    mapu.slm_UpdatedDate = DateTime.Now;

                    sdc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.Message;
            }
            return ret;
        }

        public kkslm_ms_mapping_brand_model_inscartype GetDetails(string modelCode)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_mapping_brand_model_inscartype.Where(m => m.slm_ModelCode == modelCode).FirstOrDefault();
            }
        }


        // combo
        public static List<ControlListData> GetModelDataList(string brandCode)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_redbook_model.Where(m=>m.slm_BrandCode == brandCode).ToList().OrderBy(m => m.slm_ModelName).Select(m => new ControlListData() { TextField = m.slm_ModelName, ValueField = m.slm_ModelCode }).ToList();
            }
        }

        public static List<ControlListData> GetBrandDataList()
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_redbook_brand.ToList().OrderBy(m => m.slm_BrandName).Select(b => new ControlListData() { TextField = b.slm_BrandName, ValueField = b.slm_BrandCode }).ToList();
            }
        }

        public static List<ControlListData> GetTypeDataList()
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_insurancecartype.Where(i => i.is_Deleted == false).ToList().OrderBy(i => i.slm_InsurancecarTypeName).Select(i => new ControlListData() { TextField = i.slm_InsurancecarTypeName, ValueField = i.slm_InsurancecarTypeId.ToString() }).ToList();
            }
        }


    }
}
