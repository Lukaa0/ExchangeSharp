using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class CancelOrdersResponse
	{
		[JsonProperty("orderId")]
		public string OrderId { get; set; }
		[JsonProperty("clientOrderId")]
		public string ClientOrderId { get; set; }
		[JsonProperty("state")]
		public string State { get; set; }
		[JsonProperty("code")]
		public int Code { get; set; }
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}
