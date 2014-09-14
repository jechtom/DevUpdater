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
    public class RepositoryFileController : ApiController
    {
        public HttpResponseMessage Get(string repository, string id)
        {
            throw new InvalidOperationException();
            //var item = RepositoryController.Version.GetByHash(id);
            //if (item == null)
            //    return new HttpResponseMessage(HttpStatusCode.NotFound);

            //string path = item.ResolveFullPath(RepositoryController.RootDirectory);
            
            //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //var stream = new FileStream(path, FileMode.Open);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            //return result;
        }

    }
}
