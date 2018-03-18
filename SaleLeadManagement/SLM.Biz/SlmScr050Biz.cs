using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Resource;
using SLM.Resource.Data;


namespace SLM.Biz
{
    public class SlmScr050Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        public List<SP_RPT_006_FUEL_COMMUTATION_PAYMENT_Result> GetDataForScreen(DateTime incFrom, DateTime incTo, DateTime coverFrom, DateTime coverTo, string Telesale, string Staff, string Category, string IncentiveFlag)
        {
            using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
            {
                string sqlz = @"
SELECT ST.slm_StaffNameTH AS StaffName, ren.slm_ContractNo as ContractNo , TT.SLM_TITLENAME as TitleName, LEAD.SLM_NAME as Name, LEAD.SLM_LASTNAME as LastName,
	REN.SLM_LICENSENO as LicenseNo, INS.slm_InsNameTh AS InsName, COV.slm_ConverageTypeName as CoverageTypeName
	, REN.slm_PolicyGrossPremiumTotal as PolicyGrossTotal, REN.slm_PolicyGrossPremium as PolicyGross
FROM KKSLM_TR_LEAD LEAD INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId =REN.slm_TicketId
	INNER JOIN KKSLM_MS_STAFF ST ON ST.SLM_USERNAME  = LEAD.SLM_OWNER
	LEFT JOIN kkslm_ms_title TT ON TT.SLM_TITLEID = LEAD.SLM_TITLEID
	LEFT JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com INS ON INS.slm_Ins_Com_Id = REN.slm_InsuranceComId
	LEFT JOIN kkslm_ms_coveragetype as COV ON COV.slm_CoverageTypeId = ren.slm_CoverageTypeId
WHERE REN.slm_IncentiveDate IS NOT NULL AND LEAD.is_Deleted = 0 
";

                if (incFrom.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                if (incTo.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                if (coverFrom.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate >= '{0}' ", SLMUtil.GetDateString(coverFrom));
                if (coverTo.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate < '{0}' ", SLMUtil.GetDateString(coverTo.AddDays(1)));
                if (Telesale.Trim() != "") sqlz += string.Format(" AND ST.slm_TeamTelesales_Id = {0} ", SLMUtil.SafeDecimal(Telesale));
                //if (Category.Trim() != "")
                if (IncentiveFlag.Trim() == "1") sqlz += " AND REN.slm_IncentiveFlag = 1 ";
                if (IncentiveFlag.Trim() == "2") sqlz += " AND ( REN.slm_IncentiveFlag is null OR REN.slm_IncentiveFlag = 0) ";

                sqlz += " ORDER BY ST.slm_StaffNameTH, ren.slm_CoverageTypeId desc ";
                var obj = sdc.ExecuteStoreQuery<SLM050Data>(sqlz).ToList();

                var lst = sdc.SP_RPT_006_FUEL_COMMUTATION_PAYMENT(incFrom, incTo, coverFrom.Year==  1 ? null : (DateTime?)coverFrom, coverTo.Year == 1 ? null : (DateTime?)coverTo, Telesale, Staff, SLMUtil.SafeInt(Category), SLMUtil.SafeInt(IncentiveFlag)).ToList();

                return lst;
            }
        }

        public bool CreateExcel1(string filename, DateTime incFrom, DateTime incTo, DateTime coverFrom, DateTime coverTo, string Telesale, string Staff, string Category, string IncentiveFlag)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    string sqlz = @"
SELECT REN.slm_Grade,BH.slm_BranchName,	ST.slm_StaffNameTH AS StaffName, ren.slm_ContractNo , TT.slm_TitleName, LEAD.slm_Name,
	LEAD.slm_LastName,REN.slm_LicenseNo, INS.slm_InsNameTh AS ins_Name, COV.slm_ConverageTypeName , 
	REN.slm_PolicyGrossPremiumTotal,REN.slm_PolicyGrossPremium
FROM KKSLM_TR_LEAD LEAD LEFT JOIN kkslm_tr_prelead PRE ON PRE.slm_TicketId = LEAD.slm_TicketId
	INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId =REN.slm_TicketId
	INNER JOIN KKSLM_MS_STAFF ST ON ST.SLM_USERNAME  = LEAD.SLM_OWNER
	LEFT JOIN kkslm_ms_title TT ON TT.SLM_TITLEID = LEAD.SLM_TITLEID
	LEFT JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com INS ON INS.slm_Ins_Com_Id = REN.slm_InsuranceComId
	LEFT JOIN kkslm_ms_coveragetype as COV ON COV.slm_CoverageTypeId = ren.slm_CoverageTypeId
	LEFT JOIN KKSLM_MS_BRANCH BH ON BH.slm_BranchCode = PRE.slm_BranchCode
WHERE REN.slm_IncentiveDate IS NOT NULL AND LEAD.is_Deleted = 0 
";

                    if (incFrom.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                    if (incTo.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                    if (coverFrom.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate >= '{0}' ", SLMUtil.GetDateString(coverFrom));
                    if (coverTo.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate < '{0}' ", SLMUtil.GetDateString(coverTo.AddDays(1)));
                    if (Telesale.Trim() != "") sqlz += string.Format(" AND ST.slm_TeamTelesales_Id = {0} ", SLMUtil.SafeDecimal(Telesale));
                    //if (Category.Trim() != "")
                    if (IncentiveFlag.Trim() == "1") sqlz += " AND REN.slm_IncentiveFlag = 1 ";
                    if (IncentiveFlag.Trim() == "2") sqlz += " AND ( REN.slm_IncentiveFlag is null OR REN.slm_IncentiveFlag = 0) ";

                    sqlz += " ORDER BY ST.slm_StaffNameTH ASC, COV.slm_ConverageTypeName ASC ";
                    var lst = sdc.ExecuteStoreQuery<SLM050Excel1>(sqlz).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[]
                        {
                            itm.StaffName,
                            itm.slm_Grade,
                            itm.slm_ContractNo,
                            itm.slm_TitleName,
                            itm.slm_Name,
                            itm.slm_LastName,
                            itm.slm_BranchName,
                            itm.slm_LicenseNo,
                            itm.ins_Name,
                            itm.slm_ConverageTypeName,
                            itm.slm_PolicyGrossPremiumTotal,
                            itm.slm_PolicyGrossPremium
                        });
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "รายละเอียดประกัน",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ชื่อเจ้าหน้าที่ MKT", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เกรดลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เลขที่สัญญาเช่าซื้อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "คำนำหน้าชื่อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ชื่อผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "นามสกุลผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "สาขา", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เลขทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "รายชื่อบริษัทประกันภัย", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ประเภทความคุ้มครอง", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบี้ยประกันภัย (เบี้ยเต็ม)", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ค่าเบี้ยหลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Number }

                               }
                           }
                    })) throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool CreateExcel1V2(string filename, DateTime incFrom, DateTime incTo, DateTime coverFrom, DateTime coverTo, string Telesale, string Staff, string Category, string IncentiveFlag)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    //                    string sqlz = @"
                    //SELECT REN.slm_Grade,BH.slm_BranchName,	ST.slm_StaffNameTH AS StaffName, ren.slm_ContractNo , TT.slm_TitleName, LEAD.slm_Name,
                    //	LEAD.slm_LastName,REN.slm_LicenseNo, INS.slm_InsNameTh AS ins_Name, COV.slm_ConverageTypeName , 
                    //	REN.slm_PolicyGrossPremiumTotal,REN.slm_PolicyGrossPremium
                    //FROM KKSLM_TR_LEAD LEAD LEFT JOIN kkslm_tr_prelead PRE ON PRE.slm_TicketId = LEAD.slm_TicketId
                    //	INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId =REN.slm_TicketId
                    //	INNER JOIN KKSLM_MS_STAFF ST ON ST.SLM_USERNAME  = LEAD.SLM_OWNER
                    //	LEFT JOIN kkslm_ms_title TT ON TT.SLM_TITLEID = LEAD.SLM_TITLEID
                    //	LEFT JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com INS ON INS.slm_Ins_Com_Id = REN.slm_InsuranceComId
                    //	LEFT JOIN kkslm_ms_coveragetype as COV ON COV.slm_CoverageTypeId = ren.slm_CoverageTypeId
                    //	LEFT JOIN KKSLM_MS_BRANCH BH ON BH.slm_BranchCode = PRE.slm_BranchCode
                    //WHERE REN.slm_IncentiveDate IS NOT NULL AND LEAD.is_Deleted = 0 
                    //";

                    //                    if (incFrom.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                    //                    if (incTo.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                    //                    if (coverFrom.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate >= '{0}' ", SLMUtil.GetDateString(coverFrom));
                    //                    if (coverTo.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate < '{0}' ", SLMUtil.GetDateString(coverTo.AddDays(1)));
                    //                    if (Telesale.Trim() != "") sqlz += string.Format(" AND ST.slm_TeamTelesales_Id = {0} ", SLMUtil.SafeDecimal(Telesale));
                    //                    //if (Category.Trim() != "")
                    //                    if (IncentiveFlag.Trim() == "1") sqlz += " AND REN.slm_IncentiveFlag = 1 ";
                    //                    if (IncentiveFlag.Trim() == "2") sqlz += " AND ( REN.slm_IncentiveFlag is null OR REN.slm_IncentiveFlag = 0) ";

                    //                    sqlz += " ORDER BY ST.slm_StaffNameTH ASC, COV.slm_ConverageTypeName ASC ";
                    //var lst = sdc.ExecuteStoreQuery<SLM050Excel1>(sqlz).ToList();

                    var lst = sdc.SP_RPT_006_31_FUEL_COMMUNICATION_POLICY_DETAIL(incFrom, incTo, coverFrom.Year == 1 ? null : (DateTime?)coverFrom, coverTo.Year == 1 ? null : (DateTime?)coverTo, Telesale, Staff, SLMUtil.SafeInt(Category), SLMUtil.SafeInt(IncentiveFlag)).OrderBy(s=>s.StaffName).ThenBy(s=>s.slm_ConverageTypeName).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[]
                        {
                            itm.StaffName,
                            itm.slm_Grade,
                            "=\""+itm.slm_ContractNo+"\"",
                            itm.slm_TitleName,
                            itm.slm_Name,
                            itm.slm_LastName,
                            itm.slm_BranchName,
                            itm.slm_LicenseNo,
                            itm.ins_Name,
                            itm.slm_ConverageTypeName,
                            "=\""+DecimalFormat(itm.slm_PolicyGrossPremiumTotal)+"\"",
                            "=\""+DecimalFormat(itm.slm_PolicyGrossPremium)+"\"",
                            "=\""+DecimalFormat(itm.PolicyPaid)+"\"",
                            itm.LatestHPTransdate,
                            itm.slm_IncentiveDate
                        });
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateCSV(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "รายละเอียดประกัน",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "Telesales", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เกรดลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เลขที่สัญญาเช่าซื้อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "คำนำหน้าชื่อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ชื่อผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "นามสกุลผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "สาขา", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เลขทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "รายชื่อบริษัทประกันภัย", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ประเภทความคุ้มครอง", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบี้ยประกันภัย (เบี้ยเต็ม)", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ค่าเบี้ยหลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ค่าเบี้ยที่ชำระ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "วันที่ลูกค้าจ่าย", ColumnDataType = ExcelExportBiz.ColumnType.DateTime },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "วันที่กด Incentive (ประกันภัย)", ColumnDataType = ExcelExportBiz.ColumnType.DateTime },

                               }
                           }
                    })) throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool CreateExcel2(string filename, DateTime incFrom, DateTime incTo, DateTime coverFrom, DateTime coverTo, string Telesale, string Staff, string Category, string IncentiveFlag)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    string sqlz = @"
