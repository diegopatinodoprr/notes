﻿using System.IO;
using Autofac.Extensions.DependencyInjection;
using Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog.Web;


namespace MeatsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var waitAndRetry = DatabaseConnectionHelper.BuildWaitAndRetryForDatabaseConnection(EnvionmentVariables.MeatsDatabaseLastRetryInSeconds);


            return waitAndRetry.Execute(() =>


             new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(s => s.AddAutofac())
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    config
                        .AddEnvironmentVariables();

                    var nlogEnvironmentFragment = env.IsProduction() ? string.Empty : $".{env.EnvironmentName}";
                    var nlogFileName = $"nlog{nlogEnvironmentFragment}.config";
                    var nlogFileNameOrDefault = env.ContentRootFileProvider.GetFileInfo(nlogFileName).Exists
                        ? nlogFileName
                        : "nlog.config";
                    env.ConfigureNLog(nlogFileNameOrDefault);
                })
                .UseNLog()
                .UseStartup<Startup>()
                .UseUrls("http://*:5009/")
                                        .Build()
                                       );
        }
    }
}
