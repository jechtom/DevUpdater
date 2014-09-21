using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Models
{
    public class ServerInfoModel
    {
        public string[] Groups { get; set; }
        public string Version { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
