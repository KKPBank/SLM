using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmTrPreleadInfoModel
    {
        public PreleadViewData GetPreleadData(decimal preleadId, string licenseNo, string campaignId, string ContractNo)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    int count = 0;
                    var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == preleadId).FirstOrDefault();
                    if (prelead != null)
                    {
                        count = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_CampaignId == prelead.slm_CampaignId).Count();
                        if (count > 0)
                        {
                            return GetPreleadData(preleadId.ToString(), "campaign", licenseNo, campaignId, ContractNo);
                        }
                        else
                        {
                            count = slmdb.kkslm_ms_config_product_substatus.Where(p => p.slm_Product_Id == prelead.slm_Product_Id).Count();
                            if (count > 0)
                            {
                                return GetPreleadData(preleadId.ToString(), "product", licenseNo, campaignId, ContractNo);
                            }
                            else
                            {
                                throw new Exception("ไม่พบข้อมูลสถานะย่อยในตาราง kkslm_ms_config_product_substatus ของ CampaignId " + prelead.slm_CampaignId + ", ProductId " + prelead.slm_Product_Id);
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private PreleadViewData GetPreleadData(string preleadId, string flag, string licenseNo, string campaignId, string contractNo)
        {
            try
            {
                string sql = @"select pre.slm_Prelead_Id AS PreleadId, pre.slm_Contract_Number AS ContractNo, tt.slm_TeamTelesales_Code as TeamCode , ISNULL(title1.slm_TitleName, '') + pre.slm_Name as Firstname
                                , pre.slm_LastName AS Lastname ,contact.slm_Mobile_Phone AS TelNo1, pre.slm_TicketId AS TicketId, mo.slm_OptionDesc AS StatusDesc, cs.slm_SubStatusName AS SubStatusDesc
                                , pre.slm_CampaignId AS CampaignId, cam.slm_CampaignName as CampaignName, prephone1.slm_CreatedDate AS PreContactLatestDate
                                , pre.slm_AssignDate AS AssignedDate, st.slm_BranchCode AS BranchCode, OwnerBranch.slm_BranchName AS OwnerBranchName, pre.slm_Owner AS OwnerEmpCode
                                , st.slm_StaffNameTH as OwnerName, pre.slm_CitizenId AS CitizenId, cardtype.slm_CardTypeName AS CardTypeName, pre.slm_CardTypeId AS CardTypeId
                                , pre.slm_CampaignId AS CampaignId, pre.slm_Product_Id AS ProductId, pre.slm_Status AS StatusCode, pre.slm_SubStatus AS SubStatusCode, pre.slm_NextContactDate AS NextContactDate
                                , mapproduct.product_id AS ProductGroupId, mapproduct.sub_product_name AS ProductName, pg.product_name AS ProductGroupName, cardtype.slm_CARSubscriptionTypeId AS SubScriptionTypeId
                                , pre.slm_Car_License_No AS CarLicenseNo, pre.slm_TelNoSms AS TelNoSms 
                                from dbo.kkslm_tr_prelead pre WITH (NOLOCK) ";

                if (flag == "campaign")
                {
                    sql += " inner join dbo.kkslm_ms_config_product_substatus cs WITH (NOLOCK) on cs.slm_CampaignId = pre.slm_CampaignId and pre.slm_Status = cs.slm_OptionCode  and pre.slm_SubStatus = cs.slm_SubStatusCode ";
                }
                else
                {
                    sql += " inner join dbo.kkslm_ms_config_product_substatus cs WITH (NOLOCK) on cs.slm_Product_Id = pre.slm_Product_Id and pre.slm_Status = cs.slm_OptionCode  and pre.slm_SubStatus = cs.slm_SubStatusCode ";
                }

                sql += @"   inner join dbo.kkslm_ms_option mo WITH (NOLOCK) on mo.slm_OptionCode = pre.slm_Status and mo.slm_OptionType = 'lead status'
                            inner join dbo.kkslm_ms_campaign cam WITH (NOLOCK) on cam.slm_CampaignId = pre.slm_CampaignId 
                            left join dbo.kkslm_ms_staff st WITH (NOLOCK) on pre.slm_Owner = st.slm_EmpCode 
                            left join dbo.kkslm_ms_teamtelesales tt WITH (NOLOCK) on st.slm_TeamTelesales_Id = tt.slm_TeamTelesales_Id 
                            left join dbo.kkslm_ms_title title1 WITH (NOLOCK) on title1.slm_TitleId = pre.slm_TitleId AND title1.is_Deleted = 0
                            left join dbo.CMT_MAPPING_PRODUCT mapproduct WITH (NOLOCK) on mapproduct.sub_product_id = pre.slm_Product_Id
                            left join dbo.CMT_MS_PRODUCT_GROUP pg WITH (NOLOCK) ON mapproduct.product_id = pg.product_id
                            left join
                            (
	                            select top 1 pa.slm_Prelead_Id,pa.slm_Mobile_Phone,pa.slm_Customer_Key,pa.slm_Address_Type 
	                            from dbo.kkslm_tr_prelead_address pa WITH (NOLOCK)
	                            where slm_Address_Type = 'C' and slm_Prelead_Id = '" + preleadId + @"') as contact on contact.slm_Prelead_Id = pre.slm_Prelead_Id 
                            left join 
                            (	
	                            select top 1 slm_CreatedDate,slm_Prelead_Id 
	                            from dbo.kkslm_tr_prelead_phone_call WITH (NOLOCK) 
	                            where slm_Prelead_Id = '" + preleadId + @"' 
	                            order by slm_CreatedDate desc) as prephone1 on prephone1.slm_Prelead_Id = pre.slm_Prelead_Id 
                            left join dbo.kkslm_ms_branch OwnerBranch WITH (NOLOCK) on OwnerBranch.slm_BranchCode = st.slm_BranchCode 
                            left join dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) on cardtype.slm_CardTypeId = pre.slm_CardTypeId
                            where pre.slm_AssignFlag = '1' and pre.slm_Assign_Status = '1' and pre.is_Deleted = 0 
                            and pre.slm_Prelead_Id = '" + preleadId + @"' {0} ";

                string whr = "";
                whr += (licenseNo == "" ? "" : (whr == "" ? "" : " AND ") + " pre.slm_Car_License_No = '" + licenseNo + "' ");
                whr += (campaignId == "" ? "" : (whr == "" ? "" : " AND ") + " pre.slm_CampaignId = '" + campaignId + "' ");
                whr += (contractNo == "" ? "" : (whr == "" ? "" : " AND ") + " pre.slm_Contract_Number = '" + contractNo + "' ");

                whr = (whr == "" ? "" : " AND ") + whr;
                sql = string.Format(sql, whr);

                PreleadViewData data = null;
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    data = slmdb.ExecuteStoreQuery<PreleadViewData>(sql).FirstOrDefault();
                }
                return data;
            }
            catch
            {
                throw;
            }
        }

