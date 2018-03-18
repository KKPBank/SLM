using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource.Data;
using LinqKit;

namespace SLM.Biz
{
    public class SlmScr063Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        public object[] GetDataList(string _brand, string _status)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var whr = PredicateBuilder.True<kkslm_ms_config_car_premium>();
                if (_brand.Trim() != "")
                {
                    var blst = sdc.kkslm_ms_redbook_brand.Where(b => b.slm_BrandName.Contains(_brand.Trim())).Select(b=>b.slm_BrandCode).ToList();
                    whr = whr.And(c => blst.Contains(c.slm_Brand_Code));
                }
                if (_status != "") whr = whr.And(c => c.is_Deleted == (_status == "1"));

                return (from cp in sdc.kkslm_ms_config_car_premium.AsExpandable().Where(whr)
                        join br in sdc.kkslm_ms_redbook_brand on cp.slm_Brand_Code equals br.slm_BrandCode
                        select new {
                            cp.slm_cc_pm_id,
                            cp.slm_Brand_Code,
                            cp.is_Deleted,
                            cp.slm_CreatedBy,
                            cp.slm_CreatedDate,
                            cp.slm_UpdatedBy, 
                            cp.slm_UpdatedDate,
                            br.slm_BrandName,

                        }).OrderBy(b=>b.slm_BrandName).ToArray(); 
            }

        }

        public kkslm_ms_config_car_premium GetDetail(decimal id)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_config_car_premium.Where(c => c.slm_cc_pm_id == id).FirstOrDefault();
            }
        }

        public bool SaveData(kkslm_ms_config_car_premium cp, string userId)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var cpu = sdc.kkslm_ms_config_car_premium.Where(c => c.slm_cc_pm_id == cp.slm_cc_pm_id).FirstOrDefault();
                    if (cpu == null)
                    {
                        if (sdc.kkslm_ms_config_car_premium.Where(c => c.slm_Brand_Code == cp.slm_Brand_Code).Count() > 0) throw new Exception("ยี่ห้อรถถูกกำหนดแล้ว");
                        cpu = cp;
                        cpu.slm_CreatedBy = userId;
                        cpu.slm_CreatedDate = DateTime.Now;

                        sdc.kkslm_ms_config_car_premium.AddObject(cpu);

                    }
                    else
                    {
                        //cpu.slm_Brand_Code = cp.slm_Brand_Code;
                        cpu.is_Deleted = cp.is_Deleted;

                    }

                    cpu.slm_UpdatedBy = userId;
                    cpu.slm_UpdatedDate = DateTime.Now;

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

        public static List<ControlListData> GetBrandData()
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_redbook_brand.OrderBy(b => b.slm_BrandName).ToList().Select(b => new ControlListData() { TextField = b.slm_BrandName, ValueField = b.slm_BrandCode }).ToList();
            }
        }

    }
}
