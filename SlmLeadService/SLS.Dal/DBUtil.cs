using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Dal
{
    public class DBUtil
    {
        public static SLM_DBEntities GetSlmDbEntities()
        {
            SLM_DBEntities slmdb = new SLM_DBEntities();
            slmdb.CommandTimeout = 0;
            return slmdb;
        }
    }
}
