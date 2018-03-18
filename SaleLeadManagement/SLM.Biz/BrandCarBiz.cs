using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class BrandCarBiz
    {

        public static List<BrandCarData> ListBrandCar()
        {
            KKSlmMsBrandModel brandcar = new KKSlmMsBrandModel();
            return brandcar.ListBrandCar();
        }

        public static List<BrandCarData> ListCodeCar(string carBrandValue)
        {
            KKSlmMsBrandModel brandcar = new KKSlmMsBrandModel();
            return brandcar.ListCodeCar(carBrandValue);
        }

        public static List<ControlListData> ListSubModel(string carBrandValue, string carModelValue)
        {
            KKSlmMsBrandModel brandcar = new KKSlmMsBrandModel();
            return brandcar.ListSubModel(carBrandValue, carModelValue);
        }

        //Add 26/07/2017
        public static List<ControlListData> ListModelYear(string brandCode, string modelCode)
        {
            KKSlmMsBrandModel brandcar = new KKSlmMsBrandModel();
            return brandcar.ListModelYear(brandCode, modelCode);
        }
    }
}
