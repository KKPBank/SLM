using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using log4net;

namespace SLM.Application.MasterPage
{
    public partial class Login : System.Web.UI.MasterPage
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Login));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    lblAssemblyVersion.Text = AppUtil.GetAssemblyVersion();
                }
            }
            catch (Exception ex)
            {
                _log.Debug(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}