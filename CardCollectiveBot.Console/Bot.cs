using CardCollectiveBot.Common;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CardCollectiveBot.Console
{
    public class Bot : BackgroundService
    {
        private readonly ILogger<Bot> _logger;
        private readonly ILogger<ICommandLoggingService> _commandLogger;

        private DiscordSocketClient _client;
        private CommandService _commands;

        private readonly IConfiguration _config;

        public Bot(ILogger<Bot> logger, ILogger<ICommandLoggingService> commandLogger, IConfiguration config)
        {
            _logger = logger;
            _commandLogger = commandLogger;
            _config = config;
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            _client = new DiscordSocketClient();

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async
            });

            new CommandLoggingService(_client, _commands, _commandLogger);

            var services = new ServiceCollection().SetupDefaultServiceCollection(_config).BuildServiceProvider();
            
            await _client.LoginAsync(TokenType.Bot, _config.GetValue<string>("Discord:Token"));
            await _client.StartAsync();

            await new CommandHandler(services, _client, _commands).InstallCommandsAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
