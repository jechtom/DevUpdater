using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Data
{
    public class Client
    {
        public long Id { get; set; }
        public byte[] CertificateHash { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ClientGroup> ClientGroups { get; set; }
    }
}
