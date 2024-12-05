using ExchangeBrokerX.Core.Utilities;

namespace Tests.ExchangeBrokerX.Utils
{
    public class FileLoaderTests
    {
        [Fact]
        public void LoadOrderBook_ValidFile_Success()
        {
            var filePath = "testOrderBook.json";
            var jsonContent = @"[
            {
                ""Exchange"": 1,
                ""Asks"": [{ ""Price"": 30000, ""Amount"": 1 }],
                ""Bids"": [{ ""Price"": 31000, ""Amount"": 1 }]
            },
            {
                ""Exchange"": 2,
                ""Asks"": [{ ""Price"": 30000, ""Amount"": 1 }],
                ""Bids"": [{ ""Price"": 31000, ""Amount"": 1 }]
            }]";

            File.WriteAllText(filePath, jsonContent);

            var orderBooks = FileLoader.LoadOrderBooks(filePath);

            Assert.NotNull(orderBooks);
            Assert.Equal(2, orderBooks.Count);
            Assert.Equal(1, orderBooks[0].Exchange);
            Assert.Single(orderBooks[0].Asks);
            Assert.Equal(30000, orderBooks[0].Asks[0].Price);
            Assert.Equal(1, orderBooks[0].Asks[0].Amount);
            Assert.Single(orderBooks[0].Bids);
            Assert.Equal(31000, orderBooks[0].Bids[0].Price);
            Assert.Equal(1, orderBooks[0].Bids[0].Amount);

            File.Delete(filePath);
        }

        [Fact]
        public void LoadOrderBook_FileNotFound_ThrowsException()
        {
            var filePath = "nonExistentFile.json";

            Assert.Throws<FileNotFoundException>(() =>
                FileLoader.LoadOrderBooks(filePath)
            );
        }

        [Fact]
        public void LoadExchangeBalances_ValidFile_Success()
        {
            var filePath = "testExchangeBalance.json";
            var jsonContent = @"[
            {
                ""Exchange"": 1,
                ""EURBalance"": 55000.48,
                ""BTCBalance"": 18.2
            },
            {
                ""Exchange"": 2,
                ""EURBalance"": 150.48,
                ""BTCBalance"": 0.1
            }]";

            File.WriteAllText(filePath, jsonContent);

            var exchangeBalances = FileLoader.LoadExchangeBalances(filePath);

            Assert.NotNull(exchangeBalances);
            Assert.Equal(2, exchangeBalances.Count);
            Assert.Equal(1, exchangeBalances[0].Exchange);
            Assert.Equal(55000.48, exchangeBalances[0].EURBalance);
            Assert.Equal(18.2, exchangeBalances[0].BTCBalance);

            File.Delete(filePath);
        }
    }
}
