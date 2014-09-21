using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Models
{
    public class SettingsModel
    {
        public SettingsModel()
        {
            Groups = new List<GroupModel>();
            Repositories = new List<RepositoryModel>();
            Clients = new List<ClientModel>();
            PendingCertificates = new List<PendingCertificateModel>();
        }

        public List<GroupModel> Groups { get; set; }
        public List<RepositoryModel> Repositories { get; set; }
        public List<ClientModel> Clients { get; set; }
        public List<PendingCertificateModel> PendingCertificates { get; set; }
    }
}
