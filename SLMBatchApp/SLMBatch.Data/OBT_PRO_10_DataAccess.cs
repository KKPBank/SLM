using SLMBatch.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SLMBatch.Entity;

namespace SLMBatch.Data
{
    public class OBT_PRO_10_DataAccess : IDisposable
    {
        private SLMDBEntities _DataAccess = AppUtil.GetSlmDbEntities();

        public int CalculatePerformance()
        {

            DataTable dt = Util.ToDataTable(_DataAccess.SP_SLMBatch_OBT_PRO_10_TelesalesPerformance().ToList());
            if(dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            return 0;
        }

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
