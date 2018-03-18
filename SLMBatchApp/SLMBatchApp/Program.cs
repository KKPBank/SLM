using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SLMBatch.Business;
using System.Threading;
using System.Globalization;
using log4net;

namespace SLMBatchApp
{
    class Program
    {
        private static ILog _log;
        private static string logfilename = String.Empty;

        static void Main(string[] args)
        {
            try
            {
                // Set logfile name and application name variables
                log4net.GlobalContext.Properties["ApplicationCode"] = "SLMBatchApp";
                log4net.GlobalContext.Properties["ServerName"] = System.Environment.MachineName;
                _log = LogManager.GetLogger(typeof(Program));
            }
            catch (Exception ex)
            {
                _log.Error("Exception occur:\n", ex);
            }


            bool ret = true;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            logfilename = string.Format("{0}.txt", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
            //==================================================================================================

            //OBT_PRO_09_Biz obtpro09 = new OBT_PRO_09_Biz();
            //obtpro09.SetLogFileName = logfilename;
            //obtpro09.UpdatePolicyNo("OBT_PRO_09");

            if (args.Count() > 0)
            {
                switch (args[0])
                {
                    case "OBT_PRO_01":
                        OBT_PRO_01_Biz obtpro01 = new OBT_PRO_01_Biz();
                        obtpro01.SetLogFileName = logfilename;
                        obtpro01.InsertReinsuranceView(args[0]);
                        break;
                    case "OBT_PRO_03":
                        OBT_PRO_03_Biz obtpro03 = new OBT_PRO_03_Biz();
                        obtpro03.SetLogFileName = logfilename;
                        obtpro03.InsertTelesalesOwnertoHP(args[0]);
                        break;
                    case "OBT_PRO_04":
                        OBT_PRO_04_Biz obtpro04 = new OBT_PRO_04_Biz();
                        obtpro04.SetLogFileName = logfilename;
                        long batchMonitorId = obtpro04.InsertPreLead(args[0]);
                        if (batchMonitorId != -1)
                        {
                            obtpro04.CheckForceBlacklistToDedup(batchMonitorId, args[0]);
                            obtpro04.DeletePreleadAssignLog(batchMonitorId, args[0]);
                        }
                        break;
                    case "OBT_PRO_06":
                        OBT_PRO_06_Biz obtpro06 = new OBT_PRO_06_Biz();
                        obtpro06.SetLogFileName = logfilename;
                        ret = obtpro06.InsertCoverageType(args[0]);
                        break;
                    case "OBT_PRO_07":
                        OBT_PRO_07_Biz obtpro07 = new OBT_PRO_07_Biz();
                        obtpro07.SetLogFileName = logfilename;
                        ret = obtpro07.UpdatePayment(args[0]);
                        if (ret)
                        {
                            OBT_PRO_26_Biz obtpro26_cont = new OBT_PRO_26_Biz();
                            obtpro26_cont.SetLogFileName = logfilename;
                            ret = obtpro26_cont.ExportExcelPaymentPending("OBT_PRO_26");
                        }
                        break;
                    case "OBT_PRO_08":
                        OBT_PRO_08_Biz obtpro08 = new OBT_PRO_08_Biz();
                        obtpro08.SetLogFileName = logfilename;
                        obtpro08.UpdateSaleTransaction(args[0]);
                        break;
                    case "OBT_PRO_09":
                        OBT_PRO_09_Biz obtpro09 = new OBT_PRO_09_Biz();
                        obtpro09.SetLogFileName = logfilename;
                        ret = obtpro09.UpdatePolicyNo(args[0]);
                        if (ret)
                        {
                            OBT_PRO_29_Biz obtpro29_cont = new OBT_PRO_29_Biz();
                            obtpro29_cont.SetLogFileName = logfilename;
                            ret = obtpro29_cont.ExportExcelPolicyNoActNoPending("OBT_PRO_29");
                        }
                        break;
                    case "OBT_PRO_10":
                        OBT_PRO_10_Biz obtpro10 = new OBT_PRO_10_Biz();
                        obtpro10.SetLogFileName = logfilename;
                        obtpro10.CalculatePerformance(args[0]);
                        break;
                    case "OBT_PRO_24":
                        OBT_PRO_24_Biz obtpro24 = new OBT_PRO_24_Biz();
                        obtpro24.SetLogFileName = logfilename;
                        obtpro24.PurgeData(args[0]);
                        break;
                    case "OBT_PRO_26":
                        OBT_PRO_26_Biz obtpro26 = new OBT_PRO_26_Biz();
                        obtpro26.SetLogFileName = logfilename;
                        ret = obtpro26.ExportExcelPaymentPending(args[0]);
                        break;
                    case "OBT_PRO_27":
                        OBT_PRO_27_Biz obtpro27 = new OBT_PRO_27_Biz();
                        obtpro27.SetLogFileName = logfilename;
                        obtpro27.GenerateSMS(args[0]);
                        break;
                    case "OBT_PRO_29":
                        OBT_PRO_29_Biz obtpro29 = new OBT_PRO_29_Biz();
                        obtpro29.SetLogFileName = logfilename;
                        ret = obtpro29.ExportExcelPolicyNoActNoPending(args[0]);
                        break;
                    case "OBT_PRO_30":
                        OBT_PRO_30_Biz obtpro30 = new OBT_PRO_30_Biz();
                        obtpro30.SetLogFileName = logfilename;
                        ret = obtpro30.ExportExcelLeadsForTransfer(args[0]);
                        break;
                    case "OBT_PRO_31":
                        OBT_PRO_31_Biz obtpro31 = new OBT_PRO_31_Biz();
                        obtpro31.SetLogFileName = logfilename;
                        ret = obtpro31.ExportExcelLeadsForTKS(args[0]);
                        break;
                    case "OBT_PRO_32":
                        OBT_PRO_32_Biz obtpro32 = new OBT_PRO_32_Biz();
                        obtpro32.SetLogFileName = logfilename;
                        obtpro32.GenerateSMS(args[0]);
                        break;
                    case "OBT_PRO_33":
                        OBT_PRO_33_Biz obtpro33 = new OBT_PRO_33_Biz();
                        obtpro33.SetLogFileName = logfilename;
                        obtpro33.GenerateSMS(args[0]);
                        break;
                    case "OBT_PRO_34":
                        OBT_PRO_34_Biz obtpro34 = new OBT_PRO_34_Biz();
                        obtpro34.SetLogFileName = logfilename;
                        obtpro34.ImportNotifyPremium(args[0]);
                        break;
                    case "SLM_PRO_01":
                        SLM_PRO_01_Biz slmpro01 = new SLM_PRO_01_Biz();
                        slmpro01.SetLogFileName = logfilename;
                        slmpro01.CalculateJobOnHand(args[0]);
                        break;
                    case "SLM_PRO_02":
                        SLM_PRO_02_Biz slmpro02 = new SLM_PRO_02_Biz();
                        slmpro02.SetLogFileName = logfilename;
                        slmpro02.UpdateStatusBackEnd(args[0]);
                        break;
                    case "SLM_PRO_05":
                        SLM_PRO_05_Biz slmpro05 = new SLM_PRO_05_Biz();
                        slmpro05.SetLogFileName = logfilename;
                        slmpro05.ResetRunningOfTicketId(args[0]);
                        break;
                    case "SLM_PRO_06":
                        SLM_PRO_06_Biz slmpro06 = new SLM_PRO_06_Biz();
                        slmpro06.SetLogFileName = logfilename;
                        slmpro06.ExportWSLog(args[0]);
                        break;
                    case "SLM_PRO_08":
                        SLM_PRO_08_Biz slmpro08 = new SLM_PRO_08_Biz();
                        slmpro08.SetLogFileName = logfilename;
                        ret = slmpro08.UpdateBatchCARInsertStatus(args[0]);
                        break;
                    default:
                        break;
                }
            }

            Console.Write(ret ? "SUCCESS" : "FAIL");
        }
    }
}
