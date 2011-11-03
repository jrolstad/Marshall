using System.Collections.Generic;

namespace Marshall.Web.UI.Models.Environment
{
    public class TargetMachineViewModel
    {
        public string MachineName { get; set; }

        public string DnsAlias { get; set; }

        public string ServiceName { get; set; }

        public string ServiceAccount { get; set; }

        public string Password { get; set; }

        public string PhysicalDirectory { get; set; }

        public string DirectoryStub { get; set; }

        public string ApplicationExe { get; set; }

        public IEnumerable<string> ExtenalDirectories { get; set; }

        public string Website { get; set; }

        public string VirtualDirectory { get; set; }

        public string ApplicationPool { get; set; }

        public string AuthenticationType { get; set; }

        public string MagnetoType { get; set; }

        public string AuthenticationMode { get; set; }

        public string AnonymousUserAccount { get; set; }

        public string AnonymousUserPassword { get; set; }

        public IEnumerable<TargetMachinePingTarget> PingTargets { get; set; }

        public string ConfigFileAction { get; set; }

        public string ConfigFileName { get; set; }

        public string ServiceStartupType { get; set; }

        public string InstallForEveryone { get; set; }

        public string Designation { get; set; }


    }
}