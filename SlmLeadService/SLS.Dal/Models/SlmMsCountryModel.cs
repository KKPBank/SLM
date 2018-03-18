using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Dal.Models
{
    public class SlmMsCountryModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmMsCountryModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public bool CheckCountryCodeExist(string countryCode)
        {
            var num = slmdb.kkslm_ms_country.Count(p => p.slm_CountryCode == countryCode && p.is_delete == 0);
            return num > 0 ? true : false;
        }
    }
}
