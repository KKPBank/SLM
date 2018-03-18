using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_020 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_020));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาข้อมูลข้อเสนอแนะ";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    GenerateList();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex);
            //upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                //DoSearchLeadData(pageControl.SelectedPageIndex, SortExpressionProperty, SortDirectionProperty);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        //Mocup
        private void GenerateList()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No"));
            dt.Columns.Add(new DataColumn("Topic"));
            dt.Columns.Add(new DataColumn("Type"));
            dt.Columns.Add(new DataColumn("Detail"));
            dt.Columns.Add(new DataColumn("CreatedUser"));
            dt.Columns.Add(new DataColumn("CreatedDate"));

            DataRow dr1 = dt.NewRow();
            dr1["No"] = "1";
            dr1["Topic"] = "หัวข้อ 1";
            dr1["Type"] = "ข้อเสนอแนะ";
            dr1["Detail"] = "ระบบดีมาก";
            dr1["CreatedUser"] = "ธนายุทธ์ แซ่จู";
            dr1["CreatedDate"] = "17/03/2015 15:30:00";
            dt.Rows.Add(dr1);

            DataRow dr2 = dt.NewRow();
            dr2["No"] = "2";
            dr2["Topic"] = "หัวข้อ 2";
            dr2["Type"] = "ข้อเสนอแนะ";
            dr2["Detail"] = "ประทับใจระบบมาก";
            dr2["CreatedUser"] = "ธนายุทธ์ แซ่จู";
            dr2["CreatedDate"] = "17/03/2015 15:30:00";
            dt.Rows.Add(dr2);

            DataRow dr3 = dt.NewRow();
            dr3["No"] = "3";
            dr3["Topic"] = "หัวข้อ 3";
            dr3["Type"] = "ข้อเสนอแนะ";
            dr3["Detail"] = "ประทับใจระบบมาก";
            dr3["CreatedUser"] = "ธนายุทธ์ แซ่จู";
            dr3["CreatedDate"] = "17/03/2015 15:30:00";
            dt.Rows.Add(dr3);

            gvResult.DataSource = dt;
            gvResult.DataBind();
        }
    }
}