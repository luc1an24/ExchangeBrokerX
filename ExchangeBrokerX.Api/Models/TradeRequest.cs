using System.ComponentModel.DataAnnotations;

namespace ExchangeBrokerX.Api.Models
{
    public class TradeRequest
    {
        [Required]
        [AllowedValues("buy", "sell")]
        public required string OrderType { get; set; }
        public double Amount { get; set; }
    }
}
