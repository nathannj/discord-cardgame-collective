using CardCollectiveBot.Common;
using CardCollectiveBot.Common.Responses;
using CardCollectiveBot.Currency;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardCollectiveBot.BlackJack.Module
{
    [Group("Blackjack")]
    public class BlackJack : ModuleBase<SocketCommandContext>
    {
        public IGameService _gameService { get; set; }

        public ICurrencyService _currencyService { get; set; }

        public ICommandLoggingService _loggingService { get; set; }

        public BlackJack(IGameService gameService, ICurrencyService currencyService, ICommandLoggingService loggingService)
        {
            _gameService = gameService;
            _currencyService = currencyService;
        }

        protected override void OnModuleBuilding(CommandService commandService, ModuleBuilder builder)
        {
            base.OnModuleBuilding(commandService, builder);
        }

        [Command("Create")]
        public async Task CreateBlackJack()
        {
            var response = _gameService.CreateGame(Context.User as IGuildUser);

            await GenerateBlackJackResponse(response);
        }

        [Command("Join")]
        public async Task JoinBlackJack(int wager)
        {
            var GetCoinsResponse = _currencyService.GetCoins(Context.User.Id);

            if(GetCoinsResponse.IsSuccess &&  GetCoinsResponse.Payload >= wager && _currencyService.WithdrawCoins(Context.User.Id, wager).IsSuccess)
            {
                var joinGameResponse = _gameService.JoinGame(Context.User as IGuildUser, wager);
                
                await GenerateBlackJackResponse(joinGameResponse);
            }
            else
            {
                await Context.Channel.SendMessageAsync($"An error has occured: Please ensure you have an account with a valid number of coins. (For assistance with this please tpye !currency help)");
            }
            
        }

        [Command("Start")]
        public async Task StartBlackJack()
        {
            var response = _gameService.StartGame(Context.User as IGuildUser);

            await GenerateBlackJackResponse(response);
        }

        [Command("Hit")]
        public async Task HitBlackJack()
        {
            var response = _gameService.Hit(Context.User as IGuildUser);
                      

            await GenerateBlackJackResponse(response);
        }

        [Command("Stand")]
        public async Task StandBlackJack()
        {
            var response = _gameService.Stand(Context.User as IGuildUser);

            await GenerateBlackJackResponse(response);
        }

        [Command("Reset")]
        public async Task ResetBlackJack()
        {
            var response = _gameService.ResetGame(Context.Guild.Id, true);

            await GenerateBlackJackResponse(response);
        }

        [Command("Delete")]
        public async Task DeleteBlackJack()
        {
            var response = _gameService.DeleteGame(Context.User as IGuildUser);

            await GenerateBlackJackResponse(response);
        }

        private async Task GenerateBlackJackResponse(IResponse<EmbedBuilder> response)
        {
            if (response.Payload != null)
            {
                await Context.Channel.SendMessageAsync("", false, response.Payload.Build());
            }


            if (response.OtherMessages != null)
            {
                foreach (var message in response.OtherMessages)
                {
                    await _loggingService.LogAsync(new LogMessage(LogSeverity.Info, nameof(BlackJack), message));
                    await Context.Channel.SendMessageAsync($"{message}");
                }
            }

            if(response?.Rewards != null && response.Rewards.Any())
            {
                foreach(var reward in response.Rewards)
                {
                    await _loggingService.LogAsync(new LogMessage(LogSeverity.Info, nameof(BlackJack), $"REWARD: {reward.Value} given to NAME:{reward.Nickname} USERID:{reward.Id}"));
                    _currencyService.DepositCoins(reward.Id, reward.Value);
                }
            }
        }
    }
}