SELECT PRE.slm_Grade,BH.slm_BranchName,	ST.slm_StaffNameTH AS StaffName, ren.slm_ContractNo , TT.slm_TitleName, LEAD.slm_Name,
	LEAD.slm_LastName,REN.slm_LicenseNo, INS.slm_InsNameTh AS ins_Name, COV.slm_ConverageTypeName , 
	REN.slm_ActNetPremium as slm_PolicyGrossPremiumTotal,REN.slm_ActGrossPremium as slm_PolicyGrossPremium --, REN.slm_ActNetPremium, REN.slm_ActGrossPremium
FROM KKSLM_TR_LEAD LEAD LEFT JOIN kkslm_tr_prelead PRE ON PRE.slm_TicketId = LEAD.slm_TicketId
	INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId =REN.slm_TicketId
	INNER JOIN KKSLM_MS_STAFF ST ON ST.SLM_USERNAME  = LEAD.SLM_OWNER
	LEFT JOIN kkslm_ms_title TT ON TT.SLM_TITLEID = LEAD.SLM_TITLEID
	LEFT JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com INS ON INS.slm_Ins_Com_Id = REN.slm_InsuranceComId
	LEFT JOIN kkslm_ms_coveragetype as COV ON COV.slm_CoverageTypeId = ren.slm_CoverageTypeId
	LEFT JOIN KKSLM_MS_BRANCH BH ON BH.slm_BranchCode = PRE.slm_BranchCode
