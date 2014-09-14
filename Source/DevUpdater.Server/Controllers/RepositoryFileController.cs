using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DevUpdater.Server.Controllers
{
    [Authorize]
    public class RepositoryFileController :  ApiController
    {
        public async Task<IHttpActionResult> Get(string repository, string id)
        {
            var app = DevUpdater.Server.ServerApp.Current;
            Repositories.Repository repo;

            if (!app.Repositories.TryGetValue(repository, out repo))
                return NotFound();

            var hash = Hash.Parse(id);
            var fileInfo = repo.Files.FirstOrDefault(f => f.Hash.Equals(hash));

            if(fileInfo == null)
                return NotFound();
            
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = await repo.Accessor.ReadFileAsStream(fileInfo);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return ResponseMessage(result);
        }
    }
}
