using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace CardCollectiveBot.Common
{
    public class CommandLoggingService : ICommandLoggingService
    {
        private readonly ILogger _logger;

        public CommandLoggingService(DiscordSocketClient client, CommandService command, ILogger<ICommandLoggingService> logger)
        {
            _logger = logger;
            client.Log += LogAsync;
            command.Log += LogAsync;
        }

        public Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                _logger.LogError($"{cmdException.Command.Aliases.First()}"
                                  + $" failed to execute in {cmdException.Context.Channel}. " +
                                  $"ERROR: {cmdException}");
            }
            else if(message.Severity == LogSeverity.Warning)
            {
                _logger.LogWarning($"[General/{message.Severity}] {message}");
            }
            else
            {
                _logger.LogInformation($"[General/{message.Severity}] {message}");
            }

            return Task.CompletedTask;
        }
    }
}
