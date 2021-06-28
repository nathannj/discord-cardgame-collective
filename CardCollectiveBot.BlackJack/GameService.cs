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

        public IResponse<EmbedBuilder> CreateGame(IGuildUser player)
        {
            if (GetMatch(player.GuildId) != null)
            {
                return new BlackjackResponse(null, false, null, "Game already created! Use !Blackjack Reset to start a new match or !BlackJack Delete to delete all data for server.");
            }

            var match = new BlackJackMatch(player.GuildId);

            SaveMatch(match);

            return new BlackjackResponse(null, true, null, "New Game created!");

        }

        public IResponse<EmbedBuilder> JoinGame(IGuildUser player, int wager)
        {
            var match = GetMatch(player.GuildId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            if (match?.Dealer?.Hand != null && match.Dealer.Hand.Count > 0)
            {
                return new BlackjackResponse(null, false, null, "Game is in progress! Please wait for it to finish or type !Blackjack Reset to start over.");
            }


            if (match.Players.Any(e => e.Id == player.Id))
            {
                return new BlackjackResponse(null, false, null, "You are already a part of this game! Please use !Blackjack Start to begin the match");
            }

            match.Players.Add(new Player(player.Id, player.Nickname ?? player.Username, wager));

            SaveMatch(match);

            return new BlackjackResponse(null, true, null, $"{player.Nickname} has joined the lobby with a wager of {wager}");
        }

        public IResponse<EmbedBuilder> StartGame(IGuildUser player)
        {
            var match = GetMatch(player.GuildId);

            foreach (var matchPlayer in match.Players)
            {
                matchPlayer.TakeCard(match.Deck.Pop());
            }

            match.Dealer.TakeCard(match.Deck.Pop());

            foreach (var matchPlayer in match.Players)
            {
                matchPlayer.TakeCard(match.Deck.Pop());
            }

            match.FaceDownCard = match.Deck.Pop();

            var dealerPeek = new Player(0, "Dealer", 0);

            var dealerPeekHand = match.Dealer.Hand.ToList();
            dealerPeekHand.Add(match.FaceDownCard);

            foreach (var card in dealerPeekHand)
            {
                dealerPeek.TakeCard(card);
            }

            string message;

            if (dealerPeek.State == PlayerState.BlackJack)
            {
                return FinishMatch(match);
                return new BlackjackResponse(match.RefreshScoreboard(true), otherMessages: message);
            }

            return NextRound(match);
        }

        public IResponse<EmbedBuilder> Hit(IGuildUser player)
        {
            var match = GetMatch(player.GuildId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            var matchPlayer = match?.Players?.FirstOrDefault(p => p.Id == player.Id);

            if (matchPlayer == null)
            {
                return new BlackjackResponse(null, false, null, "You are not a part of this game. Please wait for it to end or use !Blackjack Reset to start a new game");
            }

            if (matchPlayer.State != PlayerState.Choosing)
            {
                return new BlackjackResponse(null, false, null, "You have either already hit this round or have finished calls for the match. Please wait for other players to finish.");
            }

            var newCard = match.Deck.Pop();

            matchPlayer.TakeCard(newCard);

            if (matchPlayer.State == PlayerState.Bust)
            {
                match.Scoreboard.WithFooter($"{matchPlayer.Nickname} has called Hit and received {newCard}. {matchPlayer.Nickname} HAS WENT BUST");
            }
            else
            {
                match.Scoreboard.WithFooter($"{matchPlayer.Nickname} has called Hit and received {newCard}.");
            }

            return PostChoiceCheck(match);
        }

        public IResponse<EmbedBuilder> Stand(IGuildUser player)
        {
            var match = GetMatch(player.GuildId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            var matchPlayer = match?.Players?.FirstOrDefault(p => p.Id == player.Id);

            if (matchPlayer == null)
            {
                return new BlackjackResponse(null, false, null, "You are not a part of this game. Please wait for it to end or use !Blackjack Reset to start a new game");
            }

            matchPlayer.Stand();

            return PostChoiceCheck(match);
        }

        public IResponse<EmbedBuilder> ResetGame(ulong guildId, bool shouldRefund = false)
        {
            var match = GetMatch(guildId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to create a match.");
            }

            var refunds = match.Players.Select(e => new Reward(e.Id, e.Nickname, e.Wager)).ToList();

            match = new BlackJackMatch(guildId);

            SaveMatch(match);

            return new BlackjackResponse(null, true, refunds, "New Game created!");
        }

        public IResponse<EmbedBuilder> DeleteGame(IGuildUser player)
        {
            if (GetMatch(player.GuildId) == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to create a match.");
            }

            File.Delete(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.BlackJack\Matches\{player.GuildId}.json");

            return new BlackjackResponse(null, true, null, "Save Data Deleted!");
        }

        private IResponse<EmbedBuilder> PostChoiceCheck(BlackJackMatch match)
        {
            if (match.Players.Any(e => e.State == PlayerState.Choosing))
            {
                SaveMatch(match);
                match.Scoreboard.WithFooter($"{match.Scoreboard.Footer?.Text} Round still in progress!");
                match.RefreshScoreboard();
                return new BlackjackResponse(match.Scoreboard, true, null);
            }
            else if (match.Players.All(e => e.State != PlayerState.Choosing && e.State != PlayerState.Hit))
            {
                return FinishMatch(match);
            }
            else
            {
                return NextRound(match);
            }
        }

        private IResponse<EmbedBuilder> NextRound(BlackJackMatch match)
        {
            foreach (var player in match.Players.Where(e => e.State == PlayerState.Hit))
            {
                player.ResetState();
            }

            SaveMatch(match);
            match.Scoreboard.Footer = new EmbedFooterBuilder { Text = $"{match.Scoreboard.Footer?.Text} Round over! Next round commencing..." };
            match.RefreshScoreboard();
            return new BlackjackResponse(match.Scoreboard, true, null);
        }

        private IResponse<EmbedBuilder> FinishMatch(BlackJackMatch match)
        {
            match.Dealer.TakeCard(match.FaceDownCard);

            while (match.Dealer.CountScore() < 17 && match.Dealer.State != PlayerState.Bust)
            {
                match.Dealer.TakeCard(match.Deck.Pop());
            }

            match.RefreshScoreboard(true);

            ResetGame(match.GuildId).OtherMessages.First();

            match.Scoreboard.WithFooter($"{match.Scoreboard.Footer?.Text} game has ended! Rewards distributed and new game has been set up");

            return new BlackjackResponse(match.Scoreboard, true, match.GetRewards().ToList());
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
            try
            {
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
