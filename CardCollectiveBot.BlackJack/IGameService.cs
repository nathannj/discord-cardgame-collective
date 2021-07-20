using CardCollectiveBot.Common.Responses;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public interface IGameService
    {
        public IResponse<EmbedBuilder> CreateGame(IGuildUser player, ulong channelId);

        public IResponse<EmbedBuilder> DoubleDown(IGuildUser player, ulong channelId);

        public IResponse<EmbedBuilder> SplitPair(IGuildUser player, ulong channelId);

        public IResponse<EmbedBuilder> JoinGame(IGuildUser player, ulong channelId, int wager);

        public IResponse<EmbedBuilder> StartGame(IGuildUser player, ulong channelId);

        public IResponse<int> GetPlayerWager(IGuildUser player, ulong channelId);

        public IResponse<EmbedBuilder> Hit(IGuildUser player, ulong channelId);

        public IResponse<EmbedBuilder> Stand(IGuildUser player, ulong channelId);

        public IResponse<EmbedBuilder> GetLobby(IGuildUser player, ulong channelId);

        public IResponse<EmbedBuilder> ResetGame(ulong guildId, ulong channelId, bool shouldRefund = false);

        public IResponse<EmbedBuilder> DeleteGame(IGuildUser player, ulong channelId);
    }
}
