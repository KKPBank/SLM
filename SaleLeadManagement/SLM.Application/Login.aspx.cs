 using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
 using System.Text.RegularExpressions;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;
using AjaxControlToolkit;

namespace SLM.Application
{
    public partial class Login : System.Web.UI.Page
    {
        private string _displayName = "";
        private static readonly ILog _log = LogManager.GetLogger(typeof(Login));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DisplayNotice();
                    BrowserDetection();
                    Page.Form.DefaultButton = ((Button)Login1.FindControl("LoginButton")).UniqueID;

					if (Request["ReturnUrl"] != null)
                    {
                        string returnUrl = Request["ReturnUrl"];
                        var dic = GetUrlParameters(returnUrl);

                        if (dic.Count(p => p.Key == "usename") > 0)
                        {
                            var decryptUsername = StringCipher.Decrypt(dic["username"], AppConstant.CSMEncryptPassword);
                            
                            Login1.UserName = decryptUsername;
                            hddCsmUsername.Value = decryptUsername;
                        }

                        hddReturnUrl.Value = returnUrl;
                    }
                    if (HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        Response.Redirect(FormsAuthentication.DefaultUrl);
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

        private Dictionary<string, string> GetUrlParameters(string uri)
        {
            var parsedString = HttpUtility.UrlDecode(uri);
            var matches = Regex.Matches(parsedString, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
            var keyValues = new Dictionary<string, string>(matches.Count);

            foreach (Match m in matches)
            {
                keyValues.Add(Uri.UnescapeDataString(m.Groups[2].Value).ToLower(),
                    Uri.UnescapeDataString(m.Groups[3].Value));
            }

            return keyValues;
        }

        private void BrowserDetection()
        {
            System.Web.HttpBrowserCapabilities browser = Request.Browser;
            string s = "Browser Capabilities\n"
                + "Type = " + browser.Type + "\n"
                + "Name = " + browser.Browser + "\n"
                + "Version = " + browser.Version + "\n"
                + "Major Version = " + browser.MajorVersion + "\n"
                + "Minor Version = " + browser.MinorVersion + "\n"
                + "Platform = " + browser.Platform + "\n"
                + "Is Beta = " + browser.Beta + "\n"
                + "Is Crawler = " + browser.Crawler + "\n"
                + "Is AOL = " + browser.AOL + "\n"
                + "Is Win16 = " + browser.Win16 + "\n"
                + "Is Win32 = " + browser.Win32 + "\n"
                + "Supports Frames = " + browser.Frames + "\n"
                + "Supports Tables = " + browser.Tables + "\n"
                + "Supports Cookies = " + browser.Cookies + "\n"
                + "Supports VBScript = " + browser.VBScript + "\n"
                + "Supports JavaScript = " +
                    browser.EcmaScriptVersion.ToString() + "\n"
                + "Supports Java Applets = " + browser.JavaApplets + "\n"
                + "Supports ActiveX Controls = " + browser.ActiveXControls
                      + "\n"
                + "Supports JavaScript Version = " +
                    browser["JavaScriptVersion"] + "\n";

            if (browser.Browser.ToLower() != "firefox")
                ((Label)Login1.FindControl("lblBrowserWarning")).Text = "&#9658; การแสดงผลอาจมีปัญหา หรือใช้งาน ADAMs ไม่ได้<br />&nbsp;&nbsp;&nbsp;&nbsp;เนื่องจากไม่ได้ใช้งานผ่าน Firefox";

            if ((browser.Browser.ToLower() == "internetexplorer" || browser.Browser.ToLower() == "ie") && browser.MajorVersion <= 7)
            {
                ((TextBox)Login1.FindControl("Password")).TextMode = TextBoxMode.Password;
                ((TextBox)Login1.FindControl("Password")).Attributes.Remove("onkeypress");
                ((TextBox)Login1.FindControl("Password")).Attributes.Remove("onfocus");
                ((TextBox)Login1.FindControl("Password")).Attributes.Remove("onblur");

                ((TextBoxWatermarkExtender)Login1.FindControl("txtUsernameWatermark")).Enabled = false;
                ((TextBoxWatermarkExtender)Login1.FindControl("txtPasswordWatermark")).Enabled = false;
            }

            //Response.Write(s);
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            try
            {
                StaffData staffData = SlmMasterBiz.GetStaffData(Login1.UserName.Trim());

                if (staffData != null)
                //if (staffData != null && IsAuthenticated(Login1.UserName.Trim(), Login1.Password.Trim()))
                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        Login1.UserName.Trim(),
                        DateTime.Now,
                        DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                        Login1.RememberMeSet,
                        staffData.StaffNameTH + "|" + staffData.BranchName + "|" + (staffData.StaffTypeId != null ? staffData.StaffTypeId.Value.ToString() : ""),
                        FormsAuthentication.FormsCookiePath);

                    string encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    //Response.Redirect(FormsAuthentication.GetRedirectUrl(Login1.UserName, Login1.RememberMeSet), false);

					if (Login1.UserName == hddCsmUsername.Value)
                    {
                        Response.Redirect(Server.UrlDecode(hddReturnUrl.Value));
                    }
                    else  if (Request["ticketid"] != null && Request["accflag"] == "email")
					{
                        Response.Redirect("SLM_SCR_004.aspx?ticketid=" + Request["ticketid"] + "&type=" + Request["type"], false);
					}
                    else
					{
                        Response.Redirect(FormsAuthentication.DefaultUrl, false);
                    }
                }
                else
                {
                    ((TextBox)Login1.FindControl("Password")).Text = "";
                    _log.Error("Logon failure: unknown user name or bad password.");
                    AppUtil.ClientAlert(Page, "Logon failure: unknown user name or bad password.");
                }
            }
            catch (Exception ex)
            {
                ((TextBox)Login1.FindControl("Password")).Text = "";
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error("(" + Login1.UserName + ") " + message);
                _log.Debug("(" + Login1.UserName + ") ", ex);
                AppUtil.ClientAlert(Page, "Logon failure: unknown user name or bad password.");
            }
        }

