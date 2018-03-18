using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS.Resource
{
    /// <summary>
    /// <br>Class Name : ServiceException</br>
    /// <br>Purpose    : To handle exception of the service.</br>
    /// <br>Author     : TikumpornA.</br>
    /// </summary>
    public class ServiceException : Exception
    {
        public string ResponseCode { get; set; }
        public string ResponseDesc { get; set; }

        public ServiceException(string reponseCode, string responseDesc) : base(responseDesc, null)
        {
            ResponseCode = reponseCode;
            ResponseDesc = responseDesc;
        }

        public ServiceException(string reponseCode, string responseDesc, string message, Exception innerException)
            : base(message, innerException)
        {
            ResponseCode = reponseCode;
            ResponseDesc = responseDesc;
        }

    }
}
