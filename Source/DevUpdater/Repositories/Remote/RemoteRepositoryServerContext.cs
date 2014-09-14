using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DevUpdater.Repositories.Remote
{
    public class RemoteRepositoryServerContext : IDisposable
    {
        Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        public Uri BaseUrl { get; set; }

        public X509Certificate2 ClientCertificate { get; set; }

        public RemoteCertificateValidationCallback ServerCertificateValidator { get; set; }

        private WebRequestHandler handler;

        public HttpClient CreateWebClient()
        {
            if(handler == null)
            {
                if (ClientCertificate == null)
                    throw new NullReferenceException("ClientCertificate is null.");

                if (ServerCertificateValidator == null)
                    throw new NullReferenceException("ServerCertificateValidator is null.");

                if (BaseUrl == null)
                    throw new NullReferenceException("BaseUrl is null.");

                handler = new WebRequestHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ClientCertificates.Add(ClientCertificate);
                handler.ServerCertificateValidationCallback = ServerCertificateValidator;
            }

            var result = new HttpClient(handler);
            result.DefaultRequestHeaders.Add("x-devupdater-client-version", version.ToString()); // send version (next version will be able to reject older versions)

            return result;
        }

        public void Dispose()
        {
            if (handler != null)
            {
                handler.Dispose();
                handler = null;
            }
        }
    }
}
