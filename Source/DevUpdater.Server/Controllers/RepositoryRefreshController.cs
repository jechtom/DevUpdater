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
    public class RepositoryRefreshController :  RepositoryControllerBase
    {
        public async Task<IHttpActionResult> Post(string repository)
        {
            var error = ResolveAndAuthenticateToRepository(repository);
            if (error != null)
                return error;

            // refresh
            await RepoService.SynchronizeRepository(RepositoryInstance);
            return Ok();
        }
    }
}
