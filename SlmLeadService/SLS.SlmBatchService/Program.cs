using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Biz;
using log4net;

namespace SLS.SlmBatchService
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            try
            {
                log.Info("================================================================");
                log.Info(string.Format("Start at {0}", ToString(DateTime.Now)));
                log.Info(string.Format("CreateLead Start at {0}", ToString(DateTime.Now)));

                bool ret = false;

                if (args.Any())
                {
                    switch (args[0])
                    {
                        case "SLM_PRO_07":
                            UploadLeadBiz biz = new UploadLeadBiz(args[0]);
                            ret = biz.CreateLead();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ret = false;
                    log.Error("No argument provided");
                }

                log.Info(string.Format("CreateLead End at {0}", ToString(DateTime.Now)));
                Console.Write(ret ? "SUCCESS" : "FAIL");
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            finally
            {
                log.Info(string.Format("End at {0}", ToString(DateTime.Now)));
                log.Info("================================================================");
            }
        }

        private static string ToString(DateTime datetime)
        {
            return datetime.ToString("dd/MM/") + datetime.Year.ToString() + " " + datetime.ToString("HH:mm:ss");
        }
    }
}
