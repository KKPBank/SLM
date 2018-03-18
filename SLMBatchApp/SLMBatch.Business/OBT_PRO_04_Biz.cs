using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Transactions;
using SLMBatch.Common;
using SLMBatch.Entity;
using SLMBatch.Data;

namespace SLMBatch.Business
{
    public class OBT_PRO_04_Biz : ServiceBase
    {
        private string _cmt_whereClause = "";
        private decimal? _cmt_lot = 0;
        private string cmtProductId = "";

        private const string QueryRemoveSpecialCharsPrefix = "REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(";
        private const string QueryRemoveSpecialCharsSuffix = @",' ',''),'-',''),'*',''),'_',''),'\',''),'/','')";

        /// <summary>
        /// กวาดข้อมูล prelead จาก CMT ไปที่ OBT
        /// </summary>
        /// <param name="batchCode"></param>
        public long InsertPreLead(string batchCode)
        {
            //รอเทส SIT2

            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;

            string productId = "";
            string engineNo = "";
            string chassisNo = "";
            string chassisNoCleaned = "";
            List<CmtObtProductData> process_list = new List<CmtObtProductData>();
            CmtObtProductData data = null;
            List<ProvinceData> provinceDataList = new List<ProvinceData>();
            List<BrandData> brandList = new List<BrandData>();
            List<SubModelData> subModelList = new List<SubModelData>();
            bool InLoop = false;

            try
            {
                batchMonitorId = BizUtil.SetStartTime(batchCode);
                BizUtil.CheckPrerequisite(batchCode);

                List<CmtObtProductData> cmt_mainlist = GetCmtObtProduct();  //Get ข้อมูลที่มาจาก CMT
                totalRecord = cmt_mainlist.Count;

                if (cmt_mainlist.Count > 0)
                {
                    //Get data for use in all process
                    SLMDBEntities db = AppUtil.GetSlmDbEntities();
                    provinceDataList = AppUtil.GetProvinceDataList(db);
                    brandList = AppUtil.GetRedbookBrandList(db);
                    subModelList = AppUtil.GetRedbookSubModelList(db);

                    //1.Insert CMT data ที่ติด blacklist ใน table temp blacklist, function return list ของ cmtId ที่ติด blacklist
                    List<decimal?> blaklist_cmtIds = DoProcessBlacklist(brandList, subModelList);
                    totalSuccess += blaklist_cmtIds.Count;

                    //2.นำ cmtId ที่ติด blacklist ออกจาก cmt_mainlist
                    cmt_mainlist = cmt_mainlist.Where(p => blaklist_cmtIds.Contains(p.Cmt_Product_Id) == false).ToList();

                    //3.นำ cmt_mainlist มา order ตาม field ที่ใช้เช็ก dedup (productId, chassisNo, engineNo)
                    cmt_mainlist = cmt_mainlist.OrderBy(p => p.ProductId).ThenBy(p => p.ChassisNoUpperCleaned).ThenBy(p => p.EngineNoUpper).ThenByDescending(p => p.Contract_Year).ToList();

                    //4. Get config day
                    var configProductDayList = db.kkslm_ms_config_product_day.ToList();

                    //5. Get CMT Address
                    List<CMT_OBT_ADDRESS> cmtAddresslist = db.CMT_OBT_ADDRESS.Where(p => p.Running_seq == _cmt_lot).ToList();

                    var dedupMasterList = GetDedupMasterList(db);

                    InLoop = true;
                    while (cmt_mainlist.Count > 0)
                    {
                        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

                        data = cmt_mainlist[0];     //นำ record บนสุดมาเช็ก

                        productId = data.ProductId;
                        engineNo = data.EngineNoUpper;
                        chassisNo = data.ChassisNoUpper;
                        chassisNoCleaned = data.ChassisNoUpperCleaned;
                        cmtProductId = data.Cmt_Product_Id != null ? data.Cmt_Product_Id.Value.ToString() : "";

                        //process_list = cmt_mainlist.Where(p => p.ProductId == productId && p.ChassisNoUpperCleaned == chassisNoCleaned && p.EngineNoUpper == engineNo).ToList();
                        process_list = cmt_mainlist.Where(p => p.ProductId == productId && p.ChassisNoUpperCleaned == chassisNoCleaned).ToList();
                        if (process_list.Count > 1)     //ถ้าข้อมูล CMT มาซ้ำ
                        {
                            //เอา record แรกไปเช็กได้เลย เนื่องจาก 3 field (productId, chassisNo, engineNo) ที่ where จะเหมือนกันทุก record
                            //int count = dedupMasterList.Where(d => d.ProductId == process_list[0].ProductId && d.ChassisNoCleaned == process_list[0].ChassisNoUpperCleaned && d.EngineNo == process_list[0].EngineNoUpper).Count();
                            int count = dedupMasterList.Where(d => d.ProductId == process_list[0].ProductId && d.ChassisNoCleaned == process_list[0].ChassisNoUpperCleaned).Count();
                            if (count == 0)     //ไม่ติด Dedup
                            {
                                //1.เลือก row ล่าสุดไป Insert prelead (order ตาม ปีที่ต่อประกันภัยรถยนต์ล่าสุด, วันที่สร้างสัญญาล่าสุด)
                                var insertData = process_list
                                    .OrderByDescending(p => p.Voluntary_Policy_EXP_YEAR)
                                    .ThenByDescending(p => p.Contract_App_Date)
                                    .FirstOrDefault();
                                cmtProductId = insertData.Cmt_Product_Id != null ? insertData.Cmt_Product_Id.Value.ToString() : "";

                                DoInsertPrelead(slmdb, insertData, provinceDataList, brandList, subModelList, cmtAddresslist);

                                //2.เลือก row ล่าสุดไป Insert Master Dedup
                                DoInsertMasterDedup(slmdb, insertData, configProductDayList);

                                //3.นำ row ที่เหลือ ไป Insert ลง Temp Force Dedup
                                var tempList = process_list.Where(p => p.Cmt_Product_Id != insertData.Cmt_Product_Id).ToList();
                                DoInsertTempForceDedup(slmdb, tempList, brandList, subModelList);
                            }
                            else
                            {
                                //1.นำ row ทั้งหมดไป Insert ลง Temp Force Dedup
                                DoInsertTempForceDedup(slmdb, process_list, brandList, subModelList);
                            }
                        }
                        else if (process_list.Count == 1)   //ข้อมูล CMT ไม่ซ้ำ
                        {
                            //int count = dedupMasterList.Where(d => d.ProductId == process_list[0].ProductId && d.ChassisNoCleaned == process_list[0].ChassisNoUpperCleaned && d.EngineNo == process_list[0].EngineNoUpper).Count();
                            int count = dedupMasterList.Where(d => d.ProductId == process_list[0].ProductId && d.ChassisNoCleaned == process_list[0].ChassisNoUpperCleaned).Count();
                            if (count == 0)     //ไม่ติด Dedup
                            {
                                cmtProductId = process_list[0].Cmt_Product_Id != null ? process_list[0].Cmt_Product_Id.Value.ToString() : "";

                                //1.Insert prelead
                                DoInsertPrelead(slmdb, process_list[0], provinceDataList, brandList, subModelList, cmtAddresslist);

                                //2.Insert Master Dedup
                                DoInsertMasterDedup(slmdb, process_list[0], configProductDayList);
                            }
                            else
                            {
                                //Insert ลง Temp Force Dedup
                                DoInsertTempForceDedup(slmdb, process_list, brandList, subModelList);
                            }
                        }

                        totalSuccess += process_list.Count;
                        Console.WriteLine("Cmt_Product_Id " + data.Cmt_Product_Id.ToString() + ": SUCCESS");

                        cmt_mainlist = cmt_mainlist.Except(process_list).ToList();  //นำ list ที่ทำการ process แล้ว ออกจาก cmt_mainlist หลัก
                    }

                    DoInsertPortMonitor(batchCode, _cmt_lot, batchMonitorId);
                }
                else
                {
                    totalSuccess = 0;
                    totalFail = 0;
                }

                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                Rollback(batchCode, batchMonitorId, _cmt_lot);
                totalSuccess = 0;
                totalFail = totalRecord;

                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                string errorDetail = "";
                if (InLoop)
                {
                    errorDetail = "Error at group of records CmtProductId=" + cmtProductId + ", ProductId=" + productId + ", ChassisNo=" + chassisNo + ", chassisNoCleaned=" + chassisNoCleaned + ", EngineNo=" + engineNo + ", Error=" + message;
                }
                else
                    errorDetail = message;

                Util.WriteLogFile(logfilename, batchCode, errorDetail);
                BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
                batchMonitorId = -1;
            }

            return batchMonitorId;
        }

