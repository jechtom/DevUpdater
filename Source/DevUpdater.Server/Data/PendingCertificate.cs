using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Data
{
    public class PendingCertificate
    {
        public long Id { get; set; }
        public byte[] CertificateHash { get; set; }
        public string IpAddress { get; set; }
        public DateTime LastAttemptUtc { get; set; }
    }
}
