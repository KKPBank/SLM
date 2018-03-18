using System;
using System.Data;

namespace SLMBatch.Data
{
    public class SQLDataAccess : IDisposable
    {
        protected readonly SQLServerDB db;

        protected SQLDataAccess()
        {
            db = new SQLServerDB(AppUtil.SLMDBConnectionString);
        }

        public virtual DataTable LoadData()
        {
            throw new NotImplementedException();
        }

        public virtual void FeedbackData()
        {
        }

        #region Utility Query

        protected DateTime DBNow()
        {
            return db.ExecuteScalar<DateTime>("SELECT getdate()");
        }

        protected bool IsLateThanCutoff()
        {
            return db.ExecuteScalar<int>(
                       "SELECT datediff(SECOND, cast(getdate() AS DATE) + cast(slm_OptionDesc AS DATETIME), getdate()) AS CompareCutoff" +
                       " FROM kkslm_ms_option WHERE slm_OptionCode = 'endTime'"
                   ) > 0;
        }

        #endregion

        #region "IDisposable"

        private bool _disposed;

        private void ReleaseUnmanagedResources()
        {
            //            db?.CloseConnection();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                ReleaseUnmanagedResources();
                if (disposing)
                {
                    db?.CloseConnection();
                    db?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SQLDataAccess()
        {
            Dispose(false);
        }

        #endregion
    }
}
