using System;

namespace CardCollectiveBot.DeckOfCards
{
    public class PlayingCard
    {
        public SuitEnum Suit { get; }

        public int Value { get; }



        private string ValueName { get => GetValueName(); }

        public PlayingCard(SuitEnum suit, int value)
        {
            Suit = suit;
            Value = value <= 14 && value > 1 ? value
                : throw new InvalidCastException("Value can not be greater than 14 or less then 2");
        }

        public override string ToString()
        {
            return $"{ValueName} of {Suit}";
        }

        private string GetValueName()
        {
            switch (Value)
            {
                case 11:
                    return "Jack";
                case 12:
                    return "Queen";
                case 13:
                    return "King";
                case 14:
                    return "Ace";
                default: return Value.ToString();
            }
        }

        public int TrueValue()
        {
            switch (Value)
            {
                case 11:
                case 12:
                case 13:
                    return 10;
                case 14:
                    return 11;
                default: return Value;
            }
        }
    }
}