        private void Rollback(string batchCode, long batchMonitorId, decimal? lot)
        {
            try
            {
                if (lot != null)
                {
                    SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

                    string sql = "";

                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        sql = "DELETE FROM " + AppConstant.SLMDBName + ".dbo.kkslm_tr_prelead_temp where slm_CmtLot = '" + lot.Value + "'";
                        slmdb.ExecuteStoreCommand(sql);

                        sql = "DELETE FROM " + AppConstant.SLMDBName + ".dbo.kkslm_tr_prelead_assign_log where slm_CmtLot = '" + lot.Value + "'";
                        slmdb.ExecuteStoreCommand(sql);

                        sql = "DELETE FROM " + AppConstant.SLMDBName + ".dbo.kkslm_tr_prelead_address where slm_CmtLot = '" + lot.Value + "'";
                        slmdb.ExecuteStoreCommand(sql);

                        sql = "DELETE FROM " + AppConstant.SLMDBName + ".dbo.kkslm_tr_prelead where slm_CmtLot = '" + lot.Value + "'";
                        slmdb.ExecuteStoreCommand(sql);

                        sql = "DELETE FROM " + AppConstant.SLMDBName + ".dbo.kkslm_ms_config_product_dedup where slm_CmtLot = '" + lot.Value + "'";
                        slmdb.ExecuteStoreCommand(sql);

                        sql = "DELETE FROM " + AppConstant.SLMDBName + ".dbo.kkslm_tr_preleadportmonitor where slm_CmtLot = '" + lot.Value + "'";
                        slmdb.ExecuteStoreCommand(sql);

                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                string errorDetail = "Cannot rollback process because " + message;

                Util.WriteLogFile(logfilename, batchCode, errorDetail);
                BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
            }
        }

        private List<CmtObtProductData> GetCmtObtProduct()
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                string cmt_sql = "SELECT DISTINCT Running_seq FROM CMT_OBT_PRODUCT WHERE Running_seq IS NOT NULL";
                List<decimal?> cmtLotList = slmdb.ExecuteStoreQuery<decimal?>(cmt_sql).ToList();
                List<decimal?> obtLotList = slmdb.kkslm_tr_preleadportmonitor.Where(p => p.is_Deleted == false).Select(p => p.slm_CMTLot).ToList();
                List<decimal?> list = cmtLotList.Except(obtLotList).ToList();

                if (list.Count > 0)
                {
                    if (list.Count > 1)
                        throw new Exception("Cmt active lot has more than one lot");

                    _cmt_lot = list[0];

                    if (_cmt_lot == null)
                        throw new Exception("Cmt active lot value is null");

                    string sql = string.Format(@"SELECT CMT.Cmt_Product_Id AS Cmt_Product_Id, CP.PR_ProductId AS ProductId, title.slm_TitleId AS TitleId, oc.slm_OccupationId AS OccupationId, cartype.slm_InsurancecarTypeId AS InsurancecarTypeId
                                    , regispro.slm_ProvinceId AS RegisProvinceId, volunmkt.slm_EmpCode AS VoluntaryMktId, volunmkttitle.slm_TitleId AS VoluntaryMktTitleId, benifittitle.slm_TitleId AS BenefitTitleId
                                    , drivertitle1.slm_TitleId AS DriverTitleId1, drivertitle2.slm_TitleId AS DriverTitleId2
                                    , guarantortitle1.slm_TitleId AS GuarantorTitleId1, guarantorrelate1.slm_RelateId AS GuarantorRelationId1
                                    , guarantortitle2.slm_TitleId AS GuarantorTitleId2, guarantorrelate2.slm_RelateId AS GuarantorRelationId2
                                    , guarantortitle3.slm_TitleId AS GuarantorTitleId3, guarantorrelate3.slm_RelateId AS GuarantorRelationId3
                                    , CMT.*, RTRIM(LTRIM(UPPER(CMT.License_No))) LicenseNoUpper, RTRIM(LTRIM(UPPER(CMT.Engine_No))) AS EngineNoUpper
                                    , RTRIM(LTRIM(UPPER(CMT.Chassis_No))) AS ChassisNoUpper
                                    , {1}RTRIM(LTRIM(UPPER(CMT.Chassis_No))){2} AS ChassisNoUpperCleaned
                                    , CMT.Running_seq AS CmtLot
                                    FROM {0}.dbo.CMT_OBT_PRODUCT CMT
                                    LEFT JOIN {0}.dbo.CMT_CAMPAIGN_PRODUCT CP ON CP.PR_CampaignId = CMT.Campaign_Id
                                    LEFT JOIN {0}.dbo.kkslm_ms_title title ON title.slm_TitleName = cmt.Thai_Title_Name and title.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_occupation oc ON oc.slm_OccupationCode = cmt.Career_Key
                                    LEFT JOIN {0}.dbo.kkslm_ms_insurancecartype cartype ON cartype.slm_InsurancecarTypeName = cmt.Car_By_Gov_Name and cartype.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_province regispro ON regispro.slm_ProvinceNameTH = cmt.Register_Province
                                    LEFT JOIN {0}.dbo.kkslm_ms_staff volunmkt ON volunmkt.slm_MarketingCode = cmt.Voluntary_MKT_ID AND RTRIM(LTRIM(volunmkt.slm_MarketingCode)) <> '' AND volunmkt.slm_MarketingCode IS NOT NULL
                                    LEFT JOIN {0}.dbo.kkslm_ms_title volunmkttitle ON volunmkttitle.slm_TitleName = cmt.Voluntary_MKT_Title and volunmkttitle.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_title benifittitle ON benifittitle.slm_TitleName = cmt.Benefit_Title_Name and benifittitle.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_title drivertitle1 ON drivertitle1.slm_TitleName = cmt.Driver_Title_Name1 and drivertitle1.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_title drivertitle2 ON drivertitle2.slm_TitleName = cmt.Driver_Title_Name2 and drivertitle2.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_title guarantortitle1 ON guarantortitle1.slm_TitleName = cmt.Guarantor_Title_Name1 and guarantortitle1.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_relate guarantorrelate1 ON guarantorrelate1.slm_RelateDesc = cmt.Guarantor_Relation1 and guarantorrelate1.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_title guarantortitle2 ON guarantortitle2.slm_TitleName = cmt.Guarantor_Title_Name2 and guarantortitle2.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_relate guarantorrelate2 ON guarantorrelate2.slm_RelateDesc = cmt.Guarantor_Relation2 and guarantorrelate2.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_title guarantortitle3 ON guarantortitle3.slm_TitleName = cmt.Guarantor_Title_Name3 and guarantortitle3.is_Deleted = 0
                                    LEFT JOIN {0}.dbo.kkslm_ms_relate guarantorrelate3 ON guarantorrelate3.slm_RelateDesc = cmt.Guarantor_Relation3 and guarantorrelate3.is_Deleted = 0
                                    {{0}} ", AppConstant.SLMDBName, QueryRemoveSpecialCharsPrefix, QueryRemoveSpecialCharsSuffix);

                    _cmt_whereClause = " WHERE CMT.Running_seq IN ('" + _cmt_lot.Value.ToString() + "') ";

                    sql = string.Format(sql, _cmt_whereClause);
                    return slmdb.ExecuteStoreQuery<CmtObtProductData>(sql).ToList();
                }
                else
                    return new List<CmtObtProductData>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<CmtObtProductData> GetBlacklistData()
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                string sql = string.Format(@"SELECT DISTINCT A.*
                                FROM {0}.dbo.kkslm_ms_config_product_blacklist BL
                                INNER JOIN (
	                                SELECT CMT.*, CP.PR_ProductId AS ProductId, CMT.Running_seq AS CmtLot, RTRIM(LTRIM(UPPER(CMT.Chassis_No))) AS ChassisNoUpper 
	                                FROM {0}.dbo.CMT_OBT_PRODUCT CMT
	                                LEFT JOIN {0}.dbo.CMT_CAMPAIGN_PRODUCT CP ON CP.PR_CampaignId = CMT.Campaign_Id
	                                {{0}} ) A ON A.ProductId = BL.slm_Product_Id AND A.Single_View_Customer = BL.slm_CitizenId
                                WHERE (CONVERT(DATE, GETDATE()) BETWEEN CONVERT(DATE, BL.slm_StartDate) AND CONVERT(DATE, BL.slm_EndDate)) AND BL.slm_IsActive = 1 ", AppConstant.SLMDBName);

                sql = string.Format(sql, _cmt_whereClause);

                return slmdb.ExecuteStoreQuery<CmtObtProductData>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<decimal?> DoProcessBlacklist(List<BrandData> brandList, List<SubModelData> subModelList)
        {
            try
            {
                List<decimal?> blacklist_cmtIds = new List<decimal?>();
                List<CmtObtProductData> blacklists = GetBlacklistData();

                if (blacklists.Count > 0)
                {
                    SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

                    foreach (CmtObtProductData data in blacklists)
                    {
                        blacklist_cmtIds.Add(data.Cmt_Product_Id);

                        slmdb.kkslm_tr_prelead_temp.AddObject(SetTempPrelead(data, AppConstant.PreleadAssignType.Blacklist, brandList, subModelList));

                        kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                        {
                            slm_Cmt_Product_Id = data.Cmt_Product_Id,
                            slm_CmtLot = data.CmtLot,
                            slm_NewAssignType = AppConstant.PreleadAssignType.Blacklist,
                            slm_ContractNo = data.Contract_Number,
                            slm_CreatedDate = DateTime.Now,
                            slm_CreatedBy = "SYSTEM"
                        };
                        slmdb.kkslm_tr_prelead_assign_log.AddObject(log);
                    }

                    //decimal?[] tmp = blacklist_cmtIds.ToArray();
                    //var cmtproduct_list = slmdb.CMT_OBT_PRODUCT.Where(p => tmp.Contains(p.Cmt_Product_Id) == true).ToList();
                    //cmtproduct_list.ForEach(p => p.OBT_Flag = true);

                    slmdb.SaveChanges();
                }

                return blacklist_cmtIds;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception("Method DoProcessBlacklist, Error=" + message);
            }
        }

        private kkslm_tr_prelead_temp SetTempPrelead(CmtObtProductData data, string preleadAssignType, List<BrandData> brandList, List<SubModelData> subModelList)
        {
            try
            {
                kkslm_tr_prelead_temp temp = new kkslm_tr_prelead_temp()
                {
                    slm_Cmt_Product_Id = data.Cmt_Product_Id,
                    slm_CmtLot = data.CmtLot,
                    slm_Type = preleadAssignType,
                    slm_Contract_Number = data.Contract_Number,
                    slm_CampaignId = data.Campaign_Id,
                    slm_Product_Id = data.ProductId,
                    slm_BranchCode = data.Branch,
                    slm_Contract_Year = data.Contract_Year,
                    slm_Contract_Status = data.Contract_Status,
                    slm_Car_Category = data.Car_Category,
                    slm_Customer_Key = data.Customer_Key,
                    slm_TitleId = data.TitleId,                                             //AppUtil.GetTitleId(slmdb, data.Thai_Title_Name);
                    slm_Title_Name_Org = data.Thai_Title_Name,                              //เก็บค่าจาก CMT
                    slm_Name = data.Thai_First_Name,
                    slm_LastName = data.Thai_Last_Name,
                    slm_CitizenId = data.Single_View_Customer,
                    slm_Marital_Status = data.Marital_Status,
                    slm_CardType_Org = data.Customer_Type,                                  //เก็บค่าจาก CMT
                    slm_Tax_Id = data.Tax_ID,
                    slm_OccupationId = data.OccupationId,                                   //AppUtil.GetOccupationId(slmdb, data.Career_Key);
                    slm_Career_Key_Org = data.Career_Key,                                   //เก็บค่าจาก CMT
                    slm_Career_Desc_Org = data.Career_Desc,                                 //เก็บค่าจาก CMT
                    slm_Car_By_Gov_Id = data.InsurancecarTypeId,                            //AppUtil.GetInsuranceCarTypeId(slmdb, data.Car_By_Gov_Name);
                    slm_Car_By_Gov_Name_Org = data.Car_By_Gov_Name,                         //เก็บค่าจาก CMT
                    slm_Brand_Code = brandList.Where(p => p.BrandCode == data.Brand_Code).Count() > 0 ? data.Brand_Code : null,
                    slm_Brand_Code_Org = data.Brand_Code,                                   //เก็บค่าจาก CMT
                    slm_Brand_Name_Org = data.Brand_Name,                                   //เก็บค่าจาก CMT
                    slm_Model_Code = subModelList.Where(p => p.KKKey == data.Model_Code).Select(p => p.ModelCode).FirstOrDefault(),
                    slm_Model_Code_Org = data.Model_Code,                                   //เก็บค่าจาก CMT
                    slm_Model_name_Org = data.Model_Name,                                   //เก็บค่าจาก CMT
                    slm_Engine_No = data.EngineNoUpper,
                    slm_Chassis_No = data.ChassisNoUpper,
                    slm_Model_Year = data.Model_Year,
                    slm_Car_License_No = data.LicenseNoUpper,
                    slm_ProvinceRegis = data.RegisProvinceId,
                    slm_ProvinceRegis_Org = data.Register_Province,                         //เก็บค่าจาก CMT
                    slm_Cc = data.CC != null ? data.CC.Value.ToString() : null,
                    slm_Voluntary_Mkt_Id = data.VoluntaryMktId,                             //AppUtil.GetEmpCode(slmdb, data.Voluntary_MKT_ID);
                    slm_Voluntary_Mkt_Id_Org = data.Voluntary_MKT_ID,                       //เก็บค่าจาก CMT (Voluntary_MKT_ID == marketingCode)
                    slm_Voluntary_Mkt_TitleId = data.VoluntaryMktTitleId,                   //AppUtil.GetTitleId(slmdb, data.Voluntary_MKT_Title);
                    slm_Voluntary_Mkt_Title_Org = data.Voluntary_MKT_Title,                 //เก็บค่าจาก CMT
                    slm_Voluntary_Mkt_First_Name = data.Voluntary_MKT_First_Name,
                    slm_Voluntary_Mkt_Last_Name = data.Voluntary_MKT_Last_Name,
                    slm_Voluntary_Policy_Number = data.Voluntary_Policy_Number,
                    slm_Voluntary_Type_Key = data.Voluntary_Type_Key,
                    slm_Voluntary_Type_Name = data.Voluntary_Type_Name,
                    slm_Voluntary_Policy_Year = data.Voluntary_Policy_Year,
                    slm_Voluntary_Policy_Exp_Year = data.Voluntary_Policy_EXP_YEAR,
                    slm_Voluntary_Policy_Exp_Month = data.Voluntary_Policy_EXP_MONTH,
                    slm_Voluntary_Cov_Amt = data.Voluntary_COV_AMT,
                    slm_Voluntary_Gross_Premium = data.Voluntary_Gross_Premium,
                    slm_Voluntary_Channel_Key = data.Voluntary_Channel_Key,
                    slm_Voluntary_Company_Code = data.Voluntary_Company_Code,
                    slm_Voluntary_Company_Name = data.Voluntary_Company_Name,
                    slm_Benefit_TitleId = data.BenefitTitleId,                              //AppUtil.GetTitleId(slmdb, data.Benefit_Title_Name);
                    slm_Benefit_Title_Name_Org = data.Benefit_Title_Name,                   //เก็บค่าจาก CMT
                    slm_Benefit_First_Name = data.Benefit__First_Name,
                    slm_Benefit_Last_Name = data.Benefit_Last_Name,
                    slm_Benefit_Telno = data.Benefit_TelNo,
                    slm_Driver_TitleId1 = data.DriverTitleId1,                              //AppUtil.GetTitleId(slmdb, data.Driver_Title_Name1);
                    slm_Driver_Title_Name1_Org = data.Driver_Title_Name1,
                    slm_Driver_First_Name1 = data.Driver_First_Name1,
                    slm_Driver_Last_Name1 = data.Driver_Last_Name1,
                    slm_Driver_Telno1 = data.Driver_TelNo1,
                    slm_Driver_TitleId2 = data.DriverTitleId2,                              //AppUtil.GetTitleId(slmdb, data.Driver_Title_Name2);
                    slm_Driver_Title_Name2_Org = data.Driver_Title_Name2,
                    slm_Driver_First_Name2 = data.Driver_First_Name2,
                    slm_Driver_Last_Name2 = data.Driver_Last_Name2,
                    slm_Driver_Telno2 = data.Driver_TelNo2,
                    slm_Compulsory_Policy_Number = data.Compulsory_Policy_Number,
                    slm_Compulsory_Policy_Year = data.Compulsory_Policy_Year,
                    slm_Compulsory_Cov_Amt = data.Compulsory_COV_AMT,
                    slm_Compulsory_Gross_Premium = data.Compulsory_Gross_Premium,
                    slm_Compulsory_Company_Code = data.Compulsory_Company_Code,
                    slm_Compulsory_Company_Name = data.Compulsory_Company_Name,
                    slm_Guarantor_Code1 = data.Guarantor_Code1,
                    slm_Guarantor_TitleId1 = data.GuarantorTitleId1,                        //AppUtil.GetTitleId(slmdb, data.Guarantor_Title_Name1);
                    slm_Guarantor_Title_Name1_Org = data.Guarantor_Title_Name1,
                    slm_Guarantor_First_Name1 = data.Guarantor_First_Name1,
                    slm_Guarantor_Last_Name1 = data.Guarantor_Last_Name1,
                    slm_Guarantor_Card_Id1 = data.Guarantor_Card_ID1,
                    slm_Guarantor_RelationId1 = data.GuarantorRelationId1,                  //AppUtil.GetRelationId(slmdb, data.Guarantor_Relation1);
                    slm_Guarantor_Relation1_Org = data.Guarantor_Relation1,
                    slm_Guarantor_Telno1 = data.Guarantor_TelNo1,
                    slm_Guarantor_Code2 = data.Guarantor_Code2,
                    slm_Guarantor_TitleId2 = data.GuarantorTitleId2,                        //AppUtil.GetTitleId(slmdb, data.Guarantor_Tile_Name2);
                    slm_Guarantor_Title_Name2_Org = data.Guarantor_Title_Name2,
                    slm_Guarantor_First_Name2 = data.Guarantor_First_Name2,
                    slm_Guarantor_Last_Name2 = data.Guarantor_Last_Name2,
                    slm_Guarantor_Card_Id2 = data.Guarantor_Card_ID2,
                    slm_Guarantor_RelationId2 = data.GuarantorRelationId2,                  //AppUtil.GetRelationId(slmdb, data.Guarantor_Relation2);
                    slm_Guarantor_Relation2_Org = data.Guarantor_Relation2,
                    slm_Guarantor_Telno2 = data.Guarantor_TelNo2,
                    slm_Guarantor_Code3 = data.Guarantor_Code3,
                    slm_Guarantor_TitleId3 = data.GuarantorTitleId3,                        //AppUtil.GetTitleId(slmdb, data.Guarantor_Tile_Name3);
                    slm_Guarantor_Title_Name3_Org = data.Guarantor_Title_Name3,
                    slm_Guarantor_First_Name3 = data.Guarantor_First_Name3,
                    slm_Guarantor_Last_Name3 = data.Guarantor_Last_Name3,
                    slm_Guarantor_Card_Id3 = data.Guarantor_Card_ID3,
                    slm_Guarantor_RelationId3 = data.GuarantorRelationId3,                  //AppUtil.GetRelationId(slmdb, data.Guarantor_Relation3);
                    slm_Guarantor_Relation3_Org = data.Guarantor_Relation3,
                    slm_Guarantor_Telno3 = data.Guarantor_TelNo3,
                    slm_Grade = data.Grade,
                    slm_Reference1 = data.Reference1,
                    slm_Reference2 = data.Reference2
                };

                if (data.Customer_Type == "Y")          //บุคคลธรรมดา
                    temp.slm_CardTypeId = 1;
                else if (data.Customer_Type == "N")     //นิติบุคคล
                    temp.slm_CardTypeId = 2;

                if (!string.IsNullOrEmpty(data.Birth_Date))
                {
                    DateTime birthDate = AppUtil.ConvertToDateTime("Birth_Date", data.Birth_Date, AppConstant.DateTimeFormat.Format1);
                    if (birthDate.Year != 1 && birthDate.Year != 1900)
                        temp.slm_Birthdate = birthDate;
                }

                if (!string.IsNullOrEmpty(data.ExpiryDate))
                {
                    DateTime expiryDate = AppUtil.ConvertToDateTime("ExpiryDate", data.ExpiryDate, AppConstant.DateTimeFormat.Format1);
                    if (expiryDate.Year != 1 && expiryDate.Year != 1900)
                        temp.slm_Expire_Date = expiryDate;
                }

                if (!string.IsNullOrEmpty(data.Voluntary_Policy_EFF_DATE))
                {
                    DateTime policyEffDate = AppUtil.ConvertToDateTime("Voluntary_Policy_EFF_DATE", data.Voluntary_Policy_EFF_DATE, AppConstant.DateTimeFormat.Format1);
                    if (policyEffDate.Year != 1 && policyEffDate.Year != 1900)
                        temp.slm_Voluntary_Policy_Eff_Date = policyEffDate;
                }

                if (!string.IsNullOrEmpty(data.Voluntary_Policy_EXP_DATE))
                {
                    DateTime policyExpDate = AppUtil.ConvertToDateTime("Voluntary_Policy_EXP_DATE", data.Voluntary_Policy_EXP_DATE, AppConstant.DateTimeFormat.Format1);
                    if (policyExpDate.Year != 1 && policyExpDate.Year != 1900)
                        temp.slm_Voluntary_Policy_Exp_Date = policyExpDate;
                }

                if (!string.IsNullOrEmpty(data.Voluntary_First_Create_Date))
                {
                    DateTime firstCreateDate = AppUtil.ConvertToDateTime("Voluntary_First_Create_Date", data.Voluntary_First_Create_Date, AppConstant.DateTimeFormat.Format1);
                    if (firstCreateDate.Year != 1 && firstCreateDate.Year != 1900)
                        temp.slm_Voluntary_First_Create_Date = firstCreateDate;
                }

                if (!string.IsNullOrEmpty(data.Compulsory_Policy_EFF_DATE))
                {
                    DateTime policyEffDate = AppUtil.ConvertToDateTime("Compulsory_Policy_EFF_DATE", data.Compulsory_Policy_EFF_DATE, AppConstant.DateTimeFormat.Format1);
                    if (policyEffDate.Year != 1 && policyEffDate.Year != 1900)
                        temp.slm_Compulsory_Policy_Eff_Date = policyEffDate;
                }

                if (!string.IsNullOrEmpty(data.Compulsory_Policy_EXP_DATE))
                {
                    DateTime policyExpDate = AppUtil.ConvertToDateTime("Compulsory_Policy_EXP_DATE", data.Compulsory_Policy_EXP_DATE, AppConstant.DateTimeFormat.Format1);
                    if (policyExpDate.Year != 1 && policyExpDate.Year != 1900)
                        temp.slm_Compulsory_Policy_Exp_Date = policyExpDate;
                }

                if (!string.IsNullOrEmpty(data.Driver_Birth_Date1))
                {
                    DateTime driverBirthdate1 = AppUtil.ConvertToDateTime("Driver_Birth_Date1", data.Driver_Birth_Date1, AppConstant.DateTimeFormat.Format1);
                    if (driverBirthdate1.Year != 1 && driverBirthdate1.Year != 1900)
                        temp.slm_Driver_Birthdate1 = driverBirthdate1;
                }

                if (!string.IsNullOrEmpty(data.Driver_Birth_Date2))
                {
                    DateTime driverBirthdate2 = AppUtil.ConvertToDateTime("Driver_Birth_Date2", data.Driver_Birth_Date2, AppConstant.DateTimeFormat.Format1);
                    if (driverBirthdate2.Year != 1 && driverBirthdate2.Year != 1900)
                        temp.slm_Driver_Birthdate2 = driverBirthdate2;
                }

                if (!string.IsNullOrEmpty(data.UpdateDate))
                {
                    DateTime updateDate = AppUtil.ConvertToDateTime("UpdateDate", data.UpdateDate, AppConstant.DateTimeFormat.Format2);
                    if (updateDate.Year != 1 && updateDate.Year != 1900)
                        temp.slm_View_UpdateDate = updateDate;
                }

                if (!string.IsNullOrEmpty(data.Contract_App_Date))
                {
                    DateTime contractAppDate = AppUtil.ConvertToDateTime("Contract_App_Date", data.Contract_App_Date, AppConstant.DateTimeFormat.Format1);
                    if (contractAppDate.Year != 1 && contractAppDate.Year != 1900)
                        temp.slm_Contract_App_Date = contractAppDate;
                }

                DateTime createDate = DateTime.Now;
                temp.slm_CreatedBy = "SYSTEM";
                temp.slm_CreatedDate = createDate;
                temp.slm_UpdatedBy = "SYSTEM";
                temp.slm_UpdatedDate = createDate;
                temp.is_Deleted = false;

                return temp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<DedupData> GetDedupMasterList(SLMDBEntities slmdb)
        {
            try
            {
                slmdb = AppUtil.GetSlmDbEntities();
                string sql = string.Format(@"SELECT slm_cp_DedupId AS DedupId, slm_Product_Id AS ProductId
                                , slm_StartDate AS StartDate, slm_EndDate AS EndDate
                                , slm_Engine_No AS EngineNo
                                , slm_Chassis_No AS ChassisNo
                                , {1}slm_Chassis_No{2} AS ChassisNoCleaned
                                FROM {0}.dbo.kkslm_ms_config_product_dedup
                                WHERE (CONVERT(DATE, GETDATE()) BETWEEN CONVERT(DATE, slm_StartDate) AND CONVERT(DATE, slm_EndDate)) AND is_Deleted = 0 ",
                                AppConstant.SLMDBName, QueryRemoveSpecialCharsPrefix, QueryRemoveSpecialCharsSuffix);

                return slmdb.ExecuteStoreQuery<DedupData>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoInsertPrelead(SLMDBEntities slmdb, CmtObtProductData data, List<ProvinceData> provinceDataList, List<BrandData> brandList, List<SubModelData> subModelList, List<CMT_OBT_ADDRESS> cmtAddressList)
        {
            try
            {
                cmtProductId = data.Cmt_Product_Id != null ? data.Cmt_Product_Id.Value.ToString() : "";

                if (string.IsNullOrEmpty(data.ProductId))
                    throw new Exception("ไม่พบ ProductId สำหรับ CampaignId " + data.Campaign_Id);

                kkslm_tr_prelead prelead = new kkslm_tr_prelead()
                {
                    slm_Cmt_Product_Id = data.Cmt_Product_Id,
                    slm_CmtLot = data.CmtLot,
                    slm_Contract_Number = data.Contract_Number,
                    slm_CampaignId = data.Campaign_Id,
                    slm_Product_Id = data.ProductId,
                    slm_BranchCode = data.Branch,
                    slm_Contract_Year = data.Contract_Year,
                    slm_Contract_Status = data.Contract_Status,
                    slm_Car_Category = data.Car_Category,
                    slm_Customer_Key = data.Customer_Key,
                    slm_TitleId = data.TitleId,                                             //AppUtil.GetTitleId(slmdb, data.Thai_Title_Name);
                    slm_Title_Name_Org = data.Thai_Title_Name,                              //เก็บค่าจาก CMT
                    slm_Name = data.Thai_First_Name,
                    slm_LastName = data.Thai_Last_Name,
                    slm_CitizenId = data.Single_View_Customer,
                    slm_Marital_Status = data.Marital_Status,
                    slm_CardType_Org = data.Customer_Type,                                  //เก็บค่าจาก CMT
                    slm_Tax_Id = data.Tax_ID,
                    slm_OccupationId = data.OccupationId,                                   //AppUtil.GetOccupationId(slmdb, data.Career_Key);
                    slm_Career_Key_Org = data.Career_Key,                                   //เก็บค่าจาก CMT
                    slm_Career_Desc_Org = data.Career_Desc,                                 //เก็บค่าจาก CMT
                    slm_Car_By_Gov_Id = data.InsurancecarTypeId,                            //AppUtil.GetInsuranceCarTypeId(slmdb, data.Car_By_Gov_Name);
                    slm_Car_By_Gov_Name_Org = data.Car_By_Gov_Name,                         //เก็บค่าจาก CMT
                    slm_Brand_Code = brandList.Where(p => p.BrandCode == data.Brand_Code).Count() > 0 ? data.Brand_Code : null,
                    slm_Brand_Code_Org = data.Brand_Code,                                   //เก็บค่าจาก CMT
                    slm_Brand_Name_Org = data.Brand_Name,                                   //เก็บค่าจาก CMT
                    slm_Model_Code = subModelList.Where(p => p.KKKey == data.Model_Code).Select(p => p.ModelCode).FirstOrDefault(), //CMT ส่ง Model_Code คือ KKKey ใน redbook
                    slm_Model_Code_Org = data.Model_Code,                                   //เก็บค่าจาก CMT
                    slm_Model_name_Org = data.Model_Name,                                   //เก็บค่าจาก CMT
                    slm_Engine_No = data.EngineNoUpper,
                    slm_Chassis_No = data.ChassisNoUpper,
                    slm_Model_Year = data.Model_Year,
                    slm_Car_License_No = data.LicenseNoUpper,
                    slm_ProvinceRegis = data.RegisProvinceId,
                    slm_ProvinceRegis_Org = data.Register_Province,                         //เก็บค่าจาก CMT
                    slm_Cc = data.CC != null ? data.CC.Value.ToString() : null,
                    slm_Voluntary_Mkt_Id = data.VoluntaryMktId,                             //AppUtil.GetEmpCode(slmdb, data.Voluntary_MKT_ID);
                    slm_Voluntary_Mkt_Id_Org = data.Voluntary_MKT_ID,                       //เก็บค่าจาก CMT (Voluntary_MKT_ID == marketingCode)
                    slm_Voluntary_Mkt_TitleId = data.VoluntaryMktTitleId,                   //AppUtil.GetTitleId(slmdb, data.Voluntary_MKT_Title);
                    slm_Voluntary_Mkt_Title_Org = data.Voluntary_MKT_Title,                 //เก็บค่าจาก CMT
                    slm_Voluntary_Mkt_First_Name = data.Voluntary_MKT_First_Name,
                    slm_Voluntary_Mkt_Last_Name = data.Voluntary_MKT_Last_Name,
                    slm_Voluntary_Policy_Number = data.Voluntary_Policy_Number,
                    slm_Voluntary_Type_Key = data.Voluntary_Type_Key,
                    slm_Voluntary_Type_Name = data.Voluntary_Type_Name,
                    slm_Voluntary_Policy_Year = data.Voluntary_Policy_Year,
                    slm_Voluntary_Policy_Exp_Year = data.Voluntary_Policy_EXP_YEAR,
                    slm_Voluntary_Policy_Exp_Month = data.Voluntary_Policy_EXP_MONTH,
                    slm_Voluntary_Cov_Amt = data.Voluntary_COV_AMT,
                    slm_Voluntary_Gross_Premium = data.Voluntary_Gross_Premium,
                    slm_Voluntary_Channel_Key = data.Voluntary_Channel_Key,
                    slm_Voluntary_Company_Code = data.Voluntary_Company_Code,
                    slm_Voluntary_Company_Name = data.Voluntary_Company_Name,
                    slm_Benefit_TitleId = data.BenefitTitleId,                              //AppUtil.GetTitleId(slmdb, data.Benefit_Title_Name);
                    slm_Benefit_Title_Name_Org = data.Benefit_Title_Name,                   //เก็บค่าจาก CMT
                    slm_Benefit_First_Name = data.Benefit__First_Name,
                    slm_Benefit_Last_Name = data.Benefit_Last_Name,
                    slm_Benefit_Telno = data.Benefit_TelNo,
                    slm_Driver_TitleId1 = data.DriverTitleId1,                              //AppUtil.GetTitleId(slmdb, data.Driver_Title_Name1);
                    slm_Driver_Title_Name1_Org = data.Driver_Title_Name1,
                    slm_Driver_First_Name1 = data.Driver_First_Name1,
                    slm_Driver_Last_Name1 = data.Driver_Last_Name1,
                    slm_Driver_Telno1 = data.Driver_TelNo1,
                    slm_Driver_TitleId2 = data.DriverTitleId2,                              //AppUtil.GetTitleId(slmdb, data.Driver_Title_Name2);
                    slm_Driver_Title_Name2_Org = data.Driver_Title_Name2,
                    slm_Driver_First_Name2 = data.Driver_First_Name2,
                    slm_Driver_Last_Name2 = data.Driver_Last_Name2,
                    slm_Driver_Telno2 = data.Driver_TelNo2,
                    slm_Compulsory_Policy_Number = data.Compulsory_Policy_Number,
                    slm_Compulsory_Policy_Year = data.Compulsory_Policy_Year,
                    slm_Compulsory_Cov_Amt = data.Compulsory_COV_AMT,
                    slm_Compulsory_Gross_Premium = data.Compulsory_Gross_Premium,
                    slm_Compulsory_Company_Code = data.Compulsory_Company_Code,
                    slm_Compulsory_Company_Name = data.Compulsory_Company_Name,
                    slm_Guarantor_Code1 = data.Guarantor_Code1,
                    slm_Guarantor_TitleId1 = data.GuarantorTitleId1,                        //AppUtil.GetTitleId(slmdb, data.Guarantor_Title_Name1);
                    slm_Guarantor_Title_Name1_Org = data.Guarantor_Title_Name1,
                    slm_Guarantor_First_Name1 = data.Guarantor_First_Name1,
                    slm_Guarantor_Last_Name1 = data.Guarantor_Last_Name1,
                    slm_Guarantor_Card_Id1 = data.Guarantor_Card_ID1,
                    slm_Guarantor_RelationId1 = data.GuarantorRelationId1,                  //AppUtil.GetRelationId(slmdb, data.Guarantor_Relation1);
                    slm_Guarantor_Relation1_Org = data.Guarantor_Relation1,
                    slm_Guarantor_Telno1 = data.Guarantor_TelNo1,
                    slm_Guarantor_Code2 = data.Guarantor_Code2,
                    slm_Guarantor_TitleId2 = data.GuarantorTitleId2,                        //AppUtil.GetTitleId(slmdb, data.Guarantor_Tile_Name2);
                    slm_Guarantor_Title_Name2_Org = data.Guarantor_Title_Name2,
                    slm_Guarantor_First_Name2 = data.Guarantor_First_Name2,
                    slm_Guarantor_Last_Name2 = data.Guarantor_Last_Name2,
                    slm_Guarantor_Card_Id2 = data.Guarantor_Card_ID2,
                    slm_Guarantor_RelationId2 = data.GuarantorRelationId2,                  //AppUtil.GetRelationId(slmdb, data.Guarantor_Relation2);
                    slm_Guarantor_Relation2_Org = data.Guarantor_Relation2,
                    slm_Guarantor_Telno2 = data.Guarantor_TelNo2,
                    slm_Guarantor_Code3 = data.Guarantor_Code3,
                    slm_Guarantor_TitleId3 = data.GuarantorTitleId3,                        //AppUtil.GetTitleId(slmdb, data.Guarantor_Tile_Name3);
                    slm_Guarantor_Title_Name3_Org = data.Guarantor_Title_Name3,
                    slm_Guarantor_First_Name3 = data.Guarantor_First_Name3,
                    slm_Guarantor_Last_Name3 = data.Guarantor_Last_Name3,
                    slm_Guarantor_Card_Id3 = data.Guarantor_Card_ID3,
                    slm_Guarantor_RelationId3 = data.GuarantorRelationId3,                  //AppUtil.GetRelationId(slmdb, data.Guarantor_Relation3);
                    slm_Guarantor_Relation3_Org = data.Guarantor_Relation3,
                    slm_Guarantor_Telno3 = data.Guarantor_TelNo3,
                    slm_Grade = data.Grade,
                    slm_Reference1 = data.Reference1,
                    slm_Reference2 = data.Reference2
                };

                if (data.Customer_Type == "Y")          //บุคคลธรรมดา
                    prelead.slm_CardTypeId = 1;
                else if (data.Customer_Type == "N")     //นิติบุคคล
                    prelead.slm_CardTypeId = 2;

                if (!string.IsNullOrEmpty(data.Birth_Date))
                {
                    DateTime birthDate = AppUtil.ConvertToDateTime("Birth_Date", data.Birth_Date, AppConstant.DateTimeFormat.Format1);
                    if (birthDate.Year != 1 && birthDate.Year != 1900)
                        prelead.slm_Birthdate = birthDate;
                }

                if (!string.IsNullOrEmpty(data.ExpiryDate))
                {
                    DateTime expiryDate = AppUtil.ConvertToDateTime("ExpiryDate", data.ExpiryDate, AppConstant.DateTimeFormat.Format1);
                    if (expiryDate.Year != 1 && expiryDate.Year != 1900)
                        prelead.slm_Expire_Date = expiryDate;
                }

                if (!string.IsNullOrEmpty(data.Voluntary_Policy_EFF_DATE))
                {
                    DateTime policyEffDate = AppUtil.ConvertToDateTime("Voluntary_Policy_EFF_DATE", data.Voluntary_Policy_EFF_DATE, AppConstant.DateTimeFormat.Format1);
                    if (policyEffDate.Year != 1 && policyEffDate.Year != 1900)
                        prelead.slm_Voluntary_Policy_Eff_Date = policyEffDate;
                }

                if (!string.IsNullOrEmpty(data.Voluntary_Policy_EXP_DATE))
                {
                    DateTime policyExpDate = AppUtil.ConvertToDateTime("Voluntary_Policy_EXP_DATE", data.Voluntary_Policy_EXP_DATE, AppConstant.DateTimeFormat.Format1);
                    if (policyExpDate.Year != 1 && policyExpDate.Year != 1900)
                        prelead.slm_Voluntary_Policy_Exp_Date = policyExpDate;
                }

                if (!string.IsNullOrEmpty(data.Voluntary_First_Create_Date))
                {
                    DateTime firstCreateDate = AppUtil.ConvertToDateTime("Voluntary_First_Create_Date", data.Voluntary_First_Create_Date, AppConstant.DateTimeFormat.Format1);
                    if (firstCreateDate.Year != 1 && firstCreateDate.Year != 1900)
                        prelead.slm_Voluntary_First_Create_Date = firstCreateDate;
                }

                if (!string.IsNullOrEmpty(data.Compulsory_Policy_EFF_DATE))
                {
                    DateTime policyEffDate = AppUtil.ConvertToDateTime("Compulsory_Policy_EFF_DATE", data.Compulsory_Policy_EFF_DATE, AppConstant.DateTimeFormat.Format1);
                    if (policyEffDate.Year != 1 && policyEffDate.Year != 1900)
                        prelead.slm_Compulsory_Policy_Eff_Date = policyEffDate;
                }

                if (!string.IsNullOrEmpty(data.Compulsory_Policy_EXP_DATE))
                {
                    DateTime policyExpDate = AppUtil.ConvertToDateTime("Compulsory_Policy_EXP_DATE", data.Compulsory_Policy_EXP_DATE, AppConstant.DateTimeFormat.Format1);
                    if (policyExpDate.Year != 1 && policyExpDate.Year != 1900)
                        prelead.slm_Compulsory_Policy_Exp_Date = policyExpDate;
                }

                if (!string.IsNullOrEmpty(data.Driver_Birth_Date1))
                {
                    DateTime driverBirthdate1 = AppUtil.ConvertToDateTime("Driver_Birth_Date1", data.Driver_Birth_Date1, AppConstant.DateTimeFormat.Format1);
                    if (driverBirthdate1.Year != 1 && driverBirthdate1.Year != 1900)
                        prelead.slm_Driver_Birthdate1 = driverBirthdate1;
                }

                if (!string.IsNullOrEmpty(data.Driver_Birth_Date2))
                {
                    DateTime driverBirthdate2 = AppUtil.ConvertToDateTime("Driver_Birth_Date2", data.Driver_Birth_Date2, AppConstant.DateTimeFormat.Format1);
                    if (driverBirthdate2.Year != 1 && driverBirthdate2.Year != 1900)
                        prelead.slm_Driver_Birthdate2 = driverBirthdate2;
                }

                if (!string.IsNullOrEmpty(data.UpdateDate))
                {
                    DateTime updateDate = AppUtil.ConvertToDateTime("UpdateDate", data.UpdateDate, AppConstant.DateTimeFormat.Format2);
                    if (updateDate.Year != 1 && updateDate.Year != 1900)
                        prelead.slm_View_UpdateDate = updateDate;
                }

                if (!string.IsNullOrEmpty(data.Contract_App_Date))
                {
                    DateTime contractAppDate = AppUtil.ConvertToDateTime("Contract_App_Date", data.Contract_App_Date, AppConstant.DateTimeFormat.Format1);
                    if (contractAppDate.Year != 1 && contractAppDate.Year != 1900)
                        prelead.slm_Contract_App_Date = contractAppDate;
                }

                DateTime createDate = DateTime.Now;
                prelead.slm_CreatedBy = "SYSTEM";
                prelead.slm_CreatedDate = createDate;
                prelead.slm_UpdatedBy = "SYSTEM";
                prelead.slm_UpdatedDate = createDate;
                prelead.slm_ObtPro03Flag = false;
                prelead.slm_PeriodMonth = createDate.Month.ToString("00");
                prelead.slm_PeriodYear = createDate.Year.ToString();
                prelead.is_Deleted = false;

                slmdb.kkslm_tr_prelead.AddObject(prelead);

                kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                {
                    slm_Cmt_Product_Id = data.Cmt_Product_Id,
                    slm_CmtLot = data.CmtLot,
                    slm_NewAssignType = AppConstant.PreleadAssignType.Normal,
                    slm_ContractNo = data.Contract_Number,
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedDate = createDate
                };
                slmdb.kkslm_tr_prelead_assign_log.AddObject(log);

                //decimal?[] tmp = process_list.Select(p => p.Cmt_Product_Id).ToArray();
                //var cmtproduct_list = slmdb.CMT_OBT_PRODUCT.Where(p => tmp.Contains(p.Cmt_Product_Id) == true).ToList();
                //cmtproduct_list.ForEach(p => p.OBT_Flag = true);

                slmdb.SaveChanges();

                DoInsertAddress(slmdb, prelead.slm_Prelead_Id, data.Customer_Key, provinceDataList, data.CmtLot, cmtAddressList, data.Contract_Number);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoInsertAddress(SLMDBEntities slmdb, decimal preleadId, string customerKey, List<ProvinceData> provinceDataList, decimal? lot, List<CMT_OBT_ADDRESS> cmtAddressList, string contractNo)
        {
            try
            {
                //List<CMT_OBT_ADDRESS> list = slmdb.CMT_OBT_ADDRESS.Where(p => p.Customer_Key == customerKey).ToList();

                //List<CMT_OBT_ADDRESS> list = cmtAddressList.Where(p => p.Customer_Key == customerKey).ToList();

                List<CMT_OBT_ADDRESS> list = cmtAddressList.Where(p => p.Contract_No == contractNo).ToList();

                foreach (CMT_OBT_ADDRESS data in list)
                {
                    DateTime createDate = DateTime.Now;
                    kkslm_tr_prelead_address addr = new kkslm_tr_prelead_address()
                    {
                        slm_CmtLot = lot,
                        slm_Prelead_Id = preleadId,
                        slm_Customer_Key = customerKey,
                        slm_Address_Type = data.Address_Type,
                        slm_Contract_Adrress = data.Contract_Address,
                        slm_House_No = data.House_No,
                        slm_Moo = data.Moo,
                        slm_Building = data.Building,
                        slm_Village = data.Village,
                        slm_Soi = data.Soi,
                        slm_Street = data.Street,
                        slm_Province_name_Org = data.Province,
                        slm_Amphur_Name_Org = data.Amphur,
                        slm_Tambon_Name_Org = data.Tambon,
                        slm_Zipcode = data.ZipCode,
                        slm_Home_Phone = data.Home_Phone,
                        slm_Mobile_Phone = data.Mobile_Phone,
                        slm_Contract_No = data.Contract_No,
                        slm_Send_Flag = data.Send_Flag,
                        slm_Receipt_Flag = data.Receipt_Flag,
                        slm_Vat_Flag = data.Vat_Flag,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createDate,
                        is_Deleted = false
                    };

                    var province_list = provinceDataList.Where(p => p.ProvinceName == data.Province && p.AmphurName == data.Amphur && p.TambolName == data.Tambon).FirstOrDefault();
                    if (province_list != null)
                    {
                        addr.slm_Province_Id = province_list.ProvinceId;
                        addr.slm_Amphur_Id = province_list.AmphurId;
                        addr.slm_TambolId = province_list.TambolId;
                    }

                    slmdb.kkslm_tr_prelead_address.AddObject(addr);
                }

                //ถ้า CMT ADDRESS มี AddressType = C, ให้ copy ข้อมูลมา insert ใหม่แล้วใส่ type เป็น D   *outdate logic SR 6001-037 BRN001-05*
                //ถ้า CMT ADDRESS มี SendFlag = true, ให้ copy ข้อมูลมา insert ใหม่แล้วใส่ type เป็น D   *new logic SR 6001-037 BRN001-05*
                var cmtAddress = list.Where(p => p.Send_Flag == true).FirstOrDefault();
                if (cmtAddress != null)
                {
                    DateTime createDate = DateTime.Now;
                    kkslm_tr_prelead_address addr = new kkslm_tr_prelead_address()
                    {
                        slm_CmtLot = lot,
                        slm_Prelead_Id = preleadId,
                        slm_Customer_Key = customerKey,
                        slm_Address_Type = "D",
                        slm_Contract_Adrress = cmtAddress.Contract_Address,
                        slm_House_No = cmtAddress.House_No,
                        slm_Moo = cmtAddress.Moo,
                        slm_Building = cmtAddress.Building,
                        slm_Village = cmtAddress.Village,
                        slm_Soi = cmtAddress.Soi,
                        slm_Street = cmtAddress.Street,
                        slm_Province_name_Org = cmtAddress.Province,
                        slm_Amphur_Name_Org = cmtAddress.Amphur,
                        slm_Tambon_Name_Org = cmtAddress.Tambon,
                        slm_Zipcode = cmtAddress.ZipCode,
                        slm_Home_Phone = cmtAddress.Home_Phone,
                        slm_Mobile_Phone = cmtAddress.Mobile_Phone,
                        slm_Contract_No = cmtAddress.Contract_No,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = createDate,
                        slm_UpdatedBy = "SYSTEM",
                        slm_UpdatedDate = createDate,
                        is_Deleted = false
                    };
                    var province_list = provinceDataList.Where(p => p.ProvinceName == cmtAddress.Province && p.AmphurName == cmtAddress.Amphur && p.TambolName == cmtAddress.Tambon).FirstOrDefault();
                    if (province_list != null)
                    {
                        addr.slm_Province_Id = province_list.ProvinceId;
                        addr.slm_Amphur_Id = province_list.AmphurId;
                        addr.slm_TambolId = province_list.TambolId;
                    }

                    slmdb.kkslm_tr_prelead_address.AddObject(addr);
                }

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                string errorDetail = "Insert to kkslm_tr_prelead_address failed, Customer Key " + customerKey + ", Error=" + message;
                throw new Exception(errorDetail);
            }
        }

        private void DoInsertMasterDedup(SLMDBEntities slmdb, CmtObtProductData data, List<kkslm_ms_config_product_day> configProductDayList)
        {
            try
            {
                cmtProductId = data.Cmt_Product_Id != null ? data.Cmt_Product_Id.Value.ToString() : "";

                DateTime createDate = DateTime.Now;
                int? day = configProductDayList.Where(p => p.slm_Product_Id == data.ProductId && p.slm_Type == "DEDUP").Select(p => p.slm_Days).FirstOrDefault();

                //int? day = slmdb.kkslm_ms_config_product_day.Where(p => p.slm_Product_Id == data.ProductId && p.slm_Type == "DEDUP").Select(p => p.slm_Days).FirstOrDefault();

                kkslm_ms_config_product_dedup config = new kkslm_ms_config_product_dedup()
                {
                    slm_CmtLot = data.CmtLot,
                    slm_Product_Id = data.ProductId,
                    slm_Engine_No = data.EngineNoUpper,
                    slm_Chassis_No = data.ChassisNoUpper,
                    slm_StartDate = createDate,
                    is_Deleted = false,
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedDate = createDate,
                    slm_UpdatedBy = "SYSTEM",
                    slm_UpdatedDate = createDate
                };

                if (day != null)
                    config.slm_EndDate = createDate.AddDays(day.Value);

                slmdb.kkslm_ms_config_product_dedup.AddObject(config);
                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoInsertTempForceDedup(SLMDBEntities slmdb, List<CmtObtProductData> list, List<BrandData> brandList, List<SubModelData> subModelList)
        {
            try
            {
                foreach (CmtObtProductData data in list)
                {
                    cmtProductId = data.Cmt_Product_Id != null ? data.Cmt_Product_Id.Value.ToString() : "";
                    slmdb.kkslm_tr_prelead_temp.AddObject(SetTempPrelead(data, AppConstant.PreleadAssignType.Dedup, brandList, subModelList));

                    kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                    {
                        slm_Cmt_Product_Id = data.Cmt_Product_Id,
                        slm_CmtLot = data.CmtLot,
                        slm_NewAssignType = AppConstant.PreleadAssignType.Dedup,
                        slm_ContractNo = data.Contract_Number,
                        slm_CreatedBy = "SYSTEM",
                        slm_CreatedDate = DateTime.Now
                    };
                    slmdb.kkslm_tr_prelead_assign_log.AddObject(log);
                }

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoInsertPortMonitor(string batchCode, decimal? lot, long batchMonitorId)
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                int success = slmdb.kkslm_tr_prelead.Where(p => p.slm_CmtLot == lot && p.is_Deleted == false).Count();
                int dedup = slmdb.kkslm_tr_prelead_temp.Where(p => p.slm_CmtLot == lot && p.slm_Type == AppConstant.PreleadAssignType.Dedup && p.is_Deleted == false).Count();
                int blacklist = slmdb.kkslm_tr_prelead_temp.Where(p => p.slm_CmtLot == lot && p.slm_Type == AppConstant.PreleadAssignType.Blacklist && p.is_Deleted == false).Count();

                DateTime createDate = DateTime.Now;
                kkslm_tr_preleadportmonitor port = new kkslm_tr_preleadportmonitor()
                {
                    slm_CMTLot = lot,
                    slm_TotalPort = success + dedup + blacklist,
                    slm_Success = success,
                    slm_DeDup = dedup,
                    slm_Blaklist = blacklist,
                    slm_Exceptional = 0,
                    slm_PortStatus = "W",
                    slm_PeriodMonth = createDate.Month.ToString("00"),
                    slm_PeriodYear = createDate.Year.ToString(),
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedDate = createDate,
                    slm_UpdatedBy = "SYSTEM",
                    slm_UpdatedDate = createDate,
                    is_Deleted = false
                };
                slmdb.kkslm_tr_preleadportmonitor.AddObject(port);

                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                string errorDetail = "Cannot insert port monitor, Error=" + message;
                Util.WriteLogFile(logfilename, batchCode, errorDetail);
                BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
            }
        }

        /// <summary>
        /// Check กรณี มีการติ๊ก Force Blacklist มาจากหน้าจอ, Batch ต้องทำการเช็กให้ว่า Force Blacklist นั้น ติด Dedup ต่อหรือไม่
        /// ถ้าติด Dedup เปลี่ยนเป็น Temp Dedup, ถ้าไม่ติด ไม่ต้องทำอะไร รอให้ rule มา force กลับไปเป็น prelead
        /// </summary>
        /// <param name="batchCode"></param>
        public void CheckForceBlacklistToDedup(long batchMonitorId, string batchCode)
        {
            try
            {
                using (SLMDBEntities slmdb = AppUtil.GetSlmDbEntities())
                {
                    List<decimal?> lot_list = new List<decimal?>();
                    int count = 0;

                    var bl_list = slmdb.kkslm_tr_prelead_temp.Where(p => p.slm_Type == AppConstant.PreleadAssignType.Blacklist && p.slm_IsForce == true && p.is_Deleted == false).ToList();
                    if (bl_list.Count > 0)
                    {
                        var dedupMasterList = GetDedupMasterList(slmdb);
                        DateTime updateDate = DateTime.Now;

                        foreach (kkslm_tr_prelead_temp data in bl_list)
                        {
                            count = dedupMasterList.Where(d => d.ProductId == data.slm_Product_Id && d.ChassisNoCleaned == StringRemoveSpecialChars(data.slm_Chassis_No) && d.EngineNo == data.slm_Engine_No).Count();
                            if (count > 0)
                            {
                                data.slm_Type = AppConstant.PreleadAssignType.Dedup;
                                data.slm_IsForce = false;
                                data.slm_UpdatedBy = "SYSTEM";
                                data.slm_UpdatedDate = updateDate;

                                if (!lot_list.Contains(data.slm_CmtLot))
                                {
                                    lot_list.Add(data.slm_CmtLot);
                                }

                                //Add to log
                                kkslm_tr_prelead_assign_log log = new kkslm_tr_prelead_assign_log()
                                {
                                    slm_Cmt_Product_Id = data.slm_Cmt_Product_Id,
                                    slm_CmtLot = data.slm_CmtLot,
                                    slm_OldAssignType = AppConstant.PreleadAssignType.BlacklistForce,
                                    slm_NewAssignType = AppConstant.PreleadAssignType.Dedup,
                                    slm_ContractNo = data.slm_Contract_Number,
                                    slm_CreatedDate = DateTime.Now,
                                    slm_CreatedBy = "SYSTEM"
                                };
                                slmdb.kkslm_tr_prelead_assign_log.AddObject(log);
                            }
                        }

                        slmdb.SaveChanges();

                        var prelead_temp = slmdb.kkslm_tr_prelead_temp.Where(p => p.is_Deleted == false).ToList();
                        foreach (decimal? lot in lot_list)
                        {
                            int success = slmdb.kkslm_tr_prelead.Where(p => p.slm_CmtLot == lot && p.is_Deleted == false).Count();
                            int dedup = prelead_temp.Where(p => p.slm_CmtLot == lot && p.slm_Type == AppConstant.PreleadAssignType.Dedup && p.is_Deleted == false).Count();
                            int blacklist = prelead_temp.Where(p => p.slm_CmtLot == lot && p.slm_Type == AppConstant.PreleadAssignType.Blacklist && p.is_Deleted == false).Count();

                            var port_monitor = slmdb.kkslm_tr_preleadportmonitor.Where(p => p.slm_CMTLot == lot).FirstOrDefault();
                            if (port_monitor != null)
                            {
                                port_monitor.slm_Success = success;
                                port_monitor.slm_DeDup = dedup;
                                port_monitor.slm_Blaklist = blacklist;
                                port_monitor.slm_TotalPort = success + dedup + blacklist + (port_monitor.slm_Exceptional != null ? port_monitor.slm_Exceptional : 0);
                                port_monitor.slm_UpdatedBy = "SYSTEM";
                                port_monitor.slm_UpdatedDate = DateTime.Now;
                            }
                        }

                        slmdb.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                string errorDetail = "Cannot check blacklist force to dedup, Error=" + message;
                Util.WriteLogFile(logfilename, batchCode, errorDetail);
                BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
            }
        }

        public void DeletePreleadAssignLog(long batchMonitorId, string batchCode)
        {
            try
            {
                using (SLMDBEntities slmdb = AppUtil.GetSlmDbEntities())
                {
                    int numOfDay;
                    var tmp = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "prelead_assign_log_day" && p.is_Deleted == 0).Select(p => p.slm_OptionDesc).FirstOrDefault();
                    if (!int.TryParse(tmp, out numOfDay))
                    {
                        numOfDay = 120;
                    }

                    DateTime date = DateTime.Now.AddDays(-numOfDay);
                    var strDate = date.Year.ToString() + date.ToString("-MM-dd");
                    string sql = $"DELETE FROM kkslm_tr_prelead_assign_log WHERE slm_CreatedDate < '{strDate}' ";
                    slmdb.ExecuteStoreCommand(sql);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                string errorDetail = "Purge data in kkslm_tr_prelead_assign_log failed, Error=" + message;
                Util.WriteLogFile(logfilename, batchCode, errorDetail);
                BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
            }
        }

        private string StringRemoveSpecialChars(string value)
        {
            StringBuilder sb = new StringBuilder(value);
            sb.Replace(" ", "");
            sb.Replace("-", "");
            sb.Replace("*", "");
            sb.Replace("_", "");
            sb.Replace("\\", "");
            sb.Replace("/", "");
            return sb.ToString();
        }

        #region Backup

        //public void InsertPreLead(string batchCode)
        //{
        //    //รอเทส SIT2

        //    Int64 batchMonitorId = 0;
        //    int totalRecord = 0;
        //    int totalSuccess = 0;
        //    int totalFail = 0;

        //    string productId = "";
        //    string citizenId = "";
        //    string licenseNo = "";
        //    string engineNo = "";
        //    string chassisNo = "";
        //    List<CmtObtProductData> process_list = new List<CmtObtProductData>();
        //    CmtObtProductData data = null;
        //    List<ProvinceData> provinceDataList = new List<ProvinceData>();
        //    List<BrandData> brandList = new List<BrandData>();
        //    List<ModelData> modelList = new List<ModelData>();

        //    try
        //    {
        //        batchMonitorId = BizUtil.SetStartTime(batchCode);
        //        List<CmtObtProductData> cmt_mainlist = GetCmtObtProduct();  //Get ข้อมูลที่มาจาก CMT
        //        totalRecord = cmt_mainlist.Count;

        //        if (cmt_mainlist.Count > 0)
        //        {
        //            //Get data for use in all process
        //            SLMDBEntities db = AppUtil.GetSlmDbEntities();
        //            provinceDataList = AppUtil.GetProvinceDataList(db);
        //            brandList = AppUtil.GetRedbookBrandList(db);
        //            modelList = AppUtil.GetRedbookModelList(db);

        //            //1.Insert CMT data ที่ติด blacklist ใน table temp blacklist, function return list ของ cmtId ที่ติด blacklist
        //            List<decimal?> blaklist_cmtIds = DoProcessBlacklist(brandList, modelList);
        //            totalSuccess += blaklist_cmtIds.Count;

        //            //2.นำ cmtId ที่ติด blacklist ออกจาก cmt_mainlist
        //            cmt_mainlist = cmt_mainlist.Where(p => blaklist_cmtIds.Contains(p.Cmt_Product_Id) == false).ToList();

        //            //3.นำ cmt_mainlist มา order ตาม field ที่ใช้เช็ก dedup
        //            cmt_mainlist = cmt_mainlist.OrderBy(p => p.ProductId).ThenBy(p => p.Single_View_Customer).ThenBy(p => p.LicenseNoUpper).ThenBy(p => p.EngineNoUpper).ThenBy(p => p.ChassisNoUpper).ThenByDescending(p => p.Contract_Year).ToList();

        //            var dedupMasterList = GetDedupMasterList();

        //            while (cmt_mainlist.Count > 0)
        //            {
        //                try
        //                {
        //                    SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();

        //                    data = cmt_mainlist[0];     //นำ record บนสุดมาเช็ก

        //                    productId = data.ProductId;
        //                    citizenId = data.Single_View_Customer;
        //                    licenseNo = data.LicenseNoUpper;
        //                    engineNo = data.EngineNoUpper;
        //                    chassisNo = data.ChassisNoUpper;

        //                    process_list = cmt_mainlist.Where(p => p.ProductId == productId && p.Single_View_Customer == citizenId && p.LicenseNoUpper == licenseNo && p.EngineNoUpper == engineNo && p.ChassisNoUpper == chassisNo).ToList();
        //                    if (process_list.Count > 1)     //ถ้าข้อมูล CMT มาซ้ำ
        //                    {
        //                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
        //                        {
        //                            //เอา record แรกไปเช็กได้เลย เนื่องจาก 5 field ที่ where จะเหมือนกันทุก record
        //                            int count = dedupMasterList.Where(p => p.ProductId == process_list[0].ProductId && p.CitizenId == process_list[0].Single_View_Customer && p.LicenseNo == process_list[0].LicenseNoUpper && p.EngineNo == process_list[0].EngineNoUpper && p.ChassisNo == process_list[0].ChassisNoUpper).Count();
        //                            if (count == 0)     //ไม่ติด Dedup
        //                            {
        //                                //1.เลือก row ล่าสุดไป Insert prelead
        //                                var insertData = process_list.OrderByDescending(p => p.Contract_Year).FirstOrDefault();
        //                                DoInsertPrelead(slmdb, insertData, process_list, provinceDataList, brandList, modelList);

        //                                //2.เลือก row ล่าสุดไป Insert Master Dedup
        //                                DoInsertMasterDedup(slmdb, insertData);
        //                            }
        //                            else
        //                            {
        //                                //1.นำ row ทั้งหมดไป Insert ลง Temp Force Dedup
        //                                DoInsertTempForceDedup(slmdb, process_list, brandList, modelList);
        //                            }

        //                            ts.Complete();
        //                        }
        //                    }
        //                    else if (process_list.Count == 1)   //ข้อมูล CMT ไม่ซ้ำ
        //                    {
        //                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
        //                        {
        //                            int count = dedupMasterList.Where(p => p.ProductId == process_list[0].ProductId && p.CitizenId == process_list[0].Single_View_Customer && p.LicenseNo == process_list[0].LicenseNoUpper && p.EngineNo == process_list[0].EngineNoUpper && p.ChassisNo == process_list[0].ChassisNoUpper).Count();
        //                            if (count == 0)     //ไม่ติด Dedup
        //                            {
        //                                //1.Insert prelead
        //                                DoInsertPrelead(slmdb, process_list[0], process_list, provinceDataList, brandList, modelList);

        //                                //2.Insert Master Dedup
        //                                DoInsertMasterDedup(slmdb, process_list[0]);
        //                            }
        //                            else
        //                            {
        //                                //Insert ลง Temp Force Dedup
        //                                DoInsertTempForceDedup(slmdb, process_list, brandList, modelList);
        //                            }

        //                            ts.Complete();
        //                        }
        //                    }

        //                    totalSuccess += process_list.Count;
        //                    Console.WriteLine("Cmt_Product_Id " + data.Cmt_Product_Id.ToString() + ": SUCCESS");
        //                }
        //                catch (Exception ex)
        //                {
        //                    totalFail += process_list.Count;
        //                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                    string errorDetail = "Cmt_Product_Id=" + data.Cmt_Product_Id.ToString() + ", Error=" + message;

        //                    BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
        //                    Util.WriteLogFile(logfilename, batchCode, errorDetail);

        //                    Console.WriteLine("Cmt_Product_Id " + data.Cmt_Product_Id.ToString() + ": FAIL");
        //                }

        //                cmt_mainlist = cmt_mainlist.Except(process_list).ToList();  //นำ list ที่ทำการ process แล้ว ออกจาก cmt_mainlist หลัก
        //            }

        //            DoInsertPortMonitor(batchCode, _cmt_lot);
        //        }
        //        else
        //        {
        //            totalSuccess = 0;
        //            totalFail = 0;
        //        }

        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("All FAIL");
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

        //        Util.WriteLogFile(logfilename, batchCode, message);
        //        BizUtil.InsertLog(batchMonitorId, "", "", message);
        //        BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
        //        throw ex;
        //    }
        //}

        //private void DoInsertPrelead(SLMDBEntities slmdb, CmtObtProductData data, List<CmtObtProductData> process_list, List<ProvinceData> provinceDataList, List<BrandData> brandList, List<ModelData> modelList)
        //{
        //    try
        //    {
        //        kkslm_tr_prelead prelead = new kkslm_tr_prelead();
        //        prelead.slm_Contract_Number = data.Contract_Number;
        //        prelead.slm_CampaignId = data.Campaign_Id;

        //        if (string.IsNullOrEmpty(data.ProductId))
        //            throw new Exception("ไม่พบ ProductId สำหรับ CampaignId " + data.Campaign_Id);
        //        else
        //            prelead.slm_Product_Id = data.ProductId;

        //        prelead.slm_BranchCode = data.Branch;
        //        prelead.slm_Contract_Year = data.Contract_Year;
        //        prelead.slm_Contract_Status = data.Contract_Status;
        //        prelead.slm_Car_Category = data.Car_Category;
        //        prelead.slm_Customer_Key = data.Customer_Key;
        //        prelead.slm_TitleId = data.TitleId;     //AppUtil.GetTitleId(slmdb, data.Thai_Title_Name);
        //        prelead.slm_Title_Name_Org = data.Thai_Title_Name;                              //เก็บค่าจาก CMT
        //        prelead.slm_Name = data.Thai_First_Name;
        //        prelead.slm_LastName = data.Thai_Last_Name;
        //        prelead.slm_CitizenId = data.Single_View_Customer;
        //        prelead.slm_Marital_Status = data.Marital_Status;

        //        if (data.Customer_Type == "Y")          //บุคคลธรรมดา
        //            prelead.slm_CardTypeId = 1;
        //        else if (data.Customer_Type == "N")     //นิติบุคคล
        //            prelead.slm_CardTypeId = 2;

        //        prelead.slm_CardType_Org = data.Customer_Type;                                  //เก็บค่าจาก CMT

        //        if (!string.IsNullOrEmpty(data.Birth_Date))
        //        {
        //            DateTime birthDate = AppUtil.ConvertToDateTime("Birth_Date", data.Birth_Date, AppConstant.DateTimeFormat.Format1);
        //            if (birthDate.Year != 1 && birthDate.Year != 1900)
        //                prelead.slm_Birthdate = birthDate;
        //        }

        //        prelead.slm_Tax_Id = data.Tax_ID;
        //        prelead.slm_OccupationId = data.OccupationId;   //AppUtil.GetOccupationId(slmdb, data.Career_Key);
        //        prelead.slm_Career_Key_Org = data.Career_Key;                                   //เก็บค่าจาก CMT
        //        prelead.slm_Career_Desc_Org = data.Career_Desc;                                 //เก็บค่าจาก CMT
        //        prelead.slm_Car_By_Gov_Id = data.InsurancecarTypeId;    //AppUtil.GetInsuranceCarTypeId(slmdb, data.Car_By_Gov_Name);
        //        prelead.slm_Car_By_Gov_Name_Org = data.Car_By_Gov_Name;                         //เก็บค่าจาก CMT

        //        //if (AppUtil.CheckBrandCodeExist(slmdb, data.Brand_Code))
        //        //    prelead.slm_Brand_Code = data.Brand_Code;

        //        if (brandList.Where(p => p.BrandCode == data.Brand_Code).Count() > 0)
        //            prelead.slm_Brand_Code = data.Brand_Code;

        //        prelead.slm_Brand_Code_Org = data.Brand_Code;                                   //เก็บค่าจาก CMT
        //        prelead.slm_Brand_Name_Org = data.Brand_Name;                                   //เก็บค่าจาก CMT

        //        //if (AppUtil.CheckModelCodeExist(slmdb, data.Brand_Code, data.Model_Code))
        //        //    prelead.slm_Model_Code = data.Model_Code;

        //        if (modelList.Where(p => p.BrandCode == data.Brand_Code && p.ModelCode == data.Model_Code).Count() > 0)
        //            prelead.slm_Model_Code = data.Model_Code;

        //        prelead.slm_Model_Code_Org = data.Model_Code;                                   //เก็บค่าจาก CMT
        //        prelead.slm_Model_name_Org = data.Model_Name;                                   //เก็บค่าจาก CMT

        //        prelead.slm_Engine_No = data.EngineNoUpper;
        //        prelead.slm_Chassis_No = data.ChassisNoUpper;
        //        prelead.slm_Model_Year = data.Model_Year;
        //        prelead.slm_Car_License_No = data.LicenseNoUpper;

        //        //var provinceRegis = AppUtil.GetProvince(slmdb, data.Register_Province);
        //        //if (provinceRegis != null)
        //        prelead.slm_ProvinceRegis = data.RegisProvinceId;

        //        prelead.slm_ProvinceRegis_Org = data.Register_Province;                         //เก็บค่าจาก CMT
        //        prelead.slm_Cc = data.CC != null ? data.CC.Value.ToString() : null;

        //        if (!string.IsNullOrEmpty(data.ExpiryDate))
        //        {
        //            DateTime expiryDate = AppUtil.ConvertToDateTime("ExpiryDate", data.ExpiryDate, AppConstant.DateTimeFormat.Format1);
        //            if (expiryDate.Year != 1 && expiryDate.Year != 1900)
        //                prelead.slm_Expire_Date = expiryDate;
        //        }

        //        prelead.slm_Voluntary_Mkt_Id = data.VoluntaryMktId;     //AppUtil.GetEmpCode(slmdb, data.Voluntary_MKT_ID);
        //        prelead.slm_Voluntary_Mkt_Id_Org = data.Voluntary_MKT_ID;                       //เก็บค่าจาก CMT (Voluntary_MKT_ID == marketingCode)
        //        prelead.slm_Voluntary_Mkt_TitleId = data.VoluntaryMktTitleId;   //AppUtil.GetTitleId(slmdb, data.Voluntary_MKT_Title);
        //        prelead.slm_Voluntary_Mkt_Title_Org = data.Voluntary_MKT_Title;                 //เก็บค่าจาก CMT
        //        prelead.slm_Voluntary_Mkt_First_Name = data.Voluntary_MKT_First_Name;
        //        prelead.slm_Voluntary_Mkt_Last_Name = data.Voluntary_MKT_Last_Name;
        //        prelead.slm_Voluntary_Policy_Number = data.Voluntary_Policy_Number;
        //        prelead.slm_Voluntary_Type_Key = data.Voluntary_Type_Key;
        //        prelead.slm_Voluntary_Type_Name = data.Voluntary_Type_Name;
        //        prelead.slm_Voluntary_Policy_Year = data.Voluntary_Policy_Year;

        //        if (!string.IsNullOrEmpty(data.Voluntary_Policy_EFF_DATE))
        //        {
        //            DateTime policyEffDate = AppUtil.ConvertToDateTime("Voluntary_Policy_EFF_DATE", data.Voluntary_Policy_EFF_DATE, AppConstant.DateTimeFormat.Format1);
        //            if (policyEffDate.Year != 1 && policyEffDate.Year != 1900)
        //                prelead.slm_Voluntary_Policy_Eff_Date = policyEffDate;
        //        }

        //        if (!string.IsNullOrEmpty(data.Voluntary_Policy_EXP_DATE))
        //        {
        //            DateTime policyExpDate = AppUtil.ConvertToDateTime("Voluntary_Policy_EXP_DATE", data.Voluntary_Policy_EXP_DATE, AppConstant.DateTimeFormat.Format1);
        //            if (policyExpDate.Year != 1 && policyExpDate.Year != 1900)
        //                prelead.slm_Voluntary_Policy_Exp_Date = policyExpDate;
        //        }

        //        prelead.slm_Voluntary_Policy_Exp_Year = data.Voluntary_Policy_EXP_YEAR;
        //        prelead.slm_Voluntary_Policy_Exp_Month = data.Voluntary_Policy_EXP_MONTH;
        //        prelead.slm_Voluntary_Cov_Amt = data.Voluntary_COV_AMT;
        //        prelead.slm_Voluntary_Gross_Premium = data.Voluntary_Gross_Premium;
        //        prelead.slm_Voluntary_Channel_Key = data.Voluntary_Channel_Key;
        //        prelead.slm_Voluntary_Company_Code = data.Voluntary_Company_Code;
        //        prelead.slm_Voluntary_Company_Name = data.Voluntary_Company_Name;
        //        prelead.slm_Benefit_TitleId = data.BenefitTitleId;      //AppUtil.GetTitleId(slmdb, data.Benefit_Title_Name);
        //        prelead.slm_Benefit_Title_Name_Org = data.Benefit_Title_Name;                           //เก็บค่าจาก CMT
        //        prelead.slm_Benefit_First_Name = data.Benefit__First_Name;
        //        prelead.slm_Benefit_Last_Name = data.Benefit_Last_Name;
        //        prelead.slm_Benefit_Telno = data.Benefit_TelNo;
        //        prelead.slm_Driver_TitleId1 = data.DriverTitleId1;  //AppUtil.GetTitleId(slmdb, data.Driver_Title_Name1);
        //        prelead.slm_Driver_Title_Name1_Org = data.Driver_Title_Name1;
        //        prelead.slm_Driver_First_Name1 = data.Driver_First_Name1;
        //        prelead.slm_Driver_Last_Name1 = data.Driver_Last_Name1;
        //        prelead.slm_Driver_Telno1 = data.Driver_TelNo1;
        //        prelead.slm_Driver_TitleId2 = data.DriverTitleId2;  //AppUtil.GetTitleId(slmdb, data.Driver_Title_Name2);
        //        prelead.slm_Driver_Title_Name2_Org = data.Driver_Title_Name2;
        //        prelead.slm_Driver_First_Name2 = data.Driver_First_Name2;
        //        prelead.slm_Driver_Last_Name2 = data.Driver_Last_Name2;
        //        prelead.slm_Driver_Telno2 = data.Driver_TelNo2;
        //        prelead.slm_Compulsory_Policy_Number = data.Compulsory_Policy_Number;
        //        prelead.slm_Compulsory_Policy_Year = data.Compulsory_Policy_Year;

        //        if (!string.IsNullOrEmpty(data.Compulsory_Policy_EFF_DATE))
        //        {
        //            DateTime policyEffDate = AppUtil.ConvertToDateTime("Compulsory_Policy_EFF_DATE", data.Compulsory_Policy_EFF_DATE, AppConstant.DateTimeFormat.Format1);
        //            if (policyEffDate.Year != 1 && policyEffDate.Year != 1900)
        //                prelead.slm_Compulsory_Policy_Eff_Date = policyEffDate;
        //        }

        //        if (!string.IsNullOrEmpty(data.Compulsory_Policy_EXP_DATE))
        //        {
        //            DateTime policyExpDate = AppUtil.ConvertToDateTime("Compulsory_Policy_EXP_DATE", data.Compulsory_Policy_EXP_DATE, AppConstant.DateTimeFormat.Format1);
        //            if (policyExpDate.Year != 1 && policyExpDate.Year != 1900)
        //                prelead.slm_Compulsory_Policy_Exp_Date = policyExpDate;
        //        }

        //        prelead.slm_Compulsory_Cov_Amt = data.Compulsory_COV_AMT;
        //        prelead.slm_Compulsory_Gross_Premium = data.Compulsory_Gross_Premium;
        //        prelead.slm_Compulsory_Company_Code = data.Compulsory_Company_Code;
        //        prelead.slm_Compulsory_Company_Name = data.Compulsory_Company_Name;

        //        prelead.slm_Guarantor_Code1 = data.Guarantor_Code1;
        //        prelead.slm_Guarantor_TitleId1 = data.GuarantorTitleId1;    //AppUtil.GetTitleId(slmdb, data.Guarantor_Title_Name1);
        //        prelead.slm_Guarantor_Title_Name1_Org = data.Guarantor_Title_Name1;
        //        prelead.slm_Guarantor_First_Name1 = data.Guarantor_First_Name1;
        //        prelead.slm_Guarantor_Last_Name1 = data.Guarantor_Last_Name1;
        //        prelead.slm_Guarantor_Card_Id1 = data.Guarantor_Card_ID1;
        //        prelead.slm_Guarantor_RelationId1 = data.GuarantorRelationId1;  //AppUtil.GetRelationId(slmdb, data.Guarantor_Relation1);
        //        prelead.slm_Guarantor_Relation1_Org = data.Guarantor_Relation1;
        //        prelead.slm_Guarantor_Telno1 = data.Guarantor_TelNo1;

        //        prelead.slm_Guarantor_Code2 = data.Guarantor_Code2;
        //        prelead.slm_Guarantor_TitleId2 = data.GuarantorTitleId2;  //AppUtil.GetTitleId(slmdb, data.Guarantor_Tile_Name2);
        //        prelead.slm_Guarantor_Title_Name2_Org = data.Guarantor_Title_Name2;
        //        prelead.slm_Guarantor_First_Name2 = data.Guarantor_First_Name2;
        //        prelead.slm_Guarantor_Last_Name2 = data.Guarantor_Last_Name2;
        //        prelead.slm_Guarantor_Card_Id2 = data.Guarantor_Card_ID2;
        //        prelead.slm_Guarantor_RelationId2 = data.GuarantorRelationId2;  //AppUtil.GetRelationId(slmdb, data.Guarantor_Relation2);
        //        prelead.slm_Guarantor_Relation2_Org = data.Guarantor_Relation2;
        //        prelead.slm_Guarantor_Telno2 = data.Guarantor_TelNo2;

        //        prelead.slm_Guarantor_Code3 = data.Guarantor_Code3;
        //        prelead.slm_Guarantor_TitleId3 = data.GuarantorTitleId3;    //AppUtil.GetTitleId(slmdb, data.Guarantor_Tile_Name3);
        //        prelead.slm_Guarantor_Title_Name3_Org = data.Guarantor_Title_Name3;
        //        prelead.slm_Guarantor_First_Name3 = data.Guarantor_First_Name3;
        //        prelead.slm_Guarantor_Last_Name3 = data.Guarantor_Last_Name3;
        //        prelead.slm_Guarantor_Card_Id3 = data.Guarantor_Card_ID3;
        //        prelead.slm_Guarantor_RelationId3 = data.GuarantorRelationId3;  //AppUtil.GetRelationId(slmdb, data.Guarantor_Relation3);
        //        prelead.slm_Guarantor_Relation3_Org = data.Guarantor_Relation3;
        //        prelead.slm_Guarantor_Telno3 = data.Guarantor_TelNo3;

        //        prelead.slm_Grade = data.Grade;

        //        if (!string.IsNullOrEmpty(data.UpdateDate))
        //        {
        //            DateTime updateDate = AppUtil.ConvertToDateTime("UpdateDate", data.UpdateDate, AppConstant.DateTimeFormat.Format2);
        //            if (updateDate.Year != 1 && updateDate.Year != 1900)
        //                prelead.slm_Compulsory_Policy_Exp_Date = updateDate;
        //        }

        //        DateTime createDate = DateTime.Now;
        //        prelead.slm_CreatedBy = "SYSTEM";
        //        prelead.slm_CreatedDate = createDate;
        //        prelead.slm_UpdatedBy = "SYSTEM";
        //        prelead.slm_UpdatedDate = createDate;
        //        prelead.slm_ObtPro03Flag = false;
        //        prelead.is_Deleted = false;

        //        slmdb.kkslm_tr_prelead.AddObject(prelead);

        //        decimal?[] tmp = process_list.Select(p => p.Cmt_Product_Id).ToArray();
        //        var cmtproduct_list = slmdb.CMT_OBT_PRODUCT.Where(p => tmp.Contains(p.Cmt_Product_Id) == true).ToList();
        //        cmtproduct_list.ForEach(p => p.OBT_Flag = true);

        //        slmdb.SaveChanges();

        //        DoInsertAddress(slmdb, prelead.slm_Prelead_Id, data.Customer_Key, provinceDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void DoInsertPrelead(SLMDBEntities slmdb, CMT_OBT_PRODUCT data, long batchMonitorId, string batchCode)
        //{
        //    try
        //    {
        //        kkslm_tr_prelead prelead = new kkslm_tr_prelead();
        //        prelead.slm_Contract_Number = data.Contract_Number;
        //        prelead.slm_CampaignId = data.Campaign_Id;

        //        string productId = AppUtil.GetProductId(slmdb, data.Campaign_Id);
        //        if (string.IsNullOrEmpty(productId))
        //            throw new Exception("ไม่พบ ProductId สำหรับ CampaignId " + data.Campaign_Id);
        //        else
        //            prelead.slm_Product_Id = productId;

        //        prelead.slm_BranchCode = data.Branch;
        //        prelead.slm_Contract_Year = data.Contract_Year;
        //        prelead.slm_Contract_Status = data.Contract_Status;
        //        prelead.slm_Car_Category = data.Car_Category;
        //        prelead.slm_Customer_Key = data.Customer_Key;
        //        prelead.slm_TitleId = AppUtil.GetTitleId(slmdb, data.Thai_Title_Name);
        //        prelead.slm_Title_Name_Org = data.Thai_Title_Name;                              //เก็บค่าจาก CMT
        //        prelead.slm_Name = data.Thai_First_Name;
        //        prelead.slm_LastName = data.Thai_Last_Name;
        //        prelead.slm_CitizenId = data.Single_View_Customer;
        //        prelead.slm_Marital_Status = data.Marital_Status;

        //        if (data.Customer_Type == "Y")          //บุคคลธรรมดา
        //            prelead.slm_CardTypeId = 1;
        //        else if (data.Customer_Type == "N")     //นิติบุคคล
        //            prelead.slm_CardTypeId = 2;

        //        prelead.slm_CardType_Org = data.Customer_Type;                                  //เก็บค่าจาก CMT

        //        if (!string.IsNullOrEmpty(data.Birth_Date))
        //        {
        //            DateTime birthDate = AppUtil.ConvertToDateTime("Birth_Date", data.Birth_Date, AppConstant.DateTimeFormat.Format1);
        //            if (birthDate.Year != 1 && birthDate.Year != 1900)
        //                prelead.slm_Birthdate = birthDate;
        //        }

        //        prelead.slm_Tax_Id = data.Tax_ID;
        //        prelead.slm_OccupationId = AppUtil.GetOccupationId(slmdb, data.Career_Key);
        //        prelead.slm_Career_Key_Org = data.Career_Key;                                   //เก็บค่าจาก CMT
        //        prelead.slm_Career_Desc_Org = data.Career_Desc;                                 //เก็บค่าจาก CMT
        //        prelead.slm_Car_By_Gov_Id = AppUtil.GetInsuranceCarTypeId(slmdb, data.Car_By_Gov_Name);
        //        prelead.slm_Car_By_Gov_Name_Org = data.Car_By_Gov_Name;                         //เก็บค่าจาก CMT

        //        if (AppUtil.CheckBrandCodeExist(slmdb, data.Brand_Code))
        //            prelead.slm_Brand_Code = data.Brand_Code;

        //        prelead.slm_Brand_Code_Org = data.Brand_Code;                                   //เก็บค่าจาก CMT
        //        prelead.slm_Brand_Name_Org = data.Brand_Name;                                   //เก็บค่าจาก CMT

        //        if (AppUtil.CheckModelCodeExist(slmdb, data.Brand_Code, data.Model_Code))
        //            prelead.slm_Model_Code = data.Model_Code;

        //        prelead.slm_Model_Code_Org = data.Model_Code;                                   //เก็บค่าจาก CMT
        //        prelead.slm_Model_name_Org = data.Model_Name;                                   //เก็บค่าจาก CMT

        //        prelead.slm_Engine_No = data.Engine_No;
        //        prelead.slm_Chassis_No = data.Chassis_No;
        //        prelead.slm_Model_Year = data.Model_Year;
        //        prelead.slm_Car_License_No = data.License_No;

        //        var provinceRegis = AppUtil.GetProvince(slmdb, data.Register_Province);
        //        if (provinceRegis != null)
        //            prelead.slm_ProvinceRegis = provinceRegis.slm_ProvinceId;

        //        prelead.slm_ProvinceRegis_Org = data.Register_Province;                         //เก็บค่าจาก CMT
        //        prelead.slm_Cc = data.CC != null ? data.CC.Value.ToString() : null;

        //        if (!string.IsNullOrEmpty(data.ExpiryDate))
        //        {
        //            DateTime expiryDate = AppUtil.ConvertToDateTime("ExpiryDate", data.ExpiryDate, AppConstant.DateTimeFormat.Format1);
        //            if (expiryDate.Year != 1 && expiryDate.Year != 1900)
        //                prelead.slm_Expire_Date = expiryDate;
        //        }

        //        prelead.slm_Voluntary_Mkt_Id = AppUtil.GetEmpCode(slmdb, data.Voluntary_MKT_ID);
        //        prelead.slm_Voluntary_Mkt_Id_Org = data.Voluntary_MKT_ID;                       //เก็บค่าจาก CMT (Voluntary_MKT_ID == marketingCode)
        //        prelead.slm_Voluntary_Mkt_TitleId = AppUtil.GetTitleId(slmdb, data.Voluntary_MKT_Title);
        //        prelead.slm_Voluntary_Mkt_Title_Org = data.Voluntary_MKT_Title;                 //เก็บค่าจาก CMT
        //        prelead.slm_Voluntary_Mkt_First_Name = data.Voluntary_MKT_First_Name;
        //        prelead.slm_Voluntary_Mkt_Last_Name = data.Voluntary_MKT_Last_Name;
        //        prelead.slm_Voluntary_Policy_Number = data.Voluntary_Policy_Number;
        //        prelead.slm_Voluntary_Type_Key = data.Voluntary_Type_Key;
        //        prelead.slm_Voluntary_Type_Name = data.Voluntary_Type_Name;
        //        prelead.slm_Voluntary_Policy_Year = data.Voluntary_Policy_Year;

        //        if (!string.IsNullOrEmpty(data.Voluntary_Policy_EFF_DATE))
        //        {
        //            DateTime policyEffDate = AppUtil.ConvertToDateTime("Voluntary_Policy_EFF_DATE", data.Voluntary_Policy_EFF_DATE, AppConstant.DateTimeFormat.Format1);
        //            if (policyEffDate.Year != 1 && policyEffDate.Year != 1900)
        //                prelead.slm_Voluntary_Policy_Eff_Date = policyEffDate;
        //        }

        //        if (!string.IsNullOrEmpty(data.Voluntary_Policy_EXP_DATE))
        //        {
        //            DateTime policyExpDate = AppUtil.ConvertToDateTime("Voluntary_Policy_EXP_DATE", data.Voluntary_Policy_EXP_DATE, AppConstant.DateTimeFormat.Format1);
        //            if (policyExpDate.Year != 1 && policyExpDate.Year != 1900)
        //                prelead.slm_Voluntary_Policy_Exp_Date = policyExpDate;
        //        }

        //        prelead.slm_Voluntary_Policy_Exp_Year = data.Voluntary_Policy_EXP_YEAR;
        //        prelead.slm_Voluntary_Policy_Exp_Month = data.Voluntary_Policy_EXP_MONTH;
        //        prelead.slm_Voluntary_Cov_Amt = data.Voluntary_COV_AMT;
        //        prelead.slm_Voluntary_Gross_Premium = data.Voluntary_Gross_Premium;
        //        prelead.slm_Voluntary_Channel_Key = data.Voluntary_Channel_Key;
        //        prelead.slm_Voluntary_Company_Code = data.Voluntary_Company_Code;
        //        prelead.slm_Voluntary_Company_Name = data.Voluntary_Company_Name;
        //        prelead.slm_Benefit_TitleId = AppUtil.GetTitleId(slmdb, data.Benefit_Title_Name);
        //        prelead.slm_Benefit_Title_Name_Org = data.Benefit_Title_Name;                           //เก็บค่าจาก CMT
        //        prelead.slm_Benefit_First_Name = data.Benefit__First_Name;
        //        prelead.slm_Benefit_Last_Name = data.Benefit_Last_Name;
        //        prelead.slm_Benefit_Telno = data.Benefit_TelNo;
        //        prelead.slm_Driver_TitleId1 = AppUtil.GetTitleId(slmdb, data.Driver_Title_Name1);
        //        prelead.slm_Driver_Title_Name1_Org = data.Driver_Title_Name1;
        //        prelead.slm_Driver_First_Name1 = data.Driver_First_Name1;
        //        prelead.slm_Driver_Last_Name1 = data.Driver_Last_Name1;
        //        prelead.slm_Driver_Telno1 = data.Driver_TelNo1;
        //        prelead.slm_Driver_TitleId2 = AppUtil.GetTitleId(slmdb, data.Driver_Title_Name2);
        //        prelead.slm_Driver_Title_Name2_Org = data.Driver_Title_Name2;
        //        prelead.slm_Driver_First_Name2 = data.Driver_First_Name2;
        //        prelead.slm_Driver_Last_Name2 = data.Driver_Last_Name2;
        //        prelead.slm_Driver_Telno2 = data.Driver_TelNo2;
        //        prelead.slm_Compulsory_Policy_Number = data.Compulsory_Policy_Number;
        //        prelead.slm_Compulsory_Policy_Year = data.Compulsory_Policy_Year;

        //        if (!string.IsNullOrEmpty(data.Compulsory_Policy_EFF_DATE))
        //        {
        //            DateTime policyEffDate = AppUtil.ConvertToDateTime("Compulsory_Policy_EFF_DATE", data.Compulsory_Policy_EFF_DATE, AppConstant.DateTimeFormat.Format1);
        //            if (policyEffDate.Year != 1 && policyEffDate.Year != 1900)
        //                prelead.slm_Compulsory_Policy_Eff_Date = policyEffDate;
        //        }

        //        if (!string.IsNullOrEmpty(data.Compulsory_Policy_EXP_DATE))
        //        {
        //            DateTime policyExpDate = AppUtil.ConvertToDateTime("Compulsory_Policy_EXP_DATE", data.Compulsory_Policy_EXP_DATE, AppConstant.DateTimeFormat.Format1);
        //            if (policyExpDate.Year != 1 && policyExpDate.Year != 1900)
        //                prelead.slm_Compulsory_Policy_Exp_Date = policyExpDate;
        //        }

        //        prelead.slm_Compulsory_Cov_Amt = data.Compulsory_COV_AMT;
        //        prelead.slm_Compulsory_Gross_Premium = data.Compulsory_Gross_Premium;
        //        prelead.slm_Compulsory_Company_Code = data.Compulsory_Company_Code;
        //        prelead.slm_Compulsory_Company_Name = data.Compulsory_Company_Name;

        //        prelead.slm_Guarantor_Code1 = data.Guarantor_Code1;
        //        prelead.slm_Guarantor_TitleId1 = AppUtil.GetTitleId(slmdb, data.Guarantor_Title_Name1);
        //        prelead.slm_Guarantor_Title_Name1_Org = data.Guarantor_Title_Name1;
        //        prelead.slm_Guarantor_First_Name1 = data.Guarantor_First_Name1;
        //        prelead.slm_Guarantor_Last_Name1 = data.Guarantor_Last_Name1;
        //        prelead.slm_Guarantor_Card_Id1 = data.Guarantor_Card_ID1;
        //        prelead.slm_Guarantor_RelationId1 = AppUtil.GetRelationId(slmdb, data.Guarantor_Relation1);
        //        prelead.slm_Guarantor_Relation1_Org = data.Guarantor_Relation1;
        //        prelead.slm_Guarantor_Telno1 = data.Guarantor_TelNo1;

        //        prelead.slm_Guarantor_Code2 = data.Guarantor_Code2;
        //        prelead.slm_Guarantor_TitleId2 = AppUtil.GetTitleId(slmdb, data.Guarantor_Tile_Name2);
        //        prelead.slm_Guarantor_Title_Name2_Org = data.Guarantor_Tile_Name2;
        //        prelead.slm_Guarantor_First_Name2 = data.Guarantor_First_Name2;
        //        prelead.slm_Guarantor_Last_Name2 = data.Guarantor_Last_Name2;
        //        prelead.slm_Guarantor_Card_Id2 = data.Guarantor_Card_ID2;
        //        prelead.slm_Guarantor_RelationId2 = AppUtil.GetRelationId(slmdb, data.Guarantor_Relation2);
        //        prelead.slm_Guarantor_Relation2_Org = data.Guarantor_Relation2;
        //        prelead.slm_Guarantor_Telno2 = data.Guarantor_TelNo2;

        //        prelead.slm_Guarantor_Code3 = data.Guarantor_Code3;
        //        prelead.slm_Guarantor_TitleId3 = AppUtil.GetTitleId(slmdb, data.Guarantor_Tile_Name3);
        //        prelead.slm_Guarantor_Title_Name3_Org = data.Guarantor_Tile_Name3;
        //        prelead.slm_Guarantor_First_Name3 = data.Guarantor_First_Name3;
        //        prelead.slm_Guarantor_Last_Name3 = data.Guarantor_Last_Name3;
        //        prelead.slm_Guarantor_Card_Id3 = data.Guarantor_Card_ID3;
        //        prelead.slm_Guarantor_RelationId3 = AppUtil.GetRelationId(slmdb, data.Guarantor_Relation3);
        //        prelead.slm_Guarantor_Relation3_Org = data.Guarantor_Relation3;
        //        prelead.slm_Guarantor_Telno3 = data.Guarantor_TelNo3;

        //        prelead.slm_Grade = data.Grade;

        //        if (!string.IsNullOrEmpty(data.UpdateDate))
        //        {
        //            DateTime updateDate = AppUtil.ConvertToDateTime("UpdateDate", data.UpdateDate, AppConstant.DateTimeFormat.Format2);
        //            if (updateDate.Year != 1 && updateDate.Year != 1900)
        //                prelead.slm_Compulsory_Policy_Exp_Date = updateDate;
        //        }

        //        DateTime createDate = DateTime.Now;
        //        prelead.slm_CreatedBy = "SYSTEM";
        //        prelead.slm_CreatedDate = createDate;
        //        prelead.slm_UpdatedBy = "SYSTEM";
        //        prelead.slm_UpdatedDate = createDate;
        //        prelead.is_Deleted = false;

        //        slmdb.kkslm_tr_prelead.AddObject(prelead);

        //        var cmtproduct = slmdb.CMT_OBT_PRODUCT.Where(p => p.Cmt_Product_Id == data.Cmt_Product_Id).FirstOrDefault();
        //        if (cmtproduct != null)
        //            cmtproduct.Obt_Flag = true;

        //        slmdb.SaveChanges();

        //        DoInsertAddress(prelead.slm_Prelead_Id, data.Customer_Key, batchMonitorId, batchCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void DoInsertAddress(SLMDBEntities slmdb, decimal preleadId, string customerKey, long batchMonitorId, string batchCode, List<ProvinceData> provinceDataList)
        //{
        //    try
        //    {
        //        List<CMT_OBT_ADDRESS> list = slmdb.CMT_OBT_ADDRESS.Where(p => p.Customer_Key == customerKey).ToList();

        //        foreach (CMT_OBT_ADDRESS data in list)
        //        {
        //            try
        //            {
        //                DateTime createDate = DateTime.Now;
        //                kkslm_tr_prelead_address addr = new kkslm_tr_prelead_address()
        //                {
        //                    slm_Prelead_Id = preleadId,
        //                    slm_Customer_Key = customerKey,
        //                    slm_Address_Type = data.Address_Type,
        //                    slm_Contract_Adrress = data.Contract_Address,
        //                    slm_House_No = data.House_No,
        //                    slm_Moo = data.Moo,
        //                    slm_Building = data.Building,
        //                    slm_Village = data.Village,
        //                    slm_Soi = data.Soi,
        //                    slm_Street = data.Street,
        //                    slm_Province_name_Org = data.Province,
        //                    slm_Amphur_Name_Org = data.Amphur,
        //                    slm_Tambon_Name_Org = data.Tambon,
        //                    slm_Zipcode = data.ZipCode,
        //                    slm_Home_Phone = data.Home_Phone,
        //                    slm_Mobile_Phone = data.Mobile_Phone,
        //                    slm_CreatedBy = "SYSTEM",
        //                    slm_CreatedDate = createDate,
        //                    slm_UpdatedBy = "SYSTEM",
        //                    slm_UpdatedDate = createDate,
        //                    is_Deleted = false
        //                };

        //                var province_list = provinceDataList.Where(p => p.ProvinceName == data.Province && p.AmphurName == data.Amphur && p.TambolName == data.Tambon).FirstOrDefault();
        //                if (province_list != null)
        //                {
        //                    addr.slm_Province_Id = province_list.ProvinceId;
        //                    addr.slm_Amphur_Id = province_list.AmphurId;
        //                    addr.slm_TambolId = province_list.TambolId;
        //                }

        //                slmdb.kkslm_tr_prelead_address.AddObject(addr);
        //                slmdb.SaveChanges();
        //            }
        //            catch (Exception ex)
        //            {
        //                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                string errorDetail = "Cmt_Address_Id=" + data.Cmt_Address_Id.ToString() + ", Insert failed for PreleadId=" + preleadId.ToString() + ", Error=" + message;

        //                BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
        //                Util.WriteLogFile(logfilename, batchCode, errorDetail);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion
    }
}
