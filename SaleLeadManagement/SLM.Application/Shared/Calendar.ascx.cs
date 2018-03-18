using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLM.Application.Shared
{
    public partial class Calendar : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            calendar.BehaviorID = txtDate.ClientID;
        }

        public DateTime DateValue
        {
            get { return GetDate(); }
            set { SetDate(value); }
        }

        public bool Enabled
        {
            get { return imgCalendar.Style["display"] == ""; }
            set { imgCalendar.Style.Add("display", (value ? "" : "none")); }
        }

        public bool HideClearButton
        {
            set { imbClear.Visible = !value; }
        }

        private DateTime GetDate()
        {
            DateTime d;

            if (txtDate.Text.Length == 10)
            {
                string[] tx = txtDate.Text.Split('/');
                try
                {
                    d = new DateTime(Convert.ToInt32(tx[2]), Convert.ToInt32(tx[1]), Convert.ToInt32(tx[0]));

                }
                catch { d = new DateTime(); }
            }
            else
                d = new DateTime();

            return d;
        }

        private void SetDate(DateTime date)
        {
            if (date.Year == 1)
                txtDate.Text = "";
            else
                txtDate.Text = date.Day.ToString("00") + "/" + date.Month.ToString("00") + "/" + (date.Year).ToString("0000");
        }
    }
}