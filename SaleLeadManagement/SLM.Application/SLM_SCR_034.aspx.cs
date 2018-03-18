using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using SLM.Application.Services;
using SLM.Resource;

namespace SLM.Application
{
    public partial class SLM_SCR_034 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_034));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_034");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    ((Label)Page.Master.FindControl("lblTopic")).Text = "นำเข้างานติดปัญหา";
                    Page.Form.Attributes.Add("enctype", "multipart/form-data");

                    Page.Form.DefaultButton = btnSearch.UniqueID;

                    cmbInsComImport.DataSource = SlmScr034Biz.GetImportInsComListData();
                    cmbInsComImport.DataBind();

                    cmbInsComSearch.DataSource = SlmScr034Biz.GetSearchInsComListData();
                    cmbInsComSearch.DataBind();

                    cmbFileNameSearch.DataSource = SlmScr034Biz.GetSearchFileNameListData();
                    cmbFileNameSearch.DataBind();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnImportExcel_Click(object sender, EventArgs e)
        {
            mpePopupImport.Show();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //tableUploadResult.Visible = false;
            tableUploadResult.Visible = false;
            cmbInsComImport.SelectedIndex = 0;
            gvUploadErrorHeader.Visible = false;
            gvUploadError.Visible = false;
            mpePopupImport.Hide();
        }
        protected void PageSearchChange(object sender, EventArgs e)
        {
            var result = SlmScr034Biz.GetSearchResult(cmbInsComSearch.SelectedValue, cmbFileNameSearch.SelectedValue);
            BindGridview(pcTop, result.ToArray(), pcTop.SelectedPageIndex);
        }


        protected void btnUpload_Click(object sender, EventArgs e)
        {
            tableUploadResult.Visible = false;
            gvUploadError.Visible = false;
            gvUploadErrorHeader.Visible = false;

            if (cmbInsComImport.SelectedIndex == 0)
            {
                lblCompError.Text = "กรุณาเลือกบริษัท";
                fuData.ClearAllFilesFromPersistedStore();
                mpePopupImport.Show();
                return;
            }

            if (fuData.HasFile)
            {
                var ext = Path.GetExtension(fuData.FileName).ToLower();
                if (ext != ".xls")
                {
                    lblUploadError.Text = "กรุณาระบุไฟล์ให้ถูก format (xls)";
                    fuData.ClearAllFilesFromPersistedStore();
                    mpePopupImport.Show();
                    return;
                }

                try
                {
                    using (OleDbConnection conn = new OleDbConnection())
                    {
                        DataTable dt = new DataTable();
                        string filename = Path.GetTempFileName();
                        lblFilename.Text = fuData.FileName;
                        fuData.SaveAs(filename);
                        //fuData.ClearAllFilesFromPersistedStore();

                        conn.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=0;'", filename);

                        conn.Open();
                        DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();

                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        //cmd.CommandText = "SELECT * FROM [Sheet1$]";
                        cmd.CommandText = "SELECT * FROM [" + firstSheetName + "]";

                        OleDbDataAdapter adp = new OleDbDataAdapter(cmd);
                        adp.Fill(dt);
                        adp.Dispose();
                        cmd.Dispose();

                        decimal insComId = cmbInsComImport.SelectedValue == "" ? -1 : decimal.Parse(cmbInsComImport.SelectedValue);
                        // check duplicate 
                        bool hasDup = SlmScr034Biz.HasDuplicate(insComId, fuData.FileName, DateTime.Today);
                        if (hasDup)
                        {
                            Session["ImportTable"] = dt;
                            Session["insComId"] = insComId;
                            Session["fileName"] = fuData.FileName;
                            mpePopupImport.Show();
                            mpePopupConfirm.Show();
                        }
                        else
                        {
                            if (DoImport(dt, insComId, fuData.FileName))
                            {
                                // AppUtil.ClientAlert(Page, "นำเข้าข้อมูลงานติดปัญหาเรียบร้อย"); -- change to display result on popup instead                                
                                btnSearch_Click(null, null);
                                cmbFileNameSearch.DataSource = SlmScr034Biz.GetSearchFileNameListData();
                                cmbFileNameSearch.DataBind();
                                AppUtil.ClientAlert(this, "นำเข้าข้อมูลเรียบร้อย");
                            }
                            else
                                AppUtil.ClientAlert(this, "ข้อมูลไม่ถูกนำเข้า กรุณาตรวจสอบข้อผิดพลาด");
                        }

                    }
                }
                catch (Exception ex)
                {
                    // if (ex.Message.Contains("Ticket Id มากกว่า 1 รายการ "))     
                    string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                    _log.Error(message);
                    AppUtil.ClientAlert(Page, message);
                }
                finally
                {
                    fuData.ClearAllFilesFromPersistedStore();
                }
            }
            else lblUploadError.Text = "กรุณาระบุไฟล์ที่ต้องการนำเข้า";

            // lblUploadError.Text = "after upload";
            mpePopupImport.Show();
        }

        private bool DoImport(DataTable dt, decimal insComId, string fileName)
        {
            SlmProblemData data = new SlmProblemData();
            try
            {
                data.ProblemDetails = SlmScr034Biz.ValidateProblemDetails(dt, cmbInsComImport.SelectedItem.Text);


                if (data.ProblemDetails.Count > 0
                    && data.ProblemDetails.Where(p => p.hasError) != null
                    && data.ProblemDetails.Where(p => p.hasError).Count() > 0)
                {
                    gvUploadError.DataSource = data.ProblemDetails.Where(p => p.hasError).ToList();
                    gvUploadError.DataBind();
                    //lblFilename.Text = fileName;
                    lblFail.Text = data.ProblemDetails.Where(p => p.hasError).Count().ToString("#,##0");
                    lblSuccess.Text = "0";
                    lblTotal.Text = data.ProblemDetails.Count.ToString("#,##0");// lblFail.Text;
                    tableUploadResult.Visible = true;
                    gvUploadErrorHeader.Visible = true;
                    gvUploadError.Visible = true;
                    return false;
                }
                else
                {
                    data.Ins_Com_Id = insComId;
                    data.FileName = fileName;
                    data.Deleted = false;
                    data.UpdatedBy = HttpContext.Current.User.Identity.Name;
                    data.UpdatedDate = DateTime.Now;
                    SlmScr034Biz.SaveProblemData(data);

                    // บันทึกลง CAR System
                    // CreateCASActivityLog(data);
                    var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                    data.ProblemDetails.Where(ticket => ticket.needCARprocess)
                        .Select(t => new
                        {
                            TicketId = t.TicketId
                            ,
                            RenewInsureId = t.RenewInsureId
                            ,
                            IsPolicyPurchase = t.IsPolicyPurchase
                            ,
                            IsActPurchase = t.IsActPurchase
                            ,
                            PolicyRecAmt = t.PolicyRecAmt
                            ,
                            ActRecAmt = t.ActRecAmt
                            ,
                            PolicyLastRecAmt = (decimal?)null
                            ,
                            ActLastRecAmt = (decimal?) null
                        })
                        .Distinct()
                        .ToList()
                        .ForEach(ticket =>
                        {
                            AppUtil.CreateCASActivityLog(this, ticket.TicketId, ticket.RenewInsureId, ticket.IsPolicyPurchase, ticket.IsActPurchase, ticket.PolicyLastRecAmt, ticket.ActLastRecAmt, ticket.PolicyRecAmt, ticket.ActRecAmt, staff);
                        });
                    //lblFilename.Text = fileName;
                    lblFail.Text = "0";
                    lblSuccess.Text = data.ProblemDetails.Count.ToString("#,##0");
                    lblTotal.Text = lblSuccess.Text;
                    tableUploadResult.Visible = true;
                    gvUploadErrorHeader.Visible = false;
                    gvUploadError.Visible = true;
                    gvUploadError.DataSource = null;
                    gvUploadError.DataBind();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Ticket Id มากกว่า 1 รายการ "))
                {
                    _log.Error(ex.Message);
                    AppUtil.ClientAlert(Page, ex.Message);
                }
                else
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _log.Error(message);
                    AppUtil.ClientAlert(Page, message);
                }
                return false;
            }
        }

        protected void btnConfirmImport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = null;
                decimal insComId = -1;
                string fileName = "";

                if (Session["ImportTable"] != null)
                    dt = Session["ImportTable"] as DataTable;
                if (Session["insComId"] != null)
                    decimal.TryParse(Session["insComId"].ToString(), out insComId);
                if (Session["fileName"] != null)
                    fileName = Session["fileName"].ToString();
                if (fileName != "" && insComId > 0)
                {
                    SlmScr034Biz.DeleteImportedProblem(insComId, fileName, DateTime.Today);
                    if (DoImport(dt, insComId, fileName))
                    {
                        // AppUtil.ClientAlert(Page, "นำเข้าข้อมูลงานติดปัญหาเรียบร้อย");
                        btnSearch_Click(null, null);
                        cmbFileNameSearch.DataSource = SlmScr034Biz.GetSearchFileNameListData();
                        cmbFileNameSearch.DataBind();
                        AppUtil.ClientAlert(this, "นำเข้าข้อมูลเรียบร้อย");
                    }
                    else
                        AppUtil.ClientAlert(this, "ข้อมูลไม่ถูกนำเข้า กรุณาตรวจสอบข้อผิดพลาด");
                }
                mpePopupConfirm.Hide();
                upnPopupImport.Update();
                mpePopupImport.Show();
                //clearSession();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
            finally
            {
                fuData.ClearAllFilesFromPersistedStore();
            }
        }

        protected void btnCancelImport_Click(object sender, EventArgs e)
        {
            try
            {
                fuData.ClearAllFilesFromPersistedStore();
                mpePopupConfirm.Hide();
                clearSession();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void clearSession()
        {
            if (Session["ImportTable"] != null)
                Session.Remove("ImportTable");
            if (Session["insComId"] != null)
                Session.Remove("insComId");
            if (Session["fileName"] != null)
                Session.Remove("fileName");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var result = SlmScr034Biz.GetSearchResult(cmbInsComSearch.SelectedValue, cmbFileNameSearch.SelectedValue);
                BindGridview(pcTop, result.ToArray(), 0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex);
            pcTop.Visible = true;
            gvResult.Visible = true;
            upResult.Update();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            cmbInsComSearch.SelectedIndex = 0;
            cmbFileNameSearch.SelectedIndex = 0;
            pcTop.Visible = false;
            gvResult.Visible = false;
        }
        /*
                private void CreateCASActivityLog(SlmProblemData problem)
                {
                    var staff = StaffBiz.GetStaff(Page.User.Identity.Name);
                    problem.ProblemDetails.Where(data => data.needCARprocess).ToList()
                        .ForEach(data =>
                    {
                        try
                        {
                            string preleadId = data.preleadId == null ? "" : data.preleadId.ToString();
                            string actSendDate = data.ActSendDate == null ? "" : (data.ActSendDate.Value.ToString("dd-MM-") + data.ActSendDate.Value.Year.ToString());

                            string incentiveValue = data.IncentiveFlag != null && data.IncentiveFlag == true ? "Y" : "N";
                            string incentiveActValue = data.ActIncentiveFlag != null && data.ActIncentiveFlag == true ? "Y" : "N";

                            decimal? currentPaidPolicy = data.PolicyGrossPremiumTotal;
                            decimal? currentPaidAct = data.ActGrossPremium;

                            //Activity Info
                            List<CARService.DataItem> actInfoList = new List<CARService.DataItem>();
                            actInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่รับแจ้ง", DataValue = data != null ? data.ReceiveNo : "" });
                            actInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "วันที่ส่งแจ้ง พรบ.", DataValue = actSendDate });
                            actInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "Incentive ประกัน", DataValue = incentiveValue });
                            actInfoList.Add(new CARService.DataItem() { SeqNo = 4, DataLabel = "Incentive พรบ.", DataValue = incentiveActValue });
                            actInfoList.Add(new CARService.DataItem() { SeqNo = 5, DataLabel = "ค่าเบี้ยประกันที่ลูกค้าชำระครั้งนี้", DataValue = currentPaidPolicy != null ? currentPaidPolicy.Value.ToString("#,##0.00") : "" });
                            actInfoList.Add(new CARService.DataItem() { SeqNo = 6, DataLabel = "ค่าเบี้ยพรบ.ที่ลูกค้าชำระครั้งนี้", DataValue = currentPaidAct != null ? currentPaidAct.Value.ToString("#,##0.00") : "" });
                            actInfoList.Add(new CARService.DataItem() { SeqNo = 7, DataLabel = "ค่าเบี้ยประกันที่ลูกค้าชำระรวม", DataValue = data.PolicyRecAmt != null ? data.PolicyRecAmt.Value.ToString("#,##0.00") : "" });
                            actInfoList.Add(new CARService.DataItem() { SeqNo = 8, DataLabel = "ค่าเบี้ยพรบ.ที่ลูกค้าชำระรวม", DataValue = data.ActRecAmt != null ? data.ActRecAmt.Value.ToString("#,##0.00") : "" });

                            //Customer Info
                            List<CARService.DataItem> cusInfoList = new List<CARService.DataItem>();
                            cusInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = data.CitizenId ?? "" });
                            cusInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = data.CardTypeName ?? "" });
                            cusInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = data.CustomerName ?? "" });

                            //Product Info
                            List<CARService.DataItem> prodInfoList = new List<CARService.DataItem>();
                            prodInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = data.ProductGroup ?? "" });
                            prodInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = data.ProductName ?? "" });
                            prodInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = data.Campaign ?? "" });

                            //Contract Info
                            List<CARService.DataItem> contInfoList = new List<CARService.DataItem>();
                            contInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = data.ContractNo ?? "" });
                            contInfoList.Add(new CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = preleadId != "" ? "HP" : "" });
                            contInfoList.Add(new CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = data.LicenseNo ?? "" });

                            //Officer Info
                            List<CARService.DataItem> offInfoList = new List<CARService.DataItem>();
                            offInfoList.Add(new CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = staff == null ? "SYSTEM" : staff.StaffNameTH });


                            CARService.CARServiceData logdata = new CARService.CARServiceData()
                            {
                                ReferenceNo = data.RenewInsureId.ToString(),
                                SecurityKey = preleadId != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey,
                                ServiceName = "CreateActivityLog",
                                SystemCode = preleadId != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM,        //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
                                TransactionDateTime = DateTime.Now,
                                ActivityInfoList = actInfoList,
                                CustomerInfoList = cusInfoList,
                                ProductInfoList = prodInfoList,
                                ContractInfoList = contInfoList,
                                OfficerInfoList = offInfoList,
                                ActivityDateTime = DateTime.Now,
                                CampaignId = data.Campaign,
                                ChannelId = SLMConstant.CARLogService.CARPreleadChannelId, // copy from Oum's change
                                PreleadId = preleadId,
                                ProductGroupId = data.ProductGroup,
                                ProductId = data.ProductName,
                                Status = data.StatusDesc,
                                SubStatus = data.ExternalSubStatusDesc,
                                TicketId = data.TicketId,
                                SubscriptionId = data.CitizenId,
                                TypeId = SLMConstant.CARLogService.Data.TypeId,
                                AreaId = SLMConstant.CARLogService.Data.AreaId,
                                SubAreaId = SLMConstant.CARLogService.Data.SubAreaId,
                                ActivityTypeId = SLMConstant.CARLogService.Data.ActivityType.TodoId,
                                ContractNo = data != null ? data.ContractNo : ""
                            };

                            if (data.SubscriptionTypeId != null)
                                logdata.SubscriptionTypeId = data.SubscriptionTypeId.Value;

                            bool ret = CARService.CreateActivityLog(logdata);
                            // AppUtil.UpdatePhonecallCASFlag(slmdb, phonecall_id, ret ? "1" : "2");
                        }
                        catch (Exception ex)
                        {
                            //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                            string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            _log.Error(message);
                        }
                    });
                }
            */
    }
}
