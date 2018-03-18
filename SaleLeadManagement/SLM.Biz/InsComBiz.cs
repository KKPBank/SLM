using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;
using SLM.Dal;


namespace SLM.Biz
{
    public class InsComBiz
    {
        public static List<ControlListData> GetInsComList()
        {
            OperKKSlmMsInsComModel inscom = new OperKKSlmMsInsComModel();
            return inscom.GetInsComList();
        }

        public static string GetInsComName(decimal insComId)
        {
            OperKKSlmMsInsComModel inscom = new OperKKSlmMsInsComModel();
            return inscom.GetInsComName(insComId);
        }

        public static List<ControlListData> GetInsComListWithOld(decimal oldId)
        {
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                var list = operdb.kkslm_ms_ins_com.Where(p => p.is_Deleted == false || p.slm_Ins_Com_Id == oldId).OrderByDescending(p=>p.is_Deleted).ThenBy(p => p.slm_InsNameTh).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_InsNameTh, ValueField = p.slm_Ins_Com_Id.ToString() }).ToList();
                return list;
            }
        }
    }
}
