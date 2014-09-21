using DevUpdater.Server.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevUpdater.Server.Services
{
    public class RepositoryService
    {
        public TraceSource TraceSource { get; set; }
        public Func<PersistenceService> PersistenceFactory { get; set; }
        public Repositories.RepositoryProcessor RepositoryProcessor { get; set; }
        public string AppCachePath { get; set; }

        private readonly object syncLock = new object();
        private Dictionary<string, RepositoryServerInstance> repositoriesCache;

        public RepositoryService(Func<PersistenceService> persistenceFactory)
        {
            PersistenceFactory = persistenceFactory;
            AppCachePath = Path.Combine(FileHelper.CurrentDir, "appcache-server");
        }

        public void Init()
        {
            Synchronize();
        }

        public void Synchronize()
        {
            var dict = new Dictionary<string, Repositories.Repository>(StringComparer.OrdinalIgnoreCase);
            TraceSource.TraceInformation("Synchronizing repositories...");

            var repositories = new Dictionary<string, RepositoryServerInstance>(StringComparer.OrdinalIgnoreCase);
            Data.Repository[] repoDtos;
            using (var per = PersistenceFactory())
            {
                repoDtos = per.GetRepositories();

                foreach (var dto in repoDtos)
                {
                    var cacheItem = InitAndSynchronizeRepository(dto);
                    repositories.Add(cacheItem.Value.UrlName, cacheItem);
                }
            }

            lock(syncLock)
            {
                repositoriesCache = repositories;
            }
        }

        private RepositoryServerInstance InitAndSynchronizeRepository(Repository dto)
        {
            // create cache
            var cacheRepoAccessor = new Repositories.DirectoryCompressedRepositoryAccessor(Path.Combine(AppCachePath, dto.UrlName), dto.UrlName);
            var cacheRepo = cacheRepoAccessor.FetchRepository().Result;

            // create result instance
            var instance = new RepositoryServerInstance()
            {
                Dto = dto,
                GroupsAllowed = dto.ClientGroups.ToDictionary(cg => cg.Id),
                Value = cacheRepo
            };

            // sync
            try
            {
                SynchronizeRepository(instance).Wait();
            }
            catch(AggregateException ae)
            {
                throw ae.Flatten().InnerException;
            }

            return instance;
        }

        public async Task SynchronizeRepository(RepositoryServerInstance instance)
        {
            // TODO: race conditions

            // link to source folder
            var sourceRepoAccessor = new Repositories.DirectoryRepositoryAccessor(instance.Dto.SourceFolder, instance.Dto.UrlName);
            var sourceRepo = await sourceRepoAccessor.FetchRepository();

            // apply settings to repository
            sourceRepo.Settings = new Repositories.RepositorySettings()
            {
                Command = instance.Dto.Command,
                CommandArgs = instance.Dto.CommandArgs
            };

            // synchronize
            await RepositoryProcessor.Synchronize(sourceRepo, instance.Value);
        }

        public RepositoryServerInstance GetByUrlId(string repository)
        {
            lock (syncLock)
            {
                RepositoryServerInstance value;

                if (!repositoriesCache.TryGetValue(repository, out value))
                {
                    return null;
                }

                return value;
            }
        }
    }
}
