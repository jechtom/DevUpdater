using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevUpdater.Server
{
    public class AuthorizedClientsList
    {
        string path;
        object syncLock = new object();
        List<AuthorizedClient> clients = new List<AuthorizedClient>(); 

        public AuthorizedClientsList(string path)
        {
            this.path = path;
            FetchClients();
        }

        public bool Exists(byte[] thumb)
        {
            AuthorizedClient client;
            return Exists(thumb, out client);
        }

        public bool Exists(byte[] thumb, out AuthorizedClient client)
        {
            var item = clients.FirstOrDefault(c => Enumerable.SequenceEqual(c.Hash, thumb));
            if(item == null)
            {
                client = null;
                return false;
            }

            // found
            client = item;
            return true;
        }


        public void Add(AuthorizedClient item)
        {
            lock(syncLock)
            {
                if (Exists(item.Hash))
                    return;

                clients.Add(item);

                Save();
            }
        }

        private void Save()
        {
            File.WriteAllLines(path, clients.Select(c => ByteArrayHelper.ByteArrayToString(c.Hash) + " " + c.Name));
        }

        private void FetchClients()
        {
            if (!File.Exists(path))
                File.WriteAllLines(path, new string[0]);

            clients = File.ReadAllLines(path).Select(ParseClientLine).ToList();
        }

        private AuthorizedClient ParseClientLine(string arg)
        {
            int index = arg.IndexOf(' ');
            if (index < 0)
                return new AuthorizedClient() { Hash = ByteArrayHelper.StringToByteArray(arg) };
            else
                return new AuthorizedClient() { 
                    Hash = ByteArrayHelper.StringToByteArray(arg.Substring(0, index)),
                    Name = arg.Substring(index + 1) 
                };
        }
    }
}
