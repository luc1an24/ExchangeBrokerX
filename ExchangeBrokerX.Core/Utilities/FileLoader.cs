using ExchangeBrokerX.Core.Models;
using Newtonsoft.Json;

namespace ExchangeBrokerX.Core.Utilities
{
    public static class FileLoader
    {
        public static List<OrderBook> LoadOrderBooks(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"The file {filePath} does not exist.");

                var json = File.ReadAllText(filePath);

                var orderBooks = JsonConvert.DeserializeObject<List<OrderBook>>(json);

                if (orderBooks == null)
                    throw new InvalidDataException("The file content is not a valid list of order books.");

                return orderBooks;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error: Failed to parse the order book JSON.");
                Console.WriteLine($"Details: {ex.Message}");
                throw new InvalidDataException("Invalid JSON format in the order book file.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred while loading the order book.");
                Console.WriteLine($"Details: {ex.Message}");
                throw new ApplicationException("Failed to load the order book due to an unexpected error.", ex);
            }
        }

        public static List<ExchangeBalance> LoadExchangeBalances(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"The file {filePath} does not exist.");

                var json = File.ReadAllText(filePath);

                var exchangeBalances = JsonConvert.DeserializeObject<List<ExchangeBalance>>(json);

                if (exchangeBalances == null)
                    throw new InvalidDataException("The file content is not a valid list of exchange balances.");

                return exchangeBalances;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error: Failed to parse the exchange balance JSON.");
                Console.WriteLine($"Details: {ex.Message}");
                throw new InvalidDataException("Invalid JSON format in the exchange balance file.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred while loading the exchange balance.");
                Console.WriteLine($"Details: {ex.Message}");
                throw new ApplicationException("Failed to load the exchange balance due to an unexpected error.", ex);
            }
        }
    }
}
