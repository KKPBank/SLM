using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using SLS.Service.Utilities;

namespace SLS.Service.Response
{
    ///<summary>
    /// <br>Class Name : ValidatePermissionResponse</br>
    /// <br>Purpose    : To manage request for ValidatePermission operation.</br>
    /// <br>Author     : Attapong.Jan</br>
    ///</summary>
    [MessageContract(WrapperNamespace = ApplicationConstant.LEAD_SERVICE_VALIDATEPERMISSION_NS)]
    public class ValidatePermissionResponse
    {
        [MessageHeader(Name = ApplicationConstant.RESPONSE_HEADER, Namespace = ApplicationConstant.LEAD_SERVICE_VALIDATEPERMISSION_NS)]
        public Header ResponseHeader { get; set; }

        [MessageBodyMember(Name = ApplicationConstant.RESPONSE_STATUS, Namespace = ApplicationConstant.LEAD_SERVICE_VALIDATEPERMISSION_NS)]
        public string ResponseXml { get; set; }
    }
}