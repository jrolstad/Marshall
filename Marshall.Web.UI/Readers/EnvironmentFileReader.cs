using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Marshall.Web.UI.Models.Environment;
using Marshall.Web.UI.Models.Version;
using Rolstad.Extensions;

namespace Marshall.Web.UI.Readers
{
    public class EnvironmentFileReader
    {
        public IEnumerable<DatabaseConfigurationViewModel> ReadDatabaseConfigurations(XDocument environmentData)
        {
            var databases = environmentData.Descendants("ppaConfig")
                   .Select(d => new DatabaseConfigurationViewModel
                   {
                       Server = GetElementValue(d, "server"),
                       Database = GetElementValue(d, "databaseName"),
                       Name = GetElementValue(d, "configSection"),
                       Roles = GetDatabaseRoles(d)
                   })
                   .ToArray();

            return databases;
        }

        public IEnumerable<ApplicationPoolViewModel> ReadApplicationPools(XDocument environmentData)
        {
            var applicationPools = environmentData.Descendants("applicationPools").Descendants("applicationPool")
                 .Select(d => new ApplicationPoolViewModel
                 {
                     Name = this.GetAttributeValue(d, "name"),
                     ServiceAccount = this.GetAttributeValue(d, "serviceAccount"),
                     Password = this.GetAttributeValue(d, "password"),
                 })
                 .ToArray();

            return applicationPools;
        }

        public CaConfig ReadCaConfig(XDocument environmentData)
        {
            var caConfig = environmentData.Descendants("caConfig")
                    .Select(c => new CaConfig
                    {
                        Port = GetElementValue(c, "Port"),
                        Server = GetElementValue(c, "CliServer"),
                        User = GetElementValue(c, "User"),
                        Password = GetElementValue(c, "EncryptedPassword")
                    })
                    .FirstOrDefault();

            return caConfig;
        }

        public IEnumerable<SqlJobViewModel> ReadSqlJobs(XDocument environmentData)
        {
            var sqlJobs = environmentData.Descendants("sqlJobs").Descendants("sqlJob")
                    .Select(d => new SqlJobViewModel
                    {
                        Database = GetElementValue(d, "configSection"),
                        Name = GetElementValue(d, "name"),
                    })
                    .Where(j => j.Name != null)
                    .ToArray();

            return sqlJobs;
        }

        public IEnumerable<ApplicationEnvironmentViewModel> ReadApplicationEnvironments( XDocument environmentData)
        {
            var applicationEnvironments = environmentData.Descendants("application")
                    .Select(d => new ApplicationEnvironmentViewModel
                    {
                        ApplicationId = this.GetAttributeValue(d, "appID"),
                        Name = this.GetAttributeValue(d, "name"),
                        Type = this.GetAttributeValue(d,"type"),
                        ConfigSection = this.GetElementValue(d, "configSection"),
                        SqlBackupJob = this.GetElementValue(d, "sqlBackupJob"),
                        TargetMachines = this.GetTargetMachines(d),
                        CaEvents = this.GetCaEvents(d),
                        NantProperties = this.GetNantProperties(d),
                        NantTargets = this.GetNantTargets(d),
                        Queues = this.GetQueues(d)
                    })
                    .Where(j => j.Name != null)
                    .OrderBy(j=>j.Name)
                    .ToArray();

            return applicationEnvironments;
        }

