using System;
using System.Security.Cryptography.X509Certificates;
namespace DevUpdater.Certificates
{
    public interface ICertificateGenerator
    {
        X509Certificate2 CreateSelfSignedCertificate(string subjectName, string issuerName);
    }
}
