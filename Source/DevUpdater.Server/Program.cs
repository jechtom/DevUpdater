using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xaml;

namespace DevUpdater.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceSource ts = new TraceSource("console");
            ts.Listeners.Add(new ConsoleTraceListener());
            try
            {
                // start
                var serverApp = new ServerApp(ts);
                using (serverApp.Run())
                {
                    Console.WriteLine("Server started.");
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Critical, 0, "\nERROR:\n" + e.ToString());
            }
        }
    }
}
