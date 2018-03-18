using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource.Data
{
    public class Mandatory
    {
        public string Firstname { get; set; }
        public string TelNo1 { get; set; }
        public string Campaign { get; set; }

        //2017-03-29 ใช้เพื่อเช็กว่าต้องการทำ Auto Assign แทน Rule Engine หรือไม่
        public bool AutoRuleAssign { get; set; }
    }
}
