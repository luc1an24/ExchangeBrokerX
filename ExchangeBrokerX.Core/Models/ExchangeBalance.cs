namespace ExchangeBrokerX.Core.Models
{
    public class ExchangeBalance
    {
        public required string Exchange { get; set; }
        public double EURBalance { get; set; }
        public double BTCBalance { get; set; }
    }
}
