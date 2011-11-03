using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Marshall.Web.UI.Models.Version;
using Rolstad.Extensions;

namespace Marshall.Web.UI.Readers
{
    public class VersionFileReader
    {
        public IEnumerable<ApplicationViewModel> Read(XDocument versionFile)
        {
            var applications = versionFile.Descendants("application")
                    .Select(a =>
                        new ApplicationViewModel
                        {
                            Name = GetAttributeValue(a, "name"),
                            Type = GetElementValue(a, "type"),
                            TeamCityProjectName = GetElementValue(a, "teamCity", "projectName"),
                            TeamCityBuildName = GetElementValue(a, "teamCity", "buildName"),
                            SpecifiedVersion = GetElementValue(a, "teamCity", "specifiedVersion"),
                            FileToInstall = GetElementValue(a, "teamCity", "applicationInstallFile"),
                            ConfigFile = GetElementValue(a, "configFile"),
                            NetFrameworkVersion = GetElementValue(a, "netFrameworkVersion"),
                            ProductCode = GetElementValue(a, "productCode"),
                            WorkingDirectoryPrefix = GetElementValue(a, "workingDirPrefix"),
                            SubversionPrefix = GetElementValue(a, "svn", "svnPrefix"),
                            SubversionWorkingDirectoryPrefix = GetElementValue(a, "svn", "workingDirPrefix"),
                            SubversionSpecifiedVersion = GetElementValue(a, "svn", "specifiedVersion"),
                            ServiceEntries = GetServiceEntries(a),
                            PingTargets = GetPingTargets(a)
                        })
                    .OrderBy(a => a.Name)
                    .ToArray();

            return applications;
        }

        private IEnumerable<PingTargetViewModel> GetPingTargets(XElement application)
        {
            var pingTargetEntries = new List<PingTargetViewModel>();

            var pingTargets = application.Element("services");
            if (pingTargets == null) return pingTargetEntries;


            pingTargets.Descendants("pingTarget")
                .Each(s =>
                {
                    var target = new PingTargetViewModel
                    {
                        Name = GetAttributeValue(s, "name"),
                        ServiceFile = GetAttributeValue(s, "serviceFile"),
                        urlSuffix = GetAttributeValue(s, "urlSuffix")
                    };
                    pingTargetEntries.Add(target);
                });

            return pingTargetEntries;
        }

        private IEnumerable<ApplicationServiceViewModel> GetServiceEntries(XElement application)
        {
            var serviceEntries = new List<ApplicationServiceViewModel>();

            var services = application.Element("services");
            if (services == null) return serviceEntries;


            services.Descendants("service")
                .Each(s =>
                {
                    var service = new ApplicationServiceViewModel
                    {
                        Name = GetAttributeValue(s, "name"),
                        Contract = GetAttributeValue(s, "contract"),
                        ServiceFile = GetAttributeValue(s, "serviceFile"),
                        Binding = GetAttributeValue(s, "binding")
                    };
                    serviceEntries.Add(service);
                });

            return serviceEntries;
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