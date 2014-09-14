using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Certificates
{
    public class CertificateGenerator : ICertificateGenerator
    {
        public X509Certificate2 CreateSelfSignedCertificate(string subjectName, string issuerName)
        {
            // Generating Random Numbers
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);

            // The Certificate Generator
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            // Signature Algorithm
            const string signatureAlgorithm = "SHA512withRSA";
            certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);

            // Issuer and Subject Name
            X509Name subjectDN = new X509Name("CN=" + subjectName);
            X509Name issuerDN = new X509Name("CN=" + issuerName);
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            var date = DateTime.Now;
            DateTime notBefore = date.AddDays(-1);
            DateTime notAfter = notBefore.AddYears(2);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // Subject Public Key
            const int keyStrength = 2048;
            var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            // Selfsign certificate
            Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(subjectKeyPair.Private, random);

            // Convert to .NET cert // http://blog.differentpla.net/post/20/how-do-i-convert-a-bouncy-castle-certificate-to-a-net-certificate-
            var store = new Pkcs12Store();
            string friendlyName = certificate.SubjectDN.ToString();
            var certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });
            
            const string password = "password"; // required to export
            byte[] certBytes;
            using (var stream = new MemoryStream())
            {
                store.Save(stream, password.ToCharArray(), random);
                certBytes = stream.ToArray();
            }

            var convertedCertificate =
                new X509Certificate2(certBytes, password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            
            return convertedCertificate;
        }
    }

    // This one works, but requires CERTENROLLLib COM with ADMIN access
    //public class CertificateGenerator
    //{
    //    public static X509Certificate2 CreateSelfSignedCertificate(string subjectName)
    //    {
    //        // create DN for subject and issuer
    //        var dn = new CX500DistinguishedName();
    //        dn.Encode("CN=" + subjectName, X500NameFlags.XCN_CERT_NAME_STR_NONE);

    //        // create a new private key for the certificate
    //        CX509PrivateKey privateKey = new CX509PrivateKey();
    //        privateKey.ProviderName = "Microsoft Base Cryptographic Provider v1.0";
    //        privateKey.MachineContext = true;
    //        privateKey.Length = 2048;
    //        privateKey.KeySpec = X509KeySpec.XCN_AT_SIGNATURE; // use is not limited
    //        privateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG;
    //        privateKey.Create();

    //        // Use the stronger SHA512 hashing algorithm
    //        var hashobj = new CObjectId();
    //        hashobj.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID,
    //            ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY,
    //            AlgorithmFlags.AlgorithmFlagsNone, "SHA512");

    //        // add extended key usage if you want - look at MSDN for a list of possible OIDs
    //        var oid = new CObjectId();
    //        oid.InitializeFromValue("1.3.6.1.5.5.7.3.1"); // SSL server
    //        var oidlist = new CObjectIds();
    //        oidlist.Add(oid);
    //        var eku = new CX509ExtensionEnhancedKeyUsage();
    //        eku.InitializeEncode(oidlist);

    //        // Create the self signing request
    //        var cert = new CX509CertificateRequestCertificate();
    //        cert.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextMachine, privateKey, "");
    //        cert.Subject = dn;
    //        cert.Issuer = dn; // the issuer and the subject are the same
    //        var date = DateTime.Now;
    //        cert.NotBefore = date.AddDays(-1);
    //        cert.NotAfter = date.AddYears(1);
    //        cert.X509Extensions.Add((CX509Extension)eku); // add the EKU
    //        cert.HashAlgorithm = hashobj; // Specify the hashing algorithm
    //        cert.Encode(); // encode the certificate

    //        // Do the final enrollment process
    //        var enroll = new CX509Enrollment();
    //        enroll.InitializeFromRequest(cert); // load the certificate
    //        enroll.CertificateFriendlyName = subjectName; // Optional: add a friendly name
    //        string csr = enroll.CreateRequest(); // Output the request in base64
    //        // and install it back as the response
    //        enroll.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedCertificate,
    //            csr, EncodingType.XCN_CRYPT_STRING_BASE64, ""); // no password
    //        // output a base64 encoded PKCS#12 so we can import it back to the .Net security classes
    //        var base64encoded = enroll.CreatePFX("", // no password, this is for internal consumption
    //            PFXExportOptions.PFXExportChainWithRoot);

    //        // instantiate the target class with the PKCS#12 data (and the empty password)
    //        return new System.Security.Cryptography.X509Certificates.X509Certificate2(
    //            System.Convert.FromBase64String(base64encoded), "",
    //            // mark the private key as exportable (this is usually what you want to do)
    //            System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable
    //        );
    //    }
    //}
}
