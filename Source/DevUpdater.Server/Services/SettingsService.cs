using DevUpdater.Server.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Services
{
    public class SettingsService
    {
        public TraceSource TraceSource { get; set; }
        public Func<ServerDataContext> DataContextFactory { get; set; }

        public Models.SettingsModel GetSettings()
        {
            var result = new Models.SettingsModel();
 	        using(var cx = DataContextFactory())
            {
                result.Clients.AddRange(cx.Clients.ToArray().Select(o => new Models.ClientModel()
                    {
                        Id = o.Id,
                        Name = o.Name,
                        CertificateHash = ByteArrayHelper.ByteArrayToString(o.CertificateHash)
                    }));

                result.Groups.AddRange(cx.ClientGroups.Include("Clients").ToArray().Select(o => new Models.GroupModel()
                {
                    Id = o.Id,
                    Name = o.Name,
                    Users = o.Clients.Select(c => c.Id).ToList()
                }));

                result.Repositories.AddRange(cx.Repositories.Include("ClientGroups").ToArray().Select(o => new Models.RepositoryModel()
                {
                    Id = o.Id,
                    Name = o.UrlName,
                    SourceFolder = o.SourceFolder,
                    Groups = o.ClientGroups.Select(c => c.Id).ToList()
                }));

                result.PendingCertificates.AddRange(cx.PendingCertificates.ToArray().Select(o => new Models.PendingCertificateModel()
                {
                    Id = o.Id,
                    CertificateHash = ByteArrayHelper.ByteArrayToString(o.CertificateHash),
                    IpAddress = o.IpAddress,
                    LastAttemptUtc = o.LastAttemptUtc
                }));
            }

            return result;
        }
    }
}
