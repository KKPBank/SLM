using SLMBatch.Common;
using SLMBatch.Data;
using System;
using System.Text;
using System.Data;
using System.IO;
using SpreadsheetLight;
using static SLMBatch.Common.BatchExportConfigSet;

namespace SLMBatch.Business
{
    public class ServiceExcelBase<T> : ServiceBase, IDisposable where T : SQLDataAccess, new()
    {
        #region Member

        private string _errorStep = string.Empty;
        private string _errorDetail = string.Empty;
        private int _totalExportExcel = 0;
        private T _da = null;
        private Int64 _batchMonitorId = 0;

        #endregion

        #region Properties

        private T DataAccess
        {
            get { return _da = _da ?? new T(); }
            set { _da = value; }
        }

        #endregion

        #region Refactor

        private readonly Util.eExportModule _exportModuleName;
        private readonly string _excelTemplateName;
        private readonly string _emailTemplateName;
        private readonly EmailConfigSet _email;
        private readonly ExportConfigSet _export;

        protected ServiceExcelBase(Util.eExportModule exportModuleName, string excelTemplateName, string emailTemplateName, EmailConfigSet email, ExportConfigSet export)
        {
            _exportModuleName = exportModuleName;
            _excelTemplateName = excelTemplateName;
            _emailTemplateName = emailTemplateName;
            _email = email;
            _export = export;
        }

        #endregion

        protected bool ExportExcel(string batchCode)
        {
            bool ret;
            Util.WriteLogFile(logfilename, batchCode, _excelTemplateName);
            try
            {
                _batchMonitorId = BizUtil.SetStartTime(batchCode);

                _errorStep = $"Load {_exportModuleName} data";
                //Console.WriteLine(_errorStep);
                DataTable dt = DataAccess.LoadData();
                _totalExportExcel = dt.Rows.Count;

                if (_totalExportExcel > 0)
                {
                    SaveExcelToSharePath(dt);
                }
                else
                {
                    //Console.WriteLine($"{_exportModuleName} data not found.");
                }
            }
            catch (Exception ex)
            {
                _errorDetail = ex.ToString();
            }

            if (_errorDetail.Trim() != string.Empty)
            {
                _totalExportExcel = 0;
            }
            else
            {
                DataAccess.FeedbackData();
            }

            BizUtil.SetEndTime(batchCode, _batchMonitorId
                , (_errorDetail.Trim() == string.Empty ? AppConstant.Success : AppConstant.Fail)
                , _totalExportExcel
                , (_errorDetail.Trim() == string.Empty ? _totalExportExcel : 0)
                , (_errorDetail.Trim() == string.Empty ? 0 : _totalExportExcel));

            if (_errorDetail.Trim() != string.Empty)
            {
                Util.WriteLogFile(logfilename, batchCode, _errorDetail);
                BizUtil.InsertLog(_batchMonitorId, "", "", _errorDetail);
                ret = false;
            }
            else
            {
                ret = true;
            }

            try
            {
                //Console.WriteLine("Send Mail");
                SendMail();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                Util.WriteLogFile(logfilename, batchCode, ex.ToString());
                BizUtil.InsertLog(_batchMonitorId, "", "", ex.ToString());
            }
            return ret;
        }

        private void SaveExcelToSharePath(DataTable dtExport)
        {
            try
            {
                _errorStep = "Start to save excel to share path";
                //Console.WriteLine(_errorStep);
                Util.SaveExportExcel(_exportModuleName, _export, _excelTemplateName, dtExport, 2, "A", StyleExcel);
                _errorStep = "End to save excel to share path";
                //Console.WriteLine(_errorStep);
            }
            catch (Exception ex)
            {
                _errorDetail = ex.ToString();
                //Console.WriteLine(_errorDetail);
            }
        }


        /// <remarks>override this in child class if you have any specific styling need</remarks>
        protected virtual void StyleExcel(SLDocument report)
        {
        }

        private void SendMail()
        {
            string strMailSubject = _errorDetail.Trim() == string.Empty ? _email.SubjectComplete : _email.SubjectError;
            string strMailBody = CreateTemplateEmail().ToString();
            Util.SendMail(strMailSubject, strMailBody);
        }

        private StringBuilder CreateTemplateEmail()
        {
            StringBuilder builder = new StringBuilder();
            using (StreamReader reader = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}\\Template\\Mail\\{_emailTemplateName}"))
            {
                builder.Append(reader.ReadToEnd());
                builder.Replace("{MAIL_SUBJECT}", _errorDetail.Trim() == string.Empty ? _email.SubjectComplete : _email.SubjectError);
                builder.Replace("{DATE_TO_PROCESS}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                builder.Replace("{PURPOSE}", _errorDetail.Trim() == string.Empty ? _email.PurposeComplete : _email.PurposeError);
                builder.Replace("{TOTAL_EXPORT_ROW}", _totalExportExcel.ToString());
                builder.Replace("{STEP_NAME}", _errorDetail.Trim() == string.Empty ? string.Empty : _errorStep);
                builder.Replace("{ERROR_DESCRIPTION}", _errorDetail);
            }
            return builder;
        }

        #region "IDisposable"

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DataAccess = null;
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
