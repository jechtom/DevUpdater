using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Services
{
    public class RepositoryServerInstance
    {
        public Repositories.Repository Value { get; set; }
        public Data.Repository Dto { get; set; }
        public Dictionary<long, Data.ClientGroup> GroupsAllowed { get; set; }
    }
}
