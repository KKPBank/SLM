using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using SLS.Service.Utilities;

namespace SLS.Service.Request
{
    ///<summary>
    /// <br>Class Name : UpdateLeadRequest</br>
    /// <br>Purpose    : To manage request for UpdateLead operation.</br>
    /// <br>Author     : TikumpornA.</br>
    ///</summary>
    [MessageContract(WrapperNamespace = ApplicationConstant.LEAD_SERVICE_UPDATE_NS)]
    public class UpdateLeadRequest
    {
        [MessageHeader(Name = ApplicationConstant.REQUEST_HEADER, Namespace = ApplicationConstant.LEAD_SERVICE_UPDATE_NS)]
        public Header RequestHeader { get; set; }

        [MessageBodyMember(Name = ApplicationConstant.REQUEST_DETAIL, Namespace = ApplicationConstant.LEAD_SERVICE_UPDATE_NS)]
        public string RequestXml { get; set; }
    }
}