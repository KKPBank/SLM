using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource;

namespace SLM.Dal
{
    public class DBUtil
    {
        public static SLM_DBEntities GetSlmDbEntities()
        {
            SLM_DBEntities slmdb = new SLM_DBEntities();
            slmdb.CommandTimeout = SLMConstant.SLMDBCommandTimeout;
            return slmdb;
        }

        public static OPERDBEntities GetOperDbEntities()
        {
            OPERDBEntities operdb = new OPERDBEntities();
            operdb.CommandTimeout = SLMConstant.SLMDBCommandTimeout;
            return operdb;
        }
    }
}
