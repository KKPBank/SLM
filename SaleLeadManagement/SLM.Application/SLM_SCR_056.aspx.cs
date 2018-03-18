using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using SLM.Application.Utilities;
using System.Text;
using SLM.Biz;
using SLM.Resource;
using SLM.Resource.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using log4net;
using Excel;

namespace SLM.Application
{
    public partial class SLM_SCR_056 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_056));

        private string ErrorMessage { get; set; }
        private bool RedirectToSearch { get; set; }
        private bool RedirectToView { get; set; }
        private const string SearchPage = "SLM_SCR_055.aspx";
        private const string ViewPage = "SLM_SCR_057.aspx";
        private const string excelfilepath = "excelfilepath";
        private const string outputfilename = "outputfilename";
        private const string contenttype = "contenttype";

        public string SessionUploadLead
        {
            get
            {
                return string.Format("{0}_{1}", SLMConstant.SessionName.UploadLead, HttpContext.Current.User.Identity.Name);
            }
        }

        enum ExcelColumnName
        {
            Firstname = 0,
            Lastname,
            CardTypeDesc,
            CitizenId,
            OwnerEmpCode,
            DelegateEmpCode,
            TelNo1,
            TelNo2,
            Detail
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_056");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    InitialControl();
                    if (!string.IsNullOrEmpty(Request["uploadleadid"]))
                    {
                        ((Label)Page.Master.FindControl("lblTopic")).Text = "Upload Lead Management (Edit)";
                        hdfUploadLeadId.Value = Request["uploadleadid"];

                        if (!CheckPrivilege(hdfUploadLeadId.Value))
                        {
                            WriteLogFile(hdfUploadLeadId.Value, ErrorMessage);
                            AppUtil.ClientAlertAndRedirect(Page, ErrorMessage, (RedirectToView ? GetViewPageUrl(hdfUploadLeadId.Value) : SearchPage));
                            return;
                        }

                        if (!LoadLead(hdfUploadLeadId.Value))
                        {
                            WriteLogFile(hdfUploadLeadId.Value, ErrorMessage);

                            if (RedirectToSearch)
                            {
                                AppUtil.ClientAlertAndRedirect(Page, ErrorMessage, SearchPage);
                                return;
                            }
                            else
                            {
                                AppUtil.ClientAlert(Page, ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        ((Label)Page.Master.FindControl("lblTopic")).Text = "Upload Lead Management";
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool CheckPrivilege(string uploadLeadId)
        {
            try
            {
                bool ret = true;

                UploadLeadBiz biz = new UploadLeadBiz();
                if (!biz.CanEdit(int.Parse(uploadLeadId)))
                {
                    ErrorMessage = biz.ErrorMessage;
                    RedirectToSearch = biz.RedirectToSearch;
                    RedirectToView = biz.RedirectToView;
                    ret = false;
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }

        private void InitialControl()
        {
            pcTop.SetVisible = false;
            SetStateControlButton(false, false, false);
            Session.Remove(SessionUploadLead);

            UploadLeadBiz biz = new UploadLeadBiz();
            cmbCampaign.DataSource = biz.GetSaleAndBothCampaignList();
            cmbCampaign.DataTextField = "TextField";
            cmbCampaign.DataValueField = "ValueField";
            cmbCampaign.DataBind();
            cmbCampaign.Items.Insert(0, new ListItem("", ""));

            cmbChannel.DataSource = biz.GetChannelList();
            cmbChannel.DataTextField = "TextField";
            cmbChannel.DataValueField = "ValueField";
            cmbChannel.DataBind();
            cmbChannel.Items.Insert(0, new ListItem("", ""));
        }

        private string GetViewPageUrl(string uploadId)
        {
            return ViewPage + "?uploadleadid=" + uploadId;
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            int pageSize = SLMConstant.GridviewPageSizeUploadLead;
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex, pageSize);
            pageControl.GenerateRecordNumber(0, pageIndex, pageSize);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                if (Session[SessionUploadLead] != null)
                {
                    //มี Session เกิดจากการ Browse
                    System.Threading.Thread.Sleep(400);
                    DoBindGridview((List<UploadLeadData>)Session[SessionUploadLead], pageControl.SelectedPageIndex);
                }
                else
                {
                    if (!string.IsNullOrEmpty(hdfUploadLeadId.Value))   //Edit
                    {
                        UploadLeadBiz biz = new UploadLeadBiz();
                        var allData = biz.GetUploadListById(int.Parse(hdfUploadLeadId.Value));
                        if (allData != null)
                        {
                            BindGridview(pageControl, allData.LeadDataList.ToArray(), pageControl.SelectedPageIndex);
                        }
                        else
                        {
                            AppUtil.ClientAlertAndRedirect(Page, string.Format("ไม่พบข้อมูลการ Upload Lead, UploadId={0}", hdfUploadLeadId.Value), SearchPage);
                            return;
                        }
                    }
                    else
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "Session has expired", SearchPage);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        private bool LoadLead(string uploadLeadId)
        {
            try
            {
                UploadLeadBiz biz = new UploadLeadBiz();
                var allData = biz.GetUploadListById(int.Parse(uploadLeadId));
                if (allData != null)
                {
                    hdfExistingFilename.Value = allData.UploadFileName;
                    txtFileName.Text = allData.UploadFileName;
                    cmbChannel.SelectedIndex = cmbChannel.Items.IndexOf(cmbChannel.Items.FindByValue(allData.ChannelId));
                    cmbCampaign.SelectedIndex = cmbCampaign.Items.IndexOf(cmbCampaign.Items.FindByValue(allData.CampaignId));
                    DoBindGridview(allData.LeadDataList, 0);
                    SetStateControlButton(true, true, false);
                    return true;
                }
                else
                {
                    ErrorMessage = biz.ErrorMessage;
                    RedirectToSearch = biz.RedirectToSearch;
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                //Use client validation
                List<UploadLeadData> dataList = null;
                List<UploadLeadDataError> errorList = null;
                hdfFilename.Value = fuLead.PostedFile.FileName;

                if (ReadExcel(fuLead.PostedFile.FileName, out dataList, out errorList))
                {
                    if (errorList != null && errorList.Count > 0)
                    {
                        ShowPopupError(errorList, (dataList.Count + errorList.Count));
                        Session.Remove(SessionUploadLead);

                        if (string.IsNullOrEmpty(hdfUploadLeadId.Value))    //New
                        {
                            txtFileName.Text = "";
                            DoClearGridviewResult();
                            SetStateControlButton(false, false, false);
                        }
                        else
                        {
                            //Edit
                            SetStateControlButton(true, true, false);
                        }
                    }
                    else
                    {
                        txtFileName.Text = fuLead.PostedFile.FileName;
                        DoBindGridview(dataList, 0);
                        Session[SessionUploadLead] = dataList;
                        HidePopupError();

                        if (string.IsNullOrEmpty(hdfUploadLeadId.Value))    //New
                        {
                            SetStateControlButton(false, false, true);
                        }
                        else
                        {
                            //Edit
                            SetStateControlButton(true, true, true);
                        }
                    }
                }
                else
                {
                    AppUtil.ClientAlert(Page, ErrorMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnClosePopupError_Click(object sender, EventArgs e)
        {
            HidePopupError();
        }

        private void DoBindGridview(List<UploadLeadData> dataList, int pageIndex)
        {
            BindGridview(pcTop, dataList.ToArray(), pageIndex);
            pnlResult.Visible = true;
            upResult.Update();
        }

        private void ShowPopupError(List<UploadLeadDataError> errorList, int totalRows)
        {
            lblTotalError.Text = string.Format("ข้อมูลไม่ถูกต้องจำนวน {0} จาก {1} รายการ", errorList.Count.ToString(), totalRows.ToString());
            rptError.DataSource = errorList.Take(10);
            rptError.DataBind();
            upPopupError.Update();
            mpePopupError.Show();
        }

        private void DoClearGridviewResult()
        {
            gvResult.DataSource = null;
            gvResult.DataBind();
            pnlResult.Visible = false;
            upResult.Update();
        }

        private void HidePopupError()
        {
            rptError.DataSource = null;
            rptError.DataBind();
            upPopupError.Update();
            mpePopupError.Hide();
        }

        private void SetStateControlButton(bool enableDownload, bool enableDelete, bool enableSave)
        {
            btnDownload.Enabled = enableDownload;
            btnDelete.Enabled = enableDelete;
            btnSave.Enabled = enableSave;

            if (gvResult.Rows.Count == 0)
            {
                btnSave.Enabled = false;
            }

            upControlButton.Update();
        }

        private bool ReadExcel(string fileName, out List<UploadLeadData> dataList, out List<UploadLeadDataError> errorList)
        {
            IExcelDataReader excelReader = null;
            try
            {
                dataList = new List<UploadLeadData>();
                errorList = new List<UploadLeadDataError>();

                string ext = System.IO.Path.GetExtension(fileName);
                excelReader = ext == ".xls" ? ExcelReaderFactory.CreateBinaryReader(fuLead.FileContent) : ExcelReaderFactory.CreateOpenXmlReader(fuLead.FileContent);
                excelReader.IsFirstRowAsColumnNames = true;

                int rowNum = 0;
                int maxRow = SLMConstant.UploadLead.UploadLeadMaxRow;
                StringBuilder sb = new StringBuilder();
                //Regex regxSpecialChr = new Regex(@"[`]"); //@"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]"
                Regex regxEng = new Regex(@"[a-zA-Z]");         //@"([\-ก-๙0-9()., ]+)"

                UploadLeadBiz biz = new UploadLeadBiz();
                var cardTypeList = biz.GetCardTypeList();
                bool isHeader = true;
                while (excelReader.Read())
                {
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    //errText = "";
                    sb.Clear();
                    rowNum += 1;
                    if (rowNum > maxRow)
                    {
                        ErrorMessage = string.Format("Excel file มีข้อมูลเกิน {0} rows", maxRow);
                        return false;
                    }

                    string nameThai = excelReader.GetString((int)ExcelColumnName.Firstname);
                    string lastnameThai = excelReader.GetString((int)ExcelColumnName.Lastname);
                    string cardtypeDesc = excelReader.GetString((int)ExcelColumnName.CardTypeDesc);
                    string citizenId = excelReader.GetString((int)ExcelColumnName.CitizenId);
                    string ownerEmpCode = excelReader.GetString((int)ExcelColumnName.OwnerEmpCode);
                    string delegateEmpCode = excelReader.GetString((int)ExcelColumnName.DelegateEmpCode);
                    string telNo1 = excelReader.GetString((int)ExcelColumnName.TelNo1);
                    string telNo2 = excelReader.GetString((int)ExcelColumnName.TelNo2);
                    string detail = excelReader.GetString((int)ExcelColumnName.Detail);

                    nameThai = string.IsNullOrWhiteSpace(nameThai) ? "" : nameThai.Trim();
                    lastnameThai = string.IsNullOrWhiteSpace(lastnameThai) ? "" : lastnameThai.Trim();
                    cardtypeDesc = string.IsNullOrWhiteSpace(cardtypeDesc) ? "" : cardtypeDesc.Trim();
                    citizenId = string.IsNullOrWhiteSpace(citizenId) ? "" : citizenId.Trim();
                    ownerEmpCode = string.IsNullOrWhiteSpace(ownerEmpCode) ? "" : ownerEmpCode.Trim();
                    delegateEmpCode = string.IsNullOrWhiteSpace(delegateEmpCode) ? "" : delegateEmpCode.Trim();
                    telNo1 = string.IsNullOrWhiteSpace(telNo1) ? "" : telNo1.Trim();
                    telNo2 = string.IsNullOrWhiteSpace(telNo2) ? "" : telNo2.Trim();
                    detail = string.IsNullOrWhiteSpace(detail) ? "" : detail.Trim();

                    if (string.IsNullOrWhiteSpace(nameThai) && string.IsNullOrWhiteSpace(lastnameThai) && string.IsNullOrWhiteSpace(cardtypeDesc)
                        && string.IsNullOrWhiteSpace(citizenId) && string.IsNullOrWhiteSpace(ownerEmpCode) && string.IsNullOrWhiteSpace(delegateEmpCode)
                        && string.IsNullOrWhiteSpace(telNo1) && string.IsNullOrWhiteSpace(telNo2) && string.IsNullOrWhiteSpace(detail))
                    {
                        //row ว่าง, skip ไป row ถัดไป
                        continue;
                    }

                    //ชื่อลูกค้า
                    if (string.IsNullOrWhiteSpace(nameThai))
                    {
                        sb.Append("Column A : กรุณาระบุชื่อลูกค้า<br />");
                    }
                    else
                    {
                        if (regxEng.IsMatch(nameThai))
                        {
                            sb.Append("Column A : กรุณาระบุชื่อลูกค้าเป็นภาษาไทย<br />");
                        }
                        else if (nameThai.Trim().Length > SLMConstant.UploadLead.FieldFirstNameSize)
                        {
                            sb.AppendFormat("Column A : ชื่อลูกค้ายาวเกิน {0} ตัวอักษร<br />", SLMConstant.UploadLead.FieldFirstNameSize.ToString());
                        }
                    }

                    //นามสกุลลูกค้า
                    if (!string.IsNullOrWhiteSpace(lastnameThai))
                    {
                        if (regxEng.IsMatch(lastnameThai))
                        {
                            sb.Append("Column B : กรุณาระบุนามสกุลลูกค้าเป็นภาษาไทย<br />");
                        }
                        else if (lastnameThai.Trim().Length > SLMConstant.UploadLead.FieldLastNameSize)
                        {
                            sb.AppendFormat("Column B : นามสกุลลูกค้ายาวเกิน {0} ตัวอักษร<br />", SLMConstant.UploadLead.FieldLastNameSize.ToString());
                        }
                    }

                    //ประเภทลูกค้า
                    if (!string.IsNullOrWhiteSpace(cardtypeDesc) && !cardTypeList.Any(p => p.TextField == cardtypeDesc))
                    {
                        sb.Append("Column C : ประเภทลูกค้าไม่ถูกต้อง<br />");
                    }

                    //เลขที่บัตร
                    if (!string.IsNullOrWhiteSpace(citizenId))
                    {
                        var cardtype_id = cardTypeList.Where(p => p.TextField == cardtypeDesc).Select(p => p.ValueField).FirstOrDefault();

                        if (cardtype_id == AppConstant.CardType.Person)
                        {
                            if (!AppUtil.VerifyCitizenId(citizenId))
                            {
                                sb.Append("Column D : เลขบัตรประชาชนไม่ถูกต้อง<br />");
                            }
                        }
                        else
                        {
                            if (citizenId.Trim().Length > SLMConstant.UploadLead.FieldCardIdSize)
                            {
                                sb.AppendFormat("Column D : เลขนิติบุคคล/บุคคลต่างชาติ ยาวเกิน {0} ตัวอักษร<br />", SLMConstant.UploadLead.FieldCardIdSize.ToString());
                            }
                        }
                    }

                    //Validate ประเภทลูกค้า, เลขที่บัตร
                    if (!string.IsNullOrWhiteSpace(cardtypeDesc) && string.IsNullOrWhiteSpace(citizenId))
                    {
                        sb.Append("Column D : กรุณระบุบัตรประชาชน/นิติบุคคล<br />");
                    }
                    else if (string.IsNullOrWhiteSpace(cardtypeDesc) && !string.IsNullOrWhiteSpace(citizenId))
                    {
                        sb.Append("Column C : กรุณาระบุประเภทลูกค้า<br />");
                    }

                    //OwnerLead
                    if (string.IsNullOrWhiteSpace(ownerEmpCode))
                    {
                        sb.Append("Column E : กรุณระบุ Owner Lead ID<br />");
                    }

                    //DelegateLead No validate

                    //TelNo1
                    if (string.IsNullOrWhiteSpace(telNo1))
                    {
                        sb.Append("Column G : กรุณาระบุเบอร์โทร 1<br />");
                    }
                    else
                    {
                        //Validate TelNo1 with CardTypeDesc
                        string cardTypeId = cardTypeList.Where(p => p.TextField == cardtypeDesc).Select(p => p.ValueField).FirstOrDefault();
                        cardTypeId = string.IsNullOrEmpty(cardTypeId) ? "" : cardTypeId;

                        if (!AppUtil.ValidateTelNo1(cardTypeId, telNo1))
                        {
                            sb.Append("Column G : เบอร์โทร 1 ไม่ถูกต้อง<br />");
                        }
                    }

                    //TelNo2
                    if (!string.IsNullOrWhiteSpace(telNo2) && !AppUtil.ValidateTelNo2(telNo2))
                    {
                        sb.Append("Column H : เบอร์โทร 2 ไม่ถูกต้อง<br />");
                    }

                    //Detail
                    if (!string.IsNullOrWhiteSpace(detail))
                    {
                        if (detail.Length > SLMConstant.UploadLead.FieldDetailSize)
                        {
                            sb.AppendFormat("Column I : รายละเอียดยาวเกิน {0} ตัวอักษร<br />", SLMConstant.UploadLead.FieldDetailSize.ToString());
                        }
                        else
                        {
                            var ret = detail.ToCharArray().Any(c => (c >= 128 && c <= 3584));
                            if (ret)
                            {
                                sb.Append("Column I : ข้อมูลรายละเอียดมีอักขระพิเศษที่ไม่อนุญาตให้ใช้งาน<br />");
                            }
                        }
                    }

                    if (sb.Length > 0)
                    {
                        UploadLeadDataError err = new UploadLeadDataError
                        {
                            Row = (rowNum + 1).ToString(),
                            ErrorDetail = sb.ToString()
                        };
                        errorList.Add(err);
                    }
                    else
                    {
                        UploadLeadData data = new UploadLeadData
                        {
                            Firstname = nameThai,
                            Lastname = lastnameThai,
                            CardTypeDesc = cardtypeDesc,
                            CitizenId = citizenId,
                            OwnerEmpCode = ownerEmpCode,
                            DelegateEmpCode = delegateEmpCode,
                            TelNo1 = telNo1,
                            TelNo2 = telNo2,
                            Detail = detail,
                            StatusDesc = "-"
                        };
                        dataList.Add(data);
                    }
                }

                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (excelReader != null)
                {
                    excelReader.Close();
                }
            }
        }

        /// <summary>
        /// ปุ่ม Download ทำงานในกรณี Edit เท่านั้น โดย download ข้อมูลที่อยู่ใน DB แล้ว Export ออกเป็น excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(hdfUploadLeadId.Value))
                {
                    UploadLeadBiz biz = new UploadLeadBiz();
                    var allData = biz.GetUploadListById(int.Parse(hdfUploadLeadId.Value));
                    if (allData != null && allData.LeadDataList != null)
                    {
                        //ExportExcelOpenXml(allData);
                        ExportExcel(allData);
                        return;
                    }
                    else
                    {
                        WriteLogFile(hdfUploadLeadId.Value, biz.ErrorMessage);

                        if (biz.RedirectToSearch)
                        {
                            PrepareRedirectPage();
                            AppUtil.ClientAlertAndRedirect(Page, biz.ErrorMessage, SearchPage);
                            return;
                        }
                        else
                        {
                            AppUtil.ClientAlert(Page, biz.ErrorMessage);
                            return;
                        }
                    }
                }
                else
                {
                    PrepareRedirectPage();
                    WriteLogFile("None", "Upload Id not found");
                    AppUtil.ClientAlertAndRedirect(Page, "Upload Id not found", SearchPage);
                    return;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        //private void ExportExcelOpenXml(UploadAllData allData)
        //{
        //    try
        //    {
        //        string outputname = "DownloadLead_" + DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd_HHmmss") + ".xlsx";
        //        string fileName = System.IO.Path.Combine(Server.MapPath("~/Upload"), outputname);

        //        using (SpreadsheetDocument document = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook))
        //        {
        //            WorkbookPart workbookPart = document.AddWorkbookPart();
        //            workbookPart.Workbook = new Workbook();

        //            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        //            worksheetPart.Worksheet = new Worksheet();

        //            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

        //            Sheet sheet = new Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

        //            sheets.Append(sheet);
        //            workbookPart.Workbook.Save();

        //            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

        //            // Constructing header
        //            Row row = new Row();

        //            row.Append(
        //                ConstructCell("ชื่อลูกค้า", CellValues.String),
        //                ConstructCell("นามสกุลลูกค้า", CellValues.String),
        //                ConstructCell("ประเภทลูกค้า", CellValues.String),
        //                ConstructCell("บัตรประชาชน/นิติบุคคล", CellValues.String),
        //                ConstructCell("Owner Lead ID", CellValues.String),
        //                ConstructCell("Delegate Lead ID", CellValues.String),
        //                ConstructCell("เบอร์โทร#1", CellValues.String),
        //                ConstructCell("เบอร์โทร#2", CellValues.String),
        //                ConstructCell("รายละเอียด Lead", CellValues.String)
        //            );

        //            // Insert the header row to the Sheet Data
        //            sheetData.AppendChild(row);

        //            // Inserting each employee
        //            foreach (var data in allData.LeadDataList)
        //            {
        //                row = new Row();
        //                row.Append(
        //                    ConstructCell(data.Firstname, CellValues.String),
        //                    ConstructCell(data.Lastname, CellValues.String),
        //                    ConstructCell(data.CardTypeDesc, CellValues.String),
        //                    ConstructCell(data.CitizenId, CellValues.String),
        //                    ConstructCell(data.OwnerEmpCode, CellValues.String),
        //                    ConstructCell(data.DelegateEmpCode, CellValues.String),
        //                    ConstructCell(data.TelNo1, CellValues.String),
        //                    ConstructCell(data.TelNo2, CellValues.String),
        //                    ConstructCell(data.Detail, CellValues.String));

        //                sheetData.AppendChild(row);
        //            }

        //            worksheetPart.Worksheet.Save();
        //        }

        //        //เก็บใส่ Session เพื่อไว้ใช้ในหน้า Export SLM_SCR_045.aspx
        //        Session[excelfilepath] = fileName;
        //        Session[outputfilename] = outputname;
        //        Session[contenttype] = "xlsx";

        //        string script = "window.open('SLM_SCR_045.aspx', 'uploaddata', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
        //        ScriptManager.RegisterStartupScript(Page, GetType(), "uploaddata", script, true);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// Export Excel 2003
        /// </summary>
        /// <param name="allData"></param>
        private void ExportExcel(UploadAllData allData)
        {
            try
            {
                string outputname = "DownloadLead_" + DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd_HHmmss") + ".xls";
                string fileName = System.IO.Path.Combine(Server.MapPath("~/Upload"), outputname);

                List<object[]> data = new List<object[]>();

                allData.LeadDataList.ForEach(p => {
                    data.Add(new object[] {
                                p.Firstname, p.Lastname, p.CardTypeDesc, p.CitizenId, p.OwnerEmpCode
                                , p.DelegateEmpCode, p.TelNo1, p.TelNo2, p.Detail, p.StatusDesc, p.Remark, p.TicketId
                            });
                });

                var ebz = new ExcelExportBiz();
                var list = new List<ExcelExportBiz.SheetItem> {
                    new ExcelExportBiz.SheetItem {
                        SheetName = "Sheet1",
                        RowPerSheet = 0,
                        Data = data,
                        Columns = new List<ExcelExportBiz.ColumnItem> {
                            new ExcelExportBiz.ColumnItem {ColumnName="ชื่อลูกค้า", ColumnDataType= ExcelExportBiz.ColumnType.Text  },
                            new ExcelExportBiz.ColumnItem {ColumnName="นามสกุลลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                            new ExcelExportBiz.ColumnItem {ColumnName="ประเภทลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                            new ExcelExportBiz.ColumnItem {ColumnName="บัตรประชาชน/นิติบุคคล", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                            new ExcelExportBiz.ColumnItem {ColumnName="Owner Lead ID", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem {ColumnName= "Delegate Lead ID", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem {ColumnName= "เบอร์โทร#1", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem {ColumnName= "เบอร์โทร#2", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem {ColumnName= "รายละเอียด Lead", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem {ColumnName= "สถานะ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem {ColumnName= "Remark", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem {ColumnName= "Ticket Id", ColumnDataType = ExcelExportBiz.ColumnType.Text }
                        }
                    }
                };

                if (ebz.CreateExcel(fileName, list))
                {

                    Session[excelfilepath] = fileName;
                    Session[outputfilename] = outputname;

                    string script = string.Format("window.open('SLM_SCR_045.aspx', '{0}', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                    ScriptManager.RegisterStartupScript(Page, GetType(), "uploaddata", script, true);
                }
                else
                {
                    AppUtil.ClientAlert(Page, ebz.ErrorMessage);
                }
            }
            catch
            {
                throw;
            }
        }

        private Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }

        private void PrepareRedirectPage()
        {
            SetStateControlButton(false, false, false);
            btnBack.Enabled = false;
            Session.Remove(SessionUploadLead);
        }

        private void WriteLogFile(string uploadLeadId, string errorMessage)
        {
            _log.Error(string.Format("UploadLeadId: {0}, Error: {1}", uploadLeadId, errorMessage));
        }

        /// <summary>
        /// ปุ่มลบทำงานในกรณี Edit เท่านั้น โดยลบข้อมูลที่อยู่ใน DB แล้ว redirect กลับไปสู่หน้าค้นหา
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(hdfUploadLeadId.Value))
                {
                    UploadLeadBiz biz = new UploadLeadBiz();
                    var ret = biz.DeleteUpload(int.Parse(hdfUploadLeadId.Value), HttpContext.Current.User.Identity.Name);
                    if (!ret)
                    {
                        if (biz.RedirectToView)
                        {
                            PrepareRedirectPage();
                            WriteLogFile(hdfUploadLeadId.Value, biz.ErrorMessage);
                            AppUtil.ClientAlertAndRedirect(Page, biz.ErrorMessage, GetViewPageUrl(hdfUploadLeadId.Value));
                            return;
                        }
                        else if (biz.RedirectToSearch)
                        {
                            PrepareRedirectPage();
                            WriteLogFile(hdfUploadLeadId.Value, biz.ErrorMessage);
                            AppUtil.ClientAlertAndRedirect(Page, biz.ErrorMessage, SearchPage);
                            return;
                        }
                        else
                        {
                            WriteLogFile(hdfUploadLeadId.Value, biz.ErrorMessage);
                            AppUtil.ClientAlert(Page, biz.ErrorMessage);
                            return;
                        }
                    }

                    PrepareRedirectPage();
                    AppUtil.ClientAlertAndRedirect(Page, "ลบข้อมูลเรียบร้อย", SearchPage);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session[SessionUploadLead] != null)
                {
                    var dataList = Session[SessionUploadLead] as List<UploadLeadData>;
                    UploadAllData allData = new UploadAllData
                    {
                        ChannelId = cmbChannel.SelectedItem.Value,
                        CampaignId = cmbCampaign.SelectedItem.Value,
                        UploadFileName = hdfFilename.Value,
                        LeadDataList = dataList,
                        CreateByUsername = HttpContext.Current.User.Identity.Name
                    };

                    UploadLeadBiz biz = new UploadLeadBiz();
                    int uploadLeadId = 0;
                    if (string.IsNullOrEmpty(hdfUploadLeadId.Value))
                    {
                        uploadLeadId = biz.SaveNewUpload(allData);
                    }
                    else
                    {
                        uploadLeadId = int.Parse(hdfUploadLeadId.Value);
                        var ret = biz.SaveUpdateUpload(uploadLeadId, allData);
                        if (!ret)
                        {
                            if (biz.RedirectToView)
                            {
                                PrepareRedirectPage();
                                WriteLogFile(uploadLeadId.ToString(), biz.ErrorMessage);
                                AppUtil.ClientAlertAndRedirect(Page, biz.ErrorMessage, GetViewPageUrl(hdfUploadLeadId.Value));
                                return;
                            }
                            else if (biz.RedirectToSearch)
                            {
                                PrepareRedirectPage();
                                WriteLogFile(uploadLeadId.ToString(), biz.ErrorMessage);
                                AppUtil.ClientAlertAndRedirect(Page, biz.ErrorMessage, SearchPage);
                                return;
                            }
                            else
                            {
                                WriteLogFile(uploadLeadId.ToString(), biz.ErrorMessage);
                                AppUtil.ClientAlert(Page, biz.ErrorMessage);
                                return;
                            }
                        }
                    }

                    PrepareRedirectPage();
                    AppUtil.ClientAlertAndRedirect(Page, "บันทึกข้อมูลเรียบร้อย", string.Format("SLM_SCR_056.aspx?uploadleadid={0}", uploadLeadId.ToString()));
                }
                else
                {
                    PrepareRedirectPage();
                    WriteLogFile("None", "Session Expired, Page will be reloaded");
                    AppUtil.ClientAlertAndRedirect(Page, "Session Expired, Page will be reloaded", Request.Url.AbsoluteUri);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            PrepareRedirectPage();
            Response.Redirect(SearchPage);
        }
    }
}