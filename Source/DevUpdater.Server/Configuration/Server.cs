using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace DevUpdater.Server.Configuration
{
    [ContentProperty("Repositories")]
    public class Server
    {
        public Server()
        {
            Repositories = new List<Repository>();
            Hosting = new Hosting();
        }

        public Hosting Hosting { get; set; }
        
        public ICollection<Repository> Repositories { get; set; }
    }
}
