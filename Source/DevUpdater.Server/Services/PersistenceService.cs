using DevUpdater.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Services
{
    public class PersistenceService : IDisposable
    {
        Data.ServerDataContext context;

        public PersistenceService(Data.ServerDataContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.context = context;
        }

        public Data.Client ClientGetByCertificateHash(byte[] hash)
        {
            var result = context.Clients.FirstOrDefault(c => c.CertificateHash == hash);
            return result;
        }

        public void PendingCertificateAddOrUpdate(byte[] hash, string ipAddress)
        {
            var pending = context.PendingCertificates.FirstOrDefault(p => p.CertificateHash == hash);
            if (pending == null)
            {
                pending = new PendingCertificate()
                {
                    CertificateHash = hash
                };
                context.PendingCertificates.Add(pending);
            }

            pending.IpAddress = ipAddress;
            pending.LastAttemptUtc = DateTime.UtcNow;
        }

        public Repository[] GetRepositories()
        {
            var result = context.Repositories.ToArray();
            return result;
        }

        public PendingCertificate[] GetPendingCertificates()
        {
            return context.PendingCertificates.ToArray();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
            context = null;
        }
    }
}
