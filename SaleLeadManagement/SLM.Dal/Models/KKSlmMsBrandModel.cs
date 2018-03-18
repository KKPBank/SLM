using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsBrandModel
    {
        public List<ControlListData> GetBrandData()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_brand.OrderBy(p => p.slm_BrandName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_BrandName, ValueField = p.slm_BrandCode }).ToList();
            }
        }
        public int? GetBrandId(string BrandCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_brand.Where(p => p.slm_BrandCode == BrandCode).Select(p => p.slm_BrandId).FirstOrDefault();
            }
        }
        public List<BrandCarData> ListBrandCar()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"select slm_Brandcode AS BrandCode,slm_brandname AS BrandName from kkslm_ms_redbook_brand";

                return slmdb.ExecuteStoreQuery<BrandCarData>(sql).ToList();
            }
        }

        public List<BrandCarData> ListCodeCar(string carBrandValue)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = @"select slm_ModelCode AS ModelCode,slm_ModelName AS ModelName from kkslm_ms_redbook_model WITH (NOLOCK) where slm_BrandCode ='" + carBrandValue + "'";

                return slmdb.ExecuteStoreQuery<BrandCarData>(sql).ToList();
            }
        }

        public List<ControlListData> ListSubModel(string carBrandValue, string carModelValue)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                List<ControlListData> list = null;
                list = slmdb.kkslm_ms_redbook_submodel.Where(p => p.slm_BrandCode == carBrandValue && p.slm_ModelCode == carModelValue).OrderBy(p => p.slm_Description).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_Description, ValueField = p.slm_VehicleKey.ToString() }).ToList();

                return list;
            }
        }

        //Add 26/07/2017
        public List<ControlListData> ListModelYear(string brandCode, string modelCode)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_redbook_model_year.Where(p => p.slm_BrandCode == brandCode && p.slm_ModelCode == modelCode && p.slm_YearGroup != null).OrderBy(p => p.slm_YearGroup).Select(p => p.slm_YearGroup).Distinct().AsEnumerable()
                        .Select(p => new ControlListData { TextField = p.ToString(), ValueField = p.ToString() }).ToList();
            }
        }


    }
}
