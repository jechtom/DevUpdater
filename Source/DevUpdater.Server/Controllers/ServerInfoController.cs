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
    public class ServerInfoController : ApiController
    {
        public VersionService VersionService { get; set; }
        public SecurityService SecurityService { get; set; }
        
        public IHttpActionResult Get()
        {
            var result = new Models.ServerInfoModel();
            result.Version = VersionService.GetCurrentVersion().ToString();
            
            if(User.Identity != null && User.Identity.IsAuthenticated) // authenticated?
            {
                var identity = (ClaimsIdentity)User.Identity;
                result.IsAuthenticated = SecurityService.GetIdentityIsAuthenticated(identity);
                result.Groups = SecurityService.GetIdentityGroupNames(identity);
            }

            return Ok(result);
        }
    }
}
