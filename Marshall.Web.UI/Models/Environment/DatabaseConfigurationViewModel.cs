using System.Collections.Generic;

namespace Marshall.Web.UI.Models.Environment
{
    public class DatabaseConfigurationViewModel
    {
        public string Server { get; set; }

        public string Database { get; set; }

        public string Name { get; set; }

        public IEnumerable<DatabaseRoleViewModel> Roles { get; set; } 
    }
}