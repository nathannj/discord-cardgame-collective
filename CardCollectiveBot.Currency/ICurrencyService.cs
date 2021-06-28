using CardCollectiveBot.Common.Responses;

namespace CardCollectiveBot.Currency
{
    public interface ICurrencyService
    {
        IResponse<int?> DepositCoins(ulong id, int deposit);
        IResponse<int?> GetCoins(ulong id);
        IResponse<int?> WithdrawCoins(ulong id, int withdrawalAmount);
        IResponse<int?> CreateAccount(ulong id);
    }
}