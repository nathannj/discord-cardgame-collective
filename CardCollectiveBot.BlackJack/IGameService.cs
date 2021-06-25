using CardCollectiveBot.Common.Responses;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public interface IGameService
    {
        public IResponse CreateGame(IGuildUser player);

        public IResponse JoinGame(IGuildUser player);

        public IResponse StartGame(IGuildUser player);

        public IResponse Hit(IGuildUser player);

        public IResponse Stand(IGuildUser player);

        public IResponse ResetGame(IGuildUser player);

        public IResponse DeleteGame(IGuildUser player);
    }
}
