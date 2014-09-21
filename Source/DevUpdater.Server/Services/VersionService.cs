using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Services
{
    public class VersionService
    {
        Version version;

        public VersionService()
        {
            this.version = Assembly.GetExecutingAssembly().GetName().Version;
        }

        public Version GetCurrentVersion()
        {
            return version;
        }

        public string GetNameWithVersion()
        {
            return string.Format("{0} v{1}", "DevUpdater Server", GetCurrentVersion());
        }
    }
}
