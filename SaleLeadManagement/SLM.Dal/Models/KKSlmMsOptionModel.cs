using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmMsOptionModel
    {
        public List<ControlListData> GetOptionList(string optionType)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_option.Where(p => p.slm_OptionType == optionType && p.is_Deleted == 0).OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_OptionDesc, ValueField = p.slm_OptionCode }).ToList();
            }
        }

        public List<ControlListData> GetOptionListForActivityConfig(string optionType)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_option.Where(p => p.slm_OptionType == optionType && p.slm_SystemView == null && p.is_Deleted == 0).OrderBy(p => p.slm_Seq).Select(p => new ControlListData { TextField = p.slm_OptionDesc, ValueField = p.slm_OptionCode }).ToList();
            }
        }

        public List<ControlListData> GetStatusListByActivityConfig(string productId, string leadStatus)
        {
            string sql = @"SELECT ca.slm_AvailableStatus AS ValueField, opt2.slm_OptionDesc AS TextField
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_activity ca
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option opt2 ON ca.slm_AvailableStatus = opt2.slm_OptionCode AND opt2.slm_OptionType = 'lead status'  
                            WHERE ca.is_Deleted = 0 AND ca.slm_Product_Id = '" + productId + "' AND ca.slm_LeadStatus = '" + leadStatus + "'";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<ControlListData>(sql).ToList();
            }
        }
    }
}
