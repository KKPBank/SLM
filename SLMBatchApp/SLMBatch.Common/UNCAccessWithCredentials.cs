using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BOOL = System.Boolean;
using DWORD = System.UInt32;
using LPWSTR = System.String;
using NET_API_STATUS = System.UInt32;

namespace SLMBatch.Common
{
    public class UNCAccessWithCredentials : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct USE_INFO_2
        {
            internal LPWSTR ui2_local;
            internal LPWSTR ui2_remote;
            internal LPWSTR ui2_password;
            internal DWORD ui2_status;
            internal DWORD ui2_asg_type;
            internal DWORD ui2_refcount;
            internal DWORD ui2_usecount;
            internal LPWSTR ui2_username;
            internal LPWSTR ui2_domainname;
        }

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseAdd(
            LPWSTR UncServerName,
            DWORD Level,
            ref USE_INFO_2 Buf,
            out DWORD ParmError);

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseDel(
            LPWSTR UncServerName,
            LPWSTR UseName,
            DWORD ForceCond);

        private bool disposed = false;

        private string sUNCPath;
        private string sUser;
        private string sPassword;
        private string sDomain;
        private int iLastError;

        /// <summary>
        /// A disposeable class that allows access to a UNC resource with credentials.
        /// </summary>
        public UNCAccessWithCredentials()
        {
        }

        /// <summary>
        /// The last system error code returned from NetUseAdd or NetUseDel.  Success = 0
        /// </summary>
        public int LastError
        {
            get { return iLastError; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                NetUseDelete();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Connects to a UNC path using the credentials supplied.
        /// </summary>
        /// <param name="UNCPath">Fully qualified domain name UNC path</param>
        /// <param name="User">A user with sufficient rights to access the path.</param>
        /// <param name="Domain">Domain of User.</param>
        /// <param name="Password">Password of User</param>
        /// <returns>True if mapping succeeds.  Use LastError to get the system error code.</returns>
        public bool NetUseWithCredentials(string UNCPath, string User, string Domain, string Password)
        {
            //old
            //if (UNCPath.EndsWith("\\"))
            //{
            //    UNCPath = UNCPath.Substring(0, UNCPath.Length - 1);
            //}
            //sUNCPath = UNCPath;
            //sUser = User;
            //sPassword = Password;
            //sDomain = Domain;
            //return NetUseWithCredentials();
            //new 
            return NetUse(UNCPath, User, Domain, Password);
        }

        private bool NetUseWithCredentials()
        {
            uint returncode;
            try
            {
                USE_INFO_2 useinfo = new USE_INFO_2();

                useinfo.ui2_remote = sUNCPath;
                useinfo.ui2_username = sUser;
                useinfo.ui2_domainname = sDomain;
                useinfo.ui2_password = sPassword;
                useinfo.ui2_asg_type = 0;
                useinfo.ui2_usecount = 1;
                uint paramErrorIndex;
                returncode = NetUseAdd(null, 2, ref useinfo, out paramErrorIndex);
                iLastError = (int)returncode;
                //1219 is connected.
                return (returncode == 0 || returncode == 1219);
            }
            catch
            {
                iLastError = Marshal.GetLastWin32Error();
                return false;
            }
        }

        /// <summary>
        /// Ends the connection to the remote resource 
        /// </summary>
        /// <returns>True if it succeeds.  Use LastError to get the system error code</returns>
        public bool NetUseDelete()
        {
            uint returncode;
            try
            {
                returncode = NetUseDel(null, sUNCPath, 2);
                iLastError = (int)returncode;
                return (returncode == 0);
            }
            catch
            {
                iLastError = Marshal.GetLastWin32Error();
                return false;
            }
        }

        public bool NetUse(string UNCPath, string User, string Domain, string Password)
        {
            NetDelete();

            bool bResult = true;

            if (UNCPath.EndsWith("\\"))
            {
                UNCPath = UNCPath.Substring(0, UNCPath.Length - 1);
            }
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = "net.exe";
            if (string.IsNullOrEmpty(Domain) && string.IsNullOrEmpty(User) && string.IsNullOrEmpty(Password))
            {
                info.Arguments = $" use \"{UNCPath}\"";
            }
            else if (string.IsNullOrEmpty(Domain))
            {
                info.Arguments = $" use \"{UNCPath}\" /USER:{User} {Password}";
            }
            else
            {
                info.Arguments = $" use \"{UNCPath}\" /USER:{Domain}\\{User} {Password}";
            }
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();
            string output = p.StandardOutput.ReadToEnd() + " " + p.StandardError.ReadToEnd();
            p.WaitForExit();

            if (!output.Contains("The command completed successfully") && !output.Contains("Multiple connections to a server"))
            {
                bResult = false;
            }

            return bResult;
        }

        public void NetDelete()
        {
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = "net.exe";
            info.Arguments = "use * /delete /y ";
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();
            string output = p.StandardOutput.ReadToEnd() + " " + p.StandardError.ReadToEnd();
            p.WaitForExit();
        }

        ~UNCAccessWithCredentials()
        {
            Dispose(false);
        }

    }
}
