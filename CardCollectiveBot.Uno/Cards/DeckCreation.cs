using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CardCollectiveBot.Uno.Cards
{
    public static class DeckCreation
    {
        public static Stack<PlayingCard> GenerateDeckStack()
        {
            var list = GenerateDeck();

            for (int i = 0; i < 4; i++) { list.Shuffle(); }

            return new Stack<PlayingCard>(list);
        }

        public static List<PlayingCard> GenerateDeck()
        {
            if (File.Exists(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.Uno\Cards\UnoDeck.json"))
            {
                using (StreamReader r = new StreamReader(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.Uno\Cards\UnoDeck.json"))
                {
                    string json = r.ReadToEnd();
                    var item = JsonConvert.DeserializeObject<List<PlayingCard>>(json);

                    return item;
                }
            }

            return null;
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
