using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource;
using SLM.Resource.Data;
using log4net;
using SLM.Application.CSMBranchService;

namespace SLM.Application
{
    public partial class SLM_SCR_027 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_027));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาข้อมูลวันหยุดสาขา";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_027");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    Page.Form.DefaultButton = btnSearch.UniqueID;
                    InitialControl();
                    DoSearchCalendarBranch(0);
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
            cmbBranchSearch.DataSource = BranchBiz.GetBranchList(SLMConstant.Branch.All);
            cmbBranchSearch.DataTextField = "TextField";
            cmbBranchSearch.DataValueField = "ValueField";
            cmbBranchSearch.DataBind();
            cmbBranchSearch.Items.Insert(0, new ListItem("", ""));
        }

        private void BindListBox(ListBox listBox, List<ControlListData> source)
        {
            listBox.DataSource = source;
            listBox.DataTextField = "TextField";
            listBox.DataValueField = "ValueField";
            listBox.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearchCalendarBranch(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchCalendarBranch(int pageIndex)
        {
            try
            {
                string holidayDate = "";
                if (tdmHolidayDateSearch.DateValue.Year != 1)
                    holidayDate = tdmHolidayDateSearch.DateValue.Year.ToString() + tdmHolidayDateSearch.DateValue.ToString("-MM-dd");

                List<CalendarBranchData> list = CalendarBranchBiz.SearchCalendarBranch(holidayDate, txtHolidayDescSearch.Text.Trim(), cmbBranchSearch.SelectedItem.Value);
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
                DoSearchCalendarBranch(pageControl.SelectedPageIndex);
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

        protected void btnAddBranchHoliday_Click(object sender, EventArgs e)
        {
            try
            {
                cbEdit.Checked = false;
                tdmHolidayDatePopup.Enabled = true;

                BindListBox(lboxBranchAll, BranchBiz.GetBranchList(SLMConstant.Branch.All));
                lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();

                lboxBranchSelected.Items.Clear();
                lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();

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

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                lboxBranchSelected.Items.AddRange(lboxBranchAll.Items.OfType<ListItem>().ToArray());
                List<ControlListData> list = lboxBranchSelected.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                BindListBox(lboxBranchSelected, list);
                lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();

                lboxBranchAll.Items.Clear();
                lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();

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

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                if (lboxBranchAll.GetSelectedIndices().Count() > 0)
                {
                    List<ListItem> selectedList = new List<ListItem>();

                    List<int> indices = lboxBranchAll.GetSelectedIndices().ToList();
                    foreach (int index in indices)
                    {
                        selectedList.Add(lboxBranchAll.Items[index]);
                    }

                    //Remove selecteditems from lboxBranchAll
                    foreach (ListItem item in selectedList)
                    {
                        lboxBranchAll.Items.Remove(item);
                    }

                    lboxBranchSelected.Items.AddRange(selectedList.ToArray());
                    List<ControlListData> list = lboxBranchSelected.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                    BindListBox(lboxBranchSelected, list);

                    lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();
                    lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();
                }

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

        protected void btnDeselectAll_Click(object sender, EventArgs e)
        {
            try
            {
                lboxBranchAll.Items.AddRange(lboxBranchSelected.Items.OfType<ListItem>().ToArray());
                List<ControlListData> list = lboxBranchAll.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                BindListBox(lboxBranchAll, list);
                lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();

                lboxBranchSelected.Items.Clear();
                lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();

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

        protected void btnDeselect_Click(object sender, EventArgs e)
        {
            try
            {
                if (lboxBranchSelected.GetSelectedIndices().Count() > 0)
                {
                    List<ListItem> deSelectedList = new List<ListItem>();

                    List<int> indices = lboxBranchSelected.GetSelectedIndices().ToList();
                    foreach (int index in indices)
                    {
                        deSelectedList.Add(lboxBranchSelected.Items[index]);
                    }

                    //Remove deSelecteditems from lboxBranchSelected
                    foreach (ListItem item in deSelectedList)
                    {
                        lboxBranchSelected.Items.Remove(item);
                    }

                    lboxBranchAll.Items.AddRange(deSelectedList.ToArray());
                    List<ControlListData> list = lboxBranchAll.Items.OfType<ListItem>().OrderBy(p => p.Text).Select(p => new ControlListData { TextField = p.Text, ValueField = p.Value }).ToList();
                    BindListBox(lboxBranchAll, list);

                    lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();
                    lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();
                }

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

        #endregion

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
					var isConnectWsSuccess = false;
                    if (AppConstant.CSMServiceEnableSyncCalendar)
                    {
                        try
                        {
                            //send data to webservice
                            var updateBranchCalendarRequest = new UpdateBranchCalendarRequest();
                            updateBranchCalendarRequest.Header = new WebServiceHeader();
                            updateBranchCalendarRequest.Header.service_name = "CSMBranchService";
                            updateBranchCalendarRequest.Header.user_name = "SLM";
                            updateBranchCalendarRequest.Header.system_code = "SLM";
                            updateBranchCalendarRequest.Header.password = "password";
                            updateBranchCalendarRequest.Header.command = "UpdateBranchCalendar";
                            updateBranchCalendarRequest.HolidayDate = tdmHolidayDatePopup.DateValue;
                            updateBranchCalendarRequest.HolidayDesc = txtHolidayDescPopup.Text.Trim();
                            updateBranchCalendarRequest.BranchCodeList = GetSelectedNewBranchCode().ToArray();
                            updateBranchCalendarRequest.ActionUsername = HttpContext.Current.User.Identity.Name;
                            updateBranchCalendarRequest.UpdateMode = cbEdit.Checked ? 1 : 2;

                            //logging
                            _log.Info("===== [Start] Call WS Submit Staff Data to CSM: InsertOrUpdateBranchCalendar =====");
                            _log.Debug("===== [START] Request Parameter =====");
                            _log.Debug("HolidayDate=" + updateBranchCalendarRequest.HolidayDate);
                            _log.Debug("HolidayDesc=" + updateBranchCalendarRequest.HolidayDesc);
                            _log.Debug("BranchCodeList=" + updateBranchCalendarRequest.BranchCodeList);
                            _log.Debug("ActionUsername=" + updateBranchCalendarRequest.ActionUsername);
                            _log.Debug("UpdateMode=" + updateBranchCalendarRequest.UpdateMode);
                            _log.Debug("===== [END] Request Parameter =====");

                            var start = DateTime.Now;
                            _log.DebugFormat("Start Call InsertOrUpdateBranchCalendar at {0:dd/MM/yyyy HH:mm:ss}", start);

                            var csmBranchService = new CSMBranchServiceClient();
                            var response = csmBranchService.UpdateBranchCalendar(updateBranchCalendarRequest);
                            isConnectWsSuccess = true;

                            var stop = DateTime.Now;
                            _log.DebugFormat("End Call InsertOrUpdateBranchCalendar at {0:dd/MM/yyyy HH:mm:ss} (Elapsed Time={1} seconds)", stop, stop.Subtract(start).TotalSeconds);

                            if (response.StatusResponse.Status == "SUCCESS")
                            {
                                _log.Info("===== [End] Call WS Submit Staff Data to CSM: InsertOrUpdateBranchCalendar (SUCCESS) =====");
                                _log.Debug("===== [START] Response Data =====");
                                _log.Debug("IsSuccess=" + response.StatusResponse.Status);
                                _log.Debug("ErrorCode=" + response.StatusResponse.ErrorCode);
                                _log.Debug("ErrorMessage=" + response.StatusResponse.Description);
                                _log.Debug("===== [END] Response Data =====");
                            }
                            else if (response.StatusResponse.Status == "FAILED")
                            {
                                _log.Error("===== [End] Call WS Submit Staff Data to CSM: InsertOrUpdateBranchCalendar (FAIL) =====");
                                _log.Error("===== [START] Response Data =====");
                                _log.Error("IsSuccess=" + response.StatusResponse.Status);
                                _log.Error("ErrorCode=" + response.StatusResponse.ErrorCode);
                                _log.Error("ErrorMessage=" + response.StatusResponse.Description);
                                _log.Error("===== [END] Response Data =====");
                            }

                            if (response.StatusResponse.Status == "FAILED")
                            {
                                if (response.StatusResponse.ErrorCode == "004") // ไม่พบ branch ที่ CSM แสดง error message branch code และ branch name
                                {
                                    var errorMessage = new StringBuilder();
                                    var count = 0;
                                    foreach (var branchCode in response.StatusResponse.BranchCodeNotFoundList)
                                    {
                                        //displat last format
                                        if (count >= 0 && count <= 4)
                                        {
                                            errorMessage.AppendFormat("{0} :สาขา{1}\r\n", branchCode, BranchBiz.GetBranchNameNew(branchCode));
                                        }
                                        else if(count == 5)
                                        {
                                            errorMessage.Append("...\r\n");
                                        }

                                        count++;
                                    }

                                    throw new Exception(string.Format("การบันทึกข้อมูลไม่สำเร็จที่ระบบ CSM \r\n Error Message: ข้อมูลที่ไม่สามารถบันทึกรายการได้ เนื่องจากไม่พบ \r\n{0} ในฐานข้อมูล", errorMessage));
                                }

                                throw new Exception(string.Format("{0} \r\nError Message : {1}", "การบันทึกข้อมูลไม่สำเร็จที่ ระบบ CSM", !string.IsNullOrEmpty(response.StatusResponse.Description) ? response.StatusResponse.Description : string.Empty));
                            }
                        }
                        catch (Exception wsex)
                        {
                            _log.Error("===== [End] Call WS Submit Staff Data to CSM: InsertOrUpdateBranchCalendar (FAIL with Exception) =====", wsex);
                            _log.Error("===== [START] FAIL with Exception =====");
                            _log.Error("ErrorMessage=" + (!string.IsNullOrEmpty(wsex.ToString()) ? wsex.ToString() : "การบันทึกข้อมูลไม่สำเร็จเนื่องจากไม่สามารถเชื่อมต่อระบบ CSM"));
                            _log.Error("===== [END] FAIL with Exception =====");
                            if (!isConnectWsSuccess)
                                throw new Exception("การบันทึกข้อมูลไม่สำเร็จเนื่องจากไม่สามารถเชื่อมต่อระบบ CSM");
                            else
                            {
                                mpePopup.Show();
                                throw wsex;
                            }
                        }
                    }

                    if (cbEdit.Checked)
                        CalendarBranchBiz.UpdateData(tdmHolidayDatePopup.DateValue, txtHolidayDescPopup.Text.Trim(), GetSelectedBranchCode(), HttpContext.Current.User.Identity.Name);
                    else
                        CalendarBranchBiz.InsertData(tdmHolidayDatePopup.DateValue, txtHolidayDescPopup.Text.Trim(), GetSelectedBranchCode(), HttpContext.Current.User.Identity.Name);

                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                    ClearPopupControl();
                    mpePopup.Hide();

                    DoSearchCalendarBranch(0);
                }
                else
                    mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private List<string> GetSelectedBranchCode()
        {
            try
            {
                List<string> branchCodeList = new List<string>();
                foreach (ListItem item in lboxBranchSelected.Items)
                {
                    branchCodeList.Add(item.Value);
                }
                return branchCodeList;
            }
            catch
            {
                throw;
            }
        }

		private List<string> GetSelectedNewBranchCode()
        {
            try
            {
                List<string> branchCodeList = new List<string>();
                foreach (ListItem item in lboxBranchSelected.Items)
                {
                    branchCodeList.Add(BranchBiz.GetBranchCodeNew(item.Value));
                }
                return branchCodeList;
            }
            catch
            {
                throw;
            }
        } 
        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                cbEdit.Checked = true;

                int index = Convert.ToInt32(((ImageButton)sender).CommandArgument);
                string holidayDate = ((Label)gvResult.Rows[index].FindControl("lblHolidayDate")).Text.Trim();
                string holidayDesc = ((Label)gvResult.Rows[index].FindControl("lblHolidayDesc")).Text.Trim();

                if (!string.IsNullOrEmpty(holidayDate))
                {
                    string[] obj = holidayDate.Split('/');
                    tdmHolidayDatePopup.DateValue = new DateTime(int.Parse(obj[2]), int.Parse(obj[1]), int.Parse(obj[0]));
                    tdmHolidayDatePopup.Enabled = false;
                    txtHolidayDescPopup.Text = holidayDesc;

                    SetBranchForEdit(tdmHolidayDatePopup.DateValue.Year.ToString() + tdmHolidayDatePopup.DateValue.ToString("-MM-dd"));

                    upPopup.Update();
                    mpePopup.Show();
                }
                else
                    throw new Exception("ไม่พบวันหยุดที่ต้องการแก้ไข");
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void SetBranchForEdit(string holidayDate)
        {
            try
            {
                var list = CalendarBranchBiz.SearchCalendarBranch(holidayDate, "", "");
                List<ControlListData> selectedList = list.Select(p => new ControlListData { TextField = p.BranchName, ValueField = p.BranchCode }).OrderBy(p => p.TextField).ToList();
                BindListBox(lboxBranchSelected, selectedList);

                List<ControlListData> allBranchList = BranchBiz.GetBranchList(SLMConstant.Branch.All);
                foreach (ControlListData data in selectedList)
                {
                    ControlListData obj = allBranchList.Where(p => p.ValueField == data.ValueField).FirstOrDefault();
                    if (obj != null)
                        allBranchList.Remove(obj);
                }

                BindListBox(lboxBranchAll, allBranchList);

                lblBranchAllTotal.Text = lboxBranchAll.Items.Count.ToString();
                lblBranchSelectedTotal.Text = lboxBranchSelected.Items.Count.ToString();
            }
            catch
            {
                throw;
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
            tdmHolidayDatePopup.DateValue = new DateTime();
            tdmHolidayDatePopup.Enabled = true;
            txtHolidayDescPopup.Text = "";
            lboxBranchAll.Items.Clear();
            lboxBranchSelected.Items.Clear();

            alertHolidayDatePopup.Text = "";
            alertHolidayDescPopup.Text = "";
            alertBranchSelected.Text = "";
        }

        private bool ValidateInput()
        {
            int i = 0;
            int holidayDescMaxLength = 100;

            if (tdmHolidayDatePopup.DateValue.Year == 1)
            {
                alertHolidayDatePopup.Text = "กรุณาเลือก วันหยุด";
                i += 1;
            }
            else
                alertHolidayDatePopup.Text = "";

            if (txtHolidayDescPopup.Text.Trim() == "")
            {
                alertHolidayDescPopup.Text = "กรุณาระบุ รายละเอียดวันหยุด";
                i += 1;
            }
            else
            {
                if (txtHolidayDescPopup.Text.Trim().Length > holidayDescMaxLength)
                {
                    alertHolidayDescPopup.Text = "กรุณาระบุ รายละเอียดวันหยุดไม่เกิน " + holidayDescMaxLength.ToString() + " ตัวอักษร";
                    i += 1;
                }
                else
                    alertHolidayDescPopup.Text = "";
            }

            if (cbEdit.Checked == false && lboxBranchSelected.Items.Count == 0)
            {
                alertBranchSelected.Text = "กรุณาเลือก สาขา";
                i += 1;
            }
            else
                alertBranchSelected.Text = "";

            return i > 0 ? false : true;
        }
    }
}