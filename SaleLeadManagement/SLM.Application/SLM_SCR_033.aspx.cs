using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.OleDb;
using SLM.Biz;
using SLM.Application.Utilities;
using SLM.Resource.Data;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_033 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_033));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var products = new SlmScr033Biz().GetProductList();
            cmbProduct.DataSource = products;
            cmbProduct.DataValueField = "ValueField";
            cmbProduct.DataTextField = "TextField";
            cmbProduct.DataBind();
            cmbProduct.Items.Insert(0, new ListItem("", "0"));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_033");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    ((Label)Page.Master.FindControl("lblTopic")).Text = "นำเข้าข้อมูลโปรโมชั่น";
                    ErrorList = null;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private List<ControlListData> ErrorList
        {
            get { return Session["SLM_SCR_033_Error"] as List<ControlListData>; }
            set { Session["SLM_SCR_033_Error"] = value; }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbProduct.SelectedValue == "0") { lblPdError.Text = "กรุณาระบุผลิตภัณฑ์"; return; }

                if (fuData.HasFile)
                {
                    var ext = Path.GetExtension(fuData.FileName).ToLower();
                    if (ext != ".xls")
                    {
                        lblUploadError.Text = "กรุณาระบุไฟล์ให้ถูกต้อง (.xls)";
                        return;
                    }
                    lblFilename.Text = fuData.FileName;
                    using (OleDbConnection conn = new OleDbConnection())
                    {
                        DataTable dt = new DataTable();
                        string filename = Path.GetTempFileName();
                        fuData.SaveAs(filename);
                        fuData.ClearAllFilesFromPersistedStore();

                        conn.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=No;IMEX=0;'", filename);
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM [Sheet1$]";

                        OleDbDataAdapter adp = new OleDbDataAdapter(cmd);
                        adp.Fill(dt);
                        adp.Dispose();
                        cmd.Dispose();

                        List<ControlListData> errLst;
                        tbResult.Visible = false;

                        // save data
                        SlmScr033Biz bz = new SlmScr033Biz();
                        int total, succ, fail;
                        if (!bz.SaveData(cmbProduct.SelectedValue, dt, out total, out succ, out fail, out errLst, Page.User.Identity.Name))
                        {
                            AppUtil.ClientAlert(this, bz.ErrorMessage);
                        }
                        else
                        {
                            if (errLst.Count == 0)
                            {
                                pnlError.Visible = false;
                                AppUtil.ClientAlert(this, "นำเข้าข้อมูลเรียบร้อย");
                            }
                            else
                            {
                                ErrorList = errLst;
                                gvError.PageIndex = 0;
                                BindError();
                                AppUtil.ClientAlert(this, "ข้อมูลไม่ถูกนำเข้า กรุณาตรวจสอบข้อผิดพลาด");
                            }

                            tbResult.Visible = true;
                            lblSucc.Text = succ.ToString("#,##0");
                            lblFail.Text = fail.ToString("#,##0");
                            lblTotal.Text = total.ToString("#,##0");
                        }
                    }
                }
                else
                {
                    lblUploadError.Text = "กรุณาระบุไฟล์ที่ต้องการนำเข้า";
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (msg.Contains("Sheet1") && msg.Contains("not a valid")) msg = "ไม่พบข้อมูลในชีท 'Sheet1'";
                _log.Error(msg);
                AppUtil.ClientAlert(this, msg);
            }
        }

        private void BindError()
        {
            gvError.DataSource = ErrorList;
            gvError.DataBind();
            pnlError.Visible = gvError.Rows.Count > 0;
        }

        private bool CheckDate(string dtString)
        {
            var chk = dtString.Split('-');
            if (chk.Length != 3) return false;
            int d, m, y;
            int.TryParse(chk[0], out y);
            int.TryParse(chk[1], out m);
            int.TryParse(chk[2], out d);

            if (y == 0 || m == 0 || d == 0 || m > 12) return false;
            else return true;

        }

        protected void gvError_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvError.PageIndex = e.NewPageIndex;
            BindError();
        }

        protected void lnbClear_Click(object sender, EventArgs e)
        {
            fuData.ClearAllFilesFromPersistedStore();
        }
    }
}