using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SLS.Dal.Models
{
    public class CocMsMappingStatusModel : IDisposable
    {
        #region Member
        private SLM_DBEntities slmdb = null;
        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
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
                return;
            }
            if (disposing)
            {
                handle.Dispose();
            }
            disposed = true;
        }
        #endregion

        public CocMsMappingStatusModel()
        {
            slmdb = new SLM_DBEntities();
        }

        public CocMsMappingStatusModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public string GetSlmStatusCode(string systemName, string cocStatusCode, string cocSubStatusCode)
        {
            kkcoc_ms_mapping_status mapping;

            if (string.IsNullOrEmpty(cocSubStatusCode))
                mapping = slmdb.kkcoc_ms_mapping_status.Where(p => p.coc_SYSTEM == systemName && p.coc_Status_Code == cocStatusCode && p.coc_SubStatus_Code == null).FirstOrDefault();
            else
                mapping = slmdb.kkcoc_ms_mapping_status.Where(p => p.coc_SYSTEM == systemName && p.coc_Status_Code == cocStatusCode && p.coc_SubStatus_Code == cocSubStatusCode).FirstOrDefault();

            return mapping != null ? mapping.slm_Status_Code : "";
        }
    }
}
