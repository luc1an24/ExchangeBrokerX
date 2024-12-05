using ExchangeBrokerX.Api.Models;
using ExchangeBrokerX.Core.Logic;
using ExchangeBrokerX.Core.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeBrokerX.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeBrokerXController : ControllerBase
    {
        [HttpPost("execute")]
        public IActionResult ExecuteTrade([FromBody] TradeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.OrderType) ||
                    (!request.OrderType.Equals("buy", StringComparison.CurrentCultureIgnoreCase) &&
                    !request.OrderType.Equals("sell", StringComparison.CurrentCultureIgnoreCase)))
                {
                    return BadRequest("Invalid order type. Must be 'buy' or 'sell'.");
                }

                if (request.Amount <= 0)
                {
                    return BadRequest("Amount must be a positive number.");
                }

                var orderBook = FileLoader.LoadOrderBooks("orderBooks.json");
                var exchangeBalances = FileLoader.LoadExchangeBalances("exchangesBalance.json");

                var executionPlan = ExchangeBrokerXService.GetBestExecution(orderBook, exchangeBalances, request.OrderType, request.Amount);

                return Ok(executionPlan);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
