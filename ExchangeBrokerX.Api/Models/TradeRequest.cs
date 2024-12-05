namespace ExchangeBrokerX.Api.Models
{
    public class TradeRequest
    {
        public required string OrderType { get; set; }
        public double Amount { get; set; }
    }
}
