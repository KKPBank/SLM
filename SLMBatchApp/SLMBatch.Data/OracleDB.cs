using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace SLMBatch.Data
{
    public class OracleDB : IDisposable
    {
        private string _conStr = "";
        private OracleConnection _conn = null;

        public OracleDB(string connectionString)
        {
            _conStr = connectionString;
            CreateConnection();
        }

        public OracleConnection CreateConnection()
        {
            _conn = new OracleConnection(_conStr);
            _conn.Open();
            return _conn;
        }

        public void CloseConnection()
        {
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
            }
        }

        public DataTable ExcecuteTable(string sql)
        {
            OracleDataAdapter oda = null;
            try
            {
                if (_conn == null) 
                    CreateConnection();

                DataSet ds = new DataSet();

                oda = new OracleDataAdapter(sql, _conn);
                oda.Fill(ds, "TABLE1");

                return ds.Tables["TABLE1"];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oda != null)
                {
                    oda.Dispose();
                }
            }
        }

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _conn = null;
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
