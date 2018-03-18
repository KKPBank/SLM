using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;
using System.Transactions;

namespace SLM.Biz
{
    public class SlmScr037Biz
    {
        public static string GetNumOfUnassignLead(string username, decimal? stafftype)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.GetNumOfUnassignLead(username, stafftype);
        }

        public static List<SearchLeadResult> GetUnassignLeadList(string username)
        {
            SearchLeadModel search = new SearchLeadModel();
            return search.GetUnassignLeadList(username);
        }

        //public static List<UserMonitoringData> SearchUserMonitoring(string userList, string active, string assignDateFrom, string assignDateTo)
        //{
        //    SearchLeadModel search = new SearchLeadModel();
        //    return search.SearchUserMonitoring(userList, active, assignDateFrom, assignDateTo);
        //}

        public static List<UserMonitoringReInsuranceData> SearchUserMonitoringReInsurance(string userList, string productId, string campaign, string branchcode, string active, string assignDateFrom, string assignDateTo
            , string teamtelesales, string subStatusList)
        {
            SearchObtModel search = new SearchObtModel();
            return search.SearchUserMonitoringReInsurance(userList, productId, campaign, branchcode, active, assignDateFrom, assignDateTo, teamtelesales, subStatusList);
        }

        public static List<SearchUserMonitoringOBTResult> GetUserMonitoringReInsuranceListByUser(SearchUserMonitoringCondition data)
        {
            SearchObtModel search = new SearchObtModel();
            return search.GetUserMonitoringReInsuranceListByUser(data.UserList, data.ProductId, data.Campaign, data.Branchcode, data.AssignDateFrom, data.AssignDateTo, data.Teamtelesales, data.SubStatusList);
        }

        public static List<StaffData> GetStaffList()
        {
            KKSlmMsStaffModel staffmodel = new KKSlmMsStaffModel();
            return staffmodel.GetStaffList();
        }

        public static string GetStaffId(string username)
        {
            KKSlmMsStaffModel staffmodel = new KKSlmMsStaffModel();
            return staffmodel.GetStaffIdData(username);
        }

        public static decimal? GetStaffType(string username)
        {
            KKSlmMsStaffModel staffmodel = new KKSlmMsStaffModel();
            return staffmodel.GetStaffType(username);
        }

        public static List<ControlListData> GetProductGroupData(string ProductGroupId)
        {
            CmtMsProductGroupModel productGroup = new CmtMsProductGroupModel();
            return productGroup.GetProductGroupData(ProductGroupId);
        }

        public static List<ControlListData> GetProductData(string subProductId)
        {
            CmtMappingProductModel product = new CmtMappingProductModel();
            return product.GetProductData(subProductId);
        }

        public static List<ControlListData> GetProductDataNew(string subProductId)
        {
            CmtMappingProductModel product = new CmtMappingProductModel();
            return product.GetProductDataNew(subProductId);
        }
        public static List<ControlListData> GetCampaignData(string productGroupId, string productId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetCampaignData(productGroupId, productId);
        }

        public static List<ControlListData> GetCampaignDataNew(string productGroupId, string productId)
        {
            CmtCampaignProductModel campaign = new CmtCampaignProductModel();
            return campaign.GetCampaignDataNew(productGroupId, productId);
        }
        public static List<ControlListData> GetAllActiveTeamtelesalesData()
        {
            KKSlmMsTeamtelesalesModel campaign = new KKSlmMsTeamtelesalesModel();
            return campaign.GetAllActiveTeamtelesalesData();
        }

        //public static void UpdateTransferOwnerPrelead(List<Decimal> preleadList, string newowner, int staffid, string username, string branchcode, string Oldowner)
        //{
        //    try
        //    {
        //        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        //        {
        //            KKSlmTrPreleadModel lead = new KKSlmTrPreleadModel();
        //            lead.UpdateTransferPreleadOwner(preleadList, newowner, staffid, username, branchcode);

        //            //KKSlmTrActivityModel act = new KKSlmTrActivityModel();
        //            //act.InsertDataForTransfer(preleadList, Oldowner, newowner, username, "", "");

