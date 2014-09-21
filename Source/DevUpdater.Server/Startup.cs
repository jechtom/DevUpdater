using Castle.MicroKernel.Lifestyle.Scoped;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Facilities.TypedFactory;
using Owin;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Diagnostics;

namespace DevUpdater.Server
{
    public class Startup
    {
        public static TraceSource DefaultTraceSource { get; set; }

        IWindsorContainer container;

        public void Configuration(IAppBuilder app)
        {
            // ioc container
            ResolveContainer();

            // init database
            InitDatabase();

            // synchronize repositories
            container.Resolve<Services.RepositoryService>().Init();

            // authorization
            app.Use<AuthenticationClientCertListMiddleware>(container.Resolve<AuthenticationClientCertListOptions>());

#if DEBUG
            app.UseErrorPage();
#endif

            // setup config
            var config = new System.Web.Http.HttpConfiguration();

            // enable Castle Windsor
            config.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerActivator), new WindsorCompositionRoot(this.container));

            // routing
            SetUpRouting(config);

            // static files (UI)
            var staticFileOptions = new Microsoft.Owin.StaticFiles.StaticFileOptions() { 
                FileSystem = new EmbeddedResourceFileSystem(this.GetType().Assembly, "DevUpdater.Server.StaticContent")
            };
            app.UseDefaultFiles(new DefaultFilesOptions() { FileSystem = staticFileOptions.FileSystem });
            app.UseStaticFiles(staticFileOptions);

            // web api stuff
            app.UseWebApi(config);
        }

        private void InitDatabase()
        {
            // create / update database
            var context = container.Resolve<Data.ServerDataContext>();
        }

        private static void SetUpRouting(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "RepositoryFile",
                routeTemplate: "repository/{repository}/file/{hash}",
                defaults: new { controller = "RepositoryFile" }
            );

            config.Routes.MapHttpRoute(
                name: "RepositoryRefresh",
                routeTemplate: "repository/{repository}/refresh",
                defaults: new { controller = "RepositoryRefresh" }
            );

            config.Routes.MapHttpRoute(
                name: "Repository",
                routeTemplate: "repository/{repository}",
                defaults: new { controller = "Repository" }
            );

            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private void ResolveContainer()
        {
            container = new WindsorContainer();
            container.Kernel.AddFacility<TypedFactoryFacility>();
            container.Register(
                // global components
                Component.For<Data.ServerDataContext>().LifestyleTransient(),
                Component.For<AuthenticationClientCertListOptions>().LifestyleSingleton(),
                Component.For<TraceSource>().Instance(DefaultTraceSource ?? new TraceSource("server")),
                Component.For<Services.VersionService>().LifestyleSingleton(),

                // register services
                Component.For<Services.SettingsService>().LifestyleTransient(),
                Component.For<Services.PersistenceService>().LifestyleTransient(),
                Component.For<Services.SecurityService>().LifestyleSingleton(), // singleton - cached security objects
                Component.For<Services.RepositoryService>().LifestyleSingleton(), // singleton - stores repository data
                Component.For<DevUpdater.Repositories.RepositoryProcessor>().LifestyleTransient(), // singleton - stores repository data
                
                // ALL controllers
                Types.FromThisAssembly().InSameNamespaceAs<Controllers.RepositoryController>().LifestyleTransient()
            );
        }
    }
}
