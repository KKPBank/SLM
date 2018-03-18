using log4net;
using SLM.Application.CSMSRServiceProxy;
using SLM.Application.CSMUserServiceProxy;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServiceHeader = SLM.Application.CSMUserServiceProxy.WebServiceHeader;
using WebServiceHeader2 = SLM.Application.CSMSRServiceProxy.WebServiceHeader;

namespace SLM.Application.Services
{
    public class CSMService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CSMService));

        public static InsertOrUpdateUserResponse InsertOrUpdateUser(int actionType, StaffDataManagement data, string username)
        {
            _log.Info("===== [Start] Call WS Submit Staff Data to CSM: InsertOrUpdateUser =====");

            InsertOrUpdateUserRequest request = new InsertOrUpdateUserRequest();
			request.Header = new WebServiceHeader();
            request.Header.service_name = AppConstant.GetCSMServiceName;
            request.Header.user_name = AppConstant.GetCSMUsername;
            request.Header.system_code = AppConstant.GetCSMSystemCode;
            request.Header.password = AppConstant.GetCSMPassword;
            request.Header.command = "InsertOrUpdateUser";
            request.ActionType = actionType;
            request.WindowsUsername = data.Username;
            request.EmployeeCodeNew = data.EmpCode;

            if (actionType == 2)
                request.EmployeeCodeOld = data.EmpCodeOld;

            if (!string.IsNullOrEmpty(data.StaffNameTH))
            {
                var index = data.StaffNameTH.IndexOf(" ");
                if (index != -1)
                {
                    request.FirstName = data.StaffNameTH.Substring(0, index).Trim();
                    request.LastName = data.StaffNameTH.Substring(index, data.StaffNameTH.Length - index).Trim();
                }
                else
                {
                    request.FirstName = data.StaffNameTH;
                    request.LastName = string.Empty;
                }
            }
            
            if (data.HeadStaffId.HasValue)
            {
                var headStaffEmployeeCode = SlmScr019Biz.GetStaffEmployeeCode(data.HeadStaffId.Value);
                request.SupervisorEmployeeCode = headStaffEmployeeCode;
            }

            if (!string.IsNullOrEmpty(data.UserType))
            {
                request.IsGroup = data.UserType.ToUpper() == "G";
            }

            request.Phone1 = data.TelNo;
            request.Phone2 = data.TelNo2;
            request.Phone3 = data.TelNo3;

            if (data.StaffTypeId.HasValue)
            {
                var staffTypeDesc = SlmScr019Biz.GetStaffTypeDesc(data.StaffTypeId.Value);
                request.RoleSale = staffTypeDesc;
            }

            //request.BranchCode = data.BranchCode;
			//get branchCodeNew for CSM
            request.BranchCode = SlmScr019Biz.GetBranchCodeNew(data.BranchCode);

            if (data.RoleServiceId.HasValue)
            {
                var roleServiceCode = SlmScr019Biz.GetRoleServiceCode(data.RoleServiceId.Value);
                request.RoleCode = roleServiceCode;
            }

            request.Status = (data.Is_Deleted ?? 0) == 0 ? 1 : 0;

            if (data.PositionId.HasValue)
            {
                var positionNameAbb = SlmScr019Biz.GetPositionNameAbb(data.PositionId.Value);
                request.PositionCode = positionNameAbb;
            }

            request.MarketingCode = data.MarketingCode;
            request.MarketingFlag = data.IsMarketing;
            request.Email = data.StaffEmail;

            request.MarketingTeam = data.Team;

            if (data.DepartmentId.HasValue)
            {
                var departmentName = SlmScr019Biz.GetDepartmentName(data.DepartmentId.Value);
                request.Line = departmentName;
            }

            if (data.Level.HasValue)
            {
                var levelName = SlmScr019Biz.GetLevelName(data.Level.Value);
                request.Rank = levelName;
            }

            if (data.Category.HasValue)
            {
                var categoryName = SlmScr019Biz.GetCategoryName(data.Category.Value);
                request.EmployeeType = categoryName;
            }

            if (data.Host.HasValue)
            {
                var hostName = SlmScr019Biz.GetHostName(data.Host.Value);
                request.CompanyName = hostName;
            }

            if (data.TeamTelesale.HasValue)
            {
                var teamTelesaleName = SlmScr019Biz.GetTeamTelesaleName(data.TeamTelesale.Value);
                request.TelesaleTeam = teamTelesaleName;
            }

            request.ActionUsername = username;

            LogRequest_InsertOrUpdateUser(request);

            try
            {
                var start = DateTime.Now;
                _log.DebugFormat("Start Call InsertOrUpdateUser at {0:dd/MM/yyyy HH:mm:ss}", start);

                CSMUserServiceClient client = new CSMUserServiceClient();
                var response = client.InsertOrUpdateUser(request);

                var stop = DateTime.Now;
                _log.DebugFormat("End Call InsertOrUpdateUser at {0:dd/MM/yyyy HH:mm:ss} (Elapsed Time={1} seconds)", stop, stop.Subtract(start).TotalSeconds);

                if (response.IsSuccess)
                {
                    _log.Info("===== [End] Call WS Submit Staff Data to CSM: InsertOrUpdateUser (SUCCESS) =====");
					_log.Debug("===== [START] Response Data =====");
                    _log.Debug("IsSuccess=" + response.IsSuccess);
                    _log.Debug("IsNewUser=" + response.IsNewUser);
                    _log.Debug("ErrorCode=" + response.ErrorCode);
                    _log.Debug("ErrorMessage=" + response.ErrorMessage);
                    _log.Debug("===== [END] Response Data =====");
                }
                else
                {
                    _log.Error("===== [End] Call WS Submit Staff Data to CSM: InsertOrUpdateUser (FAIL) =====");
					_log.Error("===== [START] Response Data =====");
                    _log.Error("IsSuccess=" + response.IsSuccess);
                    _log.Error("ErrorCode=" + response.ErrorCode);
                    _log.Error("ErrorMessage=" + response.ErrorMessage);
                    _log.Error("===== [END] Response Data =====");
                }

                //LogResponse_InsertOrUpdateUser(response);

                return response;
            }
            catch (Exception ex)
            {
                _log.Error("===== [End] Call WS Submit Staff Data to CSM: InsertOrUpdateUser (FAIL with Exception) =====", ex);
                throw;
            }
        }

        private static void LogRequest_InsertOrUpdateUser(InsertOrUpdateUserRequest request)
        {
            _log.Debug("===== [START] Request Parameter =====");
            _log.Debug("ActionType=" + request.ActionType);
            _log.Debug("WindowsUsername=" + request.WindowsUsername);
            _log.Debug("EmployeeCodeNew=" + request.EmployeeCodeNew);
            _log.Debug("EmployeeCodeOld=" + request.EmployeeCodeOld);
            _log.Debug("MarketingCode=" + request.MarketingCode);
            _log.Debug("FirstName=" + request.FirstName);
            _log.Debug("LastName=" + request.LastName);
            _log.Debug("Phone1=" + request.Phone1);
            _log.Debug("Phone2=" + request.Phone2);
            _log.Debug("Phone3=" + request.Phone3);
            _log.Debug("Email=" + request.Email);
            _log.Debug("PositionCode=" + request.PositionCode);
            _log.Debug("RoleSale=" + request.RoleSale);
            _log.Debug("MarketingTeam=" + request.MarketingTeam);
            _log.Debug("BranchCode=" + request.BranchCode);
            _log.Debug("SupervisorEmployeeCode=" + request.SupervisorEmployeeCode);
            _log.Debug("Line=" + request.Line);
            _log.Debug("Rank=" + request.Rank);
            _log.Debug("EmployeeType=" + request.EmployeeType);
            _log.Debug("CompanyName=" + request.CompanyName);
            _log.Debug("TelesaleTeam=" + request.TelesaleTeam);
            _log.Debug("RoleCode=" + request.RoleCode);
            _log.Debug("IsGroup=" + request.IsGroup);
            _log.Debug("Status=" + request.Status);
            _log.Debug("ActionUsername=" + request.ActionUsername);
            _log.Debug("MarketingFlag=" + request.MarketingFlag);
            _log.Debug("===== [END] Request Parameter =====");
        }

        private static void LogResponse_InsertOrUpdateUser(InsertOrUpdateUserResponse response)
        {
            _log.Debug("===== [START] Response Data =====");
            _log.Debug("IsSuccess=" + response.IsSuccess);
            _log.Debug("IsNewUser=" + response.IsNewUser);
            _log.Debug("ErrorCode=" + response.ErrorCode);
            _log.Debug("ErrorMessage=" + response.ErrorMessage);
            _log.Debug("===== [END] Response Data =====");
        }

        public static SearchSRResponse GetSrOwnerDataTab18_3(string employeeCode, int pageIndex, int pageSize)
        {

            var request = new SearchSRRequest();
            request.Header = new WebServiceHeader2();
            request.Header.service_name = AppConstant.GetCSMServiceName;
            request.Header.user_name = AppConstant.GetCSMUsername;
            request.Header.system_code = AppConstant.GetCSMSystemCode;
            request.Header.password = AppConstant.GetCSMPassword;
            request.Header.command = "SearchSr";
            request.EmployeeCodeforOwnerSR = employeeCode;
            request.StartPageIndex = pageIndex;
            request.PageSize = pageSize;

            CSMSRServiceClient client = new CSMSRServiceClient();
            return client.SearchSR(request);
        }

        public static SearchSRResponse GetSrDelegateDataTab18_4(string employeeCode, int pageIndex, int pageSize)
        {
            var request = new SearchSRRequest();
            request.Header = new WebServiceHeader2();
            request.Header.service_name = AppConstant.GetCSMServiceName;
            request.Header.user_name = AppConstant.GetCSMUsername;
            request.Header.system_code = AppConstant.GetCSMSystemCode;
            request.Header.password = AppConstant.GetCSMPassword;
            request.Header.command = "SearchSr";
            request.EmployeeCodeforDelegateSR = employeeCode;
            request.StartPageIndex = pageIndex;
            request.PageSize = pageSize;

            CSMSRServiceClient client = new CSMSRServiceClient();
            return client.SearchSR(request);
        }
    }
}