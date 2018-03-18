using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource;

namespace SLM.Biz
{
    public class ExcelTemplateBiz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }

        /// <summary>
        /// ใบเสนอราคาเปรียบเทียบ
        /// </summary>
        public List<ExcelData> GetExcelData001(string ticketID, string loginUsername)
        {
            List<ExcelData> lst = new List<ExcelData>();
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var data = (from lead in sdc.kkslm_tr_lead
                            join plead in sdc.kkslm_tr_prelead on lead.slm_ticketId equals plead.slm_TicketId into r1
                            from plead in r1.DefaultIfEmpty()
                            join tt in sdc.kkslm_ms_title on lead.slm_TitleId equals tt.slm_TitleId into r2
                            from tt in r2.DefaultIfEmpty()
                            join ri in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ri.slm_TicketId into r3
                            from ri in r3.DefaultIfEmpty()
                            join br in sdc.kkslm_ms_redbook_brand on ri.slm_RedbookBrandCode equals br.slm_BrandCode into r4
                            from br in r4.DefaultIfEmpty()
                            join mo in sdc.kkslm_ms_redbook_model on ri.slm_RedbookModelCode equals mo.slm_ModelCode into r5
                            from mo in r5.DefaultIfEmpty()
                            join stf in sdc.kkslm_ms_staff on lead.slm_Owner equals stf.slm_UserName into r6
                            from stf in r6.DefaultIfEmpty()
                            where lead.slm_ticketId == ticketID
                            select new
                            {
                                lead.slm_ticketId,
                                tt.slm_TitleName,
                                lead.slm_Name,
                                lead.slm_LastName,
                                contractnumber = ri.slm_ContractNo,
                                oldpolicyno = plead.slm_Voluntary_Policy_Number,
                                oldpolicyexpdate = plead.slm_Voluntary_Policy_Exp_Date,
                                brandname = br.slm_BrandName,
                                modelname = mo.slm_ModelName,
                                licenseno = ri.slm_LicenseNo,
                                engineno = ri.slm_ChassisNo, // ri.slm_EngineNo,
                                paymentremark = ri.slm_RemarkPayment,
                                telesalename = stf.slm_StaffNameTH,
                                positionname = stf.slm_PositionName,
                                stafftel = stf.slm_TellNo,
                                stf.slm_EmpCode,
                                ri.slm_RenewInsureId,
                                ri.slm_PolicyGrossPremiumTotal,
                                ri.slm_PolicyGrossPremium,
                                ri.slm_ActGrossPremium,
                                
                             
                            }).FirstOrDefault();

                if (data != null)
                {

                    // find staff license no
                    //Comment by Pom 16/08/2016
                    //string stafflic = "";
                    //if (!String.IsNullOrEmpty(data.slm_EmpCode))
                    //{
                    //    var lic = sdc.kkslm_ms_mapping_staff_license.Where(l => l.slm_EmpCode == data.slm_EmpCode).ToList();
                    //    foreach (var lc in lic)
                    //    {
                    //        if (stafflic != "") stafflic += ", ";
                    //        stafflic += lc.slm_LicenseNo;
                    //    }

                    //}

                    string stafflic = "";
                    string loginNameTH = "";
                    string loginPositionName = "";
                    string loginTelNo = "";

                    var loginUser = sdc.kkslm_ms_staff.Where(p => p.slm_UserName == loginUsername).FirstOrDefault();
                    if (loginUser != null)
                    {
                        loginNameTH = loginUser.slm_StaffNameTH;
                        loginTelNo = loginUser.slm_TellNo;

                        var positionName = sdc.kkslm_ms_position.Where(p => p.slm_Position_id == loginUser.slm_Position_id).Select(p => p.slm_PositionNameTH).FirstOrDefault();
                        loginPositionName = positionName != null ? positionName : "";

                        if (!string.IsNullOrEmpty(loginUser.slm_EmpCode))
                        {
                            var lic = sdc.kkslm_ms_mapping_staff_license.Where(l => l.slm_EmpCode == loginUser.slm_EmpCode).ToList();
                            foreach (var lc in lic)
                            {
                                if (stafflic != "") stafflic += ", ";
                                stafflic += lc.slm_LicenseNo;
                            }
                        }
                    }

                    lst.Add(new ExcelData() { ColumnName = "I2", Value = DateTime.Now.ToString("d/MM/yyyy") }); // printdate
                    lst.Add(new ExcelData() { ColumnName = "C3", Value = String.Format("{0}{1} {2} / {3}", data.slm_TitleName, data.slm_Name, data.slm_LastName, data.contractnumber) });
                    lst.Add(new ExcelData() { ColumnName = "B5", Value = " ด้วยกรมธรรม์ประกันรถยนต์ของท่านเลขที่ " + (data.oldpolicyno == null ? "" : data.oldpolicyno) + " จะสิ้นสุดวันที่ " + (data.oldpolicyexpdate == null ? "" : data.oldpolicyexpdate.Value.ToString("dd/MM/yyyy")) + "\r\nบริษัทฯ ขอแจ้งค่าเบี้ยประกันในการต่อประกันตามรายละเอียดดังนี้\r\n" });
                    lst.Add(new ExcelData() { ColumnName = "B9", Value = String.Format("{0}", data.brandname) });
                    lst.Add(new ExcelData() { ColumnName = "C9", Value = String.Format("{0}", data.modelname) });
                    lst.Add(new ExcelData() { ColumnName = "D9", Value = String.Format("{0}", data.licenseno) });
                    lst.Add(new ExcelData() { ColumnName = "G9", Value = String.Format("{0}", data.engineno) });
                    lst.Add(new ExcelData() { ColumnName = "E40", Value = loginNameTH });
                    lst.Add(new ExcelData() { ColumnName = "E41", Value = String.Format("{0} / {1}", loginPositionName, loginTelNo) });
                    lst.Add(new ExcelData() { ColumnName = "E42", Value = String.Format("บัตรนายหน้าเลขที่ {0}", stafflic) });

                    //lst.Add(new ExcelData() { ColumnName = "C37", Value = data.paymentremark });
                    //lst.Add(new ExcelData() { ColumnName = "E40", Value = data.telesalename });
                    //lst.Add(new ExcelData() { ColumnName = "E41", Value = String.Format("{0} / {1}", data.positionname, data.stafftel) });
                    //lst.Add(new ExcelData() { ColumnName = "E42", Value = String.Format("บัตรนายหน้าเลขที่ {0}", stafflic) });
                }

                using (OPERDBEntities opr = new OPERDBEntities())
                {

                    // ข้อมูลในตาราง
                    //var tblst = sdc.kkslm_tr_renewinsurance_compare.Where(c => c.slm_RenewInsureId == data.slm_RenewInsureId && c.slm_Seq >= 2).OrderBy(c=>c.slm_Seq).ToList();
                    
                    //var data2 = (from rc in sdc.kkslm_tr_renewinsurance_compare
                    //             join rca in sdc.kkslm_tr_renewinsurance_compare_act.Where(r => r.slm_PromotionId != 0 && r.slm_PromotionId != null) on new { rid = rc.slm_RenewInsureId, prm = rc.slm_PromotionId, seq = rc.slm_Seq } equals new { rid = rca.slm_RenewInsureId, prm = rca.slm_PromotionId, seq = rca.slm_Seq + 1 } into r0
                    //             from rca in r0.DefaultIfEmpty()
                    //             where rc.slm_RenewInsureId == data.slm_RenewInsureId && rc.slm_Seq >= 2
                    //             select new { rc, rca, renewid = rc == null ? rca.slm_RenewInsureId : rc.slm_RenewInsureId }
                    //             ).Union
                    //             (from rca in sdc.kkslm_tr_renewinsurance_compare_act
                    //              join rc in sdc.kkslm_tr_renewinsurance_compare.Where(r => r.slm_PromotionId != 0 && r.slm_PromotionId != null) on new { rid = rca.slm_RenewInsureId, prm = rca.slm_PromotionId, seq = rca.slm_Seq } equals new { rid = rc.slm_RenewInsureId, prm = rc.slm_PromotionId, seq = rc.slm_Seq - 1 } into r1
                    //              from rc in r1.DefaultIfEmpty()
                    //              where rca.slm_RenewInsureId == data.slm_RenewInsureId && rca.slm_Seq >= 2
                    //              select new { rc, rca, renewid = rc == null ? rca.slm_RenewInsureId : rc.slm_RenewInsureId }
                    //              ).OrderBy(r=>r.renewid).ToList();

                    var data2 = (from rc in sdc.kkslm_tr_renewinsurance_compare
                                 join rca in sdc.kkslm_tr_renewinsurance_compare_act.Where(r => r.slm_PromotionId != 0 && r.slm_PromotionId != null) on new { rid = rc.slm_RenewInsureId, prm = rc.slm_PromotionId } equals new { rid = rca.slm_RenewInsureId, prm = rca.slm_PromotionId } into r0
                                 from rca in r0.DefaultIfEmpty()
                                 where rc.slm_RenewInsureId == data.slm_RenewInsureId && rc.slm_Seq >= 2
                                 select new { rc, rca, renewid = rc == null ? rca.slm_RenewInsureId : rc.slm_RenewInsureId }
                                 ).Union
                                 (from rca in sdc.kkslm_tr_renewinsurance_compare_act
                                  join rc in sdc.kkslm_tr_renewinsurance_compare.Where(r => r.slm_PromotionId != 0 && r.slm_PromotionId != null) on new { rid = rca.slm_RenewInsureId, prm = rca.slm_PromotionId } equals new { rid = rc.slm_RenewInsureId, prm = rc.slm_PromotionId } into r1
                                  from rc in r1.DefaultIfEmpty()
                                  where rca.slm_RenewInsureId == data.slm_RenewInsureId && rca.slm_Seq >= 2
                                  select new { rc, rca, renewid = rc == null ? rca.slm_RenewInsureId : rc.slm_RenewInsureId }
                                  ).OrderBy(r => r.renewid).ToList();

                    //Added by Pom 17/08/2016
                    var mainList = data2.ToList();
                    mainList.Clear();
                    var compareList = data2.Where(p => p.rc != null && p.rc.slm_RenewInsureCompareId != 0).OrderBy(p => p.rc.slm_Seq).ToList();
                    var compareActList = data2.Where(p => p.rca != null && p.rca.slm_RenewInsureCompareActId != 0).OrderBy(p => p.rca.slm_Seq).ToList();
                    compareList.ForEach(p => { mainList.Add(p); });
                    compareActList.ForEach(p => { mainList.Add(p); });
                    data2 = mainList.Distinct().ToList();

                    for (int i = 0; i < 3; i++)
                    {
                        kkslm_tr_renewinsurance_compare rc;
                        kkslm_tr_renewinsurance_compare_act rca;

                        //if (tblst.Count > i)
                        //    rc = tblst[i];
                        //else
                        //    rc = new kkslm_tr_renewinsurance_compare();

                        //int? rc_seq = rc.slm_Seq - 1;
                        //var rca = sdc.kkslm_tr_renewinsurance_compare_act.Where(a => a.slm_RenewInsureId == rc.slm_RenewInsureId && a.slm_PromotionId == rc.slm_PromotionId && a.slm_Seq == rc_seq).FirstOrDefault();

                        if (data2.Count > i)
                        {
                            rc = data2[i].rc ?? new kkslm_tr_renewinsurance_compare();
                            rca = data2[i].rca;
                        }
                        else
                        {
                            rc = new kkslm_tr_renewinsurance_compare();
                            rca = null;
                        }


                        string prefix = "";
                        string prefix2 = "";
                        switch (i)
                        {
                            case 0: prefix = "E"; prefix2 = "F"; break;
                            case 1: prefix = "G"; prefix2 = "H"; break;
                            case 2: prefix = "I"; prefix2 = "J"; break;
                        }

                        //var ins = opr.kkslm_ms_ins_com.Where(c => c.slm_Ins_Com_Id == rc.slm_Ins_Com_Id).FirstOrDefault();

                        decimal insComId = 0;
                        if (rc != null && rc.slm_RenewInsureCompareId != 0)
                            insComId = rc.slm_Ins_Com_Id;
                        else if (rca != null && rca.slm_RenewInsureCompareActId != 0)
                            insComId = rca.slm_Ins_Com_Id;

                        var ins = opr.kkslm_ms_ins_com.Where(c => c.slm_Ins_Com_Id == insComId).FirstOrDefault();
                        var prm = opr.kkslm_ms_promotioninsurance.Where(p => p.slm_PromotionId == rc.slm_PromotionId).FirstOrDefault();
                        var cov = sdc.kkslm_ms_coveragetype.Where(c => c.slm_CoverageTypeId == rc.slm_CoverageTypeId).FirstOrDefault();
                        var rpr = sdc.kkslm_ms_repairtype.Where(r => r.slm_RepairTypeId == rc.slm_RepairTypeId).FirstOrDefault();

                        lst.Add(new ExcelData() { ColumnName = prefix + "14", Value = String.Format("{0}", ins == null ? "" : ins.slm_InsNameTh) });            // insname
                        lst.Add(new ExcelData() { ColumnName = prefix + "15", Value = String.Format("{0}", cov == null ? "" : cov.slm_ConverageTypeName) });             //coveragename
                        lst.Add(new ExcelData() { ColumnName = prefix + "17", Value = String.Format("{0:#,##0}", rc.slm_InjuryDeath) });             //injurydeath
                        lst.Add(new ExcelData() { ColumnName = prefix + "19", Value = String.Format("{0:#,##0}", rc.slm_TPPD) });             //tppd
                        lst.Add(new ExcelData() { ColumnName = prefix + "20", Value = String.Format("{0}", rpr == null ? "" : rpr.slm_RepairTypeName) });             //repair
                        lst.Add(new ExcelData() { ColumnName = prefix + "21", Value = String.Format("{0:#,##0}", rc.slm_OD) });       //od
                        //lst.Add(new ExcelData() { ColumnName = prefix + "22", Value = String.Format("{0:#,##0}", rc.slm_DeDuctible) });       //deductible
                        lst.Add(new ExcelData() { ColumnName = prefix + "22", Value = String.Format("{0}", rc.slm_DeDuctibleFlag) });       //deductible
                        lst.Add(new ExcelData() { ColumnName = prefix + "23", Value = String.Format("{0:#,##0}", rc.slm_FT) });       //ft
                        lst.Add(new ExcelData() { ColumnName = prefix + "25", Value = String.Format("{0:#,##0}", rc.slm_PersonalAccident) });             //accident , insuranceseat
                        lst.Add(new ExcelData() { ColumnName = prefix2 + "25", Value = String.Format("{0} ที่นั่ง", prm == null ? "" : (SLMUtil.SafeInt(prm.slm_PersonalAccidentDriver) + SLMUtil.SafeInt(prm.slm_PersonalAccidentPassenger)).ToString() /* cr 20160725rc.slm_PersonalAccidentMan */ ) });         // insuranceseat
                        lst.Add(new ExcelData() { ColumnName = prefix + "26", Value = String.Format("{0:#,##0}", rc.slm_MedicalFee) });               //medicalfee 
                        lst.Add(new ExcelData() { ColumnName = prefix2 + "26", Value = String.Format("{0} ที่นั่ง", prm == null ? "" : (SLMUtil.SafeInt(prm.slm_MedicalFeeDriver) + SLMUtil.SafeInt(prm.slm_MedicalFeePassenger)).ToString() /* cr 20150725 rc.slm_MedicalFeeMan */ ) });         // medicalseat
                        lst.Add(new ExcelData() { ColumnName = prefix + "27", Value = String.Format("{0:#,##0.00}", rc.slm_InsuranceDriver) });             //insurancedriver
                        lst.Add(new ExcelData() { ColumnName = prefix + "28", Value = String.Format("{0:#,##0.00}", rc.slm_NetGrossPremium) });             //policygrosspremiumtotal
                        lst.Add(new ExcelData() { ColumnName = prefix + "29", Value = String.Format("{0:#,##0.00}", rc.slm_PolicyGrossPremiumPay) });             //policygrosspremium
                        //lst.Add(new ExcelData() { ColumnName = prefix + "30", Value = String.Format("{0:#,##0.00}", rca == null ? 0 : rca.slm_ActNetGrossPremium) });             //actgrosspremium
                        lst.Add(new ExcelData() { ColumnName = prefix + "30", Value = String.Format("{0:#,##0.00}", rca == null ? 0 : rca.slm_ActGrossPremiumPay) });             //actgrosspremium
                        lst.Add(new ExcelData() { ColumnName = prefix + "31", Value = String.Format("{0:#,##0.00}", (rc.slm_PolicyGrossPremiumPay == null ? 0 : rc.slm_PolicyGrossPremiumPay) + (rca == null ? 0 : (rca.slm_ActGrossPremiumPay != null ? rca.slm_ActGrossPremiumPay : 0))) });             //total

                    }

                }

            }


            return lst;
        }
        public List<ExcelData> GetExcelData001P(decimal preleadID, string loginUsername)
        {
            List<ExcelData> lst = new List<ExcelData>();
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var data = (from plead in sdc.kkslm_tr_prelead
                            join tt in sdc.kkslm_ms_title on plead.slm_TitleId equals tt.slm_TitleId into r2
                            from tt in r2.DefaultIfEmpty()
                            join br in sdc.kkslm_ms_redbook_brand on plead.slm_Brand_Code equals br.slm_BrandCode into r4
                            from br in r4.DefaultIfEmpty()
                            join mo in sdc.kkslm_ms_redbook_model on plead.slm_Model_Code equals mo.slm_ModelCode into r5
                            from mo in r5.DefaultIfEmpty()
                            join stf in sdc.kkslm_ms_staff on plead.slm_Owner equals stf.slm_EmpCode into r6
                            from stf in r6.DefaultIfEmpty()
                            where plead.slm_Prelead_Id == preleadID
                            select new
                            {
                                plead.slm_Prelead_Id,
                                plead.slm_TicketId,
                                tt.slm_TitleName,
                                plead.slm_Name,
                                plead.slm_LastName,
                                contractnumber = plead.slm_Contract_Number,
                                oldpolicyno = plead.slm_Voluntary_Policy_Number,
                                oldpolicyexpdate = plead.slm_Voluntary_Policy_Exp_Date,
                                brandname = br.slm_BrandName,
                                modelname = mo.slm_ModelName,
                                licenseno = plead.slm_Car_License_No,
                                engineno = plead.slm_Chassis_No,// plead.slm_Engine_No,
                                paymentremark = plead.slm_RemarkPayment,
                                telesalename = stf.slm_StaffNameTH,
                                positionname = stf.slm_PositionName,
                                stafftel = stf.slm_TellNo,
                                stf.slm_EmpCode
                                //slm_RenewInsureId = ri.slm_RenewInsureId,
                                //slm_PolicyGrossPremiumTotal = plead.slm_PolicyGrossPremiumTotal,
                                //slm_PolicyGrossPremium= plead.slm_PolicyGrossPremium,
                                //slm_ActGrossPremium = plead.slm_ActGrossPremium,


                            }).FirstOrDefault();

                if (data != null)
                {

                    // find staff license no
                    //string stafflic = "";
                    //if (!String.IsNullOrEmpty(data.slm_EmpCode))
                    //{
                    //    var lic = sdc.kkslm_ms_mapping_staff_license.Where(l => l.slm_EmpCode == data.slm_EmpCode).ToList();
                    //    foreach (var lc in lic)
                    //    {
                    //        if (stafflic != "") stafflic += ", ";
                    //        stafflic += lc.slm_LicenseNo;
                    //    }

                    //}

                    string stafflic = "";
                    string loginNameTH = "";
                    string loginPositionName = "";
                    string loginTelNo = "";

                    var loginUser = sdc.kkslm_ms_staff.Where(p => p.slm_UserName == loginUsername).FirstOrDefault();
                    if (loginUser != null)
                    {
                        loginNameTH = loginUser.slm_StaffNameTH;
                        loginTelNo = loginUser.slm_TellNo;

                        var positionName = sdc.kkslm_ms_position.Where(p => p.slm_Position_id == loginUser.slm_Position_id).Select(p => p.slm_PositionNameTH).FirstOrDefault();
                        loginPositionName = positionName != null ? positionName : "";

                        if (!string.IsNullOrEmpty(loginUser.slm_EmpCode))
                        {
                            var lic = sdc.kkslm_ms_mapping_staff_license.Where(l => l.slm_EmpCode == loginUser.slm_EmpCode).ToList();
                            foreach (var lc in lic)
                            {
                                if (stafflic != "") stafflic += ", ";
                                stafflic += lc.slm_LicenseNo;
                            }
                        }
                    }

                    lst.Add(new ExcelData() { ColumnName = "I2", Value = DateTime.Now.ToString("d/MM/yyyy") }); // printdate
                    lst.Add(new ExcelData() { ColumnName = "C3", Value = String.Format("{0}{1} {2}", data.slm_TitleName, data.slm_Name, data.slm_LastName) });
                    lst.Add(new ExcelData() { ColumnName = "D3", Value = String.Format("{0}", data.contractnumber) });
                    lst.Add(new ExcelData() { ColumnName = "B5", Value = " ด้วยกรมธรรม์ประกันรถยนต์ของท่านเลขที่ " + (data.oldpolicyno == null ? "" : data.oldpolicyno) + " จะสิ้นสุดวันที่ " + (data.oldpolicyexpdate == null ? "" : data.oldpolicyexpdate.Value.ToString("dd/MM/yyyy")) + "\r\nบริษัทฯ ขอแจ้งค่าเบี้ยประกันในการต่อประกันตามรายละเอียดดังนี้\r\n" });
                    lst.Add(new ExcelData() { ColumnName = "B9", Value = String.Format("{0}", data.brandname) });
                    lst.Add(new ExcelData() { ColumnName = "C9", Value = String.Format("{0}", data.modelname) });
                    lst.Add(new ExcelData() { ColumnName = "D9", Value = String.Format("{0}", data.licenseno) });
                    lst.Add(new ExcelData() { ColumnName = "G9", Value = String.Format("{0}", data.engineno) });
                    lst.Add(new ExcelData() { ColumnName = "C37", Value = data.paymentremark });
                    lst.Add(new ExcelData() { ColumnName = "E40", Value = loginNameTH });
                    lst.Add(new ExcelData() { ColumnName = "E41", Value = String.Format("{0} / {1}", loginPositionName, loginTelNo) });
                    lst.Add(new ExcelData() { ColumnName = "E42", Value = String.Format("บัตรนายหน้าเลขที่ {0}", stafflic) });
                }
                else return lst;

                using (OPERDBEntities opr = new OPERDBEntities())
                {

                    // ข้อมูลในตาราง
                    //var tblst = sdc.kkslm_tr_renewinsurance_compare.Where(c => c.slm_RenewInsureId == data.slm_RenewInsureId).ToList();
                    
                    //var data2 = (from rc in sdc.kkslm_tr_prelead_compare
                    //             join rca in sdc.kkslm_tr_prelead_compare_act.Where(r => r.slm_PromotionId != 0 && r.slm_PromotionId != null) on new { pid = rc.slm_Prelead_Id, prm = rc.slm_PromotionId, seq = rc.slm_Seq } equals new { pid = rca.slm_Prelead_Id, prm = rca.slm_PromotionId, seq = rca.slm_Seq + 1 } into r0
                    //             from rca in r0.DefaultIfEmpty()
                    //             where rc.slm_Prelead_Id == data.slm_Prelead_Id && rc.slm_Seq >= 2
                    //             select new { rc, rca }
                    //             ).Union
                    //             (from rca in sdc.kkslm_tr_prelead_compare_act
                    //              join rc in sdc.kkslm_tr_prelead_compare.Where(r => r.slm_PromotionId != 0 && r.slm_PromotionId != null) on new { pid = rca.slm_Prelead_Id, prm = rca.slm_PromotionId, seq = rca.slm_Seq } equals new { pid = rc.slm_Prelead_Id, prm = rc.slm_PromotionId, seq = rc.slm_Seq - 1 } into r1
                    //              from rc in r1.DefaultIfEmpty()
                    //              where rca.slm_Prelead_Id == data.slm_Prelead_Id && rca.slm_Seq > 1
                    //              select new { rc, rca }
                    //    ).ToList();

                    var data2 = (from rc in sdc.kkslm_tr_prelead_compare
                                 join rca in sdc.kkslm_tr_prelead_compare_act.Where(r => r.slm_PromotionId != 0 && r.slm_PromotionId != null) on new { pid = rc.slm_Prelead_Id, prm = rc.slm_PromotionId } equals new { pid = rca.slm_Prelead_Id, prm = rca.slm_PromotionId } into r0
                                 from rca in r0.DefaultIfEmpty()
                                 where rc.slm_Prelead_Id == data.slm_Prelead_Id && rc.slm_Seq >= 2
                                 select new { rc, rca }
                                 ).Union
                                 (from rca in sdc.kkslm_tr_prelead_compare_act
                                  join rc in sdc.kkslm_tr_prelead_compare.Where(r => r.slm_PromotionId != 0 && r.slm_PromotionId != null) on new { pid = rca.slm_Prelead_Id, prm = rca.slm_PromotionId } equals new { pid = rc.slm_Prelead_Id, prm = rc.slm_PromotionId} into r1
                                  from rc in r1.DefaultIfEmpty()
                                  where rca.slm_Prelead_Id == data.slm_Prelead_Id && rca.slm_Seq > 1
                                  select new { rc, rca }
                        ).ToList();

                    //tmpdata1.Union(tmpdata2);
                    //var data2 = tmpdata1.ToList();
                    //var tblst = sdc.kkslm_tr_prelead_compare.Where(c => c.slm_Prelead_Id == data.slm_Prelead_Id && c.slm_Seq > 2).OrderBy(c => c.slm_Seq).ToList();

                    //Added by Pom 17/08/2016
                    var mainList = data2.ToList();
                    mainList.Clear();
                    var compareList = data2.Where(p => p.rc != null && p.rc.slm_PreLeadCompareId != 0).OrderBy(p => p.rc.slm_Seq).ToList();
                    var compareActList = data2.Where(p => p.rca != null && p.rca.slm_PreLeadCompareActId != 0).OrderBy(p => p.rca.slm_Seq).ToList();
                    compareList.ForEach(p => { mainList.Add(p); });
                    compareActList.ForEach(p => { mainList.Add(p); });
                    data2 = mainList.Distinct().ToList();

                    for (int i = 0; i < 3; i++)
                    {
                        kkslm_tr_prelead_compare rc;
                        kkslm_tr_prelead_compare_act rca;
                        if (data2.Count > i)
                        {
                            rc = data2[i].rc ?? new kkslm_tr_prelead_compare();// tblst[i];
                            rca = data2[i].rca;
                        }
                        else
                        {
                            rc = new kkslm_tr_prelead_compare();
                            rca = null;
                        }

                        //int? rc_seq = rc.slm_Seq - 1;
                        //var rca = sdc.kkslm_tr_prelead_compare_act.Where(a => a.slm_Prelead_Id == rc.slm_Prelead_Id && a.slm_PromotionId == rc.slm_PromotionId && a.slm_Seq == rc_seq).FirstOrDefault();

                        string prefix = "";
                        string prefix2 = "";
                        switch (i)
                        {
                            case 0: prefix = "E"; prefix2 = "F"; break;
                            case 1: prefix = "G"; prefix2 = "H"; break;
                            case 2: prefix = "I"; prefix2 = "J"; break;
                        }

                        //decimal insComId = (rc == null ? rca.slm_Ins_Com_Id : rc.slm_Ins_Com_Id);

                        decimal insComId = 0;
                        if (rc != null && rc.slm_PreLeadCompareId != 0)
                            insComId = rc.slm_Ins_Com_Id;
                        else if (rca != null && rca.slm_PreLeadCompareActId != 0)
                            insComId = rca.slm_Ins_Com_Id;

                        var ins = opr.kkslm_ms_ins_com.Where(c => c.slm_Ins_Com_Id == insComId).FirstOrDefault();
                        var prm = opr.kkslm_ms_promotioninsurance.Where(p => p.slm_PromotionId == rc.slm_PromotionId).FirstOrDefault();
                        var cov = sdc.kkslm_ms_coveragetype.Where(c => c.slm_CoverageTypeId == rc.slm_CoverageTypeId).FirstOrDefault();
                        var rpr = sdc.kkslm_ms_repairtype.Where(r => r.slm_RepairTypeId == rc.slm_RepairTypeId).FirstOrDefault();

                        // rc
                        lst.Add(new ExcelData() { ColumnName = prefix + "14", Value = String.Format("{0}", ins == null ? "" : ins.slm_InsNameTh) });            // insname
                        lst.Add(new ExcelData() { ColumnName = prefix + "15", Value = String.Format("{0}", cov == null ? "" : cov.slm_ConverageTypeName) });             //coveragename
                        lst.Add(new ExcelData() { ColumnName = prefix + "17", Value = String.Format("{0:#,##0}", rc.slm_InjuryDeath) });             //injurydeath
                        lst.Add(new ExcelData() { ColumnName = prefix + "19", Value = String.Format("{0:#,##0}", rc.slm_TPPD) });             //tppd
                        lst.Add(new ExcelData() { ColumnName = prefix + "20", Value = String.Format("{0}", rpr == null ? "" : rpr.slm_RepairTypeName) });             //repair
                        lst.Add(new ExcelData() { ColumnName = prefix + "21", Value = String.Format("{0:#,##0}", rc.slm_OD) });       //od
                        //lst.Add(new ExcelData() { ColumnName = prefix + "22", Value = String.Format("{0:#,##0}", rc.slm_DeDuctible) });       //deductible
                        lst.Add(new ExcelData() { ColumnName = prefix + "22", Value = String.Format("{0}", rc.slm_DeDuctibleFlag) });       //deductible
                        lst.Add(new ExcelData() { ColumnName = prefix + "23", Value = String.Format("{0:#,##0}", rc.slm_FT) });       //ft
                        lst.Add(new ExcelData() { ColumnName = prefix + "25", Value = String.Format("{0:#,##0}", rc.slm_PersonalAccident) });             //accident , insuranceseat
                        lst.Add(new ExcelData() { ColumnName = prefix2 + "25", Value = String.Format("{0} ที่นั่ง", prm == null ? "" : (SLMUtil.SafeInt(prm.slm_PersonalAccidentDriver) + SLMUtil.SafeInt(prm.slm_PersonalAccidentPassenger)).ToString() /* cr 20160725rc.slm_PersonalAccidentMan */ ) });         // insuranceseat
                        lst.Add(new ExcelData() { ColumnName = prefix + "26", Value = String.Format("{0:#,##0}", rc.slm_MedicalFee) });               //medicalfee 
                        lst.Add(new ExcelData() { ColumnName = prefix2 + "26", Value = String.Format("{0} ที่นั่ง", prm == null ? "" : (SLMUtil.SafeInt(prm.slm_MedicalFeeDriver) + SLMUtil.SafeInt(prm.slm_MedicalFeePassenger)).ToString() /* cr 20150725 rc.slm_MedicalFeeMan */ ) });         // medicalseat
                        lst.Add(new ExcelData() { ColumnName = prefix + "27", Value = String.Format("{0:#,##0.00}", rc.slm_InsuranceDriver) });             //insurancedriver
                        lst.Add(new ExcelData() { ColumnName = prefix + "28", Value = String.Format("{0:#,##0.00}", rc.slm_NetGrossPremium) });             //policygrosspremiumtotal
                        lst.Add(new ExcelData() { ColumnName = prefix + "29", Value = String.Format("{0:#,##0.00}", rc.slm_PolicyGrossPremiumPay) });             //policygrosspremium

                        // rca
                        lst.Add(new ExcelData() { ColumnName = prefix + "30", Value = String.Format("{0:#,##0.00}", rca == null ? 0 : rca.slm_ActGrossPremiumPay) });             //actgrosspremium
                        lst.Add(new ExcelData() { ColumnName = prefix + "31", Value = String.Format("{0:#,##0.00}", (rc.slm_PolicyGrossPremiumPay == null ? 0 : rc.slm_PolicyGrossPremiumPay) + (rca == null ? 0 : (rca.slm_ActGrossPremiumPay != null ? rca.slm_ActGrossPremiumPay : 0))) });             //total

                        //lst.Add(new ExcelData() { ColumnName = prefix + "30", Value = String.Format("{0:#,##0.00}", rca == null ? 0 : rca.slm_ActNetGrossPremium) });             //actgrosspremium
                        //lst.Add(new ExcelData() { ColumnName = prefix + "31", Value = String.Format("{0:#,##0.00}", rc.slm_PolicyGrossPremiumPay + (rca == null ? 0 : rca.slm_ActNetGrossPremium)) });             //total

                    }

                }

            }


            return lst;
        }


        /// <summary>
        /// ฟอร์มใบเสอราคาแบบรถ Fleet
        /// </summary>
        public List<ExcelData> GetExcelData002T(string[] ticketIDLst, string loginUsername)
        {
            List<ExcelData> lst = new List<ExcelData>();
            if (ticketIDLst.Length == 0) return lst;
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                string mainticket = ticketIDLst[0];
                var data1 = (from lead in sdc.kkslm_tr_lead
                             join pre in sdc.kkslm_tr_prelead on lead.slm_ticketId equals pre.slm_TicketId into r0 from pre in r0.DefaultIfEmpty()
                             join ins in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
                             join tt in sdc.kkslm_ms_title on lead.slm_TitleId equals tt.slm_TitleId into r1
                             from tt in r1.DefaultIfEmpty()
                             where lead.is_Deleted == 0 && lead.slm_ticketId == mainticket
                             select new
                             {
                                 lead.slm_Name,
                                 lead.slm_LastName,
                                 tt.slm_TitleName,
                                 slm_Voluntary_Policy_Exp_Year = pre.slm_Voluntary_Policy_Exp_Year,
                                 slm_Voluntary_Policy_Exp_Month = pre.slm_Voluntary_Policy_Exp_Month,
                                 slm_Contract_Number = ins.slm_ContractNo
                             }
                             ).ToList();

                if (data1 == null || data1.Count == 0)
                {
                    throw new Exception("NO Data");
                }
                    
                lst.Add(new ExcelData() { ColumnName = "N2", Value = string.Format("วันที่ {0:dd/MM/yyyy}", DateTime.Now) });
                lst.Add(new ExcelData() { ColumnName = "B3", Value = string.Format("{0}{1} {2}/{3}", string.IsNullOrEmpty(data1[0].slm_TitleName) ? "คุณ" : data1[0].slm_TitleName, data1[0].slm_Name, data1[0].slm_LastName, data1[0].slm_Contract_Number) });
                lst.Add(new ExcelData() { ColumnName = "A5", Value = string.Format("              ด้วยกรมธรรม์ประกันรถยนต์ ของท่านจะสิ้นสุดในรอบเดือน {0}/{1} ขอแจ้งค่าเบี้ยประกันในการต่อประกันตามรายละเอียดดังนี้", data1[0].slm_Voluntary_Policy_Exp_Month, data1[0].slm_Voluntary_Policy_Exp_Year) });


                var data2 = (from lead in sdc.kkslm_tr_lead
                             join pre in sdc.kkslm_tr_prelead on lead.slm_ticketId equals pre.slm_TicketId into r0 from pre in r0.DefaultIfEmpty()
                             join ins in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
                             join brand in sdc.kkslm_ms_redbook_brand on ins.slm_RedbookBrandCode equals brand.slm_BrandCode into r2
                             from brand in r2.DefaultIfEmpty()
                             join model in sdc.kkslm_ms_redbook_model on new { bc = ins.slm_RedbookBrandCode, mc = ins.slm_RedbookModelCode } equals new { bc = model.slm_BrandCode, mc = model.slm_ModelCode } into r3
                             from model in r3.DefaultIfEmpty()
                             where lead.is_Deleted == 0 && ticketIDLst.Contains( lead.slm_ticketId)// == mainticket
                             orderby ins.slm_ContractNo ascending
                             select new
                             {
                                 ContractNo = ins.slm_ContractNo,
                                 ExpDate = pre.slm_Voluntary_Policy_Exp_Date,
                                 BrandName = brand.slm_BrandName,
                                 ModelName = model.slm_ModelName,
                                 LicenseNo = ins.slm_LicenseNo,
                                 ChassisNo = ins.slm_ChassisNo,
                                 CC = ins.slm_CC
                             }).ToList();

                string[] rowlst = { "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", 
                                      "20", "21", "22", "23", "24", "25", "26", "27" };
                for (int i = 0; i < rowlst.Length; i++)
                {
                    var row = rowlst[i];
                    if (data2.Count > i)
                    {
                        var curdata = data2[i];
                        lst.Add(new ExcelData() { ColumnName = "A" + row, Value = (i + 1).ToString() });
                        lst.Add(new ExcelData() { ColumnName = "B" + row, Value = curdata.ContractNo });
                        lst.Add(new ExcelData() { ColumnName = "C" + row, Value = string.Format("{0:dd/MM/yyyy}", curdata.ExpDate) });
                        lst.Add(new ExcelData() { ColumnName = "D" + row, Value = curdata.BrandName });
                        lst.Add(new ExcelData() { ColumnName = "E" + row, Value = curdata.ModelName });
                        lst.Add(new ExcelData() { ColumnName = "F" + row, Value = curdata.LicenseNo });
                        lst.Add(new ExcelData() { ColumnName = "I" + row, Value = curdata.ChassisNo });
                        lst.Add(new ExcelData() { ColumnName = "L" + row, Value = curdata.CC });
                    }
                }

                var data3 = (from data4 in (from lead in sdc.kkslm_tr_lead
                                            join ins in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
                                            join pc in sdc.kkslm_tr_renewinsurance_compare on ins.slm_RenewInsureId equals pc.slm_RenewInsureId
                                            join pca in sdc.kkslm_tr_renewinsurance_compare_act.Where(p => p.slm_PromotionId != 0 && p.slm_PromotionId != null) on new { rid = (decimal?)ins.slm_RenewInsureId, prm = pc.slm_PromotionId } equals new { rid = pca.slm_RenewInsureId, prm = pca.slm_PromotionId } into r0
                                            from pca in r0.DefaultIfEmpty()
                                            where ticketIDLst.Contains(lead.slm_ticketId) && ((pc.slm_PromotionId != 0 && pc.slm_PromotionId != null) || ((pc.slm_PromotionId == 0 || pc.slm_PromotionId == null) && pc.slm_Seq >= 2))
                                            select new { lead, pc, pca, ins }
                               ).Union
                               (
                                   from lead in sdc.kkslm_tr_lead
                                   join ins in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
                                   join pca in sdc.kkslm_tr_renewinsurance_compare_act on ins.slm_RenewInsureId equals pca.slm_RenewInsureId
                                   join pc in sdc.kkslm_tr_renewinsurance_compare.Where(p => p.slm_PromotionId != 0 && p.slm_PromotionId != null) on new { rid = (decimal?)ins.slm_RenewInsureId, prm = pca.slm_PromotionId } equals new { rid = pc.slm_RenewInsureId, prm = pc.slm_PromotionId } into r1
                                   from pc in r1.DefaultIfEmpty()
                                   where ticketIDLst.Contains(lead.slm_ticketId) && ((pca.slm_PromotionId != 0 && pca.slm_PromotionId != null) || ((pca.slm_PromotionId == 0 || pca.slm_PromotionId == null) && pca.slm_Seq > 1))
                                   select new { lead, pc, pca, ins }
                                   )
                             join ctyp in sdc.kkslm_ms_coveragetype on data4.pc.slm_CoverageTypeId equals ctyp.slm_CoverageTypeId into r2
                             from ctyp in r2.DefaultIfEmpty()
                             join rtyp in sdc.kkslm_ms_repairtype on data4.pc.slm_RepairTypeId equals rtyp.slm_RepairTypeId into r3
                             from rtyp in r3.DefaultIfEmpty()
                             orderby data4.ins.slm_ContractNo
                             select new
                             {
                                 ContractNo = data4.ins.slm_ContractNo,
                                 CompareId = data4.pc != null ? data4.pc.slm_RenewInsureCompareId : 0,
                                 CompareSeq = data4.pc != null ? data4.pc.slm_Seq : 0,
                                 CompareNotifyPremiumId = data4.pc != null ? data4.pc.slm_NotifyPremiumId : 0,
                                 CompareActId = data4.pca != null ? data4.pca.slm_RenewInsureCompareActId : 0,
                                 CompareActSeq = data4.pca != null ? data4.pca.slm_Seq : 0,
                                 LicenseNo = data4.ins.slm_LicenseNo,
                                 ChassisNo = data4.ins.slm_ChassisNo,
                                 OwnerID = data4.lead.slm_Owner,

                                 CovtypeName = ctyp.slm_ConverageTypeName,
                                 RepareName = rtyp.slm_RepairTypeName,

                                 // pc
                                 slm_PromotionId = data4.pc == null ? data4.pca == null ? null : data4.pca.slm_PromotionId : data4.pc.slm_PromotionId,
                                 FT = data4.pc.slm_FT,
                                 OD = data4.pc.slm_OD,
                                 NetGrossPremium = data4.pc.slm_PolicyGrossPremium, //data4.pc.slm_NetGrossPremium,
                                 PolicyGrossStamp = data4.pc.slm_PolicyGrossStamp,
                                 //Vat1PercentBath = data4.pc.slm_PolicyGrossPremium,
                                 Vat1PercentBath = data4.pc.slm_Vat1PercentBath,
                                 PolicyGrossPremium = data4.pc.slm_NetGrossPremium, //data4.pc.slm_PolicyGrossPremium,
                                 PolicyGrossVat = data4.pc.slm_PolicyGrossVat,
                                 DiscountBath = data4.pc.slm_DiscountBath,
                                 DeDuctible = data4.pc.slm_DeDuctible,
                                 DeDuctibleFlag = data4.pc.slm_DeDuctibleFlag,
                                 MedicalMan = data4.pc.slm_MedicalFeeMan,
                                 MedicalFee = data4.pc.slm_MedicalFee,
                                 MedicalFeeDriver = data4.pc.slm_MedicalFeeDriver,
                                 MedicalFeePassng = data4.pc.slm_MedicalFeePassenger,
                                 InsuranceDriver = data4.pc.slm_InsuranceDriver,
                                 InjuryDeath = data4.pc.slm_InjuryDeath,
                                 PersonalAccident = data4.pc.slm_PersonalAccident,
                                 TPPD = data4.pc.slm_TPPD,
                                 CompId = data4.pc == null ? data4.pca == null ? 0 : data4.pca.slm_Ins_Com_Id : data4.pc.slm_Ins_Com_Id,
                                 PolicyGrossPremiumPay = data4.pc.slm_PolicyGrossPremiumPay,

                                 // pca
                                 ActGrossPremiumPay = data4.pca.slm_ActGrossPremiumPay,
                                 ACTNO = data4.pca.slm_ActNo

                             }).ToList();

                //Added by Pom 16/08/2016
                var distinctList = data3.Select(p => p.ContractNo).Distinct().ToList();
                data3 = data3.OrderBy(p => p.ContractNo).ToList();
                var mainList = data3.ToList();
                mainList.Clear();
                foreach (string contractNo in distinctList)
                {
                    var processList = data3.Where(p => p.ContractNo == contractNo).ToList();
                    var compareList = processList.Where(p => p.CompareId != 0).OrderBy(p => p.CompareSeq).ToList();
                    var removeList = compareList.Where(p => p.CompareSeq == 2 && (p.CompareNotifyPremiumId == null || p.CompareNotifyPremiumId == 0)).ToList();
                    removeList.ForEach(p => { compareList.Remove(p); });

                    var compareActList = processList.Where(p => p.CompareActId != 0).OrderBy(p => p.CompareActSeq).ToList();

                    compareList.ForEach(p => { mainList.Add(p); });
                    compareActList.ForEach(p => { mainList.Add(p); });
                }

                data3 = mainList.Distinct().ToList();

                #region OldCode
                //var data3 = (from lead in sdc.kkslm_tr_lead
                //             join pre in sdc.kkslm_tr_prelead on lead.slm_ticketId equals pre.slm_TicketId
                //             join ins in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
                //             join pc in sdc.kkslm_tr_renewinsurance_compare on ins.slm_RenewInsureId equals pc.slm_RenewInsureId
                //             join pca in sdc.kkslm_tr_renewinsurance_compare_act on new { plid = ins.slm_RenewInsureId, pflag = true } equals new { plid = pca.slm_RenewInsureId.Value, pflag = pca.slm_ActPurchaseFlag.Value } into r1
                //             from pca in r1.DefaultIfEmpty()
                //             join ctyp in sdc.kkslm_ms_coveragetype on pc.slm_CoverageTypeId equals ctyp.slm_CoverageTypeId into r2
                //             from ctyp in r2.DefaultIfEmpty()
                //             join rtyp in sdc.kkslm_ms_repairtype on pc.slm_RepairTypeId equals rtyp.slm_RepairTypeId into r3
                //             from rtyp in r3.DefaultIfEmpty()
                //             where pre.is_Deleted == false && pc.slm_Selected == true && ticketIDLst.Contains(lead.slm_ticketId)
                //             orderby ins.slm_ContractNo ascending 

                //var data3 = (from lead in sdc.kkslm_tr_lead
                //             join pre in sdc.kkslm_tr_prelead on lead.slm_ticketId equals pre.slm_TicketId
                //             join ins in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
                //             join pc in sdc.kkslm_tr_renewinsurance_compare on ins.slm_RenewInsureId equals pc.slm_RenewInsureId into r0 from pc in r0.DefaultIfEmpty()
                //             join pca in sdc.kkslm_tr_renewinsurance_compare_act on new { plid = ins.slm_RenewInsureId, pflag = true } equals new { plid = pca.slm_RenewInsureId.Value, pflag = pca.slm_ActPurchaseFlag.Value } into r1
                //             from pca in r1.DefaultIfEmpty()
                //             join ctyp in sdc.kkslm_ms_coveragetype on pc.slm_CoverageTypeId equals ctyp.slm_CoverageTypeId into r2
                //             from ctyp in r2.DefaultIfEmpty()
                //             join rtyp in sdc.kkslm_ms_repairtype on pc.slm_RepairTypeId equals rtyp.slm_RepairTypeId into r3
                //             from rtyp in r3.DefaultIfEmpty()
                //             where pre.is_Deleted == false && ticketIDLst.Contains(lead.slm_ticketId)
                //             && (
                //                ( pca  != null && pc != null )
                //                ||
                //                ( pc != null && ( pc.slm_PromotionId == null || pc.slm_PromotionId == 0 ) && pc.slm_Seq > 2 )
                //                ||
                //                ( pca != null && ( pca.slm_PromotionId == null || pca.slm_PromotionId == 0) && pca.slm_Seq > 1 )
                //             )
                //             orderby ins.slm_ContractNo ascending 
                //             select new
                //             {
                //                 LicenseNo = pre.slm_Car_License_No,
                //                 ChassisNo = pre.slm_Chassis_No,
                //                 OwnerID = pre.slm_Owner,

                //                 CovtypeName = ctyp.slm_ConverageTypeName,
                //                 RepareName = rtyp.slm_RepairTypeName,

                //                 // pc
                //                 pc.slm_PromotionId,
                //                 FT = pc.slm_FT,
                //                 OD = pc.slm_OD,
                //                 NetGrossPremium = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_NetGrossPremium == null ? null : pc.slm_NetGrossPremium,
                //                 PolicyGrossStamp = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PolicyGrossStamp == null ? null : pc.slm_PolicyGrossStamp,
                //                 Vat1PercentBath = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_Vat1PercentBath == null ? null : pc.slm_Vat1PercentBath,
                //                 PolicyGrossPremium = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PolicyGrossPremium == null ? null : pc.slm_PolicyGrossPremium,
                //                 PolicyGrossVat = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PolicyGrossVat == null ? null : pc.slm_PolicyGrossVat,
                //                 DiscountBath = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_DiscountBath == null ? null : pc.slm_DiscountBath,
                //                 DeDuctible = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_DeDuctible == null ? null : pc.slm_DeDuctible,
                //                 MedicalMan = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_MedicalFeeMan == null ? null : pc.slm_MedicalFeeMan,
                //                 MedicalFee = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_MedicalFee == null ? null : pc.slm_MedicalFee,
                //                 MedicalFeeDriver = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_MedicalFeeDriver == null ? null : pc.slm_MedicalFeeDriver,
                //                 MedicalFeePassng = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_MedicalFeePassenger == null ? null : pc.slm_MedicalFeePassenger,
                //                 InsuranceDriver = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_InsuranceDriver == null ? null : pc.slm_InsuranceDriver,
                //                 InjuryDeath = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_InjuryDeath == null ? null : pc.slm_InjuryDeath,
                //                 PersonalAccident = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PersonalAccident == null ? null : pc.slm_PersonalAccident,
                //                 TPPD = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_TPPD == null ? null : pc.slm_TPPD,
                //                 CompId = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) ? 0 : pc.slm_Ins_Com_Id,
                //                 PolicyGrossPremiumPay = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PolicyGrossPremiumPay == null ? null : pc.slm_PolicyGrossPremiumPay,

                //                 // pca
                //                 ActGrossPremiumPay = (pc.slm_PromotionId == null || pc.slm_PromotionId == 0) || pca.slm_ActGrossPremiumPay == null ? null : pca.slm_ActGrossPremiumPay,
                //                 ACTNO = (pc.slm_PromotionId == null || pc.slm_PromotionId == 0) || pca.slm_ActNo == null ? null : pca.slm_ActNo
                //             }
                //{
                //    CovtypeName = ctyp.slm_ConverageTypeName,
                //    LicenseNo = pre.slm_Car_License_No,
                //    ChassisNo = pre.slm_Chassis_No,
                //    RepareName = rtyp.slm_RepairTypeName,
                //    FT = pc.slm_FT,
                //    OD = pc.slm_OD,
                //    NetGrossPremium = pc.slm_NetGrossPremium,
                //    PolicyGrossStamp = pc.slm_PolicyGrossStamp,
                //    Vat1PercentBath = pc.slm_Vat1PercentBath,
                //    PolicyGrossPremium = pc.slm_PolicyGrossPremium,
                //    PolicyGrossVat = pc.slm_PolicyGrossVat,
                //    DiscountBath = pc.slm_DiscountBath,
                //    ActGrossPremiumPay = pca.slm_ActGrossPremiumPay,
                //    DeDuctible = pc.slm_DeDuctible,
                //    MedicalMan = pc.slm_MedicalFeeMan,
                //    MedicalFee = pc.slm_MedicalFee,
                //    MedicalFeeDriver = pc.slm_MedicalFeeDriver,
                //    MedicalFeePassng = pc.slm_MedicalFeePassenger,
                //    InsuranceDriver = pc.slm_InsuranceDriver,
                //    InjuryDeath = pc.slm_InjuryDeath,
                //    PersonalAccident = pc.slm_PersonalAccident,
                //    TPPD = pc.slm_TPPD,
                //    ACTNO = pca.slm_ActNo,
                //    CompId = pc.slm_Ins_Com_Id,
                //    OwnerID = pre.slm_Owner,
                //    PromotionId = pc.slm_PromotionId,
                //    PolicyGrossPremiumPay = pc.slm_PolicyGrossPremiumPay
                //}
                //             ).ToList();
                #endregion
                List<kkslm_ms_ins_com> comlst;

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    comlst = odc.kkslm_ms_ins_com.ToList();
                    rowlst = new string[] { "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", 
                        "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", 
                        "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74",
                        "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "86", "87", "88", "89", "90",
                        "91", "92", "93", "94", "95", "96", "97", "98", "99", "100", "101", "102", "103", "104", "105", "106", "107", "108"
                        , "109", "110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "120"
                        , "121", "122", "123", "124", "125", "126", "127", "128", "129", "130", "131", "132"
                        , "133", "134", "135", "136", "137", "138", "139", "140", "141", "142", "143", "144"
                        , "145", "146", "147", "148", "149", "150", "151", "152", "153", "154", "155", "156", "157", "158", "159"
                        , "160", "161", "162", "163", "164", "165"
                        , "166", "167", "168", "169", "170", "171", "172", "173", "174" };

                    for (int i = 0; i < rowlst.Length; i++)
                    {
                        var row = rowlst[i];
                        if (data3.Count > i)
                        {
                            var d = data3[i];
                            var com = comlst.Where(c => c.slm_Ins_Com_Id == d.CompId).FirstOrDefault();
                            var promo = odc.kkslm_ms_promotioninsurance.Where(p => p.slm_PromotionId == d.slm_PromotionId).FirstOrDefault();

                            lst.Add(new ExcelData() { ColumnName = "A" + row, Value = string.Format("{0}", d.CovtypeName) });
                            lst.Add(new ExcelData() { ColumnName = "B" + row, Value = string.Format("{0}", d.LicenseNo) });
                            lst.Add(new ExcelData() { ColumnName = "C" + row, Value = string.Format("{0}", d.ChassisNo) });
                            lst.Add(new ExcelData() { ColumnName = "D" + row, Value = string.Format("{0}", com == null ? "" : com.slm_InsNameTh) });
                            lst.Add(new ExcelData() { ColumnName = "E" + row, Value = string.Format("{0}", d.RepareName) });
                            lst.Add(new ExcelData() { ColumnName = "F" + row, Value = string.Format("{0}", d.OD) });
                            lst.Add(new ExcelData() { ColumnName = "G" + row, Value = string.Format("{0}", d.NetGrossPremium) });
                            lst.Add(new ExcelData() { ColumnName = "H" + row, Value = string.Format("{0}", d.PolicyGrossStamp) });
                            lst.Add(new ExcelData() { ColumnName = "I" + row, Value = string.Format("{0}", d.PolicyGrossVat) });
                            lst.Add(new ExcelData() { ColumnName = "J" + row, Value = string.Format("{0}", d.Vat1PercentBath) });
                            lst.Add(new ExcelData() { ColumnName = "K" + row, Value = string.Format("{0}", d.PolicyGrossPremium) });
                            lst.Add(new ExcelData() { ColumnName = "L" + row, Value = string.Format("{0}", d.DiscountBath <= 0 ? null : d.DiscountBath) });
                            lst.Add(new ExcelData() { ColumnName = "M" + row, Value = string.Format("{0}", d.PolicyGrossPremiumPay) });
                            lst.Add(new ExcelData() { ColumnName = "N" + row, Value = string.Format("{0}", d.ActGrossPremiumPay) });
                            //lst.Add(new ExcelData() { ColumnName = "O" + row, Value = string.Format("{0}", d.DeDuctible) });
                            lst.Add(new ExcelData() { ColumnName = "O" + row, Value = string.Format("{0}", d.DeDuctibleFlag) });
                            lst.Add(new ExcelData() { ColumnName = "P" + row, Value = string.Format("{0}", d.FT) });
                            //lst.Add(new ExcelData() { ColumnName = "Q" + row, Value = string.Format("{0}", d.InjuryDeath) });
                            //lst.Add(new ExcelData() { ColumnName = "R" + row, Value = "" });
                            //lst.Add(new ExcelData() { ColumnName = "S" + row, Value = string.Format("{0}", d.TPPD) });
                            ////lst.Add(new ExcelData() { ColumnName = "T" + row, Value = string.Format("{0}", d.MedicalFeeDriver + d.MedicalFeePassng) });
                            //lst.Add(new ExcelData() { ColumnName = "T" + row, Value = string.Format("{0}", promo == null ? 0 : (SLMUtil.SafeInt(promo.slm_MedicalFeeDriver) + SLMUtil.SafeInt(promo.slm_MedicalFeePassenger))) });
                            //lst.Add(new ExcelData() { ColumnName = "U" + row, Value = string.Format("{0}", d.PersonalAccident) });
                            //lst.Add(new ExcelData() { ColumnName = "V" + row, Value = string.Format("{0}", d.MedicalFee) });
                            //lst.Add(new ExcelData() { ColumnName = "W" + row, Value = string.Format("{0}", d.InsuranceDriver) });
                            lst.Add(new ExcelData() { ColumnName = "Q" + row, Value = string.Format("{0}", d.InjuryDeath == 0 ? null : d.InjuryDeath) });
                            lst.Add(new ExcelData() { ColumnName = "R" + row, Value = null });
                            lst.Add(new ExcelData() { ColumnName = "S" + row, Value = string.Format("{0}", d.TPPD == 0 ? null : d.TPPD) });
                            lst.Add(new ExcelData() { ColumnName = "T" + row, Value = string.Format("{0}", promo == null ? null : (int?)(SLMUtil.SafeInt(promo.slm_MedicalFeeDriver) + SLMUtil.SafeInt(promo.slm_MedicalFeePassenger))) });
                            lst.Add(new ExcelData() { ColumnName = "U" + row, Value = string.Format("{0}", d.PersonalAccident == 0 ? null : d.PersonalAccident) });
                            lst.Add(new ExcelData() { ColumnName = "V" + row, Value = string.Format("{0}", d.MedicalFee == 0 ? null : d.MedicalFee) });
                            lst.Add(new ExcelData() { ColumnName = "W" + row, Value = string.Format("{0}", d.InsuranceDriver == 0 ? null : d.InsuranceDriver) });

                            lst.Add(new ExcelData() { ColumnName = "X" + row, Value = "" });
                        }
                    }
                }

                if (data3.Count > 0)
                {
                    string stafflic = "";
                    string loginNameTH = "";
                    string loginPositionName = "";
                    string loginTelNo = "";

                    var loginUser = sdc.kkslm_ms_staff.Where(p => p.slm_UserName == loginUsername).FirstOrDefault();
                    if (loginUser != null)
                    {
                        loginNameTH = loginUser.slm_StaffNameTH;
                        loginTelNo = loginUser.slm_TellNo;

                        var positionName = sdc.kkslm_ms_position.Where(p => p.slm_Position_id == loginUser.slm_Position_id).Select(p => p.slm_PositionNameTH).FirstOrDefault();
                        loginPositionName = positionName != null ? positionName : "";

                        if (!string.IsNullOrEmpty(loginUser.slm_EmpCode))
                        {
                            var lic = sdc.kkslm_ms_mapping_staff_license.Where(l => l.slm_EmpCode == loginUser.slm_EmpCode).ToList();
                            foreach (var lc in lic)
                            {
                                if (stafflic != "") stafflic += ", ";
                                stafflic += lc.slm_LicenseNo;
                            }
                        }
                    }

                    lst.Add(new ExcelData() { ColumnName = "M177", Value = loginNameTH });
                    lst.Add(new ExcelData() { ColumnName = "M178", Value = string.Format("{0}/{1}", loginPositionName, loginTelNo) });
                    lst.Add(new ExcelData() { ColumnName = "M179", Value = string.Format("บัตรนายหน้าเลขที่ {0}", stafflic) });


                    //string ownr = data3[0].OwnerID;
                    //var staff = sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode == ownr).FirstOrDefault();
                    //if (staff != null)
                    //{
                    //    lst.Add(new ExcelData() { ColumnName = "M176", Value = staff.slm_StaffNameTH });
                    //    lst.Add(new ExcelData() { ColumnName = "M177", Value = string.Format("{0}/{1}", staff.slm_PositionName, staff.slm_TellNo) });

                    //    var lic = sdc.kkslm_ms_mapping_staff_license.Where(l => l.slm_EmpCode == staff.slm_EmpCode).Select(l => l.slm_LicenseNo).ToArray();
                    //    lst.Add(new ExcelData() { ColumnName = "M178", Value = string.Format("บัตรนายหน้าเลขที่ {0}", string.Join(",", lic)) });
                    //}
                }

            }
            return lst;
        }
        /// <summary>
        /// ฟอร์มใบเสอราคาแบบรถ Fleet
        /// </summary>
        public List<ExcelData> GetExcelData002P(decimal[] preleadIDLst, string loginUsername)
        {
            List<ExcelData> lst = new List<ExcelData>();
            if (preleadIDLst.Length == 0) return lst;
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                decimal preleadmain = preleadIDLst[0];
                var data1 = (from pre in sdc.kkslm_tr_prelead
                             join tt in sdc.kkslm_ms_title on pre.slm_TitleId equals tt.slm_TitleId into r1
                             from tt in r1.DefaultIfEmpty()
                             join brand in sdc.kkslm_ms_redbook_brand on pre.slm_Brand_Code equals brand.slm_BrandCode into r2
                             from brand in r2.DefaultIfEmpty()
                             join model in sdc.kkslm_ms_redbook_model on new { bc = pre.slm_Brand_Code, mc = pre.slm_Model_Code } equals new { bc = model.slm_BrandCode, mc = model.slm_ModelCode } into r3
                             from model in r3.DefaultIfEmpty()
                             where pre.is_Deleted == false && pre.slm_Prelead_Id == preleadmain
                             select new
                             {
                                 pre.slm_Name,
                                 pre.slm_LastName,
                                 tt.slm_TitleName,
                                 pre.slm_Voluntary_Policy_Exp_Year,
                                 pre.slm_Voluntary_Policy_Exp_Month,
                                 pre.slm_Contract_Number
                             }
                             ).ToList();

                if (data1 == null || data1.Count == 0)
                {
                    throw new Exception("NO Data");
                }

                lst.Add(new ExcelData() { ColumnName = "N2", Value = string.Format("วันที่ {0:dd/MM/yyyy}", DateTime.Now) });
                lst.Add(new ExcelData() { ColumnName = "B3", Value = string.Format("{0}{1} {2}/{3}", string.IsNullOrEmpty(data1[0].slm_TitleName) ? "คุณ" : data1[0].slm_TitleName, data1[0].slm_Name, data1[0].slm_LastName, data1[0].slm_Contract_Number) });
                lst.Add(new ExcelData() { ColumnName = "A5", Value = string.Format("              ด้วยกรมธรรม์ประกันรถยนต์ ของท่านจะสิ้นสุดในรอบเดือน {0}/{1} ขอแจ้งค่าเบี้ยประกันในการต่อประกันตามรายละเอียดดังนี้", data1[0].slm_Voluntary_Policy_Exp_Month, data1[0].slm_Voluntary_Policy_Exp_Year) });


                var data2 = (from pre in sdc.kkslm_tr_prelead
                             join brand in sdc.kkslm_ms_redbook_brand on pre.slm_Brand_Code equals brand.slm_BrandCode into r2
                             from brand in r2.DefaultIfEmpty()
                             join model in sdc.kkslm_ms_redbook_model on new { bc = pre.slm_Brand_Code, mc = pre.slm_Model_Code } equals new { bc = model.slm_BrandCode, mc = model.slm_ModelCode } into r3
                             from model in r3.DefaultIfEmpty()
                             where pre.is_Deleted == false && preleadIDLst.Contains(pre.slm_Prelead_Id)
                             select new
                             {
                                 ContractNo = pre.slm_Contract_Number,
                                 ExpDate = pre.slm_Voluntary_Policy_Exp_Date,
                                 BrandName = brand.slm_BrandName,
                                 ModelName = model.slm_ModelName,
                                 LicenseNo = pre.slm_Car_License_No,
                                 ChassisNo = pre.slm_Chassis_No,
                                 CC = pre.slm_Cc
                             }).OrderBy(r=>r.ContractNo).ToList();

                string[] rowlst = { "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", 
                                      "20", "21", "22", "23", "24", "25", "26", "27" };
                for (int i = 0; i < rowlst.Length; i++)
                {
                    var row = rowlst[i];
                    if (data2.Count > i)
                    {
                        var curdata = data2[i];
                        lst.Add(new ExcelData() { ColumnName = "A" + row, Value = (i + 1).ToString() });
                        lst.Add(new ExcelData() { ColumnName = "B" + row, Value = curdata.ContractNo });
                        lst.Add(new ExcelData() { ColumnName = "C" + row, Value = string.Format("{0:dd/MM/yyyy}", curdata.ExpDate) });
                        lst.Add(new ExcelData() { ColumnName = "D" + row, Value = curdata.BrandName });
                        lst.Add(new ExcelData() { ColumnName = "E" + row, Value = curdata.ModelName });
                        lst.Add(new ExcelData() { ColumnName = "F" + row, Value = curdata.LicenseNo });
                        lst.Add(new ExcelData() { ColumnName = "I" + row, Value = curdata.ChassisNo });
                        lst.Add(new ExcelData() { ColumnName = "L" + row, Value = curdata.CC });
                    }
                }

                var data3 = (from data4 in (from pre in sdc.kkslm_tr_prelead
                                            join pc in sdc.kkslm_tr_prelead_compare on pre.slm_Prelead_Id equals pc.slm_Prelead_Id
                                            join pca in sdc.kkslm_tr_prelead_compare_act.Where(a => a.slm_PromotionId != 0 && a.slm_PromotionId != null) on new { prid = (decimal?)pre.slm_Prelead_Id, prm = pc.slm_PromotionId } equals new { prid = pca.slm_Prelead_Id, prm = pca.slm_PromotionId } into r0
                                            from pca in r0.DefaultIfEmpty()
                                                //where pc.slm_Seq > 2
                                            where ((pc.slm_PromotionId != 0 && pc.slm_PromotionId != null) || ((pc.slm_PromotionId == 0 || pc.slm_PromotionId == null) && pc.slm_Seq >= 2))
                                            select new { pre, pc, pca }
                   ).Union(
                       from pre in sdc.kkslm_tr_prelead
                       join pca in sdc.kkslm_tr_prelead_compare_act on pre.slm_Prelead_Id equals pca.slm_Prelead_Id
                       join pc in sdc.kkslm_tr_prelead_compare.Where(a => a.slm_PromotionId != 0 && a.slm_PromotionId != null) on new { prid = (decimal?)pre.slm_Prelead_Id, prm = pca.slm_PromotionId } equals new { prid = pc.slm_Prelead_Id, prm = pc.slm_PromotionId } into r1
                       from pc in r1.DefaultIfEmpty()
                       where ((pca.slm_PromotionId != 0 && pca.slm_PromotionId != null) || ((pca.slm_PromotionId == 0 || pca.slm_PromotionId == null) && pca.slm_Seq > 1))
                       select new { pre, pc, pca }
                   )
                             join ctyp in sdc.kkslm_ms_coveragetype on data4.pc.slm_CoverageTypeId equals ctyp.slm_CoverageTypeId into r3
                             from ctyp in r3.DefaultIfEmpty()
                             join rtyp in sdc.kkslm_ms_repairtype on data4.pc.slm_RepairTypeId equals rtyp.slm_RepairTypeId into r4
                             from rtyp in r4.DefaultIfEmpty()
                             where data4.pre.is_Deleted == false && preleadIDLst.Contains(data4.pre.slm_Prelead_Id)
                             orderby data4.pre.slm_Contract_Number
                             select new
                             {
                                 ContractNo = data4.pre.slm_Contract_Number,
                                 CompareId = data4.pc != null ? data4.pc.slm_PreLeadCompareId : 0,
                                 CompareSeq = data4.pc != null ? data4.pc.slm_Seq : 0,
                                 CompareNotifyPremiumId = data4.pc != null ? data4.pc.slm_NotifyPremiumId : 0,
                                 CompareActId = data4.pca != null ? data4.pca.slm_PreLeadCompareActId : 0,
                                 CompareActSeq = data4.pca != null ? data4.pca.slm_Seq : 0,

                                 LicenseNo = data4.pre.slm_Car_License_No,
                                 ChassisNo = data4.pre.slm_Chassis_No,
                                 OwnerID = data4.pre.slm_Owner,

                                 CovtypeName = ctyp.slm_ConverageTypeName,
                                 RepareName = rtyp.slm_RepairTypeName,

                                 // pc
                                 slm_PromotionId = data4.pc == null ? data4.pca == null ? null : data4.pca.slm_PromotionId : data4.pc.slm_PromotionId,
                                 FT = data4.pc.slm_FT,
                                 OD = data4.pc.slm_OD,
                                 NetGrossPremium = data4.pc.slm_PolicyGrossPremium, //data4.pc.slm_NetGrossPremium,
                                 PolicyGrossStamp = data4.pc.slm_PolicyGrossStamp,
                                 Vat1PercentBath = data4.pc.slm_Vat1PercentBath,
                                 PolicyGrossPremium = data4.pc.slm_NetGrossPremium, //data4.pc.slm_PolicyGrossPremium,
                                 PolicyGrossVat = data4.pc.slm_PolicyGrossVat,
                                 DiscountBath = data4.pc.slm_DiscountBath,
                                 DeDuctible = data4.pc.slm_DeDuctible,
                                 DeDuctibleFlag = data4.pc.slm_DeDuctibleFlag,
                                 MedicalMan = data4.pc.slm_MedicalFeeMan,
                                 MedicalFee = data4.pc.slm_MedicalFee,
                                 MedicalFeeDriver = data4.pc.slm_MedicalFeeDriver,
                                 MedicalFeePassng = data4.pc.slm_MedicalFeePassenger,
                                 InsuranceDriver = data4.pc.slm_InsuranceDriver,
                                 InjuryDeath = data4.pc.slm_InjuryDeath,
                                 PersonalAccident = data4.pc.slm_PersonalAccident,
                                 TPPD = data4.pc.slm_TPPD,
                                 CompId = data4.pc == null ? data4.pca == null ? 0 : data4.pca.slm_Ins_Com_Id : data4.pc.slm_Ins_Com_Id,
                                 PolicyGrossPremiumPay = data4.pc.slm_PolicyGrossPremiumPay,

                                 // pca
                                 ActGrossPremiumPay = data4.pca.slm_ActGrossPremiumPay,
                                 ACTNO = data4.pca.slm_ActNo
                             }
                           ).ToList();

                //Added by Pom 16/08/2016
                var distinctList = data3.Select(p => p.ContractNo).Distinct().ToList();
                data3 = data3.OrderBy(p => p.ContractNo).ToList();
                var mainList = data3.ToList();
                mainList.Clear();
                foreach (string contractNo in distinctList)
                {
                    var processList = data3.Where(p => p.ContractNo == contractNo).ToList();

                    var compareList = processList.Where(p => p.CompareId != 0).OrderBy(p => p.CompareSeq).ToList();
                    var removeList = compareList.Where(p => p.CompareSeq == 2 && (p.CompareNotifyPremiumId == null || p.CompareNotifyPremiumId == 0)).ToList();
                    removeList.ForEach(p => { compareList.Remove(p); });

                    var compareActList = processList.Where(p => p.CompareActId != 0).OrderBy(p => p.CompareActSeq).ToList();

                    compareList.ForEach(p => { mainList.Add(p); });
                    compareActList.ForEach(p => { mainList.Add(p); });
                }

                data3 = mainList.Distinct().ToList();

                #region OldCode
                //var data3 = (from pre in sdc.kkslm_tr_prelead
                //     join pc in sdc.kkslm_tr_prelead_compare on pre.slm_Prelead_Id equals pc.slm_Prelead_Id into r0 from pc in r0.DefaultIfEmpty()
                //     join pca in sdc.kkslm_tr_prelead_compare_act on pre.slm_Prelead_Id equals pca.slm_Prelead_Id /* new { plid = pre.slm_Prelead_Id, pflag = true } equals new { plid = pca.slm_Prelead_Id.Value, pflag = pca.slm_ActPurchaseFlag.Value }*/  into r1
                //     from pca in r1.DefaultIfEmpty()
                //     join ctyp in sdc.kkslm_ms_coveragetype on pc.slm_CoverageTypeId equals ctyp.slm_CoverageTypeId into r2
                //     from ctyp in r2.DefaultIfEmpty()
                //     join rtyp in sdc.kkslm_ms_repairtype on pc.slm_RepairTypeId equals rtyp.slm_RepairTypeId into r3
                //     from rtyp in r3.DefaultIfEmpty()
                //         //where pre.is_Deleted == false && pc.slm_Seq > 2 && preleadIDLst.Contains(pre.slm_Prelead_Id)
                //     where pre.is_Deleted == false && preleadIDLst.Contains(pre.slm_Prelead_Id) && 
                //     (
                //        (pca != null && pc != null && pc.slm_Seq > 2 && pc.slm_PromotionId == pca.slm_PromotionId && pca.slm_PromotionId != null && pca.slm_PromotionId != 0 )
                //        ||
                //        (pc != null && (pc.slm_PromotionId == null || pc.slm_PromotionId == 0) && pc.slm_Seq > 2 && pc.slm_PromotionId != pca.slm_PromotionId)
                //        ||
                //        (pca != null && (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) && pca.slm_Seq > 1 && pc.slm_PromotionId != pca.slm_PromotionId)
                //        //Add By Pom 2016/08/10
                //        ||
                //        (pca != null && pc != null && pc.slm_Seq > 2 && pca.slm_Seq > 1 && (pc.slm_PromotionId == null || pc.slm_PromotionId == 0) && (pca.slm_PromotionId == null || pca.slm_PromotionId == 0))
                //     )
                //     orderby pre.slm_Contract_Number
                //     select new
                //     {
                //         LicenseNo = pre.slm_Car_License_No,
                //         ChassisNo = pre.slm_Chassis_No,
                //         OwnerID = pre.slm_Owner,

                //         CovtypeName = ctyp.slm_ConverageTypeName,
                //         RepareName = rtyp.slm_RepairTypeName,

                //         // pc
                //         pc.slm_PromotionId,
                //         FT = pc.slm_FT,
                //         OD = pc.slm_OD,
                //         NetGrossPremium = (pca.slm_PromotionId ==  null || pca.slm_PromotionId == 0) || pc.slm_NetGrossPremium == null ? null : pc.slm_NetGrossPremium,
                //         PolicyGrossStamp = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PolicyGrossStamp == null ? null : pc.slm_PolicyGrossStamp,
                //         Vat1PercentBath = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_Vat1PercentBath == null ? null : pc.slm_Vat1PercentBath,
                //         PolicyGrossPremium = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PolicyGrossPremium == null ? null : pc.slm_PolicyGrossPremium,
                //         PolicyGrossVat = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PolicyGrossVat == null ? null : pc.slm_PolicyGrossVat,
                //         DiscountBath = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_DiscountBath == null ? null : pc.slm_DiscountBath,
                //         DeDuctible = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_DeDuctible == null ? null : pc.slm_DeDuctible,
                //         MedicalMan = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_MedicalFeeMan == null ? null : pc.slm_MedicalFeeMan,
                //         MedicalFee = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_MedicalFee == null ? null : pc.slm_MedicalFee,
                //         MedicalFeeDriver = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_MedicalFeeDriver == null ? null : pc.slm_MedicalFeeDriver,
                //         MedicalFeePassng = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_MedicalFeePassenger == null ? null : pc.slm_MedicalFeePassenger,
                //         InsuranceDriver = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_InsuranceDriver == null ? null : pc.slm_InsuranceDriver,
                //         InjuryDeath = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_InjuryDeath == null ? null : pc.slm_InjuryDeath,
                //         PersonalAccident = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PersonalAccident == null ? null : pc.slm_PersonalAccident,
                //         TPPD = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_TPPD == null ? null : pc.slm_TPPD,
                //         CompId = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0)  ? 0 : pc.slm_Ins_Com_Id,
                //         PolicyGrossPremiumPay = (pca.slm_PromotionId == null || pca.slm_PromotionId == 0) || pc.slm_PolicyGrossPremiumPay == null ? null : pc.slm_PolicyGrossPremiumPay,

                //         // pca
                //         ActGrossPremiumPay = (pc.slm_PromotionId == null || pc.slm_PromotionId == 0) || pca.slm_ActGrossPremiumPay == null ? null : pca.slm_ActGrossPremiumPay,
                //         ACTNO = (pc.slm_PromotionId == null || pc.slm_PromotionId == 0) || pca.slm_ActNo == null ? null : pca.slm_ActNo
                //     }
                //    ).ToList();
                #endregion

                List<kkslm_ms_ins_com> comlst;

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    comlst = odc.kkslm_ms_ins_com.ToList();

                    rowlst = new string[] { "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", 
                        "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", 
                        "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74",
                        "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "86", "87", "88", "89", "90",
                        "91", "92", "93", "94", "95", "96", "97", "98", "99", "100", "101", "102", "103", "104", "105", "106", "107", "108"
                        , "109", "110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "120"
                        , "121", "122", "123", "124", "125", "126", "127", "128", "129", "130", "131", "132"
                        , "133", "134", "135", "136", "137", "138", "139", "140", "141", "142", "143", "144"
                        , "145", "146", "147", "148", "149","150", "151", "152", "153", "154", "155", "156", "157", "158", "159"
                        , "160", "161", "162", "163", "164", "165"
                        , "166", "167", "168", "169", "170", "171", "172", "173", "174" };
                    for (int i = 0; i < rowlst.Length; i++)
                    {
                        var row = rowlst[i];
                        if (data3.Count > i)
                        {
                            var d = data3[i];
                            var com = comlst.Where(c => c.slm_Ins_Com_Id == d.CompId).FirstOrDefault();
                            var promo = odc.kkslm_ms_promotioninsurance.Where(p => p.slm_PromotionId == d.slm_PromotionId).FirstOrDefault();

                            lst.Add(new ExcelData() { ColumnName = "A" + row, Value = string.Format("{0}", d.CovtypeName) });
                            lst.Add(new ExcelData() { ColumnName = "B" + row, Value = string.Format("{0}", d.LicenseNo) });
                            lst.Add(new ExcelData() { ColumnName = "C" + row, Value = string.Format("{0}", d.ChassisNo) });
                            lst.Add(new ExcelData() { ColumnName = "D" + row, Value = string.Format("{0}", com == null ? "" : com.slm_InsNameTh) });
                            lst.Add(new ExcelData() { ColumnName = "E" + row, Value = string.Format("{0}", d.RepareName) });
                            lst.Add(new ExcelData() { ColumnName = "F" + row, Value = string.Format("{0}", d.OD) });
                            lst.Add(new ExcelData() { ColumnName = "G" + row, Value = string.Format("{0}", d.NetGrossPremium) });
                            lst.Add(new ExcelData() { ColumnName = "H" + row, Value = string.Format("{0}", d.PolicyGrossStamp) });
                            lst.Add(new ExcelData() { ColumnName = "I" + row, Value = string.Format("{0}", d.PolicyGrossVat) });
                            lst.Add(new ExcelData() { ColumnName = "J" + row, Value = d.Vat1PercentBath == null ? null : string.Format("{0}", d.Vat1PercentBath) });
                            lst.Add(new ExcelData() { ColumnName = "K" + row, Value = string.Format("{0}", d.PolicyGrossPremium) });
                            lst.Add(new ExcelData() { ColumnName = "L" + row, Value = string.Format("{0}", d.DiscountBath <= 0 ? null : d.DiscountBath) });
                            //lst.Add(new ExcelData() { ColumnName = "M" + row, Value = string.Format("{0}", (d.PolicyGrossPremium ?? 0) - (d.DiscountBath ?? 0) /* d.ActGrossPremiumPay */) });
                            lst.Add(new ExcelData() { ColumnName = "M" + row, Value = string.Format("{0}", d.PolicyGrossPremiumPay) });
                            lst.Add(new ExcelData() { ColumnName = "N" + row, Value = d.ActGrossPremiumPay == null ? null : string.Format("{0}", d.ActGrossPremiumPay) });
                            //lst.Add(new ExcelData() { ColumnName = "O" + row, Value = string.Format("{0}", d.DeDuctible) });
                            lst.Add(new ExcelData() { ColumnName = "O" + row, Value = string.Format("{0}", d.DeDuctibleFlag) });
                            lst.Add(new ExcelData() { ColumnName = "P" + row, Value = string.Format("{0}", d.FT) });
                            lst.Add(new ExcelData() { ColumnName = "Q" + row, Value = string.Format("{0}", d.InjuryDeath == 0 ? null : d.InjuryDeath) });
                            lst.Add(new ExcelData() { ColumnName = "R" + row, Value = null });
                            lst.Add(new ExcelData() { ColumnName = "S" + row, Value = string.Format("{0}", d.TPPD == 0 ? null : d.TPPD) });
                            lst.Add(new ExcelData() { ColumnName = "T" + row, Value = string.Format("{0}", promo == null ? null : (int?)(SLMUtil.SafeInt(promo.slm_MedicalFeeDriver) + SLMUtil.SafeInt(promo.slm_MedicalFeePassenger))) });
                            lst.Add(new ExcelData() { ColumnName = "U" + row, Value = string.Format("{0}", d.PersonalAccident == 0 ? null : d.PersonalAccident) });
                            lst.Add(new ExcelData() { ColumnName = "V" + row, Value = string.Format("{0}", d.MedicalFee == 0 ? null : d.MedicalFee) });
                            lst.Add(new ExcelData() { ColumnName = "W" + row, Value = string.Format("{0}", d.InsuranceDriver == 0 ? null : d.InsuranceDriver) });
                            lst.Add(new ExcelData() { ColumnName = "X" + row, Value = "" });
                        }
                    }
                }

                if (data3.Count > 0)
                {
                    //Add by Pom 16/08/2016
                    string stafflic = "";
                    string loginNameTH = "";
                    string loginPositionName = "";
                    string loginTelNo = "";

                    var loginUser = sdc.kkslm_ms_staff.Where(p => p.slm_UserName == loginUsername).FirstOrDefault();
                    if (loginUser != null)
                    {
                        loginNameTH = loginUser.slm_StaffNameTH;
                        loginTelNo = loginUser.slm_TellNo;

                        var positionName = sdc.kkslm_ms_position.Where(p => p.slm_Position_id == loginUser.slm_Position_id).Select(p => p.slm_PositionNameTH).FirstOrDefault();
                        loginPositionName = positionName != null ? positionName : "";

                        if (!string.IsNullOrEmpty(loginUser.slm_EmpCode))
                        {
                            var lic = sdc.kkslm_ms_mapping_staff_license.Where(l => l.slm_EmpCode == loginUser.slm_EmpCode).ToList();
                            foreach (var lc in lic)
                            {
                                if (stafflic != "") stafflic += ", ";
                                stafflic += lc.slm_LicenseNo;
                            }
                        }
                    }

                    lst.Add(new ExcelData() { ColumnName = "M177", Value = loginNameTH });
                    lst.Add(new ExcelData() { ColumnName = "M178", Value = string.Format("{0}/{1}", loginPositionName, loginTelNo) });
                    lst.Add(new ExcelData() { ColumnName = "M179", Value = string.Format("บัตรนายหน้าเลขที่ {0}", stafflic) });


                    //string ownr = data3[0].OwnerID;
                    //var staff = sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode == ownr).FirstOrDefault();
                    //if (staff != null)
                    //{
                    //    lst.Add(new ExcelData() { ColumnName = "M176", Value = staff.slm_StaffNameTH });
                    //    lst.Add(new ExcelData() { ColumnName = "M177", Value = string.Format("{0}/{1}", staff.slm_PositionName, staff.slm_TellNo) });

                    //    var lic = sdc.kkslm_ms_mapping_staff_license.Where(l => l.slm_EmpCode == staff.slm_EmpCode).Select(l => l.slm_LicenseNo).ToArray();
                    //    lst.Add(new ExcelData() { ColumnName = "M178", Value = string.Format("บัตรนายหน้าเลขที่ {0}", string.Join(",", lic)) });
                    //}
                }
                    
            }
            return lst;
        }

        /// <summary>
        /// ฟอร์มตัดบัตรเครดิต
        /// </summary>
        public List<ExcelData> GetExcelData003(string ticketID)
        {
            List<ExcelData> lst = new List<ExcelData>();
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var data = (from lead in sdc.kkslm_tr_lead
                            join tt in sdc.kkslm_ms_title on lead.slm_TitleId equals tt.slm_TitleId into r1
                            from tt in r1.DefaultIfEmpty()
                            join ri in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ri.slm_TicketId into r2
                            from ri in r2.DefaultIfEmpty()
                            join rioa in sdc.kkslm_tr_renewinsurance_address.Where(a => a.slm_AddressType == "O") on ri.slm_RenewInsureId equals rioa.slm_RenewInsureId into r3
                            from rioa in r3.DefaultIfEmpty()
                            join rida in sdc.kkslm_tr_renewinsurance_address.Where(a => a.slm_AddressType == "D") on ri.slm_RenewInsureId equals rida.slm_RenewInsureId into r4
                            from rida in r4.DefaultIfEmpty()
                            join br in sdc.kkslm_ms_redbook_brand on ri.slm_RedbookBrandCode equals br.slm_BrandCode into r6
                            from br in r6.DefaultIfEmpty()
                            join mo in sdc.kkslm_ms_redbook_model on ri.slm_RedbookModelCode equals mo.slm_ModelCode into r5
                            from mo in r5.DefaultIfEmpty()
                            join cus in sdc.kkslm_tr_cusinfo on lead.slm_ticketId equals cus.slm_TicketId into r7
                            from cus in r7.DefaultIfEmpty()
                            join stf in sdc.kkslm_ms_staff on lead.slm_Owner equals stf.slm_UserName into r8
                            from stf in r8.DefaultIfEmpty()
                            join opv in sdc.kkslm_ms_province on rioa.slm_Province equals opv.slm_ProvinceId into r9
                            from opv in r9.DefaultIfEmpty()
                            join oam in sdc.kkslm_ms_amphur on rioa.slm_Amphur equals oam.slm_AmphurId into r10
                            from oam in r10.DefaultIfEmpty()
                            join otm in sdc.kkslm_ms_tambol on rioa.slm_Tambon equals otm.slm_TambolId into r11
                            from otm in r11.DefaultIfEmpty()
                            join dpv in sdc.kkslm_ms_province on rida.slm_Province equals dpv.slm_ProvinceId into r12
                            from dpv in r12.DefaultIfEmpty()
                            join dam in sdc.kkslm_ms_amphur on rida.slm_Amphur equals dam.slm_AmphurId into r13
                            from dam in r13.DefaultIfEmpty()
                            join dtm in sdc.kkslm_ms_tambol on rida.slm_Tambon equals dtm.slm_TambolId into r14
                            from dtm in r14.DefaultIfEmpty()
                            where lead.slm_ticketId == ticketID
                            select new
                            {
                                ri.slm_RenewInsureId,
                                lead.slm_ticketId,
                                tt.slm_TitleName,
                                lead.slm_Name,
                                lead.slm_LastName,
                                ri.slm_ContractNo,
                                br.slm_BrandName,
                                mo.slm_ModelName,
                                ri.slm_LicenseNo,
                                hometel = cus.slm_TelNo_2,
                                telno1 = lead.slm_TelNo_1,
                                o_houseno = rioa.slm_House_No,      //rioa.slm_AddressNo,
                                o_building = rioa.slm_BuildingName,
                                o_floor = rioa.slm_Floor,
                                o_soi = rioa.slm_Soi,
                                o_street = rioa.slm_Street,
                                o_tambon = otm.slm_TambolNameTH,
                                o_amphur = oam.slm_AmphurNameTH,
                                o_province = opv.slm_ProvinceNameTH,
                                o_postcode = rioa.slm_PostalCode,
                                d_houseno = rida.slm_House_No,      //rida.slm_AddressNo,
                                d_building = rida.slm_BuildingName,
                                d_floor = rida.slm_Floor,
                                d_soi = rida.slm_Soi,
                                d_street = rida.slm_Street,
                                d_tambon = dtm.slm_TambolNameTH,
                                d_amphur = dam.slm_AmphurNameTH,
                                d_province = dpv.slm_ProvinceNameTH,
                                d_postcode = rida.slm_PostalCode,
                                ri.slm_PolicyGrossPremium,
                                ri.slm_ActGrossPremium,
                                ri.slm_ActNetPremium,
                                ri.slm_ActPayMethodId,
                                ri.slm_ActamountPeriod,
                                ri.slm_InsuranceComId,
                                ri.slm_PolicyPayMethodId,
                                ri.slm_PolicyAmountPeriod,
                                stf.slm_StaffNameTH,
                                stf.slm_StaffEmail,
                                stf.slm_TellNo
                            }
                            ).FirstOrDefault();
                string offaddr = GetAddress(data.o_houseno, "", data.o_building, data.o_floor, "", data.o_soi, data.o_street, data.o_tambon, data.o_amphur, data.o_province, data.o_postcode);
                string docaddr = GetAddress(data.d_houseno, "", data.d_building, data.d_floor, "", data.d_soi, data.d_street, data.d_tambon, data.d_amphur, data.d_province, data.d_postcode);
                decimal? paytotal = (data.slm_PolicyGrossPremium != null ? data.slm_PolicyGrossPremium : 0 ) + (data.slm_ActGrossPremium != null ? data.slm_ActGrossPremium : 0);

                string actperpay = "";
                var d1 = sdc.kkslm_tr_renewinsurance_paymentmain.Where(r => r.slm_Type == "2" && r.slm_RenewInsureId == data.slm_RenewInsureId).ToList().Select(r => r.slm_PaymentAmount).ToList();
                foreach (var dd in d1)
                {
                    if (dd != null)
                    {
                        if (actperpay != "") actperpay += ", ";
                        actperpay += dd.Value.ToString("#,##0.00");
                    }

                }

                string polperpay = "";
                var d2 = sdc.kkslm_tr_renewinsurance_paymentmain.Where(r => r.slm_Type == "1" && r.slm_RenewInsureId == data.slm_RenewInsureId).ToList().Select(r => r.slm_PaymentAmount).ToList();
                foreach (var dd in d2)
                {
                    if (dd != null)
                    {
                        if (polperpay != "") polperpay += ", ";
                        polperpay += dd.Value.ToString("#,##0.00");
                    }
                }






                lst.Add(new ExcelData() { ColumnName = "A4", Value = String.Format("วันที่ {0}", DateTime.Now.ToString("dd/MM/yyyy")) });
                //lst.Add(new ExcelData() { ColumnName = "A5", Value = String.Format("ชื่อ – นามสกุล  ลูกค้า {0}{1} {2}   เลขที่สัญญาเช่าซื้อ {3}   ", data.slm_TitleName, data.slm_Name, data.slm_LastName, data.slm_ContractNo, data.slm_ticketId) });
                lst.Add(new ExcelData() { ColumnName = "A5", Value = String.Format("ชื่อ – นามสกุล  ลูกค้า {0}{1} {2}   เลขที่สัญญาเช่าซื้อ {3}   ", data.slm_TitleName, data.slm_Name, data.slm_LastName, data.slm_ContractNo) });
                lst.Add(new ExcelData() { ColumnName = "A6", Value = String.Format("ยี่ห้อรถ / รุ่น {0}/{1}   เลขทะเบียนรถ {2}   โทรศัพท์บ้าน {3}   โทรศัพท์มือถือ {4}", data.slm_BrandName, data.slm_ModelName, data.slm_LicenseNo, data.hometel, data.telno1) });
                lst.Add(new ExcelData() { ColumnName = "A7", Value = String.Format("ที่ทำงาน {0}   ที่อยู่ในการส่งเอกสาร  : {1}", offaddr, docaddr) });
                lst.Add(new ExcelData() { ColumnName = "A9", Value = String.Format("ค่าเบี้ยประกันรถยนต์ภาคสมัครใจ จำนวนเงิน {0:#,##0.00} บาท  เงื่อนไขการชำระ ", paytotal) });
                lst.Add(new ExcelData() { ColumnName = "A10", Value = String.Format("   {0} ค่า พ.ร.บ.  จำนวนเงิน {1} บาท", data.slm_ActGrossPremium != null ? "(/)" : "( )", data.slm_ActGrossPremium) });
                lst.Add(new ExcelData() { ColumnName = "A11", Value = String.Format("         {0} ชำระครั้งเดียว        {1} ผ่อน {2} งวดละ {3:#,##0.00} บาท ", /* (data.slm_ActPayMethodId == 1 ? "[/]" : "[ ]"), (data.slm_ActPayMethodId == 2 ? "[/]" : "[ ]"),*/"[ ]","[ ]", data.slm_ActamountPeriod, actperpay /* actperpay */) });
                lst.Add(new ExcelData() { ColumnName = "A12", Value = String.Format("   {0} ค่าเบี้ยประกันอื่นๆ (ระบุ) {1} จำนวนเงิน {2} บาท  เงื่อนไขการชำระ", (data.slm_InsuranceComId != null ? "(/)" : "( )"), (data.slm_InsuranceComId != null ? "ค่าเบี้ยประกันภัยปีต่อ" : "     "), data.slm_PolicyGrossPremium) });
                lst.Add(new ExcelData() { ColumnName = "A13", Value = String.Format("         {0} ชำระครั้งเดียว        {1} ผ่อน {2} งวดละ {3:#,##0.00} บาท ", /*(data.slm_PolicyPayMethodId == 1 ? "[/]" : "[ ]"), (data.slm_PolicyPayMethodId == 2 ? "[/]" : "[ ]"),*/"[ ]", "[ ]", data.slm_PolicyAmountPeriod, polperpay /* policyperpay */) });
                lst.Add(new ExcelData() { ColumnName = "A15", Value = String.Format("รวมยอดชำระทั้งสิ้น {0:#,##0.00} บาท ({1})", paytotal, paytotal == null ? "" : GetMoneyText(Convert.ToDouble(paytotal.Value))) });
                lst.Add(new ExcelData() { ColumnName = "A33", Value = String.Format("ชื่อพนักงานขาย {0}", data.slm_StaffNameTH) });
                lst.Add(new ExcelData() { ColumnName = "E36", Value = String.Format("                     E-mail : {0}", data.slm_StaffEmail) });
                lst.Add(new ExcelData() { ColumnName = "E37", Value = String.Format("                     โทร : {0}", data.slm_TellNo) });
            }
            return lst;
        }

        /// <summary>
        /// ฟอร์ม 50 ทวิ
        /// </summary>
        public List<ExcelData> GetExcelData004(string ticketID)
        {
            List<ExcelData> lst = new List<ExcelData>();
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {

                var data = (from lead in sdc.kkslm_tr_lead
                            join tt in sdc.kkslm_ms_title on lead.slm_TitleId equals tt.slm_TitleId into r1
                            from tt in r1.DefaultIfEmpty()
                            join ri in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ri.slm_TicketId into r2
                            from ri in r2.DefaultIfEmpty()
                            join ria in sdc.kkslm_tr_renewinsurance_address.Where(a => a.slm_AddressType == "R") on ri.slm_RenewInsureId equals ria.slm_RenewInsureId into r3
                            from ria in r3.DefaultIfEmpty()
                            join rtb in sdc.kkslm_ms_tambol on ria.slm_Tambon equals rtb.slm_TambolId into r5
                            from rtb in r5.DefaultIfEmpty()
                            join ram in sdc.kkslm_ms_amphur on ria.slm_Amphur equals ram.slm_AmphurId into r6
                            from ram in r6.DefaultIfEmpty()
                            join rpv in sdc.kkslm_ms_province on ria.slm_Province equals rpv.slm_ProvinceId into r7
                            from rpv in r7.DefaultIfEmpty()
                            
                            where lead.slm_ticketId == ticketID
                            select new
                            {
                                lead.slm_ticketId,
                                tt.slm_TitleName,
                                lead.slm_Name,
                                lead.slm_LastName,
                                r_addr = ria.slm_House_No,      //ria.slm_AddressNo,
                                r_building = ria.slm_BuildingName,
                                r_floor = ria.slm_Floor,
                                r_soi = ria.slm_Soi,
                                r_street = ria.slm_Street,
                                r_tambon = rtb.slm_TambolNameTH,
                                r_amphur = ram.slm_AmphurNameTH,
                                r_province = rpv.slm_ProvinceNameTH,
                                r_postcode = ria.slm_PostalCode,
                                ri.slm_PolicyGrossPremium,
                                ri.slm_PolicyGrossVat,
                                ri.slm_InsuranceComId,
                                ri.slm_Vat1PercentBath,      //Add By Pom 2016/08/10
                                ri.slm_PolicyNetGrossPremium,
                                ri.slm_PolicyGrossStamp,
                                ri.slm_ActGrossPremium,
                                ri.slm_ActVat1Percent,
                                ri.slm_ActVat,
                                ri.slm_ActVat1PercentBath
                            }).FirstOrDefault();

                if (data == null) throw new Exception("No Data");

                // get insdata
                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    var data2 = (from ins in odc.kkslm_ms_ins_com
                                 where ins.slm_Ins_Com_Id == data.slm_InsuranceComId
                                 select new
                                 {
                                     ins.slm_InsNameTh,
                                     ins.slm_InsTax,
                                     i_addr = ins.slm_AddressNo,
                                     i_moo = ins.slm_Moo,
                                     i_building = ins.slm_BuildingName,
                                     i_floor = ins.slm_Floor,
                                     i_soi = ins.slm_Soi,
                                     i_street = ins.slm_Road,
                                     i_postcode = ins.slm_PostCode,
                                     ins.slm_TambolId,
                                     ins.slm_AmphurId,
                                     ins.slm_ProvinceId

                                 }).FirstOrDefault();


                    string itb = "";
                    string iam = "";
                    string ipv = "";
                    string tx = "";

                    if (data2 != null)
                    {
                        itb = sdc.kkslm_ms_tambol.Where(t => t.slm_TambolId == data2.slm_TambolId).Select(t => t.slm_TambolNameTH).FirstOrDefault();
                        iam = sdc.kkslm_ms_amphur.Where(a => a.slm_AmphurId == data2.slm_AmphurId).Select(a => a.slm_AmphurNameTH).FirstOrDefault();
                        ipv = sdc.kkslm_ms_province.Where(p => p.slm_ProvinceId == data2.slm_ProvinceId).Select(p => p.slm_ProvinceNameTH).FirstOrDefault();
                        tx = data2.slm_InsTax;
                    }

                    // gen data
                    decimal premium = (data.slm_PolicyNetGrossPremium != null ? data.slm_PolicyNetGrossPremium.Value : 0)
                                        + (data.slm_PolicyGrossStamp != null ? data.slm_PolicyGrossStamp.Value : 0);

                    string gp, gs, acp, acs;  // เบี้ยประกัน
                    string vv, vs, avv, avs;  // เบี้ย พรบ.
                    string tv, ts, tvv, tvs;  // เบี้ย รวม

                    // คำนวน เบี้ยประกัน
                    var tmp_premium = premium.ToString("0.00").Split('.');
                    gp = tmp_premium[0];
                    gs = tmp_premium[1];

                    if (data.slm_Vat1PercentBath != null)
                    {
                        var tmp = data.slm_Vat1PercentBath.Value.ToString("0.00").Split('.');
                        vv = tmp[0];
                        vs = tmp[1];
                    }
                    else
                    {
                        vv = "";
                        vs = "";
                    }


                    // คำนวน เบี้ยพรบ
                    var tmp_actpremium = (data.slm_ActGrossPremium ?? 0).ToString("0.00").Split('.');
                    acp = tmp_actpremium[0];
                    acs = tmp_actpremium[1];

                    if (data.slm_ActVat1PercentBath != null)
                    {
                        var tmp = data.slm_ActVat1PercentBath.Value.ToString("0.00").Split('.');
                        avv = tmp[0];
                        avs = tmp[1];
                    }
                    else
                    {
                        avv = "";
                        avs = "";
                    }


                    // คำนวน เบี้ยรวม
                    decimal total = premium;
                    if (data.slm_ActVat1Percent == true && data.slm_ActGrossPremium != null && data.slm_ActGrossPremium.Value > 0)
                    {
                        total += data.slm_ActGrossPremium.Value;
                    }
                    {
                        var tmp = total.ToString("0.00").Split('.');
                        tv = tmp[0];
                        ts = tmp[1];
                    }

                    var totalvat1p = data.slm_Vat1PercentBath ?? 0 + data.slm_ActVat1PercentBath ?? 0;
                    {
                        var tmp = totalvat1p.ToString("0.00").Split('.');
                        tvv = tmp[0];
                        tvs = tmp[1];
                    }

                    //if (data.slm_PolicyGrossPremium != null)
                    //{
                    //    var tmp = data.slm_PolicyGrossPremium.Value.ToString("0.00").Split('.');
                    //    gp = tmp[0];
                    //    gs = tmp[1];
                    //}
                    //else
                    //{
                    //    gp = "";
                    //    gs = "";
                    //}

                    //Add By Pom 2016/08/10


                    

                    //Comment By Pom 2016/08/10
                    //if (data.slm_PolicyGrossVat != null)
                    //{
                    //    var tmp = data.slm_PolicyGrossVat.Value.ToString("0.00").Split('.');
                    //    vv = tmp[0];
                    //    vs = tmp[1];
                    //}
                    //else
                    //{
                    //    vv = "";
                    //    vs = "";
                    //}

                    //double total = Convert.ToDouble((data.slm_PolicyGrossPremium == null ? 0 : data.slm_PolicyGrossPremium.Value) + (data.slm_PolicyGrossVat == null ? 0 : data.slm_PolicyGrossVat.Value));
                    double totalVat = data.slm_Vat1PercentBath != null ? Convert.ToDouble(data.slm_Vat1PercentBath.Value) : 0;

                    // fix data
                    lst.Add(new ExcelData() { ColumnName = "D5", Value = String.Format("{0}{1} {2}", data.slm_TitleName, data.slm_Name, data.slm_LastName) });            // customername
                    lst.Add(new ExcelData() { ColumnName = "D7", Value = String.Format("{0}", GetAddress(data.r_addr, "", data.r_building, data.r_floor, "", data.r_soi, data.r_street, data.r_tambon, data.r_amphur, data.r_province, data.r_postcode)) });            // address
                    lst.Add(new ExcelData() { ColumnName = "D11", Value = String.Format("{0}", data2 == null ? "" : data2.slm_InsNameTh) });            // insname
                    lst.Add(new ExcelData() { ColumnName = "D13", Value = String.Format("{0}", data2 == null ? "" : GetAddress(data2.i_addr, data2.i_moo, data2.i_building, data2.i_floor, "", data2.i_soi, data2.i_street, itb, iam, ipv, data2.i_postcode)) });            // insaddress
                    lst.Add(new ExcelData() { ColumnName = "V11", Value = String.Format("{0}", tx != null && tx.Length > 0 ? tx[0].ToString() : "") });            // instax1
                    lst.Add(new ExcelData() { ColumnName = "W11", Value = String.Format("{0}", tx != null && tx.Length > 1 ? tx[1].ToString() : "") });            // instax2
                    lst.Add(new ExcelData() { ColumnName = "X11", Value = String.Format("{0}", tx != null && tx.Length > 2 ? tx[2].ToString() : "") });            // instax3
                    lst.Add(new ExcelData() { ColumnName = "Y11", Value = String.Format("{0}", tx != null && tx.Length > 3 ? tx[3].ToString() : "") });            // instax4
                    lst.Add(new ExcelData() { ColumnName = "Z11", Value = String.Format("{0}", tx != null && tx.Length > 4 ? tx[4].ToString() : "") });            // instax5
                    lst.Add(new ExcelData() { ColumnName = "AA11", Value = String.Format("{0}", tx != null && tx.Length > 5 ? tx[5].ToString() : "") });            // instax6
                    lst.Add(new ExcelData() { ColumnName = "AB11", Value = String.Format("{0}", tx != null && tx.Length > 6 ? tx[6].ToString() : "") });            // instax7
                    lst.Add(new ExcelData() { ColumnName = "AC11", Value = String.Format("{0}", tx != null && tx.Length > 7 ? tx[7].ToString() : "") });            // instax8
                    lst.Add(new ExcelData() { ColumnName = "AD11", Value = String.Format("{0}", tx != null && tx.Length > 8 ? tx[8].ToString() : "") });            // instax9
                    lst.Add(new ExcelData() { ColumnName = "AE11", Value = String.Format("{0}", tx != null && tx.Length > 9 ? tx[9].ToString() : "") });            // instax10
                    lst.Add(new ExcelData() { ColumnName = "AF11", Value = String.Format("{0}", tx != null && tx.Length > 10 ? tx[10].ToString() : "") });            // instax11
                    lst.Add(new ExcelData() { ColumnName = "AG11", Value = String.Format("{0}", tx != null && tx.Length > 11 ? tx[11].ToString() : "") });            // instax12
                    lst.Add(new ExcelData() { ColumnName = "AH11", Value = String.Format("{0}", tx != null && tx.Length > 12 ? tx[12].ToString() : "") });            // instax13
                    lst.Add(new ExcelData() { ColumnName = "Y35", Value = String.Format("{0}", gp) });            // grosspremium amount
                    lst.Add(new ExcelData() { ColumnName = "AC35", Value = String.Format("{0}", gs) });            // grosspremium stang
                    lst.Add(new ExcelData() { ColumnName = "AD35", Value = String.Format("{0}", vv) });            // grossvat amount
                    lst.Add(new ExcelData() { ColumnName = "AH35", Value = String.Format("{0}", vs) });            // grossvat stang

                    lst.Add(new ExcelData() { ColumnName = "Y37", Value = String.Format("{0}", tv) });            // grosspremium total amount
                    lst.Add(new ExcelData() { ColumnName = "AC37", Value = String.Format("{0}", ts) });            // grosspremium total stang
                    lst.Add(new ExcelData() { ColumnName = "AD37", Value = String.Format("{0}", tvv) });            // grossvat total amount
                    lst.Add(new ExcelData() { ColumnName = "AH37", Value = String.Format("{0}", tvs) });            // grossvat total stang
                    lst.Add(new ExcelData() { ColumnName = "K39", Value = String.Format("{0}", GetMoneyText(totalVat)) });            // paybaththai

                    if (data.slm_ActVat1Percent == true && (data.slm_ActGrossPremium ?? 0) > 0) lst.Add(new ExcelData() { ColumnName = "C36", Value = string.Format("{0}", " ค่าพรบ.") });
                    if (data.slm_ActVat1Percent == true && (data.slm_ActGrossPremium ?? 0) > 0) lst.Add(new ExcelData() { ColumnName = "Y36", Value = string.Format("{0}", acp) });
                    if (data.slm_ActVat1Percent == true && (data.slm_ActGrossPremium ?? 0) > 0) lst.Add(new ExcelData() { ColumnName = "AC36", Value = string.Format("{0}", acs) });
                    if (data.slm_ActVat1Percent == true && (data.slm_ActGrossPremium ?? 0) > 0) lst.Add(new ExcelData() { ColumnName = "AD36", Value = string.Format("{0}", avv) });
                    if (data.slm_ActVat1Percent == true && (data.slm_ActGrossPremium ?? 0) > 0) lst.Add(new ExcelData() { ColumnName = "AH36", Value = string.Format("{0}", avs) });
                    //lst.Add(new ExcelData() { ColumnName = "K39", Value = String.Format("{0}", GetMoneyText(total)) });            // paybaththai

                }

            }
            return lst;
        }

        /// <summary>
        /// ฟอร์มแก้ไขใบเสร็จ
        /// </summary>
        public List<ExcelData> GetExcelData005(string ticketID, string recno)
        {
            List<ExcelData> lst = new List<ExcelData>();
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                var data = (from lead in sdc.kkslm_tr_lead
                            join tt in sdc.kkslm_ms_title on lead.slm_TitleId equals tt.slm_TitleId into r1 from tt in r1.DefaultIfEmpty()
                            join ri in sdc.kkslm_tr_renewinsurance on lead.slm_ticketId equals ri.slm_TicketId into r2 from ri in r2.DefaultIfEmpty()
                            join stf in sdc.kkslm_ms_staff on lead.slm_Owner equals stf.slm_UserName into r3 from stf in r3.DefaultIfEmpty()
                            join hstf in sdc.kkslm_ms_staff on stf.slm_HeadStaffId equals hstf.slm_StaffId into r4 from hstf in r4.DefaultIfEmpty()


                            where lead.slm_ticketId == ticketID  
                            select new
                            {
                                lead.slm_ticketId,
                                tt.slm_TitleName,
                                lead.slm_Name,
                                lead.slm_LastName,
                                ri.slm_ContractNo,
                                ri.slm_LicenseNo,
                                telesamename = stf.slm_StaffNameTH,
                                headtelesaelteam = hstf.slm_StaffNameTH,
                            }
                            ).FirstOrDefault();


                // fix data

                string cusname = String.Format("{0}{1} {2}", data.slm_TitleName, data.slm_Name, data.slm_LastName);
                //string printdate = DateTime.Now.ToString("dd/MM/yyyy");
                string printdate = DateTime.Now.ToString("dd/MM/") + DateTime.Now.Year.ToString();

                lst.Add(new ExcelData() { ColumnName = "F3", Value = String.Format("{0}", printdate ) });   // printdate
                lst.Add(new ExcelData() { ColumnName = "B8", Value = String.Format("{0}", cusname) });   // customername
                lst.Add(new ExcelData() { ColumnName = "E8", Value = String.Format("{0}", data.slm_ContractNo) });   // contractnumber
                lst.Add(new ExcelData() { ColumnName = "F8", Value = String.Format("ทะเบียน {0}", data.slm_LicenseNo) });   // licenseno
                lst.Add(new ExcelData() { ColumnName = "B12", Value = String.Format("{0}", cusname) });   // customername 1
                //lst.Add(new ExcelData() { ColumnName = "D12", Value = String.Format("{0}", printdate) });   // printdate 1
                lst.Add(new ExcelData() { ColumnName = "B13", Value = String.Format("{0}", recno) });   // recno
                lst.Add(new ExcelData() { ColumnName = "F12", Value = String.Format("{0}", cusname) });   // customername 2
                //lst.Add(new ExcelData() { ColumnName = "G12", Value = String.Format("{0}", printdate) });   // printdate 2
                lst.Add(new ExcelData() { ColumnName = "B31", Value = String.Format("{0}", data.telesamename) });   // telesalename
                lst.Add(new ExcelData() { ColumnName = "F31", Value = String.Format("{0}", data.headtelesaelteam) });   // headtelesalename

                // dynamic data

                // left
                var datal = (from rr in sdc.kkslm_tr_renewinsurance_receipt
                             join rd in sdc.kkslm_tr_renewinsurance_receipt_detail on rr.slm_RenewInsuranceReceiptId equals rd.slm_RenewInsuranceReceiptId
                             //join rrd in sdc.kkslm_tr_renewinsurance_receipt_revision_detail on rr.slm_RenewInsuranceReceiptId equals rrd.slm_RenewInsuranceReceiptId
                             where rr.slm_ticketId == ticketID && rr.slm_RecNo == recno
                             select rd).ToList();

                string strTranDate = "";
                var tranDate = datal.OrderByDescending(p => p.slm_RenewInsuranceReceiptDetailId).Select(p => p.slm_TransDate).FirstOrDefault();
                if (tranDate != null)
                {
                    //strTranDate = tranDate.Value.ToString("dd/MM/yyyy");
                    strTranDate = tranDate.Value.ToString("dd/MM/") + tranDate.Value.Year.ToString();
                }

                lst.Add(new ExcelData() { ColumnName = "D12", Value = String.Format("{0}", strTranDate) });
                lst.Add(new ExcelData() { ColumnName = "G12", Value = String.Format("{0}", strTranDate) });

                for (int i =0; i<6; i++)
                {
                    kkslm_tr_renewinsurance_receipt_detail rd;
                    if (datal.Count > i)
                        rd = datal[i];
                    else
                        rd = new kkslm_tr_renewinsurance_receipt_detail();

                    lst.Add(new ExcelData() { ColumnName = "B" + (14 + (2*i)).ToString(), Value = String.Format("{0}", rd.slm_PaymentCode) });
                    lst.Add(new ExcelData() { ColumnName = "B" + (15 + (2*i)).ToString(), Value = String.Format("{0:#,##0.00}", rd.slm_RecAmount) });
                }
                lst.Add(new ExcelData() { ColumnName = "B28", Value = String.Format("{0:#,##0.00}", datal.Sum(d => d.slm_RecAmount))});

                // right
                var datar = (from rr in sdc.kkslm_tr_renewinsurance_receipt
                             join rrd in sdc.kkslm_tr_renewinsurance_receipt_revision_detail on rr.slm_RenewInsuranceReceiptId equals rrd.slm_RenewInsuranceReceiptId
                             where rr.slm_Status != null && rr.slm_ticketId == ticketID && rr.slm_RecNo == recno
                             select rrd).ToList();


                for (int i = 0; i < 6; i++)
                {
                    kkslm_tr_renewinsurance_receipt_revision_detail rrd;
                    if (datar.Count > i)
                        rrd = datar[i];
                    else
                        rrd = new kkslm_tr_renewinsurance_receipt_revision_detail();

                    //lst.Add(new ExcelData() { ColumnName = "F" + (14 + (2*i)).ToString(), Value = String.Format("{0}", rrd.slm_PaymentCode) });
                    lst.Add(new ExcelData() { ColumnName = "F" + (14 + (2 * i)).ToString(), Value = String.Format("{0}", (rrd.slm_PaymentCode == "OTHER" ? rrd.slm_PaymentOtherDesc : rrd.slm_PaymentCode)) });
                    lst.Add(new ExcelData() { ColumnName = "F" + (15 + (2*i)).ToString(), Value = String.Format("{0:#,##0.00}", rrd.slm_RecAmount) });
                }
                lst.Add(new ExcelData() { ColumnName = "F28", Value = String.Format("{0:#,##0.00}", datar.Sum(d => d.slm_RecAmount)) });


            }
            return lst;
        }


        public partial class ExcelData
        {
            public string ColumnName { get; set; }
            public string Value { get; set; }
        }

        private string GetAddress(string addrno, string moo, string buildig, string floor, string village, string soi, string road, string tambon, string amphur, string province, string postcode)
        {
            string ret = "";

            bool isbkk = province != null && province.Contains("กรุงเทพ");
          
            ret += addrno;
            if (!String.IsNullOrEmpty(moo)) ret += "หมู่ " + moo;
            if (!String.IsNullOrEmpty(buildig)) ret += " อาคาร" + buildig;
            if (!String.IsNullOrEmpty(floor)) ret += " ชั้น " + floor;
            if (!String.IsNullOrEmpty(village)) ret += " หมู่บ้าน " + village;
            if (!String.IsNullOrEmpty(soi)) ret += " ซ." + soi;
            if (!String.IsNullOrEmpty(road)) ret += " ถ." + road;
            if (!String.IsNullOrEmpty(tambon)) ret += (isbkk ? " แขวง" : " ต.") + tambon;
            if (!String.IsNullOrEmpty(amphur)) ret += (isbkk ? " เขต" : " อ. ") + amphur;
            if (!String.IsNullOrEmpty(province)) ret += (isbkk ? " " : " จ.") + province;
            ret += " " + postcode;

            return ret;
        }

        public string GetMoneyText(double amount)
        {
            string text = "";
            if (amount > 0) text = NumberToCurrencyStang(amount);
            return text;
        }

        private string NumberToCurrencyStang(double amount)
        {
            string text = "";
            string unit = (amount >= 1 ? "บาท" : "");
            text += NumberToCurrency(amount) + unit;
            amount = Convert.ToDouble(amount.ToString("0.00"));
            double temp = (amount * 100) % 100;
            if (temp > 0)
                text += NumberToCurrency(temp) + "สตางค์";
            else
                text += (amount > 0 ? "ถ้วน" : "");
            return text;
        }

        private string NumberToCurrency(double amount)
        {
            string text = "";
            bool haveTen = false;
            amount = Math.Floor(Convert.ToDouble(amount.ToString("0.00")));
            string[] NumberText = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า", "สิบ" };
            string[] NumberText10 = { "ศูนย์", "", "ยี่", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า", "สิบ" };
            string[] NumberText1 = { "ศูนย์", "เอ็ด", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า", "สิบ" };
            string temp = amount.ToString("0");
            int number = 0;
            if (temp.Length > 6)
            {
                text = NumberToCurrency(Convert.ToDouble(temp.Substring(0, temp.Length - 6))) + "ล้าน";
                amount = Convert.ToDouble(temp.Substring(temp.Length - 6));
            }
            number = (Int32)Math.Floor(amount / 100000);
            if (number > 0)
            {
                text += NumberText[number] + "แสน";
                amount = amount % 100000;
            }
            number = (Int32)Math.Floor(amount / 10000);
            if (number > 0)
            {
                text += NumberText[number] + "หมื่น";
                amount = amount % 10000;
            }
            number = (Int32)Math.Floor(amount / 1000);
            if (number > 0)
            {
                text += NumberText[number] + "พัน";
                amount = amount % 1000;
            }
            number = (Int32)Math.Floor(amount / 100);
            if (number > 0)
            {
                text += NumberText[number] + "ร้อย";
                amount = amount % 100;
            }
            number = (Int32)Math.Floor(amount / 10);
            if (number > 0)
            {
                text += NumberText10[number] + "สิบ";
                amount = amount % 10;
                haveTen = true;
            }
            number = (Int32)amount;
            if (number > 0)
            {
                if (haveTen)
                    text += NumberText1[number];
                else
                    text += NumberText[number];
            }
            return text;
        }

    }

}
