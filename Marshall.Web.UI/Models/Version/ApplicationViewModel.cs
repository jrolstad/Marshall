using System.Collections.Generic;
using Marshall.Web.UI.Models.Environment;

namespace Marshall.Web.UI.Models.Version
{
    public class ApplicationViewModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string ProductCode { get; set; }
        public string TeamCityProjectName { get; set; }
        public string TeamCityBuildName { get; set; }
        public string SpecifiedVersion { get; set; }
        public string FileToInstall { get; set; }
        public string ConfigFile { get; set; }
        public string NetFrameworkVersion { get; set; }
        public string WorkingDirectoryPrefix { get; set; }

        public string SubversionPrefix { get; set; }
        public string SubversionWorkingDirectoryPrefix { get; set; }
        public string SubversionSpecifiedVersion { get; set; }

        public IEnumerable<ApplicationServiceViewModel> ServiceEntries { get; set; }

        public IEnumerable<PingTargetViewModel> PingTargets { get; set; }

        public ApplicationEnvironmentViewModel Environment { get; set; }
    }
}