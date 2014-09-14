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

        public Task<byte[]> ReadFileAsBytes(FileInfo file)
        {
            // remark: can be rewritten as async
            var result = File.ReadAllBytes(file.ResolveFullPath(Path));
            return Task.FromResult<byte[]>(result);
        }

        private List<FileInfo> ResolveFiles()
        {
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
        
        public bool IsFileSystemRepository
        {
            get { return true; }
        }

        public Task WriteFromFile(string sourceFilePath, FileInfo targetFile)
        {
            // remark: can be rewriten as async
            targetFile.EnsureParentDirectoryExists(Path);
            File.Copy(sourceFilePath, targetFile.ResolveFullPath(Path), overwrite: true);
            return Task.FromResult<object>(null);
        }

        public Task WriteFromBytes(byte[] content, FileInfo targetFile)
        {
            // remark: can be rewriten as async
            targetFile.EnsureParentDirectoryExists(Path);
            File.WriteAllBytes(targetFile.ResolveFullPath(Path), content);
            return Task.FromResult<object>(null);
        }
    }
}
