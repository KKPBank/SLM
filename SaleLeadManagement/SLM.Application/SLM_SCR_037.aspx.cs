using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using SLM.Biz;
using SLM.Resource.Data;
using SLM.Application.Utilities;
using log4net;
using System.Net;
using SLM.Resource;
using System.Data;
using System.Threading;
using System.Globalization;

namespace SLM.Application
{
    public partial class SLM_SCR_037 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_037));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "User Monitoring Re Insurance";
            Page.Form.DefaultButton = btnSearch.UniqueID;
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

                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_037");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "OBT_SCR_003.aspx");
                        return;
                    }

                    if (SlmScr037Biz.GetStaffType(HttpContext.Current.User.Identity.Name) == null)
                        txtStaffTypeIdLogin.Text = "";
                    else
                        txtStaffTypeIdLogin.Text = SlmScr037Biz.GetStaffType(HttpContext.Current.User.Identity.Name).ToString();

                    InitialControl();

                    tdMKTAssighDateFrom.DateValue = DateTime.Now;
                    tdMKTAssighDateTo.DateValue = DateTime.Now;
                    _log.Debug("Before GetStaffId");

                    //txtStaffId.Text = SlmScr037Biz.GetStaffId(HttpContext.Current.User.Identity.Name);

                    StaffData staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);
                    txtStaffId.Text = staff.StaffId.ToString();
                    txtEmpCode.Text = staff.EmpCode;

                    GetUserMonitoringReInsuranceList();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void InitialControl()
        {
            //ProductGroup
            cmbProductGroup.DataSource = SlmScr037Biz.GetProductGroupData("4");
            cmbProductGroup.DataTextField = "TextField";
            cmbProductGroup.DataValueField = "ValueField";
            cmbProductGroup.DataBind();

            //Product
            //cmbProduct.DataSource = SlmScr037Biz.GetProductData(cmbProductGroup.SelectedValue);
			cmbProduct.DataSource = SlmScr037Biz.GetProductDataNew(cmbProductGroup.SelectedValue);
            cmbProduct.DataTextField = "TextField";
            cmbProduct.DataValueField = "ValueField";
            cmbProduct.DataBind();
            //cmbProduct.Items.Insert(0, new ListItem("", "0"));  //value = 0 ป้องกันในกรณีส่งค่า ช่องว่างไป where ใน CMT_CAMPAIGN_PRODUCT แล้วค่า PR_ProductId บาง record เป็นช่องว่าง

            //แคมเปญ
            //cmbCampaign.DataSource = SlmScr037Biz.GetCampaignData(cmbProductGroup.SelectedValue, cmbProduct.SelectedValue);
			cmbCampaign.DataSource = SlmScr037Biz.GetCampaignDataNew(cmbProductGroup.SelectedValue, cmbProduct.SelectedValue);
            cmbCampaign.DataTextField = "TextField";
            cmbCampaign.DataValueField = "ValueField";
            cmbCampaign.DataBind();
            cmbCampaign.Items.Insert(0, new ListItem("", ""));

            //Teamtelesales
            cmbTeamtelesales.DataSource = SlmScr037Biz.GetAllActiveTeamtelesalesData();
            cmbTeamtelesales.DataTextField = "TextField";
            cmbTeamtelesales.DataValueField = "ValueField";
            cmbTeamtelesales.DataBind();
            cmbTeamtelesales.Items.Insert(0, new ListItem("", ""));

            //option list
            //cbOptionList.DataSource = SlmScr029Biz.GetOptionList(AppConstant.OptionType.LeadStatus);
            //cbOptionList.DataTextField = "TextField";
            //cbOptionList.DataValueField = "ValueField";
            //cbOptionList.DataBind();

            //substatus list
            cbSubOptionList.DataSource = SlmScr037Biz.GetSubOptionList("11", "","16");
            cbSubOptionList.DataTextField = "TextField";
            cbSubOptionList.DataValueField = "ValueField";
            cbSubOptionList.DataBind();

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

        private void BindSubOptionList()
        {
            cbSubOptionList.DataSource = SlmScr037Biz.GetSubOptionList(cmbProduct.SelectedValue.Trim(), cmbCampaign.SelectedValue.Trim(), "16");
            cbSubOptionList.DataTextField = "TextField";
            cbSubOptionList.DataValueField = "ValueField";
            cbSubOptionList.DataBind();
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
                    List<StaffData> staffList = SlmScr037Biz.GetStaffList();
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
                    List<UserMonitoringReInsuranceData> result = SlmScr037Biz.SearchUserMonitoringReInsurance(txtRecursive.Text.Trim(), cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, activeflag, tmpfrom, tmpto, cmbTeamtelesales.SelectedItem.Value, GetSubStatusList());

                    UserMonitoringReInsuranceData resultfirst = result.Where(p => p.Username.ToUpper() == HttpContext.Current.User.Identity.Name.ToUpper()).FirstOrDefault();
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
                _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
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

        private void BindGridviewExport(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvExport);
            pageControl.UpdateMonitoring(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChangePopupMKT(object sender, EventArgs e)
        {
            try
            {
                SearchUserMonitoringCondition data = (SearchUserMonitoringCondition)Session["SLM_SCR_037"];
                data.SubStatusList = txtStatuscode.Text.Trim();
                List<SearchUserMonitoringOBTResult> resultList = new List<SearchUserMonitoringOBTResult>();

                if (txtPopupFlag.Text == "AmountFooter")
                {
                    data.UserList = txtListUsernameFooter.Text.Trim();
                    resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);
                }
                else
                {
                    data.UserList = txtUsername.Text.Trim();
                    resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);
                }

                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridviewPopupMKT(pageControl, resultList.ToArray(), pageControl.SelectedPageIndex);
                mpePopupMKT.Show();

            }
            catch (Exception ex)
            {
                _log.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        #endregion

        public void GetUserMonitoringReInsuranceList()
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
                    List<StaffData> staffList = SlmScr037Biz.GetStaffList();
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

                    List<UserMonitoringReInsuranceData> result = SlmScr037Biz.SearchUserMonitoringReInsurance(txtRecursive.Text.Trim(), cmbProduct.SelectedItem.Value,
                    cmbCampaign.SelectedItem.Value, cmbSearchBranch.SelectedItem.Value, activeflag, tmpfrom, tmpto, cmbTeamtelesales.SelectedValue, GetSubStatusList());

                    Session["SLM_SCR_037"] = GetCondition();

                    UserMonitoringReInsuranceData resultfirst = result.Where(p => p.Username.ToUpper() == HttpContext.Current.User.Identity.Name.ToUpper()).FirstOrDefault();
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
                if ((tdMKTAssighDateFrom.DateValue == DateTime.MinValue && tdMKTAssighDateTo.DateValue != DateTime.MinValue) ||
                    (tdMKTAssighDateFrom.DateValue != DateTime.MinValue && tdMKTAssighDateTo.DateValue == DateTime.MinValue))
                {
                    AppUtil.ClientAlert(this, "กรุณาระบุวันที่จ่ายงานเริ่มต้นและสิ้นสุดให้ครบถ้วน");
                    return;
                }
                GetUserMonitoringReInsuranceList();
                if (gvMKT.Rows.Count > 0)
                {
                    for (int i = 0; i < gvMKT.Rows.Count; i++)
                    {
                        Label lbUsername = (Label)gvMKT.Rows[i].FindControl("lbUsername");
                        txtListUsernameFooter.Text += (txtListUsernameFooter.Text == "" ? "" : ",") + "'" + lbUsername.Text.Trim() + "'";
                    }
                    btnExport.Enabled = true;
                }else                    
                    btnExport.Enabled = false;
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
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.SubStatusCode.OfferSale;

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";
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

        protected void lbAmount2_Click(object sender, EventArgs e)
        {
            try
            {
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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

        protected void lbAmount3_Click(object sender, EventArgs e)
        {
            try
            {
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.SubStatusCode.Choose;

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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

        protected void lbAmount4_Click(object sender, EventArgs e)
        {
            try
            {
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.SubStatusCode.OnProcess;

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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

        protected void lbAmount10_Click(object sender, EventArgs e)
        {
            try
            {
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.SubStatusCode.NoSignal;

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.SubStatusCode.WasteLine;

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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

        protected void lbAmount12_Click(object sender, EventArgs e)
        {
            try
            {
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.SubStatusCode.BusyLine;

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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

        protected void lbAmount13_Click(object sender, EventArgs e)
        {
            try
            {
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.SubStatusCode.WrongNumber;

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "Amount";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = SLMConstant.SubStatusCode.NoRecipient;

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                string username = ((LinkButton)sender).CommandArgument;

                txtPopupFlag.Text = "AmountTotal";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = "ALL";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtUsername.Text;
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.OfferSale + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";
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

        protected void lbSumAmount2_Click(object sender, EventArgs e)
        {
            try
            {
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.ConsultHome + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";
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

        protected void lbSumAmount3_Click(object sender, EventArgs e)
        {
            try
            {
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.Choose + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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

        protected void lbSumAmount4_Click(object sender, EventArgs e)
        {
            try
            {
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.ContactCall + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.OnProcess + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.WasteLine + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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

        protected void lbSumAmount12_Click(object sender, EventArgs e)
        {
            try
            {
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.BusyLine + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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

        protected void lbSumAmount13_Click(object sender, EventArgs e)
        {
            try
            {
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.WrongNumber + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.NoRecipient + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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

        protected void lbSumAmount10_Click(object sender, EventArgs e)
        {
            try
            {
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "'" + SLMConstant.SubStatusCode.NoSignal + "'";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                txtPopupFlag.Text = "AmountFooter";
                txtStatuscode.Text = "ALL";

                SearchUserMonitoringCondition data = GetCondition();
                data.UserList = txtListUsernameFooter.Text.Trim();
                data.SubStatusList = txtStatuscode.Text;

                List<SearchUserMonitoringOBTResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                //pnlTransferInfo.CssClass = "Hidden";

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
                if (gvMKT.Rows.Count > 0)
                {
                    GetUserMonitoringReInsuranceList();
                }
                else
                {
                }
                txtUsername.Text = "";
                txtPopupFlag.Text = "";
                gvPopMKT.DataSource = null;
                gvPopMKT.DataBind();
                mpePopupMKT.Hide();
                upResult.Update();
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
                ((ImageButton)e.Row.FindControl("imbView")).OnClientClick = "window.open('SLM_SCR_004.aspx?preleadid=" + ((ImageButton)e.Row.FindControl("imbView")).CommandArgument + "'), '_blank'";
                ((ImageButton)e.Row.FindControl("imbEdit")).OnClientClick = "window.open('SLM_SCR_046.aspx?preleadid=" + ((ImageButton)e.Row.FindControl("imbEdit")).CommandArgument + "'), '_blank'";

                //UrlAdams
                ((ImageButton)e.Row.FindControl("imbDoc")).Visible = ((Label)e.Row.FindControl("lblHasAdamUrl")).Text.Trim().ToUpper() == "Y";
            }
        }

        protected void gvPopMKT_DataBound(object sender, EventArgs e)
        {
            try
            {
                //if (txtPopupFlag.Text == "Transfer" || txtPopupFlag.Text == "UnassignLead")
                //    gvPopMKT.Columns[0].Visible = true;
                //else
                //    gvPopMKT.Columns[0].Visible = false; ;
                //upPopMKT.Update();
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
                int amount2 = 0;
                int amount3 = 0;
                int amount4 = 0;
                int amount5 = 0;
                int amount10 = 0;
                int amount11 = 0;
                int amount12 = 0;
                int amount13 = 0;
                int amount14 = 0;
                int sumtotal = 0;

                for (int i = 0; i < gvMKT.Rows.Count; i++)
                {
                    LinkButton lbAmount1 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount1");
                    if (lbAmount1 != null)
                        amount1 += Convert.ToInt32(lbAmount1.Text.Trim().Replace(",", ""));

                    LinkButton lbAmount2 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount2");
                    if (lbAmount2 != null)
                        amount2 += Convert.ToInt32(lbAmount2.Text.Trim().Replace(",", ""));

                    LinkButton lbAmount3 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount3");
                    if (lbAmount3 != null)
                        amount3 += Convert.ToInt32(lbAmount3.Text.Trim().Replace(",", ""));

                    LinkButton lbAmount4 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount4");
                    if (lbAmount4 != null)
                        amount4 += Convert.ToInt32(lbAmount4.Text.Trim().Replace(",", ""));

                    LinkButton lbAmount5 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount5");
                    if (lbAmount5 != null)
                        amount5 += Convert.ToInt32(lbAmount5.Text.Trim().Replace(",", ""));

                    LinkButton lbAmount10 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount10");
                    if (lbAmount10 != null)
                        amount10 += Convert.ToInt32(lbAmount10.Text.Trim().Replace(",", ""));

                    LinkButton lbAmount11 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount11");
                    if (lbAmount11 != null)
                        amount11 += Convert.ToInt32(lbAmount11.Text.Trim().Replace(",", ""));

                    LinkButton lbAmount12 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount12");
                    if (lbAmount12 != null)
                        amount12 += Convert.ToInt32(lbAmount12.Text.Trim().Replace(",", ""));

                    LinkButton lbAmount13 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount13");
                    if (lbAmount13 != null)
                        amount13 += Convert.ToInt32(lbAmount13.Text.Trim().Replace(",", ""));

                    LinkButton lbAmount14 = (LinkButton)gvMKT.Rows[i].FindControl("lbAmount14");
                    if (lbAmount14 != null)
                        amount14 += Convert.ToInt32(lbAmount14.Text.Trim().Replace(",", ""));

                    LinkButton lbTotal = (LinkButton)gvMKT.Rows[i].FindControl("lbTotal");
                    if (lbTotal != null)
                        sumtotal += Convert.ToInt32(lbTotal.Text.Trim().Replace(",", ""));
                }

                LinkButton lbSumAmount1 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount1");
                if (lbSumAmount1 != null)
                {
                    lbSumAmount1.Text = amount1.ToString("#,##0");
                    if (lbSumAmount1.Text.Trim() == "0")
                        lbSumAmount1.Enabled = false;
                    else
                        lbSumAmount1.Enabled = true;
                }

                LinkButton lbSumAmount2 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount2");
                if (lbSumAmount2 != null)
                {
                    lbSumAmount2.Text = amount2.ToString("#,##0");
                    if (lbSumAmount2.Text.Trim() == "0")
                        lbSumAmount2.Enabled = false;
                    else
                        lbSumAmount2.Enabled = true;
                }

                LinkButton lbSumAmount3 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount3");
                if (lbSumAmount3 != null)
                {
                    lbSumAmount3.Text = amount3.ToString("#,##0");
                    if (lbSumAmount3.Text.Trim() == "0")
                        lbSumAmount3.Enabled = false;
                    else
                        lbSumAmount3.Enabled = true;
                }

                LinkButton lbSumAmount4 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount4");
                if (lbSumAmount4 != null)
                {
                    lbSumAmount4.Text = amount4.ToString("#,##0");
                    if (lbSumAmount4.Text.Trim() == "0")
                        lbSumAmount4.Enabled = false;
                    else
                        lbSumAmount4.Enabled = true;
                }

                LinkButton lbSumAmount5 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount5");
                if (lbSumAmount5 != null)
                {
                    lbSumAmount5.Text = amount5.ToString("#,##0");
                    if (lbSumAmount5.Text.Trim() == "0")
                        lbSumAmount5.Enabled = false;
                    else
                        lbSumAmount5.Enabled = true;
                }

                LinkButton lbSumAmount10 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount10");
                if (lbSumAmount10 != null)
                {
                    lbSumAmount10.Text = amount10.ToString("#,##0");
                    if (lbSumAmount10.Text.Trim() == "0")
                        lbSumAmount10.Enabled = false;
                    else
                        lbSumAmount10.Enabled = true;
                }

                LinkButton lbSumAmount11 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount11");
                if (lbSumAmount11 != null)
                {
                    lbSumAmount11.Text = amount11.ToString("#,##0");
                    if (lbSumAmount11.Text.Trim() == "0")
                        lbSumAmount11.Enabled = false;
                    else
                        lbSumAmount11.Enabled = true;
                }

                LinkButton lbSumAmount12 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount12");
                if (lbSumAmount12 != null)
                {
                    lbSumAmount12.Text = amount12.ToString("#,##0");
                    if (lbSumAmount12.Text.Trim() == "0")
                        lbSumAmount12.Enabled = false;
                    else
                        lbSumAmount12.Enabled = true;
                }

                LinkButton lbSumAmount13 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount13");
                if (lbSumAmount13 != null)
                {
                    lbSumAmount13.Text = amount13.ToString("#,##0");
                    if (lbSumAmount13.Text.Trim() == "0")
                        lbSumAmount13.Enabled = false;
                    else
                        lbSumAmount13.Enabled = true;
                }

                LinkButton lbSumAmount14 = (LinkButton)gvMKT.FooterRow.FindControl("lbSumAmount14");
                if (lbSumAmount14 != null)
                {
                    lbSumAmount14.Text = amount14.ToString("#,##0");
                    if (lbSumAmount14.Text.Trim() == "0")
                        lbSumAmount14.Enabled = false;
                    else
                        lbSumAmount14.Enabled = true;
                }

                LinkButton lbSumTotal = (LinkButton)gvMKT.FooterRow.FindControl("lbSumTotal");
                if (lbSumTotal != null)
                {
                    lbSumTotal.Text = sumtotal.ToString("#,##0");
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
                {
                    ((LinkButton)e.Row.FindControl("lbAmount1")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount1")).OnClientClick = "DisplayProcessing();";
                }

                if (((LinkButton)e.Row.FindControl("lbAmount2")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount2")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbAmount2")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount2")).OnClientClick = "DisplayProcessing();";
                }

                if (((LinkButton)e.Row.FindControl("lbAmount3")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount3")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbAmount3")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount3")).OnClientClick = "DisplayProcessing();";
                }


                if (((LinkButton)e.Row.FindControl("lbAmount4")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount4")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbAmount4")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount4")).OnClientClick = "DisplayProcessing();";
                }

                if (((LinkButton)e.Row.FindControl("lbAmount5")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount5")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbAmount5")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount5")).OnClientClick = "DisplayProcessing();";
                }

                if (((LinkButton)e.Row.FindControl("lbAmount10")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount10")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbAmount10")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount10")).OnClientClick = "DisplayProcessing();";
                }

                if (((LinkButton)e.Row.FindControl("lbAmount11")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount11")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbAmount11")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount11")).OnClientClick = "DisplayProcessing();";
                }

                if (((LinkButton)e.Row.FindControl("lbAmount12")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount12")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbAmount12")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount12")).OnClientClick = "DisplayProcessing();";
                }

                if (((LinkButton)e.Row.FindControl("lbAmount13")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount13")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbAmount13")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount13")).OnClientClick = "DisplayProcessing();";
                }

                if (((LinkButton)e.Row.FindControl("lbAmount14")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbAmount14")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbAmount14")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbAmount14")).OnClientClick = "DisplayProcessing();";
                }

                if (((LinkButton)e.Row.FindControl("lbTotal")).Text.Trim() == "0")
                    ((LinkButton)e.Row.FindControl("lbTotal")).Enabled = false;
                else
                {
                    ((LinkButton)e.Row.FindControl("lbTotal")).Enabled = true;
                    ((LinkButton)e.Row.FindControl("lbTotal")).OnClientClick = "DisplayProcessing();";
                }


                //if (((Label)e.Row.FindControl("lbUsername")).Text.Trim().ToUpper() == HttpContext.Current.User.Identity.Name.ToUpper())
                //    ((Button)e.Row.FindControl("btnTransfer")).Visible = false;
                //else
                //    ((Button)e.Row.FindControl("btnTransfer")).Visible = true;
            }
        }

        protected void gvExport_DataBound(object sender, EventArgs e)
        {
            //if (gvExport.Rows.Count > 0)
            //{
            //    int amount1 = 0;
            //    int amount2 = 0;
            //    int amount3 = 0;
            //    int amount4 = 0;
            //    int amount5 = 0;
            //    int amount10 = 0;
            //    int amount11 = 0;
            //    int amount12 = 0;
            //    int amount13 = 0;
            //    int amount14 = 0;
            //    int sumtotal = 0;

            //    for (int i = 0; i < gvExport.Rows.Count; i++)
            //    {
            //        LinkButton lbAmount1 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount1");
            //        if (lbAmount1 != null)
            //            amount1 += Convert.ToInt16(lbAmount1.Text);

            //        LinkButton lbAmount2 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount2");
            //        if (lbAmount2 != null)
            //            amount2 += Convert.ToInt16(lbAmount2.Text);

            //        LinkButton lbAmount3 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount3");
            //        if (lbAmount3 != null)
            //            amount3 += Convert.ToInt16(lbAmount3.Text);

            //        LinkButton lbAmount4 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount4");
            //        if (lbAmount4 != null)
            //            amount4 += Convert.ToInt16(lbAmount4.Text);

            //        LinkButton lbAmount5 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount5");
            //        if (lbAmount5 != null)
            //            amount5 += Convert.ToInt16(lbAmount5.Text);

            //        LinkButton lbAmount10 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount10");
            //        if (lbAmount10 != null)
            //            amount10 += Convert.ToInt16(lbAmount10.Text);

            //        LinkButton lbAmount11 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount11");
            //        if (lbAmount11 != null)
            //            amount11 += Convert.ToInt16(lbAmount11.Text);

            //        LinkButton lbAmount12 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount12");
            //        if (lbAmount12 != null)
            //            amount12 += Convert.ToInt16(lbAmount12.Text);

            //        LinkButton lbAmount13 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount13");
            //        if (lbAmount13 != null)
            //            amount13 += Convert.ToInt16(lbAmount13.Text);

            //        LinkButton lbAmount14 = (LinkButton)gvExport.Rows[i].FindControl("lbAmount14");
            //        if (lbAmount14 != null)
            //            amount14 += Convert.ToInt16(lbAmount14.Text);

            //        LinkButton lbTotal = (LinkButton)gvExport.Rows[i].FindControl("lbTotal");
            //        if (lbTotal != null)
            //            sumtotal += Convert.ToInt16(lbTotal.Text);


            //        LinkButton lbSumAmount1 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount1");
            //        if (lbSumAmount1 != null)
            //        {
            //            lbSumAmount1.Text = amount1.ToString();
            //            if (lbSumAmount1.Text.Trim() == "0")
            //                lbSumAmount1.Enabled = false;
            //            else
            //                lbSumAmount1.Enabled = true;
            //        }

            //        LinkButton lbSumAmount2 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount2");
            //        if (lbSumAmount2 != null)
            //        {
            //            lbSumAmount2.Text = amount2.ToString();
            //            if (lbSumAmount2.Text.Trim() == "0")
            //                lbSumAmount2.Enabled = false;
            //            else
            //                lbSumAmount2.Enabled = true;
            //        }

            //        LinkButton lbSumAmount3 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount3");
            //        if (lbSumAmount3 != null)
            //        {
            //            lbSumAmount3.Text = amount3.ToString();
            //            if (lbSumAmount3.Text.Trim() == "0")
            //                lbSumAmount3.Enabled = false;
            //            else
            //                lbSumAmount3.Enabled = true;
            //        }

            //        LinkButton lbSumAmount4 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount4");
            //        if (lbSumAmount4 != null)
            //        {
            //            lbSumAmount4.Text = amount4.ToString();
            //            if (lbSumAmount4.Text.Trim() == "0")
            //                lbSumAmount4.Enabled = false;
            //            else
            //                lbSumAmount4.Enabled = true;
            //        }

            //        LinkButton lbSumAmount5 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount5");
            //        if (lbSumAmount5 != null)
            //        {
            //            lbSumAmount5.Text = amount5.ToString();
            //            if (lbSumAmount5.Text.Trim() == "0")
            //                lbSumAmount5.Enabled = false;
            //            else
            //                lbSumAmount5.Enabled = true;
            //        }

            //        LinkButton lbSumAmount10 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount10");
            //        if (lbSumAmount10 != null)
            //        {
            //            lbSumAmount10.Text = amount10.ToString();
            //            if (lbSumAmount10.Text.Trim() == "0")
            //                lbSumAmount10.Enabled = false;
            //            else
            //                lbSumAmount10.Enabled = true;
            //        }

            //        LinkButton lbSumAmount11 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount11");
            //        if (lbSumAmount11 != null)
            //        {
            //            lbSumAmount11.Text = amount11.ToString();
            //            if (lbSumAmount11.Text.Trim() == "0")
            //                lbSumAmount11.Enabled = false;
            //            else
            //                lbSumAmount11.Enabled = true;
            //        }

            //        LinkButton lbSumAmount12 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount12");
            //        if (lbSumAmount12 != null)
            //        {
            //            lbSumAmount12.Text = amount12.ToString();
            //            if (lbSumAmount12.Text.Trim() == "0")
            //                lbSumAmount12.Enabled = false;
            //            else
            //                lbSumAmount12.Enabled = true;
            //        }

            //        LinkButton lbSumAmount13 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount13");
            //        if (lbSumAmount13 != null)
            //        {
            //            lbSumAmount13.Text = amount13.ToString();
            //            if (lbSumAmount13.Text.Trim() == "0")
            //                lbSumAmount13.Enabled = false;
            //            else
            //                lbSumAmount13.Enabled = true;
            //        }

            //        LinkButton lbSumAmount14 = (LinkButton)gvExport.FooterRow.FindControl("lbSumAmount14");
            //        if (lbSumAmount14 != null)
            //        {
            //            lbSumAmount14.Text = amount14.ToString();
            //            if (lbSumAmount14.Text.Trim() == "0")
            //                lbSumAmount14.Enabled = false;
            //            else
            //                lbSumAmount14.Enabled = true;
            //        }

            //        LinkButton lbSumTotal = (LinkButton)gvExport.FooterRow.FindControl("lbSumTotal");
            //        if (lbSumTotal != null)
            //        {
            //            lbSumTotal.Text = sumtotal.ToString();
            //            if (lbSumTotal.Text.Trim() == "0")
            //                lbSumTotal.Enabled = false;
            //            else
            //                lbSumTotal.Enabled = true;
            //        }
            //    }
            //}
            //upResult.Update();
        }

        protected void lbDocument_Click(object sender, EventArgs e)
        {
            try
            {
                LeadDataForAdam leadData = SlmScr003Biz.GetLeadDataForAdam(((ImageButton)sender).CommandArgument);
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "calladam", AppUtil.GetCallAdamScript(leadData, HttpContext.Current.User.Identity.Name, txtEmpCode.Text.Trim(), false, ""), true);

                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
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

        protected void cbSubOptionAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSubOptionAll.Checked)
            {
                foreach (ListItem li in cbSubOptionList.Items)
                {
                    li.Selected = true;
                }
            }
            else
            {
                foreach (ListItem li in cbSubOptionList.Items)
                {
                    li.Selected = false;
                }
            }
        }

        private void CheckAllSubCondition()
        {
            int count = 0;
            foreach (ListItem li in cbSubOptionList.Items)
            {
                if (!li.Selected) { count += 1; }
            }

            cbSubOptionAll.Checked = count > 0 ? false : true;
        }

        protected void chkExportAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ChkBoxHeader = (CheckBox)gvMKT.HeaderRow.FindControl("chkExportAll");
            foreach (GridViewRow row in gvMKT.Rows)
            {
                CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkExport");
                if (ChkBoxHeader.Checked == true)
                {
                    ChkBoxRows.Checked = true;
                }
                else
                {
                    ChkBoxRows.Checked = false;
                }
            }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SLM_SCR_058.aspx");
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                int IsCheck = 0;
                for (int i = 0; i < gvMKT.Rows.Count; i++)
                {
                    CheckBox chkExport = (CheckBox)gvMKT.Rows[i].FindControl("chkExport");
                    if (chkExport.Checked)
                    {
                        IsCheck += 1;
                    }
                }
                if (IsCheck == 0)
                    AppUtil.ClientAlert(Page, "กรุณาเลือกข้อมูลที่ต้องการ Export Excel");
                else
                {
                    string date = DateTime.Now.Year.ToString() + DateTime.Now.ToString("MMdd_HHmmss");
                    var filename = Path.Combine(System.IO.Path.GetTempPath(), "tmpexcel_037_" + Page.User.Identity.Name + date + ".xls");

                    string userList = "";
                    for (int i = 0; i < gvMKT.Rows.Count; i++)
                    {
                        CheckBox chkExport = (CheckBox)gvMKT.Rows[i].FindControl("chkExport");
                        if (chkExport != null && chkExport.Checked == true)
                        {
                            //Check Access Right
                            string userName = ((Label)gvMKT.Rows[i].FindControl("lbUsername")).Text.Trim();
                            userList += (userList == "" ? "" : ",") + "'" + userName.Trim() + "'";
                        }
                    }
                    SearchUserMonitoringCondition crit = (SearchUserMonitoringCondition)Session["SLM_SCR_037"];
                    crit.UserList = userList;

                    SlmScr037Biz.CreateExcel(crit, filename);
                    Session["excelfilepath"] = filename;
                    Session["outputfilename"] = "userMonitoringReInsurance.xls";

                    string script = "window.open('SLM_SCR_045.aspx', 'userMonitoringReInsurance', 'status=yes, toolbar=no, scrollbars=no, menubar=no, width=300, height=100, resizable=yes');";
                    ScriptManager.RegisterStartupScript(Page, GetType(), "userMonitoringReInsurance", script, true);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        //protected void btnExport_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        int IsCheck = 0;
        //        for (int i = 0; i < gvMKT.Rows.Count; i++)
        //        {
        //            CheckBox chkExport = (CheckBox)gvMKT.Rows[i].FindControl("chkExport");
        //            if (chkExport.Checked)
        //            {
        //                IsCheck += 1;
        //            }
        //        }
        //        if (IsCheck == 0)
        //            AppUtil.ClientAlert(Page, "กรุณาเลือกข้อมูลที่ต้องการ Export Excel");
        //        else
        //        {
        //            string userList = "";
        //            for (int i = 0; i < gvMKT.Rows.Count; i++)
        //            {
        //                CheckBox chkExport = (CheckBox)gvMKT.Rows[i].FindControl("chkExport");
        //                if (chkExport != null && chkExport.Checked == true)
        //                {
        //                    //Check Access Right
        //                    string userName = ((Label)gvMKT.Rows[i].FindControl("lbUsername")).Text.Trim();
        //                    userList += (userList == "" ? "" : ",") + "'" + userName.Trim() + "'";
        //                }
        //            }
        //            SearchUserMonitoringCondition crit = (SearchUserMonitoringCondition)Session["SLM_SCR_037"];
        //            crit.UserList = userList;

        //            List<SearchUserMonitoringOBTResult> result = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(crit);

        //            BindGridviewExport((SLM.Application.Shared.GridviewPageController)pcTop12, result.ToArray(), 0);

        //            //ExportGridToExcel();

        //            //System.Data.DataTable dt = getGridInfo();
        //            DataSet dset = new DataSet();
        //            dset.Tables.Add();
        //            //First Add Columns from gridview to excel
        //            for (int i = 0; i < gvExport.Columns.Count; i++) //GridView is id of gridview
        //            {
        //                dset.Tables[0].Columns.Add(gvExport.Columns[i].HeaderText);
        //            }
        //            //add rows to the table 
        //            System.Data.DataRow dr;
        //            foreach (GridViewRow row in gvExport.Rows)
        //            {
        //                dr = dset.Tables[0].NewRow(); //For Example There are only 3 columns into gridview
        //                Label lblPreleadId = new Label();
        //                Label lblTranferType = new Label();
        //                Label lblOwnerName = new Label();
        //                Label lblStatusDesc = new Label();
        //                Label lblCounting = new Label();
        //                Label lblCampaignName = new Label();
        //                Label lblCreatedDate = new Label();
        //                Label lblAssignedDate = new Label();
        //                Label lblFirstname = new Label();
        //                Label lblLastname = new Label();
        //                Label lblCardTypeDesc = new Label();
        //                Label lblCITIZENID = new Label();
        //                Label lblCampaignId = new Label();
        //                Label lblHasAdamUrl = new Label();
        //                Label lblTEAMCODE = new Label();
        //                Label lblEMPCODE = new Label();
        //                Label lblSubStatusDesc = new Label();
        //                Label lblContractNo = new Label();
        //                Label lblCusFullName = new Label();
        //                Label lblINSNAME = new Label();
        //                Label lblCOV_NAME = new Label();
        //                Label lblGROSS_PREMIUM = new Label();
        //                Label lblGrade = new Label();
        //                Label lblLot = new Label();

        //                //if (row.RowType == DataControlRowType.DataRow)
        //                //{
        //                lblPreleadId = (Label)row.FindControl("lblPreleadId");
        //                lblTranferType = (Label)row.FindControl("lblTranferType");
        //                lblOwnerName = (Label)row.FindControl("lblOwnerName");
        //                lblStatusDesc = (Label)row.FindControl("lblStatusDesc");
        //                lblCounting = (Label)row.FindControl("lblCounting");
        //                lblCampaignName = (Label)row.FindControl("lblCampaignName");
        //                lblCreatedDate = (Label)row.FindControl("lblCreatedDate");
        //                lblAssignedDate = (Label)row.FindControl("lblAssignedDate");
        //                lblFirstname = (Label)row.FindControl("lblFirstname");
        //                lblLastname = (Label)row.FindControl("lblLastname");
        //                lblCardTypeDesc = (Label)row.FindControl("lblCardTypeDesc");
        //                lblCITIZENID = (Label)row.FindControl("lblCITIZENID");
        //                lblCampaignId = (Label)row.FindControl("lblCampaignId");
        //                lblHasAdamUrl = (Label)row.FindControl("lblHasAdamUrl");
        //                lblTEAMCODE = (Label)row.FindControl("lblTEAMCODE");
        //                lblEMPCODE = (Label)row.FindControl("lblEMPCODE");
        //                lblSubStatusDesc = (Label)row.FindControl("lblSubStatusDesc");
        //                lblContractNo = (Label)row.FindControl("lblContractNo");
        //                lblCusFullName = (Label)row.FindControl("lblCusFullName");
        //                lblINSNAME = (Label)row.FindControl("lblINSNAME");
        //                lblCOV_NAME = (Label)row.FindControl("lblCOV_NAME");
        //                lblGROSS_PREMIUM = (Label)row.FindControl("lblGROSS_PREMIUM");
        //                lblGrade = (Label)row.FindControl("lblGrade");
        //                lblLot = (Label)row.FindControl("lblLot");
        //                //}
        //                //else
        //                //{
        //                //    lbRoleName = (Label)row.FindControl("lbRoleName");
        //                //    lbUsername = (Label)row.FindControl("lbUsername");
        //                //    lbFullnameTH = (Label)row.FindControl("lbFullnameTH");
        //                //    lbAmount1 = (Label)row.FindControl("lbSumAmount10");
        //                //    lbAmount2 = (Label)row.FindControl("lbSumAmount2");
        //                //    lbAmount3 = (Label)row.FindControl("lbSumAmount3");
        //                //    lbAmount4 = (Label)row.FindControl("lbSumAmount4");
        //                //    lbAmount5 = (Label)row.FindControl("lbSumAmount5");
        //                //    lbAmount10 = (Label)row.FindControl("lbSumAmount10");
        //                //    lbTotal = (Label)row.FindControl("lbSumTotal");
        //                //}
        //                dr[0] = lblTEAMCODE.Text;
        //                dr[1] = lblEMPCODE.Text;
        //                dr[2] = lblStatusDesc.Text;
        //                dr[3] = lblSubStatusDesc.Text;
        //                dr[4] = lblContractNo.Text;
        //                dr[5] = lblCusFullName.Text;
        //                dr[6] = lblINSNAME.Text;
        //                dr[7] = lblCOV_NAME.Text;
        //                dr[8] = lblGROSS_PREMIUM.Text;
        //                dr[9] = lblCreatedDate.Text;
        //                dr[10] = lblGrade.Text;
        //                dr[11] = lblPreleadId.Text;
        //                dr[12] = lblLot.Text;

        //                dset.Tables[0].Rows.Add(dr);
        //            }
        //            DataTable dt = dset.Tables[0];

        //            if (dt.Rows.Count > 0)
        //            {
        //                Session[SLM.Resource.SLMConstant.SessionExport] = dt;
        //                ScriptManager.RegisterClientScriptBlock(upResult, upResult.GetType(), "popup", "window.open('DownloadFile.ashx?val=userMonitoringReInsurance');", true);
        //            }
        //            else
        //            {
        //                AppUtil.ClientAlert(Page, "ไม่พบข้อมูลที่จะทำการ Export Excel");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        protected SearchUserMonitoringCondition GetCondition()
        {

            //string userList = "";

            //for (int i = 0; i < gvMKT.Rows.Count; i++)
            //{
            //    CheckBox chkExport = (CheckBox)gvMKT.Rows[i].FindControl("chkExport");
            //    if (chkExport != null && chkExport.Checked == true)
            //    {
            //        //Check Access Right
            //        string userName = ((Label)gvMKT.Rows[i].FindControl("lbUsername")).Text.Trim();
            //        userList += (userList == "" ? "" : ",") + "'" + userName.Trim() + "'";
            //    }
            //}

            //string tmpfrom = tdMKTAssighDateFrom.DateValue.Year.ToString() + tdMKTAssighDateFrom.DateValue.ToString("-MM-dd");
            //string tmpto = tdMKTAssighDateTo.DateValue.Year.ToString() + tdMKTAssighDateTo.DateValue.ToString("-MM-dd");
            //txtDateFrom.Text = tmpfrom;
            //txtDateTo.Text = tmpto;

            //string activeflag = "";
            //if (cmbStatusMKT.SelectedItem.Value == "")
            //    activeflag = "'0','1'";
            //else
            //    activeflag = cmbStatusMKT.SelectedItem.Value;

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
                List<StaffData> staffList = SlmScr037Biz.GetStaffList();
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

            return new SearchUserMonitoringCondition
            {
                Active = activeflag,
                AssignDateFrom = tmpfrom,
                AssignDateTo = tmpto,
                Branchcode = cmbSearchBranch.SelectedItem.Value,
                Campaign = cmbCampaign.SelectedItem.Value,
                ProductId = cmbProduct.SelectedItem.Value,
                SubStatusList = GetSubStatusList(),
                Teamtelesales = cmbTeamtelesales.SelectedValue,
                UserList = txtRecursive.Text.Trim()
            };
        }

        private string GetSubStatusList()
        {
            string list = string.Empty;
            foreach (ListItem li in cbSubOptionList.Items)
            {
                if (li.Selected)
                    list += (list == string.Empty ? "" : ",") + "'" + li.Value + "'";
            }
            return list;
        }

        protected void cmbCampaign_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubOptionList();
        }

        protected void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindSubOptionList();
                cmbProductSelectedIndexChanged();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void cmbProductSelectedIndexChanged()
        {
            //แคมเปญ
            cmbCampaign.DataSource = SlmScr037Biz.GetCampaignData(cmbProductGroup.SelectedValue, cmbProduct.SelectedValue);
            cmbCampaign.DataTextField = "TextField";
            cmbCampaign.DataValueField = "ValueField";
            cmbCampaign.DataBind();
            cmbCampaign.Items.Insert(0, new ListItem("", ""));
        }

        /* โอนงาน
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
                AppUtil.ClientAlert(Page, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            mpePopupMKT.Show();
        }

        private void cmbOwnerBranchSelectedIndexChanged()
        {
            try
            {
                List<ControlListData> source = null;
                source = StaffBiz.GetStaffBranchAndRecursiveList(cmbBranch.SelectedItem.Value, txtRecursive.Text.Trim());
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
         * 
         * protected void btnTransferPopup_Click(object sender, EventArgs e)
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
                        List<Decimal> preleadlist = new List<Decimal>();
                        List<Decimal> notPassList = new List<Decimal>();

                        for (int i = 0; i < gvPopMKT.Rows.Count; i++)
                        {
                            CheckBox chkTransfer = (CheckBox)gvPopMKT.Rows[i].FindControl("chkTransfer");
                            if (chkTransfer != null && chkTransfer.Checked == true)
                            {
                                //Check Access Right
                                Decimal preleadId = Decimal.Parse(((Label)gvPopMKT.Rows[i].FindControl("lblPreleadId")).Text.ToString());
                                string campaignId = ((Label)gvPopMKT.Rows[i].FindControl("lblCampaignId")).Text.Trim();
                                string TransferType = ((Label)gvPopMKT.Rows[i].FindControl("lblTranferType")).Text.Trim();

                                if (!SlmScr010Biz.PassPrivilegeCampaign(SLMConstant.Branch.Active, campaignId, cmbStaff.SelectedItem.Value))
                                    notPassList.Add(preleadId);
                                else
                                {
                                    if (txtPopupFlag.Text.Trim() == "UnassignLead")
                                        preleadlist.Add(preleadId);
                                    else if (txtPopupFlag.Text.Trim() == "Transfer")
                                    {
                                        if (TransferType == "Owner Prelead")
                                            preleadlist.Add(preleadId);
                                    }
                                }
                            }
                        }
                        if (txtPopupFlag.Text == "UnassignLead")
                            SlmScr037Biz.UpdateTransferOwnerPrelead(preleadlist, cmbStaff.SelectedItem.Value, int.Parse(txtStaffId.Text.Trim()), HttpContext.Current.User.Identity.Name, cmbBranch.SelectedItem.Value, "");
                        else if (txtPopupFlag.Text == "Transfer")
                        {
                            SlmScr037Biz.UpdateTransferOwnerPrelead(preleadlist, cmbStaff.SelectedItem.Value, int.Parse(txtStaffId.Text.Trim()), HttpContext.Current.User.Identity.Name, cmbBranch.SelectedItem.Value, txtUsername.Text.Trim().Replace("'", ""));
                        }
                        string alertPreleadIdList = "";
                        foreach (decimal preleadId in notPassList)
                        {
                            alertPreleadIdList += (alertPreleadIdList != "" ? ", " : "") + preleadId;
                        }

                        if (alertPreleadIdList == "")
                            AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อยแล้ว");
                        else
                            AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อยแล้ว โดยมี Prelead Id ที่โอนไม่ได้ดังนี้" + Environment.NewLine + alertPreleadIdList);


                        List<SearchObtResult> resultList = new List<SearchObtResult>();
                        if (txtPopupFlag.Text.Trim() == "Transfer")
                        {
                            SearchUserMonitoringCondition data = GetCondition();
                            data.SubStatusList = "ALL";
                            resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);
                            BindGridviewPopupMKT((SLM.Application.Shared.GridviewPageController)pcTop11, resultList.ToArray(), 0);
                        }
                        else if (txtPopupFlag.Text.Trim() == "UnassignLead")
                        {
                            resultList = SlmScr037Biz.SearchUserMonitoringList(HttpContext.Current.User.Identity.Name);
                        }
                        gvPopMKT.DataSource = resultList;
                        gvPopMKT.DataBind();
                        pcTop11.SetGridview(gvPopMKT);
                        pcTop11.Update(resultList.ToArray(), 0);
                        pnlTransferInfo.CssClass = "NoneHidden";
                        cmbBranch.DataSource = BranchBiz.GetMonitoringBranchList(SLMConstant.Branch.Active, HttpContext.Current.User.Identity.Name);
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
                throw ex;
            }
        }
         * 
        protected void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                string username = ((Button)sender).CommandArgument;

                txtPopupFlag.Text = "Transfer";
                txtUsername.Text = "'" + username + "'";
                txtStatuscode.Text = "ALL";

                SearchUserMonitoringCondition data = GetCondition();
                data.SubStatusList = txtStatuscode.Text;
                data.UserList = txtUsername.Text;

                List<SearchObtResult> resultList = SlmScr037Biz.GetUserMonitoringReInsuranceListByUser(data);

                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        protected void lbUnassignLeadMKT_Click(object sender, EventArgs e)
        {
            try
            {
                txtPopupFlag.Text = "UnassignLead";

                List<SearchObtResult> resultList = SlmScr037Biz.SearchUserMonitoringList(HttpContext.Current.User.Identity.Name);
                txtUsername.Text = "";
                gvPopMKT.DataSource = resultList;
                gvPopMKT.DataBind();
                BindGridviewPopupMKT((SLM.Application.Shared.GridviewPageController)pcTop11, resultList.ToArray(), 0);
                pcTop11.SetGridview(gvPopMKT);
                pcTop11.Update(resultList.ToArray(), 0);

                upPopMKT.Update();
                mpePopupMKT.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        */

    }
}