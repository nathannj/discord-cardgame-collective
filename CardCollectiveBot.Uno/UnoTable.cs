using CardCollectiveBot.Uno.Cards;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardCollectiveBot.Uno
{
    public sealed class UnoTable
    {
        public ulong GuildId { get; private set; }

        public ulong ChannelId { get; private set; }

        public Stack<PlayingCard> Deck { get; private set; }

        public Stack<PlayingCard> DiscardPile { get; private set; }

        public List<Player> Players { get; private set; }

        private bool NormalDirection { get; set; }

        private int Wager { get; set; }

        private int PlayerTurnIndex { get; set; }

        public EmbedBuilder StateOfPlay { get; private set; }

        public UnoTable(ulong guildId, ulong channelId, string channelName, int wager)
        {
            GuildId = guildId;
            ChannelId = channelId;
            DiscardPile = new Stack<PlayingCard>();
            Players = new List<Player>();
            StateOfPlay = new EmbedBuilder { Title = $"Uno - The Board for {channelName}", Color = Color.Red };
            PlayerTurnIndex = 0;
            Wager = wager; //TODO have this changeable
            NormalDirection = true;
        }

        public bool CheckPlayersUno(Player player)
        {
            return player.Uno;
        }

        public bool CallUno(Player player)
        {
            return player.CallUno();
        }

        public void EndRound(Player winner)
        {
            winner.Score += Players.Sum(e => e.HandScore());
        }

        public void Setup()
        {
            Deck = DeckCreation.GenerateDeckStack();

            foreach (var player in Players)
            {
                player.EmptyHand();

                for (int i = 0; i < 7; i++)
                {
                    player.Draw(Deck.Pop());
                }
            }

            DiscardPile.Push(Deck.Pop());

            UpdateStateOfPlay();
        }

        public EmbedBuilder UpdateStateOfPlay()
        {
            StateOfPlay.Fields = new List<EmbedFieldBuilder>();
            for (int i = 0; i < Players.Count; i++)
            {
                string yourTurn = i == PlayerTurnIndex ? " :point_left:" : string.Empty;

                var field = new EmbedFieldBuilder
                {
                    Name = $"{Players[i].Nickname}{yourTurn}",
                    Value = $"**Cards Left:** {Players[i].Hand.Count}"
                };

                StateOfPlay.AddField(field);
            }

            var cardInPlay = DiscardPile.Peek();
            StateOfPlay.AddField(new EmbedFieldBuilder { Name = "Current Card:", Value = cardInPlay.ToString() });

            return StateOfPlay;
        }

        public void EndTurn()
        {
            if (NormalDirection) { PlayerTurnIndex = PlayerTurnIndex >= Players.Count - 1 ? 0 : PlayerTurnIndex + 1; }
            else { PlayerTurnIndex = PlayerTurnIndex == 0 ? Players.Count - 1 : PlayerTurnIndex - 1; }

            UpdateStateOfPlay();
        }

        public bool PlayCardForPlayer(Player player, int card)
        {
            var cardToPlay = player.TakeCard(card, DiscardPile.Peek());
            
            if(cardToPlay == null)
            {
                return false;
            }

            DiscardPile.Push(cardToPlay);

            return true;
        }
    }
}
