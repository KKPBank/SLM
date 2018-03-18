using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using SLMBatch.Common;

namespace SLMBatch.Data
{
    public class SQLServerDB : IDisposable
    {
        private readonly string _conStr = "";
        private SqlConnection _conn = null;

        public SQLServerDB(string connectionString)
        {
            _conStr = connectionString;
            CreateConnection();
        }

        public SqlConnection CreateConnection()
        {
            _conn = new SqlConnection(_conStr);
            _conn.Open();
            return _conn;
        }

        public void CloseConnection()
        {
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
                _conn = null;
            }
        }

        public int ExecuteNonQuery(string commandText)
        {
            if (_conn == null)
                CreateConnection();

            try
            {
                using (var comm = new SqlCommand(commandText, _conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = AppConstant.CommandTimeout
                })
                {
                    return comm.ExecuteNonQuery();
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        public T ExecuteScalar<T>(string commandText)
        {
            if (_conn == null)
                CreateConnection();

            try
            {
                using (var comm = new SqlCommand(commandText, _conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = AppConstant.CommandTimeout
                })
                {
                    return (T)comm.ExecuteScalar();
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable ExecuteTable(string commandText)
        {
            if (_conn == null)
                CreateConnection();

            try
            {
                using (var da = new SqlDataAdapter(commandText, _conn))
                {
                    using (var ds = new DataSet())
                    {
                        da.SelectCommand.CommandTimeout = AppConstant.CommandTimeout;
                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            finally
            {
                CloseConnection();
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
