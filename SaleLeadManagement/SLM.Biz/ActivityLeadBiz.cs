using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;
using SLM.Dal;
using SLM.Resource;
using System.Globalization;

namespace SLM.Biz
{
    public class ActivityLeadBiz
    {
        public decimal? SettleClaimReportId { get; set; }

        public static List<ProblemDetailData> getProblem(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.getProblem(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static PreleadData GetPreleadbyTicket(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPreleadbyTicket(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }
        public static RenewInsuranceData GetRenewInsurance(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetRenewInsurance(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static PreleadCompareData GetPreleadCompareById(string prelead)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPreleadCompareById(prelead);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }
        public static PreleadCompareActData GetPreleadCompareActById(string prelead)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPreleadCompareActById(prelead);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static LeadDataForRenewInsure GetLeadbyTicketId(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetLeadbyTicketId(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }
        public static DateTime? GetStatusDateByTicketId(string ticketID)
        {
            ActivityLeadModel lead = new ActivityLeadModel();
            try
            {
                return lead.GetStatusDateByTicketId(ticketID);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }
        public static List<LeadDataForRenewInsure> GetLeads(string ticketId, string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetLeads(ticketId, username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }
        public static PreleadCompareActData GetPreleadtoCompareAct(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPreleadtoCompareAct(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<PreleadCompareData> GetLeadCompare(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetLeadCompare(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<PreleadCompareActData> GetLeadCompareAct(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetLeadCompareAct(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static PreleadCompareData GetPreleadtoCompare(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPreleadtoCompare(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static PreleadAddressData GetPreleadAddress(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPreleadAddress(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static PreleadCompareData GetNotifyPremium(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetNotifyPremium(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }


        public static List<RenewInsurancePaymentMainData> GetRenewInsurancePaymentMain(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.getRenewInsurancePaymentMain(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }


        public static int GetResultActBuy(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetResultActBuy(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static decimal? GetRecAmount(string ticketId, string paymentCode)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetRecAmount(ticketId, paymentCode);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static string GetPhonecallContent(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPhonecallLogContent(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static decimal? GetDiscountPercentInConfig(string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetDiscountPercentInConfig(username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<PaymentTransMainData> GetPaymentTransMain(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPaymentTransMain(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<PaymentTransMainData> GetPreLeadToPaymentTransMain(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPreLeadToPaymentTransMain(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<PaymentTransData> GetPaymentTrans(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPaymentTrans(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<PaymentTransData> GetPreLeadToPaymentTrans(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPreLeadToPaymentTrans(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }


        public static List<RenewInsuranceReceiptData> GetReceipt(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetReceipt(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<RenewInsuranceReceiptDetailData> GetImportList(string id)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetImportList(id);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<RenewInsuranceReceiptDetailData> getReceiptDetail(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.getReceiptDetail(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<RenewInsuranceReceiptRevisionDetailData> getReceiptRevisionDetail(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.getReceiptRevisionDetail(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public void SaveData(PreleadCompareDataCollection data, bool PolicyPaymentMainFlag, bool PolicyPaymentMainActFlag, BPReportData savebp, string claimFlag, string userName, bool fromReceiveClick)
        {
            ActivityLeadModel lead = null;
            try
            {
                DateTime? oldStatusDate = GetStatusDateByTicketId(data.RenewIns.slm_TicketId);
                lead = new ActivityLeadModel();
                kkslm_tr_activity actForSLA = null;
                lead.SaveData(data, PolicyPaymentMainFlag, PolicyPaymentMainActFlag, savebp, claimFlag, userName, out actForSLA, fromReceiveClick);
                SettleClaimReportId = lead.SettleClaimReportId;

                if (actForSLA != null) { 
                    // CalTotalSLA
                    using (SLM_DBEntities slmdb = new SLM_DBEntities())
                    {
                        var ld = slmdb.kkslm_tr_lead.FirstOrDefault(l => l.slm_ticketId == data.RenewIns.slm_TicketId);
                        var act = slmdb.kkslm_tr_activity.FirstOrDefault(a => a.slm_ActivityId == actForSLA.slm_ActivityId);
                        RenewInsureBiz slaBiz = new RenewInsureBiz();
                        slaBiz.CalculateTotalSLA(slmdb, ld, act, actForSLA.slm_UpdatedDate.Value, oldStatusDate);
                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }


        public static bool CheckOwnerOrDelegate(string ticketId, string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.CheckOwnerOrDelegate(ticketId, username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static bool CheckHeaderOwnerOrDelegate(string ticketId, string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.CheckHeaderOwnerOrDelegate(ticketId, username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static bool CheckSupervisor(string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.CheckSupervisor(username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        //public static bool CheckReceipt(RenewInsuranceData data, decimal? policyRecAmt)
        //{
        //    ActivityLeadModel lead = null;
        //    try
        //    {
        //        lead = new ActivityLeadModel();
        //        return lead.CheckReceipt(data, policyRecAmt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (lead != null)
        //            lead.Dispose();
        //    }
        //}

        public static string getReceiveNo(string ticketId, string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.getReceiveNo(ticketId, username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static void DoProcessIncentivePolicy(PreleadCompareDataCollection data, string createBy, DateTime createdDate)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();

                // -- ย้ายจาก model ออกมา
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var renew = data.RenewIns;
                    renew.slm_UpdatedBy = createBy;                    

                    var obj = sdc.kkslm_tr_renewinsurance.Where(r => r.slm_TicketId == renew.slm_TicketId).FirstOrDefault();

                    if (obj.slm_IncentiveFlag == null ? true : !obj.slm_IncentiveFlag.Value)
                    {
                        var ld = sdc.kkslm_tr_lead.Where(l => l.slm_ticketId == obj.slm_TicketId).FirstOrDefault();
                        bool incentiveFlag = lead.CalculateIncentivePolicy(obj, createdDate);

                        if (incentiveFlag)
                        {
                            data.RenewIns.slm_IncentiveFlag = incentiveFlag;

                            var lastAct = lead.InsertLog(renew, createBy, SLM.Resource.SLMConstant.ActionType.ChangeStatus, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);
                            RenewInsureBiz slaBiz = new RenewInsureBiz();
                            slaBiz.CalculateTotalSLA(sdc, ld, lastAct, createdDate);
                            lead.UpdateLeadIncentive(renew, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);

                            lead.InsertLog(renew.slm_TicketId, createBy, SLM.Resource.SLMConstant.ActionType.GetIncentiveIns, null, null, false);
                            lead.insertPhoneCall(data, lastAct);
                            lead.InsertHistory(lastAct);
                        }
                    }
                    sdc.SaveChanges();
                }
                // --
                //lead.DoProcessIncentivePolicy(data, createBy, createdDate);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static void DoProcessIncentiveAct(PreleadCompareDataCollection data, string createBy, DateTime createdDate)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                var act =lead.DoProcessIncentiveAct(data, createBy, createdDate);
                RenewInsureBiz slaBiz = new RenewInsureBiz();
                if (act != null)
                {
                    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                    {
                        var ll = slmdb.kkslm_tr_lead.FirstOrDefault(l => l.slm_ticketId == data.RenewIns.slm_TicketId);
                        var act2 = slmdb.kkslm_tr_activity.FirstOrDefault(aa => aa.slm_ActivityId == act.slm_ActivityId);
                        slaBiz.CalculateTotalSLA(slmdb, ll, act2, DateTime.Now);
                        slmdb.SaveChanges();
                    }
                }
                // SlmScr004Biz.InsertActIssueReport(data.RenewIns.slm_TicketId, DateTime.Now, createBy);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static tempData checkPolicyVat(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.checkPolicyVat(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        //public static tempData checkActVat(string ticketId)
        //{
        //    ActivityLeadModel lead = null;
        //    try
        //    {
        //        lead = new ActivityLeadModel();
        //        return lead.checkActVat(ticketId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (lead != null)
        //            lead.Dispose();
        //    }
        //}

        public static void SaveReceivePolicy(PreleadCompareDataCollection data, string crateBy)
        {
            ActivityLeadModel lead = null;
            var oldStatusDate = GetStatusDateByTicketId(data.RenewIns.slm_TicketId);
            try
            {
                lead = new ActivityLeadModel();
                var act = lead.SaveReceivePolicy(data, crateBy);
                RenewInsureBiz slaBiz = new RenewInsureBiz();
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var ll = slmdb.kkslm_tr_lead.FirstOrDefault(l => l.slm_ticketId == data.RenewIns.slm_TicketId);
                    var act2 = slmdb.kkslm_tr_activity.FirstOrDefault(aa => aa.slm_ActivityId == act.slm_ActivityId);
                    slaBiz.CalculateTotalSLA(slmdb, ll, act2, DateTime.Now, oldStatusDate);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static void UpdateActNotifyReport(string receiveNo, string ticketId)
        {
            if (!string.IsNullOrEmpty(ticketId))
            {
                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    sdc.ExecuteStoreCommand(string.Format("update kkslm_tr_notify_act_report set slm_InsReceiveNo = '{0}' where slm_TicketId = '{1}' and is_Deleted = 0 and (slm_Export_Flag = 0 OR slm_Export_Flag is null)", receiveNo, ticketId));
                }
            }
        }

        public static void UpdateSettleClaimReport(string receiveNo, DateTime receiveDate, string ticketId, decimal? settleClaimReportId)
        {
            if (settleClaimReportId != null)
            {
                using (SLM_DBEntities slmdb = new SLM_DBEntities())
                {
                    var report = slmdb.kkslm_tr_setle_claim_report.Where(p => p.slm_Id == settleClaimReportId).FirstOrDefault();
                    if (report != null && report.slm_TicketId == ticketId)
                    {
                        report.slm_InsReceiveNo = receiveNo;
                        report.slm_InsReceiveDate = receiveDate;
                    }
                    slmdb.SaveChanges();
                }
            }
        }

        public static void SaveReceiveAct(PreleadCompareDataCollection data, string crateBy)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                lead.UpdateRenewinsuranceReceiveAct(data.RenewIns);

                using (SLM_DBEntities slmdb = new SLM_DBEntities())
                {
                    string sql = @"SELECT COUNT(compare.slm_RenewInsureCompareId)
                                    FROM " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance renew
                                    INNER JOIN " + SLMConstant.SLMDBName + @".dbo.kkslm_tr_renewinsurance_compare compare ON compare.slm_RenewInsureId = renew.slm_RenewInsureId
                                    WHERE renew.slm_TicketId = '" + data.RenewIns.slm_TicketId + "' AND compare.slm_Selected = 1 ";

                    int countPolicy = slmdb.ExecuteStoreQuery<int>(sql).FirstOrDefault();
                    kkslm_tr_activity act = null;
                    if (countPolicy == 0)
                    {
                        // Get Last Date of Status Befor Update 
                        var oldStatusDate = GetStatusDateByTicketId(data.RenewIns.slm_TicketId);
                        act = lead.InsertLog(data.RenewIns, crateBy, SLM.Resource.SLMConstant.ActionType.ChangeStatus, SLM.Resource.SLMConstant.StatusCode.OnProcess, SLM.Resource.SLMConstant.SubStatusCode.InformAct);
                        lead.UpdateLeadIncentive(data.RenewIns, SLM.Resource.SLMConstant.StatusCode.OnProcess, SLM.Resource.SLMConstant.SubStatusCode.InformAct);
                        lead.InsertHistory(act);

                        // SLA
                        var ld = slmdb.kkslm_tr_lead.FirstOrDefault(l => l.slm_ticketId == data.RenewIns.slm_TicketId);
                        RenewInsureBiz slaBiz = new RenewInsureBiz();
                        slaBiz.CalculateTotalSLA(slmdb, ld, act, act.slm_UpdatedDate.Value,oldStatusDate);
                        slmdb.SaveChanges();                        
                    }
                    // ซื้อประกัน + พรบ จะไม่เปลี่ยนสถานะให้ => ต้องหาสถานะเก่ามาลงผลการติดต่อ
                    if (act == null)
                    {
                        act = new kkslm_tr_activity();
                        var prevStatus = slmdb.kkslm_tr_activity.Where(l => l.slm_TicketId == data.RenewIns.slm_TicketId && l.slm_NewStatus != null).OrderByDescending(l => l.slm_CreatedDate).FirstOrDefault();
                        var lastStatusLead = slmdb.kkslm_tr_lead.Where(e => e.slm_ticketId == data.RenewIns.slm_TicketId).Select(e => e.slm_Status).FirstOrDefault();
                        if (prevStatus != null)
                        {                            
                            act.slm_NewSubStatus = prevStatus.slm_NewStatus;
                            act.slm_OldStatus = prevStatus.slm_OldStatus;
                            act.slm_OldSubStatus = prevStatus.slm_OldSubStatus;
                            act.slm_ExternalSubStatus_New = prevStatus.slm_ExternalSubStatus_New;
                        }
                        act.slm_NewStatus = lastStatusLead; //prevStatus.slm_NewStatus;
                        act.slm_CreatedBy = crateBy;
                        act.slm_CreatedBy_Position = lead.GetPositionId(crateBy);
                    }
                    // ถ้าซื้อประกัน+พรบ ถ้ากดส่งแจ้งพรบ ให้ลง car , บันทึกผลการติดต่อ ด้วย
                    lead.insertPhoneCall(data, act);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static void SavePolicyIncentiveData(RenewInsuranceData data, string createBy)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                lead.SavePolicyIncentiveData(data, createBy);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }
        public static void SaveActIncentiveData(RenewInsuranceData data, string createBy)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                lead.SaveActIncentiveData(data, createBy);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static void CancelPolicy(PreleadCompareDataCollection data)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                lead.CancelPolicy(data);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static void CancelAct(PreleadCompareDataCollection data)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                lead.CancelAct(data);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static int? getDiscountPercent(string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.getDiscountPercent(username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static bool CheckDiscount(string username, decimal netamount, decimal total_dc_bath, decimal total_dc_percent, string discountType, out string errTxt, string ticketIdOrPrelead = null)
        {
            errTxt = "";
            var typ = discountType == "205" ? " พรบ " : "ประกัน";
            var renew = ticketIdOrPrelead != null ? GetRenewInsurance(ticketIdOrPrelead) : null;
            bool ret = true;
            try
            {
                //if (total_dc_bath != 0 || total_dc_percent != 0)
                //{
                    
                //}

                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var dc = (from s in sdc.kkslm_ms_staff
                              join d in sdc.kkslm_ms_discount on new
                              {
                                  staffType = (decimal?)s.slm_StaffTypeId,
                                  insureType = discountType,
                                  isDeleted = (bool?)false
                              }
                              equals new
                              {
                                  staffType = d.slm_StaffTypeId,
                                  insureType = d.slm_InsuranceTypeCode,
                                  isDeleted = d.is_Deleted
                              }
                              where s.slm_UserName == username
                              select d).FirstOrDefault();

                    if (renew == null)
                    {
                        decimal? oldDiscount = 0;
                        // Prelead
                        if (discountType == "204")
                        {
                            var oldPreledCompare = GetPreleadCompareById(ticketIdOrPrelead);
                            if (oldPreledCompare != null)
                            {
                                oldDiscount = oldPreledCompare.slm_DiscountBath.HasValue ? oldPreledCompare.slm_DiscountBath : 0;
                            }
                        }
                        else
                        {
                            var oldPreledCompareAct = GetPreleadCompareActById(ticketIdOrPrelead);
                            if (oldPreledCompareAct != null)
                            {
                                oldDiscount = oldPreledCompareAct.slm_DiscountBath.HasValue ? oldPreledCompareAct.slm_DiscountBath : 0;
                            }
                        }

                        if (oldDiscount != total_dc_bath)
                        {
                            if (dc == null && total_dc_bath == 0 && total_dc_percent == 0) // Edit 20170817
                            {
                                //ถ้าเป็นกรณี ไม่มีสิทธิ์กำหนดส่วนลดและ ส่วนลด(%) ส่วนลด(บาท) เป็น0 ไม่ต้อง validate
                            }
                            else
                            {
                                if (dc == null)
                                {
                                    throw new Exception("ไม่มีสิทธิ์กรอกส่วนลด " + typ);
                                }
                                CheckDiscount(dc, total_dc_bath, total_dc_percent, netamount, oldDiscount, typ);
                            }
                        }
                    }
                    else
                    {
                        // Lead
                        var oldDiscount_Lead = discountType == "205" ? (renew.slm_ActDiscountAmt != null ? renew.slm_ActDiscountAmt.Value : 0) : (renew.slm_PolicyDiscountAmt != null ? renew.slm_PolicyDiscountAmt.Value : 0);
                        if (oldDiscount_Lead != total_dc_bath)
                        {
                            if (dc == null && total_dc_bath == 0 && total_dc_percent == 0) // Edit 20170817
                            {
                                //ถ้าเป็นกรณี ไม่มีสิทธิ์กำหนดส่วนลดและ ส่วนลด(%) ส่วนลด(บาท) เป็น0 ไม่ต้อง validate
                            }
                            else
                            {
                                if (dc == null)
                                {
                                    throw new Exception("ไม่มีสิทธิ์กรอกส่วนลด " + typ);
                                }
                                CheckDiscount(dc, total_dc_bath, total_dc_percent, netamount, oldDiscount_Lead, typ);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                errTxt = ex.Message;
                return ret;
            }
            return ret;
        }

        #region Backup 15/05/2017
        //public static bool CheckDiscount(string username, decimal netamount, decimal total_dc_bath, decimal total_dc_percent, string discountType, out string errTxt ,string ticketIdOrPrelead = null)
        //{
        //    errTxt = "";
        //    var typ = discountType == "205" ? " พรบ " : "ประกัน";
        //    var renew = ticketIdOrPrelead != null ? GetRenewInsurance(ticketIdOrPrelead) : null;
        //    bool ret = true;
        //    try
        //    {
        //        if (total_dc_bath != 0 || total_dc_percent != 0)
        //        {
        //            using (SLM_DBEntities sdc = new SLM_DBEntities())
        //            {
        //                var dc = (from s in sdc.kkslm_ms_staff
        //                          join d in sdc.kkslm_ms_discount on new
        //                          {
        //                              staffType = (decimal?)s.slm_StaffTypeId,
        //                              insureType = discountType,
        //                              isDeleted = (bool?)false
        //                          } 
        //                          equals new
        //                          {
        //                              staffType = d.slm_StaffTypeId,
        //                              insureType = d.slm_InsuranceTypeCode,
        //                              isDeleted = d.is_Deleted
        //                          }
        //                          where s.slm_UserName == username
        //                          select d).FirstOrDefault();
        //                if (dc == null)
        //                {
        //                    throw new Exception("ไม่มีสิทธิ์กรอกส่วนลด " + typ);
        //                }
        //                else
        //                {
        //                    if (renew == null)
        //                    {
        //                        decimal? oldDiscount = 0;
        //                        // Prelead
        //                        if (discountType == "204")
        //                        {
        //                           var oldPreledCompare = GetPreleadCompareById(ticketIdOrPrelead);
        //                           if (oldPreledCompare != null)
        //                           {
        //                               oldDiscount = oldPreledCompare.slm_DiscountBath.HasValue ? oldPreledCompare.slm_DiscountBath : 0;
        //                           }
        //                        }
        //                        else
        //                        {
        //                            var oldPreledCompareAct = GetPreleadCompareActById(ticketIdOrPrelead);
        //                            if (oldPreledCompareAct != null)
        //                            {
        //                                oldDiscount = oldPreledCompareAct.slm_DiscountBath.HasValue ? oldPreledCompareAct.slm_DiscountBath : 0;
        //                            }
        //                        }

        //                        if (oldDiscount != total_dc_bath) CheckDiscount(dc, total_dc_bath, total_dc_percent, netamount, oldDiscount, typ);
        //                        #region Move to void CheckDiscount
        //                        //    if (dc.slm_DiscountBath != null && dc.slm_DiscountBath != 0) // config เป็นแบบ บาท
        //                        //    {
        //                        //        if (total_dc_bath == 0) total_dc_bath = (total_dc_percent / 100m) * netamount;

        //                        //        if (total_dc_bath > dc.slm_DiscountBath) throw new Exception(String.Format("ส่วนลด{0}ต้องไม่เกิน {1} บาท", typ, dc.slm_DiscountBath));
        //                        //    }
        //                        //    else if (dc.slm_DiscountPercent != null && dc.slm_DiscountPercent != 0) // config เป็นแบบ เปอร์เซนต์
        //                        //    {
        //                        //        if (total_dc_percent == 0) total_dc_percent = Math.Round(Math.Round((total_dc_bath * 100) / netamount, 2, MidpointRounding.AwayFromZero), 0, MidpointRounding.AwayFromZero);

        //                        //        if (total_dc_percent > dc.slm_DiscountPercent) throw new Exception(string.Format("ส่วนลด{0}ต้องไม่เกิน {1}%", typ, dc.slm_DiscountPercent));

        //                        //    }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        // Lead
        //                        var oldDiscount_Lead = discountType == "205" ? renew.slm_ActDiscountAmt : renew.slm_PolicyDiscountAmt;
        //                        if(oldDiscount_Lead != total_dc_bath) CheckDiscount(dc, total_dc_bath, total_dc_percent, netamount, oldDiscount_Lead, typ);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ret = false;
        //        errTxt = ex.Message;
        //        return ret;
        //    }
        //    return ret;
        //}
        #endregion

        private static void CheckDiscount(kkslm_ms_discount dc, decimal total_dc_bath, decimal total_dc_percent, decimal netamount, decimal? oldDiscoint_Prelead, string typ)
        {
            if (dc.slm_DiscountBath != null && dc.slm_DiscountBath != 0) // config เป็นแบบ บาท
            {
                if (total_dc_bath == 0) total_dc_bath = (total_dc_percent / 100m) * netamount;

                if (total_dc_bath > dc.slm_DiscountBath) throw new Exception(String.Format("ส่วนลด{0}ต้องไม่เกิน {1} บาท", typ, dc.slm_DiscountBath));
            }
            else if (dc.slm_DiscountPercent != null && dc.slm_DiscountPercent != 0) // config เป็นแบบ เปอร์เซนต์
            {
                if (total_dc_percent == 0) total_dc_percent = Math.Round(Math.Round((total_dc_bath * 100) / netamount, 2, MidpointRounding.AwayFromZero), 0, MidpointRounding.AwayFromZero);

                if (total_dc_percent > dc.slm_DiscountPercent) throw new Exception(string.Format("ส่วนลด{0}ต้องไม่เกิน {1}%", typ, dc.slm_DiscountPercent));
            }
        }

        public static bool checkSaveButton(string Product_Id, string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.checkSaveButton(Product_Id, username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }


        public static bool checkProblem(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.checkProblem(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static List<ControlListData> getCancelList()
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.getCancelList();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static bool checkPaid(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.checkPaidPolicy(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }


        public static decimal GetPaidPolicyAmount(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPaidPolicyAmount(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static bool checkPaidPolicy(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.checkPaidPolicy(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static decimal GetPaidActAmount(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.GetPaidActAmount(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static bool checkPaidAct(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.checkPaidAct(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static bool CheckSettleClaimReported(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.CheckSettleClaimReported(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static void insertkkslm_tr_setle_claim(string ticketId, string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                lead.insertkkslm_tr_setle_claim(ticketId, username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }

        }

        public static kkslm_tr_prelead_prepare_sms InsertPrepareSMS(string smsTemplate, PreleadCompareDataCollection data)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                var sms = lead.InsertPrepareSMS(smsTemplate, data);
                return sms;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static void InsertSettleClaimActivityLog(PreleadCompareDataCollection data, string username)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                lead.InsertSettleClaimActivityLog(data, username);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static string GetSubScriptionTypeId(int cardTypeId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var subId = slmdb.kkslm_ms_cardtype.Where(p => p.slm_CardTypeId == cardTypeId && p.is_Deleted == false).Select(p => p.slm_CIFSubscriptTypeId).FirstOrDefault();
                    return subId != null ? subId : "0";
                }
            }
            catch
            {
                throw;
            }
        }

        public static void UpdateLeadWaitDocStatus(RenewInsuranceData renew)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                lead.UpdateLeadIncentive(renew, SLM.Resource.SLMConstant.StatusCode.WaitConsider, SLM.Resource.SLMConstant.SubStatusCode.WaitDocIns);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }
        #region Tuning

        public void GetLeadMainData(string ticketId, PreleadCompareDataCollection PreLeadDataCollectionMain)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
                    {
                        ActivityLeadModel lead = new ActivityLeadModel(slmdb, operdb);
                        PreLeadDataCollectionMain.lead = lead.GetLeadbyTicketId(ticketId);
                        PreLeadDataCollectionMain.Prelead = lead.GetPreleadbyTicket(ticketId);

                        RenewInsuranceData RenewIns = lead.GetRenewInsurance(ticketId);
                        PreLeadDataCollectionMain.RenewIns = RenewIns;
                        PreLeadDataCollectionMain.keyTab = RenewIns.slm_TicketId.ToString();

                        List<PreleadCompareData> preleadcompare = lead.GetLeadCompare(ticketId);
                        List<PreleadCompareActData> preleadcompareact = lead.GetLeadCompareAct(ticketId);

                        List<RenewInsurancePaymentMainData> PayMainAllList = lead.getRenewInsurancePaymentMain(ticketId);
                        PreLeadDataCollectionMain.PayMainList = PayMainAllList.Where(p => p.slm_Type == "1").ToList();
                        PreLeadDataCollectionMain.PayMainActList = PayMainAllList.Where(p => p.slm_Type == "1").ToList();

                        PreLeadDataCollectionMain.PaymentTransMainList = lead.GetPaymentTransMain(ticketId);
                        PreLeadDataCollectionMain.PaymentTransList = lead.GetPaymentTrans(ticketId);
                        PreLeadDataCollectionMain.Address = lead.GetPreleadAddress(ticketId);
                        PreLeadDataCollectionMain.PolicyRecAmt = lead.GetRecAmount(ticketId, "204");
                        PreLeadDataCollectionMain.ActRecAmt = lead.GetRecAmount(ticketId, "205");
                        PreLeadDataCollectionMain.ReceiptList = lead.GetReceipt(ticketId);
                        PreLeadDataCollectionMain.ProblemList = lead.getProblem(ticketId);
                        PreLeadDataCollectionMain.ReceiptDetailList = lead.getReceiptDetail(ticketId);
                        PreLeadDataCollectionMain.ReceiptRevisionDetailList = lead.getReceiptRevisionDetail(ticketId);

                        if (preleadcompare.Count == 0)
                        {
                            PreLeadDataCollectionMain.ComparePrev = lead.GetPreleadtoCompare(ticketId);
                            PreLeadDataCollectionMain.CompareCurr = lead.GetNotifyPremium(ticketId);

                            if (PreLeadDataCollectionMain.RenewIns != null && string.IsNullOrEmpty(PreLeadDataCollectionMain.RenewIns.slm_RemarkPolicy))
                                PreLeadDataCollectionMain.RenewIns.slm_RemarkPolicy = lead.GetNotifyPremiumRemark(PreLeadDataCollectionMain.RenewIns.slm_ChassisNo);

                            if (PreLeadDataCollectionMain.ComparePrev != null)
                            {
                                if (PreLeadDataCollectionMain.ComparePrev.slm_PolicyEndCoverDate != null)
                                {
                                    if (PreLeadDataCollectionMain.CompareCurr != null)
                                    {
                                        PreLeadDataCollectionMain.CompareCurr.slm_PolicyStartCoverDate = PreLeadDataCollectionMain.ComparePrev.slm_PolicyEndCoverDate;
                                        PreLeadDataCollectionMain.CompareCurr.slm_PolicyEndCoverDate = PreLeadDataCollectionMain.ComparePrev.slm_PolicyEndCoverDate.Value.AddYears(1);
                                    }
                                }
                            }

                            PreLeadDataCollectionMain.ComparePromoList = new List<PreleadCompareData>();
                        }
                        else
                        {
                            PreLeadDataCollectionMain.ComparePrev = preleadcompare.Where(s => s.slm_Seq == 1).FirstOrDefault();
                            PreLeadDataCollectionMain.CompareCurr = preleadcompare.Where(s => s.slm_Seq == 2).FirstOrDefault();
                            PreLeadDataCollectionMain.ComparePromoList = preleadcompare.Where(s => s.slm_Seq != 2 && s.slm_Seq != 1).OrderBy(p => p.slm_Seq).ToList();
                        }

                        if (preleadcompareact.Count == 0)
                        {
                            PreLeadDataCollectionMain.ActPrev = lead.GetPreleadtoCompareAct(ticketId);
                            PreLeadDataCollectionMain.ActPromoList = new List<PreleadCompareActData>();
                        }
                        else
                        {
                            PreLeadDataCollectionMain.ActPrev = preleadcompareact.Where(s => s.slm_Seq == 1).FirstOrDefault();
                            PreLeadDataCollectionMain.ActPromoList = preleadcompareact.Where(s => s.slm_Seq != 1).OrderBy(p => p.slm_Seq).ToList();
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        public static bool IsTeamBoss(string staffId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.IsTeamBoss(staffId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        public static bool CheckPolicyPurchased(string ticketId)
        {
            ActivityLeadModel lead = null;
            try
            {
                lead = new ActivityLeadModel();
                return lead.CheckPolicyPurchased(ticketId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }

        /// <summary>
        /// Convert string date format "dd/MM/yyyy HH:mm:ss" to DateTime style en-US
        /// </summary>
        /// <param name="date"></param>
        /// <returns> Date Time</returns>
        public static DateTime? ConvertStrDateTimeToDateTime(string StrDate)
        {
            if (!String.IsNullOrEmpty(StrDate))
                return DateTime.ParseExact(StrDate, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
            else return null;
        }

        public bool CheckActPurchaseDateTimeWithActIssueReport(string ticketId, DateTime startCoverDate, out string errorMessage)
        {
            ActivityLeadModel lead = null;
            try
            {
                errorMessage = "";
                lead = new ActivityLeadModel();
                return lead.CheckActPurchaseDateTimeWithActIssueReport(ticketId, startCoverDate, out errorMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lead != null)
                {
                    lead.Dispose();
                }
            }
        }
    }
}
