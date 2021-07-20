using CardCollectiveBot.Uno.Cards;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.IO;

namespace CardCollectiveBot.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(configure =>
                    {
                        configure.AddConsole();
                        configure.AddSerilog();
                    });

                    var serviceCollection = services.BuildServiceProvider();
                    var config = serviceCollection.GetService<IConfiguration>();
                    var loggingFileLocation = config.GetValue<string>("LoggingFileLocation");

                    Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(loggingFileLocation)
                    .CreateLogger();

                    services.SetupDefaultServiceCollection(config)
                    .AddHostedService<Bot>();
                });
    }
}
