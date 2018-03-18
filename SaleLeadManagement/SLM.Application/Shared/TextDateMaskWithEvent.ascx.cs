using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLM.Application.Shared
{
    public partial class TextDateMaskWithEvent : System.Web.UI.UserControl
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetScript();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void SetScript()
        {
            string script = @"var date = document.getElementById('" + txtDate.ClientID + @"').value;
                                if (date != '')
                                {
                                    var tmp = date.split('/')
                                    var day = parseInt(tmp[0], 10);
                                    var month = parseInt(tmp[1], 10);
                                    var year = parseInt(tmp[2], 10);
                                     if (month < 1 || month > 12) 
                                     {
                                        alert('Invalid Month');
                                        document.getElementById('" + txtDate.ClientID + @"').focus(); 
                                        document.getElementById('" + txtDate.ClientID + @"').select(); return false;                                      
                                     }
                                     if (month == 1 || month == 3 ||month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
                                     {
                                        if (day < 1 || day > 31) 
                                        {
                                            alert('Invalid Day');
                                            document.getElementById('" + txtDate.ClientID + @"').focus(); 
                                            document.getElementById('" + txtDate.ClientID + @"').select(); return false; 
                                        }
                                     }
                                     else if (month == 4 || month == 6 ||month == 9 || month == 11)
                                     {
                                        if (day < 1 || day > 30) 
                                        {
                                            alert('Invalid Day');
                                            document.getElementById('" + txtDate.ClientID + @"').focus(); 
                                            document.getElementById('" + txtDate.ClientID + @"').select(); return false; 
                                        }
                                     }
                                     else
                                     {
                                        if (year % 4 == 0)
                                        {
                                            if (day < 1 || day > 29)    
                                            {
                                                alert('Invalid Day');
                                                document.getElementById('" + txtDate.ClientID + @"').focus(); 
                                                document.getElementById('" + txtDate.ClientID + @"').select(); return false; 
                                            }
                                        }
                                        else 
                                        {
                                            if (day < 1 || day > 28)
                                            {
                                                alert('Invalid Day');
                                                document.getElementById('" + txtDate.ClientID + @"').focus(); 
                                                document.getElementById('" + txtDate.ClientID + @"').select(); return false;
                                            }
                                        }
                                     }
                                }";

            txtDate.Attributes.Add("onchange", script);
        }

        public DateTime DateValue
        {
            get { return GetDate(); }
            set { SetDate(value); }
        }

        public bool Enabled
        {
            set
            {
                imgCalendar.Style.Add("display", (value ? "" : "none"));
                txtDate.ReadOnly = !value;
                txtDate.CssClass = value ? "Textbox" : "TextboxView";
            }
            get
            {
                return !txtDate.ReadOnly;
            }
        }
        public Unit Width
        {
            set { txtDate.Width = value; }
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
            // fix firefox bug, control doesn't bind when press F5
            txtDate.Attributes.Add("autocomplete", "off");
            if (date.Year == 1)
                txtDate.Text = "";
            else
                txtDate.Text = date.Day.ToString("00") + "/" + date.Month.ToString("00") + "/" + (date.Year).ToString("0000");
        }

        protected void txtDate_OnTextChanged(object sender, EventArgs e)
        {
            if (this.OnTextChanged != null)
                OnTextChanged(sender, e);
        }

        public event EventHandler OnTextChanged;
    }
}