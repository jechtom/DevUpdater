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
            ts.Switch.Level = SourceLevels.All;
            ts.Listeners.Add(new ConsoleTraceListener());
            try
            {
                // start
                var serverApp = new ServerApp(ts);
                using (serverApp.Run(args))
                {
                    Console.WriteLine("Server started.");
                    Thread.Sleep(Timeout.Infinite);

                    //serverApp.ShowControlForm();
                    //Console.WriteLine("Server started.");
                    //Console.WriteLine("Press [Enter] to exit...");
                    //Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Critical, 0, "\nERROR:\n" + e.ToString());
            }
        }
    }
}