//        private PreleadViewData GetPreleadData(string preleadId, string flag, string licenseNo, string campaignId, string contractNo)
//        {
//            try
//            {
//                string sql = @"select pre.slm_Prelead_Id AS PreleadId, pre.slm_Contract_Number AS ContractNo, tt.slm_TeamTelesales_Code as TeamCode , ISNULL(title1.slm_TitleName, '') + pre.slm_Name as Firstname
//                                 ,pre.slm_LastName AS Lastname ,contact.slm_Mobile_Phone AS TelNo1, pre.slm_TicketId AS TicketId, mo.slm_OptionDesc AS StatusDesc , cs.slm_SubStatusName AS SubStatusDesc
//                                , cam.slm_CampaignName as CampaignName, prephone1.slm_CreatedDate AS PreContactLatestDate
//                                , pre.slm_AssignDate AS AssignedDate,OwnerBranch.slm_BranchName AS OwnerBranchName, pre.slm_Owner AS OwnerEmpCode
//                                , st.slm_StaffNameTH as OwnerName, pre.slm_CitizenId AS CitizenId, cardtype.slm_CardTypeName AS CardTypeName, pre.slm_CardTypeId AS CardTypeId
//                                , pre.slm_CampaignId AS CampaignId, pre.slm_Product_Id AS ProductId, pre.slm_Status AS StatusCode, pre.slm_SubStatus AS SubStatusCode, pre.slm_NextContactDate AS NextContactDate
//                                , mapproduct.product_id AS ProductGroupId, mapproduct.sub_product_name AS ProductName, pg.product_name AS ProductGroupName
//                                from " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead pre ";

//                if (flag == "campaign")
//                {
//                    sql += " inner join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus cs on cs.slm_CampaignId = pre.slm_CampaignId and pre.slm_Status = cs.slm_OptionCode  and pre.slm_SubStatus = cs.slm_SubStatusCode ";
//                }
//                else
//                {
//                    sql += " inner join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_config_product_substatus cs on cs.slm_Product_Id = pre.slm_Product_Id and pre.slm_Status = cs.slm_OptionCode  and pre.slm_SubStatus = cs.slm_SubStatusCode ";
//                }

