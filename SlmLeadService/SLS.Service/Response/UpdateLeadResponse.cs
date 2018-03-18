using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using SLS.Service.Utilities;

namespace SLS.Service.Response
{
    ///<summary>
    /// <br>Class Name : UpdateLeadResponse</br>
    /// <br>Purpose    : To manage response for UpdateLead operation.</br>
    /// <br>Author     : TikumpornA.</br>
    ///</summary>
    [MessageContract(WrapperNamespace = ApplicationConstant.LEAD_SERVICE_UPDATE_NS)]
    public class UpdateLeadResponse
    {
        [MessageHeader(Name = ApplicationConstant.RESPONSE_HEADER, Namespace = ApplicationConstant.LEAD_SERVICE_UPDATE_NS)]
        public Header ResponseHeader { get; set; }

        [MessageBodyMember(Name = ApplicationConstant.RESPONSE_STATUS, Namespace = ApplicationConstant.LEAD_SERVICE_UPDATE_NS)]
        public string ResponseXml { get; set; }
    }
}