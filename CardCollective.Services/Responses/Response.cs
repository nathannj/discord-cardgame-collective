using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.Common.Responses
{
    public class Response<T> : IResponse<T> where T : new()
    {
        public bool IsSuccess { get; set; }
        public T Payload { get; set; }
        public IList<Reward> Rewards { get; set; }
        public string[] OtherMessages { get; set; }

        public Response(T responseMessage, bool isSuccess = true, IList<Reward> rewards = null, params string[] otherMessages)
        {
            Payload = responseMessage;
            IsSuccess = isSuccess;
            OtherMessages = otherMessages;
            Rewards = rewards;
        }
    }
}
