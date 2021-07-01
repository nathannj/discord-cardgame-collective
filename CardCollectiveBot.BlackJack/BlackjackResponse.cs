using CardCollectiveBot.Common.Responses;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.BlackJack
{
    public class BlackjackResponse : IResponse<EmbedBuilder>
    {
        public bool IsSuccess { get; set; }
        public EmbedBuilder Payload { get; set; }
        public IList<Reward> Rewards { get; set; }
        public string[] OtherMessages { get; set; }

        public BlackjackResponse(EmbedBuilder responseMessage, bool isSuccess = true, IList<Reward> rewards = null, params string[] otherMessages)
        {
            Payload = responseMessage;
            IsSuccess = isSuccess;
            OtherMessages = otherMessages;
            Rewards = rewards;
        }
    }
}