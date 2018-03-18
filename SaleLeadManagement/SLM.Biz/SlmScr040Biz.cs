using System;
using System.Collections.Generic;
using System.Linq;
using SLM.Dal;
using SLM.Resource.Data;
using System.Transactions;
using System.Data.SqlClient;

namespace SLM.Biz
{
    public class SlmScr040Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        public List<TitleData> GetTitleList()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return (from tt in sdc.kkslm_ms_title.Where(t => t.is_Deleted == false)
                        select new TitleData() { ID = tt.slm_TitleId, Name = tt.slm_TitleName }).ToList();
            }
        }
        public List<CompData> GetCompanyNameList()
        {
            using (OPERDBEntities odc = DBUtil.GetOperDbEntities())
            {
                return odc.kkslm_ms_ins_com.Select(c => new CompData() { ID = c.slm_Ins_Com_Id, Name = c.slm_InsNameTh, IsDeleted = c.is_Deleted }).ToList();
            }
        }

        public List<RepairTypeData> GetRepairTypeNameList()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_repairtype.Select(r => new RepairTypeData() { RepairTypeID = r.slm_RepairTypeId, RepairTypeName = r.slm_RepairTypeName }).ToList();
            }
        }

        public List<CoverageTypeData> GetCoverageTypeNameList()
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                return sdc.kkslm_ms_coveragetype.Select(r => new CoverageTypeData() { CoverageTypeID = r.slm_CoverageTypeId, CoverageTypeName = r.slm_ConverageTypeName, IsDeleted = r.is_Deleted }).ToList();
            }
        }

        public bool InsertNotifyPremiumData(List<notifypremiumdata> lst, string userID)
        {
            _error = "";
            bool ret = true;
            try
            {
                using (OPERDBEntities op = DBUtil.GetOperDbEntities())
                {
                    var insComLists = op.kkslm_ms_ins_com.ToList();
                    using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                    {
                        var lstRepair = sdc.kkslm_ms_repairtype.ToList();


                        foreach (var itm in lst)
                        {
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

                            np.slm_UpdatedBy = userID;
                            np.slm_UpdatedDate = DateTime.Now;
                            np.slm_CreatedBy = userID;
                            np.slm_CreatedDate = DateTime.Now;

                            sdc.kkslm_tr_notify_premium.AddObject(np);

                            // CR 3013-00233 @ 2016-06-27
                            sdc.SaveChanges();

                            string cleansingVin = np.slm_VIN.Replace(" ", "").Replace("-", "").Replace("*", "").Replace("_", "").Replace("\\", "").Replace("/", "");

                            var rcp = (from lead in sdc.kkslm_tr_lead
                                       join ins in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
                                       join cmp in sdc.kkslm_tr_renewinsurance_compare on ins.slm_RenewInsureId equals cmp.slm_RenewInsureId
                                       where cmp.slm_Seq == 2 && lead.is_Deleted == 0 && ins.slm_ReceiveNo == null && ins.slm_ReceiveDate == null && ins.slm_ChassisNo.Replace(" ", "").Replace("-", "").Replace("*", "").Replace("_", "").Replace("\\", "").Replace("/", "") == cleansingVin
                                            && ins.slm_PeriodMonth == np.slm_PeriodMonth && ins.slm_PeriodYear == np.slm_PeriodYear
                                       select cmp).FirstOrDefault();

                            if (rcp != null)
                            {
                                rcp.slm_NotifyPremiumId = np.slm_Id;
                                if (np.slm_InsuranceComId != null) rcp.slm_Ins_Com_Id = np.slm_InsuranceComId.Value;
                                rcp.slm_RepairTypeId = np.slm_RepairTypeId;
                                if (np.slm_InsuranceCarTypeId != null) rcp.slm_CoverageTypeId = np.slm_InsuranceCarTypeId.Value;
                                rcp.slm_OD = np.slm_Sum_Insure;
                                rcp.slm_PolicyGrossPremium = np.slm_NetPremium;
                                rcp.slm_PolicyGrossStamp = np.slm_Stamp;
                                rcp.slm_PolicyGrossVat = np.slm_Vat_Amount;
                                rcp.slm_NetGrossPremium = np.slm_GrossPremium;
                                rcp.slm_UpdatedDate = DateTime.Now;
                                rcp.slm_UpdatedBy = userID;
                                rcp.slm_DiscountPercent = null;
                                rcp.slm_DiscountBath = null;
                                rcp.slm_PolicyGrossPremiumPay = np.slm_GrossPremium;

                                var renew = sdc.kkslm_tr_renewinsurance.Where(p => p.slm_RenewInsureId == rcp.slm_RenewInsureId).FirstOrDefault();
                                if (renew != null)
                                {
                                    if (rcp.slm_Selected == true)
                                    {
                                        renew.slm_InsuranceComId = np.slm_InsuranceComId;
                                    }

                                    var insComName = insComLists.Where(p => p.slm_Ins_Com_Id == rcp.slm_Ins_Com_Id).Select(p => p.slm_InsNameTh).FirstOrDefault();
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
                                var prcp = (from plead in sdc.kkslm_tr_prelead
                                            join pcp in sdc.kkslm_tr_prelead_compare on plead.slm_Prelead_Id equals pcp.slm_Prelead_Id
                                            where pcp.slm_Seq == 2 && plead.is_Deleted == false && plead.slm_TicketId == null && plead.slm_Chassis_No.Replace(" ", "").Replace("-", "").Replace("*", "").Replace("_", "").Replace("\\", "").Replace("/", "") == cleansingVin
                                            && plead.slm_PeriodMonth == np.slm_PeriodMonth && plead.slm_PeriodYear == np.slm_PeriodYear
                                            select pcp).FirstOrDefault();

                                if (prcp != null)
                                {
                                    prcp.slm_NotifyPremiumId = np.slm_Id;
                                    if (np.slm_InsuranceComId != null) prcp.slm_Ins_Com_Id = np.slm_InsuranceComId.Value;
                                    prcp.slm_RepairTypeId = np.slm_RepairTypeId;
                                    if (np.slm_InsuranceCarTypeId != null) prcp.slm_CoverageTypeId = np.slm_InsuranceCarTypeId.Value;
                                    prcp.slm_OD = np.slm_Sum_Insure;
                                    prcp.slm_PolicyGrossPremium = np.slm_NetPremium;
                                    prcp.slm_PolicyGrossStamp = np.slm_Stamp;
                                    prcp.slm_PolicyGrossVat = np.slm_Vat_Amount;
                                    prcp.slm_NetGrossPremium = np.slm_GrossPremium;
                                    prcp.slm_UpdatedDate = DateTime.Now;
                                    prcp.slm_UpdatedBy = userID;
                                    prcp.slm_DiscountPercent = null;
                                    prcp.slm_DiscountBath = null;
                                    prcp.slm_PolicyGrossPremiumPay = np.slm_GrossPremium;

                                    var prelead = sdc.kkslm_tr_prelead.Where(p => p.slm_Prelead_Id == prcp.slm_Prelead_Id).FirstOrDefault();
                                    if (prelead != null)
                                    {
                                        var insComName = insComLists.Where(p => p.slm_Ins_Com_Id == prcp.slm_Ins_Com_Id).Select(p => p.slm_InsNameTh).FirstOrDefault();

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
                            // ------->
                        }
                        sdc.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                _error = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                ret = false;
            }
            return ret;
        }

        public List<ControlListData> GetMonths()
        {
            using (var slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.kkslm_ms_month.Select(p => new ControlListData { TextField = p.slm_MonthNameTh, ValueField = p.slm_MonthId }).ToList();
            }
        }

        public partial class TitleData
        {
            public int ID { get; set; }
            public string Name { get; set; }

        }

        public partial class CompData
        {
            public decimal ID { get; set; }
            public string Name { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class RepairTypeData
        {
            public int RepairTypeID { get; set; }
            public string RepairTypeName { get; set; }
        }

        public class CoverageTypeData
        {
            public int CoverageTypeID { get; set; }
            public string CoverageTypeName { get; set; }
            public bool IsDeleted { get; set; }
        }

        public partial class notifypremiumdata : kkslm_tr_notify_premium
        {
            public string cell0 { get; set; }
        }

        public bool InsertNotifyPremiumToTemp(List<notifypremiumdata> lst, string userID, string filename)
        {
            _error = "";
            bool ret = true;
            SLM_DBEntities sdc = null;
            try
            {
                List<kkslm_ms_ins_com> insComLists = null;
                using (OPERDBEntities op = DBUtil.GetOperDbEntities())
                {
                    insComLists = op.kkslm_ms_ins_com.ToList();
                }

                int count = 0;
                int commitCount = 100;

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    sdc = DBUtil.GetSlmDbEntities();
                    sdc.ExecuteStoreCommand($"DELETE FROM kkslm_tr_notify_premium_temp WHERE slm_Filename = '{filename}' AND slm_ImportFlag = '0' ");

                    foreach (var itm in lst)
                    {
                        count += 1;
                        kkslm_tr_notify_premium_temp np = new kkslm_tr_notify_premium_temp
                        {
                            slm_RunNo = itm.slm_RunNo,
                            slm_PolicyNo = itm.slm_PolicyNo,
                            slm_Title_Id = itm.slm_Title_Id,
                            slm_Name = itm.slm_Name,
                            slm_LastName = itm.slm_LastName,
                            slm_Claim_Center = itm.slm_Claim_Center,
                            slm_Claim_Garage = itm.slm_Claim_Garage,
                            slm_RepairTypeId = itm.slm_RepairTypeId,
                            slm_InsExpireDate = itm.slm_InsExpireDate,
                            slm_CarLicenseNo = itm.slm_CarLicenseNo,
                            slm_Brand = itm.slm_Brand,
                            slm_Model = itm.slm_Model,
                            slm_VIN = itm.slm_VIN,
                            slm_InsuranceCarTypeId = itm.slm_InsuranceCarTypeId, // มันคือ ประเภทประกัน ชั้น 1/2/3 (CoverageTypeId) 
                            slm_Sum_Insure = itm.slm_Sum_Insure,
                            slm_Discount_Percent = itm.slm_Discount_Percent,
                            slm_Discount_Amount = itm.slm_Discount_Amount,
                            slm_NetPremium = itm.slm_NetPremium,
                            slm_Stamp = itm.slm_Stamp,
                            slm_Vat_Amount = itm.slm_Vat_Amount,
                            slm_GrossPremium = itm.slm_GrossPremium,
                            slm_Remark = itm.slm_Remark,
                            slm_InsuranceComId = itm.slm_InsuranceComId,
                            slm_PeriodMonth = itm.slm_PeriodMonth,
                            slm_PeriodYear = itm.slm_PeriodYear,
                            slm_ActNetPremium = itm.slm_ActNetPremium,
                            slm_ActStamp = itm.slm_ActStamp,
                            slm_ActVat_Amount = itm.slm_ActVat_Amount,
                            slm_ActGrossPremium = itm.slm_ActGrossPremium,
                            slm_DriverFlag = itm.slm_DriverFlag,
                            slm_Filename = filename,
                            slm_ImportFlag = "0",
                            slm_ImportDate = null,
                            slm_UpdatedBy = userID,
                            slm_UpdatedDate = DateTime.Now,
                            slm_CreatedBy = userID,
                            slm_CreatedDate = DateTime.Now
                        };
                        sdc.kkslm_tr_notify_premium_temp.AddObject(np);

                        if (count % commitCount == 0)
                        {
                            sdc.SaveChanges();
                            sdc.Dispose();
                            sdc = DBUtil.GetSlmDbEntities();
                        }
                    }
                    sdc.SaveChanges();
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                _error = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                ret = false;
            }
            finally
            {
                if (sdc != null)
                {
                    sdc.Dispose();
                }
            }

            return ret;
        }
    }
}
