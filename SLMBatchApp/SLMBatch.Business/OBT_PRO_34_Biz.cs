using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;

namespace SLMBatch.Business
{
    public class OBT_PRO_34_Biz : ServiceBase, IDisposable
    {
        public bool ImportNotifyPremium(string batchCode)
        {
            bool ret = false;
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;
            List<kkslm_tr_notify_premium_temp> tempList;
            List<kkslm_ms_repairtype> repairTypeList;

            try
            {
                BatchCode = batchCode;
                BatchMonitorId = BizUtil.SetStartTime(batchCode);
                BizUtil.CheckPrerequisite(batchCode);

                var comList = GetInsuranceCompany();

                using (var slmdb = AppUtil.GetSlmDbEntities())
                {
                    tempList = slmdb.kkslm_tr_notify_premium_temp.Where(p => p.slm_ImportFlag == "0").OrderBy(p => p.slm_Id).ToList();
                    totalRecord = tempList.Count;
                    repairTypeList = slmdb.kkslm_ms_repairtype.ToList();
                }

                int i = 0;
                foreach (var itm in tempList)
                {
                    i += 1;
                    Console.Write("\r{0}/{1}", i.ToString(), totalRecord.ToString());

                    try
                    {
                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                        {
                            using (var slmdb = AppUtil.GetSlmDbEntities())
                            {
                                bool result = InsertNotifyPremium(slmdb, itm, comList, repairTypeList);
                                if (!result)
                                {
                                    BizUtil.InsertLog(BatchMonitorId, "", "", $"TempId={itm.slm_Id}, Error={ErrorMessage}");
                                }

                                var obj = slmdb.kkslm_tr_notify_premium_temp.FirstOrDefault(p => p.slm_Id == itm.slm_Id);
                                if (obj != null)
                                {
                                    obj.slm_ImportFlag = result ? "1" : "2";
                                    obj.slm_ImportDate = DateTime.Now;
                                    slmdb.SaveChanges();
                                }

                                ts.Complete();
                                totalSuccess += 1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        totalFail += 1;
                        string message = $"TempId={itm.slm_Id.ToString()}, Error={(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}";
                        Util.WriteLogFile(logfilename, BatchCode, message);
                        BizUtil.InsertLog(BatchMonitorId, "", "", message);
                    }
                }

                Console.WriteLine("");
                ret = true;
                BizUtil.SetEndTime(BatchCode, BatchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
            }
            catch (Exception ex)
            {
                ret = false;

                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Util.WriteLogFile(logfilename, BatchCode, message);
                BizUtil.InsertLog(BatchMonitorId, "", "", message);

                totalSuccess = 0;
                totalFail = totalRecord;
                BizUtil.SetEndTime(BatchCode, BatchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
            finally
            {
                DeleteTempNotifyPremium();
            }

            return ret;
        }

        private List<kkslm_tr_notify_premium_temp> GetNotifyPremiumTempList()
        {
            using (var slmdb = AppUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_tr_notify_premium_temp.Where(p => p.slm_ImportFlag == "0").ToList();
            }
        }

        private List<InsuranceCompanyData> GetInsuranceCompany()
        {
            string sql = string.Format(@"SELECT slm_Ins_Com_Id AS InsComId, slm_InsCode AS InsComCode, slm_InsNameTh AS InsNameTH, slm_TelContact AS TelContact
                                                FROM {0}.dbo.kkslm_ms_ins_com WHERE is_Deleted = 0", AppConstant.OPERDBName);

            using (var slmdb = AppUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<InsuranceCompanyData>(sql).ToList();
            }
        }

        private bool InsertNotifyPremium(SLMDBEntities slmdb, kkslm_tr_notify_premium_temp itm, List<InsuranceCompanyData> comList, List<kkslm_ms_repairtype> repairTypeList)
        {
            try
            {
                DateTime createdDate = DateTime.Now;

                kkslm_tr_notify_premium np = new kkslm_tr_notify_premium();
                np.slm_RunNo = itm.slm_RunNo;
                np.slm_PolicyNo = itm.slm_PolicyNo;
                np.slm_Title_Id = itm.slm_Title_Id;
                np.slm_Name = itm.slm_Name;
                np.slm_LastName = itm.slm_LastName;
                np.slm_Claim_Center = itm.slm_Claim_Center;
                np.slm_Claim_Garage = itm.slm_Claim_Garage;
                np.slm_RepairTypeId = itm.slm_RepairTypeId;
                np.slm_InsExpireDate = itm.slm_InsExpireDate;
                np.slm_CarLicenseNo = itm.slm_CarLicenseNo;
                np.slm_Brand = itm.slm_Brand;
                np.slm_Model = itm.slm_Model;
                np.slm_VIN = itm.slm_VIN;
                np.slm_InsuranceCarTypeId = itm.slm_InsuranceCarTypeId; // มันคือ ประเภทประกัน ชั้น 1/2/3 (CoverageTypeId) 
                np.slm_Sum_Insure = itm.slm_Sum_Insure;
                np.slm_Discount_Percent = itm.slm_Discount_Percent;
                np.slm_Discount_Amount = itm.slm_Discount_Amount;
                np.slm_NetPremium = itm.slm_NetPremium;
                np.slm_Stamp = itm.slm_Stamp;
                np.slm_Vat_Amount = itm.slm_Vat_Amount;
                np.slm_GrossPremium = itm.slm_GrossPremium;
                np.slm_Remark = itm.slm_Remark;
                np.slm_InsuranceComId = itm.slm_InsuranceComId;
                np.slm_PeriodMonth = itm.slm_PeriodMonth;
                np.slm_PeriodYear = itm.slm_PeriodYear;
                np.slm_ActNetPremium = itm.slm_ActNetPremium;
                np.slm_ActStamp = itm.slm_ActStamp;
                np.slm_ActVat_Amount = itm.slm_ActVat_Amount;
                np.slm_ActGrossPremium = itm.slm_ActGrossPremium;
                np.slm_DriverFlag = itm.slm_DriverFlag;
                np.slm_UpdatedBy = "SYSTEM";
                np.slm_UpdatedDate = createdDate;
                np.slm_CreatedBy = "SYSTEM";
                np.slm_CreatedDate = createdDate;
                slmdb.kkslm_tr_notify_premium.AddObject(np);
                slmdb.SaveChanges();    //Must savechanges to get slm_Id

                string cleansingVin = np.slm_VIN.Replace(" ", "").Replace("-", "").Replace("*", "").Replace("_", "").Replace("\\", "").Replace("/", "");

                var rcp = (from lead in slmdb.kkslm_tr_lead
                           join ins in slmdb.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
                           join cmp in slmdb.kkslm_tr_renewinsurance_compare on ins.slm_RenewInsureId equals cmp.slm_RenewInsureId
                           where cmp.slm_Seq == 2 && lead.is_Deleted == 0 && ins.slm_ReceiveNo == null && ins.slm_ReceiveDate == null && ins.slm_ChassisNo.Replace(" ", "").Replace("-", "").Replace("*", "").Replace("_", "").Replace("\\", "").Replace("/", "") == cleansingVin
                                && ins.slm_PeriodMonth == np.slm_PeriodMonth && ins.slm_PeriodYear == np.slm_PeriodYear
                           select cmp).FirstOrDefault();

                if (rcp != null)
                {
                    rcp.slm_NotifyPremiumId = np.slm_Id;
                    if (np.slm_InsuranceComId != null)
                    {
                        rcp.slm_Ins_Com_Id = np.slm_InsuranceComId.Value;
                    }
                    rcp.slm_RepairTypeId = np.slm_RepairTypeId;
                    if (np.slm_InsuranceCarTypeId != null)
                    {
                        rcp.slm_CoverageTypeId = np.slm_InsuranceCarTypeId.Value;
                    }
                    rcp.slm_OD = np.slm_Sum_Insure;
                    rcp.slm_PolicyGrossPremium = np.slm_NetPremium;
                    rcp.slm_PolicyGrossStamp = np.slm_Stamp;
                    rcp.slm_PolicyGrossVat = np.slm_Vat_Amount;
                    rcp.slm_NetGrossPremium = np.slm_GrossPremium;
                    rcp.slm_UpdatedDate = createdDate;
                    rcp.slm_UpdatedBy = "SYSTEM";
                    rcp.slm_DiscountPercent = null;
                    rcp.slm_DiscountBath = null;
                    rcp.slm_PolicyGrossPremiumPay = np.slm_GrossPremium;

                    var renew = slmdb.kkslm_tr_renewinsurance.Where(p => p.slm_RenewInsureId == rcp.slm_RenewInsureId).FirstOrDefault();
                    if (renew != null)
                    {
                        if (rcp.slm_Selected == true)
                        {
                            renew.slm_InsuranceComId = np.slm_InsuranceComId;
                        }

                        var insComName = comList.Where(p => p.InsComId == rcp.slm_Ins_Com_Id).Select(p => p.InsNameTH).FirstOrDefault();
                        string remark = string.IsNullOrEmpty(np.slm_Remark) ? "" : (np.slm_Remark + Environment.NewLine);
                        remark += "ปีนี้ ";
                        if (!string.IsNullOrEmpty(insComName))
                        {
                            remark += insComName;
                        }
                        remark += " ให้ส่วนลดประวัติดี ";

                        if (np.slm_Discount_Percent != null && np.slm_Discount_Amount != null)
                        {
                            remark += np.slm_Discount_Percent.Value.ToString("#,##0") + "%";
                            remark += " เป็นเงิน " + np.slm_Discount_Amount.Value.ToString("#,##0.00") + " บาท";
                        }
                        else if (np.slm_Discount_Percent != null)
                        {
                            remark += np.slm_Discount_Percent.Value.ToString("#,##0") + "%";
                        }
                        else if (np.slm_Discount_Amount != null)
                        {
                            remark += np.slm_Discount_Amount.Value.ToString("#,##0.00") + " บาท";
                        }

                        if (np.slm_CreatedDate != null)
                            remark += " นำเข้าวันที่ " + np.slm_CreatedDate.Value.ToString("dd/MM/") + np.slm_CreatedDate.Value.Year.ToString() + " " + np.slm_CreatedDate.Value.ToString("HH:mm:ss");

                        renew.slm_RemarkPolicy = string.IsNullOrEmpty(renew.slm_RemarkPolicy) ? remark : (renew.slm_RemarkPolicy + Environment.NewLine + remark);
                    }

                    //rcp.slm_DiscountPercent = np.slm_Discount_Percent;
                    //rcp.slm_DiscountBath = np.slm_Discount_Amount;

                    //if (np.slm_GrossPremium != null && np.slm_Discount_Amount != null)
                    //    rcp.slm_PolicyGrossPremiumPay = np.slm_GrossPremium - np.slm_Discount_Amount;
                    //else if (np.slm_GrossPremium != null && np.slm_Discount_Amount == null)
                    //    rcp.slm_PolicyGrossPremiumPay = np.slm_GrossPremium;
                }
                else
                {
                    var prcp = (from plead in slmdb.kkslm_tr_prelead
                                join pcp in slmdb.kkslm_tr_prelead_compare on plead.slm_Prelead_Id equals pcp.slm_Prelead_Id
                                where pcp.slm_Seq == 2 && plead.is_Deleted == false && plead.slm_TicketId == null && plead.slm_Chassis_No.Replace(" ", "").Replace("-", "").Replace("*", "").Replace("_", "").Replace("\\", "").Replace("/", "") == cleansingVin
                                && plead.slm_PeriodMonth == np.slm_PeriodMonth && plead.slm_PeriodYear == np.slm_PeriodYear
                                select pcp).FirstOrDefault();

                    if (prcp != null)
                    {
                        prcp.slm_NotifyPremiumId = np.slm_Id;
                        if (np.slm_InsuranceComId != null)
                        {
                            prcp.slm_Ins_Com_Id = np.slm_InsuranceComId.Value;
                        }
                        prcp.slm_RepairTypeId = np.slm_RepairTypeId;
                        if (np.slm_InsuranceCarTypeId != null)
                        {
                            prcp.slm_CoverageTypeId = np.slm_InsuranceCarTypeId.Value;
                        }
                        prcp.slm_OD = np.slm_Sum_Insure;
                        prcp.slm_PolicyGrossPremium = np.slm_NetPremium;
                        prcp.slm_PolicyGrossStamp = np.slm_Stamp;
                        prcp.slm_PolicyGrossVat = np.slm_Vat_Amount;
                        prcp.slm_NetGrossPremium = np.slm_GrossPremium;
                        prcp.slm_UpdatedDate = createdDate;
                        prcp.slm_UpdatedBy = "SYSTEM";
                        prcp.slm_DiscountPercent = null;
                        prcp.slm_DiscountBath = null;
                        prcp.slm_PolicyGrossPremiumPay = np.slm_GrossPremium;

                        var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == prcp.slm_Prelead_Id).FirstOrDefault();
                        if (prelead != null)
                        {
                            var insComName = comList.Where(p => p.InsComId == prcp.slm_Ins_Com_Id).Select(p => p.InsNameTH).FirstOrDefault();

                            string remark = string.IsNullOrEmpty(np.slm_Remark) ? "" : (np.slm_Remark + Environment.NewLine);
                            remark += "ปีนี้ ";

                            if (!string.IsNullOrEmpty(insComName))
                            {
                                remark += insComName;
                            }
                            remark += " ให้ส่วนลดประวัติดี ";

                            if (np.slm_Discount_Percent != null && np.slm_Discount_Amount != null)
                            {
                                remark += np.slm_Discount_Percent.Value.ToString("#,##0") + "%";
                                remark += " เป็นเงิน " + np.slm_Discount_Amount.Value.ToString("#,##0.00") + " บาท";
                            }
                            else if (np.slm_Discount_Percent != null)
                            {
                                remark += np.slm_Discount_Percent.Value.ToString("#,##0") + "%";
                            }
                            else if (np.slm_Discount_Amount != null)
                            {
                                remark += np.slm_Discount_Amount.Value.ToString("#,##0.00") + " บาท";
                            }

                            if (np.slm_CreatedDate != null)
                                remark += " นำเข้าวันที่ " + np.slm_CreatedDate.Value.ToString("dd/MM/") + np.slm_CreatedDate.Value.Year.ToString() + " " + np.slm_CreatedDate.Value.ToString("HH:mm:ss");

                            prelead.slm_RemarkPolicy = string.IsNullOrEmpty(prelead.slm_RemarkPolicy) ? remark : (prelead.slm_RemarkPolicy + Environment.NewLine + remark);

                        }

                        //prcp.slm_DiscountPercent = np.slm_Discount_Percent;
                        //prcp.slm_DiscountBath = np.slm_Discount_Amount;

                        //if (np.slm_GrossPremium != null && np.slm_Discount_Amount != null)
                        //    prcp.slm_PolicyGrossPremiumPay = np.slm_GrossPremium - np.slm_Discount_Amount;
                        //else if (np.slm_GrossPremium != null && np.slm_Discount_Amount == null)
                        //    prcp.slm_PolicyGrossPremiumPay = np.slm_GrossPremium;
                    }
                }

                slmdb.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return false;
            }
        }

        public void DeleteTempNotifyPremium()
        {
            try
            {
                using (SLMDBEntities slmdb = AppUtil.GetSlmDbEntities())
                {
                    int numOfDay;
                    var tmp = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "notify_premium_temp_day" && p.is_Deleted == 0).Select(p => p.slm_OptionDesc).FirstOrDefault();
                    if (!int.TryParse(tmp, out numOfDay))
                    {
                        numOfDay = 60;
                    }

                    DateTime date = DateTime.Now.AddDays(-numOfDay);
                    var strDate = date.Year.ToString() + date.ToString("-MM-dd");
                    string sql = $"DELETE FROM kkslm_tr_notify_premium_temp WHERE slm_CreatedDate < '{strDate}' ";
                    slmdb.ExecuteStoreCommand(sql);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                string errorDetail = "Purge data in kkslm_tr_notify_premium_temp failed, Error=" + message;
                Util.WriteLogFile(logfilename, BatchCode, errorDetail);
                BizUtil.InsertLog(BatchMonitorId, "", "", errorDetail);
            }
        }

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //set null
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
