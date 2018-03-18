using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Data
{
    public class SLM_PRO_08_DataAccess : IDisposable
    {
        private SLMDBEntities _DataAccess = AppUtil.GetSlmDbEntities();



        #region "IDisposable"
        private bool _disposed = false;
        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _DataAccess = null;
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
