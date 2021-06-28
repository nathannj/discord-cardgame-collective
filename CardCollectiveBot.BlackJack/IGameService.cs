using CardCollectiveBot.Common.Responses;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public interface IGameService
    {
        public IResponse<EmbedBuilder> CreateGame(IGuildUser player);

        public IResponse<EmbedBuilder> JoinGame(IGuildUser player, int wager);

        public IResponse<EmbedBuilder> StartGame(IGuildUser player);

        public IResponse<EmbedBuilder> Hit(IGuildUser player);

        public IResponse<EmbedBuilder> Stand(IGuildUser player);

        public IResponse<EmbedBuilder> ResetGame(ulong guildId, bool shouldRefund = false);

        public IResponse<EmbedBuilder> DeleteGame(IGuildUser player);
    }
}
