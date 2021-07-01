using CardCollectiveBot.BlackJack.Module;
using CardCollectiveBot.Currency;
using CardCollectiveBot.Currency.Module;
using CardCollectiveBot.Misc;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CardCollectiveBot.Console
{
    public class CommandHandler
    {
        private readonly IServiceProvider _services;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(IServiceProvider services, DiscordSocketClient client, CommandService commands)
        {
            _services = services;
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _commands.AddModulesAsync(Assembly.GetAssembly(typeof(Voice)), _services);
            await _commands.AddModulesAsync(Assembly.GetAssembly(typeof(BlackJackModule)), _services);
            await _commands.AddModulesAsync(Assembly.GetAssembly(typeof(CurrencyModule)), _services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            var context = new SocketCommandContext(_client, message);

            if (!message.HasCharPrefix('!', ref argPos) && !message.Author.IsBot)
            {
                (_services.GetService(typeof(ICurrencyService)) as CurrencyService).DepositCoins(context.User.Id, 10);
            }

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);
        }
    }
}
