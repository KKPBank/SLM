using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Business
{
    public class OBT_PRO_03_Biz : ServiceBase
    {
        #region Backup 04-06-2016
        //        public void InsertTelesalesOwnertoHP(string batchCode)
//        {
//            //year ที่ insert ลง HP เป็น พ.ศ. (confirmed)
//            //year, month คือ ปี เดือน ที่ update ข้อมูลไป HP

//            //รอเทสที่ SIT2

//            Int64 batchMonitorId = 0;
//            int totalRecord = 0;
//            int totalSuccess = 0;
//            int totalFail = 0;

//            SQLServerDB hpdb = null;
//            SQLServerDB slmdb = null;
//            string sql = "";
//            string year = "";
//            string month = "";
//            string processState = "";

//            try
//            {
//                batchMonitorId = BizUtil.SetStartTime(batchCode);
//                BizUtil.CheckPrerequisite(batchCode);

//                List<UpdateTelesalesOwnerData> list = GetTelesalesOwner();
//                totalRecord = list.Count;

//                if (list.Count > 0)
//                {
//                    hpdb = new SQLServerDB(AppConstant.HPConnectionString);
//                    slmdb = new SQLServerDB(AppConstant.SLMDBConnectionString);

//                    year = (DateTime.Now.Year + 543).ToString();
//                    month = DateTime.Now.Month.ToString("00");

//                    foreach (UpdateTelesalesOwnerData data in list)
//                    {
//                        try
//                        {
//                            //Insert to HP
//                            processState = "HP";

//                            sql = @"INSERT INTO Import_MKT_Data_OBT(year, month, contract_no, mkt_code) 
//                                VALUES('" + year + "','" + month + "','" + data.ContractNo + "','" + data.EmpCode + "')";

//                            hpdb.ExceuteNonQuery(sql);

//                            //Update Flag at SLM
//                            processState = "SLM";

//                            sql = @"UPDATE " + AppConstant.SLMDBName + @".dbo.kkslm_tr_prelead
//                                SET slm_ObtPro03Flag = 1
//                                WHERE slm_Prelead_Id = '" + data.PreleadId.ToString() + "'";

//                            slmdb.ExceuteNonQuery(sql);

//                            totalSuccess += 1;
//                            Console.WriteLine("ContractNo " + data.ContractNo + ": SUCCESS");
//                        }
//                        catch (Exception ex)
//                        {
//                            totalFail += 1;
//                            string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
//                            string errorDetail = "";

//                            if (processState == "HP")
//                                errorDetail = "Cannot Update Telesales Owner to HP, ContractNo=" + data.ContractNo + ", Error=" + message;
//                            else if (processState == "SLM")
//                                errorDetail = "Cannot Update slm_ObtPro03Flag=true (kkslm_tr_prelead) to SLM, ContractNo=" + data.ContractNo + ", Error=" + message;
//                            else
//                                errorDetail = "ContractNo=" + data.ContractNo + ", Error=" + message;

//                            BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
//                            Util.WriteLogFile(logfilename, batchCode, errorDetail);

//                            Console.WriteLine("ContractNo " + data.ContractNo + ": FAIL");
//                        }
//                    }
//                }

//                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("All FAIL");
//                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

//                Util.WriteLogFile(logfilename, batchCode, message);
//                BizUtil.InsertLog(batchMonitorId, "", "", message);
//                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
//                throw ex;
//            }
//            finally
//            {
//                if (hpdb != null)
//                    hpdb.CloseConnection();

//                if (slmdb != null)
//                    slmdb.CloseConnection();
//            }
        //        }
        #endregion
        
        /// <summary>
        /// Update ข้อมูล Telesales Owner จาก OBT ไปที่ HP
        /// </summary>
        /// <param name="batchCode"></param>
        public void InsertTelesalesOwnertoHP(string batchCode)
        {
            //year ที่ insert ลง HP เป็น พ.ศ. (confirmed)
            //year, month คือ ปี เดือน ที่ update ข้อมูลไป HP

            //รอเทสที่ SIT2

            Int64 batchMonitorId = 0;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;

            SQLServerDB hpdb = null;
            SQLServerDB slmdb = null;
            string sql = "";
            string year = "";
            string month = "";
            string processState = "";
            bool successFlag = true;
            List<TempDataPro03> tempList = new List<TempDataPro03>();

            try
            {
                batchMonitorId = BizUtil.SetStartTime(batchCode);
                BizUtil.CheckPrerequisite(batchCode);

                List<UpdateTelesalesOwnerData> list = GetTelesalesOwner();
                totalRecord = list.Count;

                if (list.Count > 0)
                {
                    hpdb = new SQLServerDB(AppConstant.HPConnectionString);
                    slmdb = new SQLServerDB(SLMDBConnectionString);

                    year = (DateTime.Now.Year + 543).ToString();
                    month = DateTime.Now.Month.ToString("00");

                    foreach (UpdateTelesalesOwnerData data in list)
                    {
                        try
                        {
                            //Insert to HP
                            processState = "HP";

                            //เก็บไว้ในกรณีมี row fail ให้นำค่าใน list ไปลบออกจากเบส HP
                            tempList.Add(new TempDataPro03() { Year = year, Month = month, ContractNo = data.ContractNo, EmpCode = data.EmpCode, PreleadId = data.PreleadId });

                            sql = @"INSERT INTO Import_MKT_Data_OBT(year, month, contract_no, mkt_code) 
                                VALUES('" + year + "','" + month + "','" + data.ContractNo + "','" + data.EmpCode + "')";

                            hpdb.ExecuteNonQuery(sql);

                            //Update Flag at SLM
                            processState = "SLM";

                            sql = @"UPDATE " + AppConstant.SLMDBName + @".dbo.kkslm_tr_prelead
                                    SET slm_ObtPro03Flag = 1, slm_ObtPro03FlagDate = GETDATE()
                                    WHERE slm_Prelead_Id = '" + data.PreleadId.ToString() + "'";

                            slmdb.ExecuteNonQuery(sql);

                            totalSuccess += 1;
                            Console.WriteLine("ContractNo " + data.ContractNo + ": SUCCESS");
                        }
                        catch (Exception ex)
                        {
                            successFlag = false;
                            RollbackHP(batchMonitorId, batchCode, tempList, hpdb);
                            RollBackPrelead(batchMonitorId, batchCode, tempList, slmdb);

                            string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            string errorDetail = "";

                            if (processState == "HP")
                                errorDetail = "Cannot Update Telesales Owner to HP, ContractNo=" + data.ContractNo + ", Error=" + message;
                            else if (processState == "SLM")
                                errorDetail = "Cannot Update slm_ObtPro03Flag=true (kkslm_tr_prelead) to SLM, ContractNo=" + data.ContractNo + ", Error=" + message;
                            else
                                errorDetail = "ContractNo=" + data.ContractNo + ", Error=" + message;

                            BizUtil.InsertLog(batchMonitorId, "", "", errorDetail);
                            Util.WriteLogFile(logfilename, batchCode, errorDetail);

                            successFlag = false;
                            totalSuccess = 0;
                            totalFail = totalRecord;
                            break;
                            //Console.WriteLine("ContractNo " + data.ContractNo + ": FAIL");
                        }
                    }
                }

                BizUtil.SetEndTime(batchCode, batchMonitorId, (successFlag ? AppConstant.Success : AppConstant.Fail), totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Util.WriteLogFile(logfilename, batchCode, message);
                BizUtil.InsertLog(batchMonitorId, "", "", message);

                totalSuccess = 0;
                totalFail = totalRecord;

                BizUtil.SetEndTime(batchCode, batchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
            finally
            {
                if (hpdb != null)
                    hpdb.CloseConnection();

                if (slmdb != null)
                    slmdb.CloseConnection();
            }
        }

        private List<UpdateTelesalesOwnerData> GetTelesalesOwner()
        {
            try
            {
                SLMDBEntities slmdb = AppUtil.GetSlmDbEntities();
//                string sql = @"SELECT slm_Prelead_Id AS PreleadId, slm_Contract_Number AS ContractNo, slm_Owner AS EmpCode 
//                                FROM " + AppConstant.SLMDBName + @".dbo.kkslm_tr_prelead 
//                                WHERE is_Deleted = 0 and slm_AssignFlag = '1' and slm_Assign_Status = '1' and (slm_ObtPro03Flag IS NULL Or slm_ObtPro03Flag = 0) ";

                string sql = @"SELECT prelead.slm_Prelead_Id AS PreleadId, prelead.slm_Contract_Number AS ContractNo, prelead.slm_Owner AS EmpCode 
                                FROM (
	                                SELECT slm_Contract_Number AS ContractNo, MAX(slm_Prelead_Id) AS PreleadId 
	                                from " + AppConstant.SLMDBName + @".dbo.kkslm_tr_prelead
	                                WHERE is_Deleted = 0 and slm_AssignFlag = '1' and slm_Assign_Status = '1' and (slm_ObtPro03Flag IS NULL Or slm_ObtPro03Flag = 0) 
	                                GROUP BY slm_Contract_Number
                                ) A
                                INNER JOIN " + AppConstant.SLMDBName + @".dbo.kkslm_tr_prelead prelead ON prelead.slm_Prelead_Id = A.PreleadId
                                WHERE prelead.is_Deleted = 0 and prelead.slm_AssignFlag = '1' and prelead.slm_Assign_Status = '1' and (prelead.slm_ObtPro03Flag IS NULL Or prelead.slm_ObtPro03Flag = 0) ";

                return slmdb.ExecuteStoreQuery<UpdateTelesalesOwnerData>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RollbackHP(Int64 batchMonitorId, string batchCode, List<TempDataPro03> tempList, SQLServerDB hpdb)
        {
            try
            {
                string sql = "";
                foreach (TempDataPro03 data in tempList)
                {
                    sql = @"DELETE FROM Import_MKT_Data_OBT
                            WHERE year = '" + data.Year + "' AND month = '" + data.Month + "' AND contract_no = '" + data.ContractNo + "' AND mkt_code = '" + data.EmpCode + "' ";

                    hpdb.ExecuteNonQuery(sql);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                Util.WriteLogFile(logfilename, batchCode, message);
            }
        }

        private void RollBackPrelead(Int64 batchMonitorId, string batchCode, List<TempDataPro03> tempList, SQLServerDB slmdb)
        {
            try
            {
                string sql = "";
                foreach (TempDataPro03 data in tempList)
                {
                    sql = @"UPDATE " + AppConstant.SLMDBName + @".dbo.kkslm_tr_prelead
                                SET slm_ObtPro03Flag = 0, slm_ObtPro03FlagDate = NULL
                                WHERE slm_Prelead_Id = '" + data.PreleadId.ToString() + "'";

                    slmdb.ExecuteNonQuery(sql);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BizUtil.InsertLog(batchMonitorId, "", "", message);
                Util.WriteLogFile(logfilename, batchCode, message);
            }
        }
    }

    public class TempDataPro03
    {
        public decimal? PreleadId { get; set; }
        public string ContractNo { get; set; }
        public string EmpCode { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
    }

}
