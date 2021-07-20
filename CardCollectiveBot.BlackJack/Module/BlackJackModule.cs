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
    [Alias("bj")]
    public class BlackJackModule : ModuleBase<SocketCommandContext>
    {
        public IGameService _gameService { get; set; }

        public ICurrencyService _currencyService { get; set; }

        public ICommandLoggingService _loggingService { get; set; }

        public BlackJackModule(IGameService gameService, ICurrencyService currencyService, ICommandLoggingService loggingService)
        {
            _gameService = gameService;
            _currencyService = currencyService;
        }

        protected override void OnModuleBuilding(CommandService commandService, ModuleBuilder builder)
        {
            base.OnModuleBuilding(commandService, builder);
        }

        [Command("Help")]
        public async Task Help()
        {
            var embedBuilder = new EmbedBuilder
            {
                Color = new Color(255, 200, 0),
                Title = "Blackjack - Help",
                Description = "Commands and Information about Blackjack",
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder{Name = "!Blackjack Create", Value = "If not already existing, a blackjack table for this channel will be created."},
                    new EmbedFieldBuilder{Name = "!Blackjack Join", Value = "Adds the user to the channels existing table."},
                    new EmbedFieldBuilder{Name = "!Blackjack Lobby", Value = "This will return the players in the match along with their wagers."},
                    new EmbedFieldBuilder{Name = "!Blackjack Start", Value = "Starts the match and stops anyone else from joining."},
                    new EmbedFieldBuilder{Name = "!Blackjack DoubleDown", Value = "Call a double down"},
                    new EmbedFieldBuilder{Name = "!Blackjack SplitPair", Value = "Split pairs."},
                    new EmbedFieldBuilder{Name = "!Blackjack Hit", Value = "Calls hit and gives a new card to the user."},
                    new EmbedFieldBuilder{Name = "!Blackjack Stand", Value = "Calls stand which will stop the user making anymore calls until the end of the match."},
                    new EmbedFieldBuilder{Name = "!Blackjack Reset", Value = "This will reset the match and players will have to rejoin. This will refund players from the game being reset."},
                    new EmbedFieldBuilder{Name = "!Blackjack Delete", Value = "This will delete Save data, !Blackjack create will have to be called again to create new data."},
                    new EmbedFieldBuilder{Name = "Shorthand command aliases", Value = "The following words have acronyms to make using them easier:\nBlackjack - bj\nHit - h\nStand - s\n DoubleDown - dd\nSplitPair - sp\nJoin - j"}
                }
            };

            await ReplyAsync("", false, embedBuilder.Build());
        }

        [Command("Create")]
        public async Task CreateBlackJack()
        {
            var response = _gameService.CreateGame(Context.User as IGuildUser, Context.Channel.Id);

            await GenerateBlackJackResponse(response);
        }

        [Command("Join")]
        [Alias("j")]
        public async Task JoinBlackJack(int wager)
        {
            var GetCoinsResponse = _currencyService.GetCoins(Context.User.Id);

            if(GetCoinsResponse.IsSuccess && GetCoinsResponse.Payload > 0 && GetCoinsResponse.Payload >= wager && _currencyService.WithdrawCoins(Context.User.Id, wager).IsSuccess)
            {
                var joinGameResponse = _gameService.JoinGame(Context.User as IGuildUser, Context.Channel.Id, wager);
                
                await GenerateBlackJackResponse(joinGameResponse);
            }
            else
            {
                await ReplyAsync($"An error has occured: Please ensure you have an account with a valid number of coins. (For assistance with this please tpye !currency help)");
            }
            
        }

        [Command("Start")]
        public async Task StartBlackJack()
        {
            var response = _gameService.StartGame(Context.User as IGuildUser, Context.Channel.Id);

            await GenerateBlackJackResponse(response);
        }

        [Command("DoubleDown")]
        [Alias("dd")]
        public async Task DoubleDownBlackJack()
        {
            var playersWager = _gameService.GetPlayerWager(Context.User as IGuildUser, Context.Channel.Id);

            var GetCoinsResponse = _currencyService.GetCoins(Context.User.Id);

            IResponse<EmbedBuilder> response = null;

            if(playersWager.IsSuccess && playersWager.Payload > 0 
                && GetCoinsResponse.IsSuccess && GetCoinsResponse.Payload > 0 && GetCoinsResponse.Payload >= playersWager.Payload
                && _currencyService.WithdrawCoins(Context.User.Id, playersWager.Payload).IsSuccess)
            {                
                response = _gameService.DoubleDown(Context.User as IGuildUser, Context.Channel.Id);

                await ReplyAsync($"{Context.User.Username} has Doubled Down. {Context.User.Username}'s wager is now at {playersWager.Payload*2}");
            }

            await GenerateBlackJackResponse(response);
        }

        [Command("SplitPair")]
        [Alias("sp")]
        public async Task SplittingPairsBlackJack()
        {
            var response = _gameService.SplitPair(Context.User as IGuildUser, Context.Channel.Id);

            await GenerateBlackJackResponse(response);
        }

        [Command("Hit")]
        [Alias("h")]
        public async Task HitBlackJack()
        {
            var response = _gameService.Hit(Context.User as IGuildUser, Context.Channel.Id);
                      

            await GenerateBlackJackResponse(response);
        }

        [Command("Stand")]
        [Alias("s")]
        public async Task StandBlackJack()
        {
            var response = _gameService.Stand(Context.User as IGuildUser, Context.Channel.Id);

            await GenerateBlackJackResponse(response);
        }

        [Command("Reset")]
        public async Task ResetBlackJack()
        {
            var response = _gameService.ResetGame(Context.Guild.Id, Context.Channel.Id, true);

            await GenerateBlackJackResponse(response);
        }

        [Command("Delete")]
        public async Task DeleteBlackJack()
        {
            var response = _gameService.DeleteGame(Context.User as IGuildUser, Context.Channel.Id);

            await GenerateBlackJackResponse(response);
        }

        [Command("Lobby")]
        public async Task Lobby()
        {
            var response = _gameService.GetLobby(Context.User as IGuildUser, Context.Channel.Id);

            await GenerateBlackJackResponse(response);
        }

        private async Task GenerateBlackJackResponse(IResponse<EmbedBuilder> response)
        {
            if (response?.Payload != null)
            {
                await Context.Channel.SendMessageAsync("", false, response.Payload.Build());
            }


            if (response?.OtherMessages != null)
            {
                foreach (var message in response.OtherMessages)
                {
                    await _loggingService.LogAsync(new LogMessage(LogSeverity.Info, nameof(BlackJackModule), message));
                    await Context.Channel.SendMessageAsync($"{message}");
                }
            }

            if(response?.Rewards != null && response.Rewards.Any())
            {
                foreach(var reward in response.Rewards)
                {
                    await _loggingService.LogAsync(new LogMessage(LogSeverity.Info, nameof(BlackJackModule), $"REWARD: {reward.Value} given to NAME:{reward.Nickname} USERID:{reward.Id}"));
                    _currencyService.DepositCoins(reward.Id, reward.Value);
                }
            }
        }
    }
}
