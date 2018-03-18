using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
{
    //ข้อมูลการขายประกันภัยและพ.ร.บ.
    public class SaleRenewInsuranceActData
    {
        public string TicketId { get; set; }
        public string ContractNo { get; set; }              //เลขที่สัญญา
        public string ReceiveNo { get; set; }               //เลขที่รับแจ้ง
        public DateTime? HVInfDate { get; set; }               //วันที่รับเลขรับแจ้ง
        public string EmpCode { get; set; }                 //รหัสพนักงาน
        public decimal? InsuranceComId { get; set; }          //บริษัทประกัน
        public string InsuranceComCode { get; set; }          //Code บริษัทประกัน
        public string PolicyStartDate { get; set; }      //วันเริ่มคุ้มครอง
        public string PolicyEndDate { get; set; }        //วันหมดอายุประกัน
        public decimal? PolicyDiscountAmt { get; set; }       //จำนวนส่วนลด
        public decimal? PolicyGrossPremium { get; set; }      //เบี้ยประกันที่ชำระ
        public int? CoverageTypeId { get; set; }          //รหัสประเภทประกันภัย
        public decimal? PolicyCost { get; set; }              //ทุนประกัน
        public int? RepairTypeId { get; set; }            //ประเภทการซ่อม
        public string RepairType { get; set; }
        public decimal? ActGrossPremium { get; set; }         //เบี้ย พรบ ที่ชำระ
        public decimal? ActComId { get; set; }                //บริษัท พรบ
        public string ActComCode { get; set; }              //Code บริษัท พรบ
        public string ActStartDate { get; set; }         //วันเริ่มต้น พรบ
        public string ActEndDate { get; set; }           //วันสิ้นสุด พรบ
        public decimal? ActNetPremium { get; set; }           //เบี้ยสุทธิ พรบ
        public decimal? ActVat { get; set; }                  //ภาษี พรบ
        public decimal? ActStamp { get; set; }                //อากร พรบ
        public decimal? HCFiller10 { get; set; }                  //เบี้ยประกันภัยรวมภาษีอากร
    }
}
