using System;
using System.Collections.Generic;

namespace CardCollectiveBot.BlackJack.Cards
{
    public static class DeckCreation
    {
        public static Stack<PlayingCard> GenerateDeckStack()
        {
            var list = GenerateDeck();

            list.Shuffle();

            return new Stack<PlayingCard>(list);
        }

        public static List<PlayingCard> GenerateDeck()
        {
            var list = new List<PlayingCard>();
            foreach (SuitEnum suit in (SuitEnum[])Enum.GetValues(typeof(SuitEnum)))
            {
                for (int i = 2; i < 14; i++)
                {
                    list.Add(new PlayingCard(suit, i));
                }
            }

            return list;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
