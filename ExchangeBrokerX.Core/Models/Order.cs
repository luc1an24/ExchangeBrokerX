namespace ExchangeBrokerX.Core.Models
{
    public class Order
    {
        public required string Type { get; set; }
        public float Amount { get; set; }
        public float Price { get; set; }
    }
}
