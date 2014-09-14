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

            var thumb = cert.GetCertHash();
            var thumbStr = ByteArrayHelper.ByteArrayToString(thumb);
            AuthorizedClient client;
            if (!Options.ClientList.Exists(thumb, out client))
            {
                bool accept = false;
                lock (consoleSyncLock)
                {
                    if (Options.AddPending(thumb))
                    {
                        Console.WriteLine(
                            "Unauthorized client certificate ({0}):\nTHUMB: {1}",
                            Context.Request.RemoteIpAddress,
                            thumbStr);
                        Console.WriteLine("Authorize this client? (Y/N)");
                        if(string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Enter name of client:");
                            string nickName = Console.ReadLine();
                            accept = true;
                            Options.ClientList.Add(new AuthorizedClient() { Hash = thumb, Name = nickName });
                            Console.WriteLine("Client accepted.");
                        }
                        else
                        {
                            Console.WriteLine("Client declined.");
                        }
                    }
                }

                if(!accept)
                    return Task.FromResult<AuthenticationTicket>(null);
            }
            else
            {
                if (Options.AddPending(thumb))
                {
                    Console.WriteLine("Client connected: \"{0}\" ({1})", client.Name, Context.Request.RemoteIpAddress);
                }
            }
            
            var identity = new GenericIdentity(new Hash(cert.GetCertHash()).ToString());
            identity.AddClaim(new Claim("nickname", client.Name));
            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            return Task.FromResult<AuthenticationTicket>(ticket);
        }
    }
}
