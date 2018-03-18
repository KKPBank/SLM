using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsBankModel
    {
        public BankData GetBankDataByNo(string bankNo)
        {
            SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities();
            var query = from b in slmdb.kkslm_ms_bank
                        where b.BankNo==bankNo
                        select new BankData
                        {
                            BankId = b.BankId,
                            BankNo = b.BankNo,
                            BankName = b.BankName
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }
    }
}
