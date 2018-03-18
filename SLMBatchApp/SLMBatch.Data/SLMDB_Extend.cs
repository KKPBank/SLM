using System;
using System.Linq;

namespace SLMBatch.Data
{
    public partial class SLMDBEntities
    {
        public DateTime DBNow()
        {
            return ExecuteStoreQuery<DateTime>("SELECT getdate()").First();
        }
    }
}
