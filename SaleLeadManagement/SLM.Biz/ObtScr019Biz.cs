using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class ObtScr019Biz
    {
        public static List<InsuredData> GetSearchInsuredData(string product, string code, string name, string type)
        {
            OperKKSlmMsInsComModel ins = new OperKKSlmMsInsComModel();
            var productList = new CmtMappingProductModel().GetProductList();
            var result = ins.GetSearchList(product, code, name, type);
            //result.ForEach(i => i.ProductName = productList.FirstOrDefault(p => p.ValueField == i.ProductId).TextField);
            return result;
        }

        public static InsuredData GetEditInsuredData(decimal comId)
        {
            OperKKSlmMsInsComModel ins = new OperKKSlmMsInsComModel();
            return ins.GetObjectById(comId);
        }

        public static List<ControlListData> GetTambolData(string provinceCode, string amphurCode, bool useWebservice)
        {
            KKSlmMsTambolModel tambol = new KKSlmMsTambolModel();
            return tambol.GetTambolData(provinceCode, amphurCode, useWebservice);
        }

        public static List<ControlListData> GetAmphurData(string provinceCode)
        {
            KKSlmMsAmphurModel amphur = new KKSlmMsAmphurModel();

            return amphur.GetAmphurData(provinceCode);

        }

        public static List<ControlListData> GetProvinceData()
        {
            KKSlmMsProvinceModel province = new KKSlmMsProvinceModel();
            return province.GetProvinceData();
        }

        public static void SaveInsuredData (InsuredData data)
        {
            OperKKSlmMsInsComModel ins = new OperKKSlmMsInsComModel();
            ins.InsertOrUpdate(data);
        }

        public static bool IsDuplicateComCode(decimal comId, string comCode, string productId)
        {
            OperKKSlmMsInsComModel ins = new OperKKSlmMsInsComModel();
            var searchResult = ins.GetSearchByComCodeList(productId, "", comCode, "");
            if (searchResult != null &&
                searchResult.Where(kk => kk.InsuredCode == comCode && kk.CompanyId != comId).Count() > 0)
                return true;
            return false;
        }
        public static bool IsDuplicateComName(decimal comId, string comName, string productId)
        {
            OperKKSlmMsInsComModel ins = new OperKKSlmMsInsComModel();
            var searchResult = ins.GetSearchByComNameList(productId, "", comName, "");
            if (searchResult != null &&
                searchResult.Where(kk => kk.InsuredCode == comName && kk.CompanyId != comId).Count() > 0)
                return true;
            return false;
        }

        public static bool IsDuplicateAbbName(decimal comId, string abbName)
        {
            using (OPERDBEntities odc = new OPERDBEntities())
            {
                return odc.kkslm_ms_ins_com.Where(c => c.slm_Ins_Com_Id != comId && c.slm_InsABB == abbName).Count() > 0;
            }
        }
    }
}
