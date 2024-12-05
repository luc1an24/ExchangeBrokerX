using ExchangeBrokerX.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExchangeBrokerX.Core.Utils
{
    internal class OrderBookConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderBook);
        }

        public override OrderBook ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var orderBook = new OrderBook();

            try
            {
                // Load the JSON into a JObject
                var jObject = JObject.Load(reader);

                // Deserialize Exchange ID
                if (jObject.TryGetValue("Exchange", out var exchangeToken) && exchangeToken.Type == JTokenType.Integer)
                {
                    orderBook.Exchange = exchangeToken.Value<int>();
                }
                else
                {
                    throw new JsonSerializationException("Invalid or missing 'Exchange' field in OrderBook JSON.");
                }

                // Deserialize Bids
                if (jObject.TryGetValue("Bids", out var bidsToken) && bidsToken.Type == JTokenType.Array)
                {
                    orderBook.Bids = DeserializeOrders(bidsToken, serializer);
                }
                else
                {
                    throw new JsonSerializationException("Invalid or missing 'Bids' field in OrderBook JSON.");
                }
                
                // Deserialize Asks
                if (jObject.TryGetValue("Asks", out var asksToken) && asksToken.Type == JTokenType.Array)
                {
                    orderBook.Asks = DeserializeOrders(asksToken, serializer);
                }
                else
                {
                    throw new JsonSerializationException("Invalid or missing 'Asks' field in OrderBook JSON.");
                }
            }
            catch (JsonException ex)
            {
                throw new JsonSerializationException($"Error deserializing OrderBook: {ex.Message}", ex);
            }

            return orderBook;
        }

        private List<Order> DeserializeOrders(JToken ordersToken, JsonSerializer serializer)
        {
            var orders = new List<Order>();

            foreach (var orderWrapper in ordersToken)
            {
                if (orderWrapper["Order"] is JObject orderObject)
                {
                    try
                    {
                        var order = orderObject.ToObject<Order>(serializer);
                        if (order != null)
                        {
                            orders.Add(order);
                        }
                    }
                    catch (JsonException ex)
                    {
                        throw new JsonSerializationException("Error deserializing an individual order.", ex);
                    }
                }
                else
                {
                    throw new JsonSerializationException("Expected 'Order' object in order wrapper.");
                }
            }

            return orders;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Serialization is not supported in this example.");
        }
    }
}
