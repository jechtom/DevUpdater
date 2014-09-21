using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Repositories
{
    public interface IRepositoryAccessor
    {
        Task<Repository> FetchRepository();

        Task<Stream> ReadFileAsStream(FileInfo file);

        bool IsReadOnly { get; }

        Task WriteFromStream(Stream sourceStream, FileInfo[] targets);
    }
}
