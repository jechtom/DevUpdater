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

        public AuthenticationClientCertListOptions(AuthorizedClientsList list)
            : base("client-cert-list")
        {
            this.ClientList = list;
        }

        public AuthorizedClientsList ClientList { get; set; }

        public bool AddPending(byte[] thumb)
        {
            lock(pendingCerts)
                return pendingCerts.Add(ByteArrayHelper.ByteArrayToString(thumb));
        }
    }
}
