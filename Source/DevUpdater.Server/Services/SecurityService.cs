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
    public class SecurityService
    {
        public TraceSource TraceSource { get; set; }
        public Func<PersistenceService> PersistenceFactory { get; set; }

        public const string RoleNameClaimType = "RoleName";
        public const string CertificateHashClaimType = "CertificateHash";
        public const string ClientIdClaimType = "ClientId";
        public const string IsKnownClaimType = "IsKnown";
        public const string ClaimIssuer = "DevUpdaterServer";

        public SecurityService(Func<PersistenceService> persistenceFactory)
        {
            PersistenceFactory = persistenceFactory;
        }

        public ClaimsIdentity ProcessClientCertificate(X509Certificate2 cert, string ipAddress)
        {
            using (var per = PersistenceFactory())
            {
                var hash = cert.GetCertHash();
                var client = per.ClientGetByCertificateHash(hash);

                // not found? add to pending certificates list
                if (client == null)
                {
                    TraceSource.TraceInformation("Pending certificate:\n{0} ({1})", ByteArrayHelper.ByteArrayToString(hash), ipAddress);
                    per.PendingCertificateAddOrUpdate(hash, ipAddress);
                    per.Save();
                }

                // build identity
                var identity = new ClaimsIdentity("ClientAuthentication");
                identity.AddClaim(new Claim(CertificateHashClaimType, ByteArrayHelper.ByteArrayToString(hash), ClaimValueTypes.HexBinary, ClaimIssuer));
                identity.AddClaim(new Claim(IsKnownClaimType, client == null ? "false" : "true", ClaimValueTypes.Boolean, ClaimIssuer)); // known client?

                // add details only if authenticated
                if (client != null)
                {
                    identity.AddClaim(new Claim(identity.NameClaimType, client.Name, ClaimValueTypes.String, ClaimIssuer)); // nick name
                    identity.AddClaim(new Claim(ClientIdClaimType, client.Id.ToString(), ClaimValueTypes.Integer, ClaimIssuer)); // ID
                    identity.AddClaims(client.ClientGroups.Select(group => new Claim(identity.RoleClaimType, group.Id.ToString(), ClaimValueTypes.Integer, ClaimIssuer))); // assigned groups
                    identity.AddClaims(client.ClientGroups.Select(group => new Claim(RoleNameClaimType, group.Name, ClaimValueTypes.String, ClaimIssuer))); // assigned groups (names - informative)
                }

                return identity;
            }
        }

        public long[] GetIdentityGroupsIds(ClaimsIdentity identity)
        {
            long[] result = identity.Claims
                .Where(c => c.Type == identity.RoleClaimType && c.Issuer == ClaimIssuer)
                .Select(c => long.Parse(c.Value))
                .ToArray();

            return result;
        }

        public string[] GetIdentityGroupNames(ClaimsIdentity identity)
        {
            string[] result = identity.Claims
                .Where(c => c.Type == RoleNameClaimType && c.Issuer == ClaimIssuer)
                .Select(c => c.Value)
                .ToArray();

            return result;
        }

        public bool GetIdentityIsAuthenticated(ClaimsIdentity identity)
        {
            var result = identity.HasClaim(c => c.Type == IsKnownClaimType && c.Issuer == ClaimIssuer && c.Value.Equals("true", StringComparison.OrdinalIgnoreCase));
            return result;
        }

        public bool AuthorizeClientToRepository(RepositoryServerInstance instance, ClaimsIdentity identity)
        {
            var groupIds = GetIdentityGroupsIds(identity);
            bool result = instance.GroupsAllowed.Keys.Any(groupId => groupIds.Contains(groupId));
            return result;
        }

        public Data.PendingCertificate[] GetPendingCertificates()
        {
            using (var per = PersistenceFactory())
            {
                return per.GetPendingCertificates();
            }
        }
    }
}
