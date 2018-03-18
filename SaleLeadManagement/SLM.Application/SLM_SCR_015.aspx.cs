using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Application.Utilities;
using log4net;
using System.Net;
using SLM.Resource;
using System.Threading;
using System.Globalization;

namespace SLM.Application
{
    public partial class SLM_SCR_015 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_015));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "User Monitoring";
            Page.Form.DefaultButton = btnSearch.UniqueID;
            //AppUtil.BuildCombo(cmbSubStatus, SlmScr015Biz.GetSubStatusList(), "", "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            try
            {
                if (!IsPostBack)
                {
                    _log.Debug("Before Privillege");
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_015");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    if (SlmScr015Biz.GetStaffType(HttpContext.Current.User.Identity.Name) == null)
                        txtStaffTypeIdLogin.Text = "";
                    else
                        txtStaffTypeIdLogin.Text = SlmScr015Biz.GetStaffType(HttpContext.Current.User.Identity.Name).ToString();

                    InitialControl();

                    tdMKTAssighDateFrom.DateValue = DateTime.Now;
                    tdMKTAssighDateTo.DateValue = DateTime.Now;
                    _log.Debug("Before GetStaffId");

                    //txtStaffId.Text = SlmScr015Biz.GetStaffId(HttpContext.Current.User.Identity.Name);

                    StaffData staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                    txtStaffId.Text = staff.StaffId.ToString();
                    txtEmpCode.Text = staff.EmpCode;


                    if (txtStaffTypeIdLogin.Text == "")
                    {
                        //pnlMain.CssClass="NoneHidden";
                        //pnlMKT.CssClass="Hidden";

                    }
                    else
                    {
                        if (
                            new[]
                            {
                                SLMConstant.StaffType.Marketing,
                                SLMConstant.StaffType.CallReminder,
                                SLMConstant.StaffType.NormalExpired
                            }.Contains(decimal.Parse(txtStaffTypeIdLogin.Text))
                        )
                        {
                            lbWaitSum.Visible = false;
                            lbUnassignLeadMKT.Visible = false;
                            cmbStatusMKT.SelectedIndex = 0;
                            pnlMKT.CssClass = "NoneHidden";
                        }
                        else
                        {
                            decimal? stafftype = SlmScr003Biz.GetStaffType(HttpContext.Current.User.Identity.Name);
                            lbUnassignLeadMKT.Text = SlmScr015Biz.GetNumOfUnassignLead(HttpContext.Current.User.Identity.Name, stafftype);
                            lbWaitSum.Text = "จำนวน Lead รอจ่าย";
                            cmbStatusMKT.SelectedIndex = 0;
                            pnlMKT.CssClass = "NoneHidden";
                        }
                    }

                    GetUserMonitoringMKTList();
                    if (gvMKT.Rows.Count > 0)
                    {
                        for (int i = 0; i < gvMKT.Rows.Count; i++)
                        {
                            Label lbUsername = (Label)gvMKT.Rows[i].FindControl("lbUsername");
                            txtListUsernameFooter.Text += (txtListUsernameFooter.Text == "" ? "" : ",") + "'" + lbUsername.Text.Trim() + "'";
                        }
                    }
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
            //Product
            //cmbProduct.DataSource = SlmScr015Biz.GetProductList();
            cmbProduct.DataSource = SlmScr015Biz.GetProductListNew();
            cmbProduct.DataTextField = "TextField";
            cmbProduct.DataValueField = "ValueField";
            cmbProduct.DataBind();
            cmbProduct.Items.Insert(0, new ListItem("", "0"));  //value = 0 ป้องกันในกรณีส่งค่า ช่องว่างไป where ใน CMT_CAMPAIGN_PRODUCT แล้วค่า PR_ProductId บาง record เป็นช่องว่าง

            //แคมเปญ
            //cmbCampaign.DataSource = SlmScr003Biz.GetCampaignData();
            cmbCampaign.DataSource = SlmScr003Biz.GetCampaignDataNew();
            cmbCampaign.DataTextField = "TextField";
            cmbCampaign.DataValueField = "ValueField";
            cmbCampaign.DataBind();
            cmbCampaign.Items.Insert(0, new ListItem("", ""));


            //Search Branch
            //if (txtStaffTypeIdLogin.Text == SLMConstant.StaffType.Marketing.ToString())
            cmbSearchBranch.DataSource = BranchBiz.GetMonitoringBranchList2(SLMConstant.Branch.All, HttpContext.Current.User.Identity.Name);
            //else
            //    cmbSearchBranch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.Active);
            cmbSearchBranch.DataTextField = "TextField";
            cmbSearchBranch.DataValueField = "ValueField";
            cmbSearchBranch.DataBind();
            cmbSearchBranch.Items.Insert(0, new ListItem("", ""));
        }

        private void FindStaffRecusive(int? headId, ArrayList arr, List<StaffData> staffList)
        {
            foreach (StaffData staff in staffList)
            {
                if (staff.HeadStaffId == headId)
                {
                    arr.Add("'" + staff.UserName + "'");
                    FindStaffRecusive(staff.StaffId, arr, staffList);
                }
            }
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvMKT);
            pageControl.UpdateMonitoring(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                string userList = "";

                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;

                string activeflag = "";
                if (cmbStatusMKT.SelectedItem.Value == "")
                    activeflag = "'0','1'";
                else
                    activeflag = cmbStatusMKT.SelectedItem.Value;

                txtIsActive.Text = activeflag;

                if (txtRecursive.Text.Trim() == "")
                {
                    List<StaffData> staffList = SlmScr015Biz.GetStaffList();
                    int? staffId = txtStaffId.Text.Trim() != string.Empty ? int.Parse(txtStaffId.Text.Trim()) : 0;
                    ArrayList arr = new ArrayList();

                    FindStaffRecusive(staffId, arr, staffList);

                    foreach (string staff_Id in arr)
                    {
                        userList += (userList == "" ? "" : ",") + staff_Id;
                    }
                    txtRecursive.Text = userList.Trim();
                }


                if (txtRecursive.Text.Trim() != string.Empty)
                {
                    List<UserMonitoringMKTData> result = SlmScr015Biz.SearchUserMonitoringMKT(txtRecursive.Text.Trim(), cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, activeflag, txtSubStatus.Text.Trim(), tdMKTAssighDateFrom.DateValue, tdMKTAssighDateTo.DateValue);

                    UserMonitoringMKTData resultfirst = result.Where(p => p.Username.ToUpper() == HttpContext.Current.User.Identity.Name.ToUpper()).FirstOrDefault();
                    if (resultfirst != null)
                    {
                        result.Remove(resultfirst);
                        result.Insert(0, resultfirst);
                    }

                    var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                    BindGridview(pageControl, result.ToArray(), pageControl.SelectedPageIndex);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void BindGridviewPopup(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvPopMKT);
            pageControl.Update(items, pageIndex);
            upPopMKT.Update();
            mpePopupMKT.Show();
        }

        private void BindGridviewPopupMKT(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvPopMKT);
            pageControl.Update(items, pageIndex);
            upPopMKT.Update();
        }

        private void BindGridviewMKT(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvMKT);
            pageControl.UpdateMonitoring(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChangePopupMKT(object sender, EventArgs e)
        {
            try
            {
                List<SearchLeadResult> resultList = new List<SearchLeadResult>();
                if (txtPopupFlag.Text == "UnassignLead")
                    resultList = SlmScr015Biz.SearchUserMonitoringList(HttpContext.Current.User.Identity.Name);
                else if (txtPopupFlag.Text == "AmountFooter")
                {
                    string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                    string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                    resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                   cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtListUsernameFooter.Text.Trim(), txtStatuscode.Text.Trim());
                }
                else
                {
                    string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                    string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                    resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                   cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text.Trim(), txtStatuscode.Text.Trim());
                }

                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridviewPopupMKT(pageControl, resultList.ToArray(), pageControl.SelectedPageIndex);
                mpePopupMKT.Show();

            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        public void GetUserMonitoringMKTList()
        {
            try
            {
                string userList = "";

                string tmpfrom = tdMKTAssighDateFrom.DateValue == DateTime.MinValue ? string.Empty : tdMKTAssighDateFrom.DateValue.ToString("yyyy-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue == DateTime.MinValue ? string.Empty : tdMKTAssighDateTo.DateValue.ToString("yyyy-MM-dd");

                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;

                string activeflag = "";
                if (cmbStatusMKT.SelectedItem.Value == "")
                    activeflag = "'0','1'";
                else
                    activeflag = cmbStatusMKT.SelectedItem.Value;

                txtIsActive.Text = activeflag;

                if (txtRecursive.Text.Trim() == "")
                {
                    List<StaffData> staffList = SlmScr015Biz.GetStaffList();
                    int? staffId = txtStaffId.Text.Trim() != string.Empty ? int.Parse(txtStaffId.Text.Trim()) : 0;
                    ArrayList arr = new ArrayList();
                    arr.Add("'" + HttpContext.Current.User.Identity.Name + "'");

                    FindStaffRecusive(staffId, arr, staffList);

                    foreach (string staff_Id in arr)
                    {
                        userList += (userList == "" ? "" : ",") + staff_Id;
                    }

                    txtRecursive.Text = userList.Trim();
                }

                if (txtRecursive.Text.Trim() != string.Empty)
                {

                    List<UserMonitoringMKTData> result = SlmScr015Biz.SearchUserMonitoringMKT(txtRecursive.Text.Trim(), cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, activeflag, txtSubStatus.Text.Trim(), tdMKTAssighDateFrom.DateValue, tdMKTAssighDateTo.DateValue);

                    UserMonitoringMKTData resultfirst = result.Where(p => p.Username.ToUpper() == HttpContext.Current.User.Identity.Name.ToUpper()).FirstOrDefault();
                    if (resultfirst != null)
                    {
                        result.Remove(resultfirst);
                        result.Insert(0, resultfirst);
                    }

                    BindGridviewMKT((SLM.Application.Shared.GridviewPageController)pcTop10, result.ToArray(), 0);
                }
                else
                {
                    BindGridviewMKT((SLM.Application.Shared.GridviewPageController)pcTop10, (new List<UserMonitoringData>()).ToArray(), 0);
                }

                upResult.Update();


            }
            catch
            {
                throw;
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSubStatus.Text.Length > 0 && txtSubStatus.Text.Trim().Length < 4)
                {
                    AppUtil.ClientAlert(this, "เมื่อต้องการค้นหาด้วย \"สถานะย่อยของ Lead\" กรุณากรอกไม่ต่ำกว่า 4 ตัวอักษร");
                    return;
                }
                else if ((tdMKTAssighDateFrom.DateValue == DateTime.MinValue && tdMKTAssighDateTo.DateValue != DateTime.MinValue) ||
                        (tdMKTAssighDateFrom.DateValue != DateTime.MinValue && tdMKTAssighDateTo.DateValue == DateTime.MinValue))
                {
                    AppUtil.ClientAlert(this, "กรุณาระบุวันที่จ่ายงานเริ่มต้นและสิ้นสุดให้ครบถ้วน");
                    return;
                }
                GetUserMonitoringMKTList();
                if (gvMKT.Rows.Count > 0)
                {
                    for (int i = 0; i < gvMKT.Rows.Count; i++)
                    {
                        Label lbUsername = (Label)gvMKT.Rows[i].FindControl("lbUsername");
                        txtListUsernameFooter.Text += (txtListUsernameFooter.Text == "" ? "" : ",") + "'" + lbUsername.Text.Trim() + "'";
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbAmount1_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.StatusCode.Interest;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text.Trim(), SLMConstant.StatusCode.Interest);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";
                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbAmount14_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.StatusCode.OnProcess;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text.Trim(), SLMConstant.StatusCode.OnProcess);
                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbAmount15_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.StatusCode.WaitContact;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text, SLMConstant.StatusCode.WaitContact);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbAmount5_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";

                txtStatuscode.Text = SLMConstant.StatusCode.WaitConsider;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text, SLMConstant.StatusCode.WaitConsider);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void lbAmount6_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.StatusCode.ApproveAccept;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text.Trim(), SLMConstant.StatusCode.ApproveAccept);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbAmount11_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.StatusCode.RoutebackEdit;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text.Trim(), SLMConstant.StatusCode.RoutebackEdit);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbTotal_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "AmountTotal";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = "ALL";
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text.Trim(), "ALL");

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbSumAmount1_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;

                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = SLMConstant.StatusCode.Interest;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtListUsernameFooter.Text.Trim(), SLMConstant.StatusCode.Interest);
                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";
                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbSumAmount15_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;

                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = SLMConstant.StatusCode.WaitContact;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtListUsernameFooter.Text.Trim(), SLMConstant.StatusCode.WaitContact);
                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";
                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbSumAmount14_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;

                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = SLMConstant.StatusCode.OnProcess;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtListUsernameFooter.Text.Trim(), SLMConstant.StatusCode.OnProcess);
                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbSumAmount5_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;

                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = SLMConstant.StatusCode.WaitConsider;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtListUsernameFooter.Text, SLMConstant.StatusCode.WaitConsider);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbSumAmount6_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;

                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = SLMConstant.StatusCode.ApproveAccept;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtListUsernameFooter.Text.Trim(), SLMConstant.StatusCode.ApproveAccept);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbSumAmount11_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;

                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = SLMConstant.StatusCode.RoutebackEdit;
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtListUsernameFooter.Text.Trim(), SLMConstant.StatusCode.RoutebackEdit);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbSumTotal_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;

                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "ALL";
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtListUsernameFooter.Text.Trim(), "ALL");

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "Hidden";

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCloseMKT_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtStaffTypeIdLogin.Text == "")
                {

                }
                else
                {
                    if (
                        new[]
                        {
                            SLMConstant.StaffType.Marketing,
                            SLMConstant.StaffType.CallReminder,
                            SLMConstant.StaffType.NormalExpired
                        }.Contains(decimal.Parse(txtStaffTypeIdLogin.Text))
                    )
                    {
                        lbWaitSum.Visible = false;
                        lbUnassignLeadMKT.Visible = false;
                        pnlMKT.CssClass = "NoneHidden";
                    }
                    else
                    {
                        decimal? stafftype = SlmScr003Biz.GetStaffType(HttpContext.Current.User.Identity.Name);
                        lbUnassignLeadMKT.Text = SlmScr015Biz.GetNumOfUnassignLead(HttpContext.Current.User.Identity.Name, stafftype);
                        lbWaitSum.Text = "จำนวน Lead รอจ่าย";
                        pnlMKT.CssClass = "NoneHidden";
                    }
                }

                if (gvMKT.Rows.Count > 0)
                {
                    GetUserMonitoringMKTList();
                }
                else
                {
                }
                txtUsername.Text = "";
                txtPopupFlag.Text = "";
                cmbBranch.Items.Clear();
                cmbStaff.Items.Clear();
                gvPopMKT.DataSource = null;
                gvPopMKT.DataBind();
                mpePopupMKT.Hide();
                upResult.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbOwnerBranchSelectedIndexChanged();
                if (cmbStaff.SelectedItem.Value != string.Empty && cmbStaff.SelectedItem.Value == string.Empty)
                {
                    vcmbOwner.Text = "กรุณาระบุ owner lead";
                }
                else
                {
                    vcmbOwner.Text = "";
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
            mpePopupMKT.Show();
        }

        private void cmbOwnerBranchSelectedIndexChanged()
        {
            try
            {
                List<ControlListData> source = null;
                source = StaffBiz.GetStaffBranchAndRecursiveList2(cmbBranch.SelectedItem.Value, Page.User.Identity.Name);
                //คำนวณงานในมือ
                AppUtil.CalculateAmountJobOnHandForDropdownlist(cmbBranch.SelectedItem.Value, source);
                cmbStaff.DataSource = source;
                cmbStaff.DataTextField = "TextField";
                cmbStaff.DataValueField = "ValueField";
                cmbStaff.DataBind();
                cmbStaff.Items.Insert(0, new ListItem("", ""));

                if (cmbBranch.SelectedItem.Value != string.Empty)
                    cmbStaff.Enabled = true;
                else
                    cmbStaff.Enabled = false;
            }
            catch
            {
                throw;
            }
        }

        protected void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                txtDateFrom.Text = tmpfrom;
                txtDateTo.Text = tmpto;
                string username = ((Button)sender).CommandArgument;

                txtPopupFlag.Text = "Transfer";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = "ALL";
                List<SearchLeadResult> resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text.Trim(), "ALL");

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                pnlTransferInfo.CssClass = "NoneHidden";

                cmbBranch.DataSource = BranchBiz.GetMonitoringBranchList2(SLMConstant.Branch.Active, HttpContext.Current.User.Identity.Name);
                cmbBranch.DataTextField = "TextField";
                cmbBranch.DataValueField = "ValueField";
                cmbBranch.DataBind();
                cmbBranch.Items.Insert(0, new ListItem("", ""));

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void btnTransferPopup_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbBranch.SelectedItem.Value != "" && cmbStaff.SelectedItem.Value != "")
                {
                    int IsCheck = 0;
                    for (int i = 0; i < gvPopMKT.Rows.Count; i++)
                    {
                        CheckBox chkTransfer = (CheckBox)gvPopMKT.Rows[i].FindControl("chkTransfer");
                        if (chkTransfer.Checked)
                        {
                            IsCheck += 1;
                        }
                    }
                    if (IsCheck == 0)
                        AppUtil.ClientAlert(Page, "กรุณาเลือกข้อมูลผู้มุ่งหวังที่ต้องการโอนงาน");
                    else
                    {
                        List<string> ticketlist = new List<string>();
                        List<string> ticketlistdelegate = new List<string>();
                        List<string> notPassList = new List<string>();

                        for (int i = 0; i < gvPopMKT.Rows.Count; i++)
                        {
                            CheckBox chkTransfer = (CheckBox)gvPopMKT.Rows[i].FindControl("chkTransfer");
                            if (chkTransfer != null && chkTransfer.Checked == true)
                            {
                                //Check Access Right
                                string ticketId = ((Label)gvPopMKT.Rows[i].FindControl("lblTicketId")).Text.Trim();
                                string campaignId = ((Label)gvPopMKT.Rows[i].FindControl("lblCampaignId")).Text.Trim();
                                string TransferType = ((Label)gvPopMKT.Rows[i].FindControl("lblTranferType")).Text.Trim();

                                if (!SlmScr010Biz.PassPrivilegeCampaign(SLMConstant.Branch.Active, campaignId, cmbStaff.SelectedItem.Value))
                                    notPassList.Add(ticketId);
                                else
                                {
                                    if (txtPopupFlag.Text.Trim() == "UnassignLead")
                                        ticketlist.Add(ticketId);
                                    else if (txtPopupFlag.Text.Trim() == "Transfer")
                                    {
                                        if (TransferType == "Owner Lead")
                                            ticketlist.Add(ticketId);
                                        else if (TransferType == "Delegate Lead")
                                            ticketlistdelegate.Add(ticketId);
                                    }
                                }
                            }
                        }
                        if (txtPopupFlag.Text == "UnassignLead")
                            SlmScr018Biz.UpdateTransferOwnerLead(ticketlist, cmbStaff.SelectedItem.Value, int.Parse(txtStaffId.Text.Trim()), HttpContext.Current.User.Identity.Name, cmbBranch.SelectedItem.Value, "");
                        else if (txtPopupFlag.Text == "Transfer")
                        {
                            SlmScr018Biz.UpdateTransferOwnerLead(ticketlist, cmbStaff.SelectedItem.Value, int.Parse(txtStaffId.Text.Trim()), HttpContext.Current.User.Identity.Name, cmbBranch.SelectedItem.Value, txtUsername.Text.Trim().Replace("'", ""));
                            SlmScr018Biz.UpdateTransferDelegateLead(ticketlistdelegate, cmbStaff.SelectedItem.Value, int.Parse(txtStaffId.Text.Trim()), HttpContext.Current.User.Identity.Name, cmbBranch.SelectedItem.Value, txtUsername.Text.Trim().Replace("'", ""));
                        }
                        string alertTicketIdList = "";
                        foreach (string ticketId in notPassList)
                        {
                            alertTicketIdList += (alertTicketIdList != "" ? ", " : "") + ticketId;
                        }

                        if (alertTicketIdList == "")
                            AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อยแล้ว");
                        else
                            AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อยแล้ว โดยมี Ticket Id ที่โอนไม่ได้ดังนี้" + Environment.NewLine + alertTicketIdList);


                        List<SearchLeadResult> resultList = new List<SearchLeadResult>();
                        if (txtPopupFlag.Text.Trim() == "Transfer")
                        {
                            string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
                            string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
                            resultList = SlmScr015Biz.GetUserMonitoringMKTListByUser(cmbProduct.SelectedItem.Value,
                            cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, txtSubStatus.Text.Trim(), tmpfrom, tmpto, txtUsername.Text.Trim(), "ALL");
                            BindGridviewPopupMKT((SLM.Application.Shared.GridviewPageController)pcTop11, resultList.ToArray(), 0);
                        }
                        else if (txtPopupFlag.Text.Trim() == "UnassignLead")
                        {
                            resultList = SlmScr015Biz.SearchUserMonitoringList(HttpContext.Current.User.Identity.Name);
                        }
                        gvPopMKT.DataSource = resultList;
                        gvPopMKT.DataBind();
                        pcTop11.SetGridview(gvPopMKT);
                        pcTop11.Update(resultList.ToArray(), 0);
                        pnlTransferInfo.CssClass = "NoneHidden";
                        cmbBranch.DataSource = BranchBiz.GetMonitoringBranchList2(SLMConstant.Branch.Active, HttpContext.Current.User.Identity.Name);
                        cmbBranch.DataTextField = "TextField";
                        cmbBranch.DataValueField = "ValueField";
                        cmbBranch.DataBind();
                        cmbBranch.Items.Insert(0, new ListItem("", ""));
                        cmbStaff.Items.Clear();
                    }
                }
                else
                    AppUtil.ClientAlert(Page, "กรุณาระบุ Branch และคนที่ถูกโอนงาน");

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void gvPopMKT_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((ImageButton)e.Row.FindControl("imbView")).OnClientClick = "window.open('SLM_SCR_004.aspx?ticketid=" + ((ImageButton)e.Row.FindControl("imbView")).CommandArgument + "'), '_blank'";
                ((ImageButton)e.Row.FindControl("imbEdit")).OnClientClick = "window.open('SLM_SCR_011.aspx?ticketid=" + ((ImageButton)e.Row.FindControl("imbEdit")).CommandArgument + "'), '_blank'";

                //UrlAdams
                ((ImageButton)e.Row.FindControl("imbDoc")).Visible = ((Label)e.Row.FindControl("lblHasAdamUrl")).Text.Trim().ToUpper() == "Y";
            }
        }

        protected void gvPopMKT_DataBound(object sender, EventArgs e)
        {
            try
            {
                if (txtPopupFlag.Text == "Transfer" || txtPopupFlag.Text == "UnassignLead")
                    gvPopMKT.Columns[0].Visible = true;
                else
                    gvPopMKT.Columns[0].Visible = false; ;
                upPopMKT.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void lbUnassignLeadMKT_Click(object sender, EventArgs e)
        {
            try
            {
                txtPopupFlag.Text = "UnassignLead";

                List<SearchLeadResult> resultList = SlmScr015Biz.SearchUserMonitoringList(HttpContext.Current.User.Identity.Name);
                txtUsername.Text = "";
                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                BindGridviewPopupMKT((SLM.Application.Shared.GridviewPageController)pcTop11, resultList.ToArray(), 0);
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                cmbBranch.DataSource = BranchBiz.GetMonitoringBranchList2(SLMConstant.Branch.Active, HttpContext.Current.User.Identity.Name);
                cmbBranch.DataTextField = "TextField";
                cmbBranch.DataValueField = "ValueField";
                cmbBranch.DataBind();
                cmbBranch.Items.Insert(0, new ListItem("", ""));

                pnlTransferInfo.CssClass = "NoneHidden";


                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void gvMKT_DataBound(object sender, EventArgs e)
        {
            if (gvMKT.Rows.Count > 0)
            {
                int amount1 = 0;
                int amount5 = 0;
                int amount6 = 0;
                int amount11 = 0;
                int amount14 = 0;
                int amount15 = 0;
                int sumtotal = 0;

                for (int i = 0; i < gvMKT.Rows.Count; i++)
                {
                    LinkButton lbAmount1 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount1");
                    if (lbAmount1 != null)
                        amount1 += Convert.ToInt16(lbAmount1.Text);

                    LinkButton lbAmount5 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount5");
                    if (lbAmount5 != null)
                        amount5 += Convert.ToInt16(lbAmount5.Text);

                    LinkButton lbAmount6 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount6");
                    if (lbAmount6 != null)
                        amount6 += Convert.ToInt16(lbAmount6.Text);

                    LinkButton lbAmount11 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount11");
                    if (lbAmount11 != null)
                        amount11 += Convert.ToInt16(lbAmount11.Text);

                    LinkButton lbAmount14 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount14");
                    if (lbAmount14 != null)
                        amount14 += Convert.ToInt16(lbAmount14.Text);

                    LinkButton lbAmount15 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount15");
                    if (lbAmount15 != null)
                        amount15 += Convert.ToInt16(lbAmount15.Text);

                    LinkButton lbTotal = (LinkButton)gvMKT.Rows[i].FindControl("lbTotal");
                    if (lbTotal != null)
                        sumtotal += Convert.ToInt16(lbTotal.Text);
                }

                LinkButton lbSumAmount1 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount1");
                if (lbSumAmount1 != null)
                {
                    lbSumAmount1.Text = amount1.ToString();
                    if (lbSumAmount1.Text.Trim() == "0")
                        lbSumAmount1.Enabled = false;
                    else
                        lbSumAmount1.Enabled = true;
                }

                LinkButton lbSumAmount5 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount5");
                if (lbSumAmount5 != null)
                {
                    lbSumAmount5.Text = amount5.ToString();
                    if (lbSumAmount5.Text.Trim() == "0")
                        lbSumAmount5.Enabled = false;
                    else
                        lbSumAmount5.Enabled = true;
                }

                LinkButton lbSumAmount6 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount6");
                if (lbSumAmount6 != null)
                {
                    lbSumAmount6.Text = amount6.ToString();
                    if (lbSumAmount6.Text.Trim() == "0")
                        lbSumAmount6.Enabled = false;
                    else
                        lbSumAmount6.Enabled = true;
                }

                LinkButton lbSumAmount11 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount11");
                if (lbSumAmount11 != null)
                {
                    lbSumAmount11.Text = amount11.ToString();
                    if (lbSumAmount11.Text.Trim() == "0")
                        lbSumAmount11.Enabled = false;
                    else
                        lbSumAmount11.Enabled = true;
                }

                LinkButton lbSumAmount14 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount14");
                if (lbSumAmount14 != null)
                {
                    lbSumAmount14.Text = amount14.ToString();
                    if (lbSumAmount14.Text.Trim() == "0")
                        lbSumAmount14.Enabled = false;
                    else
                        lbSumAmount14.Enabled = true;
                }

                LinkButton lbSumAmount15 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount15");
                if (lbSumAmount15 != null)
                {
                    lbSumAmount15.Text = amount15.ToString();
                    if (lbSumAmount15.Text.Trim() == "0")
                        lbSumAmount15.Enabled = false;
                    else
                        lbSumAmount15.Enabled = true;
                }

                LinkButton lbSumTotal = (LinkButton)gvMKT.FooterRow.FindControl("lbSumTotal");
                if (lbSumTotal != null)
                {
                    lbSumTotal.Text = sumtotal.ToString();
                    if (lbSumTotal.Text.Trim() == "0")
                        lbSumTotal.Enabled = false;
                    else
                        lbSumTotal.Enabled = true;
                }
            }
            upResult.Update();
        }

        protected void gvMKT_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((LinkButton)e.Row.FindControl("lbAmount1")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount1")).Enabled = false;
                else
                    ((LinkButton)e.Row.FindControl("lbAmount1")).Enabled = true;

                if (((LinkButton)e.Row.FindControl("lbAmount15")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount15")).Enabled = false;
                else
                    ((LinkButton)e.Row.FindControl("lbAmount15")).Enabled = true;

                if (((LinkButton)e.Row.FindControl("lbAmount14")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount14")).Enabled = false;
                else
                    ((LinkButton)e.Row.FindControl("lbAmount14")).Enabled = true;


                if (((LinkButton)e.Row.FindControl("lbAmount5")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount5")).Enabled = false;
                else
                    ((LinkButton)e.Row.FindControl("lbAmount5")).Enabled = true;

                if (((LinkButton)e.Row.FindControl("lbAmount6")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount6")).Enabled = false;
                else
                    ((LinkButton)e.Row.FindControl("lbAmount6")).Enabled = true;

                if (((LinkButton)e.Row.FindControl("lbAmount11")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount11")).Enabled = false;
                else
                    ((LinkButton)e.Row.FindControl("lbAmount11")).Enabled = true;

                if (((LinkButton)e.Row.FindControl("lbTotal")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbTotal")).Enabled = false;
                else
                    ((LinkButton)e.Row.FindControl("lbTotal")).Enabled = true;


                if (((Label)e.Row.FindControl("lbUsername")).Text.Trim().ToUpper() == HttpContext.Current.User.Identity.Name.ToUpper())
                    ((Button)e.Row.FindControl("btnTransfer")).Visible = false;
                else
                    ((Button)e.Row.FindControl("btnTransfer")).Visible = true;
            }
        }

        protected void lbDocument_Click(object sender, EventArgs e)
        {
            try
            {
                LeadDataForAdam leadData = SlmScr003Biz.GetLeadDataForAdam(((ImageButton)sender).CommandArgument);
                //convert customerdetail.contactBranch, channelInfo.Branch, to newBranchCode
                leadData.ContactBranch = BranchBiz.GetBranchCodeNew(leadData.ContactBranch);
                leadData.Branch = BranchBiz.GetBranchCodeNew(leadData.Branch);
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "calladam", AppUtil.GetCallAdamScript(leadData, HttpContext.Current.User.Identity.Name, txtEmpCode.Text.Trim(), false, ""), true);

                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbView_Click(object sender, EventArgs e)
        {
            try
            {
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void imbEdit_Click(object sender, EventArgs e)
        {
            try
            {
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
    }
}