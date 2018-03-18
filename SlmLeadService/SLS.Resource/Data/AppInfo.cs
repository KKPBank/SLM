using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource.Data
{
    public class AppInfo
    {
        public string System { get; set; }
        public string AppNo { get; set; }
        public string LastOwner { get; set; }
        public string StatusBy { get; set; }
        public DateTime StatusDate { get; set; }
        public string CurrentTeam { get; set; }
        public string FlowType { get; set; }
        public string COCType { get; set; }
        public string RoutebackTeam { get; set; }
        public string MarketingOwner { get; set; }
        public string COCStatus { get; set; }
        public string COCSubStatus { get; set; }
        public string ContactDetail { get; set; }
        public bool UpdateAppInfoFlag { get; set; }    //ใช้เช็กว่า HPAOFL ต้องการ update ข้อมูล lead อย่างเดียวหรือทั้งข้อมูล lead และ appinfo
    }
}
