using SLMBatch.Common;
using SLMBatch.Data;
using SpreadsheetLight;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Linq;
using SLMBatch.Entity;

namespace SLMBatch.Business
{
    public class SLM_PRO_06_Biz : ServiceBase, IDisposable
    {
        #region Member
        private string ErrorStep = string.Empty;
        private string ErrorDetail = string.Empty;
        private int TotalExportExcel = 0;
        private SLM_PRO_06_DataAccess _db = null;
        private Int64 batchMonitorId = 0;
        #endregion

        #region Properties
        private SLM_PRO_06_DataAccess Database
        {
            get { return _db = _db ?? new SLM_PRO_06_DataAccess(); }
            set { _db = value; }
        }
        #endregion

        public void ExportWSLog(string batchCode)
        {
            try
            {
                batchMonitorId = BizUtil.SetStartTime(batchCode);

                ErrorStep = "Select Data from DB";
                Console.WriteLine(ErrorStep);
                DataTable dt = Database.LoadWSLogData("INS");
                TotalExportExcel = dt.Rows.Count;

                if (TotalExportExcel > 0)
                {
                    SaveExcelToSharePath(dt);

                    if (ErrorDetail.Trim() != string.Empty)
                    {
                        TotalExportExcel = 0;
                    }
                }
                else
                {
                    ErrorDetail = "WSLog data not found.";
                    Console.WriteLine(ErrorDetail);
                }
            }
            catch (Exception ex)
            {
                ErrorDetail = ex.ToString();
            }

            BizUtil.SetEndTime(batchCode, batchMonitorId
                                , (ErrorDetail.Trim() == string.Empty ? AppConstant.Success : AppConstant.Fail)
                                , TotalExportExcel
                                , (ErrorDetail.Trim() == string.Empty ? TotalExportExcel : 0)
                                , (ErrorDetail.Trim() == string.Empty ? 0 : TotalExportExcel));

            if (ErrorDetail.Trim() != string.Empty)
            {
                Util.WriteLogFile(logfilename, batchCode, ErrorDetail);
                BizUtil.InsertLog(batchMonitorId, "", "", ErrorDetail);
            }

            try
            {
                Console.WriteLine("Send Mail");
                SendMail();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Util.WriteLogFile(logfilename, batchCode, ex.ToString());
                BizUtil.InsertLog(batchMonitorId, "", "", ex.ToString());
            }
        }

        private void SaveExcelToSharePath(DataTable dtExport)
        {
            MemoryStream TemplateStream = new System.IO.MemoryStream();
            try
            {
                string strDateTimeFileName = DateTime.Now.ToString("yyyyMMdd_HHmm");
                string strFileName = string.Empty;
                string tmpPath = string.Format("{0}\\Template\\Excel\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "WSLogINS_Template.xlsx");

                ErrorStep = "Read WSLog template file";
                Console.WriteLine(ErrorStep);
                using (var TemplateBase = new System.IO.FileStream(tmpPath, System.IO.FileMode.Open))
                {
                    TemplateBase.CopyTo(TemplateStream);
                    TemplateBase.Close();
                    TemplateStream.Seek(0, SeekOrigin.Begin);
                }

                ErrorStep = "Prepare WSLog data";
                Console.WriteLine(ErrorStep);

                int iAllExcelRow = dtExport.Rows.Count;
                int iMaxRowPerFile = Convert.ToInt32(AppConstant.MaxExcelRowPerFile);
                int iNoOfExcelFile = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(iAllExcelRow) / Convert.ToDouble(iMaxRowPerFile)));

                Console.WriteLine(string.Format("All Excel row(s) : {0}", iAllExcelRow));
                Console.WriteLine(string.Format("Max Row per file : {0}", iMaxRowPerFile));
                Console.WriteLine(string.Format("Number of Excel file : {0}", iNoOfExcelFile));

                for (int i = 0; i < iNoOfExcelFile; i++)
                {
                    if (iNoOfExcelFile == 1)
                    {
                        strFileName = string.Format("WSLogINS_{0}.xlsx", strDateTimeFileName);
                    }
                    else
                    {
                        strFileName = string.Format("WSLogINS_{0}_{1}.xlsx", strDateTimeFileName, i + 1);
                    }

                    DataTable dt = dtExport.AsEnumerable().Select(x => x).Skip(i * iMaxRowPerFile).Take(iMaxRowPerFile).CopyToDataTable();

                    var report = new SLDocument(TemplateStream, "WSLOGINS");
                    report.InsertRow(2, dt.Rows.Count - 1);
                    report.ImportDataTable("A" + 2, dt, false);

                    ErrorStep = "Save WSLog file";
                    Console.WriteLine(ErrorStep);
                    using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                    {
                        if (unc.NetUseWithCredentials(AppConstant.ExportWSLogPath, AppConstant.ExportWSLogUsername, AppConstant.ExportWSLogDomainName, AppConstant.ExportWSLogPassword))
                        {
                            report.SaveAs(string.Format("{0}{1}", AppConstant.ExportWSLogPath, strFileName));
                        }
                        else
                        {
                            ErrorDetail = string.Format("Cannot access path '{0}'", AppConstant.ExportWSLogPath);
                            Console.WriteLine(ErrorDetail);
                        }
                        unc.Dispose();
                    }

                    report.Dispose();
                    report = null;
                }
            }
            catch (Exception ex)
            {
                ErrorDetail = ex.ToString();
                Console.WriteLine(ErrorDetail);
            }
            finally
            {
                TemplateStream.Close();
                TemplateStream = null;
            }
        }

        private void SendMail()
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();

            MailAddress fromAddress = new MailAddress(AppConstant.WSLogEmailFromAddress, AppConstant.WSLogEmailDisplayName.Trim() ?? "");

            client.Host = AppConstant.WSLogEmailHostIP;
            client.Port = AppConstant.WSLogEmailPort;
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(AppConstant.WSLogEmailFromAddress, AppConstant.WSLogEmailFromPassword);
            client.Timeout = 300000;

            mail.From = fromAddress;

            foreach (string mailto in AppConstant.WSLogEmailToAddress.Split(';'))
            {
                mail.To.Add(new MailAddress(mailto));
            }

            mail.Subject = ErrorDetail.Trim() == string.Empty ? AppConstant.WSLogEmailSubjectComplete : AppConstant.WSLogEmailSubjectError;
            mail.IsBodyHtml = true;
            mail.Body = CreateTemplateEmail().ToString();

            client.Send(mail);
        }

        private StringBuilder CreateTemplateEmail()
        {
            StringBuilder builder = new StringBuilder();
            using (StreamReader reader = new StreamReader(string.Format("{0}\\Template\\Mail\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "SLM_PRO_06_EmailTemplate.html")))
            {
                builder.Append(reader.ReadToEnd());
                builder.Replace("{MAIL_SUBJECT}", ErrorDetail.Trim() == string.Empty ? AppConstant.WSLogEmailSubjectComplete : AppConstant.WSLogEmailSubjectError);
                builder.Replace("{DATE_TO_PROCESS}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                builder.Replace("{PURPOSE}", ErrorDetail.Trim() == string.Empty ? AppConstant.WSLogEmailPurposeComplete : AppConstant.WSLogEmailPurposeError);
                builder.Replace("{TOTAL_EXPORT_ROW}", TotalExportExcel.ToString());
                builder.Replace("{STEP_NAME}", ErrorDetail.Trim() == string.Empty ? string.Empty : ErrorStep);
                builder.Replace("{ERROR_DESCRIPTION}", ErrorDetail);
            }
            return builder;
        }

        #region "IDisposable"
        private bool _disposed = false;
        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Database = null;
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
