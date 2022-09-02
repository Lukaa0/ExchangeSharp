using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public sealed class OpenOrdersResponse
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("clientOrderId")]
		public string ClientOrderId { get; set; }
		[JsonProperty("symbol")]
		public string Symbol { get; set; }
		[JsonProperty("state")]
		public string State { get; set; }
		[JsonProperty("accountType")]
		public string AccountType { get; set; }
		[JsonProperty("side")]
		public string Side { get; set; }
		[JsonProperty("type")]
		public string Type { get; set; }
		[JsonProperty("timeInForce")]
		public string TimeInForce { get; set; }
		[JsonProperty("quantity")]
		public string Quantity { get; set; }
		[JsonProperty("price")]
		public string Price { get; set; }
		[JsonProperty("avgPrice")]
		public string AvgPrice { get; set; }
		[JsonProperty("amount")]
		public string Amount { get; set; }
		[JsonProperty("filledQuantity")]
		public string FilledQuantity { get; set; }
		[JsonProperty("filledAmount")]
		public string FilledAmount { get; set; }
		[JsonProperty("createTime")]
		public object CreateTime { get; set; }
		[JsonProperty("updateTime")]
		public object UpdateTime { get; set; }
	}
}
