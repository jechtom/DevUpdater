using DevUpdater.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Client.Configuration
{
    public class Client
    {
        public Uri ServerUrl { get; set; }

        public Hash ServerCertificateHash { get; set; }

        public static Client Configuration { get; set; }

        public string Repository { get; set; }

        public bool WaitForExitAndStartAgain { get; set; }
    }
}
