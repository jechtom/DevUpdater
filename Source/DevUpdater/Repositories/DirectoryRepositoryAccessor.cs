using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Repositories
{
    public class DirectoryRepositoryAccessor : IRepositoryAccessor
    {
        private Func<HashAlgorithm> hashAlgFactory;
        private string id;

        public DirectoryRepositoryAccessor(string path, string id)
        {
            hashAlgFactory = () => SHA1Managed.Create();

            this.id = id;

            this.Path = path;

            if (!Path.EndsWith(@"\")) // make sure path ends with "\"
                Path += @"\";
        }

        public string Path { get; private set; }

        public async Task<Repository> FetchRepository()
        {
            var files = await Task.Run<List<FileInfo>>(() => ResolveFiles());
            return new Repository(id, this)
            {
                Files = files,
                FileSystemPath = Path
            };
        }

        public Task<Stream> ReadFileAsStream(FileInfo file)
        {
            // remark: can be rewritten as async
            var result = new FileStream(file.ResolveFullPath(Path), FileMode.Open, FileAccess.Read);
            return Task.FromResult<Stream>(result);
        }

        private List<FileInfo> ResolveFiles()
        {
            if (!Directory.Exists(Path)) // not exists yet
                return new List<FileInfo>();

            using (HashAlgorithm hashAlg = hashAlgFactory())
            {
                var result = Directory.GetFiles(Path, "*.*", SearchOption.AllDirectories).Select(f => ResolveFile(hashAlg, f));
                return result.ToList();
            }
        }

        private FileInfo ResolveFile(HashAlgorithm hashAlg, string file)
        {
            using (var stream = File.OpenRead(file))
            {
                long size;
                var hash = ComputeFileHash(hashAlg, file, out size);
                string fileName = file.Substring(Path.Length); // keep only path inside root directory

                return new FileInfo()
                {
                    FileName = fileName,
                    Hash = hash,
                    Size = size
                };
            }
        }

        private Hash ComputeFileHash(HashAlgorithm hashAlg, string filePath, out long size)
        {
            using (var stream = File.OpenRead(filePath))
            {
                size = stream.Length;
                var hash = hashAlg.ComputeHash(stream);
                return new Hash(hash);
            }
        }
        
        public bool IsReadOnly
        {
            get { return false; }
        }

        public async Task WriteFromStream(Stream sourceStream, FileInfo[] targets)
        {
            // create copy if multiple targets
            if(targets.Length > 1)
            {
                var copy = new MemoryStream();
                await sourceStream.CopyToAsync(copy);
                sourceStream = copy;
            }

            foreach (var target in targets)
            {
                target.EnsureParentDirectoryExists(Path);
                string targetPath = target.ResolveFullPath(Path);

                using (var fileStream = new FileStream(targetPath, FileMode.Create))
                {
                    await sourceStream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
        }
    }
}
