using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardCollectiveBot.Console.Module
{
    public class Info : ModuleBase<SocketCommandContext>
    {
        [Command("about")]
        [Summary("Describes the Bots purpose.")]
        public async Task AboutAsync()
            => await ReplyAsync("I am a collection of various card games for various numbers of players. Type !help for a list of games");

        [Command("help")]
        [Summary("Lists all base commands accompanied with descriptions")]
        public async Task HelpAsync()
        {
            var embedBuilder = new EmbedBuilder
            {
                Color = new Color(255, 200, 0),
                Title = "Help",
                Description = "Commands and Information about the bot",
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder{Name = "!Currency Help", Value = "A help section for currency related commands. Currency is required for most bot games."},
                    new EmbedFieldBuilder{Name = "!Blackjack Help", Value = "A help section for Blackjack related commands."},
                }
            };

            await ReplyAsync("", false, embedBuilder.Build());
        }
    }
}
