using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.Common.Responses
{
    public interface IResponse<T> where T : new()
    {
        public bool IsSuccess { get; set; }

        public T Payload { get; set; }

        public IList<Reward> Rewards { get; set; }

        public string[] OtherMessages { get; set; }
    }
}
