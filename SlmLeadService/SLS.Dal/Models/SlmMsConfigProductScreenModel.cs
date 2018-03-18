using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;

namespace SLS.Dal.Models
{
    public class SlmMsConfigProductScreenModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmMsConfigProductScreenModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public string GetTableName(string campaignId, string productId, string actionType, string operationFlag)
        {
            try
            {
                string tableName = "";

                var config_cam = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_CampaignId == campaignId && p.slm_ActionType == actionType && p.is_Deleted == false).FirstOrDefault();
                if (config_cam != null)
                    tableName = config_cam.slm_TableName;
                else
                {
                    var config_pro = slmdb.kkslm_ms_config_product_screen.Where(p => p.slm_Product_Id == productId && p.slm_ActionType == actionType && p.is_Deleted == false).FirstOrDefault();
                    if (config_pro != null)
                        tableName = config_pro.slm_TableName; 
                }

                return tableName;
            }
            catch (Exception ex)
            {
                if (operationFlag == ApplicationResource.INS_OPERATION)
                    throw new ServiceException(ApplicationResource.INS_INSERT_FAIL_CODE, ApplicationResource.INS_INSERT_FAIL_DESC, ex.Message, ex.InnerException);
                else if (operationFlag == ApplicationResource.UPD_OPERATION)
                    throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
                else
                    throw new ServiceException(ApplicationResource.SEARCH_SEARCH_FAIL_CODE, ApplicationResource.SEARCH_SEARCH_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }
    }
}
