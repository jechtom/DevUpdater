using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevUpdater.Server
{
    public class AuthorizedClient
    {
        public byte[] Hash { get; set; }
        public string Name { get; set; }
    }
}
