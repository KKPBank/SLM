using System;
using System.Collections.Generic;
using System.Transactions;
using System.Data;
using SLMBatch.Common;
using SLMBatch.Data;
using System.Linq;

namespace SLMBatch.Business
{
    public class OBT_PRO_06_Biz : ServiceBase
    {
        #region Backup 05-06-2016 Classic Process

        //public void InsertCoverageType(string batchCode)
        //{
        //    Int64 batchMonitorId = 0;
        //    int totalRecord = 0;
        //    int totalSuccess = 0;
        //    int totalFail = 0;
        //    List<int> idList = new List<int>();

        //    try
        //    {
        //        batchMonitorId = BizUtil.SetStartTime(batchCode);
        //        BizUtil.CheckPrerequisite(batchCode);

        //        DataTable dtCoverage = GetCoverageTypeFromBDW();
        //        totalRecord = dtCoverage.Rows.Count;

        //        foreach (DataRow dr in dtCoverage.Rows)
        //        {
        //            try
        //            {
        //                if (dr["COVERAGE_NO"].ToString().Trim() == "")
        //                    throw new Exception("COVERAGE_NO is null or empty");

        //                if (dr["COVERAGE_DESC"].ToString().Trim() == "")
        //                    throw new Exception("COVERAGE_DESC is null or empty");

        //                int coverateTypeId = int.Parse(dr["COVERAGE_NO"].ToString().Trim());
        //                idList.Add(coverateTypeId);

        //                DateTime createdDate = DateTime.Now;

        //                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
        //                var coverage = slmdb.kkslm_ms_coveragetype.Where(p => p.slm_CoverageTypeId == coverateTypeId).FirstOrDefault();
        //                if (coverage == null)
        //                {
        //                    kkslm_ms_coveragetype objInsert = new kkslm_ms_coveragetype()
        //                    {
        //                        slm_CoverageTypeId = coverateTypeId,
        //                        slm_ConverageTypeName = dr["COVERAGE_DESC"].ToString().Trim(),
        //                        slm_CreatedBy = "SYSTEM",
        //                        slm_CreatedDate = createdDate,
        //                        slm_UpdatedBy = "SYSTEM",
        //                        slm_UpdatedDate = createdDate,
        //                        is_Deleted = false
        //                    };
        //                    slmdb.kkslm_ms_coveragetype.AddObject(objInsert);
        //                }
        //                else
        //                {
        //                    coverage.slm_ConverageTypeName = dr["COVERAGE_DESC"].ToString().Trim();
        //                    coverage.slm_UpdatedBy = "SYSTEM";
        //                    coverage.slm_UpdatedDate = createdDate;
        //                    coverage.is_Deleted = false;
        //                }

        //                slmdb.SaveChanges();

        //                totalSuccess += 1;
        //                Console.WriteLine("CoverageNo " + dr["COVERAGE_NO"].ToString() + ": SUCCESS");
        //            }
        //            catch (Exception ex)
        //            {
        //                totalFail += 1;
        //                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                string errorDetail = "CoverageNo=" + dr["COVERAGE_NO"].ToString() + ", Error=" + message;

        //                BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
        //                Util.WriteLogFile(logfilename, batchCode, errorDetail);

        //                Console.WriteLine("CoverageNo " + dr["COVERAGE_NO"].ToString() + ": FAIL");
        //            }
        //        }

        //        SetFlagIsDeleted(idList);

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

        #endregion
        
