using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class ProductBiz
    {
        public static List<ControlListData> GetProductList()
        { 
            CmtMappingProductModel product = new CmtMappingProductModel();
            return product.GetProductList();
        }

        public static List<ControlListData> GetProductListNew()
        {
            CmtMappingProductModel product = new CmtMappingProductModel();
            return product.GetProductListNew();
        } 
    }
}
