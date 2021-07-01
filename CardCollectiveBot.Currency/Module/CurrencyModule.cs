using CardCollectiveBot.Common;
using CardCollectiveBot.Common.Responses;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardCollectiveBot.Currency.Module
{
    [Group("Currency")]
    public class CurrencyModule : ModuleBase<SocketCommandContext>
    {
        public ICurrencyService _currencyService { get; set; }

        public ICommandLoggingService _loggingService { get; set; }

        public CurrencyModule(ICurrencyService currencyService, ICommandLoggingService loggingService)
        {
            _currencyService = currencyService;
            _loggingService = loggingService;
        }

        [Command("")]
        public async Task Currency()
        {
            var coins = _currencyService.GetCoins(Context.User.Id);

            await Context.Channel.SendMessageAsync(coins.IsSuccess ? $"{Context.User} has {coins.Payload} mangcoins" : "You may not have a coin account on this server. please use _!Currency create}_ to set up an account.");
        }

        [Command("help")]
        public async Task Help()
        {
            var embedBuilder = new EmbedBuilder
            {
                Color = new Color(255, 200, 0),
                Title = "Currency - Help",
                Description = "Commands and Information around mangcoins",
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder{Name = "!Currency", Value = "Returns how many mangcoins you have."},
                    new EmbedFieldBuilder{Name = "!Currency Create", Value = "If not already existing, you will get a new account."},
                    new EmbedFieldBuilder{Name = "!Currency Transfer {coins} {username}", Value = "Transfers mangcoins from your account to the given users account."},
                    new EmbedFieldBuilder{Name = "!Currency Reward {coins} {username}", Value = "ADMIN ONLY: reward a given user with a given number of coins."},
                    new EmbedFieldBuilder{Name = "Earning Mangcoins", Value = "Chatting in text chats this bot has access to will earn Mangcoins. Using This bots commands will not earn coins."},
                }
            };

            await ReplyAsync("", false, embedBuilder.Build());
        }

        [Command("create")]
        public async Task Create()
        {
            var createResponse = _currencyService.CreateAccount(Context.User.Id);

            if (createResponse.IsSuccess)
            {
                await ReplyAsync($"Account created successfully for {Context.User.Username}. You have a starting coin balance of {createResponse.Payload}");
            }
            else
            {
                await ReplyAsync($"An error coccured: Does your account already exist? Check your balance with _!Currency_");
            }
        }

        [Command("transfer")]
        public async Task Transfer(int coins, string user)
        {
            var users = await Context.Guild.GetUsersAsync().FlattenAsync();
            var userToSendTo = users.FirstOrDefault(e => $"{e.Username}#{e.Discriminator}" == user);

            if (userToSendTo != null && _currencyService.GetCoins(Context.User.Id).Payload > coins 
                && _currencyService.WithdrawCoins(Context.User.Id, coins).IsSuccess 
                && _currencyService.DepositCoins(userToSendTo.Id, coins).IsSuccess)
            {
                await ReplyAsync($"Succesfully transferred {coins} Mangcoins to {user}");
            }
            else
            {
                await ReplyAsync($"Transfer Failed. Please ensure you have the necessary Mangcoins and that the specified user exists and has a mangcoins account.");
            }
        }

        [Command("reward")]
        public async Task Reward(int coins, string user)
        {
            
            var users = await Context.Guild.GetUsersAsync().FlattenAsync();
            var userToSendTo = users.FirstOrDefault(e => $"{e.Username}#{e.Discriminator}" == user);

            if ((Context.User as IGuildUser).GuildPermissions.Administrator
                && userToSendTo != null
                && _currencyService.DepositCoins(userToSendTo.Id, coins).IsSuccess)
            {
                await ReplyAsync($"Succesfully rewarded {coins} Mangcoins to {user}");
            }
            else
            {
                await ReplyAsync($"Transfer Failed. Please ensure you are an admin, that the specified user exists and has a mangcoins account.");
            }
        }
    }
}
