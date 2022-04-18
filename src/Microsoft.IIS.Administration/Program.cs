// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration {
    using AspNetCore.Builder;
    using AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.HttpSys;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.EventLog;
    using Serilog;
    using System;
    using System.Diagnostics;

    public class Program {
        public const string EventSourceName = "Microsoft IIS Administration API";

        public static void Main(string[] args) {
            try
            {
                //
                // Build Config
                var configHelper = new ConfigurationHelper(args);
                IConfiguration config = configHelper.Build();

                //
                // Initialize runAsAService local variable
                string serviceName = config.GetValue<string>("serviceName")?.Trim();
                bool runAsAService = !string.IsNullOrEmpty(serviceName);

                //
                // Host


                Host.CreateDefaultBuilder(args)
                   .UseContentRoot(configHelper.RootPath)
                   .ConfigureLogging((hostingContext, logging) =>
                   {
                       logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                       //
                       // Console log is not available in running as a Service
                       if (!runAsAService)
                       {
                           logging.AddConsole();
                       }

                       logging.AddDebug();
                       logging.AddEventLog(new EventLogSettings()
                       {
                           SourceName = EventSourceName
                       });
                   })
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder.UseStartup<Startup>()
                       .UseConfiguration(config)
                       .ConfigureServices(s => s.AddSingleton(config));
                   })
                    //.UseConfiguration(config)
                    //.ConfigureServices(s => s.AddSingleton(config)) // Configuration Service
                    //.UseStartup<Startup>()
                    .Build()
                    .Run();

            }
            catch (Exception ex)
            {
                using (var shutdownLog = new EventLog("Application"))
                {
                    shutdownLog.Source = Program.EventSourceName;
                    shutdownLog.WriteEntry($"Microsoft IIS Administration API has shutdown unexpectively because the error: {ex.ToString()}", EventLogEntryType.Error);
                }
                throw ex;
            }
        }
    }
}
