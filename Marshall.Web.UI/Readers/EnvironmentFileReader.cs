using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                        SqlBackupJob = this.GetElementValue(d, "sqlBackupJob")
                    })
                    .Where(j => j.Name != null)
                    .ToArray();

            return applicationEnvironments;
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