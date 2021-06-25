using CardCollectiveBot.DeckOfCards;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public class BlackJackMatch
    {
        public ulong GuildId { get; set; }

        public Stack<PlayingCard> Deck { get; set; }

        public List<PlayingCard> DealersHand { get; set; }

        public List<Player> Players { get; set; }
    }
}
