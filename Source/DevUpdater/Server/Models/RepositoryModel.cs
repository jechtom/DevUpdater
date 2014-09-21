using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevUpdater.Server.Models
{
    public class RepositoryModel
    {
        public RepositoryModel()
        {
            Groups = new List<long>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public List<long> Groups { get; set; }
        public string SourceFolder { get; set; }
    }
}
