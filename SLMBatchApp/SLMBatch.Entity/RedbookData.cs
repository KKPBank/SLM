using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
{
    public class RedbookData
    {
        
    }

    public class BrandData
    {
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
    }

    //public class ModelData
    //{
    //    public string BrandCode { get; set; }
    //    public string ModelCode { get; set; }
    //    public string ModelName { get; set; }
    //}

    public class SubModelData
    {
        public string KKKey { get; set; }
        public string ModelCode { get; set; }
    }
}
