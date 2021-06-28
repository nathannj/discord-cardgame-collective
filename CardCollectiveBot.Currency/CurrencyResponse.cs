using CardCollectiveBot.Common.Responses;
using System.Collections.Generic;

namespace CardCollectiveBot.Currency
{
    public class CurrencyResponse : IResponse<int?>
    {
        public bool IsSuccess { get; set; }
        public int? Payload { get; set; }
        public IList<Reward> Rewards { get; set; }
        public string[] OtherMessages { get; set; }

        public CurrencyResponse(int? coins, bool isSuccess = true, params string[] otherMessages)
        {
            Payload = coins;
            IsSuccess = isSuccess;
            OtherMessages = otherMessages;
        }
    }
}
