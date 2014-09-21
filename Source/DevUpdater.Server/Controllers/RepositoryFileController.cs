using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace DevUpdater.Server.Controllers
{
    [Authorize]
    public class RepositoryFileController :  RepositoryControllerBase
    {
        public async Task<IHttpActionResult> Get(string repository, [ModelBinder(typeof(HashModelBinder))] Hash hash)
        {
            var error = ResolveAndAuthenticateToRepository(repository);
            if (error != null)
                return error;

            // get file
            var fileInfo = RepositoryInstance.Value.Files.FirstOrDefault(f => f.Hash.Equals(hash));

            if(fileInfo == null)
                return NotFound();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = await RepositoryInstance.Value.Accessor.ReadFileAsStream(fileInfo);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return ResponseMessage(result);
        }
    }
}
