using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class NoticeData
    {
        public int? NoticeId { get; set; }
        public string Topic { get; set; }
        public string ActiveStatus { get; set; }
        public string StatusDesc { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ImageName { get; set; }
        public string ImageVirtualPath { get; set; }
        public string ImagePhysicalPath { get; set; }
        public string FileName { get; set; }
        public string FileVirtualPath { get; set; }
        public string FilePhysicalPath { get; set; }
    }
}
