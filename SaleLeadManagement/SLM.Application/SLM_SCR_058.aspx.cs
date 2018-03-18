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
using Excel;

namespace SLM.Application
{
    public partial class SLM_SCR_058 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_058));

        private string ErrorMessage { get; set; }

        enum ExcelColumnName
        {
            EmpCode = 1,
            TicketId = 4,
            ContractNo = 5,
            PreleadId = 11
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ReAssign Lead";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_058");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
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

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                lblFilename.Text = "";
                lblTotal.Text = "";
                lblDataValid.Text = "";
                lblDataInvalid.Text = "";
                rptError.DataSource = null;
                rptError.DataBind();
                pnSummary.Visible = false;

                if (fuLead.HasFile)
                {
                    var dataList = ReadExcel();
                    if (dataList == null)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, ErrorMessage, Request.Url.AbsoluteUri);
                        return;
                    }

                    var errorList = new List<ReAssignValidateDataError>();
                    var biz = new ReAssignLeadBiz();
                    if (biz.ValidateData(dataList, out errorList))
                    {
                        bool ret = biz.ReAssignLead(dataList, HttpContext.Current.User.Identity.Name);
                        AppUtil.ClientAlertAndRedirect(Page, (ret ? "บันทึกข้อมูลเรียบร้อย" : biz.ErrorMessage), Request.Url.AbsoluteUri);
                        return;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(biz.ErrorMessage))
                        {
                            AppUtil.ClientAlertAndRedirect(Page, biz.ErrorMessage, Request.Url.AbsoluteUri);
                            return;
                        }
                        else
                        {
                            lblFilename.Text = fuLead.PostedFile.FileName;
                            lblTotal.Text = dataList.Count.ToString();
                            lblDataValid.Text = (dataList.Count - errorList.Count).ToString();
                            lblDataInvalid.Text = errorList.Count.ToString();
                            rptError.DataSource = errorList;
                            rptError.DataBind();
                            pnSummary.Visible = true;
                        }
                    }
                }
                else
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุไฟล์ Upload");
                    return;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlertAndRedirect(Page, message, Request.Url.AbsoluteUri);
            }
        }

        private List<ReAssignLeadData> ReadExcel()
        {
            IExcelDataReader excelReader = null;
            int rowNum = 0;
            try
            {
                string ext = System.IO.Path.GetExtension(fuLead.PostedFile.FileName);
                excelReader = ext == ".xls" ? ExcelReaderFactory.CreateBinaryReader(fuLead.FileContent) : ExcelReaderFactory.CreateOpenXmlReader(fuLead.FileContent);
                excelReader.IsFirstRowAsColumnNames = true;

                bool isHeader = true;
                int maxRow = SLMConstant.ReAssignLead.ReAssignLeadMaxRow;
                var dataList = new List<ReAssignLeadData>();

                while (excelReader.Read())
                {
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(excelReader.GetString(0)) && string.IsNullOrWhiteSpace(excelReader.GetString((int)ExcelColumnName.EmpCode))
                        && string.IsNullOrWhiteSpace(excelReader.GetString(2)) && string.IsNullOrWhiteSpace(excelReader.GetString(3))
                        && string.IsNullOrWhiteSpace(excelReader.GetString((int)ExcelColumnName.TicketId)) && string.IsNullOrWhiteSpace(excelReader.GetString((int)ExcelColumnName.ContractNo))
                        && string.IsNullOrWhiteSpace(excelReader.GetString(6)) && string.IsNullOrWhiteSpace(excelReader.GetString(7))
                        && string.IsNullOrWhiteSpace(excelReader.GetString(8)) && string.IsNullOrWhiteSpace(excelReader.GetString(9))
                        && string.IsNullOrWhiteSpace(excelReader.GetString(10)) && string.IsNullOrWhiteSpace(excelReader.GetString(11))
                        && string.IsNullOrWhiteSpace(excelReader.GetString(12)) && string.IsNullOrWhiteSpace(excelReader.GetString(13))
                        && string.IsNullOrWhiteSpace(excelReader.GetString(14)) && string.IsNullOrWhiteSpace(excelReader.GetString(15))
                        && string.IsNullOrWhiteSpace(excelReader.GetString(16)) && string.IsNullOrWhiteSpace(excelReader.GetString(17))
                        && string.IsNullOrWhiteSpace(excelReader.GetString(18)) && string.IsNullOrWhiteSpace(excelReader.GetString(19))
                        //&& string.IsNullOrWhiteSpace(excelReader.GetString(20)) && string.IsNullOrWhiteSpace(excelReader.GetString(21))
                        //&& string.IsNullOrWhiteSpace(excelReader.GetString(22)) && string.IsNullOrWhiteSpace(excelReader.GetString(23))
                        //&& string.IsNullOrWhiteSpace(excelReader.GetString(24)) && string.IsNullOrWhiteSpace(excelReader.GetString(25))
                        //&& string.IsNullOrWhiteSpace(excelReader.GetString(26)) && string.IsNullOrWhiteSpace(excelReader.GetString(27))
                        //&& string.IsNullOrWhiteSpace(excelReader.GetString(28)) && string.IsNullOrWhiteSpace(excelReader.GetString(29))
                        //&& string.IsNullOrWhiteSpace(excelReader.GetString(30)) && string.IsNullOrWhiteSpace(excelReader.GetString(31))
                        )
                    {
                        continue;
                    }

                    rowNum += 1;
                    if (rowNum > maxRow)
                    {
                        ErrorMessage = string.Format("Excel file มีข้อมูลเกิน {0} rows", maxRow);
                        return null;
                    }

                    ReAssignLeadData data = new ReAssignLeadData
                    {
                        RowNo = (rowNum + 1).ToString(),
                        EmpCode = excelReader.GetString((int)ExcelColumnName.EmpCode) != null ? excelReader.GetString((int)ExcelColumnName.EmpCode).Trim() : "",
                        TicketId = excelReader.GetString((int)ExcelColumnName.TicketId) != null ? excelReader.GetString((int)ExcelColumnName.TicketId).Trim() : "",
                        ContractNo = excelReader.GetString((int)ExcelColumnName.ContractNo) != null ? excelReader.GetString((int)ExcelColumnName.ContractNo).Trim() : ""
                    };
                    if (excelReader.GetString((int)ExcelColumnName.PreleadId) != null)
                    {
                        data.PreleadId = decimal.Parse(excelReader.GetString((int)ExcelColumnName.PreleadId).Trim());
                    }

                    dataList.Add(data);
                }

                return dataList;
            }
            catch(Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(string.Format("Method={0}, RowNo={1}, ErrorMessage={2}", "ReadExcel", (rowNum + 1).ToString(), message));
                ErrorMessage = "Excel is not in a correct format";
                return null;
            }
            finally
            {
                if (excelReader != null)
                {
                    excelReader.Close();
                    excelReader.Dispose();
                }
            }
        }
    }
}