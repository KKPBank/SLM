using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Application.Shared;
using SLM.Application.Utilities;
using SLM.Resource;

namespace SLM.Application
{
    public partial class SLM_SCR_046 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LeadInfo));
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ((Label)Page.Master.FindControl("lblTopic")).Text = "แก้ไขข้อมูล Prelead";
                    //ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_046");
                    //if (priData == null || priData.IsView != 1)
                    //    AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_046.aspx");

                    var prid = SLMUtil.SafeDecimal(Request["preleadid"]);
                    LoadData(prid);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        private void BuildCombo(DropDownList cmb, List<ControlListData> lst) { BuildCombo(cmb, lst, ""); }
        private void BuildCombo(DropDownList cmb, List<ControlListData> lst, string Blanktext)
        {
            cmb.DataSource = lst;
            cmb.DataTextField = "TextField";
            cmb.DataValueField = "ValueField";
            cmb.DataBind();
            cmb.Items.Insert(0, new ListItem(Blanktext, ""));
        }

        private void LoadData(decimal preleadId)
        {
            SlmScr046Biz bz = new SlmScr046Biz();
            var pl = bz.GetPreleadDetail(preleadId);
            if (pl != null)
                ctlPrelead.SetData(pl);
            else
                AppUtil.ClientAlert(this, "ไม่พบข้อมูลจาก Prelead Id ที่ระบุ");
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ctlPrelead.ValidateData("")) return;

                SlmScr046Biz bz = new SlmScr046Biz();
                var pl = ctlPrelead.GetData();
                if (!bz.SavePrelead(pl, Page.User.Identity.Name))
                    AppUtil.ClientAlert(this, bz.ErrorMessage);
                else
                    AppUtil.ClientAlertAndRedirect(this, "บันทึกข้อมูลเรียบร้อย", "SLM_SCR_029.aspx?backtype=" + Request["backtype"]);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("SLM_SCR_029.aspx?backtype=" + Request["backtype"]);
        }

    }
}