WHERE REN.slm_IncentiveDate IS NOT NULL and slm_ActNetPremium > 0 AND LEAD.is_Deleted = 0 
";

                    if (incFrom.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                    if (incTo.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                    if (coverFrom.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate >= '{0}' ", SLMUtil.GetDateString(coverFrom));
                    if (coverTo.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate < '{0}' ", SLMUtil.GetDateString(coverTo.AddDays(1)));
                    if (Telesale.Trim() != "") sqlz += string.Format(" AND ST.slm_TeamTelesales_Id = {0} ", SLMUtil.SafeDecimal(Telesale));
                    //if (Category.Trim() != "")
                    if (IncentiveFlag.Trim() == "1") sqlz += " AND REN.slm_IncentiveFlag = 1 ";
                    if (IncentiveFlag.Trim() == "2") sqlz += " AND ( REN.slm_IncentiveFlag is null OR REN.slm_IncentiveFlag = 0) ";

                    sqlz += " ORDER BY ST.slm_StaffNameTH ASC, COV.slm_ConverageTypeName ASC ";
                    var lst = sdc.ExecuteStoreQuery<SLM050Excel1>(sqlz).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[]
                        {
                            itm.StaffName,
                            itm.slm_Grade,
                            itm.slm_ContractNo,
                            itm.slm_TitleName,
                            itm.slm_Name,
                            itm.slm_LastName,
                            itm.slm_BranchName,
                            itm.slm_LicenseNo,
                            itm.ins_Name,
                            itm.slm_ConverageTypeName,
                            itm.slm_PolicyGrossPremiumTotal,
                            itm.slm_PolicyGrossPremium
                        });
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "รายละเอียดประกัน",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ชื่อเจ้าหน้าที่ MKT", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เกรดลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เลขที่สัญญาเช่าซื้อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "คำนำหน้าชื่อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ชื่อผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "นามสกุลผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "สาขา", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เลขทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "รายชื่อบริษัทประกันภัย", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ประเภทความคุ้มครอง", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบี้ยประกันภัย (เบี้ยเต็ม)", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ค่าเบี้ยหลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Number }

                               }
                           }
                    })) throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool CreateExcel2V2(string filename, DateTime incFrom, DateTime incTo, DateTime coverFrom, DateTime coverTo, string Telesale, string Staff, string Category, string IncentiveFlag)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    //                    string sqlz = @"
                    //SELECT PRE.slm_Grade,BH.slm_BranchName,	ST.slm_StaffNameTH AS StaffName, ren.slm_ContractNo , TT.slm_TitleName, LEAD.slm_Name,
                    //	LEAD.slm_LastName,REN.slm_LicenseNo, INS.slm_InsNameTh AS ins_Name, COV.slm_ConverageTypeName , 
                    //	REN.slm_ActNetPremium as slm_PolicyGrossPremiumTotal,REN.slm_ActGrossPremium as slm_PolicyGrossPremium --, REN.slm_ActNetPremium, REN.slm_ActGrossPremium
                    //FROM KKSLM_TR_LEAD LEAD LEFT JOIN kkslm_tr_prelead PRE ON PRE.slm_TicketId = LEAD.slm_TicketId
                    //	INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId =REN.slm_TicketId
                    //	INNER JOIN KKSLM_MS_STAFF ST ON ST.SLM_USERNAME  = LEAD.SLM_OWNER
                    //	LEFT JOIN kkslm_ms_title TT ON TT.SLM_TITLEID = LEAD.SLM_TITLEID
                    //	LEFT JOIN " + SLM.Resource.SLMConstant.OPERDBName + @".dbo.kkslm_ms_ins_com INS ON INS.slm_Ins_Com_Id = REN.slm_InsuranceComId
                    //	LEFT JOIN kkslm_ms_coveragetype as COV ON COV.slm_CoverageTypeId = ren.slm_CoverageTypeId
                    //	LEFT JOIN KKSLM_MS_BRANCH BH ON BH.slm_BranchCode = PRE.slm_BranchCode
                    //WHERE REN.slm_IncentiveDate IS NOT NULL and slm_ActNetPremium > 0 AND LEAD.is_Deleted = 0 
                    //";

                    //                    if (incFrom.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                    //                    if (incTo.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                    //                    if (coverFrom.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate >= '{0}' ", SLMUtil.GetDateString(coverFrom));
                    //                    if (coverTo.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate < '{0}' ", SLMUtil.GetDateString(coverTo.AddDays(1)));
                    //                    if (Telesale.Trim() != "") sqlz += string.Format(" AND ST.slm_TeamTelesales_Id = {0} ", SLMUtil.SafeDecimal(Telesale));
                    //                    //if (Category.Trim() != "")
                    //                    if (IncentiveFlag.Trim() == "1") sqlz += " AND REN.slm_IncentiveFlag = 1 ";
                    //                    if (IncentiveFlag.Trim() == "2") sqlz += " AND ( REN.slm_IncentiveFlag is null OR REN.slm_IncentiveFlag = 0) ";

                    //                    sqlz += " ORDER BY ST.slm_StaffNameTH ASC, COV.slm_ConverageTypeName ASC ";
                    //                    var lst = sdc.ExecuteStoreQuery<SLM050Excel1>(sqlz).ToList();

                    var lst = sdc.SP_RPT_006_32_FUEL_COMMUTATION_ACT_DETAIL(incFrom, incTo, coverFrom.Year == 1 ? null : (DateTime?)coverFrom, coverTo.Year == 1 ? null : (DateTime?)coverTo, Telesale, Staff, SLMUtil.SafeInt(Category), SLMUtil.SafeInt(IncentiveFlag)).OrderBy(s=>s.StaffName).ThenBy(s=>s.slm_Grade).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[]
                        {
                            itm.StaffName,
                            itm.slm_Grade,
                            "=\""+itm.slm_ContractNo+"\"",
                            itm.slm_TitleName,
                            itm.slm_Name,
                            itm.slm_LastName,
                            itm.slm_BranchName,
                            //itm.slm_LicenseNo,
                            itm.ins_Name,
//                            itm.slm_ConverageTypeName,
                            "=\""+DecimalFormat(itm.slm_PolicyGrossPremiumTotal)+"\"",
                            "=\""+DecimalFormat(itm.slm_PolicyGrossPremium)+"\"",
                            "=\""+DecimalFormat(itm.PolicyPaid)+"\"",
                            itm.LatestHPTransdate,
                            itm.slm_ActIncentiveDate
                        });
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateCSV(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "รายละเอียดประกัน",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "Telesales", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เกรดลูกค้า", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เลขที่สัญญาเช่าซื้อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "คำนำหน้าชื่อ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ชื่อผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "นามสกุลผู้เอาประกัน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "สาขา", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                  // new ExcelExportBiz.ColumnItem() { ColumnName = "เลขทะเบียน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "รายชื่อบริษัทพรบ.", ColumnDataType = ExcelExportBiz.ColumnType.Text },
//                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ประเภทความคุ้มครอง", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบี้ยพรบ. (เบี้ยเต็ม)", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ค่าพรบ.หลังหักส่วนลด", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ค่าพรบ.ที่ชำระ", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "วันที่ลูกค้าจ่าย", ColumnDataType = ExcelExportBiz.ColumnType.DateTime },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "วันที่กด Incentive (พรบ.)", ColumnDataType = ExcelExportBiz.ColumnType.DateTime },


                               }
                           }
                    })) throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool CreateExcel3(string filename, DateTime incFrom, DateTime incTo, DateTime coverFrom, DateTime coverTo, string Telesale, string Staff, string Category, string IncentiveFlag)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    string sqlz = @"
