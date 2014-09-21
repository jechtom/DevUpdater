using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevUpdater.Server.Models
{
    public class GroupModel
    {
        public GroupModel()
        {
            Users = new List<long>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public List<long> Users { get; set; }
    }
}
