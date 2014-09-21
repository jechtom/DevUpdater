using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevUpdater.Server.Data
{
    public class ClientGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Repository> Repositories { get; set; }
    }
}
