using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Dal;

namespace SLM.Biz
{
    public class SlmScr010Biz
    {
        public static List<ControlListData> GetBranchData()
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetBranchData();
        }

        public static List<ControlListData> GetOccupationData(bool useWebservice)
        {
            KKSlmMsOccupationModel occupation = new KKSlmMsOccupationModel();
            return occupation.GetOccupationData(useWebservice);
        }

        public static List<ControlListData> GetTambolData(string provinceCode, string amphurCode, bool useWebservice)
        {
            KKSlmMsTambolModel tambol = new KKSlmMsTambolModel();
            return tambol.GetTambolData(provinceCode, amphurCode, useWebservice);
        }

        public static List<ControlListData> GetAmphurData(string provinceCode)
        {
            KKSlmMsAmphurModel amphur = new KKSlmMsAmphurModel();

            return amphur.GetAmphurData(provinceCode);

        }

        public static List<ControlListData> GetProvinceData()
        {
            KKSlmMsProvinceModel province = new KKSlmMsProvinceModel();
            return province.GetProvinceData();
        }

        public static List<ControlListData> GetProvinceDataNew()
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_province.OrderBy(p => p.slm_ProvinceNameTH).ToList().Select(p => new ControlListData { TextField = p.slm_ProvinceNameTH, ValueField = p.slm_ProvinceId.ToString() }).ToList();
            }
        }

        public static List<ControlListData> GetAmphurDataNew(int provinceId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return (from am in slmdb.kkslm_ms_amphur
                        join pv in slmdb.kkslm_ms_province on am.slm_ProvinceCode equals pv.slm_ProvinceCode
                        where pv.slm_ProvinceId == provinceId
                        select am
                 ).OrderBy(a => a.slm_AmphurNameTH).ToList().Select(p => new ControlListData() { TextField = p.slm_AmphurNameTH, ValueField = p.slm_AmphurId.ToString() }).ToList();
            }
        }

        public static List<ControlListData> GetTambolDataNew(int amphurId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return (from tm in slmdb.kkslm_ms_tambol
                        join am in slmdb.kkslm_ms_amphur on new { tm.slm_ProvinceCode, tm.slm_AmphurCode } equals new { am.slm_ProvinceCode, am.slm_AmphurCode }
                        where am.slm_AmphurId == amphurId
                        select tm
                        ).OrderBy(t => t.slm_TambolNameTH).ToList().Select(p => new ControlListData() { TextField = p.slm_TambolNameTH, ValueField = p.slm_TambolId.ToString() }).ToList();
            }
        }


        public static List<ControlListData> GetBrandData()
        {
            KKSlmMsBrandModel brand = new KKSlmMsBrandModel();
            return brand.GetBrandData();
        }

        public static List<ControlListData> GetBrandDataNew()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_redbook_brand.OrderBy(p => p.slm_BrandName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_BrandName, ValueField = p.slm_BrandCode }).ToList();
            }
        }

        public static List<ControlListData> GetModelDataNew(string brandcode)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_redbook_model.Where(b => b.slm_BrandCode == brandcode).OrderBy(p => p.slm_ModelName).Select(m => new { m.slm_ModelName, m.slm_ModelCode }).Distinct().AsEnumerable().Select(p => new ControlListData { TextField = p.slm_ModelName, ValueField = p.slm_ModelCode }).ToList();
            }
        }

        public static List<ControlListData> GetInsuranceCarTypeDataNew()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_insurancecartype.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_InsurancecarTypeId).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_InsurancecarTypeName, ValueField = p.slm_InsurancecarTypeId.ToString() }).ToList();
            }
        }

        public static List<ControlListData> GetInsuranceCarTypeDataByModel(string brand, string model)
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return (from ct in sdc.kkslm_ms_insurancecartype
                        join map in sdc.kkslm_ms_mapping_brand_model_inscartype on ct.slm_InsurancecarTypeId equals map.slm_InsuranceCarTypeId
                        where map.slm_BrandCode == brand && map.slm_ModelCode == model && ct.is_Deleted == false && map.is_Deleted == false
                        orderby ct.slm_InsurancecarTypeName
                        select ct).ToList()
                        .Select(ct => new ControlListData()
                        {
                            TextField = ct.slm_InsurancecarTypeName,
                            ValueField = ct.slm_InsurancecarTypeId.ToString()
                        }).ToList();
            }
        }

        public static List<ControlListData> GetModelYearNew(string brandCode, string modelCode)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_redbook_model_year.Where(y => y.slm_ModelCode == modelCode && y.slm_BrandCode == brandCode).OrderBy(p => p.slm_YearGroup).Select(y => y.slm_YearGroup).Distinct().AsEnumerable().Select(p => new ControlListData { TextField = p.ToString(), ValueField = p.ToString() }).ToList();
            }
        }

        public static List<ControlListData> GetSubModelDataNew(string brandCode, string modelCode, int yearGroup)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_redbook_submodel.Where(m => m.slm_BrandCode == brandCode && m.slm_ModelCode == modelCode && m.slm_YearGroup == yearGroup).OrderBy(p => p.slm_Description).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_Description, ValueField = p.slm_KKKey }).ToList();
            }
        }
        public static string GetCCFromSubModel(string kkkey)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var kkk = sdc.kkslm_ms_redbook_submodel.Where(s => s.slm_KKKey == kkkey).FirstOrDefault();
                if (kkk != null) return kkk.slm_EngineDescription;
                else return "";
            }
        }

        public static List<ControlListData> GetModelData(string brandcode)
        {
            KKSlmMsModelModel model = new KKSlmMsModelModel();
            return model.GetModelData(brandcode);
        }

        public static List<ControlListData> GetPaymentTypeData(bool useWebservice)
        {
            KKSlmMsPaymentTypeModel payment = new KKSlmMsPaymentTypeModel();
            return payment.GetPaymentTypeData(useWebservice);
        }
        public static List<ControlListData> GetAccTypeData(bool useWebservice)
        {
            KKSlmMsModuleModel module = new KKSlmMsModuleModel();
            return module.GetModuleData(useWebservice);
        }
        public static List<ControlListData> GetAccPromotionData(bool useWebservice)
        {
            KKSlmMsPromotionModel promotion = new KKSlmMsPromotionModel();
            return promotion.GetPromotionData(useWebservice);
        }
        public static List<ControlListData> GetIssueBranchData()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_branch.OrderBy(p => p.slm_BranchName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_BranchName, ValueField = p.slm_BranchCode }).ToList();
            }
        }

        public static List<ControlListData> GetIssueActiveBranchData()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_branch.Where(i => i.is_Deleted == false).OrderBy(p => p.slm_BranchName).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_BranchName, ValueField = p.slm_BranchCode }).ToList();
            }
        }
        //add by nung 20170505
        public static string GetBranchName(string branchCode)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var name = sdc.kkslm_ms_branch.Where(p => p.slm_BranchCode == branchCode).Select(p => p.slm_BranchName).FirstOrDefault();
                return name != null ? name : "";
            }
        }

        public static LeadData GetLeadData(string ticketid)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchSCR004Data(ticketid);
        }

        public static string GetBranchData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetBrachCode(username);
        }

        public static List<ControlListData> GetSubModelData(string BrandCode, string Family, bool useWebservice)
        {
            KKSlmMsSubmodelModel submodel = new KKSlmMsSubmodelModel();
            return submodel.GetSubModelData(BrandCode, Family, useWebservice);
        }
        public static List<ControlListData> GetPlanBancData()
        {
            KKSlmMsPlanBancModel plan = new KKSlmMsPlanBancModel();
            return plan.GetPlanBancData();
        }
        public static string GetStaffIdData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffIdData(username);
        }
        public static string GetOwner(string ticketId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetOwner(ticketId);
        }

        public static string GetStaffNameData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffNameData(username);
        }

        public static List<CampaignWSData> GetCampaignData()
        {
            KKSlmMsCampaignModel campaign = new KKSlmMsCampaignModel();
            return campaign.GetCampaignPopupData();
        }
        public static StaffData GetStaffData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetStaffData(username);
        }

        public static string GetCampaignDetail(string campaignId)
        {
            KKSlmMsCampaignModel cam = new KKSlmMsCampaignModel();
            CampaignDetail cDetail = cam.GetCampaignDetail(campaignId);
            if (cDetail != null)
            {
                if (string.IsNullOrEmpty(cDetail.Offer) == true && string.IsNullOrEmpty(cDetail.Criteria) == true)
                    return "";
                else if (string.IsNullOrEmpty(cDetail.Offer) == false && string.IsNullOrEmpty(cDetail.Criteria) == true)
                    return cDetail.Offer;
                else if (string.IsNullOrEmpty(cDetail.Offer) == true && string.IsNullOrEmpty(cDetail.Criteria) == false)
                    return cDetail.Criteria;
                else
                    return cDetail.Offer + ": " + cDetail.Criteria;
            }
            else
                return "";
        }

        [Obsolete("seems to be unused", true)]
        public static List<ControlListData> GetOwnerListByCampaignIdAndBranch(string campaignId, string branchcode)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetOwnerListByCampaignIdAndBranch(campaignId, branchcode);
        }

        public static bool PassPrivilegeCampaign(int flag, string campaignId, string username)
        {
            //ใช้หลายหน้าจอ
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.PassPrivilegeCampaign(flag, campaignId, username);
        }

        public static string GetAssignFlag(string ticketId)
        {
            KKSlmTrLeadModel lead = new KKSlmTrLeadModel();
            return lead.GetAssignFlag(ticketId);
        }

        public static string GetChannelDefault(string username)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetChannelStaffData(username);
        }

        public static List<BranchData> GetBranchAccessRight(string campaignID)
        {
            return new List<BranchData>();
        }

        public static List<ControlListData> GetBranchListByAccessRight(int flag, string campaignId)
        {
            KKSlmMsBranchModel branch = new KKSlmMsBranchModel();
            return branch.GetBranchAccessRightList(flag, campaignId);
        }

        public static bool CheckStaffAccessRightExist(string campaignId, string branch, string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.CheckStaffAccessRightExist(campaignId, branch, username);
        }

        //public static bool CheckBranchIsEdit(string branch)
        //{
        //    KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
        //    return staff.CheckStaffAccessRightExist(campaignId, branch, username);
        //}

        public string GetControlname(string campaignId, string actionType)
        {
            string ret = "";
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                var cfg = sdc.kkslm_ms_config_product_screen.Where(p => p.slm_ActionType == actionType && p.slm_CampaignId == campaignId).FirstOrDefault();
                if (cfg == null)
                {
                    var pd = sdc.ExecuteStoreQuery<string>("select pr_productid from cmt_campaign_product where pr_campaignid = '" + campaignId + "'").FirstOrDefault();
                    if (pd != null)
                    {
                        cfg = sdc.kkslm_ms_config_product_screen.Where(p => p.slm_ActionType == actionType && p.slm_Product_Id == pd).FirstOrDefault();
                    }
                }

                if (cfg != null)
                    ret = cfg.slm_ScreenCode;

                return ret;
            }
        }


    }
}
