using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SLS.Resource
{
    public class ServiceConstant
    {
        public static int ConnectionTimeout
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["Timeout"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"]) : 30;
                }
                catch
                {
                    return 30;  //default (second)
                }
            }
        }

        public static bool DoValidateBranch
        {
            get
            {
                try
                {
                    if (ConfigurationManager.AppSettings["ValidateBranch"] != null)
                    {
                        return ConfigurationManager.AppSettings["ValidateBranch"].ToUpper().Trim() == "Y" ? true : false;
                    }
                    else
                        return true;
                }
                catch
                {
                    return true;
                }
            }
        }

        public static string SLMDBName
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["SLMDBName"] != null ? ConfigurationManager.AppSettings["SLMDBName"] : "SLMDB";
                }
                catch
                {
                    return "SLMDB";
                }
            }
        }

        public class System
        {
            public const string HPGABLE = "HPGABLE";
            public const string HPAOFL = "HPAOFL";
        }

        public static class CardType
        {
            public const string Person = "1";
            public const string JuristicPerson = "2";
            public const string Foreigner = "3";
        }

        public static class StaffType
        {
            public const decimal Manager = 1;
            public const decimal Supervisor = 2;
            public const decimal UserAdministrator = 3;
            public const decimal Telesales = 4;
            public const decimal CallCenter = 5;
            public const decimal Leader = 6;
            public const decimal ITAdministrator = 7;
            public const decimal Marketing = 8;
            public const decimal ManagerOper = 10;
            public const decimal SupervisorOper = 11;
            public const decimal Oper = 12;
            public const decimal SupervisorTelesalesOutbound = 15;
            public const decimal OperOutbound = 16;
            public const decimal ProductOutbound = 17;
            public const decimal ManagerOutbound = 18;
        }

        public static class ScreenType
        {
            public const string Insert = "I";
            public const string Edit = "E";
            public const string View = "V";
        }
    }
}
