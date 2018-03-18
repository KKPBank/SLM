using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using SLM.Resource;
using ToEcm = Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System.Globalization;
using File = System.IO;
using log4net;
using System.Threading;

namespace SLM.Application.Utilities
{
    public class ConnectEcm
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ConnectEcm));
        public ConnectEcm()
        {
            //
            // TODO: Add constructor logic here
            //
            //CultureInfo TempCulture = new CultureInfo(ConfigurationManager.AppSettings["CultureInfo"].ToString());
            //TempCulture.DateTimeFormat.ShortDatePattern = ConfigurationManager.AppSettings["date_format"].ToString();
            //Thread.CurrentThread.CurrentCulture = TempCulture;
        }
        public bool AddAttachment(string paramFileName, string strFileName, string ticketId, string fileType, string loginId)
        {
            try
            {
                string siteUrl = SLMConstant.Ecm.SiteUrl;       //--http://ecm/dept/public
                string sitePath = SLMConstant.Ecm.SitePath;     //--/dept/public/OBTDocument/
                string listName = SLMConstant.Ecm.ListName;     //OBTDocument
                string domain = SLMConstant.Ecm.Domain;
                string userName = SLMConstant.Ecm.Username;
                string password = SLMConstant.Ecm.Password; 

                string fileUrl = string.Empty;
                string fileNameUpload = strFileName;
                bool result = false;
                //DateTime expDate = Convert.ToDateTime(expireDate);
                //string now = DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                //DateTime today = Convert.ToDateTime(now);

                bool res = AddNewFolder(sitePath, listName, "Folder", ticketId);
                if (res)
                {
                    if (!string.IsNullOrEmpty(paramFileName))
                    {
                        fileUrl = paramFileName;
                    }
                    if (fileNameUpload == string.Empty)
                    {
                        fileNameUpload = sitePath + ticketId + "/" + strFileName;
                    }
                    else
                    {
                        fileNameUpload = sitePath + ticketId + "/" + fileNameUpload;
                    }
                    var context = new ToEcm.ClientContext(siteUrl) { Credentials = new NetworkCredential(userName, password, domain) };

                    try
                    {
                        //log.Info("AddAttachment : Ecm path to save = " + fileNameUpload);
                        //log.Info("AddAttachment : Local fileUrl to save = " + fileUrl);

                        using (File.FileStream fs = File.File.Open(fileUrl, File.FileMode.Open))
                        {
                            ToEcm.File.SaveBinaryDirect(context: context, serverRelativeUrl: fileNameUpload, stream: fs, overwriteIfExists: true);
                        }
                        result = true;
                        log.Info("AddAttachment : Success");

                        //ToEcm.File.SaveBinaryDirect(context: context, serverRelativeUrl: fileNameUpload, stream: fs, overwriteIfExists: true);

                        //if (!UpdateContent(result, campaignId, campName, description, expDate, username, campGroup, attachedType))
                        //{
                        //    DeleteFile(fileNameUpload);
                        //}
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error AddAttachment : " + ex);
                        result = false;
                        throw ex;
                    }
                    finally
                    {
                        context.Dispose();
                    }
                }
                else
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Error AddAttachment : " + ex.Message);
                throw ex;
            }
        }

        public bool AddNewFolder(string listUrl, string listName, string docSetContentTypeName, string newDocSetName)
        {
            bool res = false;
            try
            {
                string siteUrl = SLMConstant.Ecm.SiteUrl;       //http://ecm/dept/public
                string domain = SLMConstant.Ecm.Domain;
                string userName = SLMConstant.Ecm.Username;
                string password = SLMConstant.Ecm.Password; 

                //log.Info("Connect ECM");

                using (ClientContext clientContext = new ToEcm.ClientContext(siteUrl) { Credentials = new NetworkCredential(userName, password, domain) })
                {
                    // get the web
                    Web web = clientContext.Web;

                    List list = clientContext.Web.Lists.GetByTitle(listName);
                    clientContext.Load(clientContext.Site);

                    // load the folder
                    Folder f = web.GetFolderByServerRelativeUrl(listUrl + newDocSetName);
                    log.Info(listUrl + newDocSetName);

                    clientContext.Load(f);
                    bool alreadyExists = false;

                    // check if the folder exists
                    try
                    {
                        log.Info("Before clientContext.ExecuteQuery()");
                        clientContext.ExecuteQuery();
                        log.Info("After clientContext.ExecuteQuery()");

                        alreadyExists = true;
                        res = true;
                    }
                    catch (Exception ee)
                    {
                        //ถ้าเข้า catch แสดงว่า ไม่พบ folder ที่โหลดเข้ามา
                        log.Error("Error Check Folder : " + ee.Message);
                    }

                    // create folder
                    if (!alreadyExists)
                    {
                        log.Info("Create Folder: Start");

                        // folder doesn't exists so create it
                        ContentTypeCollection listContentTypes = list.ContentTypes;
                        clientContext.Load(listContentTypes, types => types.Include
                                                            (type => type.Id, type => type.Name,
                                                            type => type.Parent));

                        var result = clientContext.LoadQuery(listContentTypes.Where
                            (c => c.Name == docSetContentTypeName));

                        clientContext.ExecuteQuery();

                        ContentType targetDocumentSetContentType = result.FirstOrDefault();

                        // create the item
                        ListItemCreationInformation newItemInfo = new ListItemCreationInformation();
                        newItemInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
                        newItemInfo.LeafName = newDocSetName;
                        Microsoft.SharePoint.Client.ListItem newListItem = list.AddItem(newItemInfo);

                        // set title and content type
                        newListItem["ContentTypeId"] = targetDocumentSetContentType.Id.ToString();
                        newListItem["Title"] = newDocSetName;
                        newListItem.Update();
                        res = true;
                        // execute it
                        clientContext.Load(list);
                        clientContext.ExecuteQuery();

                        log.Info("Create Folder: Finished");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error AddNewFolder : " + ex.Message);
                throw ex;
            }

            return res;
        }

        public void DownloadFile(HttpResponse Response, string filename)
        {
            ToEcm.ClientContext context = null;
            try
            {
                string siteUrl = SLMConstant.Ecm.SiteUrl;       //http://ecm/dept/public
                string domain = SLMConstant.Ecm.Domain;
                string username = SLMConstant.Ecm.Username;
                string password = SLMConstant.Ecm.Password;

                context = new ToEcm.ClientContext(siteUrl) { Credentials = new NetworkCredential(username, password, domain) };

                FileInfo fileInf = new FileInfo(filename);

                //Ex.filename = /dept/public/OBTDocument/160083566900/ActPaymentType_25590517_045936.png

                log.Debug("Download File Path : " + filename);
                using (FileInformation sharePointFile = ToEcm.File.OpenBinaryDirect(context, filename))
                {
                    //-------------------------

                    //Response.ContentType = "text/plain";    //StreamHelpers.GetContentType(fileInf.Extension);
                    Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}", filename));
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

                        Response.Flush();
                        Response.End();
                    }
                    catch (Exception ex)
                    {
                        log.Error("Download File Error : " + ex);
                        throw ex;
                    }
                    finally
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Download File Error : " + ex);
                throw ex;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        public bool DeleteFile(string fileName, string folderName)
        {
            bool result = false;
            string siteUrl = SLMConstant.Ecm.SiteUrl;       //http://ecm/dept/public
            string sitePath = SLMConstant.Ecm.SitePath;     ///dept/public/OBTDocument/
            string domain = SLMConstant.Ecm.Domain;
            string username = SLMConstant.Ecm.Username;
            string password = SLMConstant.Ecm.Password; 

            try
            {
                using (ToEcm.ClientContext clientContext = new ToEcm.ClientContext(siteUrl) { Credentials = new NetworkCredential(username, password, domain) })
                {
                    string serverRelativeUrlOfFile = sitePath + folderName + "/" + fileName;

                    log.Info("Delete File Path " + serverRelativeUrlOfFile);

                    ToEcm.Web web = clientContext.Web;
                    clientContext.Load(web);

                    ToEcm.File file = web.GetFileByServerRelativeUrl(serverRelativeUrlOfFile);
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();

                    if (file != null)
                    {
                        file.DeleteObject();
                        clientContext.ExecuteQuery();
                    }
                    result = true;

                    //Added By Pom 24/05/2016
                    //ลบ Folder ทิ้งในกรณีที่ไม่มี file เหลือใน Folder นั้น
                    try
                    {
                        log.Info("Start Delete Folder " + sitePath + folderName + "/");
                        ToEcm.Folder folder = web.GetFolderByServerRelativeUrl(sitePath + folderName + "/");
                        clientContext.Load(folder);
                        clientContext.ExecuteQuery();

                        if (folder != null)
                        {
                            ToEcm.FileCollection fileColl = folder.Files;
                            clientContext.Load(fileColl);
                            clientContext.ExecuteQuery();

                            log.Info("Files currently in folder " + folderName + " = " + fileColl.Count.ToString());

                            if (fileColl.Count == 0)
                            {
                                folder.DeleteObject();
                                clientContext.ExecuteQuery();
                                log.Info("Folder Deleted Successfully");
                            }
                        }
                        else
                            log.Info("Folder " + folderName + " is null");
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error DeleteFile : Cannot delete empty folder " + folderName + ", " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error DeleteFile : " + ex);
            }

            return result;
        }

        public bool VerifyConnection()
        {
            ToEcm.ClientContext context = null;
            try
            {
                string siteUrl = SLMConstant.Ecm.SiteUrl;       //http://ecm/dept/public
                string domain = SLMConstant.Ecm.Domain;
                string username = SLMConstant.Ecm.Username;
                string password = SLMConstant.Ecm.Password;

                using (context = new ToEcm.ClientContext(siteUrl) { Credentials = new NetworkCredential(username, password, domain) })
                {
                    log.Info("==============================================================================");
                    context.ExecuteQuery();
                    log.Info("Verify Connection: Sucess");
                }
                return true;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                log.Error("Verify connection: Failed because " + message);
                throw new Exception("ไม่สามารถเชื่อมต่อระบบ ECM ได้");
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
        }

        public string VerifyFileToDownload(string filePath, string fileName)
        {
            ToEcm.ClientContext context = null;
            string flag = "";
            string retMessage = "";
            try
            {
                string siteUrl = SLMConstant.Ecm.SiteUrl;       //http://ecm/dept/public
                string domain = SLMConstant.Ecm.Domain;
                string username = SLMConstant.Ecm.Username;
                string password = SLMConstant.Ecm.Password;
                string fileFullname = filePath + fileName;

                //Ex.fileFullname = /dept/public/OBTDocument/160083566900/ActPaymentType_25590517_045936.png

                flag = "connection";
                using (context = new ToEcm.ClientContext(siteUrl) { Credentials = new NetworkCredential(username, password, domain) })
                {
                    context.ExecuteQuery();
                    log.Info("Verify connection: Sucess");
                }

                flag = "file";
                using (FileInformation sharePointFile = ToEcm.File.OpenBinaryDirect(context, fileFullname))
                {
                    log.Info("Verify file: Sucess");
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                if (flag == "connection")
                {
                    log.Error("Verify connection: Failed because " + message);
                    retMessage = "ไม่สามารถเชื่อมต่อระบบ ECM ได้";
                }
                else if (flag == "file")
                {
                    log.Error("Verify file: Failed because " + message);
                    retMessage = "ไม่พบไฟล์บนระบบ ECM";
                }
                else
                    log.Error(message);
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }

            return retMessage;
        }
    }
}