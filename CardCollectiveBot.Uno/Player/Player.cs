using CardCollectiveBot.Uno.Cards;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardCollectiveBot.Uno
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

        public bool Uno { get; private set; }

        public int Score { get; set; }

        public int HandScore() => ActualHand.Sum(e => e.CardScore());

        public Player(ulong id, string nickname, int wager)
        {
            Id = id;
            Nickname = nickname;
            ActualHand = new List<PlayingCard>();
        }

        [JsonConstructor]
        public Player(ulong id, string nickname, int wager, List<PlayingCard> hand)
        {
            Id = id;
            Nickname = nickname;
            ActualHand = hand;
        }
        
        public void EmptyHand()
        {
            ActualHand = new List<PlayingCard>();
        }

        public void Draw(PlayingCard card)
        {
            ActualHand.Add(card);
        }

        public bool CallUno()
        {
            if(Hand.Count == 1) {
                Uno = true;
            }

            return Uno;
        }

        public PlayingCard TakeCard(int card, PlayingCard cardInPlay)
        {
            if(ActualHand.Count < card)
            {
                return null;
            }

            var cardToBePlayed = ActualHand[card];           

            //TODO rules for card comparisons

            ActualHand.Remove(cardToBePlayed);

            return cardToBePlayed;
        }
    }
}
