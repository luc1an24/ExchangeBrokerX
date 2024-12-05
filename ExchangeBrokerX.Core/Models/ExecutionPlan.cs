namespace ExchangeBrokerX.Core.Models
{
    public class ExecutionPlan
    {
        public required string Exchange { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
    }
}
