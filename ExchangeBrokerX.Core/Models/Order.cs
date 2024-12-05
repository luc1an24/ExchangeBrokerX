namespace ExchangeBrokerX.Core.Models
{
    public class Order
    {
        public required string Type { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
    }
}
