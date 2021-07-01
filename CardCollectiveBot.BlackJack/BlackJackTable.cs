using CardCollectiveBot.Common.Responses;
using CardCollectiveBot.DeckOfCards;
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public class BlackJackTable
    {
        public ulong GuildId { get; private set; }

        public ulong ChannelId { get; private set; }

        public Stack<PlayingCard> Deck { get; private set; }

        public Player Dealer { get; private set; }

        public PlayingCard FaceDownCard { get; private set; }

        public List<Player> Players { get; private set; }

        public EmbedBuilder Scoreboard { get; private set; }

        public BlackJackTable(ulong guildId, ulong channelId)
        {
            GuildId = guildId;
            ChannelId = channelId;
            Deck = DeckCreation.GenerateDeckStack();
            Players = new List<Player>();
            Dealer = new Player(0, "Dealer", 0);
            Scoreboard = new EmbedBuilder { Title = $"Blackjack - {nameof(Scoreboard)}", Color = Color.DarkerGrey };
        }

        [JsonConstructor]
        public BlackJackTable(ulong guildId, ulong channelId, Stack<PlayingCard> deck, Player dealer, PlayingCard faceDownCard, List<Player> players, EmbedBuilder scoreboard)
        {
            GuildId = guildId;
            ChannelId = channelId;
            Deck = deck;
            Dealer = dealer;
            FaceDownCard = faceDownCard;
            Players = players;
            Scoreboard = scoreboard;
        }

        public List<Player> AddPlayerToMatch(Player player)
        {
            Players.Add(player);

            return Players;
        }

        public BlackJackTable SetUpInitialTable()
        {
            foreach (var matchPlayer in Players)
            {
                matchPlayer.Hit(Deck.Pop());
            }

            Dealer.Hit(Deck.Pop());

            foreach (var matchPlayer in Players)
            {
                matchPlayer.Hit(Deck.Pop());
            }

            FaceDownCard = Deck.Pop();

            var dealerPeek = new Player(0, "Dealer", 0);

            var dealerPeekHand = Dealer.Hand.ToList();
            dealerPeekHand.Add(FaceDownCard);

            foreach (var card in dealerPeekHand)
            {
                dealerPeek.Hit(card);
            }

            RefreshScoreboard("Game has now began!");

            return dealerPeek.State == PlayerState.BlackJack ? FinishMatch() : this;
        }

        public bool IsEndOfMatch => Players.All(e => e.State != PlayerState.Choosing);

        public bool HasTableBeganMatch => Dealer?.Hand != null && Dealer.Hand.Count > 0;

        public bool IsPlayerInGame(ulong playerId) => Players.FirstOrDefault(e => e.Id == playerId) != null;

        public Player HitPlayer(ulong playerId)
        {
            var player = Players.FirstOrDefault(e => e.Id == playerId);

            if (player != null)
            {
                var newCard = Deck.Pop();
                player.Hit(newCard);

                if (player.State == PlayerState.Bust)
                {
                    RefreshScoreboard($"{player.Nickname} has called Hit and received {newCard}. {player.Nickname} HAS WENT BUST");
                }
                else
                {
                    RefreshScoreboard($"{player.Nickname} has called Hit and received {newCard}.");
                }
            }

            return player;
        }

        public Player StandPlayer(ulong playerId)
        {
            var player = Players.FirstOrDefault(e => e.Id == playerId);

            if (player != null)
            {
                player.Stand();
                RefreshScoreboard($"{player.Nickname} has called Stand.");
            }

            return player;
        }

        public BlackJackTable FinishMatch()
        {
            Players.ForEach(e =>
            {
                if (e.State == PlayerState.Choosing)
                {
                    e.Stand();
                }
            });

            Dealer.Hit(FaceDownCard);

            while (Dealer.CountScore() < 17 && Dealer.State != PlayerState.Bust)
            {
                Dealer.Hit(Deck.Pop());
            }

            RefreshScoreboard($"{Scoreboard.Footer?.Text} game has ended! Rewards distributed and new game has been set up", true);

            return this;
        }

        public EmbedBuilder RefreshScoreboard(string footer = null, bool isFinal = false)
        {
            if (Scoreboard.Fields.Exists(e => e.Name == nameof(Dealer)))
            {
                var dealerField = Scoreboard.Fields.Find(e => e.Name == nameof(Dealer));
                dealerField.Value = DealerSummary(Dealer);
            }
            else
            {
                var dealerField = Scoreboard.AddField(nameof(Dealer), DealerSummary(Dealer));
            }

            foreach (var player in Players)
            {
                if (Scoreboard.Fields.Exists(e => e.Name == player.Nickname))
                {
                    var field = Scoreboard.Fields.Find(e => e.Name == player.Nickname);
                    field.Value = PlayerSummary(player, isFinal);
                }
                else
                {
                    var dealerField = Scoreboard.AddField(player.Nickname, PlayerSummary(player, isFinal));
                }
            }

            Scoreboard.WithFooter(footer);

            return Scoreboard;
        }

        public IEnumerable<Reward> GetWagersAsRewards()
        {
            return Players.Select(player => new Reward(player.Id, player.Nickname, player.Wager));
        }

        public IEnumerable<Reward> GetRewards()
        {
            var rewards = new List<Reward>();

            Players.ForEach(player =>
            {
                switch (Result(player))
                {
                    case ResultEnum.Win:
                        rewards.Add(new Reward(player.Id, player.Nickname, player.Wager * 2));
                        break;
                    case ResultEnum.Tie:
                        rewards.Add(new Reward(player.Id, player.Nickname, player.Wager));
                        break;
                }
            });

            return rewards;
        }

        private string PlayerSummary(Player player, bool isFinal)
        {
            var lastField = isFinal ? $"Result: {ResultToString(player)}" : $"State:{player.State}";
            return $"**Cards:** {string.Join(",", player.Hand)}\n**Score: **{player.CountScore()}\n{lastField}";
        }

        private string DealerSummary(Player player)
        {
            return $"**Cards:** {string.Join(",", player.Hand)}\n**Score: **{player.CountScore()}";
        }

        private string ResultToString(Player player)
        {
            if (player.State == PlayerState.BlackJack && Dealer.State != PlayerState.BlackJack)
            {
                return "BLACKJACK WIN! VEGAS BABY, YEAHHH!";
            }

            switch (Result(player))
            {
                case ResultEnum.Win:
                    return "WIN";
                case ResultEnum.Loss:
                    return "LOSS";
                case ResultEnum.Tie:
                    return "TIE";
                default:
                    return "Can't find result";
            }
        }

        private ResultEnum Result(Player player)
        {
            if (player.State == PlayerState.Bust || (Dealer.State != PlayerState.Bust && Dealer.CountScore() > player.CountScore()))
            {
                return ResultEnum.Loss;
            }
            else if (Dealer.State != PlayerState.Bust && Dealer.CountScore() >= player.CountScore())
            {
                return ResultEnum.Tie;
            }
            else
            {
                return ResultEnum.Win;
            }
        }
    }
}
