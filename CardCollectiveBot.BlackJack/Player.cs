using CardCollectiveBot.DeckOfCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public class Player
    {
        public ulong Id { get; set; }

        public string Nickname { get; set; }

        public List<PlayingCard> Hand { get; set; }

        public PlayerState State { get; set; }

        public Player(ulong id, string nickname)
        {
            Id = id;
            Nickname = nickname;
            Hand = new List<PlayingCard>();
            State = PlayerState.Choosing;
        }

        public int CountScore()
        {
            return Hand?.Sum(e => e.TrueValue()) ?? 0;
        }
    }
}
