using CardCollectiveBot.BlackJack;
using CardCollectiveBot.Common;
using CardCollectiveBot.Currency;
using CardCollectiveBot.Data;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CardCollectiveBot.Console
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupDefaultServiceCollection(this IServiceCollection services, IConfiguration config)
        {

            return services
                .AddDbContext<CardCollectiveBotContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(CardCollectiveBotContext).Assembly.FullName)))
                .AddScoped<ICommandLoggingService, CommandLoggingService>()
                .AddScoped<IGameService, GameService>()
                .AddScoped<ICurrencyService, CurrencyService>()
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Info,
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async
                }))
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10,
                    DefaultRetryMode = RetryMode.RetryRatelimit
                }))
                .Configure<Common.Configuration.Discord>(config.GetSection(nameof(Common.Configuration.Discord)))
                .AddLogging(configure =>
                {
                    configure.AddConsole();
                    configure.AddSerilog();
                });
        }
    }
}