        private bool IsAuthenticated(string username, string password)
        {
            try
            {
                _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + Login1.UserName.Trim() + " Begin Login (Authenticated AD)");
                string domainName = ConfigurationManager.AppSettings["LoginDomain"].ToString();
                string domainAndUsername = string.Format(@"{0}\{1}", domainName, username);

                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName, domainAndUsername, password);

                UserPrincipal user = new UserPrincipal(ctx);
                user.SamAccountName = username;

                PrincipalSearcher search = new PrincipalSearcher(user);
                UserPrincipal result = (UserPrincipal)search.FindOne();

                if (result != null)
                {
                    _displayName = result.DisplayName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _log.Debug(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + Login1.UserName.Trim() + " End Login (Authenticated AD)");
            }
        }

        private void DisplayNotice()
        {
            try
            {
                Random rnd = new Random();
                var list = NoticeBiz.GetNoticeIdList();
                if (list.Count > 0)
                {
                    int index = rnd.Next(0, list.Count);        //creates a number between 0 and list.Count - 1
                    var notice = NoticeBiz.GetNotice(list[index].ToString());

                    lblNoticeTopic.Text = notice.Topic;
                    imgNotice.ImageUrl = Page.ResolveUrl("~" + notice.ImageVirtualPath);

                    if (!string.IsNullOrEmpty(notice.FileName))
                    {
                        imbNoticeDownload.Visible = true;
                        imbNoticeDownload.OnClientClick = AppUtil.GetNoticeDownloadScript(Page, notice.FileVirtualPath, "downloadfile");

                        //lbNoticeDownload.Text = notice.FileName;
                        //lbNoticeDownload.OnClientClick = AppUtil.GetNoticeDownloadScript(Page, notice.FileVirtualPath, "downloadfile");
                    }
                    else
                    {
                        imbNoticeDownload.Visible = false;
                        //lbNoticeDownload.Text = "ไม่พบไฟล์เอกสาร";
                        //lbNoticeDownload.Enabled = false;
                    }
                }
                else
                {
                    trNoticeDownload.Visible = false;
                    trNoticeDownloadAll.Visible = false;
                }
            }
            catch
            {
                throw;
            }
        }

        protected void lbNoticeShowAll_Click(object sender, EventArgs e)
        {
            try
            {
                gvNotice.DataSource = NoticeBiz.SearchNotice("", "", "", true, false);
                gvNotice.DataBind();

                lblTotalNotice.Text = "ทั้งหมด <font class='hilightGreen'><b>" + gvNotice.Rows.Count.ToString("#,##0") + "</b></font> รายการ";

                upNoticeAll.Update();
                mpeNoticeAll.Show(); 
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnPopupNoticeAllClose_Click(object sender, EventArgs e)
        {
            mpeNoticeAll.Hide();
        }

        protected void gvNotice_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton imbNoticeImage = (ImageButton)e.Row.FindControl("imbNoticeImage");
                imbNoticeImage.ImageUrl = Page.ResolveUrl("~" + imbNoticeImage.CommandArgument);
                imbNoticeImage.OnClientClick = AppUtil.GetNoticeDownloadScript(Page, imbNoticeImage.CommandArgument, "preview");

                ImageButton imbFile = (ImageButton)e.Row.FindControl("imbPopupNoticeDownload");
                imbFile.OnClientClick = AppUtil.GetNoticeDownloadScript(Page, imbFile.CommandArgument, "downloadfile");
            }
        }
    }
}