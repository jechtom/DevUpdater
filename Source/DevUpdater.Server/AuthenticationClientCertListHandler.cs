using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server
{
    public class AuthenticationClientCertListHandler : AuthenticationHandler<AuthenticationClientCertListOptions>
    {
        static readonly object consoleSyncLock = new object();

        public AuthenticationClientCertListHandler()
        {
        }

        protected override Task<Microsoft.Owin.Security.AuthenticationTicket> AuthenticateCoreAsync()
        {
            var cert = Context.Get<X509Certificate2>("ssl.ClientCertificate");
            if (cert == null)
            {
                return Task.FromResult<AuthenticationTicket>(null);
            }

            ClaimsIdentity identity;
            identity = Options.SecurityService.ProcessClientCertificate(cert, Context.Request.RemoteIpAddress);

            if(identity == null) // unauthenticated
            {
                return Task.FromResult<AuthenticationTicket>(null);
            }
            
            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            return Task.FromResult<AuthenticationTicket>(ticket);
        }
    }
}
