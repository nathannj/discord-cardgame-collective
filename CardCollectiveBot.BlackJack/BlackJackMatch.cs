using CardCollectiveBot.Common.Responses;
using CardCollectiveBot.DeckOfCards;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public class BlackJackMatch
    {
        public ulong GuildId { get; set; }

        public Stack<PlayingCard> Deck { get; set; }

        public Player Dealer { get; set; }

        public PlayingCard FaceDownCard { get; set; }

        public List<Player> Players { get; set; }

        public EmbedBuilder Scoreboard { get; set; }

        public BlackJackMatch(ulong guildId)
        {
            GuildId = guildId;
            Deck = DeckCreation.GenerateDeckStack();
            Players = new List<Player>();
            Dealer = new Player(0, "Dealer", 0);
            Scoreboard = new EmbedBuilder { Title = $"Blackjack - {nameof(Scoreboard)}", Color = Color.DarkerGrey };
        }

        public EmbedBuilder RefreshScoreboard(bool isFinal = false)
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

            return Scoreboard;
        }

        public string PlayerSummary(Player player, bool isFinal)
        {
            var lastField = isFinal ? $"Result: {ResultToString(player)}" : $"State:{player.State}";
            return $"**Cards:** {string.Join(",", player.Hand)}\n**Score: **{player.CountScore()}\n{lastField}";
        }

        public string DealerSummary(Player player)
        {
            return $"**Cards:** {string.Join(",", player.Hand)}\n**Score: **{player.CountScore()}";
        }

        public IEnumerable<Reward> GetRewards()
        {
            var rewards = new List<Reward>();

            Players.ForEach(player =>
            {
                switch(Result(player))
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
            if (player.State == PlayerState.Bust || (Dealer.State != PlayerState.Bust && Dealer.CountScore() >= player.CountScore()))
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
