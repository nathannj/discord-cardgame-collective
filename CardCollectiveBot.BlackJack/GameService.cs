using CardCollectiveBot.Common.Responses;
using Discord;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace CardCollectiveBot.BlackJack
{
    public class GameService : IGameService
    {

        public GameService()
        {

        }

        public IResponse<EmbedBuilder> CreateGame(IGuildUser player, ulong channelId)
        {
            if (GetMatch(player.GuildId, channelId) != null)
            {
                return new BlackjackResponse(null, false, null, "Game already created! Use !Blackjack Reset to start a new match or !BlackJack Delete to delete all data for server.");
            }

            var match = new BlackJackTable(player.GuildId, channelId);

            SaveMatch(match);

            return new BlackjackResponse(null, true, null, "New Game created!");

        }

        public IResponse<EmbedBuilder> JoinGame(IGuildUser player, ulong channelId, int wager)
        {
            var match = GetMatch(player.GuildId, channelId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            if (match.HasTableBeganMatch)
            {
                return new BlackjackResponse(null, false, null, "Game is in progress! Please wait for it to finish or type !Blackjack Reset to start over.");
            }


            if (match.IsPlayerInGame(player.Id))
            {
                return new BlackjackResponse(null, false, null, "You are already a part of this game! Please use !Blackjack Start to begin the match");
            }

            match.AddPlayerToMatch(new Player(player.Id, player.Nickname ?? player.Username, wager));

            SaveMatch(match);

            return new BlackjackResponse(null, true, null, $"{player.Nickname} has joined the lobby with a wager of {wager}");
        }

        public IResponse<EmbedBuilder> StartGame(IGuildUser player, ulong channelId)
        {
            var match = GetMatch(player.GuildId, channelId).SetUpInitialTable();

            SaveMatch(match);

            return new BlackjackResponse(match.Scoreboard, true, match.IsEndOfMatch ? match.GetRewards().ToList() : null);
        }

        public IResponse<int> GetPlayerWager(IGuildUser player, ulong channelId)
        {
            var match = GetMatch(player.GuildId, channelId);

            if (match == null)
            {
                return new Response<int>(0, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            var matchPlayer = match.Players.FirstOrDefault(e => e.Id == player.Id); ;

            if (matchPlayer == null)
            {
                return new Response<int>(0, false, null, "Given player is not a part of this match");
            }

            SaveMatch(match);

            return new Response<int>(matchPlayer.Wager);
        }

        public IResponse<EmbedBuilder> DoubleDown(IGuildUser player, ulong channelId)
        {
            var match = GetMatch(player.GuildId, channelId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            var matchPlayer = match.DoubleDown(player.Id);

            if (matchPlayer == null)
            {
                return new BlackjackResponse(null, false, null, "You are not a part of this game. Please wait for it to end or use !Blackjack Reset to start a new game");
            }

            SaveMatch(match);

            return match.IsEndOfMatch ? FinishMatch(match) : new BlackjackResponse(match.Scoreboard);
        }

        public IResponse<EmbedBuilder> SplitPair(IGuildUser player, ulong channelId)
        {
            var match = GetMatch(player.GuildId, channelId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            var splitPairSuccess = match.SplitPair(player.Id);

            if (splitPairSuccess == false)
            {
                return new BlackjackResponse(null, false, null, "You can not currently Split Pairs");
            }

            SaveMatch(match);

            return match.IsEndOfMatch ? FinishMatch(match) : new BlackjackResponse(match.Scoreboard);
        }

        public IResponse<EmbedBuilder> Hit(IGuildUser player, ulong channelId)
        {
            var match = GetMatch(player.GuildId, channelId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            var matchPlayer = match.HitPlayer(player.Id);

            if (matchPlayer == null)
            {
                return new BlackjackResponse(null, false, null, "You are not a part of this game. Please wait for it to end or use !Blackjack Reset to start a new game");
            }

            SaveMatch(match);

            return match.IsEndOfMatch ? FinishMatch(match) : new BlackjackResponse(match.Scoreboard);
        }

        public IResponse<EmbedBuilder> Stand(IGuildUser player, ulong channelId)
        {
            var match = GetMatch(player.GuildId, channelId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            var matchPlayer = match.StandPlayer(player.Id);

            if (matchPlayer == null)
            {
                return new BlackjackResponse(null, false, null, "You are not a part of this game. Please wait for it to end or use !Blackjack Reset to start a new game");
            }

            SaveMatch(match);

            return match.IsEndOfMatch ? FinishMatch(match) : new BlackjackResponse(match.Scoreboard);
        }

        public IResponse<EmbedBuilder> GetLobby(IGuildUser player, ulong channelId)
        {
            var match = GetMatch(player.GuildId, channelId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to start a new match");
            }

            var responseEmbed = new EmbedBuilder 
            { 
                Title = "Blackjack - Lobby",
                Color = Color.DarkerGrey,
                Footer = new EmbedFooterBuilder { Text = ""}
            };          
            
            foreach(var matchPlayer in match.Players)
            {
                responseEmbed.AddField("Name", matchPlayer.Nickname, true);
                responseEmbed.AddField("Wager", matchPlayer.Wager, true);
            }
            
            return new BlackjackResponse(responseEmbed);
        }

        public IResponse<EmbedBuilder> ResetGame(ulong guildId, ulong channelId, bool shouldRefund = false)
        {
            var match = GetMatch(guildId, channelId);

            if (match == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to create a match.");
            }

            var refunds = match.GetWagersAsRewards();

            match = new BlackJackTable(guildId, channelId);

            SaveMatch(match);

            return new BlackjackResponse(null, true, shouldRefund ? refunds.ToList() : null, "New Game created!");
        }

        public IResponse<EmbedBuilder> DeleteGame(IGuildUser player, ulong channelId)
        {
            if (GetMatch(player.GuildId, channelId) == null)
            {
                return new BlackjackResponse(null, false, null, "Game does not exist! Use !Blackjack Create to create a match.");
            }

            File.Delete(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.BlackJack\Matches\{player.GuildId}.json");

            return new BlackjackResponse(null, true, null, "Save Data Deleted!");
        }

        private IResponse<EmbedBuilder> FinishMatch(BlackJackTable match)
        {
            match.FinishMatch();

            ResetGame(match.GuildId, match.ChannelId);

            return new BlackjackResponse(match.Scoreboard, true, match.GetRewards().ToList());
        }

        private BlackJackTable GetMatch(ulong guildId, ulong channelId)
        {
            if (File.Exists(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.BlackJack\Matches\{guildId}_{channelId}.json"))
            {
                using (StreamReader r = new StreamReader(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.BlackJack\Matches\{guildId}_{channelId}.json"))
                {
                    string json = r.ReadToEnd();
                    var item = JsonConvert.DeserializeObject<BlackJackTable>(json);

                    return item;
                }
            }

            return null;
        }

        private bool SaveMatch(BlackJackTable match)
        {
            try
            {
                var jsonItem = JsonConvert.SerializeObject(match);
                File.WriteAllText(@$"C:\Users\njack\source\repos\nathannj\discord-cardgame-collective\CardCollectiveBot.BlackJack\Matches\{match.GuildId}_{match.ChannelId}.json", jsonItem);
                return true;
            }
            catch
            {
                return false;
            }

        }        
    }
}