using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using SLMBatch.Common;
using SLMBatch.Data;
using SLMBatch.Entity;

namespace SLMBatch.Business
{
    public class OBT_PRO_27_Biz : ServiceSMSBase
    {
        public void GenerateSMS(string batchCode)
        {
            int totalRecord = 0;
            int totalSuccess = 0;
            int totalFail = 0;

            try
            {
                BatchCode = batchCode;
                BatchMonitorId = BizUtil.SetStartTime(BatchCode);

                BizUtil.CheckPrerequisite(BatchCode);

                using (SLMDBEntities slmdb = AppUtil.GetSlmDbEntities())
                {
                    List<kkslm_tr_renewinsurance> renewList = slmdb.kkslm_tr_renewinsurance
                        .Where(p => p.slm_GenSMSReceiveNo == null || p.slm_GenSMSReceiveNo == false)
                        .Where(p => p.slm_ReceiveNo != null && p.slm_ReceiveNo != "")
                        .ToList();

                    totalRecord = renewList.Count;
                    List<InsuranceCompanyData> companyList = GetInsuranceCompany(slmdb);
                    List<kkslm_ms_coveragetype> coverageList = slmdb.kkslm_ms_coveragetype.ToList();

                    foreach (kkslm_tr_renewinsurance renew in renewList)
                    {
                        string smsMessage = "";
                        try
                        {
                            renew.slm_GenSMSReceiveNo = true;
                            renew.slm_GenSMSReceiveNoDate = DateTime.Now;

                            smsMessage = InsertPrepareSMS(slmdb, renew, companyList, coverageList);
                            slmdb.SaveChanges();
                            totalSuccess += 1;

                            //Debug.WriteLine($"smsMessage: {smsMessage}");
                            //Console.WriteLine("TicketId " + renew.slm_TicketId + ": SUCCESS");
                        }
                        catch (Exception ex)
                        {
                            totalFail += 1;
                            string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            string errorDetail = string.Format("TicketId={0}, SMSMessage={1}, Error={2}", renew.slm_TicketId, smsMessage, message);

                            BizUtil.InsertLog(BatchMonitorId, renew.slm_TicketId, "", errorDetail);
                            Util.WriteLogFile(logfilename, BatchCode, errorDetail);
                        }
                    }

                    BizUtil.SetEndTime(BatchCode, BatchMonitorId, AppConstant.Success, totalRecord, totalSuccess, totalFail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("All FAIL");
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Util.WriteLogFile(logfilename, BatchCode, message);
                BizUtil.InsertLog(BatchMonitorId, "", "", message);

                totalSuccess = 0;
                totalFail = totalRecord;

                BizUtil.SetEndTime(BatchCode, BatchMonitorId, AppConstant.Fail, totalRecord, totalSuccess, totalFail);
            }
        }

        private List<InsuranceCompanyData> GetInsuranceCompany(SLMDBEntities slmdb)
        {
            try
            {
                string sql = @"SELECT slm_Ins_Com_Id AS InsComId, slm_InsCode AS InsComCode, slm_InsNameTh AS InsNameTH, slm_TelContact AS TelContact
                                FROM " + AppConstant.OPERDBName + ".dbo.kkslm_ms_ins_com";

                return slmdb.ExecuteStoreQuery<InsuranceCompanyData>(sql).ToList();
            }
            catch
            {
                //Console.WriteLine(ex);
                throw;
            }
        }

        private string InsertPrepareSMS(SLMDBEntities slmdb, kkslm_tr_renewinsurance renew, List<InsuranceCompanyData> companyList, List<kkslm_ms_coveragetype> coverageList)
        {
            try
            {
                if (string.IsNullOrEmpty(AppConstant.SMSTemplatePathReceiveNo))
                    throw new Exception($"ไม่พบ Config {nameof(AppConstant.SMSTemplatePathReceiveNo)} ใน Configuration File");

                InsuranceCompanyData insuranceCompany = companyList.FirstOrDefault(p => p.InsComId == renew.slm_InsuranceComId);
                if (insuranceCompany == null)
                    throw new Exception("ไม่พบข้อมูลบริษัทประกัน Id " + renew.slm_InsuranceComId + " ในระบบ");

                kkslm_ms_coveragetype coverage = coverageList.FirstOrDefault(p => p.slm_CoverageTypeId == renew.slm_CoverageTypeId);
                if (coverage == null)
                    throw new Exception("ไม่พบข้อมูลประเภทประกัน Id " + renew.slm_CoverageTypeId + " ในระบบ");

                kkslm_tr_lead lead = slmdb.kkslm_tr_lead.FirstOrDefault(p => p.slm_ticketId == renew.slm_TicketId);
                if (lead == null)
                    throw new Exception("ไม่พบข้อมูล Lead ในระบบ, TicketId " + renew.slm_TicketId);

                string telNoSms = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId == lead.slm_ticketId).Select(p => p.slm_TelNoSms).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(telNoSms))
                {
                    telNoSms = lead.slm_TelNo_1;
                }

                string template = File.ReadAllText(AppConstant.SMSTemplatePathReceiveNo, Encoding.UTF8);
                string smsMessage = template
                    .Replace("%CoverageType%", coverage.slm_ConverageTypeName?.Trim() ?? "")
                    .Replace("%CarLicenseNo%", renew.slm_LicenseNo?.Trim() ?? "")
                    .Replace("%InsuranceCompany%", insuranceCompany.InsNameTH?.Trim() ?? "")
                    .Replace("%ReceiveNo%", renew.slm_ReceiveNo?.Trim() ?? "")
                    .Replace("%InsuranceTelNo%", insuranceCompany.TelContact?.Trim() ?? "");

                //kkslm_tr_prelead_prepare_sms sms = InitSMS(smsMessage, telNoSms, renew.slm_TicketId, _dbNow);
                var sms = new kkslm_tr_prelead_prepare_sms
                {
                    slm_ticketId = renew.slm_TicketId,
                    slm_Prelead_Id = null,
                    slm_Message = smsMessage,
                    slm_Message_CreatedBy = "SYSTEM",
                    slm_MessageStatusId = "1",
                    slm_PhoneNumber = telNoSms,
                    slm_QueueId = "6",
                    slm_RequestDate = DateTime.Now,
                    slm_RuleActivityId = 0,
                    slm_ExportStatus = "0",
                    slm_RefId = null,
                    slm_SendingStatusCode = null,
                    slm_SendingStatus = null,
                    slm_ErrorCode = null,
                    slm_ErrorReason = null,
                    slm_CAS_Flag = null,
                    slm_CAS_Date = null,
                    slm_FlagType = "1",
                    slm_CreatedBy = "SYSTEM",
                    slm_CreatedDate = DateTime.Now,
                    slm_UpdatedBy = "SYSTEM",
                    slm_UpdatedDate = DateTime.Now,
                    is_Deleted = 0
                };
                slmdb.kkslm_tr_prelead_prepare_sms.AddObject(sms);
                return smsMessage;
            }
            catch
            {
                throw;
            }
        }
    }
}
