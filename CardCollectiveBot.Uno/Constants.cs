using CardCollectiveBot.Uno.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.Uno
{
    public static class Constants
    {
        public static class Emojis
        {
            public const string WildCard = "<:wild:867106515332628530>";
            public const string Blue = "<:blue:867106515285704744>";
            public const string Red = "<:red:867106515143884821>";
            public const string Green = "<:green:867106515277709332>";
            public const string Yellow = "<:yellow:867106515273252885>";

            public static Dictionary<ColourEnum, string> EnumEmojiDictionary = new Dictionary<ColourEnum, string>
            {
                { ColourEnum.Blue, Blue},
                { ColourEnum.Red, Red},
                { ColourEnum.Green, Green},
                { ColourEnum.Yellow, Yellow},
                { ColourEnum.WildCard, WildCard},
            };
        }

        public static Dictionary<ActionEnum, string> ActionEnumStringDictionary = new Dictionary<ActionEnum, string>
            {
                { ActionEnum.PlusTwo, "Draw Two"},
                { ActionEnum.Reverse, "Uno Reverse"},
                { ActionEnum.Skip, "Skip"},
                { ActionEnum.Wild, "Wild Card"},
                { ActionEnum.WildDrawFour, "Draw Four"},
            };
    }
}
