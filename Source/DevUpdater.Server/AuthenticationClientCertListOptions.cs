using DevUpdater.Server.Services;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server
{
    public class AuthenticationClientCertListOptions : AuthenticationOptions
    {
        HashSet<string> pendingCerts = new HashSet<string>();

        public AuthenticationClientCertListOptions(SecurityService service)
            : base("client-cert-list")
        {
            this.SecurityService = service;
        }

        public bool AddPending(byte[] thumb)
        {
            lock(pendingCerts)
                return pendingCerts.Add(ByteArrayHelper.ByteArrayToString(thumb));
        }

        public SecurityService SecurityService { get; set; }
    }
}
