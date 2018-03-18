using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsCountryModel
    {
        public List<ControlListData> GetCountryList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_country.Where(p => p.is_delete==0).OrderBy(p => p.slm_CountryDescriptionTH).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_CountryDescriptionTH, ValueField = p.slm_country_id.ToString() }).ToList();
            }
        }

        public CountryData GetCountryDataById(int countryId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var ret = slmdb.kkslm_ms_country.Where(p => p.is_delete == 0 && p.slm_country_id == countryId).AsEnumerable()
                    .Select(p => new CountryData
                    {
                        CountryId = p.slm_country_id,
                        CountryCode = p.slm_CountryCode,
                        CountryNameEn = p.slm_CountryDescriptionEN,
                        CountryNameTh = p.slm_CountryDescriptionTH
                    });

                return (ret.Any() ? ret.FirstOrDefault() : new CountryData() { CountryCode = "" });
            }
        }
    }
}
