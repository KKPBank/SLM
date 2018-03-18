using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class CountryBiz
    {
        public static List<ControlListData> GetCountryList()
        {
            KKSlmMsCountryModel country = new KKSlmMsCountryModel();
            return country.GetCountryList();
        }
        public static CountryData GetCountryCodeById(int countryId)
        {
            KKSlmMsCountryModel country = new KKSlmMsCountryModel();
            return country.GetCountryDataById(countryId);
        }
    }
}
