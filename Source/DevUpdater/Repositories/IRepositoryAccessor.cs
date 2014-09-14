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

        Task<byte[]> ReadFileAsBytes(FileInfo file);

        bool IsFileSystemRepository { get; }

        Task WriteFromFile(string sourceFilePath, FileInfo targetFile);

        Task WriteFromBytes(byte[] content, FileInfo targetFile);
    }
}
