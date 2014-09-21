using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Repositories.Remote
{
    public class RemoteRepositoryAccessor : IRepositoryAccessor
    {
        RemoteRepositoryServerContext context;

        public RemoteRepositoryAccessor(RemoteRepositoryServerContext context, string repositoryName)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.context = context;
            this.RepositoryName = repositoryName;
        }

        public string RepositoryName { get; private set; }

        public async Task<Repository> FetchRepository()
        {
            // fetch data
            HttpClient client = context.CreateWebClient();
            var httpResult = await client.GetAsync(BaseUrlWithRepo);

            httpResult.EnsureSuccessStatusCode();
            var dto = await httpResult.Content.ReadAsAsync<FetchRepositoryDto>();
            
            // build result
            var result = new Repository(RepositoryName, this)
            {
                Files = dto.Files.ToList(),
                Settings = dto.Settings
            };

            return result;
        }

        public async Task<Stream> ReadFileAsStream(FileInfo file)
        {
            HttpClient client = context.CreateWebClient();
            var httpResult = await client.GetAsync(new Uri(BaseUrlWithRepo, "file/" + file.Hash.ToString()));
            httpResult.EnsureSuccessStatusCode();
            var sourceStream = await httpResult.Content.ReadAsStreamAsync();
            var gzip = new GZipStream(sourceStream, CompressionMode.Decompress, leaveOpen: false);
            return gzip;
        }

        private Uri BaseUrlWithRepo
        {
            get
            {
                return new Uri(context.BaseUrl, "repositories/" + RepositoryName + "/");
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }


        public Task WriteFromStream(Stream sourceStream, FileInfo[] targets)
        {
            throw new NotSupportedException();
        }
    }
}
