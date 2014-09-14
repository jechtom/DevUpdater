using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Certificates
{
    public class ClientCertificateResolver
    {
        ICertificateGenerator generator;
        string clientCertificatePath;

        public ClientCertificateResolver(ICertificateGenerator generator, string clientCertificatePath)
        {
            this.generator = generator;
            this.clientCertificatePath = clientCertificatePath;
        }

        public bool ResolveOrGenerateCertificate(out X509Certificate2 cert)
        {
            if (File.Exists(clientCertificatePath))
            {
                cert = LoadFromFile(clientCertificatePath);
                return false; // existing cert
            }
            else
            {
                cert = GenerateAndSave(clientCertificatePath);
                return true; // new cert
            }
        }

        private X509Certificate2 GenerateAndSave(string path)
        {
            Trace.WriteLine("Generating new certificate...");
            var cert = generator.CreateSelfSignedCertificate("devupdater-client", "localhost");
            File.WriteAllBytes(path, cert.Export(X509ContentType.Pfx));
            return cert;
        }

        private X509Certificate2 LoadFromFile(string path)
        {
            X509Certificate2Collection store = new X509Certificate2Collection();
            store.Import(path);
            return store[0];
        }
    }
}
