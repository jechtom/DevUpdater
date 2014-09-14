using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevUpdater.Server.Configuration
{
    public class Repository
    {
        public string Id { get; set; }

        public string Command { get; set; }

        public string CommandArgs { get; set; }

        public string SourceFolder { get; set; }
    }
}
