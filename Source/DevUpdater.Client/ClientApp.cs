using DevUpdater.Certificates;
using DevUpdater.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Client
{
    public class ClientApp
    {
        private TraceSource ts;
        private Configuration.Client config;
        X509Certificate2 certificate;
        Repository localRepository;

        public ClientApp(TraceSource ts)
        {
            this.ts = ts;
        }

        public void Run(string[] args)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            ts.TraceInformation("DEV UPDATER CLIENT v" + version);

            ParseArgs(args);

            ResolveClientCertificate();

            InitLocalRepository();

            while (true)
            {

                FetchAndUpdate();

                ExecuteCommand();

                if(!config.WaitForExitAndStartAgain)
                    break;

                Console.WriteLine("Press ENTER to update and execute again.");
                Console.ReadLine();
            }
        }

        private void ParseArgs(string[] args)
        {
            // default config file
            string configFile = System.IO.Path.Combine(FileHelper.CurrentDir, "client-settings.xml");

            // override repository? (first parameter)
            string repositoryName = null;
            if (args != null && args.Length >= 1)
                repositoryName = args[0];

            // override config file (second parameter
            if (args != null && args.Length >= 2)
                configFile = args[1];

            // read config
            config = (Configuration.Client)System.Xaml.XamlServices.Load(configFile);
            if (repositoryName != null)
                config.Repository = repositoryName;
            
            // show settings
            ts.TraceInformation("Settings:");
            ts.TraceInformation(" - Server certificate hash: " + config.ServerCertificateHash);
            ts.TraceInformation(" - URL: " + config.ServerUrl);
            ts.TraceInformation(" - Repository: [{0}]", config.Repository);
        }

        private void ResolveClientCertificate()
        {
            // client cert - get or create
            ts.TraceInformation("Resolving client certificate:");
            string certificatePath = System.IO.Path.Combine(FileHelper.EnsureRootSettingsDirectory(), "devupdater-client-cert.pfx");
            X509Certificate2 cert;
            var newCert = new ClientCertificateResolver(new CertificateGenerator(), certificatePath).ResolveOrGenerateCertificate(out cert);
            if (newCert)
                ts.TraceInformation(" - New certificate has been generated");
            else
                ts.TraceInformation(" - Loaded from file");

            ts.TraceInformation(" - Client certificate hash: " + new Hash(cert.GetCertHash()));

            this.certificate = cert;
        }
        
        private void InitLocalRepository()
        {
            string repoFolder = config.ServerUrl.Authority.Replace(':','_'); // folder from host name and port
            string localRepoPath = System.IO.Path.Combine(FileHelper.CurrentDir, "appcache", repoFolder, config.Repository);
            var localRepositoryAccessor = new DirectoryRepositoryAccessor(localRepoPath, config.Repository);
            localRepository = localRepositoryAccessor.FetchRepository().Result;
        }


        private void FetchAndUpdate()
        {
            // contact server
            ts.TraceInformation("Contacting server...");

            using(var context = new Repositories.Remote.RemoteRepositoryServerContext())
            {
                // setup
                context.BaseUrl = config.ServerUrl;
                context.ClientCertificate = certificate;
                context.ServerCertificateValidator = Repositories.Remote.ServerCertificateHandler.CreateValidationCallback(config.ServerCertificateHash);

                var accessor = new Repositories.Remote.RemoteRepositoryAccessor(context, config.Repository);
                var remoteRepo = accessor.FetchRepository().Result; // fetch repository data from server

                // local repo
                var processor = new Repositories.RepositoryProcessor(ts);
                processor.Synchronize(remoteRepo, localRepository).Wait();
            }
        }

        private void ExecuteCommand()
        {
            if (localRepository.Settings == null || localRepository.Settings.Command == null)
            {
                ts.TraceInformation("No additional steps found.");
                return;
            }

            var s = localRepository.Settings;
            ts.TraceInformation("Executing: {0} {1}", s.Command, s.CommandArgs);

            string executePath = System.IO.Path.Combine(localRepository.FileSystemPath, s.Command);
            var process = Process.Start(executePath, s.Command);

            Console.WriteLine("Waiting for App exit. SID: " + process.SessionId);
            process.WaitForExit();
        }
    }
}