SELECT REN.slm_PolicyGrossPremiumTotal + slm_ActNetPremium as Total, ST.slm_StaffNameTH AS StaffName, month(ren.slm_IncentiveDate) as Incentive_Month
FROM KKSLM_TR_LEAD LEAD LEFT JOIN kkslm_tr_prelead PRE ON PRE.slm_TicketId = LEAD.slm_TicketId
	INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId =REN.slm_TicketId
	INNER JOIN KKSLM_MS_STAFF ST ON ST.SLM_USERNAME  = LEAD.SLM_OWNER
WHERE REN.slm_IncentiveDate IS NOT NULL and slm_ActNetPremium > 0 AND LEAD.is_Deleted = 0 
";

                    if (incFrom.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                    if (incTo.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                    if (coverFrom.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate >= '{0}' ", SLMUtil.GetDateString(coverFrom));
                    if (coverTo.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate < '{0}' ", SLMUtil.GetDateString(coverTo.AddDays(1)));
                    if (Telesale.Trim() != "") sqlz += string.Format(" AND ST.slm_TeamTelesales_Id = {0} ", SLMUtil.SafeDecimal(Telesale));
                    //if (Category.Trim() != "")
                    if (IncentiveFlag.Trim() == "1") sqlz += " AND REN.slm_IncentiveFlag = 1 ";
                    if (IncentiveFlag.Trim() == "2") sqlz += " AND ( REN.slm_IncentiveFlag is null OR REN.slm_IncentiveFlag = 0) ";

                    sqlz += " ORDER BY ST.slm_StaffNameTH ASC ";
                    var lst = sdc.ExecuteStoreQuery<SLM050Excel3>(sqlz).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[]
                        {
                            itm.StaffName,
                            itm.Total,
                            itm.Total,
                            "",
                            "",
                            "",
                            "",
                            ""
                        });
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "รายละเอียดประกัน",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ชื่อ-สกุล", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ค่าเบี้ยประกันภัยรวม พรบ.", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบิกปกติ ค่าเบี้ยประกันภัยรวม พรบ.", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบิกปกติ อัตราเบิก", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "BSAT จำนวนเงิน", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "คิดเป็นเงิน (เบิกปกติ)", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "คิดเป็นเงิน (เบิกตามเงื่อนไข)", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "คิดเป็นเงิน (รวมเบิก)", ColumnDataType = ExcelExportBiz.ColumnType.Text }

                               }
                           }
                    })) throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }
        public bool CreateExcel3V2(string filename, DateTime incFrom, DateTime incTo, DateTime coverFrom, DateTime coverTo, string Telesale, string Staff, string Category, string IncentiveFlag)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    //                    string sqlz = @"
                    //SELECT REN.slm_PolicyGrossPremiumTotal + slm_ActNetPremium as Total, ST.slm_StaffNameTH AS StaffName, month(ren.slm_IncentiveDate) as Incentive_Month
                    //FROM KKSLM_TR_LEAD LEAD LEFT JOIN kkslm_tr_prelead PRE ON PRE.slm_TicketId = LEAD.slm_TicketId
                    //	INNER JOIN kkslm_tr_renewinsurance REN ON LEAD.slm_ticketId =REN.slm_TicketId
                    //	INNER JOIN KKSLM_MS_STAFF ST ON ST.SLM_USERNAME  = LEAD.SLM_OWNER
                    //WHERE REN.slm_IncentiveDate IS NOT NULL and slm_ActNetPremium > 0 AND LEAD.is_Deleted = 0 
                    //";

                    //                    if (incFrom.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate >= '{0}' ", SLMUtil.GetDateString(incFrom));
                    //                    if (incTo.Year != 1) sqlz += string.Format(" AND REN.slm_IncentiveDate < '{0}' ", SLMUtil.GetDateString(incTo.AddDays(1)));
                    //                    if (coverFrom.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate >= '{0}' ", SLMUtil.GetDateString(coverFrom));
                    //                    if (coverTo.Year != 1) sqlz += string.Format(" AND REN.slm_PolicyStartCoverDate < '{0}' ", SLMUtil.GetDateString(coverTo.AddDays(1)));
                    //                    if (Telesale.Trim() != "") sqlz += string.Format(" AND ST.slm_TeamTelesales_Id = {0} ", SLMUtil.SafeDecimal(Telesale));
                    //                    //if (Category.Trim() != "")
                    //                    if (IncentiveFlag.Trim() == "1") sqlz += " AND REN.slm_IncentiveFlag = 1 ";
                    //                    if (IncentiveFlag.Trim() == "2") sqlz += " AND ( REN.slm_IncentiveFlag is null OR REN.slm_IncentiveFlag = 0) ";

                    //                    sqlz += " ORDER BY ST.slm_StaffNameTH ASC ";
                    //                    var lst = sdc.ExecuteStoreQuery<SLM050Excel3>(sqlz).ToList();

                    var lst = sdc.SP_RPT_006_33_FUEL_COMMUTATION_COVER_PAGE(incFrom, incTo, coverFrom.Year == 1 ? null : (DateTime?)coverFrom, coverTo.Year == 1 ? null : (DateTime?)coverTo, Telesale, Staff, SLMUtil.SafeInt(Category), SLMUtil.SafeInt(IncentiveFlag)).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[]
                        {
                            itm.StaffName,
                            "=\""+DecimalFormat(itm.Total)+"\"",
                            "=\""+DecimalFormat(itm.NormalPaybackPolicyAndAct)+"\"",
                            itm.NormalPaybackPolicyRatio,
                            itm.BSATAmount,
                            itm.NormalPaybackTotalAmount,
                            itm.ConditionedPaybackAmount,
                            itm.GrandTotalPaybackIncluded
                        });
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateCSV(filename, new List<ExcelExportBiz.SheetItem>()
                        {
                           new ExcelExportBiz.SheetItem()
                           {
                               SheetName = "รายละเอียดประกัน",
                               RowPerSheet = SLMConstant.ExcelRowPerSheet,
                               Data = data,
                               Columns = new List<ExcelExportBiz.ColumnItem>()
                               {
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ชื่อ-สกุล", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "ค่าเบี้ยประกันภัยรับรวมพรบ.", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบิกปกติ:เบี้ยประกันภัยรวมพรบ.", ColumnDataType = ExcelExportBiz.ColumnType.Text },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบิกปกติ:อัตราเบิก", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "BSAT:จำนวนเงิน", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบิกปกติ", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "เบิกตามเงื่อนไข", ColumnDataType = ExcelExportBiz.ColumnType.Number },
                                   new ExcelExportBiz.ColumnItem() { ColumnName = "รวมเบิก", ColumnDataType = ExcelExportBiz.ColumnType.Number }

                               }
                           }
                    })) throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return ret;
        }

        public partial class SLM050Data
        {
            public string StaffName { get; set; }
            public string ContractNo { get; set; }
            public string TitleName { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
            public string LicenseNo { get; set; }
            public string InsName { get; set; }
            public string CoverageTypeName { get; set; }
            public decimal? PolicyGrossTotal { get; set; }
            public decimal? PolicyGross { get; set; }
        }
        public partial class SLM050Excel1
        {
            public string slm_Grade { get; set; }
            public string slm_BranchName { get; set; }
            public string StaffName { get; set; }
            public string slm_ContractNo { get; set; }
            public string slm_TitleName { get; set; }
            public string slm_Name { get; set; }
            public string slm_LastName { get; set; }
            public string slm_LicenseNo { get; set; }
            public string ins_Name { get; set; }
            public string slm_ConverageTypeName { get; set; }
            public decimal slm_PolicyGrossPremiumTotal { get; set; }
            public decimal slm_PolicyGrossPremium { get; set; }
        }
        public partial class SLM050Excel3
        {
            public decimal Total { get; set; }
            public int Incentive_Month { get; set; }
            public string StaffName { get; set; }
        }

        public static List<ControlListData> GetGradeDataList()
        {
            using (SLM_DBEntities sdc = new SLM_DBEntities())
            {
                return sdc.kkslm_ms_customer_grade.Where(g => g.is_Deleted == false).ToList().Select(g => new ControlListData() { TextField = g.slm_Customer_Grade_Name, ValueField = g.slm_Customer_Grade_Id.ToString() }).ToList();
            }
        }

        public string DecimalFormat(decimal? val)
        {
            if (val == null) return "";
            else return val.Value.ToString("#,##0.00");
        }
    }
}
