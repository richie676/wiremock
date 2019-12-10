using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.Settings;

namespace WiremockStandalone
{
    class Program
    {
        static void Main(string[] args)
        {

            var hostFile = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            IConfiguration appSettings = new ConfigurationBuilder()
                                              .AddJsonFile(Path.Combine(hostFile, "appsettings.json"), true, true)
                                              .Build();

            var blackListedHeaders = new List<string>();
            appSettings.GetSection("blackListedHeaders").Bind(blackListedHeaders);

            FluentMockServerSettings wiremockSettings = null;

            switch (appSettings["mode"])
            {
                case "Record":
                    wiremockSettings = new FluentMockServerSettings
                    {
                        Urls = new[] { appSettings["url"] },
                        AllowPartialMapping = true,
                        StartAdminInterface = true,
                        ReadStaticMappings = true,
                        WatchStaticMappings = true,
                        Logger = new WireMockConsoleLogger(),
                        ProxyAndRecordSettings = new ProxyAndRecordSettings
                        {
                            Url = appSettings["proxyUrl"],
                            SaveMapping = true,
                            SaveMappingToFile = true,
                            BlackListedHeaders = blackListedHeaders.ToArray()
                        }
                    };
                    break;
                case "Play":
                default:
                    wiremockSettings = new FluentMockServerSettings
                    {
                        Urls = new[] { appSettings["url"] },
                        AllowPartialMapping = true,
                        StartAdminInterface = true,
                        ReadStaticMappings = true,
                        WatchStaticMappings = true,
                        Logger = new WireMockConsoleLogger(),
                    };
                    break;
            }

            var wiremockServer = StandAloneApp.Start(wiremockSettings);

            Console.WriteLine("Press any key to stop the server");
            Console.ReadKey();
        }
    }
}
