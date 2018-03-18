using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLS.Service
{
    public class Header
    {
        public string Version { get; set; }
        public string Encoding { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ChannelID { get; set; }
    }
}