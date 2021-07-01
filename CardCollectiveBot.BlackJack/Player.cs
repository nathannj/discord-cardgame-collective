using CardCollectiveBot.DeckOfCards;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public class Player
    {
        public ulong Id { get; private set; }

        public string Nickname { get; private set; }

        [JsonProperty]
        public ReadOnlyCollection<PlayingCard> Hand
        {
            get { return ActualHand?.AsReadOnly(); }
        }

        private List<PlayingCard> ActualHand { get; set; }

        public PlayerState State { get; private set; }

        public int Wager { get; }

        public Player(ulong id, string nickname, int wager, PlayerState state = PlayerState.Choosing)
        {
            Id = id;
            Nickname = nickname;
            ActualHand = new List<PlayingCard>();
            State = state;
            Wager = wager;
        }

        [JsonConstructor]
        public Player(ulong id, string nickname, int wager, List<PlayingCard> hand, PlayerState state)
        {
            Id = id;
            Nickname = nickname;
            ActualHand = hand;
            State = state;
            Wager = wager;
        }

        public int CountScore()
        {
            var orderedHand = ActualHand.OrderBy(e => e.TrueValue()).ToList();

            var total = 0;

            foreach(var card in orderedHand)
            {
                if (card.TrueValue() == 11 && total + card.TrueValue() > 21)
                {
                    total += 1;
                }
                else
                {
                    total += card.TrueValue();
                }
            }

            return total;
        }

        public List<PlayingCard> Hit(PlayingCard card)
        {
            ActualHand.Add(card);

            if(CountScore() > 21)
            {
                State = PlayerState.Bust;
            }
            else if (Hand.Count == 2 && CountScore() == 21)
            {
                State = PlayerState.BlackJack;
            }

            return ActualHand;
        }

        public void Stand()
        {
            State = PlayerState.Stand;
        }

        public void ResetState()
        {
            State = PlayerState.Choosing;
        }
    }
}
