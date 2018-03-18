using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using log4net;
using System.Data;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Drawing;
using System.Threading;

namespace SLM.Application
{
    /// <summary>
    /// Summary description for DownloadFile
    /// </summary>
    public class DownloadFile : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(DownloadFile));
        private string formName;

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                System.Web.HttpRequest request = HttpContext.Current.Request;
                formName = request.QueryString["val"];

                //Thread.Sleep(2000); //1000 = 1sec

                if (context.Session[SLM.Resource.SLMConstant.SessionExport] != null)
                {
                    switch (formName)
                    {
                        case "userMonitoringReInsurance":
                            ExportUserMonitoringReInsurance();
                            break;

                        case "jobDelegateSuccessTab":
                            ExportJobDelegateSuccessTab();
                            break;
                        case "jobDelegateDedupTab":
                            ExportJobDelegateDedupTab();
                            break;
                        case "jobDelegateBlacklistTab":
                            ExportJobDelegateBlacklistTab();
                            break;
                        
                        default:
                            break;
                    }
                }
                else
                {
                    string message = "Session null";
                    _log.Debug(message);
                    context.Response.Write("<script type='text/javascript'>alert('" + message + "');</script>");
                }
                context.Session[SLM.Resource.SLMConstant.SessionExport] = null;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                if (message != "Thread was being aborted.")
                {
                    _log.Debug(message);
                    context.Response.Write("<script type='text/javascript'>alert('" + message + "');</script>");
                }
            }
        }

        private int xNum;

        private int NextNum(int x)
        {
            if (x == 0)
                xNum = 0;
            else
                xNum += 1;

            return xNum;
        }

        private void ExportUserMonitoringReInsurance()
        {
            DataTable dt = (DataTable)HttpContext.Current.Session[SLM.Resource.SLMConstant.SessionExport];

            ////Order Column and Delete Other Column
            //dt.Columns["Role"].SetOrdinal(NextNum(0));
            //dt.Columns["User ID"].SetOrdinal(NextNum(1));
            //dt.Columns["ชื่อ-นามสกุลพนักงาน"].SetOrdinal(NextNum(1));
            ////dt.Columns["สถานะการทำงาน"].SetOrdinal(NextNum(1));
            //dt.Columns["สนใจ"].SetOrdinal(NextNum(1));
            //dt.Columns["รอติดต่อลูกค้า"].SetOrdinal(NextNum(1));
            //dt.Columns["อยู่ระหว่างดำเนินการ"].SetOrdinal(NextNum(1));
            //dt.Columns["รอผลการพิจารณา"].SetOrdinal(NextNum(1));
            //dt.Columns["อนุมัติตามเสนอ"].SetOrdinal(NextNum(1));
            //dt.Columns["ส่งกลับแก้ไข"].SetOrdinal(NextNum(1));
            //dt.Columns["รวม Lead ที่อยู่ในมือ"].SetOrdinal(NextNum(1));
            dt.Columns["ชื่อทีม"].SetOrdinal(NextNum(0));
            dt.Columns["รหัสพนักงาน"].SetOrdinal(NextNum(1));
            dt.Columns["สถานะของ Lead"].SetOrdinal(NextNum(1));
            dt.Columns["สถานะย่อยของ Lead"].SetOrdinal(NextNum(1));
            dt.Columns["เลขที่สัญญา"].SetOrdinal(NextNum(1));
            dt.Columns["ชื่อ-สกุล ลูกค้า"].SetOrdinal(NextNum(1));
            dt.Columns["ชื่อบริษัทประกันภัย"].SetOrdinal(NextNum(1));
            dt.Columns["ประเภทประกันภัย"].SetOrdinal(NextNum(1));
            dt.Columns["ค่าเบี้ยประกัน"].SetOrdinal(NextNum(1)); 
            dt.Columns["Grade ลูกค้า"].SetOrdinal(NextNum(1));
            dt.Columns["Prelead ID"].SetOrdinal(NextNum(1));
            dt.Columns["Lot"].SetOrdinal(NextNum(1));
            dt.Columns["Transfer Date"].SetOrdinal(NextNum(1));

            dt = DeleteOtherColumn(dt, NextNum(1));

            ExportTableData(dt);
        }
        private void ExportJobDelegateSuccessTab()
        {
            DataTable dt = (DataTable)HttpContext.Current.Session[SLM.Resource.SLMConstant.SessionExport];

            dt.Columns["slm_TeamTelesales_Code"].SetOrdinal(NextNum(0));
            dt.Columns["slm_TeamTelesales_Code"].ColumnName = "ชื่อทีม";
            dt.Columns["slm_EmpCode"].SetOrdinal(NextNum(1)); 
            dt.Columns["slm_EmpCode"].ColumnName = "รหัสพนักงาน";
            dt.Columns["slm_MonthNameTh"].SetOrdinal(NextNum(1));
            dt.Columns["slm_MonthNameTh"].ColumnName = "เดือนที่คุ้มครอง";
            dt.Columns["slm_Contract_Number"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Contract_Number"].ColumnName = "Contract";
            dt.Columns["CustomerName"].SetOrdinal(NextNum(1));
            dt.Columns["CustomerName"].ColumnName = "ชื่อ-สกุล ลูกค้า";
            dt.Columns["slm_InsNameTh"].SetOrdinal(NextNum(1));
            dt.Columns["slm_InsNameTh"].ColumnName = "บริษัทประกันภัย";
            dt.Columns["slm_ConverageTypeName"].SetOrdinal(NextNum(1));
            dt.Columns["slm_ConverageTypeName"].ColumnName = "ประเภทประกันภัย";
            dt.Columns["slm_Voluntary_Gross_Premium"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Voluntary_Gross_Premium"].ColumnName = "ค่าเบี้ยประกัน";
            dt.Columns["slm_CreatedDate"].SetOrdinal(NextNum(1));
            dt.Columns["slm_CreatedDate"].ColumnName = "Transfer Date";
            dt.Columns["slm_Grade"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Grade"].ColumnName = "Grade ลูกค้า";
            dt.Columns["slm_Prelead_Id"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Prelead_Id"].ColumnName = "Prelead Id";
            dt.Columns["slm_CmtLot"].SetOrdinal(NextNum(1));
            dt.Columns["slm_CmtLot"].ColumnName = "Lot";
            
            dt = DeleteOtherColumn(dt, NextNum(1));

            ExportTableData(dt);
        }

        private void ExportJobDelegateDedupTab()
        {
            DataTable dt = (DataTable)HttpContext.Current.Session[SLM.Resource.SLMConstant.SessionExport];

            dt.Columns.Add("ชื่อทีม");
            dt.Columns["ชื่อทีม"].SetOrdinal(NextNum(0));
            dt.Columns.Add("รหัสพนักงาน");
            dt.Columns["รหัสพนักงาน"].SetOrdinal(NextNum(1));            
            dt.Columns["slm_MonthNameTh"].SetOrdinal(NextNum(1));
            dt.Columns["slm_MonthNameTh"].ColumnName = "เดือนที่คุ้มครอง";
            dt.Columns["slm_Contract_Number"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Contract_Number"].ColumnName = "Contract";
            dt.Columns["CustomerName"].SetOrdinal(NextNum(1));
            dt.Columns["CustomerName"].ColumnName = "ชื่อ-สกุล ลูกค้า";
            dt.Columns["slm_InsNameTh"].SetOrdinal(NextNum(1));
            dt.Columns["slm_InsNameTh"].ColumnName = "บริษัทประกันภัย";
            dt.Columns["slm_ConverageTypeName"].SetOrdinal(NextNum(1));
            dt.Columns["slm_ConverageTypeName"].ColumnName = "ประเภทประกันภัย";
            dt.Columns["slm_Voluntary_Gross_Premium"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Voluntary_Gross_Premium"].ColumnName = "ค่าเบี้ยประกัน";
            dt.Columns["slm_CreatedDate"].SetOrdinal(NextNum(1));
            dt.Columns["slm_CreatedDate"].ColumnName = "Transfer Date";
            dt.Columns["slm_Grade"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Grade"].ColumnName = "Grade ลูกค้า";
            dt.Columns["slm_TempId"].SetOrdinal(NextNum(1));
            dt.Columns["slm_TempId"].ColumnName = "Temp Id";
            dt.Columns["slm_CmtLot"].SetOrdinal(NextNum(1));
            dt.Columns["slm_CmtLot"].ColumnName = "Lot";

            dt = DeleteOtherColumn(dt, NextNum(1));

            ExportTableData(dt);
        }

        private void ExportJobDelegateBlacklistTab()
        {
            DataTable dt = (DataTable)HttpContext.Current.Session[SLM.Resource.SLMConstant.SessionExport];

            dt.Columns.Add("ชื่อทีม");
            dt.Columns["ชื่อทีม"].SetOrdinal(NextNum(0));
            dt.Columns.Add("รหัสพนักงาน");
            dt.Columns["รหัสพนักงาน"].SetOrdinal(NextNum(1));
            dt.Columns["slm_MonthNameTh"].SetOrdinal(NextNum(1));
            dt.Columns["slm_MonthNameTh"].ColumnName = "เดือนที่คุ้มครอง";
            dt.Columns["slm_Contract_Number"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Contract_Number"].ColumnName = "Contract";
            dt.Columns["CustomerName"].SetOrdinal(NextNum(1));
            dt.Columns["CustomerName"].ColumnName = "ชื่อ-สกุล ลูกค้า";
            dt.Columns["slm_InsNameTh"].SetOrdinal(NextNum(1));
            dt.Columns["slm_InsNameTh"].ColumnName = "บริษัทประกันภัย";
            dt.Columns["slm_ConverageTypeName"].SetOrdinal(NextNum(1));
            dt.Columns["slm_ConverageTypeName"].ColumnName = "ประเภทประกันภัย";
            dt.Columns["slm_Voluntary_Gross_Premium"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Voluntary_Gross_Premium"].ColumnName = "ค่าเบี้ยประกัน";
            dt.Columns["slm_CreatedDate"].SetOrdinal(NextNum(1));
            dt.Columns["slm_CreatedDate"].ColumnName = "Transfer Date";
            dt.Columns["slm_Grade"].SetOrdinal(NextNum(1));
            dt.Columns["slm_Grade"].ColumnName = "Grade ลูกค้า";
            dt.Columns["slm_TempId"].SetOrdinal(NextNum(1));
            dt.Columns["slm_TempId"].ColumnName = "Temp Id";
            dt.Columns["slm_CmtLot"].SetOrdinal(NextNum(1));
            dt.Columns["slm_CmtLot"].ColumnName = "Lot";

            dt = DeleteOtherColumn(dt, NextNum(1));

            ExportTableData(dt);
        }

        private DataTable DeleteOtherColumn(DataTable dt, int startDel)
        {
            int colCount = dt.Columns.Count;
            for (int i = startDel; i < colCount; i++)
            {
                dt.Columns.RemoveAt(startDel);
            }
            return dt;
        }

        public void ExportTableData(DataTable dt)
        {
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            string attach = "attachment;filename=" + formName + ".xls";
            response.ClearContent();
            response.AddHeader("content-disposition", attach);
            response.ContentType = "application/ms-excel";
            response.ContentEncoding = System.Text.Encoding.Unicode;
            response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            if (dt != null)
            {
                string tab = "";
                foreach (DataColumn dc in dt.Columns)
                {
                    response.Write(tab + dc.ColumnName);
                    tab = "\t";
                }
                response.Write(System.Environment.NewLine);
                foreach (DataRow dr in dt.Rows)
                {
                    tab = "";
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        response.Write(tab + dr[i].ToString().Replace("\n", " "));
                        tab = "\t";
                    }
                    response.Write("\n");
                }

                response.Flush();
                response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}