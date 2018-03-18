using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SLS.Service.Request;
using SLS.Service.Response;

namespace SLS.Service
{
    [ServiceContract]
    public interface ILeadService
    {
        [OperationContract]
        InsertLeadResponse InsertLead(InsertLeadRequest request);

        [OperationContract]
        UpdateLeadResponse UpdateLead(UpdateLeadRequest request);

        [OperationContract]
        SearchLeadResponse SearchLead(SearchLeadRequest request);

        [OperationContract]
        ValidatePermissionResponse ValidatePermission(ValidatePermissionRequest request);

        //[OperationContract]
        //string SearchLeadFMS(SearchLeadFMSRequest request);
    }
}
