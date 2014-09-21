using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Repositories
{
    public class RepositoryProcessor
    {
        private System.Diagnostics.TraceSource ts;

        public RepositoryProcessor(TraceSource ts)
        {
            this.ts = ts;
        }

        public ICollection<FileInfoCopyCommand> Compare(Repository source, Repository target)
        {
            List<FileInfo> resultItems = new List<FileInfo>();

            var sourceFiles = source.Files;
            var currentFiles = target.Files;

            // compare
            foreach (var item in sourceFiles)
            {
                if (currentFiles.Any(f => f.Equals(item))) // found same file
                    continue;

                resultItems.Add(item);
            }

            // group by hash (so we don't need to fetch same file twice)
            var result = resultItems.GroupBy(i => i.Hash)
                .Select(g => new FileInfoCopyCommand()
                {
                    Source = g.First(),
                    Targets = g.ToArray()
                }).ToArray();

            // debug - source repository
            ts.TraceInformation("Source respository [{0}]:", source.UrlName);
            ts.TraceInformation(" - Files: {0}", sourceFiles.Count);

            if(sourceFiles.Count > 0)
                ts.TraceInformation(" - Total size: {0}", FileHelper.ResolveFriendlySize(sourceFiles.Sum(sf => sf.Size)));

            // debug - patch
            if (result.Length == 0)
            {
                ts.TraceInformation("All files UP TO DATE");
            }
            else
            {
                ts.TraceInformation("Update required:");
                ts.TraceInformation("- Files to update: {0}", result.Length);
                ts.TraceInformation("- Patch size: {0}", FileHelper.ResolveFriendlySize(result.Sum(f => f.Source.Size)));
            }

            return result;
        }

        public async Task Synchronize(Repository source, Repository target)
        {
            var diff = Compare(source, target);
            await Synchronize(diff, source, target);
        }

        public async Task Synchronize(IEnumerable<FileInfoCopyCommand> filesToSynchronize, Repository source, Repository target)
        {
            if (target.Accessor.IsReadOnly)
                throw new InvalidOperationException("Target repository is read only.");

            long totalSize = filesToSynchronize.Sum(f => f.Source.Size);
            long copiedSize = 0;

            foreach (var item in filesToSynchronize)
            {
                ts.TraceInformation("[{0}%]Updating {1} ({2}) #{3}...", 
                    100 * copiedSize / totalSize,
                    System.IO.Path.GetFileName(item.Source.FileName),
                    FileHelper.ResolveFriendlySize(item.Source.Size),
                    item.Source.Hash.ToString().Substring(0, 6)
                    );

                copiedSize += item.Source.Size;

                var targets = item.Targets.ToArray();
                using (var stream = await source.Accessor.ReadFileAsStream(item.Source))
                {
                    await target.Accessor.WriteFromStream(stream, targets);
                }
            }

            // synchronize properties
            target.Files = source.Files;
            target.UrlName = source.UrlName;
            target.Settings = source.Settings;

            ts.TraceInformation("[100%] Completed!");
        }
    }
}
