using ExchangeBrokerX.Core.Models;

namespace ExchangeBrokerX.Core.Logic
{
    public class ExchangeBrokerXService
    {
        public static List<ExecutionPlan> GetBestExecution(
            List<OrderBook> orderBooks,
            List<ExchangeBalance> balances,
            string orderType,
            double amount)
        {
            var executionPlan = new List<ExecutionPlan>();

            if (orderType == "buy")
            {
                foreach (var book in orderBooks)
                {
                    foreach (var ask in book.Asks)
                    {
                        if (amount <= 0) break;

                        var tradeAmount = ask.Amount > amount ? amount : ask.Amount;
                        executionPlan.Add(new ExecutionPlan
                        {
                            Exchange = book.Exchange,
                            Amount = tradeAmount,
                            Price = ask.Price
                        });
                        amount -= tradeAmount;
                    }
                }
            }
            else if (orderType == "sell")
            {
                foreach (var book in orderBooks)
                {
                    foreach (var bid in book.Bids)
                    {
                        if (amount <= 0) break;

                        var tradeAmount = bid.Amount > amount ? amount : bid.Amount;
                        executionPlan.Add(new ExecutionPlan
                        {
                            Exchange = book.Exchange,
                            Amount = tradeAmount,
                            Price = bid.Price
                        });
                        amount -= tradeAmount;
                    }
                }
            }

            return executionPlan;
        }
    }
}
