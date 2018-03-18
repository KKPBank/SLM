using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class InsuredData
    {

        //**************************************TableName : kkslm_ms_ins_com **********************************************
        public decimal CompanyId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string InsuredCode { get; set; }
        public string InsuredAbbreviation { get; set; }
        public string InsuredNameEng { get; set; }
        public string InsuredNameTh { get; set; }
        public string InsuredType { get; set; }
        public string InsusredTax { get; set; }
        public string Tel { get; set; }
        public string TelContact { get; set; }
        public string AddressNo { get; set; }
        public string Moo { get; set; }
        public string BuildingName { get; set; }
        public string Floor { get; set; }
        public string Soi { get; set; }
        public string Road { get; set; }
        public int? ProvinceId { get; set; }
        public int? AmphurId { get; set; }
        public int? TambolId { get; set; }
        public string PostCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool is_Deleted { get; set; }

        public string ProvinceName { get; set; }
        public string TambonName { get; set; }
        public string AmphurName { get; set; }

    }

}
