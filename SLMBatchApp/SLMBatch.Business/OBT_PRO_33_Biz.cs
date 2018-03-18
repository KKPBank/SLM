using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using SLMBatch.Common;
using SLMBatch.Data;

namespace SLMBatch.Business
{
    public class OBT_PRO_33_Biz : OBT_PRO_32_Biz
    {
        //        private DateTime _dbNow;

        public OBT_PRO_33_Biz()
        {
            DA = new OBT_PRO_33_DataAccess();
            SMSTemplatePath = AppConstant.SMSTemplatePathPaymentDueShort;
        }

        ~OBT_PRO_33_Biz()
        {
            DA.Dispose();
        }
    }
}
