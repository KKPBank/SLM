using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using SLM.Biz;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Application.Utilities;

namespace SLM.Application
{
    public partial class SLM_SCR_057 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_057));
        private const string SearchPage = "SLM_SCR_055.aspx";
        private const string excelfilepath = "excelfilepath";
        private const string outputfilename = "outputfilename";

        private string ErrorMessage { get; set; }
        private bool RedirectToSearch { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "Upload Lead Management";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_057");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    if (!string.IsNullOrEmpty(Request["uploadleadid"]))
                    {
                        InitialControl();
                        hdfUploadLeadId.Value = Request["uploadleadid"];

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
                        AppUtil.ClientAlertAndRedirect(Page, "ไม่พบข้อมูล Upload ID", SearchPage);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlertAndRedirect(Page, message, SearchPage);
            }
        }

        private void InitialControl()
        {
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

        private bool LoadLead(string uploadLeadId)
        {
            try
            {
                UploadLeadBiz biz = new UploadLeadBiz();
                var allData = biz.GetUploadListById(int.Parse(uploadLeadId));
                if (allData != null)
                {
                    txtFileName.Text = allData.UploadFileName;
                    cmbChannel.SelectedIndex = cmbChannel.Items.IndexOf(cmbChannel.Items.FindByValue(allData.ChannelId));
                    cmbCampaign.SelectedIndex = cmbCampaign.Items.IndexOf(cmbCampaign.Items.FindByValue(allData.CampaignId));
                    DoBindGridview(allData.LeadDataList, 0);
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

        private void DoBindGridview(List<UploadLeadData> dataList, int pageIndex)
        {
            BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, dataList.ToArray(), pageIndex);
        }

        private void WriteLogFile(string uploadLeadId, string errorMessage)
        {
            _log.Error(string.Format("UploadLeadId: {0}, Error: {1}", uploadLeadId, errorMessage));
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
                if (!string.IsNullOrEmpty(hdfUploadLeadId.Value))
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
                    AppUtil.ClientAlertAndRedirect(Page, "UploadId not found", SearchPage);
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

        #endregion

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
                var list = new List<ExcelExportBiz.SheetItem>() {
                    new ExcelExportBiz.SheetItem() {
                        SheetName = "Sheet1",
                        RowPerSheet = 0,
                        Data = data,
                        Columns = new List<ExcelExportBiz.ColumnItem>() {
                            new ExcelExportBiz.ColumnItem() {ColumnName="ชื่อลูกค้า", ColumnDataType= ExcelExportBiz.ColumnType.Text  },
                            new ExcelExportBiz.ColumnItem() {ColumnName="นามสกุลลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                            new ExcelExportBiz.ColumnItem() {ColumnName="ประเภทลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                            new ExcelExportBiz.ColumnItem() {ColumnName="บัตรประชาชน/นิติบุคคล", ColumnDataType = ExcelExportBiz.ColumnType.Text  },
                            new ExcelExportBiz.ColumnItem() {ColumnName="Owner Lead ID", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem() {ColumnName= "Delegate Lead ID", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem() {ColumnName= "เบอร์โทร#1", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem() {ColumnName= "เบอร์โทร#2", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem() {ColumnName= "รายละเอียด Lead", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem() {ColumnName= "สถานะ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem() {ColumnName= "Remark", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                            new ExcelExportBiz.ColumnItem() {ColumnName= "Ticket Id", ColumnDataType = ExcelExportBiz.ColumnType.Text }
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

        private void PrepareRedirectPage()
        {
            btnDownload.Enabled = false;
            btnBack.Enabled = false;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            PrepareRedirectPage();
            Response.Redirect(SearchPage);
        }
    }
}