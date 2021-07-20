using System;

namespace CardCollectiveBot.Uno.Cards
{
    public class PlayingCard
    {
        public ColourEnum Colour { get; }

        public int? Value { get; }

        public ActionEnum Action { get; }

        public PlayingCard(ColourEnum suit, int? value, ActionEnum action = ActionEnum.None)
        {
            Colour = suit;

            Value = Value == null || value <= 9 && value >= 0 ? value
                : throw new InvalidCastException("Value must be bteween 0 and 9");

            Action = action;
        }

        public override string ToString()
        {
            return $"{Value.ToString() ?? Constants.ActionEnumStringDictionary[Action]} {Constants.Emojis.EnumEmojiDictionary[Colour]}";
        }

        public int CardScore()
        {
            if(Value != null)
            {
                return Value.Value;
            }

            switch(Action)
            {
                case ActionEnum.PlusTwo:
                case ActionEnum.Reverse:
                case ActionEnum.Skip:
                    return 20;
                case ActionEnum.Wild:
                case ActionEnum.WildDrawFour:
                    return 50;
                default:
                    return 0;
            }
        }
    }
}
