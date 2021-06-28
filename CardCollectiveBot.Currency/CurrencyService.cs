using CardCollectiveBot.Common.Responses;
using CardCollectiveBot.Data;
using System;

namespace CardCollectiveBot.Currency
{
    public class CurrencyService : ICurrencyService
    {
        private CardCollectiveBotContext Context { get; set; }

        public CurrencyService(CardCollectiveBotContext context)
        {
            Context = context;
        }

        public IResponse<int?> DepositCoins(ulong id, int deposit)
        {
            try
            {
                var userCurrency = Context.Currency.Find(id);

                userCurrency.Mangoes += deposit;

                Context.SaveChanges();

                return new CurrencyResponse(userCurrency.Mangoes);
            }
            catch (Exception e)
            {
                return new CurrencyResponse(null, false, otherMessages: e.Message);
            }
        }

        public IResponse<int?> WithdrawCoins(ulong id, int withdrawalAmount)
        {
            try
            {
                var userCurrency = Context.Currency.Find(id);

                userCurrency.Mangoes -= withdrawalAmount;

                Context.SaveChanges();

                return new CurrencyResponse(userCurrency.Mangoes);
            }
            catch (Exception e)
            {
                return new CurrencyResponse(null, false, otherMessages: e.Message);
            }
        }

        public IResponse<int?> GetCoins(ulong id)
        {
            var userCurrency = Context.Currency.Find(id);

            return new CurrencyResponse(userCurrency?.Mangoes, userCurrency != null);
        }

        public IResponse<int?> CreateAccount(ulong id)
        {
            try
            {
                var entity = Context.Currency.Find(id);

                if (entity == null)
                {
                    entity = new Data.Entities.Currency { UserId = id, Mangoes = 1000, DateModified = DateTime.Now };
                    Context.Add(entity);
                }

                return new CurrencyResponse(entity.Mangoes, Context.SaveChanges() > 0);
            }
            catch (Exception e)
            {
                return new CurrencyResponse(null, false, e.Message);
            }
        }
    }
}
