using CardCollectiveBot.DeckOfCards;
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

        public ReadOnlyCollection<PlayingCard> Hand
        {
            get { return ActualHand.AsReadOnly(); }
        }

        public List<PlayingCard> ActualHand { get; private set; }

        public PlayerState State { get; private set; }

        public Player(ulong id, string nickname)
        {
            Id = id;
            Nickname = nickname;
            ActualHand = new List<PlayingCard>();
            State = PlayerState.Choosing;
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

        public List<PlayingCard> TakeCard(PlayingCard card)
        {
            ActualHand.Add(card);

            if(CountScore() > 21)
            {
                State = PlayerState.Bust;
            }
            else if (CountScore() == 21)
            {
                State = PlayerState.BlackJack;
            }
            else
            {
                State = PlayerState.Hit;
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
