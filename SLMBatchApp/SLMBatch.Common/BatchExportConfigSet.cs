using System.Configuration;

namespace SLMBatch.Common
{
    public class BatchExportConfigSet
    {
        public class EmailConfigSet
        {
            private string _subjectComplete;
            private string _subjectError;
            private string _purposeComplete;
            private string _purposeError;

            public string SubjectComplete
            {
                get
                {
                    if (_subjectComplete == null)
                    {
                        throw new ConfigurationErrorsException();
                    }
                    {
                        return _subjectComplete;
                    }
                }
                internal set { _subjectComplete = value; }
            }

            public string SubjectError
            {
                get
                {
                    if (_subjectError == null)
                    {
                        throw new ConfigurationErrorsException();
                    }
                    {
                        return _subjectError;
                    }
                }
                internal set { _subjectError = value; }
            }

            public string PurposeComplete
            {
                get
                {
                    if (_purposeComplete == null)
                    {
                        throw new ConfigurationErrorsException();
                    }
                    {
                        return _purposeComplete;
                    }
                }
                internal set { _purposeComplete = value; }
            }

            public string PurposeError
            {
                get
                {
                    if (_purposeError == null)
                    {
                        throw new ConfigurationErrorsException();
                    }
                    {
                        return _purposeError;
                    }
                }
                internal set { _purposeError = value; }
            }
        }

        public class ExportConfigSet
        {
            private string _path;
            private string _domainName;
            private string _username;
            private string _password;

            public string Path
            {
                get
                {
                    if (_path == null)
                    {
                        throw new ConfigurationErrorsException();
                    }
                    {
                        return _path;
                    }
                }
                internal set { _path = value; }
            }

            public string DomainName
            {
                get
                {
                    if (_domainName == null)
                    {
                        throw new ConfigurationErrorsException();
                    }
                    {
                        return _domainName;
                    }
                }
                internal set { _domainName = value; }
            }

            public string Username
            {
                get
                {
                    if (_username == null)
                    {
                        throw new ConfigurationErrorsException();
                    }
                    {
                        return _username;
                    }
                }
                internal set { _username = value; }
            }

            public string Password
            {
                get
                {
                    if (_password == null)
                    {
                        throw new ConfigurationErrorsException();
                    }
                    {
                        return _password;
                    }
                }
                internal set { _password = value; }
            }
        }
    }
}
