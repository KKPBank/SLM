using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.OleDb;
using SLM.Biz;
using SLM.Application.Utilities;
using SLM.Resource;
using SLM.Resource.Data;
using log4net;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SLM.Application
{
    public partial class SLM_SCR_040 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_040));
        private const char A1 = 'A';

        private enum ExcelColumnName
        {
            RunNo = A1,
            PolicyNo,
            CustomerTitle,
            CustomerName,
            CustomerLastName,
            ClaimCenter,
            ClaimGarage,
            InsuranceExpireDate,
            CarLicenseNo,
            CarBrand,
            CarModel,
            CarVIN,
            InsuranceTypeName,
            SumInsured,
            DiscountPercent,
            DiscountAmount,
            NetPremium,
            StampAmount,
            VatAmount,
            GrossPremium,
            Remark,
            Period,
            InsuranceCompanyName,
            ActNetPremium,
            ActStampAmount,
            ActVatAmount,
            ActGrossPremium,
            DriverFlag
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_040");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    ((Label)Page.Master.FindControl("lblTopic")).Text = "นำเข้าข้อมูลแจ้งเบี้ย";
                    InitialControl();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void InitialControl()
        {
            var biz = new SlmScr040Biz();
            cmbMonth.DataSource = biz.GetMonths();
            cmbMonth.DataTextField = "TextField";
            cmbMonth.DataValueField = "ValueField";
            cmbMonth.DataBind();
            cmbMonth.Items.Insert(0, new ListItem("", ""));
            cmbMonth.SelectedIndex = cmbMonth.Items.IndexOf(cmbMonth.Items.FindByValue(DateTime.Now.Month.ToString("00")));

            int curr_year = DateTime.Now.Year;
            cmbYear.Items.Insert(0, new ListItem("", ""));
            cmbYear.Items.Insert(1, new ListItem((curr_year - 1).ToString(), (curr_year - 1).ToString()));
            cmbYear.Items.Insert(2, new ListItem(curr_year.ToString(), curr_year.ToString()));
            cmbYear.Items.Insert(3, new ListItem((curr_year + 1).ToString(), (curr_year + 1).ToString()));
            cmbYear.SelectedIndex = cmbYear.Items.IndexOf(cmbYear.Items.FindByValue(curr_year.ToString()));
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuData.HasFile)
            {
                var ext = Path.GetExtension(fuData.FileName)?.ToLower();
                if (ext != ".xls")
                {
                    lblUploadError.Text = "กรุณาระบุไฟล์ให้ถูกต้อง (.xls)";
                    return;
                }

                try
                {
                    string filename = Path.GetTempFileName();
                    fuData.SaveAs(filename);
                    lblFilename.Text = fuData.FileName;
                    fuData.ClearAllFilesFromPersistedStore();

                    pnlError.Visible = false;
                    tbResult.Visible = false;

                    using (OleDbConnection conn = new OleDbConnection())
                    {
                        DataTable dt = new DataTable();
                        conn.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1;'", filename);
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM [kk$]";

                        using (OleDbDataAdapter adp = new OleDbDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }

                        List<ControlListData> errLst = new List<ControlListData>();

                        int maxRow = SLMConstant.ExcelMaxRowNotifyPremium;
                        if (cmbImportType.SelectedItem.Value == "2" && dt.Rows.Count > maxRow)
                        {
                            errLst.Add(new ControlListData { TextField = $"ไม่สามารถนำเข้าข้อมูลแจ้งเบี้ยเกิน {maxRow.ToString()} rows ได้", ValueField = "File" });
                            dt.Clear();
                        }

                        #region overall format validation

                        if (dt.Columns.Count != Enum.GetNames(typeof(ExcelColumnName)).Length)
                        {
                            errLst.Add(new ControlListData { TextField = "จำนวนคอลัมน์ไม่ถูกต้อง กรุณาตรวจสอบไฟล์ฟอร์แมต", ValueField = "File" });
                            dt.Clear();
                        }

                        #region validate order of seq : unwanted feature

                        /*
                        int[] lstRunNo = dt.AsEnumerable()
                            .Where(row => row[(int) ExcelColumnName.RunNo - A1] != DBNull.Value)
                            .Select(row => row.Field<double>((int) ExcelColumnName.RunNo - A1))
                            .Select(Convert.ToInt32)
                            .OrderBy(x => x)
                            .ToArray();
                        int[] missingRunNo = lstRunNo.Length > 0 ? Enumerable.Range(lstRunNo.First(), lstRunNo.Last()).Except(lstRunNo).ToArray() : new int[] { };
                        if (
                            lstRunNo.Length > 0 &&
                            (
                                missingRunNo.Length > 0 ||
                                lstRunNo.First() != 1 ||
                                lstRunNo.Last() != lstRunNo.Max() ||
                                lstRunNo.Length != dt.Rows.Count ||
                                false
                            )
                        )
                        {
                            Debug.WriteLine($"RunNo missing: {string.Join(",", missingRunNo)}");
                            errLst.Add(new ControlListData {TextField = "เลขลำดับไม่ถูกต้อง กรุณาตรวจสอบเลขลำดับ", ValueField = "File"});
                            dt.Clear();
                        }
                        */

                        #endregion

                        #endregion

                        var biz = new SlmScr040Biz();
                        var lstTitle = biz.GetTitleList();
                        var lstComp = biz.GetCompanyNameList();
                        var lstRptype = biz.GetRepairTypeNameList();
                        var lstCoveragetype = biz.GetCoverageTypeNameList();

                        // clean and squash duplicate VIN, prepare policy information for error message (if needed)
                        Dictionary<string, string[]> dictCleanedVIN = dt.AsEnumerable()
                            .Select(row => new
                            {
                                Key = Clean(row.Field<string>((int)ExcelColumnName.CarVIN - A1) ?? ""),
                                Value = new[]
                                {
                                    row.Field<string>((int) ExcelColumnName.InsuranceCompanyName - SLM.Application.SLM_SCR_040.A1) ?? "",
                                    $"เบี้ยประกันรวมภาษี {row.Field<double?>((int) ExcelColumnName.GrossPremium - SLM.Application.SLM_SCR_040.A1)?.ToString("#,##0.00") ?? ""}",
                                    $"เบี้ยประกันรวมภาษี (พรบ.) {row.Field<double?>((int) ExcelColumnName.ActGrossPremium - SLM.Application.SLM_SCR_040.A1)?.ToString("#,##0.00") ?? ""}"
                                }
                            })
                            .GroupBy(x => x.Key, StringComparer.InvariantCultureIgnoreCase)
                            .ToDictionary(x => x.Key, x => x.Select(y => string.Join(", ", y.Value)).ToArray());

                        List<SlmScr040Biz.notifypremiumdata> prLst = new List<SlmScr040Biz.notifypremiumdata>();
                        int total = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int j = i;
                            Func<ExcelColumnName, string> row = delegate (ExcelColumnName colName)
                            {
                                object o = dt.Rows[j][(int)colName - A1];
                                if (o is DateTime)
                                {
                                    return ((DateTime)o).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                                }
                                return o.ToString().Trim();
                            };

                            var clmer = new ErrorListString();
                            var company = row(ExcelColumnName.InsuranceCompanyName) == "" ? null : lstComp.FirstOrDefault(c => c.Name.Contains(row(ExcelColumnName.InsuranceCompanyName)));
                            var coverage = lstCoveragetype.FirstOrDefault(c => c.CoverageTypeName.Equals(row(ExcelColumnName.InsuranceTypeName), StringComparison.InvariantCultureIgnoreCase));

                            // verify data
                            if (dt.Rows[i].ItemArray.All(x => string.IsNullOrWhiteSpace(x.ToString()))) continue;
                            total++;

                            #region general empty check

                            if (row(ExcelColumnName.InsuranceCompanyName) == "") clmer.AddErrorDesc(ExcelColumnName.InsuranceCompanyName, "กรุณาระบุ ชื่อบริษัทประกัน");
                            if (row(ExcelColumnName.InsuranceCompanyName) != "" && company == null) clmer.AddErrorDesc(ExcelColumnName.InsuranceCompanyName, "ไม่พบข้อมูลชื่อบริษัทประกันในระบบ");
                            if (row(ExcelColumnName.InsuranceCompanyName) != "" && company != null && company.IsDeleted) clmer.AddErrorDesc(ExcelColumnName.InsuranceCompanyName, "ไม่สามารถ Upload ได้เนื่องบริษัทประกันภัยมีสถานะเป็นไม่ใช้งาน");
                            if (row(ExcelColumnName.PolicyNo) == "") clmer.AddErrorDesc(ExcelColumnName.PolicyNo, "กรุณาระบุ เลขที่กธ.");
                            if (row(ExcelColumnName.CustomerTitle) == "") clmer.AddErrorDesc(ExcelColumnName.CustomerTitle, "กรุณาระบุ คำนำหน้าชื่อ");
                            if (row(ExcelColumnName.CustomerName) == "") clmer.AddErrorDesc(ExcelColumnName.CustomerName, "กรุณาระบุ ชื่อผู้เอาประกัน");
                            // if (row(ExcelColumnName.CustomerLastName) == "") clmer.AddErrorDesc(ExcelColumnName.CustomerLastName, "กรุณาระบุ นามสกุลผู้เอาประกัน");
                            if (row(ExcelColumnName.InsuranceExpireDate) == "") clmer.AddErrorDesc(ExcelColumnName.InsuranceExpireDate, "กรุณาระบุ วันที่หมดอายุกรมธรรม์");
                            if (row(ExcelColumnName.InsuranceExpireDate) != "" && !CheckDate(row(ExcelColumnName.InsuranceExpireDate))) clmer.AddErrorDesc(ExcelColumnName.InsuranceExpireDate, "รูปแบบวันที่ไม่ถูกต้อง (DD/MM/YYYY)");
                            //if (row(ExcelColumnName.CarLicenseNo) == "") clmer.AddErrorDesc(ExcelColumnName.CarLicenseNo, "กรุณาระบุ ทะเบียน");
                            // if (row(ExcelColumnName.CarBrand) == "") clmer.AddErrorDesc(ExcelColumnName.CarBrand, "กรุณาระบุ ยี่ห้อ");
                            // if (row(ExcelColumnName.CarModel) == "") clmer.AddErrorDesc(ExcelColumnName.CarModel, "กรุณาระบุ รุ่น");
                            if (row(ExcelColumnName.CarVIN) == "") clmer.AddErrorDesc(ExcelColumnName.CarVIN, "กรุณาระบุ VIN/Chassis Number");
                            if (row(ExcelColumnName.InsuranceTypeName) == "") clmer.AddErrorDesc(ExcelColumnName.InsuranceTypeName, "กรุณาระบุ ประเภทประกัน");
                            if (row(ExcelColumnName.InsuranceTypeName) != "" && coverage == null) clmer.AddErrorDesc(ExcelColumnName.InsuranceTypeName, "ไม่พบข้อมูลประเภทประกันในระบบ");
                            if (row(ExcelColumnName.InsuranceTypeName) != "" && coverage != null && coverage.IsDeleted) clmer.AddErrorDesc(ExcelColumnName.InsuranceTypeName, "ประเภทประกันมีสถานะไม่ใช้งาน");
                            if (row(ExcelColumnName.SumInsured) == "") clmer.AddErrorDesc(ExcelColumnName.SumInsured, "กรุณาระบุ ทุนประกันใหม่");
                            if (row(ExcelColumnName.DiscountPercent) == "") clmer.AddErrorDesc(ExcelColumnName.DiscountPercent, "กรุณาระบุ ส่วนลดประวัติ(%)");
                            if (row(ExcelColumnName.DiscountAmount) == "") clmer.AddErrorDesc(ExcelColumnName.DiscountAmount, "กรุณาระบุ ส่วนลดประวัติ(บาท)");
                            if (row(ExcelColumnName.NetPremium) == "") clmer.AddErrorDesc(ExcelColumnName.NetPremium, "กรุณาระบุ เบี้ยสุทธิ");
                            if (row(ExcelColumnName.StampAmount) == "") clmer.AddErrorDesc(ExcelColumnName.StampAmount, "กรุณาระบุ อากร");
                            if (row(ExcelColumnName.VatAmount) == "") clmer.AddErrorDesc(ExcelColumnName.VatAmount, "กรุณาระบุ ภาษี");
                            if (row(ExcelColumnName.GrossPremium) == "") clmer.AddErrorDesc(ExcelColumnName.GrossPremium, "กรุณาระบุ เบี้ยประกันรวมภาษี");
                            //if (row(ExcelColumnName.ActNetPremium) == "") clmer.AddErrorDesc(ExcelColumnName.ActNetPremium, "กรุณาระบุ เบี้ยสุทธิ (พรบ.)");
                            //if (row(ExcelColumnName.ActStampAmount) == "") clmer.AddErrorDesc(ExcelColumnName.ActStampAmount, "กรุณาระบุ อากร (พรบ.)");
                            //if (row(ExcelColumnName.ActVatAmount) == "") clmer.AddErrorDesc(ExcelColumnName.ActVatAmount, "กรุณาระบุ ภาษี (พรบ.)");
                            if (row(ExcelColumnName.ActGrossPremium) == "") clmer.AddErrorDesc(ExcelColumnName.ActGrossPremium, "กรุณาระบุ เบี้ยประกันรวมภาษี (พรบ.)");
                            if (row(ExcelColumnName.DriverFlag) == "") clmer.AddErrorDesc(ExcelColumnName.DriverFlag, "กรุณาระบุ ระบุชื่อ");
                            if (row(ExcelColumnName.DriverFlag) != "" && !new[] { "Y", "N" }.Contains(row(ExcelColumnName.DriverFlag))) clmer.AddErrorDesc(ExcelColumnName.DriverFlag, "กรุณากำหนด ระบุชื่อ ระหว่าง Y หรือ N เท่านั้น");

                            #endregion

                            #region valid repair type

                            int rpTypeID = 0;
                            var claims = new[] { row(ExcelColumnName.ClaimCenter), row(ExcelColumnName.ClaimGarage) };
                            if (claims.SequenceEqual(new[] { "", "" }))
                            {
                                clmer.AddErrorDesc(ExcelColumnName.ClaimCenter, ExcelColumnName.ClaimGarage, "กรุณาระบุ ซ่อมห้าง / ซ่อมอู่");
                            }
                            else if (claims.SequenceEqual(new[] { "Y", "Y" }))
                            {
                                clmer.AddErrorDesc(ExcelColumnName.ClaimCenter, ExcelColumnName.ClaimGarage, "กรุณาเลือกระหว่าง ซ่อมห้าง / ซ่อมอู่");
                            }
                            else if (claims.SequenceEqual(new[] { "Y", "" }))
                            {
                                rpTypeID = lstRptype.Where(x => x.RepairTypeName.Contains("ห้าง")).Select(x => x.RepairTypeID).FirstOrDefault();
                                if (rpTypeID == 0) clmer.AddErrorDesc(ExcelColumnName.ClaimCenter, ExcelColumnName.ClaimGarage, "ไม่พบประเภทการซ่อมในระบบ");
                            }
                            else if (claims.SequenceEqual(new[] { "", "Y" }))
                            {
                                rpTypeID = lstRptype.Where(x => x.RepairTypeName.Contains("อู่")).Select(x => x.RepairTypeID).FirstOrDefault();
                                if (rpTypeID == 0) clmer.AddErrorDesc(ExcelColumnName.ClaimCenter, ExcelColumnName.ClaimGarage, "ไม่พบประเภทการซ่อมในระบบ");
                            }
                            else
                            {
                                clmer.AddErrorDesc(ExcelColumnName.ClaimCenter, ExcelColumnName.ClaimGarage, "กรุณากำหนด ซ่อมห้าง / ซ่อมอู่ ระหว่าง Y หรือ ปล่อยว่าง เท่านั้น");
                            }

                            #endregion

                            #region VIN/Chassis duplicate check

                            string cleanedVIN = Clean(row(ExcelColumnName.CarVIN));
                            if (dictCleanedVIN.Keys.Contains(cleanedVIN))
                            {
                                string[] lstDupVIN = dictCleanedVIN[cleanedVIN];
                                if (lstDupVIN.Length > 1)
                                {
                                    clmer.AddErrorDesc(ExcelColumnName.CarVIN, $"VIN/Chassis Number : {row(ExcelColumnName.CarVIN)} ซ้ำกันในไฟล์ดังนี้");
                                    clmer.AddRange(lstDupVIN);
                                }
                            }

                            #endregion

                            #region valid period

                            string month = "";
                            string year = "";
                            if (row(ExcelColumnName.Period) == "")
                            {
                                clmer.AddErrorDesc(ExcelColumnName.Period, "กรุณาระบุ Period");
                            }
                            else if (row(ExcelColumnName.Period).Length != 6)
                            {
                                clmer.AddErrorDesc(ExcelColumnName.Period, "รูปแบบ Period ไม่ถูกต้อง (YYYYMM)");
                            }
                            else
                            {
                                year = row(ExcelColumnName.Period).Substring(0, 4);
                                month = row(ExcelColumnName.Period).Substring(4, 2);
                                if (year != cmbYear.SelectedItem.Value || month != cmbMonth.SelectedItem.Value)
                                {
                                    clmer.AddErrorDesc(ExcelColumnName.Period, "Period ไม่ตรงกับที่ระบุบนหน้าจอ");
                                }
                            }

                            #endregion

                            if (clmer.Count > 0)
                            {
                                errLst.Add(new ControlListData { TextField = string.Join(",<br/>", clmer), ValueField = (i + 2).ToString() });
                            }
                            else
                            {
                                // get title
                                var tt = lstTitle.FirstOrDefault(t => t.Name == row(ExcelColumnName.CustomerTitle));

                                var np = new SlmScr040Biz.notifypremiumdata();
                                //np.slm_Contract_Number = row["B"].ToString();       Modified by Pom 21/03/2016 (Structure Changed)
                                np.slm_RunNo = row(ExcelColumnName.RunNo);
                                np.slm_PolicyNo = row(ExcelColumnName.PolicyNo);
                                np.cell0 = "unused ?";
                                np.slm_Title_Id = tt?.ID ?? 0;
                                np.slm_Name = row(ExcelColumnName.CustomerName);
                                np.slm_LastName = row(ExcelColumnName.CustomerLastName);
                                np.slm_Claim_Center = row(ExcelColumnName.ClaimCenter);
                                np.slm_Claim_Garage = row(ExcelColumnName.ClaimGarage);
                                np.slm_RepairTypeId = rpTypeID;
                                np.slm_InsExpireDate = GetDateValue(row(ExcelColumnName.InsuranceExpireDate));
                                np.slm_CarLicenseNo = row(ExcelColumnName.CarLicenseNo);
                                np.slm_Brand = row(ExcelColumnName.CarBrand);
                                np.slm_Model = row(ExcelColumnName.CarModel);
                                np.slm_VIN = row(ExcelColumnName.CarVIN);
                                np.slm_InsuranceCarTypeId = coverage?.CoverageTypeID;
                                np.slm_Sum_Insure = AppUtil.SafeDecimal(row(ExcelColumnName.SumInsured).Replace(",", ""));
                                np.slm_Discount_Percent = AppUtil.SafeInt(row(ExcelColumnName.DiscountPercent).Replace(",", ""));
                                np.slm_Discount_Amount = AppUtil.SafeDecimal(row(ExcelColumnName.DiscountAmount).Replace(",", ""));
                                np.slm_NetPremium = AppUtil.SafeDecimal(row(ExcelColumnName.NetPremium).Replace(",", ""));
                                np.slm_Stamp = AppUtil.SafeDecimal(row(ExcelColumnName.StampAmount).Replace(",", ""));
                                np.slm_Vat_Amount = AppUtil.SafeDecimal(row(ExcelColumnName.VatAmount).Replace(",", ""));
                                np.slm_GrossPremium = AppUtil.SafeDecimal(row(ExcelColumnName.GrossPremium).Replace(",", ""));
                                np.slm_Remark = row(ExcelColumnName.Remark);
                                np.slm_InsuranceComId = company?.ID;
                                np.slm_PeriodMonth = month;
                                np.slm_PeriodYear = year;
                                np.slm_ActNetPremium = AppUtil.SafeDecimal(row(ExcelColumnName.ActNetPremium).Replace(",", ""));
                                np.slm_ActStamp = AppUtil.SafeDecimal(row(ExcelColumnName.ActStampAmount).Replace(",", ""));
                                np.slm_ActVat_Amount = AppUtil.SafeDecimal(row(ExcelColumnName.ActVatAmount).Replace(",", ""));
                                np.slm_ActGrossPremium = AppUtil.SafeDecimal(row(ExcelColumnName.ActGrossPremium).Replace(",", ""));
                                np.slm_DriverFlag = row(ExcelColumnName.DriverFlag);
                                prLst.Add(np);
                            }
                        }

                        if (errLst.Count <= 0)
                        {
                            bool ret;
                            if (cmbImportType.SelectedItem.Value == "1")
                            {
                                ret = biz.InsertNotifyPremiumToTemp(prLst, System.Web.HttpContext.Current.User.Identity.Name, lblFilename.Text.ToLowerInvariant().Trim());
                            }
                            else
                            {
                                ret = biz.InsertNotifyPremiumData(prLst, System.Web.HttpContext.Current.User.Identity.Name);
                            }

                            if (!ret)
                            {
                                throw new Exception(biz.ErrorMessage);
                            }
                            else
                            {
                                AppUtil.ClientAlert(Page, "นำเข้าข้อมูลเรียบร้อย");
                            }
                        }
                        else
                        {
                            // show error
                            gvError.DataSource = errLst;
                            gvError.DataBind();
                            pnlError.Visible = true;

                            AppUtil.ClientAlert(Page, "ข้อมูลไม่ถูกนำเข้า กรุณาตรวจสอบข้อผิดพลาด");
                        }

                        lblTotal.Text = total.ToString("#,##0");
                        lblSucc.Text = prLst.Count.ToString("#,##0");
                        lblFail.Text = (total - prLst.Count).ToString("#,##0");
                        tbResult.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    lblTotal.Text = lblSucc.Text = lblFail.Text = "0";
                    tbResult.Visible = true;

                    var err = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                    if (err.Contains("kk") && err.Contains("is not a valid"))
                    {
                        err = "ไม่พบข้อมูล Sheet kk";
                    }
                    AppUtil.ClientAlert(Page, err);
                }
            }
        }

        private bool CheckDate(string dtString)
        {
            var chk = dtString.Split(' ')[0].Split('/');
            if (chk.Length != 3) return false;
            int d, m, y;
            int.TryParse(chk[0], out d);
            int.TryParse(chk[1], out m);
            int.TryParse(chk[2], out y);

            if (y == 0 || m == 0 || d == 0 || m > 12) return false;
            else return true;
        }

        private DateTime? GetDateValue(string dtString)
        {
            try
            {
                var chk = dtString.Split('/');
                if (chk.Length != 3) return null;
                int d, m, y;
                int.TryParse(chk[0], out d);
                int.TryParse(chk[1], out m);
                int.TryParse(chk[2], out y);

                return new DateTime(y, m, d);
            }
            catch
            {
                return null;
            }
        }

        protected void lnbClear_Click(object sender, EventArgs e)
        {
            fuData.ClearAllFilesFromPersistedStore();
            //Response.Redirect(Request.Url.PathAndQuery);
        }

        private int ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException(nameof(columnName));
            int sum = columnName.ToUpperInvariant().ToCharArray().Aggregate(0, (accu, t) => accu * 26 + (t - A1 + 1));
            return sum;
        }

        private string Clean(string vin)
        {
            return Regex.Replace(vin, @"[\s-*_\\\/]", "");
        }

        private class ErrorListString : List<string>
        {
            private string GetExcelColumnName(int columnNumber)
            {
                int dividend = columnNumber - A1 + 1;
                string columnName = string.Empty;

                while (dividend > 0)
                {
                    int modulo = (dividend - 1) % 26;
                    columnName = Convert.ToChar(A1 + modulo) + columnName;
                    dividend = (dividend - modulo) / 26;
                }

                return columnName;
            }

            private new void Add(string x)
            {
                throw new NotImplementedException();
            }

            internal void AddErrorDesc(ExcelColumnName col, string message)
            {
                base.Add($"Column {GetExcelColumnName((int)col)}: {message}");
            }

            internal void AddErrorDesc(ExcelColumnName col1, ExcelColumnName col2, string message)
            {
                base.Add($"Column {GetExcelColumnName((int)col1)}, {GetExcelColumnName((int)col2)}: {message}");
            }
        }

        //I-018067
        protected void cmbImportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string value = ((DropDownList)sender).SelectedItem.Value;
            lblRemark.Text = value == "1" ? "ระบบจะนำเข้าข้อมูลแจ้งเบี้ยโดย Batch สิ้นวัน" : ""; 
        }
    }
}
