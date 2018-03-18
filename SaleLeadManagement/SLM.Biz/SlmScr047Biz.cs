using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Resource;
using SLM.Dal;
using System.Data.Objects.SqlClient;

namespace SLM.Biz
{
    public class SlmScr047Biz
    {

        // level
        public static List<kkslm_tr_preleadportmonitor> GetTransferList(DateTime start, DateTime end)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                end = end.AddDays(1).AddSeconds(-1);
                var result = sdc.kkslm_tr_preleadportmonitor
                    .Where(tr => tr.is_Deleted == false && start <= tr.slm_CreatedDate && tr.slm_CreatedDate <= end)
                    .OrderBy(tr => tr.slm_CreatedDate)
                    .ToList();
                result.ForEach(tr => {
                        switch (tr.slm_PortStatus.ToUpper())
                        {
                            case "W": tr.slm_PortStatus = "Waiting"; break;
                            case "L": tr.slm_PortStatus = "Allocated"; break;   //Rule แจกงาน
                            case "C": tr.slm_PortStatus = "Confirmed"; break;
                            case "R": tr.slm_PortStatus = "Rejected"; break;
                            case "A": tr.slm_PortStatus = "Assigned"; break;    //Rule จ่ายงาน
                            default:
                                break;
                        }
                    });

                //Comment By Pom 23/05/2016
                //for (int i = 0; i < result.Count; i++)
                //    result[i].slm_PreleadMonitorId = i + 1;

                return result;
            }
        }

    }

}