//                sql += @"   inner join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option mo on mo.slm_OptionCode = pre.slm_Status and mo.slm_OptionType = 'lead status'
//                            inner join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_campaign cam on cam.slm_CampaignId = pre.slm_CampaignId 
//                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff st on pre.slm_Owner = st.slm_EmpCode 
//                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_teamtelesales tt on st.slm_TeamTelesales_Id = tt.slm_TeamTelesales_Id 
//                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_title title1 on title1.slm_TitleId = pre.slm_TitleId AND title1.is_Deleted = 0
//                            left join " + SLMConstant.SLMDBName + @".dbo.CMT_MAPPING_PRODUCT mapproduct on mapproduct.sub_product_id = pre.slm_Product_Id
//                            left join " + SLMConstant.SLMDBName + @".dbo.CMT_MS_PRODUCT_GROUP pg ON mapproduct.product_id = pg.product_id
//                            left join
//                            (
//	                            select top 1 pa.slm_Prelead_Id,pa.slm_Mobile_Phone,pa.slm_Customer_Key,pa.slm_Address_Type 
//	                            from " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_address pa 
//	                            where slm_Address_Type = 'C' and slm_Prelead_Id = '" + preleadId + @"') as contact on contact.slm_Prelead_Id = pre.slm_Prelead_Id 
//                            left join 
//                            (	
//	                            select top 1 slm_CreatedDate,slm_Prelead_Id 
//	                            from " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_prelead_phone_call  
//	                            where slm_Prelead_Id = '" + preleadId + @"' 
//	                            order by slm_CreatedDate desc) as prephone1 on prephone1.slm_Prelead_Id = pre.slm_Prelead_Id 
//                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_branch OwnerBranch on OwnerBranch.slm_BranchCode = st.slm_BranchCode 
//                            left join " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_cardtype cardtype on cardtype.slm_CardTypeId = pre.slm_CardTypeId
//                            where pre.slm_AssignFlag = '1' and pre.slm_Assign_Status = '1' and pre.is_Deleted = 0 
//                            and pre.slm_Prelead_Id = '" + preleadId + @"' {0} ";

//                string whr = "";
//                whr += (licenseNo == "" ? "" : (whr == "" ? "" : " AND ") + " pre.slm_Car_License_No = '" + licenseNo + "' ");
//                whr += (campaignId == "" ? "" : (whr == "" ? "" : " AND ") + " pre.slm_CampaignId = '" + campaignId + "' ");
//                whr += (contractNo == "" ? "" : (whr == "" ? "" : " AND ") + " pre.slm_Contract_Number = '" + contractNo + "' ");

//                whr = (whr == "" ? "" : " AND ") + whr;
//                sql = string.Format(sql, whr);

//                PreleadViewData data = null;
//                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
//                {
//                    data = slmdb.ExecuteStoreQuery<PreleadViewData>(sql).FirstOrDefault();
//                }
//                return data;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

        public string[] GetProductCampaign(decimal preleadId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string[] arr = new string[2];
                    var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == preleadId).FirstOrDefault();
                    if (prelead != null)
                    {
                        arr[0] = prelead.slm_CampaignId;
                        arr[1] = prelead.slm_Product_Id;
                    }
                    return arr;
                }
            }
            catch
            {
                throw;
            }
        }

        //Added by Pom 22/04/2016
        public CustomerData GetCustomerData(string preleadId)
        {
            try
            {
                string sql = @"SELECT prelead.slm_Contract_Number AS ContractNo, ISNULL(title.slm_TitleName, '') + prelead.slm_Name + ' ' + prelead.slm_LastName AS CustomerName
                                , '' AS TicketId, cam.slm_CampaignName AS CampaignName, brand.slm_BrandName AS CarBrandName, model.slm_ModelName AS CarModelName
                                , prelead.slm_Car_License_No AS CarLicenseNo
                                FROM dbo.kkslm_tr_prelead prelead with (nolock) 
                                LEFT JOIN dbo.kkslm_ms_title title with (nolock) ON title.slm_TitleId = prelead.slm_TitleId
                                LEFT JOIN dbo.kkslm_ms_campaign cam with (nolock) ON cam.slm_CampaignId = prelead.slm_CampaignId
                                LEFT JOIN dbo.kkslm_ms_redbook_brand brand with (nolock) ON brand.slm_BrandCode = prelead.slm_Brand_Code
                                LEFT JOIN dbo.kkslm_ms_redbook_model model with (nolock) ON model.slm_BrandCode = prelead.slm_Brand_Code AND model.slm_ModelCode = prelead.slm_Model_Code
                                WHERE prelead.slm_Prelead_Id = '" + preleadId + "' ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<CustomerData>(sql).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool IsFleet(string preleadid, string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string type = "";

                    if (string.IsNullOrEmpty(ticketId) && !string.IsNullOrEmpty(preleadid))
                    {
                        decimal prelead_id = decimal.Parse(preleadid);
                        type = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == prelead_id).Select(p => p.slm_AssignType).FirstOrDefault();
                    }
                    else if (!string.IsNullOrEmpty(ticketId) && string.IsNullOrEmpty(preleadid))
                    {
                        type = slmdb.kkslm_tr_prelead.Where(p => p.slm_TicketId == ticketId).Select(p => p.slm_AssignType).FirstOrDefault();
                    }

                    return type != null ? (type.ToUpper() == "F" ? true : false) : false;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
