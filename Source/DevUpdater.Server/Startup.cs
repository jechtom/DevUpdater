using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace DevUpdater.Server
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // authorization
            var authorizedClients = new AuthorizedClientsList(Path.Combine(FileHelper.CurrentDir, "server-authorized-clients.txt"));
            app.Use<AuthenticationClientCertListMiddleware>(new AuthenticationClientCertListOptions(authorizedClients));

#if DEBUG
            app.UseErrorPage();
#endif
            var config = new System.Web.Http.HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "RepositoryFile",
                routeTemplate: "repositories/{repository}/file/{id}",
                defaults: new { controller = "RepositoryFile", id = RouteParameter.Optional }
            );
            
            config.Routes.MapHttpRoute(
                name: "Repository",
                routeTemplate: "repositories/{id}",
                defaults: new { controller = "Repository", id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
        }
    }
}
