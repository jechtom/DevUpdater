using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace DevUpdater.Server
{
    public class ServerApp
    {
        public static ServerApp Current { get; set; }

        private TraceSource ts;
        private Configuration.Server config;
        private AuthorizedClientsList authorizedClients;

        public Dictionary<string, Repositories.Repository> Repositories { get; set; }

        public ServerApp(TraceSource ts)
        {
            this.ts = ts;
            Current = this;
        }

        public IDisposable Run(string[] args)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            ts.TraceInformation("DEV UPDATER SERVER v" + version);

            ReadSettings(args);

            InitRepositories();

            // startup owin
            StartOptions owinSettings = new StartOptions();
            ts.TraceInformation("Starting web server...");
            owinSettings.Urls.Add(config.Hosting.Url);
            foreach (var url in owinSettings.Urls)
            {
                ts.TraceInformation(" - URL: " + url);
            }
            return WebApp.Start<Startup>(owinSettings);
        }

        public async Task RefreshRepositories()
        {
            await Task.Run(() =>
            {
                InitRepositories();
                ts.TraceInformation("Completed.");
            });
        }

        private void ReadSettings(string[] args)
        {
            string configPath = Path.Combine(FileHelper.CurrentDir, "server-settings.xml");

            // override config path?
            if (args != null && args.Length >= 1)
                configPath = args[0];

            // read config
            config = (Configuration.Server)XamlServices.Load(configPath);
        }

        private void InitRepositories()
        {
            var dict = new Dictionary<string, Repositories.Repository>(StringComparer.OrdinalIgnoreCase);
            ts.TraceInformation("Synchronizing repositories...");

            foreach (var repo in config.Repositories)
            {
                // synchronize data
                var sourceRepoAccessor = new Repositories.DirectoryRepositoryAccessor(repo.SourceFolder, repo.Id);
                var cacheRepoAccessor = new Repositories.DirectoryRepositoryAccessor(Path.Combine(FileHelper.CurrentDir, "appcache-server", repo.Id), repo.Id);

                var sourceRepo = sourceRepoAccessor.FetchRepository().Result;
                var cacheRepo = cacheRepoAccessor.FetchRepository().Result;

                // apply settings to repository
                sourceRepo.Settings = new DevUpdater.Repositories.RepositorySettings()
                {
                    Command = repo.Command,
                    CommandArgs = repo.CommandArgs
                };

                // synchronize
                var processor = new Repositories.RepositoryProcessor(ts);
                processor.Synchronize(sourceRepo, cacheRepo).Wait();
                dict.Add(repo.Id, cacheRepo);
            }

            this.Repositories = dict;
        }

        public void ShowControlForm()
        {
            // not yet
        }
    }
}
