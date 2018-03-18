using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Data.Common;
using SLS.Resource;

namespace SLS.Dal
{
    public static class DBConnectionExtension
    {
        public static void TryOpen(this DbConnection conn, int timeout, string operationFlag)
        {
            Stopwatch sw = new Stopwatch();
            bool connectSuccess = false;
            string errorMsg = string.Empty;

            Thread t = new Thread(delegate()
            {
                try
                {
                    sw.Start();
                    conn.Open();
                    connectSuccess = true;
                }
                catch(Exception ex)
                {
                    errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                }
            });

            t.IsBackground = true;
            t.Start();

            while (timeout > sw.ElapsedMilliseconds)
                if (t.Join(1))
                    break;

            sw.Stop();

            if (!connectSuccess)
            {
                if (errorMsg == string.Empty)
                {
                    if (operationFlag == ApplicationResource.INS_OPERATION)
                        throw new ServiceException(ApplicationResource.INS_INTERNAL_TIMEOUT_CODE, ApplicationResource.INS_INTERNAL_TIMEOUT_DESC);
                    else if (operationFlag == ApplicationResource.UPD_OPERATION)
                        throw new ServiceException(ApplicationResource.UPD_INTERNAL_TIMEOUT_CODE, ApplicationResource.UPD_INTERNAL_TIMEOUT_DESC);
                    else if (operationFlag == ApplicationResource.VAL_PER_OPERATION)
                        throw new ServiceException(ApplicationResource.VAL_PER_INTERNAL_TIMEOUT_CODE, ApplicationResource.VAL_PER_INTERNAL_TIMEOUT_DESC);
                    else
                        throw new ServiceException(ApplicationResource.SEARCH_INTERNAL_TIMEOUT_CODE, ApplicationResource.SEARCH_INTERNAL_TIMEOUT_DESC);
                }
                else
                {
                    if (operationFlag == ApplicationResource.INS_OPERATION)
                        throw new ServiceException(ApplicationResource.INS_ERROR_FROM_DB_CODE, ApplicationResource.INS_ERROR_FROM_DB_DESC, errorMsg, null);
                    else if (operationFlag == ApplicationResource.UPD_OPERATION)
                        throw new ServiceException(ApplicationResource.UPD_ERROR_FROM_DB_CODE, ApplicationResource.UPD_ERROR_FROM_DB_DESC, errorMsg, null);
                    else if (operationFlag == ApplicationResource.VAL_PER_OPERATION)
                        throw new ServiceException(ApplicationResource.VAL_PER_INTERNAL_TIMEOUT_CODE, ApplicationResource.VAL_PER_INTERNAL_TIMEOUT_DESC);
                    else
                        throw new ServiceException(ApplicationResource.SEARCH_ERROR_FROM_DB_CODE, ApplicationResource.SEARCH_ERROR_FROM_DB_DESC, errorMsg, null);
                }
            }
        }
    }
}
