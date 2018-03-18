using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;
using System.Configuration;
using SLM.Application.CSMBranchService;
using SLM.Resource;
namespace SLM.Application
{
    public partial class SLM_SCR_028 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_028));
        private bool useWebservice = Convert.ToBoolean(ConfigurationManager.AppSettings["UseWebservice"]);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาข้อมูลสาขา";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_028");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    Page.Form.DefaultButton = btnSearch.UniqueID;
                    InitialControl();
                    DoSearchBranch(0);
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
            AppUtil.SetIntTextBox(txtWorkStartHourPopup);
            AppUtil.SetIntTextBox(txtWorkStartMinPopup);
            AppUtil.SetIntTextBox(txtWorkEndHourPopup);
            AppUtil.SetIntTextBox(txtWorkEndMinPopup);
            AppUtil.SetTimeControlScript(txtWorkStartHourPopup, txtWorkStartMinPopup);
            AppUtil.SetTimeControlScript(txtWorkEndHourPopup, txtWorkEndMinPopup);

            //Search
            cmbChannelSearch.DataSource = ChannelBiz.GetChannelList();
            cmbChannelSearch.DataTextField = "TextField";
            cmbChannelSearch.DataValueField = "ValueField";
            cmbChannelSearch.DataBind();
            cmbChannelSearch.Items.Insert(0, new ListItem("", ""));

            //Popup
            cmbChannelPopup.DataSource = ChannelBiz.GetChannelList();
            cmbChannelPopup.DataTextField = "TextField";
            cmbChannelPopup.DataValueField = "ValueField";
            cmbChannelPopup.DataBind();
            cmbChannelPopup.Items.Insert(0, new ListItem("", ""));

			//UpperBranch
            cmbUpperBranchPopup.DataSource = BranchBiz.GetBranchList(1);
            cmbUpperBranchPopup.DataTextField = "TextField";
            cmbUpperBranchPopup.DataValueField = "ValueField";
            cmbUpperBranchPopup.DataBind();
            cmbUpperBranchPopup.Items.Insert(0, new ListItem("", ""));
			
            //จังหวัด
            cmbProvince.DataSource = SlmScr010Biz.GetProvinceDataNew();
            cmbProvince.DataTextField = "TextField";
            cmbProvince.DataValueField = "ValueField";
            cmbProvince.DataBind();
            cmbProvince.Items.Insert(0, new ListItem("", ""));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchBranch(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchBranch(int pageIndex)
        {
            try
            {
                List<BranchData> list = BranchBiz.SearchBranch(txtBranchCodeSearch.Text.Trim(), txtBranchNameSearch.Text.Trim(), cmbChannelSearch.SelectedItem.Value, cbActiveSearch.Checked, cbInActiveSearch.Checked);
                BindGridview(pcTop, list.ToArray(), pageIndex);
            }
            catch
            {
                throw;
            }
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex);
            pageControl.GenerateRecordNumber(1, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchBranch(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        #region Popup

        protected void btnAddBranch_Click(object sender, EventArgs e)
        {
            try
            {
				BuildUpperBranchCreate();
                trIdInfo.Visible = false;
                cbEdit.Checked = false;
                txtBranchCodePopup.Text = "";
                txtBranchCodePopup.Enabled = true;
                txtBranchNamePopup.Text = "";
                cmbChannelPopup.SelectedIndex = -1;
                txtWorkStartHourPopup.Text = "";
                txtWorkStartMinPopup.Text = "";
                txtWorkEndHourPopup.Text = "";
                txtWorkEndMinPopup.Text = "";
				cmbUpperBranchPopup.Text = "";
                rbActive.Checked = true;
                rbInActive.Checked = false;

                // addres
                cmbProvince.SelectedIndex = 0;
                BuildAmphur();
                BuildTambol();
                txtAddressNo.Text = "";
                txtMoo.Text = "";
                txtBuilding.Text = "";
                txtFloor.Text = "";
                txtLane.Text = "";
                txtStreet.Text = "";
                txtZipCode.Text = "";


                upPopup.Update();
                mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    var isConnectWsSuccess = false;
                    var isShowException = false;


                    //check recursive branch
                    if (!BranchBiz.CheckRecurviceBranch(txtBranchCodeIdPopup.Text.Trim(), cmbUpperBranchPopup.SelectedItem.Value))
                    {
                        isShowException = true;
                        throw new Exception("SLM : การบันทึกข้อมูลไม่สำเร็จเนื่องจากพบ Recursive Upper Branch");
                    }

                    //check duplicate name , code
                    if (cbEdit.Checked)
                    {
                        //edit
                        if (!BranchBiz.CheckDuplicateBranchNameForEdit(txtBranchCodeIdPopup.Text.Trim(), txtBranchNamePopup.Text.Trim()))
                        {
                            isShowException = true;
                            throw new Exception(txtBranchNamePopup.Text.Trim() + " มีในระบบแล้ว");
                        }
                    }
                    else
                    {
                        if (!BranchBiz.CheckDuplicateBranchCode(txtBranchCodePopup.Text.Trim()))
                        {
                            isShowException = true;
                            throw new Exception("SLM : รหัสสาขา " + txtBranchCodePopup.Text.Trim() + " มีในระบบแล้ว");
                        }

                        if (!BranchBiz.CheckDuplicateBranchName(txtBranchNamePopup.Text.Trim()))
                        {
                            isShowException = true;
                            throw new Exception("SLM : " + txtBranchNamePopup.Text.Trim() + " มีในระบบแล้ว");
                        }
                    }

                    if (AppConstant.CSMServiceEnableSyncBranch)
                    {
                        try
                        {
                            //send data to webservice
                            var insertOrUpdateBranchRequest = new InsertOrUpdateBranchRequest();
                            insertOrUpdateBranchRequest.Header = new WebServiceHeader();
                            insertOrUpdateBranchRequest.Header.service_name = "CSMBranchService";
                            insertOrUpdateBranchRequest.Header.user_name = "SLM";
                            insertOrUpdateBranchRequest.Header.system_code = "SLM";
                            insertOrUpdateBranchRequest.Header.password = "password";
                            insertOrUpdateBranchRequest.Header.command = "InsertOrUpdateBranch";
                            insertOrUpdateBranchRequest.BranchCode = txtBranchCodePopup.Text.Trim();
                            insertOrUpdateBranchRequest.BranchName = txtBranchNamePopup.Text.Trim();
                            insertOrUpdateBranchRequest.Status = Convert.ToInt16(rbActive.Checked);
                            insertOrUpdateBranchRequest.ChannelCode = cmbChannelPopup.SelectedItem.Value;
                            //                            insertOrUpdateBranchRequest.UpperBranchCode = cmbUpperBranchPopup.SelectedItem.Value;
                            insertOrUpdateBranchRequest.UpperBranchCode = BranchBiz.GetBranchCodeNew(cmbUpperBranchPopup.SelectedItem.Value);
                            insertOrUpdateBranchRequest.StartTimeHour = Convert.ToInt32(txtWorkStartHourPopup.Text.Trim());
                            insertOrUpdateBranchRequest.StartTimeMinute = Convert.ToInt32(txtWorkStartMinPopup.Text.Trim());
                            insertOrUpdateBranchRequest.EndTimeHour = Convert.ToInt32(txtWorkEndHourPopup.Text.Trim());
                            insertOrUpdateBranchRequest.EndTimeMinute = Convert.ToInt32(txtWorkEndMinPopup.Text.Trim());
                            insertOrUpdateBranchRequest.ActionUsername = HttpContext.Current.User.Identity.Name;
                            insertOrUpdateBranchRequest.HomeNo = txtAddressNo.Text;
                            insertOrUpdateBranchRequest.Moo = txtMoo.Text;
                            insertOrUpdateBranchRequest.Building = txtBuilding.Text;
                            insertOrUpdateBranchRequest.Floor = txtFloor.Text;
                            insertOrUpdateBranchRequest.Soi = txtLane.Text;
                            insertOrUpdateBranchRequest.Street = txtStreet.Text;
                            insertOrUpdateBranchRequest.Province = cmbProvince.SelectedValue;
                            insertOrUpdateBranchRequest.Amphur = cmbAmphur.SelectedValue;
                            insertOrUpdateBranchRequest.Tambol = cmbTambol.SelectedValue;
                            insertOrUpdateBranchRequest.Zipcode = txtZipCode.Text;
                            insertOrUpdateBranchRequest.Command = cbEdit.Checked;

                            //Logging
                            _log.Info("===== [Start] Call WS Submit Staff Data to CSM: InsertOrUpdateBranch =====");
                            _log.Debug("===== [START] Request Parameter =====");
                            _log.Debug("BranchCode=" + insertOrUpdateBranchRequest.BranchCode);
                            _log.Debug("BranchName=" + insertOrUpdateBranchRequest.BranchName);
                            _log.Debug("Status=" + insertOrUpdateBranchRequest.Status);
                            _log.Debug("ChannelCode=" + insertOrUpdateBranchRequest.ChannelCode);
                            _log.Debug("UpperBranchCode=" + insertOrUpdateBranchRequest.UpperBranchCode);
                            _log.Debug("StartTimeHour=" + insertOrUpdateBranchRequest.StartTimeHour);
                            _log.Debug("StartTimeMinute=" + insertOrUpdateBranchRequest.StartTimeMinute);
                            _log.Debug("EndTimeHour=" + insertOrUpdateBranchRequest.EndTimeHour);
                            _log.Debug("EndTimeMinute=" + insertOrUpdateBranchRequest.EndTimeMinute);
                            _log.Debug("ActionUsername=" + insertOrUpdateBranchRequest.ActionUsername);
                            _log.Debug("HomeNo=" + insertOrUpdateBranchRequest.HomeNo);
                            _log.Debug("Moo=" + insertOrUpdateBranchRequest.Moo);
                            _log.Debug("Building=" + insertOrUpdateBranchRequest.Building);
                            _log.Debug("Floor=" + insertOrUpdateBranchRequest.Floor);
                            _log.Debug("Soi=" + insertOrUpdateBranchRequest.Soi);
                            _log.Debug("Street=" + insertOrUpdateBranchRequest.Street);
                            _log.Debug("Province=" + insertOrUpdateBranchRequest.Province);
                            _log.Debug("Amphur=" + insertOrUpdateBranchRequest.Amphur);
                            _log.Debug("Tambol=" + insertOrUpdateBranchRequest.Tambol);
                            _log.Debug("Zipcode=" + insertOrUpdateBranchRequest.Zipcode);
                            _log.Debug("===== [END] Request Parameter =====");

                            var start = DateTime.Now;
                            _log.DebugFormat("Start Call InsertOrUpdateBranch at {0:dd/MM/yyyy HH:mm:ss}", start);


                            var csmBranchService = new CSMBranchServiceClient();
                            var response = csmBranchService.InsertOrUpdateBranch(insertOrUpdateBranchRequest);
                            isConnectWsSuccess = true;

                            var stop = DateTime.Now;
                            _log.DebugFormat("End Call InsertOrUpdateBranch at {0:dd/MM/yyyy HH:mm:ss} (Elapsed Time={1} seconds)", stop, stop.Subtract(start).TotalSeconds);

                            if (response.StatusResponse.Status == "SUCCESS")
                            {
                                _log.Info("===== [End] Call WS Submit Staff Data to CSM: InsertOrUpdateBranch (SUCCESS) =====");
                                _log.Debug("===== [START] Response Data =====");
                                _log.Debug("IsSuccess=" + response.StatusResponse.Status);
                                _log.Debug("ErrorCode=" + response.StatusResponse.ErrorCode);
                                _log.Debug("ErrorMessage=" + response.StatusResponse.Description);
                                _log.Debug("===== [END] Response Data =====");
                            }
                            else if (response.StatusResponse.Status == "FAILED")
                            {
                                _log.Error("===== [End] Call WS Submit Staff Data to CSM: InsertOrUpdateBranch (FAIL) =====");
                                _log.Error("===== [START] Response Data =====");
                                _log.Error("IsSuccess=" + response.StatusResponse.Status);
                                _log.Error("ErrorCode=" + response.StatusResponse.ErrorCode);
                                _log.Error("ErrorMessage=" + response.StatusResponse.Description);
                                _log.Error("===== [END] Response Data =====");
                            }

                            if (response.StatusResponse.Status == "FAILED")
                            {
                                throw new Exception(string.Format("{0} \r\nError Message : {1}", "การบันทึกข้อมูลไม่สำเร็จที่ ระบบ CSM", !string.IsNullOrEmpty(response.StatusResponse.Description) ? response.StatusResponse.Description : string.Empty));
                            }
                        }
                        catch (Exception wsex)
                        {
                            _log.Error("===== [End] Call WS Submit Staff Data to CSM: InsertOrUpdateBranch (FAIL with Exception) =====", wsex);
                            _log.Error("===== [START] FAIL with Exception =====");
                            _log.Error("ErrorMessage=" + (!string.IsNullOrEmpty(wsex.ToString()) ? wsex.ToString() : "การบันทึกข้อมูลไม่สำเร็จเนื่องจากไม่สามารถเชื่อมต่อระบบ CSM"));
                            _log.Error("===== [END] FAIL with Exception =====");
                            if (isShowException)
                            {
                                throw wsex;
                            }

                            if (!isConnectWsSuccess)
                            {
                                throw new Exception("การบันทึกข้อมูลไม่สำเร็จเนื่องจากไม่สามารถเชื่อมต่อระบบ CSM");
                            }
                            else
                            {
                                throw wsex;
                            }
                        }
                    }

                    if (cbEdit.Checked)
                    {
                        BranchBiz.UpdateData(txtBranchCodeOldPopup.Text.Trim(), txtBranchCodePopup.Text.Trim(), txtBranchNamePopup.Text.Trim(), txtWorkStartHourPopup.Text.Trim(), txtWorkStartMinPopup.Text.Trim(), txtWorkEndHourPopup.Text.Trim(), txtWorkEndMinPopup.Text.Trim(), cmbUpperBranchPopup.SelectedItem.Value, cmbChannelPopup.SelectedItem.Value, rbActive.Checked, HttpContext.Current.User.Identity.Name
                            , txtAddressNo.Text, txtMoo.Text, txtBuilding.Text, txtFloor.Text, txtLane.Text, txtStreet.Text, AppUtil.SafeInt(cmbTambol.SelectedValue), AppUtil.SafeInt(cmbAmphur.SelectedValue), AppUtil.SafeInt(cmbProvince.SelectedValue), txtZipCode.Text);
                    }
                    else
                    {
                        string internalBranchCode = BranchBiz.InsertData(txtBranchCodePopup.Text.Trim(), txtBranchNamePopup.Text.Trim(), txtWorkStartHourPopup.Text.Trim(), txtWorkStartMinPopup.Text.Trim(), txtWorkEndHourPopup.Text.Trim(), txtWorkEndMinPopup.Text.Trim(), cmbUpperBranchPopup.SelectedItem.Value, cmbChannelPopup.SelectedItem.Value, rbActive.Checked, HttpContext.Current.User.Identity.Name
                                                        , txtAddressNo.Text, txtMoo.Text, txtBuilding.Text, txtFloor.Text, txtLane.Text, txtStreet.Text, AppUtil.SafeInt(cmbTambol.SelectedValue), AppUtil.SafeInt(cmbAmphur.SelectedValue), AppUtil.SafeInt(cmbProvince.SelectedValue), txtZipCode.Text);

                        InsertBranchRole(internalBranchCode);
                    }

                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                    ClearPopupControl();
                    mpePopup.Hide();

                    DoSearchBranch(0);
                }
                else
                {
                    mpePopup.Show();
                }
            }
            catch (Exception ex)
            {
                mpePopup.Show();
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool InsertBranchRole(string branchCode)
        {
            try
            {
                BranchBiz.InsertBranchRole(branchCode);
                return true;
            }
            catch(Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error($"InsertBranchRole failed, Error={message}");
                return false;
            }
        }

        #endregion      

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
//                var branch = BranchBiz.GetBranch(((ImageButton)sender).CommandArgument);
                var branch = BranchBiz.GetBranchNew(((ImageButton)sender).CommandArgument);
                if (branch != null)
                {
                    trIdInfo.Visible = true;
                    BuildUpperBranchEdit(branch.BranchCode);
                    cbEdit.Checked = true;
                    txtBranchCodePopup.Text = branch.BranchCodeNew;
                    txtBranchCodePopup.Enabled = false;
                    txtBranchCodeIdPopup.Text = branch.BranchCode;
                    txtBranchCodeIdPopup.Enabled = false;
                    txtBranchNamePopup.Text = branch.BranchName;
                    cmbChannelPopup.SelectedIndex = cmbChannelPopup.Items.IndexOf(cmbChannelPopup.Items.FindByValue(branch.ChannelId));
                    txtWorkStartHourPopup.Text = branch.StartTimeHour;
                    txtWorkStartMinPopup.Text = branch.StartTimeMinute;
                    txtWorkEndHourPopup.Text = branch.EndTimeHour;
                    txtWorkEndMinPopup.Text = branch.EndTimeMinute;

					if (!string.IsNullOrEmpty(branch.UpperBranch))
                    {
                        cmbUpperBranchPopup.SelectedIndex =
                        cmbUpperBranchPopup.Items.IndexOf(cmbUpperBranchPopup.Items.FindByValue(branch.UpperBranch.Trim()));
                    }
                    rbActive.Checked = branch.Status == "Y" ? true : false;
                    rbInActive.Checked = branch.Status == "Y" ? false : true;

                    // address
                    cmbProvince.SelectedIndex = cmbProvince.Items.IndexOf(cmbProvince.Items.FindByValue(branch.slm_ProvinceId.ToString()));
                    BuildAmphur();
                    cmbAmphur.SelectedIndex = cmbAmphur.Items.IndexOf(cmbAmphur.Items.FindByValue(branch.slm_AmphurId.ToString()));
                    BuildTambol();
                    cmbTambol.SelectedIndex = cmbTambol.Items.IndexOf(cmbTambol.Items.FindByValue(branch.slm_TambolId.ToString()));
                    txtAddressNo.Text = branch.slm_House_No;
                    txtMoo.Text = branch.slm_Moo;
                    txtBuilding.Text = branch.slm_Building;
                    txtFloor.Text = branch.slm_Village;
                    txtLane.Text = branch.slm_Soi;
                    txtStreet.Text = branch.slm_Street;
                    txtZipCode.Text = branch.slm_Zipcode;

                    upPopup.Update();
                    mpePopup.Show();
                }
                else
                    throw new Exception("ไม่พบรหัสสาขา " + ((ImageButton)sender).CommandArgument + " ในระบบ");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPopupControl();
            mpePopup.Hide();
        }

        private void ClearPopupControl()
        {
            cbEdit.Checked = false;
            txtBranchCodePopup.Text = "";
            txtBranchCodePopup.Enabled = true;
            txtBranchNamePopup.Text = "";
            cmbChannelPopup.SelectedIndex = -1;
            txtWorkStartHourPopup.Text = "";
            txtWorkStartMinPopup.Text = "";
            txtWorkEndHourPopup.Text = "";
            txtWorkEndMinPopup.Text = "";
            rbActive.Checked = true;
            rbInActive.Checked = false;

            // address
            cmbProvince.SelectedIndex = 0;
            BuildAmphur();
            BuildTambol();
            txtAddressNo.Text = "";
            txtMoo.Text = "";
            txtBuilding.Text = "";
            txtFloor.Text = "";
            txtLane.Text = "";
            txtStreet.Text = "";
            txtZipCode.Text = "";

            alertBranchCodePopup.Text = "";
            alertBranchNamePopup.Text = "";
            alertWorkStartTime.Text = "";
            alertWorkEndTime.Text = "";
            alertStatus.Text = "";
            alertChannel.Text = "";
        }

        private bool ValidateInput()
        {
            int i = 0;
            int branchCodeMaxLength = 10;
            int branchNameMaxLength = 500;
            bool starttimeOK = false;
            bool endtimeOK = false;

            if (cbEdit.Checked == false && txtBranchCodePopup.Text.Trim() == "")
            {
                alertBranchCodePopup.Text = "กรุณาระบุ รหัสสาขา";
                i += 1;
            }
            else
            {
                if (txtBranchCodePopup.Text.Trim().Length > branchCodeMaxLength)
                {
                    alertBranchCodePopup.Text = "กรุณาระบุ รหัสสาขาไม่เกิน " + branchCodeMaxLength.ToString() + " ตัวอักษร";
                    i += 1;
                }
                else
                    alertBranchCodePopup.Text = "";
            }


            if (txtBranchNamePopup.Text.Trim() == "")
            {
                alertBranchNamePopup.Text = "กรุณาระบุ ชื่อสาขา";
                i += 1;
            }
            else
            {
                if (txtBranchNamePopup.Text.Trim().Length > branchNameMaxLength)
                {
                    alertBranchNamePopup.Text = "กรุณาระบุ ชื่อสาขาไม่เกิน " + branchNameMaxLength.ToString() + " ตัวอักษร";
                    i += 1;
                }
                else
                    alertBranchNamePopup.Text = "";
            }

            if (cmbChannelPopup.Text.Trim() == "")
            {
                alertChannel.Text = "กรุณาเลือก ช่องทาง";
                i += 1;
            }
            else
            {
                alertChannel.Text = "";
            }
			
            if (txtWorkStartHourPopup.Text.Trim() == "" || txtWorkStartMinPopup.Text.Trim() == "")
            {
                alertWorkStartTime.Text = "กรุณาระบุ เวลาทำการเริ่มต้นให้ครบ";
                i += 1;
            }
            else
            {
                starttimeOK = true;
                alertWorkStartTime.Text = "";
            }

            if (txtWorkEndHourPopup.Text.Trim() == "" || txtWorkEndMinPopup.Text.Trim() == "")
            {
                alertWorkEndTime.Text = "กรุณาระบุ เวลาทำการสิ้นสุดให้ครบ";
                i += 1;
            }
            else
            {
                endtimeOK = true;
                alertWorkEndTime.Text = "";
            }

            if (starttimeOK && endtimeOK)
            {
                int start = int.Parse(txtWorkStartHourPopup.Text.Trim() + txtWorkStartMinPopup.Text.Trim());
                int end = int.Parse(txtWorkEndHourPopup.Text.Trim() + txtWorkEndMinPopup.Text.Trim());

                if (start >= end)
                {
                    alertWorkStartTime.Text = "เวลาทำการเริ่มต้นต้องน้อยกว่าเวลาทำการสิ้นสุด";
                    i += 1;
                }
                else
                    alertWorkStartTime.Text = "";
            }

            if (cbEdit.Checked && rbInActive.Checked)
            {
                if (BranchBiz.CheckEmployeeInBranch(txtBranchCodePopup.Text.Trim()))
                {
                    alertStatus.Text = "ไม่สามารถปิดสาขาได้ เนื่องจากยังมีพนักงานอยู่ในสาขา";
                    i += 1;
                }
                else
                    alertStatus.Text = "";
            }

            return i > 0 ? false : true;
        }

        protected void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildAmphur();
            BuildTambol();
            mpePopup.Show();
        }

        protected void cmbAmphur_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildTambol();
            mpePopup.Show();
        }

        private void BuildAmphur()
        {
            //เขต/อำเภอ
            cmbAmphur.DataSource = SlmScr010Biz.GetAmphurDataNew(SLMUtil.SafeInt(cmbProvince.SelectedItem.Value));
            cmbAmphur.DataTextField = "TextField";
            cmbAmphur.DataValueField = "ValueField";
            cmbAmphur.DataBind();
            cmbAmphur.Items.Insert(0, new ListItem("", ""));
        }

        private void BuildTambol()
        {
            //แขวง/ตำบล
            cmbTambol.DataSource = SlmScr010Biz.GetTambolDataNew(SLMUtil.SafeInt(cmbAmphur.SelectedItem.Value));
            cmbTambol.DataTextField = "TextField";
            cmbTambol.DataValueField = "ValueField";
            cmbTambol.DataBind();
            cmbTambol.Items.Insert(0, new ListItem("", ""));
        }

        private void BuildUpperBranchCreate()
        {
            try
            {
                cmbUpperBranchPopup.DataSource = BranchBiz.GetUpperBranchList(1);
                cmbUpperBranchPopup.DataTextField = "TextField";
                cmbUpperBranchPopup.DataValueField = "ValueField";
                cmbUpperBranchPopup.DataBind();
                cmbUpperBranchPopup.Items.Insert(0, new ListItem("", ""));
            }
            catch
            {
                throw;
            }
        }

        private void BuildUpperBranchEdit(string branchCodeNew)
        {
            try
            {
                cmbUpperBranchPopup.DataSource = BranchBiz.GetUpperBranchListEdit(branchCodeNew);
                cmbUpperBranchPopup.DataTextField = "TextField";
                cmbUpperBranchPopup.DataValueField = "ValueField";
                cmbUpperBranchPopup.DataBind();
                cmbUpperBranchPopup.Items.Insert(0, new ListItem("", ""));
            }
            catch
            {
                throw;
            }
        }
    }
}