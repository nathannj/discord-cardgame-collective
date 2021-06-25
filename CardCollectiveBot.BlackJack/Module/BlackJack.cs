using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CardCollectiveBot.BlackJack.Module
{
    public class BlackJack : ModuleBase<SocketCommandContext>
    {
        public BlackJack()
        {

        }

        [Command("shush")]
        public async Task StartBlackJack()
        {
            var player = (Context.User as IGuildUser);
        }
    }
}
