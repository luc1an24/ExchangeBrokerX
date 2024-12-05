namespace ExchangeBrokerX.Core.Models
{
    public class OrderBook
    {
        public int Exchange { get; set; }
        public List<Order> Bids { get; set; } = new List<Order>();
        public List<Order> Asks { get; set; } = new List<Order>();
    }
}
