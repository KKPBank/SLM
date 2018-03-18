using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Application.Utilities;

namespace SLM.Application
{
    public partial class SLM_SCR_011 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ((Label)Page.Master.FindControl("lblTopic")).Text = "แก้ไขข้อมูล Lead (Edit)";

                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_011");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    if (Request["ticketid"] != null && Request["ticketid"].ToString().Trim() != string.Empty)
                    {
                        CheckTicketIdPrivilege(Request["ticketid"].ToString().Trim());
                    }
                    else
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "Ticket Id not found", "SLM_SCR_003.aspx");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        private void CheckTicketIdPrivilege(string ticketId)
        {
            //if (!RoleBiz.GetTicketIdPrivilege(ticketId, HttpContext.Current.User.Identity.Name, SlmScr003Biz.GetStaffType(HttpContext.Current.User.Identity.Name), ""))
            //{
            //    string message = "ข้อมูลผู้มุ่งหวังรายนี้ ท่านไม่มีสิทธิในการมองเห็น";
            //    LeadOwnerDelegateData data = SlmScr011Biz.GetOwnerAndDelegateName(ticketId);
            //    if (data != null)
            //    {
            //        if (!string.IsNullOrEmpty(data.OwnerName) && !string.IsNullOrEmpty(data.DelegateName))
            //            message += " ณ ปัจจุบันผู้เป็นเจ้าของ คือ " + data.OwnerName.ToString().Trim() + " และ Delegate คือ " + data.DelegateName.ToString().Trim();
            //        else if (!string.IsNullOrEmpty(data.OwnerName))
            //            message += " ณ ปัจจุบันผู้เป็นเจ้าของ คือ " + data.OwnerName.ToString().Trim();
            //        else if (!string.IsNullOrEmpty(data.DelegateName))
            //            message += " ณ ปัจจุบัน Delegate คือ " + data.DelegateName.ToString().Trim();
            //    }
            //    else
            //        message = "ไม่พบ Ticket Id " + Request["ticketid"].ToString() + " ในระบบ";

            //    AppUtil.ClientAlertAndRedirect(Page, message, "SLM_SCR_003.aspx");
            //}
        }
    }
}