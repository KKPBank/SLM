using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;

namespace SLM.Biz
{
    public class ReportMenuBiz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        public object GetMenuList(string userName, int staffTypeId = 0)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                int local_staffTypeId;
                if (staffTypeId == 0)
                {
                    var staff = StaffBiz.GetStaff(userName);
                    local_staffTypeId = Convert.ToInt32(staff.StaffTypeId);
                }
                else
                {
                    local_staffTypeId = staffTypeId;
                }

                return (from mnu in sdc.kkslm_ms_report_menu
                        join typ in sdc.kkslm_ms_report_menu_role on mnu.slm_ReportMenuId equals typ.slm_ReportMenuId
                        where typ.slm_StaffTypeId == local_staffTypeId
                        select new { mnu.slm_ReportMenuId, mnu.slm_ReportMenuName, mnu.slm_ReportMenuUrl, mnu.slm_Description }
                                ).ToList();
            }
        }
    }
}