        /// <summary>
        /// Update ข้อมูลประเภทประกันภัยและประเภทความคุ้มครอง จาก DataWarehouse ไปที่ OBT
        /// </summary>
        /// <param name="batchCode"></param>
        public bool InsertCoverageType(string batchCode)
        {
            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;
            List<int> idList = new List<int>();
            bool ret = false;

            try
            {
                batchMonitorId = BizUtil.SetStartTime(batchCode);
                BizUtil.CheckPrerequisite(batchCode);

                DataTable dtCoverage = GetCoverageTypeFromBDW(batchMonitorId, batchCode);
                totalRecord = dtCoverage.Rows.Count;

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    foreach (DataRow dr in dtCoverage.Rows)
                    {
                        if (dr["COVERAGE_NO"].ToString().Trim() == "")
                            throw new Exception("COVERAGE_NO is null or empty");

                        if (dr["COVERAGE_DESC"].ToString().Trim() == "")
                            throw new Exception("COVERAGE_DESC is null or empty");

                        int coverateTypeId = int.Parse(dr["COVERAGE_NO"].ToString().Trim());
                        idList.Add(coverateTypeId);

                        DateTime createdDate = DateTime.Now;

                        SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                        var coverage = slmdb.kkslm_ms_coveragetype.Where(p => p.slm_CoverageTypeId == coverateTypeId).FirstOrDefault();
                        if (coverage == null)
                        {
                            kkslm_ms_coveragetype objInsert = new kkslm_ms_coveragetype()
                            {
                                slm_CoverageTypeId = coverateTypeId,
                                slm_ConverageTypeName = dr["COVERAGE_DESC"].ToString().Trim(),
                                slm_CreatedBy = "SYSTEM",
                                slm_CreatedDate = createdDate,
                                slm_UpdatedBy = "SYSTEM",
                                slm_UpdatedDate = createdDate,
                                is_Deleted = false
                            };
                            slmdb.kkslm_ms_coveragetype.AddObject(objInsert);
                        }
                        else
                        {
                            coverage.slm_ConverageTypeName = dr["COVERAGE_DESC"].ToString().Trim();
                            coverage.slm_UpdatedBy = "SYSTEM";
                            coverage.slm_UpdatedDate = createdDate;
                            coverage.is_Deleted = false;
                        }

                        slmdb.SaveChanges();

                        totalSuccess += 1;
                        //Console.WriteLine("CoverageNo " + dr["COVERAGE_NO"].ToString() + ": SUCCESS");
                    }

                    SetFlagIsDeleted(idList);

                    ts.Complete();
                }

                ret = true;
                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                ret = false;
                //Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Util.WriteLogFile(logfilename, batchCode, message);
                BizUtil.InsertLog(batchMonitorId, "", "", message);

                totalSuccess = 0;
                totalFail = totalRecord;

                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }

            return ret;
        }

        private DataTable GetCoverageTypeFromBDW(Int64 batchMonitorId, string batchCode)
        {
            OracleDB db = null;
            try
            {
                db = new OracleDB(AppConstant.DWHConnectionString);

                //Check ว่าต้นทางมีข้อมูลพร้อมให้ load หรือยัง
                DateTime checkDate = DateTime.Today.AddDays(-1);
                string date = checkDate.Year.ToString() + checkDate.ToString("MMdd");
                string sqlcheck = @"SELECT ZDATA_DATE, ZPROCESS_ID, ZSTART_DATE, MAX (ZSTART_TIME) AS ZSTART_TIME, ZFINISH_DATE, MAX (ZFINISH_TIME) AS ZFINISH_TIME, ZFINISH_FLG 
                                    FROM " + AppConstant.DWHSchema + @".ZBW_PROC_STAT 
                                    WHERE zprocess_id = 'OBT_IN' AND zfinish_flg = 'Y' AND ZDATA_DATE = '" + date + @"' 
                                    GROUP BY ZDATA_DATE, ZPROCESS_ID, ZSTART_DATE, ZFINISH_DATE, ZFINISH_FLG ";

                DataTable dtCheck = db.ExcecuteTable(sqlcheck);
                if (dtCheck.Rows.Count == 0)
                {
                    //Util.WriteLogFile(logfilename, batchCode, "ข้อมูลบนระบบ Datawarehouse ยังไม่พร้อมใช้งาน");
                    //BizUtil.InsertLog(batchMonitorId, "", "", "ข้อมูลบนระบบ Datawarehouse ยังไม่พร้อมใช้งาน");
                    throw new Exception("ข้อมูลบนระบบ Datawarehouse ยังไม่พร้อมใช้งาน");

                    //return null;
                }

                return db.ExcecuteTable("SELECT * FROM " + AppConstant.DWHSchema + ".OBT_VOLUNTARY_INSURANCE_TYPE");
            }            
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db != null)
                {
                    db.CloseConnection();
                }
            }
        }

        private void SetFlagIsDeleted(List<int> idList)
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
                var flagList = slmdb.kkslm_ms_coveragetype.Where(p => idList.Contains(p.slm_CoverageTypeId) == false).ToList();

                flagList.ForEach(p => { p.is_Deleted = true; p.slm_UpdatedBy = "SYSTEM"; p.slm_UpdatedDate = DateTime.Now; });
                slmdb.SaveChanges();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception("Cannot update is_deleted=true in kkslm_ms_coveragetype because " + message);
            }
        }
    }
}
