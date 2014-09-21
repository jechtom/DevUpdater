using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevUpdater.Server.Models
{
    public class PendingCertificateModel
    {
        public long Id { get; set; }
        public string CertificateHash { get; set; }
        public string IpAddress { get; set; }
        public DateTime LastAttemptUtc { get; set; }
    }
}
