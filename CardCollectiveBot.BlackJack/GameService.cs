using CardCollectiveBot.Common.Responses;
using CardCollectiveBot.DeckOfCards;
using Discord;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public class GameService : IGameService
    {

        public GameService()
        {

        }

        public IResponse CreateGame(IGuildUser player)
        {
            if (GetMatch(player.GuildId) != null)
            {
                return new Response ("Game already created! Use !Blackjack Reset to start a new match or !BlackJack Reset to delete all data for server.", false);
            }

            var match = new BlackJackMatch
            {
                GuildId = player.GuildId,
                Deck = DeckCreation.GenerateDeckStack()
            };

            SaveMatch(match);

            return new Response("New Game created!");

        }

        public IResponse JoinGame(IGuildUser player)
        {
            var match = GetMatch(player.GuildId);

            if ( match == null)
            {
                return new Response("Game does not exist! Use !Blackjack Create to start a new match", false);
            }

            if(match.DealersHand != null && match.DealersHand.Count > 0)
            {
                return new Response("Game is in progress! Please wait for it to finish or type !Blackjack Reset to start over.", false);
            }


            if(match.Players.Any(e => e.Id == player.Id))
            {
                return new Response("You are already a part of this game! Please use !Blackjack Start to begin the match", false);
            }

            match.Players.Add(new Player(player.Id, player.Nickname));

            SaveMatch(match);

            return new Response($"{player.Nickname} has been added to the list of players!");
        }

        public IResponse StartGame(IGuildUser player)
        {
            var match = GetMatch(player.GuildId);

            foreach (var matchPlayer in match.Players)
            {
                matchPlayer.Hand.Add(match.Deck.Pop());                
            }

            match.DealersHand.Add(match.Deck.Pop());

            foreach (var matchPlayer in match.Players)
            {
                matchPlayer.Hand.Add(match.Deck.Pop());
            }

            SaveMatch(match);

            var responseText = new StringBuilder();
            responseText.AppendLine("Current Cards in play:");
            responseText.Append("Dealer: ");
            match.DealersHand.ForEach(e => responseText.Append(e.ToString()));
            responseText.AppendLine();
            foreach(var matchPlayer in match.Players)
            {
                responseText.Append($"{matchPlayer.Nickname}: ");
                matchPlayer.Hand.ForEach(e => responseText.Append($"{e}    "));
                responseText.AppendLine();
            }

            return new Response(responseText.ToString());
        }

        public IResponse Hit(IGuildUser player)
        {
            var match = GetMatch(player.GuildId);

            if (match == null)
            {
                return new Response("Game does not exist! Use !Blackjack Create to start a new match", false);
            }

            var matchPlayer = match?.Players?.FirstOrDefault(p => p.Id == player.Id);

            if (matchPlayer == null)
            {
                return new Response("You are not a part of this game. Please wait for it to end or use !Blackjack Reset to start a new game", false);
            }

            var newCard = match.Deck.Pop();

            matchPlayer.Hand.Add(newCard);

            Response response;

            if(matchPlayer.CountScore() > 21)
            {
                matchPlayer.State = PlayerState.Bust;
                response = new Response($"{matchPlayer.Nickname} has called Hit and received {newCard}. {matchPlayer.Nickname} IS NOW BUST WITH TOTAL SCORE OF {matchPlayer.CountScore()}");
            }
            else
            {
                matchPlayer.State = PlayerState.Hit;
                response = new Response($"{matchPlayer.Nickname} has called Hit and received {newCard}.");
            }

            SaveMatch(match);

            return response;
        }

        public IResponse Stand(IGuildUser player)
        {
            var match = GetMatch(player.GuildId);

            if (match == null)
            {
                return new Response("Game does not exist! Use !Blackjack Create to start a new match", false);
            }

            var matchPlayer = match?.Players?.FirstOrDefault(p => p.Id == player.Id);

            if (matchPlayer == null)
            {
                return new Response("You are not a part of this game. Please wait for it to end or use !Blackjack Reset to start a new game", false);
            }

            matchPlayer.State = PlayerState.Stand;

            SaveMatch(match);

            return new Response($"{matchPlayer.Nickname} has called Stand and has a total score of {matchPlayer.CountScore()}");
        }

        public IResponse ResetGame(IGuildUser player)
        {
            if (GetMatch(player.GuildId) == null)
            {
                return new Response("Game does not exist! Use !Blackjack Create to create a match.", false);
            }

            var match = new BlackJackMatch
            {
                GuildId = player.GuildId,
                Deck = DeckCreation.GenerateDeckStack()
            };

            SaveMatch(match);

            return new Response("New Game created!");
        }

        public IResponse DeleteGame(IGuildUser player)
        {
            if (GetMatch(player.GuildId) == null)
            {
                return new Response("Game does not exist! Use !Blackjack Create to create a match.", false);
            }
            
            File.Delete(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.BlackJack\Matches\{player.GuildId}.json"));

            return new Response("Save Data Deleted!");
        }

        private BlackJackMatch GetMatch(ulong guildId)
        {
            if (File.Exists(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.BlackJack\Matches\{guildId}.json"))
            {
                using (StreamReader r = new StreamReader(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.BlackJack\Matches\{guildId}.json"))
                {
                    string json = r.ReadToEnd();
                    var item = JsonConvert.DeserializeObject<BlackJackMatch>(json);

                    return item;
                }
            }

            return null;
        }

        private bool SaveMatch(BlackJackMatch match)
        {
            try {
                var jsonItem = JsonConvert.SerializeObject(match);
                File.WriteAllText(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.BlackJack\Matches\{match.GuildId}.json", jsonItem);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
