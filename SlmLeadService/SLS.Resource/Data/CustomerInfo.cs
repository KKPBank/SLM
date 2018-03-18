using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource.Data
{
    public class CustomerInfo
    {
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string TelNo2 { get; set; }
        public string TelNo3 { get; set; }
        public string ExtNo1 { get; set; }
        public string ExtNo2 { get; set; }
        public string ExtNo3 { get; set; }
        public string BuildingName { get; set; }
        public string AddrNo { get; set; }
        public string Floor { get; set; }
        public string Soi { get; set; }
        public string Street { get; set; }
        public string Tambom { get; set; }
        public string Amphur { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Occupation { get; set; }
        public string BaseSalary { get; set; }
        public string IsCustomer { get; set; }
        public string CustomerCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string CardType { get; set; }
        public string Cid { get; set; }
        public string CountryCode { get; set; }
        public string Status { get; set; }
        public string SubStatus { get; set; }
        public string SubStatusDesc { get; set; }
        public DateTime StatusDateSource { get; set; }          //Added by Pom 15/03/2016
        public string StatusReSendFlag { get; set; }            //Added by Pom 15/03/2016
        public string ContractNoRefer { get; set; }
    }
}
