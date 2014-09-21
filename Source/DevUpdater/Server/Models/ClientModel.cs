using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevUpdater.Server.Models
{
    public class ClientModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CertificateHash { get; set; }
    }
}
