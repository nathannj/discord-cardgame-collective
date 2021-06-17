using CardCollectiveBot.Common;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CardCollectiveBot.Console
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupDefaultServiceCollection(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddSingleton<ICommandLoggingService, CommandLoggingService>()
                .AddSingleton(new CommandService())
                .AddSingleton(new DiscordSocketClient())
                .Configure<Common.Configuration.Discord>(config.GetSection(nameof(Common.Configuration.Discord)));
        }
    }
}
