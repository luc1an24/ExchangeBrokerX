using ExchangeBrokerX.Core.Models;
using Newtonsoft.Json;

namespace ExchangeBrokerX.Core.Utilities
{
    public static class FileLoader
    {
        public static OrderBook LoadOrderBook(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"The file {filePath} does not exist.");

                var json = File.ReadAllText(filePath);

                var orderBook = JsonConvert.DeserializeObject<OrderBook>(json);

                if (orderBook == null)
                    throw new InvalidDataException("The file content is not a valid order book.");

                return orderBook;
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
    }
}
