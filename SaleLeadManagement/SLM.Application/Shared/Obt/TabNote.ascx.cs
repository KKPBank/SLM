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

namespace SLM.Application.Shared.Obt
{
    [Obsolete("seems to be unused", true)]
    public partial class TabNote : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(TabNote));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AppUtil.SetNotThaiCharacter(txtEmailTo);
            //AppUtil.SetMultilineMaxLength(txtNoteDetail, "lblMsg2", AppConstant.TextMaxLength.ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void InitialControl(LeadData data)
        {
            try
            {
                if (data.ISCOC == "1")
                {
                    btnAddNote.Visible = (data.COCCurrentTeam == SLMConstant.COCTeam.Marketing ? true : false);
                    //cbNoteFlag.Visible = (data.COCCurrentTeam == SLMConstant.COCTeam.Marketing ? true : false);
                }
                else
                {
                    btnAddNote.Visible = true;
                    //cbNoteFlag.Visible = true;
                }
                txtTicketID.Text = data.TicketId;
                txtPreleadId.Text = data.PreleadId != null ? data.PreleadId.Value.ToString() : "";
                txtContractNo.Text = data.ContractNo;
                txtFirstname.Text = data.Name;
                txtLastname.Text = data.LastName;
                txtOwnerLead.Text = data.OwnerName;
                txtCampaign.Text = data.CampaignName;
                txtTelNo1.Text = data.TelNo_1;
                txtExt1.Text = data.Ext_1;
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

        private void DoBindGridview()
        {
            List<NoteHistoryData> result = SlmScr009Biz.SearchNoteHistory(txtTicketID.Text.Trim(), txtPreleadId.Text.Trim());
            BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), 0);
            //cbNoteFlag.Enabled = gvNoteHistory.Rows.Count > 0 ? true : false;
            upResult.Update();
        }

        private void CheckEmailSubject()
        {
            if (cbSendEmail.Checked)
            {
                txtEmailSubject.Enabled = true;
                txtEmailSubject.Text = "SLM: Ticket: " + txtTicketID.Text.Trim();
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
                _log.Debug(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion
    }
}