using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Repositories
{
    public class Repository
    {
        public Repository(string id, IRepositoryAccessor accessor)
        {
            this.Id = id;
            this.Accessor = accessor;
        }

        public RepositorySettings Settings { get; set; }

        public string FileSystemPath { get; set; }

        public List<FileInfo> Files { get; set; }

        public IRepositoryAccessor Accessor { get; set; }

        public string Id { get; set; }
    }
}