        //            ts.Complete();
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}        

        public static List<ControlListData> GetSubOptionList(string productId, string campaignId, string statusCode)
        {
            KKSlmMsConfigProductSubstatusModel option = new KKSlmMsConfigProductSubstatusModel();
            return option.GetSubStatusActiveList(productId, campaignId, statusCode);
        }

        public static void CreateExcel(SearchUserMonitoringCondition cond, string filename)
        {
            try
            {
                SearchObtModel search = new SearchObtModel();
                var mainList = search.GetUserMonitoringReInsuranceListByUser(cond.UserList, cond.ProductId, cond.Campaign, cond.Branchcode, cond.AssignDateFrom, cond.AssignDateTo, cond.Teamtelesales, cond.SubStatusList);
                if (mainList.Count == 0)
                    throw new Exception("ไม่พบข้อมูล สำหรับการ Exprot Excel");

                List<object[]> data = new List<object[]>();
                foreach (var itm in mainList)
                {
                    data.Add(new object[] {
                    itm.TEAMCODE,
                    itm.EMPCODE,
                    itm.StatusDesc,
                    itm.SubStatusDesc,
                    itm.TicketId,
                    itm.ContractNo,
                    itm.CusFullName,
                    itm.INSNAME,
                    itm.COV_NAME,
                    itm.GROSS_PREMIUM != null ? itm.GROSS_PREMIUM.Value.ToString("#,##0.00") : "0.00",
                    itm.GRADE,
                    itm.PreleadId,
                    itm.Lot,
                    itm.CreatedDate != null ? (itm.CreatedDate.Value.ToString("dd/MM/") + itm.CreatedDate.Value.Year.ToString() + " " + itm.CreatedDate.Value.ToString("HH:mm")) : "",
                    itm.OwnerName,
                    itm.EXP_MONTH ,
                    itm.EXPIRE_DATE != null ? (itm.EXPIRE_DATE.Value.ToString("dd/MM/") + itm.EXPIRE_DATE.Value.Year.ToString()) : "",
                    itm.NEXT_CONTRACT_DATE != null ? (itm.NEXT_CONTRACT_DATE.Value.ToString("dd/MM/") + itm.NEXT_CONTRACT_DATE.Value.Year.ToString()) : ""

                    ,itm.slm_AssignDescription
                    ,itm.slm_Car_By_Gov_Name_Org
                    ,itm.slm_Brand_Name_Org
                    ,itm.slm_Model_name_Org
                    ,itm.slm_Voluntary_Policy_Number
                    ,itm.slm_IsExportExpired.HasValue ? (itm.slm_IsExportExpired.Value ? "Y" : "N") : "N"
                    ,itm.slm_IsExportExpiredDate
                    ,itm.slm_BranchCode
                    ,itm.slm_FlagNotifyPremium
                    ,itm.slm_PolicyGrossPremium
                    ,itm.slm_LatestInsExpireDate
                    ,itm.slm_Chassis_No
                    ,itm.slm_Engine_No
                    ,itm.slm_Car_License_No
                    ,itm.slm_ProvinceRegis_Org
                });
                }

                ExcelExportBiz ebz = new ExcelExportBiz();
                bool ret = ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>() {
                            new ExcelExportBiz.SheetItem(){
                                SheetName = "userMonitoringReInsurance",
                                RowPerSheet = 0,
                                Data = data,
                                Columns = new List<ExcelExportBiz.ColumnItem>()
                                {
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อทีม", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รหัสพนักงาน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สถานะของ Lead", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="สถานะย่อยของ Lead", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="TicketId", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขที่สัญญา", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อ-สกุล ลูกค้า", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อบริษัทประกันภัย", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทประกันภัย", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ค่าเบี้ยประกัน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Grade ลูกค้า", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Prelead ID", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Lot", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="Transfer Date", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ชื่อ MKT", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เดือนคุ้มครองวันสิ้นสุดกรมธรรม์", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันสิ้นสุดกรมธรรม์", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่นัดโทรหาลูกค้า", ColumnDataType= ExcelExportBiz.ColumnType.Text },

                                    new ExcelExportBiz.ColumnItem() { ColumnName="หมายเหตุการจ่ายงาน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ประเภทการใช้งานของรถ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="ยี่ห้อ", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="รุ่น", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="เลขกรมธรรม์(ปีเดิม)", ColumnDataType= ExcelExportBiz.ColumnType.Text },

                                    new ExcelExportBiz.ColumnItem() { ColumnName="ออกรายงาน LeadsForTransfer", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() { ColumnName="วันที่ออกรายงาน", ColumnDataType= ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() {ColumnName= "รหัสสาขา", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() {ColumnName= "Flag ค่าเบี้ยปีต่อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() {ColumnName= "ราคาค่าเบี้ยใบเตือนที่mapping ได้ ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() {ColumnName= "วันที่หมดอายุกรมธรรม์ที่ mapping ได้", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() {ColumnName= "เลขตัวถัง", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() {ColumnName= "เลขตัวเครื่อง", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() {ColumnName= "ทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                    new ExcelExportBiz.ColumnItem() {ColumnName= "จังหวัดที่จดทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text }
                                }
                            }
                        });

                if (!ret)
                    throw new Exception(ebz.ErrorMessage);
            }
            catch
            {
                throw;
            }
        }
    }
}
