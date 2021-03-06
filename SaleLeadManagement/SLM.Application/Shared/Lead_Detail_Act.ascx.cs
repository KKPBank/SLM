﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using SLM.Biz;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Application.Utilities;

namespace SLM.Application.Shared
{
    public partial class Lead_Detail_Act : Lead_Detail_Master
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LeadInfo));
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //comment by nung 20170505
            //BuildCombo(cmbActIssueBranch, SlmScr010Biz.GetIssueBranchData());

            BuildCombo(cmbActIssueBranch, SlmScr010Biz.GetIssueActiveBranchData());
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private void BuildCombo(DropDownList cmb, List<ControlListData> lst) { BuildCombo(cmb, lst, ""); }
        private void BuildCombo(DropDownList cmb, List<ControlListData> lst, string Blanktext)
        {
            cmb.DataSource = lst;
            cmb.DataTextField = "TextField";
            cmb.DataValueField = "ValueField";
            cmb.DataBind();
            cmb.Items.Insert(0, new ListItem(Blanktext, ""));
        }


        // validate in control
        private bool ValidateData(string val)
        {
            int i = 0;

            //Comment on 2017/08/28, Remove validate actNo
            //if ((val == "" || val == "actno") && txtActNo.Text.Trim() == "") { i++; vtxtActNo.Text = "กรุณาระบุเลขพรบ."; } else { vtxtActNo.Text = ""; }

            if ((val == "" || val == "actstart") && tdActStart.DateValue.Year == 1) { i++; vtxtActStart.Text = "กรุณาระบุวันที่เริ่มต้นพรบ."; } else { vtxtActStart.Text = ""; }
            if ((val == "" || val == "actend") && tdActEnd.DateValue.Year == 1) { i++; vtxtActEnd.Text = "กรุณาระบุวันที่สิ้นสุดพรบ."; } else { vtxtActEnd.Text = ""; }
            if ((val == "" || val == "actissuebranch") && cmbActIssue.SelectedValue == "1" && cmbActIssueBranch.SelectedIndex <= 0) { i++; vtxtActIssueBranch.Text = "กรุณาระบุสาขาที่ออก"; } else { vtxtActIssueBranch.Text = ""; }
            if ((val == "" || val == "details") && txtDetails.Text.Trim().Length > AppConstant.TextMaxLength) { i++; vtxtDetail.Text = String.Format("<br />ไม่สามารถบันทึกรายละเอียดเกิน {0} ตัวอักษรได้", AppConstant.TextMaxLength); } else { vtxtDetail.Text = ""; }

            return i == 0;
        }
        protected void txtActNo_TextChanged(object sender, EventArgs e)
        {
            ValidateData("actno");
        }
        protected void tdActStart_OnTextChanged(object sender, EventArgs e)
        {
            ValidateData("actstart");
        }
        protected void tdActEnd_OnTextChanged(object sender, EventArgs e)
        {
            ValidateData("actend");
        }
        protected void cmbActIssueBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateData("actissuebranch");
        }



        // load and save data override
        public override bool SaveData(string leadID, string UserID)
        {
            bool ret = true;

            try
            {
                // prepare data
                var ins = ctlLeadIns.GetInsData();
                var addrLst = ctlLeadIns.GetInsAddrData();


                ins.slm_ActNo = txtActNo.Text.Trim();
                ins.slm_ActStartCoverDate = tdActStart.DateValue;
                ins.slm_ActEndCoverDate = tdActEnd.DateValue;
                ins.slm_ActIssuePlace = SLMUtil.SafeInt(cmbActIssue.SelectedValue);
                ins.slm_ActIssueBranch = cmbActIssueBranch.SelectedValue;
                


                LeadData ldata = new LeadData();
                ldata.TicketId = CommonData.TicketID;
                ldata.TitleId = CommonData.TitleID;
                ldata.Name = CommonData.FName;
                ldata.LastName = CommonData.LName;
                ldata.ChannelId = CommonData.ChannelID;
                ldata.CampaignId = CommonData.CampaignID;
                ldata.TelNo_1 = CommonData.Phone1;
                ldata.TelNo_2 = CommonData.Phone2;
                ldata.Owner_Branch = CommonData.Branch;
                ldata.Owner = CommonData.Owner;
                ldata.IsCustomer = ins.IsCustomer ? "Y" : "N";
                ldata.CardType = SLMUtil.SafeInt(ins.CardType);
                ldata.CitizenId = ins.CitizenId;
                ldata.CountryId = ins.CountryId;
                ldata.slm_SubStatus = "15";
                ldata.Detail = txtDetails.Text.Trim();
                if (ins.Birthdate.Year > 1) ldata.Birthdate = ins.Birthdate;
                ldata.Occupation = SLMUtil.SafeInt(ins.Occupation);
                ldata.TicketIdRefer = CommonData.TicketIDRefer;
                ldata.ProvinceRegis = string.IsNullOrEmpty(ins.ProvinceRegis) ? null : (int?)int.Parse(ins.ProvinceRegis);

                //owner, delegate
                ldata.HasNewOwner = CommonData.ActOwner;
                ldata.HasNewDelegate = CommonData.ActDelegate;


                //owner
                if (CommonData.Owner != "")                 //owner lead
                {
                    ldata.Owner = CommonData.Owner;
                    ldata.NewOwner = CommonData.Owner;
                    ldata.NewOwner2 = CommonData.Owner;
                    StaffData StaffData = SlmScr010Biz.GetStaffData(CommonData.Owner);
                    if (StaffData != null)
                    {
                        ldata.StaffId = Convert.ToInt32(StaffData.StaffId);
                        //LData.Owner_Branch = StaffData.BranchCode;
                    }
                }

                ldata.slmOldOwner = CommonData.OldOwner;
                ldata.OldOwner = CommonData.OldOwner;
                ldata.OldOwner2 = CommonData.OldOwner;
                ldata.Type2 = CommonData.Type2;
                ldata.Delegate_Branch = CommonData.DelegateBranch;
                ldata.Delegate = CommonData.DelegateLead;

                //delegate
                if (!string.IsNullOrEmpty(CommonData.DelegateFlag))
                {
                    ldata.Delegate_Flag = Convert.ToDecimal(CommonData.DelegateFlag);  //Add
                    if (ldata.Delegate_Flag == 1)
                    {
                        ldata.Delegate_Branch = CommonData.DelegateBranch;
                        ldata.Delegate = CommonData.DelegateLead;
                        ldata.NewDelegate = CommonData.DelegateLead;
                        ldata.OldDelegate = CommonData.OldDelegateLead;
                        ldata.Type = CommonData.Type;
                    }
                }
                else
                {
                    ldata.Delegate_Flag = 0;
                }

                if (string.IsNullOrEmpty(ldata.Delegate))
                {
                    ldata.Delegate_Flag = 0;
                }


                if (ldata.CardType == 0) ldata.CardType = null;
                if (ldata.Occupation == 0) ldata.Occupation = null;

                // get create by branch
                StaffData createbyData = SlmScr010Biz.GetStaffData(HttpContext.Current.User.Identity.Name);
                if (createbyData != null)
                    ldata.CreatedBy_Branch = createbyData.BranchCode;

                // saving
                string ticketid;
                LeadInfoBiz bz = new LeadInfoBiz();
                if (!bz.SaveInsData(ldata, ins, addrLst, UserID, out ticketid, 2)) throw new Exception(bz.ErrorMessage);


                // post action
                CommonData.R_TicketID = ticketid;

                if (ldata.HasAdamsUrl)
                {
                    CommonData.R_Message = "ต้องการแนบเอกสารต่อใช่หรือไม่?";
                    CommonData.R_HasAdams = true;
                }
                else
                {
                    CommonData.R_Message = "ต้องการไปหน้าแสดงรายละเอียดผู้มุ่งหวังใช่หรือไม่?";
                    CommonData.R_HasAdams = false;
                }

            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Debug(message);
                ret = false;
                CommonData.R_Message = message;
            }
            return ret;
        }
        public override bool LoadData(LeadData ld, bool isCopy = false, string _type = "")
        {
            bool ret = true;

            try
            {
                LeadInfoBiz.LeadDetailInsData ins = new LeadInfoBiz.LeadDetailInsData();
                List<LeadInfoBiz.LeadDetailAddress> addrLst = new List<LeadInfoBiz.LeadDetailAddress>();

                LeadInfoBiz bz = new LeadInfoBiz();
                if (!bz.GetInsDetail(ld.TicketId, out ins, out addrLst, Page.User.Identity.Name)) if (!isCopy || _type == "1" || _type == "2" || _type == "3") throw new Exception(bz.ErrorMessage);// throw new Exception(bz.ErrorMessage);

                // set for current control
                if (!isCopy)
                {
                    txtActNo.Text = ins.slm_ActNo;
                    txtDetails.Text = ld.Detail;
                }

                if (!isCopy || _type == "2" || _type == "3")
                {
                    tdActStart.DateValue = ins.slm_ActStartCoverDate == null ? new DateTime() : ins.slm_ActStartCoverDate.Value;
                    tdActEnd.DateValue = ins.slm_ActEndCoverDate == null ? new DateTime() : ins.slm_ActEndCoverDate.Value;
                    AppUtil.SetComboValue(cmbActIssue, ins.slm_ActIssuePlace.ToString());

                    ListItem item = cmbActIssueBranch.Items.FindByValue(ins.slm_ActIssueBranch);
                    if (item != null)
                    {
                        AppUtil.SetComboValue(cmbActIssueBranch, ins.slm_ActIssueBranch);
                    }
                    else
                    {
                        if (!isCopy)
                        {
                            var branchName = SlmScr010Biz.GetBranchName(ins.slm_ActIssueBranch);
                            if (!string.IsNullOrEmpty(branchName))
                            {
                                cmbActIssueBranch.Items.Insert(1, new ListItem(branchName, ins.slm_ActIssueBranch));
                                cmbActIssueBranch.SelectedIndex = 1;
                            }
                        }
                    }

                    SetActIssueBranch();
                }

                if (isCopy && _type != "1" && _type != "2" && _type != "3") // copy from default
                {
                    if (ins == null) ins = new LeadInfoBiz.LeadDetailInsData();
                    ins.IsCustomer = ld.IsCustomer == "Y";
                    ins.CardType = ld.CardType.ToString();
                    ins.CitizenId = ld.CitizenId;
                    ins.CountryId = ld.CountryId;
                    ins.Birthdate = ld.Birthdate ?? new DateTime();
                    ins.Occupation = ld.Occupation.ToString();
                    ins.slm_ContractNo = ld.ContractNo;
                    ins.slm_RedbookYearGroup = ld.slm_RedbookYearGroup;
                    ins.slm_RedbookBrandCode = ld.slm_RedbookBrandCode;
                    ins.slm_RedbookKKKey = ld.slm_RedbookKKKey;
                    ins.slm_RedbookModelCode = ld.slm_RedbookModelCode;
                    ins.slm_LicenseNo = ld.LicenseNo;
                    ins.IsCustomer = ld.IsCustomer == "1";

                    if (addrLst == null) addrLst = new List<LeadInfoBiz.LeadDetailAddress>();
                    if (addrLst.Count == 0)
                    {
                        addrLst.Add(new LeadInfoBiz.LeadDetailAddress()
                        {
                            slm_AddressType = "C",
                            slm_AddressNo = ld.AddressNo,
                            slm_BuildingName = ld.BuildingName,
                            slm_Floor = ld.Floor,
                            slm_Soi = ld.Soi,
                            slm_Street = ld.Street,
                            slm_Province = ld.Province,
                            slm_Amphur = ld.Amphur,
                            slm_Tambon = ld.Tambon,
                            slm_PostalCode = ld.PostalCode
                        });
                    }
                }

                // set for share control
                ctlLeadIns.SetInsData(ins, isCopy);
                ctlLeadIns.SetInsAddrData(addrLst);
            }
            catch (Exception ex)
            {
                ret = false;
                throw ex;
            }
            return ret;
        }
        public override bool ValidateData()
        {
            bool ret = true;

            try
            {
                bool r1 = ctlLeadIns.ValidateData();
                bool r2 = ValidateData("");
                ret = r1 && r2;
            }
            catch 
            {
                ret = false;
            }
            return ret;
        }
        public override void SetControlMode(CtlMode ctlMode)
        {
            ctlLeadIns.SetControlMode(ctlMode);
            switch (ctlMode)
            {
                case CtlMode.New:
                    break;

                case CtlMode.View:
                    break;

                case CtlMode.Edit:
                    break;
            }
        }
        public override DropDownList GetComboCardType()
        {
            return ctlLeadIns.GetComboCardType();
        }
        public override DropDownList GetComboContry()
        {
            return ctlLeadIns.GetComboCountry();
        }
        public override TextBox GetTextBoxCitizenId()
        {
            return ctlLeadIns.GetTextBoxCitizenId();
        }

        protected void cmbActIssue_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetActIssueBranch();
        }

        private void SetActIssueBranch()
        {
            if (cmbActIssue.SelectedValue == "1")
                cmbActIssueBranch.Enabled = true;
            else
            {
                cmbActIssueBranch.Enabled = false;
                cmbActIssueBranch.SelectedIndex = 0;
                vtxtActIssueBranch.Text = "";
            }
            reqActBranch.Visible = cmbActIssue.SelectedValue == "1";
        }

        protected void txtDetails_TextChanged(object sender, EventArgs e)
        {
            ValidateData("details");
        }
    }
}