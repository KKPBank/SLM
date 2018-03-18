using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using SLS.Service.Utilities;

namespace SLS.Service.Request
{
    ///<summary>
    /// <br>Class Name : SearchLeadRequest</br>
    /// <br>Purpose    : To manage request for SearchLead operation.</br>
    /// <br>Author     : TikumpornA.</br>
    ///</summary>
    [MessageContract(WrapperNamespace = ApplicationConstant.LEAD_SERVICE_SEARCH_NS)]
    public class SearchLeadRequest
    {
        [MessageHeader(Name = ApplicationConstant.REQUEST_HEADER, Namespace = ApplicationConstant.LEAD_SERVICE_SEARCH_NS)]
        public Header RequestHeader { get; set; }

        [MessageBodyMember(Name = ApplicationConstant.REQUEST_DETAIL, Namespace = ApplicationConstant.LEAD_SERVICE_SEARCH_NS)]
        public string RequestXml { get; set; }
    }

    ///<summary>
    /// <br>Class Name : SearchLeadRequest</br>
    /// <br>Purpose    : To manage request for SearchLead operation.</br>
    /// <br>Author     : TikumpornA.</br>
    ///</summary>
    //public class SearchLeadFMSRequest
    //{
    //    [MessageHeader(Name = ApplicationConstant.REQUEST_HEADER, Namespace = ApplicationConstant.LEAD_SERVICE_SEARCH_NS)]
    //    public Header RequestHeader { get; set; }

    //    [MessageBodyMember(Name = ApplicationConstant.REQUEST_TICKETID, Namespace = ApplicationConstant.LEAD_SERVICE_SEARCH_NS)]
    //    public string TicketId { get; set; }
    //}
}