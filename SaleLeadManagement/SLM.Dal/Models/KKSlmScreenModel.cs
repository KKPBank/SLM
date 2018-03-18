using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmScreenModel
    {
        private string SLMDBName = System.Configuration.ConfigurationManager.AppSettings["SLMDBName"];

        public ScreenPrivilegeData GetScreenPrivilege(string username, string screenDesc)
        {
            string screenCode = "";
            
            //ปรับเลข Screen ใหม่
            switch (screenDesc)
            { 
                case "SLM_SCR_004":
                    screenCode = "SLM_SCR_005";
                    break;
                case "SLM_SCR_010":
                    screenCode = "SLM_SCR_003";
                    break;
                case "SLM_SCR_011":
                    screenCode = "SLM_SCR_004";
                    break;
                case "SLM_SCR_015":
                    screenCode = "SLM_SCR_006";
                    break;
                case "SLM_SCR_016":
                    screenCode = "SLM_SCR_007";
                    break;
                case "SLM_SCR_017":
                    screenCode = "SLM_SCR_008";
                    break;
                case "SLM_SCR_018":
                    screenCode = "SLM_SCR_010";
                    break;
                case "SLM_SCR_019":
                    screenCode = "SLM_SCR_009";
                    break;
                default:
                    screenCode = screenDesc;
                    break;
            }

            string sql = $@"SELECT V.slm_StaffTypeId AS StaffTypeId, V.is_Save AS IsSave, V.is_View AS IsView, S.slm_ScreenDesc AS ScreenDesc 
                            FROM dbo.kkslm_ms_validate V
                            INNER JOIN dbo.kkslm_screen S ON V.slm_ScreenId = S.slm_ScreenId
                            INNER JOIN dbo.kkslm_ms_staff staff WITH (NOLOCK) ON staff.slm_StaffTypeId = V.slm_StaffTypeId
                            WHERE V.is_Deleted = 0 AND S.is_Deleted = 0
                            AND staff.slm_UserName = '{username}' AND S.slm_ScreenDesc = '{screenCode}'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ScreenPrivilegeData>(sql).FirstOrDefault();
            }
        }

        public List<ScreenPrivilegeData> GetScreenPrivilegeList(int staffTypeId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string sql = $@"SELECT v.slm_ScreenId AS ScreenId, v.slm_StaffTypeId AS StaffTypeId, v.is_View AS IsView, s.slm_ScreenDesc AS ScreenDesc
                                FROM kkslm_ms_validate v
                                INNER JOIN kkslm_screen s on v.slm_ScreenId = s.slm_ScreenId
                                WHERE v.slm_StaffTypeId = '{staffTypeId.ToString()}' AND v.is_Deleted = 0 ";

                return slmdb.ExecuteStoreQuery<ScreenPrivilegeData>(sql).ToList();
            }
        }
    }
}
