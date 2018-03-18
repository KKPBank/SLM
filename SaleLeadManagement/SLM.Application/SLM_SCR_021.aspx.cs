using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_021 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_021));
        //private int _topicMaxlength = 20;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((Label)Page.Master.FindControl("lblTopic")).Text = "ค้นหาข้อมูลประกาศ";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_021");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    Page.Form.DefaultButton = btnSearch.UniqueID;
                    DoSearchNotice(0);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (tdmCreateDateFrom.DateValue.Year != 1 && tdmCreateDateTo.DateValue.Year == 1)
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุวันที่ให้ครบ");
                    return;
                }
                if (tdmCreateDateFrom.DateValue.Year == 1 && tdmCreateDateTo.DateValue.Year != 1)
                {
                    AppUtil.ClientAlert(Page, "กรุณาระบุวันที่ให้ครบ");
                    return;
                }

                DoSearchNotice(0);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void DoSearchNotice(int pageIndex)
        {
            try
            {
                string createDateFrom = "";
                string createDateTo = "";

                if (tdmCreateDateFrom.DateValue.Year != 1)
                    createDateFrom = tdmCreateDateFrom.DateValue.Year.ToString() + tdmCreateDateFrom.DateValue.ToString("-MM-dd");
                if (tdmCreateDateTo.DateValue.Year != 1)
                    createDateTo = tdmCreateDateTo.DateValue.Year.ToString() + tdmCreateDateTo.DateValue.ToString("-MM-dd");

                List<NoticeData> list = NoticeBiz.SearchNotice(txtTopicSearch.Text.Trim(), createDateFrom, createDateTo, cbActive.Checked, cbInActive.Checked); 
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
            //upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                DoSearchNotice(pageControl.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void btnAddNotice_Click(object sender, EventArgs e)
        {
            mpePopup.Show();
        }

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var notice = NoticeBiz.GetNotice(((ImageButton)sender).CommandArgument);
                txtNoticeIdEdit.Text = notice.NoticeId.ToString();
                txtTopicEdit.Text = notice.Topic;
                rbActiveEdit.Checked = notice.ActiveStatus == "Y" ? true : false;
                rbInactiveEdit.Checked = notice.ActiveStatus == "Y" ? false : true;
                txtEditAttachFileFlag.Text = "";

                if (!string.IsNullOrEmpty(notice.ImageName))
                {
                    lbAttachImageEdit.Text = notice.ImageName;
                    lbAttachImageEdit.OnClientClick = AppUtil.GetNoticeDownloadScript(Page, notice.ImageVirtualPath, "preview");
                    lbAttachImageEdit.CommandArgument = notice.ImagePhysicalPath;   //เก็บไว้ใช้ตอนลบไฟล์
                    divImageInfoEdit.Style["display"] = "block";
                    divImageUploadEdit.Style["display"] = "none";
                }
                else
                {
                    divImageInfoEdit.Style["display"] = "none";
                    divImageUploadEdit.Style["display"] = "block";
                }

                if (!string.IsNullOrEmpty(notice.FileName))
                {
                    lbAttachFileEdit.Text = notice.FileName;
                    lbAttachFileEdit.OnClientClick = AppUtil.GetNoticeDownloadScript(Page, notice.FileVirtualPath, "downlaodfile");
                    lbAttachFileEdit.CommandArgument = notice.FilePhysicalPath;     //เก็บไว้ใช้ตอนลบไฟล์
                    divFileInfoEdit.Style["display"] = "block";
                    divFileUploadEdit.Style["display"] = "none";
                }
                else
                {
                    divFileInfoEdit.Style["display"] = "none";
                    divFileUploadEdit.Style["display"] = "block";
                    txtEditAttachFileFlag.Text = "edit";
                }

                mpePopupEdit.Show();
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
            List<string> results = new List<string>();
            try
            {
                if (ValidateInput())
                {
                    string noticeFolder = AppConstant.NoticeFolder;
                    if (string.IsNullOrEmpty(noticeFolder)) { throw new Exception("ไม่พบ NoticeFolder(ข้อมูลประกาศ) ใน File Config"); }

                    string physicalPath = Server.MapPath(noticeFolder);

                    results = NoticeBiz.InsertData(txtTopic.Text.Trim(), physicalPath, noticeFolder, fuImage.FileName, fuImage.PostedFile.ContentLength,
                        fuAttachFile.FileName, (fuAttachFile.PostedFile != null ? fuAttachFile.PostedFile.ContentLength : 0), rbActive.Checked, HttpContext.Current.User.Identity.Name);

                    string mainFolder = Path.Combine(physicalPath, results[0]); //results[0] = noticeId
                    if (!Directory.Exists(mainFolder))
                        Directory.CreateDirectory(mainFolder);

                    if (fuImage.HasFile && !string.IsNullOrEmpty(results[1]))
                        fuImage.SaveAs(results[1]);

                    if (fuAttachFile.HasFile && !string.IsNullOrEmpty(results[2]))
                        fuAttachFile.SaveAs(results[2]);

                    //ClearPopupControl();
                    //DoSearchNotice(0);
                    //AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                    AppUtil.ClientAlertAndRedirect(Page, "บันทึกข้อมูลเรียบร้อย", "SLM_SCR_021.aspx");
                }
                else
                    mpePopup.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);

                if (results.Count > 0 && !string.IsNullOrEmpty(results[0]))
                    NoticeBiz.DeleteData(int.Parse(results[0]));

                AppUtil.ClientAlert(Page, message);
            }
        }     

        private void ClearPopupControl()
        {
            txtTopic.Text = "";
            rbActive.Checked = true;
            rbInActive.Checked = false;
            lblTopicAlert.Text = "";
            lblFuImageAlert.Text = "";
            lblFuAttachFileAlert.Text = "";
        }

        private bool ValidateInput()
        {
            return true;
            //int i = 0;
            //if (txtTopic.Text.Trim() == "")
            //{
            //    lblTopicAlert.Text = "กรุณาระบุหัวข้อ";
            //    i += 1;
            //}
            //else
            //{
            //    if (txtTopic.Text.Length > _topicMaxlength)
            //    {
            //        lblTopicAlert.Text = "กรุณาระบุหัวข้อไม่เกิน " + _topicMaxlength.ToString() + " ตัวอักษร";
            //        i += 1;
            //    }
            //    else
            //        lblTopicAlert.Text = "";
            //}

            //if (fuImage.HasFile)
            //{
            //    string extension = Path.GetExtension(fuImage.FileName).ToLower();
            //    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            //    {
            //        lblFuImageAlert.Text = "กรุณาระบุรูปภาพให้ถูก format (jpg, jpeg, png)";
            //        i += 1;
            //    }
            //    else if (fuImage.PostedFile.ContentLength > AppConstant.MaximumImageUploadSize)
            //    {
            //        int mb = AppConstant.MaximumImageUploadSize / 1048576;
            //        lblFuImageAlert.Text = "กรุณาระบุรูปภาพขนาดไม่เกิน " + mb + " MB";
            //        i += 1;
            //    }
            //    else
            //        lblFuImageAlert.Text = "";
            //}
            //else
            //{
            //    lblFuImageAlert.Text = "กรุณาระบุรูปภาพ";
            //    i += 1;
            //}

            //if (fuAttachFile.HasFile)
            //{
            //    string extension = Path.GetExtension(fuAttachFile.FileName).ToLower();
            //    if (extension != ".pdf")
            //    {
            //        lblFuAttachFileAlert.Text = "กรุณาระบุไฟล์แนบให้ถูก format (pdf)";
            //        i += 1;
            //    }
            //    if (fuAttachFile.PostedFile.ContentLength > AppConstant.MaximumFileUploadSize)
            //    {
            //        int mb = AppConstant.MaximumImageUploadSize / 1048576;
            //        lblFuAttachFileAlert.Text = "กรุณาระบุไฟล์แนบขนาดไม่เกิน " + mb + " MB";
            //        i += 1;
            //    }
            //}

            //return i > 0 ? false : true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPopupControl();
            mpePopup.Hide();
        }

        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton imbPreviewImage = (ImageButton)e.Row.FindControl("imbPreviewImage");
                imbPreviewImage.ImageUrl = Page.ResolveUrl("~" + imbPreviewImage.CommandArgument);
                imbPreviewImage.OnClientClick = AppUtil.GetNoticeDownloadScript(Page, imbPreviewImage.CommandArgument, "preview");

                ImageButton imbFile = (ImageButton)e.Row.FindControl("imbDownloadFile");
                imbFile.OnClientClick = AppUtil.GetNoticeDownloadScript(Page, imbFile.CommandArgument, "downloadfile");
            }
        }

        //================================= Popup Edit =====================================================

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateEdit())
                {
                    string noticeFolder = AppConstant.NoticeFolder;
                    if (string.IsNullOrEmpty(noticeFolder)) { throw new Exception("ไม่พบ NoticeFolder(ข้อมูลประกาศ) ใน File Config"); }

                    string physicalPath = Server.MapPath(noticeFolder);
                    //bool updateImage = divImageUploadEdit.Style["display"] == "block" ? true : false;   //fuImageEdit.Visible;
                    //bool updateFile = divFileUploadEdit.Style["display"] == "block" ? true : false;     //fuAttachFileEdit.Visible;

                    bool updateImage = fuImageEdit.HasFile;
                    bool updateFile = txtEditAttachFileFlag.Text.Trim() == "edit" ? true : false;

                    List<string> results = NoticeBiz.UpdateData(int.Parse(txtNoticeIdEdit.Text.Trim()), txtTopicEdit.Text.Trim(), physicalPath, noticeFolder
                        , fuImageEdit.FileName, (fuImageEdit.HasFile ? fuImageEdit.PostedFile.ContentLength : 0)
                        , fuAttachFileEdit.FileName, (fuAttachFileEdit.HasFile ? fuAttachFileEdit.PostedFile.ContentLength : 0), rbActiveEdit.Checked, HttpContext.Current.User.Identity.Name, updateImage, updateFile);

                    //Delete ไฟล์เก่า
                    if (updateImage && !string.IsNullOrEmpty(lbAttachImageEdit.CommandArgument))
                        File.Delete(lbAttachImageEdit.CommandArgument);             //ลบไฟล์เก่า
                            
                    if (updateFile && !string.IsNullOrEmpty(lbAttachFileEdit.CommandArgument))
                        File.Delete(lbAttachFileEdit.CommandArgument);              //ลบไฟล์เก่า

                    //Save ไฟล์ใหม่
                    string mainFolder = Path.Combine(physicalPath, results[0]); //results[0] = noticeId
                    if (!Directory.Exists(mainFolder))
                        Directory.CreateDirectory(mainFolder);

                    if (updateImage && fuImageEdit.HasFile && !string.IsNullOrEmpty(results[1]))
                        fuImageEdit.SaveAs(results[1]);

                    if (updateFile && fuAttachFileEdit.HasFile && !string.IsNullOrEmpty(results[2]))
                        fuAttachFileEdit.SaveAs(results[2]);

                    //ClearPopupEditControl();
                    //DoSearchNotice(0);
                    //AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");

                    AppUtil.ClientAlertAndRedirect(Page, "บันทึกข้อมูลเรียบร้อย", "SLM_SCR_021.aspx");
                }
                else
                    mpePopupEdit.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private bool ValidateEdit()
        {
            return true;
            //if (txtTopicEdit.Text.Trim() == "")
            //{
            //    AppUtil.ClientAlert(Page, "กรุณาระบุหัวข้อ");
            //    return false;
            //}
            //if (txtTopicEdit.Text.Length > txtTopicEdit.MaxLength)
            //{
            //    AppUtil.ClientAlert(Page, "กรุณาระบุหัวข้อไม่เกิน " + txtTopicEdit.MaxLength.ToString() + " ตัวอักษร");
            //    return false;
            //}

            //if (divImageUploadEdit.Visible && !fuImageEdit.HasFile)
            //{
            //    AppUtil.ClientAlert(Page, "กรุณาระบุรูปภาพ");
            //    return false;
            //}
            //else if (divImageUploadEdit.Visible && fuImageEdit.HasFile)
            //{
            //    string extension = Path.GetExtension(fuImageEdit.FileName).ToLower();
            //    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            //    {
            //        AppUtil.ClientAlert(Page, "กรุณาระบุรูปภาพให้ถูก format (jpg, jpeg, png)");
            //        return false;
            //    }
            //    if (fuImageEdit.PostedFile.ContentLength > AppConstant.MaximumImageUploadSize)
            //    {
            //        int mb = AppConstant.MaximumImageUploadSize / 1048576;
            //        AppUtil.ClientAlert(Page, "กรุณาระบุรูปภาพขนาดไม่เกิน " + mb + " MB");
            //        return false;
            //    }
            //}

            //if (divFileUploadEdit.Visible && fuAttachFileEdit.HasFile)
            //{
            //    string extension = Path.GetExtension(fuAttachFileEdit.FileName).ToLower();
            //    if (extension != ".pdf")
            //    {
            //        AppUtil.ClientAlert(Page, "กรุณาระบุไฟล์แนบให้ถูก format (pdf)");
            //        return false;
            //    }
            //    if (fuAttachFileEdit.PostedFile.ContentLength > AppConstant.MaximumFileUploadSize)
            //    {
            //        int mb = AppConstant.MaximumImageUploadSize / 1048576;
            //        AppUtil.ClientAlert(Page, "กรุณาระบุไฟล์แนบขนาดไม่เกิน " + mb + " MB");
            //        return false;
            //    }
            //}

            //return true;
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ClearPopupEditControl();
            mpePopupEdit.Hide();
        }

        private void ClearPopupEditControl()
        {
            txtTopicEdit.Text = "";
            lbAttachImageEdit.Text = "";
            lbAttachImageEdit.OnClientClick = "";
            lbAttachImageEdit.CommandArgument = "";
            lbAttachFileEdit.Text = "";
            lbAttachFileEdit.OnClientClick = "";
            lbAttachFileEdit.CommandArgument = "";
            rbActiveEdit.Checked = true;
            rbInactiveEdit.Checked = false;
            txtEditAttachFileFlag.Text = "";
        }

        //protected void imbDeleteAttachImage_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        divImageInfoEdit.Visible = false;
        //        divImageUploadEdit.Visible = true;
        //        mpePopupEdit.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //protected void imbDeleteAttachFile_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        divFileInfoEdit.Visible = false;
        //        divFileUploadEdit.Visible = true;
        //        mpePopupEdit.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}


        #region Backup

        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    List<string> results = new List<string>();
        //    try
        //    {
        //        if (ValidateInput())
        //        {
        //            string mainSavePath = AppConstant.NoticeSavePath;
        //            if (string.IsNullOrEmpty(mainSavePath)) { throw new Exception("ไม่พบ NoticeSavePath ใน File Config"); }

        //            results = NoticeBiz.InsertData(txtTopic.Text.Trim(), mainSavePath, fuImage.FileName, fuImage.PostedFile.ContentLength,
        //                        fuAttachFile.FileName, fuAttachFile.PostedFile.ContentLength, rbActive.Checked, HttpContext.Current.User.Identity.Name);

        //            string mainFolder = Path.Combine(mainSavePath, results[0]); //results[0] = noticeId
        //            if (!Directory.Exists(mainFolder))
        //                Directory.CreateDirectory(mainFolder);

        //            if (fuImage.HasFile && !string.IsNullOrEmpty(results[1]))
        //                fuImage.SaveAs(results[1]);

        //            if (fuAttachFile.HasFile && !string.IsNullOrEmpty(results[2]))
        //                fuAttachFile.SaveAs(results[2]);

        //            ClearPopupControl();
        //            DoSearchNotice(0);
        //            AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
        //        }
        //        else
        //            mpePopup.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);

        //        if (results.Count > 0 && !string.IsNullOrEmpty(results[0]))
        //            NoticeBiz.DeleteData(int.Parse(results[0]));

        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //protected void imbDownloadFile_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        string filePath = ((ImageButton)sender).CommandArgument;
        //        FileInfo fileinfo = new FileInfo(filePath);

        //        Response.Clear();
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        Response.Buffer = true;
        //        Response.ContentType = AppUtil.GetContentType(Path.GetExtension(filePath));
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
        //        Response.AddHeader("Content-Length", fileinfo.Length.ToString());
        //        Response.Flush();
        //        Response.WriteFile(filePath);
        //        Response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        //AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //protected void imbPreviewImage_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        string filePath = ((ImageButton)sender).CommandArgument;
        //        FileInfo fileinfo = new FileInfo(filePath);

        //        Response.Clear();
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        Response.Buffer = true;
        //        Response.ContentType = AppUtil.GetContentType(Path.GetExtension(filePath));
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
        //        Response.AddHeader("Content-Length", fileinfo.Length.ToString());
        //        Response.Flush();
        //        Response.WriteFile(filePath);
        //        Response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Debug(message);
        //        //AppUtil.ClientAlert(Page, message);
        //    }
        //}

        #endregion

    }
}