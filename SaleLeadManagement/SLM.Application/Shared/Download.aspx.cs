using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using SLM.Application.Utilities;

namespace SLM.Application.Shared
{
    public partial class Download : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_downloadsource"] != null)
            {
                string source = Session["_downloadsource"] as string;
                Session.Remove("_downloadsource");
                string exportfilename = Request["fname"];
                if (string.IsNullOrEmpty(exportfilename)) exportfilename = "download.xls";

                Response.ClearHeaders();
                Response.HeaderEncoding = Request.Browser.IsBrowser("CHROME") ? Encoding.UTF8 : Encoding.Default;
                Response.ContentEncoding = System.Text.Encoding.Default;
                Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", (Request.Browser.IsBrowser("IE") ? HttpUtility.UrlEncode(exportfilename) : exportfilename)));
                Response.ContentType = "application/ms-excel";
                Response.BinaryWrite(File.ReadAllBytes(source));
                Response.End();

            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "download", "alert('ไม่พบข้อมูล');window.close();", true);
            }
        }
    }
}