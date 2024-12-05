using ExchangeBrokerX.Core.Logic;
using ExchangeBrokerX.Core.Models;

namespace Tests.ExchangeBrokerX.Services
{
    public class ExchangeBrokerXServiceTests
    {
        // buy 2 BTC, mutliple exchanges used
        [Fact]
        public void GetBestExecution_BuyOrder_Success()
        {
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    Exchange = 1,
                    Asks = new List<Order>
                    {
                        new Order { Type = "sell", Price = 30000, Amount = 1 },
                        new Order { Type = "sell", Price = 31000, Amount = 1.5 }
                    },
                    Bids = new List<Order>()
                },
                new OrderBook
                {
                    Exchange = 2,
                    Asks = new List<Order>
                    {
                        new Order { Type = "sell", Price = 29000, Amount = 0.5 },
                        new Order { Type = "sell", Price = 30500, Amount = 1 }
                    },
                    Bids = new List<Order>()
                }
            };

            var balances = new List<ExchangeBalance>
            {
                new ExchangeBalance { Exchange = 1, EURBalance = 100000, BTCBalance = 10 },
                new ExchangeBalance { Exchange = 2, EURBalance = 50000, BTCBalance = 5 }
            };

            var result = ExchangeBrokerXService.GetBestExecution(orderBooks, balances, "buy", 2);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(29000, result[0].Price);
            Assert.Equal(0.5, result[0].Amount);
            Assert.Equal(30000, result[1].Price);
            Assert.Equal(1, result[1].Amount);
            Assert.Equal(30500, result[2].Price);
            Assert.Equal(0.5, result[2].Amount);
        }

        // sell 2 BTC, exchange doesn't have liquidity to sell it
        [Fact]
        public void GetBestExecution_SellOrder_InsufficientLiquidity()
        {
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    Exchange = 1,
                    Bids = new List<Order>
                    {
                        new Order { Type = "buy", Price = 31000, Amount = 1 }
                    },
                    Asks = new List<Order>()
                }
            };

            var balances = new List<ExchangeBalance>
            {
                new ExchangeBalance { Exchange = 1, EURBalance = 5000, BTCBalance = 10 }
            };

            var exception = Assert.Throws<Exception>(() =>
                ExchangeBrokerXService.GetBestExecution(orderBooks, balances, "sell", 2)
            );

            Assert.Equal("Insufficient liquidity to fulfill the sell order for 2 BTC.", exception.Message);
        }

        // buy 3 BTC, but exchanges have combined 2.5 BTC
        [Fact]
        public void GetBestExecution_BuyOrder_LimitedLiquidity()
        {
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    Exchange = 1,
                    Asks = new List<Order>
                    {
                        new Order { Type = "sell", Price = 29000, Amount = 0.5 },
                        new Order { Type = "sell", Price = 29500, Amount = 1.5 }
                    },
                    Bids = new List<Order>()
                },
                new OrderBook
                {
                    Exchange = 2,
                    Asks = new List<Order>
                    {
                        new Order { Type = "sell", Price = 30000, Amount = 0.5 }
                    },
                    Bids = new List<Order>()
                }
            };

            var balances = new List<ExchangeBalance>
            {
                new ExchangeBalance { Exchange = 1, EURBalance = 100000, BTCBalance = 10 },
                new ExchangeBalance { Exchange = 2, EURBalance = 50000, BTCBalance = 5 }
            };


            var exception = Assert.Throws<Exception>(() =>
                ExchangeBrokerXService.GetBestExecution(orderBooks, balances, "buy", 3)
            );

            Assert.Equal("Insufficient liquidity to fulfill the buy order for 3 BTC.", exception.Message);
        }

        // sell 2 BTC, but exchange can't pay for more than 1 BTC becaus of insufficient EUR balance
        [Fact]
        public void GetBestExecution_SellOrder_EURConstraint()
        {
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    Exchange = 1,
                    Asks = new List<Order>(),
                    Bids = new List<Order>
                    {
                        new Order { Type = "buy", Price = 31000, Amount = 1 }
                    }
                },
                new OrderBook
                {
                    Exchange = 2,
                    Asks = new List<Order>(),
                    Bids = new List<Order>
                    {
                        new Order { Type = "buy", Price = 30500, Amount = 1 }
                    }
                }
            };

            var balances = new List<ExchangeBalance>
            {
                new ExchangeBalance { Exchange = 1, EURBalance = 31000, BTCBalance = 10 },
                new ExchangeBalance { Exchange = 2, EURBalance = 20000, BTCBalance = 5 }
            };

            var exception = Assert.Throws<Exception>(() =>
                ExchangeBrokerXService.GetBestExecution(orderBooks, balances, "sell", 2)
            );

            Assert.Equal("Insufficient liquidity to fulfill the sell order for 2 BTC.", exception.Message);

            balances = new List<ExchangeBalance>
            {
                new ExchangeBalance { Exchange = 1, EURBalance = 31000, BTCBalance = 10 },
                new ExchangeBalance { Exchange = 2, EURBalance = 20000, BTCBalance = 5 }
            };
            var result = ExchangeBrokerXService.GetBestExecution(orderBooks, balances, "sell", 1.5);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(31000, result[0].Price);
            Assert.Equal(1, result[0].Amount);
            Assert.Equal(30500, result[1].Price);
            Assert.Equal(0.5, result[1].Amount);
        }

        // buy 2 BTC on multiple exchanges
        [Fact]
        public void GetBestExecution_BuyOrder_MultipleExchanges()
        {
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    Exchange = 1,
                    Asks = new List<Order>
                    {
                        new Order { Type = "sell", Price = 30000, Amount = 1 },
                        new Order { Type = "sell", Price = 31000, Amount = 1 }
                    },
                    Bids = new List<Order>()
                },
                new OrderBook
                {
                    Exchange = 2,
                    Asks = new List<Order>
                    {
                        new Order { Type = "sell", Price = 29000, Amount = 0.5 },
                        new Order { Type = "sell", Price = 30500, Amount = 0.5 }
                    },
                    Bids = new List<Order>()
                }
            };

            var balances = new List<ExchangeBalance>
            {
                new ExchangeBalance { Exchange = 1, EURBalance = 100000, BTCBalance = 10 },
                new ExchangeBalance { Exchange = 2, EURBalance = 50000, BTCBalance = 5 }
            };

            var result = ExchangeBrokerXService.GetBestExecution(orderBooks, balances, "buy", 2);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(29000, result[0].Price);
            Assert.Equal(0.5, result[0].Amount);
            Assert.Equal(30000, result[1].Price);
            Assert.Equal(1, result[1].Amount);
            Assert.Equal(30500, result[2].Price);
            Assert.Equal(0.5, result[2].Amount);
        }

        // sell 1 BTC to an exchange that has exactly enough EUR to pay for it
        [Fact]
        public void GetBestExecution_SellOrder_ExactMatch()
        {
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    Exchange = 1,
                    Asks = new List<Order>(),
                    Bids = new List<Order>
                    {
                        new Order { Type = "buy", Price = 32000, Amount = 1 }
                    }
                }
            };

            var balances = new List<ExchangeBalance>
            {
                new ExchangeBalance { Exchange = 1, EURBalance = 32000, BTCBalance = 10 }
            };

            var result = ExchangeBrokerXService.GetBestExecution(orderBooks, balances, "sell", 1);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(32000, result[0].Price);
            Assert.Equal(1, result[0].Amount);
        }

        // buy 1 BTC from a single exchange with sufficient BTC available.
        [Fact]
        public void GetBestExecution_BuyOrder_SingleExchange()
        {
            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    Exchange = 1,
                    Asks = new List<Order>
                    {
                        new Order { Type = "sell", Price = 30000, Amount = 2 }
                    },
                    Bids = new List<Order>()
                }
            };

            var balances = new List<ExchangeBalance>
            {
                new ExchangeBalance { Exchange = 1, EURBalance = 100000, BTCBalance = 10 }
            };

            var result = ExchangeBrokerXService.GetBestExecution(orderBooks, balances, "buy", 1);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(30000, result[0].Price);
            Assert.Equal(1, result[0].Amount);
        }
    }
}

