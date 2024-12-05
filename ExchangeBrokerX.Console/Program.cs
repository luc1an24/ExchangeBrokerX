using ExchangeBrokerX.Core.Logic;
using ExchangeBrokerX.Core.Utilities;

class Program
{
    static void Main(string[] args)
    {
        var orderBookFilePath = string.Empty;
        while (true)
        {
            Console.Write("Enter the path to the order book file: ");
            orderBookFilePath = Console.ReadLine();

            if (File.Exists(orderBookFilePath))
                break;
            else
                Console.WriteLine("File not found. Please enter a valid file path.");
        }

        var exchangeBalanceFilePath = string.Empty;
        while (true)
        {
            Console.Write("Enter the path to the exchange balance file: ");
            exchangeBalanceFilePath = Console.ReadLine();

            if (File.Exists(exchangeBalanceFilePath))
                break;
            else
                Console.WriteLine("File not found. Please enter a valid file path.");
        }

        Console.Write("Enter the order type (buy / sell): ");
        var orderType = Console.ReadLine()?.Trim().ToLower();

        if (orderType != "buy" && orderType != "sell")
        {
            Console.WriteLine("Invalid order type. Must be 'buy' or 'sell'. Exiting program...");
            return;
        }

        Console.Write("Enter the amount of BTC: ");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount. Must be a positive number. Exiting program...");
            return;
        }

        try
        {
            var orderBooks = FileLoader.LoadOrderBooks(orderBookFilePath);
            var exchangeBalances = FileLoader.LoadExchangeBalances(exchangeBalanceFilePath);
            
            var executionPlan = ExchageBrokerXService.GetBestExecution(orderBooks, exchangeBalances, orderType, amount);

            Console.WriteLine("Best Execution Plan:");
            foreach (var order in executionPlan)
            {
                Console.WriteLine($"Amount: {order.Amount}, Price: {order.Price}, Type: {orderType}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}