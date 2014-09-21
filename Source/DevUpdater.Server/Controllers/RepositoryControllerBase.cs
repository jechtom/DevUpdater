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
    public abstract class RepositoryControllerBase : ApiController
    {
        public Services.RepositoryService RepoService { get; set; }
        public Services.SecurityService SecurityService { get; set; }
        public Services.RepositoryServerInstance RepositoryInstance { get; set; }

        protected IHttpActionResult ResolveAndAuthenticateToRepository(string repository)
        {
            // get repo
            var repo = RepoService.GetByUrlId(repository);

            if (repo == null)
                return NotFound();

            // authorize
            bool auth = SecurityService.AuthorizeClientToRepository(repo, (ClaimsIdentity)User.Identity);

            if (!auth)
                return ResponseMessage(new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));

            RepositoryInstance = repo;

            return null; // success
        }
    }
}
