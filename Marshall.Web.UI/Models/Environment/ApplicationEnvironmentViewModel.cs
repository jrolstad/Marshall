using System.Collections.Generic;

namespace Marshall.Web.UI.Models.Environment
{
    public class ApplicationEnvironmentViewModel
    {
        public string Name { get; set; }

        public string ApplicationId { get; set; }

        public string Type { get; set; }

        public string ConfigSection { get; set; }

        public IEnumerable<string> CaEvents { get; set; }

        public IEnumerable<string> SqlJobs { get; set; }

        public IEnumerable<string> NantTargets { get; set; }

        public Dictionary<string, string> NantProperties { get; set; }

        public string SqlBackupJob { get; set; }

        public IEnumerable<TargetMachineViewModel> TargetMachines { get; set; }

        public IEnumerable<MessageQueueViewModel> Queues { get; set; }
    }
}