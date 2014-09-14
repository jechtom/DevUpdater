using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server
{
    public class AuthenticationClientCertListMiddleware : AuthenticationMiddleware<AuthenticationClientCertListOptions>
    {
        public AuthenticationClientCertListMiddleware(OwinMiddleware next, AuthenticationClientCertListOptions authOptions)
            :base(next, authOptions)
        {
        }

        protected override AuthenticationHandler<AuthenticationClientCertListOptions> CreateHandler()
        {
            return new AuthenticationClientCertListHandler();
        }
    }
}
