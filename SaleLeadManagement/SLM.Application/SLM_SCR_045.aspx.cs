using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Biz;
using System.IO;
using System.Data.OleDb;
using SLM.Application.Utilities;
using SLM.Resource;
using log4net;
using SLM.Resource.Data;
using System.Text;

namespace SLM.Application
{
    public partial class SLM_SCR_045 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_045));

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            try
            {
                if (Request["ecode"] != null && Request["ecode"] == "005")
                {
                    ExportEditReceipt(Request["ticketid"], Request["recno"]);
                }
                else
                {
                    if (Session["excelfilepath"] != null && Session["outputfilename"] != null)
                    {
                        string excelFilePath = Convert.ToString(Session["excelfilepath"]);
                        string outputfilename = Convert.ToString(Session["outputfilename"]);

                        Session.Remove("excelfilepath");
                        Session.Remove("outputfilename");

                        Response.ClearHeaders();
                        Response.HeaderEncoding = Request.Browser.IsBrowser("CHROME") ? Encoding.UTF8 : Encoding.Default;
                        Response.ContentEncoding = System.Text.Encoding.Default;
                        Response.AddHeader("content-disposition", "attachment;filename=" + (Request.Browser.IsBrowser("IE") ? HttpUtility.UrlEncode(outputfilename) : outputfilename));
                        Response.ContentType = "application/ms-excel";

                        Response.BinaryWrite(File.ReadAllBytes(excelFilePath));

                        Response.End();
                    }
                }
            }   
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                //_log.Error(message);
                Response.Write(message);
                Response.End();
            }
        }

        private void ExportEditReceipt(string ticketId, string recno)
        {
            try
            {
                string cfolder = Server.MapPath("~/Upload");

                if (!Directory.Exists(cfolder))
                {
                    Directory.CreateDirectory(cfolder);
                }

                string templatename = "Template_แบบฟอร์มแก้ไขใบเสร็จ.xls";
                string sheetname = "ชำระผิด";
                string outputfilename = "แบบฟอร์มแก้ไขใบเสร็จ.xls";

                List<ExcelTemplateBiz.ExcelData> lst = new List<ExcelTemplateBiz.ExcelData>();
                ExcelTemplateBiz ebiz = new ExcelTemplateBiz();

                if (String.IsNullOrEmpty(recno))
                {
                    throw new Exception("Invalid Rec No");
                }
                lst = ebiz.GetExcelData005(ticketId, recno);

                string excelFilePath = GetExcel(templatename, sheetname, lst);

                Response.ClearHeaders();
                Response.HeaderEncoding = Request.Browser.IsBrowser("CHROME") ? Encoding.UTF8 : Encoding.Default;
                Response.ContentEncoding = System.Text.Encoding.Default;
                Response.AddHeader("content-disposition", "attachment;filename=" + (Request.Browser.IsBrowser("IE") ? HttpUtility.UrlEncode(outputfilename) : outputfilename));
                Response.ContentType = "application/ms-excel";

                Response.BinaryWrite(File.ReadAllBytes(excelFilePath));

                Response.End();
            }
            catch
            {
                throw;
            }
        }

        private string GetExcel(string templatefilename, string sheetname, List<ExcelTemplateBiz.ExcelData> datalst)
        {
            try
            {
                string templatepath = Server.MapPath("~/ExcelTemplate");

                // 1 copy excel to temp
                string tmpfile = Path.GetTempFileName();
                if (File.Exists(tmpfile))
                {
                    File.Delete(tmpfile);
                }
                File.Copy(templatepath + "\\" + templatefilename, tmpfile);

                // 2 update excel
                using (OleDbConnection oconn = new OleDbConnection(String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=NO\"", tmpfile)))
                {
                    try
                    {
                        oconn.Open();
                        OleDbCommand ocmd = new OleDbCommand();
                        ocmd.Connection = oconn;
                        ocmd.CommandType = System.Data.CommandType.Text;
                        foreach (var data in datalst)
                        {
                            string sqlz = String.Format("UPDATE [{0}${1}:{1}] SET F1 = {2}", sheetname, data.ColumnName, String.IsNullOrEmpty(data.Value) ? "null" : "'" + data.Value + "'");
                            ocmd.CommandText = sqlz;
                            ocmd.ExecuteNonQuery();
                        }
                        oconn.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return tmpfile;
            }
            catch
            {
                throw;
            }
        }

        #region Backup 06-07-2016
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    Response.Clear();
        //    try
        //    {
        //        string excelcode = Request["ecode"];
        //        string refid = Request["id"];
        //        string reftype = Request["type"]; // T = Ticket , P = Prelead

        //        if (String.IsNullOrEmpty(refid))
        //        {
        //            throw new Exception("Invalid ID");
        //        }
        //        string cfolder = Server.MapPath("~/Upload");
        //        // or get from config
        //        if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ExcelCopyLocation"]))
        //            cfolder = Path.GetDirectoryName(ConfigurationManager.AppSettings["ExcelCopyLocation"]);
        //        // check exists
        //        if (!Directory.Exists(cfolder)) Directory.CreateDirectory(cfolder);

        //        string templatename = "";
        //        string sheetname = "";
        //        string outputname = "";
        //        string copylocation = "";
        //        string fileName = "";

        //        List<ExcelTemplateBiz.ExcelData> lst = new List<ExcelTemplateBiz.ExcelData>();
        //        ExcelTemplateBiz ebiz = new ExcelTemplateBiz();
        //        switch (excelcode)
        //        {
        //            case "001": // ฟอร์มใบเสนอราคาเปรียบเทียบ
        //                templatename = "Template-ใบเสนอราคาเปรียบเทียบ.xls";
        //                sheetname = "ประเภท 1";
        //                outputname = "ใบเสนอราคาเปรียบเทียบ.xls";


        //                fileName = String.Format("ใบเสนอราคา_{0}{1}.xls", DateTime.Now.Year.ToString("0000") + DateTime.Now.ToString("MMddHHmmss"), new Random().Next(0, 999).ToString("000"));
        //                copylocation = cfolder + "\\" + fileName;

        //                if (reftype == "T") lst = ebiz.GetExcelData001(refid);
        //                else if (reftype == "P") lst = ebiz.GetExcelData001P(SLMUtil.SafeDecimal(refid));
        //                else throw new Exception("Invalid Type");

        //                if (lst.Count == 0) throw new Exception("No Data");
        //                break;

        //            case "002": // ฟอร์มใบเสอราคาแบบรถ Fleet
        //                if (String.IsNullOrEmpty(reftype)) throw new Exception("Specify type for fleet report, T = TicketID, P = PreleadID");
        //                templatename = "Template_ใบเสนอราคา_FLEET.xls";
        //                sheetname = "Sheet1";
        //                outputname = "ใบเสนอราคา_FLEET.xls";

        //                fileName = String.Format("ใบเสนอราคาFleet_{0}{1}.xls", DateTime.Now.Year.ToString("0000") + DateTime.Now.ToString("MMddHHmmss"), new Random().Next(0, 999).ToString("000"));
        //                copylocation = cfolder + "\\" + fileName;

        //                var allid = refid.Split(',');

        //                if (reftype == "T") lst = ebiz.GetExcelData002T(allid);
        //                else if (reftype == "P") { var iddec = new List<decimal>(); foreach (var str in allid) iddec.Add(SLMUtil.SafeDecimal(str)); lst = ebiz.GetExcelData002P(iddec.ToArray()); }
        //                else throw new Exception("Invalid type");

        //                if (lst.Count == 0) throw new Exception("No Data");
        //                break;

        //            case "003": // ฟอร์มตัดบัตรเครดิต
        //                templatename = "Template_ฟอร์มบัตรเครดิต.xls";
        //                sheetname = "Sheet1";
        //                outputname = "ฟอร์มบัตรเครดิต.xls";
        //                lst = ebiz.GetExcelData003(refid);
        //                break;

        //            case "004": // ฟอร์ม 50 ทวิ
        //                templatename = "Template-ทวิ50-หักภาษี_ณ_ที่จ่าย1%(นิติบุคคล).xls";
        //                sheetname = "บริษัท";
        //                outputname = "ทวิ50-หักภาษี_ณ_ที่จ่าย1%(นิติบุคคล).xls";
        //                lst = ebiz.GetExcelData004(refid);
        //                break;

        //            case "005": // ฟอร์มแก้ไขใบเสร็จ
        //                string recno = Request["rec"];
        //                if (String.IsNullOrEmpty(recno)) throw new Exception("Invalid Rec No");
        //                templatename = "Template_แบบฟอร์มแก้ไขใบเสร็จ.xls";
        //                sheetname = "ชำระผิด";
        //                outputname = "แบบฟอร์มแก้ไขใบเสร็จ.xls";
        //                lst = ebiz.GetExcelData005(refid, recno);
        //                break;

        //            default:
        //                throw new Exception("Invalid Excel Template Code");

        //        }

        //        GetExcel(templatename, sheetname, outputname, lst, copylocation, fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        Response.Write(ex.Message);
        //        Response.End();
        //    }
        //}


        //private void GetExcel(string templatefilename, string sheetname, string outputfilename, List<ExcelTemplateBiz.ExcelData> datalst)
        //{
        //    GetExcel(templatefilename, sheetname, outputfilename, datalst, "", "");
        //}

        //private void GetExcel(string templatefilename, string sheetname, string outputfilename, List<ExcelTemplateBiz.ExcelData> datalst, string copytolocation, string fileName)
        //{
        //    try
        //    {
        //        string templatepath = Server.MapPath("~/ExcelTemplate");


        //        // 1 copy excel to temp
        //        string tmpfile = Path.GetTempFileName();
        //        if (File.Exists(tmpfile)) File.Delete(tmpfile);
        //        File.Copy(templatepath + "\\" + templatefilename, tmpfile);

        //        // 2 update excel
        //        using (OleDbConnection oconn = new OleDbConnection(String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=NO\"", tmpfile)))
        //        {
        //            try
        //            {
        //                oconn.Open();
        //                OleDbCommand ocmd = new OleDbCommand();
        //                ocmd.Connection = oconn;
        //                ocmd.CommandType = CommandType.Text;
        //                foreach (var data in datalst)
        //                {
        //                    string sqlz = String.Format("UPDATE [{0}${1}:{1}] SET F1 = '{2}'", sheetname, data.ColumnName, data.Value);
        //                    ocmd.CommandText = sqlz;
        //                    ocmd.ExecuteNonQuery();
        //                }
        //                oconn.Close();
        //            }
        //            catch (Exception ex)
        //            {
        //                Response.Write(ex.Message);
        //                return;
        //            }
        //        }

        //        // if copy location selected
        //        if (copytolocation.Trim() != "")
        //        {
        //            try
        //            {
        //                if (File.Exists(copytolocation)) File.Delete(copytolocation);
        //                File.Copy(tmpfile, copytolocation);
        //                FileInfo info = new FileInfo(copytolocation);

        //                bool ret = SaveFileToEcm(copytolocation, fileName, Convert.ToInt32(info.Length));
        //                if (!ret)
        //                {
        //                    return;
        //                }
        //            }
        //            catch (Exception ex) 
        //            {
        //                throw ex;
        //                //Response.Write("<script>alert('" + message + "');window.close();</script>");
        //            }
        //        }

        //        // 3 send excel to response
        //        Response.ClearHeaders();
        //        Response.HeaderEncoding = System.Text.Encoding.Default;
        //        Response.ContentEncoding = System.Text.Encoding.Default;
        //        Response.AddHeader("content-disposition", "attachment;filename=" + outputfilename);
        //        Response.ContentType = "application/ms-excel";

        //        Response.BinaryWrite(File.ReadAllBytes(tmpfile));

        //        Response.End();

        //    }
        //    catch (Exception ex)
        //    {
        //        //Response.Write("Error : " + ex.Message);
        //        //Response.End();
        //        //return;

        //        string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
        //        _log.Debug(message);
        //        throw ex;
        //    }

        //}

        //private bool SaveFileToEcm(string fileLocation, string fileName, int fileSize)
        //{
        //    bool ret = false;
        //    try
        //    {
        //        //fileLocation ex. D:\Projects\Motif\KK\KK_SLM\SaleLeadManagement\SLM.Application\Upload\ใบเสนอราคา_20160613165258896.xls
        //        //fileName ex. ใบเสนอราคา_20160613165258896.xls

        //        string excelcode = Request["ecode"];
        //        string refid = Request["id"];               //id can be TicketId or PreleadId
        //        string reftype = Request["type"];           // T = Ticket , P = Prelead
        //        string mainId = Request["mainid"];

        //        ConnectEcm ecm = new ConnectEcm();
        //        ret = ecm.AddAttachment(fileLocation, fileName, mainId, "fileType", HttpContext.Current.User.Identity.Name);

        //        if (ret)
        //        {
        //            string filePath = SLMConstant.Ecm.SitePath + mainId.Trim() + "/";

        //            if (reftype == "T")         //Lead
        //            {
        //                if (excelcode == "001")
        //                {
        //                    int phonecallId = SlmScr008Biz.InsertPhonecallEcm(refid, filePath, fileName, fileSize, HttpContext.Current.User.Identity.Name);
        //                    bool result = CreateCASActivityLog(refid, "", excelcode, false);
        //                    SlmScr008Biz.UpdateCasFlag(phonecallId, result ? "1" : "2");
        //                }
        //                else if (excelcode == "002")
        //                {
        //                    string[] ticketIdList = refid.Split(',');
        //                    var idList = SlmScr008Biz.InsertPhonecallEcm(ticketIdList, filePath, fileName, fileSize, HttpContext.Current.User.Identity.Name);

        //                    foreach (string ticketId in ticketIdList)
        //                    {
        //                        bool result = CreateCASActivityLog(ticketId, "", excelcode, true);
        //                        int phonecallId = idList.Where(p => p.TicketId == ticketId).Select(p => p.PhoneCallId).FirstOrDefault();
        //                        SlmScr008Biz.UpdateCasFlag(phonecallId, result ? "1" : "2");
        //                    }
        //                }
        //            }
        //            else if (reftype == "P")        //Prelead
        //            {
        //                PreleadPhoneCallBiz biz = new PreleadPhoneCallBiz();
        //                if (excelcode == "001")
        //                {
        //                    decimal phonecallId = biz.InsertPhonecallEcm(Convert.ToDecimal(refid), filePath, fileName, fileSize, HttpContext.Current.User.Identity.Name);
        //                    bool result = CreateCASActivityLog("", refid, excelcode, false);
        //                    biz.UpdateCasFlag(phonecallId, result ? "1" : "2");
        //                }
        //                else if (excelcode == "002")
        //                {
        //                    string[] preleadIdList = refid.Split(',');
        //                    var idList = biz.InsertPhonecallEcm(preleadIdList, filePath, fileName, fileSize, HttpContext.Current.User.Identity.Name);

        //                    foreach (string preleadId in preleadIdList)
        //                    {
        //                        bool result = CreateCASActivityLog("", preleadId, excelcode, true);
        //                        decimal phonecallId = idList.Where(p => p.PreleadId == preleadId).Select(p => p.PhoneCallId).FirstOrDefault();
        //                        biz.UpdateCasFlag(phonecallId, result ? "1" : "2");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        //        throw ex;
        //    }
        //    finally
        //    {
        //        DeleteLocalFile(fileLocation);    //ลบไฟล์ temp ที่โฟลเดอร์ Upload
        //    }

        //    return ret;
        //}

        //private void DeleteLocalFile(string fileLocation)
        //{
        //    try
        //    {
        //        System.IO.File.Delete(fileLocation);
        //    }
        //    catch (Exception ex)
        //    {
        //        //ลบ Temp ไฟล์ใน folder Upload , เกิด error ไม่ต้อง throw
        //        _log.Debug(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //}

        //private bool CreateCASActivityLog(string ticketId, string preleadId, string ecode, bool isFleet)
        //{
        //    LeadDataForCARLogService data = null;
        //    string ft = "";
        //    string od = "";
        //    string discountBath = "";
        //    string vat1PercentBath = "";
        //    string policyGrossPremiumPay = "";
        //    string str_preleadId = "";
        //    string systemCode = "";
        //    string securityKey = "";
        //    StaffData staff = null;

        //    try
        //    {
        //        data = new SlmScr004Biz().GetDataForCARLogService(ticketId, preleadId);

        //        if (data != null)
        //        {
        //            ft = data.FT != null ? data.FT.Value.ToString("#,##0.00") : "";
        //            od = data.OD != null ? data.OD.Value.ToString("#,##0.00") : "";
        //            discountBath = data.DiscountBath != null ? data.DiscountBath.Value.ToString("#,##0.00") : "";
        //            vat1PercentBath = data.Vat1PercentBath != null ? data.Vat1PercentBath.Value.ToString("#,##0.00") : "";
        //            policyGrossPremiumPay = data.PolicyGrossPremiumPay != null ? data.PolicyGrossPremiumPay.Value.ToString("#,##0.00") : "";
        //            str_preleadId = data.PreleadId != null ? data.PreleadId.Value.ToString() : preleadId;
        //        }
        //        else
        //            str_preleadId = preleadId;

        //        systemCode = str_preleadId != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM;
        //        securityKey = str_preleadId != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey;

        //        //Activity Info
        //        List<Services.CARService.DataItem> act_dataItemList = new List<Services.CARService.DataItem>();
        //        act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "พิมพ์ใบเสนอราคาในรูปแบบ", DataValue = isFleet ? "Fleet" : "ปกติ" });
        //        act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
        //        act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.LicenseNo : "" });
        //        act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 4, DataLabel = "บริษัทประกันภัยรถยนต์", DataValue = data != null ? data.InsuranceCompany : "" });
        //        act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 5, DataLabel = "ทุนประกัน กรณีไฟไหม้/สูญหาย", DataValue = ft });
        //        act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 6, DataLabel = "ทุนประกัน กรณีชน", DataValue = od });
        //        act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 7, DataLabel = "ส่วนลด", DataValue = discountBath });
        //        act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 8, DataLabel = "หักภาษี ณ ที่จ่าย 1%", DataValue = vat1PercentBath });
        //        act_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 9, DataLabel = "ค่าเบี้ยประกันที่ต้องชำระ", DataValue = policyGrossPremiumPay });

        //        //Customer Info
        //        List<Services.CARService.DataItem> cus_dataItemList = new List<Services.CARService.DataItem>();
        //        cus_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = data != null ? data.CitizenId : "" });
        //        cus_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = data != null ? data.CardTypeName : "" });
        //        cus_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = data != null ? data.CustomerName : "" });

        //        //Product Info
        //        List<Services.CARService.DataItem> prod_dataItemList = new List<Services.CARService.DataItem>();
        //        prod_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = data != null ? data.ProductGroupName : "" });
        //        prod_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = data != null ? data.ProductName : "" });
        //        prod_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = data != null ? data.CampaignName : "" });

        //        //Contract Info
        //        List<Services.CARService.DataItem> cont_dataItemList = new List<Services.CARService.DataItem>();
        //        cont_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = data != null ? data.ContractNo : "" });
        //        cont_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = str_preleadId != "" ? "HP" : "" });
        //        cont_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data != null ? data.LicenseNo : "" });

        //        //Officer Info
        //        staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
        //        List<Services.CARService.DataItem> off_dataItemList = new List<Services.CARService.DataItem>();
        //        off_dataItemList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name });

        //        Services.CARService.CARServiceData carData = new Services.CARService.CARServiceData()
        //        {
        //            ReferenceNo = ecode,
        //            SecurityKey = securityKey,
        //            ServiceName = "CreateActivityLog",
        //            SystemCode = systemCode,
        //            TransactionDateTime = DateTime.Now,
        //            ActivityInfoList = act_dataItemList,
        //            CustomerInfoList = cus_dataItemList,
        //            ProductInfoList = prod_dataItemList,
        //            ContractInfoList = cont_dataItemList,
        //            OfficerInfoList = off_dataItemList,
        //            ActivityDateTime = DateTime.Now,
        //            CampaignId = data != null ? data.CampaignId : "",
        //            ChannelId = data != null ? data.ChannelId : "",
        //            PreleadId = str_preleadId,
        //            ProductGroupId = data != null ? data.ProductGroupId : "",
        //            ProductId = data != null ? data.ProductId : "",
        //            Status = data != null ? data.StatusName : "",
        //            SubStatus = data != null ? data.SubStatusName : "",
        //            TicketId = data != null ? data.TicketId : "",
        //            SubscriptionId = data != null ? data.CitizenId : "",
        //            TypeId = SLMConstant.CARLogService.Data.TypeId,
        //            AreaId = SLMConstant.CARLogService.Data.AreaId,
        //            SubAreaId = SLMConstant.CARLogService.Data.SubAreaId,
        //            ActivityTypeId = SLMConstant.CARLogService.Data.ActivityType.TodoId,
        //            ContractNo = data != null ? data.ContractNo : ""
        //        };

        //        if (data != null && data.SubScriptionTypeId != null)
        //            carData.SubscriptionTypeId = data.SubScriptionTypeId.Value;

        //        return Services.CARService.CreateActivityLog(carData);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Error ให้ลง Log ไว้ ไม่ต้อง Throw
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        return false;
        //    }
        //}
        #endregion
    }
}