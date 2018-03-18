using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SLM.Resource.Data;

namespace SLM.Application.Shared
{
    public abstract class Lead_Detail_Master : System.Web.UI.UserControl
    {
        public abstract bool SaveData(string leadID, string UserID);
        public abstract bool LoadData(LeadData ld, bool isCopy = false, string _type = "");
        public abstract bool ValidateData();
        public abstract void SetControlMode(CtlMode ctlMode);
        public abstract DropDownList GetComboCardType();
        public abstract DropDownList GetComboContry();
        public abstract TextBox GetTextBoxCitizenId();

        public LeadControlCommonData CommonData
        {
            get { return ViewState["LEAD_DETAIL_COMMONDATA"] as LeadControlCommonData; }
            set { ViewState["LEAD_DETAIL_COMMONDATA"] = value; }
        }
        
        public enum CtlMode
        {
           New,
           Edit,
           View
        }
    }
}
