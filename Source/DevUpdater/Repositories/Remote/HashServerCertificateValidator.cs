using DevUpdater.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Repositories.Remote
{
    public class ServerCertificateHandler
    {
        static bool serverCertAccepted = false;

        public static RemoteCertificateValidationCallback CreateValidationCallback(Hash serverCertThumb)
        {
            return new RemoteCertificateValidationCallback(
                (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
                {
                    if(sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
                        return false;
                    
                    // compare server thumbprint
                    var result = serverCertThumb.Equals(new Hash(certificate.GetCertHash()));
                    if (result == true)
                    {
                        if (!serverCertAccepted)
                        {
                            serverCertAccepted = true;
                            Trace.WriteLine("Server certificate accepted:");
                            Trace.WriteLine(" - thumb: " + ByteArrayHelper.ByteArrayToString(certificate.GetCertHash()));
                            Trace.WriteLine(" - public key: " + certificate.GetPublicKey().Length * 8 + " bits");
                            Trace.WriteLine(" - alg: " + certificate.GetKeyAlgorithm());
                            Trace.WriteLine(" - expiration: " + certificate.GetExpirationDateString());
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Server certificate thumbprint mismatch!");
                    }
                    return result;
            });
        }
    }
}
