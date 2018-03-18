using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToEcm = Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System.IO;
using System.Net;
using SLM.Resource;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_054 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_054));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string type = Request["type"];

                    if (!string.IsNullOrEmpty(type))
                    {
                        string[] arr = new string[2];

                        if (type == "1")
                        {
                            arr = (string[])Session[SLMConstant.SessionName.CreditFormFilePath];
                            Session.Remove(SLMConstant.SessionName.CreditFormFilePath);
                        }
                        else if (type == "2")
                        {
                            arr = (string[])Session[SLMConstant.SessionName.Tawi50FormFilePath];
                            Session.Remove(SLMConstant.SessionName.Tawi50FormFilePath);
                        }
                        else if (type == "3")
                        {
                            arr = (string[])Session[SLMConstant.SessionName.DriverLicenseFormFilePath];
                            Session.Remove(SLMConstant.SessionName.DriverLicenseFormFilePath);
                        }

                        if (arr.Count() == 2)
                            DownloadFile(arr[0], arr[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        private void DownloadFile(string filePath, string fileName)
        {
            ToEcm.ClientContext context = null;
            try
            {
                string siteUrl = SLMConstant.Ecm.SiteUrl;       //http://ecm/dept/public
                string domain = SLMConstant.Ecm.Domain;
                string username = SLMConstant.Ecm.Username;
                string password = SLMConstant.Ecm.Password;
                string fileFullName = filePath + fileName;

                context = new ToEcm.ClientContext(siteUrl) { Credentials = new NetworkCredential(username, password, domain) };

                //Ex.filename = /dept/public/OBTDocument/160083566900/ActPaymentType_25590517_045936.png

                _log.Info("Download File Path : " + fileFullName);
                using (FileInformation sharePointFile = ToEcm.File.OpenBinaryDirect(context, fileFullName))
                {
                    //-------------------------

                    //Response.ContentType = "text/plain";    //StreamHelpers.GetContentType(fileInf.Extension);
                    Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}", fileName));
                    // Write the file to the Response
                    const int bufferLength = 1000000;
                    byte[] buffer = new Byte[bufferLength];
                    int length = 0;
                    try
                    {
                        do
                        {
                            if (Response.IsClientConnected)
                            {
                                length = sharePointFile.Stream.Read(buffer, 0, bufferLength);
                                Response.OutputStream.Write(buffer, 0, length);
                                buffer = new Byte[bufferLength];
                            }
                            else
                            {
                                length = -1;
                            }
                        }
                        while (length > 0);

                        //Response.Flush();
                        //Response.End();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Download File Error : " + ex);
                lblError.Text = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw ex;
            }
            finally
            {
                if (context != null)
                    context.Dispose();

                Response.Flush();
                Response.End();
            }
        }
    }
}