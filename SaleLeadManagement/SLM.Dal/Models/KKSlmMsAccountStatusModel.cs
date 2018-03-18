using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmMsAccountStatusModel
    {
        public List<AccountStatusData> GetAccountStatusList()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                List<AccountStatusData> ret = (from c in slmdb.kkslm_ms_account_status
                                               select new AccountStatusData
                                               {
                                                   AccountStatusId = c.AccountStatusId,
                                                   AccountStatusCode = c.AccountStatusCode,
                                                   AccountStatusName = c.AccountStatusName,
                                                   AccountTypeCode = c.AccountTypeCode
                                               }).ToList();

                return ret;
            }
        }

        public AccountStatusData GetAccountStatusByCode(string accountStatus)
        {
            SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities();
            var query = from a in slmdb.kkslm_ms_account_status
                        where a.AccountStatusCode == accountStatus
                        select new AccountStatusData
                        {
                            AccountStatusId = a.AccountStatusId,
                            AccountStatusCode = a.AccountStatusCode,
                            AccountStatusName = a.AccountStatusName,
                            AccountTypeCode = a.AccountTypeCode
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }
    }
}
