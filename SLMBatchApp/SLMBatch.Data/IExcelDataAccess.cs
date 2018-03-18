using System;
using System.Data;

namespace SLMBatch.Data
{
    [Obsolete("IExcelDataAccess is deprecated, please use SQLDataAccess instead.", true)]
    public interface IExcelDataAccess : IDisposable
    {
        [Obsolete]
        DataTable LoadData();
    }
}
