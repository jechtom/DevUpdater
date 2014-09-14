using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevUpdater.Repositories
{
    public class FileInfoCopyCommand
    {
        public FileInfo Source { get; set; }
        public IEnumerable<FileInfo> Targets { get; set; }
    }
}
