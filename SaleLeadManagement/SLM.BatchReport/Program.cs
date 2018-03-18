using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using SLM.Biz;
using log4net;
using System.Net;

namespace SLM.BatchReport
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Start Program at " + DateTime.Now.ToString());
                int rowperpage;
                int.TryParse(ConfigurationManager.AppSettings["ItemPerSheet"], out rowperpage);

                // generate all reports
                OBTBatchBiz batch = new OBTBatchBiz();
                string status;
                string reportPath = ConfigurationManager.AppSettings["ReportPath"];
                string folUser = ConfigurationManager.AppSettings["ReportPathUser"];
                string folPass = ConfigurationManager.AppSettings["ReportPathPassword"];
                string folDomain = ConfigurationManager.AppSettings["ReportPathDomain"];

                if (!String.IsNullOrEmpty(folUser) && !String.IsNullOrEmpty(folPass) && !String.IsNullOrEmpty(folDomain) && !String.IsNullOrEmpty(reportPath))
                {
                    if (reportPath.StartsWith("\\\\"))
                    {
                        UNCAccess unc = new UNCAccess();
                        if (!unc.Login(reportPath, folUser, folDomain, folPass))
                        {
                            string message = "Cannot access shared path " + reportPath + ": " + unc.LastError;
                            Console.WriteLine(message);
                            throw new Exception(message);
                        }
                    }
                }
                else
                {
                    throw new Exception("Some configuration is missing (ReportPath, ReportPathDomain, ReportPathUser, ReportPathPassword)");
                }

                Console.WriteLine(curtime + "Generating Report 001"); //แจ้งออกBillPayment
                if (!batch.GenReport001(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 001 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 002"); //แจ้งต่อประกัน
                if (!batch.GenReport002(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 002 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 003"); //รายงานแจ้งระงับเคลม
                if (!batch.GenReport003(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 003 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 004");  //รายงานยกเลิกกรมธรรม์
                if (!batch.GenReport004(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 004 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 005"); //รายงานแจ้งออก พรบ
                if (!batch.GenReport005(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 005 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 006");  //รายงานแจ้งยกเลิกออก พรบ
                if (!batch.GenReport006(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 006 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 007");  //รายงานแจ้งกลับงานติดปัญหา
                if (!batch.GenReport007(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 007 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 008");  //รายงานขอออกสำเนากรมธรรมใหม่
                if (!batch.GenReport008(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 008 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 009"); //รายงานขอออกสำเนา พรบ ใหม่
                if (!batch.GenReport009(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 009 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 010");  //รายงานข้อมูลการแก้ไขใบเสร็จ
                if (!batch.GenReport010(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 010 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Generating Report 018");  //รายงานแจ้งยกเลิกระงับเคลม
                if (!batch.GenReport018(DateTime.Now, out status, rowperpage, reportPath))
                {
                    _log.Error("Report 018 : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else {
                    if (status != "") Console.WriteLine(curtime + status);
                    Console.WriteLine(curtime + "Finished");
                }

                Console.WriteLine(curtime + "Delete Empty Folder");  //รายงานแจ้งยกเลิกระงับเคลม
                if (!batch.DeleteEmptyFolder(DateTime.Now, reportPath))
                {
                    _log.Error("Delete Empty Folder : " + batch.ErrorMessage);
                    Console.WriteLine(curtime + batch.ErrorMessage);
                }
                else
                {
                    Console.WriteLine(curtime + "Finished");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException == null ? ex.Message : ex.InnerException.Message, ex);
            }

        }

        static string curtime
        {
            get { return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - "; }
        }

        static void showHelp()
        {
            // show help
            Console.WriteLine("KKBank OBT Batch Report v1.00.000");
            Console.WriteLine("");
            Console.WriteLine("Syntax:");
            Console.WriteLine("SLMBatchReport /all");
            Console.WriteLine("SLMBatchReport /r:[reportid] /d:[date]");
            Console.WriteLine("");
            Console.WriteLine("   /all             Generate all reports for current running date.");
            Console.WriteLine("");
            Console.WriteLine("   /r:[reportid]    report id to generate");
            Console.WriteLine("   /d:[date]        date to generate in formate yyyy-mm-dd (19xx format)");
        }
    }
}
