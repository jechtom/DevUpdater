using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Repositories
{
    public class DirectoryCompressedRepositoryAccessor : IRepositoryAccessor
    {
        private string id;
        private string pathToFilesList;
        private List<FileInfo> fileInfos;

        public DirectoryCompressedRepositoryAccessor(string path, string id)
        {
            this.id = id;

            this.Path = path;

            if (!Path.EndsWith(@"\")) // make sure path ends with "\"
                Path += @"\";

            // there will be stored files list
            pathToFilesList = System.IO.Path.Combine(Path, "_files.json");
        }

        public string Path { get; private set; }

        public async Task<Repository> FetchRepository()
        {
            await EnsureInited();

            return new Repository(id, this)
            {
                Files = fileInfos,
                FileSystemPath = Path
            };
        }

        public Task<Stream> ReadFileAsStream(FileInfo file)
        {
            // remark: can be rewritten as async
            string path = ResolveFileFullPath(file);
            var result = new FileStream(path, FileMode.Open, FileAccess.Read);
            return Task.FromResult<Stream>(result);
        }

        private async Task EnsureInited()
        {
            if (!Directory.Exists(Path)) // ensure directory exists
                Directory.CreateDirectory(Path);

            if (!File.Exists(pathToFilesList)) // not exists yet
            {
                fileInfos = new List<FileInfo>();
                return;
            }

            // read from file
            var filesContent = System.IO.File.ReadAllText(pathToFilesList);
            var result = await Task.Factory.StartNew<List<FileInfo>>(() => JsonConvert.DeserializeObject<List<FileInfo>>(filesContent));

            fileInfos = result;
        }

        private async Task UpdateFilesList(IEnumerable<FileInfo> filesToAddOrUpdate = null)
        {
            await EnsureInited();

            // update index
            if(filesToAddOrUpdate != null)
            {
                foreach (var itemToAddOrUpdate in filesToAddOrUpdate)
	            {
                    int index = fileInfos.IndexOf(itemToAddOrUpdate);
                    if(index == -1)
                        fileInfos.Add(itemToAddOrUpdate);
                    else
                        fileInfos[index] = itemToAddOrUpdate;
                }
            }
            
            // update files list file - remark: can be optimized to NOT write after each file
            var newContent = await Task.Factory.StartNew<string>(() => JsonConvert.SerializeObject(fileInfos));
            File.WriteAllText(pathToFilesList, newContent);
        }
        
        public bool IsReadOnly
        {
            get { return false; }
        }

        public async Task WriteFromStream(Stream sourceStream, FileInfo[] targets)
        {
            string targetPath = ResolveFileFullPath(targets.First()); // take first - path is based only on hash

            // write to files
            using (var fileStream = new FileStream(targetPath, FileMode.Create))
            using (var gzip = new GZipStream(fileStream, CompressionMode.Compress))
            {
                await sourceStream.CopyToAsync(gzip);
                await fileStream.FlushAsync();
            }

            // update index
            await UpdateFilesList(filesToAddOrUpdate: targets);
        }

        private string ResolveFileFullPath(FileInfo fileInfo)
        {
            return System.IO.Path.Combine(Path, "file_" + fileInfo.Hash.ToString() + ".gzip");
        }
    }
}