        private IEnumerable<TargetMachineViewModel> GetTargetMachines( XElement xElement)
        {
            return xElement.Descendants("targetMachines").Descendants("targetMachine")
                .Select(m => new TargetMachineViewModel
                                 {
                                     MachineName = this.GetElementValue(m, "machineName"),
                                     VirtualDirectory = this.GetElementValue(m, "virtualDirectory"),
                                     Designation = this.GetElementValue(m, "designation"),
                                     DnsAlias = this.GetElementValue(m, "dnsAlias"),
                                     ServiceName = this.GetElementValue(m, "serviceName"),
                                     ServiceAccount = this.GetElementValue(m, "serviceAccount"),
                                     Password = this.GetElementValue(m, "password"),
                                     PhysicalDirectory = this.GetElementValue(m, "physicalDirectory"),
                                     DirectoryStub = this.GetElementValue(m, "DirectoryStub"),
                                     ApplicationExe = this.GetElementValue(m, "ApplicationExe"),
                                     ExtenalDirectories = this.GetExternalDirectories(m),
                                     PingTargets = this.GetPingTargets(m),
                                     Website = this.GetElementValue(m, "website"),
                                     ApplicationPool = this.GetElementValue(m, "applicationPool"),
                                     AuthenticationType = this.GetElementValue(m, "authenticationType"),
                                     MagnetoType = this.GetElementValue(m, "magnetoType"),
                                     AuthenticationMode = this.GetElementValue(m, "authenticationMode"),
                                     AnonymousUserAccount = this.GetElementValue(m, "anonymousUser", "account"),
                                     AnonymousUserPassword = this.GetElementValue(m, "anonymousUser", "password"),
                                     ConfigFileAction = this.GetElementValue(m, "configFileAction"),
                                     ConfigFileName = this.GetElementValue(m, "configFileName"),
                                     ServiceStartupType = this.GetElementValue(m, "serviceStartupType"),
                                     InstallForEveryone = this.GetElementValue(m, "installForEveryone")
                                 })
                .ToArray();
        }

        private IEnumerable<TargetMachinePingTarget> GetPingTargets( XElement xElement   )
        {
            return xElement.Descendants("pingTargets").Descendants("pingTarget")
                .Select(x => new TargetMachinePingTarget
                                 {
                                     Service = this.GetAttributeValue(x,"service"),
                                     Target = this.GetAttributeValue(x,"target")
                                 })
                .ToArray();
        }

        private IEnumerable<string> GetExternalDirectories(XElement xElement)
        {
            return xElement.Descendants("externalDirectories").Descendants("externalDirectory")
                .Select(e => e.Value)
                .ToArray();
        }

        private IEnumerable<string> GetCaEvents( XElement xElement)
        {
            return xElement.Descendants("caEvents").Descendants("caEvent")
                .Select(t => t.Value)
                .ToArray();
        }

        private Dictionary<string, string> GetNantProperties( XElement xElement)
        {
            return xElement.Descendants("nant").Descendants("Properties").Descendants("Property")
                .Select(t => new { Name = this.GetAttributeValue(t, "name"), Value = this.GetAttributeValue(t, "value") })
                .ToDictionaryExplicit(k=>k.Name,v=>v.Value);
        }

        private IEnumerable<string> GetNantTargets( XElement xElement )
        {
            return xElement.Descendants("nant").Descendants("Targets").Descendants("Target")
                 .Select(t=>t.Value)
                 .ToArray();
        }

        private IEnumerable<MessageQueueViewModel> GetQueues( XElement xElement)
        {
            return xElement.Descendants("queue")
                .Select(q => new MessageQueueViewModel
                                 {
                                     Id = this.GetAttributeValue(q,"id"),
                                     Description = this.GetAttributeValue(q, "description"),
                                     Name = this.GetAttributeValue(q, "name"),
                                     Path = this.GetAttributeValue(q, "path"),
                                     Type = this.GetAttributeValue(q, "type")

                                 })
                .ToArray();
        }

        private IEnumerable<DatabaseRoleViewModel> GetDatabaseRoles(XElement database)
        {
            var roleEntries = new List<DatabaseRoleViewModel>();

            var roles = database.Descendants("role").ToArray();
            if (roles.Count() == 0) return roleEntries;


            roles.Each(s =>
            {
                var role = new DatabaseRoleViewModel
                {
                    Name = GetElementValue(s, "name"),
                    ApplicationName = GetElementValue(s, "application"),
                    Password = GetElementValue(s, "password")
                };
                roleEntries.Add(role);
            });

            return roleEntries;
        }

        private string GetElementValue(XElement element, string name1, string name2)
        {
            var element1 = element.Element(name1);
            if (element1 == null) return null;

            var element2 = element1.Element(name2);
            if (element2 == null) return null;

            return element2.Value;
        }

        private string GetElementValue(XElement element, string name1)
        {
            var element1 = element.Element(name1);
            if (element1 == null) return null;

            return element1.Value;
        }

        private string GetAttributeValue(XElement element, string name)
        {
            var attribute = element.Attribute(name);

            return attribute != null ? attribute.Value : null;
        }
    }
}