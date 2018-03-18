using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal.Models;
using SLM.Resource.Data;
using SLM.Dal;
using SLM.Resource;

namespace SLM.Biz
{
    public class SlmScr004Biz
    {
        public static LeadData SearchSCR004Data(string ticketid)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchSCR004Data(ticketid);
        }

        public static LeadDataForUpdate GetSCR004Data(string ticketId)
        {
            return new SearchLeadModel().GetSCR004Data(ticketId);
        }

        public static List<object> GetTabAndSidebarCfg(string productId, string campaignId)
        {
            var search = new SearchLeadModel();
            return search.GetTabAndSideBarConfig(productId, campaignId);
        }

        public static List<CampaignWSData> GetCampaignFinalData(string ticketid)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.GetCampaignFinalList(ticketid);
        }

        public static void InsertCampaginFinalList(List<CampaignWSData> ListCampaign, string username)
        {
            KKSLMTrCampaignFinalModel final = new KKSLMTrCampaignFinalModel();
            final.InsertCampaignList(ListCampaign, username);
        }

        public static List<ProductData> GetBundleProduct(string campaignId)
        {
            KKSlmProductBundleConfigModel bundle = new KKSlmProductBundleConfigModel();
            return bundle.GetBundleProduct(campaignId);
        }

        public static List<ProductData> GetBundleProductNew(string campaignId)
        {
            KKSlmProductBundleConfigModel bundle = new KKSlmProductBundleConfigModel();
            return bundle.GetBundleProductNew(campaignId);
        }


        public static List<ProductData> SearchCampaignViewPage(string productGroupId, string productId, string campaignId, string bundleCamIdList)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.SearchCampaignViewPage(productGroupId, productId, campaignId, bundleCamIdList);
        }

        public static List<ProductData> SearchCampaignViewPageNew(string productGroupId, string productId, string campaignId, string bundleCamIdList)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.SearchCampaignViewPageNew(productGroupId, productId, campaignId, bundleCamIdList);
        }

        public static List<ProductData> SearchCampaign(string searchWord, string bundleCamIdList)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.SearchCampaign(searchWord, bundleCamIdList);
        }

        public static List<ProductData> SearchCampaignNew(string searchWord, string bundleCamIdList)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.SearchCampaignNew(searchWord, bundleCamIdList);
        }

        public static List<ProductData> GetProductCampaignData(string campaignId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetProductCampaignData(campaignId);
        }

        public static void InsertCampaginFinalList(List<ProductData> productList, string ticketId, string username)
        {
            KKSLMTrCampaignFinalModel final = new KKSLMTrCampaignFinalModel();
            final.InsertCampaignList(productList, ticketId, username);
        }

        public static List<StaffData> GetChannelStaffData(string username)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetChannelStaffData(username);
        }

        public static List<ProductData> GetProductCampaignDataForCmt(string campaignId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetProductCampaignDataForCmt(campaignId);
        }

        public static List<ControlListData> GetCampaignDataViewPage(string productGroupId, string productId, string CmtCampaignIdList)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetCampaignDataViewPage(productGroupId, productId, CmtCampaignIdList);
        }

        public static List<ControlListData> GetCampaignDataViewPageNew(string productGroupId, string productId, string CmtCampaignIdList)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetCampaignDataViewPageNew(productGroupId, productId, CmtCampaignIdList);
        }

        public static SearchInsureSummaryResult SearchPreleadInsureSummaryData(string preleadId)
        {
            SearchObtModel search = new SearchObtModel();
            return search.SearchInsureSummary(preleadId);
        }

        public static SearchInsureSummaryResult SearchLeadInsureSummaryData(string ticketId)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.SearchInsureSummary(ticketId);
        }

        public LeadDefaultData GetLeadDataForInitialPhoneCall(string ticketId)
        {
            return new KKSlmTrLeadModel().GetLeadDataForInitialPhoneCall(ticketId);
        }

        public string GetStatus(string ticketId)
        {
            return new KKSlmTrLeadModel().GetStatus(ticketId);
        }

        public void CheckRenewPurchased(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = "";
                    int countPolicy = 0;

                    sql = @"SELECT COUNT(compare.slm_RenewInsureCompareId)
                        FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance renew
                        INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare compare ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
                        WHERE renew.slm_TicketId = '" + ticketId + "' AND compare.slm_Selected = 1 ";

                    countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                    if (countPolicy == 0)
                        throw new Exception("ไม่พบข้อมูลการซื้อประกัน");
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ReceiptInfoData> GetReceiptList(string ticketId)
        {
            try
            {
                //                string sql = @"SELECT receipt.slm_ticketId AS TicketId, receipt.slm_RecNo AS RecNo, detail.RecAmount
                //                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt receipt
                //                                INNER JOIN (SELECT SUM(slm_RecAmount) RecAmount, slm_RenewInsuranceReceiptId 
                //			                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail 
                //			                                GROUP BY slm_RenewInsuranceReceiptId) detail ON detail.slm_RenewInsuranceReceiptId = receipt.slm_RenewInsuranceReceiptId
                //                                WHERE receipt.slm_ticketId = '" + ticketId + @"'
                //                                ORDER BY receipt.slm_CreatedDate DESC ";

                string sql = @"SELECT receipt.slm_ticketId AS TicketId, receipt.slm_RecNo AS RecNo, detail.slm_TransDate AS TransDate, SUM(detail.slm_RecAmount) AS RecAmount
                                FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt receipt
                                INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_receipt_detail detail ON detail.slm_RenewInsuranceReceiptId = receipt.slm_RenewInsuranceReceiptId
                                WHERE receipt.slm_ticketId = '" + ticketId + @"'
                                GROUP BY receipt.slm_ticketId, receipt.slm_RecNo, detail.slm_TransDate
                                ORDER BY detail.slm_TransDate DESC ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.ExecuteStoreQuery<ReceiptInfoData>(sql).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckRenewOrActPurchased(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = "";
                    int countPolicy = 0;
                    int countAct = 0;

                    sql = @"SELECT COUNT(compare.slm_RenewInsureCompareId)
                        FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance renew
                        INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare compare ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
                        WHERE renew.slm_TicketId = '" + ticketId + "' AND compare.slm_Selected = 1 ";

                    countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                    sql = @"SELECT COUNT(compare.slm_RenewInsureCompareActId)
                        FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance renew
                        INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare_act compare ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
                        WHERE renew.slm_TicketId = '" + ticketId + "' AND compare.slm_ActPurchaseFlag = 1 ";

                    countAct = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                    return (countPolicy == 0 && countAct == 0) ? false : true;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckMotorRenewPurchased(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = "";
                    int countPolicy = 0;

                    sql = @"SELECT COUNT(compare.slm_RenewInsureCompareId)
                        FROM dbo.kkslm_tr_renewinsurance renew WITH (NOLOCK) 
                        INNER JOIN dbo.kkslm_tr_renewinsurance_compare compare WITH (NOLOCK) ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
                        WHERE renew.slm_TicketId = '" + ticketId + "' AND compare.slm_Selected = 1 ";

                    countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                    return countPolicy == 0 ? false : true;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckActPurchased(string ticketId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = "";
                    int countAct = 0;

                    sql = @"SELECT COUNT(compare.slm_RenewInsureCompareActId)
                        FROM dbo.kkslm_tr_renewinsurance renew WITH (NOLOCK) 
                        INNER JOIN dbo.kkslm_tr_renewinsurance_compare_act compare WITH (NOLOCK) ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
                        WHERE renew.slm_TicketId = '" + ticketId + "' AND compare.slm_ActPurchaseFlag = 1 ";

                    countAct = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
                    return countAct == 0 ? false : true;
                }
            }
            catch
            {
                throw;
            }
        }

        public static DateTime? GetActSendDate(string ticketId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var date = slmdb.kkslm_tr_renewinsurance.Where(p => p.slm_TicketId == ticketId).Select(p => p.slm_ActSendDate).FirstOrDefault();
                return date;
            }
        }

        public void GetDiscountPayment(string ticketId, out decimal policyGrossPremiumTotal, out decimal policyGrossPremium, out decimal policyDiscountPercent, out decimal policyDiscountAmount
            , out decimal actGrossPremiumTotal, out decimal actGrossPremium, out decimal actDiscountPercent, out decimal actDiscountAmount)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var renew = slmdb.kkslm_tr_renewinsurance.Where(p => p.slm_TicketId == ticketId).FirstOrDefault();
                    if (renew != null)
                    {
                        //Policy
                        var vat1PercentBath = renew.slm_Vat1PercentBath != null ? renew.slm_Vat1PercentBath.Value : 0;
                        var grossPremiumTotal_policy = renew.slm_PolicyGrossPremiumTotal != null ? renew.slm_PolicyGrossPremiumTotal.Value : 0;
                        policyGrossPremiumTotal = grossPremiumTotal_policy - vat1PercentBath;
                        policyGrossPremium = renew.slm_PolicyGrossPremium != null ? renew.slm_PolicyGrossPremium.Value : 0;
                        policyDiscountPercent = renew.slm_DiscountPercent != null ? renew.slm_DiscountPercent.Value : 0;
                        policyDiscountAmount = renew.slm_PolicyDiscountAmt != null ? renew.slm_PolicyDiscountAmt.Value : 0;

                        //Act
                        var Vat1PercentBathAct = renew.slm_ActVat1PercentBath == null ? 0 : renew.slm_ActVat1PercentBath.Value;
                        var actVat = renew.slm_ActVat == null ? 0 : renew.slm_ActVat.Value;
                        var actTax = renew.slm_ActStamp == null ? 0 : renew.slm_ActStamp.Value;
                        var actNetPremium = renew.slm_ActNetPremium == null ? 0 : renew.slm_ActNetPremium.Value;

                        actGrossPremiumTotal = (actNetPremium + actVat + actTax - Vat1PercentBathAct);
                        actGrossPremium = renew.slm_ActGrossPremium != null ? renew.slm_ActGrossPremium.Value : 0;
                        actDiscountPercent = renew.slm_ActDiscountPercent != null ? renew.slm_ActDiscountPercent.Value : 0;
                        actDiscountAmount = renew.slm_ActDiscountAmt != null ? renew.slm_ActDiscountAmt.Value : 0;
                    }
                    else
                    {
                        policyGrossPremiumTotal = 0;
                        policyGrossPremium = 0;
                        policyDiscountPercent = 0;
                        policyDiscountAmount = 0;
                        actGrossPremiumTotal = 0;
                        actGrossPremium = 0;
                        actDiscountPercent = 0;
                        actDiscountAmount = 0;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public List<decimal?> GetPaymentMain(decimal renewId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.kkslm_tr_renewinsurance_paymentmain.Where(p => p.slm_RenewInsureId == renewId && p.slm_Type == "1").OrderBy(p => p.slm_Seq).Select(p => p.slm_PaymentAmount).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<PaymentData> GetPaymentMainList(decimal renewId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    return slmdb.kkslm_tr_renewinsurance_paymentmain.Where(p => p.slm_RenewInsureId == renewId && p.slm_Type == "1").OrderBy(p => p.slm_Seq)
                            .Select(p => new PaymentData {
                                slm_PaymentDate = p.slm_PaymentDate,
                                slm_PaymentAmount = p.slm_PaymentAmount
                            }).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        //รายงานแจ้งต่อ พรบ
        public static void InsertActIssueReport(string ticketId, DateTime createdDate, string userName)
        {
            try
            {
                string sql = @"SELECT REN.slm_ActComId  AS InsComId, comact.slm_ActSignNo AS ActNo,
                                pre.slm_BranchCode as BranchCode,ren.slm_ContractNo as ContractNo,  
                                LEAD.slm_TitleId AS TitleId, LEAD.slm_Name AS Firstname, LEAD.slm_LastName AS Lastname, ISNULL(REN.slm_LicenseNo, '') + ' ' + ISNULL(proregis.slm_ProvinceNameTH, '') AS CarLicenseNo,
                                REN.slm_RedbookBrandCode AS RedbookBrandCode, REN.slm_ReceiveNo AS ReceiveNo, ST.slm_EmpCode AS OwnerEmpCode,
                                REN.slm_ChassisNo AS EngineNo, REN.slm_ActNetPremium as NetPremium,
                                ISNULL(REN.slm_ActNetPremium, 0) + ISNULL(REN.slm_ActVat, 0) + ISNULL(REN.slm_ActStamp, 0) AS NetPremiumIncludeVat ,
                                REN.slm_ActStartCoverDate  AS StartDateAct, REN.slm_ActEndCoverDate AS EndDateAct,
                                RA.slm_House_No AS HouseNo, RA.slm_Moo AS Moo, RA.slm_Village AS Village, RA.slm_BuildingName AS BuildingName, RA.slm_Soi AS Soi,
                                RA.slm_Street AS Street,RA.slm_Tambon AS TambolId, RA.slm_Amphur AS AmphurId, RA.slm_Province AS ProvinceId, 
                                RA.slm_PostalCode AS Zipcode, REN.slm_RemarkAct AS Remark, LEAD.slm_TelNo_1 AS TelNoContact,
                                CUS.slm_CitizenId AS CitizenId , pre.slm_Guarantor_TitleId1 as TitleId_Committee1,
                                PRE.slm_Guarantor_First_Name1 AS Name_Committee1, PRE.slm_Guarantor_Last_Name1 AS LastName_Committee1,
                                PRE.slm_Guarantor_Card_Id1 AS CitizenId_Committe1,PRE.slm_Guarantor_TitleId2 AS TitleId_Committee2,
                                PRE.slm_Guarantor_First_Name2 AS Name_Committee2, PRE.slm_Guarantor_Last_Name2 AS LastName_Committee2,
                                PRE.slm_Guarantor_Card_Id2 AS CitizenId_Committe2, PRE.slm_Guarantor_TitleId3 AS TitleId_Committee3,
                                PRE.slm_Guarantor_First_Name3 AS Name_Committee3, PRE.slm_Guarantor_Last_Name3 AS LastName_Committee3,
                                PRE.slm_Guarantor_Card_Id3 AS CitizenId_Committe3
                                FROM kkslm_tr_lead LEAD 
                                INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId = REN.slm_TicketId 
                                INNER JOIN kkslm_tr_cusinfo CUS ON CUS.slm_TicketId = LEAD.slm_ticketId
                                LEFT JOIN kkslm_tr_renewinsurance_compare_act comact ON comact.slm_RenewInsureId = REN.slm_RenewInsureId AND comact.slm_ActPurchaseFlag = 1
                                LEFT JOIN kkslm_tr_renewinsurance_address RA ON RA.slm_RenewInsureId = REN.slm_RenewInsureId AND slm_AddressType = 'D' 
                                LEFT JOIN kkslm_ms_staff ST ON ST.slm_UserName = LEAD.slm_Owner 
                                LEFT JOIN kkslm_tr_prelead pre on pre.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN kkslm_tr_productinfo prod ON prod.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN kkslm_ms_province proregis ON prod.slm_ProvinceRegis = proregis.slm_ProvinceId
                                WHERE lead.slm_ticketId = '" + ticketId + "' ";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var data = slmdb.ExecuteStoreQuery<ActIssueReportData>(sql).FirstOrDefault();
                    if (data != null)
                    {
                        kkslm_tr_notify_act_report report = new kkslm_tr_notify_act_report()
                        {
                            slm_TicketId = ticketId,
                            slm_ActNo = data.ActNo,
                            slm_BranchCode = data.BranchCode,
                            slm_BrandCode = data.RedbookBrandCode,
                            slm_Building = data.BuildingName,
                            slm_CarLicenseNo = data.CarLicenseNo,
                            slm_CitizenId = data.CitizenId,
                            slm_CitizenId_Committe1 = data.CitizenId_Committe1,
                            slm_CitizenId_Committe2 = data.CitizenId_Committe2,
                            slm_CitizenId_Committe3 = data.CitizenId_Committe3,
                            slm_TitleId_Committee1 = data.TitleId_Committee1,
                            slm_TitleId_Committee2 = data.TitleId_Committee2,
                            slm_TitleId_Committee3 = data.TitleId_Committee3,
                            slm_Name_Committee1 = data.Name_Committee1,
                            slm_Name_Committee2 = data.Name_Committee2,
                            slm_Name_Committee3 = data.Name_Committee3,
                            slm_LastName_Committee1 = data.LastName_Committee1,
                            slm_LastName_Committee2 = data.LastName_Committee2,
                            slm_LastName_Committee3 = data.LastName_Committee3,
                            slm_Contract_Number = data.ContractNo,
                            slm_StartDateAct = data.StartDateAct,
                            slm_EndDateAct = data.EndDateAct,
                            slm_Ins_Com_Id = data.InsComId,
                            slm_InsReceiveNo = data.ReceiveNo,
                            slm_TitleId = data.TitleId,
                            slm_Name = data.Firstname,
                            slm_LastName = data.Lastname,
                            slm_House_No = data.HouseNo,
                            slm_Soi = data.Soi,
                            slm_Village = data.Village,
                            slm_Moo = data.Moo,
                            slm_Street = data.Street,
                            slm_TambolId = data.TambolId,
                            slm_AmphurId = data.AmphurId,
                            slm_ProvinceId = data.ProvinceId,
                            slm_Zipcode = data.Zipcode,
                            slm_EngineNo = data.EngineNo,   //ใส่ค่า ChassisNo
                            slm_NetPremium = data.NetPremium,
                            slm_NetPremiumIncludeVat = data.NetPremiumIncludeVat,
                            slm_Owner = data.OwnerEmpCode,
                            slm_Remark = data.Remark,
                            slm_TelNoContact = data.TelNoContact,
                            slm_CreatedBy = userName,
                            slm_CreatedDate = createdDate,
                            slm_UpdatedBy = userName,
                            slm_UpdatedDate = createdDate,
                            is_Deleted = false
                        };
                        slmdb.kkslm_tr_notify_act_report.AddObject(report);
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static string GetNotifyPremiumRemark(string chassisNo)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var remark = slmdb.kkslm_tr_notify_premium.Where(p => p.slm_VIN == chassisNo).OrderByDescending(p => p.slm_CreatedDate).Select(p => p.slm_Remark).FirstOrDefault();
                    return remark != null ? remark : "";
                }
            }
            catch
            {
                throw;
            }
        }

        public static LeadDataForUpdate GetDataForUpdateViewMain(string ticketId)
        {
            string sql = @"SELECT lead.slm_ticketId AS TicketId
                            ,CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                            ELSE posowner.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS OwnerName
                            ,CASE WHEN posdelegate.slm_PositionNameAbb IS NULL THEN delegate.slm_StaffNameTH
	                            ELSE posdelegate.slm_PositionNameAbb + ' - ' + delegate.slm_StaffNameTH END AS DelegateName
                            ,opt.slm_OptionDesc AS StatusName
                            ,lead.slm_Status AS [Status],lead.slm_Owner AS [Owner] ,lead.slm_Delegate As Delegate 
                            ,OwnerBranch.slm_BranchName AS OwnerBranchName, lead.slm_Owner_Branch AS OwnerBranch
                            ,Delegatebranch.slm_BranchName AS DelegateBranchName, lead.slm_Delegate_Branch AS DelegateBranch, lead.slm_ExternalSubStatusDesc AS ExternalSubStatusDesc
                            , lead.slm_TelNo_1 AS TelNo1
                            FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_lead lead 
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff staff ON lead.slm_Owner = staff.slm_UserName  
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position posowner ON lead.slm_Owner_Position = posowner.slm_Position_id
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_option opt ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' AND opt.is_Deleted = '0' 
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_staff delegate ON lead.slm_Delegate = delegate.slm_UserName  
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_position posdelegate ON lead.slm_Delegate_Position = posdelegate.slm_Position_id 
                            LEFT JOIN " + SLMConstant.SLMDBName + @".DBO.kkslm_ms_branch Delegatebranch on Delegatebranch.slm_BranchCode = lead.slm_Delegate_Branch 
                            LEFT JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_ms_branch OwnerBranch on OwnerBranch.slm_BranchCode = lead.slm_Owner_Branch 
                            WHERE  lead.is_Deleted = 0  AND  lead.slm_ticketId = '" + ticketId + "' ";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<LeadDataForUpdate>(sql).FirstOrDefault();
            }
        }

        public static string GetExternalSubStatusDesc(string ticketId)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).Select(p => p.slm_ExternalSubStatusDesc).FirstOrDefault();
            }
        }

        public static LeadDefaultData GetLeadDefaultData(string ticketId)
        {
            try
            {
                string slmDBName = SLMConstant.SLMDBName;
                string sql = @"SELECT NULL AS PreleadId, '' AS TeamTelesalesCode, '' AS ContractNo, lead.slm_ticketId AS TicketId, lead.slm_Name AS FirstName, lead.slm_LastName AS LastName, lead.slm_TelNo_1 AS TelNo1
                                , lead.slm_Status AS StatusCode, opt.slm_OptionDesc AS StatusDesc, lead.slm_ExternalSubStatusDesc AS ExternalSubStatusDesc, lead.slm_ChannelId AS ChannelId
                                , lead.slm_CampaignId AS CampaignId, campaign.slm_CampaignName AS CampaignName, lead.slm_Product_Id AS ProductId, prod.slm_InterestedProd AS InterestedProd, info.slm_TelNo_2 AS TelNo2, info.slm_Ext_2 AS Ext2
                                , info.slm_TelNo_3 AS TelNo3, info.slm_Ext_3 AS Ext3
                                , phone1.slm_CreateDate AS ContactLatestDate, lead.slm_AssignedDate AS AssignedDate, phone2.slm_CreateDate AS ContactFirstDate, lead.coc_AssignedDate AS CocAssignedDate
                                , lead.slm_Product_Group_Id AS ProductGroupId, PG.product_name AS ProductGroupName, lead.slm_Product_Id AS ProductId, MP.sub_product_name AS ProductName
                                , lead.slm_AssignedFlag AS AssignedFlag, lead.slm_Delegate_Flag AS DelegateFlag
                                , NULL AS PreleadContactLatestDate, NULL AS PreleadAssignDate
                                , lead.slm_Owner_Branch AS OwnerBranchCode, OwnerBranch.slm_BranchName AS OwnerBranchName, lead.slm_Owner AS OwnerUsername
                                , CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                                    ELSE posowner.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS OwnerName
                                , lead.slm_Delegate_Branch AS DelegateBranchCode, Delegatebranch.slm_BranchName AS DelegateBranchName, lead.slm_Delegate AS DelegateUsername
                                , CASE WHEN posdelegate.slm_PositionNameAbb IS NULL THEN delegate.slm_StaffNameTH
	                                    ELSE posdelegate.slm_PositionNameAbb + ' - ' + delegate.slm_StaffNameTH END AS DelegateName
                                , CASE WHEN ISNULL(mktowner.slm_StaffNameTH, lead.coc_MarketingOwner) = lead.coc_MarketingOwner THEN lead.coc_MarketingOwner
                                        WHEN posmktowner.slm_PositionNameAbb IS NULL THEN mktowner.slm_StaffNameTH
                                        ELSE posmktowner.slm_PositionNameAbb + ' - ' + mktowner.slm_StaffNameTH END AS MarketingOwnerName
                                , CASE WHEN ISNULL(lastowner.slm_StaffNameTH, lead.coc_LastOwner) = lead.coc_LastOwner THEN lead.coc_LastOwner
	                                    WHEN poslastowner.slm_PositionNameAbb IS NULL THEN lastowner.slm_StaffNameTH
	                                    ELSE poslastowner.slm_PositionNameAbb + ' - ' + lastowner.slm_StaffNameTH END AS LastOwnerName
                                , cocstatus.slm_OptionDesc AS CocStatusDesc, lead.coc_CurrentTeam AS COCCurrentTeam 
                                , info.slm_Detail AS Detail, info.slm_CardType AS CardType, info.slm_CitizenId AS CitizenId, ISNULL(MP.HasADAMUrl, 0) AS HasAdamsUrl, campaign.slm_url AS CalculatorUrl
                                , lead.coc_Appno AS AppNo, CONVERT(VARCHAR, lead.coc_IsCOC) AS ISCOC, cardtype.slm_CARSubscriptionTypeId AS SubscriptionTypeId, info.slm_country_id CountryId
                                , lead.slm_TicketIdRefer AS TicketIDRefer
                                FROM " + slmDBName + @".dbo.kkslm_tr_lead lead WITH (NOLOCK)
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_option opt WITH (NOLOCK) ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' AND opt.is_Deleted = '0' 
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_campaign campaign WITH (NOLOCK) ON lead.slm_CampaignId = campaign.slm_CampaignId
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_tr_cusinfo info WITH (NOLOCK) ON lead.slm_ticketId = info.slm_TicketId
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) ON cardtype.slm_CardTypeId = info.slm_CardType AND cardtype.is_Deleted = 0
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_tr_productinfo prod WITH (NOLOCK) ON prod.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_branch OwnerBranch WITH (NOLOCK) on OwnerBranch.slm_BranchCode = lead.slm_Owner_Branch 
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff staff WITH (NOLOCK) ON lead.slm_Owner = staff.slm_UserName --and staff.is_Deleted = 0
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position posowner WITH (NOLOCK) ON lead.slm_Owner_Position = posowner.slm_Position_id
                                LEFT JOIN " + slmDBName + @".DBO.kkslm_ms_branch Delegatebranch WITH (NOLOCK) on Delegatebranch.slm_BranchCode = lead.slm_Delegate_Branch 
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff delegate WITH (NOLOCK) ON lead.slm_Delegate = delegate.slm_UserName  
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position posdelegate WITH (NOLOCK) ON lead.slm_Delegate_Position = posdelegate.slm_Position_id 
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff mktowner WITH (NOLOCK) ON lead.coc_MarketingOwner = mktowner.slm_EmpCode
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position posmktowner WITH (NOLOCK) ON lead.coc_MarketingOwner_Position = posmktowner.slm_Position_id
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff lastowner WITH (NOLOCK) ON lead.coc_LastOwner = lastowner.slm_EmpCode
                                LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position poslastowner WITH (NOLOCK) ON lead.coc_LastOwner_Position = poslastowner.slm_Position_id 
                                LEFT JOIN " + slmDBName + @".dbo.CMT_MAPPING_PRODUCT MP WITH (NOLOCK) ON MP.sub_product_id = lead.slm_Product_Id 
                                LEFT JOIN " + slmDBName + @".dbo.CMT_MS_PRODUCT_GROUP PG WITH (NOLOCK) ON PG.product_id = lead.slm_Product_Group_Id
                                LEFT JOIN (SELECT DISTINCT slm_OptionCode, slm_OptionSubCode, slm_OptionType, slm_OptionDesc
			                                FROM " + slmDBName + @".dbo.kkslm_ms_option WITH (NOLOCK) ) cocstatus ON lead.coc_Status = cocstatus.slm_OptionCode AND ISNULL(lead.coc_SubStatus, '0123456789') = ISNULL(cocstatus.slm_OptionSubCode, '0123456789') AND cocstatus.slm_OptionType = 'coc_status' 
                                LEFT JOIN (SELECT TOP 1 slm_CreateDate,slm_TicketId FROM " + slmDBName + @".DBO.kkslm_phone_call WITH (NOLOCK) WHERE slm_TicketId = '" + ticketId + @"' ORDER BY slm_CreateDate DESC) AS phone1 
			                                ON phone1.slm_TicketId = LEAD.slm_ticketId 
                                LEFT JOIN (SELECT TOP 1 slm_CreateDate,slm_TicketId FROM " + slmDBName + @".DBO.kkslm_phone_call WITH (NOLOCK) WHERE slm_TicketId = '" + ticketId + @"' ORDER BY slm_CreateDate ASC) AS phone2
			                                ON phone2.slm_TicketId = LEAD.slm_ticketId 
                                WHERE lead.slm_ticketId = '" + ticketId + "' ";

                LeadDefaultData data = null;
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    data = slmdb.ExecuteStoreQuery<LeadDefaultData>(sql).FirstOrDefault();
                }
                return data;
            }
            catch
            {
                throw;
            }
        }

        public static LeadDefaultData GetLeadRenewInsureData(string ticketId)
        {
            try
            {
                string slmDBName = SLMConstant.SLMDBName;
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    decimal preleadId = slmdb.kkslm_tr_prelead.Where(p => p.slm_TicketId == ticketId).Select(p => p.slm_Prelead_Id).FirstOrDefault();
                    string sql = @"SELECT prelead.slm_Prelead_Id AS PreleadId, teamtele.slm_TeamTelesales_Code AS TeamTelesalesCode, renewInsur.slm_ContractNo AS ContractNo, lead.slm_ticketId AS TicketId, lead.slm_Name AS FirstName, lead.slm_LastName AS LastName, lead.slm_TelNo_1 AS TelNo1
                                    , lead.slm_Status AS StatusCode, opt.slm_OptionDesc AS StatusDesc, lead.slm_ExternalSubStatusDesc AS ExternalSubStatusDesc, lead.slm_ChannelId AS ChannelId
                                    , lead.slm_CampaignId AS CampaignId, campaign.slm_CampaignName AS CampaignName, lead.slm_Product_Id AS ProductId, prod.slm_InterestedProd AS InterestedProd, info.slm_TelNo_2 AS TelNo2, info.slm_Ext_2 AS Ext2
                                    , info.slm_TelNo_3 AS TelNo3, info.slm_Ext_3 AS Ext3
                                    , phone1.slm_CreateDate AS ContactLatestDate, lead.slm_AssignedDate AS AssignedDate, phone2.slm_CreateDate AS ContactFirstDate, lead.coc_AssignedDate AS CocAssignedDate
                                    , lead.slm_Product_Group_Id AS ProductGroupId, PG.product_name AS ProductGroupName, lead.slm_Product_Id AS ProductId, MP.sub_product_name AS ProductName
                                    , lead.slm_AssignedFlag AS AssignedFlag, lead.slm_Delegate_Flag AS DelegateFlag
                                    , prephone1.slm_CreatedDate AS PreleadContactLatestDate, prelead.slm_AssignDate AS PreleadAssignDate
                                    , lead.slm_Owner_Branch AS OwnerBranchCode, OwnerBranch.slm_BranchName AS OwnerBranchName, lead.slm_Owner AS OwnerUsername
                                    , CASE WHEN posowner.slm_PositionNameAbb IS NULL THEN staff.slm_StaffNameTH
	                                        ELSE posowner.slm_PositionNameAbb + ' - ' + staff.slm_StaffNameTH END AS OwnerName
                                    , lead.slm_Delegate_Branch AS DelegateBranchCode, Delegatebranch.slm_BranchName AS DelegateBranchName, lead.slm_Delegate AS DelegateUsername
                                    , CASE WHEN posdelegate.slm_PositionNameAbb IS NULL THEN delegate.slm_StaffNameTH
	                                        ELSE posdelegate.slm_PositionNameAbb + ' - ' + delegate.slm_StaffNameTH END AS DelegateName
                                    , CASE WHEN ISNULL(mktowner.slm_StaffNameTH, lead.coc_MarketingOwner) = lead.coc_MarketingOwner THEN lead.coc_MarketingOwner
                                            WHEN posmktowner.slm_PositionNameAbb IS NULL THEN mktowner.slm_StaffNameTH
                                            ELSE posmktowner.slm_PositionNameAbb + ' - ' + mktowner.slm_StaffNameTH END AS MarketingOwnerName
                                    , CASE WHEN ISNULL(lastowner.slm_StaffNameTH, lead.coc_LastOwner) = lead.coc_LastOwner THEN lead.coc_LastOwner
	                                        WHEN poslastowner.slm_PositionNameAbb IS NULL THEN lastowner.slm_StaffNameTH
	                                        ELSE poslastowner.slm_PositionNameAbb + ' - ' + lastowner.slm_StaffNameTH END AS LastOwnerName
                                    , cocstatus.slm_OptionDesc AS CocStatusDesc, lead.coc_CurrentTeam AS COCCurrentTeam 
                                    , info.slm_Detail AS Detail, info.slm_CardType AS CardType, info.slm_CitizenId AS CitizenId, ISNULL(MP.HasADAMUrl, 0) AS HasAdamsUrl, campaign.slm_url AS CalculatorUrl
                                    , lead.coc_Appno AS AppNo, CONVERT(VARCHAR, lead.coc_IsCOC) AS ISCOC, cardtype.slm_CARSubscriptionTypeId AS SubscriptionTypeId
                                    , lead.slm_TicketIdRefer AS TicketIDRefer
                                    FROM " + slmDBName + @".dbo.kkslm_tr_lead lead WITH (NOLOCK)
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_tr_prelead prelead WITH (NOLOCK) ON lead.slm_TicketId = prelead.slm_TicketId
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_tr_renewinsurance renewInsur WITH (NOLOCK) ON lead.slm_ticketId = renewInsur.slm_TicketId
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_option opt WITH (NOLOCK) ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' AND opt.is_Deleted = '0' 
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_campaign campaign WITH (NOLOCK) ON lead.slm_CampaignId = campaign.slm_CampaignId
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_tr_cusinfo info WITH (NOLOCK) ON lead.slm_ticketId = info.slm_TicketId
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_cardtype cardtype WITH (NOLOCK) ON cardtype.slm_CardTypeId = info.slm_CardType AND cardtype.is_Deleted = 0
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_tr_productinfo prod WITH (NOLOCK) ON prod.slm_TicketId = lead.slm_ticketId
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_branch OwnerBranch WITH (NOLOCK) on OwnerBranch.slm_BranchCode = lead.slm_Owner_Branch 
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff staff WITH (NOLOCK) ON lead.slm_Owner = staff.slm_UserName-- and staff.is_Deleted = 0
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position posowner WITH (NOLOCK) ON lead.slm_Owner_Position = posowner.slm_Position_id
                                    LEFT JOIN " + slmDBName + @".DBO.kkslm_ms_branch Delegatebranch WITH (NOLOCK) on Delegatebranch.slm_BranchCode = lead.slm_Delegate_Branch 
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff delegate WITH (NOLOCK) ON lead.slm_Delegate = delegate.slm_UserName  
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position posdelegate WITH (NOLOCK) ON lead.slm_Delegate_Position = posdelegate.slm_Position_id 
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff mktowner WITH (NOLOCK) ON lead.coc_MarketingOwner = mktowner.slm_EmpCode
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position posmktowner WITH (NOLOCK) ON lead.coc_MarketingOwner_Position = posmktowner.slm_Position_id
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_staff lastowner WITH (NOLOCK) ON lead.coc_LastOwner = lastowner.slm_EmpCode
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_position poslastowner WITH (NOLOCK) ON lead.coc_LastOwner_Position = poslastowner.slm_Position_id 
                                    LEFT JOIN " + slmDBName + @".dbo.CMT_MAPPING_PRODUCT MP WITH (NOLOCK) ON MP.sub_product_id = lead.slm_Product_Id 
                                    LEFT JOIN " + slmDBName + @".dbo.CMT_MS_PRODUCT_GROUP PG WITH (NOLOCK) ON PG.product_id = lead.slm_Product_Group_Id
                                    LEFT JOIN " + slmDBName + @".dbo.kkslm_ms_teamtelesales teamtele WITH (NOLOCK) on teamtele.slm_TeamTelesales_Id = staff.slm_TeamTelesales_Id 
                                    LEFT JOIN (SELECT DISTINCT slm_OptionCode, slm_OptionSubCode, slm_OptionType, slm_OptionDesc
			                                    FROM " + slmDBName + @".dbo.kkslm_ms_option WITH (NOLOCK) ) cocstatus ON lead.coc_Status = cocstatus.slm_OptionCode AND ISNULL(lead.coc_SubStatus, '0123456789') = ISNULL(cocstatus.slm_OptionSubCode, '0123456789') AND cocstatus.slm_OptionType = 'coc_status' 
                                    LEFT JOIN (SELECT TOP 1 slm_CreateDate,slm_TicketId FROM " + slmDBName + @".DBO.kkslm_phone_call WITH (NOLOCK) WHERE slm_TicketId = '" + ticketId + @"' ORDER BY slm_CreateDate DESC) AS phone1 
			                                    ON phone1.slm_TicketId = LEAD.slm_ticketId 
                                    LEFT JOIN (SELECT TOP 1 slm_CreateDate,slm_TicketId FROM " + slmDBName + @".DBO.kkslm_phone_call WITH (NOLOCK) WHERE slm_TicketId = '" + ticketId + @"' ORDER BY slm_CreateDate ASC) AS phone2
			                                    ON phone2.slm_TicketId = LEAD.slm_ticketId 
                                    LEFT JOIN 
                                            (	
	                                            SELECT TOP 1 slm_CreatedDate, slm_Prelead_Id 
	                                            FROM " + slmDBName + @".dbo.kkslm_tr_prelead_phone_call WITH (NOLOCK)  
			                                    WHERE slm_Prelead_Id = '" + preleadId.ToString() + @"' 
	                                            ORDER BY slm_CreatedDate DESC) prephone1 ON prephone1.slm_Prelead_Id = prelead.slm_Prelead_Id
                                    WHERE lead.slm_ticketId = '" + ticketId + "' ";

                    return slmdb.ExecuteStoreQuery<LeadDefaultData>(sql).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public static void GetLeadStatus(string ticketId, out string statusDesc, out string subStatusDesc)
        {
            try
            {
                statusDesc = "";
                subStatusDesc = "";

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    string sql = $@"SELECT opt.slm_OptionDesc AS StatusDesc, lead.slm_ExternalSubStatusDesc AS ExternalSubStatusDesc
                                    FROM kkslm_tr_lead lead WITH (NOLOCK)
                                    LEFT JOIN kkslm_ms_option opt WITH (NOLOCK) ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' AND opt.is_Deleted = '0' 
                                    WHERE lead.slm_ticketId = '{ticketId}' ";

                    var lead = slmdb.ExecuteStoreQuery<LeadDefaultData>(sql).FirstOrDefault();
                    if (lead != null)
                    {
                        statusDesc = lead.StatusDesc;
                        subStatusDesc = lead.ExternalSubStatusDesc;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static void GetUpdatedData(string ticketId, out string assignedFlag, out string delegateFlag, out string ownerBranchCode, out string ownerUsername, out string delegateBranchCode, out string delegateUsername
            , out string statusCode, out string telNoSMS, out string countryId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    //var lead = slmdb.kkslm_tr_lead.Where(p => p.slm_ticketId == ticketId).FirstOrDefault();
                    var lead = (from l in slmdb.kkslm_tr_lead
                                join c in slmdb.kkslm_tr_cusinfo on l.slm_ticketId equals c.slm_TicketId
                                where l.slm_ticketId == ticketId
                                select new
                                {
                                    l.slm_Status,
                                    l.slm_AssignedFlag,
                                    l.slm_Delegate_Flag,
                                    l.slm_Owner_Branch,
                                    l.slm_Owner,
                                    l.slm_Delegate_Branch,
                                    l.slm_Delegate,
                                    c.slm_TelNoSms,
                                    c.slm_country_id
                                }).FirstOrDefault();
                    
                    if (lead != null)
                    {
                        statusCode = lead.slm_Status;
                        assignedFlag = lead.slm_AssignedFlag;
                        delegateFlag = lead.slm_Delegate_Flag.ToString();
                        ownerBranchCode = lead.slm_Owner_Branch;
                        ownerUsername = lead.slm_Owner;
                        delegateBranchCode = lead.slm_Delegate_Branch;
                        delegateUsername = lead.slm_Delegate;
                        telNoSMS = lead.slm_TelNoSms;
                        countryId = (lead.slm_country_id == null ? "" : lead.slm_country_id.ToString());
                    }
                    else
                    {
                        statusCode = "";
                        assignedFlag = "";
                        delegateFlag = "";
                        ownerBranchCode = "";
                        ownerUsername = "";
                        delegateBranchCode = "";
                        delegateUsername = "";
                        telNoSMS = "";
                        countryId = "";
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static List<LeadData> GetTicketReferDetail(string ticketId)
        {
            string sql = string.Format(@"SELECT lead.slm_ticketId			AS TicketId
	                                           ,cardtype.slm_CardTypeName	AS CardTypeDesc
	                                           ,info.slm_CitizenId			AS CitizenId
	                                           ,lead.slm_Name				AS Name
	                                           ,lead.slm_LastName			AS LastName
	                                           ,campaign.slm_CampaignName	AS CampaignName
	                                           ,opt.slm_OptionDesc			AS [Status]
	                                           ,lead.slm_ExternalSubStatusDesc AS ExternalSubStatusDesc 
                                            FROM kkslm_tr_lead			AS lead		WITH (NOLOCK)
                                            LEFT JOIN kkslm_tr_cusinfo	AS info     WITH (NOLOCK)		ON lead.slm_ticketId	= info.slm_TicketId
                                            LEFT JOIN kkslm_ms_cardtype AS cardtype WITH (NOLOCK) ON info.slm_CardType	= cardtype.slm_CardTypeId
                                            LEFT JOIN kkslm_ms_campaign AS campaign WITH (NOLOCK) ON lead.slm_CampaignId	= campaign.slm_CampaignId
                                            LEFT JOIN kkslm_ms_option	AS opt      WITH (NOLOCK)		ON lead.slm_Status		= opt.slm_OptionCode		AND opt.slm_OptionType = 'lead status'
                                            WHERE lead.slm_ticketId = '{0}'", ticketId);

            List<LeadData> data = null;
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                data = slmdb.ExecuteStoreQuery<LeadData>(sql).ToList();
            }
            return data;
        }
        public static List<LeadData> GetTicketRelateDetail(string ticketId)
        {
            string sql = string.Format(@"SELECT lead.slm_ticketId			AS TicketId
	                                           ,cardtype.slm_CardTypeName	AS CardTypeDesc
	                                           ,info.slm_CitizenId			AS CitizenId
	                                           ,lead.slm_Name				AS Name
	                                           ,lead.slm_LastName			AS LastName
	                                           ,campaign.slm_CampaignName	AS CampaignName
	                                           ,opt.slm_OptionDesc			AS [Status]
	                                           ,lead.slm_ExternalSubStatusDesc AS ExternalSubStatusDesc 
                                            FROM kkslm_tr_lead			AS lead		WITH (NOLOCK)
                                            LEFT JOIN kkslm_tr_cusinfo	AS info     WITH (NOLOCK)		ON lead.slm_ticketId	= info.slm_TicketId
                                            LEFT JOIN kkslm_ms_cardtype AS cardtype WITH (NOLOCK) ON info.slm_CardType	= cardtype.slm_CardTypeId
                                            LEFT JOIN kkslm_ms_campaign AS campaign WITH (NOLOCK) ON lead.slm_CampaignId	= campaign.slm_CampaignId
                                            LEFT JOIN kkslm_ms_option	AS opt      WITH (NOLOCK)		ON lead.slm_Status		= opt.slm_OptionCode		AND opt.slm_OptionType = 'lead status'
                                            WHERE lead.slm_TicketIdRefer = '{0}' order by lead.slm_createddate desc", ticketId);

            List<LeadData> data = null;
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                data = slmdb.ExecuteStoreQuery<LeadData>(sql).ToList();
            }
            return data;
        }

        //public void UpdateCasFlag(string ticketId, string preleadId, string noteId, string flag)
        //{
        //    try
        //    {
        //        SLM_DBEntities slmdb = new SLM_DBEntities();
        //        if (!string.IsNullOrEmpty(ticketId))
        //        {
        //            int note_Id = int.Parse(noteId);
        //            var note = slmdb.kkslm_note.Where(p => p.slm_NoteId == note_Id).FirstOrDefault();
        //            if (note != null)
        //            {
        //                note.slm_CAS_Flag = flag;
        //                note.slm_CAS_Date = DateTime.Now;
        //                slmdb.SaveChanges();
        //            }
        //        }
        //        else if (!string.IsNullOrEmpty(preleadId))
        //        {
        //            decimal note_Id = decimal.Parse(noteId);
        //            var note = slmdb.kkslm_tr_prelead_note.Where(p => p.slm_NoteId == note_Id).FirstOrDefault();
        //            if (note != null)
        //            {
        //                note.slm_CAS_Flag = flag;
        //                note.slm_CAS_Date = DateTime.Now;
        //                slmdb.SaveChanges();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        //throw ex;
        //    }
        //}

        public static bool ExportNotifyPolicyReport(string ticketId)
        {
            string slmDBName = SLMConstant.SLMDBName;
//            string sql = @"select count(b.slm_id) as cnt
//                from " + slmDBName + @".dbo.kkslm_tr_renewinsurance as a
//	                inner join " + slmDBName + @".dbo.kkslm_tr_notify_renewinsurance_report as b on a.slm_ReceiveNo=b.slm_InsReceiveNo
//                where a.slm_ReceiveNo ='" + receiveno + "' and ISNULL(slm_export_flag,0)=1";

            string sql = @"select count(slm_id) as cnt
                from " + slmDBName + @".dbo.kkslm_tr_notify_renewinsurance_report WITH (NOLOCK) 
                where slm_TicketID ='" + ticketId + "' and ISNULL(slm_export_flag,0)=1 and ISNULL(is_Deleted,0)=0";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                int count = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                return count == 0 ? false : true;
            }
        }

        public static bool ExportNotifyActReport(string receiveno, string ticketId)
        {
            string slmDBName = SLMConstant.SLMDBName;
            string sql;
//            if (receiveno != null && receiveno != "")
//            {
//                sql = @"select count(b.slm_id) as cnt
//                            from " + slmDBName + @".dbo.kkslm_tr_renewinsurance as a
//            	                inner join " + slmDBName + @".dbo.kkslm_tr_notify_act_report as b on a.slm_ReceiveNo=b.slm_InsReceiveNo
//                            where a.slm_ReceiveNo ='" + receiveno + "' and ISNULL(slm_export_flag,0)=1";
//            }
//            else if (ticketId != null && ticketId != "")
//            {
                sql = @"select count(slm_id) as cnt
                from " + slmDBName + @".dbo.kkslm_tr_notify_act_report 
                where slm_TicketID ='" + ticketId + "' and ISNULL(slm_export_flag,0)=1 and ISNULL(is_Deleted,0)=0";
            //}
            //else
            //    return false;

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                int count = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();

                return count == 0 ? false : true;
            }
        }

    }
}
