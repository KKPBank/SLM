using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SLM.Application.Utilities;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Biz;
using log4net;

namespace SLM.Application.Shared.Tabs
{
    public partial class Tab009 : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Tab009));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.SetNotThaiCharacter(txtEmailTo);
            //AppUtil.SetMultilineMaxLength(txtNoteDetail, "lblMsg2", AppConstant.TextMaxLength.ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        //public void InitialControl(LeadData data)
        //{
        //    try
        //    {
        //        if (data.ISCOC == "1")
        //        {
        //            btnAddNote.Visible = (data.COCCurrentTeam == SLMConstant.COCTeam.Marketing ? true : false);
        //            cbNoteFlag.Visible = (data.COCCurrentTeam == SLMConstant.COCTeam.Marketing ? true : false);
        //        }
        //        else
        //        {
        //            btnAddNote.Visible = true;
        //            cbNoteFlag.Visible = true;
        //        }
        //        txtTicketID.Text = data.TicketId;
        //        txtContractNo.Text = data.ContractNo;               //Added 01/04/2016
        //        txtPreleadId.Text = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
        //        txtFirstname.Text = data.Name;
        //        txtLastname.Text = data.LastName;
        //        txtOwnerLead.Text = data.OwnerName;
        //        txtCampaign.Text = data.CampaignName;
        //        txtTelNo1.Text = data.TelNo_1;
        //        txtExt1.Text = data.Ext_1;
        //        cbNoteFlag.Checked = data.NoteFlag != null ? (data.NoteFlag == "1" ? true : false) : false;

        //        pcTop.SetVisible = false;
        //        DoBindGridview();
        //        CheckEmailSubject();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void SetDefaultValue(string ticketId, string preleadId)
        {
            txtTicketID.Text = ticketId;
            txtPreleadId.Text = preleadId;
        }

        public void InitialControl(string ticketId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ticketId))
                {
                    ticketId = Request["ticketid"];
                }
                var data = SlmScr009Biz.GetDataForNoteTab(ticketId);

                if (data.ISCOC == "1")
                {
                    btnAddNote.Visible = (data.COCCurrentTeam == SLMConstant.COCTeam.Marketing ? true : false);
                    cbNoteFlag.Visible = (data.COCCurrentTeam == SLMConstant.COCTeam.Marketing ? true : false);
                }
                else
                {
                    btnAddNote.Visible = true;
                    cbNoteFlag.Visible = true;
                }
                txtTicketID.Text = data.TicketId;
                txtContractNo.Text = data.ContractNo;               //Added 01/04/2016
                txtPreleadId.Text = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
                txtFirstname.Text = data.Name;
                txtLastname.Text = data.LastName;
                txtOwnerLead.Text = data.OwnerName;
                txtCampaign.Text = data.CampaignName;
                txtTelNo1.Text = data.TelNo_1;
                txtExt1.Text = data.Ext_1;
                cbNoteFlag.Checked = data.NoteFlag != null ? (data.NoteFlag == "1" ? true : false) : false;

                pcTop.SetVisible = false;
                DoBindGridview();
                CheckEmailSubject();
            }
            catch
            {
                throw;
            }
        }

        private void DoBindGridview()
        {
            List<NoteHistoryData> result = SlmScr009Biz.SearchNoteHistory(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
            BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), 0);
            cbNoteFlag.Enabled = gvNoteHistory.Rows.Count > 0 ? true : false;
            upResult.Update();
            pnTab009.Visible = true;
        }

        private void CheckEmailSubject()
        {
            if (cbSendEmail.Checked)
            {
                txtEmailSubject.Enabled = true;

                if (txtTicketID.Text.Trim() != "")      //กรณี Lead
                    txtEmailSubject.Text = "SLM: Ticket: " + txtTicketID.Text.Trim();
                else
                    txtEmailSubject.Text = "SLM: ContractNo: " + txtContractNo.Text.Trim();          //กรณี Prelead

                trEmail.Visible = true;
                trEmailTo.Visible = true;
                trEmailSample.Visible = true;
            }
            else
            {
                txtEmailSubject.Text = string.Empty;
                txtEmailSubject.Enabled = false;
                txtEmailSubject.Text = "";
                alertEmailSubject.Text = "";
                txtEmailTo.Text = "";
                alertEmailTo.Text = "";
                trEmail.Visible = false;
                trEmailTo.Visible = false;
                trEmailSample.Visible = false;
            }
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvNoteHistory);
            pageControl.Update(items, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                List<NoteHistoryData> result = SlmScr009Biz.SearchNoteHistory(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                BindGridview(pageControl, result.ToArray(), pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void btnAddNote_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtTicketID.Text.Trim() != "")      //กรณีเป็น Lead
                {
                    cbSendEmail.Enabled = SlmScr009Biz.HasOwnerOrDelegate(txtTicketID.Text.Trim());
                    if (!cbSendEmail.Enabled)
                        lblInfo.Text = "ข้อมูลผู้มุ่งหวังนี้ ยังไม่ถูกจ่ายงานให้ Telesales ดังนั้นการบันทึก Note ไม่สามารถส่ง Email ได้";
                    else
                    {
                        lblInfo.Text = "";
                        CheckEmailSubject();
                    }
                }
                else
                {
                    //กรณี Prelead
                    CheckEmailSubject();
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPopupControl();
        }

        private void ClearPopupControl()
        {
            //txtContractNo, txtTicketID, txtPreleadId ห้ามเคลีย
            txtNoteDetail.Text = "";
            cbSendEmail.Checked = false;
            CheckEmailSubject();
            lblInfo.Text = "";
            alertEmailSubject.Text = "";
            alertEmailTo.Text = "";
            lblError.Text = "";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData())
                {
                    if (txtNoteDetail.Text.Trim().Length > AppConstant.TextMaxLength)
                        throw new Exception("ไม่สามารถบันทึกรายละเอียด Note เกิน " + AppConstant.TextMaxLength.ToString() + " ตัวอักษรได้");

                    string noteId = "";
                    if (txtTicketID.Text.Trim() != "")  //กรณี Lead
                    {
                        noteId = SlmScr009Biz.InsertNoteHistory(txtTicketID.Text.Trim(), cbSendEmail.Checked, txtEmailSubject.Text.Trim(), txtNoteDetail.Text.Trim(), GetEmailList(), HttpContext.Current.User.Identity.Name);
                    }
                    else if (txtPreleadId.Text.Trim() != "" && txtTicketID.Text.Trim() == "")    //กรณี Prelead
                    {
                        noteId = SlmScr009Biz.InsertPreleadNoteHistory(decimal.Parse(txtPreleadId.Text.Trim()), cbSendEmail.Checked, txtEmailSubject.Text.Trim(), txtNoteDetail.Text.Trim(), GetEmailList(), HttpContext.Current.User.Identity.Name);
                    }
                    else
                        throw new Exception("ไม่พบ PreleadId หรือ TicketId เพื่อใช้ในการบันทึกข้อมูล");

                    CreateCASActivityLog(noteId);
                    txtNoteDetail.Text = "";
                    cbSendEmail.Checked = false;
                    CheckEmailSubject();
                    lblInfo.Text = "";
                    ClearPopupControl();
                    if (txtTicketID.Text.Trim() != "")
                    {
                        cbNoteFlag.Checked = true;
                    }
                    DoBindGridview();

                    mpePopup.Hide();                  
                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
                }
                else
                {
                    mpePopup.Show();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
                mpePopup.Show();
            }
        }

        private void CreateCASActivityLog(string noteId)
        {
            SlmScr009Biz biz = new SlmScr009Biz();
            string emailTo = "";
            string emailSubject = "";
            try
            {
                var data = CARServiceBiz.GetDataForCARLogService(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
                
                if (cbSendEmail.Checked)
                {
                    emailSubject = txtEmailSubject.Text.Trim();

                    if (data != null)
                    {
                        emailSubject = "[" + data.StatusName + "]" + (string.IsNullOrEmpty(data.SubStatusName) ? "" : "-[" + data.SubStatusName + "]") + " " + emailSubject;

                        if (!string.IsNullOrEmpty(data.Owner) && !string.IsNullOrEmpty(data.OwnerEmail))
                            emailTo += (emailTo != "" ? ", " : "") + data.OwnerEmail;
                        if (!string.IsNullOrEmpty(data.Delegate) && !string.IsNullOrEmpty(data.DelegateEmail))
                            emailTo += (emailTo != "" ? ", " : "") + data.DelegateEmail;
                    }

                    if (txtEmailTo.Text.Trim() != "")
                        emailTo += (emailTo != "" ? ", " : "") + txtEmailTo.Text.Trim();
                }


                var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);

                //Activity Info
                List<Services.CARService.DataItem> actInfoList = new List<Services.CARService.DataItem>();
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "ผู้สร้าง Note", DataValue = staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ส่งอีเมล", DataValue = cbSendEmail.Checked ? "Y" : "N" });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "To", DataValue = emailTo });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 4, DataLabel = "Subject", DataValue = emailSubject });
                actInfoList.Add(new Services.CARService.DataItem() { SeqNo = 5, DataLabel = "บันทึก Note", DataValue = txtNoteDetail.Text.Trim() });

                //Customer Info
                List<Services.CARService.DataItem> cusInfoList = new List<Services.CARService.DataItem>();
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = (data != null ? data.CitizenId : "") });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = (data != null ? data.CardTypeName : "") });
                cusInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = data != null ? data.CustomerName : "" });

                //Product Info
                List<Services.CARService.DataItem> prodInfoList = new List<Services.CARService.DataItem>();
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = (data != null ? data.ProductGroupName : "") });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = (data != null ? data.ProductName : "") });
                prodInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = (data != null ? data.CampaignName : "") });

                //Contract Info
                List<Services.CARService.DataItem> contInfoList = new List<Services.CARService.DataItem>();
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = (data != null ? data.ContractNo : "") });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = txtPreleadId.Text.Trim() != "" ? "HP" : "" });
                contInfoList.Add(new Services.CARService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = (data != null ? data.LicenseNo : "") });

                //Officer Info
                List<Services.CARService.DataItem> offInfoList = new List<Services.CARService.DataItem>();
                offInfoList.Add(new Services.CARService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = (staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name) });

                Services.CARService.CARServiceData carData = new Services.CARService.CARServiceData()
                {
                    ReferenceNo = noteId,
                    SecurityKey = txtPreleadId.Text.Trim() != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey,
                    ServiceName = "CreateActivityLog",
                    SystemCode = txtPreleadId.Text.Trim() != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM,      //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
                    TransactionDateTime = DateTime.Now,
                    ActivityInfoList = actInfoList,
                    CustomerInfoList = cusInfoList,
                    ProductInfoList = prodInfoList,
                    ContractInfoList = contInfoList,
                    OfficerInfoList = offInfoList,
                    ActivityDateTime = DateTime.Now,
                    CampaignId = data != null ? data.CampaignId : "",
                    ChannelId = data != null ? data.ChannelId : "",
                    PreleadId = txtPreleadId.Text.Trim(),
                    ProductGroupId = data != null ? data.ProductGroupId : "",
                    ProductId = data != null ? data.ProductId : "",
                    Status = data != null ? data.StatusName : "",
                    SubStatus = data != null ? data.SubStatusName : "",
                    TicketId = txtTicketID.Text.Trim(),
                    SubscriptionId = data != null ? data.CitizenId : "",
                    TypeId = SLMConstant.CARLogService.Data.TypeId,
                    AreaId = SLMConstant.CARLogService.Data.AreaId,
                    SubAreaId = SLMConstant.CARLogService.Data.SubAreaId,
                    ActivityTypeId = SLMConstant.CARLogService.Data.ActivityType.FYIId,
                    ContractNo = data != null ? data.ContractNo : ""
                };

                if (data != null && !string.IsNullOrEmpty(data.CBSSubScriptionTypeId))
                    carData.CIFSubscriptionTypeId = data.CBSSubScriptionTypeId;

                bool ret = Services.CARService.CreateActivityLog(carData);
                biz.UpdateCasFlag(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim(), noteId, ret ? "1" : "2");
            }
            catch (Exception ex)
            {
                biz.UpdateCasFlag(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim(), noteId, "2");

                //Error ให้ลง Log ไว้ ไม่ต้อง Throw
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
            }
        }

        //oldcasservice
        //private void CreateCASActivityLog(string noteId)
        //{
        //    SlmScr009Biz biz = new SlmScr009Biz();
        //    string emailTo = "";
        //    try
        //    {
        //        var data = new SlmScr009Biz().GetDataForCARLogService(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());

        //        if (cbSendEmail.Checked)
        //        {
        //            if (data != null)
        //            {
        //                if (!string.IsNullOrEmpty(data.Owner) && !string.IsNullOrEmpty(data.OwnerEmail))
        //                    emailTo += (emailTo != "" ? ", " : "") + data.OwnerEmail;
        //                if (!string.IsNullOrEmpty(data.Delegate) && !string.IsNullOrEmpty(data.DelegateEmail))
        //                    emailTo += (emailTo != "" ? ", " : "") + data.DelegateEmail;
        //            }

        //            if (txtEmailTo.Text.Trim() != "")
        //                emailTo += (emailTo != "" ? ", " : "") + txtEmailTo.Text.Trim();
        //        }


        //        var staff = StaffBiz.GetStaff(HttpContext.Current.User.Identity.Name);

        //        //Activity Info
        //        List<Services.CASService.DataItem> actInfoList = new List<Services.CASService.DataItem>();
        //        actInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "ผู้สร้าง Note", DataValue = staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name });
        //        actInfoList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "ส่งอีเมล", DataValue = cbSendEmail.Checked ? "Y" : "N" });
        //        actInfoList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "To", DataValue = emailTo });
        //        actInfoList.Add(new Services.CASService.DataItem() { SeqNo = 4, DataLabel = "Subject", DataValue = txtEmailSubject.Text.Trim() });
        //        actInfoList.Add(new Services.CASService.DataItem() { SeqNo = 5, DataLabel = "บันทึก Note", DataValue = txtNoteDetail.Text.Trim() });

        //        //Customer Info
        //        List<Services.CASService.DataItem> cusInfoList = new List<Services.CASService.DataItem>();
        //        cusInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "Subscription ID", DataValue = (data != null ? data.CitizenId : "") });
        //        cusInfoList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "Subscription Type", DataValue = (data != null ? data.CardTypeName : "") });
        //        cusInfoList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "ชื่อ-นามสกุลของลูกค้า", DataValue = txtFirstname.Text.Trim() + " " + txtLastname.Text.Trim() });

        //        //Product Info
        //        List<Services.CASService.DataItem> prodInfoList = new List<Services.CASService.DataItem>();
        //        prodInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "Product Group", DataValue = (data != null ? data.ProductGroupName : "") });
        //        prodInfoList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "Product", DataValue = (data != null ? data.ProductName : "") });
        //        prodInfoList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "Campaign", DataValue = (data != null ? data.CampaignName : "") });

        //        //Contract Info
        //        List<Services.CASService.DataItem> contInfoList = new List<Services.CASService.DataItem>();
        //        contInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "เลขที่สัญญา", DataValue = (data != null ? data.ContractNo : "") });
        //        contInfoList.Add(new Services.CASService.DataItem() { SeqNo = 2, DataLabel = "ระบบที่บันทึกสัญญา", DataValue = txtPreleadId.Text.Trim() != "" ? "HP" : "" });
        //        contInfoList.Add(new Services.CASService.DataItem() { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = (data != null ? data.LicenseNo : "") });

        //        //Officer Info
        //        List<Services.CASService.DataItem> offInfoList = new List<Services.CASService.DataItem>();
        //        offInfoList.Add(new Services.CASService.DataItem() { SeqNo = 1, DataLabel = "Officer", DataValue = (staff != null ? staff.StaffNameTH : HttpContext.Current.User.Identity.Name) });

        //        Services.CASService.CASServiceData carData = new Services.CASService.CASServiceData()
        //        {
        //            ReferenceNo = noteId,
        //            SecurityKey = txtPreleadId.Text.Trim() != "" ? SLMConstant.CARLogService.OBTSecurityKey : SLMConstant.CARLogService.SLMSecurityKey,
        //            ServiceName = "CreateActivityLog",
        //            SystemCode = txtPreleadId.Text.Trim() != "" ? SLMConstant.CARLogService.CARLoginOBT : SLMConstant.CARLogService.CARLoginSLM,      //ถ้ามี preleadid ให้ถือว่าเป็น OBT ทั้งหมด ถึงจะมี TicketId ก็ตาม
        //            TransactionDateTime = DateTime.Now,
        //            ActivityInfoList = actInfoList,
        //            CustomerInfoList = cusInfoList,
        //            ProductInfoList = prodInfoList,
        //            ContractInfoList = contInfoList,
        //            OfficerInfoList = offInfoList,
        //            ActivityDateTime = DateTime.Now,
        //            CampaignId = data != null ? data.CampaignId : "",
        //            ChannelId = data != null ? data.ChannelId : "",
        //            PreleadId = txtPreleadId.Text.Trim(),
        //            ProductGroupId = data != null ? data.ProductGroupId : "",
        //            ProductId = data != null ? data.ProductId : "",
        //            Status = data != null ? data.StatusName : "",
        //            SubStatus = data != null ? data.SubStatusName : "",
        //            TicketId = txtTicketID.Text.Trim(),
        //            SubscriptionId = data != null ? data.CitizenId : "",
        //            TypeName = SLMConstant.CARLogService.Data.Type,
        //            AreaName = SLMConstant.CARLogService.Data.Area,
        //            SubAreaName = SLMConstant.CARLogService.Data.SubArea,
        //            ActivityType = SLMConstant.CARLogService.Data.ActivityType.FYI,
        //            ContractNo = data != null ? data.ContractNo : ""
        //        };

        //        if (data != null && data.CardTypeId != null)
        //            carData.SubscriptionTypeId = data.CardTypeId.Value;

        //        bool ret = Services.CASService.CreateActivityLog(carData);
        //        biz.UpdateCasFlag(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim(), noteId, ret ? "1" : "2");
        //    }
        //    catch (Exception ex)
        //    {
        //        biz.UpdateCasFlag(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim(), noteId, "2");

        //        //Error ให้ลง Log ไว้ ไม่ต้อง Throw
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //    }
        //}

        protected void cbNoteFlag_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SlmScr009Biz.ChangeNoteFlag(txtTicketID.Text.Trim(), cbNoteFlag.Checked, HttpContext.Current.User.Identity.Name);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cbSendEmail_CheckedChanged(object sender, EventArgs e)
        {
            CheckEmailSubject();
            mpePopup.Show();
        }

        private bool ValidateEmail(string email)
        {
            string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern);
            return reg.IsMatch(email);
        }

        private List<string> GetEmailList()
        {
            try
            {
                List<string> emailList = new List<string>();

                if (cbSendEmail.Checked && txtEmailTo.Text.Trim() != "")
                {
                    string[] arr_email = txtEmailTo.Text.Trim().Split(',');
                    foreach (string email in arr_email)
                    {
                        if (ValidateEmail(email.Trim()))
                        {
                            if (!emailList.Contains(email.Trim()))
                                emailList.Add(email.Trim());
                        }
                        else
                            throw new Exception("กรุณาระบุ E-mail ให้ถูกต้อง");
                    }
                }

                return emailList;
            }
            catch
            {
                throw;
            }
        }

        private bool ValidateData()
        {
            int i = 0;
            int emailSubjectMaxLength = 500;

            if (cbSendEmail.Checked)
            {
                if (txtEmailTo.Text.Trim() != "")
                {
                    alertEmailTo.Text = "";
                    string[] arr_email = txtEmailTo.Text.Trim().Split(',');
                    foreach (string email in arr_email)
                    {
                        if (email.Trim() == "")
                        {
                            alertEmailTo.Text = "กรุณาระบุ E-mail ให้ถูกต้อง";
                            i += 1;
                            break;
                        }
                        else
                        {
                            if (!ValidateEmail(email.Trim()))
                            {
                                alertEmailTo.Text = "กรุณาระบุ E-mail ให้ถูกต้อง";
                                i += 1;
                                break;
                            }
                        }
                    }
                }
                else
                    alertEmailTo.Text = "";

                if (txtEmailSubject.Text.Trim() == "")
                {
                    alertEmailSubject.Text = "กรุณากรอกข้อมูล Email Subject ก่อนบันทึก";
                    i += 1;
                }
                else
                {
                    if (txtEmailSubject.Text.Trim().Length > emailSubjectMaxLength)
                    {
                        alertEmailSubject.Text = "กรุณากรอกข้อมูล Email Subject ไม่เกิน " + emailSubjectMaxLength.ToString() + " ตัวอักษร";
                        i += 1;
                    }
                    else
                        alertEmailSubject.Text = "";
                }
            }

            if (txtNoteDetail.Text.Trim() == "")
            {
                lblError.Text = "กรุณากรอกข้อมูลบันทึก Note ก่อนบันทึก";
                i += 1;
            }
            else
            {
                if (txtNoteDetail.Text.Trim().Length > AppConstant.TextMaxLength)
                {
                    lblError.Text = "ไม่สามารถบันทึกรายละเอียด Note เกิน " + AppConstant.TextMaxLength.ToString() + " ตัวอักษรได้";
                    i += 1;
                }
                else
                    lblError.Text = "";
            }

            return i > 0 ? false : true;
        }

        //==========================================================================================================
        //Prelead Section
        //==========================================================================================================

        //public void InitialControlPrelead(PreleadViewData data)
        //{
        //    try
        //    {
        //        cbNoteFlag.Visible = false;                 //ตอนเป็น Prelead จะไม่มี checkbox แสดงเตือน Note
        //        txtTicketID.Text = data.TicketId;
        //        if (!string.IsNullOrEmpty(data.TicketId))
        //            btnAddNote.Visible = false;

        //        txtContractNo.Text = data.ContractNo;               //Added 01/04/2016
        //        txtPreleadId.Text = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
        //        txtFirstname.Text = data.Firstname;
        //        txtLastname.Text = data.Lastname;
        //        txtOwnerLead.Text = data.OwnerName;
        //        txtCampaign.Text = data.CampaignName;
        //        txtTelNo1.Text = data.TelNo1;
        //        txtExt1.Text = "";
        //        //cbNoteFlag.Checked = data.NoteFlag != null ? (data.NoteFlag == "1" ? true : false) : false;

        //        pcTop.SetVisible = false;
        //        DoBindGridview();
        //        CheckEmailSubject();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void InitialControlPrelead(string preleadId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(preleadId))
                {
                    preleadId = Request["preleadid"];
                }
                PreleadViewData data = new PreleadBiz().GetPreleadData(decimal.Parse(preleadId), "", "", "");

                cbNoteFlag.Visible = false;                 //ตอนเป็น Prelead จะไม่มี checkbox แสดงเตือน Note
                txtTicketID.Text = data.TicketId;
                if (!string.IsNullOrEmpty(data.TicketId))
                {
                    btnAddNote.Visible = false;
                }

                txtContractNo.Text = data.ContractNo;               //Added 01/04/2016
                txtPreleadId.Text = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
                txtFirstname.Text = data.Firstname;
                txtLastname.Text = data.Lastname;
                txtOwnerLead.Text = data.OwnerName;
                txtCampaign.Text = data.CampaignName;
                txtTelNo1.Text = data.TelNo1;
                txtExt1.Text = "";
                //cbNoteFlag.Checked = data.NoteFlag != null ? (data.NoteFlag == "1" ? true : false) : false;

                pcTop.SetVisible = false;
                DoBindGridview();
                CheckEmailSubject();
            }
            catch
            {
                throw;
            }
        }

        protected void lbReloadNote_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["ticketid"]))
                {
                    txtTicketID.Text = Request["ticketid"];
                    InitialControl(txtTicketID.Text.Trim());
                }
                else if (!string.IsNullOrEmpty(Request["preleadid"]))
                {
                    txtPreleadId.Text = Request["preleadid"];
                    InitialControlPrelead(txtPreleadId.Text.Trim());
                }
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