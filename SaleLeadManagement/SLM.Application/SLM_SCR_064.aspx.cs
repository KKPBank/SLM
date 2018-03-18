using SLM.Application.Utilities;
using SLM.Biz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Resource;
using System.IO;
using System.Text;

namespace SLM.Application
{
    public partial class SLM_SCR_064 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage.SaleLead)this.Master).ShowReportMenu();
            ((Label)Page.Master.FindControl("lblTopic")).Text = "Report ตรวจสอบเลขที่รับแจ้ง แจ้งส่งพรบ.";

            if (!IsPostBack)
            {
            }
        }

        private bool ValidInput()
        {
            return true;
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidInput())
                {
                    return;
                }

                var filename = Path.GetTempPath() + "\\tmpexcel_064_" + Page.User.Identity.Name + ".xls";// Path.GetTempFileName();
                var bz = new SlmScr064Biz();

                if (bz.CreateExcelV2(filename))
                {
                    // สรุปผลการต่อประกันเทียบกับ Lead รายเดือน[กรกฎาคม]_[58]_
                    string destFilename = string.Format("ReportReCheckReceiveNo_{0:yyyyMMdd_HHmmss}.xls", DateTime.Now);
                    ExportExcel(filename, destFilename);
                }
                else
                {
                    AppUtil.ClientAlert(this, bz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                // _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void ExportExcel(string sourcefile, string exportfilename)
        {
            // 3 send excel to response
            Response.ClearHeaders();
            Response.HeaderEncoding = Request.Browser.IsBrowser("CHROME") ? Encoding.UTF8 : Encoding.Default;
            Response.ContentEncoding = System.Text.Encoding.Default;
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", (Request.Browser.IsBrowser("IE") ? HttpUtility.UrlEncode(exportfilename) : exportfilename)));
            Response.ContentType = "application/ms-excel";

            Response.BinaryWrite(File.ReadAllBytes(sourcefile));

            Response.End();
        }
    }
}