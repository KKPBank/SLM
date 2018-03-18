using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal.Models;
using SLM.Resource.Data;

namespace SLM.Biz
{
    public class TitleNameBiz
    {
        public static List<TitleNameData> ListTitleName()
        {
            KKSlmMsTitleNameModel title = new KKSlmMsTitleNameModel();
            return title.ListTitleName();
        }
    }
}
