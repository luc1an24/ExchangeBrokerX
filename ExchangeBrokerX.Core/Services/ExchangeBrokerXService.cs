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

            var aggregatedOrders = new List<(int Exchange, double Price, double Amount, string Type)>();

            foreach (var orderBook in orderBooks)
            {
                var exchange = orderBook.Exchange;

                if (isBuyOrder)
                {
                    aggregatedOrders.AddRange(orderBook.Asks.Select(f => (exchange, f.Price, f.Amount, "sell")));
                }
                else
                {
                    aggregatedOrders.AddRange(orderBook.Bids.Select(f => (exchange, f.Price, f.Amount, "buy")));
                }
            }

            aggregatedOrders = isBuyOrder ? aggregatedOrders.OrderBy(f => f.Price).ToList() : aggregatedOrders.OrderByDescending(f => f.Price).ToList();

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
