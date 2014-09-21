using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            TraceSource ts = new TraceSource("server");
            ts.Switch.Level = SourceLevels.All;
            ts.Listeners.Add(new ConsoleTraceListener());
            Startup.DefaultTraceSource = ts;

            try
            {
                // parse args and start up the server
                string serverUrl = "https://+:25427"; // default url
                if (args != null && args.Length >= 1)
                    serverUrl = args[0];

                // setup
                StartOptions owinSettings = new StartOptions();
                ts.TraceInformation("Starting web server...");
                owinSettings.Urls.Add(serverUrl);
                foreach (var url in owinSettings.Urls)
                {
                    ts.TraceInformation(" - URL: " + url);
                }

                // start
                using(WebApp.Start<Startup>(owinSettings))
                {
                    Console.WriteLine("Server started.");
                    Console.WriteLine("Press [Enter] to exit...");
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Critical, 0, "\nERROR:\n" + e.ToString());
            }
        }
    }
}
