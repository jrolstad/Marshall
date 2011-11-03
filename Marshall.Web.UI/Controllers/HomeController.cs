using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Marshall.Web.UI.Models;
using Marshall.Web.UI.Models.Environment;
using Marshall.Web.UI.Models.Version;
using Rolstad.Extensions;

namespace Marshall.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EnvironmentDetails(HttpPostedFileBase versionFile,HttpPostedFileBase environmentFile)
        {
            if (versionFile != null && environmentFile != null)
            {
                var versionData = XDocument.Load(versionFile.InputStream, LoadOptions.SetLineInfo);

                var applications = versionData.Descendants("application")
                    .Select(a => 
                        new ApplicationViewModel
                            {
                                Name = GetAttributeValue(a,"name"),
                                Type = GetElementValue(a,"type"),
                                TeamCityProjectName = GetElementValue(a,"teamCity","projectName"),
                                TeamCityBuildName = GetElementValue(a,"teamCity","buildName"),
                                SpecifiedVersion = GetElementValue(a,"teamCity","specifiedVersion"),
                                FileToInstall = GetElementValue(a, "teamCity", "applicationInstallFile"),
                                ConfigFile = GetElementValue(a, "configFile"),
                                NetFrameworkVersion = GetElementValue(a,"netFrameworkVersion"),
                                ProductCode = GetElementValue(a, "productCode"),
                                WorkingDirectoryPrefix = GetElementValue(a, "workingDirPrefix"),
                                SubversionPrefix = GetElementValue(a, "svn", "svnPrefix"),
                                SubversionWorkingDirectoryPrefix = GetElementValue(a, "svn", "workingDirPrefix"),
                                SubversionSpecifiedVersion = GetElementValue(a, "svn", "specifiedVersion"),
                                ServiceEntries = GetServiceEntries(a),
                                PingTargets = GetPingTargets(a)
                            })
                    .OrderBy(a=>a.Name)
                    .ToArray();

                var environmentData = XElement.Load(environmentFile.InputStream, LoadOptions.SetLineInfo);
                var databases = environmentData.Descendants("ppaConfig")
                    .Select(d=>new DatabaseConfigurationViewModel
                                   {
                                       Server = GetElementValue(d,"server"),
                                       Database = GetElementValue(d, "databaseName"),
                                       Name = GetElementValue(d, "configSection"),
                                       Roles = GetDatabaseRoles(d)
                                   })
                    .ToArray();

                var applicationPools = environmentData.Descendants("applicationPools").Descendants("applicationPool")
                  .Select(d => new ApplicationPoolViewModel
                  {
                      Name = this.GetAttributeValue(d, "name"),
                      ServiceAccount = this.GetAttributeValue(d, "serviceAccount"),
                      Password = this.GetAttributeValue(d, "password"),
                  })
                  .ToArray();

                var caConfig = environmentData.Descendants("caConfig")
                    .Select(c =>  new CaConfig
                                      {
                                          Port = GetElementValue(c, "Port"),
                                          Server = GetElementValue(c, "CliServer"),
                                          User = GetElementValue(c, "User"),
                                          Password = GetElementValue(c, "EncryptedPassword")
                                      })
                    .FirstOrDefault();

                var sqlJobs = environmentData.Descendants("sqlJobs").Descendants("sqlJob")
                    .Select(d => new SqlJobViewModel
                    {
                        Database = GetElementValue(d, "configSection"),
                        Name = GetElementValue(d, "name"),
                    })
                    .ToArray();

                return View(new EnvironmentDetailsViewModel
                                {
                                    Applications = applications, 
                                    Databases = databases, 
                                    ApplicationPools = applicationPools,
                                    CaConfig = caConfig,
                                    SqlJobs = sqlJobs
                                });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        private IEnumerable<DatabaseRoleViewModel> GetDatabaseRoles( XElement database )
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

        private IEnumerable<PingTargetViewModel> GetPingTargets( XElement application )
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
                                                    Name = GetAttributeValue(s,"name"),
                                                    Contract = GetAttributeValue(s,"contract"),
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
