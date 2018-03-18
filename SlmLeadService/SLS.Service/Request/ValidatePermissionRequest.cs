using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using SLS.Service.Utilities;

namespace SLS.Service.Request
{
    ///<summary>
    /// <br>Class Name : ValidatePermissionRequest</br>
    /// <br>Purpose    : To manage request for ValidatePermission operation.</br>
    /// <br>Author     : Attapong.Jan</br>
    ///</summary>
    [MessageContract(WrapperNamespace = ApplicationConstant.LEAD_SERVICE_VALIDATEPERMISSION_NS)]
    public class ValidatePermissionRequest
    {
        [MessageHeader(Name = ApplicationConstant.REQUEST_HEADER, Namespace = ApplicationConstant.LEAD_SERVICE_VALIDATEPERMISSION_NS)]
        public Header RequestHeader { get; set; }

        [MessageBodyMember(Name = ApplicationConstant.REQUEST_DETAIL, Namespace = ApplicationConstant.LEAD_SERVICE_VALIDATEPERMISSION_NS)]
        public string RequestXml { get; set; }
    }
}