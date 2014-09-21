using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Data
{
    public class Repository
    {
        public long Id { get; set; }
        public string UrlName { get; set; }
        public string Command { get; set; }
        public string CommandArgs { get; set; }
        public string SourceFolder { get; set; }
        public virtual ICollection<ClientGroup> ClientGroups { get; set; }
    }
}
