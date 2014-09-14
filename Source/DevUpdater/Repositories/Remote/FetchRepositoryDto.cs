using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Repositories.Remote
{
    public class FetchRepositoryDto
    {
        public FetchRepositoryDto()
        {

        }

        public FetchRepositoryDto(Repository r)
        {
            this.Files = r.Files;
            this.Settings = r.Settings;
        }

        public ICollection<FileInfo> Files { get; set; }
        public RepositorySettings Settings { get; set; }
    }
}
