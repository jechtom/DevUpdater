using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Data
{
    public class SQLiteDbProviderService
    {
        static object syncLock = new object();
        static DbProviderServices instance;

        public static DbProviderServices Instance
        {
            get
            {
                if(instance == null)
                    lock(syncLock)
                        if(instance == null)
                        {
                            var factory = System.Data.SQLite.EF6.SQLiteProviderFactory.Instance;
                            instance = (DbProviderServices)factory.GetService(typeof(DbProviderServices));
                        }

                return instance;
            }
        }
    }
}
