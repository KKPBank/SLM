using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using SLS.Resource;
using SLS.Resource.Data;
using SLS.Biz;

namespace SLS.Service.Utilities
{
    public class ApplicationUtil
    {
        /// <summary>
        /// <br>Method Name : ConvertToDateTime</br>
        /// <br>Purpose     : To convert date and time string from xml to class DateTime.</br>
        /// </summary>
        /// <param name="date">string format yyyyMMdd</param>
        /// <param name="time">string format HHmmss</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string date, string time)
        {
            try
            {
                if (string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date) || date.Length != 8)
                {
                    return new DateTime();
                }
                else
                {
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);
                    string day = date.Substring(6, 2);

                    if (string.IsNullOrEmpty(time) || string.IsNullOrWhiteSpace(time) || time.Length != 6)
                    {
                        return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                    }
                    else
                    {
                        string hour = time.Substring(0, 2);
                        string min = time.Substring(2, 2);
                        string sec = time.Substring(4, 2);
                        return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(min), int.Parse(sec));
                    }
                }
            }
            catch
            {
                return new DateTime();
            }
        }

        public static bool ValidateDate(string date, string operationFlag, string tagName)
        {
            try
            {
                if (string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date) || date.Length != 8)
                {
                    throw new ServiceException("", "Invalid Date");
                }
                else
                {
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);
                    string day = date.Substring(6, 2);

                    DateTime temp = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                    return true;
                }
            }
            catch(Exception ex)
            {
                if (operationFlag == ApplicationResource.INS_OPERATION)
                {
                    throw new ServiceException(ApplicationResource.INS_INVALID_PARAMETERS_CODE, ApplicationResource.INS_INVALID_PARAMETERS_DESC, "<" + tagName + "> has invalid value, " + ex.Message, null);
                }
                else
                {
                    throw new ServiceException(ApplicationResource.UPD_INVALID_PARAMETERS_CODE, ApplicationResource.UPD_INVALID_PARAMETERS_DESC, "<" + tagName + "> has invalid value, " + ex.Message, null);
                }   
            }
        }

        public static bool ValidateTime(string time, string operationFlag, string tagName)
        {
            try
            {
                if (string.IsNullOrEmpty(time) || string.IsNullOrWhiteSpace(time) || time.Length != 6)
                {
                    throw new ServiceException("", "Invalid Time");
                }
                else
                {
                    int result;
                    int.TryParse(time, out result);
                    if (result == 0)
                    {
                        throw new ServiceException("", "Invalid Time");
                    }

                    int hour = int.Parse(time.Substring(0, 2));
                    int min = int.Parse(time.Substring(2, 2));
                    int sec = int.Parse(time.Substring(4, 2));

                    if (hour < 0 || hour > 23)
                    {
                        throw new ServiceException("", "Invalid Time");
                    }
                    if (min < 0 || min > 59)
                    {
                        throw new ServiceException("", "Invalid Time");
                    }
                    if (sec < 0 || sec > 59)
                    {
                        throw new ServiceException("", "Invalid Time");
                    }
                    
                    return true;
                }
            }
            catch(Exception ex)
            {
                if (operationFlag == ApplicationResource.INS_OPERATION)
                {
                    throw new ServiceException(ApplicationResource.INS_INVALID_PARAMETERS_CODE, ApplicationResource.INS_INVALID_PARAMETERS_DESC, "<" + tagName + "> has invalid value, " + ex.Message, null);
                }
                else
                {
                    throw new ServiceException(ApplicationResource.UPD_INVALID_PARAMETERS_CODE, ApplicationResource.UPD_INVALID_PARAMETERS_DESC, "<" + tagName + "> has invalid value, " + ex.Message, null);
                }  
            }
        }

        public static string GenerateXml(SearchLeadData data, string ticketId, string responseCode, string responseMessage, DateTime responseDate)
        {
            if (data != null)
            {
                XDocument doc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement("ticket", new XAttribute("id", data.TicketId != null ? data.TicketId : string.Empty),
                            new XElement("ticket", data.TicketId != null ? data.TicketId : string.Empty),
                            new XElement("mandatory", new XElement("firstname", data.Firstname != null ? data.Firstname.Trim() : string.Empty),
                                                        new XElement("telNo1", data.TelNo1 != null ? data.TelNo1.Trim() : string.Empty),
                                                        new XElement("campaign", data.Campaign != null ? data.Campaign.Trim() : string.Empty)),
                            new XElement("customerInfo", new XElement("lastname", data.Lastname != null ? data.Lastname.Trim() : string.Empty),
                                                            new XElement("email", data.Email != null ? data.Email.Trim() : string.Empty),
                                                            new XElement("telNo2", data.TelNo2 != null ? data.TelNo2.Trim() : string.Empty),
                                                            new XElement("telNo3", data.TelNo3 != null ? data.TelNo3.Trim() : string.Empty),
                                                            new XElement("extNo1", data.ExtNo1 != null ? data.ExtNo1.Trim() : string.Empty),
                                                            new XElement("extNo2", data.ExtNo2 != null ? data.ExtNo2.Trim() : string.Empty),
                                                            new XElement("extNo3", data.ExtNo3 != null ? data.ExtNo3.Trim() : string.Empty),
                                                            new XElement("BuildingName", data.BuildingName != null ? data.BuildingName.Trim() : string.Empty),
                                                            new XElement("addrNo", data.AddrNo != null ? data.AddrNo.Trim() : string.Empty),
                                                            new XElement("floor", data.Floor != null ? data.Floor.Trim() : string.Empty),
                                                            new XElement("soi", data.Soi != null ? data.Soi.Trim() : string.Empty),
                                                            new XElement("street", data.Street != null ? data.Street.Trim() : string.Empty),
                                                            new XElement("tambol", data.TambolCode != null ? data.TambolCode.Trim() : string.Empty),
                                                            new XElement("amphur", data.AmphurCode != null ? data.AmphurCode.Trim() : string.Empty),
                                                            new XElement("province", data.ProvinceCode != null ? data.ProvinceCode.Trim() : string.Empty),
                                                            new XElement("postalCode", data.PostalCode != null ? data.PostalCode.Trim() : string.Empty),
                                                            new XElement("occupation", data.OccupationCode != null ? data.OccupationCode.Trim() : string.Empty),
                                                            new XElement("baseSalary", data.BaseSalary != null ? data.BaseSalary.Value.ToString("0.00") : string.Empty),
                                                            new XElement("isCustomer", data.IsCustomer != null ? data.IsCustomer.Trim() : string.Empty),
                                                            new XElement("customerCode", data.CustomerCode != null ? data.CustomerCode.Trim() : string.Empty),
                                                            new XElement("dateOfBirth", data.DateOfBirth != null ? data.DateOfBirth.Value.Year + data.DateOfBirth.Value.ToString("MMdd") : string.Empty),
                                                            new XElement("cardType", data.CardTypeDesc != null ? data.CardTypeDesc.Trim() : string.Empty),
                                                            new XElement("cid", data.Cid != null ? data.Cid.Trim() : string.Empty),
                                                            new XElement("country", data.CountryId != null ? data.CountryId.Value.ToString() : string.Empty),
                                                            new XElement("countryCode",data.CountryCode != null ? data.CountryCode.Trim() : string.Empty),
                                                            new XElement("countryDescriptionEn", data.CountryDescriptionEN != null ? data.CountryDescriptionEN.Trim() : string.Empty),
                                                            new XElement("countryDescriptionTh", data.CountryDescriptionTH != null ? data.CountryDescriptionTH.Trim() : string.Empty),
                                                            new XElement("contractNoRefer", data.ContractNoRefer != null ? data.ContractNoRefer.Trim() : string.Empty),
                                                            new XElement("status", data.Status != null ? data.Status.Trim() : string.Empty),
                                                            new XElement("statusDesc", data.StatusDesc != null ? data.StatusDesc.Trim() : string.Empty)),
                            new XElement("customerDetail", new XElement("topic", data.Topic != null ? data.Topic.Trim() : string.Empty),
                                                            new XElement("detail", data.Detail != null ? data.Detail.Trim() : string.Empty),
                                                            new XElement("pathLink", data.PathLink != null ? data.PathLink.Trim() : string.Empty),
                                                            new XElement("telesaleName", data.TelesaleName != null ? data.TelesaleName.Trim() : string.Empty),
                                                            new XElement("availableTime", data.AvailableTime != null ? data.AvailableTime.Trim() : string.Empty),
                                                            new XElement("contactBranch", data.ContactBranch != null ? data.ContactBranch.Trim() : string.Empty)),
                            new XElement("productInfo", new XElement("interestedProdAndType", data.InterestedProdAndType != null ? data.InterestedProdAndType.Trim() : string.Empty),
                                                        new XElement("licenseNo", data.LicenseNo != null ? data.LicenseNo.Trim() : string.Empty),
                                                        new XElement("yearOfCar", data.YearOfCar != null ? data.YearOfCar.Trim() : string.Empty),
                                                        new XElement("yearOfCarRegis", data.YearOfCarRegis != null ? data.YearOfCarRegis.Trim() : string.Empty),
                                                        new XElement("registerProvince", data.RegisterProvinceCode != null ? data.RegisterProvinceCode.Trim() : string.Empty),
                                                        new XElement("brand", data.BrandCode != null ? data.BrandCode.Trim() : string.Empty),
                                                        new XElement("model", data.ModelFamily != null ? data.ModelFamily.Trim() : string.Empty),
                                                        new XElement("submodel", data.SubmodelRedBookNo != null ? data.SubmodelRedBookNo.Trim() : string.Empty),
                                                        new XElement("downPayment", data.DownPayment != null ? data.DownPayment.Value.ToString("0.00") : string.Empty),
                                                        new XElement("downPercent", data.DownPercent != null ? data.DownPercent.Value.ToString("0.00") : string.Empty),
                                                        new XElement("carPrice", data.CarPrice != null ? data.CarPrice.Value.ToString("0.00") : string.Empty),
                                                        new XElement("financeAmt", data.FinanceAmt != null ? data.FinanceAmt.Value.ToString("0.00") : string.Empty),
                                                        new XElement("term", data.Term != null ? data.Term.Trim() : string.Empty),
                                                        new XElement("paymentType", data.PaymentTypeCode != null ? data.PaymentTypeCode.Trim() : string.Empty),
                                                        new XElement("balloonAmt", data.BalloonAmt != null ? data.BalloonAmt.Value.ToString("0.00") : string.Empty),
                                                        new XElement("balloonPercent", data.BalloonPercent != null ? data.BalloonPercent.Value.ToString("0.00") : string.Empty),
                                                        new XElement("plantype", data.Plantype != null ? data.Plantype.Trim() : string.Empty),
                                                        new XElement("coverageDate", data.CoverageDate != null ? data.CoverageDate.Trim() : string.Empty),
                                                        new XElement("accType", data.AccTypeCode != null ? data.AccTypeCode.Trim() : string.Empty),
                                                        new XElement("accPromotion", data.AccPromotionCode != null ? data.AccPromotionCode.Trim() : string.Empty),
                                                        new XElement("accTerm", data.AccTerm != null ? data.AccTerm.Trim() : string.Empty),
                                                        new XElement("interest", data.Interest != null ? data.Interest.Trim() : string.Empty),
                                                        new XElement("invest", data.Invest != null ? data.Invest.Trim() : string.Empty),
                                                        new XElement("loanOd", data.LoanOd != null ? data.LoanOd.Trim() : string.Empty),
                                                        new XElement("loanOdTerm", data.LoanOdTerm != null ? data.LoanOdTerm.Trim() : string.Empty),
                                                        new XElement("slmBank", data.SlmBank != null ? data.SlmBank.Trim() : string.Empty),
                                                        new XElement("slmAtm", data.SlmAtm != null ? data.SlmAtm.Trim() : string.Empty),
                                                        new XElement("otherDetail1", data.OtherDetail1 != null ? data.OtherDetail1.Trim() : string.Empty),
                                                        new XElement("otherDetail2", data.OtherDetail2 != null ? data.OtherDetail2.Trim() : string.Empty),
                                                        new XElement("otherDetail3", data.OtherDetail3 != null ? data.OtherDetail3.Trim() : string.Empty),
                                                        new XElement("otherDetail4", data.OtherDetail4 != null ? data.OtherDetail4.Trim() : string.Empty),
                                                        new XElement("carType", data.CarType != null ? data.CarType.Trim() : string.Empty)),
                            new XElement("channelInfo", new XElement("channelId", data.ChannelId != null ? data.ChannelId.Trim() : string.Empty),
                                                        new XElement("date", data.RequestDate != null ? data.RequestDate.Value.Year + data.RequestDate.Value.ToString("MMdd") : string.Empty),
                                                        new XElement("time", data.RequestDate != null ? data.RequestDate.Value.ToString("HHmmss") : string.Empty),
                                                        new XElement("createUser", data.CreateUser != null ? data.CreateUser.Trim() : string.Empty),
                                                        new XElement("ipaddress", data.Ipaddress != null ? data.Ipaddress.Trim() : string.Empty),
                                                        new XElement("company", data.Company != null ? data.Company.Trim() : string.Empty),
                                                        new XElement("branch", data.Branch != null ? data.Branch.Trim() : string.Empty),
                                                        new XElement("branchNo", data.BranchNo != null ? data.BranchNo.Trim() : string.Empty),
                                                        new XElement("machineNo", data.MachineNo != null ? data.MachineNo.Trim() : string.Empty),
                                                        new XElement("clientServiceType", data.ClientServiceType != null ? data.ClientServiceType.Trim() : string.Empty),
                                                        new XElement("documentNo", data.DocumentNo != null ? data.DocumentNo.Trim() : string.Empty),
                                                        new XElement("commPaidCode", data.CommPaidCode != null ? data.CommPaidCode.Trim() : string.Empty),
                                                        new XElement("zone", data.Zone != null ? data.Zone.Trim() : string.Empty),
                                                        new XElement("transid", data.TransId != null ? data.TransId.Trim() : string.Empty)),
                            //new XElement("result", "1")
                            //,
                            new XElement("productGroupId", data.ProductGroupId != null ? data.ProductGroupId.Trim() : string.Empty),
                            new XElement("productGroupName", data.ProductGroupName != null ? data.ProductGroupName.Trim() : string.Empty),
                            new XElement("productId", data.ProductId != null ? data.ProductId.Trim() : string.Empty),
                            new XElement("productName", data.ProductName != null ? data.ProductName.Trim() : string.Empty),
                            new XElement("campaignName", data.CampaignName != null ? data.CampaignName.Trim() : string.Empty),
                            new XElement("responseCode", responseCode),
                            new XElement("responseMessage", responseMessage),
                            new XElement("responseDate", responseDate.ToString("dd-MM-") + responseDate.Year.ToString()),
                            new XElement("responseTime", responseDate.ToString("HH:mm:ss"))
                            )
                        );

                return "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + doc;
            }
            else
            {
                XDocument doc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement("ticket", new XAttribute("id", ticketId),
                            new XElement("ticket", string.Empty),
                            new XElement("mandatory", new XElement("firstname", string.Empty),
                                                        new XElement("telNo1", string.Empty),
                                                        new XElement("campaign", string.Empty)),
                            new XElement("customerInfo", new XElement("lastname", string.Empty),
                                                            new XElement("email", string.Empty),
                                                            new XElement("telNo2", string.Empty),
                                                            new XElement("telNo3", string.Empty),
                                                            new XElement("extNo1", string.Empty),
                                                            new XElement("extNo2", string.Empty),
                                                            new XElement("extNo3", string.Empty),
                                                            new XElement("BuildingName", string.Empty),
                                                            new XElement("addrNo", string.Empty),
                                                            new XElement("floor", string.Empty),
                                                            new XElement("soi", string.Empty),
                                                            new XElement("street", string.Empty),
                                                            new XElement("tambol", string.Empty),
                                                            new XElement("amphur", string.Empty),
                                                            new XElement("province", string.Empty),
                                                            new XElement("postalCode", string.Empty),
                                                            new XElement("occupation", string.Empty),
                                                            new XElement("baseSalary", string.Empty),
                                                            new XElement("isCustomer", string.Empty),
                                                            new XElement("customerCode", string.Empty),
                                                            new XElement("dateOfBirth", string.Empty),
                                                            new XElement("cardType", string.Empty),
                                                            new XElement("cid", string.Empty),
                                                            new XElement("country", string.Empty),
                                                            new XElement("countryCode", string.Empty),
                                                            new XElement("countryDescriptionEn", string.Empty),
                                                            new XElement("countryDescriptionTh", string.Empty),
                                                            new XElement("contractNoRefer", string.Empty),
                                                            new XElement("status", string.Empty),
                                                            new XElement("statusDesc", string.Empty)),
                            new XElement("customerDetail", new XElement("topic", string.Empty),
                                                            new XElement("detail", string.Empty),
                                                            new XElement("pathLink", string.Empty),
                                                            new XElement("telesaleName", string.Empty),
                                                            new XElement("availableTime", string.Empty),
                                                            new XElement("contactBranch", string.Empty)),
                            new XElement("productInfo", new XElement("interestedProdAndType", string.Empty),
                                                        new XElement("licenseNo", string.Empty),
                                                        new XElement("yearOfCar", string.Empty),
                                                        new XElement("yearOfCarRegis", string.Empty),
                                                        new XElement("registerProvince", string.Empty),
                                                        new XElement("brand", string.Empty),
                                                        new XElement("model", string.Empty),
                                                        new XElement("submodel", string.Empty),
                                                        new XElement("downPayment", string.Empty),
                                                        new XElement("downPercent", string.Empty),
                                                        new XElement("carPrice", string.Empty),
                                                        new XElement("financeAmt", string.Empty),
                                                        new XElement("term", string.Empty),
                                                        new XElement("paymentType", string.Empty),
                                                        new XElement("balloonAmt", string.Empty),
                                                        new XElement("balloonPercent", string.Empty),
                                                        new XElement("plantype", string.Empty),
                                                        new XElement("coverageDate", string.Empty),
                                                        new XElement("accType", string.Empty),
                                                        new XElement("accPromotion", string.Empty),
                                                        new XElement("accTerm", string.Empty),
                                                        new XElement("interest", string.Empty),
                                                        new XElement("invest", string.Empty),
                                                        new XElement("loanOd", string.Empty),
                                                        new XElement("loanOdTerm", string.Empty),
                                                        new XElement("slmBank", string.Empty),
                                                        new XElement("slmAtm", string.Empty),
                                                        new XElement("otherDetail1", string.Empty),
                                                        new XElement("otherDetail2", string.Empty),
                                                        new XElement("otherDetail3", string.Empty),
                                                        new XElement("otherDetail4", string.Empty),
                                                        new XElement("carType", string.Empty)),
                            new XElement("channelInfo", new XElement("channelId", string.Empty),
                                                        new XElement("date", string.Empty),
                                                        new XElement("time", string.Empty),
                                                        new XElement("createUser", string.Empty),
                                                        new XElement("ipaddress", string.Empty),
                                                        new XElement("company", string.Empty),
                                                        new XElement("branch", string.Empty),
                                                        new XElement("branchNo", string.Empty),
                                                        new XElement("machineNo", string.Empty),
                                                        new XElement("clientServiceType", string.Empty),
                                                        new XElement("documentNo", string.Empty),
                                                        new XElement("commPaidCode", string.Empty),
                                                        new XElement("zone", string.Empty),
                                                        new XElement("transid", string.Empty)),
                            //new XElement("result", "0")
                            //,
                            new XElement("productGroupId", string.Empty),
                            new XElement("productGroupName", string.Empty),
                            new XElement("productId", string.Empty),
                            new XElement("productName", string.Empty),
                            new XElement("campaignName", string.Empty),
                            new XElement("responseCode", responseCode),
                            new XElement("responseMessage", responseMessage),
                            new XElement("responseDate", responseDate.ToString("dd-MM-") + responseDate.Year.ToString()),
                            new XElement("responseTime", responseDate.ToString("HH:mm:ss"))
                            )
                        );

                return "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" + doc;
            }
        }

        //เช็กถ้า username เป็นตัวเลขให้ถือว่าเป็น empCode, ให้ส่งไปหา username ในเบส
        public static string ValidateUsername(string username)
        {
            try
            {
                int result;
                if (int.TryParse(username, out result))
                    return LeadServiceBiz.GetUsername(username);    //ถ้าเป็นตัวเลขให้ถือว่าเป็น empCode, ให้ส่งไปหา username
                else
                    return username;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static int SafeInt(string str)
        {
            if (str == null) return 0;
            int r; int.TryParse(str.Replace(",", ""), out r);
            return r;
        }
    }
}