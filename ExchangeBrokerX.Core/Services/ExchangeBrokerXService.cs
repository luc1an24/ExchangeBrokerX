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
            if (orderType.ToLower() != "buy" && orderType.ToLower() != "sell")
                throw new Exception($"Unknown order type: {orderType}");

            var executionPlan = new List<ExecutionPlan>();

            var isBuyOrder = orderType.ToLower() == "buy";
            var remainingAmount = amount;

            var aggregatedOrders = orderBooks
                .SelectMany(orderBook => isBuyOrder
                ? orderBook.Asks.Select(ask => (orderBook.Exchange, ask.Price, ask.Amount, "sell"))
                : orderBook.Bids.Select(bid => (orderBook.Exchange, bid.Price, bid.Amount, "buy")))
                .OrderBy(order => isBuyOrder ? order.Price : -order.Price)
                .ToList();

            foreach (var order in aggregatedOrders)
            {
                if (remainingAmount <= 0) break;

                if (isBuyOrder)
                {
                    var tradeAmount = Math.Min(order.Amount, remainingAmount);

                    if (tradeAmount > 0)
                    {
                        executionPlan.Add(new ExecutionPlan
                        {
                            Exchange = order.Exchange,
                            Price = order.Price,
                            Amount = tradeAmount
                        });

                        remainingAmount -= tradeAmount;
                    }
                }
                else
                {
                    var exchangeBalance = balances.FirstOrDefault(b => b.Exchange == order.Exchange);

                    if (exchangeBalance == null)
                        continue;

                    var maxAmountCanSell = exchangeBalance.EURBalance / order.Price;
                    var tradeAmount = Math.Min(order.Amount, Math.Min(maxAmountCanSell, remainingAmount));

                    if (tradeAmount > 0)
                    {
                        executionPlan.Add(new ExecutionPlan
                        {
                            Exchange = order.Exchange,
                            Price = order.Price,
                            Amount = tradeAmount
                        });

                        remainingAmount -= tradeAmount;
                        exchangeBalance.EURBalance -= tradeAmount * order.Price;
                    }
                }
            }

            if (remainingAmount > 0)
            {
                throw new Exception($"Insufficient liquidity to fulfill the {orderType} order for {amount} BTC.");
            }

            return executionPlan;
        }
    }
}
