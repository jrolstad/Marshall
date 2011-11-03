using System.Collections.Generic;
using Marshall.Web.UI.Models.Environment;
using Marshall.Web.UI.Models.Version;

namespace Marshall.Web.UI.Models
{
    public class EnvironmentDetailsViewModel
    {
        public IEnumerable<ApplicationViewModel> Applications { get; set; }

        public IEnumerable<ApplicationEnvironmentViewModel> ApplicationEnvironments { get; set; }

        public IEnumerable<DatabaseConfigurationViewModel> Databases { get; set; }

        public IEnumerable<ApplicationPoolViewModel> ApplicationPools { get; set; }

        public CaConfig CaConfig { get; set; }

        public IEnumerable<SqlJobViewModel> SqlJobs { get; set; }
    }
}