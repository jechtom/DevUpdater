using DevUpdater.Certificates;
using DevUpdater.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceSource ts = new TraceSource("console");
            ts.Switch.Level = SourceLevels.All;
            ts.Listeners.Add(new ConsoleTraceListener());
            try
            {
                var clientApp = new ClientApp(ts);
                clientApp.Run(args);
            }
            catch(Exception e)
            {
                ts.TraceEvent(TraceEventType.Critical, 0, "\nERROR:\n" + e.ToString());
            }
        }
    }
}
