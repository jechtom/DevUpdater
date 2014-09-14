using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DevUpdater.Server.Controllers
{
    [Authorize]
    public class RepositoryController : ApiController
    {
        public IHttpActionResult Get(string id)
        {
            var app = DevUpdater.Server.ServerApp.Current;
            Repositories.Repository repo;

            if (!app.Repositories.TryGetValue(id, out repo))
                return NotFound();
            
            var value = new Repositories.Remote.FetchRepositoryDto(repo);
            return Ok(value);
        }
    }
}
