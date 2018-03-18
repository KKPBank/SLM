using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsPaymentTypeModel
    {
        public List<ControlListData> GetPaymentTypeData(bool useWebservice)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_paymenttype.OrderBy(p => p.slm_PaymentNameTH).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_PaymentNameTH, ValueField = useWebservice ? p.slm_PaymentCode : p.slm_PaymentId.ToString() }).ToList();
            }
        }
    }
}
