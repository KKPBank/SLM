using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SLS.Dal.Models
{
    public class CocMsSystemModel : IDisposable
    {
        #region Member
        private SLM_DBEntities slmdb = null;
        bool disposed = false;
        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                slmdb = null;

            }
            disposed = true;
        }
        #endregion

        public CocMsSystemModel()
        {
            slmdb = new SLM_DBEntities();
        }

        public CocMsSystemModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public bool ValidateSystem(string systemName)
        {
            var count = slmdb.kkcoc_ms_system.Count(p => p.coc_SystemName == systemName && p.is_Deleted == 0);
            return count > 0 ? true : false;
        }
    }
}
