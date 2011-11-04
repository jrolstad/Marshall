using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Marshall.Web.UI.Models;
using Marshall.Web.UI.Models.Environment;
using Marshall.Web.UI.Models.Version;
using Marshall.Web.UI.Readers;
using Rolstad.Extensions;

namespace Marshall.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        readonly VersionFileReader _versionFileReader = new VersionFileReader();
        readonly EnvironmentFileReader _environmentFileReader = new EnvironmentFileReader();

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
                var applications = _versionFileReader.Read(versionData);

                var environmentData = XDocument.Load(environmentFile.InputStream, LoadOptions.SetLineInfo);
                var databases = _environmentFileReader.ReadDatabaseConfigurations(environmentData);
                var applicationPools = _environmentFileReader.ReadApplicationPools(environmentData);
                var caConfig = _environmentFileReader.ReadCaConfig(environmentData);
                var sqlJobs = _environmentFileReader.ReadSqlJobs(environmentData);
                var applicationEnvironments = _environmentFileReader.ReadApplicationEnvironments(environmentData);

                CombineApplicationsAndEnvironments(applications, applicationEnvironments);

                return View(new EnvironmentDetailsViewModel
                                {
                                    Applications = applications, 
                                    ApplicationEnvironments = applicationEnvironments,
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

        private void CombineApplicationsAndEnvironments( IEnumerable<ApplicationViewModel> applications, IEnumerable<ApplicationEnvironmentViewModel> applicationEnvironments )
        {
            var environmentDictionary = applicationEnvironments.ToDictionaryExplicit(e => e.Name);

            applications.Each(a =>
                                  {
                                      if(environmentDictionary.ContainsKey(a.Name))
                                      {
                                          a.Environment = environmentDictionary[a.Name];
                                      }
                                  });
        }
    }
}
