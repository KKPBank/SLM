using log4net;
using SLM.Application.Utilities;
using SpreadsheetLight;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;

namespace SLM.Application
{
    public partial class SLM_SCR_041_Export : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_041));

        protected void Page_Load(object sender, EventArgs e)
        {
            _log.Info("ExportToExcel : start");
            try
            {
                if (Session["DatableExcelExport"] != null)
                {
                    DataTable dtExport = (Session["DatableExcelExport"] as DataTable).Copy();
                    Session["DatableExcelExport"] = null;

                    if (dtExport.Rows.Count <= 0)
                    {
                        dtExport.Rows.Add(dtExport.NewRow());
                    }
                    
                    string strFileName = string.Format("ผลงานของTelesales_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd_hhmm"));

                    string tmpPath = Path.Combine(Server.MapPath("~"), "ExcelTemplate\\TelesalesPerformance_Template.xlsx");

                    MemoryStream TemplateStream = new System.IO.MemoryStream();
                    using (var TemplateBase = new System.IO.FileStream(tmpPath, System.IO.FileMode.Open))
                    {
                        TemplateBase.CopyTo(TemplateStream);
                        TemplateBase.Close();
                        TemplateStream.Seek(0, SeekOrigin.Begin);
                    }

                    var report = new SLDocument(TemplateStream, "ผลงานของ Telesales");
                    report.InsertRow(2, dtExport.Rows.Count - 1);
                    report.ImportDataTable("A" + 2, dtExport, false);

                    var stream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose);
                    report.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin); // Reset pointer for reuse.

                    TemplateStream.Close();
                    report.Dispose();
                    report = null;
                    TemplateStream = null;
                    ///////////////////////////////////////////////////////////////////

                    stream.Flush();
                    stream.Position = 0;

                    Response.ClearContent();
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";


                    Response.ClearHeaders();
                    Response.HeaderEncoding = Request.Browser.IsBrowser("CHROME") ? Encoding.UTF8 : Encoding.Default;
                    Response.ContentEncoding = System.Text.Encoding.Default;
                    Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", (Request.Browser.IsBrowser("IE") ? HttpUtility.UrlEncode(strFileName) : strFileName)));
                    Response.ContentType = "application/ms-excel";
                    byte[] data1 = new byte[stream.Length];
                    stream.Read(data1, 0, data1.Length);
                    stream.Close();
                    stream = null;
                    Response.BinaryWrite(data1);
                    Response.Flush();
                    Response.End();
                   
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
            }
        }
    }
}