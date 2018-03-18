using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using SLM.Dal;
using System.IO;
using SLM.Resource;
using System.Configuration;
using log4net;

namespace SLM.Biz
{
    public class OBTBatchBiz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        const string AppName = "SLMBatchReport";

        public bool GenReport001(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            status = "";
            string batchCode = "OBT_PRO_13";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string filename = folder + "\\" + "แจ้งออกBillPayment_" + dt.Year.ToString() + dt.ToString("MMdd_HHmm") + ".xls";
            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            int succ = 0;
            int fail = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            bool ret = true;
            try
            {

                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                    if (batch != null)
                    {
                        batch.slm_StartTime = starttime;
                        sdc.SaveChanges();
                    }


                    // get headers
                    List<string> strLst = new List<string>() { "ลำดับที่", "เลขที่สัญญา", "ชื่อ-นามสกุลลูกค้า", "ที่อยู่จัดส่งเอกสาร", "ที่อยู่เปลี่ยนแปลง/ตามระบบ", "ประกัน", "พรบ", "ประเภทการจัดส่ง", "ชื่อ MKT (ชื่อ-นามสกุล)", "รหัสพนักงาน", "ชื่อทีม", "Reference1", "Reference2" };
                    List<string> typLst = new List<string>() { "n", "c", "c", "c", "c", "n", "n", "c", "c", "c", "c", "c", "c" };
                    // get data
                    var data = (from r in sdc.kkslm_tr_bp_report
                               join t in sdc.kkslm_ms_title on r.slm_Title_Id equals t.slm_TitleId into r1
                               from t in r1.DefaultIfEmpty()
                               join tm in sdc.kkslm_ms_tambol on r.slm_TambolId equals tm.slm_TambolId into r2
                               from tm in r2.DefaultIfEmpty()
                               join am in sdc.kkslm_ms_amphur on r.slm_AmphurId equals am.slm_AmphurId into r3
                               from am in r3.DefaultIfEmpty()
                               join pv in sdc.kkslm_ms_province on r.slm_ProvinceId equals pv.slm_ProvinceId into r4
                               from pv in r4.DefaultIfEmpty()
                                   //join st in sdc.kkslm_ms_staff on r.slm_Owner equals st.slm_UserName into r5    //Comment by Pom  08/07/2016
                               join st in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on r.slm_Owner equals st.slm_EmpCode into r5
                               from st in r5.DefaultIfEmpty()
                               join tt in sdc.kkslm_ms_teamtelesales on (decimal)st.slm_TeamTelesales_Id equals tt.slm_TeamTelesales_Id into st1
                               from tt in st1.DefaultIfEmpty()
                                   //where r.slm_UpdatedDate >= curdate && r.slm_UpdatedDate < tomorow && r.slm_Export_Flag == false
                               where r.slm_Export_Flag == false || r.slm_Export_Flag == null
                               orderby st.slm_StaffNameTH
                               select new
                               {
                                   r.slm_Id,
                                   r.slm_Contract_Number,
                                   t.slm_TitleName,
                                   r.slm_Name,
                                   r.slm_LastName,
                                   r.slm_House_No,
                                   r.slm_Moo
                               ,
                                   r.slm_Building,
                                   r.slm_Village,
                                   r.slm_Soi,
                                   r.slm_Street,
                                   tm.slm_TambolNameTH,
                                   am.slm_AmphurNameTH,
                                   pv.slm_ProvinceNameTH
                               ,
                                   r.slm_Zipcode,
                                   r.slm_Address_Type,
                                   r.slm_InsCost,
                                   r.slm_ActCost,
                                   r.slm_Send_Type,
                                   st.slm_StaffNameTH,
                                   st.slm_EmpCode,
                                   tt.slm_TeamTelesales_Name,
                                   r.slm_Reference1,
                                   r.slm_Reference2
                               }).ToList();

                    List<object[]> dataLst = new List<object[]>();
                    int i = 1;
                    foreach (var itm in data)
                    {
                        var obj = new object[]
                        {
                            i,
                            //System.Text.Encoding.Default.GetString(itm.slm_Contract_Number),  //Comment by Pom 05/07/2016 - DataType Changed from Varbinary to Varchar
                            itm.slm_Contract_Number,
                            itm.slm_TitleName + itm.slm_Name + " " + itm.slm_LastName,
                            itm.slm_House_No
                                + (String.IsNullOrEmpty(itm.slm_Moo) ? "" : " หมู่ " + itm.slm_Moo)
                                + (String.IsNullOrEmpty(itm.slm_Building) ? "" : " อาคาร " + itm.slm_Building)
                                + (String.IsNullOrEmpty(itm.slm_Village) ? "" : " หมู่บ้าน " + itm.slm_Village)
                                + (String.IsNullOrEmpty(itm.slm_Soi) ? "" : " ซอย " + itm.slm_Soi)
                                + (String.IsNullOrEmpty(itm.slm_Street) ? "" : " ถนน " + itm.slm_Street)
                                + (String.IsNullOrEmpty(itm.slm_TambolNameTH) ? "" : " ตำบล " + itm.slm_TambolNameTH)
                                + (String.IsNullOrEmpty(itm.slm_AmphurNameTH) ? "" : " อำเภอ " + itm.slm_AmphurNameTH)
                                + (String.IsNullOrEmpty(itm.slm_ProvinceNameTH) ? "" : " จังหวัด " + itm.slm_ProvinceNameTH)
                                + itm.slm_Zipcode,
                            itm.slm_Address_Type == "001" ? "ที่อยู่เปลี่ยนแปลง" : itm.slm_Address_Type == "002" ? "ตามระบบ" : "",
                            itm.slm_InsCost == null ? "0.00" : itm.slm_InsCost.Value.ToString("#,##0.00"),
                            itm.slm_ActCost == null ? "0.00" : itm.slm_ActCost.Value.ToString("#,##0.00"),
                            itm.slm_Send_Type == "001" ? "ลงทะเบียน" : itm.slm_Send_Type == "002" ? "ธรรมดา" : "",
                            itm.slm_StaffNameTH,
                            itm.slm_EmpCode,
                            itm.slm_TeamTelesales_Name,
                            itm.slm_Reference1,
                            itm.slm_Reference2
                        };
                        dataLst.Add(obj);
                        i++;
                        succ++;

                    }
                    if (dataLst.Count == 0) status = "No data to generate";
                    else status = string.Format("Generated {0} files total {1} records", 1, dataLst.Count);

                    if (!writeExcel(filename, strLst, typLst, dataLst, rowperpage))
                    {
                        fail = data.Count();
                        errLst.Add(new kkslm_tr_batchlog() { slm_WorkDetailId = -1, slm_ErrorDetail = _error });
                    }
                    else
                    {
                        foreach (var itm in data)
                        {
                            // update flag
                            var rpt = sdc.kkslm_tr_bp_report.Where(r => r.slm_Id == itm.slm_Id).FirstOrDefault();
                            if (rpt != null)
                            {
                                rpt.slm_Export_Flag = true;
                                rpt.slm_Export_Date = DateTime.Now;
                                rpt.slm_UpdatedBy = AppName;
                                rpt.slm_UpdatedDate = DateTime.Now;
                            }
                        }
                    }
                    DateTime endtime = DateTime.Now;

                    // update batch
                    batch.slm_EndTime = endtime;
                    batch.slm_Status = (fail > 0 ? "2" : "1");

                    // insert batch monitor 
                    kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                    bm.slm_DateStart = starttime;
                    bm.slm_DateEnd = endtime;
                    bm.slm_BatchCode = batchCode;
                    bm.slm_Total = data.Count();
                    bm.slm_Success = succ;
                    bm.slm_Fail = fail;
                    bm.slm_ActionBy = AppName;
                    bm.slm_BatchDate = dt;
                    sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                    sdc.SaveChanges();


                    if (errLst.Count > 0)
                    {
                        // add errorlog
                        foreach (var itm in errLst)
                        {
                            itm.slm_BatchMonitor_Id = bm.slm_BatchMonitor_Id;
                            sdc.kkslm_tr_batchlog.AddObject(itm);
                        }

                        sdc.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.Message;
            }
            return ret;
        }

        public bool GenReport002(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            ILog _log = LogManager.GetLogger(typeof(OBTBatchBiz));//OBT_PRO_16
            status = "";
            string batchCode = "OBT_PRO_14";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            int succ = 0;
            int fail = 0;
            int total = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            bool ret = true;
            try
            {
                // get headers                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  //not count
                List<string> strLst = new List<string>() { "วันที่รับแจ้ง", "วันที่รับเงินค่าเบี้ยประกัน", "รายชื่อบริษัทประกันภัย", "เลขที่สัญญาเช่าซื้อ", "เลขที่กรมธรรม์เดิม", "รหัสสาขา", "สาขา KK", "เลขรับแจ้ง", "Campaign", "Sub Campaign" /* 10 */, "ประเภทบุคคล", "คำนำหน้าชื่อ", "ชื่อผู้เอาประกันภัย", "นามสกุลผู้เอาประกันภัย", "บ้านเลขที่", "หมู่", "หมู่บ้าน", "อาคาร", "ซอย", "ถนน" /* 20 */, "ตำบล/แขวง", "อำเภอ/เขต", "จังหวัด", "รหัสไปรษณีย์", "ประเภทความคุ้มครอง", "ประเภทการซ่อม", "วันเริ่มคุ้มครอง", "วันสิ้นสุดคุ้มครอง", "รหัสรถ", "ประเภทประกันภัยรถยนต์" /* 30 */, "ชื่อยี่ห้อรถ", "รุ่นรถ", "New/Used", "เลขทะเบียน", "เลขตัวถัง", "เลขเครื่องยนต์", "ปีรถยนต์", "ซีซี", "น้ำหนัก(ตัน)", "ทุนประกันปี 1" /* 40 */, "เบี้ยรวมภาษีและอากรปี 1", "ทุนประกันความเสียหายต่อตัวรถปี2(OD)", "ทุนประกันรถยนต์สูญหาย/ไฟไหม้ ปี2(F&T)", "เบี้ยรวมภาษีและอากรปี 2", "เวลารับแจ้ง", "ชื่อเจ้าหน้าที่ MKT", "หมายเหตุ", "ผู้ขับขี่ที่ 1 และวันเกิด", "ผู้ขับขี่ที่ 2 และวันเกิด", "คำนำหน้าชื่อ (ใบเสร็จ/ใบกำกับภาษี)", "ชื่อ (ใบเสร็จ/ใบกำกับภาษี)" /* 50 */, "บ้านเลขที่(ใบเสร็จ/ใบกำกับภาษี)", "หมู่บ้าน (ใบเสร็จ/ใบกำกับภาษี)", "อาคาร (ใบเสร็จ/ใบกำกับภาษี)", "ซอย (ใบเสร็จ/ใบกำกับภาษี)", "ถนน (ใบเสร็จ/ใบกำกับภาษี)", "ตำบล/แขวง (ใบเสร็จ/ใบกำกับภาษี)", "อำเภอ/เขต (ใบเสร็จ/ใบกำกับภาษี)", "จังหวัด (ใบเสร็จ/ใบกำกับภาษี)", "รหัสไปรษณีย์ (ใบเสร็จ/ใบกำกับภาษี)", "ส่วนลดประวัติดี" /* 60 */, "ส่วนลดงาน Fleet", "เบอร์ติดต่อ", "เลขที่บัตรประชาชน", "วันเดือนปีเกิด", "อาชีพ", "สถานภาพ", "เลขประจำตัวผู้เสียภาษีอากร", "คำนำหน้าชื่อ 1", "ชื่อกรรมการ 1", "นามสกุลกรรมการ 1" /* 70 */, "เลขที่บัตรประชาชนกรรมการ 1", "คำนำหน้าชื่อ 2", "ชื่อกรรมการ 2", "นามสกุลกรรมการ 2", "เลขที่บัตรประชาชนกรรมการ 2", "คำนำหน้าชื่อ 3", "ชื่อกรรมการ 3", "นามสกุลกรรมการ 3", "เลขที่บัตรประชาชนกรรมการ 3", "จัดส่งเอกสารที่สาขา" /* 80 */, "ชื่อผู้รับเอกสาร", "ผู้รับผลประโยชน์" };
                List<string> typLst = new List<string>() { "c", "d", "c", "c", "c", "c", "c", "c", "c", "c" /* 10 */, "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" /* 20 */, "c", "c", "c", "c", "c", "c", "d", "d", "c", "c" /* 30 */, "c", "c", "c", "c", "c", "c", "c", "c", "c", "n" /* 40 */, "n", "n", "n", "n", "c", "c", "c", "c", "c", "c", "c" /*50*/, "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" /* 60 */, "c", "c", "c", "d", "c", "c", "c", "c", "c", "c" /* 70 */, "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" /* 80 */, "c", "c" };

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    using (SLM_DBEntities sdc = new SLM_DBEntities())
                    {

                        var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                        if (batch != null)
                        {
                            batch.slm_StartTime = starttime;
                            sdc.SaveChanges();
                        }

                        int ttr = 0;
                        int ttf = 0;
                        foreach (var com in odc.kkslm_ms_ins_com)
                        {
                            string subfolder = folder + "\\" + com.slm_InsABB;
                            if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

                            string filename = subfolder + "\\" + "แจ้งต่อประกัน_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
                            var data = (from r in sdc.kkslm_tr_notify_renewinsurance_report
                                       join t in sdc.kkslm_ms_title on r.slm_Title_Id equals t.slm_TitleId into r1
                                       from t in r1.DefaultIfEmpty()
                                       join s in sdc.kkslm_ms_staff on r.slm_Owner equals s.slm_UserName into r2
                                       from s in r2.DefaultIfEmpty()
                                       join cr in sdc.kkslm_ms_coveragetype on r.slm_CoverageTypeId equals cr.slm_CoverageTypeId into r3
                                       from cr in r3.DefaultIfEmpty()
                                       join tm in sdc.kkslm_ms_tambol on r.slm_TambolId equals tm.slm_TambolId into r4
                                       from tm in r4.DefaultIfEmpty()
                                       join am in sdc.kkslm_ms_amphur on r.slm_AmphurId equals am.slm_AmphurId into r5
                                       from am in r5.DefaultIfEmpty()
                                       join pv in sdc.kkslm_ms_province on r.slm_ProvinceId equals pv.slm_ProvinceId into r6
                                       from pv in r6.DefaultIfEmpty()
                                       join t1 in sdc.kkslm_ms_title on r.slm_Title_Id_Committee1 equals t1.slm_TitleId into r7
                                       from t1 in r7.DefaultIfEmpty()
                                       join t2 in sdc.kkslm_ms_title on r.slm_Title_Id_Committee2 equals t2.slm_TitleId into r8
                                       from t2 in r8.DefaultIfEmpty()
                                       join t3 in sdc.kkslm_ms_title on r.slm_Title_Id_Committee3 equals t3.slm_TitleId into r9
                                       from t3 in r9.DefaultIfEmpty()
                                       join rtm in sdc.kkslm_ms_tambol on r.slm_TambolId_Receipt equals rtm.slm_TambolId into r10
                                       from rtm in r10.DefaultIfEmpty()
                                       join ram in sdc.kkslm_ms_amphur on r.slm_AmphurId_Receipt equals ram.slm_AmphurId into r11
                                       from ram in r11.DefaultIfEmpty()
                                       join rpv in sdc.kkslm_ms_province on r.slm_ProvinceId_Receipt equals rpv.slm_ProvinceId into r12
                                       from rpv in r12.DefaultIfEmpty()
                                       join br in sdc.kkslm_ms_branch on r.slm_BranchCode equals br.slm_BranchCode into r13
                                       from br in r13.DefaultIfEmpty()
                                       join cd in sdc.kkslm_ms_cardtype on r.slm_CardTypeId equals cd.slm_CardTypeId into r14
                                       from cd in r14.DefaultIfEmpty()
                                           //join cp in sdc.kkslm_ms_campaign on r.slm_CampaignId equals  cp.slm_CampaignId into r15 from cp in r15.DefaultIfEmpty()
                                           //join scp in sdc.kkslm_ms_campaign on r.slm_SubCampaignId equals scp.slm_CampaignId into r16 from scp in r16.DefaultIfEmpty()
                                       join bd in sdc.kkslm_ms_redbook_brand on r.slm_BrandCode equals bd.slm_BrandCode into r17
                                       from bd in r17.DefaultIfEmpty()
                                       join md in sdc.kkslm_ms_redbook_model on new { BC = r.slm_BrandCode, MC = r.slm_ModelCode } equals new { BC = md.slm_BrandCode, MC = md.slm_ModelCode } into r19
                                       from md in r19.DefaultIfEmpty()
                                       join rp in sdc.kkslm_ms_repairtype on r.slm_RepairTypeId equals rp.slm_RepairTypeId into r20
                                       from rp in r20.DefaultIfEmpty()
                                       join mdy in sdc.kkslm_ms_redbook_model_year on (decimal)r.slm_ModelYearId equals mdy.slm_ModelYearId into r21
                                       from mdy in r21.DefaultIfEmpty()
                                       join ins in sdc.kkslm_ms_insurancecartype on r.slm_InsuranceCarTypeId equals ins.slm_InsurancecarTypeId into r22
                                       from ins in r22.DefaultIfEmpty()
                                       join rt in sdc.kkslm_ms_title on r.slm_Title_Id_Receipt equals rt.slm_TitleId into r23
                                       from rt in r23.DefaultIfEmpty()
                                       join oc in sdc.kkslm_ms_occupation on r.slm_Occupation equals oc.slm_OccupationId into r24
                                       from oc in r24.DefaultIfEmpty()
                                           //Added By Pom 08/08/2016
                                       join sm in sdc.kkslm_ms_redbook_submodel on r.slm_KKKey equals sm.slm_KKKey into sm1
                                       from sm in sm1.DefaultIfEmpty()
                                       join renew in sdc.kkslm_tr_renewinsurance on r.slm_TicketId equals renew.slm_TicketId into r26
                                       from renew in r26.DefaultIfEmpty()
                                       join brndoc in sdc.kkslm_ms_branch on renew.slm_SendDocBrandCode equals brndoc.slm_BranchCode into r27
                                       from brndoc in r27.DefaultIfEmpty()
                                       join benefit in sdc.kkslm_ms_beneficiary on renew.slm_BeneficiaryId equals benefit.slm_Id into r28
                                       from benefit in r28.DefaultIfEmpty()

                                       orderby r.slm_InsReceiveNo
                                       where (r.slm_Export_Flag == false || r.slm_Export_Flag == null) && r.slm_Ins_Com_Id == com.slm_Ins_Com_Id
                                       select new
                                       {
                                           r.slm_Id,
                                           r.slm_InsPayDate,
                                           com.slm_InsNameTh,
                                           r.slm_Contract_Number,
                                           r.slm_PolicyNoOld,
                                           r.slm_BranchCode,
                                           br.slm_BranchName,
                                           r.slm_InsReceiveNo,
                                           r.slm_CampaignName,
                                           r.slm_SubCampaignName,
                                           cd.slm_CardTypeName,
                                           t.slm_TitleName,
                                           r.slm_Name,
                                           r.slm_LastName,
                                           r.slm_House_No,
                                           r.slm_Moo,
                                           r.slm_Village,
                                           r.slm_Building,
                                           r.slm_Soi,
                                           r.slm_Street,
                                           tm.slm_TambolNameTH,
                                           am.slm_AmphurNameTH,
                                           pv.slm_ProvinceNameTH,
                                           r.slm_Zipcode,
                                           cr.slm_ConverageTypeName,
                                           rp.slm_RepairTypeName,
                                           r.slm_CoverageStartDate,
                                           r.slm_CoverageEndDate,
                                           r.slm_VehicleNo,
                                           ins.slm_InsurancecarTypeName,
                                           bd.slm_BrandName,
                                           md.slm_ModelName,
                                           r.slm_Cartype,
                                           r.slm_CarLicenseNo,
                                           r.slm_VIN,
                                           r.slm_EngineNo,
                                           r.slm_ModelYearId,
                                           r.slm_Cc,
                                           r.slm_WeightPerTon,
                                           r.slm_SumInsured_Year1,
                                           r.slm_NetPremium_Year1,
                                           r.slm_SumInsured_Year2,
                                           r.slm_SumInsured_Year2_FT,
                                           r.slm_NetPremium_Year2,
                                           r.slm_InsReceiveDate,
                                           s.slm_StaffNameTH,
                                           r.slm_Remark,
                                           r.slm_Birthdate_Driver1,
                                           r.slm_Birthdate_Driver2,
                                           slm_TitleName_Receipt = rt.slm_TitleName,
                                           r.slm_Name_Receipt,
                                           r.slm_House_No_Receipt,
                                           r.slm_Village_Receipt,
                                           r.slm_Building_Receipt,
                                           r.slm_Soi_Receipt,
                                           r.slm_Street_Receipt,
                                           TambolName_Receipt = rtm.slm_TambolNameTH,
                                           AmphurName_Receipt = ram.slm_AmphurNameTH,
                                           ProvinceName_Receipt = rpv.slm_ProvinceNameTH,
                                           r.slm_Zipcode_Receipt,
                                           r.slm_Discouont_Good_History,
                                           r.slm_Discount_Fleet,
                                           r.slm_TelNo_1,
                                           r.slm_CitizenId,
                                           r.slm_Birthdate,
                                           oc.slm_OccupationNameTH,
                                           r.slm_MaritalStatus,
                                           r.slm_TaxNo,
                                           Committee1_Title = t1.slm_TitleName,
                                           r.slm_Name_Committee1,
                                           r.slm_LastName_Committee1,
                                           r.slm_CitizenId_Committe1,
                                           Committee2_Title = t2.slm_TitleName,
                                           r.slm_Name_Committee2,
                                           r.slm_LastName_Committee2,
                                           r.slm_CitizenId_Committe2,
                                           Committee3_Title = t3.slm_TitleName,
                                           r.slm_Name_Committee3,
                                           r.slm_LastName_Committee3,
                                           r.slm_CitizenId_Committe3,
                                           SubModelDesc = sm.slm_Description,           //Added By Pom 08/08/2016
                                           SendDocBranchName = brndoc.slm_BranchName,
                                           SendDocReceiverName = renew.slm_Receiver,
                                           BeneficiaryName = benefit.slm_Name
                                       }).ToList();

                            List<object[]> dataLst = new List<object[]>();
                            int i = 1;
                            var maritalStatusList = sdc.kkslm_ms_marital_status.ToList();
                            foreach (var itm in data)
                            {
                                var maritalStatus = maritalStatusList.Where(p => p.slm_MaritalStatusId.ToString() == itm.slm_MaritalStatus).Select(p => p.slm_MaritalStatusName).FirstOrDefault();
                                var obj = new object[]
                                {
                                     itm.slm_InsReceiveDate == null ? "" : (itm.slm_InsReceiveDate.Value.ToString("d/MM/") + itm.slm_InsReceiveDate.Value.Year.ToString() + " " + itm.slm_InsReceiveDate.Value.ToString("HH:mm:ss")),
                                     itm.slm_InsPayDate == null ? null : itm.slm_InsPayDate.Value.ToString("d/MM/yyyy"),
                                     com.slm_InsNameTh,
                                     itm.slm_Contract_Number,
                                     itm.slm_PolicyNoOld,
                                     itm.slm_BranchCode,
                                     itm.slm_BranchName,
                                     itm.slm_InsReceiveNo,
                                     itm.slm_CampaignName,
                                     itm.slm_SubCampaignName, // 10
                                     itm.slm_CardTypeName,
                                     itm.slm_TitleName,
                                     itm.slm_Name,
                                     itm.slm_LastName,
                                     itm.slm_House_No,
                                     itm.slm_Moo,
                                     itm.slm_Village,
                                     itm.slm_Building,
                                     itm.slm_Soi,
                                     itm.slm_Street, // 20
                                     itm.slm_TambolNameTH,
                                     itm.slm_AmphurNameTH,
                                     itm.slm_ProvinceNameTH,
                                     itm.slm_Zipcode,
                                     itm.slm_ConverageTypeName,
                                     itm.slm_RepairTypeName,
                                     itm.slm_CoverageStartDate == null ? null : itm.slm_CoverageStartDate.Value.ToString("d/MM/yyyy"),
                                     itm.slm_CoverageEndDate == null ? null : itm.slm_CoverageEndDate.Value.ToString("d/MM/yyyy"),
                                     itm.slm_VehicleNo,
                                     itm.slm_InsurancecarTypeName, //30
                                     itm.slm_BrandName,
                                     itm.slm_ModelName + " " + itm.SubModelDesc,
                                     itm.slm_Cartype,
                                     itm.slm_CarLicenseNo,
                                     itm.slm_VIN,
                                     itm.slm_EngineNo,
                                     itm.slm_ModelYearId,
                                     itm.slm_Cc,
                                     itm.slm_WeightPerTon,
                                     itm.slm_SumInsured_Year1, // 40
                                     itm.slm_NetPremium_Year1,
                                     itm.slm_SumInsured_Year2,
                                     itm.slm_SumInsured_Year2_FT,
                                     itm.slm_NetPremium_Year2,
                                     itm.slm_InsReceiveDate== null ? null : itm.slm_InsReceiveDate.Value.ToString("HH:mm:ss"),
                                     itm.slm_StaffNameTH,
                                     itm.slm_Remark,
                                     itm.slm_Birthdate_Driver1,
                                     itm.slm_Birthdate_Driver2,
                                     itm.slm_TitleName_Receipt,
                                     itm.slm_Name_Receipt, //50
                                     itm.slm_House_No_Receipt,
                                     itm.slm_Village_Receipt,
                                     itm.slm_Building_Receipt,
                                     itm.slm_Soi_Receipt,
                                     itm.slm_Street_Receipt,
                                     itm.TambolName_Receipt,
                                     itm.AmphurName_Receipt,
                                     itm.ProvinceName_Receipt,
                                     itm.slm_Zipcode_Receipt,
                                     itm.slm_Discouont_Good_History, //60
                                     itm.slm_Discount_Fleet,
                                     itm.slm_TelNo_1,
                                     itm.slm_CitizenId,
                                     itm.slm_Birthdate == null? null : itm.slm_Birthdate.Value.ToString("d/MM/yyyy"),
                                     itm.slm_OccupationNameTH,
                                     maritalStatus,
                                     itm.slm_TaxNo,
                                     itm.Committee1_Title,
                                     itm.slm_Name_Committee1,
                                     itm.slm_LastName_Committee1, //70
                                     itm.slm_CitizenId_Committe1,
                                     itm.Committee2_Title,
                                     itm.slm_Name_Committee2,
                                     itm.slm_LastName_Committee2,
                                     itm.slm_CitizenId_Committe2,
                                     itm.Committee3_Title,
                                     itm.slm_Name_Committee3,
                                     itm.slm_LastName_Committee3,
                                     itm.slm_CitizenId_Committe3,
                                     itm.SendDocBranchName,     //80
                                     itm.SendDocReceiverName,
                                     itm.BeneficiaryName
                                };
                                _log.DebugFormat("{0} ข้อมูลรายงานแจ้งต่อประกันจาก database : ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                _log.DebugFormat("InsReceiveDate : {0}", itm.slm_InsReceiveDate == null ? "" : (itm.slm_InsReceiveDate.Value.ToString("d/MM/") + itm.slm_InsReceiveDate.Value.Year.ToString() + " " + itm.slm_InsReceiveDate.Value.ToString("HH:mm:ss")));
                                _log.DebugFormat("InsPayDate : {0}", itm.slm_InsPayDate == null ? null : itm.slm_InsPayDate.Value.ToString("d/MM/yyyy"));
                                _log.DebugFormat("InsNameTh : {0}", com.slm_InsNameTh);
                                _log.DebugFormat("Contract_Number : {0}", itm.slm_Contract_Number);
                                _log.DebugFormat("PolicyNoOld : {0}", itm.slm_PolicyNoOld);
                                _log.DebugFormat("BranchCode : {0}", itm.slm_BranchCode);
                                _log.DebugFormat("BranchName : {0}", itm.slm_BranchName);
                                _log.DebugFormat("InsReceiveNo : {0}", itm.slm_InsReceiveNo);
                                _log.DebugFormat("SubCampaignName : {0}", itm.slm_SubCampaignName);
                                _log.DebugFormat("CardTypeName : {0}", itm.slm_CardTypeName);
                                _log.DebugFormat("TitleName : {0}", itm.slm_TitleName);
                                _log.DebugFormat("Name : {0}", itm.slm_Name);
                                _log.DebugFormat("LastName : {0}", itm.slm_LastName);
                                _log.DebugFormat("House_No : {0}", itm.slm_House_No);
                                _log.DebugFormat("Moo : {0}", itm.slm_Moo);
                                _log.DebugFormat("Village : {0}", itm.slm_Village);
                                _log.DebugFormat("Building : {0}", itm.slm_Building);
                                _log.DebugFormat("Soi : {0}", itm.slm_Soi);
                                _log.DebugFormat("Street : {0}", itm.slm_Street);
                                _log.DebugFormat("TambolNameTH : {0}", itm.slm_TambolNameTH);
                                _log.DebugFormat("AmphurNameTH : {0}", itm.slm_AmphurNameTH);
                                _log.DebugFormat("ProvinceNameTH : {0}", itm.slm_ProvinceNameTH);
                                _log.DebugFormat("Zipcode : {0}", itm.slm_Zipcode);
                                _log.DebugFormat("ConverageTypeName : {0}", itm.slm_ConverageTypeName);
                                _log.DebugFormat("RepairTypeName : {0}", itm.slm_RepairTypeName);
                                _log.DebugFormat("CoverageStartDate : {0}", itm.slm_CoverageStartDate == null ? null : itm.slm_CoverageStartDate.Value.ToString("d/MM/yyyy"));
                                _log.DebugFormat("CoverageEndDate : {0}", itm.slm_CoverageEndDate == null ? null : itm.slm_CoverageEndDate.Value.ToString("d/MM/yyyy"));
                                _log.DebugFormat("VehicleNo : {0}", itm.slm_VehicleNo);
                                _log.DebugFormat("InsurancecarTypeName : {0}", itm.slm_InsurancecarTypeName);
                                _log.DebugFormat("BrandName : {0}", itm.slm_BrandName);
                                _log.DebugFormat("ModelName : {0}", itm.slm_ModelName + " " + itm.SubModelDesc);
                                _log.DebugFormat("Cartype : {0}", itm.slm_Cartype);
                                _log.DebugFormat("CarLicenseNo : {0}", itm.slm_CarLicenseNo);
                                _log.DebugFormat("VIN : {0}", itm.slm_VIN);
                                _log.DebugFormat("EngineNo : {0}", itm.slm_EngineNo);
                                _log.DebugFormat("ModelYearId : {0}", itm.slm_ModelYearId);
                                _log.DebugFormat("Cc : {0}", itm.slm_Cc);
                                _log.DebugFormat("WeightPerTon : {0}", itm.slm_WeightPerTon);
                                _log.DebugFormat("SumInsured_Year1 : {0}", itm.slm_SumInsured_Year1);
                                _log.DebugFormat("NetPremium_Year1 : {0}", itm.slm_NetPremium_Year1);
                                _log.DebugFormat("SumInsured_Year2 : {0}", itm.slm_SumInsured_Year2);
                                _log.DebugFormat("SumInsured_Year2_FT : {0}", itm.slm_SumInsured_Year2_FT);
                                _log.DebugFormat("NetPremium_Year2 : {0}", itm.slm_NetPremium_Year2);
                                _log.DebugFormat("InsReceiveDate : {0}", itm.slm_InsReceiveDate == null ? null : itm.slm_InsReceiveDate.Value.ToString("HH:mm:ss"));
                                _log.DebugFormat("StaffNameTH : {0}", itm.slm_StaffNameTH);
                                _log.DebugFormat("Remark : {0}", itm.slm_Remark);
                                _log.DebugFormat("Birthdate_Driver1 : {0}", itm.slm_Birthdate_Driver1);
                                _log.DebugFormat("Birthdate_Driver2 : {0}", itm.slm_Birthdate_Driver2);
                                _log.DebugFormat("TitleName_Receipt : {0}", itm.slm_TitleName_Receipt);
                                _log.DebugFormat("Name_Receipt : {0}", itm.slm_Name_Receipt);
                                _log.DebugFormat("House_No_Receipt : {0}", itm.slm_House_No_Receipt);
                                _log.DebugFormat("Village_Receipt : {0}", itm.slm_Village_Receipt);
                                _log.DebugFormat("Building_Receipt : {0}", itm.slm_Building_Receipt);
                                _log.DebugFormat("Soi_Receipt : {0}", itm.slm_Soi_Receipt);
                                _log.DebugFormat("Street_Receipt : {0}", itm.slm_Street_Receipt);
                                _log.DebugFormat("TambolName_Receipt : {0}", itm.TambolName_Receipt);
                                _log.DebugFormat("AmphurName_Receipt : {0}", itm.AmphurName_Receipt);
                                _log.DebugFormat("ProvinceName_Receipt : {0}", itm.ProvinceName_Receipt);
                                _log.DebugFormat("Zipcode_Receipt : {0}", itm.slm_Zipcode_Receipt);
                                _log.DebugFormat("Discouont_Good_History : {0}", itm.slm_Discouont_Good_History);
                                _log.DebugFormat("Discount_Fleet : {0}", itm.slm_Discount_Fleet);
                                _log.DebugFormat("TelNo_1 : {0}", itm.slm_TelNo_1);
                                _log.DebugFormat("CitizenId : {0}", itm.slm_CitizenId);
                                _log.DebugFormat("Birthdate : {0}", itm.slm_Birthdate == null ? null : itm.slm_Birthdate.Value.ToString("d/MM/yyyy"));
                                _log.DebugFormat("OccupationNameTH : {0}", itm.slm_OccupationNameTH);
                                _log.DebugFormat("maritalStatus : {0}", maritalStatus);
                                _log.DebugFormat("TaxNo : {0}", itm.slm_TaxNo);
                                _log.DebugFormat("Committee1_Title : {0}", itm.Committee1_Title);
                                _log.DebugFormat("Name_Committee1 : {0}", itm.slm_Name_Committee1);
                                _log.DebugFormat("LastName_Committee1 : {0}", itm.slm_LastName_Committee1);
                                _log.DebugFormat("CitizenId_Committe1 : {0}", itm.slm_CitizenId_Committe1);
                                _log.DebugFormat("Committee2_Title : {0}", itm.Committee2_Title);
                                _log.DebugFormat("Name_Committee2 : {0}", itm.slm_Name_Committee2);
                                _log.DebugFormat("LastName_Committee2 : {0}", itm.slm_LastName_Committee2);
                                _log.DebugFormat("CitizenId_Committe2 : {0}", itm.slm_CitizenId_Committe2);
                                _log.DebugFormat("Committee3_Title : {0}", itm.Committee3_Title);
                                _log.DebugFormat("Name_Committee3 : {0}", itm.slm_Name_Committee3);
                                _log.DebugFormat("LastName_Committee3 : {0}", itm.slm_LastName_Committee3);
                                _log.DebugFormat("CitizenId_Committe3 : {0}", itm.slm_CitizenId_Committe3);
                                _log.Debug("===============================================");

                                dataLst.Add(obj);
                                i++;
                                succ++;

                                total++;
                            }

                            sdc.SaveChanges();

                            if (dataLst.Count > 0) ttf++;
                            ttr += dataLst.Count;
                            if (!writeExcel(filename, strLst, typLst, dataLst, rowperpage))
                            {
                                fail = data.Count();
                                errLst.Add(new kkslm_tr_batchlog() { slm_WorkDetailId = -1, slm_ErrorDetail = _error });
                            }
                            else
                            {
                                //if (com.slm_Ins_Com_Id == 3)
                                //{
                                //    var a = data.Count();
                                //}
                                foreach (var itm in data)
                                {
                                    // update flag
                                    var rpt = sdc.kkslm_tr_notify_renewinsurance_report.Where(r => r.slm_Id == itm.slm_Id).FirstOrDefault();
                                    if (rpt != null)
                                    {
                                        rpt.slm_Export_Flag = true;
                                        rpt.slm_Export_Date = DateTime.Now;
                                        rpt.slm_UpdatedBy = AppName;
                                        rpt.slm_UpdatedDate = DateTime.Now;
                                    }
                                }
                            }
                        }

                        if (ttf == 0) status = "No data to generate";
                        else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
                        DateTime endtime = DateTime.Now;

                        // add errorlog
                        foreach (var itm in errLst)
                            sdc.kkslm_tr_batchlog.AddObject(itm);

                        // update batch
                        batch.slm_EndTime = endtime;
                        batch.slm_Status = (fail > 0 ? "2" : "1");


                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = starttime;
                        bm.slm_DateEnd = endtime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = AppName;
                        bm.slm_BatchDate = dt;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;

        }

        //public bool GenReport002(DateTime dt, out string status, int rowperpage, string reportPath)
        //{
        //    ILog _log = LogManager.GetLogger(typeof(OBTBatchBiz));//OBT_PRO_16
        //    status = "";
        //    string batchCode = "OBT_PRO_14";
        //    string folder = "";

        //    if (!string.IsNullOrEmpty(reportPath))
        //        folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
        //    else
        //        folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

        //    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        //    int succ = 0;
        //    int fail = 0;
        //    int total = 0;
        //    List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

        //    DateTime starttime = DateTime.Now;
        //    DateTime curdate = DateTime.Now.Date;
        //    DateTime tomorow = curdate.AddDays(1);

        //    bool ret = true;
        //    try
        //    {
        //        // get headers                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  //not count
        //        List<string> strLst = new List<string>() { "วันที่รับแจ้ง", "วันที่รับเงินค่าเบี้ยประกัน", "รายชื่อบริษัทประกันภัย", "เลขที่สัญญาเช่าซื้อ", "เลขที่กรมธรรม์เดิม", "รหัสสาขา", "สาขา KK", "เลขรับแจ้ง", "Campaign", "Sub Campaign" /* 10 */, "ประเภทบุคคล", "คำนำหน้าชื่อ", "ชื่อผู้เอาประกันภัย", "นามสกุลผู้เอาประกันภัย", "บ้านเลขที่", "หมู่", "หมู่บ้าน", "อาคาร", "ซอย", "ถนน" /* 20 */, "ตำบล/แขวง", "อำเภอ/เขต", "จังหวัด", "รหัสไปรษณีย์", "ประเภทความคุ้มครอง", "ประเภทการซ่อม", "วันเริ่มคุ้มครอง", "วันสิ้นสุดคุ้มครอง", "รหัสรถ", "ประเภทประกันภัยรถยนต์" /* 30 */, "ชื่อยี่ห้อรถ", "รุ่นรถ", "New/Used", "เลขทะเบียน", "เลขตัวถัง", "เลขเครื่องยนต์", "ปีรถยนต์", "ซีซี", "น้ำหนัก(ตัน)", "ทุนประกันปี 1" /* 40 */, "เบี้ยรวมภาษีและอากรปี 1", "ทุนประกันความเสียหายต่อตัวรถปี2(OD)", "ทุนประกันรถยนต์สูญหาย/ไฟไหม้ ปี2(F&T)", "เบี้ยรวมภาษีและอากรปี 2", "เวลารับแจ้ง", "ชื่อเจ้าหน้าที่ MKT", "หมายเหตุ", "ผู้ขับขี่ที่ 1 และวันเกิด", "ผู้ขับขี่ที่ 2 และวันเกิด", "คำนำหน้าชื่อ (ใบเสร็จ/ใบกำกับภาษี)", "ชื่อ (ใบเสร็จ/ใบกำกับภาษี)" /* 50 */, "บ้านเลขที่(ใบเสร็จ/ใบกำกับภาษี)", "หมู่บ้าน (ใบเสร็จ/ใบกำกับภาษี)", "อาคาร (ใบเสร็จ/ใบกำกับภาษี)", "ซอย (ใบเสร็จ/ใบกำกับภาษี)", "ถนน (ใบเสร็จ/ใบกำกับภาษี)", "ตำบล/แขวง (ใบเสร็จ/ใบกำกับภาษี)", "อำเภอ/เขต (ใบเสร็จ/ใบกำกับภาษี)", "จังหวัด (ใบเสร็จ/ใบกำกับภาษี)", "รหัสไปรษณีย์ (ใบเสร็จ/ใบกำกับภาษี)", "ส่วนลดประวัติดี" /* 60 */, "ส่วนลดงาน Fleet", "เบอร์ติดต่อ", "เลขที่บัตรประชาชน", "วันเดือนปีเกิด", "อาชีพ", "สถานภาพ", "เลขประจำตัวผู้เสียภาษีอากร", "คำนำหน้าชื่อ 1", "ชื่อกรรมการ 1", "นามสกุลกรรมการ 1" /* 70 */, "เลขที่บัตรประชาชนกรรมการ 1", "คำนำหน้าชื่อ 2", "ชื่อกรรมการ 2", "นามสกุลกรรมการ 2", "เลขที่บัตรประชาชนกรรมการ 2", "คำนำหน้าชื่อ 3", "ชื่อกรรมการ 3", "นามสกุลกรรมการ 3", "เลขที่บัตรประชาชนกรรมการ 3", "จัดส่งเอกสารที่สาขา" /* 80 */, "ชื่อผู้รับเอกสาร", "ผู้รับผลประโยชน์" };
        //        List<string> typLst = new List<string>() { "c", "d", "c", "c", "c", "c", "c", "c", "c", "c" /* 10 */, "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" /* 20 */, "c", "c", "c", "c", "c", "c", "d", "d", "c", "c" /* 30 */, "c", "c", "c", "c", "c", "c", "c", "c", "c", "n" /* 40 */, "n", "n", "n", "n", "c", "c", "c", "c", "c", "c", "c" /*50*/, "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" /* 60 */, "c", "c", "c", "d", "c", "c", "c", "c", "c", "c" /* 70 */, "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" /* 80 */, "c", "c" };

        //        using (OPERDBEntities odc = new OPERDBEntities())
        //        {
        //            using (SLM_DBEntities sdc = new SLM_DBEntities())
        //            {

        //                var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
        //                if (batch != null)
        //                {
        //                    batch.slm_StartTime = starttime;
        //                    sdc.SaveChanges();
        //                }

        //                int ttr = 0;
        //                int ttf = 0;
        //                foreach (var com in odc.kkslm_ms_ins_com)
        //                {
        //                    string subfolder = folder + "\\" + com.slm_InsABB;
        //                    if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

        //                    string filename = subfolder + "\\" + "แจ้งต่อประกัน_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
        //                    var data = from r in sdc.kkslm_tr_notify_renewinsurance_report
        //                               join t in sdc.kkslm_ms_title on r.slm_Title_Id equals t.slm_TitleId into r1
        //                               from t in r1.DefaultIfEmpty()
        //                               join s in sdc.kkslm_ms_staff on r.slm_Owner equals s.slm_UserName into r2
        //                               from s in r2.DefaultIfEmpty()
        //                               join cr in sdc.kkslm_ms_coveragetype on r.slm_CoverageTypeId equals cr.slm_CoverageTypeId into r3
        //                               from cr in r3.DefaultIfEmpty()
        //                               join tm in sdc.kkslm_ms_tambol on r.slm_TambolId equals tm.slm_TambolId into r4
        //                               from tm in r4.DefaultIfEmpty()
        //                               join am in sdc.kkslm_ms_amphur on r.slm_AmphurId equals am.slm_AmphurId into r5
        //                               from am in r5.DefaultIfEmpty()
        //                               join pv in sdc.kkslm_ms_province on r.slm_ProvinceId equals pv.slm_ProvinceId into r6
        //                               from pv in r6.DefaultIfEmpty()
        //                               join t1 in sdc.kkslm_ms_title on r.slm_Title_Id_Committee1 equals t1.slm_TitleId into r7
        //                               from t1 in r7.DefaultIfEmpty()
        //                               join t2 in sdc.kkslm_ms_title on r.slm_Title_Id_Committee2 equals t2.slm_TitleId into r8
        //                               from t2 in r8.DefaultIfEmpty()
        //                               join t3 in sdc.kkslm_ms_title on r.slm_Title_Id_Committee3 equals t3.slm_TitleId into r9
        //                               from t3 in r9.DefaultIfEmpty()
        //                               join rtm in sdc.kkslm_ms_tambol on r.slm_TambolId_Receipt equals rtm.slm_TambolId into r10
        //                               from rtm in r10.DefaultIfEmpty()
        //                               join ram in sdc.kkslm_ms_amphur on r.slm_AmphurId_Receipt equals ram.slm_AmphurId into r11
        //                               from ram in r11.DefaultIfEmpty()
        //                               join rpv in sdc.kkslm_ms_province on r.slm_ProvinceId_Receipt equals rpv.slm_ProvinceId into r12
        //                               from rpv in r12.DefaultIfEmpty()
        //                               join br in sdc.kkslm_ms_branch on r.slm_BranchCode equals br.slm_BranchCode into r13
        //                               from br in r13.DefaultIfEmpty()
        //                               join cd in sdc.kkslm_ms_cardtype on r.slm_CardTypeId equals cd.slm_CardTypeId into r14
        //                               from cd in r14.DefaultIfEmpty()
        //                                   //join cp in sdc.kkslm_ms_campaign on r.slm_CampaignId equals  cp.slm_CampaignId into r15 from cp in r15.DefaultIfEmpty()
        //                                   //join scp in sdc.kkslm_ms_campaign on r.slm_SubCampaignId equals scp.slm_CampaignId into r16 from scp in r16.DefaultIfEmpty()
        //                               join bd in sdc.kkslm_ms_redbook_brand on r.slm_BrandCode equals bd.slm_BrandCode into r17
        //                               from bd in r17.DefaultIfEmpty()
        //                               join md in sdc.kkslm_ms_redbook_model on new { BC = r.slm_BrandCode, MC = r.slm_ModelCode } equals new { BC = md.slm_BrandCode, MC = md.slm_ModelCode } into r19
        //                               from md in r19.DefaultIfEmpty()
        //                               join rp in sdc.kkslm_ms_repairtype on r.slm_RepairTypeId equals rp.slm_RepairTypeId into r20
        //                               from rp in r20.DefaultIfEmpty()
        //                               join mdy in sdc.kkslm_ms_redbook_model_year on (decimal)r.slm_ModelYearId equals mdy.slm_ModelYearId into r21
        //                               from mdy in r21.DefaultIfEmpty()
        //                               join ins in sdc.kkslm_ms_insurancecartype on r.slm_InsuranceCarTypeId equals ins.slm_InsurancecarTypeId into r22
        //                               from ins in r22.DefaultIfEmpty()
        //                               join rt in sdc.kkslm_ms_title on r.slm_Title_Id_Receipt equals rt.slm_TitleId into r23
        //                               from rt in r23.DefaultIfEmpty()
        //                               join oc in sdc.kkslm_ms_occupation on r.slm_Occupation equals oc.slm_OccupationId into r24
        //                               from oc in r24.DefaultIfEmpty()
        //                                   //Added By Pom 08/08/2016
        //                               join sm in sdc.kkslm_ms_redbook_submodel on r.slm_KKKey equals sm.slm_KKKey into sm1
        //                               from sm in sm1.DefaultIfEmpty()
        //                               join renew in sdc.kkslm_tr_renewinsurance on r.slm_TicketId equals renew.slm_TicketId into r26
        //                               from renew in r26.DefaultIfEmpty()
        //                               join brndoc in sdc.kkslm_ms_branch on renew.slm_SendDocBrandCode equals brndoc.slm_BranchCode into r27
        //                               from brndoc in r27.DefaultIfEmpty()
        //                               join benefit in sdc.kkslm_ms_beneficiary on renew.slm_BeneficiaryId equals benefit.slm_Id into r28
        //                               from benefit in r28.DefaultIfEmpty()

        //                               orderby r.slm_InsReceiveNo
        //                               where (r.slm_Export_Flag == false || r.slm_Export_Flag == null) && r.slm_Ins_Com_Id == com.slm_Ins_Com_Id
        //                               select new
        //                               {
        //                                   r.slm_Id,
        //                                   r.slm_InsPayDate,
        //                                   com.slm_InsNameTh,
        //                                   r.slm_Contract_Number,
        //                                   r.slm_PolicyNoOld,
        //                                   r.slm_BranchCode,
        //                                   br.slm_BranchName,
        //                                   r.slm_InsReceiveNo,
        //                                   r.slm_CampaignName,
        //                                   r.slm_SubCampaignName,
        //                                   cd.slm_CardTypeName,
        //                                   t.slm_TitleName,
        //                                   r.slm_Name,
        //                                   r.slm_LastName,
        //                                   r.slm_House_No,
        //                                   r.slm_Moo,
        //                                   r.slm_Village,
        //                                   r.slm_Building,
        //                                   r.slm_Soi,
        //                                   r.slm_Street,
        //                                   tm.slm_TambolNameTH,
        //                                   am.slm_AmphurNameTH,
        //                                   pv.slm_ProvinceNameTH,
        //                                   r.slm_Zipcode,
        //                                   cr.slm_ConverageTypeName,
        //                                   rp.slm_RepairTypeName,
        //                                   r.slm_CoverageStartDate,
        //                                   r.slm_CoverageEndDate,
        //                                   r.slm_VehicleNo,
        //                                   ins.slm_InsurancecarTypeName,
        //                                   bd.slm_BrandName,
        //                                   md.slm_ModelName,
        //                                   r.slm_Cartype,
        //                                   r.slm_CarLicenseNo,
        //                                   r.slm_VIN,
        //                                   r.slm_EngineNo,
        //                                   r.slm_ModelYearId,
        //                                   r.slm_Cc,
        //                                   r.slm_WeightPerTon,
        //                                   r.slm_SumInsured_Year1,
        //                                   r.slm_NetPremium_Year1,
        //                                   r.slm_SumInsured_Year2,
        //                                   r.slm_SumInsured_Year2_FT,
        //                                   r.slm_NetPremium_Year2,
        //                                   r.slm_InsReceiveDate,
        //                                   s.slm_StaffNameTH,
        //                                   r.slm_Remark,
        //                                   r.slm_Birthdate_Driver1,
        //                                   r.slm_Birthdate_Driver2,
        //                                   slm_TitleName_Receipt = rt.slm_TitleName,
        //                                   r.slm_Name_Receipt,
        //                                   r.slm_House_No_Receipt,
        //                                   r.slm_Village_Receipt,
        //                                   r.slm_Building_Receipt,
        //                                   r.slm_Soi_Receipt,
        //                                   r.slm_Street_Receipt,
        //                                   TambolName_Receipt = rtm.slm_TambolNameTH,
        //                                   AmphurName_Receipt = ram.slm_AmphurNameTH,
        //                                   ProvinceName_Receipt = rpv.slm_ProvinceNameTH,
        //                                   r.slm_Zipcode_Receipt,
        //                                   r.slm_Discouont_Good_History,
        //                                   r.slm_Discount_Fleet,
        //                                   r.slm_TelNo_1,
        //                                   r.slm_CitizenId,
        //                                   r.slm_Birthdate,
        //                                   oc.slm_OccupationNameTH,
        //                                   r.slm_MaritalStatus,
        //                                   r.slm_TaxNo,
        //                                   Committee1_Title = t1.slm_TitleName,
        //                                   r.slm_Name_Committee1,
        //                                   r.slm_LastName_Committee1,
        //                                   r.slm_CitizenId_Committe1,
        //                                   Committee2_Title = t2.slm_TitleName,
        //                                   r.slm_Name_Committee2,
        //                                   r.slm_LastName_Committee2,
        //                                   r.slm_CitizenId_Committe2,
        //                                   Committee3_Title = t3.slm_TitleName,
        //                                   r.slm_Name_Committee3,
        //                                   r.slm_LastName_Committee3,
        //                                   r.slm_CitizenId_Committe3,
        //                                   SubModelDesc = sm.slm_Description,           //Added By Pom 08/08/2016
        //                                   SendDocBranchName = brndoc.slm_BranchName,
        //                                   SendDocReceiverName = renew.slm_Receiver,
        //                                   BeneficiaryName = benefit.slm_Name
        //                               };

        //                    List<object[]> dataLst = new List<object[]>();
        //                    int i = 1;
        //                    var maritalStatusList = sdc.kkslm_ms_marital_status.ToList();
        //                    foreach (var itm in data)
        //                    {
        //                        var maritalStatus = maritalStatusList.Where(p => p.slm_MaritalStatusId.ToString() == itm.slm_MaritalStatus).Select(p => p.slm_MaritalStatusName).FirstOrDefault();
        //                        var obj = new object[]
        //                        {
        //                             itm.slm_InsReceiveDate == null ? "" : (itm.slm_InsReceiveDate.Value.ToString("d/MM/") + itm.slm_InsReceiveDate.Value.Year.ToString() + " " + itm.slm_InsReceiveDate.Value.ToString("HH:mm:ss")),
        //                             itm.slm_InsPayDate == null ? null : itm.slm_InsPayDate.Value.ToString("d/MM/yyyy"),
        //                             com.slm_InsNameTh,
        //                             itm.slm_Contract_Number,
        //                             itm.slm_PolicyNoOld,
        //                             itm.slm_BranchCode,
        //                             itm.slm_BranchName,
        //                             itm.slm_InsReceiveNo,
        //                             itm.slm_CampaignName,
        //                             itm.slm_SubCampaignName, // 10
        //                             itm.slm_CardTypeName,
        //                             itm.slm_TitleName,
        //                             itm.slm_Name,
        //                             itm.slm_LastName,
        //                             itm.slm_House_No,
        //                             itm.slm_Moo,
        //                             itm.slm_Village,
        //                             itm.slm_Building,
        //                             itm.slm_Soi,
        //                             itm.slm_Street, // 20
        //                             itm.slm_TambolNameTH,
        //                             itm.slm_AmphurNameTH,
        //                             itm.slm_ProvinceNameTH,
        //                             itm.slm_Zipcode,
        //                             itm.slm_ConverageTypeName,
        //                             itm.slm_RepairTypeName,
        //                             itm.slm_CoverageStartDate == null ? null : itm.slm_CoverageStartDate.Value.ToString("d/MM/yyyy"),
        //                             itm.slm_CoverageEndDate == null ? null : itm.slm_CoverageEndDate.Value.ToString("d/MM/yyyy"),
        //                             itm.slm_VehicleNo,
        //                             itm.slm_InsurancecarTypeName, //30
        //                             itm.slm_BrandName,
        //                             itm.slm_ModelName + " " + itm.SubModelDesc,
        //                             itm.slm_Cartype,
        //                             itm.slm_CarLicenseNo,
        //                             itm.slm_VIN,
        //                             itm.slm_EngineNo,
        //                             itm.slm_ModelYearId,
        //                             itm.slm_Cc,
        //                             itm.slm_WeightPerTon,
        //                             itm.slm_SumInsured_Year1, // 40
        //                             itm.slm_NetPremium_Year1,
        //                             itm.slm_SumInsured_Year2,
        //                             itm.slm_SumInsured_Year2_FT,
        //                             itm.slm_NetPremium_Year2,
        //                             itm.slm_InsReceiveDate== null ? null : itm.slm_InsReceiveDate.Value.ToString("HH:mm:ss"),
        //                             itm.slm_StaffNameTH,
        //                             itm.slm_Remark,
        //                             itm.slm_Birthdate_Driver1,
        //                             itm.slm_Birthdate_Driver2,
        //                             itm.slm_TitleName_Receipt,
        //                             itm.slm_Name_Receipt, //50
        //                             itm.slm_House_No_Receipt,
        //                             itm.slm_Village_Receipt,
        //                             itm.slm_Building_Receipt,
        //                             itm.slm_Soi_Receipt,
        //                             itm.slm_Street_Receipt,
        //                             itm.TambolName_Receipt,
        //                             itm.AmphurName_Receipt,
        //                             itm.ProvinceName_Receipt,
        //                             itm.slm_Zipcode_Receipt,
        //                             itm.slm_Discouont_Good_History, //60
        //                             itm.slm_Discount_Fleet,
        //                             itm.slm_TelNo_1,
        //                             itm.slm_CitizenId,
        //                             itm.slm_Birthdate == null? null : itm.slm_Birthdate.Value.ToString("d/MM/yyyy"),
        //                             itm.slm_OccupationNameTH,
        //                             maritalStatus,
        //                             itm.slm_TaxNo,
        //                             itm.Committee1_Title,
        //                             itm.slm_Name_Committee1,
        //                             itm.slm_LastName_Committee1, //70
        //                             itm.slm_CitizenId_Committe1,
        //                             itm.Committee2_Title,
        //                             itm.slm_Name_Committee2,
        //                             itm.slm_LastName_Committee2,
        //                             itm.slm_CitizenId_Committe2,
        //                             itm.Committee3_Title,
        //                             itm.slm_Name_Committee3,
        //                             itm.slm_LastName_Committee3,
        //                             itm.slm_CitizenId_Committe3,
        //                             itm.SendDocBranchName,     //80
        //                             itm.SendDocReceiverName,
        //                             itm.BeneficiaryName
        //                        };
        //                        _log.DebugFormat("{0} ข้อมูลรายงานแจ้งต่อประกันจาก database : ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        //                        _log.DebugFormat("InsReceiveDate : {0}", itm.slm_InsReceiveDate == null ? "" : (itm.slm_InsReceiveDate.Value.ToString("d/MM/") + itm.slm_InsReceiveDate.Value.Year.ToString() + " " + itm.slm_InsReceiveDate.Value.ToString("HH:mm:ss")));
        //                        _log.DebugFormat("InsPayDate : {0}", itm.slm_InsPayDate == null ? null : itm.slm_InsPayDate.Value.ToString("d/MM/yyyy"));
        //                        _log.DebugFormat("InsNameTh : {0}", com.slm_InsNameTh);
        //                        _log.DebugFormat("Contract_Number : {0}", itm.slm_Contract_Number);
        //                        _log.DebugFormat("PolicyNoOld : {0}", itm.slm_PolicyNoOld);
        //                        _log.DebugFormat("BranchCode : {0}", itm.slm_BranchCode);
        //                        _log.DebugFormat("BranchName : {0}", itm.slm_BranchName);
        //                        _log.DebugFormat("InsReceiveNo : {0}", itm.slm_InsReceiveNo);
        //                        _log.DebugFormat("SubCampaignName : {0}", itm.slm_SubCampaignName);
        //                        _log.DebugFormat("CardTypeName : {0}", itm.slm_CardTypeName);
        //                        _log.DebugFormat("TitleName : {0}", itm.slm_TitleName);
        //                        _log.DebugFormat("Name : {0}", itm.slm_Name);
        //                        _log.DebugFormat("LastName : {0}", itm.slm_LastName);
        //                        _log.DebugFormat("House_No : {0}", itm.slm_House_No);
        //                        _log.DebugFormat("Moo : {0}", itm.slm_Moo);
        //                        _log.DebugFormat("Village : {0}", itm.slm_Village);
        //                        _log.DebugFormat("Building : {0}", itm.slm_Building);
        //                        _log.DebugFormat("Soi : {0}", itm.slm_Soi);
        //                        _log.DebugFormat("Street : {0}", itm.slm_Street);
        //                        _log.DebugFormat("TambolNameTH : {0}", itm.slm_TambolNameTH);
        //                        _log.DebugFormat("AmphurNameTH : {0}", itm.slm_AmphurNameTH);
        //                        _log.DebugFormat("ProvinceNameTH : {0}", itm.slm_ProvinceNameTH);
        //                        _log.DebugFormat("Zipcode : {0}", itm.slm_Zipcode);
        //                        _log.DebugFormat("ConverageTypeName : {0}", itm.slm_ConverageTypeName);
        //                        _log.DebugFormat("RepairTypeName : {0}", itm.slm_RepairTypeName);
        //                        _log.DebugFormat("CoverageStartDate : {0}", itm.slm_CoverageStartDate == null ? null : itm.slm_CoverageStartDate.Value.ToString("d/MM/yyyy"));
        //                        _log.DebugFormat("CoverageEndDate : {0}", itm.slm_CoverageEndDate == null ? null : itm.slm_CoverageEndDate.Value.ToString("d/MM/yyyy"));
        //                        _log.DebugFormat("VehicleNo : {0}", itm.slm_VehicleNo);
        //                        _log.DebugFormat("InsurancecarTypeName : {0}", itm.slm_InsurancecarTypeName);
        //                        _log.DebugFormat("BrandName : {0}", itm.slm_BrandName);
        //                        _log.DebugFormat("ModelName : {0}", itm.slm_ModelName + " " + itm.SubModelDesc);
        //                        _log.DebugFormat("Cartype : {0}", itm.slm_Cartype);
        //                        _log.DebugFormat("CarLicenseNo : {0}", itm.slm_CarLicenseNo);
        //                        _log.DebugFormat("VIN : {0}", itm.slm_VIN);
        //                        _log.DebugFormat("EngineNo : {0}", itm.slm_EngineNo);
        //                        _log.DebugFormat("ModelYearId : {0}", itm.slm_ModelYearId);
        //                        _log.DebugFormat("Cc : {0}", itm.slm_Cc);
        //                        _log.DebugFormat("WeightPerTon : {0}", itm.slm_WeightPerTon);
        //                        _log.DebugFormat("SumInsured_Year1 : {0}", itm.slm_SumInsured_Year1);
        //                        _log.DebugFormat("NetPremium_Year1 : {0}", itm.slm_NetPremium_Year1);
        //                        _log.DebugFormat("SumInsured_Year2 : {0}", itm.slm_SumInsured_Year2);
        //                        _log.DebugFormat("SumInsured_Year2_FT : {0}", itm.slm_SumInsured_Year2_FT);
        //                        _log.DebugFormat("NetPremium_Year2 : {0}", itm.slm_NetPremium_Year2);
        //                        _log.DebugFormat("InsReceiveDate : {0}", itm.slm_InsReceiveDate == null ? null : itm.slm_InsReceiveDate.Value.ToString("HH:mm:ss"));
        //                        _log.DebugFormat("StaffNameTH : {0}", itm.slm_StaffNameTH);
        //                        _log.DebugFormat("Remark : {0}", itm.slm_Remark);
        //                        _log.DebugFormat("Birthdate_Driver1 : {0}", itm.slm_Birthdate_Driver1);
        //                        _log.DebugFormat("Birthdate_Driver2 : {0}", itm.slm_Birthdate_Driver2);
        //                        _log.DebugFormat("TitleName_Receipt : {0}", itm.slm_TitleName_Receipt);
        //                        _log.DebugFormat("Name_Receipt : {0}", itm.slm_Name_Receipt);
        //                        _log.DebugFormat("House_No_Receipt : {0}", itm.slm_House_No_Receipt);
        //                        _log.DebugFormat("Village_Receipt : {0}", itm.slm_Village_Receipt);
        //                        _log.DebugFormat("Building_Receipt : {0}", itm.slm_Building_Receipt);
        //                        _log.DebugFormat("Soi_Receipt : {0}", itm.slm_Soi_Receipt);
        //                        _log.DebugFormat("Street_Receipt : {0}", itm.slm_Street_Receipt);
        //                        _log.DebugFormat("TambolName_Receipt : {0}", itm.TambolName_Receipt);
        //                        _log.DebugFormat("AmphurName_Receipt : {0}", itm.AmphurName_Receipt);
        //                        _log.DebugFormat("ProvinceName_Receipt : {0}", itm.ProvinceName_Receipt);
        //                        _log.DebugFormat("Zipcode_Receipt : {0}", itm.slm_Zipcode_Receipt);
        //                        _log.DebugFormat("Discouont_Good_History : {0}", itm.slm_Discouont_Good_History);
        //                        _log.DebugFormat("Discount_Fleet : {0}", itm.slm_Discount_Fleet);
        //                        _log.DebugFormat("TelNo_1 : {0}", itm.slm_TelNo_1);
        //                        _log.DebugFormat("CitizenId : {0}", itm.slm_CitizenId);
        //                        _log.DebugFormat("Birthdate : {0}", itm.slm_Birthdate == null ? null : itm.slm_Birthdate.Value.ToString("d/MM/yyyy"));
        //                        _log.DebugFormat("OccupationNameTH : {0}", itm.slm_OccupationNameTH);
        //                        _log.DebugFormat("maritalStatus : {0}", maritalStatus);
        //                        _log.DebugFormat("TaxNo : {0}", itm.slm_TaxNo);
        //                        _log.DebugFormat("Committee1_Title : {0}", itm.Committee1_Title);
        //                        _log.DebugFormat("Name_Committee1 : {0}", itm.slm_Name_Committee1);
        //                        _log.DebugFormat("LastName_Committee1 : {0}", itm.slm_LastName_Committee1);
        //                        _log.DebugFormat("CitizenId_Committe1 : {0}", itm.slm_CitizenId_Committe1);
        //                        _log.DebugFormat("Committee2_Title : {0}", itm.Committee2_Title);
        //                        _log.DebugFormat("Name_Committee2 : {0}", itm.slm_Name_Committee2);
        //                        _log.DebugFormat("LastName_Committee2 : {0}", itm.slm_LastName_Committee2);
        //                        _log.DebugFormat("CitizenId_Committe2 : {0}", itm.slm_CitizenId_Committe2);
        //                        _log.DebugFormat("Committee3_Title : {0}", itm.Committee3_Title);
        //                        _log.DebugFormat("Name_Committee3 : {0}", itm.slm_Name_Committee3);
        //                        _log.DebugFormat("LastName_Committee3 : {0}", itm.slm_LastName_Committee3);
        //                        _log.DebugFormat("CitizenId_Committe3 : {0}", itm.slm_CitizenId_Committe3);
        //                        _log.Debug("===============================================");

        //                        dataLst.Add(obj);
        //                        i++;
        //                        succ++;

        //                        total++;
        //                    }

        //                    //test
        //                    if (com.slm_Ins_Com_Id == 3)
        //                    {
        //                        var a = data.Count();
        //                    }
        //                    sdc.SaveChanges();
        //                    //test
        //                    if (com.slm_Ins_Com_Id == 3)
        //                    {
        //                        var a = data.Count();
        //                    }

        //                    if (dataLst.Count > 0) ttf++;
        //                    ttr += dataLst.Count;
        //                    if (!writeExcel(filename, strLst, typLst, dataLst, rowperpage))
        //                    {
        //                        fail = data.Count();
        //                        errLst.Add(new kkslm_tr_batchlog() { slm_WorkDetailId = -1, slm_ErrorDetail = _error });
        //                    }
        //                    else
        //                    {
        //                        foreach (var itm in data)
        //                        {
        //                            // update flag
        //                            var rpt = sdc.kkslm_tr_notify_renewinsurance_report.Where(r => r.slm_Id == itm.slm_Id).FirstOrDefault();
        //                            if (rpt != null)
        //                            {
        //                                rpt.slm_Export_Flag = true;
        //                                rpt.slm_Export_Date = DateTime.Now;
        //                                rpt.slm_UpdatedBy = AppName;
        //                                rpt.slm_UpdatedDate = DateTime.Now;
        //                            }
        //                        }
        //                    }
        //                }

        //                if (ttf == 0) status = "No data to generate";
        //                else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
        //                DateTime endtime = DateTime.Now;

        //                // add errorlog
        //                foreach (var itm in errLst)
        //                    sdc.kkslm_tr_batchlog.AddObject(itm);

        //                // update batch
        //                batch.slm_EndTime = endtime;
        //                batch.slm_Status = (fail > 0 ? "2" : "1");


        //                // insert batch monitor 
        //                kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
        //                bm.slm_DateStart = starttime;
        //                bm.slm_DateEnd = endtime;
        //                bm.slm_BatchCode = batchCode;
        //                bm.slm_Total = total;
        //                bm.slm_Success = succ;
        //                bm.slm_Fail = fail;
        //                bm.slm_ActionBy = AppName;
        //                bm.slm_BatchDate = dt;
        //                sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

        //                sdc.SaveChanges();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ret = false;
        //        _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
        //    }
        //    return ret;

        //}
        private bool TmpRp2(DateTime dt, out string status, int rowperpage)
        {
            status = "";
            var folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string filename = folder + "\\" + "ข้อมูลรายงานการแก้ไขใบเสร็จ_" + dt.Year.ToString() + dt.ToString("MMdd_HHmm") + ".xls";
            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            int succ = 0;
            int fail = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            bool ret = true;
            try
            {

                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == "OBT_PRO_13").FirstOrDefault();
                    if (batch != null)
                    {
                        batch.slm_StartTime = starttime;
                        sdc.SaveChanges();
                    }


                    // get headers
                    List<string> strLst = new List<string>() { "ลำดับที่", "เลขที่สัญญา", "ชื่อ-นามสกุลลูกค้า", "ที่อยู่จัดส่งเอกสาร", "ที่อยู่เปลี่ยนแปลง/ตามระบบ", "ประกัน", "พรบ", "ประเภทการจัดส่ง", "ชื่อ MKT (ชื่อ-นามสกุล)", "รหัสพนักงาน", "ชื่อทีม" };
                    List<string> typLst = new List<string>() { "n", "c", "c", "c", "c", "n", "n", "c", "c", "c", "c" };
                    // get data
                    var data = from r in sdc.kkslm_tr_notify_renewinsurance_report
                               join t in sdc.kkslm_ms_title on r.slm_Title_Id equals t.slm_TitleId into r1
                               from t in r1.DefaultIfEmpty()
                               join tm in sdc.kkslm_ms_tambol on r.slm_TambolId equals tm.slm_TambolId into r2
                               from tm in r2.DefaultIfEmpty()
                               join am in sdc.kkslm_ms_amphur on r.slm_AmphurId equals am.slm_AmphurId into r3
                               from am in r3.DefaultIfEmpty()
                               join pv in sdc.kkslm_ms_province on r.slm_ProvinceId equals pv.slm_ProvinceId into r4
                               from pv in r4.DefaultIfEmpty()
                               join st in sdc.kkslm_ms_staff on r.slm_Owner equals st.slm_UserName into r5
                               from st in r5.DefaultIfEmpty()
                               join tt in sdc.kkslm_ms_teamtelesales on (decimal)st.slm_TeamTelesales_Id equals tt.slm_TeamTelesales_Id into st1
                               from tt in st1.DefaultIfEmpty()
                               where r.slm_UpdatedDate >= curdate && r.slm_UpdatedDate < tomorow && (r.slm_Export_Flag == false || r.slm_Export_Flag == null)
                               orderby st.slm_StaffNameTH
                               select new
                               {
                                   r.slm_Id,
                                   r.slm_Contract_Number,
                                   t.slm_TitleName,
                                   r.slm_Name,
                                   r.slm_LastName,
                                   r.slm_House_No,
                                   r.slm_Moo
                               ,
                                   r.slm_Building,
                                   r.slm_Village,
                                   r.slm_Soi,
                                   r.slm_Street,
                                   tm.slm_TambolNameTH,
                                   am.slm_AmphurNameTH,
                                   pv.slm_ProvinceNameTH
                               ,
                                   r.slm_Zipcode,
                                   //r.slm_Address_Type,
                                   //r.slm_InsCost,
                                   //r.slm_ActCost,
                                   //r.slm_Send_Type,
                                   st.slm_StaffNameTH,
                                   st.slm_EmpCode,
                                   tt.slm_TeamTelesales_Name
                               };

                    List<object[]> dataLst = new List<object[]>();
                    int i = 1;
                    foreach (var itm in data)
                    {
                        var obj = new object[]
                        {
                            i,
                            itm.slm_Contract_Number,
                            itm.slm_TitleName + itm.slm_Name + " " + itm.slm_LastName,
                            itm.slm_House_No
                                + (String.IsNullOrEmpty(itm.slm_Moo) ? "" : " หมู่ " + itm.slm_Moo)
                                + (String.IsNullOrEmpty(itm.slm_Building) ? "" : " อาคาร " + itm.slm_Building)
                                + (String.IsNullOrEmpty(itm.slm_Village) ? "" : " หมู่บ้าน " + itm.slm_Village)
                                + (String.IsNullOrEmpty(itm.slm_Soi) ? "" : " ซอย " + itm.slm_Soi)
                                + (String.IsNullOrEmpty(itm.slm_Street) ? "" : " ถนน " + itm.slm_Street)
                                + (String.IsNullOrEmpty(itm.slm_TambolNameTH) ? "" : " ตำบล " + itm.slm_TambolNameTH)
                                + (String.IsNullOrEmpty(itm.slm_AmphurNameTH) ? "" : " อำเภอ " + itm.slm_AmphurNameTH)
                                + (String.IsNullOrEmpty(itm.slm_ProvinceNameTH) ? "" : " จังหวัด " + itm.slm_ProvinceNameTH)
                                + itm.slm_Zipcode,
                            "",//itm.slm_Address_Type == "001" ? "ที่อยู่เปลี่ยนแปลง" : itm.slm_Address_Type == "002" ? "ตามระบบ" : "",
                            "",//itm.slm_InsCost == null ? "0.00" : itm.slm_InsCost.Value.ToString("#,##0.00"),
                            "",//itm.slm_ActCost == null ? "0.00" : itm.slm_ActCost.Value.ToString("#,##0.00"),
                            "",//itm.slm_Send_Type == "001" ? "ลงทะเบียน" : itm.slm_Send_Type == "002" ? "ธรรมดา" : "",
                            itm.slm_StaffNameTH,
                            itm.slm_EmpCode,
                            itm.slm_TeamTelesales_Name
                        };
                        dataLst.Add(obj);
                        i++;
                        succ++;

                        // update flag
                        var rpt = sdc.kkslm_tr_notify_renewinsurance_report.Where(r => r.slm_Id == itm.slm_Id).FirstOrDefault();
                        if (rpt != null)
                        {
                            rpt.slm_Export_Flag = true;
                            rpt.slm_Export_Date = DateTime.Now;
                            rpt.slm_UpdatedBy = AppName;
                            rpt.slm_UpdatedDate = DateTime.Now;
                        }
                    }

                    if (dataLst.Count == 0) status = "No data to generate";
                    else status = string.Format("Generated {0} records", dataLst.Count);
                    writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                    DateTime endtime = DateTime.Now;



                    // add errorlog
                    foreach (var itm in errLst)
                        sdc.kkslm_tr_batchlog.AddObject(itm);

                    // update batch
                    batch.slm_EndTime = endtime;
                    batch.slm_Status = (fail > 0 ? "2" : "1");


                    // insert batch monitor 
                    kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                    bm.slm_DateStart = starttime;
                    bm.slm_DateEnd = endtime;
                    bm.slm_BatchCode = "OBT_PRO_13";
                    bm.slm_Total = data.Count();
                    bm.slm_Success = succ;
                    bm.slm_Fail = fail;
                    bm.slm_ActionBy = AppName;
                    bm.slm_BatchDate = dt;
                    sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                    sdc.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool GenReport003(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            status = "";
            string batchCode = "OBT_PRO_15";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            int succ = 0;
            int fail = 0;
            int total = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            bool ret = true;
            try
            {
                // get headers
                List<string> strLst = new List<string>() { "เลขรับแจ้ง", "รายชื่อบริษัทประกันภัย", "วันที่รับแจ้ง", "เลขที่สัญญาเช่า", "คำนำหน้าชื่อ", "ชื่อผู้เอาประกัน", "นามสกุลผู้เอาประกัน", "วันเริ่มคุ้มครอง", "วันสิ้นสุดคุ้มครอง", "เลขทะเบียน", "ชื่อเจ้าหน้าที่ MKT" };
                List<string> typLst = new List<string>() { "c", "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" };

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    using (SLM_DBEntities sdc = new SLM_DBEntities())
                    {
                        var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                        if (batch != null)
                        {
                            batch.slm_StartTime = starttime;
                            sdc.SaveChanges();
                        }

                        int ttf = 0;
                        int ttr = 0;
                        foreach (var com in odc.kkslm_ms_ins_com)
                        {
                            string subfolder = folder + "\\" + com.slm_InsABB;
                            if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

                            string filename = subfolder + "\\" + "ระงับเคลม_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
                            var data = from r in sdc.kkslm_tr_setle_claim_report
                                       join t in sdc.kkslm_ms_title on r.slm_Title_Id equals t.slm_TitleId into r1
                                       from t in r1.DefaultIfEmpty()
                                           //join s in sdc.kkslm_ms_staff on r.slm_Owner equals s.slm_UserName into r2  //Comment by Pom 08/07/2016
                                       join s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on r.slm_Owner equals s.slm_EmpCode into r2
                                       from s in r2.DefaultIfEmpty()
                                       orderby r.slm_InsReceiveNo
                                       where r.slm_Ins_Com_Id == com.slm_Ins_Com_Id && (r.slm_Export_Flag == false || r.slm_Export_Flag == null)
                                       select new
                                       {
                                           r.slm_Id,
                                           r.slm_InsReceiveNo,
                                           r.slm_InsReceiveDate,
                                           r.slm_Contract_Number,
                                           t.slm_TitleName,
                                           r.slm_Name,
                                           r.slm_LastName,
                                           r.slm_CoverageStartDate,
                                           r.slm_CoverageEndDate,
                                           r.slm_CarLicenseNo,
                                           s.slm_StaffNameTH
                                       };

                            List<object[]> dataLst = new List<object[]>();
                            int i = 1;
                            foreach (var itm in data)
                            {
                                var obj = new object[]
                                {
                                    itm.slm_InsReceiveNo,
                                    com.slm_InsNameTh,
                                    itm.slm_InsReceiveDate == null ? "" : itm.slm_InsReceiveDate.Value.ToString("d/MM/yyyy HH:mm"),
                                    itm.slm_Contract_Number,
                                    itm.slm_TitleName,
                                    itm.slm_Name,
                                    itm.slm_LastName,
                                    itm.slm_CoverageStartDate == null ? "" : itm.slm_CoverageStartDate.Value.ToString("d/MM/yyyy"),
                                    itm.slm_CoverageEndDate == null ? "" : itm.slm_CoverageEndDate.Value.ToString("d/MM/yyyy"),
                                    itm.slm_CarLicenseNo,
                                    itm.slm_StaffNameTH
                                };
                                dataLst.Add(obj);
                                i++;
                                succ++;

                                // update flag
                                var rpt = sdc.kkslm_tr_setle_claim_report.Where(r => r.slm_Id == itm.slm_Id).FirstOrDefault();
                                if (rpt != null)
                                {
                                    rpt.slm_Export_Flag = true;
                                    rpt.slm_Export_Date = DateTime.Now;
                                    rpt.slm_UpdatedBy = AppName;
                                    rpt.slm_UpdatedDate = DateTime.Now;
                                }

                                i++;
                                total++;
                            }

                            sdc.SaveChanges();
                            if (dataLst.Count > 0) ttf++;
                            ttr += dataLst.Count;
                            writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                        }

                        if (ttf == 0) status = "No data to generate";
                        else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
                        DateTime endtime = DateTime.Now;

                        // add errorlog
                        foreach (var itm in errLst)
                            sdc.kkslm_tr_batchlog.AddObject(itm);

                        // update batch
                        batch.slm_EndTime = endtime;
                        batch.slm_Status = (fail > 0 ? "2" : "1");


                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = starttime;
                        bm.slm_DateEnd = endtime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = AppName;
                        bm.slm_BatchDate = dt;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool GenReport004(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            ILog _log = LogManager.GetLogger(typeof(OBTBatchBiz));//OBT_PRO_16
            _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Start Generate (OBT_PRO_16)");

            status = "";
            string batchCode = "OBT_PRO_16";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            int succ = 0;
            int fail = 0;
            int total = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            bool ret = true;
            try
            {
                // get headers
                List<string> strLst = new List<string>() { "วันที่แจ้งยกเลิกกรมธรรม์", "ชื่อบริษัทประกัน", "เลขรับแจ้ง", "เลขที่สัญญา", "ชื่อ-สกุล ลูกค้า", "ทะเบียนรถยนต์", "ประเภทความคุ้มครอง", "วันคุ้มครอง", "วันสิ้นสุดคุ้มครอง", "ชื่อเจ้าหน้าที่ MKT", "เหตุผลการยกเลิก", "หมายเหตุ" };
                List<string> typLst = new List<string>() { "c", "c", "c", "c", "c", "c", "c", "d", "d", "c", "c", "c" };

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    using (SLM_DBEntities sdc = new SLM_DBEntities())
                    {
                        var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                        if (batch != null)
                        {
                            batch.slm_StartTime = starttime;
                            sdc.SaveChanges();
                        }

                        int ttf = 0;
                        int ttr = 0;
                        foreach (var com in odc.kkslm_ms_ins_com)
                        {
                            string subfolder = folder + "\\" + com.slm_InsABB;
                            if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

                            string filename = subfolder + "\\" + "แจ้งยกเลิกกรมธรรม์_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
                            var data = from r in sdc.kkslm_tr_cancel_policy_report
                                       join t in sdc.kkslm_ms_title on r.slm_Title_Id equals t.slm_TitleId into r1
                                       from t in r1.DefaultIfEmpty()
                                           //join s in sdc.kkslm_ms_staff on r.slm_Owner equals s.slm_UserName into r2  //Comment by Pom 08/07/2016
                                       join s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on r.slm_Owner equals s.slm_EmpCode into r2
                                       from s in r2.DefaultIfEmpty()
                                       join cr in sdc.kkslm_ms_coveragetype on r.slm_CoverageTypeId equals cr.slm_CoverageTypeId into r3
                                       from cr in r3.DefaultIfEmpty()
                                       join cc in sdc.kkslm_ms_cancel on r.slm_CancelId equals cc.slm_CancelId into r4
                                       from cc in r4.DefaultIfEmpty()
                                       orderby r.slm_InsReceiveNo
                                       where (r.slm_Export_Flag == false || r.slm_Export_Flag == null) && r.slm_Ins_Com_Id == com.slm_Ins_Com_Id
                                       select new
                                       {
                                           r.slm_CancelPolicyId,
                                           r.slm_UpdatedDate,
                                           r.slm_InsReceiveNo,
                                           r.slm_Contract_Number,
                                           t.slm_TitleName,
                                           r.slm_Name,
                                           r.slm_LastName,
                                           r.slm_CarLicenseNo,
                                           cr.slm_ConverageTypeName,
                                           r.slm_CoverageStartDate,
                                           r.slm_CoverageEndDate,
                                           s.slm_StaffNameTH,
                                           cc.slm_CancelName,
                                           r.slm_Remark
                                       };

                            List<object[]> dataLst = new List<object[]>();
                            int i = 1;
                            foreach (var itm in data)
                            {
                                var obj = new object[]
                                {
                                    itm.slm_UpdatedDate == null ? "" : (itm.slm_UpdatedDate.Value.ToString("d/MM/") + itm.slm_UpdatedDate.Value.Year.ToString() + " " + itm.slm_UpdatedDate.Value.ToString("HH:mm:ss")),
                                    com.slm_InsNameTh,
                                    itm.slm_InsReceiveNo,
                                    itm.slm_Contract_Number,
                                    itm.slm_TitleName + " " + itm.slm_Name + " " + itm.slm_LastName,
                                    itm.slm_CarLicenseNo,
                                    itm.slm_ConverageTypeName,
                                    itm.slm_CoverageStartDate == null ? "" : itm.slm_CoverageStartDate.Value.ToString("d/MM/yyyy"),
                                    itm.slm_CoverageEndDate == null ? "" : itm.slm_CoverageEndDate.Value.ToString("d/MM/yyyy"),
                                    itm.slm_StaffNameTH,
                                    itm.slm_CancelName,
                                    ""                      //itm.slm_Remark
                                };
                                _log.DebugFormat("{0} ข้อมูลรายงานแจ้งยกเลิกกรมธรรม์จาก database : ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                _log.DebugFormat("UpdateDate : {0}", itm.slm_UpdatedDate == null ? "" : (itm.slm_UpdatedDate.Value.ToString("d/MM/") + itm.slm_UpdatedDate.Value.Year.ToString() + " " + itm.slm_UpdatedDate.Value.ToString("HH:mm:ss")));
                                _log.DebugFormat("InsNameTh : {0}", com.slm_InsNameTh);
                                _log.DebugFormat("InsReceiveNo : {0}", itm.slm_InsReceiveNo);
                                _log.DebugFormat("Contract_Number : {0}", itm.slm_Contract_Number);
                                _log.DebugFormat("TitleName : {0}", itm.slm_TitleName + " " + itm.slm_Name + " " + itm.slm_LastName);
                                _log.DebugFormat("CarLicenseNo : {0}", itm.slm_CarLicenseNo);
                                _log.DebugFormat("ConverageTypeName : {0}", itm.slm_ConverageTypeName);
                                _log.DebugFormat("CoverageStartDate : {0}", itm.slm_CoverageStartDate == null ? "" : itm.slm_CoverageStartDate.Value.ToString("d/MM/yyyy"));
                                _log.DebugFormat("CoverageEndDate : {0}", itm.slm_CoverageEndDate == null ? "" : itm.slm_CoverageEndDate.Value.ToString("d/MM/yyyy"));
                                _log.DebugFormat("StaffNameTH : {0}", itm.slm_StaffNameTH);
                                _log.DebugFormat("CancelName : {0}", itm.slm_CancelName);
                                _log.Debug("Remark : ");
                                _log.Debug("===============================================");

                                dataLst.Add(obj);
                                i++;
                                succ++;

                                // update flag
                                var rpt = sdc.kkslm_tr_cancel_policy_report.Where(r => r.slm_CancelPolicyId == itm.slm_CancelPolicyId).FirstOrDefault();
                                if (rpt != null)
                                {
                                    rpt.slm_Export_Flag = true;
                                    rpt.slm_Export_Date = DateTime.Now;
                                    rpt.slm_UpdatedBy = AppName;
                                    rpt.slm_UpdatedDate = DateTime.Now;
                                }

                                i++;
                                total++;
                            }

                            sdc.SaveChanges();
                            if (dataLst.Count > 0) ttf++;
                            ttr += dataLst.Count;
                            writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                        }

                        if (ttf == 0) status = "No data to generate";
                        else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
                        DateTime endtime = DateTime.Now;

                        // add errorlog
                        foreach (var itm in errLst)
                            sdc.kkslm_tr_batchlog.AddObject(itm);

                        // update batch
                        batch.slm_EndTime = endtime;
                        batch.slm_Status = (fail > 0 ? "2" : "1");


                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = starttime;
                        bm.slm_DateEnd = endtime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = AppName;
                        bm.slm_BatchDate = dt;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();
                    }
                }
                _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " END Generate (OBT_PRO_16)");
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool GenReport005(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            ILog _log = LogManager.GetLogger(typeof(OBTBatchBiz));
            status = "";
            string batchCode = "OBT_PRO_17";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            int succ = 0;
            int fail = 0;
            int total = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            bool ret = true;
            try
            {
                // get headers
                List<string> strLst = new List<string>() { "ลำดับ", "บริษัทประกัน", "เลขที่เครื่องหมาย พรบ", "รหัสสาขา", "สาขา KK", "เลขที่สัญญา", "คำนำหน้าชื่อ", "ชื่อผู้เอาประกัน", "นามสกุลผู้เอาประกัน", "เลขทะเบียน" /* 10 */, "ยี่ห้อรถ", "เลขที่รับแจ้ง", "ชื่อเจ้าหน้าที่ MKT", "เลขตัวรถ", "เบี้ยพรบ.สุทธิ", "เบี้ยพรบ.รวมภาษีอากร", "วันที่เริ่มต้น พรบ", "วันที่สิ้นสุด พรบ", "บ้านเลขที่", "หมู่" /* 20 */, "หมู่บ้าน", "อาคาร", "ซอย", "ถนน", "ตำบล/แขวง", "อำเภอ/เขต", "จังหวัด", "รหัสไปรษณีย์", "หมายเหตุ", "เบอร์ติดต่อ" /* 30 */, "เลขที่บัตรประชาชน", "คำนำหน้าชื่อ 1", "ชื่อกรรมการ 1", "นามสกุลกรรมการ 1", "เลขที่บัตรประชาชนกรรมการ 1", "คำนำหน้าชื่อ 2", "ชื่อกรรมการ 2", "นามสกุลกรรมการ 2", "เลขที่บัตรประชาชนกรรมการ 2", "คำนำหน้าชื่อ 3" /* 40 */, "ชื่อกรรมการ 3", "นามสกุลกรรมการ 3", "เลขที่บัตรประชาชนกรรมการ 3", "จัดส่งเอกสารที่สาขา", "ชื่อผู้รับเอกสาร" };
                List<string> typLst = new List<string>() { "n", "c", "c", "c", "c", "c", "c", "c", "c", "c" /* 10 */, "c", "c", "c", "c", "n", "n", "d", "d", "c", "c" /* 20 */, "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" /* 30 */, "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" /* 40 */, "c", "c", "c", "c", "c" };

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    using (SLM_DBEntities sdc = new SLM_DBEntities())
                    {
                        var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                        if (batch != null)
                        {
                            batch.slm_StartTime = starttime;
                            sdc.SaveChanges();
                        }

                        int ttf = 0;
                        int ttr = 0;
                        foreach (var com in odc.kkslm_ms_ins_com)
                        {
                            string subfolder = folder + "\\" + com.slm_InsABB;
                            if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

                            string filename = subfolder + "\\" + "แจ้งออกพรบ_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
                            var data = from r in sdc.kkslm_tr_notify_act_report
                                       join t in sdc.kkslm_ms_title on r.slm_TitleId equals t.slm_TitleId into r0
                                       from t in r0.DefaultIfEmpty()
                                       join t1 in sdc.kkslm_ms_title on r.slm_TitleId_Committee1 equals t1.slm_TitleId into r1
                                       from t1 in r1.DefaultIfEmpty()
                                       join t2 in sdc.kkslm_ms_title on r.slm_TitleId_Committee2 equals t2.slm_TitleId into r2
                                       from t2 in r2.DefaultIfEmpty()
                                       join t3 in sdc.kkslm_ms_title on r.slm_TitleId_Committee3 equals t3.slm_TitleId into r3
                                       from t3 in r3.DefaultIfEmpty()
                                           //join s in sdc.kkslm_ms_staff on r.slm_Owner equals s.slm_UserName into r4      //Comment by Pom 08/07/2016
                                       join s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on r.slm_Owner equals s.slm_EmpCode into r4
                                       from s in r4.DefaultIfEmpty()
                                       join tm in sdc.kkslm_ms_tambol on r.slm_TambolId equals tm.slm_TambolId into r5
                                       from tm in r5.DefaultIfEmpty()
                                       join am in sdc.kkslm_ms_amphur on r.slm_AmphurId equals am.slm_AmphurId into r6
                                       from am in r6.DefaultIfEmpty()
                                       join pv in sdc.kkslm_ms_province on r.slm_ProvinceId equals pv.slm_ProvinceId into r7
                                       from pv in r7.DefaultIfEmpty()
                                       join br in sdc.kkslm_ms_branch on r.slm_BranchCode equals br.slm_BranchCode into r8
                                       from br in r8.DefaultIfEmpty()
                                       join rb in sdc.kkslm_ms_redbook_brand on r.slm_BrandCode equals rb.slm_BrandCode into r9
                                       from rb in r9.DefaultIfEmpty()
                                       join renew in sdc.kkslm_tr_renewinsurance on r.slm_TicketId equals renew.slm_TicketId into r10
                                       from renew in r10.DefaultIfEmpty()
                                       join brndoc in sdc.kkslm_ms_branch on renew.slm_SendDocBrandCode equals brndoc.slm_BranchCode into r11
                                       from brndoc in r11.DefaultIfEmpty()
                                       orderby r.slm_InsReceiveNo
                                       where (r.slm_Export_Flag == false || r.slm_Export_Flag == null) && r.slm_Ins_Com_Id == com.slm_Ins_Com_Id
                                       select new
                                       {
                                           r.slm_Id,
                                           r.slm_ActNo,
                                           r.slm_BranchCode,
                                           br.slm_BranchName,
                                           r.slm_Contract_Number,
                                           t.slm_TitleName,
                                           r.slm_Name,
                                           r.slm_LastName,
                                           r.slm_CarLicenseNo,
                                           r.slm_BrandCode,
                                           r.slm_InsReceiveNo,
                                           s.slm_StaffNameTH,
                                           r.slm_EngineNo,
                                           r.slm_NetPremium,
                                           r.slm_NetPremiumIncludeVat,
                                           r.slm_StartDateAct,
                                           r.slm_EndDateAct,
                                           r.slm_House_No,
                                           r.slm_Moo,
                                           r.slm_Village,
                                           r.slm_Building,
                                           r.slm_Soi,
                                           r.slm_Street,
                                           tm.slm_TambolNameTH,
                                           am.slm_AmphurNameTH,
                                           pv.slm_ProvinceNameTH,
                                           r.slm_Zipcode,
                                           r.slm_Remark,
                                           r.slm_TelNoContact,
                                           r.slm_CitizenId,
                                           slm_TitleName_c1 = t1.slm_TitleName,
                                           r.slm_Name_Committee1,
                                           r.slm_LastName_Committee1,
                                           r.slm_CitizenId_Committe1,
                                           slm_TitleName_c2 = t2.slm_TitleName,
                                           r.slm_Name_Committee2,
                                           r.slm_LastName_Committee2,
                                           r.slm_CitizenId_Committe2,
                                           slm_TitleName_c3 = t3.slm_TitleName,
                                           r.slm_Name_Committee3,
                                           r.slm_LastName_Committee3,
                                           r.slm_CitizenId_Committe3,
                                           rb.slm_BrandName,
                                           SendDocBranchName = brndoc.slm_BranchName,
                                           SendDocReceiverName = renew.slm_Receiver
                                       };

                            List<object[]> dataLst = new List<object[]>();
                            int i = 1;
                            foreach (var itm in data)
                            {
                                var obj = new object[]
                                {
                                    i.ToString(),
                                    com.slm_InsNameTh,
                                    itm.slm_ActNo,
                                    itm.slm_BranchCode,
                                    itm.slm_BranchName,
                                    itm.slm_Contract_Number,
                                    itm.slm_TitleName,
                                    itm.slm_Name,
                                    itm.slm_LastName,
                                    itm.slm_CarLicenseNo,
                                    itm.slm_BrandName,
                                    itm.slm_InsReceiveNo,
                                    itm.slm_StaffNameTH,
                                    itm.slm_EngineNo,
                                    itm.slm_NetPremium,
                                    itm.slm_NetPremiumIncludeVat,
                                    itm.slm_StartDateAct,
                                    itm.slm_EndDateAct,
                                    itm.slm_House_No,
                                    itm.slm_Moo,
                                    itm.slm_Village,
                                    itm.slm_Building,
                                    itm.slm_Soi,
                                    itm.slm_Street,
                                    itm.slm_TambolNameTH,
                                    itm.slm_AmphurNameTH,
                                    itm.slm_ProvinceNameTH,
                                    itm.slm_Zipcode,
                                    itm.slm_Remark,
                                    itm.slm_TelNoContact,
                                    itm.slm_CitizenId,
                                    itm.slm_TitleName_c1,
                                    itm.slm_Name_Committee1,
                                    itm.slm_LastName_Committee1,
                                    itm.slm_CitizenId_Committe1,
                                    itm.slm_TitleName_c2,
                                    itm.slm_Name_Committee2,
                                    itm.slm_LastName_Committee2,
                                    itm.slm_CitizenId_Committe2,
                                    itm.slm_TitleName_c3,
                                    itm.slm_Name_Committee3,
                                    itm.slm_LastName_Committee3,
                                    itm.slm_CitizenId_Committe3,
                                    itm.SendDocBranchName,
                                    itm.SendDocReceiverName
                                };
                                _log.DebugFormat("{0} ข้อมูลรายงานแจ้งออก พรบ. database : ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                _log.DebugFormat("InsNameTh : {0}", com.slm_InsNameTh);
                                _log.DebugFormat("ActNo : {0}", itm.slm_ActNo);
                                _log.DebugFormat("BranchCode : {0}", itm.slm_BranchCode);
                                _log.DebugFormat("BranchName : {0}", itm.slm_BranchName);
                                _log.DebugFormat("Contract_Number : {0}", itm.slm_Contract_Number);
                                _log.DebugFormat("TitleName : {0}", itm.slm_TitleName);
                                _log.DebugFormat("Name : {0}", itm.slm_Name);
                                _log.DebugFormat("LastName : {0}", itm.slm_LastName);
                                _log.DebugFormat("CarLicenseNo : {0}", itm.slm_CarLicenseNo);
                                _log.DebugFormat("BrandName : {0}", itm.slm_BrandName);
                                _log.DebugFormat("InsReceiveNo : {0}", itm.slm_InsReceiveNo);
                                _log.DebugFormat("StaffNameTH : {0}", itm.slm_StaffNameTH);
                                _log.DebugFormat("EngineNo : {0}", itm.slm_EngineNo);
                                _log.DebugFormat("NetPremium : {0}", itm.slm_NetPremium);
                                _log.DebugFormat("NetPremiumIncludeVat : {0}", itm.slm_NetPremiumIncludeVat);
                                _log.DebugFormat("StartDateAct : {0}", itm.slm_StartDateAct);
                                _log.DebugFormat("EndDateAct : {0}", itm.slm_EndDateAct);
                                _log.DebugFormat("House_No : {0}", itm.slm_House_No);
                                _log.DebugFormat("Moo : {0}", itm.slm_Moo);
                                _log.DebugFormat("Village : {0}", itm.slm_Village);
                                _log.DebugFormat("Building : {0}", itm.slm_Building);
                                _log.DebugFormat("Soi : {0}", itm.slm_Soi);
                                _log.DebugFormat("Street : {0}", itm.slm_Street);
                                _log.DebugFormat("TambolNameTH : {0}", itm.slm_TambolNameTH);
                                _log.DebugFormat("AmphurNameTH : {0}", itm.slm_AmphurNameTH);
                                _log.DebugFormat("ProvinceNameTH : {0}", itm.slm_ProvinceNameTH);
                                _log.DebugFormat("Zipcode : {0}", itm.slm_Zipcode);
                                _log.DebugFormat("Remark : {0}", itm.slm_Remark);
                                _log.DebugFormat("TelNoContact : {0}", itm.slm_TelNoContact);
                                _log.DebugFormat("CitizenId : {0}", itm.slm_CitizenId);
                                _log.DebugFormat("TitleName_c1 : {0}", itm.slm_TitleName_c1);
                                _log.DebugFormat("Name_Committee1 : {0}", itm.slm_Name_Committee1);
                                _log.DebugFormat("LastName_Committee1 : {0}", itm.slm_LastName_Committee1);
                                _log.DebugFormat("CitizenId_Committe1 : {0}", itm.slm_CitizenId_Committe1);
                                _log.DebugFormat("TitleName_c2 : {0}", itm.slm_TitleName_c2);
                                _log.DebugFormat("Name_Committee2 : {0}", itm.slm_Name_Committee2);
                                _log.DebugFormat("LastName_Committee2 : {0}", itm.slm_LastName_Committee2);
                                _log.DebugFormat("CitizenId_Committe2 : {0}", itm.slm_CitizenId_Committe2);
                                _log.DebugFormat("TitleName_c3 : {0}", itm.slm_TitleName_c3);
                                _log.DebugFormat("Name_Committee3 : {0}", itm.slm_Name_Committee3);
                                _log.DebugFormat("LastName_Committee3 : {0}", itm.slm_LastName_Committee3);
                                _log.DebugFormat("CitizenId_Committe3 : {0}", itm.slm_CitizenId_Committe3);
                                _log.Debug("===============================================");

                                dataLst.Add(obj);
                                i++;
                                succ++;

                                // update flag
                                var rpt = sdc.kkslm_tr_notify_act_report.Where(r => r.slm_Id == itm.slm_Id).FirstOrDefault();
                                if (rpt != null)
                                {
                                    rpt.slm_Export_Flag = true;
                                    rpt.slm_Export_Date = DateTime.Now;
                                    rpt.slm_UpdatedBy = AppName;
                                    rpt.slm_UpdatedDate = DateTime.Now;
                                }

                                //i++;  //ซ้ำข้างบน
                                total++;
                            }

                            sdc.SaveChanges();
                            if (dataLst.Count > 0) ttf++;
                            ttr += dataLst.Count;
                            writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                        }

                        if (ttf == 0) status = "No data to generate";
                        else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
                        DateTime endtime = DateTime.Now;

                        // add errorlog
                        foreach (var itm in errLst)
                            sdc.kkslm_tr_batchlog.AddObject(itm);

                        // update batch
                        batch.slm_EndTime = endtime;
                        batch.slm_Status = (fail > 0 ? "2" : "1");


                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = starttime;
                        bm.slm_DateEnd = endtime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = AppName;
                        bm.slm_BatchDate = dt;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool GenReport006(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            status = "";
            string batchCode = "OBT_PRO_18";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            int succ = 0;
            int fail = 0;
            int total = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            bool ret = true;
            try
            {
                // get headers
                List<string> strLst = new List<string>() { "วันที่แจ้งยกเลิก พรบ.", "ชื่อบริษัทประกันภัย", "ชื่อ-สกุล ลูกค้า", "เลขที่สัญญา", "เลขที่ พรบ.", "ทะเบียน-จังหวัด", "วันที่แจ้งงาน", "วันที่คุ้มครอง", "วันสิ้นสุดคุ้มครอง", "ชื่อเจ้าหน้าที่ MK", "เหตุผลการยกเลิกพรบ.", "หมายเหตุ" };
                List<string> typLst = new List<string>() { "c", "c", "c", "c", "c", "c", "c", "d", "d", "c", "c", "c" };

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    using (SLM_DBEntities sdc = new SLM_DBEntities())
                    {
                        var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                        if (batch != null)
                        {
                            batch.slm_StartTime = starttime;
                            sdc.SaveChanges();
                        }

                        int ttf = 0;
                        int ttr = 0;
                        foreach (var com in odc.kkslm_ms_ins_com)
                        {
                            string subfolder = folder + "\\" + com.slm_InsABB;
                            if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

                            string filename = subfolder + "\\" + "แจ้งยกเลิกพรบ_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
                            var data = from r in sdc.kkslm_tr_cancel_act_report
                                       join t in sdc.kkslm_ms_title on r.slm_Title_Id equals t.slm_TitleId into r0
                                       from t in r0.DefaultIfEmpty()
                                       join p in sdc.kkslm_ms_province on r.slm_ProvinceRegis equals p.slm_ProvinceId into r1
                                       from p in r1.DefaultIfEmpty()
                                       join s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on r.slm_Owner equals s.slm_EmpCode into r2
                                       from s in r2.DefaultIfEmpty()
                                       where (r.slm_Export_Flag == false || r.slm_Export_Flag == null) && r.slm_Ins_Com_Id == com.slm_Ins_Com_Id
                                       select new
                                       {
                                           r.slm_CancelActId,
                                           r.slm_CancelDate,
                                           t.slm_TitleName,
                                           r.slm_Name,
                                           r.slm_LastName,
                                           r.slm_Contract_Number,
                                           r.slm_ActNo,
                                           r.slm_LicenseNo,
                                           p.slm_ProvinceNameTH,
                                           r.slm_ReceiveDate,
                                           r.slm_ActStartCoverDate,
                                           r.slm_ActEndCoverDate,
                                           s.slm_StaffNameTH,
                                           r.slm_ReasonCancel,
                                           r.slm_Remark
                                       };

                            List<object[]> dataLst = new List<object[]>();
                            int i = 1;
                            foreach (var itm in data)
                            {
                                var obj = new object[]
                                {
                                    itm.slm_CancelDate == null ? "" : (itm.slm_CancelDate.Value.ToString("dd/MM/") + itm.slm_CancelDate.Value.Year.ToString() + " " + itm.slm_CancelDate.Value.ToString("HH:mm:ss")),
                                    com.slm_InsNameTh,
                                    itm.slm_TitleName + " " + itm.slm_Name + " " + itm.slm_LastName,
                                    itm.slm_Contract_Number,
                                    itm.slm_ActNo,
                                    itm.slm_LicenseNo,
                                    itm.slm_ReceiveDate == null ? "" : (itm.slm_ReceiveDate.Value.ToString("dd/MM/") + itm.slm_ReceiveDate.Value.Year.ToString() + " " + itm.slm_ReceiveDate.Value.ToString("HH:mm:ss")),
                                    itm.slm_ActStartCoverDate,
                                    itm.slm_ActEndCoverDate,
                                    itm.slm_StaffNameTH,
                                    itm.slm_ReasonCancel,
                                    ""                          //itm.slm_Remark
                                };
                                dataLst.Add(obj);
                                i++;
                                succ++;

                                // update flag
                                var rpt = sdc.kkslm_tr_cancel_act_report.Where(r => r.slm_CancelActId == itm.slm_CancelActId).FirstOrDefault();
                                if (rpt != null)
                                {
                                    rpt.slm_Export_Flag = true;
                                    rpt.slm_Export_Date = DateTime.Now;
                                    rpt.slm_UpdatedBy = AppName;
                                    rpt.slm_UpdatedDate = DateTime.Now;
                                }

                                i++;
                                total++;
                            }

                            sdc.SaveChanges();
                            if (dataLst.Count > 0) ttf++;
                            ttr += dataLst.Count;
                            writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                        }

                        if (ttf == 0) status = "No data to generate";
                        else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
                        DateTime endtime = DateTime.Now;

                        // add error log
                        foreach (var itm in errLst)
                            sdc.kkslm_tr_batchlog.AddObject(itm);

                        // update batch
                        batch.slm_EndTime = endtime;
                        batch.slm_Status = (fail > 0 ? "2" : "1");


                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = starttime;
                        bm.slm_DateEnd = endtime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = AppName;
                        bm.slm_BatchDate = dt;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool GenReport007(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            status = "";
            string batchCode = "OBT_PRO_19";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            int succ = 0;
            int fail = 0;
            int total = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            bool ret = true;
            try
            {
                // get headers
                List<string> strLst = new List<string>() { "วันที่แจ้งกลับงานติดปัญหา", "บริษัทประกัน", "ประเภทงานติดปัญหา", "เลขที่สัญญา", "ชื่อ-สกุล ลูกค้า", "Telesale", "ปัญหา", "สาเหตุ", "หัวหน้าทีม", "แจ้งผลกลับ", "เบอร์ติดต่อกลับลูกค้า", "วันที่แจ้งผลกลับ", "หมายเหตุ" };
                List<string> typLst = new List<string>() { "c", "c", "c", "c", "c", "c", "c", "c", "c", "c", "c", "c", "c" };

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    using (SLM_DBEntities sdc = new SLM_DBEntities())
                    {
                        var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                        if (batch != null)
                        {
                            batch.slm_StartTime = starttime;
                            batch.slm_Status = "0";
                            sdc.SaveChanges();
                        }

                        int ttf = 0;
                        int ttr = 0;
                        foreach (var com in odc.kkslm_ms_ins_com)
                        {
                            string subfolder = folder + "\\" + com.slm_InsABB;
                            if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

                            string filename = subfolder + "\\" + "แจ้งกลับงานติดปัญหา_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
                            var data = from pcr in sdc.kkslm_tr_problemcomeback_report
                                       join pd in sdc.kkslm_tr_problemdetail on pcr.slm_ProblemDetailId equals pd.slm_ProblemId into r1
                                       from t in r1.DefaultIfEmpty()
                                           //join s in sdc.kkslm_ms_staff on pcr.slm_Owner equals s.slm_UserName into r2    //Comment by Pom 08/07/2016
                                       join s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on pcr.slm_Owner equals s.slm_EmpCode into r2
                                       from s in r2.DefaultIfEmpty()
                                       orderby pcr.slm_ProblemDate
                                       where pcr.slm_Ins_Com_Id == com.slm_Ins_Com_Id && (pcr.slm_Export_Flag == false || pcr.slm_Export_Flag == null)
                                       select new
                                       {
                                           pcr.slm_ProblemComebackId,
                                           pcr.slm_UpdatedDate,
                                           com.slm_InsNameTh,
                                           pcr.slm_InsType,
                                           pcr.slm_Contract_Number,
                                           s.slm_StaffNameTH,

                                           pcr.slm_Name,
                                           pcr.slm_ProblemDetail,
                                           pcr.slm_CauseDetail,
                                           pcr.slm_HeadStaff,
                                           pcr.slm_ResponseDetail,

                                           pcr.slm_PhoneContact,
                                           pcr.slm_ResponseDate,
                                           pcr.slm_Remark
                                       };

                            List<object[]> dataLst = new List<object[]>();
                            int i = 1;
                            foreach (var itm in data)
                            {
                                var obj = new object[]
                                {

                                    itm.slm_UpdatedDate == null ? "" : itm.slm_UpdatedDate.Value.ToString("d/MM/yyyy"),
                                    itm.slm_InsNameTh,
                                    itm.slm_InsType,
                                    itm.slm_Contract_Number,
                                    itm.slm_Name,
                                    itm.slm_StaffNameTH,
                                    itm.slm_ProblemDetail,
                                    itm.slm_CauseDetail,
                                    itm.slm_HeadStaff,
                                    itm.slm_ResponseDetail,

                                    itm.slm_PhoneContact,
                                    itm.slm_ResponseDate == null ? "" : itm.slm_ResponseDate.Value.ToString("d/MM/yyyy"),
                                    itm.slm_Remark
                                };
                                dataLst.Add(obj);
                                i++;
                                succ++;

                                // update flag
                                var rpt = sdc.kkslm_tr_problemcomeback_report.Where(r => r.slm_ProblemComebackId == itm.slm_ProblemComebackId).FirstOrDefault();
                                if (rpt != null)
                                {
                                    rpt.slm_Export_Flag = true;
                                    rpt.slm_Export_Date = DateTime.Now;
                                    rpt.slm_UpdatedBy = AppName;
                                    rpt.slm_UpdatedDate = DateTime.Now;
                                }

                            }
                            sdc.SaveChanges();
                            if (dataLst.Count > 0) ttf++;
                            ttr += dataLst.Count;
                            writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                        }

                        if (ttf == 0) status = "No data to generate";
                        else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
                        DateTime endtime = DateTime.Now;

                        // add errorlog
                        foreach (var itm in errLst)
                            sdc.kkslm_tr_batchlog.AddObject(itm);

                        // update batch
                        batch.slm_EndTime = endtime;
                        batch.slm_Status = (fail > 0 ? "2" : "1");


                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = starttime;
                        bm.slm_DateEnd = endtime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = AppName;
                        bm.slm_BatchDate = dt;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool GenReport008(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            status = "";
            string batchCode = "OBT_PRO_20";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            int succ = 0;
            int fail = 0;
            int total = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            bool ret = true;
            try
            {
                // get headers
                List<string> strLst = new List<string>() {"ลำดับที่", "บริษัทประกัน", "เลขที่กรมธรรม์", "รหัสสาขา", "สาขา KK"
                    , "เลขที่สัญญา", "คำนำหน้าชื่อ", "ชื่อผู้เอาประกัน", "นามสกุลผู้เอาประกัน", "เลขที่รับแจ้ง"
                    , "วันที่ออกเลขที่รับแจ้ง", "เลขทะเบียน", "ยี่ห้อรถ", "เลขตัวรถ", "เจ้าหน้าที่ MKT"
                    , "เจ้าหน้าที่ที่ขอ", "ประเภทความคุ้มครอง", "ประเภทการซ่อม", "ทุนประกัน", "เบี้ยรวมภาษีและอากร"
                    , "วันเริ่มต้น พ.ร.บ.", "วันสิ้นสุด พ.ร.บ.", "ชื่อผู้รับ", "บ้านเลขที่", "หมู่"
                    , "หมู่บ้าน", "อาคาร", "ซอย", "ถนน", "ตำบล/แขวง"
                    , "อำเภอ/เขต", "จังหวัด", "รหัสไปรษณีย์", "หมายเหตุ", "เบอร์ติดต่อ"
                    , "จัดส่งเอกสารที่สาขา", "ชื่อผู้รับเอกสาร" };
                List<string> typLst = new List<string>() { "n", "c", "c", "c", "c"
                    , "c", "c", "c", "c", "c"
                    , "c", "c", "c", "c", "c"
                    , "c", "c", "c", "n", "n"
                    , "c", "c", "c", "c", "c"
                    , "c", "c", "c", "c", "c"
                    , "c", "c", "c", "c", "c"
                    , "c", "c" };

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    using (SLM_DBEntities sdc = new SLM_DBEntities())
                    {
                        var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                        if (batch != null)
                        {
                            batch.slm_StartTime = starttime;
                            batch.slm_Status = "0";
                            sdc.SaveChanges();
                        }

                        int ttf = 0;
                        int ttr = 0;
                        foreach (var com in odc.kkslm_ms_ins_com)
                        {
                            string subfolder = folder + "\\" + com.slm_InsABB;
                            if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

                            string filename = subfolder + "\\" + "ขอออกกรมธรรม์ใหม่_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
                            var data = from pr in sdc.kkslm_tr_policynew_report
                                       join title in sdc.kkslm_ms_title on pr.slm_Title_Id equals title.slm_TitleId into r1
                                       from t in r1.DefaultIfEmpty()
                                           //join s in sdc.kkslm_ms_staff on pr.slm_Owner equals s.slm_UserName into r2     //Comment by Pom 08/07/2016
                                       join s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on pr.slm_Owner equals s.slm_EmpCode into r2
                                       from s in r2.DefaultIfEmpty()
                                       join sr in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on pr.slm_StaffRequest equals sr.slm_EmpCode into sr1
                                       from sr in sr1.DefaultIfEmpty()
                                       join redbook in sdc.kkslm_ms_redbook_brand on pr.slm_BrandCode equals redbook.slm_BrandCode into r3
                                       from redbook in r3.DefaultIfEmpty()
                                       join repairtype in sdc.kkslm_ms_repairtype on pr.slm_RepairTypeId equals repairtype.slm_RepairTypeId into r4
                                       from repairtype in r4.DefaultIfEmpty()
                                       join tambon in sdc.kkslm_ms_tambol on pr.slm_TambolId equals tambon.slm_TambolId into r5
                                       from tambon in r5.DefaultIfEmpty()
                                       join amphor in sdc.kkslm_ms_amphur on pr.slm_AmphurId equals amphor.slm_AmphurId into r6
                                       from amphor in r6.DefaultIfEmpty()
                                       join province in sdc.kkslm_ms_province on pr.slm_ProvinceId equals province.slm_ProvinceId into r7
                                       from province in r7.DefaultIfEmpty()
                                       join branch in sdc.kkslm_ms_branch on pr.slm_BrachCode equals branch.slm_BranchCode into r8
                                       from branch in r8.DefaultIfEmpty()
                                       join cover in sdc.kkslm_ms_coveragetype on pr.slm_CoverageTypeId equals cover.slm_CoverageTypeId into r9
                                       from cover in r9.DefaultIfEmpty()
                                       join renew in sdc.kkslm_tr_renewinsurance on pr.slm_TicketId equals renew.slm_TicketId into r10
                                       from renew in r10.DefaultIfEmpty()
                                       join brndoc in sdc.kkslm_ms_branch on renew.slm_SendDocBrandCode equals brndoc.slm_BranchCode into r11
                                       from brndoc in r11.DefaultIfEmpty()

                                       orderby pr.slm_IsAddressBranch, pr.slm_ReceiveDate
                                       where pr.slm_Ins_Com_Id == com.slm_Ins_Com_Id && (pr.slm_Export_Flag == false || pr.slm_Export_Flag == null)
                                       select new
                                       {
                                           pr.slm_PolicyNewId,
                                           com.slm_InsNameTh,
                                           pr.slm_PolicyNo,
                                           pr.slm_BrachCode,
                                           branch.slm_BranchName,
                                           pr.slm_Contract_Number,
                                           t.slm_TitleName,
                                           pr.slm_Name,
                                           pr.slm_LastName,
                                           pr.slm_InsReceiveNo,
                                           pr.slm_ReceiveDate,
                                           pr.slm_CarLicenseNo,
                                           redbook.slm_BrandName,
                                           pr.slm_ChassisNo,
                                           s.slm_StaffNameTH,
                                           pr.slm_StaffRequest,
                                           cover.slm_ConverageTypeName,
                                           repairtype.slm_RepairTypeName,
                                           pr.slm_PolicyCost,
                                           pr.slm_PolicyGrossPremium,
                                           pr.slm_ActStartCoverDate,
                                           pr.slm_ActEndCoverDate,
                                           pr.slm_ReceiverName,
                                           pr.slm_House_No,
                                           pr.slm_Moo,
                                           pr.slm_Village,
                                           pr.slm_Building,
                                           pr.slm_Soi,
                                           pr.slm_Street,
                                           tambon.slm_TambolNameTH,
                                           amphor.slm_AmphurNameTH,
                                           province.slm_ProvinceNameTH,
                                           pr.slm_Zipcode,
                                           pr.slm_Remark,
                                           pr.slm_PhoneContact,
                                           StaffRequestNameTH = sr.slm_StaffNameTH,
                                           SendDocBranchName = brndoc.slm_BranchName,
                                           SendDocReceiverName = renew.slm_Receiver
                                       };

                            List<object[]> dataLst = new List<object[]>();
                            int i = 1;
                            foreach (var itm in data)
                            {
                                var obj = new object[]
                                {
                                    i,
                                    itm.slm_InsNameTh,
                                    itm.slm_PolicyNo,
                                    itm.slm_BrachCode,
                                    itm.slm_BranchName,

                                    itm.slm_Contract_Number,
                                    itm.slm_TitleName,
                                    itm.slm_Name,
                                    itm.slm_LastName,
                                    itm.slm_InsReceiveNo,

                                    itm.slm_ReceiveDate,
                                    itm.slm_CarLicenseNo,
                                    itm.slm_BrandName,
                                    itm.slm_ChassisNo,
                                    itm.slm_StaffNameTH,

                                    itm.StaffRequestNameTH,
                                    itm.slm_ConverageTypeName,
                                    itm.slm_RepairTypeName,
                                    itm.slm_PolicyCost,
                                    itm.slm_PolicyGrossPremium,

                                    itm.slm_ActStartCoverDate == null ? "" : itm.slm_ActStartCoverDate.Value.ToString("dd/MM/yyyy"),
                                    itm.slm_ActEndCoverDate == null ? "" : itm.slm_ActEndCoverDate.Value.ToString("dd/MM/yyyy"),
                                    itm.slm_ReceiverName,
                                    itm.slm_House_No,
                                    itm.slm_Moo,

                                    itm.slm_Village,
                                    itm.slm_Building,
                                    itm.slm_Soi,
                                    itm.slm_Street,
                                    itm.slm_TambolNameTH,

                                    itm.slm_AmphurNameTH,
                                    itm.slm_ProvinceNameTH,
                                    itm.slm_Zipcode,
                                    itm.slm_Remark,
                                    itm.slm_PhoneContact,

                                    itm.SendDocBranchName,
                                    itm.SendDocReceiverName
                                };
                                dataLst.Add(obj);
                                i++;
                                succ++;

                                // update flag
                                var rpt = sdc.kkslm_tr_policynew_report.Where(r => r.slm_PolicyNewId == itm.slm_PolicyNewId).FirstOrDefault();
                                if (rpt != null)
                                {
                                    rpt.slm_Export_Flag = true;
                                    rpt.slm_Export_Date = DateTime.Now;
                                    rpt.slm_UpdatedBy = AppName;
                                    rpt.slm_UpdatedDate = DateTime.Now;
                                }

                            }
                            sdc.SaveChanges();
                            if (dataLst.Count > 0) ttf++;
                            ttr += dataLst.Count;
                            writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                        }

                        if (ttf == 0) status = "No data to generate";
                        else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
                        DateTime endtime = DateTime.Now;

                        // add errorlog
                        foreach (var itm in errLst)
                            sdc.kkslm_tr_batchlog.AddObject(itm);

                        // update batch
                        batch.slm_EndTime = endtime;
                        batch.slm_Status = (fail > 0 ? "2" : "1");


                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = starttime;
                        bm.slm_DateEnd = endtime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = AppName;
                        bm.slm_BatchDate = dt;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool GenReport009(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            status = "";
            string batchCode = "OBT_PRO_21";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            int succ = 0;
            int fail = 0;
            int total = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            bool ret = true;
            try
            {
                // get headers
                List<string> strLst = new List<string>() {"ลำดับที่", "บริษัทประกัน", "เลขที่กรมธรรม์ พ.ร.บ.", "รหัสสาขา", "สาขา KK"
                    , "เลขที่สัญญา", "คำนำหน้าชื่อ", "ชื่อผู้เอาประกัน", "นามสกุลผู้เอาประกัน", "เลขที่รับแจ้ง"
                    , "เลขทะเบียน", "ยี่ห้อรถ", "เลขตัวรถ", "เจ้าหน้าที่ MKT", "เจ้าหน้าที่ที่ขอ"
                    , "เบี้ยพรบ.สุทธิ", "เบี้ยพรบ.รวมภาษีอากร", "วันเริ่มต้น พ.ร.บ.", "วันสิ้นสุด พ.ร.บ.", "วันที่แจ้งงาน"
                    , "ชื่อผู้รับ", "บ้านเลขที่", "หมู่", "หมู่บ้าน", "อาคาร"
                    , "ซอย", "ถนน", "ตำบล/แขวง", "อำเภอ/เขต", "จังหวัด"
                    , "รหัสไปรษณีย์", "หมายเหตุ", "เบอร์ติดต่อ", "จัดส่งเอกสารที่สาขา", "ชื่อผู้รับเอกสาร" };
                List<string> typLst = new List<string>() { "n", "c", "c", "c", "c"
                    , "c", "c", "c", "c", "c"
                    , "c", "c", "c", "c", "c"
                    , "n", "n", "c", "c", "c"
                    , "c", "c", "c", "c", "c"
                    , "c", "c", "c", "c", "c"
                    , "c", "c", "c", "c", "c" };

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    using (SLM_DBEntities sdc = new SLM_DBEntities())
                    {
                        var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                        if (batch != null)
                        {
                            batch.slm_StartTime = starttime;
                            batch.slm_Status = "0";
                            sdc.SaveChanges();
                        }

                        int ttf = 0;
                        int ttr = 0;
                        foreach (var com in odc.kkslm_ms_ins_com)
                        {

                            string subfolder = folder + "\\" + com.slm_InsABB;
                            if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

                            string filename = subfolder + "\\" + "แจ้งขอออกสำเนาพรบ_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
                            var data = from pr in sdc.kkslm_tr_actnew_report
                                       join title in sdc.kkslm_ms_title on pr.slm_Title_Id equals title.slm_TitleId into r1
                                       from t in r1.DefaultIfEmpty()
                                           //join s in sdc.kkslm_ms_staff on pr.slm_Owner equals s.slm_UserName into r2     //Comment by Pom 08/07/2016
                                       join s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on pr.slm_Owner equals s.slm_EmpCode into r2
                                       from s in r2.DefaultIfEmpty()
                                       join sr in sdc.kkslm_ms_staff.Where(st => st.slm_EmpCode != null) on pr.slm_StaffRequest equals sr.slm_EmpCode into r9
                                       from sr in r9.DefaultIfEmpty()
                                       join redbook in sdc.kkslm_ms_redbook_brand on pr.slm_BrandCode equals redbook.slm_BrandCode into r3
                                       from redbook in r3.DefaultIfEmpty()
                                       join tambon in sdc.kkslm_ms_tambol on pr.slm_TambolId equals tambon.slm_TambolId into r5
                                       from tambon in r5.DefaultIfEmpty()
                                       join amphor in sdc.kkslm_ms_amphur on pr.slm_AmphurId equals amphor.slm_AmphurId into r6
                                       from amphor in r6.DefaultIfEmpty()
                                       join province in sdc.kkslm_ms_province on pr.slm_ProvinceId equals province.slm_ProvinceId into r7
                                       from province in r7.DefaultIfEmpty()
                                       join branch in sdc.kkslm_ms_branch on pr.slm_BrachCode equals branch.slm_BranchCode into r8
                                       from branch in r8.DefaultIfEmpty()
                                       join renew in sdc.kkslm_tr_renewinsurance on pr.slm_TicketId equals renew.slm_TicketId into r10
                                       from renew in r10.DefaultIfEmpty()
                                       join brndoc in sdc.kkslm_ms_branch on renew.slm_SendDocBrandCode equals brndoc.slm_BranchCode into r11
                                       from brndoc in r11.DefaultIfEmpty()

                                       orderby pr.slm_IsAddressBranch, pr.slm_ReceiveDate
                                       where pr.slm_Ins_Com_Id == com.slm_Ins_Com_Id && (pr.slm_Export_Flag == false || pr.slm_Export_Flag == null)
                                       select new
                                       {
                                           pr.slm_ActNewId,
                                           com.slm_InsNameTh,
                                           pr.slm_ActNo,
                                           pr.slm_BrachCode,
                                           branch.slm_BranchName,

                                           pr.slm_Contract_Number,
                                           t.slm_TitleName,
                                           pr.slm_Name,
                                           pr.slm_LastName,
                                           pr.slm_InsReceiveNo,

                                           pr.slm_CarLicenseNo,
                                           redbook.slm_BrandName,
                                           pr.slm_ChassisNo,
                                           s.slm_StaffNameTH,
                                           slm_StaffRequest = sr.slm_StaffNameTH,

                                           pr.slm_ActGrossPremium,
                                           pr.slm_PolicyGrossPremium,
                                           pr.slm_ActStartCoverDate,
                                           pr.slm_ActEndCoverDate,
                                           pr.slm_ReceiveDate,

                                           pr.slm_ReceiverName,
                                           pr.slm_House_No,
                                           pr.slm_Moo,
                                           pr.slm_Village,
                                           pr.slm_Building,

                                           pr.slm_Soi,
                                           pr.slm_Street,
                                           tambon.slm_TambolNameTH,
                                           amphor.slm_AmphurNameTH,
                                           province.slm_ProvinceNameTH,

                                           pr.slm_Zipcode,
                                           pr.slm_Remark,
                                           pr.slm_PhoneContact,
                                           SendDocBranchName = brndoc.slm_BranchName,
                                           SendDocReceiverName = renew.slm_Receiver
                                       };
                            try
                            {
                                List<object[]> dataLst = new List<object[]>();
                                int i = 1;
                                foreach (var itm in data)
                                {
                                    var obj = new object[]
                                    {
                                        i,
                                        com.slm_InsNameTh,
                                        itm.slm_ActNo,
                                        itm.slm_BrachCode,
                                        itm.slm_BranchName,

                                        itm.slm_Contract_Number,
                                        itm.slm_TitleName,
                                        itm.slm_Name,
                                        itm.slm_LastName,
                                        itm.slm_InsReceiveNo,

                                        itm.slm_CarLicenseNo,
                                        itm.slm_BrandName,
                                        itm.slm_ChassisNo,
                                        itm.slm_StaffNameTH,
                                        itm.slm_StaffRequest,

                                        itm.slm_ActGrossPremium,
                                        itm.slm_PolicyGrossPremium,
                                        itm.slm_ActStartCoverDate == null ? "" : itm.slm_ActStartCoverDate.Value.ToString("dd/MM/yyyy"),
                                        itm.slm_ActEndCoverDate == null ? "" : itm.slm_ActEndCoverDate.Value.ToString("dd/MM/yyyy"),
                                        itm.slm_ReceiveDate == null ? "" : itm.slm_ReceiveDate.Value.ToString("dd/MM/yyyy"),

                                        itm.slm_ReceiverName,
                                        itm.slm_House_No,
                                        itm.slm_Moo,
                                        itm.slm_Village,
                                        itm.slm_Building,

                                        itm.slm_Soi,
                                        itm.slm_Street,
                                        itm.slm_TambolNameTH,
                                        itm.slm_AmphurNameTH,
                                        itm.slm_ProvinceNameTH,

                                        itm.slm_Zipcode,
                                        itm.slm_Remark,
                                        itm.slm_PhoneContact,
                                        itm.SendDocBranchName,
                                        itm.SendDocReceiverName
                                    };
                                    dataLst.Add(obj);
                                    i++;
                                    succ++;

                                    // update flag
                                    var rpt = sdc.kkslm_tr_actnew_report.Where(r => r.slm_ActNewId == itm.slm_ActNewId).FirstOrDefault();
                                    if (rpt != null)
                                    {
                                        rpt.slm_Export_Flag = true;
                                        rpt.slm_Export_Date = DateTime.Now;
                                        rpt.slm_UpdatedBy = AppName;
                                        rpt.slm_UpdatedDate = DateTime.Now;
                                    }

                                }
                                sdc.SaveChanges();
                                if (dataLst.Count > 0) ttf++;
                                ttr += dataLst.Count;
                                writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                            }
                            catch (Exception ex)
                            {
                                foreach (var itm in data)
                                {
                                    errLst.Add(new kkslm_tr_batchlog()
                                    {
                                        slm_ErrorDetail = ex.Message
                                    });
                                }
                            }

                        }

                        if (ttf == 0) status = "No data to generate";
                        else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
                        DateTime endtime = DateTime.Now;

                        // update batch
                        batch.slm_EndTime = endtime;
                        batch.slm_Status = (errLst.Count > 0 ? "2" : "1");


                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = starttime;
                        bm.slm_DateEnd = endtime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = AppName;
                        bm.slm_BatchDate = dt;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();

                        // add errorlog
                        foreach (var itm in errLst)
                        {
                            itm.slm_BatchMonitor_Id = bm.slm_BatchMonitor_Id;
                            sdc.kkslm_tr_batchlog.AddObject(itm);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool GenReport010(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            status = "";
            bool ret = true;
            string batchCode = "OBT_PRO_22";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string filename = folder + "\\" + "ข้อมูลรายงานการแก้ไขใบเสร็จ_" + dt.Year.ToString() + dt.ToString("MMdd_HHmm") + ".xls";
            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            int succ = 0;
            int fail = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            try
            {

                using (SLM_DBEntities sdc = new SLM_DBEntities())
                {
                    var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                    if (batch != null)
                    {
                        batch.slm_StartTime = starttime;
                        sdc.SaveChanges();
                    }

                    // get headers
                    List<string> strLst = new List<string>() { "วันที่แก้ไขใบเสร็จ", "ชื่อลูกค้า", "เลขที่สัญญา", "ทะเบียน", "เลขที่ใบเสร็จ", "วันที่ใบเสร็จ", "ข้อมูลเดิม1", "เงินเดิม1", "ข้อมูลใหม่1", "เงินใหม่1", "ข้อมูลใหม่2", "เงินใหม่2", "ข้อมูลใหม่3", "เงินใหม่3", "ข้อมูลใหม่4", "เงินใหม่4", "ข้อมูลใหม่5", "เงินใหม่5", "ข้อมูลใหม่6", "เงินใหม่6", "ยอดรวม", "MKT", "ทีม MKT", "รหัสพนักงาน", "ที่อยู่ในการส่งเอกสาร" };
                    List<string> typLst = new List<string>() { "d", "c", "c", "c", "c", "d", "c", "n", "c", "n", "c", "n", "c", "n", "c", "n", "c", "n", "c", "n", "n", "c", "c", "c", "c" };
                    // get data
                    var data = from r in sdc.kkslm_tr_amendmentbill_report
                               join t in sdc.kkslm_ms_title on r.slm_Title_Id equals t.slm_TitleId into r1
                               from t in r1.DefaultIfEmpty()
                                   //join s in sdc.kkslm_ms_staff on r.slm_Owner equals s.slm_UserName into r2  //Comment by Pom 08/07/2016
                               join s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on r.slm_Owner equals s.slm_EmpCode into r2
                               from s in r2.DefaultIfEmpty()
                                   //join po in sdc.kkslm_ms_payment on r.slm_PaymentCodeOld1 equals po.slm_PaymentCode into r3
                                   //from po in r3.DefaultIfEmpty()
                                   //join pn1 in sdc.kkslm_ms_payment on r.slm_PaymentCodeNew1 equals pn1.slm_PaymentCode into r4
                                   //from pn1 in r4.DefaultIfEmpty()
                                   //join pn2 in sdc.kkslm_ms_payment on r.slm_PaymentCodeNew2 equals pn2.slm_PaymentCode into r5
                                   //from pn2 in r5.DefaultIfEmpty()
                                   //join pn3 in sdc.kkslm_ms_payment on r.slm_PaymentCodeNew3 equals pn3.slm_PaymentCode into r6
                                   //from pn3 in r6.DefaultIfEmpty()
                                   //join pn4 in sdc.kkslm_ms_payment on r.slm_PaymentCodeNew4 equals pn4.slm_PaymentCode into r7
                                   //from pn4 in r7.DefaultIfEmpty()
                                   //join pn5 in sdc.kkslm_ms_payment on r.slm_PaymentCodeNew5 equals pn5.slm_PaymentCode into r8
                                   //from pn5 in r8.DefaultIfEmpty()
                               join mt in sdc.kkslm_ms_teamtelesales on (decimal)s.slm_TeamTelesales_Id equals mt.slm_TeamTelesales_Id into r9
                               from mt in r9.DefaultIfEmpty()
                               from addr in sdc.kkslm_tr_renewinsurance_address.Where(p => p.slm_RenewInsureId == r.slm_RenewInsureId && p.slm_AddressType == "D").DefaultIfEmpty()
                               //join addr in sdc.kkslm_tr_renewinsurance_address on r.slm_RenewInsureId equals addr.slm_RenewInsureId into r10
                               //from addr in r10.DefaultIfEmpty()
                               join tm in sdc.kkslm_ms_tambol on addr.slm_Tambon equals tm.slm_TambolId into r11
                               from tm in r11.DefaultIfEmpty()
                               join am in sdc.kkslm_ms_amphur on addr.slm_Amphur equals am.slm_AmphurId into r12
                               from am in r12.DefaultIfEmpty()
                               join pv in sdc.kkslm_ms_province on addr.slm_Province equals pv.slm_ProvinceId into r13
                               from pv in r13.DefaultIfEmpty()
                               where (r.slm_Export_Flag == false || r.slm_Export_Flag == null) //&& addr.slm_AddressType == "D"
                               orderby s.slm_StaffNameTH
                               select new
                               {
                                   r.slm_AmendmentBillId,
                                   r.slm_UpdatedDate,
                                   t.slm_TitleName,
                                   r.slm_Name,
                                   r.slm_LastName,
                                   r.slm_ReceiptNo,
                                   r.slm_CarLicenseNo,
                                   r.slm_ReceiptDate,
                                   DescOld = r.slm_PaymentCodeOld1,
                                   r.slm_PaymentAmtOld1,
                                   DescNew1 = r.slm_PaymentCodeNew1,
                                   r.slm_PaymentAmtNew1,
                                   DescNew2 = r.slm_PaymentCodeNew2,
                                   r.slm_PaymentAmtNew2,
                                   DescNew3 = r.slm_PaymentCodeNew3,
                                   r.slm_PaymentAmtNew3,
                                   DescNew4 = r.slm_PaymentCodeNew4,
                                   r.slm_PaymentAmtNew4,
                                   DescNew5 = r.slm_PaymentCodeNew5,
                                   r.slm_PaymentAmtNew5,
                                   DescNew6 = r.slm_PaymentCodeNew6,
                                   r.slm_PaymentAmtNew6,
                                   r.slm_PaymentAmtTotal,
                                   s.slm_StaffNameTH,
                                   mt.slm_TeamTelesales_Name,
                                   r.slm_Owner,
                                   r.slm_Contract_Number,
                                   addr.slm_House_No,
                                   addr.slm_Moo,
                                   addr.slm_BuildingName,
                                   addr.slm_Village,
                                   addr.slm_Soi,
                                   addr.slm_Street,
                                   tm.slm_TambolNameTH,
                                   am.slm_AmphurNameTH,
                                   pv.slm_ProvinceNameTH,
                                   addr.slm_PostalCode
                               };
                    List<object[]> dataLst = new List<object[]>();
                    int i = 1;
                    foreach (var itm in data)
                    {
                        List<ReportData010> list = new List<ReportData010>();
                        if (itm.DescNew1 != null && itm.DescNew1.Trim() != "")
                            list.Add(new ReportData010 { DescNew = itm.DescNew1, PaymentAmountNew = itm.slm_PaymentAmtNew1 });
                        if (itm.DescNew2 != null && itm.DescNew2.Trim() != "")
                            list.Add(new ReportData010 { DescNew = itm.DescNew2, PaymentAmountNew = itm.slm_PaymentAmtNew2 });
                        if (itm.DescNew3 != null && itm.DescNew3.Trim() != "")
                            list.Add(new ReportData010 { DescNew = itm.DescNew3, PaymentAmountNew = itm.slm_PaymentAmtNew3 });
                        if (itm.DescNew4 != null && itm.DescNew4.Trim() != "")
                            list.Add(new ReportData010 { DescNew = itm.DescNew4, PaymentAmountNew = itm.slm_PaymentAmtNew4 });
                        if (itm.DescNew5 != null && itm.DescNew5.Trim() != "")
                            list.Add(new ReportData010 { DescNew = itm.DescNew5, PaymentAmountNew = itm.slm_PaymentAmtNew5 });
                        if (itm.DescNew6 != null && itm.DescNew6.Trim() != "")
                            list.Add(new ReportData010 { DescNew = itm.DescNew6, PaymentAmountNew = itm.slm_PaymentAmtNew6 });

                        string d1 = "";
                        string d2 = "";
                        string d3 = "";
                        string d4 = "";
                        string d5 = "";
                        string d6 = "";

                        decimal? a1 = null;
                        decimal? a2 = null;
                        decimal? a3 = null;
                        decimal? a4 = null;
                        decimal? a5 = null;
                        decimal? a6 = null;

                        for (int j = 0; j < list.Count; j++)
                        {
                            if (j == 0)
                            {
                                d1 = list[j].DescNew;
                                a1 = list[j].PaymentAmountNew;
                            }
                            else if (j == 1)
                            {
                                d2 = list[j].DescNew;
                                a2 = list[j].PaymentAmountNew;
                            }
                            else if (j == 2)
                            {
                                d3 = list[j].DescNew;
                                a3 = list[j].PaymentAmountNew;
                            }
                            else if (j == 3)
                            {
                                d4 = list[j].DescNew;
                                a4 = list[j].PaymentAmountNew;
                            }
                            else if (j == 4)
                            {
                                d5 = list[j].DescNew;
                                a5 = list[j].PaymentAmountNew;
                            }
                            else if (j == 5)
                            {
                                d6 = list[j].DescNew;
                                a6 = list[j].PaymentAmountNew;
                            }
                        }

                        var obj = new object[]
                        {
                            itm.slm_UpdatedDate,
                            itm.slm_TitleName + " " + itm.slm_Name + " " + itm.slm_LastName,
                            itm.slm_Contract_Number,
                            itm.slm_CarLicenseNo,
                            itm.slm_ReceiptNo,
                            itm.slm_ReceiptDate,
                            itm.DescOld,
                            itm.slm_PaymentAmtOld1,
                            d1,        //itm.DescNew1,
                            a1,        //itm.slm_PaymentAmtNew1,
                            d2,        //itm.DescNew2,
                            a2,        //itm.slm_PaymentAmtNew2,
                            d3,        //itm.DescNew3,
                            a3,        //itm.slm_PaymentAmtNew3,
                            d4,        //itm.DescNew4,
                            a4,        //itm.slm_PaymentAmtNew4,
                            d5,        //itm.DescNew5,
                            a5,        //itm.slm_PaymentAmtNew5,
                            d6,        //itm.DescNew6,
                            a6,        //tm.slm_PaymentAmtNew6,
                            itm.slm_PaymentAmtTotal,
                            itm.slm_StaffNameTH,
                            itm.slm_TeamTelesales_Name,
                            itm.slm_Owner,
                            itm.slm_House_No
                                + (String.IsNullOrEmpty(itm.slm_Moo) ? "" : " หมู่ " + itm.slm_Moo)
                                + (String.IsNullOrEmpty(itm.slm_BuildingName) ? "" : " อาคาร " + itm.slm_BuildingName)
                                + (String.IsNullOrEmpty(itm.slm_Village) ? "" : " หมู่บ้าน " + itm.slm_Village)
                                + (String.IsNullOrEmpty(itm.slm_Soi) ? "" : " ซอย " + itm.slm_Soi)
                                + (String.IsNullOrEmpty(itm.slm_Street) ? "" : " ถนน " + itm.slm_Street)
                                + (String.IsNullOrEmpty(itm.slm_TambolNameTH) ? "" : " ตำบล " + itm.slm_TambolNameTH)
                                + (String.IsNullOrEmpty(itm.slm_AmphurNameTH) ? "" : " อำเภอ " + itm.slm_AmphurNameTH)
                                + (String.IsNullOrEmpty(itm.slm_ProvinceNameTH) ? "" : " จังหวัด " + itm.slm_ProvinceNameTH)
                                + (String.IsNullOrEmpty(itm.slm_PostalCode) ? "" : " " + itm.slm_PostalCode)
                        };
                        dataLst.Add(obj);
                        i++;
                        succ++;

                        // update flag
                        var rpt = sdc.kkslm_tr_amendmentbill_report.Where(r => r.slm_AmendmentBillId == itm.slm_AmendmentBillId).FirstOrDefault();
                        if (rpt != null)
                        {
                            rpt.slm_Export_Flag = true;
                            rpt.slm_Export_Date = DateTime.Now;
                            rpt.slm_UpdatedBy = AppName;
                            rpt.slm_UpdatedDate = DateTime.Now;
                        }
                    }

                    if (dataLst.Count == 0) status = "No data to generate";
                    else status = string.Format("Generated {0} files total {1} records", 1, dataLst.Count);
                    writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                    DateTime endtime = DateTime.Now;

                    // add errorlog
                    foreach (var itm in errLst)
                        sdc.kkslm_tr_batchlog.AddObject(itm);

                    // update batch
                    batch.slm_EndTime = endtime;
                    batch.slm_Status = (fail > 0 ? "2" : "1");


                    // insert batch monitor 
                    kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                    bm.slm_DateStart = starttime;
                    bm.slm_DateEnd = endtime;
                    bm.slm_BatchCode = batchCode;
                    bm.slm_Total = data.Count();
                    bm.slm_Success = succ;
                    bm.slm_Fail = fail;
                    bm.slm_ActionBy = AppName;
                    bm.slm_BatchDate = dt;
                    sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                    sdc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool GenReport018(DateTime dt, out string status, int rowperpage, string reportPath)
        {
            status = "";
            string batchCode = "OBT_PRO_23";
            string folder = "";

            if (!string.IsNullOrEmpty(reportPath))
                folder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
            else
                folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            int succ = 0;
            int fail = 0;
            int total = 0;
            List<kkslm_tr_batchlog> errLst = new List<kkslm_tr_batchlog>();

            DateTime starttime = DateTime.Now;
            DateTime curdate = DateTime.Now.Date;
            DateTime tomorow = curdate.AddDays(1);

            bool ret = true;
            try
            {
                // get headers
                List<string> strLst = new List<string>() {"เลขที่รับแจ้ง", "รายชื่อบริษัทประกัน", "วันที่รับแจ้ง", "เลขที่สัญญาเช่าซื้อ","คำนำหน้าชื่อ"
                    , "ชื่อผู้เอาประกัน", "นามสกุลผู้เอาประกัน", "วันที่เริ่มคุ้มครอง", "วันสิ้นสุดคุ้มครอง", "เลขทะเบียน"
                    , "เจ้าหน้าที่ MKT"
                    };
                List<string> typLst = new List<string>() { "c", "c", "c", "c", "c"
                    , "c", "c", "c", "c", "c"
                    , "c"};

                using (OPERDBEntities odc = new OPERDBEntities())
                {
                    using (SLM_DBEntities sdc = new SLM_DBEntities())
                    {
                        var batch = sdc.kkslm_ms_batch.Where(b => b.slm_BatchCode == batchCode).FirstOrDefault();
                        if (batch != null)
                        {
                            batch.slm_StartTime = starttime;
                            batch.slm_Status = "0";
                            sdc.SaveChanges();
                        }

                        int ttf = 0;
                        int ttr = 0;
                        foreach (var com in odc.kkslm_ms_ins_com)
                        {

                            string subfolder = folder + "\\" + com.slm_InsABB;
                            if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);

                            string filename = subfolder + "\\" + "ยกเลิกระงับเคลม_" + com.slm_InsABB + "_" + dt.ToString("yyyyMMdd_HHmm") + ".xls";
                            var data = from pr in sdc.kkslm_tr_setle_claim_cancel_report
                                       join title in sdc.kkslm_ms_title on pr.slm_Title_Id equals title.slm_TitleId into r1
                                       from title in r1.DefaultIfEmpty()
                                       join s in sdc.kkslm_ms_staff.Where(s => s.slm_EmpCode != null) on pr.slm_Owner equals s.slm_EmpCode /*sdc.kkslm_ms_staff on pr.slm_Owner equals s.slm_UserName*/ into r2
                                       from s in r2.DefaultIfEmpty()
                                       join su in sdc.kkslm_ms_staff on pr.slm_Owner equals su.slm_UserName into r3
                                       from su in r3.DefaultIfEmpty()

                                       orderby pr.slm_UpdatedDate
                                       where pr.slm_Ins_Com_Id == com.slm_Ins_Com_Id && (pr.slm_Export_Flag == false || pr.slm_Export_Flag == null)
                                       select new
                                       {
                                           pr.slm_Id,
                                           pr.slm_InsReceiveNo,
                                           com.slm_InsNameTh,
                                           pr.slm_InsReceiveDate,
                                           pr.slm_Contract_Number,
                                           title.slm_TitleName,
                                           pr.slm_Name,
                                           pr.slm_LastName,
                                           pr.slm_CoverageStartDate,
                                           pr.slm_CoverageEndDate,
                                           pr.slm_CarLicenseNo,
                                           s.slm_StaffNameTH,
                                           StaffNameTHByUserName = su.slm_StaffNameTH
                                       };
                            try
                            {
                                List<object[]> dataLst = new List<object[]>();
                                int i = 1;
                                foreach (var itm in data)
                                {
                                    var obj = new object[]
                                    {
                                        itm.slm_InsReceiveNo,
                                        itm.slm_InsNameTh,
                                        itm.slm_InsReceiveDate == null ? "" : itm.slm_InsReceiveDate.Value.ToString("dd/MM/yyyy HH:mm"),
                                        itm.slm_Contract_Number,
                                        itm.slm_TitleName,
                                        itm.slm_Name,
                                        itm.slm_LastName,
                                        itm.slm_CoverageStartDate == null ? "" : itm.slm_CoverageStartDate.Value.ToString("dd/MM/yyyy"),
                                        itm.slm_CoverageEndDate == null ? "" : itm.slm_CoverageEndDate.Value.ToString("dd/MM/yyyy"),
                                        itm.slm_CarLicenseNo,
                                        itm.slm_StaffNameTH == null ? itm.StaffNameTHByUserName : itm.slm_StaffNameTH
                                    };
                                    dataLst.Add(obj);
                                    i++;
                                    succ++;

                                    // update flag
                                    var rpt = sdc.kkslm_tr_setle_claim_cancel_report.Where(r => r.slm_Id == itm.slm_Id).FirstOrDefault();
                                    if (rpt != null)
                                    {
                                        rpt.slm_Export_Flag = true;
                                        rpt.slm_Export_Date = DateTime.Now;
                                        rpt.slm_UpdatedBy = AppName;
                                        rpt.slm_UpdatedDate = DateTime.Now;
                                    }

                                }
                                sdc.SaveChanges();
                                if (dataLst.Count > 0) ttf++;
                                ttr += dataLst.Count;
                                writeExcel(filename, strLst, typLst, dataLst, rowperpage);
                            }
                            catch (Exception ex)
                            {
                                foreach (var itm in data)
                                {
                                    errLst.Add(new kkslm_tr_batchlog()
                                    {
                                        slm_ErrorDetail = ex.Message
                                    });
                                }
                            }

                        }

                        if (ttf == 0) status = "No data to generate";
                        else status = string.Format("Generated {0} files total {1} records", ttf, ttr);
                        DateTime endtime = DateTime.Now;

                        // update batch
                        batch.slm_EndTime = endtime;
                        batch.slm_Status = (errLst.Count > 0 ? "2" : "1");


                        // insert batch monitor 
                        kkslm_tr_batchmonitor_overview bm = new kkslm_tr_batchmonitor_overview();
                        bm.slm_DateStart = starttime;
                        bm.slm_DateEnd = endtime;
                        bm.slm_BatchCode = batchCode;
                        bm.slm_Total = total;
                        bm.slm_Success = succ;
                        bm.slm_Fail = fail;
                        bm.slm_ActionBy = AppName;
                        bm.slm_BatchDate = dt;
                        sdc.kkslm_tr_batchmonitor_overview.AddObject(bm);

                        sdc.SaveChanges();

                        // add errorlog
                        foreach (var itm in errLst)
                        {
                            itm.slm_BatchMonitor_Id = bm.slm_BatchMonitor_Id;
                            sdc.kkslm_tr_batchlog.AddObject(itm);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="clmLst"></param>
        /// <param name="typLst"></param>
        /// <param name="dataLst"></param>
        /// <param name="rowperpage"></param>
        /// <returns></returns>
        public bool writeExcel(string filename, List<string> clmLst, List<string> typLst, List<object[]> dataLst, int rowperpage)
        {
            ILog _log = LogManager.GetLogger(typeof(OBTBatchBiz));
            _error = "";
            bool ret = true;
            try
            {
                if (dataLst.Count > 0)
                {
                    using (OleDbConnection objConn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";Extended Properties='Excel 8.0;Mode=ReadWrite;ReadOnly=false;HDR=YES;MaxScanRows=0;IMEX=0'"))
                    {
                        using (OleDbCommand objCmd = new System.Data.OleDb.OleDbCommand())
                        {

                            objConn.Open();
                            objCmd.Connection = objConn;
                            objCmd.CommandType = System.Data.CommandType.Text;


                            int page = 1;
                            int start = 0;
                            int lastrow = 0;
                            if (rowperpage == 0) rowperpage = 65534;

                            while (lastrow < dataLst.Count)
                            {
                                // create table
                                string sql = "create table [Sheet" + page.ToString() + "] ( ";
                                for (int i = 0; i < clmLst.Count; i++)
                                {
                                    if (i > 0) sql += ",";
                                    sql += "[" + clmLst[i].Replace(".", "") + "] ";
                                    switch (typLst[i])
                                    {
                                        case "n":
                                            sql += "numeric(18,2)";
                                            break;

                                        case "d":
                                            sql += "date";
                                            break;

                                        default:
                                            sql += "longtext";
                                            break;

                                    } //+ (typLst[i] == "n" ? "numeric(18,2)" : typelst[i] ==  "char");
                                }
                                sql += ")";
                                objCmd.CommandText = sql;
                                objCmd.ExecuteNonQuery();

                                lastrow += rowperpage;
                                if (lastrow > dataLst.Count) lastrow = dataLst.Count;

                                // insert data
                                _log.DebugFormat("{0} ข้อมูลรายงาน excel :", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                for (int r = start; r < lastrow; r++)
                                {
                                    var lst = dataLst[r];
                                    sql = "insert into [Sheet" + page.ToString() + "$] values (";
                                    for (int i = 0; i < lst.Length; i++)
                                    {
                                        if (i > 0) sql += ",";
                                        if (lst[i] == null || lst[i].ToString().Trim() == "")
                                            sql += "null";
                                        else
                                            sql += "'" + lst[i].ToString().Replace("'", "''") + "'";

                                        _log.DebugFormat("\n data{1} = {0} ", lst[i], i);

                                    }
                                    sql += ")";
                                    objCmd.CommandText = sql;
                                    objCmd.ExecuteNonQuery();
                                }
                                _log.Debug("\n ===============================================");

                                start += rowperpage;
                                page++;
                            }

                            objConn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //ret = false;
                //_error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;

        }

        public bool DeleteEmptyFolder(DateTime dt, string reportPath)
        {
            bool ret = true;
            try
            {
                string rootFolder = "";

                if (!string.IsNullOrEmpty(reportPath))
                    rootFolder = Path.Combine(reportPath, dt.ToString("yyyyMMdd"));
                else
                    rootFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + dt.ToString("yyyyMMdd");

                RecursiveDeleteEmptyFolder(rootFolder);
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }

        private void RecursiveDeleteEmptyFolder(string directoryPath)
        {
            var directories = Directory.GetDirectories(directoryPath);
            foreach (string path in directories)
            {
                RecursiveDeleteEmptyFolder(path);
                if (Directory.GetDirectories(path).Length == 0 && Directory.GetFiles(path).Length == 0)
                    Directory.Delete(path, false);
            }
        }

        public class ReportData010
        {
            public string DescNew { get; set; }
            public decimal? PaymentAmountNew { get; set; }
        }
    }
}
