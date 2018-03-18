using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class UploadLeadBiz : IDisposable
    {
        public string ErrorMessage { get; set; }
        public bool RedirectToSearch { get; set; }
        public bool RedirectToView { get; set; }

        public List<ControlListData> GetSaleAndBothCampaignList()
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            KKSlmMsConfigProductScreenModel config = new KKSlmMsConfigProductScreenModel();

            var mainlist = campaign.GetSaleAndBothCampaignData();
            var exceptlist = config.GetCampaignsFromConfigProductScreen();

            return mainlist.Where(p => exceptlist.Contains(p.ValueField) == false).OrderBy(p => p.TextField).ToList();
        }

        public List<ControlListData> GetChannelList()
        {
            KKSlmMsChannelModel channel = new KKSlmMsChannelModel();
            return channel.GetChannelData();
        }

        public int SaveNewUpload(UploadAllData allData)
        {
            try
            {
                KKSlmTmpUploadLeadModel model = new KKSlmTmpUploadLeadModel();
                return model.SaveNewUpload(allData);
            }
            catch
            {
                throw;
            }
        }

        public bool SaveUpdateUpload(int uploadLeadId, UploadAllData allData)
        {
            try
            {
                KKSlmTmpUploadLeadModel model = new KKSlmTmpUploadLeadModel();
                var ret = model.SaveUpdateUpload(uploadLeadId, allData);
                ErrorMessage = model.ErrorMessage;
                RedirectToSearch = model.RedirectToSearch;
                RedirectToView = model.RedirectToView;
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public bool DeleteUpload(int uploadLeadId, string deleteByUsername)
        {
            try
            {
                KKSlmTmpUploadLeadModel model = new KKSlmTmpUploadLeadModel();
                var ret = model.DeleteUpload(uploadLeadId, deleteByUsername);
                ErrorMessage = model.ErrorMessage;
                RedirectToSearch = model.RedirectToSearch;
                RedirectToView = model.RedirectToView;
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public UploadAllData GetUploadListById(int uploadLeadId)
        {
            try
            {
                KKSlmTmpUploadLeadModel model = new KKSlmTmpUploadLeadModel();
                var ret = model.GetUploadListById(uploadLeadId);
                ErrorMessage = model.ErrorMessage;
                RedirectToSearch = model.RedirectToSearch;
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public List<UploadFileInfo> SearchData(string fileName, string statusDesc)
        {
            try
            {
                KKSlmTmpUploadLeadModel model = new KKSlmTmpUploadLeadModel();
                return model.SearchData(fileName, statusDesc);
            }
            catch
            {
                throw;
            }
        }

        public bool CanEdit(int uploadLeadId)
        {
            try
            {
                KKSlmTmpUploadLeadModel model = new KKSlmTmpUploadLeadModel();
                var ret = model.CanEdit(uploadLeadId);
                ErrorMessage = model.ErrorMessage;
                RedirectToSearch = model.RedirectToSearch;
                RedirectToView = model.RedirectToView;
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public List<ControlListData> GetCardTypeList()
        {
            KKSlmMsCardTypeModel model = new KKSlmMsCardTypeModel();
            return model.GetCardTypeList();
        }

        #region IDisposable

        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
            }

            _disposed = true;
        }

        #endregion
    }
}
