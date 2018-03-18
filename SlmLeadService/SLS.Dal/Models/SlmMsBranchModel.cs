using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SLS.Dal.Models
{
    public class SlmMsBranchModel : IDisposable
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
        public SlmMsBranchModel()
        {
            slmdb = new SLM_DBEntities();
        }

        public SlmMsBranchModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public bool CheckBranchActive(string branchCode)
        {
            var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == branchCode).FirstOrDefault();
            return branch != null ? (branch.is_Deleted ? false : true) : false;
        }

        //add by nung 20161124
        public bool CheckOldBranchActive(string branchCode)
        {
            var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCodeNew == branchCode).FirstOrDefault();
            return branch != null ? (branch.is_Deleted ? false : true) : false;
        }
        
        public string GetOldBranchCode(string branchCode)
        {
            var oldBranchCode = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCodeNew == branchCode).Select(i => new{ i.slm_BranchCode }).FirstOrDefault();

            return oldBranchCode != null ? oldBranchCode.slm_BranchCode : string.Empty;
        }

        public string GetNewBranchCode(string branchCode)
        {
            var newBranchCode = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == branchCode).Select(i => new { i.slm_BranchCodeNew }).FirstOrDefault();

            return newBranchCode != null ? newBranchCode.slm_BranchCodeNew : string.Empty;
        }
        //end add by nung 20161124
    }
}
