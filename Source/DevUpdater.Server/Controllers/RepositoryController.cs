using DevUpdater.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DevUpdater.Server.Controllers
{
    [Authorize]
    public class RepositoryController : RepositoryControllerBase
    {
        public IHttpActionResult Get(string repository)
        {
            var error = ResolveAndAuthenticateToRepository(repository);
            if (error != null)
                return error;

            // return
            var value = new Repositories.Remote.FetchRepositoryDto(RepositoryInstance.Value);
            return Ok(value);
        }
    }
}
